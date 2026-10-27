// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Provides access to the Microsoft.NETCore.App.Ref package this repo is built against.
///  By default Roslyn SDK loads packages from NuGet.org, however, we build
///  against the pre-release versions that might not be available there, we need Roslyn tooling to
///  use our repo's NuGet.config.
/// </summary>
public static class CurrentReferences
{
    public static string? Tfm { get; }
    public static string? NetCoreRefsVersion { get; }

    /// <summary>
    ///  Reference assemblies for the .NET Core App that this repo is built against.
    ///  To be used with the latest public surface defined in assemblies built in this repo.
    /// </summary>
    public static ReferenceAssemblies? NetCoreAppReferences { get; }

    /// <summary>
    ///  Path to the System.Windows.Forms.dll reference assembly in our artifacts folder.
    ///  It has the latest public API surface area.
    /// </summary>
    public static string? WinFormsRefPath { get; }
    public static string? RepoRootPath { get; }

    private const string RefPackageName = "Microsoft.NETCore.App.Ref";

    static CurrentReferences()
    {
        if (!GetRootFolderPath(out string? rootFolderPath))
        {
            throw new InvalidOperationException("Could not locate the repository global.json from the analyzer test assembly.");
        }

        RepoRootPath = rootFolderPath;

        if (!TryGetNetCoreVersion(out string? tfm, out string? netCoreRefsVersion))
        {
            throw new InvalidOperationException("Could not resolve the analyzer test runtime reference assemblies.");
        }

        Tfm = tfm;
        NetCoreRefsVersion = netCoreRefsVersion;

        string configuration =
#if DEBUG
            "Debug";
#else
            "Release";
#endif

        WinFormsRefPath = Path.Join(RepoRootPath, "artifacts", "obj", "System.Windows.Forms", configuration, tfm, "ref", "System.Windows.Forms.dll");

        // Resolve the product's targeting pack from NuGet, independently of the bootstrap SDK's bundled pack.
        NetCoreAppReferences = new ReferenceAssemblies(
            tfm,
            new PackageIdentity(RefPackageName, netCoreRefsVersion),
            Path.Join("ref", tfm))
               .WithNuGetConfigFilePath(Path.Join(RepoRootPath, "NuGet.Config"));
    }

    internal static (string Tfm, string Version) ParseRuntimeConfiguration(string configuration)
    {
        JsonNode? options = JsonNode.Parse(configuration)?["runtimeOptions"];
        string? tfm = (string?)options?["tfm"];
        string? version = (string?)options?["framework"]?["version"];

        return string.IsNullOrEmpty(tfm) || string.IsNullOrEmpty(version)
            ? throw new InvalidOperationException("The analyzer test runtime configuration must specify its TFM and runtime version.")
            : ((string Tfm, string Version))(tfm, version);
    }

    private static bool TryGetNetCoreVersion(
        [NotNullWhen(true)] out string? tfm,
        [NotNullWhen(true)] out string? netCoreRefsVersion)
    {
        tfm = default;
        netCoreRefsVersion = default;

        string testRuntimeConfigPath = Path.ChangeExtension(
            typeof(CurrentReferences).Assembly.Location,
            ".runtimeconfig.json");

        if (!File.Exists(testRuntimeConfigPath))
        {
            return false;
        }

        // MSBuild sets this runtime version to the same targeting pack used to compile WinForms.
        (tfm, netCoreRefsVersion) = ParseRuntimeConfiguration(File.ReadAllText(testRuntimeConfigPath));

        return true;
    }

    private static bool GetRootFolderPath([NotNullWhen(true)] out string? root)
    {
        root = default;

        // Our tests should be running from somewhere within the repo root.
        // So, we walk the parent folder structure until we find our global.json.
        string? testPath = Path.GetDirectoryName(typeof(CurrentReferences).Assembly.Location);

        // We walk the parent folder structure until we find our global.json.
        string? currentFolderPath = Path.GetDirectoryName(testPath);

        while (currentFolderPath is not null)
        {
            string globalJsonPath = Path.Join(currentFolderPath, "global.json");
            if (File.Exists(globalJsonPath))
            {
                // We've found the repo root.
                root = currentFolderPath;
                return true;
            }

            currentFolderPath = Path.GetDirectoryName(currentFolderPath);
        }

        // Either CallerPathAttribute is didn't give us the path or global.json file had disappeared.
        return false;
    }
}
