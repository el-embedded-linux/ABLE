using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class Btn : MonoBehaviour {

    public int whatAreYouDoing = 0; // 뭐할지 (시작할지 종료할지)

    SerialPort sp = new SerialPort("COM5", 9600);

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
    public void DoSelect(int a)
    {
        if (a == 1)
        {
            if (whatAreYouDoing == 0)               // UI가 위에 있을 때는
            {
                transform.Translate(0, -43, 0);     // UI Y좌표를 -43
                Debug.Log(transform.position.y);
                whatAreYouDoing = 1;
            }
            else if (whatAreYouDoing == 1)          // 아래에 있을 때는
            {
                transform.Translate(0, 43, 0);      // UI Y좌표를 다시 +43
                Debug.Log(transform.position.y);
                whatAreYouDoing = 0;
            }
        }
        else if (a == 2)
        {
            GameObject gameObject;
            gameObject = GameObject.Find("Outline");
            if (whatAreYouDoing == 0)
            {
                SceneManager.LoadScene("03_Mapselect");
            }
            else if (whatAreYouDoing == 1)
            {
                SceneManager.LoadScene("01_Login");
            }
        }
    }
}
