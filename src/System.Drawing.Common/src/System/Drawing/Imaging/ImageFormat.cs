// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.Versioning;

namespace System.Drawing.Imaging;

/// <summary>
///  Specifies the format of the image.
/// </summary>
[TypeConverter(typeof(ImageFormatConverter))]
public sealed class ImageFormat
{
    // Format IDs
    // private static ImageFormat undefined = new ImageFormat(new Guid("{b96b3ca9-0728-11d3-9d7b-0000f81ef32e}"));
    private static readonly ImageFormat s_memoryBMP = new(PInvokeCore.ImageFormatMemoryBMP);
    private static readonly ImageFormat s_bmp = new(PInvokeCore.ImageFormatBMP);
    private static readonly ImageFormat s_emf = new(PInvokeCore.ImageFormatEMF);
    private static readonly ImageFormat s_wmf = new(PInvokeCore.ImageFormatWMF);
    private static readonly ImageFormat s_jpeg = new(PInvokeCore.ImageFormatJPEG);
    private static readonly ImageFormat s_png = new(PInvokeCore.ImageFormatPNG);
    private static readonly ImageFormat s_gif = new(PInvokeCore.ImageFormatGIF);
    private static readonly ImageFormat s_tiff = new(PInvokeCore.ImageFormatTIFF);
    private static readonly ImageFormat s_exif = new(PInvokeCore.ImageFormatEXIF);
    private static readonly ImageFormat s_icon = new(PInvokeCore.ImageFormatIcon);
    private static readonly ImageFormat s_heif = new(PInvokeCore.ImageFormatHEIF);
    private static readonly ImageFormat s_webp = new(PInvokeCore.ImageFormatWEBP);

    private readonly Guid _guid;

    /// <summary>
    ///  Initializes a new instance of the <see cref='ImageFormat'/> class with the specified GUID.
    /// </summary>
    public ImageFormat(Guid guid) => _guid = guid;

    /// <summary>
    ///  Specifies a global unique identifier (GUID) that represents this <see cref='ImageFormat'/>.
    /// </summary>
    public Guid Guid => _guid;

    /// <summary>
    ///  Specifies a memory bitmap image format.
    /// </summary>
    public static ImageFormat MemoryBmp => s_memoryBMP;

    /// <summary>
    ///  Specifies the bitmap image format.
    /// </summary>
    public static ImageFormat Bmp => s_bmp;

    /// <summary>
    ///  Specifies the enhanced Windows metafile image format.
    /// </summary>
    public static ImageFormat Emf => s_emf;

    /// <summary>
    ///  Specifies the Windows metafile image format.
    /// </summary>
    public static ImageFormat Wmf => s_wmf;

    /// <summary>
    ///  Specifies the GIF image format.
    /// </summary>
    public static ImageFormat Gif => s_gif;

    /// <summary>
    ///  Specifies the JPEG image format.
    /// </summary>
    public static ImageFormat Jpeg => s_jpeg;

    /// <summary>
    ///  Specifies the W3C PNG image format.
    /// </summary>
    public static ImageFormat Png => s_png;

    /// <summary>
    ///  Specifies the Tag Image File Format (TIFF) image format.
    /// </summary>
    public static ImageFormat Tiff => s_tiff;

    /// <summary>
    ///  Specifies the Exchangeable Image Format (EXIF).
    /// </summary>
    public static ImageFormat Exif => s_exif;

    /// <summary>
    ///  Specifies the Windows icon image format.
    /// </summary>
    public static ImageFormat Icon => s_icon;

    /// <summary>
    ///  Specifies the High Efficiency Image Format (HEIF).
    /// </summary>
    /// <remarks>
    ///  <para>This format is supported since Windows 10 1809.</para>
    /// </remarks>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static ImageFormat Heif => s_heif;

    /// <summary>
    ///  Specifies the WebP image format.
    /// </summary>
    /// <remarks>
    ///  <para>This format is supported since Windows 10 1809.</para>
    /// </remarks>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static ImageFormat Webp => s_webp;

    /// <summary>
    ///  Returns a value indicating whether the specified object is an <see cref='ImageFormat'/> equivalent to this
    ///  <see cref='ImageFormat'/>.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? o) => o is ImageFormat format && _guid == format._guid;

    /// <summary>
    ///  Returns a hash code.
    /// </summary>
    public override int GetHashCode() => _guid.GetHashCode();

    /// <summary>
    ///  The encoder that supports this format, if any.
    /// </summary>
    internal Guid Encoder => ImageCodecInfoHelper.GetEncoderClsid(_guid);

    /// <summary>
    ///  Converts this <see cref='ImageFormat'/> to a human-readable string.
    /// </summary>
    public override string ToString()
    {
        if (Guid == PInvokeCore.ImageFormatMemoryBMP)
        {
            return "MemoryBMP";
        }

        if (Guid == PInvokeCore.ImageFormatBMP)
        {
            return "Bmp";
        }

        if (Guid == PInvokeCore.ImageFormatEMF)
        {
            return "Emf";
        }

        if (Guid == PInvokeCore.ImageFormatWMF)
        {
            return "Wmf";
        }

        if (Guid == PInvokeCore.ImageFormatGIF)
        {
            return "Gif";
        }

        if (Guid == PInvokeCore.ImageFormatJPEG)
        {
            return "Jpeg";
        }

        if (Guid == PInvokeCore.ImageFormatPNG)
        {
            return "Png";
        }

        if (Guid == PInvokeCore.ImageFormatTIFF)
        {
            return "Tiff";
        }

        if (Guid == PInvokeCore.ImageFormatEXIF)
        {
            return "Exif";
        }

        if (Guid == PInvokeCore.ImageFormatIcon)
        {
            return "Icon";
        }

        if (Guid == PInvokeCore.ImageFormatHEIF)
        {
            return "Heif";
        }

        if (Guid == PInvokeCore.ImageFormatWEBP)
        {
            return "Webp";
        }

        return $"[ImageFormat: {_guid}]";
    }
}
