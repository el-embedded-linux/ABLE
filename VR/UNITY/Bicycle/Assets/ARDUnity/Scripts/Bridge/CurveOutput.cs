using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Bridge/Output/CurveOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/curveoutput")]
	public class CurveOutput : ArdunityBridge, IWireOutput<bool>, IWireOutput<Trigger>, IWireOutput<float>
	{
		private enum STATE
		{
			IDLE,
			START,
			LOOP,
			END,
			FINAL
		}
		
		public AnimationCurve startCurve;
		public AnimationCurve loopCurve;
		public AnimationCurve endCurve;
        public string outputName = "Curve Output";
		public float multiplier = 1f;
		public float speed = 1f;
		public bool loop = true;
        
        public UnityEvent OnStart;
        public UnityEvent OnStop;

		private IWireOutput<float> _curveOutput;
		private bool _stop = false;
		private STATE _state = STATE.IDLE;
		private float _endTime;
		private float _time;
		private float _bias;
		private float _multiplier;
		private float _curveValue = 0f;
		private bool _preWireBoolValue = false;
        private Trigger _preWireTriggerValue;
		private float _preWireFloatValue = 0f;
        private List<Keyframe> _keyFrames = new List<Keyframe>();

        #region MonoBehaviour
        protected override void Awake()
        {
            base.Awake();
            
            _preWireTriggerValue = new Trigger();
            _preWireTriggerValue.Clear();
        }
        
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_state != STATE.IDLE)
			{
				if(_state == STATE.START)
				{
					_curveValue = startCurve.Evaluate(_time * speed) * _multiplier + _bias;
					
					if(_stop)
					{
						_state = STATE.END;
						_bias = 0f;
						_multiplier = _curveValue;
						_endTime = endCurve.keys[endCurve.length - 1].time / speed;
						_time = 0f;
					}
					else
					{
						_time += Time.deltaTime;
						if(_time > _endTime)
						{
							_state = STATE.LOOP;
							_time -= _endTime;
							_bias = 0f;
							_multiplier = multiplier;
							_endTime = loopCurve.keys[loopCurve.length - 1].time / speed;
						}
					}
				}			
				else if(_state == STATE.LOOP)
				{
					_curveValue = loopCurve.Evaluate(_time * speed) * _multiplier;
					
					if(_stop)
					{
						_state = STATE.END;
						_multiplier = _curveValue;
						_endTime = endCurve.keys[endCurve.length - 1].time / speed;
						_time = 0f;
					}
					else
					{
						_time += Time.deltaTime;
						if(_time > _endTime)
						{
							_time -= _endTime;
							_multiplier = multiplier;
							
							if(!loop)
							{
								_state = STATE.END;
								_endTime = endCurve.keys[endCurve.length - 1].time / speed;
							}
						}
					}
				}
				else if(_state == STATE.END)
				{
					_curveValue = endCurve.Evaluate(_time * speed) * _multiplier;
					
					_time += Time.deltaTime;
					if(_time > _endTime)
					{
						_state = STATE.FINAL;
					}
				}
				else if(_state == STATE.FINAL)
				{
					_curveValue = endCurve.keys[endCurve.length - 1].value * multiplier;
					_state = STATE.IDLE;
                    OnStop.Invoke();
				}
				
				if(_curveOutput != null)
					_curveOutput.output = _curveValue;
                
                _keyFrames.Add(new Keyframe(Time.time, _curveValue));
			}
            else
            {
                _keyFrames.Add(new Keyframe(Time.time, 0f));
            }
            
            if(_keyFrames.Count > 30)
                _keyFrames.RemoveAt(0);
		}
        #endregion

		public bool isPlaying
		{
			get
			{
				if(_state == STATE.IDLE)
					return false;
				else
					return true;
			}
		}

		public void Play()
		{
			if(speed == 0f)
			{
				Debug.Log("Speed for CurveOutput must be non-zero!");
				return;
			}
		
			if(startCurve.length < 2)
				return;			
			if(loopCurve.length < 2)
				return;
			if(endCurve.length < 2)
				return;
            
            if(_state == STATE.IDLE)
                OnStart.Invoke();
			
			_stop = false;
			_state = STATE.START;
			_bias = _curveValue;
			_multiplier = multiplier - _bias;
			_endTime = startCurve.keys[startCurve.length - 1].time / speed;
			_time = 0f;
		}
		
		public void Stop()
		{
			_stop = true;
		}
        
        public float Value
        {
            get
            {
                if(_state == STATE.IDLE)
					return 0f;
				else
					return _curveValue;
            }
        }
        
        public Keyframe[] historyValues
        {
            get
            {
                return _keyFrames.ToArray();
            }
        }

        #region Wire Editor
		bool IWireOutput<bool>.output
        {
			get
			{
				return _preWireBoolValue;
			}
            set
            {
				if(!_preWireBoolValue && value)
					Play();
				else if(_preWireBoolValue && !value && loop)
					Stop();
				
				_preWireBoolValue = value;
            }
        }
        
        Trigger IWireOutput<Trigger>.output
        {
			get
			{
				return _preWireTriggerValue;
			}
            set
            {
				if(value.value)
					Play();
				
				_preWireTriggerValue = value;
            }
        }
		
		float IWireOutput<float>.output
        {
			get
			{
				return _preWireFloatValue;
			}
            set
            {
				if(_preWireFloatValue != value)
				{
					_preWireFloatValue = value;
					
					if(_preWireFloatValue == 0f && loop)
						Stop();
					else
					{
						multiplier = _preWireFloatValue;
						Play();
					}						
				}				
            }
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
			nodes.Add(new Node("curveOutput", "", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("play", "Play", typeof(IWireOutput<bool>), NodeType.WireTo, "Output<bool>"));
            nodes.Add(new Node("playOnly", "Play Only", typeof(IWireOutput<Trigger>), NodeType.WireTo, "Output<Trigger>"));
			nodes.Add(new Node("multiplier", "Play by multiplier", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("curveOutput"))
            {
				node.updated = true;
                node.text = string.Format("{0}", outputName);
                
                if(node.objectTarget == null && _curveOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_curveOutput))
                        return;
                }
                
                _curveOutput = node.objectTarget as IWireOutput<float>;
                if(_curveOutput == null)
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("play"))
            {
				node.updated = true;
				return;
			}
			else if(node.name.Equals("playOnly"))
            {
				node.updated = true;
				return;
			}
			else if(node.name.Equals("multiplier"))
            {
				node.updated = true;
				return;
			}
            
            base.UpdateNode(node);
        }
        #endregion
	}
}
