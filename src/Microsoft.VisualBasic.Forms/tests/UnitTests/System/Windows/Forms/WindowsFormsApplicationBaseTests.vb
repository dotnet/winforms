' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
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
#Enable Warning WFO5001

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

            Dim testCode As Action = Sub() MainForm = Nothing
            testCode.Should.Throw(Of ArgumentNullException)()

            Using form1 As New Form
                SplashScreen = form1
                SplashScreen.Should.Be(form1)

                testCode = Sub() MainForm = form1
                testCode.Should.Throw(Of ArgumentException)()
                SplashScreen = Nothing
            End Using

            Using form1 As New Form
                MainForm = form1
                MainForm.Should.Be(form1)

                testCode = Sub() SplashScreen = form1
                testCode.Should.Throw(Of ArgumentException)()
            End Using

            Using splashScreenForm As New Form
                Using form2 As New Form
                    SplashScreen = form2
                    splashScreenForm.Name = NameOf(splashScreenForm)
                    MainForm = splashScreenForm
                    MainForm.Should.Be(splashScreenForm)
                    SplashScreen.Should.Be(form2)
                    ApplicationContext.Should.NotBeNull()
                    ApplicationContext.MainForm.Name.Should.Be(NameOf(splashScreenForm))

                    testCode = Sub() SplashScreen = splashScreenForm
                    testCode.Should.Throw(Of ArgumentException)()

                    OpenForms.Count.Should.Be(0)
                    SplashScreen.Show()
                    OpenForms.Count.Should.Be(1)
                    SplashScreen = Nothing
                End Using
            End Using
        End Sub

        <Fact>
        Public Sub Run_DoEvents()
            Dim testCode As Action = Sub() DoEvents()
            testCode.Should.NotThrow()
        End Sub

        <Fact>
        Public Sub Run_SingleInstanceNoStartupFormException()
            IsSingleInstance = True
            Dim testCode As Action = Sub() Run(commandLine:={"1"})
            testCode.Should.Throw(Of NoStartupFormException)()
        End Sub

        <Fact>
        Public Sub ShowHideSplashScreenSuccess()
            Dim testCode As Action
            Using form1 As New Form
                SplashScreen = form1
                testCode = Sub() ShowSplashScreen()
                testCode.Should.NotThrow()
            End Using
            testCode = Sub() HideSplashScreen()
            testCode.Should.NotThrow()
            SplashScreen = Nothing
        End Sub

        <WinFormsFact>
        Public Sub ValidateAuthenticationModeEnumInvalidValue()
            Const value As AuthenticationMode = CType(-1, AuthenticationMode)
            Const paramName As String = NameOf(AuthenticationMode)

            Dim testCode As Action = Sub() ValidateAuthenticationModeEnumValue(value, paramName)
            testCode.Should.Throw(Of InvalidEnumArgumentException)()
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(EnumTestData(Of AuthenticationMode)))>
        Public Sub ValidateAuthenticationModeEnumValues(value As AuthenticationMode)
            Dim testCode As Action =
                Sub() ValidateAuthenticationModeEnumValue(value, NameOf(AuthenticationMode))

            testCode.Should.NotThrow()
        End Sub

        <WinFormsFact>
        Public Sub ValidateShutdownModeEnumInvalidValue()
            Const value As ShutdownMode = CType(-1, ShutdownMode)
            Const paramName As String = NameOf(ShutdownMode)
            Dim testCode As Action = Sub() ValidateShutdownModeEnumValue(value, paramName)
            testCode.Should.Throw(Of InvalidEnumArgumentException)()
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(EnumTestData(Of ShutdownMode)))>
        Public Sub ValidateShutdownModeEnumValues(value As ShutdownMode)
            Dim testCode As Action =
                Sub()
                    ValidateShutdownModeEnumValue(value, NameOf(ShutdownMode))
                End Sub

            testCode.Should.NotThrow()
        End Sub

    End Class
End Namespace
