#include <cstring>
#include "parameter_data.h"

ParameterData::ParameterData(const uint8_t *data) {
    _data = new parameterData_t;
    memcpy(&_data, data, get_length());
}

int ParameterData::get_length() {
    return sizeof(parameterData_t);
}

uint32_t ParameterData::get_ack_number() {
    return 0;
}

bool ParameterData::get_expects_acknowledgement() {
    return true;
}

bool ParameterData::is_valid() {
    return true;
}

void ParameterData::to_buffer(uint8_t *buffer) {
    memcpy(buffer, _data, get_length());
}
