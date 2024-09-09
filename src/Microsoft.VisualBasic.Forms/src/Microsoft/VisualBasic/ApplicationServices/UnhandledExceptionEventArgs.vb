' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Provides the exception encountered along with a flag on whether to abort the program
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class UnhandledExceptionEventArgs
        Inherits ThreadExceptionEventArgs

        Public Sub New(exitApplication As Boolean, exception As Exception)
            MyBase.New(exception)
            Me.ExitApplication = exitApplication
        End Sub

        ''' <summary>
        '''  Indicates whether the application should exit upon exiting the exception handler
        ''' </summary>
        Public Property ExitApplication() As Boolean

    End Class
End Namespace
