@echo off
echo Uploading...
py -2 ../../tools/dfu_serial/serial_dfu.py -p COM4 ../ERTS.Embedded/bin/erts-quad.bin
echo Done