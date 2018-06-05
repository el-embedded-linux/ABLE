using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Output/ToneOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/toneoutput")]
	public class ToneOutput : ArdunityBridge, IWireOutput<bool>
	{
		public ToneFrequency toneFrequency;
		
		private IWireOutput<float> _toneOutput;
		private bool _preWireBoolValue = false;
	
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		bool IWireOutput<bool>.output
        {
			get
			{
				return _preWireBoolValue;
			}
            set
            {
				if(!_preWireBoolValue && value)
				{
					if(_toneOutput != null)
						_toneOutput.output = (float)toneFrequency;
				}
				else if(_preWireBoolValue && !value)
				{
					if(_toneOutput != null)
						_toneOutput.output = (float)ToneFrequency.MUTE;
				}
				
				_preWireBoolValue = value;
            }
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("toneFrequency", "", null, NodeType.None, "Tone Frequency"));
            nodes.Add(new Node("frequency", "Frequency", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("active", "Active", typeof(IWireOutput<bool>), NodeType.WireTo, "Output<bool>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("toneFrequency"))
            {
				node.updated = true;
                node.text = string.Format("{0}", toneFrequency.ToString());
                return;
            }
            else if(node.name.Equals("frequency"))
            {
				node.updated = true;
                if(node.objectTarget == null && _toneOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_toneOutput))
                        return;
                }
                
                _toneOutput = node.objectTarget as IWireOutput<float>;
                if(_toneOutput == null)
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("active"))
            {
				node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
