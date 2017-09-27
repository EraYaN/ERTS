#pragma once

#include "packet_data.h"

class TelemetryData : public PacketData {
    telemetryData_t *_data;

public:
    TelemetryData(uint16_t battery_voltage, int16_t phi, int16_t theta, int16_t p, int16_t q, int16_t r,
                  uint16_t _loop_time, flightMode_t flight_mode);

    ~TelemetryData() override { delete _data; }

    int get_length() override;

    uint32_t get_ack_number() override;

    bool get_expects_acknowledgement() override;

    bool is_valid() override;

    void to_buffer(uint8_t *buffer) override;
};
