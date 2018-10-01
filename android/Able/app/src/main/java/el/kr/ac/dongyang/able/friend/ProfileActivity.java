package el.kr.ac.dongyang.able.friend;

import android.os.Bundle;
import android.support.annotation.Nullable;
import android.widget.ImageView;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.RequestOptions;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.ValueEventListener;

import el.kr.ac.dongyang.able.BaseActivity;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.HealthModel;
import el.kr.ac.dongyang.able.model.UserModel;

//친구목록에서 친구를 클릭시 친구의 정보를 표시
public class ProfileActivity extends BaseActivity {

    private ImageView imageView;
    private TextView userName, height, distance, kcal, speed, message, goal;
    private String uid, friendUid;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_friend_profile);

        imageView = findViewById(R.id.friendProfileImageView);
        userName = findViewById(R.id.friendProfileName);
        height = findViewById(R.id.friendProfileHeightTextView);
        goal = findViewById(R.id.friendProfileGoalTextView);
        message = findViewById(R.id.friendProfileMessageTextView);
        distance = findViewById(R.id.friendProfileDistanceTextView);
        kcal = findViewById(R.id.friendProfileKcalTextView);
        speed = findViewById(R.id.friendProfileSpeedTextView);

        uid = FirebaseAuth.getInstance().getCurrentUser().getUid();
        friendUid = getIntent().getStringExtra("friendUid");

        //DB에서 친구의 uid를 호출
        reference.child("USER").child(friendUid).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                UserModel userModel = new UserModel();
                userModel = dataSnapshot.getValue(UserModel.class);

                Glide.with(getApplicationContext())
                        .load(userModel.getProfileImageUrl())
                        .apply(new RequestOptions().circleCrop())
                        .into(imageView);

                userName.setText(userModel.getUserName());
                height.setText(userModel.getHeight());
                goal.setText(userModel.getGoal());
                message.setText(userModel.getComment());

                //DB에서 친구의 운동기록을 호출
                reference.child("TOTALHEALTH").child(friendUid).addListenerForSingleValueEvent(new ValueEventListener() {
                    @Override
                    public void onDataChange(DataSnapshot dataSnapshot) {
                        HealthModel healthModel = new HealthModel();
                        healthModel = dataSnapshot.getValue(HealthModel.class);
                        distance.setText(healthModel.getDistance() + " km");
                        kcal.setText(healthModel.getKcal() + " kcal");
                        speed.setText(healthModel.getSpeed() + " km/h");
                    }

                    @Override
                    public void onCancelled(DatabaseError databaseError) {

                    }
                });

            }

            @Override
            public void onCancelled(DatabaseError databaseError) {

            }
        });


    }
}
