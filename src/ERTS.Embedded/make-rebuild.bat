@echo off
bash.exe -c 'make -C ../ERTS.Embedded clean'
IF ERRORLEVEL 1 GOTO errorHandling
bash.exe -c 'make -C ../ERTS.Embedded'
:errorHandling