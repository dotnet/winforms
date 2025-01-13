' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.IO.Pipes
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Threading
Imports System.Windows.Forms
Imports System.Windows.Forms.Analyzers.Diagnostics

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Signature for the ApplyApplicationDefaults event handler.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub ApplyApplicationDefaultsEventHandler(sender As Object, e As ApplyApplicationDefaultsEventArgs)

    ''' <summary>
    '''  Signature for the Shutdown event handler.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub ShutdownEventHandler(sender As Object, e As EventArgs)

    ''' <summary>
    '''  Signature for the Startup event handler.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub StartupEventHandler(sender As Object, e As StartupEventArgs)

    ''' <summary>
    '''  Signature for the StartupNextInstance event handler.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub StartupNextInstanceEventHandler(sender As Object, e As StartupNextInstanceEventArgs)

    ''' <summary>
    '''  Signature for the UnhandledException event handler.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub UnhandledExceptionEventHandler(sender As Object, e As UnhandledExceptionEventArgs)

    ''' <summary>
    '''  Provides the infrastructure for the VB Windows Forms application model.
    ''' </summary>
    ''' <remarks>Don't put access on this definition.</remarks>
    Partial Public Class WindowsFormsApplicationBase : Inherits ConsoleApplicationBase

        ' How long a subsequent instance will wait for the original instance to get on its feet.
        Private Const SecondInstanceTimeOut As Integer = 2500

        Private ReadOnly _appContext As WinFormsAppContext

        ' Sync object
        Private ReadOnly _networkAvailabilityChangeLock As New Object

        Private ReadOnly _splashLock As New Object

        Private _appSynchronizationContext As SynchronizationContext

#Disable Warning WFO5001 ' Type is for evaluation purposes only and is subject to change or removal in future updates.

        ' The ColorMode (Classic/Light, System, Dark) the user assigned to the ApplyApplicationsDefault event.
        ' Note: We aim to expose this to the App Designer in later runtime/VS versions.
        Private _colorMode As SystemColorMode = SystemColorMode.Classic

