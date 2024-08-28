' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.Logging
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class FileLogTraceListenerTests
        Inherits VbFileCleanupTestBase

        <Fact>
        Public Sub LocationPropertyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Using listener As New FileLogTraceListener() With {
                .Location = LogFileLocation.Custom,
                .CustomLocation = testDirectory
            }
                listener.Location.Should.Be(LogFileLocation.Custom)
                listener.CustomLocation.Should.Be(testDirectory)
            End Using

            Using listener As New FileLogTraceListener() With {
                .Location = LogFileLocation.TempDirectory}

                listener.FullLogFileName.Should.StartWith(Path.GetTempPath)
            End Using

            Using listener As New FileLogTraceListener() With {
                .Location = LogFileLocation.LocalUserApplicationDirectory}

                listener.FullLogFileName.Should.StartWith(Application.UserAppDataPath)
            End Using

            Using listener As New FileLogTraceListener() With {
                .Location = LogFileLocation.CommonApplicationDirectory}

                listener.FullLogFileName.Should.StartWith(Application.CommonAppDataPath)
            End Using

            Using listener As New FileLogTraceListener() With {
                .Location = LogFileLocation.ExecutableDirectory}

                listener.FullLogFileName.Should.StartWith(Path.GetDirectoryName(Application.ExecutablePath))
            End Using

        End Sub

        <Fact>
        Public Sub PropertiesTest()
            Dim testDirectory As String = CreateTempDirectory()
            Using listener As New FileLogTraceListener() With {
                .Location = LogFileLocation.Custom,
                .CustomLocation = testDirectory
            }

                listener.Location.Should.Be(LogFileLocation.Custom)
                listener.CustomLocation.Should.Be(testDirectory)

                listener.BaseFileName.Should.BeEquivalentTo("testHost")

                listener.AutoFlush.Should.BeFalse()
                listener.AutoFlush = True
                listener.AutoFlush.Should.BeTrue()

                listener.IncludeHostName.Should.BeFalse()
                listener.IncludeHostName = True
                listener.IncludeHostName.Should.BeTrue()

                listener.Append.Should.BeTrue()
                listener.Append = False
                listener.Append.Should.BeFalse()

                listener.DiskSpaceExhaustedBehavior.Should.Be(DiskSpaceExhaustedOption.DiscardMessages)
                listener.DiskSpaceExhaustedBehavior = DiskSpaceExhaustedOption.ThrowException
                listener.DiskSpaceExhaustedBehavior.Should.Be(DiskSpaceExhaustedOption.ThrowException)

                listener.FullLogFileName.Should.BeEquivalentTo(Path.Combine(testDirectory, "testHost.log"))

                listener.LogFileCreationSchedule.Should.Be(LogFileCreationScheduleOption.None)
                listener.LogFileCreationSchedule = LogFileCreationScheduleOption.Daily
                listener.LogFileCreationSchedule.Should.Be(LogFileCreationScheduleOption.Daily)

                listener.MaxFileSize.Should.Be(5_000_000)
                listener.MaxFileSize = 500_000
                listener.MaxFileSize.Should.Be(500_000)

                listener.ReserveDiskSpace.Should.Be(10_000_000)
                listener.ReserveDiskSpace = 1_000_000
                listener.ReserveDiskSpace.Should.Be(1_000_000)

                listener.Delimiter.Should.Be(vbTab)
                listener.Delimiter = ";"
                listener.Delimiter.Should.Be(";")

                listener.Encoding.EncodingName.Should.Be("Unicode (UTF-8)")
                listener.Encoding = Text.Encoding.ASCII
                listener.Encoding.EncodingName.Should.Be("US-ASCII")
            End Using
        End Sub

    End Class
End Namespace
