/*
  ESP8266.cpp - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "ESP8266.h"


//#define DEBUG_ESP8266

//******************************************************************************
//* Constructors
//******************************************************************************

ESP8266Class::ESP8266Class()
{
    _txBufIndex = 0;
    _rxLength = 0;
    _rxID = 0;
    _state = 0;
    _id = 0;
    _length = 0;
    _availableLength = 0;
}


//******************************************************************************
//* Methods
//******************************************************************************
void ESP8266Class::begin(Stream *s, int port)
{
    _serial = s;
    
#ifdef DEBUG_ESP8266
    Serial.begin(9600);
    Serial.println("\nDebug ESP8266");
#endif
    
    setTimeout(300);
    
    // wait boot
    delay(1000);
    while(_serial->available() > 0)
        _serial->read();
    
    // enable echo
    _serial->println("ATE1");
    waitRX();
    delay(100);
    
    // set multiple connection
    clearRX();
    _serial->println("AT+CIPMUX=1");
    waitRX();
    delay(100);

    // start server
    clearRX();
    _serial->print("AT+CIPSERVER=1,");
    _serial->println(port);
    waitRX();
    delay(100);

    // set time 10 seconds
    clearRX();
    _serial->println("AT+CIPSTO=10");
    waitRX();
    delay(100);

    // disable echo
    clearRX();
    _serial->println("ATE0");
    waitRX();

    // wait setting
    delay(1000);
    clearRX();
}

int ESP8266Class::available(void)
{
    if(_rxLength > 0)
    {
        _availableLength = _serial->available();
        if(_availableLength > _rxLength)
            _availableLength = _rxLength;
    }
    else
    {
        while(_serial->available() > 0)
        {
            int ch = _serial->read();
#ifdef DEBUG_ESP8266
            Serial.write(ch);
#endif
            switch(_state)
            {
                case 0:
                    if(ch == '+')
                        _state++;
                    break;
                    
                case 1:
                    if(ch == 'I')
                        _state++;
                    else
                        _state = 0;
                    break;
                    
                case 2:
                    if(ch == 'P')
                        _state++;
                    else
                        _state = 0;
                    break;
                    
                case 3:
                    if(ch == 'D')
                        _state++;
                    else
                        _state = 0;
                    break;
                    
                case 4:
                    if(ch == ',')
                    {
                        _id = _serial->parseInt();
#ifdef DEBUG_ESP8266
                        Serial.print(_id);
#endif
                        _state++;
                    }
                    else
                        _state = 0;
                    break;
                    
                case 5:
                    if(ch == ',')
                    {
                        _length = _serial->parseInt();
#ifdef DEBUG_ESP8266
                        Serial.print(_length);
#endif
                        _state++;
                    }
                    else
                        _state = 0;
                    break;
                    
                case 6:
                    if(ch == ':')
                    {
                        _rxID = _id;
                        _rxLength = _length;
                        _availableLength = _serial->available();
                        if(_availableLength > _rxLength)
                            _availableLength = _rxLength;
                        
                        _state = 0;
                        return _availableLength;
                    }
                    else
                        _state = 0;
                    break;
                    
                default:
                    _state = 0;
                    break;
            }
        }
    }
    
    return _availableLength;
}

int ESP8266Class::peek(void)
{
    if(_availableLength > 0)
    {
        return _serial->peek();
    }
    
    return -1;
}

int ESP8266Class::read(void)
{
    if(_availableLength > 0)
    {
        _availableLength--;
        _rxLength--;
        
        int ch = _serial->read();
#ifdef DEBUG_ESP8266
        Serial.print(ch, HEX);
        Serial.print(" ");
#endif
        if(_rxLength == 0)
        {
            waitRX('K');
        }
        return ch;
    }
    
    return -1;
}

size_t ESP8266Class::write(uint8_t data)
{
    if(_txBufIndex >= BUFFER_SIZE)
        flush();
    
    _txBuf[_txBufIndex++] = data;
    
    return 1;
}

void ESP8266Class::flush(void)
{
    if(_txBufIndex == 0)
        return;
    
    clearRX();
    
    _serial->print("AT+CIPSEND=");
    _serial->print(_rxID);
    _serial->print(",");
    _serial->println(_txBufIndex);
#ifdef DEBUG_ESP8266
    Serial.print("AT+CIPSEND=");
    Serial.print(_rxID);
    Serial.print(",");
    Serial.println(_txBufIndex);
#endif
    
    unsigned long t = millis();
    if(waitRX('>'))
    {
        for(int i=0; i<_txBufIndex; i++)
        {
            _serial->write(_txBuf[i]);
#ifdef DEBUG_ESP8266
            Serial.print(_txBuf[i], HEX);
            Serial.print(" ");
#endif
        }
        
        waitRX('K');
    }
    
    _txBufIndex = 0;
}

void ESP8266Class::clearRX()
{
    // empty rx buffer
    while(_serial->available() > 0)
    {
#ifdef DEBUG_ESP8266
        Serial.write(_serial->read());
#else
        _serial->read();
#endif
    }
    
    _rxLength = 0;
    _availableLength = 0;
}

bool ESP8266Class::waitRX()
{
    _startMillis = millis();
    while(millis() - _startMillis < _timeout)
    {
        if(_serial->available() > 0)
            return true;
    }
    
    return false;
}

bool ESP8266Class::waitRX(char ch)
{
    _startMillis = millis();
    while(millis() - _startMillis < _timeout)
    {
        if(_serial->available() > 0)
        {
            int data = _serial->read();
#ifdef DEBUG_ESP8266
            Serial.write(data);
#endif
            if(data == ch)
                return true;
        }
    }
    
    return false;
}

ESP8266Class ESP8266;

