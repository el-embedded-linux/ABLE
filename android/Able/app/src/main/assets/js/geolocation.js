var map, markerA, markerB, markerC, marker
var markerLayer_s = new Tmap.Layer.Markers("start");// 마커 레이어 생성 시작지
var markerLayer_e = new Tmap.Layer.Markers("end");  // 목적지
var markerLayer_i = new Tmap.Layer.Markers("iam");  // 현재위치
var markerLayer = new Tmap.Layer.Markers();
var routeLayer = new Tmap.Layer.Vector("route");
var pointLayer = new Tmap.Layer.Vector("point");
routeLayer.style ={
    fillColor:"#FF0000",
    fillOpacity:0.2,
    strokeColor: "#FF0000",
    strokeWidth: 3,
    strokeDashstyle: "solid",
    pointRadius: 2,
    title: "this is a red line"	
};
var lonlat = new Tmap.LonLat(126.984895, 37.566369).transform("EPSG:4326", "EPSG:3857");//좌표 설정
var lonlat2 = new Tmap.LonLat(126.984895, 37.566369).transform("EPSG:4326", "EPSG:3857");//좌표 설정
var geolocation = navigator.geolocation;

var icon_s = icon("s");
var icon_e = icon("e");
var icon_i = icon("i");

var start_x;
var start_y;
var end_x;
var end_y;

var input_s = false;
var input_e = false;

var prtcl;
var kmlForm;
var pointFeature;
var vectorcode = 0;

// 홈페이지 로딩과 동시에 맵을 호출할 함수
function initTmap(){
    map = new Tmap.Map({
        div:'map_div',
        width : '100%',
        height : '100%'
    });

    //map.events.register("touchend", map, onClick);

    map.addLayer(markerLayer_s); // 맵에 마커레이어 추가
    map.addLayer(markerLayer_e);
    map.addLayer(markerLayer_i);
    map.addLayer(markerLayer);
    map.addLayer(routeLayer);
    map.addLayer(pointLayer);

    // HTML5의 geolocation으로 사용할 수 있는지 확인합니다. 
    // 현재 위치 정보를 얻어오는 메서드이다. 사용자가 허용을 할 경우 실행된다.
        // GeoLocation을 이용해서 접속 위치를 얻어옵니다.
        //이것도 안됨
        if (geolocation in navigator) {
            geoLocation("s");
        }
}

// 나의 위치정보를 나타낼 메서드. 안됨
function geoLocation(location) {
    navigator.geolocation.getCurrentPosition(function(position){
        // 마커가 표시될 위치를 geolocation으로 얻어온 좌표로 생성합니다
        lat = position.coords.latitude; // 위도
        lon = position.coords.longitude; // 경도
        moveCoordinate(location, lon, lat);
    });
}

function moveCoordinate (value, x, y) {
    var PR_3857 = new Tmap.Projection("EPSG:3857");  // Google Mercator 좌표계인 EPSG:3857
    var PR_4326 = new Tmap.Projection("EPSG:4326");  // WGS84 GEO 좌표계인 EPSG:4326

    lonlat = new Tmap.LonLat(x, y).transform(PR_4326, PR_3857);
    setXY(value, x, y);
    setMarker(value,lonlat);
    map.setCenter(lonlat,15); // geolocation으로 얻어온 좌표로 지도의 중심을 설정합니다.
}

//네이티브에서 좌표 받아서 지도에 표시
function geoLo(x,y) {
    moveCoordinate("i", y,x)

    var geolonlat ='현재 위치의 좌표는'+ x + ',' + y +'입니다.';
    //ar resultDiv = document.getElementById("geolonlat");
    //resultDiv.innerHTML = geolonlat;
}

// 맵 클릭할 경우 마커 표시 메서드, setLocation 호출
function onClick(e){
    lonlat = map.getLonLatFromViewPortPx(e.xy).transform("EPSG:3857", "EPSG:4326");
    //클릭 부분의 ViewPortPx를 LonLat 좌표로 변환합니다.
    var resultlonlat ='클릭한 위치의 좌표는'+lonlat+'입니다.';
	var resultDiv = document.getElementById("resultlonlat");
	resultDiv.innerHTML = resultlonlat;
    x = lonlat.lon;
    y = lonlat.lat;

//출발지가 없을때 목적지가 설정 안되도록. 순차적으로 되도록 하는 코드
    if(input_s == 0) {
        if(input_e == 0) {
            removeMarker("e");
            resetResult();
        }
        removeMarker("s");
        setLocation("#start", x, y, lonlat);
    } else if(input_e == 0) {
        removeMarker("e");
        setLocation("#end", x, y, lonlat);
    } else {
        removeMarker("s");
        removeMarker("e");
        reset();
    }
}

