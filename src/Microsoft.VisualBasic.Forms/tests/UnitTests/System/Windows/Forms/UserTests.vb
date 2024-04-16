' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.ApplicationServices

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        <WinFormsFact>
        Public Sub UserBaseTest()
            Dim testUser As New User
            Assert.Null(testUser.CurrentPrincipal)
            Dim userPrincipal As UserPrincipal = New UserPrincipal(
                            authenticationType:="Basic",
                            name:="Test",
                            isAuthenticated:=True,
                            role:="Guest")
            testUser.CurrentPrincipal = userPrincipal
            Assert.NotNull(testUser.CurrentPrincipal)
            Assert.Equal(userPrincipal, testUser.CurrentPrincipal)
        End Sub

        <WinFormsFact>
        Public Sub UserGetNameTest()
            Dim testUser As New User With {
                            .CurrentPrincipal = New UserPrincipal(
                            authenticationType:="Basic",
                            name:="Test",
                            isAuthenticated:=True,
                            role:="Guest")
            }
            Assert.Equal("Test", testUser.Name)
        End Sub

        <WinFormsFact>
        Public Sub UserIsAuthenticatedTest()
            Dim testUser As New User With {
                            .CurrentPrincipal = New UserPrincipal(
                            authenticationType:="Basic",
                            name:="Test",
                            isAuthenticated:=True,
                            role:="Guest")
            }
            Assert.Equal(True, testUser.IsAuthenticated)
        End Sub

        <WinFormsFact>
        Public Sub UserIsInRoleTest()
            Dim testUser As New User With {
                            .CurrentPrincipal = New UserPrincipal(
                            authenticationType:="Basic",
                            name:="Test",
                            isAuthenticated:=True,
                            role:="Guest")
            }
            Assert.True(testUser.IsInRole("Guest"))
            Assert.False(testUser.IsInRole("Basic"))
        End Sub

    End Class
End Namespace
