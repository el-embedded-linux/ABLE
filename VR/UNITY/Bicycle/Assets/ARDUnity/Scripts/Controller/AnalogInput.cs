using UnityEngine;
using System.Collections.Generic;

using FLOAT32 = System.Single;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Controller/Basic/AnalogInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/analoginput")]
	public class AnalogInput : ArdunityController, IWireInput<float>
	{
		public int pin;
		
		public FloatEvent OnValueChanged; 
		
		private FLOAT32 _value = 0f;
		
		
		protected override void OnExecuted()
		{
			if(OnWireInputChanged != null)
				OnWireInputChanged(_value);
				
			OnValueChanged.Invoke(_value);
		}
		
		protected override void OnPop()
		{
			FLOAT32 newValue = _value;
			Pop(ref newValue);
			if(newValue != _value)
			{
				_value = newValue;
				updated = true;
			}
		}

		public override string GetCodeDeclaration()
		{
			return string.Format("{0} {1}({2:d}, A{3:d});", this.GetType().Name, GetCodeVariable(), id, pin);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("aInput{0:d}", id);
		}
		
		public float Value
		{
			get
			{
				return (float)_value;
			}
		}
		
        #region Wire Editor
		public event WireEventHandler<float> OnWireInputChanged;
		
		float IWireInput<float>.input
		{
			get
			{
				return Value;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Analog Pin"));
            nodes.Add(new Node("Value", "Value", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: A{0:d}", pin);
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
