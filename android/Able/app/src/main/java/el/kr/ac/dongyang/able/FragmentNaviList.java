package el.kr.ac.dongyang.able;

import android.content.Context;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
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

import java.util.ArrayList;


/**
 * Created by impro on 2018-05-23.
 * 네비게이션 프래그먼트 안에 들어가는 자식 프래그먼트
 * 검색해서 리스트 띄워주고 클릭시 설정됨
 * 키보드를 내려줘야 뜸.. 문제..
 * 검색결과에 null도 붙어서 나온다..
 *
 */

public class FragmentNaviList extends Fragment {

    EditText nStart, nEnd;
    Button nSearch;
    RecyclerView recyclerView;
    private ArrayList<TMapPOIItem> poiList = new ArrayList<TMapPOIItem>();
    private ArrayList<TMapPOIItem> startList = new ArrayList<TMapPOIItem>();
    private ArrayList<TMapPOIItem> endList = new ArrayList<TMapPOIItem>();
    private OnChildFragmentInteractionListener mParentListener;

    InputMethodManager imm;
    private int requestCode;

    public FragmentNaviList(){

    }

    public interface OnChildFragmentInteractionListener {
        void messageFromChildToParent(ArrayList<TMapPOIItem> startList, ArrayList<TMapPOIItem> endList);
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);

        // check if parent Fragment implements listener
        if (getParentFragment() instanceof OnChildFragmentInteractionListener) {
            mParentListener = (OnChildFragmentInteractionListener) getParentFragment();
        } else {
            throw new RuntimeException(context.toString()
                    + " must implement OnChildFragmentInteractionListener");
        }
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_navilist,container,false);

        //리사이클러뷰 맵핑
        recyclerView = (RecyclerView)view.findViewById(R.id.fragment_naviList);
        recyclerView.setLayoutManager(new LinearLayoutManager(inflater.getContext()));

        //가상키보드 제어
        //imm = (InputMethodManager) getContext().getSystemService(INPUT_METHOD_SERVICE);


        nStart = (EditText) view.findViewById(R.id.naviStart);
        nStart.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
            }
        });

        nStart.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {
                //Enter key Action
                if (keyCode == KeyEvent.KEYCODE_ENTER) {
                    //Enter키 눌렀을 때 처리
                    requestCode = 1;
                    //가상키보드 내리기 - 내리는건 되는데 바로 뷰에 리스트가 뜨지 않음
                    //imm.hideSoftInputFromWindow(nStart.getWindowToken(),0);

                    final String strDataStart = nStart.getText().toString();
                    TMapData tMapData = new TMapData();
                    poiList.clear();

                    tMapData.findAllPOI(strDataStart, new TMapData.FindAllPOIListenerCallback() {
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

                    //터치이벤트
                    //리퀘스트코드 1일땐 출발지에 값을 저장하고, 다른값일땐 목적지에 저장함
                    recyclerView.addOnItemTouchListener(new RecyclerItemClickListener(getActivity(), recyclerView, new RecyclerItemClickListener.OnItemClickListener() {
                        @Override
                        public void onItemClick(View view, int position) {
                            if (requestCode == 1) {
                                nStart.setText(poiList.get(position).getPOIName());
                                startList.clear();
                                startList.add(poiList.get(position));
                                Toast.makeText(getActivity(), "출발지 선택", Toast.LENGTH_SHORT).show();
                            } else {
                                nEnd.setText(poiList.get(position).getPOIName());
                                endList.clear();
                                endList.add(poiList.get(position));
                                Toast.makeText(getActivity(), "목적지 선택", Toast.LENGTH_SHORT).show();
                            }
                        }
                        @Override
                        public void onLongItemClick(View view, int position) {

                        }
                    }));
                    return true;
                }
                return false;
            }
        });

        nEnd = (EditText) view.findViewById(R.id.naviEnd);
        nEnd.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
            }
        });

        //목적지 에디트텍스트뷰
        nEnd.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View view, int keyCode, KeyEvent keyEvent) {
                //Enter key Action
                if (keyCode == KeyEvent.KEYCODE_ENTER) {
                    //Enter키 눌렀을 때 처리
                    requestCode = 0;
                    //가상키보드 내리기
                    //imm.hideSoftInputFromWindow(nStart.getWindowToken(),0);

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
        nSearch = (Button) view.findViewById(R.id.naviSearch);
        nSearch.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //패런트 프래그먼트에 값을 보내줌
                mParentListener.messageFromChildToParent(startList, endList);
                //프래그먼트 종료됨
                getFragmentManager().beginTransaction().remove(FragmentNaviList.this).commitNow();
            }
        });

        //onAttachToParentFragment(getParentFragment());

        return view;
    }

   //어댑터 클래스 - 검색결과 띄움.
    class NavilistFragmentRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

        //userModels라는 리스트를 만들고 addValueEventListerner와 DataSnapshot을 이용해 데이터 호출
        //받아온 값을 userModels에 넣음.
        public NavilistFragmentRecyclerViewAdapter() {
            /*userModels = new ArrayList<>();
            FirebaseDatabase.getInstance().getReference().child("USER").addValueEventListener(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModels.clear();
                    for(DataSnapshot snapshot :dataSnapshot.getChildren()){
                        userModels.add(snapshot.getValue(UserModel.class));
                    }
                    notifyDataSetChanged();

                }

                @Override
                public void onCancelled(DatabaseError databaseError) {

                }
            });
*/

        }

        //리사이클러뷰 뷰 생성
        @Override
        public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_navilist,parent,false);

            return new FragmentNaviList.NavilistFragmentRecyclerViewAdapter.CustomViewHolder(view);
        }

        //리사이클러뷰의 내용을 넣음.
        @Override
        public void onBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            /*Glide.with
                    (holder.itemView.getContext())
                    // .load(userModels.get(position).profieImageUrl)
                    .load(R.drawable.users)
                    .apply(new RequestOptions().circleCrop())
                    .into(((CustomViewHolder)holder).imageView);*/

            ((FragmentNaviList.NavilistFragmentRecyclerViewAdapter.CustomViewHolder)holder).nameText.setText(poiList.get(position).getPOIName());
            ((FragmentNaviList.NavilistFragmentRecyclerViewAdapter.CustomViewHolder)holder).addressText.setText(poiList.get(position).getPOIAddress());

        }

        //필수, 아이템 갯수
        @Override
        public int getItemCount() {
            return poiList.size();
        }

        //커스텀뷰를 위해 필수
        private class CustomViewHolder extends RecyclerView.ViewHolder {
            public ImageView imageView;
            public TextView nameText, addressText;

            public CustomViewHolder(View view) {
                super(view);
                //imageView = (ImageView) view.findViewById(R.id.frienditem_imageview);
                nameText = (TextView) view.findViewById(R.id.naviitem_textview_name);
                addressText = (TextView) view.findViewById(R.id.naviitem_textview_address);
            }
        }
    }
}
