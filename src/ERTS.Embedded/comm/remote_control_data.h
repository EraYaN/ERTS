#pragma once
#include "packet_data.h"


class RemoteControlData : public PacketData {
    remoteControlData_t *_data;

public:
    explicit RemoteControlData(byte* data);

    ~RemoteControlData() override { delete _data; }

    bool get_expects_acknowledgement() override;

    uint16_t    get_lift()  { return _data->lift;  }
    int16_t     get_roll()  { return _data->roll;  }
    int16_t     get_pitch() { return _data->pitch; }
    int16_t     get_yaw()   { return _data->yaw;   }

    bool is_valid() override;

    int get_length() override;

private:
    byte *to_byte_array() override;
};



