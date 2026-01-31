// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Formats.Nrbf;
using System.Private.Windows.BinaryFormat;
using System.Text.Json;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Memory;

using Composition = System.Private.Windows.Ole.Composition<
    System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.NativeToManagedAdapterTests>,
    System.Private.Windows.Nrbf.CoreNrbfSerializer,
    System.Private.Windows.Ole.TestFormat>;
using DataFormats = System.Private.Windows.Ole.DataFormatsCore<System.Private.Windows.Ole.TestFormat>;

namespace System.Private.Windows.Ole;

public unsafe class NativeToManagedAdapterTests
{
    private static readonly byte[] s_serializedObjectID =
    [
        // FD9EA796-3B13-4370-A679-56106BB288FB
        0x96, 0xa7, 0x9e, 0xfd,
        0x13, 0x3b,
        0x70, 0x43,
        0xa6, 0x79, 0x56, 0x10, 0x6b, 0xb2, 0x88, 0xfb
    ];

    private readonly TestFormat _format = DataFormats.GetOrAddFormat(nameof(NativeToManagedAdapterTests));

    [Fact]
    public void GetData_CustomType_RawData()
    {
        MemoryStream stream = new([0xBE, 0xAD]);
        using HGlobalNativeDataObject dataObject = new(stream, (ushort)_format.Id);

        // Composition will finalize the data object.
        var composition = Composition.Create(ComHelpers.GetComPointer<IDataObject>(dataObject));
        object? data = composition.GetData(nameof(NativeToManagedAdapterTests));
        MemoryStream result = data.Should().BeOfType<MemoryStream>().Subject;
        result.ToArray().Should().Equal(0xBE, 0xAD);
    }

    [Fact]
    public void GetData_CustomType_RawData_WithPrefix()
    {
        // Write the serialized prefix to the stream.
        MemoryStream stream = new();
        stream.Write(s_serializedObjectID);
        stream.Write([0xBE, 0xAD]);

        using HGlobalNativeDataObject dataObject = new(stream, (ushort)_format.Id);

        // Composition will finalize the data object.
        var composition = Composition.Create(ComHelpers.GetComPointer<IDataObject>(dataObject));
        object? data = composition.GetData(nameof(NativeToManagedAdapterTests));

        // This is now considered a BinaryFormatter serialized object, the data we gave is invalid.
        data.Should().BeNull();
    }

    [Fact]
    public void GetData_CustomType_BinaryFormattedData()
    {
        // Write the serialized prefix to the stream.
        MemoryStream stream = new();
        stream.Write(s_serializedObjectID);
        stream.WriteBinaryFormat(new int[] { 0xBE, 0xAD });

        using HGlobalNativeDataObject dataObject = new(stream, (ushort)_format.Id);

        // Composition will finalize the data object.
        var composition = Composition.Create(ComHelpers.GetComPointer<IDataObject>(dataObject));
        object? data = composition.GetData(nameof(NativeToManagedAdapterTests));

        int[] result = data.Should().BeOfType<int[]>().Subject;
        result.Should().Equal(0xBE, 0xAD);
    }

    [Fact]
    public void GetData_CustomType_BinaryFormattedData_AsSerializationRecord()
    {
        // Write the serialized prefix to the stream.
        MemoryStream stream = new();
        stream.Write(s_serializedObjectID);
        stream.WriteBinaryFormat(new int[] { 0xBE, 0xAD });

        using HGlobalNativeDataObject dataObject = new(stream, (ushort)_format.Id);

        // Composition will finalize the data object.
        var composition = Composition.Create(ComHelpers.GetComPointer<IDataObject>(dataObject));
        composition.TryGetData(nameof(NativeToManagedAdapterTests), out SerializationRecord? data).Should().BeTrue();
        data!.TypeName.AssemblyQualifiedName.Should().Be("System.Int32[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
    }

    [Fact]
    public void GetData_CustomType_BinaryFormattedJson_AsSerializationRecord()
    {
        // Write the serialized prefix to the stream.
        MemoryStream stream = new();
        stream.Write(s_serializedObjectID);
        BinaryFormatWriter.TryWriteJsonData(stream, new JsonData<int[]>() { JsonBytes = JsonSerializer.SerializeToUtf8Bytes(new int[] { 0xBE, 0xAD }) });

        using HGlobalNativeDataObject dataObject = new(stream, (ushort)_format.Id);

        // Composition will finalize the data object.
        var composition = Composition.Create(ComHelpers.GetComPointer<IDataObject>(dataObject));
        composition.TryGetData(nameof(NativeToManagedAdapterTests), out SerializationRecord? data).Should().BeTrue();
        data!.TypeName.AssemblyQualifiedName.Should().Be("System.Private.Windows.JsonData, System.Private.Windows.VirtualJson");
    }

    [Theory]
    [BoolData]
    public void ReadStringFromHGLOBAL_InvalidHGLOBAL_Throws(bool unicode)
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        Action action = () =>
        {
            string result = type.TestAccessor.Dynamic.ReadStringFromHGLOBAL(HGLOBAL.Null, unicode);
        };

        action.Should().Throw<Win32Exception>().And.HResult.Should().Be((int)HRESULT.E_FAIL);
    }

