---
name: download-sdk
description: >-
  Instructions for downloading and installing .NET preview runtime versions
  required by the WinForms repository. Use when test executables fail with
  "framework not found" errors or when a specific .NET preview runtime
  version needs to be installed.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Downloading and Installing .NET Preview Runtimes

The WinForms repository targets **.NET preview** builds. Test executables and
built assemblies require a matching runtime version that may not be publicly
available on the official .NET download page. Use the internal CI feed URL
pattern below to download and install the exact version needed.

---

## 1  Determining the Required Version

The required runtime version can be found in multiple ways:

### From an error message

When a test executable cannot find its target framework, the error includes the
version:

```
Framework: 'Microsoft.NETCore.App', version '11.0.0-preview.4.26203.108' (x64)
```

### From runtimeconfig.json (programmatic — preferred)

Each test executable has a `.runtimeconfig.json` next to it that declares the
exact version required:

```powershell
$rc = Get-Content "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.runtimeconfig.json" | ConvertFrom-Json
$version = $rc.runtimeOptions.framework.version
Write-Host "Required runtime: $version"
```

---

## 2  Download URL Pattern

Use the following base URL, replacing `{version}` with the full version string:

### x64 (64-bit) — required for standard test runs

```
https://ci.dot.net/public/Runtime/{version}/dotnet-runtime-{version}-win-x64.msi
```

### x86 (32-bit) — required for 32-bit test runs

```
https://ci.dot.net/public/Runtime/{version}/dotnet-runtime-{version}-win-x86.msi
```

### Example

For version `11.0.0-preview.4.26203.108`:

```
x64: https://ci.dot.net/public/Runtime/11.0.0-preview.4.26203.108/dotnet-runtime-11.0.0-preview.4.26203.108-win-x64.msi
x86: https://ci.dot.net/public/Runtime/11.0.0-preview.4.26203.108/dotnet-runtime-11.0.0-preview.4.26203.108-win-x86.msi
```

---

## 3  Download and Install (PowerShell)

```powershell
$version = "11.0.0-preview.4.26203.108"   # ← replace with needed version

# Download both architectures
$url64 = "https://ci.dot.net/public/Runtime/$version/dotnet-runtime-$version-win-x64.msi"
$url86 = "https://ci.dot.net/public/Runtime/$version/dotnet-runtime-$version-win-x86.msi"
$msi64 = "$env:TEMP\dotnet-runtime-$version-win-x64.msi"
$msi86 = "$env:TEMP\dotnet-runtime-$version-win-x86.msi"

Invoke-WebRequest -Uri $url64 -OutFile $msi64 -UseBasicParsing
Invoke-WebRequest -Uri $url86 -OutFile $msi86 -UseBasicParsing

# Install (requires elevation)
Start-Process msiexec.exe -ArgumentList "/i `"$msi64`" /quiet /norestart" -Wait -Verb RunAs
Start-Process msiexec.exe -ArgumentList "/i `"$msi86`" /quiet /norestart" -Wait -Verb RunAs

# Verify installation
& "C:\Program Files\dotnet\dotnet.exe" --list-runtimes | Select-String $version

# Clean up
Remove-Item $msi64, $msi86 -ErrorAction SilentlyContinue
```

> **Important:** The `-Verb RunAs` flag triggers a UAC elevation prompt.
> Without it, the MSI install silently fails with exit code **1603**.

---

## 4  Verifying the Installation

After installation, verify the runtime is available:

```powershell
# x64
& "C:\Program Files\dotnet\dotnet.exe" --list-runtimes | Select-String $version

# x86
& "C:\Program Files (x86)\dotnet\dotnet.exe" --list-runtimes | Select-String $version

# Or check the shared framework directory directly
Get-ChildItem "C:\Program Files\dotnet\shared\Microsoft.NETCore.App" -Filter "$($version.Split('-')[0])*"
Get-ChildItem "C:\Program Files (x86)\dotnet\shared\Microsoft.NETCore.App" -Filter "$($version.Split('-')[0])*"
```

---

## 5  Fully Automated: Detect, Download, and Install

This script reads the required version from any test executable's
`runtimeconfig.json`, downloads both architectures, and installs them:

```powershell
# Auto-detect version from the first test runtimeconfig found
$rc = Get-ChildItem "artifacts\bin\*Tests*\Debug\*\*.runtimeconfig.json" |
    Select-Object -First 1
$version = (Get-Content $rc | ConvertFrom-Json).runtimeOptions.framework.version
Write-Host "Required runtime: $version"

# Check if already installed
$installed = & "C:\Program Files\dotnet\dotnet.exe" --list-runtimes 2>$null |
    Select-String ([regex]::Escape($version))
if ($installed) {
    Write-Host "Runtime $version is already installed."
    return
}

# Download
$base = "https://ci.dot.net/public/Runtime/$version"
$msi64 = "$env:TEMP\dotnet-runtime-$version-win-x64.msi"
$msi86 = "$env:TEMP\dotnet-runtime-$version-win-x86.msi"
Invoke-WebRequest "$base/dotnet-runtime-$version-win-x64.msi" -OutFile $msi64 -UseBasicParsing
Invoke-WebRequest "$base/dotnet-runtime-$version-win-x86.msi" -OutFile $msi86 -UseBasicParsing

# Install (elevated)
Start-Process msiexec.exe -ArgumentList "/i `"$msi64`" /quiet /norestart" -Wait -Verb RunAs
Start-Process msiexec.exe -ArgumentList "/i `"$msi86`" /quiet /norestart" -Wait -Verb RunAs

# Verify
& "C:\Program Files\dotnet\dotnet.exe" --list-runtimes | Select-String $version

# Clean up
Remove-Item $msi64, $msi86 -ErrorAction SilentlyContinue
```

---

## 6  Alternative: Use the Repo-Local Runtime

The repository includes a local `.dotnet` folder (populated by `.\Restore.cmd`)
that may already have the required runtime. To use it instead of installing
system-wide:

```powershell
# From the repository root:
$env:DOTNET_ROOT = "$PWD\.dotnet"
$env:PATH = "$PWD\.dotnet;$env:PATH"
```

Check available versions:

```powershell
Get-ChildItem ".dotnet\shared\Microsoft.NETCore.App" | Select Name
```

> **Note:** The repo-local runtime works for running test executables but may
> not be picked up by `vstest.console.exe` or other external tools.

---

## 7  Troubleshooting

| Problem | Solution |
|---------|----------|
| MSI install fails with exit code **1603** | Run with `-Verb RunAs` for admin elevation |
| MSI install exits **0** but runtime not listed | The install ran without elevation and failed silently — retry with `-Verb RunAs` |
| Download returns **404** | Double-check the version string; the runtime may not be published to the CI feed yet |
| Test exe still says "framework not found" after install | Ensure the **architecture** matches (x64 vs x86); check that the test exe is looking for `Microsoft.NETCore.App` (not `Microsoft.WindowsDesktop.App` or `Microsoft.AspNetCore.App`) |
| Need `Microsoft.WindowsDesktop.App` | Replace `Runtime` with `WindowsDesktop` in the URL: `https://ci.dot.net/public/WindowsDesktop/{version}/windowsdesktop-runtime-{version}-win-x64.msi` |
