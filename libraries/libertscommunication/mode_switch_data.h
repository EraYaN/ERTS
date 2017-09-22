#pragma once

#include "packet_data.h"

class ModeSwitchData : public PacketData {
    modeSwitchData_t *_data;

public:
    explicit ModeSwitchData(const uint8_t *data);

    ~ModeSwitchData() override { delete _data; }

    int get_length() override;

    bool get_expects_acknowledgement() override;

    flightMode_t get_new_mode() { return _data->newMode; };

    flightMode_t get_fallback_mode() { return _data->fallBackmode; };

    uint32_t get_ack_number() override { return _data->ackNumber; };

    bool is_valid() override;

    void to_buffer(uint8_t *buffer) override;

};



