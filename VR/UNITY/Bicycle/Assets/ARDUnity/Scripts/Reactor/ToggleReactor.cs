using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/UI/ToggleReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/togglereactor")]
	[RequireComponent(typeof(Toggle))]
	public class ToggleReactor : ArdunityReactor
	{
		private Toggle _toggle;
		private IWireInput<bool> _digitalInput;
		private IWireOutput<bool> _digitalOutput;
		
		protected override void Awake()
		{
            base.Awake();
            
			_toggle = GetComponent<Toggle>();
			_toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		// Use this for initialization
		void Start ()
		{			
		}
		
		void OnEnable()
		{		
			if(_digitalInput != null)
				_toggle.isOn = _digitalInput.input;
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
		
		private void OnToggleChanged(bool value)
		{
			if(_digitalOutput != null)
				_digitalOutput.output = value;
		}
		
		private void OnWireInputChanged(bool value)
		{
			if(!this.enabled)
				return;
				
			_toggle.isOn = _digitalInput.input;
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("setValue", "Set Value", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("getValue", "Get Value", typeof(IWireOutput<bool>), NodeType.WireFrom, "Output<bool>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("setValue"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalInput))
                        return;
                }
                
                if(_digitalInput != null)
                    _digitalInput.OnWireInputChanged -= OnWireInputChanged;
                
                _digitalInput = node.objectTarget as IWireInput<bool>;
                if(_digitalInput != null)
                    _digitalInput.OnWireInputChanged += OnWireInputChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("getValue"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalOutput))
                        return;
                }
                
                _digitalOutput = node.objectTarget as IWireOutput<bool>;
                if(_digitalOutput == null)
                    node.objectTarget = null;
                
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}
