#pragma once
#include "packet_data.h"

class AcknowledgeData : public PacketData {
	uint32_t _ackNumber;

public:
	AcknowledgeData(const uint8_t* data);
    AcknowledgeData(uint32_t ackNumber);

	int get_length() override {
		return 4;
	};

	uint32_t get_ack_number() override {
		return _ackNumber;			
	};
	
    void to_buffer(uint8_t *buffer) override;
};
