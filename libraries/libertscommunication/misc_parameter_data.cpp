#include "misc_parameter_data.h"

MiscParameterData::MiscParameterData(const uint8_t *data) {
    _ack_number = *(reinterpret_cast<const uint32_t *>(&data[0]));
    _func_raw = data[4];
    _func_logging = data[5];
    _func_wireless = data[6];
}

MiscParameterData::MiscParameterData(uint8_t func_raw, uint8_t func_logging, uint8_t func_wireless, uint32_t ack_number) {
    _ack_number = ack_number;
    _func_raw = func_raw;
    _func_logging = func_logging;
    _func_wireless = func_wireless;
}

void MiscParameterData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t *>(&buffer[0])) = _ack_number;
    buffer[4] = _func_raw;
    buffer[5] = _func_logging;
    buffer[6] = _func_wireless;
}
