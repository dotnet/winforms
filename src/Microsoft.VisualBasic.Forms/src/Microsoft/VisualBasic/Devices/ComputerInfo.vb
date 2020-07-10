' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    ''' Provides configuration information about the current computer and the current process.
    ''' </summary>
    <DebuggerTypeProxy(GetType(ComputerInfo.ComputerInfoDebugView))>
    Public Class ComputerInfo

        ' Keep the debugger proxy current as you change this class - see the nested ComputerInfoDebugView below.

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Sub New()
        End Sub

#Disable Warning IDE0049 ' Simplify Names, Justification:="<Public API>
        ''' <summary>
        ''' Gets the total size of physical memory on the machine.
        ''' </summary>
        ''' <value>A 64-bit unsigned integer containing the size of total physical memory on the machine, in bytes.</value>
        ''' <exception cref="ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
        <CLSCompliant(False)>
        Public ReadOnly Property TotalPhysicalMemory() As UInt64
            Get
                Return MemoryStatus.TotalPhysicalMemory
            End Get
        End Property

        ''' <summary>
        ''' Gets the total size of free physical memory on the machine.
        ''' </summary>
        ''' <value>A 64-bit unsigned integer containing the size of free physical memory on the machine, in bytes.</value>
        ''' <exception cref="ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
        <CLSCompliant(False)>
        Public ReadOnly Property AvailablePhysicalMemory() As UInt64
            Get
                Return MemoryStatus.AvailablePhysicalMemory
            End Get
        End Property

        ''' <summary>
        ''' Gets the total size of user potion of virtual address space for calling process.
        ''' </summary>
        ''' <value>A 64-bit unsigned integer containing the size of user potion of virtual address space for calling process, 
        '''          in bytes.</value>
        ''' <exception cref="ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
        <CLSCompliant(False)>
        Public ReadOnly Property TotalVirtualMemory() As UInt64
            Get
                Return MemoryStatus.TotalVirtualMemory
            End Get
        End Property

        ''' <summary>
        ''' Gets the total size of free user potion of virtual address space for calling process.
        ''' </summary>
        ''' <value>A 64-bit unsigned integer containing the size of free user potion of virtual address space for calling process, 
        '''          in bytes.</value>
        ''' <exception cref="ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
        <CLSCompliant(False)>
        Public ReadOnly Property AvailableVirtualMemory() As UInt64
            Get
                Return MemoryStatus.AvailableVirtualMemory
            End Get
        End Property
#Enable Warning IDE0049 ' Simplify Names

        ''' <summary>
        ''' Gets the current UICulture installed on the machine.
        ''' </summary>
        ''' <value>A CultureInfo object represents the UI culture installed on the machine.</value>
        Public ReadOnly Property InstalledUICulture() As Globalization.CultureInfo
            Get
                Return Globalization.CultureInfo.InstalledUICulture
            End Get
        End Property

        ''' <summary>
        ''' Gets the full operating system name.
        ''' </summary>
        ''' <value>A string contains the operating system name.</value>
        Public ReadOnly Property OSFullName() As String
            Get
                Return RuntimeInformation.OSDescription
            End Get
        End Property

        ''' <summary>
        ''' Gets the platform OS name.
        ''' </summary>
        ''' <value>A string containing a Platform ID like "Win32NT", "Win32S", "Win32Windows". See PlatformID enum.</value>
        ''' <exception cref="ExecutionEngineException">If cannot obtain the OS Version information.</exception>
        Public ReadOnly Property OSPlatform() As String
            Get
                Return Environment.OSVersion.Platform.ToString
            End Get
        End Property

        ''' <summary>
        ''' Gets the current version number of the operating system.
        ''' </summary>
        ''' <value>A string contains the current version number of the operating system.</value>
        ''' <exception cref="ExecutionEngineException">If cannot obtain the OS Version information.</exception>
        Public ReadOnly Property OSVersion() As String
            Get
                Return Environment.OSVersion.Version.ToString
            End Get
        End Property

        ''' <summary>
        ''' Debugger proxy for the ComputerInfo class.  The problem is that OSFullName can time out the debugger
        ''' so we offer a view that doesn't have that field.
        ''' </summary>
        Friend NotInheritable Class ComputerInfoDebugView
            Public Sub New(RealClass As ComputerInfo)
                _instanceBeingWatched = RealClass
            End Sub

#Disable Warning IDE0049 ' Simplify Names, Justification:=<Public API>
            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property TotalPhysicalMemory() As UInt64
                Get
                    Return _instanceBeingWatched.TotalPhysicalMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property AvailablePhysicalMemory() As UInt64
                Get
                    Return _instanceBeingWatched.AvailablePhysicalMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property TotalVirtualMemory() As UInt64
                Get
                    Return _instanceBeingWatched.TotalVirtualMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property AvailableVirtualMemory() As UInt64
                Get
                    Return _instanceBeingWatched.AvailableVirtualMemory
                End Get
            End Property
#Enable Warning IDE0049 ' Simplify Names

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property InstalledUICulture() As Globalization.CultureInfo
                Get
                    Return _instanceBeingWatched.InstalledUICulture
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property OSPlatform() As String
                Get
                    Return _instanceBeingWatched.OSPlatform
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property OSVersion() As String
                Get
                    Return _instanceBeingWatched.OSVersion
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.Never)>
            Private ReadOnly _instanceBeingWatched As ComputerInfo
        End Class

        ''' <summary>
        ''' Gets the whole memory information details.
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

        Private _internalMemoryStatus As InternalMemoryStatus ' Cache our InternalMemoryStatus

        ''' <summary>
        ''' Calls GlobalMemoryStatusEx and returns the correct value.
        ''' </summary>
        Private Class InternalMemoryStatus
            Friend Sub New()
            End Sub

#Disable Warning IDE0049 ' Simplify Names, Justification:=<Public API>
            Friend ReadOnly Property TotalPhysicalMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullTotalPhys
                End Get
            End Property

            Friend ReadOnly Property AvailablePhysicalMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullAvailPhys
                End Get
            End Property

            Friend ReadOnly Property TotalVirtualMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullTotalVirtual
                End Get
            End Property

            Friend ReadOnly Property AvailableVirtualMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullAvailVirtual
                End Get
            End Property
#Enable Warning IDE0049 ' Simplify Names

            Private Sub Refresh()
                _memoryStatusEx = New NativeMethods.MEMORYSTATUSEX
                _memoryStatusEx.Init()
                If (Not NativeMethods.GlobalMemoryStatusEx(_memoryStatusEx)) Then
                    Throw ExceptionUtils.GetWin32Exception(SR.DiagnosticInfo_Memory)
                End If
            End Sub

            Private _memoryStatusEx As NativeMethods.MEMORYSTATUSEX
        End Class
    End Class

End Namespace
