﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Design;

/// <summary>
///  Designer for the ToolStripPanel.
/// </summary>
internal class ToolStripPanelDesigner : ScrollableControlDesigner
{
    private static Padding s_defaultPadding = new(0);
    private ToolStripPanel? _panel;
    private IComponentChangeService? _componentChangeService;
    private IDesignerHost? _designerHost;

    // the container selector glyph which is associated with this designer.
    private ToolStripPanelSelectionGlyph? _containerSelectorGlyph;
    private ToolStripPanelSelectionBehavior? _behavior;

    //Designer context Menu for this designer
    private BaseContextMenuStrip? _contextMenu;

    // The SelectionService..
    private ISelectionService? _selectionService;

    private MenuCommand? _designerShortCutCommand;
    private MenuCommand? _oldShortCutCommand;

    /// <summary>
    ///  Creates a Dashed-Pen of appropriate color.
    /// </summary>
    private Pen BorderPen
    {
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        get
        {
            Color penColor = Control.BackColor.GetBrightness() < .5 ?
                          ControlPaint.Light(Control.BackColor) :
                          ControlPaint.Dark(Control.BackColor);

            Pen pen = new Pen(penColor);
            pen.DashStyle = DashStyle.Dash;

            return pen;
        }
    }

    // Custom ContextMenu.
    private ContextMenuStrip DesignerContextMenu
    {
        get
        {
            if (_contextMenu is null)
            {
                _contextMenu = new BaseContextMenuStrip(Component.Site, Component as Component);
                // If multiple Items Selected don't show the custom properties...
                _contextMenu.GroupOrdering.Clear();
                _contextMenu.GroupOrdering.AddRange([StandardGroups.Code,
                    StandardGroups.Verbs,
                    StandardGroups.Custom,
                    StandardGroups.Selection,
                    StandardGroups.Edit,
                    StandardGroups.Properties
                ]);
                _contextMenu.Text = "CustomContextMenu";
            }

            return _contextMenu;
        }
    }

    // ToolStripPanels if Inherited ACT as Readonly.
    protected override InheritanceAttribute InheritanceAttribute
    {
        get
        {
            if (_panel is not null && _panel.Parent is ToolStripContainer && (base.InheritanceAttribute == InheritanceAttribute.Inherited))
            {
                return InheritanceAttribute.InheritedReadOnly;
            }

            return base.InheritanceAttribute;
        }
    }

    private Padding Padding
    {
        get
        {
            return (Padding)ShadowProperties[nameof(Padding)];
        }
        set
        {
            ShadowProperties[nameof(Padding)] = value;
        }
    }

    /// <summary>
    ///  This designer doesn't participate in SnapLines for the controls contained..
    /// </summary>
    public override bool ParticipatesWithSnapLines => false;

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component.
    ///  This should be one or more flags from the SelectionRules class.  If no designer
    ///  provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
         => _panel?.Parent is ToolStripContainer
            ? SelectionRules.Locked
            : base.SelectionRules;

    /// <summary>
    ///  Called from <see cref="ToolStripContainerActionList"/> to set the Expanded
    ///  state to false when the panel's visibility is changed.
    /// </summary>
    public ToolStripPanelSelectionGlyph? ToolStripPanelSelectorGlyph { get; private set; }

    /// <summary>
    ///  ShadowProperty.
    /// </summary>
    private bool Visible
    {
        get
        {
            return (bool)ShadowProperties[nameof(Visible)];
        }
        set
        {
            ShadowProperties[nameof(Visible)] = value;
            _panel!.Visible = value;
        }
    }

    /// <summary>
    ///  Determines if the this designer can parent to the specified designers
    ///  generally this means if the control for this designer can parent the
    ///  given ControlDesigner's designer.
    /// </summary>
    public override bool CanParent(Control control)
         => control is ToolStrip;

    /// <summary>
    ///  This designer can be parented to only ToolStripContainer.
    /// </summary>
    public override bool CanBeParentedTo(IDesigner parentDesigner)
        => _panel is not null && _panel.Parent is not ToolStripContainer;

    /// <summary>
    ///  Update the glyph whenever component is changed.
    /// </summary>
    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
        => _containerSelectorGlyph?.UpdateGlyph();

