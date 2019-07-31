// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class CursorConverterTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetConvertFromTheoryData))]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(byte[]), true)]
        [InlineData(typeof(Cursor), false)]
        public void CursorConverter_CanConvertFrom_Invoke_ReturnsExpected(Type sourceType, bool expected)
        {
            var converter = new CursorConverter();
            Assert.Equal(expected, converter.CanConvertFrom(sourceType));
        }

        [Theory]
        [InlineData("AppStarting")]
        [InlineData("appstarting")]
        public void CursorConverter_ConvertFrom_KnownCursor_ReturnsExpected(string value)
        {
            var converter = new CursorConverter();
            Assert.Same(Cursors.AppStarting, converter.ConvertFrom(value));
        }

        [Fact]
        public void CursorConverter_ConvertFrom_ByteArray_ReturnsExpected()
        {
            var converter = new CursorConverter();
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
            var converter = new CursorConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(value));
        }

        [Fact]
        public void CursorConverter_ConvertFrom_InvalidByteArray_ThrowsArgumentException()
        {
            var converter = new CursorConverter();
            Assert.Throws<ArgumentException>("stream", () => converter.ConvertFrom(new byte[0]));
        }

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(byte[]), true)]
        [InlineData(typeof(InstanceDescriptor), true)]
        [InlineData(typeof(Cursor), false)]
        [InlineData(null, false)]
        public void CursorConverter_CanConvertTo_Invoke_ReturnsExpected(Type destinationType, bool expected)
        {
            var converter = new CursorConverter();
            Assert.Equal(expected, converter.CanConvertTo(destinationType));
        }

        [Fact]
        public void CursorConverter_ConvertTo_KnownToString_ReturnsExpected()
        {
            var converter = new CursorConverter();
            Assert.Equal("AppStarting", converter.ConvertTo(Cursors.AppStarting, typeof(string)));
            Assert.Equal("AppStarting", converter.ConvertTo(new Cursor(Cursors.AppStarting.Handle), typeof(string)));
        }

        [Fact]
        public void CursorConverter_ConvertTo_UnknownToString_ThrowsFormatException()
        {
            var converter = new CursorConverter();
            Assert.Throws<FormatException>(() => converter.ConvertTo(new Cursor((IntPtr)2), typeof(string)));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData(1, "1")]
        public void CursorConverter_ConvertTo_ValueNotCursorToString_ReturnsExpected(object value, string expected)
        {
            var converter = new CursorConverter();
            Assert.Equal(expected, converter.ConvertTo(value, typeof(string)));
        }

        [Fact]
        public void CursorConverter_ConvertTo_KnownToInstanceDescriptor_ReturnsExpected()
        {
            var converter = new CursorConverter();
            InstanceDescriptor descriptor = Assert.IsType<InstanceDescriptor>(converter.ConvertTo(Cursors.AppStarting, typeof(InstanceDescriptor)));
            Assert.Empty(descriptor.Arguments);
            Assert.Equal(typeof(Cursors).GetProperty(nameof(Cursors.AppStarting), BindingFlags.Public | BindingFlags.Static), descriptor.MemberInfo);
            Assert.True(descriptor.IsComplete);

            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new Cursor(Cursors.AppStarting.Handle), typeof(InstanceDescriptor)));
        }

        [Fact]
        public void CursorConverter_ConvertTo_UnknownToInstanceDescriptor_ThrowsNotSupportedException()
        {
            var converter = new CursorConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(new Cursor((IntPtr)2), typeof(InstanceDescriptor)));
        }

        [Fact]
        public void CursorConverter_ConvertTo_NullToInstanceDescriptor_ThrowsNotSupportedException()
        {
            var converter = new CursorConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(null, typeof(InstanceDescriptor)));
        }

        [Fact]
        public void CursorConverter_ConvertTo_StreamToByteArray_ReturnsExpected()
        {
            var converter = new CursorConverter();
            byte[] data = File.ReadAllBytes(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico"));
            using var stream = new MemoryStream(data);
            var sourceCursor = new Cursor(stream);
            Assert.Equal(data, converter.ConvertTo(sourceCursor, typeof(byte[])));
        }

        [Fact]
        public void CursorConverter_ConvertTo_FileToByteArray_ReturnsExpected()
        {
            var converter = new CursorConverter();
            string fileName = Path.Combine("bitmaps", "10x16_one_entry_32bit.ico");
            byte[] data = File.ReadAllBytes(fileName);
            var sourceCursor = new Cursor(fileName);
            Assert.Equal(data, converter.ConvertTo(sourceCursor, typeof(byte[])));
        }

        [Fact]
        public void CursorConverter_ConvertTo_KnownToByteArray_ThrowsFormatException()
        {
            var converter = new CursorConverter();
            Assert.Throws<FormatException>(() => converter.ConvertTo(Cursors.AppStarting, typeof(byte[])));
            Assert.Throws<InvalidOperationException>(() => converter.ConvertTo(new Cursor(Cursors.AppStarting.Handle), typeof(byte[])));
        }

        [Fact]
        public void CursorConverter_ConvertTo_UnknownToByteArray_ThrowsFormatException()
        {
            var converter = new CursorConverter();
            Assert.Throws<InvalidOperationException>(() => converter.ConvertTo(new Cursor((IntPtr)2), typeof(byte[])));
        }

        [Fact]
        public void CursorConverter_ConvertTo_NullToByteArray_ReturnsExpected()
        {
            var converter = new CursorConverter();
            Assert.Empty(Assert.IsType<byte[]>(converter.ConvertTo(null, typeof(byte[]))));
        }

        [Fact]
        public void CursorConverter_ConvertTo_NullDestinationType_ThrowsArgumentNullException()
        {
            var converter = new CursorConverter();
            Assert.Throws<ArgumentNullException>("destinationType", () => converter.ConvertTo(new object(), null));
        }

        [Theory]
        [InlineData(typeof(InstanceDescriptor))]
        [InlineData(typeof(byte[]))]
        public void CursorConverter_ConvertTo_ValueNotCursor_ThrowsNotSupportedException(Type destinationType)
        {
            var converter = new CursorConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertTo(1, destinationType));
        }

        [Fact]
        public void CursorConverter_GetStandardValues_Invoke_ReturnsExpected()
        {
            var converter = new CursorConverter();
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
            var converter = new CursorConverter();
            Assert.True(converter.GetStandardValuesSupported());
        }
    }
}
