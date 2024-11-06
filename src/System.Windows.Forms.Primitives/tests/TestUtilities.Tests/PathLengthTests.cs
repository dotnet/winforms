// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using System.IO.Enumeration;

namespace System.Tests;

public class PathLengthTests
{
    [Fact]
    public void RepoPathsLeaveRoomForRoot()
    {
        string currentPath = typeof(PathLengthTests).Assembly.Location;
        currentPath = Path.GetFullPath(@"..\..\..\..\..\..\src", currentPath);
        Directory.Exists(currentPath).Should().BeTrue();

        // Current path will be something like C:\Users\jkuhne\source\repos\winforms\src (41 chars, 38 without src).
        // We want to reserve 40 characters of the path length for everything past src.
        int currentRootLength = currentPath.Length - "src".Length;

        const int MaxRootLength = 40;

        // Workaround for MAX_PATH limitation
        // - https://learn.Microsoft.com/windows/win32/fileio/maximum-file-path-limitation?tabs=registry
        // Leave a char for the trailing null (MAX_PATH is 260)
        int maxLength = 260 - 1 - (currentRootLength > MaxRootLength
            ? 0
            : MaxRootLength - currentRootLength);

        FileSystemEnumerable<string> enumerable = new(
            currentPath,
            (ref FileSystemEntry entry) => entry.ToFullPath(),
            new EnumerationOptions() { RecurseSubdirectories = true })
        {
            ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                // Directory doesn't contain a trailing slash
                entry.Directory.Length + entry.FileName.Length > maxLength
        };

        enumerable.Should().BeEmpty();
    }
}
