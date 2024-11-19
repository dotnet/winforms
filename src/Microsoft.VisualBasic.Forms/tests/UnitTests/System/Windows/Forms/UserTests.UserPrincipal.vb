' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Security.Principal

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        Private NotInheritable Class UserPrincipal
            Implements IPrincipal

            Private ReadOnly _role As String

            Public Sub New(authenticationType As String, name As String, isAuthenticated As Boolean, role As String)
                Identity = New UserIdentity(authenticationType, name, isAuthenticated)
                _role = role
            End Sub

            Public ReadOnly Property Identity As IIdentity Implements IPrincipal.Identity

            Public Function IsInRole(role As String) As Boolean Implements IPrincipal.IsInRole
                Return role = _role
            End Function

        End Class

    End Class
End Namespace
