#include <SoftwareSerial.h>

#define RX A1
#define TX A0

#define BTN 12
#define LED 6

SoftwareSerial nanoSerial(RX, TX);

int buttonState = 0;
void setup() {
  Serial.begin(9600);
  nanoSerial.begin(9600);
  
  pinMode(BTN, INPUT_PULLUP);
  pinMode(LED, OUTPUT);
}
void loop() {
  buttonState = digitalRead(BTN);
  
  if(buttonState == LOW) {
    //digitalWrite(LED, HIGH);
    //delay(500);
    //digitalWrite(LED, LOW);
    //delay(500);
    char send_data = '1';
    nanoSerial.write(send_data); 
  }
  
  if(nanoSerial.available()) {
    char read_data = nanoSerial.read();
    Serial.write(read_data);
    digitalWrite(LED, HIGH);
    delay(500);
    digitalWrite(LED, LOW);
    delay(500);
    nanoSerial.flush();
  }
}
