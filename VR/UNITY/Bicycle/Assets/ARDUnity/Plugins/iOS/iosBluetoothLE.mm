//
//  iosBluetoothLE.m
//  iosPlugin
//
//  Created by Jaehong on 2016. 1. 5.
//  Copyright © 2016 Ardunity. All rights reserved.
//

#import "iosBluetoothLE.h"


extern "C" {
    
    BluetoothLE *_BluetoothLE = nil;
    UnityCallback _unityCallback = nil;
    
    void bleInitialize(UnityCallback unityCallback)
    {
        if (_BluetoothLE != nil)
            return;
        
        _unityCallback = unityCallback;
        _BluetoothLE = [BluetoothLE new];
        [_BluetoothLE initialize];
    }
    
    void bleDeinitialize()
    {
        if (_BluetoothLE == nil)
            return;
        
        [_BluetoothLE deInitialize];
     //   [_BluetoothLE release];  Must disable in iOS
        _BluetoothLE = nil;
        _unityCallback = nil;
    }
    
    void bleStartScan(char *service)
    {
        if (_BluetoothLE == nil)
            return;
                
        NSString *sUUID = [NSString stringWithFormat:@"%s", service];
        [_BluetoothLE startScan:sUUID];
    }
    
    void bleStopScan()
    {
        if (_BluetoothLE == nil)
            return;
        
        [_BluetoothLE stopScan];
        
        if(_unityCallback != nil)
            _unityCallback(nil, "StopScan");
    }
    
    void bleConnect(char *device)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        [_BluetoothLE connect:sDeivce];
    }
    
    void bleDisconnect(char *device)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        [_BluetoothLE disconnect:sDeivce];
    }
    
    void bleDiscoverService(char *device, char *service)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        NSString *sService = [NSString stringWithFormat:@"%s", service];
        [_BluetoothLE discoverService:sDeivce service:sService];
    }
    
    void bleDiscoverCharacteristic(char *device, char *service, char *characteristic)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        NSString *sService = [NSString stringWithFormat:@"%s", service];
        NSString *sCharacteristic = [NSString stringWithFormat:@"%s", characteristic];
        [_BluetoothLE discoverCharacteristic:sDeivce service:sService characteristic:sCharacteristic];
    }

    
    void bleWrite(char *device, char *service, char *characteristic, unsigned char *data, int length, BOOL withResponse)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        NSString *sService = [NSString stringWithFormat:@"%s", service];
        NSString *sCharacteristic = [NSString stringWithFormat:@"%s", characteristic];
        NSData *nsData = [NSData dataWithBytes:data length:length];
        [_BluetoothLE writeCharacteristic:sDeivce service:sService characteristic:sCharacteristic data:nsData withResponse:withResponse];
    }
    
    void bleRead(char *device, char *service, char *characteristic)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        NSString *sService = [NSString stringWithFormat:@"%s", service];
        NSString *sCharacteristic = [NSString stringWithFormat:@"%s", characteristic];
        [_BluetoothLE readCharacteristic:sDeivce service:sService characteristic:sCharacteristic];

    }
    
    void bleSubscribe(char *device, char *service, char *characteristic)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        NSString *sService = [NSString stringWithFormat:@"%s", service];
        NSString *sCharacteristic = [NSString stringWithFormat:@"%s", characteristic];
        [_BluetoothLE subscribeCharacteristic:sDeivce service:sService characteristic:sCharacteristic];

    }
    
    void bleUnsubscribe(char *device, char *service, char *characteristic)
    {
        if (_BluetoothLE == nil)
            return;
        
        NSString *sDeivce = [NSString stringWithFormat:@"%s", device];
        NSString *sService = [NSString stringWithFormat:@"%s", service];
        NSString *sCharacteristic = [NSString stringWithFormat:@"%s", characteristic];
        [_BluetoothLE unsubscribeCharacteristic:sDeivce service:sService characteristic:sCharacteristic];
    }
}

@implementation BluetoothLE

@synthesize _peripherals;


- (void)initialize
{
    _centralManager = [[CBCentralManager alloc] initWithDelegate:self queue:nil];
    _peripherals = [[NSMutableDictionary alloc] init];
    
    if(_unityCallback != nil)
        _unityCallback(nil, "Initialized");
}

