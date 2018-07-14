using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/SliderSnap")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/slidersnap")]
	[RequireComponent(typeof(Slider))]
	public class SliderSnap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public float snapValue;
		
		private Slider _slider;
		private bool _drag;
		
		void Awake()
		{
			_slider = GetComponent<Slider>();
		}

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(!_drag)
				_slider.value = snapValue;		
		}
		
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
			if(!this.enabled)
				return;
			
			_drag = true;
        }
		
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
			if(!this.enabled)
				return;
				
			_drag = false;
        }
	}
}
