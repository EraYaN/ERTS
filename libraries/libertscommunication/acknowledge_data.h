#pragma once
#include "packet_data.h"

class AcknowledgeData : public PacketData {
    acknowledgeData_t *_data;

public:
    explicit AcknowledgeData(uint32_t number);

    ~AcknowledgeData() override { delete _data; }

    int get_length() override;

    uint32_t get_ack_number() override;

    bool get_expects_acknowledgement() override;

    bool is_valid() override;

    void to_buffer(uint8_t *buffer) override;
};



