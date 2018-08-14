package el.kr.ac.dongyang.able.navigation;

import android.Manifest;
import android.annotation.SuppressLint;
import android.content.Context;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.ConsoleMessage;
import android.webkit.JavascriptInterface;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import com.skt.Tmap.TMapGpsManager;
import com.skt.Tmap.TMapTapi;
import com.skt.Tmap.TMapView;
import com.squareup.otto.Bus;
import com.squareup.otto.Subscribe;

import java.util.ArrayList;
import java.util.Timer;

import el.kr.ac.dongyang.able.BusProvider;
import el.kr.ac.dongyang.able.MainActivity;
import el.kr.ac.dongyang.able.R;


/**
 * Created by impro on 2018-03-30.
 * 지도 맵 띄움.
 * 출발지 포인트랑 목적지 포인트 받으면 라인이랑 마커 띄움
 */
public class FragmentNavigation extends android.support.v4.app.Fragment {

    WebView web;
    private Handler mHandler = new Handler();
    String fragmentTag;
    FragmentTransaction ft;
    FragmentManager fm;
    String bussett;
    Double startlist[] = new Double[2];
    ProgressBar naviWebLoadingBar;

    private TMapGpsManager tmapgps = null;
    private TMapView tMapView = null;
    private static String mApiKey = "2bcf226b-36b6-49da-82cc-5f00acee90a2"; // 발급받은 appKey
    private static int mMarkerID;

    private ArrayList<String> mArrayMarkerID = new ArrayList<String>();

    public ArrayList<String> naviList = new ArrayList<String>();
    private static final String LOG_TAG = "FragmentNavigation";

    EditText et1;
    Button startBtn, currentBtn;
    ConstraintLayout constLocationInfo;
    double latitude_r,longitude_r;

    private Bus busProvider = BusProvider.getInstance();
    Timer t = new Timer(true);
    TextView nodeText, beforeText, textTime, textDistance;