// 출력 정보 리셋
function resetResult() {
    $("#result").text("");
    $("#result1").text("");
    $("#result2").text("");
    $("#result3").text("");
}

//마커 위치 설정 -> setXY 호출
function setLocation(value, x, y, lonlat) {
    if (value == "#start"){
        setXY("s", x, y);
        lonlat = lonlat.transform("EPSG:4326", "EPSG:3857"); //마커 정보 등록
        setMarker("s");
    } else if(value == "#end") {
        setXY("e", x, y);
        lonlat = lonlat.transform("EPSG:4326", "EPSG:3857"); //마커 정보 등록
        setMarker("e");
    }else if(value == "#iam") {
             setXY("i", x, y);
             lonlat = lonlat.transform("EPSG:4326", "EPSG:3857"); //마커 정보 등록
             setMarker("i");
    }
}

function setMarker(value, lonlat) {
    if(value == "s") {
        markerLayer_s.removeMarker(markerA);
        markerA = new Tmap.Marker(lonlat, icon_s); //마커 정보 등록
        markerLayer_s.addMarker(markerA);
    } else if(value == "e") {
        markerLayer_e.removeMarker(markerB);
        markerB = new Tmap.Marker(lonlat, icon_e);
        markerLayer_e.addMarker(markerB);
    } else if(value == "i") {
        markerLayer_i.removeMarker(markerC);
        markerC = new Tmap.Marker(lonlat, icon_i);
        markerLayer_i.addMarker(markerC);
          }
}

function icon(value) {
    if(value == "e"){
        value = "b_b_e";
    } else if(value == "i"){
        value = "g_b_i";
    }else if(value == "s"){
        value = "r_b_s";
    }else{
        value = "r_b_s";
    }
    var size = new Tmap.Size(24, 38);
    var offset = new Tmap.Pixel(-(size.w / 2), -(size.h));
    var icon = new Tmap.IconHtml('<img src=http://tmapapis.sktelecom.com/upload/tmap/marker/pin_'+value+'.png />',size, offset);
    return icon;
}

function removeMarker(value) {
    if(value == "s") {
        markerLayer_s.removeMarker(markerA);
        markerA = null;
        start_x = null;
        start_y = null;
        $("#start").val("");
    } else if(value == "e") {
        markerLayer_e.removeMarker(markerB);
        markerB = null;
        end_x = null;
        end_y = null;
        $("#end").val("");
    }
}

//searchAdress 호출
function setXY(value, x, y) {
    if(value == "s") {
        start_x = x;
        start_y = y;
        //searchAdress("#start", y, x);
    } else if(value == "e") {
        end_x = x;
        end_y = y;
        //searchAdress("#end", y, x);
    } else if(value == "i") {
        end_x = x;
        end_y = y;
        //searchAdress("#iam", y, x);
    } else {
        console.log("value Error");
    }
}

function reset () {
    $("#start").val(null);
    $("#end").val(null);
    removeMarker("s");
    removeMarker("e");
    map.removeLayer(routeLayer);
}

//출발지등록 요구
function go() {
    if (input_s == 1 && input_e == 1) {
        distance();
    } else if(input_s == 0){
        alert("출발지를 등록하세요!");
    } else {
        alert("도착지를 등록하세요!");
    }
}

var headers = {}; 
headers["appKey"]="cadda216-ac54-435a-a8ea-a32ba3bb3356";//실행을 위한 키 입니다. 발급받으신 AppKey를 입력하세요.

function sendMessage(arg){
     window.tmap.setMessage(arg);
}

function sendDescription(arg){
     window.tmap.setDescription(arg);
}

function sendTimeDistance(arg){
    window.tmap.setTimeDistance(arg);
}

