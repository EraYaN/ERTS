#pragma once

#include "packet_data.h"

class ControllerParameterData : public PacketData {
    uint16_t _p_yaw;
    uint16_t _p_height; // Divider for yaw.
    uint16_t _p1_pitch_roll;
    uint16_t _p2_pitch_roll;
    uint32_t _ack_number; // For keeping track of acknowledgements

public:
    explicit ControllerParameterData(const uint8_t *data);

    ControllerParameterData(uint16_t p_yaw, uint16_t p_height, uint16_t p1_pitch_roll, uint16_t p2_pitch_roll,
                            uint32_t ack_number);

    uint32_t get_ack_number() override {
        return _ack_number;
    };

    uint16_t get_p_yaw() {
        return _p_yaw;
    }

    uint16_t get_p_height() {
        return _p_height;
    }

    uint16_t get_p1_pitch_roll() {
        return _p1_pitch_roll;
    }

    uint16_t get_p2_pitch_roll() {
        return _p2_pitch_roll;
    }

    bool get_expects_acknowledgement() override {
        return true;
    };

    int get_length() override {
        return 12;
    };

    void to_buffer(uint8_t *buffer) override;
};



