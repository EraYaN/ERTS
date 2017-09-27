#include "exception_data.h"

ExceptionData::ExceptionData(const uint8_t* data) {
    _type = (exceptionType_t)data[0];
    memcpy(&_message, &data[1], MAX_MESSAGE_LENGTH);
}

ExceptionData::ExceptionData(exceptionType_t type, const char(&message)[MAX_MESSAGE_LENGTH+1]) {
    _type = type;
    memcpy((void *)message, &_message, MAX_MESSAGE_LENGTH);
}

void ExceptionData::to_buffer(uint8_t *buffer) {
    buffer[0] = _type;
    memcpy(&buffer[1], &_message, MAX_MESSAGE_LENGTH);

}
