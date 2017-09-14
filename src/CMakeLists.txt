cmake_minimum_required(VERSION 3.8)
project(src)

set(CMAKE_CXX_STANDARD 11)

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
        ERTS.Embedded/comm/packet.cpp
        ERTS.Embedded/comm/packet.h
        ERTS.Embedded/comm/packet_datastructures.h
        ERTS.Embedded/erts-quad.cpp
        ERTS.Embedded/quad.cpp
        ERTS.Embedded/quad.h ERTS.Embedded/comm/packet_data.h ERTS.Embedded/comm/telemetry_data.cpp ERTS.Embedded/comm/telemetry_data.h ERTS.Embedded/comm/remote_control_data.cpp ERTS.Embedded/comm/remote_control_data.h ERTS.Embedded/comm/mode_switch_data.cpp ERTS.Embedded/comm/mode_switch_data.h ERTS.Embedded/comm/acknowledge_data.cpp ERTS.Embedded/comm/acknowledge_data.h)

include_directories(
        ../libraries/libcrc-2.0/include
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
        ERTS.Embedded/comm
)

add_executable(src ${SOURCE_FILES})