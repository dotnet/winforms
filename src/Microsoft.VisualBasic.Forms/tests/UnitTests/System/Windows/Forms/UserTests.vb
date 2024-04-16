' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Threading
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        <WinFormsFact>
        Public Sub UserGetNameTest()
            Dim testUser As New ProxyUser("Basic", "Test", True, "Guest")
            Assert.Equal("Test", testUser.Name)
        End Sub

        <WinFormsFact>
        Public Sub UserIsAuthenticatedTest()
            Dim testUser As New ProxyUser("Basic", "Test", True, "Guest")
            Assert.Equal(True, testUser.IsAuthenticated)
        End Sub

        <WinFormsFact>
        Public Sub UserIsInRoleTest()
            Dim testUser As New ProxyUser("Basic", "Test", True, "Guest")
            Assert.True(testUser.IsInRole("Guest"))
            Assert.False(testUser.IsInRole("Basic"))
        End Sub

    End Class
End Namespace
