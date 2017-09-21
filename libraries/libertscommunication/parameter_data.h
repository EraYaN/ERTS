#pragma once
#include "packet_data.h"


class ParameterData : public PacketData {
    parameterData_t *_data;

public:
    explicit ParameterData(const uint8_t *data);

    ~ParameterData() override { delete _data; }

    uint32_t get_ack_number() override;

    bool get_expects_acknowledgement() override;

    uint16_t    get_b()  { return _data->b;  }
    uint16_t    get_d()  { return _data->d;  }

    bool is_valid() override;

    int get_length() override;

private:
    void to_buffer(uint8_t *buffer) override;
};