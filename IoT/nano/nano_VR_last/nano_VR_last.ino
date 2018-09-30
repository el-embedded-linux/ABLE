//TFTLCD
#include <Time.h>
#include <TimeLib.h>
#include "SPI.h"
#include "Adafruit_GFX.h"
#include "Adafruit_ILI9340.h"

#if defined(__SAM3X8E__)
    #undef __FlashStringHelper::F(string_literal)
    #define F(string_literal) string_literal
#endif
#define _sclk 13
#define _miso 12
#define _mosi 11
#define _cs 10
#define _dc 9
#define _rst 8
Adafruit_ILI9340 tft = Adafruit_ILI9340(_cs, _dc, _rst);

String today;

//reedSW
float radius = 40; // 바퀴당 이동 거리를 확인 하기 위해 자전거 바퀴의 반지름을 입력해 줍니다.(Cm 단위)
float circle = (2 * radius * 3.14) / 100;  // 자전거 바퀴의 둘레를 계산(단위를 m로 바꿔주기 위해 100을 나눕니다.)

float bySpeed = 0; // 자전거의 속도
float ckTime = 0;  // 리드스위치가
float uckTime = 0; // Unckecked
float cycleTime = 0;  // 리드스위치가 인식이 안됬을 시간 부터 인식됬을 때까지의 시간
float distance = 0; // 자전거의 누적 이동 거리
float lcdDis = 0; // 자전거의 이동 거리를 LCD출력에 맞게 바꿔즌 값.(단위 수정 or 소숫점 제거)

int count = 0;  // 리드스위치의 노이즈를 제거하기 위해 카운트를 넣어줍니다.
boolean temp = 0;  // 리드 스위치가 닫혔는지 확인하는 변수

//Btn
int btLCD_up, btLCD_down;
#define BTLCD_UP 5//TFTLCD 위로 버튼
#define BTLCD_DOWN 4//TFTLCD 아래 버튼
int chmod=1000; //lcd모드를 변경해줌
#define BTR 7 //오른쪽
#define BTL 6 //왼쪽
int btLeft, btRight;  // 좌우 버튼
//int c = 0;                  // 버튼 동시 제어 변수
int presRead;               // 압력센서
int presPoint;
#define PRESSURE A0

//Heart_Beat
#include "MAX30105.h"
#include <heartRate.h>

MAX30105 particleSensor;
const byte RATE_SIZE = 4; //Increase this for more averaging. 4 is good.
byte rates[RATE_SIZE]; //Array of heart rates
byte rateSpot = 0;
long lastBeat = 0; //Time at which the last beat occurred
long irValue = 0;
float beatsPerMinute = 0;
int beatAvg = 0;

//Thread
#include <Thread.h>
#include <ThreadController.h>
#include <Stepper.h>
ThreadController controll = ThreadController();

Thread myThread_tftLCD = Thread();
Thread myThread_btnLCD = Thread();
Thread myThread_reed = Thread();
Thread myThread_heart = Thread();
Thread myThread_press = Thread();
Thread myThread_btn = Thread();

//LED
#include <Adafruit_NeoPixel.h>
#define BARWING 3     // 바 LED
#define ADAMATRIX 2  // 매트릭스 LED
#define NUMPIXELS 64  // 매트릭스 LED개수
#define BARPIXELS 28  // 바 LED개수

