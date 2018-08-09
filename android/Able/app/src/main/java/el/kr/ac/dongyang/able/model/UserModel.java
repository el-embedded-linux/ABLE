package el.kr.ac.dongyang.able.model;

import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Created by impro on 2018-05-08.
 */

public class UserModel {
    public String userName;
    public String address;
    public String height;
    public String weight;

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getHeight() {
        return height;
    }

    public void setHeight(String height) {
        this.height = height;
    }

    public String getWeight() {
        return weight;
    }

    public void setWeight(String weight) {
        this.weight = weight;
    }
}
