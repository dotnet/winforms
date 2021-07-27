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
        private static readonly ComInterfaceEntry* s_wrapperEntry = InitializeComInterfaceEntry();

        internal static WinFormsComWrappers Instance { get; } = new WinFormsComWrappers();

        private WinFormsComWrappers() { }

        private static ComInterfaceEntry* InitializeComInterfaceEntry()
        {
            GetIUnknownImpl(out IntPtr fpQueryInterface, out IntPtr fpAddRef, out IntPtr fpRelease);

            IntPtr iStreamVtbl = IStreamVtbl.Create(fpQueryInterface, fpAddRef, fpRelease);

            ComInterfaceEntry* wrapperEntry = (ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(WinFormsComWrappers), sizeof(ComInterfaceEntry));
            wrapperEntry->IID = IID.IStream;
            wrapperEntry->Vtable = iStreamVtbl;
            return wrapperEntry;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            Debug.Assert(obj is Interop.Ole32.IStream);
            Debug.Assert(s_wrapperEntry is not null);

            // Always return the same table mappings.
            count = 1;
            return s_wrapperEntry;
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            Debug.Assert(flags == CreateObjectFlags.UniqueInstance);

            Guid pictureIID = IID.IPicture;
            int hr = Marshal.QueryInterface(externalComObject, ref pictureIID, out IntPtr pictureComObject);
            if (hr == S_OK)
            {
                return new PictureWrapper(pictureComObject);
            }

            Guid fileOpenDialogIID = IID.IFileOpenDialog;
            hr = Marshal.QueryInterface(externalComObject, ref fileOpenDialogIID, out IntPtr fileOpenDialogComObject);
            if (hr == S_OK)
            {
                return new FileOpenDialogWrapper(fileOpenDialogComObject);
            }

            Guid shellItemIID = IID.IShellItem;
            hr = Marshal.QueryInterface(externalComObject, ref shellItemIID, out IntPtr shellItemComObject);
            if (hr == S_OK)
            {
                return new ShellItemWrapper(shellItemComObject);
            }

            throw new NotImplementedException();
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
            throw new NotImplementedException();
        }
    }
}
