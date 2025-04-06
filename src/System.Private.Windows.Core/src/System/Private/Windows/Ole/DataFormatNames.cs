// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Shell;

namespace System.Private.Windows.Ole;

internal static partial class DataFormatNames
{
    internal const string Text = "Text";
    internal const string UnicodeText = "UnicodeText";
    internal const string Dib = "DeviceIndependentBitmap";
    internal const string Bitmap = "Bitmap";
    internal const string Emf = "EnhancedMetafile";
    internal const string Wmf = "MetaFilePict";
    internal const string SymbolicLink = "SymbolicLink";
    internal const string Dif = "DataInterchangeFormat";
    internal const string Tiff = "TaggedImageFileFormat";
    internal const string OemText = "OEMText";
    internal const string Palette = "Palette";
    internal const string PenData = "PenData";
    internal const string Riff = "RiffAudio";
    internal const string WaveAudio = "WaveAudio";
    internal const string FileDrop = "FileDrop";
    internal const string Locale = "Locale";
    internal const string Html = "HTML Format";
    internal const string Rtf = "Rich Text Format";
    internal const string Csv = "Csv";
    internal const string String = "System.String";
    internal const string Serializable = "PersistentObject";
    internal const string Xaml = "Xaml";
    internal const string XamlPackage = "XamlPackage";
    internal const string InkSerializedFormat = PInvokeCore.INK_SERIALIZED_FORMAT;
    internal const string FileNameAnsi = PInvokeCore.CFSTR_FILENAMEA;
    internal const string FileNameUnicode = PInvokeCore.CFSTR_FILENAME;
    internal const string BinaryFormatBitmap = "System.Drawing.Bitmap";
    internal const string BinaryFormatMetafile = "System.Drawing.Imaging.Metafile";

    /// <summary>
    ///  A format used internally by the drag image manager.
    /// </summary>
    internal const string DragContext = "DragContext";

    /// <summary>
    ///  A format that contains the drag image bottom-up device-independent bitmap bits.
    /// </summary>
    internal const string DragImageBits = "DragImageBits";

    /// <summary>
    ///  A format that contains the value passed to <see cref="IDragSourceHelper2.Interface.SetFlags(uint)"/>
    ///  and controls whether to allow text specified in <see cref="DROPDESCRIPTION"/> to be displayed on the drag image.
    /// </summary>
    internal const string DragSourceHelperFlags = "DragSourceHelperFlags";

    /// <summary>
    ///  A format used to identify an object's drag image window so that it's visual information can be updated dynamically.
    /// </summary>
    internal const string DragWindow = "DragWindow";

    /// <summary>
    ///  A format that is non-zero if the drop target supports drag images.
    /// </summary>
    internal const string IsShowingLayered = "IsShowingLayered";

    /// <summary>
    ///  A format that is non-zero if the drop target supports drop description text.
    /// </summary>
    internal const string IsShowingText = "IsShowingText";

    /// <summary>
    ///  A format that is non-zero if the drag image is a layered window with a size of 96x96.
    /// </summary>
    internal const string UsingDefaultDragImage = "UsingDefaultDragImage";

    /// <summary>
    ///  Adds all the "synonyms" for the specified format.
    /// </summary>
    internal static void AddMappedFormats<T>(string format, T formats)
        where T : ICollection<string>
    {
        switch (format)
        {
            case Text:
                formats.Add(String);
                formats.Add(UnicodeText);
                break;
            case UnicodeText:
                formats.Add(String);
                formats.Add(Text);
                break;
            case String:
                formats.Add(Text);
                formats.Add(UnicodeText);
                break;
            case FileDrop:
                formats.Add(FileNameUnicode);
                formats.Add(FileNameAnsi);
                break;
            case FileNameUnicode:
                formats.Add(FileDrop);
                formats.Add(FileNameAnsi);
                break;
            case FileNameAnsi:
                formats.Add(FileDrop);
                formats.Add(FileNameUnicode);
                break;
            case Bitmap:
                formats.Add(BinaryFormatBitmap);
                break;
            case BinaryFormatBitmap:
                formats.Add(Bitmap);
                break;
            case Emf:
                formats.Add(BinaryFormatMetafile);
                break;
            case BinaryFormatMetafile:
                formats.Add(Emf);
                break;
        }
    }

    /// <summary>
    ///  Check if the <paramref name="format"/> is one of the predefined formats, which formats that
    ///  correspond to primitives or are pre-defined in the OS such as strings, bitmaps, and OLE types.
    /// </summary>
    internal static bool IsPredefinedFormat(string format) => format is Text
        or UnicodeText
        or Rtf
        or Html
        or OemText
        or FileDrop
        or FileNameAnsi
        or FileNameUnicode
        or String
        or BinaryFormatBitmap
        or Csv
        or Dib
        or Dif
        or Locale
        or PenData
        or Riff
        or SymbolicLink
        or Tiff
        or WaveAudio
        or Bitmap
        or Emf
        or Palette
        or Wmf;
}
