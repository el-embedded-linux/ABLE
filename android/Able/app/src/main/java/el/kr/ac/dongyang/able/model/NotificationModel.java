package el.kr.ac.dongyang.able.model;

/**
 * Created by myeongsic on 2017. 12. 4..
 */

public class NotificationModel {

    public String to;

    public Notification notification = new Notification();
    public Data data = new Data();

    public static class Notification {
        public String title;
        public String text;
    }
    public static class Data{
        public String title;
        public String text;
    }

}
