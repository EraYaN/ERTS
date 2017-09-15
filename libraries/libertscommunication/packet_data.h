#pragma once

#include "packet_datastructures.h"

class PacketData {
public:
    virtual ~PacketData() = default;

    virtual int get_length() = 0;

    virtual bool get_expects_acknowledgement() = 0;

    virtual bool is_valid() = 0;

    virtual uint32_t get_ack_number() = 0;

    virtual byte* to_byte_array() = 0;
};
