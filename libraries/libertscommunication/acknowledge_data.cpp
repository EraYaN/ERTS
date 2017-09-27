#include "acknowledge_data.h"

AcknowledgeData::AcknowledgeData(const uint8_t* data) {
	_ackNumber = *(reinterpret_cast<const uint32_t*>(&data[0]));
}

AcknowledgeData::AcknowledgeData(uint32_t ackNumber) {
	_ackNumber = ackNumber;
}

void AcknowledgeData::to_buffer(uint8_t *buffer) {
	*(reinterpret_cast<uint32_t*>(&buffer[0])) = _ackNumber;
}
