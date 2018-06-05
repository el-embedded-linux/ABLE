using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Physics/ForceReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/forcereactor")]
	public class ForceReactor : ArdunityReactor
	{
		public Rigidbody rigidBody;
		public Transform position;
		public Transform direction;
		public float force;
		public ForceMode forceMode;
		public bool oneShotOnly = false;
		
		private IWireInput<Trigger> _triggerInput;
		private IWireInput<DragData> _dragInput;
		private IWireInput<float> _impulseInput;
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
		
		private void AddForce(Trigger value)
		{
			if(!this.enabled)
				return;
				
			if(oneShotOnly && _done)
				return;
			
			_done = true;
			
			if(rigidBody != null)
			{
				Vector3 pos = rigidBody.transform.position;
				if(position != null)
					pos = position.position;
				
				Vector3 dir = rigidBody.transform.forward;
				if(direction != null)
					dir = direction.forward;
				
				rigidBody.AddForceAtPosition(dir * force, pos, forceMode);
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
					Vector3 pos = rigidBody.transform.position;
					if(position != null)
						pos = position.position;
					
					Vector3 dir = rigidBody.transform.forward;
					if(direction != null)
						dir = direction.forward;
					
					rigidBody.AddForceAtPosition(dir * value.force, pos, forceMode);
				}
			}
		}

		private void AddImpulseForce(float value)
		{
			if(!this.enabled)
				return;
			
			if(rigidBody != null)
			{
				Vector3 pos = rigidBody.transform.position;
				if(position != null)
					pos = position.position;
				
				Vector3 dir = rigidBody.transform.forward;
				if(direction != null)
					dir = direction.forward;
				
				rigidBody.AddForceAtPosition(dir * (value * force), pos, forceMode);
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("active", "Active", typeof(IWireInput<Trigger>), NodeType.WireFrom, "Input<Trigger>"));
			nodes.Add(new Node("activeDrag", "Active by drag", typeof(IWireInput<DragData>), NodeType.WireFrom, "Input<DragData>"));
			nodes.Add(new Node("activeImpulse", "Active by impulse", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
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
                    _triggerInput.OnWireInputChanged -= AddForce;
                
                _triggerInput = node.objectTarget as IWireInput<Trigger>;
                if(_triggerInput != null)
                    _triggerInput.OnWireInputChanged += AddForce;
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
			else if(node.name.Equals("activeImpulse"))
            {
				node.updated = true;
                if(node.objectTarget == null && _impulseInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_impulseInput))
                        return;
                }
                
                if(_impulseInput != null)
                    _impulseInput.OnWireInputChanged -= AddImpulseForce;
                
                _impulseInput = node.objectTarget as IWireInput<float>;
                if(_impulseInput != null)
                    _impulseInput.OnWireInputChanged += AddImpulseForce;
                else
                    node.objectTarget = null;
                
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
