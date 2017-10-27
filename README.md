# ERTS

Embedded Real-Time Systems - Group 7

## SDK versions

v10 [https://developer.nordicsemi.com/nRF5_SDK/nRF51_SDK_v10.x.x/nRF51_SDK_10.0.0_dc26b5e.zip](https://developer.nordicsemi.com/nRF5_SDK/nRF51_SDK_v10.x.x/nRF51_SDK_10.0.0_dc26b5e.zip)

## Building

### Prerequisites

1. SDK v10 in `libraries/nRF51_SDK_10.0.0_dc26b5e`
2. Toolchain in `src/gcc-arm-none-eabi` (make sure this toolchain is 64-bit for building under WSL)

Using the WSL based builder with a proper x64 toolchain is recommended. Can be build by hand, or installed from your favourite package repository as for example `gcc-arm-none-eabi`. If you use the package repository change the line 49 in `libraries/Shared.mk` from

```
GNU_INSTALL_ROOT := $(realpath $(PROJECT_ROOT)/src/gcc-arm-none-eabi/bin/)/
```

to

```
GNU_INSTALL_ROOT :=
```

then it will use the correct applications from your PATH variable.

### Linux/OS X

The main embedded makefile is in `src/ERTS.Embedded/Makefile` and all makefiles inlcude `libraries/Shared.mk`

1. Install all build tools like make.
2. `cd src/ERTS.Embedded`
3. `make`
4. `make upload` or `make upload-mac`

Using `make` in the directory `src/ERTS.Embedded/` to build the project, should build all the libraries and the main arm binary. `make clean` will also clean all the library artifacts.

The final output is in `src/ERTS.Embedded/bin/erts-quad.bin`.

### Windows

On Windows and with WSL, you can just build the ERTS.Embedded.WSL project in the VisualStudio solution.

1. Install Visual Studio 2017 (Community or Higher)
2. Install '.NET desktop development', 'Desktop development with C++', 'Linux development with C++' workloads.
3. Install '.NET framework 4.6 targeting pack', 'NuGet package manager', 'Windows 10 SDK (10.0.xxxxx) for Desktop C++ [x86 and x64]'
4. Open `src/ERTS.sln`
5. You can now build `ERTS.Embedded.WSL` or 'ERTS.Embedded.Linux'<br>
a. For `ERTS.Embedded.Linux` configure a connection to a suitable linux machine or VM first in the project settings and the Connection Manager.
6. Use the `src/ERTS.Embedded/upload.COM*.bat` scripts to upload the bin file to the embedded system on Windows
7. Build the Project `ERTS.Dashboard`

The final output is in `src/ERTS.Embedded/bin/erts-quad.bin` while using the WSL build. For the linux build the 'Remote Build Root Directory' action needs to be altered to the correct username, this because the default value contains a `~` and this is not supported by the 'Remote Builds/Outputs' copy back action. The path needs to be absolute.

## Usage

The final ERTS.Dashboard artifacts are in `src\ERTS.Dashboard\bin\x64\Release`.

1. Make sure the correct `cfg.bin` file is in the current directory.
2. Check the configuration with 'Tools -> Settings...'<br>
a. Sane defaults are given in `src\ERTS.Dashboard\Settings\Settings.cs`
3. When everything is ready and the button is enabled, click 'Start Stage Two' and then `Send All Parameters`

### Keyboard Input Map

For full control input map see `src\ERTS.Dashboard\Input\PatchBox.cs`, one might need to change the Guid near the top to have a new joystick fully working.

## Authoring information

The Authoring information is contained in the header files where possible. Here follows a list of all files with Authoring annotations. `//Author: <name>`

- `libraries\libertscommunication\packet_data.h` (And all derived classes `*_data.h`)
- `libraries\libertscommunication\packet.h`
- `src\ERTS.Embedded\erts-quad.cpp`
- `src\ERTS.Embedded\quad.h`
- `src\ERTS.Embedded\flash_wrapper.h`

### Supporting code authors
These pieces of code are supporting pieces, but without them the project would not be complete.

- `src\ERTS.Dashboard\*`; Erwin. The Main Dashboard GUI assembly
- `src\ERTS.Dashboard.Server\Http`; Robin, Erwin. The web files for the phone control
- `src\ERTS.Dashboard.Server\*`; Erwin. The HTTP server for in the GUI assembly to enable the phone control.
- `src\ERTS.Dashboard.Fake\*`; Robin. A way to run the quad code on the PC using the fake drivers listed below.
- `src\ERTS.Embedded\drivers\fake\*`; Robin. The fake driver interface to run the quad on the PC for testing and easier debugging.
- `src\ERTS.Serial.Test\*`; Erwin. The Code for the tests of the Serial communication due to some of the problem with missing bytes on the Embeedded system side.
- `src\protocol-test*`; Erwin. Old protocol prototypes and packet construction as it is done on the embedded system side.
- `src\TimingDividerTests`; Erwin. Tests for the proper timer divers due to a significant frequency differential (10Hz -> 8Hz) while using the same code on the Embedded system.

### External code authors
- `libraries\libcrc-2.0\*`; External Lammert Bies. The crc_* function that are used in the communication on the Embedded Systems side.
- `src\CRCLib\*`; External Lammert Bies, translated to C# by JP. The Checksum calculation functions used on the Dashboard side.
- `libraries\nRF51_SDK_10.0.0_dc26b5e\*`; External, Nordic Semi Conductors. The base SDk for the hardware platofrm that was used.
- `src\MicroMvvm`; Extrernal jeremyellul. Some helper classes to make MVVM model programming a little easier on the Dashboard side.
- `src\EraYaN.Serial\*`; Erwin de Haan from a previous project












