extern "C"
{
#include "driver.h"
}
#include "flash_wrapper.h"

uint32_t flashAddress = 0;

bool flash_write_test()
{
    //3lashPacket_t type = flashTest;
    uint8_t data[FLASH_LENGTH_TEST];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = flashAddress;
    *(reinterpret_cast<uint8_t*>(&data[4])) = 0xAA;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_TEST);
    if (status) {
        flashAddress += FLASH_LENGTH_TEST;
    }
    return status;
}

bool flash_write_remote(uint32_t timestamp, uint8_t mode, uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw)
{
    flashPacket_t type = flashRemote;
    uint8_t data[FLASH_LENGTH_REMOTE];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = timestamp;
    *(reinterpret_cast<uint8_t*>(&data[4])) = static_cast<uint8_t>(((type & 0xFFu) << 4) | (mode & 0xFFu));
    *(reinterpret_cast<uint16_t*>(&data[5])) = lift;
    *(reinterpret_cast<uint16_t*>(&data[7])) = roll;
    *(reinterpret_cast<uint16_t*>(&data[9])) = pitch;
    *(reinterpret_cast<uint16_t*>(&data[11])) = yaw;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_REMOTE);
    if (status) {
        flashAddress += FLASH_LENGTH_REMOTE;
    }
    return status;
}

bool flash_write_telemetry(uint32_t timestamp, uint8_t mode, uint16_t battery_voltage, int16_t phi, int16_t theta, int16_t p, int16_t q, int16_t r, uint16_t loop_time)
{
    flashPacket_t type = flashTelemetry;
    uint8_t data[FLASH_LENGTH_TELEMETRY];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = timestamp;
    *(reinterpret_cast<uint8_t*>(&data[4])) = static_cast<uint8_t>(((type & 0xFFu) << 4) | (mode & 0xFFu));
    *(reinterpret_cast<uint16_t*>(&data[5])) = battery_voltage;
    *(reinterpret_cast<uint16_t*>(&data[7])) = phi;
    *(reinterpret_cast<uint16_t*>(&data[9])) = theta;
    *(reinterpret_cast<uint16_t*>(&data[11])) = p;
    *(reinterpret_cast<uint16_t*>(&data[13])) = q;
    *(reinterpret_cast<uint16_t*>(&data[15])) = r;
    *(reinterpret_cast<uint16_t*>(&data[17])) = loop_time;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_TELEMETRY);
    if (status) {
        flashAddress += FLASH_LENGTH_TELEMETRY;
    }
    return status;
}

bool flash_write_sensor(uint32_t timestamp, uint8_t mode, int16_t sp, int16_t sq, int16_t sr, int16_t sax, int16_t say, int16_t saz)
{
    flashPacket_t type = flashSensor;
    uint8_t data[FLASH_LENGTH_SENSOR];
    bool status;
    *(reinterpret_cast<uint32_t*>(&data[0])) = timestamp;
    //*(reinterpret_cast<uint8_t*>(&data[4])) = static_cast<uint8_t>(((type & 0xFFu) << 4) | (mode & 0xFFu));
    data[4] = (((type & 0xFFu) << 4) | (mode & 0xFFu));
    *(reinterpret_cast<uint16_t*>(&data[5])) = sp;
    *(reinterpret_cast<uint16_t*>(&data[7])) = sq;
    *(reinterpret_cast<uint16_t*>(&data[9])) = sr;
    *(reinterpret_cast<uint16_t*>(&data[11])) = sax;
    *(reinterpret_cast<uint16_t*>(&data[13])) = say;
    *(reinterpret_cast<uint16_t*>(&data[15])) = saz;

    status = flash_write_bytes(flashAddress, data, FLASH_LENGTH_SENSOR);
    if (status) {
        flashAddress += FLASH_LENGTH_SENSOR;
    }
    return status;
}
