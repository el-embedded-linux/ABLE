using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Input/InputFilter")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/inputfilter")]
	public class InputFilter : ArdunityBridge, IWireInput<float>
	{
		public bool invert = false;
		public bool amplify = false;
		[Range(0f, 1f)]
		public float minValue = 0f;
		[Range(0f, 1f)]
		public float maxValue = 1f;
		public bool smooth = false;
		[Range(0f, 1f)]
		public float sensitivity = 0.5f;
		
        private AnalogInput _analogInput;
		private List<float> _sourceValues = new List<float>();
		private List<float> _resultValues = new List<float>();
		private float _minSourceValue;
		private float _maxSourceValue;
		private float _sensitivity;
		private int _sampleNum = 100;
		private float _sourceValue;
		private float _resultValue;
		private bool _connected;
		// Kalman filter's parameter
		private float _Q;
		private float _R;
		private float _P;
		private float _X;
		private float _K;		

	
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_analogInput != null)
			{
				if(_analogInput.connected)
				{
					if(!_connected && _analogInput.connected)
						Reset();
					
					_sourceValue = _analogInput.Value;
					_minSourceValue = Mathf.Min(_minSourceValue, _sourceValue);
					_maxSourceValue = Mathf.Max(_maxSourceValue, _sourceValue);
					
					float result = ProcessFilter(_sourceValue);
					if(result != _resultValue)
					{
						_resultValue = result;
						if(OnWireInputChanged != null)
							OnWireInputChanged(Value);
					}
					
					if(_sourceValues.Count >= _sampleNum)
					{
						_sourceValues.RemoveAt(0);
						_resultValues.RemoveAt(0);
					}
					_sourceValues.Add(_sourceValue);
					_resultValues.Add(_resultValue);
				}
				
				_connected = _analogInput.connected;
			}		
		}
		
		public float Value
		{
			get
			{
				return _resultValue;
			}
		}
		
		public float minSourceValue
		{
			get
			{
				return _minSourceValue;
			}
		}
		
		public float maxSourceValue
		{
			get
			{
				return _maxSourceValue;
			}
		}
		
		public float[] sourceValues
		{
			get
			{
				return _sourceValues.ToArray();
			}
		}
		
		public float[] resultValues
		{
			get
			{
				return _resultValues.ToArray();
			}
		}
		
		public void ResetFilter()
		{
			if(_analogInput == null)
				return;
			
			_sourceValues.Clear();
			_resultValues.Clear();
			_minSourceValue = 1f;
			_maxSourceValue = 0f;
			_sourceValue = _analogInput.Value;
			SmoothReset();
			_resultValue = ProcessFilter(_sourceValue);
			_sourceValues.Add(_sourceValue);
			_resultValues.Add(_resultValue);
		}
		
		private float ProcessFilter(float source)
		{
			float result = source;
			
			if(amplify)
			{
				result = Mathf.Clamp(result, minValue, maxValue);
				result = (result - minValue) / (maxValue - minValue);
			}
			
			if(smooth)
			{
				if(_sensitivity != sensitivity)
					SmoothReset();
				
				_K = (_P + _Q) / (_P + _Q + _R);
				_P = _R * (_P + _Q) / (_R + _P + _Q);
				result = _X + (result - _X) * _K;
				_X = result;
			}
			
			if(invert)
				result = 1f - result;
			
			return result;
		}
		
		private void SmoothReset()
		{
			_sensitivity = sensitivity;
			_Q = 0.00001f + (0.001f * _sensitivity);
			_R = 0.01f;
			_P = 1f;
			_X = 0f;
			_K = 0f;
		}
		
		public event WireEventHandler<float> OnWireInputChanged;

        float IWireInput<float>.input
        {
			get
			{
				return Value;
			}
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("analogInput", "AnalogInput", typeof(AnalogInput), NodeType.WireFrom, "AnalogInput"));
			nodes.Add(new Node("result", "Result", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("analogInput"))
            {
				node.updated = true;

                if(node.objectTarget == null && _analogInput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogInput))
                        return;
                }
                
                _analogInput = node.objectTarget as AnalogInput;
                if(_analogInput != null)
                {
                    Reset();
                    _connected = _analogInput.connected;
                }
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