- (void)deInitialize
{
    if (_centralManager != nil)
        [_centralManager stopScan];
    
    if(_unityCallback != nil)
        _unityCallback(nil, "Deinitialized");
}

- (void)startScan:(NSString *)serviceUUID
{
    if (_centralManager == nil)
        return;
    
    NSArray *services = @[[CBUUID UUIDWithString:serviceUUID]]; // For iOS
    [_centralManager scanForPeripheralsWithServices:services options:nil]; // For iOS
    
  //  [_centralManager scanForPeripheralsWithServices:nil options:@{CBCentralManagerScanOptionSolicitedServiceUUIDsKey:@[[CBUUID UUIDWithString:serviceUUID]]}]; // For OSX
    
    if(_unityCallback != nil)
        _unityCallback(nil, "StartScan");
}

- (void)stopScan
{
    if (_centralManager == nil)
        return;
    
    [_centralManager stopScan];
    
    if(_unityCallback != nil)
        _unityCallback(nil, "StopScan");
}

- (void)connect:(NSString *)uuid
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral != nil)
        [_centralManager connectPeripheral:peripheral options:nil];
}

- (void)disconnect:(NSString *)uuid
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;
        
    [_centralManager cancelPeripheralConnection:peripheral];
}

- (void)discoverService:(NSString *)uuid service:(NSString *)serviceString
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;
    
    CBUUID *cbService = [CBUUID UUIDWithString:serviceString];
    NSArray *services = [NSArray  arrayWithObject:cbService];
    [peripheral discoverServices:services];
}


- (void)discoverCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;
    
    CBUUID *cbService = [CBUUID UUIDWithString:serviceString];
    for(CBService *service in peripheral.services)
    {
        if([service.UUID isEqual:cbService])
        {
            CBUUID *cbCharacteristic = [CBUUID UUIDWithString:characteristicString];
            NSArray *characteristics = [NSArray  arrayWithObject:cbCharacteristic];
            [peripheral discoverCharacteristics:characteristics forService:service];
            return;
        }
    }
}

- (void)writeCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString data:(NSData *)data withResponse:(BOOL)withResponse
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;
    
    CBUUID *cbService = [CBUUID UUIDWithString:serviceString];
    CBUUID *cbCharacteristic = [CBUUID UUIDWithString:characteristicString];
    for(CBService *service in peripheral.services)
    {
        if([service.UUID isEqual:cbService])
        {
            for(CBCharacteristic *charac in service.characteristics)
            {
                if([charac.UUID isEqual:cbCharacteristic])
                {
                    CBCharacteristicWriteType type = CBCharacteristicWriteWithoutResponse;
                    if (withResponse)
                        type = CBCharacteristicWriteWithResponse;
                    
                    [peripheral writeValue:data forCharacteristic:charac type:type];
                    
                    if(type == CBCharacteristicWriteWithoutResponse)
                    {
                        NSString *message = [NSString stringWithFormat:@"Write~%@", [BluetoothLE CBUUIDToString:charac.UUID]];
                        if(_unityCallback != nil)
                            _unityCallback([uuid UTF8String], [message UTF8String]);
                    }
                    return;
                }
            }
        }
    }
}

- (void)readCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;
    
    CBUUID *cbService = [CBUUID UUIDWithString:serviceString];
    CBUUID *cbCharacteristic = [CBUUID UUIDWithString:characteristicString];
    for(CBService *service in peripheral.services)
    {
        if([service.UUID isEqual:cbService])
        {
            for(CBCharacteristic *charac in service.characteristics)
            {
                if([charac.UUID isEqual:cbCharacteristic])
                {
                    [peripheral readValueForCharacteristic:charac];
                    return;
                }
            }
        }
    }
}

- (void)subscribeCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;

    CBUUID *cbService = [CBUUID UUIDWithString:serviceString];
    CBUUID *cbCharacteristic = [CBUUID UUIDWithString:characteristicString];
    for(CBService *service in peripheral.services)
    {
        if([service.UUID isEqual:cbService])
        {
            for(CBCharacteristic *charac in service.characteristics)
            {
                if([charac.UUID isEqual:cbCharacteristic])
                {
                    [peripheral setNotifyValue:YES forCharacteristic:charac];
                    return;
                }
            }
        }
    }
}

