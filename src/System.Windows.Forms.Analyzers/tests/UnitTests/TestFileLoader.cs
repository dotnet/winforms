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
    private const string Data = nameof(Data);

    /// <summary>
    ///  Gets path to the test input file relative to the output folder where test binary is located.
    /// </summary>
    public static string GetTestFilePath(string toolName, string testName, SourceLanguage language)
    {
        var builder = new StringBuilder();

        builder.Append(toolName);
        builder.Append(Path.DirectorySeparatorChar);
        builder.Append(Data);
        builder.Append(Path.DirectorySeparatorChar);
        builder.Append(testName);
        if (language != SourceLanguage.None)
        {
            builder.Append(language == SourceLanguage.CSharp ? ".cs" : ".vb");
        }

        return builder.ToString();
    }

    public static Task<string> LoadTestFileAsync(string toolName, string testName, SourceLanguage language = SourceLanguage.CSharp)
    {
        string filePath = GetTestFilePath(toolName, testName, language);

        return LoadTestFileAsync(filePath);
    }

    public static async Task<string> LoadTestFileAsync(string testFilePath)
    {
        using var reader = new StreamReader(testFilePath, Encoding.UTF8);

        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public static async Task<string> GetTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await LoadTestFileAsync(toolName, testName).ConfigureAwait(false);
    }

    public static async Task<string> GetVBTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await LoadTestFileAsync(toolName, testName, SourceLanguage.VisualBasic).ConfigureAwait(false);
    }
}
