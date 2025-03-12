// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IUnknown
{
    /// <summary>
    ///  Manual COM Callable Wrapper for <see cref="IUnknown"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is for test and debug scenarios only. It should not be used directly in the product.
    ///  </para>
    /// </remarks>
    /// <devdoc>
    ///  This is a simplified version of what <see cref="ComWrappers"/> does. It is useful when we want to manage
    ///  our own <see cref="IUnknown.QueryInterface(Guid*, void**)"/> handling for debugging and testing purposes.
    /// </devdoc>
    internal static class CCW
    {
        private static readonly Vtbl* s_vtable = AllocateVTable();

        private static unsafe Vtbl* AllocateVTable()
        {
            // Allocate and create a singular VTable for this type projection.
            Vtbl* vtable = (Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CCW), sizeof(Vtbl));

            // IUnknown
            vtable->QueryInterface_1 = &QueryInterface;
            vtable->AddRef_2 = &AddRef;
            vtable->Release_3 = &Release;
            return vtable;
        }

        /// <inheritdoc cref="CCW"/>
        /// <summary>
        ///  Creates a manual COM Callable Wrapper for the given <paramref name="object"/>.
        /// </summary>
        public static unsafe IUnknown* Create(Interface @object) =>
            (IUnknown*)Lifetime<Vtbl, Interface>.Allocate(@object, s_vtable);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT QueryInterface(IUnknown* @this, Guid* iid, void* ppObject)
        {
            if (iid is null || ppObject is null)
            {
                return HRESULT.E_POINTER;
            }

            if (iid->Equals(IID_Guid))
            {
                ppObject = @this;
            }
            else
            {
                ppObject = null;
                return HRESULT.E_NOINTERFACE;
            }

            Lifetime<Vtbl, Interface>.AddRef(@this);
            return HRESULT.S_OK;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe uint AddRef(IUnknown* @this) => Lifetime<Vtbl, Interface>.AddRef(@this);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe uint Release(IUnknown* @this) => Lifetime<Vtbl, Interface>.Release(@this);
    }
}
