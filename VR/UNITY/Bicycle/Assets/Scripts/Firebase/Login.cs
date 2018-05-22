using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

    [SerializeField] string email;
    [SerializeField] string password;

    public InputField inputId;
    public InputField inputPw;
    public Text loginResult;

    FirebaseAuth auth;
    public static FirebaseUser user;

	void Awake () {
        //초기화
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
	}
	
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if(!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed In " + user.UserId);
            }
        }
    }
   
    //버튼이 눌렀을때
    public void JoinBtnOnClick()
    {
        email = inputId.text;
        password = inputPw.text;

        Debug.Log("email: " + email + ", password: " + password);

        CreateUser();
    }

    public void LoginBtnOnClick()
    {
        email = inputId.text;
        password = inputPw.text;

        Debug.Log("email: " + email + ", password: " + password);

        LoginUser();
    }

    //회원가입함수
    void CreateUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled");
                loginResult.text = "회원가입 실패";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                loginResult.text = "회원가입 실패";
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            loginResult.text = "회원가입 성공";
        });
    }

    //로그인함수
    void LoginUser()
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                loginResult.text = "로그인 실패";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                loginResult.text = "로그인 실패";
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                 newUser.DisplayName, newUser.UserId);
        });
    }
}
