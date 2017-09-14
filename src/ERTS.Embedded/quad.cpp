#include "quad.h"
#include "mode_switch_data.h"
#include "remote_control_data.h"
#include "telemetry_data.h"

// TODO: Implement kill message handling.
// TODO: Make this more like a state-machine.
// TODO: Add sensor read-outs.
// TODO: Perhaps make the drone (almost) entirely interrupt-driven.

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
        comm_buffer_index++;

        if (comm_buffer_index == 2) { // Received two bytes, check starting sequence
            if (comm_buffer[0] != ((START_SEQUENCE & 0xFF00) >> 8) || comm_buffer[1] != ((START_SEQUENCE & 0x00FF))) {
                // Invalid first two bytes, reset.
                // TODO: Maybe we should decrement in stead of reset? Next byte could be first byte of new start sequence.
                comm_buffer_index = 0;
            }
        }

        if (comm_buffer_index == 3) { // Received three bytes, check message type
            switch (comm_buffer[2]) {
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
                    break;
                default:
                    comm_buffer_index = 0;
            }
        }

        if (comm_buffer_index == MAX_PACKET_SIZE) {
            comm_buffer_index = 0;

            if (comm_buffer[MAX_PACKET_SIZE - 1] == END_SEQUENCE) {
                auto *packet = new Packet((byte *) comm_buffer);
                // TODO: Check CRC and perform any other action that does not involve the packet data itself.

                bool handled = handle_packet(packet);

                if (handled && packet->get_data()->get_expects_acknowledgement())
                    acknowledge();

                delete packet;
            }
            else {
                // TODO: handle this?
            }
        }

    }
}

void Quadrupel::send(Packet *packet) {
    byte *bytes = packet->get_byte_array();

    // TODO: Make sure the following is atomic.
    for (int i = 0; i < MAX_PACKET_SIZE; ++i) {
        uart_put(bytes[i]);
    }

    delete bytes;
    delete packet;
}

void Quadrupel::acknowledge() {
    // TODO: Implement this.
}

void Quadrupel::heartbeat() {
    auto packet = new Packet();
    // TODO: Add sensor read-outs.
    auto data = new TelemetryData(bat_volt, 0, 0, 0, 0, 0, 0, _mode);
    packet->set_data(data);

    send(packet);
}

bool Quadrupel::handle_packet(Packet *packet) {
    // TODO: static_casts should suffice as we check for type already and are faster, but are potentially dangerous, replace?
    switch (packet->get_type()) {
        case ModeSwitch: {
            // TODO: make use of fallback mode.
            flightMode_t new_mode = dynamic_cast<ModeSwitchData *>(packet->get_data())->get_new_mode();
            set_mode(new_mode);
            break;
        }
        case RemoteControl: {
            auto *data = dynamic_cast<RemoteControlData *>(packet->get_data());
            remote_control(data->get_lift(), data->get_roll(), data->get_pitch(), data->get_yaw());
            break;
        }
        default:
            // Could not handle packet.
            return false;
    }

    return true;
}

void Quadrupel::tick() {
    _new_mode = _mode;
    receive();

    if (check_timer_flag()) { // The following is executed every 50 ms (20 Hz).
        counter++;

        if (counter % 20 == 0)
            nrf_gpio_pin_toggle(BLUE);

        adc_request_sample();
        read_baro();

//        printf("%10ld | ", get_time_us());
//        printf("%3d %3d %3d %3d | ", ae[0], ae[1], ae[2], ae[3]);
//        printf("%6d %6d %6d | ", phi, theta, psi);
//        printf("%6d %6d %6d | ", sp, sq, sr);
//        printf("%4d | %4ld | %6ld \n", bat_volt, temperature, pressure);

        clear_timer_flag();

        heartbeat();
    }

    if (check_sensor_int_flag()) {
        get_dmp_data();
        control();
    }

    update_motors();
    _mode = _new_mode;
}

void Quadrupel::set_mode(flightMode_t new_mode) {
    _new_mode = new_mode;
}

void Quadrupel::remote_control(uint16_t lift, int16_t roll, int16_t pitch, int16_t yaw) {
    if (_mode != Manual)
        return;

    // TODO: Implement equations for desired lift, roll (rate?), pitch (rate?) and yaw (rate?).
}

void Quadrupel::update_motors() {
    motor[0] = ae[0];
    motor[1] = ae[1];
    motor[2] = ae[2];
    motor[3] = ae[3];
}

void Quadrupel::control() {

}

