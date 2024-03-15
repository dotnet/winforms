// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Versioning;

/// <summary>
/// Base type for all platform-specific API attributes.
/// </summary>
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    abstract class OSPlatformAttribute : Attribute
{
    private protected OSPlatformAttribute(string platformName)
    {
        PlatformName = platformName;
    }

    public string PlatformName { get; }
}

/// <summary>
/// Records the platform that the project targeted.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly,
                AllowMultiple = false, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    sealed class TargetPlatformAttribute : OSPlatformAttribute
{
    public TargetPlatformAttribute(string platformName) : base(platformName)
    {
    }
}

/// <summary>
/// Records the operating system (and minimum version) that supports an API. Multiple attributes can be
/// applied to indicate support on multiple operating systems.
/// </summary>
/// <remarks>
///  <para>
///   Callers can apply a <see cref="SupportedOSPlatformAttribute " />
///   or use guards to prevent calls to APIs on unsupported operating systems.
///  </para>
///  <para>
///   A given platform should only be specified once.
///  </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly |
                AttributeTargets.Class |
                AttributeTargets.Constructor |
                AttributeTargets.Enum |
                AttributeTargets.Event |
                AttributeTargets.Field |
                AttributeTargets.Interface |
                AttributeTargets.Method |
                AttributeTargets.Module |
                AttributeTargets.Property |
                AttributeTargets.Struct,
                AllowMultiple = true, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    sealed class SupportedOSPlatformAttribute : OSPlatformAttribute
{
    public SupportedOSPlatformAttribute(string platformName) : base(platformName)
    {
    }
}

/// <summary>
///  Marks APIs that were removed in a given operating system version.
/// </summary>
/// <remarks>
///  <para>
///   Primarily used by OS bindings to indicate APIs that are only available in
///   earlier versions.
///  </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly |
                AttributeTargets.Class |
                AttributeTargets.Constructor |
                AttributeTargets.Enum |
                AttributeTargets.Event |
                AttributeTargets.Field |
                AttributeTargets.Interface |
                AttributeTargets.Method |
                AttributeTargets.Module |
                AttributeTargets.Property |
                AttributeTargets.Struct,
                AllowMultiple = true, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    sealed class UnsupportedOSPlatformAttribute : OSPlatformAttribute
{
    public UnsupportedOSPlatformAttribute(string platformName) : base(platformName)
    {
    }

    public UnsupportedOSPlatformAttribute(string platformName, string? message) : base(platformName)
    {
        Message = message;
    }

    public string? Message { get; }
}

/// <summary>
///  Marks APIs that were obsoleted in a given operating system version.
/// </summary>
/// <remarks>
///  <para>Primarily used by OS bindings to indicate APIs that should not be used anymore.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly |
                AttributeTargets.Class |
                AttributeTargets.Constructor |
                AttributeTargets.Enum |
                AttributeTargets.Event |
                AttributeTargets.Field |
                AttributeTargets.Interface |
                AttributeTargets.Method |
                AttributeTargets.Module |
                AttributeTargets.Property |
                AttributeTargets.Struct,
                AllowMultiple = true, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    sealed class ObsoletedOSPlatformAttribute : OSPlatformAttribute
{
    public ObsoletedOSPlatformAttribute(string platformName) : base(platformName)
    {
    }

    public ObsoletedOSPlatformAttribute(string platformName, string? message) : base(platformName)
    {
        Message = message;
    }

    public string? Message { get; }
    public string? Url { get; set; }
}

/// <summary>
///  Annotates a custom guard field, property or method with a supported platform name and optional version.
///  Multiple attributes can be applied to indicate guard for multiple supported platforms.
/// </summary>
/// <remarks>
///  <para>
///   Callers can apply a <see cref="SupportedOSPlatformGuardAttribute " /> to a field, property or method
///   and use that field, property or method in a conditional or assert statements in order to safely call platform specific APIs.
///  </para>
///  <para>
///   The type of the field or property should be boolean, the method return type should be boolean in order to be used as platform guard.
///  </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field |
                AttributeTargets.Method |
                AttributeTargets.Property,
                AllowMultiple = true, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    sealed class SupportedOSPlatformGuardAttribute : OSPlatformAttribute
{
    public SupportedOSPlatformGuardAttribute(string platformName) : base(platformName)
    {
    }
}

/// <summary>
///  Annotates the custom guard field, property or method with an unsupported platform name and optional version.
///  Multiple attributes can be applied to indicate guard for multiple unsupported platforms.
/// </summary>
/// <remarks>
///  <para>
///   Callers can apply a <see cref="UnsupportedOSPlatformGuardAttribute " /> to a field, property or method
///   and use that  field, property or method in a conditional or assert statements as a guard to safely call APIs unsupported on those platforms.
///  </para>
///  <para>
///   The type of the field or property should be boolean, the method return type should be boolean in order to be used as platform guard.
///  </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field |
                AttributeTargets.Method |
                AttributeTargets.Property,
                AllowMultiple = true, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
public
#else
internal
#endif
    sealed class UnsupportedOSPlatformGuardAttribute : OSPlatformAttribute
{
    public UnsupportedOSPlatformGuardAttribute(string platformName) : base(platformName)
    {
    }
}
