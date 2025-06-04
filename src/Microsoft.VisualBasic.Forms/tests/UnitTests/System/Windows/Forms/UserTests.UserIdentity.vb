' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Security.Principal

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class UserTests

        Public Class UserIdentity
            Implements IIdentity

            Public Sub New(authenticationType As String, name As String, isAuthenticated As Boolean)
                Me.AuthenticationType = authenticationType
                Me.Name = name
            End Sub

            Public ReadOnly Property AuthenticationType As String Implements IIdentity.AuthenticationType

            Public ReadOnly Property IsAuthenticated As Boolean Implements IIdentity.IsAuthenticated
                Get
                    Return True
                End Get
            End Property

            Public ReadOnly Property Name As String Implements IIdentity.Name

        End Class

    End Class
End Namespace
