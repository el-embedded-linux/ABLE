using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Utility/UI/ListItem")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/listitem")]
    [RequireComponent(typeof(Toggle))]
	public class ListItem : MonoBehaviour
    {
        public ListView owner;
        public Image image;
        [SerializeField]
        public Text[] textList;
        public System.Object data;

        private Toggle _toggle;

		void Awake()
		{
			_toggle = GetComponent<Toggle>();
			_toggle.onValueChanged.AddListener(OnClick);
		}

        public int index
        {
            get
            {
                return this.transform.GetSiblingIndex();
            }
        }

        // This property is for ListView
        public bool selected
        {
            get
            {
                return _toggle.isOn;
            }
            set
            {
				_toggle.onValueChanged.RemoveListener(OnClick);
                _toggle.isOn = value;
				_toggle.onValueChanged.AddListener(OnClick);
            }
        }

		void OnClick(bool isOn)
        {
            owner.selectedItem = this;
        }
    }
}
