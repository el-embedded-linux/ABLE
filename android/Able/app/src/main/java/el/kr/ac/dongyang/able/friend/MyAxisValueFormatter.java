package el.kr.ac.dongyang.able.friend;

import com.github.mikephil.charting.components.AxisBase;
import com.github.mikephil.charting.formatter.IAxisValueFormatter;

import java.text.DecimalFormat;

//차트 생성시 좌측의 거리(km) 인덱스 표시
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
