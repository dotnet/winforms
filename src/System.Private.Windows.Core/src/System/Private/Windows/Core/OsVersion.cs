// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System.Private.Windows.Core;

internal static class OsVersion
{
    /// <summary>
    ///  Is Windows 10 first release or later. (Threshold 1, build 10240, version 1507)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows10.0.10240")]
    public static bool IsWindows10_1507OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 10240);

    /// <summary>
    ///  Is Windows 10 Anniversary Update or later. (Redstone 1, build 14393, version 1607)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows10.0.14393")]
    public static bool IsWindows10_1607OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 14393);

    /// <summary>
    ///  Is Windows 10 Creators Update or later. (Redstone 2, build 15063, version 1703)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows10.0.15063")]
    public static bool IsWindows10_1703OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 15063);

    /// <summary>
    ///  Is Windows 10 Creators Update or later. (Redstone 3, build 16299, version 1709)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows10.0.16299")]
    public static bool IsWindows10_1709OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 16299);

    /// <summary>
    ///  Is Windows 10 Creators Update or later. (Redstone 4, build 17134, version 1803)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows10.0.17134")]
    public static bool IsWindows10_18030rGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 17134);

    /// <summary>
    ///  Is this Windows 11 public preview or later?
    ///  The underlying API does not read supportedOs from the manifest, it returns the actual version.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows11")]
    public static bool IsWindows11_OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 22000);

    /// <summary>
    ///  Is this Windows 11 version 22H2 or greater?
    ///  The underlying API does not read supportedOs from the manifest, it returns the actual version.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows11.0.22621")]
    public static bool IsWindows11_22H2OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 10, build: 22621);

    /// <summary>
    ///  Is Windows 8.1 or later.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows8.1")]
    public static bool IsWindows8_1OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 6, minor: 3);

    /// <summary>
    ///  Is Windows 8 or later.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SupportedOSPlatformGuard("windows8")]
    public static bool IsWindows8OrGreater() => OperatingSystem.IsWindowsVersionAtLeast(major: 6, minor: 2);
}
