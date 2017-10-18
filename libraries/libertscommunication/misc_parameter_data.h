#pragma once

#include "packet_data.h"

class MiscParameterData : public PacketData {
    uint8_t _func_raw;
    uint8_t _func_logging;    
    uint8_t _func_wireless;
    uint32_t _ack_number; // For keeping track of acknowledgements

public:
    explicit MiscParameterData(const uint8_t *data);
    MiscParameterData(uint8_t func_raw, uint8_t func_logging, uint8_t func_wireless, uint32_t ack_number);

    uint32_t get_ack_number() override {
        return _ack_number;
    };

    uint8_t get_func_raw() {
        return _func_raw;
    }

    uint8_t get_func_logging() {
        return _func_logging;
    }  

    uint8_t get_func_wireless() {
        return _func_wireless;
    }

    bool get_expects_acknowledgement() override {
        return true;
    };

    int get_length() override {
        return 7;
    };

    void to_buffer(uint8_t *buffer) override;
};



