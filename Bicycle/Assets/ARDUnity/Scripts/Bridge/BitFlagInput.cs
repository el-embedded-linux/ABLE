using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

using UINT16 = System.UInt16;

namespace Ardunity
{
    [AddComponentMenu("ARDUnity/Bridge/Input/BitFlagInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/bitflaginput")]
	public class BitFlagInput : ArdunityBridge, IWireInput<bool>
	{
		public enum BitCombine
		{
			AND,
			OR
		}

        public int bitMask = 0x00;
		public BitCombine bitCombine = BitCombine.OR;
        
		public BoolEvent OnValueChanged;
        
        private IWireInput<UINT16> _input;
		private bool _value = false;


        // Use this for initialization
        void Start ()
        {
		}
		
		// Update is called once per frame
		void Update ()
        {
		}

		private void InputChanged(UINT16 value)
		{
			value &= (UINT16)bitMask;

			bool newValue = false;
			if(bitCombine == BitCombine.AND)
			{
				if(value == bitMask)
					newValue = true;
			}
			else if(bitCombine == BitCombine.OR)
			{
				if(value != 0x00)
					newValue = true;
			}

			if(_value != newValue)
			{
				_value = newValue;
				if(OnWireInputChanged != null)
					OnWireInputChanged(_value);
				
				OnValueChanged.Invoke(_value);
			}
		}

		public bool Value
		{
			get
			{
				return _value;
			}
		}

		public static string ToMaskString(int value)
		{
			string mask = "";
			for(int i=15; i>=0; i--)
			{
				if((value & (1 << i)) > 0)
					mask += "1";
				else
					mask += "0";
			}
			return mask;
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
			
            nodes.Add(new Node("input", "Input", typeof(IWireInput<UINT16>), NodeType.WireFrom, "IWireInput<UINT16>"));
			nodes.Add(new Node("bitMask", "", null, NodeType.None, "Bit Mask"));
			nodes.Add(new Node("Value", "Value", typeof(IWireInput<bool>), NodeType.WireTo, "Input<bool>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("input"))
            {
                node.updated = true;

                if(node.objectTarget == null && _input == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_input))
                        return;
                }

				if(_input != null)
                    _input.OnWireInputChanged -= InputChanged;
                
                _input = node.objectTarget as IWireInput<UINT16>;
                if(_input != null)
                    _input.OnWireInputChanged += InputChanged;
                else
                    node.objectTarget = null;

                return;
            }
			else if(node.name.Equals("bitMask"))
            {
				node.updated = true;
				node.text = "Mask: " + ToMaskString(bitMask);
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
