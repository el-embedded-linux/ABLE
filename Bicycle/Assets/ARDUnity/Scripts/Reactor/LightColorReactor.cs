using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/Effect/LightColorReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/lightcolorreactor")]
	[RequireComponent(typeof(Light))]
	public class LightColorReactor : ArdunityReactor
	{	
		private Light _light;
		private IWireInput<Color> _colorInput;
		private IWireOutput<Color> _colorOutput;
		
		protected override void Awake()
		{
            base.Awake();
            
			_light = GetComponent<Light>();
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_colorOutput != null)
				_colorOutput.output = _light.color;
		}
		
		private void OnInputColorChanged(Color value)
		{
			if(!this.enabled)
				return;
				
			_light.color = _colorInput.input;
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
 			nodes.Add(new Node("setColor", "Set Color", typeof(IWireInput<Color>), NodeType.WireFrom, "Input<Color>"));
			nodes.Add(new Node("getColor", "Get Color", typeof(IWireOutput<Color>), NodeType.WireFrom, "Output<Color>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("setColor"))
            {
                node.updated = true;
                if(node.objectTarget == null && _colorInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_colorInput))
                        return;
                }
                
                if(_colorInput != null)
                    _colorInput.OnWireInputChanged -= OnInputColorChanged;
                
                _colorInput = node.objectTarget as IWireInput<Color>;
                if(_colorInput != null)
                    _colorInput.OnWireInputChanged += OnInputColorChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("getColor"))
            {
                node.updated = true;
                if(node.objectTarget == null && _colorOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_colorOutput))
                        return;
                }
                
                _colorOutput = node.objectTarget as IWireOutput<Color>;
                if(_colorOutput == null)
                    node.objectTarget = null;
                
                return;
            }
                            
            base.UpdateNode(node);
        }
	}
}
