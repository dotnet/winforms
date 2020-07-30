// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Layout;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  The TabControl.  This control has a lot of the functionality of a TabStrip
    ///  but manages a list of TabPages which are the 'pages' that appear on each tab.
    /// </summary>
    [DefaultProperty(nameof(TabPages))]
    [DefaultEvent(nameof(SelectedIndexChanged))]
    [Designer("System.Windows.Forms.Design.TabControlDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionTabControl))]
    public partial class TabControl : Control
    {
        private static readonly Size DefaultItemSize = Size.Empty;
        private static readonly Point DefaultPaddingPoint = new Point(6, 3);

        // Properties
        private readonly TabPageCollection _tabCollection;
        private TabAlignment _alignment = TabAlignment.Top;
        private TabDrawMode _drawMode = TabDrawMode.Normal;
        private ImageList _imageList;
        private Size _itemSize = DefaultItemSize;
        private Point _padding = DefaultPaddingPoint;
        private TabSizeMode _sizeMode = TabSizeMode.Normal;
        private TabAppearance _appearance = TabAppearance.Normal;
        private Rectangle _cachedDisplayRect;
        private bool _currentlyScaling;
        private int _selectedIndex = -1;
        private string _controlTipText = string.Empty;
        private bool _handleInTable;

        // Events
        private EventHandler _onSelectedIndexChanged;
        private DrawItemEventHandler _onDrawItem;

        private static readonly object s_deselectingEvent = new object();
        private static readonly object s_deselectedEvent = new object();
        private static readonly object s_selectingEvent = new object();
        private static readonly object s_selectedEvent = new object();
        private static readonly object s_rightToLeftLayoutChangedEvent = new object();

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
        private readonly User32.WM _tabBaseReLayoutMessage = User32.RegisterWindowMessageW(Application.WindowMessagesVersion + TabBaseReLayoutMessageName);

        // State
        private TabPage[] _tabPages;
        private int _tabPageCount;
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
            _tabControlState = new Collections.Specialized.BitVector32(0x00000000);

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
                    //valid values are 0x0 to 0x3
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabAlignment.Top, (int)TabAlignment.Right))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabAlignment));
                    }

                    _alignment = value;
                    if (_alignment == TabAlignment.Left || _alignment == TabAlignment.Right)
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
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabAppearance.Normal, (int)TabAppearance.FlatButtons))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabAppearance));
                    }

                    _appearance = value;
                    RecreateHandle();

                    //Fire OnStyleChanged(EventArgs.Empty) here since we are no longer calling UpdateStyles( ) but always reCreating the Handle.
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
                //The tab control can only be rendered in 1 color: System's Control color.
                //So, always return this value... otherwise, we're inheriting the forms backcolor
                //and passing it on to the pab pages.
                return SystemColors.Control;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
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
        new public event EventHandler BackgroundImageLayoutChanged
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
        new public event EventHandler ForeColorChanged
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
                cp.ClassName = ComCtl32.WindowClasses.WC_TABCONTROL;

                // set up window styles
                //
                if (Multiline == true)
                {
                    cp.Style |= (int)ComCtl32.TCS.MULTILINE;
                }

                if (_drawMode == TabDrawMode.OwnerDrawFixed)
                {
                    cp.Style |= (int)ComCtl32.TCS.OWNERDRAWFIXED;
                }

                if (ShowToolTips && !DesignMode)
                {
                    cp.Style |= (int)ComCtl32.TCS.TOOLTIPS;
                }

                if (_alignment == TabAlignment.Bottom ||
                    _alignment == TabAlignment.Right)
                {
                    cp.Style |= (int)ComCtl32.TCS.BOTTOM;
                }

                if (_alignment == TabAlignment.Left ||
                    _alignment == TabAlignment.Right)
                {
                    cp.Style |= (int)ComCtl32.TCS.VERTICAL | (int)ComCtl32.TCS.MULTILINE;
                }

                if (GetState(State.HotTrack))
                {
                    cp.Style |= (int)ComCtl32.TCS.HOTTRACK;
                }

                if (_appearance == TabAppearance.Normal)
                {
                    cp.Style |= (int)ComCtl32.TCS.TABS;
                }
                else
                {
                    cp.Style |= (int)ComCtl32.TCS.BUTTONS;
                    if (_appearance == TabAppearance.FlatButtons && _alignment == TabAlignment.Top)
                    {
                        cp.Style |= (int)ComCtl32.TCS.FLATBUTTONS;
                    }
                }

                switch (_sizeMode)
                {
                    case TabSizeMode.Normal:
                        cp.Style |= (int)ComCtl32.TCS.RAGGEDRIGHT;
                        break;
                    case TabSizeMode.FillToRight:
                        cp.Style |= (int)ComCtl32.TCS.RIGHTJUSTIFY;
                        break;
                    case TabSizeMode.Fixed:
                        cp.Style |= (int)ComCtl32.TCS.FIXEDWIDTH;
                        break;
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
                {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= (int)(User32.WS_EX.LAYOUTRTL | User32.WS_EX.NOINHERITLAYOUT);
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(int)(User32.WS_EX.RTLREADING | User32.WS_EX.RIGHT | User32.WS_EX.LEFTSCROLLBAR);
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
                    // a ActiveX control, so check if this is ActiveX and dont force Handle Creation here as the native code breaks in this case.
                    if (!IsActiveX)
                    {
                        if (!IsHandleCreated)
                        {
                            CreateHandle();
                        }
                    }
                    if (IsHandleCreated)
                    {
                        User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.ADJUSTRECT, IntPtr.Zero, ref rect);
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
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabDrawMode.Normal, (int)TabDrawMode.OwnerDrawFixed))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabDrawMode));
                }

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
        public ImageList ImageList
        {
            get
            {
                return _imageList;
            }
            set
            {
                if (_imageList != value)
                {
                    EventHandler recreateHandler = new EventHandler(ImageListRecreateHandle);
                    EventHandler disposedHandler = new EventHandler(DetachImageList);

                    if (_imageList != null)
                    {
                        _imageList.RecreateHandle -= recreateHandler;
                        _imageList.Disposed -= disposedHandler;
                    }

                    _imageList = value;
                    IntPtr handle = (value != null) ? value.Handle : IntPtr.Zero;
                    if (IsHandleCreated)
                    {
                        User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETIMAGELIST, IntPtr.Zero, handle);
                    }

                    // Update the image list in the tab pages.
                    foreach (TabPage tabPage in TabPages)
                    {
                        tabPage.ImageIndexer.ImageList = value;
                    }

                    if (value != null)
                    {
                        value.RecreateHandle += recreateHandler;
                        value.Disposed += disposedHandler;
                    }
                }
            }
        }

        /// <summary>
        ///  By default, tabs will automatically size themselves to fit their icon, if any, and their label.
        ///  However, the tab size can be explicity set by setting this property.
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

                    return DefaultItemSize;
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
        ///  So we keep this flag in order to aviod repeatd addition (only during insert)
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
                    if (Multiline == false && (_alignment == TabAlignment.Left || _alignment == TabAlignment.Right))
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
        ///  the tab strip.  This is most commonly used when the Multline property
        ///  is 'true' and you want to know how many rows the tabs are currently
        ///  taking up.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TabBaseRowCountDescr))]
        public int RowCount
            => unchecked((int)(long)User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.GETROWCOUNT));

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
            get
            {
                if (IsHandleCreated)
                {
                    return unchecked((int)(long)User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.GETCURSEL));
                }

                return _selectedIndex;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectedIndex), value, -1));
                }

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

                        User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETCURSEL, (IntPtr)value);

                        if (!GetState(State.FromCreateHandles) && !GetState(State.SelectFirstControl))
                        {
                            // Fire Selecting & Selected .. Also if Selecting is Canceled..
                            // then retuern as we do not change the SelectedIndex...
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
        public TabPage SelectedTab
        {
            get
            {
                int index = SelectedIndex;
                if (index == -1 || _tabPages is null)
                {
                    return null;
                }

                Debug.Assert(0 <= index && index < _tabPages.Length, "SelectedIndex returned an invalid index");
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
            get
            {
                return _sizeMode;
            }
            set
            {
                if (_sizeMode == value)
                {
                    return;
                }

                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TabSizeMode.Normal, (int)TabSizeMode.Fixed))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TabSizeMode));
                }

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

        /// <summary>
        ///  Returns the number of tabs in the strip
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.TabBaseTabCountDescr))]
        public int TabCount
        {
            get { return _tabPageCount; }
        }

        /// <summary>
        ///  Returns the Collection of TabPages.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TabControlTabsDescr))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor("System.Windows.Forms.Design.TabPageCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
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
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.drawItemEventDescr))]
        public event DrawItemEventHandler DrawItem
        {
            add => _onDrawItem += value;
            remove => _onDrawItem -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => Events.AddHandler(s_rightToLeftLayoutChangedEvent, value);
            remove => Events.RemoveHandler(s_rightToLeftLayoutChangedEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged
        {
            add => _onSelectedIndexChanged += value;
            remove => _onSelectedIndexChanged -= value;
        }

        /// <summary>
        ///  Occurs before a tabpage is selected as the top tabPage.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.TabControlSelectingEventDescr))]
        public event TabControlCancelEventHandler Selecting
        {
            add => Events.AddHandler(s_selectingEvent, value);
            remove => Events.RemoveHandler(s_selectingEvent, value);
        }

        /// <summary>
        ///  Occurs after a tabpage is selected as the top tabPage.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.TabControlSelectedEventDescr))]
        public event TabControlEventHandler Selected
        {
            add => Events.AddHandler(s_selectedEvent, value);
            remove => Events.RemoveHandler(s_selectedEvent, value);
        }

        /// <summary>
        ///  Occurs before the visible property of the top tabpage is set to false.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.TabControlDeselectingEventDescr))]
        public event TabControlCancelEventHandler Deselecting
        {
            add => Events.AddHandler(s_deselectingEvent, value);
            remove => Events.RemoveHandler(s_deselectingEvent, value);
        }

        /// <summary>
        ///  Occurs after the visible property of the top tabpage is set to false.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.TabControlDeselectedEventDescr))]
        public event TabControlEventHandler Deselected
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
        public new event PaintEventHandler Paint
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
            int index = (int)SendMessage(ComCtl32.TCM.INSERTITEMW, (IntPtr)(_tabPageCount + 1), tabPage);
            User32.PostMessageW(this, _tabBaseReLayoutMessage);
            return index;
        }

        internal void ApplyItemSize()
        {
            if (IsHandleCreated && ShouldSerializeItemSize())
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETITEMSIZE, IntPtr.Zero, PARAM.FromLowHigh(_itemSize.Width, _itemSize.Height));
            }

            _cachedDisplayRect = Rectangle.Empty;
        }

        internal void BeginUpdate()
        {
            BeginUpdateInternal();
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);
                try
                {
                    var icc = new ComCtl32.INITCOMMONCONTROLSEX
                    {
                        dwICC = ComCtl32.ICC.TAB_CLASSES
                    };
                    ComCtl32.InitCommonControlsEx(ref icc);
                }
                finally
                {
                    ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }

        private void DetachImageList(object sender, EventArgs e)
        {
            ImageList = null;
        }

        /// <summary>
        ///  Allows the user to specify the index in Tabcontrol.TabPageCollection of the tabpage to be hidden.
        /// </summary>
        public void DeselectTab(int index)
        {
            TabPage t = GetTabPage(index);
            if (SelectedTab == t)
            {
                if (0 <= index && index < TabPages.Count - 1)
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
            if (tabPage is null)
            {
                throw new ArgumentNullException(nameof(tabPage));
            }
            int index = FindTabPage(tabPage);
            DeselectTab(index);
        }

        /// <summary>
        ///  Allows the user to specify the name of the tabpage in Tabcontrol.TabPageCollection to be hidden.
        /// </summary>
        public void DeselectTab(string tabPageName)
        {
            if (tabPageName is null)
            {
                throw new ArgumentNullException(nameof(tabPageName));
            }
            TabPage tabPage = TabPages[tabPageName];
            DeselectTab(tabPage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_imageList != null)
                {
                    _imageList.Disposed -= new EventHandler(DetachImageList);
                }
            }

            // Dispose unmanaged resources.
            _toolTipBuffer.Dispose();

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

        internal int FindTabPage(TabPage tabPage)
        {
            if (_tabPages != null)
            {
                for (int i = 0; i < _tabPageCount; i++)
                {
                    if (_tabPages[i].Equals(tabPage))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public Control GetControl(int index)
        {
            return (Control)GetTabPage(index);
        }

        internal TabPage GetTabPage(int index)
        {
            if (index < 0 || index >= _tabPageCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }
            return _tabPages[index];
        }

        /// <summary>
        ///  This has package scope so that TabStrip and TabControl can call it.
        /// </summary>
        protected virtual object[] GetItems()
        {
            TabPage[] result = new TabPage[_tabPageCount];
            if (_tabPageCount > 0)
            {
                Array.Copy(_tabPages, 0, result, 0, _tabPageCount);
            }

            return result;
        }

        /// <summary>
        ///  This has package scope so that TabStrip and TabControl can call it.
        /// </summary>
        protected virtual object[] GetItems(Type baseType)
        {
            object[] result = (object[])Array.CreateInstance(baseType, _tabPageCount);
            if (_tabPageCount > 0)
            {
                Array.Copy(_tabPages, 0, result, 0, _tabPageCount);
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
            if (index < 0 || (index >= _tabPageCount && !GetState(State.GetTabRectfromItemSize)))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            SetState(State.GetTabRectfromItemSize, false);
            RECT rect = new RECT();

            // normally, we would not want to create the handle for this, but since
            // it is dependent on the actual physical display, we simply must.
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.GETITEMRECT, (IntPtr)index, ref rect);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        protected string GetToolTipText(object item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (!(item is TabPage tabPage))
            {
                throw new ArgumentException(SR.TabControlBadControl, nameof(item));
            }

            return tabPage.ToolTipText;
        }

        private void ImageListRecreateHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETIMAGELIST, IntPtr.Zero, ImageList.Handle);
            }
        }

        internal void Insert(int index, TabPage tabPage)
        {
            if (_tabPages is null)
            {
                _tabPages = new TabPage[4];
            }
            else if (_tabPages.Length == _tabPageCount)
            {
                TabPage[] newTabPages = new TabPage[_tabPageCount * 2];
                Array.Copy(_tabPages, 0, newTabPages, 0, _tabPageCount);
                _tabPages = newTabPages;
            }
            if (index < _tabPageCount)
            {
                Array.Copy(_tabPages, index, _tabPages, index + 1, _tabPageCount - index);
            }
            _tabPages[index] = tabPage;
            _tabPageCount++;
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
            if (index < 0 || ((_tabPages != null) && index > _tabPageCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            if (tabPage is null)
            {
                throw new ArgumentNullException(nameof(tabPage));
            }

            if (IsHandleCreated)
            {
                int retIndex = (int)SendMessage(ComCtl32.TCM.INSERTITEMW, (IntPtr)index, tabPage);
                if (retIndex >= 0)
                {
                    Insert(retIndex, tabPage);
                }
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

            switch (keyData & Keys.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
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

            //Add the handle to hashtable for Ids ..
            _windowId = NativeWindow.CreateWindowId(this);
            _handleInTable = true;

            // Set the padding BEFORE setting the control's font (as done
            // in base.OnHandleCreated()) so that the tab control will honor both the
            // horizontal and vertical dimensions of the padding rectangle.
            if (!_padding.IsEmpty)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETPADDING, IntPtr.Zero, PARAM.FromLowHigh(_padding.X, _padding.Y));
            }

            base.OnHandleCreated(e);
            _cachedDisplayRect = Rectangle.Empty;
            ApplyItemSize();
            if (_imageList != null)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETIMAGELIST, IntPtr.Zero, _imageList.Handle);
            }

            if (ShowToolTips)
            {
                IntPtr tooltipHwnd;
                tooltipHwnd = User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.GETTOOLTIPS);
                if (tooltipHwnd != IntPtr.Zero)
                {
                    User32.SetWindowPos(
                        new HandleRef(this, tooltipHwnd),
                        User32.HWND_TOPMOST,
                        flags: User32.SWP.NOMOVE | User32.SWP.NOSIZE | User32.SWP.NOACTIVATE);
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
            //Remove the Handle from NativewIndow....

            // Don't try to remove the Handle if we've already done so
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
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (SelectedTab != null)
            {
                SelectedTab.FireEnter(e);
            }
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
        protected override void OnLeave(EventArgs e)
        {
            if (SelectedTab != null)
            {
                SelectedTab.FireLeave(e);
            }
            base.OnLeave(e);
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
        }

        /// <summary>
        ///  Raises the <see cref='OnSelecting'/> event.
        /// </summary>
        protected virtual void OnSelecting(TabControlCancelEventArgs e)
        {
            ((TabControlCancelEventHandler)Events[s_selectingEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='OnSelected'/> event.
        /// </summary>
        protected virtual void OnSelected(TabControlEventArgs e)
        {
            ((TabControlEventHandler)Events[s_selectedEvent])?.Invoke(this, e);

            // Raise the enter event for this tab.
            if (SelectedTab != null)
            {
                SelectedTab.FireEnter(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Raises the <see cref='OnDeselecting'/> event.
        /// </summary>
        protected virtual void OnDeselecting(TabControlCancelEventArgs e)
        {
            ((TabControlCancelEventHandler)Events[s_deselectingEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='OnDeselected'/> event.
        /// </summary>
        protected virtual void OnDeselected(TabControlEventArgs e)
        {
            ((TabControlEventHandler)Events[s_deselectedEvent])?.Invoke(this, e);

            // Raise the Leave event for this tab.
            if (SelectedTab != null)
            {
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
            //
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
            //
            TabPage[] tabPages = GetTabPages();

            int index = ((tabPages.Length > 0) && (SelectedIndex == -1)) ? 0 : SelectedIndex;

            // We don't actually want to remove the windows forms Tabpages - we only
            // want to remove the corresponding TCITEM structs.
            // So, no RemoveAll()
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.DELETEALLITEMS);
            }
            this._tabPages = null;
            _tabPageCount = 0;

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
            //
            UpdateSize();
        }

        protected void RemoveAll()
        {
            Controls.Clear();

            if (IsHandleCreated)
            {
                User32.SendMessageW(this, ((User32.WM)TCM.DELETEALLITEMS), IntPtr.Zero, IntPtr.Zero);
            }

            _tabPages = null;
            _tabPageCount = 0;
        }

        private void RemoveTabPage(int index)
        {
            if (index < 0 || index >= _tabPageCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            _tabPageCount--;
            if (index < _tabPageCount)
            {
                Array.Copy(_tabPages, index + 1, _tabPages, index, _tabPageCount - index);
            }
            _tabPages[_tabPageCount] = null;
            if (IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.DELETEITEM, (IntPtr)index);
            }
            _cachedDisplayRect = Rectangle.Empty;
        }

        private void ResetItemSize()
        {
            ItemSize = DefaultItemSize;
        }

        private void ResetPadding()
        {
            Padding = DefaultPaddingPoint;
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
        internal void SetToolTip(ToolTip toolTip, string controlToolTipText)
        {
            User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETTOOLTIPS, toolTip.Handle);
            GC.KeepAlive(toolTip);
            _controlTipText = controlToolTipText;
        }

        private void SetTabPage(int index, TabPage value)
        {
            if (index < 0 || index >= _tabPageCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (IsHandleCreated)
            {
                SendMessage(ComCtl32.TCM.SETITEMW, (IntPtr)index, value);
            }

            // Make the Updated tab page the currently selected tab page
            if (DesignMode && IsHandleCreated)
            {
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETCURSEL, (IntPtr)index, IntPtr.Zero);
            }
            _tabPages[index] = value;
        }

        /// <summary>
        ///  Allows the user to specify the index in Tabcontrol.TabPageCollection of the tabpage to be shown.
        /// </summary>
        public void SelectTab(int index)
        {
            TabPage t = GetTabPage(index);
            if (t != null)
            {
                SelectedTab = t;
            }
        }

        /// <summary>
        ///  Allows the user to specify the tabpage in Tabcontrol.TabPageCollection  to be shown.
        /// </summary>
        public void SelectTab(TabPage tabPage)
        {
            if (tabPage is null)
            {
                throw new ArgumentNullException(nameof(tabPage));
            }
            int index = FindTabPage(tabPage);
            SelectTab(index);
        }

        /// <summary>
        ///  Allows the user to specify the name of the tabpage in Tabcontrol.TabPageCollection to be shown.
        /// </summary>
        public void SelectTab(string tabPageName)
        {
            if (tabPageName is null)
            {
                throw new ArgumentNullException(nameof(tabPageName));
            }
            TabPage tabPage = TabPages[tabPageName];
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
            return !_itemSize.Equals(DefaultItemSize);
        }

        private new bool ShouldSerializePadding()
        {
            return !_padding.Equals(DefaultPaddingPoint);
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            if (TabPages != null)
            {
                s += ", TabPages.Count: " + TabPages.Count.ToString(CultureInfo.CurrentCulture);
                if (TabPages.Count > 0)
                {
                    s += ", TabPages[0]: " + TabPages[0].ToString();
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
                                    IContainerControl c = GetContainerControl();
                                    if (c != null)
                                    {
                                        while (c.ActiveControl is ContainerControl)
                                        {
                                            c = (IContainerControl)c.ActiveControl;
                                        }
                                        if (c.ActiveControl != null)
                                        {
                                            c.ActiveControl.Focus();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                IContainerControl c = GetContainerControl();
                                if (c != null && !DesignMode)
                                {
                                    if (c is ContainerControl)
                                    {
                                        ((ContainerControl)c).SetActiveControl(this);
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
            NMTTDISPINFOW* ttt = (NMTTDISPINFOW*)m.LParam;

            int commandID = (int)ttt->hdr.idFrom;

            string tipText = GetToolTipText(GetTabPage(commandID));
            if (string.IsNullOrEmpty(tipText))
            {
                tipText = _controlTipText;
            }

            _toolTipBuffer.SetText(tipText);
            ttt->lpszText = _toolTipBuffer.Buffer;
            ttt->hinst = IntPtr.Zero;

            // RightToLeft reading order
            if (RightToLeft == RightToLeft.Yes)
            {
                ttt->uFlags |= TTF.RTLREADING;
            }
        }

        private unsafe void WmReflectDrawItem(ref Message m)
        {
            User32.DRAWITEMSTRUCT* dis = (User32.DRAWITEMSTRUCT*)m.LParam;

            using var e = new DrawItemEventArgs(
                dis->hDC,
                Font,
                dis->rcItem,
                dis->itemID,
                dis->itemState);

            OnDrawItem(e);

            m.Result = (IntPtr)1;
        }

        private bool WmSelChange()
        {
            TabControlCancelEventArgs tcc = new TabControlCancelEventArgs(SelectedTab, SelectedIndex, false, TabControlAction.Selecting);
            OnSelecting(tcc);
            if (!tcc.Cancel)
            {
                OnSelected(new TabControlEventArgs(SelectedTab, SelectedIndex, TabControlAction.Selected));
                OnSelectedIndexChanged(EventArgs.Empty);
            }
            else
            {
                // user Cancelled the Selection of the new Tab.
                User32.SendMessageW(this, (User32.WM)ComCtl32.TCM.SETCURSEL, (IntPtr)_lastSelection);
                UpdateTabSelection(true);
            }
            return tcc.Cancel;
        }

        private bool WmSelChanging()
        {
            IContainerControl c = GetContainerControl();
            if (c != null && !DesignMode)
            {
                if (c is ContainerControl)
                {
                    ((ContainerControl)c).SetActiveControl(this);
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
            TabControlCancelEventArgs tcc = new TabControlCancelEventArgs(SelectedTab, SelectedIndex, false, TabControlAction.Deselecting);
            OnDeselecting(tcc);
            if (!tcc.Cancel)
            {
                OnDeselected(new TabControlEventArgs(SelectedTab, SelectedIndex, TabControlAction.Deselected));
            }
            return tcc.Cancel;
        }

        private void WmTabBaseReLayout(ref Message m)
        {
            BeginUpdate();
            _cachedDisplayRect = Rectangle.Empty;
            UpdateTabSelection(false);
            EndUpdate();
            Invalidate(true);

            // Remove other TabBaseReLayout messages from the message queue
            var msg = new User32.MSG();
            while (User32.PeekMessageW(ref msg, this, _tabBaseReLayoutMessage, _tabBaseReLayoutMessage, User32.PM.REMOVE).IsTrue())
            {
                // No-op.
            }
        }

        /// <summary>
        ///  The tab's window procedure.  Inheritng classes can override this
        ///  to add extra functionality, but should not forget to call
        ///  base.wndProc(m); to ensure the tab continues to function properly.
        /// </summary>
        protected unsafe override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.REFLECT_DRAWITEM:
                    WmReflectDrawItem(ref m);
                    break;

                case User32.WM.REFLECT_MEASUREITEM:
                    // We use TCM_SETITEMSIZE instead
                    break;

                case User32.WM.NOTIFY:
                case User32.WM.REFLECT_NOTIFY:
                    User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                    switch (nmhdr->code)
                    {
                        // new switch added to prevent the TabControl from changing to next TabPage ...
                        //in case of validation cancelled...
                        //Turn  tabControlState[State.UISelection] = false and Return So that no WmSelChange() gets fired.
                        //If validation not cancelled then tabControlState[State.UISelection] is turned ON to set the focus on to the ...
                        //next TabPage..

                        case (int)TCN.SELCHANGING:
                            if (WmSelChanging())
                            {
                                m.Result = (IntPtr)1;
                                SetState(State.UISelection, false);
                                return;
                            }

                            if (ValidationCancelled)
                            {
                                m.Result = (IntPtr)1;
                                SetState(State.UISelection, false);
                                return;
                            }
                            else
                            {
                                SetState(State.UISelection, true);
                            }
                            break;
                        case (int)TCN.SELCHANGE:
                            if (WmSelChange())
                            {
                                m.Result = (IntPtr)1;
                                SetState(State.UISelection, false);
                                return;
                            }
                            else
                            {
                                SetState(State.UISelection, true);
                            }
                            break;
                        case (int)TTN.GETDISPINFOW:
                            // Setting the max width has the added benefit of enabling Multiline tool tips
                            User32.SendMessageW(nmhdr->hwndFrom, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
                            WmNeedText(ref m);
                            m.Result = (IntPtr)1;
                            return;
                    }
                    break;
            }
            if (m.Msg == (int)_tabBaseReLayoutMessage)
            {
                WmTabBaseReLayout(ref m);
                return;
            }
            base.WndProc(ref m);
        }

        private bool GetState(State state) => _tabControlState[(int)state];

        private void SetState(State state, bool value) => _tabControlState[(int)state] = value;

        private unsafe IntPtr SendMessage(ComCtl32.TCM msg, IntPtr wParam, TabPage tabPage)
        {
            var tcitem = new ComCtl32.TCITEMW();
            string text = tabPage.Text;
            PrefixAmpersands(ref text);
            if (text != null)
            {
                tcitem.mask |= ComCtl32.TCIF.TEXT;
                tcitem.cchTextMax = text.Length;
            }

            int imageIndex = tabPage.ImageIndex;
            tcitem.mask |= ComCtl32.TCIF.IMAGE;
            tcitem.iImage = tabPage.ImageIndexer.ActualIndex;

            fixed (char* pText = text)
            {
                tcitem.pszText = pText;
                return User32.SendMessageW(this, (User32.WM)msg, wParam, ref tcitem);
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
            if (value.IndexOf('&') < 0)
            {
                return;
            }

            // Insert extra ampersands
            var newString = new StringBuilder();
            for (int i = 0; i < value.Length; ++i)
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
}
