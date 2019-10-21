' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Net
Imports NetInfoAlias = System.Net.NetworkInformation
Imports System.Security
Imports System.Security.Permissions
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils
Imports Microsoft.VisualBasic.MyServices.Internal

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  Used to pass network connectivity status.
    ''' </summary>
    Public Class NetworkAvailableEventArgs
        Inherits EventArgs
        Public Sub New(ByVal networkAvailable As Boolean)
            IsNetworkAvailable = networkAvailable
        End Sub

        Public ReadOnly Property IsNetworkAvailable() As Boolean
    End Class

    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub NetworkAvailableEventHandler(ByVal sender As Object, ByVal e As NetworkAvailableEventArgs)

    ''' <summary>
    '''  An object that allows easy access to some simple network properties and functionality.
    ''' </summary>
    Public Class Network

        ''' <summary>
        '''  Event fired when connected to the network
        ''' </summary>
        ''' <param name="Sender">Has no meaning for this event</param>
        ''' <param name="e">Has no meaning for this event</param>
        Public Custom Event NetworkAvailabilityChanged As NetworkAvailableEventHandler
            'This is a custom event because we want to hook up the NetworkAvailabilityChanged event only if the user writes a handler for it.
            'The reason being that it is very expensive to handle and kills our application startup perf.
            AddHandler(ByVal handler As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler)

                ' Set the current state of connectedness, swallow known exceptions since user won't be able to correct problem
                Try
                    m_Connected = Me.IsAvailable
                Catch ex As System.Security.SecurityException
                    Return
                Catch ex As System.PlatformNotSupportedException
                    Return
                End Try
                SyncLock m_SyncObject 'we don't want our event firing before we've finished setting up the infrastructure.  Also, need to assure there are no races in here so we don't hook up the OS listener twice, etc.
                    If m_NetworkAvailabilityEventHandlers Is Nothing Then m_NetworkAvailabilityEventHandlers = New System.Collections.ArrayList
                    m_NetworkAvailabilityEventHandlers.Add(handler)

                    'Only setup the event marshalling infrastructure once
                    If m_NetworkAvailabilityEventHandlers.Count = 1 Then
                        m_NetworkAvailabilityChangedCallback = New Threading.SendOrPostCallback(AddressOf NetworkAvailabilityChangedHandler) 'the async operation posts to this delegate
                        If AsyncOperationManager.SynchronizationContext IsNot Nothing Then
                            m_SynchronizationContext = AsyncOperationManager.SynchronizationContext 'We need to hang on to the syncronization context associated with the thread the network object is created on                        
                            Try ' Exceptions are thrown if the user isn't an admin.
                                AddHandler NetInfoAlias.NetworkChange.NetworkAddressChanged, New NetInfoAlias.NetworkAddressChangedEventHandler(AddressOf Me.OS_NetworkAvailabilityChangedListener) 'listen to the OS event
                            Catch ex As System.PlatformNotSupportedException
                            Catch ex As NetInfoAlias.NetworkInformationException
                            End Try
                        End If
                    End If
                End SyncLock
            End AddHandler

            RemoveHandler(ByVal handler As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler)
                If m_NetworkAvailabilityEventHandlers IsNot Nothing AndAlso m_NetworkAvailabilityEventHandlers.Count > 0 Then
                    m_NetworkAvailabilityEventHandlers.Remove(handler)
                    'Last one to leave, turn out the lights...
                    If m_NetworkAvailabilityEventHandlers.Count = 0 Then
                        RemoveHandler NetInfoAlias.NetworkChange.NetworkAddressChanged, New NetInfoAlias.NetworkAddressChangedEventHandler(AddressOf Me.OS_NetworkAvailabilityChangedListener) 'listen to the OS event
                        DisconnectListener() 'Stop listening to network change events since nobody is listening to us anymore
                    End If
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventArgs)
                If m_NetworkAvailabilityEventHandlers IsNot Nothing Then
                    For Each handler As Global.Microsoft.VisualBasic.Devices.NetworkAvailableEventHandler In m_NetworkAvailabilityEventHandlers
                        If handler IsNot Nothing Then handler.Invoke(sender, e)
                    Next
                End If
            End RaiseEvent
        End Event

        ''' <summary>
        '''  Creates class and hooks up events
        ''' </summary>
        Public Sub New()
        End Sub

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
        ''' Sends and receives a packet to and from the passed in address.
        ''' </summary>
        ''' <param name="hostNameOrAddress"></param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(ByVal hostNameOrAddress As String) As Boolean
            Return Ping(hostNameOrAddress, DEFAULT_PING_TIMEOUT)
        End Function

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in Uri.
        ''' </summary>
        ''' <param name="address">A Uri representing the host</param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(ByVal address As Uri) As Boolean
            ' We're safe from Ping(Nothing, ...) due to overload failure (Ping(String,...) vs. Ping(Uri,...)).
            ' However, it is good practice to verify address before calling address.Host.
            If address Is Nothing Then
                Throw ExceptionUtils.GetArgumentNullException("address")
            End If
            Return Ping(address.Host, DEFAULT_PING_TIMEOUT)
        End Function

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in address.
        ''' </summary>
        ''' <param name="hostNameOrAddress">The name of the host as a Url or IP Address</param>
        ''' <param name="timeout">Time to wait before aborting ping</param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(ByVal hostNameOrAddress As String, ByVal timeout As Integer) As Boolean

            ' Make sure a network is available
            If Not Me.IsAvailable Then
                Throw ExceptionUtils.GetInvalidOperationException(SR.Network_NetworkNotAvailable)
            End If

            Dim PingMaker As New NetInfoAlias.Ping
            Dim Reply As NetInfoAlias.PingReply = PingMaker.Send(hostNameOrAddress, timeout, Me.PingBuffer)
            If Reply.Status = NetworkInformation.IPStatus.Success Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in Uri.
        ''' </summary>
        ''' <param name="address">A Uri representing the host</param>
        ''' <param name="timeout">Time to wait before aborting ping</param>
        ''' <returns>True if ping was successful, otherwise False</returns>
        Public Function Ping(ByVal address As Uri, ByVal timeout As Integer) As Boolean
            ' We're safe from Ping(Nothing, ...) due to overload failure (Ping(String,...) vs. Ping(Uri,...)).
            ' However, it is good practice to verify address before calling address.Host.
            If address Is Nothing Then
                Throw ExceptionUtils.GetArgumentNullException("address")
            End If
            Return Ping(address.Host, timeout)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        Public Sub DownloadFile(ByVal address As String, ByVal destinationFileName As String)
            DownloadFile(address, destinationFileName, DEFAULT_USERNAME, DEFAULT_PASSWORD, False, DEFAULT_TIMEOUT, False)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        Public Sub DownloadFile(ByVal address As Uri, ByVal destinationFileName As String)
            DownloadFile(address, destinationFileName, DEFAULT_USERNAME, DEFAULT_PASSWORD, False, DEFAULT_TIMEOUT, False)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(ByVal address As String, ByVal destinationFileName As String, ByVal userName As String, ByVal password As String)
            DownloadFile(address, destinationFileName, userName, password, False, DEFAULT_TIMEOUT, False)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(ByVal address As Uri, ByVal destinationFileName As String, ByVal userName As String, ByVal password As String)
            DownloadFile(address, destinationFileName, userName, password, False, DEFAULT_TIMEOUT, False)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Public Sub DownloadFile(ByVal address As String,
                             ByVal destinationFileName As String,
                             ByVal userName As String,
                             ByVal password As String,
                             ByVal showUI As Boolean,
                             ByVal connectionTimeout As Integer,
                             ByVal overwrite As Boolean)

            DownloadFile(address, destinationFileName, userName, password, showUI, connectionTimeout, overwrite, UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub DownloadFile(ByVal address As String,
                             ByVal destinationFileName As String,
                             ByVal userName As String,
                             ByVal password As String,
                             ByVal showUI As Boolean,
                             ByVal connectionTimeout As Integer,
                             ByVal overwrite As Boolean,
                             ByVal onUserCancel As UICancelOption)

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...) vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrEmpty(address) OrElse address.Trim() = "" Then
                Throw ExceptionUtils.GetArgumentNullException("address")
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            DownloadFile(addressUri, destinationFileName, networkCredentials, showUI, connectionTimeout, overwrite, onUserCancel)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Sub DownloadFile(ByVal address As Uri,
                     ByVal destinationFileName As String,
                     ByVal userName As String,
                     ByVal password As String,
                     ByVal showUI As Boolean,
                     ByVal connectionTimeout As Integer,
                     ByVal overwrite As Boolean)

            DownloadFile(address, destinationFileName, userName, password, showUI, connectionTimeout, overwrite, UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Sub DownloadFile(ByVal address As Uri,
                     ByVal destinationFileName As String,
                     ByVal userName As String,
                     ByVal password As String,
                     ByVal showUI As Boolean,
                     ByVal connectionTimeout As Integer,
                     ByVal overwrite As Boolean,
                     ByVal onUserCancel As UICancelOption)

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            DownloadFile(address, destinationFileName, networkCredentials, showUI, connectionTimeout, overwrite, onUserCancel)
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Sub DownloadFile(ByVal address As Uri,
                    ByVal destinationFileName As String,
                    ByVal networkCredentials As System.Net.ICredentials,
                    ByVal showUI As Boolean,
                    ByVal connectionTimeout As Integer,
                    ByVal overwrite As Boolean)

            DownloadFile(address, destinationFileName, networkCredentials, showUI, connectionTimeout, overwrite, UICancelOption.ThrowException)

        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Sub DownloadFile(ByVal address As Uri,
                    ByVal destinationFileName As String,
                    ByVal networkCredentials As System.Net.ICredentials,
                    ByVal showUI As Boolean,
                    ByVal connectionTimeout As Integer,
                    ByVal overwrite As Boolean,
                    ByVal onUserCancel As UICancelOption)

            If connectionTimeout <= 0 Then
                Throw GetArgumentExceptionWithArgName("connectionTimeOut", SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw ExceptionUtils.GetArgumentNullException("address")
            End If

            Using client As New WebClientExtended
                client.Timeout = connectionTimeout

                ' Don't use passive mode if we're showing UI
                client.UseNonPassiveFtp = showUI

                'Construct the local file. This will validate the full name and path
                Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, "destinationFileName")

                ' Sometime a path that can't be parsed is normalized to the current directory. This makes sure we really
                ' have a file and path
                If System.IO.Directory.Exists(fullFilename) Then
                    Throw ExceptionUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
                End If

                'Throw if the file exists and the user doesn't want to overwrite
                If IO.File.Exists(fullFilename) And Not overwrite Then
                    Throw New IO.IOException(GetResourceString(SR.IO_FileExists_Path, destinationFileName))
                End If

                ' Set credentials if we have any
                If networkCredentials IsNot Nothing Then
                    client.Credentials = networkCredentials
                End If

                Dim dialog As ProgressDialog = Nothing
                If showUI AndAlso System.Environment.UserInteractive Then
                    ' Do UI demand here rather than waiting for form.show so that exception is thrown as early as possible
                    Dim UIPermission As New UIPermission(UIPermissionWindow.SafeSubWindows)
                    UIPermission.Demand()

                    dialog = New ProgressDialog()
                    dialog.Text = GetResourceString(SR.ProgressDialogDownloadingTitle, address.AbsolutePath)
                    dialog.LabelText = GetResourceString(SR.ProgressDialogDownloadingLabel, address.AbsolutePath, fullFilename)
                End If

                'Check to see if the target directory exists. If it doesn't, create it
                Dim targetDirectory As String = System.IO.Path.GetDirectoryName(fullFilename)

                ' Make sure we have a meaningful directory. If we don't, the destinationFileName is suspect
                If targetDirectory = "" Then
                    Throw ExceptionUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
                End If

                If Not IO.Directory.Exists(targetDirectory) Then
                    IO.Directory.CreateDirectory(targetDirectory)
                End If

                'Create the copier
                Dim copier As New WebClientCopy(client, dialog)

                'Download the file
                copier.DownloadFile(address, fullFilename)

                'Handle a dialog cancel
                If showUI AndAlso System.Environment.UserInteractive Then
                    If onUserCancel = UICancelOption.ThrowException And dialog.UserCanceledTheDialog Then
                        Throw New OperationCanceledException()
                    End If
                End If

            End Using

        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">The full name and path of the host destination</param>
        Public Sub UploadFile(ByVal sourceFileName As String, ByVal address As String)
            UploadFile(sourceFileName, address, DEFAULT_USERNAME, DEFAULT_PASSWORD, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        Public Sub UploadFile(ByVal sourceFileName As String, ByVal address As Uri)
            UploadFile(sourceFileName, address, DEFAULT_USERNAME, DEFAULT_PASSWORD, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">The full name and path of the host destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="password">The user's password</param>
        Public Sub UploadFile(ByVal sourceFileName As String, ByVal address As String, ByVal userName As String, ByVal password As String)
            UploadFile(sourceFileName, address, userName, password, False, DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="userName">The name of the user performing the upload</param>
        ''' <param name="password">The user's password</param>
        Public Sub UploadFile(ByVal sourceFileName As String, ByVal address As Uri, ByVal userName As String, ByVal password As String)
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
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        Public Sub UploadFile(ByVal sourceFileName As String,
                      ByVal address As String,
                      ByVal userName As String,
                      ByVal password As String,
                      ByVal showUI As Boolean,
                      ByVal connectionTimeout As Integer)

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
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub UploadFile(ByVal sourceFileName As String,
                      ByVal address As String,
                      ByVal userName As String,
                      ByVal password As String,
                      ByVal showUI As Boolean,
                      ByVal connectionTimeout As Integer,
                      ByVal onUserCancel As UICancelOption)

            ' We're safe from UploadFile(Nothing, ...) due to overload failure (UploadFile(String,...) vs. UploadFile(Uri,...)).
            ' However, it is good practice to verify address before calling address.Trim.
            If String.IsNullOrEmpty(address) OrElse address.Trim() = "" Then
                Throw ExceptionUtils.GetArgumentNullException("address")
            End If

            ' Getting a uri will validate the form of the host address
            Dim addressUri As Uri = GetUri(address.Trim())

            ' For uploads, we need to make sure the address includes the filename
            If System.IO.Path.GetFileName(addressUri.AbsolutePath) = "" Then
                Throw ExceptionUtils.GetInvalidOperationException(SR.Network_UploadAddressNeedsFilename)
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
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        Public Sub UploadFile(ByVal sourceFileName As String,
                              ByVal address As Uri,
                              ByVal userName As String,
                              ByVal password As String,
                              ByVal showUI As Boolean,
                              ByVal connectionTimeout As Integer)

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
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub UploadFile(ByVal sourceFileName As String,
                              ByVal address As Uri,
                              ByVal userName As String,
                              ByVal password As String,
                              ByVal showUI As Boolean,
                              ByVal connectionTimeout As Integer,
                              ByVal onUserCancel As UICancelOption)

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
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        Public Sub UploadFile(ByVal sourceFileName As String,
                              ByVal address As Uri,
                              ByVal networkCredentials As ICredentials,
                              ByVal showUI As Boolean,
                              ByVal connectionTimeout As Integer)

            UploadFile(sourceFileName, address, networkCredentials, showUI, connectionTimeout, UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        ''' Uploads a file from the local machine to the specified host
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded</param>
        ''' <param name="address">Uri representing the destination</param>
        ''' <param name="networkCredentials">The credentials of the user performing the upload</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time alloted before giving up on a connection</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub UploadFile(ByVal sourceFileName As String,
                              ByVal address As Uri,
                              ByVal networkCredentials As ICredentials,
                              ByVal showUI As Boolean,
                              ByVal connectionTimeout As Integer,
                              ByVal onUserCancel As UICancelOption)

            sourceFileName = FileSystemUtils.NormalizeFilePath(sourceFileName, "sourceFileName")

            'Make sure the file exists
            If Not IO.File.Exists(sourceFileName) Then
                Throw New IO.FileNotFoundException(GetResourceString(SR.IO_FileNotFound_Path, sourceFileName))
            End If

            If connectionTimeout <= 0 Then
                Throw GetArgumentExceptionWithArgName("connectionTimeout", SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw ExceptionUtils.GetArgumentNullException("address")
            End If

            Using client As New WebClientExtended()
                client.Timeout = connectionTimeout

                ' Set credentials if we have any
                If networkCredentials IsNot Nothing Then
                    client.Credentials = networkCredentials
                End If

                Dim Dialog As ProgressDialog = Nothing
                If showUI AndAlso System.Environment.UserInteractive Then
                    Dialog = New ProgressDialog
                    Dialog.Text = GetResourceString(SR.ProgressDialogUploadingTitle, sourceFileName)
                    Dialog.LabelText = GetResourceString(SR.ProgressDialogUploadingLabel, sourceFileName, address.AbsolutePath)
                End If

                'Create the copier
                Dim copier As New WebClientCopy(client, Dialog)

                'Download the file
                copier.UploadFile(sourceFileName, address)

                'Handle a dialog cancel
                If showUI AndAlso System.Environment.UserInteractive Then
                    If onUserCancel = UICancelOption.ThrowException And Dialog.UserCanceledTheDialog Then
                        Throw New OperationCanceledException()
                    End If
                End If
            End Using

        End Sub

        Friend Sub DisconnectListener()
            RemoveHandler NetInfoAlias.NetworkChange.NetworkAddressChanged, New NetInfoAlias.NetworkAddressChangedEventHandler(AddressOf Me.OS_NetworkAvailabilityChangedListener)
        End Sub

        'Listens to the AddressChanged event from the OS which comes in on an arbitrary thread
        Private Sub OS_NetworkAvailabilityChangedListener(ByVal sender As Object, ByVal e As EventArgs)
            SyncLock m_SyncObject 'Ensure we don't handle events until after we've finished setting up the event marshalling infrastructure
                'Don't call AsyncOperationManager.OperationSynchronizationContext.Post.  The reason we want to go through m_SynchronizationContext is that
                'the OperationSyncronizationContext is thread static.  Since we are getting called on some random thread, the context that was
                'in place when the Network object was created won't be available (it is on the original thread).  To hang on to the original
                'context associated with the thread that the network object is created on, I use m_SynchronizationContext.
                m_SynchronizationContext.Post(m_NetworkAvailabilityChangedCallback, Nothing)
            End SyncLock
        End Sub

        'Listens to the AddressChanged event which will come on the same thread that this class was created on (AsyncEventManager is responsible for getting the event here)
        Private Sub NetworkAvailabilityChangedHandler(ByVal state As Object)
            Dim Connected As Boolean = Me.IsAvailable
            ' Fire an event only if the connected state has changed
            If m_Connected <> Connected Then
                m_Connected = Connected
                RaiseEvent NetworkAvailabilityChanged(Me, New NetworkAvailableEventArgs(Connected))
            End If
        End Sub

        ''' <summary>
        '''  A buffer for pinging. This imitates the buffer used by Ping.Exe
        ''' </summary>
        ''' <value>A buffer</value>
        Private ReadOnly Property PingBuffer() As Byte()
            Get
                If m_PingBuffer Is Nothing Then
                    ReDim m_PingBuffer(BUFFER_SIZE - 1)
                    For i As Integer = 0 To BUFFER_SIZE - 1
                        'This is the same logic Ping.exe uses to fill it's buffer
                        m_PingBuffer(i) = System.Convert.ToByte(Asc("a"c) + i Mod 23, System.Globalization.CultureInfo.InvariantCulture)
                    Next
                End If

                Return m_PingBuffer
            End Get
        End Property

        ''' <summary>
        '''  Gets a Uri from a uri string. We also use this function to validate the UriString (remote file address)
        ''' </summary>
        ''' <param name="address">The remote file address</param>
        ''' <returns>A Uri if successful, otherwise it throws an exception</returns>
        Private Function GetUri(ByVal address As String) As Uri
            Try
                Return New Uri(address)
            Catch ex As UriFormatException
                'Throw an exception with an error message more appropriate to our API
                Throw GetArgumentExceptionWithArgName("address", SR.Network_InvalidUriString, address)
            End Try
        End Function

        ''' <summary>
        ''' Gets network credentials from a userName and password
        ''' </summary>
        ''' <param name="userName">The name of the user</param>
        ''' <param name="password">The password of the user</param>
        ''' <returns>A NetworkCredentials</returns>
        Private Function GetNetworkCredentials(ByVal userName As String, ByVal password As String) As ICredentials

            ' Make sure all nulls are empty strings
            If userName Is Nothing Then
                userName = ""
            End If

            If password Is Nothing Then
                password = ""
            End If

            If userName = "" And password = "" Then
                Return Nothing
            End If

            Return New NetworkCredential(userName, password)
        End Function

        'Holds the buffer for pinging. We lazy initialize on first use
        Private m_PingBuffer() As Byte

        'Size of Ping.exe buffer
        Private Const BUFFER_SIZE As Integer = 32

        ' Default timeout value
        Private Const DEFAULT_TIMEOUT As Integer = 100000

        ' Defalt timeout for Ping
        Private Const DEFAULT_PING_TIMEOUT As Integer = 1000

        ' UserName used in overloads where there is no userName parameter
        Private Const DEFAULT_USERNAME As String = ""

        ' Password used in overloads where there is no password parameter
        Private Const DEFAULT_PASSWORD As String = ""

        ' Indicates last known connection state
        Private m_Connected As Boolean

        ' Object for syncing
        Private m_SyncObject As New Object()

        Private m_NetworkAvailabilityEventHandlers As System.Collections.ArrayList 'Holds the listeners to our NetworkAvailability changed event

        Private m_SynchronizationContext As System.Threading.SynchronizationContext
        Private m_NetworkAvailabilityChangedCallback As Threading.SendOrPostCallback 'Used for marshalling the network address changed event to the foreground thread
    End Class

    ''' <summary>
    ''' Temporary class used to provide WebClient with a timeout property.
    ''' </summary>
    ''' <remarks>This class will be deleted when Timeout is added to WebClient</remarks>
    Friend Class WebClientExtended
        Inherits WebClient

        ''' <summary>
        ''' Sets or indicates the timeout used by WebRequest used by WebClient
        ''' </summary>
        Public WriteOnly Property Timeout() As Integer
            Set(ByVal value As Integer)
                Debug.Assert(value > 0, "illegal value for timeout")
                m_Timeout = value
            End Set
        End Property

        ''' <summary>
        ''' Enables switching the server to non passive mode.
        ''' </summary>
        ''' <remarks>We need this in order for the progress UI on a download to work</remarks>
        Public WriteOnly Property UseNonPassiveFtp() As Boolean
            Set(ByVal value As Boolean)
                m_UseNonPassiveFtp = value
            End Set
        End Property

        ''' <summary>
        ''' Makes sure that the timeout value for WebRequests (used for all Download and Upload methods) is set
        ''' to the Timeout value
        ''' </summary>
        ''' <param name="address"></param>
        Protected Overrides Function GetWebRequest(ByVal address As System.Uri) As System.Net.WebRequest
            Dim request As WebRequest = MyBase.GetWebRequest(address)

            Debug.Assert(request IsNot Nothing, "Unable to get WebRequest from base class")
            If request IsNot Nothing Then
                request.Timeout = m_Timeout
                If m_UseNonPassiveFtp Then
                    Dim ftpRequest As FtpWebRequest = TryCast(request, FtpWebRequest)
                    If ftpRequest IsNot Nothing Then
                        ftpRequest.UsePassive = False
                    End If
                End If

                Dim httpRequest As HttpWebRequest = TryCast(request, HttpWebRequest)
                If httpRequest IsNot Nothing Then
                    httpRequest.AllowAutoRedirect = False
                End If

            End If

            Return request
        End Function

        Friend Sub New()
        End Sub

        ' The Timeout value to be used by Webclient's WebRequest for Downloading or Uploading a file
        Private m_Timeout As Integer = 100000

        ' Flag used to indicate whether or not we should use passive mode when ftp downloading
        Private m_UseNonPassiveFtp As Boolean
    End Class

End Namespace
