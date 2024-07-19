// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Specifies the format of a <see cref='Metafile'/>.
/// </summary>
public enum MetafileType
{
    /// <summary>
    ///  Specifies an invalid type.
    /// </summary>
    Invalid = GdiPlus.MetafileType.MetafileTypeInvalid,

    /// <summary>
    ///  Specifies a standard Windows metafile.
    /// </summary>
    Wmf = GdiPlus.MetafileType.MetafileTypeWmf,

    /// <summary>
    ///  Specifies a Windows Placeable metafile.
    /// </summary>
    WmfPlaceable = GdiPlus.MetafileType.MetafileTypeWmfPlaceable,

    /// <summary>
    ///  Specifies a Windows enhanced metafile.
    /// </summary>
    Emf = GdiPlus.MetafileType.MetafileTypeEmf,

    /// <summary>
    ///  Specifies a Windows enhanced metafile plus.
    /// </summary>
    EmfPlusOnly = GdiPlus.MetafileType.MetafileTypeEmfPlusOnly,

    /// <summary>
    ///  Specifies both enhanced and enhanced plus commands in the same file.
    /// </summary>
    EmfPlusDual = GdiPlus.MetafileType.MetafileTypeEmfPlusDual
}
