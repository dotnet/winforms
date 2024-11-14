' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TestUtilitiesTests

        <WinFormsFact>
        Public Sub EnumDataIteratorTests()
            Dim testClass As New EnumTestData(GetType(AuthenticationMode))
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
