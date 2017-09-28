#include "quad.h"


#ifdef FAKE_DRIVERS

#include <iostream>

#endif

// TODO: Perhaps make the drone (almost) entirely interrupt-driven.
// TODO: Tick on interrupt.
// TODO: Implement exceptions.
// TODO: Implement flash write/dump.

Quadrupel::Quadrupel() {
    // Initialize all drivers.
    uart_init();
    gpio_init();
    timers_init();
    adc_init();
    twi_init();
    imu_init(true, 100);
    baro_init();
    spi_flash_init();
    ble_init();

    init_divider();
    adc_request_sample();
    read_baro();

    last_received = get_time_us();
}

void Quadrupel::receive() {
    while (uart_available()) {
        uint8_t currentByte = uart_get();

        last_two_bytes = last_two_bytes << 8 | currentByte;

        if (!_receiving) {
            if (last_two_bytes == START_SEQUENCE) {
                comm_buffer[0] = ((START_SEQUENCE & 0xFF00) >> 8);
                comm_buffer[1] = (START_SEQUENCE & 0x00FF);
                _receiving = true;
                comm_buffer_index = 2;
                nrf_gpio_pin_set(GREEN);
            }
            else {
                if (currentByte != 0xFE) {
#ifdef FAKE_DRIVERS
                    std::cout << "Looking for packet in: " << std::hex << last_two_bytes << std::endl;
#endif
                    nrf_gpio_pin_clear(GREEN);
                }
            }
        }
        else {
            comm_buffer[comm_buffer_index] = currentByte;
            comm_buffer_index++;
            // Received 5 bytes, check message type.
            if (comm_buffer_index == 3) {
                switch (comm_buffer[2]) {                
                case UnknownPacket:
                case ModeSwitch:
                case Acknowledge:
                case Telemetry:
                case RemoteControl:
                case ActuationParameters:
                case ControllerParameters:
                case MiscParameters:
                case Reset:
                case Kill:
                case Exception:
                    // Message type OK.
                    break;
                default:
                    // Message type unrecognized.
                    exception(BadMessageTypeException,"Bad MessageT ");
#ifdef FAKE_DRIVERS
                    std::cout << "Exception: Unknown packet type " << std::hex << comm_buffer[2] << std::endl;
#endif
                    _receiving = false;
                    comm_buffer_index = 0;
                    break;
                }
            }
            else if (comm_buffer_index == MAX_PACKET_SIZE) {
                if (comm_buffer[MAX_PACKET_SIZE - 1] == END_SEQUENCE) {
                    if (Packet::verify((byte *)comm_buffer)) {
                        auto *packet = new Packet((byte *)comm_buffer);
#ifdef FAKE_DRIVERS
                        std::cout << "RX:\t\t";
                        packet_print(comm_buffer);
#endif
                        nrf_gpio_pin_toggle(YELLOW);
                        bool handled = handle_packet(packet);
                        //                        // TODO: Send exception if not handled.

                        if (handled) {
                            if (packet->get_data()->get_expects_acknowledgement()) {
                                acknowledge(packet->get_data()->get_ack_number());
                            }
                        }

                        delete packet;
                    }
                    else {
#ifdef FAKE_DRIVERS
                        std::cout << "Exception: Packet did not verify." << std::endl;
#endif
                        exception(MessageValidationException, "No Verify    ");
                    }
                    _receiving = false;
                    comm_buffer_index = 0;
                }
                else {
                    // TODO: handle this?
#ifdef FAKE_DRIVERS
                    std::cout << "Exception: Last byte was " << std::hex << comm_buffer[MAX_PACKET_SIZE-1] << std::endl;
#endif
                    _receiving = false;
                    comm_buffer_index = 0;
                }
            }
        }
    }
}

void Quadrupel::send(Packet *packet) {
    auto buffer = new uint8_t[MAX_PACKET_SIZE + 3];

    packet->to_buffer(&buffer[3]);

#ifdef FAKE_DRIVERS
    std::cout << "TX:\t\t";
    packet_print(&buffer[3]);
#endif

    for (int i = 3; i < MAX_PACKET_SIZE + 3; ++i) {
        uart_put(buffer[i]);
    }

    delete[] buffer;
    delete packet;
}

void Quadrupel::acknowledge(uint32_t ack_number) {
    auto packet = new Packet(Acknowledge);
    auto data = new AcknowledgeData(ack_number);
    packet->set_data(data);

    send(packet);
}

void Quadrupel::exception(exceptionType_t type, const char (&message)[MAX_MESSAGE_LENGTH+1]) {
    auto packet = new Packet(Exception);
    auto data = new ExceptionData(type, message);
    packet->set_data(data);

    send(packet);
}

