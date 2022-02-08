// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    /// <summary>
    /// The ComWrappers implementation for System.Windows.Forms.Primitive's COM interop usages.
    ///
    /// Supports IStream COM interface.
    /// </summary>
    internal unsafe partial class WinFormsComWrappers : ComWrappers
    {
        private const int S_OK = (int)Interop.HRESULT.S_OK;
        private static readonly ComInterfaceEntry* s_streamEntry = InitializeIStreamEntry();
        private static readonly ComInterfaceEntry* s_fileDialogEventsEntry = InitializeIFileDialogEventsEntry();

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

            throw new NotImplementedException();
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }

        internal IntPtr GetComPointer<T>(T obj, Guid iid) where T : class
        {
            if (obj is null)
            {
                return IntPtr.Zero;
            }

            IntPtr pobj_local;
            IntPtr pUnk_local = GetOrCreateComInterfaceForObject(obj);
            Guid local_IID = iid;
            HRESULT result = (HRESULT)Marshal.QueryInterface(pUnk_local, ref local_IID, out pobj_local);
            Marshal.Release(pUnk_local);
            if (result.Failed())
            {
                Marshal.ThrowExceptionForHR((int)result);
            }

            return pobj_local;
        }

        private IntPtr GetOrCreateComInterfaceForObject(object obj)
        {
            return obj switch
            {
                ShellItemWrapper siw => siw.Instance,
                FileOpenDialogWrapper fodw => fodw.Instance,
                _ => GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None),
            };
        }
    }
}
