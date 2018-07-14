/*
  GenericMotor.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/
#ifndef GenericMotor_h
#define GenericMotor_h

#include "ArdunityController.h"

#define OnePWM_OneDir   0
#define TwoPWM          1
#define OnePWM_TwoDir   2


class GenericMotor : public ArdunityController
{
public:
	GenericMotor(int id, int pin1, int pin2, int pin3, int type);	

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
    int _pin1;
	int _pin2;
    int _pin3;
    int _type;
	FLOAT32 _value;
};

#endif

