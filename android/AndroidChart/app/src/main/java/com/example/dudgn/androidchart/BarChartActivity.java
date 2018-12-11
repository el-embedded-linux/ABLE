package com.example.dudgn.androidchart;

import android.graphics.Color;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;

import com.github.mikephil.charting.animation.Easing;
import com.github.mikephil.charting.charts.BarChart;
import com.github.mikephil.charting.charts.HorizontalBarChart;
import com.github.mikephil.charting.components.AxisBase;
import com.github.mikephil.charting.components.IMarker;
import com.github.mikephil.charting.components.Legend;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.components.YAxis;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.BarDataSet;
import com.github.mikephil.charting.data.BarEntry;
import com.github.mikephil.charting.formatter.IAxisValueFormatter;
import com.github.mikephil.charting.utils.ColorTemplate;

import java.util.ArrayList;
import java.util.List;

public class BarChartActivity extends AppCompatActivity{

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.bar_chart_activity);

        final String[] mMonths = new String[] {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"
        };

        //차트 맵핑, 속성
        BarChart chart = findViewById(R.id.barChart);
        chart.setBackgroundColor(Color.TRANSPARENT);
        chart.setDescription(null);
        chart.setNoDataText("데이터가 없습니다");
        chart.setDrawGridBackground(false);
        chart.setGridBackgroundColor(Color.BLUE);
        chart.setDrawBorders(true);
        chart.setBorderColor(Color.BLACK);
        chart.setDragEnabled(false);            //드래그 비활성화
        chart.setScaleEnabled(false);           //차트 배율 비활성화
        chart.setPinchZoom(false);              //줌 비활성화
        chart.setDoubleTapToZoomEnabled(false); //두번 탭 비활성화

        //범례
        Legend legend = chart.getLegend();
        legend.setEnabled(false);

        //마커뷰
        IMarker marker = new YourMarkerView(this, R.layout.item_markerview);
        chart.setMarker(marker);

        //X축
        XAxis xAxis = chart.getXAxis();
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setTextSize(10f);
        //xAxis.setGranularity(1f); // interval 1
        xAxis.setTextColor(Color.RED);
        xAxis.setDrawAxisLine(true);
        xAxis.setDrawGridLines(false);
        xAxis.setAxisMaximum(4f); // the axis maximum is 100
        xAxis.setValueFormatter(new IAxisValueFormatter() {
            @Override
            public String getFormattedValue(float value, AxisBase axis) {
                return mMonths[(int) value];
            }
        });

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

        //엔트리
        List<BarEntry> entries = new ArrayList<>();
        entries.add(new BarEntry(0f, 3000f));
        entries.add(new BarEntry(1f, 8000f));
        entries.add(new BarEntry(2f, 6000f));
        entries.add(new BarEntry(3f, 5000f));

        //데이터셋
        BarDataSet set = new BarDataSet(entries, "BarDataSet");
        //set.setColors(new int[] {Color.RED, Color.GRAY, Color.GREEN, Color.BLACK, Color.RED, Color.GRAY});
        set.setColors(ColorTemplate.VORDIPLOM_COLORS);

        //데이터 저장
        BarData data = new BarData(set);
        data.setValueFormatter(new MyValueFormatter());
        data.setValueTextColor(Color.GREEN);
        data.setValueTextSize(14f);
        data.setBarWidth(0.9f); // set custom bar width
        chart.setData(data);
        chart.setFitBars(true); // make the x-axis fit exactly all bars
        chart.animateY(5000, Easing.EasingOption.EaseInOutBounce);
        chart.invalidate(); // refresh


    }
}
