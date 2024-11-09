// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

public static partial class PlatformDetection
{
    //
    // Do not use the " { get; } = <expression> " pattern here. Having all the initialization happen in the type initializer
    // means that one exception anywhere means all tests using PlatformDetection fail. If you feel a value is worth latching,
    // do it in a way that failures don't cascade.
    //

    public static bool IsNotWindowsIoTCore => !IsWindowsIoTCore;
    public static bool IsArmProcess => RuntimeInformation.ProcessArchitecture == Architecture.Arm;
    public static bool IsArm64Process => RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
    public static bool IsNotArm64Process => !IsArm64Process;
    public static bool IsArmOrArm64Process => IsArmProcess || IsArm64Process;

    public static bool IsNativeAot => !IsReflectionEmitSupported;

    // Changed to `true` when trimming
    public static bool IsBuiltWithAggressiveTrimming => IsNativeAot;
    public static bool IsNotBuiltWithAggressiveTrimming => !IsBuiltWithAggressiveTrimming;

    private static readonly Lazy<bool> s_largeArrayIsNotSupported = new(IsLargeArrayNotSupported);

    [MethodImpl(MethodImplOptions.NoOptimization)]
    private static bool IsLargeArrayNotSupported()
    {
        try
        {
            byte[] tmp = new byte[int.MaxValue];
            return false;
        }
        catch (OutOfMemoryException)
        {
            return true;
        }
    }

    public static bool IsNotIntMaxValueArrayIndexSupported => s_largeArrayIsNotSupported.Value;

    public static bool IsReflectionEmitSupported => RuntimeFeature.IsDynamicCodeSupported;
}
