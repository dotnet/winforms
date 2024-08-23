// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Ole;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using Point = System.Drawing.Point;

namespace System.Windows.Forms.Tests;

public partial class DataObjectTests
{
    [Collection("Sequential")]
    public class ClipboardTests
    {
        private static readonly string[] s_restrictedClipboardFormats =
        [
            DataFormats.CommaSeparatedValue,
            DataFormats.Dib,
            DataFormats.Dif,
            DataFormats.PenData,
            DataFormats.Riff,
            DataFormats.Tiff,
            DataFormats.WaveAudio,
            DataFormats.SymbolicLink,
            DataFormats.StringFormat,
            DataFormats.Bitmap,
            DataFormats.EnhancedMetafile,
            DataFormats.FileDrop,
            DataFormats.Html,
            DataFormats.MetafilePict,
            DataFormats.OemText,
            DataFormats.Palette,
            DataFormats.Rtf,
            DataFormats.Text,
            DataFormats.UnicodeText,
            "FileName",
            "FileNameW",
            "System.Drawing.Bitmap",
            "  ",  // the last 3 return null and don't process the payload.
            string.Empty,
            null
        ];

        private static readonly string[] s_unboundedClipboardFormats =
        [
            DataFormats.Serializable,
            "something custom"
        ];

        public static TheoryData<string> GetData_String_TheoryData() => s_restrictedClipboardFormats.ToTheoryData();

        public static TheoryData<string> GetData_String_UnboundedFormat_TheoryData() => s_unboundedClipboardFormats.ToTheoryData();

        [Theory]
        [MemberData(nameof(GetData_String_TheoryData))]
        [MemberData(nameof(GetData_String_UnboundedFormat_TheoryData))]
        public void DataObject_GetData_InvokeStringDefault_ReturnsNull(string format)
        {
            DataObject dataObject = new();
            dataObject.GetData(format).Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(GetData_String_TheoryData))]
        [MemberData(nameof(GetData_String_UnboundedFormat_TheoryData))]
        public void DataObject_GetDataPresent_InvokeStringDefault_ReturnsFalse(string format)
        {
            DataObject dataObject = new();
            dataObject.GetDataPresent(format).Should().BeFalse();
        }

        public static TheoryData<string, bool> GetData_StringBool_TheoryData()
        {
            TheoryData<string, bool> theoryData = new();
            foreach (string format in s_restrictedClipboardFormats)
            {
                theoryData.Add(format, false);
                theoryData.Add(format, true);
            }

            return theoryData;
        }

        public static TheoryData<string, bool> GetData_StringBool_Unbounded_TheoryData()
        {
            TheoryData<string, bool> theoryData = new();
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
            TheoryData<string, bool> theoryData = new();
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

        public static TheoryData<string, string, bool, bool> SetData_StringObject_TheoryData()
        {
            TheoryData<string, string, bool, bool> theoryData = new();
            foreach (string format in s_restrictedClipboardFormats)
            {
                if (string.IsNullOrWhiteSpace(format) || format == typeof(Bitmap).FullName || format.StartsWith("FileName", StringComparison.Ordinal))
                {
                    continue;
                }

                theoryData.Add(format, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap);
                theoryData.Add(format, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap);
            }

            theoryData.Add(typeof(Bitmap).FullName, null, false, true);
            theoryData.Add(typeof(Bitmap).FullName, "input", false, true);

            theoryData.Add("FileName", null, true, false);
            theoryData.Add("FileName", "input", true, false);

            theoryData.Add("FileNameW", null, true, false);
            theoryData.Add("FileNameW", "input", true, false);

            return theoryData;
        }

        [Theory]
        [MemberData(nameof(SetData_StringObject_TheoryData))]
        private void DataObject_SetData_InvokeStringObject_GetReturnsExpected(string format, string input, bool expectedContainsFileDropList, bool expectedContainsImage)
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

        public static TheoryData<string, bool, string, bool, bool> SetData_StringBoolObject_TheoryData()
        {
            TheoryData<string, bool, string, bool, bool> theoryData = new();

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

            theoryData.Add(typeof(Bitmap).FullName, false, null, false, false);
            theoryData.Add(typeof(Bitmap).FullName, false, "input", false, false);
            theoryData.Add(typeof(Bitmap).FullName, true, null, false, true);
            theoryData.Add(typeof(Bitmap).FullName, true, "input", false, true);

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
        private void DataObject_SetData_InvokeStringBoolObject_GetReturnsExpected(string format, bool autoConvert, string input, bool expectedContainsFileDropList, bool expectedContainsImage)
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

        public static TheoryData<TextDataFormat, short> GetDataHere_UnicodeText_TheoryData() => new()
        {
            { TextDataFormat.Text, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT },
            { TextDataFormat.UnicodeText, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT }
        };

        [WinFormsTheory]
        [MemberData(nameof(GetDataHere_UnicodeText_TheoryData))]
        public unsafe void IComDataObjectGetDataHere_UnicodeText_Success(TextDataFormat textDataFormat, short cfFormat)
        {
            DataObject dataObject = new();
            dataObject.SetText("text", textDataFormat);
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = TYMED.TYMED_HGLOBAL,
                cfFormat = cfFormat
            };

            STGMEDIUM stgMedium = new()
            {
                tymed = TYMED.TYMED_HGLOBAL
            };

            HGLOBAL handle = PInvokeCore.GlobalAlloc(
                GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
                1);

            try
            {
                stgMedium.unionmember = handle;
                iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

                char* pChar = *(char**)stgMedium.unionmember;
                new string(pChar).Should().Be("text");
            }
            finally
            {
                PInvokeCore.GlobalFree(handle);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetDataHere_UnicodeText_TheoryData))]
        public unsafe void IComDataObjectGetDataHere_UnicodeTextNoData_ThrowsArgumentException(TextDataFormat textDataFormat, short cfFormat)
        {
            DataObject dataObject = new();
            dataObject.SetText("text", textDataFormat);
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = TYMED.TYMED_HGLOBAL,
                cfFormat = cfFormat
            };

