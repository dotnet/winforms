@echo off
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& """%~dp0run_individual_exe.ps1""" "
exit /b %ErrorLevel%