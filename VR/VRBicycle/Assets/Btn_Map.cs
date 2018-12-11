using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class Btn_Map : MonoBehaviour
{

    public int whereAreYouGoing = 0; // 무슨 맵 할지

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
    public void DoSelect(int a)
    {
        if (a == 1)
        {
            if (whereAreYouGoing == 0)              // UI가 왼쪽에 있을 때는
            {
				Debug.Log (transform.position.x);
				transform.Translate((float)0.5,(float)0, (float)0);     // UI X좌표를 +230 이동
                Debug.Log(transform.position.x);
                whereAreYouGoing = 1;
            }
            else if (whereAreYouGoing == 1)
			{
				Debug.Log (transform.position.x);
				transform.Translate((float)-0.5,(float)0, (float)0);     // UI X좌표를 +220 이동 (숫자는 UI 구성에 맞게 수치를 구한거)
                Debug.Log(transform.position.x);
                whereAreYouGoing = 0;
            }
        }
        else if (a == 2)
        {
            GameObject gameObject2;
            gameObject2 = GameObject.Find("Outline");
            if (whereAreYouGoing == 0)  // 첫번째 맵
            {
				sp.Close ();
				SceneManager.LoadScene("04_MainMap");
            }
            else if (whereAreYouGoing == 1) // 두번째 맵
            {
				sp.Close ();
				SceneManager.LoadScene("05_Game_Map");
            }
      
        }
    }
}