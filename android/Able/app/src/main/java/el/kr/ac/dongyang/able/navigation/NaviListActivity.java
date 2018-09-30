package el.kr.ac.dongyang.able.navigation;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.text.InputType;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.skt.Tmap.TMapData;
import com.skt.Tmap.TMapPOIItem;
import com.squareup.otto.Bus;

import java.util.ArrayList;
import java.util.List;

import el.kr.ac.dongyang.able.BusProvider;
import el.kr.ac.dongyang.able.R;

public class NaviListActivity extends AppCompatActivity {

    private EditText nEnd;
    private Button searchBtn, startBtn;
    private RecyclerView recyclerView;
    private List<TMapPOIItem> poiList = new ArrayList<>();
    private List<TMapPOIItem> endList = new ArrayList<>();
    private List<String> busitem = new ArrayList<>();
    private ConstraintLayout checkEndPointLayout;
    private NaviListRecyclerViewAdapter recyclerViewAdapter;
    private Handler handler;
    private StringBuffer address = new StringBuffer();

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.fragment_navilist);
        setTitle("목적지 선택");

        getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);

        checkEndPointLayout = findViewById(R.id.checkEndPointlayout);

        //리사이클러뷰 맵핑
        recyclerView = findViewById(R.id.fragment_naviList);
        initRecyclerView();

        nEnd = findViewById(R.id.naviEnd);
        nEnd.setFocusableInTouchMode(true);
        nEnd.requestFocus();
        nEnd.setInputType ( InputType. TYPE_TEXT_FLAG_NO_SUGGESTIONS );
        final InputMethodManager inputMethodManager = (InputMethodManager) getSystemService(INPUT_METHOD_SERVICE);
        inputMethodManager.toggleSoftInput(InputMethodManager.SHOW_FORCED, InputMethodManager.HIDE_IMPLICIT_ONLY);

        //서치버튼
        searchBtn = findViewById(R.id.searhAddr);
        searchBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(nEnd.getText().toString().equals("")) {
                    Toast.makeText(NaviListActivity.this, "목적지를 입력해주세요.", Toast.LENGTH_SHORT).show();
                } else {
                    checkEndPointLayout.setVisibility(View.GONE);
                    startBtn.setVisibility(View.VISIBLE);
                    inputMethodManager.hideSoftInputFromWindow(view.getWindowToken(),0);
                    searchAllPoi();
                }
            }
        });

        handler = new Handler(){
            public void handleMessage(Message msg){
                recyclerViewAdapter.notifyDataSetChanged();
            }
        };

        //목적지 설정
        startBtn = findViewById(R.id.naviListStartBtn);
        startBtn.setVisibility(View.GONE);
        startBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //조건 다르게 변경 필요함
                if(busitem.size() != 0) {

                    Intent intent = new Intent();
                    intent.putExtra("endName", busitem.get(0));
                    intent.putExtra("endLong", busitem.get(1));
                    intent.putExtra("endLat", busitem.get(2));
                    setResult(Activity.RESULT_OK, intent);
                    finish();
                } else {
                    Toast.makeText(NaviListActivity.this, "목적지를 선택해주세요", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    @Override
    public void onResume() {
        super.onResume();
        recyclerViewAdapter.notifyDataSetChanged();
    }

    private void searchAllPoi() {
        final String strDataEnd = nEnd.getText().toString();
        TMapData tMapData = new TMapData();
        poiList.clear();
        tMapData.findAllPOI(strDataEnd, new TMapData.FindAllPOIListenerCallback() {
            @Override
            public void onFindAllPOI(ArrayList<TMapPOIItem> poiItem) {
                for (int i = 0; i < poiItem.size(); i++) {
                    TMapPOIItem item = poiItem.get(i);
                    poiList.add(item);
                    Log.d("주소로찾기", "POI Name: " + item.getPOIName().toString() + ", " +
                            "Address: " + item.getPOIAddress().replace("null", "") + ", " +
                            "Point: " + item.getPOIPoint().toString());
                    Message msg = handler.obtainMessage();
                    handler.sendMessage(msg);
                }
            }
        });
    }

    private void initRecyclerView() {
        recyclerViewAdapter = new NaviListRecyclerViewAdapter();
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        recyclerView.setAdapter(recyclerViewAdapter);
        recyclerViewAdapter.notifyDataSetChanged();
    }

    //어댑터 클래스 - 검색결과 띄움.
    public class NaviListRecyclerViewAdapter extends RecyclerView.Adapter<NaviListRecyclerViewAdapter.ViewHolder> {
        public NaviListRecyclerViewAdapter() {
        }
        //리사이클러뷰 뷰 생성
        @NonNull
        @Override
        public NaviListRecyclerViewAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_navilist,parent,false);
            return new NaviListRecyclerViewAdapter.ViewHolder(view);
        }
        //리사이클러뷰의 내용을 넣음.
        @Override
        public void onBindViewHolder(NaviListRecyclerViewAdapter.ViewHolder holder, final int position) {

            final TMapPOIItem tMapPOIItem = poiList.get(position);
            holder.nameText.setText(tMapPOIItem.getPOIName());

            address.append(tMapPOIItem.upperAddrName + " " + tMapPOIItem.middleAddrName + " " + tMapPOIItem.lowerAddrName);
            if(tMapPOIItem.firstNo != null) address.append(" " + tMapPOIItem.firstNo);
            if(tMapPOIItem.secondNo != null) address.append("-" + tMapPOIItem.secondNo);
            holder.addressText.setText(address);
            address.setLength(0);
            holder.itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    nEnd.setText(tMapPOIItem.getPOIName());
                    endList.clear();
                    busitem.clear();
                    String address = tMapPOIItem.getPOIName();
                    String lon = Double.toString(tMapPOIItem.getPOIPoint().getLongitude());
                    String lat = Double.toString(tMapPOIItem.getPOIPoint().getLatitude());
                    busitem.add(address);
                    busitem.add(lon);
                    busitem.add(lat);
                    for(int i = 0; i<busitem.size(); i++){
                        Log.d("bus", busitem.get(i));
                    }
                    Toast.makeText(NaviListActivity.this, "목적지 선택", Toast.LENGTH_SHORT).show();
                }
            });
        }
        // 필수, 아이템 갯수
        @Override
        public int getItemCount() {
            return poiList.size();
        }

        //커스텀뷰를 위해 필수
        public class ViewHolder extends RecyclerView.ViewHolder {
            public ImageView imageView;
            public TextView nameText, addressText;

            public ViewHolder(View view) {
                super(view);
                imageView = view.findViewById(R.id.frienditem_imageview);
                nameText = view.findViewById(R.id.naviitem_textview_name);
                addressText = view.findViewById(R.id.naviitem_textview_address);
            }
        }
    }
}
