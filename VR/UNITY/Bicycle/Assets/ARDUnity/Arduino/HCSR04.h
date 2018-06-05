/*
  HCSR04.h - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

#ifndef HCSR04_h
#define HCSR04_h

#include "ArdunityController.h"


class HCSR04 : public ArdunityController
{
public:
	HCSR04(int id, int trig, int echo);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
    int _trig;
    int _echo;
    UINT16 _distance;
};

#endif

