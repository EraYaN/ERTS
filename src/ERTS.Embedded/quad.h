#pragma once

#include <cstdint>

extern "C"
{
#include "driver.h"
}

#include "packet.h"
#include "packet_datastructures.h"

#define MODE_SWITCH_OK 0
#define MODE_SWITCH_UNSUPPORTED 1

class Quadrupel {
    flightMode_t _mode = Safe;
    flightMode_t _new_mode = Safe;
    bool _is_calibrated;
    uint32_t _accum_loop_time;
    uint32_t counter = 0;
    bool _receiving = false;
    uint16_t _start_sequence = 0;
    char comm_buffer[MAX_PACKET_SIZE];
    uint8_t comm_buffer_index = 0;
    // TODO: Determine decent values.
    const double b = 1.00;
    const double d = 1.00;

    void receive();

    void send(Packet *packet);

    void acknowledge(uint32_t ack_number);

    void heartbeat();

    bool handle_packet(Packet *packet);

    void kill();

    void remote_control(uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw);

public:
    int16_t motor[4], ae[4];
    bool exit = false;

    Quadrupel();

    void tick();

    flightMode_t get_mode() { return _mode; }

	int set_mode(flightMode_t new_mode);

    void update_motors();

    void control();
};
