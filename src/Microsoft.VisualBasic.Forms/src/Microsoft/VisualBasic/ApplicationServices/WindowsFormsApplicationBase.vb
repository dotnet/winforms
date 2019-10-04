' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Provides the exception encountered along with a flag on whether to abort the program
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class UnhandledExceptionEventArgs : Inherits ThreadExceptionEventArgs
        Sub New(ByVal exitApplication As Boolean, ByVal exception As System.Exception)
            MyBase.New(exception)
            Me.ExitApplication = exitApplication
        End Sub

        ''' <summary>
        ''' Indicates whether the application should exit upon exiting the exception handler
        ''' </summary>
        Public Property ExitApplication() As Boolean
    End Class

    ''' <summary>
    ''' Provides context for the Startup event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), System.Runtime.InteropServices.ComVisible(False)>
    Public Class StartupEventArgs : Inherits CancelEventArgs
        ''' <summary>
        ''' Creates a new instance of the StartupEventArgs.
        ''' </summary>
        Public Sub New(ByVal args As ReadOnlyCollection(Of String))
            If args Is Nothing Then
                args = New ReadOnlyCollection(Of String)(Nothing)
            End If

            CommandLine = args
        End Sub

        ''' <summary>
        ''' Returns the command line sent to this application
        ''' </summary>
        Public ReadOnly Property CommandLine() As ReadOnlyCollection(Of String)
    End Class

    ''' <summary>
    ''' Provides context for the StartupNextInstance event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Class StartupNextInstanceEventArgs : Inherits EventArgs

        ''' <summary>
        ''' Creates a new instance of the StartupNextInstanceEventArgs.
        ''' </summary>
        Public Sub New(ByVal args As ReadOnlyCollection(Of String), ByVal bringToForegroundFlag As Boolean)
            If args Is Nothing Then
                args = New ReadOnlyCollection(Of String)(Nothing)
            End If

            CommandLine = args
            BringToForeground = bringToForegroundFlag
        End Sub

        ''' <summary>
        ''' Indicates whether we will bring the application to the foreground when processing the
        ''' StartupNextInstance event.
        ''' </summary>
        Public Property BringToForeground() As Boolean

        ''' <summary>
        ''' Returns the command line sent to this application
        ''' </summary>
        ''' <remarks>I'm using Me.CommandLine so that it is consistent with my.net and to assure they 
        ''' always return the same values</remarks>
        Public ReadOnly Property CommandLine() As ReadOnlyCollection(Of String)
    End Class

End Namespace

