using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimeController : MonoBehaviour {

	public Text timerText;
    public string playTime;

    FirebaseApp firebaseApp;
    DatabaseReference databaseReference;

    public class User
    {
        public string playTime;
        public string email;

        public User()
        { }

        public User(string playTime, string email)
        {
            this.playTime = playTime;
            this.email = email;
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

    void Start () {
		startTime = Time.time;
	}

    //버튼 눌렀을때 동작
    public void InitDatabase()
    {
        WriteNewUser(Login.user.UserId, playTime, Login.user.Email);
    }
    //파이어베이스 저장 함수
    private void WriteNewUser(string uid, string playTime, string email)
    {
        User user = new User(playTime, email);
        string json = JsonUtility.ToJson(user);
        databaseReference.Child("PlayData").Child(uid).SetRawJsonValueAsync(json);
    }
    // Update is called once per frame
    void Update () {
		float t = Time.time - startTime;

		string minutes = ((int)t / 60).ToString ();
		string seconds = (t % 60).ToString ("f2");
		string hours = ((int)t / 3600 % 24).ToString ();

        playTime = hours + ":" + minutes + ":" + seconds;
		timerText.text = "PlayTime " + hours + ":" + minutes + ":" + seconds;
	}
}
