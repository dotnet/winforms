// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct IPictureDisp
{
    public static object CreateObjectFromImage(IImage image)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPictureDisp> picture = CreateFromImage(image);
        return (Interface)ComHelpers.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPictureDisp> CreateFromImage(IImage image)
    {
        PICTDESC desc = PICTDESC.FromImage(image);
        ComScope<IPictureDisp> picture = new(null);
        PInvokeCore.OleCreatePictureIndirect(&desc, IID.Get<IPictureDisp>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }
}
