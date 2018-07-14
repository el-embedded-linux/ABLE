using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Text;
using UnityEngine.Events;
#if (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS)
using AOT;
using System.Runtime.InteropServices;
#endif


namespace Ardunity
{
    [AddComponentMenu("ARDUnity/CommSocket/HM10")]
	[HelpURL("https://sites.google.com/site/ardunitydoc/references/commsocket/hm10")]
    public class HM10 : CommSocket
    {
		public string serviceUUID = "FFE0";
		public string charUUID = "FFE1";

        public float searchTimeout = 5f;
    
        private bool _isBleOpen = false;
		private bool _bleOpenTry = false;
        private static bool _isSupport = true;
        private float _searchTimeout = 0f;        
        private bool _threadOnOpen = false;
        private bool _threadOnOpenFailed = false;
        private bool _threadOnStartSearch = false;
        private bool _threadOnStopSearch = false;
        private bool _threadOnFoundDevice = false;
        private bool _threadOnErrorClosed = false;
        private bool _threadOnWriteCompleted = false;
        private Thread _openThread;
        private List<byte> _txBuffer = new List<byte>();
        private List<byte> _rxBuffer = new List<byte>();
        private bool _txWait = false;
        private bool _getCompleted = false;
		private string _serviceUUID;
		private string _charUUID;

#if UNITY_ANDROID
        private AndroidJavaObject _android = null;
        
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
        private static bool _bleInitialized = false;        
        private static List<HM10> _commBleList = new List<HM10>();        
	    private delegate void UnityCallbackDelegate(IntPtr arg1, IntPtr arg2);
        
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleInitialize([MarshalAs(UnmanagedType.FunctionPtr)]UnityCallbackDelegate unityCallback);       
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDeinitialize();       
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleStartScan(string service);       
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleStopScan();
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleConnect(string uuid);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDisconnect(string uuid);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDiscoverService(string uuid, string service);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDiscoverCharacteristic(string uuid, string service, string characteristic);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleWrite(string uuid, string service, string characteristic, byte[] data, int length, bool withResponse);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleRead(string uuid, string service, string characteristic);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleSubscribe(string uuid, string service, string characteristic);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleUnsubscribe(string uuid, string service, string characteristic);

#endif
        
        protected override void Awake()
		{
            base.Awake();

#if UNITY_ANDROID
			_serviceUUID = string.Format("0000{0}-0000-1000-8000-00805f9b34fb", serviceUUID);
			_charUUID = string.Format("0000{0}-0000-1000-8000-00805f9b34fb", charUUID);

			try
            {
                AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.ardunity.android.BluetoothLE");
                _android = pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
                _isSupport = _android.Call<bool>("Initialize", activityContext, gameObject.name, "BleCallback");
                if (_isSupport == false)
                    _android = null;
            }
            catch(Exception)
            {
                _android = null;
            }

            if(_android == null)
                Debug.Log("Android BLE Failed!");
                
#elif (UNITY_STANDALONE_OSX|| UNITY_IOS)
			_serviceUUID = serviceUUID;
			_charUUID = charUUID;

			_commBleList.Add(this);
            if(_commBleList.Count == 1)
		        bleInitialize(BleCallbackDelegate);
#endif
        }
        
        // Update is called once per frame
        void Update ()
        {
            if(_threadOnOpen)
            {
                OnOpen.Invoke();
                _threadOnOpen = false;
            }
            if (_threadOnOpenFailed)
            {
                ErrorClose();
                OnOpenFailed.Invoke();
                _threadOnOpenFailed = false;
            }
            if (_threadOnErrorClosed)
            {
                ErrorClose();
                OnErrorClosed.Invoke();
                _threadOnErrorClosed = false;
            }
            if (_threadOnStartSearch)
            {
                OnStartSearch.Invoke();
                _threadOnStartSearch = false;
            }
            if (_threadOnStopSearch)
            {
                OnStopSearch.Invoke();
                _threadOnStopSearch = false;
            }
            if (_threadOnFoundDevice)
            {
                OnFoundDevice.Invoke(new CommDevice(foundDevices[foundDevices.Count - 1]));
                _threadOnFoundDevice = false;
            }
            if (_threadOnWriteCompleted)
            {
                OnWriteCompleted.Invoke();
                _threadOnWriteCompleted = false;
            }
            
            if (_searchTimeout > 0f)
            {
                _searchTimeout -= Time.deltaTime;
                if (_searchTimeout <= 0f)
                    StopSearch();
            }
        }
        
