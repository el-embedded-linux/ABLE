package el.kr.ac.dongyang.able;

import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;

public class BaseFragment extends Fragment {

    public DatabaseReference reference = FirebaseDatabase.getInstance().getReference();

    public void replaceFragment(Fragment fragment) {
        String fragmentTag;
        FragmentManager manager = getActivity().getSupportFragmentManager();
        FragmentTransaction ft;

        if (!fragment.isVisible()) {
            fragmentTag = fragment.getClass().getSimpleName();
            manager.popBackStack(null, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            ft = manager.beginTransaction()
                    .replace(R.id.main_layout, fragment, fragmentTag)
                    .setTransition(FragmentTransaction.TRANSIT_FRAGMENT_OPEN)
                    .addToBackStack(null);
            ft.commit();
        }
    }
}
