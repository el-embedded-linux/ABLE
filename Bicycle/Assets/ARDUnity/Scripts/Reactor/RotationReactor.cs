using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Reactor/Transform/RotationReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/rotationreactor")]
	public class RotationReactor : ArdunityReactor
	{
        public bool smoothFollow = true;
		
		private Quaternion _initRot;
		private IWireInput<Quaternion> _rotation;
        private Quaternion _curRotation;
		private Quaternion _fromRotation;
		private Quaternion _toRotation;
		private float _time;
		
		protected override void Awake()
		{
            base.Awake();
			
			_initRot = transform.localRotation;
            _curRotation = Quaternion.identity;
            _toRotation =  Quaternion.identity;
            _time = 1f;
		}

		// Use this for initialization
		void Start ()
		{
					
		}
		
		// Update is called once per frame
		void Update ()
		{
            if(_time < 1f && smoothFollow == true)
			{
				_time += Time.deltaTime;
				_curRotation = Quaternion.Lerp(_fromRotation, _toRotation, _time);
			}
			else
				_curRotation = _toRotation;
			
            transform.localRotation = _initRot * _curRotation;
		}
        
        private void OnRotationChanged(Quaternion q)
        {
            _fromRotation = _toRotation;
			_toRotation =  q;
            
            _time = 0f;
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("rotation", "Rotation", typeof(IWireInput<Quaternion>), NodeType.WireFrom, "Input<Quaternion>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("rotation"))
            {
                node.updated = true;
                if(node.objectTarget == null && _rotation == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_rotation))
                        return;
                }
                
                if(_rotation != null)
                    _rotation.OnWireInputChanged -= OnRotationChanged;
                
                _rotation = node.objectTarget as IWireInput<Quaternion>;
                if(_rotation != null)
                    _rotation.OnWireInputChanged += OnRotationChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}
