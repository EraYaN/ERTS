#include "quad.h"
#include "acknowledge_data.h"
#include "mode_switch_data.h"
#include "parameter_data.h"
#include "remote_control_data.h"
#include "telemetry_data.h"

#ifdef FAKE_DRIVERS

#include <iostream>

#endif

// TODO: Perhaps make the drone (almost) entirely interrupt-driven.
// TODO: Tick on interrupt.
// TODO: Implement exceptions.
// TODO: Implement flash write/dump.
// TODO: Implement parameter (b/d) change messages.

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
                    case SetControllerRollPID:
                    case SetControllerPitchPID:
                    case SetControllerYawPID:
                    case SetControllerHeightPID:
                    case SetMessageFrequencies:
                    case Parameters:
                    case Reset:
                    case Kill:
                    case Exception:
                        // Message type OK.
                        break;
                    default:
                        // Message type unrecognized.
                        // TODO: Send exception.
                        printf("Exception: Unknown packet type %X.\n", comm_buffer[2]);
                        _receiving = false;
                        comm_buffer_index = 0;
                        break;
                }
            }
            else if (comm_buffer_index == MAX_PACKET_SIZE) {
                if (comm_buffer[MAX_PACKET_SIZE - 1] == END_SEQUENCE) {
                    if (Packet::verify((byte *) comm_buffer)) {
                        printf("Handling packet.\n");
                        auto *packet = new Packet((byte *) comm_buffer);
#ifdef FAKE_DRIVERS
                        std::cout << "RX:\t\t";
                        packet_print(comm_buffer);
#endif
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
                        nrf_gpio_pin_toggle(YELLOW);
                        printf("Exception: Packet did not verify.\n");

                        // TODO: Send exception.
                    }
                    _receiving = false;
                    comm_buffer_index = 0;
                }
                else {
                    nrf_gpio_pin_toggle(GREEN);
                    // TODO: handle this?
                    printf("Exception: Last byte was %X\n", comm_buffer[MAX_PACKET_SIZE - 1]);
                    _receiving = false;
                    comm_buffer_index = 0;
                }
            }
        }
    }
}

void Quadrupel::send(Packet *packet) {
    auto buffer = new uint8_t[MAX_PACKET_SIZE+3];
    packet->to_buffer(&buffer[3]);

#ifdef FAKE_DRIVERS
    std::cout << "TX:\t\t";
    packet_print(buffer);
#endif

    for (int i = 3; i < MAX_PACKET_SIZE+3; ++i) {
        uart_put(buffer[i]);
    }

    //delete[] buffer;
    free(buffer);
    delete packet;
}

void Quadrupel::acknowledge(uint32_t ack_number) {
    auto packet = new Packet(Acknowledge);
    auto data = new AcknowledgeData(ack_number);
    packet->set_data(data);

    send(packet);
}

void Quadrupel::heartbeat() {
    // Calculate loop time.
    auto loop_time = (uint16_t) _accum_loop_time;
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
            //nrf_gpio_pin_toggle(YELLOW);
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
        case Parameters: {
            auto *data = dynamic_cast<ParameterData *>(packet->get_data());
            set_parameters(data->get_b(), data->get_d());
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
    exit = true;
}

void Quadrupel::tick() {
    uint32_t timestamp = get_time_us();
    _new_mode = _mode;
    receive();

    if (_mode != Panic) {
        if (bat_volt < BATTERY_THRESHOLD) {
            // Battery low
            printf("Battery low, entering panic mode.\n");
            set_mode(Panic);
        }

        if ((get_time_us() - last_received) > comm_timeout) {
            // Time-out
            printf("Timed out, entering panic mode.\n");
            set_mode(Panic);
        }
    }

    if (check_timer_flag()) { // The following is executed every 50 ms (20 Hz).
        counter++;

        adc_request_sample();
        read_baro();

        clear_timer_flag();

        if (counter % 20 == 0) {
            nrf_gpio_pin_toggle(BLUE);
            heartbeat();
        }

    }

    if (check_sensor_int_flag()) {
        get_dmp_data();
        control();
    }

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

    if (result == MODE_SWITCH_OK)
        _new_mode = new_mode;

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
    uint16_t setpoint_temp;

    if (_mode == Panic) {
        if (_initial_panic) {
            // Set all motors equal to the current minimum value.
            ae[0] = ae[1] = ae[2] = ae[3] = std::min(std::min(ae[0], ae[1]), std::min(ae[2], ae[3]));
            _initial_panic = false;
        }

        // Linearly decrease motor values and clamp to zero.
        ae[0] = std::max(ae[0] - panic_rate, 0);
        ae[1] = std::max(ae[1] - panic_rate, 0);
        ae[3] = std::max(ae[2] - panic_rate, 0);
        ae[3] = std::max(ae[3] - panic_rate, 0);
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

            setpoint_temp = yaw_p1 * target_state.yaw; // setpoint is angular rate
            yaw = yaw_p2 * (setpoint_temp - sr);
        }
        else {
            lift = 0;
            roll = 0;
            pitch = 0;
            yaw = 0;
        }

        oo1 = lift / (2 * b) + pitch / b - yaw / d;
        oo2 = lift / (2 * b) - roll / b + yaw / d;
        oo3 = lift / (2 * b) - pitch / b - yaw / d;
        oo4 = lift / (2 * b) + roll / b + yaw / d;

        // TODO: Re-introduce square-root if required.
        ae[0] = scale_motor(oo1);
        ae[1] = scale_motor(oo2);
        ae[2] = scale_motor(oo3);
        ae[3] = scale_motor(oo4);
    }
}

void Quadrupel::init_divider() {
    uint32_t min = 0;
    auto max = (uint32_t) (UINT16_MAX / (2 * b) + INT16_MAX / b + INT16_MAX / d);

    divider = (uint16_t) ((max - min) / (MOTOR_MAX - MOTOR_MIN));
}

uint16_t Quadrupel::scale_motor(int32_t value) {
    // Clamp to zero if required.
    value = value < 0 ? 0 : value;

    // Scale
    value = value / divider;

    // Offset
    value += MOTOR_MIN;

    return (uint16_t) value;
}

void Quadrupel::set_parameters(uint16_t b, uint16_t d) {
    this->b = b;
    this->d = d;

    init_divider();
}
