using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Physics/TorqueReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/torquereactor")]
	public class TorqueReactor : ArdunityReactor
	{
		public Rigidbody rigidBody;
		public Transform axis;
		public float torque;
		public ForceMode forceMode;
		public bool oneShotOnly = false;
		
		private IWireInput<Trigger> _triggerInput;
		private IWireInput<DragData> _dragInput;
		private bool _done = false;
	
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
		
		public void ResetOneShot()
		{
			_done = false; 
		}
		
		private void AddTorque(Trigger value)
		{
			if(!this.enabled)
				return;
				
			if(oneShotOnly && _done)
				return;
			
			_done = true;
			
			if(rigidBody != null)
			{	
				Vector3 up = rigidBody.transform.forward;
				if(axis != null)
					up = axis.forward;
				
				Quaternion q = Quaternion.AngleAxis(torque, up);
				rigidBody.AddTorque(q.eulerAngles, forceMode);
			}
		}
		
		private void AddDragForce(DragData value)
		{
			if(!this.enabled)
				return;
			
			if(!value.isDrag)
			{
				if(rigidBody != null)
				{
					Vector3 up = rigidBody.transform.forward;
					if(axis != null)
						up = axis.forward;
					
					Vector3 t = Quaternion.AngleAxis(Mathf.Abs(value.force), up).eulerAngles;
					if(value.force < 0f)
						t *= -1f;
					rigidBody.AddTorque(t, forceMode);
				}
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("active", "Active", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("activeDrag", "Active by drag", typeof(IWireInput<DragData>), NodeType.WireFrom, "Input<DragData>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("active"))
            {
				node.updated = true;
                if(node.objectTarget == null && _triggerInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_triggerInput))
                        return;
                }
                
                if(_triggerInput != null)
                    _triggerInput.OnWireInputChanged -= AddTorque;
                
                _triggerInput = node.objectTarget as IWireInput<Trigger>;
                if(_triggerInput != null)
                    _triggerInput.OnWireInputChanged += AddTorque;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("activeDrag"))
            {
				node.updated = true;
                if(node.objectTarget == null && _dragInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_dragInput))
                        return;
                }
                
                if(_dragInput != null)
                    _dragInput.OnWireInputChanged -= AddDragForce;
                
                _dragInput = node.objectTarget as IWireInput<DragData>;
                if(_dragInput != null)
                    _dragInput.OnWireInputChanged += AddDragForce;
                else
                    node.objectTarget = null;
                
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
