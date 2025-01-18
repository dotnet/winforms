' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports FluentAssertions.Execution
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ServerConfigurationTests
        <Theory>
        <BoolData>
        Public Sub VerifyNew_Success(uploading As Boolean)
            Dim serverConfigurationDefaults As New ServerConfiguration
            Dim serverConfigurationLoad As ServerConfiguration = ServerConfiguration.ServerConfigurationLoad
            serverConfigurationDefaults.GetAcceptsAnonymousLogin(uploading).Should.Be(serverConfigurationLoad.GetAcceptsAnonymousLogin(uploading))
            serverConfigurationDefaults.GetDefaultPassword(uploading).Should.Be(serverConfigurationLoad.GetDefaultPassword(uploading))
            serverConfigurationDefaults.GetDefaultUserName(uploading).Should.Be(serverConfigurationLoad.GetDefaultUserName(uploading))
            serverConfigurationDefaults.GetFileUrlPrefix(uploading).Should.Be(serverConfigurationLoad.GetFileUrlPrefix(uploading))
            serverConfigurationDefaults.GetThrowsPasswordErrors(uploading).Should.Be(serverConfigurationLoad.GetThrowsPasswordErrors(uploading))
        End Sub
        <Theory>
        <BoolData>
        Public Sub VerifyNew_Fail(uploading As Boolean)
            Dim serverConfigurationDefaults As New ServerConfiguration
            Dim serverConfigurationLoad As ServerConfiguration = ServerConfiguration.ServerConfigurationLoad("ServerConfigurationSample.JSON")
            Using New AssertionScope()
                serverConfigurationDefaults.GetAcceptsAnonymousLogin(uploading).Should.NotBe(serverConfigurationLoad.GetAcceptsAnonymousLogin(uploading))
                serverConfigurationDefaults.GetDefaultPassword(uploading).Should.NotBe(serverConfigurationLoad.GetDefaultPassword(uploading))
                serverConfigurationDefaults.GetDefaultUserName(uploading).Should.NotBe(serverConfigurationLoad.GetDefaultUserName(uploading))
                serverConfigurationDefaults.GetFileUrlPrefix(uploading).Should.NotBe(serverConfigurationLoad.GetFileUrlPrefix(uploading))
                serverConfigurationDefaults.GetThrowsPasswordErrors(uploading).Should.NotBe(serverConfigurationLoad.GetThrowsPasswordErrors(uploading))
            End Using
        End Sub

    End Class
End Namespace
