// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms;

[DefaultProperty(nameof(BindingSource))]
[DefaultEvent(nameof(RefreshItems))]
[Designer($"System.Windows.Forms.Design.BindingNavigatorDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionBindingNavigator))]
public class BindingNavigator : ToolStrip, ISupportInitialize
{
    private BindingSource? _bindingSource;

    private ToolStripItem? _moveFirstItem;
    private ToolStripItem? _movePreviousItem;
    private ToolStripItem? _moveNextItem;
    private ToolStripItem? _moveLastItem;
    private ToolStripItem? _addNewItem;
    private ToolStripItem? _deleteItem;
    private ToolStripItem? _positionItem;
    private ToolStripItem? _countItem;

    private string _countItemFormat = SR.BindingNavigatorCountItemFormat;

    private EventHandler? _onRefreshItems;

    private bool _initializing;

    private bool _addNewItemUserEnabled = true;
    private bool _deleteItemUserEnabled = true;

    /// <summary>
    ///  Creates an empty BindingNavigator tool strip.
    ///  Call AddStandardItems() to add standard tool strip items.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public BindingNavigator()
        : this(addStandardItems: false)
    {
    }

    /// <summary>
    ///  Creates a BindingNavigator strip containing standard items, bound to the specified BindingSource.
    /// </summary>
    public BindingNavigator(BindingSource? bindingSource)
        : this(addStandardItems: true)
    {
        BindingSource = bindingSource;
    }

    /// <summary>
    ///  Creates an empty BindingNavigator tool strip, and adds the strip to the specified container.
    ///  Call AddStandardItems() to add standard tool strip items.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public BindingNavigator(IContainer container)
        : this(addStandardItems: false)
    {
        ArgumentNullException.ThrowIfNull(container);

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
        _initializing = true;
    }

    /// <summary>
    ///  ISupportInitialize support. Enables updates to tool strip items after initialization.
    /// </summary>
    public void EndInit()
    {
        _initializing = false;
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

        ToolStripSeparator separator1 = new();
        ToolStripSeparator separator2 = new();
        ToolStripSeparator separator3 = new();

        //
        // Set up strings
        //

        // Default to lowercase for null name, because C# dev is more likely to create controls programmatically than
        // vb dev.
        char ch = string.IsNullOrEmpty(Name) || char.IsLower(Name[0]) ? 'b' : 'B';

        MoveFirstItem.Name = $"{ch}indingNavigatorMoveFirstItem";
        MovePreviousItem.Name = $"{ch}indingNavigatorMovePreviousItem";
        MoveNextItem.Name = $"{ch}indingNavigatorMoveNextItem";
        MoveLastItem.Name = $"{ch}indingNavigatorMoveLastItem";
        PositionItem.Name = $"{ch}indingNavigatorPositionItem";
        CountItem.Name = $"{ch}indingNavigatorCountItem";
        AddNewItem.Name = $"{ch}indingNavigatorAddNewItem";
        DeleteItem.Name = $"{ch}indingNavigatorDeleteItem";
        separator1.Name = $"{ch}indingNavigatorSeparator1";
        separator2.Name = $"{ch}indingNavigatorSeparator2";
        separator3.Name = $"{ch}indingNavigatorSeparator3";

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

        // Set up images

        Bitmap moveFirstImage = ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(BindingNavigator), "BindingNavigator.MoveFirst");
        Bitmap movePreviousImage = ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(BindingNavigator), "BindingNavigator.MovePrevious");
        Bitmap moveNextImage = ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(BindingNavigator), "BindingNavigator.MoveNext");
        Bitmap moveLastImage = ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(BindingNavigator), "BindingNavigator.MoveLast");
        Bitmap addNewImage = ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(BindingNavigator), "BindingNavigator.AddNew");
        Bitmap deleteImage = ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(BindingNavigator), "BindingNavigator.Delete");

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

        Items.AddRange(
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
            DeleteItem);
    }

    /// <summary>
    ///  The BindingSource who's list we are currently navigating, or null.
    /// </summary>
    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingNavigatorBindingSourcePropDescr))]
    [TypeConverter(typeof(ReferenceConverter))]
    public BindingSource? BindingSource
    {
        get
        {
            return _bindingSource;
        }

        set
        {
            WireUpBindingSource(ref _bindingSource, value);
        }
    }

    /// <summary>
    ///  The ToolStripItem that triggers the 'Move first' action, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorMoveFirstItemPropDescr))]
    public ToolStripItem? MoveFirstItem
    {
        get
        {
            if (_moveFirstItem is not null && _moveFirstItem.IsDisposed)
            {
                _moveFirstItem = null;
            }

            return _moveFirstItem;
        }

        set
        {
            WireUpButton(ref _moveFirstItem, value, OnMoveFirst);
        }
    }

    /// <summary>
    ///  The ToolStripItem that triggers the 'Move previous' action, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorMovePreviousItemPropDescr))]
    public ToolStripItem? MovePreviousItem
    {
        get
        {
            if (_movePreviousItem is not null && _movePreviousItem.IsDisposed)
            {
                _movePreviousItem = null;
            }

            return _movePreviousItem;
        }

        set
        {
            WireUpButton(ref _movePreviousItem, value, OnMovePrevious);
        }
    }

    /// <summary>
    ///  The ToolStripItem that triggers the 'Move next' action, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorMoveNextItemPropDescr))]
    public ToolStripItem? MoveNextItem
    {
        get
        {
            if (_moveNextItem is not null && _moveNextItem.IsDisposed)
            {
                _moveNextItem = null;
            }

            return _moveNextItem;
        }

        set
        {
            WireUpButton(ref _moveNextItem, value, OnMoveNext);
        }
    }

    /// <summary>
    ///  The ToolStripItem that triggers the 'Move last' action, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorMoveLastItemPropDescr))]
    public ToolStripItem? MoveLastItem
    {
        get
        {
            if (_moveLastItem is not null && _moveLastItem.IsDisposed)
            {
                _moveLastItem = null;
            }

            return _moveLastItem;
        }

        set
        {
            WireUpButton(ref _moveLastItem, value, OnMoveLast);
        }
    }

    /// <summary>
    ///  The ToolStripItem that triggers the 'Add new' action, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorAddNewItemPropDescr))]
    public ToolStripItem? AddNewItem
    {
        get
        {
            if (_addNewItem is not null && _addNewItem.IsDisposed)
            {
                _addNewItem = null;
            }

            return _addNewItem;
        }

        set
        {
            if (_addNewItem != value && value is not null)
            {
                value.InternalEnabledChanged += OnAddNewItemEnabledChanged;
                _addNewItemUserEnabled = value.Enabled;
            }

            WireUpButton(ref _addNewItem, value, OnAddNew);
        }
    }

    /// <summary>
    ///  The ToolStripItem that triggers the 'Delete' action, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorDeleteItemPropDescr))]
    public ToolStripItem? DeleteItem
    {
        get
        {
            if (_deleteItem is not null && _deleteItem.IsDisposed)
            {
                _deleteItem = null;
            }

            return _deleteItem;
        }

        set
        {
            if (_deleteItem != value && value is not null)
            {
                value.InternalEnabledChanged += OnDeleteItemEnabledChanged;
                _deleteItemUserEnabled = value.Enabled;
            }

            WireUpButton(ref _deleteItem, value, OnDelete);
        }
    }

    /// <summary>
    ///  The ToolStripItem that displays the current position, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorPositionItemPropDescr))]
    public ToolStripItem? PositionItem
    {
        get
        {
            if (_positionItem is not null && _positionItem.IsDisposed)
            {
                _positionItem = null;
            }

            return _positionItem;
        }

        set
        {
            WireUpTextBox(ref _positionItem, value, OnPositionKey, OnPositionLostFocus);
        }
    }

    /// <summary>
    ///  The ToolStripItem that displays the total number of items, or null.
    /// </summary>
    [TypeConverter(typeof(ReferenceConverter))]
    [SRCategory(nameof(SR.CatItems))]
    [SRDescription(nameof(SR.BindingNavigatorCountItemPropDescr))]
    public ToolStripItem? CountItem
    {
        get
        {
            if (_countItem is not null && _countItem.IsDisposed)
            {
                _countItem = null;
            }

            return _countItem;
        }
        set
        {
            WireUpLabel(ref _countItem, value);
        }
    }

    /// <summary>
    ///  Formatting to apply to count displayed in the CountItem tool strip item.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.BindingNavigatorCountItemFormatPropDescr))]
    public string CountItemFormat
    {
        get
        {
            return _countItemFormat;
        }
        set
        {
            if (_countItemFormat != value)
            {
                _countItemFormat = value;
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Event raised when the state of the tool strip items needs to be
    ///  refreshed to reflect the current state of the data.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.BindingNavigatorRefreshItemsEventDescr))]
    public event EventHandler? RefreshItems
    {
        add => _onRefreshItems += value;
        remove => _onRefreshItems -= value;
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
        if (_bindingSource is null)
        {
            count = 0;
            position = 0;
            allowNew = false;
            allowRemove = false;
        }
        else
        {
            count = _bindingSource.Count;
            position = _bindingSource.Position + 1;
            allowNew = (_bindingSource as IBindingList).AllowNew;
            allowRemove = (_bindingSource as IBindingList).AllowRemove;
        }

        // Enable or disable items (except when in design mode)
        if (!DesignMode)
        {
            if (MoveFirstItem is not null)
            {
                _moveFirstItem!.Enabled = (position > 1);
            }

            if (MovePreviousItem is not null)
            {
                _movePreviousItem!.Enabled = (position > 1);
            }

            if (MoveNextItem is not null)
            {
                _moveNextItem!.Enabled = (position < count);
            }

            if (MoveLastItem is not null)
            {
                _moveLastItem!.Enabled = (position < count);
            }

            if (AddNewItem is not null)
            {
                EventHandler handler = new(OnAddNewItemEnabledChanged);
                _addNewItem!.InternalEnabledChanged -= handler;
                _addNewItem.Enabled = (_addNewItemUserEnabled && allowNew);
                _addNewItem.InternalEnabledChanged += handler;
            }

            if (DeleteItem is not null)
            {
                EventHandler handler = new(OnDeleteItemEnabledChanged);
                _deleteItem!.InternalEnabledChanged -= handler;
                _deleteItem.Enabled = (_deleteItemUserEnabled && allowRemove && count > 0);
                _deleteItem.InternalEnabledChanged += handler;
            }

            if (PositionItem is not null)
            {
                _positionItem!.Enabled = (position > 0 && count > 0);
            }

            if (CountItem is not null)
            {
                _countItem!.Enabled = (count > 0);
            }
        }

        // Update current position indicator
        if (_positionItem is not null)
        {
            _positionItem.Text = position.ToString(CultureInfo.CurrentCulture);
        }

        // Update record count indicator
        if (_countItem is not null)
        {
            _countItem.Text = DesignMode ? CountItemFormat : string.Format(CultureInfo.CurrentCulture, CountItemFormat, count);
        }
    }

    /// <summary>
    ///  Called when the state of the tool strip items needs to be refreshed to reflect the current state of the data.
    ///  Calls <see cref="RefreshItemsCore"/> to refresh the state of the standard items, then raises the RefreshItems event.
    /// </summary>
    protected virtual void OnRefreshItems()
    {
        // Refresh all the standard items
        RefreshItemsCore();

        // Raise the public event
        _onRefreshItems?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Triggers form validation. Used by the BindingNavigator's standard items when clicked. If a validation error occurs
    ///  on the form, focus remains on the active control and the standard item does not perform its standard click action.
    ///  Custom items may also use this method to trigger form validation and check for success before performing an action.
    /// </summary>
    public bool Validate()
    {
        return ValidateActiveControl(out _);
    }

    /// <summary>
    ///  Accept new row position entered into PositionItem.
    /// </summary>
    private void AcceptNewPosition()
    {
        // If no position item or binding source, do nothing
        if (_positionItem is null || _bindingSource is null)
        {
            return;
        }

        // Default to old position, in case new position turns out to be garbage
        int newPosition = _bindingSource.Position;

        try
        {
            // Read new position from item text (and subtract one!)
            newPosition = Convert.ToInt32(_positionItem.Text, CultureInfo.CurrentCulture) - 1;
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
        if (newPosition != _bindingSource.Position)
        {
            _bindingSource.Position = newPosition;
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
    private void OnMoveFirst(object? sender, EventArgs e)
    {
        if (Validate())
        {
            if (_bindingSource is not null)
            {
                _bindingSource.MoveFirst();
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Navigates to previous item in BindingSource's list when the MovePreviousItem is clicked.
    /// </summary>
    private void OnMovePrevious(object? sender, EventArgs e)
    {
        if (Validate())
        {
            if (_bindingSource is not null)
            {
                _bindingSource.MovePrevious();
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Navigates to next item in BindingSource's list when the MoveNextItem is clicked.
    /// </summary>
    private void OnMoveNext(object? sender, EventArgs e)
    {
        if (Validate())
        {
            if (_bindingSource is not null)
            {
                _bindingSource.MoveNext();
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Navigates to last item in BindingSource's list when the MoveLastItem is clicked.
    /// </summary>
    private void OnMoveLast(object? sender, EventArgs e)
    {
        if (Validate())
        {
            if (_bindingSource is not null)
            {
                _bindingSource.MoveLast();
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Adds new item to BindingSource's list when the AddNewItem is clicked.
    /// </summary>
    private void OnAddNew(object? sender, EventArgs e)
    {
        if (Validate())
        {
            if (_bindingSource is not null)
            {
                _bindingSource.AddNew();
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Deletes current item from BindingSource's list when the DeleteItem is clicked.
    /// </summary>
    private void OnDelete(object? sender, EventArgs e)
    {
        if (Validate())
        {
            if (_bindingSource is not null)
            {
                _bindingSource.RemoveCurrent();
                RefreshItemsInternal();
            }
        }
    }

    /// <summary>
    ///  Navigates to specific item in BindingSource's list when a value is entered into the PositionItem.
    /// </summary>
    private void OnPositionKey(object? sender, KeyEventArgs e)
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
    private void OnPositionLostFocus(object? sender, EventArgs e)
    {
        AcceptNewPosition();
    }

    /// <summary>
    ///  Refresh tool strip items when something changes in the BindingSource.
    /// </summary>
    private void OnBindingSourceStateChanged(object? sender, EventArgs e)
    {
        RefreshItemsInternal();
    }

    /// <summary>
    ///  Refresh tool strip items when the BindingSource is disposed.
    /// </summary>
    private void OnBindingSourceDisposed(object? sender, EventArgs e)
    {
        BindingSource = null;
    }

    /// <summary>
    ///  Refresh tool strip items when something changes in the BindingSource's list.
    /// </summary>
    private void OnBindingSourceListChanged(object? sender, ListChangedEventArgs e)
    {
        RefreshItemsInternal();
    }

    /// <summary>
    ///  Refresh the state of the items when the state of the data changes.
    /// </summary>
    private void RefreshItemsInternal()
    {
        // Block all updates during initialization
        if (_initializing)
        {
            return;
        }

        // Call method that updates the items (overridable)
        OnRefreshItems();
    }

    private void ResetCountItemFormat()
    {
        _countItemFormat = SR.BindingNavigatorCountItemFormat;
    }

    private bool ShouldSerializeCountItemFormat()
    {
        return _countItemFormat != SR.BindingNavigatorCountItemFormat;
    }

    private void OnAddNewItemEnabledChanged(object? sender, EventArgs e)
    {
        if (AddNewItem is not null)
        {
            _addNewItemUserEnabled = _addNewItem!.Enabled;
        }
    }

    private void OnDeleteItemEnabledChanged(object? sender, EventArgs e)
    {
        if (DeleteItem is not null)
        {
            _deleteItemUserEnabled = _deleteItem!.Enabled;
        }
    }

    /// <summary>
    ///  Wire up some member variable to the specified button item, hooking events
    ///  on the new button and unhooking them from the previous button, if required.
    /// </summary>
    private void WireUpButton(ref ToolStripItem? oldButton, ToolStripItem? newButton, EventHandler clickHandler)
    {
        if (oldButton == newButton)
        {
            return;
        }

        if (oldButton is not null)
        {
            oldButton.Click -= clickHandler;
        }

        if (newButton is not null)
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
    private void WireUpTextBox(ref ToolStripItem? oldTextBox, ToolStripItem? newTextBox, KeyEventHandler keyUpHandler, EventHandler lostFocusHandler)
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
    private void WireUpLabel(ref ToolStripItem? oldLabel, ToolStripItem? newLabel)
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
    private void WireUpBindingSource(ref BindingSource? oldBindingSource, BindingSource? newBindingSource)
    {
        if (oldBindingSource != newBindingSource)
        {
            if (oldBindingSource is not null)
            {
                oldBindingSource.PositionChanged -= OnBindingSourceStateChanged;
                oldBindingSource.CurrentChanged -= OnBindingSourceStateChanged;
                oldBindingSource.CurrentItemChanged -= OnBindingSourceStateChanged;
                oldBindingSource.DataSourceChanged -= OnBindingSourceStateChanged;
                oldBindingSource.DataMemberChanged -= OnBindingSourceStateChanged;
                oldBindingSource.ListChanged -= OnBindingSourceListChanged;
                oldBindingSource.Disposed -= OnBindingSourceDisposed;
            }

            if (newBindingSource is not null)
            {
                newBindingSource.PositionChanged += OnBindingSourceStateChanged;
                newBindingSource.CurrentChanged += OnBindingSourceStateChanged;
                newBindingSource.CurrentItemChanged += OnBindingSourceStateChanged;
                newBindingSource.DataSourceChanged += OnBindingSourceStateChanged;
                newBindingSource.DataMemberChanged += OnBindingSourceStateChanged;
                newBindingSource.ListChanged += OnBindingSourceListChanged;
                newBindingSource.Disposed += OnBindingSourceDisposed;
            }

            oldBindingSource = newBindingSource;
            RefreshItemsInternal();
        }
    }
}
