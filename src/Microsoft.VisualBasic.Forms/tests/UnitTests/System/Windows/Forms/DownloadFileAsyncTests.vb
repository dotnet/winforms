' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports FluentAssertions
Imports Microsoft.VisualBasic.FileIO
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class DownloadFileAsyncTests
        Inherits VbFileCleanupTestBase

        <WinFormsFact>
        Public Sub DownloadFileAsync_AllOptions_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        Dim t As Task = Devices.Network.DownloadFileAsync(
                            addressUri:=New Uri(uriString:=webListener.Address),
                            destinationFileName,
                            clientHandler:=New HttpClientHandler,
                            dialog:=Nothing,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing,
                            cancelToken:=CancellationToken.None)
                        If t.IsFaulted Then
                            Throw t.Exception
                        End If
                        t.Wait()
                    End Sub

                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                    .Be(SmallTestFileSize)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFileAsync_AllOptionsClientHandlerNothing_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        Dim t As Task = Devices.Network.DownloadFileAsync(
                            addressUri:=Nothing,
                            destinationFileName,
                            clientHandler:=Nothing,
                            dialog:=Nothing,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing,
                            cancelToken:=CancellationToken.None)
                        If t.IsFaulted Then
                            Throw t.Exception
                        End If
                    End Sub

                testCode.Should.Throw(Of ArgumentNullException)().WithParameterName("addressUri")
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFileAsync_AllOptionsInvalidTimeout_Throws()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        Dim t As Task = Devices.Network.DownloadFileAsync(
                            addressUri:=New Uri(uriString:=webListener.Address),
                            destinationFileName,
                            clientHandler:=Nothing,
                            dialog:=Nothing,
                            connectionTimeout:=-1,
                            overwrite:=True,
                            onUserCancel:=UICancelOption.DoNothing,
                            cancelToken:=CancellationToken.None)
                        If t.IsFaulted Then
                            Throw t.Exception
                        End If
                    End Sub

                testCode.Should.Throw(Of ArgumentException)().WithParameterName("connectionTimeout")
            End Using
        End Sub

        <WinFormsFact>
        Public Sub DownloadFileAsync_UriWithAllOptions_ExceptOnUserCancelWhereUsernameIsNothing_Success()
            Dim testDirectory As String = CreateTempDirectory()
            Dim destinationFileName As String = GetUniqueFileNameWithPath(testDirectory)
            Dim webListener As New WebListener(SmallTestFileSize)
            Using listener As HttpListener = webListener.ProcessRequests()
                Dim testCode As Action =
                    Sub()
                        Devices.Network.DownloadFileAsync(
                            addressUri:=New Uri(uriString:=webListener.Address),
                            destinationFileName,
                            userName:=Nothing,
                            password:=String.Empty,
                            dialog:=Nothing,
                            connectionTimeout:=TestingConnectionTimeout,
                            overwrite:=False,
                            cancelToken:=CancellationToken.None).Wait()
                    End Sub
                testCode.Should.NotThrow()
                VerifySuccessfulDownload(testDirectory, destinationFileName, listener).Should() _
                    .Be(SmallTestFileSize)
            End Using
        End Sub

    End Class
End Namespace
