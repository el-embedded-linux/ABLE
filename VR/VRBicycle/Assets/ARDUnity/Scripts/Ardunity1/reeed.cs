using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using Ardunity;
public class reeed : MonoBehaviour {

	Stopwatch sw = new Stopwatch();
	public static int menu = 0;
	public static string curSpeed;
	public static string totalDist;
	public static string totalDist2;
	public static string speed;
	//float radius = 20; // ??? ?? ??? ?? ?? ?? ??? ??? ???? ??? ???.(Cm ??)
	float circle = (float)(2 * 35 * 3.14) / 100;  // ??? ??? ??? ??(??? m? ???? ?? 100? ????.)

	public static float bySpeed = 0; // ???? ??
	float ckTime = 0;  // ?????? 
	float uckTime = 0; // Unckecked
	float cycleTime = 0;  // ?????? ??? ??? ?? ?? ???? ???? ??
	float distance = 0; // ???? ?? ?? ??
	float lcdDis = 0; // ???? ?? ??? LCD??? ?? ??? ?.(?? ?? or ??? ??)
	float bicdis = 0;
	float avgSpeed;

	int count = 0;  // ?????? ???? ???? ?? ???? ?????.
	bool temp = false;  // ?? ???? ???? ???? ??

	public DigitalInput digitalInput;


	void Start()
	{
		sw.Start();
		digitalInput.OnValueChanged.AddListener(OnDigitalInputChanged);


	}
	void Update()
	{

		bySpeed = bySpeed-0.04f;
		if(bySpeed<0)
			bySpeed = 0;
		string realspeed = GetN2(bySpeed);
		curSpeed = (float.Parse (realspeed) / 2.5f).ToString();
	}

	private string GetN2(float A)
	{
		string result = string.Empty;

		if (A == (int)A)
			result = A.ToString();
		else
			result = A.ToString("N1");

		return result;
	}

	void OnDigitalInputChanged(bool value)
	{

		if (value == true && temp == false)
		{  // ?? ???? ?? ?(?? -> ??)
			ckTime = sw.ElapsedMilliseconds;  // ??? ???? ?????.
			temp = true;  // temp?? 1? ?????.(?????? ???? ??? ??)
		}

		else if (value == false && temp == true)
		{  // 리드 스위치가 닫히고(열림 -> 닫힘), 노이즈 방지 카운터 삭제
			Debug.Log("Close");
			uckTime = sw.ElapsedMilliseconds;   // 시간을 확인해서 저장합니다.



			cycleTime = (uckTime - ckTime);
			// 열릴 때 시각과 닫힐 때 시각의 차를 이용하여 바퀴가 한바퀴 돌때 걸린 시간을 계산합니다.\
			bySpeed = (float)((circle / cycleTime) * 360); // 바퀴가 한바퀴 돌때의 거리와 시간을 가지고 속도를 구해줍니다.(단위는 Km/h입니다.)
			temp = false;

			distance += circle;  // 한바퀴 돌았으면 이동거리를 누적 이동거리에 더해줍니다.
			//bicdis += 0.0028f;
			avgSpeed = distance / (RealTimeDB.t / 60); //분당평균 속도
			speed = avgSpeed.ToString("f2");
			//Debug.Log(avgSpeed);
			totalDist = distance.ToString("f2");//bicdis.ToString("f2");
			totalDist2 = totalDist + "M";


		}
		if (value == true)
		{  // ?? ???? ????? ???? 1? ?? ?? ???.
			count++;
			if (count > 150)
			{ // ???? 150? ????(???? ??? ?) ??? 0?? ?????.
				bySpeed = 0;
			}
		}


		string realspeed = GetN2(bySpeed);
		curSpeed = (float.Parse (realspeed) / 2.0f).ToString();
		Debug.Log("Speed: " + realspeed + "km/h");
		Debug.Log("curSpeed: " + curSpeed + "km/h");


	}
}