    public FragmentNavigation() {
    }
    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        BusProvider.getInstance().register(this);
    }

    @Nullable
    @Override
    @SuppressLint("JavascriptInterface")
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_navigation,container,false);
        getActivity().setTitle("Navigation");

        nodeText = view.findViewById(R.id.nodetext);
        beforeText = view.findViewById(R.id.beforeText);
        nodeText.setVisibility(View.GONE);
        beforeText.setVisibility(View.GONE);

        constLocationInfo = view.findViewById(R.id.constLocationInfo);
        textTime = view.findViewById(R.id.textTime);
        textDistance = view.findViewById(R.id.textDistance);
        naviWebLoadingBar = view.findViewById(R.id.naviCircleBar);
        naviWebLoadingBar.setVisibility(View.GONE);
        startBtn = view.findViewById(R.id.startBtn);
        startBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                fm = getActivity().getSupportFragmentManager();
                ft = fm.beginTransaction();
                Fragment currentFragment = getActivity().getSupportFragmentManager().findFragmentByTag("FRAGMENT_NAVIGATION");
                ft.hide(currentFragment);
                ft.commit();
                Toast.makeText(getActivity(), "주행이 시작됩니다.", Toast.LENGTH_SHORT).show();;
            }
        });

        TMapTapi tmaptapi = new TMapTapi(getActivity());
        tmaptapi.setSKTMapAuthentication ("2414ee00-3784-4c78-913d-32bf5fa9c107");

        et1 = view.findViewById(R.id.naviLocation);

        //edittext 밑줄 색 변경
        int color = Color.parseColor("#ffffff");
        et1.getBackground().setColorFilter(color, PorterDuff.Mode.SRC_IN);

        et1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Fragment fragment = new FragmentNaviList();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft=getActivity().getSupportFragmentManager().beginTransaction();
                ft.add(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });
        web = view.findViewById(R.id.web);
        web.setWebViewClient(new WebViewClient(){
            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                super.onPageStarted(view, url, favicon);
                naviWebLoadingBar.setVisibility(View.VISIBLE);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);
                naviWebLoadingBar.setVisibility(View.GONE);
            }
        });
        WebSettings webSet = web.getSettings();
        webSet.setJavaScriptEnabled(true);
        webSet.setUseWideViewPort(true);
        webSet.setBuiltInZoomControls(false);
        webSet.setAllowUniversalAccessFromFileURLs(true);
        webSet.setJavaScriptCanOpenWindowsAutomatically(true);
        webSet.setSupportMultipleWindows(true);
        webSet.setSaveFormData(false);
        webSet.setSavePassword(false);
        webSet.setLayoutAlgorithm(WebSettings.LayoutAlgorithm.SINGLE_COLUMN);
        web.setWebChromeClient(new WebChromeClient(){
            public boolean onConsoleMessage(ConsoleMessage cm) {
                Log.d("MyApplication", cm.message() + " -- From line "
                        + cm.lineNumber() + " of "
                        + cm.sourceId() + ", " + cm);
                return true;
            }
        });
        web.setWebViewClient(new WebViewClient()); //페이지 로딩을 마쳤을 경우 작업
        web.loadUrl("file:///android_asset/index.html"); //웹뷰로드

        web.setHorizontalScrollBarEnabled(false);
        web.setVerticalScrollBarEnabled(false);

        web.getSettings().setLoadWithOverviewMode(true);
        web.getSettings().setUseWideViewPort(true);

        web.getSettings().setJavaScriptEnabled(true); //자바스크립트 허용
        web.addJavascriptInterface(new TMapBridge(),"tmap");

        currentBtn = view.findViewById(R.id.currentBtn);
        currentBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(latitude_r != 0) {
                    web.loadUrl("javascript:geoLo('" + latitude_r + "', '" + longitude_r + "')");
                }
            }
        });

        setGps();

        return view;
    }
    @Override
    public void onStart() {
        super.onStart();
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    private class TMapBridge {
        int i = 0;
        @JavascriptInterface
        public void setMessage(final String arg) {
            mHandler.post(new Runnable() {
                public void run() {
                    naviList.add(i + " : " + arg + "\n");
                    i += 1;
                }
            });
        }
        @JavascriptInterface
        public void setTimeDistance(final String arg){
            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    String timeDistance = arg;
                    String[] tdList = timeDistance.split(",");
                    Log.d("tdList", tdList[0] + " , " + tdList[1]);
                    textTime.setText(tdList[0]);
                    textDistance.setText(tdList[1]);
                }
            });
        }
    }

    /*본인 gps 얻어서 맵의 메인에 넣어주는 코드*/
    private final LocationListener mLocationListener = new LocationListener() {
        public void onLocationChanged(Location location) {
            if (location != null) {
                latitude_r = location.getLatitude();
                longitude_r = location.getLongitude();
                String lonlat = "";
                startlist[0] = longitude_r;
                startlist[1] = latitude_r;
                Log.d("geo", "lat : " + Double.toString(latitude_r) + "lon : " + Double.toString(longitude_r));
                //현재위치 마커생성했는데 계속 변하는건 맞지
                web.loadUrl("javascript:geoLo('" + latitude_r + "', '" + longitude_r + "')");

                String nextPoint = "";
                int latitudeint = (int)(latitude_r * 100000);
                int longitudeint = (int)(longitude_r * 100000);

                if(!naviList.isEmpty()) {
                    String startPoint = "";
                    //처음 한번만
                    if(startPoint.isEmpty()){
                        try {
                            startPoint = naviList.get(0);
                            //셋팅에서 돌기 위한건데 흠..
                            //busProvider.post(startPoint);
                            Thread.sleep(1000);
                        }   catch (InterruptedException e) {
                        }
                    }
                    for(int i=0; i<naviList.size(); i++) {

                        String lonandlat = naviList.get(i);
                        String target = "lon=";
                        int target_num = lonandlat.indexOf(target);
                        String result;
                        result = lonandlat.substring(target_num, (lonandlat.indexOf(",")));
                        String resultlon = result.substring(4);

                        String lonandlat2 = naviList.get(i);
                        String target2 = "lat=";
                        int target_num2 = lonandlat2.indexOf(target2);
                        String result2;
                        result2 = lonandlat2.substring(target_num2);
                        String resultlat = result2.substring(4);

                        int resultlat2p = (int)((Double.parseDouble(resultlat.substring(0,7))+0.00050)*100000);
                        int resultlat2m = (int)((Double.parseDouble(resultlat.substring(0,7))-0.00050)*100000);

                        int resultlon2p = (int)((Double.parseDouble(resultlon.substring(0,8))+0.00050)*100000);
                        int resultlon2m = (int)((Double.parseDouble(resultlon.substring(0,8))-0.00050)*100000);

                        if(latitudeint >= resultlat2m && latitudeint <= resultlat2p ){
                            if(longitudeint >= resultlon2m && longitudeint <= resultlon2p ) {
                                nextPoint = naviList.get(i+1);
                                //busProvider.post(nextPoint);
                                Log.d("otto_lonlat : ", "" + nextPoint);
                                //테스트용 텍스트뷰
                                beforeText.setText(naviList.get(i));
                                nodeText.setText(nextPoint);
                                break;
                            }
                        }
                    }
                }

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
        if (ActivityCompat.checkSelfPermission(getActivity(),
            Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED &&
            ActivityCompat.checkSelfPermission(getActivity(),
            Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED)
        {
                ActivityCompat.requestPermissions(getActivity(),
                new String[]{android.Manifest.permission.ACCESS_COARSE_LOCATION,
                            android.Manifest.permission.ACCESS_FINE_LOCATION}, 1);
        }
        lm.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0,0,mLocationListener);
        lm.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, // 등록할 위치제공자(실내에선 NETWORK_PROVIDER 권장)
                0, // 통지사이의 최소 시간간격 (miliSecond)
                0, // 통지사이의 최소 변경거리 (m)
                mLocationListener);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        BusProvider.getInstance().unregister(this);
    }

    @Subscribe
    public void getPost(String msg) {
        Log.d("otto_lonlat_set : ", "" + msg);
        bussett = msg;
        String[] msglist = bussett.split(",");
        et1.setText(msglist[0]);
        Double[] endlist = {Double.parseDouble(msglist[1]), Double.parseDouble(msglist[2])};
        Log.d("list", startlist[0] + " " + startlist[1] + " " + endlist[0] + " " + endlist[1]);
        web.loadUrl("javascript:distance('" + startlist[0] + "', '" + startlist[1] + "', '" + endlist[0] + "', '" + endlist[1] + "')");
        Log.d("setbus : ", bussett.toString());
        constLocationInfo.setVisibility(tMapView.VISIBLE);
    }
}