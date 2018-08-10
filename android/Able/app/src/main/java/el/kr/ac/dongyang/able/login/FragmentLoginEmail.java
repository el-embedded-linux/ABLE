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
import el.kr.ac.dongyang.able.model.UserModel;

public class FragmentLoginEmail extends Fragment{

    private static final String TAG = "FragmentLogin";

    private FirebaseAuth mAuth;

    private EditText emailEditText;
    private EditText passwordEditText;
    private Button signupBtn;

    TextView registerTextView;
    android.support.v4.app.FragmentTransaction ft;
    String fragmentTag;
    private Calendar cal;

    public FragmentLoginEmail() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_login_email, container, false);
        getActivity().setTitle("Login");

        mAuth = FirebaseAuth.getInstance();

        emailEditText = view.findViewById(R.id.emailtxt);
        passwordEditText = view.findViewById(R.id.passwdtxt);

        signupBtn = view.findViewById(R.id.sign_up);
        signupBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                loginUser(emailEditText.getText().toString(), passwordEditText.getText().toString());
                Fragment fragment = new FragmentLogin();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });

        registerTextView = view.findViewById(R.id.register);
        registerTextView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                    Fragment fragment = new FragmentRegister();
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

    @Override
    public void onResume() {
        super.onResume();
        FirebaseUser user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
            fragmentManager.beginTransaction().remove(FragmentLoginEmail.this).commit();
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

                            //이메일과 uid를 받아서 데이터데이스에 저장하는데 사용.
                            String uid = task.getResult().getUser().getUid();

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
                            //FirebaseDatabase.getInstance().getReference().child("HEALTH").child(uid).child(date).child("kcal").setValue("0kal");
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
