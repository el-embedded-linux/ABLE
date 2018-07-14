using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/UI/ButtonReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/buttonreactor")]
	[RequireComponent(typeof(Button))]
	public class ButtonReactor : ArdunityReactor, IPointerDownHandler, IPointerUpHandler
	{
        public CheckEdge checkEdge = CheckEdge.FallingEdge;
		
		private IWireOutput<bool> _digitalOutput;
        private IWireOutput<Trigger> _triggerOutput;

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
			if(!this.enabled)
				return;
            
            if(checkEdge == CheckEdge.FallingEdge && _triggerOutput != null)
                _triggerOutput.output = new Trigger();
			
            if(_digitalOutput != null)
				_digitalOutput.output = true;
        }
		
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
			if(!this.enabled)
				return;
            
            if(checkEdge == CheckEdge.RisingEdge && _triggerOutput != null)
                _triggerOutput.output = new Trigger();
				
            if(_digitalOutput != null)
				_digitalOutput.output = false;
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);

            nodes.Add(new Node("checkEdge", "", null, NodeType.None, "Check Edge"));
            nodes.Add(new Node("getPressed", "Get Pressed", typeof(IWireOutput<bool>), NodeType.WireFrom, "Output<bool>"));
            nodes.Add(new Node("getClicked", "Get Clicked", typeof(IWireOutput<Trigger>), NodeType.WireFrom, "Output<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("checkEdge"))
            {
                node.updated = true;
                node.text = checkEdge.ToString();
                return;
            }
            else if(node.name.Equals("getPressed"))
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
            else if(node.name.Equals("getClicked"))
            {
                node.updated = true;
                if(node.objectTarget == null && _triggerOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_triggerOutput))
                        return;
                }
                
                _triggerOutput = node.objectTarget as IWireOutput<Trigger>;
                if(_triggerOutput == null)
                    node.objectTarget = null;
                
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
