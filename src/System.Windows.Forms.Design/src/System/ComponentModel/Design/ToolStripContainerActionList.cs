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
    private readonly IDesignerHost _host;
    private readonly IServiceProvider _serviceProvider;

    public ToolStripContainerActionList(ToolStripContainer toolStripContainer)
        : base(toolStripContainer)
    {
        _toolStripContainer = toolStripContainer;
        _serviceProvider = toolStripContainer.Site ?? throw new ArgumentException(nameof(toolStripContainer));
        _host = _serviceProvider.GetRequiredService<IDesignerHost>();
    }

    private static object? GetProperty(Component component, string propertyName)
    {
        PropertyDescriptor? getProperty = TypeDescriptor.GetProperties(component)[propertyName];

        if (getProperty is not null)
        {
            return getProperty.GetValue(component);
        }

        return null;
    }

    private void ChangeProperty(Component component, string propertyName, object value)
    {
        if (_host is not null)
        {
            ToolStripPanel? panel = component as ToolStripPanel;
            ToolStripPanelDesigner? panelDesigner = _host.GetDesigner(component) as ToolStripPanelDesigner;

            if (propertyName.Equals("Visible") && panel is not null)
            {
                foreach (Control control in panel.Controls)
                {
                    PropertyDescriptor? visibleProperty = TypeDescriptor.GetProperties(control)["Visible"];
                    visibleProperty?.SetValue(control, value);
                }

                if (!((bool)value))
                {
                    if (panel is not null)
                    {
                        panel.Padding = new Padding(0);
                    }

                    if (panelDesigner is not null)
                    {
                        if (panelDesigner.ToolStripPanelSelectorGlyph is not null)
                        {
                            panelDesigner.ToolStripPanelSelectorGlyph.IsExpanded = false;
                        }
                    }
                }
            }

            PropertyDescriptor? changingProperty = TypeDescriptor.GetProperties(component)[propertyName];
            changingProperty?.SetValue(component, value);

            //Reset the Glyphs.
            SelectionManager? selectionManager = (SelectionManager?)_serviceProvider.GetService(typeof(SelectionManager));
            selectionManager?.Refresh();

            // Invalidate the Window...
            panelDesigner?.InvalidateGlyph();
        }
    }

    /// <summary>
    /// Checks if the <see cref="ToolStripContainer" /> is dock filled.
    /// </summary>
    private bool IsDockFilled
    {
        get
        {
            PropertyDescriptor? property = TypeDescriptor.GetProperties(_toolStripContainer)["Dock"];
            return property is null || (DockStyle?)property.GetValue(_toolStripContainer) == DockStyle.Fill;
        }
    }

    /// <summary>
    /// Checks if the <see cref="ToolStripContainer" /> is a child control of 
    /// the <see cref="IDesignerHost.RootComponent" /> before re-parenting.
    /// </summary>
    private bool ProvideReparent
        => _host.RootComponent is Control root
            && _toolStripContainer.Parent == root
            && IsDockFilled
            && root.Controls.Count > 1;

    public void SetDockToForm()
    {
        if (_host is not null)
        {
            //change the Parent only if its not parented to the form.
            if (_host.RootComponent is Control control && _toolStripContainer.Parent is not Control)
            {
                control.Controls.Add(_toolStripContainer);
            }

            //set the dock prop to DockStyle.Fill
            if (!IsDockFilled)
            {
                PropertyDescriptor? dockProp = TypeDescriptor.GetProperties(_toolStripContainer)["Dock"];
                dockProp?.SetValue(_toolStripContainer, DockStyle.Fill);
            }
        }
    }

    /// <summary>
    /// Reparent child controls of the form to this container.
    /// </summary>
    public void ReparentControls()
    {
        // Reparent the Controls only if the ToolStripContainer is a child of the RootComponent.
        if (_host.RootComponent is not Control root
            || _toolStripContainer.Parent != root
            || root.Controls.Count <= 1)
        {
            return;
        }

        Control newParent = _toolStripContainer.ContentPanel;
        PropertyDescriptor? autoScrollProperty = TypeDescriptor.GetProperties(newParent)["AutoScroll"];
        autoScrollProperty?.SetValue(newParent, true);

        // Create a transaction so this happens as an atomic unit.
        using DesignerTransaction transaction =
            _host.CreateTransaction(string.Format(
                SR._0_reparent_controls_transaction,
                nameof(ToolStripContainer)));

        var childControls = new Control[root.Controls.Count];
        root.Controls.CopyTo(childControls, 0);

        var changeService = _serviceProvider.GetRequiredService<IComponentChangeService>();
        bool changed = false;

        foreach (Control control in childControls)
        {
            if (control == _toolStripContainer || control is MdiClient)
            {
                continue;
            }

            // We should not reparent inherited Controls.
            AttributeCollection attributes = TypeDescriptor.GetAttributes(control);
            var inheritanceAttribute = attributes[typeof(InheritanceAttribute)];
            if (inheritanceAttribute is null
                || inheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly))
            {
                continue;
            }

            newParent = control is ToolStrip
                ? GetParent(control)
                : _toolStripContainer.ContentPanel;

            PropertyDescriptor? controlsProp = TypeDescriptor.GetProperties(newParent)["Controls"];
            Control? oldParent = control.Parent;

            if (oldParent is not null)
            {
                changeService.OnComponentChanging(oldParent, controlsProp);
                // Remove control from the old parent.
                oldParent.Controls.Remove(control);
            }

            changeService.OnComponentChanging(newParent, controlsProp);

            // Finally add and relocate the control with the new parent.
            newParent.Controls.Add(control);

            if (oldParent is not null)
            {
                changeService.OnComponentChanged(oldParent, controlsProp, oldValue: null, newValue: null);
            }

            changeService.OnComponentChanged(newParent, controlsProp, oldValue: null, newValue: null);
            changed = true;
        }

        transaction.Commit();

        var selectionService = _serviceProvider.GetRequiredService<ISelectionService>();
        if (changed)
        {
            // Designer action panel will get updated correctly only if its related component is selected,
            // thus select the entire container.
            selectionService.SetSelectedComponents(new IComponent[] { _toolStripContainer }, SelectionTypes.Replace);

            DesignerActionUIService? actionUIService = (DesignerActionUIService?)Component?.Site?.GetService(typeof(DesignerActionUIService));
            actionUIService?.Refresh(_toolStripContainer);
        }
        else
        {
            selectionService.SetSelectedComponents(new IComponent[] { newParent }, SelectionTypes.Replace);
        }
    }

    private Control GetParent(Control control)
    {
        Control newParent = _toolStripContainer.ContentPanel;
        DockStyle dock = control.Dock;

        foreach (Control? panel in _toolStripContainer.Controls)
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
    /// Visibility of the top ToolStripPanel.
    /// </summary>
    public bool TopVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.TopToolStripPanelVisible)) ?? false);
        set
        {
            if (value != TopVisible)
            {
                ChangeProperty(_toolStripContainer, nameof(ToolStripContainer.TopToolStripPanelVisible), value);
            }
        }
    }

    /// <summary>
    /// Visibility of the bottom ToolStripPanel.
    /// </summary>
    public bool BottomVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.BottomToolStripPanelVisible)) ?? false);
        set
        {
            if (value != TopVisible)
            {
                ChangeProperty(_toolStripContainer, nameof(ToolStripContainer.BottomToolStripPanelVisible), value);
            }
        }
    }

    /// <summary>
    /// Visibility of left ToolStripPanel.
    /// </summary>
    public bool LeftVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.LeftToolStripPanelVisible)) ?? false);
        set
        {
            if (value != TopVisible)
            {
                ChangeProperty(_toolStripContainer, nameof(ToolStripContainer.LeftToolStripPanelVisible), value);
            }
        }
    }

    /// <summary>
    /// Visibility of right ToolStripPanel.
    /// </summary>
    public bool RightVisible
    {
        get => (bool)(GetProperty(_toolStripContainer, nameof(ToolStripContainer.RightToolStripPanelVisible)) ?? false);
        set
        {
            if (value != TopVisible)
            {
                ChangeProperty(_toolStripContainer, nameof(ToolStripContainer.RightToolStripPanelVisible), value);
            }
        }
    }

    /// <summary>
    ///  Returns the control's action list items.
    /// </summary>
    /// <returns></returns>
    public override DesignerActionItemCollection GetSortedActionItems()
    {
        var items = new DesignerActionItemCollection
        {
            new DesignerActionHeaderItem(
                SR.ToolStripContainerActionList_Visible,
                SR.ToolStripContainerActionList_Show),

            new DesignerActionPropertyItem(
                nameof(TopVisible),
                SR.ToolStripContainerActionList_Top,
                SR.ToolStripContainerActionList_Show,
                SR.ToolStripContainerActionList_TopDesc),

            new DesignerActionPropertyItem(
                nameof(BottomVisible),
                SR.ToolStripContainerActionList_Bottom,
                SR.ToolStripContainerActionList_Show,
                SR.ToolStripContainerActionList_BottomDesc),

            new DesignerActionPropertyItem(
                nameof(LeftVisible),
                SR.ToolStripContainerActionList_Left,
                SR.ToolStripContainerActionList_Show,
                SR.ToolStripContainerActionList_LeftDesc),

            new DesignerActionPropertyItem(
                nameof(RightVisible),
                SR.ToolStripContainerActionList_Right,
                SR.ToolStripContainerActionList_Show,
                SR.ToolStripContainerActionList_RightDesc)
        };

        if (!IsDockFilled)
        {
            var displayName = _host.RootComponent is UserControl
                ? SR.DesignerShortcutDockInUserControl
                : SR.DesignerShortcutDockInForm;

            items.Add(new DesignerActionMethodItem(
                this,
                nameof(SetDockToForm),
                displayName));
        }

        if (ProvideReparent)
        {
            items.Add(new DesignerActionMethodItem(
                this,
                nameof(ReparentControls),
                SR.DesignerShortcutReparentControls));
        }

        return items;
    }
}
