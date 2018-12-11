//
//  iosBluetoothLE.h
//  iosPlugin
//
//  Created by Jaehong on 2016. 1. 5.
//  Copyright © 2016 Ardunity. All rights reserved.
//

#ifndef iosBluetoothLE_h
#define iosBluetoothLE_h

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>

typedef void (* UnityCallback)(const char* uuid, const char* msg);

extern "C" {
    void bleInitialize(UnityCallback unityCallback);
    void bleDeinitialize();
    void bleStartScan(char *service);
    void bleStopScan();
    void bleConnect(char *device);
    void bleDisconnect(char *device);
    void bleDiscoverService(char *device, char *service);
    void bleDiscoverCharacteristic(char *device, char *service, char *characteristic);
    void bleWrite(char *device, char *service, char *characteristic, unsigned char *data, int length, BOOL withResponse);
    void bleRead(char *device, char *service, char *characteristic);
    void bleSubscribe(char *device, char *service, char *characteristic);
    void bleUnsubscribe(char *device, char *service, char *characteristic);
}

@interface BluetoothLE : NSObject <CBCentralManagerDelegate, CBPeripheralDelegate>
{
    CBCentralManager *_centralManager;
    
    NSMutableDictionary *_peripherals;
}

@property (atomic, strong) NSMutableDictionary *_peripherals;

- (void)initialize;
- (void)deInitialize;
- (void)startScan:(NSString *)serviceUUID;
- (void)stopScan;
- (void)connect:(NSString *)uuid;
- (void)disconnect:(NSString *)uuid;
- (void)discoverService:(NSString *)uuid service:(NSString *)serviceString;
- (void)discoverCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)writeCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString data:(NSData *)data withResponse:(BOOL)withResponse;
- (void)readCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)subscribeCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString;
- (void)unsubscribeCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString;

+ (NSString *) base64StringFromData:(NSData *)data length:(int)length;

@end

#endif /* iosBluetoothLE_h */
