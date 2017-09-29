#include "quad.h"

#include <string>

extern "C"
{
#include "driver.h"
}

#ifndef FAKE_DRIVERS
extern "C" {
    void HardFault_Handler(void)
    {
        nrf_gpio_pin_toggle(RED);
        uint32_t *sp = (uint32_t *)__get_MSP(); // Get stack pointer
        uint32_t ia = sp[12]; // Get instruction address from stack

        nrf_delay_ms(1000);
        nrf_gpio_pin_toggle(YELLOW);
        printf("Hard Fault at address: 0x%08x\r\n", (unsigned int)ia);
        nrf_delay_ms(100);
        nrf_gpio_pin_toggle(GREEN);
        while (1)
        {
            if (NRF_UART0->EVENTS_TXDRDY != 0) {
                NRF_UART0->EVENTS_TXDRDY = 0;
                if (tx_queue.count) {
                    NRF_UART0->TXD = dequeue(&tx_queue);
                    nrf_gpio_pin_toggle(YELLOW);
                }
            }
            
            nrf_delay_ms(100);
        }
    }

}
#endif


bool running = false;
int16_t motor[4], ae[4];

int main(int argc, char *argv[]) {
#ifdef FAKE_DRIVERS
    if (argc > 1) {
        serial_port = argv[1];
    }
#endif

    Quadrupel quad = Quadrupel();
    running = true;
    quad.exit = false;
    while (!quad.exit) {
        quad.busywork();
        if (check_timer_flag()) {
            quad.tick();
            clear_timer_flag();
        }
#ifdef FAKE_DRIVERS
        nrf_delay_ms(1);
#endif

    }
    running = false;
    printf("\n\t Goodbye \n\n");
    nrf_delay_ms(100);

    NVIC_SystemReset();
}

