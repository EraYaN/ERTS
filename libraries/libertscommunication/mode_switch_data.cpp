#include <cstring>
#include "mode_switch_data.h"

ModeSwitchData::ModeSwitchData(const uint8_t *data) {
    memcpy(&_data, data, get_length());
}

int ModeSwitchData::get_length() {
    return sizeof(modeSwitchData_t);
}

bool ModeSwitchData::get_expects_acknowledgement() {
    return true;
}

bool ModeSwitchData::is_valid() {
    return _data->newMode != None;
}

void ModeSwitchData::to_buffer(uint8_t *buffer) {

}