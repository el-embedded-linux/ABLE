/*
  PulseOutput.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/
#ifndef PulseOutput_h
#define PulseOutput_h

#include "ArdunityController.h"


class PulseOutput : public ArdunityController
{
public:
	PulseOutput(int id, int pin, int defaultValue);	

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
	int _setValue;	
	UINT8 _loop;
	UINT16 _setTime;
	UINT16 _delayTime;

	int _value;
	boolean _run;
	boolean _loop2;
	unsigned long _setTime2;
	unsigned long _delayTime2;
	unsigned long _preTime;
};

#endif

