using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/UI/DropdownReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/dropdownreactor")]
	[RequireComponent(typeof(Dropdown))]
	public class DropdownReactor : ArdunityReactor
	{	
		private IWireOutput<int> _intOutput;
		private Dropdown _dropdown;
		
		protected override void Awake()
		{
            base.Awake();
            
			_dropdown = GetComponent<Dropdown>();
			_dropdown.onValueChanged.AddListener(OnSelectionChanged);
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		void OnEnable()
		{
			if(_intOutput != null)
				_intOutput.output = _dropdown.value;
		}
		
		private void OnSelectionChanged(int index)
		{
			if(_intOutput != null)
				_intOutput.output = index;
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("getSelection", "Get Selection", typeof(IWireOutput<int>), NodeType.WireFrom, "Output<int>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("getSelection"))
            {
				node.updated = true;
                if(node.objectTarget == null && _intOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_intOutput))
                        return;
                }
                
                _intOutput = node.objectTarget as IWireOutput<int>;
                if(_intOutput == null)
                    node.objectTarget = null;
                
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
