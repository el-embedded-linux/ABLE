using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_STANDALONE
#if UNITY_5_6_OR_NEWER
#if NET_2_0
using System.IO;
using System.IO.Ports;
using System.Threading;
#endif
#else
using System.IO;
using System.IO.Ports;
using System.Threading;
#endif
#endif


namespace Ardunity
{
    [AddComponentMenu("ARDUnity/CommSocket/CommSerial")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/commsocket/commserial")]
    public class CommSerial : CommSocket
    {
        public int baudrate = 115200;
        public bool dtrReset = true;

#if UNITY_STANDALONE
#if UNITY_5_6_OR_NEWER
#if NET_2_0
		private bool _threadOnOpen = false;
		private bool _threadOnOpenFailed = false;
		private Thread _openThread;

        private SerialPort _serialPort;

		protected override void Awake()
		{
			base.Awake();

			_serialPort = new SerialPort();
			_serialPort.DataBits = 8;
			_serialPort.Parity = Parity.None;
			_serialPort.StopBits = StopBits.One;
			_serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread            
			_serialPort.WriteTimeout = 1000;
			_serialPort.Handshake = Handshake.None;
			_serialPort.RtsEnable = false;
		}

		void Update()
		{
			if (_threadOnOpen)
			{
				OnOpen.Invoke();
				_threadOnOpen = false;
			}

			if (_threadOnOpenFailed)
			{
				OnOpenFailed.Invoke();
				_threadOnOpenFailed = false;
			}
		}

		#region Override
		public override void Open()
		{
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
			try
			{
				_serialPort.Close();
			}
			catch (Exception)
			{
			}
		}

		public override bool IsOpen
		{
			get
			{
				if (_serialPort == null)
					return false;

				return _serialPort.IsOpen;
			}
		}

		public override void StartSearch()
		{
			foundDevices.Clear();
			OnStartSearch.Invoke();

#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = port;
                foundDevice.address = "//./" + port;
                foundDevices.Add(foundDevice);
                OnFoundDevice.Invoke(foundDevice);
            }
#elif UNITY_EDITOR_OSX
			string prefix = "/dev/";
			string[] ports = Directory.GetFiles(prefix, "*.*");
			foreach (string port in ports)
			{
				if (port.StartsWith("/dev/cu."))
				{
					CommDevice foundDevice = new CommDevice();
					foundDevice.name = port.Substring(prefix.Length);
					foundDevice.address = port;
					foundDevices.Add(foundDevice);
					OnFoundDevice.Invoke(foundDevice);
				}
			}
#endif
#else
#if UNITY_STANDALONE_WIN
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = port;
                foundDevice.address = "//./" + port;
                foundDevices.Add(foundDevice);
                OnFoundDevice.Invoke(foundDevice);
            }
#elif UNITY_STANDALONE_OSX
            string prefix = "/dev/";
            string[] ports = Directory.GetFiles(prefix, "*.*");
            foreach (string port in ports)
            {
                if (port.StartsWith("/dev/cu."))
                {
                    CommDevice foundDevice = new CommDevice();
                    foundDevice.name = port.Substring(prefix.Length);
                    foundDevice.address = port;
                    foundDevices.Add(foundDevice);
                    OnFoundDevice.Invoke(foundDevice);
                }
            }
#endif
#endif
			OnStopSearch.Invoke();
		}

		public override void Write(byte[] data, bool getCompleted = false)
		{
			if (data == null)
				return;
			if (data.Length == 0)
				return;

			try
			{
				_serialPort.Write(data, 0, data.Length);
				if (getCompleted)
					OnWriteCompleted.Invoke();
			}
			catch (Exception)
			{
				ErrorClose();
				OnErrorClosed.Invoke();
			}
		}

		public override byte[] Read()
		{
			List<byte> bytes = new List<byte>();

			while (true)
			{
				try
				{
					bytes.Add((byte)_serialPort.ReadByte());
				}
				catch (TimeoutException)
				{
					break;
				}
				catch (Exception)
				{
					ErrorClose();
					OnErrorClosed.Invoke();
					return null;
				}
			}

			if (bytes.Count == 0)
				return null;
			else
				return bytes.ToArray();
		}
		#endregion

		private void openThread()
		{
			try
			{
				_serialPort.PortName = device.address;
				_serialPort.BaudRate = baudrate;
				_serialPort.DtrEnable = dtrReset;
				_serialPort.Open();
				_threadOnOpen = true;
			}
			catch (Exception)
			{
				_threadOnOpenFailed = true;
			}

			_openThread.Abort();
			return;
		}
#else
        protected override void Awake()
        {
            base.Awake();
        }

        void Update()
        {
        }

		#region Override
        public override void Open()
        {
        }

        public override void Close()
        {
        }

        protected override void ErrorClose()
        {
        }

