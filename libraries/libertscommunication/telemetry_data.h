#pragma once

#include "packet_data.h"

class TelemetryData : public PacketData {
	uint16_t _batteryVoltage;
	flightMode_t _flightMode;
	int16_t _phi;
	int16_t _theta;
	int16_t _p;
	int16_t _q;
	int16_t _r;
	uint16_t _loopTime;

public:
    TelemetryData(uint16_t battery_voltage, int16_t phi, int16_t theta, int16_t p, int16_t q, int16_t r,
                  uint16_t loop_time, flightMode_t flight_mode);

	int get_length() override {
		return 14;
	};

    bool is_valid() override;

    void to_buffer(uint8_t *buffer) override;
};
