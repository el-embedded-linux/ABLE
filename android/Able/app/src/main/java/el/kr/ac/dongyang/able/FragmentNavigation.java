package el.kr.ac.dongyang.able;

import android.os.Bundle;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

/**
 * Created by impro on 2018-03-30.
 */
public class FragmentNavigation extends android.support.v4.app.Fragment {

    private static final String LOG_TAG = "FragmentNavigation";

    public FragmentNavigation() {
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_navigation,container,false);
        getActivity().setTitle("Navigation");



        return view;
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        getActivity().setTitle("Able");
    }
}