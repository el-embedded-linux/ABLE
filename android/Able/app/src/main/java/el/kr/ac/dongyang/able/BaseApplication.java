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

        if(progressDialog != null && progressDialog.isShowing()) {
            progressSet(message);
        } else {
            progressDialog = new AppCompatDialog(activity);
            progressDialog.setCancelable(false);
            progressDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
            progressDialog.setContentView(R.layout.progress_loading);
            progressDialog.show();
        }

        ImageView imgLoadingFrame = progressDialog.findViewById(R.id.ivFrameLoading);
        Glide.with(this)
                .load(R.raw.animate_day_night_color)
                .into(imgLoadingFrame);
        TextView tvProgressMessage = progressDialog.findViewById(R.id.tvProgressMessage);
        if(!TextUtils.isEmpty(message)) {
            tvProgressMessage.setText(message);
        }
    }

    public void progressSet(String message) {
        if(progressDialog == null || !progressDialog.isShowing()) {
            return;
        }
        TextView tvProgressMessage = progressDialog.findViewById(R.id.tvProgressMessage);
        if(!TextUtils.isEmpty(message)) {
            tvProgressMessage.setText(message);
        }
    }

    public void progressOff() {
        if(progressDialog != null && progressDialog.isShowing()) {
            progressDialog.dismiss();
        }
    }
}


