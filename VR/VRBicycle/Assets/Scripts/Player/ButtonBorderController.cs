using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonBorderController : MonoBehaviour {

    public Image BorderImage;

    //void OnMouseEnter()
    //{
    //    BorderImage.fillAmount = 1;
    //    Debug.Log("on");
    //}

    //void OnMouseExit()
    //{
    //    BorderImage.fillAmount = 0;
    //}
    public UnityEvent OnGazeClick;
    public float GazeTriggerTime = 3f;

    private bool isLoading = false;
    private float counter;

    void Update()
    {
        if (isLoading)
        {
            counter += Time.deltaTime;
            BorderImage.fillAmount += counter / GazeTriggerTime;
            if (counter > GazeTriggerTime)
            {
                OnGazeClick.Invoke();
            }
        }
    }

    public void OnMouseEnter()
    {
        isLoading = true;
    }
    public void OnMouseExit()
    {
        isLoading = false;
        BorderImage.fillAmount = 0;
        counter = 0;
    }
}
