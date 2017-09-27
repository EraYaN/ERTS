#include <cstring>
#include "parameter_data.h"

ParameterData::ParameterData(const uint8_t *data) {
	_b = *(reinterpret_cast<const uint16_t*>(&data[0]));
	_d = *(reinterpret_cast<const uint16_t*>(&data[2]));
	_ackNumber = *(reinterpret_cast<const uint32_t*>(&data[4]));
}

ParameterData::ParameterData(uint16_t b, uint16_t d, uint32_t ackNumber) {
	_b = b;
	_d = d;
	_ackNumber = ackNumber;
}

void ParameterData::to_buffer(uint8_t *buffer) {
	*(reinterpret_cast<uint16_t*>(&buffer[0])) = _b;
	*(reinterpret_cast<uint16_t*>(&buffer[2])) = _d;
	*(reinterpret_cast<uint32_t*>(&buffer[4])) = _ackNumber;
}
