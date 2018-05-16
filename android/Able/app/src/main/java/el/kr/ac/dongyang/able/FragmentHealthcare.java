package el.kr.ac.dongyang.able;


import android.app.DatePickerDialog;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.DatePicker;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.lang.ref.WeakReference;
import java.util.Calendar;

import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by user on 2018-05-13.
 *
 * 월 텍스트 클릭시 datepicker 나옴. 근데 왜 현재 날짜가 아니라 다른 날짜 기준으로 나오는지는 모르겠음.
 * 기본설정이 앱의 강조색상으로 코딩되어있는듯. 분홍색으로 나옴.
 * 월 숫자는 변경되도록 했지만 요일, 날짜 텍스트 갱신은 아직 미구현.
 *
 * 몸무게 받아와서 칼로리 계산은 성공.
 *
 */

public class FragmentHealthcare extends android.support.v4.app.Fragment{
    private static final String LOG_TAG = "FragmentNavigation";

    TextView monthHan, monthNum, sun, mon, tue, wed, thu, fri, sat;
    TextView sunNum, monNum, tueNum, wedNum, ThuNum, friNum,satNum;
    Calendar cal;
    int day,month,year;

    double speed = 31.5;
    double height = 175.3;
    Double weight;
    int minute = 60;
    double MET = 3.3;
    TextView sPeed ;
    TextView sPeedText;
    TextView cOnsume;
    FirebaseUser user;
    String uid;
    UserModel userModel;
    CalckThread kcal;
    String cal2;

    DatabaseReference mDatabase = FirebaseDatabase.getInstance().getReference();
    //DatabaseReference mConditionRef = mDatabase.child("USER");

    public FragmentHealthcare() {
    }

    private final MyHandler mHandler = new MyHandler(this);

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_healthcare,container,false);
        getActivity().setTitle("Health care");

        //월의 숫자와 텍스트 둘다 클릭시 반응하도록 레이아웃 입힘.
        ConstraintLayout monthConslay = (ConstraintLayout)view.findViewById(R.id.constraintLayoutMonth);

        monthNum = (TextView)view.findViewById(R.id.Month);
        //monthHan = (TextView)view.findViewById(R.id.month);

        cal= Calendar.getInstance();

        day=cal.get(Calendar.DAY_OF_MONTH);
        month=cal.get(Calendar.MONTH);
        year=cal.get(Calendar.YEAR);

        month = month+1;

        //monthHan.setText(day+"/"+month+"/"+year);

        monthConslay.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DatePickerDialog datePickerDialog = new DatePickerDialog(getActivity(), new DatePickerDialog.OnDateSetListener() {
                    @Override
                    public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
                        monthNum.setText(Integer.toString(monthOfYear+1));
                        //monthHan.setText(dayOfMonth+"/"+Integer.toString(monthOfYear+1)+"/"+year);
                    }
                },year,month,day);
                datePickerDialog.show();
            }
        });

        //칼로리 부분

        sPeed = (TextView)view.findViewById(R.id.speed);
        sPeedText = (TextView)view.findViewById(R.id.speed_text);
        cOnsume = (TextView)view.findViewById(R.id.consume);

        user = FirebaseAuth.getInstance().getCurrentUser();
        uid = user.getUid();
        userModel = new UserModel();

        return view;
    }

    private void handleMessage(Message msg) { // 핸들러가 gui수정
        cOnsume.setText(cal2+"kcal");
    }

    public void onStart() {
        super.onStart();

        mDatabase.child("USER").child(uid).addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                userModel = dataSnapshot.getValue(UserModel.class);
                weight = Double.parseDouble(userModel.weight);
                //Toast.makeText(getApplicationContext(), "" + weight, Toast.LENGTH_SHORT).show();
                Log.d(LOG_TAG, "start - " + Double.toString(weight));
                kcal = new CalckThread(weight);
                kcal.start();
            }

            @Override
            public void onCancelled(DatabaseError databaseError) {
            }
        });
    }
    class CalckThread extends Thread {
        Double weight;
        public CalckThread(Double weight) {
            this.weight = weight;
        }
        public void run() {
            try {
                Double air = (MET * (3.5 * weight * minute))*0.001;
                Double cal = air * 5;
                cal2 = String.format("%.2f",cal);
                Log.d(LOG_TAG, "Thread - " + Double.toString(weight));
                mHandler.sendMessage(mHandler.obtainMessage()); // 핸들러에 화면 변경 요청
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }

    private static class MyHandler extends Handler {//Handler 클래스를 하나 만듭니다. 이 클래스의 역할은 스레드에서 전달받은 메시지를 메인
        // activiy 로 넘겨주는 역할을 합니다. 메인 activity 에서는 이것을 받아 적절한 처리를
        // 하면 되는데 여기선 메시지를 사용하지 않음

        private final WeakReference<FragmentHealthcare> mActivity;

        public MyHandler(FragmentHealthcare activity) {
            mActivity = new WeakReference<FragmentHealthcare>(activity);
        }

        @Override
        public void handleMessage(Message msg) {
            FragmentHealthcare activity = mActivity.get();
            if (activity != null) {
                activity.handleMessage(msg);
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
