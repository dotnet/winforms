// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IDropSourceNotifyVtbl
        {
            public static void PopulateVTable(IDropSourceNotify.Vtbl* vtable)
            {
                vtable->DragEnterTarget_4 = &DragEnterTarget;
                vtable->DragLeaveTarget_5 = &DragLeaveTarget;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT DragEnterTarget(IDropSourceNotify* @this, HWND hwndTarget)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<Ole32.IDropSourceNotify>((ComInterfaceDispatch*)@this);
                    return instance.DragEnterTarget(hwndTarget);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT DragLeaveTarget(IDropSourceNotify* @this)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<Ole32.IDropSourceNotify>((ComInterfaceDispatch*)@this);
                    return instance.DragLeaveTarget();
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
