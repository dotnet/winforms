// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
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
        yield return new object[] { new DataObject(), DropImageType.Copy, new string('*', (int)PInvokeCore.MAX_PATH), string.Empty };
        yield return new object[] { new DataObject(), DropImageType.Copy, string.Empty, new string('*', (int)PInvokeCore.MAX_PATH) };
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
    [InlineData(DragDropHelper.DRAGCONTEXT, true)]
    [InlineData(DragDropHelper.DRAGIMAGEBITS, true)]
    [InlineData(DragDropHelper.DRAGSOURCEHELPERFLAGS, true)]
    [InlineData(DragDropHelper.DRAGWINDOW, true)]
    [InlineData(PInvoke.CFSTR_DROPDESCRIPTION, true)]
    [InlineData(PInvoke.CFSTR_INDRAGLOOP, true)]
    [InlineData(DragDropHelper.ISSHOWINGLAYERED, true)]
    [InlineData(DragDropHelper.ISSHOWINGTEXT, true)]
    [InlineData(DragDropHelper.USINGDEFAULTDRAGIMAGE, true)]
    public void IsInDragLoopFormat_ReturnsExpected(string format, bool expectedIsInDragLoopFormat)
    {
        FORMATETC formatEtc = new()
        {
            cfFormat = (short)PInvokeCore.RegisterClipboardFormat(format),
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = IntPtr.Zero,
            tymed = TYMED.TYMED_HGLOBAL
        };

        Assert.Equal(expectedIsInDragLoopFormat, DragDropHelper.IsInDragLoopFormat(Unsafe.As<FORMATETC, Com.FORMATETC>(ref formatEtc)));
    }

    [WinFormsTheory(Skip = "Causing issues with other tests on x86 from the command line")]
    [MemberData(nameof(DragImage_DataObject_Bitmap_Point_bool_TestData))]
    public unsafe void SetDragImage_DataObject_Bitmap_Point_bool_ReturnsExpected(DataObject dataObject, Bitmap dragImage, Point cursorOffset, bool useDefaultDragImage)
    {
        try
        {
            DragDropHelper.SetDragImage(dataObject, dragImage, cursorOffset, useDefaultDragImage);
            // This DataObject is backed up by the DataStore.
            dataObject.TryGetData(DragDropHelper.DRAGIMAGEBITS, out DragDropFormat dragDropFormat).Should().BeTrue();
            dragDropFormat.Should().NotBeNull();
            void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
            SHDRAGIMAGE* pDragImage = (SHDRAGIMAGE*)basePtr;
            bool isDragImageNull = BitOperations.LeadingZeroCount((uint)(nint)pDragImage->hbmpDragImage).Equals(32);
            Size dragImageSize = pDragImage->sizeDragImage;
            Point offset = pDragImage->ptOffset;
            PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
            (dragImage is null).Should().Be(isDragImageNull);
            (dragImage is null ? new Size(0, 0) : dragImage.Size).Should().Be(dragImageSize);
            cursorOffset.Should().Be(offset);
        }
        finally
        {
            DragDropHelper.ReleaseDragDropFormats(dataObject);
        }
    }

    [WinFormsTheory(Skip = "Causing issues with other tests on x86 from the command line")]
    [MemberData(nameof(DragImage_DataObject_GiveFeedbackEventArgs_TestData))]
    public unsafe void SetDragImage_DataObject_GiveFeedbackEventArgs_ReturnsExpected(DataObject dataObject, GiveFeedbackEventArgs e)
    {
        try
        {
            DragDropHelper.SetDragImage(dataObject, e);
            // This DataObject is backed up by the DataStore.
            dataObject.TryGetData(DragDropHelper.DRAGIMAGEBITS, out DragDropFormat dragDropFormat).Should().BeTrue();
            dragDropFormat.Should().NotBeNull();
            void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
            SHDRAGIMAGE* pDragImage = (SHDRAGIMAGE*)basePtr;
            bool isDragImageNull = BitOperations.LeadingZeroCount((uint)(nint)pDragImage->hbmpDragImage).Equals(32);
            Size dragImageSize = pDragImage->sizeDragImage;
            Point offset = pDragImage->ptOffset;
            PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
            (e.DragImage is null).Should().Be(isDragImageNull);
            (e.DragImage is null ? new Size(0, 0) : e.DragImage.Size).Should().Be(dragImageSize);
            e.CursorOffset.Should().Be(offset);
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
        using Bitmap bitmap = new(1, 1);
        Assert.Throws<InvalidOperationException>(() => DragDropHelper.SetDragImage(new DataObject(), bitmap, new Point(0, 0), false));
    }

    [Fact]
    public void SetDragImage_NullDataObject_ThrowsArgumentNullException()
    {
        DataObject dataObject = null;
        using Bitmap bitmap = new(1, 1);
        Assert.Throws<ArgumentNullException>(nameof(dataObject),
            () => DragDropHelper.SetDragImage(dataObject, bitmap, new Point(0, 0), false));
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
            dataObject.TryGetData(PInvoke.CFSTR_DROPDESCRIPTION, autoConvert: false, out DragDropFormat dragDropFormat).Should().BeTrue();
            dragDropFormat.Should().NotBeNull();
            void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            DROPIMAGETYPE type = pDropDescription->type;
            string szMessage = pDropDescription->szMessage.ToString();
            string szInsert = pDropDescription->szInsert.ToString();
            PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
            type.Should().Be(DROPIMAGETYPE.DROPIMAGE_INVALID);
            szMessage.Should().Be(string.Empty);
            szInsert.Should().Be(string.Empty);
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
    public unsafe void SetDropDescription_ReleaseDragDropFormats_ReturnsExpected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
        DragDropHelper.ReleaseDragDropFormats(dataObject);

        foreach (string format in dataObject.GetFormats())
        {
            if (dataObject.TryGetData(format, out DragDropFormat dragDropFormat))
            {
                Assert.Equal(nint.Zero, (nint)dragDropFormat.Medium.pUnkForRelease);
                Assert.Equal(Com.TYMED.TYMED_NULL, dragDropFormat.Medium.tymed);
                Assert.Equal(nint.Zero, (nint)dragDropFormat.Medium.hGlobal);
            }
        }
    }

    [Theory]
    [MemberData(nameof(DropDescription_DragEventArgs_TestData))]
    public unsafe void SetDropDescription_DragEventArgs_ReturnsExpected(DragEventArgs e)
    {
        try
        {
            DragDropHelper.SetDropDescription(e);
            e.Data.TryGetData(PInvoke.CFSTR_DROPDESCRIPTION, out DragDropFormat dragDropFormat).Should().BeTrue();
            void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            DROPIMAGETYPE type = pDropDescription->type;
            string szMessage = pDropDescription->szMessage.ToString();
            string szInsert = pDropDescription->szInsert.ToString();
            PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
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
    public unsafe void SetDropDescription_DataObject_DropImageType_string_string_ReturnsExpected(DataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
    {
        try
        {
            DragDropHelper.SetDropDescription(dataObject, dropImageType, message, messageReplacementToken);
            dataObject.TryGetData(PInvoke.CFSTR_DROPDESCRIPTION, autoConvert: false, out DragDropFormat dragDropFormat).Should().BeTrue();
            void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            DROPIMAGETYPE type = pDropDescription->type;
            string szMessage = pDropDescription->szMessage.ToString();
            string szInsert = pDropDescription->szInsert.ToString();
            PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
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
    public unsafe void SetInDragLoop_ReturnsExpected(DataObject dataObject, bool inDragLoop)
    {
        try
        {
            DragDropHelper.SetInDragLoop(dataObject, inDragLoop);
            dataObject.TryGetData(PInvoke.CFSTR_INDRAGLOOP, out DragDropFormat dragDropFormat).Should().BeTrue();
            void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
            bool inShellDragLoop = (basePtr is not null) && (*(BOOL*)basePtr == true);
            PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
            Assert.Equal(inDragLoop, inShellDragLoop);
        }
        finally
        {
            DragDropHelper.ReleaseDragDropFormats(dataObject);
        }
    }
}
