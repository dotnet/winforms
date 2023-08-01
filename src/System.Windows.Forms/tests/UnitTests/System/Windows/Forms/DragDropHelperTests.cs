// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests;

public class DragDropHelperTests
{
    public static IEnumerable<object[]> DragImage_DataObject_Bitmap_Point_bool_TestData()
    {
        yield return new object[] { new DataObject(), new Bitmap(1, 1), new Point(1, 1), false };
        yield return new object[] { new DataObject(), null, new Point(1, 1), false };
        yield return new object[] { new DataObject(), new Bitmap(1, 1), new Point(1, 1), true };
    }

    public static IEnumerable<object[]> DragImage_DataObject_GiveFeedbackEventArgs_TestData()
    {
        yield return new object[] { new DataObject(), new GiveFeedbackEventArgs(DragDropEffects.All, false, new Bitmap(1, 1), new Point(0, 0), false) };
        yield return new object[] { new DataObject(), new GiveFeedbackEventArgs(DragDropEffects.All, false, null, new Point(0, 0), false) };
        yield return new object[] { new DataObject(), new GiveFeedbackEventArgs(DragDropEffects.All, false, new Bitmap(1, 1), new Point(0, 0), true) };
    }

    public static IEnumerable<object[]> DropDescription_DragEventArgs_TestData()
    {
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.Invalid, string.Empty, string.Empty) };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.None, string.Empty, string.Empty) };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.Copy, "Copy to %1", "Documents") };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.Move, "Move to %1", "Documents") };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.Link, "Create link in %1", "Documents") };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.Label, "Update metadata in %1", "Document") };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.Warning, "A problem has been encountered", string.Empty) };
        yield return new object[] { new DragEventArgs(new DataObject(), 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Copy, DropImageType.NoImage, "Copy to %1", "Documents") };
    }

    public static IEnumerable<object[]> DropDescription_DataObject_DropImageType_string_string_TestData()
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

    public static IEnumerable<object[]> DropDescription_LengthExceedsMaxPath_TestData()
    {
        yield return new object[] { new DataObject(), DropImageType.Copy, new string('*', PInvoke.MAX_PATH), string.Empty };
        yield return new object[] { new DataObject(), DropImageType.Copy, string.Empty, new string('*', PInvoke.MAX_PATH) };
    }

    public static IEnumerable<object[]> InDragLoop_TestData()
    {
        yield return new object[] { new DataObject(), true };
        yield return new object[] { new DataObject(), false };
    }

    [Fact]
    public void IsInDragLoop_NullComDataObject_ThrowsArgumentNullException()
    {
        IComDataObject dataObject = null;
        Assert.Throws<ArgumentNullException>(nameof(dataObject), () => DragDropHelper.IsInDragLoop(dataObject));
    }

    [Fact]
    public void IsInDragLoop_NullDataObject_ThrowsArgumentNullException()
    {
        IDataObject dataObject = null;
        Assert.Throws<ArgumentNullException>(nameof(dataObject), () => DragDropHelper.IsInDragLoop(dataObject));
    }

    [Theory]
    [InlineData(DragDropHelper.CF_DRAGIMAGEBITS, false)]
    [InlineData(DragDropHelper.CF_DROPDESCRIPTION, false)]
    [InlineData(DragDropHelper.CF_INSHELLDRAGLOOP, true)]
    [InlineData(DragDropHelper.CF_ISSHOWINGTEXT, false)]
    [InlineData(DragDropHelper.CF_USINGDEFAULTDRAGIMAGE, false)]
    public void IsInDragLoopFormat_ReturnsExpected(string format, bool expectedIsInDragLoopFormat)
    {
        FORMATETC formatEtc = new()
        {
            cfFormat = (short)PInvoke.RegisterClipboardFormat(format),
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = IntPtr.Zero,
            tymed = TYMED.TYMED_HGLOBAL
        };

        Assert.Equal(expectedIsInDragLoopFormat, DragDropHelper.IsInDragLoopFormat(formatEtc));
    }

    [WinFormsTheory(Skip ="Causing issues with other tests on x86 from the command line")]
    [MemberData(nameof(DragImage_DataObject_Bitmap_Point_bool_TestData))]
    public unsafe void SetDragImage_DataObject_Bitmap_Point_bool_ReturnsExptected(DataObject dataObject, Bitmap dragImage, Point cursorOffset, bool useDefaultDragImage)
    {
        try
        {
            DragDropHelper.SetDragImage(dataObject, dragImage, cursorOffset, useDefaultDragImage);
            DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DRAGIMAGEBITS);
            void* basePtr = PInvoke.GlobalLock((HGLOBAL)dragDropFormat.Medium.unionmember);
            SHDRAGIMAGE* pDragImage = (SHDRAGIMAGE*)basePtr;
            bool isDragImageNull = BitOperations.LeadingZeroCount((uint)(nint)pDragImage->hbmpDragImage).Equals(32);
            Size dragImageSize = pDragImage->sizeDragImage;
            Point offset = pDragImage->ptOffset;
            PInvoke.GlobalUnlock((HGLOBAL)dragDropFormat.Medium.unionmember);
            Assert.Equal(dragImage is null, isDragImageNull);
            Assert.Equal(dragImage is null ? new Size(0, 0) : dragImage.Size, dragImageSize);
            Assert.Equal(cursorOffset, offset);
        }
        finally
        {
            DragDropHelper.ReleaseDragDropFormats(dataObject);
        }
    }

    [WinFormsTheory(Skip = "Causing issues with other tests on x86 from the command line")]
    [MemberData(nameof(DragImage_DataObject_GiveFeedbackEventArgs_TestData))]
    public unsafe void SetDragImage_DataObject_GiveFeedbackEventArgs_ReturnsExptected(DataObject dataObject, GiveFeedbackEventArgs e)
    {
        try
        {
            DragDropHelper.SetDragImage(dataObject, e);
            DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DRAGIMAGEBITS);
            void* basePtr = PInvoke.GlobalLock((HGLOBAL)dragDropFormat.Medium.unionmember);
            SHDRAGIMAGE* pDragImage = (SHDRAGIMAGE*)basePtr;
            bool isDragImageNull = BitOperations.LeadingZeroCount((uint)(nint)pDragImage->hbmpDragImage).Equals(32);
            Size dragImageSize = pDragImage->sizeDragImage;
            Point offset = pDragImage->ptOffset;
            PInvoke.GlobalUnlock((HGLOBAL)dragDropFormat.Medium.unionmember);
            Assert.Equal(e.DragImage is null, isDragImageNull);
            Assert.Equal(e.DragImage is null ? new Size(0, 0) : e.DragImage.Size, dragImageSize);
            Assert.Equal(e.CursorOffset, offset);
        }
        finally
        {
            DragDropHelper.ReleaseDragDropFormats(dataObject);
        }
    }

    [Fact(Skip = "Causing issues with other tests on x86 from the command line")]
    public void SetDragImage_NonSTAThread_ThrowsInvalidOperationException()
    {
        Control.CheckForIllegalCrossThreadCalls = true;
        Assert.Throws<InvalidOperationException>(() => DragDropHelper.SetDragImage(new DataObject(), new Bitmap(1, 1), new Point(0, 0), false));
    }

    [Fact]
    public void SetDragImage_NullDataObject_ThrowsArgumentNullException()
    {
        DataObject dataObject = null;
        Assert.Throws<ArgumentNullException>(nameof(dataObject),
            () => DragDropHelper.SetDragImage(dataObject, new Bitmap(1, 1), new Point(0, 0), false));
    }

    [Fact]
    public void SetDragImage_NullGiveFeedbackEventArgs_ThrowsArgumentNullException()
    {
        GiveFeedbackEventArgs e = null;
        Assert.Throws<ArgumentNullException>(nameof(e), () => DragDropHelper.SetDragImage(new DataObject(), e));
    }

    [Theory]
    [MemberData(nameof(DropDescription_DataObject_DropImageType_string_string_TestData))]
    public unsafe void SetDropDescription_ClearDropDescription_ReturnsExpected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        try
        {
            DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
            DragDropHelper.ClearDropDescription(dataObject);
            DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DROPDESCRIPTION);
            void* basePtr = PInvoke.GlobalLock((HGLOBAL)dragDropFormat.Medium.unionmember);
            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            DROPIMAGETYPE type = pDropDescription->type;
            string szMessage = pDropDescription->szMessage.ToString();
            string szInsert = pDropDescription->szInsert.ToString();
            PInvoke.GlobalUnlock((HGLOBAL)dragDropFormat.Medium.unionmember);
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
    [InlineData(DropImageType.Invalid - 1)]
    [InlineData(DropImageType.NoImage + 1)]
    public void SetDropDescription_InvalidDropImageType_ThrowsArgumentNullException(DropImageType dropImageType)
    {
        Assert.Throws<InvalidEnumArgumentException>(nameof(dropImageType),
            () => DragDropHelper.SetDropDescription(new DataObject(), dropImageType, string.Empty, string.Empty));
    }

    [Theory]
    [MemberData(nameof(DropDescription_DataObject_DropImageType_string_string_TestData))]
    public void SetDropDescription_IsInDragLoop_ReturnsExpected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
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
    [MemberData(nameof(DropDescription_LengthExceedsMaxPath_TestData))]
    public void SetDropDescription_LengthExceedsMaxPath_ThrowsArgumentOutOfRangeException(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken));
    }

    [Fact]
    public void SetDropDescription_NullDataObject_ThrowsArgumentNullException()
    {
        DataObject dataObject = null;
        Assert.Throws<ArgumentNullException>(nameof(dataObject),
            () => DragDropHelper.SetDropDescription(dataObject, DropImageType.Invalid, string.Empty, string.Empty));
    }

    [Theory]
    [MemberData(nameof(DropDescription_DataObject_DropImageType_string_string_TestData))]
    public void SetDropDescription_ReleaseDragDropFormats_ReturnsExptected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
        DragDropHelper.ReleaseDragDropFormats(dataObject);

        foreach (string format in dataObject.GetFormats())
        {
            if (dataObject.GetData(format) is DragDropFormat dragDropFormat)
            {
                Assert.Equal(0, (int)PInvoke.GlobalSize((HGLOBAL)dragDropFormat.Medium.unionmember));
                Assert.Null(dragDropFormat.Medium.pUnkForRelease);
                Assert.Equal(TYMED.TYMED_NULL, dragDropFormat.Medium.tymed);
                Assert.Equal(IntPtr.Zero, dragDropFormat.Medium.unionmember);
            }
        }
    }

    [Theory]
    [MemberData(nameof(DropDescription_DragEventArgs_TestData))]
    public unsafe void SetDropDescription_DragEventArgs_ReturnsExptected(DragEventArgs e)
    {
        try
        {
            DragDropHelper.SetDropDescription(e);
            DragDropFormat dragDropFormat = (DragDropFormat)e.Data.GetData(DragDropHelper.CF_DROPDESCRIPTION);
            void* basePtr = PInvoke.GlobalLock((HGLOBAL)dragDropFormat.Medium.unionmember);
            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            DROPIMAGETYPE type = pDropDescription->type;
            string szMessage = pDropDescription->szMessage.ToString();
            string szInsert = pDropDescription->szInsert.ToString();
            PInvoke.GlobalUnlock((HGLOBAL)dragDropFormat.Medium.unionmember);
            Assert.Equal((DROPIMAGETYPE)e.DropImageType, type);
            Assert.Equal(e.Message, szMessage);
            Assert.Equal(e.MessageReplacementToken, szInsert);
        }
        finally
        {
            if (e.Data is IComDataObject dataObject)
            {
                DragDropHelper.ReleaseDragDropFormats(dataObject);
            }
        }
    }

    [Theory]
    [MemberData(nameof(DropDescription_DataObject_DropImageType_string_string_TestData))]
    public unsafe void SetDropDescription_DataObject_DropImageType_string_string_ReturnsExptected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        try
        {
            DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
            DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_DROPDESCRIPTION);
            void* basePtr = PInvoke.GlobalLock((HGLOBAL)dragDropFormat.Medium.unionmember);
            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            DROPIMAGETYPE type = pDropDescription->type;
            string szMessage = pDropDescription->szMessage.ToString();
            string szInsert = pDropDescription->szInsert.ToString();
            PInvoke.GlobalUnlock((HGLOBAL)dragDropFormat.Medium.unionmember);
            Assert.Equal((DROPIMAGETYPE)dropImageType, type);
            Assert.Equal(message, szMessage);
            Assert.Equal(messageReplacementToken, szInsert);
        }
        finally
        {
            DragDropHelper.ReleaseDragDropFormats(dataObject);
        }
    }

    [Fact]
    public unsafe void SetInDragLoop_NullDataObject_ThrowsArgumentNullException()
    {
        DataObject dataObject = null;
        Assert.Throws<ArgumentNullException>(nameof(dataObject), () => DragDropHelper.SetInDragLoop(dataObject, true));
    }

    [Theory]
    [MemberData(nameof(InDragLoop_TestData))]
    public unsafe void SetInDragLoop_ReturnsExptected(DataObject dataObject, bool inDragLoop)
    {
        try
        {
            DragDropHelper.SetInDragLoop(dataObject, inDragLoop);
            DragDropFormat dragDropFormat = (DragDropFormat)dataObject.GetData(DragDropHelper.CF_INSHELLDRAGLOOP);
            void* basePtr = PInvoke.GlobalLock((HGLOBAL)dragDropFormat.Medium.unionmember);
            bool inShellDragLoop = (basePtr is not null) && (*(BOOL*)basePtr == true);
            PInvoke.GlobalUnlock((HGLOBAL)dragDropFormat.Medium.unionmember);
            Assert.Equal(inDragLoop, inShellDragLoop);
        }
        finally
        {
            DragDropHelper.ReleaseDragDropFormats(dataObject);
        }
    }
}
