/*
  DigitalInput.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef DigitalInput_h
#define DigitalInput_h

#include "ArdunityController.h"


class DigitalInput : public ArdunityController
{
public:
	DigitalInput(int id, int pin, boolean pullup);	

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
	boolean _pullup;
	UINT8 _value;
};

#endif

