using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Device/DeviceRotationReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/devicerotationreactor")]
	public class DeviceRotationReactor : ArdunityReactor, IWireInput<Quaternion>
	{
        public UnityEvent OnActive;
        public UnityEvent OnDeactive;
        
		private IWireOutput<Quaternion> _rotationWire;
        private Quaternion _rotation = Quaternion.identity;
        private Quaternion _cameraBase = Quaternion.Euler(90f, 0f, 0f);
        private Quaternion _aheadBase = Quaternion.identity;
        
        protected override void Awake()
		{
            base.Awake();
            
			if(!this.enabled)
                OnDeactive.Invoke();
		}

		// Use this for initialization
		void Start ()
		{
            Input.gyro.enabled = true;
		}
		
		// Update is called once per frame
		void Update ()
		{
            if(Input.gyro.enabled)
            {
                Quaternion q = Input.gyro.attitude;
                q = new Quaternion(q.x, q.y, -q.z, -q.w);
                q = _aheadBase * _cameraBase * q;

                if(_rotation != q)
                {
                    _rotation = q;

                    if(_rotationWire != null)
                        _rotationWire.output = rotation;
                
                    if(OnWireInputChanged != null)
                        OnWireInputChanged(rotation);
                }
            }
		}
				
		void OnEnable()
        {
            Input.gyro.enabled = true;

            if(_rotationWire != null)
                _rotationWire.output = rotation;
			
			if(OnWireInputChanged != null)
				OnWireInputChanged(rotation);
                    
            OnActive.Invoke();
        }
        
        void OnDisable()
        {
            OnDeactive.Invoke();
        }

        public void Calibration()
        {
            Quaternion q = Input.gyro.attitude;
            q = new Quaternion(q.x, q.y, -q.z, -q.w);
            q = _cameraBase * q;

			Vector3 heading = q * Vector3.forward;
			heading = Vector3.ProjectOnPlane(heading, Vector3.up);
			float angle = Vector3.Angle(heading, Vector3.forward);
			if(Vector3.Dot(Vector3.Cross(heading, Vector3.forward), Vector3.up) < 0f)
				angle = -angle;
			
			_aheadBase = Quaternion.AngleAxis(angle, Vector3.up);
        }
        
        public Quaternion rotation
        {
            get
            {
				return _rotation;
            }
        }

        #region Wire Editor
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
			
			nodes.Add(new Node("rotation", "Rotation", typeof(IWireOutput<Quaternion>), NodeType.WireFrom, "Output<Quaternion>"));
            nodes.Add(new Node("rotation2", "Rotation", typeof(IWireInput<Quaternion>), NodeType.WireTo, "Input<Quaternion>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("rotation"))
            {
                node.updated = true;
                if(node.objectTarget == null && _rotationWire == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_rotationWire))
                        return;
                }
                
                _rotationWire = node.objectTarget as IWireOutput<Quaternion>;
                if(_rotationWire == null)
                    node.objectTarget = null;

                return;
            }
            else if(node.name.Equals("rotation2"))
            {
				node.updated = true;
				return;
			}
                        
            base.UpdateNode(node);
        }
        #endregion
	}
}
