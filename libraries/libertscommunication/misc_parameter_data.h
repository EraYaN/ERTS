#pragma once

#include "packet_data.h"

class MiscParameterData : public PacketData {
    uint16_t _panic_decrement;
    uint16_t _rc_interval;
    uint16_t _log_divider;
    uint16_t _battery_threshold;
    uint16_t _target_loop_time;
    uint32_t _ack_number; // For keeping track of acknowledgements

public:
    explicit MiscParameterData(const uint8_t *data);
    MiscParameterData(uint16_t panic_decrement, uint16_t rc_interval, uint16_t log_divider,
                          uint16_t battery_threshold, uint16_t target_loop_time, uint32_t ack_number);

    uint32_t get_ack_number() override {
        return _ack_number;
    };

    uint16_t get_panic_decrement() {
        return _panic_decrement;
    }

    uint16_t get_rc_interval() {
        return _rc_interval;
    }

    uint16_t get_log_divider() {
        return _log_divider;
    }

    uint16_t get_battery_threshold() {
        return _battery_threshold;
    }

    uint16_t get_target_loop_time() {
        return _target_loop_time;
    }

    bool get_expects_acknowledgement() override {
        return true;
    };

    int get_length() override {
        return 14;
    };

    void to_buffer(uint8_t *buffer) override;
};



