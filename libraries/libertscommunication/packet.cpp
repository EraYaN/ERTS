#include "packet.h"
#include <checksum.h>

packet::packet() {
	start = START_SEQUENCE;
	type = Acknowledge;
}

packet::packet(const byte* packet)
{
	start = (packet[0]) << 8 | packet[1];
	type = static_cast<messageType_t>(packet[2]);	
#if defined(USE_CRC16)
	checksum = (packet[3]) << 8 | packet[4]; //checksum to be updated later	
	memcpy(data, &packet[5], DATA_SIZE);
#elif defined(USE_CRC8)
	checksum = packet[3]; //checksum to be updated later	
	memcpy(data, &packet[3], DATA_SIZE);
#else
	memcpy(data, &packet[3], DATA_SIZE);
#endif
	end = packet[MAX_PACKET_SIZE - 1];
}

packet::~packet()
{
	if (data != NULL)
		delete[] data;
}

byte* packet::get_byte_array() {
	byte* packet = new byte[MAX_PACKET_SIZE];

	packet[0] = (start & 0xFF00) >> 8;
	packet[1] = start & 0x00FF;
	packet[2] = type;
#if defined(USE_CRC16)
	packet[3] = 0; //checksum to be updated later
	packet[4] = 0; //checksum to be updated later	
	memset(&packet[5], 0, DATA_SIZE);
	memcpy(&packet[5], data, DATA_SIZE);
#elif defined(USE_CRC8)
	packet[3] = 0; //checksum to be updated later	
	memset(&packet[4], 0, DATA_SIZE);
	memcpy(&packet[4], data, DATA_SIZE);
#else
	memset(&packet[3], 0, DATA_SIZE);
	memcpy(&packet[3], data, DATA_SIZE);
#endif

	packet[MAX_PACKET_SIZE - 1] = end;
	//Calculate and add checksum
#if defined(USE_CRC16)
	checksum_t cs = crc_16(packet, MAX_PACKET_SIZE);
	packet[3] = (cs & 0xFF00) >> 8;
	packet[4] = cs & 0x00FF;
#elif defined(USE_CRC8)
	checksum_t cs = crc_8(packet, MAX_PACKET_SIZE);
	packet[3] = cs;
#endif
	return packet;
}

#if defined(USE_CRC16)
bool packet::verify(byte* packet) {
	checksum_t packet_cs = (packet[3] << 8) | packet[4];
	packet[3] = 0; //checksum to be updated later
	packet[4] = 0; //checksum to be updated later
	checksum_t cs = crc_16(packet, MAX_PACKET_SIZE);
	bool result = cs == packet_cs;
	packet[3] = (packet_cs & 0xFF00) >> 8;
	packet[4] = packet_cs & 0x00FF;
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

