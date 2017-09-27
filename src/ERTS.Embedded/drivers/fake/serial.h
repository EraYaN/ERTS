#pragma once
#include <string>
#if defined(_WIN32) || defined(_WIN64)
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#endif

class Serial {


#if defined(_WIN32) || defined(_WIN64)
    HANDLE handle;
#else
    int handle;
#endif


public:
    explicit Serial(const char *address);

    ~Serial();

    char getchar();

    bool getchar_nb(char *c);

    int putchar(char c);

	int Serial::read(char *buffer, int buffLen, bool nullTerminate);
};