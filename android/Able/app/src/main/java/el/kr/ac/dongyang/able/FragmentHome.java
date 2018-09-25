package el.kr.ac.dongyang.able;

import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.FragmentManager;
import android.text.Html;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import com.squareup.otto.Subscribe;

import el.kr.ac.dongyang.able.eventbus.UserEvent;
import el.kr.ac.dongyang.able.model.UserModel;
import el.kr.ac.dongyang.able.navigation.NavigationActivity;

/**
 * Created by impro on 2018-03-30.
 */

public class FragmentHome extends BaseFragment{

    Button naviBtn;
    TextView textId;
    FragmentManager manager;
    FirebaseUser user;
    private UserModel userModel;
    private String userName;
    String uid;

    TextView weatherIcon, temperature, temp_max, temp_min;
    WeatherIconManager weatherIconManager;

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

        manager = getActivity().getSupportFragmentManager();
        naviBtn = view.findViewById(R.id.hGoNavi);
        naviBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(getActivity(), NavigationActivity.class);
                intent.putExtra("clickBtn", "start");
                startActivity(intent);
            }
        });
        textId = view.findViewById(R.id.name);

        weatherIcon = view.findViewById(R.id.weather);
        temperature = view.findViewById(R.id.Temperature);
        temp_max = view.findViewById(R.id.Temp_max);
        temp_min = view.findViewById(R.id.Temp_min);
        weatherIconManager = new WeatherIconManager();
        setWeather();

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null) {
            uid = user.getUid();
            userModel = new UserModel();
            progressOn();
            FirebaseDatabase.getInstance().getReference().child("USER").child(uid).addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if (userModel != null) {
                        userName = userModel.getUserName();
                        textId.setText(userName);
                        progressOff();
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

    private void setWeather() {
        progressOn();
        String font = "fonts/weathericons-regular-webfont.ttf";
        weatherIcon.setTypeface(weatherIconManager.get_icons(font, getActivity()));

        final WeatherFunction.placeIdTask asyncTask = new WeatherFunction.placeIdTask(new WeatherFunction.AsyncResponse() {
            public void processFinish(String temperature, String temp_max, String temp_min, String updatedOn, String iconText, String sun_rise) {
                weatherIcon.setText(Html.fromHtml(iconText));
                FragmentHome.this.temperature.setText(temperature);
                FragmentHome.this.temp_max.setText(temp_max);
                FragmentHome.this.temp_min.setText(temp_min);
            }
        });
        if (uid != null) {
            reference.child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    UserModel userModel = new UserModel();
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if(userModel.getLatitude() != null) {
                        asyncTask.execute(userModel.getLatitude(), userModel.getLongitude());
                    } else {
                        asyncTask.execute("37.500774", "126.867899");
                    }
                    progressOff();
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {}
            });
        } else {
            asyncTask.execute("37.500774", "126.867899");
            progressOff();
        }
    }
}
