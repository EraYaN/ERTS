#include "remote_control_data.h"

RemoteControlData::RemoteControlData(uint16_t lift,int16_t roll,int16_t pitch,int16_t yaw) {
	_lift = lift;
	_roll = roll;
	_pitch = pitch;
	_yaw = yaw;    
}
RemoteControlData::RemoteControlData(const uint8_t *data) {
	_lift = *(reinterpret_cast<const uint16_t*>(&data[0]));
	_roll = *(reinterpret_cast<const int16_t*>(&data[2]));
	_pitch = *(reinterpret_cast<const int16_t*>(&data[4]));
	_yaw = *(reinterpret_cast<const int16_t*>(&data[6]));
}

void RemoteControlData::to_buffer(uint8_t *buffer) {
	*(reinterpret_cast<uint16_t*>(&buffer[0])) = _lift;
	*(reinterpret_cast<int16_t*>(&buffer[2])) = _roll;
	*(reinterpret_cast<int16_t*>(&buffer[4])) = _pitch;
	*(reinterpret_cast<int16_t*>(&buffer[6])) = _yaw;
}
