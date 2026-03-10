// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Versioning;

/// <summary>
///  Base type for all platform-specific API attributes.
/// </summary>
internal abstract class OSPlatformAttribute : Attribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="OSPlatformAttribute"/> class for the specified platform.
    /// </summary>
    /// <param name="platformName">The name of the platform with an optional version.</param>
    private protected OSPlatformAttribute(string platformName)
    {
        PlatformName = platformName;
    }

    /// <summary>
    ///  Gets the name of the platform that the attribute applies to with an optional version.
    /// </summary>
    public string PlatformName { get; }
}

/// <summary>
///  Records the platform that the project targeted.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly,
                AllowMultiple = false, Inherited = false)]
internal sealed class TargetPlatformAttribute : OSPlatformAttribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="TargetPlatformAttribute"/> class with the specified platform name.
    /// </summary>
    /// <param name="platformName">The name of the platform that the project targets.</param>
    public TargetPlatformAttribute(string platformName) : base(platformName)
    {
    }
}

/// <summary>
///  Records the operating system (and minimum version) that supports an API. Multiple attributes can be
///  applied to indicate support on multiple operating systems.
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
internal sealed class SupportedOSPlatformAttribute : OSPlatformAttribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="SupportedOSPlatformAttribute"/> class for the specified supported OS platform.
    /// </summary>
    /// <param name="platformName">The name of the supported OS platform with an optional version (e.g., "windows10.0").</param>
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
internal sealed class UnsupportedOSPlatformAttribute : OSPlatformAttribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="UnsupportedOSPlatformAttribute"/> class for the specified unsupported OS platform.
    /// </summary>
    /// <param name="platformName">The name of the unsupported OS platform with an optional version.</param>
    public UnsupportedOSPlatformAttribute(string platformName) : base(platformName)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="UnsupportedOSPlatformAttribute"/> class for the specified unsupported OS platform
    ///  with a descriptive message.
    /// </summary>
    /// <param name="platformName">The name of the unsupported OS platform with an optional version.</param>
    /// <param name="message">An optional message associated with the unsupported platform.</param>
    public UnsupportedOSPlatformAttribute(string platformName, string? message) : base(platformName)
    {
        Message = message;
    }

    /// <summary>
    ///  Gets the optional message associated with the unsupported platform.
    /// </summary>
    public string? Message { get; }
}

/// <summary>
///  Marks APIs that were obsoleted in a given operating system version.
/// </summary>
/// <remarks>
///  <para>
///   Primarily used by OS bindings to indicate APIs that should not be used anymore.
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
internal sealed class ObsoletedOSPlatformAttribute : OSPlatformAttribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="ObsoletedOSPlatformAttribute"/> class with the specified platform name.
    /// </summary>
    /// <param name="platformName">The name of the platform where the API was obsoleted.</param>
    public ObsoletedOSPlatformAttribute(string platformName) : base(platformName)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="ObsoletedOSPlatformAttribute"/> class with the specified platform name and message.
    /// </summary>
    /// <param name="platformName">The name of the platform where the API was obsoleted.</param>
    /// <param name="message">The message that explains the obsolescence.</param>
    public ObsoletedOSPlatformAttribute(string platformName, string? message) : base(platformName)
    {
        Message = message;
    }

    /// <summary>
    ///  Gets the message that explains the obsolescence.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    ///  Gets or sets the URL that provides more information about the obsolescence.
    /// </summary>
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
internal sealed class SupportedOSPlatformGuardAttribute : OSPlatformAttribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="SupportedOSPlatformGuardAttribute"/> class with the specified supported OS platform.
    /// </summary>
    /// <param name="platformName">The name of the supported OS platform that the guard field, property, or method checks.</param>
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
///   and use that  field, property or method sin a conditional or assert statements as a guard to safely call APIs unsupported on those platforms.
///  </para>
///  <para>
///   The type of the field or property should be boolean, the method return type should be boolean in order to be used as platform guard.
///  </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field |
                AttributeTargets.Method |
                AttributeTargets.Property,
                AllowMultiple = true, Inherited = false)]
internal sealed class UnsupportedOSPlatformGuardAttribute : OSPlatformAttribute
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="UnsupportedOSPlatformGuardAttribute"/> class with the specified unsupported OS platform.
    /// </summary>
    /// <param name="platformName">The name of the unsupported OS platform that the guard field, property, or method checks.</param>
    public UnsupportedOSPlatformGuardAttribute(string platformName) : base(platformName)
    {
    }
}
