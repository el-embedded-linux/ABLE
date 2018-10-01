package el.kr.ac.dongyang.able.setting;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.location.LocationManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.AlertDialog;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.ValueEventListener;
import com.squareup.otto.Subscribe;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Calendar;
import java.util.Iterator;
import java.util.Set;
import java.util.UUID;

import el.kr.ac.dongyang.able.BaseFragment;
import el.kr.ac.dongyang.able.BusProviderSetting;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.HealthModel;
import el.kr.ac.dongyang.able.model.UserModel;

import static java.lang.System.exit;

//블루투스 통신과 회원정보 수정 기능을 가진 프래그먼트
public class FragmentSetting extends BaseFragment {

    Double speed;
    Double weight;
    FirebaseUser user;
    String uid;
    UserModel userModel;
    HealthModel healthModel;
    CalckThread calcul;
    String cal2;
    Double bykg = 10.0;
    Double MET = 3.3;
    String date;
    int day, month, year;

    Switch bluetoothSwitch;
    Switch gpsSwitch;

    LocationManager locationManager;

    Calendar cal;

    public String lonlat = "100";

    Button infoModify;
    FragmentTransaction ft;
    String fragmentTag;

    //블루투스
    private final int REQUEST_BLUETOOTH_ENABLE = 100;
    ConnectedTask mConnectedTask = null;
    static BluetoothAdapter mBluetoothAdapter;
    private String mConnectedDeviceName = null;
    static boolean isConnectionError = false;
    private static final String TAG = "BluetoothClient";
    private double distance;

