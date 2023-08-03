// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public abstract class FileCleanupTestBase : IDisposable
{
    public readonly string TestDirectory;

    protected FileCleanupTestBase()
    {
        TestDirectory = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Directory.CreateDirectory(TestDirectory);
    }

    ~FileCleanupTestBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
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

    public string GetTestFileName() => GetUniqueName();

    private static string GetUniqueName() => Guid.NewGuid().ToString("D");
}
