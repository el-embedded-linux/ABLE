package el.kr.ac.dongyang.able.login;

import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.FragmentManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.FirebaseDatabase;

import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

public class ActivityRegister extends android.support.v4.app.Fragment{
    private static final String TAG = "SignUp";
    private  static final int RC_SIGN_IN = 9001;
    private FirebaseAuth mAuth;
    private EditText edit_emailtxt;
    private EditText edit_nametxt;
    private EditText edit_passwdtxt;
    private EditText edit_passwdchecktxt;
    private Button sign_up;

    android.support.v4.app.FragmentTransaction ft;
    String fragmentTag;

    public ActivityRegister() {
    }



    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater,@Nullable ViewGroup container, Bundle savedInstanceState) {
     View view =  inflater.inflate(R.layout.activity_register, container, false);
     getActivity().setTitle("ActivityRegister");

     mAuth = FirebaseAuth.getInstance();
     edit_emailtxt = (EditText) view.findViewById(R.id.edit_emailtxt);
     edit_nametxt = (EditText) view.findViewById(R.id.edit_nametxt);
     edit_passwdtxt = (EditText) view.findViewById(R.id.edit_passwdtxt);
     edit_passwdchecktxt = (EditText) view.findViewById(R.id.passwdchecktxt);
     sign_up = (Button)view.findViewById(R.id.sign_up);
     sign_up.setOnClickListener(new View.OnClickListener() {

         @Override
         public void onClick(View view) {
             createUser(edit_emailtxt.getText().toString(), edit_passwdtxt.getText().toString());
             FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
             fragmentManager.beginTransaction().remove(ActivityRegister.this).commit();
             fragmentManager.popBackStack();

         }
     });

     return view;
    }

    //이메일, 패스워드로 유저를 생성할 때, 로그인 버튼 클릭시 수행됨
    private void createUser(final String email, final String password) {
        mAuth.createUserWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if(task.isSuccessful()) {
                            //Toast.makeText(getActivity(), "회원가입 성공", Toast.LENGTH_SHORT).show();\

                            FirebaseUser user = mAuth.getCurrentUser();

                            //이메일과 uid를 받아서 데이터베이스에 저장하는데 사용
                            String uid = task.getResult().getUser().getUid();
                            UserModel userModel = new UserModel();
                            String email = task.getResult().getUser().getEmail();
                            StringTokenizer tokens = new StringTokenizer(email);
                            userModel.userName = tokens.nextToken("@");

                            //데이터베이스 저장
                            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).child("userName").setValue(userModel.userName);
                        } else {
                            //로그인이 실패될 경우, 즉 이미 해당하는 똑같은 이메일 주소가 있어서 로그인이 안된다는 것.
                            //그래서 로그인 유저를 통해 로그인함. 레지스터 xml을 따로 만들면 토스트로 안되는 이유에 대한 메세지만 있으면 됨.
                            //Toast.makeText(getActivity(), "등록된 회원입니다.", Toast.LENGTH_SHORT).show();
                            //loginUser(email, password);
                        }
                    }
                });
    }

    private void loginUser(String email, String password) {
        mAuth.signInWithEmailAndPassword(email, password)
                .addOnCompleteListener(getActivity(), new OnCompleteListener<AuthResult>() {
                    @Override
                    public void onComplete(@NonNull Task<AuthResult> task) {
                        if(task.isSuccessful()) {
                            Log.d(TAG, "signInWithEail : success");

                        } else {
                            Log.w(TAG, "signInWithEmail : failure", task.getException());
                            Toast.makeText(getActivity(), "Authentication failed.", Toast.LENGTH_SHORT).show();
                        }
                    }
                });
    }
}
