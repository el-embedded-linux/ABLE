//지자기센서
//심박동측정센서
//링LED

void setup(){
  Serial.begin(9600);

}
void loop(){
  press();
}

void press(){
  int SensorReading = analogRead(SensorPin); 
 
  int mfsr_r18 = map(SensorReading, 0, 1024, 0, 255);
  Serial.println(mfsr_r18);
  delay(100); 
}

