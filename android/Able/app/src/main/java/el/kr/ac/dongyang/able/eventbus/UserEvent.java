package el.kr.ac.dongyang.able.eventbus;

//로그인 후 MainActivity 의 사용자 이름 부분에 BusProvider 로 전달할 데이터의 VO
public class UserEvent {
    private String userId;

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
