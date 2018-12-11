using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Output/RTTTLSong")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/rtttlsong")]
	public class RTTTLSong : ArdunityBridge, IWireOutput<Trigger>, IWireOutput<int>, IWireOutput<string>
	{
		private class Note
		{
			public string name;
			public float frequency;
			public float time;
		}
		
		private class RTTTL
		{
			public string name;
			public List<Note> notes = new List<Note>();			
		}
		
		public TextAsset songAsset;
		
		public UnityEvent OnEndSong;
		
		private IWireOutput<float> _frequencyOutput;		
		private bool _playing;
		private List<RTTTL> _songs = new List<RTTTL>();
		private Trigger _preTrigger;
		private int _selection = -1;
		private int _index;
		private int _noteIndex;
		private float _time;
		
		protected override void Awake()
		{
            base.Awake();
            
            _preTrigger = new Trigger();
            _preTrigger.Clear();
			
			LoadRTTTL();
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_playing)
			{
				_time -= Time.deltaTime;
				if(_time < 0f)
				{
					_noteIndex++;
					if(_noteIndex >= _songs[_index].notes.Count)
					{
						if(_frequencyOutput != null)
							_frequencyOutput.output = 0f;
						_playing = false;
						OnEndSong.Invoke();
					}
					else
					{
						_time = _songs[_index].notes[_noteIndex].time;
						if(_frequencyOutput != null)
							_frequencyOutput.output = _songs[_index].notes[_noteIndex].frequency;
					}					
				}
			}
		}
		
		public string[] songs
		{
			get
			{
				List<string> list = new List<string>();
				for(int i=0; i<_songs.Count; i++)
					list.Add(_songs[i].name);
				
				return list.ToArray();
			}
		}
		
		public void LoadRTTTL()
		{
			if(songAsset == null)
				return;
			
			_selection = -1;
			_songs.Clear();
			
			using(StringReader reader = new StringReader(songAsset.text))
			{
				while(true)
				{
					string line = reader.ReadLine();
					if(line == null)
						break;
					
					RTTTL song = new RTTTL();
					string[] tokens = line.Split(new char[] { ':' });
					if(tokens.Length != 3)
						continue;
					
					song.name = tokens[0];
					
					string[] tokens2 = tokens[1].Split(new char[] { ',' });
					if(tokens2.Length != 3)
						continue;
					
					int duration = 4;
					int octave = 6;
					int beat = 63;
					for(int i=0; i<tokens2.Length; i++)
					{
						string[] tokens3 = tokens2[i].Split(new char[] { '=' });
						if(tokens3.Length == 2)
						{
							if(tokens3[0].Equals("d") || tokens3[0].Equals("D"))
								duration = int.Parse(tokens3[1]);
							else if(tokens3[0].Equals("o") || tokens3[0].Equals("O"))
								octave = int.Parse(tokens3[1]);
							else if(tokens3[0].Equals("b") || tokens3[0].Equals("B"))
								beat = int.Parse(tokens3[1]);
						}
					}
					
					string[] tokens4 = tokens[2].Split(new char[] { ',' });
					for(int i=0; i<tokens4.Length; i++)
					{
						char[] noteSymbol = { 'P', 'p', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g', 'A', 'a', 'B', 'b', 'H', 'h', '#' };
						
						tokens4[i] = tokens4[i].TrimStart(new char[] { ' ' });
						tokens4[i] = tokens4[i].TrimEnd(new char[] { ' ' });
						
						bool hasSpecialDuration = false;
						int index = tokens4[i].LastIndexOfAny(new char[] { '.' });
						if(index >= 0)
						{
							hasSpecialDuration = true;
							tokens4[i] = tokens4[i].Remove(index, 1);
						}
						
						int noteDuration = duration;
						index = tokens4[i].IndexOfAny(noteSymbol);
						if(index < 0)
							continue;
						if(index > 0)
							noteDuration = int.Parse(tokens4[i].Substring(0, index));
						
						int noteOctave = octave;
						int index2 = tokens4[i].LastIndexOfAny(noteSymbol);
						if(index2 < 0)
							continue;
						if(index2 < (tokens4[i].Length - 1))
							noteOctave = int.Parse(tokens4[i].Substring(index2 + 1));
						
						Note note = new Note();
						note.time = 60f / (float)beat * 4f;
						note.time /= (float)noteDuration;;
						if(hasSpecialDuration)
							note.time *= 1.5f;
						
						note.name = tokens4[i].Substring(index, index2 - index + 1);
						if(note.name.Equals("P") || note.name.Equals("p"))
							note.frequency = 0f;						
						else if(note.name.Equals("C") || note.name.Equals("c"))
							note.frequency = 261.63f;//Hz
						else if(note.name.Equals("C#") || note.name.Equals("c#"))
							note.frequency = 277.18f;//Hz
						else if(note.name.Equals("D") || note.name.Equals("d"))
							note.frequency = 293.66f;//Hz
						else if(note.name.Equals("D#") || note.name.Equals("d#"))
							note.frequency = 311.13f;//Hz
						else if(note.name.Equals("E") || note.name.Equals("e"))
							note.frequency = 329.63f;//Hz
						else if(note.name.Equals("F") || note.name.Equals("f"))
							note.frequency = 349.23f;//Hz
						else if(note.name.Equals("F#") || note.name.Equals("f#"))
							note.frequency = 369.99f;//Hz
						else if(note.name.Equals("G") || note.name.Equals("g"))
							note.frequency = 392f; //Hz
						else if(note.name.Equals("G#") || note.name.Equals("g#"))
							note.frequency = 415.3f;//Hz
						else if(note.name.Equals("A") || note.name.Equals("a"))
							note.frequency = 440f; //Hz
						else if(note.name.Equals("A#") || note.name.Equals("a#"))
							note.frequency = 466.16f; //Hz
						else if(note.name.Equals("B") || note.name.Equals("b"))
							note.frequency = 493.88f; //Hz
						else
							continue;
						
						if(note.frequency != 0)
							note.name += noteOctave.ToString();
						note.frequency *= Mathf.Pow(2, (noteOctave - 4));
						song.notes.Add(note);
					}
					
					_songs.Add(song);
				}				
			}
			
			if(_songs.Count > 0)
				_selection = 0;
		}
		
		public bool isPlaying
		{
			get
			{
				return _playing;
			}
		}
		
		public void SelectSong(int index)
		{
			if(index >= 0 && index < _songs.Count)
				_selection = index;
		}
		
		public void SelectSong(string name)
		{
			for(int i=0; i<_songs.Count; i++)
			{
				if(_songs[i].name.Equals(name))
				{
					_selection = i;
					return;
				}
			}
		}
		
		public void Play()
		{
			if(_selection < 0)
				return;
			
			_index = _selection;			
			_noteIndex = 0;
			_time = _songs[_index].notes[_noteIndex].time;
			if(_frequencyOutput != null)
				_frequencyOutput.output = _songs[_index].notes[_noteIndex].frequency;
			_playing = true;
		}
		
		public void Stop()
		{
			_playing = false;
			
			if(_frequencyOutput != null)
				_frequencyOutput.output = 0f;
		}
		
		Trigger IWireOutput<Trigger>.output
        {
			get
			{
				return _preTrigger;
			}
            set
            {
				if(value.value)
				{
					if(!_playing)
						Play();
					else
						Stop();
				}
								
				_preTrigger = value;
            }
        }
		
		int IWireOutput<int>.output
        {
			get
			{
				return _selection;
			}
            set
            {
				SelectSong(value);
            }
        }
		
		string IWireOutput<string>.output
        {
			get
			{
				if(_selection >= 0 && _selection < _songs.Count)
					return _songs[_selection].name;
				else
					return null;
			}
            set
            {
				SelectSong(value);
            }
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("frequency", "Frequency", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("play", "Play by Trigger", typeof(IWireOutput<Trigger>), NodeType.WireTo, "Output<Trigger>"));
			nodes.Add(new Node("selectIndex", "Select by index", typeof(IWireOutput<int>), NodeType.WireTo, "Output<int>"));
			nodes.Add(new Node("selectName", "Select by name", typeof(IWireOutput<string>), NodeType.WireTo, "Output<string>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("frequency"))
            {
				node.updated = true;
                if(node.objectTarget == null && _frequencyOutput == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_frequencyOutput))
                        return;
                }
                
                _frequencyOutput = node.objectTarget as IWireOutput<float>;;
                if(_frequencyOutput == null)
                    node.objectTarget = null;
                
                return;
            }
			else if(node.name.Equals("play"))
            {
				node.updated = true;
                return;
            }
			else if(node.name.Equals("selectIndex"))
            {
				node.updated = true;
                return;
            }
			else if(node.name.Equals("selectName"))
            {
				node.updated = true;
                return;
            }
                        
            base.UpdateNode(node);
        }
	}
}
