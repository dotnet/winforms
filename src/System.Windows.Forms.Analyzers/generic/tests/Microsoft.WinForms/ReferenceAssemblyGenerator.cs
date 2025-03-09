// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;

internal static class ReferenceAssemblyGenerator
{
    private const string NetRuntimeIdentity = "Microsoft.NETCore.App.Ref";
    private const string NetDesktopIdentity = "Microsoft.WindowsDesktop.App.Ref";

    private static readonly Dictionary<NetVersion, (string tfm, string exactVersion)> s_exactNetVersionLookup = new()
    {
        [NetVersion.Net6_0]= ("net6.0","6.0.36"),
        [NetVersion.Net7_0] = ("net7.0", "7.0.20"),
        [NetVersion.Net8_0] = ("net8.0", "8.0.13"),
        [NetVersion.Net9_0] = ("net9.0", "9.0.2"),
        [NetVersion.Net10_0_Preview1] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview2] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview3] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview4] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview5] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0_Preview6] = ("net10.0", "10.0.0-preview.1.25080.5"),
        [NetVersion.Net10_0] = ("net10.0", "10.0.0-preview.1.25080.5"),
    };

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

            return netRequestedTFMAssemblies;
        }

        throw new ArgumentOutOfRangeException($"Reference assemblies for version {version} could not be loaded.");
    }

    public static IEnumerable<ReferenceAssemblies> GetForLatestTFMs(NetVersion[] versions)
    {
        foreach (var version in versions)
            yield return GetForLatestTFM(version);
    }
}
