#pragma once

#include "packet_data.h"

class ControllerParameterData : PacketData {
    uint16_t _p_yaw;
    uint16_t _p_height; // Divider for yaw.
    uint16_t _p1_pitch_roll;
    uint16_t _p2_pitch_roll;
    uint32_t _ackNumber; // For keeping track of acknowledgements

public:
    explicit ControllerParameterData(const uint8_t *data);

    uint32_t get_ack_number() override {
        return _ackNumber;
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



