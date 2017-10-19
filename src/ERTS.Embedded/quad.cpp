#include "quad.h"
//#include "drivers/flash_wrapper.h"

#ifdef FAKE_DRIVERS

#include <iostream>
#include <driver.h>

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
    imu_init(true, 100); //TODO Do we need to do anything to get the acc and gyro measurements at a 1000kHz (switchable)
    baro_init();
    spi_flash_init();
    //ble_init();

    init_divider();
    adc_request_sample();
    read_baro();

    last_received = get_time_us();
}

void Quadrupel::receive() {
    while (uart_available() && !check_timer_flag() && !check_sensor_int_flag()) {
        uint8_t currentByte = uart_get();
        bytes++;

        //printf("Got byte: 0x%X\n",currentByte);

        last_two_bytes = last_two_bytes << 8 | currentByte;

        if (!_receiving) {
            if (last_two_bytes == START_SEQUENCE) {
                comm_buffer[0] = ((START_SEQUENCE & 0xFF00) >> 8);
                comm_buffer[1] = (START_SEQUENCE & 0x00FF);
                _receiving = true;
                comm_buffer_index = 2;
                //nrf_gpio_pin_set(GREEN);
            }
            else {
                if (currentByte != 0xFE) {
#ifdef FAKE_DRIVERS
                    std::cout << "Looking for packet in: " << std::hex << last_two_bytes << std::endl;
#endif
                    //nrf_gpio_pin_clear(GREEN);
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
                    //case ActuationParameters:
                case ControllerParameters:
                case MiscParameters:
                case Reset:
                case Kill:
                case Exception:
                    // Message type OK.
                    break;
                default:
                    // Message type unrecognized.
                    exception(BadMessageTypeException, "Bad MessageType");
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
                        //send(packet);
                        bool handled = handle_packet(packet);
                        if (handled) {
                            //nrf_gpio_pin_toggle(YELLOW);
                            packets++;
                        }

                        // TODO: Send exception if not handled.


                        if (packet->get_data()->get_expects_acknowledgement()) {
                            acknowledge(packet->get_data()->get_ack_number());
                        }


                        delete packet;
                    }
                    else {
#ifdef FAKE_DRIVERS
                        std::cout << "Exception: Packet did not verify." << std::endl;
#endif
                        exception(MessageValidationException, "No Verify");
                    }
                    _receiving = false;
                    comm_buffer_index = 0;
                }
                else {
                    // TODO: handle this?
#ifdef FAKE_DRIVERS
                    std::cout << "Exception: Last byte was " << std::hex << comm_buffer[MAX_PACKET_SIZE - 1]
                        << std::endl;
#endif
                    _receiving = false;
                    comm_buffer_index = 0;
                }

            }
        }
        /*if(packets%10==0 || packets>=999){
            if(!status_printed || currentByte == 0xFF){
                status_printed = true;
                printf("#RX Packets: %ld; RX Bytes: %ld; RX BuffIdx: %d\n",packets,bytes,comm_buffer_index);
            }
        } else if(bytes%10==1 || packets >= 999){
            status_printed = false;
        }*/
    }

    if (tx_queue.count < 4) {
        nrf_gpio_pin_set(GREEN);
    }
    else {
        nrf_gpio_pin_clear(GREEN);
    }
    if (rx_queue.count < 4) {
        nrf_gpio_pin_set(YELLOW);
    }
    else {
        nrf_gpio_pin_clear(YELLOW);
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
}

void Quadrupel::acknowledge(uint32_t ack_number) {
    auto packet = new Packet(Acknowledge);
    auto data = new AcknowledgeData(ack_number);
    packet->set_data(data);

    send(packet);
    delete packet;
}

void Quadrupel::exception(exceptionType_t type, const char* message) {
    auto packet = new Packet(Exception);
    auto data = new ExceptionData(type, message);
    packet->set_data(data);

    send(packet);

    delete packet;
}

void Quadrupel::heartbeat() {
    // Calculate loop time.
    /*if (_accum_loop_time > UINT16_MAX)
        printf("Loop time over run.\n");*/
    auto loop_time = (uint16_t)_accum_loop_time;

    _accum_loop_time = 0;
    auto packet = new Packet(Telemetry);
    auto data = new TelemetryData(bat_volt, current_state.roll, current_state.pitch, current_state.yaw,
        current_state.pressure, func_state, loop_time, _mode);
    packet->set_data(data);

    send(packet);

    delete packet;
}

