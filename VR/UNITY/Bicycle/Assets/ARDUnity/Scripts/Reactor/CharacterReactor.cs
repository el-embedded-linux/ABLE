using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Reactor/Miscellaneous/CharacterReactor")]
	public class CharacterReactor : ArdunityReactor
	{
		public Transform vrCamera;
		public Transform weapon;
		public float moveScale = 1f;
        public float aheadScale = 1f;
		public float jumpForce = 1f;
        public string groundTag = "Untagged";
        public bool usePhysics = false;

		private IWireInput<Vector2> _axisWire;
		private IWireInput<Quaternion> _rotationWire;
		private IWireInput<Trigger> _jumpWire;

		private Rigidbody _rigidBody;
        private bool _flying = true;
		private Quaternion _initRot;
        private Quaternion _curRotation;
		private Quaternion _fromRotation;
		private Quaternion _toRotation;
		private Vector2 _axis = Vector2.zero;
        private Vector3 _cameraDelta = Vector3.zero;
		private float _time;
		
		protected override void Awake()
		{
            base.Awake();

			_rigidBody = GetComponent<Rigidbody>();

			_initRot = transform.localRotation;
            _curRotation = Quaternion.identity;
            _toRotation =  Quaternion.identity;
            _time = 1f;
		}

		// Use this for initialization
		void Start ()
		{
			if(vrCamera != null)
            {
                Vector3 delta = vrCamera.position - transform.position;
                _cameraDelta.x = Vector3.Project(delta, transform.right).magnitude;
                _cameraDelta.y = Vector3.Project(delta, transform.up).magnitude;
                _cameraDelta.z = Vector3.Project(delta, transform.forward).magnitude;
            }
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_time < 1f)
			{
				_time += Time.deltaTime;
				_curRotation = Quaternion.Lerp(_fromRotation, _toRotation, _time);
			}
			else
				_curRotation = _toRotation;
            
            if(vrCamera != null)
            {
                if(_rotationWire == null)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(vrCamera.forward, Vector3.up), Vector3.up);
                }
                else
                {
                    Vector3 dir = _curRotation * Vector3.forward;
                    dir = Vector3.ProjectOnPlane(dir, Vector3.up);
                    Quaternion q = Quaternion.LookRotation(dir, Vector3.up);
                    transform.localRotation = _initRot * q;

                    if(weapon != null)
                        weapon.localRotation = _curRotation * Quaternion.Inverse(q);
                }
                
                if(usePhysics)
                {
                    if(_rigidBody != null)
                    {
                        Vector3 vel = _rigidBody.velocity;
                        if(!_flying)
                        {
                            Vector3 dir = new Vector3(_axis.x, 0f, _axis.y) * (moveScale * Time.deltaTime);
                            dir = transform.rotation * dir;
                            vel.x = dir.x;
                            vel.z = dir.z;
                            _rigidBody.velocity = vel;                        
                        }
                    }
                }
                else
                {
                    Vector3 dir = new Vector3(_axis.x, 0f, _axis.y) * (moveScale * Time.deltaTime);
                    dir = transform.rotation * dir;
                    transform.Translate(dir);
                }

                vrCamera.position = transform.position + transform.rotation * _cameraDelta;
            }
            else
            {
                if(weapon != null)
                    weapon.localRotation = _curRotation;
                
                if(usePhysics)
                {
                    if(_rigidBody != null)
                    {
                        Vector3 vel = _rigidBody.velocity;
                        if(!_flying)
                        {
                            transform.Rotate(Vector3.up * (_axis.x * aheadScale * Time.deltaTime), Space.Self);
                
                            Vector3 dir = new Vector3(0f, 0f, _axis.y) * (moveScale * Time.deltaTime);
                            dir = transform.rotation * dir;
                            vel.x = dir.x;
                            vel.z = dir.z;
                            _rigidBody.velocity = vel;
                        }
                    }
                }
                else
                {
                    transform.Rotate(Vector3.up * (_axis.x * aheadScale * Time.deltaTime), Space.Self);
                
                    Vector3 dir = new Vector3(0f, 0f, _axis.y) * (moveScale * Time.deltaTime);
                    dir = transform.rotation * dir;
                    transform.Translate(dir, Space.World);
                }
            }        
		}

        void OnCollisionStay(Collision other)
        {
            if(other.transform.tag == groundTag)
                _flying = false;
        }

        void OnCollisionExit(Collision other)
        {
            if(other.transform.tag == groundTag)
                _flying = true;
        }
        
        private void OnAxisChanged(Vector2 value)
        {
			_axis = value;
        }

		private void OnRotationChanged(Quaternion value)
        {
			_fromRotation = _toRotation;
			_toRotation =  value;
            
            _time = 0f;
        }

		private void OnJump(Trigger value)
        {
			if(_rigidBody == null || !usePhysics || _flying)
				return;
			
			_rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("axis", "Axis", typeof(IWireInput<Vector2>), NodeType.WireFrom, "Input<Vector2>"));
			nodes.Add(new Node("rotation", "Rotation", typeof(IWireInput<Quaternion>), NodeType.WireFrom, "Input<Quaternion>"));
			nodes.Add(new Node("jump", "Jump", typeof(IWireInput<Trigger>), NodeType.WireFrom, "Input<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("axis"))
            {
                node.updated = true;
                if(node.objectTarget == null && _axisWire == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_axisWire))
                        return;
                }
                
                if(_axisWire != null)
                    _axisWire.OnWireInputChanged -= OnAxisChanged;
                
                _axisWire = node.objectTarget as IWireInput<Vector2>;
                if(_axisWire != null)
                    _axisWire.OnWireInputChanged += OnAxisChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("rotation"))
            {
                node.updated = true;
                if(node.objectTarget == null && _rotationWire == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_rotationWire))
                        return;
                }
                
                if(_rotationWire != null)
                    _rotationWire.OnWireInputChanged -= OnRotationChanged;
                
                _rotationWire = node.objectTarget as IWireInput<Quaternion>;
                if(_rotationWire != null)
                    _rotationWire.OnWireInputChanged += OnRotationChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("jump"))
            {
                node.updated = true;
                if(node.objectTarget == null && _jumpWire == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_jumpWire))
                        return;
                }
                
                if(_jumpWire != null)
                    _jumpWire.OnWireInputChanged -= OnJump;
                
                _jumpWire = node.objectTarget as IWireInput<Trigger>;
                if(_jumpWire != null)
                    _jumpWire.OnWireInputChanged += OnJump;
                else
                    node.objectTarget = null;
                
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}
