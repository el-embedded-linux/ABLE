package el.kr.ac.dongyang.able;

import com.squareup.otto.Bus;

/**
 * Created by impro on 2018-06-06.
 */

public class BusProvider extends Bus {
    private static final Bus BUS = new Bus();

    public static Bus getInstance() {
        return BUS;
    }

    private BusProvider() {
    }
}
