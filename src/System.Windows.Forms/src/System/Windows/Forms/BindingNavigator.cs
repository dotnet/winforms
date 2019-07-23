// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(BindingSource)),
    DefaultEvent(nameof(RefreshItems)),
    Designer("System.Windows.Forms.Design.BindingNavigatorDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionBindingNavigator))
    ]
    public class BindingNavigator : ToolStrip, ISupportInitialize
    {
        private BindingSource bindingSource;

        private ToolStripItem moveFirstItem;
        private ToolStripItem movePreviousItem;
        private ToolStripItem moveNextItem;
        private ToolStripItem moveLastItem;
        private ToolStripItem addNewItem;
        private ToolStripItem deleteItem;
        private ToolStripItem positionItem;
        private ToolStripItem countItem;

        private string countItemFormat = SR.BindingNavigatorCountItemFormat;

        private EventHandler onRefreshItems = null;

        private bool initializing = false;

        private bool addNewItemUserEnabled = true;
        private bool deleteItemUserEnabled = true;

        /// <summary>
        ///  Creates an empty BindingNavigator tool strip.
        ///  Call AddStandardItems() to add standard tool strip items.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BindingNavigator() : this(false)
        {
        }

        /// <summary>
        ///  Creates a BindingNavigator strip containing standard items, bound to the specified BindingSource.
        /// </summary>
        public BindingNavigator(BindingSource bindingSource) : this(true)
        {
            BindingSource = bindingSource;
        }

        /// <summary>
        ///  Creates an empty BindingNavigator tool strip, and adds the strip to the specified container.
        ///  Call AddStandardItems() to add standard tool strip items.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BindingNavigator(IContainer container) : this(false)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        /// <summary>
        ///  Creates a BindingNavigator strip, optionally containing a set of standard tool strip items.
        /// </summary>
        public BindingNavigator(bool addStandardItems)
        {
            if (addStandardItems)
            {
                AddStandardItems();
            }
        }

        /// <summary>
        ///  ISupportInitialize support. Disables updates to tool strip items during initialization.
        /// </summary>
        public void BeginInit()
        {
            initializing = true;
        }

        /// <summary>
        ///  ISupportInitialize support. Enables updates to tool strip items after initialization.
        /// </summary>
        public void EndInit()
        {
            initializing = false;
            RefreshItemsInternal();
        }

        /// <summary>
        ///  Unhooks the BindingNavigator from the BindingSource.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BindingSource = null; // ...unwires from events of any prior BindingSource
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Adds standard set of tool strip items to a BindingNavigator tool strip, for basic
        ///  navigation operations such as Move First, Move Next, Add New, etc.
        ///
        ///  This method is called by the Windows Form Designer when a new BindingNavigator is
        ///  added to a Form. When creating a BindingNavigator programmatically, this method
        ///  must be called explicitly.
        ///
        ///  Override this method in derived classes to define additional or alternative standard items.
        ///  To ensure optimal design-time support for your derived class, make sure each item has a
        ///  meaningful value in its Name property. At design time, this will be used to generate a unique
        ///  name for the corresponding member variable. The item's Name property will then be updated
        ///  to match the name given to the member variable.
        ///
        ///  Note: This method does NOT remove any previous items from the strip, or suspend
        ///  layout while items are being added. Those are responsibilities of the caller.
        /// </summary>
        public virtual void AddStandardItems()
        {
            //
            // Create items
            //

            MoveFirstItem = new ToolStripButton();
            MovePreviousItem = new ToolStripButton();
            MoveNextItem = new ToolStripButton();
            MoveLastItem = new ToolStripButton();
            PositionItem = new ToolStripTextBox();
            CountItem = new ToolStripLabel();
            AddNewItem = new ToolStripButton();
            DeleteItem = new ToolStripButton();

            ToolStripSeparator separator1 = new ToolStripSeparator();
            ToolStripSeparator separator2 = new ToolStripSeparator();
            ToolStripSeparator separator3 = new ToolStripSeparator();

            //
            // Set up strings
            //

            // Default to lowercase for null name, because C# dev is more likely to create controls programmatically than
            // vb dev.
            char ch = string.IsNullOrEmpty(Name) || char.IsLower(Name[0]) ? 'b' : 'B';

            MoveFirstItem.Name = ch + "indingNavigatorMoveFirstItem";
            MovePreviousItem.Name = ch + "indingNavigatorMovePreviousItem";
            MoveNextItem.Name = ch + "indingNavigatorMoveNextItem";
            MoveLastItem.Name = ch + "indingNavigatorMoveLastItem";
            PositionItem.Name = ch + "indingNavigatorPositionItem";
            CountItem.Name = ch + "indingNavigatorCountItem";
            AddNewItem.Name = ch + "indingNavigatorAddNewItem";
            DeleteItem.Name = ch + "indingNavigatorDeleteItem";
            separator1.Name = ch + "indingNavigatorSeparator";
            separator2.Name = ch + "indingNavigatorSeparator";
            separator3.Name = ch + "indingNavigatorSeparator";

            MoveFirstItem.Text = SR.BindingNavigatorMoveFirstItemText;
            MovePreviousItem.Text = SR.BindingNavigatorMovePreviousItemText;
            MoveNextItem.Text = SR.BindingNavigatorMoveNextItemText;
            MoveLastItem.Text = SR.BindingNavigatorMoveLastItemText;
            AddNewItem.Text = SR.BindingNavigatorAddNewItemText;
            DeleteItem.Text = SR.BindingNavigatorDeleteItemText;

            CountItem.ToolTipText = SR.BindingNavigatorCountItemTip;
            PositionItem.ToolTipText = SR.BindingNavigatorPositionItemTip;
            CountItem.AutoToolTip = false;
            PositionItem.AutoToolTip = false;

            PositionItem.AccessibleName = SR.BindingNavigatorPositionAccessibleName;
            //
            // Set up images
            //

            Bitmap moveFirstImage = DpiHelper.GetBitmapFromIcon(typeof(BindingNavigator), "BindingNavigator.MoveFirst");
            Bitmap movePreviousImage = DpiHelper.GetBitmapFromIcon(typeof(BindingNavigator), "BindingNavigator.MovePrevious");
            Bitmap moveNextImage = DpiHelper.GetBitmapFromIcon(typeof(BindingNavigator), "BindingNavigator.MoveNext");
            Bitmap moveLastImage = DpiHelper.GetBitmapFromIcon(typeof(BindingNavigator), "BindingNavigator.MoveLast");
            Bitmap addNewImage = DpiHelper.GetBitmapFromIcon(typeof(BindingNavigator), "BindingNavigator.AddNew");
            Bitmap deleteImage = DpiHelper.GetBitmapFromIcon(typeof(BindingNavigator), "BindingNavigator.Delete");

            MoveFirstItem.Image = moveFirstImage;
            MovePreviousItem.Image = movePreviousImage;
            MoveNextItem.Image = moveNextImage;
            MoveLastItem.Image = moveLastImage;
            AddNewItem.Image = addNewImage;
            DeleteItem.Image = deleteImage;

            MoveFirstItem.RightToLeftAutoMirrorImage = true;
            MovePreviousItem.RightToLeftAutoMirrorImage = true;
            MoveNextItem.RightToLeftAutoMirrorImage = true;
            MoveLastItem.RightToLeftAutoMirrorImage = true;
            AddNewItem.RightToLeftAutoMirrorImage = true;
            DeleteItem.RightToLeftAutoMirrorImage = true;

            MoveFirstItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MovePreviousItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MoveNextItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MoveLastItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AddNewItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            DeleteItem.DisplayStyle = ToolStripItemDisplayStyle.Image;

            //
            // Set other random properties
            //
            PositionItem.AutoSize = false;
            PositionItem.Width = 50;

            //
            // Add items to strip
            //

            Items.AddRange(new ToolStripItem[] {
                                MoveFirstItem,
                                MovePreviousItem,
                                separator1,
                                PositionItem,
                                CountItem,
                                separator2,
                                MoveNextItem,
                                MoveLastItem,
                                separator3,
                                AddNewItem,
                                DeleteItem,
                                });
        }

        /// <summary>
        ///  The BindingSource who's list we are currently navigating, or null.
        /// </summary>
        [
        DefaultValue(null),
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingNavigatorBindingSourcePropDescr)),
        TypeConverter(typeof(ReferenceConverter)),
        ]
        public BindingSource BindingSource
        {
            get
            {
                return bindingSource;
            }

            set
            {
                WireUpBindingSource(ref bindingSource, value);
            }
        }

        /// <summary>
        ///  The ToolStripItem that triggers the 'Move first' action, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorMoveFirstItemPropDescr))
        ]
        public ToolStripItem MoveFirstItem
        {
            get
            {
                if (moveFirstItem != null && moveFirstItem.IsDisposed)
                {
                    moveFirstItem = null;
                }
                return moveFirstItem;
            }

            set
            {
                WireUpButton(ref moveFirstItem, value, new EventHandler(OnMoveFirst));
            }
        }

        /// <summary>
        ///  The ToolStripItem that triggers the 'Move previous' action, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorMovePreviousItemPropDescr))
        ]
        public ToolStripItem MovePreviousItem
        {
            get
            {

                if (movePreviousItem != null && movePreviousItem.IsDisposed)
                {
                    movePreviousItem = null;
                }

                return movePreviousItem;
            }

            set
            {
                WireUpButton(ref movePreviousItem, value, new EventHandler(OnMovePrevious));
            }
        }

        /// <summary>
        ///  The ToolStripItem that triggers the 'Move next' action, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorMoveNextItemPropDescr))
        ]
        public ToolStripItem MoveNextItem
        {
            get
            {
                if (moveNextItem != null && moveNextItem.IsDisposed)
                {
                    moveNextItem = null;
                }
                return moveNextItem;
            }

            set
            {
                WireUpButton(ref moveNextItem, value, new EventHandler(OnMoveNext));
            }
        }

        /// <summary>
        ///  The ToolStripItem that triggers the 'Move last' action, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorMoveLastItemPropDescr))
        ]
        public ToolStripItem MoveLastItem
        {
            get
            {
                if (moveLastItem != null && moveLastItem.IsDisposed)
                {
                    moveLastItem = null;
                }
                return moveLastItem;
            }

            set
            {
                WireUpButton(ref moveLastItem, value, new EventHandler(OnMoveLast));
            }
        }

        /// <summary>
        ///  The ToolStripItem that triggers the 'Add new' action, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorAddNewItemPropDescr))
        ]
        public ToolStripItem AddNewItem
        {
            get
            {
                if (addNewItem != null && addNewItem.IsDisposed)
                {
                    addNewItem = null;
                }
                return addNewItem;
            }

            set
            {
                if (addNewItem != value && value != null)
                {
                    value.InternalEnabledChanged += new EventHandler(OnAddNewItemEnabledChanged);
                    addNewItemUserEnabled = value.Enabled;
                }
                WireUpButton(ref addNewItem, value, new EventHandler(OnAddNew));
            }
        }

        /// <summary>
        ///  The ToolStripItem that triggers the 'Delete' action, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorDeleteItemPropDescr))
        ]
        public ToolStripItem DeleteItem
        {
            get
            {
                if (deleteItem != null && deleteItem.IsDisposed)
                {
                    deleteItem = null;
                }
                return deleteItem;
            }

            set
            {
                if (deleteItem != value && value != null)
                {
                    value.InternalEnabledChanged += new EventHandler(OnDeleteItemEnabledChanged);
                    deleteItemUserEnabled = value.Enabled;
                }
                WireUpButton(ref deleteItem, value, new EventHandler(OnDelete));

            }
        }

        /// <summary>
        ///  The ToolStripItem that displays the current position, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorPositionItemPropDescr))
        ]
        public ToolStripItem PositionItem
        {
            get
            {
                if (positionItem != null && positionItem.IsDisposed)
                {
                    positionItem = null;
                }
                return positionItem;
            }

            set
            {
                WireUpTextBox(ref positionItem, value, new KeyEventHandler(OnPositionKey), new EventHandler(OnPositionLostFocus));
            }
        }

        /// <summary>
        ///  The ToolStripItem that displays the total number of items, or null.
        /// </summary>
        [
        TypeConverter(typeof(ReferenceConverter)),
        SRCategory(nameof(SR.CatItems)),
        SRDescription(nameof(SR.BindingNavigatorCountItemPropDescr))
        ]
        public ToolStripItem CountItem
        {
            get
            {
                if (countItem != null && countItem.IsDisposed)
                {
                    countItem = null;
                }
                return countItem;
            }

            set
            {
                WireUpLabel(ref countItem, value);
            }
        }

        /// <summary>
        ///  Formatting to apply to count displayed in the CountItem tool strip item.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.BindingNavigatorCountItemFormatPropDescr))
        ]
        public string CountItemFormat
        {
            get
            {
                return countItemFormat;
            }

            set
            {
                if (countItemFormat != value)
                {
                    countItemFormat = value;
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Event raised when the state of the tool strip items needs to be
        ///  refreshed to reflect the current state of the data.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.BindingNavigatorRefreshItemsEventDescr))
        ]
        public event EventHandler RefreshItems
        {
            add => onRefreshItems += value;
            remove => onRefreshItems -= value;
        }

        /// <summary>
        ///  Refreshes the state of the standard items to reflect the current state of the data.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void RefreshItemsCore()
        {
            int count, position;
            bool allowNew, allowRemove;

            // Get state info from the binding source (if any)
            if (bindingSource == null)
            {
                count = 0;
                position = 0;
                allowNew = false;
                allowRemove = false;
            }
            else
            {
                count = bindingSource.Count;
                position = bindingSource.Position + 1;
                allowNew = (bindingSource as IBindingList).AllowNew;
                allowRemove = (bindingSource as IBindingList).AllowRemove;
            }

            // Enable or disable items (except when in design mode)
            if (!DesignMode)
            {
                if (MoveFirstItem != null)
                {
                    moveFirstItem.Enabled = (position > 1);
                }

                if (MovePreviousItem != null)
                {
                    movePreviousItem.Enabled = (position > 1);
                }

                if (MoveNextItem != null)
                {
                    moveNextItem.Enabled = (position < count);
                }

                if (MoveLastItem != null)
                {
                    moveLastItem.Enabled = (position < count);
                }

                if (AddNewItem != null)
                {
                    EventHandler handler = new EventHandler(OnAddNewItemEnabledChanged);
                    addNewItem.InternalEnabledChanged -= handler;
                    addNewItem.Enabled = (addNewItemUserEnabled && allowNew);
                    addNewItem.InternalEnabledChanged += handler;
                }

                if (DeleteItem != null)
                {
                    EventHandler handler = new EventHandler(OnDeleteItemEnabledChanged);
                    deleteItem.InternalEnabledChanged -= handler;
                    deleteItem.Enabled = (deleteItemUserEnabled && allowRemove && count > 0);
                    deleteItem.InternalEnabledChanged += handler;
                }

                if (PositionItem != null)
                {
                    positionItem.Enabled = (position > 0 && count > 0);
                }

                if (CountItem != null)
                {
                    countItem.Enabled = (count > 0);
                }
            }

            // Update current position indicator
            if (positionItem != null)
            {
                positionItem.Text = position.ToString(CultureInfo.CurrentCulture);
            }

            // Update record count indicator
            if (countItem != null)
            {
                countItem.Text = DesignMode ? CountItemFormat : string.Format(CultureInfo.CurrentCulture, CountItemFormat, count);
            }
        }

        /// <summary>
        ///  Called when the state of the tool strip items needs to be refreshed to reflect the current state of the data.
        ///  Calls <see cref='RefreshItemsCore'> to refresh the state of the standard items, then raises the RefreshItems event.
        /// </summary>
        protected virtual void OnRefreshItems()
        {
            // Refresh all the standard items
            RefreshItemsCore();

            // Raise the public event
            onRefreshItems?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Triggers form validation. Used by the BindingNavigator's standard items when clicked. If a validation error occurs
        ///  on the form, focus remains on the active control and the standard item does not perform its standard click action.
        ///  Custom items may also use this method to trigger form validation and check for success before performing an action.
        /// </summary>
        public bool Validate()
        {
            return ValidateActiveControl(out bool validatedControlAllowsFocusChange);
        }

        /// <summary>
        ///  Accept new row position entered into PositionItem.
        /// </summary>
        private void AcceptNewPosition()
        {
            // If no position item or binding source, do nothing
            if (positionItem == null || bindingSource == null)
            {
                return;
            }

            // Default to old position, in case new position turns out to be garbage
            int newPosition = bindingSource.Position;

            try
            {
                // Read new position from item text (and subtract one!)
                newPosition = Convert.ToInt32(positionItem.Text, CultureInfo.CurrentCulture) - 1;
            }
            catch (FormatException)
            {
                // Ignore bad user input
            }
            catch (OverflowException)
            {
                // Ignore bad user input
            }

            // If user has managed to enter a valid number, that is not the same as the current position, try
            // navigating to that position. Let the BindingSource validate the new position to keep it in range.
            if (newPosition != bindingSource.Position)
            {
                bindingSource.Position = newPosition;
            }

            // Update state of all items to reflect final position. If the user entered a bad position,
            // this will effectively reset the Position item back to showing the current position.
            RefreshItemsInternal();
        }

        /// <summary>
        ///  Cancel new row position entered into PositionItem.
        /// </summary>
        private void CancelNewPosition()
        {
            // Just refresh state of all items to reflect current position
            // (causing position item's new value to get blasted away)
            RefreshItemsInternal();
        }

        /// <summary>
        ///  Navigates to first item in BindingSource's list when the MoveFirstItem is clicked.
        /// </summary>
        private void OnMoveFirst(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (bindingSource != null)
                {
                    bindingSource.MoveFirst();
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Navigates to previous item in BindingSource's list when the MovePreviousItem is clicked.
        /// </summary>
        private void OnMovePrevious(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (bindingSource != null)
                {
                    bindingSource.MovePrevious();
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Navigates to next item in BindingSource's list when the MoveNextItem is clicked.
        /// </summary>
        private void OnMoveNext(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (bindingSource != null)
                {
                    bindingSource.MoveNext();
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Navigates to last item in BindingSource's list when the MoveLastItem is clicked.
        /// </summary>
        private void OnMoveLast(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (bindingSource != null)
                {
                    bindingSource.MoveLast();
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Adds new item to BindingSource's list when the AddNewItem is clicked.
        /// </summary>
        private void OnAddNew(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (bindingSource != null)
                {
                    bindingSource.AddNew();
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Deletes current item from BindingSource's list when the DeleteItem is clicked.
        /// </summary>
        private void OnDelete(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (bindingSource != null)
                {
                    bindingSource.RemoveCurrent();
                    RefreshItemsInternal();
                }
            }
        }

        /// <summary>
        ///  Navigates to specific item in BindingSource's list when a value is entered into the PositionItem.
        /// </summary>
        private void OnPositionKey(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    AcceptNewPosition();
                    break;
                case Keys.Escape:
                    CancelNewPosition();
                    break;
            }
        }

        /// <summary>
        ///  Navigates to specific item in BindingSource's list when a value is entered into the PositionItem.
        /// </summary>
        private void OnPositionLostFocus(object sender, EventArgs e)
        {
            AcceptNewPosition();
        }

        /// <summary>
        ///  Refresh tool strip items when something changes in the BindingSource.
        /// </summary>
        private void OnBindingSourceStateChanged(object sender, EventArgs e)
        {
            RefreshItemsInternal();
        }

        /// <summary>
        ///  Refresh tool strip items when something changes in the BindingSource's list.
        /// </summary>
        private void OnBindingSourceListChanged(object sender, ListChangedEventArgs e)
        {
            RefreshItemsInternal();
        }

        /// <summary>
        ///  Refresh the state of the items when the state of the data changes.
        /// </summary>
        private void RefreshItemsInternal()
        {
            // Block all updates during initialization
            if (initializing)
            {
                return;
            }

            // Call method that updates the items (overridable)
            OnRefreshItems();
        }

        private void ResetCountItemFormat()
        {
            countItemFormat = SR.BindingNavigatorCountItemFormat;
        }

        private bool ShouldSerializeCountItemFormat()
        {
            return countItemFormat != SR.BindingNavigatorCountItemFormat;
        }

        private void OnAddNewItemEnabledChanged(object sender, EventArgs e)
        {
            if (AddNewItem != null)
            {
                addNewItemUserEnabled = addNewItem.Enabled;
            }
        }

        private void OnDeleteItemEnabledChanged(object sender, EventArgs e)
        {
            if (DeleteItem != null)
            {
                deleteItemUserEnabled = deleteItem.Enabled;
            }
        }

        /// <summary>
        ///  Wire up some member variable to the specified button item, hooking events
        ///  on the new button and unhooking them from the previous button, if required.
        /// </summary>
        private void WireUpButton(ref ToolStripItem oldButton, ToolStripItem newButton, EventHandler clickHandler)
        {
            if (oldButton == newButton)
            {
                return;
            }

            if (oldButton != null)
            {
                oldButton.Click -= clickHandler;
            }

            if (newButton != null)
            {
                newButton.Click += clickHandler;
            }

            oldButton = newButton;
            RefreshItemsInternal();
        }

        /// <summary>
        ///  Wire up some member variable to the specified text box item, hooking events
        ///  on the new text box and unhooking them from the previous text box, if required.
        /// </summary>
        private void WireUpTextBox(ref ToolStripItem oldTextBox, ToolStripItem newTextBox, KeyEventHandler keyUpHandler, EventHandler lostFocusHandler)
        {
            if (oldTextBox != newTextBox)
            {
                if (oldTextBox is ToolStripControlHost oldCtrl)
                {
                    oldCtrl.KeyUp -= keyUpHandler;
                    oldCtrl.LostFocus -= lostFocusHandler;
                }

                if (newTextBox is ToolStripControlHost newCtrl)
                {
                    newCtrl.KeyUp += keyUpHandler;
                    newCtrl.LostFocus += lostFocusHandler;
                }

                oldTextBox = newTextBox;
                RefreshItemsInternal();
            }
        }

        /// <summary>
        ///  Wire up some member variable to the specified label item, hooking events
        ///  on the new label and unhooking them from the previous label, if required.
        /// </summary>
        private void WireUpLabel(ref ToolStripItem oldLabel, ToolStripItem newLabel)
        {
            if (oldLabel != newLabel)
            {
                oldLabel = newLabel;
                RefreshItemsInternal();
            }
        }

        /// <summary>
        ///  Wire up some member variable to the specified binding source, hooking events
        ///  on the new binding source and unhooking them from the previous one, if required.
        /// </summary>
        private void WireUpBindingSource(ref BindingSource oldBindingSource, BindingSource newBindingSource)
        {
            if (oldBindingSource != newBindingSource)
            {
                if (oldBindingSource != null)
                {
                    oldBindingSource.PositionChanged -= new EventHandler(OnBindingSourceStateChanged);
                    oldBindingSource.CurrentChanged -= new EventHandler(OnBindingSourceStateChanged);
                    oldBindingSource.CurrentItemChanged -= new EventHandler(OnBindingSourceStateChanged);
                    oldBindingSource.DataSourceChanged -= new EventHandler(OnBindingSourceStateChanged);
                    oldBindingSource.DataMemberChanged -= new EventHandler(OnBindingSourceStateChanged);
                    oldBindingSource.ListChanged -= new ListChangedEventHandler(OnBindingSourceListChanged);
                }

                if (newBindingSource != null)
                {
                    newBindingSource.PositionChanged += new EventHandler(OnBindingSourceStateChanged);
                    newBindingSource.CurrentChanged += new EventHandler(OnBindingSourceStateChanged);
                    newBindingSource.CurrentItemChanged += new EventHandler(OnBindingSourceStateChanged);
                    newBindingSource.DataSourceChanged += new EventHandler(OnBindingSourceStateChanged);
                    newBindingSource.DataMemberChanged += new EventHandler(OnBindingSourceStateChanged);
                    newBindingSource.ListChanged += new ListChangedEventHandler(OnBindingSourceListChanged);
                }

                oldBindingSource = newBindingSource;
                RefreshItemsInternal();
            }
        }

    }

}
