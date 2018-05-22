using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {
	public float moveSpeed = 10.0f;
	public float rotSpeed = 20.0f;

	public Camera fpsCam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MoveCtrl ();
		RotCtrl ();
	}

	void MoveCtrl(){
		if (Input.GetKey (KeyCode.W)) {
			this.transform.Translate (Vector3.forward * moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.S)) {
			this.transform.Translate (Vector3.back * moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.A)) {
			this.transform.Translate (Vector3.left * moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.D)) {
			this.transform.Translate (Vector3.right * moveSpeed * Time.deltaTime);
		}
	}

	void RotCtrl(){
		float rotX = Input.GetAxis ("Mouse Y") * rotSpeed;
		float rotY = Input.GetAxis ("Mouse X") * rotSpeed;

		this.transform.localRotation *= Quaternion.Euler (0, rotY, 0);
		fpsCam.transform.localRotation *= Quaternion.Euler (-rotX, 0, 0);
	}

}
