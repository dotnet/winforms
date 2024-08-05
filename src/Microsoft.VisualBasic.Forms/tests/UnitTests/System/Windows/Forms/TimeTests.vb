' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TimeTests
        Private Const PrecisionTickLimit As Integer = 1000

        Private Function timesEqual(systemLocalTime As Date, vbLocalTime As Date, acceptableDifferenceInTicks As Long) As Boolean
            Dim diff As TimeSpan = systemLocalTime - vbLocalTime
            Return Math.Abs(diff.TotalMicroseconds) < acceptableDifferenceInTicks
        End Function

        <WinFormsFact>
        Public Sub SystemTimeNotNew()
            Date.Now.Should.BeAfter(New Date)
        End Sub

        <WinFormsFact>
        Public Sub VbGmtTimeCloseToDateUtcNow()
            timesEqual(My.Computer.Clock.GmtTime, Date.UtcNow, PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbLocalTimeCloseToDateNow()
            timesEqual(My.Computer.Clock.LocalTime, Date.Now, PrecisionTickLimit).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTickCountCloseToEnvironmentTickCount()
            Dim condition As Boolean = Math.Abs(Environment.TickCount - My.Computer.Clock.TickCount) < PrecisionTickLimit
            condition.Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotNew()
            My.Computer.Clock.LocalTime.Should.BeAfter(New Date)
        End Sub

    End Class
End Namespace