Adafruit_NeoPixel matrix = Adafruit_NeoPixel(NUMPIXELS, ADAMATRIX, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel bar = Adafruit_NeoPixel(BARPIXELS, BARWING, NEO_GRB + NEO_KHZ800);

//LED arrays
int humi[4][64]={
  { 0, 0, 0, 1, 0, 0, 0, 0,
    0, 0, 1, 1, 0, 0, 0, 0,
    0, 1, 1, 0, 0, 0, 0, 0,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    0, 1, 1, 0, 0, 0, 0, 0,
    0, 0, 1, 1, 0, 0, 0, 0,
    0, 0, 0, 1, 0, 0, 0, 0
  }, 
  { 0, 0, 0, 0, 1, 0, 0, 0,
    0, 0, 0, 0, 1, 1, 0, 0,
    0, 0, 0, 0, 0, 1, 1, 0,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    0, 0, 0, 0, 0, 1, 1, 0,
    0, 0, 0, 0, 1, 1, 0, 0,
    0, 0, 0, 0, 1, 0, 0, 0
  },
  { 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 0, 0, 0, 0, 1, 1,
    1, 0, 1, 0, 0, 1, 0, 1,
    1, 0, 0, 1, 1, 0, 0, 1,
    1, 0, 0, 1, 1, 0, 0, 1,
    1, 0, 1, 0, 0, 1, 0, 1,
    1, 1, 0, 0, 0, 0, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1
  },
  { 0, 0, 1, 1, 1, 1, 0, 0,
    0, 1, 1, 0, 0, 1, 1, 0,
    1, 1, 0, 0, 1, 1, 1, 1,
    1, 0, 0, 1, 1, 1, 0, 1,
    1, 0, 1, 1, 1, 0, 0, 1,
    1, 1, 1, 1, 0, 0, 1, 1,
    0, 1, 1, 0, 0, 1, 1, 0,
    0, 0, 1, 1, 1, 1, 0, 0
  }
};

int matrixColor[4][3] = {
  {0, 20, 0},
  {0, 20, 0},
  {40, 0, 0},
  {40, 0, 0},
};

int barShape[3][28] = {
  { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
  { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
};

int barColor[3][3] = {
  {0, 20, 0}, {0, 20, 0}, {40, 0, 0}
};

void btnCallback(){
  // Matrix LED + Bar LED
  showMatrix();
  showPres(presRead, PRESSURE, presPoint);
}

void pressCallback()
{
  int SensorReading = analogRead(PRESSURE); 
 
  int mfsr_r18 = map(SensorReading, 0, 1024, 0, 255);
  Serial.println(mfsr_r18);
}

void reedCallback(){
  boolean check = digitalRead(A1); // 리드스위치의 상태를 확인합니다.(SIG=A1)
  Serial.print(check);
  if(check == 1 && temp == 0){  // 리드 스위치가 열릴 때(닫힘 -> 열림)
    ckTime = millis();  // 시간을 확인해서 저장합니다.
    temp = 1;  // temp값을 1로 바꿔줍니다.(리드스위치가 열려있는 상태값 저장)
  }
  
  else if(check == 0 && temp == 1 && count > 5){  // 리드 스위치가 닫히고(열림 -> 닫힘), 노이즈 방지 카운트가 5이상일때
    uckTime = millis();  // 시간을 확인해서 저장합니다.
      
    cycleTime = (uckTime - ckTime) / 1000;
    // 열릴 때 시각과 닫힐 때 시각의 차를 이용하여 바퀴가 한바퀴 돌때 걸린 시간을 계산합니다.
    bySpeed = (circle / cycleTime) * 3.6; // 바퀴가 한바퀴 돌때의 거리와 시간을 가지고 속도를 구해줍니다.(단위는 Km/h입니다.)
    temp = 0; 
    count = 0; 
    distance += (circle/1000);  // 한바퀴 돌았으면 이동거리를 누적 이동거리에 더해줍니다.(KM단위를 위해 1000으로 나눠줌)
  }
  
  if(check == 1){  // 리드 스위치가 열려있으면 카운트를 1씩 증가 시켜 줍니다.
    count++;
    if(count > 150){ // 카운트가 150이 넘어가면(자전거가 멈췄을 때) 속도를 0으로 바꿔줍니다.
      bySpeed = 0;
    }
  }  
}

void btLCDCallback(){
  btLCD_up = digitalRead(BTLCD_UP);
  btLCD_down = digitalRead(BTLCD_DOWN);
  
  if(btLCD_up == LOW){
    tft.fillScreen(ILI9340_BLACK);
    if(chmod>2001||chmod<2){
      chmod=1000;
    }else{
    chmod +=1;
    }
  }
  if(btLCD_down == LOW){
    tft.fillScreen(ILI9340_BLACK);
    if(chmod>2001||chmod<2){
      chmod=1000;
    }else{
    chmod -=1;
    }
  }
}

void tftLCDCallback(){
  if(chmod%3==0){
    distanceText();
  }else if(chmod%3==1){
    speedText();
  }else if(chmod%3==2){
    heartText();
  }
}

void heartCallback(){
  if (checkForBeat(irValue) == true)
  {
    //We sensed a beat!
    long delta = millis() - lastBeat;
    lastBeat = millis();

    beatsPerMinute = 60 / (delta / 1000.0);

    if (beatsPerMinute < 255 && beatsPerMinute > 20)
    {
      rates[rateSpot++] = (byte)beatsPerMinute; //Store this reading in the array
      rateSpot %= RATE_SIZE; //Wrap variable

      //Take average of readings
      beatAvg = 0;
      for (byte x = 0 ; x < RATE_SIZE ; x++)
        beatAvg += rates[x];
      beatAvg /= RATE_SIZE;
    }
  }
}

void setup() {
 #if defined (__AVR_ATtiny85__) 
  if (F_CPU == 16000000) clock_prescale_set(clock_div_1); 
  #endif 
  Serial.begin(9600);
  while (!Serial);
  pinMode(BTLCD_UP, INPUT);//tftLCD 버튼 up
  pinMode(BTLCD_DOWN, INPUT);//tftLCD 버튼 down
  setTime(01,37,0,22,9,18);
  date();
  Serial.println(today);

 
  if (!particleSensor.begin(Wire, I2C_SPEED_FAST)) //Use default I2C port, 400kHz speed
  {
    Serial.println("MAX30105 was not found. Please check wiring/power. ");
    while (1);
  }
  
  particleSensor.setup(); //Configure sensor with default settings
  particleSensor.setPulseAmplitudeRed(0x0A); //Turn Red LED to low to indicate sensor is running
  particleSensor.setPulseAmplitudeGreen(0); //Turn off Green LED
  
  Serial.println("Place your index finger on the sensor with steady pressure.");

  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(ILI9340_BLACK);

  //thread
  myThread_tftLCD.onRun(tftLCDCallback);
  myThread_reed.onRun(reedCallback);
  myThread_btnLCD.onRun(btLCDCallback);
  myThread_press.onRun(pressCallback);
  myThread_btn.onRun(btnCallback);
  myThread_heart.onRun(heartCallback);
  
  controll.add(&myThread_reed);
  controll.add(&myThread_tftLCD);
  controll.add(&myThread_btnLCD);
  controll.add(&myThread_heart);
  controll.add(&myThread_press);
  controll.add(&myThread_btn);
  controll.add(&myThread_heart);
}

void loop(void) {
  controll.run();
}

void showPres(int presRead1, int PRESSURE1, int presPoint1)
{
  presRead1 = analogRead(PRESSURE1);
  presPoint1 = map(presRead1, 0, 1024, 0,255);
  Serial.println(presPoint1);
  if(presPoint1 >= 15) {  // 값을 수정해야할지도
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(humi[2][i]*matrixColor[2][0], humi[2][i]*matrixColor[2][1], humi[2][i]*matrixColor[2][2]));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(barShape[2][i]*barColor[2][0], barShape[2][i]*barColor[2][1], barShape[2][i]*barColor[2][2]));
    }
  matrix.show();
  bar.show();
  }
}

