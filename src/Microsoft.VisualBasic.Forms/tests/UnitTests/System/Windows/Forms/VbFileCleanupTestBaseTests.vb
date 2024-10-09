﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class VbFileCleanupTestBaseTests
        Inherits VbFileCleanupTestBase

        <WinFormsFact>
        Public Sub CreateTempDirectoryTest()
            Dim tempDirectory As String = CreateTempDirectory()
            tempDirectory.Should.StartWith(Path.GetTempPath)
            tempDirectory = CreateTempDirectory()
            _testDirectories.Count.Should.Be(1)
            tempDirectory = CreateTempDirectory(lineNumber:=1)
            _testDirectories.Count.Should.Be(2)
        End Sub

        <WinFormsFact>
        Public Sub CreateTempFileTest()
            Dim tempDirectory As String = CreateTempDirectory()
            CreateTempFile(tempDirectory).Should.StartWith(Path.GetTempPath)
            CreateTempFile(tempDirectory).Should.EndWith("Testing.Txt")
            CreateTempFile(tempDirectory, "Testing1.Txt").Should.EndWith("Testing1.Txt")
        End Sub

        <WinFormsFact>
        Public Sub DirectoryIsAccessibleWithNonexistentPath()
            Dim directoryPath As String = Path.Combine(CreateTempDirectory(), GetUniqueFileName)
            DirectoryIsAccessible(directoryPath).Should.BeFalse()
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringDataAttribute>
        Public Sub DirectoryIsAccessibleWithNullOrEmptyPathTests(directoryPath As String)
            DirectoryIsAccessible(directoryPath).Should.BeFalse()
        End Sub

        <WinFormsFact>
        Public Sub GetTestFileNameWithPathTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim testFile1 As String = GetUniqueFileNameWithPath(testDirectory)
            testFile1.Should.NotBe(GetUniqueFileNameWithPath(testDirectory))
            testDirectory.Should.Be(Path.GetDirectoryName(testFile1))
            File.Exists(testFile1).Should.BeFalse()
        End Sub

    End Class
End Namespace
