using UnityEngine;
using System.Collections.Generic;

using UINT8 = System.Byte;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Controller/Basic/DigitalOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/digitaloutput")]
	public class DigitalOutput : ArdunityController, IWireOutput<bool>, IWireOutput<float>
	{
		public int pin;
		public bool defaultValue = false;
		public bool resetOnStop = true;
		public bool Value = false;

		private bool _preValue = false;
		
		protected override void Awake()
		{
			base.Awake();
			
			enableUpdate = false; // only output.
		}

		void Start()
		{
			_preValue = Value;
		}

		void Update()
		{
			if(connected)
			{
				if(_preValue != Value)
				{
					_preValue = Value;
					SetDirty();
				}
			}
		}
		
		protected override void OnPush()
		{
			UINT8 data = 0;
			if(_preValue)
				data = 1;
			
			Push(data);
		}
		
		protected override void OnConnected()
		{
			_preValue = Value;
		}

		public override string GetCodeDeclaration()
		{
			string defaultValueString = "LOW";
			if(defaultValue)
				defaultValueString = "HIGH";
			
			string resetOnStopString = "false";
			if(resetOnStop)
				resetOnStopString = "true";
			
			return string.Format("{0} {1}({2:d}, {3:d}, {4}, {5});", this.GetType().Name, GetCodeVariable(), id, pin, defaultValueString, resetOnStopString);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("dOutput{0:d}", id);
		}
		
        #region Wire Editor
		bool IWireOutput<bool>.output
        {
			get
			{
				return Value;
			}
			set
			{
				Value = value;
			}
		}
		
		float IWireOutput<float>.output
        {
			get
			{
				if(Value)
					return 1f;
				else
					return 0f;
			}
			set
			{
				if(Mathf.Abs(value) < 0.5f)
					Value = false;
				else
					Value = true;
			}
		}
       
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Digital Pin"));
            nodes.Add(new Node("digitalValue", "Value(digital)", typeof(IWireOutput<bool>), NodeType.WireTo, "Output<bool>"));
			nodes.Add(new Node("analogValue", "Value(analog)", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: {0:d}", pin);
                return;
            }
			else if(node.name.Equals("digitalValue"))
            {
				node.updated = true;
                return;
            }
			else if(node.name.Equals("analogValue"))
            {
				node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}
