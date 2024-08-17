' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Security.Principal
Imports System.Threading

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Class abstracting the computer user.
    ''' </summary>
    Public Class User

        ''' <summary>
        '''  Creates an instance of User.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        '''  The principal representing the current user.
        ''' </summary>
        ''' <value>An <see cref="IPrincipal"/> representing the current user.</value>
        ''' <remarks>
        '''  This should be overridden by derived classes that don't get the current.
        '''  user from the current thread
        ''' </remarks>
        Protected Overridable Property InternalPrincipal() As IPrincipal
            Get
                Return Thread.CurrentPrincipal
            End Get
            Set(value As IPrincipal)
                Thread.CurrentPrincipal = value
            End Set
        End Property

        ''' <summary>
        '''  The current <see cref="IPrincipal"/> which represents the current user.
        ''' </summary>
        ''' <value>An IPrincipal representing the current user.</value>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Property CurrentPrincipal() As IPrincipal
            Get
                Return InternalPrincipal
            End Get
            Set(value As IPrincipal)
                InternalPrincipal = value
            End Set
        End Property

        ''' <summary>
        '''  Indicates whether or not the current user has been authenticated.
        ''' </summary>
        Public ReadOnly Property IsAuthenticated() As Boolean
            Get
                Return InternalPrincipal.Identity.IsAuthenticated
            End Get
        End Property

        ''' <summary>
        '''  The name of the current user.
        ''' </summary>
        Public ReadOnly Property Name() As String
            Get
                Return InternalPrincipal.Identity.Name
            End Get
        End Property

        ''' <summary>
        '''  Indicates whether or not the current user is a member of the passed in role.
        ''' </summary>
        ''' <param name="role">The name of the role.</param>
        ''' <returns><see langword="True"/> if the user is a member of the role otherwise <see langword="False"/>.</returns>
        Public Function IsInRole(role As String) As Boolean
            Return InternalPrincipal.IsInRole(role)
        End Function

    End Class
End Namespace
