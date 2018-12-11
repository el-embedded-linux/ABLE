package el.kr.ac.dongyang.able;

import com.squareup.otto.Bus;

//로그인시 회원의 아이디를 메인화면의 텍스트뷰로 전달하는 이벤트버스
public class BusProvider extends Bus {
    private static final Bus BUS = new Bus();

    public static Bus getInstance() {
        return BUS;
    }

    private BusProvider() {
    }
}
