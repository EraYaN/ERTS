#include <cstring>
#include "telemetry_data.h"

TelemetryData::TelemetryData(uint16_t battery_voltage, uint16_t phi, uint16_t theta,
                             uint16_t p, uint16_t q, uint16_t r,
                             uint16_t loop_time, flightMode_t flight_mode) {
    _data->batteryVoltage = battery_voltage;
    _data->phi = phi;
    _data->theta = theta;
    _data->p = p;
    _data->q = q;
    _data->r = r;
    _data->loopTime = loop_time;
    _data->flightMode = flight_mode;
}

int TelemetryData::get_length() {
    return sizeof(telemetryData_t);
}

bool TelemetryData::get_expects_acknowledgement() {
    return false;
}

bool TelemetryData::is_valid() {
    return _data->flightMode != None;
}

byte *TelemetryData::to_byte_array() {
    return (byte *)_data;
}
