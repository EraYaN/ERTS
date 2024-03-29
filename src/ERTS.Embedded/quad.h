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
    uint16_t p_yaw = 1;
    uint16_t p_height = 1;
    uint16_t p1_pitch_roll = 1, p2_pitch_roll = 1;
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
    
    //Flash dump    
    uint16_t telemetry_divider_old = 0;
    uint16_t flash_dump_divider = 10;
    uint16_t flash_sequence_number = 0;

#pragma region Private methods
    //IN4073 Authoring claimed by: Erwin
    void receive();
    //IN4073 Authoring claimed by: Erwin
    void send(Packet *packet);
    //IN4073 Authoring claimed by: Erwin
    void acknowledge(uint32_t ack_number);
    //IN4073 Authoring claimed by: Erwin
    void exception(exceptionType_t Type, const char* message);
    //IN4073 Authoring claimed by: Robin
    void heartbeat();
    //IN4073 Authoring claimed by: Robin
    bool handle_packet(Packet *packet);
    //IN4073 Authoring claimed by: Erwin
    void kill();
    //IN4073 Authoring claimed by: Robin
    inline uint16_t scale_motor(int32_t value);
    //IN4073 Authoring claimed by: Robin
    void set_current_state();
#pragma endregion
public:
    bool exit = false;
#pragma region Public Methods
    //IN4073 Authoring claimed by: Robin
    Quadrupel();
    //IN4073 Authoring claimed by: Erwin
    void busywork();
    //IN4073 Authoring claimed by: Erwin
    void tick();
    //IN4073 Authoring claimed by: Robin
    flightMode_t get_mode() { return _mode; }
    //IN4073 Authoring claimed by: Robin
    int set_mode(flightMode_t new_mode);
    //IN4073 Authoring claimed by: Robin
    void update_motors();
    //IN4073 Authoring claimed by: Robin
    void control();
    //IN4073 Authoring claimed by: Casper
    void mix(uint32_t lift, int32_t roll, int32_t pitch, int32_t yaw);
    //IN4073 Authoring claimed by: Erwin
    void control_fast();
    //IN4073 Authoring claimed by: Robin
    void calibrate(bool finalize = false);
    //IN4073 Authoring claimed by: Erwin
    void set_p_ctr(ControllerParameterData *data);
    //IN4073 Authoring claimed by: Erwin
    void set_p_misc(MiscParameterData *data);
    //IN4073 Authoring claimed by: Casper
    void start_flash_dump();
    //IN4073 Authoring claimed by: Casper
    void stop_flash_dump();
    //IN4073 Authoring claimed by: Casper
    void send_flash_dump_data();
#pragma endregion
};
