// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class ToolStripContainerDesigner : ParentControlDesigner
{
    private ToolStripPanel? _topToolStripPanel;
    private ToolStripPanel? _bottomToolStripPanel;
    private ToolStripPanel? _leftToolStripPanel;
    private ToolStripPanel? _rightToolStripPanel;
    private ToolStripContentPanel? _contentToolStripPanel;

    private const string TopToolStripPanelName = "TopToolStripPanel";
    private const string BottomToolStripPanelName = "BottomToolStripPanel";
    private const string LeftToolStripPanelName = "LeftToolStripPanel";
    private const string RightToolStripPanelName = "RightToolStripPanel";
    private const string ContentToolStripPanelName = "ContentPanel";

    private Control[]? _panels;
    private bool _disableDrawGrid;
    private ISelectionService? _selectionService;
    private ToolStripContainer? _toolStripContainer;

    /// <summary>
    ///  The <see cref="IDesignerHost"/> that owns this designer.
    /// </summary>
    private IDesignerHost _designerHost => GetRequiredService<IDesignerHost>();

    /// <summary>
    ///  Shadow the <see cref="ToolStripContainer.TopToolStripPanelVisible"/> property at design-time
    ///  so that we only set the visibility at design time if the user sets it directly.
    /// </summary>
    private bool TopToolStripPanelVisible
    {
        get => (bool)ShadowProperties[nameof(TopToolStripPanelVisible)]!; // This is first set in the Initialize method
        set
        {
            ShadowProperties[nameof(TopToolStripPanelVisible)] = value;
            ((ToolStripContainer)Component).TopToolStripPanelVisible = value;
        }
    }

    /// <summary>
    ///  Shadow the <see cref="ToolStripContainer.LeftToolStripPanelVisible"/> property at design-time
    ///  so that we only set the visibility at design time if the user sets it directly.
    /// </summary>
    private bool LeftToolStripPanelVisible
    {
        get => (bool)ShadowProperties[nameof(LeftToolStripPanelVisible)]!; // This is first set in the Initialize method
        set
        {
            ShadowProperties[nameof(LeftToolStripPanelVisible)] = value;
            ((ToolStripContainer)Component).LeftToolStripPanelVisible = value;
        }
    }

    /// <summary>
    /// Shadow the <see cref="ToolStripContainer.RightToolStripPanelVisible"/> property at design-time
    /// so that we only set the visibility at design time if the user sets it directly.
    /// </summary>
    private bool RightToolStripPanelVisible
    {
        get => (bool)ShadowProperties[nameof(RightToolStripPanelVisible)]!; // This is first set in the Initialize method
        set
        {
            ShadowProperties[nameof(RightToolStripPanelVisible)] = value;
            ((ToolStripContainer)Component).RightToolStripPanelVisible = value;
        }
    }

    /// <summary>
    /// Shadow the <see cref="ToolStripContainer.BottomToolStripPanelVisible"/> property at design-time
    /// so that we only set the visibility at design time if the user sets it directly.
    /// </summary>
    private bool BottomToolStripPanelVisible
    {
        get => (bool)ShadowProperties[nameof(BottomToolStripPanelVisible)]!; // This is first set in the Initialize method
        set
        {
            ShadowProperties[nameof(BottomToolStripPanelVisible)] = value;
            ((ToolStripContainer)Component).BottomToolStripPanelVisible = value;
        }
    }

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            DesignerActionListCollection actions = [];

            // Here is our action list we'll use
            ToolStripContainerActionList actionList = new(_toolStripContainer!)
            {
                AutoShow = true
            };

            actions.Add(actionList);
            return actions;
        }
    }

    /// <summary>
    /// The ToolStripContainerDesigner will re-parent any controls that are within it's lasso at
    /// creation time.
    /// </summary>
    protected override bool AllowControlLasso => false;

    protected override bool DrawGrid => !_disableDrawGrid && base.DrawGrid;

    public override IList SnapLines
        // We don't want padding SnapLines, so call directly to the internal method.
        => EdgeAndMarginSnapLines().Unwrap();

    /// <summary>
    ///  Returns the internal control designer with the specified index in the ControlDesigner.
    ///  internalControlIndex is zero-based.
    /// </summary>
    public override ControlDesigner? InternalControlDesigner(int internalControlIndex)
    {
        if (_panels is null)
        {
            return null;
        }

        if (internalControlIndex >= _panels.Length || internalControlIndex < 0)
        {
            return null;
        }

        Control panel = _panels[internalControlIndex];

        return _designerHost.GetDesigner(panel) as ControlDesigner;
    }

    /// <summary>
    ///  We want those to come with in any cut, copy operations.
    /// </summary>
    public override ICollection AssociatedComponents
    {
        get
        {
            ArrayList components = [];
            foreach (Control parent in _toolStripContainer!.Controls)
            {
                foreach (Control control in parent.Controls)
                {
                    components.Add(control);
                }
            }

            return components;
        }
    }

    protected override IComponent[]? CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
    {
        if (tool is null)
        {
            return null;
        }

        Type? toolType = tool.GetType(_designerHost);

        if (typeof(StatusStrip).IsAssignableFrom(toolType))
        {
            InvokeCreateTool(GetDesigner(_bottomToolStripPanel!), tool);
        }
        else if (typeof(ToolStrip).IsAssignableFrom(toolType))
        {
            InvokeCreateTool(GetDesigner(_topToolStripPanel!), tool);
        }
        else
        {
            InvokeCreateTool(GetDesigner(_contentToolStripPanel!), tool);
        }

        return null;
    }

    public override bool CanParent(Control control) => false;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_selectionService is not null)
        {
            _selectionService = null;
        }
    }

    private ToolStripPanelDesigner? GetDesigner(ToolStripPanel panel) => _designerHost.GetDesigner(panel) as ToolStripPanelDesigner;

    private PanelDesigner? GetDesigner(ToolStripContentPanel panel) => _designerHost.GetDesigner(panel) as PanelDesigner;

    private static ToolStripContainer? ContainerParent(Control control)
    {
        if (control is null or ToolStripContainer)
        {
            return null;
        }

        while (control.Parent is not null)
        {
            if (control.Parent is ToolStripContainer parent)
            {
                return parent;
            }

            control = control.Parent;
        }

        return null;
    }

    protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
    {
        if (!TryGetService(out SelectionManager? selectionManager))
        {
            return base.GetControlGlyph(selectionType);
        }

        // Create BodyGlyphs for all _panels
        for (int i = 0; i <= 4; i++)
        {
            Control currentPanel = _panels![i];
            Rectangle translatedBounds = BehaviorService?.ControlRectInAdornerWindow(currentPanel) ?? Rectangle.Empty;
            ControlDesigner? panelDesigner = InternalControlDesigner(i);
            OnSetCursor();

            if (panelDesigner is not null)
            {
                // create our glyph, and set its cursor appropriately
                ControlBodyGlyph bodyGlyph = new(translatedBounds, Cursor.Current, currentPanel, panelDesigner);
                selectionManager.BodyGlyphAdorner.Glyphs.Add(bodyGlyph);

                bool addGlyphs = true;
                ICollection selComponents = _selectionService!.GetSelectedComponents();
                if (!_selectionService.GetComponentSelected(_toolStripContainer!))
                {
                    foreach (object comp in selComponents)
                    {
                        if (comp is Control control)
                        {
                            ToolStripContainer? container = ContainerParent(control);
                            addGlyphs = container == _toolStripContainer;
                        }
                    }
                }

                if (addGlyphs)
                {
                    if (panelDesigner is ToolStripPanelDesigner designer)
                    {
                        AddPanelSelectionGlyph(designer, selectionManager);
                    }
                }
            }
        }

        return base.GetControlGlyph(selectionType);
    }

    private static Control? GetAssociatedControl(Component component)
    {
        Control? associatedControl = null;
        if (component is Control control)
        {
            return control;
        }

        if (component is ToolStripItem item)
        {
            Control? parent = item.GetCurrentParent();
            parent ??= item.Owner;

            return parent;
        }

        return associatedControl;
    }

    private bool CheckDropDownBounds(ToolStripDropDownItem dropDownItem, Glyph childGlyph, GlyphCollection glyphs)
    {
        if (dropDownItem is null)
        {
            return false;
        }

        Rectangle glyphBounds = childGlyph.Bounds;
        Rectangle controlBounds = BehaviorService?.ControlRectInAdornerWindow(dropDownItem.DropDown) ?? Rectangle.Empty;
        if (!glyphBounds.IntersectsWith(controlBounds))
        {
            glyphs.Insert(0, childGlyph);
        }

        return true;
    }

    /// <summary>
    ///  Checks if the associated control bounds overlap the PanelSelectionGlyph bounds.
    /// </summary>
    private bool CheckAssociatedControl(Component component, Glyph childGlyph, GlyphCollection glyphs)
    {
        bool result = false;

        if (component is ToolStripDropDownItem item)
        {
            result = CheckDropDownBounds(item, childGlyph, glyphs);
        }

        if (!result)
        {
            Control? associatedControl = GetAssociatedControl(component);
            if (associatedControl is not null
                && _toolStripContainer is not null
                && associatedControl != _toolStripContainer
                && !PInvoke.IsChild(_toolStripContainer, associatedControl))
            {
                Rectangle glyphBounds = childGlyph.Bounds;
                Rectangle controlBounds = BehaviorService?.ControlRectInAdornerWindow(associatedControl) ?? Rectangle.Empty;
                if ((component == _designerHost.RootComponent) || !glyphBounds.IntersectsWith(controlBounds))
                {
                    glyphs.Insert(0, childGlyph);
                }

                result = true;
            }
        }

        return result;
    }

    protected override Control? GetParentForComponent(IComponent component)
    {
        Type toolType = component.GetType();

        if (typeof(StatusStrip).IsAssignableFrom(toolType))
        {
            return _bottomToolStripPanel;
        }
        else if (typeof(ToolStrip).IsAssignableFrom(toolType))
        {
            return _topToolStripPanel;
        }
        else
        {
            return _contentToolStripPanel;
        }
    }

    public override void Initialize(IComponent component)
    {
        _toolStripContainer = (ToolStripContainer)component;
        base.Initialize(component);
        AutoResizeHandles = true;

        _topToolStripPanel = _toolStripContainer.TopToolStripPanel;
        _bottomToolStripPanel = _toolStripContainer.BottomToolStripPanel;
        _leftToolStripPanel = _toolStripContainer.LeftToolStripPanel;
        _rightToolStripPanel = _toolStripContainer.RightToolStripPanel;
        _contentToolStripPanel = _toolStripContainer.ContentPanel;

        _panels = [_contentToolStripPanel, _leftToolStripPanel, _rightToolStripPanel, _topToolStripPanel, _bottomToolStripPanel];

        // Add custom bitmaps for the child toolStripPanels.
        ToolboxBitmapAttribute bottomToolboxBitmapAttribute = new(typeof(ToolStripPanel), "ToolStripContainer_BottomToolStripPanel");
        ToolboxBitmapAttribute rightToolboxBitmapAttribute = new(typeof(ToolStripPanel), "ToolStripContainer_RightToolStripPanel");
        ToolboxBitmapAttribute topToolboxBitmapAttribute = new(typeof(ToolStripPanel), "ToolStripContainer_TopToolStripPanel");
        ToolboxBitmapAttribute leftToolboxBitmapAttribute = new(typeof(ToolStripPanel), "ToolStripContainer_LeftToolStripPanel");

        TypeDescriptor.AddAttributes(_bottomToolStripPanel, bottomToolboxBitmapAttribute, new DescriptionAttribute("bottom"));
        TypeDescriptor.AddAttributes(_rightToolStripPanel, rightToolboxBitmapAttribute, new DescriptionAttribute("right"));
        TypeDescriptor.AddAttributes(_leftToolStripPanel, leftToolboxBitmapAttribute, new DescriptionAttribute("left"));
        TypeDescriptor.AddAttributes(_topToolStripPanel, topToolboxBitmapAttribute, new DescriptionAttribute("top"));

        EnableDesignMode(_topToolStripPanel, TopToolStripPanelName);
        EnableDesignMode(_bottomToolStripPanel, BottomToolStripPanelName);
        EnableDesignMode(_leftToolStripPanel, LeftToolStripPanelName);
        EnableDesignMode(_rightToolStripPanel, RightToolStripPanelName);
        EnableDesignMode(_contentToolStripPanel, ContentToolStripPanelName);

        _selectionService ??= GetService<ISelectionService>();

        if (_topToolStripPanel is not null)
        {
            ToolStripPanelDesigner? panelDesigner = _designerHost?.GetDesigner(_topToolStripPanel) as ToolStripPanelDesigner;
            panelDesigner?.ExpandTopPanel();
        }

        // Set ShadowProperties
        TopToolStripPanelVisible = _toolStripContainer.TopToolStripPanelVisible;
        LeftToolStripPanelVisible = _toolStripContainer.LeftToolStripPanelVisible;
        RightToolStripPanelVisible = _toolStripContainer.RightToolStripPanelVisible;
        BottomToolStripPanelVisible = _toolStripContainer.BottomToolStripPanelVisible;
    }

    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        try
        {
            _disableDrawGrid = true;

            // we don't want to do this for the tab control designer
            // because you can't drag anything onto it anyway.
            // so we will always return false for draw grid.
            base.OnPaintAdornments(pe);
        }
        finally
        {
            _disableDrawGrid = false;
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties
    ///  the component it is designing will expose through the
    ///  TypeDescriptor object. This method is called
    ///  immediately before its corresponding "Post" method.
    ///  If you are overriding this method you should call
    ///  the base implementation before you perform your own
    ///  filtering.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        string[] shadowProps =
        [
            "TopToolStripPanelVisible",
            "LeftToolStripPanelVisible",
            "RightToolStripPanelVisible",
            "BottomToolStripPanelVisible"
        ];

        Attribute[] empty = [];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            PropertyDescriptor? propertyDescriptor = (PropertyDescriptor?)properties[shadowProps[i]];
            if (propertyDescriptor is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ToolStripContainerDesigner), propertyDescriptor, empty);
            }
        }
    }

    private void AddPanelSelectionGlyph(ToolStripPanelDesigner designer, SelectionManager selectionManager)
    {
        if (designer is null || designer.GetGlyph() is not { } childGlyph || _selectionService is null)
        {
            return;
        }

        // Now create SelectionGlyph for the panel and add it
        ICollection selectedComponents = _selectionService.GetSelectedComponents();
        foreach (object selectedComponent in selectedComponents)
        {
            Component component = (Component)selectedComponent;
            if (component is not null)
            {
                if (!CheckAssociatedControl(component, childGlyph, selectionManager.BodyGlyphAdorner.Glyphs))
                {
                    selectionManager.BodyGlyphAdorner.Glyphs.Insert(0, childGlyph);
                }
            }
        }
    }
}
