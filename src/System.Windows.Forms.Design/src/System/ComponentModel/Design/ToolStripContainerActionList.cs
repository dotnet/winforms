// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  Describes the list of actions that can be performed for the ToolStripContainer control from the chrome panel.
/// </summary>
internal class ToolStripContainerActionList : DesignerActionList
{
    private readonly ToolStripContainer _toolStripContainer;
    private readonly IDesignerHost? _designerHost;
    private readonly IServiceProvider? _serviceProvider;

    /// <summary>
    ///  ToolStripContainer ActionList.
    /// </summary>
    public ToolStripContainerActionList(ToolStripContainer toolStripContainer) : base(toolStripContainer)
    {
        _toolStripContainer = toolStripContainer;
        _serviceProvider = _toolStripContainer.Site;
        _designerHost = _serviceProvider?.GetService<IDesignerHost>();
    }

    /// <summary>
    ///  Helper function to get the property on the component.
    /// </summary>
    private static object? GetProperty(Component component, string propertyName)
    {
        PropertyDescriptor? getProperty = TypeDescriptor.GetProperties(component)?[propertyName];
        return getProperty?.GetValue(component);
    }

    /// <summary>
    ///  Helper function to change the property on the component
    /// </summary>
    private void ChangeProperty(Component component, string propertyName, object value)
    {
        if (_designerHost is null)
        {
            return;
        }

        ToolStripPanel? panel = component as ToolStripPanel;
        ToolStripPanelDesigner? panelDesigner = _designerHost.GetDesigner(component) as ToolStripPanelDesigner;

        if (propertyName.Equals("Visible") && panel is not null)
        {
            foreach (Control control in panel.Controls)
            {
                PropertyDescriptor? visibleProperty = TypeDescriptor.GetProperties(control)["Visible"];
                visibleProperty?.SetValue(control, value);
            }

            if (!(bool)value)
            {
                if (panel is not null)
                {
                    panel.Padding = new Padding(0);
                }

                if (panelDesigner?.ToolStripPanelSelectorGlyph is not null)
                {
                    panelDesigner.ToolStripPanelSelectorGlyph.IsExpanded = false;
                }
            }
        }

        PropertyDescriptor? changingProperty = TypeDescriptor.GetProperties(component)[propertyName];
        changingProperty?.SetValue(component, value);

        // Reset the Glyphs.
        SelectionManager? selectionManager = _serviceProvider?.GetService<SelectionManager>();
        selectionManager?.Refresh();

        // Invalidate the Window
        panelDesigner?.InvalidateGlyph();
    }

    /// <summary>
    ///  Checks if the <see cref="ToolStripContainer" /> is dock filled.
    /// </summary>
    private bool IsDockFilled
    {
        get
        {
            PropertyDescriptor? dockProperty = TypeDescriptor.GetProperties(_toolStripContainer)["Dock"];
            return dockProperty is null || (DockStyle?)dockProperty.GetValue(_toolStripContainer) == DockStyle.Fill;
        }
    }

    /// <summary>
    ///  Checks if the ToolStripContainer is a child control of the designerHost's rootComponent
    /// </summary>
    private bool ProvideReparent
        => _designerHost?.RootComponent is Control root
            && _toolStripContainer.Parent == root
            && IsDockFilled
            && root.Controls.Count > 1;

    /// <summary>
    ///  Sets the Dock
    /// </summary>
    public void SetDockToForm()
    {
        if (_designerHost is null)
        {
            return;
        }

        // Change the Parent only if its not parented to the form.
        if (_designerHost.RootComponent is Control root && _toolStripContainer.Parent is null)
        {
            root.Controls.Add(_toolStripContainer);
        }

        // Set the dock prop to DockStyle.Fill
        if (!IsDockFilled)
        {
            PropertyDescriptor? dockProp = TypeDescriptor.GetProperties(_toolStripContainer)["Dock"];
            dockProp?.SetValue(_toolStripContainer, DockStyle.Fill);
        }
    }

