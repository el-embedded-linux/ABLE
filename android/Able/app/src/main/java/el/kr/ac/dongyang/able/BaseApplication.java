package el.kr.ac.dongyang.able;

import android.app.Activity;
import android.app.Application;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.support.v7.app.AppCompatDialog;
import android.text.TextUtils;
import android.widget.ImageView;
import android.widget.TextView;

import com.bumptech.glide.Glide;

//로딩화면을 구현하는 함수 클래스
public class BaseApplication extends Application {
    private static BaseApplication baseApplication;
    AppCompatDialog progressDialog;

    public static BaseApplication getInstance() {
        return baseApplication;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        baseApplication = this;
    }

    public void progressOn(Activity activity, String message) {
        if(activity == null || activity.isFinishing()) {
            return;
        }

        //다이얼로스 설정
        if(progressDialog != null && progressDialog.isShowing()) {
            progressSet(message);
        } else {
            progressDialog = new AppCompatDialog(activity);
            progressDialog.setCancelable(false);
            progressDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
            progressDialog.setContentView(R.layout.progress_loading);
            progressDialog.show();
        }

        //이미지뷰, 텍스트뷰 설정
        ImageView imgLoadingFrame = progressDialog.findViewById(R.id.ivFrameLoading);
        Glide.with(this)
                .load(R.raw.animate_day_night_color)
                .into(imgLoadingFrame);
        TextView tvProgressMessage = progressDialog.findViewById(R.id.tvProgressMessage);
        if(!TextUtils.isEmpty(message)) {
            tvProgressMessage.setText(message);
        }
    }

    //로딩화면 설정
    public void progressSet(String message) {
        if(progressDialog == null || !progressDialog.isShowing()) {
            return;
        }
        TextView tvProgressMessage = progressDialog.findViewById(R.id.tvProgressMessage);
        if(!TextUtils.isEmpty(message)) {
            tvProgressMessage.setText(message);
        }
    }

    //로딩화면 종료
    public void progressOff() {
        if(progressDialog != null && progressDialog.isShowing()) {
            progressDialog.dismiss();
        }
    }
}


