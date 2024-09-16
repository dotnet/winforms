' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Security.Principal

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        Public Class UserIdentity
            Implements IIdentity

            Private ReadOnly _authenticationType As String
            Private ReadOnly _name As String

            Public Sub New(authenticationType As String, name As String, isAuthenticated As Boolean)
                _authenticationType = authenticationType
                _name = name
            End Sub

            Public ReadOnly Property AuthenticationType As String Implements IIdentity.AuthenticationType
                Get
                    Return _authenticationType
                End Get
            End Property

            Public ReadOnly Property IsAuthenticated As Boolean Implements IIdentity.IsAuthenticated
                Get
                    Return True
                End Get
            End Property

            Public ReadOnly Property Name As String Implements IIdentity.Name
                Get
                    Return _name
                End Get
            End Property

        End Class

    End Class
End Namespace
