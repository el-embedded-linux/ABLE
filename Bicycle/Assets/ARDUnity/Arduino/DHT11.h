/*
  DHT11.h - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

#ifndef DHT11_h
#define DHT11_h

#include "ArdunityController.h"


class DHT11 : public ArdunityController
{
public:
	DHT11(int id, int pin);

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
	unsigned long _preTime;
	UINT8 _humidity;
	UINT8 _temperature;

	int read();
};

#endif

