// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles all design time behavior for the panel class.
///  This draws a visible border on the panel if it doesn't have a border
///  so the user knows where the boundaries of the panel lie.
/// </summary>
internal class SplitterPanelDesigner : PanelDesigner
{
    private IDesignerHost? _designerHost;
    private SplitContainerDesigner? _splitContainerDesigner;
    private SplitterPanel? _splitterPanel;
    private bool _selected;

    public override bool CanBeParentedTo(IDesigner parentDesigner)
    {
        return (parentDesigner is SplitContainerDesigner);
    }

    protected override InheritanceAttribute? InheritanceAttribute => _splitterPanel is not null && _splitterPanel.Parent is not null
        ? (InheritanceAttribute?)TypeDescriptor.GetAttributes(_splitterPanel.Parent)[typeof(InheritanceAttribute)]
        : base.InheritanceAttribute;

    internal bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            if (_selected)
            {
                DrawSelectedBorder();
            }
            else
            {
                EraseBorder();
            }
        }
    }

    protected override void OnDragEnter(DragEventArgs de)
    {
        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            de.Effect = DragDropEffects.None;
            return;
        }

        base.OnDragEnter(de);
    }

    protected override void OnDragOver(DragEventArgs de)
    {
        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            de.Effect = DragDropEffects.None;
            return;
        }

        base.OnDragOver(de);
    }

    protected override void OnDragLeave(EventArgs e)
    {
        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            return;
        }

        base.OnDragLeave(e);
    }

    protected override void OnDragDrop(DragEventArgs de)
    {
        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            de.Effect = DragDropEffects.None;
            return;
        }

        base.OnDragDrop(de);
    }

    protected override void OnMouseHover() => _splitContainerDesigner?.SplitterPanelHover();

    protected override void Dispose(bool disposing)
    {
        if (TryGetService(out IComponentChangeService? cs))
        {
            cs.ComponentChanged -= OnComponentChanged;
        }

        base.Dispose(disposing);
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);
        _splitterPanel = (SplitterPanel)component;

        _designerHost = (IDesignerHost?)component.Site?.GetService(typeof(IDesignerHost));
        _splitContainerDesigner = (SplitContainerDesigner?)_designerHost?.GetDesigner(_splitterPanel.Parent!);

        if (TryGetService(out IComponentChangeService? cs))
        {
            cs.ComponentChanged += OnComponentChanged;
        }

        PropertyDescriptor? lockedProp = TypeDescriptor.GetProperties(component)["Locked"];
        if (lockedProp is not null && _splitterPanel.Parent is SplitContainer)
        {
            lockedProp.SetValue(component, true);
        }
    }

    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        if (_splitterPanel?.Parent is null)
        {
            return;
        }

        if (_splitterPanel.Controls.Count == 0)
        {
            using Graphics graphics = _splitterPanel.CreateGraphics();
            DrawWaterMark(graphics);
        }
        else
        {
            // Erase WaterMark
            _splitterPanel.Invalidate();
        }
    }

    internal void DrawSelectedBorder()
    {
        Control control = Control;

        // Black or white pen?  Depends on the color of the control.
        Color penColor = control.BackColor.GetBrightness() < .5 ? ControlPaint.Light(control.BackColor) : ControlPaint.Dark(control.BackColor);
        using Pen pen = new(penColor)
        {
            DashStyle = DashStyle.Dash
        };

        Rectangle rectangle = control.ClientRectangle;
        rectangle.Inflate(-4, -4);

        using Graphics graphics = control.CreateGraphics();
        graphics.DrawRectangle(pen, rectangle);
    }

    internal void EraseBorder()
    {
        Control control = Control;
        Rectangle rectangle = control.ClientRectangle;
        rectangle.Inflate(-4, -4);

        using Pen pen = new(control.BackColor)
        {
            DashStyle = DashStyle.Dash
        };

        using Graphics graphics = control.CreateGraphics();
        graphics.DrawRectangle(pen, rectangle);
        control.Invalidate();
    }

    internal void DrawWaterMark(Graphics g)
    {
        Control control = Control;
        Rectangle rectangle = control.ClientRectangle;
        string name = control.Name;

        Color waterMarkTextColor = Color.Black;
        var uis = GetService(typeof(IUIService)) as IUIService;
        if (uis?.Styles["SmartTagText"] is Color clr)
        {
            waterMarkTextColor = clr;
        }

        using Font drawFont = new("Arial", 8);
        int watermarkX = rectangle.Width / 2 - (int)g.MeasureString(name, drawFont).Width / 2;
        int watermarkY = rectangle.Height / 2;
        TextRenderer.DrawText(g, name, drawFont, new Point(watermarkX, watermarkY), waterMarkTextColor, TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform);
    }

    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        base.OnPaintAdornments(pe);

        if (_splitterPanel!.BorderStyle == BorderStyle.None)
        {
            DrawBorder(pe.Graphics);
        }

        if (Selected)
        {
            DrawSelectedBorder();
        }

        if (_splitterPanel.Controls.Count == 0)
        {
            DrawWaterMark(pe.Graphics);
        }
    }

    /// <summary>
    /// Remove some basic properties that are not supported by the SplitterPanel.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);
        properties.Remove("Modifiers");
        properties.Remove("Locked");
        properties.Remove("GenerateMember");

        // Remove the "(Name)" property from the property grid.
        foreach (DictionaryEntry de in properties)
        {
            PropertyDescriptor descriptor = (PropertyDescriptor)de.Value!;
            if (descriptor.Name.Equals("Name") && descriptor.DesignTimeOnly)
            {
                properties[de.Key] = TypeDescriptor.CreateProperty(descriptor.ComponentType, descriptor, BrowsableAttribute.No, DesignerSerializationVisibilityAttribute.Hidden);
                break;
            }
        }
    }

    /// <summary>
    ///  Returns a list of SnapLine objects representing interesting alignment points for this control.
    ///  These SnapLines are used to assist in the positioning of the control on a parent's surface.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            ArrayList? snapLines = null;

            // We only want PaddingSnaplines for SplitterPanels.
            AddPaddingSnapLines(ref snapLines);

            return snapLines;
        }
    }

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component.
    ///  This should be one or more flags from the SelectionRules class.
    ///  If no designer provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules => Control.Parent is SplitContainer ? SelectionRules.Locked : SelectionRules.None;
}
