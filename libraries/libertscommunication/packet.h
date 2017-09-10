#pragma once
#include <iostream>
#include <time.h>
#include "packetdatastructures.h"

class packet
{
public:
	packet();
	packet(const byte* packet);
	~packet();

	startSequence_t start; //0xFeFF
	messageType_t type;
#if defined(USE_CRC8) || defined(USE_CRC16)
	checksum_t checksum; //CRC-8 or CRC-16
#endif	
	byte* data = NULL;
	endSequence_t end;

	byte* get_byte_array();

	static bool verify(byte* packet);

};

