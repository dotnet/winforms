// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class DataObjectTests
{
    // Each registered Clipboard format is an OS singleton,
    // we should not run this test at the same time as other tests using the same format.
    [Collection("Sequential")]
    [UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
    #pragma warning disable WFDEV005 // Type or member is obsolete
    public class ClipboardTests
    {
        public static TheoryData<string, bool> GetData_StringBool_TheoryData()
        {
            TheoryData<string, bool> theoryData = [];
            foreach (string format in s_restrictedClipboardFormats)
            {
                theoryData.Add(format, false);
                theoryData.Add(format, true);
            }

            return theoryData;
        }

        public static TheoryData<string, bool> GetData_StringBool_Unbounded_TheoryData()
        {
            TheoryData<string, bool> theoryData = [];
            foreach (string format in s_unboundedClipboardFormats)
            {
                theoryData.Add(format, false);
                theoryData.Add(format, true);
            }

            return theoryData;
        }

        [Theory]
        [MemberData(nameof(GetData_StringBool_TheoryData))]
        [MemberData(nameof(GetData_StringBool_Unbounded_TheoryData))]
        public void DataObject_GetData_InvokeStringBoolDefault_ReturnsNull(string format, bool autoConvert)
        {
            DataObject dataObject = new();
            dataObject.GetData(format, autoConvert).Should().BeNull();
        }

        public static TheoryData<string, bool> GetDataPresent_StringBool_TheoryData()
        {
            TheoryData<string, bool> theoryData = [];
            foreach (bool autoConvert in new bool[] { true, false })
            {
                foreach (string format in s_restrictedClipboardFormats)
                {
                    theoryData.Add(format, autoConvert);
                }

                foreach (string format in s_unboundedClipboardFormats)
                {
                    theoryData.Add(format, autoConvert);
                }
            }

            return theoryData;
        }

        [Theory]
        [MemberData(nameof(GetDataPresent_StringBool_TheoryData))]
        public void DataObject_GetDataPresent_InvokeStringBoolDefault_ReturnsFalse(string format, bool autoConvert)
        {
            DataObject dataObject = new();
            dataObject.GetDataPresent(format, autoConvert).Should().BeFalse();
        }

        public static TheoryData<string, string?, bool, bool> SetData_StringObject_TheoryData()
        {
            TheoryData<string, string?, bool, bool> theoryData = [];
            foreach (string format in s_restrictedClipboardFormats)
            {
                if (string.IsNullOrWhiteSpace(format) || format == typeof(Bitmap).FullName || format.StartsWith("FileName", StringComparison.Ordinal))
                {
                    continue;
                }

                theoryData.Add(format, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap);
                theoryData.Add(format, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap);
            }

            theoryData.Add(typeof(Bitmap).FullName!, null, false, true);
            theoryData.Add(typeof(Bitmap).FullName!, "input", false, true);

            theoryData.Add("FileName", null, true, false);
            theoryData.Add("FileName", "input", true, false);

            theoryData.Add("FileNameW", null, true, false);
            theoryData.Add("FileNameW", "input", true, false);

            return theoryData;
        }

        [Theory]
        [MemberData(nameof(SetData_StringObject_TheoryData))]
        public void DataObject_SetData_InvokeStringObject_GetReturnsExpected(string format, string? input, bool expectedContainsFileDropList, bool expectedContainsImage)
        {
            DataObject dataObject = new();
            dataObject.SetData(format, input);

            dataObject.GetDataPresent(format).Should().BeTrue();
            dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();
            dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();

            dataObject.GetData(format).Should().BeSameAs(input);
            dataObject.GetData(format, autoConvert: false).Should().BeSameAs(input);
            dataObject.GetData(format, autoConvert: true).Should().BeSameAs(input);

            dataObject.ContainsAudio().Should().Be(format == DataFormats.WaveAudio);
            dataObject.ContainsFileDropList().Should().Be(expectedContainsFileDropList);
            dataObject.ContainsImage().Should().Be(expectedContainsImage);
            dataObject.ContainsText().Should().Be(format == DataFormats.UnicodeText);
            dataObject.ContainsText(TextDataFormat.Text).Should().Be(format == DataFormats.UnicodeText);
            dataObject.ContainsText(TextDataFormat.UnicodeText).Should().Be(format == DataFormats.UnicodeText);
            dataObject.ContainsText(TextDataFormat.Rtf).Should().Be(format == DataFormats.Rtf);
            dataObject.ContainsText(TextDataFormat.Html).Should().Be(format == DataFormats.Html);
            dataObject.ContainsText(TextDataFormat.CommaSeparatedValue).Should().Be(format == DataFormats.CommaSeparatedValue);
        }

        public static TheoryData<string, bool, string?, bool, bool> SetData_StringBoolObject_TheoryData()
        {
            TheoryData<string, bool, string?, bool, bool> theoryData = [];

            foreach (string format in s_restrictedClipboardFormats)
            {
                if (string.IsNullOrWhiteSpace(format) || format == typeof(Bitmap).FullName || format.StartsWith("FileName", StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (bool autoConvert in new bool[] { true, false })
                {
                    theoryData.Add(format, autoConvert, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap);
                    theoryData.Add(format, autoConvert, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap);
                }
            }

            theoryData.Add(typeof(Bitmap).FullName!, false, null, false, false);
            theoryData.Add(typeof(Bitmap).FullName!, false, "input", false, false);
            theoryData.Add(typeof(Bitmap).FullName!, true, null, false, true);
            theoryData.Add(typeof(Bitmap).FullName!, true, "input", false, true);

            theoryData.Add("FileName", false, null, false, false);
            theoryData.Add("FileName", false, "input", false, false);
            theoryData.Add("FileName", true, null, true, false);
            theoryData.Add("FileName", true, "input", true, false);

            theoryData.Add("FileNameW", false, null, false, false);
            theoryData.Add("FileNameW", false, "input", false, false);
            theoryData.Add("FileNameW", true, null, true, false);
            theoryData.Add("FileNameW", true, "input", true, false);

            return theoryData;
        }

        [Theory]
        [MemberData(nameof(SetData_StringBoolObject_TheoryData))]
        public void DataObject_SetData_InvokeStringBoolObject_GetReturnsExpected(string format, bool autoConvert, string? input, bool expectedContainsFileDropList, bool expectedContainsImage)
        {
            DataObject dataObject = new();
            dataObject.SetData(format, autoConvert, input);

            dataObject.GetData(format, autoConvert: false).Should().Be(input);
            dataObject.GetData(format, autoConvert: true).Should().Be(input);

            dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();
            dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();

            dataObject.ContainsAudio().Should().Be(format == DataFormats.WaveAudio);
            dataObject.ContainsFileDropList().Should().Be(expectedContainsFileDropList);
            dataObject.ContainsImage().Should().Be(expectedContainsImage);
            dataObject.ContainsText().Should().Be(format == DataFormats.UnicodeText);
            dataObject.ContainsText(TextDataFormat.Text).Should().Be(format == DataFormats.UnicodeText);
            dataObject.ContainsText(TextDataFormat.UnicodeText).Should().Be(format == DataFormats.UnicodeText);
            dataObject.ContainsText(TextDataFormat.Rtf).Should().Be(format == DataFormats.Rtf);
            dataObject.ContainsText(TextDataFormat.Html).Should().Be(format == DataFormats.Html);
            dataObject.ContainsText(TextDataFormat.CommaSeparatedValue).Should().Be(format == DataFormats.CommaSeparatedValue);
        }
    }
}
