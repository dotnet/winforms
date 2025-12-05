' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ServerConfigurationTests

        Private Shared Sub Verify(uploading As Boolean, defaultConfiguration As ServerConfiguration, testConfiguration As ServerConfiguration)
            defaultConfiguration.GetAcceptsAnonymousLogin(uploading).Should.Be(testConfiguration.GetAcceptsAnonymousLogin(uploading))
            defaultConfiguration.GetDefaultPassword(uploading).Should.Be(testConfiguration.GetDefaultPassword(uploading))
            defaultConfiguration.GetDefaultUserName(uploading).Should.Be(testConfiguration.GetDefaultUserName(uploading))
            defaultConfiguration.GetFileUrlPrefix(uploading).Should.Be(testConfiguration.GetFileUrlPrefix(uploading))
            defaultConfiguration.GetThrowsPasswordErrors(uploading).Should.Be(testConfiguration.GetThrowsPasswordErrors(uploading))
        End Sub

        <Theory>
        <BoolData>
        Public Sub VerifyNew_Success(uploading As Boolean)
            Dim defaultConfiguration As New ServerConfiguration
            Dim testConfiguration As ServerConfiguration = ServerConfiguration.ServerConfigurationLoad
            Verify(uploading, defaultConfiguration, testConfiguration)
        End Sub

        <Theory>
        <BoolData>
        Public Sub VerifySerialization_Success(uploading As Boolean)
            Dim defaultConfiguration As New ServerConfiguration
            Dim jsonFilePathBase As String = Path.GetTempPath
            Dim jsonFullPath As String = defaultConfiguration.ServerConfigurationSave(jsonFilePathBase)
            Dim testConfiguration As ServerConfiguration = ServerConfiguration.ServerConfigurationLoad(jsonFilePathBase)
            Verify(uploading, defaultConfiguration, testConfiguration)
            File.Delete(jsonFullPath)
        End Sub

    End Class
End Namespace
