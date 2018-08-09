using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAct : MonoBehaviour {
    
    public void Login()    // 로그인 창에서 메인 화면으로
    {
        SceneManager.LoadScene("02_Menu");
    }
    public void GoToSelect()   // 메인 화면에서 맵 선택 창으로
    {
        GameObject gameObject;
        gameObject = GameObject.Find("Outline");
        float one, two;
        one = (float)160.5;
        two = one - 43;
        Debug.Log(one +" "+ two);
        if (gameObject.transform.position.y == one)
        {
            SceneManager.LoadScene("03_Mapselect");
        }
        else if(gameObject.transform.position.y == two)
        {
            SceneManager.LoadScene("01_Login");
        }
    }
    public void GoToMap()   // 맵 선택 창에서 맵으로
    {
        ButtonSelect bts = gameObject.AddComponent<ButtonSelect>();
        GameObject gameObject2;
        gameObject2 = GameObject.Find("Outline");
        float one = 98,two,three;
        two = one + 230;
        three = two + 220;
        if (gameObject2.transform.position.x == one)  // 첫번째 맵
        {
            SceneManager.LoadScene("04_Main");
        }
        else if (gameObject2.transform.position.x == two) // 두번째 맵
        {
            SceneManager.LoadScene("03_Mapselect");
        }
        else if (gameObject2.transform.position.x == three)   // 세번째 맵
        {
            SceneManager.LoadScene("05_End");
        }
    }
    public void ActMenu()
    {
        RealTimeDB realTimeDB = gameObject.AddComponent<RealTimeDB>();
        ContinueBtn cb = gameObject.AddComponent<ContinueBtn>();


        GameObject gameObject2;
        gameObject2 = GameObject.Find("Outline_m");
        float one, two, three, four;
        one = (float)256.5;
        two = one - 70;
        three = two - 60;
        four = three - 65;
        Debug.Log(one+" " + two+" " + three+" " + four);

        if (gameObject2.transform.position.y == one) 
        {
            Debug.Log("1");
            realTimeDB.InitDatabase();                              // FIrebase에 저장
        }
        else if (gameObject2.transform.position.y == two)
        {
            Debug.Log("2");
            cb.ButtonOnClick();                                     // 메뉴 버튼 다시 내리기
        }
        else if (gameObject2.transform.position.y == three)
        {
            Debug.Log("3");
            SceneManager.LoadScene("03_Mapselect");                 // 맵 선택 씬으로 이동
        }
        else if(gameObject2.transform.position.y == four)
        {
            Debug.Log("4");
            SceneManager.LoadScene("02_Menu");                      // 메뉴 선택 씬으로 이동
        }
    }
}
