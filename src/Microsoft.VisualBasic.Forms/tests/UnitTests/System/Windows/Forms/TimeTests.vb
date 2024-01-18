' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class TimeTests
        Private Const PrecisionTickLimit As Integer = 1000

        Private Function timesEqual(systemLocalTime As Date, vbLocalTime As Date, acceptableDifferenceInTicks As Long) As Boolean
            Dim diff As TimeSpan = systemLocalTime - vbLocalTime
            Return Math.Abs(diff.TotalMicroseconds) < acceptableDifferenceInTicks
        End Function

        <WinFormsFact>
        Public Sub SystemTimeNotNew()
            Assert.True(Date.Now > New Date)
        End Sub

        <WinFormsFact>
        Public Sub VbTimeNotNew()
            Assert.True(My.Computer.Clock.LocalTime > New Date)
        End Sub

        <WinFormsFact>
        Public Sub VbGmtTimeCloseToDateUtcNow()
            Assert.True(timesEqual(My.Computer.Clock.GmtTime, Date.UtcNow, PrecisionTickLimit))
        End Sub

        <WinFormsFact>
        Public Sub VbLocalTimeCloseToDateNow()
            Assert.True(timesEqual(My.Computer.Clock.LocalTime, Date.Now, PrecisionTickLimit))
        End Sub

        <WinFormsFact>
        Public Sub VbTickCountCloseToEnvironmentTickCount()
            Assert.True(Math.Abs(Environment.TickCount - My.Computer.Clock.TickCount) < PrecisionTickLimit)
        End Sub

    End Class

End Namespace
