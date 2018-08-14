package el.kr.ac.dongyang.able.model;

import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Created by impro on 2018-05-08.
 */

public class UserModel {
    public String email;
    public String password;
    public String userName;
    public String profileImageUrl;
    public String uid;
    public String pushToken;
    public String comment;
    public String address;
    public String height;
    public String weight;
    public String goal;

    public String getUserName() {
        return userName;
    }

    public String getHeight() {
        return height;
    }

    public void setHeight(String height) {
        this.height = height;
    }
}
