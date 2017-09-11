#pragma once
#include <cstdint>
#if defined(__GNUC__)
#define PACK __attribute__((packed))
#else
#define PACK
#endif

#define USE_CRC16
//#define USE_CRC8

#define START_SEQUENCE 0xFEFF
#define END_SEQUENCE EOF
#define MAX_PACKET_SIZE 20
#if defined(USE_CRC16)
#define HEADER_SIZE 5
#elif defined(USE_CRC8)
#define HEADER_SIZE 4
#else
#define HEADER_SIZE 3
#endif
#define FOOTER_SIZE 1
#define DATA_SIZE (MAX_PACKET_SIZE - HEADER_SIZE - FOOTER_SIZE)

typedef uint8_t byte;
typedef uint16_t startSequence_t;
#if defined(USE_CRC16)
typedef uint16_t checksum_t;
#elif defined(USE_CRC8)
typedef uint8_t checksum_t;
#endif
typedef uint8_t endSequence_t;

enum messageType_t : byte {
	//Primary commands (0x00-0x1F)
	Unknown = 0x00, ///Default message type
	ModeSwitch = 0x01, ///Expects Acknowledgement

	//Status messages 0x20-0x2F
	Acknowledge = 0x20,  ///Is Acknowledgement

	//Periodic messages (0x30-0x3F)
	Telemetry = 0x30, ///Expects no Acknowledgement
	RemoteControl = 0x31, ///Expects no Acknowledgement

	//Parameter messages (0x40-0x9F)
	SetControllerRollPID = 0x40, ///Expects Acknowledgement
	SetControllerPitchPID = 0x41, ///Expects Acknowledgement
	SetControllerYawPID = 0x42, ///Expects Acknowledgement
	SetControllerHeightPID = 0x43, ///Expects Acknowledgement
	SetMessageFrequencies = 0x44, ///Expects Acknowledgement. TelemetryFrequency, RemoteControlFrequency and LoopFreqency

	//Reserved for future use (0xA0-0xDF)

	//Exceptions, system commands and other failure mode related stuff (0xF0 - 0xFD)
	Reset = 0xFB, ///Expects Acknolegdement. Resets the Embedded System
	Kill = 0xFC, ///Expects Acknolegdement. Kills all activity
	Exception = 0xFD ///Expects no Acknolegdement. Reports exception to peer.

	//Reserved (0xFE-0xFF)
};

// 4 bits max
enum flightMode_t : uint8_t {
	Safe = 0x0,
	Panic = 0x1,
	Manual = 0x2,
	Calibration = 0x3,
	YawControl = 0x4,
	FullControl = 0x5,
	Raw = 0x6,
	Height = 0x7,
	Wireless = 0x8,
	None = 0xF
};

enum exceptionType_t : uint8_t {
	UnknownException = 0x00,
	InvalidModeException = 0x01,
	NotCalibratedException = 0x02,
	ValidationException = 0x03
};

#ifdef _MSC_VER
__pragma(pack(push, 1))
#endif


// 6 bytes
typedef struct PACK modeSwitchData_tag {
	flightMode_t newMode;
	flightMode_t fallBackmode; //No fallback if None (0xF) is specified)
	uint32_t ackNumber; // For keeping track of acknowledgements	
} modeSwitchData_t;

// 4 bytes
typedef struct PACK acknowledgeData_tag {
	uint32_t number; // For keeping track of acknowledgements	
} acknowledgeData_t;

// 14 bytes
typedef struct PACK telemetryData_tag {
	uint16_t batteryVoltage : 12;
	flightMode_t flightMode : 4;
	uint16_t phi;
	uint16_t theta;
	uint16_t p;
	uint16_t q;
	uint16_t r;
	uint16_t loopTime;
} telemetryData_t;

// 12 bytes
typedef struct PACK remoteControlData_tag {
	uint16_t lift; //Throttle
	uint16_t roll; //Aileron
	uint16_t pitch; //Elevator
	uint16_t yaw; // Rudder
	uint32_t ackNumber; // For keeping track of acknowledgements	
} remoteControlData_t;


#define MAX_MESSAGE_LENGTH 13
// 14 bytes
typedef struct PACK exceptionData_tag {
	exceptionType_t exceptionType; 
	char message[MAX_MESSAGE_LENGTH];
} exceptionData_t;

//TODO all other data structures

#ifdef _MSC_VER
__pragma(pack(pop))
#endif