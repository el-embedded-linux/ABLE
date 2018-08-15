using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

   
    private bool showFlag; // showFlag = true면 메뉴가 보여짐

    void Awake()
    {
       
        showFlag = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffMenu();
        }

        if (showFlag)
        {
            Time.timeScale = 0; // 메뉴가 보이면 게임 일시 정지
        }
        else
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }

    public void SetShowFlag()
    {
        if (showFlag) showFlag = false;
        else showFlag = true;
    }

    public void OffMenu()
    {
        showFlag = false;
    }

    public static void SetChecked(string menuItemName, bool setPrefsForUtilities)
    {
        throw new NotImplementedException();
    }
}
