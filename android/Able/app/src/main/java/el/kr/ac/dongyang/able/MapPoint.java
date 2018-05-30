package el.kr.ac.dongyang.able;

/**
 * Created by impro on 2018-05-23.
 * 아직 안씀. 쓸지 말지도 잘 모르겠음. 노드마다 한번씩 찍어주려면 꼭 써야할지도..
 */

public class MapPoint {
    private String Name;
    private double latitude;
    private double longitude;

    public MapPoint(){
        super();
    }

    public MapPoint(String Name, double latitude, double longitude) {
        //super();
        this.Name = Name;
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public String getName() {
        return Name;
    }

    public void setName(String Name) {
        this.Name = Name;
    }

    public double getLatitude() {
        return latitude;
    }

    public void setLatitude(double latitude) {
        this.latitude = latitude;
    }

    public double getLongitude() {
        return longitude;
    }

    public void setLongitude(double longitude) {
        this.longitude = longitude;
    }
}
