#include <iostream>
#include <iomanip>

using namespace std;

void to_buffer(uint8_t* buffer) {
	*(reinterpret_cast<uint16_t*>(&buffer[0])) = 0x1122;
	*(reinterpret_cast<int16_t*>(&buffer[2])) = 0x1133;
	*(reinterpret_cast<int16_t*>(&buffer[4])) = 0x1122;
	*(reinterpret_cast<int16_t*>(&buffer[6])) = 0x1122;
	*(reinterpret_cast<int16_t*>(&buffer[8])) = 0x1122;
	*(reinterpret_cast<int16_t*>(&buffer[10])) = 0x1122;
	*(reinterpret_cast<uint16_t*>(&buffer[12])) = 0x1122;
}
int main() {
	
	uint8_t* buffer = new uint8_t[20];
	memset(buffer, 0, 20);

	to_buffer(&buffer[5]);


	for (int i = 0; i < 20; i++) {
		cout << hex << uppercase << setw(2) << i << ": " << setw(2) << (int)buffer[i] << endl;
	}

	delete[] buffer;

	cout << "Done. Press enter to exit..." << endl;
	cin.get();
	return 0;
}