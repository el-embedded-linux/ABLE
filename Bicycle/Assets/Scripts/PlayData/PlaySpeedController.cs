using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySpeedController : MonoBehaviour {

    public Text speedText;


	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        speedText.text = NewBehaviourScript.curSpeed;
    }
}
