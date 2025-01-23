// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms.Layout;
using Windows.Win32.System.Ole;
using Com = Windows.Win32.System.Com;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms;

[DesignTimeVisible(false)]
[Designer($"System.Windows.Forms.Design.ToolStripItemDesigner, {AssemblyRef.SystemDesign}")]
[DefaultEvent(nameof(Click))]
[ToolboxItem(false)]
[DefaultProperty(nameof(Text))]
public abstract partial class ToolStripItem :
    BindableComponent,
    ICommandBindingTargetProvider,
    IDropTarget,
    ISupportOleDropSource,
    IArrangedElement,
    IKeyboardToolTip
{
    private Rectangle _bounds = Rectangle.Empty;
    private PropertyStore? _propertyStore;
    private ToolStripItemAlignment _alignment = ToolStripItemAlignment.Left;
    private ToolStrip? _parent;
    private ToolStrip? _owner;
    private ToolStripItemOverflow _overflow = ToolStripItemOverflow.AsNeeded;
    private ToolStripItemPlacement _placement = ToolStripItemPlacement.None;
    private ContentAlignment _imageAlign = ContentAlignment.MiddleCenter;
    private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
    private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
    private ToolStripItemImageIndexer? _imageIndexer;
    private ToolStripItemInternalLayout? _toolStripItemInternalLayout;
    private BitVector32 _state;
    private string? _toolTipText;
    private Color _imageTransparentColor = Color.Empty;
    private ToolStripItemImageScaling _imageScaling = ToolStripItemImageScaling.SizeToFit;
    private Size _cachedTextSize;

    private Padding _defaultMargin;
    private Padding _defaultStatusStripMargin;

    private ToolStripItemDisplayStyle _displayStyle = ToolStripItemDisplayStyle.ImageAndText;

    // Backing fields for the infrastructure to make ToolStripItem bindable and introduce (bindable) ICommand.
    private Input.ICommand? _command;
    private object? _commandParameter;

    private static readonly ArrangedElementCollection s_emptyChildCollection = new();

    internal static readonly object s_mouseDownEvent = new();
    internal static readonly object s_mouseEnterEvent = new();
    internal static readonly object s_mouseLeaveEvent = new();
    internal static readonly object s_mouseHoverEvent = new();
    internal static readonly object s_mouseMoveEvent = new();
    internal static readonly object s_mouseUpEvent = new();
    internal static readonly object s_clickEvent = new();
    internal static readonly object s_doubleClickEvent = new();
    internal static readonly object s_dragDropEvent = new();
    internal static readonly object s_dragEnterEvent = new();
    internal static readonly object s_dragLeaveEvent = new();
    internal static readonly object s_dragOverEvent = new();
    internal static readonly object s_displayStyleChangedEvent = new();
    internal static readonly object s_enabledChangedEvent = new();
    internal static readonly object s_internalEnabledChangedEvent = new();
    internal static readonly object s_fontChangedEvent = new();
    internal static readonly object s_foreColorChangedEvent = new();
    internal static readonly object s_backColorChangedEvent = new();
    internal static readonly object s_giveFeedbackEvent = new();
    internal static readonly object s_queryContinueDragEvent = new();
    internal static readonly object s_queryAccessibilityHelpEvent = new();
    internal static readonly object s_locationChangedEvent = new();
    internal static readonly object s_rightToLeftChangedEvent = new();
    internal static readonly object s_visibleChangedEvent = new();
    internal static readonly object s_availableChangedEvent = new();
    internal static readonly object s_ownerChangedEvent = new();
    internal static readonly object s_paintEvent = new();
    internal static readonly object s_textChangedEvent = new();
    internal static readonly object s_selectedChangedEvent = new();

    internal static readonly object s_commandChangedEvent = new();
    internal static readonly object s_commandParameterChangedEvent = new();
    internal static readonly object s_commandCanExecuteChangedEvent = new();

    // Property store keys for properties. The property store allocates most efficiently
    // in groups of four, so we try to lump properties in groups of four based on how
    // likely they are going to be used in a group.

    private static readonly int s_nameProperty = PropertyStore.CreateKey();
    private static readonly int s_textProperty = PropertyStore.CreateKey();
    private static readonly int s_backColorProperty = PropertyStore.CreateKey();
    private static readonly int s_foreColorProperty = PropertyStore.CreateKey();

    private static readonly int s_imageProperty = PropertyStore.CreateKey();
    private static readonly int s_fontProperty = PropertyStore.CreateKey();
    private static readonly int s_rightToLeftProperty = PropertyStore.CreateKey();
    private static readonly int s_tagProperty = PropertyStore.CreateKey();

    private static readonly int s_accessibilityProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleNameProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleRoleProperty = PropertyStore.CreateKey();

    private static readonly int s_accessibleDefaultActionDescriptionProperty = PropertyStore.CreateKey();
    private static readonly int s_accessibleDescriptionProperty = PropertyStore.CreateKey();
    private static readonly int s_textDirectionProperty = PropertyStore.CreateKey();
    private static readonly int s_mirroredImageProperty = PropertyStore.CreateKey();

    private static readonly int s_backgroundImageProperty = PropertyStore.CreateKey();
    private static readonly int s_backgroundImageLayoutProperty = PropertyStore.CreateKey();

    private static readonly int s_mergeActionProperty = PropertyStore.CreateKey();
    private static readonly int s_mergeIndexProperty = PropertyStore.CreateKey();

    private static readonly int s_stateAllowDrop = BitVector32.CreateMask();
    private static readonly int s_stateVisible = BitVector32.CreateMask(s_stateAllowDrop);
    private static readonly int s_stateEnabled = BitVector32.CreateMask(s_stateVisible);
    private static readonly int s_stateMouseDownAndNoDrag = BitVector32.CreateMask(s_stateEnabled);
    private static readonly int s_stateAutoSize = BitVector32.CreateMask(s_stateMouseDownAndNoDrag);
    private static readonly int s_statePressed = BitVector32.CreateMask(s_stateAutoSize);
    private static readonly int s_stateSelected = BitVector32.CreateMask(s_statePressed);
    private static readonly int s_stateConstructing = BitVector32.CreateMask(s_stateSelected);
    private static readonly int s_stateDisposed = BitVector32.CreateMask(s_stateConstructing);
    private static readonly int s_stateCurrentlyAnimatingImage = BitVector32.CreateMask(s_stateDisposed);
    private static readonly int s_stateDoubleClickEnabled = BitVector32.CreateMask(s_stateCurrentlyAnimatingImage);
    private static readonly int s_stateAutoToolTip = BitVector32.CreateMask(s_stateDoubleClickEnabled);
    private static readonly int s_stateSupportsRightClick = BitVector32.CreateMask(s_stateAutoToolTip);
    private static readonly int s_stateSupportsItemClick = BitVector32.CreateMask(s_stateSupportsRightClick);
    private static readonly int s_stateRightToLeftAutoMirrorImage = BitVector32.CreateMask(s_stateSupportsItemClick);
    private static readonly int s_stateInvalidMirroredImage = BitVector32.CreateMask(s_stateRightToLeftAutoMirrorImage);
    private static readonly int s_stateSupportsSpaceKey = BitVector32.CreateMask(s_stateInvalidMirroredImage);
    private static readonly int s_stateMouseDownAndUpMustBeInSameItem = BitVector32.CreateMask(s_stateSupportsSpaceKey);
    private static readonly int s_stateSupportsDisabledHotTracking = BitVector32.CreateMask(s_stateMouseDownAndUpMustBeInSameItem);
    private static readonly int s_stateUseAmbientMargin = BitVector32.CreateMask(s_stateSupportsDisabledHotTracking);
    private static readonly int s_stateDisposing = BitVector32.CreateMask(s_stateUseAmbientMargin);

    private long _lastClickTime;
    private int _deviceDpi = ScaleHelper.InitialSystemDpi;
    internal Font _defaultFont = ToolStripManager.DefaultFont;

    protected ToolStripItem()
    {
        ScaleConstants(ScaleHelper.InitialSystemDpi);

        _state[s_stateEnabled | s_stateAutoSize | s_stateVisible | s_stateConstructing | s_stateSupportsItemClick | s_stateInvalidMirroredImage | s_stateMouseDownAndUpMustBeInSameItem | s_stateUseAmbientMargin] = true;
        _state[s_stateAllowDrop | s_stateMouseDownAndNoDrag | s_stateSupportsRightClick | s_statePressed | s_stateSelected | s_stateDisposed | s_stateDoubleClickEnabled | s_stateRightToLeftAutoMirrorImage | s_stateSupportsSpaceKey] = false;
        SetAmbientMargin();
        Size = DefaultSize;
        DisplayStyle = DefaultDisplayStyle;
        CommonProperties.SetAutoSize(this, true);
        _state[s_stateConstructing] = false;
        AutoToolTip = DefaultAutoToolTip;
    }

    protected ToolStripItem(string? text, Image? image, EventHandler? onClick)
        : this(text, image, onClick, name: null)
    {
    }

    protected ToolStripItem(string? text, Image? image, EventHandler? onClick, string? name)
        : this()
    {
        Text = text;
        Image = image;
        if (onClick is not null)
        {
            Click += onClick;
        }

        Name = name;
    }

    /// <summary>
    ///  Indicates whether or not this control has an accessible object associated with it.
    /// </summary>
    internal bool IsAccessibilityObjectCreated => Properties.ContainsKey(s_accessibilityProperty);

    /// <summary>
    ///  The Accessibility Object for this Control
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ToolStripItemAccessibilityObjectDescr))]
    public AccessibleObject AccessibilityObject
    {
        get
        {
            if (!Properties.TryGetValue(s_accessibilityProperty, out AccessibleObject? accessibleObject))
            {
                accessibleObject = Properties.AddValue(s_accessibilityProperty, CreateAccessibilityInstance());
            }

            return accessibleObject;
        }
    }

    /// <summary>
    ///  The default action description of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ToolStripItemAccessibleDefaultActionDescr))]
    public string? AccessibleDefaultActionDescription
    {
        get => Properties.GetValueOrDefault<string>(s_accessibleDefaultActionDescriptionProperty);
        set
        {
            Properties.AddOrRemoveValue(s_accessibleDefaultActionDescriptionProperty, value);
            OnAccessibleDefaultActionDescriptionChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  The accessible description of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(null)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemAccessibleDescriptionDescr))]
    public string? AccessibleDescription
    {
        get => Properties.GetValueOrDefault<string>(s_accessibleDescriptionProperty);
        set
        {
            Properties.AddOrRemoveValue(s_accessibleDescriptionProperty, value);
            OnAccessibleDescriptionChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  The accessible name of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(null)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemAccessibleNameDescr))]
    public string? AccessibleName
    {
        get => Properties.GetValueOrDefault<string>(s_accessibleNameProperty);
        set
        {
            Properties.AddOrRemoveValue(s_accessibleNameProperty, value);
            OnAccessibleNameChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  The accessible role of the control
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(AccessibleRole.Default)]
    [SRDescription(nameof(SR.ToolStripItemAccessibleRoleDescr))]
    public AccessibleRole AccessibleRole
    {
        get => Properties.GetValueOrDefault(s_accessibleRoleProperty, AccessibleRole.Default);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            Properties.AddOrRemoveValue(s_accessibleRoleProperty, value, defaultValue: AccessibleRole.Default);
            OnAccessibleRoleChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Determines if the item aligns towards the beginning or end of the ToolStrip.
    /// </summary>
    [DefaultValue(ToolStripItemAlignment.Left)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ToolStripItemAlignmentDescr))]
    public ToolStripItemAlignment Alignment
    {
        get => _alignment;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_alignment != value)
            {
                _alignment = value;

                if (ParentInternal is not null && ParentInternal.IsHandleCreated)
                {
                    ParentInternal.PerformLayout();
                }
            }
        }
    }

    /// <summary>
    ///  Determines if this item can be dragged.
    ///  This is EXACTLY like Control.AllowDrop - setting this to true WILL call
    ///  the dropTarget handlers. The ToolStripDropTargetManager is the one that
    ///  handles the routing of DropTarget events to the ToolStripItem's IDropTarget
    ///  methods.
    /// </summary>
    [SRCategory(nameof(SR.CatDragDrop))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripItemAllowDropDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public virtual bool AllowDrop
    {
        get => _state[s_stateAllowDrop];
        set
        {
            if (value != _state[s_stateAllowDrop])
            {
                EnsureParentDropTargetRegistered();
                _state[s_stateAllowDrop] = value;
            }
        }
    }

    /// <summary>
    ///  Determines whether we set the ToolStripItem to its preferred size
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [RefreshProperties(RefreshProperties.All)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemAutoSizeDescr))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool AutoSize
    {
        get => _state[s_stateAutoSize];
        set
        {
            if (_state[s_stateAutoSize] != value)
            {
                _state[s_stateAutoSize] = value;
                CommonProperties.SetAutoSize(this, value);
                InvalidateItemLayout(PropertyNames.AutoSize);
            }
        }
    }

    /// <summary>
    ///  !!!!This property ONLY works when toolStrip.ShowItemToolTips = true!!!!
    ///  if AutoToolTip is set to true we use the Text, if false, we use ToolTipText.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripItemAutoToolTipDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    public bool AutoToolTip
    {
        get => _state[s_stateAutoToolTip];
        set => _state[s_stateAutoToolTip] = value;
    }

    /// <summary>
    ///  as opposed to Visible, which returns whether or not the item and its parent are Visible
    ///  Available returns whether or not the item will be shown. Setting Available sets Visible and Vice/Versa
    /// </summary>
    [Browsable(false)]
    [SRDescription(nameof(SR.ToolStripItemAvailableDescr))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Available
    {
        get => _state[s_stateVisible];
        set => SetVisibleCore(value);
    }

    [Browsable(false)]
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ToolStripItemOnAvailableChangedDescr))]
    public event EventHandler? AvailableChanged
    {
        add => Events.AddHandler(s_availableChangedEvent, value);
        remove => Events.RemoveHandler(s_availableChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets the image that is displayed on a <see cref="Label"/>.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemImageDescr))]
    [DefaultValue(null)]
    public virtual Image? BackgroundImage
    {
        get => Properties.GetValueOrDefault<Image>(s_backgroundImageProperty);
        set
        {
            if (Properties.AddOrRemoveValue(s_backgroundImageProperty, value) != value)
            {
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Gets or sets the <see cref="Input.ICommand"/> whose <see cref="Input.ICommand.Execute(object?)"/>
    ///  method will be called when the ToolStripItem's <see cref="Click"/> event gets invoked.
    /// </summary>
    [Bindable(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.CommandComponentCommandDescr))]
    public Input.ICommand? Command
    {
        get => _command;
        set => ICommandBindingTargetProvider.CommandSetter(this, value, ref _command);
    }

    /// <summary>
    ///  Occurs when the <see cref="Input.ICommand.CanExecute(object?)"/> status of the
    ///  <see cref="Input.ICommand"/> which is assigned to the <see cref="Command"/> property has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.CommandCanExecuteChangedEventDescr))]
    public event EventHandler? CommandCanExecuteChanged
    {
        add => Events.AddHandler(s_commandCanExecuteChangedEvent, value);
        remove => Events.RemoveHandler(s_commandCanExecuteChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the assigned <see cref="Input.ICommand"/> of the <see cref="Command"/> property has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.CommandChangedEventDescr))]
    public event EventHandler? CommandChanged
    {
        add => Events.AddHandler(s_commandChangedEvent, value);
        remove => Events.RemoveHandler(s_commandChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets the parameter that is passed to the <see cref="Input.ICommand"/>
    ///  which is assigned to the <see cref="Command"/> property.
    /// </summary>
    [Bindable(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.CommandComponentCommandParameterDescr))]
    public object? CommandParameter
    {
        get => _commandParameter;

        set
        {
            if (!Equals(_commandParameter, value))
            {
                _commandParameter = value;
                OnCommandParameterChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the value of the <see cref="CommandParameter"/> property has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.CommandParameterChangedEventDescr))]
    public event EventHandler? CommandParameterChanged
    {
        add => Events.AddHandler(s_commandParameterChangedEvent, value);
        remove => Events.RemoveHandler(s_commandParameterChangedEvent, value);
    }

    // Every ToolStripItem needs to cache its last/current Parent's DeviceDpi
    // for PerMonitorV2 scaling purposes.
    internal virtual int DeviceDpi
    {
        get => _deviceDpi;
        set => _deviceDpi = value;
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ImageLayout.Tile)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))]
    public virtual ImageLayout BackgroundImageLayout
    {
        get => Properties.GetValueOrDefault(s_backgroundImageLayoutProperty, ImageLayout.Tile);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            if (Properties.AddOrRemoveValue(s_backgroundImageLayoutProperty, value, defaultValue: ImageLayout.Tile) != value)
            {
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  The BackColor of the item
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemBackColorDescr))]
    public virtual Color BackColor
    {
        get
        {
            Color c = RawBackColor;
            if (!c.IsEmpty)
            {
                return c;
            }

            Control? parent = ParentInternal;
            if (parent is not null)
            {
                return parent.BackColor;
            }

            return Control.DefaultBackColor;
        }
        set
        {
            Color previous = BackColor;
            Properties.AddOrRemoveValue(s_backColorProperty, value, defaultValue: Color.Empty);
            if (!previous.Equals(BackColor))
            {
                OnBackColorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ToolStripItemOnBackColorChangedDescr))]
    public event EventHandler? BackColorChanged
    {
        add => Events.AddHandler(s_backColorChangedEvent, value);
        remove => Events.RemoveHandler(s_backColorChangedEvent, value);
    }

    /// <summary>
    ///  The bounds of the item
    /// </summary>
    [Browsable(false)]
    public virtual Rectangle Bounds => _bounds;

    /// <summary>
    ///  Zero-based rectangle, same concept as ClientRect
    /// </summary>
    internal Rectangle ClientBounds
    {
        get
        {
            Rectangle client = _bounds;
            client.Location = Point.Empty;
            return client;
        }
    }

    [Browsable(false)]
    public Rectangle ContentRectangle
    {
        get
        {
            Rectangle content = LayoutUtils.InflateRect(InternalLayout.ContentRectangle, Padding);
            content.Size = LayoutUtils.UnionSizes(Size.Empty, content.Size);
            return content;
        }
    }

    /// <summary>
    ///  Determines whether or not the item can be selected.
    /// </summary>
    [Browsable(false)]
    public virtual bool CanSelect => Enabled;

    /// <remarks>
    ///  <para>Usually the same as can select, but things like the control box in an MDI window are exceptions</para>
    /// </remarks>
    internal virtual bool CanKeyboardSelect => CanSelect;

    /// <summary>
    ///  Occurs when the control is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ToolStripItemOnClickDescr))]
    public event EventHandler? Click
    {
        add => Events.AddHandler(s_clickEvent, value);
        remove => Events.RemoveHandler(s_clickEvent, value);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(CommonProperties.DefaultAnchor)]
    public AnchorStyles Anchor
    {
        get
        {
            // since we don't support DefaultLayout go directly against the CommonProperties
            return CommonProperties.xGetAnchor(this);
        }
        set
        {
            if (value != Anchor)
            {
                // since we don't support DefaultLayout go directly against the CommonProperties
                CommonProperties.xSetAnchor(this, value);
                if (ParentInternal is not null)
                {
                    LayoutTransaction.DoLayout(this, ParentInternal, PropertyNames.Anchor);
                }
            }
        }
    }

    /// <summary>
    ///  This does not show up in the property grid because it only applies to flow and table layouts
    /// </summary>
    [Browsable(false)]
    [DefaultValue(CommonProperties.DefaultDock)]
    public DockStyle Dock
    {
        get
        {
            // since we don't support DefaultLayout go directly against the CommonProperties
            return CommonProperties.xGetDock(this);
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value != Dock)
            {
                // since we don't support DefaultLayout go directly against the CommonProperties
                CommonProperties.xSetDock(this, value);
                if (ParentInternal is not null)
                {
                    LayoutTransaction.DoLayout(this, ParentInternal, PropertyNames.Dock);
                }
            }
        }
    }

    /// <summary>
    ///  Default setting of auto tooltip when this object is created
    /// </summary>
    protected virtual bool DefaultAutoToolTip => false;

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected internal virtual Padding DefaultMargin
    {
        get
        {
            if (Owner is not null and StatusStrip)
            {
                return _defaultStatusStripMargin;
            }

            return _defaultMargin;
        }
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected virtual Padding DefaultPadding => Padding.Empty;

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected virtual Size DefaultSize
    {
        get => ScaleHelper.IsThreadPerMonitorV2Aware ?
                ScaleHelper.ScaleToDpi(new Size(23, 23), DeviceDpi) :
                new Size(23, 23);
    }

    protected virtual ToolStripItemDisplayStyle DefaultDisplayStyle => ToolStripItemDisplayStyle.ImageAndText;

    /// <summary>
    ///  Specifies the default behavior of these items on ToolStripDropDowns when clicked.
    /// </summary>
    protected internal virtual bool DismissWhenClicked => true;

    /// <summary>
    ///  DisplayStyle specifies whether the image and text are rendered. This is not on the base
    ///  item class because different derived things will have different enumeration needs.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemDisplayStyleDescr))]
    public virtual ToolStripItemDisplayStyle DisplayStyle
    {
        get => _displayStyle;
        set
        {
            if (_displayStyle != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _displayStyle = value;
                if (!_state[s_stateConstructing])
                {
                    InvalidateItemLayout(PropertyNames.DisplayStyle);
                    OnDisplayStyleChanged(EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///  Occurs when the display style has changed
    /// </summary>
    public event EventHandler? DisplayStyleChanged
    {
        add => Events.AddHandler(s_displayStyleChangedEvent, value);
        remove => Events.RemoveHandler(s_displayStyleChangedEvent, value);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    private static RightToLeft DefaultRightToLeft => RightToLeft.Inherit;

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

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripItemDoubleClickedEnabledDescr))]
    public bool DoubleClickEnabled
    {
        get => _state[s_stateDoubleClickEnabled];
        set => _state[s_stateDoubleClickEnabled] = value;
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ToolStripItemOnDragDropDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public event DragEventHandler? DragDrop
    {
        add => Events.AddHandler(s_dragDropEvent, value);
        remove => Events.RemoveHandler(s_dragDropEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ToolStripItemOnDragEnterDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public event DragEventHandler? DragEnter
    {
        add => Events.AddHandler(s_dragEnterEvent, value);
        remove => Events.RemoveHandler(s_dragEnterEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ToolStripItemOnDragOverDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public event DragEventHandler? DragOver
    {
        add => Events.AddHandler(s_dragOverEvent, value);
        remove => Events.RemoveHandler(s_dragOverEvent, value);
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ToolStripItemOnDragLeaveDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public event EventHandler? DragLeave
    {
        add => Events.AddHandler(s_dragLeaveEvent, value);
        remove => Events.RemoveHandler(s_dragLeaveEvent, value);
    }

    /// <summary>
    ///  Occurs when the control is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemEnabledDescr))]
    [DefaultValue(true)]
    public virtual bool Enabled
    {
        get
        {
            bool parentEnabled = true;
            if (Owner is not null)
            {
                parentEnabled = Owner.Enabled;
            }

            return _state[s_stateEnabled] && parentEnabled;
        }
        set
        {
            // flip disabled bit.
            if (_state[s_stateEnabled] != value)
            {
                _state[s_stateEnabled] = value;
                if (!_state[s_stateEnabled])
                {
                    bool wasSelected = _state[s_stateSelected];
                    // clear all the other states.
                    _state[s_stateSelected | s_statePressed] = false;

                    if (wasSelected)
                    {
                        KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                        OnSelectedChanged(EventArgs.Empty);
                    }
                }

                OnEnabledChanged(EventArgs.Empty);
                Invalidate();
            }

            OnInternalEnabledChanged(EventArgs.Empty);
        }
    }

    [SRDescription(nameof(SR.ToolStripItemEnabledChangedDescr))]
    public event EventHandler? EnabledChanged
    {
        add => Events.AddHandler(s_enabledChangedEvent, value);
        remove => Events.RemoveHandler(s_enabledChangedEvent, value);
    }

    internal event EventHandler? InternalEnabledChanged
    {
        add => Events.AddHandler(s_internalEnabledChangedEvent, value);
        remove => Events.RemoveHandler(s_internalEnabledChangedEvent, value);
    }

    private void EnsureParentDropTargetRegistered()
    {
        ParentInternal?.DropTargetManager.EnsureRegistered();
    }

    /// <summary>
    ///  Retrieves the current font for this item. This will be the font used
    ///  by default for painting and text in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemForeColorDescr))]
    public virtual Color ForeColor
    {
        get
        {
            Color foreColor = Properties.GetValueOrDefault<Color>(s_foreColorProperty);
            if (!foreColor.IsEmpty)
            {
                return foreColor;
            }

            Control? parent = ParentInternal;
            if (parent is not null)
            {
                return parent.ForeColor;
            }

            return Control.DefaultForeColor;
        }
        set
        {
            Color previous = ForeColor;
            Properties.AddOrRemoveValue(s_foreColorProperty, value);
            if (!previous.Equals(ForeColor))
            {
                OnForeColorChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ToolStripItemOnForeColorChangedDescr))]
    public event EventHandler? ForeColorChanged
    {
        add => Events.AddHandler(s_foreColorChangedEvent, value);
        remove => Events.RemoveHandler(s_foreColorChangedEvent, value);
    }

    /// <summary>
    ///  Retrieves the current font for this control. This will be the font used
    ///  by default for painting and text in the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemFontDescr))]
    [AllowNull]
    public virtual Font Font
    {
        get
        {
            if (TryGetExplicitlySetFont(out Font? font))
            {
                return font;
            }

            font = GetOwnerFont();
            if (font is not null)
            {
                return font;
            }

            return ScaleHelper.IsThreadPerMonitorV2Aware ? _defaultFont : ToolStripManager.DefaultFont;
        }
        set
        {
            if (Properties.AddOrRemoveValue(s_fontProperty, value) != value)
            {
                OnFontChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ToolStripItemOnGiveFeedbackDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public event GiveFeedbackEventHandler? GiveFeedback
    {
        add => Events.AddHandler(s_giveFeedbackEvent, value);
        remove => Events.RemoveHandler(s_giveFeedbackEvent, value);
    }

    /// <summary>
    ///  The height of this control.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Height
    {
        get => Bounds.Height;
        set
        {
            Rectangle currentBounds = Bounds;
            SetBounds(currentBounds.X, currentBounds.Y, currentBounds.Width, value);
        }
    }

    /// <summary>
    ///  ToolStripItems do not have children. For perf reasons always return a static empty collection.
    ///  Consider creating readonly collection.
    /// </summary>
    ArrangedElementCollection IArrangedElement.Children => s_emptyChildCollection;

    /// <summary>
    ///  Should not be exposed as this returns an unexposed type.
    /// </summary>
    IArrangedElement? IArrangedElement.Container => ParentInternal ?? Owner;

    Rectangle IArrangedElement.DisplayRectangle => Bounds;

    bool IArrangedElement.ParticipatesInLayout
    {
        get
        {
            // this can be different than "Visible" property as "Visible" takes into account whether or not you
            // are parented and your parent is visible.
            return _state[s_stateVisible];
        }
    }

    PropertyStore IArrangedElement.Properties => Properties;

    void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        // in this case the parent is telling us to refresh our bounds - don't
        // call PerformLayout
        SetBounds(bounds);
    }

    void IArrangedElement.PerformLayout(IArrangedElement container, string? propertyName)
    {
    }

    /// <summary>
    ///  Gets or sets the alignment of the image on the label control.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemImageAlignDescr))]
    public ContentAlignment ImageAlign
    {
        get => _imageAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_imageAlign != value)
            {
                _imageAlign = value;
                InvalidateItemLayout(PropertyNames.ImageAlign);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the image that is displayed on a <see cref="Label"/>.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemImageDescr))]
    public virtual Image? Image
    {
        get
        {
            Image? image = Properties.GetValueOrDefault<Image>(s_imageProperty);
            if (image is null && Owner?.ImageList is not null && ImageIndexer.ActualIndex >= 0)
            {
                bool disposing = _state[s_stateDisposing];
                if (!disposing && ImageIndexer.ActualIndex < Owner.ImageList.Images.Count)
                {
                    // CACHE (by design). If we fetched out of the image list every time it would dramatically hit perf.
                    image = Owner.ImageList.Images[ImageIndexer.ActualIndex];
                    _state[s_stateInvalidMirroredImage] = true;
                    Properties.AddValue(s_imageProperty, image);
                    return image;
                }

                return null;
            }

            return image;
        }
        set
        {
            if (Image == value)
            {
                return;
            }

            StopAnimate();

            if (value is Bitmap bmp && ImageTransparentColor != Color.Empty)
            {
                if (bmp.RawFormat.Guid != ImageFormat.Icon.Guid && !ImageAnimator.CanAnimate(bmp))
                {
                    bmp.MakeTransparent(ImageTransparentColor);
                }

                value = bmp;
            }

            if (value is not null)
            {
                ImageIndex = ImageList.Indexer.DefaultIndex;
            }

            Properties.AddOrRemoveValue(s_imageProperty, value);
            _state[s_stateInvalidMirroredImage] = true;

            Animate();
            InvalidateItemLayout(PropertyNames.Image);
        }
    }

    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemImageTransparentColorDescr))]
    public Color ImageTransparentColor
    {
        get => _imageTransparentColor;
        set
        {
            if (_imageTransparentColor == value)
            {
                return;
            }

            _imageTransparentColor = value;
            if (Image is Bitmap currentImage && value != Color.Empty)
            {
                if (currentImage.RawFormat.Guid != ImageFormat.Icon.Guid && !ImageAnimator.CanAnimate(currentImage))
                {
                    currentImage.MakeTransparent(_imageTransparentColor);
                }
            }

            Invalidate();
        }
    }

    /// <summary>
    ///  Returns the ToolStripItem's currently set image index
    ///  Here for compat only - this is NOT to be visible at DT.
    /// </summary>
    [SRDescription(nameof(SR.ToolStripItemImageIndexDescr))]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Browsable(false)]
    [RelatedImageList("Owner.ImageList")]
    public int ImageIndex
    {
        get
        {
            if ((Owner is not null) && ImageIndexer.Index != ImageList.Indexer.DefaultIndex
                && Owner.ImageList is not null && ImageIndexer.Index >= Owner.ImageList.Images.Count)
            {
                return Owner.ImageList.Images.Count - 1;
            }

            return ImageIndexer.Index;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);

            ImageIndexer.Index = value;
            _state[s_stateInvalidMirroredImage] = true;

            // Set the Image Property to null
            Properties.RemoveValue(s_imageProperty);

            InvalidateItemLayout(PropertyNames.ImageIndex);
        }
    }

    internal ToolStripItemImageIndexer ImageIndexer
        => _imageIndexer ??= new ToolStripItemImageIndexer(this);

    /// <summary>
    ///  Returns the ToolStripItem's currently set image index
    ///  Here for compat only - this is NOT to be visible at DT.
    /// </summary>
    [SRDescription(nameof(SR.ToolStripItemImageKeyDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [TypeConverter(typeof(ImageKeyConverter))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Editor($"System.Windows.Forms.Design.ToolStripImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Browsable(false)]
    [RelatedImageList("Owner.ImageList")]
    [AllowNull]
    public string ImageKey
    {
        get => ImageIndexer.Key;
        set
        {
            ImageIndexer.Key = value;
            _state[s_stateInvalidMirroredImage] = true;
            Properties.RemoveValue(s_imageProperty);
            InvalidateItemLayout(PropertyNames.ImageKey);
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ToolStripItemImageScaling.SizeToFit)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemImageScalingDescr))]
    public ToolStripItemImageScaling ImageScaling
    {
        get => _imageScaling;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_imageScaling != value)
            {
                _imageScaling = value;
                InvalidateItemLayout(PropertyNames.ImageScaling);
            }
        }
    }

    /// <summary>
    ///  This object helps determine where the image and text should be drawn.
    /// </summary>
    internal ToolStripItemInternalLayout InternalLayout
        => _toolStripItemInternalLayout ??= CreateInternalLayout();

    internal bool IsForeColorSet
    {
        get
        {
            Color color = Properties.GetValueOrDefault<Color>(s_foreColorProperty);
            if (!color.IsEmpty)
            {
                return true;
            }

            ToolStrip? parent = ParentInternal;
            if (parent is not null)
            {
                return parent.ShouldSerializeForeColor();
            }

            return false;
        }
    }

    /// <summary>
    ///  This is used by ToolStrip to pass on the mouseMessages for ActiveDropDown.
    /// </summary>
    internal bool IsInDesignMode => DesignMode;

    [Browsable(false)]
    public bool IsDisposed => _state[s_stateDisposed];

    [Browsable(false)]
    public bool IsOnDropDown
    {
        get
        {
            if (ParentInternal is not null)
            {
                return ParentInternal.IsDropDown;
            }
            else if (Owner is not null && Owner.IsDropDown)
            {
                return true;
            }

            return false;
        }
    }

    [Browsable(false)]
    public bool IsOnOverflow => Placement == ToolStripItemPlacement.Overflow;

    /// <summary>
    ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
    /// </summary>
    internal virtual bool IsMnemonicsListenerAxSourced => true;

    /// <summary>
    ///  Occurs when the location of the ToolStripItem has been updated -- usually by layout by its
    ///  owner of ToolStrips
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ToolStripItemOnLocationChangedDescr))]
    public event EventHandler? LocationChanged
    {
        add => Events.AddHandler(s_locationChangedEvent, value);
        remove => Events.RemoveHandler(s_locationChangedEvent, value);
    }

    /// <summary>
    ///  Specifies the external spacing between this item and any other item or the ToolStrip.
    /// </summary>
    [SRDescription(nameof(SR.ToolStripItemMarginDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public Padding Margin
    {
        get => CommonProperties.GetMargin(this);
        set
        {
            if (Margin != value)
            {
                _state[s_stateUseAmbientMargin] = false;
                CommonProperties.SetMargin(this, value);
            }
        }
    }

    /// <summary>
    ///  Specifies the merge action when merging two ToolStrip.
    /// </summary>
    [SRDescription(nameof(SR.ToolStripMergeActionDescr))]
    [DefaultValue(MergeAction.Append)]
    [SRCategory(nameof(SR.CatLayout))]
    public MergeAction MergeAction
    {
        get => Properties.GetValueOrDefault(s_mergeActionProperty, MergeAction.Append);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            Properties.AddOrRemoveValue(s_mergeActionProperty, value, defaultValue: MergeAction.Append);
        }
    }

    /// <summary>
    ///  Specifies the merge action when merging two ToolStrip.
    /// </summary>
    [SRDescription(nameof(SR.ToolStripMergeIndexDescr))]
    [DefaultValue(-1)]
    [SRCategory(nameof(SR.CatLayout))]
    public int MergeIndex
    {
        get => Properties.GetValueOrDefault(s_mergeIndexProperty, -1);
        set => Properties.AddOrRemoveValue(s_mergeIndexProperty, value, -1);
    }

    internal bool MouseDownAndUpMustBeInSameItem
    {
        get => _state[s_stateMouseDownAndUpMustBeInSameItem];
        set => _state[s_stateMouseDownAndUpMustBeInSameItem] = value;
    }

    /// <summary>
    ///  Occurs when the mouse pointer is over the control and a mouse button is
    ///  pressed.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ToolStripItemOnMouseDownDescr))]
    public event MouseEventHandler? MouseDown
    {
        add => Events.AddHandler(s_mouseDownEvent, value);
        remove => Events.RemoveHandler(s_mouseDownEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer enters the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ToolStripItemOnMouseEnterDescr))]
    public event EventHandler? MouseEnter
    {
        add => Events.AddHandler(s_mouseEnterEvent, value);
        remove => Events.RemoveHandler(s_mouseEnterEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer leaves the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ToolStripItemOnMouseLeaveDescr))]
    public event EventHandler? MouseLeave
    {
        add => Events.AddHandler(s_mouseLeaveEvent, value);
        remove => Events.RemoveHandler(s_mouseLeaveEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer hovers over the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ToolStripItemOnMouseHoverDescr))]
    public event EventHandler? MouseHover
    {
        add => Events.AddHandler(s_mouseHoverEvent, value);
        remove => Events.RemoveHandler(s_mouseHoverEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer is moved over the control.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ToolStripItemOnMouseMoveDescr))]
    public event MouseEventHandler? MouseMove
    {
        add => Events.AddHandler(s_mouseMoveEvent, value);
        remove => Events.RemoveHandler(s_mouseMoveEvent, value);
    }

    /// <summary>
    ///  Occurs when the mouse pointer is over the control and a mouse button is released.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ToolStripItemOnMouseUpDescr))]
    public event MouseEventHandler? MouseUp
    {
        add => Events.AddHandler(s_mouseUpEvent, value);
        remove => Events.RemoveHandler(s_mouseUpEvent, value);
    }

    /// <summary>
    ///  Name of this control. The designer will set this to the same
    ///  as the programmatic Id "(name)" of the control. The name can be
    ///  used as a key into the ControlCollection.
    /// </summary>
    [Browsable(false)]
    [DefaultValue(null)]
    public string? Name
    {
        get => WindowsFormsUtils.GetComponentName(this, Properties.GetValueOrDefault<string>(s_nameProperty));
        set
        {
            if (DesignMode)
            {
                return;
            }

            Properties.AddOrRemoveValue(s_nameProperty, value);
        }
    }

    /// <summary>
    ///  The owner of this ToolStripItem. The owner is essentially a backPointer to
    ///  the ToolStrip who contains this item in it's item collection. Handy for getting
    ///  to things such as the ImageList, which would be defined on the ToolStrip.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ToolStrip? Owner
    {
        get => _owner;
        set
        {
            if (_owner != value)
            {
                _owner?.Items.Remove(this);

                value?.Items.Add(this);
            }
        }
    }

    /// <summary>
    ///  Returns the "parent" item on the preceding menu which has spawned this item.
    ///  e.g. File->Open  the OwnerItem of Open is File.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ToolStripItem? OwnerItem
    {
        get
        {
            ToolStripDropDown? currentParent = null;
            if (ParentInternal is not null)
            {
                currentParent = ParentInternal as ToolStripDropDown;
            }
            else if (Owner is not null)
            {
                // parent may be null, but we may be "owned" by a collection.
                currentParent = Owner as ToolStripDropDown;
            }

            if (currentParent is not null)
            {
                return currentParent.OwnerItem;
            }

            return null;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripItemOwnerChangedDescr))]
    public event EventHandler? OwnerChanged
    {
        add => Events.AddHandler(s_ownerChangedEvent, value);
        remove => Events.RemoveHandler(s_ownerChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemOnPaintDescr))]
    public event PaintEventHandler? Paint
    {
        add => Events.AddHandler(s_paintEvent, value);
        remove => Events.RemoveHandler(s_paintEvent, value);
    }

    /// <summary>
    ///  The parent of this ToolStripItem. This can be distinct from the owner because
    ///  the item can fall onto another window (overflow). In this case the overflow
    ///  would be the parent but the original ToolStrip would be the Owner. The "parent"
    ///  ToolStrip will be firing things like paint events - where as the "owner" ToolStrip
    ///  will be containing shared data like image lists. Typically the only one who should
    ///  set the parent property is the layout manager on the ToolStrip.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected internal ToolStrip? Parent
    {
        get => ParentInternal;
        set => ParentInternal = value;
    }

    /// <summary>
    ///  Specifies whether or not the item is glued to the ToolStrip or overflow or
    ///  can float between the two.
    /// </summary>
    [DefaultValue(ToolStripItemOverflow.AsNeeded)]
    [SRDescription(nameof(SR.ToolStripItemOverflowDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public ToolStripItemOverflow Overflow
    {
        get => _overflow;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_overflow != value)
            {
                _overflow = value;
                if (Owner is not null)
                {
                    LayoutTransaction.DoLayout(Owner, Owner, "Overflow");
                }
            }
        }
    }

    /// <summary>
    ///  Specifies the internal spacing between the contents and the edges of the item
    /// </summary>
    [SRDescription(nameof(SR.ToolStripItemPaddingDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    public virtual Padding Padding
    {
        get => CommonProperties.GetPadding(this, DefaultPadding);
        set
        {
            if (Padding != value)
            {
                CommonProperties.SetPadding(this, value);
                InvalidateItemLayout(PropertyNames.Padding);
            }
        }
    }

    /// <summary>
    ///  This is explicitly a ToolStrip, because only ToolStrips know how to manage ToolStripItems
    /// </summary>
    internal ToolStrip? ParentInternal
    {
        get => _parent;
        set
        {
            if (_parent != value)
            {
                ToolStrip? oldParent = _parent;
                _parent = value;
                OnParentChanged(oldParent, value);
            }
        }
    }

    /// <summary>
    ///  Where the item actually ended up.
    /// </summary>
    [Browsable(false)]
    public ToolStripItemPlacement Placement => _placement;

    internal Size PreferredImageSize
    {
        get
        {
            if ((DisplayStyle & ToolStripItemDisplayStyle.Image) != ToolStripItemDisplayStyle.Image)
            {
                return Size.Empty;
            }

            Image? image = Properties.GetValueOrDefault<Image>(s_imageProperty);
            bool usingImageList = ((Owner is not null) && (Owner.ImageList is not null) && (ImageIndexer.ActualIndex >= 0));

            if (ImageScaling == ToolStripItemImageScaling.SizeToFit)
            {
                ToolStrip? ownerToolStrip = Owner;
                if (ownerToolStrip is not null && (image is not null || usingImageList))
                {
                    return ownerToolStrip.ImageScalingSize;
                }
            }

            return usingImageList
                ? Owner?.ImageList?.ImageSize ?? Size.Empty
                : (image is null) ? Size.Empty : image.Size;
        }
    }

    /// <summary>
    ///  Retrieves our internal property storage object. If you have a property
    ///  whose value is not always set, you should store it in here to save
    ///  space.
    /// </summary>
    internal PropertyStore Properties => _propertyStore ??= new PropertyStore();

    /// <summary>
    ///  Returns true if the state of the item is pushed
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool Pressed => CanSelect && _state[s_statePressed];

    [SRCategory(nameof(SR.CatDragDrop))]
    [SRDescription(nameof(SR.ToolStripItemOnQueryContinueDragDescr))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Browsable(false)]
    public event QueryContinueDragEventHandler? QueryContinueDrag
    {
        add => Events.AddHandler(s_queryContinueDragEvent, value);
        remove => Events.RemoveHandler(s_queryContinueDragEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolStripItemOnQueryAccessibilityHelpDescr))]
    public event QueryAccessibilityHelpEventHandler? QueryAccessibilityHelp
    {
        add => Events.AddHandler(s_queryAccessibilityHelpEvent, value);
        remove => Events.RemoveHandler(s_queryAccessibilityHelpEvent, value);
    }

    /// <summary>
    ///  Returns the value of the backColor field -- no asking the parent with its color is, etc.
    /// </summary>
    internal Color RawBackColor => Properties.GetValueOrDefault(s_backColorProperty, Color.Empty);

    /// <summary>
    ///  Returns the parent <see cref="ToolStrip"/>'s renderer
    /// </summary>
    protected internal virtual ToolStripRenderer? Renderer
    {
        get
        {
            if (Owner is not null)
            {
                return Owner.Renderer;
            }

            return ParentInternal?.Renderer;
        }
    }

    /// <summary>
    ///  This is used for international applications where the language
    ///  is written from RightToLeft. When this property is true,
    ///  control placement and text will be from right to left.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemRightToLeftDescr))]
    public virtual RightToLeft RightToLeft
    {
        get
        {
            if (!Properties.TryGetValue(s_rightToLeftProperty, out RightToLeft rightToLeft))
            {
                rightToLeft = RightToLeft.Inherit;
            }

            if (rightToLeft == RightToLeft.Inherit)
            {
                if (Owner is not null)
                {
                    rightToLeft = Owner.RightToLeft;
                }
                else if (ParentInternal is not null)
                {
                    // case for Overflow & Grip
                    rightToLeft = ParentInternal.RightToLeft;
                }
                else
                {
                    rightToLeft = DefaultRightToLeft;
                }
            }

            return rightToLeft;
        }

        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            RightToLeft oldValue = RightToLeft;

            if (Properties.ContainsKey(s_rightToLeftProperty) || value != RightToLeft.Inherit)
            {
                Properties.AddValue(s_rightToLeftProperty, value);
            }

            if (oldValue != RightToLeft)
            {
                OnRightToLeftChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Mirrors the image when RTL.Yes.
    ///  Note we do not change what is returned back from the Image property as this would cause problems with serialization.
    ///  Instead we only change what is painted - there's an internal MirroredImage property which fills in as
    ///  e.Image in the ToolStripItemImageRenderEventArgs if the item is RTL.Yes and AutoMirrorImage is turned on.
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemRightToLeftAutoMirrorImageDescr))]
    public bool RightToLeftAutoMirrorImage
    {
        get => _state[s_stateRightToLeftAutoMirrorImage];
        set
        {
            if (_state[s_stateRightToLeftAutoMirrorImage] != value)
            {
                _state[s_stateRightToLeftAutoMirrorImage] = value;
                Invalidate();
            }
        }
    }

    internal Image? MirroredImage
    {
        get
        {
            if (_state[s_stateInvalidMirroredImage])
            {
                Image? image = Image;
                if (image is null)
                {
                    return null;
                }

                Image mirroredImage = (Image)image.Clone();
                mirroredImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

                Properties.AddValue(s_mirroredImageProperty, mirroredImage);
                _state[s_stateInvalidMirroredImage] = false;
                return mirroredImage;
            }

            return Properties.GetValueOrDefault<Image>(s_mirroredImageProperty);
        }
    }

    bool? ICommandBindingTargetProvider.PreviousEnabledStatus { get; set; }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ToolStripItemOnRightToLeftChangedDescr))]
    public event EventHandler? RightToLeftChanged
    {
        add => Events.AddHandler(s_rightToLeftChangedEvent, value);
        remove => Events.RemoveHandler(s_rightToLeftChangedEvent, value);
    }

    /// <summary>
    ///  if the item is selected we return true.
    ///
    ///  FAQ: Why don't we have a Hot or MouseIsOver property?
    ///  After going through the scenarios, we've decided NOT to add a separate MouseIsOver or Hot flag to ToolStripItem.
    ///  The thing to use is 'Selected'.
    ///  Why?  While the selected thing can be different than the moused over item, the selected item is ALWAYS the
    ///  one you want to paint differently
    ///
    ///  Scenario 1:  Keyboard select an item then select a different item with the mouse.
    ///  -          Do Alt+F to expand your File menu, keyboard down several items.
    ///  -          Mouse over a different item
    ///  -          Notice how two things are never painted hot at the same time, and how the selection changes
    ///  -          from the keyboard selected item to the one selected with the mouse. In  this case the selection
    ///  -          should move with the mouse selection.
    ///  -          Notice how if you hit enter when the mouse is over it, it executes the item. That's selection.
    ///  Scenario 2: Put focus into a combo box, then mouse over a different item
    ///  -          Notice how all the other items you mouse over do not change the way they are painted, if you
    ///  -          hit enter, that goes to the ComboBox, rather than executing the current item.
    ///
    ///  At first look "MouseIsOver" or "Hot" seems to be the thing people want, but its almost never the desired
    ///  behavior. A unified selection model is simpler and seems to meet the scenarios.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool Selected
        => CanSelect && !DesignMode && (_state[s_stateSelected] ||
            (ParentInternal is not null && ParentInternal.IsSelectionSuspended &&
             ParentInternal.LastMouseDownedItem == this));

    /// <summary>
    ///  Occurs when value of the <see cref="Selected" /> property changes.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This event is raised when item is selected by mouse, keyboard or programmatically.
    ///   .NET Core 3.1 had removed MainMenu and MenuItem controls. ToolStripMenuItem.SelectedChanged event
    ///   is recommended as a replacement for MenuItem.Select event.
    ///  </para>
    /// </remarks>
    [SRDescription(nameof(SR.ToolStripItemSelectedChangedDescr))]
    public event EventHandler? SelectedChanged
    {
        add => Events.AddHandler(s_selectedChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedChangedEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="SelectedChanged" /> event. This method will be called when selected
    ///  <see cref="ToolStripItem" /> changes. Call base.OnSelectedChanged to send this event to any registered
    ///  event listeners.
    /// </summary>
    protected virtual void OnSelectedChanged(EventArgs e) => RaiseEvent(s_selectedChangedEvent, e);

    protected internal virtual bool ShowKeyboardCues
        => DesignMode || ToolStripManager.ShowMenuFocusCues;

    /// <summary>
    ///  The size of the item
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemSizeDescr))]
    public virtual Size Size
    {
        get => Bounds.Size;
        set
        {
            Rectangle currentBounds = Bounds;
            currentBounds.Size = value;
            SetBounds(currentBounds);
        }
    }

    internal bool SupportsRightClick
    {
        get => _state[s_stateSupportsRightClick];
        set => _state[s_stateSupportsRightClick] = value;
    }

    internal bool SupportsItemClick
    {
        get => _state[s_stateSupportsItemClick];
        set => _state[s_stateSupportsItemClick] = value;
    }

    internal bool SupportsSpaceKey
    {
        get => _state[s_stateSupportsSpaceKey];
        set => _state[s_stateSupportsSpaceKey] = value;
    }

    internal bool SupportsDisabledHotTracking
    {
        get => _state[s_stateSupportsDisabledHotTracking];
        set => _state[s_stateSupportsDisabledHotTracking] = value;
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ToolStripItemTagDescr))]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get => Properties.GetValueOrDefault<object>(s_tagProperty);
        set => Properties.AddOrRemoveValue(s_tagProperty, value);
    }

    /// <summary>
    ///  The text of the item
    /// </summary>
    [DefaultValue("")]
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemTextDescr))]
    public virtual string? Text
    {
        get => Properties.TryGetValueOrNull(s_textProperty, out string? value) ? value : string.Empty;
        set
        {
            if (Properties.AddOrRemoveValue(s_textProperty, value, defaultValue: string.Empty) != value)
            {
                OnTextChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the alignment of the text on the label control.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripItemTextAlignDescr))]
    public virtual ContentAlignment TextAlign
    {
        get => _textAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_textAlign != value)
            {
                _textAlign = value;
                InvalidateItemLayout(PropertyNames.TextAlign);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ToolStripItemOnTextChangedDescr))]
    public event EventHandler? TextChanged
    {
        add => Events.AddHandler(s_textChangedEvent, value);
        remove => Events.RemoveHandler(s_textChangedEvent, value);
    }

    [SRDescription(nameof(SR.ToolStripTextDirectionDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public virtual ToolStripTextDirection TextDirection
    {
        get
        {
            ToolStripTextDirection textDirection = Properties.GetValueOrDefault(s_textDirectionProperty, ToolStripTextDirection.Inherit);

            if (textDirection == ToolStripTextDirection.Inherit)
            {
                if (ParentInternal is not null)
                {
                    // In the case we're on a ToolStripOverflow
                    textDirection = ParentInternal.TextDirection;
                }
                else
                {
                    textDirection = (Owner is null) ? ToolStripTextDirection.Horizontal : Owner.TextDirection;
                }
            }

            return textDirection;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            Properties.AddOrRemoveValue(s_textDirectionProperty, value, defaultValue: ToolStripTextDirection.Inherit);
            InvalidateItemLayout("TextDirection");
        }
    }

    [DefaultValue(TextImageRelation.ImageBeforeText)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemTextImageRelationDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public TextImageRelation TextImageRelation
    {
        get => _textImageRelation;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value != TextImageRelation)
            {
                _textImageRelation = value;
                InvalidateItemLayout(PropertyNames.TextImageRelation);
            }
        }
    }

    /// <summary>
    ///  !!!!This property ONLY works when toolStrip.ShowItemToolTips = true!!!!
    ///  if AutoToolTip is set to true we return the Text as the ToolTipText.
    /// </summary>
    [SRDescription(nameof(SR.ToolStripItemToolTipTextDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    [Editor($"System.ComponentModel.Design.MultilineStringEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
    public string? ToolTipText
    {
        get
        {
            if (AutoToolTip && string.IsNullOrEmpty(_toolTipText))
            {
                string? toolText = Text;
                if (WindowsFormsUtils.ContainsMnemonic(toolText))
                {
                    // this shouldn't be called a lot so we can take the perf hit here.
                    toolText = string.Join(string.Empty, toolText.Split('&'));
                }

                return toolText;
            }

            return _toolTipText;
        }
        set => _toolTipText = value;
    }

    /// <summary>
    ///  Whether or not the item is visible
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolStripItemVisibleDescr))]
    public bool Visible
    {
        get => (ParentInternal is not null) && (ParentInternal.Visible) && Available;
        set => SetVisibleCore(value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ToolStripItemOnVisibleChangedDescr))]
    public event EventHandler? VisibleChanged
    {
        add => Events.AddHandler(s_visibleChangedEvent, value);
        remove => Events.RemoveHandler(s_visibleChangedEvent, value);
    }

    /// <summary>
    ///  The width of this ToolStripItem.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Width
    {
        get => Bounds.Width;
        set
        {
            Rectangle currentBounds = Bounds;
            SetBounds(currentBounds.X, currentBounds.Y, value, currentBounds.Height);
        }
    }

    internal void AccessibilityNotifyClients(AccessibleEvents accEvent)
    {
        if (ParentInternal is not null)
        {
            int index = ParentInternal.DisplayedItems.IndexOf(this);
            ParentInternal.AccessibilityNotifyClients(accEvent, index);
        }
    }

    private void Animate()
        => Animate(!DesignMode && Visible && Enabled && ParentInternal is not null);

    private void StopAnimate() => Animate(false);

    private void Animate(bool animate)
    {
        if (animate == _state[s_stateCurrentlyAnimatingImage])
        {
            return;
        }

        Image? image = Image;
        if (image is null)
        {
            return;
        }

        if (animate)
        {
            ImageAnimator.Animate(image, OnAnimationFrameChanged);
        }
        else
        {
            ImageAnimator.StopAnimate(image, OnAnimationFrameChanged);
        }

        _state[s_stateCurrentlyAnimatingImage] = animate;
    }

    internal bool BeginDragForItemReorder()
    {
        if (Control.ModifierKeys == Keys.Alt)
        {
            if (ParentInternal is not null && ParentInternal.Items.Contains(this) && ParentInternal.AllowItemReorder)
            {
                // we only drag
                ToolStripItem item = this;
                DoDragDrop(item, DragDropEffects.Move);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Constructs the new instance of the accessibility object for this ToolStripItem. Subclasses
    ///  should not call base.CreateAccessibilityObject.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual AccessibleObject CreateAccessibilityInstance() => new ToolStripItemAccessibleObject(this);

    /// <summary>
    ///  Creates an instance of the object that defines how image and text
    ///  gets laid out in the ToolStripItem
    /// </summary>
    private protected virtual ToolStripItemInternalLayout CreateInternalLayout() => new(this);

    /// <summary>
    ///  Disposes this ToolStrip item.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _state[s_stateDisposing] = true;

            if (Owner is not null)
            {
                StopAnimate();
                Debug.Assert(Owner.Items.Contains(this), "How can there be a owner and not be in the collection?");
                Owner.Items.Remove(this);
                _toolStripItemInternalLayout = null;
                _state[s_stateDisposed] = true;
            }
        }

        base.Dispose(disposing);

        if (disposing)
        {
            // need to call base() first since the Component.Dispose(_) is listened to by the ComponentChangeService
            // which Serializes the object in Undo-Redo transactions.
            Properties.RemoveValue(s_mirroredImageProperty);
            Properties.RemoveValue(s_imageProperty);
            _state[s_stateDisposing] = false;
        }
    }

    internal static long DoubleClickTicks
    {
        // (DoubleClickTime in ms) * 1,000,000 ns/1ms * 1 Tick / 100ns = XXX in Ticks.
        // Therefore: (DoubleClickTime) * 1,000,000/100 = xxx in Ticks.
        get => SystemInformation.DoubleClickTime * 10000;
    }

    /// <summary>
    ///  Begins a drag operation. The allowedEffects determine which
    ///  drag operations can occur. If the drag operation needs to interop
    ///  with applications in another process, data should either be
    ///  a base managed class (String, Bitmap, or Metafile) or some Object
    ///  that implements System.Runtime.Serialization.ISerializable. data can also be any Object that
    ///  implements System.Windows.Forms.IDataObject.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
    {
        return DoDragDrop(data, allowedEffects, dragImage: null, cursorOffset: default, useDefaultDragImage: false);
    }

    /// <summary>
    ///  Begins a drag operation. The <paramref name="allowedEffects"/> determine which drag operations can occur.
    ///  If the drag operation needs to interop with applications in another process, <paramref name="data"/> should
    ///  either be a base managed class (<see cref="string"/>, <see cref="Bitmap"/>, or <see cref="Metafile"/>)
    ///  or some <see cref="object"/> that implements <see cref="Runtime.Serialization.ISerializable"/>.
    ///  <paramref name="data"/> can also be any <see cref="object"/> that implements <see cref="IDataObject"/>.
    ///  <paramref name="dragImage"/> is the bitmap that will be displayed during the  drag operation and
    ///  <paramref name="cursorOffset"/> specifies the location of the cursor within <paramref name="dragImage"/>,
    ///  which is an offset from the upper-left corner. Specify <see langword="true"/> for
    ///  <paramref name="useDefaultDragImage"/> to use a layered window drag image with a
    ///  size of 96x96; otherwise <see langword="false"/>. Note the outer edges of <paramref name="dragImage"/>
    ///  are blended out if the image width or height exceeds 300 pixels.
    /// </summary>
    /// <returns>
    ///  A value from the <see cref="DragDropEffects"/> enumeration that represents the final effect that was performed
    ///  during the drag-and-drop operation.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   Because <see cref="DoDragDrop(object, DragDropEffects, Bitmap, Point, bool)"/> always performs the
    ///   RGB multiplication step in calculating the alpha value, you should always pass a <see cref="Bitmap"/>
    ///   without pre-multiplied alpha blending. Note that no error will result from passing a <see cref="Bitmap"/>
    ///   with pre-multiplied alpha blending, but this method will multiply it again, doubling the resulting alpha value.
    ///  </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public unsafe DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects, Bitmap? dragImage, Point cursorOffset, bool useDefaultDragImage)
    {
        if (data is not IComDataObject dataObject)
        {
            DataObject? iwdata;
            if (data is IDataObject idataObject)
            {
                iwdata = new DataObject(idataObject);
            }
            else if (data is ToolStripItem)
            {
                // it seems that the DataObject does string comparison
                // on the type, so you can't ask for GetDataPresent of
                // a base type (e.g. ToolStripItem) when you are really
                // looking at a ToolStripButton. The alternative is
                // to set the format string expressly to a string matching
                // the type of ToolStripItem
                iwdata = new DataObject();
                iwdata.SetData(typeof(ToolStripItem).ToString(), data);
            }
            else
            {
                iwdata = new DataObject();
                iwdata.SetData(data);
            }

            dataObject = iwdata;
        }

        DROPEFFECT finalEffect;

        try
        {
            using var dropSource = ComHelpers.GetComScope<IDropSource>(CreateDropSource(dataObject, dragImage, cursorOffset, useDefaultDragImage));
            using var dataObjectScope = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
            if (PInvoke.DoDragDrop(dataObjectScope, dropSource, (DROPEFFECT)(uint)allowedEffects, out finalEffect).Failed)
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
    ///  This represents what we're actually going to drag. If the parent has set AllowItemReorder to true,
    ///  then the item should call back on the private OnQueryContinueDrag/OnGiveFeedback that is implemented
    ///  in the parent ToolStrip.
    ///
    ///  Else if the parent does not support reordering of items (Parent.AllowItemReorder = false) -
    ///  then call back on the ToolStripItem's OnQueryContinueDrag/OnGiveFeedback methods.
    /// </summary>
    internal IDropSource.Interface CreateDropSource(IComDataObject dataObject, Bitmap? dragImage, Point cursorOffset, bool useDefaultDragImage)
        => ParentInternal is not null && ParentInternal.AllowItemReorder && ParentInternal.ItemReorderDropSource is not null
            ? new DropSource(ParentInternal.ItemReorderDropSource, dataObject, dragImage, cursorOffset, useDefaultDragImage)
            : new DropSource(this, dataObject, dragImage, cursorOffset, useDefaultDragImage);

    internal void FireEvent(ToolStripItemEventType met)
        => FireEvent(EventArgs.Empty, met);

    internal void FireEvent(EventArgs e, ToolStripItemEventType met)
    {
        switch (met)
        {
            case ToolStripItemEventType.LocationChanged:
                OnLocationChanged(e);
                break;
            case ToolStripItemEventType.Paint:
                HandlePaint((PaintEventArgs)e);
                break;
            case ToolStripItemEventType.MouseHover:
                // disabled toolstrip items should show tooltips.
                // we won't raise mouse events though.
                if (!Enabled && ParentInternal is not null && !string.IsNullOrEmpty(ToolTipText))
                {
                    ParentInternal.UpdateToolTip(this);
                }
                else
                {
                    FireEventInteractive(e, met);
                }

                break;
            case ToolStripItemEventType.MouseEnter:
                HandleMouseEnter(e);
                break;
            case ToolStripItemEventType.MouseLeave:
                // disabled toolstrip items should also clear tooltips.
                // disabled toolstrip items should also handle leave.
                // we won't raise mouse events though.
                if (!Enabled && ParentInternal is not null)
                {
                    ParentInternal.UpdateToolTip(item: null);
                    HandleLeave();
                }
                else
                {
                    HandleMouseLeave(e);
                }

                break;
            case ToolStripItemEventType.MouseMove:
                // Disabled items typically don't get mouse move
                // but they should be allowed to re-order if the ALT key is pressed
                if (!Enabled && ParentInternal is not null)
                {
                    BeginDragForItemReorder();
                }
                else
                {
                    FireEventInteractive(e, met);
                }

                break;
            default:
                FireEventInteractive(e, met);
                break;
        }
    }

    internal void FireEventInteractive(EventArgs e, ToolStripItemEventType met)
    {
        if (Enabled)
        {
            switch (met)
            {
                case ToolStripItemEventType.MouseMove:
                    HandleMouseMove((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.MouseHover:
                    HandleMouseHover(e);
                    break;
                case ToolStripItemEventType.MouseUp:
                    HandleMouseUp((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.MouseDown:
                    HandleMouseDown((MouseEventArgs)e);
                    break;
                case ToolStripItemEventType.Click:
                    HandleClick(e);
                    break;
                case ToolStripItemEventType.DoubleClick:
                    HandleDoubleClick(e);
                    break;
                default:
                    Debug.Assert(false, "Invalid event type.");
                    break;
            }
        }
    }

    private Font? GetOwnerFont() => Owner?.Font;

    /// <summary>
    ///  We don't want a public settable property and usually owner will work
    ///  except for things like the overflow button
    /// </summary>
    public ToolStrip? GetCurrentParent() => Parent;

    internal ToolStripDropDown? GetCurrentParentDropDown()
    {
        if (ParentInternal is not null)
        {
            return ParentInternal as ToolStripDropDown;
        }

        return Owner as ToolStripDropDown;
    }

    public virtual Size GetPreferredSize(Size constrainingSize)
    {
        // Switch Size.Empty to maximum possible values
        constrainingSize = LayoutUtils.ConvertZeroToUnbounded(constrainingSize);
        return InternalLayout.GetPreferredSize(constrainingSize - Padding.Size) + Padding.Size;
    }

    internal Size GetTextSize()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return Size.Empty;
        }
        else if (_cachedTextSize == Size.Empty)
        {
            _cachedTextSize = TextRenderer.MeasureText(Text, Font);
        }

        return _cachedTextSize;
    }

    /// <summary>
    ///  Invalidates the ToolStripItem
    /// </summary>
    public void Invalidate() => ParentInternal?.Invalidate(Bounds, true);

    /// <summary>
    ///  invalidates a rectangle within the ToolStripItem's bounds
    /// </summary>
    public void Invalidate(Rectangle r)
    {
        // the only value add to this over calling invalidate on the ToolStrip is that
        // you can specify the rectangle with coordinates relative to the upper left hand
        // corner of the ToolStripItem.
        Point rectangleLocation = TranslatePoint(r.Location, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ToolStripCoords);
        ParentInternal?.Invalidate(new Rectangle(rectangleLocation, r.Size), true);
    }

    internal void InvalidateItemLayout(string affectedProperty)
    {
        _toolStripItemInternalLayout = null;

        if (Owner is not null)
        {
            LayoutTransaction.DoLayout(Owner, this, affectedProperty);
            // DoLayout may cause the ToolStrip size to change. If the ToolStrip is an MdiControlStrip, the
            // active Mdi child is maximized, and DoLayout causes the size to change then Form.MdiControlStrip
            // (Owner) will be disposed and replaced with a new one. This means the current ToolStripItem will
            // also be disposed and have no owner on the next line. See https://github.com/dotnet/winforms/issues/6535
            Owner?.Invalidate();
        }
    }

    internal void InvalidateImageListImage()
    {
        // Invalidate the cache.
        if (ImageIndexer.ActualIndex >= 0)
        {
            Properties.RemoveValue(s_imageProperty);
            InvalidateItemLayout(PropertyNames.Image);
        }
    }

    internal void InvokePaint() => ParentInternal?.InvokePaintItem(this);

    protected internal virtual bool IsInputKey(Keys keyData) => false;

    protected internal virtual bool IsInputChar(char charCode) => false;

    private void HandleClick(EventArgs e)
    {
        try
        {
            if (!DesignMode)
            {
                _state[s_statePressed] = true;
            }

            // Force painting w/o using message loop here because it may be quite a long
            // time before it gets pumped again.
            InvokePaint();

            if (SupportsItemClick && Owner is not null)
            {
                Owner.HandleItemClick(this);
            }

            OnClick(e);

            if (SupportsItemClick && Owner is not null)
            {
                Owner.HandleItemClicked(this);
            }
        }
        finally
        {
            _state[s_statePressed] = false;
        }

        // when we get around to it, paint unpressed.
        Invalidate();
    }

    private void HandleDoubleClick(EventArgs e) => OnDoubleClick(e);

    private void HandlePaint(PaintEventArgs e)
    {
        Animate();
        ImageAnimator.UpdateFrames(Image);

        OnPaint(e);
        RaisePaintEvent(s_paintEvent, e);
    }

    private void HandleMouseEnter(EventArgs e)
    {
        if (!DesignMode)
        {
            if (ParentInternal is not null
                 && ParentInternal.CanHotTrack
                 && ParentInternal.ShouldSelectItem())
            {
                if (Enabled)
                {
                    // calling select can dismiss a child dropdown which would break auto-expansion.
                    // save off auto expand and restore it.
                    bool autoExpand = ParentInternal.MenuAutoExpand;

                    if (ParentInternal.LastMouseDownedItem == this)
                    {
                        // Same as Control.MouseButtons == MouseButtons.Left, but slightly more efficient.
                        if (PInvoke.GetKeyState((int)Keys.LButton) < 0)
                        {
                            Push(true);
                        }
                    }

                    Select();
                    ParentInternal.MenuAutoExpand = autoExpand;
                }
                else if (SupportsDisabledHotTracking)
                {
                    Select();
                }
            }
        }

        KeyboardToolTipStateMachine.Instance.NotifyAboutMouseEnter(this);

        if (Enabled)
        {
            OnMouseEnter(e);
            RaiseEvent(s_mouseEnterEvent, e);
        }
    }

    private void HandleMouseMove(MouseEventArgs mea)
    {
        if (Enabled && CanSelect && !Selected)
        {
            if (ParentInternal is not null
                 && ParentInternal.CanHotTrack
                 && ParentInternal.ShouldSelectItem())
            {
                // this is the case where we got a mouse enter, but ShouldSelectItem
                // returned false.
                // typically occurs when a window first opens - we get a mouse enter on the item
                // the cursor is hovering over - but we don't actually want to change selection to it.
                Select();
            }
        }

        OnMouseMove(mea);
        RaiseMouseEvent(s_mouseMoveEvent, mea);
    }

    private void HandleMouseHover(EventArgs e)
    {
        OnMouseHover(e);
        RaiseEvent(s_mouseHoverEvent, e);
    }

    private void HandleLeave()
    {
        if (_state[s_stateMouseDownAndNoDrag] || _state[s_statePressed] || _state[s_stateSelected])
        {
            _state[s_stateMouseDownAndNoDrag | s_statePressed | s_stateSelected] = false;

            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);

            Invalidate();
        }
    }

    private void HandleMouseLeave(EventArgs e)
    {
        HandleLeave();
        if (Enabled)
        {
            OnMouseLeave(e);
            RaiseEvent(s_mouseLeaveEvent, e);
        }
    }

    private void HandleMouseDown(MouseEventArgs e)
    {
        _state[s_stateMouseDownAndNoDrag] = !BeginDragForItemReorder();
        if (_state[s_stateMouseDownAndNoDrag])
        {
            if (e.Button == MouseButtons.Left)
            {
                Push(true);
            }

            OnMouseDown(e);
            RaiseMouseEvent(s_mouseDownEvent, e);
        }
    }

    private void HandleMouseUp(MouseEventArgs e)
    {
        bool fireMouseUp = (ParentInternal?.LastMouseDownedItem == this);

        if (!fireMouseUp && !MouseDownAndUpMustBeInSameItem)
        {
            // in the case of menus, you can mouse down on one item and mouse up
            // on another. We do need to be careful
            // that the mouse has actually moved from when a dropdown has been opened -
            // otherwise we may accidentally click what's underneath the mouse at the time
            // the dropdown is opened.
            fireMouseUp = ParentInternal is not null && ParentInternal.ShouldSelectItem();
        }

        if (_state[s_stateMouseDownAndNoDrag] || fireMouseUp)
        {
            Push(false);

            if (e.Button == MouseButtons.Left || (e.Button == MouseButtons.Right && _state[s_stateSupportsRightClick]))
            {
                bool shouldFireDoubleClick = false;
                if (DoubleClickEnabled)
                {
                    long newTime = DateTime.Now.Ticks;
                    long deltaTicks = newTime - _lastClickTime;
                    _lastClickTime = newTime;
                    // use >= for cases where the delta is so fast DateTime cannot pick up the delta.
                    Debug.Assert(deltaTicks >= 0, "why are deltaticks less than zero? thats some mighty fast clicking");
                    // if we've seen a mouse up less than the double click time ago, we should fire.
                    if (deltaTicks >= 0 && deltaTicks < DoubleClickTicks)
                    {
                        shouldFireDoubleClick = true;
                    }
                }

                if (shouldFireDoubleClick)
                {
                    HandleDoubleClick(EventArgs.Empty);
                    // If we actually fired DoubleClick - reset the lastClickTime.
                    _lastClickTime = 0;
                }
                else
                {
                    HandleClick(EventArgs.Empty);
                }
            }

            OnMouseUp(e);
            RaiseMouseEvent(s_mouseUpEvent, e);
        }
    }

    internal virtual void OnAccessibleDescriptionChanged(EventArgs e)
    {
    }

    internal virtual void OnAccessibleNameChanged(EventArgs e)
    {
    }

    internal virtual void OnAccessibleDefaultActionDescriptionChanged(EventArgs e)
    {
    }

    internal virtual void OnAccessibleRoleChanged(EventArgs e)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnBackColorChanged(EventArgs e)
    {
        Invalidate();
        RaiseEvent(s_backColorChangedEvent, e);
    }

    protected virtual void OnBoundsChanged()
    {
        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
        InternalLayout.PerformLayout();
    }

    protected virtual void OnClick(EventArgs e)
    {
        RaiseEvent(s_clickEvent, e);
        OnRequestCommandExecute(e);
    }

    protected internal virtual void OnLayout(LayoutEventArgs e)
    {
    }

    void IDropTarget.OnDragEnter(DragEventArgs dragEvent) => OnDragEnter(dragEvent);

    void IDropTarget.OnDragOver(DragEventArgs dragEvent) => OnDragOver(dragEvent);

    void IDropTarget.OnDragLeave(EventArgs e) => OnDragLeave(e);

    void IDropTarget.OnDragDrop(DragEventArgs dragEvent) => OnDragDrop(dragEvent);

    void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs)
        => OnGiveFeedback(giveFeedbackEventArgs);

    void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs)
        => OnQueryContinueDrag(queryContinueDragEventArgs);

    private void OnAnimationFrameChanged(object? o, EventArgs e)
    {
        ToolStrip? parent = ParentInternal;
        if (parent is not null)
        {
            if (parent.Disposing || parent.IsDisposed)
            {
                return;
            }

            if (parent.IsHandleCreated && parent.InvokeRequired)
            {
                parent.BeginInvoke(new EventHandler(OnAnimationFrameChanged), [o, e]);
                return;
            }

            Invalidate();
        }
    }

    protected virtual void OnAvailableChanged(EventArgs e) => RaiseEvent(s_availableChangedEvent, e);

    /// <summary>
    ///  Raises the <see cref="CommandChanged"/> event.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCommandChanged(EventArgs e)
        => RaiseEvent(s_commandChangedEvent, e);

    /// <summary>
    ///  Raises the <see cref="CommandCanExecuteChanged"/> event.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCommandCanExecuteChanged(EventArgs e)
        => ((EventHandler?)Events[s_commandCanExecuteChangedEvent])?.Invoke(this, e);

    /// <summary>
    ///  Raises the <see cref="CommandParameterChanged"/> event.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCommandParameterChanged(EventArgs e) => RaiseEvent(s_commandParameterChangedEvent, e);

    /// <summary>
    ///  Called in the context of <see cref="OnClick(EventArgs)"/> to invoke
    ///  <see cref="Input.ICommand.Execute(object?)"/> if the context allows.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    protected virtual void OnRequestCommandExecute(EventArgs e)
        => ICommandBindingTargetProvider.RequestCommandExecute(this);

    // Called by the CommandProviderManager's command handling logic.
    void ICommandBindingTargetProvider.RaiseCommandChanged(EventArgs e)
        => OnCommandChanged(e);

    // Called by the CommandProviderManager's command handling logic.
    void ICommandBindingTargetProvider.RaiseCommandCanExecuteChanged(EventArgs e)
        => OnCommandCanExecuteChanged(e);

    /// <summary>
    ///  Raises the <see cref="DragEnter"/> event.
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnEnter to send this event to any registered event listeners.
    /// </summary>
    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnDragEnter to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragEnter(DragEventArgs dragEvent)
        => RaiseDragEvent(s_dragEnterEvent, dragEvent);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnDragOver to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragOver(DragEventArgs dragEvent)
        => RaiseDragEvent(s_dragOverEvent, dragEvent);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnDragLeave to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragLeave(EventArgs e)
        => RaiseEvent(s_dragLeaveEvent, e);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnDragDrop to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDragDrop(DragEventArgs dragEvent)
        => RaiseDragEvent(s_dragDropEvent, dragEvent);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDisplayStyleChanged(EventArgs e)
        => RaiseEvent(s_displayStyleChangedEvent, e);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnGiveFeedback to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEvent)
        => ((GiveFeedbackEventHandler?)Events[s_giveFeedbackEvent])?.Invoke(this, giveFeedbackEvent);

    internal virtual void OnImageScalingSizeChanged(EventArgs e)
    {
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnQueryContinueDrag to send this event to any registered event listeners.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEvent)
        => RaiseQueryContinueDragEvent(s_queryContinueDragEvent, queryContinueDragEvent);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnDoubleClick(EventArgs e) => RaiseEvent(s_doubleClickEvent, e);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnEnabledChanged(EventArgs e)
    {
        RaiseEvent(s_enabledChangedEvent, e);
        Animate();
    }

    internal void OnInternalEnabledChanged(EventArgs e) => RaiseEvent(s_internalEnabledChangedEvent, e);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnForeColorChanged(EventArgs e)
    {
        Invalidate();
        RaiseEvent(s_foreColorChangedEvent, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnFontChanged(EventArgs e)
    {
        _cachedTextSize = Size.Empty;
        // PERF - only invalidate if we actually care about the font
        if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
        {
            InvalidateItemLayout(PropertyNames.Font);
        }
        else
        {
            _toolStripItemInternalLayout = null;
        }

        RaiseEvent(s_fontChangedEvent, e);
    }

    protected virtual void OnLocationChanged(EventArgs e) => RaiseEvent(s_locationChangedEvent, e);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnMouseEnter(EventArgs e)
    {
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnMouseMove(MouseEventArgs mea)
    {
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnMouseHover(EventArgs e)
    {
        if (ParentInternal is not null && !string.IsNullOrEmpty(ToolTipText))
        {
            ParentInternal.UpdateToolTip(this);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnMouseLeave(EventArgs e) => ParentInternal?.UpdateToolTip(item: null);

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnMouseDown(MouseEventArgs e)
    {
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnMouseUp(MouseEventArgs e)
    {
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnPaint(PaintEventArgs e)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentBackColorChanged(EventArgs e)
    {
        Color backColor = Properties.GetValueOrDefault<Color>(s_backColorProperty);
        if (backColor.IsEmpty)
        {
            OnBackColorChanged(e);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnParentChanged(ToolStrip? oldParent, ToolStrip? newParent)
    {
        SetAmbientMargin();
        if ((oldParent is not null) && (oldParent.DropTargetManager is not null))
        {
            oldParent.DropTargetManager.EnsureUnRegistered();
        }

        if (AllowDrop && (newParent is not null))
        {
            EnsureParentDropTargetRegistered();
        }

        Animate();
    }

    /// <summary>
    ///  Occurs when this.Parent.Enabled changes.
    /// </summary>
    protected internal virtual void OnParentEnabledChanged(EventArgs e)
        => OnEnabledChanged(EventArgs.Empty);

    /// <summary>
    ///  Occurs when the font property has changed on the parent - used to notify inheritors of the font property that
    ///  the font has changed
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void OnOwnerFontChanged(EventArgs e)
    {
        if (!TryGetExplicitlySetFont(out _))
        {
            OnFontChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnParentForeColorChanged(EventArgs e)
    {
        Color foreColor = Properties.GetValueOrDefault<Color>(s_foreColorProperty);
        if (foreColor.IsEmpty)
        {
            OnForeColorChanged(e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void OnParentRightToLeftChanged(EventArgs e)
    {
        if (!Properties.TryGetValue(s_rightToLeftProperty, out RightToLeft rightToLeft) || rightToLeft == RightToLeft.Inherit)
        {
            OnRightToLeftChanged(e);
        }
    }

    /// <summary>
    ///  Occurs when the owner of an item changes.
    /// </summary>
    protected virtual void OnOwnerChanged(EventArgs e)
    {
        RaiseEvent(s_ownerChangedEvent, e);
        SetAmbientMargin();
        if (Owner is not null)
        {
            // check if we need to fire OnRightToLeftChanged
            if (!Properties.TryGetValue(s_rightToLeftProperty, out RightToLeft rightToLeft))
            {
                rightToLeft = RightToLeft.Inherit;
            }

            if (rightToLeft == RightToLeft.Inherit && RightToLeft != DefaultRightToLeft)
            {
                OnRightToLeftChanged(EventArgs.Empty);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal void OnOwnerTextDirectionChanged()
    {
        ToolStripTextDirection textDirection = Properties.GetValueOrDefault(s_textDirectionProperty, ToolStripTextDirection.Inherit);

        if (textDirection == ToolStripTextDirection.Inherit)
        {
            InvalidateItemLayout("TextDirection");
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnRightToLeftChanged(EventArgs e)
    {
        InvalidateItemLayout(PropertyNames.RightToLeft);
        RaiseEvent(s_rightToLeftChangedEvent, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnTextChanged(EventArgs e)
    {
        _cachedTextSize = Size.Empty;
        // Make sure we clear the cache before we perform the layout.
        InvalidateItemLayout(PropertyNames.Text);
        RaiseEvent(s_textChangedEvent, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected virtual void OnVisibleChanged(EventArgs e)
    {
        if (Owner is not null && !(Owner.IsDisposed || Owner.Disposing))
        {
            Owner.OnItemVisibleChanged(new ToolStripItemEventArgs(this), performLayout: true);
        }

        RaiseEvent(s_visibleChangedEvent, e);
        Animate();
    }

    public void PerformClick()
    {
        if (Enabled && Available)
        {
            FireEvent(ToolStripItemEventType.Click);
        }
    }

    /// <summary>
    ///  Pushes the button.
    /// </summary>
    internal void Push(bool push)
    {
        if (!CanSelect || !Enabled || DesignMode)
        {
            return;
        }

        if (_state[s_statePressed] != push)
        {
            _state[s_statePressed] = push;
            if (Available)
            {
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  See Control.ProcessDialogKey for more info.
    /// </summary>
    protected internal virtual bool ProcessDialogKey(Keys keyData)
    {
        if (keyData == Keys.Enter || (_state[s_stateSupportsSpaceKey] && keyData == Keys.Space))
        {
            FireEvent(ToolStripItemEventType.Click);
            if (ParentInternal is not null && !ParentInternal.IsDropDown && Enabled)
            {
                ParentInternal.RestoreFocusInternal();
            }

            Owner?.UpdateToolTip(this, true);

            return true;
        }

        return false;
    }

    /// <summary>
    ///  See Control.ProcessCmdKey for more info.
    /// </summary>
    protected internal virtual bool ProcessCmdKey(ref Message m, Keys keyData) => false;

    protected internal virtual bool ProcessMnemonic(char charCode)
    {
        // checking IsMnemonic is not necessary - control does this for us.
        FireEvent(ToolStripItemEventType.Click);
        return true;
    }

    internal void RaiseCancelEvent(object key, CancelEventArgs e) => ((CancelEventHandler?)Events[key])?.Invoke(this, e);

    internal void RaiseDragEvent(object key, DragEventArgs e) => ((DragEventHandler?)Events[key])?.Invoke(this, e);

    internal void RaiseKeyEvent(object key, KeyEventArgs e) => ((KeyEventHandler?)Events[key])?.Invoke(this, e);

    internal void RaiseKeyPressEvent(object key, KeyPressEventArgs e) => ((KeyPressEventHandler?)Events[key])?.Invoke(this, e);

    internal void RaiseMouseEvent(object key, MouseEventArgs e) => ((MouseEventHandler?)Events[key])?.Invoke(this, e);

    internal void RaisePaintEvent(object key, PaintEventArgs e) => ((PaintEventHandler?)Events[key])?.Invoke(this, e);

    internal void RaiseQueryContinueDragEvent(object key, QueryContinueDragEventArgs e) =>
        ((QueryContinueDragEventHandler?)Events[key])?.Invoke(this, e);

    internal virtual void ReleaseUiaProvider()
    {
        if (Properties.TryGetValue(s_accessibilityProperty, out AccessibleObject? accessibleObject))
        {
            PInvoke.UiaDisconnectProvider(accessibleObject, skipOSCheck: true);
            Properties.RemoveValue(s_accessibilityProperty);
        }
    }

    private void ResetToolTipText() => _toolTipText = null;

    // This will only be called in PerMonitorV2 scenarios.
    internal virtual void ToolStrip_RescaleConstants(int oldDpi, int newDpi)
    {
        DeviceDpi = newDpi;
        RescaleConstantsInternal(newDpi);
        OnFontChanged(EventArgs.Empty);
    }

    internal void RescaleConstantsInternal(int newDpi)
    {
        ToolStripManager.CurrentDpi = newDpi;
        _defaultFont = ToolStripManager.DefaultFont;
        ScaleConstants(newDpi);
    }

    private void ScaleConstants(int dpi)
    {
        _defaultMargin = ScaleHelper.ScaleToDpi(new Padding(0, 1, 0, 2), dpi);
        _defaultStatusStripMargin = ScaleHelper.ScaleToDpi(new Padding(0, 2, 0, 0), dpi);
    }

    public void Select() => Select(forceRaiseAccessibilityFocusChanged: false);

    internal void Select(bool forceRaiseAccessibilityFocusChanged)
    {
        if (!CanSelect)
        {
            return;
        }

        if (Owner is not null && Owner.IsCurrentlyDragging)
        {
            // Make sure we don't select during a drag operation.
            return;
        }

        if (ParentInternal is not null && ParentInternal.IsSelectionSuspended)
        {
            return;
        }

        if (!Selected)
        {
            _state[s_stateSelected] = true;
            ParentInternal?.NotifySelectionChange(this);

            if (IsOnDropDown)
            {
                if (OwnerItem is not null && OwnerItem.IsOnDropDown)
                {
                    // Ensure the selection is moved back to our owner item.
                    OwnerItem.Select();
                }
            }

            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(this);

            OnSelectedChanged(EventArgs.Empty);

            forceRaiseAccessibilityFocusChanged = true;
        }

        if (forceRaiseAccessibilityFocusChanged)
        {
            bool accessibilityIsOn = IsAccessibilityObjectCreated ||
                // When ToolStripItem is selected automatically for the first time
                // (for example, when menu bar gets focus or a sub menu is opened, its first item is selected automatically),
                // ToolStripItem's and parent sub menu's AccessibilityObjects might not be created yet.
                // AO tree is going to be constructed just after this first selection, if any accessibility client is on.
                // In this case, to be able to notify Accessibility of focus event right now
                // we determine Accessibility status by checking if parent AO instance is created.
                // If so, then we can force child AO creation.
                (IsOnDropDown
                    ? OwnerItem?.IsAccessibilityObjectCreated ?? false
                    : IsParentAccessibilityObjectCreated);

            if (accessibilityIsOn && AccessibilityObject is ToolStripItemAccessibleObject accessibleObject)
            {
                accessibleObject.RaiseFocusChanged();
            }
        }
    }

    internal void SetOwner(ToolStrip? newOwner)
    {
        if (_owner != newOwner)
        {
            Font f = Font;

            if (_owner is not null)
            {
                _owner._rescaleConstsCallbackDelegate -= ToolStrip_RescaleConstants;
            }

            _owner = newOwner;

            if (_owner is not null)
            {
                _owner._rescaleConstsCallbackDelegate += ToolStrip_RescaleConstants;
            }

            // clear the parent if the owner is null.
            if (newOwner is null)
            {
                ParentInternal = null;
            }

            if (!_state[s_stateDisposing] && !IsDisposed)
            {
                OnOwnerChanged(EventArgs.Empty);
                if (f != Font)
                {
                    OnFontChanged(EventArgs.Empty);
                }
            }
        }
    }

    protected virtual void SetVisibleCore(bool visible)
    {
        if (_state[s_stateVisible] != visible)
        {
            _state[s_stateVisible] = visible;
            Unselect();
            Push(false);

            OnAvailableChanged(EventArgs.Empty);
            OnVisibleChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Sets the bounds of the item
    /// </summary>
    protected internal virtual void SetBounds(Rectangle bounds)
    {
        Rectangle oldBounds = _bounds;
        _bounds = bounds;

        if (!_state[s_stateConstructing])
        {
            // Don't fire while we're in the base constructor as the inherited
            // class may not have had a chance to initialize yet.

            if (_bounds != oldBounds)
            {
                OnBoundsChanged();
            }

            if (_bounds.Location != oldBounds.Location)
            {
                OnLocationChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Sets the bounds of the item
    /// </summary>
    internal void SetBounds(int x, int y, int width, int height)
        => SetBounds(new Rectangle(x, y, width, height));

    /// <summary>
    ///  Sets the placement of the item
    /// </summary>
    internal void SetPlacement(ToolStripItemPlacement placement) => _placement = placement;

    /// <remarks>
    ///  <para>
    ///   Some implementations of DefaultMargin check which container they
    ///   are on. They need to be re-evaluated when the containership changes.
    ///   DefaultMargin will stop being honored the moment someone sets the Margin property.
    ///  </para>
    /// </remarks>
    internal void SetAmbientMargin()
    {
        if (_state[s_stateUseAmbientMargin] && Margin != DefaultMargin)
        {
            CommonProperties.SetMargin(this, DefaultMargin);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeImageTransparentColor() => ImageTransparentColor != Color.Empty;

    /// <summary>
    ///  Returns true if the backColor should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeBackColor()
    {
        Color backColor = Properties.GetValueOrDefault<Color>(s_backColorProperty);
        return !backColor.IsEmpty;
    }

    private bool ShouldSerializeDisplayStyle() => DisplayStyle != DefaultDisplayStyle;

    private bool ShouldSerializeToolTipText() => !string.IsNullOrEmpty(_toolTipText);

    /// <summary>
    ///  Returns true if the foreColor should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeForeColor()
    {
        Color foreColor = Properties.GetValueOrDefault<Color>(s_foreColorProperty);
        return !foreColor.IsEmpty;
    }

    /// <summary>
    ///  Returns true if the font should be persisted in code gen.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeFont() => TryGetExplicitlySetFont(out _);

    /// <summary>
    ///  Determines if the <see cref="Padding"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializePadding() => Padding != DefaultPadding;

    /// <summary>
    ///  Determines if the <see cref="Margin"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeMargin() => Margin != DefaultMargin;

    /// <summary>
    ///  Determines if the <see cref="Visible"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeVisible() => !_state[s_stateVisible];

    /// <summary>
    ///  Determines if the <see cref="Image"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeImage() => Image is not null && ImageIndexer.ActualIndex < 0;

    /// <summary>
    ///  Determines if the <see cref="Image"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeImageKey()
        => Image is not null && ImageIndexer.ActualIndex >= 0 && !string.IsNullOrEmpty(ImageIndexer.Key);

    /// <summary>
    ///  Determines if the <see cref="Image"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeImageIndex()
        => Image is not null && ImageIndexer.ActualIndex >= 0 && ImageIndexer.Index != ImageList.Indexer.DefaultIndex;

    /// <summary>
    ///  Determines if the <see cref="RightToLeft"/> property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeRightToLeft()
    {
        if (!Properties.TryGetValue(s_rightToLeftProperty, out RightToLeft rightToLeft))
        {
            return false;
        }

        return rightToLeft != DefaultRightToLeft;
    }

    private bool ShouldSerializeTextDirection() => Properties.ContainsKey(s_textDirectionProperty);

    /// <summary>
    ///  Resets the back color to be based on the parent's back color.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetBackColor() => BackColor = Color.Empty;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetDisplayStyle() => DisplayStyle = DefaultDisplayStyle;

    /// <summary>
    ///  Resets the fore color to be based on the parent's fore color.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetForeColor() => ForeColor = Color.Empty;

    /// <summary>
    ///  Resets the Font to be based on the parent's Font.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetFont() => Font = null;

    /// <summary>
    ///  Resets the back color to be based on the parent's back color.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetImage() => Image = null;

    [EditorBrowsable(EditorBrowsableState.Never)]
    private void ResetImageTransparentColor() => ImageTransparentColor = Color.Empty;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetMargin()
    {
        _state[s_stateUseAmbientMargin] = true;
        SetAmbientMargin();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetPadding()
    {
        CommonProperties.ResetPadding(this);
    }

    /// <summary>
    ///  Resets the RightToLeft to be the default.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetRightToLeft() => RightToLeft = RightToLeft.Inherit;

    /// <summary>
    ///  Resets the TextDirection to be the default.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ResetTextDirection() => TextDirection = ToolStripTextDirection.Inherit;

    /// <summary>
    ///  Translates a point from one coordinate system to another
    /// </summary>
    internal Point TranslatePoint(Point fromPoint, ToolStripPointType fromPointType, ToolStripPointType toPointType)
    {
        ToolStrip? parent = ParentInternal;

        parent ??= (IsOnOverflow && Owner is not null) ? Owner.OverflowButton.DropDown : Owner;

        if (parent is null)
        {
            // should not throw here as it's an internal function call.
            return fromPoint;
        }

        if (fromPointType == toPointType)
        {
            return fromPoint;
        }

        Point toPoint;
        Point currentToolStripItemLocation = Bounds.Location;

        // From: Screen
        // To:      ToolStrip or ToolStripItem
        if (fromPointType == ToolStripPointType.ScreenCoords)
        {
            // Convert ScreenCoords --> ToolStripCoords
            toPoint = parent.PointToClient(fromPoint);

            // Convert ToolStripCoords --> ToolStripItemCoords
            if (toPointType == ToolStripPointType.ToolStripItemCoords)
            {
                toPoint.X += currentToolStripItemLocation.X;
                toPoint.Y += currentToolStripItemLocation.Y;
            }
        }

        // From: ToolStrip or ToolStripItem
        // To:      Screen or ToolStripItem
        else
        {
            // Convert "fromPoint" ToolStripItemCoords --> ToolStripCoords
            if (fromPointType == ToolStripPointType.ToolStripItemCoords)
            {
                fromPoint.X += currentToolStripItemLocation.X;
                fromPoint.Y += currentToolStripItemLocation.Y;
            }

            // At this point, fromPoint is now in ToolStrip coordinates.

            // Convert ToolStripCoords --> ScreenCoords
            if (toPointType == ToolStripPointType.ScreenCoords)
            {
                toPoint = parent.PointToScreen(fromPoint);
            }

            // Convert ToolStripCoords --> ToolStripItemCoords
            else if (toPointType == ToolStripPointType.ToolStripItemCoords)
            {
                fromPoint.X -= currentToolStripItemLocation.X;
                fromPoint.Y -= currentToolStripItemLocation.Y;
                toPoint = fromPoint;
            }
            else
            {
                Debug.Assert((toPointType == ToolStripPointType.ToolStripCoords), "why are we here! - investigate");
                toPoint = fromPoint;
            }
        }

        return toPoint;
    }

    internal static bool GetIsOffscreenPropertyValue(ToolStripItemPlacement? toolStripItemPlacement, Rectangle bounds)
    {
        return toolStripItemPlacement != ToolStripItemPlacement.Main || bounds.Height <= 0 || bounds.Width <= 0;
    }

    internal ToolStrip? RootToolStrip
    {
        get
        {
            ToolStripItem item = this;
            while (item.OwnerItem is not null)
            {
                item = item.OwnerItem;
            }

            return item.ParentInternal;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            return Text;
        }

        return base.ToString();
    }

    /// <summary>
    ///  Removes selection bits from item state
    /// </summary>
    internal void Unselect()
    {
        if (_state[s_stateSelected])
        {
            _state[s_stateSelected] = false;
            if (Available)
            {
                Invalidate();
                ParentInternal?.NotifySelectionChange(this);

                KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
            }
        }
    }

    bool IKeyboardToolTip.CanShowToolTipsNow()
        => Visible && _parent is not null && ((IKeyboardToolTip)_parent).AllowsChildrenToShowToolTips();

    Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => AccessibilityObject.Bounds;

    IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
    {
        List<Rectangle> neighbors = new(3);
        if (_parent is not null)
        {
            ToolStripItemCollection items = _parent.DisplayedItems;
            int i = 0, count = items.Count;
            bool found = false;
            while (!found && i < count)
            {
                found = ReferenceEquals(items[i], this);
                if (found)
                {
                    int previousIndex = i - 1;
                    if (previousIndex >= 0)
                    {
                        neighbors.Add(((IKeyboardToolTip)items[previousIndex]).GetNativeScreenRectangle());
                    }

                    int nextIndex = i + 1;
                    if (nextIndex < count)
                    {
                        neighbors.Add(((IKeyboardToolTip)items[nextIndex]).GetNativeScreenRectangle());
                    }
                }
                else
                {
                    i++;
                }
            }

            Debug.Assert(i < count, "Item has a parent set but the parent doesn't own the item");
        }

        if (_parent is ToolStripDropDown dropDown && dropDown.OwnerItem is not null)
        {
            neighbors.Add(((IKeyboardToolTip)dropDown.OwnerItem).GetNativeScreenRectangle());
        }

        return neighbors;
    }

    bool IKeyboardToolTip.IsHoveredWithMouse()
        => ((IKeyboardToolTip)this).GetNativeScreenRectangle().Contains(Control.MousePosition);

    bool IKeyboardToolTip.HasRtlModeEnabled()
        => _parent is not null && ((IKeyboardToolTip)_parent).HasRtlModeEnabled();

    bool IKeyboardToolTip.AllowsToolTip() => true;

    IWin32Window IKeyboardToolTip.GetOwnerWindow()
    {
        Debug.Assert(ParentInternal is not null, "Tool Strip Item Parent is null");
        return ParentInternal;
    }

    void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

    void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

    string? IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) => ToolTipText;

    bool IKeyboardToolTip.ShowsOwnToolTip() => true;

    bool IKeyboardToolTip.IsBeingTabbedTo() => IsBeingTabbedTo();

    bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => true;

    internal virtual void OnKeyboardToolTipHook(ToolTip toolTip)
    {
    }

    internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip)
    {
    }

    /// <summary>
    ///  Indicates whether or not the parent of this item has an accessible object associated with it.
    /// </summary>
    internal bool IsParentAccessibilityObjectCreated => ParentInternal is not null && ParentInternal.IsAccessibilityObjectCreated;

    internal virtual bool IsBeingTabbedTo() => Control.AreCommonNavigationalKeysDown();

    /// <summary>
    ///  Query font from property bag.
    /// </summary>
    internal bool TryGetExplicitlySetFont([NotNullWhen(true)] out Font? local) => Properties.TryGetValue(s_fontProperty, out local);
}
