// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms.Design;

internal class BindingNavigatorDesigner : ToolStripDesigner
{
    private static readonly string[] s_itemNames =
    [
        "MovePreviousItem",
        "MoveFirstItem",
        "MoveNextItem",
        "MoveLastItem",
        "AddNewItem",
        "DeleteItem",
        "PositionItem",
        "CountItem"
    ];

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        if (TryGetService(out IComponentChangeService? componentChangeService))
        {
            componentChangeService.ComponentRemoved += ComponentChangeService_ComponentRemoved;
            componentChangeService.ComponentChanged += ComponentChangeService_ComponentChanged;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (TryGetService(out IComponentChangeService? componentChangeService))
            {
                componentChangeService.ComponentRemoved -= ComponentChangeService_ComponentRemoved;
                componentChangeService.ComponentChanged -= ComponentChangeService_ComponentChanged;
            }
        }

        base.Dispose(disposing);
    }

    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        BindingNavigator navigator = (BindingNavigator)Component;
        IDesignerHost? host = Component?.Site?.GetService<IDesignerHost>();

        try
        {
            s_autoAddNewItems = false;    // Temporarily suppress "new items go to the selected strip" behavior
            navigator.SuspendLayout();          // Turn off layout while adding items
            navigator.AddStandardItems();       // Let the control add its standard items (user overridable)
            SiteItems(host: host, items: navigator.Items);   // Recursively site and name all the items on the strip
            RaiseItemsChanged();         // Make designer Undo engine aware of the newly added and sited items
            navigator.ResumeLayout();           // Allow strip to lay out now
            navigator.ShowItemToolTips = true;  // Non-default property setting for ShowToolTips
        }
        finally
        {
            s_autoAddNewItems = true;
        }
    }

    private void RaiseItemsChanged()
    {
        BindingNavigator navigator = (BindingNavigator)Component;

        if (TryGetService(out IComponentChangeService? componentChangeService))
        {
            MemberDescriptor? memberDescriptor = TypeDescriptor.GetProperties(navigator)["Items"];
            componentChangeService.OnComponentChanging(component: navigator, member: memberDescriptor);
            componentChangeService.OnComponentChanged(component: navigator, member: memberDescriptor, oldValue: null, newValue: null);

            foreach (string itemName in s_itemNames)
            {
                PropertyDescriptor? propertyDescriptor = TypeDescriptor.GetProperties(navigator)[itemName];

                if (propertyDescriptor is not null)
                {
                    componentChangeService.OnComponentChanging(component: navigator, member: propertyDescriptor);
                    componentChangeService.OnComponentChanged(component: navigator, member: propertyDescriptor, oldValue: null, newValue: null);
                }
            }
        }
    }

    private void SiteItem(IDesignerHost? host, ToolStripItem item)
    {
        // Skip any controls added for design-time use only
        if (item is DesignerToolStripControlHost)
        {
            return;
        }

        // Site the item in the container, giving it a unique site name based on its initial Name property
        host?.Container.Add(item, DesignerUtils.GetUniqueSiteName(host, item.Name));

        // Update the item's Name property to reflect the unique site name that it was actually given
        item.Name = item?.Site?.Name;

        // Site any sub-items of this item
        ToolStripDropDownItem? dropDownItem = item as ToolStripDropDownItem;
        if (dropDownItem is not null && dropDownItem.HasDropDownItems)
        {
            SiteItems(host: host, items: dropDownItem.DropDownItems);
        }
    }

    private void SiteItems(IDesignerHost? host, ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            SiteItem(host, item);
        }
    }

    private void ComponentChangeService_ComponentRemoved(object? sender, ComponentEventArgs e)
    {
        ToolStripItem? item = e.Component as ToolStripItem;

        if (item is not null)
        {
            BindingNavigator navigator = (BindingNavigator)Component;

            if (item == navigator.MoveFirstItem)
            {
                navigator.MoveFirstItem = null;
            }
            else if (item == navigator.MovePreviousItem)
            {
                navigator.MovePreviousItem = null;
            }
            else if (item == navigator.MoveNextItem)
            {
                navigator.MoveNextItem = null;
            }
            else if (item == navigator.MoveLastItem)
            {
                navigator.MoveLastItem = null;
            }
            else if (item == navigator.PositionItem)
            {
                navigator.PositionItem = null;
            }
            else if (item == navigator.CountItem)
            {
                navigator.CountItem = null;
            }
            else if (item == navigator.AddNewItem)
            {
                navigator.AddNewItem = null;
            }
            else if (item == navigator.DeleteItem)
            {
                navigator.DeleteItem = null;
            }
        }
    }

    private void ComponentChangeService_ComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        BindingNavigator navigator = (BindingNavigator)Component;

        if (e.Component is not null && e.Component == navigator.CountItem && e.Member is not null && e.Member.Name == "Text")
        {
            navigator.CountItemFormat = navigator.CountItem.Text ?? string.Empty;
        }
    }
}
