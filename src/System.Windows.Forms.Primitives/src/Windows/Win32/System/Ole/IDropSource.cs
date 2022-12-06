// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.SystemServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct IDropSource : IVTable<IDropSource, IDropSource.Vtbl>
{
    static void IVTable<IDropSource, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
    {
        vtable->QueryContinueDrag_4 = &QueryContinueDrag;
        vtable->GiveFeedback_5 = &GiveFeedback;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT QueryContinueDrag(IDropSource* @this, BOOL fEscapePressed, MODIFIERKEYS_FLAGS grfKeyState)
        => ComWrappers.UnwrapAndInvoke<IDropSource, Interface>(@this, o => o.QueryContinueDrag(fEscapePressed, grfKeyState));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT GiveFeedback(IDropSource* @this, DROPEFFECT dwEffect)
        => ComWrappers.UnwrapAndInvoke<IDropSource, Interface>(@this, o => o.GiveFeedback(dwEffect));
}
