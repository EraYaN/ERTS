#pragma once

#include "packet_data.h"
#include <algorithm>

class ExceptionData : public PacketData {
    exceptionType_t _type;
    char _message[MAX_MESSAGE_LENGTH];

public:
    ExceptionData(const uint8_t* data);
    ExceptionData(exceptionType_t type, const char* message);

    int get_length() override {
        return 14;
    };
    
    void to_buffer(uint8_t *buffer) override;
};