bool Quadrupel::handle_packet(Packet *packet) {
    // TODO: static_casts should suffice as we check for type already and are faster, but are potentially dangerous, replace?

    switch (packet->get_type()) {
    case ModeSwitch: {
        auto *data = dynamic_cast<ModeSwitchData *>(packet->get_data());
        if (set_mode(data->get_new_mode()) != MODE_SWITCH_OK) {
            // TODO: Send exception.
            exception(InvalidModeException, "Bad Main Mode");
            if (data->get_fallback_mode() != None) {
                if (set_mode(data->get_fallback_mode()) != MODE_SWITCH_OK) {
                    exception(InvalidModeException, "Bad Fallback.");
                    return false;
                }
            }
            else {
                return false;
            }
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

        /**/


    }
    if (check_sensor_int_flag()) {
        get_dmp_data();
        set_current_state();
    }
}

void Quadrupel::tick() {
    uint32_t timestamp = get_time_us();


    if (_mode == DumpFlash && (func_state & FUNC_FLASH_DUMP)) {
        // Send all of the flash to PC over UART
        //if (counter_fd == DIVIDER_FLASH_DUMP) {
        if (tx_queue.count < TX_QUEUE_FLASH_DUMP_LIMIT) {
            //counter_fd = 0;
            send_flash_dump_data(); // non blocking function, sends one message   
        }
        //}
        //else {
        //    counter_fd++;
        //}
        return;
    }

    if (counter_dmp == DIVIDER_DMP_MODE) {

        counter_dmp = 0;

        if (counter_led == DIVIDER_LED) {
            counter_led = 0;
            nrf_gpio_pin_toggle(BLUE);
            if (_mode == Panic) {
                nrf_gpio_pin_toggle(RED);
            }
        }
        else {
            counter_led++;
        }

        read_baro();

        if (_mode != Panic && _mode != Safe && _mode != Calibration && _mode != DumpFlash) {
            if ((get_time_us() - last_received) > (TIMEOUT_COMM >> 1)) {
                nrf_gpio_pin_clear(RED);
            }
            else {
                nrf_gpio_pin_set(RED);
            }
            if ((get_time_us() - last_received) > TIMEOUT_COMM) {
#ifdef FAKE_DRIVERS
                std::cout << "Timed out, entering panic mode." << std::endl;
#endif
                exception(UnknownException, "RC Timeout...");
                set_mode(Panic);

            }

        }

        if (_mode == Height && target_state.lift != current_state.lift) {
            // Revert to full control upon throttle movement.
            set_mode(FullControl);
        }


        if (counter_hb == DIVIDER_TELEMETRY) {
            counter_hb = 0;
            adc_request_sample(); // Really only needed once per heartbeat
            if (_mode != Panic && _mode != Safe && _mode != DumpFlash) {
                if (bat_volt < BATTERY_THRESHOLD) {
#ifdef FAKE_DRIVERS
                    std::cout << "Battery low, entering panic mode." << std::endl;
#endif
                    exception(UnknownException, "Battery Low..");
                    set_mode(Panic);

                }
            }
            if (func_state & FUNC_TELEMETRY) {
                heartbeat();
            }
        }
        else {
            counter_hb++;
        }

        if (_mode == Calibration) {
            if (calibration_state.steps > 500) {
                set_mode(Safe);
            }
            else {
                calibrate();
            }
        }
        else {
            control();
            update_motors();
        }

    }
    else {
        counter_dmp++;
    }

    if (func_state & FUNC_RAW) {
        //TODO get_raw_sensor_data(); //Read raw sensor data
        control_fast(); //Inner kalman filter loop
    }

    _accum_loop_time += get_time_us() - timestamp;
}

int Quadrupel::set_mode(flightMode_t new_mode) {
    if (_mode == new_mode) return MODE_SWITCH_OK;

    int result;

    switch (_mode) {
        // Transitions from safe mode.
    case Safe: {
        // Disallow mode switch when any of the inputs is non-zero.
        if (new_mode != Panic
            && (target_state.lift != 0
                || target_state.roll != 0
                || target_state.pitch != 0
                || target_state.yaw != 0
                )) {

            result = MODE_SWITCH_NOT_ALLOWED;
            break;
        }

        switch (new_mode) {
        case Panic: {
            _initial_panic = true;
        }

        case DumpFlash: {
            start_flash_dump();
        }
        case Calibration:
        case Manual: {
            result = MODE_SWITCH_OK;
            break;
        }

        case YawControl:
        case FullControl:
        case Raw:
        case Wireless: {
            if (_is_calibrated) {
                result = MODE_SWITCH_OK;
            }
            else {
                result = MODE_SWITCH_UNSUPPORTED;
            }
            break;
        }

        case Height: {
            result = MODE_SWITCH_NOT_ALLOWED;
            break;
        }

        default: {
            result = MODE_SWITCH_UNSUPPORTED;
            break;
        }

        }
        break;
    }
    case Panic: {
        switch (new_mode) {
            // Always OK.
        case Safe: {
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
    case Calibration: {
        switch (new_mode) {
        case Safe:
            calibrate(true);
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
    case FullControl: {
        switch (new_mode) {
        case Height: {
            // Set setpoint and pin lift.
            target_state.pressure = current_state.pressure;
            current_state.lift = target_state.lift;
        }
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
    case Manual:
    case YawControl:
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
    case DumpFlash: {
        switch (new_mode) {
        case Safe: {
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
    uint32_t lift;
    int32_t roll, pitch, yaw, p_s, q_s;

    if (func_state & FUNC_LOGGING) {
        flash_write_remote(get_time_us(), _mode, target_state.lift, target_state.roll, target_state.pitch,
            target_state.yaw);
    }

    if (_mode == Panic) {
        if (_initial_panic) {
            // Set all motors equal to the current average value.
            ae[0] = ae[1] = ae[2] = ae[3] = (ae[0] + ae[1] + ae[2] + ae[3]) / 4;
            _initial_panic = false;
        }

        if (ae[0] != 0) {
            // Linearly decrease motor values and clamp to zero.
            ae[0] = ae[1] = ae[2] = ae[3] = std::max(ae[0] - PANIC_DECREMENT, 0);
        }
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

            yaw = p_ctr.p_yaw * (target_state.yaw - current_state.yaw);
        }
        else if (_mode == FullControl || _mode == Height) {

            p_s = p_ctr.p1_pitch_roll * (target_state.roll - current_state.roll);
            roll = p_ctr.p2_pitch_roll * (p_s - sp);

            q_s = p_ctr.p1_pitch_roll * (target_state.pitch - current_state.pitch);
            pitch = -p_ctr.p2_pitch_roll * (q_s - sq);

            yaw = p_ctr.p_yaw * (target_state.yaw - current_state.yaw);

            if (_mode == Height)
                lift = p_ctr.p_height * (target_state.pressure - current_state.pressure);
            else
                lift = target_state.lift;
        }
        else {
            lift = 0;
            roll = 0;
            pitch = 0;
            yaw = 0;
        }

        oo1 = lift / RATE_LIFT + pitch / RATE_PITCH_ROLL - yaw / RATE_YAW;
        oo2 = lift / RATE_LIFT - roll / RATE_PITCH_ROLL + yaw / RATE_YAW;
        oo3 = lift / RATE_LIFT - pitch / RATE_PITCH_ROLL - yaw / RATE_YAW;
        oo4 = lift / RATE_LIFT + roll / RATE_PITCH_ROLL + yaw / RATE_YAW;
        /*oo1 = (lift / p_act.rate_pitch_roll_lift + 2 * pitch / (2 * p_act.rate_pitch_roll_lift) - yaw / p_act.rate_yaw);
        oo2 = (lift / p_act.rate_pitch_roll_lift - 2 * roll / (2 * p_act.rate_pitch_roll_lift) + yaw / p_act.rate_yaw);
        oo3 = (lift / p_act.rate_pitch_roll_lift - 2 * pitch / (2 * p_act.rate_pitch_roll_lift) - yaw / p_act.rate_yaw);
        oo4 = (lift / p_act.rate_pitch_roll_lift + 2 * roll / (2 * p_act.rate_pitch_roll_lift) + yaw / p_act.rate_yaw);*/

        if (_mode == Safe || _mode == Calibration) {
            ae[0] = ae[1] = ae[2] = ae[3] = 0;
        }
        else {
            // TODO: Re-introduce square-root if required.
            ae[0] = scale_motor(oo1);
            ae[1] = scale_motor(oo2);
            ae[2] = scale_motor(oo3);
            ae[3] = scale_motor(oo4);
        }
    }
}

void Quadrupel::control_fast() {
    //P2PHI depends on loop freq -> compute/measure
    //C1 small: believe sphi; C1 large: believe sp
    //C2 large: (typically > 1000*C1): slow drift

    //p = sp - b; //estimate real p  

    //phi = phi + p * P2PHI; // predict phi

    //e = phi - sphi;// compare to measured phi

    //phi = phi – e / C1;// correct phi to some extent
    //b = b + (e / P2PHI) / C2;// adjust bias term
    // TODO http://www.st.ewi.tudelft.nl/~koen/in4073/lect06-Integration.pdf slide 11
}

void Quadrupel::calibrate(bool finalize) {
    calibration_state.roll += phi;
    calibration_state.pitch += theta;
    calibration_state.steps++;

    if (finalize) {
        _is_calibrated = true;
        calibration_offsets.roll = (int16_t)(calibration_state.roll / calibration_state.steps);
        calibration_offsets.pitch = (int16_t)(calibration_state.pitch / calibration_state.steps);

        // Reset.
        calibration_state.steps = 0;
        calibration_state.roll = 0;
        calibration_state.pitch = 0;
    }
}

void Quadrupel::init_divider() {
    /*auto max = (int32_t);*/
    //0xDD + 0x3F + 0x7F+
    motor_divider = (uint32_t)(DIVIDER_MAX / (MOTOR_MAX - MOTOR_MIN));
}

uint16_t Quadrupel::scale_motor(int32_t value) {
    // Clamp to zero if required.
    value = value < 0 ? 0 : value;
    value = value > DIVIDER_MAX ? DIVIDER_MAX : value;

    // Scale
    value = value / motor_divider;

    // Offset
    value += MOTOR_MIN;
    if (target_state.lift < 100)
        return 0;
    else
        return (uint16_t)value;
}

void Quadrupel::set_p_ctr(ControllerParameterData *data) {
    p_ctr.p_yaw = data->get_p_yaw();
    p_ctr.p_height = data->get_p_height();
    p_ctr.p1_pitch_roll = data->get_p1_pitch_roll();
    p_ctr.p2_pitch_roll = data->get_p2_pitch_roll();
}

void Quadrupel::set_p_misc(MiscParameterData *data) {
    if (data->get_func_raw()) {
        // Enable RAW Mode
        func_state |= FUNC_RAW;
    }
    else {
        //Disable RAW Mode
        func_state &= ~FUNC_RAW;
    }
    if (data->get_func_logging()) {
        // Enable Logging
        func_state |= FUNC_LOGGING;
    }
    else {
        //Disable Logging
        func_state &= ~FUNC_LOGGING;
    }
    if (data->get_func_wireless()) {
        // Enable Wireless Mode
        func_state |= FUNC_WIRELESS;
    }
    else {
        //Disable Wireless Mode
        func_state &= ~FUNC_WIRELESS;
    }
}

void Quadrupel::set_current_state() {
    // Center barometer output around normal sea level atmospheric pressure, so it fits in an int16_t.
    auto norm_pressure = static_cast<int16_t>(pressure - BARO_SEA_LEVEL);

    current_state.roll = phi - calibration_offsets.roll;
    current_state.pitch = theta - calibration_offsets.pitch;
    current_state.yaw = sr;

    // Apply moving average filter.
    current_state.pressure = static_cast<int16_t>((current_state.pressure * (BARO_WINDOW_SIZE - 1) + norm_pressure) / BARO_WINDOW_SIZE);
}


void Quadrupel::start_flash_dump() {
    // Turn off heartbeats 
    func_state &= ~FUNC_TELEMETRY;
    flash_sequence_number = 0;
    func_state |= FUNC_FLASH_DUMP;
    nrf_gpio_pin_clear(RED);
}

void Quadrupel::stop_flash_dump() {
    func_state &= ~FUNC_FLASH_DUMP;
    // Turn on heartbeats
    func_state |= FUNC_TELEMETRY;
    nrf_gpio_pin_set(RED);
    set_mode(Safe);
}

void Quadrupel::send_flash_dump_data() {
    if (func_state & FUNC_FLASH_DUMP) {
        if (flash_sequence_number < FLASH_PACKETS) {
            uint8_t* flash_data = new uint8_t[DATA_SIZE];
            if (flash_sequence_number == FLASH_PACKETS - 1) {
                flash_read_bytes(flash_sequence_number * FLASH_BYTES_PER_UART_PACKET, flash_data, FLASH_LAST_PACKET_SIZE);
            }
            else {
                flash_read_bytes(flash_sequence_number * FLASH_BYTES_PER_UART_PACKET, flash_data, FLASH_BYTES_PER_UART_PACKET);
            }
            auto packet = new Packet(FlashData);
            auto data = new FlashDumpData(flash_sequence_number, flash_data);
            packet->set_data(data);
            send(packet);
            delete packet;
            delete[] flash_data;
        }
        else {
            stop_flash_dump();
        }
        flash_sequence_number++;
    }
}