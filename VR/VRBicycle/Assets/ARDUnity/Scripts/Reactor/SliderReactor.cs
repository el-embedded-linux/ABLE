using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/UI/SliderReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/sliderreactor")]
	[RequireComponent(typeof(Slider))]
	public class SliderReactor : ArdunityReactor
	{
		private Slider _slider;
		private IWireInput<float> _analogInput;
		private IWireOutput<float> _analogOutput;
		
		protected override void Awake()
		{
            base.Awake();
            
			_slider = GetComponent<Slider>();
			_slider.onValueChanged.AddListener(OnSliderChanged);
		}

		// Use this for initialization
		void Start ()
		{
		}
		
		void OnEnable()
		{
			if(_analogInput != null)
				_slider.value = _analogInput.input;
			
			if(_analogOutput != null)
				_analogOutput.output = _slider.value;
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
		
		private void OnSliderChanged(float value)
		{
			if(_analogOutput != null)
				_analogOutput.output = value;
		}
		
		private void OnWireInputChanged(float value)
		{
			if(!this.enabled)
				return;
				
			_slider.value = _analogInput.input;
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
 			nodes.Add(new Node("setValue", "Set Value", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("getValue", "Get Value", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("setValue"))
            {
				node.updated = true;
                if(node.objectTarget == null && _analogInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogInput))
                        return;
                }
                
                if(_analogInput != null)
                    _analogInput.OnWireInputChanged -= OnWireInputChanged;
                
                _analogInput = node.objectTarget as IWireInput<float>;
                if(_analogInput != null)
                    _analogInput.OnWireInputChanged += OnWireInputChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("getValue"))
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