#Enable Warning WFO5001

        ' We only need to show the splash screen once.
        ' Protect the user from himself if they are overriding our app model.
        Private _didSplashScreen As Boolean

        ' Whether to use Windows XP styles.
        Private _enableVisualStyles As Boolean

        ' Whether we have made it through the processing of OnInitialize.
        Private _finishedOnInitialize As Boolean

        Private _formLoadWaiter As AutoResetEvent

        ' The HighDpiMode the user picked from the AppDesigner or assigned to the ApplyApplicationsDefault event.
        Private _highDpiMode As HighDpiMode = HighDpiMode.SystemAware

        ' Whether this app runs using Word like instancing behavior.
        Private _isSingleInstance As Boolean

        ' Minimum amount of time to show the splash screen. 0 means hide as soon as the app comes up.
        Private _minimumSplashExposure As Integer = MinimumSplashExposureDefault

        Private _networkAvailabilityEventHandlers As List(Of Devices.NetworkAvailableEventHandler)

        Private _networkObject As Devices.Network

        Private _processingUnhandledExceptionEvent As Boolean

        ' Informs My.Settings whether to save the settings on exit or not.
        Private _saveMySettingsOnExit As Boolean

        ' Defines when the application decides to close.
        Private _shutdownStyle As ShutdownMode

        Private _splashScreen As Form

        ' For splash screens with a minimum display time, this let's us know when that time
        ' has expired and it is OK to close the splash screen.
        Private _splashScreenCompletionSource As TaskCompletionSource(Of Boolean)

        Private _splashTimer As Timers.Timer

        ' Tracks whether we need to create the network object so we can listen to the NetworkAvailabilityChanged event.
        Private _turnOnNetworkListener As Boolean

        Private _unhandledExceptionHandlers As List(Of UnhandledExceptionEventHandler)

        ' Milliseconds.
        Friend Const MinimumSplashExposureDefault As Integer = 2000

        Friend Const WinFormsExperimentalUrl As String = "https://aka.ms/winforms-experimental/{0}"

        ' Used to marshal a call to Dispose on the Splash Screen.
        Private Delegate Sub DisposeDelegate()

        ''' <summary>
        '''  Constructs the application Shutdown/Startup model object
        ''' </summary>
        ''' <remarks>
        '''  We have to have a parameterless ctor because the platform specific Application object
        '''  derives from this one and it doesn't define a ctor because the partial class generated by the
        '''  designer does that to configure the application.
        ''' </remarks>
        Public Sub New()
            Me.New(AuthenticationMode.Windows)
        End Sub

        ''' <summary>
        '''  Constructs the application Shutdown/Startup model object
        ''' </summary>
        <SecuritySafeCritical()>
        Public Sub New(authenticationMode As AuthenticationMode)
            MyBase.New()

            ValidateAuthenticationModeEnumValue(authenticationMode, NameOf(authenticationMode))

            ' Setup Windows Authentication if that's what the user wanted. Note, we want to do this now,
            ' before the Network object gets created because the network object will be doing a
            ' AsyncOperationsManager.CreateOperation() which captures the execution context. So we must
            ' have our principal on the thread before that happens.
            If authenticationMode = AuthenticationMode.Windows Then
                Try
                    ' Consider: Sadly, a call to:
                    ' Security.SecurityManager.IsGranted(New SecurityPermission(SecurityPermissionFlag.ControlPrincipal))
                    ' Will only check the THIS caller so you'll always get TRUE.
                    ' What we need is a way to get to the value of this on a demand basis.
                    ' So I try/catch instead for now but would rather be able to IF my way around this block.
                    Dim ntIdentity As Principal.WindowsIdentity = Principal.WindowsIdentity.GetCurrent
                    Thread.CurrentPrincipal = New Principal.WindowsPrincipal(ntIdentity)
                Catch ex As SecurityException
                End Try
            End If

            _appContext = New WinFormsAppContext(Me)

            ' We need to set the WindowsFormsSynchronizationContext because the network object is going to
            ' get created after this ctor runs (network gets created during event hookup) and we need the
            ' context in place for it to latch on to. The WindowsFormsSynchronizationContext won't otherwise
            ' get created until OnCreateMainForm() when the startup form is created and by then it is too late.
            ' When the startup form gets created, WinForms is going to push our context into the previous context
            ' and then restore it when Application.Run() exits.
            _appSynchronizationContext = AsyncOperationManager.SynchronizationContext
        End Sub

        ''' <summary>
        '''  Use GDI for the text rendering engine by default.
        '''  The user can shadow this function to return True if they want their app
        '''  to use the GDI+ render. We read this function in Main() (My template) to
        '''  determine how to set the text rendering flag on the WinForms application object.
        ''' </summary>
        ''' <value><see langword="True"/> if uses GDI+ renderer. <see langword="False"/> if uses GDI renderer.</value>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Shared ReadOnly Property UseCompatibleTextRendering() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        '''  Gets or sets the ColorMode for the Application.
        ''' </summary>
        ''' <value>
        '''  The <see cref="SystemColorMode"/> that the application is running in.
        ''' </value>
        <Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat:=WinFormsExperimentalUrl)>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Protected Property ColorMode As SystemColorMode
            Get
                Return _colorMode
            End Get
            Set(value As SystemColorMode)
                _colorMode = value
            End Set
        End Property

        ''' <summary>
        '''  Determines whether this application will use the XP Windows styles for windows, controls, etc.
        ''' </summary>
        Protected Property EnableVisualStyles() As Boolean
            Get
                Return _enableVisualStyles
            End Get
            Set(value As Boolean)
                _enableVisualStyles = value
            End Set
        End Property

        ''' <summary>
        '''  Gets or sets the HighDpiMode for the Application.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Protected Property HighDpiMode() As HighDpiMode
            Get
                Return _highDpiMode
            End Get
            Set(value As HighDpiMode)
                _highDpiMode = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Property IsSingleInstance() As Boolean
            Get
                Return _isSingleInstance
            End Get
            Set(value As Boolean)
                _isSingleInstance = value
            End Set
        End Property

        ''' <summary>
        '''  Determines when this application will terminate (when the main form goes down, all forms)
        ''' </summary>
        Protected Friend Property ShutdownStyle() As ShutdownMode
            Get
                Return _shutdownStyle
            End Get
            Set(value As ShutdownMode)
                ValidateShutdownModeEnumValue(value, NameOf(value))
                _shutdownStyle = value
            End Set
        End Property

        ''' <summary>
        '''  Provides the WinForms application context that we are running on
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public ReadOnly Property ApplicationContext() As ApplicationContext
            Get
                Return _appContext
            End Get
        End Property

        ''' <summary>
        '''  Informs My.Settings whether to save the settings on exit or not
        ''' </summary>
        Public Property SaveMySettingsOnExit() As Boolean
            Get
                Return _saveMySettingsOnExit
            End Get
            Set(value As Boolean)
                _saveMySettingsOnExit = value
            End Set
        End Property

        ''' <summary>
        '''  Provides access to the splash screen for this application
        ''' </summary>
        Public Property SplashScreen() As Form
            Get
                Return _splashScreen
            End Get
            Set(value As Form)

                ' Allow for the case where they set splash screen = nothing and mainForm is currently nothing.
                If value IsNot Nothing AndAlso value Is _appContext.MainForm Then
                    Dim message As String = VbUtils.GetResourceString(SR.AppModel_SplashAndMainFormTheSame)
                    Throw New ArgumentException(message)
                End If

                _splashScreen = value
            End Set
        End Property

        ''' <summary>
        '''  Provides access to the main form for this application
        ''' </summary>
        Protected Property MainForm() As Form
            Get
                Return _appContext?.MainForm
            End Get
            Set(value As Form)
                If value Is Nothing Then
                    Throw VbUtils.GetArgumentNullException("MainForm", SR.General_PropertyNothing, "MainForm")
                End If
                If value Is _splashScreen Then
                    Throw New ArgumentException(VbUtils.GetResourceString(SR.AppModel_SplashAndMainFormTheSame))
                End If
                _appContext.MainForm = value
            End Set
        End Property

        ''' <summary>
        '''  The splash screen timeout specifies whether there is a minimum time that the splash
        '''  screen should be displayed for. When not set then the splash screen is hidden
        '''  as soon as the main form becomes active.
        ''' </summary>
        ''' <value>The minimum amount of time, in milliseconds, to display the splash screen.</value>
        ''' <remarks>
        '''  This property, although public, used to be set in an `Overrides Function OnInitialize` _before_
        '''  calling `MyBase.OnInitialize`. We want to phase this out, and with the introduction of the
        '''  ApplyApplicationDefaults events have it handled in that event, rather than as awkwardly
        '''  as it is currently suggested to be used in the docs.
        '''  First step for that is to make it hidden in IntelliSense.
        ''' </remarks>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Property MinimumSplashScreenDisplayTime() As Integer
            Get
                Return _minimumSplashExposure
            End Get
            Set(value As Integer)
                _minimumSplashExposure = value
            End Set
        End Property

        ''' <summary>
        '''  Returns the collection of forms that are open. We no longer have thread
        '''  affinity meaning that this is the WinForms collection that contains Forms that may
        '''  have been opened on another thread then the one we are calling in on right now.
        ''' </summary>
        Public ReadOnly Property OpenForms() As FormCollection
            Get
                Return Application.OpenForms
            End Get
        End Property

        ''' <summary>
        '''  Occurs when the application is ready to accept default values for various application areas.
        ''' </summary>
        Public Event ApplyApplicationDefaults As ApplyApplicationDefaultsEventHandler

        ''' <summary>
        '''  Occurs when the application shuts down.
        ''' </summary>
        Public Event Shutdown As ShutdownEventHandler

        ''' <summary>
        '''  Occurs when the application starts.
        ''' </summary>
        Public Event Startup As StartupEventHandler

        ''' <summary>
        '''  Occurs when attempting to start a single-instance application and the application is already active.
        ''' </summary>
        Public Event StartupNextInstance As StartupNextInstanceEventHandler

        ''' <summary>
        '''  Occurs when the application encounters an <see cref="UnhandledException"/>.
        ''' </summary>
        Public Custom Event UnhandledException As UnhandledExceptionEventHandler

            ' This is a custom event because we want to hook up System.Windows.Forms.Application.ThreadException
            ' only if the user writes a handler for this event. We only want to hook the ThreadException event
            ' if the user is handling this event because the act of listening to Application.ThreadException
            ' causes WinForms to snuff exceptions and we only want WinForms to do that if we are assured that
            ' the user wrote their own handler to deal with the error instead.
            AddHandler(value As UnhandledExceptionEventHandler)
                If _unhandledExceptionHandlers Is Nothing Then
                    _unhandledExceptionHandlers = New List(Of UnhandledExceptionEventHandler)
                End If

                _unhandledExceptionHandlers.Add(value)

                ' Only add the listener once so we don't fire the
                ' UnHandledException event over and over for the same exception
                If _unhandledExceptionHandlers.Count = 1 Then
                    AddHandler Application.ThreadException, AddressOf OnUnhandledExceptionEventAdaptor
                End If
            End AddHandler

            RemoveHandler(value As UnhandledExceptionEventHandler)
                If _unhandledExceptionHandlers IsNot Nothing AndAlso
                    _unhandledExceptionHandlers.Count > 0 Then
                    _unhandledExceptionHandlers.Remove(value)

                    ' Last one to leave, turn out the lights...
                    If _unhandledExceptionHandlers.Count = 0 Then
                        RemoveHandler Application.ThreadException, AddressOf OnUnhandledExceptionEventAdaptor
                    End If
                End If
            End RemoveHandler

            RaiseEvent(sender As Object, e As UnhandledExceptionEventArgs)
                If _unhandledExceptionHandlers IsNot Nothing Then

                    ' In the case that we throw from the UnhandledException handler, we don't want to
                    ' run the UnhandledException handler again.
                    _processingUnhandledExceptionEvent = True

                    For Each handler As UnhandledExceptionEventHandler In _unhandledExceptionHandlers
                        handler?.Invoke(sender, e)
                    Next

                    ' Now that we are out of the UnhandledException handler, treat exceptions normally again.
                    _processingUnhandledExceptionEvent = False
                End If
            End RaiseEvent
        End Event

        ''' <summary>
        '''  Occurs when the network availability changes.
        ''' </summary>
        Public Custom Event NetworkAvailabilityChanged As Devices.NetworkAvailableEventHandler
            ' This is a custom event because we want to hook up the NetworkAvailabilityChanged event only
            ' if the user writes a handler for it.
            ' The reason being that it is very expensive to handle and kills our application startup performance.
            AddHandler(value As Devices.NetworkAvailableEventHandler)
                SyncLock _networkAvailabilityChangeLock
                    If _networkAvailabilityEventHandlers Is Nothing Then
                        _networkAvailabilityEventHandlers = New List(Of Devices.NetworkAvailableEventHandler)
                    End If

                    _networkAvailabilityEventHandlers.Add(value)

                    ' We don't want to create the network object now - it takes a snapshot
                    ' of the executionContext and our IPrincipal isn't on the thread yet.
                    ' We know we need to create it and we will at the appropriate time
                    _turnOnNetworkListener = True

                    ' But the user may be doing an AddHandler of their own in which case we need
                    ' to make sure to honor the request. If we aren't past OnInitialize() yet
                    ' we shouldn't do it but the flag above catches that case.
                    If _networkObject Is Nothing AndAlso _finishedOnInitialize Then
                        _networkObject = New Devices.Network
                        Dim windowsFormsApplicationBase As WindowsFormsApplicationBase = Me
                        AddHandler _networkObject.NetworkAvailabilityChanged,
                            AddressOf windowsFormsApplicationBase.NetworkAvailableEventAdaptor
                    End If
                End SyncLock
            End AddHandler

            RemoveHandler(value As Devices.NetworkAvailableEventHandler)
                If _networkAvailabilityEventHandlers IsNot Nothing AndAlso
                    _networkAvailabilityEventHandlers.Count > 0 Then

                    _networkAvailabilityEventHandlers.Remove(value)

                    ' Last one to leave - turn out the lights...
                    If _networkAvailabilityEventHandlers.Count = 0 Then
                        RemoveHandler _networkObject.NetworkAvailabilityChanged, AddressOf NetworkAvailableEventAdaptor
                        If _networkObject IsNot Nothing Then

                            ' Stop listening to network change events because we are going to go away.
                            _networkObject.DisconnectListener()

                            ' No sense holding on to this if nobody is listening.
                            _networkObject = Nothing
                        End If
                    End If
                End If
            End RemoveHandler

            RaiseEvent(sender As Object, e As Devices.NetworkAvailableEventArgs)
                If _networkAvailabilityEventHandlers IsNot Nothing Then
                    For Each handler As Devices.NetworkAvailableEventHandler In _networkAvailabilityEventHandlers
                        Try
                            If handler IsNot Nothing Then handler.Invoke(sender, e)
                        Catch ex As Exception
                            If Not OnUnhandledException(New UnhandledExceptionEventArgs(True, ex)) Then

                                ' The user didn't write a handler so throw the error up the chain.
                                Throw
                            End If
                        End Try
                    Next
                End If
            End RaiseEvent
        End Event

        ''' <summary>
        '''  Displays the splash screen. We get called here from a different thread than what the
        '''  main form is starting up on. This allows us to process events for the Splash screen so
        '''  it doesn't freeze up while the main form is getting it together.
        ''' </summary>
        Private Sub DisplaySplash()
            Debug.Assert(_splashScreen IsNot Nothing, "We should have never get here if there is no splash screen")

            If _splashTimer IsNot Nothing Then

                ' We only have a timer if there is a minimum time that the splash screen is supposed to be displayed.
                ' Enable the timer now that we are about to show the splash screen.
                _splashTimer.Enabled = True
            End If

            Application.Run(_splashScreen)
        End Sub

        ''' <summary>
        '''  Runs the user's program through the VB Startup/Shutdown application model
        ''' </summary>
        Private Sub DoApplicationModel()

            Dim eventArgs As New StartupEventArgs(CommandLineArgs)

            ' Only do the try/catch if we aren't running under the debugger.
            ' If we do try/catch under the debugger the debugger never gets
            ' a crack at exceptions which breaks the exception helper.
            If Not Debugger.IsAttached Then

                ' NO DEBUGGER ATTACHED - we use a catch so that we can run our UnhandledException code
                ' Note - Sadly, code changes within this IF (that don't pertain to exception handling)
                ' need to be mirrored in the ELSE debugger attached clause below.
                Try
                    If OnInitialize(CommandLineArgs) Then
                        If OnStartup(eventArgs) Then
                            OnRun()
                            OnShutdown()
                        End If
                    End If
                Catch ex As Exception

                    ' This catch is for exceptions that happen during the On* methods above,
                    ' but have occurred outside of the message pump (which exceptions we would
                    ' have already seen via our hook of System.Windows.Forms.Application.ThreadException).
                    If _processingUnhandledExceptionEvent Then

                        ' If the UnhandledException handler threw for some reason, throw that error out to the system.
                        Throw
                    Else

                        ' We had an exception, but not during the OnUnhandledException handler so give the user
                        ' a chance to look at what happened in the UnhandledException event handler
                        If Not OnUnhandledException(New UnhandledExceptionEventArgs(True, ex)) Then

                            ' The user didn't write a handler so throw the error out to the system
                            Throw
                        End If
                    End If
                End Try
            Else
                ' DEBUGGER ATTACHED - we don't have an uber catch when debugging so the exception
                ' will bubble out to the exception helper.
                ' We also don't hook up the Application.ThreadException event because WinForms ignores it
                ' when we are running under the debugger.
                If OnInitialize(CommandLineArgs) Then
                    If OnStartup(eventArgs) Then
                        OnRun()
                        OnShutdown()
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        '''  The Load() event happens before the Shown and Paint events. When we get called here
        '''  we know that the form load event is done and that the form is about to paint
        '''  itself for the first time.
        '''  We can now hide the splash screen.
        '''  Note that this function gets called from the main thread - the same thread
        '''  that creates the startup form.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub MainFormLoadingDone(sender As Object, e As EventArgs)

            ' We don't want this event to call us again.
            RemoveHandler MainForm.Load, AddressOf MainFormLoadingDone

            ' Now, we don't want to eat up a complete Processor Core just for waiting on the main form,
            ' since this is eating up a LOT of energy and workload, especially on Notebooks and tablets.
            If _splashScreenCompletionSource IsNot Nothing Then

                _formLoadWaiter = New AutoResetEvent(False)

                Task.Run(Async Function() As Task
                             Await _splashScreenCompletionSource.Task.ConfigureAwait(continueOnCapturedContext:=False)
                             _formLoadWaiter.Set()
                         End Function)

                ' Block until the splash screen time is up.
                ' See MinimumSplashExposureTimeIsUp() which releases us.
                _formLoadWaiter.WaitOne()
            End If

            HideSplashScreen()
        End Sub

        ''' <summary>
        '''  If a splash screen has a minimum time out, then once that is up we check to see whether
        '''  we should close the splash screen. If the main form has activated then we close it.
        '''  Note that we are getting called on a secondary thread here which isn't necessarily
        '''  associated with any form. Don't touch forms from this function.
        ''' </summary>
        Private Sub MinimumSplashExposureTimeIsUp(sender As Object, e As Timers.ElapsedEventArgs)

            If _splashTimer IsNot Nothing Then

                ' We only have a timer if there was a minimum timeout on the splash screen.
                _splashTimer.Dispose()
                _splashTimer = Nothing
            End If

            _splashScreenCompletionSource.SetResult(True)
        End Sub

        ''' <summary>
        '''  Handles the Network.NetworkAvailability event (on the correct thread) and raises the
        '''  NetworkAvailabilityChanged event.
        ''' </summary>
        ''' <param name="Sender">Contains the Network instance that raised the event.</param>
        ''' <param name="e">Contains whether the network is available or not.</param>
        Private Sub NetworkAvailableEventAdaptor(sender As Object, e As Devices.NetworkAvailableEventArgs)
            RaiseEvent NetworkAvailabilityChanged(sender, e)
        End Sub

        Private Sub OnStartupNextInstanceMarshallingAdaptor(args As String())

            Dim invoked As Boolean = False

            Try
                Dim handleNextInstance As New Action(
                    Sub()
                        invoked = True
                        OnStartupNextInstance(New StartupNextInstanceEventArgs(
                                        New ReadOnlyCollection(Of String)(args),
                                        bringToForegroundFlag:=True))
                    End Sub)

                ' If we have a Main form, we need to make sure that we are on _its_ UI thread
                ' before we call OnStartupNextInstance.
                If MainForm IsNot Nothing Then
                    MainForm.Invoke(handleNextInstance)
                Else
                    ' Otherwise, we need to make sure that we are on the thread
                    ' provided by the SynchronizationContext.
                    AsyncOperationManager.
                        SynchronizationContext.
                        Send(Sub() handleNextInstance(), Nothing)
                End If
            Catch ex As Exception When Not invoked
                ' Only catch exceptions thrown when the UI thread is not available, before
                ' the UI thread has been created or after it has been terminated. Exceptions
                ' thrown from OnStartupNextInstance() should be allowed to propagate.
            End Try
        End Sub

        ''' <summary>
        '''  Handles the Windows.Forms.Application.ThreadException event and raises our Unhandled
        '''  exception event.
        ''' </summary>
        ''' <param name="e"></param>
        ''' <remarks>
        '''  Our UnHandledException event has a different signature then the Windows.Forms.Application
        '''  <see cref="UnhandledException"/> event so we do the translation here before raising our event.
        ''' </remarks>
        Private Sub OnUnhandledExceptionEventAdaptor(sender As Object, e As ThreadExceptionEventArgs)
            OnUnhandledException(New UnhandledExceptionEventArgs(True, e.Exception))
        End Sub

        ''' <summary>
        '''  Hide the splash screen. The splash screen was created on another thread
        '''  thread (main thread) than the one it was run on (secondary thread for the
        '''  splash screen so it doesn't block app startup. We need to invoke the close.
        '''  This function gets called from the main thread by the app fx.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <SecuritySafeCritical()>
        Protected Sub HideSplashScreen()

            ' This ultimately wasn't necessary. I suppose we better keep it for backwards compatibility.
            SyncLock _splashLock

                ' .NET Framework 4.0 (Dev10 #590587) - we now activate the main form before calling
                ' Dispose on the Splash screen. (we're just swapping the order of the two If blocks.)
                ' This is to fix the issue where the main form doesn't come to the front after the
                ' Splash screen disappears.
                MainForm?.Activate()

                If _splashScreen IsNot Nothing AndAlso Not _splashScreen.IsDisposed Then
                    Dim disposeSplashDelegate As New DisposeDelegate(AddressOf _splashScreen.Dispose)
                    _splashScreen.Invoke(disposeSplashDelegate)
                    _splashScreen = Nothing
                End If
            End SyncLock
        End Sub

        ''' <summary>
        '''  Provides a hook that designers will override to set the main form.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnCreateMainForm()
        End Sub

        ''' <summary>
        '''  A designer will override this method and provide a splash screen if this application has one.
        ''' </summary>
        ''' <remarks>
        '''  For instance, a designer would override this method and emit: Me.Splash = new Splash
        '''  where Splash was designated in the application designer as being the splash screen for this app
        ''' </remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnCreateSplashScreen()
        End Sub

        ''' <summary>
        '''  This exposes the first in a series of extensibility points for the Startup process. By default, it shows
        '''  the splash screen and does rudimentary processing of the command line to see if /nosplash or its
        '''  variants was passed in.
        ''' </summary>
        ''' <param name="commandLineArgs"></param>
        ''' <returns>
        '''  Returning <see langword="True">
        '''   Indicates that we should continue on with the application Startup sequence.
        '''  </see>
        ''' </returns>
        ''' <remarks>
        '''  This extensibility point is exposed for people who want to override
        '''  the Startup sequence at the earliest possible point to
        ''' </remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced), STAThread()>
        Protected Overridable Function OnInitialize(commandLineArgs As ReadOnlyCollection(Of String)) As Boolean

            ' Rationale for how we process the default values and how we let the user modify
            ' them on demand via the ApplyApplicationDefaults event.
            ' ===========================================================================================
            ' a) Users used to be able to set MinimumSplashScreenDisplayTime _only_ by overriding OnInitialize
            '    in a derived class and setting `MyBase.MinimumSplashScreenDisplayTime` there.
            '    We are picking this (probably) changed value up, and pass it to the ApplyDefaultsEvents
            '    where it could be modified (again). So event wins over Override over default value (2 seconds).
            ' b) We feed the defaults for HighDpiMode, ColorMode, VisualStylesMode to the EventArgs.
            '    With the introduction of the HighDpiMode property, we changed Project System the chance to reflect
            '    those default values in the App Designer UI and have it code-generated based on a modified
            '    Application.myapp, which would result it to be set in the derived constructor.
            '    (See the hidden file in the Solution Explorer "My Project\Application.myapp\Application.Designer.vb
            '     for how those UI-set values get applied.)
            '    Once all this is done, we give the User another chance to change the value by code through
            '    the ApplyDefaults event.
            ' Note: Overriding MinimumSplashScreenDisplayTime needs still to keep working!