    /// <summary>
    ///  Reparent the controls on the form.
    /// </summary>
    public void ReparentControls()
    {
        // Reparent the Controls only if the ToolStripContainer is a child of the RootComponent.
        if (_designerHost?.RootComponent is not Control root
            || _toolStripContainer.Parent != root
            || root.Controls.Count <= 1)
        {
            return;
        }

        Control newParent = _toolStripContainer.ContentPanel;
        PropertyDescriptor? autoScrollProp = TypeDescriptor.GetProperties(newParent)["AutoScroll"];
        autoScrollProp?.SetValue(newParent, true);

        // create a transaction so this happens as an atomic unit.
        DesignerTransaction? changeParent = _designerHost.CreateTransaction(string.Format(SR._0_reparent_controls_transaction, nameof(ToolStripContainer)));

        try
        {
            IComponentChangeService? componentChangeService = _serviceProvider?.GetService<IComponentChangeService>();
            Control[] childControls = new Control[root.Controls.Count];
            root.Controls.CopyTo(childControls, 0);

            foreach (Control control in childControls)
            {
                if (control == _toolStripContainer || control is MdiClient)
                {
                    continue;
                }

                // We should not reparent inherited Controls
                if (TypeDescriptor.GetAttributes(control)?[typeof(InheritanceAttribute)] is not InheritanceAttribute inheritanceAttribute
                    || inheritanceAttribute.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
                {
                    continue;
                }

                newParent = control is ToolStrip ? GetParent(control) : _toolStripContainer.ContentPanel;

                PropertyDescriptor? controlsProp = TypeDescriptor.GetProperties(newParent)["Controls"];
                Control? oldParent = control.Parent;

                if (oldParent is not null)
                {
                    componentChangeService?.OnComponentChanging(oldParent, controlsProp);

                    // Remove control from the old parent
                    oldParent.Controls.Remove(control);
                }

                componentChangeService?.OnComponentChanging(newParent, controlsProp);

                // Finally add & relocate the control with the new parent
                newParent.Controls.Add(control);

                // Fire our component changed events
                if (componentChangeService is not null && oldParent is not null)
                {
                    componentChangeService.OnComponentChanged(oldParent, controlsProp, oldValue: null, newValue: null);
                }

                // fire component changed on the newParent
                componentChangeService?.OnComponentChanged(newParent, controlsProp, oldValue: null, newValue: null);
            }
        }
        catch
        {
            if (changeParent is not null)
            {
                changeParent.Cancel();
                changeParent = null;
            }
        }
        finally
        {
            if (changeParent is not null)
            {
                changeParent.Commit();
                changeParent = null;
            }

            // Set the Selection on the new Parent, so that the selection is restored to the new item
            ISelectionService? selectionService = _serviceProvider?.GetService<ISelectionService>();
            selectionService?.SetSelectedComponents(new IComponent[] { newParent });
        }
    }

    private Control GetParent(Control control)
    {
        Control newParent = _toolStripContainer.ContentPanel;
        DockStyle dock = control.Dock;

        foreach (Control panel in _toolStripContainer.Controls)
        {
            if (panel is ToolStripPanel && panel.Dock == dock)
            {
                newParent = panel;
                break;
            }
        }

        return newParent;
    }

    /// <summary>
    ///  Visibility of TopToolStripPanel.
    /// </summary>
    public bool TopVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.TopToolStripPanelVisible)) ?? false);
        set
        {
            if (value != TopVisible)
            {
                ChangeProperty(_toolStripContainer, "TopToolStripPanelVisible", value);
            }
        }
    }

    /// <summary>
    ///  Visibility of BottomToolStripPanel.
    /// </summary>
    public bool BottomVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.BottomToolStripPanelVisible)) ?? false);
        set
        {
            if (value != BottomVisible)
            {
                ChangeProperty(_toolStripContainer, "BottomToolStripPanelVisible", value);
            }
        }
    }

    /// <summary>
    ///  Visibility of LeftToolStripPanel.
    /// </summary>
    public bool LeftVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.LeftToolStripPanelVisible)) ?? false);
        set
        {
            if (value != LeftVisible)
            {
                ChangeProperty(_toolStripContainer, "LeftToolStripPanelVisible", value);
            }
        }
    }

    /// <summary>
    ///  Visibility of RightToolStripPanel.
    /// </summary>
    public bool RightVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.RightToolStripPanelVisible)) ?? false);
        set
        {
            if (value != RightVisible)
            {
                ChangeProperty(_toolStripContainer, "RightToolStripPanelVisible", value);
            }
        }
    }

    /// <summary>
    ///  Returns the control's action list items.
    /// </summary>
    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items =
        [
            new DesignerActionHeaderItem(SR.ToolStripContainerActionList_Visible, SR.ToolStripContainerActionList_Show),
            new DesignerActionPropertyItem(nameof(TopVisible),
                                           SR.ToolStripContainerActionList_Top,
                                           SR.ToolStripContainerActionList_Show,
                                           SR.ToolStripContainerActionList_TopDesc),

            new DesignerActionPropertyItem(nameof(BottomVisible),
                                           SR.ToolStripContainerActionList_Bottom,
                                           SR.ToolStripContainerActionList_Show,
                                           SR.ToolStripContainerActionList_BottomDesc),

            new DesignerActionPropertyItem(nameof(LeftVisible),
                                           SR.ToolStripContainerActionList_Left,
                                           SR.ToolStripContainerActionList_Show,
                                           SR.ToolStripContainerActionList_LeftDesc),

            new DesignerActionPropertyItem(nameof(RightVisible),
                                           SR.ToolStripContainerActionList_Right,
                                           SR.ToolStripContainerActionList_Show,
                                           SR.ToolStripContainerActionList_RightDesc)
        ];

        if (!IsDockFilled)
        {
            string displayName = _designerHost?.RootComponent is UserControl
                ? SR.DesignerShortcutDockInUserControl
                : SR.DesignerShortcutDockInForm;

            items.Add(new DesignerActionMethodItem(
                this,
                nameof(SetDockToForm),
                displayName));
        }

        if (ProvideReparent)
        {
            items.Add(new DesignerActionMethodItem(this, nameof(ReparentControls), SR.DesignerShortcutReparentControls));
        }

        return items;
    }
}
