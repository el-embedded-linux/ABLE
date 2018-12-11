using UnityEngine;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/CommSerialUI")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/commserialui")]
	public class CommSerialUI : CommSocketUI
	{
		public CommSerial commSerial;
		public Dropdown portList;
		
	
		protected override void Awake()
		{
			base.Awake();
			
			commSerial.OnStartSearch.AddListener(OnStartSearch);
			commSerial.OnFoundDevice.AddListener(OnFoundDevice);
			commSerial.OnStopSearch.AddListener(OnStopSearch);
			
			portList.onValueChanged.AddListener(OnSelectionChanged);
		}
		
		protected override void Start()
		{
			base.Start();
			
			portList.options.Clear();
		}
		
		public override void ShowUI()
		{
			base.ShowUI();
			
			commSerial.StartSearch();
		}
		
		protected override void CloseOK()
		{
			base.CloseOK();
			
			if(portList.options.Count > 0)
				commSerial.device = new CommDevice(commSerial.foundDevices[portList.value]);
		}
		
		protected override void CloseCancel()
		{
			base.CloseCancel();
		}
		
		private void OnSelectionChanged(int index)
		{
		}
		
		private void OnStartSearch()
		{
			portList.options.Clear();
			ok.interactable = true;
		}
		
		private void OnFoundDevice(CommDevice device)
		{
			Dropdown.OptionData item = new Dropdown.OptionData();
			item.text = device.name;
			portList.options.Add(item);
		}
		
		private void OnStopSearch()
		{
			for(int i=0; i<commSerial.foundDevices.Count; i++)
			{
				if(commSerial.device.Equals(commSerial.foundDevices[i]))
				{
					if(portList.value == i)
						portList.captionText.text = commSerial.device.name;
					else
						portList.value = i;
					
					return;
				}
			}
			
			if(commSerial.foundDevices.Count > 0)
				portList.captionText.text = portList.options[0].text;
			else
			{
				portList.captionText.text = "";
				ok.interactable = false;
			}				
		}
	}
}
