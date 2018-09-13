/*
 Compile with:
 sudo gcc raspberry_serial1.c -o raspberry1 -lwiringPi -DRaspberryPi
 sudo ./raspberry1
*/

#ifdef RaspberryPi 

#include <stdio.h>
#include <stdint.h> 
#include <stdlib.h> 
#include <string.h> 
#include <pthread.h>
#include <errno.h> 

#include <wiringPi.h>
#include <wiringSerial.h>



// Find Serial device on Raspberry with ~ls /dev/tty*
// ARDUINO_UNO "/dev/ttyACM0"
// FTDI_PROGRAMMER "/dev/ttyUSB0"
// HARDWARE_UART "/dev/ttyAMA0"

char device[]= "/dev/ttyACM0";
char device_2[]= "/dev/ttyACM1";

int fd, fd2;
unsigned long baud = 9600;
char result[255];
int resultNum=0;  


int main(void);
void *readSerial(void *arg);
void *writeSerial(void *arg);
void setup(void);


void setup(){
	   
	printf("%s \n", "Raspberry Startup!");
	fflush(stdout);
	       
	//get filedescriptor
	if ((fd = serialOpen (device, baud)) < 0){
		fprintf (stderr, "Unable to open serial device: %s\n", strerror (errno)) ;
		exit(1); //error
	}


	
	if ((fd2 = serialOpen (device_2, baud)) < 0){
		fprintf (stderr, "Unable to open serial device: %s\n", strerror (errno)) ;
		exit(1); //error
	}
	

		 
	//setup GPIO in wiringPi mode
	if (wiringPiSetup () == -1){
		fprintf (stdout, "Unable to start wiringPi: %s\n", strerror (errno)) ;
		exit(1); //error
	}
		   
}


void *readSerial(void *arg){
	while(1){
		if(serialDataAvail (fd)){

			char newChar = serialGetchar (fd);

			if(newChar == '\n')	{
				printf("result: %s\n", result);
				resultNum=0;
				delay(500);
		     	}
	     		else{
				result[resultNum++] = newChar;
	     		}
		}
	}

	
}


void *writeSerial(void *arg){
	while(1){
		serialPuts (fd2, "serial test\n");
		delay(1000);
	}
}


int main(void){
	setup();
	
	pthread_t tid[2];

	int i;

	pthread_create(&tid[0], NULL, readSerial, (void*)1);
	pthread_create(&tid[1], NULL, writeSerial, (void*)2);

	pthread_exit(NULL);
}


#endif