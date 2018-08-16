package el.kr.ac.dongyang.able.friend;

import android.app.ActivityOptions;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
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
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.chat.MessageActivity;
import el.kr.ac.dongyang.able.friend.FragmentUserlist;
import el.kr.ac.dongyang.able.groupriding.ChatFragment;
import el.kr.ac.dongyang.able.groupriding.PeopleFragment;
import el.kr.ac.dongyang.able.model.ChatModel;
import el.kr.ac.dongyang.able.model.FriendModel;
import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by impro on 2018-05-08.
 * 친구추가 버튼으로 유저목록으로 넘어감.
 * 아직 친구에 대한 버튼 이벤트 없음.
 * 현재 on/off 미구현
 * 마지막 접속시간 미구현
 */

public class FragmentFriend extends Fragment {

    Button btn;
    FragmentTransaction ft;
    String fragmentTag;

    HashMap friendMap;
    List<String> friendList;
    FirebaseUser user;
    String uid;
    Map.Entry entry;

    public FragmentFriend() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_friend, container, false);
        getActivity().setTitle("Friend");

        ConstraintLayout loginConstraintLayout = view.findViewById(R.id.loginConlayout);

        //친구 추가 : 유저목록으로 넘어감
        btn = view.findViewById(R.id.insert_friend);
        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Fragment fragment = new FragmentUserlist();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });
        btn.setVisibility(View.GONE);

        RecyclerView recyclerView = view.findViewById(R.id.fragment_recyclerview_friend);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new FriendlistFragmentRecyclerViewAdapter());
        recyclerView.setVisibility(View.GONE);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if (user != null) {
            uid = user.getUid();
            btn.setVisibility(View.VISIBLE);
            recyclerView.setVisibility(View.VISIBLE);
            loginConstraintLayout.setVisibility(View.GONE);
        }

        return view;
    }

    /*어댑터
        우선은 데이터베이스에 friend : uid : {대연 : true, 영훈 : true} 로 저장되어있음.
        친구 이름이 뜨려면 키값을 따로 디비에서 받아와서 저장해야했으나, 
        쿼리문으로 키값만 받아올 수가 없음.
        해시맵 friendMap으로 먼저 키,밸류값을 나누어 저장하고, entry로 키값만 불러
        리스트 friendList에 저장함.
    */
    class FriendlistFragmentRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

        private List<FriendModel> friendModels = new ArrayList<>();
        private List<String> keys = new ArrayList<>();
        private ArrayList<String> friendUsers = new ArrayList<>();

        public FriendlistFragmentRecyclerViewAdapter() {
            FirebaseDatabase.getInstance().getReference().child("FRIEND").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                    friendModels.clear();
                    keys.clear();
                    for (DataSnapshot item : dataSnapshot.child(uid).getChildren()) {
                        //friendModels.add(item.getValue(FriendModel.class));
                        keys.add(item.getKey());
                    }
                    notifyDataSetChanged();
                }

                @Override
                public void onCancelled(@NonNull DatabaseError databaseError) {

                }
            });
        }

        @Override
        public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_userlist, parent, false);

            return new CustomViewHolder(view);
        }

        @Override
        public void onBindViewHolder(RecyclerView.ViewHolder holder, final int position) {
            final CustomViewHolder customViewHolder = (CustomViewHolder) holder;
            String friendUid = null;
            // 유저를 체크
            String user = keys.get(position);
            friendUid = user;
            friendUsers.add(friendUid);

            FirebaseDatabase.getInstance().getReference().child("USER").child(user).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    UserModel userModel = dataSnapshot.getValue(UserModel.class);
                    Glide.with(customViewHolder.itemView.getContext())
                            .load(userModel.profileImageUrl)
                            .apply(new RequestOptions().circleCrop())
                            .into(customViewHolder.imageView);

                    customViewHolder.textView.setText(userModel.userName);

                    if (userModel.comment != null) {
                        customViewHolder.textView_comment.setText(userModel.comment);
                    }
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });

            holder.itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    Intent intent = new Intent(view.getContext(), ProfileActivity.class);
                    intent.putExtra("friendUid", keys.get(position));
                        startActivity(intent);
                }
            });
        }


        @Override
        public int getItemCount() {
            return keys.size();
        }

        private class CustomViewHolder extends RecyclerView.ViewHolder {
            public ImageView imageView;
            public TextView textView;
            public TextView textView_comment;

            public CustomViewHolder(View view) {
                super(view);
                imageView = view.findViewById(R.id.frienditem_imageview);
                textView = view.findViewById(R.id.frienditem_textview);
                textView_comment = view.findViewById(R.id.frienditem_textview_comment);
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
