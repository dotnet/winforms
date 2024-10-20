' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Microsoft.VisualBasic.Devices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ComputerInfoDebugViewTests

        <WinFormsFact>
        Public Sub ComputerInfoDebugView_Memory()
            Dim info As New ComputerInfo.ComputerInfoDebugView(New ComputerInfo)
            info.AvailablePhysicalMemory.Should.NotBe(0UI)
            info.AvailableVirtualMemory.Should.NotBe(0UI)
            info.TotalPhysicalMemory.Should.NotBe(0UI)
            info.TotalVirtualMemory.Should.NotBe(0UI)
        End Sub

        <WinFormsFact>
        Public Sub ComputerInfoDebugView_Properties()
            Dim realClass As New ComputerInfo
            Dim infoDebugView As New ComputerInfo.ComputerInfoDebugView(realClass)
            infoDebugView.InstalledUICulture.Should.Be(realClass.InstalledUICulture)
            infoDebugView.OSPlatform.Should.Be(realClass.OSPlatform)
            infoDebugView.OSVersion.Should.Be(realClass.OSVersion)
        End Sub

    End Class
End Namespace
