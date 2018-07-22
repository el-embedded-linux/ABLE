#include <SoftwareSerial.h>


String income="";

void setup() {
   Serial.begin(9600); 
}

void loop() {
   while(Serial.available()) {
      income += (char)Serial.read();
   } 
   
   if(income != 0) {
      Serial.print(income);
     income = ""; 
   }
   delay(10);
}
