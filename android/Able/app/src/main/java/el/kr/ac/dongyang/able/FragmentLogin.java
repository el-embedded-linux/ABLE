package el.kr.ac.dongyang.able;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.design.widget.Snackbar;
import android.support.v4.app.FragmentActivity;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.facebook.AccessToken;
import com.facebook.CallbackManager;
import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.login.LoginManager;
import com.facebook.login.LoginResult;
import com.facebook.login.widget.LoginButton;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.SignInButton;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthCredential;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FacebookAuthProvider;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.auth.GoogleAuthProvider;
import com.google.firebase.database.FirebaseDatabase;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by impro on 2018-03-30.
 * 이메일/비밀번호, 구글, 페이스북 로그인
 * 코드정리의 필요성이 보임
 *
 */

public class FragmentLogin extends android.support.v4.app.Fragment implements GoogleApiClient.OnConnectionFailedListener {

    private static final String TAG = "FragmentLogin";
    private static final int RC_SIGN_IN = 9001;
    //프로그레스 다이얼로그로 로그인이 느릴때 로딩이 뜨게 하려고 했는데 안뜸
    private ProgressDialog mProgressDialog;
    private GoogleSignInClient mGoogleSignInClient;

    // [START declare_auth]
    private FirebaseAuth mAuth;
    // [END declare_auth]

    //이메일과 uid를 표시. 추후 삭제.
    private TextView mStatusTextView;
    private TextView mDetailTextView;

    private LoginButton loginButton;    //페이스북
    private SignInButton signInButton;  //구글
    private Button signOutButton;
    private Button disconnectButton;
    private CallbackManager mCallbackManager;

    private EditText editTextEmail;
    private EditText editTextPassword;
    private Button emaillogin;          //이메일


