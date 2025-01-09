// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;
using Microsoft.Win32;

namespace System.Windows.Forms;

/// <summary>
///  ToolStrip control.
/// </summary>
[DesignerSerializer($"System.Windows.Forms.Design.ToolStripCodeDomSerializer, {AssemblyRef.SystemDesign}",
    $"System.ComponentModel.Design.Serialization.CodeDomSerializer, {AssemblyRef.SystemDesign}")]
[Designer($"System.Windows.Forms.Design.ToolStripDesigner, {AssemblyRef.SystemDesign}")]
[DefaultProperty(nameof(Items))]
[SRDescription(nameof(SR.DescriptionToolStrip))]
[DefaultEvent(nameof(ItemClicked))]
public partial class ToolStrip : ScrollableControl, IArrangedElement, ISupportToolStripPanel
{
    private static Size s_onePixel = new(1, 1);
    internal static Point s_invalidMouseEnter = new(int.MaxValue, int.MaxValue);

    private ToolStripItemCollection? _toolStripItemCollection;
    private ToolStripOverflowButton? _toolStripOverflowButton;
    private ToolStripGrip? _toolStripGrip;
    private ToolStripItemCollection? _displayedItems;
    private ToolStripItemCollection? _overflowItems;
    private ToolStripDropTargetManager? _dropTargetManager;
    private HWND _hwndThatLostFocus;
    private ToolStripItem? _lastMouseActiveItem;
    private ToolStripItem? _lastMouseDownedItem;
    private LayoutEngine _layoutEngine;
    private ToolStripLayoutStyle _layoutStyle = ToolStripLayoutStyle.StackWithOverflow;
    private Rectangle _lastInsertionMarkRect = Rectangle.Empty;
    private ImageList? _imageList;
    private ToolStripGripStyle _toolStripGripStyle = ToolStripGripStyle.Visible;
    private int _toolStripState;
    private bool _showItemToolTips;
    private MouseHoverTimer? _mouseHoverTimer;
    private ToolStripItem? _currentlyActiveTooltipItem;
    private NativeWindow? _dropDownOwnerWindow;
    private byte _mouseDownID;  // NEVER use this directly from another class, 0 should never be returned to another class.
    private ToolStripRenderer? _renderer;
    private Type _currentRendererType = typeof(Type);
    private Dictionary<Keys, ToolStripMenuItem>? _shortcuts;
    private Stack<MergeHistory>? _mergeHistoryStack;
    private ToolStripDropDownDirection _toolStripDropDownDirection = ToolStripDropDownDirection.Default;
    private Size _largestDisplayedItemSize = Size.Empty;
    private CachedItemHdcInfo? _cachedItemHdcInfo;
    private bool _alreadyHooked;

    private Size _imageScalingSize;
    private const int LogicalIconSize = 16;
    private static int s_iconSize = LogicalIconSize;

    private Font? _defaultFont;
    private RestoreFocusMessageFilter? _restoreFocusFilter;
    private static readonly Padding s_logicalDefaultPadding = new(0, 0, 1, 0);
    private static readonly Padding s_logicalDefaultGripMargin = new(2);
    private Padding _defaultPadding = s_logicalDefaultPadding;
    private Padding _defaultGripMargin = s_logicalDefaultGripMargin;

    private Point _mouseEnterWhenShown = s_invalidMouseEnter;

    private const int LogicalInsertionBeamWidth = 6;

    internal static int s_insertionBeamWidth = LogicalInsertionBeamWidth;

    private static readonly object s_eventPaintGrip = new();
    private static readonly object s_eventLayoutCompleted = new();
    private static readonly object s_eventItemAdded = new();
    private static readonly object s_eventItemRemoved = new();
    private static readonly object s_eventLayoutStyleChanged = new();
    private static readonly object s_eventRendererChanged = new();
    private static readonly object s_eventItemClicked = new();
    private static readonly object s_eventLocationChanging = new();
    private static readonly object s_eventBeginDrag = new();
    private static readonly object s_eventEndDrag = new();

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

    private delegate void BooleanMethodInvoker(bool arg);
    internal Action<int, int>? _rescaleConstsCallbackDelegate;

    public ToolStrip()
    {
        if (ScaleHelper.IsThreadPerMonitorV2Aware)
        {
            ToolStripManager.CurrentDpi = DeviceDpi;
            _defaultFont = ToolStripManager.DefaultFont;
        }

        s_insertionBeamWidth = ScaleHelper.ScaleToDpi(LogicalInsertionBeamWidth, DeviceDpi);
        _defaultPadding = ScaleHelper.ScaleToDpi(s_logicalDefaultPadding, DeviceDpi);
        _defaultGripMargin = ScaleHelper.ScaleToDpi(s_logicalDefaultGripMargin, DeviceDpi);
        s_iconSize = ScaleHelper.ScaleToDpi(LogicalIconSize, DeviceDpi);
        _imageScalingSize = new Size(s_iconSize, s_iconSize);

        SuspendLayout();
        CanOverflow = true;
        TabStop = false;
        MenuAutoExpand = false;
        SetStyle(
            ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor,
            true);

        SetStyle(ControlStyles.Selectable, false);
        SetToolStripState(STATE_USEDEFAULTRENDERER | STATE_ALLOWMERGE, true);

        SetExtendedState(
            // A toolstrip does not take capture on MouseDown.
            ExtendedStates.MaintainsOwnCaptureMode
                // This class overrides GetPreferredSizeCore, let Control automatically cache the result.
                | ExtendedStates.UserPreferredSizeCache,
                true);

        // Add a weak reference link in ToolstripManager.
        ToolStripManager.ToolStrips.Add(this);

        _layoutEngine = new ToolStripSplitStackLayout(this);
        Dock = DefaultDock;
        AutoSize = true;
        CausesValidation = false;
        SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
        ShowItemToolTips = DefaultShowItemToolTips;
        ResumeLayout(true);
    }

    public ToolStrip(params ToolStripItem[] items)
        : this()
    {
        Items.AddRange(items);
    }

    internal List<ToolStripDropDown> ActiveDropDowns { get; } = new(1);

    // returns true when entered into menu mode through this toolstrip/menustrip
    // this is only really supported for menustrip active event, but to prevent casting everywhere...
    internal virtual bool KeyboardActive
    {
        get { return GetToolStripState(STATE_MENUACTIVE); }
        set { SetToolStripState(STATE_MENUACTIVE, value); }
    }

