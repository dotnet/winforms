' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized
Imports System.IO
Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.FileIO
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TestUtilitiesTests

        <WinFormsFact>
        Public Sub CleanupDirectoryNullOrEmptyPathTestDataIteratorTests()
            Dim testClass As New BadPathsTestData
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
        End Sub

        <WinFormsFact>
        Public Sub CleanupDirectoryPathTestDataIteratorTests()
            Dim testClass As New NullOrEmptyPathTestData
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
        End Sub
        <WinFormsFact>
        Public Sub GetUniqueIntegerTests_Success()
            Dim condition As Boolean = GetUniqueInteger(positiveOnly:=True) >= 0
            condition.Should.BeTrue()
            GetUniqueInteger(positiveOnly:=False).Should.NotBe(GetUniqueInteger(positiveOnly:=False))
        End Sub

        <WinFormsFact>
        Public Sub WrongPasswordTestDataIteratorTests()
            Dim testClass As New WrongPasswordTestData
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
        End Sub
    End Class
End Namespace
