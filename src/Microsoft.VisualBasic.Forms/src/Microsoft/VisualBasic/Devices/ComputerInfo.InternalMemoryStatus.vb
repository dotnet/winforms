' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic.Devices

    Partial Public Class ComputerInfo

        ''' <summary>
        '''  Calls GlobalMemoryStatusEx and returns the correct value.
        ''' </summary>
        Private NotInheritable Class InternalMemoryStatus
            Private _memoryStatusEx As MEMORYSTATUSEX

            Friend Sub New()
            End Sub

#Disable Warning IDE0049  ' Use language keywords instead of framework type names for type references, Justification:=<Public API>

            Friend ReadOnly Property AvailablePhysicalMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullAvailPhys
                End Get
            End Property

            Friend ReadOnly Property AvailableVirtualMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullAvailVirtual
                End Get
            End Property

            Friend ReadOnly Property TotalPhysicalMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullTotalPhys
                End Get
            End Property

            Friend ReadOnly Property TotalVirtualMemory() As UInt64
                Get
                    Refresh()
                    Return _memoryStatusEx.ullTotalVirtual
                End Get
            End Property

#Enable Warning IDE0049

            Private Sub Refresh()
                _memoryStatusEx = New MEMORYSTATUSEX
                _memoryStatusEx.Init()
                If Not NativeMethods.GlobalMemoryStatusEx(_memoryStatusEx) Then
                    Throw ExceptionUtils.GetWin32Exception(SR.DiagnosticInfo_Memory)
                End If
            End Sub

        End Class
    End Class
End Namespace
