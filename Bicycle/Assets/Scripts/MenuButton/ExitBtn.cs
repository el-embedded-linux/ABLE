using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitBtn : MonoBehaviour {

    private Menu menu;
    public string LoadScene;
    public Button thisButton;

    void Start()
    {
        menu = FindObjectOfType<Menu>();
        thisButton.onClick.AddListener(StartLoad);
    }

    public void StartLoad()
    {
        menu.gameObject.SetActive(true);
        menu.SetShowFlag();
        SceneManager.LoadScene(LoadScene);
    }
}
