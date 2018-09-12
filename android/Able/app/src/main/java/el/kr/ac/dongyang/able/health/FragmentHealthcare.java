package el.kr.ac.dongyang.able.health;

import android.animation.ObjectAnimator;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
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

/**
 * Created by user on 2018-05-13.
 * <p>
 * 클릭시 데이터가 있으면 데이터있는 화면을 표시
 * 데이터가 없으면 오늘은 운동을 안했다고 표시
 *
 * <p>
 * 몸무게 받아와서 칼로리 계산은 성공.
 */

public class FragmentHealthcare extends BaseFragment {
    private static final String LOG_TAG = "FragmentNavigation";

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

    private void healthDataInit() {
        Day day = collapsibleCalendar.getSelectedDay();
        collapsibleCalendar.collapse(0);
        userModel = new UserModel();
        final String dayFormat = day.getYear() + "-" + (day.getMonth() + 1) + "-" + day.getDay();

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

                                            //페이드인 되는 코드
                                                /*AnimatorSet set = (AnimatorSet) AnimatorInflater.loadAnimator(getActivity(), R.animator.progress_anim);
                                                set.setInterpolator(new DecelerateInterpolator());
                                                set.setTarget(arcProgress);
                                                set.start();*/
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

    public void setdate(String isdate){
        if (user != null) {
            // User is signed in
            if (reference.child("HEALTH").child(uid).getKey() != null) {
                reference.child("HEALTH").child(uid).addValueEventListener(new ValueEventListener() {
                    @Override
                    public void onDataChange(DataSnapshot dataSnapshot) {
                        healthModel = dataSnapshot.child(date).getValue(HealthModel.class);
                        if (healthModel != null) {
                            kcalTextView.setText(healthModel.getKcal() + "kcal");
                            speedTextView.setText(healthModel.getSpeed() + "km");
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
        kcalTextView.setText(cal2 + "kcal");
        speedTextView.setText(speedTextView + "km");
        Log.d(LOG_TAG, "mhcal2:  " + cal2+"mhspeed: "+cal2);
    }
    public void onStart() {
        super.onStart();

        if(user != null) {
            reference.child("HEALTH").child(uid).child("2018-08-13").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    healthModel = dataSnapshot.getValue(HealthModel.class);
                    if (healthModel != null) {
                        cal2 = healthModel.getKcal();
                        Log.d(LOG_TAG, "cal2:  " + cal2 + "speed: " + cal2);
                    }
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {
                    Log.d("databaseError", databaseError.getMessage());
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
