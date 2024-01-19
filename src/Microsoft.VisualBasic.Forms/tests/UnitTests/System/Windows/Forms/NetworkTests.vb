' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests

        Private Const DownloadFileUrl As String = "https://raw.githubusercontent.com/dotnet/winforms/main/docs/testing.md"
        Private Const DownloadLargeFileUrl As String = "https://ash-speed.hetzner.com/100MB.bin"
        Private ReadOnly _baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")

        Private ReadOnly _lockObject As New Object

        Private Shared Function ValidateDownLoad(tmpFilePath As String, destinationFileName As String) As Long
            Assert.True(Directory.Exists(tmpFilePath))
            Dim fileInfo As New FileInfo(destinationFileName)
            Assert.True(fileInfo.Exists)
            Return fileInfo.Length
        End Function

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
        ''' This gets a master test folder using a predefined Guid
        ''' So all temp directories and files end up in 1 place
        ''' </summary>
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
        Public Sub DownloadWithNegativeTimeout()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.DownloadFile(DownloadLargeFileUrl,
                                                     destinationFileName,
                                                     "",
                                                     "",
                                                     True,
                                                     -1,
                                                     False
                                                    )
                End Sub)
            Assert.False(File.Exists(destinationFileName))
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingDestinationFile()
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                     Nothing,
                                                     "",
                                                     "",
                                                     True,
                                                     100000,
                                                     False
                                                    )
                End Sub)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingPassword()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(DownloadFileUrl,
                                               destinationFileName,
                                               "",
                                               Nothing,
                                               True,
                                               100000,
                                               False
                                           )
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingURI()
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    Dim tmpFilePath As String = CreateTempDirectory()
                    Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
                    My.Computer.Network.DownloadFile(CType(Nothing, Uri),
                                                     destinationFileName,
                                                     "",
                                                     "",
                                                     True,
                                                     100000,
                                                     False
                                                    )
                End Sub)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingUsername()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                destinationFileName,
                                                Nothing,
                                                "",
                                                True,
                                                100000,
                                                False
                                            )
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingWebAddress()
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    Dim tmpFilePath As String = CreateTempDirectory()
                    Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
                    My.Computer.Network.DownloadFile(CStr(Nothing),
                                                     destinationFileName,
                                                     "",
                                                     "",
                                                     True,
                                                     100000,
                                                     False
                                                    )
                End Sub)
        End Sub

        <WinFormsFact>
        Public Sub FileDownloadWithJustUriAndDestinationFileName_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl),
                                             destinationFileName)
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub FileDownloadWithJustUrlAndDestinationFileName_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(DownloadFileUrl,
                                             destinationFileName)
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub FileDownloadWithUriAndDestinationFileNameUserNamePassword_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl),
                                             destinationFileName,
                                             "TDB",
                                             "TBD")
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWhereFileExistsNoOverwrite()
            Assert.Throws(Of IOException)(Sub()
                                              Dim tmpFilePath As String = CreateTempDirectory()
                                              Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
                                              Directory.CreateDirectory(tmpFilePath)
                                              Dim destinationStream As FileStream = File.Create(destinationFileName)
                                              destinationStream.Close()
                                              Try
                                                  My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                                                      destinationFileName,
                                                                                      "",
                                                                                      "",
                                                                                      False,
                                                                                      100000,
                                                                                      overwrite:=False)
                                              Finally
                                                  Assert.True(Directory.Exists(tmpFilePath))
                                                  Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 0)
                                                  CleanupTempDirectory(tmpFilePath)
                                              End Try
                                          End Sub
                                             )

        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithExpectedTimeOut_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName = Path.Combine(tmpFilePath, "testing.md")
            Try

                Assert.Throws(Of WebException)(Sub()
                                                   My.Computer.Network.DownloadFile(DownloadLargeFileUrl,
                                                                                    destinationFileName,
                                                                                    "",
                                                                                    "",
                                                                                    False,
                                                                                    1,
                                                                                    True)
                                               End Sub
                                               )
            Finally
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.False(File.Exists(destinationFileName))
                CleanupTempDirectory(tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithJustUriAndDestinationFileName_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl),
                                             destinationFileName)
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithUI_method()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
            Directory.CreateDirectory(tmpFilePath)
            Dim destinationStream As FileStream = File.Create(destinationFileName)
            destinationStream.Close()
            My.Computer.Network.DownloadFile(DownloadLargeFileUrl,
                                             destinationFileName,
                                             "",
                                             "",
                                             True,
                                             100000,
                                             True
                                            )
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 104857600)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub SimpleFileDownloadWithUIDoNotOverwrite_method()
            Assert.Throws(Of IOException)(Sub()
                                              Dim tmpFilePath As String = CreateTempDirectory()
                                              Dim destinationFileName As String = Path.Combine(tmpFilePath, "testing.md")
                                              Directory.CreateDirectory(tmpFilePath)
                                              Dim destinationStream As FileStream = File.Create(destinationFileName)
                                              destinationStream.Close()
                                              Try
                                                  My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                 destinationFileName,
                                                 "",
                                                 "",
                                                 True,
                                                 100000,
                                                 False
                                                )
                                              Finally
                                                  Assert.True(Directory.Exists(tmpFilePath))
                                                  Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFileName), 0)
                                                  CleanupTempDirectory(tmpFilePath)
                                              End Try
                                          End Sub)
        End Sub

    End Class

End Namespace
