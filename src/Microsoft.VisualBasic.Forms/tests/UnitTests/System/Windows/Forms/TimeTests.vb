' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TimeTests
        Friend Const PrecisionTickLimit As Integer = 1_000_000

        Private Shared Function TimesEqual(
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

        <WinFormsTheory>
        <ClassData(GetType(TimeTestData))>
        Public Sub VbTimeCloseToSystemTime(timeData As DualTimeZones)
            Dim systemTime As Date = timeData.SystemTime
            Dim vbTime As Date = timeData.ComputerTime
            Dim because As String = $"{timeData.TimeName} is wrong, System Time is {systemTime} and Clock Time is {vbTime}"
            TimesEqual(
                vbTime,
                systemTime,
                acceptableDifferenceInTicks:=PrecisionTickLimit).Should.BeTrue(because)
        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotCloseToSystemTime()
            Dim timeData As New DualTimeZones(TimeZone.MismatchedTimes)
            Dim systemTime As Date = timeData.SystemTime
            Dim vbTime As Date = timeData.ComputerTime
            Dim because As String = $"{timeData.ComputerTime} is wrong, System Time is {systemTime} and Clock Time is {vbTime}"
            Dim results As Boolean = TimesEqual(
                vbTime,
                systemTime,
                acceptableDifferenceInTicks:=PrecisionTickLimit)
            results.Should.NotBe(True, because)

        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotNew()
            My.Computer.Clock.LocalTime.Should.BeAfter(New Date)
        End Sub

    End Class
End Namespace
