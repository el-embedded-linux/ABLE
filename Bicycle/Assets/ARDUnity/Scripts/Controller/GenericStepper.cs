using UnityEngine;
using System.Collections.Generic;

using UINT8 = System.Byte;
using INT16 = System.Int16;
using FLOAT32 = System.Single;


namespace Ardunity
{		
	[AddComponentMenu("ARDUnity/Controller/Motor/GenericStepper")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/genericstepper")]
	public class GenericStepper : ArdunityController, IWireOutput<float>
	{
        public enum DriveType
        {
			WAVE,
            FULL_STEP,
			HALF_STEP
        }

		public enum ControlMode
        {
			Speed = 1,
            Angle = 2,
        }
        
        public DriveType driveType = DriveType.FULL_STEP;
		public int step;
		public int gearRatio;
		public int pin1;
		public int pin2;
        public int pin3;
		public int pin4;

		[SerializeField]
        private ControlMode _mode = ControlMode.Angle;
        [SerializeField]
        private FLOAT32 _rpm;
		[SerializeField]
		private FLOAT32 _angle;
		
		
		protected override void Awake()
		{
			base.Awake();
			
			enableUpdate = false; // only output.
		}
		
		protected override void OnPush()
		{
			Push((UINT8)_mode);
			Push(_rpm);
			Push(_angle);
		}

		public override string GetCodeDeclaration()
		{
            string declaration = string.Format("{0} {1}({2:d}, {3:d}", this.GetType().Name, GetCodeVariable(), id, (int)(step * gearRatio));
			if(driveType == DriveType.WAVE)
                declaration += ", WAVE_DRIVE";
			else if(driveType == DriveType.FULL_STEP)
                declaration += ", FULL_STEP_DRIVE";
			else if(driveType == DriveType.HALF_STEP)
                declaration += ", HALF_STEP_DRIVE";
            declaration += string.Format(", {0:d}, {1:d}, {2:d}, {3:d});", pin1, pin2, pin3, pin4);

			return declaration;
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("stepper{0:d}", id);
		}

		public ControlMode mode
		{
			get
			{
				return _mode;
			}
			set
			{
				if(_mode != value)
				{
					_mode = value;
					if(_mode == ControlMode.Speed)
					{
						_rpm = 0;
					}
					else if(_mode == ControlMode.Angle)
					{
						_rpm = (INT16)Mathf.Abs(_rpm);
						_angle = 0;
					}
					SetDirty();
				}
			}
		}

		public float rpm
		{
			get
			{
				return _rpm;
			}
			set
			{
				if(_rpm != (FLOAT32)value)
				{
					_rpm = (FLOAT32)value;
					SetDirty();
				}
			}
		}
		
		public float angle
		{
			get
			{
				return (float)_angle;
			}
			set
			{
				if(_angle != (FLOAT32)value)
				{
					_angle = (FLOAT32)value;
					SetDirty();
				}
			}
		}
		
        #region Wire Editor
		float IWireOutput<float>.output
		{
			get
			{
				if(_mode == ControlMode.Speed)
					return rpm;
				else
					return angle;
			}
			set
			{
				if(_mode == ControlMode.Angle)
					rpm = (int)value;
				else
					angle = value;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("pin1", "", null, NodeType.None, "Arduino Pin"));
            nodes.Add(new Node("pin2", "", null, NodeType.None, "Arduino Pin"));
            nodes.Add(new Node("pin3", "", null, NodeType.None, "Arduino Pin"));
			nodes.Add(new Node("pin4", "", null, NodeType.None, "Arduino Pin"));
            nodes.Add(new Node("value", "", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }

        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin1"))
            {
                node.updated = true;
				node.text = string.Format("Pin1: {0:d}", pin1);
                
                return;
            }
            else if(node.name.Equals("pin2"))
            {
                node.updated = true;
				node.text = string.Format("Pin2: {0:d}", pin2);
                
                return;
            }
            else if(node.name.Equals("pin3"))
            {
                node.updated = true;
				node.text = string.Format("Pin3: {0:d}", pin3);
                
                return;
            }
			else if(node.name.Equals("pin4"))
            {
                node.updated = true;
				node.text = string.Format("Pin4: {0:d}", pin4);
                
                return;
            }
            else if(node.name.Equals("value"))
            {
                node.updated = true;
				if(_mode == ControlMode.Speed)
					node.text = "RPM";
                else
                    node.text = "Angle";
				
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
    }
}
