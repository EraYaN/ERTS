#include "quad.h"
#include "nrf_delay.h"

extern "C"
{
#include "driver.h"
}

int16_t motor[4], ae[4];

int main() {
    // Init instance of the drone class and bind motors.
    Quadrupel quad = Quadrupel();
    quad.exit = false;
    *motor = *quad.motor;
    *ae = *quad.ae;

    while (!quad.exit) {
        quad.tick();
    }

    printf("\n\t Goodbye \n\n");
    nrf_delay_ms(100);

    NVIC_SystemReset();
}

