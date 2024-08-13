' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Provides context for the Startup event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class StartupEventArgs
        Inherits CancelEventArgs

        ''' <summary>
        '''  Creates a new instance of the StartupEventArgs.
        ''' </summary>
        Public Sub New(args As ReadOnlyCollection(Of String))
            If args Is Nothing Then
                args = New ReadOnlyCollection(Of String)(Nothing)
            End If

            CommandLine = args
        End Sub

        ''' <summary>
        '''  Returns the command line sent to this application.
        ''' </summary>
        Public ReadOnly Property CommandLine() As ReadOnlyCollection(Of String)

    End Class
End Namespace
