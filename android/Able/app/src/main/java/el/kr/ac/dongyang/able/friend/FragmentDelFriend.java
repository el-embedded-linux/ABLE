package el.kr.ac.dongyang.able.friend;

import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
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
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

import el.kr.ac.dongyang.able.BaseFragment;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

//친구목록에서 친구를 삭제하는 뷰
public class FragmentDelFriend extends BaseFragment {

    FirebaseUser user;
    String uid;

    public FragmentDelFriend(){}
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_del_friend,container,false);
        getActivity().setTitle("DelFriend");

        //recycler View 설정
        RecyclerView recyclerView = view.findViewById(R.id.fragment_recyclerview_delfriend);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new DelFriendAdapter());
        recyclerView.setVisibility(View.GONE);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            uid = user.getUid();
            recyclerView.setVisibility(View.VISIBLE);
        }
        return view;
    }

    //친구 삭제하는 recyclerView 어댑터 클래스 설정
    class DelFriendAdapter extends RecyclerView.Adapter<DelFriendAdapter.CustomViewHolder> {
        private List<String> keys = new ArrayList<>();
        private List<String> friendUsers = new ArrayList<>();

        public DelFriendAdapter() {
            reference.child("FRIEND").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    keys.clear();
                    for (DataSnapshot item : dataSnapshot.child(uid).getChildren()) {
                        keys.add(item.getKey());
                    }
                    notifyDataSetChanged();
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
        }
        @Override
        public DelFriendAdapter.CustomViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            LayoutInflater inflater = LayoutInflater.from(parent.getContext());
            View view = inflater.inflate(R.layout.item_del_friend,parent,false);
            return new DelFriendAdapter.CustomViewHolder(view);
        }

        @Override
        public void onBindViewHolder(final DelFriendAdapter.CustomViewHolder holder, final int position) {
            String friendUid = null;
            // 유저를 체크
            String user = keys.get(position);
            friendUid = user;
            friendUsers.add(friendUid);

            reference.child("USER").child(user).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    UserModel userModel = dataSnapshot.getValue(UserModel.class);
                    Glide.with(holder.itemView.getContext())
                            .load(userModel.getProfileImageUrl())
                            .apply(new RequestOptions().circleCrop())
                            .into(holder.imageView);
                    holder.textView.setText(userModel.getUserName());
                    if (userModel.getComment() != null) {
                        holder.delTextView_comment.setText(userModel.getComment());
                    }
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });

            //친구 삭제 버튼
            holder.delBtn.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    reference.child("FRIEND").child(uid).child(keys.get(position)).removeValue();
                }
            });
        }
        @Override
        public int getItemCount() {
            return keys.size();
        }

        public class CustomViewHolder extends RecyclerView.ViewHolder {
            ImageView imageView;
            TextView textView;
            TextView delTextView_comment;
            Button delBtn;

            public CustomViewHolder(View view) {
                super(view);
                imageView = view.findViewById(R.id.delfrienditem_imageview);
                textView = view.findViewById(R.id.delfrienditem_textview);
                delBtn =view.findViewById(R.id.delfriendItem_btn);
                delTextView_comment = view.findViewById(R.id.delfrienditem_textview_comment);
            }
        }
    }
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
