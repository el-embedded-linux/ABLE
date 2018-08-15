using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalDistantceController : MonoBehaviour {

    public Text dist;

	// Update is called once per frame
	void Update () {
        dist.text = NewBehaviourScript.totalDist;
	}
}
