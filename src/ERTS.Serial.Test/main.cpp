
#include <serial.h>
#include <packet.h>
#include <iostream>
#include <iomanip>
#include <remote_control_data.h>
#include <thread>

Serial *serial;

void packet_print(uint8_t *packet) {
    for (int i = 0; i < MAX_PACKET_SIZE; ++i) {
        std::cout << std::setfill('0') << std::setw(2) << std::uppercase << std::hex << (int) packet[i];

        if (i < MAX_PACKET_SIZE - 1) {
            std::cout << " ";
        }
    }

    std::cout << std::endl;
}

void packet_send(Packet *packet) {
    auto buffer = new uint8_t[MAX_PACKET_SIZE + 3];

    packet->to_buffer(&buffer[3]);

#ifdef DEBUG
    std::cout << "TX:\t\t";
    packet_print(&buffer[3]);
#endif

    for (int i = 3; i < MAX_PACKET_SIZE + 3; ++i) {
        serial->putchar(buffer[i]);
    }

    delete[] buffer;
    delete packet;
}

int main(int argc, char *argv[]) {
    if (argc != 2)
        throw "Serial port not set.";

    serial = new Serial(argv[1]);

    while (true) {
        auto *p = new Packet(RemoteControl);
        auto *d = new RemoteControlData(0, 0, 0, 0);
        p->set_data(d);
        packet_send(p);
        serial->flush(TCIFLUSH);

        std::this_thread::sleep_for(std::chrono::microseconds(100000));
    }

    return 0;
}