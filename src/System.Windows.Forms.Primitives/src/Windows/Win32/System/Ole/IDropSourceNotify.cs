// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct IDropSourceNotify : IVTable<IDropSourceNotify, IDropSourceNotify.Vtbl>
{
    static void IVTable<IDropSourceNotify, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
    {
        vtable->DragEnterTarget_4 = &DragEnterTarget;
        vtable->DragLeaveTarget_5 = &DragLeaveTarget;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT DragEnterTarget(IDropSourceNotify* @this, HWND hwndTarget)
        => ComWrappers.UnwrapAndInvoke<IDropSourceNotify, Interface>(@this, o => o.DragEnterTarget(hwndTarget));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT DragLeaveTarget(IDropSourceNotify* @this)
        => ComWrappers.UnwrapAndInvoke<IDropSourceNotify, Interface>(@this, o => o.DragLeaveTarget());
}
