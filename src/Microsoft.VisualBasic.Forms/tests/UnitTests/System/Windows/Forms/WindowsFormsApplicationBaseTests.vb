' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks
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

            ColorMode.Should.Be(SystemColorMode.Classic)
            ColorMode = SystemColorMode.Dark
            ColorMode.Should.Be(SystemColorMode.Dark)

            VisualStylesMode.Should.Be(VisualStylesMode.Classic)

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
                    MainForm = splashScreenForm
                    MainForm.Should.Be(splashScreenForm)
                    SplashScreen.Should.Be(form2)
                    ApplicationContext.Should.NotBeNull()

                    testCode = Sub() SplashScreen = splashScreenForm
                    testCode.Should.Throw(Of ArgumentException)()

                    OpenForms.Count.Should.Be(0)
                    SplashScreen.Show()
                    OpenForms.Count.Should.Be(1)

                    splashScreenForm.Name = NameOf(splashScreenForm)
                    ApplicationContext.MainForm.Name.Should.Be(NameOf(splashScreenForm))
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
        Public Sub OnInitialize_ApplyApplicationDefaults_VisualStylesModeFlowsToApplication()
            If RemoteExecutor.IsSupported Then
                Dim test As Action =
                    Sub()
                        Dim appModel As New SubWindowsFormsApplicationBase() With
                        {
                            .EnableVisualStylesCore = True
                        }

                        AddHandler appModel.ApplyApplicationDefaults,
                            Sub(sender, e)
                                e.VisualStylesMode = VisualStylesMode.Latest
                            End Sub

                        appModel.CallOnInitialize(Array.Empty(Of String)()).Should.BeTrue()
                        System.Windows.Forms.Application.DefaultVisualStylesMode.Should.Be(VisualStylesMode.Latest)
                    End Sub

                Using handle As RemoteInvokeHandle = RemoteExecutor.Invoke(test)
                    handle.ExitCode.Should.Be(RemoteExecutor.SuccessExitCode)
                End Using
            End If
        End Sub

        <WinFormsFact>
        Public Sub StartupNextInstance_HandlerThrows_ReportedOnUIThreadAndNextLaunchArrives()
            Dim originalSynchronizationContext As SynchronizationContext = SynchronizationContext.Current
            Dim deliveryContext As New WindowsFormsSynchronizationContext()
            Dim testThreadId As Integer = Environment.CurrentManagedThreadId
            Dim startupCallCount As Integer
            Dim receivedLaunch As String = Nothing
            Dim receivedLaunchThreadId As Integer
            Dim unhandledExceptionCallCount As Integer
            Dim observedException As Exception = Nothing
            Dim unhandledExceptionThreadId As Integer
            Dim firstException As New InvalidOperationException("first launch failed", New ArgumentException("inner"))

            Dim app As New SubWindowsFormsApplicationBase
            Dim startupHandler As StartupNextInstanceEventHandler =
                Sub(sender, e)
                    startupCallCount += 1
                    If startupCallCount = 1 Then
                        Throw firstException
                    End If

                    receivedLaunch = e.CommandLine(0)
                    receivedLaunchThreadId = Environment.CurrentManagedThreadId
                End Sub
            Dim unhandledExceptionHandler As UnhandledExceptionEventHandler =
                Sub(sender, e)
                    unhandledExceptionCallCount += 1
                    observedException = e.Exception
                    unhandledExceptionThreadId = Environment.CurrentManagedThreadId
                    e.ExitApplication = False
                End Sub

            AddHandler app.StartupNextInstance, startupHandler
            AddHandler app.UnhandledException, unhandledExceptionHandler

            Try
                app.CallEnableNextInstanceDelivery(deliveryContext)

                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"first"})).Wait()
                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"second"})).Wait()

                Dim timer As Stopwatch = Stopwatch.StartNew()
                While (unhandledExceptionCallCount = 0 OrElse receivedLaunch Is Nothing) AndAlso
                    timer.Elapsed < TimeSpan.FromSeconds(5)
                    Application.DoEvents()
                    Thread.Sleep(10)
                End While

                Assert.Equal(1, unhandledExceptionCallCount)
                Assert.Same(firstException, observedException)
                Assert.Equal(testThreadId, unhandledExceptionThreadId)
                Assert.Equal("second", receivedLaunch)
                Assert.Equal(testThreadId, receivedLaunchThreadId)
                Assert.Equal(2, startupCallCount)
            Finally
                RemoveHandler app.UnhandledException, unhandledExceptionHandler
                RemoveHandler app.StartupNextInstance, startupHandler
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub StartupNextInstance_SecondLaunchFailsWhileFirstFailureIsReported_BothReportedInOrder()
            Dim originalSynchronizationContext As SynchronizationContext = SynchronizationContext.Current
            Dim deliveryContext As New WindowsFormsSynchronizationContext()
            Dim firstException As New InvalidOperationException("first launch failed")
            Dim secondException As New InvalidOperationException("second launch failed")
            Dim observedExceptions As New List(Of Exception)
            Dim startupCallCount As Integer
            Dim reportingFirst As Boolean
            Dim secondStartedDuringFirstReport As Boolean

            Dim app As New SubWindowsFormsApplicationBase
            Dim startupHandler As StartupNextInstanceEventHandler =
                Sub(sender, e)
                    startupCallCount += 1
                    If e.CommandLine(0) = "first" Then
                        Throw firstException
                    End If

                    secondStartedDuringFirstReport = reportingFirst
                    Throw secondException
                End Sub
            Dim unhandledExceptionHandler As UnhandledExceptionEventHandler =
                Sub(sender, e)
                    observedExceptions.Add(e.Exception)
                    e.ExitApplication = False

                    If observedExceptions.Count = 1 Then
                        reportingFirst = True
                        Try
                            Dim reportTimer As Stopwatch = Stopwatch.StartNew()
                            While reportTimer.Elapsed < TimeSpan.FromMilliseconds(300)
                                Application.DoEvents()
                                Thread.Sleep(10)
                            End While
                        Finally
                            reportingFirst = False
                        End Try
                    End If
                End Sub

            AddHandler app.StartupNextInstance, startupHandler
            AddHandler app.UnhandledException, unhandledExceptionHandler

            Try
                app.CallEnableNextInstanceDelivery(deliveryContext)

                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"first"})).Wait()
                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"second"})).Wait()

                Dim timer As Stopwatch = Stopwatch.StartNew()
                While observedExceptions.Count < 2 AndAlso timer.Elapsed < TimeSpan.FromSeconds(5)
                    Application.DoEvents()
                    Thread.Sleep(10)
                End While

                Assert.False(secondStartedDuringFirstReport)
                Assert.Equal(2, observedExceptions.Count)
                Assert.Same(firstException, observedExceptions(0))
                Assert.Same(secondException, observedExceptions(1))
                Assert.Equal(2, startupCallCount)
            Finally
                RemoveHandler app.UnhandledException, unhandledExceptionHandler
                RemoveHandler app.StartupNextInstance, startupHandler
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub StartupNextInstance_BeforeDeliveryIsEnabled_IsDeliveredOnUIThreadOnceEnabled()
            Dim originalSynchronizationContext As SynchronizationContext = SynchronizationContext.Current
            Dim deliveryContext As New WindowsFormsSynchronizationContext()
            Dim testThreadId As Integer = Environment.CurrentManagedThreadId
            Dim startupCallCount As Integer
            Dim receivedLaunch As String = Nothing
            Dim receivedLaunchThreadId As Integer

            Dim app As New SubWindowsFormsApplicationBase
            Dim startupHandler As StartupNextInstanceEventHandler =
                Sub(sender, e)
                    startupCallCount += 1
                    receivedLaunch = e.CommandLine(0)
                    receivedLaunchThreadId = Environment.CurrentManagedThreadId
                End Sub

            AddHandler app.StartupNextInstance, startupHandler

            Try
                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"early"})).Wait()

                Dim waitTimer As Stopwatch = Stopwatch.StartNew()
                While waitTimer.Elapsed < TimeSpan.FromMilliseconds(200)
                    Application.DoEvents()
                    Thread.Sleep(10)
                End While

                Assert.Equal(0, startupCallCount)
                app.CallEnableNextInstanceDelivery(deliveryContext)

                Dim deliveryTimer As Stopwatch = Stopwatch.StartNew()
                While startupCallCount = 0 AndAlso deliveryTimer.Elapsed < TimeSpan.FromSeconds(5)
                    Application.DoEvents()
                    Thread.Sleep(10)
                End While

                Assert.Equal(1, startupCallCount)
                Assert.Equal("early", receivedLaunch)
                Assert.Equal(testThreadId, receivedLaunchThreadId)
            Finally
                RemoveHandler app.StartupNextInstance, startupHandler
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext)
            End Try
        End Sub

        <WinFormsFact>
        Public Sub StartupNextInstance_MultipleBeforeDeliveryIsEnabled_AllDeliveredInOrder()
            Dim originalSynchronizationContext As SynchronizationContext = SynchronizationContext.Current
            Dim deliveryContext As New WindowsFormsSynchronizationContext()
            Dim receivedLaunches As New List(Of String)

            Dim app As New SubWindowsFormsApplicationBase
            Dim startupHandler As StartupNextInstanceEventHandler =
                Sub(sender, e)
                    receivedLaunches.Add(e.CommandLine(0))
                End Sub

            AddHandler app.StartupNextInstance, startupHandler

            Try
                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"early1"})).Wait()
                Task.Run(Sub() app.CallOnStartupNextInstanceMarshallingAdaptor({"early2"})).Wait()

                Dim waitTimer As Stopwatch = Stopwatch.StartNew()
                While waitTimer.Elapsed < TimeSpan.FromMilliseconds(200)
                    Application.DoEvents()
                    Thread.Sleep(10)
                End While

                Assert.Empty(receivedLaunches)
                app.CallEnableNextInstanceDelivery(deliveryContext)

                Dim deliveryTimer As Stopwatch = Stopwatch.StartNew()
                While receivedLaunches.Count < 2 AndAlso deliveryTimer.Elapsed < TimeSpan.FromSeconds(5)
                    Application.DoEvents()
                    Thread.Sleep(10)
                End While

                Assert.Equal(2, receivedLaunches.Count)
                Assert.Equal("early1", receivedLaunches(0))
                Assert.Equal("early2", receivedLaunches(1))
            Finally
                RemoveHandler app.StartupNextInstance, startupHandler
                SynchronizationContext.SetSynchronizationContext(originalSynchronizationContext)
            End Try
        End Sub

        <Fact>
        Public Sub ShowHideSplashScreenSuccess()
            Dim testCode As Action
            Using form1 As New Form
                SplashScreen = form1
                Dim handle As IntPtr = form1.Handle
                testCode = Sub() ShowSplashScreen()
                testCode.Should.NotThrow()

                testCode = Sub() HideSplashScreen()
                testCode.Should.NotThrow()
            End Using
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

        Private NotInheritable Class SubWindowsFormsApplicationBase
            Inherits WindowsFormsApplicationBase

            Public Function CallOnInitialize(commandLineArgs As String()) As Boolean
                Return MyBase.OnInitialize(Array.AsReadOnly(commandLineArgs))
            End Function

            Public Sub CallOnStartupNextInstanceMarshallingAdaptor(args As String())
                MyBase.OnStartupNextInstanceMarshallingAdaptor(args)
            End Sub

            Public Sub CallEnableNextInstanceDelivery(context As SynchronizationContext)
                MyBase.EnableNextInstanceDelivery(context)
            End Sub

            Public WriteOnly Property EnableVisualStylesCore As Boolean
                Set(value As Boolean)
                    EnableVisualStyles = value
                End Set
            End Property
        End Class

    End Class
End Namespace
