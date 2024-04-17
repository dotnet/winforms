' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.Devices
Imports Microsoft.VisualBasic.Devices.ComputerInfo
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ComputerInfoDebugViewTests

        <Fact>
        Public Sub Memory()
            Dim info As New ComputerInfoDebugView(New ComputerInfo)
            If PlatformDetection.IsWindows Then
                Assert.NotEqual(0UI, info.AvailablePhysicalMemory)
                Assert.NotEqual(0UI, info.AvailableVirtualMemory)
                Assert.NotEqual(0UI, info.TotalPhysicalMemory)
                Assert.NotEqual(0UI, info.TotalVirtualMemory)
            End If
        End Sub

        <Fact>
        Public Sub Properties()
            Dim info As New ComputerInfoDebugView(New ComputerInfo)
            Assert.Equal(Globalization.CultureInfo.InstalledUICulture, info.InstalledUICulture)
            Assert.Equal(Environment.OSVersion.Platform.ToString(), info.OSPlatform)
            Assert.Equal(Environment.OSVersion.Version.ToString(), info.OSVersion)
        End Sub

    End Class
End Namespace
