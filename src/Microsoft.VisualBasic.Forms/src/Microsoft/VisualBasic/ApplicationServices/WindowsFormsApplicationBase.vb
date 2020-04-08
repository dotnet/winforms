' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.Versioning
Imports System.Security
Imports System.Security.AccessControl
Imports System.Security.Permissions
Imports System.Threading
Imports Microsoft.Win32.SafeHandles
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.Utils
Imports System.IO.Pipes
Imports System.Security.Principal
Imports System.Xml.Serialization

Namespace Microsoft.VisualBasic.ApplicationServices

    ' Any changes to this enum must be reflected in ValidateAuthenticationModeEnumValue()
    Public Enum AuthenticationMode
        Windows
        ApplicationDefined
    End Enum

    ' Any changes to this enum must be reflected in ValidateShutdownModeEnumValue()
    Public Enum ShutdownMode
        AfterMainFormCloses
        AfterAllFormsClose
    End Enum

    ''' <summary>
    ''' Provides the exception encountered along with a flag on whether to abort the program
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), ComVisible(False)>
    Public Class UnhandledExceptionEventArgs : Inherits ThreadExceptionEventArgs
        Sub New(ByVal exitApplication As Boolean, ByVal exception As System.Exception)
            MyBase.New(exception)
            Me.ExitApplication = exitApplication
        End Sub

        ''' <summary>
        ''' Indicates whether the application should exit upon exiting the exception handler
        ''' </summary>
        Public Property ExitApplication() As Boolean
    End Class

    ''' <summary>
    ''' Provides context for the Startup event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced), System.Runtime.InteropServices.ComVisible(False)>
    Public Class StartupEventArgs : Inherits CancelEventArgs
        ''' <summary>
        ''' Creates a new instance of the StartupEventArgs.
        ''' </summary>
        Public Sub New(ByVal args As ReadOnlyCollection(Of String))
            If args Is Nothing Then
                args = New ReadOnlyCollection(Of String)(Nothing)
            End If

            CommandLine = args
        End Sub

        ''' <summary>
        ''' Returns the command line sent to this application
        ''' </summary>
        Public ReadOnly Property CommandLine() As ReadOnlyCollection(Of String)
    End Class

    ''' <summary>
    ''' Provides context for the StartupNextInstance event.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Class StartupNextInstanceEventArgs : Inherits EventArgs

        ''' <summary>
        ''' Creates a new instance of the StartupNextInstanceEventArgs.
        ''' </summary>
        Public Sub New(ByVal args As ReadOnlyCollection(Of String), ByVal bringToForegroundFlag As Boolean)
            If args Is Nothing Then
                args = New ReadOnlyCollection(Of String)(Nothing)
            End If

            CommandLine = args
            BringToForeground = bringToForegroundFlag
        End Sub

        ''' <summary>
        ''' Indicates whether we will bring the application to the foreground when processing the
        ''' StartupNextInstance event.
        ''' </summary>
        Public Property BringToForeground() As Boolean

        ''' <summary>
        ''' Returns the command line sent to this application
        ''' </summary>
        ''' <remarks>I'm using Me.CommandLine so that it is consistent with my.net and to assure they
        ''' always return the same values</remarks>
        Public ReadOnly Property CommandLine() As ReadOnlyCollection(Of String)
    End Class

    ''' <summary>
    ''' Signature for the Startup event handler
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub StartupEventHandler(ByVal sender As Object, ByVal e As StartupEventArgs)

    ''' <summary>
    ''' Signature for the StartupNextInstance event handler
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub StartupNextInstanceEventHandler(ByVal sender As Object, ByVal e As StartupNextInstanceEventArgs)

    ''' <summary>
    ''' Signature for the Shutdown event handler
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub ShutdownEventHandler(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' Signature for the UnhandledException event handler
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub UnhandledExceptionEventHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)

    ''' <summary>
    ''' Exception for when the WinForms VB application model isn't supplied with a startup form
    ''' </summary>
    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
    <System.Serializable()>
    Public Class NoStartupFormException : Inherits System.Exception

        ''' <summary>
        '''  Creates a new exception
        ''' </summary>
        Public Sub New()
            MyBase.New(GetResourceString(SR.AppModel_NoStartupForm))
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As System.Exception)
            MyBase.New(message, inner)
        End Sub

        ' Deserialization constructor must be defined since we are serializable
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>
        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

    ''' <summary>
    ''' Provides the infrastructure for the VB Windows Forms application model
    ''' </summary>
    ''' <remarks>Don't put access on this definition.</remarks>
    Public Class WindowsFormsApplicationBase : Inherits ConsoleApplicationBase

        Public Event Startup As StartupEventHandler
        Public Event StartupNextInstance As StartupNextInstanceEventHandler
        Public Event Shutdown As ShutdownEventHandler

        Public Custom Event NetworkAvailabilityChanged As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler
            'This is a custom event because we want to hook up the NetworkAvailabilityChanged event only if the user writes a handler for it.
            'The reason being that it is very expensive to handle and kills our application startup perf.
            AddHandler(ByVal value As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler)
                SyncLock m_NetworkAvailChangeLock
                    If m_NetworkAvailabilityEventHandlers Is Nothing Then m_NetworkAvailabilityEventHandlers = New System.Collections.ArrayList
                    m_NetworkAvailabilityEventHandlers.Add(value)
                    m_TurnOnNetworkListener = True 'We don't want to create the network object now - it takes a snapshot of the executionContext and our IPrincipal isn't on the thread yet.  We know we need to create it and we will at the appropriate time
                    If m_NetworkObject Is Nothing And m_FinishedOnInitilaize = True Then 'But the user may be doing an Addhandler of their own in which case we need to make sure to honor the request.  If we aren't past OnInitialize() yet we shouldn't do it but the flag above catches that case
                        m_NetworkObject = New Microsoft.VisualBasic.Devices.Network
                        AddHandler m_NetworkObject.NetworkAvailabilityChanged, AddressOf Me.NetworkAvailableEventAdaptor
                    End If
                End SyncLock
            End AddHandler

            RemoveHandler(ByVal value As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler)
                If m_NetworkAvailabilityEventHandlers IsNot Nothing AndAlso m_NetworkAvailabilityEventHandlers.Count > 0 Then
                    m_NetworkAvailabilityEventHandlers.Remove(value)
                    'Last one to leave, turn out the lights...
                    If m_NetworkAvailabilityEventHandlers.Count = 0 Then
                        RemoveHandler m_NetworkObject.NetworkAvailabilityChanged, AddressOf Me.NetworkAvailableEventAdaptor
                        If m_NetworkObject IsNot Nothing Then
                            m_NetworkObject.DisconnectListener() 'Stop listening to network change events because we are going to go away
                            m_NetworkObject = Nothing 'no sense hanging on to this if nobody is listening.
                        End If
                    End If
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventArgs)
                If m_NetworkAvailabilityEventHandlers IsNot Nothing Then
                    For Each handler As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler In m_NetworkAvailabilityEventHandlers
                        Try
                            If handler IsNot Nothing Then handler.Invoke(sender, e)
                        Catch ex As Exception
                            If Not OnUnhandledException(New UnhandledExceptionEventArgs(True, ex)) Then
                                Throw 'the user didn't write a handler so throw the error up the chain
                            End If
                        End Try
                    Next
                End If
            End RaiseEvent
        End Event

        Public Custom Event UnhandledException As UnhandledExceptionEventHandler
            'This is a custom event because we want to hook up System.Windows.Forms.Application.ThreadException only if the user writes a
            'handler for this event.  We only want to hook the ThreadException event if the user is handling this event because the act of listening to
            'Application.ThreadException causes WinForms to snuff exceptions and we only want WinForms to do that if we are assured that the user wrote their own handler
            'to deal with the error instead
            AddHandler(ByVal value As UnhandledExceptionEventHandler)
                If m_UnhandledExceptionHandlers Is Nothing Then m_UnhandledExceptionHandlers = New System.Collections.ArrayList
                m_UnhandledExceptionHandlers.Add(value)
                'Only add the listener once so we don't fire the UnHandledException event over and over for the same exception
                If m_UnhandledExceptionHandlers.Count = 1 Then AddHandler System.Windows.Forms.Application.ThreadException, AddressOf Me.OnUnhandledExceptionEventAdaptor
            End AddHandler

            RemoveHandler(ByVal value As UnhandledExceptionEventHandler)
                If m_UnhandledExceptionHandlers IsNot Nothing AndAlso m_UnhandledExceptionHandlers.Count > 0 Then
                    m_UnhandledExceptionHandlers.Remove(value)
                    'Last one to leave, turn out the lights...
                    If m_UnhandledExceptionHandlers.Count = 0 Then RemoveHandler System.Windows.Forms.Application.ThreadException, AddressOf Me.OnUnhandledExceptionEventAdaptor
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
                If m_UnhandledExceptionHandlers IsNot Nothing Then
                    m_ProcessingUnhandledExceptionEvent = True 'In the case that we throw from the unhandled exception handler, we don't want to run the unhandled exception handler again
                    For Each handler As UnhandledExceptionEventHandler In m_UnhandledExceptionHandlers
                        If handler IsNot Nothing Then handler.Invoke(sender, e)
                    Next
                    m_ProcessingUnhandledExceptionEvent = False 'Now that we are out of the unhandled exception handler, treat exceptions normally again.
                End If
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Constructs the application Shutdown/Startup model object
        ''' </summary>
        ''' <remarks>
        ''' We have to have a parameterless ctor because the platform specific Application object
        ''' derives from this one and it doesn't define a ctor because the partial class generated by the
        ''' designer does that to configure the application. </remarks>
        Public Sub New()
            Me.New(AuthenticationMode.Windows)
        End Sub

        ''' <summary>
        ''' Constructs the application Shutdown/Startup model object
        ''' </summary>
        <SecuritySafeCritical()>
        Public Sub New(ByVal authenticationMode As AuthenticationMode)
            MyBase.New()
            m_Ok2CloseSplashScreen = True 'Default to true in case there is no splash screen so we won't block forever waiting for it to appear.
            ValidateAuthenticationModeEnumValue(authenticationMode, "authenticationMode")

            'Setup Windows Authentication if that's what the user wanted.  Note, we want to do this now, before the Network object gets created because
            'the network object will be doing a AsyncOperationsManager.CreateOperation() which captures the execution context.  So we have to have our
            'principal on the thread before that happens.
            If authenticationMode = AuthenticationMode.Windows Then
                Try
                    'Consider:  - sadly, a call to: System.Security.SecurityManager.IsGranted(New SecurityPermission(SecurityPermissionFlag.ControlPrincipal))
                    'will only check THIS caller so you'll always get TRUE.  What is needed is a way to get to the value of this on a demand basis.  So I try/catch instead for now
                    'but would rather be able to IF my way around this block.
                    System.Threading.Thread.CurrentPrincipal = New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent)
                Catch ex As System.Security.SecurityException
                End Try
            End If

            m_AppContext = New WinFormsAppContext(Me)

            'We need to set the WindowsFormsSynchronizationContext because the network object is going to get created after this ctor runs
            '(network gets created during event hookup) and we need the context in place for it to latch on to.  The WindowsFormsSynchronizationContext
            'won't otherwise get created until OnCreateMainForm() when the startup form is created and by then it is too late.
            'When the startup form gets created, WinForms is going to push our context into the previous context and then restore it when Application.Run() exits.
            Call New System.Security.Permissions.UIPermission(UIPermissionWindow.AllWindows).Assert()
            m_AppSyncronizationContext = AsyncOperationManager.SynchronizationContext
            AsyncOperationManager.SynchronizationContext = New System.Windows.Forms.WindowsFormsSynchronizationContext()
            System.Security.PermissionSet.RevertAssert() 'CLR also reverts if we throw or when we return from this function
        End Sub

        ''' <summary>
        ''' Entry point to kick off the VB Startup/Shutdown Application model
        ''' </summary>
        ''' <param name="commandLine">The command line from Main()</param>
        <SecuritySafeCritical()>
        Public Sub Run(ByVal commandLine As String())
            'Prime the command line args with what we receive from Main() so that Click-Once windows apps don't have to do a System.Environment call which would require permissions.
            MyBase.InternalCommandLine = New System.Collections.ObjectModel.ReadOnlyCollection(Of String)(commandLine)

            'Is this a single-instance application?
            If Not Me.IsSingleInstance Then
                DoApplicationModel() 'This isn't a Single-Instance application
            Else 'This is a Single-Instance application
                Dim ApplicationInstanceID As String = GetApplicationInstanceID(Assembly.GetCallingAssembly) 'Note: Must pass the calling assembly from here so we can get the running app.  Otherwise, can break single instance
                m_NamedPipeID = ApplicationInstanceID & "NamedPipe"
                _semaphoreID = ApplicationInstanceID & "Semaphore"
                ' If are the first instance then we start the named pipe server listening and allow the form to load
                If FirstInstance() Then
                    ' Create a new pipe - it will return immediately and async wait for connections
                    NamedPipeServerCreateServer()
                    Try
                        DoApplicationModel()
                    Finally
                        If _mutexSingleInstance IsNot Nothing Then
                            _mutexSingleInstance.Dispose()
                        End If
                        If _namedPipeServerStream IsNot Nothing AndAlso _namedPipeServerStream.IsConnected Then
                            _namedPipeServerStream.Close()
                        End If
                    End Try
                Else
                    ' We are not the first instance, send the named pipe message with our payload and stop loading
                    Dim _NamedPipeXmlData As New NamedPipeXMLData With
                        {
                        .CommandLineArguments = Environment.GetCommandLineArgs().ToList()
                        }
                    ' Notify first instance by sending args
                    NamedPipeClientSendOptions(_NamedPipeXmlData)
                    RaiseEvent Shutdown(Me, EventArgs.Empty)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Returns the collection of forms that are open.  We no longer have thread
        ''' affinity meaning that this is the WinForms collection that contains Forms that may
        ''' have been opened on another thread then the one we are calling in on right now.
        ''' </summary>
        Public ReadOnly Property OpenForms() As System.Windows.Forms.FormCollection
            Get
                Return System.Windows.Forms.Application.OpenForms
            End Get
        End Property

        ''' <summary>
        ''' Provides access to the main form for this application
        ''' </summary>
        Protected Property MainForm() As System.Windows.Forms.Form
            Get
                Return m_AppContext?.MainForm
            End Get
            Set(ByVal value As System.Windows.Forms.Form)
                If value Is Nothing Then
                    Throw ExceptionUtils.GetArgumentNullException("MainForm", SR.General_PropertyNothing, "MainForm")
                End If
                If value Is m_SplashScreen Then
                    Throw New ArgumentException(GetResourceString(SR.AppModel_SplashAndMainFormTheSame))
                End If
                m_AppContext.MainForm = value
            End Set
        End Property

        ''' <summary>
        ''' Provides access to the splash screen for this application
        ''' </summary>
        Public Property SplashScreen() As System.Windows.Forms.Form
            Get
                Return m_SplashScreen
            End Get
            Set(ByVal value As System.Windows.Forms.Form)
                If value IsNot Nothing AndAlso value Is m_AppContext.MainForm Then 'allow for the case where they set splash screen = nothing and mainForm is currently nothing
                    Throw New ArgumentException(GetResourceString(SR.AppModel_SplashAndMainFormTheSame))
                End If
                m_SplashScreen = value
            End Set
        End Property

        ''' <summary>
        ''' The splash screen timeout specifies whether there is a minimum time that the splash
        ''' screen should be displayed for.  When not set then the splash screen is hidden
        ''' as soon as the main form becomes active.
        ''' </summary>
        ''' <value>The minimum amount of time, in milliseconds, to display the splash screen.</value>
        Public Property MinimumSplashScreenDisplayTime() As Integer
            Get
                Return m_MinimumSplashExposure
            End Get
            Set(ByVal value As Integer)
                m_MinimumSplashExposure = value
            End Set
        End Property

        ''' <summary>
        ''' Use GDI for the text rendering engine by default.
        ''' The user can shadow this function to return True if they want their app
        ''' to use the GDI+ render.  We read this function in Main() (My template) to
        ''' determine how to set the text rendering flag on the WinForms application object.
        ''' </summary>
        ''' <returns>True - Use GDI+ renderer.  False - use GDI renderer</returns>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Shared ReadOnly Property UseCompatibleTextRendering() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Provides the WinForms application context that we are running on
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public ReadOnly Property ApplicationContext() As System.Windows.Forms.ApplicationContext
            Get
                Return m_AppContext
            End Get
        End Property

        ''' <summary>
        ''' Informs My.Settings whether to save the settings on exit or not
        ''' </summary>
        Public Property SaveMySettingsOnExit() As Boolean
            Get
                Return m_SaveMySettingsOnExit
            End Get
            Set(ByVal value As Boolean)
                m_SaveMySettingsOnExit = value
            End Set
        End Property

        ''' <summary>
        '''  Processes all windows messages currently in the message queue
        ''' </summary>
        Public Sub DoEvents()
            System.Windows.Forms.Application.DoEvents()
        End Sub

        ''' <summary>
        ''' This exposes the first in a series of extensibility points for the Startup process.  By default, it shows
        ''' the splash screen and does rudimentary processing of the command line to see if /nosplash or its
        ''' variants was passed in.
        ''' </summary>
        ''' <param name="commandLineArgs"></param>
        ''' <returns>Returning True indicates that we should continue on with the application Startup sequence</returns>
        ''' <remarks>This extensibility point is exposed for people who want to override the Startup sequence at the earliest possible point
        ''' to </remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced), Global.System.STAThread()>
        Protected Overridable Function OnInitialize(ByVal commandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String)) As Boolean
            ' EnableVisualStyles
            If m_EnableVisualStyles Then
                System.Windows.Forms.Application.EnableVisualStyles()
            End If

            'We'll handle /nosplash for you
            If Not (commandLineArgs.Contains("/nosplash") OrElse Me.CommandLineArgs.Contains("-nosplash")) Then
                ShowSplashScreen()
            End If

            m_FinishedOnInitilaize = True 'we are now at a point where we can allow the network object to be created since the iprincipal is on the thread by now.
            Return True 'true means to not bail out but keep on running after OnIntiailize() finishes
        End Function

        ''' <summary>
        ''' Extensibility point which raises the Startup event
        ''' </summary>
        ''' <param name="eventArgs"></param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Function OnStartup(ByVal eventArgs As StartupEventArgs) As Boolean
            eventArgs.Cancel = False
            'It is important not to create the network object until the ExecutionContext has everything on it.  By now the principal will be on the thread so
            'we can create the network object.  The timing is important because the network object has an AsyncOperationsManager in it that marshals
            'the network changed event to the main thread.  The asycnOperationsManager does a CreateOperation() which makes a copy of the executionContext
            'That execution context shows up on your thread during the callback so I delay creating the network object (and consequently the capturing of the
            'execution context) until the principal has been set on the thread.
            'this avoid the problem where My.User isn't set during the NetworkAvailabilityChanged event.  This problem would just extend
            'itself to any future callback that involved the asyncOperationsManager so this is where we need to create objects that have a asyncOperationsContext
            'in them.
            If m_TurnOnNetworkListener = True And m_NetworkObject Is Nothing Then 'the is nothing check is to avoid hooking the object more than once
                m_NetworkObject = New Microsoft.VisualBasic.Devices.Network
                AddHandler m_NetworkObject.NetworkAvailabilityChanged, AddressOf Me.NetworkAvailableEventAdaptor
            End If
            RaiseEvent Startup(Me, eventArgs)
            Return Not eventArgs.Cancel
        End Function

        ''' <summary>
        ''' Extensibility point which raises the StartupNextInstance
        ''' </summary>
        ''' <param name="eventArgs"></param>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <SecuritySafeCritical()>
        Protected Overridable Sub OnStartupNextInstance(ByVal eventArgs As StartupNextInstanceEventArgs)
            RaiseEvent StartupNextInstance(Me, eventArgs)
            'Activate the original instance
            Call New UIPermission(UIPermissionWindow.SafeSubWindows Or UIPermissionWindow.SafeTopLevelWindows).Assert()
            If eventArgs.BringToForeground = True AndAlso Me.MainForm IsNot Nothing Then
                If MainForm.WindowState = System.Windows.Forms.FormWindowState.Minimized Then
                    MainForm.Invoke(Sub()
                                        MainForm.WindowState = Windows.Forms.FormWindowState.Normal
                                    End Sub)
                End If
                MainForm.Invoke(Sub()
                                    MainForm.Activate()
                                End Sub)

            End If
        End Sub

        ''' <summary>
        ''' At this point, the command line args should have been processed and the application will create the
        ''' main form and enter the message loop.
        ''' </summary>
        <SecuritySafeCritical()>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnRun()
            If Me.MainForm Is Nothing Then
                OnCreateMainForm() 'A designer overrides OnCreateMainForm() to set the main form we are supposed to use
                If Me.MainForm Is Nothing Then Throw New NoStartupFormException

                'When we have a splash screen that hasn't timed out before the main form is ready to paint, we want to
                'block the main form from painting.  To do that I let the form get past the Load() event and hold it until
                'the splash screen goes down.  Then I let the main form continue it's startup sequence.  The ordering of
                'Form startup events for reference is: Ctor(), Load Event, Layout event, Shown event, Activated event, Paint event
                AddHandler Me.MainForm.Load, AddressOf MainFormLoadingDone
            End If

            'Run() eats all exceptions (unless running under the debugger) If the user wrote an UnhandledException handler we will hook
            'the System.Windows.Forms.Application.ThreadException event (see Public Custom Event UnhandledException) which will raise our
            'UnhandledException Event.  If our user didn't write an UnhandledException event, then we land in the try/catch handler for Forms.Application.Run()
            Try
                System.Windows.Forms.Application.Run(m_AppContext)
            Finally
                'When Run() returns, the context we pushed in our ctor (which was a WindowsFormsSynchronizationContext) is restored.  But we are going to dispose it
                'so we need to disconnect the network listener so that it can't fire any events in response to changing network availability conditions through a dead context.  VSWHIDBEY #343374
                If m_NetworkObject IsNot Nothing Then m_NetworkObject.DisconnectListener()

                AsyncOperationManager.SynchronizationContext = m_AppSyncronizationContext 'Restore the prior sync context
                m_AppSyncronizationContext = Nothing
            End Try
        End Sub

        ''' <summary>
        ''' A designer will override this method and provide a splash screen if this application has one.
        ''' </summary>
        ''' <remarks>For instance, a designer would override this method and emit: Me.Splash = new Splash
        ''' where Splash was designated in the application designer as being the splash screen for this app</remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnCreateSplashScreen()
        End Sub

        ''' <summary>
        ''' Provides a hook that designers will override to set the main form.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnCreateMainForm()
        End Sub

        ''' <summary>
        ''' The last in a series of extensibility points for the Shutdown process
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Sub OnShutdown()
            If _namedPipeServerStream IsNot Nothing Then
                _namedPipeServerStream.Dispose()
            End If

            If _mutexSingleInstance IsNot Nothing Then
                _mutexSingleInstance.Dispose()
            End If

            RaiseEvent Shutdown(Me, System.EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Raises the UnHandled exception event and exits the application if the event handler indicated
        ''' that execution shouldn't continue
        ''' </summary>
        ''' <param name="e"></param>
        ''' <returns>True indicates the exception event was raised / False it was not</returns>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Overridable Function OnUnhandledException(ByVal e As UnhandledExceptionEventArgs) As Boolean
            If m_UnhandledExceptionHandlers IsNot Nothing AndAlso m_UnhandledExceptionHandlers.Count > 0 Then 'Does the user have a handler for this event?
                'We don't put a try/catch around the handler event so that exceptions in there will bubble out - else we will have a recursive exception handler
                RaiseEvent UnhandledException(Me, e)
                If e.ExitApplication = True Then System.Windows.Forms.Application.Exit()
                Return True 'User handled the event
            End If
            Return False 'Nobody was listening to the UnhandledException event
        End Function

        ''' <summary>
        ''' Uses the extensibility model to see if there is a splash screen provided for this app and if there is,
        ''' displays it.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Sub ShowSplashScreen()
            If Not m_DidSplashScreen Then
                m_DidSplashScreen = True
                If m_SplashScreen Is Nothing Then
                    OnCreateSplashScreen() 'If the user specified a splash screen, the designer will have overriden this method to set it
                End If
                If m_SplashScreen IsNot Nothing Then
                    'Some splash screens have minimum face time they are supposed to get.  We'll set up a time to let us know when we can take it down.
                    If m_MinimumSplashExposure > 0 Then
                        m_Ok2CloseSplashScreen = False 'Don't close until the timer expires.
                        m_SplashTimer = New System.Timers.Timer(m_MinimumSplashExposure)
                        AddHandler m_SplashTimer.Elapsed, AddressOf MinimumSplashExposureTimeIsUp
                        m_SplashTimer.AutoReset = False
                        'We'll enable it in DisplaySplash() once the splash screen thread gets running
                    Else
                        m_Ok2CloseSplashScreen = True 'No timeout so just close it when then main form comes up
                    End If
                    'Run the splash screen on another thread so we don't starve it for events and painting while the main form gets its act together
                    Dim SplashThread As New System.Threading.Thread(AddressOf DisplaySplash)
                    SplashThread.Start()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Hide the splash screen.  The splash screen was created on another thread
        ''' thread (main thread) than the one it was run on (secondary thread for the
        ''' splash screen so it doesn't block app startup. We need to invoke the close.
        ''' This function gets called from the main thread by the app fx.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <SecuritySafeCritical()>
        Protected Sub HideSplashScreen()
            SyncLock m_SplashLock 'This ultimately wasn't necessary.  I suppose we better keep it for backwards compat
                'Dev10 590587 - we now activate the main form before calling Dispose on the Splash screen. (we're just
                '       swapping the order of the two If blocks). This is to fix the issue where the main form
                '       doesn't come to the front after the Splash screen disappears
                If Me.MainForm IsNot Nothing Then
                    Call New System.Security.Permissions.UIPermission(UIPermissionWindow.AllWindows).Assert()
                    Me.MainForm.Activate()
                    System.Security.PermissionSet.RevertAssert() 'CLR also reverts if we throw or when we return from this function
                End If
                If m_SplashScreen IsNot Nothing AndAlso Not m_SplashScreen.IsDisposed Then
                    Dim TheBigGoodbye As New DisposeDelegate(AddressOf m_SplashScreen.Dispose)
                    m_SplashScreen.Invoke(TheBigGoodbye)
                    m_SplashScreen = Nothing
                End If
            End SyncLock
        End Sub

        ''' <summary>
        '''  Determines when this application will terminate (when the main form goes down, all forms)
        ''' </summary>
        Protected Friend Property ShutdownStyle() As ShutdownMode
            Get
                Return m_ShutdownStyle
            End Get
            Set(ByVal value As ShutdownMode)
                ValidateShutdownModeEnumValue(value, "value")
                m_ShutdownStyle = value
            End Set
        End Property

        ''' <summary>
        ''' Determines whether this application will use the XP Windows styles for windows, controls, etc.
        ''' </summary>
        Protected Property EnableVisualStyles() As Boolean
            Get
                Return m_EnableVisualStyles
            End Get
            Set(ByVal value As Boolean)
                m_EnableVisualStyles = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Protected Property IsSingleInstance() As Boolean
            Get
                Return _IsSingleInstance
            End Get
            Set(value As Boolean)
                If value Then
                    _IsSingleInstance = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Validates that the value being passed as an AuthenticationMode enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateAuthenticationModeEnumValue(ByVal value As AuthenticationMode, ByVal paramName As String)
            If value < ApplicationServices.AuthenticationMode.Windows OrElse value > ApplicationServices.AuthenticationMode.ApplicationDefined Then
                Throw New System.ComponentModel.InvalidEnumArgumentException(paramName, value, GetType(AuthenticationMode))
            End If
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an ShutdownMode enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateShutdownModeEnumValue(ByVal value As ShutdownMode, ByVal paramName As String)
            If value < ShutdownMode.AfterMainFormCloses OrElse value > ShutdownMode.AfterAllFormsClose Then
                Throw New System.ComponentModel.InvalidEnumArgumentException(paramName, value, GetType(ShutdownMode))
            End If
        End Sub

        ''' <summary>
        ''' Displays the splash screen.  We get called here from a different thread than what the
        ''' main form is starting up on.  This allows us to process events for the Splash screen so
        ''' it doesn't freeze up while the main form is getting it together.
        ''' </summary>
        Private Sub DisplaySplash()
            Debug.Assert(m_SplashScreen IsNot Nothing, "We should have never get here if there is no splash screen")
            If m_SplashTimer IsNot Nothing Then 'We only have a timer if there is a minimum time that the splash screen is supposed to be displayed.
                m_SplashTimer.Enabled = True 'enable the timer now that we are about to show the splash screen
            End If
            System.Windows.Forms.Application.Run(m_SplashScreen)
        End Sub

        ''' <summary>
        ''' If a splash screen has a minimum time out, then once that is up we check to see whether
        ''' we should close the splash screen.  If the main form has activated then we close it.
        ''' Note that we are getting called on a secondary thread here which isn't necessairly
        ''' associated with any form.  Don't touch forms from this function.
        ''' </summary>
        Private Sub MinimumSplashExposureTimeIsUp(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
            If m_SplashTimer IsNot Nothing Then 'We only have a timer if there was a minimum timeout on the splash screen
                m_SplashTimer.Dispose()
                m_SplashTimer = Nothing
            End If
            m_Ok2CloseSplashScreen = True
        End Sub

        ''' <summary>
        ''' The Load() event happens before the Shown and Paint events.  When we get called here
        ''' we know that the form load event is done and that the form is about to paint
        ''' itself for the first time.
        ''' We can now hide the splash screen.
        ''' Note that this function gets called from the main thread - the same thread
        ''' that creates the startup form.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub MainFormLoadingDone(ByVal sender As Object, ByVal e As System.EventArgs)
            RemoveHandler Me.MainForm.Load, AddressOf MainFormLoadingDone 'We don't want this event to call us again.

            'block until the splash screen time is up.  See MinimumSplashExposureTimeIsUp() which releases us
            While Not m_Ok2CloseSplashScreen
                DoEvents() 'In case Load() event, which we are waiting for, is doing stuff that requires windows messages.  our timer message doesn't count because it is on another thread.
            End While

            HideSplashScreen()
        End Sub

        ''' <summary>
        ''' Encapsulates an ApplicationContext.  We have our own to get the shutdown behaviors we
        ''' offer in the application model.  This derivation of the ApplicationContext listens for when
        ''' the main form closes and provides for shutting down when the main form closes or the
        ''' last form closes, depending on the mode this application is running in.
        ''' </summary>
        Private Class WinFormsAppContext : Inherits System.Windows.Forms.ApplicationContext
            Sub New(ByVal App As WindowsFormsApplicationBase)
                m_App = App
            End Sub

            ''' <summary>
            ''' Handles the two types of application shutdown:
            '''   1 - shutdown when the main form closes
            '''   2 - shutdown only after the last form closes
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            <SecuritySafeCritical()>
            Protected Overrides Sub OnMainFormClosed(ByVal sender As Object, ByVal e As System.EventArgs)
                If m_App.ShutdownStyle = ShutdownMode.AfterMainFormCloses Then
                    MyBase.OnMainFormClosed(sender, e)
                Else 'identify a new main form so we can keep running
                    Call New System.Security.Permissions.UIPermission(UIPermissionWindow.AllWindows).Assert()
                    Dim forms As System.Windows.Forms.FormCollection = System.Windows.Forms.Application.OpenForms
                    System.Security.PermissionSet.RevertAssert() 'CLR also reverts if we throw or when we return from this function.
                    If forms.Count > 0 Then
                        'Note: Initially I used Process::MainWindowHandle to obtain an open form.  But that is bad for two reasons:
                        '1 - It appears to be broken and returns NULL sometimes even when there is still a window around.  WinForms people are looking at that issue.
                        '2 - It returns the first window it hits from enum thread windows, which is not necessarily a windows forms form, so that doesn't help us even if it did work
                        'all the time.  So I'll use one of our open forms.  We may not necessairily get a visible form here but that's ok.  Some apps may run on an invisible window
                        'and we need to keep them going until all windows close.
                        Me.MainForm = forms(0)
                    Else
                        MyBase.OnMainFormClosed(sender, e)
                    End If
                End If
            End Sub

            Private m_App As WindowsFormsApplicationBase
        End Class 'WinFormsAppContext

        ''' <summary>
        ''' Handles the Windows.Forms.Application.ThreadException event and raises our Unhandled
        ''' exception event
        ''' </summary>
        ''' <param name="e"></param>
        ''' <remarks>Our UnHandledException event has a different signature then the Windows.Forms.Application
        ''' unhandled exception event so we do the translation here before raising our event.
        ''' </remarks>
        Private Sub OnUnhandledExceptionEventAdaptor(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
            OnUnhandledException(New Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs(True, e.Exception))
        End Sub

        ''' <summary>
        ''' Handles the Network.NetworkAvailability event (on the correct thread) and raises the
        ''' NetworkAvailabilityChanged event
        ''' </summary>
        ''' <param name="Sender">Contains the Network instance that raised the event</param>
        ''' <param name="e">Contains whether the network is available or not</param>
        Private Sub NetworkAvailableEventAdaptor(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.Devices.NetworkAvailableEventArgs)
            RaiseEvent NetworkAvailabilityChanged(sender, e)
        End Sub

        Private m_UnhandledExceptionHandlers As System.Collections.ArrayList
        Private m_ProcessingUnhandledExceptionEvent As Boolean
        Private m_TurnOnNetworkListener As Boolean 'Tracks whether we need to create the network object so we can listen to the NetworkAvailabilityChanged event
        Private m_FinishedOnInitilaize As Boolean 'Whether we have made it through the processing of OnInitialize
        Private m_NetworkAvailabilityEventHandlers As System.Collections.ArrayList
        Private m_NetworkObject As Microsoft.VisualBasic.Devices.Network
        Private m_ShutdownStyle As ShutdownMode 'defines when the application decides to close
        Private m_EnableVisualStyles As Boolean 'whether to use Windows XP styles
        Private m_DidSplashScreen As Boolean 'we only need to show the splash screen once.  Protect the user from himself if they are overriding our app model.
        Private Delegate Sub DisposeDelegate() 'used to marshal a call to Dispose on the Splash Screen
        Private m_Ok2CloseSplashScreen As Boolean 'For splash screens with a minimum display time, this let's us know when that time has expired and it is ok to close the splash screen.
        Private m_SplashScreen As System.Windows.Forms.Form
        Private m_MinimumSplashExposure As Integer = 2000 'Minimum amount of time to show the splash screen.  0 means hide as soon as the app comes up.
        Private m_SplashTimer As Timers.Timer
        Private m_SplashLock As New Object
        Private m_AppContext As WinFormsAppContext
        Private m_AppSyncronizationContext As SynchronizationContext
        Private m_NetworkAvailChangeLock As New Object 'sync object
        Private m_SaveMySettingsOnExit As Boolean 'Informs My.Settings whether to save the settings on exit or not

        Private _firstInstance As Boolean
        Private _mutexSingleInstance As Mutex
        Private m_NamedPipeID As String 'global OS handles must have a unique ID
        Private _namedPipeServerStream As NamedPipeServerStream
        Private _namedPipeXmlData As NamedPipeXMLData
        Private _semaphoreID As String
        Private _IsSingleInstance As Boolean
        Private ReadOnly _namedPiperServerThreadLock As New Object

        ''' <summary>
        ''' Runs the user's program through the VB Startup/Shutdown application model
        ''' </summary>
        Private Sub DoApplicationModel()

            Dim EventArgs As New StartupEventArgs(MyBase.CommandLineArgs)

            'Only do the try/catch if we aren't running under the debugger.  If we do try/catch under the debugger the debugger never gets a crack at exceptions which breaks the exception helper
            If Not System.Diagnostics.Debugger.IsAttached Then
                'NO DEBUGGER ATTACHED - we use a catch so that we can run our UnhandledException code
                'Note - Sadly, code changes within this IF (that don't pertain to exception handling) need to be mirrored in the ELSE debugger attached clause below
                Try
                    If OnInitialize(MyBase.CommandLineArgs) Then
                        If OnStartup(EventArgs) = True Then
                            OnRun()
                            OnShutdown()
                        End If
                    End If
                Catch ex As System.Exception
                    'This catch is for exceptions that happen during the On* methods above, but have occurred outside of the message pump (which exceptions we would
                    'have already seen via our hook of System.Windows.Forms.Application.ThreadException)
                    If m_ProcessingUnhandledExceptionEvent Then
                        Throw 'If the UnhandledException handler threw for some reason, throw that error out to the system.
                    Else 'We had an exception, but not during the OnUnhandledException handler so give the user a chance to look at what happened in the UnhandledException event handler
                        If Not OnUnhandledException(New UnhandledExceptionEventArgs(True, ex)) = True Then
                            Throw 'the user didn't write a handler so throw the error out to the system
                        End If
                    End If
                End Try
            Else 'DEBUGGER ATTACHED - we don't have an uber catch when debugging so the exception will bubble out to the exception helper
                'We also don't hook up the Application.ThreadException event because WinForms ignores it when we are running under the debugger
                If OnInitialize(MyBase.CommandLineArgs) Then
                    If OnStartup(EventArgs) = True Then
                        OnRun()
                        OnShutdown()
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Generates the name for the remote singleton that we use to channel multiple instances
        ''' to the same application model thread.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <SecurityCritical()>
        Private Function GetApplicationInstanceID(ByVal Entry As Assembly) As String
            'CONSIDER: We may want to make this public so users can set up what single instance means to them, e.g. for us, separate paths mean different instances, etc.

            Dim Permissions As New System.Security.PermissionSet(PermissionState.None)
            Permissions.AddPermission(New FileIOPermission(PermissionState.Unrestricted)) 'Chicken and egg problem.  All I need is PathDiscovery for the location of this assembly but to get the location of the assembly (see Getname below) I need to know the path which I can't get without asserting...
            Permissions.AddPermission(New SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode))
            Permissions.Assert()

            Dim Guid As Guid = GetTypeLibGuidForAssembly(Entry)
            If Guid = Nothing Then
                Return Entry.ManifestModule.ModuleVersionId.ToString
            End If
            Dim Version As String = Entry.GetName.Version.ToString
            Dim VersionParts As String() = Version.Split(CType(".", Char()))
            Dim SemiUniqueApplicationID As String = Guid.ToString + VersionParts(0) + "." + VersionParts(1)
            PermissionSet.RevertAssert()

            'Note: We used to make the terminal server session ID part of the key.  It turns out to be unnecessary and the call to
            'NativeMethods.ProcessIdToSessionId(System.Diagnostics.Process.GetCurrentProcess.Id, TerminalSessionID) was not supported on Win98, anyway.
            'It turns out that terminal server sessions, even when you are logged in as the same user to multiple terminal server sessions on the same
            'machine, are separate.  So you can have session 1 running as  and have a global system object named "FOO" that won't conflict with
            'any other global system object named "FOO" whether it be in session 2 running as  or session n running as whoever.
            'So it isn't necessary to make the session id part of the unique name that identifies a

            Return SemiUniqueApplicationID  'Re: version parts, we have the major, minor, build, revision.  We key off major+minor.
        End Function

        Private Function GetTypeLibGuidForAssembly(_assembly As Assembly) As Guid
            Dim CustomAttributes As Object() = _assembly.GetCustomAttributes(GetType(GuidAttribute), True)
            If CustomAttributes.Any Then
                Dim attribute As GuidAttribute = CType(CustomAttributes(0), GuidAttribute)
                Return New Guid(attribute.Value)
            End If
            Return Nothing
        End Function

        ''' <summary>
        '''     Are we the first instance of this application.
        ''' </summary>
        ''' <returns></returns>
        Private Function FirstInstance() As Boolean
            ' Allow for multiple runs but only try and get the mutex once
            If _mutexSingleInstance Is Nothing Then
                _mutexSingleInstance = New Mutex(True, _semaphoreID, _firstInstance)
            End If

            Return _firstInstance
        End Function

        ''' <summary>
        '''     Uses a named pipe to send the currently parsed options to an already running instance.
        ''' </summary>
        ''' <param name="namedPipePayload"></param>
        Private Sub NamedPipeClientSendOptions(namedPipePayload As NamedPipeXMLData)
            Try
                Using _namedPipeClientStream As New NamedPipeClientStream(".", m_NamedPipeID, PipeDirection.Out)
                    _namedPipeClientStream.Connect(3000)
                    ' Maximum wait 3 seconds

                    Dim _xmlSerializer As New XmlSerializer(GetType(NamedPipeXMLData))
                    _xmlSerializer.Serialize(_namedPipeClientStream, namedPipePayload)
                End Using
            Catch ex As Exception
                ' Error connecting or sending
            End Try
        End Sub

        ''' <summary>
        '''     The function called when a client connects to the named pipe. Note: This method is called on a non-UI thread.
        ''' </summary>
        ''' <param name="_iAsyncResult"></param>
        Private Sub NamedPipeServerConnectionCallback(_iAsyncResult As IAsyncResult)
            Try
                ' End waiting for the connection
                _namedPipeServerStream.EndWaitForConnection(_iAsyncResult)

                ' Read data and prevent access to _NamedPipeXmlData during threaded operations
                SyncLock _namedPiperServerThreadLock
                    Dim _xmlSerializer As New XmlSerializer(GetType(NamedPipeXMLData))
                    _namedPipeXmlData = CType(_xmlSerializer.Deserialize(_namedPipeServerStream), NamedPipeXMLData)
                    Dim remoteEventArgs As New StartupNextInstanceEventArgs(New ReadOnlyCollection(Of String)(_namedPipeXmlData.CommandLineArguments), bringToForegroundFlag:=True)
                    OnStartupNextInstance(remoteEventArgs)
                End SyncLock
            Catch ex As ObjectDisposedException
                ' EndWaitForConnection will throw exception when someone closes the pipe before connection made
                ' In that case we don't create any more pipes and just return
                ' This will happen when app is closing and our pipe is closed/disposed
                Return
            Catch
                ' ignored
            Finally
                ' Close the original pipe (we will create a new one each time)
                _namedPipeServerStream.Dispose()
                ' ignored
            End Try

            ' Create a new pipe for next connection
            NamedPipeServerCreateServer()
        End Sub

        ''' <summary>
        '''     Starts a new pipe server if one isn't already active.
        ''' </summary>
        Private Sub NamedPipeServerCreateServer()
            ' Create a new pipe accessible by local authenticated users, disallow network
            'var sidAuthUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            Dim sidNetworkService As New SecurityIdentifier(WellKnownSidType.NetworkServiceSid, Nothing)
            Dim sidWorld As New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing)

            Dim _pipeSecurity As New PipeSecurity

            ' Deny network access to the pipe
            Dim accessRule As New PipeAccessRule(sidNetworkService, PipeAccessRights.ReadWrite, AccessControlType.Deny)
            _pipeSecurity.AddAccessRule(accessRule)

            ' Allow Everyone to read/write
            accessRule = New PipeAccessRule(sidWorld, PipeAccessRights.ReadWrite, AccessControlType.Allow)
            _pipeSecurity.AddAccessRule(accessRule)

            ' Current user is the owner
            Dim sidOwner As SecurityIdentifier = WindowsIdentity.GetCurrent().Owner
            If sidOwner IsNot Nothing Then
                accessRule = New PipeAccessRule(sidOwner, PipeAccessRights.FullControl, AccessControlType.Allow)
                _pipeSecurity.AddAccessRule(accessRule)
            End If

            ' Create pipe and start the async connection wait
            _namedPipeServerStream = New NamedPipeServerStream(
                    pipeName:=m_NamedPipeID,
                    direction:=PipeDirection.In,
                    maxNumberOfServerInstances:=1,
                    transmissionMode:=PipeTransmissionMode.Byte,
                    options:=PipeOptions.Asynchronous,
                    inBufferSize:=0,
                    outBufferSize:=0)

            ' Begin async wait for connections
            _namedPipeServerStream.BeginWaitForConnection(AddressOf NamedPipeServerConnectionCallback, _namedPipeServerStream)
        End Sub

    End Class 'WindowsFormsApplicationBase
End Namespace

