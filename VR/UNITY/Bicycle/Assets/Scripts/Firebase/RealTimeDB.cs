using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeDB : MonoBehaviour
{   
    public static string playTime;
    public static string kcal = "50Kcal";
    public static string totalDist = "1.2km";
    public string RecordDate = DateTime.Now.ToString("yyyy-MM-dd");

    FirebaseApp firebaseApp;
    public static DatabaseReference databaseReference;

    public class User
    {
        public string playTime;
        public string email;
        public string kcal;
        public string totalDist;

        public User()
        { }

        public User(string playTime, string email, string kcal, string totalDist)
        {
            this.playTime = playTime;
            this.email = email;
            this.kcal = kcal;
            this.totalDist = totalDist;
        }
    }

    private float startTime;

    // 초기화 밑 설정
    void Awake()
    {

        firebaseApp = FirebaseDatabase.DefaultInstance.App;
        firebaseApp.SetEditorDatabaseUrl("https://test-dc302.firebaseio.com/");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        startTime = Time.time;
    }

    //버튼 눌렀을때 동작
    public void InitDatabase(FPSController fPSController)
    {
        WriteNewUser(Login.user.UserId, playTime, Login.user.Email, kcal, totalDist);
    }
    //파이어베이스 저장 함수
    private void WriteNewUser(string uid, string playTime, string email, string kcal, string totalDist)
    {
        User user = new User(playTime, email, kcal, totalDist);
        string json = JsonUtility.ToJson(user);
        databaseReference.Child("PlayData").Child(uid).Child(RecordDate).SetRawJsonValueAsync(json);
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
