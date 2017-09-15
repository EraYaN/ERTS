#include "quad.h"
#include "acknowledge_data.h"
#include "mode_switch_data.h"
#include "remote_control_data.h"
#include "telemetry_data.h"

// TODO: Perhaps make the drone (almost) entirely interrupt-driven.
// TODO: Handle connection loss.
// TODO: Tick on interrupt.

Quadrupel::Quadrupel() {
    uart_init();
    gpio_init();
    timers_init();
    adc_init();
    twi_init();
    imu_init(true, 100);
    baro_init();
    spi_flash_init();
    ble_init();
}

void Quadrupel::receive() {
    while (rx_queue.count) {
        comm_buffer[comm_buffer_index] = dequeue(&rx_queue);

        if (!_receiving) {
            _start_sequence = (_start_sequence << 8 | comm_buffer[0]);

            if (_start_sequence == START_SEQUENCE) {
                _receiving = true;
                comm_buffer_index = 0;
            }

        }
        else {
            // Received 5 bytes, check message type.
            if (comm_buffer_index == 4) {
                switch (comm_buffer[4]) {
                    case Unknown:
                    case ModeSwitch:
                    case Acknowledge:
                    case Telemetry:
                    case RemoteControl:
                    case SetControllerRollPID:
                    case SetControllerPitchPID:
                    case SetControllerYawPID:
                    case SetControllerHeightPID:
                    case SetMessageFrequencies:
                    case Reset:
                    case Kill:
                    case Exception:
                        // Message type OK.
                        break;
                    default:
                        // Message type unrecognized.
                        // TODO: Send exception.
                        _receiving = false;
                        break;
                }
            }

            // Received all 20 bytes, process message.
            if (comm_buffer_index == MAX_PACKET_SIZE - 1) {
                if (comm_buffer[MAX_PACKET_SIZE - 1] == END_SEQUENCE) {
                    if (Packet::verify((byte *) comm_buffer)) {
                        auto *packet = new Packet((byte *) comm_buffer);
                        bool handled = handle_packet(packet);
                        // TODO: Send exception if not handled.

                        if (handled && packet->get_data()->get_expects_acknowledgement())
                            acknowledge(packet->get_data()->get_ack_number());

                        delete packet;
                    }
                    else {
                        // TODO: Send exception.
                    }
                }
                else {
                    // TODO: handle this?
                }

                _receiving = false;
            }

            comm_buffer_index++;
        }
    }
}

void Quadrupel::send(Packet *packet) {
    byte *bytes = packet->get_byte_array();

    for (int i = 0; i < MAX_PACKET_SIZE; ++i) {
        uart_put(bytes[i]);
    }

    delete bytes;
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
    uint16_t loop_time = _accum_loop_time / QUADRUPEL_TIMER_PERIOD;

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
            remote_control(data->get_lift(), data->get_roll(), data->get_pitch(), data->get_yaw());
            break;
        }
        case Kill: {
            kill();
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
                case Panic:
                case Calibration: {
                    result =  MODE_SWITCH_OK;
                    break;
                }
                // Requires calibration.
                case Manual:
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

void Quadrupel::remote_control(uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw) {
    if (_mode != Manual)
        return;

	// Equations to get desired lift, roll (rate?), pitch (rate?) and yaw (rate?).
	int oo1, oo2, oo3, oo4;
	int bb;
	bb = b + b;

	oo1 = (lift / b + 2 * pitch / (bb)-yaw / d);
	oo2 = (lift / b - 2 * roll / (bb)+yaw / d);
	oo3 = (lift / b - 2 * pitch / (bb)-yaw / d);
	oo4 = (lift / b + 2 * roll / (bb)+yaw / d);

	// clip ooi as rotors only provide positive thrust
	if (oo1 < 0) oo1 = 0;
	if (oo2 < 0) oo2 = 0;
	if (oo3 < 0) oo3 = 0;
	if (oo4 < 0) oo4 = 0;

	// with ai = oi it follows
	ae[0] = sqrt(oo1);
	ae[1] = sqrt(oo2);
	ae[2] = sqrt(oo3);
	ae[3] = sqrt(oo4);
}

void Quadrupel::update_motors() {
    motor[0] = ae[0];
    motor[1] = ae[1];
    motor[2] = ae[2];
    motor[3] = ae[3];
}

void Quadrupel::control() {

}

