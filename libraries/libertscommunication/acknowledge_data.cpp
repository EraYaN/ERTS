#include "acknowledge_data.h"

AcknowledgeData::AcknowledgeData(const uint8_t* data) {
	_ack_number = *(reinterpret_cast<const uint32_t*>(&data[0]));
}

AcknowledgeData::AcknowledgeData(uint32_t ack_number) {
	_ack_number = ack_number;
}

void AcknowledgeData::to_buffer(uint8_t *buffer) {
	*(reinterpret_cast<uint32_t*>(&buffer[0])) = _ack_number;
}
