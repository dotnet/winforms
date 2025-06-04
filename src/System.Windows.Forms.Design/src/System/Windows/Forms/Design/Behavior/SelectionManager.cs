// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The SelectionBehavior is pushed onto the BehaviorStack in response to a positively hit tested SelectionGlyph.
///  The SelectionBehavior performs two main tasks:
///  1) forward messages to the related ControlDesigner
///  2) calls upon the SelectionManager to push a potential DragBehavior.
/// </summary>
internal sealed class SelectionManager : IDisposable
{
    // ptr back to our BehaviorService
    private BehaviorService _behaviorService;
    private IServiceProvider _serviceProvider;
    // used for quick look up of designers related to components
    private readonly Dictionary<IComponent, ControlDesigner> _componentToDesigner;
    private readonly Control _rootComponent;
    private ISelectionService _selectionService;
    private IDesignerHost _designerHost;
    // used to only repaint the changing part of the selection
    private Rectangle[]? _previousSelectionBounds;
    // used to check if the primary selection changed
    private object? _previousPrimarySelection;
    private Rectangle[]? _currentSelectionBounds;
    private int _curCompIndex;
    // the "container" for all things related to the designer action (SmartTags) UI
    // we don't want the OnSelectionChanged to be recursively called.
    private DesignerActionUI? _designerActionUI;
    private bool _selectionChanging;

    /// <summary>
    ///  Here we query for necessary services and cache them for performance reasons.
    ///  We also hook to <see cref="Component" /> Added/Removed/Changed notifications so we can keep in sync when the designers'
    ///  components change. Also, we create our custom <see cref="Adorner" /> and add it to the <see cref="BehaviorService" />.
    /// </summary>
    public SelectionManager(IServiceProvider serviceProvider, BehaviorService behaviorService)
    {
        _previousSelectionBounds = null;
        _previousPrimarySelection = null;
        _behaviorService = behaviorService;
        _serviceProvider = serviceProvider;

        _selectionService = serviceProvider.GetRequiredService<ISelectionService>();
        _designerHost = serviceProvider.GetRequiredService<IDesignerHost>();

        // sync the BehaviorService's BeginDrag event
        behaviorService.BeginDrag += OnBeginDrag;

        // sync the BehaviorService's Synchronize event
        behaviorService.Synchronize += OnSynchronize;

        _selectionService.SelectionChanged += OnSelectionChanged;
        _rootComponent = (Control)_designerHost.RootComponent;

        // create and add both of our adorners,
        // one for selection, one for bodies
        SelectionGlyphAdorner = new Adorner();
        BodyGlyphAdorner = new Adorner();
        behaviorService.Adorners.Add(BodyGlyphAdorner);
        // Adding this will cause the adorner to get set up with the BehaviorService.
        behaviorService.Adorners.Add(SelectionGlyphAdorner);

        _componentToDesigner = [];

        if (_serviceProvider.TryGetService(out IComponentChangeService? cs))
        {
            cs.ComponentAdded += OnComponentAdded;
            cs.ComponentRemoved += OnComponentRemoved;
            cs.ComponentChanged += OnComponentChanged;
        }

        _designerHost.TransactionClosed += OnTransactionClosed;

        // DesignerActionUI
        DesignerOptionService? options = _designerHost.GetService<DesignerOptionService>();
        PropertyDescriptor? p = options?.Options.Properties["UseSmartTags"];
        if (p is not null && p.TryGetValue(component: null, out bool b) && b)
        {
            _designerActionUI = new DesignerActionUI(serviceProvider, SelectionGlyphAdorner);
            behaviorService.DesignerActionUI = _designerActionUI;
        }
    }

    /// <summary>
    ///  Returns the Adorner that contains all the BodyGlyphs for the current selection state.
    /// </summary>
    internal Adorner BodyGlyphAdorner { get; private set; }

