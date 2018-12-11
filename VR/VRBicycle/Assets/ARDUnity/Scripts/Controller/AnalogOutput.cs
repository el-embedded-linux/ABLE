using UnityEngine;
using System.Collections.Generic;

using FLOAT32 = System.Single;


namespace Ardunity
{		
	[AddComponentMenu("ARDUnity/Controller/Basic/AnalogOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/analogoutput")]
	public class AnalogOutput : ArdunityController, IWireOutput<float>
	{
		public int pin;
		[Range(0f, 1f)]
		public float defaultValue = 0f;
		public bool resetOnStop = true;
		public float Value = 0f;

		private float _preValue;
				
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
			Value = Mathf.Clamp(Value, 0f, 1f);

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
			FLOAT32 data = _preValue;
			Push(data);
		}

		protected override void OnConnected()
		{
			_preValue = Value;
		}
		
		public override string GetCodeDeclaration()
		{
			string resetOnStopString = "false";
			if(resetOnStop)
				resetOnStopString = "true";
			
			return string.Format("{0} {1}({2:d}, {3:d}, {4:f2}, {5});", this.GetType().Name, GetCodeVariable(), id, pin, Mathf.Clamp(defaultValue, 0f, 1f), resetOnStopString);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("aOutput{0:d}", id);
		}
		
        #region Wire Editor
		float IWireOutput<float>.output
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
		
        protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino PWM Pin"));
			nodes.Add(new Node("Value", "Value", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: ~ {0:d}", pin);
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
