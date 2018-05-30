package el.kr.ac.dongyang.able;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.RelativeLayout;

import com.skt.Tmap.TMapData;
import com.skt.Tmap.TMapGpsManager;
import com.skt.Tmap.TMapMarkerItem;
import com.skt.Tmap.TMapPOIItem;
import com.skt.Tmap.TMapPoint;
import com.skt.Tmap.TMapPolyLine;
import com.skt.Tmap.TMapView;

import java.util.ArrayList;

import android.support.v4.app.FragmentTransaction;


/**
 * Created by impro on 2018-03-30.
 * 지도 맵 띄움.
 * 출발지 포인트랑 목적지 포인트 받으면 라인이랑 마커 띄움
 * 지도 레벨이 변경될때 마커 크기 유동적으로 바뀌도록 만들어야함
 * 돋보기 버튼 누르면 프래그먼트네비리스트 를 띄움 - 차일드 프래그먼트
 */
public class FragmentNavigation extends android.support.v4.app.Fragment implements  FragmentNaviList.OnChildFragmentInteractionListener {

    private Context mContext = null;
    private boolean m_bTrackingMode = true;
    private TMapGpsManager tmapgps = null;
    private TMapView tMapView = null;
    private static String mApiKey = "2bcf226b-36b6-49da-82cc-5f00acee90a2"; // 발급받은 appKey
    private static int mMarkerID;
    Fragment FragmentNaviList;
    FragmentTransaction transaction;

    private ArrayList<String> mArrayMarkerID = new ArrayList<String>();

    public ArrayList<TMapPOIItem> startList = new ArrayList<TMapPOIItem>();
    public ArrayList<TMapPOIItem> endList = new ArrayList<TMapPOIItem>();

    private static final String LOG_TAG = "FragmentNavigation";

    Button nSearchlist;

    public FragmentNavigation() {
    }

