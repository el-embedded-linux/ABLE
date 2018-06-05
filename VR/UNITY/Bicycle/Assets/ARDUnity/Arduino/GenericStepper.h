/*
  GenericStepper.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef GenericStepper_h
#define GenericStepper_h

#include "ArdunityController.h"

#define WAVE_DRIVE		0
#define FULL_STEP_DRIVE	1
#define HALF_STEP_DRIVE	2

class GenericStepper : public ArdunityController
{
public:
	GenericStepper(int id, int step, int drive, int pin1, int pin2, int pin3, int pin4);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
	int _drive;
    int _pin1;
	int _pin2;
	int _pin3;
	int _pin4;

	float _stepAngle;
	float _halfStepAngle;
	float _errorAngle;
	boolean _direction;
	unsigned long _delayTime;
	unsigned long _preTime;
	int _phase;

	UINT8 _mode;
	FLOAT32 _rpm;
	FLOAT32 _angle;
	UINT8 _newMode;
	FLOAT32 _newRPM;
	FLOAT32 _newAngle;

	void SetStep();
	void SetStep(boolean direction);
};

#endif

