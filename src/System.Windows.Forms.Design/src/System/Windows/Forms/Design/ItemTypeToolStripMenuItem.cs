// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Associates Type with ToolStripMenuItem.
/// </summary>
internal class ItemTypeToolStripMenuItem : ToolStripMenuItem
{
    private static readonly ToolboxItem s_invalidToolboxItem = new();
    private readonly Type _itemType;
    private bool _convertTo;
    private Image? _image;

    public ItemTypeToolStripMenuItem(Type t) => _itemType = t;

    public Type ItemType
    {
        get => _itemType;
    }

    public bool ConvertTo
    {
        get => _convertTo;
        set => _convertTo = value;
    }

    public override Image? Image
    {
        get
        {
            _image ??= ToolStripDesignerUtils.GetToolboxBitmap(ItemType);

            return _image;
        }
        set
        {
        }
    }

    public override string? Text
    {
        get => ToolStripDesignerUtils.GetToolboxDescription(ItemType);
        set
        {
        }
    }

    public ToolboxItem ToolboxItem { get; set; } = s_invalidToolboxItem;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ToolboxItem = null!;
        }

        base.Dispose(disposing);
    }
}
