' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict Off

Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.Logging
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class FileLogTraceListenerTests
        Inherits VbFileCleanupTestBase

        <WinFormsFact>
        Public Sub ListenerProperties()
            Dim testCode As Action =
                Sub()
                    Dim testDirectory As String = CreateTempDirectory()
                    Using listener As New FileLogTraceListener(NameOf(FileLogTraceListenerTests)) With {
                        .Location = LogFileLocation.Custom,
                        .CustomLocation = testDirectory}

                        Dim expectedBaseFileName As String = Path.GetFileNameWithoutExtension(Application.ExecutablePath)
                        listener.BaseFileName.Should.BeEquivalentTo(expectedBaseFileName)

                        listener.Name.Should.NotBeNull()

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

                        listener.FullLogFileName.Should.BeEquivalentTo(Path.Combine(testDirectory, $"{expectedBaseFileName}.log"))

                        listener.LogFileCreationSchedule.Should.Be(LogFileCreationScheduleOption.None)
                        listener.LogFileCreationSchedule = LogFileCreationScheduleOption.Daily
                        listener.LogFileCreationSchedule.Should.Be(LogFileCreationScheduleOption.Daily)
                        listener.LogFileCreationSchedule = LogFileCreationScheduleOption.Weekly
                        listener.LogFileCreationSchedule.Should.Be(LogFileCreationScheduleOption.Weekly)

                        listener.MaxFileSize.Should.Be(5_000_000L)
                        listener.MaxFileSize = 500_000L
                        listener.MaxFileSize.Should.Be(500_000L)

                        listener.ReserveDiskSpace.Should.Be(10_000_000L)
                        listener.ReserveDiskSpace = 1_000_000L
                        listener.ReserveDiskSpace.Should.Be(1_000_000L)

                        listener.Delimiter.Should.Be(vbTab)
                        listener.Delimiter = ";"
                        listener.Delimiter.Should.Be(";")

                        listener.Encoding.Should.Be(Encoding.UTF8)
                        listener.Encoding = Encoding.ASCII
                        listener.Encoding.Should.Be(Encoding.ASCII)

                        listener.TraceOutputOptions.Should.Be(TraceOptions.None)
                        listener.TraceOutputOptions = TraceOptions.Callstack
                        listener.TraceOutputOptions.Should.Be(TraceOptions.Callstack)

                        CStr(listener.TestAccessor().Dynamic.HostName).Should.NotBeEmpty()

                        Dim listenerStream As FileLogTraceListener.ReferencedStream = CType(listener.TestAccessor().Dynamic.ListenerStream, FileLogTraceListener.ReferencedStream)
                        listenerStream.Should.NotBeNull()
                        listenerStream.IsInUse.Should.BeTrue()
                        listenerStream.FileSize.Should.Be(0)

                    End Using
                End Sub

            If DirectoryIsAccessible(Application.CommonAppDataPath) Then
                testCode.Should.NotThrow()
            End If
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub ListenerSetInvalidBaseFileNamePropertyThrows(value As String)
            Dim testDirectory As String = CreateTempDirectory()
            Using listener As New FileLogTraceListener(NameOf(FileLogTraceListenerTests)) _
                With {.Location = LogFileLocation.Custom, .CustomLocation = testDirectory}

                CType(Sub()
                          listener.BaseFileName = value
                      End Sub, Action).Should.Throw(Of ArgumentNullException)()
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData(-1)>
        <InlineData(0)>
        <InlineData(999)>
        Public Sub ListenerSetInvalidMaxFileSizePropertiesThrows(value As Long)
            Dim testDirectory As String = CreateTempDirectory()
            Using listener As New FileLogTraceListener(NameOf(FileLogTraceListenerTests)) _
                With {.Location = LogFileLocation.Custom, .CustomLocation = testDirectory}

                CType(Sub()
                          listener.MaxFileSize = value
                      End Sub, Action).Should.Throw(Of ArgumentException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub ListenerSetInvalidReserveDiskSpacePropertiesThrows()
            Dim testDirectory As String = CreateTempDirectory()
            Using listener As New FileLogTraceListener(NameOf(FileLogTraceListenerTests)) _
                With {.Location = LogFileLocation.Custom, .CustomLocation = testDirectory}

                CType(Sub()
                          listener.ReserveDiskSpace = -1
                      End Sub, Action).Should.Throw(Of ArgumentException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub ListenerSetInvalidTraceOptionsPropertiesThrows()
            Dim testDirectory As String = CreateTempDirectory()
            Using listener As New FileLogTraceListener(NameOf(FileLogTraceListenerTests)) _
                With {.Location = LogFileLocation.Custom, .CustomLocation = testDirectory}

                CType(Sub()
                          listener.TraceOutputOptions = DirectCast(-1, TraceOptions)
                      End Sub, Action).Should.Throw(Of ArgumentException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithCommonApplicationDirectoryLocation()
            Dim testCode As Action =
                Sub()
                    Dim fullLogFileName As String
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.CommonApplicationDirectory}

                        fullLogFileName = listener.FullLogFileName
                        fullLogFileName.Should.StartWith(Application.CommonAppDataPath)
                    End Using
                    If File.Exists(fullLogFileName) Then
                        File.Delete(fullLogFileName)
                    End If
                End Sub

            If DirectoryIsAccessible(Application.CommonAppDataPath) Then
                testCode.Should.NotThrow()
            End If
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithCustomLocation()
            Dim testCode As Action =
                Sub()
                    Dim testDirectory As String = CreateTempDirectory()
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.Custom,
                        .CustomLocation = testDirectory}

                        listener.Location.Should.Be(LogFileLocation.Custom)
                        listener.CustomLocation.Should.Be(testDirectory)
                    End Using
                End Sub

            testCode.Should.NotThrow()
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithCustomLocationEmptyThrows()
            Dim testCode As Action =
                Sub()
                    Dim testDirectory As String = CreateTempDirectory()
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.Custom,
                        .CustomLocation = String.Empty}
                    End Using
                End Sub

            testCode.Should.Throw(Of ArgumentException)()
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithCustomLocationNullThrows()
            Dim testCode As Action =
                Sub()
                    Dim testDirectory As String = CreateTempDirectory()
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.Custom,
                        .CustomLocation = Nothing}
                    End Using
                End Sub

            testCode.Should.Throw(Of ArgumentNullException)()
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithExecutableDirectoryLocation()
            Dim testCode As Action =
                Sub()
                    Dim fullLogFileName As String
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.ExecutableDirectory}

                        fullLogFileName = listener.FullLogFileName
                        fullLogFileName.Should.StartWith(Path.GetDirectoryName(Application.ExecutablePath))
                    End Using
                    If File.Exists(fullLogFileName) Then
                        File.Delete(fullLogFileName)
                    End If
                End Sub

            If DirectoryIsAccessible(Path.GetDirectoryName(Application.ExecutablePath)) Then
                testCode.Should.NotThrow()
            End If
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithLocalUserApplicationDirectoryLocation()
            Dim testCode As Action =
                Sub()
                    Dim fullLogFileName As String
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.LocalUserApplicationDirectory}

                        fullLogFileName = listener.FullLogFileName
                        fullLogFileName.Should.StartWith(Application.UserAppDataPath)
                    End Using
                    If File.Exists(fullLogFileName) Then
                        File.Delete(fullLogFileName)
                    End If
                End Sub

            If DirectoryIsAccessible(Application.UserAppDataPath) Then
                testCode.Should.NotThrow()
            End If
        End Sub

        <WinFormsFact>
        Public Sub LocationPropertyWithTempDirectoryLocation()
            Dim testCode As Action =
                Sub()
                    Dim fullLogFileName As String
                    Using listener As New FileLogTraceListener() With {
                        .Location = LogFileLocation.TempDirectory}

                        fullLogFileName = listener.FullLogFileName
                        fullLogFileName.Should.StartWith(Path.GetTempPath)
                    End Using
                    If File.Exists(fullLogFileName) Then
                        File.Delete(fullLogFileName)
                    End If
                End Sub

            If DirectoryIsAccessible(Application.UserAppDataPath) Then
                testCode.Should.NotThrow()
            End If
        End Sub

    End Class
End Namespace
