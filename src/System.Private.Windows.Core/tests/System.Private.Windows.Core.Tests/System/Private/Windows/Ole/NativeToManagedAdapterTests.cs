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
            string result = type.TestAccessor().Dynamic.ReadStringFromHGLOBAL(HGLOBAL.Null, unicode);
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

            string result = type.TestAccessor().Dynamic.ReadStringFromHGLOBAL(global, unicode);
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

            string result = type.TestAccessor().Dynamic.ReadStringFromHGLOBAL(global, unicode);
            result.Should().NotBeEmpty();
        }
        finally
        {
            PInvokeCore.GlobalFree(global);
        }
    }
}
