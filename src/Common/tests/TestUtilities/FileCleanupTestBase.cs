// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

/// <summary>
///  Creates a test directory path and cleans it up when test class finishes execution.
/// </summary>
public abstract class FileCleanupTestBase : IDisposable
{
    private string? _testDirectory;

    public string TestDirectory
    {
        get
        {
            if (_testDirectory is null)
            {
                _testDirectory = Path.Combine(Path.GetTempPath(), GetUniqueName());
                Directory.CreateDirectory(_testDirectory);
            }

            return _testDirectory;
        }
    }

    ~FileCleanupTestBase() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        try
        {
            Directory.Delete(TestDirectory);
        }
        catch
        {
        }
    }

    public string GetTestFilePath() => Path.Combine(TestDirectory, GetTestFileName());

    public static string GetTestFileName() => GetUniqueName();

    private static string GetUniqueName() => Guid.NewGuid().ToString("D");
}