void Quadrupel::heartbeat() {
    // Calculate loop time.
    /*if (_accum_loop_time > UINT16_MAX)
        printf("Loop time over run.\n");*/
    auto loop_time = (uint16_t)_accum_loop_time;

    _accum_loop_time = 0;
    auto packet = new Packet(Telemetry);
    auto data = new TelemetryData(bat_volt, phi, theta, sp, sq, sr, loop_time, _mode);
    packet->set_data(data);

    send(packet);
}

bool Quadrupel::handle_packet(Packet *packet) {
    // TODO: static_casts should suffice as we check for type already and are faster, but are potentially dangerous, replace?

    switch (packet->get_type()) {
    case ModeSwitch: {
        auto *data = dynamic_cast<ModeSwitchData *>(packet->get_data());
        if (set_mode(data->get_new_mode()) != MODE_SWITCH_OK) {
            // TODO: Send exception.
            set_mode(data->get_fallback_mode());
        }
        break;
    }
    case RemoteControl: {
        auto *data = dynamic_cast<RemoteControlData *>(packet->get_data());

        target_state.lift = data->get_lift();
        target_state.roll = data->get_roll();
        target_state.pitch = data->get_pitch();
        target_state.yaw = data->get_yaw();


        last_received = get_time_us();
        break;
    }
    case Kill: {
        kill();
        break;
    }
    case ActuationParameters: {
        auto *data = dynamic_cast<ActuationParameterData *>(packet->get_data());
        set_p_act(data);
        break;
    }
    case ControllerParameters: {
        auto *data = dynamic_cast<ControllerParameterData *>(packet->get_data());
        set_p_ctr(data);
        break;
    }
    case MiscParameters: {
        auto *data = dynamic_cast<MiscParameterData *>(packet->get_data());
        set_p_misc(data);
        break;
    }
    default:
        // Could not handle packet.
        return false;
    }

    return true;
}

void Quadrupel::kill() {
    // Stop motors and assert exit flag.
    ae[0] = 0;
    ae[1] = 0;
    ae[2] = 0;
    ae[3] = 0;
    update_motors();
    nrf_delay_ms(10);
    exit = true;
}

void Quadrupel::busywork() {
    receive();
    if (_mode != Panic && _mode != Safe && _mode != Calibration) {

        if (bat_volt < p_misc.battery_threshold) {
            // Battery low
#ifdef FAKE_DRIVERS
            std::cout << "Battery low, entering panic mode." << std::endl;
#endif
            set_mode(Panic);
        }

        if ((get_time_us() - last_received) > p_misc.comm_timeout) {
            // Time-out
#ifdef FAKE_DRIVERS
            std::cout << "Timed out, entering panic mode." << std::endl;
#endif
            set_mode(Panic);
        }
    }
    if (check_sensor_int_flag()) {
        get_dmp_data();
    }
}

void Quadrupel::tick() {
    uint32_t timestamp = get_time_us();
    //_new_mode = _mode;

    counter++;

    adc_request_sample();
    read_baro();


    if (counter == HB_INTERVAL) {
        counter = 1;
        nrf_gpio_pin_toggle(BLUE);
        if (_mode == Panic) {
            nrf_gpio_pin_toggle(RED);
        }
        heartbeat();
    }


    control();


    update_motors();
    _mode = _new_mode;
    _accum_loop_time += get_time_us() - timestamp;
}

int Quadrupel::set_mode(flightMode_t new_mode) {
    if (_mode == new_mode) return MODE_SWITCH_OK;

    int result;

    switch (_mode) {
        // Transitions from safe mode.
    case Safe: {
        switch (new_mode) {
            // Always OK.
        case Panic: {
            _initial_panic = true;
        }
        case Calibration:
        case Manual: {
            result = MODE_SWITCH_OK;
            break;
        }
                     // Requires calibration.
        case YawControl:
        case FullControl:
        case Raw:
        case Height:
        case Wireless: {
            if (_is_calibrated) {
                result = MODE_SWITCH_OK;
            }
            else {
                result = MODE_SWITCH_UNSUPPORTED;
            }
            break;
        }
                       // Unknown transition.
        default: {
            result = MODE_SWITCH_UNSUPPORTED;
            break;
        }

        }
        break;
    }
               // Never transition from panic mode.
    case Panic: {
        result = MODE_SWITCH_UNSUPPORTED;
        break;
    }
                // Other modes can only transition to safe or panic mode.
    case Calibration:
    case Manual:
    case YawControl:
    case FullControl:
    case Raw:
    case Height:
    case Wireless: {
        switch (new_mode) {
        case Safe:
        case Panic: {
            result = MODE_SWITCH_OK;
            break;
        }
        default: {
            result = MODE_SWITCH_UNSUPPORTED;
            break;
        }
        }
        break;
    }
    default: {
        result = MODE_SWITCH_UNSUPPORTED;
        break;
    }
    }

    if (result == MODE_SWITCH_OK) {
#ifdef FAKE_DRIVERS
        std::cout << "Set mode " << _mode << " -> " << new_mode << std::endl;
#endif
        _mode = new_mode;
    }

    return result;
}

