#include <cstring>
#include "remote_control_data.h"

RemoteControlData::RemoteControlData(byte *data) {
    memcpy(&_data, data, get_length());
}

int RemoteControlData::get_length() {
    return sizeof(remoteControlData_t);
}

uint32_t RemoteControlData::get_ack_number() {
    return 0;
}

bool RemoteControlData::get_expects_acknowledgement() {
    return false;
}

bool RemoteControlData::is_valid() {
    return true;
}

byte *RemoteControlData::to_byte_array() {
    // Not implemented.
    return nullptr;
}
