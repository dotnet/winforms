' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests

        Private ReadOnly _downloadFileSize As Integer = 18135
        Private ReadOnly _downloadLargeFileSize As Integer = 104857600

        Private Shared Function ValidateDownload(tmpFilePath As String, destinationFilename As String) As Long
            Assert.True(Directory.Exists(tmpFilePath))
            Dim fileInfo As New FileInfo(destinationFilename)
            Assert.True(fileInfo.Exists)
            Return fileInfo.Length
        End Function

        Private Sub CleanUp(listener As HttpListener, Optional tmpFilePath As String = Nothing)
            listener.Stop()
            listener.Close()
            If tmpFilePath IsNot Nothing Then
                Directory.Delete(tmpFilePath, True)
            End If
        End Sub

        <WinFormsFact>
        Public Sub DownloadWhereFileExistsNoOverwrite()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Assert.Throws(Of IOException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        connectionTimeout:=100000,
                        overwrite:=False)
                End Sub)
            Assert.True(Directory.Exists(tmpFilePath))
            Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), 1)
            CleanUp(listener, tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadWhereFileExistsOverwrite()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        connectionTimeout:=100000,
                        overwrite:=True)
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), 18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWhereTimeoutNegative()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(_downloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=True,
                        connectionTimeout:=-1,
                        overwrite:=False)
                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithExpectedTimeOut()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(_downloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of WebException)(
                        Sub()
                            My.Computer.Network.DownloadFile(webListener.Address,
                                destinationFilename,
                                userName:="",
                                password:="",
                                showUI:=False,
                                connectionTimeout:=1,
                                overwrite:=True)
                        End Sub)
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingDestinationFile()
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.DownloadFile(webListener.Address,
                            destinationFileName:=Nothing,
                            userName:="",
                            password:="",
                            showUI:=True,
                            connectionTimeout:=100000,
                            overwrite:=False)
                    End Sub)
            Finally
                CleanUp(listener)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingPassword()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                    destinationFilename,
                    userName:="",
                    password:=Nothing)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingUri()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.DownloadFile(address:=CType(Nothing, Uri),
                            destinationFilename,
                            userName:="",
                            password:="",
                            showUI:=True,
                            connectionTimeout:=100000,
                            overwrite:=False)
                    End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingUsername()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:=Nothing,
                        password:="",
                        showUI:=True,
                        connectionTimeout:=100000,
                        overwrite:=False)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithNothingWebAddress()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.DownloadFile(address:=CStr(Nothing),
                            destinationFilename)
                    End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUI()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=0)
            Dim webListener As New WebListener(_downloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                    destinationFilename,
                    userName:="",
                    password:="",
                    showUI:=True,
                    connectionTimeout:=100000,
                    overwrite:=True)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=104857600)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUriFilename()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                    destinationFilename)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUriFilenameAndUserCredentials()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                    destinationFilename,
                    userName:="TDB",
                    password:="TBD")
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadWithUrlDestinationFilename()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(_downloadFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                   destinationFilename)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

    End Class

End Namespace
