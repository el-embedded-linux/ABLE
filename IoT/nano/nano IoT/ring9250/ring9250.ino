//RingLED
#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
  #include <avr/power.h>
#endif
#define PIN 6
//지자기
//SDA핀 A4, SCL핀 A5, 나머지 VDD 5V, GND는 GND
#include "I2Cdev.h"
#include "MPU9250.h"
int16_t   mx, my, mz;
MPU9250 accelgyro;
I2Cdev   I2C_M;
uint8_t buffer_m[6];

int i;
int temp;

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
float heading;
float Mxyz[3];

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
}
//ToDoList
//1. 값이 갑자기 너무 커지는 경우에는 반응하지 않는다.
//2. 값이 너무 조금 움이는 경우에는 반응하지 않는다.
//3. if문을 너무 많이 사용해서 switch문이나 다른 방식으로 바꾼다.

void loop() {
  getCompassDate_calibrated();
  getHeading();
  Serial.println("The clockwise angle between the magnetic north and X-Axis:");
  Serial.print(heading);
  Serial.println(" ");
    if(heading>=0&&heading<15){
      i=0;
    }else if(heading>=15&&heading<30){
      i=23;
    }else if(heading>=30&&heading<45){
      i=22;
    }else if(heading>=45&&heading<60){
      i=21;
    }else if(heading>=60&&heading<75){
      i=20;
    }else if(heading>=75&&heading<90){
      i=19;
    }else if(heading>=90&&heading<105){
      i=18;
    }else if(heading>=105&&heading<120){
      i=17;
    }else if(heading>=120&&heading<135){
      i=16;
    }else if(heading>=135&&heading<150){
      i=15;
    }else if(heading>=150&&heading<165){
      i=14;
    }else if(heading>=165&&heading<180){
      i=13;
    }else if(heading>=180&&heading<195){
      i=12;
    }else if(heading>=195&&heading<210){
      i=11;
    }else if(heading>=210&&heading<225){
      i=10;
    }else if(heading>=225&&heading<240){
      i= 9;
    }else if(heading>=240&&heading<255){
      i= 8;
    }else if(heading>=255&&heading<270){
      i= 7;
    }else if(heading>=270&&heading<285){
      i= 6;
    }else if(heading>=285&&heading<300){
      i= 5;
    }else if(heading>=300&&heading<315){
      i= 4;
    }else if(heading>=315&&heading<330){
      i= 3;
    }else if(heading>=330&&heading<345){
      i= 2;
    }else if(heading>=345&&heading<360){
      i= 1;
    }else{
      i= 0;
    }
     colorWipe(temp,strip.Color(0, 0, 0), 0); // Green
     colorWipe(i,strip.Color(0, 5, 0), 5); // Green
  delay(100);
    temp=i;
}

void colorWipe(int i, uint32_t c, uint8_t wait) {
  
    strip.setPixelColor(i, c);
    strip.show();
    delay(wait);
  
}
void getHeading(void)
{
    heading = 180 * atan2(Mxyz[1], Mxyz[0]); //PI;
    if (heading < 0) heading += 360;
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
