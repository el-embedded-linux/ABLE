package el.kr.ac.dongyang.able;

import com.squareup.otto.Bus;

//네비게이션의 좌표값을 FragmentSetting 로 전달하는 이벤트버스
public class BusProviderSetting extends Bus {
    private static final Bus BUS = new Bus();

    public static Bus getInstance() {
        return BUS;
    }

    private BusProviderSetting() {
    }
}
