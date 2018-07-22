#include "Ardunity.h"
#include "DigitalInput.h"

DigitalInput dInput0(0, 7, true);
DigitalInput dInput1(1, 8, true);
DigitalInput dInput2(2, 9, true);
DigitalInput dInput3(3, 10, true);

void setup()
{
  ArdunityApp.attachController((ArdunityController*)&dInput0);
  ArdunityApp.attachController((ArdunityController*)&dInput1);
  ArdunityApp.attachController((ArdunityController*)&dInput2);
  ArdunityApp.attachController((ArdunityController*)&dInput3);
  ArdunityApp.resolution(256, 1024);
  ArdunityApp.timeout(5000);
  ArdunityApp.begin(115200);
}

void loop()
{
  ArdunityApp.process();
}
