using UnityEngine;
using System.Collections.Generic;

using UINT16 = System.UInt16;


namespace Ardunity
{
	public enum ToneFrequency
	{
		MUTE = 0,
		B0 = 31,
		C1  = 33,
		CS1 = 35,
		D1  = 37,
		DS1 = 39,
		E1  = 41,
		F1  = 44,
		FS1 = 46,
		G1  = 49,
		GS1 = 52,
		A1  = 55,
		AS1 = 58,
		B1  = 62,
		C2  = 65,
		CS2 = 69,
		D2  = 73,
		DS2 = 78,
		E2  = 82,
		F2  = 87,
		FS2 = 93,
		G2  = 98,
		GS2 = 104,
		A2  = 110,
		AS2 = 117,
		B2  = 123,
		C3  = 131,
		CS3 = 139,
		D3  = 147,
		DS3 = 156,
		E3  = 165,
		F3  = 175,
		FS3 = 185,
		G3  = 196,
		GS3 = 208,
		A3  = 220,
		AS3 = 233,
		B3  = 247,
		C4  = 262,
		CS4 = 277,
		D4  = 294,
		DS4 = 311,
		E4  = 330,
		F4  = 349,
		FS4 = 370,
		G4  = 392,
		GS4 = 415,
		A4  = 440,
		AS4 = 466,
		B4  = 494,
		C5  = 523,
		CS5 = 554,
		D5  = 587,
		DS5 = 622,
		E5  = 659,
		F5  = 698,
		FS5 = 740,
		G5  = 784,
		GS5 = 831,
		A5  = 880,
		AS5 = 932,
		B5  = 988,
		C6  = 1047,
		CS6 = 1109,
		D6  = 1175,
		DS6 = 1245,
		E6  = 1319,
		F6  = 1397,
		FS6 = 1480,
		G6  = 1568,
		GS6 = 1661,
		A6  = 1760,
		AS6 = 1865,
		B6  = 1976,
		C7  = 2093,
		CS7 = 2217,
		D7  = 2349,
		DS7 = 2489,
		E7  = 2637,
		F7  = 2794,
		FS7 = 2960,
		G7  = 3136,
		GS7 = 3322,
		A7  = 3520,
		AS7 = 3729,
		B7  = 3951,
		C8  = 4186,
		CS8 = 4435,
		D8  = 4699,
		DS8 = 4978
	}
	
	[AddComponentMenu("ARDUnity/Controller/Sound/GenericTone")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/generictone")]
	public class GenericTone : ArdunityController, IWireOutput<float>
	{
		public int pin;
		
        [SerializeField]
		private ToneFrequency _toneFrequency;
		private UINT16 _frequency;
		
		protected override void Awake()
		{
			base.Awake();
			
			enableUpdate = false; // only output.
		}
		
		protected override void OnPush()
		{
			Push(_frequency);
		}

		public override string GetCodeDeclaration()
		{
			return string.Format("{0} {1}({2:d}, {3:d});", this.GetType().Name, GetCodeVariable(), id, pin);
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("tone{0:d}", id);
		}
		
		public ToneFrequency toneFrequency
		{
			get
			{
				return _toneFrequency;
			}
			set
			{
				if(_toneFrequency != value)
				{
					_toneFrequency = value;
					_frequency = (UINT16)_toneFrequency;
					SetDirty();
				}
			}
		}
		
		public float frequency
		{
			get
			{
				return (float)_frequency;
			}
			set
			{
				int newValue = (int)Mathf.Round(value);
				newValue = Mathf.Abs(newValue);				
				if(_frequency != (UINT16)newValue)
				{
					_frequency = (UINT16)newValue;
					SetDirty();
				}
			}
		}
		
		float IWireOutput<float>.output
        {
			get
			{
				return frequency;
			}
            set
            {
				frequency = value;
            }
		}
        
        protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Digital Pin"));
			nodes.Add(new Node("frequency", "Frequency", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: {0:d}", pin);
                return;
            }
			else if(node.name.Equals("frequency"))
            {
				node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}
