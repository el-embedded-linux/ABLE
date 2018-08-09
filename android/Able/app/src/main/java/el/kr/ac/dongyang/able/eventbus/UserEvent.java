package el.kr.ac.dongyang.able.eventbus;

public class UserEvent {
    String userId;

    public UserEvent(String userId) {
        this.userId = userId;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }
}
