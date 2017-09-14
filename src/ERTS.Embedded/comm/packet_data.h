#pragma once

#include "packet_datastructures.h"

//TODO: Implement acknowledgement logic.

class PacketData {
public:
    virtual ~PacketData() = default;

    virtual int get_length() = 0;

    virtual bool get_expects_acknowledgement() = 0;

    virtual bool is_valid() = 0;

    // virtual void set_ack_number() = 0;

    virtual byte* to_byte_array() = 0;
};
