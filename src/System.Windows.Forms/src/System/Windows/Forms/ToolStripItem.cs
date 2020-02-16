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
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using static Interop;

namespace System.Windows.Forms
{
    [DesignTimeVisible(false)]
    [Designer("System.Windows.Forms.Design.ToolStripItemDesigner, " + AssemblyRef.SystemDesign)]
    [DefaultEvent(nameof(Click))]
    [ToolboxItem(false)]
    [DefaultProperty(nameof(Text))]
    public abstract class ToolStripItem : Component,
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
        private BitVector32 _state = new BitVector32();
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

        // Property store keys for properties.  The property store allocates most efficiently
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
        private static readonly int s_accessibleHelpProviderProperty = PropertyStore.CreateKey();

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
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ToolStripItemAccessibilityObjectDescr))
        ]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(s_accessibilityProperty);
                if (accessibleObject == null)
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
        [
        SRCategory(nameof(SR.CatAccessibility)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ToolStripItemAccessibleDefaultActionDescr))
        ]
        public string AccessibleDefaultActionDescription
        {
            get
            {
                return (string)Properties.GetObject(s_accessibleDefaultActionDescriptionProperty);
            }
            set
            {
                Properties.SetObject(s_accessibleDefaultActionDescriptionProperty, value);
                OnAccessibleDefaultActionDescriptionChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        ///  The accessible description of the control
        /// </summary>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemAccessibleDescriptionDescr))
        ]
        public string AccessibleDescription
        {
            get
            {
                return (string)Properties.GetObject(s_accessibleDescriptionProperty);
            }
            set
            {
                Properties.SetObject(s_accessibleDescriptionProperty, value);
                OnAccessibleDescriptionChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  The accessible name of the control
        /// </summary>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemAccessibleNameDescr))
        ]
        public string AccessibleName
        {
            get
            {
                return (string)Properties.GetObject(s_accessibleNameProperty);
            }

            set
            {
                Properties.SetObject(s_accessibleNameProperty, value);
                OnAccessibleNameChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  The accessible role of the control
        /// </summary>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(AccessibleRole.Default),
        SRDescription(nameof(SR.ToolStripItemAccessibleRoleDescr))
        ]
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
                //valid values are 0xffffffff to 0x40
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
        [
        DefaultValue(ToolStripItemAlignment.Left),
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.ToolStripItemAlignmentDescr))
        ]
        public ToolStripItemAlignment Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemAlignment.Left, (int)ToolStripItemAlignment.Right))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemAlignment));
                }
                if (_alignment != value)
                {
                    _alignment = value;

                    if ((ParentInternal != null) && ParentInternal.IsHandleCreated)
                    {
                        ParentInternal.PerformLayout();
                    }
                }
            }
        }

        /// <summary>
        ///  Determines if this item can be dragged.
        ///  This is EXACTLY like Control.AllowDrop - setting this to true WILL call
        ///  the droptarget handlers.  The ToolStripDropTargetManager is the one that
        ///  handles the routing of DropTarget events to the ToolStripItem's IDropTarget
        ///  methods.
        /// </summary>
        [
        SRCategory(nameof(SR.CatDragDrop)),
        DefaultValue(false),
        SRDescription(nameof(SR.ToolStripItemAllowDropDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public virtual bool AllowDrop
        {
            get
            {
                return _state[s_stateAllowDrop];
            }
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
            get
            {
                return _state[s_stateAutoSize];
            }
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
            get
            {
                return _state[s_stateAutoToolTip];
            }
            set
            {
                _state[s_stateAutoToolTip] = value;
            }
        }

        /// <summary>
        ///  as opposed to Visible, which returns whether or not the item and its parent are Visible
        ///  Available returns whether or not the item will be shown.  Setting Available sets Visible and Vice/Versa
        /// </summary>
        [
        Browsable(false),
        SRDescription(nameof(SR.ToolStripItemAvailableDescr)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool Available
        {
            get
            {
                // MainMenu compat:
                // the only real diff is the getter - this returns what the item really thinks,
                // as opposed to whether or not the parent is also Visible.  Visible behavior is the same
                // so that it matches Control behavior (we dont have to do special things for control hosts, etc etc).
                return _state[s_stateVisible];
            }
            set
            {
                SetVisibleCore(value);
            }
        }

        [
        Browsable(false),
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.ToolStripItemOnAvailableChangedDescr))
        ]
        public event EventHandler AvailableChanged
        {
            add => Events.AddHandler(s_availableChangedEvent, value);
            remove => Events.RemoveHandler(s_availableChangedEvent, value);
        }

        /// <summary>
        ///  Gets or sets the image that is displayed on a <see cref='Label'/>.
        /// </summary>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageDescr)),
        DefaultValue(null)
        ]
        public virtual Image BackgroundImage
        {
            get
            {
                return Properties.GetObject(s_backgroundImageProperty) as Image;
            }
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
            get
            {
                return _deviceDpi;
            }
            set
            {
                _deviceDpi = value;
            }
        }

        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ImageLayout.Tile),
        Localizable(true),
        SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))
        ]
        public virtual ImageLayout BackgroundImageLayout
        {
            get
            {
                bool found = Properties.ContainsObject(s_backgroundImageLayoutProperty);
                if (!found)
                {
                    return ImageLayout.Tile;
                }
                else
                {
                    return ((ImageLayout)Properties.GetObject(s_backgroundImageLayoutProperty));
                }
            }
            set
            {
                if (BackgroundImageLayout != value)
                {
                    //valid values are 0x0 to 0x4
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
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemBackColorDescr))
        ]
        public virtual Color BackColor
        {
            get
            {
                Color c = RawBackColor; // inheritedProperties.BackColor
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

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnBackColorChangedDescr))]
        public event EventHandler BackColorChanged
        {
            add => Events.AddHandler(s_backColorChangedEvent, value);
            remove => Events.RemoveHandler(s_backColorChangedEvent, value);
        }

        /// <summary>
        ///  The bounds of the item
        /// </summary>
        [Browsable(false)]
        public virtual Rectangle Bounds
        {
            get
            {
                return _bounds;
            }
        }

        // Zero-based rectangle, same concept as ClientRect
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
        public virtual bool CanSelect
        {
            get
            {
                return true;
            }
        }

        // usually the same as can select, but things like the control box in an MDI window are exceptions
        internal virtual bool CanKeyboardSelect
        {
            get
            {
                return CanSelect;
            }
        }

        /// <summary>
        ///  Occurs when the control is clicked.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAction)),
        SRDescription(nameof(SR.ToolStripItemOnClickDescr))
        ]
        public event EventHandler Click
        {
            add => Events.AddHandler(s_clickEvent, value);
            remove => Events.RemoveHandler(s_clickEvent, value);
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DefaultValue(CommonProperties.DefaultAnchor)
        ]
        public AnchorStyles Anchor
        {
            get
            {
                // since we dont support DefaultLayout go directly against the CommonProperties
                return CommonProperties.xGetAnchor(this);
            }
            set
            {
                // flags enum - dont check for validity....

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

        /// <summary> This does not show up in the property grid because it only applies to flow and table layouts </summary>
        [
        Browsable(false),
        DefaultValue(CommonProperties.DefaultDock)
        ]
        public DockStyle Dock
        {
            get
            {
                // since we dont support DefaultLayout go directly against the CommonProperties
                return CommonProperties.xGetDock(this);
            }
            set
            {
                //valid values are 0x0 to 0x5
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

        /// <summary>default setting of auto tooltip when this object is created</summary>
        protected virtual bool DefaultAutoToolTip
        {
            get
            {
                return false;
            }
        }

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
                else
                {
                    return _scaledDefaultMargin;
                }
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected virtual Padding DefaultPadding
        {
            get
            {
                return Padding.Empty;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected virtual Size DefaultSize
        {
            get
            {
                return DpiHelper.IsPerMonitorV2Awareness ?
                       DpiHelper.LogicalToDeviceUnits(new Size(23, 23), DeviceDpi) :
                       new Size(23, 23);
            }
        }

        protected virtual ToolStripItemDisplayStyle DefaultDisplayStyle
        {
            get
            {
                return ToolStripItemDisplayStyle.ImageAndText;
            }
        }

        /// <summary>
        ///  specifies the default behavior of these items on ToolStripDropDowns when clicked.
        /// </summary>
        internal protected virtual bool DismissWhenClicked
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  DisplayStyle specifies whether the image and text are rendered.  This is not on the base
        ///  item class because different derived things will have different enumeration needs.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemDisplayStyleDescr))
        ]
        public virtual ToolStripItemDisplayStyle DisplayStyle
        {
            get
            {
                return _displayStyle;
            }
            set
            {
                if (_displayStyle != value)
                {
                    //valid values are 0x0 to 0x3
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
        private RightToLeft DefaultRightToLeft
        {
            get
            {
                return RightToLeft.Inherit;
            }
        }

        /// <summary>
        ///  Occurs when the control is double clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick
        {
            add => Events.AddHandler(s_doubleClickEvent, value);
            remove => Events.RemoveHandler(s_doubleClickEvent, value);
        }

        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ToolStripItemDoubleClickedEnabledDescr))
        ]
        public bool DoubleClickEnabled
        {
            get
            {
                return _state[s_stateDoubleClickEnabled];
            }
            set
            {
                _state[s_stateDoubleClickEnabled] = value;
            }
        }

        [
        SRCategory(nameof(SR.CatDragDrop)),
        SRDescription(nameof(SR.ToolStripItemOnDragDropDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event DragEventHandler DragDrop
        {
            add => Events.AddHandler(s_dragDropEvent, value);
            remove => Events.RemoveHandler(s_dragDropEvent, value);
        }

        [
        SRCategory(nameof(SR.CatDragDrop)),
        SRDescription(nameof(SR.ToolStripItemOnDragEnterDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event DragEventHandler DragEnter
        {
            add => Events.AddHandler(s_dragEnterEvent, value);
            remove => Events.RemoveHandler(s_dragEnterEvent, value);
        }

        [
        SRCategory(nameof(SR.CatDragDrop)),
        SRDescription(nameof(SR.ToolStripItemOnDragOverDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event DragEventHandler DragOver
        {
            add => Events.AddHandler(s_dragOverEvent, value);
            remove => Events.RemoveHandler(s_dragOverEvent, value);
        }

        [
        SRCategory(nameof(SR.CatDragDrop)),
        SRDescription(nameof(SR.ToolStripItemOnDragLeaveDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event EventHandler DragLeave
        {
            add => Events.AddHandler(s_dragLeaveEvent, value);
            remove => Events.RemoveHandler(s_dragLeaveEvent, value);
        }

        /// <summary>
        ///  ToolStripItem.DropSource
        ///
        ///  This represents what we're actually going to drag.  If the parent has set AllowItemReorder to true,
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemEnabledDescr)),
        DefaultValue(true)
        ]
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

        [
        SRDescription(nameof(SR.ToolStripItemEnabledChangedDescr))
        ]
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
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemForeColorDescr))
        ]
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

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnForeColorChangedDescr))]
        public event EventHandler ForeColorChanged
        {
            add => Events.AddHandler(s_foreColorChangedEvent, value);
            remove => Events.RemoveHandler(s_foreColorChangedEvent, value);
        }

        /// <summary>
        ///  Retrieves the current font for this control. This will be the font used
        ///  by default for painting and text in the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemFontDescr))
        ]
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

                return DpiHelper.IsPerMonitorV2Awareness ?
                       _defaultFont :
                       ToolStripManager.DefaultFont;
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
        [
        SRCategory(nameof(SR.CatDragDrop)),
        SRDescription(nameof(SR.ToolStripItemOnGiveFeedbackDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event GiveFeedbackEventHandler GiveFeedback
        {
            add => Events.AddHandler(s_giveFeedbackEvent, value);
            remove => Events.RemoveHandler(s_giveFeedbackEvent, value);
        }

        /// <summary>
        ///  The height of this control
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int Height
        {
            get
            {
                return Bounds.Height;
            }
            set
            {
                Rectangle currentBounds = Bounds;
                SetBounds(currentBounds.X, currentBounds.Y, currentBounds.Width, value);
            }
        }

        /// <summary>
        ///  ToolStripItems do not have children.  For perf reasons always return a static empty collection.
        ///  Consider creating readonly collection.
        /// </summary>
        ArrangedElementCollection IArrangedElement.Children
        {
            get
            {
                return ToolStripItem.s_emptyChildCollection;
            }
        }
        /// <summary>
        ///  Should not be exposed as this returns an unexposed type.
        /// </summary>
        IArrangedElement IArrangedElement.Container
        {
            get
            {
                if (ParentInternal == null)
                {
                    return Owner;
                }
                return ParentInternal;
            }
        }

        Rectangle IArrangedElement.DisplayRectangle
        {
            get
            {
                return Bounds;
            }
        }

        bool IArrangedElement.ParticipatesInLayout
        {
            get
            {
                // this can be different than "Visible" property as "Visible" takes into account whether or not you
                // are parented and your parent is visible.
                return _state[s_stateVisible];
            }
        }

        PropertyStore IArrangedElement.Properties
        {
            get
            {
                return Properties;
            }
        }

        // Sets the bounds for an element.
        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            // in this case the parent is telling us to refresh our bounds - dont
            // call PerformLayout
            SetBounds(bounds);
        }

        void IArrangedElement.PerformLayout(IArrangedElement container, string propertyName)
        {
            return;
        }

        /// <summary>
            ///  Gets or sets the alignment of the image on the label control.
            /// </summary>
        [
        DefaultValue(ContentAlignment.MiddleCenter),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageAlignDescr))
        ]
        public ContentAlignment ImageAlign
        {
            get
            {
                return _imageAlign;
            }
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
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageDescr))
        ]
        public virtual Image Image
        {
            get
            {
                Image image = (Image)Properties.GetObject(s_imageProperty);

                if (image == null && (Owner != null) && (Owner.ImageList != null) && ImageIndexer.ActualIndex >= 0)
                {
                    if (ImageIndexer.ActualIndex < Owner.ImageList.Images.Count)
                    {
                        // CACHE (by design).  If we fetched out of the image list every time it would dramatically hit perf.
                        image = Owner.ImageList.Images[ImageIndexer.ActualIndex];
                        _state[s_stateInvalidMirroredImage] = true;
                        Properties.SetObject(s_imageProperty, image);
                        return image;
                    }
                }
                else
                {
                    return image;
                }
                return null;
            }
            set
            {
                if (Image != value)
                {
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
                        ImageIndex = -1;
                    }
                    Properties.SetObject(s_imageProperty, value);
                    _state[s_stateInvalidMirroredImage] = true;
                    Animate();
                    InvalidateItemLayout(PropertyNames.Image);
                }
            }
        }

        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageTransparentColorDescr))
        ]
        public Color ImageTransparentColor
        {
            get
            {
                return _imageTransparentColor;
            }
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
        [
        SRDescription(nameof(SR.ToolStripItemImageIndexDescr)),
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter(typeof(NoneExcludedImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(false),
        RelatedImageList("Owner.ImageList")
        ]
        public int ImageIndex
        {
            get
            {
                if ((Owner != null) && ImageIndexer.Index != -1 && Owner.ImageList != null && ImageIndexer.Index >= Owner.ImageList.Images.Count)
                {
                    return Owner.ImageList.Images.Count - 1;
                }
                return ImageIndexer.Index;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, -1));
                }

                ImageIndexer.Index = value;
                _state[s_stateInvalidMirroredImage] = true;
                // Set the Image Property to null
                Properties.SetObject(s_imageProperty, null);

                InvalidateItemLayout(PropertyNames.ImageIndex);
            }
        }

        internal ToolStripItemImageIndexer ImageIndexer
        {
            get
            {
                if (_imageIndexer == null)
                {
                    _imageIndexer = new ToolStripItemImageIndexer(this);
                }
                return _imageIndexer;
            }
        }

        /// <summary>
        ///  Returns the ToolStripItem's currently set image index
        ///  Here for compat only - this is NOT to be visible at DT.
        /// </summary>
        [
        SRDescription(nameof(SR.ToolStripItemImageKeyDescr)),
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        TypeConverter(typeof(ImageKeyConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        Editor("System.Windows.Forms.Design.ToolStripImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(false),
        RelatedImageList("Owner.ImageList")
       ]
        public string ImageKey
        {
            get
            {
                return ImageIndexer.Key;
            }
            set
            {
                ImageIndexer.Key = value;
                _state[s_stateInvalidMirroredImage] = true;
                Properties.SetObject(s_imageProperty, null);

                InvalidateItemLayout(PropertyNames.ImageKey);
            }
        }

        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ToolStripItemImageScaling.SizeToFit),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemImageScalingDescr))
        ]
        public ToolStripItemImageScaling ImageScaling
        {
            get
            {
                return _imageScaling;
            }
            set
            {
                //valid values are 0x0 to 0x1
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
        {
            get
            {
                if (_toolStripItemInternalLayout == null)
                {
                    _toolStripItemInternalLayout = CreateInternalLayout();
                }
                return _toolStripItemInternalLayout;
            }
        }

        internal bool IsForeColorSet
        {
            get
            {
                Color color = Properties.GetColor(s_foreColorProperty);
                if (!color.IsEmpty)
                {
                    return true;
                }
                else
                {
                    Control parent = ParentInternal;
                    if (parent != null)
                    {
                        return parent.ShouldSerializeForeColor();
                    }
                }
                return false;
            }
        }

        /// <summary>
        ///  This is used by ToolStrip to pass on the mouseMessages for ActiveDropDown.
        /// </summary>
        internal bool IsInDesignMode
        {
            get
            {
                return DesignMode;
            }
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return _state[s_stateDisposed];
            }
        }

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

        ///  returns whether the item placement is set to overflow.
        [Browsable(false)]
        public bool IsOnOverflow
        {
            get
            {
                return (Placement == ToolStripItemPlacement.Overflow);
            }
        }

        /// <summary>
        ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
        /// </summary>
        internal virtual bool IsMnemonicsListenerAxSourced
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  Occurs when the location of the ToolStripItem has been updated -- usually by layout by its
        ///  owner of ToolStrips
        /// </summary>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ToolStripItemOnLocationChangedDescr))]
        public event EventHandler LocationChanged
        {
            add => Events.AddHandler(s_locationChangedEvent, value);
            remove => Events.RemoveHandler(s_locationChangedEvent, value);
        }

        /// <summary>
        ///  Specifies the external spacing between this item and any other item or the ToolStrip.
        /// </summary>
        [
        SRDescription(nameof(SR.ToolStripItemMarginDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public Padding Margin
        {
            get { return CommonProperties.GetMargin(this); }
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
        [
        SRDescription(nameof(SR.ToolStripMergeActionDescr)),
        DefaultValue(MergeAction.Append),
        SRCategory(nameof(SR.CatLayout))
        ]
        public MergeAction MergeAction
        {
            get
            {
                int action = Properties.GetInteger(s_mergeActionProperty, out bool found);
                if (found)
                {
                    return (MergeAction)action;
                }
                else
                {
                    // default value
                    return MergeAction.Append;
                }
            }

            set
            {
                //valid values are 0x0 to 0x4
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
        [
        SRDescription(nameof(SR.ToolStripMergeIndexDescr)),
        DefaultValue(-1),
        SRCategory(nameof(SR.CatLayout))
        ]
        public int MergeIndex
        {
            get
            {
                int index = Properties.GetInteger(s_mergeIndexProperty, out bool found);
                if (found)
                {
                    return index;
                }
                else
                {
                    // default value
                    return -1;
                }
            }

            set
            {
                Properties.SetInteger(s_mergeIndexProperty, value);
            }
        }

        // required for menus
        internal bool MouseDownAndUpMustBeInSameItem
        {
            get { return _state[s_stateMouseDownAndUpMustBeInSameItem]; }
            set { _state[s_stateMouseDownAndUpMustBeInSameItem] = value; }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is
        ///  pressed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatMouse)),
        SRDescription(nameof(SR.ToolStripItemOnMouseDownDescr))
        ]
        public event MouseEventHandler MouseDown
        {
            add => Events.AddHandler(s_mouseDownEvent, value);
            remove => Events.RemoveHandler(s_mouseDownEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer enters the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatMouse)),
        SRDescription(nameof(SR.ToolStripItemOnMouseEnterDescr))
        ]
        public event EventHandler MouseEnter
        {
            add => Events.AddHandler(s_mouseEnterEvent, value);
            remove => Events.RemoveHandler(s_mouseEnterEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer leaves the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatMouse)),
        SRDescription(nameof(SR.ToolStripItemOnMouseLeaveDescr))
        ]
        public event EventHandler MouseLeave
        {
            add => Events.AddHandler(s_mouseLeaveEvent, value);
            remove => Events.RemoveHandler(s_mouseLeaveEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer hovers over the contro.
        /// </summary>
        [
        SRCategory(nameof(SR.CatMouse)),
        SRDescription(nameof(SR.ToolStripItemOnMouseHoverDescr))
        ]
        public event EventHandler MouseHover
        {
            add => Events.AddHandler(s_mouseHoverEvent, value);
            remove => Events.RemoveHandler(s_mouseHoverEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is moved over the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatMouse)),
        SRDescription(nameof(SR.ToolStripItemOnMouseMoveDescr))
        ]
        public event MouseEventHandler MouseMove
        {
            add => Events.AddHandler(s_mouseMoveEvent, value);
            remove => Events.RemoveHandler(s_mouseMoveEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        [
        SRCategory(nameof(SR.CatMouse)),
        SRDescription(nameof(SR.ToolStripItemOnMouseUpDescr))
        ]
        public event MouseEventHandler MouseUp
        {
            add => Events.AddHandler(s_mouseUpEvent, value);
            remove => Events.RemoveHandler(s_mouseUpEvent, value);
        }

        /// <summary>
        ///  Name of this control. The designer will set this to the same
        ///  as the programatic Id "(name)" of the control.  The name can be
        ///  used as a key into the ControlCollection.
        /// </summary>
        [
        Browsable(false),
        DefaultValue(null)
        ]
        public string Name
        {
            get
            {
                return WindowsFormsUtils.GetComponentName(this, (string)Properties.GetObject(ToolStripItem.s_nameProperty));
            }
            set
            {
                if (DesignMode) //InDesignMode the Extender property will set it to the right value.
                {
                    return;
                }
                Properties.SetObject(ToolStripItem.s_nameProperty, value);
            }
        }

        /// <summary>
        ///  The owner of this ToolStripItem.  The owner is essentially a backpointer to
        ///  the ToolStrip who contains this item in it's item collection.  Handy for getting
        ///  to things such as the ImageList, which would be defined on the ToolStrip.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ToolStrip Owner
        {
            get
            {
                return _owner;
            }
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

        /// <summary> returns the "parent" item on the preceeding menu which has spawned this item.
        ///  e.g.  File->Open  the OwnerItem of Open is File. </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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

        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ToolStripItemOwnerChangedDescr))
        ]
        public event EventHandler OwnerChanged
        {
            add => Events.AddHandler(s_ownerChangedEvent, value);
            remove => Events.RemoveHandler(s_ownerChangedEvent, value);
        }

        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemOnPaintDescr))
        ]
        public event PaintEventHandler Paint
        {
            add => Events.AddHandler(s_paintEvent, value);
            remove => Events.RemoveHandler(s_paintEvent, value);
        }

        /// <summary>
        ///  The parent of this ToolStripItem.  This can be distinct from the owner because
        ///  the item can fall onto another window (overflow).  In this case the overflow
        ///  would be the parent but the original ToolStrip would be the Owner.  The "parent"
        ///  ToolStrip will be firing things like paint events - where as the "owner" ToolStrip
        ///  will be containing shared data like image lists.  Typically the only one who should
        ///  set the parent property is the layout manager on the ToolStrip.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        internal protected ToolStrip Parent
        {
            get
            {
                // we decided that there is no "parent" protection for toolstripitems.
                // since toolstrip and toolstripitem are tightly coupled.
                return ParentInternal;
            }
            set
            {
                ParentInternal = value;
            }
        }

        /// <summary>
        ///  Specifies whether or not the item is glued to the ToolStrip or overflow or
        ///  can float between the two.
        /// </summary>
        [
        DefaultValue(ToolStripItemOverflow.AsNeeded),
        SRDescription(nameof(SR.ToolStripItemOverflowDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public ToolStripItemOverflow Overflow
        {
            get
            {
                return _overflow;
            }
            set
            {
                //valid values are 0x0 to 0x2
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
        [
        SRDescription(nameof(SR.ToolStripItemPaddingDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public virtual Padding Padding
        {
            get { return CommonProperties.GetPadding(this, DefaultPadding); }
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
            get
            {
                return _parent;
            }
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
        public ToolStripItemPlacement Placement
        {
            get
            {
                return _placement;
            }
        }

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
                    imageSize = (image == null) ? Size.Empty : image.Size;
                }

                return imageSize;
            }
        }

        /// <summary>
        ///  Retrieves our internal property storage object. If you have a property
        ///  whose value is not always set, you should store it in here to save
        ///  space.
        /// </summary>
        internal PropertyStore Properties
        {
            get
            {
                if (_propertyStore == null)
                {
                    _propertyStore = new PropertyStore();
                }
                return _propertyStore;
            }
        }

        /// <summary>
        ///  Returns true if the state of the item is pushed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Pressed
        {
            get
            {
                return CanSelect && _state[s_statePressed];
            }
        }

        [
        SRCategory(nameof(SR.CatDragDrop)),
        SRDescription(nameof(SR.ToolStripItemOnQueryContinueDragDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event QueryContinueDragEventHandler QueryContinueDrag
        {
            add => Events.AddHandler(s_queryContinueDragEvent, value);
            remove => Events.RemoveHandler(s_queryContinueDragEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ToolStripItemOnQueryAccessibilityHelpDescr))]
        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add => Events.AddHandler(s_queryAccessibilityHelpEvent, value);
            remove => Events.RemoveHandler(s_queryAccessibilityHelpEvent, value);
        }

        // Returns the value of the backColor field -- no asking the parent with its color is, etc.
        internal Color RawBackColor
        {
            get
            {
                return Properties.GetColor(s_backColorProperty);
            }
        }

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
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemRightToLeftDescr))
        ]
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
                //valid values are 0x0 to 0x2
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
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemRightToLeftAutoMirrorImageDescr))
        ]
        public bool RightToLeftAutoMirrorImage
        {
            get
            {
                return _state[s_stateRightToLeftAutoMirrorImage];
            }
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
                    if (image != null)
                    {
                        Image mirroredImage = image.Clone() as Image;
                        mirroredImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

                        Properties.SetObject(s_mirroredImageProperty, mirroredImage);
                        _state[s_stateInvalidMirroredImage] = false;
                        return mirroredImage;
                    }
                    else
                    {
                        return null;
                    }
                }
                return Properties.GetObject(s_mirroredImageProperty) as Image;
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged
        {
            add => Events.AddHandler(s_rightToLeftChangedEvent, value);
            remove => Events.RemoveHandler(s_rightToLeftChangedEvent, value);
        }

        /// <summary>
        ///  if the item is selected we return true.
        ///
        ///  FAQ: Why dont we have a Hot or MouseIsOver property?
        ///  After going through the scenarios, we've decided NOT to add a separate MouseIsOver or Hot flag to ToolStripItem.  The thing to use is 'Selected'.
        ///  Why?  While the selected thing can be different than the moused over item, the selected item is ALWAYS the one you want to paint differently
        ///
        ///  Scenario 1:  Keyboard select an item then select a different item with the mouse.
        ///  -          Do Alt+F to expand your File menu, keyboard down several items.
        ///  -          Mouse over a different item
        ///  -          Notice how two things are never painted hot at the same time, and how the selection changes from the keyboard selected item to the one selected with the mouse.  In  this case the selection should move with the mouse selection.
        ///  -          Notice how if you hit enter when the mouse is over it, it executes the item.  That's selection.
        ///  Scenario 2: Put focus into a combo box, then mouse over a different item
        ///  -          Notice how all the other items you mouse over do not change the way they are painted, if you hit enter, that goes to the combobox, rather than executing the current item.
        ///
        ///  At first look "MouseIsOver" or "Hot" seems to be the thing people want, but its almost never the desired behavior.  A unified selection model is simpler and seems to meet the scenarios.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selected
        {
            get
            {
                return CanSelect && !DesignMode && (_state[s_stateSelected] ||
                    (ParentInternal != null && ParentInternal.IsSelectionSuspended &&
                     ParentInternal.LastMouseDownedItem == this));
            }
        }

        internal protected virtual bool ShowKeyboardCues
        {
            get
            {
                if (!DesignMode)
                {
                    return ToolStripManager.ShowMenuFocusCues;
                }
                // default to true.
                return true;
            }
        }

        /// <summary>The size of the item</summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemSizeDescr))
        ]
        public virtual Size Size
        {
            get
            {
                return Bounds.Size;
            }
            set
            {
                Rectangle currentBounds = Bounds;
                currentBounds.Size = value;
                SetBounds(currentBounds);
            }
        }

        internal bool SupportsRightClick
        {
            get
            {
                return _state[s_stateSupportsRightClick];
            }
            set
            {
                _state[s_stateSupportsRightClick] = value;
            }
        }

        internal bool SupportsItemClick
        {
            get
            {
                return _state[s_stateSupportsItemClick];
            }
            set
            {
                _state[s_stateSupportsItemClick] = value;
            }
        }

        internal bool SupportsSpaceKey
        {
            get
            {
                return _state[s_stateSupportsSpaceKey];
            }
            set
            {
                _state[s_stateSupportsSpaceKey] = value;
            }
        }

        internal bool SupportsDisabledHotTracking
        {
            get
            {
                return _state[s_stateSupportsDisabledHotTracking];
            }
            set
            {
                _state[s_stateSupportsDisabledHotTracking] = value;
            }
        }

        /// <summary>Summary for Tag</summary>
        [DefaultValue(null),
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ToolStripItemTagDescr)),
        TypeConverter(typeof(StringConverter))
        ]
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
            set
            {
                Properties.SetObject(ToolStripItem.s_tagProperty, value);
            }
        }

        /// <summary>The text of the item</summary>
        [
        DefaultValue(""),
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemTextDescr))
        ]
        public virtual string Text
        {
            get
            {
                if (Properties.ContainsObject(ToolStripItem.s_textProperty))
                {
                    return (string)Properties.GetObject(ToolStripItem.s_textProperty);
                }
                return "";
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
        [
        DefaultValue(ContentAlignment.MiddleCenter),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemTextAlignDescr))
        ]
        public virtual ContentAlignment TextAlign
        {
            get
            {
                return _textAlign;
            }
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

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnTextChangedDescr))]
        public event EventHandler TextChanged
        {
            add => Events.AddHandler(s_textChangedEvent, value);
            remove => Events.RemoveHandler(s_textChangedEvent, value);
        }

        [
        SRDescription(nameof(SR.ToolStripTextDirectionDescr)),
        SRCategory(nameof(SR.CatAppearance))
        ]
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
                        textDirection = (Owner == null) ? ToolStripTextDirection.Horizontal : Owner.TextDirection;
                    }
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
            set
            {
                _toolTipText = value;
            }
        }

        /// <summary>Whether or not the item is visible</summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemVisibleDescr))
        ]
        public bool Visible
        {
            get
            {
                return (ParentInternal != null) && (ParentInternal.Visible) && Available;
            }
            set
            {
                SetVisibleCore(value);
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnVisibleChangedDescr))]
        public event EventHandler VisibleChanged
        {
            add => Events.AddHandler(s_visibleChangedEvent, value);
            remove => Events.RemoveHandler(s_visibleChangedEvent, value);
        }

        /// <summary>
        ///  The width of this ToolStripItem.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int Width
        {
            get
            {
                return Bounds.Width;
            }
            set
            {
                Rectangle currentBounds = Bounds;
                SetBounds(currentBounds.X, currentBounds.Y, value, currentBounds.Height);
            }
        }

        //
        //  Methods for ToolStripItem
        //

        internal void AccessibilityNotifyClients(AccessibleEvents accEvent)
        {
            if (ParentInternal != null)
            {
                int index = ParentInternal.DisplayedItems.IndexOf(this);
                ParentInternal.AccessibilityNotifyClients(accEvent, index);
            }
        }

        private void Animate()
        {
            Animate(!DesignMode && Visible && Enabled && ParentInternal != null);
        }

        private void StopAnimate()
        {
            Animate(false);
        }

        private void Animate(bool animate)
        {
            if (animate != _state[s_stateCurrentlyAnimatingImage])
            {
                if (animate)
                {
                    if (Image != null)
                    {
                        ImageAnimator.Animate(Image, new EventHandler(OnAnimationFrameChanged));
                        _state[s_stateCurrentlyAnimatingImage] = animate;
                    }
                }
                else
                {
                    if (Image != null)
                    {
                        ImageAnimator.StopAnimate(Image, new EventHandler(OnAnimationFrameChanged));
                        _state[s_stateCurrentlyAnimatingImage] = animate;
                    }
                }
            }
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
        ///  constructs the new instance of the accessibility object for this ToolStripItem. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripItemAccessibleObject(this);
        }

        /// <summary>
        ///  Creates an instance of the object that defines how image and text
        ///  gets laid out in the ToolStripItem
        /// </summary>
        internal virtual ToolStripItemInternalLayout CreateInternalLayout()
        {
            return new ToolStripItemInternalLayout(this);
        }
        /// <summary>
        ///  Disposes this ToolStrip item...
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
            get { return SystemInformation.DoubleClickTime * 10000; }
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
            if (dataObject == null)
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
                    // looking at a ToolStripButton.  The alternative is
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
        {
            FireEvent(EventArgs.Empty, met);
        }
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

        private Font GetOwnerFont()
        {
            if (Owner != null)
            {
                return Owner.Font;
            }
            return null;
        }

        /// <summary> we dont want a public settable property... and usually owner will work
        ///  except for things like the overflow button</summary>
        public ToolStrip GetCurrentParent()
        {
            return Parent;
        }

        internal ToolStripDropDown GetCurrentParentDropDown()
        {
            if (ParentInternal != null)
            {
                return ParentInternal as ToolStripDropDown;
            }
            else
            {
                return Owner as ToolStripDropDown;
            }
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
        public void Invalidate()
        {
            if (ParentInternal != null)
            {
                ParentInternal.Invalidate(Bounds, true);
            }
        }

        /// <summary>
        ///  invalidates a rectangle within the ToolStripItem's bounds
        /// </summary>
        public void Invalidate(Rectangle r)
        {
            // the only value add to this over calling invalidate on the ToolStrip is that
            // you can specify the rectangle with coordinates relative to the upper left hand
            // corner of the ToolStripItem.
            Point rectangleLocation = TranslatePoint(r.Location, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ToolStripCoords);

            if (ParentInternal != null)
            {
                ParentInternal.Invalidate(new Rectangle(rectangleLocation, r.Size), true);
            }
        }

        internal void InvalidateItemLayout(string affectedProperty, bool invalidatePainting)
        {
            _toolStripItemInternalLayout = null;

            if (Owner != null)
            {
                LayoutTransaction.DoLayout(Owner, this, affectedProperty);
            }

            if (invalidatePainting && Owner != null)
            {
                Owner.Invalidate();
            }
        }
        internal void InvalidateItemLayout(string affectedProperty)
        {
            InvalidateItemLayout(affectedProperty, /*invalidatePainting*/true);
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

        internal void InvokePaint()
        {
            if (ParentInternal != null)
            {
                ParentInternal.InvokePaintItem(this);
            }
        }

        protected internal virtual bool IsInputKey(Keys keyData)
        {
            return false;
        }

        protected internal virtual bool IsInputChar(char charCode)
        {
            return false;
        }

        //
        //  Private handlers which are the equivilant of Control.Wm<Message>
        //

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
        private void HandleDoubleClick(EventArgs e)
        {
            OnDoubleClick(e);
        }

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
                // on another.  We do need to be careful
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

        protected virtual void OnClick(EventArgs e)
        {
            RaiseEvent(s_clickEvent, e);
        }

        protected internal virtual void OnLayout(LayoutEventArgs e)
        {
        }

        ///
        ///  Explicit support of DropTarget
        ///
        void IDropTarget.OnDragEnter(DragEventArgs dragEvent)
        {
            OnDragEnter(dragEvent);
        }
        void IDropTarget.OnDragOver(DragEventArgs dragEvent)
        {
            OnDragOver(dragEvent);
        }
        void IDropTarget.OnDragLeave(EventArgs e)
        {
            OnDragLeave(e);
        }
        void IDropTarget.OnDragDrop(DragEventArgs dragEvent)
        {
            OnDragDrop(dragEvent);
        }
        ///
        ///  Explicit support of DropSource
        ///
        void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs)
        {
            OnGiveFeedback(giveFeedbackEventArgs);
        }
        void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs)
        {
            OnQueryContinueDrag(queryContinueDragEventArgs);
        }

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

        protected virtual void OnAvailableChanged(EventArgs e)
        {
            RaiseEvent(s_availableChangedEvent, e);
        }

        /// <summary>
        ///  Raises the <see cref='ToolStripItem.DragEnter'/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onEnter to send this event to any registered event listeners.
        /// </summary>
        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragEnter to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragEnter(DragEventArgs dragEvent)
        {
            RaiseDragEvent(s_dragEnterEvent, dragEvent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragOver to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragOver(DragEventArgs dragEvent)
        {
            RaiseDragEvent(s_dragOverEvent, dragEvent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragLeave to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragLeave(EventArgs e)
        {
            RaiseEvent(s_dragLeaveEvent, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragDrop to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragDrop(DragEventArgs dragEvent)
        {
            RaiseDragEvent(s_dragDropEvent, dragEvent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDisplayStyleChanged(EventArgs e)
        {
            RaiseEvent(s_displayStyleChangedEvent, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onGiveFeedback to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
// PM review done
        protected virtual void OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEvent)
        {
            ((GiveFeedbackEventHandler)Events[s_giveFeedbackEvent])?.Invoke(this, giveFeedbackEvent);
        }

        internal virtual void OnImageScalingSizeChanged(EventArgs e)
        {
        }
        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onQueryContinueDrag to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
// PM review done
        protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEvent)
        {
            RaiseQueryContinueDragEvent(s_queryContinueDragEvent, queryContinueDragEvent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected virtual void OnDoubleClick(EventArgs e)
        {
            RaiseEvent(s_doubleClickEvent, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected virtual void OnEnabledChanged(EventArgs e)
        {
            RaiseEvent(s_enabledChangedEvent, e);
            Animate();
        }

        internal void OnInternalEnabledChanged(EventArgs e)
        {
            RaiseEvent(s_internalEnabledChangedEvent, e);
        }

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

        protected virtual void OnLocationChanged(EventArgs e)
        {
            RaiseEvent(s_locationChangedEvent, e);
        }

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
        protected virtual void OnMouseLeave(EventArgs e)
        {
            if (ParentInternal != null)
            {
                ParentInternal.UpdateToolTip(null);
            }
        }

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
        {
            OnEnabledChanged(EventArgs.Empty);
        }

        /// <summary>
        ///  Occurs when the font property has changed on the parent - used to notify inheritors of the font property that
        ///  the font has changed
            /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal protected virtual void OnOwnerFontChanged(EventArgs e)
        {
            if (Properties.GetObject(s_fontProperty) == null)
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
                Owner.OnItemVisibleChanged(new ToolStripItemEventArgs(this), /*performLayout*/true);
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
            //
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
        protected internal virtual bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            return false;
        }

        protected internal virtual bool ProcessMnemonic(char charCode)
        {
            // checking IsMnemonic is not necessary - control does this for us.
            FireEvent(ToolStripItemEventType.Click);
            return true;
        }

        internal void RaiseCancelEvent(object key, CancelEventArgs e)
        {
            ((CancelEventHandler)Events[key])?.Invoke(this, e);
        }

        internal void RaiseDragEvent(object key, DragEventArgs e)
        {
            ((DragEventHandler)Events[key])?.Invoke(this, e);
        }
        internal void RaiseEvent(object key, EventArgs e)
        {
            ((EventHandler)Events[key])?.Invoke(this, e);
        }
        internal void RaiseKeyEvent(object key, KeyEventArgs e)
        {
            ((KeyEventHandler)Events[key])?.Invoke(this, e);
        }
        internal void RaiseKeyPressEvent(object key, KeyPressEventArgs e)
        {
            ((KeyPressEventHandler)Events[key])?.Invoke(this, e);
        }
        internal void RaiseMouseEvent(object key, MouseEventArgs e)
        {
            ((MouseEventHandler)Events[key])?.Invoke(this, e);
        }
        internal void RaisePaintEvent(object key, PaintEventArgs e)
        {
            ((PaintEventHandler)Events[key])?.Invoke(this, e);
        }

        internal void RaiseQueryContinueDragEvent(object key, QueryContinueDragEventArgs e)
        {
            ((QueryContinueDragEventHandler)Events[key])?.Invoke(this, e);
        }

        private void ResetToolTipText()
        {
            _toolTipText = null;
        }

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
            if (ToolStrip.SelectionDebug.TraceVerbose)
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
                Debug.WriteLineIf(ToolStrip.SelectionDebug.TraceVerbose, "[Selection DBG] BAILING, selection is currently suspended");
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

                if (AccessibilityObject is ToolStripItemAccessibleObject)
                {
                    ((ToolStripItemAccessibleObject)AccessibilityObject).RaiseFocusChanged();
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
                    _owner.rescaleConstsCallbackDelegate -= ToolStrip_RescaleConstants;
                }
                _owner = newOwner;

                if (_owner != null)
                {
                    _owner.rescaleConstsCallbackDelegate += ToolStrip_RescaleConstants;
                }

                // clear the parent if the owner is null...
                //
                if (newOwner == null)
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
        internal protected virtual void SetBounds(Rectangle bounds)
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
        {
            SetBounds(new Rectangle(x, y, width, height));
        }

        /// <summary>
        ///  Sets the placement of the item
        /// </summary>
        internal void SetPlacement(ToolStripItemPlacement placement)
        {
            _placement = placement;
        }

        // Some implementations of DefaultMargin check which container they
        // are on.  They need to be re-evaluated when the containership changes.
        // DefaultMargin will stop being honored the moment someone sets the Margin property.
        internal void SetAmbientMargin()
        {
            if (_state[s_stateUseAmbientMargin] && Margin != DefaultMargin)
            {
                CommonProperties.SetMargin(this, DefaultMargin);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageTransparentColor()
        {
            return ImageTransparentColor != Color.Empty;
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

        private bool ShouldSerializeDisplayStyle()
        {
            return DisplayStyle != DefaultDisplayStyle;
        }

        private bool ShouldSerializeToolTipText()
        {
            return !string.IsNullOrEmpty(_toolTipText);
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
            object font = Properties.GetObject(s_fontProperty, out bool found);
            return (found && font != null);
        }

        /// <summary>
        ///  Determines if the <see cref='Padding'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePadding()
        {
            return (Padding != DefaultPadding);
        }

        /// <summary>
        ///  Determines if the <see cref='Margin'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMargin()
        {
            return (Margin != DefaultMargin);
        }

        /// <summary>
        ///  Determines if the <see cref='Visible'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeVisible()
        {
            return !_state[s_stateVisible]; // only serialize if someone turned off visiblilty
        }

        /// <summary>
        ///  Determines if the <see cref='Image'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImage()
        {
            return (Image != null) && (ImageIndexer.ActualIndex < 0);
        }

        /// <summary>
        ///  Determines if the <see cref='Image'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageKey()
        {
            return (Image != null) && (ImageIndexer.ActualIndex >= 0) && (ImageIndexer.Key != null && ImageIndexer.Key.Length != 0);
        }

        /// <summary>
        ///  Determines if the <see cref='Image'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageIndex()
        {
            return (Image != null) && (ImageIndexer.ActualIndex >= 0) && (ImageIndexer.Index != -1);
        }

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
            return (rightToLeft != (int)DefaultRightToLeft);
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
        public virtual void ResetBackColor()
        {
            BackColor = Color.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetDisplayStyle()
        {
            DisplayStyle = DefaultDisplayStyle;
        }

        /// <summary>
        ///  Resets the fore color to be based on the parent's fore color.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetForeColor()
        {
            ForeColor = Color.Empty;
        }

        /// <summary>
        ///  Resets the Font to be based on the parent's Font.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetFont()
        {
            Font = null;
        }

        /// <summary>
        ///  Resets the back color to be based on the parent's back color.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetImage()
        {
            Image = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ResetImageTransparentColor()
        {
            ImageTransparentColor = Color.Empty;
        }

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
        public virtual void ResetRightToLeft()
        {
            RightToLeft = RightToLeft.Inherit;
        }

        /// <summary>
        ///  Resets the TextDirection to be the default.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetTextDirection()
        {
            TextDirection = ToolStripTextDirection.Inherit;
        }

        /// <summary>
        ///  Translates a point from one coordinate system to another
        /// </summary>
        internal Point TranslatePoint(Point fromPoint, ToolStripPointType fromPointType, ToolStripPointType toPointType)
        {
            ToolStrip parent = ParentInternal;

            if (parent == null)
            {
                parent = (IsOnOverflow && Owner != null) ? Owner.OverflowButton.DropDown : Owner;
            }
            if (parent == null)
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

        /// <summary>
        ///  ToString support
        /// </summary>
        public override string ToString()
        {
            if (Text != null && Text.Length != 0)
            {
                return Text;
            }
            return base.ToString();
        }

        /// <summary>
        ///  removes selection bits from item state
        /// </summary>
        internal void Unselect()
        {
            Debug.WriteLineIf(ToolStrip.SelectionDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "[Selection DBG] WBI.Unselect: {0}", ToString()));
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

        #region IKeyboardToolTip implementation

        bool IKeyboardToolTip.CanShowToolTipsNow()
        {
            return Visible && _parent != null && ((IKeyboardToolTip)_parent).AllowsChildrenToShowToolTips();
        }

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle()
        {
            return AccessibilityObject.Bounds;
        }

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
        {
            return ((IKeyboardToolTip)this).GetNativeScreenRectangle().Contains(Control.MousePosition);
        }

        bool IKeyboardToolTip.HasRtlModeEnabled()
        {
            return _parent != null && ((IKeyboardToolTip)_parent).HasRtlModeEnabled();
        }

        bool IKeyboardToolTip.AllowsToolTip()
        {
            return true;
        }

        IWin32Window IKeyboardToolTip.GetOwnerWindow()
        {
            Debug.Assert(ParentInternal != null, "Tool Strip Item Parent is null");
            return ParentInternal;
        }

        void IKeyboardToolTip.OnHooked(ToolTip toolTip)
        {
            OnKeyboardToolTipHook(toolTip);
        }

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip)
        {
            OnKeyboardToolTipUnhook(toolTip);
        }

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip)
        {
            return ToolTipText;
        }

        bool IKeyboardToolTip.ShowsOwnToolTip()
        {
            return true;
        }

        bool IKeyboardToolTip.IsBeingTabbedTo()
        {
            return IsBeingTabbedTo();
        }

        bool IKeyboardToolTip.AllowsChildrenToShowToolTips()
        {
            return true;
        }

        #endregion

        internal virtual void OnKeyboardToolTipHook(ToolTip toolTip)
        {
        }

        internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip)
        {
        }

        internal virtual bool IsBeingTabbedTo()
        {
            return ToolStrip.AreCommonNavigationalKeysDown();
        }

        /// <summary>
        ///  An implementation of AccessibleChild for use with ToolStripItems
        /// </summary>
        [Runtime.InteropServices.ComVisible(true)]
        public class ToolStripItemAccessibleObject : AccessibleObject
        {
            private readonly ToolStripItem _ownerItem; // The associated ToolStripItem for this AccessibleChild (if any)

            private AccessibleStates _additionalState = AccessibleStates.None; // Test hook for the designer

            private int[] _runtimeId;

            public ToolStripItemAccessibleObject(ToolStripItem ownerItem)
            {
                _ownerItem = ownerItem ?? throw new ArgumentNullException(nameof(ownerItem));
            }

            public override string DefaultAction
            {
                get
                {
                    string defaultAction = _ownerItem.AccessibleDefaultActionDescription;
                    if (defaultAction != null)
                    {
                        return defaultAction;
                    }
                    else
                    {
                        return SR.AccessibleActionPress;
                    }
                }
            }

            public override string Description
            {
                get
                {
                    string description = _ownerItem.AccessibleDescription;
                    if (description != null)
                    {
                        return description;
                    }
                    else
                    {
                        return base.Description;
                    }
                }
            }

            public override string Help
            {
                get
                {
                    QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Owner.Events[ToolStripItem.s_queryAccessibilityHelpEvent];

                    if (handler != null)
                    {
                        QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                        handler(Owner, args);
                        return args.HelpString;
                    }
                    else
                    {
                        return base.Help;
                    }
                }
            }

            public override string KeyboardShortcut
            {
                get
                {
                    // This really is the Mnemonic - NOT the shortcut.  E.g. in notepad Edit->Replace is Control+H
                    // but the KeyboardShortcut comes up as the mnemonic 'r'.
                    char mnemonic = WindowsFormsUtils.GetMnemonic(_ownerItem.Text, false);
                    if (_ownerItem.IsOnDropDown)
                    {
                        // no ALT on dropdown
                        return (mnemonic == (char)0) ? string.Empty : mnemonic.ToString();
                    }
                    return (mnemonic == (char)0) ? string.Empty : ("Alt+" + mnemonic);
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (_runtimeId == null)
                    {
                        // we need to provide a unique ID
                        // others are implementing this in the same manner
                        // first item should be UiaAppendRuntimeId since this is not a top-level element of the fragment.
                        // second item can be anything, but here it is a hash. For toolstrip hash is unique even with child controls. Hwnd  is not.

                        _runtimeId = new int[2];
                        _runtimeId[0] = NativeMethods.UiaAppendRuntimeId;
                        _runtimeId[1] = _ownerItem.GetHashCode();
                    }

                    return _runtimeId;
                }
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId:
                        return (object)IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId);
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _ownerItem.Enabled;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return _ownerItem.Placement != ToolStripItemPlacement.Main;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return _ownerItem.CanSelect;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _ownerItem.Selected;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return KeyboardShortcut;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override string Name
            {
                get
                {
                    string name = _ownerItem.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }

                    string baseName = base.Name;

                    if (baseName == null || baseName.Length == 0)
                    {
                        return WindowsFormsUtils.TextWithoutMnemonics(_ownerItem.Text);
                    }

                    return baseName;
                }
                set
                {
                    _ownerItem.AccessibleName = value;
                }
            }

            internal ToolStripItem Owner
            {
                get
                {
                    return _ownerItem;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = _ownerItem.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    else
                    {
                        return AccessibleRole.PushButton;
                    }
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (!_ownerItem.CanSelect)
                    {
                        return base.State | _additionalState;
                    }

                    if (!_ownerItem.Enabled)
                    {
                        if (_ownerItem.Selected && _ownerItem is ToolStripMenuItem)
                        {
                            return AccessibleStates.Unavailable | _additionalState | AccessibleStates.Focused;
                        }

                        // Disabled menu items that are selected must have focus
                        // state so that Narrator can announce them.
                        if (_ownerItem.Selected && _ownerItem is ToolStripMenuItem)
                        {
                            return AccessibleStates.Focused;
                        }

                        return AccessibleStates.Unavailable | _additionalState;
                    }

                    AccessibleStates accState = AccessibleStates.Focusable | _additionalState;
                    if (_ownerItem.Selected || _ownerItem.Pressed)
                    {
                        accState |= AccessibleStates.Focused | AccessibleStates.HotTracked;
                    }
                    if (_ownerItem.Pressed)
                    {
                        accState |= AccessibleStates.Pressed;
                    }
                    return accState;
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner != null)
                {
                    ((ToolStripItem)Owner).PerformClick();
                }
            }
            public override int GetHelpTopic(out string fileName)
            {
                int topic = 0;

                QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Owner.Events[ToolStripItem.s_queryAccessibilityHelpEvent];

                if (handler != null)
                {
                    QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                    handler(Owner, args);

                    fileName = args.HelpNamespace;

                    try
                    {
                        topic = int.Parse(args.HelpKeyword, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                    }

                    return topic;
                }
                else
                {
                    return base.GetHelpTopic(out fileName);
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                ToolStripItem nextItem = null;

                if (Owner != null)
                {
                    ToolStrip parent = Owner.ParentInternal;
                    if (parent == null)
                    {
                        return null;
                    }

                    bool forwardInCollection = (parent.RightToLeft == RightToLeft.No);
                    switch (navigationDirection)
                    {
                        case AccessibleNavigation.FirstChild:
                            nextItem = parent.GetNextItem(null, ArrowDirection.Right, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.LastChild:
                            nextItem = parent.GetNextItem(null, ArrowDirection.Left,/*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Previous:
                        case AccessibleNavigation.Left:
                            nextItem = parent.GetNextItem(Owner, ArrowDirection.Left, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Next:
                        case AccessibleNavigation.Right:
                            nextItem = parent.GetNextItem(Owner, ArrowDirection.Right, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Up:
                            nextItem = (Owner.IsOnDropDown) ? parent.GetNextItem(Owner, ArrowDirection.Up) :
                                                               parent.GetNextItem(Owner, ArrowDirection.Left, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Down:
                            nextItem = (Owner.IsOnDropDown) ? parent.GetNextItem(Owner, ArrowDirection.Down) :
                                                               parent.GetNextItem(Owner, ArrowDirection.Right, /*RTLAware=*/true);
                            break;
                    }
                }
                if (nextItem != null)
                {
                    return nextItem.AccessibilityObject;
                }
                return null;
            }

            public void AddState(AccessibleStates state)
            {
                if (state == AccessibleStates.None)
                {
                    _additionalState = state;
                }
                else
                {
                    _additionalState |= state;
                }
            }

            public override string ToString()
            {
                if (Owner != null)
                {
                    return "ToolStripItemAccessibleObject: Owner = " + Owner.ToString();
                }
                else
                {
                    return "ToolStripItemAccessibleObject: Owner = null";
                }
            }

            /// <summary>
            ///  Gets the bounds of the accessible object, in screen coordinates.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    Rectangle bounds = Owner.Bounds;
                    if (Owner.ParentInternal != null &&
                        Owner.ParentInternal.Visible)
                    {
                        return new Rectangle(Owner.ParentInternal.PointToScreen(bounds.Location), bounds.Size);
                    }
                    return Rectangle.Empty;
                }
            }

            /// <summary>
            ///  When overridden in a derived class, gets or sets the parent of an accessible object.
            /// </summary>
            public override AccessibleObject Parent
            {
                get
                {
                    if (Owner.IsOnDropDown)
                    {
                        // Return the owner item as the accessible parent.
                        ToolStripDropDown dropDown = Owner.GetCurrentParentDropDown();
                        return dropDown.AccessibilityObject;
                    }
                    return (Owner.Parent != null) ? Owner.Parent.AccessibilityObject : base.Parent;
                }
            }

            /// <summary>
            ///  Gets the top level element.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _ownerItem.RootToolStrip?.AccessibilityObject;
                }
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                    case UiaCore.NavigateDirection.PreviousSibling:
                        int index = GetChildFragmentIndex();
                        if (index == -1)
                        {
                            Debug.Fail("No item matched the index?");
                            return null;
                        }

                        int increment = direction == UiaCore.NavigateDirection.NextSibling ? 1 : -1;
                        AccessibleObject sibling = null;

                        index += increment;
                        int itemsCount = GetChildFragmentCount();
                        if (index >= 0 && index < itemsCount)
                        {
                            sibling = GetChildFragment(index);
                        }

                        return sibling;
                }

                return base.FragmentNavigate(direction);
            }

            private AccessibleObject GetChildFragment(int index)
            {
                if (Parent is ToolStrip.ToolStripAccessibleObject toolStripParent)
                {
                    return toolStripParent.GetChildFragment(index);
                }

                // ToolStripOverflowButtonAccessibleObject is derived from ToolStripDropDownItemAccessibleObject
                // and we should not process ToolStripOverflowButton as a ToolStripDropDownItem here so check for
                // the ToolStripOverflowButton firstly as more specific condition.
                if (Parent is ToolStripOverflowButton.ToolStripOverflowButtonAccessibleObject toolStripOverflowButtonParent)
                {
                    if (toolStripOverflowButtonParent.Parent is ToolStrip.ToolStripAccessibleObject toolStripGrandParent)
                    {
                        return toolStripGrandParent.GetChildFragment(index, true);
                    }
                }

                if (Parent is ToolStripDropDownItemAccessibleObject dropDownItemParent)
                {
                    return dropDownItemParent.GetChildFragment(index);
                }

                return null;
            }

            private int GetChildFragmentCount()
            {
                if (Parent is ToolStrip.ToolStripAccessibleObject toolStripParent)
                {
                    return toolStripParent.GetChildFragmentCount();
                }

                if (Parent is ToolStripOverflowButton.ToolStripOverflowButtonAccessibleObject toolStripOverflowButtonParent)
                {
                    if (toolStripOverflowButtonParent.Parent is ToolStrip.ToolStripAccessibleObject toolStripGrandParent)
                    {
                        return toolStripGrandParent.GetChildOverflowFragmentCount();
                    }
                }

                if (Parent is ToolStripDropDownItemAccessibleObject dropDownItemParent)
                {
                    return dropDownItemParent.GetChildCount();
                }

                return -1;
            }

            private int GetChildFragmentIndex()
            {
                if (Parent is ToolStrip.ToolStripAccessibleObject toolStripParent)
                {
                    return toolStripParent.GetChildFragmentIndex(this);
                }

                if (Parent is ToolStripOverflowButton.ToolStripOverflowButtonAccessibleObject toolStripOverflowButtonParent)
                {
                    if (toolStripOverflowButtonParent.Parent is ToolStrip.ToolStripAccessibleObject toolStripGrandParent)
                    {
                        return toolStripGrandParent.GetChildFragmentIndex(this);
                    }
                }

                if (Parent is ToolStripDropDownItemAccessibleObject dropDownItemParent)
                {
                    return dropDownItemParent.GetChildFragmentIndex(this);
                }

                return -1;
            }

            internal override void SetFocus()
            {
                Owner.Select();
            }

            internal void RaiseFocusChanged()
            {
                ToolStrip root = _ownerItem.RootToolStrip;
                if (root != null && root.SupportsUiaProviders)
                {
                    RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                }
            }
        }
    }

    // We need a special way to defer to the ToolStripItem's image
    // list for indexing purposes.
    internal class ToolStripItemImageIndexer : ImageList.Indexer
    {
        private readonly ToolStripItem _item;

        public ToolStripItemImageIndexer(ToolStripItem item)
        {
            _item = item;
        }

        public override ImageList ImageList
        {
            get
            {
                if ((_item != null) && (_item.Owner != null))
                {
                    return _item.Owner.ImageList;
                }
                return null;
            }
            set { Debug.Assert(false, "We should never set the image list"); }
        }
    }

    /// <summary>
    ///  This class helps determine where the image and text should be drawn.
    /// </summary>
    internal class ToolStripItemInternalLayout
    {
        private ToolStripItemLayoutOptions _currentLayoutOptions;
        private readonly ToolStripItem _ownerItem;
        private ButtonBaseAdapter.LayoutData _layoutData;
        private const int BorderWidth = 2;
        private const int BorderHeight = 3;
        private readonly static Size s_invalidSize = new Size(int.MinValue, int.MinValue);

        private Size _lastPreferredSize = s_invalidSize;
        private ToolStripLayoutData _parentLayoutData;

        public ToolStripItemInternalLayout(ToolStripItem ownerItem)
        {
            this._ownerItem = ownerItem ?? throw new ArgumentNullException(nameof(ownerItem));
        }

        // the thing that we fetch properties off of -- this can be different than ownerItem - e.g. case of split button.
        protected virtual ToolStripItem Owner
        {
            get { return _ownerItem; }
        }

        public virtual Rectangle ImageRectangle
        {
            get
            {
                Rectangle imageRect = LayoutData.imageBounds;
                imageRect.Intersect(_layoutData.field);
                return imageRect;
            }
        }

        internal ButtonBaseAdapter.LayoutData LayoutData
        {
            get
            {
                EnsureLayout();
                return _layoutData;
            }
        }

        public Size PreferredImageSize
        {
            get
            {
                return Owner.PreferredImageSize;
            }
        }

        protected virtual ToolStrip ParentInternal
        {
            get
            {
                return _ownerItem?.ParentInternal;
            }
        }

        public virtual Rectangle TextRectangle
        {
            get
            {
                Rectangle textRect = LayoutData.textBounds;
                textRect.Intersect(_layoutData.field);
                return textRect;
            }
        }

        public virtual Rectangle ContentRectangle
        {
            get
            {
                return LayoutData.field;
            }
        }

        public virtual TextFormatFlags TextFormat
        {
            get
            {
                if (_currentLayoutOptions != null)
                {
                    return _currentLayoutOptions.gdiTextFormatFlags;
                }
                return CommonLayoutOptions().gdiTextFormatFlags;
            }
        }

        internal static TextFormatFlags ContentAlignToTextFormat(ContentAlignment alignment, bool rightToLeft)
        {
            TextFormatFlags textFormat = TextFormatFlags.Default;
            if (rightToLeft)
            {
                //We specifically do not want to turn on TextFormatFlags.Right.
                textFormat |= TextFormatFlags.RightToLeft;
            }

            // Calculate Text Positioning
            textFormat |= ControlPaint.TranslateAlignmentForGDI(alignment);
            textFormat |= ControlPaint.TranslateLineAlignmentForGDI(alignment);
            return textFormat;
        }

        protected virtual ToolStripItemLayoutOptions CommonLayoutOptions()
        {
            ToolStripItemLayoutOptions layoutOptions = new ToolStripItemLayoutOptions();
            Rectangle bounds = new Rectangle(Point.Empty, _ownerItem.Size);

            layoutOptions.client = bounds;

            layoutOptions.growBorderBy1PxWhenDefault = false;

            layoutOptions.borderSize = BorderWidth;
            layoutOptions.paddingSize = 0;
            layoutOptions.maxFocus = true;
            layoutOptions.focusOddEvenFixup = false;
            layoutOptions.font = _ownerItem.Font;
            layoutOptions.text = ((Owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) ? Owner.Text : string.Empty;
            layoutOptions.imageSize = PreferredImageSize;
            layoutOptions.checkSize = 0;
            layoutOptions.checkPaddingSize = 0;
            layoutOptions.checkAlign = ContentAlignment.TopLeft;
            layoutOptions.imageAlign = Owner.ImageAlign;
            layoutOptions.textAlign = Owner.TextAlign;
            layoutOptions.hintTextUp = false;
            layoutOptions.shadowedText = !_ownerItem.Enabled;
            layoutOptions.layoutRTL = RightToLeft.Yes == Owner.RightToLeft;
            layoutOptions.textImageRelation = Owner.TextImageRelation;
            //set textImageInset to 0 since we don't draw 3D border for ToolStripItems.
            layoutOptions.textImageInset = 0;
            layoutOptions.everettButtonCompat = false;

            // Support RTL
            layoutOptions.gdiTextFormatFlags = ContentAlignToTextFormat(Owner.TextAlign, Owner.RightToLeft == RightToLeft.Yes);

            // Hide underlined &File unless ALT is pressed
            layoutOptions.gdiTextFormatFlags = (Owner.ShowKeyboardCues) ? layoutOptions.gdiTextFormatFlags : layoutOptions.gdiTextFormatFlags | TextFormatFlags.HidePrefix;

            return layoutOptions;
        }

        private bool EnsureLayout()
        {
            if (_layoutData == null || _parentLayoutData == null || !_parentLayoutData.IsCurrent(ParentInternal))
            {
                PerformLayout();
                return true;
            }
            return false;
        }

        private ButtonBaseAdapter.LayoutData GetLayoutData()
        {
            _currentLayoutOptions = CommonLayoutOptions();

            if (Owner.TextDirection != ToolStripTextDirection.Horizontal)
            {
                _currentLayoutOptions.verticalText = true;
            }

            ButtonBaseAdapter.LayoutData data = _currentLayoutOptions.Layout();
            return data;
        }
        public virtual Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = Size.Empty;
            EnsureLayout();
            // we would prefer not to be larger than the ToolStrip itself.
            // so we'll ask the ButtonAdapter layout guy what it thinks
            // its preferred size should be - and we'll tell it to be no
            // bigger than the ToolStrip itself.  Note this is "Parent" not
            // "Owner" because we care in this instance what we're currently displayed on.

            if (_ownerItem != null)
            {
                _lastPreferredSize = _currentLayoutOptions.GetPreferredSizeCore(constrainingSize);
                return _lastPreferredSize;
            }
            return Size.Empty;
        }

        internal void PerformLayout()
        {
            _layoutData = GetLayoutData();
            ToolStrip parent = ParentInternal;
            if (parent != null)
            {
                _parentLayoutData = new ToolStripLayoutData(parent);
            }
            else
            {
                _parentLayoutData = null;
            }
        }

        internal class ToolStripItemLayoutOptions : ButtonBaseAdapter.LayoutOptions
        {
            Size cachedSize = LayoutUtils.InvalidSize;
            Size cachedProposedConstraints = LayoutUtils.InvalidSize;

            // override GetTextSize to provide simple text caching.
            protected override Size GetTextSize(Size proposedConstraints)
            {
                if (cachedSize != LayoutUtils.InvalidSize
                    && (cachedProposedConstraints == proposedConstraints
                       || cachedSize.Width <= proposedConstraints.Width))
                {
                    return cachedSize;
                }
                else
                {
                    cachedSize = base.GetTextSize(proposedConstraints);
                    cachedProposedConstraints = proposedConstraints;
                }
                return cachedSize;
            }
        }
        private class ToolStripLayoutData
        {
            private readonly ToolStripLayoutStyle layoutStyle;
            private readonly bool autoSize;
            private Size size;

            public ToolStripLayoutData(ToolStrip toolStrip)
            {
                layoutStyle = toolStrip.LayoutStyle;
                autoSize = toolStrip.AutoSize;
                size = toolStrip.Size;
            }
            public bool IsCurrent(ToolStrip toolStrip)
            {
                if (toolStrip == null)
                {
                    return false;
                }
                return (toolStrip.Size == size && toolStrip.LayoutStyle == layoutStyle && toolStrip.AutoSize == autoSize);
            }
        }
    }
}
