// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;

namespace Windows.Win32.Graphics.GdiPlus;

internal static unsafe class GpMetafileExtensions
{
    public static HENHMETAFILE GetHENHMETAFILE(this IPointer<GpMetafile> metafile)
    {
        HENHMETAFILE hemf;
        PInvokeGdiPlus.GdipGetHemfFromMetafile(metafile.GetPointer(), &hemf).ThrowIfFailed();
        GC.KeepAlive(metafile);
        return hemf;
    }

    /// <summary>
    ///  Creates a <see cref="PICTDESC"/> structure from the specified <see cref="GpMetafile"/>.
    /// </summary>
    public static PICTDESC CreatePICTDESC(this IPointer<GpMetafile> metafile)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_ENHMETAFILE
        };

        desc.Anonymous.emf.hemf = metafile.GetHENHMETAFILE();
        return desc;
    }
}
