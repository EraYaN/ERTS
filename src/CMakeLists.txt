cmake_minimum_required(VERSION 3.8)
project(src)

set(CMAKE_CXX_STANDARD 11)

set(FAKE_SOURCE_FILES
        ERTS.Embedded/drivers/driver.h
        ERTS.Embedded/drivers/fake/fake_driver.cpp
        ERTS.Embedded/drivers/fake/serial.cpp
        ERTS.Embedded/drivers/queue.c
        ERTS.Embedded/erts-quad.cpp
        ERTS.Embedded/quad.cpp
        ERTS.Embedded/quad.h
        ../libraries/libcrc-2.0/src/crc16.c
        ../libraries/libcrc-2.0/src/crcccitt.c
        ../libraries/libcrc-2.0/src/crcdnp.c
        ../libraries/libcrc-2.0/src/crckrmit.c
        ../libraries/libcrc-2.0/src/crcsick.c
        ../libraries/libcrc-2.0/src/nmea-chk.c
        ../libraries/libertscommunication/packet.h
        ../libraries/libertscommunication/packet.cpp
        ../libraries/libertscommunication/packet_data.h
        ../libraries/libertscommunication/packet_datastructures.h
        ../libraries/libertscommunication/acknowledge_data.h
        ../libraries/libertscommunication/acknowledge_data.cpp
        ../libraries/libertscommunication/mode_switch_data.h
        ../libraries/libertscommunication/mode_switch_data.cpp
        ../libraries/libertscommunication/remote_control_data.h
        ../libraries/libertscommunication/remote_control_data.cpp
        ../libraries/libertscommunication/telemetry_data.h
        ../libraries/libertscommunication/telemetry_data.cpp
        ../libraries/libertscommunication/actuation_parameter_data.cpp
        ../libraries/libertscommunication/actuation_parameter_data.h
        ERTS.Embedded/drivers/fake/serial.h ../libraries/libertscommunication/controller_parameter_data.cpp
        ../libraries/libertscommunication/controller_parameter_data.h
        ../libraries/libertscommunication/misc_parameter_data.cpp
        ../libraries/libertscommunication/misc_parameter_data.h
        ../libraries/libertscommunication/exception_data.cpp
        ../libraries/libertscommunication/exception_data.h)

add_executable(fake_quad ${FAKE_SOURCE_FILES})
target_compile_definitions(fake_quad PUBLIC -DFAKE_DRIVERS)
target_include_directories(fake_quad PUBLIC
        ../libraries/libcrc-2.0/include
        ../libraries/libertscommunication
        ERTS.Embedded/drivers)


set(SOURCE_FILES
        ERTS.Embedded/drivers/config/nrf_drv_config.h
        ERTS.Embedded/drivers/config/pstorage_platform.h
        ERTS.Embedded/drivers/adc.c
        ERTS.Embedded/drivers/baro.c
        ERTS.Embedded/drivers/ble.c
        ERTS.Embedded/drivers/driver.h
        ERTS.Embedded/drivers/gpio.c
        ERTS.Embedded/drivers/queue.c
        ERTS.Embedded/drivers/spi_flash.c
        ERTS.Embedded/drivers/timers.c
        ERTS.Embedded/drivers/twi.c
        ERTS.Embedded/drivers/uart.c
        ERTS.Embedded/drivers/invensense/dmpKey.h
        ERTS.Embedded/drivers/invensense/dmpmap.h
        ERTS.Embedded/drivers/invensense/inv_mpu.c
        ERTS.Embedded/drivers/invensense/inv_mpu.h
        ERTS.Embedded/drivers/invensense/inv_mpu_dmp_motion_driver.c
        ERTS.Embedded/drivers/invensense/inv_mpu_dmp_motion_driver.h
        ERTS.Embedded/drivers/invensense/ml.c
        ERTS.Embedded/drivers/invensense/ml.h
        ERTS.Embedded/drivers/invensense/mpu_wrapper.c
        ERTS.Embedded/erts-quad.cpp
        ERTS.Embedded/quad.cpp
        ERTS.Embedded/quad.h
        ../libraries/libertscommunication/packet.h
        ../libraries/libertscommunication/packet.cpp
        ../libraries/libertscommunication/packet_data.h
        ../libraries/libertscommunication/packet_datastructures.h
        ../libraries/libertscommunication/acknowledge_data.h
        ../libraries/libertscommunication/acknowledge_data.cpp
        ../libraries/libertscommunication/mode_switch_data.h
        ../libraries/libertscommunication/mode_switch_data.cpp
        ../libraries/libertscommunication/remote_control_data.h
        ../libraries/libertscommunication/remote_control_data.cpp
        ../libraries/libertscommunication/telemetry_data.h
        ../libraries/libertscommunication/telemetry_data.cpp
        ../libraries/libertscommunication/actuation_parameter_data.cpp
        ../libraries/libertscommunication/actuation_parameter_data.h
        ../libraries/libertscommunication/controller_parameter_data.cpp
        ../libraries/libertscommunication/controller_parameter_data.h
        ../libraries/libertscommunication/misc_parameter_data.cpp
        ../libraries/libertscommunication/misc_parameter_data.h)

add_executable(quad ${SOURCE_FILES})
target_include_directories(quad PUBLIC
        ../libraries/libcrc-2.0/include
        ../libraries/libertscommunication
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/libraries/util
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/ble/common
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/ble/ble_services/ble_nus
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/ble/ble_advertising
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/device
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/drivers_nrf/config
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/drivers_nrf/delay
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/drivers_nrf/hal
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/libraries/timer
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/toolchain/gcc
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/softdevice/s110/headers
        ../libraries/nRF51_SDK_10.0.0_dc26b5e/components/softdevice/common/softdevice_handler
        ERTS.Embedded/drivers
)


set(SERIAL_SOURCE_FILES
        ERTS.Serial.Test/main.cpp
        ERTS.Embedded/drivers/fake/serial.cpp
        ../libraries/libertscommunication/packet.cpp
        ../libraries/libertscommunication/acknowledge_data.cpp
        ../libraries/libertscommunication/mode_switch_data.cpp
        ../libraries/libertscommunication/remote_control_data.cpp
        ../libraries/libertscommunication/telemetry_data.cpp
        ../libraries/libertscommunication/actuation_parameter_data.cpp
        ../libraries/libertscommunication/controller_parameter_data.cpp
        ../libraries/libertscommunication/misc_parameter_data.cpp
        ../libraries/libcrc-2.0/src/crc16.c
        ../libraries/libcrc-2.0/src/crcccitt.c
        ../libraries/libcrc-2.0/src/crcdnp.c
        ../libraries/libcrc-2.0/src/crckrmit.c
        ../libraries/libcrc-2.0/src/crcsick.c
        ../libraries/libcrc-2.0/src/nmea-chk.c
        )

add_executable(serial_test ${SERIAL_SOURCE_FILES})

target_include_directories(serial_test PUBLIC
        ERTS.Embedded/drivers/fake
        ../libraries/libertscommunication
        ../libraries/libcrc-2.0/include)