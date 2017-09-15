#include <cstring>
#include "telemetry_data.h"

TelemetryData::TelemetryData(uint16_t battery_voltage, int16_t phi, int16_t theta,
                             int16_t p, int16_t q, int16_t r,
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

uint32_t TelemetryData::get_ack_number() {
    return 0;
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
