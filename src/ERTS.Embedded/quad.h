#pragma once

#include <cstdint>
#include <cstdio>

extern "C"
{
#include "driver.h"
}

class Quadrupel
{
    uint32_t counter;

public:
    int16_t motor[4], ae[4];
    bool exit;

    Quadrupel();
    void tick();
    void process_key(uint8_t c);
    void update_motors();
    void control();
};
