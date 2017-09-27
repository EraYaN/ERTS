#pragma once
#include "packet_data.h"


class ParameterData : public PacketData {
	uint16_t _b; // Divider for lift, pitch and roll.
	uint16_t _d; // Divider for yaw.
	uint32_t _ackNumber; // For keeping track of acknowledgements

public:
	ParameterData(const uint8_t *data);
	ParameterData(uint16_t b, uint16_t d, uint32_t ackNumber);

	uint32_t get_ack_number() override {
		return _ackNumber;
	};

	bool get_expects_acknowledgement() override {
		return true;
	};

	uint16_t    get_b() {
		return _b;
	}
	uint16_t    get_d() {
		return _d;
	}

	int get_length() override {
		return 8;
	};

	void to_buffer(uint8_t *buffer) override;
};
