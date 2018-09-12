package el.kr.ac.dongyang.able;

import android.support.v4.app.Fragment;

import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;

public class BaseFragment extends Fragment {

    public DatabaseReference reference = FirebaseDatabase.getInstance().getReference();
}
