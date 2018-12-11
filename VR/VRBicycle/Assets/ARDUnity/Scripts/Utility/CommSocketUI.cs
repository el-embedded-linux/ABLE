using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Internal/CommSocketUI")]
	public class CommSocketUI : MonoBehaviour
	{	
		public Canvas popupCanvas;
		public RectTransform settingCommSocket;
		public Button ok;
		public Button cancel;
		
		public UnityEvent OnSettingCompleted;

	
		protected virtual void Awake()
		{
			ok.onClick.AddListener(SettingOK);
			cancel.onClick.AddListener(CloseCancel);
		}
		
		protected virtual void Start()
		{
			popupCanvas.gameObject.SetActive(false);
			settingCommSocket.gameObject.SetActive(false);
		}
		
		public virtual void ShowUI()
		{
			popupCanvas.gameObject.SetActive(true);
			settingCommSocket.gameObject.SetActive(true);
		}
		
		private void SettingOK()
		{
			CloseOK();
			OnSettingCompleted.Invoke();
		}
		
		protected virtual void CloseOK()
		{
			popupCanvas.gameObject.SetActive(false);
			settingCommSocket.gameObject.SetActive(false);
		}
		
		protected virtual void CloseCancel()
		{
			popupCanvas.gameObject.SetActive(false);
			settingCommSocket.gameObject.SetActive(false);
		}
	}
}

