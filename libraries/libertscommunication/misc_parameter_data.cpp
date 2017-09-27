#include "misc_parameter_data.h"

MiscParameterData::MiscParameterData(const uint8_t *data) {
    _ackNumber = *(reinterpret_cast<const uint32_t *>(&data[0]));
    _panic_decrement = *(reinterpret_cast<const uint16_t *>(&data[4]));
    _rc_interval = *(reinterpret_cast<const uint16_t *>(&data[6]));
    _log_divider = *(reinterpret_cast<const uint16_t *>(&data[8]));
    _battery_threshold = *(reinterpret_cast<const uint16_t *>(&data[10]));
    _target_loop_time = *(reinterpret_cast<const uint16_t *>(&data[12]));
}

void MiscParameterData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t *>(&buffer[0])) = _ackNumber;
    *(reinterpret_cast<uint16_t *>(&buffer[4])) = _panic_decrement;
    *(reinterpret_cast<uint16_t *>(&buffer[6])) = _rc_interval;
    *(reinterpret_cast<uint16_t *>(&buffer[8])) = _log_divider;
    *(reinterpret_cast<uint16_t *>(&buffer[10])) = _battery_threshold;
    *(reinterpret_cast<uint16_t *>(&buffer[12])) = _target_loop_time;
}
