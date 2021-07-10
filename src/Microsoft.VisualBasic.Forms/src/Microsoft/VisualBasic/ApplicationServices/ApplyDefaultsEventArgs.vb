' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On
Option Infer On

Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Provides context for the Startup event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class ApplyDefaultsEventArgs
        Inherits EventArgs

        Friend Sub New(minimumSplashScreenDisplayTime As Integer,
                highDpiMode As HighDpiMode)
            Me.MinimumSplashScreenDisplayTime = minimumSplashScreenDisplayTime
            Me.HighDpiMode = highDpiMode
        End Sub
        ''' <summary>
        ''' Gets or Sets the HighDpiMode.
        ''' </summary>
        Public Property Font As Font
        Public Property MinimumSplashScreenDisplayTime As Integer =
            WindowsFormsApplicationBase.MINIMUM_SPLASH_EXPOSURE_DEFAULT
        Public Property HighDpiMode As HighDpiMode

    End Class
End Namespace
