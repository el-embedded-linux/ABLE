#ifdef RaspberryPi
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <stdint.h>
#include <unistd.h>
#include <sys/socket.h>
#include <bluetooth/bluetooth.h>
#include <bluetooth/sdp.h>
#include <bluetooth/sdp_lib.h>
#include <bluetooth/rfcomm.h>
#include <pthread.h>
#include <wiringPi.h>
#include <wiringSerial.h>

#define CHATDATA 100

bdaddr_t bdaddr_any = {0, 0, 0, 0, 0, 0};
bdaddr_t bdaddr_local = {0, 0, 0, 0xff, 0xff, 0xff};

char device[] = "/dev/ttyUSB0";

char serialstr[CHATDATA];

int fd;

unsigned long baud = 9600;
unsigned long tm=0;

void *loope(void);
void setup(void);

pthread_t	loopth;
void setup(){
	printf("%s \n", "Raspberry Startup!");
	fflush(stdout);
	//get filedescriptor
  	if ((fd = serialOpen (device, baud)) < 0){
    		fprintf (stderr, "Unable to open serial device: %s\n", strerror (errno)) ;
    		exit(1); //error
  	}
 
  	//setup GPIO in wiringPi mode
  	if (wiringPiSetup () == -1){
    		fprintf (stdout, "Unable to start wiringPi: %s\n", strerror (errno)) ;
    		exit(1); //error
  	}
}

int _str2uuid( const char *uuid_str, uuid_t *uuid ) {
    /* This is from the pybluez stack */

    uint32_t uuid_int[4];
    char *endptr;
    if( strlen( uuid_str ) == 36 ) {
        char buf[9] = { 0 };
        if( uuid_str[8] != '-' && uuid_str[13] != '-' &&
        uuid_str[18] != '-' && uuid_str[23] != '-' ) {
        return 0;
    }
    // first 8-bytes
    strncpy(buf, uuid_str, 8);
    uuid_int[0] = htonl( strtoul( buf, &endptr, 16 ) );
    if( endptr != buf + 8 ) return 0;
        // second 8-bytes
        strncpy(buf, uuid_str+9, 4);
        strncpy(buf+4, uuid_str+14, 4);
        uuid_int[1] = htonl( strtoul( buf, &endptr, 16 ) );
        if( endptr != buf + 8 ) return 0;

        // third 8-bytes
        strncpy(buf, uuid_str+19, 4);
        strncpy(buf+4, uuid_str+24, 4);
        uuid_int[2] = htonl( strtoul( buf, &endptr, 16 ) );

        if( endptr != buf + 8 ) return 0;
        // fourth 8-bytes
        strncpy(buf, uuid_str+28, 8);
        uuid_int[3] = htonl( strtoul( buf, &endptr, 16 ) );
        if( endptr != buf + 8 ) return 0;
        if( uuid != NULL ) sdp_uuid128_create( uuid, uuid_int );
    } else if ( strlen( uuid_str ) == 8 ) {
        // 32-bit reserved UUID
        uint32_t i = strtoul( uuid_str, &endptr, 16 );
        if( endptr != uuid_str + 8 ) return 0;
        if( uuid != NULL ) sdp_uuid32_create( uuid, i );
    } else if( strlen( uuid_str ) == 4 ) {
        // 16-bit reserved UUID
        int i = strtol( uuid_str, &endptr, 16 );
        if( endptr != uuid_str + 4 ) return 0;
        if( uuid != NULL ) sdp_uuid16_create( uuid, i );
    } else {
        return 0;
    }

    return 1;
}

