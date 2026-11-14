@echo off
powershell -ExecutionPolicy ByPass -NoProfile -File "%~dp0eng\build.cmd.ps1" %*
exit /b %ErrorLevel%
