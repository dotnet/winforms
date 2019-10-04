﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    <ComVisible(False)>
    Friend NotInheritable Class NativeMethods

        <PreserveSig()>
        Friend Declare Auto Function _
            WaitForInputIdle _
                Lib "user32" (ByVal Process As NativeTypes.LateInitSafeHandleZeroOrMinusOneIsInvalid, ByVal Milliseconds As Integer) As Integer

        <PreserveSig()>
        Friend Declare Function _
            GetWindow _
                Lib "user32" (ByVal hwnd As IntPtr, ByVal wFlag As Integer) As IntPtr

        <PreserveSig()>
        Friend Declare Function _
            GetDesktopWindow _
                Lib "user32" () As IntPtr

        <DllImport("user32", CharSet:=CharSet.Auto, PreserveSig:=True, SetLastError:=True)>
        Friend Shared Function GetWindowText(ByVal hWnd As IntPtr, <Out(), MarshalAs(UnmanagedType.LPTStr)> ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
        End Function

        <PreserveSig()>
        Friend Declare Function _
            AttachThreadInput _
                Lib "user32" (ByVal idAttach As Integer, ByVal idAttachTo As Integer, ByVal fAttach As Integer) As Integer

        <PreserveSig()>
        Friend Declare Function _
            SetForegroundWindow _
                Lib "user32" (ByVal hwnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <PreserveSig()>
        Friend Declare Function _
            SetFocus _
                Lib "user32" (ByVal hwnd As IntPtr) As IntPtr

        <PreserveSig()>
        Friend Declare Auto Function _
            FindWindow _
                Lib "user32" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr

        <PreserveSig()>
        Friend Declare Function _
            CloseHandle _
                Lib "kernel32" (ByVal hObject As IntPtr) As Integer

        <PreserveSig()>
        Friend Declare Function _
            WaitForSingleObject _
                Lib "kernel32" (ByVal hHandle As NativeTypes.LateInitSafeHandleZeroOrMinusOneIsInvalid, ByVal dwMilliseconds As Integer) As Integer

        <DllImport(
             "kernel32",
             CharSet:=CharSet.Auto,
             PreserveSig:=True,
             BestFitMapping:=False,
             ThrowOnUnmappableChar:=True)>
        Friend Shared Sub GetStartupInfo(<InAttribute(), OutAttribute()> ByVal lpStartupInfo As NativeTypes.STARTUPINFO)
        End Sub

        <DllImport(
             "kernel32",
             CharSet:=CharSet.Auto,
             PreserveSig:=True,
             BestFitMapping:=False,
             ThrowOnUnmappableChar:=True)>
        Friend Shared Function CreateProcess(
            ByVal lpApplicationName As String,
            ByVal lpCommandLine As String,
            ByVal lpProcessAttributes As NativeTypes.SECURITY_ATTRIBUTES,
            ByVal lpThreadAttributes As NativeTypes.SECURITY_ATTRIBUTES,
            <MarshalAs(UnmanagedType.Bool)> ByVal bInheritHandles As Boolean,
            ByVal dwCreationFlags As Integer,
            ByVal lpEnvironment As IntPtr,
            ByVal lpCurrentDirectory As String,
            ByVal lpStartupInfo As NativeTypes.STARTUPINFO,
            ByVal lpProcessInformation As NativeTypes.PROCESS_INFORMATION) As Integer
        End Function

        ''' <summary>
        ''' Contains information about the current state of both physical and virtual memory, including extended memory.
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
            '   DWORDLONG ullAvailExtendedVirtual;  Total size of unreserved and uncommitted memory in extended portion of virual address.
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

            Friend Sub Init()
                dwLength = CType(Marshal.SizeOf(GetType(MEMORYSTATUSEX)), UInt32)
            End Sub
        End Structure

        ''' <summary>
        ''' Obtains information about the system's current usage of both physical and virtual memory.
        ''' </summary>
        ''' <param name="lpBuffer">Pointer to a MEMORYSTATUSEX structure.</param>
        ''' <returns>True if the function succeeds. Otherwise, False.</returns>
        <DllImport("Kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Friend Shared Function GlobalMemoryStatusEx(ByRef lpBuffer As MEMORYSTATUSEX) As <MarshalAsAttribute(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Adding a private constructor to prevent the compiler from generating a default constructor.
        ''' </summary>
        Private Sub New()
        End Sub

    End Class

End Namespace
