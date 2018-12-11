using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Input/ColorInput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/colorinput")]
	public class ColorInput : ArdunityBridge, IWireInput<Color>
	{		
		[SerializeField]
		private Color _color;
        
		private IWireInput<float> _analogRed;
		private IWireInput<float> _analogBlue;
		private IWireInput<float> _analogGreen;
        private IWireInput<bool> _digitalRed;
		private IWireInput<bool> _digitalBlue;
		private IWireInput<bool> _digitalGreen;
        
        #region MonoBehavior
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
        #endregion
		
		private void AnalogRedChanged(float value)
		{
			_color.r = Mathf.Clamp(value, 0f, 1f);
			if(OnWireInputChanged != null)
				OnWireInputChanged(_color);
		}
		
		private void AnalogBlueChanged(float value)
		{
			_color.b = Mathf.Clamp(value, 0f, 1f);
			if(OnWireInputChanged != null)
				OnWireInputChanged(_color);
		}
		
		private void AnalogGreenChanged(float value)
		{
			_color.g = Mathf.Clamp(value, 0f, 1f);
			if(OnWireInputChanged != null)
				OnWireInputChanged(_color);
		}
        
        private void DigitalRedChanged(bool value)
		{
			if(value)
				_color.r = 1f;
			else
				_color.r = 0f;
			if(OnWireInputChanged != null)
				OnWireInputChanged(_color);
		}
		
		private void DigitalBlueChanged(bool value)
		{
			if(value)
				_color.b = 1f;
			else
				_color.b = 0f;
			if(OnWireInputChanged != null)
				OnWireInputChanged(_color);
		}
		
		private void DigitalGreenChanged(bool value)
		{
			if(value)
				_color.g = 1f;
			else
				_color.g = 0f;
			if(OnWireInputChanged != null)
				OnWireInputChanged(_color);
		}
		
		public Color color
		{
			get
			{
				return _color;
			}
		}
		
        #region Wire Editor
		public event WireEventHandler<Color> OnWireInputChanged;

        Color IWireInput<Color>.input
        {
			get
			{
				return color;
			}
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("analogRed", "Red(analog)", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("analogGreen", "Green(analog)", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
			nodes.Add(new Node("analogBlue", "Blue(analog)", typeof(IWireInput<float>), NodeType.WireFrom, "Input<float>"));
            nodes.Add(new Node("digitalRed", "Red(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("digitalGreen", "Green(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("digitalBlue", "Blue(digital)", typeof(IWireInput<bool>), NodeType.WireFrom, "Input<bool>"));
			nodes.Add(new Node("inputColor", "Color", typeof(IWireInput<Color>), NodeType.WireTo, "Input<Color>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("analogRed"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogRed == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogRed))
                        return;
                }
                
                if(_analogRed != null)
                    _analogRed.OnWireInputChanged -= AnalogRedChanged;
                
                _analogRed = node.objectTarget as IWireInput<float>;
                if(_analogRed != null)
                    _analogRed.OnWireInputChanged += AnalogRedChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("analogGreen"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogGreen == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogGreen))
                        return;
                }
                
                if(_analogGreen != null)
                    _analogGreen.OnWireInputChanged -= AnalogGreenChanged;
                
                _analogGreen = node.objectTarget as IWireInput<float>;
                if(_analogGreen != null)
                    _analogGreen.OnWireInputChanged += AnalogGreenChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("analogBlue"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogBlue == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogBlue))
                        return;
                }
                
                if(_analogBlue != null)
                    _analogBlue.OnWireInputChanged -= AnalogBlueChanged;
                
                _analogBlue = node.objectTarget as IWireInput<float>;
                if(_analogBlue != null)
                    _analogBlue.OnWireInputChanged += AnalogBlueChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalRed"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalRed == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalRed))
                        return;
                }
                
                if(_digitalRed != null)
                    _digitalRed.OnWireInputChanged -= DigitalRedChanged;
                
                _digitalRed = node.objectTarget as IWireInput<bool>;
                if(_digitalRed != null)
                    _digitalRed.OnWireInputChanged += DigitalRedChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalGreen"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalGreen == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalGreen))
                        return;
                }
                
                if(_digitalGreen != null)
                    _digitalGreen.OnWireInputChanged -= DigitalGreenChanged;
                
                _digitalGreen = node.objectTarget as IWireInput<bool>;
                if(_digitalGreen != null)
                    _digitalGreen.OnWireInputChanged += DigitalGreenChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalBlue"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalBlue == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalBlue))
                        return;
                }
                
                if(_digitalBlue != null)
                    _digitalBlue.OnWireInputChanged -= DigitalBlueChanged;
                
                _digitalBlue = node.objectTarget as IWireInput<bool>;
                if(_digitalBlue != null)
                    _digitalBlue.OnWireInputChanged += DigitalBlueChanged;
                else
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("inputColor"))
            {
                node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}

