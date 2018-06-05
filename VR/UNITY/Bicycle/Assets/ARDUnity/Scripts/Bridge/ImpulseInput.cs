using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace Ardunity
{
    [AddComponentMenu("ARDUnity/Bridge/Input/ImpulseInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/impulseinput")]
	public class ImpulseInput : ArdunityBridge, IWireInput<float>, IWireInput<Trigger>
	{
        public enum ImpulseMode
        {
            Height,
            Time,
            All
        }

        [Range(0f, 1f)]
        public float threshold = 0.1f;
        public float delayTime = 0.1f;
        public float clearTime = 1f;
        public ImpulseMode impulseMode = ImpulseMode.All;
        public bool invert = false;
        
		public UnityEvent OnTriggerShot;
        public FloatEvent OnImpulseShot;
        
        private IWireInput<float> _input;
		private Trigger _trigger = new Trigger();
        private bool _impulseStart = false;
		private float _impulseValue = 0f;
        private float _maxValue;
		private float _preValue;
		private float _time = 0f;


        // Use this for initialization
        void Start ()
        {
		}
		
		// Update is called once per frame
		void Update ()
        {
            _time += Time.deltaTime;

			if(_impulseStart)
			{
				if(_time > clearTime)
				{
					_impulseStart = false;
					_impulseValue = 0f;
				}
			}
		}

		private void InputChanged(float value)
		{
			value = Mathf.Clamp(value, 0f, 1f);

			if(invert)
				value = 1f - value;
			float th = threshold;
			if(invert)
				th = 1f - th;
			
			if(_impulseStart)
			{
				_maxValue = Mathf.Max(value, _maxValue);
				if(value < th)
				{
					_impulseStart = false;
                    if(impulseMode == ImpulseMode.Height)
                        _impulseValue = _maxValue;
                    else
                    {
                        float t = Mathf.Max(_time - 0.1f, 0f) * 10f;
                        _impulseValue = Mathf.Clamp(Mathf.Exp(-t), 0f, 1f);
                        if(impulseMode == ImpulseMode.All)
                            _impulseValue *= _maxValue;
                    }
                    _time = 0f;

                    if(_OnImpulseValueChanged != null)
                        _OnImpulseValueChanged(_impulseValue);
                    
                    OnImpulseShot.Invoke(_impulseValue);
                    
                    _trigger.Reset();
                    if(_OnTriggerChanged != null)
                        _OnTriggerChanged(_trigger);
                    
                    OnTriggerShot.Invoke();
				}					
			}
			else
			{
				if(value > th && _preValue <= th && _time > delayTime)
				{
					_time = 0f;
					_impulseStart = true;
					_maxValue = value;
				}
			}

			_preValue = value;
		}

		public float Value
		{
			get
			{
				return _impulseValue;
			}
		}
		
        #region Wire Editor
		event WireEventHandler<float> _OnImpulseValueChanged;
        
        event WireEventHandler<float> IWireInput<float>.OnWireInputChanged
        {
            add
            {
                _OnImpulseValueChanged += value;
            }

            remove
            {
                _OnImpulseValueChanged -= value;
            }
        }
		
        float IWireInput<float>.input
        {
			get
			{
				return Value;
			}
        }

		public event WireEventHandler<Trigger> _OnTriggerChanged;

		event WireEventHandler<Trigger> IWireInput<Trigger>.OnWireInputChanged
        {
            add
            {
                _OnTriggerChanged += value;
            }

            remove
            {
                _OnTriggerChanged -= value;
            }
        }
		
		Trigger IWireInput<Trigger>.input
        {
			get
			{
				return _trigger;
			}
        }

        protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("input", "Input", typeof(IWireInput<float>), NodeType.WireFrom, "IWireInput<float>"));
			nodes.Add(new Node("impulse", "Impulse", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
            nodes.Add(new Node("trigger", "Trigger", typeof(IWireInput<Trigger>), NodeType.WireTo, "Input<Trigger>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("input"))
            {
                node.updated = true;

                if(node.objectTarget == null && _input == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_input))
                        return;
                }

				if(_input != null)
                    _input.OnWireInputChanged -= InputChanged;
                
                _input = node.objectTarget as IWireInput<float>;
                if(_input != null)
                    _input.OnWireInputChanged += InputChanged;
                else
                    node.objectTarget = null;

                return;
            }
            else if(node.name.Equals("impulse"))
            {
                node.updated = true;
                return;
            }
            else if(node.name.Equals("trigger"))
            {
                node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
        #endregion
    }
}