    public FragmentLogin() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_login, container, false);
        getActivity().setTitle("Login");

        // Views
        mStatusTextView = view.findViewById(R.id.status);
        mDetailTextView = view.findViewById(R.id.detail);

        // Button listeners
        signInButton = (SignInButton) view.findViewById(R.id.sign_in_button);
        signInButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                signIn();
            }
        });
        signOutButton = view.findViewById(R.id.sign_out_button);
        signOutButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                signOut();
            }
        });
        disconnectButton = view.findViewById(R.id.disconnect_button);
        disconnectButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                revokeAccess();
            }
        });

        // 구글 로그인
        // [START configure_signin]
        // Configure sign-in to request the user's ID, email address, and basic
        // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
        GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
                .requestIdToken(getString(R.string.default_web_client_id))
                .requestEmail()
                .build();
        // [END configure_signin]

        mGoogleSignInClient = GoogleSignIn.getClient(getContext(), gso);

        // [START initialize_auth]
        mAuth = FirebaseAuth.getInstance();
        // [END initialize_auth]

        //페이스북 로그인
        // Initialize Facebook Login button
        mCallbackManager = CallbackManager.Factory.create();
        loginButton = (LoginButton) view.findViewById(R.id.facebook_login_button);
        loginButton.setReadPermissions("email");
        loginButton.setFragment(this);
        loginButton.registerCallback(mCallbackManager, new FacebookCallback<LoginResult>() {
            @Override
            public void onSuccess(LoginResult loginResult) {
                Log.d(TAG, "facebook:onSuccess:" + loginResult);
                handleFacebookAccessToken(loginResult.getAccessToken());
            }

            @Override
            public void onCancel() {
                Log.d(TAG, "facebook:onCancel");
                // ...
            }

            @Override
            public void onError(FacebookException error) {
                Log.d(TAG, "facebook:onError", error);
                // ...
            }
        });

        //이메일과 비밀번호로 로그인
        editTextEmail = (EditText) view.findViewById(R.id.editText_email);
        editTextPassword = (EditText) view.findViewById(R.id.editText_password);
        emaillogin = (Button) view.findViewById(R.id.email_login_button);
        emaillogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                createUser(editTextEmail.getText().toString(),editTextPassword.getText().toString());
            }
        });

        return view;
    }

    //이메일, 패스워드로 유저를 생성할때. 지금은 로그인 버튼 클릭시 수행됨.
    private void createUser(final String email, final String password) {
        mAuth.createUserWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            // Sign in success, update UI with the signed-in user's information
                            Toast.makeText(getActivity(), "회원가입 성공", Toast.LENGTH_SHORT).show();
                        } else {
                            //로그인이 실패될 경우, 즉 이미 해당하는 똑같은 이메일 주소가 있어서 로그인이 안된다는 것.
                            //그래서 로그인 유저를 통해 로그인함. 레지스터 xml을 따로 만들면 토스트로 안되는 이유에 대한 메세지만 있으면 됨.
                            loginUser(email, password);
                        }
                        // ...
                    }
                });
    }

    //이메일, 패스워드로 로그인
    private void loginUser(String email, String password){
        mAuth.signInWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            // Sign in success, update UI with the signed-in user's information
                            Log.d(TAG, "signInWithEmail:success");
                            FirebaseUser user = mAuth.getCurrentUser();

                            //이메일과 uid를 받아서 데이터베이스에 저장하는데 사용.
                            String uid = task.getResult().getUser().getUid();
                            UserModel userModel = new UserModel();
                            String email = task.getResult().getUser().getEmail();
                            StringTokenizer tokens = new StringTokenizer(email);
                            userModel.userName = tokens.nextToken("@");

                            //데이터베이스 저장.
                            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).child("userName").setValue(userModel.userName);
                            updateUI(user); //로그인 전의 ui 업데이트.
                        } else {
                            // If sign in fails, display a message to the user.
                            Log.w(TAG, "signInWithEmail:failure", task.getException());
                            Toast.makeText(getActivity(), "Authentication failed.",
                                    Toast.LENGTH_SHORT).show();
                        }
                        // ...
                    }
                });
    }

    //페이스북 로그인 기능 수행. 다른 로그인 코드와 비슷.
    private void handleFacebookAccessToken(AccessToken token) {
        Log.d(TAG, "handleFacebookAccessToken:" + token);

        AuthCredential credential = FacebookAuthProvider.getCredential(token.getToken());
        mAuth.signInWithCredential(credential)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            // Sign in success, update UI with the signed-in user's information
                            Log.d(TAG, "signInWithCredential:success");

                            FirebaseUser user = mAuth.getCurrentUser();

                            String uid = task.getResult().getUser().getUid();
                            UserModel userModel = new UserModel();
                            String email = task.getResult().getUser().getEmail();
                            StringTokenizer tokens = new StringTokenizer(email);
                            userModel.userName = tokens.nextToken("@");

                             /* 접속시간
                            Date signUpTime = new Date();
                            SimpleDateFormat date = new SimpleDateFormat("yyyyMMddHHmmss");
                            String today = date.format(signUpTime);
                            */

                            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).child("userName").setValue(userModel.userName);
                            updateUI(user);
                        } else {
                            // If sign in fails, display a message to the user.
                            Log.w(TAG, "signInWithCredential:failure", task.getException());
                            Toast.makeText(getActivity(), "Authentication Failed.", Toast.LENGTH_SHORT).show();
                            updateUI(null);
                        }
                    }
                });
    }

    //구글 로그인
    //사용자가 정상적으로 로그인한 후에 GoogleSignInAccount 개체에서 ID 토큰을 가져와서
    // Firebase 사용자 인증 정보로 교환하고 Firebase 사용자 인증 정보를 사용해 Firebase에 인증.
    // [START auth_with_google]
    private void firebaseAuthWithGoogle(GoogleSignInAccount acct) {
        Log.d(TAG, "firebaseAuthWithGoogle:" + acct.getId());
        // [START_EXCLUDE silent]
        mProgressDialog = ProgressDialog.show(getActivity(), "", "잠시만 기다려주세요",true);
        // [END_EXCLUDE]

        AuthCredential credential = GoogleAuthProvider.getCredential(acct.getIdToken(), null);
        mAuth.signInWithCredential(credential)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            // Sign in success, update UI with the signed-in user's information
                            Log.d(TAG, "signInWithCredential:success");
                            Snackbar.make(getView(), "Authentication Success.", Snackbar.LENGTH_SHORT).show();
                            FirebaseUser user = mAuth.getCurrentUser();

                            String uid = task.getResult().getUser().getUid();
                            UserModel userModel = new UserModel();
                            String email = task.getResult().getUser().getEmail();
                            StringTokenizer tokens = new StringTokenizer(email);
                            userModel.userName = tokens.nextToken("@");

                            FirebaseDatabase.getInstance().getReference().child("USER").
                                    child(uid).child("userName").setValue(userModel.userName);

                            updateUI(user);
                        } else {
                            // If sign in fails, display a message to the user.
                            Log.w(TAG, "signInWithCredential:failure", task.getException());
                            Snackbar.make(getView(), "Authentication Failed.", Snackbar.LENGTH_SHORT).show();
                            updateUI(null);
                        }
                    }
                });
        mProgressDialog.dismiss();
    }
    // [END auth_with_google]

    //시작할때 로그인 유저의 상태를 가져옴.
    @Override
    public void onStart() {
        super.onStart();
        FirebaseUser currentUser = mAuth.getCurrentUser();
        updateUI(currentUser);
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    //구글,페이스북으로부터 로그인 유저 정보 획득.
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        //페이스북쪽으로..
        mCallbackManager.onActivityResult(requestCode, resultCode, data);

        // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
        if (requestCode == RC_SIGN_IN) {
            Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
            try {
                // Google Sign In was successful, authenticate with Firebase
                GoogleSignInAccount account = task.getResult(ApiException.class);
                //파이어베이스 구글쪽으로..
                firebaseAuthWithGoogle(account);
            } catch (ApiException e) {
                // Google Sign In failed, update UI appropriately
                Log.w(TAG, "Google sign in failed", e);
                // [START_EXCLUDE]
                updateUI(null);
                // [END_EXCLUDE]
            }
        }
    }

    //버튼들 기능
    // [START signIn]
    private void signIn() {
        Intent signInIntent = mGoogleSignInClient.getSignInIntent();
        startActivityForResult(signInIntent, RC_SIGN_IN);
    }
    private void signOut() {
        // Firebase sign out
        mAuth.signOut();
        //facebook sign out
        LoginManager.getInstance().logOut();
        // Google sign out
        mGoogleSignInClient.signOut().addOnCompleteListener(getActivity(),
                new OnCompleteListener<Void>() {
                    @Override
                    public void onComplete(@NonNull Task<Void> task) {
                        updateUI(null);
                    }
                });
    }
    private void revokeAccess() {
        // Firebase sign out
        mAuth.signOut();
        // Google revoke access
        mGoogleSignInClient.revokeAccess().addOnCompleteListener(getActivity(),
                new OnCompleteListener<Void>() {
                    @Override
                    public void onComplete(@NonNull Task<Void> task) {
                        updateUI(null);
                    }
                });
    }

    //ui 변경
    private void updateUI(FirebaseUser user) {
//        mProgressDialog.cancel();
        if (user != null) {
            mStatusTextView.setText(getString(R.string.google_status_fmt, user.getEmail()));
            mDetailTextView.setText(getString(R.string.firebase_status_fmt, user.getUid()));
            editTextEmail.setVisibility(View.GONE);
            editTextPassword.setVisibility(View.GONE);
            emaillogin.setVisibility(View.GONE);
            signInButton.setVisibility(View.GONE);
            loginButton.setVisibility(View.GONE);
            signOutButton.setVisibility(View.VISIBLE);
            disconnectButton.setVisibility(View.VISIBLE);
        } else {
            mStatusTextView.setText(R.string.signed_out);
            mDetailTextView.setText(null);
            editTextEmail.setVisibility(View.VISIBLE);
            editTextPassword.setVisibility(View.VISIBLE);
            emaillogin.setVisibility(View.VISIBLE);
            signInButton.setVisibility(View.VISIBLE);
            loginButton.setVisibility(View.VISIBLE);
            signOutButton.setVisibility(View.GONE);
            disconnectButton.setVisibility(View.GONE);
        }
    }

    @Override
    public void onConnectionFailed(@NonNull ConnectionResult connectionResult) {
/*
        Toast.makeText(getApplicationContext(), ""+connectionResult, Toast.LENGTH_SHORT).show();
*/
    }

    //백버튼 눌렀을때 메뉴바에 에이블이 뜨도록
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}