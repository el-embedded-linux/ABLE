package el.kr.ac.dongyang.able.health;


import android.app.DatePickerDialog;
import android.bluetooth.BluetoothAdapter;
import android.os.Bundle;
import android.os.Message;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.DatePicker;
import android.widget.TextView;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.lang.ref.WeakReference;
import java.lang.String;
import java.util.Calendar;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.HealthModel;
import el.kr.ac.dongyang.able.model.UserModel;

import static com.facebook.login.widget.ProfilePictureView.TAG;

/**
 * Created by user on 2018-05-13.
 * <p>
 * 월 텍스트 클릭시 datepicker 나옴. 근데 왜 현재 날짜가 아니라 다른 날짜 기준으로 나오는지는 모르겠음.
 * 월 숫자는 변경되도록 했지만 요일, 날짜 텍스트 갱신은 아직 미구현.
 * <p>
 * 몸무게 받아와서 칼로리 계산은 성공.
 */

public class FragmentHealthcare extends android.support.v4.app.Fragment{
    private static final String LOG_TAG = "FragmentNavigation";

    TextView monthNum;
    Calendar cal;
    int day, month, year;

    String date;
    String speed = "31.5";
    Double bykg = 10.0;
    int minute = 60;
    Double MET = 3.3;
    TextView sPeed;
    TextView sPeedText;
    TextView cOnsume;
    FirebaseUser user;
    String uid;
    UserModel userModel;
    HealthModel healthModel;
    String cal2;
    private int listenerCode;

    DatabaseReference mDatabase = FirebaseDatabase.getInstance().getReference();

    public FragmentHealthcare() {
    }

    static BluetoothAdapter mBluetoothAdapter;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_healthcare, container, false);
        getActivity().setTitle("Health care");
        //if(getArguments() !=null) {
        //int speedInt = getArguments().getInt("key");
        //Log.e("aa", "값 : " + speedInt);
        //}
        //월의 숫자와 텍스트 둘다 클릭시 반응하도록 레이아웃 입힘.
        ConstraintLayout monthConslay = (ConstraintLayout) view.findViewById(R.id.constraintLayoutMonth);

        monthNum = (TextView) view.findViewById(R.id.Month);

        cal = Calendar.getInstance();

        day = cal.get(Calendar.DAY_OF_MONTH);
        month = cal.get(Calendar.MONTH)+1;
        year = cal.get(Calendar.YEAR);
        if(month>=10 && day<10){
            date = year + "-" + month + "-0" + day;
        }else if(month<10 && day>=10){
            date = year + "-0" + month + "-" + day;
        } else if (month>=10 && day>=10) {
            date = year + "-" + month + "-" + day;
        }else{
            date = year + "-0" + month + "-0" + day;
        }
        setdate(date);
        Log.d("Today : ", date);
        monthConslay.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DatePickerDialog datePickerDialog = new DatePickerDialog(getActivity(), new DatePickerDialog.OnDateSetListener() {
                    @Override
                    public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
                        monthOfYear += 1;
                        if(monthOfYear>=10 && dayOfMonth<10){
                            date = year + "-" + monthOfYear + "-0" + dayOfMonth;
                        }else if(monthOfYear<10 && dayOfMonth>=10){
                            date = year + "-0" + monthOfYear + "-" + dayOfMonth;
                        } else if (monthOfYear>=10 && dayOfMonth>=10) {
                            date = year + "-" + monthOfYear + "-" + dayOfMonth;
                        }else{
                            date = year + "-0" + monthOfYear + "-0" + dayOfMonth;
                        }
                        setdate(date);
                    }
                }, year, month-1, day);
                datePickerDialog.show();
            }
        });

        //칼로리 부분

        sPeed = (TextView) view.findViewById(R.id.speed);
        sPeedText = (TextView) view.findViewById(R.id.speed_text);
        cOnsume = (TextView) view.findViewById(R.id.consume);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            uid = user.getUid();
        }
        userModel = new UserModel();
        healthModel = new HealthModel();
        Log.d(TAG, "Initalizing Bluetooth adapter...");

        return view;
    }

    protected void setdate(String isdate){
        //user가 있으면 기존에 저장된 값을 호출함.
        this.date = isdate;
        String datetoday = isdate;
        String year = datetoday.substring(0,4);
        String month = datetoday.substring(5,7);
        String day = datetoday.substring(8,10);

        monthNum.setText(year+"." +month+"."+day);
        listenerCode = 1;
        if (user != null) {
            // User is signed in
            if (mDatabase.child("HEALTH").child(uid).getKey() != null) {
                mDatabase.child("HEALTH").child(uid).addValueEventListener(new ValueEventListener() {
                    @Override
                    public void onDataChange(DataSnapshot dataSnapshot) {
                        healthModel = dataSnapshot.child(date).getValue(HealthModel.class);
                        if (healthModel != null) {
                            cOnsume.setText(healthModel.kcal + "kcal");
                            sPeedText.setText(healthModel.speed + "km");
                        }
                    }
                    @Override
                    public void onCancelled(DatabaseError databaseError) {
                    }
                });
            } else {
            }
        } else {
            // No user is signed in
        }
    }
    private void handleMessage(Message msg) { // 핸들러가 gui수정
        cOnsume.setText(cal2 + "kcal");
        sPeedText.setText(speed + "km");
        Log.d(LOG_TAG, "mhcal2:  " + cal2+"mhspeed: "+cal2);
    }
    public void onStart() {
        super.onStart();

        cal = Calendar.getInstance();

        day = cal.get(Calendar.DAY_OF_MONTH);
        month = cal.get(Calendar.MONTH)+1;
        year = cal.get(Calendar.YEAR);
        if(month>=10 && day<10){
            date = year + "-" + month + "-0" + day;
        }else if(month<10 && day>=10){
            date = year + "-0" + month + "-" + day;
        } else if (month>=10 && day>=10) {
            date = year + "-" + month + "-" + day;
        }else{
            date = year + "-0" + month + "-0" + day;
        }
        setdate(date);

        if(user != null) {
            mDatabase.child("HEALTH").child(uid).child(date).addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    healthModel = dataSnapshot.getValue(HealthModel.class);
                    if (healthModel != null) {
                        cal2 = healthModel.kcal;
                        speed = healthModel.speed;
                        Log.d(LOG_TAG, "cal2:  " + cal2 + "speed: " + cal2);
                    }
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {

                }
            });
        }
    }
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
