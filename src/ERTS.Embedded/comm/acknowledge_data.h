#pragma once
#include "packet_data.h"

class AcknowledgeData : public PacketData {
    acknowledgeData_t *_data;

public:
    explicit AcknowledgeData(uint32_t number);

    ~AcknowledgeData() override { delete _data; }

    int get_length() override;

    bool get_expects_acknowledgement() override;

    bool is_valid() override;

    byte *to_byte_array() override;
};



