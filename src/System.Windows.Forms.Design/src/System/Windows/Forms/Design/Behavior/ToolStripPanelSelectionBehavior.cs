// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Behavior;

internal sealed class ToolStripPanelSelectionBehavior : Behavior
{
    private readonly ToolStripPanel _relatedControl;
    private readonly IServiceProvider _serviceProvider;
    private readonly BehaviorService _behaviorService;

    private const int DefaultBounds = 25;

    internal ToolStripPanelSelectionBehavior(ToolStripPanel containerControl, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _behaviorService = serviceProvider.GetRequiredService<BehaviorService>();
        _relatedControl = containerControl;
    }

    private static bool DragComponentContainsToolStrip(DropSourceBehavior.BehaviorDataObject? data)
    {
        if (data is null)
        {
            return false;
        }

        foreach (var component in data.DragComponents)
        {
            if (component is ToolStrip)
            {
                return true;
            }
        }

        return false;
    }

    private void ExpandPanel(bool setSelection)
    {
        // Change the padding to "dynamically" increase the bounds.
        switch (_relatedControl.Dock)
        {
            case DockStyle.Top:
                _relatedControl.Padding = new Padding(0, 0, 0, DefaultBounds);
                break;
            case DockStyle.Left:
                _relatedControl.Padding = new Padding(0, 0, DefaultBounds, 0);
                break;
            case DockStyle.Right:
                _relatedControl.Padding = new Padding(DefaultBounds, 0, 0, 0);
                break;
            case DockStyle.Bottom:
                _relatedControl.Padding = new Padding(0, DefaultBounds, 0, 0);
                break;
        }

        if (setSelection)
        {
            ISelectionService selection = _serviceProvider.GetRequiredService<ISelectionService>();
            selection?.SetSelectedComponents(new object[] { _relatedControl }, SelectionTypes.Replace);
        }
    }

    /// <summary>
    ///  Simply clear the initial drag point, so we can start again
    ///  on the next mouse down.
    /// </summary>
    public override bool OnMouseDown(Glyph? glyph, MouseButtons button, Point screenCoordinates)
    {
        if (button != MouseButtons.Left || !(glyph is ToolStripPanelSelectionGlyph selectionGlyph))
        {
            return false;
        }

        if (!selectionGlyph.IsExpanded)
        {
            ExpandPanel(true);

            Rectangle oldBounds = selectionGlyph.Bounds;
            selectionGlyph.IsExpanded = true;
            _behaviorService.Invalidate(oldBounds);
            _behaviorService.Invalidate(selectionGlyph.Bounds);
        }
        else
        {
            // Change the padding to "dynamically" increase the bounds.
            _relatedControl.Padding = new Padding(0);

            Rectangle oldBounds = selectionGlyph.Bounds;
            selectionGlyph.IsExpanded = false;
            _behaviorService.Invalidate(oldBounds);
            _behaviorService.Invalidate(selectionGlyph.Bounds);

            // Select our parent.
            ISelectionService selectionService = _serviceProvider.GetRequiredService<ISelectionService>();
            Component? currentSelection = selectionService.PrimarySelection as Component;

            if (_relatedControl.Parent is not null)
            {
                if (currentSelection != _relatedControl.Parent)
                {
                    selectionService?.SetSelectedComponents(new object[] { _relatedControl.Parent }, SelectionTypes.Replace);
                }
                else
                {
                    Control parent = _relatedControl.Parent;
                    parent.PerformLayout();

                    var selectionManager = _serviceProvider.GetRequiredService<SelectionManager>();
                    selectionManager.Refresh();

                    Point loc = _behaviorService.ControlToAdornerWindow(parent);
                    Rectangle translatedBounds = new(loc, parent.Size);
                    _behaviorService.Invalidate(translatedBounds);
                }
            }
        }

        return false;
    }

