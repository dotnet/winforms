// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IDropSourceNotifyVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDropSourceNotifyVtbl), IntPtr.Size * 5);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, HRESULT>)&DragEnterTarget;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, HRESULT>)&DragLeaveTarget;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT DragEnterTarget(IntPtr thisPtr, IntPtr hwndTarget)
            {
                var inst = ComInterfaceDispatch.GetInstance<Ole32.IDropSourceNotify>((ComInterfaceDispatch*)thisPtr);
                return inst.DragEnterTarget(hwndTarget);
            }

            [UnmanagedCallersOnly]
            private static HRESULT DragLeaveTarget(IntPtr thisPtr)
            {
                var inst = ComInterfaceDispatch.GetInstance<Ole32.IDropSourceNotify>((ComInterfaceDispatch*)thisPtr);
                return inst.DragLeaveTarget();
            }
        }
    }
}
