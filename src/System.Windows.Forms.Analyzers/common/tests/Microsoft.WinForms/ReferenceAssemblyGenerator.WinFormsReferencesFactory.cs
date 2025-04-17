// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Text.Json;
using System.Text.Json.Nodes;

using Microsoft.CodeAnalysis.Testing;

// File-Cherry-Picked and modified a bit from Tanya Solyanik's Commit ec6a9f8,
// PR #12860 for back-port (release/net9-Servicing) purposes.
internal static partial class ReferenceAssemblyGenerator
{
    /// <summary>
    ///  Provides access to the Microsoft.NETCore.App.Ref package this repo is built against.
    ///  By default Roslyn SDK loads packages from NuGet.org, however, we build
    ///  against the pre-release versions that might not be available there, we need Roslyn tooling to
    ///  use our repo's NuGet.config.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This class locates the repository root directory by finding the global.json file,
    ///   then determines the correct .NET Core version and reference assemblies to use for testing.
    ///   It provides paths to reference assemblies and configuration settings needed for the
    ///   test infrastructure.
    ///  </para>
    /// </remarks>
    public static class WinFormsReferencesFactory
    {
        private const string RefPackageName = "Microsoft.NETCore.App.Ref";
        private const string PrivatePackagePath = "artifacts\\packages\\Debug\\NonShipping\\Microsoft.Private.Winforms.9.0.3-dev.nupkg";

        /// <summary>
        ///  Gets the Target Framework Moniker for the current repository.
        /// </summary>
        public static string? Tfm { get; }

        /// <summary>
        ///  Gets the exact version of the .NET Core reference assemblies being used.
        /// </summary>
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

        /// <summary>
        ///  Path to the NuGet including the latest build.
        /// </summary>
        public static string? WinFormsPrivatePackagePath =>
            Path.Join(
                RepoRootPath,
                PrivatePackagePath);

        /// <summary>
        ///  Gets the root path of the repository.
        /// </summary>
        public static string? RepoRootPath { get; }

        static WinFormsReferencesFactory()
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

            WinFormsRefPath = Path.Join(
                RepoRootPath,
                "artifacts",
                "obj",
                "System.Windows.Forms",
                configuration,
                tfm,
                "ref",
                "System.Windows.Forms.dll");

            // Specify absolute path to the reference assemblies because this version is not necessarily available in the nuget packages cache.
            string netCoreAppRefPath = Path.Join(RepoRootPath, ".dotnet", "packs", RefPackageName);

            if (!Directory.Exists(Path.Join(netCoreAppRefPath, netCoreRefsVersion)))
            {
                try
                {
                    netCoreRefsVersion = GetAvailableVersion(
                        netCoreAppRefPath: netCoreAppRefPath,
                        major: $"{netCoreRefsVersion.Split('.')[0]}.");
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    Debug.WriteLine($"Error accessing version directories: {ex.Message}");
                    return;
                }
            }

            NetCoreRefsVersion = netCoreRefsVersion;

            // Get package from our feeds.
            NetCoreAppReferences = new ReferenceAssemblies(
                targetFramework: tfm,
                referenceAssemblyPackage: new PackageIdentity(RefPackageName, netCoreRefsVersion),
                referenceAssemblyPath: Path.Join("ref", tfm))
                   .WithNuGetConfigFilePath(Path.Join(RepoRootPath, "NuGet.config"));
        }

        /// <summary>
        ///  Gets an available .NET Core version by searching for directories matching a major version prefix.
        /// </summary>
        /// <param name="netCoreAppRefPath">Path to the directory containing .NET Core reference assemblies.</param>
        /// <param name="major">Major version prefix to search for (e.g., "6.").</param>
        /// <returns>The full version string of the matching directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when no matching version directory is found.</exception>
        private static string GetAvailableVersion(string netCoreAppRefPath, string major)
        {
            if (!Directory.Exists(netCoreAppRefPath))
            {
                throw new DirectoryNotFoundException($"Reference assembly directory not found: {netCoreAppRefPath}");
            }

            string[] versions = Directory.GetDirectories(netCoreAppRefPath);
            string? availableVersion = versions.FirstOrDefault(v =>
                Path.GetFileName(v).StartsWith(major, StringComparison.InvariantCultureIgnoreCase));

            return availableVersion is null
                ? throw new DirectoryNotFoundException($"No matching version directory found for major version {major} in {netCoreAppRefPath}")
                : Path.GetFileName(availableVersion);
        }

