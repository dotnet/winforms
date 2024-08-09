' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        <WinFormsFact>
        Public Sub UserBaseTest()
            Dim testUser As New User
            testUser.CurrentPrincipal.Should.BeNull()
            Dim userPrincipal As New UserPrincipal(
                authenticationType:="Basic",
                name:="Test",
                isAuthenticated:=True,
                role:="Guest")
            testUser.CurrentPrincipal = userPrincipal
            testUser.CurrentPrincipal.Should.NotBeNull()
            testUser.CurrentPrincipal.Should.Be(userPrincipal)
            userPrincipal.Identity.AuthenticationType.Should.Be("Basic")
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
            testUser.Name.Should.Be("Test")
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
            testUser.IsAuthenticated.Should.BeTrue()
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
            testUser.IsInRole("Guest").Should.BeTrue()
            testUser.IsInRole("Basic").Should.BeFalse()
        End Sub

    End Class
End Namespace
