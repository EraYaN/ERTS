#pragma once

#include "packet_data.h"

class TelemetryData : public PacketData {
    telemetryData_t *_data;

public:
    TelemetryData(uint16_t battery_voltage, uint16_t phi, uint16_t theta, uint16_t p, uint16_t q, uint16_t r,
                  uint16_t _loop_time, flightMode_t flight_mode);

    ~TelemetryData() override { delete _data; }

    int get_length() override;

    bool get_expects_acknowledgement() override;

    bool is_valid() override;

    byte *to_byte_array() override;
};
