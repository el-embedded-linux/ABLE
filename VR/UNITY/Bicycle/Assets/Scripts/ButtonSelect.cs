using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour {
    public int whatAreYouDoing = 0; // 뭐할지 (시작할지 종료할지)
    public int whereAreYouGoing = 0; // 무슨 맵 할지
    public int menu = 0;            // 무슨 메뉴를 선택할지

    public void MovBut()
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
    public void MovMap()
    {
        if (whereAreYouGoing == 0)              // UI가 왼쪽에 있을 때는
        {
            transform.Translate(230, 0, 0);     // UI X좌표를 +230 이동
            Debug.Log(transform.position.x);
            whereAreYouGoing = 1;
        }
        else if (whereAreYouGoing == 1)         
        {
            transform.Translate(220, 0, 0);     // UI X좌표를 +220 이동 (숫자는 UI 구성에 맞게 수치를 구한거)
            Debug.Log(transform.position.x);
            whereAreYouGoing = 2;
        }
        else if (whereAreYouGoing == 2)
        {
            transform.Translate(-450, 0, 0);
            Debug.Log(transform.position.x);
            whereAreYouGoing = 0;
        }
    }
    public void MovMenu()
    {
        if (menu == 0)                               // UI가 맨위에 있을 때는
        {
            transform.Translate(0, -70, 0);         // UI Y좌표를 -70
            Debug.Log(transform.position.y);
            menu = 1;
        }
        else if (menu == 1)
        {
            transform.Translate(0, -60, 0);
            Debug.Log(transform.position.y);
            menu = 2;
        }
        else if (menu == 2)
        {
            transform.Translate(0, -65, 0);
            Debug.Log(transform.position.y);
            menu = 3;
        }
        else if (menu == 3)                         // UI가 맨 아래에 있을 때는
        {
            transform.Translate(0, 195, 0);         // UI 좌표를 내린만큼 다시 올림
            Debug.Log(transform.position.y);
            menu = 0;
        }

    }
}