#pragma once

#include <stdint.h>
#include <stdio.h>
#include <math.h>

#ifdef FAKE_DRIVERS
#include <stdbool.h>
#else
#include "app_timer.h"
#include "nrf_gpio.h"
#include "nrf_delay.h"
#include "invensense/dmpKey.h"
#include "invensense/inv_mpu.h"
#include "invensense/inv_mpu_dmp_motion_driver.h"
#include "invensense/ml.h"
#include "app_util_platform.h"
#endif

#define NUM_MOTORS  4

#define RED         22
#define YELLOW      24
#define GREEN       28
#define BLUE        30
#define INT_PIN     5

// Spoof some device functions.
#ifdef FAKE_DRIVERS
void nrf_gpio_pin_toggle(uint32_t pin_number);
void nrf_gpio_pin_set(uint32_t pin_number);
void nrf_gpio_pin_clear(uint32_t pin_number);
void NVIC_SystemReset();
void nrf_delay_ms(uint32_t volatile number_of_ms);
void packet_print(uint8_t *packet);

extern const char *serial_port;
#endif

// Control
extern int16_t motor[NUM_MOTORS], ae[NUM_MOTORS];

// Timers
#define TIMER_PERIOD    10 //50ms=20Hz (MAX 23bit, 4.6h)
#define HB_INTERVAL    10 // 1000/10

#define APP_TIMER_PRESCALER             0                                           /**< Value of the RTC1 PRESCALER register. */
#define APP_TIMER_OP_QUEUE_SIZE         4                                           /**< Size of timer operation queues. */
#define QUADRUPEL_TIMER_PERIOD  APP_TIMER_TICKS(TIMER_PERIOD, APP_TIMER_PRESCALER)  // timer period is in ms
//#define QUADRUPEL_TIMER_PERIOD 33000 // 33 * 1000 * 30.517 = 1007.061 ms

void timers_init(void);

uint32_t get_time_us(void);

bool check_timer_flag(void);

void clear_timer_flag(void);

// GPIO
void gpio_init(void);

// Queue
#define QUEUE_SIZE 256
typedef struct {
    uint8_t Data[QUEUE_SIZE];
    uint16_t first, last;
    uint16_t count;
} queue;

void init_queue(queue *q);

void enqueue(queue *q, uint8_t x);

uint8_t dequeue(queue *q);

// UART
#define RX_PIN_NUMBER  16
#define TX_PIN_NUMBER  14
extern queue rx_queue;
extern queue tx_queue;
extern uint32_t last_correct_checksum_time;

void uart_init(void);

bool uart_available();

uint8_t uart_get();

void uart_put(uint8_t);

// TWI
#define TWI_SCL 4
#define TWI_SDA 2

void twi_init(void);

bool i2c_write(uint8_t slave_addr, uint8_t reg_addr, uint8_t length, uint8_t const *data);

bool i2c_read(uint8_t slave_addr, uint8_t reg_addr, uint8_t length, uint8_t *data);

// MPU wrapper
extern int16_t phi, theta, psi;
extern int16_t sp, sq, sr;
extern int16_t sax, say, saz;
extern uint8_t sensor_fifo_count;

void imu_init(bool dmp,
              uint16_t interrupt_frequency); // if dmp is true, the interrupt frequency is 100Hz - otherwise 32Hz-8kHz
void get_dmp_data(void);

void get_raw_sensor_data(void);

bool check_sensor_int_flag(void);

void clear_sensor_int_flag(void);

// Barometer
extern int32_t pressure;
extern int32_t temperature;

void read_baro(void);

void baro_init(void);

// ADC
extern uint16_t bat_volt;

void adc_init(void);

void adc_request_sample(void);

// Flash
bool spi_flash_init(void);

bool flash_chip_erase(void);

bool flash_write_byte(uint32_t address, uint8_t data);

bool flash_write_bytes(uint32_t address, uint8_t *data, uint32_t count);

bool flash_read_byte(uint32_t address, uint8_t *buffer);

bool flash_read_bytes(uint32_t address, uint8_t *buffer, uint32_t count);

// BLE
extern queue ble_rx_queue;
extern queue ble_tx_queue;
extern volatile bool radio_active;

void ble_init(void);

void ble_send(void);
