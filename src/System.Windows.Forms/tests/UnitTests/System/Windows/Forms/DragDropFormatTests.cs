using System.Runtime.InteropServices.ComTypes;
using Xunit;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.Tests
{
    public class DragDropFormatTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> DragDropFormat_InDragLoop_TestData()
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
                unionmember = Kernel32.GlobalAlloc(
                    Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                    sizeof(BOOL))
            };

            SaveInDragLoopToHandle(medium.unionmember, inDragLoop: true);
            yield return new object[] { formatEtc, medium };
        }

        [Theory]
        [MemberData(nameof(DragDropFormat_InDragLoop_TestData))]
        public unsafe void DragDropFormat_Set_Dispose_ReturnsExpected(FORMATETC formatEtc, STGMEDIUM medium)
        {
            DragDropFormat dragDropFormat = default;

            try
            {
                dragDropFormat = new DragDropFormat(formatEtc.cfFormat, medium, copyData: false);
                int preDisposeHandleSize = Kernel32.GlobalSize(dragDropFormat.Medium.unionmember);
                dragDropFormat.Dispose();
                int postDisposeHandleSize = Kernel32.GlobalSize(dragDropFormat.Medium.unionmember);
                Assert.Equal(sizeof(BOOL), preDisposeHandleSize);
                Assert.Equal(0, postDisposeHandleSize);
                Assert.Null(dragDropFormat.Medium.pUnkForRelease);
                Assert.Equal(TYMED.TYMED_NULL, dragDropFormat.Medium.tymed);
                Assert.Equal(IntPtr.Zero, dragDropFormat.Medium.unionmember);
            }
            finally
            {
                Ole32.ReleaseStgMedium(ref medium);
                dragDropFormat?.Dispose();
            }
        }

        [Theory]
        [MemberData(nameof(DragDropFormat_InDragLoop_TestData))]
        public unsafe void DragDropFormat_Set_GetData_ReturnsExpected(FORMATETC formatEtc, STGMEDIUM medium)
        {
            DragDropFormat dragDropFormat = default;
            STGMEDIUM data = default;

            try
            {
                dragDropFormat = new DragDropFormat(formatEtc.cfFormat, medium, copyData: false);
                data = dragDropFormat.GetData();
                IntPtr basePtr = Kernel32.GlobalLock(data.unionmember);
                bool inDragLoop = *(BOOL*)basePtr == BOOL.TRUE;
                Kernel32.GlobalUnlock(data.unionmember);
                Assert.True(inDragLoop);
                Assert.Equal(medium.pUnkForRelease, data.pUnkForRelease);
                Assert.Equal(medium.tymed, data.tymed);
                Assert.NotEqual(medium.unionmember, data.unionmember);
            }
            finally
            {
                Ole32.ReleaseStgMedium(ref medium);
                Ole32.ReleaseStgMedium(ref data);
                dragDropFormat?.Dispose();
            }
        }

        [Theory]
        [MemberData(nameof(DragDropFormat_InDragLoop_TestData))]
        public unsafe void DragDropFormat_Set_RefreshData_ReturnsExpected(FORMATETC formatEtc, STGMEDIUM medium)
        {
            DragDropFormat dragDropFormat = default;
            STGMEDIUM data = default;
            STGMEDIUM dataRefresh = default;

            try
            {
                dragDropFormat = new DragDropFormat(formatEtc.cfFormat, medium, copyData: false);
                bool preRefreshInDragLoop = GetInDragLoopFromHandle(dragDropFormat.Medium.unionmember);
                dataRefresh = new()
                {
                    pUnkForRelease = null,
                    tymed = TYMED.TYMED_HGLOBAL,
                    unionmember = Kernel32.GlobalAlloc(
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                        sizeof(BOOL))
                };

                SaveInDragLoopToHandle(dataRefresh.unionmember, inDragLoop: false);
                dragDropFormat.RefreshData(formatEtc.cfFormat, dataRefresh, copyData: false);
                data = dragDropFormat.GetData();
                bool postRefreshInDragLoop = GetInDragLoopFromHandle(data.unionmember);
                Assert.True(preRefreshInDragLoop);
                Assert.False(postRefreshInDragLoop);
            }
            finally
            {
                Ole32.ReleaseStgMedium(ref medium);
                Ole32.ReleaseStgMedium(ref data);
                Ole32.ReleaseStgMedium(ref dataRefresh);
                dragDropFormat?.Dispose();
            }
        }

        private unsafe static bool GetInDragLoopFromHandle(IntPtr handle)
        {
            try
            {
                IntPtr basePtr = Kernel32.GlobalLock(handle);
                return (basePtr != IntPtr.Zero) && (*(BOOL*)basePtr == BOOL.TRUE);
            }
            finally
            {
                Kernel32.GlobalUnlock(handle);
            }
        }

        private unsafe static void SaveInDragLoopToHandle(IntPtr handle, bool inDragLoop)
        {
            try
            {
                IntPtr basePtr = Kernel32.GlobalLock(handle);
                *(BOOL*)basePtr = inDragLoop ? BOOL.TRUE : BOOL.FALSE;
            }
            finally
            {
                Kernel32.GlobalUnlock(handle);
            }
        }
    }
}
