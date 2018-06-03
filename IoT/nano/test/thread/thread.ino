#include <Thread.h>
#include <ThreadController.h>

#include <Stepper.h>

ThreadController controll = ThreadController();

int ledPin = 13;

//My simple Thread
Thread myThread = Thread();
Thread myThread2 = Thread();

int m_nCount = 0;

int m_nTCount1 = 0;
int m_nTCount2 = 0;

Stepper stepper(200, 8, 9, 10, 11);


// callback for myThread
void niceCallback()
{
  ++m_nTCount1;
  Serial.print(" niceCallback : ");
  Serial.print(millis());
  Serial.print(" , ");
  Serial.println(m_nTCount1);
}

void testCallback()
{
  ++m_nTCount2;
  Serial.print(" testCallback : ");
  Serial.print(millis());
  Serial.print(" , ");
  Serial.println(m_nTCount2);

}

void setup()
{
  Serial.begin(9600);
  pinMode(ledPin, OUTPUT);
  myThread.onRun(niceCallback);
  myThread.setInterval(500);
  
  myThread2.onRun(testCallback);
  myThread2.setInterval(800);
  
  controll.add(&myThread);
  controll.add(&myThread2);
  
  stepper.setSpeed(30);
}

void loop()
{
  if(Serial.available())
  {
    int c = Serial.read();
    
    if(c == '1')
    {
      stepper.step(200);
    }
  }
  
  // checks if thread should run
  /*if(myThread.shouldRun())
    myThread.run();
    
  if(myThread2.shouldRun())
    myThread2.run();*/
  controll.run();

  // Other code...
  ++m_nCount;
/*
  Serial.print("Main Thread :  ");
  Serial.println(m_nCount);
  
  delay(1000);*/
  
}
