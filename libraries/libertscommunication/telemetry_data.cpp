#include <cstring>
#include "telemetry_data.h"

TelemetryData::TelemetryData(uint16_t battery_voltage, int16_t phi, int16_t theta,
                             int16_t p, int16_t q, int16_t r,
                             uint16_t loop_time, flightMode_t flight_mode) {
    _data = new telemetryData_t;
    _data->batteryVoltage = battery_voltage;
    _data->flightMode = flight_mode;
    _data->phi = phi;
    _data->theta = theta;
    _data->p = p;
    _data->q = q;
    _data->r = r;
    _data->loopTime = loop_time;
}

int TelemetryData::get_length() {
    return sizeof(telemetryData_t);
}

uint32_t TelemetryData::get_ack_number() {
    return 0;
}

bool TelemetryData::get_expects_acknowledgement() {
    return false;
}

bool TelemetryData::is_valid() {
    return _data->flightMode != None;
}

void TelemetryData::to_buffer(uint8_t *buffer) {
//    *(reinterpret_cast<uint16_t*>(&buffer[0])) = static_cast<uint16_t>(((_data->batteryVoltage & 0x0FFFu) << 4) | (_data->flightMode & 0x000Fu));
//    *(reinterpret_cast<int16_t*>(&buffer[2])) = _data->phi;
//    *(reinterpret_cast<int16_t*>(&buffer[4])) = _data->theta;
//    *(reinterpret_cast<int16_t*>(&buffer[6])) = _data->p;
//    *(reinterpret_cast<int16_t*>(&buffer[8])) = _data->q;
//    *(reinterpret_cast<int16_t*>(&buffer[10])) = _data->r;
//    *(reinterpret_cast<uint16_t*>(&buffer[12])) = _data->loopTime;

    memcpy(buffer, _data, get_length());
}
