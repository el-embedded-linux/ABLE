/*
  Ardunity.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/


//******************************************************************************
//* Includes
//******************************************************************************

#include "Ardunity.h"
#include "HardwareSerial.h"

extern "C" {
#include <string.h>
#include <stdlib.h>
}

//#define DEBUG_ARDUNITYAPP

//******************************************************************************
//* Constructors
//******************************************************************************

ArdunityAppClass::ArdunityAppClass()
{	
	firstController = 0;
    connected = false;
    timeoutMillis = 5000; // default timeout
}

//******************************************************************************
//* Public Methods
//******************************************************************************

void ArdunityAppClass::begin()
{
	commSocket = 0;
    bypassSocket = 0;
	readyReceived = false;
    bypassProcessUpdate = 0;
	processUpdate = 0;
    connected = false;
	Reset();
    
#ifdef DEBUG_ARDUNITYAPP
    Serial.begin(9600);
    Serial.println("\nDebug ArdunityApp");
#endif
}

void ArdunityAppClass::begin(long speed)
{
	begin();
	Serial.begin(speed);
	commSocket = (Stream*)&Serial;
	
	commSocket->write(CMD_RESET);
    commSocket->flush();
}

void ArdunityAppClass::begin(Stream *s)
{
	begin();
	commSocket = s;

	commSocket->write(CMD_RESET);
    commSocket->flush();
}

void ArdunityAppClass::begin(Stream *s, Stream *s1)
{
    begin();
    commSocket = s;
    bypassSocket = s1;
    
    commSocket->write(CMD_RESET);
    commSocket->flush();
}

void ArdunityAppClass::resolution(int pwm, int adc)
{
#if defined(__SAM3X8E__) || defined(_VARIANT_ARDUINO_ZERO_) // only DUE or ZERO
	maxPWM = pwm - 1;
	int bit = 0;
	int value = maxPWM;
	while(value > 0)
	{
		value = (int)(value >> 1);
		bit++;
	}
    analogWriteResolution(bit);
    
    maxADC = adc - 1;
	bit = 0;
	value = maxADC;
	while(value > 0)
	{
		value = (int)(value >> 1);
		bit++;
	}
    analogReadResolution(bit);    
#else
	maxPWM = 255;
	maxADC = 1023;
#endif	
}

void ArdunityAppClass::timeout(unsigned long millisec)
{
	timeoutMillis = millisec;
}

void ArdunityAppClass::process(void)
{
	process(commSocket);
}

void ArdunityAppClass::process(Stream *s)
{
	ArdunityController* controller;
	Stream *backup = commSocket;
	commSocket = s;

	if(commSocket != 0)
	{
		while(commSocket->available() > 0)
		{
			byte bit = 1;
			int inputData = commSocket->read(); // this is 'int' to handle -1 when no data

			if(inputData >= 0)
			{
				if(inputData & 0x80)
				{
					if(inputData == CMD_PING)
					{
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("RX:CMD_PING");
#endif
						commSocket->write(CMD_PING);
                        commSocket->flush();
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("TX:CMD_PING");
#endif
					}
					else if(inputData == CMD_START)
					{
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("RX:CMD_START");
#endif
                        connected = true;
                        preTime = millis();

						if(startCallback != 0)
							(*startCallback)();
                        
						controller = firstController;
						while(controller != 0)
						{
							controller->start();
							controller = controller->nextController;
						}
                        
                        if(bypassSocket != 0)
                        {
                            bypassSocket->write(CMD_START);
                            bypassSocket->flush();
                        }
                        
						commSocket->write(CMD_READY);
                        commSocket->flush();
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("TX:CMD_READY");
#endif
					}
					else if(inputData == CMD_EXIT)
					{
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("RX:CMD_EXIT");
#endif
                        connected = false;
                        
						controller = firstController;
						while(controller != 0)
						{
							controller->stop();
							controller = controller->nextController;
						}
                        
                        if(bypassSocket != 0)
                        {
                            bypassSocket->write(CMD_EXIT);
                            bypassSocket->flush();
                        }

						if(exitCallback != 0)
							(*exitCallback)();
					}
					else if(inputData == CMD_READY)
					{
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("RX:CMD_READY");
#endif
						readyReceived = true;
                        preTime = millis();
                        
                        if(bypassSocket != 0)
                        {
                            bypassSocket->write(CMD_READY);
                            bypassSocket->flush();
                        }
                    }
					else if(inputData == CMD_EXECUTE)
					{
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("RX:CMD_EXECUTE");
#endif
						if(processUpdate > 0)
						{
							controller = firstController;
							while(controller != 0)
							{
								controller->execute();
								controller = controller->nextController;
							}
                            
							processUpdate = 0;
						}
                        
                        if(bypassSocket != 0)
                            bypassSocket->write(CMD_EXECUTE);
					}
			
					if(inputData == CMD_UPDATE)
                    {
#ifdef DEBUG_ARDUNITYAPP
                        Serial.println("RX:CMD_UPDATE");
#endif
						processUpdate = 1;
                        
                        if(bypassSocket != 0)
                            bypassSocket->write(CMD_UPDATE);
                    }
					else
                    {
#ifdef DEBUG_ARDUNITYAPP
                        if(inputData >= CMD_UNKNOWN)
                        {
                            Serial.print("RX:Unknown (");
                            Serial.print(inputData);
                            Serial.println(")");
                        }
#endif
						Reset();
                    }
				}
				else if(processUpdate > 0)
				{
					if(processUpdate == 1)
					{
						ID = inputData;
						processUpdate = 2;
					}
					else if(processUpdate == 2)
					{
						numData = inputData;
						if(numData > MAX_ARGUMENT_BYTES)
							Reset();
						else
						{
							processUpdate = 3;
							currentNumData = 0;
						}
					}
					else if(processUpdate == 3)
					{
						if(currentNumData < numData)
							storedData[currentNumData++] = inputData;

						if(currentNumData >= numData)
						{
							// Decoding 7bit bytes
							numData = 0;
							for(int i=0; i<currentNumData; i++)
							{
								if(bit == 1)
								{
									storedData[numData] = storedData[i] << bit;
									bit++;
								}
								else if(bit == 8)
								{
									storedData[numData++] |= storedData[i];
									bit = 1;
								}
								else
								{
									storedData[numData++] |= storedData[i] >> (8 - bit);
									storedData[numData] = storedData[i] << bit;
									bit++;
								}
							}

							currentNumData = 0;
						
							controller = firstController;
							while(controller != 0)
							{
								if(controller->update(ID) == true)
									break;
								controller = controller->nextController;
							}
						
							processUpdate = 1;
						}
					}
                    
                    if(bypassSocket != 0)
                        bypassSocket->write(inputData);
				}
				else
					Reset();
			}
		}
        
        if(bypassSocket != 0)
        {
            while(bypassSocket->available() > 0)
            {
                int inputData = bypassSocket->read(); // this is 'int' to handle -1 when no data
                if(inputData >= 0)
                {
                    if(inputData & 0x80)
                    {
                        if(inputData == CMD_UPDATE)
                        {
                            if(readyReceived == true)
                            {
#ifdef DEBUG_ARDUNITYAPP
                                Serial.println("TX:CMD_UPDATE");
#endif
                                commSocket->write(CMD_UPDATE);
                                
                                controller = firstController;
                                while(controller != 0)
                                {
                                    controller->flush();
                                    controller = controller->nextController;
                                }
                            }
                            bypassProcessUpdate = 1;
                        }
                        else if(inputData == CMD_EXECUTE)
                        {
                            if(readyReceived == true)
                            {
#ifdef DEBUG_ARDUNITYAPP
                                Serial.println("TX:CMD_EXECUTE");
                                Serial.println("TX:CMD_READY");
#endif
                                commSocket->write(CMD_EXECUTE);
                                commSocket->write(CMD_READY);
                                commSocket->flush();
                                readyReceived = false;

                            }
                            bypassProcessUpdate = 0;
                        }
                        else
                            bypassProcessUpdate = 0;
                    }
                    else if(bypassProcessUpdate == 1)
                    {
                        if(readyReceived == true)
                            commSocket->write(inputData);
                    }
                    else
                        bypassProcessUpdate = 0;
                }
            }
        }
        else
        {
            if(readyReceived == true)
            {
#ifdef DEBUG_ARDUNITYAPP
                Serial.println("TX:CMD_UPDATE");
#endif
                commSocket->write(CMD_UPDATE);
                
                controller = firstController;
                while(controller != 0)
                {
                    controller->flush();
                    controller = controller->nextController;
                }
#ifdef DEBUG_ARDUNITYAPP
                Serial.println("TX:CMD_EXECUTE");
                Serial.println("TX:CMD_READY");
#endif
                commSocket->write(CMD_EXECUTE);
                commSocket->write(CMD_READY);
                commSocket->flush();
                readyReceived = false;
            }
        }
	}

	commSocket = backup;

	controller = firstController;
	while(controller != 0)
	{
		controller->process();				
		controller = controller->nextController;
	}
    
    if(connected)
    {
        unsigned long curTime = millis();
        if(curTime >= preTime) // if overflow then skip
        {
            if((curTime - preTime) > timeoutMillis) // check timeout
            {
                connected = false;
                
                controller = firstController;
                while(controller != 0)
                {
                    controller->stop();
                    controller = controller->nextController;
                }
            }
        }
        else
            preTime = curTime;
    }
}

void ArdunityAppClass::select(byte id) 
{
	commSocket->write(id & 0x7F);
	numArgument = 0;
}

void ArdunityAppClass::flush() 
{
	float a = numArgument / 7;
	float b = numArgument % 7;
	byte addedNum = (byte)a;
	if(b > 0)
		addedNum++;

	commSocket->write((numArgument + addedNum) & 0x7F);
	// Encoding 7bit bytes
	byte bit = 1;
	byte temp = 0;
	for(byte i=0; i<numArgument; i++)
	{
		commSocket->write((temp | (storedArgument[i] >> bit)) & 0x7F);
		if(bit == 7)
		{
			commSocket->write(storedArgument[i] & 0x7F);
			bit = 1;
			temp = 0;
		}
		else
		{
			temp = storedArgument[i] << (7 - bit);
			if(i == (numArgument - 1))
				commSocket->write(temp & 0x7F);
			bit++;
		}		
	}
}

void ArdunityAppClass::attachController(ArdunityController* controller)
{
	ArdunityController* c = firstController;
	while(true)
	{
		if(c == 0)
		{
			firstController = controller;
			break;
		}
		if(c->nextController == 0)
		{
			c->nextController = controller;
			break;
		}

		c = c->nextController;
	}
	
	controller->setup();
}

void ArdunityAppClass::detachController(ArdunityController* controller)
{
	ArdunityController* c = firstController;
	ArdunityController* c1 = 0;

	while(c != 0)
	{
		if(c == controller)
		{
			if(c1 == 0)
				firstController = controller->nextController;
			else
				c1->nextController = controller->nextController;
			break;
		}
		
		c1 = c;
		c = c->nextController;
	}
}

void ArdunityAppClass::attachCallback(byte command, callbackFunction newFunction)
{
	switch(command)
	{
    	case CMD_START:
			startCallback = newFunction;
			break;

    	case CMD_EXIT:
			exitCallback = newFunction;
			break;
  	}
}

void ArdunityAppClass::detachCallback(byte command)
{
	switch(command)
	{
    	case CMD_START:
			startCallback = 0;
			break;

    	case CMD_EXIT:
			exitCallback = 0;
			break;
  	}
}

boolean ArdunityAppClass::push(byte* value, int size)
{
	if((MAX_ARGUMENT_BYTES - numArgument) < size)
		return false;
		
	for(int i=0; i<size; i++)
		storedArgument[numArgument++] = value[i];
	
	return true;
}

boolean ArdunityAppClass::push(UINT8 value)
{
	return push((byte*)&value, 1);
}

boolean ArdunityAppClass::push(INT8 value)
{
	return push((byte*)&value, 1);
}

boolean ArdunityAppClass::push(UINT16 value)
{
	return push((byte*)&value, 2);
}

boolean ArdunityAppClass::push(INT16 value)
{
	return push((byte*)&value, 2);
}

boolean ArdunityAppClass::push(UINT32 value)
{
	return push((byte*)&value, 4);
}

boolean ArdunityAppClass::push(INT32 value)
{
	return push((byte*)&value, 4);
}

boolean ArdunityAppClass::push(FLOAT32 value)
{
	return push((byte*)&value, 4);
}

boolean ArdunityAppClass::push(STRING value)
{
	byte size = 0;
	while(size < 255)
	{
		if(value[size] == '\0')
			break;
		size++;
	}
	
	if((MAX_ARGUMENT_BYTES - numArgument) < (size + 1))
		return false;
	
	storedArgument[numArgument++] = size;	
	for(int i=0; i<(int)size; i++)
		storedArgument[numArgument++] = value[i];
	
	return true;
}

boolean ArdunityAppClass::pop(byte* value, int size)
{
	if((numData - currentNumData) < size)
		return false;
	
	for(int i=0; i<size; i++)
		value[i] = storedData[currentNumData++];
}

boolean ArdunityAppClass::pop(UINT8* value)
{
	return pop((byte*)value, 1);
}

boolean ArdunityAppClass::pop(INT8* value)
{
	return pop((byte*)value, 1);
}

boolean ArdunityAppClass::pop(UINT16* value)
{
	return pop((byte*)value, 2);
}

boolean ArdunityAppClass::pop(INT16* value)
{
	return pop((byte*)value, 2);
}

boolean ArdunityAppClass::pop(UINT32* value)
{
	return pop((byte*)value, 4);
}

boolean ArdunityAppClass::pop(INT32* value)
{
	return pop((byte*)value, 4);
}

boolean ArdunityAppClass::pop(FLOAT32* value)
{
	return pop((byte*)value, 4);
}

boolean ArdunityAppClass::pop(STRING value, int maxSize)
{
	if((numData - currentNumData) < 1)
		return false;
	
	byte size = storedData[currentNumData++];
	
	if((numData - currentNumData) < size)
		return false;
		
	for(int i=0; i<(int)size; i++)
	{
		if(i < maxSize)
			value[i] = (char)storedData[currentNumData];

		currentNumData++;
	}
	
	if(size > maxSize)
		size = maxSize - 1;
	
	value[size] = '\0';
	return true;
}


//******************************************************************************
//* Private Methods
//******************************************************************************

// resets the system state upon a SYSTEM_RESET message from the host software
void ArdunityAppClass::Reset(void)
{
	if(processUpdate > 0)
    {
		commSocket->write(CMD_READY);
        commSocket->flush();
    }
	
	processUpdate = 0;
	numData = 0;
	currentNumData = 0;
	numArgument = 0;
}


ArdunityAppClass ArdunityApp;


