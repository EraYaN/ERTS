#include "packet.h"
#include "mode_switch_data.h"
#include "remote_control_data.h"

#include <cstring>

extern "C"
{
#include "checksum.h"
}


Packet::Packet() {
    _start = START_SEQUENCE;
    _type = Unknown;
    _end = END_SEQUENCE;
}

Packet::Packet(messageType_t type) {
    _start = START_SEQUENCE;
    _type = type;
    _end = END_SEQUENCE;
}

Packet::Packet(const byte* packet)
{
    _start = (packet[0]) << 8 | packet[1];
    _type = static_cast<messageType_t>(packet[2]);
#if defined(USE_CRC16)
    checksum = (packet[4]) << 8 | packet[3]; //checksum to be updated later
    memcpy(_data_segment, &packet[5], DATA_SIZE);
#elif defined(USE_CRC8)
    checksum = packet[3]; //checksum to be updated later
    memcpy(data_segment, &packet[3], DATA_SIZE);
#else
    memcpy(data_segment, &packet[3], DATA_SIZE);
#endif

    switch (_type) {
        case ModeSwitch:
            _data = new ModeSwitchData(_data_segment);
            break;
        case RemoteControl:
            _data = new RemoteControlData(_data_segment);
            break;
        case Kill:
            // Nothing
            break;
        default:
            // Not implemented.
            break;
    }

    _end = packet[MAX_PACKET_SIZE - 1];
}

Packet::~Packet()
{
    delete _data;
}

byte* Packet::get_byte_array() {
    byte* packet = new byte[MAX_PACKET_SIZE];

    packet[0] = (_start & 0xFF00) >> 8;
    packet[1] = _start & 0x00FF;
    packet[2] = _type;
#if defined(USE_CRC16)
    packet[3] = 0; //checksum to be updated later
    packet[4] = 0; //checksum to be updated later
    memset(&packet[5], 0, DATA_SIZE);
    memcpy(&packet[5], _data, DATA_SIZE);
#elif defined(USE_CRC8)
    packet[3] = 0; //checksum to be updated later
    memset(&packet[4], 0, DATA_SIZE);
    memcpy(&packet[4], data, DATA_SIZE);
#else
    memset(&packet[3], 0, DATA_SIZE);
    memcpy(&packet[3], data, DATA_SIZE);
#endif

    packet[MAX_PACKET_SIZE - 1] = _end;
    //Calculate and add checksum
#if defined(USE_CRC16)
    checksum_t cs = crc_16(packet, MAX_PACKET_SIZE);
    packet[3] = cs & 0x00FF;
    packet[4] = (cs & 0xFF00) >> 8;
#elif defined(USE_CRC8)
    checksum_t cs = crc_8(packet, MAX_PACKET_SIZE);
    packet[3] = cs;
#endif
    return packet;
}

#if defined(USE_CRC16)
bool Packet::verify(byte* packet) {
    checksum_t packet_cs = (packet[4] << 8) | packet[3];
    packet[3] = 0; //checksum to be updated later
    packet[4] = 0; //checksum to be updated later
    checksum_t cs = crc_16(packet, MAX_PACKET_SIZE);
    bool result = cs == packet_cs;
    packet[3] = cs & 0x00FF;
    packet[4] = (cs & 0xFF00) >> 8;
    return result;
}
#elif defined(USE_CRC8)
bool verify_packet(byte* packet) {
    checksum packet_cs = packet[3];
    packet[3] = 0; //checksum to be updated later
    checksum cs = crc_8(packet, MAX_PACKET_SIZE);
    bool result = cs == packet_cs;
    packet[3] = packet_cs;
    return result;
}
#else
bool verify_packet(byte* packet) {
    return true;
}
#endif

