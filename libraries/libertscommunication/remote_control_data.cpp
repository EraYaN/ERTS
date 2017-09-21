#include <cstring>
#include "remote_control_data.h"

RemoteControlData::RemoteControlData(const uint8_t *data) {
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

void RemoteControlData::to_buffer(uint8_t *buffer) {

}
