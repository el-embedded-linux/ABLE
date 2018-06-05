/*
  AnalogInput.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef AnalogInput_h
#define AnalogInput_h

#include "ArdunityController.h"


class AnalogInput : public ArdunityController
{
public:
	AnalogInput(int id, int pin);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
    int _pin;
	FLOAT32 _value;
};

#endif

