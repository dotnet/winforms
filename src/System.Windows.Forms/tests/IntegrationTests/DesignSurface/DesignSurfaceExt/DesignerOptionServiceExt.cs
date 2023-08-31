using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace DesignSurfaceExt;

internal class DesignerOptionServiceExt4SnapLines : DesignerOptionService
{
    public DesignerOptionServiceExt4SnapLines() : base() { }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptions();
        ops.UseSnapLines = true;
        ops.UseSmartTags = true;
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal class DesignerOptionServiceExt4Grid : DesignerOptionService
{
    private System.Drawing.Size _gridSize;

    public DesignerOptionServiceExt4Grid(System.Drawing.Size gridSize) : base() { _gridSize = gridSize; }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptions();
        ops.GridSize = _gridSize;
        ops.SnapToGrid = true;
        ops.ShowGrid = true;
        ops.UseSnapLines = false;
        ops.UseSmartTags = true;
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal class DesignerOptionServiceExt4GridWithoutSnapping : DesignerOptionService
{
    private System.Drawing.Size _gridSize;

    public DesignerOptionServiceExt4GridWithoutSnapping(System.Drawing.Size gridSize) : base() { _gridSize = gridSize; }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptions();
        ops.GridSize = _gridSize;
        ops.SnapToGrid = false;
        ops.ShowGrid = true;
        ops.UseSnapLines = false;
        ops.UseSmartTags = true;
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}

internal class DesignerOptionServiceExt4NoGuides : DesignerOptionService
{
    public DesignerOptionServiceExt4NoGuides() : base() { }

    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options.Parent is not null)
            return;

        DesignerOptions ops = new DesignerOptions();
        ops.GridSize = new System.Drawing.Size(8, 8);
        ops.SnapToGrid = false;
        ops.ShowGrid = false;
        ops.UseSnapLines = false;
        ops.UseSmartTags = true;
        DesignerOptionCollection wfd = CreateOptionCollection(options, "WindowsFormsDesigner", null);
        CreateOptionCollection(wfd, "General", ops);
    }
}
