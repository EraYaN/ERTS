#pragma once

#include "packet_data.h"

class ModeSwitchData : public PacketData {
	flightMode_t _newMode;
	flightMode_t _fallBackmode; //No fallback if None (0xF) is specified)
	uint32_t _ackNumber; // For keeping track of acknowledgements

public:
	ModeSwitchData(const uint8_t *data);
	ModeSwitchData(flightMode_t newMode, flightMode_t fallBackmode, uint32_t ackNumber);

	int get_length() override {
		return 6;
	};

	bool get_expects_acknowledgement() override {
		return true;
	};

	flightMode_t get_new_mode() {
		return _newMode;
	};

	flightMode_t get_fallback_mode() {
		return _fallBackmode;
	};

	uint32_t get_ack_number() override {
		return _ackNumber;
	};

	bool is_valid() override;

	void to_buffer(uint8_t *buffer) override;
};
