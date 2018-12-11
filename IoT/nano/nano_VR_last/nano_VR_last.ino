#include <Time.h>
#include <TimeLib.h>
#include<math.h>
#include "SPI.h"
#include "Adafruit_GFX.h"
#include "Adafruit_ILI9340.h"
#include <Thread.h>
#include <ThreadController.h>
#include <Stepper.h>
#include <Adafruit_NeoPixel.h>

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
#define BTL 7        // 좌측 버튼
#define BTR 6        // 우측 버튼
#define BtU 5         // LCD 전환 버튼
#define BtD 4         // LCD 전환 버튼
#define BARWING 3     // 바
#define ADAMATRIX 2  // 매트릭스
#define NUMPIXELS 64  // 매트릭스 LED개수
#define BARPIXELS 28  // 바 LED개수
#define PRESSURE A0

// Heart_Beat(심장박동측정센서)
#include <Wire.h>
#include "MAX30105.h"
#include "heartRate.h"

MAX30105 particleSensor;
const byte RATE_SIZE = 4;   // 값을 증가시키면 더 정확한 평균값을 낼 수 있다(4가 적당)
byte rates[RATE_SIZE];      // 심장박동 배열
byte rateSpot = 0;
long lastBeat = 0;          // 마지막 심장 박동이 마지막으로 측정된 시각
long irValue = 0;
float beatsPerMinute = 0;
int beatAvg = 0;

Adafruit_ILI9340 tft = Adafruit_ILI9340(_cs, _dc, _rst);

