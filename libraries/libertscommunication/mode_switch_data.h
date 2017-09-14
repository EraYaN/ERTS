#pragma once

#include "packet_data.h"

class ModeSwitchData : public PacketData {
    modeSwitchData_t *_data;

public:
    explicit ModeSwitchData(byte *data);

    ~ModeSwitchData() override { delete _data; }

    int get_length() override;

    bool get_expects_acknowledgement() override;

    flightMode_t get_new_mode() { return _data->newMode; };

    flightMode_t get_fallback_mode() { return _data->fallBackmode; };

    uint32_t get_ack_number() { return _data->ackNumber; };

    bool is_valid() override;

    byte *to_byte_array() override;
};



