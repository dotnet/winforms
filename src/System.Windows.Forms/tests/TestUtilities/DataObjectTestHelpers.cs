// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.Ole;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms.TestUtilities;

/// <summary>
///  Test utilities relating to <see cref="DataObject"/>.
/// </summary>
public static class DataObjectTestHelpers
{
    // These formats set and get strings by accessing HGLOBAL directly.
    public static TheoryData<string> StringFormat() =>
    [
         DataFormats.Text,
         DataFormats.UnicodeText,
         DataFormatNames.String,
         DataFormats.Rtf,
         DataFormats.Html,
         DataFormats.OemText,
         DataFormats.FileDrop,
         "FileName",
         "FileNameW"
    ];

    public static TheoryData<string> UnboundedFormat() =>
    [
        DataFormats.Serializable,
        "something custom"
    ];

    // These formats contain only known types.
    public static TheoryData<string> UndefinedRestrictedFormat() =>
    [
        DataFormats.CommaSeparatedValue,
        DataFormats.Dib,
        DataFormats.Dif,
        DataFormats.PenData,
        DataFormats.Riff,
        DataFormats.Tiff,
        DataFormats.WaveAudio,
        DataFormats.SymbolicLink,
        DataFormats.EnhancedMetafile,
        DataFormats.MetafilePict,
        DataFormats.Palette
    ];

    public static TheoryData<string> BitmapFormat() =>
    [
        DataFormats.Bitmap,
        "System.Drawing.Bitmap"
    ];

    [Serializable]
    [TypeForwardedFrom("System.ForwardAssembly")]
    public struct SimpleTestData
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