//경로탐색
function distance(startx, starty, endx, endy) {
    if(vectorcode == 1){
        map.removeLayer(pointLayer);
        map.removeLayer(routeLayer);
        routeLayer.removeAllFeatures();
        pointLayer.removeAllFeatures();
    }
    start_x = startx;
    start_y = starty;
    end_x = endx;
    end_y = endy;

    lonlat = new Tmap.LonLat(start_x, start_y).transform("EPSG:4326", "EPSG:3857");
    setXY("s", start_x, start_y);
    setMarker("s",lonlat);

    lonlat2 = new Tmap.LonLat(end_x, end_y).transform("EPSG:4326", "EPSG:3857");
    setXY("e", end_x, end_y);
    setMarker("e",lonlat2);

    if (start_x != null && end_x != null) {
            $.ajax({
                method:"POST",
                headers:headers,
                url:"https://api2.sktelecom.com/tmap/routes?version=1",
                data:{
                    startX:start_x,
                    startY:start_y,
                    endX:end_x,
                    endY:end_y,
                    reqCoordType : "WGS84GEO",
                    resCoordType : "EPSG3857",
                    angle:"172",
                    searchOption:0
                },
                success:function(data) {
                    var obj = JSON.stringify(data);
                    obj = JSON.parse(obj);
                    var total = obj.features[0].properties;

                    var time = "";
                    if((total.totalTime*3) > 3600) {
                        time = Math.floor((total.totalTime*3)/3600) + "시간 " + Math.floor((total.totalTime*3)%3600/60) + "분";
                    } else {
                        time = Math.floor((total.totalTime*3)%3600/60) + "분 ";
                    }
                    var distance = total.totalDistance/1000;
                    console.log(time);
                    console.log(distance);

                    var timeDistance = time + "," + distance + "km";
                    sendTimeDistance(timeDistance);

                },
                error:function(request,status,error){
                    alert("출발지 혹은 도착지를 잘못 지정하였습니다.");
                    console.log("code:"+request.status+"\n"+"message:"+request.responseText+"\n"+"error:"+error);
                }
            });
        }

    if (start_x != null && end_x != null) {
        $.ajax({
            method:"POST",
            headers:headers,
            url:"https://api2.sktelecom.com/tmap/routes?version=1&format=xml",
            data:{
                startX:start_x,
                startY:start_y,
                endX:end_x,
                endY:end_y,
                reqCoordType : "WGS84GEO",
                resCoordType : "EPSG3857",
                angle:"172",
                searchOption:0
            },
            success:function(response) {
                vectorcode = 1;
                prtcl = response;

                map.addLayer(routeLayer);

        		// 5. 경로 탐색  결과 Line 그리기
        		//경로 탐색  결과 POINT 찍기
        		/* -------------- Geometry.Point -------------- */
        		var prtclString = new XMLSerializer().serializeToString(prtcl);//xml to String
        		   xmlDoc = $.parseXML( prtclString ),
        		   $xml = $( xmlDoc ),
        		   $intRate = $xml.find("Placemark");
        		   var style_red = {
        		           fillColor:"#188DEF",
        		           fillOpacity:0.2,
        		           strokeColor: "#188DEF",
        		           strokeWidth: 3,
        		           strokeDashstyle: "solid",
        		           pointRadius: 2,
        		           title: "this is a blue line"
        		      };
        		   $intRate.each(function(index, element) {
        		   	var nodeType = element.getElementsByTagName("tmap:nodeType")[0].childNodes[0].nodeValue;
        			if(nodeType == "POINT"){
        			    var description;
        			    description = element.getElementsByTagName("description")[0].childNodes[0].nodeValue;
        			    //console.log("code:"+description);
        			    sendDescription(description);
        			    //"code:"+request.status+"\n"+"message:"+request.responseText+"\n"+"error:"+error

        				var point = element.getElementsByTagName("coordinates")[0].childNodes[0].nodeValue.split(',');
        				var lonlat = new Tmap.LonLat(point).transform("EPSG:3857", "EPSG:4326");
        				//console.log("code:"+lonlat);
                        sendMessage(lonlat.toString());

        				var geoPoint =new Tmap.Geometry.Point(point[0],point[1]);
        				var pointFeature = new Tmap.Feature.Vector(geoPoint, null, style_red);
        				pointLayer.addFeatures([pointFeature]);
        			}
        		   });

        		map.addLayer(pointLayer);

        		/* -------------- Geometry.Point -------------- */
        		//경로 탐색  결과 Line 그리기
        		routeLayer.style ={
        				fillColor:"#188DEF",
        		        fillOpacity:0.2,
        		        strokeColor: "#188DEF",
        		        strokeWidth: 3,
        		        strokeDashstyle: "solid",
        		        pointRadius: 2,
        		        title: "this is a blue line"
        		}
        		kmlForm = new Tmap.Format.KML().read(prtcl);
        		routeLayer.addFeatures(kmlForm);

        		// 6. 경로탐색 결과 반경만큼 지도 레벨 조정
        		map.zoomToExtent(routeLayer.getDataExtent());

        	},
        	error:function(request,status,error){
        		console.log("code:"+request.status+"\n"+"message:"+request.responseText+"\n"+"error:"+error);
        	}
        });
    }
}

//에러시 발생. 마커 초기화
function alertAdress(input) {
    alert("제공되지 않는 주소 범위입니다.");
        if(input == "#start") {
            removeMarker("s");
        } else {
            removeMarker("e");
        }
}

function startInputS() {
    input_s = 1;
}
function endInputS() {
    input_s = 0;
}
function startInputE() {
    input_e = 1;
}
function endInputE() {
    input_e = 0;
}
