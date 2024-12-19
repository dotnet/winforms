' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    Partial Friend NotInheritable Class NativeTypes

#Disable Warning CA1812 ' Supress warning as this is a type used in PInvoke and shouldn't be changed.

        <StructLayout(LayoutKind.Sequential)>
        Friend NotInheritable Class SECURITY_ATTRIBUTES
            Implements IDisposable

            Friend Sub New()
                nLength = Marshal.SizeOf(Of SECURITY_ATTRIBUTES)()
            End Sub

            Public nLength As Integer
            Public lpSecurityDescriptor As IntPtr
            Public bInheritHandle As Boolean

            Public Overloads Sub Dispose() Implements IDisposable.Dispose
                If lpSecurityDescriptor <> IntPtr.Zero Then
                    UnsafeNativeMethods.LocalFree(lpSecurityDescriptor)
                    lpSecurityDescriptor = IntPtr.Zero
                End If
                GC.SuppressFinalize(Me)
            End Sub

            Protected Overrides Sub Finalize()
                Dispose()
                MyBase.Finalize()
            End Sub

        End Class
    End Class
End Namespace
