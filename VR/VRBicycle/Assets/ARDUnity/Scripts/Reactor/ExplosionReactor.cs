using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Physics/ExplosionReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/explosionreactor")]
	public class ExplosionReactor : ArdunityReactor
	{
        public float effectRadius = 1f;
		public float explosionForce = 1f;
		public bool oneShotOnly = false;
		public LayerMask layerMask;
		
		private IWireInput<Trigger> _triggerInput;
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
		
		private void DoExplosion(Trigger value)
		{
			if(!this.enabled)
				return;
				
			if(oneShotOnly && _done)
				return;
			
			_done = true;
			
			Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, layerMask);
			foreach(Collider col in colliders)
			{
				Rigidbody rigid = col.GetComponent<Rigidbody>();
				if(rigid == null)
					continue;				
				rigid.AddExplosionForce(explosionForce, transform.position, effectRadius, 1f, ForceMode.Impulse);
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("active", "Active", typeof(IWireInput<Trigger>), NodeType.WireFrom, "Input<Trigger>"));
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
                    _triggerInput.OnWireInputChanged -= DoExplosion;
                
                _triggerInput = node.objectTarget as IWireInput<Trigger>;
                if(_triggerInput != null)
                    _triggerInput.OnWireInputChanged += DoExplosion;
                else
                    node.objectTarget = null;
                
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}
