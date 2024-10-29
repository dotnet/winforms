' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Security
Imports System.Threading

Imports NetInfoAlias = System.Net.NetworkInformation

Namespace Microsoft.VisualBasic.Devices

    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Delegate Sub NetworkAvailableEventHandler(sender As Object, e As NetworkAvailableEventArgs)

    Partial Public Class Network

        ' Object for syncing
        Private ReadOnly _syncObject As New Object()

        ' Indicates last known connection state
        Private _connected As Boolean

        ' Used for marshalling the network address changed event to the foreground thread
        Private _networkAvailabilityChangedCallback As SendOrPostCallback

        ' Holds the listeners to our NetworkAvailability changed event
        Private _networkAvailabilityEventHandlers As List(Of NetworkAvailableEventHandler)

        Private _synchronizationContext As SynchronizationContext

        ''' <summary>
        '''  Indicates whether or not the local machine is connected to an IP network.
        ''' </summary>
        ''' <value>
        '''  <see langword="True"/> if connected,
        '''  otherwise <see langword="False"/>.
        ''' </value>
        Public ReadOnly Property IsAvailable() As Boolean
            Get
                Return NetInfoAlias.NetworkInterface.GetIsNetworkAvailable()
            End Get
        End Property

        ''' <summary>
        '''  <see langword="Event"/> fired when connected to the network.
        ''' </summary>
        ''' <param name="Sender">Has no meaning for this event.</param>
        ''' <param name="e">Has no meaning for this event.</param>
        Public Custom Event NetworkAvailabilityChanged As NetworkAvailableEventHandler
            ' This is a custom event because we want to hook up
            ' the NetworkAvailabilityChanged event only if the user writes a handler for it.
            ' The reason being that it is very expensive to handle and kills our application startup perf.
            AddHandler(handler As NetworkAvailableEventHandler)

                ' Set the current state of connectedness, swallow known exceptions
                ' since user won't be able to correct problem
                Try
                    _connected = IsAvailable
                Catch ex As SecurityException
                    Return
                Catch ex As PlatformNotSupportedException
                    Return
                End Try
                ' We don't want our event firing before we've finished setting up the infrastructure.
                ' Also, need to assure there are no races in here so we don't hook up the OS listener twice, etc.
                SyncLock _syncObject
                    If _networkAvailabilityEventHandlers Is Nothing Then
                        _networkAvailabilityEventHandlers = New List(Of NetworkAvailableEventHandler)
                    End If
                    _networkAvailabilityEventHandlers.Add(handler)

                    ' Only setup the event Marshalling infrastructure once
                    If _networkAvailabilityEventHandlers.Count = 1 Then
                        ' The async operation posts to this delegate
                        _networkAvailabilityChangedCallback = New SendOrPostCallback(
                            AddressOf NetworkAvailabilityChangedHandler)

                        If AsyncOperationManager.SynchronizationContext IsNot Nothing Then
                            ' We need to hang on to the synchronization context associated
                            ' with the thread the network object is created on
                            _synchronizationContext = AsyncOperationManager.SynchronizationContext
                            Try
                                ' Exceptions are thrown if the user isn't an admin.
                                ' Listen to the OS event
                                AddHandler NetInfoAlias.NetworkChange.NetworkAddressChanged,
                                    New NetInfoAlias.NetworkAddressChangedEventHandler(
                                        AddressOf OS_NetworkAvailabilityChangedListener)
                            Catch ex As PlatformNotSupportedException
                            Catch ex As NetInfoAlias.NetworkInformationException
                            End Try
                        End If
                    End If
                End SyncLock
            End AddHandler

            RemoveHandler(handler As NetworkAvailableEventHandler)
                If _networkAvailabilityEventHandlers IsNot Nothing AndAlso
                    _networkAvailabilityEventHandlers.Count > 0 Then

                    _networkAvailabilityEventHandlers.Remove(handler)
                    ' Last one to leave, turn out the lights...
                    If _networkAvailabilityEventHandlers.Count = 0 Then
                        ' Listen to the OS event
                        RemoveHandler NetInfoAlias.NetworkChange.NetworkAddressChanged,
                            New NetInfoAlias.NetworkAddressChangedEventHandler(
                                AddressOf OS_NetworkAvailabilityChangedListener)

                        ' Stop listening to network change events since nobody is listening to us anymore
                        DisconnectListener()
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

        ' Listens to the AddressChanged event which will come on the same thread that this
        ' class was created on (AsyncEventManager is responsible for getting the event here)
        Private Sub NetworkAvailabilityChangedHandler(state As Object)
            Dim connected As Boolean = IsAvailable
            ' Fire an event only if the connected state has changed
            If _connected <> connected Then
                _connected = connected
                RaiseEvent NetworkAvailabilityChanged(Me, New NetworkAvailableEventArgs(connected))
            End If
        End Sub

        ' Listens to the AddressChanged event from the OS which comes in on an arbitrary thread
        Private Sub OS_NetworkAvailabilityChangedListener(sender As Object, e As EventArgs)
            SyncLock _syncObject
                ' Ensure we don't handle events until after we've finished setting up the event
                ' marshalling infrastructure. Don't call AsyncOperationManager.OperationSynchronizationContext.Post.
                ' The reason we want to go through _synchronizationContext is that the
                ' OperationSynchronizationContext is thread static. Since we are getting called on some random thread,
                ' the context that was in place when the Network object was created won't be available
                ' (it is on the original thread). To hang on to the original context associated with the thread
                ' that the network object is created on, I use _synchronizationContext.
                _synchronizationContext.Post(_networkAvailabilityChangedCallback, Nothing)
            End SyncLock
        End Sub

        Friend Sub DisconnectListener()
            RemoveHandler NetInfoAlias.NetworkChange.NetworkAddressChanged,
                New NetInfoAlias.NetworkAddressChangedEventHandler(
                    AddressOf OS_NetworkAvailabilityChangedListener)
        End Sub

    End Class
End Namespace
