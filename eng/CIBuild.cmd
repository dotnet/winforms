@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0\Build.ps1""" -restore -ci %*"
