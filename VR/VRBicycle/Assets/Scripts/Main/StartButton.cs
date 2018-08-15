using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {

    public string LoadScene;
    public Button thisButton;

    void Start()
    {
        thisButton.onClick.AddListener(SelectLoad);
    }

    public void SelectLoad()
    {
        SceneManager.LoadScene(LoadScene);
    }
}