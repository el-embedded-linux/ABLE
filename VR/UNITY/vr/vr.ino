const int mov1=4;
const int mov2=5;
const int sel=6;

void setup() {
  Serial.begin(9600);

  pinMode(mov1,INPUT);
  pinMode(mov2,INPUT);
  pinMode(sel,INPUT);

  digitalWrite(mov1,HIGH);
  digitalWrite(mov2,HIGH);
  digitalWrite(sel,HIGH);
}

void loop() {
  if(digitalRead(mov1) == LOW){
    Serial.write(1);
    Serial.flush();
    delay(200);
  }
  if(digitalRead(mov2) == LOW){
    Serial.write(2);
    Serial.flush();
    delay(200);
  }
  if(digitalRead(sel) == LOW){
    Serial.write(3);
    Serial.flush();
    delay(200);
  }
}
