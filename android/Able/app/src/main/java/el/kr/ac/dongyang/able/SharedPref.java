package el.kr.ac.dongyang.able;

import android.content.Context;
import android.content.SharedPreferences;

public class SharedPref {

    //필드
    private static SharedPref instance = null;
    private static SharedPreferences pref;

    //생성자
    private SharedPref(Context context) {
        pref = context.getSharedPreferences("todoPref", Context.MODE_PRIVATE);
    }

    public static SharedPref getInstance(Context context) {
        if(instance == null) {
            instance = new SharedPref(context);
        }
        return instance;
    }

    public void setData(String key, String value){
        //저장
        pref.edit().putString(key, value).apply();
    }

    public String getData(String key){
        return pref.getString(key, null);
    }

    public void removeData(String key){
        pref.edit().remove(key).apply();
    }

}