    public FragmentSetting() {}

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        BusProviderSetting.getInstance().register(this);
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_setting, container, false);
        getActivity().setTitle("Setting");

        user = FirebaseAuth.getInstance().getCurrentUser();
        if (user != null) {
            uid = user.getUid(); //로그인 된 상태라면 내 uid 가져옴
        } else { //아니라면 홈에서 로그인이 필요하다고 토스트
            FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
            fragmentManager.beginTransaction().remove(FragmentSetting.this).commit();
            fragmentManager.popBackStack();
            Toast.makeText(getActivity(), "로그인이 필요합니다.", Toast.LENGTH_SHORT).show();
        }

        userModel = new UserModel();
        healthModel = new HealthModel();

        cal = Calendar.getInstance(); //
        day = cal.get(Calendar.DAY_OF_MONTH);
        month = cal.get(Calendar.MONTH) + 1;
        year = cal.get(Calendar.YEAR);
        date = year + "-" + month + "-" + day;

        gpsSwitch = view.findViewById(R.id.gps_switch);
        locationManager = (LocationManager) getActivity().getSystemService(Context.LOCATION_SERVICE); //위치 좌표를 받아오기 위한 Location Provider
        if(locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) { //GPS 사용 유무 확인,
            gpsSwitch.setChecked(true); //사용가능하다면 체크 상태
        } else {
            gpsSwitch.setChecked(false); //아니라면 체크 해제 상태
        }
        gpsSwitch.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() { //GPS체커의 상태가 바뀌면
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean isChecked) {
                Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS); //GPS설정으로 이동
                intent.addCategory(Intent.CATEGORY_DEFAULT);
                startActivity(intent);
            }
        });

        bluetoothSwitch = view.findViewById(R.id.bluetooth_switch);
        bluetoothSwitch.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean isChecked) {
                if (isChecked) {
                    mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
                    if (mBluetoothAdapter == null) {//null을 반환하는 경우 기기는 블루투스를 지원하지 않는다
                        //showErrorDialog("This device is not implement Bluetooth.");
                        exit(1);
                    }

                    if (!mBluetoothAdapter.isEnabled()) { // 블루투스 활성화. isEnabled를 호출해 블투가 활성화 되었는지 확인.
                        // false반환시 비활성화.
                        Intent intent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE); //ACTION_REQUEST_ENABLE 를 사용해
                        startActivityForResult(intent, REQUEST_BLUETOOTH_ENABLE); // startActivityForResult를 호출. 그러면 블투 활성화 요청이 발급.
                    } else {
                        Log.d(TAG, "Initialisation successful.");
                        showPairedDevicesListDialog();
                    }
                }
            }
        });

        infoModify = view.findViewById(R.id.info_modify); //내정보 수정 버튼
        infoModify.setOnClickListener(new View.OnClickListener() { //클릭시 내정보 수정 프래그먼트로
            @Override
            public void onClick(View view) {
                Fragment fragment = new FragmentInformation();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft = getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });
        return view;
    }

    @Override
    public void onStart() {
        super.onStart();

        if (uid != null) { //로그인시
            reference.child("USER").child(uid).addValueEventListener(new ValueEventListener() { //파이어베이스 데이터베이스에서 정보 가져옴
                @Override
                public void onDataChange(DataSnapshot dataSnapshot) {
                    userModel = dataSnapshot.getValue(UserModel.class);
                    if (userModel != null && userModel.getWeight() != null) { //usermodel, 체중값이 null이 아닐시
                        weight = Double.parseDouble(userModel.getWeight()); // 받아옴
                        Log.d(TAG, "start - " + weight);
                    }
                }

                @Override
                public void onCancelled(DatabaseError databaseError) {
                }
            });
        }
    }

    //여기서 데이터 보낼수 있음.. 하ㅠ
    public void connected(BluetoothSocket socket) { // 블루투스 연결에 성공시 호출
        mConnectedTask = new ConnectedTask(socket);
        mConnectedTask.execute(); //데이터 수신 등의 일을 담당하는 ConnectedTask 실행
        new Thread(new Runnable() { //Navigation 액티비티에서 받아오는 GPS값을 라즈베리에 송신하는 역할의 스레드
            String msg = "msg";
            @Override
            public void run() {
                while (true) {
                    if (!lonlat.equals(msg)) {
                        try {
                            mConnectedTask.write(lonlat);
                            msg = lonlat;
                            Log.d("msg2 : ", msg);
                            Thread.sleep(4000);
                        } catch (Exception e) {
                        }
                    }
                }
            }
        }).start();
    }

    //연결 가능한 디바이스 목록
    public void showPairedDevicesListDialog() {
        Set<BluetoothDevice> devices = mBluetoothAdapter.getBondedDevices();
        final BluetoothDevice[] pairedDevices = devices.toArray(new BluetoothDevice[0]);

        if (pairedDevices.length == 0) {
            showQuitDialog("No devices have been paired.\n"
                    + "You must pair it with another device.");
            return;
        }

        final String[] items;
        final int[] position = new int[1];
        items = new String[pairedDevices.length];
        for (int i = 0; i < pairedDevices.length; i++) {
            items[i] = pairedDevices[i].getName(); //블루투스 페어링된 디바이스들 가져옴
        }

        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setTitle("Select device");
        builder.setCancelable(false);
        builder.setSingleChoiceItems(items, -1, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                position[0] = which;
                Toast.makeText(getActivity(), items[which], Toast.LENGTH_SHORT).show();
            }
        });
        builder.setPositiveButton("확인", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                if (position[0] == -1) {
                    return;
                }
                Toast.makeText(getActivity(), items[position[0]], Toast.LENGTH_SHORT).show();
                dialog.dismiss();
                ConnectTask task = new ConnectTask(pairedDevices[position[0]]);//선택한 디바이스와 블루투스 연결 시도
                task.execute(); //디바이스와 블루투스 연결을 담당하는 ConnetTask 실행
            }
        });

        builder.setNegativeButton("취소", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int i) { //취소 버튼 클릭시 다이얼로그 cancel
                bluetoothSwitch.setChecked(false);
                dialog.cancel();
            }
        });
        builder.create().show();
    }

    public void showErrorDialog(String message) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setTitle("Quit");
        builder.setCancelable(false);
        builder.setMessage(message);
        builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                if (isConnectionError) {
                    isConnectionError = false;
                    //finish();
                }
            }
        });
        builder.create().show();
    }

    public void showQuitDialog(String message) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setTitle("Quit");
        builder.setCancelable(false);
        builder.setMessage(message);
        builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                //finish();
            }
        });
        builder.create().show();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_BLUETOOTH_ENABLE) {
            if (resultCode == getActivity().RESULT_OK) { //블루투스 활성화에 성공할 시 액티비티가 Result OK 결콰 코드 수신
                //BlueTooth is now Enabled
                showPairedDevicesListDialog(); //페어링된 디바이스들 목록 다이얼로그 보여줌.
            }
            if (resultCode == getActivity().RESULT_CANCELED) { //오류나 기타 이유로 실패시 받는 코드
                showQuitDialog("You need to enable bluetooth");
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        if (mConnectedTask != null) {
            mConnectedTask.cancel(true);
        }
        getActivity().setTitle("Able");
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        BusProviderSetting.getInstance().unregister(this);
    }

    @Subscribe
    public void getPost(String msg) {
        if(msg.equals("end")) {
            if(healthModel != null) {
                reference.child("HEALTH").child(uid).child(date).setValue(healthModel);
                totalHealthRenew();
            }
        } else {
            Log.d("otto_lonlat_set : ", "" + msg);
            lonlat = msg;
            Log.d("setlonlat : ", lonlat);
        }
    }

    //최근 30회의 운동기록을 합산하여 DB에 저장. 차트에 반영하기 위함
    private void totalHealthRenew() {
        reference.child("HEALTH")
                .child(uid)
                .limitToLast(30)
                .addListenerForSingleValueEvent(new ValueEventListener() {
                    double distance = 0;
                    double kcal = 0;
                    double speed = 0;
                    int count = 0;
                    Iterable<DataSnapshot> dataSnapshotIterable;
                    @Override
                    public void onDataChange(DataSnapshot dataSnapshot) {
                        HealthModel healthModel = new HealthModel();
                        dataSnapshotIterable = dataSnapshot.getChildren();
                        Iterator iterator = dataSnapshotIterable.iterator();
                        while(iterator.hasNext()) {
                            iterator.next();
                            count++;
                        }
                        for(DataSnapshot snapshot : dataSnapshot.getChildren()) {
                            healthModel = snapshot.getValue(HealthModel.class);
                            distance += Double.parseDouble(healthModel.getDistance());
                            kcal += Double.parseDouble(healthModel.getKcal());
                            if(healthModel.getSpeed() != null)
                                speed += Double.parseDouble(healthModel.getSpeed());
                        }
                        distance = roundingRecord(distance);
                        kcal = roundingRecord(kcal);
                        speed = roundingRecord(speed/count);
                        reference.child("TOTALHEALTH").child(uid).child("distance").setValue(distance);
                        reference.child("TOTALHEALTH").child(uid).child("kcal").setValue(kcal);
                        reference.child("TOTALHEALTH").child(uid).child("speed").setValue(speed);
                    }
                    @Override
                    public void onCancelled(DatabaseError databaseError) {
                    }
                });
    }

    private double roundingRecord(double record) {
        return Math.round(record*100)/100.0;
    }

    private class CalckThread extends Thread { //킬로리 계산 스레드
        Double weight;
        Double speed;

        public CalckThread(Double weight, Double speed) { //칼로리 계산에 필요한 체중, 속도 받아옴
            this.weight = weight;
            this.speed = speed;
        }

        public void run() {
            try {
                Double call = (weight + bykg) * 0.001 * MET * speed; //소모 칼로리 계산
                cal2 = String.format("%.2f", call); //소수점의 포맷
                healthModel.setKcal(cal2);//health model에 추가
                Log.d(TAG, "Thread - " + Double.toString(weight));
                Log.d(TAG, "cal - " + Double.parseDouble(cal2));
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }

    private class ConnectTask extends AsyncTask<Void, Void, Boolean> { //라즈베리와 블루투스 연결 담당

        private BluetoothSocket mBluetoothSocket = null; // 다른 기기로부터 통신을 대기하는 블루투스 소켓
        private BluetoothDevice mBluetoothDevice;

        ConnectTask(BluetoothDevice bluetoothDevice) {
            mBluetoothDevice = bluetoothDevice;
            mConnectedDeviceName = bluetoothDevice.getName(); //연결하는 블루투스 디바이스의 이름

            //SPP
            UUID uuid = UUID.fromString("00001101-0000-1000-8000-00805f9b34fb"); //uuid는 연결하는 디바이스와 같아야 함.
            //통신 방식에 따라 다름

            try {
                mBluetoothSocket = mBluetoothDevice.createRfcommSocketToServiceRecord(uuid);//RFcomm 방식의 통신
                Log.d(TAG, "create socket for " + mConnectedDeviceName);

            } catch (IOException e) {
                Log.e(TAG, "socket create failed " + e.getMessage());
            }
        }

        @Override
        protected Boolean doInBackground(Void... params) {
            // Always cancel discovery because it will slow down a connection
            mBluetoothAdapter.cancelDiscovery();
            // Make a connection to the BluetoothSocket
            try {
                // This is a blocking call and will only return on a
                // successful connection or an exception
                mBluetoothSocket.connect(); //연결
            } catch (IOException e) {
                // Close the socket
                try {
                    mBluetoothSocket.close();
                } catch (IOException e2) {
                    Log.e(TAG, "unable to close() " +
                            " socket during connection failure", e2);
                }
                return false;
            }
            return true;
        }

        @Override
        protected void onPostExecute(Boolean isSucess) {
            if (isSucess) {//연결 성공시
                connected(mBluetoothSocket); //소켓을 가지고 connectedTask 실행
            } else {
                isConnectionError = true;
                Log.d(TAG, "Unable to connect device");
                showErrorDialog("Unable to connect device");
            }
        }
    }

    private class ConnectedTask extends AsyncTask<Void, String, Boolean> { //데이터 송/수신 담당

        private InputStream mInputStream = null; //라즈베리에서 데이터를 받아오기 위한 인풋스트림
        private OutputStream mOutputStream = null; //라즈베리에 GPS값을 주기 위한 아웃풋스트림
        private BluetoothSocket mBluetoothSocket = null;

        ConnectedTask(BluetoothSocket socket) {
            mBluetoothSocket = socket; //연결된 소켓 저장
            try {
                mInputStream = mBluetoothSocket.getInputStream();
                mOutputStream = mBluetoothSocket.getOutputStream();
            } catch (IOException e) {
                Log.e(TAG, "socket not created", e);
            }

            Log.d(TAG, "connected to " + mConnectedDeviceName); //연결된 디바이스 로그 출력
            //mConnectionStatus.setText( "connected to "+mConnectedDeviceName);
        }
        @Override

        protected Boolean doInBackground(Void... params) {
            byte[] readBuffer = new byte[1024];
            int readBufferPosition = 0;

            while (true) {
                if (isCancelled()) return false;
                try {
                    int bytesAvailable = mInputStream.available(); //수신 데이터 확인
                    if (bytesAvailable > 0) { // 읽어올게 있다면
                        byte[] packetBytes = new byte[bytesAvailable];
                        mInputStream.read(packetBytes); //읽음
                        for (int i = 0; i < bytesAvailable; i++) {
                            byte b = packetBytes[i];
                            if (b == '\n') { //끝자리가 개행 문자일시
                                byte[] encodedBytes = new byte[readBufferPosition];

                                System.arraycopy(readBuffer, 0, encodedBytes, 0,
                                        encodedBytes.length); //readBuffer에서 encodedBytes로 복사
                                String recvMessage = new String(encodedBytes, "UTF-8"); //recvMessage 변수에 UTF-8 방식으로 인코딩

                                readBufferPosition = 0;

                                Log.d(TAG, "recv message: " + recvMessage);
                                String[] array = recvMessage.split(",");
                                speed = Double.parseDouble(array[0]);
                                healthModel.setSpeed(Double.toString(speed));
                                distance = Double.parseDouble(array[1]);
                                healthModel.setDistance(Double.toString(distance));
                                calcul = new CalckThread(weight, speed);//체중,속도값을 가지고 칼로리 계산 스레드
                                calcul.start();//실행
                            } else {
                                readBuffer[readBufferPosition++] = b;
                            }
                        }
                    }
                } catch (IOException e) {
                    Log.e(TAG, "disconnected", e);
                    return false;
                }
            }
        }

        @Override
        protected void onPostExecute(Boolean isSucess) {
            super.onPostExecute(isSucess); //연결 끊겼을 시
            if (!isSucess) {
                closeSocket();
                Log.d(TAG, "Device connection was lost");
                isConnectionError = true;
                showErrorDialog("Device connection was lost");
            }
        }

        @Override
        protected void onCancelled(Boolean aBoolean) {
            super.onCancelled(aBoolean);
            closeSocket();
        }

        void closeSocket() {
            try {
                mBluetoothSocket.close();
                Log.d(TAG, "close socket()");
            } catch (IOException e2) {
                Log.e(TAG, "unable to close() " +
                        " socket during connection failure", e2);
            }
        }
        void write(String msg) { //라즈베리에 송신하는 write
            msg += "\n";
            Log.d("write msg : ", msg);
            try {
                mOutputStream.write(msg.getBytes()); //문자열을 바이트코드로 인코딩 후 송신(손실을 방지하기 위해서?)
                mOutputStream.flush(); //초기화
            } catch (IOException e) {
                Log.e(TAG, "Exception during send", e);
            }
            //mInputEditText.setText(" ");
        }
    }
}