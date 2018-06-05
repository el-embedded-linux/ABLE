using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Input/MappingInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/mappinginput")]
	public class MappingInput : ArdunityBridge, IWireInput<float>
	{
		public string sourceName = "Mapping Source";
        public string resultName = "Mapping Result";
		public AnimationCurve mapCurve;
		
		private IWireInput<float> _sourceInput;
		private float _analogValue;
	
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		public float Value
		{
			get
			{
				return mapCurve.Evaluate(_analogValue);
			}
		}
		
		private void SourceInputChanged(float value)
		{
			_analogValue = Mathf.Abs(value);
			if(OnWireInputChanged != null)
				OnWireInputChanged(Value);
		}
		
		public event WireEventHandler<float> OnWireInputChanged;

        float IWireInput<float>.input
        {
			get
			{
				return Value;
			}
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("source", "", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("result", "", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("source"))
            {
				node.updated = true;
                node.text = sourceName;
                
                if(node.objectTarget == null && _sourceInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_sourceInput))
                        return;
                }
                
                if(_sourceInput != null)
                    _sourceInput.OnWireInputChanged -= SourceInputChanged;
                
                _sourceInput = node.objectTarget as IWireInput<float>;;
                if(_sourceInput != null)
                    _sourceInput.OnWireInputChanged += SourceInputChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("result"))
            {
				node.updated = true;
                node.text = resultName;
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}