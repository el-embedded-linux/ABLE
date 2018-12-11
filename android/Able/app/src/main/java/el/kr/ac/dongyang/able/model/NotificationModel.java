package el.kr.ac.dongyang.able.model;

//푸시알림에 필요한 데이터클래스
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
