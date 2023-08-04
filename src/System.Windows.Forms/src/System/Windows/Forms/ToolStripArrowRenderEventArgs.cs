﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripArrowRenderEventArgs : EventArgs
{
    private Color _arrowColor = Color.Empty;
    private bool _arrowColorChanged;

    public ToolStripArrowRenderEventArgs(
        Graphics g,
        ToolStripItem? toolStripItem,
        Rectangle arrowRectangle,
        Color arrowColor,
        ArrowDirection arrowDirection)
    {
        Graphics = g.OrThrowIfNull();
        Item = toolStripItem;
        ArrowRectangle = arrowRectangle;
        DefaultArrowColor = arrowColor;
        Direction = arrowDirection;
    }

    public Rectangle ArrowRectangle { get; set; }

    public Color ArrowColor
    {
        get => _arrowColorChanged ? _arrowColor : DefaultArrowColor;
        set
        {
            _arrowColor = value;
            _arrowColorChanged = true;
        }
    }

    internal Color DefaultArrowColor { get; set; }

    public ArrowDirection Direction { get; set; }

    public Graphics Graphics { get; }

    public ToolStripItem? Item { get; }
}
