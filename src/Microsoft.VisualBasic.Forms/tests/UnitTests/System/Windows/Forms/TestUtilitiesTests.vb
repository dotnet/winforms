' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TestUtilitiesTests

        <WinFormsFact>
        Public Sub EnumTestDataInvalidType()
            Dim testClass As New EnumTestData(Of Integer)
            Dim testCode As Action =
                Sub()
                    testClass.Any()
                End Sub
            testCode.Should.Throw(Of ArgumentException)()
        End Sub

        <WinFormsFact>
        Public Sub EnumTestDataIteratorTests()
            Dim testClass As New EnumTestData(Of AuthenticationMode)
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
            testClass.Any.Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub TimeTestDataIteratorTests()
            Dim testClass As New TimeTestData
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
            testClass.Any.Should.BeTrue()
        End Sub

    End Class
End Namespace
