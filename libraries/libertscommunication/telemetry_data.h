#pragma once

#include "packet_data.h"

class TelemetryData : public PacketData {
	uint16_t _battery_voltage;
	flightMode_t _flight_mode;
	int16_t _phi;
	int16_t _theta;
	int16_t _psi;
	int16_t _pressure;
	int16_t _r; // Unused
	uint16_t _loop_time;

public:
    TelemetryData(uint16_t battery_voltage, int16_t phi, int16_t theta, int16_t psi, int16_t pressure, int16_t r,
                  uint16_t loop_time, flightMode_t flight_mode);

	int get_length() override {
		return 14;
	};

    bool is_valid() override;

    void to_buffer(uint8_t *buffer) override;
};
