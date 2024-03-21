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
        Private Const InvalidUrlAddress As String = "invalidURL"
        Private Const TestingConnectionTimeout As Integer = 100000

        ''' <summary>
        '''  Verify that tmpDirectoryPath exists, that destinationFilename exist and what its length is
        ''' </summary>
        ''' <param name="destinationFilename">The full path and filename of the new file</param>
        ''' <returns>
        '''  The size in bytes of the destination file, this saves the caller from having to
        '''  do another FileInfo call
        ''' </returns>
        Private Shared Function ValidateDownload(destinationFilename As String) As Long
            Dim fileInfo As New FileInfo(destinationFilename)

            ' This directory should not be systems Temp Directory because it may be created
            Assert.True(Directory.Exists(fileInfo.DirectoryName))
            Assert.True(fileInfo.Exists)

            Return fileInfo.Length
        End Function

        Private Sub CleanUp(listener As HttpListener, Optional tmpDirectoryPath As String = Nothing)
            Debug.Assert(tmpDirectoryPath <> Path.GetTempPath)
            listener.Stop()
            listener.Close()
            If Not String.IsNullOrWhiteSpace(tmpDirectoryPath) Then
                Assert.True(tmpDirectoryPath.StartsWith(Path.GetTempPath))
                Directory.Delete(tmpDirectoryPath, recursive:=True)
            End If
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_Url_destinationFilename_Throw(destinationFilename As String)
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(address,
                                          destinationFilename)

                    End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                My.Computer.Network _
                    .DownloadFile(address,
                                  destinationFilename)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_Url_UrlEqualsNothing_Throws(address As String)
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(address,
                                          destinationFilename)

                    End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_Password_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                My.Computer.Network _
                    .DownloadFile(address,
                                  destinationFilename,
                                  DefaultUserName,
                                  DefaultPassword)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_Url_WrongPassword_Throw(password As String)
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, "")
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of WebException)(
                     Sub()
                         My.Computer.Network _
                             .DownloadFile(address,
                                           destinationFilename,
                                           DefaultUserName,
                                           password)

                     End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Ur_InvalidUrl_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(InvalidUrlAddress,
                                          destinationFilename,
                                          userName:="",
                                          password:="",
                                          showUI:=True,
                                          TestingConnectionTimeout,
                                          overwrite:=False)

                    End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_DestinationFileNameEqualNothing_Throws()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(address,
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
        Public Sub DownloadFile_Url_FileExistsNoOverwrite_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpDirectoryPath, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Assert.Throws(Of IOException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(address,
                                      destinationFilename,
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=False)

                End Sub)

            Assert.True(Directory.Exists(tmpDirectoryPath))
            Assert.Equal(ValidateDownload(destinationFilename), 1)

            CleanUp(listener, tmpDirectoryPath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_FileExistsOverwrite_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpDirectoryPath, 1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                My.Computer.Network _
                    .DownloadFile(address,
                                  destinationFilename,
                                  userName:="",
                                  password:="",
                                  showUI:=False,
                                  TestingConnectionTimeout,
                                  overwrite:=True)

                Assert.True(Directory.Exists(tmpDirectoryPath))
                Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_InvalidUrlDoNotShowUI_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(InvalidUrlAddress,
                                          destinationFilename,
                                          userName:="",
                                          password:="",
                                          showUI:=False,
                                          TestingConnectionTimeout,
                                          overwrite:=False)

                    End Sub)

                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_TimeOut_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of WebException)(
                        Sub()
                            My.Computer.Network _
                                .DownloadFile(address,
                                              destinationFilename,
                                              userName:="",
                                              password:="",
                                              showUI:=False,
                                              connectionTimeout:=1,
                                              overwrite:=True)

                        End Sub)

                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_TimeoutNegative_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(address,
                                      destinationFilename,
                                      userName:="",
                                      password:="",
                                      showUI:=True,
                                      connectionTimeout:=-1,
                                      overwrite:=False)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_UsernameEqualNothing_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                My.Computer.Network _
                    .DownloadFile(address,
                                  destinationFilename,
                                  userName:=Nothing,
                                  password:="",
                                  showUI:=True,
                                  TestingConnectionTimeout,
                                  overwrite:=False)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_CheckFilePathTrailingSeparator_Throw(separator As String)
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(address,
                                      $"{destinationFilename}{separator}",
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=True,
                                      onUserCancel:=UICancelOption.DoNothing)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_GetLongPath_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(address,
                                      destinationFileName:=tmpDirectoryPath, ' This is a Directory!
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=True,
                                      onUserCancel:=UICancelOption.DoNothing)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_RootDirectory_Throw(root As String)
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(address,
                                      destinationFileName:=root, ' This is a Root Directory!
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=True,
                                      onUserCancel:=UICancelOption.DoNothing)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_RootDirectoryTrailingSeparator_Throw(root As String)
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(address,
                                      destinationFileName:=root, ' This is a Root Directory!
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=True,
                                      onUserCancel:=UICancelOption.DoNothing)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Url_AllOptionsSet_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim address As String = webListener.Address
            My.Computer.Network _
                .DownloadFile(address,
                              destinationFilename,
                              userName:="",
                              password:="",
                              showUI:=False,
                              TestingConnectionTimeout,
                              overwrite:=True,
                              onUserCancel:=UICancelOption.DoNothing)

            Assert.True(Directory.Exists(tmpDirectoryPath))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, tmpDirectoryPath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_ICredentialsShowUiTimeOutOverwriteSpecified_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim networkCredentials As ICredentials = New NetworkCredential(DefaultUserName, DefaultPassword)

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename,
                                  networkCredentials,
                                  showUI:=False, TestingConnectionTimeout,
                                  overwrite:=True)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_UserNamePassword_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename,
                                  DefaultUserName,
                                  DefaultPassword)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_UserNamePassword_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of WebException)(
                 Sub()
                     My.Computer.Network _
                         .DownloadFile(New Uri(webListener.Address),
                            destinationFilename,
                            DefaultUserName,
                            "WrongPassword")

                 End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_OverwriteShowUISpecified_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(tmpDirectoryPath, size:=1)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename,
                                  userName:="",
                                  password:="",
                                  showUI:=True,
                                  TestingConnectionTimeout,
                                  overwrite:=True)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadLargeFileSize)
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereAllOptionsSpecifiedExceptUserCancel_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            My.Computer.Network _
                .DownloadFile(New Uri(webListener.Address),
                              destinationFilename,
                              userName:="",
                              password:="",
                              showUI:=False,
                              TestingConnectionTimeout,
                              overwrite:=True)

            Assert.True(Directory.Exists(tmpDirectoryPath))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, tmpDirectoryPath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_AllOptionsSpecified_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            My.Computer.Network _
                .DownloadFile(New Uri(webListener.Address),
                              destinationFilename,
                              userName:="",
                              password:="",
                              showUI:=False,
                              TestingConnectionTimeout,
                              overwrite:=True,
                              UICancelOption.DoNothing)

            Assert.True(Directory.Exists(tmpDirectoryPath))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, tmpDirectoryPath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_TargetDirectoryDoesNotExist_Success()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Directory.Delete(tmpDirectoryPath, recursive:=True)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            My.Computer.Network _
                .DownloadFile(New Uri(webListener.Address),
                              destinationFilename,
                              userName:="",
                              password:="",
                              showUI:=False,
                              TestingConnectionTimeout,
                              overwrite:=True,
                              UICancelOption.DoNothing)

            Assert.True(Directory.Exists(tmpDirectoryPath))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, tmpDirectoryPath)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_Uri_UriIsNothing_Throws()
            Dim tmpDirectoryPath As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(tmpDirectoryPath)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(address:=CType(Nothing, Uri),
                                          destinationFilename,
                                          userName:="",
                                          password:="",
                                          showUI:=True,
                                          TestingConnectionTimeout,
                                          overwrite:=False)

                    End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, tmpDirectoryPath)
            End Try
        End Sub

    End Class
End Namespace
