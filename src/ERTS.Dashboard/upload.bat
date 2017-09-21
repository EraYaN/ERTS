@echo off
echo Uploading...
py -2 ../../tools/dfu_serial/serial_dfu.py -p COM3 ../ERTS.Embedded/bin/ARM/erts-quad.bin
echo Done