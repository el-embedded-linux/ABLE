package el.kr.ac.dongyang.able.friend;

import android.support.annotation.Nullable;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
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
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

public class FragmentDelFriend extends android.support.v4.app.Fragment {

    HashMap friendMap;
    List<String> friendList;
    FirebaseUser user;
    String uid;
    Map.Entry entry;
    private DatabaseReference databaseReference;

    public FragmentDelFriend(){}
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_del_friend,container,false);
        getActivity().setTitle("DelFriend");
        databaseReference = FirebaseDatabase.getInstance().getReference();

        RecyclerView recyclerView = view.findViewById(R.id.fragment_recyclerview_delfriend);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new DelFriendlistFragmentRecyclerViewAdapter());
        recyclerView.setVisibility(View.GONE);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            uid = user.getUid();
            recyclerView.setVisibility(View.VISIBLE);
        }
        return view;
    }

    class DelFriendlistFragmentRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
        private List<String> keys = new ArrayList<>();
        private ArrayList<String> friendUsers = new ArrayList<>();

        public DelFriendlistFragmentRecyclerViewAdapter() {

            Log.d("data", "no");
            FirebaseDatabase.getInstance().getReference().child("FRIEND").addValueEventListener(new ValueEventListener() {
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
        public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_del_friend,parent,false);

            return new FragmentDelFriend.DelFriendlistFragmentRecyclerViewAdapter.CustomViewHolder(view);
        }

        @Override
        public void onBindViewHolder(final RecyclerView.ViewHolder holder, final int position) {

            final FragmentDelFriend.DelFriendlistFragmentRecyclerViewAdapter.CustomViewHolder customViewHolder = (FragmentDelFriend.DelFriendlistFragmentRecyclerViewAdapter.CustomViewHolder) holder;
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
                        customViewHolder.deltextview_comment.setText(userModel.comment);
                    }
                }
                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
            customViewHolder.delbtn.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    databaseReference.child("FRIEND").child(uid).child(keys.get(position)).removeValue();
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
            public TextView deltextview_comment;
            public Button delbtn;

            public CustomViewHolder(View view) {
                super(view);
                imageView = (ImageView) view.findViewById(R.id.delfrienditem_imageview);
                textView = (TextView) view.findViewById(R.id.delfrienditem_textview);
                delbtn = (Button)view.findViewById(R.id.delfriendItem_btn);
                deltextview_comment = view.findViewById(R.id.delfrienditem_textview_comment);
            }
        }
    }
    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
