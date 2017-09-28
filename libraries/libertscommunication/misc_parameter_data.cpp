#include "misc_parameter_data.h"

MiscParameterData::MiscParameterData(const uint8_t *data) {
    _ack_number = *(reinterpret_cast<const uint32_t *>(&data[0]));
    _panic_decrement = *(reinterpret_cast<const uint16_t *>(&data[4]));
    _rc_interval = *(reinterpret_cast<const uint16_t *>(&data[6]));
    _log_divider = *(reinterpret_cast<const uint16_t *>(&data[8]));
    _battery_threshold = *(reinterpret_cast<const uint16_t *>(&data[10]));
    _target_loop_time = *(reinterpret_cast<const uint16_t *>(&data[12]));
}

MiscParameterData::MiscParameterData(uint16_t panic_decrement, uint16_t rc_interval, uint16_t log_divider,
                                     uint16_t battery_threshold, uint16_t target_loop_time, uint32_t ack_number) {
    _ack_number = ack_number;
    _panic_decrement = panic_decrement;
    _rc_interval = rc_interval;
    _log_divider = log_divider;
    _battery_threshold = battery_threshold;
    _target_loop_time = target_loop_time;
}

void MiscParameterData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t *>(&buffer[0])) = _ack_number;
    *(reinterpret_cast<uint16_t *>(&buffer[4])) = _panic_decrement;
    *(reinterpret_cast<uint16_t *>(&buffer[6])) = _rc_interval;
    *(reinterpret_cast<uint16_t *>(&buffer[8])) = _log_divider;
    *(reinterpret_cast<uint16_t *>(&buffer[10])) = _battery_threshold;
    *(reinterpret_cast<uint16_t *>(&buffer[12])) = _target_loop_time;
}
