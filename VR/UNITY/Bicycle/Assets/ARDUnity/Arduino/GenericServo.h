/*
  GenericServo.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef GenericServo_h
#define GenericServo_h

#include <Servo.h>
#include "ArdunityController.h"


class GenericServo : public ArdunityController
{
public:
	GenericServo(int id, int pin, boolean smooth);	

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
	boolean _smooth;
	boolean _first;
	int _startAngle;
	int _endAngle;
	int _curAngle;
	unsigned long _endTime;
	unsigned long _startTime;

	Servo _servo;
	UINT8 _angle;
};

#endif

