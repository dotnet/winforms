// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Allows for the acquiring of <see cref="Graphics"/> objects in async scenarios.
/// </summary>
public class AsyncGraphicsFactory
{
    private readonly IntPtr _hdc;

    internal AsyncGraphicsFactory(IntPtr hdc)
    {
        _hdc = hdc;
    }

    /// <summary>
    ///  Allows to acquire <see cref="Graphics"/> objects for async scenarios. Note that you cannot use the Graphics object of the <see cref="PaintEventArgs"/>,
    ///  since it is getting disposed when the scope of the <see cref="PaintEventHandler" /> proc has been exited.
    /// </summary>
    /// <returns></returns>
    public Task<Graphics> GetGraphicsAsync()
    {
        Graphics graphics = Graphics.FromHdc(_hdc);
        return Task.FromResult(graphics);
    }
}
