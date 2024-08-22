' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    <ComVisible(False)>
    Friend Module UnsafeNativeMethods

        ''' <summary>
        '''  Used to determine how much free space is on a disk.
        ''' </summary>
        ''' <param name="Directory">Path including drive we're getting information about.</param>
        ''' <param name="UserSpaceFree">The amount of free space available to the current user.</param>
        ''' <param name="TotalUserSpace">The total amount of space on the disk relative to the current user.</param>
        ''' <param name="TotalFreeSpace">The amount of free space on the disk.</param>
        ''' <returns><see langword="True"/> if function succeeds in getting info otherwise <see langword="False"/>.</returns>
        <DllImport("Kernel32.dll", CharSet:=CharSet.Auto, BestFitMapping:=False, SetLastError:=True)>
        Friend Function GetDiskFreeSpaceEx(
            Directory As String,
            ByRef UserSpaceFree As Long,
            ByRef TotalUserSpace As Long,
            ByRef TotalFreeSpace As Long) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        '''  Gets the state of the specified key on the keyboard when the function
        '''  is called.
        ''' </summary>
        ''' <param name="KeyCode">Integer representing the key in question.</param>
        ''' <returns>
        '''  The high order byte is 1 if the key is down. The low order byte is one
        '''  if the key is toggled on (i.e. for keys like CapsLock)
        ''' </returns>
        <DllImport("User32.dll", ExactSpelling:=True, CharSet:=CharSet.Auto)>
        Friend Function GetKeyState(KeyCode As Integer) As Short
        End Function

        ''' <summary>
        '''  Frees memory allocated from the local heap. i.e. frees memory allocated
        '''  by LocalAlloc or LocalReAlloc.
        ''' </summary>
        <DllImport("kernel32", ExactSpelling:=True, SetLastError:=True)>
        Friend Function LocalFree(LocalHandle As IntPtr) As IntPtr
        End Function

    End Module
End Namespace
