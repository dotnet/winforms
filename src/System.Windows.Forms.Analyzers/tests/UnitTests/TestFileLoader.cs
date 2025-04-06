// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Text;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Utility that handles loading of test files from the output folder.
/// </summary>
public static class TestFileLoader
{
    // Name of the subfolder that contains the test input files.
    private const string TestData = nameof(TestData);

    /// <summary>
    ///  Gets path to the test input file relative to the output folder where test binary is located.
    /// </summary>
    public static string GetTestFilePath(string toolName, string testName, SourceLanguage language)
    {
        var builder = new StringBuilder();

        builder.Append(toolName);
        builder.Append(Path.DirectorySeparatorChar);
        builder.Append(TestData);
        builder.Append(Path.DirectorySeparatorChar);
        builder.Append(testName);

        if (language != SourceLanguage.None)
        {
            builder.Append(language == SourceLanguage.CSharp ? ".cs" : ".vb");
        }

        return builder.ToString();
    }

    public static Task<string> LoadTestFileAsync(string pathSegment, string testName, SourceLanguage language = SourceLanguage.CSharp)
    {
        string filePath = GetTestFilePath(pathSegment, testName, language);

        return LoadTestFileAsync(filePath);
    }

    public static async Task<string> LoadTestFileAsync(string testFilePath)
    {
        using var reader = new StreamReader(testFilePath, Encoding.UTF8);

        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public static async Task<string> GetAnalyzerTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await LoadTestFileAsync(Path.Join("Analyzers", toolName), testName, SourceLanguage.None).ConfigureAwait(false);
    }

    public static async Task<string> GetGeneratorTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await LoadTestFileAsync(Path.Join("Generators", toolName), testName, SourceLanguage.None).ConfigureAwait(false);
    }

    public static async Task<string> GetCSAnalyzerTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await LoadTestFileAsync(Path.Join("Analyzers", toolName), testName).ConfigureAwait(false);
    }

    public static async Task<string> GetVBAnalyzerTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await LoadTestFileAsync(Path.Join("Analyzers", toolName), testName, SourceLanguage.VisualBasic).ConfigureAwait(false);
    }
}
