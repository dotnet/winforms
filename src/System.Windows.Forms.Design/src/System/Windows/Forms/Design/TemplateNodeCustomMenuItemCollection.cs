// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  Custom ContextMenu section for ToolStripMenuItems.
/// </summary>
internal class TemplateNodeCustomMenuItemCollection : CustomMenuItemCollection
{
    private readonly ToolStripItem _currentItem;
    private readonly IServiceProvider _serviceProvider;
    private ToolStripMenuItem _insertToolStripMenuItem;

    public TemplateNodeCustomMenuItemCollection(IServiceProvider provider, Component currentItem) : base()
    {
        _serviceProvider = provider;
        _currentItem = currentItem as ToolStripItem;
        PopulateList();
    }

    /// <summary>
    ///  Immediate parent - can be ToolStrip if the Item is on the toplevel
    /// </summary>
    private ToolStrip ParentTool
    {
        get => _currentItem.Owner;
    }

    private void PopulateList()
    {
        _insertToolStripMenuItem = new ToolStripMenuItem
        {
            Text = SR.ToolStripItemContextMenuInsert,
            DropDown = ToolStripDesignerUtils.GetNewItemDropDown(ParentTool, _currentItem, new EventHandler(AddNewItemClick), false, _serviceProvider, true)
        };
        Add(_insertToolStripMenuItem);
    }

    private void AddNewItemClick(object sender, EventArgs e)
    {
        ItemTypeToolStripMenuItem senderItem = (ItemTypeToolStripMenuItem)sender;
        Type t = senderItem.ItemType;
        // we are inserting a new item..
        InsertItem(t);
    }

    private void InsertItem(Type t)
    {
        InsertToolStripItem(t);
    }

    /// <summary>
    ///  Insert Item into ToolStrip.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    private void InsertToolStripItem(Type t)
    {
        IDesignerHost designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Why didn't we get a designer host?");
        ToolStrip parent = ParentTool;
        int dummyIndex = parent.Items.IndexOf(_currentItem);
        DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
        try
        {
            // turn off Adding/Added events listened to by the ToolStripDesigner...
            ToolStripDesigner.s_autoAddNewItems = false;
            // the code in ComponentAdded will actually get the add done.
            IComponent component = designerHost.CreateComponent(t);
            IDesigner designer = designerHost.GetDesigner(component);
            if (designer is ComponentDesigner componentDesigner)
            {
                componentDesigner.InitializeNewComponent(null);
            }

            // Set the Image property and DisplayStyle...
            if (component is ToolStripButton or ToolStripSplitButton or ToolStripDropDownButton)
            {
                Image image = null;
                try
                {
                    image = new Icon(typeof(ToolStripButton), "blank").ToBitmap();
                }
                catch (Exception e) when (!e.IsCriticalException())
                {
                }

                PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(component)["Image"];
                Debug.Assert(imageProperty is not null, "Could not find 'Image' property in ToolStripItem.");
                if (imageProperty is not null && image is not null)
                {
                    imageProperty.SetValue(component, image);
                }

                PropertyDescriptor dispProperty = TypeDescriptor.GetProperties(component)["DisplayStyle"];
                Debug.Assert(dispProperty is not null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                dispProperty?.SetValue(component, ToolStripItemDisplayStyle.Image);

                PropertyDescriptor imageTransProperty = TypeDescriptor.GetProperties(component)["ImageTransparentColor"];
                Debug.Assert(imageTransProperty is not null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                imageTransProperty?.SetValue(component, Color.Magenta);
            }

            Debug.Assert(dummyIndex != -1, "Why is the index of the Item negative?");
            parent.Items.Insert(dummyIndex, (ToolStripItem)component);
            // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionService from new Component
            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
        }
        catch (Exception ex)
        {
            if (newItemTransaction is not null)
            {
                newItemTransaction.Cancel();
                newItemTransaction = null;
            }

            if (ex.IsCriticalException())
            {
                throw;
            }
        }
        finally
        {
            newItemTransaction?.Commit();

            // turn off Adding/Added events listened to by the ToolStripDesigner...
            ToolStripDesigner.s_autoAddNewItems = true;
            // Add the glyphs if the parent is DropDown.
            if (parent is ToolStripDropDown parentDropDown && parentDropDown.Visible)
            {
                if (parentDropDown.OwnerItem is ToolStripDropDownItem ownerItem)
                {
                    if (designerHost.GetDesigner(ownerItem) is ToolStripMenuItemDesigner itemDesigner)
                    {
                        itemDesigner.ResetGlyphs(ownerItem);
                    }
                }
            }
        }
    }
}
