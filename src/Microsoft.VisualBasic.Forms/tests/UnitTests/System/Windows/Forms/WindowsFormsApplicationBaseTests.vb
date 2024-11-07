﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict Off

Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class WindowsFormsApplicationBaseTests
        Inherits WindowsFormsApplicationBase

        <WinFormsFact>
        Public Sub Properties()
            UseCompatibleTextRendering.Should.BeFalse()

#Disable Warning WFO5001 ' Type is for evaluation purposes only and is subject to change or removal in future updates.
            ColorMode.Should.Be(SystemColorMode.Classic)
            ColorMode = SystemColorMode.Dark
            ColorMode.Should.Be(SystemColorMode.Dark)
#Enable Warning WFO5001 ' Type is for evaluation purposes only and is subject to change or removal in future updates.

            EnableVisualStyles.Should.Be(False)
            EnableVisualStyles = True
            EnableVisualStyles.Should.Be(True)

            HighDpiMode.Should.Be(HighDpiMode.SystemAware)
            HighDpiMode = HighDpiMode.PerMonitorV2
            HighDpiMode.Should.Be(HighDpiMode.PerMonitorV2)

            IsSingleInstance.Should.Be(False)
            IsSingleInstance = True
            IsSingleInstance.Should.Be(True)

            ShutdownStyle.Should.Be(ShutdownMode.AfterMainFormCloses)
            ShutdownStyle = ShutdownMode.AfterAllFormsClose
            ShutdownStyle.Should.Be(ShutdownMode.AfterAllFormsClose)

            MinimumSplashScreenDisplayTime.Should.Be(MinimumSplashExposureDefault)
            MinimumSplashScreenDisplayTime = 1000
            MinimumSplashScreenDisplayTime.Should.Be(1000)

            SaveMySettingsOnExit.Should.Be(False)
            SaveMySettingsOnExit = True
            SaveMySettingsOnExit.Should.Be(True)

            SplashScreen.Should.BeNull()

            Dim testCode As Action =
                Sub()
                    MainForm = Nothing
                End Sub

            testCode.Should.Throw(Of ArgumentNullException)()

            Using form1 As New Form
                SplashScreen = form1
                SplashScreen.Should.Be(form1)

                testCode =
                    Sub()
                        MainForm = form1
                    End Sub
                testCode.Should.Throw(Of ArgumentException)()
            End Using

            Using form1 As New Form
                Using form2 As New Form
                    SplashScreen = form2
                    MainForm = form1
                    MainForm.Should.Be(form1)
                    SplashScreen.Should.Be(form2)
                    testCode =
                        Sub()
                            SplashScreen = form1
                        End Sub
                    testCode.Should.Throw(Of ArgumentException)()
                    OpenForms.Count.Should.Be(0)
                    SplashScreen.Show()
                    OpenForms.Count.Should.Be(1)
                End Using
            End Using

        End Sub

    End Class
End Namespace
