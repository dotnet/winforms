' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class DualTimeZones

        Public Sub New(zone As TimeZoneNames)
            TimeName = zone.ToString
            Select Case zone
                Case TimeZoneNames.GMT
                    SystemTime = Date.UtcNow
                    ComputerTime = My.Computer.Clock.GmtTime
                Case TimeZoneNames.Local
                    SystemTime = Date.Now
                    ComputerTime = My.Computer.Clock.LocalTime
                Case Else
                    SystemTime = Date.Now.AddDays(1)
                    ComputerTime = My.Computer.Clock.LocalTime
            End Select
        End Sub

        Public Property ComputerTime As Date
        Public Property SystemTime As Date
        Public Property TimeName As String

    End Class
End Namespace
