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
    /// <param name="additionalPath">The additional path to include in the test file path.</param>
    /// <returns>The test file path.</returns>
    public static IEnumerable<string> GetTestFilePaths(
        Type analyzerType,
        string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            throw new ArgumentException("The base path must be a valid directory.", nameof(basePath));
        }

        // Let's see if the class defines an AnalyzerTestPathAttribute:
        AnalyzerTestPathAttribute? analyzerTestPathAttribute = analyzerType.GetCustomAttribute<AnalyzerTestPathAttribute>();

        string analyzerPath = analyzerTestPathAttribute is not null
            ? analyzerTestPathAttribute.Path
            : TestData;

        var builder = new StringBuilder();

        builder.Append($"{basePath}\\{analyzerPath}");
        builder.Append('\\');

        analyzerPath = builder.ToString();

        List<string> analyzerTestDataPaths;

        // Now let's see, if we have subdirectories for the analyzer test data:
        if (Directory.Exists(analyzerPath))
        {
            analyzerTestDataPaths = [.. Directory.EnumerateDirectories(analyzerPath)];

            // If we have subdirectories AND test data files, we throw. So, let's see if we got files in the directory:
            foreach (var directory in Directory.EnumerateFiles(analyzerPath))
            {
                throw new InvalidOperationException($"The directory '{analyzerPath}' contains files. It should only contain subdirectories.");
            }

            return analyzerTestDataPaths;
        }

        analyzerTestDataPaths = [];
        analyzerTestDataPaths.Add(analyzerPath);

        return analyzerTestDataPaths;
    }
}
