using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KcalTextController : MonoBehaviour {

    public Text kcalText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        kcalText.text = PlayTimeController.kcal_m.ToString("f1") + "kcal/m";
	}
}
