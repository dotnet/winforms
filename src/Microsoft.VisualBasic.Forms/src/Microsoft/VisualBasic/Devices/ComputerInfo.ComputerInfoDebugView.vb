' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Devices

    Partial Public Class ComputerInfo

        ''' <summary>
        '''  Debugger proxy for the <see cref="ComputerInfo"/> class. The problem
        '''  is that OSFullName can time out the debugger so we offer a view
        '''  that doesn't have that field.
        ''' </summary>
        Friend NotInheritable Class ComputerInfoDebugView

            <DebuggerBrowsable(DebuggerBrowsableState.Never)>
            Private ReadOnly _instanceBeingWatched As ComputerInfo

            Public Sub New(RealClass As ComputerInfo)
                _instanceBeingWatched = RealClass
            End Sub

#Disable Warning IDE0049  ' Use language keywords instead of framework type names for type references, Justification:=<Public API>

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property AvailablePhysicalMemory() As UInt64
                Get
                    Return _instanceBeingWatched.AvailablePhysicalMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property AvailableVirtualMemory() As UInt64
                Get
                    Return _instanceBeingWatched.AvailableVirtualMemory
                End Get
            End Property

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

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property TotalPhysicalMemory() As UInt64
                Get
                    Return _instanceBeingWatched.TotalPhysicalMemory
                End Get
            End Property

            <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
            Public ReadOnly Property TotalVirtualMemory() As UInt64
                Get
                    Return _instanceBeingWatched.TotalVirtualMemory
                End Get
            End Property

#Enable Warning IDE0049

        End Class
    End Class
End Namespace
