package el.kr.ac.dongyang.able.friend;

import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.RecyclerItemClickListener;
import el.kr.ac.dongyang.able.model.FriendModel;
import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by impro on 2018-05-08.
 * 전체 유저의 목록을 데이터베이스로부터 불러와 리사이클러뷰로 보여준다.
 * 전체 유저라서 본인까지 뜨는 문제.
 * 예외를 두려면 어떻게 해야할지 고민해야 함.
 * 이미지뷰도 아직 안넣었음.
 */

public class FragmentUserlist extends android.support.v4.app.Fragment{

    public FragmentUserlist() {
    }

    List<UserModel> userModels;
    FirebaseUser user;
    String uid;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_userlist,container,false);
        getActivity().setTitle("UserList");
        //리사이클러뷰 맵핑
        RecyclerView recyclerView = (RecyclerView)view.findViewById(R.id.fragment_recyclerview);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new UserlistFragmentRecyclerViewAdapter());
        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            uid = user.getUid();
        }

        //리사이클러뷰 클릭이벤트
        recyclerView.addOnItemTouchListener(
                new RecyclerItemClickListener(getActivity(), recyclerView, new RecyclerItemClickListener.OnItemClickListener() {
                    @Override
                    public void onItemClick(View view, int position) {
                        FirebaseDatabase.getInstance().getReference().child("FRIEND").child(uid).child(userModels.get(position).userName).setValue("true");
                        Toast.makeText(getActivity(),position+"번 째 유저 친구추가",Toast.LENGTH_SHORT).show();
                    }

                    @Override
                    public void onLongItemClick(View view, int position) {
                        Toast.makeText(getActivity(),position+"번 째 아이템 롱 클릭", Toast.LENGTH_SHORT).show();
                    }
                }));

        return view;
    }

    //어댑터 클래스
    class UserlistFragmentRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

        //userModels라는 리스트를 만들고 addValueEventListerner와 DataSnapshot을 이용해 데이터 호출
        //받아온 값을 userModels에 넣음.
        public UserlistFragmentRecyclerViewAdapter() {
            userModels = new ArrayList<>();
            FirebaseDatabase.getInstance().getReference().child("USER").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModels.clear();
                    if(user != null){
                        for(DataSnapshot snapshot :dataSnapshot.getChildren()){
                            userModels.add(snapshot.getValue(UserModel.class));
                        }
                    }
                notifyDataSetChanged();
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });


        }

        //리사이클러뷰 뷰 생성
        @Override
        public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_userlist,parent,false);

            return new CustomViewHolder(view);
        }

        //리사이클러뷰의 내용을 넣음.
        @Override
        public void onBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            /*Glide.with
                    (holder.itemView.getContext())
                    // .load(userModels.get(position).profieImageUrl)
                    .load(R.drawable.drawer_menu_users)
                    .apply(new RequestOptions().circleCrop())
                    .into(((CustomViewHolder)holder).imageView);*/

            ((CustomViewHolder)holder).textView.setText(userModels.get(position).userName);

        }

        //필수, 아이템 갯수
        @Override
        public int getItemCount() {
            return userModels.size();
        }

        //커스텀뷰를 위해 필수
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