// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Com;

internal partial class Interop
{
    /// <summary>
    ///  The ComWrappers implementation for System.Windows.Forms.Primitive's COM interop usages.
    ///
    ///  Supports IStream COM interface.
    /// </summary>
    internal unsafe partial class WinFormsComWrappers : ComWrappers
    {
        private const int S_OK = 0;
        private static readonly ComInterfaceEntry* s_streamEntry = InitializeIStreamEntry();
        private static readonly ComInterfaceEntry* s_fileDialogEventsEntry = InitializeEntry<IFileDialogEvents, IFileDialogEvents.Vtbl>();
        private static readonly ComInterfaceEntry* s_enumStringEntry = InitializeIEnumStringEntry();
        private static readonly ComInterfaceEntry* s_enumFormatEtcEntry = InitializeIEnumFORMATETCEntry();
        private static readonly ComInterfaceEntry* s_dropSourceEntry = InitializeIDropSourceEntry();
        private static readonly ComInterfaceEntry* s_dropTargetEntry = InitializeIDropTargetEntry();
        private static readonly ComInterfaceEntry* s_dataObjectEntry = InitializeIDataObjectEntry();

        internal static WinFormsComWrappers Instance { get; } = new WinFormsComWrappers();

        private WinFormsComWrappers() { }

        private static ComInterfaceEntry* InitializeIStreamEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iStreamVtbl = IStreamVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IStream;
            wrapperEntry->Vtable = iStreamVtbl;
            return wrapperEntry;
        }

        private static ComInterfaceEntry* InitializeEntry<TComInterface, TVTable>()
            where TComInterface : unmanaged, IPopulateVTable<TVTable>, INativeGuid
            where TVTable : unmanaged
        {
            TVTable* vtable = (TVTable*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(TComInterface), sizeof(TVTable));

            IUnknown.Vtbl* unknown = (IUnknown.Vtbl*)vtable;

            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);
            unknown->QueryInterface_1 = (delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HRESULT>)fpQueryInterface;
            unknown->AddRef_2 = (delegate* unmanaged[Stdcall]<IUnknown*, uint>)fpAddRef;
            unknown->Release_3 = (delegate* unmanaged[Stdcall]<IUnknown*, uint>)fpRelease;

            TComInterface.PopulateVTable(vtable);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = *TComInterface.NativeGuid;
            wrapperEntry->Vtable = (nint)(void*)vtable;
            return wrapperEntry;
        }

        private static ComInterfaceEntry* InitializeIEnumStringEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iEnumStringVtbl = IEnumStringVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IEnumString;
            wrapperEntry->Vtable = iEnumStringVtbl;
            return wrapperEntry;
        }

        private static ComInterfaceEntry* InitializeIEnumFORMATETCEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iEnumFormatCVtbl = IEnumFORMATETCVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IEnumFORMATETC;
            wrapperEntry->Vtable = iEnumFormatCVtbl;
            return wrapperEntry;
        }

        private static ComInterfaceEntry* InitializeIDropSourceEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iDropSourceVtbl = IDropSourceVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);
            IntPtr iDropSourceNotifyVtbl = IDropSourceNotifyVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry) * 2);
            wrapperEntry[0].IID = IID.IDropSource;
            wrapperEntry[0].Vtable = iDropSourceVtbl;
            wrapperEntry[1].IID = IID.IDropSourceNotify;
            wrapperEntry[1].Vtable = iDropSourceNotifyVtbl;
            return wrapperEntry;
        }

        private static ComInterfaceEntry* InitializeIDropTargetEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iDropTargetVtbl = IDropTargetVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IDropTarget;
            wrapperEntry->Vtable = iDropTargetVtbl;
            return wrapperEntry;
        }

        private static ComInterfaceEntry* InitializeIDataObjectEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iDataObjectVtbl = IDataObjectVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IDataObject;
            wrapperEntry->Vtable = iDataObjectVtbl;
            return wrapperEntry;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            if (obj is Ole32.IStream)
            {
                count = 1;
                return s_streamEntry;
            }

            if (obj is IFileDialogEvents.Interface)
            {
                count = 1;
                return s_fileDialogEventsEntry;
            }

            if (obj is Ole32.IDropSource)
            {
                count = 2;
                return s_dropSourceEntry;
            }

            if (obj is Ole32.IDropTarget)
            {
                count = 1;
                return s_dropTargetEntry;
            }

            if (obj is ComTypes.IEnumString)
            {
                count = 1;
                return s_enumStringEntry;
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

            throw new NotImplementedException($"ComWrappers for type {obj.GetType()} not implemented.");
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            Debug.Assert(flags == CreateObjectFlags.UniqueInstance
                || flags == CreateObjectFlags.None
                || flags == CreateObjectFlags.Unwrap);

            Guid pictureIID = IID.IPicture;
            int hr = Marshal.QueryInterface(externalComObject, ref pictureIID, out IntPtr pictureComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new PictureWrapper(pictureComObject);
            }

            Guid errorInfoIID = IID.IErrorInfo;
            hr = Marshal.QueryInterface(externalComObject, ref errorInfoIID, out IntPtr errorInfoComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new ErrorInfoWrapper(errorInfoComObject);
            }

            Guid enumFormatEtcIID = IID.IEnumFORMATETC;
            hr = Marshal.QueryInterface(externalComObject, ref enumFormatEtcIID, out IntPtr enumFormatEtcComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new EnumFORMATETCWrapper(enumFormatEtcComObject);
            }

            Guid lockBytesIID = IID.ILockBytes;
            hr = Marshal.QueryInterface(externalComObject, ref lockBytesIID, out IntPtr lockBytesComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new LockBytesWrapper(lockBytesComObject);
            }

            Guid autoCompleteIID = IID.IAutoComplete2;
            hr = Marshal.QueryInterface(externalComObject, ref autoCompleteIID, out IntPtr autoCompleteComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new AutoCompleteWrapper(autoCompleteComObject);
            }

            Guid enumVariantIID = IID.IEnumVariant;
            hr = Marshal.QueryInterface(externalComObject, ref enumVariantIID, out IntPtr enumVariantComObject);
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
        ///  Attempts to get the specified <paramref name="iid"/> interface for the given <paramref name="obj"/>.
        /// </summary>
        internal bool TryGetComPointer<T>(object? obj, in Guid iid, out T* ppvObject) where T : unmanaged
        {
            ppvObject = null;

            if (obj is null)
            {
                return false;
            }

            // Get the CCW and attempt to get the requested interface.
            IUnknown* ccw = GetOrCreateComInterfaceForObject(obj);
            if (ccw is null)
            {
                return false;
            }

            HRESULT result = ccw->QueryInterface(in iid, out void* unknown);
            ppvObject = (T*)unknown;
            ccw->Release();

            return result == HRESULT.S_OK;
        }

        private IUnknown* GetOrCreateComInterfaceForObject(object obj)
        {
            return obj switch
            {
                _ => (IUnknown*)GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None),
            };
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
