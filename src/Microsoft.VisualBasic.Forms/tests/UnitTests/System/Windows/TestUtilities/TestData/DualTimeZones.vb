' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class DualTimeZones

        Public Sub New(zone As TimeZoneNames)
            TimeName = zone.ToString
            Select Case zone
                Case TimeZoneNames.GMT
                    ComputerTime = My.Computer.Clock.GmtTime
                    SystemTime = Date.UtcNow
                Case TimeZoneNames.Local
                    ComputerTime = My.Computer.Clock.LocalTime
                    SystemTime = Date.Now
                Case Else
                    ComputerTime = My.Computer.Clock.LocalTime
                    SystemTime = Date.Now.AddSeconds(1)
            End Select
        End Sub

        Public Property ComputerTime As Date
        Public Property SystemTime As Date
        Public Property TimeName As String

    End Class
End Namespace