    /// <summary>
    ///  There are certain cases like Adding Item to ToolStrips through InSitu Editor, where there is
    ///  ParentTransaction that has to be cancelled depending upon the user action When this parent transaction is
    ///  cancelled, there may be no reason to REFRESH the selectionManager which actually clears all the glyphs and
    ///  readds them This REFRESH causes a lot of flicker and can be avoided by setting this property to false.
    ///  Since this property is checked in the TransactionClosed, the SelectionManager won't REFRESH and hence
    ///  just eat up the refresh thus avoiding unnecessary flicker.
    /// </summary>
    internal bool NeedRefresh { get; set; }

    /// <summary>
    ///  Returns the Adorner that contains all the selection glyphs for the current selection state.
    /// </summary>
    internal Adorner SelectionGlyphAdorner { get; private set; }

    /// <summary>
    ///  This method fist calls the recursive AddControlGlyphs() method. When finished, we add the final glyph(s)
    ///  to the root comp.
    /// </summary>
    private void AddAllControlGlyphs(Control parent, List<IComponent> selComps, object? primarySelection)
    {
        foreach (Control control in parent.Controls)
        {
            AddAllControlGlyphs(control, selComps, primarySelection);
        }

        GlyphSelectionType selType = GlyphSelectionType.NotSelected;
        if (selComps.Contains(parent))
        {
            selType = parent.Equals(primarySelection)
                ? GlyphSelectionType.SelectedPrimary
                : GlyphSelectionType.Selected;
        }

        AddControlGlyphs(parent, selType);
    }

    /// <summary>
    ///  Recursive method that goes through and adds all the glyphs of every child to our global Adorner.
    /// </summary>
    private void AddControlGlyphs(Control control, GlyphSelectionType selType)
    {
        Debug.Assert(_currentSelectionBounds is not null);
        bool hasSelection = selType is GlyphSelectionType.SelectedPrimary or GlyphSelectionType.Selected;

        if (_componentToDesigner.TryGetValue(control, out ControlDesigner? controlDesigner))
        {
            ControlBodyGlyph bodyGlyph = controlDesigner.GetControlGlyphInternal(selType);
            if (bodyGlyph is not null)
            {
                BodyGlyphAdorner.Glyphs.Add(bodyGlyph);
                if (hasSelection)
                {
                    ref Rectangle currentSelectionBounds = ref _currentSelectionBounds[_curCompIndex];
                    currentSelectionBounds = currentSelectionBounds == Rectangle.Empty
                        ? bodyGlyph.Bounds
                        : Rectangle.Union(currentSelectionBounds, bodyGlyph.Bounds);
                }
            }

            GlyphCollection glyphs = controlDesigner.GetGlyphs(selType);
            if (glyphs is not null)
            {
                SelectionGlyphAdorner.Glyphs.AddRange(glyphs);
                if (hasSelection)
                {
                    foreach (Glyph glyph in glyphs)
                    {
                        _currentSelectionBounds[_curCompIndex] = Rectangle.Union(_currentSelectionBounds[_curCompIndex], glyph.Bounds);
                    }
                }
            }
        }

        if (hasSelection)
        {
            _curCompIndex++;
        }
    }

    /// <summary>
    ///  Unhook all of our event notifications, clear our adorner and remove it from the Beh.Svc.
    /// </summary>
    // We don't need to Dispose rootComponent.
    public void Dispose()
    {
        if (_designerHost is not null)
        {
            _designerHost.TransactionClosed -= OnTransactionClosed;
            _designerHost = null!;
        }

        if (_serviceProvider is not null)
        {
            if (_serviceProvider.TryGetService(out IComponentChangeService? cs))
            {
                cs.ComponentAdded -= OnComponentAdded;
                cs.ComponentChanged -= OnComponentChanged;
                cs.ComponentRemoved -= OnComponentRemoved;
            }

            if (_selectionService is not null)
            {
                _selectionService.SelectionChanged -= OnSelectionChanged;
                _selectionService = null!;
            }

            _serviceProvider = null!;
        }

        if (_behaviorService is not null)
        {
            _behaviorService.Adorners.Remove(BodyGlyphAdorner);
            _behaviorService.Adorners.Remove(SelectionGlyphAdorner);
            _behaviorService.BeginDrag -= OnBeginDrag;
            _behaviorService.Synchronize -= OnSynchronize;
            _behaviorService = null!;
        }

        if (SelectionGlyphAdorner is not null)
        {
            SelectionGlyphAdorner.Glyphs.Clear();
            SelectionGlyphAdorner = null!;
        }

        if (BodyGlyphAdorner is not null)
        {
            BodyGlyphAdorner.Glyphs.Clear();
            BodyGlyphAdorner = null!;
        }

        if (_designerActionUI is not null)
        {
            _designerActionUI.Dispose();
            _designerActionUI = null;
        }
    }

