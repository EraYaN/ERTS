#include "controller_parameter_data.h"

ControllerParameterData::ControllerParameterData(const uint8_t *data) {
    _ack_number = *(reinterpret_cast<const uint32_t *>(&data[0]));
    _p_yaw = *(reinterpret_cast<const uint16_t *>(&data[4]));
    _p_height = *(reinterpret_cast<const uint16_t *>(&data[6]));
    _p1_pitch_roll = *(reinterpret_cast<const uint16_t *>(&data[8]));
    _p2_pitch_roll = *(reinterpret_cast<const uint16_t *>(&data[10]));
}

ControllerParameterData::ControllerParameterData(uint16_t p_yaw, uint16_t p_height, uint16_t p1_pitch_roll,
                                                 uint16_t p2_pitch_roll, uint32_t ack_number) {
    _ack_number = ack_number;
    _p_yaw = p_yaw;
    _p_height = p_height;
    _p1_pitch_roll = p1_pitch_roll;
    _p2_pitch_roll = p2_pitch_roll;
}

void ControllerParameterData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint32_t *>(&buffer[0])) = _ack_number;
    *(reinterpret_cast<uint16_t *>(&buffer[4])) = _p_yaw;
    *(reinterpret_cast<uint16_t *>(&buffer[6])) = _p_height;
    *(reinterpret_cast<uint16_t *>(&buffer[8])) = _p1_pitch_roll;
    *(reinterpret_cast<uint16_t *>(&buffer[10])) = _p2_pitch_roll;
}
