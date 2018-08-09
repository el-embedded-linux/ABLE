using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

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
