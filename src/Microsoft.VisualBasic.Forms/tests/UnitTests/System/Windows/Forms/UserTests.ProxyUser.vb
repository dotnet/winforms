' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Security.Principal
Imports Microsoft.VisualBasic.ApplicationServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        Public Class ProxyUser
            Inherits User
            Private _newIPrincipal As IPrincipal

            Public Sub New(authenticationType As String, name As String, isAuthenticated As Boolean, role As String)
                _newIPrincipal = New UserPrincipal(authenticationType, name, isAuthenticated, role)
            End Sub

            Protected Overrides Property InternalPrincipal() As IPrincipal
                Get
                    Return _newIPrincipal
                End Get
                Set(value As IPrincipal)
                    _newIPrincipal = value
                End Set
            End Property

        End Class

    End Class
End Namespace
