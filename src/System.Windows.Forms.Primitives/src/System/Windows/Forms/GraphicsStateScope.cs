// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

/// <summary>
///  Saves and restores the entire state of a <see cref="Graphics"/>.
/// </summary>
internal readonly ref struct GraphicsStateScope
{
    private readonly GraphicsState _state;
    private readonly Graphics _graphics;

    public GraphicsStateScope(Graphics graphics)
    {
        _state = graphics.Save();
        _graphics = graphics;
    }

    public void Dispose() => _graphics.Restore(_state);
}
