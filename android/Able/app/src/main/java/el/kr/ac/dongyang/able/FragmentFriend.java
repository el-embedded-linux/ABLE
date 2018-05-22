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
 * 친구추가 버튼으로 유저목록으로 넘어감.
 * 아직 친구에 대한 버튼 이벤트 없음.
 * 현재 on/off 미구현
 * 마지막 접속시간 미구현
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

        //친구 추가 : 유저목록으로 넘어감
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

    /*어댑터
        우선은 데이터베이스에 friend : uid : {대연 : true, 영훈 : true} 로 저장되어있음.
        친구 이름이 뜨려면 키값을 따로 디비에서 받아와서 저장해야했으나, 
        쿼리문으로 키값만 받아올 수가 없음.
        해시맵 friendMap으로 먼저 키,밸류값을 나누어 저장하고, entry로 키값만 불러
        리스트 friendList에 저장함.
    */
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

        //Glide 라는 깃허브 오픈 라이브러리를 그대로 따라해봤으나
        //안됐음. 에러코드는 app수준 빌드.그래들에 있음.ㅠ..
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
