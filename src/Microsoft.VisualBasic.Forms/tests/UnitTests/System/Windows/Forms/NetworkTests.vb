' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests

        Private Const DownloadFileUrl As String = "https://raw.githubusercontent.com/dotnet/winforms/main/docs/testing.md"
        Private Const DownloadLargeFileUrl As String = "https://ash-speed.hetzner.com/100MB.bin"
        Private Const UploadFileUrl As String = "ftp://speedtest.tele2.net/upload/testing.md"

        Private ReadOnly _baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")
        Private ReadOnly _lockObject As New Object

        ''' <summary>
        ''' If size >= 0 then create the file with size length
        ''' The file will contain the letters A-Z repeating as needed.
        ''' </summary>
        ''' <param name="tmpFilePath">Full path to working directory</param>
        ''' <param name="size">File size to be created</param>
        ''' <returns></returns>
        Private Shared Function CreateTempFile(tmpFilePath As String, Optional size As Integer = -1) As String
            Dim filename As String = Path.Combine(tmpFilePath, "testing.md")
            If size >= 0 Then
                Using destinationStream As FileStream = File.Create(filename)
                    For i As Long = 0 To size - 1
                        destinationStream.WriteByte(CByte((AscW("A") + (i Mod 26))))
                    Next
                    destinationStream.Flush()
                    destinationStream.Close()
                End Using
            End If
            Return filename
        End Function

        Private Shared Function ValidateDownload(tmpFilePath As String, destinationFilename As String) As Long
            Assert.True(Directory.Exists(tmpFilePath))
            Dim fileInfo As New FileInfo(destinationFilename)
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
        ''' This gets a master test folder using a predefined Guid,
        ''' so all temp directories and files end up in one place.
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
                Directory.CreateDirectory(folder)
            End SyncLock
            Return folder
        End Function

        <WinFormsFact>
        Public Sub DownloadWhereFileExistsNoOverwrite()
            Assert.Throws(Of IOException)(
                Sub()
                    Dim tmpFilePath As String = CreateTempDirectory()
                    Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
                    Try
                        My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                         destinationFilename,
                                                         "",
                                                         "",
                                                         False,
                                                         100000,
                                                         overwrite:=False
                                                        )
                    Finally
                        Assert.True(Directory.Exists(tmpFilePath))
                        Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 1)
                        CleanupTempDirectory(tmpFilePath)
                    End Try
                End Sub
                                             )

        End Sub

        <WinFormsFact>
        Public Sub DownloadWhereFileExistsOverwrite()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Try
                My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                 destinationFilename,
                                                 "",
                                                 "",
                                                 False,
                                                 100000,
                                                 True
                                                )
            Finally
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 18135)
                CleanupTempDirectory(tmpFilePath)
            End Try

        End Sub

        <WinFormsFact>
        Public Sub DownloadWhereTimeoutNegative()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.DownloadFile(DownloadLargeFileUrl,
                                                     destinationFilename,
                                                     "",
                                                     "",
                                                     True,
                                                     -1,
                                                     False
                                                    )
                End Sub)
            Assert.False(File.Exists(destinationFilename))
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithExpectedTimeOut()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Try

                Assert.Throws(Of WebException)(
                    Sub()
                        My.Computer.Network.DownloadFile(DownloadLargeFileUrl,
                                                         destinationFilename,
                                                         "",
                                                         "",
                                                         False,
                                                         1,
                                                         True
                                                        )
                    End Sub
                                               )
            Finally
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.False(File.Exists(destinationFilename))
                CleanupTempDirectory(tmpFilePath)
            End Try
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
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                destinationFilename,
                                                "",
                                                Nothing
                                            )
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingUri()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    My.Computer.Network.DownloadFile(CType(Nothing, Uri),
                                                     destinationFilename,
                                                     "",
                                                     "",
                                                     True,
                                                     100000,
                                                     False
                                                    )
                End Sub)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingUsername()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            My.Computer.Network.DownloadFile(DownloadFileUrl,
                                                destinationFilename,
                                                Nothing,
                                                "",
                                                True,
                                                100000,
                                                False
                                            )
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingWebAddress()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    My.Computer.Network.DownloadFile(CStr(Nothing),
                                                     destinationFilename
)
                End Sub)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUI()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 0)
            My.Computer.Network.DownloadFile(DownloadLargeFileUrl,
                                             destinationFilename,
                                             "",
                                             "",
                                             True,
                                             100000,
                                             True
                                            )
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 104857600)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUriFilename()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl),
                                             destinationFilename)
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUriFilenameAndUserCredentials()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            My.Computer.Network.DownloadFile(New Uri(DownloadFileUrl),
                                             destinationFilename,
                                             "TDB",
                                             "TBD")
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUrlDestinationFilename()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            My.Computer.Network.DownloadFile(DownloadFileUrl,
                                             destinationFilename)
            Assert.Equal(ValidateDownLoad(tmpFilePath, destinationFilename), 18135)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub UploadFilenameUrl()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, 1)
            My.Computer.Network.UploadFile(sourceFilename, UploadFileUrl)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub UploadWhereTimeoutNegative()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, &H100_0000)
            Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.UploadFile(sourceFilename,
                                                   UploadFileUrl,
                                                   "anonymous",
                                                   "anonymous",
                                                   True,
                                                   -1
                                                  )
                End Sub)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact(Skip:="Issue #10706")>
        Public Sub UploadWithExpectedTimeOut()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, &H10_0000)
            Try

                Assert.Throws(Of WebException)(
                    Sub()
                        My.Computer.Network.UploadFile(sourceFilename,
                                                       UploadFileUrl,
                                                       "anonymous",
                                                       "anonymous",
                                                       True,
                                                       1
                                                      )
                    End Sub)
            Finally
                CleanupTempDirectory(tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub UploadWithNothingPassword()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim filename As String = CreateTempFile(tmpFilePath, 1)
            Try
                My.Computer.Network.UploadFile(filename,
                                               UploadFileUrl,
                                               "anonymous",
                                               Nothing
                                              )
            Finally
                CleanupTempDirectory(tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub UploadWithNothingSourceFile()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, 1)
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.UploadFile(Nothing,
                                                       UploadFileUrl,
                                                       "anonymous",
                                                       "anonymous"
                                                      )
                    End Sub)
            Finally
                CleanupTempDirectory(tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub UploadWithNothingUri()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    My.Computer.Network.DownloadFile(CType(Nothing, Uri),
                                                     destinationFilename,
                                                     "anonymous",
                                                     "anonymous"
                                                    )
                End Sub)
            CleanupTempDirectory(tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub UploadWithUI()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Try
                My.Computer.Network.UploadFile(destinationFilename,
                                               UploadFileUrl,
                                               "anonymous",
                                               "anonymous",
                                               True,
                                               100000
                                              )
            Finally
                CleanupTempDirectory(tmpFilePath)
            End Try
        End Sub

    End Class

End Namespace
