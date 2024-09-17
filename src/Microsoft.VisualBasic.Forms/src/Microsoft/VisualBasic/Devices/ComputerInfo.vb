' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  Provides configuration information about the current computer and the
    '''  current process.
    ''' </summary>
    <DebuggerTypeProxy(GetType(ComputerInfo.ComputerInfoDebugView))>
    Partial Public Class ComputerInfo

        ' Cache our InternalMemoryStatus
        Private _internalMemoryStatus As InternalMemoryStatus

        ' Keep the debugger proxy current as you change this class - see the nested ComputerInfoDebugView below.

        ''' <summary>
        '''  Default constructor
        ''' </summary>
        Public Sub New()
        End Sub

#Disable Warning IDE0049  ' Use language keywords instead of framework type names for type references, Justification:="<Public API>

        ''' <summary>
        '''  Gets the whole memory information details.
        ''' </summary>
        ''' <value>An InternalMemoryStatus class.</value>
        Private ReadOnly Property MemoryStatus() As InternalMemoryStatus
            Get
                If _internalMemoryStatus Is Nothing Then
                    _internalMemoryStatus = New InternalMemoryStatus
                End If
                Return _internalMemoryStatus
            End Get
        End Property

        ''' <summary>
        '''  Gets the total size of free physical memory on the machine.
        ''' </summary>
        ''' <value>A 64-bit unsigned integer containing the size of free
        '''  physical memory on the machine, in bytes.
        ''' </value>
        ''' <exception cref="ComponentModel.Win32Exception">
        '''  Throw if we are unable to obtain the memory status.
        ''' </exception>
        <CLSCompliant(False)>
        Public ReadOnly Property AvailablePhysicalMemory() As UInt64
            Get
                Return MemoryStatus.AvailablePhysicalMemory
            End Get
        End Property

        ''' <summary>
        '''  Gets the total size of free user potion of virtual address space for calling process.
        ''' </summary>
        ''' <value>
        '''  A 64-bit unsigned integer containing the size of free user potion of
        '''  virtual address space for calling process, in bytes.
        ''' </value>
        ''' <exception cref="ComponentModel.Win32Exception">
        '''  Throw if we are unable to obtain the memory status.
        ''' </exception>
        <CLSCompliant(False)>
        Public ReadOnly Property AvailableVirtualMemory() As UInt64
            Get
                Return MemoryStatus.AvailableVirtualMemory
            End Get
        End Property

        ''' <summary>
        '''  Gets the current UICulture installed on the machine.
        ''' </summary>
        ''' <value>A CultureInfo object represents the UI culture installed on the machine.</value>
        Public ReadOnly Property InstalledUICulture() As Globalization.CultureInfo
            Get
                Return Globalization.CultureInfo.InstalledUICulture
            End Get
        End Property

        ''' <summary>
        '''  Gets the full operating system name.
        ''' </summary>
        ''' <value>A string contains the operating system name.</value>
        Public ReadOnly Property OSFullName() As String
            Get
                Return RuntimeInformation.OSDescription
            End Get
        End Property

        ''' <summary>
        '''  Gets the platform OS name.
        ''' </summary>
        ''' <value>
        '''  A string containing a <see cref="PlatformID"/> like "Win32NT", "Win32S",
        '''  "Win32Windows". See <see cref="PlatformID"/> enum.
        ''' </value>
        ''' <exception cref="ExecutionEngineException">
        '''  Thrown if cannot obtain the OS Version information.
        ''' </exception>
        Public ReadOnly Property OSPlatform() As String
            Get
                Return Environment.OSVersion.Platform.ToString
            End Get
        End Property

        ''' <summary>
        '''  Gets the current version number of the operating system.
        ''' </summary>
        ''' <value>
        '''  A string contains the current version number of the operating system.</value>
        ''' <exception cref="ExecutionEngineException">
        '''  If cannot obtain the OS Version information.
        ''' </exception>
        Public ReadOnly Property OSVersion() As String
            Get
                Return Environment.OSVersion.Version.ToString
            End Get
        End Property

        ''' <summary>
        '''  Gets the total size of physical memory on the machine.
        ''' </summary>
        ''' <value>
        '''  A 64-bit unsigned integer containing the size of total physical
        '''  memory on the machine, in bytes.
        '''  </value>
        ''' <exception cref="ComponentModel.Win32Exception">
        '''  Throw if we are unable to obtain the memory status.
        ''' </exception>
        <CLSCompliant(False)>
        Public ReadOnly Property TotalPhysicalMemory() As UInt64
            Get
                Return MemoryStatus.TotalPhysicalMemory
            End Get
        End Property

        ''' <summary>
        '''  Gets the total size of user potion of virtual address space for calling process.
        ''' </summary>
        ''' <value>
        '''   A 64-bit unsigned integer containing the size of user potion of virtual
        '''   address space for calling process, in bytes.
        '''  </value>
        ''' <exception cref="ComponentModel.Win32Exception">
        '''  Throw if we are unable to obtain the memory status.
        ''' </exception>
        <CLSCompliant(False)>
        Public ReadOnly Property TotalVirtualMemory() As UInt64
            Get
                Return MemoryStatus.TotalVirtualMemory
            End Get
        End Property

    End Class
End Namespace