    // This is only for use in determining whether to show scroll bars on
    // ToolStripDropDownMenus. No one else should be using it for anything.
    internal virtual bool AllItemsVisible
    {
        get
        {
            return true;
        }
        set
        {
            // we do nothing in response to a set, since we calculate the value above.
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
                // which would join it to a new row. Set the specified bounds to the new location to
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
    public new event EventHandler? AutoSizeChanged
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

    /// <summary>
    ///  When <see langword="true"/> Allows the control to be interacted when window does not have focus.
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripAllowClickThrough))]
    [SRCategory(nameof(SR.CatBehavior))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool AllowClickThrough { get; set; }

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
                DropTargetManager.EnsureRegistered();
            }
            else
            {
                DropTargetManager.EnsureUnRegistered();
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

                if (value)
                {
                    ToolStripSplitStackDragDropHandler dragDropHandler = new(this);
                    ItemReorderDropSource = dragDropHandler;
                    ItemReorderDropTarget = dragDropHandler;

                    DropTargetManager.EnsureRegistered();
                }
                else
                {
                    DropTargetManager.EnsureUnRegistered();
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
    public event EventHandler? BeginDrag
    {
        add => Events.AddHandler(s_eventBeginDrag, value);
        remove => Events.RemoveHandler(s_eventBeginDrag, value);
    }

    public override BindingContext? BindingContext
    {
        get
        {
            if (Properties.TryGetValue(s_propBindingContext, out BindingContext? context))
            {
                return context;
            }

            // Try the parent.
            if (ParentInternal is { } parent && parent.CanAccessProperties)
            {
                return parent.BindingContext;
            }

            // We don't have a binding context.
            return null;
        }
        set
        {
            if (Properties.AddOrRemoveValue(s_propBindingContext, value) != value)
            {
                // Re-wire the bindings.
                OnBindingContextChanged(EventArgs.Empty);
            }
        }
    }

    [DefaultValue(true)]
    [SRDescription(nameof(SR.ToolStripCanOverflowDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public bool CanOverflow
    {
        get => GetToolStripState(STATE_CANOVERFLOW);
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
    ///  SCENARIO: put focus in combo box, move the mouse over another item... selection
    ///  should not shift until the ComboBox relinquishes its focus.
    /// </summary>
    internal bool CanHotTrack
    {
        get
        {
            if (!Focused)
            {
                // if  ContainsFocus in one of the children = false, someone is just mousing by, we can hot track
                return !ContainsFocus;
            }
            else
            {
                // if the toolstrip itself contains focus we can definitely hottrack.
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
    public new event EventHandler? CausesValidationChanged
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
    public new event ControlEventHandler? ControlAdded
    {
        add => base.ControlAdded += value;
        remove => base.ControlAdded -= value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override Cursor Cursor
    {
        get => base.Cursor;
        set => base.Cursor = value;
    }

    /// <summary>
    ///  Hide browsable property
    /// </summary>
    [Browsable(false)]
    public new event EventHandler? CursorChanged
    {
        add => base.CursorChanged += value;
        remove => base.CursorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event ControlEventHandler? ControlRemoved
    {
        add => base.ControlRemoved += value;
        remove => base.ControlRemoved -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripOnEndDrag))]
    public event EventHandler? EndDrag
    {
        add => Events.AddHandler(s_eventEndDrag, value);
        remove => Events.RemoveHandler(s_eventEndDrag, value);
    }

    [AllowNull]
    public override Font Font
    {
        get
        {
            if (LocalAppContextSwitches.ApplyParentFontToMenus || IsFontSet())
            {
                return base.Font;
            }

            // since toolstrip manager default font is thread static, hold onto a copy of the
            // pointer in an instance variable for perf so we don't have to keep fishing into
            // thread local storage for it.
            _defaultFont ??= ToolStripManager.DefaultFont;

            return _defaultFont;
        }
        set => base.Font = value;
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize => ScaleHelper.IsThreadPerMonitorV2Aware
        ? ScaleHelper.ScaleToDpi(new Size(100, 25), DeviceDpi)
        : new Size(100, 25);

    protected override Padding DefaultPadding
    {
        get
        {
            // one pixel from the right edge to prevent the right border from painting over the
            // aligned-right toolstrip item.
            return _defaultPadding;
        }
    }

    protected override Padding DefaultMargin => Padding.Empty;

    protected virtual DockStyle DefaultDock => DockStyle.Top;

    protected virtual Padding DefaultGripMargin =>
        _toolStripGrip is not null ? _toolStripGrip.DefaultMargin : _defaultGripMargin;

    protected virtual bool DefaultShowItemToolTips => true;

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
                        DockStyle actualDock = (ParentInternal is not null) ? ParentInternal.Dock : DockStyle.Left;
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
                    if (IsInToolStripPanel && ParentInternal is not null)
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
            // can't use Enum.IsValid as its not sequential
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
                    // change the orientation. Instead we've duplicated the logic of Control.Dock.set here, but with a
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
    ///  Returns an owner window that can be used to own a drop down.
    /// </summary>
    internal virtual NativeWindow DropDownOwnerWindow
    {
        get
        {
            _dropDownOwnerWindow ??= new NativeWindow();

            if (_dropDownOwnerWindow.Handle == IntPtr.Zero)
            {
                CreateParams cp = new CreateParams
                {
                    ExStyle = (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                };
                _dropDownOwnerWindow.CreateHandle(cp);
            }

            return _dropDownOwnerWindow;
        }
    }

    /// <summary>
    ///  Returns the drop target manager that all the hwndless
    ///  items and this ToolStrip share. this is necessary as
    ///  RegisterDragDrop requires an HWND.
    /// </summary>
    [AllowNull]
    internal ToolStripDropTargetManager DropTargetManager
    {
        get
        {
            _dropTargetManager ??= new ToolStripDropTargetManager(this);

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
            _displayedItems ??= new ToolStripItemCollection(this, false);

            return _displayedItems;
        }
    }

    /// <summary>
    ///  Retrieves the current display rectangle. The display rectangle
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
    ///  [ToolStrip ForeColorChanged event, overridden to turn browsing off.]
    /// </summary>
    [Browsable(false)]
    public new event EventHandler? ForeColorChanged
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
            _toolStripGrip ??= new ToolStripGrip
            {
                Overflow = ToolStripItemOverflow.Never,
                Visible = _toolStripGripStyle == ToolStripGripStyle.Visible,
                AutoSize = false,
                ParentInternal = this,
                Margin = DefaultGripMargin
            };

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
            // valid values are 0x0 to 0x1
            SourceGenerated.EnumValidator.Validate(value);
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
    ///  The boundaries of the grip on the ToolStrip. If it is invisible - returns Rectangle.Empty.
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
    public new HScrollProperties HorizontalScroll
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
    ///  Gets or sets the <see cref="Forms.ImageList"/> that contains the <see cref="Image"/> displayed on a label control.
    /// </summary>
    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripImageListDescr))]
    [Browsable(false)]
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
                EventHandler handler = new(ImageListRecreateHandle);

                // Remove the previous imagelist handle recreate handler.
                if (_imageList is not null)
                {
                    _imageList.RecreateHandle -= handler;
                }

                _imageList = value;

                // Add the new imagelist handle recreate handler.
                if (value is not null)
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

    [MemberNotNullWhen(true, nameof(ToolStripPanelRow))]
    internal bool IsInToolStripPanel
    {
        get
        {
            return ToolStripPanelRow is not null;
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
    ///  in this collection will be displayed. They may not even be displayed
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
            _toolStripItemCollection ??= new ToolStripItemCollection(this, true);

            return _toolStripItemCollection;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemAddedDescr))]
    public event ToolStripItemEventHandler? ItemAdded
    {
        add => Events.AddHandler(s_eventItemAdded, value);
        remove => Events.RemoveHandler(s_eventItemAdded, value);
    }

    /// <summary>
    ///  Occurs when the control is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ToolStripItemOnClickDescr))]
    public event ToolStripItemClickedEventHandler? ItemClicked
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
    ///  This is done because GDI won't honor GDI+ TranslateTransform. We used to use DCMapping to change the viewport
    ///  origin and clipping rect of the toolstrip double buffer hdc to paint each item, but this proves costly
    ///  because you need to allocate GDI+ Graphics objects for every single item. This method allows us to only
    ///  allocate 1 Graphics object and share it between all the items in OnPaint.
    /// </summary>
    private CachedItemHdcInfo ItemHdcInfo
    {
        get
        {
            _cachedItemHdcInfo ??= new CachedItemHdcInfo();

            return _cachedItemHdcInfo;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemRemovedDescr))]
    public event ToolStripItemEventHandler? ItemRemoved
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
    internal IDropTarget? ItemReorderDropTarget { get; set; }

    /// <summary>
    ///  The OnQueryContinueDrag and OnGiveFeedback methods that will be called if
    ///  AllowItemReorder is true.
    ///
    ///  This allows us to have methods that handle drag/drop of the ToolStrip items
    ///  without calling back on the user's code
    /// </summary>
    internal ISupportOleDropSource? ItemReorderDropSource { get; set; }

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

    internal ToolStripItem? LastMouseDownedItem
    {
        get
        {
            if (_lastMouseDownedItem is not null &&
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
    public LayoutSettings? LayoutSettings { get; set; }

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
            // valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);
            if (_layoutStyle != value)
            {
                _layoutStyle = value;

                switch (value)
                {
                    case ToolStripLayoutStyle.Flow:
                        if (_layoutEngine is not FlowLayout)
                        {
                            _layoutEngine = FlowLayout.Instance;
                        }

                        // Orientation really only applies to split stack layout (which swaps based on Dock, ToolStripPanel location)
                        UpdateOrientation(Orientation.Horizontal);
                        break;
                    case ToolStripLayoutStyle.Table:

                        if (_layoutEngine is not TableLayout)
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

                        if (_layoutEngine is not ToolStripSplitStackLayout)
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
    public event EventHandler? LayoutCompleted
    {
        add => Events.AddHandler(s_eventLayoutCompleted, value);
        remove => Events.RemoveHandler(s_eventLayoutCompleted, value);
    }

    internal bool LayoutRequired { get; set; }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripLayoutStyleChangedDescr))]
    public event EventHandler? LayoutStyleChanged
    {
        add => Events.AddHandler(s_eventLayoutStyleChanged, value);
        remove => Events.RemoveHandler(s_eventLayoutStyleChanged, value);
    }

    public override LayoutEngine LayoutEngine
    {
        get
        {
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
            _mergeHistoryStack ??= new Stack<MergeHistory>();

            return _mergeHistoryStack;
        }
    }

    private MouseHoverTimer MouseHoverTimer
    {
        get
        {
            _mouseHoverTimer ??= new MouseHoverTimer();

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

    internal ToolStripItemCollection OverflowItems
    {
        get
        {
            _overflowItems ??= new ToolStripItemCollection(this, false);

            return _overflowItems;
        }
    }

    [Browsable(false)]
    public Orientation Orientation { get; private set; } = Orientation.Horizontal;

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripPaintGripDescr))]
    public event PaintEventHandler? PaintGrip
    {
        add => Events.AddHandler(s_eventPaintGrip, value);
        remove => Events.RemoveHandler(s_eventPaintGrip, value);
    }

    internal RestoreFocusMessageFilter RestoreFocusFilter => _restoreFocusFilter ??= new RestoreFocusMessageFilter(this);

    internal ToolStripPanelCell? ToolStripPanelCell => ((ISupportToolStripPanel)this).ToolStripPanelCell;

    internal ToolStripPanelRow? ToolStripPanelRow => ((ISupportToolStripPanel)this).ToolStripPanelRow;

    // fetches the Cell associated with this toolstrip.
    ToolStripPanelCell? ISupportToolStripPanel.ToolStripPanelCell
    {
        get
        {
            ToolStripPanelCell? toolStripPanelCell = null;
            if (!IsDropDown && !IsDisposed && !Properties.TryGetValue(s_propToolStripPanelCell, out toolStripPanelCell))
            {
                toolStripPanelCell = Properties.AddValue(s_propToolStripPanelCell, new ToolStripPanelCell(this));
            }

            return toolStripPanelCell;
        }
    }

    ToolStripPanelRow? ISupportToolStripPanel.ToolStripPanelRow
    {
        get => ToolStripPanelCell?.ToolStripPanelRow;
        set
        {
            ToolStripPanelRow? oldToolStripPanelRow = ToolStripPanelRow;

            if (oldToolStripPanelRow == value)
            {
                return;
            }

            ToolStripPanelCell? cell = ToolStripPanelCell;
            if (cell is null)
            {
                return;
            }

            cell.ToolStripPanelRow = value;

            if (value is not null)
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
                if (oldToolStripPanelRow is not null && oldToolStripPanelRow.ControlsInternal.Contains(this))
                {
                    oldToolStripPanelRow.ControlsInternal.Remove(this);
                }

                UpdateLayoutStyle(Dock);
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
    ///  The renderer is used to paint the hwndless ToolStrip items. If someone wanted to
    ///  change the "Hot" look of all of their buttons to be a green triangle, they should
    ///  create a class that derives from ToolStripRenderer, assign it to this property and call
    ///  invalidate.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public ToolStripRenderer Renderer
    {
        get
        {
            if (IsDropDown)
            {
                // PERF: since this is called a lot we don't want to make it virtual
                var dropDown = (ToolStripDropDown)this;
                if (dropDown is ToolStripOverflow || dropDown.IsAutoGenerated)
                {
                    if (dropDown.OwnerToolStrip is not null)
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
                _renderer = ToolStripManager.CreateRenderer(RenderMode);
                _currentRendererType = _renderer.GetType();
                OnRendererChanged(EventArgs.Empty);
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
                _currentRendererType = (_renderer is not null) ? _renderer.GetType() : typeof(Type);
                OnRendererChanged(EventArgs.Empty);
            }
        }
    }

    public event EventHandler? RendererChanged
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

            if (_renderer is not null && !_renderer.IsAutoGenerated)
            {
                return ToolStripRenderMode.Custom;
            }

            // check the type of the currently set renderer.
            // types are cached as this may be called frequently.
            return _currentRendererType == ToolStripManager.s_professionalRendererType
                ? ToolStripRenderMode.Professional
                : _currentRendererType == ToolStripManager.s_systemRendererType
                    ? ToolStripRenderMode.System
                    : ToolStripRenderMode.Custom;
        }
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
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
    ///  for their accelerators. Since they are not HWNDs, and this method is protected on control
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
                    UpdateToolTip(item: null);
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
                if (_toolStripOverflowButton is not null && OverflowButton.HasDropDownItems)
                {
                    OverflowButton.DropDown.ShowItemToolTips = value;
                }
            }
        }
    }

    /// <summary> internal lookup table for shortcuts... intended to speed search time </summary>
    internal Dictionary<Keys, ToolStripMenuItem> Shortcuts
    {
        get
        {
            _shortcuts ??= new Dictionary<Keys, ToolStripMenuItem>(1);

            return _shortcuts;
        }
    }

    /// <summary>
    ///  Indicates whether the user can give the focus to this control using the TAB
    ///  key. This property is read-only.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [DispId(PInvokeCore.DISPID_TABSTOP)]
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
            if (!Properties.TryGetValue(s_propToolTip, out ToolTip? toolTip))
            {
                toolTip = Properties.AddValue(s_propToolTip, new ToolTip());
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
            ToolStripTextDirection textDirection = Properties.GetValueOrDefault(s_propTextDirection, ToolStripTextDirection.Horizontal);

            if (textDirection == ToolStripTextDirection.Inherit)
            {
                textDirection = ToolStripTextDirection.Horizontal;
            }

            return textDirection;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            Properties.AddOrRemoveValue(s_propTextDirection, value, defaultValue: ToolStripTextDirection.Horizontal);

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
    public new VScrollProperties VerticalScroll => base.VerticalScroll;

    void ISupportToolStripPanel.BeginDrag() => OnBeginDrag(EventArgs.Empty);

    internal virtual void ChangeSelection(ToolStripItem? nextItem)
    {
        if (nextItem is null)
        {
            return;
        }

        ToolStripControlHost? controlHost = nextItem as ToolStripControlHost;
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

        if (controlHost is not null)
        {
            if (_hwndThatLostFocus == IntPtr.Zero)
            {
                SnapFocus(PInvoke.GetFocus());
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

    protected virtual LayoutSettings? CreateLayoutSettings(ToolStripLayoutStyle layoutStyle) => layoutStyle switch
    {
        ToolStripLayoutStyle.Flow => new FlowLayoutSettings(this),
        ToolStripLayoutStyle.Table => new TableLayoutSettings(this),
        _ => null,
    };

    protected internal virtual ToolStripItem CreateDefaultItem(string? text, Image? image, EventHandler? onClick) =>
        text == "-" ? new ToolStripSeparator() : new ToolStripButton(text, image, onClick);

    private void ClearAllSelections() => ClearAllSelectionsExcept(null);

    private void ClearAllSelectionsExcept(ToolStripItem? item)
    {
        Rectangle regionRect = (item is null) ? Rectangle.Empty : item.Bounds;
        Region? region = null;

        // Copy displayed items collection so it doesn't mutate when we begin to hide items.
        ToolStripItem[] displayedItems = new ToolStripItem[DisplayedItems.Count];
        DisplayedItems.CopyTo(displayedItems, 0);

        try
        {
            for (int i = 0; i < displayedItems.Length; i++)
            {
                if (displayedItems[i] == item)
                {
                    continue;
                }

                if (item is not null
                    && displayedItems[i] is ToolStripDropDownItem dropDownItem
                    && dropDownItem.Pressed
                    && dropDownItem.HasDropDownItems)
                {
                    dropDownItem.AutoHide(item);
                }

                bool invalidate = false;
                if (displayedItems[i].Selected)
                {
                    displayedItems[i].Unselect();
                    invalidate = true;
                }

                if (invalidate)
                {
                    // since regions are heavy weight - only use if we need it.
                    region ??= new Region(regionRect);
                    region.Union(displayedItems[i].Bounds);
                }
            }

            // force an WM_PAINT to happen now to instantly reflect the selection change.
            if (region is not null)
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
            region?.Dispose();
        }

        // fire accessibility
        if (IsHandleCreated && item is not null)
        {
            int focusIndex = Array.IndexOf(displayedItems, item);
            AccessibilityNotifyClients(AccessibleEvents.Focus, focusIndex);
        }
    }

    internal void ClearInsertionMark()
    {
        if (_lastInsertionMarkRect != Rectangle.Empty)
        {
            // stuff away the lastInsertionMarkRect
            // and clear it out _before_ we call paint OW
            // the call to invalidate won't help as it will get
            // repainted.
            Rectangle invalidate = _lastInsertionMarkRect;
            _lastInsertionMarkRect = Rectangle.Empty;

            Invalidate(invalidate);
        }
    }

    private void ClearLastMouseDownedItem()
    {
        ToolStripItem? lastItem = _lastMouseDownedItem;
        _lastMouseDownedItem = null;
        if (IsSelectionSuspended)
        {
            SetToolStripState(STATE_LASTMOUSEDOWNEDITEMCAPTURE, false);
            lastItem?.Invalidate();
        }
    }

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ToolStripOverflow? overflow = GetOverflow();

            try
            {
                SuspendLayout();
                overflow?.SuspendLayout();

                // If there's a problem in config, don't be a leaker.
                SetToolStripState(STATE_DISPOSINGITEMS, true);
                _lastMouseDownedItem = null;

                HookStaticEvents(hook: false);

                if (Properties.TryGetValue(s_propToolStripPanelCell, out ToolStripPanelCell? toolStripPanelCell))
                {
                    toolStripPanelCell.Dispose();
                    Properties.RemoveValue(s_propToolStripPanelCell);
                }

                _cachedItemHdcInfo?.Dispose();

                _mouseHoverTimer?.Dispose();

                if (Properties.TryGetValue(s_propToolTip, out ToolTip? toolTip))
                {
                    toolTip.Dispose();
                    Properties.RemoveValue(s_propToolTip);
                }

                if (!Items.IsReadOnly)
                {
                    // Only dispose the items we actually own.
                    for (int i = Items.Count - 1; i >= 0; i--)
                    {
                        Items[i].Dispose();
                    }

                    Items.Clear();
                }

                // Clean up items not in the Items list.
                _toolStripGrip?.Dispose();

                _toolStripOverflowButton?.Dispose();

                // Remove the restore focus filter.
                if (_restoreFocusFilter is not null)
                {
                    Application.ThreadContext.FromCurrent().RemoveMessageFilter(_restoreFocusFilter);
                    _restoreFocusFilter = null;
                }

                // Exit menu mode if necessary.
                bool exitMenuMode = false;
                if (ToolStripManager.ModalMenuFilter.GetActiveToolStrip() == this)
                {
                    exitMenuMode = true;
                }

                ToolStripManager.ModalMenuFilter.RemoveActiveToolStrip(this);
                // if we were the last toolstrip in the queue, exit menu mode.
                if (exitMenuMode && ToolStripManager.ModalMenuFilter.GetActiveToolStrip() is null)
                {
                    ToolStripManager.ModalMenuFilter.ExitMenuMode();
                }

                ToolStripManager.ToolStrips.Remove(this);
            }
            finally
            {
                ResumeLayout(false);
                overflow?.ResumeLayout(false);

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

    internal ToolStripOverflow? GetOverflow()
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

    internal virtual ToolStripItem? GetNextItem(ToolStripItem? start, ArrowDirection direction, bool rtlAware)
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
    public virtual ToolStripItem? GetNextItem(ToolStripItem? start, ArrowDirection direction)
    {
        switch (direction)
        {
            case ArrowDirection.Right:
                return GetNextItemHorizontal(start, forward: true);
            case ArrowDirection.Left:
                bool forward = LastKeyData == Keys.Tab || (TabStop && start is null && LastKeyData != Keys.Left);
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
    ///  <para>Helper function for GetNextItem - do not directly call this.</para>
    /// </remarks>
    private ToolStripItem? GetNextItemHorizontal(ToolStripItem? start, bool forward)
    {
        if (DisplayedItems.Count <= 0)
        {
            return null;
        }

        ToolStripDropDown? dropDown = this as ToolStripDropDown;

        // The navigation should be consistent when navigating in forward and
        // backward direction entering the toolstrip, it means that the first
        // toolstrip item should be selected irrespectively TAB or SHIFT+TAB
        // is pressed.
        start ??= forward ? DisplayedItems[DisplayedItems.Count - 1] : DisplayedItems[0];

        int current = DisplayedItems.IndexOf(start);
        if (current == -1)
        {
            return null;
        }

        int count = DisplayedItems.Count;

        do
        {
            if (forward)
            {
                current = ++current % count;
            }
            else
            {
                // Provide negative wrap if necessary.
                current = (--current < 0) ? count + current : current;
            }

            if (dropDown?.OwnerItem is not null && dropDown.OwnerItem.IsInDesignMode)
            {
                return DisplayedItems[current];
            }

            if (DisplayedItems[current].CanKeyboardSelect)
            {
                return DisplayedItems[current];
            }
        }
        while (DisplayedItems[current] != start);

        return null;
    }

    /// <remarks>
    ///  <para>Helper function for GetNextItem - do not directly call this.</para>
    /// </remarks>
    private ToolStripItem? GetNextItemVertical(ToolStripItem? selectedItem, bool down)
    {
        ToolStripItem? tanWinner = null;
        ToolStripItem? hypotenuseWinner = null;

        double minHypotenuse = double.MaxValue;
        double minTan = double.MaxValue;
        double tanOfHypotenuseWinner = double.MaxValue;

        if (selectedItem is null)
        {
            return GetNextItemHorizontal(selectedItem, down);
        }

        if (this is ToolStripDropDown dropDown && dropDown.OwnerItem is not null && (dropDown.OwnerItem.IsInDesignMode || (dropDown.OwnerItem.Owner is not null && dropDown.OwnerItem.Owner.IsInDesignMode)))
        {
            return GetNextItemHorizontal(selectedItem, down);
        }

        Point midPointOfCurrent = new(selectedItem.Bounds.X + selectedItem.Width / 2,
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

            // [ otherControl ]
            //       *
            Point otherItemMidLocation = new(otherItem.Bounds.X + otherItem.Width / 2, (down) ? otherItem.Bounds.Top : otherItem.Bounds.Bottom);
            int oppositeSide = otherItemMidLocation.X - midPointOfCurrent.X;
            int adjacentSide = otherItemMidLocation.Y - midPointOfCurrent.Y;

            // use pythagorean theorem to calculate the length of the distance
            // between the middle of the current control in question and it's adjacent
            // objects.
            double hypotenuse = Math.Sqrt(adjacentSide * adjacentSide + oppositeSide * oppositeSide);

            if (adjacentSide != 0)
            {
                // avoid divide by zero - we don't do layered controls
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
            // the case where the hypotenuse is smaller. The only
            // scenarios where that is not the case is when the hypotenuse
            // winner is clearly closer than the angle winner.

            // [a.winner]                       |       [s]
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
        ToolStrip toolStrip = (ToolStrip)container;

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
            maxSize.Width += 2;  // add Padding of 2 Pixels to the right if not Overflow.
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

    internal static Size GetPreferredSizeVertical(IArrangedElement container)
    {
        Size maxSize = Size.Empty;
        bool requiresOverflow = false;
        ToolStrip toolStrip = (ToolStrip)container;

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
            maxSize.Height += 2;  // add Padding to the bottom if not Overflow.
        }

        if (toolStrip.GripStyle == ToolStripGripStyle.Visible)
        {
            // add in the grip width
            Padding gripMargin = toolStrip.GripMargin;
            maxSize.Height += gripMargin.Vertical + toolStrip.Grip.GripThickness;
        }

        // note here the difference in vertical - we want the strings to fit perfectly
        // so we're not going to constrain by the specified size.
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

    internal ToolStripItem? GetSelectedItem()
    {
        ToolStripItem? selectedItem = null;

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

    internal virtual ToolStrip? GetToplevelOwnerToolStrip()
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
        // If we had a particular item that was "entered" notify it that we have left.
        if (_lastMouseActiveItem is not null)
        {
            if (!DesignMode)
            {
                MouseHoverTimer.Cancel(_lastMouseActiveItem);
            }

            try
            {
                _lastMouseActiveItem.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseLeave);
            }
            finally
            {
                _lastMouseActiveItem = null;
            }
        }

        ToolStripMenuItem.MenuTimer.HandleToolStripMouseLeave(this);
    }

    internal void HandleItemClick(ToolStripItem dismissingItem)
    {
        ToolStripItemClickedEventArgs e = new(dismissingItem);
        OnItemClicked(e);
        // Ensure both the overflow and the main toolstrip fire ItemClick event
        // otherwise the overflow won't dismiss.
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
                    ToolStripManager.RendererChanged += OnDefaultRendererChanged;
                    SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
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
                ToolStripManager.RendererChanged -= OnDefaultRendererChanged;
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            }
            finally
            {
                _alreadyHooked = false;
            }
        }
    }

    // initialize ToolStrip
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
        using (new LayoutTransaction(this, this, "ShowKeyboardFocusCues", resumeLayout: Visible))
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

    protected override bool IsInputKey(Keys keyData) =>
        (GetSelectedItem() is { } item && item.IsInputKey(keyData)) || base.IsInputKey(keyData);

    protected override bool IsInputChar(char charCode) =>
        (GetSelectedItem() is { } item && item.IsInputChar(charCode)) || base.IsInputChar(charCode);

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
    ///  Gets or sets the <see cref="Forms.ImageList"/> that contains the <see cref="Image"/> displayed on a label control
    /// </summary>
    private void ImageListRecreateHandle(object? sender, EventArgs e)
    {
        Invalidate();
    }

    /// <summary>
    ///  This override fires the LocationChanging event if
    ///  1) We are not currently Rafting .. since this cause this infinite times...
    ///  2) If we haven't been called once .. Since the "LocationChanging" is listened to by the RaftingCell and
    ///  calls "JOIN" which may call us back.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        Point location = Location;

        if (!IsCurrentlyDragging && !IsLocationChanging && IsInToolStripPanel)
        {
            ToolStripLocationCancelEventArgs cae = new(new Point(x, y), false);
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
                if (location.X != x || location.Y != y)
                {
                    using Region? transparentRegion = Renderer.GetTransparentRegion(this);
                    if (transparentRegion is not null)
                    {
                        Invalidate(transparentRegion);
                        Update();
                    }
                }
            }

            SetToolStripState(STATE_LOCATIONCHANGING, false);
            base.SetBoundsCore(x, y, width, height, specified);
        }
    }

    internal bool ProcessCmdKeyInternal(ref Message m, Keys keyData)
    {
        return ProcessCmdKey(ref m, keyData);
    }

    // This function will print to the PrinterDC. ToolStrip have there own buffered painting and doesn't play very well
    // with the DC translations done by base Control class. Hence we do our own Painting and the BitBLT the DC into the printerDc.
    private protected override void PrintToMetaFileRecursive(HDC hDC, IntPtr lParam, Rectangle bounds)
    {
        using Bitmap image = new(bounds.Width, bounds.Height);
        using Graphics g = Graphics.FromImage(image);
        using DeviceContextHdcScope imageHdc = new(g, applyGraphicsState: false);

        // Send the actual wm_print message
        PInvokeCore.SendMessage(
            this,
            PInvokeCore.WM_PRINT,
            (WPARAM)imageHdc,
            (LPARAM)(uint)(PInvoke.PRF_CHILDREN | PInvoke.PRF_CLIENT | PInvoke.PRF_ERASEBKGND | PInvoke.PRF_NONCLIENT));

        // Now BLT the result to the destination bitmap.
        PInvokeCore.BitBlt(
            hDC,
            bounds.X,
            bounds.Y,
            bounds.Width,
            bounds.Height,
            imageHdc,
            0,
            0,
            ROP_CODE.SRCCOPY);
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (ToolStripManager.IsMenuKey(keyData))
        {
            if (!IsDropDown && ToolStripManager.ModalMenuFilter.InMenuMode)
            {
                ClearAllSelections();
                ToolStripManager.ModalMenuFilter.MenuKeyToggle = true;
                ToolStripManager.ModalMenuFilter.ExitMenuMode();
            }
        }

        // Give the ToolStripItem very first chance at
        // processing keys (except for ALT handling)
        ToolStripItem? selectedItem = GetSelectedItem();
        if (selectedItem is not null)
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

            if (isControlTab
                && !TabStop
                && HasKeyboardInput
                && ToolStripManager.SelectNextToolStrip(this, forward: (keyData & Keys.Shift) == Keys.None))
            {
                // Handled
                return true;
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
        ToolStripItem? item = GetSelectedItem();
        if (item is not null)
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

        Keys keyCode = keyData & Keys.KeyCode;
        switch (keyCode)
        {
            case Keys.Back:
                // if it's focused itself, process. if it's not focused, make sure a child control
                // doesn't have focus before processing
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
                SelectNextToolStripItem(start: null, forward: true);
                retVal = true;
                break;
            case Keys.End:
                SelectNextToolStripItem(start: null, forward: false);
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

        return retVal ? retVal : base.ProcessDialogKey(keyData);
    }

    internal virtual void ProcessDuplicateMnemonic(ToolStripItem item, char charCode)
    {
        if (!CanProcessMnemonic())
        {
            // Checking again for security.
            return;
        }

        if (item is not null)
        {
            SetFocusUnsafe();
            item.Select();
        }
    }

    /// <summary>
    ///  Rules for parsing mnemonics
    ///  PASS 1: Real mnemonics
    ///  Check items for the character after the &amp;. If it matches, perform the click event or open the dropdown
    ///  (in the case that it has dropdown items)
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
        if (!inMenuMode && ModifierKeys == Keys.Alt)
        {
            // This is the case where someone hasnt released the ALT key yet, but has pushed another letter.
            // In some cases we can activate the menu that is not the MainMenuStrip...
            return ProcessMnemonicInternal(charCode);
        }
        else if (inMenuMode && ToolStripManager.ModalMenuFilter.GetActiveToolStrip() == this)
        {
            return ProcessMnemonicInternal(charCode);
        }

        // do not call base, as we don't want to walk through the controls collection and reprocess everything
        // we should have processed in the displayed items collection.
        return false;
    }

    private bool ProcessMnemonicInternal(char charCode)
    {
        if (!CanProcessMnemonic())
        {
            // Checking again for security.
            return false;
        }

        // at this point we assume we can process mnemonics as process mnemonic has filtered for use.
        ToolStripItem? startingItem = GetSelectedItem();
        int startIndex = 0;
        if (startingItem is not null)
        {
            startIndex = DisplayedItems.IndexOf(startingItem);
        }

        startIndex = Math.Max(0, startIndex);

        ToolStripItem? firstMatch = null;
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

            if (IsMnemonic(charCode, currentItem.Text))
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
        if (firstMatch is not null)
        {
            return firstMatch.ProcessMnemonic(charCode);
        }

        if (!foundMenuItem)
        {
            return false;
        }

        index = startIndex;

        // MenuStrip parity: key presses should change selection if mnemonic not present
        // if we haven't found a mnemonic, cycle through the menu items and
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

            if (IsPseudoMnemonic(charCode, currentItem.Text))
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

        if (firstMatch is not null)
        {
            return firstMatch.ProcessMnemonic(charCode);
        }

        // do not call base, as we don't want to walk through the controls collection and reprocess everything
        // we should have processed in the displayed items collection.
        return false;
    }

    private bool ProcessTabKey(bool forward)
    {
        if (TabStop)
        {
            // ToolBar in tab-order parity
            //  this means we want the toolstrip in the normal tab order - which means it shouldn't wrap.
            //  First tab gets you into the toolstrip, second tab moves you on your way outside the container.
            //  arrow keys would continue to wrap.
            return false;
        }
        else
        {
            // TabStop = false
            // this means we don't want the toolstrip in the normal tab order (default).
            // We got focus to the toolstrip by putting focus into a control contained on the toolstrip or
            // via a mnemonic e.g. Bold. In this case we want to wrap.
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
                    ToolStripItem? currentSel = GetSelectedItem();
                    if (keyCode == Keys.Down)
                    {
                        ToolStripItem? nextItem = GetNextItem(currentSel, ArrowDirection.Down);
                        if (nextItem is not null)
                        {
                            ChangeSelection(nextItem);
                            retVal = true;
                        }
                    }
                    else
                    {
                        ToolStripItem? nextItem = GetNextItem(currentSel, ArrowDirection.Up);
                        if (nextItem is not null)
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
        SelectNextToolStripItem(GetSelectedItem(), right);
        return true;
    }

    internal void NotifySelectionChange(ToolStripItem? item)
    {
        if (item is null)
        {
            ClearAllSelections();
        }
        else if (item.Selected)
        {
            ClearAllSelectionsExcept(item);
        }
    }

    private void OnDefaultRendererChanged(object? sender, EventArgs e)
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
        UpdateToolTip(item: null); // suppress the tooltip.
        ((EventHandler?)Events[s_eventBeginDrag])?.Invoke(this, e);
    }

    protected virtual void OnEndDrag(EventArgs e)
    {
        SetToolStripState(STATE_DRAGGING, false);
        ((EventHandler?)Events[s_eventEndDrag])?.Invoke(this, e);
    }

    protected override void OnDockChanged(EventArgs e)
    {
        base.OnDockChanged(e);
    }

    protected virtual void OnRendererChanged(EventArgs e)
    {
        InitializeRenderer(Renderer);

        ((EventHandler?)Events[s_eventRendererChanged])?.Invoke(this, e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);

        // notify items that the parent has changed
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] is not null && Items[i].ParentInternal == this)
            {
                Items[i].OnParentEnabledChanged(e);
            }
        }
    }

    internal void OnDefaultFontChanged()
    {
        _defaultFont = null;
        if (ScaleHelper.IsThreadPerMonitorV2Aware)
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

    protected override void OnHandleCreated(EventArgs e)
    {
        if ((AllowDrop || AllowItemReorder) && (DropTargetManager is not null))
        {
            DropTargetManager.EnsureRegistered();
        }

        // calling control's (in base) version AFTER we register our DropTarget, so it will
        // listen to us instead of control's implementation
        base.OnHandleCreated(e);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        // Make sure we unregister ourselves as a drop target
        DropTargetManager?.EnsureUnRegistered();

        base.OnHandleDestroyed(e);
    }

    protected internal virtual void OnItemAdded(ToolStripItemEventArgs e)
    {
        DoLayoutIfHandleCreated(e);

        if (!HasVisibleItems && e.Item is not null && ((IArrangedElement)e.Item).ParticipatesInLayout)
        {
            // in certain cases, we may not have laid out yet (e.g. a dropdown may not layout until
            // it becomes visible.)   We will recalculate this in SetDisplayedItems, but for the moment
            // if we find an item that ParticipatesInLayout, mark us as having visible items.
            HasVisibleItems = true;
        }

        ((ToolStripItemEventHandler?)Events[s_eventItemAdded])?.Invoke(this, e);
    }

    /// <summary>
    ///  Called when an item has been clicked on the ToolStrip.
    /// </summary>
    protected virtual void OnItemClicked(ToolStripItemClickedEventArgs e)
    {
        ((ToolStripItemClickedEventHandler?)Events[s_eventItemClicked])?.Invoke(this, e);
    }

    protected internal virtual void OnItemRemoved(ToolStripItemEventArgs e)
    {
        // clear cached item states.
        OnItemVisibleChanged(e, performLayout: true);

        ((ToolStripItemEventHandler?)Events[s_eventItemRemoved])?.Invoke(this, e);
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
            UpdateToolTip(item: null);
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
        ToolStripOverflow? overflow = GetOverflow();
        if (overflow is not null)
        {
            overflow.SuspendLayout();
            _toolStripOverflowButton!.Size = _toolStripOverflowButton.GetPreferredSize(DisplayRectangle.Size - Padding.Size);
        }

        for (int j = 0; j < Items.Count; j++)
        {
            Items[j].OnLayout(e);
        }

        base.OnLayout(e);
        SetDisplayedItems();
        OnLayoutCompleted(EventArgs.Empty);
        Invalidate();

        overflow?.ResumeLayout();
    }

    protected virtual void OnLayoutCompleted(EventArgs e)
    {
        ((EventHandler?)Events[s_eventLayoutCompleted])?.Invoke(this, e);
    }

    protected virtual void OnLayoutStyleChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_eventLayoutStyleChanged])?.Invoke(this, e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        ClearAllSelections();
    }

    protected internal override void OnLeave(EventArgs e)
    {
        base.OnLeave(e);
        if (!IsDropDown)
        {
            Application.ThreadContext.FromCurrent().RemoveMessageFilter(RestoreFocusFilter);
        }
    }

    internal virtual void OnLocationChanging(ToolStripLocationCancelEventArgs e)
    {
        ((ToolStripLocationCancelEventHandler?)Events[s_eventLocationChanging])?.Invoke(this, e);
    }

    /// <summary>
    ///  Delegate mouse down to the ToolStrip and its affected items
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs mea)
    {
        // NEVER use this directly from another class. Always use GetMouseID so that
        // 0 is not returned to another class.
        _mouseDownID++;

        ToolStripItem? item = GetItemAt(mea.X, mea.Y);
        if (item is not null)
        {
            if (!IsDropDown && (!(item is ToolStripDropDownItem)))
            {
                // set capture only when we know we're not on a dropdown (already effectively have capture due to modal menufilter)
                // and the item in question requires the mouse to be in the same item to be clicked.
                SetToolStripState(STATE_LASTMOUSEDOWNEDITEMCAPTURE, true);
                Capture = true;
            }

            MenuAutoExpand = true;

            if (mea is not null)
            {
                // Transpose this to "client coordinates" of the ToolStripItem.
                Point itemRelativePoint = item.TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripCoords, ToolStripPointType.ToolStripItemCoords);
                mea = new MouseEventArgs(mea.Button, mea.Clicks, itemRelativePoint.X, itemRelativePoint.Y, mea.Delta);
            }

            _lastMouseDownedItem = item;
            item.FireEvent(mea!, ToolStripItemEventType.MouseDown);
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
        ToolStripItem? item = GetItemAt(mea.X, mea.Y);

        if (!Grip.MovingToolStrip)
        {
            // If we had a particular item that was "entered"
            // notify it that we have entered. It's fair to put
            // this in the MouseMove event, as MouseEnter is fired during
            // control's WM_MOUSEMOVE. Waiting until this event gives us
            // the actual coordinates.

            if (item != _lastMouseActiveItem)
            {
                // Notify the item that we've moved on.
                HandleMouseLeave();

                // Track only items that don't get mouse events themselves.
                _lastMouseActiveItem = (item is ToolStripControlHost) ? null : item;

                if (_lastMouseActiveItem is not null)
                {
                    item!.FireEvent(EventArgs.Empty, ToolStripItemEventType.MouseEnter);
                }

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

        if (item is not null)
        {
            // Fire mouse move on the item
            // Transpose this to "client coordinates" of the ToolStripItem.
            Point itemRelativePoint = item!.TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripCoords, ToolStripPointType.ToolStripItemCoords);
            mea = new MouseEventArgs(mea.Button, mea.Clicks, itemRelativePoint.X, itemRelativePoint.Y, mea.Delta);
            item.FireEvent(mea, ToolStripItemEventType.MouseMove);
        }
        else
        {
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
            // while we're showing a feedback rect, don't cancel moving the toolstrip.
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
        ToolStripItem? item = Grip.MovingToolStrip ? Grip : GetItemAt(mea.X, mea.Y);

        if (item is not null)
        {
            if (mea is not null)
            {
                // Transpose this to "client coordinates" of the ToolStripItem.
                Point itemRelativePoint = item.TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripCoords, ToolStripPointType.ToolStripItemCoords);
                mea = new MouseEventArgs(mea.Button, mea.Clicks, itemRelativePoint.X, itemRelativePoint.Y, mea.Delta);
            }

            item.FireEvent(mea!, ToolStripItemEventType.MouseUp);
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
        using Region? transparentRegion = Renderer.GetTransparentRegion(this);

        // Paint the items
        //
        // The idea here is to let items pretend they are controls. They should get paint events at 0,0 and have
        // proper clipping regions set up for them. We cannot use g.TranslateTransform as that does not translate
        // the GDI world, and things like Visual Styles and the TextRenderer only know how to speak GDI.
        //
        // The previous approach was to set up the GDI clipping region and allocate a graphics from that, but that
        // meant we were allocating graphics objects left and right, which turned out to be slow.
        //
        // So now we allocate an offscreen bitmap of size == MaxItemSize, copy the background of the toolstrip into
        // that bitmap, paint the item on top of the bitmap, then finally copy the contents of the bitmap back onto
        // the toolstrip. This gives us our paint event starting at 0,0. Combine this with double buffering of the
        // toolstrip and the entire toolstrip is updated after returning from this function.

        if (!LayoutUtils.IsZeroWidthOrHeight(bitmapSize))
        {
            // can't create a 0x0 bmp.

            // Supporting RoundedEdges...
            // we've got a concept of a region that we shouldn't paint (the TransparentRegion as specified in the Renderer).
            // in order to support this we're going to intersect that region with the clipping region.
            // this new region will be excluded during the guts of OnPaint, and restored at the end of OnPaint.
            if (transparentRegion is not null)
            {
                // only use the intersection so we can easily add back in the bits we took out at the end.
                transparentRegion.Intersect(toolstripGraphics.Clip);
                toolstripGraphics.ExcludeClip(transparentRegion);
                excludedTransparentRegion = true;
            }

            // Preparing for painting the individual items...
            // using WindowsGraphics here because we want to preserve the clipping information.

            // calling GetHdc by itself does not set up the clipping info.
            using DeviceContextHdcScope toolStripHDC = new(toolstripGraphics, ApplyGraphicsProperties.Clipping);

            // Get the cached item HDC.
            HDC itemHDC = ItemHdcInfo.GetCachedItemDC(toolStripHDC, bitmapSize);

            Graphics itemGraphics = itemHDC.CreateGraphics();
            try
            {
                // Iterate through all the items, painting them one by one into the compatible offscreen DC,
                // and then copy them back onto the main toolstrip.
                for (int i = 0; i < DisplayedItems.Count; i++)
                {
                    ToolStripItem item = DisplayedItems[i];
                    if (item is not null)
                    {
                        Rectangle clippingRect = e.ClipRectangle;
                        Rectangle bounds = item.Bounds;

                        if (!IsDropDown && item.Owner == this)
                        {
                            // owned items should not paint outside the client
                            // area. (this is mainly to prevent obscuring the grip
                            // and overflowbutton - ToolStripDropDownMenu places items
                            // outside of the display rectangle - so we need to allow for this
                            // in dropdowns).
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
                            // the cached HDC isn't big enough for this item. make it bigger.
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
                        PInvokeCore.BitBlt(
                            ItemHdcInfo,
                            0,
                            0,
                            item.Size.Width,
                            item.Size.Height,
                            toolStripHDC,
                            item.Bounds.X,
                            item.Bounds.Y,
                            ROP_CODE.SRCCOPY);

                        // Paint the item into the offscreen bitmap
                        using (PaintEventArgs itemPaintEventArgs = new(itemGraphics, clippingRect))
                        {
                            item.FireEvent(itemPaintEventArgs, ToolStripItemEventType.Paint);
                        }

                        // copy the item back onto the toolstrip
                        PInvokeCore.BitBlt(
                            toolStripHDC,
                            item.Bounds.X,
                            item.Bounds.Y,
                            item.Size.Width,
                            item.Size.Height,
                            ItemHdcInfo,
                            0,
                            0,
                            ROP_CODE.SRCCOPY);

                        GC.KeepAlive(ItemHdcInfo);
                    }
                }
            }
            finally
            {
                itemGraphics.Dispose();
            }
        }

        // Painting the edge effects...
        // These would include things like (shadow line on the bottom, some overflow effects)
        Renderer.DrawToolStripBorder(new ToolStripRenderEventArgs(toolstripGraphics, this));

        // Restoring the clip region to its original state...
        // the transparent region should be added back in as the insertion mark should paint over it.
        if (excludedTransparentRegion)
        {
            toolstripGraphics.SetClip(transparentRegion!, CombineMode.Union);
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

            _toolStripOverflowButton?.OnParentRightToLeftChanged(e);

            _toolStripGrip?.OnParentRightToLeftChanged(e);
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
            using (Region? transparentRegion = Renderer.GetTransparentRegion(this))
            {
                if (transparentRegion is not null)
                {
                    EraseCorners(e, transparentRegion);
                    g.ExcludeClip(transparentRegion);
                }
            }

            Renderer.DrawToolStripBackground(new ToolStripRenderEventArgs(g, this));
        }
        finally
        {
            if (graphicsState is not null)
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
        if (transparentRegion is not null)
        {
            PaintTransparentBackground(e, ClientRectangle, transparentRegion);
        }
    }

    protected internal virtual void OnPaintGrip(PaintEventArgs e)
    {
        Renderer.DrawGrip(new ToolStripGripRenderEventArgs(e.Graphics, this));

        ((PaintEventHandler?)Events[s_eventPaintGrip])?.Invoke(this, e);
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
        if (ScaleHelper.IsThreadPerMonitorV2Aware)
        {
            if (deviceDpiOld != deviceDpiNew)
            {
                ToolStripManager.CurrentDpi = deviceDpiNew;
                _defaultFont = ToolStripManager.DefaultFont;

                // We need to take care of this control.
                ResetScaling(deviceDpiNew);

                // We need to scale the one Grip per ToolStrip as well (if present).
                _toolStripGrip?.ToolStrip_RescaleConstants(deviceDpiOld, deviceDpiNew);

                // ToolStripItems are components and have Font property. Components do not receive WM_DPICHANGED messages, nor they have
                // parent-child relationship with owners and, thus, do not get scaled by parent/Container. For these reasons, they need the font
                // to be explicitly updated when Dpi changes (only if the font was set explicitly).
                float factor = (float)deviceDpiNew / deviceDpiOld;
                foreach (ToolStripItem item in Items)
                {
                    if (item.TryGetExplicitlySetFont(out Font? local))
                    {
                        item.Font = local.WithSize(local.Size * factor);
                    }
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
        s_iconSize = ScaleHelper.ScaleToDpi(LogicalIconSize, newDpi);
        s_insertionBeamWidth = ScaleHelper.ScaleToDpi(LogicalInsertionBeamWidth, newDpi);
        _defaultPadding = ScaleHelper.ScaleToDpi(s_logicalDefaultPadding, newDpi);
        _defaultGripMargin = ScaleHelper.ScaleToDpi(s_logicalDefaultGripMargin, newDpi);
        _imageScalingSize = new Size(s_iconSize, s_iconSize);
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

                // Draw vertical lines.
                g.DrawLines(SystemPens.ControlText, (ReadOnlySpan<Point>)
                    [
                        new(verticalBeamStart, _lastInsertionMarkRect.Y), new(verticalBeamStart, _lastInsertionMarkRect.Bottom - 1),
                        new(verticalBeamStart + 1, _lastInsertionMarkRect.Y), new(verticalBeamStart + 1, _lastInsertionMarkRect.Bottom - 1)
                    ]);

                // Draw top horizontal lines.
                g.DrawLines(SystemPens.ControlText, (ReadOnlySpan<Point>)
                    [
                        new(start, _lastInsertionMarkRect.Bottom - 1), new(start + widthOfBeam - 1, _lastInsertionMarkRect.Bottom - 1),
                        new(start + 1, _lastInsertionMarkRect.Bottom - 2), new(start + widthOfBeam - 2, _lastInsertionMarkRect.Bottom - 2)
                    ]);

                // Draw bottom horizontal lines.
                g.DrawLines(SystemPens.ControlText, (ReadOnlySpan<Point>)
                     [
                         new(start, _lastInsertionMarkRect.Y), new(start + widthOfBeam - 1, _lastInsertionMarkRect.Y),
                         new(start + 1, _lastInsertionMarkRect.Y + 1), new(start + widthOfBeam - 2, _lastInsertionMarkRect.Y + 1)
                     ]);
            }
            else
            {
                widthOfBeam = s_insertionBeamWidth;
                int start = _lastInsertionMarkRect.Y;
                int horizontalBeamStart = start + 2;

                // Draw horizontal lines.
                g.DrawLines(SystemPens.ControlText, (ReadOnlySpan<Point>)
                    [
                        new(_lastInsertionMarkRect.X, horizontalBeamStart), new(_lastInsertionMarkRect.Right - 1, horizontalBeamStart),
                        new(_lastInsertionMarkRect.X, horizontalBeamStart + 1), new(_lastInsertionMarkRect.Right - 1, horizontalBeamStart + 1)
                    ]);

                // Draw left vertical lines.
                g.DrawLines(SystemPens.ControlText, (ReadOnlySpan<Point>)
                    [
                        new(_lastInsertionMarkRect.X, start), new(_lastInsertionMarkRect.X, start + widthOfBeam - 1),
                        new(_lastInsertionMarkRect.X + 1, start + 1), new(_lastInsertionMarkRect.X + 1, start + widthOfBeam - 2)
                    ]);

                // Draw right vertical lines.
                g.DrawLines(SystemPens.ControlText, (ReadOnlySpan<Point>)
                     [
                         new(_lastInsertionMarkRect.Right - 1, start), new(_lastInsertionMarkRect.Right - 1, start + widthOfBeam - 1),
                         new(_lastInsertionMarkRect.Right - 2, start + 1), new(_lastInsertionMarkRect.Right - 2, start + widthOfBeam - 2)
                     ]);
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
    public new Control? GetChildAtPoint(Point point) => base.GetChildAtPoint(point);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Control? GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue) => base.GetChildAtPoint(pt, skipValue);

    // GetNextControl for ToolStrip should always return null
    // we do our own tabbing/etc - this allows us to pretend
    // we don't have child controls.
    internal override Control? GetFirstChildControlInTabOrder(bool forward) => null;

    /// <summary>
    ///  Finds the ToolStripItem contained within a specified client coordinate point
    ///  If item not found - returns null
    /// </summary>
    public ToolStripItem? GetItemAt(int x, int y) => GetItemAt(new Point(x, y));

    /// <summary>
    ///  Finds the ToolStripItem contained within a specified client coordinate point
    ///  If item not found - returns null
    /// </summary>
    public ToolStripItem? GetItemAt(Point point)
    {
        Rectangle comparisonRect = new(point, s_onePixel);
        Rectangle bounds;

        // Check the last item we had the mouse over
        if (_lastMouseActiveItem is not null)
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
            if (_toolStripGrip is not null && DisplayedItems[i] == _toolStripGrip)
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

    internal override void ReleaseUiaProvider(HWND handle)
    {
        if (!IsAccessibilityObjectCreated)
        {
            return;
        }

        if (OsVersion.IsWindows8OrGreater())
        {
            ReleaseToolStripItemsProviders(Items);

            _toolStripGrip?.ReleaseUiaProvider();
            _toolStripOverflowButton?.ReleaseUiaProvider();
        }

        base.ReleaseUiaProvider(handle);
    }

    /// <summary>
    ///  Call UiaDisconnectProvider for all ToolsStripItem Accessible objects.
    ///  This method is invoked from ReleaseUiaProvider method.
    /// </summary>
    /// <param name="items">contains ToolStrip or ToolStripDropDown items to disconnect</param>
    internal virtual void ReleaseToolStripItemsProviders(ToolStripItemCollection items)
    {
        ToolStripItem[] itemsArray = [..items.Cast<ToolStripItem>()];
        foreach (ToolStripItem toolStripItem in itemsArray)
        {
            if (toolStripItem is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
            {
                ToolStripItemCollection dropDownMenuItems = dropDownItem.DropDownItems;
                ReleaseToolStripItemsProviders(dropDownMenuItems);
            }

            toolStripItem.ReleaseUiaProvider();
        }
    }

    private void RestoreFocusInternal(bool wasInMenuMode)
    {
        // This is called from the RestoreFocusFilter. If the state of MenuMode has changed
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

        ToolStripManager.ModalMenuFilter.ExitMenuMode();

        if (!IsDropDown)
        {
            // Reset menu auto expansion.
            Application.ThreadContext.FromCurrent().RemoveMessageFilter(RestoreFocusFilter);
            MenuAutoExpand = false;

            if (!DesignMode && !TabStop && (Focused || ContainsFocus))
            {
                RestoreFocus();
            }
        }

        // this matches the case where you click on a toolstrip control host
        // then tab off of it, then hit ESC. ESC would "restore focus" and
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

        if (!_hwndThatLostFocus.IsNull && (_hwndThatLostFocus != Handle))
        {
            Control? control = FromHandle(_hwndThatLostFocus);
            _hwndThatLostFocus = default;

            if ((control is not null) && control.Visible)
            {
                focusSuccess = control.Focus();
            }
        }

        _hwndThatLostFocus = default;

        if (!focusSuccess)
        {
            // Clear out the focus, we have focus, we're not supposed to anymore.
            PInvoke.SetFocus(default);
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

    internal void ResumeCaptureMode()
    {
        SetToolStripState(STATE_SUSPENDCAPTURE, false);
    }

    internal void SuspendCaptureMode()
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
        ArgumentNullException.ThrowIfNull(item);

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
            CachedItemHdcInfo? lastInfo = _cachedItemHdcInfo;
            _cachedItemHdcInfo = null;

            _lastMouseDownedItem = null;

            lastInfo?.Dispose();
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
            return true;
        }

        Point mousePosition = WindowsFormsUtils.LastCursorPoint;
        if (_mouseEnterWhenShown != mousePosition)
        {
            _mouseEnterWhenShown = s_invalidMouseEnter;
            return true;
        }

        return false;
    }

    protected override void Select(bool directed, bool forward)
    {
        bool correctParentActiveControl = true;
        if (ParentInternal is not null)
        {
            IContainerControl? c = ParentInternal.GetContainerControl();

            if (c is not null)
            {
                c.ActiveControl = this;
                correctParentActiveControl = (c.ActiveControl == this);
            }
        }

        if (directed && correctParentActiveControl)
        {
            SelectNextToolStripItem(start: null, forward);
        }
    }

    internal ToolStripItem? SelectNextToolStripItem(ToolStripItem? start, bool forward)
    {
        ToolStripItem? nextItem = GetNextItem(start, forward ? ArrowDirection.Right : ArrowDirection.Left, rtlAware: true);
        ChangeSelection(nextItem);
        return nextItem;
    }

    internal void SetFocusUnsafe()
    {
        if (TabStop)
        {
            Focus();
        }
        else
        {
            ToolStripManager.ModalMenuFilter.SetActiveToolStrip(this, menuKeyPressed: false);
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
    public new void SetAutoScrollMargin(int x, int y)
    {
        base.SetAutoScrollMargin(x, y);
    }

    internal void SetLargestItemSize(Size size)
    {
        if (_toolStripOverflowButton is not null && _toolStripOverflowButton.Visible)
        {
            size = LayoutUtils.UnionSizes(size, _toolStripOverflowButton.Bounds.Size);
        }

        if (_toolStripGrip is not null && _toolStripGrip.Visible)
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
            if (GripStyle == ToolStripGripStyle.Visible)
            {
                DisplayedItems.Add(Grip);
                SetupGrip();
            }

            // For splitstack layout we re-arrange the items in the displayed items
            // collection so that we can easily tab through them in natural order

            // We've historically called this virtual, still need to for compat.
            _ = DisplayRectangle;
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
                                    // stash away this index so we don't have to iterate through the whole list again.
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

            ToolStripOverflow? overflow = GetOverflow();
            if (overflow is not null)
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
            // NOT a SplitStack layout. We don't change the order of the displayed items collection
            // for custom keyboard handling override GetNextItem.
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

                    bool verticallyContained = clientBounds.Contains(clientBounds.X, item.Bounds.Top)
                        && clientBounds.Contains(clientBounds.X, item.Bounds.Bottom);

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
    private void SnapFocus(HWND otherHwnd)
    {
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
                if ((Handle != otherHwnd) && !PInvoke.IsChild(this, otherHwnd))
                {
                    // make sure the root window of the otherHwnd is the same as
                    // the root window of thisHwnd.
                    HWND thisHwndRoot = PInvoke.GetAncestor(this, GET_ANCESTOR_FLAGS.GA_ROOT);
                    HWND otherHwndRoot = PInvoke.GetAncestor(otherHwnd, GET_ANCESTOR_FLAGS.GA_ROOT);

                    if (thisHwndRoot == otherHwndRoot && !thisHwndRoot.IsNull)
                    {
                        // We know we're in the same window heirarchy.
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
        Size invalidDefaultSize = new(-1, -1);
        return (CommonProperties.GetMinimumSize(this, invalidDefaultSize) != invalidDefaultSize);
    }

    private bool ShouldSerializeGripMargin()
    {
        return GripMargin != DefaultGripMargin;
    }

    internal virtual bool ShouldSerializeRenderMode()
    {
        // We should NEVER serialize custom.
        return (RenderMode is not ToolStripRenderMode.ManagerRenderMode and not ToolStripRenderMode.Custom);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Name: {Name}, Items: {Items.Count}";
    }

    /// <summary>
    ///  Updates a tooltip for the given toolstrip item.
    /// </summary>
    /// <param name="item">The toolstrip item.</param>
    /// <param name="refresh">
    ///  see langword="true"/> to force-update the tooltip (if it is configured); otherwise <see langword="false"/>.
    /// </param>
    internal void UpdateToolTip(ToolStripItem? item, bool refresh = false)
    {
        if (!ShowItemToolTips || (item == _currentlyActiveTooltipItem && !refresh) || ToolTip is null)
        {
            return;
        }

        if (item != _currentlyActiveTooltipItem)
        {
            ToolTip.Hide(this);
            if (!refresh)
            {
                _currentlyActiveTooltipItem = item;
            }
        }

        if (_currentlyActiveTooltipItem is not null && !GetToolStripState(STATE_DRAGGING) && Cursor.Current is { } currentCursor)
        {
            Point cursorLocation = Cursor.Position;
            cursorLocation.Y += Cursor.Size.Height - currentCursor.HotSpot.Y;

            cursorLocation = WindowsFormsUtils.ConstrainToScreenBounds(new Rectangle(cursorLocation, s_onePixel)).Location;

            ToolTip.Show(
                _currentlyActiveTooltipItem.ToolTipText,
                this,
                PointToClient(cursorLocation),
                ToolTip.AutoPopDelay);
        }
    }

    private void UpdateLayoutStyle(DockStyle newDock)
    {
        if (!IsInToolStripPanel && _layoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow && _layoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow)
        {
            using (new LayoutTransaction(this, this, PropertyNames.Orientation))
            {
                // We want the ToolStrip to size appropriately when the dock has switched.
                if (newDock is DockStyle.Left or DockStyle.Right)
                {
                    UpdateOrientation(Orientation.Vertical);
                }
                else
                {
                    UpdateOrientation(Orientation.Horizontal);
                }
            }

            OnLayoutStyleChanged(EventArgs.Empty);

            if (ParentInternal is not null)
            {
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Orientation);
            }
        }
    }

    private void UpdateLayoutStyle(Orientation newRaftingRowOrientation)
    {
        if (_layoutStyle is not ToolStripLayoutStyle.HorizontalStackWithOverflow and not ToolStripLayoutStyle.VerticalStackWithOverflow)
        {
            using (new LayoutTransaction(this, this, PropertyNames.Orientation))
            {
                // We want the ToolStrip to size appropriately when the rafting container orientation has switched.
                UpdateOrientation(newRaftingRowOrientation);
                if (LayoutEngine is ToolStripSplitStackLayout && _layoutStyle == ToolStripLayoutStyle.StackWithOverflow)
                {
                    OnLayoutStyleChanged(EventArgs.Empty);
                }
            }
        }
        else
        {
            // update the orientation but don't force a layout.
            UpdateOrientation(newRaftingRowOrientation);
        }
    }

    private void UpdateOrientation(Orientation newOrientation)
    {
        if (newOrientation != Orientation)
        {
            Orientation = newOrientation;
            SetupGrip();
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.MsgInternal == PInvokeCore.WM_SETFOCUS)
        {
            SnapFocus((HWND)(nint)m.WParamInternal);
        }

        if (!AllowClickThrough && m.MsgInternal == PInvokeCore.WM_MOUSEACTIVATE)
        {
            // We want to prevent taking focus if someone clicks on the toolstrip dropdown itself. The mouse message
            // will still go through, but focus won't be taken. If someone clicks on a child control (ComboBox,
            // textbox, etc) focus will be taken - but we'll handle that in WM_NCACTIVATE handler.
            Point pt = PointToClient(WindowsFormsUtils.LastCursorPoint);
            HWND hwndClicked = PInvoke.ChildWindowFromPointEx(
                this,
                pt,
                CWP_FLAGS.CWP_SKIPINVISIBLE | CWP_FLAGS.CWP_SKIPDISABLED | CWP_FLAGS.CWP_SKIPTRANSPARENT);

            // If we click on the toolstrip itself, eat the activation.
            // If we click on a child control, allow the toolstrip to activate.
            if (hwndClicked == HWND)
            {
                _lastMouseDownedItem = null;
                m.ResultInternal = (LRESULT)(nint)PInvoke.MA_NOACTIVATE;

                if (!IsDropDown && !IsInDesignMode)
                {
                    // If our root HWND is not the active hwnd,eat the mouse message and bring the form to the front.
                    HWND rootHwnd = PInvoke.GetAncestor(this, GET_ANCESTOR_FLAGS.GA_ROOT);
                    if (!rootHwnd.IsNull)
                    {
                        // snap the active window and compare to our root window.
                        HWND hwndActive = PInvoke.GetActiveWindow();
                        if (hwndActive != rootHwnd)
                        {
                            // Activate the window, and discard the mouse message.
                            // this appears to be the same behavior as office.
                            m.ResultInternal = (LRESULT)(nint)PInvoke.MA_ACTIVATEANDEAT;
                        }
                    }
                }

                return;
            }
            else
            {
                // We're setting focus to a child control - remember who gave it to us so we can restore it on ESC.
                SnapFocus(PInvoke.GetFocus());
                if (!IsDropDown && !TabStop)
                {
                    Application.ThreadContext.FromCurrent().AddMessageFilter(RestoreFocusFilter);
                }
            }
        }

        base.WndProc(ref m);

        if (AllowClickThrough && m.MsgInternal == PInvokeCore.WM_MOUSEACTIVATE && m.ResultInternal == PInvoke.MA_ACTIVATEANDEAT)
        {
            m.ResultInternal = (LRESULT)(nint)PInvoke.MA_ACTIVATE;
        }

        if (m.Msg == (int)PInvokeCore.WM_NCDESTROY)
        {
            // Destroy the owner window, if we created one. We
            // cannot do this in OnHandleDestroyed, because at
            // that point our handle is not actually destroyed so
            // destroying our parent actually causes a recursive
            // WM_DESTROY.
            _dropDownOwnerWindow?.DestroyHandle();
        }
    }

    // Overridden to return Items instead of Controls.
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
        return new ReadOnlyControlCollection(this, isReadOnly: !DesignMode);
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

    // When we click somewhere outside of the toolstrip it should be as if we hit esc.

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
