using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/Device/DeviceCamera")]
	[HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/devicecamera")]
	public class DeviceCamera : MonoBehaviour
	{
		public int defaultDevice = 0;
		public int capWidth = 320;
		public int capHeight = 240;
		public int capFPS = 30;
		public bool autoPlay = true;
		
		public Material material;
		public RawImage rawImage;

		private string _deviceName;
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
		private WebCamTexture _webcam = null;
#endif
		
		void Awake()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			_webcam = new WebCamTexture();
			_deviceName = _webcam.deviceName;

			if(material != null)
				material.mainTexture = _webcam;
			
			if(rawImage != null)
				rawImage.texture = _webcam;
#endif
		}
		
		// Use this for initialization
		void Start ()
		{
			if(ChangeDevice(defaultDevice) == false)
				ChangeDevice();
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}

		public string[] devices
		{
			get
			{
				List<string> names = new List<string>();
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)			
				WebCamDevice[] webcams = WebCamTexture.devices;
				for(int i=0; i<webcams.Length; i++)
					names.Add(webcams[i].name);
#endif
				return names.ToArray();
			}
		}

		public string deviceName
		{
			get
			{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				return _webcam.deviceName;
#else
				return "";
#endif
			}
		}

		public int Width
		{
			get
			{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				return _webcam.requestedWidth;
#else
				return 0;
#endif
			}
		}
		
		public int Height
		{
			get
			{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				return _webcam.requestedHeight;
#else
				return 0;
#endif
			}
		}

		public bool isPlaying
		{
			get
			{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				return _webcam.isPlaying;
#else
				return false;
#endif
			}
		}

		public void RefreshSettings()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			if(_webcam.isPlaying)
				_webcam.Stop();
			
			_webcam.deviceName = _deviceName;
			_webcam.requestedWidth = capWidth;
			_webcam.requestedHeight = capHeight;
			_webcam.requestedFPS = capFPS;
			if(autoPlay)
				_webcam.Play();
#endif
		}

		public void ChangeDevice()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			WebCamDevice[] devices = WebCamTexture.devices;
			if(devices.Length > 0)
			{
				int index = -1;
				for(int i=0; i<devices.Length; i++)
				{
					if(_webcam.deviceName.Equals(devices[0].name))
					{
						index = i;
						break;
					}
				}

				if(index < 0)
					index = Mathf.Clamp(defaultDevice, 0, devices.Length - 1);
				else if(index == (devices.Length - 1))
					index = 0;
				else
					index++;
				
				_deviceName = devices[index].name;
				RefreshSettings();
			}
#endif
		}

		public bool ChangeDevice(int index)
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			WebCamDevice[] devices = WebCamTexture.devices;
			if(index >= 0 && index < devices.Length)
			{
				_deviceName = devices[index].name;
				RefreshSettings();
				return true;
			}
#endif
			return false;
		}

		public bool ChangeDevice(string name)
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			if(!_webcam.deviceName.Equals(name))
			{
				WebCamDevice[] devices = WebCamTexture.devices;
				if(devices.Length > 0)
				{
					for(int i=0; i<devices.Length; i++)
					{
						if(name.Equals(devices[i].name))
						{
							_deviceName = devices[i].name;
							RefreshSettings();							
							return true;
						}
					}
				}
			}			
#endif
			return false;
		}
		
		public void Play()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			_webcam.Play();
#endif
		}
		
		public void Pause()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			_webcam.Pause();
#endif
		}
		
		public void Stop()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			_webcam.Stop();
#endif
		}
	}
}
