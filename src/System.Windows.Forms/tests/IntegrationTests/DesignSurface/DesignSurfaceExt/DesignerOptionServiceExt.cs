// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace DesignSurfaceExt;

internal sealed class DesignerOptionServiceExt4SnapLines : DesignerOptionService
{
    public DesignerOptionServiceExt4SnapLines() : base() { }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new()
        {
            UseSnapLines = true,
            UseSmartTags = true
        };
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal sealed class DesignerOptionServiceExt4Grid : DesignerOptionService
{
    private System.Drawing.Size _gridSize;

    public DesignerOptionServiceExt4Grid(System.Drawing.Size gridSize) : base() { _gridSize = gridSize; }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new()
        {
            GridSize = _gridSize,
            SnapToGrid = true,
            ShowGrid = true,
            UseSnapLines = false,
            UseSmartTags = true
        };
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal sealed class DesignerOptionServiceExt4GridWithoutSnapping : DesignerOptionService
{
    private System.Drawing.Size _gridSize;

    public DesignerOptionServiceExt4GridWithoutSnapping(System.Drawing.Size gridSize) : base() { _gridSize = gridSize; }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new()
        {
            GridSize = _gridSize,
            SnapToGrid = false,
            ShowGrid = true,
            UseSnapLines = false,
            UseSmartTags = true
        };
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal sealed class DesignerOptionServiceExt4NoGuides : DesignerOptionService
{
    public DesignerOptionServiceExt4NoGuides() : base() { }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new()
        {
            GridSize = new System.Drawing.Size(8, 8),
            SnapToGrid = false,
            ShowGrid = false,
            UseSnapLines = false,
            UseSmartTags = true
        };
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}
