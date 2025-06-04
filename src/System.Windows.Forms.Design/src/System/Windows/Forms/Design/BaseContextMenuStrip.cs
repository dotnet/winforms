// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class is going to replace the shell contextMenu and uses the ContextMenuStrip.
///  The ContextMenuStrip contains groups and groupOrder which it uses to add items to itself.
///  ControlDesigners can add custom items to the contextMenu, using the new member to the
///  group and add the groupOrder to the ContextMenu.
/// </summary>
internal class BaseContextMenuStrip : GroupedContextMenuStrip
{
    private readonly IServiceProvider _serviceProvider;
    private ToolStripMenuItem? _selectionMenuItem;

    public BaseContextMenuStrip(IServiceProvider provider) : base()
    {
        _serviceProvider = provider;
        // Now initialize the contextMenu
        InitializeContextMenu();
    }

    /// <summary>
    ///  Helper function to add the "View Code" menuItem.
    /// </summary>
    private void AddCodeMenuItem()
    {
        StandardCommandToolStripMenuItem codeMenuItem = new(StandardCommands.ViewCode, SR.ContextMenuViewCode, "viewcode", _serviceProvider);
        Groups[StandardGroups.Code].Items.Add(codeMenuItem);
    }

    /// <summary>
    ///  Helper function to add the "SendToBack/BringToFront" menuItem.
    /// </summary>
    private void AddZorderMenuItem()
    {
        StandardCommandToolStripMenuItem ZOrderMenuItem = new(StandardCommands.BringToFront, SR.ContextMenuBringToFront, "bringToFront", _serviceProvider);
        Groups[StandardGroups.ZORder].Items.Add(ZOrderMenuItem);
        ZOrderMenuItem = new StandardCommandToolStripMenuItem(StandardCommands.SendToBack, SR.ContextMenuSendToBack, "sendToBack", _serviceProvider);
        Groups[StandardGroups.ZORder].Items.Add(ZOrderMenuItem);
    }

    /// <summary>
    ///  Helper function to add the "Alignment" menuItem.
    /// </summary>
    private void AddGridMenuItem()
    {
        StandardCommandToolStripMenuItem gridMenuItem = new(StandardCommands.AlignToGrid, SR.ContextMenuAlignToGrid, "alignToGrid", _serviceProvider);
        Groups[StandardGroups.Grid].Items.Add(gridMenuItem);
    }

    /// <summary>
    ///  Helper function to add the "Locked" menuItem.
    /// </summary>
    private void AddLockMenuItem()
    {
        StandardCommandToolStripMenuItem lockMenuItem = new(StandardCommands.LockControls, SR.ContextMenuLockControls, "lockControls", _serviceProvider);
        Groups[StandardGroups.Lock].Items.Add(lockMenuItem);
    }