void Quadrupel::update_motors() {
    motor[0] = ae[0];
    motor[1] = ae[1];
    motor[2] = ae[2];
    motor[3] = ae[3];
}

void Quadrupel::control() {
    // Equations to get desired lift, roll rate, pitch rate and yaw rate.
    int32_t oo1, oo2, oo3, oo4;
    uint16_t lift;
    int16_t roll, pitch, yaw;
    // uint16_t setpoint_temp;

    if (_mode == Panic) {
        if (_initial_panic) {
            // Set all motors equal to the current minimum value.
            ae[0] = ae[1] = ae[2] = ae[3] = std::min(std::min(ae[0], ae[1]), std::min(ae[2], ae[3]));
            _initial_panic = false;
        }

        // Linearly decrease motor values and clamp to zero.
        ae[0] = std::max(ae[0] - p_misc.panic_decrement, 0);
        ae[1] = std::max(ae[1] - p_misc.panic_decrement, 0);
        ae[3] = std::max(ae[2] - p_misc.panic_decrement, 0);
        ae[3] = std::max(ae[3] - p_misc.panic_decrement, 0);
    }
    else {
        if (_mode == Manual) {
            lift = target_state.lift;
            roll = target_state.roll;
            pitch = target_state.pitch;
            yaw = target_state.yaw;
        }
        else if (_mode == YawControl) {
            lift = target_state.lift;
            roll = target_state.roll;
            pitch = target_state.pitch;

            //setpoint_temp = yaw_p1.yaw_p1 * target_state.yaw;
            yaw = p_ctr.p_yaw * (target_state.yaw - sr);
        }
        else {
            lift = 0;
            roll = 0;
            pitch = 0;
            yaw = 0;
        }

        oo1 = lift / (2 * p_act.rate_pitch_roll_lift) + pitch / p_act.rate_pitch_roll_lift - yaw / p_act.rate_yaw;
        oo2 = lift / (2 * p_act.rate_pitch_roll_lift) - roll / p_act.rate_pitch_roll_lift + yaw / p_act.rate_yaw;
        oo3 = lift / (2 * p_act.rate_pitch_roll_lift) - pitch / p_act.rate_pitch_roll_lift - yaw / p_act.rate_yaw;
        oo4 = lift / (2 * p_act.rate_pitch_roll_lift) + roll / p_act.rate_pitch_roll_lift + yaw / p_act.rate_yaw;

        // TODO: Re-introduce square-root if required.
        ae[0] = scale_motor(oo1);
        ae[1] = scale_motor(oo2);
        ae[2] = scale_motor(oo3);
        ae[3] = scale_motor(oo4);
    }
}

void Quadrupel::init_divider() {
    uint32_t min = 0;

    auto max = (uint32_t)(UINT16_MAX / (2 * p_act.rate_pitch_roll_lift) + INT16_MAX / p_act.rate_pitch_roll_lift +
        INT16_MAX / p_act.rate_yaw);

    p_act.divider = (uint16_t)((max - min) / (p_act.motor_max - p_act.motor_min));
}

uint16_t Quadrupel::scale_motor(int32_t value) {
    // Clamp to zero if required.
    value = value < 0 ? 0 : value;

    // Scale
    value = value / p_act.divider;

    // Offset
    value += p_act.motor_min;

    return (uint16_t)value;
}

void Quadrupel::set_p_act(ActuationParameterData *data) {
    p_act.rate_pitch_roll_lift = data->get_rate_pitch_roll_lift();
    p_act.rate_yaw = data->get_rate_yaw();
    p_act.motor_min = data->get_motor_min();
    p_act.motor_max = data->get_motor_max();

    init_divider();
}

void Quadrupel::set_p_ctr(ControllerParameterData *data) {
    p_ctr.p_yaw = data->get_p_yaw();
    p_ctr.p_height = data->get_p_height();
    p_ctr.p1_pitch_roll = data->get_p1_pitch_roll();
    p_ctr.p2_pitch_roll = data->get_p2_pitch_roll();
}

void Quadrupel::set_p_misc(MiscParameterData *data) {
    p_misc.panic_decrement = data->get_panic_decrement();
    p_misc.rc_interval = data->get_rc_interval();
    p_misc.log_divider = data->get_log_divider();
    p_misc.battery_threshold = data->get_battery_threshold();
    p_misc.target_loop_time = data->get_target_loop_time();
    p_misc.comm_timeout = 2u * p_misc.target_loop_time;
}