sdp_session_t *register_service(uint8_t rfcomm_channel) {
    /* A 128-bit number used to identify this service. The words are ordered from most to least
    * significant, but within each word, the octets are ordered from least to most significant.
    * For example, the UUID represneted by this array is 00001101-0000-1000-8000-00805F9B34FB. (The
    * hyphenation is a convention specified by the Service Discovery Protocol of the Bluetooth Core
    * Specification, but is not particularly important for this program.)
    *
    * This UUID is the Bluetooth Base UUID and is commonly used for simple Bluetooth applications.
    * Regardless of the UUID used, it must match the one that the Armatus Android app is searching
    * for.
    */
    const char *service_name = "Armatus Bluetooth server";
    const char *svc_dsc = "A HERMIT server that interfaces with the Armatus Android app";
    const char *service_prov = "Armatus";

    uuid_t root_uuid, l2cap_uuid, rfcomm_uuid, svc_uuid,
           svc_class_uuid;
    sdp_list_t *l2cap_list = 0,
                *rfcomm_list = 0,
                 *root_list = 0,
                  *proto_list = 0,
                   *access_proto_list = 0,
                    *svc_class_list = 0,
                     *profile_list = 0;

    sdp_data_t *channel = 0;
    sdp_profile_desc_t profile;
    sdp_record_t record = { 0 };
    sdp_session_t *session = 0;

    // set the general service ID
    //sdp_uuid128_create(&svc_uuid, &svc_uuid_int);
    _str2uuid("00001101-0000-1000-8000-00805F9B34FB",&svc_uuid);
    sdp_set_service_id(&record, svc_uuid);

    char str[256] = "";
    sdp_uuid2strn(&svc_uuid, str, 256);
    printf("Registering UUID %s\n", str);

    // set the service class
    sdp_uuid16_create(&svc_class_uuid, SERIAL_PORT_SVCLASS_ID);
    svc_class_list = sdp_list_append(0, &svc_class_uuid);
    sdp_set_service_classes(&record, svc_class_list);

    // set the Bluetooth profile information
    sdp_uuid16_create(&profile.uuid, SERIAL_PORT_PROFILE_ID);
    profile.version = 0x0100;
    profile_list = sdp_list_append(0, &profile);
    sdp_set_profile_descs(&record, profile_list);

    // make the service record publicly browsable
    sdp_uuid16_create(&root_uuid, PUBLIC_BROWSE_GROUP);
    root_list = sdp_list_append(0, &root_uuid);
    sdp_set_browse_groups(&record, root_list);

    // set l2cap information
    sdp_uuid16_create(&l2cap_uuid, L2CAP_UUID);
    l2cap_list = sdp_list_append(0, &l2cap_uuid);
    proto_list = sdp_list_append(0, l2cap_list);

    // register the RFCOMM channel for RFCOMM sockets
    sdp_uuid16_create(&rfcomm_uuid, RFCOMM_UUID);
    channel = sdp_data_alloc(SDP_UINT8, &rfcomm_channel);
    rfcomm_list = sdp_list_append(0, &rfcomm_uuid);
    sdp_list_append(rfcomm_list, channel);
    sdp_list_append(proto_list, rfcomm_list);

    access_proto_list = sdp_list_append(0, proto_list);
    sdp_set_access_protos(&record, access_proto_list);

    // set the name, provider, and description
    sdp_set_info_attr(&record, service_name, service_prov, svc_dsc);
    // connect to the local SDP server, register the service record,
    // and disconnect
    session = sdp_connect(&bdaddr_any, &bdaddr_local, SDP_RETRY_IF_BUSY);
    sdp_record_register(session, &record, 0);

    // cleanu
    sdp_data_free(channel);
    sdp_list_free(l2cap_list, 0);
    sdp_list_free(rfcomm_list, 0);
    sdp_list_free(root_list, 0);
    sdp_list_free(access_proto_list, 0);
    sdp_list_free(svc_class_list, 0);
    sdp_list_free(profile_list, 0);

    return session;
}

