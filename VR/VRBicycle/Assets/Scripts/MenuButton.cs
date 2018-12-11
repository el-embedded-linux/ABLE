using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;

public class MenuButton : MonoBehaviour
{

    private Menu menu2;
    public static int menu = 0;            // 무슨 메뉴를 선택할지

	SerialPort sp = new SerialPort("\\\\.\\COM16", 9600);

    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                int a = sp.ReadByte();
                if (a == 3)
                {
                    Debug.Log("3");
                    ButtonOnClick();
                }
                else if (a == 1)
                {
                    Debug.Log("1");
                    DoSelect(a);
                }
                else if (a == 2)
                {
                    Debug.Log("2");
                    DoSelect(a);
                }
            }
            catch (System.Exception) { }

        }
    }
    void Awake()
    {
        menu2 = FindObjectOfType<Menu>();
    }

    public void ButtonOnClick()
    {
        menu2.gameObject.SetActive(true);
        menu2.SetShowFlag();
    }

    public void DoSelect(int a)
    {
        RealTimeDB realTimeDB = gameObject.AddComponent<RealTimeDB>();
        ContinueBtn cb = gameObject.AddComponent<ContinueBtn>();
        if (a == 1)
        {
            GameObject gameObject2;
            gameObject2 = GameObject.Find("Outline_m");
            if (menu == 0)                               // UI가 맨위에 있을 때는
            {
				gameObject2.transform.Translate(0, (float)-0.1, 0);         // UI Y좌표를 -70
                menu = 1;
            }
            else if (menu == 1)
            {
				gameObject2.transform.Translate(0, (float)-0.1, 0);
                menu = 2;
            }
            else if (menu == 2)
            {
				gameObject2.transform.Translate(0, (float)-0.1, 0);
                menu = 3;
            }
            else if (menu == 3)                         // UI가 맨 아래에 있을 때는
            {
				gameObject2.transform.Translate(0,(float)0.3, 0);         // UI 좌표를 내린만큼 다시 올림
                menu = 0;
            }
        }
        else if (a == 2)
        {

            GameObject gameObject2;
            gameObject2 = GameObject.Find("Outline_m");

            if (menu == 0)
            {
                menu = 0;
                realTimeDB.InitDatabase();                              // FIrebase에 저장
            }
            else if (menu == 1)
            {
                menu = 0;
                gameObject2.transform.Translate(0, 60, 0);
                cb.ButtonOnClick();                                     // 메뉴 버튼 다시 내리기
            }
            else if (menu == 2)
            {
                menu = 0;
                gameObject2.transform.Translate(0, 125, 0);
                SceneManager.LoadScene("03_Mapselect");                 // 맵 선택 씬으로 이동
            }
            else if (menu == 3)
            {
                menu = 0;
                gameObject2.transform.Translate(0, 195, 0);
                SceneManager.LoadScene("02_Menu");                      // 메뉴 선택 씬으로 이동
            }
        }
    }
}
