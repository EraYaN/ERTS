/*------------------------------------------------------------------
 *  timers.c -- TIMER1 is used for the motors & TIMER2 is used for
 *                  motors and time-keeping.
 *              TIMER0 is reserved for the soft-device (BLE).
 *
 *  I. Protonotarios
 *  Embedded Software Lab
 *
 *  July 2016
 *------------------------------------------------------------------
 */

#include "driver.h"

uint32_t global_time;
bool timer_flag;

uint32_t get_time_us() {
    NRF_TIMER2->TASKS_CAPTURE[2] = 1;
    return (global_time + (NRF_TIMER2->CC[2] >> 3));
}

bool check_timer_flag() {
    return timer_flag;
}

void clear_timer_flag() {
    timer_flag = false;
}


void quadrupel_timer_handler(void *p_context) {
    timer_flag = true;
    //LoopHandler();
}


void TIMER1_IRQHandler() {
    if (NRF_TIMER1->EVENTS_COMPARE[3])
    {
        NRF_TIMER1->EVENTS_COMPARE[3] = 0;

        NRF_TIMER1->TASKS_CAPTURE[2] = 1;
        if (!radio_active && (NRF_TIMER1->CC[2] < 500))
        {
            motor[0] = (motor[0] < 1000) ? ((motor[0] < 0) ? 0 : motor[0]) : 1000;
            motor[1] = (motor[1] < 1000) ? ((motor[1] < 0) ? 0 : motor[1]) : 1000;
            NRF_TIMER1->CC[0] = 1000 + motor[0];
            NRF_TIMER1->CC[1] = 1000 + motor[1];
        }
    }
}

void TIMER2_IRQHandler() {
    if (NRF_TIMER2->EVENTS_COMPARE[3])
    {
        NRF_TIMER2->EVENTS_COMPARE[3] = 0;
        global_time += 312; //2500*0.125 in us

        // Check that the radio is not being used (highest interrupt) and that we are not near a flip in the pulse
        // This interrupt should only fire at the first 62 us of each motor pulse - otherwise there is a chance of flipping the pwm signal!
        NRF_TIMER2->TASKS_CAPTURE[2] = 1;
        if (!radio_active && (NRF_TIMER2->CC[2] < 500))
        {
            motor[2] = (motor[2] < 1000) ? ((motor[2] < 0) ? 0 : motor[2]) : 1000;
            motor[3] = (motor[3] < 1000) ? ((motor[3] < 0) ? 0 : motor[3]) : 1000;
            NRF_TIMER2->CC[0] = 1000 + motor[2];
            NRF_TIMER2->CC[1] = 1000 + motor[3];
        }
    }
}

/*void TIMER3_IRQHandler() {
    if (NRF_TIMER3->EVENTS_COMPARE[5])
    {
        NRF_TIMER3->EVENTS_COMPARE[5] = 0;
        nrf_gpio_pin_toggle(RED);
        quad.tick();
    }
}*/

/*void RTC1_IRQHandler() {
    nrf_gpio_pin_toggle(YELLOW);
    if (NRF_RTC1->EVENTS_COMPARE[3])
    {
        NRF_RTC1->EVENTS_COMPARE[3] = 0;
        nrf_gpio_pin_toggle(RED);
        //LoopHandler();
    }
}*/

