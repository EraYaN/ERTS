#pragma once

#include "packet_datastructures.h"
#include "packet_data.h"

class Packet {
    startSequence_t _start; //0xFeFF
    PacketData *_data = nullptr;
    byte _data_segment[DATA_SIZE];
    messageType_t _type;
    endSequence_t _end;

public:
    Packet();

    explicit Packet(messageType_t type);

    explicit Packet(const byte *packet);

    ~Packet();

    messageType_t get_type() { return _type; }

    PacketData *get_data() { return _data; }

    void set_data(PacketData *data) { _data = data; }

#if defined(USE_CRC8) || defined(USE_CRC16)
    checksum_t checksum; //CRC-8 or CRC-16
#endif

    byte *get_byte_array();

    static bool verify(byte *packet);

};

