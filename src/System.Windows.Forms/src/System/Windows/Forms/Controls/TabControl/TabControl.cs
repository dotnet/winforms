// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms.Layout;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  The TabControl.  This control has a lot of the functionality of a TabStrip
///  but manages a list of TabPages which are the 'pages' that appear on each tab.
/// </summary>
[DefaultProperty(nameof(TabPages))]
[DefaultEvent(nameof(SelectedIndexChanged))]
[Designer($"System.Windows.Forms.Design.TabControlDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionTabControl))]
public partial class TabControl : Control
{
    private static readonly Size s_defaultItemSize = Size.Empty;
    private static readonly Point s_defaultPaddingPoint = new(6, 3);

    // Properties
    private readonly TabPageCollection _tabCollection;
    private TabAlignment _alignment = TabAlignment.Top;
    private TabDrawMode _drawMode = TabDrawMode.Normal;
    private ImageList? _imageList;
    private Size _itemSize = s_defaultItemSize;
    private Point _padding = s_defaultPaddingPoint;
    private TabSizeMode _sizeMode = TabSizeMode.Normal;
    private TabAppearance _appearance = TabAppearance.Normal;
    private Rectangle _cachedDisplayRect;
    private bool _currentlyScaling;
    private int _selectedIndex = -1;
    private string? _controlTipText = string.Empty;
    private bool _handleInTable;

    // Events
    private EventHandler? _onSelectedIndexChanged;
    private DrawItemEventHandler? _onDrawItem;

    private static readonly object s_deselectingEvent = new();
    private static readonly object s_deselectedEvent = new();
    private static readonly object s_selectingEvent = new();
    private static readonly object s_selectedEvent = new();
    private static readonly object s_rightToLeftLayoutChangedEvent = new();

    // Perf: take all the bools and put them into a state variable: see TabControlState consts above
    private BitVector32 _tabControlState;

    private const string TabBaseReLayoutMessageName = "_TabBaseReLayout";

    /// <summary>
    ///  This message is posted by the control to itself after a TabPage is
    ///  added to it.  On certain occasions, after items are added to a
    ///  TabControl in quick succession, TCM_ADJUSTRECT calls return the wrong
    ///  display rectangle.  When the message is received, the control calls
    ///  updateTabSelection() to layout the TabPages correctly.
    /// </summary>
    private readonly MessageId _tabBaseReLayoutMessage = PInvoke.RegisterWindowMessage($"{Application.WindowMessagesVersion}{TabBaseReLayoutMessageName}");

    // State
    private readonly List<TabPage> _tabPages = [];
    private int _lastSelection;
    private short _windowId;

    private bool _rightToLeftLayout;
    private bool _skipUpdateSize;

    private ToolTipBuffer _toolTipBuffer;

    /// <summary>
    ///  Constructs a TabBase object, usually as the base class for a TabStrip or TabControl.
    /// </summary>
    public TabControl()
        : base()
    {
        _tabControlState = new BitVector32(0x00000000);

        _tabCollection = new TabPageCollection(this);
        SetStyle(ControlStyles.UserPaint, false);
    }

    /// <summary>
    ///  Returns on what area of the control the tabs reside on (A TabAlignment value).
    ///  The possibilities are Top (the default), Bottom, Left, and Right.  When alignment
    ///  is left or right, the Multiline property is ignored and Multiline is implicitly on.
    ///  If the alignment is anything other than top, TabAppearance.FlatButtons degenerates
    ///  to TabAppearance.Buttons.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(TabAlignment.Top)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.TabBaseAlignmentDescr))]
    public TabAlignment Alignment
    {
        get
        {
            return _alignment;
        }

        set
        {
            if (_alignment != value)
            {
                // valid values are 0x0 to 0x3
                SourceGenerated.EnumValidator.Validate(value);

                _alignment = value;
                if (_alignment is TabAlignment.Left or TabAlignment.Right)
                {
                    SetState(State.Multiline, true);
                }

                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Indicates whether the tabs in the tabstrip look like regular tabs, or if they look
    ///  like buttons as seen in the Windows 95 taskbar.
    ///  If the alignment is anything other than top, TabAppearance.FlatButtons degenerates
    ///  to TabAppearance.Buttons.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(TabAppearance.Normal)]
    [SRDescription(nameof(SR.TabBaseAppearanceDescr))]
    public TabAppearance Appearance
    {
        get
        {
            if (_appearance == TabAppearance.FlatButtons && _alignment != TabAlignment.Top)
            {
                return TabAppearance.Buttons;
            }
            else
            {
                return _appearance;
            }
        }

        set
        {
            if (_appearance != value)
            {
                // valid values are 0x0 to 0x2
                SourceGenerated.EnumValidator.Validate(value);

                _appearance = value;
                RecreateHandle();

                // Fire OnStyleChanged(EventArgs.Empty) here since we are no longer calling UpdateStyles( ) but always reCreating the Handle.
                OnStyleChanged(EventArgs.Empty);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get
        {
            // The tab control can only be rendered in 1 color: System's Control color.
            // So, always return this value... otherwise, we're inheriting the forms BackColor
            // and passing it on to the pab pages.
            return Application.SystemColors.Control;
        }
        set
        {
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackColorChanged
    {
        add => base.BackColorChanged += value;
        remove => base.BackColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize
    {
        get
        {
            return new Size(200, 100);
        }
    }

    /// <summary>
    ///  This property is overridden and hidden from statement completion
    ///  on controls that are based on Win32 Native Controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool DoubleBuffered
    {
        get => base.DoubleBuffered;
        set => base.DoubleBuffered = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    /// <summary>
    ///  Returns the parameters needed to create the handle.  Inheriting classes
    ///  can override this to provide extra functionality.  They should not,
    ///  however, forget to call base.getCreateParams() first to get the struct
    ///  filled up with the basic info.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_TABCONTROL;

            // set up window styles
            //
            if (Multiline)
            {
                cp.Style |= (int)PInvoke.TCS_MULTILINE;
            }

            if (_drawMode == TabDrawMode.OwnerDrawFixed)
            {
                cp.Style |= (int)PInvoke.TCS_OWNERDRAWFIXED;
            }

            if (ShowToolTips && !DesignMode)
            {
                cp.Style |= (int)PInvoke.TCS_TOOLTIPS;
            }

            if (_alignment is TabAlignment.Bottom or
                TabAlignment.Right)
            {
                cp.Style |= (int)PInvoke.TCS_BOTTOM;
            }

            if (_alignment is TabAlignment.Left or
                TabAlignment.Right)
            {
                cp.Style |= (int)PInvoke.TCS_VERTICAL | (int)PInvoke.TCS_MULTILINE;
            }

            if (GetState(State.HotTrack))
            {
                cp.Style |= (int)PInvoke.TCS_HOTTRACK;
            }

            if (_appearance == TabAppearance.Normal)
            {
                cp.Style |= (int)PInvoke.TCS_TABS;
            }
            else
            {
                cp.Style |= (int)PInvoke.TCS_BUTTONS;
                if (_appearance == TabAppearance.FlatButtons && _alignment == TabAlignment.Top)
                {
                    cp.Style |= (int)PInvoke.TCS_FLATBUTTONS;
                }
            }

            switch (_sizeMode)
            {
                case TabSizeMode.Normal:
                    cp.Style |= (int)PInvoke.TCS_RAGGEDRIGHT;
                    break;
                case TabSizeMode.FillToRight:
                    cp.Style |= (int)PInvoke.TCS_RIGHTJUSTIFY;
                    break;
                case TabSizeMode.Fixed:
                    cp.Style |= (int)PInvoke.TCS_FIXEDWIDTH;
                    break;
            }

            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                // We want to turn on mirroring for Form explicitly.
                cp.ExStyle |= (int)(WINDOW_EX_STYLE.WS_EX_LAYOUTRTL | WINDOW_EX_STYLE.WS_EX_NOINHERITLAYOUT);
                // Don't need these styles when mirroring is turned on.
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;
        }
    }

    /// <summary>
    ///  The rectangle that represents the Area of the tab strip not
    ///  taken up by the tabs, borders, or anything else owned by the Tab.  This
    ///  is typically the rectangle you want to use to place the individual
    ///  children of the tab strip.
    /// </summary>
    public override Rectangle DisplayRectangle
    {
        get
        {
            // Set the cached display rect to Rectangle.Empty whenever we do anything to change it.
            if (!_cachedDisplayRect.IsEmpty)
            {
                return _cachedDisplayRect;
            }

            RECT rect = Bounds;

            // We force a handle creation here, because otherwise the DisplayRectangle will be wildly inaccurate
            if (!IsDisposed)
            {
                // Since this is called thru the OnResize (and Layout) which is triggered by SetExtent if the TabControl is hosted as
                // a ActiveX control, so check if this is ActiveX and don't force Handle Creation here as the native code breaks in this case.
                if (!IsActiveX)
                {
                    if (!IsHandleCreated)
                    {
                        CreateHandle();
                    }
                }

                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.TCM_ADJUSTRECT, 0, ref rect);
                }
            }

            Rectangle r = rect;

            Point p = Location;
            r.X -= p.X;
            r.Y -= p.Y;

            _cachedDisplayRect = r;
            return r;
        }
    }

    /// <summary>
    ///  The drawing mode of the tabs in the tab strip.  This will indicate
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(TabDrawMode.Normal)]
    [SRDescription(nameof(SR.TabBaseDrawModeDescr))]
    public TabDrawMode DrawMode
    {
        get
        {
            return _drawMode;
        }

        set
        {
            // valid values are 0x0 to 0x1
            SourceGenerated.EnumValidator.Validate(value);

            if (_drawMode != value)
            {
                _drawMode = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Indicates whether the tabs visually change when the mouse passes over them.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TabBaseHotTrackDescr))]
    public bool HotTrack
    {
        get => GetState(State.HotTrack);
        set
        {
            if (HotTrack != value)
            {
                SetState(State.HotTrack, value);
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  Returns the imageList the control points at.  This is where tabs that have imageIndex
    ///  set will get there images from.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.TabBaseImageListDescr))]
    public ImageList? ImageList
    {
        get
        {
            return _imageList;
        }
        set
        {
            if (_imageList != value)
            {
                EventHandler recreateHandler = new(ImageListRecreateHandle);
                EventHandler disposedHandler = new(DetachImageList);

                if (_imageList is not null)
                {
                    _imageList.RecreateHandle -= recreateHandler;
                    _imageList.Disposed -= disposedHandler;
                }

                _imageList = value;
                IntPtr handle = (value is not null) ? value.Handle : IntPtr.Zero;
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.TCM_SETIMAGELIST, 0, handle);
                }

                // Update the image list in the tab pages.
                foreach (TabPage tabPage in TabPages)
                {
                    tabPage.ImageIndexer.ImageList = value;
                }

                if (value is not null)
                {
                    value.RecreateHandle += recreateHandler;
                    value.Disposed += disposedHandler;
                }
            }
        }
    }

    /// <summary>
    ///  By default, tabs will automatically size themselves to fit their icon, if any, and their label.
    ///  However, the tab size can be explicitly set by setting this property.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.TabBaseItemSizeDescr))]
    public Size ItemSize
    {
        get
        {
            if (_itemSize.IsEmpty)
            {
                // Obtain the current itemsize of the first tab from the winctl control
                if (IsHandleCreated)
                {
                    SetState(State.GetTabRectfromItemSize, true);
                    return GetTabRect(0).Size;
                }

                return s_defaultItemSize;
            }

            return _itemSize;
        }
        set
        {
            if (value.Width < 0 || value.Height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(ItemSize), value));
            }

            _itemSize = value;
            ApplyItemSize();
            UpdateSize();
            Invalidate();
        }
    }

    /// <summary>
    ///  This private property is set by the TabPageCollection when the user calls "InsertItem".
    ///  The problem is when InsertItem is called then we add this item to the ControlsCollection (in addition to the TabPageCollection)
    ///  to keep both the collections is sync. But the controlCollection.Add is overriden to again ADD the item to the TabPageCollection.
    ///  So we keep this flag in order to avoid repeated addition (only during insert)
    ///  When the Add ends ... we reset this flag.
    /// </summary>
    private bool InsertingItem
    {
        get => GetState(State.InsertingItem);
        set => SetState(State.InsertingItem, value);
    }

    /// <summary>
    ///  Indicates if there can be more than one row of tabs.  By default [when
    ///  this property is false], if there are more tabs than available display
    ///  space, arrows are shown to let the user navigate between the extra
    ///  tabs, but only one row is shown.  If this property is set to true, then
    ///  Windows spills extra tabs over on to second rows.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TabBaseMultilineDescr))]
    public bool Multiline
    {
        get => GetState(State.Multiline);
        set
        {
            if (Multiline != value)
            {
                SetState(State.Multiline, value);
                if (!Multiline && (_alignment == TabAlignment.Left || _alignment == TabAlignment.Right))
                {
                    _alignment = TabAlignment.Top;
                }

                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  The amount of padding around the items in the individual tabs.
    ///  You can specify both horizontal and vertical padding.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.TabBasePaddingDescr))]
    public new Point Padding
    {
        get
        {
            return _padding;
        }
        set
        {
            if (value.X < 0 || value.Y < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(Padding), value));
            }

            if (_padding != value)
            {
                _padding = value;
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  This is used for international applications where the language is written from RightToLeft.
    ///  When this property is true, and the RightToLeft is true, mirroring will be turned on on
    ///  the form, and control placement and text will be from right to left.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))]
    public virtual bool RightToLeftLayout
    {
        get
        {
            return _rightToLeftLayout;
        }

        set
        {
            if (value != _rightToLeftLayout)
            {
                _rightToLeftLayout = value;
                using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                {
                    OnRightToLeftLayoutChanged(EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///  The number of rows currently being displayed in
    ///  the tab strip.  This is most commonly used when the Multiline property
    ///  is 'true' and you want to know how many rows the tabs are currently
    ///  taking up.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TabBaseRowCountDescr))]
    public int RowCount
        => (int)PInvoke.SendMessage(this, PInvoke.TCM_GETROWCOUNT);

    /// <summary>
    ///  The index of the currently selected tab in the strip, if there
    ///  is one.  If the value is -1, there is currently no selection.  If the
    ///  value is 0 or greater, than the value is the index of the currently
    ///  selected tab.
    /// </summary>
    [Browsable(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(-1)]
    [SRDescription(nameof(SR.selectedIndexDescr))]
    public int SelectedIndex
    {
        get => IsHandleCreated ? (int)PInvoke.SendMessage(this, PInvoke.TCM_GETCURSEL) : _selectedIndex;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);

            if (SelectedIndex != value)
            {
                if (IsHandleCreated)
                {
                    // Guard Against CreateHandle ..
                    // And also if we are setting SelectedIndex ourselves from SelectNextTab..
                    if (!GetState(State.FromCreateHandles) && !GetState(State.SelectFirstControl))
                    {
                        SetState(State.UISelection, true);
                        // Fire Deselecting .. Deselected on currently selected TabPage...
                        if (WmSelChanging())
                        {
                            SetState(State.UISelection, false);
                            return;
                        }

                        if (ValidationCancelled)
                        {
                            SetState(State.UISelection, false);
                            return;
                        }
                    }

                    PInvoke.SendMessage(this, PInvoke.TCM_SETCURSEL, (WPARAM)value);

                    if (!GetState(State.FromCreateHandles) && !GetState(State.SelectFirstControl))
                    {
                        // Fire Selecting & Selected .. Also if Selecting is Canceled..
                        // then return as we do not change the SelectedIndex...
                        SetState(State.SelectFirstControl, true);
                        if (WmSelChange())
                        {
                            SetState(State.UISelection, false);
                            SetState(State.SelectFirstControl, false);
                            return;
                        }
                        else
                        {
                            SetState(State.SelectFirstControl, false);
                        }
                    }
                }
                else
                {
                    _selectedIndex = value;
                }
            }
        }
    }

    /// <summary>
    ///  The selection to the given tab, provided it .equals a tab in the
    ///  list.  The return value is the index of the tab that was selected,
    ///  or -1 if no tab was selected.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TabControlSelectedTabDescr))]
    public TabPage? SelectedTab
    {
        get
        {
            int index = SelectedIndex;
            if (index == -1 || _tabPages.Count == 0)
            {
                return null;
            }

            return _tabPages[index];
        }
        set
        {
            int index = FindTabPage(value);
            SelectedIndex = index;
        }
    }

    /// <summary>
    ///  By default, tabs are big enough to display their text, and any space
    ///  on the right of the strip is left as such.  However, you can also
    ///  set it such that the tabs are stretched to fill out the right extent
    ///  of the strip, if necessary, or you can set it such that all tabs
    ///  the same width.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(TabSizeMode.Normal)]
    [SRDescription(nameof(SR.TabBaseSizeModeDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public TabSizeMode SizeMode
    {
        get => _sizeMode;
        set
        {
            if (_sizeMode == value)
            {
                return;
            }

            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            _sizeMode = value;
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Indicates whether tooltips are being shown for tabs that have tooltips set on
    ///  them.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [Localizable(true)]
    [SRDescription(nameof(SR.TabBaseShowToolTipsDescr))]
    public bool ShowToolTips
    {
        get => GetState(State.ShowToolTips);
        set
        {
            if (ShowToolTips != value)
            {
                SetState(State.ShowToolTips, value);
                RecreateHandle();
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Returns the number of tabs in the strip
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TabBaseTabCountDescr))]
    public int TabCount => _tabPages?.Count ?? 0;

    /// <summary>
    ///  Returns the Collection of TabPages.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TabControlTabsDescr))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Editor($"System.Windows.Forms.Design.TabPageCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MergableProperty(false)]
    public TabPageCollection TabPages
    {
        get
        {
            return _tabCollection;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.drawItemEventDescr))]
    public event DrawItemEventHandler? DrawItem
    {
        add => _onDrawItem += value;
        remove => _onDrawItem -= value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => Events.AddHandler(s_rightToLeftLayoutChangedEvent, value);
        remove => Events.RemoveHandler(s_rightToLeftLayoutChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
    public event EventHandler? SelectedIndexChanged
    {
        add => _onSelectedIndexChanged += value;
        remove => _onSelectedIndexChanged -= value;
    }

    /// <summary>
    ///  Occurs before a tabpage is selected as the top tabPage.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.TabControlSelectingEventDescr))]
    public event TabControlCancelEventHandler? Selecting
    {
        add => Events.AddHandler(s_selectingEvent, value);
        remove => Events.RemoveHandler(s_selectingEvent, value);
    }

    /// <summary>
    ///  Occurs after a tabpage is selected as the top tabPage.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.TabControlSelectedEventDescr))]
    public event TabControlEventHandler? Selected
    {
        add => Events.AddHandler(s_selectedEvent, value);
        remove => Events.RemoveHandler(s_selectedEvent, value);
    }

    /// <summary>
    ///  Occurs before the visible property of the top tabpage is set to false.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.TabControlDeselectingEventDescr))]
    public event TabControlCancelEventHandler? Deselecting
    {
        add => Events.AddHandler(s_deselectingEvent, value);
        remove => Events.RemoveHandler(s_deselectingEvent, value);
    }

    /// <summary>
    ///  Occurs after the visible property of the top tabpage is set to false.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.TabControlDeselectedEventDescr))]
    public event TabControlEventHandler? Deselected
    {
        add => Events.AddHandler(s_deselectedEvent, value);
        remove => Events.RemoveHandler(s_deselectedEvent, value);
    }

    /// <summary>
    ///  TabControl Onpaint.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    private int AddTabPage(TabPage tabPage)
    {
        int index = AddNativeTabPage(tabPage);
        if (index >= 0)
        {
            Insert(index, tabPage);
        }

        return index;
    }

    private int AddNativeTabPage(TabPage tabPage)
    {
        int index = SendMessage(PInvoke.TCM_INSERTITEMW, TabCount + 1, tabPage);
        PInvoke.PostMessage(this, _tabBaseReLayoutMessage);
        return index;
    }

    internal void ApplyItemSize()
    {
        if (IsHandleCreated && ShouldSerializeItemSize())
        {
            PInvoke.SendMessage(this, PInvoke.TCM_SETITEMSIZE, 0, PARAM.FromLowHigh(_itemSize.Width, _itemSize.Height));
        }

        _cachedDisplayRect = Rectangle.Empty;
    }

    internal void BeginUpdate()
    {
        BeginUpdateInternal();
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new TabControlAccessibleObject(this);

    protected override Control.ControlCollection CreateControlsInstance()
    {
        return new ControlCollection(this);
    }

    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX()
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_TAB_CLASSES
            });
        }

        base.CreateHandle();
    }

    private void DetachImageList(object? sender, EventArgs e) => ImageList = null;

    /// <summary>
    ///  Allows the user to specify the index in Tabcontrol.TabPageCollection of the tabpage to be hidden.
    /// </summary>
    public void DeselectTab(int index)
    {
        TabPage t = GetTabPage(index);
        if (SelectedTab == t)
        {
            if (index >= 0 && index < TabPages.Count - 1)
            {
                SelectedTab = GetTabPage(++index);
            }
            else
            {
                SelectedTab = GetTabPage(0);
            }
        }
    }

    /// <summary>
    ///  Allows the user to specify the tabpage in Tabcontrol.TabPageCollection  to be hidden.
    /// </summary>
    public void DeselectTab(TabPage tabPage)
    {
        ArgumentNullException.ThrowIfNull(tabPage);

        int index = FindTabPage(tabPage);
        DeselectTab(index);
    }

    /// <summary>
    ///  Allows the user to specify the name of the tabpage in Tabcontrol.TabPageCollection to be hidden.
    /// </summary>
    public void DeselectTab(string tabPageName)
    {
        ArgumentNullException.ThrowIfNull(tabPageName);

        TabPage tabPage = TabPages[tabPageName]!;
        DeselectTab(tabPage);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_imageList is not null)
            {
                _imageList.Disposed -= new EventHandler(DetachImageList);
            }
        }

        base.Dispose(disposing);
    }

    internal void EndUpdate()
    {
        EndUpdate(true);
    }

    internal void EndUpdate(bool invalidate)
    {
        EndUpdateInternal(invalidate);
    }

    internal int FindTabPage(TabPage? tabPage)
    {
        for (int i = 0; i < _tabPages.Count; i++)
        {
            if (_tabPages[i].Equals(tabPage))
            {
                return i;
            }
        }

        return -1;
    }

    public Control GetControl(int index)
    {
        return GetTabPage(index);
    }

    internal TabPage GetTabPage(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, TabCount);

        return _tabPages[index];
    }

    /// <summary>
    ///  This has package scope so that TabStrip and TabControl can call it.
    /// </summary>
    protected virtual object[] GetItems()
    {
        if (_tabPages.Count > 0)
        {
            return _tabPages.ToArray();
        }

        return Array.Empty<TabPage>();
    }

    /// <summary>
    ///  This has package scope so that TabStrip and TabControl can call it.
    /// </summary>
    protected virtual object[] GetItems(Type baseType)
    {
        int tabPageCount = TabCount;
        object[] result = (object[])Array.CreateInstance(baseType, tabPageCount);
        if (tabPageCount > 0)
        {
            for (int i = 0; i < tabPageCount; i++)
            {
                result[i] = _tabPages[i];
            }
        }

        return result;
    }

    internal TabPage[] GetTabPages()
    {
        return (TabPage[])GetItems();
    }

    /// <summary>
    ///  Retrieves the bounding rectangle for the given tab in the tab strip.
    /// </summary>
    public Rectangle GetTabRect(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        if (!GetState(State.GetTabRectfromItemSize))
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, TabCount);
        }

        SetState(State.GetTabRectfromItemSize, false);
        RECT rect = default;

        // normally, we would not want to create the handle for this, but since
        // it is dependent on the actual physical display, we simply must.
        if (!IsHandleCreated)
        {
            CreateHandle();
        }

        PInvoke.SendMessage(this, PInvoke.TCM_GETITEMRECT, (WPARAM)index, ref rect);
        return rect;
    }

    protected string GetToolTipText(object item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item is not TabPage tabPage)
        {
            throw new ArgumentException(SR.TabControlBadControl, nameof(item));
        }

        return tabPage.ToolTipText;
    }

    private void ImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.TCM_SETIMAGELIST, 0, ImageList!.Handle);
        }
    }

    internal void Insert(int index, TabPage tabPage)
    {
        _tabPages.Insert(index, tabPage);

        _cachedDisplayRect = Rectangle.Empty;
        ApplyItemSize();
        if (Appearance == TabAppearance.FlatButtons)
        {
            Invalidate();
        }
    }

    /// <summary>
    ///  This function is used by the Insert Logic to insert a tabPage in the current TabPage in the TabPageCollection.
    /// </summary>
    private void InsertItem(int index, TabPage tabPage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, TabCount);
        ArgumentNullException.ThrowIfNull(tabPage);

        index = IsHandleCreated ? SendMessage(PInvoke.TCM_INSERTITEMW, index, tabPage) : index;
        if (index >= 0)
        {
            Insert(index, tabPage);
        }
    }

    /// <summary>
    ///  Handling special input keys, such as pgup, pgdown, home, end, etc...
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.Alt) == Keys.Alt)
        {
            return false;
        }

        return (keyData & Keys.KeyCode) switch
        {
            Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End => true,
            _ => base.IsInputKey(keyData),
        };
    }

    private static void NotifyAboutFocusState(TabPage? selectedTab, bool focused)
    {
        if (selectedTab is null)
        {
            return;
        }

        if (focused)
        {
            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(selectedTab);
        }
        else
        {
            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(selectedTab);
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        NotifyAboutFocusState(SelectedTab, focused: true);
        base.OnGotFocus(e);

        if (IsAccessibilityObjectCreated && SelectedTab is not null)
        {
            SelectedTab.TabAccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    /// <summary>
    ///  This is a notification that the handle has been created.
    ///  We do some work here to configure the handle.
    ///  Overriders should call base.OnHandleCreated()
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        if (!IsHandleCreated)
        {
            base.OnHandleCreated(e);
            return;
        }

        // Add the handle to HashTable for Ids ..
        _windowId = NativeWindow.CreateWindowId(this);
        _handleInTable = true;

        // Set the padding BEFORE setting the control's font (as done
        // in base.OnHandleCreated()) so that the tab control will honor both the
        // horizontal and vertical dimensions of the padding rectangle.
        if (!_padding.IsEmpty)
        {
            PInvoke.SendMessage(this, PInvoke.TCM_SETPADDING, 0, PARAM.FromPoint(_padding));
        }

        base.OnHandleCreated(e);
        _cachedDisplayRect = Rectangle.Empty;
        ApplyItemSize();

        if (_imageList is not null)
        {
            PInvoke.SendMessage(this, PInvoke.TCM_SETIMAGELIST, 0, _imageList.Handle);
        }

        if (ShowToolTips)
        {
            HWND tooltipHwnd = (HWND)PInvoke.SendMessage(this, PInvoke.TCM_GETTOOLTIPS);
            if (!tooltipHwnd.IsNull)
            {
                PInvoke.SetWindowPos(
                    this,
                    HWND.HWND_TOPMOST,
                    0, 0, 0, 0,
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
            }
        }

        // Add the pages
        //
        foreach (TabPage page in TabPages)
        {
            AddNativeTabPage(page);
        }

        // Resize the pages
        //
        ResizePages();

        if (_selectedIndex != -1)
        {
            try
            {
                SetState(State.FromCreateHandles, true);
                SelectedIndex = _selectedIndex;
            }
            finally
            {
                SetState(State.FromCreateHandles, false);
            }

            _selectedIndex = -1;
        }

        UpdateTabSelection(false);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (!Disposing)
        {
            _selectedIndex = SelectedIndex;
        }

        // Remove the handle from NativeWindow.
        if (_handleInTable)
        {
            _handleInTable = false;
            NativeWindow.RemoveWindowFromIDTable(_windowId);
        }

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    ///  Actually goes and fires the OnDrawItem event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event].  They should,
    ///  however, remember to call base.onDrawItem(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected virtual void OnDrawItem(DrawItemEventArgs e)
    {
        _onDrawItem?.Invoke(this, e);
    }

    /// <summary>
    ///  Actually goes and fires the OnLeave event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event].  They should,
    ///  however, remember to call base.OnLeave(e); to ensure the event is
    ///  still fired to external listeners
    ///  This listener is overidden so that we can fire SAME ENTER and LEAVE
    ///  events on the TabPage.
    ///  TabPage should fire enter when the focus is on the TABPAGE and not when the control
    ///  within the TabPage gets Focused.
    ///  Similary the Leave event should fire when the TabControl (and hence the TabPage) looses
    ///  Focus. To be Backward compatible we have added new bool which can be set to true
    ///  to the get the NEW SANE ENTER-LEAVE EVENTS ON THE TABPAGE.
    /// </summary>
    protected internal override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        SelectedTab?.FireEnter(e);
    }

    /// <summary>
    ///  Actually goes and fires the OnLeave event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event].  They should,
    ///  however, remember to call base.OnLeave(e); to ensure the event is
    ///  still fired to external listeners
    ///  This listener is overidden so that we can fire SAME ENTER and LEAVE
    ///  events on the TabPage.
    ///  TabPage should fire enter when the focus is on the TABPAGE and not when the control
    ///  within the TabPage gets Focused.
    ///  Similary the Leave event  should fire when the TabControl (and hence the TabPage) looses
    ///  Focus. To be Backward compatible we have added new bool which can be set to true
    ///  to the get the NEW SANE ENTER-LEAVE EVENTS ON THE TABPAGE.
    /// </summary>
    protected internal override void OnLeave(EventArgs e)
    {
        SelectedTab?.FireLeave(e);

        base.OnLeave(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        NotifyAboutFocusState(SelectedTab, focused: false);
        base.OnLostFocus(e);
    }

    /// <summary>
    ///  We override this to get tabbing functionality.
    ///  If overriding this, remember to call base.onKeyDown.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs ke)
    {
        if (ke.KeyCode == Keys.Tab && (ke.KeyData & Keys.Control) != 0)
        {
            bool forward = (ke.KeyData & Keys.Shift) == 0;
            SelectNextTab(ke, forward);
        }

        if (ke.KeyCode == Keys.PageDown && (ke.KeyData & Keys.Control) != 0)
        {
            SelectNextTab(ke, true);
        }

        if (ke.KeyCode == Keys.PageUp && (ke.KeyData & Keys.Control) != 0)
        {
            SelectNextTab(ke, false);
        }

        base.OnKeyDown(ke);
    }

    internal override void OnParentHandleRecreated()
    {
        // Avoid temporarily resizing the TabControl while the parent
        // recreates its handle to avoid
        _skipUpdateSize = true;
        try
        {
            base.OnParentHandleRecreated();
        }
        finally
        {
            _skipUpdateSize = false;
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        _cachedDisplayRect = Rectangle.Empty;
        UpdateTabSelection(false);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        if (RightToLeft == RightToLeft.Yes)
        {
            RecreateHandle();
        }

        if (Events[s_rightToLeftLayoutChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Actually goes and fires the onSelectedIndexChanged event.  Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event].  They should,
    ///  however, remember to call base.onSelectedIndexChanged(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected virtual void OnSelectedIndexChanged(EventArgs e)
    {
        int index = SelectedIndex;
        _cachedDisplayRect = Rectangle.Empty;
        UpdateTabSelection(GetState(State.UISelection));
        SetState(State.UISelection, false);
        _onSelectedIndexChanged?.Invoke(this, e);
        KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
        NotifyAboutFocusState(SelectedTab, focused: true);
    }

    /// <summary>
    ///  Raises the <see cref="OnSelecting"/> event.
    /// </summary>
    protected virtual void OnSelecting(TabControlCancelEventArgs e)
    {
        ((TabControlCancelEventHandler?)Events[s_selectingEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="OnSelected"/> event.
    /// </summary>
    protected virtual void OnSelected(TabControlEventArgs e)
    {
        ((TabControlEventHandler?)Events[s_selectedEvent])?.Invoke(this, e);

        // Raise the enter event for this tab.
        SelectedTab?.FireEnter(EventArgs.Empty);
    }

    /// <summary>
    ///  Raises the <see cref="OnDeselecting"/> event.
    /// </summary>
    protected virtual void OnDeselecting(TabControlCancelEventArgs e)
    {
        ((TabControlCancelEventHandler?)Events[s_deselectingEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="OnDeselected"/> event.
    /// </summary>
    protected virtual void OnDeselected(TabControlEventArgs e)
    {
        ((TabControlEventHandler?)Events[s_deselectedEvent])?.Invoke(this, e);

        // Raise the Leave event for this tab.
        if (SelectedTab is not null)
        {
            NotifyAboutFocusState(SelectedTab, focused: false);
            SelectedTab.FireLeave(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  We override this to get the Ctrl and Ctrl-Shift Tab functionality.
    /// </summary>
    protected override bool ProcessKeyPreview(ref Message m)
    {
        if (ProcessKeyEventArgs(ref m))
        {
            return true;
        }

        return base.ProcessKeyPreview(ref m);
    }

    internal void UpdateSize()
    {
        if (_skipUpdateSize)
        {
            return;
        }

        // the spin control (left right arrows) won't update without resizing.
        // the most correct thing would be to recreate the handle, but this works
        // and is cheaper.
        BeginUpdate();
        Size size = Size;
        Size = new Size(size.Width + 1, size.Height);
        Size = size;
        EndUpdate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        _cachedDisplayRect = Rectangle.Empty;
        UpdateSize();
    }

    internal override void RecreateHandleCore()
    {
        TabPage[] tabPages = GetTabPages();

        int index = ((tabPages.Length > 0) && (SelectedIndex == -1)) ? 0 : SelectedIndex;

        // We don't actually want to remove the windows forms Tabpages - we only
        // want to remove the corresponding TCITEM structs.
        // So, no RemoveAll()
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.TCM_DELETEALLITEMS);
        }

        _tabPages.Clear();

        base.RecreateHandleCore();

        for (int i = 0; i < tabPages.Length; i++)
        {
            TabPages.Add(tabPages[i]);
        }

        try
        {
            SetState(State.FromCreateHandles, true);
            SelectedIndex = index;
        }
        finally
        {
            SetState(State.FromCreateHandles, false);
        }

        // The comctl32 TabControl seems to have some painting glitches. Briefly
        // resizing the control seems to fix these.
        UpdateSize();
    }

    protected void RemoveAll()
    {
        Controls.Clear();

        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, (PInvoke.TCM_DELETEALLITEMS));
        }

        _tabPages.Clear();
    }

    private void RemoveTabPage(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, TabCount);

        if (index < _tabPages.Count)
        {
            _tabPages.RemoveAt(index);
        }

        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.TCM_DELETEITEM, (WPARAM)index);
        }

        _cachedDisplayRect = Rectangle.Empty;
    }

    private void ResetItemSize()
    {
        ItemSize = s_defaultItemSize;
    }

    private void ResetPadding()
    {
        Padding = s_defaultPaddingPoint;
    }

    private void ResizePages()
    {
        Rectangle rect = DisplayRectangle;
        TabPage[] pages = GetTabPages();
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].Bounds = rect;
        }
    }

    /// <summary>
    ///  Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
    /// </summary>
    internal override void SetToolTip(ToolTip toolTip)
    {
        if (toolTip is null || !ShowToolTips)
        {
            return;
        }

        PInvoke.SendMessage(this, PInvoke.TCM_SETTOOLTIPS, (WPARAM)toolTip.Handle);
        GC.KeepAlive(toolTip);
        _controlTipText = toolTip.GetToolTip(this);
    }

    private void SetTabPage(int index, TabPage value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, TabCount);
        ArgumentNullException.ThrowIfNull(value);

        if (IsHandleCreated)
        {
            SendMessage(PInvoke.TCM_SETITEMW, index, value);
        }

        // Make the Updated tab page the currently selected tab page
        if (DesignMode && IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.TCM_SETCURSEL, (WPARAM)index);
        }

        _tabPages[index] = value;
    }

    /// <summary>
    ///  Allows the user to specify the index in Tabcontrol.TabPageCollection of the tabpage to be shown.
    /// </summary>
    public void SelectTab(int index)
    {
        TabPage t = GetTabPage(index);
        if (t is not null)
        {
            SelectedTab = t;
        }
    }

    /// <summary>
    ///  Allows the user to specify the tabpage in Tabcontrol.TabPageCollection  to be shown.
    /// </summary>
    public void SelectTab(TabPage tabPage)
    {
        ArgumentNullException.ThrowIfNull(tabPage);

        int index = FindTabPage(tabPage);
        SelectTab(index);
    }

    /// <summary>
    ///  Allows the user to specify the name of the tabpage in Tabcontrol.TabPageCollection to be shown.
    /// </summary>
    public void SelectTab(string tabPageName)
    {
        ArgumentNullException.ThrowIfNull(tabPageName);

        TabPage tabPage = TabPages[tabPageName]!;
        SelectTab(tabPage);
    }

    /// <summary>
    ///  This is called by TabControl in response to the KeyDown event to override the selection of tabpages
    ///  for different key combinations.
    ///  Control + Tab selects the next tabpage.
    ///  Control + Shift + Tab selects the previous tabpage.
    ///  Control + PageDown selects the next tabpage.
    ///  Control + PageUp selects the previous tabpage.
    /// </summary>
    private void SelectNextTab(KeyEventArgs ke, bool forward)
    {
        // WmSelChanging actually changes focus to cause validations.
        // So cache in the Focused value so that we can reuse it later
        bool focused = Focused;

        // Fire Deselecting .. Deselected on currently selected TabPage...
        if (WmSelChanging())
        {
            SetState(State.UISelection, false);
            return;
        }

        if (ValidationCancelled)
        {
            SetState(State.UISelection, false);
            return;
        }
        else
        {
            int sel = SelectedIndex;
            if (sel != -1)
            {
                int count = TabCount;
                if (forward)
                {
                    sel = (sel + 1) % count;
                }
                else
                {
                    sel = (sel + count - 1) % count;
                }

                // this is special casing..
                // this function is called from OnKeyDown( ) which selects the NEXT TABPAGE
                // But now we call the WmSelChanging( ) to Focus the tab page
                // This breaks the logic in UpdateTabSelection (which is called
                // thru SET of SelectedIndex) to Select the First control
                // So adding this new Flag to select the first control.
                try
                {
                    SetState(State.UISelection, true);
                    SetState(State.SelectFirstControl, true);
                    SelectedIndex = sel;
                    // This is required so that we select the first control if the TabControl is not current focused.
                    SetState(State.SelectFirstControl, !focused);
                    // Fire Selecting .. Selected on newly selected TabPage...
                    WmSelChange();
                }
                finally
                {
                    // tabControlState[State.SelectFirstControl] can be true if the TabControl is not focussed
                    // But at the end of this function reset the state !!
                    SetState(State.SelectFirstControl, false);
                    ke.Handled = true;
                }
            }
        }
    }

    // TabControl overrides this method to return true.
    internal override bool ShouldPerformContainerValidation()
    {
        return true;
    }

    private bool ShouldSerializeItemSize()
    {
        return !_itemSize.Equals(s_defaultItemSize);
    }

    private new bool ShouldSerializePadding()
    {
        return !_padding.Equals(s_defaultPaddingPoint);
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        if (TabPages is not null)
        {
            s += $", TabPages.Count: {TabPages.Count}";
            if (TabPages.Count > 0)
            {
                s += $", TabPages[0]: {TabPages[0]}";
            }
        }

        return s;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void ScaleCore(float dx, float dy)
    {
        _currentlyScaling = true;
        base.ScaleCore(dx, dy);
        _currentlyScaling = false;
    }

    /// <summary>
    ///  Set the panel selections appropriately
    /// </summary>
    protected void UpdateTabSelection(bool updateFocus)
    {
        if (IsHandleCreated)
        {
            int index = SelectedIndex;

            // make current panel invisible
            TabPage[] tabPages = GetTabPages();
            if (index != -1)
            {
                // Changing the bounds of the tabPage during scaling
                // will force a layout to occur.  After this layout
                // the tabpage will then be scaled again resulting
                // in incorrect sizes.  Suspend Layout in this case.
                if (_currentlyScaling)
                {
                    tabPages[index].SuspendLayout();
                }

                tabPages[index].Bounds = DisplayRectangle;

                // After changing the Bounds of TabPages, we need to
                // make TabPages Redraw.
                // Use Invalidate directly here has no performance
                // issue, since ReSize is calling low frequence.
                tabPages[index].Invalidate();

                if (_currentlyScaling)
                {
                    tabPages[index].ResumeLayout(false);
                }

                tabPages[index].Visible = true;
                if (updateFocus)
                {
                    if (!Focused || GetState(State.SelectFirstControl))
                    {
                        SetState(State.UISelection, false);
                        bool selectNext = tabPages[index].SelectNextControl(null, true, true, false, false);

                        if (selectNext)
                        {
                            if (!ContainsFocus)
                            {
                                IContainerControl? c = GetContainerControl();
                                if (c is not null)
                                {
                                    while (c.ActiveControl is ContainerControl)
                                    {
                                        c = (IContainerControl)c.ActiveControl;
                                    }

                                    c.ActiveControl?.Focus();
                                }
                            }
                        }
                        else
                        {
                            IContainerControl? c = GetContainerControl();
                            if (c is not null && !DesignMode)
                            {
                                if (c is ContainerControl containerControl)
                                {
                                    containerControl.SetActiveControl(this);
                                }
                                else
                                {
                                    c.ActiveControl = this;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < tabPages.Length; i++)
            {
                if (i != SelectedIndex)
                {
                    tabPages[i].Visible = false;
                }
            }
        }
    }

    protected override void OnStyleChanged(EventArgs e)
    {
        base.OnStyleChanged(e);
        _cachedDisplayRect = Rectangle.Empty;
        UpdateTabSelection(false);
    }

    internal void UpdateTab(TabPage tabPage)
    {
        int index = FindTabPage(tabPage);
        SetTabPage(index, tabPage);

        // It's possible that changes to this TabPage will change the DisplayRectangle of the
        // TabControl, so invalidate and resize the size of this page.
        //
        _cachedDisplayRect = Rectangle.Empty;
        UpdateTabSelection(false);
    }

    private unsafe void WmNeedText(ref Message m)
    {
        NMTTDISPINFOW* ttt = (NMTTDISPINFOW*)(nint)m.LParamInternal;

        int commandID = (int)ttt->hdr.idFrom;

        string? tipText = GetToolTipText(GetTabPage(commandID));
        if (string.IsNullOrEmpty(tipText))
        {
            tipText = _controlTipText;
        }

        _toolTipBuffer.SetText(tipText);
        ttt->lpszText = (char*)_toolTipBuffer.Buffer;
        ttt->hinst = HINSTANCE.Null;

        // RightToLeft reading order
        if (RightToLeft == RightToLeft.Yes)
        {
            ttt->uFlags |= TOOLTIP_FLAGS.TTF_RTLREADING;
        }
    }

    private unsafe void WmReflectDrawItem(ref Message m)
    {
        DRAWITEMSTRUCT* dis = (DRAWITEMSTRUCT*)(nint)m.LParamInternal;

        using DrawItemEventArgs e = new(
            dis->hDC,
            Font,
            dis->rcItem,
            dis->itemID,
            dis->itemState,
            ForeColor,
            BackColor);

        OnDrawItem(e);

        m.ResultInternal = (LRESULT)1;
    }

    private bool WmSelChange()
    {
        TabControlCancelEventArgs tcc = new(SelectedTab, SelectedIndex, false, TabControlAction.Selecting);
        OnSelecting(tcc);
        if (!tcc.Cancel)
        {
            OnSelected(new TabControlEventArgs(SelectedTab, SelectedIndex, TabControlAction.Selected));
            OnSelectedIndexChanged(EventArgs.Empty);

            if (IsAccessibilityObjectCreated && SelectedTab?.ParentInternal is TabControl)
            {
                SelectedTab.TabAccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId);
                BeginInvoke((MethodInvoker)(() => SelectedTab.TabAccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId)));
            }
        }
        else
        {
            // user Cancelled the Selection of the new Tab.
            PInvoke.SendMessage(this, PInvoke.TCM_SETCURSEL, (WPARAM)_lastSelection);
            UpdateTabSelection(true);
        }

        return tcc.Cancel;
    }

    private bool WmSelChanging()
    {
        IContainerControl? c = GetContainerControl();
        if (c is not null && !DesignMode)
        {
            if (c is ContainerControl containerControl)
            {
                containerControl.SetActiveControl(this);
            }
            else
            {
                c.ActiveControl = this;
            }
        }

        // Fire DeSelecting .... on the current Selected Index...
        // Set the return value to a global
        // if 'cancelled' return from here else..
        // fire Deselected.
        _lastSelection = SelectedIndex;
        TabControlCancelEventArgs tcc = new(SelectedTab, SelectedIndex, false, TabControlAction.Deselecting);
        OnDeselecting(tcc);
        if (!tcc.Cancel)
        {
            OnDeselected(new TabControlEventArgs(SelectedTab, SelectedIndex, TabControlAction.Deselected));
        }

        return tcc.Cancel;
    }

    private unsafe void WmTabBaseReLayout()
    {
        BeginUpdate();
        _cachedDisplayRect = Rectangle.Empty;
        UpdateTabSelection(false);
        EndUpdate();
        Invalidate(true);

        // Remove other TabBaseReLayout messages from the message queue
        MSG msg = default;
        while (PInvoke.PeekMessage(
            &msg,
            this,
            (uint)_tabBaseReLayoutMessage,
            (uint)_tabBaseReLayoutMessage,
            PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
        {
        }
    }

    /// <summary>
    ///  The tab's window procedure.  Inheriting classes can override this
    ///  to add extra functionality, but should not forget to call
    ///  base.wndProc(m); to ensure the tab continues to function properly.
    /// </summary>
    protected override unsafe void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_DRAWITEM:
                WmReflectDrawItem(ref m);
                break;

            case MessageId.WM_REFLECT_MEASUREITEM:
                // We use TCM_SETITEMSIZE instead
                break;

            case PInvoke.WM_NOTIFY:
            case MessageId.WM_REFLECT_NOTIFY:
                NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
                switch (nmhdr->code)
                {
                    // new switch added to prevent the TabControl from changing to next TabPage ...
                    // in case of validation cancelled...
                    // Turn  tabControlState[State.UISelection] = false and Return So that no WmSelChange() gets fired.
                    // If validation not cancelled then tabControlState[State.UISelection] is turned ON to set the focus on to the ...
                    // next TabPage..

                    case PInvoke.TCN_SELCHANGING:
                        if (WmSelChanging())
                        {
                            m.ResultInternal = (LRESULT)1;
                            SetState(State.UISelection, false);
                            return;
                        }

                        if (ValidationCancelled)
                        {
                            m.ResultInternal = (LRESULT)1;
                            SetState(State.UISelection, false);
                            return;
                        }
                        else
                        {
                            SetState(State.UISelection, true);
                        }

                        break;
                    case PInvoke.TCN_SELCHANGE:
                        if (WmSelChange())
                        {
                            m.ResultInternal = (LRESULT)1;
                            SetState(State.UISelection, false);
                            return;
                        }
                        else
                        {
                            SetState(State.UISelection, true);
                        }

                        break;
                    case PInvoke.TTN_GETDISPINFOW:
                        // Setting the max width has the added benefit of enabling Multiline tool tips
                        PInvoke.SendMessage(nmhdr->hwndFrom, PInvoke.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                        WmNeedText(ref m);
                        m.ResultInternal = (LRESULT)1;
                        return;
                }

                break;
        }

        if (m.MsgInternal == _tabBaseReLayoutMessage)
        {
            WmTabBaseReLayout();
            return;
        }

        base.WndProc(ref m);
    }

    private bool GetState(State state) => _tabControlState[(int)state];

    private void SetState(State state, bool value) => _tabControlState[(int)state] = value;

    private unsafe int SendMessage(uint msg, int wParam, TabPage tabPage)
    {
        TCITEMW tcitem = default;
        string text = tabPage.Text;
        PrefixAmpersands(ref text);
        if (text is not null)
        {
            tcitem.mask |= TCITEMHEADERA_MASK.TCIF_TEXT;
            tcitem.cchTextMax = text.Length;
        }

        int imageIndex = tabPage.ImageIndex;
        tcitem.mask |= TCITEMHEADERA_MASK.TCIF_IMAGE;
        tcitem.iImage = tabPage.ImageIndexer.ActualIndex;

        fixed (char* pText = text)
        {
            tcitem.pszText = pText;
            return (int)PInvoke.SendMessage(this, msg, (WPARAM)wParam, ref tcitem);
        }
    }

    private static void PrefixAmpersands(ref string value)
    {
        // Due to a comctl32 problem, ampersands underline the next letter in the
        // text string, but the accelerators don't work.
        // So in this function, we prefix ampersands with another ampersand
        // so that they actually appear as ampersands.
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        // If there are no ampersands, we don't need to do anything here
        int firstAmpersand = value.IndexOf('&');
        if (firstAmpersand < 0)
        {
            return;
        }

        // Insert extra ampersands
        StringBuilder newString = new();
        newString.Append(value.AsSpan(0, firstAmpersand));
        for (int i = firstAmpersand; i < value.Length; ++i)
        {
            if (value[i] == '&')
            {
                if (i < value.Length - 1 && value[i + 1] == '&')
                {
                    // Skip the second ampersand
                    ++i;
                }

                newString.Append("&&");
            }
            else
            {
                newString.Append(value[i]);
            }
        }

        value = newString.ToString();
    }
}