    private void ReParentControls(List<IComponent> controls, bool copy)
    {
        if (controls.Count <= 0)
        {
            return;
        }

        // Create a transaction so this happens as an atomic unit.
        var host = _serviceProvider.GetRequiredService<IDesignerHost>();
        using DesignerTransaction transaction = host.CreateTransaction(GetTransactionDescription());

        List<IComponent>? temp = copy ? [] : null;
        ISelectionService selectionService = _serviceProvider.GetRequiredService<ISelectionService>();
        IComponentChangeService changeService = _serviceProvider.GetRequiredService<IComponentChangeService>();

        for (int i = 0; i < controls.Count; i++)
        {
            if (controls[0] is not ToolStrip control)
            {
                continue;
            }

            if (copy)
            {
                temp!.Clear();
                temp.Add(control);

                temp = DesignerUtils.CopyDragObjects(temp, _serviceProvider);
                if (temp is not null)
                {
                    control = (ToolStrip)temp[0];
                    control.Visible = true;
                }
            }

            Control newParent = _relatedControl;
            PropertyDescriptor? controlsProp = TypeDescriptor.GetProperties(newParent)["Controls"];
            Control? oldParent = control.Parent;
            if (oldParent is not null && !copy)
            {
                changeService.OnComponentChanging(oldParent, controlsProp);
                oldParent.Controls.Remove(control);
            }

            changeService.OnComponentChanging(newParent, controlsProp);

            // Finally add & relocate the control with the new parent.
            newParent.Controls.Add(control);

            if (oldParent is not null && !copy)
            {
                changeService.OnComponentChanged(oldParent, controlsProp, oldValue: null, newValue: null);
            }

            changeService.OnComponentChanged(newParent, controlsProp, oldValue: null, newValue: null);

            selectionService.SetSelectedComponents(
                (Collections.ICollection)control,
                i == 0 ? SelectionTypes.Primary | SelectionTypes.Replace : SelectionTypes.Add);
        }

        transaction.Commit();

        string GetTransactionDescription()
        {
            var control = controls[0];

            if (controls.Count == 1 && control is ToolStrip)
            {
                string? name = TypeDescriptor.GetComponentName(control);
                if (string.IsNullOrEmpty(name))
                {
                    name = control.GetType().Name;
                }

                return string.Format(
                    copy ? SR.BehaviorServiceCopyControl : SR.BehaviorServiceMoveControl,
                    name);
            }

            return string.Format(
                copy ? SR.BehaviorServiceCopyControls : SR.BehaviorServiceMoveControls,
                controls.Count);
        }
    }

    /// <summary>
    ///  Simply clear the initial drag point, so we can start again
    ///  on the next mouse down.
    /// </summary>
    public override void OnDragDrop(Glyph? glyph, DragEventArgs e)
    {
        // Expand the glyph only if ToolStrip is dragged around
        bool expandPanel = false;
        List<IComponent>? components = null;

        if (e.Data is DropSourceBehavior.BehaviorDataObject data)
        {
            components = [..data.DragComponents];

            foreach (IComponent dragComponent in components)
            {
                if (dragComponent is ToolStrip tool && tool.Parent != _relatedControl)
                {
                    expandPanel = true;
                    break;
                }
            }

            if (expandPanel && _relatedControl.Parent is { } root)
            {
                try
                {
                    root.SuspendLayout();
                    ExpandPanel(false);

                    if (glyph is ToolStripPanelSelectionGlyph selectionGlyph)
                    {
                        Rectangle oldBounds = selectionGlyph.Bounds;
                        selectionGlyph.IsExpanded = true;
                        _behaviorService.Invalidate(oldBounds);
                        _behaviorService.Invalidate(selectionGlyph.Bounds);
                    }

                    ReParentControls(components, e.Effect == DragDropEffects.Copy);
                }
                finally
                {
                    root.ResumeLayout(true);
                }
            }

            data.CleanupDrag();
        }
        else if (e.Data is DataObject && components is null)
        {
            IToolboxService toolboxService = _serviceProvider.GetRequiredService<IToolboxService>();
            IDesignerHost host = _serviceProvider.GetRequiredService<IDesignerHost>();

            if (toolboxService is not null && host is not null)
            {
                ToolboxItem item = toolboxService.DeserializeToolboxItem(e.Data, host);
                if (item.GetType(host) == typeof(ToolStrip)
                    || item.GetType(host) == typeof(MenuStrip)
                    || item.GetType(host) == typeof(StatusStrip))
                {
                    ToolStripPanelDesigner? panelDesigner =
                        host.GetDesigner(_relatedControl) is ToolStripPanelDesigner toolStripPanelDesigner
                           ? toolStripPanelDesigner
                           : null;

                    if (panelDesigner is not null)
                    {
                        OleDragDropHandler oleDragDropHandler = panelDesigner.GetOleDragHandler();
                        oleDragDropHandler?.CreateTool(item, _relatedControl, 0, 0, 0, 0, false, false);
                    }
                }
            }
        }
    }

    public override void OnDragEnter(Glyph? glyph, DragEventArgs e)
    {
        if (e.Data is not null)
        {
            e.Effect = GetEffect(e.Data);
        }

        base.OnDragEnter(glyph, e);
    }

    public override void OnDragOver(Glyph? glyph, DragEventArgs e)
    {
        if (e.Data is not null)
        {
            e.Effect = GetEffect(e.Data);
        }

        base.OnDragOver(glyph, e);
    }

    private static DragDropEffects GetEffect(IDataObject data)
            => DragComponentContainsToolStrip(data as DropSourceBehavior.BehaviorDataObject)
                ? Control.ModifierKeys == Keys.Control
                    ? DragDropEffects.Copy
                    : DragDropEffects.Move
                : DragDropEffects.None;
}
