using UnityEngine;
using System.Collections.Generic;

using FLOAT32 = System.Single;


namespace Ardunity
{		
	[AddComponentMenu("ARDUnity/Controller/Motor/GenericMotor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/genericmotor")]
	public class GenericMotor : ArdunityController, IWireOutput<float>
	{
        public enum ControlType
        {
            OnePWM_OneDir,
            TwoPWM,
            OnePWM_TwoDir
        }
        
        public ControlType controlType = ControlType.OnePWM_OneDir;
		public int pin1;
		public int pin2;
        public int pin3;
        [Range(0f, 1f)]
        public float punchValue = 0f;
		
        [SerializeField]
        private float _value;
		private FLOAT32 _motorValue;
		
		
		protected override void Awake()
		{
			base.Awake();
			
			enableUpdate = false; // only output.
            Value = _value;
		}
		
		protected override void OnPush()
		{
			Push(_motorValue);
		}

		public override string GetCodeDeclaration()
		{
            string declaration = string.Format("{0} {1}({2:d}, {3:d}, {4:d}, {5:d}", this.GetType().Name, GetCodeVariable(), id, pin1, pin2, pin3);
            if(controlType == ControlType.OnePWM_OneDir)
                declaration += ", OnePWM_OneDir);";
            else if(controlType == ControlType.TwoPWM)
                declaration += ", TwoPWM);";
            else
                declaration += ", OnePWM_TwoDir);";
            
			return declaration;
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("motor{0:d}", id);
		}
		
		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				float newValue = Mathf.Clamp(value, -1f, 1f);
				if(_value != newValue)
				{					
					_value = newValue;
                    if(_value == 0f)
                        _motorValue = 0f;
                    else if(_value > 0f && _value <= punchValue)
                        _motorValue = punchValue;
                    else if(_value < 0f && _value >= -punchValue)
                        _motorValue = -punchValue;
                    else
                        _motorValue = _value;
                    
					SetDirty();
				}
			}
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
			
            nodes.Add(new Node("pin1", "", null, NodeType.None, "Arduino Pin"));
            nodes.Add(new Node("pin2", "", null, NodeType.None, "Arduino Pin"));
            nodes.Add(new Node("pin3", "", null, NodeType.None, "Arduino Pin"));
            nodes.Add(new Node("Value", "Value", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }

        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin1"))
            {
                node.updated = true;
                if(controlType == ControlType.OnePWM_OneDir)
                    node.text = string.Format("Dir Pin: {0:d}", pin1);
                else if(controlType == ControlType.TwoPWM)
                    node.text = string.Format("F PWM Pin: ~ {0:d}", pin1);
                else
                    node.text = string.Format("PWM Pin: ~ {0:d}", pin1);
                
                return;
            }
            else if(node.name.Equals("pin2"))
            {
                node.updated = true;
                if(controlType == ControlType.OnePWM_OneDir)
                    node.text = string.Format("PWM Pin: ~ {0:d}", pin2);
                else if(controlType == ControlType.TwoPWM)
                    node.text = string.Format("B PWM Pin: ~ {0:d}", pin2);
                else
                    node.text = string.Format("F Dir Pin: {0:d}", pin2);
                
                return;
            }
            else if(node.name.Equals("pin3"))
            {
                node.updated = true;
                if(controlType == ControlType.OnePWM_OneDir)
                    node.text = "";
                else if(controlType == ControlType.TwoPWM)
                    node.text = "";
                else
                    node.text = string.Format("B Dir Pin: {0:d}", pin3);
                
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
