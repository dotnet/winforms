// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ToolStripMenuItem
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    [DesignerSerializer("System.Windows.Forms.Design.ToolStripMenuItemCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    public partial class ToolStripMenuItem : ToolStripDropDownItem
    {
        private static readonly MenuTimer s_menuTimer = new MenuTimer();

        private static readonly int s_propShortcutKeys = PropertyStore.CreateKey();
        private static readonly int s_propCheckState = PropertyStore.CreateKey();
        private static readonly int s_propMdiForm = PropertyStore.CreateKey();

        private bool _checkOnClick;
        private bool _showShortcutKeys = true;
        private ToolStrip _lastOwner;

        /// <summary>
        /// Support for mapping NATIVE menu commands to ToolStripMenuItems.
        /// It corresponds to <see cref="User32.MENUITEMINFOW.wID"/>.
        /// </summary>
        private readonly int _nativeMenuCommandID = -1;
        private IntPtr _targetWindowHandle = IntPtr.Zero;
        private IntPtr _nativeMenuHandle = IntPtr.Zero;

        // Keep checked images shared between menu items, but per thread so we dont have locking issues in GDI+
        [ThreadStatic]
        private static Image t_indeterminateCheckedImage;

        [ThreadStatic]
        private static Image t_checkedImage;

        private string _shortcutKeyDisplayString;
        private string _cachedShortcutText;
        private Size _cachedShortcutSize = Size.Empty;

        private static readonly Padding s_defaultPadding = new Padding(4, 0, 4, 0);
        private static readonly Padding s_defaultDropDownPadding = new Padding(0, 1, 0, 1);
        private static readonly Size s_checkMarkBitmapSize = new Size(16, 16);
        private Padding _scaledDefaultPadding = s_defaultPadding;
        private Padding _scaledDefaultDropDownPadding = s_defaultDropDownPadding;
        private Size _scaledCheckMarkBitmapSize = s_checkMarkBitmapSize;

        private byte _openMouseId;

        private static readonly object s_eventCheckedChanged = new object();
        private static readonly object s_eventCheckStateChanged = new object();

        public ToolStripMenuItem() : base()
        {
            Initialize(); // all additional work should be done in Initialize
        }

        public ToolStripMenuItem(string text) : base(text, null, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripMenuItem(Image image) : base(null, image, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripMenuItem(string text, Image image) : base(text, image, (EventHandler)null)
        {
            Initialize();
        }

        public ToolStripMenuItem(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
            Initialize();
        }

        public ToolStripMenuItem(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            Initialize();
        }

        public ToolStripMenuItem(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems)
        {
            Initialize();
        }

        public ToolStripMenuItem(string text, Image image, EventHandler onClick, Keys shortcutKeys) : base(text, image, onClick)
        {
            Initialize();
            ShortcutKeys = shortcutKeys;
        }

        internal ToolStripMenuItem(Form mdiForm)
        {
            Initialize();
            Properties.SetObject(s_propMdiForm, mdiForm);
        }

        /// <summary> this constructor is only used when we're trying to
        ///  mimic a native menu like the system menu.  In that case
        ///  we've got to go ahead and collect the command id and the
        ///  target window to send WM_COMMAND/WM_SYSCOMMAND messages to.
        /// </summary>
        internal ToolStripMenuItem(IntPtr hMenu, int nativeMenuCommandId, IWin32Window targetWindow)
        {
            Initialize();
            Overflow = ToolStripItemOverflow.Never;
            _nativeMenuCommandID = nativeMenuCommandId;
            _targetWindowHandle = Control.GetSafeHandle(targetWindow);
            _nativeMenuHandle = hMenu;

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
            string text = GetNativeMenuItemTextAndShortcut();

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
                    // We dont care about the shortcut here, the OS is going to
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
            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(s_defaultPadding);
                _scaledDefaultDropDownPadding = DpiHelper.LogicalToDeviceUnits(s_defaultDropDownPadding);
                _scaledCheckMarkBitmapSize = DpiHelper.LogicalToDeviceUnits(s_checkMarkBitmapSize);
            }

            Overflow = ToolStripItemOverflow.Never;
            MouseDownAndUpMustBeInSameItem = false;
            SupportsDisabledHotTracking = true;
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return DpiHelper.IsPerMonitorV2Awareness ?
                      DpiHelper.LogicalToDeviceUnits(new Size(32, 19), DeviceDpi) :
                      new Size(32, 19);
            }
        }

        protected internal override Padding DefaultMargin
        {
            get
            {
                return Padding.Empty;
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                if (IsOnDropDown)
                {
                    return _scaledDefaultDropDownPadding;
                }
                else
                {
                    return _scaledDefaultPadding;
                }
            }
        }

        public override bool Enabled
        {
            get
            {
                if (_nativeMenuCommandID != -1)
                {
                    // if we're based off a native menu item,
                    // we need to ask it if it's enabled.
                    if (base.Enabled && _nativeMenuHandle != IntPtr.Zero && _targetWindowHandle != IntPtr.Zero)
                    {
                        return GetNativeMenuItemEnabled();
                    }

                    return false;
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
        internal Image CheckedImage
        {
            get
            {
                CheckState checkedState = CheckState;

                if (checkedState == CheckState.Indeterminate)
                {
                    if (t_indeterminateCheckedImage is null)
                    {
                        if (DpiHelper.IsScalingRequirementMet)
                        {
                            t_indeterminateCheckedImage = GetBitmapFromIcon("IndeterminateChecked", _scaledCheckMarkBitmapSize);
                        }
                        else
                        {
                            Bitmap indeterminateCheckedBmp = DpiHelper.GetBitmapFromIcon(typeof(ToolStripMenuItem), "IndeterminateChecked");
                            if (indeterminateCheckedBmp is not null)
                            {
                                if (DpiHelper.IsScalingRequired)
                                {
                                    DpiHelper.ScaleBitmapLogicalToDevice(ref indeterminateCheckedBmp);
                                }

                                t_indeterminateCheckedImage = indeterminateCheckedBmp;
                            }
                        }
                    }

                    return t_indeterminateCheckedImage;
                }
                else if (checkedState == CheckState.Checked)
                {
                    if (t_checkedImage is null)
                    {
                        if (DpiHelper.IsScalingRequirementMet)
                        {
                            t_checkedImage = GetBitmapFromIcon("Checked", _scaledCheckMarkBitmapSize);
                        }
                        else
                        {
                            Bitmap checkedBmp = DpiHelper.GetBitmapFromIcon(typeof(ToolStripMenuItem), "Checked");
                            if (checkedBmp is not null)
                            {
                                if (DpiHelper.IsScalingRequired)
                                {
                                    DpiHelper.ScaleBitmapLogicalToDevice(ref checkedBmp);
                                }

                                t_checkedImage = checkedBmp;
                            }
                        }
                    }

                    return t_checkedImage;
                }

                return null;
            }
        }

        private static Bitmap GetBitmapFromIcon(string iconName, Size desiredIconSize)
        {
            Bitmap b = null;

            Icon icon = new Icon(typeof(ToolStripMenuItem), iconName);
            if (icon is not null)
            {
                Icon desiredIcon = new Icon(icon, desiredIconSize);
                if (desiredIcon is not null)
                {
                    try
                    {
                        b = desiredIcon.ToBitmap();

                        if (b is not null)
                        {
                            if (DpiHelper.IsScalingRequired && (b.Size.Width != desiredIconSize.Width || b.Size.Height != desiredIconSize.Height))
                            {
                                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(b, desiredIconSize);
                                if (scaledBitmap is not null)
                                {
                                    b.Dispose();
                                    b = scaledBitmap;
                                }
                            }
                        }
                    }
                    finally
                    {
                        icon.Dispose();
                        desiredIcon.Dispose();
                    }
                }
            }

            return b;
        }

        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolStripButtonCheckOnClickDescr))]
        public bool CheckOnClick
        {
            get => _checkOnClick;
            set => _checkOnClick = value;
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating whether the check box is checked.
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
                //valid values are 0x0 to 0x2
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
        ///  value of the <see cref='CheckBox.Checked'/>
        ///  property changes.
        /// </summary>
        [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
        public event EventHandler CheckedChanged
        {
            add => Events.AddHandler(s_eventCheckedChanged, value);
            remove => Events.RemoveHandler(s_eventCheckedChanged, value);
        }

        /// <summary>
        ///  Occurs when the
        ///  value of the <see cref='CheckBox.CheckState'/>
        ///  property changes.
        /// </summary>
        [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
        public event EventHandler CheckStateChanged
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
                    ToolStrip owner = Owner;
                    if (owner is not null)
                    {
                        // add to the shortcut caching system.
                        if (originalShortcut != Keys.None)
                        {
                            owner.Shortcuts.Remove(originalShortcut);
                        }

                        if (owner.Shortcuts.Contains(value))
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
        public string ShortcutKeyDisplayString
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
        ///  keys that are assocaited
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
        internal Form MdiForm => Properties.ContainsObject(s_propMdiForm) ? Properties.GetObject(s_propMdiForm) as Form : null;

        internal ToolStripMenuItem Clone()
        {
            // dirt simple clone - just properties, no subitems

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
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

            // cant actually use "Visible" property as that returns whether or not the parent
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
            get => base.DeviceDpi;

            // This gets called via ToolStripItem.RescaleConstantsForDpi.
            // It's practically calling Initialize on DpiChanging with the new Dpi value.
            set
            {
                base.DeviceDpi = value;
                _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(s_defaultPadding, value);
                _scaledDefaultDropDownPadding = DpiHelper.LogicalToDeviceUnits(s_defaultDropDownPadding, value);
                _scaledCheckMarkBitmapSize = DpiHelper.LogicalToDeviceUnits(s_checkMarkBitmapSize, value);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lastOwner is not null)
                {
                    Keys shortcut = this.ShortcutKeys;
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

            var info = new User32.MENUITEMINFOW
            {
                cbSize = (uint)sizeof(User32.MENUITEMINFOW),
                fMask = User32.MIIM.STATE,
                wID = _nativeMenuCommandID
            };
            User32.GetMenuItemInfoW(new HandleRef(this, _nativeMenuHandle), _nativeMenuCommandID, /*fByPosition instead of ID=*/ BOOL.FALSE, ref info);
            return (info.fState & User32.MFS.DISABLED) == 0;
        }

        // returns text and shortcut separated by tab.
        private unsafe string GetNativeMenuItemTextAndShortcut()
        {
            if (_nativeMenuCommandID == -1 || _nativeMenuHandle == IntPtr.Zero)
            {
                Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
                return null;
            }

            string text = null;

            // fetch the string length
            var info = new User32.MENUITEMINFOW
            {
                cbSize = (uint)sizeof(User32.MENUITEMINFOW),
                fMask = User32.MIIM.STRING,
                wID = _nativeMenuCommandID
            };
            User32.GetMenuItemInfoW(new HandleRef(this, _nativeMenuHandle), _nativeMenuCommandID, /*fByPosition instead of ID=*/ BOOL.FALSE, ref info);

            if (info.cch > 0)
            {
                // fetch the string
                info.cch += 1;  // according to MSDN we need to increment the count we receive by 1.
                info.wID = _nativeMenuCommandID;
                IntPtr allocatedStringBuffer = Marshal.AllocCoTaskMem(info.cch * sizeof(char));
                info.dwTypeData = (char*)allocatedStringBuffer;

                try
                {
                    User32.GetMenuItemInfoW(new HandleRef(this, _nativeMenuHandle), _nativeMenuCommandID, /*fByPosition instead of ID=*/ BOOL.FALSE, ref info);

                    // convert the string into managed data.
                    if (info.dwTypeData is not null)
                    {
                        text = new string(info.dwTypeData, 0, info.cch);
                    }
                }
                finally
                {
                    if (allocatedStringBuffer != IntPtr.Zero)
                    {
                        // use our local instead of the info structure member *just* in case windows decides to clobber over it.
                        // we want to be sure to deallocate the memory we know we allocated.
                        Marshal.FreeCoTaskMem(allocatedStringBuffer);
                    }
                }
            }

            return text;
        }

        private unsafe Image GetNativeMenuItemImage()
        {
            if (_nativeMenuCommandID == -1 || _nativeMenuHandle == IntPtr.Zero)
            {
                Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
                return null;
            }

            var info = new User32.MENUITEMINFOW
            {
                cbSize = (uint)sizeof(User32.MENUITEMINFOW),
                fMask = User32.MIIM.BITMAP,
                wID = _nativeMenuCommandID
            };
            User32.GetMenuItemInfoW(new HandleRef(this, _nativeMenuHandle), _nativeMenuCommandID, /*fByPosition instead of ID=*/ BOOL.FALSE, ref info);

            if (info.hbmpItem != IntPtr.Zero && info.hbmpItem.ToInt32() > (int)User32.HBMMENU.POPUP_MINIMIZE)
            {
                return Bitmap.FromHbitmap(info.hbmpItem);
            }

            // its a system defined bitmap
            int buttonToUse = -1;

            switch (info.hbmpItem.ToInt32())
            {
                case (int)User32.HBMMENU.MBAR_CLOSE:
                case (int)User32.HBMMENU.MBAR_CLOSE_D:
                case (int)User32.HBMMENU.POPUP_CLOSE:
                    buttonToUse = (int)CaptionButton.Close;
                    break;
                case (int)User32.HBMMENU.MBAR_MINIMIZE:
                case (int)User32.HBMMENU.MBAR_MINIMIZE_D:
                case (int)User32.HBMMENU.POPUP_MINIMIZE:
                    buttonToUse = (int)CaptionButton.Minimize;
                    break;
                case (int)User32.HBMMENU.MBAR_RESTORE:
                case (int)User32.HBMMENU.POPUP_RESTORE:
                    buttonToUse = (int)CaptionButton.Restore;
                    break;
                case (int)User32.HBMMENU.POPUP_MAXIMIZE:
                    buttonToUse = (int)CaptionButton.Maximize;
                    break;
                case (int)User32.HBMMENU.SYSTEM:
                //
                case (int)User32.HBMMENU.CALLBACK:
                // owner draw not supported
                default:
                    break;
            }

            if (buttonToUse > -1)
            {
                // we've mapped to a system defined bitmap we know how to draw
                Bitmap image = new Bitmap(16, 16);

                using (Graphics g = Graphics.FromImage(image))
                {
                    ControlPaint.DrawCaptionButton(g, new Rectangle(Point.Empty, image.Size), (CaptionButton)buttonToUse, ButtonState.Flat);
                    g.DrawRectangle(SystemPens.Control, 0, 0, image.Width - 1, image.Height - 1);
                }

                image.MakeTransparent(SystemColors.Control);
                return image;
            }

            return null;
        }

        internal Size GetShortcutTextSize()
        {
            if (!ShowShortcutKeys)
            {
                return Size.Empty;
            }

            string shortcutString = GetShortcutText();
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

        internal string GetShortcutText()
        {
            if (_cachedShortcutText is null)
            {
                _cachedShortcutText = ShortcutToText(ShortcutKeys, ShortcutKeyDisplayString);
            }

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
                User32.PostMessageW(new HandleRef(this, _targetWindowHandle), User32.WM.SYSCOMMAND, (IntPtr)_nativeMenuCommandID);
            }
            else
            {
                // These are user added items like ".Net Window..."

                // be consistent with sending a WM_SYSCOMMAND, use POST not SEND.
                User32.PostMessageW(new HandleRef(this, _targetWindowHandle), User32.WM.COMMAND, (IntPtr)_nativeMenuCommandID);
            }

            Invalidate();
        }

        /// <summary>
        ///  Raises the <see cref='CheckedChanged'/>
        ///  event.
        /// </summary>
        protected virtual void OnCheckedChanged(EventArgs e)
        {
            ((EventHandler)Events[s_eventCheckedChanged])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='CheckStateChanged'/> event.
        /// </summary>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            AccessibilityNotifyClients(AccessibleEvents.StateChange);
            ((EventHandler)Events[s_eventCheckStateChanged])?.Invoke(this, e);
        }

        protected override void OnDropDownHide(EventArgs e)
        {
            Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnDropDownHide] MenuTimer.Cancel called");
            MenuTimer.Cancel(this);
            base.OnDropDownHide(e);
        }

        protected override void OnDropDownShow(EventArgs e)
        {
            // if someone has beaten us to the punch by arrowing around
            // cancel the current menu timer.
            Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnDropDownShow] MenuTimer.Cancel called");
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
            // Opening should happen on mouse down
            // we use a mouse down ID to ensure that the reshow

            Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnMouseDown] MenuTimer.Cancel called");
            MenuTimer.Cancel(this);
            OnMouseButtonStateChange(e, /*isMouseDown=*/true);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            OnMouseButtonStateChange(e, /*isMouseDown=*/false);
            base.OnMouseUp(e);
        }

        private void OnMouseButtonStateChange(MouseEventArgs e, bool isMouseDown)
        {
            bool showDropDown = true;
            if (IsOnDropDown)
            {
                ToolStripDropDown dropDown = GetCurrentParentDropDown() as ToolStripDropDown;

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
                Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "received mouse enter - calling drop down");

                Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnMouseEnter] MenuTimer.Cancel / MenuTimer.Start called");

                MenuTimer.Cancel(this);
                MenuTimer.Start(this);
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Debug.WriteLineIf(ToolStrip.s_menuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnMouseLeave] MenuTimer.Cancel called");
            MenuTimer.Cancel(this);
            base.OnMouseLeave(e);
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            Keys shortcut = ShortcutKeys;
            if (shortcut != Keys.None)
            {
                if (_lastOwner is not null)
                {
                    _lastOwner.Shortcuts.Remove(shortcut);
                }

                if (Owner is not null)
                {
                    if (Owner.Shortcuts.Contains(shortcut))
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

            ToolStripRenderer renderer = Renderer;
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

        /// <summary>
        ///  handle shortcut keys here.
        /// </summary>
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

                DropDown.SelectNextToolStripItem(null, /*forward=*/true);
                return true;
            }

            return base.ProcessMnemonic(charCode);
        }

        /// <summary> overridden here so we scooch over when we're in the ToolStripDropDownMenu</summary>
        internal protected override void SetBounds(Rectangle rect)
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
        internal void SetNativeTargetMenu(IntPtr hMenu)
        {
            _nativeMenuHandle = hMenu;
        }

        internal static string ShortcutToText(Keys shortcutKeys, string shortcutKeyDisplayString)
        {
            if (!string.IsNullOrEmpty(shortcutKeyDisplayString))
            {
                return shortcutKeyDisplayString;
            }

            if (shortcutKeys == Keys.None)
            {
                return string.Empty;
            }

            return TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(shortcutKeys);
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
}
