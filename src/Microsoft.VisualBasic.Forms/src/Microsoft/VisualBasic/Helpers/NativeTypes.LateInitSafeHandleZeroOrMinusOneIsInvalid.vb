' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.Win32.SafeHandles

Namespace Microsoft.VisualBasic.CompilerServices

    Partial Friend NotInheritable Class NativeTypes

        ''' <summary>
        '''  Inherits <see cref="SafeHandleZeroOrMinusOneIsInvalid"/>, with additional InitialSetHandle method.
        '''  This is required because call to constructor of SafeHandle is not allowed in constrained region.
        ''' </summary>
        Friend NotInheritable Class LateInitSafeHandleZeroOrMinusOneIsInvalid
            Inherits SafeHandleZeroOrMinusOneIsInvalid

            Friend Sub New()
                MyBase.New(True)
            End Sub

            Protected Overrides Function ReleaseHandle() As Boolean
                Return NativeMethods.CloseHandle(handle) <> 0
            End Function

            Friend Sub InitialSetHandle(h As IntPtr)
                Debug.Assert(IsInvalid, "Safe handle should only be set once.")
                SetHandle(h)
            End Sub

        End Class
    End Class
End Namespace