        public override bool IsOpen
        {
            get
            {
                return false;
            }
        }

        public override void StartSearch()
        {
        }

        public override void Write(byte[] data, bool getCompleted = false)
        {
        }

        public override byte[] Read()
        {
            return null;
        }
		#endregion
#endif
#else
        private bool _threadOnOpen = false;
        private bool _threadOnOpenFailed = false;
        private Thread _openThread;

        private SerialPort _serialPort;

        protected override void Awake()
        {
            base.Awake();

            _serialPort = new SerialPort();
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread            
            _serialPort.WriteTimeout = 1000;
            _serialPort.Handshake = Handshake.None;
            _serialPort.RtsEnable = false;
        }

        void Update()
        {
            if (_threadOnOpen)
            {
                OnOpen.Invoke();
                _threadOnOpen = false;
            }

            if (_threadOnOpenFailed)
            {
                OnOpenFailed.Invoke();
                _threadOnOpenFailed = false;
            }
        }

		#region Override
        public override void Open()
        {
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
            try
            {
                _serialPort.Close();
            }
            catch (Exception)
            {
            }
        }

        public override bool IsOpen
        {
            get
            {
                if (_serialPort == null)
                    return false;

                return _serialPort.IsOpen;
            }
        }

        public override void StartSearch()
        {
            foundDevices.Clear();
            OnStartSearch.Invoke();

#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = port;
                foundDevice.address = "//./" + port;
                foundDevices.Add(foundDevice);
                OnFoundDevice.Invoke(foundDevice);
            }
#elif UNITY_EDITOR_OSX
            string prefix = "/dev/";
            string[] ports = Directory.GetFiles(prefix, "*.*");
            foreach (string port in ports)
            {
                if (port.StartsWith("/dev/cu."))
                {
                    CommDevice foundDevice = new CommDevice();
                    foundDevice.name = port.Substring(prefix.Length);
                    foundDevice.address = port;
                    foundDevices.Add(foundDevice);
                    OnFoundDevice.Invoke(foundDevice);
                }
            }
#endif
#else
#if UNITY_STANDALONE_WIN
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = port;
                foundDevice.address = "//./" + port;
                foundDevices.Add(foundDevice);
                OnFoundDevice.Invoke(foundDevice);
            }
#elif UNITY_STANDALONE_OSX
            string prefix = "/dev/";
            string[] ports = Directory.GetFiles(prefix, "*.*");
            foreach (string port in ports)
            {
                if (port.StartsWith("/dev/cu."))
                {
                    CommDevice foundDevice = new CommDevice();
                    foundDevice.name = port.Substring(prefix.Length);
                    foundDevice.address = port;
                    foundDevices.Add(foundDevice);
                    OnFoundDevice.Invoke(foundDevice);
                }
            }
#endif
#endif
            OnStopSearch.Invoke();
        }

        public override void Write(byte[] data, bool getCompleted = false)
        {
            if (data == null)
                return;
            if (data.Length == 0)
                return;

            try
            {
                _serialPort.Write(data, 0, data.Length);
                if (getCompleted)
                    OnWriteCompleted.Invoke();
            }
            catch (Exception)
            {
                ErrorClose();
                OnErrorClosed.Invoke();
            }
        }

        public override byte[] Read()
        {
            List<byte> bytes = new List<byte>();

            while (true)
            {
                try
                {
                    bytes.Add((byte)_serialPort.ReadByte());
                }
                catch (TimeoutException)
                {
                    break;
                }
                catch (Exception)
                {
                    ErrorClose();
                    OnErrorClosed.Invoke();
                    return null;
                }
            }

            if (bytes.Count == 0)
                return null;
            else
                return bytes.ToArray();
        }
		#endregion

        private void openThread()
        {
            try
            {
                _serialPort.PortName = device.address;
                _serialPort.BaudRate = baudrate;
                _serialPort.DtrEnable = dtrReset;
                _serialPort.Open();
                _threadOnOpen = true;
            }
            catch (Exception)
            {
                _threadOnOpenFailed = true;
            }

            _openThread.Abort();
            return;
        }
#endif
#else
        protected override void Awake()
        {
            base.Awake();
        }

        void Update()
        {
        }

		#region Override
        public override void Open()
        {
        }

        public override void Close()
        {
        }

        protected override void ErrorClose()
        {
        }

        public override bool IsOpen
        {
            get
            {
                return false;
            }
        }

        public override void StartSearch()
        {
        }

        public override void Write(byte[] data, bool getCompleted = false)
        {
        }

        public override byte[] Read()
        {
            return null;
        }
		#endregion
#endif

		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("baudrate", "", null, NodeType.None, "Serial Baudrate Speed"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("baudrate"))
            {
                node.updated = true;
                node.text = string.Format("{0:d} bps", baudrate);
                return;
            }
            
            base.UpdateNode(node);
        }
    }
}
