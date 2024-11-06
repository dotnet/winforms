// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
public class ResxDataNodeTests
{
    [Fact]
    public void ResxDataNode_ResXFileRefConstructor()
    {
        string nodeName = "Node";
        ResXFileRef fileRef = new(string.Empty, string.Empty);
        ResXDataNode dataNode = new(nodeName, fileRef);

        Assert.Equal(nodeName, dataNode.Name);
        Assert.Same(fileRef, dataNode.FileRef);
    }

    [Fact]
    public void ResxDataNode_GetValue_ByteArray_FromDataNodeInfo_Framework()
    {
        using Bitmap bitmap = new(10, 10);
        var converter = TypeDescriptor.GetConverter(bitmap);
        ResXDataNode temp = new("test", converter.ConvertTo(bitmap, typeof(byte[])));
        var dataNodeInfo = temp.GetDataNodeInfo();
        ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

        object? bitmapBytes = dataNode.GetValue(typeResolver: null);
        Bitmap result = Assert.IsType<Bitmap>(converter.ConvertFrom(bitmapBytes!));
        Assert.Equal(bitmap.Size, result.Size);
    }

    [Fact]
    public void ResxDataNode_GetValue_ByteArray_FromDataNodeInfo_Core()
    {
        using Bitmap bitmap = new(10, 10);
        var converter = TypeDescriptor.GetConverter(bitmap);
        ResXDataNode temp = new("test", converter.ConvertTo(bitmap, typeof(byte[])));
        var dataNodeInfo = temp.GetDataNodeInfo();
        dataNodeInfo.TypeName = "System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

        object? bitmapBytes = dataNode.GetValue(typeResolver: null);
        var result = Assert.IsType<Bitmap>(converter.ConvertFrom(bitmapBytes!));
        Assert.Equal(bitmap.Size, result.Size);
    }

    [Fact]
    public void ResxDataNode_GetValue_Null_FromDataNodeInfo()
    {
        ResXDataNode temp = new("test", value: null);
        var dataNodeInfo = temp.GetDataNodeInfo();
        ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

        object? valueNull = dataNode.GetValue(typeResolver: null);
        Assert.Null(valueNull);
    }

    [Fact]
    public void ResxDataNode_GetValue_String_FromDataNodeInfo()
    {
        ResXDataNode temp = new("testName", "test");
        var dataNodeInfo = temp.GetDataNodeInfo();
        ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

        object? valueString = dataNode.GetValue(typeResolver: null);
        Assert.Equal("test", valueString);
    }

    [Theory]
    [MemberData(nameof(RoundTrip_BinaryFormatted_TestData))]
    public void ResxDataNode_RoundTrip_BinaryFormatted(object? value)
    {
        using BinaryFormatterScope formatterScope = new(enable: false);
        ResXDataNode dataNode = new("Test", value);
        DataNodeInfo nodeInfo = dataNode.GetDataNodeInfo();

        ResXDataNode deserializedDataNode = new(nodeInfo, basePath: null);
        Assert.Equal(value, deserializedDataNode.GetValue(typeResolver: null));
    }

    public static TheoryData<object?> RoundTrip_BinaryFormatted_TestData =>
    [
        new RectangleF(10.0f, 20.0f, 30.0f, 40.0f),
        new PointF(1.0f, 2.0f),
        new List<string> { "Jack", "Queen", "King" },
        new List<int> { 4, 3, 2, 1 },
        new Hashtable(),
        new Hashtable()
        {
            { "This", "That" }
        },
        new Hashtable()
        {
            { "Meaning", 42 }
        },
        new Hashtable()
        {
            { 42, 42 }
        },
        new Hashtable()
        {
            { 42, 42 },
            { 43, 42 }
        },
        new Hashtable()
        {
            { "Hastings", new DateTime(1066, 10, 14) }
        },
        new Hashtable()
        {
            { "Decimal", decimal.MaxValue }
        },
        new Hashtable()
        {
            { "This", "That" },
            { "TheOther", "This" },
            { "That", "This" }
        },
        new Hashtable()
        {
            { "Yowza", null },
            { "Youza", "Anakin" },
            { "Meeza", "Binks" }
        },
        new Hashtable()
        {
            { decimal.MinValue, decimal.MaxValue },
            { float.MinValue, float.MaxValue },
            { DateTime.MinValue, DateTime.MaxValue },
            { TimeSpan.MinValue, TimeSpan.MaxValue }
        },
    ];
}
