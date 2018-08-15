using System;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour {

    public Button thisButton;

    void Start()
    {
        try {
            thisButton.onClick.AddListener(Exit);
        }catch (NullReferenceException ex)
        {
            Debug.Log("예외발생");
        }
       
    }

    public void Exit()
    {
        Application.Quit();
    }
}
