#pragma once
#define TIMER_PERIOD        1  //1ms=1000Hz (MAX 23bit, 4.6h)

#define QUADRUPEL_TIMER_PERIOD 31 // RTC1 has a clock of 32.768 kHz (T = 30.517578125 us)

//#define DIVIDER_FLASH_DUMP  4   // 1000/500 - 1     (500 Hz) // not divided by DMP divider, unused use fill rate of tx_buffer instead

#define DIVIDER_DMP_MODE    9   // 1000/100 - 1     (100 Hz)
#define DIVIDER_LED         99  // 100/1 - 1        (  1 Hz) // also divided by DMP divider
#define DIVIDER_TELEMETRY   9   // 100/10 - 1       ( 10 Hz) // also divided by DMP divider
#define DIVIDER_LOG         0   // 1000 - 1         (500 Hz) // also divided by DMP divider

#define TX_QUEUE_FLASH_DUMP_LIMIT  128  // Used to throttle the flash dump speed.

#define TIMEOUT_COMM        500000 // us

#define PANIC_DECREMENT     1 // unitless

#define BATTERY_THRESHOLD   100//1050 // cV

#define MOTOR_MIN 200
#define MOTOR_MAX 750

#define RATE_LIFT 128
#define RATE_PITCH_ROLL 512
#define RATE_YAW 256

#define BARO_SEA_LEVEL 101325
#define BARO_WINDOW_SIZE 8