// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using ComTypes = System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    /// <summary>
    ///  The ComWrappers implementation for System.Windows.Forms.Primitive's COM interop usages.
    /// </summary>
    internal unsafe partial class WinFormsComWrappers : ComWrappers
    {
        private const int S_OK = 0;
        private static readonly ComInterfaceEntry* s_enumFormatEtcEntry = IEnumFORMATETCVtbl.InitializeEntry();
        private static readonly ComInterfaceEntry* s_dropSourceEntry = IDropSourceVtbl.InitializeEntry();
        private static readonly ComInterfaceEntry* s_dataObjectEntry = IDataObjectVtbl.InitializeEntry();

        internal static WinFormsComWrappers Instance { get; } = new WinFormsComWrappers();

        private WinFormsComWrappers() { }

        internal static void PopulateIUnknownVTable(IUnknown.Vtbl* unknown)
        {
            GetIUnknownImpl(out nint fpQueryInterface, out nint fpAddRef, out nint fpRelease);
            unknown->QueryInterface_1 = (delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HRESULT>)fpQueryInterface;
            unknown->AddRef_2 = (delegate* unmanaged[Stdcall]<IUnknown*, uint>)fpAddRef;
            unknown->Release_3 = (delegate* unmanaged[Stdcall]<IUnknown*, uint>)fpRelease;
        }

        /// <summary>
        ///  Check to see if the given object is supported by our ComWrappers implementation.
        /// </summary>
        internal static bool IsSupportedObject(object obj)
        {
            // This maps to what we're currently doing in ComputeVtables. We currently presume only one match, so
            // if we see that we get multiple matches we're going to not claim it as supported.

            // We need to figure out a more direct way of tying objects to our ComWrappers implementation. Going by
            // interface is fragile unless we check the object for all supported interfaces and dynamically build
            // the ComInterfaceEntry table for it. This seems slow, perhaps we need some sort of deliberate interface
            // on our classes we're exposing to COM to give the data?

            int count = 0;

            if (obj is IStream.Interface)
            {
                count++;
            }

            if (obj is IFileDialogEvents.Interface)
            {
                count++;
            }

            if (obj is Ole32.IDropSource)
            {
                count++;
            }

            if (obj is IDropTarget.Interface)
            {
                count++;
            }

            if (obj is IEnumString.Interface)
            {
                count++;
            }

            if (obj is ComTypes.IEnumFORMATETC)
            {
                count++;
            }

            if (obj is ComTypes.IDataObject)
            {
                count++;
            }

            Debug.Assert(count < 2);
            return count == 1;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            if (obj is Ole32.IDropSource)
            {
                count = 2;
                return s_dropSourceEntry;
            }

            if (obj is ComTypes.IEnumFORMATETC)
            {
                count = 1;
                return s_enumFormatEtcEntry;
            }

            if (obj is ComTypes.IDataObject)
            {
                count = 1;
                return s_dataObjectEntry;
            }

            if (obj is not IManagedWrapper vtables)
            {
                Debug.Fail("object does not implement IManagedWrapper");
                count = 0;
                return null;
            }

            ComInterfaceTable table = vtables.GetComInterfaceTable();
            count = table.Count;

            return table.Entries;
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            Debug.Assert(flags == CreateObjectFlags.UniqueInstance
                || flags == CreateObjectFlags.None
                || flags == CreateObjectFlags.Unwrap);

            int hr = Marshal.QueryInterface(externalComObject, ref IID.GetRef<IErrorInfo>(), out IntPtr errorInfoComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new ErrorInfoWrapper(errorInfoComObject);
            }

            hr = Marshal.QueryInterface(externalComObject, ref IID.GetRef<IEnumFORMATETC>(), out IntPtr enumFormatEtcComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new EnumFORMATETCWrapper(enumFormatEtcComObject);
            }

            hr = Marshal.QueryInterface(externalComObject, ref IID.GetRef<IEnumVARIANT>(), out IntPtr enumVariantComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new EnumVariantWrapper(enumVariantComObject);
            }

            throw new NotImplementedException();
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  For the given <paramref name="this"/> pointer unwrap the associated managed object and use it to
        ///  invoke <paramref name="func"/>.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   Handles exceptions and converts to <see cref="HRESULT"/>.
        ///  </para>
        /// </remarks>
        internal static HRESULT UnwrapAndInvoke<TThis, TInterface>(TThis* @this, Func<TInterface, HRESULT> func)
            where TThis : unmanaged
            where TInterface : class
        {
            try
            {
                TInterface? @object = ComInterfaceDispatch.GetInstance<TInterface>((ComInterfaceDispatch*)@this);
                return @object is null ? HRESULT.COR_E_OBJECTDISPOSED : func(@object);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
