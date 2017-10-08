#pragma once

#include <cstdint>
#include <stdint.h>
#include <algorithm>
#include "actuation_parameter_data.h"
#include "controller_parameter_data.h"
#include "misc_parameter_data.h"
#include "acknowledge_data.h"
#include "exception_data.h"
#include "mode_switch_data.h"
#include "remote_control_data.h"
#include "telemetry_data.h"
#include "flashdump_data.h"

extern "C"
{
#include "driver.h"
}

#include "packet.h"
#include "packet_datastructures.h"
#include "flash_wrapper.h"

#define MODE_SWITCH_OK 0
#define MODE_SWITCH_UNSUPPORTED 1
#define MODE_SWITCH_NOT_ALLOWED 2

typedef struct {
    uint16_t lift = 0;
    int16_t yaw = 0;
    int16_t pitch = 0;
    int16_t roll = 0;
} quad_state_t;


typedef struct {
    uint16_t rate_yaw = 1;
    uint16_t rate_pitch_roll_lift = 1;
    uint16_t divider = 1;
    uint16_t motor_min = 500, motor_max = 1000;
} actuator_params_t;

typedef struct {
    uint16_t p_yaw = 5;
    uint16_t p_height = 5;
    uint16_t p1_pitch_roll = 5, p2_pitch_roll = 5;
} controller_params_t;

typedef struct {
    uint16_t panic_decrement = 1;
    uint16_t rc_interval = 50;
    uint16_t log_divider = 0;
	uint16_t telemetry_divider = 10;
    uint16_t battery_threshold = 400;
    uint16_t target_loop_time = 20000;
    uint32_t comm_timeout = 200000;
} misc_params_t;

class Quadrupel {
    // Parameter settings
    actuator_params_t p_act;
    controller_params_t p_ctr;
    misc_params_t p_misc;

    // State
    bool _initial_panic = false;
    flightMode_t _mode = Safe;
    flightMode_t _new_mode = Safe;
    bool _is_calibrated;
    uint32_t _accum_loop_time;
    quad_state_t target_state, current_state, calibration_offsets;
    uint32_t calibration_steps = 0;

    // Comm
    uint32_t counterHB = 1;
	uint32_t counterLED = 1;
    uint16_t last_two_bytes = 0;
    bool _receiving = false;
    uint8_t comm_buffer[MAX_PACKET_SIZE];
    uint8_t comm_buffer_index = 0;
    uint32_t last_received;

    uint32_t bytes = 0;
    uint32_t packets = 0;
    bool status_printed = false;

    // Private methods
    void receive();

    void send(Packet *packet);

    void acknowledge(uint32_t ack_number);

    void exception(exceptionType_t Type, const char (&message)[MAX_MESSAGE_LENGTH+1]);

    void heartbeat();

    bool handle_packet(Packet *packet);

    void kill();

    void init_divider();

    inline uint16_t scale_motor(int32_t value);

    void set_current_state();

public:
    bool exit = false;

    Quadrupel();

    void busywork();

    void tick();

    flightMode_t get_mode() { return _mode; }

    int set_mode(flightMode_t new_mode);

    void update_motors();

    void control();

    void calibrate(bool finalize = false);

    void set_p_act(ActuationParameterData *data);

    void set_p_ctr(ControllerParameterData *data);

    void set_p_misc(MiscParameterData *data);

    void dumpflash();
};
