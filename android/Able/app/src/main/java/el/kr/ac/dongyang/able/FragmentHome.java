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
import com.squareup.otto.Subscribe;

import el.kr.ac.dongyang.able.db.Userdb;
import el.kr.ac.dongyang.able.eventbus.UserEvent;
import el.kr.ac.dongyang.able.model.UserModel;
import el.kr.ac.dongyang.able.navigation.FragmentNavigation;

/**
 * Created by impro on 2018-03-30.
 */

public class FragmentHome extends android.support.v4.app.Fragment{

    Button naviBtn;
    TextView textId;
    FragmentTransaction ft;
    String fragmentTag;
    Fragment fragmentNav;
    FirebaseUser user;
    private UserModel userModel;
    private String userName;

    public FragmentHome() {
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        BusProvider.getInstance().register(this);
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

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null) {
            String uid = user.getUid();
            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    //왜 userModel 이 null이지
                    if (userModel != null) {
                        userName = userModel.getUserName();
                    }
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
            textId.setText(userName);
        }

        return view;
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    @Override
    public void onDestroy() {
        BusProvider.getInstance().unregister(this);
        super.onDestroy();
    }

    @Subscribe
    public void finishLoad(UserEvent userEvent){
        textId.setText(userEvent.getUserId());
        Log.d("finishLoad", userEvent.getUserId());
    }
}