        /// <summary>
        ///  Attempts to get the .NET Core version information from the repository.
        /// </summary>
        /// <param name="rootFolderPath">The repository root path.</param>
        /// <param name="tfm">When successful, contains the Target Framework Moniker.</param>
        /// <param name="netCoreRefsVersion">When successful, contains the .NET Core reference version.</param>
        /// <returns>True if both TFM and version were successfully retrieved; otherwise, false.</returns>
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
                Debug.WriteLine($"SDK folder not found: {sdkFolderPath}");
                return false;
            }

            return TryGetNetCoreVersionFromJson(sdkFolderPath, out tfm, out netCoreRefsVersion);
        }

        /// <summary>
        ///  Attempts to find the repository root folder by searching for global.json.
        /// </summary>
        /// <param name="root">When successful, contains the path to the repository root.</param>
        /// <returns>True if the root folder was found; otherwise, false.</returns>
        private static bool GetRootFolderPath([NotNullWhen(true)] out string? root)
        {
            root = default;

            try
            {
                // Our tests should be running from somewhere within the repo root.
                // So, we walk the parent folder structure until we find our global.json.
                string? testPath = Path.GetDirectoryName(typeof(WinFormsReferencesFactory).Assembly.Location);

                if (testPath is null)
                {
                    Debug.WriteLine("Unable to determine assembly location path");
                    return false;
                }

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

                // Either we couldn't determine the assembly location or global.json file couldn't be found.
                Debug.WriteLine("Unable to find global.json in any parent directory");

                return false;
            }
            catch (Exception ex) when (ex is IOException or SecurityException)
            {
                Debug.WriteLine($"Error accessing file system when searching for root folder: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///  Attempts to get the SDK version from the global.json file.
        /// </summary>
        /// <param name="rootFolderPath">The repository root path.</param>
        /// <param name="version">When successful, contains the SDK version.</param>
        /// <returns>True if the SDK version was successfully retrieved; otherwise, false.</returns>
        /// <exception cref="FileNotFoundException">Thrown when global.json file does not exist.</exception>
        /// <exception cref="JsonException">Thrown when global.json has invalid format.</exception>
        private static bool TryGetSdkVersion(string rootFolderPath, [NotNullWhen(true)] out string? version)
        {
            version = default;
            string globalJsonPath = Path.Join(rootFolderPath, "global.json");

            if (!File.Exists(globalJsonPath))
            {
                Debug.WriteLine($"global.json file not found at: {globalJsonPath}");
                return false;
            }

            try
            {
                string globalJsonString = File.ReadAllText(globalJsonPath);
                JsonObject? jsonObject = JsonNode.Parse(globalJsonString)?.AsObject();
                version = (string?)jsonObject?["sdk"]?["version"];

                if (version is null)
                {
                    Debug.WriteLine("SDK version not found in global.json");
                }

                return version is not null;
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                Debug.WriteLine($"Error reading or parsing global.json: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///  Attempts to get the .NET Core version from the runtime configuration JSON file.
        /// </summary>
        /// <param name="sdkFolderPath">Path to the SDK folder.</param>
        /// <param name="tfm">When successful, contains the Target Framework Moniker.</param>
        /// <param name="version">When successful, contains the .NET Core version.</param>
        /// <returns>True if both TFM and version were successfully retrieved; otherwise, false.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the runtime config file is not found.</exception>
        /// <exception cref="JsonException">Thrown when JSON parsing fails.</exception>
        private static bool TryGetNetCoreVersionFromJson(
            string sdkFolderPath,
            [NotNullWhen(true)] out string? tfm,
            [NotNullWhen(true)] out string? version)
        {
            tfm = default;
            version = default;

            string configJsonPath = Path.Join(sdkFolderPath, "dotnet.runtimeconfig.json");

            if (!File.Exists(configJsonPath))
            {
                Debug.WriteLine($"Runtime config JSON file not found at: {configJsonPath}");
                return false;
            }

            try
            {
                string configJsonString = File.ReadAllText(configJsonPath);
                JsonObject? jsonObject = JsonNode.Parse(configJsonString)?.AsObject();
                JsonNode? runtimeOptions = jsonObject?["runtimeOptions"];

                tfm = (string?)runtimeOptions?["tfm"];

                if (tfm is null)
                {
                    Debug.WriteLine("TFM not found in runtime config JSON");
                    version = default;
                    return false;
                }

                version = (string?)runtimeOptions?["framework"]?["version"];

                if (version is null)
                {
                    Debug.WriteLine("Framework version not found in runtime config JSON");
                    tfm = null;
                    return false;
                }

                return true;
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                Debug.WriteLine($"Error reading or parsing runtime config JSON: {ex.Message}");
                tfm = null;
                version = null;
                return false;
            }
        }
    }
}
