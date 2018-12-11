using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Bridge/Input/ToggleInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/toggleinput")]
	public class ToggleInput : ArdunityBridge, IWireInput<bool>
	{
		public CheckEdge checkEdge = CheckEdge.FallingEdge;
		
		public BoolEvent OnValueChanged;
		
		private bool _value = false;
		private IWireInput<bool> _digitalInput;


		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		}

		public bool Value
		{
			get
			{
				return _value;
			}
		}
		
		private void DoToggling(bool value)
		{
			if(!value && checkEdge == CheckEdge.RisingEdge)
				return;
			else if(value && checkEdge == CheckEdge.FallingEdge)
				return;
						
			if(_value)
				_value = false;
			else
				_value = true;
				
			if(OnWireInputChanged != null)
				OnWireInputChanged(_value);
			
			OnValueChanged.Invoke(_value);
		}
		
		public event WireEventHandler<bool> OnWireInputChanged;
		
		bool IWireInput<bool>.input
        {
			get
			{
				return _value;
			}
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("checkEdge", "", null, NodeType.None, "Check Edge"));
            nodes.Add(new Node("source", "Source", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("result", "Result", typeof(IWireInput<bool>), NodeType.WireTo, "Input<bool>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("checkEdge"))
            {
				node.updated = true;
                node.text = checkEdge.ToString();
                return;
            }
            else if(node.name.Equals("source"))
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
                    _digitalInput.OnWireInputChanged -= DoToggling;
                
                _digitalInput = node.objectTarget as IWireInput<bool>;
                if(_digitalInput != null)
                    _digitalInput.OnWireInputChanged += DoToggling;
                else
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("result"))
            {
				node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