#Disable Warning WFO5001 ' Type is for evaluation purposes only and is subject to change or removal in future updates.
            Dim applicationDefaultsEventArgs As New ApplyApplicationDefaultsEventArgs(
                MinimumSplashScreenDisplayTime,
                HighDpiMode,
                ColorMode) With
            {
                .MinimumSplashScreenDisplayTime = MinimumSplashScreenDisplayTime
            }
#Enable Warning WFO5001

            RaiseEvent ApplyApplicationDefaults(Me, applicationDefaultsEventArgs)

            If applicationDefaultsEventArgs.Font IsNot Nothing Then
                Application.SetDefaultFont(applicationDefaultsEventArgs.Font)
            End If

            MinimumSplashScreenDisplayTime = applicationDefaultsEventArgs.MinimumSplashScreenDisplayTime

            ' This creates the native window, and that means, we can no longer apply a different Default Font.
            ' So, this is the earliest point in time to set the AsyncOperationManager's SyncContext.
            AsyncOperationManager.SynchronizationContext = New WindowsFormsSynchronizationContext()

            _highDpiMode = applicationDefaultsEventArgs.HighDpiMode

#Disable Warning WFO5001 ' Type is for evaluation purposes only and is subject to change or removal in future updates.

            _colorMode = applicationDefaultsEventArgs.ColorMode

            ' Then, it's applying what we got back as HighDpiMode.
            Dim dpiSetResult As Boolean = Application.SetHighDpiMode(_highDpiMode)

            If dpiSetResult Then
                _highDpiMode = Application.HighDpiMode
            End If
            Debug.Assert(dpiSetResult, "We could not set the HighDpiMode.")

            ' Now, let's set VisualStyles and ColorMode:
            If _enableVisualStyles Then
                Application.EnableVisualStyles()
            End If

            Application.SetColorMode(_colorMode)