    /// <summary>
    ///  This is the worker method of all CreateTool methods.  It is the only one
    ///  that can be overridden.
    /// </summary>
    protected override IComponent[]? CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
    {
        if (tool is not null)
        {
            Type toolType = tool.GetType(_designerHost);

            if (!(typeof(ToolStrip).IsAssignableFrom(toolType)))
            {
                ToolStripContainer? parent = _panel?.Parent as ToolStripContainer;
                if (parent is not null)
                {
                    ToolStripContentPanel contentPanel = parent.ContentPanel;
                    if (contentPanel is not null)
                    {
                        PanelDesigner? designer = _designerHost?.GetDesigner(contentPanel) as PanelDesigner;
                        if (designer is not null)
                        {
                            InvokeCreateTool(designer, tool);
                        }
                    }
                }
            }
            else
            {
                base.CreateToolCore(tool, x, y, width, height, hasLocation, hasSize);
            }
        }

        return null;
    }

    /// <summary>
    ///  Disposes of this designer.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        try
        {
            // Bug 904525: Need to recover the old menu item when deleting the component.
            if (_selectionService is not null && _selectionService.PrimarySelection is null)
            {
                OnSelectionChanging(_selectionService, EventArgs.Empty);
            }

            base.Dispose(disposing);
        }
        finally
        {
            if (disposing)
            {
                _contextMenu?.Dispose();
            }

            if (_selectionService is not null)
            {
                _selectionService.SelectionChanging -= OnSelectionChanging;
                _selectionService.SelectionChanged -= OnSelectionChanged;
                _selectionService = null;
            }

            if (_componentChangeService is not null)
            {
                _componentChangeService.ComponentChanged -= OnComponentChanged;
            }

            _panel!.ControlAdded -= OnControlAdded;
            _panel.ControlRemoved -= OnControlRemoved;
        }
    }

    /// <summary>
    ///  This draws a nice border around our RaftingContainer.  We need
    ///  this because the _panel can have no border and you can't
    ///  tell where it is.
    /// </summary>
    private void DrawBorder(Graphics graphics)
    {
        Pen pen = BorderPen;
        Rectangle rc = Control.ClientRectangle;

        rc.Width--;
        rc.Height--;

        graphics.DrawRectangle(pen, rc);

        pen.Dispose();
    }

    /// <summary>
    ///  We need to expand the TopToolStripPanel only when the control is Dropped onto the form .. for the first time.
    /// </summary>
    internal void ExpandTopPanel()
    {
        if (_containerSelectorGlyph is null && Component.Site is not null)
        {
            //get the adorner window-relative coordinates for the container control
            _behavior = new ToolStripPanelSelectionBehavior(_panel!, Component.Site);
            _containerSelectorGlyph = new ToolStripPanelSelectionGlyph(Rectangle.Empty, Cursors.Default, _panel!, Component.Site, _behavior);
        }

        if (_containerSelectorGlyph is not null && _panel?.Dock == DockStyle.Top)
        {
            _panel.Padding = new Padding(0, 0, 25, 25);
            _containerSelectorGlyph.IsExpanded = true;
        }
    }

    private void OnKeyShowDesignerActions(object? sender, EventArgs e)
    {
        if (_containerSelectorGlyph is not null)
        {
            _behavior?.OnMouseDown(_containerSelectorGlyph, MouseButtons.Left, Point.Empty);
        }
    }

    /// <summary>
    ///  Since we have to initialize glyphs for SplitterPanel (which is not a part of Components.) we override the
    ///  GetGlyphs for the parent.
    /// </summary>
    internal Glyph? GetGlyph()
    {
        if (_panel is null)
        {
            return null;
        }

        // Add own Glyphs.
        if (_containerSelectorGlyph is null && Component.Site is not null)
        {
            //get the adorner window-relative coordinates for the container control
            _behavior = new ToolStripPanelSelectionBehavior(_panel, Component.Site);
            _containerSelectorGlyph = new ToolStripPanelSelectionGlyph(Rectangle.Empty, Cursors.Default, _panel, Component.Site, _behavior);
        }

        // Show the Glyph only if Panel is Visible.
        return _panel.Visible ? _containerSelectorGlyph : null;
    }

    /// <summary>
    ///  This property is used by deriving classes to determine if it returns the control being designed or some other Container
    ///  while adding a component to it.
    ///  e.g: When SplitContainer is selected and a component is being added ... the SplitContainer designer would return a
    ///  SelectedPanel as the ParentControl for all the items being added rather than itself.
    /// </summary>
    protected override Control? GetParentForComponent(IComponent component)
    {
        Type toolType = component.GetType();

        if (typeof(ToolStrip).IsAssignableFrom(toolType))
        {
            return _panel;
        }

        ToolStripContainer? parent = _panel?.Parent as ToolStripContainer;
        return parent is not null ? parent.ContentPanel : (Control?)null;
    }

    /// <summary>
    ///  Get the designer set up to run.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        if (component is null)
        {
            return;
        }

        _panel = component as ToolStripPanel;

        base.Initialize(component);

        Padding = _panel!.Padding;
        _designerHost = component?.Site?.GetService<IDesignerHost>();

        if (_selectionService is null)
        {
            _selectionService = GetService<ISelectionService>();
            _selectionService.SelectionChanging += OnSelectionChanging;
            _selectionService.SelectionChanged += OnSelectionChanged;
        }

        if (_designerHost is not null)
        {
            _componentChangeService = _designerHost.GetService<IComponentChangeService>();
        }

        if (_componentChangeService is not null)
        {
            _componentChangeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
        }

        //Hook up the ControlAdded Event
        _panel.ControlAdded += new ControlEventHandler(OnControlAdded);
        _panel.ControlRemoved += new ControlEventHandler(OnControlRemoved);
    }

    /// <summary>
    ///  We need to invalidate the glyphBounds when the glyphs are turned off.
    /// </summary>
    internal void InvalidateGlyph()
    {
        if (_containerSelectorGlyph is not null)
        {
            BehaviorService.Invalidate(_containerSelectorGlyph.Bounds);
        }
    }

    /// <summary>
    ///  Required to CodeGen the Controls collection.
    /// </summary>
    private void OnControlAdded(object? sender, ControlEventArgs e)
    {
        if (e.Control is ToolStrip)
        {
            // Change the padding which might have been set by the Behavior if the _panel is Expanded.
            _panel!.Padding = new Padding(0);
            if (_containerSelectorGlyph is not null)
            {
                _containerSelectorGlyph.IsExpanded = false;
            }

            // Smoke the dock property whenever we add a toolstrip to a toolstrip _panel.
            PropertyDescriptor? dockProp = TypeDescriptor.GetProperties(e.Control)["Dock"];
            dockProp?.SetValue(e.Control, DockStyle.None);

            RefreshSelection();
        }
    }

    /// <summary>
    ///  Required to CodeGen the Controls collection.
    /// </summary>
    private void OnControlRemoved(object? sender, ControlEventArgs e)
    {
        if (_panel is not null && _panel.Controls.Count == 0)
        {
            if (_containerSelectorGlyph is not null)
            {
                _containerSelectorGlyph.IsExpanded = false;
            }

            RefreshSelection();
        }
    }

    private void RefreshSelection()
    {
        if (_designerHost is not null && !_designerHost.Loading)
        {
            SelectionManager selectionManager = GetService<SelectionManager>();
            selectionManager?.Refresh();
        }
    }

    /// <summary>
    ///  Called when ContextMenu is invoked.
    /// </summary>
    protected override void OnContextMenu(int x, int y)
    {
        if (_panel is not null && _panel.Parent is ToolStripContainer)
        {
            DesignerContextMenu.Show(x, y);
        }
        else
        {
            base.OnContextMenu(x, y);
        }
    }

    private void OnSelectionChanging(object? sender, EventArgs e)
    {
        //Remove our DesignerShortCutHandler
        if (_designerShortCutCommand is not null)
        {
            IMenuCommandService menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            if (menuCommandService is not null)
            {
                menuCommandService.RemoveCommand(_designerShortCutCommand);
                if (_oldShortCutCommand is not null)
                {
                    menuCommandService.AddCommand(_oldShortCutCommand);
                }
            }

            _designerShortCutCommand = null;
        }
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        if (_selectionService?.PrimarySelection == _panel)
        {
            _designerShortCutCommand = new MenuCommand(OnKeyShowDesignerActions, MenuCommands.KeyInvokeSmartTag);
            IMenuCommandService menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            if (menuCommandService is not null)
            {
                _oldShortCutCommand = menuCommandService.FindCommand(MenuCommands.KeyInvokeSmartTag);
                if (_oldShortCutCommand is not null)
                {
                    menuCommandService.RemoveCommand(_oldShortCutCommand);
                }

                menuCommandService.AddCommand(_designerShortCutCommand);
            }
        }
    }

    /// <summary>
    ///  Paint the borders for the panels.
    /// </summary>
    protected override void OnPaintAdornments(PaintEventArgs paintEvent)
    {
        if (!ToolStripDesignerUtils.DisplayInformation.TerminalServer && !ToolStripDesignerUtils.DisplayInformation.HighContrast && !ToolStripDesignerUtils.DisplayInformation.LowResolution)
        {
            using (Brush brush = new SolidBrush(Color.FromArgb(50, Color.White)))
            {
                paintEvent.Graphics.FillRectangle(brush, _panel!.ClientRectangle);
            }
        }

        DrawBorder(paintEvent.Graphics);
    }

    protected override void PreFilterEvents(IDictionary events)
    {
        base.PreFilterEvents(events);
        EventDescriptor? eventDescriptor;

        if (_panel?.Parent is ToolStripContainer)
        {
            string[] noBrowseEvents =
            [
                nameof(ToolStripPanel.AutoSizeChanged),
                nameof(ToolStripPanel.BindingContextChanged),
                nameof(ToolStripPanel.CausesValidationChanged),
                nameof(ToolStripPanel.ChangeUICues),
                nameof(ToolStripPanel.DockChanged),
                nameof(ToolStripPanel.DragDrop),
                nameof(ToolStripPanel.DragEnter),
                nameof(ToolStripPanel.DragLeave),
                nameof(ToolStripPanel.DragOver),
                nameof(ToolStripPanel.EnabledChanged),
                nameof(ToolStripPanel.FontChanged),
                nameof(ToolStripPanel.ForeColorChanged),
                nameof(ToolStripPanel.GiveFeedback),
                nameof(ToolStripPanel.ImeModeChanged),
                nameof(ToolStripPanel.KeyDown),
                nameof(ToolStripPanel.KeyPress),
                nameof(ToolStripPanel.KeyUp),
                nameof(ToolStripPanel.LocationChanged),
                nameof(ToolStripPanel.MarginChanged),
                nameof(ToolStripPanel.MouseCaptureChanged),
                nameof(ToolStripPanel.Move),
                nameof(ToolStripPanel.QueryAccessibilityHelp),
                nameof(ToolStripPanel.QueryContinueDrag),
                nameof(ToolStripPanel.RegionChanged),
                nameof(ToolStripPanel.Scroll),
                nameof(ToolStripPanel.Validated),
                nameof(ToolStripPanel.Validating)
            ];

            for (int i = 0; i < noBrowseEvents.Length; i++)
            {
                eventDescriptor = (EventDescriptor?)events[noBrowseEvents[i]];
                if (eventDescriptor is not null)
                {
                    events[noBrowseEvents[i]] = TypeDescriptor.CreateEvent(eventDescriptor.ComponentType, eventDescriptor, BrowsableAttribute.No);
                }
            }
        }
    }

    /// <summary>
    ///  Set some properties to non-browsable depending on the Parent. (StandAlone ToolStripPanel should support properties that are usually hidden when its a part of ToolStripContainer)
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);
        PropertyDescriptor? propertyDescriptor;

        if (_panel?.Parent is ToolStripContainer)
        {
            properties.Remove("Modifiers");
            properties.Remove("Locked");
            properties.Remove("GenerateMember");

            string[] noBrowseProps =
            [
                nameof(ToolStripPanel.Anchor),
                nameof(ToolStripPanel.AutoSize),
                nameof(ToolStripPanel.Dock),
                nameof(ToolStripPanel.DockPadding),
                nameof(ToolStripPanel.Height),
                nameof(ToolStripPanel.Location),
                nameof(ToolStripPanel.Name),
                nameof(ToolStripPanel.Orientation),
                nameof(ToolStripPanel.Renderer),
                nameof(ToolStripPanel.RowMargin),
                nameof(ToolStripPanel.Size),
                nameof(ToolStripPanel.Visible),
                nameof(ToolStripPanel.Width),
            ];

            for (int i = 0; i < noBrowseProps.Length; i++)
            {
                propertyDescriptor = (PropertyDescriptor?)properties[noBrowseProps[i]];
                if (propertyDescriptor is not null)
                {
                    properties[noBrowseProps[i]] = TypeDescriptor.CreateProperty(propertyDescriptor.ComponentType, propertyDescriptor, BrowsableAttribute.No, DesignerSerializationVisibilityAttribute.Hidden);
                }
            }
        }

        string[] shadowProps = ["Padding", "Visible"];
        Attribute[] empty = [];
        for (int i = 0; i < shadowProps.Length; i++)
        {
            propertyDescriptor = (PropertyDescriptor?)properties[shadowProps[i]];
            if (propertyDescriptor is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ToolStripPanelDesigner), propertyDescriptor, empty);
            }
        }
    }

    /// <summary>
    ///  Should Serialize Padding
    /// </summary>
    private bool ShouldSerializePadding()
    {
        Padding padding = (Padding)ShadowProperties[nameof(Padding)];
        return !padding.Equals(s_defaultPadding);
    }

    /// <summary>
    ///  Should serialize for visible property
    /// </summary>
    private bool ShouldSerializeVisible()
    {
        return !Visible;
    }
}
