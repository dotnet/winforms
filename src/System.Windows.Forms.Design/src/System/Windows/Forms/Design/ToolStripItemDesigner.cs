// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    internal class ToolStripItemDesigner : ComponentDesigner
    {
        private const int GLYPHINSET = 2;
        // Cached in value of the TemplateNode (which is the InSitu Editor)
        private ToolStripTemplateNode _editorNode;
        // Used by the ParentDesigner (ToolStripDesigner) to know whether there is any active Editor.
        private bool isEditorActive;
        // this property is used in the InitializeNewComponent not to set the text for the ToolstripItem
        private bool internalCreate;
        //hook to SelectionService to listen to SelectionChanged
        private ISelectionService selSvc;
        //ToolStripItems Visibility needs to be WYSIWYG.
        private bool currentVisible;
        // Required to remove Body Glyphs...
        internal ControlBodyGlyph bodyGlyph;
        //bool which is set if we Add Dummy Item
        internal bool dummyItemAdded;
        //Needed to Store the DRAGDROP Rect from the ToolStripItemBehavior.
        internal Rectangle dragBoxFromMouseDown = Rectangle.Empty;
        //defaulted to invalid index. this will be set by the behaviour.
        internal int indexOfItemUnderMouseToDrag = -1;
        private ToolStripItemCustomMenuItemCollection toolStripItemCustomMenuItemCollection;

        internal bool AutoSize
        {
            get => (bool)ShadowProperties[nameof(AutoSize)];
            set
            {
                bool autoSize = (bool)ShadowProperties[nameof(AutoSize)];
                // always set this in regardless of whether the property changed. it can come back to bite later after in-situ editing if we dont.
                ShadowProperties[nameof(AutoSize)] = value;
                if (value != autoSize)
                {
                    ToolStripItem.AutoSize = value;
                }
            }
        }

        private string AccessibleName
        {
            get
            {
                return (string)ShadowProperties[nameof(AccessibleName)];
            }
            set
            {
                ShadowProperties[nameof(AccessibleName)] = value;
            }
        }

        /// <summary>
        ///  Associated Parent Designer
        /// </summary>
        internal override bool CanBeAssociatedWith(IDesigner parentDesigner)
        {
            return (parentDesigner is ToolStripDesigner);
        }

        /// <summary>
        ///  Designer Custom ContextMenu.
        /// </summary>
        private ContextMenuStrip DesignerContextMenu
        {
            get
            {
                BaseContextMenuStrip toolStripContextMenu = new BaseContextMenuStrip(Component.Site, ToolStripItem);
                // If multiple Items Selected dont show the custom properties...
                if (selSvc.SelectionCount > 1)
                {
                    toolStripContextMenu.GroupOrdering.Clear();
                    toolStripContextMenu.GroupOrdering.AddRange(new string[] { StandardGroups.Code, StandardGroups.Selection, StandardGroups.Edit, StandardGroups.Properties });
                }
                else
                {
                    toolStripContextMenu.GroupOrdering.Clear();
                    toolStripContextMenu.GroupOrdering.AddRange(new string[] { StandardGroups.Code, StandardGroups.Custom, StandardGroups.Selection, StandardGroups.Edit, StandardGroups.Properties });
                    toolStripContextMenu.Text = "CustomContextMenu";
                    if (toolStripItemCustomMenuItemCollection is null)
                    {
                        toolStripItemCustomMenuItemCollection = new ToolStripItemCustomMenuItemCollection(Component.Site, ToolStripItem);
                    }
                    foreach (ToolStripItem item in toolStripItemCustomMenuItemCollection)
                    {
                        toolStripContextMenu.Groups[StandardGroups.Custom].Items.Add(item);
                    }
                }

                // Refresh the list on every show..
                if (toolStripItemCustomMenuItemCollection != null)
                {
                    toolStripItemCustomMenuItemCollection.RefreshItems();
                }
                toolStripContextMenu.Populated = false;
                return toolStripContextMenu;
            }
        }

        /// <summary>
        ///  ToolStripEditorManager used this internal property to  Activate the editor.
        /// </summary>
        internal virtual ToolStripTemplateNode Editor
        {
            get => _editorNode;
            set => _editorNode = value;
        }

        // ToolStripItems if Inherited ACT as Readonly.
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
        ///  ToolStripEditorManager used this internal property to  set the the desinger's IsEditorActive to notify  if this item has entered or exited the InSitu Edit Mode.
        /// </summary>
        internal bool IsEditorActive
        {
            get => isEditorActive;
            set => isEditorActive = value;
        }

        /// <summary>
        ///  When the ToolStripItem is created we dont want InitializeNewComponent to set the "text" we do it ourselves from the Text the User has provided in the InSitu Edit Mode. Reason being the item and the Parent unnecessarily Layout and cause flicker.
        /// </summary>
        internal bool InternalCreate
        {
            get => internalCreate;
            set => internalCreate = value;
        }

        protected IComponent ImmediateParent
        {
            get
            {
                if (ToolStripItem != null)
                {
                    ToolStrip parent = ToolStripItem.GetCurrentParent();
                    return parent ?? ToolStripItem.Owner;
                }
                return null;
            }
        }

        private ToolStripItemOverflow Overflow
        {
            get => (ToolStripItemOverflow)ShadowProperties[nameof(Overflow)];
            set
            {
                // first Hide the Overflow..
                if (ToolStripItem.IsOnOverflow)
                {
                    ToolStrip strip = ToolStripItem.Owner as ToolStrip;
                    if (strip.OverflowButton.DropDown.Visible)
                    {
                        strip.OverflowButton.HideDropDown();
                    }
                }
                if (ToolStripItem is ToolStripDropDownItem)
                {
                    ToolStripDropDownItem item = ToolStripItem as ToolStripDropDownItem;
                    item.HideDropDown();
                }
                //set the value on actual item
                if (value != ToolStripItem.Overflow)
                {
                    ToolStripItem.Overflow = value;
                    ShadowProperties[nameof(Overflow)] = value;
                }

                // Since this cause the whole Layout to Change ... Call SyncSelection to reset the glyphs...
                BehaviorService b = (BehaviorService)GetService(typeof(BehaviorService));
                if (b != null)
                {
                    b.SyncSelection();
                }
            }
        }

        protected override IComponent ParentComponent
        {
            get
            {
                if (ToolStripItem != null)
                {
                    if (ToolStripItem.IsOnDropDown && !ToolStripItem.IsOnOverflow)
                    {
                        if (ImmediateParent is ToolStripDropDown parentDropDown)
                        {
                            if (parentDropDown.IsAutoGenerated)
                            {
                                return parentDropDown.OwnerItem;
                            }
                            else
                            {
                                return parentDropDown;
                            }
                        }
                    }
                    return GetMainToolStrip();
                }
                return null;
            }
        }

        /// <summary>
        ///  Easy method for getting to the ToolStripItem
        /// </summary>
        public ToolStripItem ToolStripItem
        {
            get => (ToolStripItem)Component;
        }

        protected bool Visible
        {
            get => (bool)ShadowProperties[nameof(Visible)];
            set
            {
                ShadowProperties[nameof(Visible)] = value;
                currentVisible = value;
            }
        }

        /// <summary>
        ///  This method adds the Parent Hierarchy to arraylist and returns that arraylist to the
        ///  Base ContextMenu provider. This way the ToolStripItem can show the right parents in
        ///  the contextMenu.
        /// </summary>
        internal ArrayList AddParentTree()
        {
            ArrayList parentControls = new ArrayList();
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (designerHost != null)
            {
                IComponent root = designerHost.RootComponent;
                Component startComp = ToolStripItem;
                if (startComp != null && root != null)
                {
                    while (startComp != root)
                    {
                        if (startComp is ToolStripItem)
                        {
                            ToolStripItem item = startComp as ToolStripItem;
                            if (item.IsOnDropDown)
                            {
                                if (item.IsOnOverflow)
                                {
                                    parentControls.Add(item.Owner);
                                    startComp = item.Owner;
                                }
                                else
                                {
                                    if (item.Owner is ToolStripDropDown parentDropDown)
                                    {
                                        ToolStripItem ownerItem = parentDropDown.OwnerItem;
                                        if (ownerItem != null)
                                        {
                                            parentControls.Add(ownerItem);
                                            startComp = ownerItem;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (item.Owner.Site != null)
                                {
                                    parentControls.Add(item.Owner);
                                }
                                startComp = item.Owner;
                            }
                        }
                        else if (startComp is Control)
                        {
                            Control selectedControl = startComp as Control;
                            Control parentControl = selectedControl.Parent;
                            if (parentControl.Site != null)
                            {
                                parentControls.Add(parentControl);
                            }
                            startComp = parentControl;
                        }
                    }
                }
            }
            return parentControls;
        }

        /// <summary>
        ///  Creates the InSitu Edit Node (which is called the TemplateNode).
        /// </summary>
        private void CreateDummyNode()
        {
            _editorNode = new ToolStripTemplateNode(ToolStripItem, ToolStripItem.Text, ToolStripItem.Image);
        }

        /// <summary>
        ///  This is called by the TemplateNode to Commit the Edit. This Function Simply changes the "Text and Image" property of the  current ToolStripItem.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        internal virtual void CommitEdit(Type type, string text, bool commit, bool enterKeyPressed, bool tabKeyPressed)
        {
            ToolStripItem newItem = null;
            SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
            BehaviorService bSvc = (BehaviorService)GetService(typeof(BehaviorService));
            ToolStrip immediateParent = ImmediateParent as ToolStrip;
            immediateParent.SuspendLayout();
            HideDummyNode();
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            ToolStripDesigner designer = (ToolStripDesigner)designerHost.GetDesigner(ToolStripItem.Owner);
            if (designer != null && designer.EditManager != null)
            {
                designer.EditManager.ActivateEditor(null, false);
            }
            // Cannot Add ToolStripSeparator to MenuStrip
            if (immediateParent is MenuStrip && type == typeof(ToolStripSeparator))
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    IUIService uiSvc = (IUIService)host.GetService(typeof(IUIService));
                    if (uiSvc != null)
                    {
                        uiSvc.ShowError(SR.ToolStripSeparatorError);
                        // dont commit the item..
                        commit = false;
                        // Select the MenuStrip
                        if (selSvc != null)
                        {
                            selSvc.SetSelectedComponents(new object[] { immediateParent });
                        }
                    }
                }
            }
            if (commit)
            {
                if (dummyItemAdded)
                {
                    try
                    {
                        RemoveItem();
                        newItem = designer.AddNewItem(type, text, enterKeyPressed, false /* Dont select the templateNode but select the newly added item */);
                    }
                    finally
                    {
                        if (designer.NewItemTransaction != null)
                        {
                            designer.NewItemTransaction.Commit();
                            designer.NewItemTransaction = null;
                        }
                    }
                }
                else
                {
                    //create our transaction
                    DesignerTransaction designerTransaction = designerHost.CreateTransaction(SR.ToolStripItemPropertyChangeTransaction);
                    try
                    {
                        //Change the Text...
                        PropertyDescriptor textProp = TypeDescriptor.GetProperties(ToolStripItem)["Text"];
                        string oldValue = (string)textProp.GetValue(ToolStripItem);
                        if (textProp != null && text != oldValue)
                        {
                            textProp.SetValue(ToolStripItem, text);
                        }
                        if (enterKeyPressed && selSvc != null)
                        {
                            SelectNextItem(selSvc, enterKeyPressed, designer);
                        }
                    }
                    catch (Exception e)
                    {
                        if (designerTransaction != null)
                        {
                            designerTransaction.Cancel();
                            designerTransaction = null;
                        }
                        if (selMgr != null)
                        {
                            selMgr.Refresh();
                        }
                        if (ClientUtils.IsCriticalException(e))
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        if (designerTransaction != null)
                        {
                            designerTransaction.Commit();
                            designerTransaction = null;
                        }
                    }
                }
                //Reset the DummyItem flag
                dummyItemAdded = false;
            }
            else
            {
                // Refresh on SelectionManager... To Change Glyph Size.
                if (dummyItemAdded)
                {
                    dummyItemAdded = false;
                    RemoveItem();

                    if (designer.NewItemTransaction != null)
                    {
                        designer.NewItemTransaction.Cancel();
                        designer.NewItemTransaction = null;
                    }
                }
            }
            immediateParent.ResumeLayout();
            if (newItem != null && !newItem.IsOnDropDown)
            {
                if (newItem is ToolStripDropDownItem dropDown)
                {
                    ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)designerHost.GetDesigner(newItem);
                    Rectangle itemBounds = itemDesigner.GetGlyphBounds();
                    if (designerHost.RootComponent is Control parent)
                    {
                        if (bSvc != null)
                        {
                            Rectangle parentBounds = bSvc.ControlRectInAdornerWindow(parent);
                            if (!ToolStripDesigner.IsGlyphTotallyVisible(itemBounds, parentBounds))
                            {
                                dropDown.HideDropDown();
                            }
                        }
                    }
                }
            }

            // used the SelectionManager to Add the glyphs.
            if (selMgr != null)
            {
                selMgr.Refresh();
            }
        }

        /// <summary>
        ///  Disposes of this designer.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //clean up
                if (_editorNode != null)
                {
                    _editorNode.CloseEditor();
                    _editorNode = null;
                }

                if (ToolStripItem != null)
                {
                    ToolStripItem.Paint -= new System.Windows.Forms.PaintEventHandler(OnItemPaint);
                }
                // Now, unhook the component rename event
                IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (cs != null)
                {
                    cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                }

                //clean up
                if (selSvc != null)
                {
                    selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
                }
                //clean up the ToolStripItem Glyph if Any
                if (bodyGlyph != null)
                {
                    ToolStripAdornerWindowService toolStripAdornerWindowService = (ToolStripAdornerWindowService)GetService(typeof(ToolStripAdornerWindowService));
                    if (toolStripAdornerWindowService != null && toolStripAdornerWindowService.DropDownAdorner.Glyphs.Contains(bodyGlyph))
                    {
                        toolStripAdornerWindowService.DropDownAdorner.Glyphs.Remove(bodyGlyph);
                    }
                }
                // Remove the Collection
                if (toolStripItemCustomMenuItemCollection != null && toolStripItemCustomMenuItemCollection.Count > 0)
                {
                    foreach (ToolStripItem item in toolStripItemCustomMenuItemCollection)
                    {
                        item.Dispose();
                    }
                    toolStripItemCustomMenuItemCollection.Clear();
                }
                toolStripItemCustomMenuItemCollection = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Returns the owner of the current ToolStripItem.
        /// </summary>
        protected virtual Component GetOwnerForActionList() => (ToolStripItem.Placement == ToolStripItemPlacement.Main) ? ToolStripItem.GetCurrentParent() : ToolStripItem.Owner;

        internal virtual ToolStrip GetMainToolStrip() => ToolStripItem.Owner;

        public Rectangle GetGlyphBounds()
        {
            BehaviorService b = (BehaviorService)GetService(typeof(BehaviorService));
            Rectangle r = Rectangle.Empty;
            if (b != null && ImmediateParent != null)
            {
                Point loc = b.ControlToAdornerWindow((Control)ImmediateParent);
                r = ToolStripItem.Bounds;
                r.Offset(loc);
            }
            return r;
        }

        // Need to Fire ComponentChanging on all the DropDownItems. Please see "MorphToolStripItem" function for more details.
        private void FireComponentChanging(ToolStripDropDownItem parent)
        {
            if (parent != null)
            {
                IComponentChangeService changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (changeSvc != null && parent.Site != null)
                {
                    changeSvc.OnComponentChanging(parent, TypeDescriptor.GetProperties(parent)["DropDownItems"]);
                }
                foreach (ToolStripItem item in parent.DropDownItems)
                {
                    //Dont Serialize the DesignerToolStripControlHost...
                    if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 1 /*including TN*/)
                    {
                        FireComponentChanging(dropDownItem);
                    }
                }
            }
        }

        private void FireComponentChanged(ToolStripDropDownItem parent)
        {
            if (parent != null)
            {
                IComponentChangeService changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (changeSvc != null && parent.Site != null)
                {
                    changeSvc.OnComponentChanged(parent, TypeDescriptor.GetProperties(parent)["DropDownItems"], null, null);
                }

                foreach (ToolStripItem item in parent.DropDownItems)
                {
                    //Dont Serialize the DesignerToolStripControlHost...
                    if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 1 /*including TN*/)
                    {
                        FireComponentChanged(dropDownItem);
                    }
                }
            }
        }

        public void GetGlyphs(ref GlyphCollection glyphs, System.Windows.Forms.Design.Behavior.Behavior standardBehavior)
        {
            if (ImmediateParent != null)
            {
                Rectangle r = GetGlyphBounds();
                ToolStripDesignerUtils.GetAdjustedBounds(ToolStripItem, ref r);
                BehaviorService b = (BehaviorService)GetService(typeof(BehaviorService));
                Rectangle parentBounds = b.ControlRectInAdornerWindow((Control)ImmediateParent);
                if (parentBounds.Contains(r.Left, r.Top))
                {
                    // Dont paint the glyphs if we are opening a DropDown...
                    if (ToolStripItem.IsOnDropDown)
                    {
                        ToolStrip parent = ToolStripItem.GetCurrentParent();
                        if (parent is null)
                        {
                            parent = ToolStripItem.Owner;
                        }
                        if (parent != null && parent.Visible)
                        {
                            glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Top, standardBehavior, true));
                            glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Bottom, standardBehavior, true));
                            glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Left, standardBehavior, true));
                            glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Right, standardBehavior, true));
                        }
                    }
                    else
                    {
                        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Top, standardBehavior, true));
                        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Bottom, standardBehavior, true));
                        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Left, standardBehavior, true));
                        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Right, standardBehavior, true));
                    }
                }
            }
        }

        /// <summary>
        ///  Returns the root dropdown in the chain.
        /// </summary>
        internal ToolStripDropDown GetFirstDropDown(ToolStripItem currentItem)
        {
            if (currentItem.Owner is ToolStripDropDown)
            {
                ToolStripDropDown topmost = currentItem.Owner as ToolStripDropDown;
                // walk back up the chain of windows to get the topmost
                while (topmost.OwnerItem != null && (topmost.OwnerItem.Owner is ToolStripDropDown))
                {
                    topmost = topmost.OwnerItem.Owner as ToolStripDropDown;
                }
                return topmost;
            }
            return null;
        }

        /// <summary>
        ///  This helper function resets the AutoSize property so that the item SNAPS back to its "preferredSize".
        /// </summary>
        private void HideDummyNode()
        {
            ToolStripItem.AutoSize = AutoSize;
            if (_editorNode != null)
            {
                _editorNode.CloseEditor();
                _editorNode = null;
            }
        }

        /// <summary>
        ///  Get the designer set up to run.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            //Shadow AutoSize
            AutoSize = ToolStripItem.AutoSize;
            Visible = true;
            currentVisible = Visible;
            //Shadow the AccessibleName as we are going to change it at DesignTime
            AccessibleName = ToolStripItem.AccessibleName;
            ToolStripItem.Paint += new System.Windows.Forms.PaintEventHandler(OnItemPaint);
            //Change the AccessibleName to point to ToolStirpItem.Name
            ToolStripItem.AccessibleName = ToolStripItem.Name;
            // Now, hook the component rename event so we can update the AccessibleName
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (cs != null)
            {
                cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
            }

            //hook our SelectionService.
            selSvc = (ISelectionService)GetService(typeof(ISelectionService));
            if (selSvc != null)
            {
                selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
            }
        }

        /// <summary>
        ///  Overriden to always Initialise the ToolStripItem with Text property.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            //Set the Text only if the item is not created internally (via InSitu Edit)
            if (!internalCreate)
            {
                ISite site = Component.Site;
                if (site != null && Component is ToolStripDropDownItem)
                {
                    if (defaultValues is null)
                    {
                        defaultValues = new Hashtable();
                    }

                    defaultValues["Text"] = site.Name;
                    IComponent component = Component;
                    PropertyDescriptor pd = TypeDescriptor.GetProperties(ToolStripItem)["Text"];

                    if (pd != null && pd.PropertyType.Equals(typeof(string)))
                    {
                        string current = (string)pd.GetValue(component);
                        if (current is null || current.Length == 0)
                        {
                            pd.SetValue(component, site.Name);
                        }
                    }
                }
            }
            base.InitializeNewComponent(defaultValues);
            // ComboBoxes and TextBoxes shouldnt have Texts... In TextBoxBaseDesigner we do similar thing where we call the base (which sets the text) and then reset it back
            if (Component is ToolStripTextBox || Component is ToolStripComboBox)
            {
                PropertyDescriptor textProp = TypeDescriptor.GetProperties(Component)["Text"];
                if (textProp != null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
                {
                    textProp.SetValue(Component, "");
                }
            }
        }

        /// <summary>
        ///  This will morph the current item to the provided type "t" of the item...
        /// </summary>
        internal virtual ToolStripItem MorphCurrentItem(Type t)
        {
            ToolStripItem newItem = null;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host is null)
            {
                Debug.Fail("Couldn't get designer host!");
                return newItem;
            }

            //create our transaction
            DesignerTransaction designerTransaction = host.CreateTransaction(SR.ToolStripMorphingItemTransaction);
            ToolStrip parent = (ToolStrip)ImmediateParent;
            // Sepcial case overflow...
            if (parent is ToolStripOverflow)
            {
                parent = ToolStripItem.Owner;
            }
            ToolStripMenuItemDesigner ownerItemDesigner = null;

            int dummyIndex = parent.Items.IndexOf(ToolStripItem);
            string name = ToolStripItem.Name;
            ToolStripItem ownerItem = null;

            // Get the main ToolStrip to Set the Glyph for the new Item once it is MORPHED.
            if (ToolStripItem.IsOnDropDown)
            {
                if (ImmediateParent is ToolStripDropDown parentDropDown)
                {
                    ownerItem = parentDropDown.OwnerItem;
                    if (ownerItem != null)
                    {
                        ownerItemDesigner = (ToolStripMenuItemDesigner)host.GetDesigner(ownerItem);
                    }
                }
            }

            try
            {
                //turn off Adding and Added Transactions..
                ToolStripDesigner.s_autoAddNewItems = false;
                SerializationStore _serializedData = null;
                if (GetService(typeof(ComponentSerializationService)) is ComponentSerializationService _serializationService)
                {
                    _serializedData = _serializationService.CreateStore();
                    _serializationService.Serialize(_serializedData, Component); //notice the use of component... since we want to preserve the type.

                    //Serialize all the DropDownItems for this Item....
                    SerializationStore _serializedDataForDropDownItems = null;
                    ToolStripDropDownItem dropDownItem = ToolStripItem as ToolStripDropDownItem;
                    if (dropDownItem != null && typeof(ToolStripDropDownItem).IsAssignableFrom(t))
                    {
                        // Hide the DropDown.
                        dropDownItem.HideDropDown();
                        _serializedDataForDropDownItems = _serializationService.CreateStore();
                        SerializeDropDownItems(dropDownItem, ref _serializedDataForDropDownItems, _serializationService);
                        //close the SerializationStore to Serialize Items..
                        _serializedDataForDropDownItems.Close();
                    }

                    //close the SerializationStore to Serialize the ToolStripItem
                    _serializedData.Close();
                    //Remove the currentItem that is getting morphed..
                    IComponentChangeService changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                    if (changeSvc != null)
                    {
                        if (parent.Site != null)
                        {
                            changeSvc.OnComponentChanging(parent, TypeDescriptor.GetProperties(parent)["Items"]);
                        }
                        else if (ownerItem != null)
                        {
                            changeSvc.OnComponentChanging(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                            changeSvc.OnComponentChanged(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"], null, null);
                        }
                    }

                    FireComponentChanging(dropDownItem);
                    parent.Items.Remove(ToolStripItem);
                    host.DestroyComponent(ToolStripItem);
                    //Create our new Item
                    ToolStripItem component = (ToolStripItem)host.CreateComponent(t, name);
                    //Since destroying the original item took away its DropDownItems. We need to Deserialize the items again...
                    if (component is ToolStripDropDownItem)
                    {
                        if (_serializedDataForDropDownItems != null)
                        {
                            _serializationService.Deserialize(_serializedDataForDropDownItems);
                        }
                    }

                    //Now deserialize the newItem to morph to the old item...
                    _serializationService.DeserializeTo(_serializedData, host.Container, false, true);
                    // Add the new Item...
                    newItem = (ToolStripItem)host.Container.Components[name];
                    //Set the Image property and DisplayStyle...
                    if (newItem.Image is null && newItem is ToolStripButton)
                    {
                        Image image = null;
                        try
                        {
                            image = new Icon(typeof(ToolStripButton), "blank").ToBitmap();
                        }
                        catch (Exception ex)
                        {
                            if (ClientUtils.IsCriticalException(ex))
                            {
                                throw;
                            }
                        }

                        PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(newItem)["Image"];
                        Debug.Assert(imageProperty != null, "Could not find 'Image' property in ToolStripItem.");
                        if (imageProperty != null && image != null)
                        {
                            imageProperty.SetValue(newItem, image);
                        }

                        PropertyDescriptor dispProperty = TypeDescriptor.GetProperties(newItem)["DisplayStyle"];
                        Debug.Assert(dispProperty != null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                        if (dispProperty != null)
                        {
                            dispProperty.SetValue(newItem, ToolStripItemDisplayStyle.Image);
                        }

                        PropertyDescriptor imageTransProperty = TypeDescriptor.GetProperties(newItem)["ImageTransparentColor"];
                        Debug.Assert(imageTransProperty != null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                        if (imageTransProperty != null)
                        {
                            imageTransProperty.SetValue(newItem, Color.Magenta);
                        }
                    }

                    parent.Items.Insert(dummyIndex, newItem);
                    if (changeSvc != null)
                    {
                        if (parent.Site != null)
                        {
                            changeSvc.OnComponentChanged(parent, TypeDescriptor.GetProperties(parent)["Items"], null, null);
                        }
                        else if (ownerItem != null)
                        {
                            changeSvc.OnComponentChanging(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                            changeSvc.OnComponentChanged(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"], null, null);
                        }
                    }

                    FireComponentChanged(dropDownItem);
                    // Add the Glyph for the DropDown ... We are responsible for the Glyh Addition since BodyGlyphs for DropDownItems are added by us.
                    if (newItem.IsOnDropDown && ownerItemDesigner != null)
                    {
                        ownerItemDesigner.RemoveItemBodyGlyph(newItem);
                        ownerItemDesigner.AddItemBodyGlyph(newItem);
                    }
                    // re start the ComponentAdding/Added events
                    ToolStripDesigner.s_autoAddNewItems = true;
                    //Invalidate the AdornerWindow to refresh selectionglyphs.
                    if (newItem != null)
                    {
                        if (newItem is ToolStripSeparator)
                        {
                            parent.PerformLayout();
                        }
                        BehaviorService windowService = (BehaviorService)newItem.Site.GetService(typeof(BehaviorService));
                        if (windowService != null)
                        {
                            windowService.Invalidate();
                        }

                        // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionServive from new Component
                        ISelectionService selSvc = (ISelectionService)newItem.Site.GetService(typeof(ISelectionService));
                        if (selSvc != null)
                        {
                            selSvc.SetSelectedComponents(new object[] { newItem }, SelectionTypes.Replace);
                        }
                    }
                }
            }
            catch
            {
                host.Container.Add(ToolStripItem);
                parent.Items.Insert(dummyIndex, ToolStripItem);
                if (designerTransaction != null)
                {
                    designerTransaction.Cancel();
                    designerTransaction = null;
                }
            }
            finally
            {
                if (designerTransaction != null)
                {
                    designerTransaction.Commit();
                    designerTransaction = null;
                }
            }
            return newItem;
        }

        /// <summary>
        ///  Raised when a component's name changes.  Here we update the AccessibleName Property to match the newName.
        /// </summary>
        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            if (e.Component == ToolStripItem)
            {
                ToolStripItem.AccessibleName = e.NewName;
            }
        }

        /// <summary>
        ///  This can be used for OVERFLOW !!!
        /// </summary>
        private void OnItemPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (ToolStripItem.GetCurrentParent() is ToolStripDropDown dropDown)
            {
                if (selSvc != null)
                {
                    if (!IsEditorActive && ToolStripItem.Equals(selSvc.PrimarySelection))
                    {
                        BehaviorService behaviorService = (BehaviorService)GetService(typeof(BehaviorService));

                        if (behaviorService != null)
                        {
                            Point loc = behaviorService.ControlToAdornerWindow((Control)ImmediateParent);
                            Rectangle r = ToolStripItem.Bounds;
                            r.Offset(loc);
                            r.Inflate(GLYPHINSET, GLYPHINSET);
                            //this will allow any Glyphs to re-paint
                            //after this control and its designer has painted
                            behaviorService.ProcessPaintMessage(r);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  For ToolStripItems that are not MenuItems and are on Dropdown we need ot update Selection Rect.
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!(sender is ISelectionService sSvc))
            {
                return;
            }
            //determine if we are selected
            ToolStripItem currentSelection = sSvc.PrimarySelection as ToolStripItem;
            // Accessibility information
            if (ToolStripItem.AccessibilityObject is ToolStripItem.ToolStripItemAccessibleObject acc)
            {
                acc.AddState(AccessibleStates.None);
                ToolStrip tool = GetMainToolStrip();
                if (sSvc.GetComponentSelected(ToolStripItem))
                {
                    ToolStrip owner = ImmediateParent as ToolStrip;
                    int focusIndex = 0;
                    if (owner != null)
                    {
                        focusIndex = owner.Items.IndexOf(currentSelection);
                    }
                    acc.AddState(AccessibleStates.Selected);
                    if (tool != null)
                    {
                        Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "MSAA: SelectionAdd, tool = " + tool.ToString());
                        User32.NotifyWinEvent((uint)AccessibleEvents.SelectionAdd, new HandleRef(owner, owner.Handle), User32.OBJID.CLIENT, focusIndex + 1);
                    }
                    if (currentSelection == ToolStripItem)
                    {
                        acc.AddState(AccessibleStates.Focused);
                        if (tool != null)
                        {
                            User32.NotifyWinEvent((uint)AccessibleEvents.Focus, new HandleRef(owner, owner.Handle), User32.OBJID.CLIENT, focusIndex + 1);
                        }
                    }
                }
            }

            if (currentSelection != null && currentSelection.Equals(ToolStripItem) && !(ToolStripItem is ToolStripMenuItem))
            {
                if (currentSelection.IsOnDropDown)
                {
                    //If the Item is on DropDown ... Show its DropDown and all PArent Dropdown if not visible..
                    IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
                    if (designerHost != null)
                    {
                        if (currentSelection.Owner is ToolStripDropDown parentDropDown)
                        {
                            bool needRefresh = false;
                            if (parentDropDown.OwnerItem is ToolStripDropDownItem parentItem)
                            {
                                ToolStripMenuItemDesigner parentItemDesigner = (ToolStripMenuItemDesigner)designerHost.GetDesigner(parentItem);
                                if (parentItemDesigner != null)
                                {
                                    parentItemDesigner.InitializeDropDown();
                                }
                                needRefresh = true;
                            }
                            else if (parentDropDown is ContextMenuStrip)
                            {
                                // For ContextMenuStrip, we need use different ways to show the menu.
                                ToolStripDropDownDesigner parentDropDownDesigner = (ToolStripDropDownDesigner)designerHost.GetDesigner(parentDropDown);
                                if (parentDropDownDesigner != null)
                                {
                                    parentDropDownDesigner.ShowMenu(currentSelection);
                                }
                                needRefresh = true;
                            }

                            if (needRefresh)
                            {
                                // Refresh on SelectionManager... To Change Glyph Size.
                                SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                                // used the cached value...
                                if (selMgr != null)
                                {
                                    selMgr.Refresh();
                                }
                                // Invalidate the dropdown area. This is necessary when a different item is selected in the same dropdown.
                                BehaviorService behaviorService = (BehaviorService)GetService(typeof(BehaviorService));
                                if (behaviorService != null)
                                {
                                    behaviorService.Invalidate(parentDropDown.Bounds);
                                }
                            }
                        }
                    }
                }
                else if (currentSelection.Owner != null)
                {
                    // The selected item could be in a MenuStrip, StatusStrip or ToolStrip. Need invalidate the BehaviorService to reflect the selection change.
                    BehaviorService behaviorService = (BehaviorService)GetService(typeof(BehaviorService));
                    if (behaviorService != null)
                    {
                        behaviorService.Invalidate(behaviorService.ControlRectInAdornerWindow(currentSelection.Owner));
                    }
                }
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties the component it is designing will expose through the TypeDescriptor object.  This method is called immediately before its corresponding "Post" method. If you are overriding this method you should call the base implementation before you perform your own filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            // Handle shadowed properties
            string[] shadowProps = new string[] { "AutoSize", "AccessibleName", "Visible", "Overflow" };

            PropertyDescriptor prop;
            Attribute[] empty = Array.Empty<Attribute>();
            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ToolStripItemDesigner), prop, empty);
                }
            }
        }

        // CALLED ONLY IF THE EDIT ACTION WAS ROLLBACKED!!!
        public void RemoveItem()
        {
            dummyItemAdded = false;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host is null)
            {
                Debug.Fail("Couldn't get designer host!");
                return;
            }
            //Remove the dummy Item since the Edit was CANCELLED..
            ToolStrip parent = (ToolStrip)ImmediateParent;
            if (parent is ToolStripOverflow)
            {
                parent = ParentComponent as ToolStrip;
            }
            parent.Items.Remove(ToolStripItem);
            host.DestroyComponent(ToolStripItem);
        }

        /// <summary>
        /// Resets the ToolStripItemAutoSize to be the default autosize
        /// </summary>
        private void ResetAutoSize() => ShadowProperties[nameof(AutoSize)] = false;

        /// <summary>
        ///      Restores the AutoSize to be the value set in the property grid.
        /// </summary>
        private void RestoreAutoSize() => ToolStripItem.AutoSize = (bool)ShadowProperties[nameof(AutoSize)];

        /// <summary>
        /// Resets the ToolStrip Visible to be the default value
        /// </summary>
        private void ResetVisible() => Visible = true;

        /// <summary>
        ///  Restore Overflow
        /// </summary>
        private void RestoreOverflow() => ToolStripItem.Overflow = (ToolStripItemOverflow)ShadowProperties[nameof(Overflow)];

        /// <summary>
        ///  Resets Overflow
        /// </summary>
        private void ResetOverflow() => ToolStripItem.Overflow = ToolStripItemOverflow.AsNeeded;

        /// <summary>
        /// Resets the ToolStripItem AccessibleName to the default
        /// </summary>
        private void ResetAccessibleName() => ShadowProperties[nameof(AccessibleName)] = null;

        /// <summary>
        ///      Restores the AutoSize to be the value set in the property grid.
        /// </summary>
        private void RestoreAccessibleName() => ToolStripItem.AccessibleName = (string)ShadowProperties[nameof(AccessibleName)];

        // internal method called to select the next item from the current item.
        internal void SelectNextItem(ISelectionService service, bool enterKeyPressed, ToolStripDesigner designer)
        {
            if (ToolStripItem is ToolStripDropDownItem dropDownItem)
            {
                SetSelection(enterKeyPressed);
            }
            else
            //We are here for simple ToolStripItems...
            {
                ToolStrip parent = (ToolStrip)ImmediateParent;
                if (parent is ToolStripOverflow)
                {
                    parent = ToolStripItem.Owner;
                }
                int currentIndex = parent.Items.IndexOf(ToolStripItem);
                ToolStripItem nextItem = parent.Items[currentIndex + 1];
                // Set the Selection to the NEXT ITEM in the TOOLSTRIP...
                ToolStripKeyboardHandlingService keyboardHandlingService = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
                if (keyboardHandlingService != null)
                {
                    if (nextItem == designer.EditorNode)
                    {
                        keyboardHandlingService.SelectedDesignerControl = nextItem;
                        selSvc.SetSelectedComponents(null, SelectionTypes.Replace);
                    }
                    else
                    {
                        keyboardHandlingService.SelectedDesignerControl = null;
                        selSvc.SetSelectedComponents(new object[] { nextItem });
                    }
                }
            }
        }

        // Recursive function to add all the menuItems to the SerializationStore during Morphing..
        private void SerializeDropDownItems(ToolStripDropDownItem parent, ref SerializationStore _serializedDataForDropDownItems, ComponentSerializationService _serializationService)
        {
            foreach (ToolStripItem item in parent.DropDownItems)
            {
                //Dont Serialize the DesignerToolStripControlHost...
                if (!(item is DesignerToolStripControlHost))
                {
                    _serializationService.Serialize(_serializedDataForDropDownItems, item);
                    if (item is ToolStripDropDownItem dropDownItem)
                    {
                        SerializeDropDownItems(dropDownItem, ref _serializedDataForDropDownItems, _serializationService);
                    }
                }
            }
        }

        // Sets the Item visibility to honor WYSIWYG
        internal void SetItemVisible(bool toolStripSelected, ToolStripDesigner designer)
        {
            if (toolStripSelected)
            {
                // Set the Visiblity if different.
                if (!currentVisible)
                {
                    ToolStripItem.Visible = true;
                    if (designer != null && !designer.FireSyncSelection)
                    {
                        designer.FireSyncSelection = true;
                    }
                }
            }
            else
            {
                if (!currentVisible)
                {
                    ToolStripItem.Visible = currentVisible;
                }
            }
        }

        private bool ShouldSerializeVisible() => !Visible;

        /// <summary>
        /// Since we're shadowing autosize, we get called here to determine whether or not to serialize
        /// </summary>
        private bool ShouldSerializeAutoSize() => (ShadowProperties.Contains(nameof(AutoSize)));

        /// <summary>
        /// Since we're shadowing autosize, we get called here to determine whether or not to serialize
        /// </summary>
        private bool ShouldSerializeAccessibleName() => (ShadowProperties[nameof(AccessibleName)] != null);

        /// <summary>
        /// Since we're Overflow Size, we get called here to determine whether or not to serialize
        /// </summary>
        private bool ShouldSerializeOverflow() => (ShadowProperties[nameof(Overflow)] != null);

        /// <summary>
        ///  This Function is called thru the ToolStripEditorManager which is listening for the  F2 command.
        /// </summary>
        internal virtual void ShowEditNode(bool clicked)
        {
            // ACTIVATION ONLY FOR TOOLSTRIPMENUITEMS
            if (ToolStripItem is ToolStripMenuItem)
            {
                if (_editorNode is null)
                {
                    CreateDummyNode();
                }

                IDesignerHost designerHost = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
                ToolStrip parent = ImmediateParent as ToolStrip;
                Debug.Assert(parent != null, "ImmediateParent is null for the current ToolStripItem !!");
                if (parent != null)
                {
                    ToolStripDesigner parentDesigner = (ToolStripDesigner)designerHost.GetDesigner(parent);
                    BehaviorService b = (BehaviorService)GetService(typeof(BehaviorService));
                    Point loc = b.ControlToAdornerWindow(parent);

                    //Get the original ToolStripItem bounds.
                    Rectangle origBoundsInAdornerWindow = ToolStripItem.Bounds;
                    origBoundsInAdornerWindow.Offset(loc);
                    ToolStripItem.AutoSize = false;
                    _editorNode.SetWidth(ToolStripItem.Text);
                    if (parent.Orientation == Orientation.Horizontal)
                    {
                        ToolStripItem.Width = _editorNode.EditorToolStrip.Width + 2;
                    }
                    else
                    {
                        ToolStripItem.Height = _editorNode.EditorToolStrip.Height;
                    }
                    // Refresh the glyphs.
                    if (!dummyItemAdded)
                    {
                        b.SyncSelection();
                    }

                    if (ToolStripItem.Placement != ToolStripItemPlacement.None)
                    {
                        Rectangle boundsInAdornerWindow = ToolStripItem.Bounds;
                        boundsInAdornerWindow.Offset(loc);

                        //Center it in verticaldirection.
                        if (parent.Orientation == Orientation.Horizontal)
                        {
                            boundsInAdornerWindow.X++;
                            boundsInAdornerWindow.Y += (ToolStripItem.Height - _editorNode.EditorToolStrip.Height) / 2;
                            boundsInAdornerWindow.Y++;
                        }
                        else
                        {
                            boundsInAdornerWindow.X += (ToolStripItem.Width - _editorNode.EditorToolStrip.Width) / 2;
                            boundsInAdornerWindow.X++;
                        }
                        _editorNode.Bounds = boundsInAdornerWindow;
                        //Invalidate the union of the original bounds and the new bounds.
                        boundsInAdornerWindow = Rectangle.Union(origBoundsInAdornerWindow, boundsInAdornerWindow);
                        b.Invalidate(boundsInAdornerWindow);
                        // PLEASE DONT CHANGE THIS ORDER !!!
                        if (parentDesigner != null && parentDesigner.EditManager != null)
                        {
                            parentDesigner.EditManager.ActivateEditor(ToolStripItem, clicked);
                        }
                        SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                        if (bodyGlyph != null)
                        {
                            selMgr.BodyGlyphAdorner.Glyphs.Remove(bodyGlyph);
                        }
                    }
                    else
                    {
                        ToolStripItem.AutoSize = AutoSize;
                        if (ToolStripItem is ToolStripDropDownItem) //We have no place to show this item... so Hide the DropDown
                        {
                            if (ToolStripItem is ToolStripDropDownItem ddItem)
                            {
                                ddItem.HideDropDown();
                            }
                            // And select the parent... since we cannot show the current selection.
                            selSvc.SetSelectedComponents(new object[] { ImmediateParent });
                        }
                    }
                }
            }
        }

        // This method is called by the ToolStripDesigner to SetSelections to proper ToolStripItems  after the parent ToolStripItem is committed. Consider this : the ToolStrip would cause the NEXT item on the TOPLEVEL to get selected... while on MenuStrip.. we would want the Child ToolStripItem in the DropDown to get  selected after the TopLevel MenuStripItem is commited.
        internal virtual bool SetSelection(bool enterKeyPressed) => false;

        internal override void ShowContextMenu(int x, int y)
        {
            ToolStripKeyboardHandlingService keySvc = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
            if (keySvc != null)
            {
                if (!keySvc.ContextMenuShownByKeyBoard)
                {
                    BehaviorService b = (BehaviorService)GetService(typeof(BehaviorService));
                    Point newPoint = Point.Empty;
                    if (b != null)
                    {
                        newPoint = b.ScreenToAdornerWindow(new Point(x, y));
                    }
                    Rectangle itemBounds = GetGlyphBounds();
                    if (itemBounds.Contains(newPoint))
                    {
                        DesignerContextMenu.Show(x, y);
                    }
                }
                else
                {
                    keySvc.ContextMenuShownByKeyBoard = false;
                    DesignerContextMenu.Show(x, y);
                }
            }
        }
    }
}
