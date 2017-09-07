#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>
#include <checksum.h>
#include <time.h>


#ifdef _MSC_VER
#define PACKED_STRUCT(name) \
    __pragma(pack(push, 1)) struct name __pragma(pack(pop))
#elif defined(__GNUC__)
#define PACKED_STRUCT(name) struct __attribute__((packed)) name
#endif

#define START_SEQUENCE 0xFEFF
#define END_SEQUENCE EOF
#define MAX_PACKET_SIZE 20

//#define USE_CRC16
#define USE_CRC8

typedef uint8_t byte;
typedef uint16_t startSequence;
#if defined(USE_CRC16)
typedef uint16_t checksum;
#elif defined(USE_CRC8)
typedef uint8_t checksum;
#endif
typedef uint8_t messageType; //enum would probably be 32 bits, not worth it
typedef uint8_t messageID; //enum would probably be 32 bits, not worth it
typedef uint8_t endSequence;

#ifdef _MSC_VER
__pragma(pack(push, 1))
#endif
// 4, 5 or 6 bytes bytes
typedef struct messageHeader {
	startSequence start; //0xFeFF
	messageType type;
	messageID id;
#if defined(USE_CRC8) || defined(USE_CRC16)
	checksum checksum; //CRC-8 or CRC-16
#endif	
} messageHeader_t;

// Data is 13, 14 or 15 bytes as byte[]

// 1 byte
typedef struct messageFooter {
	endSequence end;
} messageFooter_t;

#ifdef _MSC_VER
__pragma(pack(pop))
#endif

int dataSize = 1;

byte* get_byte_array(messageHeader_t* header, const byte* data, int data_len, messageFooter_t* footer) {
	byte* packet = malloc(MAX_PACKET_SIZE);
	if (data_len > dataSize) {
		//truncate input data
		//TODO multi part
		data_len = dataSize;
	}

	packet[0] = (header->start & 0xFF00) >> 8;
	packet[1] = header->start & 0x00FF;
	packet[2] = header->type;
	packet[3] = header->id;
#if defined(USE_CRC16)
	packet[4] = 0; //checksum to be updated later
	packet[5] = 0; //checksum to be updated later	
	memset(&packet[6], 0, dataSize);
	memcpy(&packet[6], data, data_len);
#elif defined(USE_CRC8)
	packet[4] = 0; //checksum to be updated later	
	memset(&packet[5], 0, dataSize);
	memcpy(&packet[5], data, data_len);
#else
	memset(&packet[4], 0, dataSize);
	memcpy(&packet[4], data, data_len);
#endif

	packet[MAX_PACKET_SIZE - 1] = footer->end;
#if defined(USE_CRC16)
	checksum cs = crc_16(packet, MAX_PACKET_SIZE);
	packet[4] = (cs & 0xFF00) >> 8;
	packet[5] = cs & 0x00FF;
#elif defined(USE_CRC8)
	checksum cs = crc_8(packet, MAX_PACKET_SIZE);
	packet[4] = cs;
#endif
	return packet;
}

void get_packet_contents(const byte* packet, messageHeader_t** header, byte** data, messageFooter_t** footer) {
	*header = malloc(sizeof(messageHeader_t));
	*data = malloc(dataSize);
	*footer = malloc(sizeof(messageFooter_t));

	(*header)->start = (packet[0]) << 8 | packet[1];
	(*header)->type = packet[2];
	(*header)->id = packet[3];

#if defined(USE_CRC16)
	(*header)->checksum = (packet[4]) << 8 | packet[5]; //checksum to be updated later	
	memcpy(mD, &packet[6], dataSize);
#elif defined(USE_CRC8)
	(*header)->checksum = packet[4]; //checksum to be updated later	
	memcpy((*data), &packet[5], dataSize);
#else
	memcpy((*data), &packet[4], dataSize);
#endif

	(*footer)->end = packet[MAX_PACKET_SIZE - 1];

}


#if defined(USE_CRC16)
bool verify_packet(byte* packet) {
	checksum packet_cs = (packet[4] << 8) | packet[5];
	packet[4] = 0; //checksum to be updated later
	packet[5] = 0; //checksum to be updated later
	checksum cs = crc_16(packet, MAX_PACKET_SIZE);
	bool result = cs == packet_cs;
	packet[4] = (packet_cs & 0xFF00) >> 8;
	packet[5] = packet_cs & 0x00FF;
	return result;
}
#elif defined(USE_CRC8)
bool verify_packet(byte* packet) {
	checksum packet_cs = packet[4];
	packet[4] = 0; //checksum to be updated later
	checksum cs = crc_8(packet, MAX_PACKET_SIZE);
	bool result = cs == packet_cs;
	packet[4] = packet_cs;
	return result;
}
#else
bool verify_packet(byte* packet) {
	return true;
}
#endif



