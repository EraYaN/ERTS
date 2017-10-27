#pragma once

#include "packet_datastructures.h"
#include "packet_data.h"

class Packet {
    startSequence_t _start; //0xFeFF
    PacketData *_data = nullptr;
    messageType_t _type;
    endSequence_t _end;
    checksum_t checksum; //CRC-16

public:
    //Author: Erwin
    Packet();
    //Author: Robin
    explicit Packet(messageType_t type);
    //Author: Erwin
    explicit Packet(const byte *packet);

    ~Packet();

    messageType_t get_type() { return _type; }

    PacketData *get_data() { return _data; }
    
    void set_data(PacketData *data) { _data = data; }
    //Author: Erwin
    void to_buffer(uint8_t *buffer);
    //Author: Erwin
    static bool verify(byte *packet);

};

