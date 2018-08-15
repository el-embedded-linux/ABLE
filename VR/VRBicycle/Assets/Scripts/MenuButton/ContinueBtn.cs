using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueBtn : MonoBehaviour {

    private Menu menu;

    void Awake()
    {
        menu = FindObjectOfType<Menu>();
    }

    public void ButtonOnClick()
    {
        menu.gameObject.SetActive(true);
        menu.SetShowFlag();
    }
}
