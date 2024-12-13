// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Graphics.GdiPlus;

namespace System.Drawing;

internal interface IGraphicsContextInfo : IPointer<GpGraphics>
{
    (HDC hdc, int saveState) GetHdc(ApplyGraphicsProperties apply, bool alwaysSaveState);
}
