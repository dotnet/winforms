// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DesignSurfaceExt;

internal abstract class DesignerOptionServiceExtBase : DesignerOptionService
{
    private readonly Size _gridSize;
    private readonly bool _useSnapLines;
    private readonly bool _snapToGrid;
    private readonly bool _showGrid;

    protected DesignerOptionServiceExtBase(Size gridSize, bool useSnapLines, bool snapToGrid, bool showGrid)
    {
        _gridSize = gridSize;
        _useSnapLines = useSnapLines;
        _snapToGrid = snapToGrid;
        _showGrid = showGrid;
    }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new()
        {
            GridSize = _gridSize,
            UseSnapLines = _useSnapLines,
            SnapToGrid = _snapToGrid,
            ShowGrid = _showGrid,
            UseSmartTags = true,
            ObjectBoundSmartTagAutoShow = false
        };

        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal sealed class DesignerOptionServiceExt4SnapLines : DesignerOptionServiceExtBase
{
    public DesignerOptionServiceExt4SnapLines()
        : base(new Size(0, 0), snapToGrid: true, useSnapLines: false, showGrid: false) { }
}

internal sealed class DesignerOptionServiceExt4Grid : DesignerOptionServiceExtBase
{
    public DesignerOptionServiceExt4Grid(Size gridSize)
        : base(gridSize, useSnapLines: false, snapToGrid: true, showGrid: true) { }
}

internal sealed class DesignerOptionServiceExt4GridWithoutSnapping : DesignerOptionServiceExtBase
{
    public DesignerOptionServiceExt4GridWithoutSnapping(Size gridSize)
        : base(gridSize, useSnapLines: false, snapToGrid: false, showGrid: true) { }
}

internal sealed class DesignerOptionServiceExt4NoGuides : DesignerOptionServiceExtBase
{
    public DesignerOptionServiceExt4NoGuides()
        : base(new Size(8, 8), useSnapLines: false, snapToGrid: false, showGrid: false) { }
}
