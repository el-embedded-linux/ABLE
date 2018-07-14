/*
  ESP8266.h - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

#ifndef ESP8266_h
#define ESP8266_h

#include <inttypes.h>
#include <Stream.h>

#define BUFFER_SIZE 64


class ESP8266Class : public Stream
{
public:
	ESP8266Class();
    
    void begin(Stream *s, int port);
    virtual int available(void);
    virtual size_t write(uint8_t data);
    virtual int read(void);
    virtual void flush(void);
    virtual int peek(void);
    
protected:
	
private:
    Stream* _serial;
    uint8_t _txBuf[BUFFER_SIZE];
    int _txBufIndex;
    int _rxLength;
    int _availableLength;
    int _rxID;
    int _state;
    int _id;
    int _length;
    
    void clearRX(void);
    bool waitRX();
    bool waitRX(char ch);
};

extern ESP8266Class ESP8266;

#endif

