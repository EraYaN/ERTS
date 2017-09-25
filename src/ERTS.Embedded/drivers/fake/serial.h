#pragma once
#include <string>

class Serial {

#if defined(_WIN32) || defined(_WIN64)
#include <windows.h>

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
};