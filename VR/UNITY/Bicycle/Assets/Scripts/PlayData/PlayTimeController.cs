using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimeController : MonoBehaviour {

	public Text timerText;
    public static string playTime;
    public static string kcal;
    public static double kcal_m;

    private float startTime;

    void Start () {
		startTime = Time.time;
	}

    // Update is called once per frame
    void Update () {
		float t = Time.time - startTime;

        //(몸무게(70kg)+자전거무게(10kg)) x 평속에 따른 소모열량지수(0.1129) x 운동시간(분 단위) = kcal
        float min = t / 60;
        kcal_m = 80 * 0.1129 * min;
        kcal = kcal_m.ToString("f1") + "kcal";
        
       
        string minutes = ((int)t / 60).ToString ();
		string seconds = (t % 60).ToString ("f2");
		string hours = ((int)t / 3600 % 24).ToString ();

        playTime = hours + ":" + minutes + ":" + seconds;
		timerText.text = "PlayTime " + hours + ":" + minutes + ":" + seconds;
	}
}
