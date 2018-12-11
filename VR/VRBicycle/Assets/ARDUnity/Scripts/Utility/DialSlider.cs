using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Ardunity
{	
	[AddComponentMenu("ARDUnity/Utility/UI/DialSlider")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/dialslider")]
	[RequireComponent(typeof(RectTransform))]
	public class DialSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
	{
		public RectTransform knob;		
		[Range(-180f, 0f)]
		public float minAngle = -180f;
		[Range(0f, 180f)]
		public float maxAngle = 180f;
		public bool interactable = true;
		public bool spring = true;

		public UnityEvent OnDragStart;
		public UnityEvent OnDragEnd;
		public FloatEvent OnAngleChanged;
	
		private RectTransform _rectTransform;
		private Vector2 _prePos;
		private float _sumAngle;
		
		void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}
	
		// Use this for initialization
		void Start ()
		{			
		}
		
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if(interactable == false || knob == null)
				return;
			
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _prePos);
			_sumAngle = knob.localEulerAngles.z;
			if(_sumAngle > 180f)
				_sumAngle -= 360f;
			else if(_sumAngle < -180f)
				_sumAngle += 360f;
		}
	
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if(interactable == false || knob == null)
				return;
	
			Vector2 pos;
			if(RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out pos))
			{
				float a = Vector2.Angle(_prePos, pos);
				Vector3 axis = Vector3.Cross(_prePos, pos);
				if(axis.z < 0f)
					a = -a;
	
				_sumAngle += a;
				angle = _sumAngle;
				_prePos = pos;
			}
		}
		
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if(interactable == false || knob == null)
				return;
			
			if(spring)
				angle = 0f;
		}
		
		public float angle
		{
			set
			{
				if(knob == null)
					return;
	
				float a = Mathf.Clamp(value, minAngle, maxAngle);
	
				Vector3 eulerAngles = knob.localEulerAngles;
				if(Mathf.Approximately(eulerAngles.z, a) == false)
				{
					eulerAngles.z = a;
					knob.localEulerAngles = eulerAngles;
					OnAngleChanged.Invoke(a);
				}
			}
			get
			{
				if(knob == null)
					return 0f;
	
				float a = knob.localEulerAngles.z;
				if(a > 180f)
					a -= 360f;
				else if(a < -180f)
					a += 360f;
	
				return a;
			}
		}
        
        public void SetInteractable(bool value)
        {
            interactable = value;
            
            if(interactable)
            {
                if(spring)
				    angle = 0f;
            }
        }
	}
}