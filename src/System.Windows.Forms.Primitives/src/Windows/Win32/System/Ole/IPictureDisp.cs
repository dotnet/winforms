// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct IPictureDisp
{
    public static object CreateObjectFromImage(Image image)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPictureDisp> picture = CreateFromImage(image);
        return (Interface)Marshal.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPictureDisp> CreateFromImage(Image image)
    {
        PICTDESC desc = PICTDESC.FromImage(image);
        ComScope<IPictureDisp> picture = new(null);
        PInvoke.OleCreatePictureIndirect(&desc, IID.Get<IPictureDisp>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }

    public Image? ToImage()
    {
        fixed (IPictureDisp* picture = &this)
        {
            OLE_HANDLE paletteHandle = default;

            IDispatch* dispatch = (IDispatch*)picture;
            using VARIANT variant = default;
            dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
            PICTYPE type = (PICTYPE)variant.data.iVal;
            if (type == PICTYPE.PICTYPE_BITMAP)
            {
                dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_HPAL, &variant).ThrowOnFailure();
                paletteHandle = (OLE_HANDLE)variant.data.uintVal;
            }

            dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_HANDLE, &variant).ThrowOnFailure();
            OLE_HANDLE handle = (OLE_HANDLE)variant.data.uintVal;

            dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
            int width = variant.data.intVal;

            dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
            int height = variant.data.intVal;

            return OLE_HANDLE.OleHandleToImage(handle, type, paletteHandle, width, height);
        }
    }
}
