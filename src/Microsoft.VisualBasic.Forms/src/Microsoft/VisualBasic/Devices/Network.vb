﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Net
Imports System.Net.Http
Imports System.Security
Imports System.Threading

Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports NetInfoAlias = System.Net.NetworkInformation

Namespace Microsoft.VisualBasic.Devices

    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub NetworkAvailableEventHandler(sender As Object, e As NetworkAvailableEventArgs)

    ''' <summary>
    '''  An object that allows easy access to some simple network properties and functionality.
    ''' </summary>
    Public Class Network

        'Size of Ping.exe buffer
        Private Const BUFFER_SIZE As Integer = 32

        ' Password used in overloads where there is no password parameter
        Private Const DEFAULT_PASSWORD As String = ""

        ' Default timeout for Ping
        Private Const DEFAULT_PING_TIMEOUT As Integer = 1000

        ' Default timeout value
        Private Const DEFAULT_TIMEOUT As Integer = 100000

        ' UserName used in overloads where there is no userName parameter
        Private Const DEFAULT_USERNAME As String = ""

        ' Object for syncing
        Private ReadOnly _syncObject As New Object()

        ' Indicates last known connection state
        Private _connected As Boolean

        'Used for marshalling the network address changed event to the foreground thread
        Private _networkAvailabilityChangedCallback As SendOrPostCallback

        'Holds the listeners to our NetworkAvailability changed event
        Private _networkAvailabilityEventHandlers As List(Of NetworkAvailableEventHandler)

        'Holds the buffer for pinging. We lazy initialize on first use
        Private _pingBuffer() As Byte

        Private _synchronizationContext As SynchronizationContext

        ''' <summary>
        '''  Creates class and hooks up events
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        '''  A buffer for pinging. This imitates the buffer used by Ping.Exe
        ''' </summary>
        ''' <value>A buffer</value>
        Private ReadOnly Property PingBuffer() As Byte()
            Get
                If _pingBuffer Is Nothing Then
                    ReDim _pingBuffer(BUFFER_SIZE - 1)
                    For i As Integer = 0 To BUFFER_SIZE - 1
                        'This is the same logic Ping.exe uses to fill it's buffer
                        _pingBuffer(i) = Convert.ToByte(Asc("a"c) + i Mod 23, Globalization.CultureInfo.InvariantCulture)
                    Next
                End If

                Return _pingBuffer
            End Get
        End Property

        ''' <summary>
        '''  Indicates whether or not the local machine is connected to an IP network.
        ''' </summary>
        ''' <value>True if connected, otherwise False</value>
        Public ReadOnly Property IsAvailable() As Boolean
            Get

                Return NetInfoAlias.NetworkInterface.GetIsNetworkAvailable()

            End Get
        End Property

        ''' <summary>
        '''  Event fired when connected to the network
        ''' </summary>
        ''' <param name="Sender">Has no meaning for this event</param>
        ''' <param name="e">Has no meaning for this event</param>
        Public Custom Event NetworkAvailabilityChanged As NetworkAvailableEventHandler
            'This is a custom event because we want to hook up the NetworkAvailabilityChanged event only if the user writes a handler for it.
            'The reason being that it is very expensive to handle and kills our application startup perf.
            AddHandler(handler As NetworkAvailableEventHandler)

                ' Set the current state of connectedness, swallow known exceptions since user won't be able to correct problem
                Try
                    _connected = IsAvailable
                Catch ex As SecurityException
                    Return
                Catch ex As PlatformNotSupportedException
                    Return
                End Try
                SyncLock _syncObject 'we don't want our event firing before we've finished setting up the infrastructure.  Also, need to assure there are no races in here so we don't hook up the OS listener twice, etc.
                    If _networkAvailabilityEventHandlers Is Nothing Then _networkAvailabilityEventHandlers = New List(Of NetworkAvailableEventHandler)
                    _networkAvailabilityEventHandlers.Add(handler)

                    'Only setup the event Marshalling infrastructure once
                    If _networkAvailabilityEventHandlers.Count = 1 Then
                        _networkAvailabilityChangedCallback = New SendOrPostCallback(AddressOf NetworkAvailabilityChangedHandler) 'the async operation posts to this delegate
                        If AsyncOperationManager.SynchronizationContext IsNot Nothing Then
                            _synchronizationContext = AsyncOperationManager.SynchronizationContext 'We need to hang on to the synchronization context associated with the thread the network object is created on
                            Try ' Exceptions are thrown if the user isn't an admin.
                                AddHandler NetInfoAlias.NetworkChange.NetworkAddressChanged, New NetInfoAlias.NetworkAddressChangedEventHandler(AddressOf OS_NetworkAvailabilityChangedListener) 'listen to the OS event
                            Catch ex As PlatformNotSupportedException
                            Catch ex As NetInfoAlias.NetworkInformationException
                            End Try
                        End If
                    End If
                End SyncLock
            End AddHandler

            RemoveHandler(handler As NetworkAvailableEventHandler)
                If _networkAvailabilityEventHandlers IsNot Nothing AndAlso _networkAvailabilityEventHandlers.Count > 0 Then
                    _networkAvailabilityEventHandlers.Remove(handler)
                    'Last one to leave, turn out the lights...
                    If _networkAvailabilityEventHandlers.Count = 0 Then
                        RemoveHandler NetInfoAlias.NetworkChange.NetworkAddressChanged, New NetInfoAlias.NetworkAddressChangedEventHandler(AddressOf OS_NetworkAvailabilityChangedListener) 'listen to the OS event
                        DisconnectListener() 'Stop listening to network change events since nobody is listening to us anymore
                    End If
                End If
            End RemoveHandler

            RaiseEvent(sender As Object, e As NetworkAvailableEventArgs)
                If _networkAvailabilityEventHandlers IsNot Nothing Then
                    For Each handler As NetworkAvailableEventHandler In _networkAvailabilityEventHandlers
                        If handler IsNot Nothing Then handler.Invoke(sender, e)
                    Next
                End If
            End RaiseEvent
        End Event

        ''' <summary>
        '''  Posts a message to close the progress dialog
        ''' </summary>
        Private Shared Sub CloseProgressDialog(dialog As ProgressDialog)
            ' Don't invoke unless dialog is up and running
            If dialog IsNot Nothing Then
                dialog.IndicateClosing()

                If dialog.IsHandleCreated Then
                    dialog.BeginInvoke(New System.Windows.Forms.MethodInvoker(AddressOf dialog.CloseDialog))
                Else
                    ' Ensure dialog is closed. If we get here it means the file was copied before the handle for
                    ' the progress dialog was created.
                    dialog.Close()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in Uri.
        ''' Maps older networkCredentials to HttpClientHandler
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel"></param>
        ''' <returns></returns>
        Private Shared Function DownloadFileAsync(addressUri As Uri,
                                        destinationFileName As String,
                                        networkCredentials As ICredentials,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean,
                                        onUserCancel As UICancelOption) As Task

            Dim clientHandler = If(networkCredentials Is Nothing,
                                   New HttpClientHandler,
                                   New HttpClientHandler With {.Credentials = networkCredentials}
                                  )
            Return DownloadFileAsync(addressUri, destinationFileName,
                                     clientHandler, dialog, connectionTimeout,
                                     overwrite, onUserCancel)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Private Shared Async Function DownloadFileAsync(address As String,
                                             destinationFileName As String,
                                             userName As String,
                                             password As String,
                                             dialog As ProgressDialog,
                                             connectionTimeout As Integer,
                                             overwrite As Boolean) As Task

            Await DownloadFileAsync(address, destinationFileName, userName, password,
                                    dialog, connectionTimeout, overwrite,
                                    UICancelOption.ThrowException).ConfigureAwait(False)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Private Shared Async Function DownloadFileAsync(address As String,
                                            destinationFileName As String,
                                            userName As String,
                                            password As String,
                                            dialog As ProgressDialog,
                                            connectionTimeout As Integer,
                                            overwrite As Boolean,
                                            onUserCancel As UICancelOption) As Task

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...) vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Await DownloadFileAsync(addressUri,
                                    destinationFileName,
                                    networkCredentials,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel
                                   ).ConfigureAwait(False)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                             destinationFileName As String,
                                             userName As String,
                                             password As String,
                                             dialog As ProgressDialog,
                                             connectionTimeout As Integer,
                                             overwrite As Boolean) As Task

            Await DownloadFileAsync(addressUri,
                                    destinationFileName,
                                    userName,
                                    password,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    UICancelOption.ThrowException).ConfigureAwait(False)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                            destinationFileName As String,
                                            userName As String,
                                            password As String,
                                            dialog As ProgressDialog,
                                            connectionTimeout As Integer,
                                            overwrite As Boolean,
                                            onUserCancel As UICancelOption) As Task

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Await DownloadFileAsync(addressUri,
                                    destinationFileName,
                                    networkCredentials,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel).ConfigureAwait(False)
        End Function

