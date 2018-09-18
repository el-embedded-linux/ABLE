package el.kr.ac.dongyang.able;

import android.animation.ObjectAnimator;
import android.animation.StateListAnimator;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.os.Bundle;
import android.support.design.widget.AppBarLayout;
import android.support.design.widget.NavigationView;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.widget.Toolbar;
import android.text.Html;
import android.util.Base64;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.RequestOptions;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import com.google.firebase.iid.FirebaseInstanceId;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.HashMap;
import java.util.Map;

import el.kr.ac.dongyang.able.friend.FragmentFriend;
import el.kr.ac.dongyang.able.groupriding.PeopleFragment;
import el.kr.ac.dongyang.able.health.FragmentHealthcare;
import el.kr.ac.dongyang.able.login.FragmentLogin;
import el.kr.ac.dongyang.able.model.UserModel;
import el.kr.ac.dongyang.able.navigation.NavigationActivity;
import el.kr.ac.dongyang.able.setting.FragmentSetting;


//기본적으로 프래그먼트홈이 뜨도록 되어있음. content_xml에서 설정됨.
public class MainActivity extends BaseActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    private String TAG = "Able";
    FragmentTransaction ft;
    FragmentManager manager;
    FirebaseUser firebaseUser;
    NavigationView navigationView;
    TextView naviTitle, naviSubTitle;
    ImageView naviImg;

    TextView weatherIcon, temperature, temp_max, temp_min;
    WeatherIconManager weatherIconManager;

    FragmentHome fragmentHome;
    FragmentFriend fragmentFriend;
    FragmentLogin fragmentLogin;
    FragmentHealthcare fragmentHealthcare;
    FragmentSetting fragmentSetting;
    PeopleFragment peopleFragment;
    private long pressedTime;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        manager = getSupportFragmentManager();
        fragmentHome = new FragmentHome();
        fragmentFriend = new FragmentFriend();
        fragmentLogin = new FragmentLogin();
        fragmentHealthcare = new FragmentHealthcare();
        fragmentSetting = new FragmentSetting();
        peopleFragment = new PeopleFragment();

        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.addDrawerListener(toggle);
        toggle.syncState();

        navigationView = findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);

        View headerView = navigationView.getHeaderView(0);
        naviTitle = headerView.findViewById(R.id.NaviHeaderMainTextViewTitle);
        naviSubTitle = headerView.findViewById(R.id.NaviHeaderMainTextViewSubTitle);
        naviImg = headerView.findViewById(R.id.NaviHeaderMainImageView);

        weatherIcon = findViewById(R.id.weather);
        temperature = findViewById(R.id.Temperature);
        temp_max = findViewById(R.id.Temp_max);
        temp_min = findViewById(R.id.Temp_min);
        weatherIconManager = new WeatherIconManager();
        setWeather();

        //getHashKey();
    }

    @Override
    protected void onStart() {
        super.onStart();
        FirebaseUser user = FirebaseAuth.getInstance().getCurrentUser();
        if (user != null) {
            String uid = user.getUid();
            passPushTokenToServer(uid);
            reference.child("USER").child(uid).addValueEventListener(new ValueEventListener() {
                UserModel userModel = new UserModel();
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if(userModel.getProfileImageUrl() != null) {
                        Glide.with(MainActivity.this)
                                .load(userModel.getProfileImageUrl())
                                .apply(new RequestOptions().circleCrop())
                                .into(naviImg);
                    }
                    naviTitle.setText(userModel.getUserName());
                    naviSubTitle.setText(userModel.getComment());
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
        }
    }

    private void setWeather() {
        String font = "fonts/weathericons-regular-webfont.ttf";
        weatherIcon.setTypeface(weatherIconManager.get_icons(font, this));

        WeatherFunction.placeIdTask asyncTask = new WeatherFunction.placeIdTask(new WeatherFunction.AsyncResponse() {
            public void processFinish(String temperature, String temp_max, String temp_min, String updatedOn, String iconText, String sun_rise) {
                weatherIcon.setText(Html.fromHtml(iconText));
                MainActivity.this.temperature.setText(temperature);
                MainActivity.this.temp_max.setText(temp_max);
                MainActivity.this.temp_min.setText(temp_min);
            }
        });
        asyncTask.execute("37.500774", "126.867899");
    }

    private void passPushTokenToServer(String uid) {
        String token = FirebaseInstanceId.getInstance().getToken();
        Map<String, Object> map = new HashMap<>();
        map.put("pushToken", token);

        FirebaseDatabase.getInstance().getReference().child("USER").child(uid).updateChildren(map);
    }

    //해시키 구하는 함수
    private void getHashKey() {
        try {
            PackageInfo info = getPackageManager().getPackageInfo(this.getPackageName(), PackageManager.GET_SIGNATURES);
            for (Signature signature : info.signatures) {
                MessageDigest md = MessageDigest.getInstance("SHA");
                md.update(signature.toByteArray());
                Log.d(TAG, "key_hash=" + Base64.encodeToString(md.digest(), Base64.DEFAULT));
            }
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
    }

    //drawer 백버튼 클릭시 동작
    @Override
    public void onBackPressed() {
        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else if (manager.getBackStackEntryCount()==0){
            if (pressedTime == 0) {
                Toast.makeText(MainActivity.this, " 한 번 더 누르면 종료됩니다.", Toast.LENGTH_SHORT).show();
                pressedTime = System.currentTimeMillis();
            } else {
                int seconds = (int) (System.currentTimeMillis() - pressedTime);
                if (seconds > 2000) {
                    Toast.makeText(MainActivity.this, " 한 번 더 누르면 종료됩니다.", Toast.LENGTH_SHORT).show();
                    pressedTime = 0;
                } else {
                    super.onBackPressed();
                }
            }
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    //메뉴 아이템에 무엇을 넣을 것인지, 더 추가도 가능. 현재는 친구목록 : FragmentFriend로 이동
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        // set the toolbar title

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_friend) {
            replaceFragment(fragmentFriend);
        }
        return super.onOptionsItemSelected(item);
    }

    //네비게이션 바에서 항목을 클릭해서 넘어가는 부분.
    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();

        switch (id) {
            case R.id.nav_login:
                replaceFragment(fragmentLogin);
                break;
            case R.id.nav_navigation:
                Intent intent = new Intent(MainActivity.this, NavigationActivity.class);
                intent.putExtra("clickBtn", "start");
                startActivity(intent);
                break;
            case R.id.nav_helthcare:
                replaceFragment(fragmentHealthcare);
                break;
            case R.id.nav_groupriding:
                replaceFragment(peopleFragment);
                break;
            case R.id.nav_setting:
                replaceFragment(fragmentSetting);
                break;
        }

        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }
}

