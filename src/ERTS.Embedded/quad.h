#pragma once

#include <cstdint>

extern "C"
{
#include "driver.h"
}

#include "packet.h"
#include "packet_datastructures.h"

class Quadrupel {
    flightMode_t _mode = Safe;
    flightMode_t _new_mode = Safe;
    uint32_t counter = 0;
    char comm_buffer[20];
    uint8_t comm_buffer_index = 0;

    void receive();

    void send(Packet *packet);

    void acknowledge();

    void heartbeat();

    bool handle_packet(Packet *packet);

    void remote_control(uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw);

public:
    int16_t motor[4], ae[4];
    bool exit = false;

    Quadrupel();

    void tick();

    flightMode_t get_mode() { return _mode; }

    void set_mode(flightMode_t new_mode) { _new_mode = new_mode; }

    void update_motors();

    void control();
};
