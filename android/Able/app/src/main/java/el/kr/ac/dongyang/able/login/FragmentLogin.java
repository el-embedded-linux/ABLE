package el.kr.ac.dongyang.able.login;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.design.widget.NavigationView;
import android.support.design.widget.Snackbar;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.RequestOptions;
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
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthCredential;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FacebookAuthProvider;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.auth.GoogleAuthProvider;
import com.google.firebase.auth.UserProfileChangeRequest;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.storage.FirebaseStorage;
import com.google.firebase.storage.UploadTask;

import java.util.HashMap;
import java.util.Map;
import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.BaseFragment;
import el.kr.ac.dongyang.able.BusProvider;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.SharedPref;
import el.kr.ac.dongyang.able.eventbus.UserEvent;
import el.kr.ac.dongyang.able.model.UserModel;

import static android.app.Activity.RESULT_OK;


public class FragmentLogin extends BaseFragment implements GoogleApiClient.OnConnectionFailedListener {


    private static final String TAG = "FagmentLogin";
    private static final int RC_SIGN_IN = 9001;
    private static final int PICK_FROM_ALBUM = 10;

    private GoogleSignInClient mGoogleSignInClient; //구글 로그인 부분
    public UserProfileChangeRequest profileChangeRequest;
    private NavigationView navigationView;
    TextView naviTitle, naviSubTitle;
    ImageView naviImg;

    // [START declare_auth]
    private FirebaseAuth mAuth;
    // [END declare_auth]

    private View emailView, registerView, loginView;
    private Button signOutButton, existLoginBtn, signupBtn, registerSignUpBtn;
    private LoginButton facebook_btn; //페이스북으로 로그인
    private SignInButton google_btn; //구글로 로그인
    private CallbackManager mcallBackManager;
    private EditText passwordEditText, emailEditText, registerEmailEditText, userNameEditText, registerPwEditText, registerPwCheckEditText;
    private CheckBox checkBox;
    private ImageView profile;
    private Uri imageUri;

    android.support.v4.app.FragmentTransaction ft;
    private String userName;
    private UserModel userModel;
    private int registerNum = 0;

