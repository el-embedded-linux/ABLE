package el.kr.ac.dongyang.able.navigation;

import android.Manifest;
import android.app.AlertDialog;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.media.Image;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.Nullable;
import android.support.constraint.ConstraintLayout;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.NotificationCompat;
import android.util.Log;
import android.view.View;
import android.webkit.ConsoleMessage;
import android.webkit.JavascriptInterface;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.RemoteViews;
import android.widget.TextView;
import android.widget.Toast;

import com.skt.Tmap.TMapTapi;
import com.skt.Tmap.TMapView;

import java.util.ArrayList;
import java.util.List;
import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.BaseActivity;
import el.kr.ac.dongyang.able.R;

public class NavigationActivity extends BaseActivity {

    private Double startlist[] = new Double[2];
    private double latitude_r, longitude_r;
    private TMapView tMapView = null;
    private List<String> naviList = new ArrayList<>();
    private List<String> descriptionList = new ArrayList<>();
    private WebView web;
    private Handler mHandler = new Handler();

    private EditText searchAddressEditText;
    private Button startBtn, currentBtn, endBtn, shareBtn;
    private ConstraintLayout constLocationInfo, endConstraintLayout, startConstraintLayout;
    private TextView nodeText, beforeText, textTime, textDistance, textDirection;
    private ProgressBar naviWebLoadingBar;
    private ImageView directionImg;

