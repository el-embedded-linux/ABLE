//RingLED
#include<Adafruit_NeoPixel.h>
#ifdef __AVR__
  #include <avr/power.h>
#endif
#define PIN 6
//지자기
//GND는 GND, VDD는 3V, SDA핀 A4, SCL핀 A5, 
#include "I2Cdev.h"
#include "MPU9250.h"
int16_t   mx, my, mz;
MPU9250 accelgyro;
I2Cdev   I2C_M;

uint8_t buffer_m[6];

float heading;
float tiltheading;
int head;

int i;
int temp_com;
int pump;
int test;
int randNumber;

volatile float mx_sample[3];
volatile float my_sample[3];
volatile float mz_sample[3];

static float mx_centre = 0;
static float my_centre = 0;
static float mz_centre = 0;

volatile int mx_max = 0;
volatile int my_max = 0;
volatile int mz_max = 0;

volatile int mx_min = 0;
volatile int my_min = 0;
volatile int mz_min = 0;

Adafruit_NeoPixel strip = Adafruit_NeoPixel(64, PIN, NEO_GRB + NEO_KHZ800);

// IMPORTANT: To reduce NeoPixel burnout risk, add 1000 uF capacitor across
// pixel power leads, add 300 - 500 Ohm resistor on first pixel's data input
// and minimize distance between Arduino and first pixel.  Avoid connecting
// on a live circuit...if you must, connect GND first.
float Mxyz[3];
float Axyz[3];

void setup() {
  // This is for Trinket 5V 16MHz, you can remove these three lines if you are not using a Trinket
  #if defined (__AVR_ATtiny85__)
    if (F_CPU == 16000000) clock_prescale_set(clock_div_1);
  #endif
  // End of trinket special code
  
  strip.begin();
  strip.show(); // Initialize all pixels to 'off'
 
  // join I2C bus (I2Cdev library doesn't do this automatically)
    Wire.begin();
    // it's really up to you depending on your project)
    Serial.begin(9600);

    // initialize device
    Serial.println("Initializing I2C devices...");
    accelgyro.initialize();

    // verify connection
    Serial.println("Testing device connections...");
    Serial.println(accelgyro.testConnection() ? "MPU9250 connection successful" : "MPU9250 connection failed");



    delay(1000);
    Serial.println("     ");

    //  Mxyz_init_calibrated ();
    
    //randomSeed의 매개변수로 0번 채널(A0번 핀)에서 읽은 아날로그 값을 전달
    randomSeed(analogRead(0));
}
//ToDoList
//1. 값이 갑자기 너무 커지는 경우에는 반응하지 않는다.
//2. 값이 너무 조금 움이는 경우에는 반응하지 않는다.
void loop() {
randNumber = random(360);//randNumber 변수에 0 ~ 299 값 중 랜덤한 값을 저장
Serial.println(randNumber);//randNumber 변수의 값을 씨리얼모니터에 한 줄로 출력
  while(true){
    getCompassDate_calibrated();
    getTiltHeading();
    Serial.println("The clockwise angle between the magnetic north and X-Axis:");
    Serial.println(" ");
    head = (int)tiltheading;
    if(head<0){//값이 0~360 범위안에 없는 경우
        head=head+360;
    }else if(head>360){
      head= head-360;
    }
    Serial.println(head);
    switch(head){
      case 0 ... 14: i=0; break;
      case 15 ... 29: i=1; break;
      case 30 ... 44: i=2; break;
      case 45 ... 59: i=3; break;
      case 60 ... 74: i=4; break;
      case 75 ... 89: i=5; break;
      case 90 ... 104: i=6; break;
      case 105 ... 119: i=7; break;
      case 120 ... 134: i=8; break;
      case 135 ... 149: i=9; break;
      case 150 ... 164: i=10; break;
      case 165 ... 179: i=11; break;
      case 180 ... 194: i=12; break;
      case 195 ... 209: i=13; break;
      case 210 ... 224: i=14; break;
      case 225 ... 239: i=15; break;
      case 240 ... 254: i=16; break;
      case 255 ... 269: i=17; break;
      case 270 ... 284: i=18; break;
      case 285 ... 299: i=19; break;
      case 300 ... 314: i=20; break;
      case 315 ... 329: i=21; break;
      case 330 ... 344: i=22; break;
      case 345 ... 359: i=23; break;
      default: break;
    }

        colorWipe(temp_com,strip.Color(0, 0, 0), 0); // 전값을 초기화시키고
        colorWipe(i,strip.Color(0, 100, 0), 5); // 초록색으로 해당 픽셀 on
        
    delay(1000);
    temp_com=i;
    //test가 1000이 될 때마다 한 번씩 렌덤하게 각도를 받아온다.
    test+=1;
    Serial.println(test);
    if(test==1000)break;
  }
}

void colorWipe(int i, uint32_t c, uint8_t wait) {
  
    strip.setPixelColor(i, c);
    strip.show();
    delay(wait);
  
}

void getHeading(void)
{
    heading = 180 * atan2(Mxyz[1], Mxyz[0])/PI;
    if (heading < 0)heading += 360;
}

void getTiltHeading(void)
{
    float pitch = asin(-Axyz[0]);
    float roll = asin(Axyz[1] / cos(pitch));

    float xh = Mxyz[0] * cos(pitch) + Mxyz[2] * sin(pitch);
    float yh = Mxyz[0] * sin(roll) * sin(pitch) + Mxyz[1] * cos(roll) - Mxyz[2] * sin(roll) * cos(pitch);
    float zh = -Mxyz[0] * cos(roll) * sin(pitch) + Mxyz[1] * sin(roll) + Mxyz[2] * cos(roll) * cos(pitch);
    tiltheading = 180 * atan2(yh, xh)/PI;
    if (yh < 0) tiltheading += 360;
}

void getCompass_Data(void)
{
    I2C_M.writeByte(MPU9150_RA_MAG_ADDRESS, 0x0A, 0x01); //enable the magnetometer
    delay(10);
    I2C_M.readBytes(MPU9150_RA_MAG_ADDRESS, MPU9150_RA_MAG_XOUT_L, 6, buffer_m);

    mx = ((int16_t)(buffer_m[1]) << 8) | buffer_m[0] ;
    my = ((int16_t)(buffer_m[3]) << 8) | buffer_m[2] ;
    mz = ((int16_t)(buffer_m[5]) << 8) | buffer_m[4] ;

    Mxyz[0] = (double) mx * 1200 / 4096;
    Mxyz[1] = (double) my * 1200 / 4096;
    Mxyz[2] = (double) mz * 1200 / 4096;
}

void getCompassDate_calibrated ()
{
    getCompass_Data();
    Mxyz[0] = Mxyz[0] - mx_centre;
    Mxyz[1] = Mxyz[1] - my_centre;
    Mxyz[2] = Mxyz[2] - mz_centre;
}
