package el.kr.ac.dongyang.able.model;

//헬스케어에 필요한 데이터클래스
public class HealthModel {
    private String kcal;
    private String speed;
    private String distance;

    public String getKcal() {
        return kcal;
    }
    public void setKcal(String kcal) {
        this.kcal = kcal;
    }
    public String getSpeed() {
        return speed;
    }
    public void setSpeed(String speed) {
        this.speed = speed;
    }
    public String getDistance() {
        return distance;
    }
    public void setDistance(String distance) {
        this.distance = distance;
    }
}
