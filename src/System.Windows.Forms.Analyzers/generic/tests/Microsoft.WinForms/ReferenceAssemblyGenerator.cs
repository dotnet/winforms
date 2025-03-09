// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;

using Microsoft.CodeAnalysis.Testing;

/// <summary>
///  Provides utility methods for generating reference assemblies for specific .NET versions.
/// </summary>
/// <remarks>
///  <para>
///   This class maintains a mapping of .NET versions to their corresponding Target Framework Monikers (TFMs)
///   and exact version numbers. It provides methods to generate appropriate reference assemblies for tests
///   targeting specific .NET runtime versions.
///  </para>
///  <para>
///   It supports both runtime packages (Microsoft.NETCore.App.Ref) and desktop packages 
///   (Microsoft.WindowsDesktop.App.Ref) to ensure tests have access to all required references.
///  </para>
/// </remarks>
internal static class ReferenceAssemblyGenerator
{
    private const string NetRuntimeIdentity = "Microsoft.NETCore.App.Ref";
    private const string NetDesktopIdentity = "Microsoft.WindowsDesktop.App.Ref";

    private static readonly Dictionary<NetVersion, (string tfm, string exactVersion)> s_exactNetVersionLookup = new()
    {
        [NetVersion.Net6_0] = ("net6.0", "6.0.36"),
        [NetVersion.Net7_0] = ("net7.0", "7.0.20"),
        [NetVersion.Net8_0] = ("net8.0", "8.0.13"),
        [NetVersion.Net9_0] = ("net9.0", "9.0.2"),
        [NetVersion.Net10_0_Preview1] = ("net10.0-preview1", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview2] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview3] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview4] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview5] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview6] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0] = ("net10.0", "10.0.0-preview.1.25080.5"),
    };

    /// <summary>
    ///  Gets a reference assembly configuration for a specified .NET version.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method retrieves the appropriate TFM and exact version number for the specified .NET version,
    ///   then creates a reference assembly configuration including both runtime and desktop packages.
    ///  </para>
    /// </remarks>
    /// <param name="version">
    ///  The .NET version to get reference assemblies for.
    /// </param>
    /// <returns>
    ///  A <see cref="ReferenceAssemblies"/> instance configured for the specified .NET version.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if reference assemblies for the specified version are not available.
    /// </exception>
    public static ReferenceAssemblies GetForLatestTFM(NetVersion version)
    {
        if (s_exactNetVersionLookup.TryGetValue(version, out (string tfm, string exactVersion) value))
        {
            var netRuntimePackage = new PackageIdentity(NetRuntimeIdentity, value.exactVersion);
            var netDesktopPackage = new PackageIdentity(NetDesktopIdentity, value.exactVersion);

            ReferenceAssemblies netRequestedTFMAssemblies = new ReferenceAssemblies(
                value.tfm,
                netRuntimePackage,
                $"ref\\{value.tfm}");

            netRequestedTFMAssemblies = netRequestedTFMAssemblies.WithPackages([netDesktopPackage]);

            netRequestedTFMAssemblies.ResolveAsync(string.Empty,CancellationToken.None).Wait();

            return netRequestedTFMAssemblies;
        }

        throw new ArgumentOutOfRangeException($"Reference assemblies for version {version} could not be loaded.");
    }

    /// <summary>
    ///  Gets reference assembly configurations for multiple .NET versions.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method provides an enumerable of reference assembly configurations for each 
    ///   specified .NET version by calling <see cref="GetForLatestTFM(NetVersion)"/> for each version.
    ///  </para>
    /// </remarks>
    /// <param name="versions">
    ///  An array of .NET versions to get reference assemblies for.
    /// </param>
    /// <returns>
    ///  An enumerable of <see cref="ReferenceAssemblies"/> instances, one for each specified version.
    /// </returns>
    public static IEnumerable<ReferenceAssemblies> GetForLatestTFMs(NetVersion[] versions)
    {
        foreach (var version in versions)
            yield return GetForLatestTFM(version);
    }
}
