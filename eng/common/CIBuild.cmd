@echo off
:: TODO: Add -test after -build once we verify helix is working
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0Build.ps1""" -restore -build -sign -pack -publish -ci %*"
exit /b %ErrorLevel%
