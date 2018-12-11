package com.example.dudgn.androidchart;

import com.github.mikephil.charting.components.AxisBase;
import com.github.mikephil.charting.formatter.IAxisValueFormatter;

import java.text.DecimalFormat;

public class MyAxisValueFormatter implements IAxisValueFormatter{

    private DecimalFormat mFormat;

    public MyAxisValueFormatter(){
        mFormat = new DecimalFormat("###.#");
    }
    @Override
    public String getFormattedValue(float value, AxisBase axis) {
        return mFormat.format(value) + "km";
    }
}
