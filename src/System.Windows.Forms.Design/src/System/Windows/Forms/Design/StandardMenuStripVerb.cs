// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  Internal class to provide 'Insert Standard Items" verb for ToolStrips and MenuStrips.
/// </summary>
internal class StandardMenuStripVerb
{
    private readonly ToolStripDesigner _designer;
    private readonly IDesignerHost _host;
    private readonly IComponentChangeService _changeService;
    private readonly IServiceProvider _provider;

    /// <summary>
    ///  Create one of these things...
    /// </summary>
    internal StandardMenuStripVerb(ToolStripDesigner designer)
    {
        Debug.Assert(designer is not null, "Can't have a StandardMenuStripVerb without an associated designer");
        _designer = designer;
        _provider = designer.Component.Site;
        _host = (IDesignerHost)_provider.GetService(typeof(IDesignerHost));
        _changeService = (IComponentChangeService)_provider.GetService(typeof(IComponentChangeService));
    }

    /// <summary>
    ///  When the verb is invoked, use all the stuff above to show the dialog, etc.
    /// </summary>
    public void InsertItems()
    {
        DesignerActionUIService actionUIService = (DesignerActionUIService)_host.GetService(typeof(DesignerActionUIService));
        actionUIService?.HideUI(_designer.Component);

        Cursor current = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            if (_designer.Component is MenuStrip menuStrip)
            {
                CreateStandardMenuStrip(_host, menuStrip);
            }
            else
            {
                CreateStandardToolStrip(_host, (ToolStrip)_designer.Component);
            }
        }
        finally
        {
            Cursor.Current = current;
        }
    }

    /// <summary>
    ///  Here is where all the fun stuff starts. We create the structure and apply the naming here.
    /// </summary>
    private void CreateStandardMenuStrip(IDesignerHost host, MenuStrip tool)
    {
        // build the static menu items structure.
        string[][] menuItemNames =
        [
            [SR.StandardMenuFile, SR.StandardMenuNew, SR.StandardMenuOpen, "-", SR.StandardMenuSave, SR.StandardMenuSaveAs, "-", SR.StandardMenuPrint, SR.StandardMenuPrintPreview, "-", SR.StandardMenuExit],
            [SR.StandardMenuEdit, SR.StandardMenuUndo, SR.StandardMenuRedo, "-", SR.StandardMenuCut, SR.StandardMenuCopy, SR.StandardMenuPaste, "-", SR.StandardMenuSelectAll],
            [SR.StandardMenuTools, SR.StandardMenuCustomize, SR.StandardMenuOptions],
            [SR.StandardMenuHelp, SR.StandardMenuContents, SR.StandardMenuIndex, SR.StandardMenuSearch, "-", SR.StandardMenuAbout]
        ];

        // build the static menu items image list that maps one-one with above menuItems structure.
        // This is required so that the in LOCALIZED build we don't use the Localized item string.
        string[][] menuItemImageNames =
        [
            ["", "new", "open", "-", "save", "", "-", "print", "printPreview", "-", ""],
            ["", "", "", "-", "cut", "copy", "paste", "-", ""],
            ["", "", ""],
            ["", "", "", "", "-", ""]
        ];

        Keys[][] menuItemShortcuts =
        [
            [/*File*/Keys.None, /*New*/Keys.Control | Keys.N, /*Open*/Keys.Control | Keys.O, /*Separator*/ Keys.None, /*Save*/ Keys.Control | Keys.S, /*SaveAs*/Keys.None, Keys.None, /*Print*/ Keys.Control | Keys.P, /*PrintPreview*/ Keys.None, /*Separator*/Keys.None, /*Exit*/ Keys.None],
            [/*Edit*/Keys.None, /*Undo*/Keys.Control | Keys.Z, /*Redo*/Keys.Control | Keys.Y, /*Separator*/Keys.None, /*Cut*/ Keys.Control | Keys.X, /*Copy*/ Keys.Control | Keys.C, /*Paste*/Keys.Control | Keys.V, /*Separator*/ Keys.None, /*SelectAll*/Keys.None],
            [/*Tools*/Keys.None, /*Customize*/Keys.None, /*Options*/Keys.None],
            [/*Help*/Keys.None, /*Contents*/Keys.None, /*Index*/Keys.None, /*Search*/Keys.None, /*Separator*/Keys.None, /*About*/Keys.None]
        ];

        Debug.Assert(host is not null, "can't create standard menu without designer _host.");
        if (host is null)
        {
            return;
        }

        tool.SuspendLayout();
        ToolStripDesigner.s_autoAddNewItems = false;
        // create a transaction so this happens as an atomic unit.
        DesignerTransaction createMenu = _host.CreateTransaction(SR.StandardMenuCreateDesc);
        try
        {
            INameCreationService nameCreationService = (INameCreationService)_provider.GetService(typeof(INameCreationService));
            string defaultName = "standardMainMenuStrip";
            string name = defaultName;
            int index = 1;

            if (host is not null)
            {
                while (_host.Container.Components[name] is not null)
                {
                    name = $"{defaultName}{index++}";
                }
            }

            // now build the menu items themselves.
            for (int j = 0; j < menuItemNames.Length; j++)
            {
                string[] menuArray = menuItemNames[j];
                ToolStripMenuItem rootItem = null;
                for (int i = 0; i < menuArray.Length; i++)
                {
                    name = null;
                    // for separators, just use the default name. Otherwise,
                    // remove any non-characters and get the name from the text.
                    string itemText = menuArray[i];
                    name = NameFromText(itemText, typeof(ToolStripMenuItem), nameCreationService, true);
                    ToolStripItem item = null;
                    if (name.Contains("Separator"))
                    {
                        // create the component.
                        item = (ToolStripSeparator)_host.CreateComponent(typeof(ToolStripSeparator), name);
                        IDesigner designer = _host.GetDesigner(item);
                        if (designer is ComponentDesigner componentDesigner)
                        {
                            componentDesigner.InitializeNewComponent(null);
                        }

                        item.Text = itemText;
                    }
                    else
                    {
                        // create the component.
                        item = (ToolStripMenuItem)_host.CreateComponent(typeof(ToolStripMenuItem), name);
                        IDesigner designer = _host.GetDesigner(item);
                        if (designer is ComponentDesigner componentDesigner)
                        {
                            componentDesigner.InitializeNewComponent(null);
                        }

                        item.Text = itemText;
                        Keys shortcut = menuItemShortcuts[j][i];
                        if ((item is ToolStripMenuItem menuItem) && shortcut != Keys.None)
                        {
                            if (!ToolStripManager.IsShortcutDefined(shortcut) && ToolStripManager.IsValidShortcut(shortcut))
                            {
                                menuItem.ShortcutKeys = shortcut;
                            }
                        }

                        Bitmap image = null;
                        try
                        {
                            image = GetImage(menuItemImageNames[j][i]);
                        }
                        catch
                        {
                            // eat the exception.. as you may not find image for all MenuItems.
                        }

                        if (image is not null)
                        {
                            PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(item)["Image"];
                            Debug.Assert(imageProperty is not null, "Could not find 'Image' property in ToolStripItem.");
                            imageProperty?.SetValue(item, image);

                            item.ImageTransparentColor = Color.Magenta;
                        }
                    }

                    // the first item in each array is the root item.
                    if (i == 0)
                    {
                        rootItem = (ToolStripMenuItem)item;
                        rootItem.DropDown.SuspendLayout();
                    }
                    else
                    {
                        rootItem.DropDownItems.Add(item);
                    }

                    // If Last SubItem Added the Raise the Events
                    if (i == menuArray.Length - 1)
                    {
                        // member is OK to be null...
                        MemberDescriptor member = TypeDescriptor.GetProperties(rootItem)["DropDownItems"];
                        _changeService.OnComponentChanging(rootItem, member);
                        _changeService.OnComponentChanged(rootItem, member);
                    }
                }

                // finally, add it to the MainMenu.
                rootItem.DropDown.ResumeLayout(false);
                tool.Items.Add(rootItem);
                // If Last SubItem Added the Raise the Events
                if (j == menuItemNames.Length - 1)
                {
                    // member is OK to be null...
                    MemberDescriptor topMember = TypeDescriptor.GetProperties(tool)["Items"];
                    _changeService.OnComponentChanging(tool, topMember);
                    _changeService.OnComponentChanged(tool, topMember);
                }
            }
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException)
            {
                IUIService uiService = (IUIService)_provider.GetService(typeof(IUIService));
                uiService.ShowError(e.Message);
            }

            if (createMenu is not null)
            {
                createMenu.Cancel();
                createMenu = null;
            }
        }
        finally
        {
            ToolStripDesigner.s_autoAddNewItems = true;
            if (createMenu is not null)
            {
                createMenu.Commit();
                createMenu = null;
            }

            tool.ResumeLayout();
            // Select the Main Menu...
            ISelectionService selSvc = (ISelectionService)_provider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { _designer.Component });

            // Refresh the Glyph
            DesignerActionUIService actionUIService = (DesignerActionUIService)_provider.GetService(typeof(DesignerActionUIService));
            actionUIService?.Refresh(_designer.Component);

            // this will invalidate the Selection Glyphs.
            SelectionManager selMgr = (SelectionManager)_provider.GetService(typeof(SelectionManager));
            selMgr.Refresh();
        }
    }

    /// <summary>
    ///  Here is where all the fun stuff starts. We create the structure and apply the naming here.
    /// </summary>
    private void CreateStandardToolStrip(IDesignerHost host, ToolStrip tool)
    {
        // build the static menu items structure.
        //
        string[] menuItemNames = [SR.StandardMenuNew, SR.StandardMenuOpen, SR.StandardMenuSave, SR.StandardMenuPrint, "-", SR.StandardToolCut, SR.StandardMenuCopy, SR.StandardMenuPaste, "-", SR.StandardToolHelp];

        // build a image list mapping one-one the above menuItems list...
        // this is required so that the in LOCALIZED build we don't use the Localized item string.
        string[] menuItemImageNames = ["new", "open", "save", "print", "-", "cut", "copy", "paste", "-", "help"];
        Debug.Assert(host is not null, "can't create standard menu without designer _host.");

        if (host is null)
        {
            return;
        }

        tool.SuspendLayout();
        ToolStripDesigner.s_autoAddNewItems = false;
        // create a transaction so this happens as an atomic unit.
        DesignerTransaction createMenu = _host.CreateTransaction(SR.StandardMenuCreateDesc);
        try
        {
            INameCreationService nameCreationService = (INameCreationService)_provider.GetService(typeof(INameCreationService));
            string defaultName = "standardMainToolStrip";
            string name = defaultName;
            int index = 1;
            if (host is not null)
            {
                while (_host.Container.Components[name] is not null)
                {
                    name = $"{defaultName}{index++}";
                }
            }

            // keep an index in the MenuItemImageNames .. so that mapping is maintained.
            int menuItemImageNamesCount = 0;
            // now build the menu items themselves.
            foreach (string itemText in menuItemNames)
            {
                name = null;
                // for separators, just use the default name. Otherwise, remove any non-characters and get the name from the text.
                defaultName = "ToolStripButton";
                name = NameFromText(itemText, typeof(ToolStripButton), nameCreationService, true);
                ToolStripItem item = null;
                if (name.Contains("Separator"))
                {
                    // create the component.
                    item = (ToolStripSeparator)_host.CreateComponent(typeof(ToolStripSeparator), name);
                    IDesigner designer = _host.GetDesigner(item);
                    if (designer is ComponentDesigner componentDesigner)
                    {
                        componentDesigner.InitializeNewComponent(null);
                    }
                }
                else
                {
                    // create the component.
                    item = (ToolStripButton)_host.CreateComponent(typeof(ToolStripButton), name);
                    IDesigner designer = _host.GetDesigner(item);
                    if (designer is ComponentDesigner componentDesigner)
                    {
                        componentDesigner.InitializeNewComponent(null);
                    }

                    PropertyDescriptor displayStyleProperty = TypeDescriptor.GetProperties(item)["DisplayStyle"];
                    Debug.Assert(displayStyleProperty is not null, "Could not find 'Text' property in ToolStripItem.");
                    displayStyleProperty?.SetValue(item, ToolStripItemDisplayStyle.Image);

                    PropertyDescriptor textProperty = TypeDescriptor.GetProperties(item)["Text"];
                    Debug.Assert(textProperty is not null, "Could not find 'Text' property in ToolStripItem.");
                    textProperty?.SetValue(item, itemText);

                    Bitmap image = null;
                    try
                    {
                        image = GetImage(menuItemImageNames[menuItemImageNamesCount]);
                    }
                    catch
                    {
                        // eat the exception.. as you may not find image for all MenuItems.
                    }

                    if (image is not null)
                    {
                        PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(item)["Image"];
                        Debug.Assert(imageProperty is not null, "Could not find 'Image' property in ToolStripItem.");
                        imageProperty?.SetValue(item, image);

                        item.ImageTransparentColor = Color.Magenta;
                    }
                }

                tool.Items.Add(item);
                // increment the counter...
                menuItemImageNamesCount++;
            }

            // finally, add it to the Main ToolStrip.
            MemberDescriptor topMember = TypeDescriptor.GetProperties(tool)["Items"];
            _changeService.OnComponentChanging(tool, topMember);
            _changeService.OnComponentChanged(tool, topMember);
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException)
            {
                IUIService uiService = (IUIService)_provider.GetService(typeof(IUIService));
                uiService.ShowError(e.Message);
            }

            if (createMenu is not null)
            {
                createMenu.Cancel();
                createMenu = null;
            }
        }
        finally
        {
            // Reset the AutoAdd state
            ToolStripDesigner.s_autoAddNewItems = true;
            if (createMenu is not null)
            {
                createMenu.Commit();
                createMenu = null;
            }

            tool.ResumeLayout();
            // Select the Main Menu...
            ISelectionService selSvc = (ISelectionService)_provider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { _designer.Component });

            // Refresh the Glyph
            DesignerActionUIService actionUIService = (DesignerActionUIService)_provider.GetService(typeof(DesignerActionUIService));
            actionUIService?.Refresh(_designer.Component);

            // this will invalidate the Selection Glyphs.
            SelectionManager selMgr = (SelectionManager)_provider.GetService(typeof(SelectionManager));
            selMgr.Refresh();
        }
    }

    /// <summary>
    ///  Helper Function to get Images from types.
    /// </summary>
    private static Bitmap GetImage(string name)
    {
        Bitmap image = null;
        if (name.StartsWith("new", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "new").ToBitmap();
        }
        else if (name.StartsWith("open", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "open").ToBitmap();
        }
        else if (name.StartsWith("save", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "save").ToBitmap();
        }
        else if (name.StartsWith("printPreview", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "printPreview").ToBitmap();
        }
        else if (name.StartsWith("print", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "print").ToBitmap();
        }
        else if (name.StartsWith("cut", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "cut").ToBitmap();
        }
        else if (name.StartsWith("copy", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "copy").ToBitmap();
        }
        else if (name.StartsWith("paste", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "paste").ToBitmap();
        }
        else if (name.StartsWith("help", StringComparison.Ordinal))
        {
            image = new Icon(typeof(ToolStripMenuItem), "help").ToBitmap();
        }

        return image;
    }

    /// <summary>
    ///  Computes a name from a text label by removing all spaces and non-alphanumeric characters.
    /// </summary>
    private string NameFromText(string text, Type itemType, INameCreationService nameCreationService, bool adjustCapitalization)
    {
        string baseName;
        // for separators, name them ToolStripSeparator...
        if (text == "-")
        {
            baseName = "toolStripSeparator";
        }
        else
        {
            string nameSuffix = itemType.Name;
            // remove all the non letter and number characters. Append length of "MenuItem"
            Text.StringBuilder name = new(text.Length + nameSuffix.Length);
            bool firstCharSeen = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetterOrDigit(c))
                {
                    if (!firstCharSeen)
                    {
                        c = char.ToLower(c, CultureInfo.CurrentCulture);
                        firstCharSeen = true;
                    }

                    name.Append(c);
                }
            }

            name.Append(nameSuffix);
            if (adjustCapitalization)
            {
                string nameOfRandomItem = ToolStripDesigner.NameFromText(null, typeof(ToolStripMenuItem), _designer.Component.Site);
                if (!string.IsNullOrEmpty(nameOfRandomItem) && char.IsUpper(nameOfRandomItem[0]))
                {
                    name[0] = char.ToUpper(name[0], CultureInfo.InvariantCulture);
                }
            }

            baseName = name.ToString();
        }

        // see if this name matches another one in the container..
        object existingComponent = _host.Container.Components[baseName];
        if (existingComponent is null)
        {
            if (!nameCreationService.IsValidName(baseName))
            {
                // we don't have a name collision but this still isn't a valid name...
                // something is wrong and we can't make a valid identifier out of this so bail.
                return nameCreationService.CreateName(_host.Container, itemType);
            }
            else
            {
                return baseName;
            }
        }
        else
        {
            // start appending numbers.
            string newName = baseName;
            for (int indexer = 1; !nameCreationService.IsValidName(newName); indexer++)
            {
                newName = $"{baseName}{indexer}";
            }

            return newName;
        }
    }
}
