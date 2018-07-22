package el.kr.ac.dongyang.able.login;

import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.FirebaseDatabase;

import java.util.Calendar;
import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.login.ActivityLogin;
import el.kr.ac.dongyang.able.login.ActivityRegister;
import el.kr.ac.dongyang.able.model.UserModel;

public class SignUp extends Fragment{


    private static final String TAG = "FragmentLogin";
    private static final int RC_SIGN_IN = 9001;

    private FirebaseAuth mAuth;


    private TextView mStatusTextView;
    private TextView mDetailTextView;

    private EditText editemailtxt;
    private EditText editpasswdtxt;
    private Button signup;

    private Button signOutButton;
    private Button disconnectButton;

    TextView register;
    android.support.v4.app.FragmentTransaction ft;
    String fragmentTag;
    private Calendar cal;

    public SignUp() {
    }
    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_login2, container, false);
        getActivity().setTitle("Login");

        mAuth = FirebaseAuth.getInstance();

        editemailtxt = view.findViewById(R.id.emailtxt);
        editpasswdtxt = view.findViewById(R.id.passwdtxt);
        signup = view.findViewById(R.id.sign_up);
        signup.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                loginUser(editemailtxt.getText().toString(), editpasswdtxt.getText().toString());
                Fragment fragment = new ActivityLogin();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });

        register = view.findViewById(R.id.register);
        register.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                    Fragment fragment = new ActivityRegister();
                    fragmentTag = fragment.getClass().getSimpleName();
                    Log.i("fragmentTag", fragmentTag);
                    getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                    ft=getActivity().getSupportFragmentManager().beginTransaction();
                    ft.replace(R.id.main_layout, fragment);
                    ft.addToBackStack(fragmentTag);
                    ft.commit();
                }
            });
        return view;
    }

    //이메일, 패스워드로 유저를 생성할 때, 지금은 로그인 버튼 클릭시 수행됨
    private void createUser(final String email, final String password) {
        mAuth.createUserWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if (task.isSuccessful()) {
                            Toast.makeText(getActivity(), "회원가입 성공", Toast.LENGTH_SHORT).show();
                        } else {
                            //로그인이 실패될 경우, 즉 이미 해당하는 똑같은 이메일 주소가 있어서 로그인이 안된다는 것.
                            //그래서 로그인 유저를 통해 로그인함. 레지스터 xml을 따로 만들면 토스트로 안되는 이유에 대한 메세지만 있으면 됨.
                            loginUser(email, password);
                        }
                    }
                });
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

                            //이메일과 uid를 받아서 데이터데이스에 저장하는데 사용.
                            String uid = task.getResult().getUser().getUid();
                            UserModel userModel = new UserModel();
                            String email = task.getResult().getUser().getEmail();
                            StringTokenizer tokens = new StringTokenizer(email);
                            userModel.userName = tokens.nextToken("@");

                            //날짜
                            String date;
                            cal = Calendar.getInstance();
                            int day = cal.get(Calendar.DAY_OF_MONTH);
                            int month = cal.get(Calendar.MONTH)+1;
                            int year = cal.get(Calendar.YEAR);
                            if(month>=10 && day<10){
                                date = year + "-" + month + "-0" + day;
                            }else if(month<10 && day>=10){
                                date = year + "-0" + month + "-" + day;
                            } else if (month>=10 && day>=10) {
                                date = year + "-" + month + "-" + day;
                            }else{
                                date = year + "-0" + month + "-0" + day;
                            }

                            //데이터베이스 저장.
                            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).child("userName").setValue(userModel.userName);
                            FirebaseDatabase.getInstance().getReference().child("HEALTH").child(uid).child(date).child("kcal").setValue("0kal");

                            //updateUI(user);
                        } else {
                            // If sign in fails, display a message to the user.
                            Log.w(TAG, "signInWithEmail:failure", task.getException());
                            //Toast.makeText(getActivity(), "Authentication failed.", Toast.LENGTH_SHORT).show();
                        }
                    }
                });
    }

    //백버튼 눌렀을때 메뉴바에 에이블이 뜨도록
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