void timers_init(void) {
    global_time = 0;
    timer_flag = false;

    /*NRF_TIMER3->PRESCALER = 0x1UL; // 0.125us
    NRF_TIMER3->BITMODE = 0x3UL; // 32bit
    NRF_TIMER3->INTENSET = TIMER_INTENSET_COMPARE5_Msk;
    NRF_TIMER3->CC[0] = 1000;
    NRF_TIMER3->CC[1] = 1000;
    NRF_TIMER3->CC[2] = 1000;
    NRF_TIMER3->CC[3] = 1000;
    NRF_TIMER3->CC[4] = 1000;
    NRF_TIMER3->CC[5] = 8000000; // 8000 * 1000 * 0.125 us = 1000 ms
    NRF_TIMER3->SHORTS = TIMER_SHORTS_COMPARE5_CLEAR_Msk;
    NRF_TIMER3->TASKS_CLEAR = 1;*/

    NRF_TIMER2->PRESCALER = 0x1UL; // 0.125us
    NRF_TIMER2->INTENSET = TIMER_INTENSET_COMPARE3_Msk;
    NRF_TIMER2->CC[0] = 1000; // motor signal is 125-250us
    NRF_TIMER2->CC[1] = 1000; //
//  NRF_TIMER2->CC[2] = 1000; // is used to measure at which part of the motor pulse we are currently at & timekeeping
    NRF_TIMER2->CC[3] = 2500;
    NRF_TIMER2->SHORTS = TIMER_SHORTS_COMPARE3_CLEAR_Msk;
    NRF_TIMER2->TASKS_CLEAR = 1;

    NRF_TIMER1->PRESCALER = 0x1UL; // 0.125us
    NRF_TIMER1->INTENSET = TIMER_INTENSET_COMPARE3_Msk;
    NRF_TIMER1->CC[0] = 1000;
    NRF_TIMER1->CC[1] = 1000;
    NRF_TIMER1->CC[3] = 2500;
    NRF_TIMER1->SHORTS = TIMER_SHORTS_COMPARE3_CLEAR_Msk;
    NRF_TIMER1->TASKS_CLEAR = 1;

    /*NRF_RTC1->PRESCALER = 0x0UL; // 32768 Hz / 30.517 us
    NRF_RTC1->INTENSET = RTC_INTENSET_COMPARE3_Msk;
    NRF_RTC1->CC[0] = 1;
    NRF_RTC1->CC[1] = 1;
    NRF_RTC1->CC[2] = 1;
    NRF_RTC1->CC[3] = 33000; // 33 * 1000 * 30.517 = 1007.061 ms
    NRF_RTC1->TASKS_CLEAR = 1;*/

    NRF_RTC1->TASKS_START = 1;
    //NRF_TIMER3->TASKS_START = 1;
    NRF_TIMER2->TASKS_START = 1;
    NRF_TIMER1->TASKS_START = 1;

    NVIC_ClearPendingIRQ(TIMER2_IRQn);
    NVIC_SetPriority(TIMER2_IRQn, 3);
    NVIC_EnableIRQ(TIMER2_IRQn);
    NVIC_ClearPendingIRQ(TIMER1_IRQn);
    NVIC_SetPriority(TIMER1_IRQn, 3);
    NVIC_EnableIRQ(TIMER1_IRQn);

    /*NVIC_ClearPendingIRQ(RTC1_IRQn);
    NVIC_SetPriority(RTC1_IRQn, 8); //middle to low
    NVIC_EnableIRQ(RTC1_IRQn);*/

    /*NVIC_ClearPendingIRQ(TIMER3_IRQn);
    NVIC_SetPriority(TIMER3_IRQn, 15); //lowest prio
    NVIC_EnableIRQ(TIMER3_IRQn);*/

    // motor 0 - gpiote 0
    NRF_PPI->CH[0].EEP = (uint32_t)&NRF_TIMER1->EVENTS_COMPARE[0];
    NRF_PPI->CH[0].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[0];
    NRF_PPI->CH[1].EEP = (uint32_t)&NRF_TIMER1->EVENTS_COMPARE[3];
    NRF_PPI->CH[1].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[0];

    // motor 1 - gpiote 1
    NRF_PPI->CH[2].EEP = (uint32_t)&NRF_TIMER1->EVENTS_COMPARE[1];
    NRF_PPI->CH[2].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[1];
    NRF_PPI->CH[3].EEP = (uint32_t)&NRF_TIMER1->EVENTS_COMPARE[3];
    NRF_PPI->CH[3].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[1];

    // motor 2 - gpiote 2
    NRF_PPI->CH[4].EEP = (uint32_t)&NRF_TIMER2->EVENTS_COMPARE[0];
    NRF_PPI->CH[4].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[2];
    NRF_PPI->CH[5].EEP = (uint32_t)&NRF_TIMER2->EVENTS_COMPARE[3];
    NRF_PPI->CH[5].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[2];

    // motor 3 - gpiote 3
    NRF_PPI->CH[6].EEP = (uint32_t)&NRF_TIMER2->EVENTS_COMPARE[1];
    NRF_PPI->CH[6].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[3];
    NRF_PPI->CH[7].EEP = (uint32_t)&NRF_TIMER2->EVENTS_COMPARE[3];
    NRF_PPI->CH[7].TEP = (uint32_t)&NRF_GPIOTE->TASKS_OUT[3];

    NRF_PPI->CHENSET = PPI_CHENSET_CH0_Msk | PPI_CHENSET_CH1_Msk | PPI_CHENSET_CH2_Msk | PPI_CHENSET_CH3_Msk | PPI_CHENSET_CH4_Msk | PPI_CHENSET_CH5_Msk | PPI_CHENSET_CH6_Msk | PPI_CHENSET_CH7_Msk;

    APP_TIMER_INIT(APP_TIMER_PRESCALER, APP_TIMER_OP_QUEUE_SIZE, NULL);

    APP_TIMER_DEF(quadrupel_timer);
    app_timer_create(&quadrupel_timer, APP_TIMER_MODE_REPEATED, quadrupel_timer_handler);
    app_timer_start(quadrupel_timer, QUADRUPEL_TIMER_PERIOD, NULL);

}
