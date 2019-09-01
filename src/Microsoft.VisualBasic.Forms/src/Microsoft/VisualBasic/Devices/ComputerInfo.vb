' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System
Imports System.Diagnostics
Imports System.Management
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

        ''' <summary>
        ''' Gets the total size of physical memory on the machine.
        ''' </summary>
        ''' <value>A 64-bit unsigned integer containing the size of total physical memory on the machine, in bytes.</value>
        ''' <exception cref="System.ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
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
        ''' <exception cref="System.ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
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
        ''' <exception cref="System.ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
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
        ''' <exception cref="System.ComponentModel.Win32Exception">If we are unable to obtain the memory status.</exception>
        <CLSCompliant(False)>
        Public ReadOnly Property AvailableVirtualMemory() As UInt64
            Get
                Return MemoryStatus.AvailableVirtualMemory
            End Get
        End Property

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
        ''' Gets the full operating system name. This method requires full trust and WMI installed.
        ''' </summary>
        ''' <value>A string contains the operating system name.</value>
        ''' <exception cref="System.Security.SecurityException">If the immediate caller does not have full trust.</exception>
        ''' <exception cref="System.InvalidOperationException">If we cannot obtain the query object from WMI.</exception>
        ''' <remarks>Since this property depends on WMI, we have OSPlatform property that does not require WMI.</remarks>
        Public ReadOnly Property OSFullName() As String
            Get
                Try
                    ' There is no PInvoke Call for this purpose, have to use WMI.
                    ' The result from WMI is 'MS Windows xxx|C:\WINNT\Device\Harddisk0\Partition1.
                    ' We only show the first part. NOTE: This is fragile.
                    Dim PropertyName As String = "Name"
                    Dim Separator As Char = "|"c

                    Dim Result As String = CStr(OSManagementBaseObject.Properties(PropertyName).Value)
                    If Result.Contains(Separator) Then
                        Return Result.Substring(0, Result.IndexOf(Separator))
                    Else
                        Return Result
                    End If
                Catch ex As System.Runtime.InteropServices.COMException
                    Return OSPlatform
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Gets the platform OS name.
        ''' </summary>
        ''' <value>A string containing a Platform ID like "Win32NT", "Win32S", "Win32Windows". See PlatformID enum.</value>
        ''' <exception cref="System.ExecutionEngineException">If cannot obtain the OS Version information.</exception>
        Public ReadOnly Property OSPlatform() As String
            Get
                Return Environment.OSVersion.Platform.ToString
            End Get
        End Property

        ''' <summary>
        ''' Gets the current version number of the operating system.
        ''' </summary>
        ''' <value>A string contains the current version number of the operating system.</value>
        ''' <exception cref="System.ExecutionEngineException">If cannot obtain the OS Version information.</exception>
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
            Public Sub New(ByVal RealClass As ComputerInfo)
                m_InstanceBeingWatched = RealClass
            End Sub

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property TotalPhysicalMemory() As UInt64
                Get
                    Return m_InstanceBeingWatched.TotalPhysicalMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property AvailablePhysicalMemory() As UInt64
                Get
                    Return m_InstanceBeingWatched.AvailablePhysicalMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property TotalVirtualMemory() As UInt64
                Get
                    Return m_InstanceBeingWatched.TotalVirtualMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property AvailableVirtualMemory() As UInt64
                Get
                    Return m_InstanceBeingWatched.AvailableVirtualMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property InstalledUICulture() As Globalization.CultureInfo
                Get
                    Return m_InstanceBeingWatched.InstalledUICulture
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property OSPlatform() As String
                Get
                    Return m_InstanceBeingWatched.OSPlatform
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property OSVersion() As String
                Get
                    Return m_InstanceBeingWatched.OSVersion
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.Never)>
            Private m_InstanceBeingWatched As ComputerInfo
        End Class

        ''' <summary>
        ''' Gets the whole memory information details.
        ''' </summary>
        ''' <value>An InternalMemoryStatus class.</value>
        Private ReadOnly Property MemoryStatus() As InternalMemoryStatus
            Get
                If m_InternalMemoryStatus Is Nothing Then
                    m_InternalMemoryStatus = New InternalMemoryStatus
                End If
                Return m_InternalMemoryStatus
            End Get
        End Property

        ''' <summary>
        ''' Get the management object used in WMI to query for the operating system name.
        ''' </summary>
        ''' <value>A ManagementBaseObject represents the result of "Win32_OperatingSystem" query.</value>
        ''' <exception cref="System.Security.SecurityException">If the immediate caller does not have full trust.</exception>
        ''' <exception cref="System.InvalidOperationException">If we cannot obtain the query object from WMI.</exception>
        Private ReadOnly Property OSManagementBaseObject() As ManagementBaseObject
            Get
                ' Query string to get the OperatingSystem information.
                Dim QueryString As String = "Win32_OperatingSystem"

                ' Assumption: Each thread will have its own instance of App class so no need to SyncLock this.
                If m_OSManagementObject Is Nothing Then
                    ' Build a query for enumeration of Win32_OperatingSystem instances
                    Dim Query As New SelectQuery(QueryString)

                    ' Instantiate an object searcher with this query
                    Dim Searcher As New ManagementObjectSearcher(Query)

                    Dim ManagementObjCollection As ManagementObjectCollection = Searcher.Get

                    If ManagementObjCollection.Count > 0 Then
                        Debug.Assert(ManagementObjCollection.Count = 1, "Should find 1 instance only!!!")

                        Dim ManagementObjEnumerator As ManagementObjectCollection.ManagementObjectEnumerator =
                            ManagementObjCollection.GetEnumerator
                        ManagementObjEnumerator.MoveNext()
                        m_OSManagementObject = ManagementObjEnumerator.Current
                    Else
                        Throw ExceptionUtils.GetInvalidOperationException(SR.DiagnosticInfo_FullOSName)
                    End If
                End If

                Debug.Assert(m_OSManagementObject IsNot Nothing, "Null management object!!!")
                Return m_OSManagementObject
            End Get
        End Property

        Private m_OSManagementObject As ManagementBaseObject = Nothing ' Cache the management object gotten from WMI.
        Private m_InternalMemoryStatus As InternalMemoryStatus = Nothing ' Cache our InternalMemoryStatus

        ''' <summary>
        ''' Calls GlobalMemoryStatusEx and returns the correct value.
        ''' </summary>
        Private Class InternalMemoryStatus
            Friend Sub New()
            End Sub

            Friend ReadOnly Property TotalPhysicalMemory() As UInt64
                Get
                    Refresh()
                    Return m_MemoryStatusEx.ullTotalPhys
                End Get
            End Property

            Friend ReadOnly Property AvailablePhysicalMemory() As UInt64
                Get
                    Refresh()
                    Return m_MemoryStatusEx.ullAvailPhys
                End Get
            End Property

            Friend ReadOnly Property TotalVirtualMemory() As UInt64
                Get
                    Refresh()
                    Return m_MemoryStatusEx.ullTotalVirtual
                End Get
            End Property

            Friend ReadOnly Property AvailableVirtualMemory() As UInt64
                Get
                    Refresh()
                    Return m_MemoryStatusEx.ullAvailVirtual
                End Get
            End Property

            Private Sub Refresh()
                m_MemoryStatusEx = New NativeMethods.MEMORYSTATUSEX
                m_MemoryStatusEx.Init()
                If (Not NativeMethods.GlobalMemoryStatusEx(m_MemoryStatusEx)) Then
                    Throw ExceptionUtils.GetWin32Exception(SR.DiagnosticInfo_Memory)
                End If
            End Sub

            Private m_MemoryStatusEx As NativeMethods.MEMORYSTATUSEX
        End Class
    End Class

End Namespace
