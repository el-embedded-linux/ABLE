package el.kr.ac.dongyang.able.login;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.FragmentManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.auth.UserProfileChangeRequest;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.storage.FirebaseStorage;
import com.google.firebase.storage.UploadTask;

import java.util.HashMap;
import java.util.Map;
import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

import static android.app.Activity.RESULT_OK;

public class FragmentRegister extends android.support.v4.app.Fragment {
    private static final int PICK_FROM_ALBUM = 10;
    private static final String TAG = "FragmentLoginEmail";
    private FirebaseAuth mAuth;
    private EditText emailEditText, userNameEditText, passwordEditText, passwdcheckEditText;
    private Button signUpBtn;
    private ImageView profile;
    private Uri imageUri;

    android.support.v4.app.FragmentTransaction ft;
    String fragmentTag;

    public FragmentRegister() {
    }


    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_register, container, false);
        getActivity().setTitle("FragmentRegister");

        final String emailPattern = "[a-zA-Z0-9._-]+@[a-z]+\\.+[a-z]+";

        mAuth = FirebaseAuth.getInstance();
        emailEditText = view.findViewById(R.id.edit_emailtxt);
        userNameEditText = view.findViewById(R.id.edit_nametxt);
        passwordEditText = view.findViewById(R.id.edit_passwdtxt);
        passwdcheckEditText = view.findViewById(R.id.passwdchecktxt);

        profile = view.findViewById(R.id.fragment_register_imageview_profile);
        profile.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(Intent.ACTION_PICK);
                intent.setType(MediaStore.Images.Media.CONTENT_TYPE);
                startActivityForResult(intent, PICK_FROM_ALBUM);
            }
        });

        signUpBtn = view.findViewById(R.id.sign_up);
        signUpBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (emailEditText.getText().toString() == null || userNameEditText.getText().toString() == null || passwordEditText.getText().toString() == null || passwdcheckEditText.getText().toString() == null || imageUri == null) {
                    Toast.makeText(getContext(), "모두 입력해주세요.", Toast.LENGTH_SHORT).show();
                    return;
                } else if (!passwordEditText.getText().toString().equals(passwdcheckEditText.getText().toString())) {
                    Toast.makeText(getActivity(), "비밀번호가 일치하지 않습니다.", Toast.LENGTH_SHORT).show();
                    return;
                } else if (passwordEditText.getText().toString().trim().length() < 6) {
                    Toast.makeText(getActivity(), "비밀번호는 최소 6자리 이상이어야 합니다.", Toast.LENGTH_SHORT).show();
                    return;
                } else if (!emailEditText.getText().toString().trim().matches(emailPattern)) {
                    Toast.makeText(getActivity(), "이메일이 같지 않습니다.", Toast.LENGTH_SHORT).show();
                    return;
                } else {
                    createUser(emailEditText.getText().toString(), passwordEditText.getText().toString());
                    FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
                    fragmentManager.beginTransaction().remove(FragmentRegister.this).commit();
                    fragmentManager.popBackStack();
                }
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
                        if (task.isSuccessful()) {
                            //Toast.makeText(getActivity(), "회원가입 성공", Toast.LENGTH_SHORT).show();
                            final String uid = task.getResult().getUser().getUid();

                            UserProfileChangeRequest userProfileChangeRequest = new UserProfileChangeRequest.Builder().setDisplayName(userNameEditText.getText().toString()).build();
                            task.getResult().getUser().updateProfile(userProfileChangeRequest);

                            //이메일과 uid를 받아서 데이터베이스에 저장하는데 사용
                            final UserModel userModel = new UserModel();
                            userModel.email = emailEditText.getText().toString();
                            userModel.userName = userNameEditText.getText().toString();
                            userModel.uid = FirebaseAuth.getInstance().getCurrentUser().getUid();
                            userModel.password = passwordEditText.getText().toString();
                            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).setValue(userModel);

                            passPushTokenToServer();

                            FirebaseStorage.getInstance().getReference().child("userImages").child(uid).putFile(imageUri).addOnCompleteListener(new OnCompleteListener<UploadTask.TaskSnapshot>() {
                                @Override
                                public void onComplete(@NonNull Task<UploadTask.TaskSnapshot> task) {
                                    final String imagePath = task.getResult().getStorage().getPath();
                                    FirebaseStorage.getInstance().getReference().child(imagePath).getDownloadUrl().addOnSuccessListener(new OnSuccessListener<Uri>() {
                                        @Override
                                        public void onSuccess(Uri uri) {
                                            //데이터베이스 저장
                                            FirebaseDatabase.getInstance().getReference()
                                                    .child("USER").child(uid).child("profileImageUrl").setValue(uri.toString());
                                            Toast.makeText(getActivity(), "회원가입되었습니다.", Toast.LENGTH_SHORT).show();
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
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == PICK_FROM_ALBUM && resultCode == RESULT_OK) {
            profile.setImageURI(data.getData());    //가운데 뷰를 바꿈
            imageUri = data.getData();  // 이미지 경로 원본

        }
    }

    private void passPushTokenToServer() {
        String uid = FirebaseAuth.getInstance().getCurrentUser().getUid();
        String token = FirebaseInstanceId.getInstance().getToken();
        Map<String, Object> map = new HashMap<>();
        map.put("pushToken", token);

        FirebaseDatabase.getInstance().getReference().child("USER").child(uid).updateChildren(map);
    }
}
