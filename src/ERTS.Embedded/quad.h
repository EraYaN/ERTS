#pragma once

#include <cstdint>
#include <algorithm>

extern "C"
{
#include "driver.h"
}

#include "packet.h"
#include "packet_datastructures.h"

#define MODE_SWITCH_OK 0
#define MODE_SWITCH_UNSUPPORTED 1

typedef struct {
    uint16_t  lift;
    int16_t  yaw;
    int16_t  pitch;
    int16_t  roll;
} quad_state_t;


class Quadrupel {
    const uint16_t MOTOR_MIN = 0;
    const uint16_t MOTOR_MAX = 1500;

    uint16_t b = 1;
    uint16_t d = 1;
    uint16_t divider = 1;

	uint16_t yaw_p1 = 5;
    uint16_t yaw_p2 = 5;

    uint16_t panic_rate = 100;
    bool _initial_panic = false;

    flightMode_t _mode = Safe;
    flightMode_t _new_mode = Safe;
    bool _is_calibrated;
    uint32_t _accum_loop_time;
    uint32_t counter = 0;
	uint16_t lastTwoBytes = 0;
    bool _receiving = false;
    uint16_t _start_sequence = 0;
    uint8_t comm_buffer[MAX_PACKET_SIZE];
    uint8_t comm_buffer_index = 0;

    quad_state_t target_state, current_state;

    void receive();

    void send(Packet *packet);

    void acknowledge(uint32_t ack_number);

    void heartbeat();

    bool handle_packet(Packet *packet);

    void kill();

    void init_divider();

    inline uint16_t scale_motor(int32_t value);

public:
    int16_t motor[4], ae[4];
    bool exit = false;

    Quadrupel();

    void tick();

    flightMode_t get_mode() { return _mode; }

	int set_mode(flightMode_t new_mode);

    void update_motors();

    void control();

    void set_parameters(uint16_t b, uint16_t d);
};
