﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TestUtilitiesTests

        <WinFormsFact>
        Public Sub NullOrEmptyPathTestDataIteratorTests()
            Dim testClass As New NullOrEmptyPathTestData
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
        End Sub

        <WinFormsFact>
        Public Sub TimeTestDataIteratorTests()
            Dim testClass As New TimeTestData
            testClass.Any.Should.BeTrue()
        End Sub

    End Class
End Namespace