    [Theory]
    [BoolData]
    public void ReadStringFromHGLOBAL_NoTerminator_ReturnsEmptyString(bool unicode)
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        // There is no way to create a zero-length HGLOBAL, GlobalAlloc will always allocate at least some memory.
        HGLOBAL global = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, 6);
        nuint size = PInvokeCore.GlobalSize(global);

        try
        {
            using (GlobalBuffer buffer = new(global, (uint)size))
            {
                Span<byte> span = buffer.AsSpan();
                // Fill spaces or daggers
                span.Fill(0x20);
            }

            string result = type.TestAccessor.Dynamic.ReadStringFromHGLOBAL(global, unicode);
            result.Should().BeEmpty();
        }
        finally
        {
            PInvokeCore.GlobalFree(global);
        }
    }

    [Theory]
    [BoolData]
    public void ReadStringFromHGLOBAL_Terminator_ReturnsString(bool unicode)
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        // There is no way to create a zero-length HGLOBAL, GlobalAlloc will always allocate at least some memory.
        HGLOBAL global = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, 6);
        nuint size = PInvokeCore.GlobalSize(global);

        try
        {
            using (GlobalBuffer buffer = new(global, (uint)size))
            {
                Span<byte> span = buffer.AsSpan();
                // Fill spaces or daggers, leave the last two bytes as zero
                span[..^2].Fill(0x20);
            }

            string result = type.TestAccessor.Dynamic.ReadStringFromHGLOBAL(global, unicode);
            result.Should().NotBeEmpty();
        }
        finally
        {
            PInvokeCore.GlobalFree(global);
        }
    }

    [Fact]
    public void ReadUtf8StringFromHGLOBAL_InvalidHGLOBAL_Throws()
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        Action action = () =>
        {
            string result = type.TestAccessor.Dynamic.ReadUtf8StringFromHGLOBAL(HGLOBAL.Null);
        };

        action.Should().Throw<Win32Exception>().And.HResult.Should().Be((int)HRESULT.E_FAIL);
    }

    [Fact]
    public void ReadUtf8StringFromHGLOBAL_NoTerminator_ReturnsEmptyString()
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        HGLOBAL global = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, 6);
        nuint size = PInvokeCore.GlobalSize(global);

        try
        {
            using (GlobalBuffer buffer = new(global, (uint)size))
            {
                Span<byte> span = buffer.AsSpan();
                // Fill with non-null bytes (UTF-8 compatible ASCII)
                span.Fill(0x41); // 'A'
            }

            string result = type.TestAccessor.Dynamic.ReadUtf8StringFromHGLOBAL(global);

            // Should return empty string when no null terminator is found (consistent with ReadStringFromHGLOBAL)
            result.Should().BeEmpty();
        }
        finally
        {
            PInvokeCore.GlobalFree(global);
        }
    }

    [Fact]
    public void ReadUtf8StringFromHGLOBAL_WithTerminator_ReturnsString()
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        HGLOBAL global = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, 6);
        nuint size = PInvokeCore.GlobalSize(global);

        try
        {
            using (GlobalBuffer buffer = new(global, (uint)size))
            {
                Span<byte> span = buffer.AsSpan();
                // Write "Hello" with null terminator
                span[0] = (byte)'H';
                span[1] = (byte)'e';
                span[2] = (byte)'l';
                span[3] = (byte)'l';
                span[4] = (byte)'o';
                span[5] = 0; // null terminator
            }

            string result = type.TestAccessor.Dynamic.ReadUtf8StringFromHGLOBAL(global);
            result.Should().Be("Hello");
        }
        finally
        {
            PInvokeCore.GlobalFree(global);
        }
    }

    [Fact]
    public void ReadUtf8StringFromHGLOBAL_WithEarlyTerminator_ReturnsPartialString()
    {
        Type type = typeof(Composition).GetFullNestedType("NativeToManagedAdapter");

        HGLOBAL global = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE, 10);
        nuint size = PInvokeCore.GlobalSize(global);

        try
        {
            using (GlobalBuffer buffer = new(global, (uint)size))
            {
                Span<byte> span = buffer.AsSpan();
                // Write "Hi" with null terminator in the middle, followed by garbage
                span[0] = (byte)'H';
                span[1] = (byte)'i';
                span[2] = 0; // null terminator
                span[3] = (byte)'X'; // garbage after terminator
                span[4] = (byte)'Y';
            }

            string result = type.TestAccessor.Dynamic.ReadUtf8StringFromHGLOBAL(global);

            // Should stop at the first null terminator
            result.Should().Be("Hi");
        }
        finally
        {
            PInvokeCore.GlobalFree(global);
        }
    }

    [Fact]
    public void TryGetIStreamData_DoesNotDoubleReleaseStream()
    {
        // This test verifies the fix for double-releasing the IStream.
        // https://github.com/dotnet/wpf/issues/11401
        //
        // Previously, the code wrapped the IStream in a ComScope which would Release it,
        // and then ReleaseStgMedium would also try to Release it, causing a double-release.

        MemoryStream stream = new([0xBE, 0xAD, 0xCA, 0xFE]);
        using IStreamNativeDataObject dataObject = new(stream, (ushort)_format.Id);

        IDataObject* pDataObject = ComHelpers.GetComPointer<IDataObject>(dataObject);

        // GetComPointer returns a pointer with ref count of 1.
        // Add an extra reference so we can track the ref count throughout the test.
        uint initialRefCount = pDataObject->AddRef();
        initialRefCount.Should().Be(2);

        object? data;
        uint refCountBeforeGetData;
        uint refCountAfterGetData;

        // Scope the Composition so we can observe ref count changes.
        {
            // Composition.Create calls AddRef twice (once for NativeToManagedAdapter, once for NativeToRuntimeAdapter)
            // and takes ownership of the original ref from GetComPointer.
            var composition = Composition.Create(pDataObject);

            // After Create: original(1) + our AddRef(1) + Composition's two AddRefs(2) = 4
            refCountBeforeGetData = pDataObject->AddRef();
            pDataObject->Release(); // Undo our test AddRef

            // Try to get data - this should not crash due to CFG violation
            // The IStream data object will return data via TYMED_ISTREAM
            data = composition.GetData(nameof(NativeToManagedAdapterTests));

            // Verify data was retrieved successfully
            data.Should().BeOfType<MemoryStream>();

            // Check ref count after GetData - should be unchanged from before
            // (the IStream from GetData should be properly released by ReleaseStgMedium only once)
            refCountAfterGetData = pDataObject->AddRef();
            pDataObject->Release(); // Undo our test AddRef

            refCountAfterGetData.Should().Be(
                refCountBeforeGetData,
                "GetData should not leak or double-release the IDataObject");
        }

        // Note: Composition doesn't implement IDisposable, so refs are released via GC.
        // We still hold our extra ref, so the object won't be collected.

        // Release our extra ref from the start of the test.
        uint finalRefCount = pDataObject->Release();

        // We should still have refs from Composition's adapters (they're not disposed yet).
        // The important thing is that GetData didn't corrupt the ref count.
        finalRefCount.Should().BeGreaterThan(0);

        MemoryStream result = (MemoryStream)data!;
        result.ToArray().Should().Equal(0xBE, 0xAD, 0xCA, 0xFE);
    }

    [Fact]
    public void GetData_WhenGetDataFails_ReturnsNullInsteadOfCorruptedData()
    {
        // This test verifies the fix for returning corrupted data when GetData fails.
        // https://github.com/dotnet/wpf/issues/11402
        //
        // Previously, if IDataObject::GetData returned a failure HRESULT, the code would still
        // try to read from the STGMEDIUM which could contain uninitialized data, leading to:
        // - Empty strings (if garbage memory happened to have null at the start)
        // - Mojibake/corrupted text (if garbage memory was interpreted as characters)
        //
        // This can happen in practice when there's clipboard contention - QueryGetData succeeds
        // but GetData fails because another application modified the clipboard in between.

        using FailingGetDataNativeDataObject dataObject = new((ushort)_format.Id);

        var composition = Composition.Create(ComHelpers.GetComPointer<IDataObject>(dataObject));

        // GetData should return null when the underlying GetData call fails,
        // not corrupted data from uninitialized memory.
        object? data = composition.GetData(nameof(NativeToManagedAdapterTests));

        // Should return null, not corrupted data
        data.Should().BeNull();
    }
}
