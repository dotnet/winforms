' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports FluentAssertions
Imports Microsoft.VisualBasic.FileIO
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class NetworkTests
        Private Const DefaultPassword As String = "TBD"
        Private Const DefaultUserName As String = "TBD"
        Private Const DownloadLargeFileSize As Integer = 104857600
        Private Const DownloadSmallFileSize As Integer = 18135
        Private Const InvalidUrlAddress As String = "invalidURL"
        Private Const TestingConnectionTimeout As Integer = 100000

        ''' <summary>
        '''  Verify that testDirectory exists, that destinationFileName exist and what its length is.
        ''' </summary>
        ''' <param name="destinationFileName">The full path and filename of the new file.</param>
        ''' <returns>
        '''  The size in bytes of the destination file, this saves the caller from having to
        '''  do another FileInfo call.
        ''' </returns>
        Private Shared Function ValidateDownload(destinationFileName As String) As Long
            Dim fileInfo As New FileInfo(destinationFileName)
            fileInfo.Exists.Should.BeTrue()
            Directory.Exists(fileInfo.DirectoryName).Should.BeTrue()
            ' This directory should not be systems Temp Directory because it must be created
            Path.GetTempPath.Should.NotBe(fileInfo.DirectoryName)

            Return fileInfo.Length
        End Function

        Private Sub CleanUp(listener As HttpListener, Optional testDirectory As String = Nothing)
            Debug.Assert(Path.GetTempPath <> testDirectory)
            listener.Stop()
            listener.Close()
            If Not String.IsNullOrWhiteSpace(testDirectory) Then
                testDirectory.Should.StartWith(Path.GetTempPath)
                Directory.Delete(testDirectory, recursive:=True)
            End If
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriCheckDestinationIsDirectory_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=testDirectory, ' This is a Directory!
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub
                testCode.Should() _
                        .Throw(Of InvalidOperationException)() _
                        .Where(Function(e) e.Message.StartsWith(SR.Network_DownloadNeedsFilename))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UriCheckFilePathTrailingSeparators_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=$"{destinationFileName}{separator}",
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should() _
                        .Throw(Of ArgumentException)() _
                        .Where(Function(e) e.Message.StartsWith(SR.IO_FilePathException) AndAlso e.Message.Contains(NameOf(destinationFileName)))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UriCheckRootDirectoryTrailingSeparator_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=root, ' This is a Root Directory!
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should() _
                        .Throw(Of ArgumentException)() _
                        .Where(Function(e) e.Message.StartsWith(SR.IO_FilePathException) AndAlso e.Message.Contains(NameOf(destinationFileName)))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_UriCheckRootDirectoryValidation_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=root, ' This is a Root Directory!
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should() _
                        .Throw(Of InvalidOperationException)() _
                        .Where(Function(e) e.Message.StartsWith(SR.Network_DownloadNeedsFilename))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(uriString:=webListener.Address),
                            destinationFileName)
                    End Sub

                testCode.Should.NotThrow()
                Directory.Exists(testDirectory).Should.BeTrue()
                ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereTimeoutNegative_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=-1,
                            overwrite:=False)
                    End Sub

                testCode.Should() _
                        .Throw(Of ArgumentException)() _
                        .Where(Function(e) e.Message.StartsWith(SR.Network_BadConnectionTimeout))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=CType(Nothing, Uri),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)

                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereUriIsNothingAllParameters_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(address:=CType(Nothing, Uri),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWhereUriIsNothingSpecifyOnUserCancel_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=CType(Nothing, Uri),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsSpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address:=New Uri(webListener.Address),
                        destinationFileName,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsSpecifiedExceptUserCancel_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address:=New Uri(webListener.Address),
                        destinationFileName,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=True)
                End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithICredentialsShowUiTimeOutOverwriteSpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)

            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address:=New Uri(webListener.Address),
                        destinationFileName,
                        networkCredentials,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=True)
                End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithNonexistentTargetDirectory_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Directory.Delete(testDirectory, recursive:=True)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim testCode As Action =
              Sub()
                  My.Computer.Network.DownloadFile(
                          address:=New Uri(webListener.Address),
                          destinationFileName,
                          userName:=String.Empty,
                          password:=String.Empty,
                          showUI:=False,
                          connectionTimeout:=TestingConnectionTimeout,
                          overwrite:=True,
                          onUserCancel:=UICancelOption.DoNothing)
              End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithOverwriteShowUISpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address:=New Uri(webListener.Address),
                        destinationFileName,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=True,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=True)
                End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadLargeFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithUriOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Dim testCode As Action =
             Sub()
                 My.Computer.Network.DownloadFile(
                       address:=New Uri(webListener.Address),
                       destinationFileName)
             End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithUserNamePassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address:=New Uri(webListener.Address),
                        destinationFileName,
                        userName:=DefaultUserName,
                        password:=DefaultPassword)
                End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UriWithWrongPassword_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should _
                    .Throw(Of WebException)() _
                    .WithMessage("The remote server returned an error: (401) Unauthorized.")
                File.Exists(destinationFileName).Should.BeFalse()

            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlCheckDestinationIsDirectory_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                  Sub()
                      My.Computer.Network.DownloadFile(
                             address:=webListener.Address,
                            destinationFileName:=testDirectory, ' This is a Directory!
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)

                  End Sub

                testCode.Should _
                    .Throw(Of InvalidOperationException)() _
                    .WithMessage(SR.Network_DownloadNeedsFilename)
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UrlCheckFilePathTrailingSeparators_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try

                Dim testCode As Action =
                  Sub()
                      My.Computer.Network.DownloadFile(
                        address:=webListener.Address,
                        destinationFileName:=$"{destinationFileName}{separator}",
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)

                  End Sub

                testCode.Should _
                    .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.IO_FilePathException) AndAlso e.Message.Contains(NameOf(destinationFileName)))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UrlCheckRootDirectoryTrailingSeparator_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName:=root, ' This is a Root Directory!
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.IO_FilePathException) AndAlso e.Message.Contains(NameOf(destinationFileName)))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_UrlCheckRootDirectoryValidation_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName:=root, ' This is a Root Directory!
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should _
                    .Throw(Of InvalidOperationException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.Network_DownloadNeedsFilename))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName)

                    End Sub

                testCode.Should.NotThrow()
                Directory.Exists(testDirectory).Should.BeTrue()
                ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereAddressInvalid_Throws(address As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address,
                            destinationFileName)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereDestinationFileNameInvalidAddressOnly_Throws(destinationFileName As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName)

                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereDestinationFileNameInvalidOverwrite_Throws(destinationFileName As String)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName:=Nothing,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("")>
        Public Sub DownloadFile_UrlWhereDestinationFileNameInvalidWithOnUerCancel_Throws(destinationFileName As String)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName:=Nothing,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereFileExistsNoOverwrite_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()
            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address:=webListener.Address,
                        destinationFileName,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=False)
                End Sub


            testCode.Should _
                    .Throw(Of IOException)() _
                    .Where(Function(e) e.Message.Equals(SR.IO_FileExists_Path.Replace("{0}", destinationFileName)))
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(1)

            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereInvalidUrlDoNotShowUI_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=InvalidUrlAddress,
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should _
                    .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.Network_InvalidUriString.Replace("{0}", "invalidURL")))

                Directory.Exists(testDirectory).Should.BeTrue()
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereUrlInvalid_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                             address:=InvalidUrlAddress,
                             destinationFileName,
                             userName:=String.Empty,
                             password:=String.Empty,
                             showUI:=True,
                             connectionTimeout:=TestingConnectionTimeout,
                             overwrite:=False)


                    End Sub

                testCode.Should() _
                        .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.Network_InvalidUriString.Replace("{0}", "invalidURL")))
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                My.Computer.Network.DownloadFile(
                    address:=webListener.Address,
                    destinationFileName,
                    userName:=Nothing,
                    password:=String.Empty,
                    showUI:=True,
                    connectionTimeout:=TestingConnectionTimeout,
                    overwrite:=False)

                ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsSpecified_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Dim testCode As Action =
               Sub()
                   My.Computer.Network.DownloadFile(
                        address:=webListener.Address,
                        destinationFileName,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=True,
                        onUserCancel:=UICancelOption.DoNothing)
               End Sub

            testCode.Should.NotThrow()
            Directory.Exists(testDirectory).Should.BeTrue()
            ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)
            CleanUp(listener, testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithOverwriteWhereDestinationFileExists_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                  Sub()
                      My.Computer.Network.DownloadFile(
                          address:=webListener.Address,
                          destinationFileName,
                          userName:=String.Empty,
                          password:=String.Empty,
                          showUI:=False,
                          connectionTimeout:=TestingConnectionTimeout,
                          overwrite:=True)
                  End Sub

                testCode.Should.NotThrow()
                Directory.Exists(testDirectory).Should.BeTrue()
                ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithPassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, DefaultPassword)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=DefaultUserName,
                            password:=DefaultPassword)
                    End Sub

                testCode.Should.NotThrow()
                Directory.Exists(testDirectory).Should.BeTrue()
                ValidateDownload(destinationFileName).Should.Be(DownloadSmallFileSize)
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithTimeOut_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=1,
                            overwrite:=True)
                    End Sub

                testCode.Should _
                    .Throw(Of WebException)() _
                    .WithMessage("The operation has timed out.")
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UrlWithWrongPassword_Throw(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetDestinationFileName(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize, DefaultUserName, String.Empty)
            Dim listener As HttpListener = webListener.ProcessRequests()

            Try
                Dim testCode As Action =
                     Sub()
                         My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=DefaultUserName,
                            password)
                     End Sub

                testCode.Should _
                    .Throw(Of WebException)() _
                    .WithMessage("The remote server returned an error: (401) Unauthorized.")
                File.Exists(destinationFileName).Should.BeFalse()
            Finally
                CleanUp(listener, testDirectory)
            End Try
        End Sub

    End Class
End Namespace
