using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/Event/ToggleEvent")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/toggleevent")]
	[RequireComponent(typeof(Toggle))]
	public class ToggleEvent : MonoBehaviour
	{
        public UnityEvent OnChecked;
        public UnityEvent OnUnchecked;

		private Toggle _toggle;
		
		void Awake()
		{
			_toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		// Use this for initialization
		void Start ()
		{
            OnToggleChanged(_toggle.isOn);	
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
        
        private void OnToggleChanged(bool value)
		{
            if(value)
                OnChecked.Invoke();
            else
                OnUnchecked.Invoke();
		}
	}
}
