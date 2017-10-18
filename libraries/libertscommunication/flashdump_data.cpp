#include <cstring>
#include "flashdump_data.h"

FlashDumpData::FlashDumpData(uint16_t seqNumber, uint8_t* data){
    _seqNumber = seqNumber;
    memcpy(_data, data, DATA_SIZE-2);
}

void FlashDumpData::to_buffer(uint8_t *buffer) {
    *(reinterpret_cast<uint16_t*>(&buffer[0])) = _seqNumber;
    memcpy(&buffer[2], _data, DATA_SIZE-2);
}
