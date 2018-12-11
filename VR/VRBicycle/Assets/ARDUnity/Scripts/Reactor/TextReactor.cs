using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Reactor/UI/TextReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/textreactor")]
	[RequireComponent(typeof(Text))]
	public class TextReactor : ArdunityReactor
	{
		public string format = "f2";
		
		private Text _text;
		private IWireInput<float> _analogInput;
		
		protected override void Awake()
		{
            base.Awake();
            
			_text = GetComponent<Text>();
		}

		// Use this for initialization
		void Start ()
		{			
		}
		
		void OnEnable()
		{		
			DoReacting();
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
		
		private void DoReacting()
		{
			if(_analogInput == null)
				return;
			
            if(format.Length > 0)
                _text.text = _analogInput.input.ToString(format);
            else
                _text.text = _analogInput.input.ToString();
		}
		
		private void OnWireInputChanged(float value)
		{
			if(!this.enabled)
				return;
				
			DoReacting();
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("setText", "Set Text", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("setText"))
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
                           
            base.UpdateNode(node);
        }
	}
}
