package el.kr.ac.dongyang.able.setting;

import android.os.Bundle;
import android.os.UserManager;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by impro on 2018-05-08.
 * 설정 - 내 정보 수정 에서 데이터 저장 가능.
 * 주소를 넣을때 큰 범위에선 특정 위치명만 선택할 수 있도록 바꿀까 고민중.
 * 이미지 저장 - 스토리지,디비 아직 미구현
 */

public class FragmentInformation extends Fragment{

    public FragmentInformation() {
    }

    Button infoSave;
    EditText mName, mAddress, mHeight, mWeight;
    FirebaseUser user;
    private DatabaseReference mDatabase;
    UserModel userModel;
    String uid;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_userinfo,container,false);
        getActivity().setTitle("내 정보 수정");

        mName = (EditText)view.findViewById(R.id.editTextName);
        mAddress = (EditText)view.findViewById(R.id.editTextAddr);
        mHeight = (EditText)view.findViewById(R.id.editTextHeight);
        mWeight = (EditText)view.findViewById(R.id.editTextWeight);
        infoSave = (Button)view.findViewById(R.id.info_save);
        mDatabase = FirebaseDatabase.getInstance().getReference();
        user = FirebaseAuth.getInstance().getCurrentUser();

        uid = user.getUid();
        userModel = new UserModel();

        //user가 있으면 기존에 저장된 값을 호출함.
        if (user != null) {
            // User is signed in
            mDatabase.child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if(userModel != null) {
                        mName.setText(userModel.userName);
                        mAddress.setText(userModel.address);
                        mHeight.setText(userModel.height);
                        mWeight.setText(userModel.weight);
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
                userModel.userName = mName.getText().toString();
                userModel.address = mAddress.getText().toString();
                userModel.height = mHeight.getText().toString();
                userModel.weight = mWeight.getText().toString();
                mDatabase.child("USER").child(uid).child("userName").setValue(userModel.userName);
                mDatabase.child("USER").child(uid).child("address").setValue(userModel.address);
                mDatabase.child("USER").child(uid).child("height").setValue(userModel.height);
                mDatabase.child("USER").child(uid).child("weight").setValue(userModel.weight);
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
}

