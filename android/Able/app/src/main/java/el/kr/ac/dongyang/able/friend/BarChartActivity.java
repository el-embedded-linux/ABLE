package el.kr.ac.dongyang.able.friend;

import android.annotation.SuppressLint;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;

import com.github.mikephil.charting.animation.Easing;
import com.github.mikephil.charting.charts.BarChart;
import com.github.mikephil.charting.components.AxisBase;
import com.github.mikephil.charting.components.Legend;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.components.YAxis;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.formatter.IAxisValueFormatter;
import com.github.mikephil.charting.utils.ColorTemplate;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

import el.kr.ac.dongyang.able.R;
import el.kr.ac.dongyang.able.model.HealthModel;
import el.kr.ac.dongyang.able.model.UserModel;

public class BarChartActivity extends AppCompatActivity {

    private List<String> userNames = new ArrayList<>();
    private List<BarEntry> entries = new ArrayList<>();
    private List<String> keys = new ArrayList<>();
    BarDataSet set;
    BarChart chart;
    XAxis xAxis;
    private Handler handler;

    @SuppressLint("HandlerLeak")
    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_barchart);

        initBarChart();

        handler = new Handler() {
            public void handleMessage(Message msg) {
                //chart.setVisibleXRangeMaximum((float)userNames.size());
                xAxis.setAxisMaximum((float)userNames.size()-1); // the axis maximum is 100
                //xAxis.setAxisMaximum(6f); // the axis maximum is 100
                Log.d("username","is" + userNames.size());
                xAxis.setValueFormatter(new IAxisValueFormatter() {
                    @Override
                    public String getFormattedValue(float value, AxisBase axis) {
                        return userNames.get((int)value);
                    }
                });

                //데이터셋
                set = new BarDataSet(entries, "BarDataSet");
                //set.setColors(new int[] {Color.RED, Color.GRAY, Color.GREEN, Color.BLACK, Color.RED, Color.GRAY});
                set.setColors(ColorTemplate.VORDIPLOM_COLORS);

                //데이터 저장
                BarData data = new BarData(set);
                data.setValueFormatter(new MyValueFormatter());
                data.setValueTextColor(Color.GREEN);
                data.setValueTextSize(14f);
                //data.setBarWidth(0.5f); // set custom bar width
                chart.setData(data);
                chart.setFitBars(true); // make the x-axis fit exactly all bars
                chart.animateY(3000, Easing.EasingOption.EaseInOutBounce);
                chart.invalidate(); // refresh
            }
        };

        String uid = FirebaseAuth.getInstance().getCurrentUser().getUid();

        FirebaseDatabase.getInstance().getReference().child("FRIEND").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                keys.clear();
                UserModel userModel = new UserModel();
                for (DataSnapshot item : dataSnapshot.getChildren()) {
                    userModel = item.getValue(UserModel.class);
                    keys.add(item.getKey());
                    userNames.add(userModel.getUserName());
                }
                userNames.add("");

                FirebaseDatabase.getInstance().getReference().child("TOTALHEALTH").addListenerForSingleValueEvent(new ValueEventListener() {
                    @Override
                    public void onDataChange(DataSnapshot dataSnapshot) {
                        HealthModel healthModel = new HealthModel();
                        for (int i = 0; i < keys.size(); i++) {
                            for (DataSnapshot item : dataSnapshot.getChildren()) {
                                if (keys.get(i).equals(item.getKey())) {
                                    healthModel = item.getValue(HealthModel.class);
                                    entries.add(new BarEntry((float) i, Float.parseFloat(healthModel.getDistance())));
                                    continue;
                                }
                            }
                        }
                        Log.d("userNames", ""+ userNames.size());
                        Log.d("endtries", ""+ entries.size());
                        Message msg = handler.obtainMessage();
                        handler.sendMessage(msg);
                    }

                    @Override
                    public void onCancelled(DatabaseError databaseError) {
                    }
                });
            }

            @Override
            public void onCancelled(DatabaseError databaseError) {

            }
        });
    }

    private void initBarChart() {
        chart = findViewById(R.id.chart1);
        chart.setBackgroundColor(Color.TRANSPARENT);
        chart.setDescription(null);
        chart.setNoDataText("데이터가 없습니다");
        chart.setDrawGridBackground(false);
        chart.setGridBackgroundColor(Color.BLUE);
        chart.setDrawBorders(false);
        chart.setBorderColor(Color.BLACK);
        //chart.setDragEnabled(false);            //드래그 비활성화
        chart.setScaleEnabled(false);           //차트 배율 비활성화
        chart.setPinchZoom(false);              //줌 비활성화
        chart.setDoubleTapToZoomEnabled(false); //두번 탭 비활성화

        //범례
        Legend legend = chart.getLegend();
        legend.setEnabled(false);

        xAxis = chart.getXAxis();
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setTextSize(16f);
        xAxis.setTextColor(Color.BLACK);
        xAxis.setDrawAxisLine(true);
        xAxis.setDrawGridLines(false);
        xAxis.setGranularity(1f); // interval 1

        //Y축 왼쪽
        // data has AxisDependency.LEFT
        YAxis left = chart.getAxisLeft();
        left.setValueFormatter(new MyAxisValueFormatter());
        left.setDrawAxisLine(true); // no axis line
        left.setDrawGridLines(false); // no grid lines
        left.setDrawZeroLine(true); // draw a zero line
        chart.getAxisRight().setEnabled(false); // no right axis
        left.setTextSize(12f); // set the text size
        //left.setAxisMinimum(0f); // start at zero
        //left.setAxisMaximum(10f); // the axis maximum is 100
        left.setTextColor(Color.BLUE);
        //left.setGranularity(1f); // interval 1
        //left.setLabelCount(6, true); // force 6 labels*/

        /*//엔트리
        entries.add(new BarEntry(0f, 30f));
        entries.add(new BarEntry(1f, 80f));
        entries.add(new BarEntry(2f, 60f));
        entries.add(new BarEntry(3f, 50f));
        // gap of 2f
        entries.add(new BarEntry(5f, 70f));
        entries.add(new BarEntry(6f, 60f));*/
    }
}
