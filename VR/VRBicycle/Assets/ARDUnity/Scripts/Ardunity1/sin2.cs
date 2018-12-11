using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sin2 : MonoBehaviour {
	public int whereAreYouGoing = 0; // 무슨 맵 할지

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveButton(){
		
		GameObject gameObject2;
		gameObject2 = GameObject.Find("Outline");

		if (whereAreYouGoing == 0)              // UI가 왼쪽에 있을 때는
		{
			Debug.Log (transform.position.x);
			transform.Translate((float)0.60,(float)0, (float)0);     // UI X좌표를 +230 이동
			Debug.Log(transform.position.x);
			whereAreYouGoing = 1;
		}
		else if (whereAreYouGoing == 1)
		{
			Debug.Log (transform.position.x);
			transform.Translate((float)-0.60,(float)0, (float)0);     // UI X좌표를 +220 이동 (숫자는 UI 구성에 맞게 수치를 구한거)
			Debug.Log(transform.position.x);
			whereAreYouGoing = 0;
		}
	}

	public void SelectButton(){
		if (whereAreYouGoing == 0)  // 첫번째 맵
		{
			SceneManager.LoadScene("04_MainMap");
		}
		else if (whereAreYouGoing == 1) // 두번째 맵
		{
			SceneManager.LoadScene("05_Game_Map");
		}
	}
}
