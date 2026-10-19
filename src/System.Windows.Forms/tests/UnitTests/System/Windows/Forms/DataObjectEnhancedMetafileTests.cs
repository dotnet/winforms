// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests;

// Keep WinForms' public EMF behavior unchanged while the shared OLE layer enables WPF rendering.
[Collection(nameof(EnhancedMetafileCollection))]
public class DataObjectEnhancedMetafileTests
{
    // Clipboard contents are global; these cases must not overlap other test collections.
    [CollectionDefinition(nameof(EnhancedMetafileCollection), DisableParallelization = true)]
    public class EnhancedMetafileCollection
    {
    }

    // Exercise both supported managed payload shapes: the original stream and a System.Drawing Metafile.
    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void DataObject_EnhancedMetafile_PreservesManagedDataButRejectsNativeMedium(bool useMetafile)
    {
        using MemoryStream stream = CreateMetafileStream();
        using Metafile? metafile = useMetafile ? new Metafile(stream) : null;
        object payload = (object?)metafile ?? stream;
        DataObject dataObject = new(DataFormats.EnhancedMetafile, payload);

        // Managed storage preserves the original payload even though native EMF rendering is unsupported.
        Assert.True(dataObject.GetDataPresent(DataFormats.EnhancedMetafile, autoConvert: false));
#pragma warning disable WFDEV005 // Capture the .NET 9 public API contract.
        Assert.Same(payload, dataObject.GetData(DataFormats.EnhancedMetafile, autoConvert: false));
#pragma warning restore WFDEV005
        Assert.Equal<string>([DataFormats.EnhancedMetafile], dataObject.GetFormats(autoConvert: false));

        IComDataObject comDataObject = dataObject;
        FORMATETC[] formats = new FORMATETC[1];
        int[] fetched = new int[1];
        Assert.Equal(0, comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET).Next(1, formats, fetched));
        Assert.Equal(1, fetched[0]);
        Assert.Equal((short)DataFormats.GetFormat(DataFormats.EnhancedMetafile).Id, formats[0].cfFormat);
        // .NET 9 advertises HGLOBAL for this format; native ENHMF requests must still fail below.
        Assert.Equal(TYMED.TYMED_HGLOBAL, formats[0].tymed);

        // All COM retrieval paths must reject TYMED_ENHMF, despite retaining the format as managed data.
        FORMATETC format = CreateFormat(TYMED.TYMED_ENHMF);
        Assert.Equal((int)HRESULT.DV_E_TYMED, comDataObject.QueryGetData(ref format));
        Assert.Equal((int)HRESULT.DV_E_TYMED, Assert.Throws<COMException>(() => comDataObject.GetData(ref format, out _)).HResult);

        STGMEDIUM medium = new() { tymed = TYMED.TYMED_ENHMF };
        Assert.Equal((int)HRESULT.DV_E_TYMED, Assert.Throws<COMException>(() => comDataObject.GetDataHere(ref format, ref medium)).HResult);
    }

    // Verify that a stream payload is transferred as its bytes in HGLOBAL storage, not as an EMF handle.
    [WinFormsTheory]
    [InlineData(TYMED.TYMED_HGLOBAL)]
    [InlineData(TYMED.TYMED_HGLOBAL | TYMED.TYMED_ENHMF)]
    public unsafe void DataObject_EnhancedMetafileStream_GetDataReturnsHGlobalBytes(TYMED tymed)
    {
        using MemoryStream stream = CreateMetafileStream();
        IComDataObject dataObject = new DataObject(DataFormats.EnhancedMetafile, stream);
        FORMATETC format = CreateFormat(tymed);
        // A mixed mask must still select HGLOBAL rather than opt WinForms into native ENHMF rendering.
        Assert.Equal(0, dataObject.QueryGetData(ref format));

        dataObject.GetData(ref format, out STGMEDIUM medium);
        try
        {
            Assert.Equal(TYMED.TYMED_HGLOBAL, medium.tymed);
            HGLOBAL handle = (HGLOBAL)medium.unionmember;
            Assert.True(PInvokeCore.GlobalSize(handle) >= (nuint)stream.Length);
            IntPtr pointer = (IntPtr)PInvokeCore.GlobalLock(handle);
            Assert.NotEqual(IntPtr.Zero, pointer);
            try
            {
                byte[] actual = new byte[stream.Length];
                Marshal.Copy(pointer, actual, 0, actual.Length);
                Assert.Equal(stream.ToArray(), actual);
            }
            finally
            {
                PInvokeCore.GlobalUnlock(handle);
            }
        }
        finally
        {
            // Unlocking only releases the memory lock; ReleaseStgMedium frees the returned HGLOBAL.
            ReleaseStgMedium(ref medium);
            GC.KeepAlive(dataObject);
        }
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void Clipboard_SetDataObject_EnhancedMetafile_DoesNotExportNativeHandle(bool useMetafile, bool copy)
    {
        using MemoryStream stream = CreateMetafileStream();
        using Metafile? metafile = useMetafile ? new Metafile(stream) : null;
        DataObject dataObject = new(DataFormats.EnhancedMetafile, (object?)metafile ?? stream);
        dataObject.SetText("Enhanced metafile control");

        try
        {
            // Cover delayed rendering and flushing: WinForms preserves the managed format even though it does not
            // publish the payload as a native enhanced-metafile clipboard handle.
            Clipboard.SetDataObject(dataObject, copy);
            Assert.Equal("Enhanced metafile control", Clipboard.GetText());
            Assert.True(Clipboard.ContainsData(DataFormats.EnhancedMetafile));

            // Other processes may briefly hold the clipboard open.
            Assert.True(SpinWait.SpinUntil(() => OpenClipboard(IntPtr.Zero), TimeSpan.FromSeconds(5)));
            try
            {
                // Managed format discovery succeeds above, but the native CF_ENHMETAFILE handle is not exported.
                Assert.Equal(IntPtr.Zero, GetClipboardData((uint)DataFormats.GetFormat(DataFormats.EnhancedMetafile).Id));
            }
            finally
            {
                Assert.True(CloseClipboard());
            }
        }
        finally
        {
            Clipboard.Clear();
            GC.KeepAlive(dataObject);
        }
    }

    private static FORMATETC CreateFormat(TYMED tymed) => new()
    {
        cfFormat = (short)DataFormats.GetFormat(DataFormats.EnhancedMetafile).Id,
        dwAspect = DVASPECT.DVASPECT_CONTENT,
        lindex = -1,
        tymed = tymed
    };

    private static MemoryStream CreateMetafileStream()
    {
        // Use a real EMF and an expandable stream, matching the WPF regression's stream payload.
        byte[] bytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "bitmaps", "milkmateya01.emf"));
        MemoryStream stream = new();
        stream.Write(bytes);
        stream.Position = 0;
        return stream;
    }

    // Public IDataObject uses the BCL STGMEDIUM, not the product import's native COM projection.
    [DllImport("ole32.dll", ExactSpelling = true)]
    private static extern void ReleaseStgMedium(ref STGMEDIUM medium);

    // Inspect the native clipboard independently of managed format discovery and conversion.
    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenClipboard(IntPtr owner);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetClipboardData(uint format);
}
