@ECHO OFF
SETLOCAL

:: This command launches a Visual Studio Code with environment variables required to use a local version of the .NET Core SDK.

FOR /f "delims=" %%a IN ('where.exe code') DO @SET vscode=%%a& GOTO break
:break

IF ["%vscode%"] == [""] (
    echo [ERROR] Visual Studio Code is not installed or can't be found.
    exit /b 1
)

:: This tells .NET Core to use the same dotnet.exe that build scripts use
SET DOTNET_ROOT=%~dp0.dotnet
IF DEFINED DOTNET_GLOBAL_INSTALL_DIR SET "DOTNET_ROOT=%DOTNET_GLOBAL_INSTALL_DIR%"
SET "__DotNetSdkRoot="
FOR /f "delims=" %%r IN ('powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0eng\dotnet-architecture.ps1" -ResolveSdkRoot "%DOTNET_ROOT%"') DO SET "__DotNetSdkRoot=%%r"
IF NOT DEFINED __DotNetSdkRoot EXIT /b 1
SET "DOTNET_ROOT=%__DotNetSdkRoot%"
SET "DOTNET_GLOBAL_INSTALL_DIR=%DOTNET_ROOT%"
SET "__DotNetSdkRoot="

:: Put our local dotnet.exe on PATH first so Visual Studio knows which one to use
SET PATH=%DOTNET_ROOT%;%PATH%

IF NOT EXIST "%DOTNET_ROOT%\dotnet.exe" (
    echo [ERROR] .NET has not yet been installed. Run `%~dp0restore.cmd` to install tools
    exit /b 1
)

FOR /f "delims=" %%r IN ('powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0eng\dotnet-architecture.ps1" -ResolveSdkRoot "%DOTNET_ROOT%" -ResolveRuntimeRoots') DO SET "%%r"
SET "DOTNET_ROOT(x86)=%DOTNET_ROOT_X86%"

"%vscode%" "."