package el.kr.ac.dongyang.able;

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

import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by impro on 2018-05-08.
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

