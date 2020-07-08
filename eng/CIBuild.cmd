@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0\custom-cibuild.ps1""" -ci -preparemachine %*"
