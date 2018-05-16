package el.kr.ac.dongyang.able;

import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.support.annotation.NonNull;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.support.design.widget.NavigationView;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.AttributeSet;
import android.util.Base64;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.widget.Toast;

import com.facebook.AccessToken;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthCredential;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FacebookAuthProvider;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

//기본적으로 프래그먼트홈이 뜨도록 되어있음. content_xml에서 설정됨.
public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    private String TAG = "Able";
    FragmentTransaction ft;
    String fragmentTag;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.addDrawerListener(toggle);
        toggle.syncState();

        NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);

        getHashKey();
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
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
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
            Fragment fragment = new FragmentFriend();
            fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
            Log.i("fagmentTag", fragmentTag);
            getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            ft = getSupportFragmentManager().beginTransaction();
            ft.replace(R.id.main_layout, fragment);
            ft.addToBackStack(fragmentTag);
            ft.commit();
        }

        return super.onOptionsItemSelected(item);
    }

    //네비게이션 바에서 항목을 클릭해서 넘어가는 부분.
    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();

        if (id == R.id.nav_login) {
            Fragment fragment = new FragmentLogin();
            fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
            Log.i("fagmentTag", fragmentTag);
            getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            ft = getSupportFragmentManager().beginTransaction();
            ft.replace(R.id.main_layout, fragment);
            ft.addToBackStack(fragmentTag);
            ft.commit();

        } else if (id == R.id.nav_navigation) {     //영훈
            Fragment fragment = new FragmentNavigation();
            fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
            Log.i("fagmentTag", fragmentTag);
            getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            ft = getSupportFragmentManager().beginTransaction();
            ft.replace(R.id.main_layout, fragment);
            ft.addToBackStack(fragmentTag);
            ft.commit();

        } else if (id == R.id.nav_helthcare) {      //승현
            Fragment fragment = new FragmentHealthcare();
            fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
            Log.i("fagmentTag", fragmentTag);
            getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            ft = getSupportFragmentManager().beginTransaction();
            ft.replace(R.id.main_layout, fragment);
            ft.addToBackStack(fragmentTag);
            ft.commit();

        } else if (id == R.id.nav_groupriding) {    //지수

        } else if (id == R.id.nav_setting) {        //수현
            Fragment fragment = new FragmentSetting();
            fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
            Log.i("fagmentTag", fragmentTag);
            getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            ft = getSupportFragmentManager().beginTransaction();
            ft.replace(R.id.main_layout, fragment);
            ft.addToBackStack(fragmentTag);
            ft.commit();

        }

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }
}

