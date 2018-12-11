using UnityEngine;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/ArdunityAppUI")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/ardunityappui")]
	public class ArdunityAppUI : MonoBehaviour
	{
		public ArdunityApp ardunityApp;
		public CommSocketUI commSocketUI;
		public Button connect;
		public Button disconnect;
		public Button quit;
		public Canvas messageCanvas;
		public RectTransform msgConnecting;
		public RectTransform msgConnectionFailed;
		public RectTransform msgLostConnection;
		public Button okConnectionFailed;
		public Button okLostConnection;
		
		void Awake()
		{
			ardunityApp.OnConnected.AddListener(OnArdunityConnected);
			ardunityApp.OnDisconnected.AddListener(OnArdunityDisconnected);
			ardunityApp.OnConnectionFailed.AddListener(OnArdunityConnectionFailed);
			ardunityApp.OnLostConnection.AddListener(OnArdunityLostConnection);
			
			if(commSocketUI != null)
				commSocketUI.OnSettingCompleted.AddListener(OnCommSocketSettingCompleted);
			
			connect.onClick.AddListener(OnConnectClick);
			disconnect.onClick.AddListener(OnDisconnectClick);
			quit.onClick.AddListener(OnQuitClick);
			
			okConnectionFailed.onClick.AddListener(OnMessageOKClick);
			okLostConnection.onClick.AddListener(OnMessageOKClick);
		}
	
		// Use this for initialization
		void Start ()
		{
			messageCanvas.gameObject.SetActive(false);
			disconnect.gameObject.SetActive(false);
			connect.gameObject.SetActive(true);
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		private void OnConnectClick()
		{
			if(commSocketUI != null)
				commSocketUI.ShowUI();
			else
				OnCommSocketSettingCompleted();
		}
		
		private void OnDisconnectClick()
		{
			ardunityApp.Disconnect();
		}
		
		private void OnCommSocketSettingCompleted()
		{
			ardunityApp.Connect();
			
			messageCanvas.gameObject.SetActive(true);
			msgConnecting.gameObject.SetActive(true);
			msgConnectionFailed.gameObject.SetActive(false);
			msgLostConnection.gameObject.SetActive(false);
		}
		
		private void OnQuitClick()
		{
			ardunityApp.Disconnect();
			Application.Quit();
		}
		
		private void OnMessageOKClick()
		{
			messageCanvas.gameObject.SetActive(false);
		}
		
		private void OnArdunityConnected()
		{
			disconnect.gameObject.SetActive(true);
			connect.gameObject.SetActive(false);
			
			messageCanvas.gameObject.SetActive(false);
			msgConnecting.gameObject.SetActive(false);
			msgConnectionFailed.gameObject.SetActive(false);
			msgLostConnection.gameObject.SetActive(false);
		}
		
		private void OnArdunityDisconnected()
		{
			disconnect.gameObject.SetActive(false);
			connect.gameObject.SetActive(true);
		}
		
		private void OnArdunityConnectionFailed()
		{
			messageCanvas.gameObject.SetActive(true);
			msgConnecting.gameObject.SetActive(false);
			msgConnectionFailed.gameObject.SetActive(true);
			msgLostConnection.gameObject.SetActive(false);
		}
		
		private void OnArdunityLostConnection()
		{
			messageCanvas.gameObject.SetActive(true);
			msgConnecting.gameObject.SetActive(false);
			msgConnectionFailed.gameObject.SetActive(false);
			msgLostConnection.gameObject.SetActive(true);
		}
	}
}
