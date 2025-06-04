// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles all design time behavior for the SplitContainer class. This
///  draws a visible border on the splitter if it doesn't have a border so the
///  user knows where the boundaries of the splitter lie.
/// </summary>
internal partial class SplitContainerDesigner : ParentControlDesigner
{
    private const string Panel1Name = "Panel1";
    private const string Panel2Name = "Panel2";
    private IDesignerHost? _designerHost;
    private SplitContainer? _splitContainer;
    private SplitterPanel? _selectedPanel;
    private const int NumberOfSplitterPanels = 2;
    private SplitterPanel? _splitterPanel1, _splitterPanel2;

    // The Container Should not Show any GRIDs in the Splitter Region.
    private bool _disableDrawGrid;

    private bool _disabledGlyphs;
    private bool _splitContainerSelected;
    private int _initialSplitterDistance;
    private bool _splitterDistanceException;

    /// <summary>
    ///  Gets the design-time supported actions on the control.
    /// </summary>
    public override DesignerActionListCollection ActionLists
    {
        get
        {
            DesignerActionListCollection designerActionListCollection = new();
            if (HasComponent)
            {
                designerActionListCollection.Add(new OrientationActionList(this));
            }

            return designerActionListCollection;
        }
    }

    /// <summary>
    ///  The <see cref="SplitContainerDesigner"/> will not re-parent any controls
    ///  that are within it's lasso at creation time.
    /// </summary>
    protected override bool AllowControlLasso => false;

    protected override bool DrawGrid => !_disableDrawGrid && base.DrawGrid;

    /// <summary>
    ///  This property is used by deriving classes to determine if it returns the control being designed or
    ///  some other Container while adding a component to it.
    ///  e.g: When SplitContainer is selected and a component is being added,
    ///  the SplitContainer designer would return a SelectedPanel as the ParentControl
    ///  for all the items being added rather than itself.
    /// </summary>
    protected override Control? GetParentForComponent(IComponent component) => _splitterPanel1;

    /// <summary>
    ///  Returns a list of SnapLine objects representing interesting alignment points for this control.
    ///  These SnapLines are used to assist in the positioning of the control on a parent's surface.
    /// </summary>
    public override IList SnapLines =>
        // We don't want padding snaplines, so call directly to the internal method.
        EdgeAndMarginSnapLines().Unwrap();

    /// <summary>
    ///  Returns the number of internal control designers in the <see cref="SplitContainerDesigner"/>.
    ///  An internal control is a control that is not in the IDesignerHost.Container.Components collection.
    ///  We use this to get SnapLines for the internal control designers.
    /// </summary>
    public override int NumberOfInternalControlDesigners() => NumberOfSplitterPanels;

    /// <summary>
    ///  Returns the internal control designer with the specified index in the <see cref="ControlDesigner"/>.
    ///  InternalControlIndex is zero-based.
    /// </summary>
    public override ControlDesigner? InternalControlDesigner(int internalControlIndex)
    {
        SplitterPanel panel;

        switch (internalControlIndex)
        {
            case 0:
                panel = _splitterPanel1!;
                break;
            case 1:
                panel = _splitterPanel2!;
                break;
            default:
                return null;
        }

        return _designerHost?.GetDesigner(panel) as ControlDesigner;
    }

    /// <summary>
    ///  This is the internal Property which stores the currently selected panel.
    ///  If the user double clicks a controls it is placed in the SelectedPanel.
    /// </summary>
    internal SplitterPanel? Selected
    {
        get => _selectedPanel;
        set
        {
            if (_selectedPanel is not null)
            {
                if (_designerHost!.GetDesigner(_selectedPanel) is SplitterPanelDesigner panelDesigner1)
                {
                    panelDesigner1.Selected = false;
                }

                _selectedPanel = null;
            }

            if (value is not null)
            {
                _selectedPanel = value;
                if (_designerHost!.GetDesigner(value) is SplitterPanelDesigner panelDesigner)
                {
                    panelDesigner.Selected = true;
                }
            }
        }
    }

    /// <summary>
    ///  The ToolStripItems are the associated components. We want those to come with in any cut, copy operations.
    /// </summary>
    public override ICollection AssociatedComponents
    {
        get
        {
            List<Control> components = [];
            foreach (SplitterPanel panel in _splitContainer!.Controls)
            {
                components.AddRange(panel.Controls.Cast<Control>());
            }

            return components;
        }
    }

    protected override void OnDragEnter(DragEventArgs de) => de.Effect = DragDropEffects.None;

