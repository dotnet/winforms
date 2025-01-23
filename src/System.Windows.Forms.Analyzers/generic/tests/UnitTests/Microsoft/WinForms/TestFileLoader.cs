// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO.Enumeration;
using System.Reflection;
using System.Text;

namespace Microsoft.WinForms.Test;

/// <summary>
///  Utility that handles loading of test files from a folder called 'TestData'.
/// </summary>
public static class TestFileLoader
{
    private const string TestData = nameof(TestData);

    private const string AnalyzerTestCode = nameof(TestFileType.AnalyzerTestCode);
    private const string CodeFixTestCode = nameof(TestFileType.CodeFixTestCode);
    private const string FixedCode = nameof(TestFileType.FixedCode);
    private const string GlobalUsing = nameof(TestFileType.GlobalUsing);

    /// <summary>
    ///  Enumerates the test file entries in the specified base path.
    /// </summary>
    /// <param name="basePath">The base path to enumerate.</param>
    /// <param name="excludeAttributes">The file attributes to exclude.</param>
    /// <returns>An enumerable collection of test file entries.</returns>
    /// <exception cref="ArgumentException">Thrown when the base path is null or empty.</exception>
    public static IEnumerable<TestFileEntry> EnumerateEntries(
           string basePath,
           FileAttributes excludeAttributes = default)
    {
        if (string.IsNullOrEmpty(basePath))
        {
            throw new ArgumentException("The base path must be a valid directory.", nameof(basePath));
        }

        var enumOptions = new EnumerationOptions
        {
            RecurseSubdirectories = false
        };

        return EnumerateEntriesInternal(basePath, enumOptions, excludeAttributes);

        IEnumerable<TestFileEntry> EnumerateEntriesInternal(
            string basePath,
            EnumerationOptions enumOptions,
            FileAttributes excludeAttributes)
        {
            FileSystemEnumerable<TestFileEntry> enumeration = new FileSystemEnumerable<TestFileEntry>(
                directory: basePath,
                transform: (ref FileSystemEntry entry) =>
                {
                    TestFileType fileType = Path.GetFileName(basePath) switch
                    {
                        AnalyzerTestCode => TestFileType.AnalyzerTestCode,
                        CodeFixTestCode => TestFileType.CodeFixTestCode,
                        FixedCode => TestFileType.FixedCode,
                        GlobalUsing => TestFileType.GlobalUsing,
                        _ => TestFileType.AdditionalCodeFile
                    };

                    return new TestFileEntry(entry.ToFullPath(), fileType);
                },
                options: enumOptions);

            foreach (var fileEntry in enumeration)
            {
                yield return fileEntry;
            }
        }
    }

    /// <summary>
    ///  Asynchronously loads the content of a test file.
    /// </summary>
    /// <param name="testFilePath">The path of the test file to load.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content of the test file.</returns>
    public static async Task<string> LoadTestFileAsync(string testFilePath)
    {
        using var reader = new StreamReader(testFilePath, Encoding.UTF8);

        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///  Gets the test file paths for the specified analyzer type.
    /// </summary>
    /// <param name="analyzerType">The type of the analyzer.</param>
    /// <param name="basePath">The additional path to include in the test file path.</param>
    /// <returns>The test file path.</returns>
    public static IEnumerable<string> GetTestFilePaths(
        Type analyzerType,
        string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            throw new ArgumentException("The base path must be a valid directory.", nameof(basePath));
        }

        // If this is a file, let's get it's parent folder.
        if (File.Exists(basePath))
        {
            basePath = Path.GetDirectoryName(basePath)
                ?? throw new InvalidOperationException("The base path is a file, but its containing folder could not be found.");
        }

        // Go up the path to the 'UnitTests' folder, and then go to the TestData folder.
        DirectoryInfo basePathInfo = new DirectoryInfo(basePath);

        // No go up so often until we've found the UnitTests folder.
        while (basePathInfo.Name != "UnitTests")
        {
            basePathInfo = basePathInfo.Parent
                ?? throw new InvalidOperationException("Expected a folder 'UnitTest' in the hierarchy, but it could not be found.");
        }

        // Let's see if the class defines an AnalyzerTestPathAttribute:
        AnalyzerTestPathAttribute? analyzerTestPathAttribute = analyzerType.GetCustomAttribute<AnalyzerTestPathAttribute>();

        string analyzerPath = analyzerTestPathAttribute is not null
            ? analyzerTestPathAttribute.Path
            : analyzerType.Name;

        // Now go to the TestData folder:
        basePath = Path.Combine(basePathInfo.FullName, TestData);
        analyzerPath = $"{basePath}\\{analyzerPath}\\";

        // Now go to the TestData folder:
        basePath = Path.Combine(basePathInfo.FullName, TestData);
        analyzerPath = $"{basePath}\\{analyzerPath}\\";

        // Define possible variations of the analyzer path
        string[] possiblePaths =
        [
            analyzerPath,
            analyzerPath.Replace("Tests", ""),
            analyzerPath.Replace("Test", "")
        ];

        // Find the first existing path
        analyzerPath = possiblePaths.FirstOrDefault(Directory.Exists)
            ?? throw new InvalidOperationException(
                $"The directory '{analyzerPath}' could not be found.");

        List<string> analyzerTestDataPaths;
        analyzerTestDataPaths = [.. Directory.EnumerateDirectories(analyzerPath)];

        if (analyzerTestDataPaths.Count == 0)
        {
            // We have only one test data directory for the test, so we use the analyzer path as the test data path.
            analyzerTestDataPaths.Add(analyzerPath);

            return analyzerTestDataPaths;
        }

        // If we have subdirectories AND test data files, we throw, because we decided,
        // that we do not want to mix generic test data files and specific test data directory files.
        // So, let's see if we got files in the directory:
        foreach (var directory in Directory.EnumerateFiles(analyzerPath))
        {
            // We have files in the directory, so we throw.
            throw new InvalidOperationException($"The directory '{analyzerPath}' contains files. It should only contain subdirectories.");
        }

        return analyzerTestDataPaths;
    }
}
