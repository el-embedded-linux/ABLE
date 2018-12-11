using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Probe/FloatProbe")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/floatprobe")]
	public class FloatProbe : ArdunityBridge, IWireInput<float>
	{
		private IWireOutput<float> _floatOutput;
        private float _value;
        
        #region MonoBehavior
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
            if(_floatOutput != null)
            {
                float value = _floatOutput.output;
                if(_value != value)
                {
                    _value = value;
                    if(OnWireInputChanged != null)
                        OnWireInputChanged(_value);
                }
            }
		}
        #endregion
		
        #region Wire Editor
		public event WireEventHandler<float> OnWireInputChanged;

        float IWireInput<float>.input
        {
			get
			{
				return _value;
			}
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("floatOutput", "Output(float)", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
            nodes.Add(new Node("floatInput", "Input(float)", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("floatOutput"))
            {
                node.updated = true;
                if(node.objectTarget == null && _floatOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_floatOutput))
                        return;
                }
                
                _floatOutput = node.objectTarget as IWireOutput<float>;
                if(_floatOutput == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("floatInput"))
            {
                node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}

