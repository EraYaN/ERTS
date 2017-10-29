#pragma once

#include "packet_data.h"

//See base class for author info
class ActuationParameterData : public PacketData {
    uint16_t _rate_pitch_roll_lift; // Divider for lift, pitch and roll.
    uint16_t _rate_yaw; // Divider for yaw.
    uint16_t _motor_min;
    uint16_t _motor_max;
    uint32_t _ack_number; // For keeping track of acknowledgements

public:
    explicit ActuationParameterData(const uint8_t *data);

    ActuationParameterData(uint16_t rate_roll_pitch_lift, uint16_t rate_yaw, uint16_t motor_min, uint16_t motor_max,
                           uint32_t ack_number);

    uint32_t get_ack_number() override {
        return _ack_number;
    };

    uint16_t get_rate_pitch_roll_lift() {
        return _rate_pitch_roll_lift;
    }

    uint16_t get_rate_yaw() {
        return _rate_yaw;
    }

    uint16_t get_motor_min() {
        return _motor_min;
    }

    uint16_t get_motor_max() {
        return _motor_max;
    }

    bool get_expects_acknowledgement() override {
        return true;
    };

    int get_length() override {
        return 12;
    };

    void to_buffer(uint8_t *buffer) override;
};