        void LateUpdate()
        {
            if(IsOpen && !_txWait)
                txWrite();
        }
        
        void OnDestroy()
        {
#if UNITY_ANDROID

#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            ErrorClose();
            _commBleList.Remove(this);
            if(_commBleList.Count == 0)
		        bleDeinitialize();            
#endif
        }
        
        public bool isSupport
        {
            get
            {
#if UNITY_ANDROID
                return _isSupport;
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if(_bleInitialized)
                    return _isSupport;
                else
                    return false;
#else
                return false;
#endif
            }            
        }
        
        public bool isSearching
        {
            get
            {
                if(_searchTimeout > 0f)
                    return true;
                else
                    return false;
            }
        }
        
        #region Override
        public override void Open()
        {
            StopSearch();
            
            if (IsOpen)
                return;

            _openThread = new Thread(openThread);
            _openThread.Start();
        }

        public override void Close()
        {
            if (!IsOpen)
                return;

            ErrorClose();
            OnClose.Invoke();
        }

        protected override void ErrorClose()
        {
            if (_openThread != null)
            {
                if (_openThread.IsAlive)
                    _openThread.Abort();
            }
            
            if(_isBleOpen)
            {
 #if UNITY_ANDROID
                if (_android != null)
                    _android.Call("UnsubscribeCharacteristic", _serviceUUID, _charUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if (_bleInitialized)
                    bleUnsubscribe(device.address, _serviceUUID, _charUUID);
#endif
            }
            
			_rxBuffer.Clear();
            
#if UNITY_ANDROID
            if (_android != null)
                _android.Call("Disconnect");                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)            
            if (_bleInitialized)
                bleDisconnect(device.address);
#endif
            _isBleOpen = false;
            _bleOpenTry = false;
        }

        public override bool IsOpen
        {
            get
            {
                return _isBleOpen;
            }
        }

        public override void StartSearch()
        {
            _searchTimeout = searchTimeout;

#if UNITY_ANDROID
            if (_android != null)
                _android.Call("StartScan", _serviceUUID);                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
                bleStartScan(_serviceUUID);
#endif
        }
        
        public override void StopSearch()
        {
            _searchTimeout = 0f;
            
#if UNITY_ANDROID
            if (_android != null)
                _android.Call("StopScan");                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
                bleStopScan();
#endif
        }

        public override void Write(byte[] data, bool getCompleted = false)
        {         
            if (data == null)
                return;
            if (data.Length == 0)
                return;
            
            _txBuffer.AddRange(data);
            _getCompleted = getCompleted;
        }
        
        private void txWrite()
        {         
            if(_txBuffer.Count == 0)
            {
                _txWait = false;
                if(_getCompleted)
                    _threadOnWriteCompleted = true;
                return;
            }
            _txWait = true;
            
            byte[] data20 = new byte[Mathf.Min(20, _txBuffer.Count)];
            for(int i=0; i<data20.Length; i++)
                data20[i] = _txBuffer[i];
            
            _txBuffer.RemoveRange(0, data20.Length);            
#if UNITY_ANDROID
           if (_android != null)
                _android.Call("Write", _serviceUUID, _charUUID, data20, true);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
           if (_bleInitialized)
                bleWrite(device.address, _serviceUUID, _charUUID, data20, data20.Length, false);
#endif
        }

        public override byte[] Read()
        {
            if(_rxBuffer.Count > 0)
            {
                byte[] bytes = _rxBuffer.ToArray();
                _rxBuffer.Clear();
                return bytes;
            }
            else
                return null;
        }
        #endregion

