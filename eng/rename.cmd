@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0\rename.ps1""" %*"