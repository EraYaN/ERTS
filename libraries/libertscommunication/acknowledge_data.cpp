#include "acknowledge_data.h"

AcknowledgeData::AcknowledgeData(uint32_t number) {
    _data->number = number;
}

int AcknowledgeData::get_length() {
    return sizeof(acknowledgeData_t);
}

uint32_t AcknowledgeData::get_ack_number() {
    return 0;
}

bool AcknowledgeData::get_expects_acknowledgement() {
    return false;
}

bool AcknowledgeData::is_valid() {
    return true;
}

byte *AcknowledgeData::to_byte_array() {
    return (byte*)_data;
}
