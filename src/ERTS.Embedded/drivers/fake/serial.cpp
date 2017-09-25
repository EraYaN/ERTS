/*------------------------------------------------------------
 * Serial I/O
 * 8 bits, 1 stopbit, no parity,
 * 115,200 baud
 *
 * Original UNIX code:
 * Original by Arjan J.C. van Gemund (+ mods by Ioannis Protonotarios)
 *
 * Original Windows code:
 * Hans de Ruiter
 * http://hdrlab.org.nz/articles/windows-development/a-c-class-for-controlling-a-comm-port-in-windows/
 *
 * Modified for the ERTS project.
 *------------------------------------------------------------
 */

#include "serial.h"

#include <cassert>

#if defined(_WIN32) || defined(_WIN64)

#include <iostream>

using namespace std;

Serial::Serial(const char *address) {
    handle = CreateFile(address, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);

    if (handle == INVALID_HANDLE_VALUE) {
        throw ("ERROR: Could not open com port");
    }
    else {
        // set timeouts
        COMMTIMEOUTS cto = {MAXDWORD, 0, 0, 0, 0};
        DCB dcb;
        if (!SetCommTimeouts(handle, &cto)) {
            Serial::~Serial();
            throw ("ERROR: Could not set com port time-outs");
        }

        // set DCB
        memset(&dcb, 0, sizeof(dcb));
        dcb.DCBlength = sizeof(dcb);
        dcb.BaudRate = 115200;
        dcb.fBinary = 1;
        dcb.fDtrControl = DTR_CONTROL_ENABLE;
        dcb.fRtsControl = RTS_CONTROL_ENABLE;

        dcb.Parity = NOPARITY;
        dcb.StopBits = ONESTOPBIT;
        dcb.ByteSize = 8;

        if (!SetCommState(handle, &dcb)) {
            Serial::~Serial();
            throw ("ERROR: Could not set com port parameters");
        }
    }
}

Serial::~Serial() {
    CloseHandle(handle);
}

bool Serial::getchar_nb(char *c) {
    return result = ReadFile(handle, &c, 1, NULL, NULL);
}

char Serial::getchar() {
    char c;

    while (!getchar_nb(&c));
    return c;
}

int Serial::putchar(char c) {
    DWORD numWritten;
    WriteFile(handle, &c, 1, &numWritten, NULL);

    assert(numWritten == 1);
    return numWritten;
}

int Serial::read(char *buffer, int buffLen, bool nullTerminate) {
    DWORD numRead;
    if (nullTerminate) {
        --buffLen;
    }

    BOOL ret = ReadFile(handle, buffer, buffLen, &numRead, NULL);

    if (!ret) {
        return 0;
    }

    if (nullTerminate) {
        buffer[numRead] = '\0';
    }

    return numRead;
}

#else

#include <termios.h>
#include <fcntl.h>
#include <unistd.h>

Serial::Serial(const char *address) {
    int result;
    struct termios tty;

    handle = ::open(address, O_RDWR | O_NOCTTY);

    assert(handle >= 0);

    result = isatty(handle);
    assert(result == 1);

    auto name = ttyname(handle);
    assert(name != 0);

    result = tcgetattr(handle, &tty);
    assert(result == 0);

    tty.c_iflag = IGNBRK; /* ignore break condition */
    tty.c_oflag = 0;
    tty.c_lflag = 0;

    tty.c_cflag = (tty.c_cflag & ~CSIZE) | CS8; /* 8 bits-per-character */
    tty.c_cflag |= CLOCAL | CREAD; /* Ignore model status + read input */

    cfsetospeed(&tty, B115200);
    cfsetispeed(&tty, B115200);

    tty.c_cc[VMIN] = 0;
    tty.c_cc[VTIME] = 1; // added timeout

    tty.c_iflag &= ~(IXON | IXOFF | IXANY);

    result = tcsetattr(handle, TCSANOW, &tty); /* non-canonical */

    tcflush(handle, TCIOFLUSH); /* flush I/O buffer */
}

Serial::~Serial() {
    assert (::close(handle) == 0);
}

bool Serial::getchar_nb(char *c) {

    auto result = read(handle, c, 1);

    if (result == 0) {
        return false;
    }
    else {
        assert(result == 1);
        return true;
    }
}


char Serial::getchar() {
    char c;

    while (!getchar_nb(&c));
    return c;
}


int Serial::putchar(char c) {
    int result;

    do {
        result = (int) ::write(handle, &c, 1);
    } while (result == 0);

    assert(result == 1);
    return result;
}

#endif
