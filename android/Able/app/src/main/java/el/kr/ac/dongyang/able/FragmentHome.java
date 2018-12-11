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

//메인화면의 프레임레이아웃에 처음 들어가는 메인 역할의 프래그먼트
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
        //버스 프로바이더 등록
        BusProvider.getInstance().register(this);
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_home, container,false);

        manager = getActivity().getSupportFragmentManager();

        //목적지 설정(네비게이션) 으로 가는 버튼
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

        //메인화면의 날씨를 표시하는 뷰
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

        return view;
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    @Override
    public void onDestroy() {
        //버스 프로바이더 등록 제거
        BusProvider.getInstance().unregister(this);
        super.onDestroy();
    }

    //BusProvider에서 받아서 아이디 텍스트의 값을 변경함
    @Subscribe
    public void finishLoad(UserEvent userEvent){
        textId.setText(userEvent.getUserId());
        Log.d("finishLoad", userEvent.getUserId());
    }

    // 현재 주소에 따라서 날씨를 호출하여 뷰로 보여주는 메소드
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
