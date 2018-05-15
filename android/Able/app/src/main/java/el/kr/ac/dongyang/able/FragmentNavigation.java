package el.kr.ac.dongyang.able;

import android.graphics.Color;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.RelativeLayout;

import com.skt.Tmap.TMapData;
import com.skt.Tmap.TMapPoint;
import com.skt.Tmap.TMapPolyLine;
import com.skt.Tmap.TMapView;

/**
 * Created by impro on 2018-03-30.
 */
public class FragmentNavigation extends android.support.v4.app.Fragment {

    private static final String LOG_TAG = "FragmentNavigation";
    private TMapView tMapView;

    public FragmentNavigation() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_navigation,container,false);
        getActivity().setTitle("Navigation");

        RelativeLayout tmap = (RelativeLayout) view.findViewById(R.id.map_view);
        tMapView = new TMapView(getActivity());

        tMapView.setSKTMapApiKey( "2bcf226b-36b6-49da-82cc-5f00acee90a2" );
        tMapView.setCompassMode(true);
        tMapView.setZoomLevel(16);
        tMapView.setIconVisibility(true);
        tMapView.setLanguage(TMapView.LANGUAGE_KOREAN);
        tMapView.setTrackingMode(true);
        tMapView.setSightVisible(true);
        tmap.addView(tMapView);

        Carline carline = new Carline();
        carline.start();

        return view;
    }

    public class Carline extends Thread {
        public void run() {
            TMapPoint tMapPointStart = new TMapPoint(37.570841, 126.985302); // SKT타워(출발지)
            TMapPoint tMapPointEnd = new TMapPoint(37.551135, 126.988205); // N서울타워(목적지)

            try {
                TMapPolyLine tMapPolyLine = new TMapData().findPathData(tMapPointStart, tMapPointEnd);
                tMapPolyLine.setLineColor(Color.BLUE);
                tMapPolyLine.setLineWidth(2);
                tMapView.addTMapPolyLine("Line1", tMapPolyLine);

            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}