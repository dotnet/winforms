// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Ole;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Ole;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Verifies the native COM IDataObject surface that DataObject exposes to native callers: FORMATETC/TYMED
///  validation, HRESULTs, format aliases and STGMEDIUM ownership on failure.
/// </summary>
public partial class DataObjectTests
{
    private const int DataSSameFormatEtc = 0x00040130;

    private static Com.FORMATETC CreateNativeFormat(
        CLIPBOARD_FORMAT format,
        TYMED tymed = TYMED.TYMED_HGLOBAL,
        DVASPECT aspect = DVASPECT.DVASPECT_CONTENT) => new()
    {
        cfFormat = (ushort)format,
        dwAspect = (uint)aspect,
        lindex = -1,
        tymed = (uint)tymed
    };

    public static TheoryData<ushort, DVASPECT, TYMED, int> NativeQueryGetData_TheoryData() => new()
    {
        // Present format with every medium the data object supports.
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_HGLOBAL, (int)HRESULT.S_OK },
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_ISTREAM, (int)HRESULT.S_OK },
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_GDI, (int)HRESULT.S_OK },

        // Unicode text is stored; the native ANSI format is not synthesized for it.
        { (ushort)CLIPBOARD_FORMAT.CF_TEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_HGLOBAL, (int)HRESULT.DV_E_FORMATETC },

        // Unsupported aspect, medium, missing format and the "no format" request each get their own HRESULT.
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_THUMBNAIL, TYMED.TYMED_HGLOBAL, (int)HRESULT.DV_E_DVASPECT },
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_ICON, TYMED.TYMED_HGLOBAL, (int)HRESULT.DV_E_DVASPECT },
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_FILE, (int)HRESULT.DV_E_TYMED },
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_ISTORAGE, (int)HRESULT.DV_E_TYMED },
        { (ushort)CLIPBOARD_FORMAT.CF_UNICODETEXT, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_NULL, (int)HRESULT.DV_E_TYMED },
        { (ushort)CLIPBOARD_FORMAT.CF_HDROP, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_HGLOBAL, (int)HRESULT.DV_E_FORMATETC },
        { 0, DVASPECT.DVASPECT_CONTENT, TYMED.TYMED_HGLOBAL, (int)HRESULT.S_FALSE }
    };

    [StaTheory]
    [MemberData(nameof(NativeQueryGetData_TheoryData))]
    public unsafe void NativeQueryGetData_ReturnsExpectedHResult(ushort cfFormat, DVASPECT aspect, TYMED tymed, int expected)
    {
        // A native caller asks whether a format is available: the data object validates aspect, medium and
        // format, and answers with a distinct HRESULT for each failure.
        DataObject dataObject = new();
        dataObject.SetText("text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);

        Com.FORMATETC format = CreateNativeFormat((CLIPBOARD_FORMAT)cfFormat, tymed, aspect);

        ((int)native.Value->QueryGetData(&format)).Should().Be(expected);
    }

    [StaFact]
    public unsafe void NativeQueryGetData_NullFormat_ReturnsFormatError()
    {
        // A native caller passing a null FORMATETC pointer gets an error HRESULT rather than a crash.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);

        native.Value->QueryGetData(null).Should().Be(HRESULT.DV_E_FORMATETC);
    }

    [StaFact]
    public unsafe void NativeQueryGetData_CustomFormat_ReflectsStoredData()
    {
        // A custom registered format becomes queryable once data is stored and is not reported for other formats.
        string custom = "NativeInteropCustomFormat";
        ushort customId = (ushort)DataFormats.GetFormat(custom).Id;
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = new()
        {
            cfFormat = customId,
            dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = (uint)TYMED.TYMED_HGLOBAL
        };

        native.Value->QueryGetData(&format).Should().Be(HRESULT.DV_E_FORMATETC);

        dataObject.SetData(custom, "value");

        native.Value->QueryGetData(&format).Should().Be(HRESULT.S_OK);
    }

    [StaFact]
    public unsafe void NativeGetData_UnicodeText_ReturnsHGlobalWithText()
    {
        // A native caller requesting stored text as an HGLOBAL receives a medium holding the text, and the
        // caller owns (and frees) the memory.
        DataObject dataObject = new();
        dataObject.SetText("native text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        Com.STGMEDIUM medium = default;

        native.Value->GetData(&format, &medium).Should().Be(HRESULT.S_OK);
        try
        {
            ((TYMED)medium.tymed).Should().Be(TYMED.TYMED_HGLOBAL);
            medium.hGlobal.IsNull.Should().BeFalse();
            void* locked = PInvokeCore.GlobalLock(medium.hGlobal);
            try
            {
                Marshal.PtrToStringUni((nint)locked).Should().Be("native text");
            }
            finally
            {
                PInvokeCore.GlobalUnlock(medium.hGlobal);
            }
        }
        finally
        {
            PInvokeCore.GlobalFree(medium.hGlobal);
        }
    }

    [StaFact]
    public unsafe void NativeGetData_AnsiText_ReturnsHGlobalWithAnsiString()
    {
        // Text stored under the ANSI text format is handed to native callers as a null-terminated ANSI string.
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Text, "ansi text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_TEXT);
        Com.STGMEDIUM medium = default;

        native.Value->GetData(&format, &medium).Should().Be(HRESULT.S_OK);
        try
        {
            void* locked = PInvokeCore.GlobalLock(medium.hGlobal);
            try
            {
                Marshal.PtrToStringAnsi((nint)locked).Should().Be("ansi text");
            }
            finally
            {
                PInvokeCore.GlobalUnlock(medium.hGlobal);
            }
        }
        finally
        {
            PInvokeCore.GlobalFree(medium.hGlobal);
        }
    }

    [StaFact]
    public unsafe void NativeGetData_FormatNotPresent_FailsAndReturnsNoMedium()
    {
        // Asking for a format that was never stored fails with a format error and leaves no allocation for the
        // caller to free.
        DataObject dataObject = new();
        dataObject.SetText("text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_HDROP);
        Com.STGMEDIUM medium = default;

        native.Value->GetData(&format, &medium).Should().Be(HRESULT.DV_E_FORMATETC);

        medium.hGlobal.IsNull.Should().BeTrue();
    }

    [StaTheory]
    [InlineData(TYMED.TYMED_FILE)]
    [InlineData(TYMED.TYMED_ISTORAGE)]
    [InlineData(TYMED.TYMED_MFPICT)]
    [InlineData(TYMED.TYMED_ENHMF)]
    [InlineData(TYMED.TYMED_NULL)]
    public unsafe void NativeGetData_UnsupportedMedium_ReturnsTymedError(TYMED tymed)
    {
        // A medium the data object does not support is rejected up front and no medium is returned.
        DataObject dataObject = new();
        dataObject.SetText("text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT, tymed);
        Com.STGMEDIUM medium = default;

        native.Value->GetData(&format, &medium).Should().Be(HRESULT.DV_E_TYMED);

        medium.hGlobal.IsNull.Should().BeTrue();
    }

    [StaFact]
    public unsafe void NativeGetData_NullArguments_ReturnErrors()
    {
        // Null FORMATETC and null STGMEDIUM pointers produce error HRESULTs instead of crashing.
        DataObject dataObject = new();
        dataObject.SetText("text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        Com.STGMEDIUM medium = default;

        native.Value->GetData(null, &medium).Should().Be(HRESULT.DV_E_FORMATETC);
        native.Value->GetData(&format, null).Should().Be(HRESULT.E_POINTER);
    }

    [StaFact]
    public unsafe void NativeGetDataHere_FillsCallerAllocatedHGlobal()
    {
        // GetDataHere writes the stored text into the memory the caller allocated and keeps the caller as owner.
        DataObject dataObject = new();
        dataObject.SetText("here");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        HGLOBAL handle = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, 1);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;
        medium.hGlobal = handle;

        try
        {
            native.Value->GetDataHere(&format, &medium).Should().Be(HRESULT.S_OK);

            void* locked = PInvokeCore.GlobalLock(medium.hGlobal);
            try
            {
                Marshal.PtrToStringUni((nint)locked).Should().Be("here");
            }
            finally
            {
                PInvokeCore.GlobalUnlock(medium.hGlobal);
            }
        }
        finally
        {
            PInvokeCore.GlobalFree(medium.hGlobal);
        }
    }

    [StaFact]
    public unsafe void NativeGetDataHere_FormatNotPresentOrNullArguments_ReturnErrors()
    {
        // GetDataHere reports a missing format, and null pointers, with error HRESULTs.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;

        native.Value->GetDataHere(&format, &medium).Should().Be(HRESULT.DV_E_FORMATETC);
        native.Value->GetDataHere(null, &medium).Should().Be(HRESULT.DV_E_FORMATETC);
        native.Value->GetDataHere(&format, null).Should().Be(HRESULT.E_POINTER);
    }

    [StaFact]
    public unsafe void NativeSetData_OutsideDragLoop_IsNotImplementedAndLeavesDataUnchanged()
    {
        // Native SetData is only honored while a drag is in progress; otherwise it fails with E_NOTIMPL and the
        // stored data is untouched.
        DataObject dataObject = new();
        dataObject.SetText("original");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;

        native.Value->SetData(&format, &medium, false).Should().Be(HRESULT.E_NOTIMPL);
        native.Value->SetData(null, &medium, false).Should().Be(HRESULT.DV_E_FORMATETC);
        native.Value->SetData(&format, null, false).Should().Be(HRESULT.E_POINTER);

        dataObject.GetText().Should().Be("original");
    }

    private const string DragLoopMarkerFormat = "InShellDragLoop";
    private const string DragLoopPrivateFormat = "DragContext";

    private static unsafe HGLOBAL AllocHGlobal(ReadOnlySpan<byte> bytes)
    {
        HGLOBAL handle = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, (nuint)bytes.Length);
        void* locked = PInvokeCore.GlobalLock(handle);
        try
        {
            bytes.CopyTo(new Span<byte>(locked, bytes.Length));
        }
        finally
        {
            PInvokeCore.GlobalUnlock(handle);
        }

        return handle;
    }

    private static unsafe string ReadUnicode(HGLOBAL handle)
    {
        void* locked = PInvokeCore.GlobalLock(handle);
        try
        {
            return Marshal.PtrToStringUni((nint)locked)!;
        }
        finally
        {
            PInvokeCore.GlobalUnlock(handle);
        }
    }

    private static unsafe HGLOBAL AllocUnicode(string value) => AllocHGlobal(MemoryMarshal.AsBytes($"{value}\0".AsSpan()));

    private static unsafe void EnterNativeDragLoop(Com.IDataObject* native)
    {
        // The drag-loop marker is a BOOL set to true.
        Com.FORMATETC format = CreateNativeFormat((CLIPBOARD_FORMAT)DataFormats.GetFormat(DragLoopMarkerFormat).Id);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;
        medium.hGlobal = AllocHGlobal(BitConverter.GetBytes(1));

        // The data object takes ownership of the marker medium.
        native->SetData(&format, &medium, true).Should().Be(HRESULT.S_OK);
    }

    [StaFact]
    public unsafe void NativeSetData_DragLoopFormatCallerKeepsOwnership_StoresCopy()
    {
        // When a native caller passes a drag-loop private format without transferring ownership, the data object
        // stores its own copy: the caller can free its memory and the data can still be read back.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        EnterNativeDragLoop(native.Value);
        Com.FORMATETC format = CreateNativeFormat((CLIPBOARD_FORMAT)DataFormats.GetFormat(DragLoopPrivateFormat).Id);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;
        medium.hGlobal = AllocUnicode("copied");

        HRESULT result = native.Value->SetData(&format, &medium, false);
        bool storedCallerMemory = dataObject.TryGetData(DragLoopPrivateFormat, out DragDropFormat? stored)
            && (nint)stored!.Medium.hGlobal == (nint)medium.hGlobal;
        PInvokeCore.GlobalFree(medium.hGlobal);
        result.Should().Be(HRESULT.S_OK);
        storedCallerMemory.Should().BeFalse();

        Com.STGMEDIUM actual = default;
        native.Value->GetData(&format, &actual).Should().Be(HRESULT.S_OK);
        try
        {
            ((TYMED)actual.tymed).Should().Be(TYMED.TYMED_HGLOBAL);
            ReadUnicode(actual.hGlobal).Should().Be("copied");
        }
        finally
        {
            PInvokeCore.GlobalFree(actual.hGlobal);
        }
    }

    [StaFact]
    public unsafe void NativeSetData_DragLoopFormatOwnershipTransferred_ReturnsStoredData()
    {
        // When ownership of the medium is transferred, the data object keeps the caller's memory and hands out
        // independent copies on each read.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        EnterNativeDragLoop(native.Value);
        Com.FORMATETC format = CreateNativeFormat((CLIPBOARD_FORMAT)DataFormats.GetFormat(DragLoopPrivateFormat).Id);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;
        medium.hGlobal = AllocUnicode("owned");

        native.Value->SetData(&format, &medium, true).Should().Be(HRESULT.S_OK);

        dataObject.TryGetData(DragLoopPrivateFormat, out DragDropFormat? stored).Should().BeTrue();
        ((nint)stored!.Medium.hGlobal).Should().Be((nint)medium.hGlobal);

        Com.STGMEDIUM first = default;
        Com.STGMEDIUM second = default;
        native.Value->GetData(&format, &first).Should().Be(HRESULT.S_OK);
        native.Value->GetData(&format, &second).Should().Be(HRESULT.S_OK);
        try
        {
            ReadUnicode(first.hGlobal).Should().Be("owned");
            ReadUnicode(second.hGlobal).Should().Be("owned");
            ((nint)first.hGlobal).Should().NotBe((nint)second.hGlobal);
            ((nint)first.hGlobal).Should().NotBe((nint)medium.hGlobal);
        }
        finally
        {
            PInvokeCore.GlobalFree(first.hGlobal);
            PInvokeCore.GlobalFree(second.hGlobal);
        }
    }

    [StaFact]
    public unsafe void NativeSetData_DragLoopFormatSetTwice_ReplacesStoredData()
    {
        // Setting the same drag-loop private format again replaces the earlier value instead of keeping both.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        EnterNativeDragLoop(native.Value);
        Com.FORMATETC format = CreateNativeFormat((CLIPBOARD_FORMAT)DataFormats.GetFormat(DragLoopPrivateFormat).Id);

        foreach (string value in new[] { "first", "second" })
        {
            Com.STGMEDIUM medium = default;
            medium.tymed = Com.TYMED.TYMED_HGLOBAL;
            medium.hGlobal = AllocUnicode(value);
            HRESULT result = native.Value->SetData(&format, &medium, false);
            PInvokeCore.GlobalFree(medium.hGlobal);
            result.Should().Be(HRESULT.S_OK);
        }

        Com.STGMEDIUM actual = default;
        native.Value->GetData(&format, &actual).Should().Be(HRESULT.S_OK);
        try
        {
            ReadUnicode(actual.hGlobal).Should().Be("second");
        }
        finally
        {
            PInvokeCore.GlobalFree(actual.hGlobal);
        }
    }

    [StaFact]
    public unsafe void NativeGetData_InDragLoopFormatMissing_SucceedsWithEmptyMedium()
    {
        // While a drag is in progress, asking for a private format that was never set succeeds with an empty
        // medium rather than failing, because the shell probes for such formats.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        EnterNativeDragLoop(native.Value);
        Com.FORMATETC format = CreateNativeFormat((CLIPBOARD_FORMAT)DataFormats.GetFormat(DragLoopPrivateFormat).Id);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;

        native.Value->GetData(&format, &medium).Should().Be(HRESULT.S_OK);

        ((TYMED)medium.tymed).Should().Be(TYMED.TYMED_NULL);
        medium.hGlobal.IsNull.Should().BeTrue();
    }

    [StaFact]
    public unsafe void NativeSetData_UnrelatedFormatInDragLoop_StoresData()
    {
        // Once a drag is in progress, native SetData accepts any format, not only the shell's private ones.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        EnterNativeDragLoop(native.Value);
        Com.FORMATETC format = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        Com.STGMEDIUM medium = default;
        medium.tymed = Com.TYMED.TYMED_HGLOBAL;
        medium.hGlobal = AllocUnicode("dragged");

        HRESULT result = native.Value->SetData(&format, &medium, false);
        PInvokeCore.GlobalFree(medium.hGlobal);

        result.Should().Be(HRESULT.S_OK);
        dataObject.GetDataPresent(DataFormats.UnicodeText).Should().BeTrue();
    }

    [StaFact]
    public unsafe void NativeGetCanonicalFormatEtc_ReportsSameFormatAndClearsOutput()
    {
        // The data object has no canonical alternatives: it reports DATA_S_SAMEFORMATETC and clears the output
        // FORMATETC, and a null output pointer is an error.
        DataObject dataObject = new();
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.FORMATETC input = CreateNativeFormat(CLIPBOARD_FORMAT.CF_UNICODETEXT);
        Com.FORMATETC output = CreateNativeFormat(CLIPBOARD_FORMAT.CF_HDROP);

        ((int)native.Value->GetCanonicalFormatEtc(&input, &output)).Should().Be(DataSSameFormatEtc);
        output.cfFormat.Should().Be(0);
        output.tymed.Should().Be(0u);

        native.Value->GetCanonicalFormatEtc(&input, null).Should().Be(HRESULT.E_POINTER);
    }

    [StaFact]
    public unsafe void NativeEnumFormatEtc_UnsupportedDirectionOrNullOutput_ReturnsErrors()
    {
        // Only the "get" direction can be enumerated; the "set" direction and a null output pointer fail.
        DataObject dataObject = new();
        dataObject.SetText("text");
        using var native = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        Com.IEnumFORMATETC* enumerator = null;

        native.Value->EnumFormatEtc((uint)DATADIR.DATADIR_SET, &enumerator).Should().Be(HRESULT.E_NOTIMPL);
        ((nint)enumerator).Should().Be(0);
        native.Value->EnumFormatEtc((uint)DATADIR.DATADIR_GET, null).Should().Be(HRESULT.E_POINTER);
    }
}
