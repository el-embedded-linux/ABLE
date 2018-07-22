using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndManager : MonoBehaviour {
    int count = 0;
    private Menu menu;

    void Awake()
    {
        menu = FindObjectOfType<Menu>();
    }

    void MenuPopup()
    {
        menu.gameObject.SetActive(true);
        menu.SetShowFlag();
    }

    /* void OnTriggerEnter(Collider collider)
     {
         Debug.Log("check");
         if (collider.tag == "endCheck")
         {
             count++;
             if (count == 1)
             {
                 MenuPopup();
             }
         }
     }*/
    private void OnTriggerEnter(Collider col) {
        Debug.Log("enter check");
        if (col.tag == "endCheck") {
            Debug.Log("End!!");
        }
    }
    
    //void Update()
    //{
    //    RaycastHit hit;
    //    transform.position = new Vector3(transform.position.y + 0.01f, 0.92f, transform.position.z + 0.01f);

    //    if(Physics.Raycast(transform.position,transform.forward,out hit, 5f))
    //    {
    //        if (hit.collider.gameObject.tag == "endCheck")
    //        {
    //            Debug.Log("collier!");
    //        }
    //    }
    //}

}
