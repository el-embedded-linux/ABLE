using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace Ardunity
{
    public class DragData
    {
        public bool isDrag;
        public float delta;
        public float force;
        
        public DragData()
        {
            isDrag = false;
            delta = 0f;
            force = 0f;
        }
        
        public DragData(DragData source)
        {
            isDrag = source.isDrag;
            delta = source.delta;
            force = source.force;
        }
    }
    
    [AddComponentMenu("ARDUnity/Bridge/Input/DragInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/draginput")]
	public class DragInput : ArdunityBridge, IWireInput<DragData>, IWireInput<float>
	{
        [Range(0f, 1f)]
        public float minValue = 0.1f;
        [Range(0f, 1f)]
        public float maxValue = 0.9f;
        public bool invert = false;
        public float deltaMultiplier = 1f;
        public float forceMultiplier = 1f;
        
        public UnityEvent OnDragStart;
        public UnityEvent OnDragEnd;
        
        private AnalogInput _analogInput;
        private DragData _dragData = new DragData();
        private float _startValue;
        private float _value;
        private float _time;

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
                    float value = _analogInput.Value;
                    _time += Time.deltaTime;
                    if(value > minValue && value < maxValue)
                    {
                        if(!_dragData.isDrag)
                        {
                            _dragData.isDrag = true;
                            _dragData.delta = 0f;
                            _dragData.force = 0f;
                            _startValue = value;
                            _value = value;
                            _time = 0f;
                            if(_OnDragDataChanged != null)
                                _OnDragDataChanged(new DragData(_dragData));
                            
                            OnDragStart.Invoke();
                        }
                        else
                        {
                            if(_value != value)
                            {
                                _dragData.delta = (_value - value) * deltaMultiplier;
                                if(invert)
                                    _dragData.delta = -_dragData.delta;
                                
                                _value = value;
                                
                                if(_OnDragDataChanged != null)
                                    _OnDragDataChanged(new DragData(_dragData));
                                
                                if(_OnValueChanged != null)
                                    _OnValueChanged(_value);
                            }
                        }
                    }
                    else
                    {
                        if(_dragData.isDrag)
                        {
                            _dragData.isDrag = false;
                            _dragData.delta = 0f;
                            _dragData.force = ((_startValue - _value) / _time) * forceMultiplier;
                            if(_OnDragDataChanged != null)
                                _OnDragDataChanged(new DragData(_dragData));
                            
                            OnDragEnd.Invoke();
                        }
                    }
                }
                else
                    _value = _analogInput.Value;
            }
		}
        
        public DragData dragData
        {
            get
            {
                return new DragData(_dragData);
            }
        }
        
        public float Value
        {
            get
            {
                return _value;
            }
        }
		
        #region Wire Editor
		event WireEventHandler<DragData> _OnDragDataChanged;
        
        event WireEventHandler<DragData> IWireInput<DragData>.OnWireInputChanged
        {
            add
            {
                _OnDragDataChanged += value;
            }

            remove
            {
                _OnDragDataChanged -= value;
            }
        }
		
        DragData IWireInput<DragData>.input
        {
			get
			{
				return dragData;
			}
        }
        
        event WireEventHandler<float> _OnValueChanged;
        
        event WireEventHandler<float> IWireInput<float>.OnWireInputChanged
        {
            add
            {
                _OnValueChanged += value;
            }

            remove
            {
                _OnValueChanged -= value;
            }
        }

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
			nodes.Add(new Node("dragData", "DragData", typeof(IWireInput<DragData>), NodeType.WireTo, "Input<DragData>"));
            nodes.Add(new Node("Value", "Value", typeof(IWireInput<float>), NodeType.WireTo, "Input<float>"));
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
                if(_analogInput == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("dragData"))
            {
                node.updated = true;
                return;
            }
            else if(node.name.Equals("Value"))
            {
                node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
        #endregion
    }
}
