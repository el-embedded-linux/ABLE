package el.kr.ac.dongyang.able.health;

import android.animation.ObjectAnimator;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.DecelerateInterpolator;
import android.widget.TextView;

import com.github.lzyzsd.circleprogress.ArcProgress;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import com.shrikanthravi.collapsiblecalendarview.data.Day;
import com.shrikanthravi.collapsiblecalendarview.widget.CollapsibleCalendar;

import el.kr.ac.dongyang.able.BaseFragment;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.HealthModel;
import el.kr.ac.dongyang.able.model.UserModel;

//유저의 운동기록을 달력으로 구현하는 프래그먼트
public class FragmentHealthcare extends BaseFragment {

    ConstraintLayout constraintLayoutHealth, constraintLayoutNone;

    float currentGoal;
    String date, uid, cal2;
    TextView speedTextView, kcalTextView, distanceTextView, goalTextView, informationTextView;
    FirebaseUser user;
    UserModel userModel;
    HealthModel healthModel;

    ArcProgress arcProgress;
    CollapsibleCalendar collapsibleCalendar;

    private Handler mHandler;

    public FragmentHealthcare() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_healthcare, container, false);
        getActivity().setTitle("EL 헬스케어");

        constraintLayoutHealth = view.findViewById(R.id.constraintLayoutHealth);
        constraintLayoutHealth.setVisibility(View.GONE);
        constraintLayoutNone = view.findViewById(R.id.constraintLayoutNone);
        constraintLayoutNone.setVisibility(View.GONE);
        informationTextView = view.findViewById(R.id.informationTextView);

        //하루의 운동기록이 미리 설정된 목표에 도달했는지 프로그레스바로 표현
        arcProgress = view.findViewById(R.id.arc_progress);
        mHandler = new Handler();

        //칼로리 부분
        speedTextView = view.findViewById(R.id.speed_text);
        kcalTextView = view.findViewById(R.id.burnUpTextView);
        distanceTextView = view.findViewById(R.id.distanceTextView);
        goalTextView = view.findViewById(R.id.goal_text);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            uid = user.getUid();
        }
        userModel = new UserModel();
        healthModel = new HealthModel();

        //달력을 이동하면서 해당일의 기록 확인 가능
        collapsibleCalendar = view.findViewById(R.id.collapsibleCalendarView);
        collapsibleCalendar.setState(0);
        collapsibleCalendar.setCalendarListener(new CollapsibleCalendar.CalendarListener() {
            @Override
            public void onDaySelect() {
                if(user == null) {
                    collapsibleCalendar.collapse(0);
                    constraintLayoutHealth.setVisibility(View.GONE);
                    constraintLayoutNone.setVisibility(View.VISIBLE);
                    informationTextView.setText("로그인을 해주세요");
                } else {
                    healthDataInit();
                }
            }
            @Override
            public void onItemClick(View view) {

            }
            @Override
            public void onDataUpdate() {

            }
            @Override
            public void onMonthChange() {

            }
            @Override
            public void onWeekChange(int i) {

            }
        });

        return view;
    }

    //달력의 날짜를 클릭했을때 실행.
    private void healthDataInit() {
        Day day = collapsibleCalendar.getSelectedDay();
        collapsibleCalendar.collapse(0);
        userModel = new UserModel();
        final String dayFormat = day.getYear() + "-" + (day.getMonth() + 1) + "-" + day.getDay();

        //DB에서 헬스기록을 받아온다.
        FirebaseDatabase.getInstance().getReference().child("HEALTH").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
            HealthModel healthModel = new HealthModel();
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                healthModel = dataSnapshot.child(dayFormat).getValue(HealthModel.class);

                if(healthModel != null) {
                    constraintLayoutNone.setVisibility(View.GONE);
                    constraintLayoutHealth.setVisibility(View.VISIBLE);
                    kcalTextView.setText(healthModel.getKcal());
                    distanceTextView.setText(healthModel.getDistance());
                    speedTextView.setText(healthModel.getSpeed());

                    //유저의 운동목표를 받아온다. 목표와 운동기록을 비교하여 화면에 다른 텍스트를 표시
                    FirebaseDatabase.getInstance().getReference().child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
                        @Override
                        public void onDataChange(DataSnapshot dataSnapshot) {
                            userModel = dataSnapshot.getValue(UserModel.class);
                            if(userModel.getGoal() == null) {
                                currentGoal = 0;
                                goalTextView.setText("먼저 내정보 설정에서 목표를 설정해주세요");
                            }else if(healthModel.getDistance().equals("")){
                                currentGoal = 0;
                                goalTextView.setText("목표 완수까지 " + userModel.getGoal() + "km가 남았습니다!");

                            } else if(Float.parseFloat(healthModel.getDistance()) < Float.parseFloat(userModel.getGoal())) {
                                currentGoal = (Float.parseFloat(healthModel.getDistance()) / Float.parseFloat(userModel.getGoal())) * 100;
                                float remainDistance = Float.parseFloat(userModel.getGoal())-Float.parseFloat(healthModel.getDistance());
                                goalTextView.setText("목표 완수까지 " + remainDistance + "km가 남았습니다!");

                            } else {
                                currentGoal = 100;
                                goalTextView.setText("오늘의 목표를 달성했습니다!");
                            }
                            //운동기록에 따라 프로그레스 바의 값이 올라간다.
                            Thread t = new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    mHandler.post(new Runnable() {
                                        @Override
                                        public void run() {
                                            //수치가 올라가는 코드
                                            ObjectAnimator anim = ObjectAnimator.ofInt(arcProgress, "progress", 0, (int)currentGoal);
                                            anim.setInterpolator(new DecelerateInterpolator());
                                            anim.setDuration(1000);
                                            anim.start();
                                        }
                                    });
                                }
                            });
                            t.start();
                        }
                        @Override
                        public void onCancelled(DatabaseError databaseError) {

                        }
                    });
                } else {
                    constraintLayoutHealth.setVisibility(View.GONE);
                    constraintLayoutNone.setVisibility(View.VISIBLE);
                    informationTextView.setText("기록이 없습니다\n운동을 해주세요");
                }
            }
            @Override
            public void onCancelled(DatabaseError databaseError) {
                Log.d("databaseError", databaseError.getMessage());
            }
        });
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
