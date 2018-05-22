using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour {

    public Button thisButton;

    void Start()
    {
        thisButton.onClick.AddListener(Exit);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
