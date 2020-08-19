// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ToolStrip control.
    /// </summary>
    [DesignerSerializer("System.Windows.Forms.Design.ToolStripCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)]
    [Designer("System.Windows.Forms.Design.ToolStripDesigner, " + AssemblyRef.SystemDesign)]
    [DefaultProperty(nameof(Items))]
    [SRDescription(nameof(SR.DescriptionToolStrip))]
    [DefaultEvent(nameof(ItemClicked))]
    public class ToolStrip : ScrollableControl, IArrangedElement, ISupportToolStripPanel
    {
        private static Size s_onePixel = new Size(1, 1);
        internal static Point s_invalidMouseEnter = new Point(int.MaxValue, int.MaxValue);

        private ToolStripItemCollection _toolStripItemCollection;
        private ToolStripOverflowButton _toolStripOverflowButton;
        private ToolStripGrip _toolStripGrip;
        private ToolStripItemCollection _displayedItems;
        private ToolStripItemCollection _overflowItems;
        private ToolStripDropTargetManager _dropTargetManager;
        private IntPtr _hwndThatLostFocus = IntPtr.Zero;
        private ToolStripItem _lastMouseActiveItem;
        private ToolStripItem _lastMouseDownedItem;
        private LayoutEngine _layoutEngine;
        private ToolStripLayoutStyle _layoutStyle = ToolStripLayoutStyle.StackWithOverflow;
        private Rectangle _lastInsertionMarkRect = Rectangle.Empty;
        private ImageList _imageList;
        private ToolStripGripStyle _toolStripGripStyle = ToolStripGripStyle.Visible;
        private int _toolStripState;
        private bool _showItemToolTips;
        private MouseHoverTimer _mouseHoverTimer;
        private ToolStripItem _currentlyActiveTooltipItem;
        private NativeWindow _dropDownOwnerWindow;
        private byte _mouseDownID;  // NEVER use this directly from another class, 0 should never be returned to another class.
        private ToolStripRenderer _renderer;
        private Type _currentRendererType = typeof(Type);
        private Hashtable _shortcuts;
        private Stack<MergeHistory> _mergeHistoryStack;
        private ToolStripDropDownDirection _toolStripDropDownDirection = ToolStripDropDownDirection.Default;
        private Size _largestDisplayedItemSize = Size.Empty;
        private CachedItemHdcInfo _cachedItemHdcInfo;
        private bool _alreadyHooked;

        private Size _imageScalingSize;
        private const int IconDimension = 16;
        private static int s_iconWidth = IconDimension;
        private static int s_iconHeight = IconDimension;

        private Font _defaultFont;
        private RestoreFocusMessageFilter _restoreFocusFilter;
        private static readonly Padding s_defaultPadding = new Padding(0, 0, 1, 0);
        private static readonly Padding s_defaultGripMargin = new Padding(2);
        private Padding _scaledDefaultPadding = s_defaultPadding;
        private Padding _scaledDefaultGripMargin = s_defaultGripMargin;

        private Point _mouseEnterWhenShown = s_invalidMouseEnter;

        private const int InsertionBeamWidth = 6;

        internal static int s_insertionBeamWidth = InsertionBeamWidth;

        private static readonly object s_eventPaintGrip = new object();
        private static readonly object s_eventLayoutCompleted = new object();
        private static readonly object s_eventItemAdded = new object();
        private static readonly object s_eventItemRemoved = new object();
        private static readonly object s_eventLayoutStyleChanged = new object();
        private static readonly object s_eventRendererChanged = new object();
        private static readonly object s_eventItemClicked = new object();
        private static readonly object s_eventLocationChanging = new object();
        private static readonly object s_eventBeginDrag = new object();
        private static readonly object s_eventEndDrag = new object();

        private static readonly int s_propBindingContext = PropertyStore.CreateKey();
        private static readonly int s_propTextDirection = PropertyStore.CreateKey();
        private static readonly int s_propToolTip = PropertyStore.CreateKey();
        private static readonly int s_propToolStripPanelCell = PropertyStore.CreateKey();

        internal const int STATE_CANOVERFLOW = 0x00000001;
        internal const int STATE_ALLOWITEMREORDER = 0x00000002;
        internal const int STATE_DISPOSINGITEMS = 0x00000004;
        internal const int STATE_MENUAUTOEXPAND = 0x00000008;
        internal const int STATE_MENUAUTOEXPANDDEFAULT = 0x00000010;
        internal const int STATE_SCROLLBUTTONS = 0x00000020;
        internal const int STATE_USEDEFAULTRENDERER = 0x00000040;
        internal const int STATE_ALLOWMERGE = 0x00000080;
        internal const int STATE_RAFTING = 0x00000100;
        internal const int STATE_STRETCH = 0x00000200;
        internal const int STATE_LOCATIONCHANGING = 0x00000400;
        internal const int STATE_DRAGGING = 0x00000800;
        internal const int STATE_HASVISIBLEITEMS = 0x00001000;
        internal const int STATE_SUSPENDCAPTURE = 0x00002000;
        internal const int STATE_LASTMOUSEDOWNEDITEMCAPTURE = 0x00004000;
        internal const int STATE_MENUACTIVE = 0x00008000;

#if DEBUG
        internal static readonly TraceSwitch s_selectionDebug = new TraceSwitch("SelectionDebug", "Debug ToolStrip Selection code");
        internal static readonly TraceSwitch s_dropTargetDebug = new TraceSwitch("DropTargetDebug", "Debug ToolStrip Drop code");
        internal static readonly TraceSwitch s_layoutDebugSwitch = new TraceSwitch("Layout debug", "Debug ToolStrip layout code");
        internal static readonly TraceSwitch s_mouseActivateDebug = new TraceSwitch("ToolStripMouseActivate", "Debug ToolStrip WM_MOUSEACTIVATE code");
        internal static readonly TraceSwitch s_mergeDebug = new TraceSwitch("ToolStripMergeDebug", "Debug toolstrip merging");
        internal static readonly TraceSwitch s_snapFocusDebug = new TraceSwitch("SnapFocus", "Debug snapping/restoration of focus");
        internal static readonly TraceSwitch s_flickerDebug = new TraceSwitch("FlickerDebug", "Debug excessive calls to Invalidate()");
        internal static readonly TraceSwitch s_itemReorderDebug = new TraceSwitch("ItemReorderDebug", "Debug excessive calls to Invalidate()");
        internal static readonly TraceSwitch s_mdiMergeDebug = new TraceSwitch("MDIMergeDebug", "Debug toolstrip MDI merging");
        internal static readonly TraceSwitch s_menuAutoExpandDebug = new TraceSwitch("MenuAutoExpand", "Debug menu auto expand");
        internal static readonly TraceSwitch s_controlTabDebug = new TraceSwitch("ControlTab", "Debug ToolStrip Control+Tab selection");
#else
        internal static readonly TraceSwitch s_selectionDebug;
        internal static readonly TraceSwitch s_dropTargetDebug;
        internal static readonly TraceSwitch s_layoutDebugSwitch;
        internal static readonly TraceSwitch s_mouseActivateDebug;
        internal static readonly TraceSwitch s_mergeDebug;
        internal static readonly TraceSwitch s_snapFocusDebug;
        internal static readonly TraceSwitch s_flickerDebug;
        internal static readonly TraceSwitch s_itemReorderDebug;
        internal static readonly TraceSwitch s_mdiMergeDebug;
        internal static readonly TraceSwitch s_menuAutoExpandDebug;
        internal static readonly TraceSwitch s_controlTabDebug;
#endif

        private delegate void BooleanMethodInvoker(bool arg);
        internal Action<int, int> _rescaleConstsCallbackDelegate;

        public ToolStrip()
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                ToolStripManager.CurrentDpi = DeviceDpi;
                _defaultFont = ToolStripManager.DefaultFont;
                s_iconWidth = DpiHelper.LogicalToDeviceUnits(IconDimension, DeviceDpi);
                s_iconHeight = DpiHelper.LogicalToDeviceUnits(IconDimension, DeviceDpi);
                s_insertionBeamWidth = DpiHelper.LogicalToDeviceUnits(InsertionBeamWidth, DeviceDpi);
                _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(s_defaultPadding, DeviceDpi);
                _scaledDefaultGripMargin = DpiHelper.LogicalToDeviceUnits(s_defaultGripMargin, DeviceDpi);
            }
            else if (DpiHelper.IsScalingRequired)
            {
                s_iconWidth = DpiHelper.LogicalToDeviceUnitsX(IconDimension);
                s_iconHeight = DpiHelper.LogicalToDeviceUnitsY(IconDimension);
                s_insertionBeamWidth = DpiHelper.LogicalToDeviceUnitsX(InsertionBeamWidth);
                _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(s_defaultPadding);
                _scaledDefaultGripMargin = DpiHelper.LogicalToDeviceUnits(s_defaultGripMargin);
            }

            _imageScalingSize = new Size(s_iconWidth, s_iconHeight);

            SuspendLayout();
            CanOverflow = true;
            TabStop = false;
            MenuAutoExpand = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            SetStyle(ControlStyles.Selectable, false);
            SetToolStripState(STATE_USEDEFAULTRENDERER | STATE_ALLOWMERGE, true);

            SetExtendedState(ExtendedStates.MaintainsOwnCaptureMode // A toolstrip does not take capture on MouseDown.
                      | ExtendedStates.UserPreferredSizeCache, // this class overrides GetPreferredSizeCore, let Control automatically cache the result
                       true);

            //add a weak ref link in ToolstripManager
            ToolStripManager.ToolStrips.Add(this);

            _layoutEngine = new ToolStripSplitStackLayout(this);
            Dock = DefaultDock;
            AutoSize = true;
            CausesValidation = false;
            Size defaultSize = DefaultSize;
            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
            ShowItemToolTips = DefaultShowItemToolTips;
            ResumeLayout(true);
        }

        public ToolStrip(params ToolStripItem[] items) : this()
        {
            Items.AddRange(items);
        }

        internal ArrayList ActiveDropDowns { get; } = new ArrayList(1);

        // returns true when entered into menu mode through this toolstrip/menustrip
        // this is only really supported for menustrip active event, but to prevent casting everywhere...
        internal virtual bool KeyboardActive
        {
            get { return GetToolStripState(STATE_MENUACTIVE); }
            set { SetToolStripState(STATE_MENUACTIVE, value); }
        }

        // This is only for use in determining whether to show scroll bars on
        // ToolStripDropDownMenus.  No one else should be using it for anything.
        internal virtual bool AllItemsVisible
        {
            get
            {
                return true;
            }
            set
            {
                // we do nothing in repsonse to a set, since we calculate the value above.
            }
        }

        [DefaultValue(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                if (IsInToolStripPanel && base.AutoSize && !value)
                {
                    // Restoring the bounds can change the location of the toolstrip -
                    // which would join it to a new row.  Set the specified bounds to the new location to
                    // prevent this.
                    Rectangle bounds = CommonProperties.GetSpecifiedBounds(this);
                    bounds.Location = Location;
                    CommonProperties.UpdateSpecifiedBounds(this, bounds.X, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.Location);
                }
                base.AutoSize = value;
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoScroll
        {
            get => base.AutoScroll;
            set
            {
                throw new NotSupportedException(SR.ToolStripDoesntSupportAutoScroll);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size AutoScrollMargin
        {
            get => base.AutoScrollMargin;
            set => base.AutoScrollMargin = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size AutoScrollMinSize
        {
            get => base.AutoScrollMinSize;
            set => base.AutoScrollMinSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Point AutoScrollPosition
        {
            get => base.AutoScrollPosition;
            set => base.AutoScrollPosition = value;
        }

        public override bool AllowDrop
        {
            get => base.AllowDrop;
            set
            {
                if (value && AllowItemReorder)
                {
                    throw new ArgumentException(SR.ToolStripAllowItemReorderAndAllowDropCannotBeSetToTrue);
                }

                base.AllowDrop = value;

                //

                if (value)
                {
                    DropTargetManager.EnsureRegistered(this);
                }
                else
                {
                    DropTargetManager.EnsureUnRegistered(this);
                }
            }
        }

        /// <summary>
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ToolStripAllowItemReorderDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public bool AllowItemReorder
        {
            get { return GetToolStripState(STATE_ALLOWITEMREORDER); }
            set
            {
                if (GetToolStripState(STATE_ALLOWITEMREORDER) != value)
                {
                    if (AllowDrop && value)
                    {
                        throw new ArgumentException(SR.ToolStripAllowItemReorderAndAllowDropCannotBeSetToTrue);
                    }
                    SetToolStripState(STATE_ALLOWITEMREORDER, value);

                    //

                    if (value)
                    {
                        ToolStripSplitStackDragDropHandler dragDropHandler = new ToolStripSplitStackDragDropHandler(this);
                        ItemReorderDropSource = dragDropHandler;
                        ItemReorderDropTarget = dragDropHandler;

                        DropTargetManager.EnsureRegistered(this);
                    }
                    else
                    {
                        DropTargetManager.EnsureUnRegistered(this);
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ToolStripAllowMergeDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public bool AllowMerge
        {
            get { return GetToolStripState(STATE_ALLOWMERGE); }
            set
            {
                if (GetToolStripState(STATE_ALLOWMERGE) != value)
                {
                    SetToolStripState(STATE_ALLOWMERGE, value);
                }
            }
        }

        public override AnchorStyles Anchor
        {
            get => base.Anchor;
            set
            {
                // the base calls SetDock, which causes an OnDockChanged to be called
                // which forces two layouts of the parent.
                using (new LayoutTransaction(this, this, PropertyNames.Anchor))
                {
                    base.Anchor = value;
                }
            }
        }

        /// <summary>
        ///  Just here so we can implement ShouldSerializeBackColor
        /// </summary>
        [SRDescription(nameof(SR.ToolStripBackColorDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public new Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolStripOnBeginDrag))]
        public event EventHandler BeginDrag
        {
            add => Events.AddHandler(s_eventBeginDrag, value);
            remove => Events.RemoveHandler(s_eventBeginDrag, value);
        }

        public override BindingContext BindingContext
        {
            get
            {
                BindingContext bc = (BindingContext)Properties.GetObject(s_propBindingContext);
                if (bc != null)
                {
                    return bc;
                }

                // try the parent
                //
                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties)
                {
                    return p.BindingContext;
                }

                // we don't have a binding context
                return null;
            }
            set
            {
                if (Properties.GetObject(s_propBindingContext) != value)
                {
                    Properties.SetObject(s_propBindingContext, value);

                    // re-wire the bindings
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(true)]
        [SRDescription(nameof(SR.ToolStripCanOverflowDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        public bool CanOverflow
        {
            get
            {
                return GetToolStripState(STATE_CANOVERFLOW);
            }
            set
            {
                if (GetToolStripState(STATE_CANOVERFLOW) != value)
                {
                    SetToolStripState(STATE_CANOVERFLOW, value);
                    InvalidateLayout();
                }
            }
        }

        /// <summary> we can only shift selection when we're not focused (someone mousing over us)
        ///  or we are focused and one of our toolstripcontrolhosts do not have focus.
        ///  SCENARIO: put focus in combo box, move the mouse over another item... selectioni
        ///  should not shift until the combobox relinquishes its focus.
        /// </summary>
        internal bool CanHotTrack
        {
            get
            {
                if (!Focused)
                {
                    // if  ContainsFocus in one of the children = false, someone is just mousing by, we can hot track
                    return (ContainsFocus == false);
                }
                else
                {
                    // if the toolstrip itself contains focus we can definately hottrack.
                    return true;
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public new bool CausesValidation
        {
            get
            {
                // By default: CausesValidation is false for a ToolStrip
                // we want people to be able to use menus without validating
                // their controls.
                return base.CausesValidation;
            }
            set => base.CausesValidation = value;
        }

        [Browsable(false)]
        public new event EventHandler CausesValidationChanged
        {
            add => base.CausesValidationChanged += value;
            remove => base.CausesValidationChanged -= value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls
        {
            get => base.Controls;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event ControlEventHandler ControlAdded
        {
            add => base.ControlAdded += value;
            remove => base.ControlAdded -= value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Cursor Cursor
        {
            get => base.Cursor;
            set => base.Cursor = value;
        }

        /// <summary>
        ///  Hide browsable property
        /// </summary>
        [Browsable(false)]
        public new event EventHandler CursorChanged
        {
            add => base.CursorChanged += value;
            remove => base.CursorChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event ControlEventHandler ControlRemoved
        {
            add => base.ControlRemoved += value;
            remove => base.ControlRemoved -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolStripOnEndDrag))]
        public event EventHandler EndDrag
        {
            add => Events.AddHandler(s_eventEndDrag, value);
            remove => Events.RemoveHandler(s_eventEndDrag, value);
        }

        public override Font Font
        {
            get
            {
                if (IsFontSet())
                {
                    return base.Font;
                }
                if (_defaultFont is null)
                {
                    // since toolstrip manager default font is thread static, hold onto a copy of the
                    // pointer in an instance variable for perf so we dont have to keep fishing into
                    // thread local storage for it.
                    _defaultFont = ToolStripManager.DefaultFont;
                }
                return _defaultFont;
            }
            set => base.Font = value;
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
            => DpiHelper.IsPerMonitorV2Awareness ?
               DpiHelper.LogicalToDeviceUnits(new Size(100, 25), DeviceDpi) :
               new Size(100, 25);

        protected override Padding DefaultPadding
        {
            get
            {
                // one pixel from the right edge to prevent the right border from painting over the
                // aligned-right toolstrip item.
                return _scaledDefaultPadding;
            }
        }

        protected override Padding DefaultMargin
        {
            get { return Padding.Empty; }
        }

        protected virtual DockStyle DefaultDock
        {
            get
            {
                return DockStyle.Top;
            }
        }

        protected virtual Padding DefaultGripMargin
        {
            get
            {
                if (_toolStripGrip != null)
                {
                    return _toolStripGrip.DefaultMargin;
                }
                else
                {
                    return _scaledDefaultGripMargin;
                }
            }
        }

        protected virtual bool DefaultShowItemToolTips
        {
            get
            {
                return true;
            }
        }

        [Browsable(false)]
        [SRDescription(nameof(SR.ToolStripDefaultDropDownDirectionDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public virtual ToolStripDropDownDirection DefaultDropDownDirection
        {
            get
            {
                ToolStripDropDownDirection direction = _toolStripDropDownDirection;
                if (direction == ToolStripDropDownDirection.Default)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        if (IsInToolStripPanel)
                        {
                            // parent can be null when we're swapping between ToolStripPanels.
                            DockStyle actualDock = (ParentInternal != null) ? ParentInternal.Dock : DockStyle.Left;
                            direction = (actualDock == DockStyle.Right) ? ToolStripDropDownDirection.Left : ToolStripDropDownDirection.Right;
                            if (DesignMode && actualDock == DockStyle.Left)
                            {
                                direction = ToolStripDropDownDirection.Right;
                            }
                        }
                        else
                        {
                            direction = ((Dock == DockStyle.Right) && (RightToLeft == RightToLeft.No)) ? ToolStripDropDownDirection.Left : ToolStripDropDownDirection.Right;
                            if (DesignMode && Dock == DockStyle.Left)
                            {
                                direction = ToolStripDropDownDirection.Right;
                            }
                        }
                    }
                    else
                    { // horizontal
                        DockStyle dock = Dock;
                        if (IsInToolStripPanel && ParentInternal != null)
                        {
                            dock = ParentInternal.Dock;  // we want the orientation of the ToolStripPanel;
                        }

                        if (dock == DockStyle.Bottom)
                        {
                            direction = (RightToLeft == RightToLeft.Yes) ? ToolStripDropDownDirection.AboveLeft : ToolStripDropDownDirection.AboveRight;
                        }
                        else
                        {
                            // assume Dock.Top
                            direction = (RightToLeft == RightToLeft.Yes) ? ToolStripDropDownDirection.BelowLeft : ToolStripDropDownDirection.BelowRight;
                        }
                    }
                }
                return direction;
            }
            set
            {
                // cant use Enum.IsValid as its not sequential
                switch (value)
                {
                    case ToolStripDropDownDirection.AboveLeft:
                    case ToolStripDropDownDirection.AboveRight:
                    case ToolStripDropDownDirection.BelowLeft:
                    case ToolStripDropDownDirection.BelowRight:
                    case ToolStripDropDownDirection.Left:
                    case ToolStripDropDownDirection.Right:
                    case ToolStripDropDownDirection.Default:
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripDropDownDirection));
                }

                _toolStripDropDownDirection = value;
            }
        }

        /// <summary>
        ///  Just here so we can add the default value attribute
        /// </summary>
        [DefaultValue(DockStyle.Top)]
        public override DockStyle Dock
        {
            get => base.Dock;
            set
            {
                if (value != Dock)
                {
                    using (new LayoutTransaction(this, this, PropertyNames.Dock))
                    using (new LayoutTransaction(ParentInternal, this, PropertyNames.Dock))
                    {
                        // We don't call base.Dock = value, because that would cause us to get 2 LocationChanged events.
                        // The first is when the parent gets a Layout due to the DockChange, and the second comes from when we
                        // change the orientation.  Instead we've duplicated the logic of Control.Dock.set here, but with a
                        // LayoutTransaction on the Parent as well.
                        DefaultLayout.SetDock(this, value);
                        UpdateLayoutStyle(Dock);
                    }
                    // This will cause the DockChanged event to fire.
                    OnDockChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Returns an owner window that can be used to
        ///  own a drop down.
        /// </summary>
        internal virtual NativeWindow DropDownOwnerWindow
        {
            get
            {
                if (_dropDownOwnerWindow is null)
                {
                    _dropDownOwnerWindow = new NativeWindow();
                }

                if (_dropDownOwnerWindow.Handle == IntPtr.Zero)
                {
                    CreateParams cp = new CreateParams
                    {
                        ExStyle = (int)User32.WS_EX.TOOLWINDOW
                    };
                    _dropDownOwnerWindow.CreateHandle(cp);
                }

                return _dropDownOwnerWindow;
            }
        }

        /// <summary>
        ///  Returns the drop target manager that all the hwndless
        ///  items and this ToolStrip share.  this is necessary as
        ///  RegisterDragDrop requires an HWND.
        /// </summary>
        internal ToolStripDropTargetManager DropTargetManager
        {
            get
            {
                if (_dropTargetManager is null)
                {
                    _dropTargetManager = new ToolStripDropTargetManager(this);
                }
                return _dropTargetManager;
            }
            set
            {
                _dropTargetManager = value;
            }
        }

        /// <summary>
        ///  Just here so we can add the default value attribute
        /// </summary>
        protected internal virtual ToolStripItemCollection DisplayedItems
        {
            get
            {
                if (_displayedItems is null)
                {
                    _displayedItems = new ToolStripItemCollection(this, false);
                }
                return _displayedItems;
            }
        }

        /// <summary>
        ///  Retreives the current display rectangle. The display rectangle
        ///  is the virtual display area that is used to layout components.
        ///  The position and dimensions of the Form's display rectangle
        ///  change during autoScroll.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = base.DisplayRectangle;

                if ((LayoutEngine is ToolStripSplitStackLayout) && (GripStyle == ToolStripGripStyle.Visible))
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        int gripwidth = Grip.GripThickness + Grip.Margin.Horizontal;
                        rect.Width -= gripwidth;
                        // in RTL.No we need to shift the rectangle
                        rect.X += (RightToLeft == RightToLeft.No) ? gripwidth : 0;
                    }
                    else
                    { // Vertical Grip placement
                        int gripheight = Grip.GripThickness + Grip.Margin.Vertical;
                        rect.Y += gripheight;
                        rect.Height -= gripheight;
                    }
                }
                return rect;
            }
        }

        /// <summary>
        ///  Forecolor really has no meaning for ToolStrips - so lets hide it
        /// </summary>
        [Browsable(false)]
        public new Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        /// <summary>
        ///  [ToolStrip ForeColorChanged event, overriden to turn browsing off.]
        /// </summary>
        [Browsable(false)]
        public new event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        private bool HasKeyboardInput
        {
            get
            {
                return (ContainsFocus || (ToolStripManager.ModalMenuFilter.InMenuMode && ToolStripManager.ModalMenuFilter.GetActiveToolStrip() == this));
            }
        }

        internal ToolStripGrip Grip
        {
            get
            {
                if (_toolStripGrip is null)
                {
                    _toolStripGrip = new ToolStripGrip
                    {
                        Overflow = ToolStripItemOverflow.Never,
                        Visible = _toolStripGripStyle == ToolStripGripStyle.Visible,
                        AutoSize = false,
                        ParentInternal = this,
                        Margin = DefaultGripMargin
                    };
                }
                return _toolStripGrip;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripGripStyleDescr))]
        [DefaultValue(ToolStripGripStyle.Visible)]
        public ToolStripGripStyle GripStyle
        {
            get
            {
                return _toolStripGripStyle;
            }
            set
            {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripGripStyle.Hidden, (int)ToolStripGripStyle.Visible))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripGripStyle));
                }
                if (_toolStripGripStyle != value)
                {
                    _toolStripGripStyle = value;
                    Grip.Visible = _toolStripGripStyle == ToolStripGripStyle.Visible;
                    LayoutTransaction.DoLayout(this, this, PropertyNames.GripStyle);
                }
            }
        }

        [Browsable(false)]
        public ToolStripGripDisplayStyle GripDisplayStyle
        {
            get
            {
                return (LayoutStyle == ToolStripLayoutStyle.HorizontalStackWithOverflow) ? ToolStripGripDisplayStyle.Vertical
                                                                     : ToolStripGripDisplayStyle.Horizontal;
            }
        }

        /// <summary>
        ///  The external spacing between the grip and the padding of the ToolStrip and the first item in the collection
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.ToolStripGripDisplayStyleDescr))]
        public Padding GripMargin
        {
            get
            {
                return Grip.Margin;
            }
            set
            {
                Grip.Margin = value;
            }
        }

        /// <summary>
        ///  The boundaries of the grip on the ToolStrip.  If it is invisible - returns Rectangle.Empty.
        /// </summary>
        [Browsable(false)]
        public Rectangle GripRectangle
        {
            get
            {
                return (GripStyle == ToolStripGripStyle.Visible) ? Grip.Bounds : Rectangle.Empty;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool HasChildren
        {
            get => base.HasChildren;
        }

        internal bool HasVisibleItems
        {
            get
            {
                if (!IsHandleCreated)
                {
                    foreach (ToolStripItem item in Items)
                    {
                        if (((IArrangedElement)item).ParticipatesInLayout)
                        {
                            // set in the state so that when the handle is created, we're accurate.
                            SetToolStripState(STATE_HASVISIBLEITEMS, true);
                            return true;
                        }
                    }
                    SetToolStripState(STATE_HASVISIBLEITEMS, false);
                    return false;
                }
                // after the handle is created, we start layout... so this state is cached.
                return GetToolStripState(STATE_HASVISIBLEITEMS);
            }
            set
            {
                SetToolStripState(STATE_HASVISIBLEITEMS, value);
            }
        }

        /// <summary>
        ///  Gets the Horizontal Scroll bar for this ScrollableControl.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public HScrollProperties HorizontalScroll
        {
            get => base.HorizontalScroll;
        }

        [DefaultValue(typeof(Size), "16,16")]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripImageScalingSizeDescr))]
        public Size ImageScalingSize
        {
            get
            {
                return ImageScalingSizeInternal;
            }
            set
            {
                ImageScalingSizeInternal = value;
            }
        }

        internal virtual Size ImageScalingSizeInternal
        {
            get
            {
                return _imageScalingSize;
            }
            set
            {
                if (_imageScalingSize != value)
                {
                    _imageScalingSize = value;

                    LayoutTransaction.DoLayoutIf((Items.Count > 0), this, this, PropertyNames.ImageScalingSize);
                    foreach (ToolStripItem item in Items)
                    {
                        item.OnImageScalingSizeChanged(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the <see cref='Forms.ImageList'/> that contains the <see cref='Image'/> displayed on a label control.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripImageListDescr))]
        [Browsable(false)]
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
                    EventHandler handler = new EventHandler(ImageListRecreateHandle);

                    // Remove the previous imagelist handle recreate handler
                    //
                    if (_imageList != null)
                    {
                        _imageList.RecreateHandle -= handler;
                    }

                    _imageList = value;

                    // Add the new imagelist handle recreate handler
                    //
                    if (value != null)
                    {
                        value.RecreateHandle += handler;
                    }

                    foreach (ToolStripItem item in Items)
                    {
                        item.InvalidateImageListImage();
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
        /// </summary>
        internal override bool IsMnemonicsListenerAxSourced
        {
            get
            {
                return true;
            }
        }

        internal bool IsInToolStripPanel
        {
            get
            {
                return ToolStripPanelRow != null;
            }
        }

        /// <summary> indicates whether the user is currently
        ///  moving the toolstrip from one toolstrip container
        ///  to another
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsCurrentlyDragging
        {
            get
            {
                return GetToolStripState(STATE_DRAGGING);
            }
        }

        /// <summary>
        ///  indicates if the SetBoundsCore is called thru Locationchanging.
        /// </summary>
        private bool IsLocationChanging
        {
            get
            {
                return GetToolStripState(STATE_LOCATIONCHANGING);
            }
        }

        /// <summary>
        ///  The items that belong to this ToolStrip.
        ///  Note - depending on space and layout preferences, not all items
        ///  in this collection will be displayed.  They may not even be displayed
        ///  on this ToolStrip (say in the case where we're overflowing the item).
        ///  The collection of _Displayed_ items is the DisplayedItems collection.
        ///  The displayed items collection also includes things like the OverflowButton
        ///  and the Grip.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.ToolStripItemsDescr))]
        [MergableProperty(false)]
        public virtual ToolStripItemCollection Items
        {
            get
            {
                if (_toolStripItemCollection is null)
                {
                    _toolStripItemCollection = new ToolStripItemCollection(this, true);
                }
                return _toolStripItemCollection;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripItemAddedDescr))]
        public event ToolStripItemEventHandler ItemAdded
        {
            add => Events.AddHandler(s_eventItemAdded, value);
            remove => Events.RemoveHandler(s_eventItemAdded, value);
        }

        /// <summary>
        ///  Occurs when the control is clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ToolStripItemOnClickDescr))]
        public event ToolStripItemClickedEventHandler ItemClicked
        {
            add => Events.AddHandler(s_eventItemClicked, value);
            remove => Events.RemoveHandler(s_eventItemClicked, value);
        }

        /// <summary>
        ///  we have a backbuffer for painting items... this is cached to be the size of the largest
        ///  item in the collection - and is cached in OnPaint, and disposed when the toolstrip
        ///  is no longer visible.
        ///
        ///  [: toolstrip - main hdc       ] ← visible to user
        ///  [ toolstrip double buffer hdc ] ← onpaint hands us this buffer, after we're done DBuf is copied to "main hdc"/
        ///  [tsi dc] ← we copy the background from the DBuf, then paint the item into this DC, then BitBlt back up to DBuf
        ///
        ///  This is done because GDI wont honor GDI+ TranslateTransform.  We used to use DCMapping to change the viewport
        ///  origin and clipping rect of the toolstrip double buffer hdc to paint each item, but this proves costly
        ///  because you need to allocate GDI+ Graphics objects for every single item.  This method allows us to only
        ///  allocate 1 Graphics object and share it between all the items in OnPaint.
        /// </summary>
        private CachedItemHdcInfo ItemHdcInfo
        {
            get
            {
                if (_cachedItemHdcInfo is null)
                {
                    _cachedItemHdcInfo = new CachedItemHdcInfo();
                }
                return _cachedItemHdcInfo;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripItemRemovedDescr))]
        public event ToolStripItemEventHandler ItemRemoved
        {
            add => Events.AddHandler(s_eventItemRemoved, value);
            remove => Events.RemoveHandler(s_eventItemRemoved, value);
        }

        /// <summary> handy check for painting and sizing </summary>
        [Browsable(false)]
        public bool IsDropDown
        {
            get { return (this is ToolStripDropDown); }
        }

        internal bool IsDisposingItems
        {
            get
            {
                return GetToolStripState(STATE_DISPOSINGITEMS);
            }
        }

        /// <summary>
        ///  The OnDrag[blah] methods that will be called if AllowItemReorder is true.
        ///
        ///  This allows us to have methods that handle drag/drop of the ToolStrip items
        ///  without calling back on the user's code
        /// </summary>
        internal IDropTarget ItemReorderDropTarget { get; set; }

        /// <summary>
        ///  The OnQueryContinueDrag and OnGiveFeedback methods that will be called if
        ///  AllowItemReorder is true.
        ///
        ///  This allows us to have methods that handle drag/drop of the ToolStrip items
        ///  without calling back on the user's code
        /// </summary>
        internal ISupportOleDropSource ItemReorderDropSource { get; set; }

        internal bool IsInDesignMode
        {
            get
            {
                return DesignMode;
            }
        }

        internal bool IsSelectionSuspended
        {
            get { return GetToolStripState(STATE_LASTMOUSEDOWNEDITEMCAPTURE); }
        }

        internal ToolStripItem LastMouseDownedItem
        {
            get
            {
                if (_lastMouseDownedItem != null &&
                    (_lastMouseDownedItem.IsDisposed || _lastMouseDownedItem.ParentInternal != this))
                {
                    // handle disposal, parent changed since we last mouse downed.
                    _lastMouseDownedItem = null;
                }
                return _lastMouseDownedItem;
            }
        }

        [DefaultValue(null)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LayoutSettings LayoutSettings { get; set; }

        /// <summary>
        ///  Specifies whether we're horizontal or vertical
        /// </summary>
        [SRDescription(nameof(SR.ToolStripLayoutStyle))]
        [SRCategory(nameof(SR.CatLayout))]
        [AmbientValue(ToolStripLayoutStyle.StackWithOverflow)]
        public ToolStripLayoutStyle LayoutStyle
        {
            get
            {
                if (_layoutStyle == ToolStripLayoutStyle.StackWithOverflow)
                {
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            return ToolStripLayoutStyle.HorizontalStackWithOverflow;
                        case Orientation.Vertical:
                            return ToolStripLayoutStyle.VerticalStackWithOverflow;
                    }
                }
                return _layoutStyle;
            }
            set
            {
                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripLayoutStyle.StackWithOverflow, (int)ToolStripLayoutStyle.Table))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripLayoutStyle));
                }
                if (_layoutStyle != value)
                {
                    _layoutStyle = value;

                    switch (value)
                    {
                        case ToolStripLayoutStyle.Flow:
                            if (!(_layoutEngine is FlowLayout))
                            {
                                _layoutEngine = FlowLayout.Instance;
                            }
                            // Orientation really only applies to split stack layout (which swaps based on Dock, ToolStripPanel location)
                            UpdateOrientation(Orientation.Horizontal);
                            break;
                        case ToolStripLayoutStyle.Table:

                            if (!(_layoutEngine is TableLayout))
                            {
                                _layoutEngine = TableLayout.Instance;
                            }
                            // Orientation really only applies to split stack layout (which swaps based on Dock, ToolStripPanel location)
                            UpdateOrientation(Orientation.Horizontal);
                            break;
                        case ToolStripLayoutStyle.StackWithOverflow:
                        case ToolStripLayoutStyle.HorizontalStackWithOverflow:
                        case ToolStripLayoutStyle.VerticalStackWithOverflow:
                        default:

                            if (value != ToolStripLayoutStyle.StackWithOverflow)
                            {
                                UpdateOrientation((value == ToolStripLayoutStyle.VerticalStackWithOverflow) ? Orientation.Vertical : Orientation.Horizontal);
                            }
                            else
                            {
                                if (IsInToolStripPanel)
                                {
                                    UpdateLayoutStyle(ToolStripPanelRow.Orientation);
                                }
                                else
                                {
                                    UpdateLayoutStyle(Dock);
                                }
                            }
                            if (!(_layoutEngine is ToolStripSplitStackLayout))
                            {
                                _layoutEngine = new ToolStripSplitStackLayout(this);
                            }
                            break;
                    }

                    using (LayoutTransaction.CreateTransactionIf(IsHandleCreated, this, this, PropertyNames.LayoutStyle))
                    {
                        LayoutSettings = CreateLayoutSettings(_layoutStyle);
                    }
                    OnLayoutStyleChanged(EventArgs.Empty);
                }
            }
        }
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripLayoutCompleteDescr))]
        public event EventHandler LayoutCompleted
        {
            add => Events.AddHandler(s_eventLayoutCompleted, value);
            remove => Events.RemoveHandler(s_eventLayoutCompleted, value);
        }

        internal bool LayoutRequired { get; set; }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripLayoutStyleChangedDescr))]
        public event EventHandler LayoutStyleChanged
        {
            add => Events.AddHandler(s_eventLayoutStyleChanged, value);
            remove => Events.RemoveHandler(s_eventLayoutStyleChanged, value);
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                //
                return _layoutEngine;
            }
        }

        internal event ToolStripLocationCancelEventHandler LocationChanging
        {
            add => Events.AddHandler(s_eventLocationChanging, value);
            remove => Events.RemoveHandler(s_eventLocationChanging, value);
        }

        protected internal virtual Size MaxItemSize
        {
            get
            {
                return DisplayRectangle.Size;
            }
        }

        internal bool MenuAutoExpand
        {
            get
            {
                if (!DesignMode)
                {
                    if (GetToolStripState(STATE_MENUAUTOEXPAND))
                    {
                        if (!IsDropDown && !ToolStripManager.ModalMenuFilter.InMenuMode)
                        {
                            SetToolStripState(STATE_MENUAUTOEXPAND, false);
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            set
            {
                if (!DesignMode)
                {
                    SetToolStripState(STATE_MENUAUTOEXPAND, value);
                }
            }
        }

        internal Stack<MergeHistory> MergeHistoryStack
        {
            get
            {
                if (_mergeHistoryStack is null)
                {
                    _mergeHistoryStack = new Stack<MergeHistory>();
                }
                return _mergeHistoryStack;
            }
        }

        private MouseHoverTimer MouseHoverTimer
        {
            get
            {
                if (_mouseHoverTimer is null)
                {
                    _mouseHoverTimer = new MouseHoverTimer();
                }
                return _mouseHoverTimer;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ToolStripOverflowButton OverflowButton
        {
            get
            {
                if (_toolStripOverflowButton is null)
                {
                    _toolStripOverflowButton = new ToolStripOverflowButton(this)
                    {
                        Overflow = ToolStripItemOverflow.Never,
                        ParentInternal = this,
                        Alignment = ToolStripItemAlignment.Right
                    };
                    _toolStripOverflowButton.Size = _toolStripOverflowButton.GetPreferredSize(DisplayRectangle.Size - Padding.Size);
                }
                return _toolStripOverflowButton;
            }
        }

        //
        //

        internal ToolStripItemCollection OverflowItems
        {
            get
            {
                if (_overflowItems is null)
                {
                    _overflowItems = new ToolStripItemCollection(this, false);
                }
                return _overflowItems;
            }
        }

        [Browsable(false)]
        public Orientation Orientation { get; private set; } = Orientation.Horizontal;

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripPaintGripDescr))]
        public event PaintEventHandler PaintGrip
        {
            add => Events.AddHandler(s_eventPaintGrip, value);
            remove => Events.RemoveHandler(s_eventPaintGrip, value);
        }

        internal RestoreFocusMessageFilter RestoreFocusFilter
        {
            get
            {
                if (_restoreFocusFilter is null)
                {
                    _restoreFocusFilter = new RestoreFocusMessageFilter(this);
                }
                return _restoreFocusFilter;
            }
        }

        internal ToolStripPanelCell ToolStripPanelCell
        {
            get { return ((ISupportToolStripPanel)this).ToolStripPanelCell; }
        }

        internal ToolStripPanelRow ToolStripPanelRow
        {
            get { return ((ISupportToolStripPanel)this).ToolStripPanelRow; }
        }

        // fetches the Cell associated with this toolstrip.
        ToolStripPanelCell ISupportToolStripPanel.ToolStripPanelCell
        {
            get
            {
                ToolStripPanelCell toolStripPanelCell = null;
                if (!IsDropDown && !IsDisposed)
                {
                    if (Properties.ContainsObject(ToolStrip.s_propToolStripPanelCell))
                    {
                        toolStripPanelCell = (ToolStripPanelCell)Properties.GetObject(ToolStrip.s_propToolStripPanelCell);
                    }
                    else
                    {
                        toolStripPanelCell = new ToolStripPanelCell(this);
                        Properties.SetObject(ToolStrip.s_propToolStripPanelCell, toolStripPanelCell);
                    }
                }
                return toolStripPanelCell;
            }
        }

        ToolStripPanelRow ISupportToolStripPanel.ToolStripPanelRow
        {
            get
            {
                ToolStripPanelCell cell = ToolStripPanelCell;
                if (cell is null)
                {
                    return null;
                }
                return ToolStripPanelCell.ToolStripPanelRow;
            }
            set
            {
                ToolStripPanelRow oldToolStripPanelRow = ToolStripPanelRow;

                if (oldToolStripPanelRow != value)
                {
                    ToolStripPanelCell cell = ToolStripPanelCell;
                    if (cell is null)
                    {
                        return;
                    }
                    cell.ToolStripPanelRow = value;

                    if (value != null)
                    {
                        if (oldToolStripPanelRow is null || oldToolStripPanelRow.Orientation != value.Orientation)
                        {
                            if (_layoutStyle == ToolStripLayoutStyle.StackWithOverflow)
                            {
                                UpdateLayoutStyle(value.Orientation);
                            }
                            else
                            {
                                UpdateOrientation(value.Orientation);
                            }
                        }
                    }
                    else
                    {
                        if (oldToolStripPanelRow != null && oldToolStripPanelRow.ControlsInternal.Contains(this))
                        {
                            oldToolStripPanelRow.ControlsInternal.Remove(this);
                        }
                        UpdateLayoutStyle(Dock);
                    }
                }
            }
        }

        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.ToolStripStretchDescr))]
        public bool Stretch
        {
            get
            {
                return GetToolStripState(STATE_STRETCH);
            }
            set
            {
                if (Stretch != value)
                {
                    SetToolStripState(STATE_STRETCH, value);
                }
            }
        }

        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  The renderer is used to paint the hwndless ToolStrip items.  If someone wanted to
        ///  change the "Hot" look of all of their buttons to be a green triangle, they should
        ///  create a class that derives from ToolStripRenderer, assign it to this property and call
        ///  invalidate.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripRenderer Renderer
        {
            get
            {
                if (IsDropDown)
                {
                    // PERF: since this is called a lot we dont want to make it virtual
                    ToolStripDropDown dropDown = this as ToolStripDropDown;
                    if (dropDown is ToolStripOverflow || dropDown.IsAutoGenerated)
                    {
                        if (dropDown.OwnerToolStrip != null)
                        {
                            return dropDown.OwnerToolStrip.Renderer;
                        }
                    }
                }
                if (RenderMode == ToolStripRenderMode.ManagerRenderMode)
                {
                    return ToolStripManager.Renderer;
                }
                // always return a valid renderer so our paint code
                // doesn't have to be bogged down by checks for null.

                SetToolStripState(STATE_USEDEFAULTRENDERER, false);
                if (_renderer is null)
                {
                    Renderer = ToolStripManager.CreateRenderer(RenderMode);
                }
                return _renderer;
            }
            set
            {
                // if the value happens to be null, the next get
                // will autogenerate a new ToolStripRenderer.
                if (_renderer != value)
                {
                    SetToolStripState(STATE_USEDEFAULTRENDERER, (value is null));
                    _renderer = value;
                    _currentRendererType = (_renderer != null) ? _renderer.GetType() : typeof(Type);
                    OnRendererChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler RendererChanged
        {
            add => Events.AddHandler(s_eventRendererChanged, value);
            remove => Events.RemoveHandler(s_eventRendererChanged, value);
        }

        [SRDescription(nameof(SR.ToolStripRenderModeDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public ToolStripRenderMode RenderMode
        {
            get
            {
                if (GetToolStripState(STATE_USEDEFAULTRENDERER))
                {
                    return ToolStripRenderMode.ManagerRenderMode;
                }
                if (_renderer != null && !_renderer.IsAutoGenerated)
                {
                    return ToolStripRenderMode.Custom;
                }
                // check the type of the currently set renderer.
                // types are cached as this may be called frequently.
                if (_currentRendererType == ToolStripManager.s_professionalRendererType)
                {
                    return ToolStripRenderMode.Professional;
                }
                if (_currentRendererType == ToolStripManager.s_systemRendererType)
                {
                    return ToolStripRenderMode.System;
                }
                return ToolStripRenderMode.Custom;
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripRenderMode.Custom, (int)ToolStripRenderMode.ManagerRenderMode))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripRenderMode));
                }
                if (value == ToolStripRenderMode.Custom)
                {
                    throw new NotSupportedException(SR.ToolStripRenderModeUseRendererPropertyInstead);
                }

                if (value == ToolStripRenderMode.ManagerRenderMode)
                {
                    if (!GetToolStripState(STATE_USEDEFAULTRENDERER))
                    {
                        SetToolStripState(STATE_USEDEFAULTRENDERER, true);
                        OnRendererChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    SetToolStripState(STATE_USEDEFAULTRENDERER, false);
                    Renderer = ToolStripManager.CreateRenderer(value);
                }
            }
        }

        /// <summary>
        ///  ToolStripItems need to access this to determine if they should be showing underlines
        ///  for their accelerators.  Since they are not HWNDs, and this method is protected on control
        ///  we need a way for them to get at it.
        /// </summary>
        internal bool ShowKeyboardCuesInternal
        {
            get
            {
                return ShowKeyboardCues;
            }
        }

        [DefaultValue(true)]
        [SRDescription(nameof(SR.ToolStripShowItemToolTipsDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public bool ShowItemToolTips
        {
            get
            {
                return _showItemToolTips;
            }
            set
            {
                if (_showItemToolTips != value)
                {
                    _showItemToolTips = value;
                    if (!_showItemToolTips)
                    {
                        UpdateToolTip(null);
                    }

                    ToolTip internalToolTip = ToolTip;
                    foreach (ToolStripItem item in Items)
                    {
                        if (_showItemToolTips)
                        {
                            KeyboardToolTipStateMachine.Instance.Hook(item, internalToolTip);
                        }
                        else
                        {
                            KeyboardToolTipStateMachine.Instance.Unhook(item, internalToolTip);
                        }
                    }

                    // If the overflow button has not been created, don't check its properties
                    // since this will force its creating and cause a re-layout of the control
                    if (_toolStripOverflowButton != null && OverflowButton.HasDropDownItems)
                    {
                        OverflowButton.DropDown.ShowItemToolTips = value;
                    }
                }
            }
        }

        /// <summary> internal lookup table for shortcuts... intended to speed search time </summary>
        internal Hashtable Shortcuts
        {
            get
            {
                if (_shortcuts is null)
                {
                    _shortcuts = new Hashtable(1);
                }
                return _shortcuts;
            }
        }

        /// <summary>
        ///  Indicates whether the user can give the focus to this control using the TAB
        ///  key. This property is read-only.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [DispId((int)Ole32.DispatchID.TABSTOP)]
        [SRDescription(nameof(SR.ControlTabStopDescr))]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        /// <summary> this is the ToolTip used for the individual items
        ///  it only works if ShowItemToolTips = true
        /// </summary>
        internal ToolTip ToolTip
        {
            get
            {
                ToolTip toolTip;
                if (!Properties.ContainsObject(ToolStrip.s_propToolTip))
                {
                    toolTip = new ToolTip();
                    Properties.SetObject(ToolStrip.s_propToolTip, toolTip);
                }
                else
                {
                    toolTip = (ToolTip)Properties.GetObject(ToolStrip.s_propToolTip);
                }
                return toolTip;
            }
        }

        [DefaultValue(ToolStripTextDirection.Horizontal)]
        [SRDescription(nameof(SR.ToolStripTextDirectionDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public virtual ToolStripTextDirection TextDirection
        {
            get
            {
                ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
                if (Properties.ContainsObject(ToolStrip.s_propTextDirection))
                {
                    textDirection = (ToolStripTextDirection)Properties.GetObject(ToolStrip.s_propTextDirection);
                }

                if (textDirection == ToolStripTextDirection.Inherit)
                {
                    textDirection = ToolStripTextDirection.Horizontal;
                }

                return textDirection;
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripTextDirection.Inherit, (int)ToolStripTextDirection.Vertical270))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripTextDirection));
                }
                Properties.SetObject(ToolStrip.s_propTextDirection, value);

                using (new LayoutTransaction(this, this, "TextDirection"))
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        Items[i].OnOwnerTextDirectionChanged();
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the Vertical Scroll bar for this ScrollableControl.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public VScrollProperties VerticalScroll
        {
            get => base.VerticalScroll;
        }

        void ISupportToolStripPanel.BeginDrag()
        {
            OnBeginDrag(EventArgs.Empty);
        }

        // Internal so that it's not a public API.
        internal virtual void ChangeSelection(ToolStripItem nextItem)
        {
            if (nextItem != null)
            {
                ToolStripControlHost controlHost = nextItem as ToolStripControlHost;
                // if we contain focus, we should set focus to ourselves
                // so we get the focus off the thing that's currently focused
                // e.g. go from a text box to a toolstrip button
                if (ContainsFocus && !Focused)
                {
                    Focus();
                    if (controlHost is null)
                    {
                        // if nextItem IS a toolstripcontrolhost, we're going to focus it anyways
                        // we only fire KeyboardActive when "focusing" a non-hwnd backed item
                        KeyboardActive = true;
                    }
                }
                if (controlHost != null)
                {
                    if (_hwndThatLostFocus == IntPtr.Zero)
                    {
                        SnapFocus(User32.GetFocus());
                    }
                    controlHost.Control.Select();
                    controlHost.Control.Focus();
                }

                nextItem.Select();

                if (nextItem is ToolStripMenuItem tsNextItem && !IsDropDown)
                {
                    // only toplevel menus auto expand when the selection changes.
                    tsNextItem.HandleAutoExpansion();
                }
            }
        }

        protected virtual LayoutSettings CreateLayoutSettings(ToolStripLayoutStyle layoutStyle)
        {
            switch (layoutStyle)
            {
                case ToolStripLayoutStyle.Flow:
                    return new FlowLayoutSettings(this);
                case ToolStripLayoutStyle.Table:
                    return new TableLayoutSettings(this);
                default:
                    return null;
            }
        }

        protected internal virtual ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
        {
            if (text == "-")
            {
                return new ToolStripSeparator();
            }
            else
            {
                return new ToolStripButton(text, image, onClick);
            }
        }

        private void ClearAllSelections()
        {
            ClearAllSelectionsExcept(null);
        }

        private void ClearAllSelectionsExcept(ToolStripItem item)
        {
            Rectangle regionRect = (item is null) ? Rectangle.Empty : item.Bounds;
            Region region = null;

            try
            {
                for (int i = 0; i < DisplayedItems.Count; i++)
                {
                    if (DisplayedItems[i] == item)
                    {
                        continue;
                    }
                    else if (item != null && DisplayedItems[i].Pressed)
                    {
                        //

                        if (DisplayedItems[i] is ToolStripDropDownItem dropDownItem && dropDownItem.HasDropDownItems)
                        {
                            dropDownItem.AutoHide(item);
                        }
                    }
                    bool invalidate = false;
                    if (DisplayedItems[i].Selected)
                    {
                        DisplayedItems[i].Unselect();
                        Debug.WriteLineIf(s_selectionDebug.TraceVerbose, "[SelectDBG ClearAllSelectionsExcept] Unselecting " + DisplayedItems[i].Text);
                        invalidate = true;
                    }

                    if (invalidate)
                    {
                        // since regions are heavy weight - only use if we need it.
                        if (region is null)
                        {
                            region = new Region(regionRect);
                        }
                        region.Union(DisplayedItems[i].Bounds);
                    }
                }

                // force an WM_PAINT to happen now to instantly reflect the selection change.
                if (region != null)
                {
                    Invalidate(region, true);
                    Update();
                }
                else if (regionRect != Rectangle.Empty)
                {
                    Invalidate(regionRect, true);
                    Update();
                }
            }
            finally
            {
                if (region != null)
                {
                    region.Dispose();
                }
            }
            // fire accessibility
            if (IsHandleCreated && item != null)
            {
                int focusIndex = DisplayedItems.IndexOf(item);
                AccessibilityNotifyClients(AccessibleEvents.Focus, focusIndex);
            }
        }

        internal void ClearInsertionMark()
        {
            if (_lastInsertionMarkRect != Rectangle.Empty)
            {
                // stuff away the lastInsertionMarkRect
                // and clear it out _before_ we call paint OW
                // the call to invalidate wont help as it will get
                // repainted.
                Rectangle invalidate = _lastInsertionMarkRect;
                _lastInsertionMarkRect = Rectangle.Empty;

                Invalidate(invalidate);
            }
        }
        private void ClearLastMouseDownedItem()
        {
            ToolStripItem lastItem = _lastMouseDownedItem;
            _lastMouseDownedItem = null;
            if (IsSelectionSuspended)
            {
                SetToolStripState(STATE_LASTMOUSEDOWNEDITEMCAPTURE, false);
                if (lastItem != null)
                {
                    lastItem.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ToolStripOverflow overflow = GetOverflow();

                try
                {
                    SuspendLayout();
                    if (overflow != null)
                    {
                        overflow.SuspendLayout();
                    }
                    // if there's a problem in config, dont be a leaker.
                    SetToolStripState(STATE_DISPOSINGITEMS, true);
                    _lastMouseDownedItem = null;

                    HookStaticEvents(/*hook=*/false);

                    if (Properties.GetObject(ToolStrip.s_propToolStripPanelCell) is ToolStripPanelCell toolStripPanelCell)
                    {
                        toolStripPanelCell.Dispose();
                    }

                    if (_cachedItemHdcInfo != null)
                    {
                        _cachedItemHdcInfo.Dispose();
                    }

                    if (_mouseHoverTimer != null)
                    {
                        _mouseHoverTimer.Dispose();
                    }

                    ToolTip toolTip = (ToolTip)Properties.GetObject(ToolStrip.s_propToolTip);
                    if (toolTip != null)
                    {
                        toolTip.Dispose();
                    }

                    if (!Items.IsReadOnly)
                    {
                        // only dispose the items we actually own.
                        for (int i = Items.Count - 1; i >= 0; i--)
                        {
                            Items[i].Dispose();
                        }
                        Items.Clear();
                    }
                    // clean up items not in the Items list
                    if (_toolStripGrip != null)
                    {
                        _toolStripGrip.Dispose();
                    }
                    if (_toolStripOverflowButton != null)
                    {
                        _toolStripOverflowButton.Dispose();
                    }

                    // remove the restore focus filter
                    if (_restoreFocusFilter != null)
                    {
                        // PERF,

                        Application.ThreadContext.FromCurrent().RemoveMessageFilter(_restoreFocusFilter);
                        _restoreFocusFilter = null;
                    }

                    // exit menu mode if necessary.
                    bool exitMenuMode = false;
                    if (ToolStripManager.ModalMenuFilter.GetActiveToolStrip() == this)
                    {
                        exitMenuMode = true;
                    }
                    ToolStripManager.ModalMenuFilter.RemoveActiveToolStrip(this);
                    // if we were the last toolstrip in the queue, exit menu mode.
                    if (exitMenuMode && ToolStripManager.ModalMenuFilter.GetActiveToolStrip() is null)
                    {
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "Exiting menu mode because we're the last toolstrip in the queue, and we've disposed.");
                        ToolStripManager.ModalMenuFilter.ExitMenuMode();
                    }

                    ToolStripManager.ToolStrips.Remove(this);

                    _imageList?.Dispose();
                }
                finally
                {
                    ResumeLayout(false);
                    if (overflow != null)
                    {
                        overflow.ResumeLayout(false);
                    }
                    SetToolStripState(STATE_DISPOSINGITEMS, false);
                }
            }
            base.Dispose(disposing);
        }

        internal void DoLayoutIfHandleCreated(ToolStripItemEventArgs e)
        {
            if (IsHandleCreated)
            {
                LayoutTransaction.DoLayout(this, e.Item, PropertyNames.Items);
                Invalidate();
                // Adding this item may have added it to the overflow
                // However, we can't check if it's in OverflowItems, because
                // it gets added there in Layout, and layout might be suspended.
                if (CanOverflow && OverflowButton.HasDropDown)
                {
                    if (DeferOverflowDropDownLayout())
                    {
                        CommonProperties.xClearPreferredSizeCache(OverflowButton.DropDown);
                        OverflowButton.DropDown.LayoutRequired = true;
                    }
                    else
                    {
                        LayoutTransaction.DoLayout(OverflowButton.DropDown, e.Item, PropertyNames.Items);
                        OverflowButton.DropDown.Invalidate();
                    }
                }
            }
            else
            {
                // next time we fetch the preferred size, recalc it.
                CommonProperties.xClearPreferredSizeCache(this);
                LayoutRequired = true;
                if (CanOverflow && OverflowButton.HasDropDown)
                {
                    OverflowButton.DropDown.LayoutRequired = true;
                }
            }
        }

        private bool DeferOverflowDropDownLayout()
        {
            return IsLayoutSuspended
                || !OverflowButton.DropDown.Visible
                || !OverflowButton.DropDown.IsHandleCreated;
        }

        void ISupportToolStripPanel.EndDrag()
        {
            ToolStripPanel.ClearDragFeedback();
            OnEndDrag(EventArgs.Empty);
        }

        internal ToolStripOverflow GetOverflow()
        {
            return (_toolStripOverflowButton is null || !_toolStripOverflowButton.HasDropDown) ? null : _toolStripOverflowButton.DropDown as ToolStripOverflow;
        }
        internal byte GetMouseId()
        {
            // never return 0 as the mousedown ID, this is the "reset" value.
            if (_mouseDownID == 0)
            {
                _mouseDownID++;
            }
            return _mouseDownID;
        }
        internal virtual ToolStripItem GetNextItem(ToolStripItem start, ArrowDirection direction, bool rtlAware)
        {
            if (rtlAware && RightToLeft == RightToLeft.Yes)
            {
                if (direction == ArrowDirection.Right)
                {
                    direction = ArrowDirection.Left;
                }
                else if (direction == ArrowDirection.Left)
                {
                    direction = ArrowDirection.Right;
                }
            }
            return GetNextItem(start, direction);
        }

        /// <summary>
        ///  Gets the next item from the given start item in the direction specified.
        ///  - This function wraps if at the end
        ///  - This function will only surf the items in the current container
        ///  - Overriding this function will change the tab ordering and accessible child ordering.
        /// </summary>
        public virtual ToolStripItem GetNextItem(ToolStripItem start, ArrowDirection direction)
        {
            switch (direction)
            {
                case ArrowDirection.Right:
                    return GetNextItemHorizontal(start, forward: true);
                case ArrowDirection.Left:
                    bool isRtl = RightToLeft == RightToLeft.Yes;
                    bool forward = (LastKeyData == (Keys.Shift | Keys.Tab) && !isRtl) || (LastKeyData == Keys.Tab && isRtl);
                    return GetNextItemHorizontal(start, forward);
                case ArrowDirection.Down:
                    return GetNextItemVertical(start, down: true);
                case ArrowDirection.Up:
                    return GetNextItemVertical(start, down: false);
                default:
                    throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(ArrowDirection));
            }
        }

        /// <remarks>
        ///  Helper function for GetNextItem - do not directly call this.
        /// </remarks>
        private ToolStripItem GetNextItemHorizontal(ToolStripItem start, bool forward)
        {
            if (DisplayedItems.Count <= 0)
            {
                return null;
            }

            ToolStripDropDown dropDown = this as ToolStripDropDown;

            if (start is null)
            {
                // The navigation should be consistent when navigating in forward and
                // backward direction entering the toolstrip, it means that the first
                // toolstrip item should be selected irrespectively TAB or SHIFT+TAB
                // is pressed.
                start = GetStartItem(forward, dropDown != null);
            }

            int current = DisplayedItems.IndexOf(start);
            if (current == -1)
            {
                Debug.WriteLineIf(s_selectionDebug.TraceVerbose, "Started from a visible = false item");
                return null;
            }

            Debug.WriteLineIf(s_selectionDebug.TraceVerbose && (current != -1), "[SelectDBG GetNextToolStripItem] Last selected item was " + ((current != -1) ? DisplayedItems[current].Text : ""));
            Debug.WriteLineIf(s_selectionDebug.TraceVerbose && (current == -1), "[SelectDBG GetNextToolStripItem] Last selected item was null");

            int count = DisplayedItems.Count;

            do
            {
                if (forward)
                {
                    current = ++current % count;
                }
                else
                {  // provide negative wrap if necessary
                    current = (--current < 0) ? count + current : current;
                }

                if (dropDown?.OwnerItem != null && dropDown.OwnerItem.IsInDesignMode)
                {
                    return DisplayedItems[current];
                }

                if (DisplayedItems[current].CanKeyboardSelect)
                {
                    Debug.WriteLineIf(s_selectionDebug.TraceVerbose, "[SelectDBG GetNextToolStripItem] selecting " + DisplayedItems[current].Text);
                    //ClearAllSelectionsExcept(Items[current]);
                    return DisplayedItems[current];
                }
            } while (DisplayedItems[current] != start);

            return null;
        }

        private ToolStripItem GetStartItem(bool forward, bool isDropDown)
        {
            if (forward)
            {
                return DisplayedItems[DisplayedItems.Count - 1];
            }

            if (!isDropDown)
            {
                // For the drop-down up-directed loop should be preserved.
                // So if the current item is topmost, then the bottom item should be selected on up-key press.
                return DisplayedItems[DisplayedItems.Count > 1 ? 1 : 0];
            }

            return DisplayedItems[0];
        }

        /// <remarks>
        ///  Helper function for GetNextItem - do not directly call this.
        /// </remarks>
        private ToolStripItem GetNextItemVertical(ToolStripItem selectedItem, bool down)
        {
            ToolStripItem tanWinner = null;
            ToolStripItem hypotenuseWinner = null;

            double minHypotenuse = double.MaxValue;
            double minTan = double.MaxValue;
            double hypotenuseOfTanWinner = double.MaxValue;
            double tanOfHypotenuseWinner = double.MaxValue;

            if (selectedItem is null)
            {
                ToolStripItem item = GetNextItemHorizontal(selectedItem, down);
                return item;
            }

            if (this is ToolStripDropDown dropDown && dropDown.OwnerItem != null && (dropDown.OwnerItem.IsInDesignMode || (dropDown.OwnerItem.Owner != null && dropDown.OwnerItem.Owner.IsInDesignMode)))
            {
                ToolStripItem item = GetNextItemHorizontal(selectedItem, down);
                return item;
            }

            Point midPointOfCurrent = new Point(selectedItem.Bounds.X + selectedItem.Width / 2,
                                                    selectedItem.Bounds.Y + selectedItem.Height / 2);

            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                ToolStripItem otherItem = DisplayedItems[i];
                if (otherItem == selectedItem || !otherItem.CanKeyboardSelect)
                {
                    continue;
                }
                if (!down && otherItem.Bounds.Bottom > selectedItem.Bounds.Top)
                {
                    // if we are going up the other control has to be above
                    continue;
                }
                else if (down && otherItem.Bounds.Top < selectedItem.Bounds.Bottom)
                {
                    // if we are going down the other control has to be below
                    continue;
                }

                //[ otherControl ]
                //       *
                Point otherItemMidLocation = new Point(otherItem.Bounds.X + otherItem.Width / 2, (down) ? otherItem.Bounds.Top : otherItem.Bounds.Bottom);
                int oppositeSide = otherItemMidLocation.X - midPointOfCurrent.X;
                int adjacentSide = otherItemMidLocation.Y - midPointOfCurrent.Y;

                // use pythagrian theorem to calculate the length of the distance
                // between the middle of the current control in question and it's adjacent
                // objects.
                double hypotenuse = Math.Sqrt(adjacentSide * adjacentSide + oppositeSide * oppositeSide);

                if (adjacentSide != 0)
                { // avoid divide by zero - we dont do layered controls
                  //    _[o]
                  //    |/
                  //   [s]
                  //   get the angle between s and o by taking the arctan.
                  //   PERF consider using approximation instead
                    double tan = Math.Abs(Math.Atan(oppositeSide / adjacentSide));

                    // we want the thing with the smallest angle and smallest distance between midpoints
                    minTan = Math.Min(minTan, tan);
                    minHypotenuse = Math.Min(minHypotenuse, hypotenuse);

                    if (minTan == tan && !double.IsNaN(minTan))
                    {
                        tanWinner = otherItem;
                        hypotenuseOfTanWinner = hypotenuse;
                    }

                    if (minHypotenuse == hypotenuse)
                    {
                        hypotenuseWinner = otherItem;
                        tanOfHypotenuseWinner = tan;
                    }
                }
            }

            if ((tanWinner is null) || (hypotenuseWinner is null))
            {
                return (GetNextItemHorizontal(null, down));
            }

            // often times the guy with the best angle will be the guy with the closest hypotenuse.
            // however in layouts where things are more randomly spaced, this is not necessarily the case.
            if (tanOfHypotenuseWinner == minTan)
            {
                // if the angles match up, such as in the case of items of the same width in vertical flow
                // then pick the closest one.
                return hypotenuseWinner;
            }

            if ((!down && tanWinner.Bounds.Bottom <= hypotenuseWinner.Bounds.Top)
              || (down && tanWinner.Bounds.Top > hypotenuseWinner.Bounds.Bottom))
            {
                // we prefer the case where the angle is smaller than
                // the case where the hypotenuse is smaller.  The only
                // scenarios where that is not the case is when the hypoteneuse
                // winner is clearly closer than the angle winner.

                //   [a.winner]                       |       [s]
                //                                    |         [h.winner]
                //       [h.winner]                   |
                //     [s]                            |    [a.winner]
                return hypotenuseWinner;
            }

            return tanWinner;
        }

        internal override Size GetPreferredSizeCore(Size proposedSize)
        {
            // We act like a container control

            // Translating 0,0 from ClientSize to actual Size tells us how much space
            // is required for the borders.
            if (proposedSize.Width == 1)
            {
                proposedSize.Width = int.MaxValue;
            }
            if (proposedSize.Height == 1)
            {
                proposedSize.Height = int.MaxValue;
            }

            Padding padding = Padding;
            Size prefSize = LayoutEngine.GetPreferredSize(this, proposedSize - padding.Size);
            Padding newPadding = Padding;

            // as a side effect of some of the layouts, we can change the padding.
            // if this happens, we need to clear the cache.
            if (padding != newPadding)
            {
                CommonProperties.xClearPreferredSizeCache(this);
            }
            return prefSize + newPadding.Size;
        }

        #region GetPreferredSizeHelpers

        //
        // These are here so they can be shared between splitstack layout and StatusStrip
        //
        internal static Size GetPreferredSizeHorizontal(IArrangedElement container, Size proposedConstraints)
        {
            Size maxSize = Size.Empty;
            ToolStrip toolStrip = container as ToolStrip;

            // ensure preferred size respects default size as a minimum.
            Size defaultSize = toolStrip.DefaultSize - toolStrip.Padding.Size;
            maxSize.Height = Math.Max(0, defaultSize.Height);

            bool requiresOverflow = false;
            bool foundItemParticipatingInLayout = false;

            for (int j = 0; j < toolStrip.Items.Count; j++)
            {
                ToolStripItem item = toolStrip.Items[j];

                if (((IArrangedElement)item).ParticipatesInLayout)
                {
                    foundItemParticipatingInLayout = true;
                    if (item.Overflow != ToolStripItemOverflow.Always)
                    {
                        Padding itemMargin = item.Margin;
                        Size prefItemSize = GetPreferredItemSize(item);
                        maxSize.Width += itemMargin.Horizontal + prefItemSize.Width;
                        maxSize.Height = Math.Max(maxSize.Height, itemMargin.Vertical + prefItemSize.Height);
                    }
                    else
                    {
                        requiresOverflow = true;
                    }
                }
            }

            if (toolStrip.Items.Count == 0 || (!foundItemParticipatingInLayout))
            {
                // if there are no items there, create something anyways.
                maxSize = defaultSize;
            }

            if (requiresOverflow)
            {
                // add in the width of the overflow button
                ToolStripOverflowButton overflowItem = toolStrip.OverflowButton;
                Padding overflowItemMargin = overflowItem.Margin;

                maxSize.Width += overflowItemMargin.Horizontal + overflowItem.Bounds.Width;
            }
            else
            {
                maxSize.Width += 2;  //add Padding of 2 Pixels to the right if not Overflow.
            }

            if (toolStrip.GripStyle == ToolStripGripStyle.Visible)
            {
                // add in the grip width
                Padding gripMargin = toolStrip.GripMargin;
                maxSize.Width += gripMargin.Horizontal + toolStrip.Grip.GripThickness;
            }

            maxSize = LayoutUtils.IntersectSizes(maxSize, proposedConstraints);
            return maxSize;
        }

        internal static Size GetPreferredSizeVertical(IArrangedElement container, Size proposedConstraints)
        {
            Size maxSize = Size.Empty;
            bool requiresOverflow = false;
            ToolStrip toolStrip = container as ToolStrip;

            bool foundItemParticipatingInLayout = false;

            for (int j = 0; j < toolStrip.Items.Count; j++)
            {
                ToolStripItem item = toolStrip.Items[j];

                if (((IArrangedElement)item).ParticipatesInLayout)
                {
                    foundItemParticipatingInLayout = true;
                    if (item.Overflow != ToolStripItemOverflow.Always)
                    {
                        Size preferredSize = GetPreferredItemSize(item);
                        Padding itemMargin = item.Margin;
                        maxSize.Height += itemMargin.Vertical + preferredSize.Height;
                        maxSize.Width = Math.Max(maxSize.Width, itemMargin.Horizontal + preferredSize.Width);
                    }
                    else
                    {
                        requiresOverflow = true;
                    }
                }
            }

            if (toolStrip.Items.Count == 0 || !foundItemParticipatingInLayout)
            {
                // if there are no items there, create something anyways.
                maxSize = LayoutUtils.FlipSize(toolStrip.DefaultSize);
            }

            if (requiresOverflow)
            {
                // add in the width of the overflow button
                ToolStripOverflowButton overflowItem = toolStrip.OverflowButton;
                Padding overflowItemMargin = overflowItem.Margin;
                maxSize.Height += overflowItemMargin.Vertical + overflowItem.Bounds.Height;
            }
            else
            {
                maxSize.Height += 2;  //add Padding to the bottom if not Overflow.
            }

            if (toolStrip.GripStyle == ToolStripGripStyle.Visible)
            {
                // add in the grip width
                Padding gripMargin = toolStrip.GripMargin;
                maxSize.Height += gripMargin.Vertical + toolStrip.Grip.GripThickness;
            }

            // note here the difference in vertical - we want the strings to fit perfectly so we're not going to constrain by the specified size.
            if (toolStrip.Size != maxSize)
            {
                CommonProperties.xClearPreferredSizeCache(toolStrip);
            }
            return maxSize;
        }

        private static Size GetPreferredItemSize(ToolStripItem item)
        {
            return item.AutoSize ? item.GetPreferredSize(Size.Empty) : item.Size;
        }
        #endregion

        internal ToolStripItem GetSelectedItem()
        {
            ToolStripItem selectedItem = null;

            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                if (DisplayedItems[i].Selected)
                {
                    selectedItem = DisplayedItems[i];
                }
            }

            return selectedItem;
        }

        /// <summary>
        ///  Retrieves the current value of the specified bit in the control's state.
        /// </summary>
        internal bool GetToolStripState(int flag)
        {
            return (_toolStripState & flag) != 0;
        }

        internal virtual ToolStrip GetToplevelOwnerToolStrip()
        {
            return this;
        }

        ///  In the case of a
        ///  toolstrip -> toolstrip
        ///  contextmenustrip -> the control that is showing it
        ///  toolstripdropdown -> top most toolstrip
        internal virtual Control GetOwnerControl()
        {
            return this;
        }

        private void HandleMouseLeave()
        {
            // If we had a particular item that was "entered"
            // notify it that we have left.
            if (_lastMouseActiveItem != null)
            {
                if (!DesignMode)
                {
                    MouseHoverTimer.Cancel(_lastMouseActiveItem);
                }
                try
                {
                    Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "firing mouse leave on " + _lastMouseActiveItem.ToString());
                    _lastMouseActiveItem.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseLeave);
                }
                finally
                {
                    Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "setting last active item to null");
                    _lastMouseActiveItem = null;
                }
            }
            ToolStripMenuItem.MenuTimer.HandleToolStripMouseLeave(this);
        }

        internal void HandleItemClick(ToolStripItem dismissingItem)
        {
            ToolStripItemClickedEventArgs e = new ToolStripItemClickedEventArgs(dismissingItem);
            OnItemClicked(e);
            // Ensure both the overflow and the main toolstrip fire ItemClick event
            // otherwise the overflow wont dismiss.
            if (!IsDropDown && dismissingItem.IsOnOverflow)
            {
                OverflowButton.DropDown.HandleItemClick(dismissingItem);
            }
        }

        internal virtual void HandleItemClicked(ToolStripItem dismissingItem)
        {
            // post processing after the click has happened.
            if (dismissingItem is ToolStripDropDownItem item && !item.HasDropDownItems)
            {
                KeyboardActive = false;
            }
        }

        private void HookStaticEvents(bool hook)
        {
            if (hook)
            {
                if (!_alreadyHooked)
                {
                    try
                    {
                        ToolStripManager.RendererChanged += new EventHandler(OnDefaultRendererChanged);
                        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                    }
                    finally
                    {
                        _alreadyHooked = true;
                    }
                }
            }
            else if (_alreadyHooked)
            {
                try
                {
                    ToolStripManager.RendererChanged -= new EventHandler(OnDefaultRendererChanged);
                    SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                }
                finally
                {
                    _alreadyHooked = false;
                }
            }
        }

        //initialize ToolStrip
        private void InitializeRenderer(ToolStripRenderer renderer)
        {
            // wrap this in a LayoutTransaction so that if they change sizes
            // in this method we've suspended layout.
            using (LayoutTransaction.CreateTransactionIf(AutoSize, this, this, PropertyNames.Renderer))
            {
                renderer.Initialize(this);
                for (int i = 0; i < Items.Count; i++)
                {
                    renderer.InitializeItem(Items[i]);
                }
            }
            Invalidate(Controls.Count > 0);
        }

        // sometimes you only want to force a layout if the ToolStrip is visible.
        private void InvalidateLayout()
        {
            if (IsHandleCreated)
            {
                LayoutTransaction.DoLayout(this, this, null);
            }
        }
        internal void InvalidateTextItems()
        {
            using (new LayoutTransaction(this, this, "ShowKeyboardFocusCues", /*PerformLayout=*/Visible))
            {
                for (int j = 0; j < DisplayedItems.Count; j++)
                {
                    if (((DisplayedItems[j].DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text))
                    {
                        DisplayedItems[j].InvalidateItemLayout("ShowKeyboardFocusCues");
                    }
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            ToolStripItem item = GetSelectedItem();
            if ((item != null) && item.IsInputKey(keyData))
            {
                return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override bool IsInputChar(char charCode)
        {
            ToolStripItem item = GetSelectedItem();
            if ((item != null) && item.IsInputChar(charCode))
            {
                return true;
            }
            return base.IsInputChar(charCode);
        }

        private static bool IsPseudoMnemonic(char charCode, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (!WindowsFormsUtils.ContainsMnemonic(text))
                {
                    char charToCompare = char.ToUpper(charCode, CultureInfo.CurrentCulture);
                    char firstLetter = char.ToUpper(text[0], CultureInfo.CurrentCulture);
                    if (firstLetter == charToCompare || (char.ToLower(charCode, CultureInfo.CurrentCulture) == char.ToLower(text[0], CultureInfo.CurrentCulture)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary> Force an item to be painted immediately, rather than waiting for WM_PAINT to occur. </summary>
        internal void InvokePaintItem(ToolStripItem item)
        {
            // Force a WM_PAINT to happen NOW.
            Invalidate(item.Bounds);
            Update();
        }

        /// <summary>
        ///  Gets or sets the <see cref='Forms.ImageList'/> that contains the <see cref='Image'/> displayed on a label control
        /// </summary>
        private void ImageListRecreateHandle(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        ///  This override fires the LocationChanging event if
        ///  1) We are not currently Rafting .. since this cause this infinite times...
        ///  2) If we havent been called once .. Since the "LocationChanging" is listened to by the RaftingCell and calls "JOIN" which may call us back.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Point location = Location;

            if (!IsCurrentlyDragging && !IsLocationChanging && IsInToolStripPanel)
            {
                ToolStripLocationCancelEventArgs cae = new ToolStripLocationCancelEventArgs(new Point(x, y), false);
                try
                {
                    if (location.X != x || location.Y != y)
                    {
                        SetToolStripState(STATE_LOCATIONCHANGING, true);
                        OnLocationChanging(cae);
                    }

                    if (!cae.Cancel)
                    {
                        base.SetBoundsCore(x, y, width, height, specified);
                    }
                }
                finally
                {
                    SetToolStripState(STATE_LOCATIONCHANGING, false);
                }
            }
            else
            {
                if (IsCurrentlyDragging)
                {
                    Region transparentRegion = Renderer.GetTransparentRegion(this);
                    if (transparentRegion != null && (location.X != x || location.Y != y))
                    {
                        try
                        {
                            Invalidate(transparentRegion);
                            Update();
                        }
                        finally
                        {
                            transparentRegion.Dispose();
                        }
                    }
                }
                SetToolStripState(STATE_LOCATIONCHANGING, false);
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        internal void PaintParentRegion(Graphics g, Region region)
        {
        }

        internal bool ProcessCmdKeyInternal(ref Message m, Keys keyData)
        {
            return ProcessCmdKey(ref m, keyData);
        }

        // This function will print to the PrinterDC. ToolStrip have there own buffered painting and doesnt play very well
        // with the DC translations done by base Control class. Hence we do our own Painting and the BitBLT the DC into the printerDc.
        private protected override void PrintToMetaFileRecursive(Gdi32.HDC hDC, IntPtr lParam, Rectangle bounds)
        {
            using Bitmap image = new Bitmap(bounds.Width, bounds.Height);
            using Graphics g = Graphics.FromImage(image);
            using var imageHdc = new DeviceContextHdcScope(g, applyGraphicsState: false);

            // Send the actual wm_print message
            User32.SendMessageW(
                this,
                User32.WM.PRINT,
                (IntPtr)imageHdc.HDC,
                (IntPtr)(User32.PRF.CHILDREN | User32.PRF.CLIENT | User32.PRF.ERASEBKGND | User32.PRF.NONCLIENT));

            // Now BLT the result to the destination bitmap.
            Gdi32.BitBlt(
                hDC,
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height,
                imageHdc,
                0,
                0,
                Gdi32.ROP.SRCCOPY);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (ToolStripManager.IsMenuKey(keyData))
            {
                if (!IsDropDown && ToolStripManager.ModalMenuFilter.InMenuMode)
                {
                    ClearAllSelections();
                    ToolStripManager.ModalMenuFilter.MenuKeyToggle = true;
                    Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.ProcessCmdKey] Detected a second ALT keypress while in Menu Mode.");
                    ToolStripManager.ModalMenuFilter.ExitMenuMode();
                }
            }

            // Give the ToolStripItem very first chance at
            // processing keys (except for ALT handling)
            ToolStripItem selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                if (selectedItem.ProcessCmdKey(ref m, keyData))
                {
                    return true;
                }
            }

            foreach (ToolStripItem item in Items)
            {
                if (item == selectedItem)
                {
                    continue;
                }
                if (item.ProcessCmdKey(ref m, keyData))
                {
                    return true;
                }
            }

            if (!IsDropDown)
            {
                bool isControlTab =
                    (keyData & Keys.Control) == Keys.Control && (keyData & Keys.KeyCode) == Keys.Tab;

                if (isControlTab && !TabStop && HasKeyboardInput)
                {
                    bool handled = false;
                    if ((keyData & Keys.Shift) == Keys.None)
                    {
                        handled = ToolStripManager.SelectNextToolStrip(this, /*forward*/true);
                    }
                    else
                    {
                        handled = ToolStripManager.SelectNextToolStrip(this, /*forward*/false);
                    }
                    if (handled)
                    {
                        return true;
                    }
                }
            }
            return base.ProcessCmdKey(ref m, keyData);
        }

        /// <summary>
        ///  Processes a dialog key. Overrides Control.processDialogKey(). This
        ///  method implements handling of the TAB, LEFT, RIGHT, UP, and DOWN
        ///  keys in dialogs.
        ///  The method performs no processing on keys that include the ALT or
        ///  CONTROL modifiers. For the TAB key, the method selects the next control
        ///  on the form. For the arrow keys,
        ///  !!!
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool retVal = false;
            LastKeyData = keyData;

            // Give the ToolStripItem first dibs
            ToolStripItem item = GetSelectedItem();
            if (item != null)
            {
                if (item.ProcessDialogKey(keyData))
                {
                    return true;
                }
            }

            // if the ToolStrip receives an escape, then we
            // should send the focus back to the last item that
            // had focus.
            bool hasModifiers = ((keyData & (Keys.Alt | Keys.Control)) != Keys.None);

            Keys keyCode = (Keys)keyData & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.Back:
                    // if it's focused itself, process.  if it's not focused, make sure a child control
                    // doesnt have focus before processing
                    if (!ContainsFocus)
                    {
                        // shift backspace/backspace work as backspace, which is the same as shift+tab
                        retVal = ProcessTabKey(false);
                    }
                    break;
                case Keys.Tab:
                    // ctrl+tab does nothing
                    if (!hasModifiers)
                    {
                        retVal = ProcessTabKey((keyData & Keys.Shift) == Keys.None);
                    }
                    break;
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    retVal = ProcessArrowKey(keyCode);
                    break;
                case Keys.Home:
                    SelectNextToolStripItem(null, /*forward =*/ true);
                    retVal = true;
                    break;
                case Keys.End:
                    SelectNextToolStripItem(null, /*forward =*/ false);
                    retVal = true;
                    break;
                case Keys.Escape: // escape and menu key should restore focus
                    // ctrl+esc does nothing
                    if (!hasModifiers && !TabStop)
                    {
                        RestoreFocusInternal();
                        retVal = true;
                    }
                    break;
            }

            if (retVal)
            {
                return retVal;
            }
            Debug.WriteLineIf(s_selectionDebug.TraceVerbose, "[SelectDBG ProcessDialogKey] calling base");
            return base.ProcessDialogKey(keyData);
        }

        internal virtual void ProcessDuplicateMnemonic(ToolStripItem item, char charCode)
        {
            if (!CanProcessMnemonic())
            {  // Checking again for security...
                return;
            }
            //
            if (item != null)
            {
                //
                SetFocusUnsafe();
                item.Select();
            }
        }

        /// <summary>
        ///  Rules for parsing mnemonics
        ///  PASS 1: Real mnemonics
        ///  Check items for the character after the &amp;. If it matches, perform the click event or open the dropdown (in the case that it has dropdown items)
        ///  PASS 2: Fake mnemonics
        ///  Begin with the current selection and parse through the first character in the items in the menu.
        ///  If there is only one item that matches
        ///  perform the click event or open the dropdown (in the case that it has dropdown items)
        ///  Else
        ///  change the selection from the current selected item to the first item that matched.
        /// </summary>
        protected internal override bool ProcessMnemonic(char charCode)
        {
            // menus and toolbars only take focus on ALT
            if (!CanProcessMnemonic())
            {
                return false;
            }
            if (Focused || ContainsFocus)
            {
                return ProcessMnemonicInternal(charCode);
            }
            bool inMenuMode = ToolStripManager.ModalMenuFilter.InMenuMode;
            if (!inMenuMode && Control.ModifierKeys == Keys.Alt)
            {
                // This is the case where someone hasnt released the ALT key yet, but has pushed another letter.
                // In some cases we can activate the menu that is not the MainMenuStrip...
                return ProcessMnemonicInternal(charCode);
            }
            else if (inMenuMode && ToolStripManager.ModalMenuFilter.GetActiveToolStrip() == this)
            {
                return ProcessMnemonicInternal(charCode);
            }

            // do not call base, as we dont want to walk through the controls collection and reprocess everything
            // we should have processed in the displayed items collection.
            return false;
        }
        private bool ProcessMnemonicInternal(char charCode)
        {
            if (!CanProcessMnemonic())
            {  // Checking again for security...
                return false;
            }
            // at this point we assume we can process mnemonics as process mnemonic has filtered for use.
            ToolStripItem startingItem = GetSelectedItem();
            int startIndex = 0;
            if (startingItem != null)
            {
                startIndex = DisplayedItems.IndexOf(startingItem);
            }
            startIndex = Math.Max(0, startIndex);

            ToolStripItem firstMatch = null;
            bool foundMenuItem = false;
            int index = startIndex;

            // PASS1, iterate through the real mnemonics
            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                ToolStripItem currentItem = DisplayedItems[index];

                index = (index + 1) % DisplayedItems.Count;
                if (string.IsNullOrEmpty(currentItem.Text) || !currentItem.Enabled)
                {
                    continue;
                }
                // Only items which display text should be processed
                if ((currentItem.DisplayStyle & ToolStripItemDisplayStyle.Text) != ToolStripItemDisplayStyle.Text)
                {
                    continue;
                }
                // keep track whether we've found a menu item - we'll have to do a
                // second pass for fake mnemonics in that case.
                foundMenuItem = (foundMenuItem || (currentItem is ToolStripMenuItem));

                if (Control.IsMnemonic(charCode, currentItem.Text))
                {
                    if (firstMatch is null)
                    {
                        firstMatch = currentItem;
                    }
                    else
                    {
                        // we've found a second match - we should only change selection.
                        if (firstMatch == startingItem)
                        {
                            // change the selection to be the second match as the first is already selected
                            ProcessDuplicateMnemonic(currentItem, charCode);
                        }
                        else
                        {
                            ProcessDuplicateMnemonic(firstMatch, charCode);
                        }
                        // we've found two mnemonics, just return.
                        return true;
                    }
                }
            }
            // We've found a singular match.
            if (firstMatch != null)
            {
                return firstMatch.ProcessMnemonic(charCode);
            }

            if (!foundMenuItem)
            {
                return false;
            }

            index = startIndex;

            // MenuStrip parity: key presses should change selection if mnemonic not present
            // if we havent found a mnemonic, cycle through the menu items and
            // checbbbMk if we match.

            // PASS2, iterate through the pseudo mnemonics
            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                ToolStripItem currentItem = DisplayedItems[index];
                index = (index + 1) % DisplayedItems.Count;

                // Menu items only
                if (!(currentItem is ToolStripMenuItem) || string.IsNullOrEmpty(currentItem.Text) || !currentItem.Enabled)
                {
                    continue;
                }
                // Only items which display text should be processed
                if ((currentItem.DisplayStyle & ToolStripItemDisplayStyle.Text) != ToolStripItemDisplayStyle.Text)
                {
                    continue;
                }

                if (ToolStrip.IsPseudoMnemonic(charCode, currentItem.Text))
                {
                    if (firstMatch is null)
                    {
                        firstMatch = currentItem;
                    }
                    else
                    {
                        // we've found a second match - we should only change selection.
                        if (firstMatch == startingItem)
                        {
                            // change the selection to be the second match as the first is already selected
                            ProcessDuplicateMnemonic(currentItem, charCode);
                        }
                        else
                        {
                            ProcessDuplicateMnemonic(firstMatch, charCode);
                        }
                        // we've found two mnemonics, just return.
                        return true;
                    }
                }
            }

            if (firstMatch != null)
            {
                return firstMatch.ProcessMnemonic(charCode);
            }

            // do not call base, as we dont want to walk through the controls collection and reprocess everything
            // we should have processed in the displayed items collection.
            return false;
        }

        private bool ProcessTabKey(bool forward)
        {
            if (TabStop)
            {
                // ToolBar in tab-order parity
                //  this means we want the toolstrip in the normal tab order - which means it shouldnt wrap.
                //  First tab gets you into the toolstrip, second tab moves you on your way outside the container.
                //  arrow keys would continue to wrap.
                return false;
            }
            else
            {
                // TabStop = false
                // this means we dont want the toolstrip in the normal tab order (default).
                // We got focus to the toolstrip by putting focus into a control contained on the toolstrip or
                // via a mnemonic e.g. Bold.  In this case we want to wrap.
                // arrow keys would continue to wrap
                if (RightToLeft == RightToLeft.Yes)
                {
                    forward = !forward;
                }
                SelectNextToolStripItem(GetSelectedItem(), forward);
                return true;
            }
        }

        /// <summary>
        ///  This is more useful than overriding ProcessDialogKey because usually the difference
        ///  between ToolStrip/ToolStripDropDown is arrow key handling. ProcessDialogKey first gives
        ///  the selected ToolStripItem the chance to process the message... so really a proper
        ///  inheritor would call down to the base first. Unfortunately doing this would cause the
        ///  arrow keys would be eaten in the base class.
        ///  Instead we're providing a separate place to override all arrow key handling.
        /// </summary>
        internal virtual bool ProcessArrowKey(Keys keyCode)
        {
            bool retVal = false;
            Debug.WriteLineIf(s_menuAutoExpandDebug.TraceVerbose, "[ToolStrip.ProcessArrowKey] MenuTimer.Cancel called");
            ToolStripMenuItem.MenuTimer.Cancel();

            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    retVal = ProcessLeftRightArrowKey(keyCode == Keys.Right);
                    break;
                case Keys.Up:
                case Keys.Down:
                    if (IsDropDown || Orientation != Orientation.Horizontal)
                    {
                        ToolStripItem currentSel = GetSelectedItem();
                        if (keyCode == Keys.Down)
                        {
                            ToolStripItem nextItem = GetNextItem(currentSel, ArrowDirection.Down);
                            if (nextItem != null)
                            {
                                ChangeSelection(nextItem);
                                retVal = true;
                            }
                        }
                        else
                        {
                            ToolStripItem nextItem = GetNextItem(currentSel, ArrowDirection.Up);
                            if (nextItem != null)
                            {
                                ChangeSelection(nextItem);
                                retVal = true;
                            }
                        }
                    }
                    break;
            }
            return retVal;
        }

        /// <summary>
        ///  Process an arrowKey press by selecting the next control in the group
        ///  that the activeControl belongs to.
        /// </summary>
        private bool ProcessLeftRightArrowKey(bool right)
        {
            ToolStripItem selectedItem = GetSelectedItem();
            ToolStripItem nextItem = SelectNextToolStripItem(GetSelectedItem(), right);
            return true;
        }

        internal void NotifySelectionChange(ToolStripItem item)
        {
            if (item is null)
            {
                Debug.WriteLineIf(s_selectionDebug.TraceVerbose, "[SelectDBG NotifySelectionChange] none should be selected");
                ClearAllSelections();
            }
            else if (item.Selected)
            {
                Debug.WriteLineIf(s_selectionDebug.TraceVerbose, "[SelectDBG NotifySelectionChange] Notify selection change: " + item.ToString() + ": " + item.Selected.ToString());
                ClearAllSelectionsExcept(item);
            }
        }

        private void OnDefaultRendererChanged(object sender, EventArgs e)
        {
            // callback from static event
            if (GetToolStripState(STATE_USEDEFAULTRENDERER))
            {
                OnRendererChanged(e);
            }
        }

        protected virtual void OnBeginDrag(EventArgs e)
        {
            SetToolStripState(STATE_DRAGGING, true);
            ClearAllSelections();
            UpdateToolTip(null); // supress the tooltip.
            ((EventHandler)Events[s_eventBeginDrag])?.Invoke(this, e);
        }

        protected virtual void OnEndDrag(EventArgs e)
        {
            SetToolStripState(STATE_DRAGGING, false);
            ((EventHandler)Events[s_eventEndDrag])?.Invoke(this, e);
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
        }

        protected virtual void OnRendererChanged(EventArgs e)
        {
            InitializeRenderer(Renderer);

            ((EventHandler)Events[s_eventRendererChanged])?.Invoke(this, e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            // notify items that the parent has changed
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null && Items[i].ParentInternal == this)
                {
                    Items[i].OnParentEnabledChanged(e);
                }
            }
        }

        internal void OnDefaultFontChanged()
        {
            _defaultFont = null;
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                ToolStripManager.CurrentDpi = DeviceDpi;
                _defaultFont = ToolStripManager.DefaultFont;
            }
            if (!IsFontSet())
            {
                OnFontChanged(EventArgs.Empty);
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].OnOwnerFontChanged(e);
            }
        }

#if DEBUG
#pragma warning disable RS0016 // Add public types and members to the declared API
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            // Debug code which is helpful for FlickerFest debugging.
            if (s_flickerDebug.TraceVerbose)
            {
                string name = this.Name;
                if (string.IsNullOrEmpty(name))
                {
                    if (IsDropDown)
                    {
                        ToolStripItem item = ((ToolStripDropDown)this).OwnerItem;
                        if (item != null && item.Name != null)
                        {
                            name = item.Name = ".DropDown";
                        }
                    }
                    if (string.IsNullOrEmpty(name))
                    {
                        name = GetType().Name;
                    }
                }

                // for debugging VS we want to filter out the propgrid toolstrip
                Debug.WriteLineIf(!(ParentInternal is PropertyGrid), "Invalidate called on: " + name + new StackTrace().ToString());
            }
        }
