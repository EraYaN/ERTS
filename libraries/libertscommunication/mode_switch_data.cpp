#include <cstring>
#include "mode_switch_data.h"

ModeSwitchData::ModeSwitchData(const uint8_t *data) {
    _new_mode = (flightMode_t)data[4];
	_fallback_mode = (flightMode_t)data[5];
	_ack_number = *(reinterpret_cast<const uint32_t*>(&data[0]));
}

ModeSwitchData::ModeSwitchData(flightMode_t new_mode, flightMode_t fallback_mode, uint32_t ack_number) {
	_new_mode = new_mode;
	_fallback_mode = fallback_mode;
	_ack_number = ack_number;
}

bool ModeSwitchData::is_valid() {
    return _new_mode != None;
}

void ModeSwitchData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t*>(&buffer[0])) = _ack_number;
	*(reinterpret_cast<flightMode_t*>(&buffer[4])) = _new_mode;
	*(reinterpret_cast<flightMode_t*>(&buffer[5])) = _fallback_mode;
	
}
