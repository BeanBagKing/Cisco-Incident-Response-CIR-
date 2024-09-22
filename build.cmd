echo off
if [%1]==[] goto help
.\ThirdParty\Build\nant-0.85\bin\NAnt %1
goto end

:help
@echo give build target as single argument
@echo example: build.cmd dist-CIR
@echo   for the Recurity Labs build
:end