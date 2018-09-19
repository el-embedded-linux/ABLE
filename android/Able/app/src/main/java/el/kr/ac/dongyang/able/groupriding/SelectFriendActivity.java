package el.kr.ac.dongyang.able.groupriding;

import android.app.ActivityOptions;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.RequestOptions;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import el.kr.ac.dongyang.able.BaseActivity;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.chat.MessageActivity;
import el.kr.ac.dongyang.able.model.ChatModel;
import el.kr.ac.dongyang.able.model.UserModel;

public class SelectFriendActivity extends BaseActivity {
    ChatModel chatModel = new ChatModel();
    Map<String, Boolean> user = new HashMap<>();
    String myUid;
    SelectFriendRecyclerViewAdapter selectFriendRecyclerViewAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_friend);

        RecyclerView recyclerView = findViewById(R.id.selectFriendActivity_recyclerview);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        FirebaseUser firebaseUser = FirebaseAuth.getInstance().getCurrentUser();
        if (firebaseUser != null) {
            myUid = firebaseUser.getUid();
            selectFriendRecyclerViewAdapter = new SelectFriendRecyclerViewAdapter();
            recyclerView.setAdapter(selectFriendRecyclerViewAdapter);
        }

        Button button = findViewById(R.id.selectFriendActivity_button);
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (myUid != null) {
                    user.put(myUid, true);
                    chatModel.setUsers(user);
                    FirebaseDatabase.getInstance().getReference().child("CHATROOMS").push().setValue(chatModel);
                    user.clear();
                    //selectFriendRecyclerViewAdapter.notifyDataSetChanged();
                }
            }
        });
    }


    class SelectFriendRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

        List<UserModel> userModels = new ArrayList<>();

        public SelectFriendRecyclerViewAdapter() {
            reference.child("USER").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModels.clear();
                    for(DataSnapshot snapshot :dataSnapshot.getChildren()){
                        UserModel userModel = snapshot.getValue(UserModel.class);
                        if(userModel.getUid().equals(myUid)){
                            continue;
                        }
                        userModels.add(userModel);
                    }
                    notifyDataSetChanged();
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {}
            });
        }

        @Override
        public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_friend_select,parent,false);
            return new CustomViewHolder(view);
        }

        @Override
        public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, final int position) {
            CustomViewHolder customViewHolder = ((CustomViewHolder) holder);

            Glide
                    .with(customViewHolder.itemView.getContext())
                    .load(userModels.get(position).getProfileImageUrl())
                    .apply(new RequestOptions().circleCrop())
                    .into(customViewHolder.imageView);
            customViewHolder.textView.setText(userModels.get(position).getUserName());

            customViewHolder.itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    Intent intent = new Intent(view.getContext(), MessageActivity.class);
                    intent.putExtra("destinationUid",userModels.get(position).getUid());
                    ActivityOptions activityOptions = null;
                    activityOptions = ActivityOptions.makeCustomAnimation(view.getContext(), R.anim.fromright,R.anim.toleft);
                    startActivity(intent,activityOptions.toBundle());

                }
            });

            if(userModels.get(position).getComment() != null){
                customViewHolder.textView_comment.setText(userModels.get(position).getComment());
            }
            customViewHolder.checkBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
                @Override
                public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                    if(b){
                        user.put(userModels.get(position).getUid(),true);
                    } else {
                        user.remove(userModels.get(position).getUid());
                    }
                }
            });
        }

        @Override
        public int getItemCount() {
            return userModels.size();
        }

        private class CustomViewHolder extends RecyclerView.ViewHolder {
            ImageView imageView;
            TextView textView;
            TextView textView_comment;
            CheckBox checkBox;

            CustomViewHolder(View view) {
                super(view);
                imageView =  view.findViewById(R.id.frienditem_imageview);
                textView =  view.findViewById(R.id.frienditem_textview);
                textView_comment = view.findViewById(R.id.frienditem_textview_comment);
                checkBox = view.findViewById(R.id.friendItem_checkbox);
            }
        }
    }
}
