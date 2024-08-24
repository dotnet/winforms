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
        Public Sub VbTickCountCloseToEnvironmentTickCount()
            Dim tickCount As Integer = Math.Abs(Environment.TickCount - My.Computer.Clock.TickCount)
            Call (tickCount < PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTimeCloseToSystemTime()
            Dim systemTime As Date = Date.UtcNow
            Dim vbTime As Date = My.Computer.Clock.GmtTime
            TimesEqual(
                vbTime,
                systemTime,
                acceptableDifferenceInTicks:=PrecisionTickLimit).Should.BeTrue()

            systemTime = Date.Now
            vbTime = My.Computer.Clock.LocalTime
            TimesEqual(
                vbTime,
                systemTime,
                acceptableDifferenceInTicks:=PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotNew()
            My.Computer.Clock.LocalTime.Should.BeAfter(New Date)
        End Sub

    End Class
End Namespace
