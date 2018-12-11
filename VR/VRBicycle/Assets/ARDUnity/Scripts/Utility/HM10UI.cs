using UnityEngine;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/HM10UI")]
	public class HM10UI : CommSocketUI
	{
		public HM10 hm10;
		public ListView deviceList;
		public ListItem deviceItem;
        
	
		protected override void Awake()
		{
			base.Awake();
			
			hm10.OnStartSearch.AddListener(OnStartSearch);
			hm10.OnFoundDevice.AddListener(OnFoundDevice);
			hm10.OnStopSearch.AddListener(OnStopSearch);
            hm10.OnOpen.AddListener(OnBleOpen);
            hm10.OnClose.AddListener(OnBleClose);
			
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
			
			hm10.StartSearch();
		}
		
		protected override void CloseOK()
		{
			base.CloseOK();
			
			ListItem selectedItem = deviceList.selectedItem;
			if(selectedItem != null)
				hm10.device = new CommDevice((CommDevice)selectedItem.data);
		}
		
		protected override void CloseCancel()
		{
			base.CloseCancel();
		}
		
		private void OnSelectionChanged(ListItem item)
		{
			if(item != null)
            {
                ok.interactable = true;
            }
			else
            {
                ok.interactable = false;
            }
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
				if(hm10.device.Equals(device))
					deviceList.selectedItem = item;
			}			
		}
		
		private void OnStopSearch()
		{
		}
        
        private void OnBleOpen()
        {
        }
        
        private void OnBleClose()
        {
        }
	}
}
