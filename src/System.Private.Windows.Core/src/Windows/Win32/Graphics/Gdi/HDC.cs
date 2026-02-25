// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Numerics;

namespace Windows.Win32.Graphics.Gdi;

internal readonly partial struct HDC : IHandle<HDC>
{
    HDC IHandle<HDC>.Handle => this;
    object? IHandle<HDC>.Wrapper => null;

    internal Point GetViewPointOrigin() =>
        PInvokeCore.GetViewportOrgEx(this, out Point point) ? point : Point.Empty;

    internal Point GetWindowOrigin() =>
        PInvokeCore.GetWindowOrgEx(this, out Point point) ? point : Point.Empty;

    internal Matrix3x2 GetWorldTransform()
    {
        if (PInvokeCore.GetWorldTransform(this, out XFORM matrix))
        {
            return Unsafe.As<XFORM, Matrix3x2>(ref matrix);
        }

        // If we can't get the transform, return the identity matrix.
        return Matrix3x2.Identity;
    }
}