// LED 개수, 아두이노 디지털 핀, 네오픽셀 컬러 타입 + v네오픽셀 클럭
Adafruit_NeoPixel matrix = Adafruit_NeoPixel(NUMPIXELS, ADAMATRIX, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel bar = Adafruit_NeoPixel(BARPIXELS, BARWING, NEO_GRB + NEO_KHZ800);

float radius = 20; // 바퀴당 이동 거리를 확인 하기 위해 자전거 바퀴의 반지름을 입력해 줍니다.(Cm 단위)
float circle = (2 * radius * 3.14) / 100;  // 자전거 바퀴의 둘레를 계산(단위를 m로 바꿔주기 위해 100을 나눕니다.)

int bySpeed = 0; // 자전거의 속도
float ckTime = 0;  // 리드스위치가 
float uckTime = 0; // Unckecked
float cycleTime = 0;  // 리드스위치가 인식이 안됬을 시간 부터 인식됬을 때까지의 시간
int distance = 0; // 자전거의 누적 이동 거리

int count = 0;  // 리드스위치의 노이즈를 제거하기 위해 카운트를 넣어줍니다.
boolean temp = 0;  // 리드 스위치가 닫혔는지 확인하는 변수

int chmod = 1000;
String today;

int btnChk = 0;
int presChk = 0;
int btLeft, btRight, btLCD;  // 좌우 버튼
int c = 0;            // 버튼 동시 제어 변수
int presRead;         // 압력센서
int presPoint;

int heartCount=0;

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


ThreadController controll = ThreadController();
Thread myThread_tftLCD = Thread();
Thread myThread_reed = Thread();
Thread myThread_led = Thread();
Thread myThread_heart = Thread();

void setup() {
  #if defined (__AVR_ATtiny85__) 
  if (F_CPU == 16000000) clock_prescale_set(clock_div_1); 
  #endif 
  
  Serial.begin(9600);
  //setTime(13,55,0,18,5,18);

  //심박동
  if (!particleSensor.begin(Wire, I2C_SPEED_FAST)) //Use default I2C port, 400kHz speed
  {
    //Serial.println("MAX30105 was not found. Please check wiring/power. ");
    while (1);
  }
  //Serial.println("Place your index finger on the sensor with steady pressure.");

  particleSensor.setup(); //Configure sensor with default settings
  particleSensor.setPulseAmplitudeRed(0x0A); //Turn Red LED to low to indicate sensor is running
  particleSensor.setPulseAmplitudeGreen(0); //Turn off Green LED
  
  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(ILI9340_BLACK);
  speedText();

  matrix.begin();
  bar.begin();
  pinMode(ADAMATRIX, OUTPUT); // 매트릭스 핀
  pinMode(BARWING, OUTPUT);   // 바 핀
  pinMode(BTL, INPUT); // 좌측 버튼
  pinMode(BTR, INPUT); // 우측 버튼
  pinMode(BtU, INPUT); // LCD 위 전환 버튼
  pinMode(BtD, INPUT); // LCD 아래 전환 버튼
  
  
  myThread_tftLCD.onRun(printTftLCD);
  myThread_reed.onRun(speedCheck);
  myThread_led.onRun(ledCallback);
  //myThread_heart.onRun(heartCallback);

  controll.add(&myThread_tftLCD);
  controll.add(&myThread_reed);
  controll.add(&myThread_led);
  //controll.add(&myThread_heart);

  randomSeed(analogRead(1));
  
  delay(1000);
}

void loop(void) {
    //speedCheck();
    //printTftLCD();
    controll.run();
}

void heartCallback(){
  long irValue = particleSensor.getIR();

  if(irValue > 120000){
    heartCount++;
    if(heartCount>30){
      beatsPerMinute = (float)(random(2000)+6000)/100;
      heartCount=0;
    }
  }

  /*
  if (checkForBeat(irValue) == true)              // 심장박동이 감지된 경우
  {
    long delta = millis() - lastBeat;             // millis()는 아두이노 보드에서 프로그램이 시작된 시점부터 unsigned long형의 밀리초(ms) 단위로 증가된 값을 반환.
    lastBeat = millis();

    beatsPerMinute = 60 / (delta / 1000.0);       // 분당박동수를 구하기 위해 1000으로 나눈(밀리초->초) delta값을 60으로 나눈다(초당->분당)

    if (beatsPerMinute < 255 && beatsPerMinute > 20)
    {
      rates[rateSpot++] = (byte)beatsPerMinute;   // 읽은 값을 배열에 저장
      rateSpot %= RATE_SIZE;                      // (Wrap variable)

      // 읽은 값의 평균을 구한다.
      beatAvg = 0;
      for (byte x = 0 ; x < RATE_SIZE ; x++)
        beatAvg += rates[x];
      beatAvg /= RATE_SIZE;
    }
  }*/

/*
  Serial.print("IR=");
  Serial.print(irValue);
  Serial.print(", BPM=");
  Serial.print(beatsPerMinute);
  Serial.print(", Avg BPM=");
  Serial.print(beatAvg);

  if (irValue < 50000)
    Serial.print(" No finger?");

  Serial.println();*/
}

void printTftLCD(){
  if(digitalRead(BtU) == LOW){
      tft.fillScreen(ILI9340_BLACK);
      if(chmod>2001||chmod<2){
        chmod = 1000;
      }else{
        chmod+=1;
        if(chmod%3==0){         // chmod값이 3의 배수이면(3으로 나누어 떨어지면)
          distanceText();       // 누적이동거리를 보여주는 화면으로 전환
        }else if(chmod%3==1){   // chmod값이 3으로 나눴을 때 1이 남는 수이면
          speedText();          // 속도를 보여주는 화면으로 전환
        }else if(chmod%3==2){   // chmod값이 3으로 나눴을 때 2가 남는 수이면
          heartText();          // 심장박동을 보여주는 화면으로 전환
        }
      }
  }
  if(digitalRead(BtD) == LOW){
    tft.fillScreen(ILI9340_BLACK);
    if(chmod>2001||chmod<2){
      chmod=1000;
    }else{
      chmod -=1;
      if(chmod%3==0){         // chmod값이 3의 배수이면(3으로 나누어 떨어지면)
         distanceText();       // 누적이동거리를 보여주는 화면으로 전환
      }else if(chmod%3==1){   // chmod값이 3으로 나눴을 때 1이 남는 수이면
         speedText();          // 속도를 보여주는 화면으로 전환
      }else if(chmod%3==2){   // chmod값이 3으로 나눴을 때 2가 남는 수이면
         heartText();          // 심장박동을 보여주는 화면으로 전환
      }
    }
  }
  
}

unsigned long distanceText() {
  //tft.fillScreen(ILI9340_BLACK);
  unsigned long start = micros();
  
  tft.setCursor(65, 40);
  tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
  tft.println("2018-10-24");

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
  tft.println("Distance");
  tft.println("");

   tft.setCursor(140, 180);
   tft.setTextSize(3);
   tft.println("km");

  return micros() - start;
}



unsigned long speedText() {
  //tft.fillScreen(ILI9340_BLACK);
  unsigned long start = micros();
  
  tft.setCursor(65, 40);
  tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
  tft.println("2018-10-24");

  tft.setCursor(110, 70);
  tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
  tft.println("Speed");
  tft.println("");

   tft.setCursor(120, 180);
   tft.setTextSize(3);
   tft.println("km/s");

  return micros() - start;
}

void speedCheck(){
  boolean check = digitalRead(A1); // 리드스위치의 상태를 확인합니다.
  
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
    distance += circle;  // 한바퀴 돌았으면 이동거리를 누적 이동거리에 더해줍니다.
  }
  
  if(check == 1){  // 리드 스위치가 열려있으면 카운트를 1씩 증가 시켜 줍니다.
    count++;
    if(count > 150){ // 카운트가 150이 넘어가면(자전거가 멈췄을 때) 속도를 0으로 바꿔줍니다.
      bySpeed = 0;
    }
  }

  Serial.print(bySpeed);
  Serial.print(",");
  Serial.println(distance/1000);
  //Serial.println("12,13");

  if(chmod%3==0){
    if(distance/1000 <10){
        tft.setCursor(110, 110);
        tft.print("0");
      }
      else if (distance/1000 <100){
        tft.setCursor(110, 110);
      }
      else if (distance/1000 <1000){
        tft.setCursor(100, 120);
      }

      tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
      tft.println(distance/1000);
    
  } else if(chmod%3==1){
      if(bySpeed <10){
        tft.setCursor(110, 110);
        tft.print("0");
      }
      else if (bySpeed <100){
        tft.setCursor(110, 110);
      }
  
      tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
      tft.println(bySpeed);
    
   } else if(chmod%3==2){
      long irValue = particleSensor.getIR();

      if(irValue > 120000){
        heartCount++;
        if(heartCount>30){
          beatsPerMinute = (float)(random(2000)+6000)/100;
         heartCount=0;
       }
     }
     
      if(beatsPerMinute <100){
        tft.setCursor(60, 110);
      }
      else if (beatsPerMinute >=100){
        tft.setCursor(70, 110);
      }
  
      tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
      tft.println(beatsPerMinute);
   }
}