- (void)unsubscribeCharacteristic:(NSString *)uuid service:(NSString *)serviceString characteristic:(NSString *)characteristicString
{
    if (_peripherals == nil)
        return;
    
    CBPeripheral *peripheral = [_peripherals objectForKey:uuid];
    if (peripheral == nil)
        return;
    
    CBUUID *cbService = [CBUUID UUIDWithString:serviceString];
    CBUUID *cbCharacteristic = [CBUUID UUIDWithString:characteristicString];
    for(CBService *service in peripheral.services)
    {
        if([service.UUID isEqual:cbService])
        {
            for(CBCharacteristic *charac in service.characteristics)
            {
                if([charac.UUID isEqual:cbCharacteristic])
                {
                    [peripheral setNotifyValue:NO forCharacteristic:charac];
                    return;
                }
            }
        }
    }
}

// central delegate implementation
- (void)centralManagerDidUpdateState:(CBCentralManager *)central
{
    if (central.state == CBCentralManagerStatePoweredOff)
    {
        if(_unityCallback != nil)
            _unityCallback(nil, "PoweredOff");
    }
    else if (central.state == CBCentralManagerStatePoweredOn)
    {
        if(_unityCallback != nil)
            _unityCallback(nil, "PoweredOn");
    }
    else if (central.state == CBCentralManagerStateUnknown)
    {
        if(_unityCallback != nil)
            _unityCallback(nil, "StateUnknown");
    }
    else if(central.state == CBCentralManagerStateUnsupported)
    {
        if(_unityCallback != nil)
            _unityCallback(nil, "NotSupported");
    }
    else if(central.state == CBCentralManagerStateUnauthorized)
    {
        if(_unityCallback != nil)
            _unityCallback(nil, "Unauthorized");
    }
}

- (void)centralManager:(CBCentralManager *)central didRetrievePeripherals:(NSArray *)peripherals
{
    
}

- (void)centralManager:(CBCentralManager *)central didRetrieveConnectedPeripherals:(NSArray *)peripherals
{
    
}

- (void)centralManager:(CBCentralManager *)central didFailToConnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"ConnectFailed~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
}

- (void)centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)peripheral advertisementData:(NSDictionary *)advertisementData RSSI:(NSNumber *)RSSI
{
   // NSString *name = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
    NSString *name = [NSString stringWithFormat:@"%@", peripheral.name];
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    [_peripherals setObject:peripheral forKey:uuid];
    
    NSString *message = [NSString stringWithFormat:@"DiscoveredDevice~%@~%@", name, uuid];
    if(_unityCallback != nil)
        _unityCallback(nil, [message UTF8String]);
}

- (void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    NSString *message = [NSString stringWithFormat:@"Disconnected"];
    
    if(_unityCallback != nil)
        _unityCallback([uuid UTF8String], [message UTF8String]);
}

- (void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    CBPeripheral *foundPeripheral = [self findPeripheralInList:peripheral];
    if (foundPeripheral != nil)
    {
        peripheral.delegate = self;
        
        NSString *message = [NSString stringWithFormat:@"Connected"];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
}

- (CBPeripheral *) findPeripheralInList:(CBPeripheral*)peripheral
{
    CBPeripheral *foundPeripheral = nil;
    
    NSEnumerator *enumerator = [_peripherals keyEnumerator];
    id key;
    while ((key = [enumerator nextObject]))
    {
        CBPeripheral *tempPeripheral = [_peripherals objectForKey:key];
        if ([tempPeripheral isEqual:peripheral])
        {
            foundPeripheral = tempPeripheral;
            break;
        }
    }
    
    return foundPeripheral;
}


// peripheral delegate implementation
- (void)peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"ErrorDiscoverService~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString stringWithFormat:@"DiscoveredService"];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"ErrorDiscoverCharacteristic~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
    else
    {
        NSString *charUUIDs = [NSString new];
        for (CBCharacteristic *characteristic in service.characteristics)
        {
            if(characteristic.UUID != nil)
                charUUIDs = [NSString stringWithFormat:@"%@:%@", charUUIDs, characteristic.UUID.UUIDString];
        }
        
        NSString *message = [NSString stringWithFormat:@"DiscoveredCharacteristic~%@", charUUIDs];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"ErrorRead~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
    else
    {
        if (characteristic.value != nil)
        {
            NSString *message = [NSString stringWithFormat:@"Read~%@~%@", [BluetoothLE CBUUIDToString:characteristic.UUID], [BluetoothLE base64StringFromData:characteristic.value length:(int)characteristic.value.length]];
            if(_unityCallback != nil)
                _unityCallback([uuid UTF8String], [message UTF8String]);
        }
    }
}

- (void)peripheral:(CBPeripheral *)peripheral didWriteValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"ErrorWrite~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString stringWithFormat:@"Write~%@", [BluetoothLE CBUUIDToString:characteristic.UUID]];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }

}

