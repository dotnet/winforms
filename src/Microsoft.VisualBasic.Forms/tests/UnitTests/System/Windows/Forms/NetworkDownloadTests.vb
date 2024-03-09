' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports Microsoft.VisualBasic.FileIO

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests
        Private Const DefaultPassword As String = "TBD"
        Private Const DefaultUserName As String = "TBD"
        Private Const DownloadLargeFileSize As Integer = 104857600
        Private Const DownloadSmallFileSize As Integer = 18135
        Private Const TestingConnectionTimeout As Integer = 100000

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

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As String, destinationFileName As String) -> Void"

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_Url_destinationFilename_Throw(destinationFilename As String)
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.DownloadFile(webListener.Address,
                            destinationFilename)
                    End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
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
        Public Sub DownloadFile_Url_UrlEqualsNothing_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
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

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As String, destinationFileName As String, userName As String, password As String) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Url_Password_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                    destinationFilename,
                    DefaultUserName,
                    DefaultPassword)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("123")>
        Public Sub DownloadFile_Url_WrongPassword_Throw(password As String)
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, "")
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of WebException)(
                     Sub()
                         My.Computer.Network.DownloadFile(webListener.Address,
                            destinationFilename,
                            DefaultUserName,
                            password)
                     End Sub)
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As String, destinationFileName As String, userName As String, password As String, showUI As Boolean, connectionTimeout As Integer, overwrite As Boolean) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Ur_InvalidUrl_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentException)(
                    Sub()
                        My.Computer.Network.DownloadFile(address:="x",
                            destinationFilename,
                            userName:="",
                            password:="",
                            showUI:=True,
                            TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_DestinationFileNameEqualNothing_Throw()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.DownloadFile(webListener.Address,
                            destinationFileName:=Nothing,
                            userName:="",
                            password:="",
                            showUI:=True,
                            TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub)
            Finally
                CleanUp(listener)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_FileExistsNoOverwrite_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Assert.Throws(Of IOException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=False)
                End Sub)
            Assert.True(Directory.Exists(tmpFilePath))
            Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), 1)
            CleanUp(listener, tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_FileExistsOverwrite_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, 1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True)
                Assert.True(Directory.Exists(tmpFilePath))
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_InvalidUrlDoNotShowUI_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentException)(
                    Sub()
                        My.Computer.Network.DownloadFile(address:="x",
                            destinationFilename,
                            userName:="",
                            password:="",
                            showUI:=False,
                            TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_TimeOut_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadLargeFileSize)
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
        Public Sub DownloadFile_Url_TimeoutNegative_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(DownloadLargeFileSize)
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
        Public Sub DownloadFile_Url_UsernameEqualNothing_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:=Nothing,
                        password:="",
                        showUI:=True,
                        TestingConnectionTimeout,
                        overwrite:=False)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As String, destinationFileName As String, userName As String, password As String, showUI As Boolean, connectionTimeout As Integer, overwrite As Boolean, onUserCancel As Microsoft.VisualBasic.FileIO.UICancelOption) -> Void"

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_CheckFilePathTrailingSeparator_Throw(separator As String)
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        $"{destinationFilename}{separator}",
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_GetLongPath_Throw()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFileName:=tmpFilePath, ' This is a Directory!
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_RootDirectory_Throw(root As String)
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFileName:=root, ' This is a Root Directory!
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_RootDirectoryTrailingSeparator_Throw(root As String)
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFileName:=root, ' This is a Root Directory!
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_AllOptionsSet_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            My.Computer.Network.DownloadFile(webListener.Address,
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)

            Assert.True(Directory.Exists(tmpFilePath))
            Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), DownloadSmallFileSize)
            CleanUp(listener, tmpFilePath)
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As System.Uri, destinationFileName As String) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Uri_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(New Uri(webListener.Address),
                    destinationFilename)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As System.Uri, destinationFileName As String, networkCredentials As System.Net.ICredentials, showUI As Boolean, connectionTimeout As Integer, overwrite As Boolean) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Uri_ICredentialsShowUiTimeOutOverwriteSpecified_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            ' Get network credentials
            Dim networkCredentials As ICredentials = New NetworkCredential(DefaultUserName, DefaultPassword)
            Try
                Dim address As Uri = New Uri(webListener.Address)
                My.Computer.Network.DownloadFile(address,
                    destinationFilename,
                    networkCredentials,
                    showUI:=False, TestingConnectionTimeout,
                    True)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As System.Uri, destinationFileName As String, userName As String, password As String) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Uri_UserNamePassword_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(New Uri(webListener.Address),
                    destinationFilename,
                    DefaultUserName,
                    DefaultPassword)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=18135)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_UserNamePassword_Throws()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, -1)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of WebException)(
                 Sub()
                     My.Computer.Network.DownloadFile(New Uri(webListener.Address),
                       destinationFilename,
                       DefaultUserName,
                       password:="123")
                 End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As System.Uri, destinationFileName As String, userName As String, password As String, showUI As Boolean, connectionTimeout As Integer, overwrite As Boolean) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Uri_OverwriteShowUISpecified_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=0)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                My.Computer.Network.DownloadFile(webListener.Address,
                    destinationFilename,
                    userName:="",
                    password:="",
                    showUI:=True,
                    TestingConnectionTimeout,
                    overwrite:=True)
                Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), actual:=104857600)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereAllOptionsSpecifiedExceptUserCancel_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            My.Computer.Network.DownloadFile(New Uri(webListener.Address),
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True)
            Assert.True(Directory.Exists(tmpFilePath))
            Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), DownloadSmallFileSize)
            CleanUp(listener, tmpFilePath)
        End Sub

#End Region

#Region "Microsoft.VisualBasic.Devices.Network.DownloadFile(address As System.Uri, destinationFileName As String, userName As String, password As String, showUI As Boolean, connectionTimeout As Integer, overwrite As Boolean, onUserCancel As Microsoft.VisualBasic.FileIO.UICancelOption) -> Void"

        <WinFormsFact>
        Public Sub DownloadFile_Uri_AllOptionsSpecified_Success()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, DownloadSmallFileSize)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            My.Computer.Network.DownloadFile(New Uri(webListener.Address),
                        destinationFilename,
                        userName:="",
                        password:="",
                        showUI:=False,
                        TestingConnectionTimeout,
                        overwrite:=True,
                        FileIO.UICancelOption.DoNothing)
            Assert.True(Directory.Exists(tmpFilePath))
            Assert.Equal(ValidateDownload(tmpFilePath, destinationFilename), DownloadSmallFileSize)
            CleanUp(listener, tmpFilePath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_UriIsNothing_Throws()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpFilePath, size:=-1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.DownloadFile(address:=CType(Nothing, Uri),
                            destinationFilename,
                            userName:="",
                            password:="",
                            showUI:=True,
                            TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub)
            Finally
                CleanUp(listener, tmpFilePath)
            End Try
        End Sub

#End Region

    End Class

End Namespace
