' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Microsoft.VisualBasic.Devices
Imports Microsoft.VisualBasic.Devices.ComputerInfo
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ComputerInfoDebugViewTests

        <WinFormsFact>
        Public Sub ComputerInfoDebugView_Memory()
            Dim info As New ComputerInfoDebugView(New ComputerInfo)
            If PlatformDetection.IsWindows Then
                info.AvailablePhysicalMemory.Should.NotBe(0UI)
                info.AvailableVirtualMemory.Should.NotBe(0UI)
                info.TotalPhysicalMemory.Should.NotBe(0UI)
                info.TotalVirtualMemory.Should.NotBe(0UI)
            End If
        End Sub

        <WinFormsFact>
        Public Sub ComputerInfoDebugView_Properties()
            Dim info As New ComputerInfoDebugView(New ComputerInfo)
            info.InstalledUICulture.Should.Be(Globalization.CultureInfo.InstalledUICulture)
            info.OSPlatform.Should.Be(Environment.OSVersion.Platform.ToString())
            info.OSVersion.Should.Be(Environment.OSVersion.Version.ToString())
        End Sub

    End Class
End Namespace
