// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms.Layout;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    [DesignTimeVisible(false)]
    [Designer("System.Windows.Forms.Design.ToolStripItemDesigner, " + AssemblyRef.SystemDesign)]
    [DefaultEvent(nameof(Click))]
    [ToolboxItem(false)]
    [DefaultProperty(nameof(Text))]
    public abstract partial class ToolStripItem : Component,
                              IDropTarget,
                              ISupportOleDropSource,
                              IArrangedElement,
                              IKeyboardToolTip
    {
#if DEBUG
        internal static readonly TraceSwitch s_mouseDebugging = new TraceSwitch("MouseDebugging", "Debug ToolStripItem mouse debugging code");
#else
        internal static readonly TraceSwitch s_mouseDebugging;
#endif

        private Rectangle _bounds = Rectangle.Empty;
        private PropertyStore _propertyStore;
        private ToolStripItemAlignment _alignment = ToolStripItemAlignment.Left;
        private ToolStrip _parent;
        private ToolStrip _owner;
        private ToolStripItemOverflow _overflow = ToolStripItemOverflow.AsNeeded;
        private ToolStripItemPlacement _placement = ToolStripItemPlacement.None;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleCenter;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
        private ToolStripItemImageIndexer _imageIndexer;
        private ToolStripItemInternalLayout _toolStripItemInternalLayout;
        private BitVector32 _state;
        private string _toolTipText;
        private Color _imageTransparentColor = Color.Empty;
        private ToolStripItemImageScaling _imageScaling = ToolStripItemImageScaling.SizeToFit;
        private Size _cachedTextSize;

        private static readonly Padding s_defaultMargin = new Padding(0, 1, 0, 2);
        private static readonly Padding s_defaultStatusStripMargin = new Padding(0, 2, 0, 0);
        private Padding _scaledDefaultMargin = s_defaultMargin;
        private Padding _scaledDefaultStatusStripMargin = s_defaultStatusStripMargin;

        private ToolStripItemDisplayStyle _displayStyle = ToolStripItemDisplayStyle.ImageAndText;

        private static readonly ArrangedElementCollection s_emptyChildCollection = new ArrangedElementCollection();

        internal static readonly object s_mouseDownEvent = new object();
        internal static readonly object s_mouseEnterEvent = new object();
        internal static readonly object s_mouseLeaveEvent = new object();
        internal static readonly object s_mouseHoverEvent = new object();
        internal static readonly object s_mouseMoveEvent = new object();
        internal static readonly object s_mouseUpEvent = new object();
        internal static readonly object s_clickEvent = new object();
        internal static readonly object s_doubleClickEvent = new object();
        internal static readonly object s_dragDropEvent = new object();
        internal static readonly object s_dragEnterEvent = new object();
        internal static readonly object s_dragLeaveEvent = new object();
        internal static readonly object s_dragOverEvent = new object();
        internal static readonly object s_displayStyleChangedEvent = new object();
        internal static readonly object s_enabledChangedEvent = new object();
        internal static readonly object s_internalEnabledChangedEvent = new object();
        internal static readonly object s_fontChangedEvent = new object();
        internal static readonly object s_foreColorChangedEvent = new object();
        internal static readonly object s_backColorChangedEvent = new object();
        internal static readonly object s_giveFeedbackEvent = new object();
        internal static readonly object s_queryContinueDragEvent = new object();
        internal static readonly object s_queryAccessibilityHelpEvent = new object();
        internal static readonly object s_locationChangedEvent = new object();
        internal static readonly object s_rightToLeftChangedEvent = new object();
        internal static readonly object s_visibleChangedEvent = new object();
        internal static readonly object s_availableChangedEvent = new object();
        internal static readonly object s_ownerChangedEvent = new object();
        internal static readonly object s_paintEvent = new object();
        internal static readonly object s_textChangedEvent = new object();

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
        private static readonly int s_stateContstructing = BitVector32.CreateMask(s_stateSelected);
        private static readonly int s_stateDisposed = BitVector32.CreateMask(s_stateContstructing);
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
        private int _deviceDpi = DpiHelper.DeviceDpi;
        internal Font _defaultFont = ToolStripManager.DefaultFont;

        protected ToolStripItem()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(s_defaultMargin);
                _scaledDefaultStatusStripMargin = DpiHelper.LogicalToDeviceUnits(s_defaultStatusStripMargin);
            }

            _state[s_stateEnabled | s_stateAutoSize | s_stateVisible | s_stateContstructing | s_stateSupportsItemClick | s_stateInvalidMirroredImage | s_stateMouseDownAndUpMustBeInSameItem | s_stateUseAmbientMargin] = true;
            _state[s_stateAllowDrop | s_stateMouseDownAndNoDrag | s_stateSupportsRightClick | s_statePressed | s_stateSelected | s_stateDisposed | s_stateDoubleClickEnabled | s_stateRightToLeftAutoMirrorImage | s_stateSupportsSpaceKey] = false;
            SetAmbientMargin();
            Size = DefaultSize;
            DisplayStyle = DefaultDisplayStyle;
            CommonProperties.SetAutoSize(this, true);
            _state[s_stateContstructing] = false;
            AutoToolTip = DefaultAutoToolTip;
        }

        protected ToolStripItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, null)
        {
        }
        protected ToolStripItem(string text, Image image, EventHandler onClick, string name) : this()
        {
            Text = text;
            Image = image;
            if (onClick != null)
            {
                Click += onClick;
            }
            Name = name;
        }

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
                AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(s_accessibilityProperty);
                if (accessibleObject is null)
                {
                    accessibleObject = CreateAccessibilityInstance();
                    Properties.SetObject(s_accessibilityProperty, accessibleObject);
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
        public string AccessibleDefaultActionDescription
        {
            get => (string)Properties.GetObject(s_accessibleDefaultActionDescriptionProperty);
            set
            {
                Properties.SetObject(s_accessibleDefaultActionDescriptionProperty, value);
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
        public string AccessibleDescription
        {
            get => (string)Properties.GetObject(s_accessibleDescriptionProperty);
            set
            {
                Properties.SetObject(s_accessibleDescriptionProperty, value);
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
        public string AccessibleName
        {
            get => (string)Properties.GetObject(s_accessibleNameProperty);
            set
            {
                Properties.SetObject(s_accessibleNameProperty, value);
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
            get
            {
                int role = Properties.GetInteger(s_accessibleRoleProperty, out bool found);
                if (found)
                {
                    return (AccessibleRole)role;
                }

                return AccessibleRole.Default;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AccessibleRole.Default, (int)AccessibleRole.OutlineButton))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AccessibleRole));
                }
                Properties.SetInteger(s_accessibleRoleProperty, (int)value);
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemAlignment.Left, (int)ToolStripItemAlignment.Right))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemAlignment));
                }

                if (_alignment != value)
                {
                    _alignment = value;

                    if (ParentInternal != null && ParentInternal.IsHandleCreated)
                    {
                        ParentInternal.PerformLayout();
                    }
                }
            }
        }

        /// <summary>
        ///  Determines if this item can be dragged.
        ///  This is EXACTLY like Control.AllowDrop - setting this to true WILL call
        ///  the droptarget handlers. The ToolStripDropTargetManager is the one that
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
        public event EventHandler AvailableChanged
        {
            add => Events.AddHandler(s_availableChangedEvent, value);
            remove => Events.RemoveHandler(s_availableChangedEvent, value);
        }

        /// <summary>
        ///  Gets or sets the image that is displayed on a <see cref='Label'/>.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripItemImageDescr))]
        [DefaultValue(null)]
        public virtual Image BackgroundImage
        {
            get => Properties.GetObject(s_backgroundImageProperty) as Image;
            set
            {
                if (BackgroundImage != value)
                {
                    Properties.SetObject(s_backgroundImageProperty, value);
                    Invalidate();
                }
            }
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
            get
            {
                bool found = Properties.ContainsObject(s_backgroundImageLayoutProperty);
                if (!found)
                {
                    return ImageLayout.Tile;
                }

                return ((ImageLayout)Properties.GetObject(s_backgroundImageLayoutProperty));
            }
            set
            {
                if (BackgroundImageLayout != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ImageLayout.None, (int)ImageLayout.Zoom))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ImageLayout));
                    }

                    Properties.SetObject(s_backgroundImageLayoutProperty, value);
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

                Control p = ParentInternal;
                if (p != null)
                {
                    return p.BackColor;
                }

                return Control.DefaultBackColor;
            }
            set
            {
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
        [SRDescription(nameof(SR.ToolStripItemOnBackColorChangedDescr))]
        public event EventHandler BackColorChanged
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
        /// Zero-based rectangle, same concept as ClientRect
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
        public virtual bool CanSelect => true;

        /// <remarks>
        ///  Usually the same as can select, but things like the control box in an MDI window are exceptions
        /// </remarks>
        internal virtual bool CanKeyboardSelect => CanSelect;

        /// <summary>
        ///  Occurs when the control is clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ToolStripItemOnClickDescr))]
        public event EventHandler Click
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
                // since we dont support DefaultLayout go directly against the CommonProperties
                return CommonProperties.xGetAnchor(this);
            }
            set
            {
                if (value != Anchor)
                {
                    // since we dont support DefaultLayout go directly against the CommonProperties
                    CommonProperties.xSetAnchor(this, value);
                    if (ParentInternal != null)
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
                // since we dont support DefaultLayout go directly against the CommonProperties
                return CommonProperties.xGetDock(this);
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DockStyle.None, (int)DockStyle.Fill))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DockStyle));
                }

                if (value != Dock)
                {
                    // since we dont support DefaultLayout go directly against the CommonProperties
                    CommonProperties.xSetDock(this, value);
                    if (ParentInternal != null)
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
                if (Owner != null && Owner is StatusStrip)
                {
                    return _scaledDefaultStatusStripMargin;
                }

                return _scaledDefaultMargin;
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
            get => DpiHelper.IsPerMonitorV2Awareness ?
                    DpiHelper.LogicalToDeviceUnits(new Size(23, 23), DeviceDpi) :
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
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemDisplayStyle.None, (int)ToolStripItemDisplayStyle.ImageAndText))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemDisplayStyle));
                    }

                    _displayStyle = value;
                    if (!_state[s_stateContstructing])
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
        public event EventHandler DisplayStyleChanged
        {
            add => Events.AddHandler(s_displayStyleChangedEvent, value);
            remove => Events.RemoveHandler(s_displayStyleChangedEvent, value);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private RightToLeft DefaultRightToLeft => RightToLeft.Inherit;

        /// <summary>
        ///  Occurs when the control is double clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick
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
        public event DragEventHandler DragDrop
        {
            add => Events.AddHandler(s_dragDropEvent, value);
            remove => Events.RemoveHandler(s_dragDropEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop))]
        [SRDescription(nameof(SR.ToolStripItemOnDragEnterDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public event DragEventHandler DragEnter
        {
            add => Events.AddHandler(s_dragEnterEvent, value);
            remove => Events.RemoveHandler(s_dragEnterEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop))]
        [SRDescription(nameof(SR.ToolStripItemOnDragOverDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public event DragEventHandler DragOver
        {
            add => Events.AddHandler(s_dragOverEvent, value);
            remove => Events.RemoveHandler(s_dragOverEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop))]
        [SRDescription(nameof(SR.ToolStripItemOnDragLeaveDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public event EventHandler DragLeave
        {
            add => Events.AddHandler(s_dragLeaveEvent, value);
            remove => Events.RemoveHandler(s_dragLeaveEvent, value);
        }

        /// <summary>
        ///  This represents what we're actually going to drag. If the parent has set AllowItemReorder to true,
        ///  then the item should call back on the private OnQueryContinueDrag/OnGiveFeedback that is implemented
        ///  in the parent ToolStrip.
        ///
        ///  Else if the parent does not support reordering of items (Parent.AllowItemReorder = false) -
        ///  then call back on the ToolStripItem's OnQueryContinueDrag/OnGiveFeedback methods.
        /// </summary>
        private DropSource DropSource
        {
            get
            {
                if ((ParentInternal != null) && (ParentInternal.AllowItemReorder) && (ParentInternal.ItemReorderDropSource != null))
                {
                    return new DropSource(ParentInternal.ItemReorderDropSource);
                }

                return new DropSource(this);
            }
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
                if (Owner != null)
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
                        }
                    }

                    OnEnabledChanged(EventArgs.Empty);
                    Invalidate();
                }

                OnInternalEnabledChanged(EventArgs.Empty);
            }
        }

        [SRDescription(nameof(SR.ToolStripItemEnabledChangedDescr))]
        public event EventHandler EnabledChanged
        {
            add => Events.AddHandler(s_enabledChangedEvent, value);
            remove => Events.RemoveHandler(s_enabledChangedEvent, value);
        }

        internal event EventHandler InternalEnabledChanged
        {
            add => Events.AddHandler(s_internalEnabledChangedEvent, value);
            remove => Events.RemoveHandler(s_internalEnabledChangedEvent, value);
        }

        private void EnsureParentDropTargetRegistered()
        {
            if (ParentInternal != null)
            {
                ParentInternal.DropTargetManager.EnsureRegistered(this);
            }
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
                Color foreColor = Properties.GetColor(s_foreColorProperty);
                if (!foreColor.IsEmpty)
                {
                    return foreColor;
                }

                Control p = ParentInternal;
                if (p != null)
                {
                    return p.ForeColor;
                }

                return Control.DefaultForeColor;
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
        [SRDescription(nameof(SR.ToolStripItemOnForeColorChangedDescr))]
        public event EventHandler ForeColorChanged
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
        public virtual Font Font
        {
            get
            {
                Font font = (Font)Properties.GetObject(s_fontProperty);
                if (font != null)
                {
                    return font;
                }

                Font f = GetOwnerFont();
                if (f != null)
                {
                    return f;
                }

                return DpiHelper.IsPerMonitorV2Awareness ? _defaultFont : ToolStripManager.DefaultFont;
            }
            set
            {
                Font local = (Font)Properties.GetObject(s_fontProperty);
                if ((local != value))
                {
                    Properties.SetObject(s_fontProperty, value);
                    OnFontChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatDragDrop))]
        [SRDescription(nameof(SR.ToolStripItemOnGiveFeedbackDescr))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public event GiveFeedbackEventHandler GiveFeedback
        {
            add => Events.AddHandler(s_giveFeedbackEvent, value);
            remove => Events.RemoveHandler(s_giveFeedbackEvent, value);
        }

        /// <summary>
        ///  The height of this control
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
        IArrangedElement IArrangedElement.Container => ParentInternal ?? Owner;

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
            // in this case the parent is telling us to refresh our bounds - dont
            // call PerformLayout
            SetBounds(bounds);
        }

        void IArrangedElement.PerformLayout(IArrangedElement container, string propertyName)
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
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                if (_imageAlign != value)
                {
                    _imageAlign = value;
                    InvalidateItemLayout(PropertyNames.ImageAlign);
                }
            }
        }

        /// <summary>
        ///  Gets or sets the image that is displayed on a <see cref='Label'/>.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripItemImageDescr))]
        public virtual Image Image
        {
            get
            {
                Image image = (Image)Properties.GetObject(s_imageProperty);
                if (image is null && Owner?.ImageList != null && ImageIndexer.ActualIndex >= 0)
                {
                    bool disposing = _state[s_stateDisposing];
                    if (!disposing && ImageIndexer.ActualIndex < Owner.ImageList.Images.Count)
                    {
                        // CACHE (by design). If we fetched out of the image list every time it would dramatically hit perf.
                        image = Owner.ImageList.Images[ImageIndexer.ActualIndex];
                        _state[s_stateInvalidMirroredImage] = true;
                        Properties.SetObject(s_imageProperty, image);
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
                if (value != null)
                {
                    ImageIndex = ImageList.Indexer.DefaultIndex;
                }

                Properties.SetObject(s_imageProperty, value);
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
                if (_imageTransparentColor != value)
                {
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(false)]
        [RelatedImageList("Owner.ImageList")]
        public int ImageIndex
        {
            get
            {
                if ((Owner != null) && ImageIndexer.Index != ImageList.Indexer.DefaultIndex && Owner.ImageList != null && ImageIndexer.Index >= Owner.ImageList.Images.Count)
                {
                    return Owner.ImageList.Images.Count - 1;
                }

                return ImageIndexer.Index;
            }
            set
            {
                if (value < ImageList.Indexer.DefaultIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, ImageList.Indexer.DefaultIndex));
                }

                ImageIndexer.Index = value;
                _state[s_stateInvalidMirroredImage] = true;
                // Set the Image Property to null
                Properties.SetObject(s_imageProperty, null);

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
        [Editor("System.Windows.Forms.Design.ToolStripImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(false)]
        [RelatedImageList("Owner.ImageList")]
        public string ImageKey
        {
            get => ImageIndexer.Key;
            set
            {
                ImageIndexer.Key = value;
                _state[s_stateInvalidMirroredImage] = true;
                Properties.SetObject(s_imageProperty, null);

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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemImageScaling.None, (int)ToolStripItemImageScaling.SizeToFit))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemImageScaling));
                }

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
                Color color = Properties.GetColor(s_foreColorProperty);
                if (!color.IsEmpty)
                {
                    return true;
                }

                Control parent = ParentInternal;
                if (parent != null)
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
                if (ParentInternal != null)
                {
                    return ParentInternal.IsDropDown;
                }
                else if (Owner != null && Owner.IsDropDown)
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
        public event EventHandler LocationChanged
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
            get
            {
                int action = Properties.GetInteger(s_mergeActionProperty, out bool found);
                if (found)
                {
                    return (MergeAction)action;
                }

                return MergeAction.Append;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)MergeAction.Append, (int)MergeAction.MatchOnly))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(MergeAction));
                }

                Properties.SetInteger(s_mergeActionProperty, (int)value);
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
            get
            {
                int index = Properties.GetInteger(s_mergeIndexProperty, out bool found);
                if (found)
                {
                    return index;
                }

                return -1;
            }
            set => Properties.SetInteger(s_mergeIndexProperty, value);
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
        public event MouseEventHandler MouseDown
        {
            add => Events.AddHandler(s_mouseDownEvent, value);
            remove => Events.RemoveHandler(s_mouseDownEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer enters the control.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ToolStripItemOnMouseEnterDescr))]
        public event EventHandler MouseEnter
        {
            add => Events.AddHandler(s_mouseEnterEvent, value);
            remove => Events.RemoveHandler(s_mouseEnterEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer leaves the control.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ToolStripItemOnMouseLeaveDescr))]
        public event EventHandler MouseLeave
        {
            add => Events.AddHandler(s_mouseLeaveEvent, value);
            remove => Events.RemoveHandler(s_mouseLeaveEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer hovers over the contro.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ToolStripItemOnMouseHoverDescr))]
        public event EventHandler MouseHover
        {
            add => Events.AddHandler(s_mouseHoverEvent, value);
            remove => Events.RemoveHandler(s_mouseHoverEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is moved over the control.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ToolStripItemOnMouseMoveDescr))]
        public event MouseEventHandler MouseMove
        {
            add => Events.AddHandler(s_mouseMoveEvent, value);
            remove => Events.RemoveHandler(s_mouseMoveEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ToolStripItemOnMouseUpDescr))]
        public event MouseEventHandler MouseUp
        {
            add => Events.AddHandler(s_mouseUpEvent, value);
            remove => Events.RemoveHandler(s_mouseUpEvent, value);
        }

        /// <summary>
        ///  Name of this control. The designer will set this to the same
        ///  as the programatic Id "(name)" of the control. The name can be
        ///  used as a key into the ControlCollection.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public string Name
        {
            get => WindowsFormsUtils.GetComponentName(this, (string)Properties.GetObject(ToolStripItem.s_nameProperty));
            set
            {
                if (DesignMode)
                {
                    return;
                }

                Properties.SetObject(ToolStripItem.s_nameProperty, value);
            }
        }

        /// <summary>
        ///  The owner of this ToolStripItem. The owner is essentially a backpointer to
        ///  the ToolStrip who contains this item in it's item collection. Handy for getting
        ///  to things such as the ImageList, which would be defined on the ToolStrip.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStrip Owner
        {
            get => _owner;
            set
            {
                if (_owner != value)
                {
                    if (_owner != null)
                    {
                        _owner.Items.Remove(this);
                    }
                    if (value != null)
                    {
                        value.Items.Add(this);
                    }
                }
            }
        }

        /// <summary>
        ///  Returns the "parent" item on the preceeding menu which has spawned this item.
        ///  e.g. File->Open  the OwnerItem of Open is File.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripItem OwnerItem
        {
            get
            {
                ToolStripDropDown currentParent = null;
                if (ParentInternal != null)
                {
                    currentParent = ParentInternal as ToolStripDropDown;
                }
                else if (Owner != null)
                {
                    // parent may be null, but we may be "owned" by a collection.
                    currentParent = Owner as ToolStripDropDown;
                }

                if (currentParent != null)
                {
                    return currentParent.OwnerItem;
                }

                return null;
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolStripItemOwnerChangedDescr))]
        public event EventHandler OwnerChanged
        {
            add => Events.AddHandler(s_ownerChangedEvent, value);
            remove => Events.RemoveHandler(s_ownerChangedEvent, value);
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ToolStripItemOnPaintDescr))]
        public event PaintEventHandler Paint
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
        protected internal ToolStrip Parent
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemOverflow.Never, (int)ToolStripItemOverflow.AsNeeded))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripGripStyle));
                }

                if (_overflow != value)
                {
                    _overflow = value;
                    if (Owner != null)
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
        ///  This is explicitly a ToolStrip, because only ToolStrips know how to manage ToolStripitems
        /// </summary>
        internal ToolStrip ParentInternal
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    ToolStrip oldParent = _parent;
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

                Image image = (Image)Properties.GetObject(s_imageProperty);
                bool usingImageList = ((Owner != null) && (Owner.ImageList != null) && (ImageIndexer.ActualIndex >= 0));

                if (ImageScaling == ToolStripItemImageScaling.SizeToFit)
                {
                    ToolStrip ownerToolStrip = Owner;
                    if (ownerToolStrip != null && (image != null || usingImageList))
                    {
                        return ownerToolStrip.ImageScalingSize;
                    }
                }

                Size imageSize = Size.Empty;
                if (usingImageList)
                {
                    imageSize = Owner.ImageList.ImageSize;
                }
                else
                {
                    imageSize = (image is null) ? Size.Empty : image.Size;
                }

                return imageSize;
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
        public event QueryContinueDragEventHandler QueryContinueDrag
        {
            add => Events.AddHandler(s_queryContinueDragEvent, value);
            remove => Events.RemoveHandler(s_queryContinueDragEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolStripItemOnQueryAccessibilityHelpDescr))]
        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add => Events.AddHandler(s_queryAccessibilityHelpEvent, value);
            remove => Events.RemoveHandler(s_queryAccessibilityHelpEvent, value);
        }

        /// <summary>
        ///  Returns the value of the backColor field -- no asking the parent with its color is, etc.
        /// </summary>
        internal Color RawBackColor => Properties.GetColor(s_backColorProperty);

        internal ToolStripRenderer Renderer
        {
            get
            {
                if (Owner != null)
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
                int rightToLeft = Properties.GetInteger(s_rightToLeftProperty, out bool found);
                if (!found)
                {
                    rightToLeft = (int)RightToLeft.Inherit;
                }

                if (((RightToLeft)rightToLeft) == RightToLeft.Inherit)
                {
                    if (Owner != null)
                    {
                        rightToLeft = (int)Owner.RightToLeft;
                    }
                    else if (ParentInternal != null)
                    {
                        // case for Overflow & Grip
                        rightToLeft = (int)ParentInternal.RightToLeft;
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)RightToLeft.No, (int)RightToLeft.Inherit))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(RightToLeft));
                }

                RightToLeft oldValue = RightToLeft;

                if (Properties.ContainsInteger(s_rightToLeftProperty) || value != RightToLeft.Inherit)
                {
                    Properties.SetInteger(s_rightToLeftProperty, (int)value);
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

        internal Image MirroredImage
        {
            get
            {
                if (_state[s_stateInvalidMirroredImage])
                {
                    Image image = Image;
                    if (image is null)
                    {
                        return null;
                    }

                    Image mirroredImage = image.Clone() as Image;
                    mirroredImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

                    Properties.SetObject(s_mirroredImageProperty, mirroredImage);
                    _state[s_stateInvalidMirroredImage] = false;
                    return mirroredImage;
                }

                return Properties.GetObject(s_mirroredImageProperty) as Image;
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ToolStripItemOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged
        {
            add => Events.AddHandler(s_rightToLeftChangedEvent, value);
            remove => Events.RemoveHandler(s_rightToLeftChangedEvent, value);
        }

        /// <summary>
        ///  if the item is selected we return true.
        ///
        ///  FAQ: Why dont we have a Hot or MouseIsOver property?
        ///  After going through the scenarios, we've decided NOT to add a separate MouseIsOver or Hot flag to ToolStripItem. The thing to use is 'Selected'.
        ///  Why?  While the selected thing can be different than the moused over item, the selected item is ALWAYS the one you want to paint differently
        ///
        ///  Scenario 1:  Keyboard select an item then select a different item with the mouse.
        ///  -          Do Alt+F to expand your File menu, keyboard down several items.
        ///  -          Mouse over a different item
        ///  -          Notice how two things are never painted hot at the same time, and how the selection changes from the keyboard selected item to the one selected with the mouse. In  this case the selection should move with the mouse selection.
        ///  -          Notice how if you hit enter when the mouse is over it, it executes the item. That's selection.
        ///  Scenario 2: Put focus into a combo box, then mouse over a different item
        ///  -          Notice how all the other items you mouse over do not change the way they are painted, if you hit enter, that goes to the combobox, rather than executing the current item.
        ///
        ///  At first look "MouseIsOver" or "Hot" seems to be the thing people want, but its almost never the desired behavior. A unified selection model is simpler and seems to meet the scenarios.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selected
            => CanSelect && !DesignMode && (_state[s_stateSelected] ||
                (ParentInternal != null && ParentInternal.IsSelectionSuspended &&
                 ParentInternal.LastMouseDownedItem == this));

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
        public object Tag
        {
            get
            {
                if (Properties.ContainsObject(ToolStripItem.s_tagProperty))
                {
                    return _propertyStore.GetObject(ToolStripItem.s_tagProperty);
                }

                return null;
            }
            set => Properties.SetObject(ToolStripItem.s_tagProperty, value);
        }

        /// <summary>
        ///  The text of the item
        /// </summary>
        [DefaultValue("")]
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [SRDescription(nameof(SR.ToolStripItemTextDescr))]
        public virtual string Text
        {
            get
            {
                if (Properties.ContainsObject(ToolStripItem.s_textProperty))
                {
                    return (string)Properties.GetObject(ToolStripItem.s_textProperty);
                }

                return string.Empty;
            }
            set
            {
                if (value != Text)
                {
                    Properties.SetObject(ToolStripItem.s_textProperty, value);
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
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                if (_textAlign != value)
                {
                    _textAlign = value;
                    InvalidateItemLayout(PropertyNames.TextAlign);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ToolStripItemOnTextChangedDescr))]
        public event EventHandler TextChanged
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
                ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
                if (Properties.ContainsObject(ToolStripItem.s_textDirectionProperty))
                {
                    textDirection = (ToolStripTextDirection)Properties.GetObject(ToolStripItem.s_textDirectionProperty);
                }

                if (textDirection == ToolStripTextDirection.Inherit)
                {
                    if (ParentInternal != null)
                    {
                        // in the case we're on a ToolStripOverflow
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripTextDirection.Inherit, (int)ToolStripTextDirection.Vertical270))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripTextDirection));
                }

                Properties.SetObject(ToolStripItem.s_textDirectionProperty, value);
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TextImageRelation.Overlay, (int)TextImageRelation.TextBeforeImage, 1))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TextImageRelation));
                }

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
        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        public string ToolTipText
        {
            get
            {
                if (AutoToolTip && string.IsNullOrEmpty(_toolTipText))
                {
                    string toolText = Text;
                    if (WindowsFormsUtils.ContainsMnemonic(toolText))
                    {
                        // this shouldnt be called a lot so we can take the perf hit here.
                        toolText = string.Join("", toolText.Split('&'));
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
            get => (ParentInternal != null) && (ParentInternal.Visible) && Available;
            set => SetVisibleCore(value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ToolStripItemOnVisibleChangedDescr))]
        public event EventHandler VisibleChanged
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
            if (ParentInternal != null)
            {
                int index = ParentInternal.DisplayedItems.IndexOf(this);
                ParentInternal.AccessibilityNotifyClients(accEvent, index);
            }
        }

        private void Animate()
            => Animate(!DesignMode && Visible && Enabled && ParentInternal != null);

        private void StopAnimate() => Animate(false);

        private void Animate(bool animate)
        {
            if (animate == _state[s_stateCurrentlyAnimatingImage])
            {
                return;
            }

            Image image = Image;
            if (image is null)
            {
                return;
            }

            if (animate)
            {
                ImageAnimator.Animate(image, new EventHandler(OnAnimationFrameChanged));
            }
            else
            {
                ImageAnimator.StopAnimate(image, new EventHandler(OnAnimationFrameChanged));
            }

            _state[s_stateCurrentlyAnimatingImage] = animate;
        }

        internal bool BeginDragForItemReorder()
        {
            if (Control.ModifierKeys == Keys.Alt)
            {
                if (ParentInternal.Items.Contains(this) && ParentInternal.AllowItemReorder)
                {
                    // we only drag
                    ToolStripItem item = this as ToolStripItem;
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
        protected virtual AccessibleObject CreateAccessibilityInstance()
            => new ToolStripItemAccessibleObject(this);

        /// <summary>
        ///  Creates an instance of the object that defines how image and text
        ///  gets laid out in the ToolStripItem
        /// </summary>
        private protected virtual ToolStripItemInternalLayout CreateInternalLayout()
            => new ToolStripItemInternalLayout(this);

        /// <summary>
        ///  Disposes this ToolStrip item.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _state[s_stateDisposing] = true;

                if (Owner != null)
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
                Properties.SetObject(s_mirroredImageProperty, null);
                Properties.SetObject(s_imageProperty, null);
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
            Ole32.IDropSource dropSource = DropSource;
            IComDataObject dataObject = null;

            dataObject = data as IComDataObject;
            if (dataObject is null)
            {
                DataObject iwdata = null;
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
                dataObject = (IComDataObject)iwdata;
            }

            HRESULT hr = Ole32.DoDragDrop(dataObject, dropSource, (Ole32.DROPEFFECT)allowedEffects, out Ole32.DROPEFFECT finalEffect);
            if (!hr.Succeeded())
            {
                return DragDropEffects.None;
            }

            return (DragDropEffects)finalEffect;
        }

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
                    HandlePaint(e as PaintEventArgs);
                    break;
                case ToolStripItemEventType.MouseHover:
                    // disabled toolstrip items should show tooltips.
                    // we wont raise mouse events though.
                    if (!Enabled && ParentInternal != null && !string.IsNullOrEmpty(ToolTipText))
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
                    // we wont raise mouse events though.
                    if (!Enabled && ParentInternal != null)
                    {
                        ParentInternal.UpdateToolTip(null);
                    }
                    else
                    {
                        HandleMouseLeave(e);
                    }
                    break;
                case ToolStripItemEventType.MouseMove:
                    // Disabled items typically dont get mouse move
                    // but they should be allowed to re-order if the ALT key is pressed
                    if (!Enabled && ParentInternal != null)
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
                        HandleMouseMove(e as MouseEventArgs);
                        break;
                    case ToolStripItemEventType.MouseHover:
                        HandleMouseHover(e as EventArgs);
                        break;
                    case ToolStripItemEventType.MouseUp:
                        HandleMouseUp(e as MouseEventArgs);
                        break;
                    case ToolStripItemEventType.MouseDown:
                        HandleMouseDown(e as MouseEventArgs);
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

        private Font GetOwnerFont() => Owner?.Font;

        /// <summary>
        ///  We dont want a public settable property and usually owner will work
        ///  except for things like the overflow button
        /// </summary>
        public ToolStrip GetCurrentParent() => Parent;

        internal ToolStripDropDown GetCurrentParentDropDown()
        {
            if (ParentInternal != null)
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

            if (Owner != null)
            {
                LayoutTransaction.DoLayout(Owner, this, affectedProperty);
                Owner.Invalidate();
            }
        }

        internal void InvalidateImageListImage()
        {
            // invalidate the cache.
            if (ImageIndexer.ActualIndex >= 0)
            {
                Properties.SetObject(s_imageProperty, null);
                InvalidateItemLayout(PropertyNames.Image);
            }
        }

        internal void InvokePaint() => ParentInternal?.InvokePaintItem(this);

        protected internal virtual bool IsInputKey(Keys keyData) => false;

        protected internal virtual bool IsInputChar(char charCode) => false;

        private void HandleClick(EventArgs e)
        {
            Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] HandleClick");

            try
            {
                if (!DesignMode)
                {
                    _state[s_statePressed] = true;
                }
                // force painting w/o using message loop here because it may be quite a long
                // time before it gets pumped again.
                InvokePaint();

                if (SupportsItemClick && Owner != null)
                {
                    Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] HandleItemClick");
                    Owner.HandleItemClick(this);
                }

                OnClick(e);

                if (SupportsItemClick && Owner != null)
                {
                    Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] HandleItemClicked");
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
                if (ParentInternal != null
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
                            if (User32.GetKeyState((int)Keys.LButton) < 0)
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
                Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] MouseEnter");
                RaiseEvent(s_mouseEnterEvent, e);
            }
        }

        private void HandleMouseMove(MouseEventArgs mea)
        {
            Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] MouseMove");

            if (Enabled && CanSelect && !Selected)
            {
                if (ParentInternal != null
                     && ParentInternal.CanHotTrack
                     && ParentInternal.ShouldSelectItem())
                {
                    // this is the case where we got a mouse enter, but ShouldSelectItem
                    // returned false.
                    // typically occus when a window first opens - we get a mouse enter on the item
                    // the cursor is hovering over - but we dont actually want to change selection to it.
                    Select();
                }
            }
            OnMouseMove(mea);
            RaiseMouseEvent(s_mouseMoveEvent, mea);
        }

        private void HandleMouseHover(EventArgs e)
        {
            Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] MouseHover");
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
            Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] MouseLeave");
            HandleLeave();
            if (Enabled)
            {
                OnMouseLeave(e);
                RaiseEvent(s_mouseLeaveEvent, e);
            }
        }
        private void HandleMouseDown(MouseEventArgs e)
        {
            Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] MouseDown");

            _state[s_stateMouseDownAndNoDrag] = !BeginDragForItemReorder();
            if (_state[s_stateMouseDownAndNoDrag])
            {
                if (e.Button == MouseButtons.Left)
                {
                    Push(true);
                }
                //
                OnMouseDown(e);
                RaiseMouseEvent(s_mouseDownEvent, e);
            }
        }
        private void HandleMouseUp(MouseEventArgs e)
        {
            Debug.WriteLineIf(s_mouseDebugging.TraceVerbose, "[" + Text + "] MouseUp");

            bool fireMouseUp = (ParentInternal.LastMouseDownedItem == this);

            if (!fireMouseUp && !MouseDownAndUpMustBeInSameItem)
            {
                // in the case of menus, you can mouse down on one item and mouse up
                // on another. We do need to be careful
                // that the mouse has actually moved from when a dropdown has been opened -
                // otherwise we may accidentally click what's underneath the mouse at the time
                // the dropdown is opened.
                fireMouseUp = ParentInternal.ShouldSelectItem();
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

        protected virtual void OnClick(EventArgs e) => RaiseEvent(s_clickEvent, e);

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

        private void OnAnimationFrameChanged(object o, EventArgs e)
        {
            ToolStrip parent = ParentInternal;
            if (parent != null)
            {
                if (parent.Disposing || parent.IsDisposed)
                {
                    return;
                }

                if (parent.IsHandleCreated && parent.InvokeRequired)
                {
                    parent.BeginInvoke(new EventHandler(OnAnimationFrameChanged), new object[] { o, e });
                    return;
                }

                Invalidate();
            }
        }

        protected virtual void OnAvailableChanged(EventArgs e) => RaiseEvent(s_availableChangedEvent, e);

        /// <summary>
        ///  Raises the <see cref='ToolStripItem.DragEnter'/> event.
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
            => ((GiveFeedbackEventHandler)Events[s_giveFeedbackEvent])?.Invoke(this, giveFeedbackEvent);

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
            if (ParentInternal != null && !string.IsNullOrEmpty(ToolTipText))
            {
                ParentInternal.UpdateToolTip(this);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected virtual void OnMouseLeave(EventArgs e) => ParentInternal?.UpdateToolTip(null);

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
            Color backColor = Properties.GetColor(s_backColorProperty);
            if (backColor.IsEmpty)
            {
                OnBackColorChanged(e);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected virtual void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            SetAmbientMargin();
            if ((oldParent != null) && (oldParent.DropTargetManager != null))
            {
                oldParent.DropTargetManager.EnsureUnRegistered(this);
            }
            if (AllowDrop && (newParent != null))
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
            if (Properties.GetObject(s_fontProperty) is null)
            {
                OnFontChanged(e);
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
        protected internal virtual void OnParentRightToLeftChanged(EventArgs e)
        {
            if (!Properties.ContainsInteger(s_rightToLeftProperty) || ((RightToLeft)Properties.GetInteger(s_rightToLeftProperty)) == RightToLeft.Inherit)
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
            if (Owner != null)
            {
                // check if we need to fire OnRightToLeftChanged
                int rightToLeft = Properties.GetInteger(s_rightToLeftProperty, out bool found);
                if (!found)
                {
                    rightToLeft = (int)RightToLeft.Inherit;
                }
                if ((rightToLeft == (int)RightToLeft.Inherit) && RightToLeft != DefaultRightToLeft)
                {
                    OnRightToLeftChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal void OnOwnerTextDirectionChanged()
        {
            ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
            if (Properties.ContainsObject(ToolStripItem.s_textDirectionProperty))
            {
                textDirection = (ToolStripTextDirection)Properties.GetObject(ToolStripItem.s_textDirectionProperty);
            }

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
            if (Owner != null && !(Owner.IsDisposed || Owner.Disposing))
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
                if (ParentInternal != null && !ParentInternal.IsDropDown && Enabled)
                {
                    ParentInternal.RestoreFocusInternal();
                }
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

        internal void RaiseCancelEvent(object key, CancelEventArgs e)
            => ((CancelEventHandler)Events[key])?.Invoke(this, e);

        internal void RaiseDragEvent(object key, DragEventArgs e)
            => ((DragEventHandler)Events[key])?.Invoke(this, e);

        internal void RaiseEvent(object key, EventArgs e)
            => ((EventHandler)Events[key])?.Invoke(this, e);

        internal void RaiseKeyEvent(object key, KeyEventArgs e)
            => ((KeyEventHandler)Events[key])?.Invoke(this, e);

        internal void RaiseKeyPressEvent(object key, KeyPressEventArgs e)
            => ((KeyPressEventHandler)Events[key])?.Invoke(this, e);

        internal void RaiseMouseEvent(object key, MouseEventArgs e)
            => ((MouseEventHandler)Events[key])?.Invoke(this, e);

        internal void RaisePaintEvent(object key, PaintEventArgs e)
            => ((PaintEventHandler)Events[key])?.Invoke(this, e);

        internal void RaiseQueryContinueDragEvent(object key, QueryContinueDragEventArgs e)
            => ((QueryContinueDragEventHandler)Events[key])?.Invoke(this, e);

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
            _scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(s_defaultMargin, _deviceDpi);
            _scaledDefaultStatusStripMargin = DpiHelper.LogicalToDeviceUnits(s_defaultStatusStripMargin, _deviceDpi);
        }

        public void Select()
        {
#if DEBUG
            // let's not snap the stack trace unless we're debugging selection.
            if (ToolStrip.s_selectionDebug.TraceVerbose)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "[Selection DBG] WBI.Select: {0} \r\n{1}\r\n", ToString(), new StackTrace().ToString().Substring(0, 200)));
            }
#endif
            if (!CanSelect)
            {
                return;
            }

            if (Owner != null && Owner.IsCurrentlyDragging)
            {
                // make sure we dont select during a drag operation.
                return;
            }
            if (ParentInternal != null && ParentInternal.IsSelectionSuspended)
            {
                Debug.WriteLineIf(ToolStrip.s_selectionDebug.TraceVerbose, "[Selection DBG] BAILING, selection is currently suspended");
                return;
            }

            if (!Selected)
            {
                _state[s_stateSelected] = true;
                if (ParentInternal != null)
                {
                    ParentInternal.NotifySelectionChange(this);
                    Debug.Assert(_state[s_stateSelected], "calling notify selection change changed the selection state of this item");
                }
                if (IsOnDropDown)
                {
                    if (OwnerItem != null && OwnerItem.IsOnDropDown)
                    {
                        // ensure the selection is moved back to our owner item.
                        OwnerItem.Select();
                    }
                }

                KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(this);

                if (AccessibilityObject is ToolStripItemAccessibleObject accessibleObject)
                {
                    accessibleObject.RaiseFocusChanged();
                }
            }
        }

        internal void SetOwner(ToolStrip newOwner)
        {
            if (_owner != newOwner)
            {
                Font f = this.Font;

                if (_owner != null)
                {
                    _owner._rescaleConstsCallbackDelegate -= ToolStrip_RescaleConstants;
                }
                _owner = newOwner;

                if (_owner != null)
                {
                    _owner._rescaleConstsCallbackDelegate += ToolStrip_RescaleConstants;
                }

                // clear the parent if the owner is null.
                if (newOwner is null)
                {
                    this.ParentInternal = null;
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

            if (!_state[s_stateContstructing])
            {
                // Dont fire while we're in the base constructor as the inherited
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
        ///  Some implementations of DefaultMargin check which container they
        ///  are on. They need to be re-evaluated when the containership changes.
        ///  DefaultMargin will stop being honored the moment someone sets the Margin property.
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
            Color backColor = Properties.GetColor(s_backColorProperty);
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
            Color foreColor = Properties.GetColor(s_foreColorProperty);
            return !foreColor.IsEmpty;
        }

        /// <summary>
        ///  Returns true if the font should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeFont()
        {
            object font = Properties.GetObject(s_fontProperty, out bool found);
            return (found && font != null);
        }

        /// <summary>
        ///  Determines if the <see cref='Padding'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePadding() => Padding != DefaultPadding;

        /// <summary>
        ///  Determines if the <see cref='Margin'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMargin() => Margin != DefaultMargin;

        /// <summary>
        ///  Determines if the <see cref='Visible'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeVisible() => !_state[s_stateVisible];

        /// <summary>
        ///  Determines if the <see cref='Image'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImage() => Image != null && ImageIndexer.ActualIndex < 0;

        /// <summary>
        ///  Determines if the <see cref='Image'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageKey()
            => Image != null && ImageIndexer.ActualIndex >= 0 && !string.IsNullOrEmpty(ImageIndexer.Key);

        /// <summary>
        ///  Determines if the <see cref='Image'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageIndex()
            => Image != null && ImageIndexer.ActualIndex >= 0 && ImageIndexer.Index != ImageList.Indexer.DefaultIndex;

        /// <summary>
        ///  Determines if the <see cref='RightToLeft'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeRightToLeft()
        {
            int rightToLeft = Properties.GetInteger(s_rightToLeftProperty, out bool found);
            if (!found)
            {
                return false;
            }

            return rightToLeft != (int)DefaultRightToLeft;
        }

        private bool ShouldSerializeTextDirection()
        {
            ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
            if (Properties.ContainsObject(ToolStripItem.s_textDirectionProperty))
            {
                textDirection = (ToolStripTextDirection)Properties.GetObject(ToolStripItem.s_textDirectionProperty);
            }

            return textDirection != ToolStripTextDirection.Inherit;
        }

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
            ToolStrip parent = ParentInternal;

            if (parent is null)
            {
                parent = (IsOnOverflow && Owner != null) ? Owner.OverflowButton.DropDown : Owner;
            }
            if (parent is null)
            {
                // should not throw here as it's an internal function call.
                return fromPoint;
            }

            if (fromPointType == toPointType)
            {
                return fromPoint;
            }

            Point toPoint = Point.Empty;
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

        internal ToolStrip RootToolStrip
        {
            get
            {
                ToolStripItem item = this;
                while (item.OwnerItem != null)
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
            Debug.WriteLineIf(ToolStrip.s_selectionDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "[Selection DBG] WBI.Unselect: {0}", ToString()));
            if (_state[s_stateSelected])
            {
                _state[s_stateSelected] = false;
                if (Available)
                {
                    Invalidate();
                    if (ParentInternal != null)
                    {
                        ParentInternal.NotifySelectionChange(this);
                    }

                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                }
            }
        }

        bool IKeyboardToolTip.CanShowToolTipsNow()
            => Visible && _parent != null && ((IKeyboardToolTip)_parent).AllowsChildrenToShowToolTips();

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => AccessibilityObject.Bounds;

        IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
        {
            List<Rectangle> neighbors = new List<Rectangle>(3);
            if (_parent != null)
            {
                ToolStripItemCollection items = _parent.DisplayedItems;
                int i = 0, count = items.Count;
                bool found = false;
                while (!found && i < count)
                {
                    found = Object.ReferenceEquals(items[i], this);
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

            if (_parent is ToolStripDropDown dropDown && dropDown.OwnerItem != null)
            {
                neighbors.Add(((IKeyboardToolTip)dropDown.OwnerItem).GetNativeScreenRectangle());
            }

            return neighbors;
        }

        bool IKeyboardToolTip.IsHoveredWithMouse()
            => ((IKeyboardToolTip)this).GetNativeScreenRectangle().Contains(Control.MousePosition);

        bool IKeyboardToolTip.HasRtlModeEnabled()
            => _parent != null && ((IKeyboardToolTip)_parent).HasRtlModeEnabled();

        bool IKeyboardToolTip.AllowsToolTip() => true;

        IWin32Window IKeyboardToolTip.GetOwnerWindow()
        {
            Debug.Assert(ParentInternal != null, "Tool Strip Item Parent is null");
            return ParentInternal;
        }

        void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) => ToolTipText;

        bool IKeyboardToolTip.ShowsOwnToolTip() => true;

        bool IKeyboardToolTip.IsBeingTabbedTo() => IsBeingTabbedTo();

        bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => true;

        internal virtual void OnKeyboardToolTipHook(ToolTip toolTip)
        {
        }

        internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip)
        {
        }

        internal virtual bool IsBeingTabbedTo() => ToolStrip.AreCommonNavigationalKeysDown();
    }
}
