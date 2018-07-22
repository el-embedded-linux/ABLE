using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Reactor/Transform/Rotation2DReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/rotation2dreactor")]
	public class Rotation2DReactor : ArdunityReactor
	{
        public bool invert = false;
        
        private IWireInput<float> _analogInput;
        private IWireOutput<float> _analogOutput;
        
		// Use this for initialization
		void Start ()
		{
					
		}
		
		// Update is called once per frame
		void Update ()
		{
            if(_analogInput != null || _analogOutput != null)
			{
				if(_analogOutput != null)
                {
                    if(invert)
                        _analogOutput.output = -transform.eulerAngles.z;
                    else
                        _analogOutput.output = transform.eulerAngles.z;
                }					
			
				if(_analogInput != null)
				{
					Vector3 rot = transform.eulerAngles;
                    if(invert)
                        rot.z = -_analogInput.input;
                    else
                        rot.z = _analogInput.input;
                    transform.eulerAngles = rot;
				}
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("setAngle", "Set Angle", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("getAngle", "Get Angle", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("setAngle"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogInput))
                        return;
                }
                
                _analogInput = node.objectTarget as IWireInput<float>;
                if(_analogInput == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("getAngle"))
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
