// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.WinForms.Test;

/// <summary>
///  Represents an entry for a test file.
/// </summary>
public class TestFileEntry
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="TestFileEntry"/> class.
    /// </summary>
    /// <param name="filePath">The file path of the test file.</param>
    /// <param name="fileType">The type of the test file.</param>
    public TestFileEntry(string filePath, TestFileType fileType)
    {
        FilePath = filePath;
        FileType = fileType;
    }

    /// <summary>
    ///  Gets the file path of the test file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    ///  Gets the type of the test file.
    /// </summary>
    public TestFileType FileType { get; }
}