#Enable Warning WFO5001

            ' We'll handle "/nosplash" for you.
            If Not (commandLineArgs.Contains("/nosplash") OrElse Me.CommandLineArgs.Contains("-nosplash")) Then
                ShowSplashScreen()
            End If

            _finishedOnInitialize = True

            ' We are now at a point where we can allow the network object
            ' to be created since the iPrincipal is on the thread by now.

            ' True means to not bail out but keep on running after OnInitialize() finishes
            Return True
        End Function

        ''' <summary>
        '''  At this point, the command line args should have been processed and the application will create the
        '''  main form and enter the message loop.
        ''' </summary>
        <SecuritySafeCritical()>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnRun()

            If MainForm Is Nothing Then

                ' A designer overrides OnCreateMainForm() to set the main form we are supposed to use.
                OnCreateMainForm()

                If MainForm Is Nothing Then
                    Throw New NoStartupFormException
                End If

                ' When we have a splash screen that hasn't timed out before the main form is ready to paint, we want to
                ' block the main form from painting. To do that I let the form get past the Load() event and
                ' hold it until the splash screen goes down. Then I let the main form continue it's startup sequence.
                ' The ordering of Form startup events for reference is:
                ' Ctor(), Load Event, Layout event, Shown event, Activated event, Paint event.
                AddHandler MainForm.Load, AddressOf MainFormLoadingDone
            End If

            ' Run() eats all exceptions (unless running under the debugger). If the user wrote an
            ' UnhandledException handler we will hook the System.Windows.Forms.Application.ThreadException event
            ' (see Public Custom Event UnhandledException) which will raise our UnhandledException Event.
            ' If our user didn't write an UnhandledException event, then we land in the try/catch handler
            ' for Forms.Application.Run().
            Try
                Application.Run(_appContext)
            Finally

                ' When Run() returns, the context we pushed in our ctor
                ' (which was a WindowsFormsSynchronizationContext) is restored.
                ' But we are going to dispose it so we need to disconnect the network listener so that it
                ' can't fire any events in response to changing network availability conditions through a dead context.
                If _networkObject IsNot Nothing Then _networkObject.DisconnectListener()

                ' Restore the prior sync context.
                AsyncOperationManager.SynchronizationContext = _appSynchronizationContext
                _appSynchronizationContext = Nothing
            End Try
        End Sub

        ''' <summary>
        '''  The last in a series of extensibility points for the Shutdown process
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnShutdown()
            RaiseEvent Shutdown(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        '''  Extensibility point which raises the Startup event
        ''' </summary>
        ''' <param name="eventArgs"></param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Function OnStartup(eventArgs As StartupEventArgs) As Boolean

            eventArgs.Cancel = False

            ' It is important not to create the network object until the ExecutionContext has everything on it.
            ' By now the principal will be on the thread so we can create the network object.
            ' The timing is important because the network object has an AsyncOperationsManager in it that marshals
            ' the network changed event to the main thread. The asyncOperationsManager does a CreateOperation()
            ' which makes a copy of the executionContext. That execution context shows up on your thread during
            ' the callback so I delay creating the network object
            ' (and consequently the capturing of the execution context) until the principal has been set on the thread.
            ' This avoids the problem where My.User isn't set during the NetworkAvailabilityChanged event.
            ' This problem would just extend itself to any future callback that involved
            ' the asyncOperationsManager so this is where we need to create objects that have
            ' a asyncOperationsContext in them.
            If _turnOnNetworkListener And _networkObject Is Nothing Then

                ' The is-nothing-check is to avoid hooking the object more than once.
                _networkObject = New Devices.Network
                AddHandler _networkObject.NetworkAvailabilityChanged, AddressOf NetworkAvailableEventAdaptor
            End If

            RaiseEvent Startup(Me, eventArgs)

            Return Not eventArgs.Cancel
        End Function

        ''' <summary>
        '''  Extensibility point which raises the StartupNextInstance
        ''' </summary>
        ''' <param name="eventArgs"></param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <SecuritySafeCritical()>
        Protected Overridable Sub OnStartupNextInstance(eventArgs As StartupNextInstanceEventArgs)

            RaiseEvent StartupNextInstance(Me, eventArgs)

            ' Activate the original instance.
            If eventArgs.BringToForeground AndAlso MainForm IsNot Nothing Then
                If MainForm.WindowState = FormWindowState.Minimized Then
                    MainForm.WindowState = FormWindowState.Normal
                End If

                MainForm.Activate()
            End If
        End Sub

        ''' <summary>
        '''  Raises the <see cref="UnhandledException"/> event and exits the application if the event handler indicated
        '''  that execution shouldn't continue.
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns>
        '''  <see langword="True"/> indicates the exception event was raised
        '''  <see langword="False"/> it was not.
        ''' </returns>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Function OnUnhandledException(e As UnhandledExceptionEventArgs) As Boolean

            ' Does the user have a handler for this event?
            If _unhandledExceptionHandlers IsNot Nothing AndAlso _unhandledExceptionHandlers.Count > 0 Then

                ' We don't put a try/catch around the handler event so that exceptions in there will
                ' bubble out - else we will have a recursive exception handler.
                RaiseEvent UnhandledException(Me, e)
                If e.ExitApplication Then Application.Exit()

                ' User handled the event.
                Return True
            End If

            ' Nobody was listening to the UnhandledException event.
            Return False
        End Function

        ''' <summary>
        '''  Uses the extensibility model to see if there is a splash screen provided for this app and if there is,
        '''  displays it.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Sub ShowSplashScreen()
            If Not _didSplashScreen Then
                _didSplashScreen = True

                If _splashScreen Is Nothing Then

                    ' If the user specified a splash screen, the designer will have overridden this method to set it.
                    OnCreateSplashScreen()
                End If

                If _splashScreen IsNot Nothing Then

                    ' Some splash screens have minimum face time they are supposed to get.
                    ' We'll set up a time to let us know when we can take it down.
                    If _minimumSplashExposure > 0 Then

                        ' Don't close until the timer expires.
                        _splashTimer = New Timers.Timer(_minimumSplashExposure)
                        AddHandler _splashTimer.Elapsed, AddressOf MinimumSplashExposureTimeIsUp
                        _splashTimer.AutoReset = False

                        ' We only need this, when we actually need to wait for it.
                        _splashScreenCompletionSource = New TaskCompletionSource(Of Boolean)

                        ' We'll enable it in DisplaySplash() once the splash screen thread gets running.
                    End If

                    ' Run the splash screen on another thread so we don't starve it for events and painting
                    ' while the main form gets its act together.
                    Task.Run(AddressOf DisplaySplash)
                End If
            End If
        End Sub

        ''' <summary>
        '''  Generates the name for the remote singleton that we use to channel multiple instances
        '''  to the same application model thread.
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns>
        '''  A string unique to the application that should be the same for versions of
        '''  the application that have the same Major and Minor Version Number.
        ''' </returns>
        ''' <remarks>If GUID Attribute does not exist fall back to unique ModuleVersionId.</remarks>
        Friend Shared Function GetApplicationInstanceID(entry As Assembly) As String

            Dim guidAttrib As GuidAttribute = entry.GetCustomAttribute(Of GuidAttribute)()

            If guidAttrib IsNot Nothing Then

                Dim version As Version = entry.GetName.Version

                If version IsNot Nothing Then
                    Return $"{guidAttrib.Value}{version.Major}.{version.Minor}"
                Else
                    Return guidAttrib.Value
                End If
            End If

            Return entry.ManifestModule.ModuleVersionId.ToString()
        End Function

        ''' <summary>
        '''  Validates that the value being passed as an AuthenticationMode enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Friend Shared Sub ValidateAuthenticationModeEnumValue(value As AuthenticationMode, paramName As String)
            If value < AuthenticationMode.Windows OrElse value > AuthenticationMode.ApplicationDefined Then
                Throw New InvalidEnumArgumentException(paramName, value, GetType(AuthenticationMode))
            End If
        End Sub

        ''' <summary>
        '''  Validates that the value being passed as an ShutdownMode enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Friend Shared Sub ValidateShutdownModeEnumValue(value As ShutdownMode, paramName As String)
            If value < ShutdownMode.AfterMainFormCloses OrElse value > ShutdownMode.AfterAllFormsClose Then
                Throw New InvalidEnumArgumentException(paramName, value, GetType(ShutdownMode))
            End If
        End Sub

        ''' <summary>
        '''  Processes all windows messages currently in the message queue
        ''' </summary>
        Public Sub DoEvents()
            Application.DoEvents()
        End Sub

        ''' <summary>
        '''  Entry point to kick off the VB Startup/Shutdown Application model.
        ''' </summary>
        ''' <param name="commandLine">The command line from Main().</param>
        <SecuritySafeCritical()>
        Public Sub Run(commandLine As String())

            ' Prime the command line args with what we receive from Main() so that Click-Once windows
            ' apps don't have to do a System.Environment call which would require permissions.
            InternalCommandLine = New ReadOnlyCollection(Of String)(commandLine)

            If Not IsSingleInstance Then
                DoApplicationModel()
            Else
                ' This is a Single-Instance application

                ' Note: Must pass the calling assembly from here so we can get the running app.
                ' Otherwise, can break single instance.
                Dim applicationInstanceID As String = GetApplicationInstanceID(Assembly.GetCallingAssembly)
                Dim pipeServer As NamedPipeServerStream = Nothing

                If TryCreatePipeServer(applicationInstanceID, pipeServer) Then

                    ' --- This is the first instance of a single-instance application to run.
                    ' This is the instance that subsequent instances will attach to.
                    Using pipeServer
                        Dim tokenSource As New CancellationTokenSource()
#Disable Warning BC42358 ' Call is not awaited.
                        WaitForClientConnectionsAsync(
                            pipeServer,
                            AddressOf OnStartupNextInstanceMarshallingAdaptor,
                            cancellationToken:=tokenSource.Token)
#Enable Warning BC42358
                        DoApplicationModel()
                        tokenSource.Cancel()
                    End Using
                Else

                    ' --- We are launching a subsequent instance.
                    Dim tokenSource As New CancellationTokenSource()
                    tokenSource.CancelAfter(SecondInstanceTimeOut)
                    Try
                        Dim awaitable As ConfiguredTaskAwaitable = SendSecondInstanceArgsAsync(
                            pipeName:=applicationInstanceID,
                            args:=commandLine,
                            cancellationToken:=tokenSource.Token).ConfigureAwait(continueOnCapturedContext:=False)

                        awaitable.GetAwaiter().GetResult()
                    Catch ex As Exception
                        Throw New CantStartSingleInstanceException()
                    End Try
                End If
            End If ' Single-Instance application
        End Sub

    End Class
End Namespace
