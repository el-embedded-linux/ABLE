using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeDB : MonoBehaviour
{   
    public static string playTime;
    public static string kcal;
    public static string recordDate = DateTime.Now.ToString("yyyy-M-dd");
	public static float t;

    FirebaseApp firebaseApp;
    public static DatabaseReference databaseReference;

    public class User
    {
        public string playTime;
        public string kcal;
		public string distance;
		public string speed;

        public User()
        { }

		public User(string playTime, string kcal, string distance, string speed)
        {
            this.playTime = playTime;
            this.kcal = kcal;
			this.distance = distance;
			this.speed = speed;
        }
    }

    private float startTime;

    // 초기화 밑 설정
    void Awake()
    {

        firebaseApp = FirebaseDatabase.DefaultInstance.App;
        firebaseApp.SetEditorDatabaseUrl("https://able-7f90d.firebaseio.com/");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        startTime = Time.time;
    }

    //버튼 눌렀을때 동작
    public void InitDatabase()
    {
		WriteNewUser(Login.user.UserId, PlayTimeController.playTime, PlayTimeController.kcal, NewBehaviourScript.totalDist, NewBehaviourScript.speed);
    }

    //게임종료시
    public void InitDatabase(FPSController fPSController)
    {
		WriteNewUser(Login.user.UserId, PlayTimeController.playTime, PlayTimeController.kcal, NewBehaviourScript.totalDist, NewBehaviourScript.speed);
    }
    //파이어베이스 저장 함수
	private void WriteNewUser(string uid, string playTime, string kcal, string distance, string speed)
    {
		User user = new User(playTime, kcal, distance, speed);
        string json = JsonUtility.ToJson(user);
		databaseReference.Child ("HEALTH").Child (uid).Child (recordDate).SetRawJsonValueAsync(json);
    }
    // Update is called once per frame
    void Update()
    {
        t = Time.time - startTime;

        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f2");
        string hours = ((int)t / 3600 % 24).ToString();

        playTime = hours + ":" + minutes + ":" + seconds;
    }

}
