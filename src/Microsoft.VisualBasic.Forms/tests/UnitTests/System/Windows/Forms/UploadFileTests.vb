﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports FluentAssertions
Imports Microsoft.VisualBasic.FileIO
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class UploadFileTests
        Inherits VbFileCleanupTestBase

        <WinFormsFact>
        Public Sub UploadFile_UriOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address))
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriOnlyWhereAddressIsEmptyString_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(String.Empty))
                    End Sub

                testCode.Should.Throw(Of UriFormatException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriOnlyWhereAddressIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(Nothing))
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)()
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UriOnlyWhereSourceFileNameInvalidAddressOnly_Throws(sourceFileName As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address))
                    End Sub

                testCode.Should() _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriOnlyWrongFileSize_Throw()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address))
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancel_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWherePasswordWrong_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            networkCredentials:=New NetworkCredential(userName:=DefaultUserName, password),
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should() _
                    .Throw(Of WebException)() _
                    .WithMessage(SR.net_webstatus_Unauthorized)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereSourceFileNameInvalid_Throws(
            sourceFileName As String)

            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=Nothing,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should() _
                    .Throw(Of ArgumentNullException)() _
                    .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, sourceFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereTimeOut_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=LargeTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=1)
                    End Sub

                testCode.Should() _
                   .Throw(Of WebException)() _
                   .WithMessage(SR.net_webstatus_Timeout)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereTimeoutNegative_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=LargeTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=-1)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(Function(e) e.Message.StartsWith(SR.Network_BadConnectionTimeout))
            End Using
        End Sub

        <WinFormsFact(Skip:="showUI:=True tests fail")>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereTrue_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=LargeTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=CType(Nothing, Uri),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereUrlInvalid_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(InvalidUrlAddress),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should().Throw(Of UriFormatException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=Nothing,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptions_ExceptOnUserCancelWhereWhereDestinationFileExists_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsAndNetworkCredentials_Fail()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=Nothing,
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsAndNetworkCredentials_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsAndNetworkCredentialsTimeout0_Fail()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=0,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.Throw(Of ArgumentException)()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsDoNotShowUI_ExceptOnUserCancelWhereInvalidUrl_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(InvalidUrlAddress),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.Throw(Of UriFormatException)()
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UriWithAllOptionsExceptOnUserCancelWhereSourceFileNameInvalidThrows(
            sourceFileName As String)

            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=Nothing,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, sourceFileName, listener)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("\"c)>
        <InlineData("/"c)>
        Public Sub UploadFile_UriWithAllOptionsWhereCheckFilePathTrailingSeparators_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=$"{sourceFileName}{separator}",
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub UploadFile_UriWithAllOptionsWhereDestinationIsRootDirectory_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=root, ' This is a Root Directory!
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should.Throw(Of ArgumentException)().Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub UploadFile_UriWithAllOptionsWhereFilePathTrailingSeparatorsAreInvalid_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=$"{sourceFileName}{separator}",
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsWhereOnUserCancelIsDoNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:")>
        <InlineData("\\myshare\mydir")>
        Public Sub UploadFile_UriWithAllOptionsWhereRootDirectoryInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=root, ' This is a Root Directory!
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should().Throw(Of FileNotFoundException)()
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub UploadFile_UriWithAllOptionsWhereRootDirectoryTrailingSeparatorInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=root, ' This is a Root Directory!
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UriWithAllOptionsWhereSourceFileNameInvalid_Throws(sourceFileName As String)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=Nothing,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)()
                VerifyFailedDownload(testDirectory:=Nothing, sourceFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=CType(Nothing, Uri),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithAllOptionsWithAllOptions_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UriWithUserNamePassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=DefaultUserName,
                            password:=DefaultPassword)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub UploadFile_UriWithUserNamePasswordWherePasswordWrong_Throw(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=String.Empty)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR.net_webstatus_Unauthorized)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub UploadFile_UriWithUserNamePasswordWherePasswordWrong_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR.net_webstatus_Unauthorized)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlOnly_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UrlOnlyWhereAddressInvalid_Throws(address As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(sourceFileName, address)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UrlOnlyWhereSourceFileNameInvalidAddressOnly_Throws(sourceFileName As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(sourceFileName, address:=webListener.Address)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereInvalidUrlDoNotShowUI_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=InvalidUrlAddress,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                Dim value As String = SR.Network_InvalidUriString.Replace("{0}", "invalidURL")
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(Function(e) e.Message.StartsWith(value))
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereSourceFileNameInvalid_Throws(
            sourceFileName As String)

            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=Nothing,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, sourceFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereTimeOut_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=LargeTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=1)
                    End Sub

                testCode.Should() _
                .Throw(Of WebException)() _
                .WithMessage(SR.net_webstatus_Timeout)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereTimeoutNegative_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=LargeTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=-1)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(Function(e) e.Message.StartsWith(SR.Network_BadConnectionTimeout))
            End Using
        End Sub

        <WinFormsFact(Skip:="Manual Testing Only")>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereTrue_Fail()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=ExtraLargeTestFileSize)
            Dim webListener As New WebListener(ExtraLargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.Throw(Of OperationCanceledException)()
            End Using
        End Sub

        <WinFormsFact(Skip:="showUI:=True tests fail")>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereTrue_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=LargeTestFileSize)
            Dim webListener As New WebListener(LargeTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=True,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereUrlInvalid_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=InvalidUrlAddress,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                Dim value As String = SR.Network_InvalidUriString.Replace("{0}", "invalidURL")
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(value))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=Nothing,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptions_ExceptOnUserCancelWhereWhereDestinationFileExists_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=1)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptionsAndNetworkCredentials_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim networkCredentials As New NetworkCredential(DefaultUserName, DefaultPassword)
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            networkCredentials,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.ThrowException)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptionsDoNotShowUI_ExceptOnUserCancelWhereInvalidUrl_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=InvalidUrlAddress,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout)
                    End Sub

                Dim value As String = SR.Network_InvalidUriString.Replace("{0}", "invalidURL")
                testCode.Should() _
                    .Throw(Of ArgumentException)() _
                    .Where(Function(e) e.Message.StartsWith(value))
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UrlWithAllOptionsWhereAddressIsNothingOrEmpty_Throws(address As String)
            Dim testCode As Action =
                Sub()
                    My.Computer.Network.UploadFile(
                        sourceFileName:=Nothing,
                        address,
                        userName:=String.Empty,
                        password:=String.Empty,
                        showUI:=False,
                        connectionTimeout:=TestingConnectionTimeout,
                        onUserCancel:=UICancelOption.DoNothing)
                End Sub

            testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub UploadFile_UrlWithAllOptionsWhereDestinationIsRootDirectory_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=root, ' This is a Root Directory!
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("\")>
        <InlineData("/")>
        Public Sub UploadFile_UrlWithAllOptionsWhereFilePathTrailingSeparatorsAreInvalid_Throw(separator As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=$"{sourceFileName}{separator}",
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptionsWhereOnUserCancelIsDoNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData("C:\")>
        <InlineData("\\myshare\mydir\")>
        Public Sub UploadFile_UrlWithAllOptionsWhereRootDirectoryTrailingSeparatorInvalid_Throw(root As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=root, ' This is a Root Directory!
                            address:=New Uri(webListener.Address),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                Dim exceptionExpression As Expressions.Expression(Of Func(Of ArgumentException, Boolean)) =
                Function(e) e.Message.StartsWith(SR.IO_FilePathException) _
                    AndAlso e.Message.Contains(NameOf(sourceFileName))
                testCode.Should() _
                .Throw(Of ArgumentException)() _
                .Where(exceptionExpression)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        Public Sub UploadFile_UrlWithAllOptionsWhereSourceFileNameInvalid_Throws(sourceFileName As String)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName:=Nothing,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
                VerifyFailedDownload(testDirectory:=Nothing, sourceFileName, listener)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptionsWhereUriIsNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=CType(Nothing, Uri),
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should() _
                .Throw(Of ArgumentNullException)() _
                .Where(Function(e) e.Message.StartsWith(SR.General_ArgumentNullException))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithAllOptionsWithAllOptions_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=String.Empty,
                            password:=String.Empty,
                            showUI:=False,
                            connectionTimeout:=TestingConnectionTimeout,
                            onUserCancel:=UICancelOption.DoNothing)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub UploadFile_UrlWithUserNamePassword_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=DefaultUserName,
                            password:=DefaultPassword)
                    End Sub

                testCode.Should.NotThrow()
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub UploadFile_UrlWithUserNamePasswordWherePasswordWrong_Throw(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=String.Empty)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=webListener.Address,
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                    .Throw(Of WebException)() _
                    .WithMessage(SR.net_webstatus_Unauthorized)
            End Using
        End Sub

        <WinFormsTheory>
        <NullAndEmptyStringData>
        <InlineData("WrongPassword")>
        Public Sub UploadFile_UrlWithUserNamePasswordWherePasswordWrong_Throws(password As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(testDirectory, size:=SmallTestFileSize)
            Dim webListener As New WebListener(
                fileSize:=SmallTestFileSize,
                userName:=DefaultUserName,
                password:=DefaultPassword)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        My.Computer.Network.UploadFile(
                            sourceFileName,
                            address:=New Uri(webListener.Address),
                            userName:=DefaultUserName,
                            password)
                    End Sub

                testCode.Should() _
                    .Throw(Of WebException)() _
                    .WithMessage(SR.net_webstatus_Unauthorized)
            End Using
        End Sub

    End Class
End Namespace
