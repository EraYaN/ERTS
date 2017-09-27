#include "controller_parameter_data.h"

ControllerParameterData::ControllerParameterData(const uint8_t *data) {
    _ackNumber = *(reinterpret_cast<const uint32_t *>(&data[0]));
    _p_yaw = *(reinterpret_cast<const uint16_t *>(&data[4]));
    _p_height = *(reinterpret_cast<const uint16_t *>(&data[6]));
    _p1_pitch_roll = *(reinterpret_cast<const uint16_t *>(&data[8]));
    _p2_pitch_roll = *(reinterpret_cast<const uint16_t *>(&data[10]));
}

void ControllerParameterData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t *>(&buffer[0])) = _ackNumber;
    *(reinterpret_cast<uint16_t *>(&buffer[4])) = _p_yaw;
    *(reinterpret_cast<uint16_t *>(&buffer[6])) = _p_height;
    *(reinterpret_cast<uint16_t *>(&buffer[8])) = _p1_pitch_roll;
    *(reinterpret_cast<uint16_t *>(&buffer[10])) = _p2_pitch_roll;
}
