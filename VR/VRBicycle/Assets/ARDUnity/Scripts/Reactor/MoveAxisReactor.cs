using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
    [AddComponentMenu("ARDUnity/Reactor/Transform/MoveAxisReactor")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/reactor/moveaxisreactor")]
	public class MoveAxisReactor : ArdunityReactor
	{
		public Axis moveAxis;
		public bool invert = false;
		public float scaler = 1f;
		
		private Vector3 _initPos;
		private Vector3 _dragPos;
		private IWireInput<float> _analogInput;
		private IWireOutput<float> _analogOutput;
		private IWireInput<DragData> _dragInput;
		
		protected override void Awake()
		{
            base.Awake();
			
			_initPos = transform.localPosition;
			_dragPos = Vector3.zero;
		}
	
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_analogOutput != null)
			{
				Vector3 offset = transform.localPosition - (_initPos + _dragPos);
				if(invert)
					offset *= -1f;
				
				if(moveAxis == Axis.X)
					_analogOutput.output = offset.x;
				else if(moveAxis == Axis.Y)
					_analogOutput.output = offset.y;
				else if(moveAxis == Axis.Z)
					_analogOutput.output = offset.z;
			}
		}
		
		private void OnAnalogInputChanged(float value)
		{
			if(invert)
				value = -value;
			
			value *= scaler;			
			
			Vector3 pos = transform.localPosition;
			if(moveAxis == Axis.X)
				pos.x = _initPos.x + _dragPos.x + value;
			else if(moveAxis == Axis.Y)
				pos.y = _initPos.y + _dragPos.y + value;
			else if(moveAxis == Axis.Z)
				pos.z = _initPos.z + _dragPos.z + value;
			
			transform.localPosition = pos;
		}
		
		private void OnDragInputChanged(DragData value)
		{
			if(value.isDrag)
			{
				if(invert)
					value.delta = -value.delta;
				
				Vector3 pos = transform.localPosition;
				if(moveAxis == Axis.X)
				{
					pos.x += value.delta;
					_dragPos.x += value.delta;
				}
				else if(moveAxis == Axis.Y)
				{
					pos.y += value.delta;
					_dragPos.y += value.delta;
				}
				else if(moveAxis == Axis.Z)
				{
					pos.z += value.delta;
					_dragPos.z += value.delta;
				}
				
				transform.localPosition = pos;
			}
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("setPosition", "Set Position", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("getPosition", "Get Position", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("moveDrag", "Move by drag", typeof(IWireInput<DragData>), NodeType.WireFrom, "Input<DragData>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("setPosition"))
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
                    _analogInput.OnWireInputChanged -= OnAnalogInputChanged;
                
                _analogInput = node.objectTarget as IWireInput<float>;
                if(_analogInput != null)
                    _analogInput.OnWireInputChanged += OnAnalogInputChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("getPosition"))
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
            else if(node.name.Equals("moveDrag"))
            {
				node.updated = true;
                if(node.objectTarget == null && _dragInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_dragInput))
                        return;
                }
                
                if(_dragInput != null)
                    _dragInput.OnWireInputChanged -= OnDragInputChanged;
                
                _dragInput = node.objectTarget as IWireInput<DragData>;
                if(_dragInput != null)
                    _dragInput.OnWireInputChanged += OnDragInputChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
                            
            base.UpdateNode(node);
        }
	}
}
