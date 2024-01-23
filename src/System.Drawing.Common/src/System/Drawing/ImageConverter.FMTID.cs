// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

public partial class ImageConverter
{
    // OLE 1.0 Format Identifiers
    private enum FMTID : uint
    {
        FMTID_LINK = 1,
        FMTID_EMBED = 2,
        FMTID_STATIC = 3,
        FMTID_PRES = 5
    }
}
