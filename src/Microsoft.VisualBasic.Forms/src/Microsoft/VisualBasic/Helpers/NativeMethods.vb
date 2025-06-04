' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.VisualBasic.CompilerServices

    <ComVisible(False)>
    Friend Module NativeMethods

        <PreserveSig()>
        Friend Declare Auto Function WaitForInputIdle Lib "user32" (
            Process As NativeTypes.LateInitSafeHandleZeroOrMinusOneIsInvalid,
            Milliseconds As Integer) As Integer

        <PreserveSig()>
        Friend Declare Function GetWindow Lib "user32" (
            hwnd As IntPtr,
            wFlag As Integer) As IntPtr

        <PreserveSig()>
        Friend Declare Function GetDesktopWindow Lib "user32" () As IntPtr

#Disable Warning CA1838 ' Avoid 'StringBuilder' parameters for P/Invokes

        <DllImport(
             "kernel32",
             CharSet:=CharSet.Auto,
             PreserveSig:=True,
             BestFitMapping:=False,
             ThrowOnUnmappableChar:=True)>
        Friend Function CreateProcess(
            lpApplicationName As String,
            lpCommandLine As String,
            lpProcessAttributes As NativeTypes.SECURITY_ATTRIBUTES,
            lpThreadAttributes As NativeTypes.SECURITY_ATTRIBUTES,
            <MarshalAs(UnmanagedType.Bool)> bInheritHandles As Boolean,
            dwCreationFlags As Integer,
            lpEnvironment As IntPtr,
            lpCurrentDirectory As String,
            lpStartupInfo As NativeTypes.STARTUPINFO,
            lpProcessInformation As NativeTypes.PROCESS_INFORMATION) As Integer
        End Function

        <DllImport(
             "kernel32",
             CharSet:=CharSet.Auto,
             PreserveSig:=True,
             BestFitMapping:=False,
             ThrowOnUnmappableChar:=True)>
        Friend Sub GetStartupInfo(<[In](), Out()> lpStartupInfo As NativeTypes.STARTUPINFO)
        End Sub

        <DllImport("user32", CharSet:=CharSet.Auto, PreserveSig:=True, SetLastError:=True)>
        Friend Function GetWindowText(
            hWnd As IntPtr,
            <Out(),
            MarshalAs(UnmanagedType.LPTStr)> lpString As StringBuilder,
            nMaxCount As Integer) As Integer
#Enable Warning CA1838
        End Function

        <PreserveSig()>
        Friend Declare Function AttachThreadInput Lib "user32" (
            idAttach As Integer,
            idAttachTo As Integer,
            fAttach As Integer) As Integer

        <PreserveSig()>
        Friend Declare Function SetForegroundWindow Lib "user32" (hwnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <PreserveSig()>
        Friend Declare Function SetFocus Lib "user32" (hwnd As IntPtr) As IntPtr

        <PreserveSig()>
        Friend Declare Auto Function FindWindow Lib "user32" (
            lpClassName As String,
            lpWindowName As String) As IntPtr

        <PreserveSig()>
        Friend Declare Function CloseHandle Lib "kernel32" (hObject As IntPtr) As Integer

        <PreserveSig()>
        Friend Declare Function WaitForSingleObject Lib "kernel32" (
            hHandle As NativeTypes.LateInitSafeHandleZeroOrMinusOneIsInvalid,
            dwMilliseconds As Integer) As Integer

#Disable Warning IDE0049  ' Use language keywords instead of framework type names for type references, Justification:=<Types come from Windows Native API>
#Disable Warning IDE1006 ' Naming Styles, Justification:=<Names come from Windows Native API>

        ''' <summary>
        '''  Obtains information about the system's current usage of both physical and virtual memory.
        ''' </summary>
        ''' <param name="lpBuffer">Pointer to a <see cref="MEMORYSTATUSEX"/> structure.</param>
        ''' <returns><see langword="True"/> if the function succeeds. Otherwise, <see langword="False"/>.</returns>
        <DllImport("Kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Friend Function GlobalMemoryStatusEx(ByRef lpBuffer As MEMORYSTATUSEX) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        '''  Contains information about the current state of both physical and virtual memory, including extended memory.
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Friend Structure MEMORYSTATUSEX
            'typedef struct _MEMORYSTATUSEX {
            '   DWORD dwLength;                     Size of the structure. Must set before calling GlobalMemoryStatusEx.
            '   DWORD dwMemoryLoad;                 Number between 0 and 100 on current memory utilization.
            '   DWORDLONG ullTotalPhys;             Total size of physical memory.
            '   DWORDLONG ullAvailPhys;             Total size of available physical memory.
            '   DWORDLONG ullTotalPageFile;         Size of committed memory limit.
            '   DWORDLONG ullAvailPageFile;         Size of available memory to committed (ullTotalPageFile max).
            '   DWORDLONG ullTotalVirtual;          Total size of user potion of virtual address space of calling process.
            '   DWORDLONG ullAvailVirtual;          Total size of unreserved and uncommitted memory in virtual address space.
            '   DWORDLONG ullAvailExtendedVirtual;  Total size of unreserved and uncommitted memory in extended portion of virtual address.
            '} MEMORYSTATUSEX, *LPMEMORYSTATUSEX;

            Friend dwLength As UInt32
            Friend dwMemoryLoad As UInt32
            Friend ullTotalPhys As UInt64
            Friend ullAvailPhys As UInt64
            Friend ullTotalPageFile As UInt64
            Friend ullAvailPageFile As UInt64
            Friend ullTotalVirtual As UInt64
            Friend ullAvailVirtual As UInt64
            Friend ullAvailExtendedVirtual As UInt64
#Enable Warning IDE1006

            Friend Sub Init()
                dwLength = CType(Marshal.SizeOf(Of MEMORYSTATUSEX)(), UInt32)
            End Sub

        End Structure
#Enable Warning IDE0049

    End Module
End Namespace
