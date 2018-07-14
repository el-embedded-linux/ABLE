using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Diagnostics;

using FLOAT32 = System.Single;


namespace Ardunity
{    
	[AddComponentMenu("ARDUnity/Controller/AHRS/MPUSeries")]
	[HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/mpuseries")]
	public class MPUSeries : ArdunityController, IWireInput<Quaternion>
	{
		public enum MODEL
		{
			MPU6050,
			MPU6500,
			MPU9150,
			MPU9250
		}

		public MODEL model = MODEL.MPU6050;
		public bool secondary = false;
		public Vector3 forward = Vector3.forward;
		public Vector3 up = Vector3.up;

		public UnityEvent OnStartCalibration;
		public UnityEvent OnCalibrated;

		private Quaternion _rotation = Quaternion.identity;
		private Quaternion _calibrated_rotation;
		private Quaternion _preRotation;
		private bool _calibrating = false;
		private Stopwatch _stopwatch = new Stopwatch();
		private FLOAT32 _qX;
		private FLOAT32 _qY;
		private FLOAT32 _qZ;
		private FLOAT32 _qW;
		
		protected override void OnConnected()
		{
			OnReset();
		}

		protected override void OnReset()
		{
			_calibrated_rotation = Quaternion.identity;
			_preRotation = Quaternion.identity;
			_calibrating = true;
			_stopwatch.Reset();			
			_stopwatch.Start();

			OnStartCalibration.Invoke();
		}

		protected override void OnExecuted()
		{
			Quaternion q = new Quaternion(_qX, _qY, _qZ, _qW);
			// Convert Axis XYZ -> XZY and Rotation direction right -> left
			Vector3 angles = q.eulerAngles;
			float tmp_y = angles.y;
			angles.y = angles.z;
			angles.z = tmp_y;
			q.eulerAngles = angles;
			q = Quaternion.Inverse(q);

			if(_calibrating)
			{
				if(_preRotation == Quaternion.identity)
					_preRotation = q;

				if(_stopwatch.ElapsedMilliseconds > 1000)
				{				
					float compareAngle = Quaternion.Angle(q, _preRotation);
					if(compareAngle < 0.5f)
					{
						float dot = Vector3.Dot(q * Vector3.forward, _preRotation * Vector3.forward);						
						if(dot > 0.99f)
						{
							_calibrating = false;
							_stopwatch.Stop();
							Calibration();
						}
					}

					_preRotation = q;
					
					if(_calibrating)
					{
						_stopwatch.Reset();
						_stopwatch.Start();
					}
				}
			}

			_rotation = _calibrated_rotation * q;
			
			if(OnWireInputChanged != null)
				OnWireInputChanged(rotation);
		}

		protected override void OnPop()
		{
			Pop(ref _qX);
			Pop(ref _qY);
			Pop(ref _qZ);
			Pop(ref _qW);
			updated = true;
		}
		
		public override string GetCodeDeclaration()
		{
			string modelDefine = "";
			if(model == MODEL.MPU6050)
				modelDefine = "MPU6050";
			else if(model == MODEL.MPU6500)
				modelDefine = "MPU6500";
			else if(model == MODEL.MPU9150)
				modelDefine = "MPU9150";
			else if(model == MODEL.MPU9250)
				modelDefine = "MPU9250";
			
			string secondaryString = "false";
			if(secondary)
				secondaryString = "true";

			Quaternion q = Quaternion.LookRotation(forward, up);
			// Convert Axis XYZ -> XZY and Rotation direction right -> left
			Vector3 angles = q.eulerAngles;
			float tmp_y = angles.y;
			angles.y = angles.z;
			angles.z = tmp_y;
			q.eulerAngles = angles;
			q = Quaternion.Inverse(q);

			Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);

			string orientName = string.Format("orient_{0}", GetCodeVariable());
			string code = string.Format("signed char {0}[9] = {{ {1:d}, {2:d}, {3:d}, {4:d}, {5:d}, {6:d}, {7:d}, {8:d}, {9:d} }};"
										,orientName, (int)Mathf.Round(m[0, 0]), (int)Mathf.Round(m[0, 1]), (int)Mathf.Round(m[0, 2])
										,(int)Mathf.Round(m[1, 0]), (int)Mathf.Round(m[1, 1]), (int)Mathf.Round(m[1, 2])
										,(int)Mathf.Round(m[2, 0]), (int)Mathf.Round(m[2, 1]), (int)Mathf.Round(m[2, 2]));
			code += "\n";
			code += string.Format("{0} {1}({2:d}, {3}, {4}, {5});", this.GetType().Name, GetCodeVariable(), id, modelDefine, secondaryString, orientName);
			return code;
		}
		
		public override string GetCodeVariable()
		{
			string modelName = "";
			if(model == MODEL.MPU6050)
				modelName = "mpu6050";
			else if(model == MODEL.MPU6500)
				modelName = "mpu6500";
			else if(model == MODEL.MPU9150)
				modelName = "mpu9150";
			else if(model == MODEL.MPU9250)
				modelName = "mpu9250";
			
			return string.Format("{0}_{1:d}", modelName, id);
		}
		
		public override string[] GetAdditionalFiles()
		{
			List<string> additionals = new List<string>();
			additionals.Add("ArdunityI2C.h");
			additionals.Add("ArdunityI2C.cpp");
			return additionals.ToArray();
		}

		public bool calibrating
		{
			get
			{
				return _calibrating;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return _rotation;
			}
		}

		public void Calibration()
		{
			if(_calibrating)
				return;
			
			Quaternion q = new Quaternion(_qX, _qY, _qZ, _qW);
			// Convert Axis XYZ -> XZY and Rotation direction right -> left
			Vector3 angles = q.eulerAngles;
			float tmp_y = angles.y;
			angles.y = angles.z;
			angles.z = tmp_y;
			q.eulerAngles = angles;
			q = Quaternion.Inverse(q);

			Vector3 heading = q * Vector3.forward;
			heading = Vector3.ProjectOnPlane(heading, Vector3.up);
			float angle = Vector3.Angle(heading, Vector3.forward);
			if(Vector3.Dot(Vector3.Cross(heading, Vector3.forward), Vector3.up) < 0f)
				angle = -angle;
			
			_calibrated_rotation = Quaternion.AngleAxis(angle, Vector3.up);
			_rotation = _calibrated_rotation * _preRotation;

			OnCalibrated.Invoke();
		}

		public event WireEventHandler<Quaternion> OnWireInputChanged;

		Quaternion IWireInput<Quaternion>.input
		{
			get
			{
				return rotation;
			}
		}

		protected override void AddNode(List<Node> nodes)
		{
			base.AddNode(nodes);

			nodes.Add(new Node("model", "", null, NodeType.None, "Model Name"));
			nodes.Add(new Node("secondary", "", null, NodeType.None, "Secondary"));
			nodes.Add(new Node("rotation", "Rotation", typeof(IWireInput<Quaternion>), NodeType.WireTo));
		}
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("model"))
            {
				node.updated = true;
				if(model == MODEL.MPU6050)
					node.text = "MPU6050";
				else if(model == MODEL.MPU6500)
					node.text = "MPU6500";
				else if(model == MODEL.MPU9150)
					node.text = "MPU9150";
				else if(model == MODEL.MPU9250)
					node.text = "MPU9250";

                return;
            }
            else if(node.name.Equals("secondary"))
            {
				node.updated = true;
				if(secondary)
					node.text = "Secondary";
				else
					node.text = "";
				
                return;
            }
			else if(node.name.Equals("rotation"))
            {
				node.updated = true;
				return;
			}
            
            base.UpdateNode(node);
        }
	}
}
