' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Windows.Forms.Analyzers.Diagnostics

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Provides context for the ApplyApplicationDefaults event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class ApplyApplicationDefaultsEventArgs
        Inherits EventArgs

#Disable Warning WFO5001

        Friend Sub New(minimumSplashScreenDisplayTime As Integer,
                highDpiMode As HighDpiMode,
                colorMode As SystemColorMode)

            Me.MinimumSplashScreenDisplayTime = minimumSplashScreenDisplayTime
            Me.HighDpiMode = highDpiMode
            Me.ColorMode = colorMode
        End Sub

#Enable Warning WFO5001

        ''' <summary>
        '''  Setting this property inside the event handler determines the
        '''  <see cref="Application.ColorMode"/> for the application.
        ''' </summary>
        <Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat:=WindowsFormsApplicationBase.WinFormsExperimentalUrl)>
        Public Property ColorMode As SystemColorMode

        ''' <summary>
        '''  Setting this property inside the event handler causes a
        '''  new default <see cref="Font"/> for Forms and UserControls to be set.
        ''' </summary>
        ''' <remarks>
        '''  When the <see cref="WindowsFormsApplicationBase.ApplyApplicationDefaults"/> event is raised,
        '''  this property contains nothing. A new default <see cref="Font"/> for the application
        '''  is applied by setting this property with a value different than nothing.
        ''' </remarks>
        Public Property Font As Font

        ''' <summary>
        '''  Setting this <see langword="Property"/> inside the <see langword="Event"/> handler determines the general
        '''  <see cref="HighDpiMode"/> for the application.
        ''' </summary>
        ''' <remarks>
        '''  The default value for this property is SystemAware.
        ''' </remarks>
        Public Property HighDpiMode As HighDpiMode

        ''' <summary>
        '''  Setting this <see langword="Property"/> inside the <see langword="Event"/> handler determines
        '''  how long an application's Splash dialog is displayed at a minimum.
        ''' </summary>
        Public Property MinimumSplashScreenDisplayTime As Integer =
            WindowsFormsApplicationBase.MinimumSplashExposureDefault

    End Class
End Namespace
