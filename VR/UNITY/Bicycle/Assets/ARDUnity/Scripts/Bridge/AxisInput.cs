using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Input/AxisInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/axisinput")]
	public class AxisInput : ArdunityBridge, IWireInput<Vector2>
	{
        [Range(0f, 0.5f)]
        public float minCenterHorizontal = 0.45f;
        [Range(0.5f, 1f)]
        public float maxCenterHorizontal = 0.55f;
        [Range(0f, 0.5f)]
        public float minCenterVertical = 0.45f;
        [Range(0.5f, 1f)]
        public float maxCenterVertical = 0.55f;
        public bool invertHorizontal = false;
        public bool invertVertical = false;
        
        private bool _upValue = false;
        private bool _downValue = false;
        private bool _rightValue = false;
        private bool _leftValue = false;
        private Vector2 _inputAxis = Vector2.zero;
        private Vector2 _outputAxis = Vector2.zero;
        
		private IWireInput<float> _analogHorizontal;
		private IWireInput<float> _analogVertical;
		private IWireInput<bool> _digitalUp;
		private IWireInput<bool> _digitalDown;
        private IWireInput<bool> _digitalRight;
        private IWireInput<bool> _digitalLeft;
        
        #region MonoBehavior
		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update()
		{
            if(_inputAxis != _outputAxis)
            {
                _outputAxis = _inputAxis;
                if(OnWireInputChanged != null)
				    OnWireInputChanged(_outputAxis);
            }
		}
        #endregion

        private void AnalogHorizontalChanged(float value)
		{
            if(value > minCenterHorizontal && value < maxCenterHorizontal)
                _inputAxis.x = 0f;
            else
                _inputAxis.x = (value - 0.5f) * 2f;
            
            if(invertHorizontal)
                _inputAxis.x = -_inputAxis.x;
        }

        private void AnalogVerticalChanged(float value)
		{
            if(value > minCenterVertical && value < maxCenterVertical)
                _inputAxis.y = 0f;
            else
                _inputAxis.y = (value - 0.5f) * 2f;

            if(invertVertical)
                _inputAxis.y = -_inputAxis.y;
        }

        private void DigitalUpChanged(bool value)
		{
            _upValue = value;
            UpdateDigitalAxis();
        }

        private void DigitalDownChanged(bool value)
		{
            _downValue = value;
            UpdateDigitalAxis();
        }

        private void DigitalRightChanged(bool value)
		{
            _rightValue = value;
            UpdateDigitalAxis();
        }

        private void DigitalLeftChanged(bool value)
		{
            _leftValue = value;
            UpdateDigitalAxis();
        }

        private void UpdateDigitalAxis()
        {
            if(_upValue)
                _inputAxis.y = 1f;
            else if(_downValue)
                _inputAxis.y = -1f;
            else
                _inputAxis.y = 0f;
            
            if(_rightValue)
                _inputAxis.x = 1f;
            else if(_leftValue)
                _inputAxis.x = -1f;
            else
                _inputAxis.x = 0f;
            
            if(invertVertical)
                _inputAxis.y = -_inputAxis.y;
            
            if(invertHorizontal)
                _inputAxis.x = -_inputAxis.x;
        }
		
		public Vector2 axis
		{
			get
			{
                return _outputAxis;
			}
		}
		
        #region Wire Editor
		public event WireEventHandler<Vector2> OnWireInputChanged;

        Vector2 IWireInput<Vector2>.input
        {
			get
			{
				return axis;
			}
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("analogHorizontal", "Horizontal(analog)", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("analogVertical", "Vertical(analog)", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
            nodes.Add(new Node("digitalUp", "Up(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("digitalDown", "Down(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("digitalRight", "Right(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
            nodes.Add(new Node("digitalLeft", "Left(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
            nodes.Add(new Node("axis", "Axis Vector", typeof(IWireInput<Vector2>), NodeType.WireTo, "Input<Vector2>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("analogHorizontal"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogHorizontal == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogHorizontal))
                        return;
                }

                if(_analogHorizontal != null)
                    _analogHorizontal.OnWireInputChanged -= AnalogHorizontalChanged;
                
                _analogHorizontal = node.objectTarget as IWireInput<float>;
                if(_analogHorizontal != null)
                    _analogHorizontal.OnWireInputChanged += AnalogHorizontalChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("analogVertical"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogVertical == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogVertical))
                        return;
                }

                if(_analogVertical != null)
                    _analogVertical.OnWireInputChanged -= AnalogVerticalChanged;
                
                _analogVertical = node.objectTarget as IWireInput<float>;
                if(_analogVertical != null)
                    _analogVertical.OnWireInputChanged += AnalogVerticalChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalUp"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalUp == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalUp))
                        return;
                }

                if(_digitalUp != null)
                    _digitalUp.OnWireInputChanged -= DigitalUpChanged;
                
                _digitalUp = node.objectTarget as IWireInput<bool>;
                if(_digitalUp != null)
                    _digitalUp.OnWireInputChanged += DigitalUpChanged;
                else
                    node.objectTarget = null;

                return;
            }
            else if(node.name.Equals("digitalDown"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalDown == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalDown))
                        return;
                }

                if(_digitalDown != null)
                    _digitalDown.OnWireInputChanged -= DigitalDownChanged;
                
                _digitalDown = node.objectTarget as IWireInput<bool>;
                if(_digitalDown != null)
                    _digitalDown.OnWireInputChanged += DigitalDownChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalRight"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalRight == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalRight))
                        return;
                }

                if(_digitalRight != null)
                    _digitalRight.OnWireInputChanged -= DigitalRightChanged;
                
                _digitalRight = node.objectTarget as IWireInput<bool>;
                if(_digitalRight != null)
                    _digitalRight.OnWireInputChanged += DigitalRightChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalLeft"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalLeft == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalLeft))
                        return;
                }

                if(_digitalLeft != null)
                    _digitalLeft.OnWireInputChanged -= DigitalLeftChanged;
                
                _digitalLeft = node.objectTarget as IWireInput<bool>;
                if(_digitalLeft != null)
                    _digitalLeft.OnWireInputChanged += DigitalLeftChanged;
                else
                    node.objectTarget = null;

                return;
            }
            else if(node.name.Equals("axis"))
            {
                node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}