    public FragmentLogin() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_login_total, container, false);
        getActivity().setTitle("Login");

        final String emailPattern = "[a-zA-Z0-9._-]+@[a-z]+\\.+[a-z]+";

        navigationView = getActivity().findViewById(R.id.nav_view);
        View headerView = navigationView.getHeaderView(0);
        naviTitle = headerView.findViewById(R.id.NaviHeaderMainTextViewTitle);
        naviSubTitle = headerView.findViewById(R.id.NaviHeaderMainTextViewSubTitle);
        naviImg = headerView.findViewById(R.id.NaviHeaderMainImageView);

        //뷰
        emailView = view.findViewById(R.id.loginEmailXml);
        emailView.setVisibility(View.GONE);
        registerView = view.findViewById(R.id.registerXml);
        registerView.setVisibility(View.GONE);
        loginView = view.findViewById(R.id.loginXml);

        google_btn = view.findViewById(R.id.google_btn);
        facebook_btn = view.findViewById(R.id.facebook_btn);

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

        //구글 로그인
        // [START configure_signin]
        // Configure sign-in to request the user's ID, email destinationAddress, and basic
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
        mcallBackManager = CallbackManager.Factory.create();
        facebook_btn.setReadPermissions("email");
        facebook_btn.setFragment(this);
        facebook_btn.registerCallback(mcallBackManager, new FacebookCallback<LoginResult>() {
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

        existLoginBtn = view.findViewById(R.id.exist_login_btn);
        existLoginBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                loginView.setVisibility(View.GONE);
                emailView.setVisibility(View.VISIBLE);
            }
        });

        //로그인이메일 xml
        emailEditText = emailView.findViewById(R.id.emailtxt);
        passwordEditText = emailView.findViewById(R.id.passwdtxt);

        signupBtn = emailView.findViewById(R.id.sign_up);
        signupBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                loginUser(emailEditText.getText().toString(), passwordEditText.getText().toString());
            }
        });

        TextView registerTextView = view.findViewById(R.id.register);
        registerTextView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                getActivity().setTitle("Register");
                emailView.setVisibility(View.GONE);
                registerView.setVisibility(View.VISIBLE);
            }
        });

        //레지스터 xml
        registerEmailEditText = registerView.findViewById(R.id.edit_emailtxt);
        userNameEditText = registerView.findViewById(R.id.edit_nametxt);
        registerPwEditText = registerView.findViewById(R.id.edit_passwdtxt);
        registerPwCheckEditText = registerView.findViewById(R.id.passwdchecktxt);
        checkBox = registerView.findViewById(R.id.check1);

        profile = registerView.findViewById(R.id.fragment_register_imageview_profile);
        profile.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(Intent.ACTION_PICK);
                intent.setType(MediaStore.Images.Media.CONTENT_TYPE);
                startActivityForResult(intent, PICK_FROM_ALBUM);
            }
        });

        registerSignUpBtn = registerView.findViewById(R.id.sign_up);
        registerSignUpBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (registerEmailEditText.getText().toString() == null || userNameEditText.getText().toString() == null || registerPwEditText.getText().toString() == null || registerPwCheckEditText.getText().toString() == null || imageUri == null) {
                    Toast.makeText(getContext(), "모두 입력해주세요.", Toast.LENGTH_SHORT).show();
                } else if (!registerPwEditText.getText().toString().equals(registerPwCheckEditText.getText().toString())) {
                    Toast.makeText(getActivity(), "비밀번호가 일치하지 않습니다.", Toast.LENGTH_SHORT).show();
                } else if (registerPwEditText.getText().toString().trim().length() < 6) {
                    Toast.makeText(getActivity(), "비밀번호는 최소 6자리 이상이어야 합니다.", Toast.LENGTH_SHORT).show();
                } else if (!registerEmailEditText.getText().toString().trim().matches(emailPattern)) {
                    Toast.makeText(getActivity(), "이메일이 같지 않습니다.", Toast.LENGTH_SHORT).show();
                } else if (!checkBox.isChecked()) {
                    Toast.makeText(getActivity(), "개인정보 약관에 동의하여 주십시오.", Toast.LENGTH_SHORT).show();
                } else {
                    createUser(registerEmailEditText.getText().toString(), registerPwEditText.getText().toString());
                }
            }
        });

        return view;
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
                    userModel.setUserName(tokens.nextToken("@"));
                    profileChangeRequest = new UserProfileChangeRequest.Builder().setDisplayName(tokens.nextToken("@")).build();
                    task.getResult().getUser().updateProfile(profileChangeRequest);
                    reference.child("USER").child(uid).child("userName").setValue(userModel.getUserName());
                    reference.child("USER").child(uid).child("uid").setValue(uid);
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
                    Snackbar.make(getView(), "로그인 성공", Snackbar.LENGTH_SHORT).show();
                    FirebaseUser user = mAuth.getCurrentUser();
                    String uid = task.getResult().getUser().getUid();
                    UserModel userModel = new UserModel();
                    String email = task.getResult().getUser().getEmail();
                    StringTokenizer tokens = new StringTokenizer(email);
                    userModel.setUserName(tokens.nextToken("@"));
                    profileChangeRequest = new UserProfileChangeRequest.Builder().setDisplayName(tokens.nextToken("@")).build();
                    task.getResult().getUser().updateProfile(profileChangeRequest);
                    reference.child("USER").child(uid).child("userName").setValue(userModel.getUserName());
                    reference.child("USER").child(uid).child("uid").setValue(uid);
                    updateUI(user);
                } else {
                    // If sign in fails, display a message to the user.
                    Log.w(TAG, "signInWithCredential:failure", task.getException());
                    Snackbar.make(getView(), "로그인 실패", Snackbar.LENGTH_SHORT).show();
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
        if (registerNum == 1) {
            loginView.setVisibility(View.GONE);
            emailView.setVisibility(View.GONE);
            registerView.setVisibility(View.VISIBLE);
            registerNum = 0;
        } else {
            FirebaseUser currentUser = mAuth.getCurrentUser();
            updateUI(currentUser);
        }
    }

    //구글,페이스북으로부터 로그인 유저 정보 획득.
    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        //페이스북쪽으로..
        mcallBackManager.onActivityResult(requestCode, resultCode, data);
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
        } else if (requestCode == PICK_FROM_ALBUM && resultCode == RESULT_OK) {
            profile.setImageURI(data.getData());    //가운데 뷰를 바꿈
            imageUri = data.getData();  // 이미지 경로 원본
            registerNum = 1;
        }
    }

    private void passPushTokenToServer() {
        FirebaseUser user = FirebaseAuth.getInstance().getCurrentUser();
        if (user != null) {
            String uid = firebaseUser.getUid();
            String token = FirebaseInstanceId.getInstance().getToken();
            Map<String, Object> map = new HashMap<>();
            map.put("pushToken", token);

            reference.child("USER").child(uid).updateChildren(map);
        }
    }

    //버튼들 기능
    // [START signIn] 구글 버튼
    //구글 로그인이 필요한 시점에 아래에 같은 메소드를 실행
    private void signIn() {
        Intent signInIntent = mGoogleSignInClient.getSignInIntent();
        startActivityForResult(signInIntent, RC_SIGN_IN);
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

    @Override
    public void onResume() {
        super.onResume();
    }

    //ui 변경
    private void updateUI(FirebaseUser user) {
        if (user != null) {

            google_btn.setVisibility(View.GONE);
            facebook_btn.setVisibility(View.GONE);
            existLoginBtn.setVisibility(View.GONE);
            signOutButton.setVisibility(View.VISIBLE);
            emailView.setVisibility(View.GONE);
            registerView.setVisibility(View.GONE);
            loginView.setVisibility(View.VISIBLE);

            String uid = user.getUid();
            reference.child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if (userModel != null) {
                        userName = userModel.getUserName();
                        if (userModel.getProfileImageUrl() != null) {
                            Glide.with(getActivity())
                                    .load(userModel.getProfileImageUrl())
                                    .apply(new RequestOptions().circleCrop())
                                    .into(naviImg);
                        }
                        naviTitle.setText(userModel.getUserName());
                        naviSubTitle.setText(userModel.getComment());
                    }
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });

        } else {
            existLoginBtn.setVisibility(View.VISIBLE);
            google_btn.setVisibility(View.VISIBLE);
            facebook_btn.setVisibility(View.VISIBLE);
            signOutButton.setVisibility(View.GONE);
            emailView.setVisibility(View.GONE);
            registerView.setVisibility(View.GONE);
            loginView.setVisibility(View.VISIBLE);
            userName = "회원";
            naviImg.setImageResource(R.drawable.playstore_icon);
            naviTitle.setText(R.string.naviTitle);
            naviSubTitle.setText(R.string.naviSubTitle);
        }
    }

    //이메일, 패스워드로 로그인
    private void loginUser(String email, String password) {
        mAuth.signInWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            Log.d(TAG, "signInWithEmail:success");
                            FirebaseUser user = mAuth.getCurrentUser();
                            updateUI(user);

                        } else {
                            // If sign in fails, display a message to the user.
                            Log.w(TAG, "signInWithEmail:failure", task.getException());
                            //Toast.makeText(getActivity(), "Authentication failed.", Toast.LENGTH_SHORT).show();
                        }
                    }
                });
    }

    //이메일, 패스워드로 유저를 생성할 때, 로그인 버튼 클릭시 수행됨
    private void createUser(final String email, final String password) {
        mAuth.createUserWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            //Toast.makeText(getActivity(), "회원가입 성공", Toast.LENGTH_SHORT).show();
                            final String uid = task.getResult().getUser().getUid();

                            profileChangeRequest = new UserProfileChangeRequest.Builder().setDisplayName(userNameEditText.getText().toString()).build();
                            task.getResult().getUser().updateProfile(profileChangeRequest);

                            //이메일과 uid를 받아서 데이터베이스에 저장하는데 사용
                            final UserModel userModel = new UserModel();
                            userModel.setEmail(registerEmailEditText.getText().toString());
                            userModel.setUserName(userNameEditText.getText().toString());
                            userModel.setUid(FirebaseAuth.getInstance().getCurrentUser().getUid());
                            userModel.setPassword(registerPwEditText.getText().toString());
                            reference.child("USER").child(uid).setValue(userModel);

                            passPushTokenToServer();
                            SharedPref.getInstance(getContext()).setData("userName", userModel.getUserName());

                            FirebaseStorage.getInstance().getReference().child("userImages").child(uid).putFile(imageUri).addOnCompleteListener(new OnCompleteListener<UploadTask.TaskSnapshot>() {
                                @Override
                                public void onComplete(@NonNull Task<UploadTask.TaskSnapshot> task) {
                                    final String imagePath = task.getResult().getStorage().getPath();
                                    FirebaseStorage.getInstance().getReference().child(imagePath).getDownloadUrl().addOnSuccessListener(new OnSuccessListener<Uri>() {
                                        @Override
                                        public void onSuccess(Uri uri) {
                                            //데이터베이스 저장
                                            reference.child("USER").child(uid).child("profileImageUrl").setValue(uri.toString());
                                            Toast.makeText(getActivity(), "회원가입되었습니다.", Toast.LENGTH_SHORT).show();
                                            FirebaseUser user = mAuth.getCurrentUser();
                                            updateUI(user);
                                        }
                                    });
                                }
                            });
                        }
                    }
                }).addOnFailureListener(new OnFailureListener() {
            @Override
            public void onFailure(@NonNull Exception e) {
                e.printStackTrace();
            }
        });
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
        BusProvider.getInstance().post(new UserEvent(userName));
        //FragmentHome fragmentHome = new FragmentHome();
        //fragmentHome.onResume();
    }
}
