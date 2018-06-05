using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Physics/ColliderReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/colliderreactor")]
	public class ColliderReactor : ArdunityReactor
	{
		private IWireOutput<bool> _triggerOutput;
        private IWireOutput<Trigger> _collisionOutput;

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
				
		void OnTriggerEnter(Collider other)
		{
			DoTriggerEnter();
		}
		
		void OnTriggerExit(Collider other)
		{
			DoTriggerExit();
		}
		
		void OnTriggerEnter2D(Collider2D other)
		{
			DoTriggerEnter();
		}
		
		void OnTriggerExit2D(Collider2D other)
		{
			DoTriggerExit();
		}
		
		void OnCollisionEnter(Collision other)
		{
			DoCollisionEnter();
		}
		
		void OnCollisionEnter2D(Collision2D other)
		{
			DoCollisionEnter();
		}
		
		private void DoTriggerEnter()
		{
			if(_triggerOutput != null)
				_triggerOutput.output = true;
		}
		
		private void DoTriggerExit()
		{
			if(_triggerOutput != null)
				_triggerOutput.output = false;
		}
        
        private void DoCollisionEnter()
		{
            if(_collisionOutput != null)
				_collisionOutput.output = new Trigger();
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("getTrigger", "Get Trigger", typeof(IWireOutput<bool>), NodeType.WireFrom, "Output<bool>"));
            nodes.Add(new Node("getCollision", "Get Collision", typeof(IWireOutput<Trigger>), NodeType.WireFrom, "Output<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("getTrigger"))
            {
				node.updated = true;
                if(node.objectTarget == null && _triggerOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_triggerOutput))
                        return;
                }
                
                _triggerOutput = node.objectTarget as IWireOutput<bool>;
                if(_triggerOutput == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("getCollision"))
            {
				node.updated = true;
                if(node.objectTarget == null && _collisionOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_collisionOutput))
                        return;
                }
                
                _collisionOutput = node.objectTarget as IWireOutput<Trigger>;
                if(_collisionOutput == null)
                    node.objectTarget = null;
                
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
