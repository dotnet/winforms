' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    <ComVisible(False)>
    Friend Module SafeNativeMethods

        <PreserveSig()>
        Friend Declare Function IsWindowEnabled Lib "user32" (hwnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <PreserveSig()>
        Friend Declare Function IsWindowVisible Lib "user32" (hwnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <PreserveSig()>
        Friend Declare Function GetWindowThreadProcessId Lib "user32" (hwnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer

    End Module
End Namespace