    /// <summary>
    ///  This is the worker method of all CreateTool methods. It is the only one that can be overridden.
    /// </summary>
    protected override IComponent[]? CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
    {
        // We invoke the drag drop handler for this. This implementation is shared between all designers that create components.
        Selected ??= _splitterPanel1!;
        var selectedPanelDesigner = (SplitterPanelDesigner)_designerHost!.GetDesigner(Selected)!;
        InvokeCreateTool(selectedPanelDesigner, tool);

        // Return Dummy null as the InvokeCreateTool of SplitterPanel would do the necessary hookups.
        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (TryGetService(out ISelectionService? svc))
        {
            svc.SelectionChanged -= OnSelectionChanged;
        }

        if (_splitContainer is not null)
        {
            _splitContainer.MouseDown -= OnSplitContainer;
            _splitContainer.SplitterMoved -= OnSplitterMoved;
            _splitContainer.SplitterMoving -= OnSplitterMoving;
            _splitContainer.DoubleClick -= OnSplitContainerDoubleClick;
        }

        base.Dispose(disposing);
    }

    protected override bool GetHitTest(Point point) => !(InheritanceAttribute == InheritanceAttribute.InheritedReadOnly) && _splitContainerSelected;

    /// <summary>
    ///  Returns a 'BodyGlyph' representing the bounds of this control.
    ///  The BodyGlyph is responsible for hit testing the related CtrlDes and
    ///  forwarding messages directly to the designer.
    /// </summary>
    protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
    {
        if (TryGetService(out SelectionManager? selMgr))
        {
            Rectangle translatedBounds = BehaviorService?.ControlRectInAdornerWindow(_splitterPanel1!) ?? Rectangle.Empty;
            var panelDesigner = _designerHost?.GetDesigner(_splitterPanel1!) as SplitterPanelDesigner;
            OnSetCursor();

            if (panelDesigner is not null)
            {
                // Create our glyph, and set its cursor appropriately.
                ControlBodyGlyph bodyGlyph = new(translatedBounds, Cursor.Current, _splitterPanel1, panelDesigner);
                selMgr.BodyGlyphAdorner.Glyphs.Add(bodyGlyph);
            }

            translatedBounds = BehaviorService?.ControlRectInAdornerWindow(_splitterPanel2!) ?? Rectangle.Empty;
            panelDesigner = _designerHost?.GetDesigner(_splitterPanel2!) as SplitterPanelDesigner;

            if (panelDesigner is not null)
            {
                // Create our glyph, and set its cursor appropriately.
                ControlBodyGlyph bodyGlyph = new(translatedBounds, Cursor.Current, _splitterPanel2, panelDesigner);
                selMgr.BodyGlyphAdorner.Glyphs.Add(bodyGlyph);
            }
        }

        return base.GetControlGlyph(selectionType);
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        AutoResizeHandles = true;

        _splitContainer = component as SplitContainer;
        Debug.Assert(_splitContainer is not null, $"Component must be a non-null SplitContainer, it is a: {component.GetType().FullName}");
        _splitterPanel1 = _splitContainer.Panel1;
        _splitterPanel2 = _splitContainer.Panel2;

        EnableDesignMode(_splitContainer.Panel1, Panel1Name);
        EnableDesignMode(_splitContainer.Panel2, Panel2Name);

        _designerHost = (IDesignerHost?)component.Site?.GetService(typeof(IDesignerHost));
        if (_selectedPanel is null)
        {
            Selected = _splitterPanel1;
        }

        _splitContainer.MouseDown += OnSplitContainer;
        _splitContainer.SplitterMoved += OnSplitterMoved;
        _splitContainer.SplitterMoving += OnSplitterMoving;
        _splitContainer.DoubleClick += OnSplitContainerDoubleClick;

        if (TryGetService(out ISelectionService? svc))
        {
            svc.SelectionChanged += OnSelectionChanged;
        }
    }