#If False Then ' Here in case DownloadFileAsync becomes public
        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                            destinationFileName As String,
                                            networkCredentials As ICredentials,
                                            dialog As ProgressDialog,
                                            connectionTimeout As Integer,
                                            overwrite As Boolean) As Task

            Await DownloadFileAsync(addressUri, destinationFileName, networkCredentials,
                                    dialog, connectionTimeout, overwrite,
                                    UICancelOption.ThrowException).ConfigureAwait(False)
        End Function
#End If

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="clientHandler">An HttpClientHandler of the user performing the download</param>
        ''' <param name="dialog">Progress Dialog</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                            destinationFileName As String,
                                            clientHandler As HttpClientHandler,
                                            dialog As ProgressDialog,
                                            connectionTimeout As Integer,
                                            overwrite As Boolean,
                                            onUserCancel As UICancelOption) As Task
            If connectionTimeout <= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If addressUri Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(addressUri))
            End If

            Dim client = If(clientHandler IsNot Nothing,
                        New HttpClient(clientHandler),
                        New HttpClient()
                       )

            ' Set credentials if we have any
            client.Timeout = New TimeSpan(0, 0, 0, 0, connectionTimeout)

            'Construct the local file. This will validate the full name and path
            Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
            ' Sometime a path that can't be parsed is normalized to the current directory. This makes sure we really
            ' have a file and path
            If IO.Directory.Exists(fullFilename) Then
                Throw ExUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            'Throw if the file exists and the user doesn't want to overwrite
            If Not overwrite AndAlso IO.File.Exists(fullFilename) Then
                Throw New IO.IOException(Utils.GetResourceString(SR.IO_FileExists_Path, destinationFileName))
            End If

            'Check to see if the target directory exists. If it doesn't, create it
            Dim targetDirectory As String = IO.Path.GetDirectoryName(fullFilename)

            ' Make sure we have a meaningful directory. If we don't, the destinationFileName is suspect
            If String.IsNullOrEmpty(targetDirectory) Then
                Throw ExUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            If Not IO.Directory.Exists(targetDirectory) Then
                IO.Directory.CreateDirectory(targetDirectory)
            End If

            'Create the copier
            Dim copier As New HttpClientCopy(client, dialog)

            'Download the file
            Try
                Await copier.DownloadFileAsync(addressUri,
                                               fullFilename).ConfigureAwait(False)
            Catch ex As Exception
                If onUserCancel = UICancelOption.ThrowException OrElse Not dialog.UserCanceledTheDialog Then
                    Throw
                End If
            End Try

        End Function

        ''' <summary>
        ''' Gets network credentials from a userName and password
        ''' </summary>
        ''' <param name="userName">The name of the user</param>
        ''' <param name="password">The password of the user</param>
        ''' <returns>A NetworkCredentials</returns>
        Private Shared Function GetNetworkCredentials(userName As String, password As String) As ICredentials

            Return If(String.IsNullOrWhiteSpace(userName) OrElse String.IsNullOrWhiteSpace(password),
                      Nothing,
                      DirectCast(New NetworkCredential(userName, password), ICredentials)
                     )
        End Function

        ''' <summary>
        '''  Gets a Uri from a uri string. We also use this function to validate the UriString (remote file address)
        ''' </summary>
        ''' <param name="address">The remote file address</param>
        ''' <returns>A Uri if successful, otherwise it throws an exception</returns>
        Private Shared Function GetUri(address As String) As Uri
            Try
                Return New Uri(address)
            Catch ex As UriFormatException
                'Throw an exception with an error message more appropriate to our API
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(address), SR.Network_InvalidUriString, address)
            End Try
        End Function

        'Listens to the AddressChanged event which will come on the same thread that this class was created on (AsyncEventManager is responsible for getting the event here)
        Private Sub NetworkAvailabilityChangedHandler(state As Object)
            Dim Connected As Boolean = IsAvailable
            ' Fire an event only if the connected state has changed
            If _connected <> Connected Then
                _connected = Connected
                RaiseEvent NetworkAvailabilityChanged(Me, New NetworkAvailableEventArgs(Connected))
            End If
        End Sub

        'Listens to the AddressChanged event from the OS which comes in on an arbitrary thread
        Private Sub OS_NetworkAvailabilityChangedListener(sender As Object, e As EventArgs)
            SyncLock _syncObject 'Ensure we don't handle events until after we've finished setting up the event marshalling infrastructure
                'Don't call AsyncOperationManager.OperationSynchronizationContext.Post.  The reason we want to go through m_SynchronizationContext is that
                'the OperationSynchronizationContext is thread static.  Since we are getting called on some random thread, the context that was
                'in place when the Network object was created won't be available (it is on the original thread).  To hang on to the original
                'context associated with the thread that the network object is created on, I use m_SynchronizationContext.
                _synchronizationContext.Post(_networkAvailabilityChangedCallback, Nothing)
            End SyncLock
        End Sub

        Friend Sub DisconnectListener()
            RemoveHandler NetInfoAlias.NetworkChange.NetworkAddressChanged, New NetInfoAlias.NetworkAddressChangedEventHandler(AddressOf OS_NetworkAvailabilityChangedListener)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        Public Sub DownloadFile(address As String, destinationFileName As String)
            Try
                DownloadFileAsync(address, destinationFileName, DEFAULT_USERNAME,
                              DEFAULT_PASSWORD, Nothing,
                              DEFAULT_TIMEOUT,
                              False
                             ).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        Public Sub DownloadFile(address As Uri, destinationFileName As String)
            Try
                DownloadFileAsync(address, destinationFileName, DEFAULT_USERNAME,
                            DEFAULT_PASSWORD, Nothing,
                            DEFAULT_TIMEOUT, False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(address As String, destinationFileName As String, userName As String, password As String)
            Try
                DownloadFileAsync(address, destinationFileName, userName, password,
                            Nothing, DEFAULT_TIMEOUT,
                            False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(address As Uri, destinationFileName As String, userName As String, password As String)
            Try
                DownloadFileAsync(address, destinationFileName, userName, password,
                             Nothing, DEFAULT_TIMEOUT,
                             False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try

        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Public Sub DownloadFile(address As String,
                               destinationFileName As String,
                               userName As String,
                               password As String,
                               showUI As Boolean,
                               connectionTimeout As Integer,
                               overwrite As Boolean)
            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
                    dialog = New ProgressDialog With {
                    .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address),
                    .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address, fullFilename)
                }
                End If

                Dim t As Task = DownloadFileAsync(address,
                                                  destinationFileName,
                                                  userName,
                                                  password,
                                                  dialog, connectionTimeout, overwrite,
                                              UICancelOption.ThrowException
                                             )
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    If TryCast(ex.InnerException, OperationCanceledException) IsNot Nothing AndAlso Environment.UserInteractive Then
                        If showUI AndAlso Environment.UserInteractive Then
                            IO.File.Delete(destinationFileName)
                            Throw New OperationCanceledException()
                        End If
                    End If

                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub DownloadFile(address As String,
                                destinationFileName As String,
                                userName As String,
                                password As String,
                                showUI As Boolean,
                                connectionTimeout As Integer,
                                overwrite As Boolean,
                                onUserCancel As UICancelOption)

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...) vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
                    dialog = New ProgressDialog With {
                .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address),
                .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address, fullFilename)
            }
                End If

                Dim t As Task = DownloadFileAsync(addressUri, destinationFileName,
                                              networkCredentials, dialog, connectionTimeout,
                                              overwrite, onUserCancel
                                             )
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Public Sub DownloadFile(address As Uri,
                                destinationFileName As String,
                                userName As String,
                                password As String,
                                showUI As Boolean,
                                connectionTimeout As Integer,
                                overwrite As Boolean)
            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
                    dialog = New ProgressDialog With {
                .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address.AbsolutePath),
                .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address.AbsolutePath, fullFilename)
            }
                End If
                Dim t As Task = DownloadFileAsync(address,
                                                  destinationFileName,
                                                  userName,
                                                  password,
                                                  dialog,
                                                  connectionTimeout,
                                                  overwrite,
                                              UICancelOption.ThrowException
                                             )
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub DownloadFile(address As Uri,
                                destinationFileName As String,
                                userName As String,
                                password As String,
                                showUI As Boolean,
                                connectionTimeout As Integer,
                                overwrite As Boolean,
                                onUserCancel As UICancelOption)

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
                    dialog = New ProgressDialog With {
                    .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address.AbsolutePath),
                    .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address.AbsolutePath, fullFilename)
                }
                End If

                Dim t As Task = DownloadFileAsync(address,
                                                  destinationFileName,
                                                  networkCredentials,
                                                  dialog,
                                                  connectionTimeout,
                                                  overwrite,
                                                  onUserCancel)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Public Sub DownloadFile(address As Uri,
                                destinationFileName As String,
                                networkCredentials As ICredentials,
                                showUI As Boolean,
                                connectionTimeout As Integer,
                                overwrite As Boolean)

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
                    dialog = New ProgressDialog With {
                    .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address.AbsolutePath),
                    .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address.AbsolutePath, fullFilename)
                }
                End If

                Dim t As Task = DownloadFileAsync(address,
                                                  destinationFileName,
                                                  networkCredentials,
                                                  dialog,
                                                  connectionTimeout,
                                                  overwrite,
                                              UICancelOption.ThrowException
                                             )
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Public Sub DownloadFile(address As Uri,
                                destinationFileName As String,
                                networkCredentials As ICredentials,
                                showUI As Boolean,
                                connectionTimeout As Integer,
                                overwrite As Boolean,
                                onUserCancel As UICancelOption)
            If connectionTimeout <= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")
                    dialog = New ProgressDialog With {
                    .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address.AbsolutePath),
                    .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address.AbsolutePath, fullFilename)
                }
                End If

                Dim t As Task = DownloadFileAsync(address,
                                                  destinationFileName,
                                                  networkCredentials,
                                                  dialog,
                                                  connectionTimeout,
                                                  overwrite,
                                                  onUserCancel
                                                 )
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in address.
        ''' </summary>
        ''' <param name="hostNameOrAddress"></param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(hostNameOrAddress As String) As Boolean
            Return Ping(hostNameOrAddress, DEFAULT_PING_TIMEOUT)
        End Function

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in Uri.
        ''' </summary>
        ''' <param name="address">A Uri representing the host</param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(address As Uri) As Boolean
            ' We're safe from Ping(Nothing, ...) due to overload failure (Ping(String,...) vs. Ping(Uri,...)).
            ' However, it is good practice to verify address before calling address.Host.
            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If
            Return Ping(address.Host, DEFAULT_PING_TIMEOUT)
        End Function

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in address.
        ''' </summary>
        ''' <param name="hostNameOrAddress">The name of the host as a Url or IP Address</param>
        ''' <param name="timeout">Time to wait before aborting ping</param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(hostNameOrAddress As String, timeout As Integer) As Boolean

            ' Make sure a network is available
            If Not IsAvailable Then
                Throw ExUtils.GetInvalidOperationException(SR.Network_NetworkNotAvailable)
            End If

            Dim PingMaker As New NetInfoAlias.Ping
            Dim Reply As NetInfoAlias.PingReply = PingMaker.Send(hostNameOrAddress, timeout, PingBuffer)
            Return Reply.Status = NetInfoAlias.IPStatus.Success
        End Function

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in Uri.
        ''' </summary>
        ''' <param name="address">A Uri representing the host</param>
        ''' <param name="timeout">Time to wait before aborting ping</param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(address As Uri, timeout As Integer) As Boolean
            ' We're safe from Ping(Nothing, ...) due to overload failure (Ping(String,...) vs. Ping(Uri,...)).
            ' However, it is good practice to verify address before calling address.Host.
            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If
            Return Ping(address.Host, timeout)
        End Function

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">The full name and path of the host destination</param>
        Public Sub UploadFile(sourceFileName As String, address As String)
            UploadFile(sourceFileName, address, DEFAULT_USERNAME, DEFAULT_PASSWORD, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        Public Sub UploadFile(sourceFileName As String, address As Uri)
            UploadFile(sourceFileName, address, DEFAULT_USERNAME, DEFAULT_PASSWORD, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">The full name and path of the host destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="password">The user's password</param>
        Public Sub UploadFile(sourceFileName As String, address As String, userName As String, password As String)
            UploadFile(sourceFileName, address, userName, password, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="password">The user's password</param>
        Public Sub UploadFile(sourceFileName As String, address As Uri, userName As String, password As String)
            UploadFile(sourceFileName, address, userName, password, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">The full name and path of the host destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="Password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        Public Sub UploadFile(sourceFileName As String,
                      address As String,
                      userName As String,
                      password As String,
                      showUI As Boolean,
                      connectionTimeout As Integer)

            UploadFile(sourceFileName, address, userName, password, showUI, connectionTimeout, UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">The full name and path of the host destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="Password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub UploadFile(sourceFileName As String,
                      address As String,
                      userName As String,
                      password As String,
                      showUI As Boolean,
                      connectionTimeout As Integer,
                      onUserCancel As UICancelOption)

            ' We're safe from UploadFile(Nothing, ...) due to overload failure (UploadFile(String,...) vs. UploadFile(Uri,...)).
            ' However, it is good practice to verify address before calling address.Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            ' Getting a uri will validate the form of the host address
            Dim addressUri As Uri = GetUri(address.Trim())

            ' For uploads, we need to make sure the address includes the filename
            If String.IsNullOrEmpty(IO.Path.GetFileName(addressUri.AbsolutePath)) Then
                Throw ExUtils.GetInvalidOperationException(SR.Network_UploadAddressNeedsFilename)
            End If

            UploadFile(sourceFileName, addressUri, userName, password, showUI, connectionTimeout, onUserCancel)

        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        Public Sub UploadFile(sourceFileName As String,
                              address As Uri,
                              userName As String,
                              password As String,
                              showUI As Boolean,
                              connectionTimeout As Integer)

            UploadFile(sourceFileName, address, userName, password, showUI, connectionTimeout, UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub UploadFile(sourceFileName As String,
                              address As Uri,
                              userName As String,
                              password As String,
                              showUI As Boolean,
                              connectionTimeout As Integer,
                              onUserCancel As UICancelOption)

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            UploadFile(sourceFileName, address, networkCredentials, showUI, connectionTimeout, onUserCancel)

        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="networkCredentials">The credentials of the user performing the upload</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        Public Sub UploadFile(sourceFileName As String,
                              address As Uri,
                              networkCredentials As ICredentials,
                              showUI As Boolean,
                              connectionTimeout As Integer)

            UploadFile(sourceFileName, address, networkCredentials, showUI, connectionTimeout, UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="networkCredentials">The credentials of the user performing the upload</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub UploadFile(sourceFileName As String,
                              address As Uri,
                              networkCredentials As ICredentials,
                              showUI As Boolean,
                              connectionTimeout As Integer,
                              onUserCancel As UICancelOption)
            sourceFileName = FileSystemUtils.NormalizeFilePath(sourceFileName, "sourceFileName")

            'Make sure the file exists
            If Not IO.File.Exists(sourceFileName) Then
                Throw New IO.FileNotFoundException(Utils.GetResourceString(SR.IO_FileNotFound_Path, sourceFileName))
            End If

            If connectionTimeout <= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Using client As New WebClientExtended()
                client.Timeout = connectionTimeout

                ' Set credentials if we have any
                If networkCredentials IsNot Nothing Then
                    client.Credentials = networkCredentials
                End If

                Dim Dialog As ProgressDialog = Nothing
                If showUI AndAlso Environment.UserInteractive Then
                    Dialog = New ProgressDialog With {
                        .Text = Utils.GetResourceString(SR.ProgressDialogUploadingTitle, sourceFileName),
                        .LabelText = Utils.GetResourceString(SR.ProgressDialogUploadingLabel, sourceFileName, address.AbsolutePath)
                    }
                End If

                'Create the copier
                Dim copier As New WebClientCopy(client, Dialog)

                'Download the file
                copier.UploadFile(sourceFileName, address)

                'Handle a dialog cancel
                If showUI AndAlso Environment.UserInteractive Then
                    If onUserCancel = UICancelOption.ThrowException And Dialog.UserCanceledTheDialog Then
                        Throw New OperationCanceledException()
                    End If
                End If
            End Using

        End Sub

    End Class

End Namespace
