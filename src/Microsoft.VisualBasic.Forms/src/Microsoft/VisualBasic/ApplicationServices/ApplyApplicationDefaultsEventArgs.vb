' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict On
Option Explicit On
Option Infer On

Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Provides context for the ApplyApplicationDefaults event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class ApplyApplicationDefaultsEventArgs
        Inherits EventArgs

        Friend Sub New(minimumSplashScreenDisplayTime As Integer,
                highDpiMode As HighDpiMode)
            Me.MinimumSplashScreenDisplayTime = minimumSplashScreenDisplayTime
            Me.HighDpiMode = highDpiMode
        End Sub

        ''' <summary>
        ''' Setting this property inside the event handler causes a new default Font for Forms and UserControls to be set.
        ''' </summary>
        ''' <remarks>
        ''' When the ApplyApplicationDefault event is raised, this property contains nothing. A new default Font for the
        ''' application is applied by setting this property with a value different than nothing.
        ''' </remarks>
        Public Property Font As Font

        ''' <summary>
        ''' Setting this Property inside the event handler determines how long an application's Splash dialog is displayed at a minimum.
        ''' </summary>
        Public Property MinimumSplashScreenDisplayTime As Integer =
            WindowsFormsApplicationBase.MINIMUM_SPLASH_EXPOSURE_DEFAULT

        ''' <summary>
        ''' Setting this Property inside the event handler determines the general HighDpiMode for the application.
        ''' </summary>
        ''' <remarks>
        ''' The default value for this property is SystemAware.
        ''' </remarks>
        Public Property HighDpiMode As HighDpiMode

    End Class
End Namespace
