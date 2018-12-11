using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/CommSocket/CommWifi")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/commsocket/commwifi")]
	public class CommWifi : CommSocket
	{
        
		private Socket _socket;
        private bool _threadOnOpen = false;
        private bool _threadOnOpenFailed = false;
        private Thread _openThread;
        private List<byte> _txBuffer = new List<byte>();
        private bool _getCompleted = false;
        
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
        
        void LateUpdate()
        {
            if(IsOpen && _txBuffer.Count > 0)
            {
                try
                {
                    _socket.Send(_txBuffer.ToArray());
                    _txBuffer.Clear();
                    if(_getCompleted)
                        OnWriteCompleted.Invoke();
                }
                catch(Exception e)
                {
                    Debug.Log("Write: " + e);
                    ErrorClose();
                    OnErrorClosed.Invoke();
                }
            }
        }

        #region Override
        public override void Open()
        {
            if (IsOpen)
                return;

            if(_openThread != null)
                _openThread.Abort();
            
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
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception e)
            {
                Debug.Log("ErrorClose: " + e);
            }
        }

        public override bool IsOpen
        {
            get
            {
                if(_socket == null)
					return false;
				
				return _socket.Connected;
            }
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

        public override byte[] Read()
        {
            List<byte> bytes = new List<byte>();

			try
			{
				if(_socket.Available > 0)
				{
					byte[] rcvData = new byte[256];
					int count = _socket.Receive(rcvData);
					for(int i=0; i<count; i++)
                        bytes.Add(rcvData[i]);
				}
			}
			catch(Exception e)
			{
                Debug.Log("Read: " + e);
                ErrorClose();
                OnErrorClosed.Invoke();
                return null;
            }

            if (bytes.Count == 0)
                return null;
            else
                return bytes.ToArray();
        }
        #endregion
        
        private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
			if(_socket.Connected == true)
                _threadOnOpen = true;
			else
			{
				Debug.Log("SocketError: " + e.SocketError.ToString());
                _threadOnOpenFailed = true;
            }
		}

        private void openThread()
        {
#if UNITY_ANDROID
            AndroidJNI.AttachCurrentThread();
#endif
            _txBuffer.Clear();

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.NoDelay = true;
                _socket.ReceiveBufferSize = 4096;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(device.address), int.Parse(device.args[0]));
                e.UserToken = _socket;
                e.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
                _socket.ConnectAsync(e);
            }
            catch (Exception e)
            {
                Debug.Log("OpenThread: " + e);
                _threadOnOpenFailed = true;
            }
            
#if UNITY_ANDROID
            AndroidJNI.DetachCurrentThread();
#endif

            _openThread.Abort();
            return;
        }
    }
}
