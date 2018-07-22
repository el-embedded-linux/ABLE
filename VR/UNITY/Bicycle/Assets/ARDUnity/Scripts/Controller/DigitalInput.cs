using UnityEngine;
using System.Collections.Generic;

using UINT8 = System.Byte;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Controller/Basic/DigitalInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/digitalinput")]
	public class DigitalInput : ArdunityController, IWireInput<bool>
	{
		public int pin;
		public bool pullup = true;		
		
		public BoolEvent OnValueChanged;
		
		private UINT8 _value = 0;
		
		protected override void OnExecuted()
		{
			if(OnWireInputChanged != null)
				OnWireInputChanged(Value);
				
			OnValueChanged.Invoke(Value);
		}

		protected override void OnPop()
		{
			UINT8 newValue = _value;
			Pop(ref newValue);
			if(newValue != _value)
			{
				_value = newValue;
				updated = true;
			}
		}

		public override string GetCodeDeclaration()
		{
			string code = string.Format("{0} {1}({2:d}, {3:d}, ", this.GetType().Name, GetCodeVariable(), id, pin);
			if(pullup == true)
				code += "true);";
			else
				code += "false);";

			return code;
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("dInput{0:d}", id);
		}
		
		public bool Value
		{
			get
			{
				if(connected)
				{
					bool result = false;
					if(_value != 0)
						result = true;
					
					if(pullup)
						return !result;
					else
						return result;
				}
				else
					return false;
			}
		}
		
        #region Wire Editor
		public event WireEventHandler<bool> OnWireInputChanged;
		
		bool IWireInput<bool>.input
		{
			get
			{
				return Value;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Digital Pin"));
            nodes.Add(new Node("pullup", "", null, NodeType.None, "Use Internal pullup"));
            nodes.Add(new Node("Value", "Value", typeof(IWireInput<bool>), NodeType.WireTo, "Input<bool>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: {0:d}", pin);
                return;
            }
            else if(node.name.Equals("pullup"))
            {
				node.updated = true;
                node.text = string.Format("Pullup: {0}", pullup.ToString());
                return;
            }
			else if(node.name.Equals("Value"))
            {
				node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}
