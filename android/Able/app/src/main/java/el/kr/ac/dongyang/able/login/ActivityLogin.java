package el.kr.ac.dongyang.able.login;

import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.design.widget.Snackbar;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
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

import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;


public class ActivityLogin extends Fragment implements GoogleApiClient.OnConnectionFailedListener {


    private static final String TAG = "FagmentLogin";
    private static final int RC_SIGN_IN = 9001;

    private GoogleSignInClient mGoogleSignInClient; //구글 로그인 부분

    // [START declare_auth]
    private FirebaseAuth mAuth;
    // [END declare_auth]


    //이메일과 uid를 표시, 추후 삭제
    private TextView mStatusTextView;
    private TextView mDetailTextView;

    //private Button emaillogin;

    private Button signOutButton;
    private Button disconnectButton;

    //private EditText editTextEmail;
    //private EditText editTextPassword;

    private LoginButton facebook_btn; //페이스북으로 로그인
    private SignInButton google_btn; //구글로 로그인
    private CallbackManager mcallbackManager;

    private Button exist;
    android.support.v4.app.FragmentTransaction ft;
    String fragmentTag;

    public ActivityLogin() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_login, container, false);
        getActivity().setTitle("Login");

        /*mStatusTextView = view.findViewById(R.id.status);
        mDetailTextView = view.findViewById(R.id.detail);*/


        //google_btn = signInButton
        google_btn = (SignInButton) view.findViewById(R.id.google_btn);
        google_btn.setOnClickListener(new View.OnClickListener() {

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

        /*disconnectButton = view.findViewById(R.id.disconnect_button);
        disconnectButton.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                revokeAccess();
            }
        });*/

        //구글 로그인
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
        mcallbackManager = CallbackManager.Factory.create();
        facebook_btn = (LoginButton) view.findViewById(R.id.facebook_btn);
        facebook_btn.setReadPermissions("email");
        facebook_btn.setFragment(this);
        facebook_btn.registerCallback(mcallbackManager, new FacebookCallback<LoginResult>() {
            @Override
            public void onSuccess(LoginResult loginResult) {
                Log.d(TAG, "facebook:onSuccess:" + loginResult);
                handleFacebookAccessToken(loginResult.getAccessToken());
            }

            @Override
            public void onCancel() {
                Log.d(TAG, "facebook:onCancel");
            }

            @Override
            public void onError(FacebookException error) {
                Log.d(TAG, "facebook:onError", error);

            }
        });

        exist = (Button) view.findViewById(R.id.exist_btn);
        exist.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Fragment fragment = new SignUp();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fragmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });

        return view;
    }

    private void signOut() {
        //Firebase sign out
        mAuth.signOut();

        //facebook sign out
        LoginManager.getInstance().logOut();

        //Google sign out
        mGoogleSignInClient.signOut().addOnCompleteListener(getActivity(), new OnCompleteListener<Void>() {
            @Override
            public void onComplete(@NonNull Task<Void> task) {
                updateUI(null);
            }
        });
    }

    //페이스북 로그인 기능 수행, 다른 로그인 코드와 비슷
    private void handleFacebookAccessToken(AccessToken token) {
        Log.d(TAG, "handleFacebookAccessToken:" + token);

        AuthCredential credential = FacebookAuthProvider.getCredential(token.getToken());
        mAuth.signInWithCredential(credential).addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
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

                    FirebaseDatabase.getInstance().getReference().child("USER").child(uid).child("userName").setValue(userModel.userName);
                    updateUI(user);
                } else {
                    // If sign in fails, display a message to the user.
                    Log.w(TAG, "signInWithCredential:failure", task.getException());
                    Toast.makeText(getActivity(), "Authentication Failed.", Toast.LENGTH_SHORT).show();
                    //updateUI(null);
                }
            }
        });
    }

    //구글 로그인
    //사용자가 정상적으로 로그인한 후에 GoogleSignInAccount 개체에서 ID 토큰을 가져와서
    // Firebase 사용자 인증 정보로 교환하고 Firebase 사용자 인증 정보를 사용해 Firebase에 인증.
    // [START auth_with_google]
    private void firebaseAuthWithGoogle(GoogleSignInAccount acct) {
        Log.d(TAG, "frebaseAuthWithGoogle:" + acct.getId());

        AuthCredential credential = GoogleAuthProvider.getCredential(acct.getIdToken(), null);
        mAuth.signInWithCredential(credential).addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
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
                    //updateUI(null);
                }
            }
        });
    }
    // [END auth_with_google]


    //시작할때 로그인 유저의 상태를 가져옴.
    @Override
    public void onStart() {
        super.onStart();
        FirebaseUser currentUser = mAuth.getCurrentUser();
        updateUI(currentUser);
    }

    //구글,페이스북으로부터 로그인 유저 정보 획득.
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        //페이스북쪽으로..
        mcallbackManager.onActivityResult(requestCode, resultCode, data);

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
                //updateUI(null);
                // [END_EXCLUDE]
            }
        }
    }

    //버튼들 기능
    // [START signIn] 구글 버튼
    //구글 로그인이 필요한 시점에 아래에 같은 메소드를 실행
    private void signIn() {
        Intent signInIntent = mGoogleSignInClient.getSignInIntent();
        startActivityForResult(signInIntent, RC_SIGN_IN);
    }


    private void revokeAccess() {
        // Firebase sign out
        mAuth.signOut();

        // Google revoke access
        mGoogleSignInClient.revokeAccess().addOnCompleteListener(getActivity(),
                new OnCompleteListener<Void>() {
                    @Override
                    public void onComplete(@NonNull Task<Void> task) {
                        //updateUI(null);
                    }
                });
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    //ui 변경
    private void updateUI(FirebaseUser user) {
//        mProgressDialog.cancel();
        if (user != null) {
            //mStatusTextView.setText(getString(R.string.google_status_fmt, user.getEmail()));
            //mDetailTextView.setText(getString(R.string.firebase_status_fmt, user.getUid()));
            //editTextEmail.setVisibility(View.GONE);
            //editTextPassword.setVisibility(View.GONE);
            //emaillogin.setVisibility(View.GONE);
            google_btn.setVisibility(View.GONE);
            facebook_btn.setVisibility(View.GONE);
            exist.setVisibility(View.GONE);
            signOutButton.setVisibility(View.VISIBLE);
            //disconnectButton.setVisibility(View.VISIBLE);
        } else {
            //mStatusTextView.setText(R.string.signed_out);
            //mDetailTextView.setText(null);
            //editTextEmail.setVisibility(View.VISIBLE);
            //editTextPassword.setVisibility(View.VISIBLE);
            //emaillogin.setVisibility(View.VISIBLE);
            exist.setVisibility(View.VISIBLE);
            google_btn.setVisibility(View.VISIBLE);
            facebook_btn.setVisibility(View.VISIBLE);
            signOutButton.setVisibility(View.GONE);
            //ㅗdisconnectButton.setVisibility(View.GONE);
        }
    }

    @Override
    public void onConnectionFailed(@NonNull ConnectionResult connectionResult) {
        //Toast.makeText(getApplicationContext(), ""+connectionResult, Toast.LENGTH_SHORT).show();
    }

    //백버튼 눌렀을때 메뉴바에 에이블이 뜨도록
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
