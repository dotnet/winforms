' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TimeTests
        Private Const PrecisionTickLimit As Integer = 1000

        Private Function TimesEqual(
            vbTime As Date,
            systemTime As Date,
            acceptableDifferenceInTicks As Long) As Boolean

            Dim diff As TimeSpan = systemTime - vbTime
            Return Math.Abs(diff.TotalMicroseconds) < acceptableDifferenceInTicks
        End Function

        <WinFormsFact>
        Public Sub SystemTimeNotNew()
            Date.Now.Should.BeAfter(New Date)
        End Sub

        <WinFormsFact>
        Public Sub TimeTestsDataIteratorTests()
            Dim testClass As New TimeData
            testClass.IEnumerable_GetEnumerator.Should.NotBeNull()
        End Sub

        <WinFormsFact>
        Public Sub VbTickCountCloseToEnvironmentTickCount()
            Dim tickCount As Integer = Math.Abs(Environment.TickCount - My.Computer.Clock.TickCount)
            Call (tickCount < PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(TimeData))>
        Public Sub VbTimeCloseToDateUtcNow(systemTime As Date, vbTime As Date)
            TimesEqual(
            vbTime,
            systemTime,
            acceptableDifferenceInTicks:=PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotNew()
            My.Computer.Clock.LocalTime.Should.BeAfter(New Date)
        End Sub

        Protected Friend Class TimeData
            Implements IEnumerable(Of Object())

            Public Iterator Function GetEnumerator() As IEnumerator(Of Object()) Implements IEnumerable(Of Object()).GetEnumerator
                Yield {My.Computer.Clock.GmtTime, Date.UtcNow}
                Yield {My.Computer.Clock.LocalTime, Date.Now}
            End Function

            Public Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return GetEnumerator()
            End Function

        End Class

    End Class
End Namespace
