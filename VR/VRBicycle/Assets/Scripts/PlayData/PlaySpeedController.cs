using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySpeedController : MonoBehaviour {

    public Text speedText;


	// Use this for initialization
	void Start () {
		speedText.text = "0";
    }
	
	// Update is called once per frame
	void Update () {
        speedText.text = reeed.curSpeed;
    }
}