#pragma warning restore RS0016 // Add public types and members to the declared API
#endif

        protected override void OnHandleCreated(EventArgs e)
        {
            if ((AllowDrop || AllowItemReorder) && (DropTargetManager != null))
            {
                DropTargetManager.EnsureRegistered(this);
            }

            // calling control's (in base) version AFTER we register our DropTarget, so it will
            // listen to us instead of control's implementation
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (DropTargetManager != null)
            {
                // Make sure we unregister ourselves as a drop target
                DropTargetManager.EnsureUnRegistered(this);
            }
            base.OnHandleDestroyed(e);
        }

        protected internal virtual void OnItemAdded(ToolStripItemEventArgs e)
        {
            DoLayoutIfHandleCreated(e);

            if (!HasVisibleItems && e.Item != null && ((IArrangedElement)e.Item).ParticipatesInLayout)
            {
                // in certain cases, we may not have laid out yet (e.g. a dropdown may not layout until
                // it becomes visible.)   We will recalculate this in SetDisplayedItems, but for the moment
                // if we find an item that ParticipatesInLayout, mark us as having visible items.
                HasVisibleItems = true;
            }

            ((ToolStripItemEventHandler)Events[s_eventItemAdded])?.Invoke(this, e);
        }

        /// <summary>
        ///  Called when an item has been clicked on the ToolStrip.
        /// </summary>
        protected virtual void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            ((ToolStripItemClickedEventHandler)Events[s_eventItemClicked])?.Invoke(this, e);
        }

        protected internal virtual void OnItemRemoved(ToolStripItemEventArgs e)
        {
            // clear cached item states.
            OnItemVisibleChanged(e, /*performlayout*/true);

            ((ToolStripItemEventHandler)Events[s_eventItemRemoved])?.Invoke(this, e);
        }

        internal void OnItemVisibleChanged(ToolStripItemEventArgs e, bool performLayout)
        {
            // clear cached item states.
            if (e.Item == _lastMouseActiveItem)
            {
                _lastMouseActiveItem = null;
            }
            if (e.Item == LastMouseDownedItem)
            {
                _lastMouseDownedItem = null;
            }
            if (e.Item == _currentlyActiveTooltipItem)
            {
                UpdateToolTip(null);
            }
            if (performLayout)
            {
                DoLayoutIfHandleCreated(e);
            }
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            LayoutRequired = false;

            // we need to do this to prevent autosizing to happen while we're reparenting.
            ToolStripOverflow overflow = GetOverflow();
            if (overflow != null)
            {
                overflow.SuspendLayout();
                _toolStripOverflowButton.Size = _toolStripOverflowButton.GetPreferredSize(DisplayRectangle.Size - Padding.Size);
            }

            for (int j = 0; j < Items.Count; j++)
            {
                Items[j].OnLayout(e);
            }

            base.OnLayout(e);
            SetDisplayedItems();
            OnLayoutCompleted(EventArgs.Empty);
            Invalidate();

            if (overflow != null)
            {
                overflow.ResumeLayout();
            }
        }

        protected virtual void OnLayoutCompleted(EventArgs e)
        {
            ((EventHandler)Events[s_eventLayoutCompleted])?.Invoke(this, e);
        }

        protected virtual void OnLayoutStyleChanged(EventArgs e)
        {
            ((EventHandler)Events[s_eventLayoutStyleChanged])?.Invoke(this, e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            ClearAllSelections();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (!IsDropDown)
            {
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "uninstalling RestoreFocusFilter");

                // PERF,

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(RestoreFocusFilter);
            }
        }
        internal virtual void OnLocationChanging(ToolStripLocationCancelEventArgs e)
        {
            ((ToolStripLocationCancelEventHandler)Events[s_eventLocationChanging])?.Invoke(this, e);
        }

        /// <summary>
        ///  Delegate mouse down to the ToolStrip and its affected items
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs mea)
        {
            // NEVER use this directly from another class.  Always use GetMouseID so that
            // 0 is not returned to another class.
            _mouseDownID++;

            ToolStripItem item = GetItemAt(mea.X, mea.Y);
            if (item != null)
            {
                if (!IsDropDown && (!(item is ToolStripDropDownItem)))
                {
                    // set capture only when we know we're not on a dropdown (already effectively have capture due to modal menufilter)
                    // and the item in question requires the mouse to be in the same item to be clicked.
                    SetToolStripState(STATE_LASTMOUSEDOWNEDITEMCAPTURE, true);
                    Capture = true;
                }
                MenuAutoExpand = true;

                if (mea != null)
                {
                    // Transpose this to "client coordinates" of the ToolStripItem.
                    Point itemRelativePoint = item.TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripCoords, ToolStripPointType.ToolStripItemCoords);
                    mea = new MouseEventArgs(mea.Button, mea.Clicks, itemRelativePoint.X, itemRelativePoint.Y, mea.Delta);
                }
                _lastMouseDownedItem = item;
                item.FireEvent(mea, ToolStripItemEventType.MouseDown);
            }
            else
            {
                base.OnMouseDown(mea);
            }
        }

        /// <summary>
        ///  Delegate mouse moves to the ToolStrip and its affected items
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs mea)
        {
            Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "OnMouseMove called");

            ToolStripItem item = GetItemAt(mea.X, mea.Y);

            if (!Grip.MovingToolStrip)
            {
                // If we had a particular item that was "entered"
                // notify it that we have entered.  It's fair to put
                // this in the MouseMove event, as MouseEnter is fired during
                // control's WM_MOUSEMOVE. Waiting until this event gives us
                // the actual coordinates.

                Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "Item to get mouse move: {0}", (item is null) ? "null" : item.ToString()));
                if (item != _lastMouseActiveItem)
                {
                    Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "This is a new item - last item to get was {0}", (_lastMouseActiveItem is null) ? "null" : _lastMouseActiveItem.ToString()));

                    // notify the item that we've moved on
                    HandleMouseLeave();

                    // track only items that dont get mouse events themselves.
                    _lastMouseActiveItem = (item is ToolStripControlHost) ? null : item;

                    if (_lastMouseActiveItem != null)
                    {
                        Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "Firing MouseEnter on: {0}", (_lastMouseActiveItem is null) ? "null" : _lastMouseActiveItem.ToString()));
                        item.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseEnter);
                    }
                    //

                    if (!DesignMode)
                    {
                        MouseHoverTimer.Start(_lastMouseActiveItem);
                    }
                }
            }
            else
            {
                item = Grip;
            }
            if (item != null)
            {
                Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "Firing MouseMove on: {0}", (item is null) ? "null" : item.ToString()));

                // Fire mouse move on the item
                // Transpose this to "client coordinates" of the ToolStripItem.
                Point itemRelativePoint = item.TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripCoords, ToolStripPointType.ToolStripItemCoords);
                mea = new MouseEventArgs(mea.Button, mea.Clicks, itemRelativePoint.X, itemRelativePoint.Y, mea.Delta);
                item.FireEvent(mea, ToolStripItemEventType.MouseMove);
            }
            else
            {
                Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "Firing MouseMove on: {0}", (this is null) ? "null" : ToString()));

                base.OnMouseMove(mea);
            }
        }

        /// <summary>
        ///  Delegate mouse leave to the ToolStrip and its affected items
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            HandleMouseLeave();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            if (!GetToolStripState(STATE_SUSPENDCAPTURE))
            {
                // while we're showing a feedback rect, dont cancel moving the toolstrip.
                Grip.MovingToolStrip = false;
            }
            ClearLastMouseDownedItem();

            base.OnMouseCaptureChanged(e);
        }

        /// <summary>
        ///  Delegate mouse up to the ToolStrip and its affected items
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mea)
        {
            ToolStripItem item = (Grip.MovingToolStrip) ? Grip : GetItemAt(mea.X, mea.Y);

            if (item != null)
            {
                if (mea != null)
                {
                    // Transpose this to "client coordinates" of the ToolStripItem.
                    Point itemRelativePoint = item.TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripCoords, ToolStripPointType.ToolStripItemCoords);
                    mea = new MouseEventArgs(mea.Button, mea.Clicks, itemRelativePoint.X, itemRelativePoint.Y, mea.Delta);
                }
                item.FireEvent(mea, ToolStripItemEventType.MouseUp);
            }
            else
            {
                base.OnMouseUp(mea);
            }
            ClearLastMouseDownedItem();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics toolstripGraphics = e.GraphicsInternal;
            Size bitmapSize = _largestDisplayedItemSize;
            bool excludedTransparentRegion = false;

            Rectangle viewableArea = DisplayRectangle;
            using Region transparentRegion = Renderer.GetTransparentRegion(this);

            // Paint the items
            //
            // The idea here is to let items pretend they are controls. They should get paint events at 0,0 and have
            // proper clipping regions set up for them.  We cannot use g.TranslateTransform as that does not translate
            // the GDI world, and things like Visual Styles and the TextRenderer only know how to speak GDI.
            //
            // The previous approach was to set up the GDI clipping region and allocate a graphics from that, but that
            // meant we were allocating graphics objects left and right, which turned out to be slow.
            //
            // So now we allocate an offscreen bitmap of size == MaxItemSize, copy the background of the toolstrip into
            // that bitmap, paint the item on top of the bitmap, then finally copy the contents of the bitmap back onto
            // the toolstrip. This gives us our paint event starting at 0,0.  Combine this with double buffering of the
            // toolstrip and the entire toolstrip is updated after returning from this function.

            if (!LayoutUtils.IsZeroWidthOrHeight(bitmapSize))
            {
                // cant create a 0x0 bmp.

                // Supporting RoundedEdges...
                // we've got a concept of a region that we shouldnt paint (the TransparentRegion as specified in the Renderer).
                // in order to support this we're going to intersect that region with the clipping region.
                // this new region will be excluded during the guts of OnPaint, and restored at the end of OnPaint.
                if (transparentRegion != null)
                {
                    // only use the intersection so we can easily add back in the bits we took out at the end.
                    transparentRegion.Intersect(toolstripGraphics.Clip);
                    toolstripGraphics.ExcludeClip(transparentRegion);
                    excludedTransparentRegion = true;
                }

                // Preparing for painting the individual items...
                // using WindowsGraphics here because we want to preserve the clipping information.

                // calling GetHdc by itself does not set up the clipping info.
                using (var toolStripHDC = new DeviceContextHdcScope(toolstripGraphics, ApplyGraphicsProperties.Clipping))
                {
                    // Get the cached item HDC.
                    Gdi32.HDC itemHDC = ItemHdcInfo.GetCachedItemDC(toolStripHDC, bitmapSize);

                    Graphics itemGraphics = itemHDC.CreateGraphics();
                    try
                    {
                        // Iterate through all the items, painting them one by one into the compatible offscreen DC,
                        // and then copy them back onto the main toolstrip.
                        for (int i = 0; i < DisplayedItems.Count; i++)
                        {
                            ToolStripItem item = DisplayedItems[i];
                            if (item != null)
                            {
                                Rectangle clippingRect = e.ClipRectangle;
                                Rectangle bounds = item.Bounds;

                                if (!IsDropDown && item.Owner == this)
                                {
                                    // owned items should not paint outside the client
                                    // area. (this is mainly to prevent obscuring the grip
                                    // and overflowbutton - ToolStripDropDownMenu places items
                                    // outside of the display rectangle - so we need to allow for this
                                    // in dropdoowns).
                                    clippingRect.Intersect(viewableArea);
                                }

                                // get the intersection of these two.
                                clippingRect.Intersect(bounds);

                                if (LayoutUtils.IsZeroWidthOrHeight(clippingRect))
                                {
                                    continue;  // no point newing up a graphics object if there's nothing to paint.
                                }

                                Size itemSize = item.Size;

                                // check if our item buffer is large enough to handle.
                                if (!LayoutUtils.AreWidthAndHeightLarger(bitmapSize, itemSize))
                                {
                                    // the cached HDC isnt big enough for this item.  make it bigger.
                                    _largestDisplayedItemSize = itemSize;
                                    bitmapSize = itemSize;
                                    // dispose the old graphics - create a new, bigger one.
                                    itemGraphics.Dispose();

                                    // calling this should take the existing DC and select in a bigger bitmap.
                                    itemHDC = ItemHdcInfo.GetCachedItemDC(toolStripHDC, bitmapSize);

                                    // allocate a new graphics.
                                    itemGraphics = itemHDC.CreateGraphics();
                                }

                                // since the item graphics object will have 0,0 at the
                                // corner we need to actually shift the origin of the rect over
                                // so it will be 0,0 too.
                                clippingRect.Offset(-bounds.X, -bounds.Y);

                                // PERF - consider - we only actually need to copy the clipping rect.
                                // copy the background from the toolstrip onto the offscreen bitmap
                                Gdi32.BitBlt(
                                    ItemHdcInfo,
                                    0,
                                    0,
                                    item.Size.Width,
                                    item.Size.Height,
                                    toolStripHDC,
                                    item.Bounds.X,
                                    item.Bounds.Y,
                                    Gdi32.ROP.SRCCOPY);

                                // Paint the item into the offscreen bitmap
                                using (PaintEventArgs itemPaintEventArgs = new PaintEventArgs(itemGraphics, clippingRect))
                                {
                                    item.FireEvent(itemPaintEventArgs, ToolStripItemEventType.Paint);
                                }

                                // copy the item back onto the toolstrip
                                Gdi32.BitBlt(
                                    toolStripHDC,
                                    item.Bounds.X,
                                    item.Bounds.Y,
                                    item.Size.Width,
                                    item.Size.Height,
                                    ItemHdcInfo,
                                    0,
                                    0,
                                    Gdi32.ROP.SRCCOPY);

                                GC.KeepAlive(ItemHdcInfo);
                            }
                        }
                    }
                    finally
                    {
                        if (itemGraphics != null)
                        {
                            itemGraphics.Dispose();
                        }
                    }
                }
            }

            // Painting the edge effects...
            // These would include things like (shadow line on the bottom, some overflow effects)
            Renderer.DrawToolStripBorder(new ToolStripRenderEventArgs(toolstripGraphics, this));

            // Restoring the clip region to its original state...
            // the transparent region should be added back in as the insertion mark should paint over it.
            if (excludedTransparentRegion)
            {
                toolstripGraphics.SetClip(transparentRegion, CombineMode.Union);
            }

            // Paint the item re-order insertion mark...
            // This should ignore the transparent region and paint
            // over the entire area.
            PaintInsertionMark(toolstripGraphics);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            // normally controls just need to do handle recreation, but ToolStrip does it based on layout of items.
            using (new LayoutTransaction(this, this, PropertyNames.RightToLeft))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].OnParentRightToLeftChanged(e);
                }
                if (_toolStripOverflowButton != null)
                {
                    _toolStripOverflowButton.OnParentRightToLeftChanged(e);
                }
                if (_toolStripGrip != null)
                {
                    _toolStripGrip.OnParentRightToLeftChanged(e);
                }
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle the erase
        ///  background request from windows. It is not necessary to call
        ///  base.onPaintBackground, however if you do not want the default
        ///  Windows behavior you must set event.handled to true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            Graphics g = e.GraphicsInternal;
            GraphicsState graphicsState = g.Save();
            try
            {
                using (Region transparentRegion = Renderer.GetTransparentRegion(this))
                {
                    if (transparentRegion != null)
                    {
                        EraseCorners(e, transparentRegion);
                        g.ExcludeClip(transparentRegion);
                    }
                }
                Renderer.DrawToolStripBackground(new ToolStripRenderEventArgs(g, this));
            }
            finally
            {
                if (graphicsState != null)
                {
                    g.Restore(graphicsState);
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Disposing && !IsDisposed)
            {
                HookStaticEvents(Visible);
            }
        }

        private void EraseCorners(PaintEventArgs e, Region transparentRegion)
        {
            if (transparentRegion != null)
            {
                PaintTransparentBackground(e, ClientRectangle, transparentRegion);
            }
        }

        internal protected virtual void OnPaintGrip(PaintEventArgs e)
        {
            Renderer.DrawGrip(new ToolStripGripRenderEventArgs(e.Graphics, this));

            ((PaintEventHandler)Events[s_eventPaintGrip])?.Invoke(this, e);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            if (se.Type != ScrollEventType.ThumbTrack && se.NewValue != se.OldValue)
            {
                ScrollInternal(se.OldValue - se.NewValue);
            }
            base.OnScroll(se);
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            switch (e.Category)
            {
                case UserPreferenceCategory.Window:
                    OnDefaultFontChanged();
                    break;
                case UserPreferenceCategory.General:
                    InvalidateTextItems();
                    break;
            }
        }

        protected override void OnTabStopChanged(EventArgs e)
        {
            // SelectNextControl can select non-tabstop things.
            // we need to prevent this by changing the value of "CanSelect"
            SetStyle(ControlStyles.Selectable, TabStop);
            base.OnTabStopChanged(e);
        }

        /// <summary>
        ///  When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        ///  Must call the base class method to get the current DPI values. This method is invoked only when
        ///  Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has
        ///  EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI value</param>
        /// <param name="deviceDpiNew">New DPI value</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                if (deviceDpiOld != deviceDpiNew)
                {
                    ToolStripManager.CurrentDpi = deviceDpiNew;
                    _defaultFont = ToolStripManager.DefaultFont;

                    // We need to take care of this control.
                    ResetScaling(deviceDpiNew);

                    // We need to scale the one Grip per ToolStrip as well (if present).
                    if (_toolStripGrip != null)
                    {
                        _toolStripGrip.ToolStrip_RescaleConstants(deviceDpiOld, deviceDpiNew);
                    }

                    // We need to delegate this "event" to the Controls/Components, which are
                    // not directly affected by this, but need to consume.
                    _rescaleConstsCallbackDelegate?.Invoke(deviceDpiOld, deviceDpiNew);
                }
            }
        }

        /// <summary>
        ///  Resets the scaling (only in PerMonitorV2 scenarios).
        /// </summary>
        /// <param name="newDpi">The new DPI passed by WmDpiChangedBeforeParent.</param>
        internal virtual void ResetScaling(int newDpi)
        {
            s_iconWidth = DpiHelper.LogicalToDeviceUnits(IconDimension, newDpi);
            s_iconHeight = DpiHelper.LogicalToDeviceUnits(IconDimension, newDpi);
            s_insertionBeamWidth = DpiHelper.LogicalToDeviceUnits(InsertionBeamWidth, newDpi);
            _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(s_defaultPadding, newDpi);
            _scaledDefaultGripMargin = DpiHelper.LogicalToDeviceUnits(s_defaultGripMargin, newDpi);
            _imageScalingSize = new Size(s_iconWidth, s_iconHeight);
        }

        /// <summary>
        ///  Paints the I beam when items are being reordered
        /// </summary>
        internal void PaintInsertionMark(Graphics g)
        {
            if (_lastInsertionMarkRect != Rectangle.Empty)
            {
                int widthOfBeam = s_insertionBeamWidth;
                if (Orientation == Orientation.Horizontal)
                {
                    int start = _lastInsertionMarkRect.X;
                    int verticalBeamStart = start + 2;

                    // draw two vertical lines
                    g.DrawLines(SystemPens.ControlText,
                        new Point[] { new Point(verticalBeamStart, _lastInsertionMarkRect.Y), new Point(verticalBeamStart, _lastInsertionMarkRect.Bottom-1), // first vertical line
                                  new Point(verticalBeamStart+1, _lastInsertionMarkRect.Y), new Point(verticalBeamStart+1, _lastInsertionMarkRect.Bottom-1), //second  vertical line
                        });
                    // then two top horizontal
                    g.DrawLines(SystemPens.ControlText,
                        new Point[] { new Point(start, _lastInsertionMarkRect.Bottom-1), new Point(start + widthOfBeam-1, _lastInsertionMarkRect.Bottom-1), //bottom line
                                  new Point(start+1, _lastInsertionMarkRect.Bottom -2), new Point(start + widthOfBeam-2, _lastInsertionMarkRect.Bottom-2),//bottom second line
                        });
                    // then two bottom horizontal
                    g.DrawLines(SystemPens.ControlText,
                         new Point[] {  new Point(start, _lastInsertionMarkRect.Y), new Point(start + widthOfBeam-1, _lastInsertionMarkRect.Y), //top line
                                    new Point(start+1, _lastInsertionMarkRect.Y+1), new Point(start + widthOfBeam-2, _lastInsertionMarkRect.Y+1)//top second line
                         });
                }
                else
                {
                    widthOfBeam = s_insertionBeamWidth;
                    int start = _lastInsertionMarkRect.Y;
                    int horizontalBeamStart = start + 2;

                    // draw two horizontal lines
                    g.DrawLines(SystemPens.ControlText,
                        new Point[] { new Point(_lastInsertionMarkRect.X, horizontalBeamStart), new Point(_lastInsertionMarkRect.Right-1, horizontalBeamStart), // first vertical line
                                      new Point(_lastInsertionMarkRect.X, horizontalBeamStart+1), new Point(_lastInsertionMarkRect.Right-1, horizontalBeamStart+1), //second  vertical line
                                      });
                    // then two left vertical
                    g.DrawLines(SystemPens.ControlText,
                        new Point[] { new Point(_lastInsertionMarkRect.X, start), new Point(_lastInsertionMarkRect.X, start + widthOfBeam-1), //left line
                                      new Point(_lastInsertionMarkRect.X+1, start+1), new Point(_lastInsertionMarkRect.X+1, start + widthOfBeam-2), //second left line
                                       });
                    // then two right vertical
                    g.DrawLines(SystemPens.ControlText,
                         new Point[] { new Point(_lastInsertionMarkRect.Right-1, start), new Point(_lastInsertionMarkRect.Right-1, start + widthOfBeam-1), //right line
                                      new Point(_lastInsertionMarkRect.Right-2, start+1), new Point(_lastInsertionMarkRect.Right-2, start + widthOfBeam-2), //second right line
                                      });
                }
            }
        }

        /// <summary>
        ///  Paints the I beam when items are being reordered
        /// </summary>
        internal void PaintInsertionMark(Rectangle insertionRect)
        {
            if (_lastInsertionMarkRect != insertionRect)
            {
                ClearInsertionMark();
                _lastInsertionMarkRect = insertionRect;
                Invalidate(insertionRect);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Control GetChildAtPoint(Point point)
        {
            return base.GetChildAtPoint(point);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
        {
            return base.GetChildAtPoint(pt, skipValue);
        }

        // GetNextControl for ToolStrip should always return null
        // we do our own tabbing/etc - this allows us to pretend
        // we dont have child controls.
        internal override Control GetFirstChildControlInTabOrder(bool forward)
        {
            return null;
        }

        /// <summary>
        ///  Finds the ToolStripItem contained within a specified client coordinate point
        ///  If item not found - returns null
        /// </summary>
        public ToolStripItem GetItemAt(int x, int y)
        {
            return GetItemAt(new Point(x, y));
        }

        /// <summary>
        ///  Finds the ToolStripItem contained within a specified client coordinate point
        ///  If item not found - returns null
        /// </summary>
        public ToolStripItem GetItemAt(Point point)
        {
            Rectangle comparisonRect = new Rectangle(point, s_onePixel);
            Rectangle bounds;

            // Check the last item we had the mouse over
            if (_lastMouseActiveItem != null)
            {
                bounds = _lastMouseActiveItem.Bounds;

                if (bounds.IntersectsWith(comparisonRect) && _lastMouseActiveItem.ParentInternal == this)
                {
                    return _lastMouseActiveItem;
                }
            }

            // Walk the ToolStripItem collection
            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                if (DisplayedItems[i] is null || DisplayedItems[i].ParentInternal != this)
                {
                    continue;
                }

                bounds = DisplayedItems[i].Bounds;

                // inflate the grip so it is easier to access
                if (_toolStripGrip != null && DisplayedItems[i] == _toolStripGrip)
                {
                    bounds = LayoutUtils.InflateRect(bounds, GripMargin);
                }
                if (bounds.IntersectsWith(comparisonRect))
                {
                    return DisplayedItems[i];
                }
            }

            return null;
        }

        private void RestoreFocusInternal(bool wasInMenuMode)
        {
            // This is called from the RestoreFocusFilter.  If the state of MenuMode has changed
            // since we posted this message, we do not know enough information about whether
            // we should exit menu mode.
            if (wasInMenuMode == ToolStripManager.ModalMenuFilter.InMenuMode)
            {
                RestoreFocusInternal();
            }
        }

        /// <summary> RestoreFocus - returns focus to the control who activated us
        ///  See comment on SnapFocus
        /// </summary>
        internal void RestoreFocusInternal()
        {
            ToolStripManager.ModalMenuFilter.MenuKeyToggle = false;
            ClearAllSelections();
            _lastMouseDownedItem = null;

            Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.RestoreFocus] Someone has called RestoreFocus, exiting MenuMode.");
            ToolStripManager.ModalMenuFilter.ExitMenuMode();

            if (!IsDropDown)
            {
                // reset menu auto expansion.
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.RestoreFocus] Setting menu auto expand to false");
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.RestoreFocus] uninstalling RestoreFocusFilter");

                // PERF,

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(RestoreFocusFilter);

                MenuAutoExpand = false;

                if (!DesignMode && !TabStop && (Focused || ContainsFocus))
                {
                    RestoreFocus();
                }
            }

            // this matches the case where you click on a toolstrip control host
            // then tab off of it, then hit ESC.  ESC would "restore focus" and
            // we should cancel keyboard activation if this method has cancelled focus.
            if (KeyboardActive && !Focused && !ContainsFocus)
            {
                KeyboardActive = false;
            }
        }

        // override if you want to control (when TabStop = false) where the focus returns to
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void RestoreFocus()
        {
            bool focusSuccess = false;

            if ((_hwndThatLostFocus != IntPtr.Zero) && (_hwndThatLostFocus != Handle))
            {
                Control c = Control.FromHandle(_hwndThatLostFocus);

                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip RestoreFocus]: Will Restore Focus to: " + WindowsFormsUtils.GetControlInformation(_hwndThatLostFocus));
                _hwndThatLostFocus = IntPtr.Zero;

                if ((c != null) && c.Visible)
                {
                    focusSuccess = c.Focus();
                }
            }
            _hwndThatLostFocus = IntPtr.Zero;

            if (!focusSuccess)
            {
                // clear out the focus, we have focus, we're not supposed to anymore.
                User32.SetFocus(IntPtr.Zero);
            }
        }

        internal virtual void ResetRenderMode()
        {
            RenderMode = ToolStripRenderMode.ManagerRenderMode;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetMinimumSize()
        {
            CommonProperties.SetMinimumSize(this, new Size(-1, -1));
        }

        private void ResetGripMargin()
        {
            GripMargin = Grip.DefaultMargin;
        }

        internal void ResumeCaputureMode()
        {
            SetToolStripState(STATE_SUSPENDCAPTURE, false);
        }

        internal void SuspendCaputureMode()
        {
            SetToolStripState(STATE_SUSPENDCAPTURE, true);
        }

        internal virtual void ScrollInternal(int delta)
        {
            SuspendLayout();
            foreach (ToolStripItem item in Items)
            {
                Point newLocation = item.Bounds.Location;

                newLocation.Y -= delta;

                SetItemLocation(item, newLocation);
            }

            ResumeLayout(false);
            Invalidate();
        }

        protected internal void SetItemLocation(ToolStripItem item, Point location)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.Owner != this)
            {
                throw new NotSupportedException(SR.ToolStripCanOnlyPositionItsOwnItems);
            }

            item.SetBounds(new Rectangle(location, item.Size));
        }

        /// <summary>
        ///  This is needed so that people doing custom layout engines can change the "Parent" property of the item.
        /// </summary>
        protected static void SetItemParent(ToolStripItem item, ToolStrip parent)
        {
            item.Parent = parent;
        }

        protected override void SetVisibleCore(bool visible)
        {
            if (visible)
            {
                SnapMouseLocation();
            }
            else
            {
                // make sure we reset selection - this is critical for close/reopen dropdowns.
                if (!Disposing && !IsDisposed)
                {
                    ClearAllSelections();
                }

                // when we're not visible, clear off old item HDC.
                CachedItemHdcInfo lastInfo = _cachedItemHdcInfo;
                _cachedItemHdcInfo = null;

                _lastMouseDownedItem = null;

                if (lastInfo != null)
                {
                    lastInfo.Dispose();
                }
            }
            base.SetVisibleCore(visible);
        }

        internal bool ShouldSelectItem()
        {
            // we only want to select the item if the cursor position has
            // actually moved from when the window became visible.

            // We ALWAYS get a WM_MOUSEMOVE when the window is shown,
            // which could accidentally change selection.
            if (_mouseEnterWhenShown == s_invalidMouseEnter)
            {
                Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "[TS: ShouldSelectItem] MouseEnter already reset.");
                return true;
            }

            Point mousePosition = WindowsFormsUtils.LastCursorPoint;
            if (_mouseEnterWhenShown != mousePosition)
            {
                Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "[TS: ShouldSelectItem] Mouse position has changed - call Select().");
                _mouseEnterWhenShown = s_invalidMouseEnter;
                return true;
            }
            Debug.WriteLineIf(ToolStripItem.s_mouseDebugging.TraceVerbose, "[TS: ShouldSelectItem] Mouse hasnt actually moved yet.");

            return false;
        }

        protected override void Select(bool directed, bool forward)
        {
            bool correctParentActiveControl = true;
            if (ParentInternal != null)
            {
                IContainerControl c = ParentInternal.GetContainerControl();

                if (c != null)
                {
                    c.ActiveControl = this;
                    correctParentActiveControl = (c.ActiveControl == this);
                }
            }
            if (directed && correctParentActiveControl)
            {
                SelectNextToolStripItem(null, forward);
            }
        }

        internal ToolStripItem SelectNextToolStripItem(ToolStripItem start, bool forward)
        {
            ToolStripItem nextItem = GetNextItem(start, (forward) ? ArrowDirection.Right : ArrowDirection.Left, /*RTLAware=*/true);
            ChangeSelection(nextItem);
            return nextItem;
        }

        internal void SetFocusUnsafe()
        {
            if (TabStop)
            {
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.SetFocus] Focusing toolstrip.");
                Focus();
            }
            else
            {
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.SetFocus] Entering menu mode.");
                ToolStripManager.ModalMenuFilter.SetActiveToolStrip(this, /*menuKeyPressed=*/false);
            }
        }

        private void SetupGrip()
        {
            Rectangle gripRectangle = Rectangle.Empty;
            Rectangle displayRect = DisplayRectangle;

            if (Orientation == Orientation.Horizontal)
            {
                // the display rectangle already knows about the padding and the grip rectangle width
                // so place it relative to that.
                gripRectangle.X = Math.Max(0, displayRect.X - Grip.GripThickness);
                gripRectangle.Y = Math.Max(0, displayRect.Top - Grip.Margin.Top);
                gripRectangle.Width = Grip.GripThickness;
                gripRectangle.Height = displayRect.Height;
                if (RightToLeft == RightToLeft.Yes)
                {
                    gripRectangle.X = ClientRectangle.Right - gripRectangle.Width - Grip.Margin.Horizontal;
                    gripRectangle.X += Grip.Margin.Left;
                }
                else
                {
                    gripRectangle.X -= Grip.Margin.Right;
                }
            }
            else
            {
                // vertical split stack mode
                gripRectangle.X = displayRect.Left;
                gripRectangle.Y = displayRect.Top - (Grip.GripThickness + Grip.Margin.Bottom);
                gripRectangle.Width = displayRect.Width;
                gripRectangle.Height = Grip.GripThickness;
            }

            if (Grip.Bounds != gripRectangle)
            {
                Grip.SetBounds(gripRectangle);
            }
        }

        /// <summary>
        ///  Sets the size of the auto-scroll margins.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public void SetAutoScrollMargin(int x, int y)
        {
            base.SetAutoScrollMargin(x, y);
        }

        internal void SetLargestItemSize(Size size)
        {
            if (_toolStripOverflowButton != null && _toolStripOverflowButton.Visible)
            {
                size = LayoutUtils.UnionSizes(size, _toolStripOverflowButton.Bounds.Size);
            }
            if (_toolStripGrip != null && _toolStripGrip.Visible)
            {
                size = LayoutUtils.UnionSizes(size, _toolStripGrip.Bounds.Size);
            }
            _largestDisplayedItemSize = size;
        }

        /// <summary>
        ///  Afer we've performed a layout we need to reset the DisplayedItems and the OverflowItems collection.
        ///  OverflowItems are not supported in layouts other than ToolStripSplitStack
        /// </summary>
        protected virtual void SetDisplayedItems()
        {
            DisplayedItems.Clear();
            OverflowItems.Clear();
            HasVisibleItems = false;

            Size biggestItemSize = Size.Empty; // used in determining OnPaint caching.

            if (LayoutEngine is ToolStripSplitStackLayout)
            {
                if (ToolStripGripStyle.Visible == GripStyle)
                {
                    DisplayedItems.Add(Grip);
                    SetupGrip();
                }

                // For splitstack layout we re-arrange the items in the displayed items
                // collection so that we can easily tab through them in natural order
                Rectangle displayRect = DisplayRectangle;
                int lastRightAlignedItem = -1;

                for (int pass = 0; pass < 2; pass++)
                {
                    int j = 0;

                    if (pass == 1 /*add right aligned items*/)
                    {
                        j = lastRightAlignedItem;
                    }

                    // add items to the DisplayedItem collection.
                    // in pass 0, we go forward adding the head (left) aligned items
                    // in pass 1, we go backward starting from the last (right) aligned item we found
                    for (; j >= 0 && j < Items.Count; j = (pass == 0) ? j + 1 : j - 1)
                    {
                        ToolStripItem item = Items[j];
                        ToolStripItemPlacement placement = item.Placement;
                        if (((IArrangedElement)item).ParticipatesInLayout)
                        {
                            if (placement == ToolStripItemPlacement.Main)
                            {
                                bool addItem = false;
                                if (pass == 0)
                                { // Align.Left items
                                    addItem = (item.Alignment == ToolStripItemAlignment.Left);
                                    if (!addItem)
                                    {
                                        // stash away this index so we dont have to iterate through the whole list again.
                                        lastRightAlignedItem = j;
                                    }
                                }
                                else if (pass == 1)
                                {
                                    // Align.Right items
                                    addItem = (item.Alignment == ToolStripItemAlignment.Right);
                                }
                                if (addItem)
                                {
                                    HasVisibleItems = true;
                                    biggestItemSize = LayoutUtils.UnionSizes(biggestItemSize, item.Bounds.Size);
                                    DisplayedItems.Add(item);
                                }
                            }
                            else if (placement == ToolStripItemPlacement.Overflow && !(item is ToolStripSeparator))
                            {
                                OverflowItems.Add(item);
                            }
                        }
                        else
                        {
                            item.SetPlacement(ToolStripItemPlacement.None);
                        }
                    }
                }
                ToolStripOverflow overflow = GetOverflow();
                if (overflow != null)
                {
                    overflow.LayoutRequired = true;
                }
                if (OverflowItems.Count == 0)
                {
                    OverflowButton.Visible = false;
                }
                else if (CanOverflow)
                {
                    DisplayedItems.Add(OverflowButton);
                }
            }
            else
            {
                // NOT a SplitStack layout.  We dont change the order of the displayed items collection
                // for custom keyboard handling override GetNextItem.
                Debug.WriteLineIf(s_layoutDebugSwitch.TraceVerbose, "Setting Displayed Items: Current bounds: " + Bounds.ToString());
                Rectangle clientBounds = ClientRectangle;

                // for all other layout managers, we ignore overflow placement
                bool allContained = true;
                for (int j = 0; j < Items.Count; j++)
                {
                    ToolStripItem item = Items[j];
                    if (((IArrangedElement)item).ParticipatesInLayout)
                    {
                        item.ParentInternal = this;

                        bool boundsCheck = !IsDropDown;
                        bool intersects = item.Bounds.IntersectsWith(clientBounds);

                        bool verticallyContained = clientBounds.Contains(clientBounds.X, item.Bounds.Top) &&
                                                clientBounds.Contains(clientBounds.X, item.Bounds.Bottom);
                        if (!verticallyContained)
                        {
                            allContained = false;
                        }

                        if (!boundsCheck || intersects)
                        {
                            HasVisibleItems = true;
                            biggestItemSize = LayoutUtils.UnionSizes(biggestItemSize, item.Bounds.Size);
                            DisplayedItems.Add(item);
                            item.SetPlacement(ToolStripItemPlacement.Main);
                        }
                    }
                    else
                    {
                        item.SetPlacement(ToolStripItemPlacement.None);
                    }

                    Debug.WriteLineIf(s_layoutDebugSwitch.TraceVerbose, item.ToString() + Items[j].Bounds);
                }

                // For performance we calculate this here, since we're already iterating over the items.
                // the only one who cares about it is ToolStripDropDownMenu (to see if it needs scroll buttons).
                AllItemsVisible = allContained;
            }

            SetLargestItemSize(biggestItemSize);
        }

        /// <summary>
        ///  Sets the current value of the specified bit in the control's state.
        /// </summary>
        internal void SetToolStripState(int flag, bool value)
        {
            _toolStripState = value ? _toolStripState | flag : _toolStripState & ~flag;
        }

        // remembers the current mouse location so we can determine
        // later if we need to shift selection.
        internal void SnapMouseLocation()
        {
            _mouseEnterWhenShown = WindowsFormsUtils.LastCursorPoint;
        }

        /// <summary> SnapFocus
        ///  When get focus to the toolstrip (and we're not participating in the tab order)
        ///  it's probably cause someone hit the ALT key. We need to remember who that was
        ///  so when we're done here we can RestoreFocus back to it.
        ///
        ///  We're called from WM_SETFOCUS, and otherHwnd is the HWND losing focus.
        ///
        ///  Required checks
        ///  - make sure it's not a dropdown
        ///  - make sure it's not a child control of this control.
        ///  - make sure the control is on this window
        /// </summary>
        private void SnapFocus(IntPtr otherHwnd)
        {
#if DEBUG
            if (s_snapFocusDebug.TraceVerbose)
            {
                string stackTrace = new StackTrace().ToString();
                Regex regex = new Regex("FocusInternal");
                Debug.WriteLine(!regex.IsMatch(stackTrace), "who is setting focus to us?");
            }
#endif
            // we need to know who sent us focus so we know who to send it back to later.

            if (!TabStop && !IsDropDown)
            {
                bool snapFocus = false;
                if (Focused && (otherHwnd != Handle))
                {
                    // the case here is a label before a combo box calling FocusInternal in ProcessMnemonic.
                    // we'll filter out children later.
                    snapFocus = true;
                }
                else if (!ContainsFocus && !Focused)
                {
                    snapFocus = true;
                }

                if (snapFocus)
                {
                    // remember the current mouse position so that we can check later if it actually moved
                    // otherwise we'd unexpectedly change selection to whatever the cursor was over at this moment.
                    SnapMouseLocation();

                    // make sure the otherHandle is not a child of thisHandle
                    if ((Handle != otherHwnd) && !User32.IsChild(new HandleRef(this, Handle), otherHwnd).IsTrue())
                    {
                        // make sure the root window of the otherHwnd is the same as
                        // the root window of thisHwnd.
                        IntPtr thisHwndRoot = User32.GetAncestor(this, User32.GA.ROOT);
                        IntPtr otherHwndRoot = User32.GetAncestor(otherHwnd, User32.GA.ROOT);

                        if (thisHwndRoot == otherHwndRoot && (thisHwndRoot != IntPtr.Zero))
                        {
                            Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip SnapFocus]: Caching for return focus:" + WindowsFormsUtils.GetControlInformation(otherHwnd));
                            // we know we're in the same window heirarchy.
                            _hwndThatLostFocus = otherHwnd;
                        }
                    }
                }
            }
        }

        // when we're control tabbing around we need to remember the original
        // thing that lost focus.
        internal void SnapFocusChange(ToolStrip otherToolStrip)
        {
            otherToolStrip._hwndThatLostFocus = _hwndThatLostFocus;
        }

        private bool ShouldSerializeDefaultDropDownDirection()
        {
            return (_toolStripDropDownDirection != ToolStripDropDownDirection.Default);
        }

        internal virtual bool ShouldSerializeLayoutStyle()
        {
            return _layoutStyle != ToolStripLayoutStyle.StackWithOverflow;
        }

        internal override bool ShouldSerializeMinimumSize()
        {
            Size invalidDefaultSize = new Size(-1, -1);
            return (CommonProperties.GetMinimumSize(this, invalidDefaultSize) != invalidDefaultSize);
        }

        private bool ShouldSerializeGripMargin()
        {
            return GripMargin != DefaultGripMargin;
        }

        internal virtual bool ShouldSerializeRenderMode()
        {
            // We should NEVER serialize custom.
            return (RenderMode != ToolStripRenderMode.ManagerRenderMode && RenderMode != ToolStripRenderMode.Custom);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append(", Name: ");
            sb.Append(Name);
            sb.Append(", Items: ").Append(Items.Count);
            return sb.ToString();
        }

        internal void UpdateToolTip(ToolStripItem item)
        {
            if (ShowItemToolTips)
            {
                if (item != _currentlyActiveTooltipItem && ToolTip != null)
                {
                    ToolTip.Hide(this);

                    _currentlyActiveTooltipItem = item;

                    if (_currentlyActiveTooltipItem != null && !GetToolStripState(STATE_DRAGGING))
                    {
                        Cursor currentCursor = Cursor.Current;

                        if (currentCursor != null)
                        {
                            Point cursorLocation = Cursor.Position;
                            cursorLocation.Y += Cursor.Size.Height - currentCursor.HotSpot.Y;

                            cursorLocation = WindowsFormsUtils.ConstrainToScreenBounds(new Rectangle(cursorLocation, s_onePixel)).Location;

                            ToolTip.Show(_currentlyActiveTooltipItem.ToolTipText,
                                        this,
                                        PointToClient(cursorLocation),
                                        ToolTip.AutoPopDelay);
                        }
                    }
                }
            }
        }

        private void UpdateLayoutStyle(DockStyle newDock)
        {
            if (!IsInToolStripPanel && _layoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow && _layoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow)
            {
                using (new LayoutTransaction(this, this, PropertyNames.Orientation))
                {
                    //
                    //  We want the ToolStrip to size appropriately when the dock has switched.
                    //
                    if (newDock == DockStyle.Left || newDock == DockStyle.Right)
                    {
                        UpdateOrientation(Orientation.Vertical);
                    }
                    else
                    {
                        UpdateOrientation(Orientation.Horizontal);
                    }
                }

                OnLayoutStyleChanged(EventArgs.Empty);

                if (ParentInternal != null)
                {
                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Orientation);
                }
            }
        }

        private void UpdateLayoutStyle(Orientation newRaftingRowOrientation)
        {
            if (_layoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow && _layoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow)
            {
                using (new LayoutTransaction(this, this, PropertyNames.Orientation))
                {
                    //  We want the ToolStrip to size appropriately when the rafting container orientation has switched.
                    UpdateOrientation(newRaftingRowOrientation);
                    if (LayoutEngine is ToolStripSplitStackLayout && _layoutStyle == ToolStripLayoutStyle.StackWithOverflow)
                    {
                        OnLayoutStyleChanged(EventArgs.Empty);
                    }
                }
            }
            else
            {
                // update the orientation but dont force a layout.
                UpdateOrientation(newRaftingRowOrientation);
            }
        }

        private void UpdateOrientation(Orientation newOrientation)
        {
            if (newOrientation != Orientation)
            {
                // snap our last dimensions before switching over.
                // use specifed bounds so that if something is docked or anchored we dont take the extra stretching
                // effects into account.
                Size size = CommonProperties.GetSpecifiedBounds(this).Size;
                Orientation = newOrientation;
                // since the Grip affects the DisplayRectangle, we need to re-adjust the size
                SetupGrip();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)User32.WM.SETFOCUS)
            {
                SnapFocus(m.WParam);
            }
            if (m.Msg == (int)User32.WM.MOUSEACTIVATE)
            {
                // we want to prevent taking focus if someone clicks on the toolstrip dropdown
                // itself.  the mouse message will still go through, but focus wont be taken.
                // if someone clicks on a child control (combobox, textbox, etc) focus will
                // be taken - but we'll handle that in WM_NCACTIVATE handler.
                Point pt = PointToClient(WindowsFormsUtils.LastCursorPoint);
                IntPtr hwndClicked = User32.ChildWindowFromPointEx(this, pt, User32.CWP.SKIPINVISIBLE | User32.CWP.SKIPDISABLED | User32.CWP.SKIPTRANSPARENT);

                // if we click on the toolstrip itself, eat the activation.
                // if we click on a child control, allow the toolstrip to activate.
                if (hwndClicked == Handle)
                {
                    _lastMouseDownedItem = null;
                    m.Result = (IntPtr)User32.MA.NOACTIVATE;

                    if (!IsDropDown && !IsInDesignMode)
                    {
                        // If our root HWND is not the active hwnd,
                        // eat the mouse message and bring the form to the front.
                        IntPtr rootHwnd = User32.GetAncestor(this, User32.GA.ROOT);
                        if (rootHwnd != IntPtr.Zero)
                        {
                            // snap the active window and compare to our root window.
                            IntPtr hwndActive = User32.GetActiveWindow();
                            if (hwndActive != rootHwnd)
                            {
                                // Activate the window, and discard the mouse message.
                                // this appears to be the same behavior as office.
                                m.Result = (IntPtr)User32.MA.ACTIVATEANDEAT;
                            }
                        }
                    }
                    return;
                }
                else
                {
                    // we're setting focus to a child control - remember who gave it to us
                    // so we can restore it on ESC.
                    SnapFocus(User32.GetFocus());
                    if (!IsDropDown && !TabStop)
                    {
                        Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "Installing restoreFocusFilter");
                        // PERF,

                        Application.ThreadContext.FromCurrent().AddMessageFilter(RestoreFocusFilter);
                    }
                }
            }

            base.WndProc(ref m);

            if (m.Msg == (int)User32.WM.NCDESTROY)
            {
                // Destroy the owner window, if we created one.  We
                // cannot do this in OnHandleDestroyed, because at
                // that point our handle is not actually destroyed so
                // destroying our parent actually causes a recursive
                // WM_DESTROY.
                if (_dropDownOwnerWindow != null)
                {
                    _dropDownOwnerWindow.DestroyHandle();
                }
            }
        }

        // Overriden to return Items instead of Controls.
        ArrangedElementCollection IArrangedElement.Children
        {
            get { return Items; }
        }

        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);
        }

        bool IArrangedElement.ParticipatesInLayout
        {
            get { return GetState(States.Visible); }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripAccessibleObject(this);
        }

        protected override ControlCollection CreateControlsInstance()
        {
            return new ReadOnlyControlCollection(this, /* isReadOnly = */ !DesignMode);
        }

        internal void OnItemAddedInternal(ToolStripItem item)
        {
            if (ShowItemToolTips)
            {
                KeyboardToolTipStateMachine.Instance.Hook(item, ToolTip);
            }
        }

        internal void OnItemRemovedInternal(ToolStripItem item)
        {
            KeyboardToolTipStateMachine.Instance.Unhook(item, ToolTip);
        }

        internal override bool AllowsChildrenToShowToolTips()
        {
            return base.AllowsChildrenToShowToolTips() && ShowItemToolTips;
        }

        public class ToolStripAccessibleObject : ControlAccessibleObject
        {
            private readonly ToolStrip owner;

            public ToolStripAccessibleObject(ToolStrip owner) : base(owner)
            {
                this.owner = owner;
            }

            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
                => owner.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            /// <summary>
            ///  Return the child object at the given screen coordinates.
            /// </summary>
            public override AccessibleObject HitTest(int x, int y)
            {
                if (!owner.IsHandleCreated)
                {
                    return null;
                }

                Point clientHit = owner.PointToClient(new Point(x, y));
                ToolStripItem item = owner.GetItemAt(clientHit);
                return ((item != null) && (item.AccessibilityObject != null))
                    ? item.AccessibilityObject
                    : base.HitTest(x, y);
            }

            /// <summary>
            ///  When overridden in a derived class, gets the accessible child corresponding to the specified
            ///  index.
            /// </summary>
            //
            public override AccessibleObject GetChild(int index)
            {
                if ((owner is null) || (owner.Items is null))
                {
                    return null;
                }

                if (index == 0 && owner.Grip.Visible)
                {
                    return owner.Grip.AccessibilityObject;
                }
                else if (owner.Grip.Visible && index > 0)
                {
                    index--;
                }

                if (index < owner.Items.Count)
                {
                    ToolStripItem item = null;
                    int myIndex = 0;

                    // First we walk through the head aligned items.
                    for (int i = 0; i < owner.Items.Count; ++i)
                    {
                        if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Left)
                        {
                            if (myIndex == index)
                            {
                                item = owner.Items[i];
                                break;
                            }
                            myIndex++;
                        }
                    }

                    // If we didn't find it, then we walk through the tail aligned items.
                    if (item is null)
                    {
                        for (int i = 0; i < owner.Items.Count; ++i)
                        {
                            if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Right)
                            {
                                if (myIndex == index)
                                {
                                    item = owner.Items[i];
                                    break;
                                }
                                myIndex++;
                            }
                        }
                    }

                    if (item is null)
                    {
                        Debug.Fail("No item matched the index??");
                        return null;
                    }

                    if (item.Placement == ToolStripItemPlacement.Overflow)
                    {
                        return new ToolStripAccessibleObjectWrapperForItemsOnOverflow(item);
                    }
                    return item.AccessibilityObject;
                }

                if (owner.CanOverflow && owner.OverflowButton.Visible && index == owner.Items.Count)
                {
                    return owner.OverflowButton.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  When overridden in a derived class, gets the number of children
            ///  belonging to an accessible object.
            /// </summary>
            public override int GetChildCount()
            {
                if ((owner is null) || (owner.Items is null))
                {
                    return -1;
                }

                int count = 0;
                for (int i = 0; i < owner.Items.Count; i++)
                {
                    if (owner.Items[i].Available)
                    {
                        count++;
                    }
                }
                if (owner.Grip.Visible)
                {
                    count++;
                }
                if (owner.CanOverflow && owner.OverflowButton.Visible)
                {
                    count++;
                }
                return count;
            }

            internal AccessibleObject GetChildFragment(int fragmentIndex, bool getOverflowItem = false)
            {
                ToolStripItemCollection items = getOverflowItem ? owner.OverflowItems : owner.DisplayedItems;
                int childFragmentCount = items.Count;

                if (!getOverflowItem && owner.CanOverflow && owner.OverflowButton.Visible && fragmentIndex == childFragmentCount - 1)
                {
                    return owner.OverflowButton.AccessibilityObject;
                }

                for (int index = 0; index < childFragmentCount; index++)
                {
                    ToolStripItem item = items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Left && fragmentIndex == index)
                    {
                        if (item is ToolStripControlHost controlHostItem)
                        {
                            return controlHostItem.ControlAccessibilityObject;
                        }

                        return item.AccessibilityObject;
                    }
                }

                for (int index = 0; index < childFragmentCount; index++)
                {
                    ToolStripItem item = owner.Items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Right && fragmentIndex == index)
                    {
                        if (item is ToolStripControlHost controlHostItem)
                        {
                            return controlHostItem.ControlAccessibilityObject;
                        }

                        return item.AccessibilityObject;
                    }
                }

                return null;
            }

            internal int GetChildOverflowFragmentCount()
            {
                if (owner is null || owner.OverflowItems is null)
                {
                    return -1;
                }

                return owner.OverflowItems.Count;
            }

            internal int GetChildFragmentCount()
            {
                if (owner is null || owner.DisplayedItems is null)
                {
                    return -1;
                }

                return owner.DisplayedItems.Count;
            }

            internal int GetChildFragmentIndex(ToolStripItem.ToolStripItemAccessibleObject child)
            {
                if (owner is null || owner.Items is null)
                {
                    return -1;
                }

                if (child.Owner == owner.Grip)
                {
                    return 0;
                }

                ToolStripItemCollection items;
                ToolStripItemPlacement placement = child.Owner.Placement;

                if (owner is ToolStripOverflow)
                {
                    // Overflow items in ToolStripOverflow host are in DisplayedItems collection.
                    items = owner.DisplayedItems;
                }
                else
                {
                    if (owner.CanOverflow && owner.OverflowButton.Visible && child.Owner == owner.OverflowButton)
                    {
                        return GetChildFragmentCount() - 1;
                    }

                    // Items can be either in DisplayedItems or in OverflowItems (if overflow)
                    items = (placement == ToolStripItemPlacement.Main) ? owner.DisplayedItems : owner.OverflowItems;
                }

                // First we walk through the head aligned items.
                for (int index = 0; index < items.Count; index++)
                {
                    ToolStripItem item = items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Left && child.Owner == items[index])
                    {
                        return index;
                    }
                }

                // If we didn't find it, then we walk through the tail aligned items.
                for (int index = 0; index < items.Count; index++)
                {
                    ToolStripItem item = items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Right && child.Owner == items[index])
                    {
                        return index;
                    }
                }

                return -1;
            }

            internal int GetChildIndex(ToolStripItem.ToolStripItemAccessibleObject child)
            {
                if ((owner is null) || (owner.Items is null))
                {
                    return -1;
                }

                int index = 0;
                if (owner.Grip.Visible)
                {
                    if (child.Owner == owner.Grip)
                    {
                        return 0;
                    }
                    index = 1;
                }

                if (owner.CanOverflow && owner.OverflowButton.Visible && child.Owner == owner.OverflowButton)
                {
                    return owner.Items.Count + index;
                }

                // First we walk through the head aligned items.
                for (int i = 0; i < owner.Items.Count; ++i)
                {
                    if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Left)
                    {
                        if (child.Owner == owner.Items[i])
                        {
                            return index;
                        }
                        index++;
                    }
                }

                // If we didn't find it, then we walk through the tail aligned items.
                for (int i = 0; i < owner.Items.Count; ++i)
                {
                    if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Right)
                    {
                        if (child.Owner == owner.Items[i])
                        {
                            return index;
                        }
                        index++;
                    }
                }

                return -1;
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    return AccessibleRole.ToolBar;
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return this;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        int childCount = GetChildFragmentCount();
                        if (childCount > 0)
                        {
                            return GetChildFragment(0);
                        }
                        break;
                    case UiaCore.NavigateDirection.LastChild:
                        childCount = GetChildFragmentCount();
                        if (childCount > 0)
                        {
                            return GetChildFragment(childCount - 1);
                        }
                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ToolBarControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                }

                return base.GetPropertyValue(propertyID);
            }
        }

        private class ToolStripAccessibleObjectWrapperForItemsOnOverflow : ToolStripItem.ToolStripItemAccessibleObject
        {
            public ToolStripAccessibleObjectWrapperForItemsOnOverflow(ToolStripItem item)
                : base(item)
            {
            }
            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = base.State;
                    state |= AccessibleStates.Offscreen;
                    state |= AccessibleStates.Invisible;
                    return state;
                }
            }
        }

        // When we click somewhere outside of the toolstrip it should be as if we hit esc.

        internal sealed class RestoreFocusMessageFilter : IMessageFilter
        {
            private readonly ToolStrip _ownerToolStrip;

            public RestoreFocusMessageFilter(ToolStrip ownerToolStrip)
            {
                _ownerToolStrip = ownerToolStrip;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (_ownerToolStrip.Disposing || _ownerToolStrip.IsDisposed || _ownerToolStrip.IsDropDown)
                {
                    return false;
                }
                // if the app has changed activation, restore focus

                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.LBUTTONDOWN:
                    case User32.WM.RBUTTONDOWN:
                    case User32.WM.MBUTTONDOWN:
                    case User32.WM.NCLBUTTONDOWN:
                    case User32.WM.NCRBUTTONDOWN:
                    case User32.WM.NCMBUTTONDOWN:
                        if (_ownerToolStrip.ContainsFocus)
                        {
                            // if we've clicked on something that's not a child of the toolstrip and we
                            // currently have focus, restore it.
                            if (!User32.IsChild(new HandleRef(_ownerToolStrip, _ownerToolStrip.Handle), m.HWnd).IsTrue())
                            {
                                IntPtr rootHwnd = User32.GetAncestor(_ownerToolStrip, User32.GA.ROOT);
                                if (rootHwnd == m.HWnd || User32.IsChild(rootHwnd, m.HWnd).IsTrue())
                                {
                                    // Only RestoreFocus if the hwnd is a child of the root window and isnt on the toolstrip.
                                    RestoreFocusInternal();
                                }
                            }
                        }
                        return false;

                    default:
                        return false;
                }
            }
            private void RestoreFocusInternal()
            {
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.RestoreFocusFilter] Detected a click, restoring focus.");

                _ownerToolStrip.BeginInvoke(new BooleanMethodInvoker(_ownerToolStrip.RestoreFocusInternal), new object[] { ToolStripManager.ModalMenuFilter.InMenuMode });

                // PERF,

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
            }
        }

        internal override bool ShowsOwnKeyboardToolTip()
        {
            bool hasVisibleSelectableItems = false;
            int i = Items.Count;
            while (i-- != 0 && !hasVisibleSelectableItems)
            {
                ToolStripItem item = Items[i];
                if (item.CanKeyboardSelect && item.Visible)
                {
                    hasVisibleSelectableItems = true;
                }
            }

            return !hasVisibleSelectableItems;
        }
    }

    internal class CachedItemHdcInfo : IDisposable, IHandle
    {
        internal CachedItemHdcInfo()
        {
        }

        private Gdi32.HDC _cachedItemHDC;
        private Size _cachedHDCSize = Size.Empty;
        private Gdi32.HBITMAP _cachedItemBitmap;

        public IntPtr Handle => (IntPtr)_cachedItemHDC;

        // this DC is cached and should only be deleted on Dispose or when the size changes.

        public Gdi32.HDC GetCachedItemDC(Gdi32.HDC toolStripHDC, Size bitmapSize)
        {
            if (_cachedHDCSize.Width < bitmapSize.Width
                 || _cachedHDCSize.Height < bitmapSize.Height)
            {
                if (_cachedItemHDC.IsNull)
                {
                    // Create a new DC - we dont have one yet.
                    _cachedItemHDC = Gdi32.CreateCompatibleDC(toolStripHDC);
                }

                // Create compatible bitmap with the correct size.
                _cachedItemBitmap = Gdi32.CreateCompatibleBitmap(toolStripHDC, bitmapSize.Width, bitmapSize.Height);
                Gdi32.HGDIOBJ oldBitmap = Gdi32.SelectObject(_cachedItemHDC, _cachedItemBitmap);

                // Delete the old bitmap
                if (!oldBitmap.IsNull)
                {
                    Gdi32.DeleteObject(oldBitmap);
                }

                // remember what size we created.
                _cachedHDCSize = bitmapSize;
            }

            return _cachedItemHDC;
        }

        public void Dispose()
        {
            if (!_cachedItemHDC.IsNull)
            {
                if (!_cachedItemBitmap.IsNull)
                {
                    Gdi32.DeleteObject(_cachedItemBitmap);
                }

                // delete the DC itself.
                Gdi32.DeleteDC(_cachedItemHDC);
            }

            _cachedItemHDC = default;
            _cachedItemBitmap = default;
            _cachedHDCSize = Size.Empty;

            GC.SuppressFinalize(this);
        }

        ~CachedItemHdcInfo()
        {
            Dispose();
        }
    }

    internal class MouseHoverTimer : IDisposable
    {
        private Timer _mouseHoverTimer = new Timer();

        // Consider - weak reference?
        private ToolStripItem _currentItem;

        public MouseHoverTimer()
        {
            _mouseHoverTimer.Interval = SystemInformation.MouseHoverTime;
            _mouseHoverTimer.Tick += new EventHandler(OnTick);
        }

        public void Start(ToolStripItem item)
        {
            if (item != _currentItem)
            {
                Cancel(_currentItem);
            }
            _currentItem = item;
            if (_currentItem != null)
            {
                _mouseHoverTimer.Enabled = true;
            }
        }

        public void Cancel()
        {
            _mouseHoverTimer.Enabled = false;
            _currentItem = null;
        }

        /// <summary> cancels if and only if this item was the one that
        ///  requested the timer
        /// </summary>
        public void Cancel(ToolStripItem item)
        {
            if (item == _currentItem)
            {
                Cancel();
            }
        }

        public void Dispose()
        {
            if (_mouseHoverTimer != null)
            {
                Cancel();
                _mouseHoverTimer.Dispose();
                _mouseHoverTimer = null;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            _mouseHoverTimer.Enabled = false;
            if (_currentItem != null && !_currentItem.IsDisposed)
            {
                _currentItem.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseHover);
            }
        }
    }

    /// <summary>
    ///  This class supports the AllowItemReorder feature.
    ///  When reordering items ToolStrip and ToolStripItem drag/drop events are routed here.
    /// </summary>
    internal sealed class ToolStripSplitStackDragDropHandler : IDropTarget, ISupportOleDropSource
    {
        private readonly ToolStrip _owner;

        public ToolStripSplitStackDragDropHandler(ToolStrip owner)
        {
            this._owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public void OnDragEnter(DragEventArgs e)
        {
            Debug.WriteLineIf(ToolStrip.s_itemReorderDebug.TraceVerbose, "OnDragEnter: " + e.ToString());
            if (e.Data.GetDataPresent(typeof(ToolStripItem)))
            {
                e.Effect = DragDropEffects.Move;
                ShowItemDropPoint(_owner.PointToClient(new Point(e.X, e.Y)));
            }
        }

        public void OnDragLeave(EventArgs e)
        {
            Debug.WriteLineIf(ToolStrip.s_itemReorderDebug.TraceVerbose, "OnDragLeave: " + e.ToString());
            _owner.ClearInsertionMark();
        }

        public void OnDragDrop(DragEventArgs e)
        {
            Debug.WriteLineIf(ToolStrip.s_itemReorderDebug.TraceVerbose, "OnDragDrop: " + e.ToString());

            if (e.Data.GetDataPresent(typeof(ToolStripItem)))
            {
                ToolStripItem item = (ToolStripItem)e.Data.GetData(typeof(ToolStripItem));
                OnDropItem(item, _owner.PointToClient(new Point(e.X, e.Y)));
            }
        }
        public void OnDragOver(DragEventArgs e)
        {
            Debug.WriteLineIf(ToolStrip.s_itemReorderDebug.TraceVerbose, "OnDragOver: " + e.ToString());

            if (e.Data.GetDataPresent(typeof(ToolStripItem)))
            {
                if (ShowItemDropPoint(_owner.PointToClient(new Point(e.X, e.Y))))
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    if (_owner != null)
                    {
                        _owner.ClearInsertionMark();
                    }
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        public void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
        }

        public void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
        }

        private void OnDropItem(ToolStripItem droppedItem, Point ownerClientAreaRelativeDropPoint)
        {
            Point start = Point.Empty;

            int toolStripItemIndex = GetItemInsertionIndex(ownerClientAreaRelativeDropPoint);
            if (toolStripItemIndex >= 0)
            {
                ToolStripItem item = _owner.Items[toolStripItemIndex];
                if (item == droppedItem)
                {
                    _owner.ClearInsertionMark();
                    return;  // optimization
                }

                RelativeLocation relativeLocation = ComparePositions(item.Bounds, ownerClientAreaRelativeDropPoint);
                droppedItem.Alignment = item.Alignment;

                // Protect against negative indicies
                int insertIndex = Math.Max(0, toolStripItemIndex);

                if (relativeLocation == RelativeLocation.Above)
                {
                    insertIndex = (item.Alignment == ToolStripItemAlignment.Left) ? insertIndex : insertIndex + 1;
                }
                else if (relativeLocation == RelativeLocation.Below)
                {
                    insertIndex = (item.Alignment == ToolStripItemAlignment.Left) ? insertIndex : insertIndex - 1;
                }
                else if (((item.Alignment == ToolStripItemAlignment.Left) && (relativeLocation == RelativeLocation.Left)) ||
                    ((item.Alignment == ToolStripItemAlignment.Right) && (relativeLocation == RelativeLocation.Right)))
                {
                    // the item alignment is Tail & dropped to right of the center of the item
                    // or the item alignment is Head & dropped to the left of the center of the item

                    // Normally, insert the new item after the item, however in RTL insert before the item
                    insertIndex = Math.Max(0, (_owner.RightToLeft == RightToLeft.Yes) ? insertIndex + 1 : insertIndex);
                }
                else
                {
                    // the item alignment is Tail & dropped to left of the center of the item
                    // or the item alignment is Head & dropped to the right of the center of the item

                    // Normally, insert the new item before the item, however in RTL insert after the item
                    insertIndex = Math.Max(0, (_owner.RightToLeft == RightToLeft.No) ? insertIndex + 1 : insertIndex);
                }

                // If the control is moving from a lower to higher index, you actually want to set it one less than its position.
                // This is because it is being removed from its original position, which lowers the index of every control before
                // its new drop point by 1.
                if (_owner.Items.IndexOf(droppedItem) < insertIndex)
                {
                    insertIndex--;
                }

                _owner.Items.MoveItem(Math.Max(0, insertIndex), droppedItem);
                _owner.ClearInsertionMark();
            }
            else if (toolStripItemIndex == -1 && _owner.Items.Count == 0)
            {
                _owner.Items.Add(droppedItem);
                _owner.ClearInsertionMark();
            }
        }

        private bool ShowItemDropPoint(Point ownerClientAreaRelativeDropPoint)
        {
            int i = GetItemInsertionIndex(ownerClientAreaRelativeDropPoint);
            if (i >= 0)
            {
                ToolStripItem item = _owner.Items[i];
                RelativeLocation relativeLocation = ComparePositions(item.Bounds, ownerClientAreaRelativeDropPoint);

                Debug.WriteLineIf(ToolStrip.s_itemReorderDebug.TraceVerbose, "Drop relative loc " + relativeLocation);
                Debug.WriteLineIf(ToolStrip.s_itemReorderDebug.TraceVerbose, "Index " + i);

                Rectangle insertionRect = Rectangle.Empty;
                switch (relativeLocation)
                {
                    case RelativeLocation.Above:
                        insertionRect = new Rectangle(_owner.Margin.Left, item.Bounds.Top, _owner.Width - (_owner.Margin.Horizontal) - 1, ToolStrip.s_insertionBeamWidth);
                        break;
                    case RelativeLocation.Below:
                        insertionRect = new Rectangle(_owner.Margin.Left, item.Bounds.Bottom, _owner.Width - (_owner.Margin.Horizontal) - 1, ToolStrip.s_insertionBeamWidth);
                        break;
                    case RelativeLocation.Right:
                        insertionRect = new Rectangle(item.Bounds.Right, _owner.Margin.Top, ToolStrip.s_insertionBeamWidth, _owner.Height - (_owner.Margin.Vertical) - 1);
                        break;
                    case RelativeLocation.Left:
                        insertionRect = new Rectangle(item.Bounds.Left, _owner.Margin.Top, ToolStrip.s_insertionBeamWidth, _owner.Height - (_owner.Margin.Vertical) - 1);
                        break;
                }

                _owner.PaintInsertionMark(insertionRect);
                return true;
            }
            else if (_owner.Items.Count == 0)
            {
                Rectangle insertionRect = _owner.DisplayRectangle;
                insertionRect.Width = ToolStrip.s_insertionBeamWidth;
                _owner.PaintInsertionMark(insertionRect);
                return true;
            }
            return false;
        }

        private int GetItemInsertionIndex(Point ownerClientAreaRelativeDropPoint)
        {
            for (int i = 0; i < _owner.DisplayedItems.Count; i++)
            {
                Rectangle bounds = _owner.DisplayedItems[i].Bounds;
                bounds.Inflate(_owner.DisplayedItems[i].Margin.Size);
                if (bounds.Contains(ownerClientAreaRelativeDropPoint))
                {
                    Debug.WriteLineIf(ToolStrip.s_dropTargetDebug.TraceVerbose, "MATCH " + _owner.DisplayedItems[i].Text + " Bounds: " + _owner.DisplayedItems[i].Bounds.ToString());

                    // consider what to do about items not in the display
                    return _owner.Items.IndexOf(_owner.DisplayedItems[i]);
                }
            }

            if (_owner.DisplayedItems.Count > 0)
            {
                for (int i = 0; i < _owner.DisplayedItems.Count; i++)
                {
                    if (_owner.DisplayedItems[i].Alignment == ToolStripItemAlignment.Right)
                    {
                        if (i > 0)
                        {
                            return _owner.Items.IndexOf(_owner.DisplayedItems[i - 1]);
                        }
                        return _owner.Items.IndexOf(_owner.DisplayedItems[i]);
                    }
                }
                return _owner.Items.IndexOf(_owner.DisplayedItems[_owner.DisplayedItems.Count - 1]);
            }
            return -1;
        }

        private enum RelativeLocation
        {
            Above,
            Below,
            Right,
            Left
        }

        private RelativeLocation ComparePositions(Rectangle orig, Point check)
        {
            if (_owner.Orientation == Orientation.Horizontal)
            {
                int widthUnit = orig.Width / 2;
                RelativeLocation relativeLocation = RelativeLocation.Left;

                // we can return here if we are checking abovebelowleftright, because
                // the left right calculation is more picky than the above/below calculation
                // and the above below calculation will just override this one.
                if ((orig.Left + widthUnit) >= check.X)
                {
                    relativeLocation = RelativeLocation.Left;
                    return relativeLocation;
                }
                else if ((orig.Right - widthUnit) <= check.X)
                {
                    relativeLocation = RelativeLocation.Right;
                    return relativeLocation;
                }
            }

            if (_owner.Orientation == Orientation.Vertical)
            {
                int heightUnit = orig.Height / 2;
                RelativeLocation relativeLocation = (check.Y <= (orig.Top + heightUnit)) ?
                    RelativeLocation.Above
                    : RelativeLocation.Below;

                return relativeLocation;
            }

            Debug.Fail("Could not calculate the relative position for AllowItemReorder");
            return RelativeLocation.Left;
        }
    }
}
