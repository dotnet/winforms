﻿' Licensed to the .NET Foundation under one or more agreements.
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
        '''  Verify that testDirectory exists, that destinationFilename exist and what its length is
        ''' </summary>
        ''' <param name="destinationFilename">The full path and filename of the new file</param>
        ''' <returns>
        '''  The size in bytes of the destination file, this saves the caller from having to
        '''  do another FileInfo call
        ''' </returns>
        Private Shared Function ValidateDownload(destinationFilename As String) As Long
            Dim fileInfo As New FileInfo(destinationFilename)

            ' This directory should not be systems Temp Directory because it must be created
            Assert.NotEqual(Path.GetTempPath, fileInfo.DirectoryName)
            Assert.True(Directory.Exists(fileInfo.DirectoryName))
            Assert.True(fileInfo.Exists)

            Return fileInfo.Length
        End Function

        Private Sub CleanUp(listener As HttpListener, Optional testDirectory As String = Nothing)
            Debug.Assert(Path.GetTempPath <> testDirectory)
            listener.Stop()
            listener.Close()
            If Not String.IsNullOrWhiteSpace(testDirectory) Then
                Assert.True(testDirectory.StartsWith(Path.GetTempPath, StringComparison.InvariantCultureIgnoreCase))
                Directory.Delete(testDirectory, recursive:=True)
            End If
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriCheckDestinationIsDirectory_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(New Uri(webListener.Address),
                                      destinationFileName:=testDirectory, ' This is a Directory!
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=True,
                                      onUserCancel:=UICancelOption.DoNothing)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UriCheckFilePathTrailingSeparators_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(New Uri(webListener.Address),
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UriCheckRootDirectoryTrailingSeparator_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(New Uri(webListener.Address),
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_UriCheckRootDirectoryValidation_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(New Uri(webListener.Address),
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereTimeoutNegative_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(New Uri(webListener.Address),
                                      destinationFilename,
                                      userName:="",
                                      password:="",
                                      showUI:=True,
                                      connectionTimeout:=-1,
                                      overwrite:=False)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereUriIsNothingSpecifyOnUserCancel_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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
                                          overwrite:=False,
                                          UICancelOption.DoNothing)

                    End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsSpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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

            Assert.True(Directory.Exists(testDirectory))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsSpecifiedExceptUserCancel_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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

            Assert.True(Directory.Exists(testDirectory))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithICredentialsShowUiTimeOutOverwriteSpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim networkCredentials As ICredentials = New NetworkCredential(DefaultUserName, DefaultPassword)

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename,
                                  networkCredentials,
                                  showUI:=False,
                                  TestingConnectionTimeout,
                                  overwrite:=True)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithNonexistentTargetDirectory_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Directory.Delete(testDirectory, recursive:=True)
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

            Assert.True(Directory.Exists(testDirectory))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithOverwriteShowUISpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(testDirectory, size:=1)
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithUriOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(New Uri(webListener.Address),
                                  destinationFilename)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithUserNamePassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UriWithWrongPassword_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of WebException)(
                 Sub()
                     My.Computer.Network _
                         .DownloadFile(New Uri(webListener.Address),
                            destinationFilename,
                            DefaultUserName,
                            password)

                 End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlCheckDestinationIsDirectory_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(webListener.Address,
                                      destinationFileName:=testDirectory, ' This is a Directory!
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=True,
                                      onUserCancel:=UICancelOption.DoNothing)

                End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UrlCheckFilePathTrailingSeparators_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(webListener.Address,
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UrlCheckRootDirectoryTrailingSeparator_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(webListener.Address,
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_UrlCheckRootDirectoryValidation_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of InvalidOperationException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(webListener.Address,
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(webListener.Address,
                                  destinationFilename)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereAddressInvalid_Throws(address As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereDestinationFilenameInvalidAddressOnly_Throws(destinationFilename As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(webListener.Address,
                                          destinationFilename)

                    End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereDestinationFileNameInvalidOverwrite_Throws(destinationFilename As String)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(webListener.Address,
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

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereDestinationFileNameInvalidWithOnUerCancel_Throws(destinationFilename As String)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network _
                            .DownloadFile(webListener.Address,
                                          destinationFileName:=Nothing,
                                          userName:="",
                                          password:="",
                                          showUI:=True,
                                          TestingConnectionTimeout,
                                          overwrite:=False,
                                          UICancelOption.DoNothing)

                    End Sub)
            Finally
                CleanUp(listener)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereFileExistsNoOverwrite_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Assert.Throws(Of IOException)(
                Sub()
                    My.Computer.Network _
                        .DownloadFile(webListener.Address,
                                      destinationFilename,
                                      userName:="",
                                      password:="",
                                      showUI:=False,
                                      TestingConnectionTimeout,
                                      overwrite:=False)

                End Sub)

            Assert.True(Directory.Exists(testDirectory))
            Assert.Equal(ValidateDownload(destinationFilename), 1)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereInvalidUrlDoNotShowUI_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereUrlInvalid_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
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
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(webListener.Address,
                                  destinationFilename,
                                  userName:=Nothing,
                                  password:="",
                                  showUI:=True,
                                  TestingConnectionTimeout,
                                  overwrite:=False)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsSpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            My.Computer.Network _
                .DownloadFile(webListener.Address,
                              destinationFilename,
                              userName:="",
                              password:="",
                              showUI:=False,
                              TestingConnectionTimeout,
                              overwrite:=True,
                              onUserCancel:=UICancelOption.DoNothing)

            Assert.True(Directory.Exists(testDirectory))
            Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithOverwriteWhereDestinationFileExists_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(webListener.Address,
                                  destinationFilename,
                                  userName:="",
                                  password:="",
                                  showUI:=False,
                                  TestingConnectionTimeout,
                                  overwrite:=True)

                Assert.True(Directory.Exists(testDirectory))
                Assert.Equal(ValidateDownload(destinationFilename), DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithPassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network _
                    .DownloadFile(webListener.Address,
                                  destinationFilename,
                                  DefaultUserName,
                                  DefaultPassword)

                Assert.Equal(ValidateDownload(destinationFilename), actual:=DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithTimeOut_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of WebException)(
                        Sub()
                            My.Computer.Network _
                                .DownloadFile(webListener.Address,
                                              destinationFilename,
                                              userName:="",
                                              password:="",
                                              showUI:=False,
                                              connectionTimeout:=1,
                                              overwrite:=True)

                        End Sub)

                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UrlWithWrongPassword_Throw(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFilename As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, "")
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Assert.Throws(Of WebException)(
                     Sub()
                         My.Computer.Network _
                             .DownloadFile(webListener.Address,
                                           destinationFilename,
                                           DefaultUserName,
                                           password)

                     End Sub)
                Assert.False(File.Exists(destinationFilename))
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

    End Class
End Namespace
