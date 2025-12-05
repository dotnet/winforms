// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DemoConsole;

internal class DesignerOptionServiceBase : DesignerOptionService
{
    private readonly Size _gridSize;
    private readonly bool _useSnapLines;
    private readonly bool _snapToGrid;
    private readonly bool _showGrid;

    protected DesignerOptionServiceBase(Size gridSize, bool useSnapLines, bool snapToGrid, bool showGrid)
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

internal sealed class DesignerOptionService4SnapLinesExtension : DesignerOptionServiceBase
{
    public DesignerOptionService4SnapLinesExtension()
        : base(new Size(0, 0), snapToGrid: true, useSnapLines: false, showGrid: false) { }
}

internal sealed class DesignerOptionService4GridExtension : DesignerOptionServiceBase
{
    public DesignerOptionService4GridExtension(Size gridSize)
        : base(gridSize, useSnapLines: false, snapToGrid: true, showGrid: true) { }
}

internal sealed class DesignerOptionService4GridWithoutSnappingExtension : DesignerOptionServiceBase
{
    public DesignerOptionService4GridWithoutSnappingExtension(Size gridSize)
        : base(gridSize, useSnapLines: false, snapToGrid: false, showGrid: true) { }
}

internal sealed class DesignerOptionService4NoGuidesExtension : DesignerOptionServiceBase
{
    public DesignerOptionService4NoGuidesExtension ()
        : base(new Size(8, 8), useSnapLines: false, snapToGrid: false, showGrid: false) { }
}
