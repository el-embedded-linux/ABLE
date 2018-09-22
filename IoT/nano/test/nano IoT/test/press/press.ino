int SensorPin = A0;

void setup(){
  Serial.begin(9600);

}
void loop(){
  int SensorReading = analogRead(SensorPin); 
 
  int mfsr_r18 = map(SensorReading, 0, 1024, 0, 255);
  Serial.println(mfsr_r18);
  delay(100); 
}