    /// <summary>
    ///  Overrides our base class. We don't draw the Grids for this Control. Also we Select the Panel1 if nothing is still Selected.
    /// </summary>
    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        try
        {
            _disableDrawGrid = true;

            // We don't want to do this for the tab control designer because you can't drag anything onto it anyway.
            // So we will always return false for draw grid.
            base.OnPaintAdornments(pe);
        }
        finally
        {
            _disableDrawGrid = false;
        }
    }

    /// <summary>
    ///  Determines if the this designer can parent to the specified designer.
    ///  Generally this means if the control for this designer can parent the given ControlDesigner's designer.
    /// </summary>
    public override bool CanParent(Control control) => false;

    private void OnSplitContainer(object? sender, MouseEventArgs e)
    {
        var svc = GetRequiredService<ISelectionService>();
        svc.SetSelectedComponents(new object[] { Control });
    }

    private void OnSplitContainerDoubleClick(object? sender, EventArgs e)
    {
        if (_splitContainerSelected)
        {
            try
            {
                DoDefaultAction();
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                DisplayError(ex);
            }
        }
    }

    private void OnSplitterMoved(object? sender, SplitterEventArgs e)
    {
        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly || _splitterDistanceException)
        {
            return;
        }

        try
        {
            RaiseComponentChanging(TypeDescriptor.GetProperties(_splitContainer!)["SplitterDistance"]);
            RaiseComponentChanged(TypeDescriptor.GetProperties(_splitContainer!)["SplitterDistance"], oldValue: null, newValue: null);

            // Enable all adorners except for BodyGlyph adorner. But only if we turned off the adorners.
            if (_disabledGlyphs)
            {
                BehaviorService?.EnableAllAdorners(true);

                var selMgr = GetService<SelectionManager>();
                selMgr?.Refresh();
                _disabledGlyphs = false;
            }
        }
        catch (InvalidOperationException ex)
        {
            var uiService = (IUIService?)Component?.Site?.GetService(typeof(IUIService));
            uiService?.ShowError(ex.Message);
        }
        catch (CheckoutException checkoutException) when (checkoutException == CheckoutException.Canceled)
        {
            try
            {
                _splitterDistanceException = true;
                _splitContainer!.SplitterDistance = _initialSplitterDistance;
            }
            finally
            {
                _splitterDistanceException = false;
            }
        }
    }

    private void OnSplitterMoving(object? sender, SplitterCancelEventArgs e)
    {
        _initialSplitterDistance = _splitContainer!.SplitterDistance;

        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            return;
        }

        // We are moving the splitter via the mouse or key and not as a result of resize of the container itself
        // (through the ResizeBehavior::OnMouseMove).
        _disabledGlyphs = true;

        // Find our BodyGlyph adorner offered by the behavior service. We don't want to disable
        // the transparent body glyphs.
        SelectionManager? selMgr = GetService<SelectionManager>();
        Adorner? bodyGlyphAdorner = selMgr?.BodyGlyphAdorner;

        // Disable all adorners except for BodyGlyph adorner.
        if (BehaviorService is not null)
        {
            foreach (Adorner adorner in BehaviorService.Adorners)
            {
                if (bodyGlyphAdorner is not null && adorner.Equals(bodyGlyphAdorner))
                {
                    continue;
                }

                adorner.EnabledInternal = false;
            }

            BehaviorService.Invalidate();
        }

        // From the BodyAdorners Remove all Glyphs Except the ones for SplitterPanels.
        List<ControlBodyGlyph> glyphsToRemove = [];
        foreach (ControlBodyGlyph g in bodyGlyphAdorner!.Glyphs)
        {
            if (g.RelatedComponent is not SplitterPanel)
            {
                glyphsToRemove.Add(g);
            }
        }

        foreach (Glyph g in glyphsToRemove)
        {
            bodyGlyphAdorner.Glyphs.Remove(g);
        }
    }

    /// <summary>
    ///  Called when the current selection changes. Here we check to see if the newly selected component
    ///  is one of our Panels. If it is, we make sure that the tab is the currently visible tab.
    /// </summary>
    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        _splitContainerSelected = false;

        if (!TryGetService(out ISelectionService? svc))
        {
            return;
        }

        ICollection selComponents = svc.GetSelectedComponents();
        foreach (object comp in selComponents)
        {
            SplitterPanel? panel = CheckIfPanelSelected(comp);
            if (panel is not null && panel.Parent == _splitContainer)
            {
                _splitContainerSelected = false;
                Selected = panel;
                return;
            }

            if (comp == _splitContainer)
            {
                _splitContainerSelected = true; // This is for HitTest purposes
                break;
            }
        }

        Selected = null;
    }

    /// <summary>
    ///  Given a component, this retrieves the splitter panel that it's parented to,
    ///  or null if it's not parented to any splitter panel.
    /// </summary>
    private static SplitterPanel? CheckIfPanelSelected(object comp) => comp as SplitterPanel;

    /// <summary>
    ///  Called when one of the child splitter panels receives a MouseHover message.
    ///  Here, we will simply call the parenting SplitContainer.OnMouseHover so we can get a
    ///  grab handle for moving this thing around.
    /// </summary>
    internal void SplitterPanelHover() => OnMouseHover();
}
