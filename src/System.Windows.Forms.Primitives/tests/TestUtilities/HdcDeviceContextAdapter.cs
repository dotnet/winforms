// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System;

/// <summary>
///  Simple adapter for passing <see cref="HDC"/> as <see cref="IDeviceContext"/>. Does not manage HDC
///  lifetime.
/// </summary>
internal class HdcDeviceContextAdapter : IDeviceContext
{
    private readonly HDC _hdc;

    public HdcDeviceContextAdapter(HDC hdc) => _hdc = hdc;

    public IntPtr GetHdc() => (IntPtr)_hdc;
    public void ReleaseHdc() { }
    public void Dispose() { }
}