            STGMEDIUM stgMedium = new()
            {
                tymed = TYMED.TYMED_HGLOBAL
            };

            ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<ArgumentException>();
        }

        [WinFormsFact]
        public unsafe void IComDataObjectGetDataHere_FileNames_Success()
        {
            DataObject dataObject = new();
            dataObject.SetFileDropList(["Path1", "Path2"]);
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = TYMED.TYMED_HGLOBAL,
                cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
            };

            STGMEDIUM stgMedium = new()
            {
                tymed = TYMED.TYMED_HGLOBAL
            };

            HGLOBAL handle = PInvokeCore.GlobalAlloc(
                GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
                1);

            try
            {
                stgMedium.unionmember = handle;
                iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

                DROPFILES* pDropFiles = *(DROPFILES**)stgMedium.unionmember;
                pDropFiles->pFiles.Should().Be(20u);
                pDropFiles->pt.Should().Be(Point.Empty);
                pDropFiles->fNC.Should().Be(BOOL.FALSE);
                pDropFiles->fWide.Should().Be(BOOL.TRUE);
                char* text = (char*)IntPtr.Add((IntPtr)pDropFiles, (int)pDropFiles->pFiles);
                new string(text, 0, "Path1".Length + 1 + "Path2".Length + 1 + 1).Should().Be("Path1\0Path2\0\0");
            }
            finally
            {
                PInvokeCore.GlobalFree(handle);
            }
        }

        [WinFormsFact]
        public unsafe void IComDataObjectGetDataHere_EmptyFileNames_Success()
        {
            DataObject dataObject = new();
            dataObject.SetFileDropList([]);
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = TYMED.TYMED_HGLOBAL,
                cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
            };

            STGMEDIUM stgMedium = new()
            {
                tymed = TYMED.TYMED_HGLOBAL
            };

            HGLOBAL handle = PInvokeCore.GlobalAlloc(
               GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
               (uint)sizeof(DROPFILES));

            try
            {
                stgMedium.unionmember = handle;
                iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

                DROPFILES* pDropFiles = *(DROPFILES**)stgMedium.unionmember;
                pDropFiles->pFiles.Should().Be(0u);
                pDropFiles->pt.Should().Be(Point.Empty);
                pDropFiles->fNC.Should().Be(BOOL.FALSE);
                pDropFiles->fWide.Should().Be(BOOL.FALSE);
            }
            finally
            {
                PInvokeCore.GlobalFree(handle);
            }
        }

        [WinFormsFact]
        public unsafe void IComDataObjectGetDataHere_FileNamesNoData_ThrowsArgumentException()
        {
            DataObject dataObject = new();
            dataObject.SetFileDropList(["Path1", "Path2"]);
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = TYMED.TYMED_HGLOBAL,
                cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
            };
            STGMEDIUM stgMedium = new()
            {
                tymed = TYMED.TYMED_HGLOBAL
            };
            ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<ArgumentException>();
        }

        [WinFormsFact]
        public unsafe void IComDataObjectGetDataHere_EmptyFileNamesNoData_Success()
        {
            DataObject dataObject = new();
            dataObject.SetFileDropList([]);
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = TYMED.TYMED_HGLOBAL,
                cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
            };
            STGMEDIUM stgMedium = new()
            {
                tymed = TYMED.TYMED_HGLOBAL
            };
            iComDataObject.GetDataHere(ref formatetc, ref stgMedium);
        }

        [WinFormsTheory]
        [InlineData(TYMED.TYMED_HGLOBAL, TYMED.TYMED_HGLOBAL, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT)]
        [InlineData(TYMED.TYMED_HGLOBAL, TYMED.TYMED_HGLOBAL, (short)CLIPBOARD_FORMAT.CF_HDROP)]
        [InlineData(TYMED.TYMED_ISTREAM, TYMED.TYMED_ISTREAM, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT)]
        [InlineData(TYMED.TYMED_ISTREAM, TYMED.TYMED_ISTREAM, (short)CLIPBOARD_FORMAT.CF_HDROP)]
        [InlineData(TYMED.TYMED_GDI, TYMED.TYMED_GDI, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT)]
        [InlineData(TYMED.TYMED_GDI, TYMED.TYMED_GDI, (short)CLIPBOARD_FORMAT.CF_HDROP)]
        public void IComDataObjectGetDataHere_NoDataPresentNoData_ThrowsCOMException(TYMED formatetcTymed, TYMED stgMediumTymed, short cfFormat)
        {
            DataObject dataObject = new();
            IComDataObject iComDataObject = dataObject;

            FORMATETC formatetc = new()
            {
                tymed = formatetcTymed,
                cfFormat = cfFormat
            };
            STGMEDIUM stgMedium = new()
            {
                tymed = stgMediumTymed
            };

            ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<COMException>()
                .Where(e => e.HResult == HRESULT.DV_E_FORMATETC);
        }
    }
}
