using UnityEngine;
using System.Collections.Generic;

using UINT8 = System.Byte;
using UINT16 = System.UInt16;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Controller/Basic/PulseOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/pulseoutput")]
	public class PulseOutput : ArdunityController, IWireOutput<bool>, IWireOutput<Trigger>
	{
		public int pin;
		public bool defaultValue = false;
		public int setTime = 1000;
		public int delayTime = 1000;
		
		private bool _firstPush = true;
		private UINT8 _loop = 0;
		private Trigger _preWireTriggerValue;
		
		
		protected override void Awake()
		{
			base.Awake();
			
			enableUpdate = false; // only output.

			_preWireTriggerValue = new Trigger();
            _preWireTriggerValue.Clear();
		}
		
		protected override void OnPush()
		{
			if(_firstPush)
				_firstPush = false;
			else
			{
				Push(_loop);
				Push((UINT16)setTime);
				Push((UINT16)delayTime);				
			}			
		}
		
		protected override void OnPop()
		{ 			 
		}

		protected override void OnConnected()
		{
			_firstPush = true;
		}

		protected override void OnDisconnected()
		{
			_loop = 0;
		}
		
		public override string GetCodeDeclaration()
		{
			string defaultValueString = "LOW";
			if(defaultValue)
				defaultValueString = "HIGH";
			
			return string.Format("{0} {1}({2:d}, {3:d}, {4});", this.GetType().Name, GetCodeVariable(), id, pin, defaultValueString);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("pulseOutput{0:d}", id);
		}

		public void OneShot()
		{
			if(connected && !Active)
			{
				_loop = 0;
				SetDirty();
			}
		}
		
		public bool Active
		{
			get
			{
				if(_loop == 0)
					return false;
				else
					return true;
			}
			set
			{
				if(!connected)
					return;
				
				if(value == Active)
					return;
				
				_loop = 0;
				if(value)
					_loop = 1;
				
				SetDirty();
			}
		}
		
        #region Wire Editor
		bool IWireOutput<bool>.output
        {
			get
			{
				return Active;
			}
			set
			{
				Active = value;
			}
		}
		
		Trigger IWireOutput<Trigger>.output
        {
			get
			{
				return _preWireTriggerValue;
			}
            set
            {
				if(value.value)
					OneShot();
				
				_preWireTriggerValue = value;
            }
		}
       
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Digital Pin"));
            nodes.Add(new Node("active", "Active", typeof(IWireOutput<bool>), NodeType.WireTo, "Output<bool>"));
			nodes.Add(new Node("oneShot", "OneShot", typeof(IWireOutput<Trigger>), NodeType.WireTo, "Output<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: {0:d}", pin);
                return;
            }
			else if(node.name.Equals("active"))
            {
				node.updated = true;
                return;
            }
			else if(node.name.Equals("oneShot"))
            {
				node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}
