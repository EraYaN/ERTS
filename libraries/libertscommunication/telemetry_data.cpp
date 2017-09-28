#include <cstring>
#include "telemetry_data.h"

TelemetryData::TelemetryData(uint16_t battery_voltage, int16_t phi, int16_t theta,
                             int16_t p, int16_t q, int16_t r,
                             uint16_t loop_time, flightMode_t flight_mode) {
    _battery_voltage = battery_voltage;
    _flight_mode = flight_mode;
    _phi = phi;
    _theta = theta;
    _p = p;
    _q = q;
    _r = r;
    _loop_time = loop_time;
}

bool TelemetryData::is_valid() {
    return _flight_mode != None;
}

void TelemetryData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint16_t*>(&buffer[0])) = static_cast<uint16_t>(((_battery_voltage & 0x0FFFu) << 4) | (_flight_mode & 0x000Fu));
    *(reinterpret_cast<int16_t*>(&buffer[2])) = _phi;
    *(reinterpret_cast<int16_t*>(&buffer[4])) = _theta;
    *(reinterpret_cast<int16_t*>(&buffer[6])) = _p;
    *(reinterpret_cast<int16_t*>(&buffer[8])) = _q;
    *(reinterpret_cast<int16_t*>(&buffer[10])) = _r;
    *(reinterpret_cast<uint16_t*>(&buffer[12])) = _loop_time;
}
