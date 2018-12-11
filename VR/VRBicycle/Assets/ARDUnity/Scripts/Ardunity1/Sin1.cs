using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sin1 : MonoBehaviour {
	
	public int whatAreYouDoing = 0; // 뭐할지 (시작할지 종료할지)
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () { 
		
		
	}
	public void MoveButton(){

		GameObject gameObject;
		gameObject = GameObject.Find("Outline");
		Debug.Log ("btn3");
			if (whatAreYouDoing == 0)               // UI가 위에 있을 때는
			{
				gameObject.transform.Translate((float)0,(float)-0.065, (float)0); 
				Debug.Log(transform.position.y);
				whatAreYouDoing = 1;
			}
			else if (whatAreYouDoing == 1)          // 아래에 있을 때는
			{
				gameObject.transform.Translate((float)0, (float)0.065, (float)0);     
				Debug.Log(transform.position.y);
				whatAreYouDoing = 0;
			}
	}

	public void SelectButton(){
		Debug.Log ("btn4");
		if (whatAreYouDoing == 0)
		{
			SceneManager.LoadScene("03_Mapselect");
		}
		else if (whatAreYouDoing == 1)
		{
			SceneManager.LoadScene("01_Login");
		}
	}
}