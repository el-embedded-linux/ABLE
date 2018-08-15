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
    public static string RecordDate = DateTime.Now.ToString("yyyy-M-dd");

    FirebaseApp firebaseApp;
    public static DatabaseReference databaseReference;

    public class User
    {
        public string playTime;
        public string kcal;
		public string distance;

        public User()
        { }

		public User(string playTime, string kcal, string distance)
        {
            this.playTime = playTime;
            this.kcal = kcal;
			this.distance = distance;
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
        WriteNewUser(Login.user.UserId, PlayTimeController.playTime, PlayTimeController.kcal, NewBehaviourScript.totalDist);
    }

    //게임종료시
    public void InitDatabase(FPSController fPSController)
    {
        WriteNewUser(Login.user.UserId, PlayTimeController.playTime, PlayTimeController.kcal, NewBehaviourScript.totalDist);
    }
    //파이어베이스 저장 함수
	private void WriteNewUser(string uid, string playTime, string kcal, string distance)
    {
        User user = new User(playTime, kcal, distance);
        string json = JsonUtility.ToJson(user);
		databaseReference.Child ("HEALTH").Child (uid).Child (RecordDate).SetRawJsonValueAsync(json);
    }
    // Update is called once per frame
    void Update()
    {
        float t = Time.time - startTime;

        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f2");
        string hours = ((int)t / 3600 % 24).ToString();

        playTime = hours + ":" + minutes + ":" + seconds;
    }

}
