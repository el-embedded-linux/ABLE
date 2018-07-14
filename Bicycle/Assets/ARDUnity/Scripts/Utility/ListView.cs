using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


namespace Ardunity
{    
    [Serializable]
	public class ListViewEvent : UnityEvent<ListItem> {}
    
    
    [AddComponentMenu("ARDUnity/Utility/UI/ListView")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/listview")]
    public class ListView : MonoBehaviour
    {
        public RectTransform itemRoot;

        public ListViewEvent OnSelectionChanged;

        private int _itemNum = 0;
        private ListItem _selectedItem;

        public int itemCount
        {
            get
            {
                if (itemRoot == null)
                    return 0;
                else
                    return itemRoot.transform.childCount;
            }
        }

        public ListItem[] items
        {
            get
            {
                List<ListItem> list = new List<ListItem>();
                foreach (Transform item in itemRoot.transform)
                    list.Add(item.GetComponent<ListItem>());

                return list.ToArray();
            }
        }

        public ListItem selectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                bool changed = false;
                if (_selectedItem != null)
                {
                    if (_selectedItem.Equals(value) == false)
                        changed = true;
                }
                else
                {
                    if (value != null)
                        changed = true;
                }

                if (_selectedItem != null)
                    _selectedItem.selected = false;

                _selectedItem = value;
                if (_selectedItem != null)
                    _selectedItem.selected = true;

                if (changed == true)
                    OnSelectionChanged.Invoke(_selectedItem);
            }
        }

        public int selectedIndex
        {
            get
            {
                if (_selectedItem == null)
                    return -1;

                return _selectedItem.index;
            }
            set
            {
                if (value < 0 || value >= itemCount)
                    return;

                selectedItem = itemRoot.transform.GetChild(value).GetComponent<ListItem>();
            }
        }

        public void ClearItem()
        {
            if (_itemNum == 0)
                return;
                
            selectedItem = null;

            List<GameObject> list = new List<GameObject>();
            foreach (Transform item in itemRoot.transform)
                list.Add(item.gameObject);

            for (int i = 0; i < list.Count; i++)
                GameObject.DestroyImmediate(list[i]);

            _itemNum = 0;            
        }

        public void AddItem(ListItem item)
        {
            if (item == null)
                return;

            item.transform.SetParent(itemRoot.transform);
			item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
            item.owner = this;
            _itemNum++;
        }

        public void InsertItem(ListItem item)
        {
            if (_selectedItem == null || item == null)
                return;

            int index = _selectedItem.index;
            AddItem(item);
            item.transform.SetSiblingIndex(index);
        }

        public void RemoveItem()
        {
            if (_selectedItem == null)
                return;

            GameObject.DestroyImmediate(_selectedItem.gameObject);
            _itemNum--;
            selectedItem = null;
        }
    }
}
