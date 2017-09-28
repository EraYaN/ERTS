#include <cstdint>

//#include "packet_datastructures.h"

#define FLASH_LENGTH_HEADER     5 // uint32 timestamp + uint8 timestamp
#define FLASH_LENGTH_REMOTE     (8 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_TELEMETRY  (12 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_SENSOR     (14 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_TEST       (0 + FLASH_LENGTH_HEADER)

// 4 bits max
enum flashPacket_t
{
    flashRemote = 0x0,
    flashTelemetry = 0x1,
    flashSensor = 0x2,
    flashSensorIntermediate = 0x3,
    flashControl = 0x4,
    flashTest = 0x8,
    flashUnkown = 0xF
};

bool flash_write_test();
bool flash_write_remote(uint32_t, uint8_t, uint16_t, int16_t, int16_t, int16_t);
bool flash_write_telemetry(uint32_t, uint8_t, uint16_t, int16_t, int16_t, int16_t, int16_t, int16_t, uint16_t);
bool flash_write_sensor(uint32_t, uint8_t, int16_t, int16_t, int16_t, int16_t, int16_t, int16_t);