int init_server() {
    int port = 3, result, sock, client, bytes_read, bytes_sent;
    struct sockaddr_rc loc_addr = { 0 }, rem_addr = { 0 };
    char buffer[1024] = { 0 };

    socklen_t opt = sizeof(rem_addr);

    // local bluetooth adapter
    loc_addr.rc_family = AF_BLUETOOTH;
    loc_addr.rc_bdaddr = bdaddr_any;
    loc_addr.rc_channel = (uint8_t) port;

    // register service

    sdp_session_t *session = register_service(port);

    // allocate socket
    sock = socket(AF_BLUETOOTH, SOCK_STREAM, BTPROTO_RFCOMM);
    printf("socket() returned %d\n", sock);

    // bind socket to port 3 of the first available
    result = bind(sock, (struct sockaddr *)&loc_addr, sizeof(loc_addr));
    printf("bind() on channel %d returned %d\n", port, result);

    // put socket into listening mode
    result = listen(sock, 1);
    printf("listen() returned %d\n", result);

    //sdpRegisterL2cap(port);

    // accept one connection
    printf("calling accept()\n");
    client = accept(sock, (struct sockaddr *)&rem_addr, &opt);
    printf("accept() returned %d\n", client);

    ba2str(&rem_addr.rc_bdaddr, buffer);
    fprintf(stderr, "accepted connection from %s\n", buffer);
    memset(buffer, 0, sizeof(buffer));

    return client;
}

char input[1024] = { 0 };
char *read_server(int client) {
    // read data from the client
    int bytes_read;
    bytes_read = read(client, input, sizeof(input));
    if (bytes_read > 0) {
        printf("received [%s]\n", input);
        return input;
    } else {
        return NULL;
    }
}

/*void write_server(void *arg) {
    // send data to the client
    char messageArr[1024] = { 0 };
    int client = (int) arg;
    int bytes_sent;
    int i;
    char* message = "1000";

    strcpy(messageArr, message);
	printf("ddd\n");
	for(i = 0;i<30;i++){
   		 bytes_sent = write(client, messageArr, strlen(messageArr));
		memset(messageArr,0,sizeof(messageArr));
	}
    //bytes_sent = write(client, i, sizeof(i));
	printf("aaa\n");
    if (bytes_sent > 0) {
	printf("bbb\n");
        printf("sent [%s] %d\n", messageArr, bytes_sent);
    }
    
}

int main()
{
    pthread_t writeT;
    int client = init_server();
    char recv_message[] = "1000";
    while(1)
    {
        //char *recv_message = read_server(client);
        /*if ( recv_message == NULL ){
            printf("client disconnected\n");
            break;
        }
        // pthread_create(&writeT,NULL,write_server,(void*)client);
	//sleep(1000);
	write(client,recv_message, strlen(recv_message));
	printf("1000\n");
	sleep(100);
    }
}*/
void write_server(int client, char *message) {
    // send data to the client
    char messageArr[1024] = { 0 };
    int bytes_sent;
    strcpy(messageArr, message);
    bytes_sent = write(client, messageArr, strlen(messageArr));
    if (bytes_sent > 0) {
        printf("sent [%s] %d\n", messageArr, bytes_sent);
    }
}

int main()
{
    int client = init_server();
    char recv_message[CHATDATA];
    int number = 100;
    setup();

    pthread_create(&loopth, NULL, loope,NULL);
    while(1)
    {
       /* char *recv_message = read_server(client);
        if ( recv_message == NULL ){
            printf("client disconnected\n");
            bre ak;
        }*/
	sprintf(recv_message," %d",number);
        write_server(client, recv_message);
	memset(recv_message,0,sizeof(recv_message));
	number+=10;
	sleep(1);
    }
}
void *loope(){
	while(1){
		if(millis() -tm >=3000){
			serialPuts(fd, "Pong!\n");
			serialPutchar(fd,65);
			tm = millis();
		}
		if(serialDataAvail (fd)){
			char newChar = serialGetchar(fd);
			printf("%c",newChar);
			fflush(stdout);
		}
	}
}
#endif
