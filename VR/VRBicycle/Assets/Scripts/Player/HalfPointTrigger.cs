using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPointTrigger : MonoBehaviour {
	public GameObject LapComleteTrig;
	public GameObject HalfLapTrig;

	void OnTriggerEnter(){
		LapComleteTrig.SetActive (true);
		HalfLapTrig.SetActive (false);
	}
}
