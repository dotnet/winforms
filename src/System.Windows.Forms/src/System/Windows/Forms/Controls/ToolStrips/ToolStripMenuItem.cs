// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
///  ToolStripMenuItem
/// </summary>
[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
[DesignerSerializer($"System.Windows.Forms.Design.ToolStripMenuItemCodeDomSerializer, {AssemblyRef.SystemDesign}",
    $"System.ComponentModel.Design.Serialization.CodeDomSerializer, {AssemblyRef.SystemDesign}")]
public partial class ToolStripMenuItem : ToolStripDropDownItem
{
    private static readonly MenuTimer s_menuTimer = new();

    private static readonly int s_propShortcutKeys = PropertyStore.CreateKey();
    private static readonly int s_propCheckState = PropertyStore.CreateKey();
    private static readonly int s_propMdiForm = PropertyStore.CreateKey();

    private bool _checkOnClick;
    private bool _showShortcutKeys = true;
    private ToolStrip? _lastOwner;

    /// <summary>
    ///  Support for mapping NATIVE menu commands to ToolStripMenuItems.
    ///  It corresponds to <see cref="MENUITEMINFOW.wID"/>.
    /// </summary>
    private readonly int _nativeMenuCommandID = -1;
    private HandleRef<HWND> _targetWindowHandle;
    private HMENU _nativeMenuHandle = HMENU.Null;

    // Keep checked images shared between menu items, but per thread so we don't have locking issues in GDI+
    [ThreadStatic]
    private static Image? t_indeterminateCheckedImage;

    [ThreadStatic]
    private static Image? t_checkedImage;

    private string? _shortcutKeyDisplayString;
    private string? _cachedShortcutText;
    private Size _cachedShortcutSize = Size.Empty;

    private static readonly Padding s_defaultPadding = new(4, 0, 4, 0);
    private static readonly Padding s_defaultDropDownPadding = new(0, 1, 0, 1);
    private static readonly Size s_checkMarkBitmapSize = new(16, 16);

    private byte _openMouseId;

    private static readonly object s_eventCheckedChanged = new();
    private static readonly object s_eventCheckStateChanged = new();

    public ToolStripMenuItem()
    {
        Initialize(); // all additional work should be done in Initialize
    }

    public ToolStripMenuItem(string? text)
        : base(text, image: null, onClick: null)
    {
        Initialize();
    }

    public ToolStripMenuItem(Image? image)
        : base(text: null, image, onClick: null)
    {
        Initialize();
    }

    public ToolStripMenuItem(string? text, Image? image)
        : base(text, image, onClick: null)
    {
        Initialize();
    }

    public ToolStripMenuItem(string? text, Image? image, EventHandler? onClick)
        : base(text, image, onClick)
    {
        Initialize();
    }

    public ToolStripMenuItem(string? text, Image? image, EventHandler? onClick, string? name)
        : base(text, image, onClick, name)
    {
        Initialize();
    }

    public ToolStripMenuItem(string? text, Image? image, params ToolStripItem[]? dropDownItems)
        : base(text, image, dropDownItems)
    {
        Initialize();
    }

    public ToolStripMenuItem(string? text, Image? image, EventHandler? onClick, Keys shortcutKeys)
        : base(text, image, onClick)
    {
        Initialize();
        ShortcutKeys = shortcutKeys;
    }

    internal ToolStripMenuItem(Form mdiForm)
    {
        Initialize();
        Properties.SetObject(s_propMdiForm, mdiForm);
    }

    /// <summary>
    ///  This constructor is only used when we're trying to mimic a native menu like the system menu. In that case
    ///  we collect the command id and the target window to send WM_COMMAND/WM_SYSCOMMAND messages to.
    /// </summary>
    internal ToolStripMenuItem(HMENU hmenu, int nativeMenuCommandId, IWin32Window targetWindow)
    {
        Initialize();
        Overflow = ToolStripItemOverflow.Never;
        _nativeMenuCommandID = nativeMenuCommandId;
        _targetWindowHandle = Control.GetSafeHandle(targetWindow);
        _nativeMenuHandle = hmenu;

        // Since fetching the image and the text is an awful lot of work
        // we're going to just cache it and assume the native stuff
        // doesnt update.
        // we'll only live-update enabled.
        // if this becomes a problem we can override Image and Text properties
        // to live-return the results.

        // fetch image
        Image = GetNativeMenuItemImage();
        ImageScaling = ToolStripItemImageScaling.None;

        // fetch text
        string? text = GetNativeMenuItemTextAndShortcut();

        // the shortcut is tab separated from the item text.
        if (text is not null)
        {
            // separate out the two fields.
            string[] textFields = text.Split('\t');

            if (textFields.Length >= 1)
            {
                Text = textFields[0];
            }

            if (textFields.Length >= 2)
            {
                // We don't care about the shortcut here, the OS is going to
                // handle it for us by sending a WM_(SYS)COMMAND during TranslateAccelerator
                // Just display whatever the OS would have.
                ShowShortcutKeys = true;
                ShortcutKeyDisplayString = textFields[1];
            }
        }
    }

    internal override void AutoHide(ToolStripItem otherItemBeingSelected)
    {
        if (IsOnDropDown)
        {
            MenuTimer.Transition(this, otherItemBeingSelected as ToolStripMenuItem);
        }
        else
        {
            base.AutoHide(otherItemBeingSelected);
        }
    }

    private void ClearShortcutCache()
    {
        _cachedShortcutSize = Size.Empty;
        _cachedShortcutText = null;
    }

    protected override ToolStripDropDown CreateDefaultDropDown()
    {
        // AutoGenerate a ToolStrip DropDown - set the property so we hook events
        return new ToolStripDropDownMenu(this, true);
    }

    private protected override ToolStripItemInternalLayout CreateInternalLayout()
    {
        return new ToolStripMenuItemInternalLayout(this);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new ToolStripMenuItemAccessibleObject(this);
    }

    private void Initialize()
    {
        if (Control.UseComponentModelRegisteredTypes)
        {
            // Register the type with the ComponentModel so as to be trim safe
            TypeDescriptor.RegisterType<Keys>();
        }

        Overflow = ToolStripItemOverflow.Never;
        MouseDownAndUpMustBeInSameItem = false;
        SupportsDisabledHotTracking = true;
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize => ScaleHelper.IsThreadPerMonitorV2Aware
        ? ScaleHelper.ScaleToDpi(new Size(32, 19), DeviceDpi)
        : new Size(32, 19);

    protected internal override Padding DefaultMargin => Padding.Empty;

    protected override Padding DefaultPadding => IsOnDropDown
        ? ScaleHelper.ScaleToDpi(s_defaultDropDownPadding, DeviceDpi)
        : ScaleHelper.ScaleToDpi(s_defaultPadding, DeviceDpi);

    public override bool Enabled
    {
        get
        {
            if (_nativeMenuCommandID != -1)
            {
                // if we're based off a native menu item,
                // we need to ask it if it's enabled.
                return base.Enabled
                    && !_nativeMenuHandle.IsNull
                    && !_targetWindowHandle.IsNull
                    && GetNativeMenuItemEnabled();
            }
            else
            {
                return base.Enabled;
            }
        }
        set => base.Enabled = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the item is checked.
    /// </summary>
    [Bindable(true)]
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.CheckBoxCheckedDescr))]
    public bool Checked
    {
        get => CheckState != CheckState.Unchecked;
        set
        {
            if (value != Checked)
            {
                CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                InvokePaint();
            }
        }
    }

    /// <summary>
    ///  Keeps a shared copy of the checked image between all menu items
    ///  Fishes out the appropriate one based on CheckState.
    /// </summary>
    internal Image? CheckedImage => CheckState switch
    {
        CheckState.Indeterminate => t_indeterminateCheckedImage ??= ScaleHelper.GetIconResourceAsBitmap(
            typeof(ToolStripMenuItem),
            "IndeterminateChecked",
            ScaleHelper.ScaleToDpi(s_checkMarkBitmapSize, DeviceDpi)),
        CheckState.Checked => t_checkedImage ??= ScaleHelper.GetIconResourceAsBitmap(
            typeof(ToolStripMenuItem),
            "Checked",
            ScaleHelper.ScaleToDpi(s_checkMarkBitmapSize, DeviceDpi)),
        _ => null,
    };

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripButtonCheckOnClickDescr))]
    public bool CheckOnClick
    {
        get => _checkOnClick;
        set => _checkOnClick = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the check box is checked.
    /// </summary>
    [Bindable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(CheckState.Unchecked)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.CheckBoxCheckStateDescr))]
    public CheckState CheckState
    {
        get
        {
            object checkState = Properties.GetInteger(s_propCheckState, out bool found);
            return (found) ? (CheckState)checkState : CheckState.Unchecked;
        }
        set
        {
            // Valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            if (value != CheckState)
            {
                Properties.SetInteger(s_propCheckState, (int)value);
                OnCheckedChanged(EventArgs.Empty);
                OnCheckStateChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the
    ///  value of the <see cref="CheckBox.Checked"/>
    ///  property changes.
    /// </summary>
    [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
    public event EventHandler? CheckedChanged
    {
        add => Events.AddHandler(s_eventCheckedChanged, value);
        remove => Events.RemoveHandler(s_eventCheckedChanged, value);
    }

    /// <summary>
    ///  Occurs when the
    ///  value of the <see cref="CheckBox.CheckState"/>
    ///  property changes.
    /// </summary>
    [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
    public event EventHandler? CheckStateChanged
    {
        add => Events.AddHandler(s_eventCheckStateChanged, value);
        remove => Events.RemoveHandler(s_eventCheckStateChanged, value);
    }

    /// <summary>
    ///  Specifies whether or not the item is glued to the ToolStrip or overflow or
    ///  can float between the two.
    /// </summary>
    [DefaultValue(ToolStripItemOverflow.Never)]
    [SRDescription(nameof(SR.ToolStripItemOverflowDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public new ToolStripItemOverflow Overflow
    {
        get => base.Overflow;
        set => base.Overflow = value;
    }

    /// <summary>
    ///  Gets or sets the shortcut keys associated with the menu
    ///  item.
    /// </summary>
    [Localizable(true)]
    [DefaultValue(Keys.None)]
    [SRDescription(nameof(SR.MenuItemShortCutDescr))]
    public Keys ShortcutKeys
    {
        get
        {
            object shortcutKeys = Properties.GetInteger(s_propShortcutKeys, out bool found);
            return (found) ? (Keys)shortcutKeys : Keys.None;
        }
        set
        {
            if ((value != Keys.None) && !ToolStripManager.IsValidShortcut(value))
            {
                // prevent use of alt, ctrl, shift modifiers with no key code.
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Keys));
            }

            Keys originalShortcut = ShortcutKeys;
            if (originalShortcut != value)
            {
                ClearShortcutCache();
                ToolStrip? owner = Owner;
                if (owner is not null)
                {
                    // add to the shortcut caching system.
                    if (originalShortcut != Keys.None)
                    {
                        owner.Shortcuts.Remove(originalShortcut);
                    }

                    if (owner.Shortcuts.ContainsKey(value))
                    {
                        // last one in wins.
                        owner.Shortcuts[value] = this;
                    }
                    else
                    {
                        owner.Shortcuts.Add(value, this);
                    }
                }

                Properties.SetInteger(s_propShortcutKeys, (int)value);

                if (ShowShortcutKeys && IsOnDropDown)
                {
                    if (GetCurrentParentDropDown() is ToolStripDropDownMenu parent)
                    {
                        LayoutTransaction.DoLayout(ParentInternal, this, "ShortcutKeys");
                        parent.AdjustSize();
                    }
                }
            }
        }
    }

    [SRDescription(nameof(SR.ToolStripMenuItemShortcutKeyDisplayStringDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(null)]
    [Localizable(true)]
    public string? ShortcutKeyDisplayString
    {
        get => _shortcutKeyDisplayString;
        set
        {
            if (_shortcutKeyDisplayString == value)
            {
                return;
            }

            _shortcutKeyDisplayString = value;
            ClearShortcutCache();

            if (!ShowShortcutKeys)
            {
                return;
            }

            if (ParentInternal is ToolStripDropDown parent)
            {
                LayoutTransaction.DoLayout(parent, this, "ShortcutKeyDisplayString");
                parent.AdjustSize();
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value that indicates whether the shortcut
    ///  keys that are associated
    ///  with the menu item are displayed next to the menu item
    ///  caption.
    /// </summary>
    [DefaultValue(true)]
    [Localizable(true)]
    [SRDescription(nameof(SR.MenuItemShowShortCutDescr))]
    public bool ShowShortcutKeys
    {
        get => _showShortcutKeys;
        set
        {
            if (value == _showShortcutKeys)
            {
                return;
            }

            ClearShortcutCache();
            _showShortcutKeys = value;

            if (ParentInternal is ToolStripDropDown parent)
            {
                LayoutTransaction.DoLayout(parent, this, "ShortcutKeys");
                parent.AdjustSize();
            }
        }
    }

    /// <summary>
    ///  An item is toplevel if it is parented to anything other than a ToolStripDropDownMenu
    ///  This implies that a ToolStripMenuItem in an overflow IS a toplevel item
    /// </summary>
    internal bool IsTopLevel => ParentInternal as ToolStripDropDown is null;

    [Browsable(false)]
    public bool IsMdiWindowListEntry => MdiForm is not null;

    internal static MenuTimer MenuTimer => s_menuTimer;

    /// <summary> Tag property for internal use </summary>
    internal Form? MdiForm => Properties.TryGetObject(s_propMdiForm, out Form? form) ? form : null;

    internal ToolStripMenuItem Clone()
    {
        // dirt simple clone - just properties, no subitems

        ToolStripMenuItem menuItem = new();
        menuItem.Events.AddHandlers(Events);

        menuItem.AccessibleName = AccessibleName;
        menuItem.AccessibleRole = AccessibleRole;
        menuItem.Alignment = Alignment;
        menuItem.AllowDrop = AllowDrop;
        menuItem.Anchor = Anchor;
        menuItem.AutoSize = AutoSize;
        menuItem.AutoToolTip = AutoToolTip;
        menuItem.BackColor = BackColor;
        menuItem.BackgroundImage = BackgroundImage;
        menuItem.BackgroundImageLayout = BackgroundImageLayout;
        menuItem.Checked = Checked;
        menuItem.CheckOnClick = CheckOnClick;
        menuItem.CheckState = CheckState;
        menuItem.DisplayStyle = DisplayStyle;
        menuItem.Dock = Dock;
        menuItem.DoubleClickEnabled = DoubleClickEnabled;
        menuItem.Enabled = Enabled;
        menuItem.Font = Font;
        menuItem.ForeColor = ForeColor;
        menuItem.Image = Image;
        menuItem.ImageAlign = ImageAlign;
        menuItem.ImageScaling = ImageScaling;
        menuItem.ImageTransparentColor = ImageTransparentColor;
        menuItem.Margin = Margin;
        menuItem.MergeAction = MergeAction;
        menuItem.MergeIndex = MergeIndex;
        menuItem.Name = Name;
        menuItem.Overflow = Overflow;
        menuItem.Padding = Padding;
        menuItem.RightToLeft = RightToLeft;

        // No settings support for cloned items.
        // menuItem.SaveSettings= this.SaveSettings;
        // menuItem.SettingsKey = this.SettingsKey;

        menuItem.ShortcutKeys = ShortcutKeys;
        menuItem.ShowShortcutKeys = ShowShortcutKeys;
        menuItem.Tag = Tag;
        menuItem.Text = Text;
        menuItem.TextAlign = TextAlign;
        menuItem.TextDirection = TextDirection;
        menuItem.TextImageRelation = TextImageRelation;
        menuItem.ToolTipText = ToolTipText;

        // can't actually use "Visible" property as that returns whether or not the parent
        // is visible too.. instead use ParticipatesInLayout as this queries the actual state.
        menuItem.Visible = ((IArrangedElement)this).ParticipatesInLayout;

        if (!AutoSize)
        {
            menuItem.Size = Size;
        }

        return menuItem;
    }

    internal override int DeviceDpi
    {
        set
        {
            // This gets called via ToolStripItem.RescaleConstantsForDpi.
            // It's practically calling Initialize on DpiChanging with the new Dpi value.

            if (DeviceDpi != value)
            {
                base.DeviceDpi = value;
                t_indeterminateCheckedImage?.Dispose();
                t_indeterminateCheckedImage = null;
                t_checkedImage?.Dispose();
                t_checkedImage = null;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_lastOwner is not null)
            {
                Keys shortcut = ShortcutKeys;
                if (shortcut != Keys.None && _lastOwner.Shortcuts.ContainsKey(shortcut))
                {
                    _lastOwner.Shortcuts.Remove(shortcut);
                }

                _lastOwner = null;
                if (MdiForm is not null)
                {
                    Properties.SetObject(s_propMdiForm, null);
                }
            }
        }

        base.Dispose(disposing);
    }

    private unsafe bool GetNativeMenuItemEnabled()
    {
        if (_nativeMenuCommandID == -1 || _nativeMenuHandle == IntPtr.Zero)
        {
            Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
            return false;
        }

        MENUITEMINFOW info = new()
        {
            cbSize = (uint)sizeof(MENUITEMINFOW),
            fMask = MENU_ITEM_MASK.MIIM_STATE,
            wID = (uint)_nativeMenuCommandID
        };

        PInvoke.GetMenuItemInfo(_nativeMenuHandle, (uint)_nativeMenuCommandID, fByPosition: false, ref info);
        return (info.fState & MENU_ITEM_STATE.MFS_DISABLED) == 0;
    }

    // returns text and shortcut separated by tab.
    private unsafe string? GetNativeMenuItemTextAndShortcut()
    {
        if (_nativeMenuCommandID == -1 || _nativeMenuHandle == IntPtr.Zero)
        {
            Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
            return null;
        }

        string? text = null;

        // fetch the string length
        MENUITEMINFOW info = new()
        {
            cbSize = (uint)sizeof(MENUITEMINFOW),
            fMask = MENU_ITEM_MASK.MIIM_STRING,
            wID = (uint)_nativeMenuCommandID
        };

        PInvoke.GetMenuItemInfo(_nativeMenuHandle, (uint)_nativeMenuCommandID, fByPosition: false, ref info);

        if (info.cch > 0)
        {
            // Fetch the string
            info.cch += 1;  // according to MSDN we need to increment the count we receive by 1.
            info.wID = (uint)_nativeMenuCommandID;
            nint allocatedStringBuffer = Marshal.AllocCoTaskMem((int)info.cch * sizeof(char));
            info.dwTypeData = (char*)allocatedStringBuffer;

            try
            {
                PInvoke.GetMenuItemInfo(_nativeMenuHandle, (uint)_nativeMenuCommandID, fByPosition: false, ref info);

                // Convert the string into managed data.
                if (!info.dwTypeData.IsNull)
                {
                    text = new string(info.dwTypeData, 0, (int)info.cch);
                }
            }
            finally
            {
                if (allocatedStringBuffer != 0)
                {
                    // use our local instead of the info structure member *just* in case windows decides to clobber over it.
                    // we want to be sure to deallocate the memory we know we allocated.
                    Marshal.FreeCoTaskMem(allocatedStringBuffer);
                }
            }
        }

        return text;
    }

    private unsafe Bitmap? GetNativeMenuItemImage()
    {
        if (_nativeMenuCommandID == -1 || _nativeMenuHandle.IsNull)
        {
            Debug.Fail("Why were we called to fetch native menu item info with nothing assigned?");
            return null;
        }

        MENUITEMINFOW info = new()
        {
            cbSize = (uint)sizeof(MENUITEMINFOW),
            fMask = MENU_ITEM_MASK.MIIM_BITMAP,
            wID = (uint)_nativeMenuCommandID
        };

        PInvoke.GetMenuItemInfo(_nativeMenuHandle, (uint)_nativeMenuCommandID, fByPosition: false, ref info);

        if (!info.hbmpItem.IsNull && (int)info.hbmpItem > (int)HBITMAP.HBMMENU_POPUP_MINIMIZE)
        {
            return Image.FromHbitmap(info.hbmpItem);
        }

        // Its a system defined bitmap.
        int buttonToUse = -1;

        if (info.hbmpItem == HBITMAP.HBMMENU_MBAR_CLOSE
            || info.hbmpItem == HBITMAP.HBMMENU_MBAR_CLOSE_D
            || info.hbmpItem == HBITMAP.HBMMENU_POPUP_CLOSE)
        {
            buttonToUse = (int)CaptionButton.Close;
        }
        else if (info.hbmpItem == HBITMAP.HBMMENU_MBAR_MINIMIZE
            || info.hbmpItem == HBITMAP.HBMMENU_MBAR_MINIMIZE_D
            || info.hbmpItem == HBITMAP.HBMMENU_POPUP_MINIMIZE)
        {
            buttonToUse = (int)CaptionButton.Minimize;
        }
        else if (info.hbmpItem == HBITMAP.HBMMENU_MBAR_RESTORE
            || info.hbmpItem == HBITMAP.HBMMENU_POPUP_RESTORE)
        {
            buttonToUse = (int)CaptionButton.Restore;
        }
        else if (info.hbmpItem == HBITMAP.HBMMENU_POPUP_MAXIMIZE)
        {
            buttonToUse = (int)CaptionButton.Maximize;
        }
        else
        {
            return null;
        }

        // Ee've mapped to a system defined bitmap we know how to draw.
        Bitmap image = new(16, 16);

        using (Graphics g = Graphics.FromImage(image))
        {
            ControlPaint.DrawCaptionButton(g, new Rectangle(Point.Empty, image.Size), (CaptionButton)buttonToUse, ButtonState.Flat);
            g.DrawRectangle(SystemPens.Control, 0, 0, image.Width - 1, image.Height - 1);
        }

        image.MakeTransparent(SystemColors.Control);
        return image;
    }

    internal Size GetShortcutTextSize()
    {
        if (!ShowShortcutKeys)
        {
            return Size.Empty;
        }

        string? shortcutString = GetShortcutText();
        if (string.IsNullOrEmpty(shortcutString))
        {
            return Size.Empty;
        }

        if (_cachedShortcutSize == Size.Empty)
        {
            _cachedShortcutSize = TextRenderer.MeasureText(shortcutString, Font);
        }

        return _cachedShortcutSize;
    }

    internal string? GetShortcutText()
    {
        _cachedShortcutText ??= ShortcutToText(ShortcutKeys, ShortcutKeyDisplayString);

        return _cachedShortcutText;
    }

    internal void HandleAutoExpansion()
    {
        if (!Enabled || ParentInternal is null || !ParentInternal.MenuAutoExpand || !HasDropDownItems)
        {
            return;
        }

        ShowDropDown();

        KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);

        DropDown.SelectNextToolStripItem(start: null, forward: true);
    }

    protected override void OnClick(EventArgs e)
    {
        if (_checkOnClick)
        {
            Checked = !Checked;
        }

        base.OnClick(e);

        if (_nativeMenuCommandID == -1)
        {
            return;
        }

        // fire off the appropriate native handler by posting a message to the window target.
        if ((_nativeMenuCommandID & 0xF000) != 0)
        {
            // These are system menu items like Minimize, Maximize, Restore, Resize, Move, Close.

            // use PostMessage instead of SendMessage so that the DefWndProc can appropriately handle
            // the system message... if we use SendMessage the dismissal of our window
            // breaks things like the modal sizing loop.
            PInvoke.PostMessage(_targetWindowHandle, PInvoke.WM_SYSCOMMAND, (WPARAM)(uint)_nativeMenuCommandID);
        }
        else
        {
            // These are user added items like ".Net Window..."

            // be consistent with sending a WM_SYSCOMMAND, use POST not SEND.
            PInvoke.PostMessage(_targetWindowHandle, PInvoke.WM_COMMAND, (WPARAM)(uint)_nativeMenuCommandID);
        }

        Invalidate();
    }

    /// <summary>
    ///  Raises the <see cref="CheckedChanged"/> event.
    /// </summary>
    protected virtual void OnCheckedChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_eventCheckedChanged])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="CheckStateChanged"/> event.
    /// </summary>
    protected virtual void OnCheckStateChanged(EventArgs e)
    {
        AccessibilityNotifyClients(AccessibleEvents.StateChange);
        ((EventHandler?)Events[s_eventCheckStateChanged])?.Invoke(this, e);
    }

    protected override void OnDropDownHide(EventArgs e)
    {
        MenuTimer.Cancel(this);
        base.OnDropDownHide(e);
    }

    protected override void OnDropDownShow(EventArgs e)
    {
        // If someone has beaten us to the punch by arrowing around, cancel the current menu timer.
        MenuTimer.Cancel(this);
        if (ParentInternal is not null)
        {
            ParentInternal.MenuAutoExpand = true;
        }

        base.OnDropDownShow(e);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        ClearShortcutCache();
        base.OnFontChanged(e);
    }

    internal void OnMenuAutoExpand()
    {
        ShowDropDown();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        MenuTimer.Cancel(this);
        OnMouseButtonStateChange(e, isMouseDown: true);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        OnMouseButtonStateChange(e, isMouseDown: false);
        base.OnMouseUp(e);
    }

    private void OnMouseButtonStateChange(MouseEventArgs e, bool isMouseDown)
    {
        bool showDropDown = true;
        if (IsOnDropDown)
        {
            ToolStripDropDown dropDown = GetCurrentParentDropDown()!;

            // Right click support for context menus.
            // used in ToolStripItem to determine whether to fire click OnMouseUp.
            SupportsRightClick = (dropDown.GetFirstDropDown() is ContextMenuStrip);
        }
        else
        {
            showDropDown = !DropDown.Visible;
            SupportsRightClick = false;
        }

        if (e.Button == MouseButtons.Left ||
          (e.Button == MouseButtons.Right && SupportsRightClick))
        {
            if (isMouseDown && showDropDown)
            {
                // opening should happen on mouse down.
                Debug.Assert(ParentInternal is not null, "Parent is null here, not going to get accurate ID");
                _openMouseId = (ParentInternal is null) ? (byte)0 : ParentInternal.GetMouseId();
                ShowDropDown(/*mousePush =*/true);
            }
            else if (!isMouseDown && !showDropDown)
            {
                // closing should happen on mouse up.  ensure it's not the mouse
                // up for the mouse down we opened with.
                Debug.Assert(ParentInternal is not null, "Parent is null here, not going to get accurate ID");
                byte closeMouseId = (ParentInternal is null) ? (byte)0 : ParentInternal.GetMouseId();
                int openedMouseID = _openMouseId;
                if (closeMouseId != openedMouseID)
                {
                    _openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
                    ToolStripManager.ModalMenuFilter.CloseActiveDropDown(DropDown, ToolStripDropDownCloseReason.AppClicked);
                    Select();
                }
            }
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        Debug.Assert(ParentInternal is not null, "Why is parent null");

        // If we are in a submenu pop down the submenu.
        if (ParentInternal is not null && ParentInternal.MenuAutoExpand && Selected)
        {
            MenuTimer.Cancel(this);
            MenuTimer.Start(this);
        }

        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        MenuTimer.Cancel(this);
        base.OnMouseLeave(e);
    }

    protected override void OnOwnerChanged(EventArgs e)
    {
        Keys shortcut = ShortcutKeys;
        if (shortcut != Keys.None)
        {
            _lastOwner?.Shortcuts.Remove(shortcut);

            if (Owner is not null)
            {
                if (Owner.Shortcuts.ContainsKey(shortcut))
                {
                    // last one in wins
                    Owner.Shortcuts[shortcut] = this;
                }
                else
                {
                    Owner.Shortcuts.Add(shortcut, this);
                }

                _lastOwner = Owner;
            }
        }

        base.OnOwnerChanged(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (Owner is null)
        {
            return;
        }

        ToolStripRenderer renderer = Renderer!;
        Graphics g = e.Graphics;
        renderer.DrawMenuItemBackground(new ToolStripItemRenderEventArgs(g, this));

        Color textColor = SystemColors.MenuText;
        if (IsForeColorSet)
        {
            textColor = ForeColor;
        }
        else if (!IsTopLevel || (ToolStripManager.VisualStylesEnabled))
        {
            if (Selected || Pressed)
            {
                textColor = SystemColors.HighlightText;
            }
            else
            {
                textColor = SystemColors.MenuText;
            }
        }

        bool rightToLeft = (RightToLeft == RightToLeft.Yes);

        if (InternalLayout is ToolStripMenuItemInternalLayout menuItemInternalLayout && menuItemInternalLayout.UseMenuLayout)
        {
            // Support for special DropDownMenu layout
            if (CheckState != CheckState.Unchecked && menuItemInternalLayout.PaintCheck)
            {
                Rectangle checkRectangle = menuItemInternalLayout.CheckRectangle;
                if (!menuItemInternalLayout.ShowCheckMargin)
                {
                    checkRectangle = menuItemInternalLayout.ImageRectangle;
                }

                if (checkRectangle.Width != 0)
                {
                    renderer.DrawItemCheck(new ToolStripItemImageRenderEventArgs(g, this, CheckedImage, checkRectangle));
                }
            }

            if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
            {
                // render text AND shortcut
                renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, Text, InternalLayout.TextRectangle, textColor, Font, (rightToLeft) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft));
                bool showShortCut = ShowShortcutKeys;
                if (!DesignMode)
                {
                    showShortCut = showShortCut && !HasDropDownItems;
                }

                if (showShortCut)
                {
                    renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, GetShortcutText(), InternalLayout.TextRectangle, textColor, Font, (rightToLeft) ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight));
                }
            }

            if (HasDropDownItems)
            {
                ArrowDirection arrowDir = (rightToLeft) ? ArrowDirection.Left : ArrowDirection.Right;
                Color arrowColor = (Selected || Pressed) ? SystemColors.HighlightText : SystemColors.MenuText;
                arrowColor = (Enabled) ? arrowColor : SystemColors.ControlDark;
                renderer.DrawArrow(new ToolStripArrowRenderEventArgs(g, this, menuItemInternalLayout.ArrowRectangle, arrowColor, arrowDir));
            }

            if (menuItemInternalLayout.PaintImage && (DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image && Image is not null)
            {
                renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, InternalLayout.ImageRectangle));
            }
        }
        else
        {
            // Toplevel item support, menu items hosted on a plain ToolStrip dropdown
            if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
            {
                renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, Text, InternalLayout.TextRectangle, textColor, Font, InternalLayout.TextFormat));
            }

            if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image && Image is not null)
            {
                renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, InternalLayout.ImageRectangle));
            }
        }
    }

    protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (Enabled && ShortcutKeys == keyData && !HasDropDownItems)
        {
            FireEvent(ToolStripItemEventType.Click);
            return true;
        }

        // call base here to get ESC, ALT, etc.. handling.
        return base.ProcessCmdKey(ref m, keyData);
    }

    protected internal override bool ProcessMnemonic(char charCode)
    {
        // no need to check IsMnemonic, toolstrip.ProcessMnemonic checks this already.
        if (HasDropDownItems)
        {
            Select();
            ShowDropDown();

            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);

            DropDown.SelectNextToolStripItem(start: null, forward: true);
            return true;
        }

        return base.ProcessMnemonic(charCode);
    }

    /// <summary> overridden here so we scooch over when we're in the ToolStripDropDownMenu</summary>
    protected internal override void SetBounds(Rectangle rect)
    {
        if (InternalLayout is ToolStripMenuItemInternalLayout internalLayout && internalLayout.UseMenuLayout)
        {
            // Scooch over by the padding amount.  The padding is added to
            // the ToolStripDropDownMenu to keep the non-menu item riffraff
            // aligned to the text rectangle. When flow layout comes through to set our position
            // via IArrangedElement DEFY IT!
            if (Owner is ToolStripDropDownMenu dropDownMenu)
            {
                rect.X -= dropDownMenu.Padding.Left;
                rect.X = Math.Max(rect.X, 0);
            }
        }

        base.SetBounds(rect);
    }

    /// <summary> this is to support routing to native menu commands </summary>
    internal void SetNativeTargetWindow(IWin32Window window)
    {
        _targetWindowHandle = Control.GetSafeHandle(window);
    }

    /// <summary> this is to support routing to native menu commands </summary>
    internal void SetNativeTargetMenu(HMENU hmenu) => _nativeMenuHandle = hmenu;

    internal static string? ShortcutToText(Keys shortcutKeys, string? shortcutKeyDisplayString)
    {
        if (!string.IsNullOrEmpty(shortcutKeyDisplayString))
        {
            return shortcutKeyDisplayString;
        }

        if (shortcutKeys == Keys.None)
        {
            return string.Empty;
        }

        if (!Control.UseComponentModelRegisteredTypes)
        {
            return TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(context: null, CultureInfo.CurrentUICulture, shortcutKeys);
        }
        else
        {
            // Call the trim safe API, Keys type has been registered at Initialize()
            return TypeDescriptor.GetConverterFromRegisteredType(typeof(Keys)).ConvertToString(context: null, CultureInfo.CurrentUICulture, shortcutKeys);
        }
    }

    internal override bool IsBeingTabbedTo()
    {
        if (base.IsBeingTabbedTo())
        {
            return true;
        }

        return ToolStripManager.ModalMenuFilter.InMenuMode;
    }
}
