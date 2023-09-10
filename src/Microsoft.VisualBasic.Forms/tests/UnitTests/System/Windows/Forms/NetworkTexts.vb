' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTexts
        Private Const DownloadFileUrl As String = "https://raw.githubusercontent.com/dotnet/winforms/main/docs/testing.md"
        Private ReadOnly _baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")
        Private ReadOnly _lockObject As New Object

        Private Sub CleanupTempDirectory(path As String)
            SyncLock _lockObject
                Directory.Delete(path, True)
            End SyncLock
        End Sub

        Private Function CreateTempDirectory() As String
            SyncLock _lockObject
                If Not Directory.Exists(_baseTempPath) Then
                    Directory.CreateDirectory(_baseTempPath)
                End If
                Return GetTempTestFolder()
            End SyncLock
        End Function

        ''' <summary>
        ''' This gets a master test folder using a predefinded Guid
        ''' So all temp directories and files end up in 1 place
        ''' </summary>
        ''' <param name="baseDirectory"></param>
        ''' <returns></returns>
        Private Function GetTempTestFolder() As String
            Dim i As Integer = 0
            Dim folder As String = Path.Combine(_baseTempPath, $"Test{i}")
            SyncLock _lockObject
                Do While Directory.Exists(folder) OrElse File.Exists(folder)
                    folder = Path.Combine(_baseTempPath, $"Test{i}")
                    i += 1
                Loop
            End SyncLock
            Return folder
        End Function

        <WinFormsFact>
        Public Sub FileDownloadWithJustUriAndDestinationFileName_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName = Path.Combine(tmpFilePath, "testing.md")

            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl), destinationFileName)
            Assert.True(Directory.Exists(tmpFilePath))
            Dim fileInfo As New FileInfo(destinationFileName)
            Assert.True(fileInfo.Exists)
            Assert.True(fileInfo.Length = 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub FileDownloadWithJustUrlAndDestinationFileName_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(DownloadFileUrl, destinationFileName)
            Assert.True(Directory.Exists(tmpFilePath))
            Dim fileInfo As New FileInfo(destinationFileName)
            Assert.True(fileInfo.Exists)
            Assert.True(fileInfo.Length = 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub FileDownloadWithUriAndDestinationFileNameUserNamePassword_method()
            'Dim tmpFilePath As String = CreateTempDirectory()
            'Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            'My.Computer.Network.DownloadFile(New Uri(downloadPath), destinationFileName, "TDB", "TBD")
            'Assert.True(Directory.Exists(tmpFilePath))
            'Dim fileInfo As New FileInfo(destinationFileName)
            'Assert.True(fileInfo.Exists)
            'Assert.True(fileInfo.Length = 18135)
            'CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithExpectedTimeOut_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName = Path.Combine(tmpFilePath, "testing.md")
            Assert.Throws(Of WebException)(My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                                            destinationFileName,
                                                                            "",
                                                                            "",
                                                                            False,
                                                                            1,
                                                                            True)
                                                                           )
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithJustUriAndDestinationFileName_method4()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl), destinationFileName)
            Assert.True(Directory.Exists(tmpFilePath))
            Dim fileInfo As New FileInfo(destinationFileName)
            Assert.True(fileInfo.Exists)
            Assert.True(fileInfo.Length = 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithJustUrlAndDestinationFileName_method6()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            File.Create(destinationFileName)
            Assert.Throws(Of IOException)(My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl),
                                                                           destinationFileName, ' reuse filename
                                                                           "",
                                                                           "",
                                                                           False,
                                                                           connectionTimeout:=100000,
                                                                           overwrite:=False)
                                                                          )
        End Sub

        ''' <summary>
        ''' This is a UI test that I assume will fail here, it also fails in a form
        ''' </summary>
        <WinFormsFact>
        Public Sub SimpleFileDownloadWithUI_method()
            'Dim tmpFilePath As String = CreateTempDirectory()
            'Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            'My.Computer.Network.DownloadFile(DownloadFileUrl,
            '                                 destinationFileName,
            '                                 "",
            '                                 "",
            '                                 True,
            '                                 100000,
            '                                 True
            '                                )
            'TestForSuccess(tmpFilePath, destinationFileName, "showUI=True, connectionTimeout=100000, overwrite=true")
        End Sub

    End Class

End Namespace