    /// <summary>
    ///  Helper function to add the Select Parent menuItem.
    /// </summary>
    private void RefreshSelectionMenuItem()
    {
        int index = -1;
        if (_selectionMenuItem is not null)
        {
            index = Items.IndexOf(_selectionMenuItem);
            Groups[StandardGroups.Selection].Items.Remove(_selectionMenuItem);
            Items.Remove(_selectionMenuItem);
        }

        List<Component> parentControls = [];
        int nParentControls = 0;

        // Get the currently selected Control
        if (_serviceProvider.GetService(typeof(ISelectionService)) is ISelectionService selectionService && _serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
        {
            IComponent root = host.RootComponent;
            Debug.Assert(root is not null, "Null root component. Will be unable to build selection menu");
            if (selectionService.PrimarySelection is Control selectedControl && root is not null && selectedControl != root)
            {
                Control? parentControl = selectedControl.Parent;
                while (parentControl is not null)
                {
                    if (parentControl.Site is not null)
                    {
                        parentControls.Add(parentControl);
                        nParentControls++;
                    }

                    if (parentControl == root)
                    {
                        break;
                    }

                    parentControl = parentControl.Parent;
                }
            }
            else if (selectionService.PrimarySelection is ToolStripItem selectedItem)
            {
                if (host.GetDesigner(selectedItem) is ToolStripItemDesigner itemDesigner)
                {
                    parentControls = itemDesigner.AddParentTree();
                    nParentControls = parentControls.Count;
                }
            }
        }

        if (nParentControls > 0)
        {
            _selectionMenuItem = new ToolStripMenuItem();

            if (_serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
            {
                _selectionMenuItem.DropDown.Renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"]!;

                // Set the right Font
                _selectionMenuItem.DropDown.Font = (Font)uis.Styles["DialogFont"]!;

                if (uis.Styles["VsColorPanelText"] is Color color)
                {
                    _selectionMenuItem.DropDown.ForeColor = color;
                }
            }

            _selectionMenuItem.Text = SR.ContextMenuSelect;
            foreach (Component parent in parentControls)
            {
                ToolStripMenuItem selectListItem = new SelectToolStripMenuItem(parent, _serviceProvider);
                _selectionMenuItem.DropDownItems.Add(selectListItem);
            }

            Groups[StandardGroups.Selection].Items.Add(_selectionMenuItem);

            // Re-add the newly refreshed item.
            if (index != -1)
            {
                Items.Insert(index, _selectionMenuItem);
            }
        }
    }

    /// <summary>
    ///  Helper function to add the Verbs.
    /// </summary>
    private void AddVerbMenuItem()
    {
        // Add Designer Verbs..
        if (_serviceProvider.TryGetService(out IMenuCommandService? menuCommandService))
        {
            DesignerVerbCollection verbCollection = menuCommandService.Verbs;
            foreach (DesignerVerb verb in verbCollection)
            {
                DesignerVerbToolStripMenuItem verbItem = new(verb);
                Groups[StandardGroups.Verbs].Items.Add(verbItem);
            }
        }
    }

    /// <summary>
    ///  Helper function to add the "Cut/Copy/Paste/Delete" menuItem.
    /// </summary>
    private void AddEditMenuItem()
    {
        StandardCommandToolStripMenuItem stdMenuItem = new(StandardCommands.Cut, SR.ContextMenuCut, "cut", _serviceProvider);
        Groups[StandardGroups.Edit].Items.Add(stdMenuItem);
        stdMenuItem = new StandardCommandToolStripMenuItem(StandardCommands.Copy, SR.ContextMenuCopy, "copy", _serviceProvider);
        Groups[StandardGroups.Edit].Items.Add(stdMenuItem);
        stdMenuItem = new StandardCommandToolStripMenuItem(StandardCommands.Paste, SR.ContextMenuPaste, "paste", _serviceProvider);
        Groups[StandardGroups.Edit].Items.Add(stdMenuItem);
        stdMenuItem = new StandardCommandToolStripMenuItem(StandardCommands.Delete, SR.ContextMenuDelete, "delete", _serviceProvider);
        Groups[StandardGroups.Edit].Items.Add(stdMenuItem);
    }

    /// <summary>
    ///  Helper function to add the "Properties" menuItem.
    /// </summary>
    private void AddPropertiesMenuItem()
    {
        StandardCommandToolStripMenuItem stdMenuItem = new(StandardCommands.DocumentOutline, SR.ContextMenuDocumentOutline, "", _serviceProvider);
        Groups[StandardGroups.Properties].Items.Add(stdMenuItem);
        stdMenuItem = new StandardCommandToolStripMenuItem(MenuCommands.DesignerProperties, SR.ContextMenuProperties, "properties", _serviceProvider);
        Groups[StandardGroups.Properties].Items.Add(stdMenuItem);
    }

    /// <summary>
    ///  Basic Initialize method.
    /// </summary>
    private void InitializeContextMenu()
    {
        Name = "designerContextMenuStrip";

        if (_serviceProvider.TryGetService(out IUIService? uis))
        {
            Renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"]!;

            if (uis.Styles["VsColorPanelText"] is Color color)
            {
                ForeColor = color;
            }
        }

        GroupOrdering.AddRange([
            StandardGroups.Code,
            StandardGroups.ZORder,
            StandardGroups.Grid,
            StandardGroups.Lock,
            StandardGroups.Verbs,
            StandardGroups.Custom,
            StandardGroups.Selection,
            StandardGroups.Edit,
            StandardGroups.Properties]);

        // ADD MENUITEMS
        AddCodeMenuItem();
        AddZorderMenuItem();
        AddGridMenuItem();
        AddLockMenuItem();
        AddVerbMenuItem();
        RefreshSelectionMenuItem();
        AddEditMenuItem();
        AddPropertiesMenuItem();
    }

    /// <summary>
    ///  Public function that allows the individual MenuItems to get refreshed each time the ContextMenu is opened.
    /// </summary>
    public override void RefreshItems()
    {
        if (_serviceProvider.TryGetService(out IUIService? uis))
        {
            Font = (Font)uis.Styles["DialogFont"]!;
        }

        foreach (ToolStripItem item in Items)
        {
            if (item is StandardCommandToolStripMenuItem stdItem)
            {
                stdItem.RefreshItem();
            }
        }

        RefreshSelectionMenuItem();
    }

    /// <summary>
    ///  A ToolStripMenuItem that gets added for the "Select" menuitem.
    /// </summary>
    private class SelectToolStripMenuItem : ToolStripMenuItem
    {
        private readonly Component? _comp;
        private readonly IServiceProvider _serviceProvider;
        private readonly Type _itemType;
        private bool _cachedImage;
        private Image? _image;
        private static readonly string s_systemWindowsFormsNamespace = typeof(ToolStripItem).Namespace!;

        public SelectToolStripMenuItem(Component c, IServiceProvider provider)
        {
            _comp = c;
            _serviceProvider = provider;
            // Get NestedSiteName...
            string? compName = null;
            if (_comp is not null && _comp.Site is { } site)
            {
                if (site is INestedSite nestedSite && !string.IsNullOrEmpty(nestedSite.FullName))
                {
                    compName = nestedSite.FullName;
                }
                else if (!string.IsNullOrEmpty(site.Name))
                {
                    compName = site.Name;
                }
            }

            Text = string.Format(SR.ToolStripSelectMenuItem, compName);
            _itemType = c.GetType();
        }

        public override Image? Image
        {
            get
            {
                // Defer loading the image until we're sure we need it
                if (!_cachedImage)
                {
                    _cachedImage = true;
                    // else attempt to get the resource from a known place in the manifest.
                    // if and only if the namespace of the type is System.Windows.Forms.
                    // else attempt to get the resource from a known place in the manifest
                    if (_itemType.Namespace == s_systemWindowsFormsNamespace)
                    {
                        _image = ToolboxBitmapAttribute.GetImageFromResource(_itemType, imageName: null, large: false);
                    }

                    // if all else fails, throw up a default image.
                    _image ??= ToolboxBitmapAttribute.GetImageFromResource(_comp!.GetType(), imageName: null, large: false);
                }

                return _image;
            }
            set
            {
                _image = value;
                _cachedImage = true;
            }
        }

        /// <summary>
        ///  Items OnClick event, to select the Parent Control.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            if (_comp is not null && _serviceProvider.TryGetService(out ISelectionService? selectionService))
            {
                selectionService.SetSelectedComponents(new object[] { _comp }, SelectionTypes.Replace);
            }
        }
    }
}
