package el.kr.ac.dongyang.able.friend;

import android.content.Intent;
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
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

import el.kr.ac.dongyang.able.BaseFragment;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

/**
 * Created by impro on 2018-05-08.
 * 친구추가 버튼으로 유저목록으로 넘어감.
 * 아직 친구에 대한 버튼 이벤트 없음.
 * 현재 on/off 미구현
 * 마지막 접속시간 미구현
 */

public class FragmentFriend extends BaseFragment {

    Button btn, gobtn ,delbtn;
    FragmentTransaction ft;
    String fragmentTag;
    FirebaseUser user;
    String uid;

    public FragmentFriend() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_friend, container, false);
        getActivity().setTitle("Friend");

        ConstraintLayout loginConstraintLayout = view.findViewById(R.id.loginConlayout);

        gobtn = view.findViewById(R.id.go_rank);
        gobtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(view.getContext(), BarChartActivity.class);
                startActivity(intent);
            }
        });
        gobtn.setVisibility(View.GONE);

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

        delbtn = view.findViewById(R.id.delete_friend);
        delbtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Fragment fragment = new FragmentDelFriend();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });
        delbtn.setVisibility(View.GONE);

        RecyclerView recyclerView = view.findViewById(R.id.fragment_recyclerview_friend);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new FriendlistRecyclerViewAdapter());
        recyclerView.setVisibility(View.GONE);

        user = FirebaseAuth.getInstance().getCurrentUser();
        if (user != null) {
            uid = user.getUid();
            btn.setVisibility(View.VISIBLE);
            gobtn.setVisibility(View.VISIBLE);
            delbtn.setVisibility(View.VISIBLE);
            recyclerView.setVisibility(View.VISIBLE);
            loginConstraintLayout.setVisibility(View.GONE);
        }

        return view;
    }

    class FriendlistRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

        private List<String> keys = new ArrayList<>();
        private ArrayList<String> friendUsers = new ArrayList<>();

        public FriendlistRecyclerViewAdapter() {
            reference.child("FRIEND").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                    keys.clear();
                    try {
                        for (DataSnapshot item : dataSnapshot.child(uid).getChildren()) {
                            keys.add(item.getKey());
                        }
                    } catch (NullPointerException e) {
                        e.printStackTrace();
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

            reference.child("USER").child(user).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    UserModel userModel = dataSnapshot.getValue(UserModel.class);
                    Glide.with(customViewHolder.itemView.getContext())
                            .load(userModel.getProfileImageUrl())
                            .apply(new RequestOptions().circleCrop())
                            .into(customViewHolder.imageView);

                    customViewHolder.textView.setText(userModel.getUserName());

                    if (userModel.getComment() != null) {
                        customViewHolder.textView_comment.setText(userModel.getComment());
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
