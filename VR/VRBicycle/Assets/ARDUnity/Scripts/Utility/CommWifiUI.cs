using UnityEngine;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/CommWifiUI")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/commwifiui")]
	public class CommWifiUI : CommSocketUI
	{
		public CommWifi commWifi;
		public InputField ipAddress;
		public InputField port;
	
		protected override void Awake()
		{
			base.Awake();
		}
		
		protected override void Start()
		{
			base.Start();
		}
		
		public override void ShowUI()
		{
			base.ShowUI();
            
            ipAddress.text = commWifi.device.address;
            if(commWifi.device.args.Count == 0)
                commWifi.device.args.Add("0");
            port.text = commWifi.device.args[0];
		}
		
		protected override void CloseOK()
		{
			base.CloseOK();
            
            commWifi.device.address = ipAddress.text;
            commWifi.device.args[0] = port.text;
		}
		
		protected override void CloseCancel()
		{
			base.CloseCancel();
		}
	}
}
