package el.kr.ac.dongyang.able;

import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.FirebaseApiNotAvailableException;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.ChildEventListener;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import el.kr.ac.dongyang.able.model.UserModel;
import el.kr.ac.dongyang.able.navigation.FragmentNavigation;

/**
 * Created by impro on 2018-03-30.
 */

public class FragmentHome extends android.support.v4.app.Fragment{

    Button naviBtn;
    TextView textId;
    FirebaseUser firebaseUser;
    UserModel userModel;
    FragmentTransaction ft;
    String fragmentTag;
    Fragment fragmentNav;
    String uid;

    public FragmentHome() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_home,container,false);

        fragmentNav = new FragmentNavigation();
        naviBtn = view.findViewById(R.id.hGoNavi);
        naviBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                fragmentTag = fragmentNav.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.add(R.id.main_layout, fragmentNav);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });
        textId = view.findViewById(R.id.name);
        userModel = new UserModel();

        return view;
    }

    @Override
    public void onResume() {
        super.onResume();
        Log.d("resume", "resume ok");
        firebaseUser = FirebaseAuth.getInstance().getCurrentUser();
        if(firebaseUser != null){
            uid = firebaseUser.getUid();
            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if(userModel != null) {
                        textId.setText(userModel.userName);
                    }
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
        } else {
            textId.setText("회원");
        }
    }
}
