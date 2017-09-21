#pragma once
#include "packet_data.h"


class RemoteControlData : public PacketData {
    remoteControlData_t *_data;

public:
    explicit RemoteControlData(const uint8_t *data);

    ~RemoteControlData() override { delete _data; }

    uint32_t get_ack_number() override;

    bool get_expects_acknowledgement() override;

    uint16_t    get_lift()  { return _data->lift;  }
    int16_t     get_roll()  { return _data->roll;  }
    int16_t     get_pitch() { return _data->pitch; }
    int16_t     get_yaw()   { return _data->yaw;   }

    bool is_valid() override;

    int get_length() override;

private:
    void to_buffer(uint8_t *buffer) override;
};



