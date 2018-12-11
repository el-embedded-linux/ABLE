#include "Ardunity.h"
#include "DigitalInput.h"

DigitalInput dInput6(6, 6, true);
DigitalInput dInput5(5, 5, true);
DigitalInput dInput4(4, 4, true);
DigitalInput dInput7(7, 15, true);

void setup()
{
  ArdunityApp.attachController((ArdunityController*)&dInput6);
  ArdunityApp.attachController((ArdunityController*)&dInput5);
  ArdunityApp.attachController((ArdunityController*)&dInput4);
  ArdunityApp.attachController((ArdunityController*)&dInput7);
  ArdunityApp.resolution(256, 1024);
  ArdunityApp.timeout(5000);
  ArdunityApp.begin(115200);
}

void loop()
{
  ArdunityApp.process();
}
