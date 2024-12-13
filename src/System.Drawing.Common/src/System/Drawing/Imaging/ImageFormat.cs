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
    private static readonly ImageFormat s_memoryBMP = new(PInvokeGdiPlus.ImageFormatMemoryBMP);
    private static readonly ImageFormat s_bmp = new(PInvokeGdiPlus.ImageFormatBMP);
    private static readonly ImageFormat s_emf = new(PInvokeGdiPlus.ImageFormatEMF);
    private static readonly ImageFormat s_wmf = new(PInvokeGdiPlus.ImageFormatWMF);
    private static readonly ImageFormat s_jpeg = new(PInvokeGdiPlus.ImageFormatJPEG);
    private static readonly ImageFormat s_png = new(PInvokeGdiPlus.ImageFormatPNG);
    private static readonly ImageFormat s_gif = new(PInvokeGdiPlus.ImageFormatGIF);
    private static readonly ImageFormat s_tiff = new(PInvokeGdiPlus.ImageFormatTIFF);
    private static readonly ImageFormat s_exif = new(PInvokeGdiPlus.ImageFormatEXIF);
    private static readonly ImageFormat s_icon = new(PInvokeGdiPlus.ImageFormatIcon);
    private static readonly ImageFormat s_heif = new(PInvokeGdiPlus.ImageFormatHEIF);
    private static readonly ImageFormat s_webp = new(PInvokeGdiPlus.ImageFormatWEBP);

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
        if (Guid == PInvokeGdiPlus.ImageFormatMemoryBMP)
        {
            return "MemoryBMP";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatBMP)
        {
            return "Bmp";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatEMF)
        {
            return "Emf";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatWMF)
        {
            return "Wmf";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatGIF)
        {
            return "Gif";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatJPEG)
        {
            return "Jpeg";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatPNG)
        {
            return "Png";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatTIFF)
        {
            return "Tiff";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatEXIF)
        {
            return "Exif";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatIcon)
        {
            return "Icon";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatHEIF)
        {
            return "Heif";
        }

        if (Guid == PInvokeGdiPlus.ImageFormatWEBP)
        {
            return "Webp";
        }

        return $"[ImageFormat: {_guid}]";
    }
}
