#include <stdio.h>
#include <stdint.h>


#ifdef _MSC_VER
#define PACKED_STRUCT(name) \
    __pragma(pack(push, 1)) struct name __pragma(pack(pop))
#elif defined(__GNUC__)
#define PACKED_STRUCT(name) struct __attribute__((packed)) name
#endif

#define START_SEQUENCE 0xFEFF
#define END_SEQUENCE EOF

typedef uint8_t byte;
typedef uint16_t startSequence;
typedef uint16_t checksum;
typedef uint8_t messageLength;
typedef uint8_t messageType; //enum would probably be 32 bits, not worth it
typedef uint8_t messageID; //enum would probably be 32 bits, not worth it
typedef uint8_t endSequence;

//__pragma(pack(push, 1))
// 7 bytes
typedef struct messageHeader {
	startSequence Start; //0xFeFF
	checksum Checksum; //CRC-16
	messageLength len; //Only really needed if we use var length packets (BLE gives you 20 bytes)
	messageType type;
	messageID id;
} messageHeader_t;

// Data is 12 bytes as byte[]

// 1 byte
typedef struct messageFooter {
	endSequence End;
} messageFooter_t;





int main() {
	printf("Size of messageHeader: %d\n", sizeof(messageHeader_t));
	printf("Size of messageFooter: %d\n", sizeof(messageFooter_t));
	getchar();
	return 0;
}