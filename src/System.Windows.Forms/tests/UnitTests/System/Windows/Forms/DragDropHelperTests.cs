using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using Xunit;
using static Interop;
using static Interop.Shell32;
using static Interop.User32;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests
{
    public class DragDropHelperTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> DragDropHelper_DragImage_TestData()
        {
            yield return new object[] { new DataObject(), new Bitmap(1, 1), new Point(1, 1), false };
            yield return new object[] { new DataObject(), null, new Point(1, 1), false };
            yield return new object[] { new DataObject(), new Bitmap(1, 1), new Point(1, 1), true };
        }

        public static IEnumerable<object[]> DragDropHelper_DropDescription_TestData()
        {
            yield return new object[] { new DataObject(), DropImageType.Invalid, string.Empty, string.Empty };
            yield return new object[] { new DataObject(), DropImageType.None, string.Empty, string.Empty };
            yield return new object[] { new DataObject(), DropImageType.Copy, "Copy to %1", "Documents" };
            yield return new object[] { new DataObject(), DropImageType.Move, "Move to %1", "Documents" };
            yield return new object[] { new DataObject(), DropImageType.Link, "Create link in %1", "Documents" };
            yield return new object[] { new DataObject(), DropImageType.Label, "Update metadata in %1", "Document" };
            yield return new object[] { new DataObject(), DropImageType.Warning, "A problem has been encountered", string.Empty };
            yield return new object[] { new DataObject(), DropImageType.NoImage, "Copy to %1", "Documents" };
        }

        public static IEnumerable<object[]> DragDropHelper_InDragLoop_TestData()
        {
            yield return new object[] { new DataObject(), true };
            yield return new object[] { new DataObject(), false };
        }

        [Fact]
        public void DragDropHelper_IsInDragLoopFormat_ReturnsExpected()
        {
            FORMATETC formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW(DragDropHelper.CF_INSHELLDRAGLOOP),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = TYMED.TYMED_HGLOBAL
            };

            Assert.True(DragDropHelper.IsInDragLoopFormat(formatEtc));
        }

        [StaTheory]
        [MemberData(nameof(DragDropHelper_DragImage_TestData))]
        public unsafe void DragDropHelper_SetDragImage_ReturnsExptected(DataObject dataObject, Bitmap dragImage, Point cursorOffset, bool useDefaultDragImage)
        {
            try
            {
                DragDropHelper.SetDragImage(dataObject, dragImage, cursorOffset, useDefaultDragImage);
                DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DRAGIMAGEBITS);
                IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                SHDRAGIMAGE* pDragImage = (SHDRAGIMAGE*)basePtr;
                bool isDragImageNull = pDragImage->hbmpDragImage.IsNull;
                Size dragImageSize = pDragImage->sizeDragImage;
                Point offset = pDragImage->ptOffset;
                Kernel32.GlobalUnlock(dragDropFormat.Medium.unionmember);
                Assert.False(isDragImageNull);
                Assert.Equal(dragImage is null ? new Size(0, 0) : dragImage.Size, dragImageSize);
                Assert.Equal(cursorOffset, offset);
            }
            finally
            {
                DragDropHelper.ReleaseDragDropFormats(dataObject);
            }
        }

        [Theory]
        [MemberData(nameof(DragDropHelper_DropDescription_TestData))]
        public unsafe void DragDropHelper_SetDropDescription_ClearDropDescription_ReturnsExpected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
        {
            try
            {
                DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
                DragDropHelper.ClearDropDescription(dataObject);
                DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DROPDESCRIPTION);
                IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
                DROPIMAGETYPE type = pDropDescription->type;
                string szMessage = pDropDescription->Message.ToString();
                string szInsert = pDropDescription->Insert.ToString();
                Kernel32.GlobalUnlock(dragDropFormat.Medium.unionmember);
                Assert.Equal(DROPIMAGETYPE.DROPIMAGE_INVALID, type);
                Assert.Equal(string.Empty, szMessage);
                Assert.Equal(string.Empty, szInsert);
            }
            finally
            {
                DragDropHelper.ReleaseDragDropFormats(dataObject);
            }
        }

        [Theory]
        [MemberData(nameof(DragDropHelper_DropDescription_TestData))]
        public void DragDropHelper_SetDropDescription_IsInDragLoop_ReturnsExpected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
        {
            try
            {
                DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
                Assert.True(DragDropHelper.IsInDragLoop(dataObject as IComDataObject));
                Assert.True(DragDropHelper.IsInDragLoop(dataObject as IDataObject));
            }
            finally
            {
                DragDropHelper.ReleaseDragDropFormats(dataObject);
            }
        }

        [Theory]
        [MemberData(nameof(DragDropHelper_DropDescription_TestData))]
        public void DragDropHelper_SetDropDescription_ReleaseDragDropFormats_ReturnsExptected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
        {
            DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
            DragDropHelper.ReleaseDragDropFormats(dataObject);

            foreach (string format in dataObject.GetFormats())
            {
                if (dataObject.GetData(format) is DragDropFormat dragDropFormat)
                {
                    Assert.Equal(0, Kernel32.GlobalSize(dragDropFormat.Medium.unionmember));
                    Assert.Null(dragDropFormat.Medium.pUnkForRelease);
                    Assert.Equal(TYMED.TYMED_NULL, dragDropFormat.Medium.tymed);
                    Assert.Equal(IntPtr.Zero, dragDropFormat.Medium.unionmember);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DragDropHelper_DropDescription_TestData))]
        public unsafe void DragDropHelper_SetDropDescription_ReturnsExptected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
        {
            try
            {
                DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
                DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DROPDESCRIPTION);
                IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
                DROPIMAGETYPE type = pDropDescription->type;
                string szMessage = pDropDescription->Message.ToString();
                string szInsert = pDropDescription->Insert.ToString();
                Kernel32.GlobalUnlock(dragDropFormat.Medium.unionmember);
                Assert.Equal((DROPIMAGETYPE)dropImageType, type);
                Assert.Equal(message, szMessage);
                Assert.Equal(messageReplacementToken, szInsert);
            }
            finally
            {
                DragDropHelper.ReleaseDragDropFormats(dataObject);
            }
        }

        [Theory]
        [MemberData(nameof(DragDropHelper_InDragLoop_TestData))]
        public unsafe void DragDropHelper_SetInDragLoop_ReturnsExptected(DataObject dataObject, bool inDragLoop)
        {
            try
            {
                DragDropHelper.SetInDragLoop(dataObject, inDragLoop);
                DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_INSHELLDRAGLOOP);
                IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                bool inShellDragLoop = (basePtr != IntPtr.Zero) && (*(BOOL*)basePtr == BOOL.TRUE);
                Kernel32.GlobalUnlock(dragDropFormat.Medium.unionmember);
                Assert.Equal(inDragLoop, inShellDragLoop);
            }
            finally
            {
                DragDropHelper.ReleaseDragDropFormats(dataObject);
            }
        }
    }
}