    /// <summary>
    ///  Refreshes all selection Glyphs.
    /// </summary>
    public void Refresh()
    {
        NeedRefresh = false;
        OnSelectionChanged(sender: this, e: null);
    }

    /// <summary>
    ///  When a component is added, we get the designer and add it to our hash table for quick lookup.
    /// </summary>
    private void OnComponentAdded(object? source, ComponentEventArgs ce)
    {
        IComponent component = ce.Component!;
        IDesigner? designer = _designerHost.GetDesigner(component);
        if (designer is ControlDesigner controlDesigner)
        {
            _componentToDesigner.Add(component, controlDesigner);
        }
    }

    /// <summary>
    ///  Before a drag, remove all glyphs that are involved in the drag operation and any that don't allow drops.
    /// </summary>
    private void OnBeginDrag(object? source, BehaviorDragDropEventArgs e)
    {
        List<IComponent> dragComps = [..e.DragComponents.Cast<IComponent>()];
        List<Glyph> glyphsToRemove = [];
        foreach (ControlBodyGlyph g in BodyGlyphAdorner.Glyphs)
        {
            if (g.RelatedComponent is Control control && (dragComps.Contains(g.RelatedComponent) || !control.AllowDrop))
            {
                glyphsToRemove.Add(g);
            }
        }

        foreach (Glyph g in glyphsToRemove)
        {
            BodyGlyphAdorner.Glyphs.Remove(g);
        }
    }

    // Called by the DropSourceBehavior when dragging into a new host
    internal void OnBeginDrag(BehaviorDragDropEventArgs e)
    {
        OnBeginDrag(source: null, e);
    }

    /// <summary>
    ///  When a component is changed - we need to refresh the selection.
    /// </summary>
    private void OnComponentChanged(object? source, ComponentChangedEventArgs ce)
    {
        if (_selectionService.GetComponentSelected(ce.Component!))
        {
            if (!_designerHost.InTransaction)
            {
                Refresh();
            }
            else
            {
                NeedRefresh = true;
            }
        }
    }

    /// <summary>
    ///  When a component is removed - we remove the key and value from our hash table.
    /// </summary>
    private void OnComponentRemoved(object? source, ComponentEventArgs ce)
    {
        _componentToDesigner.Remove(ce.Component!);

        // remove the associated DesignerActionPanel
        _designerActionUI?.RemoveActionGlyph(ce.Component);
    }

