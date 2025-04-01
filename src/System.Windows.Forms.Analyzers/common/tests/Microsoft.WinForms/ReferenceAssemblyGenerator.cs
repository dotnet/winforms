// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
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
internal static partial class ReferenceAssemblyGenerator
{
    private const string NetRuntimeIdentity = "Microsoft.NETCore.App.Ref";
    private const string NetDesktopIdentity = "Microsoft.WindowsDesktop.App.Ref";

    private static readonly Dictionary<NetVersion, (string tfm, string exactVersion)> s_exactNetVersionLookup = new()
    {
        [NetVersion.Net6_0] = ("net6.0", "6.0.36"),
        [NetVersion.Net7_0] = ("net7.0", "7.0.20"),
        [NetVersion.Net8_0] = ("net8.0", "8.0.13"),
        [NetVersion.Net9_0] = ("net9.0", "9.0.2")
    };

    private static readonly List<string> s_winFormsAssemblies = new()
    {
        "Accessibility.dll",
        "Microsoft.VisualBasic.dll",
        "Microsoft.VisualBasic.Forms.dll",
        "System.Design.dll",
        "System.Drawing.Common.dll",
        "System.Drawing.Design.dll",
        "System.Drawing.dll",
        "System.Private.Windows.Core.dll",
        "System.Windows.Forms.Design.dll",
        "System.Windows.Forms.Design.Editors.dll",
        "System.Windows.Forms.dll",
        "System.Windows.Forms.Primitives.dll"
    };

    /// <summary>
    ///  Modifies each entry in the s_winFormsAssemblies list to include the provided folder name.
    /// </summary>
    /// <param name="profile">The folder name to include in each entry.</param>
    public static ImmutableArray<string> GetWinFormsBuildAssemblies(string tfm, string profile)
    {
        var winFormsAssembliesBuilder =
            ImmutableArray.CreateBuilder<string>(s_winFormsAssemblies.Count);

        // Microsoft.VisualBasic.Forms is at the top of the reference chain,
        // so it has all the references we need. But we assert all the assemblies
        // we need in addition to that.
        string fullAssemblyPath = Path.Join(
            WinFormsReferencesFactory.RepoRootPath,
            "artifacts\\bin");

        // Check, if s_winFormsAssemblies has all the assemblies in the folder:
        foreach (string file in FindAssembliesInSubfolders(fullAssemblyPath, tfm, profile, s_winFormsAssemblies))
        {
            winFormsAssembliesBuilder.Add(file);
        }

        return winFormsAssembliesBuilder.ToImmutable();
    }

    private static ImmutableArray<string> FindAssembliesInSubfolders(
        string root, string tfm, string profile, IEnumerable<string> assemblies)
    {
        var foundAssembliesBuilder = ImmutableArray.CreateBuilder<string>();
    
        foreach (string assembly in assemblies)
        {
            // Extract the base assembly folder name (without .dll extension)
            string assemblyName = Path.GetFileNameWithoutExtension(assembly);
        
            // Get all directories at the root level
            string[] possibleAssemblyFolders = Directory.GetDirectories(root);
        
            foreach (string assemblyFolder in possibleAssemblyFolders)
            {
                // Check if the expected profile and tfm subdirectory path exists
                string pathToCheck = Path.Combine(assemblyFolder, profile, tfm);

                if (!Directory.Exists(pathToCheck))
                {
                    continue;
                }
            
                // Look for the matching assembly file in this directory
                string assemblyPath = Path.Combine(pathToCheck, assembly);

                if (File.Exists(assemblyPath))
                {
                    foundAssembliesBuilder.Add(assemblyPath);
                    break; // Found the assembly, no need to check other folders
                }
            }
        }
    
        return foundAssembliesBuilder.ToImmutable();
    }

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
        if (version.HasFlag(NetVersion.WinFormsBuild))
        {
            ReferenceAssemblies netRequestedTFMAssemblies =
                WinFormsReferencesFactory.NetCoreAppReferences
                    ?? throw new NotSupportedException(
                        "The reference assemblies for the .NET Version based on the " +
                        "latest WinForms build couldn't be retrieved.");

            ReferenceAssemblies netCoreAppReferences = WinFormsReferencesFactory.NetCoreAppReferences
                ?? throw new NotSupportedException(
                    "The AppCore reference assemblies for the .NET Version based on the " +
                    "latest WinForms build couldn't be retrieved.");

#if DEBUG
            string profile = "debug";
#else
            string profile = "release";
#endif

            netRequestedTFMAssemblies =
                netRequestedTFMAssemblies.AddAssemblies(
                    GetWinFormsBuildAssemblies(
                        tfm: netRequestedTFMAssemblies.TargetFramework,
                        profile: profile));

            return netRequestedTFMAssemblies;
        }

        (string tfm, string exactVersion) value;

        if (version.HasFlag(NetVersion.BuildOutput))
        {
            version = version & ~NetVersion.BuildOutput;

            if (s_exactNetVersionLookup.TryGetValue(version, out value))
            {
                var netRuntimePackage = new PackageIdentity(NetRuntimeIdentity, value.exactVersion);

                ReferenceAssemblies netRequestedTFMAssemblies = new ReferenceAssemblies(
                    targetFramework: value.tfm,
                    referenceAssemblyPackage: netRuntimePackage,
                    referenceAssemblyPath: $"ref\\{value.tfm}");

                ReferenceAssemblies netCoreAppReferences = WinFormsReferencesFactory.NetCoreAppReferences
                    ?? throw new NotSupportedException(
                        "The AppCore reference assemblies for the .NET Version based on the " +
                        "latest WinForms build couldn't be retrieved.");

                return netRequestedTFMAssemblies;
            }
        }

        if (s_exactNetVersionLookup.TryGetValue(version, out value))
        {
            var netRuntimePackage = new PackageIdentity(NetRuntimeIdentity, value.exactVersion);
            var netDesktopPackage = new PackageIdentity(NetDesktopIdentity, value.exactVersion);

            ReferenceAssemblies netRequestedTFMAssemblies = new ReferenceAssemblies(
                targetFramework: value.tfm,
                referenceAssemblyPackage: netRuntimePackage,
                referenceAssemblyPath: $"ref\\{value.tfm}");

            netRequestedTFMAssemblies = netRequestedTFMAssemblies.WithPackages([netDesktopPackage]);

            netRequestedTFMAssemblies.ResolveAsync(string.Empty, CancellationToken.None).Wait();

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
