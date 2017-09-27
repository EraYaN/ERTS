#include "quad.h"

#include <string>

extern "C"
{
#include "driver.h"
}

int16_t motor[4], ae[4];

int main(int argc, char *argv[]) {
#ifdef FAKE_DRIVERS
    if (argc > 1) {
        serial_port = argv[1];
    }
#endif

    // Init instance of the drone class and bind motors.
    Quadrupel quad = Quadrupel();
    quad.exit = false;
    *motor = *quad.motor;
    *ae = *quad.ae;

    while (!quad.exit) {
        quad.tick();

#ifdef FAKE_DRIVERS
        nrf_delay_ms(50);
#endif

    }

    printf("\n\t Goodbye \n\n");
    nrf_delay_ms(100);

    NVIC_SystemReset();
}

