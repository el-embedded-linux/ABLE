using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackCountController : MonoBehaviour {

     public Text countText;
     string trackCount;
    
    void Start () {
    }
	
	void Update () {
	}

    public void updateCount(int num)
    {
        trackCount = num + "바퀴";
		Debug.Log (trackCount);
        //countText.text = num + "바퀴";
    }
}
