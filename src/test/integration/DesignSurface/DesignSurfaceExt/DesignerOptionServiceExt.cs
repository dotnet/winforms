// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DesignSurfaceExt;

internal sealed class DesignerOptionsBuilder : DesignerOptionService
{
    private readonly DesignerOptions _options;

    public DesignerOptionsBuilder(DesignerOptions baseOptions = null)
    {
        _options = baseOptions ?? new DesignerOptions();
        _options.UseSmartTags = true;
        _options.ObjectBoundSmartTagAutoShow = false;
    }

    public DesignerOptionsBuilder WithSnapLines(bool value)
    {
        _options.UseSnapLines = value;
        return this;
    }

    public DesignerOptionsBuilder WithGrid(Size gridSize, bool snapToGrid, bool showGrid)
    {
        _options.GridSize = gridSize;
        _options.SnapToGrid = snapToGrid;
        _options.ShowGrid = showGrid;
        return this;
    }

    public DesignerOptions Build() => _options;
}

internal sealed class DesignerOptionServiceExt4SnapLines : DesignerOptionService
{
    public DesignerOptionServiceExt4SnapLines() : base() { }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptionsBuilder()
            .WithSnapLines(true)
            .Build();

        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal sealed class DesignerOptionServiceExt4Grid : DesignerOptionService
{
    private readonly Size _gridSize;

    public DesignerOptionServiceExt4Grid(Size gridSize) : base() { _gridSize = gridSize; }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptionsBuilder()
            .WithSnapLines(false)
            .WithGrid(_gridSize, snapToGrid: true, showGrid: true)
            .Build();

        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal sealed class DesignerOptionServiceExt4GridWithoutSnapping : DesignerOptionService
{
    private readonly Size _gridSize;

    public DesignerOptionServiceExt4GridWithoutSnapping(Size gridSize) : base() { _gridSize = gridSize; }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptionsBuilder()
            .WithSnapLines(false)
            .WithGrid(_gridSize, snapToGrid: false, showGrid: true)
            .Build();

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

        DesignerOptions ops = new DesignerOptionsBuilder()
            .WithSnapLines(false)
            .WithGrid(new Size(8, 8), snapToGrid: false, showGrid: false)
            .Build();

        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}
