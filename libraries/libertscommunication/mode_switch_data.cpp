#include <cstring>
#include "mode_switch_data.h"

ModeSwitchData::ModeSwitchData(byte *data) {
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

byte *ModeSwitchData::to_byte_array() {
    // Not implemented.
    return nullptr;
}
