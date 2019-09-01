' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.IO.Ports
Imports System.Security.Permissions
Imports System.Text
Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  Gives access to Ports on the local machine
    ''' </summary>
    ''' <remarks>Only serial ports are supported at present, but this class may expand in the future</remarks>
    Public Class Ports

        ''' <summary>
        '''  Constructor
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        '''  Creates an opens a SerialPort object
        ''' </summary>
        ''' <param name="portName">The name of the port to open</param>
        ''' <returns>An opened SerialPort</returns>
        ''' <remarks>
        '''  We delegate all validation to the fx SerialPort class. We open the port so exceptions will
        '''  be thrown as quickly as possible
        '''</remarks>
        Public Function OpenSerialPort(ByVal portName As String) As SerialPort
            Dim Port As New SerialPort(portName)
            Port.Open()
            Return Port
        End Function

        ''' <summary>
        '''  Creates an opens a SerialPort object
        ''' </summary>
        ''' <param name="portName">The name of the port to open</param>
        ''' <param name="baudRate">The baud rate of the port</param>
        ''' <returns>An opened SerialPort</returns>
        ''' <remarks>
        '''  We delegate all validation to the fx SerialPort class. We open the port so exceptions will
        '''  be thrown as quickly as possible
        '''</remarks>
        Public Function OpenSerialPort(ByVal portName As String, ByVal baudRate As Integer) As SerialPort
            Dim Port As New SerialPort(portName, baudRate)
            Port.Open()
            Return Port
        End Function

        ''' <summary>
        '''  Creates an opens a SerialPort object
        ''' </summary>
        ''' <param name="portName">The name of the port to open</param>
        ''' <param name="baudRate">The baud rate of the port</param>
        ''' <param name="parity">The parity of the port</param>
        ''' <returns>An opened SerialPort</returns>
        ''' <remarks>
        '''  We delegate all validation to the fx SerialPort class. We open the port so exceptions will
        '''  be thrown as quickly as possible
        '''</remarks>
        Public Function OpenSerialPort(ByVal portName As String, ByVal baudRate As Integer, ByVal parity As Parity) As SerialPort
            Dim Port As New SerialPort(portName, baudRate, parity)
            Port.Open()
            Return Port
        End Function

        ''' <summary>
        '''  Creates an opens a SerialPort object
        ''' </summary>
        ''' <param name="portName">The name of the port to open</param>
        ''' <param name="baudRate">The baud rate of the port</param>
        ''' <param name="parity">The parity of the port</param>
        ''' <param name="dataBits">The data bits of the port</param>
        ''' <returns>An opened SerialPort</returns>
        ''' <remarks>
        '''  We delegate all validation to the fx SerialPort class. We open the port so exceptions will
        '''  be thrown as quickly as possible
        '''</remarks>
        Public Function OpenSerialPort(ByVal portName As String, ByVal baudRate As Integer, ByVal parity As Parity, ByVal dataBits As Integer) As SerialPort
            Dim Port As New SerialPort(portName, baudRate, parity, dataBits)
            Port.Open()
            Return Port
        End Function

        ''' <summary>
        '''  Creates an opens a SerialPort object
        ''' </summary>
        ''' <param name="portName">The name of the port to open</param>
        ''' <param name="baudRate">The baud rate of the port</param>
        ''' <param name="parity">The parity of the port</param>
        ''' <param name="dataBits">The data bits of the port</param>
        ''' <param name="stopBits">The stop bit setting of the port</param>
        ''' <returns>An opened SerialPort</returns>
        ''' <remarks>
        '''  We delegate all validation to the fx SerialPort class. We open the port so exceptions will
        '''  be thrown as quickly as possible
        '''</remarks>
        Public Function OpenSerialPort(ByVal portName As String, ByVal baudRate As Integer, ByVal parity As Parity, ByVal dataBits As Integer, ByVal stopBits As StopBits) As SerialPort
            Dim Port As New SerialPort(portName, baudRate, parity, dataBits, stopBits)
            Port.Open()
            Return Port
        End Function

        ''' <summary>
        '''  Returns the names of the serial ports on the local machine
        ''' </summary>
        ''' <value>A collection of the names of the serial ports</value>
        Public ReadOnly Property SerialPortNames() As ReadOnlyCollection(Of String)
            Get
                Dim names() As String = SerialPort.GetPortNames()
                Dim namesList As New List(Of String)

                For Each portName As String In names
                    namesList.Add(portName)
                Next

                Return New ReadOnlyCollection(Of String)(namesList)
            End Get
        End Property

    End Class
End Namespace
