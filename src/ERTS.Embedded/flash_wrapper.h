#include <cstdint>

// #include <packet_datastructures.h>

#define FLASH_LENGTH_HEADER     6 // uint32 timestamp + uint16 timestamp
#define FLASH_LENGTH_REMOTE     (8 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_TELEMETRY  (14 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_SENSOR     (12 + FLASH_LENGTH_HEADER)
#define FLASH_LENGTH_TEST       (0 + FLASH_LENGTH_HEADER)

// #define FLASH_BYTES_PER_UART_PACKET (DATA_SIZE - 2)
#define FLASH_BYTES_PER_UART_PACKET (14 - 2)
#define FLASH_BYTES_TOTAL 0x1FFFF //128kB flash memory
#define FLASH_PACKETS ((FLASH_BYTES_TOTAL/FLASH_BYTES_PER_UART_PACKET)+1)
#define FLASH_LAST_PACKET_SIZE (FLASH_BYTES_TOTAL % FLASH_BYTES_PER_UART_PACKET)

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
//Author: Casper
bool flash_write_test();
//Author: Casper
bool flash_write_remote(uint32_t, uint8_t, uint16_t, int16_t, int16_t, int16_t);
//Author: Casper
bool flash_write_telemetry(uint32_t, uint8_t, uint16_t, int16_t, int16_t, int16_t, int16_t, int16_t, uint16_t);
//Author: Casper
bool flash_write_sensor(uint32_t, uint8_t, int16_t, int16_t, int16_t, int16_t, int16_t, int16_t);

