using UnityEngine;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/CommBluetoothUI")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/commbluetoothui")]
	public class CommBluetoothUI : CommSocketUI
	{
		public CommBluetooth commBluetooth;
		public ListView deviceList;
		public ListItem deviceItem;
		
	
		protected override void Awake()
		{
			base.Awake();
			
			commBluetooth.OnStartSearch.AddListener(OnStartSearch);
			commBluetooth.OnFoundDevice.AddListener(OnFoundDevice);
			commBluetooth.OnStopSearch.AddListener(OnStopSearch);
			
			deviceList.OnSelectionChanged.AddListener(OnSelectionChanged);
		}
		
		protected override void Start()
		{
			base.Start();
			
			deviceList.ClearItem();
		}
		
		public override void ShowUI()
		{
			base.ShowUI();
			
			commBluetooth.StartSearch();
		}
		
		protected override void CloseOK()
		{
			base.CloseOK();
			
			ListItem selectedItem = deviceList.selectedItem;
			if(selectedItem != null)
				commBluetooth.device = new CommDevice((CommDevice)selectedItem.data);
		}
		
		protected override void CloseCancel()
		{
			base.CloseCancel();
		}
		
		private void OnSelectionChanged(ListItem item)
		{
			if(item != null)
				ok.interactable = true;
			else
				ok.interactable = false;
		}
		
		private void OnStartSearch()
		{
			deviceList.ClearItem();
			ok.interactable = false;
		}
		
		private void OnFoundDevice(CommDevice device)
		{
			ListItem item = GameObject.Instantiate(deviceItem);
			item.gameObject.SetActive(true);
			item.textList[0].text = device.name;
			if(item.textList.Length > 1)
				item.textList[1].text = device.address;
			item.data = device;
			
			deviceList.AddItem(item);
			
			if(deviceList.selectedItem == null)
			{
				if(commBluetooth.device.Equals(device))
					deviceList.selectedItem = item;
			}			
		}
		
		private void OnStopSearch()
		{
		}
	}
}
