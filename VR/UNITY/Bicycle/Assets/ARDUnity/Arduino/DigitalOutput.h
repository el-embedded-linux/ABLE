/*
  DigitalOutput.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/
#ifndef DigitalOutput_h
#define DigitalOutput_h

#include "ArdunityController.h"


class DigitalOutput : public ArdunityController
{
public:
	DigitalOutput(int id, int pin, int defaultValue, boolean resetOnStop);	

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
	int _defaultValue;
	boolean _resetOnStop;
	UINT8 _value;
};

#endif

