using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class Btn_Menu : MonoBehaviour
{
    private Menu menu2;

    public static int menu = 0;            // 무슨 메뉴를 선택할지

	SerialPort sp = new SerialPort("\\\\.\\COM16", 9600);

    // Use this for initialization
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
                DoSelect(sp.ReadByte());
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
        sp.Close();
    }
    public void DoSelect(int a)
    {
        RealTimeDB realTimeDB = gameObject.AddComponent<RealTimeDB>();
        ContinueBtn cb = gameObject.AddComponent<ContinueBtn>();
        if (a == 1)
        {
            if (menu == 0)                               // UI가 맨위에 있을 때는
            {
                transform.Translate(0, -70, 0);         // UI Y좌표를 -70
                menu = 1;
            }
            else if (menu == 1)
            {
                transform.Translate(0, -60, 0);
                menu = 2;
            }
            else if (menu == 2)
            {
                transform.Translate(0, -65, 0);
                menu = 3;
            }
            else if (menu == 3)                         // UI가 맨 아래에 있을 때는
            {
                transform.Translate(0, 195, 0);         // UI 좌표를 내린만큼 다시 올림
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
                transform.Translate(0, 60, 0);
                cb.ButtonOnClick();                                     // 메뉴 버튼 다시 내리기
            }
            else if (menu == 2)
            {
                menu = 0;
                transform.Translate(0, 125, 0);
                SceneManager.LoadScene("03_Mapselect");                 // 맵 선택 씬으로 이동
            }
            else if (menu == 3)
            {
                menu = 0;
                transform.Translate(0, 195, 0);
                SceneManager.LoadScene("02_Menu");                      // 메뉴 선택 씬으로 이동
            }
        }
        else if (a == 3)
        {
            cb.ButtonOnClick();

        }
    }
}