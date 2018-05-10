package el.kr.ac.dongyang.able;

import android.os.Bundle;
import android.support.annotation.Nullable;
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

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.security.KeyStore;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

/**
 * Created by impro on 2018-05-08.
 */

public class FragmentFriend extends Fragment{

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
        View view = inflater.inflate(R.layout.fragment_friend,container,false);
        getActivity().setTitle("Friend");

        btn = (Button) view.findViewById(R.id.insert_friend);
        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Fragment fragment = new FragmentUserlist();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft=getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });

        RecyclerView recyclerView = (RecyclerView)view.findViewById(R.id.fragment_recyclerview_friend);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new FriendlistFragmentRecyclerViewAdapter());
        user = FirebaseAuth.getInstance().getCurrentUser();
        uid = user.getUid();

        return view;
    }

    class FriendlistFragmentRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
        public FriendlistFragmentRecyclerViewAdapter() {
            friendMap = new HashMap();
            friendList = new ArrayList<>();

            FirebaseDatabase.getInstance().getReference().child("FRIEND").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    friendList.clear();
                    for(DataSnapshot snapshot :dataSnapshot.child(uid).getChildren()){
                        friendMap.clear();
                        friendMap.put(snapshot.getKey(), snapshot.getValue());
                        Iterator iterator = friendMap.entrySet().iterator();
                        while (iterator.hasNext()) {
                            entry = (Map.Entry)iterator.next();
                            Log.d("entry","Key: " + entry.getKey() + ", Value: " + entry.getValue());
                            friendList.add(entry.getKey().toString());
                        }
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
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_userlist,parent,false);

            return new CustomViewHolder(view);
        }

        @Override
        public void onBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            /*Glide.with
                    (holder.itemView.getContext())
                    // .load(userModels.get(position).profieImageUrl)
                    .load(R.drawable.users)
                    .apply(new RequestOptions().circleCrop())
                    .into(((CustomViewHolder)holder).imageView);*/

            ((CustomViewHolder)holder).textView.setText(friendList.get(position).toString());

        }

        @Override
        public int getItemCount() {
            return friendList.size();
        }

        private class CustomViewHolder extends RecyclerView.ViewHolder {
            public ImageView imageView;
            public TextView textView;

            public CustomViewHolder(View view) {
                super(view);
                //imageView = (ImageView) view.findViewById(R.id.frienditem_imageview);
                textView = (TextView) view.findViewById(R.id.frienditem_textview);
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}