        private void openThread()
        {
#if UNITY_ANDROID
            AndroidJNI.AttachCurrentThread();
#endif
			_bleOpenTry = false;
            _txBuffer.Clear();
            _txWait = false;
            
#if UNITY_ANDROID
            if (_android != null)
            {
                _android.Call("Connect", device.address);
				_bleOpenTry = true;
            }
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
            {
                bleConnect(device.address);
				_bleOpenTry = true;
            }
#endif

			if(!_bleOpenTry)
			{
				_bleOpenTry = false;
				_threadOnOpenFailed = true;
			}

#if UNITY_ANDROID
            AndroidJNI.DetachCurrentThread();
#endif
            _openThread.Abort();
            return;
        }

#if (UNITY_STANDALONE_OSX || UNITY_IOS)
        [MonoPInvokeCallback(typeof(UnityCallbackDelegate))]
        private static void BleCallbackDelegate(IntPtr arg1, IntPtr arg2)
        {
            string uuid = Marshal.PtrToStringAuto(arg1);
            string message = Marshal.PtrToStringAuto(arg2);

            if(_commBleList.Count > 0)
            {
                for(int i=0; i<_commBleList.Count; i++)
                {
                    if(uuid == null)
                        _commBleList[i].BleCallback(message);
                    else
                    {
                        if(_commBleList[i].device.address.Equals(uuid))
                            _commBleList[i].BleCallback(message);
                    }
                }
            }
            else
                Debug.Log(message);            
        }
#endif
        
