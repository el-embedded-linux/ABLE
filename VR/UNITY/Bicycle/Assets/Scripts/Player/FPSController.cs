using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class FPSController : MonoBehaviour {

    RealTimeDB realTimeDB = new RealTimeDB();
	TrackCountController trackCnt;

	public float moveSpeed = 10.0f;
	public float rotSpeed = 8.0f;
	public string LoadScene;
	int count=0;

	public Camera fpsCam;

    // Use this for initialization
    void Start () {
		trackCnt = GetComponent<TrackCountController> ();
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
        float rotX = Input.GetAxis("Mouse Y") * rotSpeed;
        float rotY = Input.GetAxis("Mouse X") * rotSpeed;

		fpsCam.transform.localRotation *= Quaternion.Euler (0, rotY, 0);
		fpsCam.transform.localRotation *= Quaternion.Euler (-rotX, 0, 0);
	}
 
    private void OnTriggerEnter(Collider col) {
        if (col.tag == "endCheck") {
            Debug.Log("enter EndCheck");
			count += 1;
			trackCnt.updateCount(count);
			if (count == 2) {
				SceneManager.LoadScene (LoadScene);
                try
                {
                    realTimeDB.InitDatabase(this);
                }catch(NullReferenceException ex)
                {
                    Debug.Log("데이터 저장 예외발생");
                }
			}
        }
    }

}