    private NotificationManager notificationManager;
    private RemoteViews contentView;
    private NotificationCompat.Builder mBuilder;
    private static final int NAVILIST_CODE = 3000;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.fragment_navigation);
        setTitle("Navigation");

        nodeText = findViewById(R.id.nodetext);
        beforeText = findViewById(R.id.beforeText);
        nodeText.setVisibility(View.GONE);
        beforeText.setVisibility(View.GONE);

        textDirection = findViewById(R.id.directionTextView);
        directionImg = findViewById(R.id.directionImageView);

        constLocationInfo = findViewById(R.id.constLocationInfo);
        constLocationInfo.setVisibility(View.GONE);
        endConstraintLayout = findViewById(R.id.endConstraintLayout);
        startConstraintLayout = findViewById(R.id.startConstraintLayout);

        textTime = findViewById(R.id.textTime);
        textDistance = findViewById(R.id.textDistance);
        naviWebLoadingBar = findViewById(R.id.naviCircleBar);
        naviWebLoadingBar.setVisibility(View.GONE);
        startBtn = findViewById(R.id.startBtn);
        startBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(NavigationActivity.this, "주행이 시작됩니다.", Toast.LENGTH_SHORT).show();
                startConstraintLayout.setVisibility(View.GONE);
                endConstraintLayout.setVisibility(View.VISIBLE);
                startNotification();
            }
        });
        endBtn = findViewById(R.id.endBtn);
        endBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //다이얼로그 생성
                new AlertDialog.Builder(NavigationActivity.this)
                        .setTitle("종료하시겠습니까?")
                        .setPositiveButton("확인", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialogInterface, int i) {
                                //종료했을때 운동데이터 저장(거리, 속도, 시간, 칼로리
                                //노티바 제거
                                notificationManager.cancel(1);
                                //액티비티 종료
                                finish();
                            }
                        })
                        .setNegativeButton("취소", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialogInterface, int i) {
                                dialogInterface.cancel();
                            }
                        })
                        .create()
                        .show();
            }
        });
        shareBtn = findViewById(R.id.shareBtn);
        shareBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //메세지를 저장해야하는데 문자열로 저장하면 되겠지, 파베에 저장
                //신도림역 좌표 좌표 클릭버튼
                //뷰어댑터를 하나 더 만들어야하나 아니면 그냥 안보이던걸 보이게 하는게 낫겠다
                finish();
            }
        });

        Intent intent = getIntent();
        String clickText = intent.getStringExtra("clickBtn");

        try {
            switch (clickText) {
                case "start":
                    //startConstraintLayout.setVisibility(View.VISIBLE);
                    endConstraintLayout.setVisibility(View.GONE);
                    shareBtn.setVisibility(View.GONE);
                    break;
                case "end":
                    //endConstraintLayout.setVisibility(View.VISIBLE);
                    startConstraintLayout.setVisibility(View.GONE);
                    break;
                case "share":
                    //startConstraintLayout.setVisibility(View.VISIBLE);
                    endConstraintLayout.setVisibility(View.GONE);
                    startBtn.setVisibility(View.GONE);
                    break;
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        TMapTapi tmaptapi = new TMapTapi(this);
        tmaptapi.setSKTMapAuthentication("2414ee00-3784-4c78-913d-32bf5fa9c107");

        searchAddressEditText = findViewById(R.id.naviLocation);
        //edittext 밑줄 색 변경
        int color = Color.parseColor("#ffffff");
        searchAddressEditText.getBackground().setColorFilter(color, PorterDuff.Mode.SRC_IN);
        searchAddressEditText.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(NavigationActivity.this, NaviListActivity.class);
                startActivityForResult(intent, NAVILIST_CODE);
            }
        });

        web = findViewById(R.id.web);
        initWeb();

        currentBtn = findViewById(R.id.currentBtn);
        currentBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (latitude_r != 0) {
                    web.loadUrl("javascript:geoLo('" + latitude_r + "', '" + longitude_r + "')");
                } else {
                    toastText("현재 위치가 없습니다.");
                }
            }
        });

        setGps();

    }

    private void startNotification() {
        String id = "my_channel_01";
        CharSequence name = "test";

        notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        int importance = 0;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.N) {
            importance = NotificationManager.IMPORTANCE_LOW;
        }
        NotificationChannel mChannel;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            mChannel = new NotificationChannel(id, name, importance);
            mChannel.enableLights(true);
            mChannel.setLightColor(Color.RED);
            mChannel.enableVibration(true);
            notificationManager.createNotificationChannel(mChannel);
        }
        Intent intent2 = new Intent(NavigationActivity.this, NavigationActivity.class);
        PendingIntent pintent2 = PendingIntent.getActivity(this, 0, intent2, PendingIntent.FLAG_UPDATE_CURRENT);

        contentView = new RemoteViews(this.getPackageName(), R.layout.remoteview);
        contentView.setTextViewText(R.id.remoteViewEndPoint, searchAddressEditText.getText().toString());

        mBuilder = new NotificationCompat.Builder(this)
                .setSmallIcon(R.drawable.playstore_icon_null)
                .setContent(contentView)
                .setChannelId(id)
                .setContentIntent(pintent2);

        notificationManager.notify(1, mBuilder.build());
    }

    private void initWeb() {
        web.setWebViewClient(new WebViewClient() {
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
        web.setWebChromeClient(new WebChromeClient() {
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
        web.addJavascriptInterface(new NavigationActivity.TMapBridge(), "tmap");
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
        public void setTimeDistance(final String arg) {
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

        @JavascriptInterface
        public void setDescription(final String arg) {
            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    descriptionList.add(i + " : " + arg);
                    Log.d("description", i + arg);
                }
            });
        }
    }

    /*본인 gps 얻어서 맵의 메인에 넣어주는 코드*/
    private LocationListener mLocationListener = new LocationListener() {
        public void onLocationChanged(Location location) {
            if (location != null) {
                latitude_r = location.getLatitude();
                longitude_r = location.getLongitude();
                String lonlat = "";
                startlist[0] = longitude_r;
                startlist[1] = latitude_r;
                Log.d("geo", "lat : " + Double.toString(latitude_r) + ", lon : " + Double.toString(longitude_r));
                //현재위치 마커생성했는데 계속 변하는건 맞지
                web.loadUrl("javascript:geoLo('" + latitude_r + "', '" + longitude_r + "')");

                String nextPoint = "";
                int latitudeint = (int) (latitude_r * 100000);
                int longitudeint = (int) (longitude_r * 100000);

                if (!naviList.isEmpty()) {
                    String startPoint = "";
                    descriptionChange(0);
                    //처음 한번만
                    if (startPoint.isEmpty()) {
                        try {
                            startPoint = naviList.get(0);
                            //셋팅에서 돌기 위한건데 흠..
                            //busProvider.post(startPoint);
                            Thread.sleep(1000);
                        } catch (InterruptedException e) {
                        }
                    }
                    for (int i = 0; i < naviList.size(); i++) {

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

                        int resultlat2p = (int) ((Double.parseDouble(resultlat.substring(0, 7)) + 0.00050) * 100000);
                        int resultlat2m = (int) ((Double.parseDouble(resultlat.substring(0, 7)) - 0.00050) * 100000);

                        int resultlon2p = (int) ((Double.parseDouble(resultlon.substring(0, 8)) + 0.00050) * 100000);
                        int resultlon2m = (int) ((Double.parseDouble(resultlon.substring(0, 8)) - 0.00050) * 100000);

                        if (latitudeint >= resultlat2m && latitudeint <= resultlat2p) {
                            if (longitudeint >= resultlon2m && longitudeint <= resultlon2p) {
                                nextPoint = naviList.get(i + 1);
                                //busProvider.post(nextPoint);
                                Log.d("otto_lonlat : ", "" + nextPoint);
                                //테스트용 텍스트뷰
                                beforeText.setText(naviList.get(i));
                                nodeText.setText(nextPoint);

                                //endConstraintLayout 수정
                                descriptionChange(i);

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

    private void descriptionChange(final int position) {
        mHandler.post(new Runnable() {
            @Override
            public void run() {
                StringTokenizer st = new StringTokenizer(descriptionList.get(position));
                int countTokens = st.countTokens();
                for (int i = 0; i < countTokens; i++) {
                    String token = st.nextToken();
                    char meter = token.charAt(token.length()-1);
                    if(token.equals("좌회전")) directionImg.setImageResource(R.drawable.arrow_left);
                    else if(token.equals("우회전")) directionImg.setImageResource(R.drawable.arrow_right);
                    else directionImg.setImageResource(R.drawable.arrow_up_straight);
                    if(Character.toString(meter).equals("m")) {
                        textDirection.setText(token);
                    }
                }
            }
        });
    }

    public void setGps() {
        final LocationManager lm = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
        String fineLocation = Manifest.permission.ACCESS_FINE_LOCATION;
        String CoarseLocation = Manifest.permission.ACCESS_COARSE_LOCATION;

        if (ActivityCompat.checkSelfPermission(this, fineLocation) != PackageManager.PERMISSION_GRANTED &&
                ActivityCompat.checkSelfPermission(this, CoarseLocation) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{CoarseLocation, fineLocation}, 1);
        }
        lm.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000, 0, mLocationListener);
        lm.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, // 등록할 위치제공자(실내에선 NETWORK_PROVIDER 권장)
                1000, // 통지사이의 최소 시간간격 (miliSecond)
                0, // 통지사이의 최소 변경거리 (m)
                mLocationListener);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == NAVILIST_CODE && resultCode == RESULT_OK) {
            String address = data.getStringExtra("endName");
            String endLong = data.getStringExtra("endLong");
            String endLat = data.getStringExtra("endLat");
            searchAddressEditText.setText(address);
            web.loadUrl("javascript:distance('" + startlist[0] + "', '" + startlist[1] + "', '" + Double.parseDouble(endLong) + "', '" + Double.parseDouble(endLat) + "')");
            endConstraintLayout.setVisibility(View.GONE);
            startConstraintLayout.setVisibility(View.VISIBLE);
            constLocationInfo.setVisibility(View.VISIBLE);
        }
    }

    @Override
    protected void onDestroy() {
        LocationManager lm = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
        lm.removeUpdates(mLocationListener);
        mLocationListener = null;
        super.onDestroy();
    }

    @Override
    public void onBackPressed() {
        new AlertDialog.Builder(this)
                .setTitle("네비게이션을 종료하시겠습니까?")
                .setPositiveButton("확인", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        //중간에 꺼도 데이터 저장 필요
                        NavigationActivity.super.onBackPressed();
                    }
                })
                .setNegativeButton("취소", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        dialogInterface.cancel();
                    }
                })
                .create()
                .show();
    }
}
