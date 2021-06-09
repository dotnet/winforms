' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On
Option Infer On

Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Provides context for the Startup event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class ApplyDefaultsEventArgs
        Inherits EventArgs

        ''' <summary>
        ''' Creates a new instance of the StartupEventArgs.
        ''' </summary>
        Public Sub New()
            Me.DefaultFont = DefaultFont
        End Sub

        ''' <summary>
        ''' Gets or Sets the HighDpiMode.
        ''' </summary>
        Public Property DefaultFont As Font
    End Class
End Namespace
