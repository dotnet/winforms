' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

' Here for future testing of My.Computer.Network.UploadFile
' the site SpeedTest.tele2.net should not be used
#If False Then

Imports System.IO
Imports System.Net

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests

        ' The following URL MUST be replaced with a local URL
        Private Const UploadFileUrl As String = "ftp://speedtest.tele2.net/upload/testing.txt"

        <WinFormsFact(Skip:="Until UploadAsync Implemented")>
        Public Sub UploadFilenameUrl()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, size:=1)
            My.Computer.Network.UploadFile(sourceFilename,
                address:=UploadFileUrl)
            Directory.Delete(tmpFilePath, True)
        End Sub

        <WinFormsFact(Skip:="Until UploadAsync Implemented")>
        Public Sub UploadWhereTimeoutNegative()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, size:=&H100_0000)
            Assert.Throws(Of ArgumentException)(
                Sub()
                    My.Computer.Network.UploadFile(sourceFilename,
                        address:=UploadFileUrl,
                        userName:="anonymous",
                        password:="anonymous",
                        showUI:=True,
                        connectionTimeout:=-1)
                End Sub)
            Directory.Delete(tmpFilePath, True)
        End Sub

        <WinFormsFact(Skip:="Issue #10706")>
        Public Sub UploadWithExpectedTimeOut()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, &H10_0000)
            Try

                Assert.Throws(Of WebException)(
                    Sub()
                        My.Computer.Network.UploadFile(sourceFilename,
                            address:=UploadFileUrl,
                            userName:="anonymous",
                            password:="anonymous",
                            showUI:=True,
                            connectionTimeout:=1)
                    End Sub)
            Finally
                Directory.Delete(tmpFilePath, True)
            End Try
        End Sub

        <WinFormsFact(Skip:="Until UploadAsync Implemented")>
        Public Sub UploadWithNothingPassword()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, 1)
            Try
                My.Computer.Network.UploadFile(sourceFilename,
                    address:=UploadFileUrl,
                    userName:="anonymous",
                    password:=Nothing)
            Finally
                Directory.Delete(tmpFilePath, True)
            End Try
        End Sub

        <WinFormsFact(Skip:="Until UploadAsync Implemented")>
        Public Sub UploadWithNothingSourceFile()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, 1)
            Try
                Assert.Throws(Of ArgumentNullException)(
                    Sub()
                        My.Computer.Network.UploadFile(sourceFileName:=Nothing,
                            address:=UploadFileUrl,
                            userName:="anonymous",
                            password:="anonymous")
                    End Sub)
            Finally
                Directory.Delete(tmpFilePath, True)
            End Try
        End Sub

        <WinFormsFact(Skip:="Until UploadAsync Implemented")>
        Public Sub UploadWithNothingUri()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, 1)
            Assert.Throws(Of ArgumentNullException)(
                Sub()
                    My.Computer.Network.UploadFile(sourceFilename,
                        address:=CType(Nothing, Uri),
                        userName:="anonymous",
                        password:="anonymous")
                End Sub)
            Directory.Delete(tmpFilePath, True)
        End Sub

        <WinFormsFact(Skip:="Until UploadAsync Implemented")>
        Public Sub UploadWithUI()
            Dim tmpFilePath As String = CreateTempDirectory()
            Dim sourceFilename As String = CreateTempFile(tmpFilePath, 1)
            Try
                My.Computer.Network.UploadFile(sourceFilename,
                    address:=UploadFileUrl,
                    userName:="anonymous",
                    password:="anonymous",
                    showUI:=True,
                    connectionTimeout:=100000)
            Finally
                Directory.Delete(tmpFilePath, True)
            End Try
        End Sub

    End Class

End Namespace
#End If
