using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Device/DeviceRollReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/devicerollreactor")]
	public class DeviceRollReactor : ArdunityReactor, IWireInput<float>
	{
        public bool invert = false;
        
        public UnityEvent OnActive;
        public UnityEvent OnDeactive;
        
		private IWireOutput<float> _rollOutput;
        private float _rollAngle = 0f;
        
        protected override void Awake()
		{
            base.Awake();
            
			if(!this.enabled)
                OnDeactive.Invoke();
		}

		// Use this for initialization
		void Start ()
		{            
		}
		
		// Update is called once per frame
		void Update ()
		{
            Vector2 origin = new Vector2(0f, 1f);
            Vector2 current = new Vector2(Input.acceleration.x, -Input.acceleration.y);
            float angle = Vector2.Angle(origin, current);
            if(current.x > 0f)
                angle = -angle;
 
            if(angle != _rollAngle)
            {
                _rollAngle = angle;
                if(_rollOutput != null)
                    _rollOutput.output = rollAngle;
            }
		}
				
		void OnEnable()
        {
            if(_rollOutput != null)
                _rollOutput.output = rollAngle;
                    
            OnActive.Invoke();
        }
        
        void OnDisable()
        {
            OnDeactive.Invoke();
        }
        
        public float rollAngle
        {
            get
            {
                if(invert)
                    return -_rollAngle;
                else
                    return _rollAngle;
            }
        }

        #region Wire Editor
        public event WireEventHandler<float> OnWireInputChanged;
		
		float IWireInput<float>.input
		{
			get
			{
				return rollAngle;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("rollAngle", "Roll Angle", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
            nodes.Add(new Node("rollAngle2", "Roll Angle", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("rollAngle"))
            {
                node.updated = true;
                if(node.objectTarget == null && _rollOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_rollOutput))
                        return;
                }
                
                _rollOutput = node.objectTarget as IWireOutput<float>;
                if(_rollOutput == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("rollAngle2"))
            {
				node.updated = true;
				return;
			}
                        
            base.UpdateNode(node);
        }
        #endregion
	}
}
