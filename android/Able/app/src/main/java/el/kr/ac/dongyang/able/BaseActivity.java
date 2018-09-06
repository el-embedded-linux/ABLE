package el.kr.ac.dongyang.able;

import android.support.v7.app.AppCompatActivity;
import android.widget.Toast;

public class BaseActivity extends AppCompatActivity {

    public void toastText(String text){
        Toast.makeText(this, text, Toast.LENGTH_SHORT).show();
    }
}
