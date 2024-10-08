' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class GetUniqueUtilitiesTests

        <WinFormsFact>
        Public Sub GetUniqueFileNameTest()
            Dim unexpected As String = GetUniqueFileName()
            unexpected.Should.NotBe(GetUniqueFileName())
            unexpected.Should.StartWith("Test")
        End Sub

        <WinFormsTheory>
        <BoolData>
        Public Sub GetUniqueIntegerTest(positiveOnly As Boolean)
            For i As Integer = 0 To 99
                Dim unexpected As Integer = GetUniqueInteger(positiveOnly)
                If positiveOnly Then
                    unexpected.Should.BeGreaterThanOrEqualTo(0)
                End If
                GetUniqueInteger(positiveOnly).Should.NotBe(unexpected)
            Next
        End Sub

        <WinFormsFact>
        Public Sub GetUniqueTextTest()
            For i As Integer = 0 To 99
                Dim unexpected As String = GetUniqueText()
                unexpected.Should.NotBe(GetUniqueText())
            Next
        End Sub

    End Class
End Namespace
