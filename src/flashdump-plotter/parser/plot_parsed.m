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
% close all

VERBOSE = 0;
EXPORT_PLOT = 0;
LineWidth = 2;

folder = './';
filename = 'remote.csv';
file = [folder filename];

A = importdata(file);


figure;hold on;
% plot(A(:,1)); %time
% plot(A(:,1),A(:,2),'LineWidth',LineWidth); %mode
% figure; plot(A(:,1),A(:,3),'LineWidth',LineWidth);title('lift'); %lift
% figure; plot(A(:,1),A(:,4),'LineWidth',LineWidth);title('roll'); %roll
figure; plot(A(:,1),A(:,5),'LineWidth',LineWidth);title('pitch'); %pitch
% figure; plot(A(:,1),abs(A(:,6)),'LineWidth',LineWidth);title('yaw'); %yaw
% legend('lift','roll','pitch','yaw');
