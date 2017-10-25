#include<stdio.h>
#include<stdint.h>
#include<stdlib.h>

#define LENGTH 131071


#define FLASH_LENGTH_HEADER     6 // uint32 timestamp + uint16 timestamp
#define FLASH_LENGTH_REMOTE     (8 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_TELEMETRY  (14 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_SENSOR     (12 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_TEST       (0 + FLASH_LENGTH_HEADER)


FILE *ptr_myfile;
FILE *fp_remote;
FILE *fp_telemetry;
FILE *fp_sensor;

// 4 bits max
typedef enum flashPacket_t
{
    flashRemote = 0x0,
    flashTelemetry = 0x1,
    flashSensor = 0x2,
    flashSensorIntermediate = 0x3,
    flashControl = 0x4,
    flashTest = 0x8,
    flashUnkown = 0xF
} flashPacket_t;


int parse_remote(uint8_t* data){
    uint16_t lift;
    int16_t roll;
    int16_t pitch;
    int16_t yaw;
    lift  = data[0] << 8 | data[1];
    roll  = data[2] << 8 | data[3];
    pitch = data[4] << 8 | data[5];
    yaw   = data[6] << 8 | data[7];
    fprintf(fp_remote,"%d\t%d\t%d\t%d\n",lift,roll,pitch,yaw);
    return 8-1;
}

int parse_telemetry(uint8_t* data){
    uint16_t batteryVoltage;
    int16_t phi;
    int16_t theta;
    int16_t psi;
    int16_t pressure;
    int16_t r;
    uint16_t loopTime;

    batteryVoltage  = data[0] << 8 | data[1];
    phi             = data[2] << 8 | data[3];
    theta           = data[4] << 8 | data[5];
    psi             = data[6] << 8 | data[7];
    pressure        = data[8] << 8 | data[9];
    r               = data[10] << 8 | data[11];
    loopTime        = data[12] << 8 | data[13];

    fprintf(fp_telemetry,"%d\t%d\t%d\t%d\t%d\t%d\t%d\t\n",batteryVoltage, phi, theta, psi, pressure, r, loopTime);
    return 14-1;
}
int parse_sensor(uint8_t* data){
    int16_t sp;
    int16_t sq;
    int16_t sr;
    int16_t sax;
    int16_t say;
    int16_t saz;

    sp  = data[0] << 8 | data[1];
    sq  = data[2] << 8 | data[3];
    sr  = data[4] << 8 | data[5];
    sax = data[6] << 8 | data[7];
    say = data[8] << 8 | data[9];
    saz = data[10] << 8 | data[11];

    fprintf(fp_sensor,"%d\t%d\t%d\t%d\t%d\t%d\n",sp, sq, sr, sax, say, saz);
    // return 12;
    return 8-1;
}

int main()
{
    uint8_t *data;
    uint32_t *timestamp;
    int message_byte = 0;
    uint16_t temp16;
    uint32_t temp32;

    uint32_t time;
    flashPacket_t type;
    uint8_t mode;

    data = malloc(LENGTH);
    if (data == NULL) {
        printf("ERROR: Malloc()");
        return 1;
    }

    ptr_myfile=fopen("data.bin","rb");
    if (!ptr_myfile){
        printf("Unable to open file!");
        return 1;
    }
    fp_remote = fopen("remote.csv","w");
    if (!fp_remote){
        printf("Unable to open file! remote.csv");
        return 1;
    }
    fp_telemetry = fopen("telemetry.csv","w");
    if (!fp_telemetry){
        printf("Unable to open file! telemetry.csv");
        return 1;
    }
    fp_sensor = fopen("sensors.csv","w");
    if (!fp_sensor){
        printf("Unable to open file! sensors.csv");
        return 1;
    }

    fread(data,sizeof(char), LENGTH,ptr_myfile);
    fclose(ptr_myfile);

    for (int counter=0; counter < 100; counter++){
        printf("%d\n", data[counter]);
    }

    for (int counter=0; counter < LENGTH; counter++) {
    // for (int counter=0; counter < 100; counter++){
        printf("counter: %d\n",counter);
        // Look-ahead for message type:
        if (message_byte < 4) {
            if (message_byte==0){
                time = 0;
            }
            // printf("\t\t\ttime:%d\t%d\t%d\t%d\n",data[counter], message_byte , data[counter] << (message_byte * 8 ), time);
            time |= data[counter] << (message_byte * 8 );
            // printf("%d\t%d\t%d\n",message_byte,time,data[counter]);
        } else if (message_byte == 4) {
            type = (flashPacket_t) data[counter];
            printf("type: %d\n",type);
        } else if (message_byte == 5) {
            mode = data[counter];
        } else {
            switch(type) {
            case flashRemote :
            case flashTelemetry :
            case flashSensor :
                fprintf(fp_remote,"%d\t%d\t",time,mode);
                counter += parse_remote(&data[counter]);
                break;
            // case flashTelemetry :
            //     fprintf(fp_telemetry,"%d\t%d\t",time,mode);
            //     counter += parse_telemetry(&data[counter]);
            //     break;
            // case flashSensor :
            //     fprintf(fp_sensor,"%d\t%d\t",time,mode);
            //     counter += parse_sensor(&data[counter]);
            //     break;
            default :
                printf("unkown message type\n");
                return 1;
            }
            message_byte = -1;
        }
        message_byte++;
    }

    fclose(fp_remote);
    fclose(fp_telemetry);
    fclose(fp_sensor);

    return 0;
}
