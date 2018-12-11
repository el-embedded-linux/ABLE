package com.example.dudgn.androidchart;

import android.graphics.Color;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;

import com.github.mikephil.charting.charts.LineChart;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.components.YAxis;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.LineData;
import com.github.mikephil.charting.data.LineDataSet;

import java.util.ArrayList;
import java.util.List;

public class LineChartActivity extends AppCompatActivity {
    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.linechartactivity);

        LineChart chart = findViewById(R.id.chart);

        XAxis xAxis = chart.getXAxis();
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setTextSize(10f);
        xAxis.setGranularity(1f); // interval 1
        xAxis.setTextColor(Color.RED);
        xAxis.setDrawAxisLine(true);
        xAxis.setDrawGridLines(false);

        // data has AxisDependency.LEFT
        YAxis left = chart.getAxisLeft();
        left.setDrawAxisLine(true); // no axis line
        left.setDrawGridLines(false); // no grid lines
        left.setDrawZeroLine(true); // draw a zero line
        chart.getAxisRight().setEnabled(false); // no right axis
        left.setTextSize(12f); // set the text size
        left.setAxisMinimum(0f); // start at zero
        left.setAxisMaximum(10f); // the axis maximum is 100
        left.setTextColor(Color.BLUE);
        left.setGranularity(1f); // interval 1
        left.setLabelCount(6, true); // force 6 labels

        Values values = new Values(3,4);
        Values values1 = new Values(5,2);
        Values values2 = new Values(7,1);

        Values[] dataObjects = {values, values1, values2};

        List<Entry> entries = new ArrayList<Entry>();

        for (Values data : dataObjects) {
            // turn your data into Entry objects
            entries.add(new Entry(data.getValueX(), data.getValueY()));
        }

        LineDataSet dataSet = new LineDataSet(entries, "Label"); // add entries to dataset
        dataSet.setColor(R.color.colorAccent);
        dataSet.setValueTextColor(R.color.colorPrimary); // styling, ...

        LineData lineData = new LineData(dataSet);
        chart.setData(lineData);
        chart.invalidate(); // refresh
    }
}
