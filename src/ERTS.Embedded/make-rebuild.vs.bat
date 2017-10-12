@echo off
rem use sysnative from within visual studio
C:\Windows\Sysnative\bash.exe -c 'make -C ../ERTS.Embedded clean'
IF ERRORLEVEL 1 GOTO errorHandling
C:\Windows\Sysnative\bash.exe -c 'make -C ../ERTS.Embedded'
:errorHandling
