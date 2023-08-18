// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms.Design;

internal class BindingNavigatorDesigner : ToolStripDesigner
{
    private static string[] itemNames = new string[]
    {
        "MovePreviousItem", "MoveFirstItem", "MoveNextItem", "MoveLastItem", "AddNewItem",
                                            "DeleteItem", "PositionItem", "CountItem"
    };

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (componentChangeSvc is not null)
        {
            componentChangeSvc.ComponentRemoved += new ComponentEventHandler(ComponentChangeSvc_ComponentRemoved);
            componentChangeSvc.ComponentChanged += new ComponentChangedEventHandler(ComponentChangeSvc_ComponentChanged);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (componentChangeSvc is not null)
            {
                componentChangeSvc.ComponentRemoved -= new ComponentEventHandler(ComponentChangeSvc_ComponentRemoved);
                componentChangeSvc.ComponentChanged -= new ComponentChangedEventHandler(ComponentChangeSvc_ComponentChanged);
            }
        }

        base.Dispose(disposing);
    }

    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        BindingNavigator dn = (BindingNavigator)Component;
        IDesignerHost? host = Component?.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;

        try
        {
            s_autoAddNewItems = false;    // Temporarily suppress "new items go to the selected strip" behavior
            dn.SuspendLayout();          // Turn off layout while adding items
            dn.AddStandardItems();       // Let the control add its standard items (user overridable)
            SiteItems(host: host, dn.Items);   // Recursively site and name all the items on the strip
            RaiseItemsChanged();         // Make designer Undo engine aware of the newly added and sited items
            dn.ResumeLayout();           // Allow strip to lay out now
            dn.ShowItemToolTips = true;  // Non-default property setting for ShowToolTips
        }
        finally
        {
            s_autoAddNewItems = true;
        }
    }

    private void RaiseItemsChanged()
    {
        BindingNavigator dn = (BindingNavigator)Component;
        IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));

        if (componentChangeSvc is not null)
        {
            MemberDescriptor? itemsProp = TypeDescriptor.GetProperties(dn)["Items"];
            componentChangeSvc.OnComponentChanging(dn, itemsProp);
            componentChangeSvc.OnComponentChanged(dn, itemsProp, null, null);

            foreach (string itemName in itemNames)
            {
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(dn)[itemName];

                if (prop is not null)
                {
                    componentChangeSvc.OnComponentChanging(dn, prop);
                    componentChangeSvc.OnComponentChanged(dn, prop, null, null);
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
            SiteItems(host, dropDownItem.DropDownItems);
        }
    }

    private void SiteItems(IDesignerHost? host, ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            SiteItem(host, item);
        }
    }

    private void ComponentChangeSvc_ComponentRemoved(object? sender, ComponentEventArgs e)
    {
        ToolStripItem? item = e.Component as ToolStripItem;

        if (item is not null)
        {
            BindingNavigator dn = (BindingNavigator)Component;

            if (item == dn.MoveFirstItem)
            {
                dn.MoveFirstItem = null;
            }
            else if (item == dn.MovePreviousItem)
            {
                dn.MovePreviousItem = null;
            }
            else if (item == dn.MoveNextItem)
            {
                dn.MoveNextItem = null;
            }
            else if (item == dn.MoveLastItem)
            {
                dn.MoveLastItem = null;
            }
            else if (item == dn.PositionItem)
            {
                dn.PositionItem = null;
            }
            else if (item == dn.CountItem)
            {
                dn.CountItem = null;
            }
            else if (item == dn.AddNewItem)
            {
                dn.AddNewItem = null;
            }
            else if (item == dn.DeleteItem)
            {
                dn.DeleteItem = null;
            }
        }
    }

    private void ComponentChangeSvc_ComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        BindingNavigator dn = (BindingNavigator)Component;

        if (e.Component is not null && e.Component == dn.CountItem && e.Member is not null && e.Member.Name == "Text")
        {
            dn.CountItemFormat = dn.CountItem.Text ?? string.Empty;
        }
    }
}
