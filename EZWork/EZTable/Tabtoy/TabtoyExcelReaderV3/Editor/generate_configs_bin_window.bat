@echo off
cd /d %~dp0
if exist "%windir%\SysWOW64" (
	set bit=64
) else (
	set bit=32
)

cd %6

if %4=="null" (
	if %5=="null" (
		tabtoy\windows_x%bit%\tabtoy --mode=v3 --combinename=%1 --csharp_out=%2 -binary_out=%3
	) else (
		tabtoy\windows_x%bit%\tabtoy --mode=v3 --combinename=%1 --csharp_out=%2 -binary_out=%3 -lua_out=%5
	)
) else (
	if %5=="null" (
		tabtoy\windows_x%bit%\tabtoy --mode=v3 --combinename=%1 --csharp_out=%2 -binary_out=%3 -json_out=%4
	) else (
		tabtoy\windows_x%bit%\tabtoy --mode=v3 --combinename=%1 --csharp_out=%2 -binary_out=%3 -json_out=%4 -lua_out=%5
	)
)