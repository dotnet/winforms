' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TimeTests
        Private Const PrecisionTickLimit As Integer = 1000

        Private Function timesEqual(
            systemLocalTime As Date,
            vbLocalTime As Date,
            acceptableDifferenceInTicks As Long) As Boolean

            Dim diff As TimeSpan = systemLocalTime - vbLocalTime
            Return Math.Abs(diff.TotalMicroseconds) < acceptableDifferenceInTicks
        End Function

        <WinFormsFact>
        Public Sub SystemTimeNotNew()
            Date.Now.Should.BeAfter(New Date)
        End Sub

        <WinFormsFact>
        Public Sub VbGmtTimeCloseToDateUtcNow()
            timesEqual(
                systemLocalTime:=My.Computer.Clock.GmtTime,
                vbLocalTime:=Date.UtcNow,
                acceptableDifferenceInTicks:=PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbLocalTimeCloseToDateNow()
            timesEqual(
                systemLocalTime:=My.Computer.Clock.LocalTime,
                vbLocalTime:=Date.Now,
                acceptableDifferenceInTicks:=PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTickCountCloseToEnvironmentTickCount()
            Dim tickCount As Integer = Math.Abs(Environment.TickCount - My.Computer.Clock.TickCount)
            Call (tickCount < PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotNew()
            My.Computer.Clock.LocalTime.Should.BeAfter(New Date)
        End Sub

    End Class
End Namespace
