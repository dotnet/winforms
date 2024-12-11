' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports FluentAssertions
Imports Microsoft.VisualBasic.FileIO
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class DownloadFileTests
        Inherits VbFileCleanupTestBase

        ' REVIEWER NOTE: The next 2 Constants need to be SR Resources,
        '                they are not accessible in this project they come from WebClient.
        Private Const SR_net_webstatus_Timeout As String = "The operation has timed out."
        Private Const SR_net_webstatus_Unauthorized As String =
            "The remote server returned an error: (401) Unauthorized."

        <WinFormsFact>
        Public Sub DownloadFile_UriOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                    .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriOnlyWhereAddressIsEmptyString_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                        Sub()
                            My.Computer.Network.DownloadFile(
                                address:=New Uri(String.Empty),
                                destinationFileName)
                        End Sub

                testCode.Should.Throw(Of UriFormatException)()
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriOnlyWhereAddressIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(Nothing),
                            destinationFileName)
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)()
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UriOnlyWhereDestinationFileNameInvalidAddressOnly_Throws(destinationFileName As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName)
                    End Sub

                testCode.Should() _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancel_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereDestinationFileNameInvalidOverwrite_Throws(
            destinationFileName As String)

            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=Nothing,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereFileExistsNoOverwrite_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                Dim value As String = SR.IO_FileExists_Path.Replace("{0}", destinationFileName)
                testCode.Should() _
                    .Throw(Of IOException)() _
                    .Where(Function(e) e.Message.Equals(value))
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should.Be(1)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereOverwriteTrue_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadLargeFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereOverwriteWhereDestinationFileExists_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWherePasswordWrong_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            networkCredentials:=New NetworkCredential(userName:=DefaultUserName, password),
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR_net_webstatus_Unauthorized)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereTimeOut_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=1,
                            overwrite:=True)
                    End Sub

                testCode.Should() _
               .Throw(Of WebException)() _
               .WithMessage(SR_net_webstatus_Timeout)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereTimeoutNegative_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereUrlInvalid_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(InvalidUrlAddress),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should().Throw(Of UriFormatException)()
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptions_ExceptOnUserCancelWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=Nothing,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsAndNetworkCredentials_Fail()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=Nothing,
                            destinationFileName,
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)()
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsAndNetworkCredentials_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsAndNetworkCredentialsTimeout0_Fail()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=0,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.Throw(Of ArgumentException)()
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsDoNotShowUI_ExceptOnUserCancelWhereInvalidUrl_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(InvalidUrlAddress),
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should.Throw(Of UriFormatException)()
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UriWithAllOptionsExceptOnUserCancelWhereDestinationFileNameInvalidOverwriteThrows(
            destinationFileName As String)

            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=Nothing,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UriWithAllOptionsWhereCheckFilePathTrailingSeparators_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UriWithAllOptionsWhereDestinationFileNameInvalid_Throws(destinationFileName As String)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName:=Nothing,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)()
                VerifyFailedDownload(testDirectory:=Nothing, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UriWithAllOptionsWhereDestinationIsRootDirectory_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should.Throw(Of ArgumentException)().Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UriWithAllOptionsWhereFilePathTrailingSeparatorsAreInvalid_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsWhereOnUserCancelIsDoNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_UriWithAllOptionsWhereRootDirectoryInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UriWithAllOptionsWhereRootDirectoryTrailingSeparatorInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsWhereTargetDirectoryNonexistent_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Directory.Delete(testDirectory, recursive:=True)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsWithAllOptions_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithAllOptionsWithAllOptionsWithAllOptionsWhereDestinationIsDirectory_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                .WithMessage(SR.Network_DownloadNeedsFilename)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UriWithUserNamePassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=DefaultUserName,
                            password:=DefaultPassword)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UriWithUserNamePasswordWherePasswordWrong_Throw(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=String.Empty)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR_net_webstatus_Unauthorized)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UriWithUserNamePasswordWherePasswordWrong_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR_net_webstatus_Unauthorized)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UrlOnlyWhereAddressInvalid_Throws(address As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(address, destinationFileName)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UrlOnlyWhereDestinationFileNameInvalidAddressOnly_Throws(destinationFileName As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(address:=webListener.Address, destinationFileName)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereDestinationFileNameInvalidOverwrite_Throws(
            destinationFileName As String)

            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereFileExistsNoOverwrite_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim value As String = SR.IO_FileExists_Path.Replace("{0}", destinationFileName)
                testCode.Should() _
                .Throw(Of IOException)() _
                .Where(Function(e) e.Message.Equals(value))
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should.Be(1)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereInvalidUrlDoNotShowUI_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim value As String = SR.Network_InvalidUriString.Replace("{0}", "invalidURL")
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(Function(e) e.Message.StartsWith(value))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereOverwriteTrue_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadLargeFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereOverwriteWhereDestinationFileExists_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereTimeOut_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR_net_webstatus_Timeout)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereTimeoutNegative_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadLargeFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereUrlInvalid_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim value As String = SR.Network_InvalidUriString.Replace("{0}", "invalidURL")
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(value))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptions_ExceptOnUserCancelWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=Nothing,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsAndNetworkCredentials_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsDoNotShowUI_ExceptOnUserCancelWhereInvalidUrl_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim value As String = SR.Network_InvalidUriString.Replace("{0}", "invalidURL")
                testCode.Should() _
                    .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(value))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UrlWithAllOptionsWhereAddressIsNothingOrEmpty_Throws(address As String)
            Dim testCode As Action =
                Sub()
                    My.Computer.Network.DownloadFile(
                        address,
                        destinationFileName:=Nothing,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=True,
                        connectionTimeout:=TestingConnectionTimeout,
                        overwrite:=False,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub

            testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub DownloadFile_UrlWithAllOptionsWhereDestinationFileNameInvalid_Throws(destinationFileName As String)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UrlWithAllOptionsWhereDestinationIsRootDirectory_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub DownloadFile_UrlWithAllOptionsWhereFilePathTrailingSeparatorsAreInvalid_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsWhereOnUserCancelIsDoNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub DownloadFile_UrlWithAllOptionsWhereRootDirectoryInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of InvalidOperationException)() _
                .Where(Function(e) e.Message.StartsWith(SR.Network_DownloadNeedsFilename))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub DownloadFile_UrlWithAllOptionsWhereRootDirectoryTrailingSeparatorInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(destinationFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsWhereTargetDirectoryNonexistent_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Directory.Delete(testDirectory, recursive:=True)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsWithAllOptions_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithAllOptionsWithAllOptionsWithAllOptionsWhereDestinationIsDirectory_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(DownloadSmallFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
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

                testCode.Should() _
                .Throw(Of InvalidOperationException)() _
                .WithMessage(SR.Network_DownloadNeedsFilename)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFile_UrlWithUserNamePassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=DefaultUserName,
                            password:=DefaultPassword)
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                .Be(DownloadSmallFileSize)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UrlWithUserNamePasswordWherePasswordWrong_Throw(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=String.Empty)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=webListener.Address,
                            destinationFileName,
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR_net_webstatus_Unauthorized)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub DownloadFile_UrlWithUserNamePasswordWherePasswordWrong_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(
                fileSize:=DownloadSmallFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.DownloadFile(
                            address:=New Uri(webListener.Address),
                            destinationFileName,
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                    .Throw(Of WebException)() _
                    .WithMessage(SR_net_webstatus_Unauthorized)
                VerifyFailedDownload(testDirectory, destinationFileName, listener)
            End Using
        End Sub

    End Class
End Namespace
