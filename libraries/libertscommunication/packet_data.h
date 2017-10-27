#pragma once
#include <cstring>
#include "packet_datastructures.h"

class PacketData {
public:
    //Author: Erwin
	virtual ~PacketData() = default;

	virtual int get_length() = 0;

	virtual bool get_expects_acknowledgement() {
		return false;
	};

	virtual bool is_valid() {
		return true;
	};

	virtual uint32_t get_ack_number() {
		return 0;
	};
    //Author: Erwin
	virtual void to_buffer(uint8_t *buffer) = 0;
};
