' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    <ComVisible(False)>
    Friend NotInheritable Class _
        SafeNativeMethods

        <PreserveSig()> Friend Declare Function _
            IsWindowEnabled _
                Lib "user32" (hwnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <PreserveSig()> Friend Declare Function _
            IsWindowVisible _
                Lib "user32" (hwnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <PreserveSig()> Friend Declare Function _
            GetWindowThreadProcessId _
                Lib "user32" (hwnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer

        ''' <summary>
        ''' Adding a private constructor to prevent the compiler from generating a default constructor.
        ''' </summary>
        Private Sub New()
        End Sub
    End Class

End Namespace

