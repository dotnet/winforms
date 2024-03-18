// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;
using Windows.Win32.System.Ole;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.Graphics.Dwm;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using Encoding = System.Text.Encoding;

namespace System.Windows.Forms;

/// <summary>
///  Defines the base class for controls, which are components with visual representation.
/// </summary>
/// <remarks>
///  <para>
///   Do not add instance variables to Control absolutely necessary. Every control on a form has the overhead of
///   all of these variables.
///  </para>
/// </remarks>
[DefaultProperty(nameof(Text))]
[DefaultEvent(nameof(Click))]
[Designer($"System.Windows.Forms.Design.ControlDesigner, {AssemblyRef.SystemDesign}")]
[DesignerSerializer(
    $"System.Windows.Forms.Design.ControlCodeDomSerializer, {AssemblyRef.SystemDesign}",
    $"System.ComponentModel.Design.Serialization.CodeDomSerializer, {AssemblyRef.SystemDesign}")]
[ToolboxItemFilter("System.Windows.Forms")]
public unsafe partial class Control :
    Component,
    ISupportOleDropSource,
    IDropTarget,
    ISynchronizeInvoke,
    IWin32Window,
    IArrangedElement,
    IBindableComponent,
    IKeyboardToolTip,
    IHandle<HWND>
{
#if DEBUG
    private static readonly BooleanSwitch s_bufferPinkRect = new(
        "BufferPinkRect",
        "Renders a pink rectangle with painting double buffered controls");
    private static readonly BooleanSwitch s_bufferDisabled = new(
        "BufferDisabled",
        "Makes double buffered controls non-double buffered");
#endif

    // Feature switch, when set to false, design time features of controls are not supported in trimmed applications.
    [FeatureSwitchDefinition("System.Windows.Forms.Control.AreDesignTimeFeaturesSupported")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
    internal static bool AreDesignTimeFeaturesSupported { get; } =
        AppContext.TryGetSwitch("System.Windows.Forms.Control.AreDesignTimeFeaturesSupported", out bool isEnabled)
            ? isEnabled
            : true;

    // Feature switch, when set to true, used for trimming to access ComponentModel in a trim safe manner
    [FeatureSwitchDefinition("System.Windows.Forms.Control.UseComponentModelRegisteredTypes")]
    internal static bool UseComponentModelRegisteredTypes { get; } =
        AppContext.TryGetSwitch("System.Windows.Forms.Control.UseComponentModelRegisteredTypes", out bool isEnabled)
            ? isEnabled
            : false;
#pragma warning restore IDE0075

    private static readonly uint WM_GETCONTROLNAME = PInvoke.RegisterWindowMessage("WM_GETCONTROLNAME");
    private static readonly uint WM_GETCONTROLTYPE = PInvoke.RegisterWindowMessage("WM_GETCONTROLTYPE");

    private static readonly object s_autoSizeChangedEvent = new();
    private static readonly object s_keyDownEvent = new();
    private static readonly object s_keyPressEvent = new();
    private static readonly object s_keyUpEvent = new();
    private static readonly object s_mouseDownEvent = new();
    private static readonly object s_mouseEnterEvent = new();
    private static readonly object s_mouseLeaveEvent = new();
    private static readonly object s_dpiChangedBeforeParentEvent = new();
    private static readonly object s_dpiChangedAfterParentEvent = new();
    private static readonly object s_mouseHoverEvent = new();
    private static readonly object s_mouseMoveEvent = new();
    private static readonly object s_mouseUpEvent = new();
    private static readonly object s_mouseWheelEvent = new();
    private static readonly object s_clickEvent = new();
    private static readonly object s_clientSizeEvent = new();
    private static readonly object s_doubleClickEvent = new();
    private static readonly object s_mouseClickEvent = new();
    private static readonly object s_mouseDoubleClickEvent = new();
    private static readonly object s_mouseCaptureChangedEvent = new();
    private static readonly object s_moveEvent = new();
    private static readonly object s_resizeEvent = new();
    private static readonly object s_layoutEvent = new();
    private static readonly object s_gotFocusEvent = new();
    private static readonly object s_lostFocusEvent = new();
    private static readonly object s_enterEvent = new();
    private static readonly object s_leaveEvent = new();
    private static readonly object s_handleCreatedEvent = new();
    private static readonly object s_handleDestroyedEvent = new();
    private static readonly object s_controlAddedEvent = new();
    private static readonly object s_controlRemovedEvent = new();
    private static readonly object s_changeUICuesEvent = new();
    private static readonly object s_systemColorsChangedEvent = new();
    private static readonly object s_validatingEvent = new();
    private static readonly object s_validatedEvent = new();
    private static readonly object s_styleChangedEvent = new();
    private static readonly object s_imeModeChangedEvent = new();
    private static readonly object s_helpRequestedEvent = new();
    private static readonly object s_paintEvent = new();
    private static readonly object s_invalidatedEvent = new();
    private static readonly object s_queryContinueDragEvent = new();
    private static readonly object s_giveFeedbackEvent = new();
    private static readonly object s_dragEnterEvent = new();
    private static readonly object s_dragLeaveEvent = new();
    private static readonly object s_dragOverEvent = new();
    private static readonly object s_dragDropEvent = new();
    private static readonly object s_queryAccessibilityHelpEvent = new();
    private static readonly object s_backgroundImageEvent = new();
    private static readonly object s_backgroundImageLayoutEvent = new();
    private static readonly object s_bindingContextEvent = new();
    private static readonly object s_backColorEvent = new();
    private static readonly object s_parentEvent = new();
    private static readonly object s_visibleEvent = new();
    private static readonly object s_textEvent = new();
    private static readonly object s_tabStopEvent = new();
    private static readonly object s_tabIndexEvent = new();
    private static readonly object s_sizeEvent = new();
    private static readonly object s_rightToLeftEvent = new();
    private static readonly object s_locationEvent = new();
    private static readonly object s_foreColorEvent = new();
    private static readonly object s_fontEvent = new();
    private static readonly object s_enabledEvent = new();
    private static readonly object s_dockEvent = new();
    private static readonly object s_cursorEvent = new();
    private static readonly object s_contextMenuStripEvent = new();
    private static readonly object s_causesValidationEvent = new();
    private static readonly object s_regionChangedEvent = new();
    private static readonly object s_marginChangedEvent = new();
    private protected static readonly object s_paddingChangedEvent = new();
    private static readonly object s_previewKeyDownEvent = new();
    private static readonly object s_dataContextEvent = new();

    private static MessageId s_threadCallbackMessage;
    private static ContextCallback? s_invokeMarshaledCallbackHelperDelegate;

    [ThreadStatic]
    private static bool t_inCrossThreadSafeCall;

    [ThreadStatic]
    internal static HelpInfo? t_currentHelpInfo;

    private static FontHandleWrapper? s_defaultFontHandleWrapper;

    private const short PaintLayerBackground = 1;
    private const short PaintLayerForeground = 2;

    private const byte RequiredScalingEnabledMask = 0x10;
    private const byte RequiredScalingMask = 0x0F;

    private const byte HighOrderBitMask = 0x80;

    private static Font? s_defaultFont;

    // Property store keys for properties.  The property store allocates most efficiently
    // in groups of four, so we try to lump properties in groups of four based on how
    // likely they are going to be used in a group.
    private static readonly int s_namePropertyProperty = PropertyStore.CreateKey();
    private static readonly int s_backBrushProperty = PropertyStore.CreateKey();
    private static readonly int s_fontHeightProperty = PropertyStore.CreateKey();
    private static readonly int s_currentAmbientFontProperty = PropertyStore.CreateKey();

    private static readonly int s_controlsCollectionProperty = PropertyStore.CreateKey();
    private static readonly int s_backColorProperty = PropertyStore.CreateKey();
    private static readonly int s_foreColorProperty = PropertyStore.CreateKey();
    private static readonly int s_fontProperty = PropertyStore.CreateKey();

    private static readonly int s_backgroundImageProperty = PropertyStore.CreateKey();
    private static readonly int s_fontHandleWrapperProperty = PropertyStore.CreateKey();
    private static readonly int s_userDataProperty = PropertyStore.CreateKey();

    private static readonly int s_cursorProperty = PropertyStore.CreateKey();
    private static readonly int s_regionProperty = PropertyStore.CreateKey();
    private static readonly int s_rightToLeftProperty = PropertyStore.CreateKey();

    private static readonly int s_bindingsProperty = PropertyStore.CreateKey();
    private static readonly int s_bindingManagerProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleDefaultActionProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleDescriptionProperty = PropertyStore.CreateKey();

    private static readonly int s_accessibilityProperty = PropertyStore.CreateKey();
    private static readonly int s_ncAccessibilityProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleNameProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleRoleProperty = PropertyStore.CreateKey();

    private static readonly int s_activeXImplProperty = PropertyStore.CreateKey();
    private static readonly int s_controlVersionInfoProperty = PropertyStore.CreateKey();
    private static readonly int s_backgroundImageLayoutProperty = PropertyStore.CreateKey();

    private static readonly int s_contextMenuStripProperty = PropertyStore.CreateKey();
    private static readonly int s_autoScrollOffsetProperty = PropertyStore.CreateKey();
    private static readonly int s_useCompatibleTextRenderingProperty = PropertyStore.CreateKey();

    private static readonly int s_imeWmCharsToIgnoreProperty = PropertyStore.CreateKey();
    private static readonly int s_imeModeProperty = PropertyStore.CreateKey();
    private static readonly int s_disableImeModeChangedCountProperty = PropertyStore.CreateKey();
    private static readonly int s_lastCanEnableImeProperty = PropertyStore.CreateKey();

    private static readonly int s_cacheTextCountProperty = PropertyStore.CreateKey();
    private static readonly int s_cacheTextFieldProperty = PropertyStore.CreateKey();
    private static readonly int s_ambientPropertiesServiceProperty = PropertyStore.CreateKey();

    private static readonly int s_dataContextProperty = PropertyStore.CreateKey();

    private static bool s_needToLoadComCtl = true;
    private static readonly int s_darkModeProperty = PropertyStore.CreateKey();

    // This switch determines the default text rendering engine to use by some controls that support switching rendering engine.
    // CheckedListBox, PropertyGrid, GroupBox, Label and LinkLabel, and ButtonBase controls.
    // True means use GDI+, false means use GDI (TextRenderer).
    internal static bool UseCompatibleTextRenderingDefault { get; set; } = true;

    // Control instance members
    //
    // Note: Do not add anything to this list unless absolutely necessary.
    //       Every control on a form has the overhead of all of these
    //       variables!

    // Resist the temptation to make this variable 'internal' rather than
    // private. Handle access should be tightly controlled, and is in this
    // file.  Making it 'internal' makes controlling it quite difficult.
    private readonly ControlNativeWindow _window;

    private Control? _parent;
    private WeakReference<Control>? _reflectParent;
    private CreateParams? _createParams;
    private int _x;
    private int _y;
    private int _width;
    private int _height;
    private int _clientWidth;
    private int _clientHeight;
    private States _state;
    private ExtendedStates _extendedState;

    /// <summary>
    /// User supplied control style
    /// </summary>
    private ControlStyles _controlStyle;
    private int _tabIndex;
    private string? _text;                       // See ControlStyles.CacheText for usage notes
    private byte _requiredScaling;              // bits 0-4: BoundsSpecified stored in RequiredScaling property.  Bit 5: RequiredScalingEnabled property.
    private TRACKMOUSEEVENT _trackMouseEvent;
    private short _updateCount;
    private LayoutEventArgs? _cachedLayoutEventArgs;
    private Queue<ThreadMethodEntry>? _threadCallbackList;
    internal int _deviceDpi;
    internal int _oldDeviceDpi;

    // For keeping track of our ui state for focus and keyboard cues. Using a member
    // variable here because we hit this a lot
    private UICuesStates _uiCuesState;

    // Stores scaled font from Dpi changed values. This is required to distinguish the Font change from
    // Dpi changed events and explicit Font change/assignment.
    private Font? _scaledControlFont;
    private FontHandleWrapper? _scaledFontWrapper;

    // ContainerControls like 'PropertyGrid' scale their children when they resize.
    // no explicit scaling of children required in such cases. They have specific logic.
    internal bool _doNotScaleChildren;

    // Contains a collection of calculated fonts for various Dpi values of the control in the PerMonV2 mode.
    private Dictionary<int, Font>? _dpiFonts;

    // Flag to signify whether any child controls necessitate the calculation of AnchorsInfo, particularly in cases involving nested containers.
    internal bool _childControlsNeedAnchorLayout;

    // Inform whether the AnchorsInfo needs to be reevaluated, especially when the control's bounds have been altered explicitly.
    internal bool _forceAnchorCalculations;

    internal byte LayoutSuspendCount { get; private set; }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Control"/> class.
    /// </summary>
    public Control() : this(true)
    {
    }

    internal Control(bool autoInstallSyncContext) : base()
    {
        Properties = new PropertyStore();

        // Initialize Dpi to the value on the primary screen, we will have the correct value when the Handle is created.
        _deviceDpi = _oldDeviceDpi = ScaleHelper.InitialSystemDpi;
        _window = new ControlNativeWindow(this);
        RequiredScalingEnabled = true;
        RequiredScaling = BoundsSpecified.All;
        _tabIndex = -1;

        _state = States.Visible | States.Enabled | States.TabStop | States.CausesValidation;
        _extendedState = ExtendedStates.InterestedInUserPreferenceChanged;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.StandardClick
                | ControlStyles.StandardDoubleClick
                | ControlStyles.UseTextForAccessibility
                | ControlStyles.Selectable,
            true);

        // We baked the "default default" margin and min size into CommonProperties
        // so that in the common case the PropertyStore would be empty.  If, however,
        // someone overrides these Default* methods, we need to write the default
        // value into the PropertyStore in the ctor.

        // Changing the order of property accesses here can break existing code as these are all virtual properties.
        // Try to keep observable state for Control unchanged in this constructor to avoid nasty subtle bugs.

        InitializeConstantsForInitialDpi(_deviceDpi);

        if (DefaultMargin != CommonProperties.DefaultMargin)
        {
            Margin = DefaultMargin;
        }

        if (DefaultMinimumSize != CommonProperties.DefaultMinimumSize)
        {
            MinimumSize = DefaultMinimumSize;
        }

        if (DefaultMaximumSize != CommonProperties.DefaultMaximumSize)
        {
            MaximumSize = DefaultMaximumSize;
        }

        // Compute our default size.
        Size defaultSize = DefaultSize;
        _width = defaultSize.Width;
        _height = defaultSize.Height;

        // DefaultSize may have hit GetPreferredSize causing a PreferredSize to be cached.  The
        // PreferredSize may change as a result of the current size.  Since a  SetBoundsCore did
        // not happen, so we need to clear the preferredSize cache manually.
        CommonProperties.xClearPreferredSizeCache(this);

        if (_width != 0 && _height != 0)
        {
            RECT rect = default;

            CreateParams cp = CreateParams;

            AdjustWindowRectExForControlDpi(ref rect, (WINDOW_STYLE)cp.Style, false, (WINDOW_EX_STYLE)cp.ExStyle);
            _clientWidth = _width - rect.Width;
            _clientHeight = _height - rect.Height;
        }

        // Set up for async operations on this thread.
        if (autoInstallSyncContext)
        {
            WindowsFormsSynchronizationContext.InstallIfNeeded();
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Control"/> class.
    /// </summary>
    public Control(string? text) : this(null, text)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Control"/> class.
    /// </summary>
    public Control(string? text, int left, int top, int width, int height) : this(null, text, left, top, width, height)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Control"/> class.
    /// </summary>
    public Control(Control? parent, string? text) : this()
    {
        Parent = parent;
        Text = text;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Control"/> class.
    /// </summary>
    public Control(Control? parent, string? text, int left, int top, int width, int height) : this(parent, text)
    {
        Location = new Point(left, top);
        Size = new Size(width, height);
    }

    /// <summary>
    ///  Gets control Dpi awareness context value.
    /// </summary>
    internal DPI_AWARENESS_CONTEXT DpiAwarenessContext => _window.DpiAwarenessContext;

    /// <summary>
    ///  The Accessibility Object for this Control
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlAccessibilityObjectDescr))]
    public AccessibleObject AccessibilityObject
    {
        get
        {
            AccessibleObject? accessibleObject = (AccessibleObject?)Properties.GetObject(s_accessibilityProperty);
            if (accessibleObject is null)
            {
                accessibleObject = CreateAccessibilityInstance();
                Properties.SetObject(s_accessibilityProperty, accessibleObject);
            }

            Debug.Assert(accessibleObject is not null, "Failed to create accessibility object");
            return accessibleObject;
        }
    }

    /// <summary>
    ///  Private accessibility object for control, used to wrap the object that
    ///  OLEACC.DLL creates to represent the control's non-client (NC) region.
    /// </summary>
    private AccessibleObject NcAccessibilityObject
    {
        get
        {
            AccessibleObject? ncAccessibleObject = (AccessibleObject?)Properties.GetObject(s_ncAccessibilityProperty);
            if (ncAccessibleObject is null)
            {
                ncAccessibleObject = new ControlAccessibleObject(this, (int)OBJECT_IDENTIFIER.OBJID_WINDOW);
                Properties.SetObject(s_ncAccessibilityProperty, ncAccessibleObject);
            }

            Debug.Assert(ncAccessibleObject is not null, "Failed to create NON-CLIENT accessibility object");
            return ncAccessibleObject;
        }
    }

    /// <summary>
    ///  Returns a specific AccessibleObject associated with this
    ///  control, based on standard "accessible object id".
    /// </summary>
    private AccessibleObject? GetAccessibilityObject(int accObjId)
    {
        AccessibleObject? accessibleObject;

        switch ((OBJECT_IDENTIFIER)accObjId)
        {
            case OBJECT_IDENTIFIER.OBJID_CLIENT:
                accessibleObject = AccessibilityObject;
                break;
            case OBJECT_IDENTIFIER.OBJID_WINDOW:
                accessibleObject = NcAccessibilityObject;
                break;
            default:
                if (accObjId > 0)
                {
                    accessibleObject = GetAccessibilityObjectById(accObjId);
                }
                else
                {
                    accessibleObject = null;
                }

                break;
        }

        return accessibleObject;
    }

    /// <summary>
    ///  Returns a specific AccessibleObject associated w/ the objectID
    /// </summary>
    protected virtual AccessibleObject? GetAccessibilityObjectById(int objectId)
    {
        if (this is IAutomationLiveRegion)
        {
            return AccessibilityObject;
        }

        return null;
    }

    /// <summary>
    ///  The default action description of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlAccessibleDefaultActionDescr))]
    public string? AccessibleDefaultActionDescription
    {
        get => (string?)Properties.GetObject(s_accessibleDefaultActionProperty);
        set => Properties.SetObject(s_accessibleDefaultActionProperty, value);
    }

    /// <summary>
    ///  The accessible description of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(null)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlAccessibleDescriptionDescr))]
    public string? AccessibleDescription
    {
        get => (string?)Properties.GetObject(s_accessibleDescriptionProperty);
        set => Properties.SetObject(s_accessibleDescriptionProperty, value);
    }

    /// <summary>
    ///  The accessible name of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(null)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlAccessibleNameDescr))]
    public string? AccessibleName
    {
        get => (string?)Properties.GetObject(s_accessibleNameProperty);
        set => Properties.SetObject(s_accessibleNameProperty, value);
    }

    /// <summary>
    ///  The accessible role of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(AccessibleRole.Default)]
    [SRDescription(nameof(SR.ControlAccessibleRoleDescr))]
    public AccessibleRole AccessibleRole
    {
        get
        {
            int role = Properties.GetInteger(s_accessibleRoleProperty, out bool found);
            if (found)
            {
                return (AccessibleRole)role;
            }
            else
            {
                return AccessibleRole.Default;
            }
        }
        set
        {
            // valid values are -1 to 0x40
            SourceGenerated.EnumValidator.Validate(value);
            Properties.SetInteger(s_accessibleRoleProperty, (int)value);
        }
    }

    /// <summary>
    ///  The AllowDrop property. If AllowDrop is set to true then
    ///  this control will allow drag and drop operations and events to be used.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ControlAllowDropDescr))]
    public virtual bool AllowDrop
    {
        get
        {
            return GetState(States.AllowDrop);
        }
        set
        {
            if (GetState(States.AllowDrop) != value)
            {
                SetState(States.AllowDrop, value);

                if (IsHandleCreated)
                {
                    try
                    {
                        SetAcceptDrops(value);
                    }
                    catch
                    {
                        // If there is an error, back out the AllowDrop state...
                        //
                        SetState(States.AllowDrop, !value);
                        throw;
                    }
                }
            }
        }
    }

    // Queries the Site for AmbientProperties.  May return null.
    // Do not confuse with inheritedProperties -- the service is turned to
    // after we've exhausted inheritedProperties.
    private AmbientProperties? AmbientPropertiesService
    {
        get
        {
            AmbientProperties? props = (AmbientProperties?)Properties.GetObject(s_ambientPropertiesServiceProperty, out bool contains);
            if (!contains)
            {
                if (Site is not null)
                {
                    props = Site.GetService(typeof(AmbientProperties)) as AmbientProperties;
                }
                else
                {
                    props = (AmbientProperties?)GetService(typeof(AmbientProperties));
                }

                if (props is not null)
                {
                    Properties.SetObject(s_ambientPropertiesServiceProperty, props);
                }
            }

            return props;
        }
    }

    /// <summary>
    ///  The current value of the anchor property. The anchor property
    ///  determines which edges of the control are anchored to the container's
    ///  edges.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [DefaultValue(CommonProperties.DefaultAnchor)]
    [SRDescription(nameof(SR.ControlAnchorDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public virtual AnchorStyles Anchor
    {
        get => DefaultLayout.GetAnchor(this);
        set => DefaultLayout.SetAnchor(this, value);
    }

    [SRCategory(nameof(SR.CatLayout))]
    [RefreshProperties(RefreshProperties.All)]
    [Localizable(true)]
    [DefaultValue(CommonProperties.DefaultAutoSize)]
    [SRDescription(nameof(SR.ControlAutoSizeDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool AutoSize
    {
        get { return CommonProperties.GetAutoSize(this); }
        set
        {
            if (value != AutoSize)
            {
                CommonProperties.SetAutoSize(this, value);
                if (ParentInternal is not null)
                {
                    // DefaultLayout does not keep anchor information until it needs to.  When
                    // AutoSize became a common property, we could no longer blindly call into
                    // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                    if (value && ParentInternal.LayoutEngine == DefaultLayout.Instance)
                    {
                        ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                    }

                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                }

                OnAutoSizeChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler? AutoSizeChanged
    {
        add => Events.AddHandler(s_autoSizeChangedEvent, value);
        remove => Events.RemoveHandler(s_autoSizeChangedEvent, value);
    }

    /// <summary>
    ///  Controls the location of where this control is scrolled to in ScrollableControl.ScrollControlIntoView.
    ///  Default is the upper left hand corner of the control.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DefaultValue(typeof(Point), "0, 0")]
    public virtual Point AutoScrollOffset
    {
        get => Properties.TryGetObject(s_autoScrollOffsetProperty, out Point point)
                ? point
                : Point.Empty;
        set
        {
            if (AutoScrollOffset != value)
            {
                Properties.SetObject(s_autoScrollOffsetProperty, value);
            }
        }
    }

    protected void SetAutoSizeMode(AutoSizeMode mode) => CommonProperties.SetAutoSizeMode(this, mode);

    protected AutoSizeMode GetAutoSizeMode() => CommonProperties.GetAutoSizeMode(this);

    // Public because this is interesting for ControlDesigners.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual LayoutEngine LayoutEngine => DefaultLayout.Instance;

    /// <summary>
    ///  The GDI brush for our background color.
    ///  Whidbey Note: Made this internal, since we need to use this in ButtonStandardAdapter. Also, renamed
    ///         from BackBrush to BackColorBrush due to a naming conflict with DataGrid's BackBrush.
    /// </summary>
    internal HBRUSH BackColorBrush
    {
        get
        {
            object? customBackBrush = Properties.GetObject(s_backBrushProperty);
            if (customBackBrush is not null)
            {
                // We already have a valid brush.  Unbox, and return.
                return (HBRUSH)customBackBrush;
            }

            if (!Properties.ContainsObject(s_backColorProperty))
            {
                // No custom back color.  See if we can get to our parent.
                // The color check here is to account for parents and children who
                // override the BackColor property.
                if (_parent is not null && _parent.BackColor == BackColor)
                {
                    return _parent.BackColorBrush;
                }
            }

            // No parent, or we have a custom back color.  Either way, we need to
            // create our own.
            Color color = BackColor;
            HBRUSH backBrush;

            if (color.IsSystemColor)
            {
                backBrush = PInvoke.GetSysColorBrush(color);
                SetState(States.OwnCtlBrush, false);
            }
            else
            {
                backBrush = PInvoke.CreateSolidBrush((COLORREF)(uint)ColorTranslator.ToWin32(color));
                SetState(States.OwnCtlBrush, true);
            }

            Debug.Assert(!backBrush.IsNull, "Failed to create brushHandle");
            Properties.SetObject(s_backBrushProperty, backBrush);

            return backBrush;
        }
    }

    /// <summary>
    ///  Gets or sets the data context for the purpose of data binding.
    ///  This is an ambient property.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Data context is a concept that allows elements to inherit information from their parent elements
    ///   about the data source that is used for binding. It's the duty of deriving controls which inherit from
    ///   this class to handle the provided data source accordingly. For example, UserControls, which use
    ///   <see cref="BindingSource"/> components for data binding scenarios could either handle the
    ///   <see cref="DataContextChanged"/> event or override <see cref="OnDataContextChanged(EventArgs)"/> to provide
    ///   the relevant data from the data context to a BindingSource component's <see cref="BindingSource.DataSource"/>.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatData))]
    [Browsable(false)]
    [Bindable(true)]
    public virtual object? DataContext
    {
        get => Properties.TryGetObject(s_dataContextProperty, out object? value)
                ? value
                : ParentInternal?.DataContext;
        set
        {
            if (Equals(value, DataContext))
            {
                return;
            }

            // When DataContext was different than its parent before, but now it is about to become the same,
            // we're removing it altogether, so it can inherit the value from its parent.
            if (Properties.ContainsObject(s_dataContextProperty) && Equals(ParentInternal?.DataContext, value))
            {
                Properties.RemoveObject(s_dataContextProperty);
                OnDataContextChanged(EventArgs.Empty);
                return;
            }

            Properties.SetObject(s_dataContextProperty, value);
            OnDataContextChanged(EventArgs.Empty);
        }
    }

    private bool ShouldSerializeDataContext()
        => Properties.ContainsObject(s_dataContextProperty);

    private void ResetDataContext()
        => Properties.RemoveObject(s_dataContextProperty);

    /// <summary>
    ///  The background color of this control. This is an ambient property and
    ///  will always return a non-null value.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DispId(PInvokeCore.DISPID_BACKCOLOR)]
    [SRDescription(nameof(SR.ControlBackColorDescr))]
    public virtual Color BackColor
    {
        get
        {
            Color c = RawBackColor; // inheritedProperties.BackColor
            if (!c.IsEmpty)
            {
                return c;
            }

            Control? parent = ParentInternal;
            if (parent is not null && parent.CanAccessProperties)
            {
                c = parent.BackColor;
                if (IsValidBackColor(c))
                {
                    return c;
                }
            }

            if (IsActiveX)
            {
                c = ActiveXAmbientBackColor;
            }

            if (c.IsEmpty)
            {
                AmbientProperties? ambient = AmbientPropertiesService;
                if (ambient is not null)
                {
                    c = ambient.BackColor;
                }
            }

            if (!c.IsEmpty && IsValidBackColor(c))
            {
                return c;
            }
            else
            {
                return DefaultBackColor;
            }
        }
        set
        {
            if (!value.Equals(Color.Empty) && !GetStyle(ControlStyles.SupportsTransparentBackColor) && value.A < 255)
            {
                throw new ArgumentException(SR.TransparentBackColorNotAllowed);
            }

            Color c = BackColor;
            if (!value.IsEmpty || Properties.ContainsObject(s_backColorProperty))
            {
                Properties.SetColor(s_backColorProperty, value);
            }

            if (!c.Equals(BackColor))
            {
                OnBackColorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnBackColorChangedDescr))]
    public event EventHandler? BackColorChanged
    {
        add => Events.AddHandler(s_backColorEvent, value);
        remove => Events.RemoveHandler(s_backColorEvent, value);
    }

    /// <summary>
    ///  The background image of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(null)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlBackgroundImageDescr))]
    public virtual Image? BackgroundImage
    {
        get => (Image?)Properties.GetObject(s_backgroundImageProperty);
        set
        {
            if (BackgroundImage == value)
            {
                return;
            }

            Properties.SetObject(s_backgroundImageProperty, value);
            OnBackgroundImageChanged(EventArgs.Empty);
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnBackgroundImageChangedDescr))]
    public event EventHandler? BackgroundImageChanged
    {
        add => Events.AddHandler(s_backgroundImageEvent, value);
        remove => Events.RemoveHandler(s_backgroundImageEvent, value);
    }

    /// <summary>
    ///  The BackgroundImageLayout of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ImageLayout.Tile)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))]
    public virtual ImageLayout BackgroundImageLayout
    {
        get => Properties.TryGetObject(s_backgroundImageLayoutProperty, out ImageLayout imageLayout)
                ? imageLayout
                : ImageLayout.Tile;
        set
        {
            if (BackgroundImageLayout == value)
            {
                return;
            }

            // Valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);

            // Check if the value is either center, stretch or zoom;
            if (value is ImageLayout.Center or ImageLayout.Zoom or ImageLayout.Stretch)
            {
                SetStyle(ControlStyles.ResizeRedraw, true);

                // Only for images that support transparency.
                if (ControlPaint.IsImageTransparent(BackgroundImage))
                {
                    DoubleBuffered = true;
                }
            }

            Properties.SetObject(s_backgroundImageLayoutProperty, value);
            OnBackgroundImageLayoutChanged(EventArgs.Empty);
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnBackgroundImageLayoutChangedDescr))]
    public event EventHandler? BackgroundImageLayoutChanged
    {
        add => Events.AddHandler(s_backgroundImageLayoutEvent, value);
        remove => Events.RemoveHandler(s_backgroundImageLayoutEvent, value);
    }

    // Set/reset by ContainerControl.AssignActiveControlInternal
    internal bool BecomingActiveControl
    {
        get
        {
            return GetExtendedState(ExtendedStates.BecomingActiveControl);
        }
        set
        {
            if (value != BecomingActiveControl)
            {
                Application.ThreadContext.FromCurrent().ActivatingControl = (value) ? this : null;
                SetExtendedState(ExtendedStates.BecomingActiveControl, value);
            }
        }
    }

    private bool ShouldSerializeAccessibleName() => !string.IsNullOrEmpty(AccessibleName);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetBindings()
    {
        if (!Binding.IsSupported)
        {
            // This gets called with Dispose that needs to be handled, a throwing is not appropriate in this case.
            return;
        }

        ControlBindingsCollection? bindings = (ControlBindingsCollection?)Properties.GetObject(s_bindingsProperty);
        bindings?.Clear();
    }

    /// <summary>
    ///  BindingContextInternal provides a mechanism so that controls like SplitContainer that inherit from the
    ///  ContainerControl can bypass the "containerControls" bindingContext property and do what the other simple controls
    ///  do.
    /// </summary>
    internal BindingContext? BindingContextInternal
    {
        get
        {
            // See if we have locally overridden the binding manager.
            BindingContext? context = (BindingContext?)Properties.GetObject(s_bindingManagerProperty);
            if (context is not null)
            {
                return context;
            }

            // Otherwise, see if the parent has one for us.
            Control? parent = ParentInternal;
            if (parent is not null && parent.CanAccessProperties)
            {
                return parent.BindingContext;
            }

            // Otherwise, we have no binding manager available.
            return null;
        }
        set
        {
            BindingContext? oldContext = (BindingContext?)Properties.GetObject(s_bindingManagerProperty);
            BindingContext? newContext = value;

            if (oldContext != newContext)
            {
                Properties.SetObject(s_bindingManagerProperty, newContext);

                // the property change will wire up the bindings.
                OnBindingContextChanged(EventArgs.Empty);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlBindingContextDescr))]
    public virtual BindingContext? BindingContext
    {
        get => BindingContextInternal;
        set => BindingContextInternal = value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnBindingContextChangedDescr))]
    public event EventHandler? BindingContextChanged
    {
        add => Events.AddHandler(s_bindingContextEvent, value);
        remove => Events.RemoveHandler(s_bindingContextEvent, value);
    }

    /// <summary>
    ///  The bottom coordinate of this control.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlBottomDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public int Bottom => _y + _height;

    /// <summary>
    ///  The bounds of this control. This is the window coordinates of the
    ///  control in parent client coordinates.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlBoundsDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public Rectangle Bounds
    {
        get => new(_x, _y, _width, _height);
        set => SetBounds(value.X, value.Y, value.Width, value.Height, BoundsSpecified.All);
    }

    internal virtual bool CanAccessProperties => true;

    /// <summary>
    ///  Indicates whether the control can receive focus. This
    ///  property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlCanFocusDescr))]
    public bool CanFocus
    {
        get
        {
            if (!IsHandleCreated)
            {
                return false;
            }

            return PInvoke.IsWindowVisible(this)
                && PInvoke.IsWindowEnabled(this);
        }
    }

    /// <summary>
    ///  Determines if events can be fired on the control.  If this control is being
    ///  hosted as an ActiveX control, this property will return false if the ActiveX
    ///  control has its events frozen.
    /// </summary>
    protected override bool CanRaiseEvents => !IsActiveX || !ActiveXEventsFrozen;

    /// <summary>
    ///  Indicates whether the control can be selected. This property
    ///  is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlCanSelectDescr))]
    public bool CanSelect => CanSelectCore();

    /// <summary>
    ///  Indicates whether the control has captured the mouse.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlCaptureDescr))]
    public bool Capture
    {
        get => IsHandleCreated && PInvoke.GetCapture() == HWND;
        set
        {
            if (Capture != value)
            {
                if (value)
                {
                    PInvoke.SetCapture(this);
                }
                else
                {
                    PInvoke.ReleaseCapture();
                }
            }
        }
    }

    /// <summary>
    ///  Indicates whether entering the control causes validation on the controls requiring validation.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.ControlCausesValidationDescr))]
    public bool CausesValidation
    {
        get => GetState(States.CausesValidation);
        set
        {
            if (value != CausesValidation)
            {
                SetState(States.CausesValidation, value);
                OnCausesValidationChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnCausesValidationChangedDescr))]
    public event EventHandler? CausesValidationChanged
    {
        add => Events.AddHandler(s_causesValidationEvent, value);
        remove => Events.RemoveHandler(s_causesValidationEvent, value);
    }

    /// <summary>
    ///  This is for perf. Turn this property on to temporarily enable text caching. This is good for operations such
    ///  as layout or painting where we don't expect the text to change (we will update the cache if it does). It
    ///  prevents us from sending a ton of messages during layout. See the <see cref="PaintWithErrorHandling(PaintEventArgs, short)"/>
    ///  function.
    /// </summary>
    internal bool CacheTextInternal
    {
        get
        {
            // Check if we're caching text.
            int cacheTextCounter = Properties.GetInteger(s_cacheTextCountProperty, out _);

            return cacheTextCounter > 0 || GetStyle(ControlStyles.CacheText);
        }
        set
        {
            // If this control always caches text or the handle hasn't been created, just bail.
            if (GetStyle(ControlStyles.CacheText) || !IsHandleCreated)
            {
                return;
            }

            // Otherwise, get the state and update the cache if necessary.
            int cacheTextCounter = Properties.GetInteger(s_cacheTextCountProperty, out _);

            if (value)
            {
                if (cacheTextCounter == 0)
                {
                    Properties.SetObject(s_cacheTextFieldProperty, _text);
                    _text ??= WindowText;
                }

                cacheTextCounter++;
            }
            else
            {
                cacheTextCounter--;
                if (cacheTextCounter == 0)
                {
                    _text = (string?)Properties.GetObject(s_cacheTextFieldProperty, out _);
                }
            }

            Properties.SetInteger(s_cacheTextCountProperty, cacheTextCounter);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlCheckForIllegalCrossThreadCalls))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool CheckForIllegalCrossThreadCalls { get; set; } = Debugger.IsAttached;

    /// <summary>
    ///  The client rect of the control.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlClientRectangleDescr))]
    public Rectangle ClientRectangle
    {
        get
        {
            return new Rectangle(0, 0, _clientWidth, _clientHeight);
        }
    }

    /// <summary>
    ///  The size of the clientRect.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlClientSizeDescr))]
    public Size ClientSize
    {
        get => new(_clientWidth, _clientHeight);
        set => SetClientSizeCore(value.Width, value.Height);
    }

    /// <summary>
    ///  Fired when ClientSize changes.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnClientSizeChangedDescr))]
    public event EventHandler? ClientSizeChanged
    {
        add => Events.AddHandler(s_clientSizeEvent, value);
        remove => Events.RemoveHandler(s_clientSizeEvent, value);
    }

    /// <summary>
    ///  Retrieves the company name of this specific component.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlCompanyNameDescr))]
    public string CompanyName => VersionInfo.CompanyName;

    /// <summary>
    ///  Indicates whether the control or one of its children currently has the system
    ///  focus. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlContainsFocusDescr))]
    public bool ContainsFocus
    {
        get
        {
            if (!IsHandleCreated)
            {
                return false;
            }

            HWND focusHwnd = PInvoke.GetFocus();
            if (focusHwnd.IsNull)
            {
                return false;
            }

            return focusHwnd == Handle || PInvoke.IsChild(this, focusHwnd);
        }
    }

    /// <summary>
    ///  The contextMenuStrip associated with this control. The contextMenuStrip
    ///  will be shown when the user right clicks the mouse on the control.
    ///  Note: if a context menu is also assigned, it will take precedence over this property.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.ControlContextMenuDescr))]
    public virtual ContextMenuStrip? ContextMenuStrip
    {
        get => (ContextMenuStrip?)Properties.GetObject(s_contextMenuStripProperty);
        set
        {
            ContextMenuStrip? oldValue = Properties.GetObject(s_contextMenuStripProperty) as ContextMenuStrip;

            if (oldValue != value)
            {
                EventHandler disposedHandler = new(DetachContextMenuStrip);

                if (oldValue is not null)
                {
                    oldValue.Disposed -= disposedHandler;
                }

                Properties.SetObject(s_contextMenuStripProperty, value);

                if (value is not null)
                {
                    value.Disposed += disposedHandler;
                }

                OnContextMenuStripChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlContextMenuStripChangedDescr))]
    public event EventHandler? ContextMenuStripChanged
    {
        add => Events.AddHandler(s_contextMenuStripEvent, value);
        remove => Events.RemoveHandler(s_contextMenuStripEvent, value);
    }

    /// <summary>
    ///  Collection of child controls.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRDescription(nameof(SR.ControlControlsDescr))]
    public ControlCollection Controls
    {
        get
        {
            ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

            if (controlsCollection is null)
            {
                controlsCollection = CreateControlsInstance();
                Properties.SetObject(s_controlsCollectionProperty, controlsCollection);
            }

            return controlsCollection;
        }
    }

    /// <summary>
    ///  Indicates whether the control has been created. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlCreatedDescr))]
    public bool Created => GetState(States.Created);

    /// <summary>
    ///  Returns the CreateParams used to create the handle for this control.
    ///  Inheriting classes should call base.CreateParams in the manor
    ///  below:
    /// </summary>
    protected virtual CreateParams CreateParams
    {
        get
        {
            // CLR4.0 or later, comctl32.dll needs to be loaded explicitly.
            if (s_needToLoadComCtl)
            {
                if ((PInvoke.GetModuleHandle(Libraries.Comctl32) != 0)
                 || (PInvoke.LoadComctl32(Application.StartupPath) != 0))
                {
                    s_needToLoadComCtl = false;
                }
                else
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, Libraries.Comctl32));
                }
            }

            // In a typical control this is accessed ten times to create and show a control.
            // It is a net memory savings, then, to maintain a copy on control.
            _createParams ??= new CreateParams();

            CreateParams cp = _createParams;
            cp.Style = 0;
            cp.ExStyle = 0;
            cp.ClassStyle = 0;
            cp.Caption = _text;

            cp.X = _x;
            cp.Y = _y;
            cp.Width = _width;
            cp.Height = _height;

            cp.Style = (int)WINDOW_STYLE.WS_CLIPCHILDREN;
            if (GetStyle(ControlStyles.ContainerControl))
            {
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CONTROLPARENT;
            }

            cp.ClassStyle = (int)WNDCLASS_STYLES.CS_DBLCLKS;

            if (!_state.HasFlag(States.TopLevel))
            {
                // When the window is actually created, we will parent WS_CHILD windows to the
                // parking form if cp.parent == 0.
                cp.Parent = _parent is null ? IntPtr.Zero : _parent.InternalHandle;
                cp.Style |= (int)(WINDOW_STYLE.WS_CHILD | WINDOW_STYLE.WS_CLIPSIBLINGS);
            }
            else
            {
                cp.Parent = IntPtr.Zero;
            }

            if (_state.HasFlag(States.TabStop))
            {
                cp.Style |= (int)WINDOW_STYLE.WS_TABSTOP;
            }

            if (_state.HasFlag(States.Visible))
            {
                cp.Style |= (int)WINDOW_STYLE.WS_VISIBLE;
            }

            // Unlike Visible, Windows doesn't correctly inherit disabledness from its parent -- an enabled child
            // of a disabled parent will look enabled but not get mouse events
            if (!Enabled)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_DISABLED;
            }

            // If we are being hosted as an Ax control, try to prevent the parking window
            // from being created by pre-filling the window handle here.
            if (cp.Parent == IntPtr.Zero && IsActiveX)
            {
                cp.Parent = ActiveXHWNDParent;
            }

            // Set Rtl bits
            if (RightToLeft == RightToLeft.Yes)
            {
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_RTLREADING;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_RIGHT;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR;
            }

            return cp;
        }
    }

    internal virtual void NotifyValidationResult(object? sender, CancelEventArgs ev)
    {
        ValidationCancelled = ev.Cancel;
    }

    /// <summary>
    ///  Helper method...
    ///
    ///  Triggers validation on the active control, and returns bool indicating whether that control was valid.
    ///
    ///  The correct way to do this is to find the common ancestor of the active control and this control,
    ///  then request validation to be performed by that common container control.
    ///
    ///  Used by controls that don't participate in the normal enter/leave/validation process, but which
    ///  want to force form-level validation to occur before they attempt some important action.
    /// </summary>
    internal bool ValidateActiveControl(out bool validatedControlAllowsFocusChange)
    {
        bool valid = true;
        validatedControlAllowsFocusChange = false;
        IContainerControl? c = GetContainerControl();
        if (c is not null && CausesValidation)
        {
            if (c is ContainerControl container)
            {
                while (container.ActiveControl is null)
                {
                    ContainerControl? cc;
                    Control? parent = container.ParentInternal;
                    if (parent is not null)
                    {
                        cc = parent.GetContainerControl() as ContainerControl;
                        if (cc is not null)
                        {
                            container = cc;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                valid = container.ValidateInternal(true, out validatedControlAllowsFocusChange);
            }
        }

        return valid;
    }

    internal bool ValidationCancelled
    {
        get
        {
            if (GetState(States.ValidationCancelled))
            {
                return true;
            }
            else
            {
                Control? parent = ParentInternal;
                if (parent is not null)
                {
                    return parent.ValidationCancelled;
                }

                return false;
            }
        }
        set => SetState(States.ValidationCancelled, value);
    }

    /// <summary>
    ///  returns bool indicating whether the Top MDI Window is closing.
    ///  This property is set in the MDI children in WmClose method in form.cs when the top window is closing.
    ///  This property will be used in ActiveControl to determine if we want to skip set focus and window handle re-creation for the control.
    /// </summary>
    internal bool IsTopMdiWindowClosing
    {
        get => GetExtendedState(ExtendedStates.TopMDIWindowClosing);
        set => SetExtendedState(ExtendedStates.TopMDIWindowClosing, value);
    }

    /// <summary>
    ///  Returns bool indicating whether the control is currently being scaled.
    ///  This property is set in ScaleControl method to allow method being called to condition code that should not run for scaling.
    /// </summary>
    internal bool ScalingInProgress
    {
        get => GetExtendedState(ExtendedStates.CurrentlyBeingScaled);
        private set => SetExtendedState(ExtendedStates.CurrentlyBeingScaled, value);
    }

    /// <summary>
    ///  Indicates whether or not this control has an accessible object associated with it.
    /// </summary>
    internal bool IsAccessibilityObjectCreated => Properties.GetObject(s_accessibilityProperty) is ControlAccessibleObject;

    /// <summary>
    ///  Retrieves the Win32 thread ID of the thread that created the
    ///  handle for this control.  If the control's handle hasn't been
    ///  created yet, this method will return the current thread's ID.
    /// </summary>
    internal uint CreateThreadId => IsHandleCreated
        ? PInvoke.GetWindowThreadProcessId(this, out _)
        : PInvoke.GetCurrentThreadId();

    /// <summary>
    ///  Retrieves the cursor that will be displayed when the mouse is over this
    ///  control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ControlCursorDescr))]
    [AmbientValue(null)]
    [AllowNull]
    public virtual Cursor Cursor
    {
        get
        {
            if (GetState(States.UseWaitCursor))
            {
                return Cursors.WaitCursor;
            }

            Cursor? cursor = (Cursor?)Properties.GetObject(s_cursorProperty);
            if (cursor is not null)
            {
                return cursor;
            }

            // We only do ambients for things with "Cursors.Default"
            // as their default.
            Cursor localDefault = DefaultCursor;
            if (localDefault != Cursors.Default)
            {
                return localDefault;
            }

            Control? parent = ParentInternal;
            if (parent is not null)
            {
                return parent.Cursor;
            }

            AmbientProperties? ambient = AmbientPropertiesService;
            if (ambient is not null && ambient.Cursor is not null)
            {
                return ambient.Cursor;
            }

            return localDefault;
        }
        set
        {
            Cursor? localCursor = (Cursor?)Properties.GetObject(s_cursorProperty);
            Cursor resolvedCursor = Cursor;
            if (localCursor != value)
            {
                Properties.SetObject(s_cursorProperty, value);
            }

            // Other things can change the cursor. We always want to force the correct cursor.
            if (IsHandleCreated)
            {
                // We want to instantly change the cursor if the mouse is within our bounds.
                // This includes the case where the mouse is over one of our children.
                PInvoke.GetCursorPos(out Point p);
                PInvoke.GetWindowRect(this, out RECT r);
                if ((r.left <= p.X && p.X < r.right && r.top <= p.Y && p.Y < r.bottom) || PInvoke.GetCapture() == HWND)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETCURSOR, (WPARAM)HWND, (LPARAM)(int)PInvoke.HTCLIENT);
                }
            }

            if (!resolvedCursor.Equals(value))
            {
                OnCursorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnCursorChangedDescr))]
    public event EventHandler? CursorChanged
    {
        add => Events.AddHandler(s_cursorEvent, value);
        remove => Events.RemoveHandler(s_cursorEvent, value);
    }

    /// <summary>
    ///  Retrieves the bindings for this control.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.ControlBindingsDescr))]
    [RefreshProperties(RefreshProperties.All)]
    [ParenthesizePropertyName(true)]
    public ControlBindingsCollection DataBindings
    {
        get
        {
            if (!Binding.IsSupported)
            {
                throw new NotSupportedException(SR.BindingNotSupported);
            }

            ControlBindingsCollection? bindings = (ControlBindingsCollection?)Properties.GetObject(s_bindingsProperty);
            if (bindings is null)
            {
                bindings = new ControlBindingsCollection(this);
                Properties.SetObject(s_bindingsProperty, bindings);
            }

            return bindings;
        }
    }

    /// <summary>
    ///  Gets or sets the dark mode for the control.
    /// </summary>
    /// <value>
    ///  The dark mode for the control. The default value is <see cref="DarkMode.Inherits"/>. This property is ambient.
    ///  Deriving classes should override <see cref="SetDarkModeCore(DarkMode)"/> to implement their own dark-mode detection logic.
    /// </value>
    public DarkMode DarkMode
    {
        get
        {
            if (Properties.ContainsObject(s_darkModeProperty))
            {
                return (DarkMode)Properties.GetObject(s_darkModeProperty)!;
            }

            return ParentInternal?.DarkMode ?? DarkMode.Inherits;
        }

        set => SetDarkMode(value);
    }

    private bool ShouldSerializeDarkMode()
        => DarkMode != DefaultDarkMode;

    private void ResetDarkMode()
        => DarkMode = DefaultDarkMode;

    /// <summary>
    ///  Tests, if the control is currently in dark mode. This property is ambient. Inherited controls can return false,
    ///  to prevent their base classes to apply their dark-mode logic, and can still test for dark-mode by calling base.IsDarkModeEnabled.
    /// </summary>
    protected virtual bool IsDarkModeEnabled
    {
        get
        {
            if (Properties.ContainsObject(s_darkModeProperty))
            {
                // If we inherit, it's the parent's dark-mode, otherwise it's the value we have.
                return (DarkMode == DarkMode.Inherits
                    && (ParentInternal?.IsDarkModeEnabled ?? false))
                    || DarkMode == DarkMode.Enabled;
            }
            else
            {
                // We're ambient: It's either the parent's or the application's dark-mode.
                return ParentInternal?.IsDarkModeEnabled ?? Application.IsDarkModeEnabled;
            }
        }
    }

    private void SetDarkMode(DarkMode darkMode)
    {
        if (Equals(darkMode, DarkMode))
        {
            return;
        }

        if (darkMode switch
        {
            DarkMode.Inherits or
            DarkMode.Enabled or
            DarkMode.Disabled => Application.EnvironmentDarkMode == DarkMode.NotSupported || !DarkModeSupported
                    ? throw new ArgumentException("${darkModeSetting} is not supported in this Environment.")
                    : true,
            _ => throw new ArgumentException("${darkModeSetting} is not supported in this context.")
        })
        {
            // When DarkModeSetting was different than its parent before, but now it is about to become the same,
            // we're removing it altogether, so it can inherit the value from its parent.
            if (Properties.ContainsObject(s_darkModeProperty) && Equals(ParentInternal?.DarkMode, darkMode))
            {
                Properties.RemoveObject(s_darkModeProperty);
            }
            else
            {
                Properties.SetObject(s_darkModeProperty, darkMode);
            }

            SetDarkModeCore(darkMode);
        }
    }

    /// <summary>
    ///  Inherited classes should override this method to implement their own dark-mode changed logic, if they need it.
    /// </summary>
    /// <param name="darkModeSetting">A value of type <see cref="DarkMode"/> with the new dark-mode setting.</param>
    /// <returns><see langword="true"/>, if the setting succeeded, otherwise <see langword="false"/>.</returns>
    protected virtual bool SetDarkModeCore(DarkMode darkModeSetting) => true;

    /// <summary>
    ///  Determines whether the control supports dark mode.
    /// </summary>
    /// <returns><see langword="true"/>, if the control supports dark mode; otherwise, <see langword="false"/>.</returns>
    protected virtual bool DarkModeSupported
        => Application.EnvironmentDarkMode != DarkMode.NotSupported;

    private static DarkMode DefaultDarkMode => DarkMode.Inherits;

    /// <summary>
    ///  The default BackColor of a generic top-level Control.  Subclasses may have
    ///  different defaults.
    /// </summary>
    public static Color DefaultBackColor => Application.SystemColors.Control;

    /// <summary>
    ///  Deriving classes can override this to configure a default cursor for their control.
    ///  This is more efficient than setting the cursor in the control's constructor,
    ///  and gives automatic support for ShouldSerialize and Reset in the designer.
    /// </summary>
    protected virtual Cursor DefaultCursor => Cursors.Default;

    /// <summary>
    ///  The default Font of a generic top-level Control.  Subclasses may have
    ///  different defaults.
    /// </summary>
    public static Font DefaultFont
    {
        get
        {
            if (s_defaultFont is null)
            {
                s_defaultFont = Application.DefaultFont ?? SystemFonts.MessageBoxFont;
                Debug.Assert(s_defaultFont is not null, "defaultFont wasn't set!");
            }

            return s_defaultFont;
        }
    }

    /// <summary>
    ///  The default ForeColor of a generic top-level Control.  Subclasses may have
    ///  different defaults.
    /// </summary>
    public static Color DefaultForeColor => Application.SystemColors.ControlText;

    protected virtual Padding DefaultMargin => CommonProperties.DefaultMargin;

    protected virtual Size DefaultMaximumSize => CommonProperties.DefaultMaximumSize;

    protected virtual Size DefaultMinimumSize => CommonProperties.DefaultMinimumSize;

    protected virtual Padding DefaultPadding => Padding.Empty;

    private static RightToLeft DefaultRightToLeft => RightToLeft.No;

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected virtual Size DefaultSize => Size.Empty;

    private void DetachContextMenuStrip(object? sender, EventArgs e) => ContextMenuStrip = null;

    /// <summary>
    ///  Dpi value either for the primary screen or for the monitor where the top-level parent is displayed when
    ///  EnableDpiChangedMessageHandling option is on and the application is per-monitor V2 Dpi-aware (rs2+)
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DeviceDpi
        // deviceDpi may change in WmDpiChangedBeforeParent in PmV2 scenarios, so we can't cache statically.
        => ScaleHelper.IsThreadPerMonitorV2Aware ? _deviceDpi : ScaleHelper.InitialSystemDpi;

    // The color to use when drawing disabled text.  Normally we use BackColor,
    // but that obviously won't work if we're transparent.
    internal Color DisabledColor
    {
        get
        {
            Color color = BackColor;
            if (color.A == 0)
            {
                Control? control = ParentInternal;
                while (color.A == 0)
                {
                    if (control is null)
                    {
                        // Don't know what to do, this seems good as anything
                        color = Application.SystemColors.Control;
                        break;
                    }

                    color = control.BackColor;
                    control = control.ParentInternal;
                }
            }

            return color;
        }
    }

    /// <summary>
    ///  Returns the client rect of the display area of the control.
    ///  For the base control class, this is identical to getClientRect.
    ///  However, inheriting controls may want to change this if their client
    ///  area differs from their display area.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlDisplayRectangleDescr))]
    public virtual Rectangle DisplayRectangle
        => new(0, 0, _clientWidth, _clientHeight);

    /// <summary>
    ///  Indicates whether the control has been disposed. This
    ///  property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlDisposedDescr))]
    public bool IsDisposed => GetState(States.Disposed);

    /// <summary>
    ///  Disposes of the currently selected font handle (if cached).
    /// </summary>
    private void DisposeFontHandle()
    {
        if (Properties.ContainsObject(s_fontHandleWrapperProperty))
        {
            if (Properties.GetObject(s_fontHandleWrapperProperty) is FontHandleWrapper fontHandle)
            {
                fontHandle.Dispose();
            }

            Properties.SetObject(s_fontHandleWrapperProperty, null);
        }
    }

    /// <summary>
    ///  Indicates whether the control is in the process of being disposed. This
    ///  property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlDisposingDescr))]
    public bool Disposing => GetState(States.Disposing);

    /// <summary>
    ///  The dock property. The dock property controls to which edge
    ///  of the container this control is docked to. For example, when docked to
    ///  the top of the container, the control will be displayed flush at the
    ///  top of the container, extending the length of the container.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(CommonProperties.DefaultDock)]
    [SRDescription(nameof(SR.ControlDockDescr))]
    public virtual DockStyle Dock
    {
        get => DefaultLayout.GetDock(this);
        set
        {
            if (value != Dock)
            {
                using SuspendLayoutScope scope = new(this);
                DefaultLayout.SetDock(this, value);
                OnDockChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnDockChangedDescr))]
    public event EventHandler? DockChanged
    {
        add => Events.AddHandler(s_dockEvent, value);
        remove => Events.RemoveHandler(s_dockEvent, value);
    }

    /// <summary>
    ///  This will enable or disable double buffering.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ControlDoubleBufferedDescr))]
    protected virtual bool DoubleBuffered
    {
        get => GetStyle(ControlStyles.OptimizedDoubleBuffer);
        set
        {
            if (value != DoubleBuffered)
            {
                if (value)
                {
                    SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, value);
                }
                else
                {
                    SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
                }
            }
        }
    }

    private bool DoubleBufferingEnabled => GetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint);

    /// <summary>
    ///  Indicates whether the control is currently enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DispId(PInvokeCore.DISPID_ENABLED)]
    [SRDescription(nameof(SR.ControlEnabledDescr))]
    public bool Enabled
    {
        get
        {
            // We are only enabled if our parent is enabled
            if (!GetState(States.Enabled))
            {
                return false;
            }
            else if (ParentInternal is null)
            {
                return true;
            }
            else
            {
                return ParentInternal.Enabled;
            }
        }
        set
        {
            bool oldValue = Enabled;
            SetState(States.Enabled, value);

            if (oldValue != value)
            {
                if (!value)
                {
                    SelectNextIfFocused();
                }

                OnEnabledChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the control is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnEnabledChangedDescr))]
    public event EventHandler? EnabledChanged
    {
        add => Events.AddHandler(s_enabledEvent, value);
        remove => Events.RemoveHandler(s_enabledEvent, value);
    }

    /// <summary>
    ///  Indicates whether the control has focus. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlFocusedDescr))]
    public virtual bool Focused
        => IsHandleCreated && PInvoke.GetFocus() == InternalHandle;

    /// <summary>
    ///  Retrieves the current font for this control. This will be the font used
    ///  by default for painting and text in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DispId(PInvokeCore.DISPID_FONT)]
    [AmbientValue(null)]
    [SRDescription(nameof(SR.ControlFontDescr))]
    [AllowNull]
    public virtual Font Font
    {
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ActiveXFontMarshaler))]
        get => GetCurrentFontAndDpi(out _);

        [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ActiveXFontMarshaler))]
        set
        {
            if (Equals((Font?)Properties.GetObject(s_fontProperty), value))
            {
                // Explicitly set font for this control is unchanged, do nothing.
                return;
            }

            Font currentFont = Font;
            Properties.SetObject(s_fontProperty, value);

            // Clear the current cached HFONT, if any.
            DisposeFontHandle();

            // If the value being assigned is the same as the current effective font, we do not need to raise the
            // FontChanged event. Just make sure the WM_SETFONT message is sent.
            if (currentFont.Equals(value))
            {
                if (IsHandleCreated && !GetStyle(ControlStyles.UserPaint))
                {
                    SetWindowFont();
                }

                return;
            }

            if (ScaleHelper.IsThreadPerMonitorV2Aware)
            {
                // Reset the ScaledControlFont value when the font is being set explicitly, in order to keep it
                // in sync when the application is moved between monitors with different Dpi settings.
                ScaledControlFont = null;
                ClearDpiFonts();
            }

            if (Properties.ContainsInteger(s_fontHeightProperty))
            {
                Properties.SetInteger(s_fontHeightProperty, (value is null) ? -1 : value.Height);
            }

            // Font is an ambient property.  We need to layout our parent because Font may
            // change our size.  We need to layout ourselves because our children may change
            // size by inheriting the new value.
            using (new LayoutTransaction(ParentInternal, this, PropertyNames.Font))
            {
                OnFontChanged(EventArgs.Empty);
            }
        }
    }

    internal Font GetScaledFont(Font font, int newDpi, int oldDpi)
    {
        Debug.Assert(
            DpiAwarenessContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2),
            $"Fonts need to be cached only for PerMonitorV2 mode applications : {ScaleHelper.IsThreadPerMonitorV2Aware} : {DpiAwarenessContext}");

        _dpiFonts ??= new Dictionary<int, Font>
        {
            { oldDpi, font.WithSize(font.Size) }
        };

        if (_dpiFonts.TryGetValue(newDpi, out Font? scaledFont))
        {
            return scaledFont!;
        }

        float factor = ((float)newDpi / oldDpi);
        scaledFont = font.WithSize(font.Size * factor);

        _dpiFonts.Add(newDpi, scaledFont);

        return scaledFont;
    }

    private void ClearDpiFonts()
    {
        if (_dpiFonts is null)
        {
            return;
        }

        foreach (Font font in _dpiFonts.Values)
        {
            font?.Dispose();
        }

        _dpiFonts.Clear();
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnFontChangedDescr))]
    public event EventHandler? FontChanged
    {
        add => Events.AddHandler(s_fontEvent, value);
        remove => Events.RemoveHandler(s_fontEvent, value);
    }

    internal HFONT FontHandle
    {
        get
        {
            // If application is in PerMonitorV2 mode and font is scaled when application moved between monitor.
            if (ScaledControlFont is not null)
            {
                _scaledFontWrapper ??= new FontHandleWrapper(ScaledControlFont);
                return _scaledFontWrapper.Handle;
            }

            if (TryGetExplicitlySetFont(out Font? font))
            {
                FontHandleWrapper? fontHandle = (FontHandleWrapper?)Properties.GetObject(s_fontHandleWrapperProperty);
                if (fontHandle is null)
                {
                    fontHandle = new FontHandleWrapper(font);

                    Properties.SetObject(s_fontHandleWrapperProperty, fontHandle);
                }

                return fontHandle.Handle;
            }

            if (_parent is not null)
            {
                return _parent.FontHandle;
            }

            AmbientProperties? ambient = AmbientPropertiesService;

            if (ambient is not null && ambient.Font is not null)
            {
                FontHandleWrapper? fontHandle = null;

                Font? currentAmbient = (Font?)Properties.GetObject(s_currentAmbientFontProperty);

                if (currentAmbient is not null && currentAmbient == ambient.Font)
                {
                    fontHandle = (FontHandleWrapper?)Properties.GetObject(s_fontHandleWrapperProperty);
                }
                else
                {
                    Properties.SetObject(s_currentAmbientFontProperty, ambient.Font);
                }

                if (fontHandle is null)
                {
                    font = ambient.Font;
                    fontHandle = new FontHandleWrapper(font);

                    Properties.SetObject(s_fontHandleWrapperProperty, fontHandle);
                }

                return fontHandle.Handle;
            }

            return GetDefaultFontHandleWrapper().Handle;
        }
    }

    protected int FontHeight
    {
        get
        {
            int fontHeight = Properties.GetInteger(s_fontHeightProperty, out bool found);
            if (found && fontHeight != -1)
            {
                return fontHeight;
            }

            if (TryGetExplicitlySetFont(out Font? font))
            {
                fontHeight = font.Height;
                Properties.SetInteger(s_fontHeightProperty, fontHeight);
                return fontHeight;
            }

            // Ask the parent if it has the font height.
            int localFontHeight = -1;

            if (ParentInternal is not null && ParentInternal.CanAccessProperties)
            {
                localFontHeight = ParentInternal.FontHeight;
            }

            // If we still have a bad value, then get the actual font height.
            if (localFontHeight == -1)
            {
                localFontHeight = Font.Height;
                Properties.SetInteger(s_fontHeightProperty, localFontHeight);
            }

            return localFontHeight;
        }
        set => Properties.SetInteger(s_fontHeightProperty, value);
    }

    /// <summary>
    ///  The foreground color of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DispId(PInvokeCore.DISPID_FORECOLOR)]
    [SRDescription(nameof(SR.ControlForeColorDescr))]
    public virtual Color ForeColor
    {
        get
        {
            Color color = Properties.GetColor(s_foreColorProperty);
            if (!color.IsEmpty)
            {
                return color;
            }

            Control? parent = ParentInternal;
            if (parent is not null && parent.CanAccessProperties)
            {
                return parent.ForeColor;
            }

            Color c = Color.Empty;

            if (IsActiveX)
            {
                c = ActiveXAmbientForeColor;
            }

            if (c.IsEmpty)
            {
                AmbientProperties? ambient = AmbientPropertiesService;
                if (ambient is not null)
                {
                    c = ambient.ForeColor;
                }
            }

            if (!c.IsEmpty)
            {
                return c;
            }
            else
            {
                return DefaultForeColor;
            }
        }

        set
        {
            Color c = ForeColor;
            if (!value.IsEmpty || Properties.ContainsObject(s_foreColorProperty))
            {
                Properties.SetColor(s_foreColorProperty, value);
            }

            if (!c.Equals(ForeColor))
            {
                OnForeColorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnForeColorChangedDescr))]
    public event EventHandler? ForeColorChanged
    {
        add => Events.AddHandler(s_foreColorEvent, value);
        remove => Events.RemoveHandler(s_foreColorEvent, value);
    }

    private Font? GetParentFont(out int fontDpi)
    {
        fontDpi = _deviceDpi;
        if (ParentInternal is not null && ParentInternal.CanAccessProperties)
        {
            return ParentInternal.GetCurrentFontAndDpi(out fontDpi);
        }
        else
        {
            return null;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Size GetPreferredSize(Size proposedSize)
    {
        Size prefSize;

        if (GetState(States.Disposing | States.Disposed))
        {
            // if someone's asking when we're disposing just return what we last had.
            prefSize = CommonProperties.xGetPreferredSizeCache(this);
        }
        else
        {
            // Switch Size.Empty to maximum possible values
            proposedSize = LayoutUtils.ConvertZeroToUnbounded(proposedSize);

            // Force proposedSize to be within the elements constraints.  (This applies
            // minimumSize, maximumSize, etc.)
            proposedSize = ApplySizeConstraints(proposedSize);
            if (GetExtendedState(ExtendedStates.UserPreferredSizeCache))
            {
                Size cachedSize = CommonProperties.xGetPreferredSizeCache(this);

                // If the "default" preferred size is being requested, and we have a cached value for it, return it.
                if (!cachedSize.IsEmpty && (proposedSize == LayoutUtils.s_maxSize))
                {
                    return cachedSize;
                }
            }

            CacheTextInternal = true;
            try
            {
                prefSize = GetPreferredSizeCore(proposedSize);
            }
            finally
            {
                CacheTextInternal = false;
            }

            // There is no guarantee that GetPreferredSizeCore() return something within
            // proposedSize, so we apply the element's constraints again.
            prefSize = ApplySizeConstraints(prefSize);

            // If the "default" preferred size was requested, cache the computed value.
            if (GetExtendedState(ExtendedStates.UserPreferredSizeCache) && proposedSize == LayoutUtils.s_maxSize)
            {
                CommonProperties.xSetPreferredSizeCache(this, prefSize);
            }
        }

        return prefSize;
    }

    // Overriding this method allows us to get the caching and clamping the proposedSize/output to
    // MinimumSize / MaximumSize from GetPreferredSize for free.
    internal virtual Size GetPreferredSizeCore(Size proposedSize)
    {
        return CommonProperties.GetSpecifiedBounds(this).Size;
    }

    /// <summary>
    ///  The HWND handle that this control is bound to. If the handle
    ///  has not yet been created, this will force handle creation.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DispId(PInvokeCore.DISPID_HWND)]
    [SRDescription(nameof(SR.ControlHandleDescr))]
    public IntPtr Handle
    {
        get
        {
            if (CheckForIllegalCrossThreadCalls && !t_inCrossThreadSafeCall && InvokeRequired)
            {
                throw new InvalidOperationException(string.Format(SR.IllegalCrossThreadCall, Name));
            }

            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            return HandleInternal;
        }
    }

    internal IntPtr HandleInternal => _window.Handle;

    internal HWND HWNDInternal => _window.HWND;

    /// <summary>
    ///  True if this control has child controls in its collection.  This
    ///  is more efficient than checking for Controls.Count > 0, but has the
    ///  same effect.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlHasChildrenDescr))]
    public bool HasChildren
    {
        get
        {
            ControlCollection? controls = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
            return controls is not null && controls.Count > 0;
        }
    }

    /// <summary>
    ///  The height of this control
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlHeightDescr))]
    public int Height
    {
        get => _height;
        set => SetBounds(_x, _y, _width, value, BoundsSpecified.Height);
    }

    internal unsafe bool HostedInWin32DialogManager
    {
        get
        {
            if (!GetState(States.CheckedHost))
            {
                Control topMost = TopMostParent;
                if (this != topMost)
                {
                    SetState(States.HostedInDialog, topMost.HostedInWin32DialogManager);
                }
                else
                {
                    HWND parentHandle = PInvoke.GetParent(this);
                    HWND lastParentHandle = parentHandle;
                    SetState(States.HostedInDialog, false);
                    Span<char> buffer = stackalloc char[PInvoke.MaxClassName];

                    while (!parentHandle.IsNull)
                    {
                        int length = 0;
                        fixed (char* lpClassName = buffer)
                        {
                            length = PInvoke.GetClassName(lastParentHandle, lpClassName, buffer.Length);
                        }

                        // #32770 is the standard windows dialog class name
                        // https://learn.microsoft.com/windows/win32/winmsg/about-window-classes#system-classes
                        ReadOnlySpan<char> className = "#32770";
                        if (className.Equals(buffer[..length], StringComparison.Ordinal))
                        {
                            SetState(States.HostedInDialog, true);
                            break;
                        }

                        lastParentHandle = parentHandle;
                        parentHandle = PInvoke.GetParent(parentHandle);
                    }
                }

                SetState(States.CheckedHost, true);
            }

            return GetState(States.HostedInDialog);
        }
    }

    /// <summary>
    ///  Whether or not this control has a handle associated with it.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlHandleCreatedDescr))]
    public bool IsHandleCreated => _window.Handle != 0;

    private protected virtual bool IsHoveredWithMouse() => ClientRectangle.Contains(PointToClient(MousePosition));

    /// <summary>
    ///  Determines if layout is currently suspended.
    /// </summary>
    internal bool IsLayoutSuspended => LayoutSuspendCount > 0;

    internal bool IsWindowObscured
    {
        get
        {
            if (!IsHandleCreated || !Visible)
            {
                return false;
            }

            Control? parent = ParentInternal;
            if (parent is not null)
            {
                while (parent.ParentInternal is not null)
                {
                    parent = parent.ParentInternal;
                }
            }

            PInvoke.GetWindowRect(this, out var temp);
            using Region working = new(temp);

            HWND prev;
            HWND next;
            HWND start = parent is not null ? parent.HWND : HWND;

            for (prev = start;
                !(next = PInvoke.GetWindow(prev, GET_WINDOW_CMD.GW_HWNDPREV)).IsNull;
                prev = next)
            {
                PInvoke.GetWindowRect(next, out temp);
                if (PInvoke.IsWindowVisible(next))
                {
                    working.Exclude(temp);
                }
            }

            using Graphics g = CreateGraphics();
            return working.IsEmpty(g);
        }
    }

    /// <summary>
    ///  Returns the current value of the handle. This may be zero if the handle
    ///  has not been created.
    /// </summary>
    internal HWND InternalHandle => !IsHandleCreated ? default : (HWND)Handle;

    /// <summary>
    ///  Determines if the caller must call invoke when making method
    ///  calls to this control.  Controls in windows forms are bound to a specific thread,
    ///  and are not thread safe.  Therefore, if you are calling a control's method
    ///  from a different thread, you must use the control's invoke method
    ///  to marshal the call to the proper thread.  This function can be used to
    ///  determine if you must call invoke, which can be handy if you don't know
    ///  what thread owns a control.
    ///
    ///  There are five functions on a control that are safe to call from any
    ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and
    ///  CreateGraphics.  For all other method calls, you should use one of the
    ///  invoke methods.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlInvokeRequiredDescr))]
    public bool InvokeRequired
    {
        get
        {
            using var scope = MultithreadSafeCallScope.Create();

            Control control;
            if (IsHandleCreated)
            {
                control = this;
            }
            else
            {
                Control marshalingControl = FindMarshalingControl();

                if (!marshalingControl.IsHandleCreated)
                {
                    return false;
                }

                control = marshalingControl;
            }

            return PInvoke.GetWindowThreadProcessId(control, out _) != PInvoke.GetCurrentThreadId();
        }
    }

    /// <summary>
    ///  Indicates whether or not this control is an accessible control
    ///  i.e. whether it should be visible to accessibility applications.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlIsAccessibleDescr))]
    public bool IsAccessible
    {
        get => GetState(States.IsAccessible);
        set => SetState(States.IsAccessible, value);
    }

    /// <summary>
    ///  Used to tell if this control is being hosted as an ActiveX control.
    /// </summary>
    internal bool IsActiveX => GetExtendedState(ExtendedStates.IsActiveX);

    /// <summary>
    ///  Indicates if one of the Ancestors of this control is sited
    ///  and that site in DesignMode. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsAncestorSiteInDesignMode =>
        GetSitedParentSite(this) is ISite parentSite && parentSite.DesignMode;

    private static ISite? GetSitedParentSite(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);
        return (control.Site is not null && control.Site.DesignMode) || control.Parent is null ?
            control.Site : GetSitedParentSite(control.Parent);
    }

    // If the control on which GetContainerControl( ) is called is a ContainerControl, then we don't return the parent
    // but return the same control. This is Everett behavior so we cannot change this since this would be a breaking change.
    // Hence we have a new internal property IsContainerControl which returns false for all Everett control, but
    // this property is overidden in SplitContainer to return true so that we skip the SplitContainer
    // and the correct Parent ContainerControl is returned by GetContainerControl().
    internal virtual bool IsContainerControl => false;

    /// <summary>
    ///  Used to tell if the control is mirrored
    ///  Don't call this from CreateParams. Will lead to nasty problems
    ///  since we might call CreateParams here - you dig!
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.IsMirroredDescr))]
    public bool IsMirrored
    {
        get
        {
            if (!IsHandleCreated)
            {
                CreateParams cp = CreateParams;
                SetState(States.Mirrored, (cp.ExStyle & (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL) != 0);
            }

            return GetState(States.Mirrored);
        }
    }

    /// <summary>
    ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
    /// </summary>
    internal virtual bool IsMnemonicsListenerAxSourced => false;

    /// <summary>
    ///  Used to tell if this BackColor is Supported
    /// </summary>
    private bool IsValidBackColor(Color c)
    {
        if (!c.IsEmpty && !GetStyle(ControlStyles.SupportsTransparentBackColor) && c.A < 255)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///  Stores information about the last button or combination pressed by the user.
    /// </summary>
    private protected static Keys LastKeyData { get; set; }

    /// <summary>
    ///  The left coordinate of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlLeftDescr))]
    public int Left
    {
        get => _x;
        set => SetBounds(value, _y, _width, _height, BoundsSpecified.X);
    }

    /// <summary>
    ///  The location of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlLocationDescr))]
    public Point Location
    {
        get => new(_x, _y);
        set => SetBounds(value.X, value.Y, _width, _height, BoundsSpecified.Location);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnLocationChangedDescr))]
    public event EventHandler? LocationChanged
    {
        add => Events.AddHandler(s_locationEvent, value);
        remove => Events.RemoveHandler(s_locationEvent, value);
    }

    [SRDescription(nameof(SR.ControlMarginDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    public Padding Margin
    {
        get => CommonProperties.GetMargin(this);
        set
        {
            // This should be done here rather than in the property store as
            // some IArrangedElements actually support negative padding.
            value = LayoutUtils.ClampNegativePaddingToZero(value);

            // SetMargin causes a layout as a side effect.
            if (value != Margin)
            {
                CommonProperties.SetMargin(this, value);
                OnMarginChanged(EventArgs.Empty);
            }

            Debug.Assert(Margin == value, "Error detected while setting Margin.");
        }
    }

    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnMarginChangedDescr))]
    public event EventHandler? MarginChanged
    {
        add => Events.AddHandler(s_marginChangedEvent, value);
        remove => Events.RemoveHandler(s_marginChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlMaximumSizeDescr))]
    [AmbientValue(typeof(Size), "0, 0")]
    public virtual Size MaximumSize
    {
        get { return CommonProperties.GetMaximumSize(this, DefaultMaximumSize); }
        set
        {
            if (value == Size.Empty)
            {
                CommonProperties.ClearMaximumSize(this);
                Debug.Assert(MaximumSize == DefaultMaximumSize, "Error detected while resetting MaximumSize.");
            }
            else if (value != MaximumSize)
            {
                // SetMaximumSize causes a layout as a side effect.
                CommonProperties.SetMaximumSize(this, value);
                Debug.Assert(MaximumSize == value, "Error detected while setting MaximumSize.");
            }
        }
    }

    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlMinimumSizeDescr))]
    public virtual Size MinimumSize
    {
        get { return CommonProperties.GetMinimumSize(this, DefaultMinimumSize); }
        set
        {
            if (value != MinimumSize)
            {
                // SetMinimumSize causes a layout as a side effect.
                CommonProperties.SetMinimumSize(this, value);
            }

            Debug.Assert(MinimumSize == value, "Error detected while setting MinimumSize.");
        }
    }

    /// <summary>
    ///  Retrieves the current state of the modifier keys. This will check the
    ///  current state of the shift, control, and alt keys.
    /// </summary>
    public static Keys ModifierKeys
    {
        get
        {
            Keys modifiers = 0;

            if (PInvoke.GetKeyState((int)Keys.ShiftKey) < 0)
            {
                modifiers |= Keys.Shift;
            }

            if (PInvoke.GetKeyState((int)Keys.ControlKey) < 0)
            {
                modifiers |= Keys.Control;
            }

            if (PInvoke.GetKeyState((int)Keys.Menu) < 0)
            {
                modifiers |= Keys.Alt;
            }

            return modifiers;
        }
    }

    /// <summary>
    ///  The current state of the mouse buttons. This will check the
    ///  current state of the left, right, and middle mouse buttons.
    /// </summary>
    public static MouseButtons MouseButtons
    {
        get
        {
            MouseButtons buttons = default;

            if (PInvoke.GetKeyState((int)Keys.LButton) < 0)
            {
                buttons |= MouseButtons.Left;
            }

            if (PInvoke.GetKeyState((int)Keys.RButton) < 0)
            {
                buttons |= MouseButtons.Right;
            }

            if (PInvoke.GetKeyState((int)Keys.MButton) < 0)
            {
                buttons |= MouseButtons.Middle;
            }

            if (PInvoke.GetKeyState((int)Keys.XButton1) < 0)
            {
                buttons |= MouseButtons.XButton1;
            }

            if (PInvoke.GetKeyState((int)Keys.XButton2) < 0)
            {
                buttons |= MouseButtons.XButton2;
            }

            return buttons;
        }
    }

    /// <summary>
    ///  The current position of the mouse in screen coordinates.
    /// </summary>
    public static Point MousePosition
    {
        get
        {
            PInvoke.GetCursorPos(out Point pt);
            return pt;
        }
    }

    /// <summary>
    ///  Name of this control. The designer will set this to the same
    ///  as the programatic Id "(name)" of the control.  The name can be
    ///  used as a key into the ControlCollection.
    /// </summary>
    [Browsable(false)]
    [AllowNull]
    public string Name
    {
        get
        {
            string? name = (string?)Properties.GetObject(s_namePropertyProperty);
            if (string.IsNullOrEmpty(name))
            {
                if (Site is not null)
                {
                    name = Site.Name;
                }

                name ??= string.Empty;
            }

            return name;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Properties.SetObject(s_namePropertyProperty, null);
            }
            else
            {
                Properties.SetObject(s_namePropertyProperty, value);
            }
        }
    }

    /// <summary>
    ///  The parent of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlParentDescr))]
    public Control? Parent
    {
        get => ParentInternal;
        set => ParentInternal = value;
    }

    internal virtual Control? ParentInternal
    {
        get => _parent;
        set
        {
            if (_parent == value)
            {
                return;
            }

            if (value is not null)
            {
                value.Controls.Add(this);
            }
            else
            {
                _parent?.Controls.Remove(this);
            }
        }
    }

    /// <summary>
    ///  Retrieves the product name of this specific component.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlProductNameDescr))]
    public string ProductName => VersionInfo.ProductName;

    /// <summary>
    ///  Retrieves the product version of this specific component.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlProductVersionDescr))]
    public string ProductVersion => VersionInfo.ProductVersion;

    /// <summary>
    ///  Retrieves our internal property storage object. If you have a property
    ///  whose value is not always set, you should store it in here to save
    ///  space.
    /// </summary>
    internal PropertyStore Properties { get; }

    // Returns the value of the backColor field -- no asking the parent with its color is, etc.
    internal Color RawBackColor => Properties.GetColor(s_backColorProperty);

    /// <summary>
    ///  Indicates whether the control is currently recreating its handle. This
    ///  property is read-only.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlRecreatingHandleDescr))]
    public bool RecreatingHandle => GetState(States.Recreate);

    internal virtual void AddReflectChild()
    {
    }

    internal virtual void RemoveReflectChild()
    {
    }

    internal virtual void RemoveToolTip(ToolTip toolTip)
    {
        // Control doesn't have a specific logic after a toolTip is removed
    }

    private Control? ReflectParent
    {
        get => _reflectParent?.TryGetTarget(out Control? parent) ?? false ? parent : null;
        set
        {
            value?.AddReflectChild();

            Control? existing = ReflectParent;
            _reflectParent = value is not null ? new(value) : null;
            existing?.RemoveReflectChild();
        }
    }

    /// <summary>
    ///  The Region associated with this control.  (defines the
    ///  outline/silhouette/boundary of control)
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlRegionDescr))]
    public Region? Region
    {
        get => (Region?)Properties.GetObject(s_regionProperty);
        set
        {
            Region? oldRegion = Region;
            if (oldRegion != value)
            {
                oldRegion?.Dispose();
                SetRegion(value);
                OnRegionChanged(EventArgs.Empty);
            }
        }
    }

    internal void SetRegion(Region? region)
    {
        Properties.SetObject(s_regionProperty, region);

        if (!IsHandleCreated)
        {
            // We'll get called when OnHandleCreated runs.
            return;
        }

        if (region is null)
        {
            PInvoke.SetWindowRgn(this, default, PInvoke.IsWindowVisible(this));
            return;
        }

        // If we're an ActiveX control, clone the region so it can potentially be modified
        using Region? regionCopy = IsActiveX ? ActiveXMergeRegion(region.Clone()) : null;
        using RegionScope regionHandle = new(regionCopy ?? region, HWND);

        if (PInvoke.SetWindowRgn(this, regionHandle, PInvoke.IsWindowVisible(this)) != 0)
        {
            // Success, the window now owns the region
            regionHandle.RelinquishOwnership();
        }
    }

    /// <summary>
    ///  Event fired when the value of Region property is changed on Control
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlRegionChangedDescr))]
    public event EventHandler? RegionChanged
    {
        add => Events.AddHandler(s_regionChangedEvent, value);
        remove => Events.RemoveHandler(s_regionChangedEvent, value);
    }

    // Helper function for Rtl
    [Obsolete("This property has been deprecated. Please use RightToLeft instead. https://go.microsoft.com/fwlink/?linkid=14202")]
    protected internal bool RenderRightToLeft => true;

    /// <summary>
    ///  Determines if the parent's background will be rendered on the label control.
    /// </summary>
    internal bool RenderTransparent
        => GetStyle(ControlStyles.SupportsTransparentBackColor) && BackColor.A < 255;

    private bool RenderColorTransparent(Color c)
        => GetStyle(ControlStyles.SupportsTransparentBackColor) && c.A < 255;

    /// <summary>
    ///  This property is required by certain controls (TabPage) to render its transparency using theming API.
    ///  We don't want all controls (that are have transparent BackColor) to use theming API to render its background because it has  HUGE PERF cost.
    /// </summary>
    internal virtual bool RenderTransparencyWithVisualStyles => false;

    /// <summary>
    ///  Represents the bounds of the control that need to be scaled.  Control bounds
    ///  need to be scaled until ScaleControl is called.  They need to be scaled again
    ///  if their bounds change after ScaleControl is called.
    /// </summary>
    internal BoundsSpecified RequiredScaling
    {
        get
        {
            if ((_requiredScaling & RequiredScalingEnabledMask) != 0)
            {
                return (BoundsSpecified)(_requiredScaling & RequiredScalingMask);
            }

            return BoundsSpecified.None;
        }
        set
        {
            byte enableBit = (byte)(_requiredScaling & RequiredScalingEnabledMask);
            _requiredScaling = (byte)(((int)value & RequiredScalingMask) | enableBit);
        }
    }

    /// <summary>
    ///  Determines if the required scaling property is enabled.  If not,
    ///  RequiredScaling always returns None.
    /// </summary>
    internal bool RequiredScalingEnabled
    {
        get => (_requiredScaling & RequiredScalingEnabledMask) != 0;
        set
        {
            byte scaling = (byte)(_requiredScaling & RequiredScalingMask);
            _requiredScaling = scaling;
            if (value)
            {
                _requiredScaling |= RequiredScalingEnabledMask;
            }
        }
    }

    /// <summary>
    ///  Indicates whether the control should redraw itself when resized.
    /// </summary>
    [SRDescription(nameof(SR.ControlResizeRedrawDescr))]
    protected bool ResizeRedraw
    {
        get => GetStyle(ControlStyles.ResizeRedraw);
        set => SetStyle(ControlStyles.ResizeRedraw, value);
    }

    /// <summary>
    ///  The right coordinate of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlRightDescr))]
    public int Right => _x + _width;

    /// <summary>
    ///  This is used for international applications where the language
    ///  is written from RightToLeft. When this property is true,
    ///  control placement and text will be from right to left.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [AmbientValue(RightToLeft.Inherit)]
    [SRDescription(nameof(SR.ControlRightToLeftDescr))]
    public virtual RightToLeft RightToLeft
    {
        get
        {
            int rightToLeft = Properties.GetInteger(s_rightToLeftProperty, out bool found);
            if (!found)
            {
                rightToLeft = (int)RightToLeft.Inherit;
            }

            if (((RightToLeft)rightToLeft) == RightToLeft.Inherit)
            {
                Control? parent = ParentInternal;
                if (parent is not null)
                {
                    rightToLeft = (int)parent.RightToLeft;
                }
                else
                {
                    rightToLeft = (int)DefaultRightToLeft;
                }
            }

            return (RightToLeft)rightToLeft;
        }
        set
        {
            // valid values are 0x0 to 0x2.
            SourceGenerated.EnumValidator.Validate(value);

            RightToLeft oldValue = RightToLeft;

            if (Properties.ContainsInteger(s_rightToLeftProperty) || value != RightToLeft.Inherit)
            {
                Properties.SetInteger(s_rightToLeftProperty, (int)value);
            }

            if (oldValue != RightToLeft)
            {
                // Setting RTL on a container does not cause the container to change size.
                // Only the children need to have their layout updated.
                using (new LayoutTransaction(this, this, PropertyNames.RightToLeft))
                {
                    OnRightToLeftChanged(EventArgs.Empty);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftChangedDescr))]
    public event EventHandler? RightToLeftChanged
    {
        add => Events.AddHandler(s_rightToLeftEvent, value);
        remove => Events.RemoveHandler(s_rightToLeftEvent, value);
    }

    /// <summary>
    ///  This property controls the scaling of child controls.  If true child controls
    ///  will be scaled when the Scale method on this control is called.  If false,
    ///  child controls will not be scaled.  The default is true, and you must override
    ///  this property to provide a different value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual bool ScaleChildren => true;

    /// <summary>
    /// Stores scaled font from Dpi changed values. This is required to distinguish the Font change from
    /// Dpi changed events and explicit Font change/assignment. Caching Font values for each Dpi is complex.
    /// ToDo: Look into caching Dpi and control bounds for each Dpi to improve perf.
    /// https://github.com/dotnet/winforms/issues/5047
    /// </summary>
    internal Font? ScaledControlFont
    {
        get => _scaledControlFont;
        set
        {
            _scaledFontWrapper?.Dispose();
            _scaledFontWrapper = null;

            _scaledControlFont = value;
            if (Properties.ContainsInteger(s_fontHeightProperty))
            {
                Properties.SetInteger(s_fontHeightProperty, (value is null) ? -1 : value.Height);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override ISite? Site
    {
        get => base.Site;
        set
        {
            AmbientProperties? oldAmbients = AmbientPropertiesService;
            AmbientProperties? newAmbients = null;

            if (value is not null)
            {
                newAmbients = value.GetService(typeof(AmbientProperties)) as AmbientProperties;
            }

            // If the ambients changed, compare each property.
            if (oldAmbients != newAmbients)
            {
                bool checkFont = !Properties.ContainsObject(s_fontProperty);
                bool checkBackColor = !Properties.ContainsObject(s_backColorProperty);
                bool checkForeColor = !Properties.ContainsObject(s_foreColorProperty);
                bool checkCursor = !Properties.ContainsObject(s_cursorProperty);

                Font? oldFont = null;
                Color oldBackColor = Color.Empty;
                Color oldForeColor = Color.Empty;
                Cursor? oldCursor = null;

                if (checkFont)
                {
                    oldFont = Font;
                }

                if (checkBackColor)
                {
                    oldBackColor = BackColor;
                }

                if (checkForeColor)
                {
                    oldForeColor = ForeColor;
                }

                if (checkCursor)
                {
                    oldCursor = Cursor;
                }

                Properties.SetObject(s_ambientPropertiesServiceProperty, newAmbients);
                base.Site = value;

                if (checkFont && !oldFont!.Equals(Font))
                {
                    OnFontChanged(EventArgs.Empty);
                }

                if (checkForeColor && !oldForeColor.Equals(ForeColor))
                {
                    OnForeColorChanged(EventArgs.Empty);
                }

                if (checkBackColor && !oldBackColor.Equals(BackColor))
                {
                    OnBackColorChanged(EventArgs.Empty);
                }

                if (checkCursor && oldCursor!.Equals(Cursor))
                {
                    OnCursorChanged(EventArgs.Empty);
                }
            }
            else
            {
                // If the ambients haven't changed, we just set a new site.
                base.Site = value;
            }
        }
    }

    /// <summary>
    ///  The size of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlSizeDescr))]
    public Size Size
    {
        get => new(_width, _height);
        set => SetBounds(_x, _y, value.Width, value.Height, BoundsSpecified.Size);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnSizeChangedDescr))]
    public event EventHandler? SizeChanged
    {
        add => Events.AddHandler(s_sizeEvent, value);
        remove => Events.RemoveHandler(s_sizeEvent, value);
    }

    /// <summary>
    ///  The tab index of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [MergableProperty(false)]
    [SRDescription(nameof(SR.ControlTabIndexDescr))]
    public int TabIndex
    {
        get => _tabIndex == -1 ? 0 : _tabIndex;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (_tabIndex != value)
            {
                _tabIndex = value;
                OnTabIndexChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnTabIndexChangedDescr))]
    public event EventHandler? TabIndexChanged
    {
        add => Events.AddHandler(s_tabIndexEvent, value);
        remove => Events.RemoveHandler(s_tabIndexEvent, value);
    }

    /// <summary>
    ///  Indicates whether the user can give the focus to this control using the TAB
    ///  key. This property is read-only.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [DispId(PInvokeCore.DISPID_TABSTOP)]
    [SRDescription(nameof(SR.ControlTabStopDescr))]
    public bool TabStop
    {
        get => TabStopInternal;
        set
        {
            if (TabStop != value)
            {
                TabStopInternal = value;
                if (IsHandleCreated)
                {
                    SetWindowStyle((int)WINDOW_STYLE.WS_TABSTOP, value);
                }

                OnTabStopChanged(EventArgs.Empty);
            }
        }
    }

    // Grab out the logical of setting TABSTOP state, so that derived class could use this.
    internal bool TabStopInternal
    {
        get => (_state & States.TabStop) != 0;
        set
        {
            if (TabStopInternal != value)
            {
                SetState(States.TabStop, value);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnTabStopChangedDescr))]
    public event EventHandler? TabStopChanged
    {
        add => Events.AddHandler(s_tabStopEvent, value);
        remove => Events.RemoveHandler(s_tabStopEvent, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get => Properties.GetObject(s_userDataProperty);
        set => Properties.SetObject(s_userDataProperty, value);
    }

    /// <summary>
    ///  The current text associated with this control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [Bindable(true)]
    [DispId(PInvokeCore.DISPID_TEXT)]
    [SRDescription(nameof(SR.ControlTextDescr))]
    [AllowNull]
    public virtual string Text
    {
        get => CacheTextInternal ? _text ?? string.Empty : WindowText;
        set
        {
            value ??= string.Empty;

            if (value == Text)
            {
                return;
            }

            if (CacheTextInternal)
            {
                _text = value;
            }

            WindowText = value;
            OnTextChanged(EventArgs.Empty);

            if (IsMnemonicsListenerAxSourced)
            {
                for (Control? control = this; control is not null; control = control.ParentInternal)
                {
                    if (control.IsActiveX && control.Properties.GetObject(s_activeXImplProperty) is ActiveXImpl activeX)
                    {
                        activeX.UpdateAccelTable();
                        break;
                    }
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnTextChangedDescr))]
    public event EventHandler? TextChanged
    {
        add => Events.AddHandler(s_textEvent, value);
        remove => Events.RemoveHandler(s_textEvent, value);
    }

    /// <summary>
    ///  Top coordinate of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlTopDescr))]
    public int Top
    {
        get => _y;
        set => SetBounds(_x, value, _width, _height, BoundsSpecified.Y);
    }

    /// <summary>
    ///  The top level control that contains this control. This doesn't
    ///  have to be the same as the value returned from getForm since forms
    ///  can be parented to other controls.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlTopLevelControlDescr))]
    public Control? TopLevelControl => TopLevelControlInternal;

    internal Control? TopLevelControlInternal
    {
        get
        {
            Control? control = this;
            while (control is not null && !control.GetTopLevel())
            {
                control = control.ParentInternal;
            }

            return control;
        }
    }

    internal Control TopMostParent
    {
        get
        {
            Control control = this;
            while (control.ParentInternal is not null)
            {
                control = control.ParentInternal;
            }

            return control;
        }
    }

    // This auto upgraded v1 client to per-process doublebuffering logic
    private static BufferedGraphicsContext BufferContext => BufferedGraphicsManager.Current;

    /// <summary>
    ///  Indicates whether the user interface is in a state to show or hide keyboard
    ///  accelerators. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected internal virtual bool ShowKeyboardCues
    {
        get
        {
            // Controls in design mode always draw their accelerators.
            if (!IsHandleCreated || DesignMode)
            {
                return true;
                // would be nice to query SystemParametersInfo, but have trouble
                // getting this to work and this should not really be called before
                // handle created anyway
            }

            // How this all works

            // uiCuesState contains this control's cached state of whether or not it thinks
            // accelerators/focus cues are turned on. the first 16 bits represent focus cues
            // the second represent keyboard cues.  "F" is the UICuesStates.FocusMask,
            // "F0" is the UICuesStates.KeyboardMask

            // We check here if we have cached state.  If we don't, we need to initialize ourself.
            // We do this by checking "MenuAccessKeysUnderlined" - we show if this returns true.

            // If MenuAccessKeysUnderlined returns false, we have to manually call CHANGEUISTATE on the topmost control
            // Why? Well the way the API seems to work is that it stores in a bit flag for the hidden
            // state.

            // Details from the Menu keydown to changed value of _uiCuesState.

            // When someone does press the ALT (Menu)/F10 key we will
            //   Call ProcessUICues on the control that had focus at the time
            //          ProcessUICues will check the current state of the control using WM_QUERYUISTATE
            //          If WM_QUERYUISTATE indicates that the accelerators are hidden we will
            //                  either call WM_UPDATEUISTATE or WM_CHANGEUISTATE depending on whether we're hosted or not.
            //          All controls in the heirarchy will be individually called back on WM_UPDATEUISTATE, which will go into WmUpdateUIState.
            //   In WmUpdateUIState, we will update our uiCuesState cached value, which
            //   changes the public value of what we return here for ShowKeyboardCues/ShowFocusCues.

            if ((_uiCuesState & UICuesStates.KeyboardMask) == 0)
            {
                if (SystemInformation.MenuAccessKeysUnderlined)
                {
                    _uiCuesState |= UICuesStates.KeyboardShow;
                }
                else
                {
                    // if we're in the hidden state, we need to manufacture an update message so everyone knows it.
                    uint actionMask = PInvoke.UISF_HIDEACCEL << 16;
                    _uiCuesState |= UICuesStates.KeyboardHidden;

                    // The side effect of this initial state is that adding new controls may clear the accelerator
                    // state (has been this way forever)
                    PInvoke.SendMessage(
                        TopMostParent,
                        PInvoke.WM_CHANGEUISTATE,
                        (WPARAM)(actionMask | PInvoke.UIS_SET));
                }
            }

            return (_uiCuesState & UICuesStates.KeyboardMask) == UICuesStates.KeyboardShow;
        }
    }

    /// <summary>
    ///  Indicates whether the user interface is in a state to show or hide focus
    ///  rectangles. This property is read-only.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected internal virtual unsafe bool ShowFocusCues
    {
        get
        {
            if (!IsHandleCreated)
            {
                return true;
                // would be nice to query SystemParametersInfo, but have trouble
                // getting this to work and this should not really be called before
                // handle created anyway
            }

            // See "How this all works" in ShowKeyboardCues

            if ((_uiCuesState & UICuesStates.FocusMask) == 0)
            {
                if (SystemInformation.MenuAccessKeysUnderlined)
                {
                    _uiCuesState |= UICuesStates.FocusShow;
                }
                else
                {
                    _uiCuesState |= UICuesStates.FocusHidden;

                    // if we're in the hidden state, we need to manufacture an update message so everyone knows it.
                    int actionMask = (int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16;

                    // The side effect of this initial state is that adding new controls may clear the focus cue state
                    // state (has been this way forever)
                    PInvoke.SendMessage(TopMostParent,
                        PInvoke.WM_CHANGEUISTATE,
                        (WPARAM)(actionMask | (int)PInvoke.UIS_SET));
                }
            }

            return (_uiCuesState & UICuesStates.FocusMask) == UICuesStates.FocusShow;
        }
    }

    // The parameter used in the call to ShowWindow for this control
    internal virtual SHOW_WINDOW_CMD ShowParams => SHOW_WINDOW_CMD.SW_SHOW;

    /// <summary>
    ///  When this property in true the Cursor Property is set to WaitCursor as well as the Cursor Property
    ///  of all the child controls.
    /// </summary>
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ControlUseWaitCursorDescr))]
    public bool UseWaitCursor
    {
        get => GetState(States.UseWaitCursor);
        set
        {
            if (GetState(States.UseWaitCursor) != value)
            {
                SetState(States.UseWaitCursor, value);
                ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

                if (controlsCollection is not null)
                {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        controlsCollection[i].UseWaitCursor = value;
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This property overrides <see cref="UseCompatibleTextRenderingDefault"/>.
    ///  </para>
    ///  <para>
    ///   Exposed publicly only by controls that support GDI text rendering (<see cref="Label"/>, <see cref="LinkLabel"/>
    ///   and some others).
    ///  </para>
    ///  <para>
    ///   Observe that this property is NOT virtual (to allow for caching the property value - see <see cref="LinkLabel"/>)
    ///   and should be used by controls that support it only (see <see cref="SupportsUseCompatibleTextRendering"/>).
    ///  </para>
    /// </remarks>
    internal bool UseCompatibleTextRenderingInternal
    {
        get
        {
            if (Properties.ContainsInteger(s_useCompatibleTextRenderingProperty))
            {
                int value = Properties.GetInteger(s_useCompatibleTextRenderingProperty, out bool found);
                if (found)
                {
                    return value == 1;
                }
            }

            return UseCompatibleTextRenderingDefault;
        }
        set
        {
            if (SupportsUseCompatibleTextRendering && UseCompatibleTextRenderingInternal != value)
            {
                Properties.SetInteger(s_useCompatibleTextRenderingProperty, value ? 1 : 0);

                // Update the preferred size cache since we will be rendering text using a different engine.
                LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.UseCompatibleTextRendering);
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Determines whether the control supports rendering text using GDI+ and GDI.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is provided for container controls (PropertyGrid) to iterate through its children to set
    ///   <see cref="UseCompatibleTextRenderingInternal"/> to the same value if the child control supports it.
    ///  </para>
    /// </remarks>
    internal virtual bool SupportsUseCompatibleTextRendering => false;

    private ControlVersionInfo VersionInfo
    {
        get
        {
            ControlVersionInfo? info = (ControlVersionInfo?)Properties.GetObject(s_controlVersionInfoProperty);
            if (info is null)
            {
                info = new ControlVersionInfo(this);
                Properties.SetObject(s_controlVersionInfoProperty, info);
            }

            return info;
        }
    }

    /// <summary>
    ///  Indicates whether the control is visible.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlVisibleDescr))]
    public bool Visible
    {
        get
        {
            if (!DesiredVisibility)
            {
                return false;
            }

            // We are only visible if our parent is visible
            return ParentInternal is null || ParentInternal.Visible;
        }
        set => SetVisibleCore(value);
    }

    /// <summary>
    ///  Occurs when the control becomes visible.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnVisibleChangedDescr))]
    public event EventHandler? VisibleChanged
    {
        add => Events.AddHandler(s_visibleEvent, value);
        remove => Events.RemoveHandler(s_visibleEvent, value);
    }

    /// <summary>
    ///  Wait for the wait handle to receive a signal: throw an exception if the thread is no longer with us.
    /// </summary>
    private unsafe void WaitForWaitHandle(WaitHandle waitHandle)
    {
        uint threadId = CreateThreadId;
        Application.ThreadContext? ctx = Application.ThreadContext.FromId(threadId);
        if (ctx is null)
        {
            // Couldn't find the thread context, so we don't know the state.  We shouldn't throw.
            return;
        }

        HANDLE threadHandle = ctx.Handle;
        bool processed = false;

        // setting default exitcode to 0, though it won't be accessed in current code below due to short-circuit logic in condition (returnValue will be false when exitCode is undefined)
        uint exitCode = 0;
        bool returnValue = false;
        while (!processed)
        {
            // Get the thread's exit code, if we found the thread as expected
            if (threadHandle != 0)
            {
                returnValue = PInvoke.GetExitCodeThread(threadHandle, &exitCode);
            }

            // If we didn't find the thread, or if GetExitCodeThread failed, we don't know the thread's state:
            // if we don't know, we shouldn't throw.
            if ((returnValue && exitCode != NTSTATUS.STILL_ACTIVE)
                || (!returnValue && Marshal.GetLastWin32Error() == (int)WIN32_ERROR.ERROR_INVALID_HANDLE)
                || AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                if (waitHandle.WaitOne(1, false))
                {
                    break;
                }

                throw new InvalidAsynchronousStateException(SR.ThreadNoLongerValid);
            }

            if (IsDisposed && _threadCallbackList is not null && _threadCallbackList.Count > 0)
            {
                lock (_threadCallbackList)
                {
                    Exception ex = new ObjectDisposedException(GetType().Name);
                    while (_threadCallbackList.Count > 0)
                    {
                        ThreadMethodEntry entry = _threadCallbackList.Dequeue();
                        entry._exception = ex;
                        entry.Complete();
                    }
                }
            }

            processed = waitHandle.WaitOne(1000, false);
        }
    }

    /// <summary>
    ///  The width of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlWidthDescr))]
    public int Width
    {
        get => _width;
        set => SetBounds(_x, _y, value, _height, BoundsSpecified.Width);
    }

    /// <summary>
    ///  The current exStyle of the hWnd
    /// </summary>
    private protected WINDOW_EX_STYLE ExtendedWindowStyle
    {
        get => (WINDOW_EX_STYLE)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        set => PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (nint)value);
    }

    /// <summary>
    ///  The current style of the hWnd
    /// </summary>
    internal WINDOW_STYLE WindowStyle
    {
        get => (WINDOW_STYLE)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        set => PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (nint)value);
    }

    /// <summary>
    ///  The target of Win32 window messages.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ControlWindowTargetDescr))]
    public IWindowTarget WindowTarget
    {
        get => _window.WindowTarget;
        set => _window.WindowTarget = value;
    }

    /// <summary>
    ///  The current text of the Window; if the window has not yet been created, stores it in the control.
    ///  If the window has been created, stores the text in the underlying win32 control.
    ///  This property should be used whenever you want to get at the win32 control's text. For all other cases,
    ///  use the Text property - but note that this is overridable, and any of your code that uses it will use
    ///  the overridden version in controls that subclass your own.
    /// </summary>
    internal virtual string WindowText
    {
        get
        {
            if (!IsHandleCreated)
            {
                return _text ?? string.Empty;
            }

            using var scope = MultithreadSafeCallScope.Create();
            return PInvoke.GetWindowText(this);
        }
        set
        {
            value ??= string.Empty;

            if (!WindowText.Equals(value))
            {
                if (IsHandleCreated)
                {
                    PInvoke.SetWindowText(this, value);
                }
                else
                {
                    _text = value.Length == 0 ? null : value;
                }
            }
        }
    }

    /// <summary>
    ///  Occurs when the control is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnClickDescr))]
    public event EventHandler? Click
    {
        add => Events.AddHandler(s_clickEvent, value);
        remove => Events.RemoveHandler(s_clickEvent, value);
    }

    /// <summary>
    ///  Occurs when a new control is added.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlOnControlAddedDescr))]
    public event ControlEventHandler? ControlAdded
    {
        add => Events.AddHandler(s_controlAddedEvent, value);
        remove => Events.RemoveHandler(s_controlAddedEvent, value);
    }

    /// <summary>
    ///  Occurs when a control is removed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlOnControlRemovedDescr))]
    public event ControlEventHandler? ControlRemoved
    {
        add => Events.AddHandler(s_controlRemovedEvent, value);
        remove => Events.RemoveHandler(s_controlRemovedEvent, value);
    }

    /// <summary>
    ///  Occurs when the value of the <see cref="DataContext"/> property changes.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlDataContextChangedDescr))]
    public event EventHandler? DataContextChanged
    {
        add => Events.AddHandler(s_dataContextEvent, value);
        remove => Events.RemoveHandler(s_dataContextEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ControlOnDragDropDescr))]
    public event DragEventHandler? DragDrop
    {
        add => Events.AddHandler(s_dragDropEvent, value);
        remove => Events.RemoveHandler(s_dragDropEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ControlOnDragEnterDescr))]
    public event DragEventHandler? DragEnter
    {
        add => Events.AddHandler(s_dragEnterEvent, value);
        remove => Events.RemoveHandler(s_dragEnterEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ControlOnDragOverDescr))]
    public event DragEventHandler? DragOver
    {
        add => Events.AddHandler(s_dragOverEvent, value);
        remove => Events.RemoveHandler(s_dragOverEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ControlOnDragLeaveDescr))]
    public event EventHandler? DragLeave
    {
        add => Events.AddHandler(s_dragLeaveEvent, value);
        remove => Events.RemoveHandler(s_dragLeaveEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ControlOnGiveFeedbackDescr))]
    public event GiveFeedbackEventHandler? GiveFeedback
    {
        add => Events.AddHandler(s_giveFeedbackEvent, value);
        remove => Events.RemoveHandler(s_giveFeedbackEvent, value);
    }

    /// <summary>
    ///  Occurs when a handle is created for the control.
    /// </summary>
    [SRCategory(nameof(SR.CatPrivate))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlOnCreateHandleDescr))]
    public event EventHandler? HandleCreated
    {
        add => Events.AddHandler(s_handleCreatedEvent, value);
        remove => Events.RemoveHandler(s_handleCreatedEvent, value);
    }

    /// <summary>
    ///  Occurs when the control's handle is destroyed.
    /// </summary>
    [SRCategory(nameof(SR.CatPrivate))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlOnDestroyHandleDescr))]
    public event EventHandler? HandleDestroyed
    {
        add => Events.AddHandler(s_handleDestroyedEvent, value);
        remove => Events.RemoveHandler(s_handleDestroyedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ControlOnHelpDescr))]
    public event HelpEventHandler? HelpRequested
    {
        add => Events.AddHandler(s_helpRequestedEvent, value);
        remove => Events.RemoveHandler(s_helpRequestedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.ControlOnInvalidateDescr))]
    public event InvalidateEventHandler? Invalidated
    {
        add => Events.AddHandler(s_invalidatedEvent, value);
        remove => Events.RemoveHandler(s_invalidatedEvent, value);
    }

    [Browsable(false)]
    public Size PreferredSize
    {
        get { return GetPreferredSize(Size.Empty); }
    }

    [SRDescription(nameof(SR.ControlPaddingDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    public Padding Padding
    {
        get { return CommonProperties.GetPadding(this, DefaultPadding); }
        set
        {
            if (value != Padding)
            {
                CommonProperties.SetPadding(this, value);
                // Ideally we are being laid out by a LayoutEngine that cares about our preferred size.
                // We set our LAYOUTISDIRTY bit and ask our parent to refresh us.
                SetState(States.LayoutIsDirty, true);
                using (new LayoutTransaction(ParentInternal, this, PropertyNames.Padding))
                {
                    OnPaddingChanged(EventArgs.Empty);
                }

                if (GetState(States.LayoutIsDirty))
                {
                    // The above did not cause our layout to be refreshed.  We explicitly refresh our
                    // layout to ensure that any children are repositioned to account for the change
                    // in padding.
                    LayoutTransaction.DoLayout(this, this, PropertyNames.Padding);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnPaddingChangedDescr))]
    public event EventHandler? PaddingChanged
    {
        add => Events.AddHandler(s_paddingChangedEvent, value);
        remove => Events.RemoveHandler(s_paddingChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ControlOnPaintDescr))]
    public event PaintEventHandler? Paint
    {
        add => Events.AddHandler(s_paintEvent, value);
        remove => Events.RemoveHandler(s_paintEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ControlOnQueryContinueDragDescr))]
    public event QueryContinueDragEventHandler? QueryContinueDrag
    {
        add => Events.AddHandler(s_queryContinueDragEvent, value);
        remove => Events.RemoveHandler(s_queryContinueDragEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ControlOnQueryAccessibilityHelpDescr))]
    public event QueryAccessibilityHelpEventHandler? QueryAccessibilityHelp
    {
        add => Events.AddHandler(s_queryAccessibilityHelpEvent, value);
        remove => Events.RemoveHandler(s_queryAccessibilityHelpEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is double clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
    public event EventHandler? DoubleClick
    {
        add => Events.AddHandler(s_doubleClickEvent, value);
        remove => Events.RemoveHandler(s_doubleClickEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is entered.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlOnEnterDescr))]
    public event EventHandler? Enter
    {
        add => Events.AddHandler(s_enterEvent, value);
        remove => Events.RemoveHandler(s_enterEvent, value);
    }

    /// <summary>
    ///  Occurs when the control receives focus.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlOnGotFocusDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler? GotFocus
    {
        add => Events.AddHandler(s_gotFocusEvent, value);
        remove => Events.RemoveHandler(s_gotFocusEvent, value);
    }

    /// <summary>
    ///  Occurs when a key is pressed down while the control has focus.
    /// </summary>
    [SRCategory(nameof(SR.CatKey))]
    [SRDescription(nameof(SR.ControlOnKeyDownDescr))]
    public event KeyEventHandler? KeyDown
    {
        add => Events.AddHandler(s_keyDownEvent, value);
        remove => Events.RemoveHandler(s_keyDownEvent, value);
    }

    /// <summary>
    ///  Occurs when a key is pressed while the control has focus.
    /// </summary>
    [SRCategory(nameof(SR.CatKey))]
    [SRDescription(nameof(SR.ControlOnKeyPressDescr))]
    public event KeyPressEventHandler? KeyPress
    {
        add => Events.AddHandler(s_keyPressEvent, value);
        remove => Events.RemoveHandler(s_keyPressEvent, value);
    }

    /// <summary>
    ///  Occurs when a key is released while the control has focus.
    /// </summary>
    [SRCategory(nameof(SR.CatKey))]
    [SRDescription(nameof(SR.ControlOnKeyUpDescr))]
    public event KeyEventHandler? KeyUp
    {
        add => Events.AddHandler(s_keyUpEvent, value);
        remove => Events.RemoveHandler(s_keyUpEvent, value);
    }

    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnLayoutDescr))]
    public event LayoutEventHandler? Layout
    {
        add => Events.AddHandler(s_layoutEvent, value);
        remove => Events.RemoveHandler(s_layoutEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is left.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlOnLeaveDescr))]
    public event EventHandler? Leave
    {
        add => Events.AddHandler(s_leaveEvent, value);
        remove => Events.RemoveHandler(s_leaveEvent, value);
    }

    /// <summary>
    ///  Occurs when the control loses focus.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlOnLostFocusDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler? LostFocus
    {
        add => Events.AddHandler(s_lostFocusEvent, value);
        remove => Events.RemoveHandler(s_lostFocusEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is mouse clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnMouseClickDescr))]
    public event MouseEventHandler? MouseClick
    {
        add => Events.AddHandler(s_mouseClickEvent, value);
        remove => Events.RemoveHandler(s_mouseClickEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is mouse double clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnMouseDoubleClickDescr))]
    public event MouseEventHandler? MouseDoubleClick
    {
        add => Events.AddHandler(s_mouseDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_mouseDoubleClickEvent, value);
    }

    /// <summary>
    ///  Occurs when the control loses mouse Capture.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnMouseCaptureChangedDescr))]
    public event EventHandler? MouseCaptureChanged
    {
        add => Events.AddHandler(s_mouseCaptureChangedEvent, value);
        remove => Events.RemoveHandler(s_mouseCaptureChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer is over the control and a mouse button is
    ///  pressed.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseDownDescr))]
    public event MouseEventHandler? MouseDown
    {
        add => Events.AddHandler(s_mouseDownEvent, value);
        remove => Events.RemoveHandler(s_mouseDownEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer enters the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseEnterDescr))]
    public event EventHandler? MouseEnter
    {
        add => Events.AddHandler(s_mouseEnterEvent, value);
        remove => Events.RemoveHandler(s_mouseEnterEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer leaves the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseLeaveDescr))]
    public event EventHandler? MouseLeave
    {
        add => Events.AddHandler(s_mouseLeaveEvent, value);
        remove => Events.RemoveHandler(s_mouseLeaveEvent, value);
    }

    /// <summary>
    ///  Occurs when the Dpi resolution of the screen this control is displayed on changes,
    ///  either when the top level window is moved between monitors or when the OS settings are changed.
    ///  This event is raised before the top level parent window receives WM_DPICHANGED message.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnDpiChangedBeforeParentDescr))]
    public event EventHandler? DpiChangedBeforeParent
    {
        add => Events.AddHandler(s_dpiChangedBeforeParentEvent, value);
        remove => Events.RemoveHandler(s_dpiChangedBeforeParentEvent, value);
    }

    /// <summary>
    ///  Occurs when the Dpi resolution of the screen this control is displayed on changes,
    ///  either when the top level window is moved between monitors or when the OS settings are changed.
    ///  This message is received after the top level parent window receives WM_DPICHANGED message.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnDpiChangedAfterParentDescr))]
    public event EventHandler? DpiChangedAfterParent
    {
        add => Events.AddHandler(s_dpiChangedAfterParentEvent, value);
        remove => Events.RemoveHandler(s_dpiChangedAfterParentEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer hovers over the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseHoverDescr))]
    public event EventHandler? MouseHover
    {
        add => Events.AddHandler(s_mouseHoverEvent, value);
        remove => Events.RemoveHandler(s_mouseHoverEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer is moved over the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseMoveDescr))]
    public event MouseEventHandler? MouseMove
    {
        add => Events.AddHandler(s_mouseMoveEvent, value);
        remove => Events.RemoveHandler(s_mouseMoveEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer is over the control and a mouse button is released.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseUpDescr))]
    public event MouseEventHandler? MouseUp
    {
        add => Events.AddHandler(s_mouseUpEvent, value);
        remove => Events.RemoveHandler(s_mouseUpEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse wheel moves while the control has focus.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseWheelDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event MouseEventHandler? MouseWheel
    {
        add => Events.AddHandler(s_mouseWheelEvent, value);
        remove => Events.RemoveHandler(s_mouseWheelEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is moved.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnMoveDescr))]
    public event EventHandler? Move
    {
        add => Events.AddHandler(s_moveEvent, value);
        remove => Events.RemoveHandler(s_moveEvent, value);
    }

    /// <summary>
    ///  Raised to preview a key down event
    /// </summary>
    [SRCategory(nameof(SR.CatKey))]
    [SRDescription(nameof(SR.PreviewKeyDownDescr))]
    public event PreviewKeyDownEventHandler? PreviewKeyDown
    {
        add => Events.AddHandler(s_previewKeyDownEvent, value);
        remove => Events.RemoveHandler(s_previewKeyDownEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is resized.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ControlOnResizeDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler? Resize
    {
        add => Events.AddHandler(s_resizeEvent, value);
        remove => Events.RemoveHandler(s_resizeEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ControlOnChangeUICuesDescr))]
    public event UICuesEventHandler? ChangeUICues
    {
        add => Events.AddHandler(s_changeUICuesEvent, value);
        remove => Events.RemoveHandler(s_changeUICuesEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ControlOnStyleChangedDescr))]
    public event EventHandler? StyleChanged
    {
        add => Events.AddHandler(s_styleChangedEvent, value);
        remove => Events.RemoveHandler(s_styleChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ControlOnSystemColorsChangedDescr))]
    public event EventHandler? SystemColorsChanged
    {
        add => Events.AddHandler(s_systemColorsChangedEvent, value);
        remove => Events.RemoveHandler(s_systemColorsChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is validating.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlOnValidatingDescr))]
    public event CancelEventHandler? Validating
    {
        add => Events.AddHandler(s_validatingEvent, value);
        remove => Events.RemoveHandler(s_validatingEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is done validating.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.ControlOnValidatedDescr))]
    public event EventHandler? Validated
    {
        add => Events.AddHandler(s_validatedEvent, value);
        remove => Events.RemoveHandler(s_validatedEvent, value);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal void AccessibilityNotifyClients(AccessibleEvents accEvent, int childID)
    {
        AccessibilityNotifyClients(accEvent, (int)OBJECT_IDENTIFIER.OBJID_CLIENT, childID);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void AccessibilityNotifyClients(AccessibleEvents accEvent, int objectID, int childID)
    {
        if (IsHandleCreated && !LocalAppContextSwitches.NoClientNotifications)
        {
            PInvoke.NotifyWinEvent((uint)accEvent, this, objectID, childID + 1);
        }
    }

    /// <summary>
    ///  Assigns a new parent control. Sends out the appropriate property change
    ///  notifications for properties that are affected by the change of parent.
    /// </summary>
    internal virtual void AssignParent(Control? value)
    {
        // Adopt the parent's required scaling bits
        if (value is not null)
        {
            RequiredScalingEnabled = value.RequiredScalingEnabled;
        }

        if (CanAccessProperties)
        {
            // Store the old values for these properties
            Font oldFont = Font;
            Color oldForeColor = ForeColor;
            Color oldBackColor = BackColor;
            RightToLeft oldRtl = RightToLeft;
            bool oldEnabled = Enabled;
            bool oldVisible = Visible;

            // Update the parent
            _parent = value;
            OnParentChanged(EventArgs.Empty);

            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            // Compare property values with new parent to old values
            if (oldEnabled != Enabled)
            {
                OnEnabledChanged(EventArgs.Empty);
            }

            // When a control seems to be going from invisible -> visible,
            // yet its parent is being set to null and it's not top level, do not raise OnVisibleChanged.
            bool newVisible = Visible;

            if (oldVisible != newVisible && !(!oldVisible && newVisible && _parent is null && !GetTopLevel()))
            {
                OnVisibleChanged(EventArgs.Empty);
            }

            if (!oldFont.Equals(Font))
            {
                OnFontChanged(EventArgs.Empty);
            }

            if (!oldForeColor.Equals(ForeColor))
            {
                OnForeColorChanged(EventArgs.Empty);
            }

            if (!oldBackColor.Equals(BackColor))
            {
                OnBackColorChanged(EventArgs.Empty);
            }

            if (oldRtl != RightToLeft)
            {
                OnRightToLeftChanged(EventArgs.Empty);
            }

            if (!Properties.ContainsObjectThatIsNotNull(s_bindingManagerProperty) && Created)
            {
                // We do not want to call our parent's BindingContext property here.
                // We have no idea if us or any of our children are using data binding,
                // and invoking the property would just create the binding manager, which
                // we don't need.  We just blindly notify that the binding manager has
                // changed, and if anyone cares, they will do the comparison at that time.
                OnBindingContextChanged(EventArgs.Empty);
            }
        }
        else
        {
            _parent = value;
            OnParentChanged(EventArgs.Empty);
        }

        SetState(States.CheckedHost, false);

        _forceAnchorCalculations = LocalAppContextSwitches.AnchorLayoutV2; // Parent has changed. AnchorsInfo should be recalculated.
        try
        {
            ParentInternal?.LayoutEngine.InitLayout(this, BoundsSpecified.All);
        }
        finally
        {
            _forceAnchorCalculations = false;
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnParentChangedDescr))]
    public event EventHandler? ParentChanged
    {
        add => Events.AddHandler(s_parentEvent, value);
        remove => Events.RemoveHandler(s_parentEvent, value);
    }

    /// <summary>
    ///  Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
    /// </summary>
    /// <param name="method">A delegate to a method that takes no parameters.</param>
    /// <returns>
    ///  An <see cref="IAsyncResult"/> that represents the result of the <see cref="BeginInvoke(Delegate)"/> operation.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IAsyncResult BeginInvoke(Delegate method) => BeginInvoke(method, null);

    /// <summary>
    ///  Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
    /// </summary>
    /// <param name="method">A delegate to a method that takes no parameters.</param>
    /// <returns>
    ///  An <see cref="IAsyncResult"/> that represents the result of the <see cref="BeginInvoke(Action)"/> operation.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IAsyncResult BeginInvoke(Action method) => BeginInvoke(method, null);

    /// <summary>
    ///  Executes the given delegate on the thread that owns this Control's
    ///  underlying window handle.  The delegate is called asynchronously and this
    ///  method returns immediately.  You may call this from any thread, even the
    ///  thread that owns the control's handle.  If the control's handle doesn't
    ///  exist yet, this will follow up the control's parent chain until it finds a
    ///  control or form that does have a window handle.  If no appropriate handle
    ///  can be found, BeginInvoke will throw an exception.  Exceptions within the
    ///  delegate method are considered untrapped and will be sent to the
    ///  application's untrapped exception handler.
    ///
    ///  There are five functions on a control that are safe to call from any
    ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
    ///  For all other method calls, you should use one of the invoke methods to marshal
    ///  the call to the control's thread.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IAsyncResult BeginInvoke(Delegate method, params object?[]? args)
    {
        using var scope = MultithreadSafeCallScope.Create();
        Control marshaler = FindMarshalingControl();
        return (IAsyncResult)marshaler.MarshaledInvoke(this, method, args, synchronous: false);
    }

    internal void BeginUpdateInternal()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        if (_updateCount == 0)
        {
            PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)false);
        }

        _updateCount++;
    }

    /// <summary>
    ///  Brings this control to the front of the zorder.
    /// </summary>
    public void BringToFront()
    {
        if (_parent is not null)
        {
            _parent.Controls.SetChildIndex(this, 0);
        }
        else if (IsHandleCreated && GetTopLevel() && PInvoke.IsWindowEnabled(this))
        {
            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOP,
                0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
        }
    }

    /// <summary>
    ///  Specifies whether this control can process the mnemonic or not.  A condition to process a mnemonic is that
    ///  all controls in the parent chain can do it too, but since the semantics for this function can be overriden,
    ///  we need to call the method on the parent 'recursively' (not exactly since it is not necessarily the same method).
    /// </summary>
    internal virtual bool CanProcessMnemonic()
    {
        if (!Enabled || !Visible)
        {
            return false;
        }

        if (_parent is not null)
        {
            return _parent.CanProcessMnemonic();
        }

        return true;
    }

    // Package scope to allow AxHost to override
    internal virtual bool CanSelectCore()
    {
        if ((_controlStyle & ControlStyles.Selectable) != ControlStyles.Selectable)
        {
            return false;
        }

        for (Control? ctl = this; ctl is not null; ctl = ctl._parent)
        {
            if (!ctl.Enabled || !ctl.Visible)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Searches the parent/owner tree for bottom to find any instance
    ///  of toFind in the parent/owner tree.
    /// </summary>
    internal static void CheckParentingCycle(Control? bottom, Control? toFind)
    {
        Form? lastOwner = null;
        Control? lastParent = null;

        for (Control? ctl = bottom; ctl is not null; ctl = ctl.ParentInternal)
        {
            lastParent = ctl;
            if (ctl == toFind)
            {
                throw new ArgumentException(SR.CircularOwner);
            }
        }

        if (lastParent is not null)
        {
            if (lastParent is Form f)
            {
                for (Form? form = f; form is not null; form = form.OwnerInternal)
                {
                    lastOwner = form;
                    if (form == toFind)
                    {
                        throw new ArgumentException(SR.CircularOwner);
                    }
                }
            }
        }

        if (lastOwner is not null)
        {
            if (lastOwner.ParentInternal is not null)
            {
                CheckParentingCycle(lastOwner.ParentInternal, toFind);
            }
        }
    }

    private void ChildGotFocus(Control child)
    {
        if (IsActiveX)
        {
            ActiveXOnFocus(true);
        }

        _parent?.ChildGotFocus(child);
    }

    /// <summary>
    ///  Verifies if a control is a child of this control.
    /// </summary>
    public bool Contains([NotNullWhen(true)] Control? ctl)
    {
        while (ctl is not null)
        {
            ctl = ctl.ParentInternal;
            if (ctl is null)
            {
                return false;
            }

            if (ctl == this)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Constructs a new instance of the accessibility object for this control. Subclasses
    ///  should not call base.CreateAccessibilityObject.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual AccessibleObject CreateAccessibilityInstance()
    {
        return new ControlAccessibleObject(this);
    }

    /// <summary>
    ///  Constructs the new instance of the Controls collection objects. Subclasses
    ///  should not call base.CreateControlsInstance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual ControlCollection CreateControlsInstance()
    {
        return new ControlCollection(this);
    }

    /// <summary>
    ///  Creates a Graphics for this control. The control's brush, font, foreground
    ///  color and background color become the default values for the Graphics.
    ///  The returned Graphics must be disposed through a call to its dispose()
    ///  method when it is no longer needed.  The Graphics Object is only valid for
    ///  the duration of the current window's message.
    /// </summary>
    public Graphics CreateGraphics()
    {
        using var scope = MultithreadSafeCallScope.Create();
        return CreateGraphicsInternal();
    }

    internal Graphics CreateGraphicsInternal()
    {
        return Graphics.FromHwndInternal(Handle);
    }

    /// <summary>
    ///  Creates a handle for this control. This method is called by the framework, this should
    ///  not be called directly. Inheriting classes should always call <c>base.CreateHandle()</c> when
    ///  overriding this method.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void CreateHandle()
    {
        ObjectDisposedException.ThrowIf(GetState(States.Disposed), this);

        if (GetState(States.CreatingHandle))
        {
            return;
        }

        Rectangle originalBounds;

        try
        {
            SetState(States.CreatingHandle, true);

            originalBounds = Bounds;

            // Activate theming scope to get theming for controls at design time and when hosted in browser.
            // NOTE: If a theming context is already active, this call is very fast, so shouldn't be a perf issue.
            using ThemingScope scope = new(Application.UseVisualStyles);

            CreateParams cp = CreateParams;
            SetState(States.Mirrored, (cp.ExStyle & (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL) != 0);

            // Adjust for scrolling of parent.
            if (_parent is not null)
            {
                Rectangle parentClient = _parent.ClientRectangle;

                if (!parentClient.IsEmpty)
                {
                    if (cp.X != PInvoke.CW_USEDEFAULT)
                    {
                        cp.X -= parentClient.X;
                    }

                    if (cp.Y != PInvoke.CW_USEDEFAULT)
                    {
                        cp.Y -= parentClient.Y;
                    }
                }
            }

            // And if we are WS_CHILD, ensure we have a parent handle.
            if (cp.Parent == IntPtr.Zero && (cp.Style & (int)WINDOW_STYLE.WS_CHILD) != 0)
            {
                Debug.Assert((cp.ExStyle & (int)WINDOW_EX_STYLE.WS_EX_MDICHILD) == 0, "Can't put MDI child forms on the parking form");
                Application.ParkHandle(cp, DpiAwarenessContext);
            }

            _window.CreateHandle(cp);

            UpdateReflectParent();
        }
        finally
        {
            SetState(States.CreatingHandle, false);
        }

        // For certain controls (e.g., ComboBox) CreateWindowEx
        // may cause the control to resize.  WM_SETWINDOWPOSCHANGED takes care of
        // the control being resized, but our layout container may need a refresh as well.
        if (Bounds != originalBounds)
        {
            LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
        }
    }

    /// <summary>
    ///  Forces the creation of the control. This includes the creation of the handle,
    ///  and any child controls.
    /// </summary>
    public void CreateControl()
    {
        bool controlIsAlreadyCreated = Created;
        CreateControl(ignoreVisible: false);

        if (!Properties.ContainsObjectThatIsNotNull(s_bindingManagerProperty) && ParentInternal is not null && !controlIsAlreadyCreated)
        {
            // We do not want to call our parent's BindingContext property here.
            // We have no idea if us or any of our children are using data binding,
            // and invoking the property would just create the binding manager, which
            // we don't need.  We just blindly notify that the binding manager has
            // changed, and if anyone cares, they will do the comparison at that time.
            OnBindingContextChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Forces the creation of the control if it is visible. This includes the creation of the handle,
    ///  and any child controls.
    /// </summary>
    /// <param name="ignoreVisible">
    ///  When <see langword="true"/> create even if the control is not visible.
    /// </param>
    internal void CreateControl(bool ignoreVisible)
    {
        // Unless specified otherwise, only "create" the control if it is visible for performance. This has the
        // effect of delayed handle creation of hidden controls.
        if (!ignoreVisible && (GetState(States.Created) || !Visible))
        {
            return;
        }

        SetState(States.Created, true);
        bool createdOK = false;
        try
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            if (Properties.GetObject(s_controlsCollectionProperty) is ControlCollection controlsCollection)
            {
                // Snapshot this array because z-order updates from Windows may rearrange it.
                Control[] controlSnapshot = new Control[controlsCollection.Count];
                controlsCollection.CopyTo(controlSnapshot, 0);

                foreach (Control control in controlSnapshot)
                {
                    if (control.IsHandleCreated)
                    {
                        control.SetParentHandle(HWND);
                    }

                    control.CreateControl(ignoreVisible);
                }
            }

            createdOK = true;
        }
        finally
        {
            if (!createdOK)
            {
                SetState(States.Created, false);
            }
        }

        OnCreateControl();
    }

    /// <summary>
    ///  Sends the specified message to the default window procedure.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DefWndProc(ref Message m) => _window.DefWndProc(ref m);

    /// <summary>
    ///  Destroys the handle associated with this control. Inheriting classes should
    ///  always call base.destroyHandle.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DestroyHandle()
    {
        if (RecreatingHandle && _threadCallbackList is not null)
        {
            // See if we have a thread marshaling request pending.  If so, we will need to
            // re-post it after recreating the handle.
            lock (_threadCallbackList)
            {
                if (s_threadCallbackMessage != 0)
                {
                    MSG msg = default;
                    BOOL result = PInvoke.PeekMessage(
                        &msg,
                        this,
                        (uint)s_threadCallbackMessage,
                        (uint)s_threadCallbackMessage,
                        PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE);

                    if (result)
                    {
                        SetState(States.ThreadMarshalPending, true);
                    }
                }
            }
        }

        // If we're not recreating the handle, then any items in the thread callback list will
        // be orphaned.  An orphaned item is bad, because it will cause the thread to never
        // wake up.  So, we put exceptions into all these items and wake up all threads.
        // If we are recreating the handle, then we're fine because recreation will re-post
        // the thread callback message to the new handle for us.
        if (!RecreatingHandle && _threadCallbackList is not null)
        {
            lock (_threadCallbackList)
            {
                Exception ex = new ObjectDisposedException(GetType().Name);

                while (_threadCallbackList.Count > 0)
                {
                    ThreadMethodEntry entry = _threadCallbackList.Dequeue();
                    entry._exception = ex;
                    entry.Complete();
                }
            }
        }

        if (((WINDOW_EX_STYLE)PInvoke.GetWindowLong(_window, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE))
            .HasFlag(WINDOW_EX_STYLE.WS_EX_MDICHILD))
        {
            PInvoke.DefMDIChildProc(InternalHandle, PInvoke.WM_CLOSE, default, default);
        }
        else
        {
            _window.DestroyHandle();
        }

        _trackMouseEvent = default;
    }

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the <see cref="Control"/>.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (GetState(States.OwnCtlBrush))
        {
            object? backBrush = Properties.GetObject(s_backBrushProperty);
            if (backBrush is not null)
            {
                HBRUSH p = (HBRUSH)backBrush;
                if (!p.IsNull)
                {
                    PInvokeCore.DeleteObject(p);
                }

                Properties.SetObject(s_backBrushProperty, value: null);
            }
        }

        ReflectParent = null;

        if (disposing)
        {
            if (GetState(States.Disposing))
            {
                return;
            }

            if (GetState(States.CreatingHandle))
            {
                throw new InvalidOperationException(string.Format(SR.ClosingWhileCreatingHandle, "Dispose"));
                // I imagine most subclasses will get themselves in a half disposed state
                // if this exception is thrown, but things will be equally broken if we ignore this error,
                // and this way at least the user knows what they did wrong.
            }

            SetState(States.Disposing, true);
            SuspendLayout();
            try
            {
                Properties.SetObject(s_ncAccessibilityProperty, value: null);

                DisposeAxControls();
                ((ActiveXImpl?)Properties.GetObject(s_activeXImplProperty))?.Dispose();

                ResetBindings();

                if (IsHandleCreated)
                {
                    DestroyHandle();
                }

                _parent?.Controls.Remove(this);

                ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

                if (controlsCollection is not null)
                {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        Control ctl = controlsCollection[i];
                        ctl._parent = null;
                        ctl.Dispose();
                    }

                    Properties.SetObject(s_controlsCollectionProperty, null);
                }

                ClearDpiFonts();
                base.Dispose(disposing);
            }
            finally
            {
                ResumeLayout(false);
                SetState(States.Disposing, false);
                SetState(States.Disposed, true);
            }
        }
        else
        {
            // This same post is done in NativeWindow's finalize method, so if you change
            // it, change it there too.
            _window?.ForceExitMessageLoop();

            base.Dispose(disposing);
        }
    }

    // Package scope to allow AxHost to override.
    internal virtual void DisposeAxControls()
    {
        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].DisposeAxControls();
            }
        }
    }

    /// <summary>
    ///  Begins a drag operation. The allowedEffects determine which
    ///  drag operations can occur. If the drag operation needs to interop
    ///  with applications in another process, data should either be
    ///  a base managed class (String, Bitmap, or Metafile) or some Object
    ///  that implements System.Runtime.Serialization.ISerializable. data can also be any Object that
    ///  implements System.Windows.Forms.IDataObject.
    /// </summary>
    public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
    {
        return DoDragDrop(data, allowedEffects, dragImage: null, cursorOffset: default, useDefaultDragImage: false);
    }

    /// <summary>
    ///  Begins a drag operation. The <paramref name="allowedEffects"/> determine which drag operations can occur. If the drag operation
    ///  needs to interop with applications in another process, <paramref name="data"/> should either be a base managed class
    ///  (<see cref="string"/>, <see cref="Bitmap"/>, or <see cref="Drawing.Imaging.Metafile"/>) or some <see cref="object"/> that implements
    ///  <see cref="Runtime.Serialization.ISerializable"/>. <paramref name="data"/> can also be any <see cref="object"/> that implements
    ///  <see cref="IDataObject"/>. <paramref name="dragImage"/> is the bitmap that will be displayed during the  drag operation and
    ///  <paramref name="cursorOffset"/> specifies the location of the cursor within <paramref name="dragImage"/>, which is an offset from the
    ///  upper-left corner. Specify <see langword="true"/> for <paramref name="useDefaultDragImage"/> to use a layered window drag image with a
    ///  size of 96x96; otherwise <see langword="false"/>. Note the outer edges of <paramref name="dragImage"/> are blended out if the image width
    ///  or height exceeds 300 pixels.
    /// </summary>
    /// <returns>
    ///  A value from the <see cref="DragDropEffects"/> enumeration that represents the final effect that was performed during the drag-and-drop
    ///  operation.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   Because <see cref="DoDragDrop(object, DragDropEffects, Bitmap, Point, bool)"/> always performs the RGB multiplication step in calculating
    ///   the alpha value, you should always pass a <see cref="Bitmap"/> without premultiplied alpha blending. Note that no error will result from
    ///   passing a <see cref="Bitmap"/> with premultiplied alpha blending, but this method will multiply it again, doubling the resulting alpha
    ///   value.
    ///  </para>
    /// </remarks>
    public DragDropEffects DoDragDrop(
        object data,
        DragDropEffects allowedEffects,
        Bitmap? dragImage,
        Point cursorOffset,
        bool useDefaultDragImage)
    {
        ComTypes.IDataObject dataObject = CreateRuntimeDataObjectForDrag(data);

        DROPEFFECT finalEffect;

        try
        {
            using var dropSource = ComHelpers.GetComScope<IDropSource>(
                new DropSource(this, dataObject, dragImage, cursorOffset, useDefaultDragImage));
            using var dataScope = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
            if (PInvoke.DoDragDrop(dataScope, dropSource, (DROPEFFECT)(uint)allowedEffects, out finalEffect).Failed)
            {
                return DragDropEffects.None;
            }
        }
        finally
        {
            if (DragDropHelper.IsInDragLoop(dataObject))
            {
                DragDropHelper.SetInDragLoop(dataObject, inDragLoop: false);
            }
        }

        return (DragDropEffects)finalEffect;
    }

    /// <summary>
    ///  Creates <see cref="DataObject"/> for drag operation.
    ///  The incoming <paramref name="data"/> will always be wrapped.
    /// </summary>
    private static DataObject CreateRuntimeDataObjectForDrag(object data) =>
        data is DataObject dataObject ? dataObject : new DataObject(data);

    public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        if (targetBounds.Width <= 0 || targetBounds.Height <= 0
            || targetBounds.X < 0 || targetBounds.Y < 0)
        {
            throw new ArgumentException(message: null, nameof(targetBounds));
        }

        if (!IsHandleCreated)
        {
            CreateHandle();
        }

        int width = Math.Min(Width, targetBounds.Width);
        int height = Math.Min(Height, targetBounds.Height);

        using Bitmap image = new(width, height, bitmap.PixelFormat);
        using Graphics g = Graphics.FromImage(image);
        using DeviceContextHdcScope hDc = new(g, applyGraphicsState: false);

        // Send the WM_PRINT message.
        PInvoke.SendMessage(
            this,
            PInvoke.WM_PRINT,
            (WPARAM)hDc,
            (LPARAM)(PInvoke.PRF_CHILDREN | PInvoke.PRF_CLIENT | PInvoke.PRF_ERASEBKGND | PInvoke.PRF_NONCLIENT));

        // Now BLT the result to the destination bitmap.
        using Graphics destGraphics = Graphics.FromImage(bitmap);
        using DeviceContextHdcScope desthDC = new(destGraphics, applyGraphicsState: false);
        PInvokeCore.BitBlt(
            desthDC,
            targetBounds.X,
            targetBounds.Y,
            width,
            height,
            hDc,
            0,
            0,
            ROP_CODE.SRCCOPY);
    }

    /// <summary>
    ///  Retrieves the return value of the asynchronous operation
    ///  represented by the IAsyncResult interface passed. If the
    ///  async operation has not been completed, this function will
    ///  block until the result is available.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? EndInvoke(IAsyncResult asyncResult)
    {
        using var scope = MultithreadSafeCallScope.Create();
        ArgumentNullException.ThrowIfNull(asyncResult);

        if (asyncResult is not ThreadMethodEntry entry)
        {
            throw new ArgumentException(SR.ControlBadAsyncResult, nameof(asyncResult));
        }

        Debug.Assert(this == entry._caller, "Called BeginInvoke on one control, and the corresponding EndInvoke on a different control");

        if (!asyncResult.IsCompleted)
        {
            Control marshaler = FindMarshalingControl();
            if (PInvoke.GetWindowThreadProcessId(marshaler, out _) == PInvoke.GetCurrentThreadId())
            {
                marshaler.InvokeMarshaledCallbacks();
            }
            else
            {
                marshaler = entry._marshaler;
                marshaler.WaitForWaitHandle(asyncResult.AsyncWaitHandle);
            }
        }

        Debug.Assert(asyncResult.IsCompleted, "Why isn't this asyncResult done yet?");
        if (entry._exception is not null)
        {
            throw entry._exception;
        }

        return entry._retVal;
    }

    internal bool EndUpdateInternal()
    {
        return EndUpdateInternal(true);
    }

    internal bool EndUpdateInternal(bool invalidate)
    {
        if (_updateCount > 0)
        {
            Debug.Assert(IsHandleCreated, "Handle should be created by now");
            _updateCount--;
            if (_updateCount == 0)
            {
                PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)true);
                if (invalidate)
                {
                    Invalidate();
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  Retrieves the form that this control is on. The control's parent may not be the same as the form.
    /// </summary>
    public Form? FindForm()
    {
        Control? current = this;
        while (current is not null and not Form)
        {
            current = current.ParentInternal;
        }

        return (Form?)current;
    }

    /// <summary>
    ///  Attempts to find a control Object that we can use to marshal
    ///  calls.  We must marshal calls to a control with a window
    ///  handle, so we traverse up the parent chain until we find one.
    ///  Failing that, we just return ourselves.
    /// </summary>
    private Control FindMarshalingControl()
    {
        lock (this)
        {
            Control? c = this;

            while (c is not null && !c.IsHandleCreated)
            {
                Control? p = c.ParentInternal;
                c = p;
            }

            if (c is null)
            {
                // No control with a created handle.  We
                // just use our own control.  MarshaledInvoke
                // will throw an exception because there
                // is no handle.
                c = this;
            }
            else
            {
                Debug.Assert(c.IsHandleCreated, "FindMarshalingControl chose a bad control.");
            }

            return c;
        }
    }

    protected bool GetTopLevel() => (_state & States.TopLevel) != 0;

    /// <summary>
    ///  Used by AxHost to fire the CreateHandle event.
    /// </summary>
    internal void RaiseCreateHandleEvent(EventArgs e)
    {
        ((EventHandler?)Events[s_handleCreatedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the event associated with key with the event data of
    ///  e and a sender of this control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void RaiseKeyEvent(object key, KeyEventArgs e)
    {
        ((KeyEventHandler?)Events[key])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the event associated with key with the event data of
    ///  e and a sender of this control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void RaiseMouseEvent(object key, MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[key])?.Invoke(this, e);
    }

    /// <summary>
    ///  Attempts to set focus to this control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool Focus()
    {
        // Call the internal method (which form overrides)
        return FocusInternal();
    }

    /// <summary>
    ///  Internal method for setting focus to the control.
    ///  Form overrides this method - because MDI child forms
    ///  need to be focused by calling the MDIACTIVATE message.
    /// </summary>
    private protected virtual bool FocusInternal()
    {
        if (CanFocus)
        {
            PInvoke.SetFocus(this);
        }

        if (Focused && ParentInternal is not null)
        {
            IContainerControl? control = ParentInternal.GetContainerControl();

            if (control is not null)
            {
                if (control is ContainerControl containerControl)
                {
                    containerControl.SetActiveControl(this);
                }
                else
                {
                    control.ActiveControl = this;
                }
            }
        }

        return Focused;
    }

    /// <summary>
    ///  Returns the control that is currently associated with handle.
    ///  This method will search up the HWND parent chain until it finds some
    ///  handle that is associated with with a control. This method is more
    ///  robust that fromHandle because it will correctly return controls
    ///  that own more than one handle.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Control? FromChildHandle(IntPtr handle)
    {
        HWND hwnd = (HWND)handle;
        while (!hwnd.IsNull)
        {
            Control? control = FromHandle(hwnd);
            if (control is not null)
            {
                return control;
            }

            hwnd = PInvoke.GetAncestor(hwnd, GET_ANCESTOR_FLAGS.GA_PARENT);
        }

        return null;
    }

    /// <summary>
    ///  Creates a <see cref="HandleRef{THandle}"/> for the given <paramref name="hwnd"/>, associating
    ///  it with the first parent <see cref="Control"/> if possible.
    /// </summary>
    internal static HandleRef<HWND> GetHandleRef(HWND hwnd) => new(FromChildHandle(hwnd), hwnd);

    /// <summary>
    ///  Returns the control that is currently associated with handle.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Control? FromHandle(IntPtr handle)
    {
        NativeWindow? nativeWindow = NativeWindow.FromHandle(handle);
        while (nativeWindow is not null and not ControlNativeWindow)
        {
            nativeWindow = nativeWindow.PreviousWindow;
        }

        return nativeWindow is ControlNativeWindow controlNativeWindow ? controlNativeWindow.GetControl() : null;
    }

    // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
    // constraints on their size.
    internal Size ApplySizeConstraints(int width, int height)
    {
        return ApplyBoundsConstraints(0, 0, width, height).Size;
    }

    // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
    // constraints on their size.
    internal Size ApplySizeConstraints(Size proposedSize)
    {
        return ApplyBoundsConstraints(0, 0, proposedSize.Width, proposedSize.Height).Size;
    }

    internal virtual Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
    {
        // COMPAT: in Everett we would allow you to set negative values in pre-handle mode
        // in Whidbey, if you've set Min/Max size we will constrain you to 0,0.  Everett apps didnt
        // have min/max size on control, which is why this works.
        if (MaximumSize != Size.Empty || MinimumSize != Size.Empty)
        {
            Size maximumSize = LayoutUtils.ConvertZeroToUnbounded(MaximumSize);
            Rectangle newBounds = new(suggestedX, suggestedY, 0, 0)
            {
                // Clip the size to maximum and inflate it to minimum as necessary.
                Size = LayoutUtils.IntersectSizes(new Size(proposedWidth, proposedHeight), maximumSize)
            };
            newBounds.Size = LayoutUtils.UnionSizes(newBounds.Size, MinimumSize);

            return newBounds;
        }

        return new Rectangle(suggestedX, suggestedY, proposedWidth, proposedHeight);
    }

    /// <summary>
    ///  Retrieves the child control that is located at the specified client coordinates.
    /// </summary>
    public Control? GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
    {
        int value = (int)skipValue;

        // Since this is a flags enumeration the only way to validate skipValue is by checking if its within the range.
        if (value is < 0 or > 7)
        {
            throw new InvalidEnumArgumentException(nameof(skipValue), value, typeof(GetChildAtPointSkip));
        }

        HWND hwnd = PInvoke.ChildWindowFromPointEx(this, pt, (CWP_FLAGS)value);
        Control? control = FromChildHandle(hwnd);

        return (control == this) ? null : control;
    }

    private protected virtual string? GetCaptionForTool(ToolTip toolTip) =>
        ToolStripControlHost is IKeyboardToolTip host
            ? host.GetCaptionForTool(toolTip)
            : toolTip.GetCaptionForTool(this);

    /// <summary>
    ///  Retrieves the child control that is located at the specified client coordinates.
    /// </summary>
    public Control? GetChildAtPoint(Point pt) => GetChildAtPoint(pt, GetChildAtPointSkip.None);

    /// <summary>
    ///  Returns the closest ContainerControl in the control's chain of parent controls and forms.
    /// </summary>
    public IContainerControl? GetContainerControl()
    {
        Control? c = this;

        // Refer to IsContainerControl property for more details.
        if (c is not null && IsContainerControl)
        {
            c = c.ParentInternal;
        }

        while (c is not null && !IsFocusManagingContainerControl(c))
        {
            c = c.ParentInternal;
        }

        return (IContainerControl?)c;
    }

    private static bool IsFocusManagingContainerControl(Control ctl)
    {
        return ((ctl._controlStyle & ControlStyles.ContainerControl) == ControlStyles.ContainerControl && ctl is IContainerControl);
    }

    /// <summary>
    ///  This new Internal method checks the updateCount to signify that the control is within the "BeginUpdate" and "EndUpdate" cycle.
    ///  Check out : for usage of this. The Treeview tries to ForceUpdate the scrollbars by calling "WM_SETREDRAW"
    ///  even if the control in "Begin - End" update cycle. Using this Function we can guard against repetitively redrawing the control.
    /// </summary>
    internal bool IsUpdating()
    {
        return _updateCount > 0;
    }

    // Essentially an Hfont; see inner class for details.
    private static FontHandleWrapper GetDefaultFontHandleWrapper()
    {
        s_defaultFontHandleWrapper ??= new FontHandleWrapper(DefaultFont);

        return s_defaultFontHandleWrapper;
    }

    /// <summary>
    ///  This is a helper method that is called by ScaleControl to retrieve the bounds
    ///  that the control should be scaled by.  You may override this method if you
    ///  wish to reuse ScaleControl's scaling logic but you need to supply your own
    ///  bounds.  The default implementation returns scaled bounds that take into
    ///  account the BoundsSpecified, whether the control is top level, and whether
    ///  the control is fixed width or auto size, and any adornments the control may have.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
    {
        float dx = factor.Width;
        float dy = factor.Height;

        int sx = bounds.X;
        int sy = bounds.Y;

        // Don't reposition top level controls.  Also, if we're in
        // design mode, don't reposition the root component.
        bool scaleLoc = !GetState(States.TopLevel);
        if (scaleLoc)
        {
            ISite? site = Site;
            if (site is not null && site.DesignMode)
            {
                if (site.GetService(typeof(IDesignerHost)) is IDesignerHost host && host.RootComponent == this)
                {
                    scaleLoc = false;
                }
            }
        }

        if (scaleLoc)
        {
            if ((specified & BoundsSpecified.X) != 0)
            {
                sx = (int)Math.Round(bounds.X * dx);
            }

            if ((specified & BoundsSpecified.Y) != 0)
            {
                sy = (int)Math.Round(bounds.Y * dy);
            }
        }

        int sw = bounds.Width;
        int sh = bounds.Height;

        // We should not include the window adornments in our calculation,
        // because windows scales them for us.
        RECT adornmentsBeforeDpiChange = default;
        RECT adornmentsAfterDpiChange = default;
        CreateParams cp = CreateParams;

        // We would need to get adornments metrics for both (old and new) Dpi in case application is in PerMonitorV2 mode and Dpi changed.
        AdjustWindowRectExForControlDpi(ref adornmentsAfterDpiChange, (WINDOW_STYLE)cp.Style, bMenu: false, (WINDOW_EX_STYLE)cp.ExStyle);

        if (_oldDeviceDpi != _deviceDpi && OsVersion.IsWindows10_1703OrGreater())
        {
            AdjustWindowRectExForDpi(ref adornmentsBeforeDpiChange, (WINDOW_STYLE)cp.Style, bMenu: false, (WINDOW_EX_STYLE)cp.ExStyle, _oldDeviceDpi);
        }
        else
        {
            adornmentsBeforeDpiChange = adornmentsAfterDpiChange;
        }

        // Do this even for auto sized controls.  They'll "snap back", but it is important to size them in case
        // they are anchored.
        if ((_controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth && (specified & BoundsSpecified.Width) != 0)
        {
            int localWidth = bounds.Width - adornmentsBeforeDpiChange.Width;
            sw = (int)Math.Round(localWidth * dx) + adornmentsAfterDpiChange.Width;
        }

        if ((_controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight && (specified & BoundsSpecified.Height) != 0)
        {
            int localHeight = bounds.Height - adornmentsBeforeDpiChange.Height;
            sh = (int)Math.Round(localHeight * dy) + adornmentsAfterDpiChange.Height;
        }

        return new Rectangle(sx, sy, sw, sh);
    }

    private static MouseButtons GetXButton(int wparam) => wparam switch
    {
        PInvoke.XBUTTON1 => MouseButtons.XButton1,
        PInvoke.XBUTTON2 => MouseButtons.XButton2,
        _ => MouseButtons.None,
    };

    internal bool DesiredVisibility => GetState(States.Visible);

    internal bool GetAnyDisposingInHierarchy()
    {
        Control? up = this;
        bool isDisposing = false;
        while (up is not null)
        {
            if (up.Disposing)
            {
                isDisposing = true;
                break;
            }

            up = up._parent;
        }

        return isDisposing;
    }

    /// <summary>
    ///  Returns native child windows sorted according to their TabIndex property order. Controls with the same
    ///  TabIndex remain in original relative child index order (= z-order). Child windows with no corresponding
    ///  Control objects (and therefore no discernable TabIndex) are sorted to the front of the list (but remain
    ///  in relative z-order to one another).
    ///
    ///  This version returns a sorted array of integers, representing the original z-order based indexes of the
    ///  native child windows.
    /// </summary>
    private int[] GetChildWindowsInTabOrder()
    {
        List<ControlTabOrderHolder> holders = [];

        for (HWND hWndChild = PInvoke.GetWindow(this, GET_WINDOW_CMD.GW_CHILD);
            !hWndChild.IsNull;
            hWndChild = PInvoke.GetWindow(new HandleRef<HWND>(this, hWndChild), GET_WINDOW_CMD.GW_HWNDNEXT))
        {
            Control? ctl = FromHandle(hWndChild);
            int tabIndex = (ctl is null) ? -1 : ctl.TabIndex;
            holders.Add(new ControlTabOrderHolder(holders.Count, tabIndex, ctl));
        }

        holders.Sort(ControlTabOrderComparer.Instance);

        int[] indexes = new int[holders.Count];
        for (int i = 0; i < holders.Count; i++)
        {
            indexes[i] = holders[i].OriginalIndex;
        }

        return indexes;
    }

    /// <summary>
    ///  Returns child controls sorted according to their TabIndex property order. Controls with the same TabIndex
    ///  remain in original relative child index order (= z-order).
    ///
    ///  This version returns a sorted array of control references.
    /// </summary>
    internal Control[] GetChildControlsInTabOrder(bool handleCreatedOnly)
    {
        List<ControlTabOrderHolder> holders = new(Controls.Count);

        foreach (Control c in Controls)
        {
            if (!handleCreatedOnly || c.IsHandleCreated)
            {
                holders.Add(new ControlTabOrderHolder(holders.Count, c.TabIndex, c));
            }
        }

        holders.Sort(ControlTabOrderComparer.Instance);

        Control[] controls = new Control[holders.Count];
        for (int i = 0; i < holders.Count; i++)
        {
            controls[i] = holders[i].Control!;
        }

        return controls;
    }

    internal virtual Control? GetFirstChildControlInTabOrder(bool forward)
    {
        ControlCollection? controls = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

        if (controls is null)
        {
            return null;
        }

        Control? found = null;
        if (forward)
        {
            for (int c = 0; c < controls.Count; c++)
            {
                if (found is null || found._tabIndex > controls[c]._tabIndex)
                {
                    found = controls[c];
                }
            }
        }
        else
        {
            // Cycle through the controls in reverse z-order looking for the one with the highest
            // tab index.
            for (int c = controls.Count - 1; c >= 0; c--)
            {
                if (found is null || found._tabIndex < controls[c]._tabIndex)
                {
                    found = controls[c];
                }
            }
        }

        return found;
    }

    /// <summary>
    ///  Gets the control <see cref="Font"/>. If the font is inherited, traverse through the parent hierarchy and
    ///  retrieve the font.
    /// </summary>
    /// <param name="fontDpi">Dpi of the control for which <see cref="Font"/> is evaluated.</param>
    /// <returns>The control's <see cref="Font"/></returns>
    internal Font GetCurrentFontAndDpi(out int fontDpi)
    {
        fontDpi = _deviceDpi;

        // If application is in PerMonitorV2 mode and font is scaled when moved between monitors.
        if (ScaledControlFont is not null)
        {
            return ScaledControlFont;
        }

        if (TryGetExplicitlySetFont(out Font? font))
        {
            return font;
        }

        font = GetParentFont(out fontDpi);
        if (font is not null)
        {
            return font;
        }

        if (IsActiveX)
        {
            font = ActiveXAmbientFont;
            if (font is not null)
            {
                return font;
            }
        }

        return AmbientPropertiesService?.Font ?? DefaultFont;
    }

    private protected virtual IList<Rectangle> GetNeighboringToolsRectangles()
        => ((IKeyboardToolTip?)ToolStripControlHost)?.GetNeighboringToolsRectangles() ?? GetOwnNeighboringToolsRectangles();

    /// <summary>
    ///  Retrieves the next control in the tab order of child controls.
    /// </summary>
    public Control? GetNextControl(Control? ctl, bool forward)
    {
        if (!Contains(ctl))
        {
            ctl = this;
        }

        if (forward)
        {
            ControlCollection? controls = (ControlCollection?)ctl.Properties.GetObject(s_controlsCollectionProperty);

            if (controls is not null && controls.Count > 0 && (ctl == this || !IsFocusManagingContainerControl(ctl)))
            {
                Control? found = ctl.GetFirstChildControlInTabOrder(forward: true);
                if (found is not null)
                {
                    return found;
                }
            }

            while (ctl != this)
            {
                int targetIndex = ctl!._tabIndex;
                bool hitCtl = false;
                Control? found = null;
                Control? p = ctl._parent;

                // Cycle through the controls in z-order looking for the one with the next highest
                // tab index.  Because there can be dups, we have to start with the existing tab index and
                // remember to exclude the current control.
                int parentControlCount = 0;

                ControlCollection? parentControls = (ControlCollection?)p?.Properties.GetObject(s_controlsCollectionProperty);

                if (parentControls is not null)
                {
                    parentControlCount = parentControls.Count;
                }

                for (int c = 0; c < parentControlCount; c++)
                {
                    // The logic for this is a bit lengthy, so I have broken it into separate
                    // clauses:

                    // We are not interested in ourself.
                    if (parentControls![c] != ctl)
                    {
                        // We are interested in controls with >= tab indexes to ctl.  We must include those
                        // controls with equal indexes to account for duplicate indexes.
                        if (parentControls[c]._tabIndex >= targetIndex)
                        {
                            // Check to see if this control replaces the "best match" we've already
                            // found.
                            if (found is null || found._tabIndex > parentControls[c]._tabIndex)
                            {
                                // Finally, check to make sure that if this tab index is the same as ctl,
                                // that we've already encountered ctl in the z-order.  If it isn't the same,
                                // than we're more than happy with it.
                                if (parentControls[c]._tabIndex != targetIndex || hitCtl)
                                {
                                    found = parentControls[c];
                                }
                            }
                        }
                    }
                    else
                    {
                        // We track when we have encountered "ctl".  We never want to select ctl again, but
                        // we want to know when we've seen it in case we find another control with the same tab index.
                        hitCtl = true;
                    }
                }

                if (found is not null)
                {
                    return found;
                }

                ctl = ctl._parent;
            }
        }
        else
        {
            if (ctl != this)
            {
                int targetIndex = ctl._tabIndex;
                bool hitCtl = false;
                Control? found = null;
                Control? parent = ctl._parent ?? throw new InvalidOperationException(
                    string.Format(SR.ParentPropertyNotSetInGetNextControl, nameof(Parent), ctl));

                ControlCollection? siblings = GetControlCollection(parent) ?? throw new InvalidOperationException(
                    string.Format(SR.ControlsPropertyNotSetInGetNextControl, nameof(Controls), parent));

                int siblingCount = siblings.Count;

                if (siblingCount == 0)
                {
                    throw new InvalidOperationException(string.Format(
                        SR.ControlsCollectionShouldNotBeEmptyInGetNextControl,
                        nameof(Controls),
                        parent));
                }

                // Cycle through the controls in reverse z-order looking for the next lowest tab index.  We must
                // start with the same tab index as ctl, because there can be dups.
                for (int c = siblingCount - 1; c >= 0; c--)
                {
                    Control sibling = siblings[c];
                    // The logic for this is a bit lengthy, so I have broken it into separate
                    // clauses:

                    // We are not interested in ourself.
                    if (sibling != ctl)
                    {
                        // We are interested in controls with <= tab indexes to ctl.  We must include those
                        // controls with equal indexes to account for duplicate indexes.
                        if (sibling._tabIndex <= targetIndex)
                        {
                            // Check to see if this control replaces the "best match" we've already
                            // found.
                            if (found is null || found._tabIndex < sibling._tabIndex)
                            {
                                // Finally, check to make sure that if this tab index is the same as ctl,
                                // that we've already encountered ctl in the z-order.  If it isn't the same,
                                // than we're more than happy with it.
                                if (sibling._tabIndex != targetIndex || hitCtl)
                                {
                                    found = sibling;
                                }
                            }
                        }
                    }
                    else
                    {
                        // We track when we have encountered "ctl".  We never want to select ctl again, but
                        // we want to know when we've seen it in case we find another control with the same tab index.
                        hitCtl = true;
                    }
                }

                // If we were unable to find a control we should return the control's parent.  However, if that parent is us, return
                // NULL.
                if (found is not null)
                {
                    ctl = found;
                }
                else
                {
                    if (parent == this)
                    {
                        return null;
                    }
                    else
                    {
                        // If we don't found any siblings, and the control is a ToolStripItem that hosts a control itself,
                        // then we shouldn't return its parent, because it would be the same ToolStrip we're currently at.
                        // Instead, we should return the control that is previous to the current ToolStrip
                        if (ctl.ToolStripControlHost is not null)
                        {
                            return GetNextControl(ctl._parent, forward: false);
                        }

                        return parent;
                    }
                }
            }

            // We found a control.  Walk into this control to find the proper child control within it to select.
            ControlCollection? children = GetControlCollection(ctl);

            while (children is not null && children.Count > 0 && (ctl == this || !IsFocusManagingContainerControl(ctl)))
            {
                Control? found = ctl.GetFirstChildControlInTabOrder(forward: false);
                if (found is not null)
                {
                    ctl = found;
                    children = GetControlCollection(ctl);
                }
                else
                {
                    break;
                }
            }
        }

        return ctl == this ? null : ctl;

        static ControlCollection? GetControlCollection(Control control)
           => (ControlCollection?)control.Properties.GetObject(s_controlsCollectionProperty);
    }

    /// <summary>
    ///  Return <see cref="Handle"/> if <paramref name="window"/> is a <see cref="Control"/>.
    ///  Otherwise, returns <see cref="IWin32Window.Handle"/> after validating the handle is valid.
    /// </summary>
    internal static HandleRef<HWND> GetSafeHandle(IWin32Window window)
    {
        Debug.Assert(window is not null, "window is null in Control.GetSafeHandle");
        if (window is Control control)
        {
            return new(control);
        }
        else
        {
            HWND hwnd = (HWND)window.Handle;
            if (hwnd.IsNull || PInvoke.IsWindow(hwnd))
            {
                return new(window, hwnd);
            }
            else
            {
                throw new Win32Exception((int)WIN32_ERROR.ERROR_INVALID_HANDLE);
            }
        }
    }

    /// <summary>
    ///  Retrieves the current value of the specified bit in the control's state.
    /// </summary>
    private protected bool GetState(States flag) => (_state & flag) != 0;

    /// <summary>
    ///  Retrieves the current value of the specified bit in the control's state2.
    /// </summary>
    private protected bool GetExtendedState(ExtendedStates flag) => (_extendedState & flag) != 0;

    /// <summary>
    ///  Retrieves the current value of the specified bit in the control's style.
    ///  This is control style, not the Win32 style of the hWnd.
    /// </summary>
    protected bool GetStyle(ControlStyles flag) => (_controlStyle & flag) == flag;

    /// <summary>
    ///  Hides the control by setting the visible property to false.
    /// </summary>
    public void Hide()
    {
        Visible = false;
    }

    /// <summary>
    ///  Sets up the TrackMouseEvent for listening for the mouse leave event.
    /// </summary>
    private void HookMouseEvent()
    {
        if (!GetState(States.TrackingMouseEvent))
        {
            SetState(States.TrackingMouseEvent, true);

            _trackMouseEvent = new()
            {
                cbSize = (uint)sizeof(TRACKMOUSEEVENT),
                dwFlags = TRACKMOUSEEVENT_FLAGS.TME_LEAVE | TRACKMOUSEEVENT_FLAGS.TME_HOVER,
                hwndTrack = HWND,
                dwHoverTime = 100
            };

            PInvoke.TrackMouseEvent(ref _trackMouseEvent);
        }
    }

    /// <summary>
    ///  Called after the control has been added to another container.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void InitLayout()
    {
        LayoutEngine.InitLayout(this, BoundsSpecified.All);
    }

    /// <summary>
    ///  This method initializes the scaling bits for this control based on the bounds.
    /// </summary>
    private void InitScaling(BoundsSpecified specified)
    {
        _requiredScaling |= (byte)((int)specified & RequiredScalingMask);
    }

    /// <summary>
    ///  Sets the text and background colors of the DC, and returns the background HBRUSH.
    /// </summary>
    internal virtual HBRUSH InitializeDCForWmCtlColor(HDC dc, MessageId msg)
    {
        // NOTE: this message may not have originally been sent to this HWND.
        if (!GetStyle(ControlStyles.UserPaint))
        {
            PInvoke.SetTextColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(ForeColor));
            PInvoke.SetBkColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(BackColor));
            return BackColorBrush;
        }

        return (HBRUSH)PInvokeCore.GetStockObject(GET_STOCK_OBJECT_FLAGS.NULL_BRUSH);
    }

    /// <summary>
    ///  Invalidates a region of the control and causes a paint message
    ///  to be sent to the control. This will not force a synchronous paint to
    ///  occur, calling update after invalidate will force a
    ///  synchronous paint.
    /// </summary>
    public void Invalidate(Region? region)
    {
        Invalidate(region, false);
    }

    /// <summary>
    ///  Invalidates a region of the control and causes a paint message
    ///  to be sent to the control. This will not force a synchronous paint to
    ///  occur, calling update after invalidate will force a
    ///  synchronous paint.
    /// </summary>
    public unsafe void Invalidate(Region? region, bool invalidateChildren)
    {
        if (region is null)
        {
            Invalidate(invalidateChildren);
        }
        else if (IsHandleCreated)
        {
            using Graphics graphics = CreateGraphicsInternal();
            using RegionScope regionHandle = new(region, graphics);

            if (invalidateChildren)
            {
                PInvoke.RedrawWindow(
                    this,
                    lprcUpdate: null,
                    regionHandle,
                    REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_ERASE | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN);
            }
            else
            {
                // It's safe to invoke InvalidateRgn from a separate thread.
                using var scope = MultithreadSafeCallScope.Create();
                PInvoke.InvalidateRgn(
                    this,
                    regionHandle,
                    !GetStyle(ControlStyles.Opaque));
            }

            OnInvalidated(new InvalidateEventArgs(Rectangle.Ceiling(region.GetBounds(graphics))));
        }
    }

    /// <summary>
    ///  Invalidates the control and causes a paint message to be sent to the control.
    ///  This will not force a synchronous paint to occur, calling update after
    ///  invalidate will force a synchronous paint.
    /// </summary>
    public void Invalidate()
    {
        Invalidate(false);
    }

    /// <summary>
    ///  Invalidates the control and causes a paint message to be sent to the control.
    ///  This will not force a synchronous paint to occur, calling update after
    ///  invalidate will force a synchronous paint.
    /// </summary>
    public unsafe void Invalidate(bool invalidateChildren)
    {
        if (IsHandleCreated)
        {
            if (invalidateChildren)
            {
                PInvoke.RedrawWindow(
                    _window,
                    lprcUpdate: null,
                    HRGN.Null,
                    REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_ERASE | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN);
            }
            else
            {
                // It's safe to invoke InvalidateRect from a separate thread.
                using var scope = MultithreadSafeCallScope.Create();
                PInvoke.InvalidateRect(
                    this,
                    lpRect: null,
                    bErase: !_controlStyle.HasFlag(ControlStyles.Opaque));
            }

            NotifyInvalidate(ClientRectangle);
        }
    }

    /// <summary>
    ///  Invalidates a rectangular region of the control and causes a paint message
    ///  to be sent to the control. This will not force a synchronous paint to
    ///  occur, calling update after invalidate will force a
    ///  synchronous paint.
    /// </summary>
    public void Invalidate(Rectangle rc)
    {
        Invalidate(rc, false);
    }

    /// <summary>
    ///  Invalidates a rectangular region of the control and causes a paint message
    ///  to be sent to the control. This will not force a synchronous paint to
    ///  occur, calling update after invalidate will force a
    ///  synchronous paint.
    /// </summary>
    public unsafe void Invalidate(Rectangle rc, bool invalidateChildren)
    {
        if (rc.IsEmpty)
        {
            Invalidate(invalidateChildren);
        }
        else if (IsHandleCreated)
        {
            RECT rcArea = rc;
            if (invalidateChildren)
            {
                PInvoke.RedrawWindow(
                    _window,
                    &rcArea,
                    HRGN.Null,
                    REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_ERASE | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN);
            }
            else
            {
                // It's safe to invoke InvalidateRect from a separate thread.
                using var scope = MultithreadSafeCallScope.Create();
                PInvoke.InvalidateRect(
                    this,
                    &rcArea,
                    bErase: !_controlStyle.HasFlag(ControlStyles.Opaque));
            }

            NotifyInvalidate(rc);
        }
    }

    /// <summary>
    ///  Executes the specified delegate on the thread that owns the control's underlying window handle.
    /// </summary>
    /// <param name="method">A delegate that contains a method to be called in the control's thread context.</param>
    public void Invoke(Action method)
    {
        _ = Invoke(method, null);
    }

    /// <summary>
    ///  Executes the given delegate on the thread that owns this Control's
    ///  underlying window handle.  It is an error to call this on the same thread that
    ///  the control belongs to.  If the control's handle doesn't exist yet, this will
    ///  follow up the control's parent chain until it finds a control or form that does
    ///  have a window handle.  If no appropriate handle can be found, invoke will throw
    ///  an exception.  Exceptions that are raised during the call will be
    ///  propagated back to the caller.
    ///
    ///  There are five functions on a control that are safe to call from any
    ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
    ///  For all other method calls, you should use one of the invoke methods to marshal
    ///  the call to the control's thread.
    /// </summary>
    public object Invoke(Delegate method) => Invoke(method, null);

    /// <summary>
    ///  Executes the given delegate on the thread that owns this Control's
    ///  underlying window handle.  It is an error to call this on the same thread that
    ///  the control belongs to.  If the control's handle doesn't exist yet, this will
    ///  follow up the control's parent chain until it finds a control or form that does
    ///  have a window handle.  If no appropriate handle can be found, invoke will throw
    ///  an exception.  Exceptions that are raised during the call will be
    ///  propagated back to the caller.
    ///
    ///  There are five functions on a control that are safe to call from any
    ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
    ///  For all other method calls, you should use one of the invoke methods to marshal
    ///  the call to the control's thread.
    /// </summary>
    public object Invoke(Delegate method, params object?[]? args)
    {
        using var scope = MultithreadSafeCallScope.Create();
        Control marshaler = FindMarshalingControl();
        return marshaler.MarshaledInvoke(this, method, args, synchronous: true);
    }

    /// <summary>
    ///  Executes the specified delegate on the thread that owns the control's underlying window handle.
    /// </summary>
    /// <typeparam name="T">The return type of the <paramref name="method"/>.</typeparam>
    /// <param name="method">A function to be called in the control's thread context.</param>
    /// <returns>The return value from the function being invoked.</returns>
    public T Invoke<T>(Func<T> method) => (T)Invoke(method, null);

    /// <summary>
    ///  Perform the callback of a particular ThreadMethodEntry - called by InvokeMarshaledCallbacks below.
    ///
    ///  If the invoke request originated from another thread, we should have already captured the ExecutionContext
    ///  of that thread. The callback is then invoked using that ExecutionContext (which includes info like the
    ///  compressed security stack).
    ///
    ///  NOTE: The one part of the ExecutionContext that we DON'T want applied to the callback is its SyncContext,
    ///  since this is the SyncContext of the other thread. So we grab the SyncContext of OUR thread, and pass
    ///  this through to the callback to use instead.
    ///
    ///  When the invoke request comes from this thread, there won't be an ExecutionContext so we just invoke
    ///  the callback as is.
    /// </summary>
    private static void InvokeMarshaledCallback(ThreadMethodEntry tme)
    {
        if (tme._executionContext is not null)
        {
            s_invokeMarshaledCallbackHelperDelegate ??= new ContextCallback(InvokeMarshaledCallbackHelper);

            // If there's no ExecutionContext, make sure we have a SynchronizationContext.  There's no
            // direct check for ExecutionContext: this is as close as we can get.
            if (SynchronizationContext.Current is null)
            {
                WindowsFormsSynchronizationContext.InstallIfNeeded();
            }

            tme._syncContext = SynchronizationContext.Current;
            ExecutionContext.Run(tme._executionContext, s_invokeMarshaledCallbackHelperDelegate, tme);
        }
        else
        {
            InvokeMarshaledCallbackHelper(tme);
        }
    }

    /// <summary>
    ///  Worker for invoking marshaled callbacks.
    /// </summary>
    private static void InvokeMarshaledCallbackHelper(object? obj)
    {
        ThreadMethodEntry? tme = (ThreadMethodEntry?)obj;

        if (tme?._syncContext is not null)
        {
            SynchronizationContext? oldContext = SynchronizationContext.Current;

            try
            {
                SynchronizationContext.SetSynchronizationContext(tme._syncContext);

                InvokeMarshaledCallbackDo(tme);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }
        else
        {
            InvokeMarshaledCallbackDo(tme);
        }
    }

    private static void InvokeMarshaledCallbackDo(ThreadMethodEntry? tme)
    {
        if (tme is null)
        {
            return;
        }

        // We short-circuit a couple of common cases for speed.
        if (tme._method is EventHandler handler)
        {
            if (tme._args is null || tme._args.Length < 1)
            {
                handler(tme._caller, EventArgs.Empty);
            }
            else if (tme._args.Length < 2)
            {
                handler(tme._args[0], EventArgs.Empty);
            }
            else
            {
                handler(tme._args[0], (EventArgs)tme._args[1]!);
            }
        }
        else if (tme._method is MethodInvoker invoker)
        {
            invoker();
        }
        else if (tme._method is Action action)
        {
            action();
        }
        else if (tme._method is WaitCallback waitCallback)
        {
            Debug.Assert(tme._args!.Length == 1, "Arguments are wrong for WaitCallback");
            waitCallback(tme._args[0]);
        }
        else if (tme._method is SendOrPostCallback sendOrPostCallback)
        {
            Debug.Assert(tme._args!.Length == 1, "Arguments are wrong for SendOrPostCallback");
            sendOrPostCallback(tme._args[0]);
        }
        else
        {
            tme._retVal = tme._method!.DynamicInvoke(tme._args);
        }
    }

    /// <summary>
    ///  Called on the control's owning thread to perform the actual callback.
    ///  This empties this control's callback queue, propagating any exceptions
    ///  back as needed.
    /// </summary>
    private void InvokeMarshaledCallbacks()
    {
        ThreadMethodEntry? current = null;
        if (_threadCallbackList is not null)
        {
            lock (_threadCallbackList)
            {
                if (_threadCallbackList.Count > 0)
                {
                    current = _threadCallbackList.Dequeue();
                }
            }
        }

        // Now invoke on all the queued items.
        while (current is not null)
        {
            if (current._method is not null)
            {
                try
                {
                    // If we are running under the debugger, don't wrap asynchronous
                    // calls in a try catch.  It is much better to throw here than pop up
                    // a thread exception dialog below.
                    if (NativeWindow.WndProcShouldBeDebuggable && !current._synchronous)
                    {
                        InvokeMarshaledCallback(current);
                    }
                    else
                    {
                        try
                        {
                            InvokeMarshaledCallback(current);
                        }
                        catch (Exception t)
                        {
                            current._exception = t.GetBaseException();
                        }
                    }
                }
                finally
                {
                    current.Complete();

                    // This code matches the behavior above.  Basically, if we're debugging, don't
                    // do this because the exception would have been handled above.  If we're
                    // not debugging, raise the exception here.
                    if (!NativeWindow.WndProcShouldBeDebuggable
                        && current._exception is not null
                        && !current._synchronous)
                    {
                        Application.OnThreadException(current._exception);
                    }
                }
            }

            if (_threadCallbackList is not null)
            {
                lock (_threadCallbackList)
                {
                    if (_threadCallbackList.Count > 0)
                    {
                        current = _threadCallbackList.Dequeue();
                    }
                    else
                    {
                        current = null;
                    }
                }
            }
        }
    }

    protected void InvokePaint(Control c, PaintEventArgs e)
    {
        c.OnPaint(e);
    }

    protected void InvokePaintBackground(Control c, PaintEventArgs e)
    {
        c.OnPaintBackground(e);
    }

    /// <summary>
    ///  Determines whether the font is set.
    /// </summary>
    internal bool IsFontSet()
    {
        if (Properties.ContainsObjectThatIsNotNull(s_fontProperty))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///  WARNING! The meaning of this method is not what it appears.
    ///  The method returns true if "descendant" (the argument) is a descendant
    ///  of "this". I'd expect it to be the other way around, but oh well too late.
    /// </summary>
    internal bool IsDescendant(Control? descendant)
    {
        Control? control = descendant;
        while (control is not null)
        {
            if (control == this)
            {
                return true;
            }

            control = control.ParentInternal;
        }

        return false;
    }

    /// <summary>
    ///  This Function will return a Boolean as to whether the Key value passed in is Locked...
    /// </summary>
    public static bool IsKeyLocked(Keys keyVal)
    {
        if (keyVal is Keys.Insert or Keys.NumLock or Keys.CapsLock or Keys.Scroll)
        {
            int result = PInvoke.GetKeyState((int)keyVal);

            // If the high-order bit is 1, the key is down; otherwise, it is up.
            // If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key,
            // is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0.
            // A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled,
            // and off when the key is untoggled.

            // Toggle keys (only low bit is of interest).
            if (keyVal is Keys.Insert or Keys.CapsLock)
            {
                return (result & 0x1) != 0x0;
            }

            return (result & 0x8001) != 0x0;
        }

        // else - it's an un-lockable key.
        // Actually get the exception string from the system resource.
        throw new NotSupportedException(SR.ControlIsKeyLockedNumCapsScrollLockKeysSupportedOnly);
    }

    /// <summary>
    ///  Determines if <paramref name="charCode"/> is an input character that the control wants.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called during window message pre-processing to determine whether the given input
    ///   character should be pre-processed or sent directly to the control. The pre-processing of a character
    ///   includes checking whether the character is a mnemonic of another control.
    ///   (<see cref="PreProcessControlMessage(ref Message)"/>)
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  'true' if the <paramref name="charCode"/> should be sent directly to the control.
    /// </returns>
    protected virtual bool IsInputChar(char charCode)
    {
        int mask;
        if (charCode == (char)(int)Keys.Tab)
        {
            mask = (int)(PInvoke.DLGC_WANTCHARS | PInvoke.DLGC_WANTALLKEYS | PInvoke.DLGC_WANTTAB);
        }
        else
        {
            mask = (int)(PInvoke.DLGC_WANTCHARS | PInvoke.DLGC_WANTALLKEYS);
        }

        return ((int)PInvoke.SendMessage(this, PInvoke.WM_GETDLGCODE) & mask) != 0;
    }

    /// <summary>
    ///  Determines if <paramref name="keyData"/> is an input key that the control wants.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called during window message pre-processing to determine whether the given input key
    ///   should be pre-processed or sent directly to the control.Keys that are pre-processed include TAB, RETURN,
    ///   ESCAPE, and arrow keys. (<see cref="PreProcessControlMessage(ref Message)"/>)
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  'true' if the <paramref name="keyData"/> should be sent directly to the control.
    /// </returns>
    protected virtual bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.Alt) == Keys.Alt)
        {
            return false;
        }

        uint mask = PInvoke.DLGC_WANTALLKEYS;
        switch (keyData & Keys.KeyCode)
        {
            case Keys.Tab:
                mask = PInvoke.DLGC_WANTALLKEYS | PInvoke.DLGC_WANTTAB;
                break;
            case Keys.Left:
            case Keys.Right:
            case Keys.Up:
            case Keys.Down:
                mask = PInvoke.DLGC_WANTALLKEYS | PInvoke.DLGC_WANTARROWS;
                break;
        }

        return IsHandleCreated
            && ((uint)PInvoke.SendMessage(this, PInvoke.WM_GETDLGCODE) & mask) != 0;
    }

    /// <summary>
    ///  Determines if <paramref name="charCode"/> is the mnemonic character in <paramref name="text"/>.
    ///  The mnemonic character is the character immediately following the first
    ///  instance of "&amp;" in text
    /// </summary>
    public static bool IsMnemonic(char charCode, string? text)
    {
        // Special case handling:
        if (charCode == '&')
        {
            return false;
        }

        if (text is not null)
        {
            int pos = -1; // start with -1 to handle double &'s
            char c2 = char.ToUpper(charCode, CultureInfo.CurrentCulture);
            for (; ; )
            {
                if (pos + 1 >= text.Length)
                {
                    break;
                }

                pos = text.IndexOf('&', pos + 1) + 1;
                if (pos <= 0 || pos >= text.Length)
                {
                    break;
                }

                char c1 = char.ToUpper(text[pos], CultureInfo.CurrentCulture);

                if (c1 == c2 || char.ToLower(c1, CultureInfo.CurrentCulture) == char.ToLower(c2, CultureInfo.CurrentCulture))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Checks if this is a container control and will be scaled by parent.
    private static bool IsScaledByParent(Control control)
    {
        Control? parentControl = control.Parent;
        while (parentControl is not null and not ContainerControl)
        {
            parentControl = parentControl.Parent;
        }

        return parentControl is ContainerControl;
    }

    /// <summary>
    ///  Transforms an integer coordinate from logical to device units by scaling it for the current DPI
    ///  and rounding down to the nearest integer value.
    /// </summary>
    public int LogicalToDeviceUnits(int value) => ScaleHelper.ScaleToDpi(value, DeviceDpi);

    /// <summary>
    ///  Transforms size from logical to device units by scaling it for the current
    ///  Dpi and rounding down to the nearest integer value for width and height.
    /// </summary>
    /// <param name="value"> size to be scaled</param>
    /// <returns> scaled size</returns>
    public Size LogicalToDeviceUnits(Size value) => ScaleHelper.ScaleToDpi(value, DeviceDpi);

    /// <summary>
    ///  Create a new bitmap scaled for the device units. When displayed on the device,
    ///  the scaled image will have same size as the original image would have when
    ///  displayed at 96dpi.
    /// </summary>
    /// <param name="logicalBitmap">The image to scale from logical units to device units</param>
    public void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap)
    {
        if (logicalBitmap is null)
        {
            return;
        }

        logicalBitmap = ScaleHelper.ScaleToDpi(logicalBitmap, DeviceDpi, disposeBitmap: true);
    }

    private protected void AdjustWindowRectExForControlDpi(ref RECT rect, WINDOW_STYLE style, bool bMenu, WINDOW_EX_STYLE exStyle)
    {
        AdjustWindowRectExForDpi(ref rect, style, bMenu, exStyle, _deviceDpi);
    }

    private static void AdjustWindowRectExForDpi(ref RECT rect, WINDOW_STYLE style, bool bMenu, WINDOW_EX_STYLE exStyle, int dpi)
    {
        if ((ScaleHelper.IsThreadPerMonitorV2Aware || ScaleHelper.IsScalingRequired) && OsVersion.IsWindows10_1703OrGreater())
        {
            PInvoke.AdjustWindowRectExForDpi(ref rect, style, bMenu, exStyle, (uint)dpi);
        }
        else
        {
            PInvoke.AdjustWindowRectEx(ref rect, style, bMenu, exStyle);
        }
    }

    private object MarshaledInvoke(Control caller, Delegate method, object?[]? args, bool synchronous)
    {
        // Marshaling an invoke occurs in three steps:
        //
        // 1.  Create a ThreadMethodEntry that contains the packet of information
        //     about this invoke.  This TME is placed on a linked list of entries because
        //     we have a gap between the time we PostMessage and the time it actually
        //     gets processed, and this gap may allow other invokes to come in.  Access
        //     to this linked list is always synchronized.
        //
        // 2.  Post ourselves a message.  Our caller has already determined the
        //     best control to call us on, and we should almost always have a handle.
        //
        // 3.  If we're synchronous, wait for the message to get processed.  We don't do
        //     a SendMessage here so we're compatible with OLE, which will abort many
        //     types of calls if we're within a SendMessage.

        if (!IsHandleCreated)
        {
            throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
        }

        // We don't want to wait if we're on the same thread, or else we'll deadlock.
        // It is important that syncSameThread always be false for asynchronous calls.
        bool syncSameThread = synchronous && PInvoke.GetWindowThreadProcessId(this, out _) == PInvoke.GetCurrentThreadId();

        // Store the compressed stack information from the thread that is calling the Invoke()
        // so we can assign the same security context to the thread that will actually execute
        // the delegate being passed.
        ExecutionContext? executionContext = null;
        if (!syncSameThread)
        {
            executionContext = ExecutionContext.Capture();
        }

        ThreadMethodEntry tme = new(
            caller,
            this,
            method,
            args,
            synchronous,
            executionContext);

        lock (this)
        {
            _threadCallbackList ??= new Queue<ThreadMethodEntry>();
        }

        lock (_threadCallbackList)
        {
            if (s_threadCallbackMessage == PInvoke.WM_NULL)
            {
                s_threadCallbackMessage = PInvoke.RegisterWindowMessage($"{Application.WindowMessagesVersion}_ThreadCallbackMessage");
            }

            _threadCallbackList.Enqueue(tme);
        }

        if (syncSameThread)
        {
            InvokeMarshaledCallbacks();
        }
        else
        {
            PInvoke.PostMessage(this, s_threadCallbackMessage);
        }

        if (synchronous)
        {
            if (!tme.IsCompleted)
            {
                // In synchronous call we not need waitHandle after wait.
                using WaitHandle waitHandle = tme.AsyncWaitHandle;
                WaitForWaitHandle(waitHandle);
            }

            if (tme._exception is not null)
            {
                ExceptionDispatchInfo.Throw(tme._exception);
            }

            return tme._retVal!;
        }
        else
        {
            return tme;
        }
    }

    /// <summary>
    ///  This method is used by WM_GETCONTROLNAME and WM_GETCONTROLTYPE
    ///  to marshal a string to a message structure.  It handles
    ///  two cases:  if no buffer was passed it returns the size of
    ///  buffer needed.  If a buffer was passed, it fills the buffer.
    ///  If the passed buffer is not long enough it will return -1.
    /// </summary>
    private static void MarshalStringToMessage(string value, ref Message m)
    {
        if (m.LParamInternal == 0)
        {
            m.ResultInternal = (LRESULT)((value.Length + 1) * sizeof(char));
            return;
        }

        if ((int)m.WParamInternal < value.Length + 1)
        {
            m.ResultInternal = (LRESULT)(-1);
            return;
        }

        // Copy the name into the given IntPtr
        char[] nullChar = [(char)0];
        byte[] nullBytes;
        byte[] bytes;

        bytes = Encoding.Unicode.GetBytes(value);
        nullBytes = Encoding.Unicode.GetBytes(nullChar);

        Marshal.Copy(bytes, 0, m.LParamInternal, bytes.Length);
        Marshal.Copy(nullBytes, 0, m.LParamInternal + (nint)bytes.Length, nullBytes.Length);

        m.ResultInternal = (LRESULT)((bytes.Length + nullBytes.Length) / sizeof(char));
    }

    /// <summary>
    ///  Propagates the invalidation event, notifying the control that
    ///  some part of it is being invalidated and will subsequently need
    ///  to repaint.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void NotifyInvalidate(Rectangle invalidatedArea)
    {
        OnInvalidated(new InvalidateEventArgs(invalidatedArea));
    }

    // Used by form to notify the control that it is validating.
    private bool NotifyValidating()
    {
        CancelEventArgs ev = new();
        OnValidating(ev);
        return ev.Cancel;
    }

    // Used by form to notify the control that it has been validated.
    private void NotifyValidated()
    {
        OnValidated(EventArgs.Empty);
    }

    /// <summary>
    ///  Raises the <see cref="Click"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void InvokeOnClick(Control? toInvoke, EventArgs e)
    {
        toInvoke?.OnClick(e);
    }

    protected virtual void OnAutoSizeChanged(EventArgs e)
    {
        if (Events[s_autoSizeChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBackColorChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        object? backBrush = Properties.GetObject(s_backBrushProperty);
        if (backBrush is not null)
        {
            if (GetState(States.OwnCtlBrush))
            {
                HBRUSH p = (HBRUSH)backBrush;
                if (!p.IsNull)
                {
                    PInvokeCore.DeleteObject(p);
                }
            }

            Properties.SetObject(s_backBrushProperty, value: null);
        }

        Invalidate();

        if (Events[s_backColorEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentBackColorChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBackgroundImageChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        Invalidate();

        if (Events[s_backgroundImageEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentBackgroundImageChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBackgroundImageLayoutChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        Invalidate();

        if (Events[s_backgroundImageLayoutEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBindingContextChanged(EventArgs e)
    {
        if (Properties.ContainsObjectThatIsNotNull(s_bindingsProperty))
        {
            if (!Binding.IsSupported)
            {
                throw new NotSupportedException(SR.BindingNotSupported);
            }

            UpdateBindings();
        }

        if (Events[s_bindingContextEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentBindingContextChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCausesValidationChanged(EventArgs e)
    {
        if (Events[s_causesValidationEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Called when a child is about to resume its layout.  The default implementation
    ///  calls OnChildLayoutResuming on the parent.
    /// </summary>
    internal virtual void OnChildLayoutResuming(Control child, bool performLayout)
    {
        ParentInternal?.OnChildLayoutResuming(child, performLayout);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnContextMenuStripChanged(EventArgs e)
    {
        if (Events[s_contextMenuStripEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCursorChanged(EventArgs e)
    {
        if (Events[s_cursorEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentCursorChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDataContextChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        if (Events[s_dataContextEvent] is EventHandler eventHandler)
        {
            eventHandler(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentDataContextChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDockChanged(EventArgs e)
    {
        if (Events[s_dockEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="Enabled"/> event.
    ///
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnEnabled to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnEnabledChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        if (IsHandleCreated)
        {
            PInvoke.EnableWindow(this, Enabled);

            // User-paint controls should repaint when their enabled state changes
            if (GetStyle(ControlStyles.UserPaint))
            {
                Invalidate();
                Update();
            }
        }

        if (Events[s_enabledEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentEnabledChanged(e);
            }
        }
    }

    private protected virtual void OnFrameWindowActivate(bool fActivate)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnFontChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        Invalidate();

        if (Properties.ContainsInteger(s_fontHeightProperty))
        {
            Properties.SetInteger(s_fontHeightProperty, -1);
        }

        // Cleanup any font handle wrapper.
        DisposeFontHandle();

        if (IsHandleCreated)
        {
            SetWindowFont();
        }

        if (Events[s_fontEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        using (new LayoutTransaction(this, this, PropertyNames.Font, false))
        {
            if (controlsCollection is not null)
            {
                // This may have changed the sizes of our children.
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentFontChanged(e);
                }
            }
        }

        LayoutTransaction.DoLayout(this, this, PropertyNames.Font);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnForeColorChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        Invalidate();

        if (Events[s_foreColorEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentForeColorChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnRightToLeftChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        // update the scroll position when the handle has been created
        // MUST SET THIS BEFORE CALLING RecreateHandle!!!
        SetExtendedState(ExtendedStates.SetScrollPosition, true);

        RecreateHandle();

        if (Events[s_rightToLeftEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnParentRightToLeftChanged(e);
            }
        }
    }

    /// <summary>
    ///  OnNotifyMessage is called if the ControlStyles.EnableNotifyMessage bit is set.
    ///  This allows for controls to listen to window messages, without allowing them to
    ///  actually modify the message.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnNotifyMessage(Message m)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentBackColorChanged(EventArgs e)
    {
        Color backColor = Properties.GetColor(s_backColorProperty);
        if (backColor.IsEmpty)
        {
            OnBackColorChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentBackgroundImageChanged(EventArgs e)
    {
        OnBackgroundImageChanged(e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentBindingContextChanged(EventArgs e)
    {
        if (!Properties.ContainsObjectThatIsNotNull(s_bindingManagerProperty))
        {
            OnBindingContextChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentCursorChanged(EventArgs e)
    {
        if (!Properties.ContainsObjectThatIsNotNull(s_cursorProperty))
        {
            OnCursorChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentDataContextChanged(EventArgs e)
    {
        if (Properties.ContainsObject(s_dataContextProperty))
        {
            // If this DataContext was the same as the Parent's just became,
            if (Equals(Properties.GetObject(s_dataContextProperty), Parent?.DataContext))
            {
                // we need to make it ambient again by removing it.
                Properties.RemoveObject(s_dataContextProperty);

                // Even though internally we don't store it any longer, and the
                // value we had stored therefore changed, technically the value
                // remains the same, so we don't raise the DataContextChanged event.
                return;
            }
        }

        // In every other case we're going to raise the event.
        OnDataContextChanged(e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentEnabledChanged(EventArgs e)
    {
        if (GetState(States.Enabled))
        {
            OnEnabledChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentFontChanged(EventArgs e)
    {
        // Container controls that were marked IsDpiChangeScalingRequired had to go through OnFontChanged event
        // Irrespective of the Font status (explicit set or inherit Font). See "WmDpiChangedBeforeParent" for more info.
        var container = this as ContainerControl;

        try
        {
            if (!IsFontSet() || (container is not null && container.IsDpiChangeScalingRequired))
            {
                OnFontChanged(e);
            }
        }
        finally
        {
            if (container is not null)
            {
                container.IsDpiChangeScalingRequired = false;
            }
        }
    }

    /// <summary>
    ///  Occurs when the parent of this control has recreated its handle.
    /// </summary>
    internal virtual void OnParentHandleRecreated()
    {
        // Restore ourselves over to the original control.
        // Use SetParent directly so as to not raise ParentChanged events.
        Control? parent = ParentInternal;
        if (parent is not null && IsHandleCreated)
        {
            if (PInvoke.SetParent(this, parent).IsNull)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32SetParentFailed);
            }

            UpdateZOrder();
        }

        SetState(States.ParentRecreating, false);

        // If our parent was initially the parent who's handle just got recreated, we need
        // to recreate ourselves so that we get notification. See UpdateReflectParent for more details.
        if (ReflectParent == ParentInternal)
        {
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Occurs when the parent of this control is recreating its handle.
    /// </summary>
    internal virtual void OnParentHandleRecreating()
    {
        SetState(States.ParentRecreating, true);

        // Move this control over to the parking window.

        // If we left it parented to the parent control, DestroyWindow would force us to destroy our handle as well.
        // Temporarilty parenting to the parking window avoids having to recreate our handle from scratch.

        if (IsHandleCreated)
        {
            Application.ParkHandle(handle: new(this), DpiAwarenessContext);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentForeColorChanged(EventArgs e)
    {
        Color foreColor = Properties.GetColor(s_foreColorProperty);
        if (foreColor.IsEmpty)
        {
            OnForeColorChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentRightToLeftChanged(EventArgs e)
    {
        if (!Properties.ContainsInteger(s_rightToLeftProperty) || ((RightToLeft)Properties.GetInteger(s_rightToLeftProperty)) == RightToLeft.Inherit)
        {
            OnRightToLeftChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentVisibleChanged(EventArgs e)
    {
        if (DesiredVisibility)
        {
            OnVisibleChanged(e);
        }
    }

    // OnVisibleChanged/OnParentVisibleChanged is not called when a parent becomes invisible
    internal virtual void OnParentBecameInvisible()
    {
        if (DesiredVisibility)
        {
            // This control became invisible too - notify its children
            ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection is not null)
            {
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentBecameInvisible();
                }
            }
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPrint(PaintEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (GetStyle(ControlStyles.UserPaint))
        {
            // Theme support requires that we paint the background and foreground to support semi-transparent children
            PaintWithErrorHandling(e, PaintLayerBackground);
            e.ResetGraphics();
            PaintWithErrorHandling(e, PaintLayerForeground);
        }
        else
        {
            if (e is not PrintPaintEventArgs ppev)
            {
                uint flags = PInvoke.PRF_CHILDREN | PInvoke.PRF_CLIENT | PInvoke.PRF_ERASEBKGND | PInvoke.PRF_NONCLIENT;

                using DeviceContextHdcScope hdc = new(e);
                Message m = Message.Create(HWND, PInvoke.WM_PRINTCLIENT, (WPARAM)hdc, (LPARAM)flags);
                DefWndProc(ref m);
            }
            else
            {
                Message m = ppev.Message;
                DefWndProc(ref m);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnTabIndexChanged(EventArgs e)
    {
        if (Events[s_tabIndexEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnTabStopChanged(EventArgs e)
    {
        if (Events[s_tabStopEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnTextChanged(EventArgs e)
    {
        if (Events[s_textEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="Visible"/> event.
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnVisible to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnVisibleChanged(EventArgs e)
    {
        bool visible = Visible;
        if (visible)
        {
            UnhookMouseEvent();
            _trackMouseEvent = default;
        }

        if (_parent is not null && visible && !Created)
        {
            bool isDisposing = GetAnyDisposingInHierarchy();
            if (!isDisposing)
            {
                // Usually the control is created by now, but in a few corner cases
                // exercised by the PropertyGrid dropdowns, it isn't
                CreateControl();
            }
        }

        if (Events[s_visibleEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                Control ctl = controlsCollection[i];
                if (ctl.Visible)
                {
                    ctl.OnParentVisibleChanged(e);
                }

                if (!visible)
                {
                    ctl.OnParentBecameInvisible();
                }
            }
        }
    }

    internal virtual void OnTopMostActiveXParentChanged(EventArgs e)
    {
        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnTopMostActiveXParentChanged(e);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentChanged(EventArgs e)
    {
        if (Events[s_parentEvent] is EventHandler eh)
        {
            eh(this, e);
        }

        // Inform the control that the topmost control is now an ActiveX control
        if (TopMostParent.IsActiveX)
        {
            OnTopMostActiveXParentChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Raises the <see cref="Click"/>
    ///  event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnClick(EventArgs e)
    {
        ((EventHandler?)Events[s_clickEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnClientSizeChanged(EventArgs e)
    {
        if (Events[s_clientSizeEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="ControlAdded"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnControlAdded(ControlEventArgs e)
    {
        ((ControlEventHandler?)Events[s_controlAddedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="ControlRemoved"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnControlRemoved(ControlEventArgs e)
    {
        ((ControlEventHandler?)Events[s_controlRemovedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Called when the control is first created.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCreateControl()
    {
    }

    /// <summary>
    ///  Inheriting classes should override this method to find out when the handle has been created.
    ///  Call base.OnHandleCreated first.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnHandleCreated(EventArgs e)
    {
        if (IsHandleCreated)
        {
            // Setting fonts is for some reason incredibly expensive.
            // (Even if you exclude font handle creation)
            if (!GetStyle(ControlStyles.UserPaint))
            {
                SetWindowFont();
            }

            HandleHighDpi();

            // Restore drag drop status. Ole Initialize happens when the ThreadContext in Application is created.
            SetAcceptDrops(AllowDrop);

            Region? region = Region;
            if (region is not null)
            {
                SetRegion(region);
            }

            // Cache Handle in a local before asserting so we minimize code running under the Assert.
            IntPtr handle = Handle;

            // The Accessibility Object for this Control
            if (Properties.GetObject(s_accessibilityProperty) is ControlAccessibleObject clientAccessibleObject)
            {
                clientAccessibleObject.Handle = handle;
            }

            // Private accessibility object for control, used to wrap the object that
            // OLEACC.DLL creates to represent the control's non-client (NC) region.
            if (Properties.GetObject(s_ncAccessibilityProperty) is ControlAccessibleObject nonClientAccessibleObject)
            {
                nonClientAccessibleObject.Handle = handle;
            }

            // Set the window text from the Text property.
            if (_text is not null && _text.Length != 0)
            {
                PInvoke.SetWindowText(this, _text);
            }

            if (IsDarkModeEnabled)
            {
                if (this is

                    // Controls with 4 levels of inheritance, sorted alphabetically by type name
                    DomainUpDown         // Inherits from UpDownBase, ContainerControl, ScrollableControl, Control
                    or NumericUpDown     // Inherits from UpDownBase, ContainerControl, ScrollableControl, Control

                    // Controls with 3 levels of inheritance, sorted alphabetically by type name
                    or CheckedListBox    // Inherits from ListBox, ListControl, Control
                    or Form              // Excluded - too invasive.
                    or FlowLayoutPanel   // Inherits from Panel, ScrollableControl, Control
                    or SplitContainer    // Inherits from ContainerControl, ScrollableControl, Control
                    or TabPage           // Inherits from Panel, ScrollableControl, Control
                    or TableLayoutPanel  // Inherits from Panel, ScrollableControl, Control

                    // Controls with 2 levels of inheritance, sorted alphabetically by type name
                    // or ComboBox       // Excluded - directly handled.
                    or ListBox           // Inherits from ListControl, Control

                    or Button            // Inherits from ButtonBase, Control
                    or CheckBox          // Inherits from ButtonBase, Control
                    or MaskedTextBox     // Inherits from TextBoxBase, Control
                    or Panel             // Inherits from ScrollableControl, Control
                    or RadioButton       // Inherits from ButtonBase, Control
                    or RichTextBox       // Inherits from TextBoxBase, Control
                    or TextBox           // Inherits from TextBoxBase, Control
                    or HScrollBar        // Inherits from ScrollBar, Control
                    or VScrollBar        // Inherits from ScrollBar, Control

                    // Base classes and controls with direct inheritance from Control, sorted alphabetically by type name
                    or ButtonBase        // Inherits from Control
                    or DateTimePicker    // Inherits from Control
                    // or GroupBox       // Inherits from Control directly, but behaves like a container
                    or Label             // Inherits from Control
                    or LinkLabel         // Inherits from Label, Control
                    // or ListView       // Excluded - directly handled.
                    or MonthCalendar     // Inherits from Control
                    or PictureBox        // Inherits from Control
                    or ProgressBar       // Inherits from Control
                    or ScrollableControl // Inherits from Control
                    // or TextBoxBase    // Excluded - probably too invasive.
                    or TrackBar          // Inherits from Control
                    or TreeView          // Inherits from Control
                    or UpDownBase)       // Inherits from Control

                // Base class for all UI controls in WinForms
                // or Control           // Excluded.
                {
                    _ = PInvoke.SetWindowTheme(HWND, "DarkMode_Explorer", null);
                }
            }

            if (this is not ScrollableControl
                && !IsMirrored
                && GetExtendedState(ExtendedStates.SetScrollPosition)
                && !GetExtendedState(ExtendedStates.HaveInvoked))
            {
                BeginInvoke(new EventHandler(OnSetScrollPosition));
                SetExtendedState(ExtendedStates.HaveInvoked, true);
                SetExtendedState(ExtendedStates.SetScrollPosition, false);
            }
        }

        ((EventHandler?)Events[s_handleCreatedEvent])?.Invoke(this, e);

        if (IsHandleCreated)
        {
            // Now, repost the thread callback message if we found it. We should do this last, so we're as close
            // to the same state as when the message was placed.
            if (GetState(States.ThreadMarshalPending))
            {
                PInvoke.PostMessage(this, s_threadCallbackMessage);
                SetState(States.ThreadMarshalPending, false);
            }
        }

        void HandleHighDpi()
        {
            if (!DpiAwarenessContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
            {
                return;
            }

            int old = _deviceDpi;
            Font localFont = GetCurrentFontAndDpi(out int fontDpi);
            _deviceDpi = (int)PInvoke.GetDpiForWindow(this);
            if (old == _deviceDpi)
            {
                return;
            }

            if (fontDpi != _deviceDpi)
            {
                // Controls are by default font scaled.
                // Dpi change requires font to be recalculated in order to get controls scaled with right dpi.
                Font fontForDpi = GetScaledFont(localFont, _deviceDpi, fontDpi);
                ScaledControlFont = fontForDpi;

                // If it is a container control that inherit Font and is scaled by parent, we simply scale Font
                // and wait for OnFontChangedEvent caused by its parent. Otherwise, we scale Font and trigger
                // 'OnFontChanged' event explicitly. ex: Windows Forms designer natively hosted in VS.
                if (IsFontSet())
                {
                    SetScaledFont(fontForDpi);
                }
            }

            RescaleConstantsForDpi(old, _deviceDpi);

            // If the control is top-level window and its StartPosition is not WindowsDefaultLocation, Location needs
            // recalculated. For example, a Form centered as FormStartPosition.CenterParent or FormStartPosition.CenterScreen,
            // would need recalculated to place it correctly.
            if (this is Form form && form.TopLevel)
            {
                // Form gets location information from CreateParams but the values are calculated before the handle creation.
                // When launching the Form on a secondary monitor, DPI is evaluated only after handle is created. for the Form and the
                // Form resized according to the new DPI.Hence, Form location need to be recalculated with new bounds information.
                form.AdjustFormPosition();
            }
        }
    }

    private void OnSetScrollPosition(object? sender, EventArgs e)
    {
        SetExtendedState(ExtendedStates.HaveInvoked, false);
        OnInvokedSetScrollPosition(sender, e);
    }

    internal virtual unsafe void OnInvokedSetScrollPosition(object? sender, EventArgs e)
    {
        if (this is not ScrollableControl && !IsMirrored)
        {
            SCROLLINFO si = new()
            {
                cbSize = (uint)sizeof(SCROLLINFO),
                fMask = SCROLLINFO_MASK.SIF_RANGE
            };

            if (PInvoke.GetScrollInfo(this, SCROLLBAR_CONSTANTS.SB_HORZ, ref si))
            {
                si.nPos = (RightToLeft == RightToLeft.Yes) ? si.nMax : si.nMin;
                PInvoke.SendMessage(this, PInvoke.WM_HSCROLL, WPARAM.MAKEWPARAM((int)SCROLLBAR_COMMAND.SB_THUMBPOSITION, si.nPos));
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnLocationChanged(EventArgs e)
    {
        OnMove(e);
        if (Events[s_locationEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="HandleDestroyed"/> event.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Inheriting classes should override this method to find out when the handle is about to be destroyed.
    ///   Call base.OnHandleDestroyed last.
    ///  </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnHandleDestroyed(EventArgs e)
    {
        ((EventHandler?)Events[s_handleDestroyedEvent])?.Invoke(this, e);

        // The Accessibility Object for this Control
        if (Properties.GetObject(s_accessibilityProperty) is ControlAccessibleObject accObj)
        {
            accObj.Handle = IntPtr.Zero;
        }

        // Private accessibility object for control, used to wrap the object that
        // OLEACC.DLL creates to represent the control's non-client (NC) region.
        if (Properties.GetObject(s_ncAccessibilityProperty) is ControlAccessibleObject nonClientAccessibleObject)
        {
            nonClientAccessibleObject.Handle = IntPtr.Zero;
        }

        ReflectParent = null;

        if (!RecreatingHandle)
        {
            if (GetState(States.OwnCtlBrush))
            {
                object? backBrush = Properties.GetObject(s_backBrushProperty);
                if (backBrush is not null)
                {
                    Properties.SetObject(s_backBrushProperty, value: null);
                    HBRUSH p = (HBRUSH)backBrush;
                    if (!p.IsNull)
                    {
                        PInvokeCore.DeleteObject(p);
                    }
                }
            }
        }

        // this code is important -- it is critical that we stash away
        // the value of the text for controls such as edit, button,
        // label, etc. Without this processing, any time you change a
        // property that forces handle recreation, you lose your text!
        // See the above code in wmCreate
        try
        {
            if (!GetAnyDisposingInHierarchy())
            {
                _text = Text;
                if (_text is not null && _text.Length == 0)
                {
                    _text = null;
                }
            }

            SetAcceptDrops(false);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Some ActiveX controls throw exceptions when you ask for the text property after you have destroyed their
            // handle. We don't want those exceptions to bubble all the way to the top, since we leave our state in a mess.
        }
    }

    /// <summary>
    ///  Raises the <see cref="DoubleClick"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDoubleClick(EventArgs e)
    {
        ((EventHandler?)Events[s_doubleClickEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="Enter"/> event.
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onEnter to send this event to any registered event listeners.
    /// </summary>
    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onDragEnter to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragEnter(DragEventArgs drgevent)
    {
        ((DragEventHandler?)Events[s_dragEnterEvent])?.Invoke(this, drgevent);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onDragOver to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragOver(DragEventArgs drgevent)
    {
        ((DragEventHandler?)Events[s_dragOverEvent])?.Invoke(this, drgevent);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onDragLeave to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragLeave(EventArgs e)
    {
        ((EventHandler?)Events[s_dragLeaveEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onDragDrop to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragDrop(DragEventArgs drgevent)
    {
        ((DragEventHandler?)Events[s_dragDropEvent])?.Invoke(this, drgevent);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onGiveFeedback to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
    {
        ((GiveFeedbackEventHandler?)Events[s_giveFeedbackEvent])?.Invoke(this, gfbevent);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void OnEnter(EventArgs e)
    {
        ((EventHandler?)Events[s_enterEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="GotFocus"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void InvokeGotFocus(Control? toInvoke, EventArgs e)
    {
        if (toInvoke is not null)
        {
            toInvoke.OnGotFocus(e);
            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(toInvoke);
        }
    }

    /// <summary>
    ///  Raises the <see cref="GotFocus"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGotFocus(EventArgs e)
    {
        if (IsActiveX)
        {
            ActiveXOnFocus(true);
        }

        _parent?.ChildGotFocus(this);

        ((EventHandler?)Events[s_gotFocusEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onHelp to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnHelpRequested(HelpEventArgs hevent)
    {
        HelpEventHandler? handler = (HelpEventHandler?)Events[s_helpRequestedEvent];
        if (handler is not null)
        {
            handler(this, hevent);

            // Mark the event as handled so that the event isn't raised for the
            // control's parent.
            if (hevent is not null)
            {
                hevent.Handled = true;
            }
        }

        if (hevent is not null && !hevent.Handled)
        {
            ParentInternal?.OnHelpRequested(hevent);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnInvalidate to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnInvalidated(InvalidateEventArgs e)
    {
        // Ask the site to change the view...
        if (IsActiveX)
        {
            ActiveXViewChanged();
        }

        // Transparent control support
        ControlCollection? controls = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controls is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].OnParentInvalidated(e);
            }
        }

        ((InvalidateEventHandler?)Events[s_invalidatedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="KeyDown"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnKeyDown(KeyEventArgs e)
    {
        ((KeyEventHandler?)Events[s_keyDownEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="KeyPress"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnKeyPress(KeyPressEventArgs e)
    {
        ((KeyPressEventHandler?)Events[s_keyPressEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="KeyUp"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnKeyUp(KeyEventArgs e)
    {
        if (OsVersion.IsWindows11_OrGreater()
            && (e.KeyCode.HasFlag(Keys.ControlKey) || e.KeyCode == Keys.Escape))
        {
            KeyboardToolTipStateMachine.HidePersistentTooltip();
        }

        ((KeyEventHandler?)Events[s_keyUpEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Core layout logic. Inheriting controls should override this function to do any custom
    ///  layout logic. It is not necessary to call base.OnLayout, however for normal docking
    ///  an functions to work, base.OnLayout must be called.
    ///  Raises the <see cref="Layout"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnLayout(LayoutEventArgs levent)
    {
        // Ask the site to change the view.
        if (IsActiveX)
        {
            ActiveXViewChanged();
        }

        ((LayoutEventHandler?)Events[s_layoutEvent])?.Invoke(this, levent);

        bool parentRequiresLayout = LayoutEngine.Layout(this, levent);
        if (parentRequiresLayout && ParentInternal is not null)
        {
            // LayoutEngine.Layout can return true to request that our parent resize us because
            // we did not have enough room for our contents. We can not just call PerformLayout
            // because this container is currently suspended. PerformLayout will check this state
            // flag and PerformLayout on our parent.
            ParentInternal.SetState(States.LayoutIsDirty, true);
        }
    }

    /// <summary>
    ///  Called when the last resume layout call is made. If performLayout is true a layout will
    ///  occur as soon as this call returns. Layout is still suspended when this call is made.
    ///  The default implementation calls OnChildLayoutResuming on the parent, if it exists.
    /// </summary>
    internal virtual void OnLayoutResuming(bool performLayout)
    {
        ParentInternal?.OnChildLayoutResuming(this, performLayout);
    }

    internal virtual void OnLayoutSuspended()
    {
    }

    /// <summary>
    ///  Raises the <see cref="Leave"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void OnLeave(EventArgs e)
    {
        ((EventHandler?)Events[s_leaveEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void InvokeLostFocus(Control? toInvoke, EventArgs e)
    {
        if (toInvoke is not null)
        {
            toInvoke.OnLostFocus(e);
            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(toInvoke);
        }
    }

    /// <summary>
    ///  Raises the <see cref="LostFocus"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnLostFocus(EventArgs e)
    {
        if (IsActiveX)
        {
            ActiveXOnFocus(false);
        }

        ((EventHandler?)Events[s_lostFocusEvent])?.Invoke(this, e);
    }

    protected virtual void OnMarginChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_marginChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseDoubleClick"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseDoubleClickEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="OnMouseClick"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseClick(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseClickEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseCaptureChanged"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseCaptureChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_mouseCaptureChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseDown"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseDown(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseDownEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseEnter"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseEnter(EventArgs e)
    {
        ((EventHandler?)Events[s_mouseEnterEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseLeave"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseLeave(EventArgs e)
    {
        ((EventHandler?)Events[s_mouseLeaveEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="DpiChangedBeforeParent"/> event.
    ///  Occurs when the form is moved to a monitor with a different resolution (number of dots per inch),
    ///  or when scaling level is changed in the windows setting by the user.
    ///  This message is not sent to the top level windows.
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    protected virtual void OnDpiChangedBeforeParent(EventArgs e)
    {
        ((EventHandler?)Events[s_dpiChangedBeforeParentEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="DpiChangedAfterParent"/> event.
    ///  Occurs when the form is moved to a monitor with a different resolution (number of dots per inch),
    ///  or when scaling level is changed in windows setting by the user.
    ///  This message is not sent to the top level windows.
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    protected virtual void OnDpiChangedAfterParent(EventArgs e)
    {
        ((EventHandler?)Events[s_dpiChangedAfterParentEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseHover"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseHover(EventArgs e)
    {
        ((EventHandler?)Events[s_mouseHoverEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseMove"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseMove(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseMoveEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseUp"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseUp(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseUpEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MouseWheel"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseWheel(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseWheelEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="Move"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMove(EventArgs e)
    {
        ((EventHandler?)Events[s_moveEvent])?.Invoke(this, e);

        if (RenderTransparent)
        {
            Invalidate();
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onPaint to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPaint(PaintEventArgs e)
    {
        ((PaintEventHandler?)Events[s_paintEvent])?.Invoke(this, e);
    }

    protected virtual void OnPaddingChanged(EventArgs e)
    {
        if (GetStyle(ControlStyles.ResizeRedraw))
        {
            Invalidate();
        }

        ((EventHandler?)Events[s_paddingChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle the erase background request from windows. It is
    ///  not necessary to call base.OnPaintBackground.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPaintBackground(PaintEventArgs pevent)
    {
        // We need the true client rectangle as clip rectangle causes problems on "Windows Classic" theme.
        PInvokeCore.GetClientRect(new HandleRef<HWND>(_window, InternalHandle), out RECT rect);
        PaintBackground(pevent, rect);
    }

    // Transparent control support
    private void OnParentInvalidated(InvalidateEventArgs e)
    {
        if (!RenderTransparent)
        {
            return;
        }

        if (IsHandleCreated)
        {
            // move invalid rect into child space
            Rectangle cliprect = e.InvalidRect;
            Point offs = Location;
            cliprect.Offset(-offs.X, -offs.Y);
            cliprect = Rectangle.Intersect(ClientRectangle, cliprect);

            // if we don't intersect at all, do nothing
            if (cliprect.IsEmpty)
            {
                return;
            }

            Invalidate(cliprect);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onQueryContinueDrag to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
    {
        ((QueryContinueDragEventHandler?)Events[s_queryContinueDragEvent])?.Invoke(this, qcdevent);
    }

    /// <summary>
    ///  Raises the <see cref="RegionChanged"/> event when the Region property has changed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnRegionChanged(EventArgs e)
    {
        if (Events[s_regionChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="Resize"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnResize(EventArgs e)
    {
        if ((_controlStyle & ControlStyles.ResizeRedraw) == ControlStyles.ResizeRedraw
            || GetState(States.ExceptionWhilePainting))
        {
            Invalidate();
        }

        LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
        ((EventHandler?)Events[s_resizeEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="PreviewKeyDown"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
    {
        ((PreviewKeyDownEventHandler?)Events[s_previewKeyDownEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSizeChanged(EventArgs e)
    {
        OnResize(EventArgs.Empty);

        if (Events[s_sizeEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="ChangeUICues"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnChangeUICues(UICuesEventArgs e)
    {
        ((UICuesEventHandler?)Events[s_changeUICuesEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="OnStyleChanged"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnStyleChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_styleChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="SystemColorsChanged"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSystemColorsChanged(EventArgs e)
    {
        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                controlsCollection[i].OnSystemColorsChanged(EventArgs.Empty);
            }
        }

        Invalidate();

        ((EventHandler?)Events[s_systemColorsChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="Validating"/>
    ///  event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnValidating(CancelEventArgs e)
    {
        ((CancelEventHandler?)Events[s_validatingEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="Validated"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnValidated(EventArgs e)
    {
        ((EventHandler?)Events[s_validatedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  This is called in the <see cref="Control"/> constructor before calculating the initial <see cref="Size"/>.
    ///  This gives a chance to initialize fields that will be used in calls to sizing related virtuals such as
    ///  <see cref="DefaultSize"/>, etc. The real size cannot be calculated until the handle is created as Windows
    ///  can have their own DPI setting. When the handle is created, <see cref="RescaleConstantsForDpi(int, int)"/>
    ///  is called.
    /// </summary>
    private protected virtual void InitializeConstantsForInitialDpi(int initialDpi) { }

    /// <summary>
    ///  Invoked when the control handle is created and right before the top level parent control receives a
    ///  WM_DPICHANGED message.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is an opportunity to rescale any constant sizes, glyphs or bitmaps before re-painting.
    ///  </para>
    /// </remarks>
    /// <param name="deviceDpiOld">The DPI value prior to the change.</param>
    /// <param name="deviceDpiNew">The DPI value after the change.</param>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
    }

    // This is basically OnPaintBackground, put in a separate method for ButtonBase,
    // which does all painting under OnPaint, and tries very hard to avoid double-painting the border pixels.
    internal void PaintBackground(PaintEventArgs e, Rectangle rectangle) =>
        PaintBackground(e, rectangle, BackColor, Point.Empty);

    internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor, Point scrollOffset = default)
    {
        ArgumentNullException.ThrowIfNull(e);

        bool renderColorTransparent = RenderColorTransparent(backColor);
        if (renderColorTransparent)
        {
            PaintTransparentBackground(e, rectangle);
        }

        // If the form or mdiclient is mirrored then we do not render the background image due to GDI+ issues.
        bool formRTL = ((this is Form || this is MdiClient) && IsMirrored);

        // The rest of this won't do much if BackColor is transparent and there is no BackgroundImage,
        // but we need to call it in the partial alpha case.

        if (BackgroundImage is not null && !DisplayInformation.HighContrast && !formRTL)
        {
            bool imageIsTransparent = ControlPaint.IsImageTransparent(BackgroundImage);
            if (!renderColorTransparent && BackgroundImageLayout == ImageLayout.Tile && imageIsTransparent)
            {
                PaintTransparentBackground(e, rectangle);
            }

            Point scrollLocation = scrollOffset;
            if (this is ScrollableControl scrollControl && scrollLocation != Point.Empty)
            {
                scrollLocation = scrollControl.AutoScrollPosition;
            }

            if (imageIsTransparent)
            {
                PaintBackColor(e, rectangle, backColor);
            }

            ControlPaint.DrawBackgroundImage(
                e.GraphicsInternal,
                BackgroundImage,
                backColor,
                BackgroundImageLayout,
                ClientRectangle,
                rectangle,
                scrollLocation,
                RightToLeft);
        }
        else
        {
            PaintBackColor(e, rectangle, backColor);
        }
    }

    private static void PaintBackColor(PaintEventArgs e, Rectangle rectangle, Color backColor)
    {
        // Common case of just painting the background.  For this, we
        // use GDI because it is faster for simple things than creating
        // a graphics object, brush, etc.  Also, we may be able to
        // use a system brush, avoiding the brush create altogether.

        Color color = backColor;

        // Note: PaintEvent.HDC == 0 if GDI+ has used the HDC -- it wouldn't be safe for us
        // to use it without enough bookkeeping to negate any performance gain of using GDI.
        if (!color.HasTransparency())
        {
            using DeviceContextHdcScope hdc = new(e);
            using CreateBrushScope hbrush = new(hdc.FindNearestColor(color));
            hdc.FillRectangle(rectangle, hbrush);
        }
        else if (!color.IsFullyTransparent())
        {
            // Color has some transparency (but not completely transparent) use GDI+.
            using var brush = color.GetCachedSolidBrushScope();
            e.Graphics.FillRectangle(brush, rectangle);
        }
    }

    // Paints a red rectangle with a red X, painted on a white background
    private void PaintException(PaintEventArgs e)
    {
        // As this is unusual we won't cache the pen.
        using Pen pen = new(Color.Red, width: 2);
        Rectangle clientRectangle = ClientRectangle;
        Rectangle rectangle = clientRectangle;
        rectangle.X++;
        rectangle.Y++;
        rectangle.Width--;
        rectangle.Height--;

        e.Graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
        rectangle.Inflate(-1, -1);
        e.Graphics.FillRectangle(Brushes.White, rectangle);
        e.Graphics.DrawLine(pen, clientRectangle.Left, clientRectangle.Top,
                            clientRectangle.Right, clientRectangle.Bottom);
        e.Graphics.DrawLine(pen, clientRectangle.Left, clientRectangle.Bottom,
                            clientRectangle.Right, clientRectangle.Top);
    }

    /// <summary>
    ///  Trick our parent into painting our background for us, or paint some default color if that doesn't work.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is the hardest part of implementing transparent controls; call this in
    ///   <see cref="OnPaintBackground(PaintEventArgs)"/>.
    ///  </para>
    /// </remarks>
    /// <param name="rectangle">The area to redraw.</param>
    /// <param name="transparentRegion">
    ///  Region of the rectangle to be transparent, or null for the entire control.
    /// </param>
    internal unsafe void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle, Region? transparentRegion = null)
    {
        Control? parent = ParentInternal;

        if (parent is null)
        {
            // For whatever reason, our parent can't paint our background, but we need some kind of background
            // since we're transparent.
            using DeviceContextHdcScope hdcNoParent = new(e);
            using CreateBrushScope hbrush = new(Application.SystemColors.Control);
            hdcNoParent.FillRectangle(rectangle, hbrush);
            return;
        }

        // We need to use theming painting for certain controls (like TabPage) when they parent other controls.
        // But we don't want to to this always as this causes serious performance (at Runtime and DesignTime)
        // so checking for RenderTransparencyWithVisualStyles which is TRUE for TabPage and false by default.
        if (Application.RenderWithVisualStyles && parent.RenderTransparencyWithVisualStyles)
        {
            // When we are rendering with visual styles, we can use the cool DrawThemeParentBackground function
            // that UxTheme provides to render the parent's background. This function is control agnostic, so
            // we use the wrapper in ButtonRenderer - this should do the right thing for all controls,
            // not just Buttons.

            if (transparentRegion is not null)
            {
                Graphics g = e.GraphicsInternal;
                using GraphicsStateScope saveState = new(g);
                g.Clip = transparentRegion;
                ButtonRenderer.DrawParentBackground(g, rectangle, this);
            }
            else
            {
                ButtonRenderer.DrawParentBackground(e, rectangle, this);
            }

            return;
        }

        // Move the rendering area and setup it's size (we want to translate it to the parent's origin).
        Rectangle shift = new(-Left, -Top, parent.Width, parent.Height);

        // Moving the clipping rectangle to the parent coordinate system.
        Rectangle newClipRect = new(
            rectangle.Left + Left,
            rectangle.Top + Top,
            rectangle.Width,
            rectangle.Height);

        using DeviceContextHdcScope hdc = new(e);
        using SaveDcScope savedc = new(hdc);

        PInvokeCore.OffsetViewportOrgEx(hdc, -Left, -Top, lppt: null);

        using PaintEventArgs newArgs = new(hdc, newClipRect);

        if (transparentRegion is not null)
        {
            using GraphicsStateScope saveState = new(newArgs.Graphics);

            // Is this clipping something we can apply directly to the HDC?
            newArgs.Graphics.Clip = transparentRegion;
            newArgs.Graphics.TranslateClip(-shift.X, -shift.Y);
            InvokePaintBackground(parent, newArgs);
            InvokePaint(parent, newArgs);
        }
        else
        {
            InvokePaintBackground(parent, newArgs);
            InvokePaint(parent, newArgs);
        }
    }

    private void PaintWithErrorHandling(PaintEventArgs e, short layer)
    {
        try
        {
            CacheTextInternal = true;
            if (GetState(States.ExceptionWhilePainting))
            {
                if (layer == PaintLayerBackground)
                {
                    PaintException(e);
                }

                return;
            }

            try
            {
                switch (layer)
                {
                    case PaintLayerForeground:
                        OnPaint(e);
                        break;
                    case PaintLayerBackground:
                        if (!GetStyle(ControlStyles.Opaque))
                        {
                            OnPaintBackground(e);
                        }

                        break;
                    default:
                        Debug.Fail($"Unknown PaintLayer {layer}");
                        break;
                }
            }
            catch
            {
                // Exceptions during painting are nasty, because paint events happen so often.
                // So if user painting code has an issue, we make sure never to call it again,
                // so as not to spam the end-user with exception dialogs.

                SetState(States.ExceptionWhilePainting, true);
                Invalidate();

                throw;
            }
        }
        finally
        {
            CacheTextInternal = false;
        }
    }

    /// <summary>
    ///  Find ContainerControl that is the container of this control.
    /// </summary>
    internal ContainerControl? ParentContainerControl
    {
        get
        {
            for (Control? c = ParentInternal; c is not null; c = c.ParentInternal)
            {
                if (c is ContainerControl)
                {
                    return c as ContainerControl;
                }
            }

            return null;
        }
    }

    /// <summary>
    ///  Forces the control to apply layout logic to all of the child controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void PerformLayout()
    {
        if (_cachedLayoutEventArgs is not null)
        {
            PerformLayout(_cachedLayoutEventArgs);
            _cachedLayoutEventArgs = null;

            // we need to be careful
            // about which LayoutEventArgs are used in
            // SuspendLayout, PerformLayout, ResumeLayout() sequences.
            SetExtendedState(ExtendedStates.ClearLayoutArgs, false);
        }
        else
        {
            PerformLayout(null, null);
        }
    }

    /// <summary>
    ///  Forces the control to apply layout logic to all of the child controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void PerformLayout(Control? affectedControl, string? affectedProperty)
    {
        PerformLayout(new LayoutEventArgs(affectedControl, affectedProperty));
    }

    internal void PerformLayout(LayoutEventArgs args)
    {
        Debug.Assert(args is not null, "This method should never be called with null args.");
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        if (LayoutSuspendCount > 0)
        {
            SetState(States.LayoutDeferred, true);
            if (_cachedLayoutEventArgs is null || GetExtendedState(ExtendedStates.ClearLayoutArgs))
            {
                _cachedLayoutEventArgs = args;
                if (GetExtendedState(ExtendedStates.ClearLayoutArgs))
                {
                    SetExtendedState(ExtendedStates.ClearLayoutArgs, false);
                }
            }

            LayoutEngine.ProcessSuspendedLayoutEventArgs(this, args);

            return;
        }

        // (Essentially the same as suspending layout while we layout, but we clear differently below.)
        LayoutSuspendCount = 1;

        try
        {
            CacheTextInternal = true;
            OnLayout(args);
        }
        finally
        {
            CacheTextInternal = false;
            // Rather than resume layout (which will could allow a deferred layout to layout the
            // the container we just finished laying out) we set layoutSuspendCount back to zero
            // and clear the deferred and dirty flags.
            SetState(States.LayoutDeferred | States.LayoutIsDirty, false);
            LayoutSuspendCount = 0;

            // LayoutEngine.Layout can return true to request that our parent resize us because
            // we did not have enough room for our contents.  Now that we are unsuspended,
            // see if this happened and layout parent if necessary.  (See also OnLayout)
            if (ParentInternal is not null && ParentInternal.GetState(States.LayoutIsDirty))
            {
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.PreferredSize);
            }
        }
    }

    /// <summary>
    ///  Performs data validation (not paint validation!) on a single control.
    ///
    ///  Returns whether validation failed:
    ///  False = Validation succeeded, control is valid, accept its new value
    ///  True = Validation was cancelled, control is invalid, reject its new value
    ///
    ///  NOTE: This is the lowest possible level of validation. It does not account
    ///  for the context in which the validation is occurring, eg. change of focus
    ///  between controls in a container. Stuff like that is handled by the caller.
    /// </summary>
    internal bool PerformControlValidation(bool bulkValidation)
    {
        // Skip validation for controls that don't support it
        if (!CausesValidation)
        {
            return false;
        }

        // Raise the 'Validating' event. Stop now if handler cancels (ie. control is invalid).
        // NOTE: Handler may throw an exception here, but we must not attempt to catch it.
        if (NotifyValidating())
        {
            return true;
        }

        // Raise the 'Validated' event. Handlers may throw exceptions here too - but
        // convert these to ThreadException events, unless the app is being debugged,
        // or the control is being validated as part of a bulk validation operation.
        if (bulkValidation || NativeWindow.WndProcShouldBeDebuggable)
        {
            NotifyValidated();
        }
        else
        {
            try
            {
                NotifyValidated();
            }
            catch (Exception e)
            {
                Application.OnThreadException(e);
            }
        }

        return false;
    }

    /// <summary>
    ///  Validates all the child controls in a container control. Exactly which controls are
    ///  validated and which controls are skipped is determined by <paramref name="validationConstraints"/>.
    ///  Return value indicates whether validation failed for any of the controls validated.
    ///  Calling function is responsible for checking the correctness of the validationConstraints argument.
    /// </summary>
    internal bool PerformContainerValidation(ValidationConstraints validationConstraints)
    {
        bool failed = false;

        // For every child control of this container control...
        foreach (Control c in Controls)
        {
            // First, if the control is a container, recurse into its descendants.
            if ((validationConstraints & ValidationConstraints.ImmediateChildren) != ValidationConstraints.ImmediateChildren
                && c.ShouldPerformContainerValidation()
                && c.PerformContainerValidation(validationConstraints))
            {
                failed = true;
            }

            // Next, use input flags to decide whether to validate the control itself
            if (((validationConstraints & ValidationConstraints.Selectable) == ValidationConstraints.Selectable && !c.GetStyle(ControlStyles.Selectable))
                || ((validationConstraints & ValidationConstraints.Enabled) == ValidationConstraints.Enabled && !c.Enabled)
                || ((validationConstraints & ValidationConstraints.Visible) == ValidationConstraints.Visible && !c.Visible)
                || ((validationConstraints & ValidationConstraints.TabStop) == ValidationConstraints.TabStop && !c.TabStop))
            {
                continue;
            }

            // Finally, perform validation on the control itself
            if (c.PerformControlValidation(true))
            {
                failed = true;
            }
        }

        return failed;
    }

    /// <summary>
    ///  Computes the location of the screen point p in client coordinates.
    /// </summary>
    public Point PointToClient(Point p)
    {
        PInvoke.MapWindowPoints((HWND)default, this, ref p);
        return p;
    }

    /// <summary>
    ///  Computes the location of the client point p in screen coordinates.
    /// </summary>
    public Point PointToScreen(Point p)
    {
        PInvoke.MapWindowPoints(this, (HWND)default, ref p);
        return p;
    }

    /// <summary>
    ///  This method is called by the application's message loop to pre-process input messages before they
    ///  are dispatched. If this method processes the message it must return true, in which case the message
    ///  loop will not dispatch the message.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The messages that this method handles are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.
    ///  </para>
    ///  <para>
    ///   For WM_KEYDOWN and WM_SYSKEYDOWN messages, this first calls <see cref="ProcessCmdKey(ref Message, Keys)"/>
    ///   to check for command keys such as accelerators and menu shortcuts. If it doesn't process the message, then
    ///   <see cref="IsInputKey(Keys)"/> is called to check whether the key message represents an input key for the
    ///   control. Finally, if <see cref="IsInputKey(Keys)"/> indicates that the control isn't interested in the key
    ///   message, then <see cref="ProcessDialogKey(Keys)"/> is called to check for dialog keys such as TAB, arrow
    ///   keys, and mnemonics.
    ///  </para>
    ///  <para>
    ///   For WM_CHAR messages, <see cref="IsInputChar(char)"/> is first called to check whether the character
    ///   message represents an input character for the control. If <see cref="IsInputChar(char)"/> indicates that
    ///   the control isn't interested in the character message, then <see cref="ProcessDialogChar(char)"/> is
    ///   called to check for dialog characters such as mnemonics.
    ///  </para>
    ///  <para>
    ///   For WM_SYSCHAR messages, this calls <see cref="ProcessDialogChar(char)"/> to check for dialog characters
    ///   such as mnemonics.
    ///  </para>
    ///  <para>
    ///   When overriding this method, a control should return true to indicate that it has processed the message.
    ///   For messages that aren't  processed by the control, the result of "base.PreProcessMessage()" should be
    ///   returned.
    ///  </para>
    ///  <para>
    ///   Controls will typically override one of the more specialized methods (<see cref="IsInputChar(char)"/>,
    ///   <see cref="IsInputKey(Keys)"/>, <see cref="ProcessCmdKey(ref Message, Keys)"/>, <see cref="ProcessDialogChar(char)"/>,
    ///   or <see cref="ProcessDialogKey(Keys)"/>) instead of overriding this method.
    ///  </para>
    /// </remarks>
    public virtual bool PreProcessMessage(ref Message msg)
    {
        bool result;

        if (msg.MsgInternal == PInvoke.WM_KEYDOWN || msg.MsgInternal == PInvoke.WM_SYSKEYDOWN)
        {
            if (!GetExtendedState(ExtendedStates.UiCues))
            {
                ProcessUICues(ref msg);
            }

            Keys keyData = (Keys)(nint)msg.WParamInternal | ModifierKeys;
            if (ProcessCmdKey(ref msg, keyData))
            {
                result = true;
            }
            else if (IsInputKey(keyData))
            {
                SetExtendedState(ExtendedStates.InputKey, true);
                result = false;
            }
            else
            {
                result = ProcessDialogKey(keyData);
            }
        }
        else if (msg.MsgInternal == PInvoke.WM_CHAR || msg.MsgInternal == PInvoke.WM_SYSCHAR)
        {
            if (msg.MsgInternal == PInvoke.WM_CHAR && IsInputChar((char)(nint)msg.WParamInternal))
            {
                SetExtendedState(ExtendedStates.InputChar, true);
                result = false;
            }
            else
            {
                result = ProcessDialogChar((char)(nint)msg.WParamInternal);
            }
        }
        else
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    ///  <see cref="PreProcessControlMessage(ref Message)"/> calls <see cref="PreProcessMessage(ref Message)"/>
    ///  on the <see cref="Control"/> referenced by the <paramref name="msg"/> <see cref="Message.HWnd"/>. It
    ///  handles dispatching of <see cref="OnPreviewKeyDown(PreviewKeyDownEventArgs)"/> and determines whether
    ///  to forward input messages when <see cref="PreProcessMessage(ref Message)"/> indicates it did not handle
    ///  the message by returning false.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is the method that is called directly by the <see cref="Application"/>'s message loop.
    ///   See <see cref="Application.ThreadContext.PreTranslateMessage(ref MSG)"/>.
    ///  </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public PreProcessControlState PreProcessControlMessage(ref Message msg)
        => PreProcessControlMessageInternal(target: null, ref msg);

    internal static PreProcessControlState PreProcessControlMessageInternal(Control? target, ref Message message)
    {
        target ??= FromChildHandle(message.HWnd);

        if (target is null)
        {
            return PreProcessControlState.MessageNotNeeded;
        }

        // Reset state that is used to make sure IsInputChar, IsInputKey and ProcessUICues are not called multiple times.
        target.SetExtendedState(ExtendedStates.InputKey, false);
        target.SetExtendedState(ExtendedStates.InputChar, false);
        target.SetExtendedState(ExtendedStates.UiCues, true);

        try
        {
            Keys keyData = (Keys)(nint)message.WParamInternal | ModifierKeys;

            // Allow control to preview key down message.
            if (message.Msg is ((int)PInvoke.WM_KEYDOWN) or ((int)PInvoke.WM_SYSKEYDOWN))
            {
                target.ProcessUICues(ref message);

                PreviewKeyDownEventArgs args = new(keyData);
                target.OnPreviewKeyDown(args);

                if (args.IsInputKey)
                {
                    // Control wants this message - indicate it should be dispatched.
                    return PreProcessControlState.MessageNeeded;
                }
            }

            PreProcessControlState state = PreProcessControlState.MessageNotNeeded;

            if (!target.PreProcessMessage(ref message))
            {
                if (message.MsgInternal == PInvoke.WM_KEYDOWN || message.MsgInternal == PInvoke.WM_SYSKEYDOWN)
                {
                    // Check if IsInputKey has already processed this message
                    // or if it is safe to call - we only want it to be called once.
                    if (target.GetExtendedState(ExtendedStates.InputKey) || target.IsInputKey(keyData))
                    {
                        state = PreProcessControlState.MessageNeeded;
                    }
                }
                else if (message.MsgInternal == PInvoke.WM_CHAR || message.MsgInternal == PInvoke.WM_SYSCHAR)
                {
                    // Check if IsInputChar has already processed this message
                    // or if it is safe to call - we only want it to be called once.
                    if (target.GetExtendedState(ExtendedStates.InputChar) || target.IsInputChar((char)(nint)message.WParamInternal))
                    {
                        state = PreProcessControlState.MessageNeeded;
                    }
                }
            }
            else
            {
                state = PreProcessControlState.MessageProcessed;
            }

            return state;
        }
        finally
        {
            target.SetExtendedState(ExtendedStates.UiCues, false);
        }
    }

    /// <summary>
    ///  Processes a command key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called during message pre-processing to handle command keys. Command keys are keys that always
    ///   take precedence over regular input keys. Examples of command keys include accelerators and menu shortcuts. The
    ///   method must return <see langword="true"/> to indicate that it has  processed the command key, or
    ///   <see langword="false"/> to indicate that the key is not a command key.
    ///  </para>
    ///  <para>
    ///   If the control has a parent, the key is passed to the parent's <see cref="ProcessCmdKey(ref Message, Keys)"/>
    ///   method. The net effect is that command keys are "bubbled" up the control hierarchy. In addition to the key the
    ///   user pressed, the key data also indicates which, if any, modifier keys were pressed at the same time as the
    ///   key. Modifier keys include the SHIFT, CTRL, and ALT keys.
    ///  </para>
    /// </remarks>
    protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData) =>
        _parent?.ProcessCmdKey(ref msg, keyData) ?? false;

    private unsafe void PrintToMetaFile(HDC hDC, IntPtr lParam)
    {
        Debug.Assert((OBJ_TYPE)PInvokeCore.GetObjectType(hDC) == OBJ_TYPE.OBJ_ENHMETADC,
            "PrintToMetaFile() called with a non-Enhanced MetaFile DC.");
        Debug.Assert((lParam & (long)PInvoke.PRF_CHILDREN) != 0,
            "PrintToMetaFile() called without PRF_CHILDREN.");

        // Strip the PRF_CHILDREN flag.  We will manually walk our children and print them.
        lParam = (nint)(lParam & (long)~PInvoke.PRF_CHILDREN);

        // We're the root control, so we need to set up our clipping region.  Retrieve the
        // x-coordinates and y-coordinates of the viewport origin for the specified device context.
        Point viewportOrg = default;
        bool success = PInvokeCore.GetViewportOrgEx(hDC, &viewportOrg);
        Debug.Assert(success, "GetViewportOrgEx() failed.");

        using RegionScope hClippingRegion = new(
            viewportOrg.X,
            viewportOrg.Y,
            viewportOrg.X + Width,
            viewportOrg.Y + Height);

        Debug.Assert(!hClippingRegion.IsNull, "CreateRectRgn() failed.");

        // Select the new clipping region; make sure it's a SIMPLEREGION or NULLREGION
        GDI_REGION_TYPE selectResult = PInvokeCore.SelectClipRgn(hDC, hClippingRegion);
        Debug.Assert(
            selectResult is GDI_REGION_TYPE.SIMPLEREGION or GDI_REGION_TYPE.NULLREGION,
            "SIMPLEREGION or NULLLREGION expected.");

        PrintToMetaFileRecursive(hDC, lParam, new Rectangle(Point.Empty, Size));
    }

    private protected virtual void PrintToMetaFileRecursive(HDC hDC, IntPtr lParam, Rectangle bounds)
    {
        // We assume the target does not want us to offset the root control in the metafile.

        using DCMapping mapping = new(hDC, bounds);

        // Print the non-client area.
        PrintToMetaFile_SendPrintMessage(hDC, (nint)(lParam & (long)~PInvoke.PRF_CLIENT));

        // Figure out mapping for the client area.
        bool success = PInvoke.GetWindowRect(this, out var windowRect);
        Debug.Assert(success, "GetWindowRect() failed.");
        Point clientOffset = PointToScreen(Point.Empty);
        clientOffset = new(clientOffset.X - windowRect.left, clientOffset.Y - windowRect.top);
        Rectangle clientBounds = new(clientOffset, ClientSize);

        using DCMapping clientMapping = new(hDC, clientBounds);

        // Print the client area.
        PrintToMetaFile_SendPrintMessage(hDC, (nint)(lParam & (long)~PInvoke.PRF_NONCLIENT));

        // Paint children in reverse Z-Order.
        int count = Controls.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            Control child = Controls[i];
            if (child.Visible)
            {
                child.PrintToMetaFileRecursive(hDC, lParam, child.Bounds);
            }
        }
    }

    private void PrintToMetaFile_SendPrintMessage(HDC hDC, nint lParam)
    {
        if (GetStyle(ControlStyles.UserPaint))
        {
            // We let user paint controls paint directly into the metafile
            PInvoke.SendMessage(this, PInvoke.WM_PRINT, (WPARAM)hDC, (LPARAM)lParam);
        }
        else
        {
            // If a system control has no children in the Controls collection we
            // restore the PRF_CHILDREN flag because it may internally
            // have nested children we do not know about.  ComboBox is a
            // good example.
            if (Controls.Count == 0)
            {
                lParam |= PInvoke.PRF_CHILDREN;
            }

            // System controls must be painted into a temporary bitmap
            // which is then copied into the metafile.  (Old GDI line drawing
            // is 1px thin, which causes borders to disappear, etc.)
            using MetafileDCWrapper dcWrapper = new(hDC, Size);
            PInvoke.SendMessage(this, PInvoke.WM_PRINT, (WPARAM)dcWrapper.HDC, (LPARAM)lParam);
        }
    }

    /// <summary>
    ///  Processes a dialog character.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called during message preprocessing to handle dialog characters, such as control mnemonics.
    ///   This method is called only if the <see cref="IsInputChar(char)"/> method indicates that the control is not
    ///   processing the character. The <see cref="ProcessDialogChar(char)"/> method simply sends the character to the
    ///   parent's <see cref="ProcessDialogChar(char)"/> method, or returns <see langword="false"/> if the control has no
    ///   parent. The <see cref="Form"/> class overrides this method to perform actual processing of dialog characters.
    ///  </para>
    /// </remarks>
    protected virtual bool ProcessDialogChar(char charCode) => _parent?.ProcessDialogChar(charCode) ?? false;

    /// <summary>
    ///  Processes a dialog key.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called during message preprocessing to handle dialog characters, such as TAB, RETURN, ESC, and
    ///   arrow keys. This method is called only if the <see cref="IsInputKey(Keys)"/> method indicates that the control
    ///   is not processing the key. The <see cref="ProcessDialogKey(Keys)"/> simply sends the character to the parent's
    ///   <see cref="ProcessDialogKey(Keys)"/> method, or returns <see langword="false"/> if the control has no parent.
    ///   The <see cref="Form"/> class overrides this method to perform actual processing of dialog keys.
    ///  </para>
    /// </remarks>
    protected virtual bool ProcessDialogKey(Keys keyData) => _parent?.ProcessDialogKey(keyData) ?? false;

    /// <summary>
    ///  Processes a key message and generates the appropriate control events.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called when a control receives a keyboard message. The method is responsible for generating the
    ///   appropriate key events for the message by calling the <see cref="OnKeyPress(KeyPressEventArgs)"/>,
    ///   <see cref="OnKeyDown(KeyEventArgs)"/>, or <see cref="OnKeyUp(KeyEventArgs)"/>. The <paramref name="m"/>
    ///   parameter contains the window message that must be processed. Possible values for the <see cref="Message.Msg"/>
    ///   property are WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, WM_SYSKEYUP, and WM_IME_CHAR.
    ///  </para>
    /// </remarks>
    protected virtual bool ProcessKeyEventArgs(ref Message m)
    {
        KeyEventArgs? ke = null;
        KeyPressEventArgs? kpe = null;
        WPARAM newWParam = 0;

        if (m.MsgInternal == PInvoke.WM_CHAR || m.MsgInternal == PInvoke.WM_SYSCHAR)
        {
            int charsToIgnore = ImeWmCharsToIgnore;

            if (charsToIgnore > 0)
            {
                charsToIgnore--;
                ImeWmCharsToIgnore = charsToIgnore;
                return false;
            }
            else
            {
                kpe = new KeyPressEventArgs((char)(int)m.WParamInternal);
                OnKeyPress(kpe);
                newWParam = kpe.KeyChar;
            }
        }
        else if (m.MsgInternal == PInvoke.WM_IME_CHAR)
        {
            int charsToIgnore = ImeWmCharsToIgnore;

            charsToIgnore += (3 - sizeof(char));
            ImeWmCharsToIgnore = charsToIgnore;

            kpe = new KeyPressEventArgs((char)(int)m.WParamInternal);

            char preEventCharacter = kpe.KeyChar;
            OnKeyPress(kpe);

            // If the character wasn't changed, just use the original value rather than round tripping.
            if (kpe.KeyChar == preEventCharacter)
            {
                newWParam = m.WParamInternal;
            }
            else
            {
                newWParam = (WPARAM)kpe.KeyChar;
            }
        }
        else
        {
            ke = new KeyEventArgs((Keys)(int)m.WParamInternal | ModifierKeys);
            if (m.MsgInternal == PInvoke.WM_KEYDOWN || m.MsgInternal == PInvoke.WM_SYSKEYDOWN)
            {
                OnKeyDown(ke);
            }
            else
            {
                OnKeyUp(ke);
            }
        }

        if (kpe is not null)
        {
            m.WParamInternal = newWParam;
            return kpe.Handled;
        }
        else
        {
            if (ke!.SuppressKeyPress)
            {
                RemovePendingMessages(PInvoke.WM_CHAR, PInvoke.WM_CHAR);
                RemovePendingMessages(PInvoke.WM_SYSCHAR, PInvoke.WM_SYSCHAR);
                RemovePendingMessages(PInvoke.WM_IME_CHAR, PInvoke.WM_IME_CHAR);
            }

            return ke.Handled;
        }
    }

    /// <summary>
    ///  Processes a key message.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called when a control receives a keyboard message. The method first determines whether the
    ///   control has a parent; if so, it calls the parent's <see cref="ProcessKeyPreview(ref Message)"/> method. If the
    ///   parent's <see cref="ProcessKeyPreview(ref Message)"/> method does not process the message then the
    ///   <see cref="ProcessKeyEventArgs(ref Message)"/> is called to generate the appropriate keyboard events. The
    ///   <paramref name="m"/> parameter contains the window message that must be processed. Possible values for the
    ///   <see cref="Message.Msg"/> property are WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and WM_SYSKEYUP.
    ///  </para>
    /// </remarks>
    protected internal virtual bool ProcessKeyMessage(ref Message m)
    {
        if (_parent is not null && _parent.ProcessKeyPreview(ref m))
        {
            return true;
        }

        return ProcessKeyEventArgs(ref m);
    }

    /// <summary>
    ///  Previews a keyboard message.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called by a child control when the child control receives a keyboard message. The child control
    ///   calls this method before generating any keyboard events for the message. If this method returns <see langword="true"/>,
    ///   the child control considers the message processed and does not generate any keyboard events. The
    ///   <paramref name="m"/> parameter contains the window message to preview. Possible values for the
    ///   <see cref="Message.Msg"/> property are WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and WM_SYSKEYUP. The
    ///   <see cref="ProcessKeyPreview(ref Message)"/> method simply sends the character to the parent's
    ///   <see cref="ProcessKeyPreview(ref Message)"/> method, or returns <see langword="false"/> if the control has no
    ///   parent. The <see cref="Form"/> class overrides this method to perform actual processing of dialog keys.
    ///  </para>
    /// </remarks>
    protected virtual bool ProcessKeyPreview(ref Message m) => _parent?.ProcessKeyPreview(ref m) ?? false;

    /// <summary>
    ///  Processes a mnemonic character.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is called to give a control the opportunity to process a mnemonic character. The method should
    ///   check if the control is in a state to process mnemonics and if the given  character represents a mnemonic. If
    ///   so, the method should perform the action associated with the mnemonic and return <see langword="true"/>.
    ///   If not, the method should return <see langword="false"/>. Implementations of this method often use the
    ///   <see cref="IsMnemonic(char, string?)"/> method to determine whether the given character matches a mnemonic
    ///   in the control's text.
    ///  </para>
    /// </remarks>
    protected internal virtual bool ProcessMnemonic(char charCode) => false;

    /// <summary>
    ///  Preprocess keys which affect focus indicators and keyboard cues.
    /// </summary>
    internal void ProcessUICues(ref Message msg)
    {
        Keys keyCode = (Keys)(nint)msg.WParamInternal & Keys.KeyCode;

        if (keyCode is not Keys.F10 and not Keys.Menu and not Keys.Tab)
        {
            return;  // PERF: don't WM_QUERYUISTATE if we don't have to.
        }

        Control? topMostParent = null;
        uint current = (uint)PInvoke.SendMessage(this, PInvoke.WM_QUERYUISTATE);

        // don't trust when a control says the accelerators are showing.
        // make sure the topmost parent agrees with this as we could be in a mismatched state.
        if (current == 0 /*accelerator and focus cues are showing*/)
        {
            topMostParent = TopMostParent;
            current = (uint)PInvoke.SendMessage(topMostParent, PInvoke.WM_QUERYUISTATE);
        }

        uint toClear = 0;

        // if we are here, a key or tab has been pressed on this control.
        // now that we know the state of accelerators, check to see if we need
        // to show them.  NOTE: due to the strangeness of the API we OR in
        // the opposite of what we want to do.  So if we want to show accelerators,
        // we OR in UISF_HIDEACCEL, then call UIS_CLEAR to clear the "hidden" state.

        if (keyCode is Keys.F10 or Keys.Menu)
        {
            if ((current & PInvoke.UISF_HIDEACCEL) != 0)
            {
                // Keyboard accelerators are hidden, they need to be shown
                toClear |= PInvoke.UISF_HIDEACCEL;
            }
        }

        if (keyCode == Keys.Tab)
        {
            if ((current & PInvoke.UISF_HIDEFOCUS) != 0)
            {
                // Focus indicators are hidden, they need to be shown
                toClear |= PInvoke.UISF_HIDEFOCUS;
            }
        }

        if (toClear != 0)
        {
            // We've detected some state we need to unset, usually clearing the hidden state of
            // the accelerators.  We need to get the topmost parent and call CHANGEUISTATE so
            // that the entire tree of controls is
            topMostParent ??= TopMostParent;

            // A) if we're parented to a native dialog - REFRESH our child states ONLY
            //       Then we've got to send a WM_UPDATEUISTATE to the topmost managed control (which will be non-toplevel)
            //           (we assume here the root native window has changed UI state, and we're not to manage the UI state for it)
            //
            // B) if we're totally managed - CHANGE the root window state AND REFRESH our child states.
            //       Then we've got to send a WM_CHANGEUISTATE to the topmost managed control (which will be toplevel)
            //       According to MSDN, WM_CHANGEUISTATE will generate WM_UPDATEUISTATE messages for all immediate children (via DefWndProc)
            //           (we're in charge here, we've got to change the state of the root window)
            PInvoke.SendMessage(
                topMostParent,
                PInvoke.GetParent(topMostParent).IsNull ? PInvoke.WM_CHANGEUISTATE : PInvoke.WM_UPDATEUISTATE,
                (WPARAM)((int)PInvoke.UIS_CLEAR | ((int)toClear << 16)));
        }
    }

    /// <summary>
    ///  Raises the event associated with key with the event data of
    ///  e and a sender of this control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void RaiseDragEvent(object key, DragEventArgs e)
    {
        ((DragEventHandler?)Events[key])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the event associated with <paramref name="key"/> with the event data of <paramref name="e"/>
    ///  and a sender of this control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void RaisePaintEvent(object key, PaintEventArgs e)
    {
        ((PaintEventHandler?)Events[s_paintEvent])?.Invoke(this, e);
    }

    private void RemovePendingMessages(MessageId msgMin, MessageId msgMax)
    {
        if (!IsDisposed)
        {
            MSG msg = default;
            while (PInvoke.PeekMessage(&msg, this, (uint)msgMin, (uint)msgMax, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
            {
                // No-op.
            }
        }
    }

    /// <summary>
    ///  Resets the back color to be based on the parent's back color.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetBackColor()
    {
        BackColor = Color.Empty;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetCursor()
    {
        Cursor = null;
    }

    private void ResetEnabled()
    {
        Enabled = true;
    }

    /// <summary>
    ///  Resets the font to be based on the parent's font.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetFont()
    {
        Font = null;
    }

    /// <summary>
    ///  Resets the fore color to be based on the parent's fore color.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetForeColor()
    {
        ForeColor = Color.Empty;
    }

    private void ResetLocation()
    {
        Location = new Point(0, 0);
    }

    private void ResetMargin()
    {
        Margin = DefaultMargin;
    }

    private void ResetMinimumSize()
    {
        MinimumSize = DefaultMinimumSize;
    }

    private void ResetPadding()
    {
        CommonProperties.ResetPadding(this);
    }

    private void ResetSize()
    {
        Size = DefaultSize;
    }

    /// <summary>
    ///  Resets the RightToLeft to be the default.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetRightToLeft()
    {
        RightToLeft = RightToLeft.Inherit;
    }

    /// <summary>
    ///  Forces the recreation of the handle for this control. Inheriting controls
    ///  must call base.RecreateHandle.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void RecreateHandle()
    {
        RecreateHandleCore();
    }

    internal virtual void RecreateHandleCore()
    {
        lock (this)
        {
            if (!IsHandleCreated)
            {
                // Do nothing if the handle is not created yet.
                return;
            }

            bool focused = ContainsFocus;

            Debug.WriteLineIf(CoreSwitches.PerfTrack.Enabled, $"RecreateHandle: {GetType().FullName} [Text={Text}]");

            bool created = GetState(States.Created);
            if (GetState(States.TrackingMouseEvent))
            {
                SetState(States.MouseEnterPending, true);
                UnhookMouseEvent();
            }

            HWND parentHandle = PInvoke.GetParent(this);

            Control?[]? controlSnapshot = null;
            SetState(States.Recreate, true);

            try
            {
                // Inform child controls that their parent is recreating handle.

                // The default behavior is to now SetParent to parking window, then
                // SetParent back after the parent's handle has been recreated.
                // This behavior can be overridden in OnParentHandleRecreat* and is in ListView.

                // fish out control collection w/o demand creating one.
                ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
                if (controlsCollection is not null && controlsCollection.Count > 0)
                {
                    controlSnapshot = new Control[controlsCollection.Count];
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        Control childControl = controlsCollection[i];
                        if (childControl is not null && childControl.IsHandleCreated)
                        {
                            // SetParent to parking window
                            childControl.OnParentHandleRecreating();

                            // if we were successful, remember this control
                            // so we can raise OnParentHandleRecreated
                            controlSnapshot[i] = childControl;
                        }
                        else
                        {
                            // put in a null slot which we'll skip over later.
                            controlSnapshot[i] = null;
                        }
                    }
                }

                // do the main work of recreating the handle
                DestroyHandle();

                // Note that CreateHandle --> _window.CreateHandle may fail due to Dpi awareness setting.
                // By carefully choosing the correct parking window / keeping this and this.Parent Dpi awareness untouched,
                // the call shouldn't fail.
                // However, it could fail if this.CreateParams.Parent is changed outside our control.
                CreateHandle();
            }
            catch (Exception)
            {
                // this.DestroyHandle succeeded, but CreateHandle failed.
                // The control is actually destroyed.
                if (_window.Handle == IntPtr.Zero)
                {
                    SetState(States.Created, false);
                }

                throw;
            }
            finally
            {
                SetState(States.Recreate, false);

                // Inform children their parent's handle has been created.
                // This means
                // an Exception gets thrown before/during the invocation of DestroyHandle.
                //      In this case, GetState(States.Created) == true.
                //      We will restore the Parent value of the (visited) child controls.
                // - or -
                // an Exception gets thrown in CreateHandle.
                //      In this case, _window.Handle will be IntPtr.Zero,
                //      and we should have GetState(States.Created) == false.
                //      Do not go through this if CreateHandle fails (and an Exception is probably on its way bubbling up).
                // - or -
                // CreateHandle is successful.
                //      We will move the child controls to the new parent.
                if (controlSnapshot is not null && IsHandleCreated)
                {
                    for (int i = 0; i < controlSnapshot.Length; i++)
                    {
                        Control? childControl = controlSnapshot[i];
                        if (childControl is not null && childControl.IsHandleCreated)
                        {
                            // Re-parent the control.
                            // If the control fails to re-parent itself,
                            // It and its next siblings will keep States.ParentRecreating state,
                            // parked in ParkingWindow.
                            // We let the error bubble up immediately.
                            childControl.OnParentHandleRecreated();
                        }
                    }
                }
            }

            if (created)
            {
                CreateControl();
            }

            if (
                // The window has a parent Win32 window before re-creation
                !parentHandle.IsNull
                // But the parent is not a managed WinForm Control, or this.Parent is null
                && (FromHandle(parentHandle) is null || _parent is null)
                // Still, parentHandle is a valid native Win32 window handle, e.g. the desktop window.
                && PInvoke.IsWindow(parentHandle))
            {
                // correctly parent back up to where we were before.
                // if we were parented to a proper windows forms control, CreateControl would have properly parented
                // us back.
                if (PInvoke.SetParent(this, parentHandle) == IntPtr.Zero)
                {
                    // Somehow we failed to SetParent due to, e.g., different Dpi awareness setting.
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32SetParentFailed);
                }
            }

            // Restore control focus
            if (focused)
            {
                Focus();
            }

            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Computes the location of the screen rectangle r in client coordinates.
    /// </summary>
    public Rectangle RectangleToClient(Rectangle r)
    {
        RECT rect = r;
        PInvoke.MapWindowPoints(HWND.Null, this, ref rect);
        return rect;
    }

    /// <summary>
    ///  Computes the location of the client rectangle r in screen coordinates.
    /// </summary>
    public Rectangle RectangleToScreen(Rectangle r)
    {
        RECT rect = r;
        PInvoke.MapWindowPoints(this, HWND.Null, ref rect);
        return rect;
    }

    /// <summary>
    ///  Reflects the specified message to the control that is bound to the specified handle.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the message was reflected; otherwise, <see langword="false"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static bool ReflectMessage(IntPtr hWnd, ref Message m)
    {
        if (FromHandle(hWnd) is not { } control)
        {
            return false;
        }

        m.ResultInternal = PInvoke.SendMessage(
            control,
            MessageId.WM_REFLECT | m.MsgInternal,
            m.WParamInternal,
            m.LParamInternal);

        return true;
    }

    /// <summary>
    ///  Forces the control to invalidate and immediately repaint itself and any children.
    /// </summary>
    public virtual void Refresh()
    {
        Invalidate(invalidateChildren: true);
        Update();
    }

    /// <summary>
    ///  Releases UI Automation provider for specified window.
    /// </summary>
    /// <param name="handle">The window handle.</param>
    internal virtual void ReleaseUiaProvider(HWND handle)
    {
        if (!handle.IsNull)
        {
            // When a window that previously returned providers has been destroyed,
            // you should notify UI Automation by calling the UiaReturnRawElementProvider
            // as follows: UiaReturnRawElementProvider(hwnd, 0, 0, NULL). This call tells
            // UI Automation that it can safely remove all map entries that refer to the specified window.
            PInvoke.UiaReturnRawElementProvider(handle, 0, 0, (IRawElementProviderSimple*)null);
        }

        if (OsVersion.IsWindows8OrGreater() && TryGetAccessibilityObject(out AccessibleObject? accessibleObject))
        {
            PInvoke.UiaDisconnectProvider(accessibleObject, skipOSCheck: true);
        }

        Properties.SetObject(s_accessibilityProperty, null);
    }

    private protected bool TryGetAccessibilityObject([NotNullWhen(true)] out AccessibleObject? accessibleObject)
    {
        accessibleObject = Properties.GetObject(s_accessibilityProperty) as AccessibleObject;
        return accessibleObject is not null;
    }

    /// <summary>
    ///  Resets the mouse leave listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void ResetMouseEventArgs()
    {
        if (GetState(States.TrackingMouseEvent))
        {
            UnhookMouseEvent();
            HookMouseEvent();
        }
    }

    /// <summary>
    ///  Resets the text to it's default value.
    /// </summary>
    public virtual void ResetText()
    {
        Text = string.Empty;
    }

    private void ResetVisible()
    {
        Visible = true;
    }

    /// <summary>
    ///  Resumes normal layout logic. This will force a layout immediately
    ///  if there are any pending layout requests.
    /// </summary>
    public void ResumeLayout() => ResumeLayout(performLayout: true);

    /// <summary>
    ///  Resumes normal layout logic. If performLayout is set to true then
    ///  this will force a layout immediately if there are any pending layout requests.
    /// </summary>
    public void ResumeLayout(bool performLayout)
    {
        bool performedLayout = false;
        if (LayoutSuspendCount > 0)
        {
            if (LayoutSuspendCount == 1)
            {
                LayoutSuspendCount++;
                try
                {
                    OnLayoutResuming(performLayout);
                }
                finally
                {
                    LayoutSuspendCount--;
                }
            }

            LayoutSuspendCount--;
            if (LayoutSuspendCount == 0 && GetState(States.LayoutDeferred) && performLayout)
            {
                PerformLayout();
                performedLayout = true;
            }
        }

        if (!performedLayout)
        {
            SetExtendedState(ExtendedStates.ClearLayoutArgs, true);
        }

        // We've had this since Everett, but it seems wrong, redundant and a performance hit. The
        // correct layout calls are already made when bounds or parenting changes, which is all
        // we care about. We may want to call this at layout suspend count == 0, but certainly
        // not for all resumes. I  tried removing it, and doing it only when suspendCount == 0,
        // but we break things at every step.

        if (!performLayout)
        {
            CommonProperties.xClearPreferredSizeCache(this);
            ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

            // PERFNOTE: This is more efficient than using Foreach. Foreach forces the creation of an array subset
            // enum each time we enumerate.
            if (controlsCollection is not null)
            {
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    Control control = controlsCollection[i];
                    LayoutEngine.InitLayout(control, BoundsSpecified.All);
                    CommonProperties.xClearPreferredSizeCache(control);
                }
            }
        }
    }

    /// <summary>
    ///  Used to actually register the control as a drop target.
    /// </summary>
    internal void SetAcceptDrops(bool accept)
    {
        if (accept == GetState(States.DropTarget) || !IsHandleCreated)
        {
            return;
        }

        try
        {
            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            if (accept)
            {
                // Register
                HRESULT hr = PInvoke.RegisterDragDrop(this, new DropTarget(this));
                if (hr != HRESULT.S_OK && hr != HRESULT.DRAGDROP_E_ALREADYREGISTERED)
                {
                    throw Marshal.GetExceptionForHR((int)hr)!;
                }
            }
            else
            {
                // Revoke
                HRESULT hr = PInvoke.RevokeDragDrop(this);
                if (hr != HRESULT.S_OK && hr != HRESULT.DRAGDROP_E_NOTREGISTERED)
                {
                    throw Marshal.GetExceptionForHR((int)hr)!;
                }
            }

            SetState(States.DropTarget, accept);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(SR.DragDropRegFailed, e);
        }
    }

    /// <summary>
    ///  Scales to entire control and any child controls.
    /// </summary>
    [Obsolete("This method has been deprecated. Use the Scale(SizeF ratio) method instead. https://go.microsoft.com/fwlink/?linkid=14202")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Scale(float ratio)
    {
        ScaleCore(ratio, ratio);
    }

    /// <summary>
    ///  Scales the entire control and any child controls.
    /// </summary>
    [Obsolete("This method has been deprecated. Use the Scale(SizeF ratio) method instead. https://go.microsoft.com/fwlink/?linkid=14202")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Scale(float dx, float dy)
    {
        using SuspendLayoutScope scope = new(this);
        ScaleCore(dx, dy);
    }

    /// <summary>
    ///  Scales a control and its children given a scaling factor.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void Scale(SizeF factor)
    {
        // Manually call ScaleControl recursively instead of the internal scale method
        // when someone calls this method, they really do want to do some sort of
        // zooming feature, as opposed to AutoScale.
        using (new LayoutTransaction(this, this, PropertyNames.Bounds, resumeLayout: false))
        {
            ScaleControl(factor, factor);
            if (ScaleChildren)
            {
                ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
                if (controlsCollection is not null)
                {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        Control c = controlsCollection[i];
                        c.Scale(factor);
                    }
                }
            }
        }

        LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
    }

    /// <summary>
    /// Scales control and its children given a pair of scaling factors.
    /// IncludedFactor will be applied to the dimensions of controls based on
    /// their <see cref="RequiredScaling"/> property.  For example, if a control's
    /// RequiredScaling property returns Width, the width of the control will
    /// be scaled according to the includedFactor value.
    /// </summary>
    /// <param name="includedFactor">Control bounds that are included in <see cref="RequiredScaling"/>.
    /// If the factor is empty, it indicates that no scaling of those control dimensions should be done.</param>
    /// <param name="excludedFactor">Control bounds that are not included in <see cref="RequiredScaling"/>.
    /// If the factor is empty, it indicates that no scaling of those control dimensions should be done.</param>
    /// <param name="requestingControl">Control that has requested the scaling function.</param>
    /// <param name="causedByFontChanged">Indicates if it need to update Window font for controls
    /// that need it, i.e. controls using default or inherited font, that are also not user-painted.</param>
    internal virtual void Scale(SizeF includedFactor, SizeF excludedFactor, Control requestingControl, bool causedByFontChanged = false)
    {
        // When we scale, we are establishing new baselines for the
        // positions of all controls.  Therefore, we should resume(false).
        using (new LayoutTransaction(this, this, PropertyNames.Bounds, false))
        {
            ScaleControl(includedFactor, excludedFactor);

            // Certain controls like 'PropertyGrid' does special scaling. Differing scaling to their own methods.
            if (!_doNotScaleChildren)
            {
                ScaleChildControls(includedFactor, excludedFactor, requestingControl, causedByFontChanged);
            }
        }

        LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
    }

    /// <summary>
    /// Scales control and its children given a pair of scaling factors.
    /// IncludedFactor will be applied to the dimensions of controls based on
    /// their <see cref="RequiredScaling"/> property.  For example, if a control's
    /// RequiredScaling property returns Width, the width of the control will
    /// be scaled according to the includedFactor value.
    /// </summary>
    /// <param name="includedFactor">Control bounds that are included in <see cref="RequiredScaling"/>.
    /// If the factor is empty, it indicates that no scaling of those control dimensions should be done.</param>
    /// <param name="excludedFactor">Control bounds that are not included in <see cref="RequiredScaling"/>.
    /// If the factor is empty, it indicates that no scaling of those control dimensions should be done.</param>
    /// <param name="requestingControl">Control that has requested the scaling function.</param>
    /// <param name="causedByFontChanged">Indicates if it need to update Window font for controls
    /// that need it, i.e. controls using default or inherited font, that are also not user-painted.</param>
    internal void ScaleChildControls(SizeF includedFactor, SizeF excludedFactor, Control requestingControl, bool causedByFontChanged = false)
    {
        if (ScaleChildren)
        {
            var controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

            if (controlsCollection is not null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    Control control = controlsCollection[i];

                    // ContainerControls get their own OnFontChanged Events and scale.
                    // If this scaling is caused by ResumeLayout instead of OnFontChanged,
                    // We would be scaling all container controls.
                    if (control is ContainerControl && causedByFontChanged)
                    {
                        continue;
                    }

                    // Update window font before scaling, as controls often use font metrics during scaling.
                    if (causedByFontChanged)
                    {
                        if (ScaleHelper.IsScalingRequirementMet && !GetStyle(ControlStyles.UserPaint) && !IsFontSet())
                        {
                            SetWindowFont();
                        }
                    }

                    control.Scale(includedFactor, excludedFactor, requestingControl, causedByFontChanged);
                }
            }
        }
    }

    /// <summary>
    ///  Scales the children of this control.  The default implementation walks the controls
    ///  collection for the control and calls Scale on each control.
    ///  IncludedFactor will be applied to the dimensions of controls based on
    ///  their RequiredScaling property.  For example, if a control's
    ///  RequiredScaling property returns Width, the width of the control will
    ///  be scaled according to the includedFactor value.
    ///
    ///  The excludedFactor parameter is used to scale those control bounds who
    ///  are not included in RequiredScaling.
    ///
    ///  If a factor is empty, it indicates that no scaling of those control
    ///  dimensions should be done.
    ///
    ///  The requestingControl property indicates which control has requested
    ///  the scaling function.
    /// </summary>
    internal void ScaleControl(SizeF includedFactor, SizeF excludedFactor)
    {
        try
        {
            ScalingInProgress = true;

            BoundsSpecified includedSpecified = BoundsSpecified.None;
            BoundsSpecified excludedSpecified = BoundsSpecified.None;

            if (!includedFactor.IsEmpty)
            {
                includedSpecified = RequiredScaling;
            }

            if (!excludedFactor.IsEmpty)
            {
                excludedSpecified |= (~RequiredScaling & BoundsSpecified.All);
            }

            if (includedSpecified != BoundsSpecified.None)
            {
                ScaleControl(includedFactor, includedSpecified);
            }

            if (excludedSpecified != BoundsSpecified.None)
            {
                ScaleControl(excludedFactor, excludedSpecified);
            }

            if (!includedFactor.IsEmpty)
            {
                RequiredScaling = BoundsSpecified.None;
            }
        }
        finally
        {
            ScalingInProgress = false;
        }
    }

    /// <summary>
    ///  Scales an individual control's location, size, padding and margin.
    ///  If the control is top level, this will not scale the control's location.
    ///  This does not scale children or the size of auto sized controls.  You can
    ///  omit scaling in any direction by changing BoundsSpecified.
    ///
    ///  After the control is scaled the RequiredScaling property is set to
    ///  BoundsSpecified.None.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        CreateParams cp = CreateParams;
        RECT adornments = default;
        AdjustWindowRectExForControlDpi(ref adornments, (WINDOW_STYLE)cp.Style, false, (WINDOW_EX_STYLE)cp.ExStyle);
        Size minSize = MinimumSize;
        Size maxSize = MaximumSize;

        // clear out min and max size, otherwise this could affect the scaling logic.
        MinimumSize = Size.Empty;
        MaximumSize = Size.Empty;

        // this is raw because Min/Max size have been cleared at this point.
        Rectangle rawScaledBounds = GetScaledBounds(Bounds, factor, specified);

        //
        // Scale Padding and Margin
        //
        float dx = factor.Width;
        float dy = factor.Height;

        Padding padding = Padding;
        Padding margins = Margin;

        // Clear off specified bits for 1.0 scaling factors
        if (dx == 1.0F)
        {
            specified &= ~(BoundsSpecified.X | BoundsSpecified.Width);
        }

        if (dy == 1.0F)
        {
            specified &= ~(BoundsSpecified.Y | BoundsSpecified.Height);
        }

        if (dx != 1.0F)
        {
            padding.Left = (int)Math.Round(padding.Left * dx);
            padding.Right = (int)Math.Round(padding.Right * dx);
            margins.Left = (int)Math.Round(margins.Left * dx);
            margins.Right = (int)Math.Round(margins.Right * dx);
        }

        if (dy != 1.0F)
        {
            padding.Top = (int)Math.Round(padding.Top * dy);
            padding.Bottom = (int)Math.Round(padding.Bottom * dy);
            margins.Top = (int)Math.Round(margins.Top * dy);
            margins.Bottom = (int)Math.Round(margins.Bottom * dy);
        }

        // Apply padding and margins
        Padding = padding;
        Margin = margins;

        //
        // Scale Min/Max size
        //

        // make sure we consider the adornments as fixed.  rather than scaling the entire size,
        // we should pull out the fixed things such as the border, scale the rest, then apply the fixed
        // adornment size.
        Size adornmentSize = adornments.Size;
        if (!minSize.IsEmpty)
        {
            minSize -= adornmentSize;
            minSize = ScaleSize(LayoutUtils.UnionSizes(Size.Empty, minSize), // make sure we don't go below 0.
                                    factor.Width,
                                    factor.Height) + adornmentSize;
        }

        if (!maxSize.IsEmpty)
        {
            maxSize -= adornmentSize;
            maxSize = ScaleSize(LayoutUtils.UnionSizes(Size.Empty, maxSize), // make sure we don't go below 0.
                                    factor.Width,
                                    factor.Height) + adornmentSize;
        }

        // Apply the min/max size constraints - don't call ApplySizeConstraints
        // as MinimumSize/MaximumSize are currently cleared out.
        Size maximumSize = LayoutUtils.ConvertZeroToUnbounded(maxSize);
        Size scaledSize = LayoutUtils.IntersectSizes(rawScaledBounds.Size, maximumSize);
        scaledSize = LayoutUtils.UnionSizes(scaledSize, minSize);

        if (ScaleHelper.IsScalingRequirementMet
            && ParentInternal is { } parent
            && (parent.LayoutEngine == DefaultLayout.Instance))
        {
            // We need to scale AnchorInfo to update distances to container edges
            DefaultLayout.ScaleAnchorInfo(this, factor);
        }

        // Set in the scaled bounds as constrained by the newly scaled min/max size.
        SetBoundsCore(rawScaledBounds.X, rawScaledBounds.Y, scaledSize.Width, scaledSize.Height, BoundsSpecified.All);

        MaximumSize = maxSize;
        MinimumSize = minSize;
    }

    /// <summary>
    ///  Performs the work of scaling the entire control and any child controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void ScaleCore(float dx, float dy)
    {
        using SuspendLayoutScope scope = new(this);

        int sx = (int)Math.Round(_x * dx);
        int sy = (int)Math.Round(_y * dy);

        int sw = _width;
        if ((_controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth)
        {
            sw = (int)(Math.Round((_x + _width) * dx)) - sx;
        }

        int sh = _height;
        if ((_controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight)
        {
            sh = (int)(Math.Round((_y + _height) * dy)) - sy;
        }

        SetBounds(sx, sy, sw, sh, BoundsSpecified.All);

        ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);

        if (controlsCollection is not null)
        {
            // PERFNOTE: This is more efficient than using Foreach.  Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
#pragma warning disable CS0618 // Type or member is obsolete - compat
                controlsCollection[i].Scale(dx, dy);
#pragma warning restore CS0618
            }
        }
    }

    /// <summary>
    ///  Scales a given size with the provided values.
    /// </summary>
    internal Size ScaleSize(Size startSize, float x, float y)
    {
        Size size = startSize;
        if (!GetStyle(ControlStyles.FixedWidth))
        {
            size.Width = (int)Math.Round(size.Width * x);
        }

        if (!GetStyle(ControlStyles.FixedHeight))
        {
            size.Height = (int)Math.Round(size.Height * y);
        }

        return size;
    }

    /// <summary>
    ///  Activates this control.
    /// </summary>
    public void Select()
    {
        Select(false, false);
    }

    // used by Form
    protected virtual void Select(bool directed, bool forward)
    {
        IContainerControl? c = GetContainerControl();

        if (c is not null)
        {
            c.ActiveControl = this;
        }
    }

    /// <summary>
    ///  Selects the next control following ctl.
    /// </summary>
    public bool SelectNextControl(Control? ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
    {
        Control? nextSelectableControl = GetNextSelectableControl(ctl, forward, tabStopOnly, nested, wrap);
        if (nextSelectableControl is not null)
        {
            nextSelectableControl.Select(true, forward);
            return true;
        }
        else
        {
            return false;
        }
    }

    private Control? GetNextSelectableControl(Control? ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
    {
        if (!Contains(ctl) || (!nested && ctl._parent != this))
        {
            ctl = null;
        }

        bool alreadyWrapped = false;
        Control? start = ctl;
        do
        {
            ctl = GetNextControl(ctl, forward);
            if (ctl is null)
            {
                if (!wrap)
                {
                    break;
                }

                if (alreadyWrapped)
                {
                    return null; // prevent infinite wrapping.
                }

                alreadyWrapped = true;
            }
            else
            {
                if (ctl.CanSelect
                    && (!tabStopOnly || ctl.TabStop)
                    && (nested || ctl._parent == this))
                {
                    if (ctl._parent is ToolStrip)
                    {
                        continue;
                    }

                    return ctl;
                }
            }
        }
        while (ctl != start);

        return null;
    }

    /// <summary>
    ///  This is called recursively when visibility is changed for a control, this
    ///  forces focus to be moved to a visible control.
    /// </summary>
    private void SelectNextIfFocused()
    {
        // We want to move focus away from hidden controls, so this function was added.
        if (ContainsFocus && ParentInternal is not null)
        {
            IContainerControl? c = ParentInternal.GetContainerControl();

            if (c is not null)
            {
                ((Control)c).SelectNextControl(this, true, true, true, true);
            }
        }
    }

    /// <summary>
    ///  sends this control to the back of the z-order
    /// </summary>
    public void SendToBack()
    {
        if (_parent is not null)
        {
            _parent.Controls.SetChildIndex(this, -1);
        }
        else if (IsHandleCreated && GetTopLevel())
        {
            PInvoke.SetWindowPos(
                this,
                HWND.HWND_BOTTOM,
                0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
        }
    }

    /// <summary>
    ///  Sets the bounds of the control.
    /// </summary>
    public void SetBounds(int x, int y, int width, int height)
    {
        if (_x != x || _y != y || _width != width || _height != height)
        {
            _forceAnchorCalculations = LocalAppContextSwitches.AnchorLayoutV2;
            try
            {
                SetBoundsCore(x, y, width, height, BoundsSpecified.All);
            }
            finally
            {
                _forceAnchorCalculations = false;
            }

            // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
            // have refreshed the interior layout.  We only need to layout the parent.
            LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
        }
        else
        {
            // Still need to init scaling.
            InitScaling(BoundsSpecified.All);
        }
    }

    /// <summary>
    ///  Sets the bounds of the control.
    /// </summary>
    public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if ((specified & BoundsSpecified.X) == BoundsSpecified.None)
        {
            x = _x;
        }

        if ((specified & BoundsSpecified.Y) == BoundsSpecified.None)
        {
            y = _y;
        }

        if ((specified & BoundsSpecified.Width) == BoundsSpecified.None)
        {
            width = _width;
        }

        if ((specified & BoundsSpecified.Height) == BoundsSpecified.None)
        {
            height = _height;
        }

        if (_x != x || _y != y || _width != width || _height != height)
        {
            _forceAnchorCalculations = LocalAppContextSwitches.AnchorLayoutV2;
            try
            {
                SetBoundsCore(x, y, width, height, specified);
            }
            finally
            {
                _forceAnchorCalculations = false;
            }

            // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
            // have refreshed the interior layout or the resized control.  We only need to layout
            // the parent.  This happens after InitLayout has been invoked.
            LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
        }
        else
        {
            // Still need to init scaling.
            InitScaling(specified);
        }
    }

    /// <summary>
    ///  Performs the work of setting the specified bounds of this control.
    /// </summary>
    /// <param name="x">The new <see cref="Left" /> property value of the control.</param>
    /// <param name="y">The new <see cref="Top" /> property value of the control.</param>
    /// <param name="width">The new <see cref="Width" /> property value of the control.</param>
    /// <param name="height">The new <see cref="Height" /> property value of the control.</param>
    /// <param name="specified">A bitwise combination of the <see cref="BoundsSpecified"/> values.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        // SetWindowPos below sends a WmWindowPositionChanged (not posts) so we immediately
        // end up in WmWindowPositionChanged which may cause the parent to layout.  We need to
        // suspend/resume to defer the parent from laying out until after InitLayout has been called
        // to update the layout engine's state with the new control bounds.

        if (_x == x && _y == y && _width == width && _height == height)
        {
            return;
        }

        using SuspendLayoutScope scope = new(ParentInternal);

        try
        {
            CommonProperties.UpdateSpecifiedBounds(this, x, y, width, height, specified);

            // Provide control with an opportunity to apply self imposed constraints on its size.
            Rectangle adjustedBounds = ApplyBoundsConstraints(x, y, width, height);
            width = adjustedBounds.Width;
            height = adjustedBounds.Height;
            x = adjustedBounds.X;
            y = adjustedBounds.Y;

            if (!IsHandleCreated)
            {
                // Handle is not created, just record our new position and we're done.
                UpdateBounds(x, y, width, height);
            }
            else
            {
                if (!GetState(States.SizeLockedByOS))
                {
                    SET_WINDOW_POS_FLAGS flags = SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE;

                    if (_x == x && _y == y)
                    {
                        flags |= SET_WINDOW_POS_FLAGS.SWP_NOMOVE;
                    }

                    if (_width == width && _height == height)
                    {
                        flags |= SET_WINDOW_POS_FLAGS.SWP_NOSIZE;
                    }

                    // Give a chance for derived controls to do what they want, just before we resize.
                    OnBoundsUpdate(x, y, width, height);

                    PInvoke.SetWindowPos(this, HWND.Null, x, y, width, height, flags);

                    // NOTE: SetWindowPos causes a WM_WINDOWPOSCHANGED which is processed
                    // synchronously so we effectively end up in UpdateBounds immediately following
                    // SetWindowPos.
                    //
                    // UpdateBounds(x, y, width, height);
                }
            }
        }
        finally
        {
            // Initialize the scaling engine.
            InitScaling(specified);

            if (ParentInternal is not null)
            {
                // Some layout engines (DefaultLayout) base their PreferredSize on
                // the bounds of their children.  If we change change the child bounds, we
                // need to clear their PreferredSize cache.  The semantics of SetBoundsCore
                // is that it does not cause a layout, so we just clear.
                CommonProperties.xClearPreferredSizeCache(ParentInternal);

                // Cause the current control to initialize its layout (e.g., Anchored controls
                // memorize their distance from their parent's edges).  It is your parent's
                // LayoutEngine which manages your layout, so we call into the parent's
                // LayoutEngine.
                ParentInternal.LayoutEngine.InitLayout(this, specified);
            }
        }
    }

    /// <summary>
    ///  Performs the work of setting the size of the client area of the control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void SetClientSizeCore(int x, int y)
    {
        Size = SizeFromClientSizeInternal(new(x, y));
        _clientWidth = x;
        _clientHeight = y;
        OnClientSizeChanged(EventArgs.Empty);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual Size SizeFromClientSize(Size clientSize) => SizeFromClientSizeInternal(clientSize);

    internal Size SizeFromClientSizeInternal(Size size)
    {
        RECT rect = new(size);
        CreateParams cp = CreateParams;
        AdjustWindowRectExForControlDpi(ref rect, (WINDOW_STYLE)cp.Style, false, (WINDOW_EX_STYLE)cp.ExStyle);
        return rect.Size;
    }

    private void SetHandle(IntPtr value)
    {
        if (value == IntPtr.Zero)
        {
            SetState(States.Created, false);
        }

        UpdateRoot();
    }

    private void SetParentHandle(HWND value)
    {
        Debug.Assert(value != -1, "Outdated call to SetParentHandle");

        if (IsHandleCreated)
        {
            HWND parentHandle = PInvoke.GetParent(this);
            bool topLevel = GetTopLevel();
            if (parentHandle != value || (parentHandle.IsNull && !topLevel))
            {
                Debug.Assert(Handle != value, "Cycle created in SetParentHandle");

                bool recreate = (parentHandle.IsNull && !topLevel)
                    || (value.IsNull && topLevel);

                if (recreate)
                {
                    // We will recreate later, when the MdiChild's visibility
                    // is set to true (see
                    if (this is Form f)
                    {
                        if (!f.CanRecreateHandle())
                        {
                            recreate = false;
                            // we don't want to recreate - but our styles may have changed.
                            // before we unpark the window below we need to update
                            UpdateStyles();
                        }
                    }
                }

                if (recreate)
                {
                    RecreateHandle();
                }

                if (!GetTopLevel())
                {
                    if (value.IsNull)
                    {
                        Application.ParkHandle(handle: new(this), DpiAwarenessContext);
                        UpdateRoot();
                    }
                    else
                    {
                        if (PInvoke.SetParent(this, value).IsNull)
                        {
                            // Somehow we failed to SetParent, e.g. due to different Dpi awareness setting.
                            // Throwing exception will keep the handle parked inside ParkingWindow if recreate == true.
                            throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32SetParentFailed);
                        }

                        _parent?.UpdateChildZOrder(this);

                        Application.UnparkHandle(this, _window.DpiAwarenessContext);
                    }
                }
            }
            else if (value.IsNull && parentHandle.IsNull && topLevel)
            {
                // The handle was previously parented to the parking window. Its TopLevel property was
                // then changed to true so the above call to GetParent returns null even though the parent of the control is
                // not null. We need to explicitly set the parent to null.
                if (PInvoke.SetParent(this, HWND.Null).IsNull)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32SetParentFailed);
                }

                Application.UnparkHandle(this, _window.DpiAwarenessContext);
            }
        }
    }

    private protected void SetState(States flag, bool value)
    {
        _state = value ? _state | flag : _state & ~flag;
    }

    private protected void SetExtendedState(ExtendedStates flag, bool value)
    {
        _extendedState = value ? _extendedState | flag : _extendedState & ~flag;
    }

    /// <summary>
    ///  Sets the current value of the specified bit in the control's style.
    ///  NOTE: This is control style, not the Win32 style of the hWnd.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void SetStyle(ControlStyles flag, bool value)
    {
        // WARNING: if we ever add argument checking to "flag", we will need
        // to move private styles like Layered to State.
        _controlStyle = value ? _controlStyle | flag : _controlStyle & ~flag;
    }

    internal virtual void SetToolTip(ToolTip toolTip)
    {
        // Control doesn't have a specific logic after a toolTip is set
    }

    protected void SetTopLevel(bool value)
    {
        if (value && IsActiveX)
        {
            throw new InvalidOperationException(SR.TopLevelNotAllowedIfActiveX);
        }
        else
        {
            SetTopLevelInternal(value);
        }
    }

    private protected void SetTopLevelInternal(bool value)
    {
        if (GetTopLevel() != value)
        {
            if (_parent is not null)
            {
                throw new ArgumentException(SR.TopLevelParentedControl, nameof(value));
            }

            SetState(States.TopLevel, value);

            UpdateStyles();
            SetParentHandle(default);
            if (value && Visible)
            {
                CreateControl();
            }

            UpdateRoot();
        }
    }

    private static unsafe void PrepareDarkMode(HWND hwnd, bool darkModeEnabled)
    {
        BOOL value = darkModeEnabled;

        PInvoke.DwmSetWindowAttribute(
            hwnd,
            DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
            &value,
            (uint)sizeof(BOOL));
    }

    protected virtual void SetVisibleCore(bool value)
    {
        if (value != Visible)
        {
            if (!value)
            {
                SelectNextIfFocused();
            }

            bool fireChange = false;

            if (GetTopLevel())
            {
                // The processing of WmShowWindow will set the visibility
                // bit and call CreateControl()
                if (IsHandleCreated || value)
                {
                    if (value)
                    {
                        PrepareDarkMode(HWND, IsDarkModeEnabled);
                    }

                    PInvoke.ShowWindow(HWND, value ? ShowParams : SHOW_WINDOW_CMD.SW_HIDE);
                }
            }
            else if (IsHandleCreated || (value && _parent?.Created == true))
            {
                // We want to mark the control as visible so that CreateControl
                // knows that we are going to be displayed... however in case
                // an exception is thrown, we need to back the change out.

                SetState(States.Visible, value);
                fireChange = true;
                try
                {
                    if (value)
                    {
                        CreateControl();
                    }

                    PInvoke.SetWindowPos(
                        this,
                        HWND.Null,
                        0, 0, 0, 0,
                        SET_WINDOW_POS_FLAGS.SWP_NOSIZE
                            | SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                            | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
                            | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                            | (value ? SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW : SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW));
                }
                catch
                {
                    SetState(States.Visible, !value);
                    throw;
                }
            }

            if (value != Visible)
            {
                SetState(States.Visible, value);
                fireChange = true;
            }

            if (fireChange)
            {
                // We do not do this in the OnPropertyChanged event for visible
                // Lots of things could cause us to become visible, including a
                // parent window.  We do not want to indiscriminately layout
                // due to this, but we do want to layout if the user changed
                // our visibility.
                using (new LayoutTransaction(_parent, this, PropertyNames.Visible))
                {
                    OnVisibleChanged(EventArgs.Empty);
                }
            }

            UpdateRoot();
        }
        else
        {
            // value of Visible property not changed, but raw bit may have

            if (!DesiredVisibility && !value && IsHandleCreated)
            {
                // PERF - setting Visible=false twice can get us into this else block
                // which makes us process WM_WINDOWPOS* messages - make sure we've already
                // visible=false - if not, make it so.
                if (!PInvoke.IsWindowVisible(this))
                {
                    // we're already invisible - bail.
                    return;
                }
            }

            SetState(States.Visible, value);

            // If the handle is already created, we need to update the window style.
            // This situation occurs when the parent control is not currently visible,
            // but the child control has already been created.
            if (IsHandleCreated)
            {
                PInvoke.SetWindowPos(
                    this,
                    HWND.HWND_TOP,
                    0, 0, 0, 0,
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE
                        | SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                        | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
                        | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                        | (value ? SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW : SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW));
            }
        }
    }

    /// <summary>
    ///  Determine effective auto-validation setting for a given control, based on the AutoValidate property
    ///  of its containing control. Defaults to 'EnablePreventFocusChange' if there is no containing control
    ///  (eg. because this control is a top-level container).
    /// </summary>
    internal static AutoValidate GetAutoValidateForControl(Control control)
    {
        ContainerControl? parent = control.ParentContainerControl;
        return (parent is not null) ? parent.AutoValidate : AutoValidate.EnablePreventFocusChange;
    }

    /// <summary>
    ///  Is auto-validation currently in effect for this control?
    ///  Depends on the AutoValidate property of the containing control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool ShouldAutoValidate
    {
        get
        {
            return GetAutoValidateForControl(this) != AutoValidate.Disable;
        }
    }

    // This method is called in PerformContainerValidation to check if this control supports containerValidation.
    // TabControl overrides this method to return true.
    internal virtual bool ShouldPerformContainerValidation()
    {
        return GetStyle(ControlStyles.ContainerControl);
    }

    /// <summary>
    ///  Returns true if the backColor should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeBackColor()
    {
        Color backColor = Properties.GetColor(s_backColorProperty);
        return !backColor.IsEmpty;
    }

    /// <summary>
    ///  Returns true if the cursor should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeCursor()
    {
        return Properties.ContainsObjectThatIsNotNull(s_cursorProperty);
    }

    /// <summary>
    ///  Returns true if the enabled property should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeEnabled()
    {
        return (!GetState(States.Enabled));
    }

    /// <summary>
    ///  Returns true if the foreColor should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeForeColor()
    {
        Color foreColor = Properties.GetColor(s_foreColorProperty);
        return !foreColor.IsEmpty;
    }

    /// <summary>
    ///  Returns true if the font should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeFont()
    {
        return Properties.ContainsObjectThatIsNotNull(s_fontProperty);
    }

    /// <summary>
    ///  Returns true if the RightToLeft should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeRightToLeft()
    {
        int rtl = Properties.GetInteger(s_rightToLeftProperty, out bool found);
        return (found && rtl != (int)RightToLeft.Inherit);
    }

    /// <summary>
    ///  Returns true if the visible property should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeVisible() => !DesiredVisibility;

    // Helper function - translates text alignment for Rtl controls
    // Read TextAlign as Left == Near, Right == Far
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align)
    {
        return RtlTranslateHorizontal(align);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align)
    {
        return RtlTranslateLeftRight(align);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected ContentAlignment RtlTranslateAlignment(ContentAlignment align)
    {
        return RtlTranslateContent(align);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align)
    {
        if (RightToLeft == RightToLeft.Yes)
        {
            if (align == HorizontalAlignment.Left)
            {
                return HorizontalAlignment.Right;
            }
            else if (align == HorizontalAlignment.Right)
            {
                return HorizontalAlignment.Left;
            }
        }

        return align;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align)
    {
        if (RightToLeft == RightToLeft.Yes)
        {
            if (align == LeftRightAlignment.Left)
            {
                return LeftRightAlignment.Right;
            }
            else if (align == LeftRightAlignment.Right)
            {
                return LeftRightAlignment.Left;
            }
        }

        return align;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal ContentAlignment RtlTranslateContent(ContentAlignment align)
    {
        if (RightToLeft == RightToLeft.Yes)
        {
            if ((align & WindowsFormsUtils.AnyTopAlign) != 0)
            {
                switch (align)
                {
                    case ContentAlignment.TopLeft:
                        return ContentAlignment.TopRight;
                    case ContentAlignment.TopRight:
                        return ContentAlignment.TopLeft;
                }
            }

            if ((align & WindowsFormsUtils.AnyMiddleAlign) != 0)
            {
                switch (align)
                {
                    case ContentAlignment.MiddleLeft:
                        return ContentAlignment.MiddleRight;
                    case ContentAlignment.MiddleRight:
                        return ContentAlignment.MiddleLeft;
                }
            }

            if ((align & WindowsFormsUtils.AnyBottomAlign) != 0)
            {
                switch (align)
                {
                    case ContentAlignment.BottomLeft:
                        return ContentAlignment.BottomRight;
                    case ContentAlignment.BottomRight:
                        return ContentAlignment.BottomLeft;
                }
            }
        }

        return align;
    }

    private void SetWindowFont() => PInvoke.SendMessage(this, PInvoke.WM_SETFONT, (WPARAM)FontHandle, (LPARAM)(BOOL)false);

    private void SetWindowStyle(int flag, bool value)
    {
        int styleFlags = (int)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE, value ? styleFlags | flag : styleFlags & ~flag);
    }

    /// <summary>
    ///  Makes the control display by setting the visible property to true
    /// </summary>
    public void Show()
    {
        Visible = true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool ShouldSerializeMargin()
    {
        return !Margin.Equals(DefaultMargin);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeMaximumSize()
    {
        return MaximumSize != DefaultMaximumSize;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeMinimumSize()
    {
        return MinimumSize != DefaultMinimumSize;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool ShouldSerializePadding()
    {
        return !Padding.Equals(DefaultPadding);
    }

    /// <summary>
    ///  Determines if the <see cref="Size"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeSize()
    {
        // In Whidbey the ControlDesigner class will always serialize size as it replaces the Size
        // property descriptor with its own.  This is here for compat.
        Size s = DefaultSize;
        return _width != s.Width || _height != s.Height;
    }

    /// <summary>
    ///  Determines if the <see cref="Text"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeText()
    {
        return Text.Length != 0;
    }

    /// <summary>
    ///  Suspends the layout logic for the control.
    /// </summary>
    public void SuspendLayout()
    {
        LayoutSuspendCount++;
        if (LayoutSuspendCount == 1)
        {
            OnLayoutSuspended();
        }

        Debug.Assert(LayoutSuspendCount > 0, "SuspendLayout: layoutSuspendCount overflowed.");
    }

    /// <summary>
    ///  Retrieve Font from property bag. This is the Font that was explicitly set on control by the application.
    /// </summary>
    private protected bool TryGetExplicitlySetFont([NotNullWhen(true)] out Font? font)
    {
        font = (Font?)Properties.GetObject(s_fontProperty);
        return font is not null;
    }

    /// <summary>
    ///  Sets the scaled font value with the option to control whether <see cref="OnFontChanged(EventArgs)"/> event is raised.
    /// </summary>
    /// <param name="scaledFont">The scaled <see cref="Font"/> value to be set.</param>
    /// <param name="raiseOnFontChangedEvent">Indicates whether to raise <see cref="OnFontChanged(EventArgs)"/> event.</param>
    private protected void SetScaledFont(Font scaledFont, bool raiseOnFontChangedEvent = true)
    {
        // Store new scaled value
        Properties.SetObject(s_fontProperty, scaledFont);

        // Dispose old FontHandle.
        DisposeFontHandle();

        if (Properties.ContainsInteger(s_fontHeightProperty))
        {
            Properties.SetInteger(s_fontHeightProperty, scaledFont.Height);
        }

        if (!raiseOnFontChangedEvent)
        {
            return;
        }

        // Font is an ambient property.  We need to layout our parent because Font may
        // change our size.  We need to layout ourselves because our children may change
        // size by inheriting the new value.
        using (new LayoutTransaction(ParentInternal, this, PropertyNames.Font))
        {
            OnFontChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Stops listening for the mouse leave event.
    /// </summary>
    private void UnhookMouseEvent()
    {
        SetState(States.TrackingMouseEvent, false);
    }

    /// <summary>
    ///  Forces the control to paint any currently invalid areas.
    /// </summary>
    public void Update()
    {
        if (IsHandleCreated)
        {
            PInvoke.UpdateWindow(this);
        }
    }

    /// <summary>
    ///  Updates the bounds of the control based on the handle the control is bound to.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal void UpdateBounds()
    {
        RECT rect = default;
        int clientWidth = 0;
        int clientHeight = 0;

        if (IsHandleCreated)
        {
            PInvokeCore.GetClientRect(this, out rect);
            clientWidth = rect.right;
            clientHeight = rect.bottom;
            PInvoke.GetWindowRect(this, out rect);
            if (!GetTopLevel())
            {
                PInvoke.MapWindowPoints(HWND.Null, PInvoke.GetParent(this), ref rect);
            }
        }

        UpdateBounds(
            rect.left,
            rect.top,
            rect.Width,
            rect.Height,
            clientWidth,
            clientHeight);
    }

    /// <summary>
    ///  Updates the bounds of the control based on the bounds passed in.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void UpdateBounds(int x, int y, int width, int height)
    {
        // reverse-engineer the AdjustWindowRectEx call to figure out the appropriate clientWidth and clientHeight
        RECT rect = default;
        CreateParams cp = CreateParams;

        AdjustWindowRectExForControlDpi(ref rect, (WINDOW_STYLE)cp.Style, false, (WINDOW_EX_STYLE)cp.ExStyle);
        int clientWidth = width - rect.Width;
        int clientHeight = height - rect.Height;
        UpdateBounds(x, y, width, height, clientWidth, clientHeight);
    }

    /// <summary>
    ///  Updates the bounds of the control based on the bounds passed in.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight)
    {
        bool newLocation = _x != x || _y != y;
        bool newSize = Width != width || Height != height || _clientWidth != clientWidth || _clientHeight != clientHeight;

        _x = x;
        _y = y;
        _width = width;
        _height = height;
        _clientWidth = clientWidth;
        _clientHeight = clientHeight;

        if (newLocation)
        {
            OnLocationChanged(EventArgs.Empty);
        }

        if (newSize)
        {
            OnSizeChanged(EventArgs.Empty);
            OnClientSizeChanged(EventArgs.Empty);

            // Clear PreferredSize cache for this control
            CommonProperties.xClearPreferredSizeCache(this);
            LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
        }
    }

    /// <summary>
    ///  Updates the binding manager bindings when the binding property changes.
    ///  We have the code here, rather than in PropertyChanged, so we don't pull
    ///  in the data assembly if it's not used.
    /// </summary>
    private void UpdateBindings()
    {
        for (int i = 0; i < DataBindings.Count; i++)
        {
            BindingContext.UpdateBinding(BindingContext, DataBindings[i]);
        }
    }

    /// <summary>
    ///  Updates the child control's position in the control array to correctly reflect its index.
    /// </summary>
    private void UpdateChildControlIndex(Control control)
    {
        // Don't reorder the child control array for tab controls. Implemented as a special case
        // in order to keep the method private.
        //
        // Also short-circuit when the Control class is instantiated directly. This is to provide
        // consistency with the behavior prior to bug fix https://github.com/dotnet/winforms/issues/7837
        if (this is TabControl || GetType() == typeof(Control))
        {
            return;
        }

        int newIndex = 0;
        int currentIndex = Controls.GetChildIndex(control);
        HWND hWnd = control.InternalHandle;
        while (!(hWnd = PInvoke.GetWindow(hWnd, GET_WINDOW_CMD.GW_HWNDPREV)).IsNull)
        {
            Control? previousControl = FromHandle(hWnd);
            if (previousControl is not null)
            {
                newIndex = Controls.GetChildIndex(previousControl, throwException: false) + 1;
                break;
            }
        }

        if (newIndex > currentIndex)
        {
            newIndex--;
        }

        if (newIndex != currentIndex)
        {
            Controls.SetChildIndex(control, newIndex);
        }
    }

    private void UpdateReflectParent()
    {
        // WM_REFLECT messages (e.g. WM_NOTIFY, WM_DRAWITEM, etc) will always be sent to the original parent HWND. As
        // such, we need to track our parent HWND to see if it is changed so that we can recreate our own handle.
        //
        // Scenario is when you've got a control in one parent, you move it to another, then destroy the first parent.  It'll stop
        // getting any reflected messages because Windows will send them to the original parent.
        //
        // See:
        //
        // https://learn.microsoft.com/cpp/mfc/tn061-on-notify-and-wm-notify-messages
        // https://learn.microsoft.com/cpp/mfc/tn062-message-reflection-for-windows-controls?view=msvc-170
        // https://learn.microsoft.com/windows/win32/controls/wm-notify
        // https://learn.microsoft.com/windows/win32/controls/wm-drawitem

        if (!Disposing && IsHandleCreated)
        {
            HWND parentHandle = PInvoke.GetParent(this);
            if (!parentHandle.IsNull)
            {
                ReflectParent = FromHandle(parentHandle);
                return;
            }
        }

        ReflectParent = null;
    }

    /// <summary>
    ///  Updates this control in it's parent's zorder.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void UpdateZOrder()
    {
        _parent?.UpdateChildZOrder(this);
    }

    /// <summary>
    ///  Syncs the ZOrder of child control to the index we want it to be.
    /// </summary>
    private void UpdateChildZOrder(Control control)
    {
        if (!IsHandleCreated || !control.IsHandleCreated || control._parent != this)
        {
            return;
        }

        HWND previous = HWND.HWND_TOP;
        for (int i = Controls.GetChildIndex(control); --i >= 0;)
        {
            Control child = Controls[i];
            if (child.IsHandleCreated && child._parent == this)
            {
                previous = child.HWND;
                break;
            }
        }

        if (PInvoke.GetWindow(control, GET_WINDOW_CMD.GW_HWNDPREV) != previous)
        {
            _state |= States.NoZOrder;
            try
            {
                PInvoke.SetWindowPos(
                    control,
                    previous,
                    0, 0, 0, 0,
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
            }
            finally
            {
                _state &= ~States.NoZOrder;
            }
        }
    }

    /// <summary>
    ///  Updates the rootReference in the bound window.
    ///  (Used to prevent visible top-level controls from being garbage collected)
    /// </summary>
    private void UpdateRoot()
    {
        _window.LockReference(GetTopLevel() && Visible);
    }

    /// <summary>
    ///  Forces styles to be reapplied to the handle. This function will call
    ///  CreateParams to get the styles to apply.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected void UpdateStyles()
    {
        UpdateStylesCore();

        OnStyleChanged(EventArgs.Empty);
    }

    internal virtual void UpdateStylesCore()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        CreateParams cp = CreateParams;
        WINDOW_STYLE currentStyle = WindowStyle;
        WINDOW_EX_STYLE currentExtendedStyle = ExtendedWindowStyle;

        // Resolve the Form's lazy visibility.
        if ((_state & States.Visible) != 0)
        {
            cp.Style |= (int)WINDOW_STYLE.WS_VISIBLE;
        }

        if (currentStyle != (WINDOW_STYLE)cp.Style)
        {
            WindowStyle = (WINDOW_STYLE)cp.Style;
        }

        if (currentExtendedStyle != (WINDOW_EX_STYLE)cp.ExStyle)
        {
            ExtendedWindowStyle = (WINDOW_EX_STYLE)cp.ExStyle;
            SetState(States.Mirrored, ((WINDOW_EX_STYLE)cp.ExStyle).HasFlag(WINDOW_EX_STYLE.WS_EX_LAYOUTRTL));
        }

        PInvoke.SetWindowPos(
            this,
            HWND.HWND_TOP,
            0, 0, 0, 0,
            SET_WINDOW_POS_FLAGS.SWP_DRAWFRAME
                | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                | SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                | SET_WINDOW_POS_FLAGS.SWP_NOSIZE
                | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);

        Invalidate(true);
    }

    // Give a chance for derived controls to do what they want, just before we resize.
    internal virtual void OnBoundsUpdate(int x, int y, int width, int height)
    {
    }

    // These Window* methods allow us to keep access to the "window"
    // property private, which is important for restricting access to the
    // handle.
    internal void WindowAssignHandle(HWND handle, bool value)
    {
        _window.AssignHandle(handle, value);
    }

    internal void WindowReleaseHandle()
    {
        _window.ReleaseHandle();
    }

    private void WmClose(ref Message m)
    {
        if (ParentInternal is not null)
        {
            HWND parentHandle = HWND;
            HWND lastParentHandle = parentHandle;

            while (!parentHandle.IsNull)
            {
                lastParentHandle = parentHandle;
                parentHandle = PInvoke.GetParent(parentHandle);

                if (((WINDOW_STYLE)PInvoke.GetWindowLong(lastParentHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE))
                    .HasFlag(WINDOW_STYLE.WS_CHILD))
                {
                    break;
                }
            }

            if (!lastParentHandle.IsNull)
            {
                PInvoke.PostMessage(lastParentHandle, PInvoke.WM_CLOSE);
            }
        }

        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_CAPTURECHANGED message
    /// </summary>
    private void WmCaptureChanged(ref Message m)
    {
        OnMouseCaptureChanged(EventArgs.Empty);
        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_COMMAND message
    /// </summary>
    private void WmCommand(ref Message m)
    {
        if (m.LParamInternal == 0)
        {
            if (Command.DispatchID(m.WParamInternal.LOWORD))
            {
                return;
            }
        }
        else
        {
            if (ReflectMessage(m.LParamInternal, ref m))
            {
                return;
            }
        }

        DefWndProc(ref m);
    }

    // Overridable so nested controls can provide a different source control.
    internal virtual void WmContextMenu(ref Message m)
    {
        WmContextMenu(ref m, this);
    }

    /// <summary>
    ///  Handles the WM_CONTEXTMENU message
    /// </summary>
    internal void WmContextMenu(ref Message m, Control sourceControl)
    {
        var contextMenuStrip = (ContextMenuStrip?)Properties.GetObject(s_contextMenuStripProperty);
        if (contextMenuStrip is not null)
        {
            int x = PARAM.SignedLOWORD(m.LParamInternal);
            int y = PARAM.SignedHIWORD(m.LParamInternal);
            Point client;
            bool keyboardActivated = false;

            // lparam will be exactly -1 when the user invokes the context menu with the keyboard.
            if (m.LParamInternal == -1)
            {
                keyboardActivated = true;
                client = new Point(Width / 2, Height / 2);
            }
            else
            {
                client = PointToClient(new Point(x, y));
            }

            if (ClientRectangle.Contains(client))
            {
                contextMenuStrip.ShowInternal(sourceControl, client, keyboardActivated);
            }
            else
            {
                DefWndProc(ref m);
            }
        }
        else
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_CTLCOLOR message
    /// </summary>
    private void WmCtlColorControl(ref Message m)
    {
        // We could simply reflect the message, but it's faster to handle it here if possible.
        Control? control = FromHandle(m.LParamInternal);
        if (control is not null)
        {
            m.ResultInternal = (LRESULT)(nint)control.InitializeDCForWmCtlColor((HDC)(nint)m.WParamInternal, m.MsgInternal);
            if (m.ResultInternal != 0)
            {
                return;
            }
        }

        DefWndProc(ref m);
    }

    private void WmDisplayChange(ref Message m)
    {
        BufferedGraphicsManager.Current.Invalidate();

        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_ERASEBKGND message
    /// </summary>
    private void WmEraseBkgnd(ref Message m)
    {
        if (GetStyle(ControlStyles.UserPaint))
        {
            // When possible, it's best to do all painting directly from WM_PAINT.
            // OptimizedDoubleBuffer is the "same" as turning on AllPaintingInWMPaint
            if (!GetStyle(ControlStyles.AllPaintingInWmPaint))
            {
                HDC dc = (HDC)(nint)m.WParamInternal;
                if (dc.IsNull)
                {
                    // This happens under extreme stress conditions
                    m.ResultInternal = (LRESULT)0;
                    return;
                }

                PInvokeCore.GetClientRect(this, out RECT rc);
                using PaintEventArgs pevent = new(dc, rc);
                PaintWithErrorHandling(pevent, PaintLayerBackground);
            }

            m.ResultInternal = (LRESULT)1;
        }
        else
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_GETCONTROLNAME message. Returns the name of the control.
    /// </summary>
    private void WmGetControlName(ref Message m)
    {
        string? name;

        if (Site is not null)
        {
            name = Site.Name;
        }
        else
        {
            name = Name;
        }

        name ??= string.Empty;

        MarshalStringToMessage(name, ref m);
    }

    /// <summary>
    ///  Handles the WM_GETCONTROLTYPE message. Returns the name of the control.
    /// </summary>
    private void WmGetControlType(ref Message m)
    {
        string type = GetType().AssemblyQualifiedName!;
        MarshalStringToMessage(type, ref m);
    }

    /// <summary>
    ///  Handles the WM_GETOBJECT message. Used for accessibility.
    /// </summary>
    private unsafe void WmGetObject(ref Message m)
    {
        if (m.LParamInternal == PInvoke.UiaRootObjectId && SupportsUiaProviders)
        {
            // If the requested object identifier is UiaRootObjectId,
            // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
            m.ResultInternal = PInvoke.UiaReturnRawElementProvider(
                this,
                m.WParamInternal,
                m.LParamInternal,
                AccessibilityObject);

            return;
        }

        AccessibleObject? accessibleObject = GetAccessibilityObject((int)m.LParamInternal);

        // See "How to Handle WM_GETOBJECT" in MSDN.
        if (accessibleObject is null)
        {
            // Some accessible object requested that we don't care about, so do default message processing.
            DefWndProc(ref m);

            return;
        }

        try
        {
            // Obtain the Lresult.
            m.ResultInternal = accessibleObject.GetLRESULT(m.WParamInternal);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(SR.RichControlLresult, e);
        }
    }

    /// <summary>
    ///  Handles the WM_HELP message
    /// </summary>
    private unsafe void WmHelp(ref Message m)
    {
        // If there's currently a message box open - grab the help info from it.
        HelpInfo? hpi = MessageBox.HelpInfo;
        if (hpi is not null)
        {
            switch (hpi.Option)
            {
                case HelpInfo.HelpFileOption:
                    Help.ShowHelp(this, hpi.HelpFilePath);
                    break;
                case HelpInfo.HelpKeywordOption:
                    Help.ShowHelp(this, hpi.HelpFilePath, hpi.Keyword);
                    break;
                case HelpInfo.HelpNavigatorOption:
                    Help.ShowHelp(this, hpi.HelpFilePath, hpi.Navigator);
                    break;
                case HelpInfo.HelpObjectOption:
                    Help.ShowHelp(this, hpi.HelpFilePath, hpi.Navigator, hpi.Param);
                    break;
            }
        }

        // Note: info.hItemHandle is the handle of the window that sent the help message.
        HELPINFO* info = (HELPINFO*)(nint)m.LParamInternal;
        HelpEventArgs hevent = new(info->MousePos);
        OnHelpRequested(hevent);
        if (!hevent.Handled)
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_CREATE message
    /// </summary>
    private void WmCreate(ref Message m)
    {
        DefWndProc(ref m);

        _parent?.UpdateChildZOrder(this);

        UpdateBounds();

        // Let any interested sites know that we've now created a handle
        OnHandleCreated(EventArgs.Empty);

        // this code is important -- it is critical that we stash away
        // the value of the text for controls such as edit, button,
        // label, etc. Without this processing, any time you change a
        // property that forces handle recreation, you lose your text!
        // See the below code in wmDestroy
        if (!GetStyle(ControlStyles.CacheText))
        {
            _text = null;
        }
    }

    /// <summary>
    ///  Handles the WM_DESTROY message
    /// </summary>
    private void WmDestroy(ref Message m)
    {
        // Let any interested sites know that we're destroying our handle

        if (!RecreatingHandle && !Disposing && !IsDisposed && GetState(States.TrackingMouseEvent))
        {
            // Raise the MouseLeave event for the control below the mouse
            // when a modal dialog is discarded.
            OnMouseLeave(EventArgs.Empty);
            UnhookMouseEvent();
        }

        if (SupportsUiaProviders)
        {
            ReleaseUiaProvider(HWNDInternal);
        }

        OnHandleDestroyed(EventArgs.Empty);

        if (!Disposing)
        {
            // If we are not recreating the handle, set our created state
            // back to false so we can be rebuilt if we need to be.
            if (!RecreatingHandle)
            {
                SetState(States.Created, false);
            }
        }
        else
        {
            SetState(States.Visible, false);
        }

        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and
    ///  WM_SYSKEYUP messages.
    /// </summary>
    private void WmKeyChar(ref Message m)
    {
        if (ProcessKeyMessage(ref m))
        {
            return;
        }

        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_KILLFOCUS message
    /// </summary>
    private void WmKillFocus(ref Message m)
    {
        WmImeKillFocus();
        DefWndProc(ref m);
        InvokeLostFocus(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the WM_MOUSEDOWN message
    /// </summary>
    private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
    {
        // If this is a "real" mouse event (not just WM_LBUTTONDOWN, etc) then we need to see if something happens
        // during processing of user code that changed the state of the buttons (i.e. bringing up a dialog) to keep
        // the control in a consistent state.
        MouseButtons realState = MouseButtons;
        SetState(States.MousePressed, true);

        // If the UserMouse style is set, the control does its own processing of mouse messages.
        if (!GetStyle(ControlStyles.UserMouse))
        {
            DefWndProc(ref m);

            // We might have re-entered the message loop and processed a WM_CLOSE message.
            if (IsDisposed)
            {
                return;
            }
        }
        else
        {
            // DefWndProc would normally set the focus to this control, but
            // since we're skipping DefWndProc, we need to do it ourselves.
            if (button == MouseButtons.Left && GetStyle(ControlStyles.Selectable))
            {
                Focus();
            }
        }

        if (realState != MouseButtons)
        {
            return;
        }

        if (!GetExtendedState(ExtendedStates.MaintainsOwnCaptureMode))
        {
            // Capture is set usually in MouseDown (ToolStrip main exception)
            Capture = true;
        }

        if (realState != MouseButtons)
        {
            return;
        }

        // Control should be enabled when this method is entered, but may have become
        // disabled during its lifetime (e.g. through a Click or Focus listener).
        if (Enabled)
        {
            OnMouseDown(new MouseEventArgs(button, clicks, PARAM.ToPoint(m.LParamInternal)));
        }
    }

    /// <summary>
    ///  Handles the WM_MOUSEENTER message
    /// </summary>
    private void WmMouseEnter(ref Message m)
    {
        DefWndProc(ref m);
        KeyboardToolTipStateMachine.Instance.NotifyAboutMouseEnter(this);
        OnMouseEnter(EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the WM_MOUSELEAVE message
    /// </summary>
    private void WmMouseLeave(ref Message m)
    {
        DefWndProc(ref m);
        OnMouseLeave(EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the WM_DPICHANGED_BEFOREPARENT message. This message is not sent to top level windows.
    /// </summary>
    private void WmDpiChangedBeforeParent(ref Message m)
    {
        DefWndProc(ref m);

        _oldDeviceDpi = _deviceDpi;

        // In order to support tests, will be querying Dpi from the message first.
        int newDeviceDpi = (short)m.WParamInternal.LOWORD;

        // On certain OS versions, for non-test scenarios, WParam may be empty.
        if (newDeviceDpi == 0)
        {
            newDeviceDpi = (int)PInvoke.GetDpiForWindow(this);
        }

        if (_oldDeviceDpi == newDeviceDpi)
        {
            OnDpiChangedBeforeParent(EventArgs.Empty);
            return;
        }

        Font localFont = GetCurrentFontAndDpi(out int fontDpi);
        _deviceDpi = newDeviceDpi;

        if (fontDpi == _deviceDpi)
        {
            OnDpiChangedBeforeParent(EventArgs.Empty);
            return;
        }

        // If it is a container control that inherit Font and is scaled by parent, we simply scale Font
        // and wait for OnFontChangedEvent caused by its parent. Otherwise, we scale Font and trigger
        // 'OnFontChanged' event explicitly. ex: winforms designer in VS.
        ContainerControl? container = this as ContainerControl;
        bool isLocalFontSet = IsFontSet();

        ScaledControlFont = GetScaledFont(localFont, _deviceDpi, fontDpi);

        if (isLocalFontSet || container is null || !IsScaledByParent(this))
        {
            if (isLocalFontSet)
            {
                // Container controls with the font set explicitly have their fonts scaled according to the current dpi (i.e. set to ScaledFont).
                // For container control with  AutoscaleMode=Inherit we'd like to avoid raising OnFontChanged event that would trigger
                // PerformAutoscale with the parent's AutoscaleFactor. This is because the parent control is yet to receive DpiChanged event
                // and, thus, neither scaled nor updated its AutoscaleFactor value.
                // Mark the containers as required scaling, so they are scaled when their parents update AutoscaleFactor value and ready to scale.
                bool raiseOnFontChangedEvent = container is null || container.AutoScaleMode != AutoScaleMode.Inherit;
                SetScaledFont(ScaledControlFont, raiseOnFontChangedEvent);
            }

            // Mark the container as needing to rescale when its parent is scaled.
            // This flag is reset when scaling is done on Container in "OnParentFontChanged".
            if (container is not null)
            {
                container.IsDpiChangeScalingRequired = true;
            }

            RescaleConstantsForDpi(_oldDeviceDpi, _deviceDpi);
        }

        OnDpiChangedBeforeParent(EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the WM_DPICHANGED_AFTERPARENT message
    /// </summary>
    private void WmDpiChangedAfterParent(ref Message m)
    {
        DefWndProc(ref m);

        OnDpiChangedAfterParent(EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the "WM_MOUSEHOVER" message... until we get actual OS support
    ///  for this, it is implemented as a custom message.
    /// </summary>
    private void WmMouseHover(ref Message m)
    {
        DefWndProc(ref m);
        OnMouseHover(EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the WM_MOUSEMOVE message.
    /// </summary>
    private void WmMouseMove(ref Message m)
    {
        // If the UserMouse style is set, the control does its own processing of mouse messages.
        if (!GetStyle(ControlStyles.UserMouse))
        {
            DefWndProc(ref m);
        }

        OnMouseMove(new MouseEventArgs(MouseButtons, 0, PARAM.ToPoint(m.LParamInternal)));
    }

    /// <summary>
    ///  Handles the WM_MOUSEUP message.
    /// </summary>
    private void WmMouseUp(ref Message m, MouseButtons button, int clicks)
    {
        try
        {
            Point location = PARAM.ToPoint(m.LParamInternal);
            Point screenLocation = PointToScreen(location);

            // If the UserMouse style is set, the control does its own processing of mouse messages.
            if (!GetStyle(ControlStyles.UserMouse))
            {
                DefWndProc(ref m);
            }
            else
            {
                // DefWndProc would normally trigger a context menu here (for a right button click), but since
                // we're skipping DefWndProc we have to do it ourselves.
                if (button == MouseButtons.Right)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_CONTEXTMENU, (WPARAM)HWND, (LPARAM)screenLocation);
                }
            }

            bool fireClick = _controlStyle.HasFlag(ControlStyles.StandardClick)
                && GetState(States.MousePressed)
                && !IsDisposed
                && PInvoke.WindowFromPoint(screenLocation) == HWND;

            if (fireClick && !ValidationCancelled)
            {
                if (!GetState(States.DoubleClickFired))
                {
                    OnClick(new MouseEventArgs(button, clicks, location));
                    OnMouseClick(new MouseEventArgs(button, clicks, location));
                }
                else
                {
                    OnDoubleClick(new MouseEventArgs(button, 2, location));
                    OnMouseDoubleClick(new MouseEventArgs(button, 2, location));
                }
            }

            OnMouseUp(new MouseEventArgs(button, clicks, location));
        }
        finally
        {
            // Always reset the States.DoubleClickFired in UP. Since we get UP - DOWN - DBLCLK - UP sequence
            // The flag is set in L_BUTTONDBLCLK in the controls WndProc().
            SetState(States.DoubleClickFired, false);
            SetState(States.MousePressed, false);
            SetState(States.ValidationCancelled, false);

            // Capture is reset while exiting MouseUp.
            Capture = false;
        }
    }

    /// <summary>
    ///  Handles the WM_MOUSEWHEEL message.
    /// </summary>
    private void WmMouseWheel(ref Message m)
    {
        HandledMouseEventArgs e = new(
            MouseButtons.None,
            0,
            PointToClient(PARAM.ToPoint(m.LParamInternal)),
            (short)m.WParamInternal.HIWORD);

        OnMouseWheel(e);
        m.ResultInternal = (LRESULT)(BOOL)e.Handled;
        if (!e.Handled)
        {
            // Forwarding the message to the parent window.
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_MOVE message.  We must do this in addition to WM_WINDOWPOSCHANGED because windows may
    ///  send WM_MOVE directly.
    /// </summary>
    private void WmMove(ref Message m)
    {
        DefWndProc(ref m);
        UpdateBounds();
    }

    /// <summary>
    ///  Handles the WM_NOTIFY message.
    /// </summary>
    private unsafe void WmNotify(ref Message m)
    {
        NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
        if (!ReflectMessage(nmhdr->hwndFrom, ref m))
        {
            switch (nmhdr->code)
            {
                case PInvoke.TTN_SHOW:
                    m.ResultInternal = PInvoke.SendMessage(
                        nmhdr->hwndFrom,
                        MessageId.WM_REFLECT | m.MsgInternal,
                        m.WParamInternal,
                        m.LParamInternal);
                    return;
                case PInvoke.TTN_POP:
                    PInvoke.SendMessage(
                        nmhdr->hwndFrom,
                        MessageId.WM_REFLECT | m.MsgInternal,
                        m.WParamInternal,
                        m.LParamInternal);
                    break;
            }

            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_NOTIFYFORMAT message.
    /// </summary>
    private void WmNotifyFormat(ref Message m)
    {
        if (!ReflectMessage((nint)m.WParamInternal, ref m))
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_DRAWITEM\WM_MEASUREITEM messages for controls other than menus.
    /// </summary>
    private void WmOwnerDraw(ref Message m)
    {
        int controlId = (int)m.WParamInternal;
        HWND dialogItem = PInvoke.GetDlgItem(m.HWND, controlId);
        if (dialogItem.IsNull)
        {
            // On 64-bit platforms wParam is already 64 bit but the control ID stored in it is only 32-bit.
            // Empirically, we have observed that the 64 bit HWND is just a sign extension of the 32-bit ctrl ID
            // Since WParam is already 64-bit, we need to discard the high dword first and then re-extend the
            // 32-bit value treating it as signed.
            dialogItem = (HWND)controlId;
        }

        if (ReflectMessage(dialogItem, ref m))
        {
            return;
        }

        // Try the parameter as a WindowId. TabControl truncates the HWND value.
        HWND handle = NativeWindow.GetHandleFromWindowId((short)m.WParamInternal.LOWORD);
        if (!handle.IsNull && FromHandle(handle) is { } control)
        {
            m.ResultInternal = PInvoke.SendMessage(
                control,
                MessageId.WM_REFLECT | m.MsgInternal,
                (WPARAM)handle, m.LParamInternal);

            return;
        }

        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_PAINT messages. This should only be called for userpaint controls.
    /// </summary>
    private void WmPaint(ref Message m)
    {
        bool doubleBuffered = DoubleBuffered || (GetStyle(ControlStyles.AllPaintingInWmPaint) && DoubleBufferingEnabled);
#if DEBUG
        if (s_bufferDisabled.Enabled)
        {
            doubleBuffered = false;
        }
#endif
        Rectangle clip;
        HDC dc = (HDC)(nint)m.WParamInternal;

        bool usingBeginPaint = dc.IsNull;
        using var paintScope = usingBeginPaint ? new BeginPaintScope(HWND) : default;

        if (usingBeginPaint)
        {
            dc = paintScope!.HDC;
            clip = paintScope.PaintRectangle;
        }
        else
        {
            clip = ClientRectangle;
        }

        // Consider: Why don't check the clip condition when non-doubleBuffered?
        //           we should probably get rid of the !doubleBuffered condition.
        if (doubleBuffered && (clip.Width <= 0 || clip.Height <= 0))
        {
            return;
        }

        BufferedGraphics? bufferedGraphics = null;
        PaintEventArgs? pevent = null;

        using var paletteScope = doubleBuffered || usingBeginPaint
            ? SelectPaletteScope.HalftonePalette(dc, forceBackground: false, realizePalette: false)
            : default;

        bool paintBackground = (usingBeginPaint && GetStyle(ControlStyles.AllPaintingInWmPaint)) || doubleBuffered;

        if (doubleBuffered)
        {
            try
            {
                bufferedGraphics = BufferContext.Allocate(dc, ClientRectangle);

#if DEBUG
                if (s_bufferPinkRect.Enabled)
                {
                    Rectangle band = ClientRectangle;
                    using (BufferedGraphics bufferedGraphics2 = BufferContext.Allocate(dc, band))
                    {
                        bufferedGraphics2.Graphics.FillRectangle(Brushes.Red, band);
                        bufferedGraphics2.Render();
                    }

                    Thread.Sleep(50);
                }
#endif
            }
            catch (Exception ex) when (!ex.IsCriticalException() || ex is OutOfMemoryException)
            {
                // BufferContext.Allocate will throw out of memory exceptions when it fails to create a device
                // dependent bitmap while trying to get information about the device we are painting on.
                //
                // That is not the same as a system running out of memory and there is a very good chance that we
                // can continue to paint successfully. We cannot check whether double buffering is supported in
                // this case, and we will disable it.
                //
                // We could set a specific string when throwing the exception and check for it here to distinguish
                // between that case and real out of memory exceptions but we see no reasons justifying the
                // additional complexity.

#if DEBUG
                if (s_bufferPinkRect.Enabled)
                {
                    Debug.WriteLine("Could not create buffered graphics, will paint in the surface directly");
                }
#endif
                doubleBuffered = false; // paint directly on the window DC.
            }
        }

        if (bufferedGraphics is not null)
        {
            bufferedGraphics.Graphics.SetClip(clip);
            pevent = new PaintEventArgs(
                bufferedGraphics.Graphics,
                clip,
                // We've applied a Clip, so we need to apply it when we draw
                (paintBackground ? DrawingEventFlags.SaveState : default) | DrawingEventFlags.GraphicsStateUnclean);
        }
        else
        {
            pevent = new PaintEventArgs(
                dc,
                clip,
                paintBackground ? DrawingEventFlags.SaveState : default);
        }

        using (pevent)
        {
            if (paintBackground)
            {
                PaintWithErrorHandling(pevent, PaintLayerBackground);
                pevent.ResetGraphics();
            }

            PaintWithErrorHandling(pevent, PaintLayerForeground);

            bufferedGraphics?.Render();
        }

        bufferedGraphics?.Dispose();
    }

    /// <summary>
    ///  Handles the WM_PRINTCLIENT messages.
    /// </summary>
    private void WmPrintClient(ref Message m)
    {
        HDC hdc = (HDC)(nint)m.WParamInternal;
        if (hdc.IsNull)
        {
            return;
        }

        using PaintEventArgs e = new PrintPaintEventArgs(m, hdc, ClientRectangle);
        OnPrint(e);
    }

    private void WmQueryNewPalette(ref Message m)
    {
        using GetDcScope dc = new(HWND);

        // We don't want to unset the palette in this case so we don't do this in a using.
        var paletteScope = SelectPaletteScope.HalftonePalette(
            dc,
            forceBackground: true,
            realizePalette: true);

        Invalidate(true);
        m.ResultInternal = (LRESULT)1;
        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_SETCURSOR message
    /// </summary>
    private void WmSetCursor(ref Message m)
    {
        // Accessing through the Handle property has side effects that break this logic. You must use InternalHandle.
        if ((HWND)m.WParamInternal == InternalHandle && m.LParamInternal.LOWORD == PInvoke.HTCLIENT)
        {
            Cursor.Current = Cursor;
        }
        else
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_WINDOWPOSCHANGING message.
    /// </summary>
    private unsafe void WmWindowPosChanging(ref Message m)
    {
        // We let this fall through to defwndproc unless we are being surfaced as
        // an ActiveX control.  In that case, we must let the ActiveX side of things
        // manipulate our bounds here.
        if (IsActiveX)
        {
            WINDOWPOS* wp = (WINDOWPOS*)(nint)m.LParamInternal;

            // Only call UpdateBounds if the new bounds are different.
            bool different = false;

            if ((wp->flags & SET_WINDOW_POS_FLAGS.SWP_NOMOVE) == 0 && (wp->x != Left || wp->y != Top))
            {
                different = true;
            }

            if ((wp->flags & SET_WINDOW_POS_FLAGS.SWP_NOSIZE) == 0 && (wp->cx != Width || wp->cy != Height))
            {
                different = true;
            }

            if (different)
            {
                ActiveXUpdateBounds(ref wp->x, ref wp->y, ref wp->cx, ref wp->cy, wp->flags);
            }
        }

        DefWndProc(ref m);
    }

    /// <summary>
    ///  Handles the WM_PARENTNOTIFY message.
    /// </summary>
    private void WmParentNotify(ref Message m)
    {
        MessageId msg = (MessageId)(uint)m.WParamInternal.LOWORD;
        HWND hWnd = HWND.Null;
        switch (msg)
        {
            case PInvoke.WM_CREATE:
                hWnd = (HWND)m.LParamInternal;
                break;
            case PInvoke.WM_DESTROY:
                break;
            default:
                hWnd = PInvoke.GetDlgItem(this, m.WParamInternal.HIWORD);
                break;
        }

        if (hWnd.IsNull || !ReflectMessage(hWnd, ref m))
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Handles the WM_SETFOCUS message.
    /// </summary>
    private void WmSetFocus(ref Message m)
    {
        WmImeSetFocus();

        if (!HostedInWin32DialogManager)
        {
            IContainerControl? c = GetContainerControl();
            if (c is not null)
            {
                bool activateSucceed;

                if (c is ContainerControl knowncontainer)
                {
                    activateSucceed = knowncontainer.ActivateControl(this);
                }
                else
                {
                    // Taking focus and activating a control in response to a user gesture (WM_SETFOCUS) is OK.
                    activateSucceed = c.ActivateControl(this);
                }

                if (!activateSucceed)
                {
                    return;
                }
            }
        }

        DefWndProc(ref m);
        InvokeGotFocus(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Handles the WM_SHOWWINDOW message.
    /// </summary>
    private void WmShowWindow(ref Message m)
    {
        // We get this message for each control, even if their parent is not visible.

        DefWndProc(ref m);

        if (_state.HasFlag(States.Recreate))
        {
            return;
        }

        bool visible = m.WParamInternal != 0u;
        bool oldVisibleProperty = Visible;

        if (visible)
        {
            bool oldVisibleBit = GetState(States.Visible);
            SetState(States.Visible, true);
            bool executedOk = false;
            try
            {
                CreateControl();
                executedOk = true;
            }

            finally
            {
                if (!executedOk)
                {
                    // We do it this way instead of a try/catch because catching and rethrowing
                    // an exception loses call stack information
                    SetState(States.Visible, oldVisibleBit);
                }
            }
        }
        else
        {
            // Not visible. If Windows tells us it's visible, that's pretty unambiguous. But if it tells us it's
            // not visible, there's more than one explanation -- maybe the container control became invisible. So
            // we look at the parent and take a guess at the reason.

            // We do not want to update state if we are on the parking window.
            bool parentVisible = GetTopLevel();
            if (ParentInternal is not null)
            {
                parentVisible = ParentInternal.Visible;
            }

            if (parentVisible)
            {
                SetState(States.Visible, false);
            }
        }

        if (!GetState(States.ParentRecreating) && (oldVisibleProperty != visible))
        {
            OnVisibleChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Handles the WM_UPDATEUISTATE message
    /// </summary>
    private void WmUpdateUIState(ref Message m)
    {
        // See "How this all works" in ShowKeyboardCues

        bool keyboard = false;
        bool focus = false;

        // check the cached values in uiCuesState to see if we've ever set in the UI state
        bool keyboardInitialized = (_uiCuesState & UICuesStates.KeyboardMask) != 0;
        bool focusInitialized = (_uiCuesState & UICuesStates.FocusMask) != 0;

        if (keyboardInitialized)
        {
            keyboard = ShowKeyboardCues;
        }

        if (focusInitialized)
        {
            focus = ShowFocusCues;
        }

        DefWndProc(ref m);

        uint cmd = m.WParamInternal.LOWORD;

        // if we're initializing, don't bother updating the uiCuesState/Firing the event.

        if (cmd == PInvoke.UIS_INITIALIZE)
        {
            return;
        }

        // Set in the cached value for uiCuesStates...

        // Windows stores the opposite of what you would think, it has bit
        // flags for the "Hidden" state, the presence of this flag means its
        // hidden, the absence thereof means it's shown.
        //
        // When we're called here with a UIS_CLEAR and the hidden state is set
        // that means we want to show the accelerator.
        UICues UIcues = UICues.None;
        if ((m.WParamInternal.HIWORD & PInvoke.UISF_HIDEACCEL) != 0)
        {
            // yes, clear means show.  nice api, guys.
            bool showKeyboard = (cmd == PInvoke.UIS_CLEAR);

            if (showKeyboard != keyboard || !keyboardInitialized)
            {
                UIcues |= UICues.ChangeKeyboard;

                // Clear the old state.
                _uiCuesState &= ~UICuesStates.KeyboardMask;
                _uiCuesState |= (showKeyboard ? UICuesStates.KeyboardShow : UICuesStates.KeyboardHidden);
            }

            if (showKeyboard)
            {
                UIcues |= UICues.ShowKeyboard;
            }
        }

        // Same deal for the Focus cues as the keyboard cues.
        if ((m.WParamInternal.HIWORD & PInvoke.UISF_HIDEFOCUS) != 0)
        {
            // Yes, clear means show.
            bool showFocus = cmd == PInvoke.UIS_CLEAR;

            if (showFocus != focus || !focusInitialized)
            {
                UIcues |= UICues.ChangeFocus;

                // Clear the old state.
                _uiCuesState &= ~UICuesStates.FocusMask;
                _uiCuesState |= (showFocus ? UICuesStates.FocusShow : UICuesStates.FocusHidden);
            }

            if (showFocus)
            {
                UIcues |= UICues.ShowFocus;
            }
        }

        // Fire the UI cues state changed event.
        if ((UIcues & UICues.Changed) != 0)
        {
            OnChangeUICues(new UICuesEventArgs(UIcues));
            Invalidate(true);
        }
    }

    /// <summary>
    ///  Handles the WM_WINDOWPOSCHANGED message.
    /// </summary>
    private unsafe void WmWindowPosChanged(ref Message m)
    {
        DefWndProc(ref m);

        // Update new size / position
        UpdateBounds();
        if (IsHandleCreated
            && _parent is not null
            && PInvoke.GetParent(this) == _parent.InternalHandle
            && (_state & States.NoZOrder) == 0)
        {
            WINDOWPOS* wp = (WINDOWPOS*)(nint)m.LParamInternal;
            if ((wp->flags & SET_WINDOW_POS_FLAGS.SWP_NOZORDER) == 0)
            {
                _parent.UpdateChildControlIndex(this);
            }
        }
    }

    /// <summary>
    ///  Processes Windows messages.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   All messages are sent to the <see cref="WndProc(ref Message)"/> method after getting filtered through the
    ///   <see cref="PreProcessMessage(ref Message)"/> method.
    ///  </para>
    ///  <para>
    ///   The <see cref="WndProc(ref Message)"/> method corresponds exactly to the Windows <c>WindowProc</c>
    ///   function. For more information about processing Windows messages see the
    ///   <see href="https://go.microsoft.com/fwlink/?LinkId=181565">WindowProc function</see>.
    ///  </para>
    /// </remarks>
    /// <notesToInheritors>
    ///  Inheriting controls should call the base class's <see cref="WndProc(ref Message)"/> method to process any
    ///  messages that they do not handle.
    /// </notesToInheritors>
    protected virtual void WndProc(ref Message m)
    {
        // Inlined code from GetStyle(...) to ensure no perf hit for a method call.

        if ((_controlStyle & ControlStyles.EnableNotifyMessage) == ControlStyles.EnableNotifyMessage)
        {
            // Pass message *by value* to avoid the possibility of the OnNotifyMessage modifying the message.
            OnNotifyMessage(m);
        }

        // If you add any new messages below (or change the message handling code for any messages)
        // please make sure that you also modify AxHost.WndProc to do the right thing and intercept
        // messages which the Ocx would own before passing them onto Control.WndProc.
        switch (m.MsgInternal)
        {
            case PInvoke.WM_CAPTURECHANGED:
                WmCaptureChanged(ref m);
                break;

            case PInvoke.WM_GETOBJECT:
                WmGetObject(ref m);
                break;

            case PInvoke.WM_COMMAND:
                WmCommand(ref m);
                break;

            case PInvoke.WM_CLOSE:
                WmClose(ref m);
                break;

            case PInvoke.WM_CONTEXTMENU:
                WmContextMenu(ref m);
                break;

            case PInvoke.WM_DISPLAYCHANGE:
                WmDisplayChange(ref m);
                break;

            case PInvoke.WM_DRAWITEM:
                if (m.WParamInternal != 0u)
                {
                    WmOwnerDraw(ref m);
                }

                break;

            case PInvoke.WM_ERASEBKGND:
                WmEraseBkgnd(ref m);
                break;

            case PInvoke.WM_HELP:
                WmHelp(ref m);
                break;

            case PInvoke.WM_PAINT:
                if (GetStyle(ControlStyles.UserPaint))
                {
                    WmPaint(ref m);
                }
                else
                {
                    DefWndProc(ref m);
                }

                break;

            case PInvoke.WM_PRINTCLIENT:
                if (GetStyle(ControlStyles.UserPaint))
                {
                    WmPrintClient(ref m);
                }
                else
                {
                    DefWndProc(ref m);
                }

                break;

            case PInvoke.WM_SYSCOMMAND:
                if ((m.WParamInternal & 0xFFF0) == PInvoke.SC_KEYMENU && ToolStripManager.ProcessMenuKey(ref m))
                {
                    m.ResultInternal = (LRESULT)0;
                    return;
                }

                DefWndProc(ref m);
                break;

            case PInvoke.WM_INPUTLANGCHANGE:
                WmInputLangChange(ref m);
                break;

            case PInvoke.WM_INPUTLANGCHANGEREQUEST:
                WmInputLangChangeRequest(ref m);
                break;

            case PInvoke.WM_MEASUREITEM:
                if (m.WParamInternal != 0u)
                {
                    WmOwnerDraw(ref m);
                }

                break;

            case PInvoke.WM_SETCURSOR:
                WmSetCursor(ref m);
                break;

            case PInvoke.WM_WINDOWPOSCHANGING:
                WmWindowPosChanging(ref m);
                break;

            case PInvoke.WM_CHAR:
            case PInvoke.WM_KEYDOWN:
            case PInvoke.WM_SYSKEYDOWN:
            case PInvoke.WM_KEYUP:
            case PInvoke.WM_SYSKEYUP:
                WmKeyChar(ref m);
                break;

            case PInvoke.WM_CREATE:
                WmCreate(ref m);
                break;

            case PInvoke.WM_DESTROY:
                WmDestroy(ref m);
                break;

            case PInvoke.WM_CTLCOLOR:
            case PInvoke.WM_CTLCOLORBTN:
            case PInvoke.WM_CTLCOLORDLG:
            case PInvoke.WM_CTLCOLORMSGBOX:
            case PInvoke.WM_CTLCOLORSCROLLBAR:
            case PInvoke.WM_CTLCOLOREDIT:
            case PInvoke.WM_CTLCOLORLISTBOX:
            case PInvoke.WM_CTLCOLORSTATIC:

            // this is for the trinity guys.  The case is if you've got a windows
            // forms edit or something hosted as an AX control somewhere, there isn't anyone to reflect
            // these back.  If they went ahead and just sent them back, some controls don't like that
            // and end up recursing.  Our code handles it fine because we just pick the HWND out of the LPARAM.
            case MessageId.WM_REFLECT_CTLCOLOR:
            case MessageId.WM_REFLECT_CTLCOLORBTN:
            case MessageId.WM_REFLECT_CTLCOLORDLG:
            case MessageId.WM_REFLECT_CTLCOLORMSGBOX:
            case MessageId.WM_REFLECT_CTLCOLORSCROLLBAR:
            case MessageId.WM_REFLECT_CTLCOLOREDIT:
            case MessageId.WM_REFLECT_CTLCOLORLISTBOX:
            case MessageId.WM_REFLECT_CTLCOLORSTATIC:
                WmCtlColorControl(ref m);
                break;

            case PInvoke.WM_HSCROLL:
            case PInvoke.WM_VSCROLL:
            case PInvoke.WM_DELETEITEM:
            case PInvoke.WM_VKEYTOITEM:
            case PInvoke.WM_CHARTOITEM:
            case PInvoke.WM_COMPAREITEM:
                if (!ReflectMessage(m.LParamInternal, ref m))
                {
                    DefWndProc(ref m);
                }

                break;

            case PInvoke.WM_IME_CHAR:
                WmImeChar(ref m);
                break;

            case PInvoke.WM_IME_STARTCOMPOSITION:
                WmImeStartComposition(ref m);
                break;

            case PInvoke.WM_IME_ENDCOMPOSITION:
                WmImeEndComposition(ref m);
                break;

            case PInvoke.WM_IME_NOTIFY:
                WmImeNotify(ref m);
                break;

            case PInvoke.WM_KILLFOCUS:
                WmKillFocus(ref m);
                break;

            case PInvoke.WM_LBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Left, 2);
                if (GetStyle(ControlStyles.StandardDoubleClick))
                {
                    SetState(States.DoubleClickFired, true);
                }

                break;

            case PInvoke.WM_LBUTTONDOWN:
                WmMouseDown(ref m, MouseButtons.Left, 1);
                break;

            case PInvoke.WM_LBUTTONUP:
                WmMouseUp(ref m, MouseButtons.Left, 1);
                break;

            case PInvoke.WM_MBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Middle, 2);
                if (GetStyle(ControlStyles.StandardDoubleClick))
                {
                    SetState(States.DoubleClickFired, true);
                }

                break;

            case PInvoke.WM_MBUTTONDOWN:
                WmMouseDown(ref m, MouseButtons.Middle, 1);
                break;

            case PInvoke.WM_MBUTTONUP:
                WmMouseUp(ref m, MouseButtons.Middle, 1);
                break;

            case PInvoke.WM_XBUTTONDOWN:
                WmMouseDown(ref m, GetXButton(m.WParamInternal.HIWORD), 1);
                break;

            case PInvoke.WM_XBUTTONUP:
                WmMouseUp(ref m, GetXButton(m.WParamInternal.HIWORD), 1);
                break;

            case PInvoke.WM_XBUTTONDBLCLK:
                WmMouseDown(ref m, GetXButton(m.WParamInternal.HIWORD), 2);
                if (GetStyle(ControlStyles.StandardDoubleClick))
                {
                    SetState(States.DoubleClickFired, true);
                }

                break;

            case PInvoke.WM_MOUSELEAVE:
                WmMouseLeave(ref m);
                break;

            case PInvoke.WM_DPICHANGED_BEFOREPARENT:
                WmDpiChangedBeforeParent(ref m);
                m.ResultInternal = (LRESULT)0;
                break;

            case PInvoke.WM_DPICHANGED_AFTERPARENT:
                WmDpiChangedAfterParent(ref m);
                m.ResultInternal = (LRESULT)0;
                break;

            case PInvoke.WM_MOUSEMOVE:
                WmMouseMove(ref m);
                break;

            case PInvoke.WM_MOUSEWHEEL:
                WmMouseWheel(ref m);
                break;

            case PInvoke.WM_MOVE:
                WmMove(ref m);
                break;

            case PInvoke.WM_NOTIFY:
                WmNotify(ref m);
                break;

            case PInvoke.WM_NOTIFYFORMAT:
                WmNotifyFormat(ref m);
                break;

            case MessageId.WM_REFLECT_NOTIFYFORMAT:
                m.ResultInternal = (LRESULT)(nint)PInvoke.NFR_UNICODE;
                break;

            case PInvoke.WM_SHOWWINDOW:
                WmShowWindow(ref m);
                break;

            case PInvoke.WM_RBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Right, 2);
                if (GetStyle(ControlStyles.StandardDoubleClick))
                {
                    SetState(States.DoubleClickFired, true);
                }

                break;

            case PInvoke.WM_RBUTTONDOWN:
                WmMouseDown(ref m, MouseButtons.Right, 1);
                break;

            case PInvoke.WM_RBUTTONUP:
                WmMouseUp(ref m, MouseButtons.Right, 1);
                break;

            case PInvoke.WM_SETFOCUS:
                WmSetFocus(ref m);
                break;

            case PInvoke.WM_MOUSEHOVER:
                WmMouseHover(ref m);
                break;

            case PInvoke.WM_WINDOWPOSCHANGED:
                WmWindowPosChanged(ref m);
                break;

            case PInvoke.WM_QUERYNEWPALETTE:
                WmQueryNewPalette(ref m);
                break;

            case PInvoke.WM_UPDATEUISTATE:
                WmUpdateUIState(ref m);
                break;

            case PInvoke.WM_PARENTNOTIFY:
                WmParentNotify(ref m);
                break;

            case PInvoke.WM_SETTINGCHANGE:
                if (GetExtendedState(ExtendedStates.InterestedInUserPreferenceChanged) && GetTopLevel())
                {
                    SYSTEM_PARAMETERS_INFO_ACTION action = (SYSTEM_PARAMETERS_INFO_ACTION)(uint)m.WParamInternal;

                    // Left here for debugging purposes.
                    string? text = m.LParamInternal == 0 ? null : new((char*)m.LParamInternal);

                    if (action is SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETNONCLIENTMETRICS && m.LParamInternal == 0)
                    {
                        // Text scaling needs refreshed. This can happen when changing Accessibility->Text Size.
                        //
                        // SPI_SETNONCLIENTMETRICS is sent multiple times, once with no LParam, then twice with
                        // "WindowMetrics". Common controls listen to both SPI_SETNONCLIENTMETRICS and
                        // SPI_SETICONTITLELOGFONT. Waiting for SPI_SETICONTITLELOGFONT has some sort of timing issue
                        // where layout doesn't always update correctly.
                        //
                        // Historically we reset the font on WM_SYSCOLORCHANGE, which does come through before any
                        // of the WM_SETTINGCHANGE messages. SPI_SETNONCLIENTMETRICS seems more correct.

                        s_defaultFont = null;
                        Application.ScaleDefaultFont();
                    }
                }

                break;

            case PInvoke.WM_SYSCOLORCHANGE:
                if (GetExtendedState(ExtendedStates.InterestedInUserPreferenceChanged) && GetTopLevel())
                {
                    OnSystemColorsChanged(EventArgs.Empty);
                }

                break;

            case PInvoke.WM_EXITMENULOOP:
            case PInvoke.WM_INITMENUPOPUP:
            case PInvoke.WM_MENUSELECT:
            default:

                // If we received a thread execute message, then execute it.
                if (m.Msg == (int)s_threadCallbackMessage && m.Msg != 0)
                {
                    InvokeMarshaledCallbacks();
                    return;
                }
                else if (m.Msg == (int)WM_GETCONTROLNAME)
                {
                    WmGetControlName(ref m);
                    return;
                }
                else if (m.Msg == (int)WM_GETCONTROLTYPE)
                {
                    WmGetControlType(ref m);
                    return;
                }

                if (m.MsgInternal == RegisteredMessage.WM_MOUSEENTER)
                {
                    WmMouseEnter(ref m);
                    break;
                }

                DefWndProc(ref m);
                break;
        }

        // Keep ourselves rooted until we're done processing the current message. While unlikely, it is possible
        // that we can lose the rooting for `this` and get finalized when it is no longer referenced as a local.
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Called when an exception occurs in dispatching messages through
    ///  the main window procedure.
    /// </summary>
    private static void WndProcException(Exception e)
    {
        Application.OnThreadException(e);
    }

    ArrangedElementCollection IArrangedElement.Children
    {
        get
        {
            ControlCollection? controlsCollection = (ControlCollection?)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection is null)
            {
                return ArrangedElementCollection.Empty;
            }

            return controlsCollection;
        }
    }

    IArrangedElement? IArrangedElement.Container
    {
        get
        {
            // This is safe because the IArrangedElement interface is internal
            return ParentInternal;
        }
    }

    bool IArrangedElement.ParticipatesInLayout
    {
        get { return GetState(States.Visible); }
    }

    void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string? affectedProperty)
    {
        PerformLayout(new LayoutEventArgs(affectedElement, affectedProperty));
    }

    PropertyStore IArrangedElement.Properties
    {
        get { return Properties; }
    }

    // CAREFUL: This really calls SetBoundsCore, not SetBounds.
    void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        ISite? site = Site;
        IComponentChangeService? changeService = null;
        PropertyDescriptor? sizeProperty = null;
        PropertyDescriptor? locationProperty = null;
        bool sizeChanged = false;
        bool locationChanged = false;

        if (site is not null && site.DesignMode && site.TryGetService(out changeService))
        {
            if (!AreDesignTimeFeaturesSupported)
            {
                throw new NotSupportedException(SR.DesignTimeFeaturesNotSupported);
            }

            sizeProperty = TypeDescriptor.GetProperties(this)[PropertyNames.Size];
            locationProperty = TypeDescriptor.GetProperties(this)[PropertyNames.Location];
            Debug.Assert(sizeProperty is not null && locationProperty is not null, "Error retrieving Size/Location properties on Control.");

            try
            {
                if (sizeProperty is not null && !sizeProperty.IsReadOnly && (bounds.Width != Width || bounds.Height != Height))
                {
                    if (site is not INestedSite)
                    {
                        changeService.OnComponentChanging(this, sizeProperty);
                    }

                    sizeChanged = true;
                }

                if (locationProperty is not null && !locationProperty.IsReadOnly && (bounds.X != _x || bounds.Y != _y))
                {
                    if (site is not INestedSite)
                    {
                        changeService.OnComponentChanging(this, locationProperty);
                    }

                    locationChanged = true;
                }
            }
            catch (InvalidOperationException)
            {
                // The component change events can throw InvalidOperationException if a change is
                // currently not allowed (typically because the doc data in VS is locked).
                // When this happens, we just eat the exception and proceed with the change.
            }
        }

        SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);

        if (changeService is not null)
        {
            try
            {
                if (sizeChanged)
                {
                    changeService.OnComponentChanged(this, sizeProperty);
                }

                if (locationChanged)
                {
                    changeService.OnComponentChanged(this, locationProperty);
                }
            }
            catch (InvalidOperationException)
            {
                // The component change events can throw InvalidOperationException if a change is
                // currently not allowed (typically because the doc data in VS is locked).
                // When this happens, we just eat the exception and proceed with the change.
            }
        }
    }

    /// <summary>
    ///  Indicates whether or not the control supports UIA Providers via
    ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces
    /// </summary>
    internal virtual bool SupportsUiaProviders => false;

    ///
    ///  Explicit support of DropTarget
    ///
    void IDropTarget.OnDragEnter(DragEventArgs drgEvent) => OnDragEnter(drgEvent);

    void IDropTarget.OnDragOver(DragEventArgs drgEvent) => OnDragOver(drgEvent);

    void IDropTarget.OnDragLeave(EventArgs e) => OnDragLeave(e);

    void IDropTarget.OnDragDrop(DragEventArgs drgEvent) => OnDragDrop(drgEvent);

    ///
    ///  Explicit support of DropSource
    ///
    void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs) => OnGiveFeedback(giveFeedbackEventArgs);

    void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs) => OnQueryContinueDrag(queryContinueDragEventArgs);

    #region IKeyboardToolTip implementation

    bool IKeyboardToolTip.CanShowToolTipsNow() =>
        IsHandleCreated && Visible && (ToolStripControlHost is not IKeyboardToolTip toolTip || toolTip.CanShowToolTipsNow());

    Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => GetToolNativeScreenRectangle();

    IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles() => GetNeighboringToolsRectangles();

    bool IKeyboardToolTip.IsHoveredWithMouse() => IsHoveredWithMouse();

    bool IKeyboardToolTip.HasRtlModeEnabled() =>
        TopLevelControlInternal is { } topLevelControl && topLevelControl.RightToLeft == RightToLeft.Yes && !IsMirrored;

    bool IKeyboardToolTip.AllowsToolTip() =>
        (ToolStripControlHost is not IKeyboardToolTip toolTip || toolTip.AllowsToolTip()) && AllowsKeyboardToolTip();

    IWin32Window IKeyboardToolTip.GetOwnerWindow() => this;

    void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

    void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

    string? IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) => GetCaptionForTool(toolTip);

    bool IKeyboardToolTip.ShowsOwnToolTip() =>
        (ToolStripControlHost is not IKeyboardToolTip toolTip || toolTip.ShowsOwnToolTip()) && ShowsOwnKeyboardToolTip();

    bool IKeyboardToolTip.IsBeingTabbedTo() => AreCommonNavigationalKeysDown();

    bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => AllowsChildrenToShowToolTips();

    #endregion

    private IList<Rectangle> GetOwnNeighboringToolsRectangles()
    {
        Control? controlParent = ParentInternal;
        if (controlParent is null)
        {
            return Array.Empty<Rectangle>();
        }

        List<Rectangle> neighboringControlsRectangles = new(4);

        // Next and previous control which are accessible with Tab and Shift+Tab
        AddIfCreated(controlParent.GetNextSelectableControl(this, true, true, true, false));
        AddIfCreated(controlParent.GetNextSelectableControl(this, false, true, true, false));
        // Next and previous control which are accessible with arrow keys
        AddIfCreated(controlParent.GetNextSelectableControl(this, true, false, false, true));
        AddIfCreated(controlParent.GetNextSelectableControl(this, false, false, false, true));

        return neighboringControlsRectangles;

        void AddIfCreated(Control? control)
        {
            if (control?.IsHandleCreated ?? false)
            {
                neighboringControlsRectangles.Add(((IKeyboardToolTip)control).GetNativeScreenRectangle());
            }
        }
    }

    internal virtual bool ShowsOwnKeyboardToolTip() => true;

    internal virtual void OnKeyboardToolTipHook(ToolTip toolTip)
    {
    }

    internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip)
    {
    }

    internal virtual Rectangle GetToolNativeScreenRectangle()
    {
        PInvoke.GetWindowRect(this, out var rect);
        return rect;
    }

    internal virtual bool AllowsKeyboardToolTip()
    {
        // This internal method enables keyboard ToolTips for all controls including the foreign descendants of
        // Control unless this method is overridden in a child class belonging to this assembly. ElementHost is one
        // such control which is located in a different assembly.
        //
        // This control doesn't show a mouse ToolTip when hovered and thus should not have a keyboard ToolTip as
        // well. We are not going to fix it now since it seems unlikely that someone would set ToolTip on such
        // special container control as ElementHost.
        return true;
    }

    internal static unsafe bool AreCommonNavigationalKeysDown()
    {
        static bool IsKeyDown(Keys key, ReadOnlySpan<byte> stateArray)
            => (stateArray[(int)key] & HighOrderBitMask) != 0;

        ReadOnlySpan<byte> stateArray = stackalloc byte[256];

        fixed (byte* b = stateArray)
        {
            PInvoke.GetKeyboardState(b);
            return IsKeyDown(Keys.Tab, stateArray)
                || IsKeyDown(Keys.Up, stateArray)
                || IsKeyDown(Keys.Down, stateArray)
                || IsKeyDown(Keys.Left, stateArray)
                || IsKeyDown(Keys.Right, stateArray)
                // receiving focus from the ToolStrip
                || IsKeyDown(Keys.Menu, stateArray)
                || IsKeyDown(Keys.F10, stateArray)
                || IsKeyDown(Keys.Escape, stateArray);
        }
    }

    internal virtual ToolInfoWrapper<Control> GetToolInfoWrapper(TOOLTIP_FLAGS flags, string? caption, ToolTip tooltip)
        => new(this, flags, caption);

    private readonly WeakReference<ToolStripControlHost?> _toolStripControlHostReference = new(null);

    internal ToolStripControlHost? ToolStripControlHost
    {
        get
        {
            _toolStripControlHostReference.TryGetTarget(out ToolStripControlHost? value);
            return value;
        }
        set
        {
            _toolStripControlHostReference.SetTarget(value);
        }
    }

    HWND IHandle<HWND>.Handle => HWND;

    internal HWND HWND => (HWND)Handle;

    internal virtual bool AllowsChildrenToShowToolTips() => true;
}
