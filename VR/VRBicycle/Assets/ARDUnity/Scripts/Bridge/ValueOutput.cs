using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Output/ValueOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/valueoutput")]
	public class ValueOutput : ArdunityBridge, IWireOutput<Trigger>
	{
		public float value;
		
		private IWireOutput<float> _analogOutput;
        private Trigger _value;
		
		protected override void Awake()
		{
            base.Awake();
            
            _value = new Trigger();
            _value.Clear();
		}
	
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		Trigger IWireOutput<Trigger>.output
        {
			get
			{
				return _value;
			}
            set
            {
                if(value.value)
                {
                    if(_analogOutput != null)
						_analogOutput.output = this.value;
                }
								
				_value = value;
            }
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("value", "", null, NodeType.None, "Value"));
            nodes.Add(new Node("output", "Value", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("active", "Active", typeof(IWireOutput<Trigger>), NodeType.WireTo, "Output<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("value"))
            {
                node.updated = true;
                node.text = value.ToString();
                return;
            }
            else if(node.name.Equals("output"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogOutput))
                        return;
                }
                
                _analogOutput = node.objectTarget as IWireOutput<float>;
                if(_analogOutput == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("active"))
            {
				node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
