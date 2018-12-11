/*
  AnalogOutput.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/
#ifndef AnalogOutput_h
#define AnalogOutput_h

#include "ArdunityController.h"


class AnalogOutput : public ArdunityController
{
public:
	AnalogOutput(int id, int pin, float defaultValue, boolean resetOnStop);	

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
	float _defaultValue;
	boolean _resetOnStop;
	FLOAT32 _value;
};

#endif

