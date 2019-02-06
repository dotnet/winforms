@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0\common\Build.ps1""" -restore -ci %*"
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0\renamebinlog.ps1""" %*"