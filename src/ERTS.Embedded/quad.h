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
#include "quad_config.h"

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

#define FUNC_LOGGING    0b0000'0001
#define FUNC_TELEMETRY  0b0000'0010
#define FUNC_RAW        0b0000'0100
#define FUNC_FLASH_DUMP 0b0000'1000
#define FUNC_WIRELESS   0b0001'0000

typedef struct {
    uint16_t lift = 0;
    int16_t pressure = 0;
    int16_t yaw = 0;
    int16_t pitch = 0;
    int16_t roll = 0;
} quad_state_t;

typedef struct {
    int32_t pitch = 0;
    int32_t roll = 0;
    int32_t steps = 0;
} calibration_state_t;

typedef struct {
    uint16_t p_yaw = 50;
    uint16_t p_height = 5;
    uint16_t p1_pitch_roll = 50, p2_pitch_roll = 50;
} controller_params_t;

class Quadrupel {
    // Parameter settings
    controller_params_t p_ctr;

    uint16_t func_state = (FUNC_TELEMETRY); // Use the FUNC_* flags to set this.

    // State
    bool _initial_panic = false;
    flightMode_t _mode = Safe;
    bool _is_calibrated = false;
    uint32_t _accum_loop_time;
    quad_state_t target_state, current_state, calibration_offsets;
    calibration_state_t calibration_state;

    // Timing Counters
    uint32_t counter_hb = 0;
    uint32_t counter_led = 0;
    //uint32_t counter_fd = 0;
    uint32_t counter_dmp = 0;    

    // Comm    
    uint16_t last_two_bytes = 0;
    bool _receiving = false;
    uint8_t comm_buffer[MAX_PACKET_SIZE];
    uint8_t comm_buffer_index = 0;
    uint32_t last_received;

    uint32_t bytes = 0;
    uint32_t packets = 0;
    bool status_printed = false;

    // Motors
    uint16_t motor_divider = 1;

    //Flash dump    
    uint16_t telemetry_divider_old = 0;
    uint16_t flash_dump_divider = 10;
    uint16_t flash_sequence_number = 0;

    // Private methods
    void receive();

    void send(Packet *packet);

    void acknowledge(uint32_t ack_number);

    void exception(exceptionType_t Type, const char* message);

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

    void control_fast();

    void calibrate(bool finalize = false);

    //void set_p_act(ActuationParameterData *data);

    void set_p_ctr(ControllerParameterData *data);

    void set_p_misc(MiscParameterData *data);

    void start_flash_dump();

    void stop_flash_dump();

    void send_flash_dump_data();
};
