// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Designer for ToolStripDropDowns...just provides the Edit... verb.
    /// </summary>
    internal class ToolStripDropDownDesigner : ComponentDesigner
    {
        private ISelectionService selSvc;
        private MenuStrip designMenu;
        private ToolStripMenuItem menuItem;
        private IDesignerHost host;
        private ToolStripDropDown dropDown;
        private bool selected;
        private ControlBodyGlyph dummyToolStripGlyph;
        private uint _editingCollection; // non-zero if the collection editor is up for this ToolStrip or a child of it.
        FormDocumentDesigner parentFormDesigner;
        internal ToolStripMenuItem currentParent;
        private INestedContainer nestedContainer; //NestedContainer for our DesignTime MenuItem.
        private UndoEngine undoEngine;

        /// <summary>
        ///  ShadowProperty.
        /// </summary>
        private bool AutoClose
        {
            get => (bool)ShadowProperties[nameof(AutoClose)];
            set => ShadowProperties[nameof(AutoClose)] = value;
        }

        private bool AllowDrop
        {
            get => (bool)ShadowProperties[nameof(AllowDrop)];
            set => ShadowProperties[nameof(AllowDrop)] = value;
        }

        /// <summary>
        ///  Adds designer actions to the ActionLists collection.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection actionLists = new DesignerActionListCollection();
                actionLists.AddRange(base.ActionLists);
                ContextMenuStripActionList cmActionList = new ContextMenuStripActionList(this);
                if (cmActionList != null)
                {
                    actionLists.Add(cmActionList);
                }
                // finally add the verbs for this component there...
                DesignerVerbCollection cmVerbs = Verbs;
                if (cmVerbs != null && cmVerbs.Count != 0)
                {
                    DesignerVerb[] cmverbsArray = new DesignerVerb[cmVerbs.Count];
                    cmVerbs.CopyTo(cmverbsArray, 0);
                    actionLists.Add(new DesignerActionVerbList(cmverbsArray));
                }
                return actionLists;
            }
        }

        /// <summary>
        ///  The ToolStripItems are the associated components.   We want those to come with in any cut, copy opreations.
        /// </summary>
        public override System.Collections.ICollection AssociatedComponents
        {
            get => ((ToolStrip)Component).Items;
        }

        // Dummy menuItem that is used for the contextMenuStrip design
        public ToolStripMenuItem DesignerMenuItem
        {
            get => menuItem;
        }

        /// <summary>
        ///  Set by the ToolStripItemCollectionEditor when it's launched for this The Items property doesnt open another instance
        ///  of collectioneditor.  We count this so that we can deal with nestings.
        /// </summary>
        internal bool EditingCollection
        {
            get => _editingCollection != 0;
            set
            {
                if (value)
                {
                    _editingCollection++;
                }
                else
                {
                    _editingCollection--;
                }
            }
        }

        // ContextMenuStrip if Inherited ACT as Readonly.
        protected override InheritanceAttribute InheritanceAttribute
        {
            get
            {
                if ((base.InheritanceAttribute == InheritanceAttribute.Inherited))
                {
                    return InheritanceAttribute.InheritedReadOnly;
                }
                return base.InheritanceAttribute;
            }
        }

        /// <summary>
        ///  Prefilter this property so that we can set the right To Left on the Design Menu...
        /// </summary>
        private RightToLeft RightToLeft
        {
            get => dropDown.RightToLeft;
            set
            {
                if (menuItem != null && designMenu != null && value != RightToLeft)
                {
                    Rectangle bounds = Rectangle.Empty;
                    try
                    {
                        bounds = dropDown.Bounds;
                        menuItem.HideDropDown();
                        designMenu.RightToLeft = value;
                        dropDown.RightToLeft = value;
                    }
                    finally
                    {
                        BehaviorService behaviorService = (BehaviorService)GetService(typeof(BehaviorService));
                        if (behaviorService != null && bounds != Rectangle.Empty)
                        {
                            behaviorService.Invalidate(bounds);
                        }
                        ToolStripMenuItemDesigner itemDesigner = (ToolStripMenuItemDesigner)host.GetDesigner(menuItem);
                        if (itemDesigner != null)
                        {
                            itemDesigner.InitializeDropDown();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  shadowing the SettingsKey so we can default it to be RootComponent.Name + "." + Control.Name
        /// </summary>
        private string SettingsKey
        {
            get
            {
                if (string.IsNullOrEmpty((string)ShadowProperties[SettingsKeyName]))
                {
                    if (Component is IPersistComponentSettings persistableComponent && host != null)
                    {
                        if (persistableComponent.SettingsKey is null)
                        {
                            IComponent rootComponent = host.RootComponent;
                            if (rootComponent != null && rootComponent != persistableComponent)
                            {
                                ShadowProperties[SettingsKeyName] = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", rootComponent.Site.Name, Component.Site.Name);
                            }
                            else
                            {
                                ShadowProperties[SettingsKeyName] = Component.Site.Name;
                            }
                        }
                        persistableComponent.SettingsKey = ShadowProperties[SettingsKeyName] as string;
                        return persistableComponent.SettingsKey;
                    }
                }
                return ShadowProperties[SettingsKeyName] as string;
            }
            set
            {
                ShadowProperties[SettingsKeyName] = value;
                if (Component is IPersistComponentSettings persistableComponent)
                {
                    persistableComponent.SettingsKey = value;
                }
            }
        }

        // We have to add the glyphs ourselves.
        private void AddSelectionGlyphs(SelectionManager selMgr, ISelectionService selectionService)
        {
            //If one or many of our items are selected then Add Selection Glyphs ourselces since this is a ComponentDesigner which wont get called on the "GetGlyphs"
            ICollection selComponents = selectionService.GetSelectedComponents();
            GlyphCollection glyphs = new GlyphCollection();
            foreach (object selComp in selComponents)
            {
                if (selComp is ToolStripItem item)
                {
                    ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)host.GetDesigner(item);
                    if (itemDesigner != null)
                    {
                        itemDesigner.GetGlyphs(ref glyphs, new ResizeBehavior(item.Site));
                    }
                }
            }
            // Get the Glyphs union Rectangle.
            if (glyphs.Count > 0)
            {
                // Add Glyphs and then invalidate the unionRect
                selMgr.SelectionGlyphAdorner.Glyphs.AddRange(glyphs);
            }
        }

        // internal method called by outside designers to add glyphs for the ContextMenuStrip
        internal void AddSelectionGlyphs()
        {
            SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
            if (selMgr != null)
            {
                AddSelectionGlyphs(selMgr, selSvc);
            }
        }

        /// <summary>
        ///  Disposes of this designer.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unhook our services
                if (selSvc != null)
                {
                    selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
                    selSvc.SelectionChanging -= new EventHandler(OnSelectionChanging);
                }

                DisposeMenu();
                if (designMenu != null)
                {
                    designMenu.Dispose();
                    designMenu = null;
                }
                if (dummyToolStripGlyph != null)
                {
                    dummyToolStripGlyph = null;
                }
                if (undoEngine != null)
                {
                    undoEngine.Undone -= new EventHandler(OnUndone);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Disposes of this dummy menuItem and its designer..
        /// </summary>
        private void DisposeMenu()
        {
            HideMenu();
            if (host.RootComponent is Control form)
            {
                if (designMenu != null)
                {
                    form.Controls.Remove(designMenu);
                }
                if (menuItem != null)
                {
                    if (nestedContainer != null)
                    {
                        nestedContainer.Dispose();
                        nestedContainer = null;
                    }
                    menuItem.Dispose();
                    menuItem = null;
                }
            }
        }

        // private helper function to Hide the ContextMenu structure.
        private void HideMenu()
        {
            if (menuItem is null)
            {
                return;
            }

            selected = false;
            if (host.RootComponent is Control form)
            {
                menuItem.DropDown.AutoClose = true;
                menuItem.HideDropDown();
                menuItem.Visible = false;
                //Hide the MenuItem DropDown.
                designMenu.Visible = false;
                //Invalidate the Bounds..
                ToolStripAdornerWindowService toolStripAdornerWindowService = (ToolStripAdornerWindowService)GetService(typeof(ToolStripAdornerWindowService));
                if (toolStripAdornerWindowService != null)
                {
                    //toolStripAdornerWindowService.Invalidate(boundsToInvalidate);
                    toolStripAdornerWindowService.Invalidate();
                }

                //Query for the Behavior Service and Remove Glyph....
                BehaviorService behaviorService = (BehaviorService)GetService(typeof(BehaviorService));
                if (behaviorService != null)
                {
                    if (dummyToolStripGlyph != null)
                    {
                        SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                        if (selMgr != null)
                        {
                            if (selMgr.BodyGlyphAdorner.Glyphs.Contains(dummyToolStripGlyph))
                            {
                                selMgr.BodyGlyphAdorner.Glyphs.Remove(dummyToolStripGlyph);
                            }
                            selMgr.Refresh();
                        }
                    }
                    dummyToolStripGlyph = null;
                }

                //Unhook all the events for DesignMenuItem
                if (menuItem != null)
                {
                    if (host.GetDesigner(menuItem) is ToolStripMenuItemDesigner itemDesigner)
                    {
                        itemDesigner.UnHookEvents();
                        itemDesigner.RemoveTypeHereNode(menuItem);
                    }
                }
            }
        }

        /// <summary>
        ///  Initialize the item.
        /// </summary>
        // EditorServiceContext is newed up to add Edit Items verb.
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            host = (IDesignerHost)GetService(typeof(IDesignerHost));
            //Add the EditService so that the ToolStrip can do its own Tab and Keyboard Handling
            ToolStripKeyboardHandlingService keyboardHandlingService = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
            if (keyboardHandlingService is null)
            {
                keyboardHandlingService = new ToolStripKeyboardHandlingService(component.Site);
            }

            //Add the InsituEditService so that the ToolStrip can do its own Insitu Editing
            ISupportInSituService inSituService = (ISupportInSituService)GetService(typeof(ISupportInSituService));
            if (inSituService is null)
            {
                inSituService = new ToolStripInSituService(Component.Site);
            }

            dropDown = (ToolStripDropDown)Component;
            dropDown.Visible = false;
            //shadow properties as we would change these for DropDowns at DesignTime.
            AutoClose = dropDown.AutoClose;
            AllowDrop = dropDown.AllowDrop;

            selSvc = (ISelectionService)GetService(typeof(ISelectionService));
            if (selSvc != null)
            {
                // first select the rootComponent and then hook on the events... but not if we are loading - VSWhidbey #484576
                if (host != null && !host.Loading)
                {
                    selSvc.SetSelectedComponents(new IComponent[] { host.RootComponent }, SelectionTypes.Replace);
                }
                selSvc.SelectionChanging += new EventHandler(OnSelectionChanging);
                selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
            }

            designMenu = new MenuStrip
            {
                Visible = false,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            if (DpiHelper.IsScalingRequired)
            {
                designMenu.Height = DpiHelper.LogicalToDeviceUnitsY(designMenu.Height);
            }
            //Add MenuItem
            if (host.RootComponent is Control form)
            {
                menuItem = new ToolStripMenuItem
                {
                    BackColor = SystemColors.Window,
                    Name = Component.Site.Name
                };
                menuItem.Text = (dropDown != null) ? dropDown.GetType().Name : menuItem.Name;
                designMenu.Items.Add(menuItem);
                form.Controls.Add(designMenu);
                designMenu.SendToBack();
                nestedContainer = GetService(typeof(INestedContainer)) as INestedContainer;
                if (nestedContainer != null)
                {
                    nestedContainer.Add(menuItem, "ContextMenuStrip");
                }
            }

            // init the verb.
            new EditorServiceContext(this, TypeDescriptor.GetProperties(Component)["Items"], SR.ToolStripItemCollectionEditorVerb);
            // use the UndoEngine.Undone to Show the DropDown Again..
            if (undoEngine is null)
            {
                undoEngine = GetService(typeof(UndoEngine)) as UndoEngine;
                if (undoEngine != null)
                {
                    undoEngine.Undone += new EventHandler(OnUndone);
                }
            }
        }

        // Helper function to check if the ToolStripItem on the ContextMenu is selected.
        private bool IsContextMenuStripItemSelected(ISelectionService selectionService)
        {
            bool showDesignMenu = false;
            if (menuItem is null)
            {
                return showDesignMenu;
            }

            ToolStripDropDown topmost = null;
            IComponent comp = (IComponent)selectionService.PrimarySelection;
            if (comp is null && dropDown.Visible)
            {
                ToolStripKeyboardHandlingService keyboardHandlingService = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
                if (keyboardHandlingService != null)
                {
                    comp = (IComponent)keyboardHandlingService.SelectedDesignerControl;
                }
            }
            // This case covers (a) and (b) above....
            if (comp is ToolStripDropDownItem)
            {
                if (comp is ToolStripDropDownItem currentItem && currentItem == menuItem)
                {
                    topmost = menuItem.DropDown;
                }
                else
                {
                    ToolStripMenuItemDesigner itemDesigner = (ToolStripMenuItemDesigner)host.GetDesigner(comp);
                    if (itemDesigner != null)
                    {
                        topmost = itemDesigner.GetFirstDropDown((ToolStripDropDownItem)comp);
                    }
                }
            }
            else if (comp is ToolStripItem) //case (c)
            {
                if (!(((ToolStripItem)comp).GetCurrentParent() is ToolStripDropDown parent))
                {
                    // Try if the item has not laid out...
                    parent = ((ToolStripItem)comp).Owner as ToolStripDropDown;
                }
                if (parent != null && parent.Visible)
                {
                    ToolStripItem ownerItem = parent.OwnerItem;
                    if (ownerItem != null && ownerItem == menuItem)
                    {
                        topmost = menuItem.DropDown;
                    }
                    else
                    {
                        ToolStripMenuItemDesigner itemDesigner = (ToolStripMenuItemDesigner)host.GetDesigner(ownerItem);
                        if (itemDesigner != null)
                        {
                            topmost = itemDesigner.GetFirstDropDown((ToolStripDropDownItem)ownerItem);
                        }
                    }
                }
            }
            if (topmost != null)
            {
                ToolStripItem topMostItem = topmost.OwnerItem;
                if (topMostItem == menuItem)
                {
                    showDesignMenu = true;
                }
            }
            return showDesignMenu;
        }

        /// <summary>
        ///  Listens SelectionChanging to Show the MenuDesigner.
        /// </summary>
        private void OnSelectionChanging(object sender, EventArgs e)
        {
            ISelectionService selectionService = (ISelectionService)sender;
            // If we are no longer selected ... Hide the DropDown
            bool showDesignMenu = IsContextMenuStripItemSelected(selectionService) || Component.Equals(selectionService.PrimarySelection);
            if (selected && !showDesignMenu)
            {
                HideMenu();
            }
        }

        /// <summary>
        ///  Listens SelectionChanged to Show the MenuDesigner.
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (Component is null || menuItem is null)
            {
                return;
            }
            ISelectionService selectionService = (ISelectionService)sender;
            // Select the container if TopLevel Dummy MenuItem is selected.
            if (selectionService.GetComponentSelected(menuItem))
            {
                selectionService.SetSelectedComponents(new IComponent[] { Component }, SelectionTypes.Replace);
            }

            //return if DropDown is already is selected.
            if (Component.Equals(selectionService.PrimarySelection) && selected)
            {
                return;
            }

            bool showDesignMenu = IsContextMenuStripItemSelected(selectionService) || Component.Equals(selectionService.PrimarySelection);

            if (showDesignMenu)
            {
                if (!dropDown.Visible)
                {
                    ShowMenu();
                }
                //Selection change would remove our Glyph from the BodyGlyph Collection.
                SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                if (selMgr != null)
                {
                    if (dummyToolStripGlyph != null)
                    {
                        selMgr.BodyGlyphAdorner.Glyphs.Insert(0, dummyToolStripGlyph);
                    }
                    // Add our SelectionGlyphs and Invalidate.
                    AddSelectionGlyphs(selMgr, selectionService);
                }
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties the component it is designing will expose through the TypeDescriptor object.  This method is called immediately before its corresponding "Post" method. If you are overriding this method you should call the base implementation before you perform your own filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            PropertyDescriptor prop;
            string[] shadowProps = new string[] { "AutoClose", SettingsKeyName, "RightToLeft", "AllowDrop" };
            Attribute[] empty = Array.Empty<Attribute>();
            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ToolStripDropDownDesigner), prop, empty);
                }
            }
        }

        // Reset Settings.
        public void ResetSettingsKey()
        {
            if (Component is IPersistComponentSettings persistableComponent)
            {
                SettingsKey = null;
            }
        }

        /// <summary>
        /// Resets the ToolStripDropDown AutoClose to be the default padding
        /// </summary>
        private void ResetAutoClose()
        {
            ShadowProperties[nameof(AutoClose)] = true;
        }

        /// <summary>
        /// Restores the ToolStripDropDown AutoClose to be the value set in the property grid.
        /// </summary>
        private void RestoreAutoClose()
        {
            dropDown.AutoClose = (bool)ShadowProperties[nameof(AutoClose)];
        }

        /// <summary>
        /// Resets the ToolStripDropDown AllowDrop to be the default padding
        /// </summary>
        private void ResetAllowDrop()
        {
            ShadowProperties[nameof(AllowDrop)] = false;
        }

        /// <summary>
        /// Restores the ToolStripDropDown AllowDrop to be the value set in the property grid.
        /// </summary>
        private void RestoreAllowDrop()
        {
            dropDown.AutoClose = (bool)ShadowProperties[nameof(AllowDrop)];
        }

        /// <summary>
        /// Resets the ToolStripDropDown RightToLeft to be the default RightToLeft
        /// </summary>
        private void ResetRightToLeft()
        {
            RightToLeft = RightToLeft.No;
        }

        /// <summary>
        ///  Show the MenuDesigner; used by ToolStripmenuItemdesigner to show the menu when the user selects the dropDown item through the PG or Document outline. The editor node will be selected by default.
        /// </summary>
        public void ShowMenu()
        {
            int count = dropDown.Items.Count - 1;
            if (count >= 0)
            {
                ShowMenu(dropDown.Items[count]);
            }
            else
            {
                ShowMenu(null);
            }
        }

        /// <summary>
        ///  Show the MenuDesigner; used by ToolStripmenuItemdesigner to show the menu when the user selects the dropDown item through the PG or Document outline. The input toolstrip item will be selected.
        /// </summary>
        public void ShowMenu(ToolStripItem selectedItem)
        {
            if (menuItem is null)
            {
                return;
            }

            Control parent = designMenu.Parent as Control;
            if (parent is Form parentForm)
            {
                parentFormDesigner = host.GetDesigner(parentForm) as FormDocumentDesigner;
            }

            selected = true;
            designMenu.Visible = true;
            designMenu.BringToFront();
            menuItem.Visible = true;

            // Check if this is a design-time DropDown
            if (currentParent != null && currentParent != menuItem)
            {
                if (host.GetDesigner(currentParent) is ToolStripMenuItemDesigner ownerItemDesigner)
                {
                    ownerItemDesigner.RemoveTypeHereNode(currentParent);
                }
            }

            //Everytime you hide/show .. set the DropDown of the designer MenuItem to the component dropDown beign designed.
            menuItem.DropDown = dropDown;
            menuItem.DropDown.OwnerItem = menuItem;
            if (dropDown.Items.Count > 0)
            {
                ToolStripItem[] items = new ToolStripItem[dropDown.Items.Count];
                dropDown.Items.CopyTo(items, 0);
                foreach (ToolStripItem toolItem in items)
                {
                    if (toolItem is DesignerToolStripControlHost)
                    {
                        dropDown.Items.Remove(toolItem);
                    }
                }
            }

            ToolStripMenuItemDesigner itemDesigner = (ToolStripMenuItemDesigner)host.GetDesigner(menuItem);
            BehaviorService behaviorService = (BehaviorService)GetService(typeof(BehaviorService));
            if (behaviorService != null)
            {
                // Show the contextMenu only if the dummy menuStrip is contained in the Form. Refer to VsWhidbey 484317 for more details.
                if (itemDesigner != null && parent != null)
                {
                    Rectangle parentBounds = behaviorService.ControlRectInAdornerWindow(parent);
                    Rectangle menuBounds = behaviorService.ControlRectInAdornerWindow(designMenu);
                    if (ToolStripDesigner.IsGlyphTotallyVisible(menuBounds, parentBounds))
                    {
                        itemDesigner.InitializeDropDown();
                    }
                }

                if (dummyToolStripGlyph is null)
                {
                    Point loc = behaviorService.ControlToAdornerWindow(designMenu);
                    Rectangle r = designMenu.Bounds;
                    r.Offset(loc);
                    dummyToolStripGlyph = new ControlBodyGlyph(r, Cursor.Current, menuItem, new ContextMenuStripBehavior(menuItem));
                    SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                    if (selMgr != null)
                    {
                        selMgr.BodyGlyphAdorner.Glyphs.Insert(0, dummyToolStripGlyph);
                    }
                }

                if (selectedItem != null)
                {
                    ToolStripKeyboardHandlingService keyboardHandlingService = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
                    if (keyboardHandlingService != null)
                    {
                        keyboardHandlingService.SelectedDesignerControl = selectedItem;
                    }
                }
            }
        }

        // Should the designer serialize the settings?
        private bool ShouldSerializeSettingsKey() => (Component is IPersistComponentSettings persistableComponent && persistableComponent.SaveSettings && SettingsKey != null);

        /// <summary>
        /// Since we're shadowing ToolStripDropDown AutoClose, we get called here to determine whether or not to serialize
        /// </summary>
        private bool ShouldSerializeAutoClose() => (!(bool)ShadowProperties[nameof(AutoClose)]);

        /// <summary>
        /// Since we're shadowing ToolStripDropDown AllowDrop, we get called here to determine whether or not to serialize
        /// </summary>
        private bool ShouldSerializeAllowDrop() => AllowDrop;

        /// <summary>
        /// Since we're shadowing ToolStripDropDown RightToLeft, we get called here to determine whether or not to serialize
        /// </summary>
        private bool ShouldSerializeRightToLeft() => RightToLeft != RightToLeft.No;

        /// <summary>
        ///  ResumeLayout after Undone.
        /// </summary>
        private void OnUndone(object source, EventArgs e)
        {
            if (selSvc != null && Component.Equals(selSvc.PrimarySelection))
            {
                HideMenu();
                ShowMenu();
            }
        }

        /// <summary>
        ///  This is an internal class which provides the Behavior for our MenuStrip Body Glyph. This will just eat the MouseUps...
        /// </summary>
        internal class ContextMenuStripBehavior : System.Windows.Forms.Design.Behavior.Behavior
        {
            readonly ToolStripMenuItem _item;
            internal ContextMenuStripBehavior(ToolStripMenuItem menuItem)
            {
                _item = menuItem;
            }

            public override bool OnMouseUp(Glyph g, MouseButtons button)
            {
                if (button == MouseButtons.Left)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
