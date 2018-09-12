package el.kr.ac.dongyang.able.setting;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.RequestOptions;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.ValueEventListener;
import com.google.firebase.storage.FirebaseStorage;
import com.google.firebase.storage.UploadTask;

import el.kr.ac.dongyang.able.BaseFragment;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.SharedPref;
import el.kr.ac.dongyang.able.model.UserModel;

import static android.app.Activity.RESULT_OK;

/**
 * Created by impro on 2018-05-08.
 * 설정 - 내 정보 수정 에서 데이터 저장 가능.
 * 주소를 넣을때 큰 범위에선 특정 위치명만 선택할 수 있도록 바꿀까 고민중.
 * 이미지 저장 - 스토리지,디비 아직 미구현
 */

public class FragmentInformation extends BaseFragment{

    private static final int PICK_FROM_ALBUM = 10;
    private Uri imageUri;

    public FragmentInformation() {
    }

    Button infoSave;
    EditText mName, mAddress, mHeight, mWeight, mComment, mGoal;
    FirebaseUser user;

    UserModel userModel;
    String uid;
    ImageView infoImg;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_userinfo,container,false);
        getActivity().setTitle("내 정보 수정");

        infoImg = view.findViewById(R.id.infoImg);
        infoImg.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(Intent.ACTION_PICK);
                intent.setType(MediaStore.Images.Media.CONTENT_TYPE);
                startActivityForResult(intent, PICK_FROM_ALBUM);
            }
        });

        mName = view.findViewById(R.id.editTextName);
        mAddress = view.findViewById(R.id.editTextAddr);
        mHeight = view.findViewById(R.id.editTextHeight);
        mWeight = view.findViewById(R.id.editTextWeight);
        mComment = view.findViewById(R.id.editTextComment);
        mGoal = view.findViewById(R.id.editTextGoal);
        infoSave = view.findViewById(R.id.info_save);
        user = FirebaseAuth.getInstance().getCurrentUser();

        if(user != null){
            uid = user.getUid();
        }
        userModel = new UserModel();

        //user가 있으면 기존에 저장된 값을 호출함.
        if (user != null) {
            // User is signed in
            reference.child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {

                    userModel = dataSnapshot.getValue(UserModel.class);
                    if(userModel != null) {
                        if(userModel.getProfileImageUrl() != null) {
                            Glide.with(getActivity())
                                    .load(userModel.getProfileImageUrl())
                                    .apply(new RequestOptions().circleCrop())
                                    .into(infoImg);
                        }
                        mName.setText(userModel.getUserName());
                        mAddress.setText(userModel.getAddress());
                        mHeight.setText(userModel.getHeight());
                        mWeight.setText(userModel.getWeight());
                        mComment.setText(userModel.getComment());
                        mGoal.setText(userModel.getGoal());
                    }
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {

                }
            });
        } else {
            // No user is signed in
        }

        //저장버튼 클릭시 userModel에 각 텍스트를 넣고, 디비에 저장.
        infoSave.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(imageUri != null){
                    FirebaseStorage.getInstance().getReference()
                            .child("userImages")
                            .child(uid)
                            .putFile(imageUri)
                            .addOnCompleteListener(new OnCompleteListener<UploadTask.TaskSnapshot>() {
                        @Override
                        public void onComplete(@NonNull Task<UploadTask.TaskSnapshot> task) {
                            final String imagePath = task.getResult().getStorage().getPath();
                            FirebaseStorage.getInstance().getReference()
                                    .child(imagePath)
                                    .getDownloadUrl()
                                    .addOnSuccessListener(new OnSuccessListener<Uri>() {
                                @Override
                                public void onSuccess(Uri uri) {
                                    //데이터베이스 저장
                                    reference.child("USER").child(uid).child("profileImageUrl").setValue(uri.toString());
                                }
                            });
                        }
                    });
                }
                if(mName.length() != 0) {
                    userModel.setUserName(mName.getText().toString());
                    reference.child("USER").child(uid).child("userName").setValue(userModel.getUserName());
                    SharedPref.getInstance(getContext()).setData("userName", userModel.getUserName());
                }
                if(mAddress.length() != 0) {
                    userModel.setAddress(mAddress.getText().toString());
                    reference.child("USER").child(uid).child("destinationAddress").setValue(userModel.getAddress());
                }
                if(mHeight.length() != 0) {
                    userModel.setHeight(mHeight.getText().toString());
                    reference.child("USER").child(uid).child("height").setValue(userModel.getHeight());
                }
                if(mWeight.length() != 0) {
                    userModel.setWeight(mWeight.getText().toString());
                    reference.child("USER").child(uid).child("weight").setValue(userModel.getWeight());
                }
                if(mComment.length() != 0) {
                    userModel.setComment(mComment.getText().toString());
                    reference.child("USER").child(uid).child("comment").setValue(userModel.getComment());
                }
                if(mGoal.length() != 0) {
                    userModel.setGoal(mGoal.getText().toString());
                    reference.child("USER").child(uid).child("goal").setValue(userModel.getGoal());
                }
                getActivity().onBackPressed();
            }
        });

        return view;
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == PICK_FROM_ALBUM && resultCode == RESULT_OK) {
            infoImg.setImageURI(data.getData());    //가운데 뷰를 바꿈
            imageUri = data.getData();  // 이미지 경로 원본
        }
    }
}

