#pragma once

#include "packet_data.h"

class ModeSwitchData : public PacketData {
	flightMode_t _new_mode;
	flightMode_t _fallback_mode; //No fallback if None (0xF) is specified)
	uint32_t _ack_number; // For keeping track of acknowledgements

public:
	ModeSwitchData(const uint8_t *data);
	ModeSwitchData(flightMode_t new_mode, flightMode_t fallback_mode, uint32_t ack_number);

	int get_length() override {
		return 6;
	};

	bool get_expects_acknowledgement() override {
		return true;
	};

	flightMode_t get_new_mode() {
		return _new_mode;
	};

	flightMode_t get_fallback_mode() {
		return _fallback_mode;
	};

	uint32_t get_ack_number() override {
		return _ack_number;
	};

	bool is_valid() override;

	void to_buffer(uint8_t *buffer) override;
};