    //네비리스트 프래그먼트(차일드)에서 지점 포인트 받아와서 라인 그림
    @Override
    public void messageFromChildToParent(ArrayList<TMapPOIItem> startList, ArrayList<TMapPOIItem> endList) {
        this.startList = startList;
        this.endList = endList;
        showMarkerPoint(startList,endList);
        Carline carline = new Carline(startList, endList);
        carline.start();
        Log.d("msg : ", "start - " + startList.toString() + " end - " + endList.toString());
        Log.d("distance : ",Double.toString(startList.get(0).getDistance(endList.get(0).getPOIPoint())) + " m");
    }


    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_navigation,container,false);
        getActivity().setTitle("Navigation");

        mContext = getActivity();
        nSearchlist = (Button) view.findViewById(R.id.naviStartEnd);

        nSearchlist.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                FragmentNaviList = new FragmentNaviList();
                transaction = getChildFragmentManager().beginTransaction();
                transaction.add(R.id.child_fragment_container, FragmentNaviList)
                        .addToBackStack("navilist").commit();
            }
        });

        RelativeLayout tmap = (RelativeLayout) view.findViewById(R.id.map_view);
        tMapView = new TMapView(getActivity());

        tMapView.setSKTMapApiKey(mApiKey);
        tMapView.setCompassMode(true);
        tMapView.setZoomLevel(15);
        tMapView.setIconVisibility(true);
        tMapView.setLanguage(TMapView.LANGUAGE_KOREAN);
        //tMapView.setTrackingMode(true);
        tMapView.setSightVisible(true);
        tmap.addView(tMapView);

        setGps();

        return view;
    }

    //두 지점의 포인트에 마커를 찍음
    public void showMarkerPoint(ArrayList<TMapPOIItem> startlist, ArrayList<TMapPOIItem> endlist) {// 마커 찍는거 빨간색 포인트.
        //for (int i = 0; i < m_mapPoint.size(); i++) {
            TMapPoint point = new TMapPoint(startlist.get(0).getPOIPoint().getLatitude(),
                    startlist.get(0).getPOIPoint().getLongitude());
            TMapMarkerItem item1 = new TMapMarkerItem();
            Bitmap bitmap = null;
            bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.location_pointer_red);
            //poi_dot은 지도에 꼽을 빨간 핀 이미지입니다

            item1.setTMapPoint(point);
            item1.setName(startlist.get(0).getPOIName());
            item1.setVisible(item1.VISIBLE);

            item1.setIcon(bitmap);

            //풍선뷰 안의 항목에 글을 지정합니다.
            item1.setCalloutTitle(startlist.get(0).getPOIName());
            //item1.setCalloutSubTitle(list.get(0).getPOIID());
            item1.setCanShowCallout(true);
            item1.setAutoCalloutVisible(true);
            item1.setPosition(0f, 0f); // 마커의 중심점을 중앙, 하단으로 설정

        //서브 이미지
        //Bitmap bitmap_i = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.location_pointer_blue);
            //item1.setCalloutRightButtonImage(bitmap_i);

            String strID = "start";

            tMapView.addMarkerItem(strID, item1);
            mArrayMarkerID.add(strID);

            TMapPoint point2 = new TMapPoint(endlist.get(0).getPOIPoint().getLatitude(),
                    endlist.get(0).getPOIPoint().getLongitude());
            TMapMarkerItem item2 = new TMapMarkerItem();
            Bitmap bitmap2 = null;
            bitmap2 = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.location_pointer_blue);
            //poi_dot은 지도에 꼽을 빨간 핀 이미지입니다

            item2.setTMapPoint(point2);
            item2.setName(endlist.get(0).getPOIName());
            item2.setVisible(item2.VISIBLE);

            item2.setIcon(bitmap2);

            //풍선뷰 안의 항목에 글을 지정합니다.
            item2.setCalloutTitle(endlist.get(0).getPOIName());
            //item1.setCalloutSubTitle(list.get(0).getPOIID());
            item2.setCanShowCallout(true);
            item2.setAutoCalloutVisible(true);

            //String strID2 = String.format("pmarker%d", mMarkerID++);
            String strID2 = "end";

            tMapView.addMarkerItem(strID2, item2);
            mArrayMarkerID.add(strID2);
        //}
    }

    //두 포인트 간에 자동차 라인 그려줌
    public class Carline extends Thread {
        ArrayList<TMapPOIItem> startList = new ArrayList<TMapPOIItem>();
        ArrayList<TMapPOIItem> endList = new ArrayList<TMapPOIItem>();

        public Carline(ArrayList<TMapPOIItem> startList, ArrayList<TMapPOIItem> endList){
            this.startList = startList;
            this.endList = endList;

        }
        public void run() {
            TMapPoint tMapPointStart = new TMapPoint(startList.get(0).getPOIPoint().getLatitude(), startList.get(0).getPOIPoint().getLongitude());
            TMapPoint tMapPointEnd = new TMapPoint(endList.get(0).getPOIPoint().getLatitude(), endList.get(0).getPOIPoint().getLongitude());
            try {
                TMapPolyLine tMapPolyLine = new TMapData().findPathData(tMapPointStart, tMapPointEnd);
                tMapPolyLine.setLineColor(Color.BLUE);
                tMapPolyLine.setLineWidth(2);
                tMapView.addTMapPolyLine("Line1", tMapPolyLine);

            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }

    /*본인 gps 얻어서 맵의 메인에 넣어주는 코드*/
    private final LocationListener mLocationListener = new LocationListener() {
        public void onLocationChanged(Location location) {

            if (location != null) {
                double latitude = location.getLatitude();
                double longitude = location.getLongitude();
                tMapView.setLocationPoint(longitude, latitude);
                tMapView.setCenterPoint(longitude, latitude);

            }

        }

        public void onProviderDisabled(String provider) {
        }

        public void onProviderEnabled(String provider) {
        }

        public void onStatusChanged(String provider, int status, Bundle extras) {
        }
    };

    public void setGps() {
        final LocationManager lm = (LocationManager) getActivity().getSystemService(Context.LOCATION_SERVICE);
        if (ActivityCompat.checkSelfPermission(getActivity(), Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(getActivity(), Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(getActivity(), new String[]{android.Manifest.permission.ACCESS_COARSE_LOCATION, android.Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        }
        lm.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, // 등록할 위치제공자(실내에선 NETWORK_PROVIDER 권장)
                1000, // 통지사이의 최소 시간간격 (miliSecond)
                1, // 통지사이의 최소 변경거리 (m)
                mLocationListener);
    }



    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}