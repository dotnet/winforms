// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Configuration;
using System.Drawing.Configuration;
using System.Reflection;

namespace System.Drawing;

/// <summary>
///  Provides methods to select from multiple bitmaps depending on a "bitmapSuffix" config setting.
/// </summary>
internal static class BitmapSelector
{
    private static string? s_suffix;

    /// <summary>
    ///  Gets the bitmap ID suffix defined in the application configuration, or string.Empty if
    ///  the suffix is not specified.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   For performance, the suffix is cached in a static variable so it only has to be read
    ///   once per <see cref="AppDomain"/>.
    ///  </para>
    /// </remarks>
    private static string Suffix =>
        s_suffix ??= ConfigurationManager.GetSection("system.drawing") is SystemDrawingSection section && section.BitmapSuffix is string suffix
            ? suffix
            : string.Empty;

    /// <summary>
    ///  Appends the current suffix to <paramref name="filePath"/>. The suffix is appended
    ///  before the existing extension (if any).
    /// </summary>
    private static string AppendSuffix(string filePath)
        => string.IsNullOrEmpty(Suffix) ? filePath : Path.ChangeExtension(filePath, Suffix + Path.GetExtension(filePath));

    /// <summary>
    ///  Returns <paramref name="originalPath"/> with the current suffix appended (before the
    ///  existing extension) if the resulting file path exists; otherwise the original path is
    ///  returned.
    /// </summary>
    public static string GetFileName(string originalPath)
    {
        if (Suffix == string.Empty)
            return originalPath;

        string newPath = AppendSuffix(originalPath);
        return File.Exists(newPath) ? newPath : originalPath;
    }

    // Calls assembly.GetManifestResourceStream in a try/catch and returns null if not found
    private static Stream? GetResourceStreamHelper(Assembly assembly, Type type, string name)
    {
        Stream? stream = null;
        try
        {
            stream = assembly.GetManifestResourceStream(type, name);
        }
        catch (FileNotFoundException)
        {
        }

        return stream;
    }

    private static bool DoesAssemblyHaveCustomAttribute(Assembly assembly, string typeName) =>
        assembly.GetType(typeName) is Type type && DoesAssemblyHaveCustomAttribute(assembly, type);

    private static bool DoesAssemblyHaveCustomAttribute(Assembly assembly, Type attributeType) =>
        assembly.GetCustomAttributes(attributeType, inherit: false).Length > 0;

    private static bool SatelliteAssemblyOptIn(Assembly assembly)
    {
        // Try 4.5 public attribute type first
        if (DoesAssemblyHaveCustomAttribute(assembly, typeof(BitmapSuffixInSatelliteAssemblyAttribute)))
        {
            return true;
        }

        // Also load attribute type by name for dlls compiled against older frameworks
        return DoesAssemblyHaveCustomAttribute(assembly, "System.Drawing.BitmapSuffixInSatelliteAssemblyAttribute");
    }

    private static bool SameAssemblyOptIn(Assembly assembly)
    {
        // Try 4.5 public attribute type first
        if (DoesAssemblyHaveCustomAttribute(assembly, typeof(BitmapSuffixInSameAssemblyAttribute)))
        {
            return true;
        }

        // Also load attribute type by name for dlls compiled against older frameworks
        return DoesAssemblyHaveCustomAttribute(assembly, "System.Drawing.BitmapSuffixInSameAssemblyAttribute");
    }

    /// <summary>
    ///  Returns a resource stream loaded from the appropriate location according to the current suffix.
    /// </summary>
    /// <param name="type">The type whose namespace is used to scope the manifest resource name</param>
    /// <param name="originalName">The name of the manifest resource being requested</param>
    /// <returns>
    ///  The manifest resource stream corresponding to <paramref name="originalName"/> with the
    ///  current suffix applied; or if that is not found, the stream corresponding to <paramref name="originalName"/>.
    /// </returns>
    internal static Stream? GetResourceStream(Type type, string originalName)
    {
        Assembly assembly = type.Module.Assembly;

        if (Suffix != string.Empty)
        {
            try
            {
                // Resource with suffix has highest priority
                if (SameAssemblyOptIn(assembly))
                {
                    string newName = AppendSuffix(originalName);
                    Stream? stream = GetResourceStreamHelper(assembly, type, newName);
                    if (stream is not null)
                    {
                        return stream;
                    }
                }
            }
            catch
            {
                // Ignore failures and continue to try other options
            }

            try
            {
                // Satellite assembly has second priority, using the original name
                if (SatelliteAssemblyOptIn(assembly))
                {
                    AssemblyName assemblyName = assembly.GetName();
                    assemblyName.Name += Suffix;
#pragma warning disable SYSLIB0037 // Type or member is obsolete
                    assemblyName.ProcessorArchitecture = ProcessorArchitecture.None;
#pragma warning restore SYSLIB0037

                    if (Assembly.Load(assemblyName) is { } satellite)
                    {
                        Stream? stream = GetResourceStreamHelper(satellite, type, originalName);
                        if (stream is not null)
                        {
                            return stream;
                        }
                    }
                }
            }
            catch
            {
                // Ignore failures and continue to try other options
            }
        }

        // Otherwise fall back to specified assembly and original name requested
        return assembly.GetManifestResourceStream(type, originalName);
    }
}
