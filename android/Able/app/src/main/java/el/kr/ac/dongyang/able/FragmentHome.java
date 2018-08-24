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
    FragmentManager manager;
    String fragmentTag;
    FragmentNavigation fragmentNavigation;
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
        View view = inflater.inflate(R.layout.fragment_home, container,false);

        fragmentNavigation = new FragmentNavigation();
        naviBtn = view.findViewById(R.id.hGoNavi);
        naviBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                addFragment(fragmentNavigation);
            }
        });
        textId = view.findViewById(R.id.name);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null) {
            final String uid = user.getUid();
            userModel = new UserModel();
            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if (userModel != null) {
                        userName = userModel.getUserName();
                        textId.setText(userName);
                    }
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
        }

        //String name = SharedPref.getInstance(getContext()).getData("userName");
        //textId.setText(name);

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

    public void addFragment(Fragment fragment) {
        if (!fragment.isVisible()) {
            if (fragment instanceof FragmentNavigation) {
                ft = manager.beginTransaction()
                        .addToBackStack(null)
                        .replace(R.id.main_layout, fragment, "FRAGMENT_NAVIGATION");
                Log.d("navigationok", "okokokok");
                ft.commit();
            } else {
                ft = manager.beginTransaction()
                        .addToBackStack(null)
                        .replace(R.id.main_layout, fragment);
                ft.commit();
            }
        }
    }
}