    /// <summary>
    ///  Computes the region representing the difference between the old selection and the new selection.
    /// </summary>
    private Region DetermineRegionToRefresh(object? primarySelection, Rectangle[] previousSelectionBounds, Rectangle[] currentSelectionBounds)
    {
        Region toRefresh = new(Rectangle.Empty);
        Rectangle[] larger;
        Rectangle[] smaller;
        if (currentSelectionBounds.Length >= previousSelectionBounds.Length)
        {
            larger = currentSelectionBounds;
            smaller = previousSelectionBounds;
        }
        else
        {
            larger = previousSelectionBounds;
            smaller = currentSelectionBounds;
        }

        // we need to make sure all of the rectangles in the smaller array are
        // accounted for. Any that don't intersect a rectangle in the larger
        // array need to be included in the region to repaint.
        bool[] intersected = new bool[smaller.Length];
        for (int i = 0; i < smaller.Length; i++)
        {
            intersected[i] = false;
        }

        // determine which rectangles in the larger array need to be
        // included in the region to invalidate by intersecting
        // with rectangles in the smaller array.
        foreach (Rectangle large in larger)
        {
            bool largeIntersected = false;
            for (int s = 0; s < smaller.Length; s++)
            {
                if (large.IntersectsWith(smaller[s]))
                {
                    Rectangle small = smaller[s];
                    largeIntersected = true;
                    if (large != small)
                    {
                        toRefresh.Union(large);
                        toRefresh.Union(small);
                    }

                    intersected[s] = true;
                    break;
                }
            }

            if (!largeIntersected)
            {
                toRefresh.Union(large);
            }
        }

        // now add any rectangles from the smaller array that weren't accounted for
        for (int k = 0; k < intersected.Length; k++)
        {
            if (!intersected[k])
            {
                toRefresh.Union(smaller[k]);
            }
        }

        using Graphics g = _behaviorService.AdornerWindowGraphics;

        // If all that changed was the primary selection, then the refresh region was empty,
        // but we do need to update the 2 controls.
        if (toRefresh.IsEmpty(g) && primarySelection is not null && !primarySelection.Equals(_previousPrimarySelection))
        {
            for (int i = 0; i < currentSelectionBounds.Length; i++)
            {
                toRefresh.Union(currentSelectionBounds[i]);
            }
        }

        return toRefresh;
    }

    /// <summary>
    ///  Event handler for the behaviorService's Synchronize event
    /// </summary>
    private void OnSynchronize(object? sender, EventArgs e)
    {
        Refresh();
    }

    /// <summary>
    ///  On every SelectionChanged Event, we remove all glyphs, get the newly selected components,
    ///  and re-add all glyphs back to the Adorner.
    /// </summary>
    private void OnSelectionChanged(object? sender, EventArgs? e)
    {
        // Note: selectionChanging would guard against a re-entrant code...
        // Since we don't want to be in messed up state when adding new Glyphs.
        if (!_selectionChanging)
        {
            _selectionChanging = true;

            SelectionGlyphAdorner.Glyphs.Clear();
            BodyGlyphAdorner.Glyphs.Clear();

            List<IComponent> selComps = [.._selectionService.GetSelectedComponents().Cast<IComponent>()];
            object? primarySelection = _selectionService.PrimarySelection;

            // add all control glyphs to all controls on rootComp
            _curCompIndex = 0;
            _currentSelectionBounds = new Rectangle[selComps.Count];
            AddAllControlGlyphs(_rootComponent, selComps, primarySelection);

            if (_previousSelectionBounds is not null)
            {
                Region toUpdate = DetermineRegionToRefresh(primarySelection, _previousSelectionBounds, _currentSelectionBounds);
                using Graphics g = _behaviorService.AdornerWindowGraphics;
                if (!toUpdate.IsEmpty(g))
                {
                    SelectionGlyphAdorner.Invalidate(toUpdate);
                }
            }
            else
            {
                // There was no previous selection, so just invalidate
                // the current selection
                if (_currentSelectionBounds.Length > 0)
                {
                    Rectangle toUpdate = _currentSelectionBounds[0];
                    for (int i = 1; i < _currentSelectionBounds.Length; i++)
                    {
                        toUpdate = Rectangle.Union(toUpdate, _currentSelectionBounds[i]);
                    }

                    if (toUpdate != Rectangle.Empty)
                    {
                        SelectionGlyphAdorner.Invalidate(toUpdate);
                    }
                }
                else
                {
                    SelectionGlyphAdorner.Invalidate();
                }
            }

            _previousPrimarySelection = primarySelection;
            _previousSelectionBounds = _currentSelectionBounds.Length > 0 ? [.. _currentSelectionBounds] : null;

            _selectionChanging = false;
        }
    }

    /// <summary>
    ///  When a transaction that involves one of our components closes, refresh to reflect any changes.
    /// </summary>
    private void OnTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (e.LastTransaction && NeedRefresh)
        {
            Refresh();
        }
    }
}
