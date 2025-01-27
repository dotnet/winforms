' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.DotNet.RemoteExecutor
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
        Public Sub DoEvents_DoesNotThrow()
            Dim testCode As Action = Sub() DoEvents()
            testCode.Should.NotThrow()
        End Sub

        <Fact>
        Public Sub Run_SingleInstance_ThrowsNoStartupFormException()
            ' WindowsFormsApplicationBase.Run method can change process-wide settings, such as HighDpiMode,
            ' it should run in a dedicated process.
            If (RemoteExecutor.IsSupported) Then
                Dim test As Action =
                    Sub()
                        IsSingleInstance = True
                        Dim testCode As Action = Sub() Run(commandLine:={"1"})
                        testCode.Should.Throw(Of NoStartupFormException)()
                    End Sub

                Using handle As RemoteInvokeHandle = RemoteExecutor.Invoke(test)
                    handle.ExitCode.Should.Be(RemoteExecutor.SuccessExitCode)
                End Using
            End If
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

        <WinFormsTheory>
        <ClassData(GetType(EnumTestValidData(Of AuthenticationMode)))>
        <ClassData(GetType(EnumTestInvalidData(Of AuthenticationMode)))>
        Public Sub ValidateAuthenticationModeEnumValues(testData As EnumValueAndThrowIndicatorData(Of AuthenticationMode))
            Dim testCode As Action =
                Sub() ValidateAuthenticationModeEnumValue(testData.Value, NameOf(AuthenticationMode))

            If testData.Throws Then
                testCode.Should.Throw(Of InvalidEnumArgumentException)()
            Else
                testCode.Should.NotThrow()
            End If
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(EnumTestValidData(Of ShutdownMode)))>
        <ClassData(GetType(EnumTestInvalidData(Of ShutdownMode)))>
        Public Sub ValidateShutdownModeEnumValues(testData As EnumValueAndThrowIndicatorData(Of ShutdownMode))
            Dim testCode As Action =
                Sub()
                    ValidateShutdownModeEnumValue(testData.Value, NameOf(ShutdownMode))
                End Sub

            If testData.Throws Then
                testCode.Should.Throw(Of InvalidEnumArgumentException)()
            Else
                testCode.Should.NotThrow()
            End If
        End Sub

    End Class
End Namespace
