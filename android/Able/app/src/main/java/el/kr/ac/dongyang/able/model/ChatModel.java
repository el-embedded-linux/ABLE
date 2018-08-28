package el.kr.ac.dongyang.able.model;

import java.util.HashMap;
import java.util.Map;

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

    public static class Comment {
        public String uid;
        public String message;
        public Object timestamp;
        public Map<String,Object> readUsers = new HashMap<>();
    }
}
