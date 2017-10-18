#pragma once

#include "packet_data.h"

class FlashDumpData : public PacketData {
    uint16_t _seqNumber;
    uint8_t _data[DATA_SIZE-2];

public:
    FlashDumpData(uint16_t seqNumber, uint8_t* data);

    int get_length() override {
        return DATA_SIZE;
    };

    void to_buffer(uint8_t *buffer) override;
};
