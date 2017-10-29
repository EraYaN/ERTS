import io
import struct
import argparse
import numpy as np
import scipy as sp

LENGTH=131071


FLASH_LENGTH_HEADER=6 
FLASH_LENGTH_REMOTE=(8 + FLASH_LENGTH_HEADER)
FLASH_LENGTH_TELEMETRY=(14 + FLASH_LENGTH_HEADER)
FLASH_LENGTH_SENSOR=(12 + FLASH_LENGTH_HEADER)
FLASH_LENGTH_TEST=(0 + FLASH_LENGTH_HEADER)

flashRemote = 0x0
flashTelemetry = 0x1
flashSensor = 0x2
flashSensorIntermediate = 0x3
flashControl = 0x4
flashTest = 0x8
flashUnkown = 0xF

fmt_remote = '<Hhhh'
fmt_header = '<IBB'
fmt_telemetry = '<HhhhhhH'
fmt_sensor = '<hhhhhh'


l_roll = []
l_pitch = []
l_yaw = []
l_pressure = []
l_sp = []
l_sq = []
l_sr = []
l_sax = []
l_say = []
l_saz = []

def process_file(file):
    with open(file,'rb') as f:        
        print('Opened {} for reading...'.format(file))
        while True:
            buff = f.read(struct.calcsize(fmt_header))
            if not buff: break
            if len(buff)!=struct.calcsize(fmt_header): break
            (timestamp,type,mode) = struct.unpack_from(fmt_header,buff)            
            
            if type == flashRemote:
                buff = f.read(struct.calcsize(fmt_remote))
                if not buff: break
                if len(buff)!=struct.calcsize(fmt_remote): break
                (rc_lift,rc_roll,rc_pitch,rc_yaw) = struct.unpack_from(fmt_remote,buff)                
            elif type == flashTelemetry:
                buff = f.read(struct.calcsize(fmt_telemetry))
                if not buff: break
                if len(buff)!=struct.calcsize(fmt_telemetry): break
                (battery_voltage,roll,pitch,yaw,pressure,func_state,loop_time) = struct.unpack_from(fmt_telemetry,buff)
                l_roll.append(roll)
                l_pitch.append(pitch)
                l_yaw.append(yaw)
                l_pressure.append(pressure)               
            elif type == flashSensor:
                buff = f.read(struct.calcsize(fmt_sensor))
                if not buff: break
                if len(buff)!=struct.calcsize(fmt_sensor): break
                (sp,sq,sr,sax,say,saz) = struct.unpack_from(fmt_sensor,buff)
                l_sp.append(sp)
                l_sq.append(sq)
                l_sr.append(sr)
                l_sax.append(sax) 
                l_say.append(say)
                l_saz.append(saz)
    np_roll = np.array(l_roll)
    np_pitch = np.array(l_pitch)
    np_yaw = np.array(l_yaw)
    np_pressure = np.array(l_pressure)
    np_sp = np.array(l_sp)
    np_sq = np.array(l_sq)
    np_sr = np.array(l_sr)
    np_sax = np.array(l_sax)
    np_say = np.array(l_say)
    np_saz = np.array(l_saz)

    print_stats(np_roll,name='roll')
    print_stats(np_pitch,name='pitch')
    print_stats(np_yaw,name='yaw')
    print_stats(np_pressure,name='pressure')
    print_stats(np_sp,name='sp')
    print_stats(np_sq,name='sq')
    print_stats(np_sr,name='sr')
    print_stats(np_sax,name='sax')
    print_stats(np_say,name='say')
    print_stats(np_saz,name='saz')


def print_stats(nparray, name='data'):
    average = np.average(nparray)
    stdev = np.std(nparray)
    smallsignal = nparray - average
    p_p_max_noise = np.amax(np.abs(smallsignal))
    print("{}; Average: {}. Stdev: {}. p-p max noise: {}".format(name,average,stdev,p_p_max_noise))

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Process flash data.')
    parser.add_argument('--file', '-f', dest='file', action='store', help='The input filename')

    args = parser.parse_args()
    process_file(args.file)
