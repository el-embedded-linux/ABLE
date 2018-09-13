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
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.NonNull;
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

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ServerValue;
import com.google.firebase.database.ValueEventListener;
import com.google.gson.Gson;
import com.skt.Tmap.TMapTapi;
import com.skt.Tmap.TMapView;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;

import el.kr.ac.dongyang.able.BaseActivity;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.ChatModel;
import el.kr.ac.dongyang.able.model.NotificationModel;
import el.kr.ac.dongyang.able.model.UserModel;
import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class NavigationActivity extends BaseActivity {

    Map<String, UserModel> users = new HashMap<>();
    private Double startlist[] = new Double[2];
    private double latitude_r, longitude_r;
    private List<String> naviList = new ArrayList<>();
    private List<String> descriptionList = new ArrayList<>();
    private WebView web;
    private Handler mHandler = new Handler();
    private Thread thread;

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
    private String destinationRoom;
    private String uid;

    String address;
    String endLong;
    String endLat;
    private String clickText;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.fragment_navigation);
        setTitle("Navigation");

        reference.child("USER").addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                for (DataSnapshot item : dataSnapshot.getChildren()) {
                    users.put(item.getKey(), item.getValue(UserModel.class));
                }
            }

            @Override
            public void onCancelled(DatabaseError databaseError) {

            }
        });

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
                ChatModel.Comment comment =
                        new ChatModel.Comment(uid, "[ " + address + " ]\n그룹라이딩 공유", ServerValue.TIMESTAMP, true);
                comment.destinationLatitude = endLat;
                comment.destinationLongitude = endLong;
                comment.myLonitude = startlist[0].toString();
                comment.myLatitude = startlist[1].toString();
                comment.destinationAddress = address;

                reference
                        .child("CHATROOMS")
                        .child(destinationRoom)
                        .child("comments")
                        .push()
                        .setValue(comment)
                        .addOnCompleteListener(new OnCompleteListener<Void>() {
                            @Override
                            public void onComplete(@NonNull Task<Void> task) {
                                //gcm 전송
                                sendGcmUsers();
                            }
                        });
                //그룹라이딩에서 값을 통해서 방향지시 해주고 싶다면 기본값 1로 저장하고
                //숫자를 디비에 넣고 리스너로 계속 받아주면 되겠네
                finish();
            }
        });

        Intent intent = getIntent();
        clickText = intent.getStringExtra("clickBtn");
        if (clickText.equals("share")) {
            uid = intent.getStringExtra("uid");
            destinationRoom = intent.getStringExtra("destinationRoom");
        }

        try {
            switch (clickText) {
                case "start":
                    endConstraintLayout.setVisibility(View.GONE);
                    shareBtn.setVisibility(View.GONE);
                    break;
                case "end":
                    startConstraintLayout.setVisibility(View.GONE);
                    break;
                case "share":
                    endConstraintLayout.setVisibility(View.GONE);
                    startBtn.setVisibility(View.GONE);
                    shareBtn.setVisibility(View.VISIBLE);
                    break;
                case "shareStart":
                    endConstraintLayout.setVisibility(View.GONE);
                    shareBtn.setVisibility(View.GONE);
                    Object commentOb = intent.getSerializableExtra("comment");
                    ChatModel.Comment comment = (ChatModel.Comment) commentOb;
                    web.loadUrl(
                            "javascript:distance('" +
                                    comment.myLonitude + "', '" +
                                    comment.myLatitude + "', '" +
                                    comment.destinationLongitude + "', '" +
                                    comment.destinationLatitude + "')");
                    searchAddressEditText.setText(comment.destinationAddress);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        startBtn = findViewById(R.id.startBtn);
        startBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(NavigationActivity.this, "주행이 시작됩니다.", Toast.LENGTH_SHORT).show();
                startConstraintLayout.setVisibility(View.GONE);
                endConstraintLayout.setVisibility(View.VISIBLE);
                if(clickText.equals("shareStart")){

                    descriptionChange(0);
                }
                startNotification();
            }
        });

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

        new Thread(new Runnable() {
            @Override
            public void run() {
                setGps();
            }
        }).start();
    }

    public void sendGcmUsers() {
        reference
                .child("CHATROOMS")
                .child(destinationRoom)
                .child("users")
                .addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                Map<String, Boolean> map = (Map<String, Boolean>) dataSnapshot.getValue();

                for (String item : map.keySet()) {
                    if (item.equals(uid)) {
                        continue;
                    }
                    gcmSetting(users.get(item).getPushToken());
                }
            }

            @Override
            public void onCancelled(DatabaseError databaseError) {
            }
        });
    }

    void gcmSetting(String pushToken) {

        Gson gson = new Gson();

        String userName = FirebaseAuth.getInstance().getCurrentUser().getDisplayName();
        NotificationModel notificationModel = new NotificationModel();
        notificationModel.to = pushToken;
        notificationModel.notification.title = userName;
        notificationModel.notification.text = "[ " + searchAddressEditText.getText().toString() + " ]\n그룹라이딩 시작";
        notificationModel.data.title = userName;
        notificationModel.data.text = "[ " + searchAddressEditText.getText().toString() + " ]\n그룹라이딩 시작";

        RequestBody requestBody = RequestBody.create(MediaType.parse("application/json; charset=utf8"), gson.toJson(notificationModel));

        Request request = new Request.Builder()
                .header("Content-Type", "application/json")
                .addHeader("Authorization", "key=AIzaSyAXArVX1TeAhf2L9MNlTuKgumJgPK1Y0BU")
                .url("https://gcm-http.googleapis.com/gcm/send")
                .post(requestBody)
                .build();
        OkHttpClient okHttpClient = new OkHttpClient();
        okHttpClient.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
            }
        });
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
        thread = new Thread(new Runnable() {
            @Override
            public void run() {
                StringTokenizer st = new StringTokenizer(descriptionList.get(position));
                int countTokens = st.countTokens();
                for (int i = 0; i < countTokens; i++) {
                    final String token = st.nextToken();
                    final char meter = token.charAt(token.length() - 1);
                    handlerDirectionJob(token, Character.toString(meter));
                }
            }
        });
        thread.start();
    }

    private void handlerDirectionJob(final String text, final String meter) {
        mHandler.post(new Runnable() {
            @Override
            public void run() {
                if (text.equals("좌회전"))
                    directionImg.setImageResource(R.drawable.arrow_left);
                else if (text.equals("우회전"))
                    directionImg.setImageResource(R.drawable.arrow_right);
                else if (meter.equals("m")) textDirection.setText(text);
                else directionImg.setImageResource(R.drawable.arrow_up_straight);
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
            address = data.getStringExtra("endName");
            endLong = data.getStringExtra("endLong");
            endLat = data.getStringExtra("endLat");
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
