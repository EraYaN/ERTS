#include "actuation_parameter_data.h"

ActuationParameterData::ActuationParameterData(const uint8_t *data) {
    _ackNumber = *(reinterpret_cast<const uint32_t *>(&data[0]));
    _rate_pitch_roll_lift = *(reinterpret_cast<const uint16_t *>(&data[4]));
    _rate_yaw = *(reinterpret_cast<const uint16_t *>(&data[6]));
    _motor_min = *(reinterpret_cast<const uint16_t *>(&data[8]));
    _motor_max = *(reinterpret_cast<const uint16_t *>(&data[10]));
}

void ActuationParameterData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t *>(&buffer[0])) = _ackNumber;
    *(reinterpret_cast<uint16_t *>(&buffer[4])) = _rate_pitch_roll_lift;
    *(reinterpret_cast<uint16_t *>(&buffer[6])) = _rate_yaw;
    *(reinterpret_cast<uint16_t *>(&buffer[8])) = _motor_min;
    *(reinterpret_cast<uint16_t *>(&buffer[10])) = _motor_max;
}
