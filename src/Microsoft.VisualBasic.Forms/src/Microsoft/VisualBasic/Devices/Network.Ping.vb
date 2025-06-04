' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports NetInfoAlias = System.Net.NetworkInformation
Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    Partial Public Class Network

        ' Size of Ping.exe buffer
        Private Const BUFFER_SIZE As Integer = 32

        ' Default timeout for Ping
        Private Const DEFAULT_PING_TIMEOUT As Integer = 1000

        ' Holds the buffer for pinging. We lazy initialize on first use
        Private _pingBuffer() As Byte

        ''' <summary>
        '''  A buffer for pinging. This imitates the buffer used by Ping.Exe.
        ''' </summary>
        ''' <value>A buffer.</value>
        Private ReadOnly Property PingBuffer() As Byte()
            Get
                If _pingBuffer Is Nothing Then
                    ReDim _pingBuffer(BUFFER_SIZE - 1)
                    For i As Integer = 0 To BUFFER_SIZE - 1
                        ' This is the same logic Ping.exe uses to fill it's buffer
                        _pingBuffer(i) = Convert.ToByte(
                            value:=Asc("a"c) + (i Mod 23),
                            provider:=Globalization.CultureInfo.InvariantCulture)
                    Next
                End If

                Return _pingBuffer
            End Get
        End Property

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in address.
        ''' </summary>
        ''' <param name="hostNameOrAddress"></param>
        ''' <returns>True if ping was successful, otherwise False.</returns>
        Public Function Ping(hostNameOrAddress As String) As Boolean
            Return Ping(hostNameOrAddress, DEFAULT_PING_TIMEOUT)
        End Function

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in Uri.
        ''' </summary>
        ''' <param name="address">A Uri representing the host.</param>
        ''' <returns>True if ping was successful, otherwise False.</returns>
        Public Function Ping(address As Uri) As Boolean
            ' We're safe from Ping(Nothing, ...) due to overload failure (Ping(String,...) vs. Ping(Uri,...)).
            ' However, it is good practice to verify address before calling address.Host.
            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If
            Return Ping(address.Host, DEFAULT_PING_TIMEOUT)
        End Function

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in address.
        ''' </summary>
        ''' <param name="hostNameOrAddress">The name of the host as a Url or IP Address.</param>
        ''' <param name="timeout">Time to wait before aborting ping.</param>
        ''' <returns>True if ping was successful, otherwise False.</returns>
        Public Function Ping(hostNameOrAddress As String, timeout As Integer) As Boolean

            ' Make sure a network is available
            If Not IsAvailable Then
                Throw VbUtils.GetInvalidOperationException(SR.Network_NetworkNotAvailable)
            End If

            Dim pingMaker As New NetInfoAlias.Ping
            Dim reply As NetInfoAlias.PingReply = pingMaker.Send(
                hostNameOrAddress,
                timeout,
                PingBuffer)
            If reply.Status = NetInfoAlias.IPStatus.Success Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in Uri.
        ''' </summary>
        ''' <param name="address">A Uri representing the host.</param>
        ''' <param name="timeout">Time to wait before aborting ping.</param>
        ''' <returns>True if ping was successful, otherwise False.</returns>
        Public Function Ping(address As Uri, timeout As Integer) As Boolean
            ' We're safe from Ping(Nothing, ...) due to overload failure (Ping(String,...) vs. Ping(Uri,...)).
            ' However, it is good practice to verify address before calling address.Host.
            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If
            Return Ping(address.Host, timeout)
        End Function

    End Class
End Namespace
