using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Xunit;
using static Interop;
using static Interop.User32;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;

namespace System.Windows.Forms.Tests
{
    public class DragDropFormatTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> DragDropFormat_TestData()
        {
            FORMATETC formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW("InShellDragLoop"),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = TYMED.TYMED_HGLOBAL
            };

            STGMEDIUM medium = new()
            {
                pUnkForRelease = null,
                tymed = TYMED.TYMED_HGLOBAL,
                unionmember = PInvoke.GlobalAlloc(
                    GMEM_MOVEABLE | GMEM_ZEROINIT,
                    BOOL.Size)
            };

            SaveInDragLoopToHandle(medium.unionmember, inDragLoop: true);
            yield return new object[] { formatEtc, medium };

            MemoryStream memoryStream = new();
            Ole32.IStream iStream = new Ole32.GPStream(memoryStream);
            formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW("DragContext"),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
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
            DragDropFormat dragDropFormat = default;

            try
            {
                dragDropFormat = new DragDropFormat(formatEtc.cfFormat, medium, copyData: false);
                dragDropFormat.Dispose();
                int handleSize = (int)PInvoke.GlobalSize(dragDropFormat.Medium.unionmember);
                Assert.Equal(0, handleSize);
                Assert.Null(dragDropFormat.Medium.pUnkForRelease);
                Assert.Equal(TYMED.TYMED_NULL, dragDropFormat.Medium.tymed);
                Assert.Equal(IntPtr.Zero, dragDropFormat.Medium.unionmember);
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
            DragDropFormat dragDropFormat = default;

            try
            {
                dragDropFormat = new DragDropFormat(formatEtc.cfFormat, medium, copyData: false);
                STGMEDIUM data = dragDropFormat.GetData();
                Assert.Equal(medium.pUnkForRelease, data.pUnkForRelease);
                Assert.Equal(medium.tymed, data.tymed);

                switch (data.tymed)
                {
                    case TYMED.TYMED_HGLOBAL:
                    case TYMED.TYMED_FILE:
                    case TYMED.TYMED_ENHMF:
                    case TYMED.TYMED_GDI:
                    case TYMED.TYMED_MFPICT:

                        Assert.NotEqual(medium.unionmember, data.unionmember);
                        break;

                    case TYMED.TYMED_ISTORAGE:
                    case TYMED.TYMED_ISTREAM:
                    case TYMED.TYMED_NULL:
                    default:

                        Assert.Equal(medium.unionmember, data.unionmember);
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
            DragDropFormat dragDropFormat = default;

            try
            {
                dragDropFormat = new DragDropFormat(formatEtc.cfFormat, medium, copyData: false);
                STGMEDIUM dataRefresh = new()
                {
                    pUnkForRelease = dragDropFormat.Medium.pUnkForRelease,
                    tymed = dragDropFormat.Medium.tymed,
                    unionmember = dragDropFormat.Medium.tymed switch
                    {
                        TYMED.TYMED_HGLOBAL or TYMED.TYMED_FILE or TYMED.TYMED_ENHMF or TYMED.TYMED_GDI or TYMED.TYMED_MFPICT
                        => Ole32.OleDuplicateData(
                            dragDropFormat.Medium.unionmember,
                            formatEtc.cfFormat,
                            PInvoke.GMEM.MOVEABLE | PInvoke.GMEM.DDESHARE | PInvoke.GMEM.ZEROINIT),
                        _ => dragDropFormat.Medium.unionmember,
                    }
                };

                dragDropFormat.RefreshData(formatEtc.cfFormat, dataRefresh, copyData: false);
                STGMEDIUM data = dragDropFormat.GetData();

                switch (dragDropFormat.Medium.tymed)
                {
                    case TYMED.TYMED_HGLOBAL:
                    case TYMED.TYMED_FILE:
                    case TYMED.TYMED_ENHMF:
                    case TYMED.TYMED_GDI:
                    case TYMED.TYMED_MFPICT:

                        Assert.NotEqual(dragDropFormat.Medium.unionmember, data.unionmember);
                        break;

                    case TYMED.TYMED_ISTORAGE:
                    case TYMED.TYMED_ISTREAM:
                    case TYMED.TYMED_NULL:
                    default:

                        Assert.Equal(dragDropFormat.Medium.unionmember, data.unionmember);
                        break;
                }
            }
            finally
            {
                dragDropFormat?.Dispose();
            }
        }

        private static unsafe void SaveInDragLoopToHandle(IntPtr handle, bool inDragLoop)
        {
            try
            {
                void* basePtr = PInvoke.GlobalLock(handle);
                *(BOOL*)basePtr = (BOOL)inDragLoop;
            }
            finally
            {
                PInvoke.GlobalUnlock(handle);
            }
        }
    }
}
