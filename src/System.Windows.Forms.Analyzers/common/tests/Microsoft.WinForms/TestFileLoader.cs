// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO.Enumeration;
using System.Text;

namespace Microsoft.WinForms.Test;

/// <summary>
///  Provides methods to load and enumerate test files used for analysis and code fix testing.
/// </summary>
/// <remarks>
///  <para>
///   The TestFileLoader class offers functionality to:
///   - Enumerate test file entries in a given directory with filtering based on file attributes.
///   - Load the content of a specific test file.
///   - Retrieve test file paths based on the analyzer type provided.
///  </para>
///  <para>
///   It assists in organizing test data by identifying files such as AnalyzerTestCode, CodeFixTestCode,
///   FixedTestCode, GlobalUsing, and AdditionalCodeFile. This clear segregation aids analyzer and code-fix
///   tests to operate with the appropriate test file.
///  </para>
/// </remarks>
public static class TestFileLoader
{
    private const string TestData = nameof(TestData);

    /// <summary>
    ///  Represents the filename string for analyzer test code files.
    /// </summary>
    public const string AnalyzerTestCode = nameof(TestFileType.AnalyzerTestCode);

    /// <summary>
    ///  Represents the filename string for code fix test code files.
    /// </summary>
    public const string CodeFixTestCode = nameof(TestFileType.CodeFixTestCode);

    /// <summary>
    ///  Represents the filename string for fixed test code files.
    /// </summary>
    public const string FixedTestCode = nameof(TestFileType.FixedTestCode);

    /// <summary>
    ///  Represents the filename string for global using files.
    /// </summary>
    public const string GlobalUsing = nameof(TestFileType.GlobalUsing);

    /// <summary>
    ///  Enumerates the test file entries in the specified base path.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Scans the provided base directory to locate and classify files used in analyzer and code fix tests.
    ///   Based on the file name, the file is assigned a corresponding TestFileType.
    ///  </para>
    ///  <para>
    ///   This method throws an ArgumentException if the basePath is null or empty, ensuring that a valid
    ///   path is provided.
    ///  </para>
    /// </remarks>
    /// <param name="basePath">The base path to enumerate.</param>
    /// <param name="excludeAttributes">File attributes to be excluded from enumeration.</param>
    /// <returns>An enumerable collection of test file entries.</returns>
    /// <exception cref="ArgumentException">Thrown when the base path is null or empty.</exception>
    public static IEnumerable<TestFileEntry> EnumerateEntries(
           string? basePath = default,
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

        static IEnumerable<TestFileEntry> EnumerateEntriesInternal(
            string basePath,
            EnumerationOptions enumOptions,
            FileAttributes excludeAttributes)
        {
            FileSystemEnumerable<TestFileEntry> enumeration = new FileSystemEnumerable<TestFileEntry>(
                directory: basePath,
                transform: TestFileTransform,
                options: enumOptions);

            foreach (var fileEntry in enumeration)
            {
                yield return fileEntry;
            }

            static TestFileEntry TestFileTransform(ref FileSystemEntry entry)
            {
                TestFileType fileType = Path.GetFileName(entry.ToFullPath()) switch
                {
                    var filename when filename.Contains(AnalyzerTestCode) => TestFileType.AnalyzerTestCode,
                    var filename when filename.Contains(FixedTestCode) => TestFileType.FixedTestCode,
                    var filename when filename.Contains(CodeFixTestCode) => TestFileType.CodeFixTestCode,
                    var filename when filename.Contains(GlobalUsing) => TestFileType.GlobalUsing,
                    _ => TestFileType.AdditionalCodeFile
                };

                return new TestFileEntry(entry.ToFullPath(), fileType);
            }
        }
    }

    /// <summary>
    ///  Loads the content of a test file.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Opens the specified test file using UTF-8 encoding and reads its entire content as a string.
    ///   This method is typically used to retrieve the source code or data required for testing analyzers
    ///   and code fixes.
    ///  </para>
    /// </remarks>
    /// <param name="testFilePath">The path of the test file to load.</param>
    /// <returns>A string containing the content of the test file.</returns>
    public static string LoadTestFile(string testFilePath)
    {
        using var reader = new StreamReader(testFilePath, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    ///  Gets the test file paths for the specified analyzer type.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Constructs the path to the test data directory based on the provided analyzer type and base path.
    ///   If the base path is a file, its parent directory is used. The method then determines the correct
    ///   subdirectory or directories that hold the test files.
    ///  </para>
    ///  <para>
    ///   It throws an InvalidOperationException if the constructed analyzer test file directory does not exist
    ///   or if the directory structure is not as expected (e.g., contains both files and subdirectories).
    ///  </para>
    /// </remarks>
    /// <param name="analyzerType">The type of the analyzer used for determining file paths.</param>
    /// <param name="basePath">The additional base path to include in the test file path.</param>
    /// <returns>An enumerable collection of test file paths.</returns>
    /// <exception cref="ArgumentException">Thrown when the base path is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">
    ///  Thrown if the analyzer test file directory does not exist or the directory structure is invalid.
    /// </exception>
    public static IEnumerable<string> GetTestFilePaths(
        Type analyzerType,
        string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            throw new ArgumentException("The base path must be a valid directory.", nameof(basePath));
        }

        // If this is a file, let's get its parent folder.
        if (File.Exists(basePath))
        {
            basePath = Path.GetDirectoryName(basePath)
                ?? throw new InvalidOperationException("The base path is a file, but its containing folder could not be found.");
        }

        // Now go to the TestData folder:
        basePath = Path.Combine(basePath, TestData);

        // Add the type name as the subdirectory to the basePath to get the
        // path to the analyzer test files:
        string analyzerPath = Path.Combine(basePath, analyzerType.Name);

        if (!Directory.Exists(analyzerPath))
        {
            throw new InvalidOperationException($"The directory '{analyzerPath}' does not exist.");
        }

        List<string> analyzerTestDataSubDirPaths = new List<string>(Directory.EnumerateDirectories(analyzerPath));

        if (analyzerTestDataSubDirPaths.Count == 0)
        {
            // We have only one test data directory for the test, so we use the analyzer path
            // as the test data path.
            analyzerTestDataSubDirPaths.Add(analyzerPath);
            return analyzerTestDataSubDirPaths;
        }

        // If we have subdirectories AND test data files, we throw, because we decided,
        // that we do not want to mix generic test data files and specific test data directory files.
        // So, let's see if we got files in the directory:
        foreach (var directory in Directory.EnumerateFiles(analyzerPath))
        {
            // We have files in the directory, so we throw.
            throw new InvalidOperationException(
                $"The directory '{analyzerPath}' contains files. It should only contain subdirectories.");
        }

        return analyzerTestDataSubDirPaths;
    }
}
