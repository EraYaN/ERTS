%{
    /*******************************\
    |                               |
    |   Data plotter                |
    |       ERTS                     |
    |       Casper van Wezel        |
    |       2017-10-19              |
    |                               |
    \*******************************/
%}

clc
clear all
close all

VERBOSE = 0;
EXPORT_PLOT = 0;
LineWidth = 2;

% folder = 'C:\Users\12cas\git\ERTS\src\ERTS.Dashboard\bin\x64\Debug\';

folder = './parser/';
filename = 'data.bin';
% filename = 'flash-636440265692572166.bin';
% filename = 'flash-636440301372236726.bin';
file = [folder filename];

fileID = fopen(file);
raw = fread(fileID);
fclose(fileID);

% get_time_us()
% 0x1
% mode
% uint16_t lift
% int16_t roll
% int16_t pitch
% int16_t yaw


% state = struct('lift',{},'roll',{},'pitch',{},'yaw',{});
% remote = struct('time',{},'mode',{},'status',{});
remote = struct('time',{},'mode',{},'lift',{},'roll',{},'pitch',{},'yaw',{});

message_counter = 1;
message_byte = 1;
lifts(1) = 0;
for i = 1:size(raw,1)
% for i = 1:50
    if (VERBOSE)
        txt = sprintf("message_counter: %d\tmessage_byte: %d\t",message_counter,message_byte);
        disp(txt);
    end
    
    if (message_byte==1)
        time(1) = raw(i);
    elseif (message_byte==2)
        time(2) = raw(i);
    elseif (message_byte==3)
        time(3) = raw(i);
    elseif (message_byte==4)
        time(4) = raw(i);
    elseif (message_byte==5) %message
%         disp('message:');
%         disp(raw(i));
        if (raw(i)==255)
            break; %end off messages
        end
%         message = raw(i);
%         disp(message);
    elseif (message_byte==6) %mode
        remote(message_counter).mode = raw(i);
        
    elseif (message_byte==7) %lift1
        lift(1) = raw(i);
    elseif (message_byte==8) %lift2
        lift(2) = raw(i);
    elseif (message_byte==9) %roll1
        roll(1) = raw(i);
    elseif (message_byte==10) %roll2
        roll(2) = raw(i);
    elseif (message_byte==11) %pitch1
        pitch(1) = raw(i);
    elseif (message_byte==12) %pitch2
        pitch(2) = raw(i);
    elseif (message_byte==13) %yaw1
        yaw(1) = raw(i);
    elseif (message_byte==14) %yaw2
        yaw(2) = raw(i);
%         disp(yaw);
%         lift = swapbytes(lift);
%         roll = swapbytes(roll)
%         pitch = swapbytes(pitch);
%         yaw = swapbytes(yaw);
%         disp(time);
        remote(message_counter).time = typecast(uint8(time), 'uint32');
        remote(message_counter).lift = typecast(uint8(lift), 'uint16');
        remote(message_counter).roll = typecast(uint8(roll), 'uint16');
        remote(message_counter).pitch = typecast(uint8(pitch), 'uint16');
        remote(message_counter).yaw = typecast(uint8(yaw), 'uint16');
        message_counter = message_counter + 1;
        message_byte = 0;
    end
    
    if message_counter == 1000
        %         return
    end
    message_byte = message_byte +1;
end

figure;
plot([remote.mode],'*')
figure;
hold on;
% plot([remote.time],swapbytes([remote.lift]),'LineWidth',LineWidth);
% plot([remote.time],[remote.roll],'LineWidth',LineWidth);
plot([remote.time],[remote.pitch],'LineWidth',LineWidth);
% plot([remote.time],[remote.yaw],'LineWidth',LineWidth);
legend('lift','roll','pitch','yaw');
