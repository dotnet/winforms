// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;
using Windows.Win32.System.SystemServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IDropSourceVtbl
        {
            private static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IDropSource.Vtbl* vtblRaw = (IDropSource.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDropSourceVtbl), sizeof(IDropSource.Vtbl));
                vtblRaw->QueryInterface_1 = (delegate* unmanaged[Stdcall]<IDropSource*, Guid*, void**, HRESULT>)fpQueryInterface;
                vtblRaw->AddRef_2 = (delegate* unmanaged[Stdcall]<IDropSource*, uint>)fpAddRef;
                vtblRaw->Release_3 = (delegate* unmanaged[Stdcall]<IDropSource*, uint>)fpRelease;
                vtblRaw->QueryContinueDrag_4 = &QueryContinueDrag;
                vtblRaw->GiveFeedback_5 = &GiveFeedback;

                return (IntPtr)vtblRaw;
            }

            internal static ComInterfaceEntry* InitializeEntry()
            {
                GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

                IntPtr iDropSourceVtbl = Create(fpQueryInterface, fpAddRef, fpRelease);
                IntPtr iDropSourceNotifyVtbl = IDropSourceNotifyVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

                ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry) * 2);
                wrapperEntry[0].IID = *IID.Get<IDropSource>();
                wrapperEntry[0].Vtable = iDropSourceVtbl;
                wrapperEntry[1].IID = *IID.Get<IDropSourceNotify>();
                wrapperEntry[1].Vtable = iDropSourceNotifyVtbl;
                return wrapperEntry;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT QueryContinueDrag(IDropSource* @this, BOOL fEscapePressed, MODIFIERKEYS_FLAGS grfKeyState)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<Ole32.IDropSource>((ComInterfaceDispatch*)@this);
                    return instance.QueryContinueDrag(fEscapePressed, (User32.MK)grfKeyState);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT GiveFeedback(IDropSource* @this, DROPEFFECT dwEffect)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<Ole32.IDropSource>((ComInterfaceDispatch*)@this);
                    return instance.GiveFeedback((Ole32.DROPEFFECT)dwEffect);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
