// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class CursorConverterTests
{
    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetConvertFromTheoryData))]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(byte[]), true)]
    [InlineData(typeof(Cursor), false)]
    public void CursorConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
    {
        CursorConverter converter = new();
        Assert.Equal(expected, converter.CanConvertFrom(sourceType));
    }

    [Theory]
    [InlineData("AppStarting")]
    [InlineData("appstarting")]
    public void CursorConverter_ConvertFrom_KnownCursor_ReturnsExpected(string value)
    {
        CursorConverter converter = new();
        Assert.Same(Cursors.AppStarting, converter.ConvertFrom(value));
    }

    [Fact]
    public void CursorConverter_ConvertFrom_ByteArray_ReturnsExpected()
    {
        CursorConverter converter = new();
        byte[] data = File.ReadAllBytes(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        using Cursor cursor = Assert.IsType<Cursor>(converter.ConvertFrom(data));
        Assert.NotEqual(IntPtr.Zero, cursor.Handle);
        Assert.Equal(new Point(5, 8), cursor.HotSpot);
        Assert.True(cursor.Size == new Size(32, 32) || cursor.Size == new Size(64, 64));
        Assert.Null(cursor.Tag);
    }

    [Theory]
    [InlineData(1)]
    [InlineData("NoSuchString")]
    [InlineData(null)]
    public void CursorConverter_ConvertFrom_InvalidValue_ThrowsNotSupportedException(object value)
    {
        CursorConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
    }

    [Fact]
    public void CursorConverter_ConvertFrom_InvalidByteArray_ThrowsArgumentException()
    {
        CursorConverter converter = new();
        Assert.Throws<ArgumentException>("stream", () => converter.ConvertFrom(Array.Empty<byte>()));
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(byte[]), true)]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(Cursor), false)]
    [InlineData(null, false)]
    public void CursorConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
    {
        CursorConverter converter = new();
        Assert.Equal(expected, converter.CanConvertTo(destinationType));
    }

    [Fact]
    public void CursorConverter_ConvertTo_KnownToString_ReturnsExpected()
    {
        CursorConverter converter = new();
        Assert.Equal("AppStarting", converter.ConvertTo(Cursors.AppStarting, typeof(string)));
    }

    [Fact]
    public void CursorConverter_ConvertTo_UnknownToString_ThrowsFormatException()
    {
        CursorConverter converter = new();
        Assert.Throws<FormatException>(() => converter.ConvertTo(new Cursor(2), typeof(string)));
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(1, "1")]
    public void CursorConverter_ConvertTo_ValueNotCursorToString_ReturnsExpected(object value, string expected)
    {
        CursorConverter converter = new();
        Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
    }

    [Fact]
    public void CursorConverter_ConvertTo_KnownToInstanceDescriptor_ReturnsExpected()
    {
        CursorConverter converter = new();
        InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(Cursors.AppStarting, typeof(InstanceDescriptor)));
        Assert.Empty(descriptor.Arguments);
        Assert.Equal(typeof(Cursors).GetProperty(nameof(Cursors.AppStarting), BindingFlags.Public | BindingFlags.Static), descriptor.MemberInfo);
        Assert.True(descriptor.IsComplete);

        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new Cursor(Cursors.AppStarting.Handle), typeof(InstanceDescriptor)));
    }

    [Fact]
    public void CursorConverter_ConvertTo_UnknownToInstanceDescriptor_ThrowsNotSupportedException()
    {
        CursorConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new Cursor(2), typeof(InstanceDescriptor)));
    }

    [Fact]
    public void CursorConverter_ConvertTo_NullToInstanceDescriptor_ThrowsNotSupportedException()
    {
        CursorConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(null, typeof(InstanceDescriptor)));
    }

    [Fact]
    public void CursorConverter_ConvertTo_StreamToByteArray_ReturnsExpected()
    {
        CursorConverter converter = new();
        byte[] data = File.ReadAllBytes(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
        using MemoryStream stream = new(data);
        using Cursor sourceCursor = new(stream);
        Assert.Equal(data, converter.ConvertTo(sourceCursor, typeof(byte[])));
    }

    [Fact]
    public void CursorConverter_ConvertTo_FileToByteArray_ReturnsExpected()
    {
        CursorConverter converter = new();
        string fileName = Path.Combine("bitmaps", "10x16_one_entry_32bit.ico");
        byte[] data = File.ReadAllBytes(fileName);
        using Cursor sourceCursor = new(fileName);
        Assert.Equal(data, converter.ConvertTo(sourceCursor, typeof(byte[])));
    }

    [Fact]
    public void CursorConverter_ConvertTo_KnownToByteArray_ThrowsFormatException()
    {
        CursorConverter converter = new();
        Assert.Throws<FormatException>(() => converter.ConvertTo(Cursors.AppStarting, typeof(byte[])));
        Assert.Throws<InvalidOperationException>(() => converter.ConvertTo(new Cursor(Cursors.AppStarting.Handle), typeof(byte[])));
    }

    [Fact]
    public void CursorConverter_ConvertTo_UnknownToByteArray_ThrowsFormatException()
    {
        CursorConverter converter = new();
        Assert.Throws<InvalidOperationException>(() => converter.ConvertTo(new Cursor(2), typeof(byte[])));
    }

    [Fact]
    public void CursorConverter_ConvertTo_NullToByteArray_ReturnsExpected()
    {
        CursorConverter converter = new();
        Assert.Empty(Assert.IsType<byte[]>(converter.ConvertTo(null, typeof(byte[]))));
    }

    [Fact]
    public void CursorConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        CursorConverter converter = new();
        Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
    }

    [Theory]
    [InlineData(typeof(InstanceDescriptor))]
    [InlineData(typeof(byte[]))]
    public void CursorConverter_ConvertTo_ValueNotCursor_ThrowsNotSupportedException(Type destinationType)
    {
        CursorConverter converter = new();
        Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, destinationType));
    }

    [Fact]
    public void CursorConverter_GetStandardValues_Invoke_ReturnsExpected()
    {
        CursorConverter converter = new();

        // The static accessors only provide a weak guarantee about their return values, when multiple threads
        // are involved it is possible that return values differ between calls. We need a dry run and memory barrier
        // to ensure the fields backing the property are all initialized and visible to the current thread,
        // failing to do so means that the very first call to a static cursor accessor may return a different
        // cursor instance than subsequent calls.
        converter.GetStandardValues();
        Thread.MemoryBarrier();

        ICollection<Cursor> values = converter.GetStandardValues().Cast<Cursor>().ToArray();
        Assert.Equal(28, values.Count);
        Assert.Contains(Cursors.AppStarting, values);
        Assert.Contains(Cursors.Arrow, values);
        Assert.Contains(Cursors.Cross, values);
        Assert.Contains(Cursors.Default, values);
        Assert.Contains(Cursors.Hand, values);
        Assert.Contains(Cursors.Help, values);
        Assert.Contains(Cursors.HSplit, values);
        Assert.Contains(Cursors.IBeam, values);
        Assert.Contains(Cursors.No, values);
        Assert.Contains(Cursors.NoMove2D, values);
        Assert.Contains(Cursors.NoMoveHoriz, values);
        Assert.Contains(Cursors.NoMoveVert, values);
        Assert.Contains(Cursors.PanEast, values);
        Assert.Contains(Cursors.PanNE, values);
        Assert.Contains(Cursors.PanNorth, values);
        Assert.Contains(Cursors.PanNW, values);
        Assert.Contains(Cursors.PanSE, values);
        Assert.Contains(Cursors.PanSouth, values);
        Assert.Contains(Cursors.PanSW, values);
        Assert.Contains(Cursors.PanWest, values);
        Assert.Contains(Cursors.SizeAll, values);
        Assert.Contains(Cursors.SizeNESW, values);
        Assert.Contains(Cursors.SizeNS, values);
        Assert.Contains(Cursors.SizeNWSE, values);
        Assert.Contains(Cursors.SizeWE, values);
        Assert.Contains(Cursors.UpArrow, values);
        Assert.Contains(Cursors.VSplit, values);
        Assert.Contains(Cursors.WaitCursor, values);
        Assert.NotSame(values, converter.GetStandardValues());
    }

    [Fact]
    public void CursorConverter_GetStandardValuesSupported_Invoke_ReturnsTrue()
    {
        CursorConverter converter = new();
        Assert.True(converter.GetStandardValuesSupported());
    }

    [Fact]
    public void CursorConverter_ConvertTo_FromKnownCursorHandle()
    {
        CursorConverter converter = new();
        string converted = (string)converter.ConvertTo(new Cursor(Cursors.Default.Handle), typeof(string));
        converted.Should().Be(nameof(Cursors.Default));
    }
}
