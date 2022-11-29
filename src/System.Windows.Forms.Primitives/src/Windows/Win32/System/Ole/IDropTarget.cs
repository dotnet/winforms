// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.SystemServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct IDropTarget : IPopulateVTable<IDropTarget.Vtbl>
{
    public static void PopulateVTable(Vtbl* vtable)
    {
        vtable->DragEnter_4 = &DragEnter;
        vtable->DragOver_5 = &DragOver;
        vtable->DragLeave_6 = &DragLeave;
        vtable->Drop_7 = &Drop;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT DragEnter(IDropTarget* @this, IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
        => ComWrappers.UnwrapAndInvoke<IDropTarget, Interface>(@this, o => o.DragEnter(pDataObj, grfKeyState, pt, pdwEffect));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT DragOver(IDropTarget* @this, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
        => ComWrappers.UnwrapAndInvoke<IDropTarget, Interface>(@this, o => o.DragOver(grfKeyState, pt, pdwEffect));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT DragLeave(IDropTarget* @this)
        => ComWrappers.UnwrapAndInvoke<IDropTarget, Interface>(@this, o => o.DragLeave());

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Drop(IDropTarget* @this, IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
        => ComWrappers.UnwrapAndInvoke<IDropTarget, Interface>(@this, o => o.Drop(pDataObj, grfKeyState, pt, pdwEffect));
}
