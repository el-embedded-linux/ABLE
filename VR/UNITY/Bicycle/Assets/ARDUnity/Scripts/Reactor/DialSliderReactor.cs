using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/UI/DialSliderReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/dialsliderreactor")]
	[RequireComponent(typeof(DialSlider))]
	public class DialSliderReactor : ArdunityReactor
	{
		public bool invert = false;
		
		private DialSlider _dialSlider;
		private IWireOutput<float> _analogOutput;
		
		protected override void Awake()
		{
            base.Awake();
            
			_dialSlider = GetComponent<DialSlider>();
			_dialSlider.OnAngleChanged.AddListener(OnDialSliderChanged);
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		void OnEnable()
		{
			if(_analogOutput != null)
			{
				if(invert)
					_analogOutput.output = -_dialSlider.angle;
				else
					_analogOutput.output = _dialSlider.angle;
			}
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
		
		private void OnDialSliderChanged(float value)
		{
			if(_analogOutput != null)
			{
				if(invert)
					_analogOutput.output = -value;
				else
					_analogOutput.output = value;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("getAngle", "Get Angle", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("getAngle"))
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
                        
            base.UpdateNode(node);
        }
	}
}
