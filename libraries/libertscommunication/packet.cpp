#include "packet.h"
#include "mode_switch_data.h"
#include "remote_control_data.h"
#include "actuation_parameter_data.h"
#include "controller_parameter_data.h"
#include "misc_parameter_data.h"

#include <cstring>

extern "C"
{
#include "checksum.h"
}


Packet::Packet() {
    _start = START_SEQUENCE;
    _type = UnknownPacket;
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

    checksum = (packet[4]) << 8 | packet[3]; //checksum to be updated later
    
    switch (_type) {
        case ModeSwitch:
            _data = new ModeSwitchData(&packet[5]);
            break;
        case RemoteControl:
            _data = new RemoteControlData(&packet[5]);
            break;
        case Kill:
            // Nothing
            break;
        /*case ActuationParameters:
            _data = new ActuationParameterData(&packet[5]);
            break;*/
        case ControllerParameters:
            _data = new ControllerParameterData(&packet[5]);
            break;
        case MiscParameters:
            _data = new MiscParameterData(&packet[5]);
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

void Packet::to_buffer(uint8_t *buffer) {
    buffer[0] = (uint8_t)((_start & 0xFF00) >> 8);
    buffer[1] = (uint8_t)(_start & 0x00FF);
    buffer[2] = _type;
    buffer[3] = 0; //checksum to be updated later
    buffer[4] = 0; //checksum to be updated later
    memset(&buffer[5], 0, DATA_SIZE);
    _data->to_buffer(&buffer[5]);

    buffer[MAX_PACKET_SIZE - 1] = _end;
    //Calculate and add checksum
    checksum_t cs = crc_16(buffer, MAX_PACKET_SIZE);
    buffer[3] = (uint8_t)(cs & 0x00FF);
    buffer[4] = (uint8_t)((cs & 0xFF00) >> 8);
}

bool Packet::verify(byte* packet) {
    checksum_t packet_cs = (packet[4] << 8) | packet[3];
    packet[3] = 0; //checksum to be updated later
    packet[4] = 0; //checksum to be updated later
    checksum_t cs = crc_16(packet, MAX_PACKET_SIZE);
    bool result = cs == packet_cs;
    packet[3] = (uint8_t)(cs & 0x00FF);
    packet[4] = (uint8_t)((cs & 0xFF00) >> 8);
    return result;
}