byte* flip_random_bit(byte* array, int len, int count) {
	byte* array_error = malloc(len);
	memcpy(array_error, array, len);
	for (int i = 0; i < count; i++) {
		int bitpos = rand() % (len * 8);
		int bytepos = bitpos / 8;
		bitpos = bitpos - bytepos * 8;
		byte mask = 1 << bitpos;
		array_error[bytepos] ^= mask;
	}
	return array_error;
}
void print_packet_array(byte* packet) {
	for (int i = 0; i < MAX_PACKET_SIZE; i++) {
		printf("%02X ", packet[i]);
	}
	printf("\n");
}

void print_packet(messageHeader_t* header, byte* data, messageFooter_t* footer) {

	printf("header->start:    %04X\n", header->start);
	printf("header->type:       %02X\n", header->type);
	printf("header->id:         %02X\n", header->id);
#if defined(USE_CRC16)
	printf("header->checksum: %04X\n", header->checksum);
#elif defined(USE_CRC8)
	printf("header->checksum:   %02X\n", header->checksum);
#endif
	printf("Data: ");
	for (int i = 0; i < dataSize; i++) {
		printf("%02X ", data[i]);
	}
	printf("\n");
	printf("footer->end:        %02X\n", footer->end);


}



int main() {
	printf("Size of messageHeader: %d\n", sizeof(messageHeader_t));
	printf("Size of messageFooter: %d\n", sizeof(messageFooter_t));
	printf("Size of byte: %d\n", sizeof(byte));
	dataSize = MAX_PACKET_SIZE - sizeof(messageHeader_t) - sizeof(messageFooter_t);
	while (dataSize % sizeof(byte) > 0) {
		dataSize--;
		if (dataSize <= 0)
			printf("Protocol definition error, space for data is zero.\n");
		exit(-2);
	}
	printf("Size of data: %d\n", dataSize);
	messageHeader_t* mH = malloc(sizeof(messageHeader_t));
	byte* mD = malloc(dataSize);
	messageFooter_t* mF = malloc(sizeof(messageFooter_t));

	mH->start = START_SEQUENCE;
	mH->type = 0xF0;
	mH->id = 0x12;

	mD[0] = 1;
	mD[1] = 2;
	mD[2] = 3;
	mD[3] = 4;
	mD[4] = 5;
	mD[5] = 6;
	mD[6] = 7;
	mD[7] = 8;

	mF->end = END_SEQUENCE;
	printf("\nOriginal packet.\n");
	print_packet(mH, mD, mF);

	byte* packet = get_byte_array(mH, mD, 8, mF);

	printf("\nEncoded packet.\n");
	print_packet_array(packet);

	printf("Packet is valid: %s\n", verify_packet(packet) ? "Yes" : "No");

	messageHeader_t* mH_d = NULL;
	byte* mD_d = NULL;
	messageFooter_t* mF_d = NULL;

	get_packet_contents(packet, &mH_d, &mD_d, &mF_d);
	printf("\nDecoded packet.\n");
	print_packet(mH_d, mD_d, mF_d);


	srand(time(NULL));

	printf("Testing robustness...\n");
#if defined(USE_CRC16)
	printf("Using: CRC-16\n");
#elif defined(USE_CRC8)
	printf("Using: CRC-8\n");
#else
	printf("Using: No CRC\n");
#endif


	for (int bits = 1; bits <= 16; bits++) {
		int iterations = 50000;

		int num_good = 0;
		int num_bad = 0;

		for (int iter = 0; iter < iterations; iter++) {
			byte* new_packet = flip_random_bit(packet, MAX_PACKET_SIZE, bits);
			if (verify_packet(new_packet))
				num_good++;
			else
				num_bad++;
		}
		printf("%2d-bit flip:\t%6d out of %6d failed\t%6d out of %6d passed\t%4.2f %% confidence.\n", bits, num_bad, iterations, num_good, iterations, (double)num_bad / iterations*100.f);
	}



	getchar();
	return 0;
}