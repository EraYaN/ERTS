#pragma once

#include "packet_data.h"

class RemoteControlData : public PacketData {
	uint16_t _lift; //Throttle
	int16_t _roll; //Aileron
	int16_t _pitch; //Elevator
	int16_t _yaw; // Rudder
public:
    RemoteControlData(uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw);
	RemoteControlData(const uint8_t *data);    

	int get_length() override {
		return 8;
	};

    uint16_t    get_lift()  { return _lift;  }
    int16_t     get_roll()  { return _roll;  }
    int16_t     get_pitch() { return _pitch; }
    int16_t     get_yaw()   { return _yaw;   }
 
    void to_buffer(uint8_t *buffer) override;
};
