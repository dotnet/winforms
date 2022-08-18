// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    /// <summary>
    /// The ComWrappers implementation for System.Windows.Forms.Primitive's COM interop usages.
    ///
    /// Supports IStream COM interface.
    /// </summary>
    internal unsafe partial class WinFormsComWrappers : ComWrappers
    {
        private const int S_OK = (int)HRESULT.Values.S_OK;
        private static readonly ComInterfaceEntry* s_streamEntry = InitializeIStreamEntry();
        private static readonly ComInterfaceEntry* s_fileDialogEventsEntry = InitializeIFileDialogEventsEntry();
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

        private static ComInterfaceEntry* InitializeIFileDialogEventsEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iFileDialogEventsVtbl = IFileDialogEventsVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IFileDialogEvents;
            wrapperEntry->Vtable = iFileDialogEventsVtbl;
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
            if (obj is Interop.Ole32.IStream)
            {
                count = 1;
                return s_streamEntry;
            }

            if (obj is Interop.Shell32.IFileDialogEvents)
            {
                count = 1;
                return s_fileDialogEventsEntry;
            }

            if (obj is Ole32.IDropSource)
            {
                count = 2;
                return s_dropSourceEntry;
            }

            if (obj is Interop.Ole32.IDropTarget)
            {
                count = 1;
                return s_dropTargetEntry;
            }

            if (obj is IEnumString)
            {
                count = 1;
                return s_enumStringEntry;
            }

            if (obj is IEnumFORMATETC)
            {
                count = 1;
                return s_enumFormatEtcEntry;
            }

            if (obj is IDataObject)
            {
                count = 1;
                return s_dataObjectEntry;
            }

            throw new NotImplementedException($"ComWrappers for type {obj.GetType()} not implemented.");
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            Debug.Assert(flags == CreateObjectFlags.UniqueInstance || flags == CreateObjectFlags.None || flags == CreateObjectFlags.Unwrap);

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

            Guid fileOpenDialogIID = IID.IFileOpenDialog;
            hr = Marshal.QueryInterface(externalComObject, ref fileOpenDialogIID, out IntPtr fileOpenDialogComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new FileOpenDialogWrapper(fileOpenDialogComObject);
            }

            Guid fileSaveDialogIID = IID.IFileSaveDialog;
            hr = Marshal.QueryInterface(externalComObject, ref fileSaveDialogIID, out IntPtr fileSaveDialogComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new FileSaveDialogWrapper(fileSaveDialogComObject);
            }

            Guid lockBytesIID = IID.ILockBytes;
            hr = Marshal.QueryInterface(externalComObject, ref lockBytesIID, out IntPtr lockBytesComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new LockBytesWrapper(lockBytesComObject);
            }

            Guid shellItemIID = IID.IShellItem;
            hr = Marshal.QueryInterface(externalComObject, ref shellItemIID, out IntPtr shellItemComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new ShellItemWrapper(shellItemComObject);
            }

            Guid shellItemArrayIID = IID.IShellItemArray;
            hr = Marshal.QueryInterface(externalComObject, ref shellItemArrayIID, out IntPtr shellItemArrayComObject);
            if (hr == S_OK)
            {
                Marshal.Release(externalComObject);
                return new ShellItemArrayWrapper(shellItemArrayComObject);
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

        internal IntPtr GetComPointer<T>(T obj, Guid iid) where T : class
        {
            TryGetComPointer(obj, iid, out var comPtr).ThrowOnFailure();
            return comPtr;
        }

        internal HRESULT TryGetComPointer<T>(T? obj, Guid iid, out IntPtr comPtr) where T : class
        {
            if (obj is null)
            {
                comPtr = IntPtr.Zero;
                return HRESULT.Values.S_OK;
            }

            IntPtr pobj_local;
            IntPtr pUnk_local = GetOrCreateComInterfaceForObject(obj);
            Guid local_IID = iid;
            HRESULT result = (HRESULT)Marshal.QueryInterface(pUnk_local, ref local_IID, out pobj_local);
            Marshal.Release(pUnk_local);
            comPtr = pobj_local;
            return result;
        }

        private IntPtr GetOrCreateComInterfaceForObject(object obj)
        {
            return obj switch
            {
                ShellItemWrapper siw => siw.Instance,
                FileOpenDialogWrapper fodw => fodw.Instance,
                FileSaveDialogWrapper fsdw => fsdw.Instance,
                _ => GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None),
            };
        }
    }
}