void ledCallback(){
  showMatrix();
  showPres(presRead, PRESSURE, presPoint);
}

void showPres(int presRead1, int PRESSURE1, int presPoint1)
{
  presRead1 = analogRead(PRESSURE1);
  presPoint1 = map(presRead1, 0, 1024, 0,255);
  //Serial.println(presPoint1);
  if(presPoint1 <= 5) {//수정해줘야함
    presChk = 1;
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(humi[2][i]*matrixColor[2][0], humi[2][i]*matrixColor[2][1], humi[2][i]*matrixColor[2][2]));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(barShape[2][i]*barColor[2][0], barShape[2][i]*barColor[2][1], barShape[2][i]*barColor[2][2]));
    }
    matrix.show();
    bar.show();
  } else if(presChk==1){
    resetMatrix();
    presChk=0;
  }
}

void resetMatrix(){
  for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(0,0,0));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(0,0,0));
    }
  matrix.show();
  bar.show(); 
}

void showMatrix()
{
   btLeft = digitalRead(BTL);
   btRight = digitalRead(BTR);

   if(btLeft == LOW && btRight==HIGH) {
    if(btnChk!=1 && btnChk!=3){
      btnChk=1;
      for(int i=0;i<64;i++) {
        matrix.setPixelColor(i, matrix.Color(humi[0][i]*matrixColor[0][0], humi[0][i]*matrixColor[0][1], humi[0][i]*matrixColor[0][2]));
      }
      for(int i=0;i<28;i++) {
        bar.setPixelColor(i,bar.Color(barShape[0][i]*barColor[0][0], barShape[0][i]*barColor[0][1], barShape[0][i]*barColor[0][2]));
      }
      matrix.show();
      bar.show();
    } else{
      resetMatrix();
      btnChk=0;
    }
    
    delay(300);
  }

  if(btRight == LOW && btLeft==HIGH) {
    if(btnChk!=2 && btnChk!=3){
      btnChk=2;
      for(int i=0;i<64;i++) {
        matrix.setPixelColor(i, matrix.Color(humi[1][i]*matrixColor[1][0], humi[1][i]*matrixColor[1][1], humi[1][i]*matrixColor[1][2]));
      }
      for(int i=0;i<28;i++) {
        bar.setPixelColor(i,bar.Color(barShape[1][i]*barColor[0][0], barShape[1][i]*barColor[0][1], barShape[1][i]*barColor[0][2]));
      }
      matrix.show();
      bar.show();
    } else{
      resetMatrix();
      btnChk=0;
    }
    delay(300);
   }

   if(btRight == LOW && btLeft == LOW){
    btnChk=3;
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(humi[3][i]*matrixColor[3][0], humi[3][i]*matrixColor[3][1], humi[3][i]*matrixColor[3][2]));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(barShape[2][i]*barColor[2][0], barShape[2][i]*barColor[2][1], barShape[2][i]*barColor[2][2]));
    }
    matrix.show();
    bar.show();
    delay(100);
    for(int i=0;i<64;i++) { 
      matrix.setPixelColor(i, matrix.Color(0,0,0));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(0,0,0));
    }
    matrix.show();
    bar.show();
    delay(100);
  }
}
void heartText(){
//  tft.fillScreen(ILI9340_BLACK);
//  long irValue = particleSensor.getIR();
  //unsigned long start = micros();     //micros()함수는 아두이노 보드에서 프로그램이 시작된 후 마이크초 단위로 카운트된 값 반환(70분 후 오버플로우 되어 다시 0부터 카운트)
  beatsPerMinute = beatAvg;
  tft.setCursor(65, 40);
  tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
  tft.println("2018-10-24");

  tft.setCursor(65, 70);
  tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(3);
  tft.println("Heart Rate");
  tft.println("");


   tft.setCursor(125, 180);
   tft.setTextSize(3);
   tft.println("BPM");

   //return micros() - start;
}
