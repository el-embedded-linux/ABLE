package el.kr.ac.dongyang.able.navigation;

import android.content.Context;
import android.graphics.Color;
import android.support.annotation.NonNull;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.RequestOptions;

import java.util.ArrayList;
import java.util.List;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.UserModel;

// Navigation Activity 에서 그룹라이딩시 우측 드로우어에 유저리스트를 추가하는 recyclerView 어댑터
public class SideUserAdapter extends RecyclerView.Adapter<SideUserAdapter.ViewHolder> {

    private Context context;
    private List<UserModel> list = new ArrayList<>();

    public SideUserAdapter(Context context) {
        this.context = context;
    }

    public void add(UserModel userModel) {
        this.list.add(userModel);
    }

    public void listClear() {
        list.clear();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        LayoutInflater inflater = LayoutInflater.from(parent.getContext());
        View view = inflater.inflate(R.layout.item_userlist, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        UserModel item = list.get(position);
        Glide.with(holder.itemView)
                .load(item.getProfileImageUrl())
                .apply(new RequestOptions().circleCrop())
                .into(holder.imageView);
        holder.userName.setText(item.getUserName());
    }

    @Override
    public int getItemCount() {
        return list.size();
    }

    public class ViewHolder extends RecyclerView.ViewHolder{
        public ImageView imageView;
        public TextView userName;
        public TextView comment;

        public ViewHolder(View itemView) {
            super(itemView);
            imageView = itemView.findViewById(R.id.frienditem_imageview);
            userName = itemView.findViewById(R.id.frienditem_textview);
            userName.setTextColor(Color.BLACK);
            comment = itemView.findViewById(R.id.frienditem_textview_comment);
            comment.setVisibility(View.GONE);
        }
    }
}
