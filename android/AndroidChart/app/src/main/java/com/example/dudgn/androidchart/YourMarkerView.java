package com.example.dudgn.androidchart;

import android.content.Context;
import android.widget.TextView;

import com.github.mikephil.charting.components.IMarker;
import com.github.mikephil.charting.components.MarkerView;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.highlight.Highlight;
import com.github.mikephil.charting.utils.MPPointF;

public class YourMarkerView extends MarkerView {

    private TextView tvContent;

    public YourMarkerView(Context context, int layoutResource) {
        super(context, layoutResource);
        tvContent = findViewById(R.id.tvContent);
    }

    private MPPointF mOffset;

    @Override
    public MPPointF getOffset() {

        if(mOffset == null){
            mOffset = new MPPointF(-(getWidth()/2), -getHeight());
        }
        return mOffset;
    }

    @Override
    public void refreshContent(Entry e, Highlight highlight) {

        tvContent.setText(""+e.getY());

        super.refreshContent(e, highlight);
    }
}
