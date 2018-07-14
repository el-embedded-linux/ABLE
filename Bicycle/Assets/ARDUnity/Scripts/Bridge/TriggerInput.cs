using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Bridge/Input/TriggerInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/triggerinput")]
	public class TriggerInput : ArdunityBridge, IWireInput<Trigger>
	{
		public CheckEdge checkEdge = CheckEdge.FallingEdge;
		
		public UnityEvent OnTrigger;
		
		private IWireInput<bool> _digitalInput;
		private Trigger _value = new Trigger();
		private bool _preWireValue = false;
		
		protected override void Awake()
		{
            base.Awake();

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
		
		private void DoTrigger(bool value)
		{
			if(!_preWireValue  && value && checkEdge == CheckEdge.RisingEdge)
			{
				_value.Reset();

				if(OnWireInputChanged != null)
					OnWireInputChanged(_value);
				
				OnTrigger.Invoke();
			}
			else if(_preWireValue && !value && checkEdge == CheckEdge.FallingEdge)
			{
				_value.Reset();

				if(OnWireInputChanged != null)
					OnWireInputChanged(_value);
				
				OnTrigger.Invoke();
			}
			
			_preWireValue = value;			
		}
		
		public event WireEventHandler<Trigger> OnWireInputChanged;
		
		Trigger IWireInput<Trigger>.input
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
            nodes.Add(new Node("sourceValue", "Source", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("resultValue", "Result", typeof(IWireInput<Trigger>), NodeType.WireTo, "Input<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("checkEdge"))
            {
				node.updated = true;
                node.text = checkEdge.ToString();
                return;
            }
            else if(node.name.Equals("sourceValue"))
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
                    _digitalInput.OnWireInputChanged -= DoTrigger;
                
                _digitalInput = node.objectTarget as IWireInput<bool>;
                if(_digitalInput != null)
                    _digitalInput.OnWireInputChanged += DoTrigger;
                else
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("resultValue"))
            {
				node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
