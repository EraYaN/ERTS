#include <cstring>
#include "mode_switch_data.h"

ModeSwitchData::ModeSwitchData(const uint8_t *data) {
    _newMode = (flightMode_t)data[4];
	_fallBackmode = (flightMode_t)data[5];
	_ackNumber = *(reinterpret_cast<const uint32_t*>(&data[0]));
}

ModeSwitchData::ModeSwitchData(flightMode_t newMode, flightMode_t fallBackmode, uint32_t ackNumber) {
	_newMode = newMode;
	_fallBackmode = fallBackmode;
	_ackNumber = ackNumber;
}

bool ModeSwitchData::is_valid() {
    return _newMode != None;
}

void ModeSwitchData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t*>(&buffer[0])) = _ackNumber;
	*(reinterpret_cast<flightMode_t*>(&buffer[4])) = _newMode;
	*(reinterpret_cast<flightMode_t*>(&buffer[5])) = _fallBackmode;
	
}
