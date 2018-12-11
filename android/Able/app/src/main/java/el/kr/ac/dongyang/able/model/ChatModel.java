package el.kr.ac.dongyang.able.model;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

//채팅에 필요한 데이터클래스
public class ChatModel {

    private Map<String, Boolean> users = new HashMap<>();    //채팅방의 유저들
    private Map<String, Comment> comments = new HashMap<>(); //채팅방의 대화내용

    public Map<String, Boolean> getUsers() {
        return users;
    }

    public void setUsers(Map<String, Boolean> users) {
        this.users = users;
    }

    public Map<String, Comment> getComments() {
        return comments;
    }

    public void setComments(Map<String, Comment> comments) {
        this.comments = comments;
    }

    public static class Comment implements Serializable {
        public String uid;
        public String message;
        public Object timestamp;
        //경도 세자리수 126.123123123
        public String myLonitude;
        public String destinationLongitude;
        //위도 두자리수 32.123123123
        public String myLatitude;
        public String destinationLatitude;
        public Map<String,Object> readUsers = new HashMap<>();
        public boolean naviShare;
        public String destinationAddress;

        public Comment() {
        }

        public Comment(String uid, String message, Object timestamp) {
            this.uid = uid;
            this.message = message;
            this.timestamp = timestamp;
        }

        public Comment(String uid, String message, Object timestamp, boolean naviShare) {
            this.uid = uid;
            this.message = message;
            this.timestamp = timestamp;
            this.naviShare = naviShare;
        }
    }
}
