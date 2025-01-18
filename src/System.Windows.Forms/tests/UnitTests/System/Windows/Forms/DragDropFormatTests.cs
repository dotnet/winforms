// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using DVASPECT = System.Runtime.InteropServices.ComTypes.DVASPECT;
using FORMATETC = System.Runtime.InteropServices.ComTypes.FORMATETC;
using IStream = Windows.Win32.System.Com.IStream;
using STGMEDIUM = System.Runtime.InteropServices.ComTypes.STGMEDIUM;
using TYMED = System.Runtime.InteropServices.ComTypes.TYMED;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // Each registered Clipboard format is an OS singleton,
                           // and we should not run this test at the same time as other tests using the same format.
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
public unsafe class DragDropFormatTests
{
    public static IEnumerable<object[]> DragDropFormat_TestData()
    {
        FORMATETC formatEtc = new()
        {
            cfFormat = (short)PInvokeCore.RegisterClipboardFormat("InShellDragLoop"),
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = nint.Zero,
            tymed = TYMED.TYMED_HGLOBAL
        };

        STGMEDIUM medium = new()
        {
            pUnkForRelease = null,
            tymed = TYMED.TYMED_HGLOBAL,
            unionmember = PInvokeCore.GlobalAlloc(
                GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
                BOOL.Size)
        };

        SaveInDragLoopToHandle((HGLOBAL)medium.unionmember, inDragLoop: true);
        yield return new object[] { formatEtc, medium };

        MemoryStream memoryStream = new();
        IStream.Interface iStream = new ComManagedStream(memoryStream);
        formatEtc = new()
        {
            cfFormat = (short)PInvokeCore.RegisterClipboardFormat("DragContext"),
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = nint.Zero,
            tymed = TYMED.TYMED_ISTREAM
        };

        medium = new()
        {
            pUnkForRelease = null,
            tymed = TYMED.TYMED_ISTREAM,
            unionmember = Marshal.GetIUnknownForObject(iStream)
        };

        yield return new object[] { formatEtc, medium };
    }

    [Theory]
    [MemberData(nameof(DragDropFormat_TestData))]
    public void DragDropFormat_Set_Dispose_ReturnsExpected(FORMATETC formatEtc, STGMEDIUM medium)
    {
        DragDropFormat? dragDropFormat = default;

        try
        {
            dragDropFormat = new DragDropFormat((ushort)formatEtc.cfFormat, (Com.STGMEDIUM)medium, copyData: false);
            dragDropFormat.Dispose();
            int handleSize = (int)PInvokeCore.GlobalSize(dragDropFormat.Medium.hGlobal);
            Assert.Equal(0, handleSize);
            Assert.Equal(nint.Zero, (nint)dragDropFormat.Medium.pUnkForRelease);
            Assert.Equal(Com.TYMED.TYMED_NULL, dragDropFormat.Medium.tymed);
            Assert.True(dragDropFormat.Medium.hGlobal.IsNull);
        }
        finally
        {
            dragDropFormat?.Dispose();
        }
    }

    [Theory]
    [MemberData(nameof(DragDropFormat_TestData))]
    public void DragDropFormat_Set_GetData_ReturnsExpected(FORMATETC formatEtc, STGMEDIUM medium)
    {
        DragDropFormat? dragDropFormat = default;

        try
        {
            dragDropFormat = new DragDropFormat((ushort)formatEtc.cfFormat, (Com.STGMEDIUM)medium, copyData: false);
            Com.STGMEDIUM data = dragDropFormat.GetData();
            Assert.Equal(medium.pUnkForRelease ?? nint.Zero, (nint)data.pUnkForRelease);
            Assert.Equal((uint)medium.tymed, (uint)data.tymed);

            switch (data.tymed)
            {
                case Com.TYMED.TYMED_HGLOBAL:
                case Com.TYMED.TYMED_FILE:
                case Com.TYMED.TYMED_ENHMF:
                case Com.TYMED.TYMED_GDI:
                case Com.TYMED.TYMED_MFPICT:

                    Assert.NotEqual(medium.unionmember, data.hGlobal);
                    break;

                case Com.TYMED.TYMED_ISTORAGE:
                case Com.TYMED.TYMED_ISTREAM:
                case Com.TYMED.TYMED_NULL:
                default:

                    Assert.Equal(medium.unionmember, (nint)data.hGlobal);
                    break;
            }
        }
        finally
        {
            dragDropFormat?.Dispose();
        }
    }

    [Theory]
    [MemberData(nameof(DragDropFormat_TestData))]
    public void DragDropFormat_Set_RefreshData_ReturnsExpected(FORMATETC formatEtc, STGMEDIUM medium)
    {
        DragDropFormat? dragDropFormat = default;

        try
        {
            dragDropFormat = new DragDropFormat((ushort)formatEtc.cfFormat, (Com.STGMEDIUM)medium, copyData: false);
            Com.STGMEDIUM dataRefresh = new()
            {
                pUnkForRelease = dragDropFormat.Medium.pUnkForRelease,
                tymed = dragDropFormat.Medium.tymed,
                u = new()
                {
                    hGlobal = dragDropFormat.Medium.tymed switch
                    {
                        Com.TYMED.TYMED_HGLOBAL or Com.TYMED.TYMED_FILE or Com.TYMED.TYMED_ENHMF or Com.TYMED.TYMED_GDI or Com.TYMED.TYMED_MFPICT
                        => (HGLOBAL)(nint)PInvoke.OleDuplicateData(
                            (HANDLE)(nint)dragDropFormat.Medium.hGlobal,
                            (CLIPBOARD_FORMAT)formatEtc.cfFormat,
                            GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT),
                        _ => dragDropFormat.Medium.hGlobal,
                    }
                }
            };

            dragDropFormat.RefreshData((ushort)formatEtc.cfFormat, dataRefresh, copyData: false);
            Com.STGMEDIUM data = dragDropFormat.GetData();

            switch (dragDropFormat.Medium.tymed)
            {
                case Com.TYMED.TYMED_HGLOBAL:
                case Com.TYMED.TYMED_FILE:
                case Com.TYMED.TYMED_ENHMF:
                case Com.TYMED.TYMED_GDI:
                case Com.TYMED.TYMED_MFPICT:

                    Assert.NotEqual(dragDropFormat.Medium.u, data.u);
                    break;

                case Com.TYMED.TYMED_ISTORAGE:
                case Com.TYMED.TYMED_ISTREAM:
                case Com.TYMED.TYMED_NULL:
                default:

                    Assert.Equal(dragDropFormat.Medium.u, data.u);
                    break;
            }
        }
        finally
        {
            dragDropFormat?.Dispose();
        }
    }

    private static unsafe void SaveInDragLoopToHandle(HGLOBAL handle, bool inDragLoop)
    {
        try
        {
            void* basePtr = PInvokeCore.GlobalLock(handle);
            *(BOOL*)basePtr = (BOOL)inDragLoop;
        }
        finally
        {
            PInvokeCore.GlobalUnlock(handle);
        }
    }
}
