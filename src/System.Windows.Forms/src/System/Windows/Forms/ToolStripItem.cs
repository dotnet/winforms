// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define SELECTEDCHANGED


namespace System.Windows.Forms {
    using System;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms.ButtonInternal;
    using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Layout;
    using System.Collections.Specialized;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Security;
    using System.Configuration;        
    using System.Drawing.Imaging;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.Versioning;
    using Collections.Generic;

    /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem"]/*' />
    /// <devdoc/>
    [System.ComponentModel.DesignTimeVisible(false)]
    [Designer("System.Windows.Forms.Design.ToolStripItemDesigner, " + AssemblyRef.SystemDesign)]
    [DefaultEvent(nameof(Click))]
    [ToolboxItem(false)]
    [DefaultProperty(nameof(Text))]
    public abstract class ToolStripItem : Component, 
                              IDropTarget, 
                              ISupportOleDropSource,
                              IArrangedElement,
                              IKeyboardToolTip {
                                          
        

    
#if DEBUG        
        internal static readonly TraceSwitch MouseDebugging                 = new TraceSwitch("MouseDebugging", "Debug ToolStripItem mouse debugging code");
#else
        internal static readonly TraceSwitch MouseDebugging;
#endif
        
        private Rectangle                      bounds                              = Rectangle.Empty;
        private PropertyStore                  propertyStore;
        private ToolStripItemAlignment         alignment                           = ToolStripItemAlignment.Left;
        private ToolStrip                      parent                              = null;
        private ToolStrip                      owner                               = null;
        private ToolStripItemOverflow          overflow                            = ToolStripItemOverflow.AsNeeded;
        private ToolStripItemPlacement         placement                           = ToolStripItemPlacement.None;
        private ContentAlignment               imageAlign                          = ContentAlignment.MiddleCenter;
        private ContentAlignment               textAlign                           = ContentAlignment.MiddleCenter;
        private TextImageRelation              textImageRelation                   = TextImageRelation.ImageBeforeText;
        private ToolStripItemImageIndexer      imageIndexer                        = null;
        private ToolStripItemInternalLayout    toolStripItemInternalLayout         = null;
        private BitVector32                    state                               = new BitVector32();
        private string                         toolTipText                         = null;
        private Color                          imageTransparentColor               = Color.Empty;
        private ToolStripItemImageScaling imageScaling = ToolStripItemImageScaling.SizeToFit;
        private Size                           cachedTextSize                      = Size.Empty;
              
        private static readonly Padding        defaultMargin                       = new Padding(0, 1, 0, 2);
        private static readonly Padding        defaultStatusStripMargin            = new Padding(0, 2, 0, 0);
        private Padding                        scaledDefaultMargin                 = defaultMargin;
        private Padding                        scaledDefaultStatusStripMargin      = defaultStatusStripMargin;

        private ToolStripItemDisplayStyle      displayStyle                        = ToolStripItemDisplayStyle.ImageAndText;

        private static readonly ArrangedElementCollection EmptyChildCollection  = new ArrangedElementCollection();


        /// 
        ///   Adding a new event??  Make sure you dont need to add to ToolStripControlHost.cs 
        ///
        internal static readonly object EventMouseDown                       = new object();
        internal static readonly object EventMouseEnter                      = new object();
        internal static readonly object EventMouseLeave                      = new object();
        internal static readonly object EventMouseHover                      = new object();
        internal static readonly object EventMouseMove                       = new object();
        internal static readonly object EventMouseUp                         = new object();
        internal static readonly object EventMouseWheel                      = new object();
        internal static readonly object EventClick                           = new object();
        internal static readonly object EventDoubleClick                     = new object();
        internal static readonly object EventDragDrop                        = new object();
        internal static readonly object EventDragEnter                       = new object();
        internal static readonly object EventDragLeave                       = new object();
        internal static readonly object EventDragOver                        = new object();
        internal static readonly object EventDisplayStyleChanged             = new object();
        internal static readonly object EventEnabledChanged                  = new object();
        internal static readonly object EventInternalEnabledChanged          = new object();
        internal static readonly object EventFontChanged                     = new object();
        internal static readonly object EventForeColorChanged                = new object();
        internal static readonly object EventBackColorChanged                = new object();
        internal static readonly object EventGiveFeedback                    = new object();
        internal static readonly object EventQueryContinueDrag               = new object();
        internal static readonly object EventQueryAccessibilityHelp          = new object();
        internal static readonly object EventMove                            = new object();
        internal static readonly object EventResize                          = new object();
        internal static readonly object EventLayout                          = new object();
        internal static readonly object EventLocationChanged                 = new object();
        internal static readonly object EventRightToLeft                     = new object();
        internal static readonly object EventVisibleChanged                  = new object();
        internal static readonly object EventAvailableChanged                  = new object();        
        internal static readonly object EventOwnerChanged                    = new object();
        internal static readonly object EventPaint                           = new object();
        internal static readonly object EventText                            = new object();
        internal static readonly object EventSelectedChanged                 = new object();
        
        /// 
        ///   Adding a new event??  Make sure you dont need to add to ToolStripControlHost.cs 
        ///
        

        // Property store keys for properties.  The property store allocates most efficiently
        // in groups of four, so we try to lump properties in groups of four based on how
        // likely they are going to be used in a group.

        private static readonly int PropName                                = PropertyStore.CreateKey();
        private static readonly int PropText                                = PropertyStore.CreateKey();
        private static readonly int PropBackColor                           = PropertyStore.CreateKey();
        private static readonly int PropForeColor                           = PropertyStore.CreateKey();

        private static readonly int PropImage                               = PropertyStore.CreateKey();       
        private static readonly int PropFont                                = PropertyStore.CreateKey();
        private static readonly int PropRightToLeft                         = PropertyStore.CreateKey();
        private static readonly int PropTag                                 = PropertyStore.CreateKey();
        
        private static readonly int PropAccessibility                       = PropertyStore.CreateKey();
        private static readonly int PropAccessibleName                      = PropertyStore.CreateKey();
        private static readonly int PropAccessibleRole                      = PropertyStore.CreateKey();
        private static readonly int PropAccessibleHelpProvider              = PropertyStore.CreateKey();
        
        private static readonly int PropAccessibleDefaultActionDescription  = PropertyStore.CreateKey();
        private static readonly int PropAccessibleDescription               = PropertyStore.CreateKey();        
        private static readonly int PropTextDirection                       = PropertyStore.CreateKey();
        private static readonly int PropMirroredImage                       = PropertyStore.CreateKey();
        

        private static readonly int PropBackgroundImage                     = PropertyStore.CreateKey();
        private static readonly int PropBackgroundImageLayout               = PropertyStore.CreateKey();
            
        private static readonly int PropMergeAction                         = PropertyStore.CreateKey();
        private static readonly int PropMergeIndex                          = PropertyStore.CreateKey();

      
        private static readonly int stateAllowDrop          = BitVector32.CreateMask();
        private static readonly int stateVisible            = BitVector32.CreateMask(stateAllowDrop);
        private static readonly int stateEnabled            = BitVector32.CreateMask(stateVisible);
        private static readonly int stateMouseDownAndNoDrag = BitVector32.CreateMask(stateEnabled);
        private static readonly int stateAutoSize           = BitVector32.CreateMask(stateMouseDownAndNoDrag);
        private static readonly int statePressed            = BitVector32.CreateMask(stateAutoSize);
        private static readonly int stateSelected           = BitVector32.CreateMask(statePressed);
        private static readonly int stateContstructing      = BitVector32.CreateMask(stateSelected);
        private static readonly int stateDisposed           = BitVector32.CreateMask(stateContstructing);
        private static readonly int stateCurrentlyAnimatingImage = BitVector32.CreateMask(stateDisposed);
        private static readonly int stateDoubleClickEnabled = BitVector32.CreateMask(stateCurrentlyAnimatingImage);
        private static readonly int stateAutoToolTip        = BitVector32.CreateMask(stateDoubleClickEnabled);
        private static readonly int stateSupportsRightClick = BitVector32.CreateMask(stateAutoToolTip);
        private static readonly int stateSupportsItemClick = BitVector32.CreateMask(stateSupportsRightClick);
        private static readonly int stateRightToLeftAutoMirrorImage = BitVector32.CreateMask(stateSupportsItemClick);
        private static readonly int stateInvalidMirroredImage = BitVector32.CreateMask(stateRightToLeftAutoMirrorImage);
        private static readonly int stateSupportsSpaceKey   = BitVector32.CreateMask(stateInvalidMirroredImage);        
        private static readonly int stateMouseDownAndUpMustBeInSameItem =  BitVector32.CreateMask(stateSupportsSpaceKey); 
        private static readonly int stateSupportsDisabledHotTracking =  BitVector32.CreateMask(stateMouseDownAndUpMustBeInSameItem); 
        private static readonly int stateUseAmbientMargin = BitVector32.CreateMask(stateSupportsDisabledHotTracking); 
        private static readonly int stateDisposing          =  BitVector32.CreateMask(stateUseAmbientMargin); 

        
        private long lastClickTime = 0;
     
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ToolStripItem"]/*' />
        /// <devdoc>
        /// Constructor
        /// </devdoc>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected ToolStripItem() {
            if (DpiHelper.IsScalingRequirementMet) {
                scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(defaultMargin);
                scaledDefaultStatusStripMargin = DpiHelper.LogicalToDeviceUnits(defaultStatusStripMargin);
            }

            state[stateEnabled | stateAutoSize | stateVisible | stateContstructing | stateSupportsItemClick | stateInvalidMirroredImage | stateMouseDownAndUpMustBeInSameItem | stateUseAmbientMargin] = true;  
            state[stateAllowDrop | stateMouseDownAndNoDrag | stateSupportsRightClick | statePressed | stateSelected | stateDisposed | stateDoubleClickEnabled | stateRightToLeftAutoMirrorImage | stateSupportsSpaceKey] = false;
            SetAmbientMargin();
            this.Size = DefaultSize;
            this.DisplayStyle = DefaultDisplayStyle;
            CommonProperties.SetAutoSize(this,true);
            state[stateContstructing] = false;
            this.AutoToolTip = DefaultAutoToolTip;
           
        }

        
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected ToolStripItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, null) {
        }
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected ToolStripItem(string text, Image image, EventHandler onClick, string name) : this() {
            this.Text = text;
            this.Image = image;
            if (onClick != null) {
                Click += onClick;
            }
            this.Name = name;
        }
          
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AccessibilityObject"]/*' />
        /// <devdoc>
        /// The Accessibility Object for this Control
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ToolStripItemAccessibilityObjectDescr))
        ]
        public AccessibleObject AccessibilityObject {
            get {
                AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(PropAccessibility);
                if (accessibleObject == null) {
                    accessibleObject = CreateAccessibilityInstance();
                    Properties.SetObject(PropAccessibility, accessibleObject);
                }
                
                Debug.Assert(accessibleObject != null, "Failed to create accessibility object");
                return accessibleObject;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AccessibleDefaultActionDescription"]/*' />
        /// <devdoc>
        /// The default action description of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ToolStripItemAccessibleDefaultActionDescr))
        ]
        public string AccessibleDefaultActionDescription {
            get {
                return (string)Properties.GetObject(PropAccessibleDefaultActionDescription);
            }
            set {
                Properties.SetObject(PropAccessibleDefaultActionDescription, value);
                OnAccessibleDefaultActionDescriptionChanged(EventArgs.Empty);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AccessibleDescription"]/*' />
        /// <devdoc>
        /// The accessible description of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemAccessibleDescriptionDescr))
        ]
        public string AccessibleDescription {
            get {
                return (string)Properties.GetObject(PropAccessibleDescription);
            }
            set {
                Properties.SetObject(PropAccessibleDescription, value);
                OnAccessibleDescriptionChanged(EventArgs.Empty);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AccessibleName"]/*' />
        /// <devdoc>
        /// The accessible name of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemAccessibleNameDescr))
        ]
        public string AccessibleName {
            get {
                return (string)Properties.GetObject(PropAccessibleName);
            }

            set {
                Properties.SetObject(PropAccessibleName, value);
                OnAccessibleNameChanged(EventArgs.Empty);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AccessibleRole"]/*' />
        /// <devdoc>
        /// The accessible role of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(AccessibleRole.Default),
        SRDescription(nameof(SR.ToolStripItemAccessibleRoleDescr))
        ]
        public AccessibleRole AccessibleRole {

            get {
                bool found;
                int role = Properties.GetInteger(PropAccessibleRole, out found);
                if (found) {
                    return (AccessibleRole)role;
                }
                else {
                    return AccessibleRole.Default;
                }
            }

            set {
                //valid values are 0xffffffff to 0x40
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AccessibleRole.Default, (int)AccessibleRole.OutlineButton)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AccessibleRole));
                }
                Properties.SetInteger(PropAccessibleRole, (int)value);
                OnAccessibleRoleChanged(EventArgs.Empty);
            }


        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Alignment"]/*' />
        /// <devdoc>
        /// Determines if the item aligns towards the beginning or end of the winbar.
        /// </devdoc>
        [
        DefaultValue(ToolStripItemAlignment.Left),
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.ToolStripItemAlignmentDescr))
        ]
        public ToolStripItemAlignment Alignment {
            get {
                return alignment;
            }
            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemAlignment.Left, (int)ToolStripItemAlignment.Right))
                {
                     throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemAlignment));
                }
                if (alignment != value) {
                    alignment = value;
                    
                    if ((this.ParentInternal != null) && this.ParentInternal.IsHandleCreated) {
                        this.ParentInternal.PerformLayout();
                    }
                }
            }
        }

       
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AllowDrop"]/*' />
        /// <devdoc>
        /// Determines if this item can be dragged.
        /// This is EXACTLY like Control.AllowDrop - setting this to true WILL call 
        /// the droptarget handlers.  The ToolStripDropTargetManager is the one that 
        /// handles the routing of DropTarget events to the ToolStripItem's IDropTarget
        /// methods.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatDragDrop)),
        DefaultValue(false),
        SRDescription(nameof(SR.ToolStripItemAllowDropDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public virtual bool AllowDrop {
            get {
                return state[stateAllowDrop];     
            }
            set {
                if (value !=  state[stateAllowDrop]) {
                    EnsureParentDropTargetRegistered();
                    state[stateAllowDrop] = value;
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AutoSize"]/*' />
        /// <devdoc>
        /// Determines whether we set the ToolStripItem to its preferred size
        /// </devdoc>
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatBehavior))]
        [RefreshProperties(RefreshProperties.All)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ToolStripItemAutoSizeDescr))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AutoSize {
            get {       
                return state[stateAutoSize];
            }
            set {
                if (state[stateAutoSize] != value) {
                    state[stateAutoSize] = value;
                    CommonProperties.SetAutoSize(this,value);
                    InvalidateItemLayout(PropertyNames.AutoSize);
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AutoToolTip"]/*' />
        /// <devdoc> 
        ///   !!!!This property ONLY works when toolStrip.ShowItemToolTips = true!!!!
        ///   if AutoToolTip is set to true we use the Text, if false, we use ToolTipText.
        ///</devdoc>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ToolStripItemAutoToolTipDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public bool AutoToolTip {
           get { 
                return state[stateAutoToolTip];
           }
           set {
                state[stateAutoToolTip] = value;
           }
        }

        /// <devdoc>
        /// as opposed to Visible, which returns whether or not the item and its parent are Visible 
        /// Available returns whether or not the item will be shown.  Setting Available sets Visible and Vice/Versa
        ///</devdoc>
        [
        Browsable(false),
        SRDescription(nameof(SR.ToolStripItemAvailableDescr)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool Available{
            get { 
                // MainMenu compat:
                // the only real diff is the getter - this returns what the item really thinks,
                // as opposed to whether or not the parent is also Visible.  Visible behavior is the same
                // so that it matches Control behavior (we dont have to do special things for control hosts, etc etc).
                return state[stateVisible]; 
            }
            set { 
                SetVisibleCore(value);                   
            }
        }
     

        [
        Browsable(false),
        SRCategory(nameof(SR.CatPropertyChanged)), 
        SRDescription(nameof(SR.ToolStripItemOnAvailableChangedDescr))
        ]
        public event EventHandler AvailableChanged {
            add {
                Events.AddHandler(EventAvailableChanged, value);
            }
            remove {
                Events.RemoveHandler(EventAvailableChanged, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Image"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets the image that is displayed on a <see cref='System.Windows.Forms.Label'/>.
        /// </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageDescr)),
        DefaultValue(null)
        ]
        public virtual Image BackgroundImage {
            get {
                return Properties.GetObject(PropBackgroundImage) as Image;
                 
            }
            set {
                if (BackgroundImage != value) {
                     Properties.SetObject(PropBackgroundImage, value);
                     Invalidate();
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.BackgroundImageLayout"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ImageLayout.Tile),
        Localizable(true),
        SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))
        ]
        public virtual ImageLayout BackgroundImageLayout {
          get {
              bool found = Properties.ContainsObject(PropBackgroundImageLayout);
              if (!found) {
                  return ImageLayout.Tile;
              }
              else {
                  return ((ImageLayout)Properties.GetObject(PropBackgroundImageLayout));
             }
          }
          set {
              if (BackgroundImageLayout != value) {
                  //valid values are 0x0 to 0x4
                  if (!ClientUtils.IsEnumValid(value, (int)value, (int)ImageLayout.None, (int)ImageLayout.Zoom))
                  {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ImageLayout));
                  }
                  Properties.SetObject(PropBackgroundImageLayout, value);
                  Invalidate();
              }
          }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.BackColor"]/*' />
        /// <devdoc>
        /// The BackColor of the item
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemBackColorDescr))
        ]
        public virtual Color BackColor {
            get {
                Color c = RawBackColor; // inheritedProperties.BackColor
                if (!c.IsEmpty)
                    return c;

                Control p = ParentInternal;
                if (p != null) {
                    return p.BackColor;
                }
                return Control.DefaultBackColor;
            }
            set {
                Color c = BackColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropBackColor)) {
                    Properties.SetColor(PropBackColor, value);
                }

                if (!c.Equals(BackColor)) {
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.BackColorChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnBackColorChangedDescr))]
        public event EventHandler BackColorChanged {
            add {
                Events.AddHandler(EventBackColorChanged, value);
            }
            remove {
                Events.RemoveHandler(EventBackColorChanged, value);
            }
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Bounds"]/*' />
        /// <devdoc>
        /// The bounds of the item
        /// </devdoc>
        [Browsable(false)]
        public virtual Rectangle Bounds {
            [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
            get {     
                return bounds;
            }
        }

        // Zero-based rectangle, same concept as ClientRect
        internal Rectangle ClientBounds {
            get { 
                Rectangle client = bounds;
                client.Location = Point.Empty;
                return client;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ContentRectangle"]/*' />
        [Browsable(false)]
        public Rectangle ContentRectangle {
            get {
                Rectangle content = LayoutUtils.InflateRect(InternalLayout.ContentRectangle, this.Padding);
                content.Size = LayoutUtils.UnionSizes(Size.Empty, content.Size);
                return content;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.CanSelect"]/*' />
        /// <devdoc>
        /// Determines whether or not the item can be selected.
        /// </devdoc>
        [Browsable(false)]
        public virtual bool CanSelect {
            get {
                return true; 
            }
        }

        // usually the same as can select, but things like the control box in an MDI window are exceptions
        internal virtual bool CanKeyboardSelect {
             get {
                 return CanSelect; 
             }
         }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Click"]/*' />
        /// <devdoc>
        /// <para>Occurs when the control is clicked.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAction)), 
        SRDescription(nameof(SR.ToolStripItemOnClickDescr))
        ]
        public event EventHandler Click {
            add { 
                Events.AddHandler(EventClick, value); 
             }
            remove {
                Events.RemoveHandler(EventClick, value);                
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DefaultValue(CommonProperties.DefaultAnchor)
        ]
        public AnchorStyles Anchor {
            get {
                // since we dont support DefaultLayout go directly against the CommonProperties
                return CommonProperties.xGetAnchor(this);
            }
            set {
                // flags enum - dont check for validity.... 
                
                if(value != Anchor) { 
                    // since we dont support DefaultLayout go directly against the CommonProperties
                    CommonProperties.xSetAnchor(this, value);
                    if (ParentInternal != null) {
                        LayoutTransaction.DoLayout(this, ParentInternal, PropertyNames.Anchor);
                    }
                }
            }
        }


        /// <devdoc> This does not show up in the property grid because it only applies to flow and table layouts </devdoc>
        [
        Browsable(false), 
        DefaultValue(CommonProperties.DefaultDock)
        ]
        public DockStyle Dock {
            get {
                
                // since we dont support DefaultLayout go directly against the CommonProperties
                return CommonProperties.xGetDock(this);
            }
            set {
                //valid values are 0x0 to 0x5
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DockStyle.None, (int)DockStyle.Fill)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DockStyle));
                }
                if(value != Dock) {               
                    // since we dont support DefaultLayout go directly against the CommonProperties
                    CommonProperties.xSetDock(this, value);
                    if (ParentInternal != null) {
                        LayoutTransaction.DoLayout(this, ParentInternal, PropertyNames.Dock);
                    }
                }
            }
        }

        /// <devdoc>default setting of auto tooltip when this object is created</devdoc>
        protected virtual bool DefaultAutoToolTip {
            get { 
                return false; 
            } 
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DefaultMargin"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected internal virtual Padding DefaultMargin {
            get {
                if (this.Owner != null && this.Owner is StatusStrip) {
                    return scaledDefaultStatusStripMargin;
                }
                else {
                    return scaledDefaultMargin;
                }
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DefaultPadding"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected virtual Padding DefaultPadding {
            get {
                return Padding.Empty;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DefaultSize"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected virtual Size DefaultSize {
            get {
                return new Size(23, 23);
            }
        }

        protected virtual ToolStripItemDisplayStyle DefaultDisplayStyle {
            get {
                return ToolStripItemDisplayStyle.ImageAndText;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DismissWhenClicked"]/*' />
        /// <devdoc>
        /// specifies the default behavior of these items on ToolStripDropDowns when clicked.
        /// </devdoc>
        internal protected virtual bool DismissWhenClicked {
            get {
                return true;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DisplayStyle"]/*' />
        /// <devdoc>
        /// DisplayStyle specifies whether the image and text are rendered.  This is not on the base
        /// item class because different derived things will have different enumeration needs.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemDisplayStyleDescr))
        ]
        public virtual ToolStripItemDisplayStyle DisplayStyle {
            get { 
                return displayStyle; 
            }
            set {
                 if (displayStyle != value) {
                    //valid values are 0x0 to 0x3
                     if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemDisplayStyle.None, (int)ToolStripItemDisplayStyle.ImageAndText))
                     {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemDisplayStyle));
                    }               
                    displayStyle = value;
                    if (!state[stateContstructing]) {
                        InvalidateItemLayout(PropertyNames.DisplayStyle);
                        OnDisplayStyleChanged(new EventArgs());
                    }
                }
                
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DisplayStyleChanged"]/*' />
        /// <devdoc>
        /// <para>Occurs when the display style has changed</para>
        /// </devdoc>
        public event EventHandler DisplayStyleChanged {
            add { 
                Events.AddHandler(EventDisplayStyleChanged, value); 
            }
            remove {
                Events.RemoveHandler(EventDisplayStyleChanged, value);
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]     
        private RightToLeft DefaultRightToLeft {
            get {
                  return RightToLeft.Inherit;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DoubleClick"]/*' />
        /// <devdoc>
        /// <para>Occurs when the control is double clicked.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick {
            add {
                Events.AddHandler(EventDoubleClick, value);
            }
            remove {
                Events.RemoveHandler(EventDoubleClick, value);
            }
        }

        [
        DefaultValue(false), 
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.ToolStripItemDoubleClickedEnabledDescr))
        ]
        public bool DoubleClickEnabled {
            get {
                return state[stateDoubleClickEnabled];
            }
            set {
                state[stateDoubleClickEnabled] = value;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DragDrop"]/*' />
        [
        SRCategory(nameof(SR.CatDragDrop)), 
        SRDescription(nameof(SR.ToolStripItemOnDragDropDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event DragEventHandler DragDrop {
            add {
                Events.AddHandler(EventDragDrop, value);
            }
            remove {
                Events.RemoveHandler(EventDragDrop, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DragEnter"]/*' />
        [
        SRCategory(nameof(SR.CatDragDrop)), 
        SRDescription(nameof(SR.ToolStripItemOnDragEnterDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event DragEventHandler DragEnter {
            add {
                Events.AddHandler(EventDragEnter, value);
            }
            remove {
                Events.RemoveHandler(EventDragEnter, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DragOver"]/*' />
        [
        SRCategory(nameof(SR.CatDragDrop)), 
        SRDescription(nameof(SR.ToolStripItemOnDragOverDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event DragEventHandler DragOver {
            add {
                Events.AddHandler(EventDragOver, value);
            }
            remove {
                Events.RemoveHandler(EventDragOver, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DragLeave"]/*' />
        [
        SRCategory(nameof(SR.CatDragDrop)), 
        SRDescription(nameof(SR.ToolStripItemOnDragLeaveDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event EventHandler DragLeave {
            add {
                Events.AddHandler(EventDragLeave, value);
            }
            remove {
                Events.RemoveHandler(EventDragLeave, value);
            }
        }

        /// <devdoc>
        /// ToolStripItem.DropSource
        ///
        /// This represents what we're actually going to drag.  If the parent has set AllowItemReorder to true,
        /// then the item should call back on the private OnQueryContinueDrag/OnGiveFeedback that is implemented
        /// in the parent winbar.
        ///
        /// Else if the parent does not support reordering of items (Parent.AllowItemReorder = false) - 
        /// then call back on the ToolStripItem's OnQueryContinueDrag/OnGiveFeedback methods.
        /// </devdoc> <internalonly/>
        private DropSource DropSource {
            get { 
                if ((ParentInternal != null) && (ParentInternal.AllowItemReorder) && (ParentInternal.ItemReorderDropSource != null)) {
                    return new DropSource(ParentInternal.ItemReorderDropSource);
                }
                return new DropSource(this);
            }     
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Enabled"]/*' />
        /// <devdoc>
        /// <para>Occurs when the control is clicked.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemEnabledDescr)),
        DefaultValue(true)
        ]
        public virtual bool Enabled {
            get { 
                bool parentEnabled = true;  

                if (this.Owner != null) {
                    parentEnabled = this.Owner.Enabled;
                }
                
                return state[stateEnabled] &&  parentEnabled;
            }
            set {
                // flip disabled bit.
                if (state[stateEnabled] != value) {
                    state[stateEnabled] = value;
                    if (!state[stateEnabled]) {
                        bool wasSelected = state[stateSelected];
                        // clear all the other states.
                        state[stateSelected | statePressed] = false;

                        if (wasSelected && !AccessibilityImprovements.UseLegacyToolTipDisplay) {
                            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                        }

                    }
                    OnEnabledChanged(EventArgs.Empty);
                    Invalidate();
                }
                OnInternalEnabledChanged(EventArgs.Empty);
            }

        }



        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.EnabledChanged"]/*' />
        [ 
        SRDescription(nameof(SR.ToolStripItemEnabledChangedDescr))
        ]
        public event EventHandler EnabledChanged {
            add {
                Events.AddHandler(EventEnabledChanged, value);
            }
            remove {
                Events.RemoveHandler(EventEnabledChanged, value);
            }
        }

        internal event EventHandler InternalEnabledChanged {
            add {
                Events.AddHandler(EventInternalEnabledChanged, value);
            }
            remove {
                Events.RemoveHandler(EventInternalEnabledChanged, value);
            }
        }
       
        private void EnsureParentDropTargetRegistered() {
            if (ParentInternal != null) {
                IntSecurity.ClipboardRead.Demand();
                ParentInternal.DropTargetManager.EnsureRegistered(this);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ForeColor"]/*' />
        /// <devdoc>
        /// Retrieves the current font for this item. This will be the font used
        /// by default for painting and text in the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemForeColorDescr))
        ]
        public virtual Color ForeColor {
            get {
                Color foreColor = Properties.GetColor(PropForeColor);
                if (!foreColor.IsEmpty){                
                    return foreColor;
                }
                
                Control p = ParentInternal;
                if (p != null) {
                    return p.ForeColor;
                }
                return Control.DefaultForeColor;
            }
        
            set {
                Color c = ForeColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropForeColor)) {
                    Properties.SetColor(PropForeColor, value);
                }
                if (!c.Equals(ForeColor)) {
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ForeColorChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnForeColorChangedDescr))]
        public event EventHandler ForeColorChanged {
            add {
                Events.AddHandler(EventForeColorChanged, value);
            }
            remove {
                Events.RemoveHandler(EventForeColorChanged, value);
            }
        }
   
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Font"]/*' />
        /// <devdoc>
        /// Retrieves the current font for this control. This will be the font used
        /// by default for painting and text in the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemFontDescr))
        ]
        public virtual Font Font {
            get {
                Font font = (Font)Properties.GetObject(PropFont);
                if (font != null) {
                    return font;
                }

                Font f = GetOwnerFont();
                if (f != null) {
                    return f;
                }
                
                return ToolStripManager.DefaultFont;
            }
            set {

                Font local = (Font)Properties.GetObject(PropFont);
                if ((local != value)){
                    Properties.SetObject(PropFont, value);
                    OnFontChanged(EventArgs.Empty);
                }       
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.GiveFeedback"]/*' />
        [
        SRCategory(nameof(SR.CatDragDrop)), 
        SRDescription(nameof(SR.ToolStripItemOnGiveFeedbackDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event GiveFeedbackEventHandler GiveFeedback {
            add {
                Events.AddHandler(EventGiveFeedback, value);
            }
            remove {
                Events.RemoveHandler(EventGiveFeedback, value);
            }
        }   
  
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Height"]/*' />
        /// <devdoc>
        /// The height of this control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int Height {
            get {
                return Bounds.Height;
            }
            set{
                Rectangle currentBounds = this.Bounds;
                SetBounds(currentBounds.X, currentBounds.Y, currentBounds.Width, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.Children"]/*' />
        /// <devdoc>
        /// ToolStripItems do not have children.  For perf reasons always return a static empty collection.
        /// Consider creating readonly collection.
        /// </devdoc>
        /// <internalonly/>
        ArrangedElementCollection IArrangedElement.Children {
            get {
                return ToolStripItem.EmptyChildCollection;   
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.Container"]/*' />
        /// <devdoc>
        /// Should not be exposed as this returns an unexposed type.
        /// </devdoc>
        /// <internalonly/>
        IArrangedElement IArrangedElement.Container { 
            get {
                if (this.ParentInternal == null) {
                    return this.Owner;
                }
                return this.ParentInternal;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.DisplayRectangle"]/*' />

        Rectangle IArrangedElement.DisplayRectangle {
            get {
                return this.Bounds;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.ParticipatesInLayout"]/*' />

        bool IArrangedElement.ParticipatesInLayout {
            get {
                // this can be different than "Visible" property as "Visible" takes into account whether or not you 
                // are parented and your parent is visible. 
                return state[stateVisible];
            }
        }

       
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.Properties"]/*' />
        /// <internalonly/>
        PropertyStore IArrangedElement.Properties { 
            get {
                return this.Properties;
            }
        }

         
        // Sets the bounds for an element.
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.SetBounds"]/*' />
        /// <internalonly/>
        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified) {
            // in this case the parent is telling us to refresh our bounds - dont 
            // call PerformLayout
            SetBounds(bounds);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IArrangedElement.PerformLayout"]/*' />

        void IArrangedElement.PerformLayout(IArrangedElement container, string propertyName) {
            return;
        }
    
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ImageAlign"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets the alignment of the image on the label control.
        /// </para>
        /// </devdoc>
        [
        DefaultValue(ContentAlignment.MiddleCenter),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageAlignDescr))
        ]
        public ContentAlignment ImageAlign {
            get {
                return imageAlign;
            }
            set {

                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }
                if (imageAlign != value) {
                    imageAlign = value;
                    InvalidateItemLayout(PropertyNames.ImageAlign);
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Image"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets the image that is displayed on a <see cref='System.Windows.Forms.Label'/>.
        /// </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageDescr))
        ]
        public virtual Image Image {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                Image image = (Image)Properties.GetObject(PropImage);
            
                if (image == null && (Owner != null) && (Owner.ImageList != null) && ImageIndexer.ActualIndex >= 0) {
                    if (ImageIndexer.ActualIndex < Owner.ImageList.Images.Count) {
                        // CACHE (by design).  If we fetched out of the image list every time it would dramatically hit perf.
                        image =  Owner.ImageList.Images[ImageIndexer.ActualIndex];
                        state[stateInvalidMirroredImage] = true;
                        Properties.SetObject(PropImage, image);
                        return image; 
                    }
                }
                else {
                    return image;
                }
                return null;
            }
            set {
                if (Image != value) {
                    StopAnimate();
                    Bitmap bmp = value as Bitmap;
                    if (bmp != null && ImageTransparentColor != Color.Empty) {
                        if (bmp.RawFormat.Guid != ImageFormat.Icon.Guid && !ImageAnimator.CanAnimate(bmp)) {
                            bmp.MakeTransparent(ImageTransparentColor);
                        }
                        value = bmp;
                    }
                    if (value != null) {
                        ImageIndex = -1;
                    }
                    Properties.SetObject(PropImage, value);
                    state[stateInvalidMirroredImage] = true;
                    Animate();
                    InvalidateItemLayout(PropertyNames.Image);
                }
            }
        }


        /// <include file='doc\WinBarItem.uex' path='docs/doc[@for="ToolStripItem.ImageTransparentColor"]/*' />
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemImageTransparentColorDescr))
        ] 
        public Color ImageTransparentColor {
            get {
                return imageTransparentColor;
            }
            set {
                if (imageTransparentColor != value) {
                    imageTransparentColor = value;
                    Bitmap currentImage = Image as Bitmap;
                    if (currentImage != null && value != Color.Empty) {
                       if (currentImage.RawFormat.Guid != ImageFormat.Icon.Guid && !ImageAnimator.CanAnimate(currentImage)) {
                            currentImage.MakeTransparent(imageTransparentColor);
                       }
                    }
                    Invalidate();
                }
                
            }
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ImageIndex"]/*' />
        /// <devdoc>
        /// Returns the ToolStripItem's currently set image index     
        /// Here for compat only - this is NOT to be visible at DT.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripItemImageIndexDescr)),
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        RefreshProperties(RefreshProperties.Repaint),        
        TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ToolStripImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(false),
        RelatedImageList("Owner.ImageList")
        ]        
        public int ImageIndex {
            get {
                if ((Owner != null) && ImageIndexer.Index != -1 && Owner.ImageList != null && ImageIndexer.Index >= Owner.ImageList.Images.Count) {
                    return Owner.ImageList.Images.Count - 1;
                } 
                return this.ImageIndexer.Index;
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException(nameof(ImageIndex), string.Format(SR.InvalidLowBoundArgumentEx, "ImageIndex", value.ToString(CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
                }
                
              
                ImageIndexer.Index = value;
                state[stateInvalidMirroredImage] = true;
                // Set the Image Property to null
                Properties.SetObject(PropImage, null);
                      

                InvalidateItemLayout(PropertyNames.ImageIndex);
             }
        }

        internal ToolStripItemImageIndexer ImageIndexer {
            get { 
                if (imageIndexer == null)  {
                    imageIndexer = new ToolStripItemImageIndexer(this);
                }
                return imageIndexer;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ImageKey"]/*' />
        /// <devdoc>
        /// Returns the ToolStripItem's currently set image index     
        /// Here for compat only - this is NOT to be visible at DT.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripItemImageKeyDescr)),
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        Editor("System.Windows.Forms.Design.ToolStripImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(false),
        RelatedImageList("Owner.ImageList")
       ]
        public string ImageKey {
            get { 
                return this.ImageIndexer.Key;
            }
            set {
                ImageIndexer.Key = value;
                state[stateInvalidMirroredImage] = true;
                Properties.SetObject(PropImage, null);
              
                InvalidateItemLayout(PropertyNames.ImageKey);
                
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ImageScaling"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ToolStripItemImageScaling.SizeToFit),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemImageScalingDescr))
        ]
        public ToolStripItemImageScaling ImageScaling {
            get {
                return imageScaling;
            }
            set {
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemImageScaling.None, (int)ToolStripItemImageScaling.SizeToFit))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripItemImageScaling));
                }
                if (imageScaling != value) {
                    imageScaling = value;
                    
                    InvalidateItemLayout(PropertyNames.ImageScaling);
                }
            }
        }

        /// <devdoc>
        ///    <para>
        ///       This object helps determine where the image and text should be drawn.
        ///    </para>
        /// </devdoc>
        /// <internalonly/>
        internal ToolStripItemInternalLayout InternalLayout {
            get { 
                if (toolStripItemInternalLayout == null) {
                    toolStripItemInternalLayout = CreateInternalLayout();
                }
                return toolStripItemInternalLayout;
            }
        }

        internal bool IsForeColorSet {
           get  {
                Color color = Properties.GetColor(PropForeColor);
                if (!color.IsEmpty) {
                    return true;
                }
                else {
                    Control parent = ParentInternal;
                    if (parent != null) {
                        return parent.ShouldSerializeForeColor();
                    }
                }                    
                return false;
           }
        }

        /// <devdoc>
        ///    <para>
        ///       This is used by ToolStrip to pass on the mouseMessages for ActiveDropDown.
        ///    </para>
        /// </devdoc>
        /// <internalonly/>
        internal bool IsInDesignMode {
            get {
                return DesignMode;
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IsDisposed"]/*' />
        [Browsable(false)]
        public bool IsDisposed {
            get {
                return state[stateDisposed];
            }

        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IsOnDropDown"]/*' />
        [Browsable(false)]
        public bool IsOnDropDown {
            get {
                
                if (ParentInternal != null) {
                    return ParentInternal.IsDropDown;
                }
                else if (Owner != null && Owner.IsDropDown) {
                    return true;
                }
                
                return false;
            }
        }

        /// returns whether the item placement is set to overflow.
        [Browsable(false)]
        public bool IsOnOverflow {
            get {
                return (this.Placement == ToolStripItemPlacement.Overflow);
            }
        }


        /// <devdoc>
        ///     Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).  
        /// </devdoc>
        internal virtual bool IsMnemonicsListenerAxSourced 
        {
            get{
                return true;
            }
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.LocationChanged"]/*' />
        /// <devdoc>
        /// Occurs when the location of the ToolStripItem has been updated -- usually by layout by its
        /// owner of ToolStrips
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ToolStripItemOnLocationChangedDescr))]
        public event EventHandler LocationChanged {
            add {
                Events.AddHandler(EventLocationChanged, value);
            }
            remove {
                Events.RemoveHandler(EventLocationChanged, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Margin"]/*' />
        /// <devdoc>
        /// Specifies the external spacing between this item and any other item or the ToolStrip.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripItemMarginDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public Padding Margin {
            get { return CommonProperties.GetMargin(this); }
            set { 
                if (Margin != value) {
                    state[stateUseAmbientMargin] = false;
                    CommonProperties.SetMargin(this, value); 
                }
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MergeAction"]/*' />
        /// <devdoc>
        /// Specifies the merge action when merging two ToolStrip.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripMergeActionDescr)),
        DefaultValue(MergeAction.Append),
        SRCategory(nameof(SR.CatLayout))
        ]
        public MergeAction MergeAction {
            get {
                bool found;
                int action = Properties.GetInteger(PropMergeAction, out found);
                if (found) {
                    return (MergeAction)action;
                }
                else {
                    // default value
                    return MergeAction.Append;
                }
            }

            set {
                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)MergeAction.Append, (int)MergeAction.MatchOnly))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(MergeAction));
                }
                Properties.SetInteger(PropMergeAction, (int)value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MergeIndex"]/*' />
        /// <devdoc>
        /// Specifies the merge action when merging two ToolStrip.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripMergeIndexDescr)),
        DefaultValue(-1),
        SRCategory(nameof(SR.CatLayout))
        ]
        public int MergeIndex {
            get {
                bool found;
                int index = Properties.GetInteger(PropMergeIndex, out found);
                if (found) {
                    return index;
                }
                else {
                    // default value
                    return -1;
                }
            }

            set {
                Properties.SetInteger(PropMergeIndex, value);
            }
        }

        // required for menus
        internal bool MouseDownAndUpMustBeInSameItem {
            get { return state[stateMouseDownAndUpMustBeInSameItem]; }
            set { state[stateMouseDownAndUpMustBeInSameItem] = value; }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MouseDown"]/*' />
        /// <devdoc>
        /// <para>Occurs when the mouse pointer is over the control and a mouse button is 
        /// pressed.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatMouse)), 
        SRDescription(nameof(SR.ToolStripItemOnMouseDownDescr))
        ]
        public event MouseEventHandler MouseDown {
            add {
                Events.AddHandler(EventMouseDown, value);
            }
            remove {
                Events.RemoveHandler(EventMouseDown, value);
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MouseEnter"]/*' />
        /// <devdoc>
        /// <para> Occurs when the mouse pointer enters the control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatMouse)), 
        SRDescription(nameof(SR.ToolStripItemOnMouseEnterDescr))
        ]
        public event EventHandler MouseEnter {
            add {
                Events.AddHandler(EventMouseEnter, value);
            }
            remove {
                Events.RemoveHandler(EventMouseEnter, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MouseLeave"]/*' />
        /// <devdoc>
        /// <para> Occurs when the mouse pointer leaves the control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatMouse)), 
        SRDescription(nameof(SR.ToolStripItemOnMouseLeaveDescr))
        ]
        public event EventHandler MouseLeave {
            add {
                Events.AddHandler(EventMouseLeave, value);
            }
            remove {
                Events.RemoveHandler(EventMouseLeave, value);
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MouseHover"]/*' />
        /// <devdoc>
        /// <para> Occurs when the mouse pointer hovers over the contro.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatMouse)), 
        SRDescription(nameof(SR.ToolStripItemOnMouseHoverDescr))
        ]
        public event EventHandler MouseHover {
            add {
                Events.AddHandler(EventMouseHover, value);
            }
            remove {
                Events.RemoveHandler(EventMouseHover, value);
            }
        }   

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MouseMove"]/*' />
        /// <devdoc>
        /// <para> Occurs when the mouse pointer is moved over the control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatMouse)), 
        SRDescription(nameof(SR.ToolStripItemOnMouseMoveDescr))
        ]
        public event MouseEventHandler MouseMove {
            add {
                Events.AddHandler(EventMouseMove, value);
            }
            remove {
                Events.RemoveHandler(EventMouseMove, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.MouseUp"]/*' />
        /// <devdoc>
        /// <para>Occurs when the mouse pointer is over the control and a mouse button is released.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatMouse)), 
        SRDescription(nameof(SR.ToolStripItemOnMouseUpDescr))
        ]
        public event MouseEventHandler MouseUp {
            add {
                Events.AddHandler(EventMouseUp, value);
            }
            remove {
                Events.RemoveHandler(EventMouseUp, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Name"]/*' />
        /// <devdoc>
        /// Name of this control. The designer will set this to the same
        /// as the programatic Id "(name)" of the control.  The name can be
        /// used as a key into the ControlCollection.
        /// </devdoc>
        [
        Browsable(false),
        DefaultValue(null)
        ]
        public string Name {
            get {
                return WindowsFormsUtils.GetComponentName(this, (string)Properties.GetObject(ToolStripItem.PropName));
            }
            set {
                if (DesignMode) //InDesignMode the Extender property will set it to the right value.
                {
                    return;
                }
                Properties.SetObject(ToolStripItem.PropName, value);
            }
        }           

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Owner"]/*' />
        /// <devdoc>
        /// The owner of this ToolStripItem.  The owner is essentially a backpointer to 
        /// the ToolStrip who contains this item in it's item collection.  Handy for getting
        /// to things such as the ImageList, which would be defined on the ToolStrip.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ToolStrip Owner {
            get {
                return this.owner;
            }
             set {
               if (owner != value) {
                    if (owner != null) {
                        owner.Items.Remove(this);
                    }
                    if (value != null) {
                        value.Items.Add(this);
                    }
               }
            }
        }

        
        /// <devdoc> returns the "parent" item on the preceeding menu which has spawned this item.
        /// e.g.  File->Open  the OwnerItem of Open is File. </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ToolStripItem OwnerItem {
            get {
                ToolStripDropDown currentParent = null;
                
                if (ParentInternal != null) {
                    currentParent =  ParentInternal as ToolStripDropDown;
                }
                else if (Owner != null) {
                    // parent may be null, but we may be "owned" by a collection.
                    currentParent = Owner as ToolStripDropDown;
                }

                if (currentParent != null) {
                    return currentParent.OwnerItem;
                }
                return null;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Paint"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.ToolStripItemOwnerChangedDescr))
        ]
        public event EventHandler OwnerChanged {
            add {
                Events.AddHandler(EventOwnerChanged, value);
            }
            remove {
                Events.RemoveHandler(EventOwnerChanged, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Paint"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)), 
        SRDescription(nameof(SR.ToolStripItemOnPaintDescr))
        ]
        public event PaintEventHandler Paint {
            add {
                Events.AddHandler(EventPaint, value);
            }
            remove {
                Events.RemoveHandler(EventPaint, value);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Parent"]/*' />
        /// <devdoc>
        /// The parent of this ToolStripItem.  This can be distinct from the owner because
        /// the item can fall onto another window (overflow).  In this case the overflow 
        /// would be the parent but the original winbar would be the Owner.  The "parent"
        /// winbar will be firing things like paint events - where as the "owner" winbar 
        /// will be containing shared data like image lists.  Typically the only one who should
        /// set the parent property is the layout manager on the ToolStrip.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        internal protected ToolStrip Parent {
            get {
                // we decided that there is no "parent" protection for toolstripitems.
                // since toolstrip and toolstripitem are tightly coupled.
                return ParentInternal;
            }
            set {
                ParentInternal = value;       
            }
        }

      
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Overflow"]/*' />
        /// <devdoc>
        /// <para>Specifies whether or not the item is glued to the winbar or overflow or
        /// can float between the two.</para>
        /// </devdoc>
        [
        DefaultValue(ToolStripItemOverflow.AsNeeded),
        SRDescription(nameof(SR.ToolStripItemOverflowDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public ToolStripItemOverflow Overflow {
            get { 
                return overflow; 
            }
            set { 
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripItemOverflow.Never, (int)ToolStripItemOverflow.AsNeeded))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripGripStyle));
                }
                if (overflow != value) {
                   overflow = value;
                   if (Owner != null) {
                        LayoutTransaction.DoLayout(Owner, this.Owner, "Overflow");  
                   }
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Padding"]/*' />
        /// <devdoc>
        /// Specifies the internal spacing between the contents and the edges of the item
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolStripItemPaddingDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public virtual Padding Padding {
            get { return CommonProperties.GetPadding(this, DefaultPadding); }
            set {
                if (Padding != value) {
                    CommonProperties.SetPadding(this, value);
                    InvalidateItemLayout(PropertyNames.Padding);
                }
            }
        }            
        
        /// <devdoc>
        /// This is explicitly a winbar, because only winbars know how to manage winbaritems
        /// </devdoc>
        internal ToolStrip ParentInternal {
            get {
                return parent;
            }
            set {  
                if (parent != value) {
                    ToolStrip oldParent = parent;
                    parent = value;
                    OnParentChanged(oldParent, value);
                }
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Placement"]/*' />
        /// <devdoc>
        /// Where the item actually ended up.
        /// </devdoc>
        [Browsable(false)]
        public ToolStripItemPlacement Placement {
            get { 
                return placement; 
            }
        }

        internal Size PreferredImageSize {
            get {
               
                if ((DisplayStyle & ToolStripItemDisplayStyle.Image) != ToolStripItemDisplayStyle.Image) {
                    return Size.Empty;
                }
                
                Image image = (Image)Properties.GetObject(PropImage);
                bool usingImageList = ((Owner != null) && (Owner.ImageList != null) && (ImageIndexer.ActualIndex >= 0));
                    
                if (ImageScaling == ToolStripItemImageScaling.SizeToFit) {
                    ToolStrip ownerToolStrip = Owner;
                    if (ownerToolStrip != null && (image != null || usingImageList)) {
                        return ownerToolStrip.ImageScalingSize;
                    }
                } 

                
                Size imageSize = Size.Empty;
                if (usingImageList) {
                    imageSize = Owner.ImageList.ImageSize;
                }
                else {
                     imageSize = (image == null) ? Size.Empty : image.Size;
                }
              
                return imageSize;
                
            }
            
        }
        /// <devdoc>
        ///     Retrieves our internal property storage object. If you have a property
        ///     whose value is not always set, you should store it in here to save
        ///     space.
        /// </devdoc>
        internal PropertyStore Properties {
            get {
                if (propertyStore == null) {
                    propertyStore = new PropertyStore();
                }
                return propertyStore;
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Pressed"]/*' />
        /// <devdoc>
        /// Returns true if the state of the item is pushed
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Pressed {
            get {
                return CanSelect && state[statePressed];
                  
            }
         }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.QueryContinueDrag"]/*' />
        [
        SRCategory(nameof(SR.CatDragDrop)), 
        SRDescription(nameof(SR.ToolStripItemOnQueryContinueDragDescr)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public event QueryContinueDragEventHandler QueryContinueDrag {
            add {
                Events.AddHandler(EventQueryContinueDrag, value);
            }
            remove {
                Events.RemoveHandler(EventQueryContinueDrag, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.QueryAccessibilityHelp"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ToolStripItemOnQueryAccessibilityHelpDescr))]
        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp {
            add {
                Events.AddHandler(EventQueryAccessibilityHelp, value);
            }
            remove {
                Events.RemoveHandler(EventQueryAccessibilityHelp, value);
            }
        }

      
         // Returns the value of the backColor field -- no asking the parent with its color is, etc.
        internal Color RawBackColor {
            get {
                return Properties.GetColor(PropBackColor);              
            }
        }

        internal ToolStripRenderer Renderer {
            get {
                if (Owner != null) {
                    return Owner.Renderer;
                }
                return (ParentInternal != null) ? ParentInternal.Renderer : null;
               
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.RightToLeft"]/*' />
        /// <devdoc>
        /// This is used for international applications where the language
        /// is written from RightToLeft. When this property is true,
        /// control placement and text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemRightToLeftDescr))
        ]
        public virtual RightToLeft RightToLeft {
            get {
                bool found;
                int rightToLeft = Properties.GetInteger(PropRightToLeft, out found);
                if (!found) {
                    rightToLeft = (int)RightToLeft.Inherit;
                }
                
                if (((RightToLeft)rightToLeft) == RightToLeft.Inherit) {
                    if (Owner != null) {
                        rightToLeft = (int)Owner.RightToLeft;
                    }
                    else if (ParentInternal != null) {
                        // case for Overflow & Grip
                        rightToLeft = (int)ParentInternal.RightToLeft;
                    }
                    else {
                        rightToLeft = (int)DefaultRightToLeft;
                    }
                }
                return (RightToLeft)rightToLeft;                
            }
            
            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)RightToLeft.No, (int)RightToLeft.Inherit))
                {
                    throw new InvalidEnumArgumentException(nameof(RightToLeft), (int)value, typeof(RightToLeft));
                }
                
                RightToLeft oldValue = RightToLeft;
                
                if (Properties.ContainsInteger(PropRightToLeft) || value != RightToLeft.Inherit) {
                    Properties.SetInteger(PropRightToLeft, (int)value);
                }
                
                if (oldValue != RightToLeft) {
                    OnRightToLeftChanged(EventArgs.Empty);
                }
            }
        }


        ///<devdoc>
        /// Mirrors the image when RTL.Yes.
        /// Note we do not change what is returned back from the Image property as this would cause problems with serialization.  
        /// Instead we only change what is painted - there's an internal MirroredImage property which fills in as 
        /// e.Image in the ToolStripItemImageRenderEventArgs if the item is RTL.Yes and AutoMirrorImage is turned on.
        ///</devdoc>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemRightToLeftAutoMirrorImageDescr))
        ]
        public bool RightToLeftAutoMirrorImage {
            get {
                return state[stateRightToLeftAutoMirrorImage];
            }
            set {
                if (state[stateRightToLeftAutoMirrorImage] != value) {
                    state[stateRightToLeftAutoMirrorImage]  = value; 
                    Invalidate();
                }
            }
        }

        internal Image MirroredImage {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                if (state[stateInvalidMirroredImage]) {
                    Image image = Image;
                    if (image != null) {
                        Image mirroredImage = image.Clone() as Image;
                        mirroredImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        
                        Properties.SetObject(PropMirroredImage, mirroredImage);
                        state[stateInvalidMirroredImage] = false;
                        return mirroredImage;
                    }
                    else {
                        return null;
                    }
                }
                return Properties.GetObject(PropMirroredImage) as Image;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.RightToLeftChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged {
            add {
                Events.AddHandler(EventRightToLeft, value);
            }
            remove {
                Events.RemoveHandler(EventRightToLeft, value);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Selected"]/*' />
        /// <devdoc>
        ///  if the item is selected we return true. 
        ///
        ///  FAQ: Why dont we have a Hot or MouseIsOver property?
        ///  After going through the scenarios, we've decided NOT to add a separate MouseIsOver or Hot flag to ToolStripItem.  The thing to use is 'Selected'.
        ///  Why?  While the selected thing can be different than the moused over item, the selected item is ALWAYS the one you want to paint differently
        ///
        ///   Scenario 1:  Keyboard select an item then select a different item with the mouse.
        ///   -          Do Alt+F to expand your File menu, keyboard down several items.
        ///   -          Mouse over a different item
        ///   -          Notice how two things are never painted hot at the same time, and how the selection changes from the keyboard selected item to the one selected with the mouse.  In  this case the selection should move with the mouse selection.
        ///   -          Notice how if you hit enter when the mouse is over it, it executes the item.  That's selection.
        ///   Scenario 2: Put focus into a combo box, then mouse over a different item
        ///   -          Notice how all the other items you mouse over do not change the way they are painted, if you hit enter, that goes to the combobox, rather than executing the current item.
        ///      
        ///   At first look "MouseIsOver" or "Hot" seems to be the thing people want, but its almost never the desired behavior.  A unified selection model is simpler and seems to meet the scenarios.
        ///
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selected {
            get {
                return CanSelect && !DesignMode && (state[stateSelected] || 
                    (ParentInternal != null && ParentInternal.IsSelectionSuspended && 
                     ParentInternal.LastMouseDownedItem == this));
            }
        }

#if SELECTEDCHANGED
        public event EventHandler SelectedChanged {
            add { 
                Events.AddHandler(EventSelectedChanged, value); 
             }
            remove {
                Events.RemoveHandler(EventSelectedChanged, value);                
            }
        }
#endif

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ShowKeyboardCues"]/*' />
        /// <devdoc/>
        internal protected virtual bool ShowKeyboardCues {
            get {
                if (!DesignMode) {
                    return ToolStripManager.ShowMenuFocusCues;
                }
                // default to true.
                return true;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Size"]/*' />
        /// <devdoc>The size of the item</devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemSizeDescr))
        ]
        public virtual Size Size {
            get {
                return Bounds.Size; 
            }
            set { 
                Rectangle currentBounds = Bounds;
                currentBounds.Size = value;
                SetBounds(currentBounds); 
            } 
        }

        internal bool SupportsRightClick {
            get {
                return state[stateSupportsRightClick];
            }
            set {
                state[stateSupportsRightClick] = value;
            }
        }

        internal bool SupportsItemClick {
            get {
                return state[stateSupportsItemClick];
            }
            set {
                state[stateSupportsItemClick] = value;
            }
        }

        internal bool SupportsSpaceKey {
            get {
                return state[stateSupportsSpaceKey];
            }
            set {
                state[stateSupportsSpaceKey] = value;
            }
        }

                   
        internal bool SupportsDisabledHotTracking {
            get {
                return state[stateSupportsDisabledHotTracking];
            }
            set {
                state[stateSupportsDisabledHotTracking] = value;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Tag"]/*' />
        /// <devdoc>Summary for Tag</devdoc>
        [DefaultValue(null),
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ToolStripItemTagDescr)),
        TypeConverter(typeof(StringConverter))
        ]
        public object Tag {
            get {
                if (Properties.ContainsObject(ToolStripItem.PropTag)) {
                    return propertyStore.GetObject(ToolStripItem.PropTag);
                }
                return null;
                
            }
            set {
                Properties.SetObject(ToolStripItem.PropTag, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Text"]/*' />
        /// <devdoc>The text of the item</devdoc>
        [
        DefaultValue(""),
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemTextDescr))
        ]
        public virtual string Text {
            get {
                if (Properties.ContainsObject(ToolStripItem.PropText)) {
                    return (string)Properties.GetObject(ToolStripItem.PropText);
                }
                return "";
                
            }
            set {
                if (value != Text) {
                    Properties.SetObject(ToolStripItem.PropText, value);
                    OnTextChanged(EventArgs.Empty);
                
                }
            }

        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.TextAlign"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets the alignment of the text on the label control.
        /// </para>
        /// </devdoc>
        [
        DefaultValue(ContentAlignment.MiddleCenter),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripItemTextAlignDescr))
        ]
        public virtual ContentAlignment TextAlign {
            get {
                return textAlign;
            }
            set {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }
                if (textAlign != value) {
                    textAlign = value;
                    InvalidateItemLayout(PropertyNames.TextAlign);
                }
            }
        }
       
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.TextChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnTextChangedDescr))]
        public event EventHandler TextChanged {
            add {
                Events.AddHandler(EventText, value);
            }
            remove {
                Events.RemoveHandler(EventText, value);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.TextDirection"]/*' />
        [
        SRDescription(nameof(SR.ToolStripTextDirectionDescr)),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public virtual ToolStripTextDirection TextDirection {
             get {
                 ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
                 if (Properties.ContainsObject(ToolStripItem.PropTextDirection)) {
                     textDirection= (ToolStripTextDirection)Properties.GetObject(ToolStripItem.PropTextDirection);
                 }

                 if (textDirection == ToolStripTextDirection.Inherit) {
                     if (this.ParentInternal != null) {
                           // in the case we're on a ToolStripOverflow
                           textDirection = ParentInternal.TextDirection;
                     }
                     else {
                         textDirection = (Owner == null) ? ToolStripTextDirection.Horizontal : Owner.TextDirection;
                     }
                 }
                 
                 return textDirection;
             }
             set {
                 //valid values are 0x0 to 0x3
                 if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripTextDirection.Inherit, (int)ToolStripTextDirection.Vertical270))
                 {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripTextDirection));
                 }   
                 Properties.SetObject(ToolStripItem.PropTextDirection, value);
                 InvalidateItemLayout("TextDirection");
             }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.TextImageRelation"]/*' />
        [DefaultValue(TextImageRelation.ImageBeforeText),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemTextImageRelationDescr)),
        SRCategory(nameof(SR.CatAppearance))]
        public TextImageRelation TextImageRelation {
            get {
                return textImageRelation;
            }
            set {
                if (!WindowsFormsUtils.EnumValidator.IsValidTextImageRelation(value)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TextImageRelation));
                }
                if(value != TextImageRelation) {
                    textImageRelation = value;
                    InvalidateItemLayout(PropertyNames.TextImageRelation);
                }                
            }
        }

      
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ToolTipText"]/*' />
        /// <devdoc>
        ///   !!!!This property ONLY works when toolStrip.ShowItemToolTips = true!!!!
        ///   if AutoToolTip is set to true we return the Text as the ToolTipText.
        /// </devdoc>
        [SRDescription(nameof(SR.ToolStripItemToolTipTextDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor))]
        [Localizable(true)]
        public string ToolTipText {
            get {
                if (AutoToolTip && string.IsNullOrEmpty(toolTipText)) {
                    string toolText = Text;
                    if (WindowsFormsUtils.ContainsMnemonic(toolText)) {
                       // this shouldnt be called a lot so we can take the perf hit here. 
                       toolText = String.Join("", toolText.Split('&'));
                    }
                    return toolText;
                }
                return toolTipText;
            }
            set {
                toolTipText = value;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Visible"]/*' />
        /// <devdoc>Whether or not the item is visible</devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ToolStripItemVisibleDescr))
        ]
        public bool Visible {
            get { 
                return (ParentInternal!=null) && (ParentInternal.Visible) &&  Available; 
            }
            set { 
                SetVisibleCore(value);                
            }

        }

        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.VisibleChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ToolStripItemOnVisibleChangedDescr))]
        public event EventHandler VisibleChanged {
            add {
                Events.AddHandler(EventVisibleChanged, value);
            }
            remove {
                Events.RemoveHandler(EventVisibleChanged, value);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Width"]/*' />
        /// <devdoc>
        /// The width of this ToolStripItem.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int Width {
            get {
                return Bounds.Width;
            }
            set {
                Rectangle currentBounds = this.Bounds;
                SetBounds(currentBounds.X, currentBounds.Y, value, currentBounds.Height);
            }
        }
        
       //
       //  Methods for ToolStripItem 
       //

        internal void AccessibilityNotifyClients(AccessibleEvents accEvent) {
            if (ParentInternal != null) {
                int index = ParentInternal.DisplayedItems.IndexOf(this);
                ParentInternal.AccessibilityNotifyClients(accEvent, index);
            }
        }

        private void Animate() {
            Animate(!DesignMode && Visible && Enabled && ParentInternal != null);
        }

        private void StopAnimate() {
            Animate(false);
        }

        private void Animate(bool animate) {
           if (animate != state[stateCurrentlyAnimatingImage]) {
               if (animate) {
                   if (Image != null) {
                       ImageAnimator.Animate(Image, new EventHandler(this.OnAnimationFrameChanged));
                       state[stateCurrentlyAnimatingImage] = animate;
                   }
               }
               else {
                   if (Image != null) {
                       ImageAnimator.StopAnimate(Image, new EventHandler(this.OnAnimationFrameChanged));
                       state[stateCurrentlyAnimatingImage] = animate;
                   }
               }
           }
        }

      
        internal bool BeginDragForItemReorder() {
            if (Control.ModifierKeys == Keys.Alt) {
               
                if (this.ParentInternal.Items.Contains(this) && this.ParentInternal.AllowItemReorder) {
                    // we only drag
                    ToolStripItem item = this as ToolStripItem;
                    DoDragDrop(item,DragDropEffects.Move);
                    return true;
                }
               
            }
            return false;
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.CreateAccessibilityInstance"]/*' />
        /// <devdoc>
        /// constructs the new instance of the accessibility object for this ToolStripItem. Subclasses
        /// should not call base.CreateAccessibilityObject.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual AccessibleObject CreateAccessibilityInstance() {
            return new ToolStripItemAccessibleObject(this);
        }

        /// <devdoc>
        /// Creates an instance of the object that defines how image and text
        /// gets laid out in the ToolStripItem
        /// </devdoc>
        
        internal virtual ToolStripItemInternalLayout CreateInternalLayout() {
            return new ToolStripItemInternalLayout(this);
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Dispose"]/*' />
        /// <devdoc>
        /// Disposes this winbar item...
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                state[stateDisposing] = true;
            
                if (this.Owner != null) {
                    StopAnimate();
                    Debug.Assert(this.Owner.Items.Contains(this), "How can there be a owner and not be in the collection?");
                    this.Owner.Items.Remove(this);
                    toolStripItemInternalLayout = null;
                    state[stateDisposed] = true;
                }
            }
            base.Dispose (disposing);

            if (disposing) {
                 // need to call base() first since the Component.Dispose(_) is listened to by the ComponentChangeService 
                 // which Serializes the object in Undo-Redo transactions.
                 Properties.SetObject(PropMirroredImage, null);
                 Properties.SetObject(PropImage, null);                 
                 state[stateDisposing] = false;
            }                
            
        }

        internal static long DoubleClickTicks {
            // (DoubleClickTime in ms) * 1,000,000 ns/1ms * 1 Tick / 100ns = XXX in Ticks.
            // Therefore: (DoubleClickTime) * 1,000,000/100 = xxx in Ticks. 
            get { return SystemInformation.DoubleClickTime * 10000; }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.DoDragDrop"]/*' />
        /// <devdoc>
        /// Begins a drag operation. The allowedEffects determine which
        /// drag operations can occur. If the drag operation needs to interop
        /// with applications in another process, data should either be
        /// a base managed class (String, Bitmap, or Metafile) or some Object
        /// that implements System.Runtime.Serialization.ISerializable. data can also be any Object that
        /// implements System.Windows.Forms.IDataObject.
        /// </devdoc>        
        [UIPermission(SecurityAction.Demand, Clipboard=UIPermissionClipboard.OwnClipboard)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DragDropEffects DoDragDrop(Object data, DragDropEffects allowedEffects) {
            int[] finalEffect = new int[] {(int)DragDropEffects.None};
            UnsafeNativeMethods.IOleDropSource dropSource = this.DropSource;
            IComDataObject dataObject = null;

            dataObject = data as IComDataObject;
            if (dataObject == null) {
                DataObject iwdata = null;
                IDataObject idataObject = data as IDataObject;
                if (idataObject != null) {
                    iwdata = new DataObject(idataObject);
                }
                else if (data is ToolStripItem) {

                    // it seems that the DataObject does string comparison
                    // on the type, so you can't ask for GetDataPresent of
                    // a base type (e.g. ToolStripItem) when you are really 
                    // looking at a ToolStripButton.  The alternative is
                    // to set the format string expressly to a string matching 
                    // the type of ToolStripItem
                    iwdata = new DataObject();
                    iwdata.SetData(typeof(ToolStripItem).ToString(), data);
                }
                else {
                    iwdata = new DataObject();
                    iwdata.SetData(data);
                }
                dataObject = (IComDataObject)iwdata;
            }

            try {
                SafeNativeMethods.DoDragDrop(dataObject, dropSource, (int)allowedEffects, finalEffect);
            }
            catch {
            }
            return(DragDropEffects)finalEffect[0];
        }
  
        internal void FireEvent(ToolStripItemEventType met) {
            FireEvent(new System.EventArgs(), met);
        }
        internal void FireEvent(EventArgs e, ToolStripItemEventType met) {

            switch (met) {
                case ToolStripItemEventType.LocationChanged:
                    OnLocationChanged(e);
                    break;
                case ToolStripItemEventType.Paint:
                    HandlePaint(e as PaintEventArgs);
                    break;
                case ToolStripItemEventType.MouseHover:
                    // disabled toolstrip items should show tooltips.
                    // we wont raise mouse events though.
                    if (!Enabled && ParentInternal != null && !string.IsNullOrEmpty(ToolTipText)) {
                        ParentInternal.UpdateToolTip(this);
                    }
                    else {
                        FireEventInteractive(e, met);
                    }
                    break;
                case ToolStripItemEventType.MouseEnter:
                    HandleMouseEnter(e);
                    break;
                case ToolStripItemEventType.MouseLeave:
                    // disabled toolstrip items should also clear tooltips.
                    // we wont raise mouse events though.                    
                   if (!Enabled && ParentInternal != null) {
                       ParentInternal.UpdateToolTip(null);
                   }
                   else {
                       HandleMouseLeave(e);
                   }
                   break;
                case ToolStripItemEventType.MouseMove:
                    // Disabled items typically dont get mouse move 
                    // but they should be allowed to re-order if the ALT key is pressed
                    if (!Enabled && ParentInternal != null) {
                        BeginDragForItemReorder();
                    }
                    else {
                        FireEventInteractive(e, met);
                    }
                    break;                        
                default:
                    FireEventInteractive(e, met);
                    break;
            }
            
        }

        ///
        ///
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal void FireEventInteractive(EventArgs e, ToolStripItemEventType met) {
            if (Enabled) {
                switch (met) {
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

        private Font GetOwnerFont() {
            if (Owner  != null) {
                return Owner.Font;
            }
            return null;
        }
        
        /// <devdoc> we dont want a public settable property... and usually owner will work 
        ///          except for things like the overflow button</devdoc>
        public ToolStrip GetCurrentParent() {
            return this.Parent;
        }
        internal ToolStripDropDown GetCurrentParentDropDown() {
            if (ParentInternal != null) {
                return ParentInternal as ToolStripDropDown;
            }
            else {
                return Owner as ToolStripDropDown;
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.GetPreferredSize"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
        public virtual Size GetPreferredSize(Size constrainingSize) {
        
           // Switch Size.Empty to maximum possible values
           constrainingSize = LayoutUtils.ConvertZeroToUnbounded(constrainingSize);
           return InternalLayout.GetPreferredSize(constrainingSize - Padding.Size) + Padding.Size;
        }

        internal Size GetTextSize() {
             if (string.IsNullOrEmpty(Text)) {
                return Size.Empty;
             }
             else if (cachedTextSize == Size.Empty) {
                cachedTextSize = TextRenderer.MeasureText(Text, Font);
             }
             return cachedTextSize;
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Invalidate"]/*' />
        /// <devdoc>
        /// Invalidates the ToolStripItem
        /// </devdoc>
        public void Invalidate() {
            if (this.ParentInternal != null) {
                ParentInternal.Invalidate(this.Bounds, true);
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Invalidate1"]/*' />
        /// <devdoc>
        /// invalidates a rectangle within the ToolStripItem's bounds
        /// </devdoc>
        public void Invalidate(Rectangle r) {
            // the only value add to this over calling invalidate on the ToolStrip is that 
            // you can specify the rectangle with coordinates relative to the upper left hand
            // corner of the ToolStripItem.
            Point rectangleLocation = this.TranslatePoint(r.Location, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ToolStripCoords);
           
            if (this.ParentInternal != null) {
                ParentInternal.Invalidate(new Rectangle(rectangleLocation, r.Size), true);
            }
        }

        internal void InvalidateItemLayout(string affectedProperty, bool invalidatePainting) {
            this.toolStripItemInternalLayout = null;

            if (Owner != null) {
                LayoutTransaction.DoLayout(Owner, this, affectedProperty);
            }
                
            if (invalidatePainting && Owner != null) {
               Owner.Invalidate(); 
            }
        }
        internal void InvalidateItemLayout(string affectedProperty) {
            InvalidateItemLayout(affectedProperty, /*invalidatePainting*/true);
        }

        internal void InvalidateImageListImage() {
            // invalidate the cache.
            if (ImageIndexer.ActualIndex >= 0) {
                Properties.SetObject(PropImage, null);
                InvalidateItemLayout(PropertyNames.Image);
            }          
        }

        internal void InvokePaint() {
            if (this.ParentInternal != null) {
                this.ParentInternal.InvokePaintItem(this);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IsInputKey"]/*' />
        protected internal virtual bool IsInputKey(Keys keyData) {
            return false;
        }
          
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IsInputChar"]/*' />
        protected internal virtual bool IsInputChar(char charCode) {
            return false;
        }

        //
        //  Private handlers which are the equivilant of Control.Wm<Message>
        //

        private void HandleClick(System.EventArgs e) {  
            Debug.Assert(Enabled, "Who called me when I am disabled?");
            Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] HandleClick");      

            try {
                if (!DesignMode) {
                    state[statePressed] = true;
                }
                // force painting w/o using message loop here because it may be quite a long
                // time before it gets pumped again.
                InvokePaint();

                if (SupportsItemClick && Owner != null) {
                    Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] HandleItemClick");      
                    Owner.HandleItemClick(this);       
                }

                OnClick(e);
                
                if (SupportsItemClick && Owner != null) {
                    Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] HandleItemClicked");      
                    Owner.HandleItemClicked(this);       
                }
            }
            finally {
                state[statePressed] = false;
            }
            // when we get around to it, paint unpressed.
            Invalidate();
            
        }
        private void HandleDoubleClick(System.EventArgs e) {
            OnDoubleClick(e);
        }
        private void HandlePaint(PaintEventArgs e) {
            Animate();
            ImageAnimator.UpdateFrames(this.Image);

            OnPaint(e);
            RaisePaintEvent(EventPaint, e);
        }
        private void HandleMouseEnter(System.EventArgs e) {
            
            if (!DesignMode) {
               if (ParentInternal != null 
                    && ParentInternal.CanHotTrack 
                    && ParentInternal.ShouldSelectItem()) {
    
                    if (Enabled) {
                       // calling select can dismiss a child dropdown which would break auto-expansion.
                       // save off auto expand and restore it.
                       bool autoExpand = ParentInternal.MenuAutoExpand;

                       if (ParentInternal.LastMouseDownedItem == this) {
                           // Same as Control.MouseButtons == MouseButtons.Left, but slightly more efficient.
                           if (UnsafeNativeMethods.GetKeyState((int)Keys.LButton) < 0) {
                                this.Push(true);
                           }
                       }
                   
                        Select();
                        ParentInternal.MenuAutoExpand = autoExpand;
                    }
                    else if (SupportsDisabledHotTracking) {
                        Select();
                    }                    
               }               
            }

            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                KeyboardToolTipStateMachine.Instance.NotifyAboutMouseEnter(this);
            }

            if (Enabled) {
                OnMouseEnter(e);
                Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] MouseEnter"); 
                RaiseEvent(EventMouseEnter, e);
            }
            
        }
        private void HandleMouseMove(System.Windows.Forms.MouseEventArgs mea) {
            Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] MouseMove");    

            if (Enabled && CanSelect && !Selected) {
                if (ParentInternal != null 
                     && ParentInternal.CanHotTrack 
                     && ParentInternal.ShouldSelectItem()) {
                    // this is the case where we got a mouse enter, but ShouldSelectItem 
                    // returned false.  
                    // typically occus when a window first opens - we get a mouse enter on the item
                    // the cursor is hovering over - but we dont actually want to change selection to it.
                    Select();
                }
            }
            OnMouseMove(mea);
            RaiseMouseEvent(EventMouseMove, mea);
            
        }      
        private void HandleMouseHover(EventArgs e) {
            Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] MouseHover");   
            OnMouseHover(e);
            RaiseEvent(EventMouseHover, e);
        }
        private void HandleLeave() {
            if (state[stateMouseDownAndNoDrag] || state[statePressed] || state[stateSelected]) {
                state[stateMouseDownAndNoDrag | statePressed | stateSelected] = false;

                if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                }

                Invalidate();
            }         
        }
        private void HandleMouseLeave(System.EventArgs e) {
            Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] MouseLeave");
            HandleLeave();
            if (Enabled) {
                OnMouseLeave(e);
                RaiseEvent(EventMouseLeave, e);
            }
        }
        private void HandleMouseDown(MouseEventArgs e) {
            Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] MouseDown");        
        
            state[stateMouseDownAndNoDrag] = !BeginDragForItemReorder();
            if (state[stateMouseDownAndNoDrag]) {
                if (e.Button == MouseButtons.Left) {
                    this.Push(true);
                }
                // 
                OnMouseDown(e);
                RaiseMouseEvent(EventMouseDown, e);
            }
        }
        private void HandleMouseUp(MouseEventArgs e) {
            
            Debug.WriteLineIf(MouseDebugging.TraceVerbose, "[" + this.Text + "] MouseUp");      

            bool fireMouseUp = (ParentInternal.LastMouseDownedItem == this);

            if (!fireMouseUp && !MouseDownAndUpMustBeInSameItem) {
                // in the case of menus, you can mouse down on one item and mouse up 
                // on another.  We do need to be careful
                // that the mouse has actually moved from when a dropdown has been opened -
                // otherwise we may accidentally click what's underneath the mouse at the time
                // the dropdown is opened.
                fireMouseUp = ParentInternal.ShouldSelectItem();
            }


            if (state[stateMouseDownAndNoDrag] || fireMouseUp) {
                this.Push(false);

                if (e.Button == MouseButtons.Left || (e.Button == MouseButtons.Right && state[stateSupportsRightClick])) {
                    bool shouldFireDoubleClick = false;
                    if (DoubleClickEnabled) {
                        long newTime = DateTime.Now.Ticks;
                        long deltaTicks = newTime - lastClickTime;
                        lastClickTime = newTime;
                        // use >= for cases where the delta is so fast DateTime cannot pick up the delta.
                        Debug.Assert(deltaTicks >= 0, "why are deltaticks less than zero? thats some mighty fast clicking");
                        // if we've seen a mouse up less than the double click time ago, we should fire.
                        if (deltaTicks >= 0 && deltaTicks < DoubleClickTicks) {
                            shouldFireDoubleClick = true;
                        }
                    }
                    if (shouldFireDoubleClick) {
                        HandleDoubleClick(new System.EventArgs());
                        // If we actually fired DoubleClick - reset the lastClickTime.
                        lastClickTime = 0;
                    } 
                    else {
                        HandleClick(new System.EventArgs());
                    }
                }

                // 
                OnMouseUp(e);
                RaiseMouseEvent(EventMouseUp, e);
                
                
            }
        }


        internal virtual void OnAccessibleDescriptionChanged(EventArgs e) {
        }
        internal virtual void OnAccessibleNameChanged(EventArgs e) {
        }
        internal virtual void OnAccessibleDefaultActionDescriptionChanged(EventArgs e) {
        }
        internal virtual void OnAccessibleRoleChanged(EventArgs e) {
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnBackColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackColorChanged(EventArgs e) {
           this.Invalidate();
           RaiseEvent(EventBackColorChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnBoundsChanged"]/*' />
        protected virtual void OnBoundsChanged() {
            LayoutTransaction.DoLayout(this.ParentInternal, this, PropertyNames.Bounds);
            InternalLayout.PerformLayout();
        }

       
    
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnClick"]/*' />
        protected virtual void OnClick(System.EventArgs e) {    
            RaiseEvent(EventClick, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnLayout"]/*' />
        protected internal virtual void OnLayout(LayoutEventArgs e) {    
        }

        ///
        /// Explicit support of DropTarget 
        ///
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IDropTarget.OnDragEnter"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragEnter(DragEventArgs dragEvent) {
            OnDragEnter(dragEvent);
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IDropTarget.OnDragOver"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragOver(DragEventArgs dragEvent) {
            OnDragOver(dragEvent);
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IDropTarget.OnDragLeave"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragLeave(EventArgs e) {
            OnDragLeave(e);
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.IDropTarget.OnDragDrop"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragDrop(DragEventArgs dragEvent) {
            OnDragDrop(dragEvent);
        }
        ///
        /// Explicit support of DropSource 
        ///
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ISupportOleDropSource.OnGiveFeedback"]/*' />
        /// <internalonly/>
        void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs)  {
            OnGiveFeedback(giveFeedbackEventArgs);
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ISupportOleDropSource.OnQueryContinueDrag"]/*' />
        /// <internalonly/>
        void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs) {
            OnQueryContinueDrag(queryContinueDragEventArgs);
        }


        private void OnAnimationFrameChanged(object o, EventArgs e) {

           ToolStrip parent = ParentInternal;
           if (parent != null) {
               if (parent.Disposing || parent.IsDisposed) {
                  return;
               }

               if (parent.IsHandleCreated && parent.InvokeRequired) {
                   parent.BeginInvoke(new EventHandler(this.OnAnimationFrameChanged), new object[]{o,e});
                   return;
               }

               Invalidate();
           }
       }
        
        protected virtual void OnAvailableChanged(System.EventArgs e) {
            RaiseEvent(EventAvailableChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnDragEnter"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.ToolStripItem.Enter'/> event.</para>
        /// <para>Inheriting classes should override this method to handle this event.
        /// Call base.onEnter to send this event to any registered event listeners.</para>
        /// </devdoc>
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// Call base.onDragEnter to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragEnter(DragEventArgs dragEvent) {
            RaiseDragEvent(EventDragEnter, dragEvent);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnDragOver"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// Call base.onDragOver to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragOver(DragEventArgs dragEvent) {
            RaiseDragEvent(EventDragOver, dragEvent);   
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnDragLeave"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// Call base.onDragLeave to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragLeave(EventArgs e) {
            RaiseEvent(EventDragLeave, e);
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnDragDrop"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// Call base.onDragDrop to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragDrop(DragEventArgs dragEvent) {
            RaiseDragEvent(EventDragDrop, dragEvent);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnDisplayStyleChanged"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDisplayStyleChanged(EventArgs e) {
            RaiseEvent(EventDisplayStyleChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnGiveFeedback"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// Call base.onGiveFeedback to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")] // PM review done
        protected virtual void OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEvent) {
            GiveFeedbackEventHandler handler = (GiveFeedbackEventHandler)Events[EventGiveFeedback];
            if (handler != null) handler(this,giveFeedbackEvent);
        }

        internal virtual void OnImageScalingSizeChanged(EventArgs e) {            
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnQueryContinueDrag"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// Call base.onQueryContinueDrag to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")] // PM review done
        protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEvent) {
            RaiseQueryContinueDragEvent(EventQueryContinueDrag, queryContinueDragEvent);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnDoubleClick"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnDoubleClick(System.EventArgs e) {
            RaiseEvent(EventDoubleClick, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnEnabledChanged"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnEnabledChanged(System.EventArgs e) {
             RaiseEvent(EventEnabledChanged, e);
             Animate();
        }

        internal void OnInternalEnabledChanged(System.EventArgs e) {
             RaiseEvent(EventInternalEnabledChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnForeColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnForeColorChanged(EventArgs e) {
           this.Invalidate();
           RaiseEvent(EventForeColorChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnFontChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFontChanged(EventArgs e) {
           cachedTextSize = Size.Empty;
           // PERF - only invalidate if we actually care about the font
           if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) {
                this.InvalidateItemLayout(PropertyNames.Font);
           }
           else {
               this.toolStripItemInternalLayout = null;
           }
           RaiseEvent(EventFontChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnLocationChanged"]/*' />
        protected virtual void OnLocationChanged (System.EventArgs e) {
            RaiseEvent(EventLocationChanged, e);
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnMouseEnter"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnMouseEnter(EventArgs e) {      
        }    
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnMouseMove"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnMouseMove(System.Windows.Forms.MouseEventArgs mea) {
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnMouseHover"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnMouseHover(EventArgs e) {     
            if (ParentInternal != null && !string.IsNullOrEmpty(ToolTipText)) {
              ParentInternal.UpdateToolTip(this);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnMouseLeave"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnMouseLeave(EventArgs e) {       
            if (ParentInternal != null) {
                ParentInternal.UpdateToolTip(null);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnMouseDown"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnMouseDown(MouseEventArgs e) {  
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnMouseUp"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnMouseUp(MouseEventArgs e) {
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnPaint"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnPaint(System.Windows.Forms.PaintEventArgs e) {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentBackColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBackColorChanged(EventArgs e) {
            Color backColor = Properties.GetColor(PropBackColor);
            if (backColor.IsEmpty) {
                OnBackColorChanged(e);
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnParentChanged"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnParentChanged(ToolStrip oldParent, ToolStrip newParent) {
            SetAmbientMargin();
            if ((oldParent != null) && (oldParent.DropTargetManager != null)) {
                oldParent.DropTargetManager.EnsureUnRegistered(this);
            }
            if (AllowDrop && (newParent != null)) { 
                EnsureParentDropTargetRegistered();
            }
            Animate();
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnParentEnabledChanged"]/*' />
        /// <devdoc>
        /// Occurs when this.Parent.Enabled changes.
        /// </devdoc>
        protected internal virtual void OnParentEnabledChanged(System.EventArgs e) {
            OnEnabledChanged(EventArgs.Empty);
        }                 

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnParentFontChanged"]/*' />
        /// <devdoc>
        /// <para>Occurs when the font property has changed on the parent - used to notify inheritors of the font property that 
        /// the font has changed
        /// </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal protected virtual void OnOwnerFontChanged(EventArgs e) {
            if (Properties.GetObject(PropFont) == null) {
                OnFontChanged(e);
            }
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnParentForeColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentForeColorChanged(EventArgs e) {
            Color foreColor = Properties.GetColor(PropForeColor);
            if (foreColor.IsEmpty) {
                OnForeColorChanged(e);
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnParentRightToLeftChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual void OnParentRightToLeftChanged(EventArgs e) {
            if (!Properties.ContainsInteger(PropRightToLeft) || ((RightToLeft)Properties.GetInteger(PropRightToLeft)) == RightToLeft.Inherit) {
                OnRightToLeftChanged(e);
            }
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnOwnerChanged"]/*' />
        /// <devdoc>
        /// Occurs when the owner of an item changes. 
        /// </devdoc>
        protected virtual void OnOwnerChanged(EventArgs e) {
            RaiseEvent(EventOwnerChanged,e);
            SetAmbientMargin();
            if (Owner != null) {
                // check if we need to fire OnRightToLeftChanged
                bool found = false;
                int rightToLeft = Properties.GetInteger(PropRightToLeft, out found);
                if (!found) {
                    rightToLeft = (int)RightToLeft.Inherit;
                }
                if ((rightToLeft == (int)RightToLeft.Inherit) && RightToLeft != DefaultRightToLeft) {
                    OnRightToLeftChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal void OnOwnerTextDirectionChanged() {
            ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
            if (Properties.ContainsObject(ToolStripItem.PropTextDirection)) {
               textDirection= (ToolStripTextDirection)Properties.GetObject(ToolStripItem.PropTextDirection);
            }

            if (textDirection == ToolStripTextDirection.Inherit) {
                InvalidateItemLayout("TextDirection");
            }
        }
            
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnRightToLeftChanged"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnRightToLeftChanged(EventArgs e) {
            InvalidateItemLayout(PropertyNames.RightToLeft);    
            RaiseEvent(EventRightToLeft, e);
            
        }
#if SELECTEDCHANGED
        protected virtual void OnSelectedChanged(EventArgs e) {
            RaiseEvent(EventSelectedChanged, e);  
        }
#endif
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnTextChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTextChanged(EventArgs e) {
           cachedTextSize = Size.Empty;
           // Make sure we clear the cache before we perform the layout.
           InvalidateItemLayout(PropertyNames.Text);
           RaiseEvent(EventText, e);  
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.OnVisibleChanged"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected virtual void OnVisibleChanged(System.EventArgs e) {
            if (Owner != null && !(Owner.IsDisposed || Owner.Disposing)) {
                Owner.OnItemVisibleChanged(new ToolStripItemEventArgs(this), /*performLayout*/true);
            }
            RaiseEvent(EventVisibleChanged, e);
            Animate();
        }

        public void PerformClick() {
            if (Enabled && Available) {
                FireEvent(ToolStripItemEventType.Click);
            }
        }

        /// <devdoc>
        ///    Pushes the button.
        /// </devdoc>
        /// <internalonly/>
        internal void Push(bool push) {
            if (!CanSelect || !Enabled || DesignMode) {
               return;
            }

            if (state[statePressed] != push) {
                state[statePressed] = push;   
                if (Available) {
                    this.Invalidate();
                }
            }
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ProcessDialogKey"]/*' />
        /// <devdoc>
        /// See Control.ProcessDialogKey for more info.
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal virtual bool ProcessDialogKey(Keys keyData) {
            // 
            if (keyData == Keys.Enter || (state[stateSupportsSpaceKey] && keyData == Keys.Space)) {
                FireEvent(ToolStripItemEventType.Click);
                if (ParentInternal != null && !ParentInternal.IsDropDown && !(AccessibilityImprovements.Level2 && !Enabled)) {
                    ParentInternal.RestoreFocusInternal();
                }
                return true;
            }
            return false;
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ProcessCmdKey"]/*' />
        /// <devdoc>
        /// See Control.ProcessCmdKey for more info.
        /// </devdoc>
        [
        SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
        SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected internal virtual bool ProcessCmdKey(ref Message m, Keys keyData) {
            return false;
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ProcessMnemonic"]/*' />
        
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'charCode' matches control.cs
        protected internal virtual bool ProcessMnemonic(char charCode) {
            // checking IsMnemonic is not necessary - control does this for us.
            FireEvent(ToolStripItemEventType.Click);
            return true;
        }

        
        internal void RaiseCancelEvent(object key, CancelEventArgs e) {
             CancelEventHandler handler = (CancelEventHandler)Events[key];
             if (handler != null) handler(this, e);  
        }

        internal void RaiseDragEvent(object key, DragEventArgs e) {
             DragEventHandler handler = (DragEventHandler)Events[key];
             if (handler != null) handler(this, e);  
        }
        internal void RaiseEvent(object key, EventArgs e) {
            EventHandler handler = (EventHandler)Events[key];
            if (handler != null) handler(this, e);
        }
        internal void RaiseKeyEvent(object key, KeyEventArgs e) {
            KeyEventHandler handler = (KeyEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }
        internal void RaiseKeyPressEvent(object key, KeyPressEventArgs e) {
            KeyPressEventHandler handler = (KeyPressEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }
        internal void RaiseMouseEvent(object key, MouseEventArgs e) {
            MouseEventHandler handler = (MouseEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }
        internal void RaisePaintEvent(object key, PaintEventArgs e) {
            PaintEventHandler handler = (PaintEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }
        
        internal void RaiseQueryContinueDragEvent(object key, QueryContinueDragEventArgs e) {
            QueryContinueDragEventHandler handler = (QueryContinueDragEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }

        private void ResetToolTipText() {
            toolTipText = null;
        }
        
       
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Select"]/*' />
        /// <devdoc>
        /// Selects the item
        /// </devdoc>
        public void Select() {
        
#if DEBUG
    // let's not snap the stack trace unless we're debugging selection.
    if (ToolStrip.SelectionDebug.TraceVerbose) {
        Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "[Selection DBG] WBI.Select: {0} \r\n{1}\r\n", this.ToString(), new StackTrace().ToString().Substring(0,200)));
    }
#endif
            if (!CanSelect) {
                return;
            }

            if (Owner != null && Owner.IsCurrentlyDragging) {
                // make sure we dont select during a drag operation.
                return;
            }
            if (ParentInternal != null && ParentInternal.IsSelectionSuspended) {                
                Debug.WriteLineIf(ToolStrip.SelectionDebug.TraceVerbose, "[Selection DBG] BAILING, selection is currently suspended");
                return;
            }
            
            if (!Selected) {
                state[stateSelected] = true;
                if (ParentInternal != null) { 
                    ParentInternal.NotifySelectionChange(this);
                    Debug.Assert(state[stateSelected], "calling notify selection change changed the selection state of this item");
                }
                if (IsOnDropDown) {
                    if (OwnerItem != null && OwnerItem.IsOnDropDown) {
                        // ensure the selection is moved back to our owner item.
                        OwnerItem.Select();
                    }
                }

                if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(this);
                }

                if (AccessibilityImprovements.Level3 && AccessibilityObject is ToolStripItemAccessibleObject) {
                    ((ToolStripItemAccessibleObject)AccessibilityObject).RaiseFocusChanged();
                }

#if SELECTEDCHANGED                
                OnSelectedChanged(EventArgs.Empty);
#endif
            }   
        }

        internal void SetOwner(ToolStrip newOwner) {
            if (owner != newOwner) {
               Font f = this.Font;
               owner = newOwner;

               // clear the parent if the owner is null...
               //
               if (newOwner == null) {
                   this.ParentInternal = null;
               }
               if (!state[stateDisposing] &&  !IsDisposed) {
                   OnOwnerChanged(EventArgs.Empty);
                   if (f != Font) {                
                      OnFontChanged(EventArgs.Empty);                
                   }
               }
           }

        }



        protected virtual void SetVisibleCore(bool visible) {
            if (state[stateVisible] != visible) {
                state[stateVisible]=  visible;
                Unselect();
                Push(false);

                OnAvailableChanged(EventArgs.Empty); 
                OnVisibleChanged(EventArgs.Empty);
            }
        }
      
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.SetBounds"]/*' />
        /// <devdoc>
        /// Sets the bounds of the item
        /// </devdoc>
        internal protected virtual void SetBounds(Rectangle bounds) {
            Rectangle oldBounds = this.bounds;
            this.bounds = bounds;

            if (! state[stateContstructing]) {
                // Dont fire while we're in the base constructor as the inherited
                // class may not have had a chance to initialize yet.

                if (this.bounds != oldBounds) {
                    OnBoundsChanged();
                }
                
                if (this.bounds.Location != oldBounds.Location) {
                    OnLocationChanged(EventArgs.Empty);
                }
            }
        }
          
        /// <devdoc>
        ///     Sets the bounds of the item
        /// </devdoc>
        internal void SetBounds(int x, int y, int width, int height) {
            SetBounds(new Rectangle(x,y,width,height));
        }
        /// <devdoc>
        ///     Sets the placement of the item
        /// </devdoc>
        internal void SetPlacement(ToolStripItemPlacement placement) {
            this.placement = placement;
        }  

        // Some implementations of DefaultMargin check which container they 
        // are on.  They need to be re-evaluated when the containership changes.
        // DefaultMargin will stop being honored the moment someone sets the Margin property.
        internal void SetAmbientMargin() {            
             if (state[stateUseAmbientMargin] && Margin != DefaultMargin) {
                 CommonProperties.SetMargin(this, DefaultMargin); 
             }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageTransparentColor() {
            return ImageTransparentColor != Color.Empty;
        }

        /// <devdoc>
        ///     Returns true if the backColor should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeBackColor() {
            Color backColor = Properties.GetColor(PropBackColor);
            return !backColor.IsEmpty;
        }

        private bool ShouldSerializeDisplayStyle() {
            return DisplayStyle != DefaultDisplayStyle;
        }
        
        private bool ShouldSerializeToolTipText() {
            return !string.IsNullOrEmpty(toolTipText);
        }
        /// <devdoc>
        ///     Returns true if the foreColor should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeForeColor() {
            Color foreColor = Properties.GetColor(PropForeColor);
            return !foreColor.IsEmpty;
        }

        /// <devdoc>
        ///     Returns true if the font should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeFont() {
            bool found;
            object font = Properties.GetObject(PropFont, out found);
            return (found && font != null);
        }
        
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Padding'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePadding() {
            return  (Padding != DefaultPadding);
        }         
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Margin'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMargin() {
            return  (Margin != DefaultMargin);
        }               
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Visible'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeVisible() {
            return  !state[stateVisible]; // only serialize if someone turned off visiblilty
        }
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Image'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImage() {
            return  (Image != null) && (ImageIndexer.ActualIndex < 0);
        }    
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Image'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageKey() {
            return  (Image != null) && (ImageIndexer.ActualIndex >= 0) && (ImageIndexer.Key != null && ImageIndexer.Key.Length != 0);
        }
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.Image'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageIndex() {
            return  (Image != null) &&  (ImageIndexer.ActualIndex >= 0) && (ImageIndexer.Index != -1);
        }
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.ToolStripItem.RightToLeft'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeRightToLeft() {
            bool found = false;
            int rightToLeft = Properties.GetInteger(PropRightToLeft, out found);
            if (!found) {
                return false;
            }
            return (rightToLeft != (int)DefaultRightToLeft);
        }

        private bool ShouldSerializeTextDirection() {
            ToolStripTextDirection textDirection = ToolStripTextDirection.Inherit;
            if (Properties.ContainsObject(ToolStripItem.PropTextDirection)) {
                textDirection= (ToolStripTextDirection)Properties.GetObject(ToolStripItem.PropTextDirection);
            }
            return textDirection != ToolStripTextDirection.Inherit;
        }

     
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ResetBackColor"]/*' />
        /// <devdoc>
        /// Resets the back color to be based on the parent's back color.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetBackColor() {
            BackColor = Color.Empty;
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetDisplayStyle() {
            DisplayStyle = DefaultDisplayStyle;
        }
        
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ResetForeColor"]/*' />
        /// <devdoc>
        /// Resets the fore color to be based on the parent's fore color.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetForeColor() {
            ForeColor = Color.Empty;
        }


        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ResetFont"]/*' />
        /// <devdoc>
        /// Resets the Font to be based on the parent's Font.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetFont() {
            Font = null;
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ResetImage"]/*' />
        /// <devdoc>
        /// Resets the back color to be based on the parent's back color.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetImage() {
            Image = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ResetImageTransparentColor() {
            ImageTransparentColor = Color.Empty;
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetMargin() {
            state[stateUseAmbientMargin] = true;
            SetAmbientMargin();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetPadding() {
            CommonProperties.ResetPadding(this);
        }
        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ResetRightToLeft"]/*' />
        /// <devdoc>
        /// Resets the RightToLeft to be the default.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetRightToLeft() {
            RightToLeft = RightToLeft.Inherit;
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ResetTextDirection"]/*' />
        /// <devdoc>
        /// Resets the TextDirection to be the default.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetTextDirection() {
             TextDirection = ToolStripTextDirection.Inherit;
        }

        /// <devdoc>
        /// Translates a point from one coordinate system to another
        /// </devdoc>
        internal Point TranslatePoint(Point fromPoint, ToolStripPointType fromPointType, ToolStripPointType toPointType) {          
            ToolStrip parent = ParentInternal;
            
            if (parent == null) {
                parent = (IsOnOverflow && Owner != null)? Owner.OverflowButton.DropDown : Owner;
            }
            if (parent == null) {
                // should not throw here as it's an internal function call.
                Debug.Fail("could not determine current parent");
                return fromPoint;
            }

            if (fromPointType == toPointType) { 
                return fromPoint;
            }

            Point toPoint = Point.Empty;
            Point currentToolStripItemLocation = Bounds.Location;

            // From: Screen
            // To:      ToolStrip or ToolStripItem
            if (fromPointType == ToolStripPointType.ScreenCoords) {
                // Convert ScreenCoords --> ToolStripCoords
                toPoint = parent.PointToClient(fromPoint);

                // Convert ToolStripCoords --> ToolStripItemCoords
                if (toPointType == ToolStripPointType.ToolStripItemCoords) {
                    toPoint.X += currentToolStripItemLocation.X;
                    toPoint.Y += currentToolStripItemLocation.Y;
                }
            }
                // From: ToolStrip or ToolStripItem
                // To:      Screen or ToolStripItem
            else {
                // Convert "fromPoint" ToolStripItemCoords --> ToolStripCoords
                if (fromPointType == ToolStripPointType.ToolStripItemCoords) {
                    fromPoint.X += currentToolStripItemLocation.X;
                    fromPoint.Y += currentToolStripItemLocation.Y;
                }

                // At this point, fromPoint is now in ToolStrip coordinates.

                // Convert ToolStripCoords --> ScreenCoords 
                if (toPointType == ToolStripPointType.ScreenCoords) {
                    toPoint = parent.PointToScreen(fromPoint);
                }
                    // Convert ToolStripCoords --> ToolStripItemCoords 
                else if (toPointType == ToolStripPointType.ToolStripItemCoords) {
                    fromPoint.X -= currentToolStripItemLocation.X;
                    fromPoint.Y -= currentToolStripItemLocation.Y;
                    toPoint = fromPoint;
                }           
                else {
                    Debug.Assert((toPointType == ToolStripPointType.ToolStripCoords), "why are we here! - investigate");
                    toPoint = fromPoint;
                }
               
            }
            return toPoint;
               
        }

        internal ToolStrip RootToolStrip {
            get {
                ToolStripItem item = this;
                while (item.OwnerItem != null) {
                    item = item.OwnerItem;
                }
                return item.ParentInternal;
            }
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.ToString"]/*' />
        /// <devdoc>
        /// ToString support
        /// </devdoc>
        public override string ToString() {
            if (Text != null && this.Text.Length != 0) {
                return this.Text;
            }
            return base.ToString();
        }

        /// <devdoc>
        ///   removes selection bits from item state
        /// </devdoc>
        internal void Unselect() {
            Debug.WriteLineIf(ToolStrip.SelectionDebug.TraceVerbose, String.Format(CultureInfo.CurrentCulture, "[Selection DBG] WBI.Unselect: {0}", this.ToString()));
            if (state[stateSelected]) {
                state[stateSelected] = false;   
                if (Available) {
                    Invalidate();
                    if (ParentInternal != null) {
                        ParentInternal.NotifySelectionChange(this);
                    }

                    if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                        KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                    }
#if SELECTEDCHANGED                
                    OnSelectedChanged(EventArgs.Empty);
#endif
                }
            }
        }

        #region IKeyboardToolTip implementation

        bool IKeyboardToolTip.CanShowToolTipsNow() {
            return this.Visible && this.parent != null && ((IKeyboardToolTip)this.parent).AllowsChildrenToShowToolTips();
        }

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle() {
            return this.AccessibilityObject.Bounds;
        }

        IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles() {
            List<Rectangle> neighbors = new List<Rectangle>(3);
            if (this.parent != null) {
                ToolStripItemCollection items = this.parent.DisplayedItems;
                int i = 0, count = items.Count;
                bool found = false;
                while (!found && i < count) {
                    found = Object.ReferenceEquals(items[i], this);
                    if (found) {
                        int previousIndex = i - 1;
                        if (previousIndex >= 0) {
                            neighbors.Add(((IKeyboardToolTip)items[previousIndex]).GetNativeScreenRectangle());
                        }

                        int nextIndex = i + 1;
                        if (nextIndex < count) {
                            neighbors.Add(((IKeyboardToolTip)items[nextIndex]).GetNativeScreenRectangle());
                        }
                    }
                    else {
                        i++;
                    }
                }
                Debug.Assert(i < count, "Item has a parent set but the parent doesn't own the item");
            }

            ToolStripDropDown dropDown = this.parent as ToolStripDropDown;
            if (dropDown != null && dropDown.OwnerItem != null) {
                neighbors.Add(((IKeyboardToolTip)dropDown.OwnerItem).GetNativeScreenRectangle());
            }

            return neighbors;
        }

        bool IKeyboardToolTip.IsHoveredWithMouse() {
            return ((IKeyboardToolTip)this).GetNativeScreenRectangle().Contains(Control.MousePosition);
        }

        bool IKeyboardToolTip.HasRtlModeEnabled() {
            return this.parent != null && ((IKeyboardToolTip)this.parent).HasRtlModeEnabled();
        }

        bool IKeyboardToolTip.AllowsToolTip() {
            return true;
        }

        IWin32Window IKeyboardToolTip.GetOwnerWindow() {
            Debug.Assert(this.ParentInternal != null, "Tool Strip Item Parent is null");
            return this.ParentInternal;
        }

        void IKeyboardToolTip.OnHooked(ToolTip toolTip) {
            this.OnKeyboardToolTipHook(toolTip);
        }

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) {
            this.OnKeyboardToolTipUnhook(toolTip);
        }

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) {
            return this.ToolTipText;
        }

        bool IKeyboardToolTip.ShowsOwnToolTip() {
            return true;
        }

        bool IKeyboardToolTip.IsBeingTabbedTo() {
            return this.IsBeingTabbedTo();
        }

        bool IKeyboardToolTip.AllowsChildrenToShowToolTips() {
            return true;
        }

        #endregion

        internal virtual void OnKeyboardToolTipHook(ToolTip toolTip) {
        }

        internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip) {
        }

        internal virtual bool IsBeingTabbedTo() {
            return ToolStrip.AreCommonNavigationalKeysDown();
        }

        /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject"]/*' />
        /// <devdoc>
        /// An implementation of AccessibleChild for use with ToolStripItems        
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]        
        public class ToolStripItemAccessibleObject : AccessibleObject {

            // Member variables

            private ToolStripItem ownerItem = null; // The associated ToolStripItem for this AccessibleChild (if any)
          
            private  AccessibleStates additionalState = AccessibleStates.None;  // Test hook for the designer

            private int[] runtimeId = null; // Used by UIAutomation
            // constructors

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.ToolStripItemAccessibleObject"]/*' />
            public ToolStripItemAccessibleObject(ToolStripItem ownerItem) {

                Debug.Assert(ownerItem != null, "Cannot construct a ToolStripItemAccessibleObject with a null ownerItem");
                if (ownerItem == null) {
                    throw new ArgumentNullException(nameof(ownerItem));
                }

                this.ownerItem = ownerItem;
            }
            
            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction {
                get {
                    string defaultAction = ownerItem.AccessibleDefaultActionDescription;
                    if (defaultAction != null) {
                        return defaultAction;
                    }
                    else {
                        return SR.AccessibleActionPress;
                    }
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Description"]/*' />
            public override string Description {
                get {
                    string description = ownerItem.AccessibleDescription;
                    if (description != null) {
                        return description;
                    }
                    else {
                        return base.Description;
                    }
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Help"]/*' />
            public override string Help {
                get {
                    QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Owner.Events[ToolStripItem.EventQueryAccessibilityHelp];

                    if (handler != null) {
                        QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                        handler(Owner, args);
                        return args.HelpString;
                    }
                    else {
                        return base.Help;
                    }
                }
            }  

            public override string KeyboardShortcut {
                get {
                    // This really is the Mnemonic - NOT the shortcut.  E.g. in notepad Edit->Replace is Control+H 
                    // but the KeyboardShortcut comes up as the mnemonic 'r'.
                    char mnemonic = WindowsFormsUtils.GetMnemonic(ownerItem.Text, false);
                    if (ownerItem.IsOnDropDown) {
                        // no ALT on dropdown
                        return (mnemonic == (char) 0) ? string.Empty :  mnemonic.ToString();
                    }
                    return (mnemonic == (char) 0) ? string.Empty : ("Alt+" + mnemonic);
                    
                }
            }
            
            internal override int[] RuntimeId {
                get {
                    if (AccessibilityImprovements.Level1) {
                        if (runtimeId == null) {
                            // we need to provide a unique ID
                            // others are implementing this in the same manner
                            // first item is static - 0x2a
                            // [AccessibilityImprovements.Level3] first item should be UiaAppendRuntimeId since this is not a top-level element of the fragment.
                            // second item can be anything, but here it is a hash. For toolstrip hash is unique even with child controls. Hwnd  is not.

                            runtimeId = new int[2];
                            runtimeId[0] = AccessibilityImprovements.Level3 ? NativeMethods.UiaAppendRuntimeId : 0x2a;
                            runtimeId[1] = ownerItem.GetHashCode();
                        }
                        return runtimeId;
                    }
                    else {
                        return base.RuntimeId;
                    }
                }
            }

            internal override object GetPropertyValue(int propertyID) {

                if (AccessibilityImprovements.Level1) {
                    if (propertyID == NativeMethods.UIA_NamePropertyId) {
                        return Name;
                    }
                    else if (propertyID == NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId) {
                        return (Object)this.IsPatternSupported(NativeMethods.UIA_ExpandCollapsePatternId);
                    }
                }

                if (AccessibilityImprovements.Level3) {
                    switch (propertyID) {
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return ownerItem.Enabled;
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return ownerItem.Placement != ToolStripItemPlacement.Main;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                            return ownerItem.CanSelect;
                        case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                            return ownerItem.Selected;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return KeyboardShortcut;
                        case NativeMethods.UIA_IsPasswordPropertyId:
                            return false;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            return Help;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Name"]/*' />
            public override string Name {
                get {
                    string name = ownerItem.AccessibleName;
                    if (name != null) {
                        return name;
                    }
                    
                    string baseName = base.Name;
                    
                    if (baseName == null || baseName.Length == 0) {
                        return WindowsFormsUtils.TextWithoutMnemonics(ownerItem.Text);
                    }
                        
                    return baseName;
                }
                set {
                    ownerItem.AccessibleName = value;
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Owner"]/*' />
            internal ToolStripItem Owner {
                get {
                    return ownerItem;
                }
            }
            
            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Role"]/*' />
            public override AccessibleRole Role {
                get {
                    AccessibleRole role = ownerItem.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    else {
                        return AccessibleRole.PushButton;
                    }

                }
            }

            public override AccessibleStates State {
                get {
                    if (!ownerItem.CanSelect) {
                        return base.State | additionalState;
                    }
                    
                    
                    if (!ownerItem.Enabled) {

                        if (AccessibilityImprovements.Level2) {
                            if (ownerItem.Selected && ownerItem is ToolStripMenuItem) {
                                return AccessibleStates.Unavailable | additionalState | AccessibleStates.Focused;
                            }
                        }

                        // Disabled menu items that are selected must have focus
                        // state so that Narrator can announce them.
                        if (AccessibilityImprovements.Level1) {
                            if (ownerItem.Selected && ownerItem is ToolStripMenuItem) {
                                return AccessibleStates.Focused;
                            }
                        }

                        return AccessibleStates.Unavailable | additionalState;
                    }
                    
                    AccessibleStates accState = AccessibleStates.Focusable | additionalState;
                    
                    // 
                    /*if (HasDropDownItems) {
                        accState |= AccessibleState.HasPopup;
                    }*/

                    if (ownerItem.Selected || ownerItem.Pressed) {
                        accState |= AccessibleStates.Focused | AccessibleStates.HotTracked;
                    }
                    if (ownerItem.Pressed) {
                        accState |= AccessibleStates.Pressed;
                    }
                    return accState; 
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                if (Owner != null) {
                    ((ToolStripItem)Owner).PerformClick();
                }
            }
            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.GetHelpTopic"]/*' />
            public override int GetHelpTopic(out string fileName) {
                int topic = 0;

                QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Owner.Events[ToolStripItem.EventQueryAccessibilityHelp];                                     

                if (handler != null) {
                    QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                    handler(Owner, args);

                    fileName = args.HelpNamespace;                             
                    if (fileName != null && fileName.Length > 0) {
                        IntSecurity.DemandFileIO(FileIOPermissionAccess.PathDiscovery, fileName);
                    }

                    try {
                        topic = Int32.Parse(args.HelpKeyword, CultureInfo.InvariantCulture);
                    }
                    catch {
                    }

                    return topic;
                }
                else {
                    return base.GetHelpTopic(out fileName);
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection) {
                ToolStripItem nextItem = null;

                if (Owner != null) {
                    ToolStrip parent =  Owner.ParentInternal;
                    if (parent == null) {
                        return null;
                    }
                    
                    bool forwardInCollection = (parent.RightToLeft == RightToLeft.No);
                    switch(navigationDirection) {
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
                            nextItem =  (Owner.IsOnDropDown) ? parent.GetNextItem(Owner, ArrowDirection.Up):
                                                               parent.GetNextItem(Owner, ArrowDirection.Left, /*RTLAware=*/true);
                            break;                     
                        case AccessibleNavigation.Down:
                            nextItem =  (Owner.IsOnDropDown) ? parent.GetNextItem(Owner, ArrowDirection.Down):
                                                               parent.GetNextItem(Owner, ArrowDirection.Right, /*RTLAware=*/true);
                            break;                     
                             
                    }


                }
                if (nextItem != null) {
                    return nextItem.AccessibilityObject;
                }
                return null;     
            }

            // 
            public void AddState(AccessibleStates state) {
                if (state == AccessibleStates.None) {
                    additionalState = state;
                }
                else {
                    additionalState |= state;
                }
            }
           
            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.ToString"]/*' />
            public override string ToString() {
                if (Owner != null) {
                    return "ToolStripItemAccessibleObject: Owner = " + Owner.ToString();                    
                }
                else {
                    return "ToolStripItemAccessibleObject: Owner = null";
                }
     
            }
          
            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Bounds"]/*' />
            /// <devdoc>
            /// <para> Gets the bounds of the accessible object, in screen coordinates.</para>
            /// </devdoc>
            public override Rectangle Bounds {
                get {
                    Rectangle bounds = Owner.Bounds;
                    if (Owner.ParentInternal != null &&
                        Owner.ParentInternal.Visible) {
                        return new Rectangle(Owner.ParentInternal.PointToScreen(bounds.Location), bounds.Size);
                    }
                    return Rectangle.Empty;
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItemAccessibleObject.Parent"]/*' />
            /// <devdoc>
            /// <para>When overridden in a derived class, gets or sets the parent of an accessible object.</para>
            /// </devdoc>
            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    if (Owner.IsOnDropDown) {
                        // Return the owner item as the accessible parent.
                        ToolStripDropDown dropDown = Owner.GetCurrentParentDropDown();
                        if (dropDown.OwnerItem != null) {
                            return dropDown.OwnerItem.AccessibilityObject;
                        }
                        return dropDown.AccessibilityObject;                            
                    }
                    return (Owner.Parent != null) ? Owner.Parent.AccessibilityObject : base.Parent; 


                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot {
                get {
                    return ownerItem.RootToolStrip?.AccessibilityObject;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                switch (direction) {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return Parent;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        ToolStrip.ToolStripAccessibleObject parent = Parent as ToolStrip.ToolStripAccessibleObject;

                        if (parent == null) {
                            return null; 
                        }

                        int index = parent.GetChildIndex(this);

                        if (index == -1) {
                            Debug.Fail("No item matched the index?");
                            return null;
                        }

                        int increment = direction == UnsafeNativeMethods.NavigateDirection.NextSibling ? 1 : -1;
                        AccessibleObject sibling = null;
                        // Skipping contol host items, as they are provided by system
                        do {
                            index += increment;
                            sibling = index >= 0 && index < parent.GetChildCount() ? parent.GetChild(index) : null;
                        } while (sibling != null && sibling is Control.ControlAccessibleObject);

                        return sibling;
                }

                return base.FragmentNavigate(direction);
            }

            internal override void SetFocus() {
                Owner.Select();
            }

            internal void RaiseFocusChanged() {
                ToolStrip root = ownerItem.RootToolStrip;
                if (root != null && root.SupportsUiaProviders) {
                    RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
                }
            }
        }

    }

    // We need a special way to defer to the ToolStripItem's image
    // list for indexing purposes.
    internal class ToolStripItemImageIndexer : ImageList.Indexer {
       private ToolStripItem item;
       
       public ToolStripItemImageIndexer(ToolStripItem item) {
          this.item = item;
       }
       
       public override ImageList ImageList {
            get { 
                    if ((item != null) && (item.Owner != null)) {
                        return item.Owner.ImageList;
                    }
                    return null;
                }
            set { Debug.Assert(false, "We should never set the image list"); }
        }
    }
    
    
    
     
    /// <devdoc>
    ///    <para>
    ///       This class helps determine where the image and text should be drawn.
    ///    </para>
    /// </devdoc>
    /// <internalonly/>
    internal class ToolStripItemInternalLayout { 
    
        private ToolStripItemLayoutOptions currentLayoutOptions;
        private ToolStripItem ownerItem;
        private ButtonBaseAdapter.LayoutData layoutData;
        private const int BORDER_WIDTH = 2;
        private const int BORDER_HEIGHT = 3;
        private readonly static Size INVALID_SIZE = new Size(Int32.MinValue,Int32.MinValue);
        
        private Size lastPreferredSize = INVALID_SIZE;
        private ToolStripLayoutData parentLayoutData = null;

     
        public ToolStripItemInternalLayout(ToolStripItem ownerItem) {
            if (ownerItem == null) {
               throw new ArgumentNullException(nameof(ownerItem));
            }
            this.ownerItem = ownerItem;
        }

      
        // the thing that we fetch properties off of -- this can be different than ownerItem - e.g. case of split button.
        protected virtual ToolStripItem Owner {
            get { return ownerItem; }                       
        }
                  
        public virtual Rectangle ImageRectangle {
            get {               
                Rectangle imageRect = LayoutData.imageBounds;
                imageRect.Intersect(layoutData.field);
                return imageRect;
            }
        }

       

        internal ButtonBaseAdapter.LayoutData LayoutData {
           get {
               EnsureLayout();
               return layoutData;
           }
        }

        public Size PreferredImageSize {
            get {
                return Owner.PreferredImageSize;
            }
        }


        protected virtual ToolStrip ParentInternal {
          get {
              return (ownerItem != null) ? ownerItem.ParentInternal : null;
          }
        }
                     
        public virtual Rectangle TextRectangle {
            get { 
                Rectangle textRect = LayoutData.textBounds;
                textRect.Intersect(layoutData.field);
                return textRect;                
            }
        }

        public virtual Rectangle ContentRectangle {
            get {
                return LayoutData.field;
            }
        }

        public virtual TextFormatFlags TextFormat {
            get {
                if (currentLayoutOptions != null) {
                    return currentLayoutOptions.gdiTextFormatFlags;
                }
                return CommonLayoutOptions().gdiTextFormatFlags;
            }
        }


        internal static TextFormatFlags ContentAlignToTextFormat(ContentAlignment alignment, bool rightToLeft) {
            TextFormatFlags textFormat = TextFormatFlags.Default;  
            if (rightToLeft) {
               //We specifically do not want to turn on TextFormatFlags.Right.
               textFormat |= TextFormatFlags.RightToLeft;
            }
     
            // Calculate Text Positioning
            textFormat |= ControlPaint.TranslateAlignmentForGDI(alignment);
            textFormat |= ControlPaint.TranslateLineAlignmentForGDI(alignment);
            return textFormat;
 
        }

        
        protected virtual ToolStripItemLayoutOptions CommonLayoutOptions() {
            ToolStripItemLayoutOptions layoutOptions = new ToolStripItemLayoutOptions();
            Rectangle bounds = new Rectangle(Point.Empty, ownerItem.Size);   
            
            layoutOptions.client = bounds;
            
            layoutOptions.growBorderBy1PxWhenDefault = false;

            layoutOptions.borderSize         = BORDER_WIDTH;
            layoutOptions.paddingSize        = 0;
            layoutOptions.maxFocus           = true;
            layoutOptions.focusOddEvenFixup  = false;
            layoutOptions.font               = ownerItem.Font;
            layoutOptions.text               = ((Owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) ? Owner.Text : String.Empty;
            layoutOptions.imageSize          = PreferredImageSize;
            layoutOptions.checkSize          = 0;
            layoutOptions.checkPaddingSize   = 0;
            layoutOptions.checkAlign         = ContentAlignment.TopLeft;
            layoutOptions.imageAlign         = Owner.ImageAlign;
            layoutOptions.textAlign          = Owner.TextAlign;
            layoutOptions.hintTextUp         = false;
            layoutOptions.shadowedText       = !ownerItem.Enabled;
            layoutOptions.layoutRTL          = RightToLeft.Yes == Owner.RightToLeft;
            layoutOptions.textImageRelation  = Owner.TextImageRelation;
            //set textImageInset to 0 since we don't draw 3D border for ToolStripItems.
            layoutOptions.textImageInset     = 0;
            layoutOptions.everettButtonCompat = false;


            // Support RTL
            layoutOptions.gdiTextFormatFlags = ContentAlignToTextFormat(Owner.TextAlign, Owner.RightToLeft == RightToLeft.Yes); 
            
            
            // in 2K and XP++ hide underlined &File unless ALT is pressed
            layoutOptions.gdiTextFormatFlags = (Owner.ShowKeyboardCues) ? layoutOptions.gdiTextFormatFlags : layoutOptions.gdiTextFormatFlags | TextFormatFlags.HidePrefix;
            
            return layoutOptions;
        }

        private bool EnsureLayout() {
            
            if (layoutData == null || parentLayoutData == null || !parentLayoutData.IsCurrent(ParentInternal)) {
                PerformLayout();
                return true;
            }
            return false;
        }

        private ButtonBaseAdapter.LayoutData GetLayoutData() {
            
            currentLayoutOptions = CommonLayoutOptions();
          
            if (Owner.TextDirection != ToolStripTextDirection.Horizontal) {
                currentLayoutOptions.verticalText = true;
            }
    
            ButtonBaseAdapter.LayoutData data =  currentLayoutOptions.Layout();
            return data;
        
        }
        public virtual Size GetPreferredSize(Size constrainingSize) {
            Size preferredSize = Size.Empty;
            EnsureLayout();
            // we would prefer not to be larger than the winbar itself.
            // so we'll ask the ButtonAdapter layout guy what it thinks
            // its preferred size should be - and we'll tell it to be no 
            // bigger than the ToolStrip itself.  Note this is "Parent" not
            // "Owner" because we care in this instance what we're currently displayed on.

            if (ownerItem != null) {
                lastPreferredSize = currentLayoutOptions.GetPreferredSizeCore(constrainingSize);    
                return lastPreferredSize;
            }
            Debug.Fail("Why are we here without an owner?");
            return Size.Empty;
        }


        internal void PerformLayout() {
            layoutData = GetLayoutData();
            ToolStrip parent = ParentInternal;
            if (parent != null) {
                parentLayoutData = new ToolStripLayoutData(parent);
            }
            else {
                parentLayoutData = null;
            }
        }

        internal class ToolStripItemLayoutOptions : ButtonBaseAdapter.LayoutOptions {
            Size cachedSize = LayoutUtils.InvalidSize;
            Size cachedProposedConstraints = LayoutUtils.InvalidSize;
            
            // override GetTextSize to provide simple text caching.
            protected override Size GetTextSize(Size proposedConstraints) {
                if (cachedSize != LayoutUtils.InvalidSize 
                    && (cachedProposedConstraints == proposedConstraints
                       || cachedSize.Width <= proposedConstraints.Width)) {
                    return cachedSize;
                }
                else {
                    cachedSize = base.GetTextSize(proposedConstraints);
                    cachedProposedConstraints = proposedConstraints;
                }
                return cachedSize;
                
            }

        }
        private class ToolStripLayoutData {
            private ToolStripLayoutStyle layoutStyle;
            private bool autoSize;
            private Size size;
            
            public ToolStripLayoutData(ToolStrip toolStrip) {
                this.layoutStyle = toolStrip.LayoutStyle;
                this.autoSize = toolStrip.AutoSize;
                this.size = toolStrip.Size;
            }
            public bool IsCurrent(ToolStrip toolStrip) {
                if (toolStrip == null) {
                    return false;
                }
                return (toolStrip.Size == size && toolStrip.LayoutStyle == layoutStyle && toolStrip.AutoSize == autoSize);
            }
        }
    }


}

