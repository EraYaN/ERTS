extern "C"
{
#include "driver.h"
}

#include <packet_datastructures.h>
#include "flash_wrapper.h"

uint32_t flashAddress = 0;

bool flash_write_test()
{
    //FlashPacket_t type = flashTest;
    uint8_t data[FLASH_LENGTH_TEST];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = flashAddress;
    *(reinterpret_cast<uint16_t*>(&data[4])) = 0xAAAA;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_TEST);
    if (status) {
        flashAddress += FLASH_LENGTH_TEST;
    }
    return status;
}

bool flash_write_remote(uint32_t timestamp, uint8_t mode, uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw)
{
    if (flashAddress + FLASH_LENGTH_REMOTE >= FLASH_BYTES_TOTAL) {
        flashAddress = 0;
        return false;
    }
    flashPacket_t type = flashRemote;
    uint8_t data[FLASH_LENGTH_REMOTE];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = timestamp;
    data[4] = type;
    data[5] = mode;
    *(reinterpret_cast<uint16_t*>(&data[6])) = lift;
    *(reinterpret_cast<int16_t*>(&data[8])) = roll;
    *(reinterpret_cast<int16_t*>(&data[10])) = pitch;
    *(reinterpret_cast<int16_t*>(&data[12])) = yaw;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_REMOTE);
    if (status) {
        flashAddress += FLASH_LENGTH_REMOTE;
    }
    return status;
}

bool flash_write_telemetry(uint32_t timestamp, uint8_t mode, uint16_t battery_voltage, int16_t roll, int16_t pitch, int16_t yaw, int16_t pressure, int16_t func_state, uint16_t loop_time)
{
    if (flashAddress + FLASH_LENGTH_TELEMETRY >= FLASH_BYTES_TOTAL) {
        flashAddress = 0;
        return false;
    }
    flashPacket_t type = flashTelemetry;
    uint8_t data[FLASH_LENGTH_TELEMETRY];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = timestamp;
    data[4] = type;
    data[5] = mode;
    
    *(reinterpret_cast<uint16_t*>(&data[6])) = battery_voltage;
    *(reinterpret_cast<int16_t*>(&data[8])) = roll;
    *(reinterpret_cast<int16_t*>(&data[10])) = pitch;
    *(reinterpret_cast<int16_t*>(&data[12])) = yaw;
    *(reinterpret_cast<int16_t*>(&data[14])) = pressure;
    *(reinterpret_cast<int16_t*>(&data[16])) = func_state;
    *(reinterpret_cast<uint16_t*>(&data[18])) = loop_time;
    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_TELEMETRY);
    if (status) {
        flashAddress += FLASH_LENGTH_TELEMETRY;
    }
    return status;
}

bool flash_write_sensor(uint32_t timestamp, uint8_t mode, int16_t sp, int16_t sq, int16_t sr, int16_t sax, int16_t say, int16_t saz)
{
    if (flashAddress + FLASH_LENGTH_SENSOR >= FLASH_BYTES_TOTAL) {
        flashAddress = 0;
        return false;
    }
    flashPacket_t type = flashSensor;
    uint8_t data[FLASH_LENGTH_SENSOR];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = timestamp;
    data[4] = type;
    data[5] = mode;
    *(reinterpret_cast<int16_t*>(&data[6])) = sp;
    *(reinterpret_cast<int16_t*>(&data[8])) = sq;
    *(reinterpret_cast<int16_t*>(&data[10])) = sr;
    *(reinterpret_cast<int16_t*>(&data[12])) = sax;
    *(reinterpret_cast<int16_t*>(&data[14])) = say;
    *(reinterpret_cast<int16_t*>(&data[16])) = saz;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_SENSOR);
    if (status) {
        flashAddress += FLASH_LENGTH_SENSOR;
    }
    return status;
}