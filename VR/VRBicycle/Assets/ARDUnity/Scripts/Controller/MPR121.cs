using UnityEngine;
using System.Collections.Generic;

using UINT16 = System.UInt16;


namespace Ardunity
{		
	[AddComponentMenu("ARDUnity/Controller/Sensor/MPR121")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/mpr121")]
	public class MPR121 : ArdunityController, IWireInput<UINT16>
	{
		public int address = 0x5A;
		
		private UINT16 _value;

		protected override void OnExecuted()
		{
			if(OnWireInputChanged != null)
				OnWireInputChanged(_value);
		}
		
		protected override void OnPop()
		{
			UINT16 newValue = _value;
			Pop(ref newValue);
			if(newValue != _value)
			{
				_value = newValue;
				updated = true;
			}
		}

		public override string GetCodeDeclaration()
		{
             return string.Format("{0} {1}({2:d}, 0x{3:x});", this.GetType().Name, GetCodeVariable(), id, address);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("mpr121_{0:d}", id);
		}

		public override string[] GetAdditionalFiles()
		{
			List<string> additionals = new List<string>();
			additionals.Add("ArdunityI2C.h");
			additionals.Add("ArdunityI2C.cpp");
			return additionals.ToArray();
		}
		
		public bool GetElectrodeState(int ch)
		{
			if((_value & (0x01 << ch)) == 0x00)
				return false;
			else
				return true;
		}
		
        #region Wire Editor
		public event WireEventHandler<UINT16> OnWireInputChanged;
		
		UINT16 IWireInput<UINT16>.input
		{
			get
			{
				return _value;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("address", "", null, NodeType.None, "I2C Address"));
            nodes.Add(new Node("Value", "Value", typeof(IWireInput<UINT16>), NodeType.WireTo, "Input<UINT16>"));
        }

        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("address"))
            {
				node.updated = true;
				node.text = string.Format("Address: 0x{0:X}", address);

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