        private void BleCallback(string message)
        {
            if (message == null)
                return;
            
            string[] tokens = message.Split(new char[] { '~' });
            if(tokens.Length == 0)
                return;
            
            if(tokens[0].Equals("Initialized"))
            {
                Debug.Log("BLE Initialized");
#if (UNITY_STANDALONE_OSX || UNITY_IOS)                
                _bleInitialized = true;
#endif
            }
            else if(tokens[0].Equals("Deinitialized"))
            {
                Debug.Log("BLE Deinitialized");
#if (UNITY_STANDALONE_OSX || UNITY_IOS)                
                _bleInitialized = false;
#endif
            }
            else if(tokens[0].Equals("NotSupported"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE not supported");
                _isSupport = false;
#endif
            }
            else if(tokens[0].Equals("PoweredOff"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Power Off");
#endif
            }
            else if(tokens[0].Equals("PoweredOn"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Power On");
#endif
            }
            else if(tokens[0].Equals("Unauthorized"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Unauthorized");
#endif
            }
            else if(tokens[0].Equals("StateUnknown"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Unauthorized");
#endif
            }
            else if(tokens[0].Equals("StartScan"))
            {
                Debug.Log("HM10 Start Scanning");
                foundDevices.Clear();
                _threadOnStartSearch = true;
            }
            else if(tokens[0].Equals("StopScan"))
            {
                Debug.Log("HM10 Stop Scanning");
                _threadOnStopSearch = true;
            }
            else if(tokens[0].Equals("ConnectFailed"))
            {
                Debug.Log("BLE GATT Connect Failed");
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("DiscoveredDevice"))
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = tokens[1];
                foundDevice.address = tokens[2];
                
                for(int i=0; i<foundDevices.Count; i++)
                {
                    if (foundDevices[i].Equals(foundDevice))
                        return;
                }
                
                foundDevices.Add(foundDevice);
                _threadOnFoundDevice = true;
            }
            else if(tokens[0].Equals("Disconnected"))
            {
                Debug.Log("BLE GATT Disconnected");
                
				if(_isBleOpen)
					_threadOnErrorClosed = true;
				else if(_bleOpenTry)
					_threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("Connected"))
            {
                Debug.Log("BLE GATT Connected");
              
#if UNITY_ANDROID
                if (_android != null)
                    _android.Call("DiscoverService", _serviceUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if (_bleInitialized)
                    bleDiscoverService(device.address, _serviceUUID);
#endif
            }
            else if(tokens[0].Equals("DiscoveredService"))
            {
                Debug.Log("HM10 Discovered Service");
                              
#if UNITY_ANDROID
                if (_android != null)
                    _android.Call("DiscoverCharacteristic", _serviceUUID, _charUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if (_bleInitialized)
                    bleDiscoverCharacteristic(device.address, _serviceUUID, _charUUID);
#endif
            }
            else if(tokens[0].Equals("ErrorDiscoveredService"))
            {
                Debug.Log("HM10 Discovered Service Error: " + tokens[1]);
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("DiscoveredCharacteristic"))
            { 
#if UNITY_ANDROID
				Debug.Log("HM10 Discovered Characteristic");
				if(_android != null)
					_android.Call("SubscribeCharacteristic2", _serviceUUID, _charUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
				if(tokens.Length > 1 && !_isBleOpen)
				{
					string[] tokens2 = message.Split(new char[] { ':' });
					bool foundCharUUID = false;
					for(int i=0; i<tokens2.Length; i++)
					{
						if(_charUUID.Equals(tokens2[i]) == true)
						{
							Debug.Log("HM10 Discovered Characteristic");
                            if(_bleInitialized)
                            {
                                bleSubscribe(device.address, _serviceUUID, _charUUID);
                            }
							foundCharUUID = true;
							break;
						}
					}

					if(!foundCharUUID)
					{
						Debug.Log(string.Format("Can not find HM-10 Characteristic <{0}>", _charUUID));
						_threadOnOpenFailed = true;
					}
				}
#endif     
            }
            else if(tokens[0].Equals("ErrorDiscoverCharacteristic"))
            {
                Debug.Log("HM10 Discover Characteristic Error: " + tokens[1]);
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("ErrorWrite"))
            {
                Debug.Log("HM10 Write Error: " + tokens[1]);
                _threadOnErrorClosed = true;
            }
            else if(tokens[0].Equals("ErrorSubscribeCharacteristic"))
            {
                Debug.Log("HM10 Subscribe Characteristic Error: " + tokens[1]);
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("SubscribedCharacteristic"))
            {
#if UNITY_ANDROID
				Debug.Log("HM10 Subscribed Characteristic");
				_isBleOpen = true;
				_bleOpenTry = false;
				_threadOnOpen = true;
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if(tokens.Length > 1)
                {
                    if(_charUUID.Equals(tokens[1]) == true)
                    {
                        Debug.Log("HM10 Subscribed Characteristic");
                        _isBleOpen = true;
						_bleOpenTry = false;
                        _threadOnOpen = true;
                    }
                }
#endif
            }
            else if(tokens[0].Equals("UnSubscribedCharacteristic"))
            {
#if UNITY_ANDROID
				Debug.Log("HM10 UnSubscribed Characteristic");
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if(tokens.Length > 1)
                {
                    if(_charUUID.Equals(tokens[1]) == true)
                    {
                        Debug.Log("HM10 UnSubscribed Characteristic");
                    }
                }
#endif
            }
            else if(tokens[0].Equals("Write"))
            {
                if(string.Compare(_charUUID, tokens[1], true) == 0)
                {
                    txWrite();
                }
            }
            else if(tokens[0].Equals("ErrorRead"))
            {
                Debug.Log("HM10 Read Error: " + tokens[1]);
                _threadOnErrorClosed = true;
            }
            else if(tokens[0].Equals("Read"))
            {
                byte[] base64Bytes = Convert.FromBase64String(tokens[2]);
                if(base64Bytes.Length > 0)
                {
                    if(string.Compare(_charUUID, tokens[1], true) == 0)
                    {
                        _rxBuffer.AddRange(base64Bytes);
                    }
                }
            }
            else
            {
                if(tokens.Length == 1)
                    Debug.Log(tokens[0]);
                else if(tokens.Length == 2)
                    Debug.Log(tokens[0] + ":" + tokens[1]);
            }
        }
    }
}