package el.kr.ac.dongyang.able.setting;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.AlertDialog;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.Switch;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import com.squareup.otto.Subscribe;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Set;
import java.util.Timer;
import java.util.UUID;

import el.kr.ac.dongyang.able.BusProvider;
import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.health.FragmentHealthcare;
import el.kr.ac.dongyang.able.model.HealthModel;
import el.kr.ac.dongyang.able.model.UserModel;

import static java.lang.System.exit;

/**
 * Created by impro on 2018-05-08.
 */

public class FragmentSetting extends Fragment{

    DatabaseReference mDatabase = FirebaseDatabase.getInstance().getReference();

    FragmentHealthcare f = new FragmentHealthcare();
    Double speed;
    Switch btsw;
    Double weight;
    FirebaseUser user;
    String uid;
    UserModel userModel;
    HealthModel healthModel;
    CalckThread calcul;
    String cal2;
    Double bykg = 10.0;
    int minute = 60;
    Double MET = 3.3;
    String date;
    int day, month, year;

    Calendar cal;

    private ArrayList<String> navigeo = new ArrayList<String>();
    public String lonlat = "msg";

    public FragmentSetting() {
    }

    Timer t = new Timer(true);

    Button infoModify;
    FragmentTransaction ft;
    String fragmentTag;

