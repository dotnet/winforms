// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataFormatsTests
    {
        public static IEnumerable<object[]> KnownFormats_TestData()
        {
            yield return new object[] { DataFormats.Bitmap, "Bitmap" };
            yield return new object[] { DataFormats.CommaSeparatedValue, "Csv" };
            yield return new object[] { DataFormats.Dib, "DeviceIndependentBitmap" };
            yield return new object[] { DataFormats.Dif, "DataInterchangeFormat" };
            yield return new object[] { DataFormats.EnhancedMetafile, "EnhancedMetafile" };
            yield return new object[] { DataFormats.FileDrop, "FileDrop" };
            yield return new object[] { DataFormats.Html, "HTML Format" };
            yield return new object[] { DataFormats.Locale, "Locale" };
            yield return new object[] { DataFormats.MetafilePict, "MetaFilePict" };
            yield return new object[] { DataFormats.OemText, "OEMText" };
            yield return new object[] { DataFormats.Palette, "Palette" };
            yield return new object[] { DataFormats.PenData, "PenData" };
            yield return new object[] { DataFormats.Riff, "RiffAudio" };
            yield return new object[] { DataFormats.Rtf, "Rich Text Format" };
            yield return new object[] { DataFormats.Serializable, "WindowsForms10PersistentObject" };
            yield return new object[] { DataFormats.StringFormat, "System.String" };
            yield return new object[] { DataFormats.SymbolicLink, "SymbolicLink" };
            yield return new object[] { DataFormats.Text, "Text" };
            yield return new object[] { DataFormats.Tiff, "TaggedImageFileFormat" };
            yield return new object[] { DataFormats.UnicodeText, "UnicodeText" };
            yield return new object[] { DataFormats.WaveAudio, "WaveAudio" };
        }

        [Theory]
        [MemberData(nameof(KnownFormats_TestData))]
        public void DataFormats_KnownFormat_Get_ReturnsExpected(string value, string expected)
        {
            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> GetFormat_KnownString_TestData()
        {
            yield return new object[] { DataFormats.Bitmap, "Bitmap", 2 };
            yield return new object[] { DataFormats.Dib, "DeviceIndependentBitmap", 8 };
            yield return new object[] { DataFormats.Dif, "DataInterchangeFormat", 5 };
            yield return new object[] { DataFormats.EnhancedMetafile, "EnhancedMetafile", 14 };
            yield return new object[] { DataFormats.FileDrop, "FileDrop", 15 };
            yield return new object[] { DataFormats.Locale, "Locale", 16 };
            yield return new object[] { DataFormats.MetafilePict, "MetaFilePict", 3 };
            yield return new object[] { DataFormats.OemText, "OEMText", 7 };
            yield return new object[] { DataFormats.Palette, "Palette", 9 };
            yield return new object[] { DataFormats.PenData, "PenData", 10 };
            yield return new object[] { DataFormats.Riff, "RiffAudio", 11 };
            yield return new object[] { DataFormats.SymbolicLink, "SymbolicLink", 4 };
            yield return new object[] { DataFormats.Text, "Text", 1 };
            yield return new object[] { DataFormats.Tiff, "TaggedImageFileFormat", 6 };
            yield return new object[] { DataFormats.UnicodeText, "UnicodeText", 13 };
            yield return new object[] { DataFormats.WaveAudio, "WaveAudio", 12 };
        }

        [Theory]
        [MemberData(nameof(GetFormat_KnownString_TestData))]
        public void DataFormats_GetFormat_InvokeKnownString_ReturnsExpected(string format, string expectedName, int expectedId)
        {
            DataFormats.Format result = DataFormats.GetFormat(format);
            Assert.Same(result, DataFormats.GetFormat(format.ToLower()));
            Assert.Equal(expectedName, result.Name);
            Assert.Equal(expectedId, result.Id);

            // Call again to test caching behavior.
            Assert.Same(result, DataFormats.GetFormat(format));
        }

        public static IEnumerable<object[]> GetFormat_UnknownString_TestData()
        {
            yield return new object[] { DataFormats.CommaSeparatedValue, "Csv" };
            yield return new object[] { DataFormats.Html, "HTML Format" };
            yield return new object[] { DataFormats.Rtf, "Rich Text Format" };
            yield return new object[] { DataFormats.Serializable, "WindowsForms10PersistentObject" };
            yield return new object[] { DataFormats.StringFormat, "System.String" };
            yield return new object[] { "Custom", "Custom" };
        }

        [Theory]
        [MemberData(nameof(GetFormat_UnknownString_TestData))]
        public void DataFormats_GetFormat_InvokeUnknownFormatString_ReturnsExpected(string format, string expectedName)
        {
            DataFormats.Format result = DataFormats.GetFormat(format);
            Assert.Same(result, DataFormats.GetFormat(format));
            Assert.Same(result, DataFormats.GetFormat(format.ToLower()));
            Assert.Equal(expectedName, result.Name);

            // Internally the format is registered with RegisterClipboardFormat.
            // According to the documentation: "Registered clipboard formats are
            // identified by values in the range 0xC000 through 0xFFFF."
            Assert.True(result.Id >= 0xC000);
            Assert.True(result.Id < 0xFFFF);

            // Should register the format.
            Assert.Same(result, DataFormats.GetFormat(result.Id));
        }

        public static IEnumerable<object[]> GetFormat_InvalidString_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { string.Empty };
            yield return new object[] { new string('a', 256) };
        }

        [Theory]
        [MemberData(nameof(GetFormat_InvalidString_TestData))]
        public void DataFormats_GetFormat_NullOrEmptyString_ThrowsWin32Exception(string format)
        {
            Assert.Throws<Win32Exception>(() => DataFormats.GetFormat(format));
        }

        [DllImport("user32.dll")]
        private static extern int RegisterClipboardFormat(string format);

        public static IEnumerable<object[]> GetFormat_Int_TestData()
        {
            yield return new object[] { 2, "Bitmap" };
            yield return new object[] { 8, "DeviceIndependentBitmap" };
            yield return new object[] { 5, "DataInterchangeFormat" };
            yield return new object[] { 14, "EnhancedMetafile" };
            yield return new object[] { 15, "FileDrop" };
            yield return new object[] { 16, "Locale" };
            yield return new object[] { 3, "MetaFilePict" };
            yield return new object[] { 7, "OEMText" };
            yield return new object[] { 9, "Palette" };
            yield return new object[] { 10, "PenData" };
            yield return new object[] { 11, "RiffAudio" };
            yield return new object[] { 4, "SymbolicLink" };
            yield return new object[] { 1, "Text" };
            yield return new object[] { 6, "TaggedImageFileFormat" };
            yield return new object[] { 13, "UnicodeText" };
            yield return new object[] { 12, "WaveAudio" };
            yield return new object[] { 12 & 0xFFFFFFFF, "WaveAudio" };

            yield return new object[] { -1, "Format65535" };
            yield return new object[] { 1234, "Format1234" };

            int? manuallyRegisteredFormatId = null;
            int? longManuallyRegisteredFormatId = null;
            try
            {
                manuallyRegisteredFormatId = RegisterClipboardFormat("ManuallyRegisteredFormat");
                longManuallyRegisteredFormatId = RegisterClipboardFormat(new string('a', 255));
            }
            catch
            {
            }

            if (manuallyRegisteredFormatId.HasValue)
            {
                yield return new object[] { manuallyRegisteredFormatId, "ManuallyRegisteredFormat" };
                yield return new object[] { longManuallyRegisteredFormatId, new string('a', 255) };
            }
        }

        [Theory]
        [MemberData(nameof(GetFormat_Int_TestData))]
        public void DataFormats_GetFormat_InvokeId_ReturnsExpected(int id, string expectedName)
        {
            DataFormats.Format result = DataFormats.GetFormat(id);
            Assert.Equal(result, DataFormats.GetFormat(id));
            Assert.Equal(expectedName, result.Name);
            Assert.Equal(id & 0xFFFF, result.Id);
        }

        [Theory]
        [InlineData(null, -1)]
        [InlineData("", 0)]
        [InlineData("name", int.MaxValue)]
        public void DataFormats_Format_Ctor_String_Int(string name, int id)
        {
            var format = new DataFormats.Format(name, id);
            Assert.Equal(name, format.Name);
        }
    }
}