- (void)peripheral:(CBPeripheral *)peripheral didUpdateNotificationStateForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    NSString *uuid = [NSString stringWithFormat:@"%@", peripheral.identifier.UUIDString];
    
    if (error)
    {
        NSString *message = [NSString stringWithFormat:@"ErrorSubscribeCharacteristic~%@", error.description];
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
    else
    {
        NSString *message = [NSString new];
        if(characteristic.isNotifying == true)
            message = [NSString stringWithFormat:@"SubscribedCharacteristic~%@", characteristic.UUID.UUIDString];
        else
            message = [NSString stringWithFormat:@"UnSubscribedCharacteristic~%@", characteristic.UUID.UUIDString];
        
        if(_unityCallback != nil)
            _unityCallback([uuid UTF8String], [message UTF8String]);
    }
}

+ (NSString*) CBUUIDToString: (CBUUID *)cbuuid;
{
    NSData *data = cbuuid.data;
    
    NSUInteger bytesToConvert = [data length];
    const unsigned char *uuidBytes = (const unsigned  char *)[data bytes];
    NSMutableString *outputString = [NSMutableString stringWithCapacity:16];
    
    for (NSUInteger currentByteIndex = 0; currentByteIndex < bytesToConvert; currentByteIndex++)
    {
        switch (currentByteIndex)
        {
            case 3:
            case 5:
            case 7:
            case 9:[outputString appendFormat:@"%02X-", uuidBytes[currentByteIndex]]; break;
            default:[outputString appendFormat:@"%02X", uuidBytes[currentByteIndex]];
        }
        
    }
    
    return outputString;
}

static char base64EncodingTable[64] =
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
    'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
    'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
    'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
};

+ (NSString *) base64StringFromData: (NSData *)data length: (int)length
{
    unsigned long ixtext, lentext;
    long ctremaining;
    unsigned char input[3], output[4];
    short i, charsonline = 0, ctcopy;
    const unsigned char *raw;
    NSMutableString *result;
    
    lentext = [data length];
    if (lentext < 1)
        return @"";
    result = [NSMutableString stringWithCapacity: lentext];
    raw = (const unsigned char *)[data bytes];
    ixtext = 0;
    
    while (true) {
        ctremaining = lentext - ixtext;
        if (ctremaining <= 0)
            break;
        for (i = 0; i < 3; i++) {
            unsigned long ix = ixtext + i;
            if (ix < lentext)
                input[i] = raw[ix];
            else
                input[i] = 0;
        }
        output[0] = (input[0] & 0xFC) >> 2;
        output[1] = ((input[0] & 0x03) << 4) | ((input[1] & 0xF0) >> 4);
        output[2] = ((input[1] & 0x0F) << 2) | ((input[2] & 0xC0) >> 6);
        output[3] = input[2] & 0x3F;
        ctcopy = 4;
        switch (ctremaining) {
            case 1:
                ctcopy = 2;
                break;
            case 2:
                ctcopy = 3;
                break;
        }
        
        for (i = 0; i < ctcopy; i++)
            [result appendString: [NSString stringWithFormat: @"%c", base64EncodingTable[output[i]]]];
        
        for (i = ctcopy; i < 4; i++)
            [result appendString: @"="];
        
        ixtext += 3;
        charsonline += 4;
        
        if ((length > 0) && (charsonline >= length))
            charsonline = 0;
    }
    return result;
}

#pragma mark Internal

@end