    //블루투스
    private final int REQUEST_BLUETOOTH_ENABLE = 100;
    ConnectedTask mConnectedTask = null;
    static BluetoothAdapter mBluetoothAdapter;
    private String mConnectedDeviceName = null;
    private ArrayAdapter<String> mConversationArrayAdapter;
    static boolean isConnectionError = false;
    private static final String TAG = "BluetoothClient";


    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        BusProvider.getInstance().register(this);
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_setting,container,false);
        getActivity().setTitle("Setting");

        user = FirebaseAuth.getInstance().getCurrentUser();
        if(user != null){
            uid = user.getUid();
        }

        userModel = new UserModel();
        healthModel = new HealthModel();
        cal = Calendar.getInstance();

        day = cal.get(Calendar.DAY_OF_MONTH);
        month = cal.get(Calendar.MONTH) + 1;
        year = cal.get(Calendar.YEAR);
        if(month>=10 && day<10){
            date = year + "-" + month + "-0" + day;
        }else if(month<10 && day>=10){
            date = year + "-0" + month + "-" + day;
        } else if (month>=10 && day>=10) {
            date = year + "-" + month + "-" + day;
        }else{
            date = year + "-0" + month + "-0" + day;
        }
        btsw = (Switch) view.findViewById(R.id.bluetooth_switch);
        btsw.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
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


        infoModify = (Button)view.findViewById(R.id.info_modify);
        infoModify.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Fragment fragment = new FragmentInformation();
                fragmentTag = fragment.getClass().getSimpleName();  //FragmentLogin
                Log.i("fagmentTag", fragmentTag);
                getActivity().getSupportFragmentManager().popBackStack(fragmentTag, FragmentManager.POP_BACK_STACK_INCLUSIVE);
                ft=getActivity().getSupportFragmentManager().beginTransaction();
                ft.replace(R.id.main_layout, fragment);
                ft.addToBackStack(fragmentTag);
                ft.commit();
            }
        });

        /*mConversationArrayAdapter = new ArrayAdapter<>( getActivity(), android.R.layout.simple_list_item_1 );
        //mMessageListview.setAdapter(mConversationArrayAdapter);*/

        return view;
    }
    public void setSpeed(Double speed){
        this.speed = speed;
    }

    @Override
    public void onStart() {
        super.onStart();

        mDatabase.child("USER").child(uid).addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                userModel = dataSnapshot.getValue(UserModel.class);
                if(userModel.weight != null) {
                    weight = Double.parseDouble(userModel.weight);
                    Log.d(TAG, "start - " + Double.toString(weight));
                }
            }
            @Override
            public void onCancelled(DatabaseError databaseError) {
            }
        });
    }

    class CalckThread extends Thread {
        Double weight;
        Double speed;
        public CalckThread(Double weight, Double speed) {
            this.weight = weight;
            this.speed = speed;
        }

        public void run() {
            try {
                Double call = (weight + bykg)*0.001*MET*speed;
                cal2 = String.format("%.2f", call);
                healthModel.kcal = cal2;
                mDatabase.child("HEALTH").child(uid).child(date).child("kcal").setValue(healthModel.kcal);
                Log.d(TAG, "Thread - " + Double.toString(weight));
                Log.d(TAG, "cal - " + Double.parseDouble(cal2));
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }

    public class SendTh extends Thread {
        String msg = "msg";
        /*        String msg;
        public SendTh(String msg){
            this.msg = msg;
        }*/
        public void run(){
            while (true) {
                if(!lonlat.equals(msg)) {
//                    try {
                        mConnectedTask.write(lonlat);
                        msg = lonlat;
                        Log.d("msg2 : ", msg);
                       /* Thread.sleep(4000);
                    } catch (InterruptedException e) {
                    }*/
                }
            }
        }
    }

    private class ConnectTask extends AsyncTask<Void, Void, Boolean> {

        private BluetoothSocket mBluetoothSocket = null;
        private BluetoothDevice mBluetoothDevice = null;

        ConnectTask(BluetoothDevice bluetoothDevice) {
            mBluetoothDevice = bluetoothDevice;
            mConnectedDeviceName = bluetoothDevice.getName();

            //SPP
            UUID uuid = UUID.fromString("00001101-0000-1000-8000-00805f9b34fb");

            try {
                mBluetoothSocket = mBluetoothDevice.createRfcommSocketToServiceRecord(uuid);
                Log.d( TAG, "create socket for "+mConnectedDeviceName);

            } catch (IOException e) {
                Log.e( TAG, "socket create failed " + e.getMessage());
            }

            //mConnectionStatus.setText("connecting...");
        }
        @Override
        protected Boolean doInBackground(Void... params) {
            // Always cancel discovery because it will slow down a connection
            mBluetoothAdapter.cancelDiscovery();
            // Make a connection to the BluetoothSocket
            try {
                // This is a blocking call and will only return on a
                // successful connection or an exception
                mBluetoothSocket.connect();
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
            if ( isSucess ) {
                connected(mBluetoothSocket);
            }
            else{
                isConnectionError = true;
                Log.d( TAG,  "Unable to connect device");
                showErrorDialog("Unable to connect device");
            }
        }
    }

    //여기서 데이터 보낼수 있음.. 하ㅠ
    public void connected( BluetoothSocket socket ) {
        mConnectedTask = new ConnectedTask(socket);
        mConnectedTask.execute();
        SendTh sendTh = new SendTh();
        sendTh.start();

    }

    private class ConnectedTask extends AsyncTask<Void, String, Boolean> {

        private InputStream mInputStream = null;
        private OutputStream mOutputStream = null;
        private BluetoothSocket mBluetoothSocket = null;
        ConnectedTask(BluetoothSocket socket){
            mBluetoothSocket = socket;
            try {
                mInputStream = mBluetoothSocket.getInputStream();
                mOutputStream = mBluetoothSocket.getOutputStream();
            } catch (IOException e) {
                Log.e(TAG, "socket not created", e );
            }

            Log.d( TAG, "connected to "+mConnectedDeviceName);
            //mConnectionStatus.setText( "connected to "+mConnectedDeviceName);
        }


        @Override
        protected Boolean doInBackground(Void... params) {
            byte [] readBuffer = new byte[1024];
            int readBufferPosition = 0;

            while (true) {
                if ( isCancelled() ) return false;
                try {
                    int bytesAvailable = mInputStream.available();
                    if(bytesAvailable > 0) {
                        byte[] packetBytes = new byte[bytesAvailable];
                        mInputStream.read(packetBytes);
                        for(int i=0;i<bytesAvailable;i++) {
                            byte b = packetBytes[i];
                            if(b == '\n') {
                                byte[] encodedBytes = new byte[readBufferPosition];

                                System.arraycopy(readBuffer, 0, encodedBytes, 0,
                                        encodedBytes.length);
                                String recvMessage = new String(encodedBytes, "UTF-8");

                                readBufferPosition = 0;

                                Log.d(TAG, "recv message: " + recvMessage);
                                for (int j = 0; j < recvMessage.length(); j++) {
                                    speed = Double.parseDouble(recvMessage);
                                    healthModel.speed = Double.toString(speed);
                                }
                                mDatabase.child("HEALTH").child(uid).child(date).child("speed").setValue(healthModel.speed);
                                calcul = new CalckThread(weight,speed);
                                calcul.start();
                            }
                            else {
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
        protected void onProgressUpdate(String... recvMessage) {
            mConversationArrayAdapter.insert(mConnectedDeviceName + ": " + recvMessage[0], 0);
        }

        @Override
        protected void onPostExecute(Boolean isSucess) {
            super.onPostExecute(isSucess);

            if ( !isSucess ) {
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

        void closeSocket(){
            try {
                mBluetoothSocket.close();
                Log.d(TAG, "close socket()");
            } catch (IOException e2) {
                Log.e(TAG, "unable to close() " +
                        " socket during connection failure", e2);
            }
        }

        void write(String msg){
            msg += "\n";
            //Log.d("msg : ", msg);
            try {
                mOutputStream.write(msg.getBytes());
                mOutputStream.flush();
            } catch (IOException e) {
                Log.e(TAG, "Exception during send", e );
            }
            //mInputEditText.setText(" ");
        }
    }

    public void showPairedDevicesListDialog() {
        Set<BluetoothDevice> devices = mBluetoothAdapter.getBondedDevices();
        final BluetoothDevice[] pairedDevices = devices.toArray(new BluetoothDevice[0]);

        if ( pairedDevices.length == 0 ){
            showQuitDialog( "No devices have been paired.\n"
                    +"You must pair it with another device.");
            return;
        }

        String[] items;
        items = new String[pairedDevices.length];
        for (int i=0;i<pairedDevices.length;i++) {
            items[i] = pairedDevices[i].getName();
        }

        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setTitle("Select device");
        builder.setCancelable(false);
        builder.setItems(items, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                ConnectTask task = new ConnectTask(pairedDevices[which]);
                task.execute();
            }
        });
        builder.create().show();
    }

    public void showErrorDialog(String message) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setTitle("Quit");
        builder.setCancelable(false);
        builder.setMessage(message);
        builder.setPositiveButton("OK",  new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                if ( isConnectionError  ) {
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
        builder.setPositiveButton("OK",  new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                //finish();
            }
        });
        builder.create().show();
    }

    void sendMessage(String msg){
        Log.d(TAG, "send : " + msg);
        if ( mConnectedTask != null ) {
            mConnectedTask.write(msg);
            Log.d(TAG, "send message: " + msg);
            mConversationArrayAdapter.insert("Me:  " + msg, 0);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if(requestCode == REQUEST_BLUETOOTH_ENABLE){
            if (resultCode == getActivity().RESULT_OK){
                //BlueTooth is now Enabled
                showPairedDevicesListDialog();
            }
            if(resultCode == getActivity().RESULT_CANCELED){
                showQuitDialog( "You need to enable bluetooth");
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        if ( mConnectedTask != null ) {
            mConnectedTask.cancel(true);
        }
        getActivity().setTitle("Able");
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        BusProvider.getInstance().unregister(this);
    }

    @Subscribe
    public void getPost(String msg){
        Log.d("otto_lonlat_set : ", ""+msg);
        lonlat = msg;
        Log.d("setlonlat : ", lonlat);
    }
}