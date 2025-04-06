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
            return;
        }

        RepoRootPath = rootFolderPath;

        if (!TryGetNetCoreVersion(rootFolderPath, out string? tfm, out string? netCoreRefsVersion))
        {
            return;
        }

        Tfm = tfm;

        string configuration =
#if DEBUG
            "Debug";
#else
            "Release";
#endif

        WinFormsRefPath = Path.Join(RepoRootPath, "artifacts", "obj", "System.Windows.Forms", configuration, tfm, "ref", "System.Windows.Forms.dll");

        // Specify absolute path to the reference assemblies because this version is not necessarily available in the nuget packages cache.
        string netCoreAppRefPath = Path.Join(RepoRootPath, ".dotnet", "packs", RefPackageName);
        if (!Directory.Exists(Path.Join(netCoreAppRefPath, netCoreRefsVersion)))
        {
            netCoreRefsVersion = GetAvailableVersion(netCoreAppRefPath, $"{netCoreRefsVersion.Split('.')[0]}.");
        }

        NetCoreRefsVersion = netCoreRefsVersion;

        // Get package from our feeds.
        NetCoreAppReferences = new ReferenceAssemblies(
            tfm,
            new PackageIdentity(RefPackageName, netCoreRefsVersion),
            Path.Join("ref", tfm))
               .WithNuGetConfigFilePath(Path.Join(RepoRootPath, "NuGet.Config"));
    }

    private static string GetAvailableVersion(string netCoreAppRefPath, string major)
    {
        string[] versions = Directory.GetDirectories(netCoreAppRefPath);
        string? availableVersion = versions.FirstOrDefault(v =>
            Path.GetFileName(v).StartsWith(major, StringComparison.InvariantCultureIgnoreCase));

        return availableVersion!;
    }

    private static bool TryGetNetCoreVersion(
        string rootFolderPath,
        [NotNullWhen(true)] out string? tfm,
        [NotNullWhen(true)] out string? netCoreRefsVersion)
    {
        tfm = default;
        netCoreRefsVersion = default;

        if (!TryGetSdkVersion(rootFolderPath, out string? version))
        {
            return false;
        }

        // First, try to use the local .NET SDK if it's there.
        string sdkFolderPath = Path.Join(rootFolderPath, ".dotnet", "sdk", version);
        if (!Directory.Exists(sdkFolderPath))
        {
            return false;
        }

        return TryGetNetCoreVersionFromJson(sdkFolderPath, out tfm, out netCoreRefsVersion);
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

    private static bool TryGetSdkVersion(string rootFolderPath, [NotNullWhen(true)] out string? version)
    {
        string globalJsonPath = Path.Join(rootFolderPath, "global.json");
        string globalJsonString = File.ReadAllText(globalJsonPath);
        JsonObject? jsonObject = JsonNode.Parse(globalJsonString)?.AsObject();
        version = (string?)jsonObject?["sdk"]?["version"];

        return version is not null;
    }

    private static bool TryGetNetCoreVersionFromJson(
        string sdkFolderPath,
        [NotNullWhen(true)] out string? tfm,
        [NotNullWhen(true)] out string? version)
    {
        string configJsonPath = Path.Join(sdkFolderPath, "dotnet.runtimeconfig.json");
        string configJsonString = File.ReadAllText(configJsonPath);
        JsonObject? jsonObject = JsonNode.Parse(configJsonString)?.AsObject();
        JsonNode? runtimeOptions = jsonObject?["runtimeOptions"];
        tfm = (string?)runtimeOptions?["tfm"];
        if (tfm is null)
        {
            version = default;
            return false;
        }

        version = (string?)runtimeOptions?["framework"]?["version"];
        if (version is null)
        {
            tfm = null;
            return false;
        }

        return true;
    }
}
