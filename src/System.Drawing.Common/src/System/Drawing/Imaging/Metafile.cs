// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Drawing.Imaging;

/// <summary>
///  Defines a graphic metafile. A metafile contains records that describe a sequence of graphics operations that
///  can be recorded and played back.
/// </summary>
[Editor($"System.Drawing.Design.MetafileEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[Serializable]
[TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public sealed unsafe class Metafile : Image, IPointer<GpMetafile>
{
    // GDI+ doesn't handle filenames over MAX_PATH very well
    private const int MaxPath = 260;

    GpMetafile* IPointer<GpMetafile>.Pointer => this.Pointer();

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified handle and
    ///  <see cref='WmfPlaceableFileHeader'/>.
    /// </summary>
    public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader, bool deleteWmf)
    {
        fixed (GdiPlus.WmfPlaceableFileHeader* header = &wmfHeader._header)
        {
            GpMetafile* metafile;
            PInvoke.GdipCreateMetafileFromWmf(
                (HMETAFILE)hmetafile,
                deleteWmf,
                header,
                &metafile).ThrowIfFailed();

            SetNativeImage((GpImage*)metafile);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified handle and
    ///  <see cref='WmfPlaceableFileHeader'/>.
    /// </summary>
    public Metafile(IntPtr henhmetafile, bool deleteEmf)
    {
        GpMetafile* metafile;
        PInvoke.GdipCreateMetafileFromEmf((HENHMETAFILE)henhmetafile, deleteEmf, &metafile).ThrowIfFailed();
        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified filename.
    /// </summary>
    public Metafile(string filename)
    {
        // Called in order to emulate exception behavior from .NET Framework related to invalid file paths.
        Path.GetFullPath(filename);
        fixed (char* fn = filename)
        {
            GpMetafile* metafile;
            PInvoke.GdipCreateMetafileFromFile(fn, &metafile).ThrowIfFailed();
            SetNativeImage((GpImage*)metafile);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, Rectangle frameRect) :
        this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified handle to a device context.
    /// </summary>
    public Metafile(IntPtr referenceHdc, EmfType emfType) :
        this(referenceHdc, emfType, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, RectangleF frameRect) :
        this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
        this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) :
        this(referenceHdc, frameRect, frameUnit, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string? description)
    {
        RectF rect = frameRect;

        GpMetafile* metafile;
        fixed (char* d = description)
        {
            PInvoke.GdipRecordMetafile(
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                &rect,
                (GdiPlus.MetafileFrameUnit)frameUnit,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
        this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
        this(referenceHdc, frameRect, frameUnit, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc) :
        this(fileName, referenceHdc, EmfType.EmfPlusDual, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, EmfType type) :
        this(fileName, referenceHdc, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect) :
        this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
        this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) :
        this(fileName, referenceHdc, frameRect, frameUnit, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, string? desc) :
        this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, desc)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string? description)
    {
        // Called in order to emulate exception behavior from .NET Framework related to invalid file paths.
        Path.GetFullPath(fileName);
        if (fileName.Length > MaxPath)
        {
            throw new PathTooLongException();
        }

        RectF rect = frameRect;
        GpMetafile* metafile;
        fixed (char* f = fileName)
        fixed (char* d = description)
        {
            PInvoke.GdipRecordMetafileFileName(
                f,
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                &rect,
                (GdiPlus.MetafileFrameUnit)frameUnit,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect) :
        this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
        this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
        this(fileName, referenceHdc, frameRect, frameUnit, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, string? description) :
        this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, description)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc) :
        this(stream, referenceHdc, EmfType.EmfPlusDual, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, EmfType type) :
        this(stream, referenceHdc, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect) :
        this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
        this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) :
        this(stream, referenceHdc, frameRect, frameUnit, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect) :
        this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
        this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
        this(stream, referenceHdc, frameRect, frameUnit, type, null)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified handle and
    ///  <see cref='WmfPlaceableFileHeader'/>.
    /// </summary>
    public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader) :
        this(hmetafile, wmfHeader, false)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified stream.
    /// </summary>
    public unsafe Metafile(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var iStream = stream.ToIStream(makeSeekable: true);

        GpMetafile* metafile = null;
        PInvoke.GdipCreateMetafileFromStream(iStream, &metafile).ThrowIfFailed();
        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified handle to a device context.
    /// </summary>
    public Metafile(IntPtr referenceHdc, EmfType emfType, string? description)
    {
        GpMetafile* metafile;
        fixed (char* d = description)
        {
            PInvoke.GdipRecordMetafile(
                (HDC)referenceHdc,
                (GdiPlus.EmfType)emfType,
                null,
                GdiPlus.MetafileFrameUnit.MetafileFrameUnitGdi,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
    ///  by the specified rectangle.
    /// </summary>
    public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string? desc)
    {
        GpMetafile* metafile;

        RectF rect = (RectangleF)frameRect;

        fixed (char* d = desc)
        {
            PInvoke.GdipRecordMetafile(
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                frameRect.IsEmpty ? null : &rect,
                (GdiPlus.MetafileFrameUnit)frameUnit,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, EmfType type, string? description)
    {
        // Called in order to emulate exception behavior from .NET Framework related to invalid file paths.
        Path.GetFullPath(fileName);

        GpMetafile* metafile;
        fixed (char* f = fileName)
        fixed (char* d = description)
        {
            PInvoke.GdipRecordMetafileFileName(
                f,
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                null,
                GdiPlus.MetafileFrameUnit.MetafileFrameUnitGdi,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string? description)
    {
        // Called in order to emulate exception behavior from .NET Framework related to invalid file paths.
        Path.GetFullPath(fileName);

        RectF rect = (RectangleF)frameRect;
        GpMetafile* metafile;
        fixed (char* f = fileName)
        fixed (char* d = description)
        {
            PInvoke.GdipRecordMetafileFileName(
                f,
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                frameRect.IsEmpty ? null : &rect,
                (GdiPlus.MetafileFrameUnit)frameUnit,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
    /// </summary>
    public unsafe Metafile(Stream stream, IntPtr referenceHdc, EmfType type, string? description)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var iStream = stream.ToIStream(makeSeekable: true);

        GpMetafile* metafile = null;
        fixed (char* d = description)
        {
            PInvoke.GdipRecordMetafileStream(
                iStream,
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                null,
                GdiPlus.MetafileFrameUnit.MetafileFrameUnitGdi,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public unsafe Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string? description)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var iStream = stream.ToIStream(makeSeekable: true);

        GpMetafile* metafile = null;
        fixed (char* d = description)
        {
            RectF rect = frameRect;
            PInvoke.GdipRecordMetafileStream(
                iStream,
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                &rect,
                (GdiPlus.MetafileFrameUnit)frameUnit,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
    /// </summary>
    public unsafe Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string? description)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var iStream = stream.ToIStream(makeSeekable: true);

        GpMetafile* metafile = null;
        fixed (char* d = description)
        {
            RectF rect = (RectangleF)frameRect;
            PInvoke.GdipRecordMetafileStream(
                iStream,
                (HDC)referenceHdc,
                (GdiPlus.EmfType)type,
                frameRect.IsEmpty ? null : &rect,
                (GdiPlus.MetafileFrameUnit)frameUnit,
                d,
                &metafile).ThrowIfFailed();
        }

        SetNativeImage((GpImage*)metafile);
    }

    private Metafile(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Metafile"/> class from a native metafile handle.
    /// </summary>
    internal Metafile(IntPtr ptr) => SetNativeImage((GpImage*)ptr);

    /// <summary>
    ///  Plays an EMF+ file.
    /// </summary>
    public void PlayRecord(EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
    {
        // Used in conjunction with Graphics.EnumerateMetafile to play an EMF+
        // The data must be DWORD aligned if it's an EMF or EMF+. It must be
        // WORD aligned if it's a WMF.

        fixed (byte* d = data)
        {
            PInvoke.GdipPlayMetafileRecord(
                this.Pointer(),
                (GdiPlus.EmfPlusRecordType)recordType,
                (uint)flags,
                (uint)dataSize,
                d).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
    /// </summary>
    public static MetafileHeader GetMetafileHeader(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
    {
        MetafileHeader header = new();

        fixed (GdiPlus.MetafileHeader* mf = &header._header)
        fixed (GdiPlus.WmfPlaceableFileHeader* wmf = &wmfHeader._header)
        {
            PInvoke.GdipGetMetafileHeaderFromWmf((HMETAFILE)hmetafile, wmf, mf).ThrowIfFailed();
            return header;
        }
    }

    /// <summary>
    ///  Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
    /// </summary>
    public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile)
    {
        MetafileHeader header = new();
        fixed (GdiPlus.MetafileHeader* mf = &header._header)
        {
            PInvoke.GdipGetMetafileHeaderFromEmf((HENHMETAFILE)henhmetafile, mf).ThrowIfFailed();
            return header;
        }
    }

    /// <summary>
    ///  Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
    /// </summary>
    public static MetafileHeader GetMetafileHeader(string fileName)
    {
        // Called in order to emulate exception behavior from .NET Framework related to invalid file paths.
        Path.GetFullPath(fileName);

        MetafileHeader header = new();

        fixed (char* fn = fileName)
        fixed (GdiPlus.MetafileHeader* mf = &header._header)
        {
            PInvoke.GdipGetMetafileHeaderFromFile(fn, mf).ThrowIfFailed();
            return header;
        }
    }

    /// <summary>
    ///  Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
    /// </summary>
    public static MetafileHeader GetMetafileHeader(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        MetafileHeader header = new();

        fixed (GdiPlus.MetafileHeader* mf = &header._header)
        {
            using var iStream = stream.ToIStream(makeSeekable: true);
            PInvoke.GdipGetMetafileHeaderFromStream(iStream, mf).ThrowIfFailed();
            return header;
        }
    }

    /// <summary>
    ///  Returns the <see cref='MetafileHeader'/> associated with this <see cref='Metafile'/>.
    /// </summary>
    public MetafileHeader GetMetafileHeader()
    {
        MetafileHeader header = new();

        fixed (GdiPlus.MetafileHeader* mf = &header._header)
        {
            PInvoke.GdipGetMetafileHeaderFromMetafile(this.Pointer(), mf).ThrowIfFailed();
            GC.KeepAlive(this);
            return header;
        }
    }

    /// <summary>
    ///  Returns a Windows handle to an enhanced <see cref='Metafile'/>.
    /// </summary>
    public IntPtr GetHenhmetafile() => this.GetHENHMETAFILE();
}
