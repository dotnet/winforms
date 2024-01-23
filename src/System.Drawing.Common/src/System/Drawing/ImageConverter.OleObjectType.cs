// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

public partial class ImageConverter
{
    // OLE 1.0 object types (As defined in Windows 3.1 headers)
    private enum OleObjectType : uint
    {
        OT_LINK = 1,
        OT_EMBEDDED = 2,
        OT_STATIC = 3
    }
}
