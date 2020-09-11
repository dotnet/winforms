' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On
Option Infer On

Imports System.ComponentModel
Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Provides context for the Startup event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class ApplyHighDpiModeEventArgs
        Inherits EventArgs

        Private _highDpiMode As System.Windows.Forms.HighDpiMode

        ''' <summary>
        ''' Creates a new instance of the StartupEventArgs.
        ''' </summary>
        Public Sub New(highDpiMode As System.Windows.Forms.HighDpiMode)
            Me.HighDpiMode = highDpiMode
        End Sub

        ''' <summary>
        ''' Gets or Sets the HighDpiMode.
        ''' </summary>
        Public Property HighDpiMode As System.Windows.Forms.HighDpiMode
            Get
                Return _highDpiMode
            End Get
            Set(value As System.Windows.Forms.HighDpiMode)
                _highDpiMode = value
            End Set
        End Property
    End Class
End Namespace
