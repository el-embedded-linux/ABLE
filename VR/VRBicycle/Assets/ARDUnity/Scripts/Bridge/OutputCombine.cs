using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	public enum CombineMode
	{
		Minimum,
		Maximum,
		Average,
		Sum
	}
	
	[AddComponentMenu("ARDUnity/Bridge/Output/OutputCombine")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/outputcombine")]
	public class OutputCombine : ArdunityBridge, IWireOutput<float>
	{
		public CombineMode combineMode;
		
		private IWireOutput<float> _combineResult;
		private List<float> _outputs = new List<float>();
		private float _output;
        private List<Keyframe> _keyFrames = new List<Keyframe>();

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		void LateUpdate()
		{
			if(!Application.isPlaying)
				return;
		    
			if(_outputs.Count > 0)
			{
				float output = 0f;
				
				if(combineMode == CombineMode.Minimum)
				{
					output = 10000f;
					for(int i=0; i<_outputs.Count; i++)
					{
						if(output > _outputs[i])
							output = _outputs[i];
					}
				}
				else if(combineMode == CombineMode.Maximum)
				{
					output = -10000f;
					for(int i=0; i<_outputs.Count; i++)
					{
						if(output < _outputs[i])
							output = _outputs[i];
					}
				}
				else if(combineMode == CombineMode.Average)
				{
					output = 0f;
					for(int i=0; i<_outputs.Count; i++)
						output += _outputs[i];
					output /= _outputs.Count;
				}
				else if(combineMode == CombineMode.Sum)
				{
					output = 0f;
					for(int i=0; i<_outputs.Count; i++)
						output += _outputs[i];
				}
				
				_outputs.Clear();
				_output = output;
				if(_combineResult != null)
					_combineResult.output = _output;
                
                _keyFrames.Add(new Keyframe(Time.time, _output));
			}
            else
            {
                _keyFrames.Add(new Keyframe(Time.time, 0f));
            }
            
            if(_keyFrames.Count > 100)
                _keyFrames.RemoveAt(0);
		}
        
        public float Value
        {
            get
            {
                return _output;
            }
        }
        
        public Keyframe[] historyValues
        {
            get
            {
                return _keyFrames.ToArray();
            }
        }
		
		float IWireOutput<float>.output
        {
			get
			{
				return _output;
			}
            set
            {				
                _outputs.Add(value);
            }
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("mode", "", null, NodeType.None, "Method for Combine"));
            nodes.Add(new Node("result", "Result", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("source", "Source", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("mode"))
            {
				node.updated = true;
                node.text = string.Format("Combine by {0}", combineMode.ToString());
                return;
            }
            else if(node.name.Equals("result"))
            {
				node.updated = true;
                if(node.objectTarget == null && _combineResult == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_combineResult))
                        return;
                }
                
                _combineResult = node.objectTarget as IWireOutput<float>;;
                if(_combineResult == null)
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("source"))
            {
				node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
