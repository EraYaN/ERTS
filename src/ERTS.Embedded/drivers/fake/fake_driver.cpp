#include <fstream>
#include <iostream>
#include <iomanip>
#include <thread>
#include <packet_datastructures.h>

extern "C"
{
#include "driver.h"
}

#include "serial.h"

// Device
void NVIC_SystemReset() {
    std::cout << "System reset." << std::endl;
}

void nrf_delay_ms(uint32_t number_of_ms) {
    std::this_thread::sleep_for(std::chrono::milliseconds(number_of_ms));
}

// Timers
void timers_init() {
    std::cout << "Timers initialized." << std::endl;
};

uint32_t get_time_us() {
    auto now = std::chrono::high_resolution_clock::now();
    return (uint32_t)std::chrono::duration_cast<std::chrono::microseconds>(now.time_since_epoch()).count();
}

bool check_timer_flag() {
    return true;
}

void clear_timer_flag() {

}

// GPIO
void gpio_init() {
    std::cout << "GPIO initialized." << std::endl;
}

void nrf_gpio_pin_toggle(uint32_t pin_number) {
    switch (pin_number) {
        case BLUE:
            std::cout << "Toggled blue led." << std::endl;
            break;
        case GREEN:
            std::cout << "Toggled green led." << std::endl;
            break;
        case RED:
            std::cout << "Toggled red led." << std::endl;
            break;
        case YELLOW:
            std::cout << "Toggled yellow led." << std::endl;
            break;
        default:
            std::cout << "Toggled pin: " << pin_number << std::endl;
            break;
    }
}

void nrf_gpio_pin_set(uint32_t pin_number) {
    switch (pin_number) {
    case BLUE:
        std::cout << "Set (OFF) blue led." << std::endl;
        break;
    case GREEN:
        std::cout << "Set (OFF) green led." << std::endl;
        break;
    case RED:
        std::cout << "Set (OFF) red led." << std::endl;
        break;
    case YELLOW:
        std::cout << "Set (OFF) yellow led." << std::endl;
        break;
    default:
        std::cout << "Set pin: " << pin_number << std::endl;
        break;
    }
}

void nrf_gpio_pin_clear(uint32_t pin_number) {
    switch (pin_number) {
    case BLUE:
        std::cout << "Cleared (ON) blue led." << std::endl;
        break;
    case GREEN:
        std::cout << "Cleared (ON) green led." << std::endl;
        break;
    case RED:
        std::cout << "Cleared (ON) red led." << std::endl;
        break;
    case YELLOW:
        std::cout << "Cleared (ON) yellow led." << std::endl;
        break;
    default:
        std::cout << "Cleared pin: " << pin_number << std::endl;
        break;
    }
}

// UART
const char *serial_port;
queue rx_queue;
Serial *serial;

void packet_print(uint8_t *packet) {
    for (int i = 0; i < MAX_PACKET_SIZE; ++i) {
        std::cout << std::setfill('0') << std::setw(2)<< std::uppercase << std::hex << (int)packet[i];

        if (i < MAX_PACKET_SIZE - 1) {
            std::cout << " ";
        }
    }

    std::cout << std::endl;
}

void uart_init() {
    serial =  new Serial(serial_port);
    std::cout << "UART initialized." << std::endl;
}

bool uart_available() {
    uint8_t c;

    if (serial->getchar_nb((char *)&c)) {
        enqueue(&rx_queue, c);
        return true;
    }

    return false;
}

uint8_t uart_get() {
    uint8_t c;

    if (rx_queue.count > 0)
        c = dequeue(&rx_queue);
    else
        c = (uint8_t)serial->getchar();

    return c;
}

void uart_put(uint8_t byte) {
    serial->putchar(byte);
}

// TWI
void twi_init() {
    std::cout << "I2C initialized." << std::endl;
}

bool i2c_write(uint8_t slave_addr, uint8_t reg_addr, uint8_t length, uint8_t const *data) {
    throw std::runtime_error("I2C spoofing is not implemented.");
}

bool i2c_read(uint8_t slave_addr, uint8_t reg_addr, uint8_t length, uint8_t *data) {
    throw std::runtime_error("I2C spoofing is not implemented.");
}

// MPU wrapper
int16_t phi, theta, psi;
int16_t sp, sq, sr;
int16_t sax, say, saz;

void imu_init(bool dmp, uint16_t interrupt_frequency) {
    std::cout << "IMU initialized." << std::endl;
}

void get_dmp_data() {
    phi = 1337;
    theta = -1337;
    sp = sax = 1234;
    sq = say = -1234;
    sr = saz = 5678;
}

bool check_sensor_int_flag() {
    return true;
}

// Barometer
int32_t pressure;
int32_t temperature;

void baro_init() {
    std::cout << "Baro initialized." << std::endl;
}

void read_baro() {
    pressure = 101325;
    temperature = 2300;
}

// ADC
uint16_t bat_volt;

void adc_init() {
    std::cout << "ADC initialized." << std::endl;
}

void adc_request_sample() {
    bat_volt = 1200;
}

// Flash
const char* file_name = "flash.dat";
const int file_size = 128 * 1024;
std::fstream file;

bool spi_flash_init() {
    file.open(file_name, std::fstream::ate | std::fstream::binary);
    long long int length = file.tellg();
    file.close();

    if (length != file_size)
        flash_chip_erase();

    std::cout << "SPI flash initialized." << std::endl;

    return true;
}

bool flash_chip_erase() {
    file.open(file_name, std::fstream::out | std::fstream::trunc | std::fstream::binary);
    for (int i = 0; i < file_size; i++) {
        file.put(0);
    }
    file.close();

    return true;
}

bool flash_write_byte(uint32_t address, uint8_t data) {
    file.open(file_name, std::fstream::out | std::fstream::binary);
    file.seekp(address);
    file.put((char)data);
    file.close();

    return true;
}

bool flash_write_bytes(uint32_t address, uint8_t *data, uint32_t count) {
    file.open(file_name, std::fstream::out | std::fstream::binary);
    file.seekp(address);
    file.write((char *)data, count);
    file.close();

    return true;
}

bool flash_read_byte(uint32_t address, uint8_t *buffer) {
    file.open(file_name, std::fstream::in | std::fstream::binary);
    file.seekg(address);
    char c = *buffer;
    file.get(c);
    file.close();

    return true;
}

bool flash_read_bytes(uint32_t address, uint8_t *buffer, uint32_t count) {
    file.open(file_name, std::fstream::in | std::fstream::binary);
    file.seekg(address);
    file.read((char *)buffer, count);
    file.close();

    return true;
}

// BLE
void ble_init() {
    std::cout << "Bluetooth initialized." << std::endl;
}

void ble_send() {
    throw std::runtime_error("Bluetooth spoofing is not implemented.");
};