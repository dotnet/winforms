@echo off
setlocal enabledelayedexpansion

:: This command launches a Visual Studio solution with environment variables required to use a local version of the .NET Core SDK.

:: This tells .NET Core to use the same dotnet.exe that build scripts use
set DOTNET_ROOT=%~dp0.dotnet
if defined DOTNET_GLOBAL_INSTALL_DIR set "DOTNET_ROOT=%DOTNET_GLOBAL_INSTALL_DIR%"
set "__DotNetSdkRoot="
for /f "delims=" %%r in ('powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0eng\dotnet-architecture.ps1" -ResolveSdkRoot "%DOTNET_ROOT%"') do set "__DotNetSdkRoot=%%r"
if not defined __DotNetSdkRoot exit /b 1
set "DOTNET_ROOT=%__DotNetSdkRoot%"
set "DOTNET_GLOBAL_INSTALL_DIR=%DOTNET_ROOT%"
set "__DotNetSdkRoot="

:: Put our local dotnet.exe on PATH first so Visual Studio knows which one to use
set PATH=%DOTNET_ROOT%;%PATH%

call restore.cmd
if errorlevel 1 exit /b %ErrorLevel%

if not exist "%DOTNET_ROOT%\dotnet.exe" (
    echo [ERROR] .NET Core has not yet been installed. Run `%~dp0restore.cmd` to install tools
    exit /b 1
)

for /f "delims=" %%r in ('powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0eng\dotnet-architecture.ps1" -ResolveSdkRoot "%DOTNET_ROOT%" -ResolveRuntimeRoots') do set "%%r"
set "DOTNET_ROOT(x86)=%DOTNET_ROOT_X86%"

:: Prefer the VS in the developer command prompt if we're in one, followed by whatever shows up in the current search path.
set "DEVENV=%DevEnvDir%devenv.exe"

if exist "%DEVENV%" (
    :: Fully qualified works
    set "COMMAND=start "" /B "%ComSpec%" /S /C ""%DEVENV%" "%~dp0Winforms.sln"""
) else (
    where devenv.exe /Q
    if !errorlevel! equ 0 (
        :: On the PATH, use that.
        set "COMMAND=start "" /B "%ComSpec%" /S /C "devenv.exe "%~dp0Winforms.sln"""
    ) else (
        :: Can't find devenv.exe, let file associations take care of it
        set "COMMAND=start /B .\Winforms.sln"
    )
)

%COMMAND%
