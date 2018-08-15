using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour {
    public int whatAreYouDoing = 0;
    public int whereAreYouGoing = 0;
    public int menu = 0;
    public void MovBut()
    {
        if (whatAreYouDoing == 0)
        {
            transform.Translate(0, -43, 0);
            Debug.Log(transform.position.y);
            whatAreYouDoing = 1;
        }
        else if (whatAreYouDoing == 1)
        {
            transform.Translate(0, 43, 0);
            Debug.Log(transform.position.y);
            whatAreYouDoing = 0;
        }
    }
    public void MovMap()
    {
        if (whereAreYouGoing == 0)
        {
            transform.Translate(230, 0, 0);
            Debug.Log(transform.position.x);
            whereAreYouGoing = 1;
        }
        else if (whereAreYouGoing == 1)
        {
            transform.Translate(220, 0, 0);
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
        if (menu == 0)
        {
            transform.Translate(0, -70, 0);
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
        else if (menu == 3)
        {
            transform.Translate(0, 195, 0);
            Debug.Log(transform.position.y);
            menu = 0;
        }

    }
}