void showMatrix() 
{
  
}

void date(){
  today = year();
  today = today+"-";
  today = today+month();
  today = today+"-";
  today = today+day();
}

unsigned long distanceText() {
  unsigned long start = micros();
  
  tft.setCursor(85, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Distance");
  tft.println("");

  if(distance <10){
    tft.setCursor(20, 120);
  }
  else if (distance <100){
    tft.setCursor(10, 120);
  }
  else if (distance <1000){
    tft.setCursor(40, 120);
  }

   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
   tft.print(distance);
   tft.println("km");

  return micros() - start;
}

unsigned long speedText() {
  unsigned long start = micros();
  
  tft.setCursor(85, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(110, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Speed");
  tft.println("");


  //속도 출력
  if(bySpeed <10){
    tft.setCursor(50, 110);
  }
  else if (bySpeed <100){
    tft.setCursor(60, 110);
  }

   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
   tft.println(bySpeed);

   tft.setCursor(120, 180);
   tft.setTextSize(3);
   tft.println("km/s");

  return micros() - start;
}

unsigned int heartText(){
//  tft.fillScreen(ILI9340_BLACK);
//  long irValue = particleSensor.getIR();
  unsigned long start = micros();
  beatsPerMinute = 30;
  tft.setCursor(85, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Heart Rate");
  tft.println("");

  //심박도 출력(todo 손가락이 없는 경우, 정확도 이슈)
//  if(beatAvg==0 && irValue > 50000){
//   tft.setCursor(10, 110);
//   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
//   tft.println("Wait a miniue");
//  }else if(beatAvg <100 && irValue > 50000){
//   tft.setCursor(60, 110);
//   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
//   tft.println(beatAvg);
//   tft.setCursor(120, 180);
//   tft.setTextSize(3);
//   tft.println("BPM");
//  }
//  else if (beatAvg >=100 && irValue > 50000){
//   tft.setCursor(70, 110);
//   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
//   tft.println(beatAvg);
//   tft.setCursor(120, 180);
//   tft.setTextSize(3);
//   tft.println("BPM");
//  }else if ( irValue < 50000){
//   tft.setCursor(30, 110);
//   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
//   tft.println("No finger?");
//  }

  if(beatsPerMinute <100){
    tft.setCursor(60, 110);
  }
  else if (beatsPerMinute >=100){
    tft.setCursor(70, 110);
  }

   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
   tft.println(beatsPerMinute);

   tft.setCursor(120, 180);
   tft.setTextSize(3);
   tft.println("BPM");
   
   return micros() - start;
}
