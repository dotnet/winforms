' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    Partial Friend NotInheritable Class NativeTypes

        ''' <summary>
        '''  Represent Win32 PROCESS_INFORMATION structure. IMPORTANT: Copy the handles to a SafeHandle before use them.
        ''' </summary>
        ''' <remarks>
        '''  The handles in PROCESS_INFORMATION are initialized in unmanaged function.
        '''  We can't use SafeHandle here because Interop doesn't support [out] SafeHandles in structure / classes yet.
        '''  This class makes no attempt to free the handles. To use the handle, first copy it to a SafeHandle class
        '''  (using LateInitSafeHandleZeroOrMinusOneIsInvalid.InitialSetHandle) to correctly use and dispose the handle.
        ''' </remarks>
        <StructLayout(LayoutKind.Sequential)>
        Friend NotInheritable Class PROCESS_INFORMATION
            Public hProcess As IntPtr = IntPtr.Zero
            Public hThread As IntPtr = IntPtr.Zero
            Public dwProcessId As Integer
            Public dwThreadId As Integer

            Friend Sub New()
            End Sub

        End Class
    End Class
End Namespace
