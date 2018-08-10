package el.kr.ac.dongyang.able.navigation;

import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.text.InputType;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
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
import el.kr.ac.dongyang.able.RecyclerItemClickListener;

import static android.content.Context.INPUT_METHOD_SERVICE;


/**
 * Created by impro on 2018-05-23.
 * 검색해서 리스트 띄워주고 클릭시 설정됨
 * 키보드를 내려줘야 뜸.. 문제..
 * 검색결과에 null도 붙어서 나온다..
 *
 */

public class FragmentNaviList extends android.support.v4.app.Fragment {

    EditText nStart, nEnd;
    Button nSearch;
    RecyclerView recyclerView;
    private ArrayList<TMapPOIItem> poiList = new ArrayList<TMapPOIItem>();
    private ArrayList<TMapPOIItem> startList = new ArrayList<TMapPOIItem>();
    private ArrayList<TMapPOIItem> endList = new ArrayList<TMapPOIItem>();
    public List<String> busitem = new ArrayList<>();
    ConstraintLayout checkEndPoint;

    InputMethodManager imm;
    private int requestCode;
    private Bus busProvider = BusProvider.getInstance();

    public FragmentNaviList(){
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_navilist,container,false);

        checkEndPoint = view.findViewById(R.id.checkEndPoint);

        //리사이클러뷰 맵핑
        recyclerView = view.findViewById(R.id.fragment_naviList);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));
        recyclerView.setAdapter(new FragmentNaviList.NavilistFragmentRecyclerViewAdapter());

        //터치이벤트
        recyclerView.addOnItemTouchListener(new RecyclerItemClickListener(getActivity(), recyclerView, new RecyclerItemClickListener.OnItemClickListener() {
            @Override
            public void onItemClick(View view, int position) {
                nEnd.setText(poiList.get(position).getPOIName());
                endList.clear();
                busitem.clear();
                String address = poiList.get(position).getPOIName().toString();
                String lon = Double.toString(poiList.get(position).getPOIPoint().getLongitude());
                String lat = Double.toString(poiList.get(position).getPOIPoint().getLatitude());
                busitem.add(address);
                busitem.add(lon);
                busitem.add(lat);
                for(int i = 0; i<busitem.size(); i++){
                    Log.d("bus", busitem.get(i).toString());
                }
                Toast.makeText(getActivity(), "목적지 선택", Toast.LENGTH_SHORT).show();
            }
            @Override
            public void onLongItemClick(View view, int position) {
            }
        }));

        nEnd = view.findViewById(R.id.naviEnd);
        nEnd.setFocusableInTouchMode(true);
        nEnd.requestFocus();
        nEnd.setInputType ( InputType. TYPE_TEXT_FLAG_NO_SUGGESTIONS );
        InputMethodManager imm = (InputMethodManager) getContext().getSystemService(INPUT_METHOD_SERVICE);
        imm.toggleSoftInput(InputMethodManager.SHOW_FORCED, InputMethodManager.HIDE_IMPLICIT_ONLY);

        //목적지 에디트텍스트뷰
        nEnd.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View view, int keyCode, KeyEvent keyEvent) {
                //Enter key Action
                if (keyCode == KeyEvent.KEYCODE_ENTER) {
                    checkEndPoint.setVisibility(View.GONE);
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
                            }
                        }
                    });
                    recyclerView.removeAllViewsInLayout();
                    recyclerView.setAdapter(new FragmentNaviList.NavilistFragmentRecyclerViewAdapter());
                    return true;
                }
                return false;
            }
        });
        //서치버튼
        nSearch = view.findViewById(R.id.searhAddr);
        nSearch.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                busProvider.post(busitem.get(0) + "," + busitem.get(1) + "," + busitem.get(2));
                //프래그먼트 종료됨
                getFragmentManager().beginTransaction().remove(FragmentNaviList.this).commitNow();
            }
        });
        return view;
    }

   //어댑터 클래스 - 검색결과 띄움.
    public class NavilistFragmentRecyclerViewAdapter extends RecyclerView.Adapter<NavilistFragmentRecyclerViewAdapter.ViewHolder> {
        public NavilistFragmentRecyclerViewAdapter() {
        }
       //리사이클러뷰 뷰 생성
        @Override
        public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_navilist,parent,false);
            return new ViewHolder(view);
        }
        //리사이클러뷰의 내용을 넣음.
        @Override
        public void onBindViewHolder(ViewHolder holder, int position) {
            holder.nameText.setText(poiList.get(position).getPOIName());
            holder.addressText.setText(poiList.get(position).getPOIAddress());
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
                //imageView = view.findViewById(R.id.frienditem_imageview);
                nameText = view.findViewById(R.id.naviitem_textview_name);
                addressText = view.findViewById(R.id.naviitem_textview_address);
            }
        }
    }
}
