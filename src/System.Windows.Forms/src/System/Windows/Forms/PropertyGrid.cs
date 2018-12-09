// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms.ComponentModel.Com2Interop;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.PropertyGridInternal;
    using Microsoft.Win32;

    /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid"]/*' />
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Designer("System.Windows.Forms.Design.PropertyGridDesigner, " + AssemblyRef.SystemDesign)]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
    [SRDescription(nameof(SR.DescriptionPropertyGrid))]
    public class PropertyGrid : ContainerControl, IComPropertyBrowser, UnsafeNativeMethods.IPropertyNotifySink {

        private DocComment                          doccomment;
        private int                                 dcSizeRatio = -1;
        private int                                 hcSizeRatio = -1;
        private HotCommands                         hotcommands;
        private ToolStrip                           toolStrip;
        
        private bool                                helpVisible = true;
        private bool                                toolbarVisible = true;

        private ImageList[]                         imageList = new ImageList[2];
        private Bitmap                              bmpAlpha;
        private Bitmap                              bmpCategory;
        private Bitmap                              bmpPropPage;

        // our array of viewTabs
        private bool                                viewTabsDirty = true;
        private bool                                drawFlatToolBar = false;
        private PropertyTab[]                       viewTabs = new PropertyTab[0];
        private PropertyTabScope[]                  viewTabScopes = new PropertyTabScope[0];
        private Hashtable                           viewTabProps;

        // the tab view buttons
        private ToolStripButton[]                   viewTabButtons;
        // the index of the currently selected tab view
        private int                                 selectedViewTab;
        
        
        // our view type buttons (Alpha vs. categorized)
        private ToolStripButton[]                   viewSortButtons; 
        private int                                 selectedViewSort;   
        private PropertySort                        propertySortValue;

        // this guy's kind of an odd one...he gets special treatment
        private ToolStripButton                     btnViewPropertyPages;
        private ToolStripSeparator                  separator1;
        private ToolStripSeparator                  separator2;
        private int                                 buttonType = NORMAL_BUTTONS;

        // our main baby
        private PropertyGridView                    gridView;

        
        private IDesignerHost                       designerHost;
        private IDesignerEventService               designerEventService;
        
        private Hashtable                           designerSelections;

        private GridEntry peDefault;
        private GridEntry peMain;
        private GridEntryCollection currentPropEntries;
        private Object[]   currentObjects;
        
        private int                                 paintFrozen;
        private Color                               lineColor = SystemInformation.HighContrast ? (AccessibilityImprovements.Level1 ? SystemColors.ControlDarkDark : SystemColors.ControlDark )
                                                                : SystemColors.InactiveBorder;
        internal bool                               developerOverride = false;
        internal Brush                              lineBrush = null;
        private Color                               categoryForeColor = SystemColors.ControlText;
        private Color                               categorySplitterColor = SystemColors.Control;
        private Color                               viewBorderColor = SystemColors.ControlDark;
        private Color                               selectedItemWithFocusForeColor = SystemColors.HighlightText;
        private Color                               selectedItemWithFocusBackColor = SystemColors.Highlight;
        internal Brush                              selectedItemWithFocusBackBrush = null;
        private bool                                canShowVisualStyleGlyphs = true;

        private AttributeCollection browsableAttributes;

        private SnappableControl                    targetMove = null;
        private int                                 dividerMoveY = -1;
        private const int                    CYDIVIDER = 3;
        private static int                   cyDivider = CYDIVIDER;
        private const int                    CXINDENT = 0;
        private const int                    CYINDENT = 2;
        private const int                    MIN_GRID_HEIGHT = 20;

        private const int                    PROPERTIES = 0;
        private const int                    EVENTS = 1;
        private const int                    ALPHA = 1;
        private const int                    CATEGORIES = 0;
        private const int                    NO_SORT = 2;

        private const int                    NORMAL_BUTTONS = 0;
        private const int                    LARGE_BUTTONS = 1;

        private const int                    TOOLSTRIP_BUTTON_PADDING_Y = 9;
        private int                          toolStripButtonPaddingY = TOOLSTRIP_BUTTON_PADDING_Y;
        private static readonly Size         DEFAULT_LARGE_BUTTON_SIZE = new Size(32, 32);
        private static readonly Size         DEFAULT_NORMAL_BUTTON_SIZE = new Size(16, 16);
        private static Size                  largeButtonSize = DEFAULT_LARGE_BUTTON_SIZE;
        private static Size                  normalButtonSize = DEFAULT_NORMAL_BUTTON_SIZE;
        private static bool                  isScalingInitialized = false;

        private const ushort                  PropertiesChanged          = 0x0001;
        private const ushort                  GotDesignerEventService    = 0x0002;
        private const ushort                  InternalChange             = 0x0004;
        private const ushort                  TabsChanging               = 0x0008;
        private const ushort                  BatchMode                  = 0x0010;
        private const ushort                  ReInitTab                  = 0x0020;
        private const ushort                  SysColorChangeRefresh      = 0x0040;
        private const ushort                  FullRefreshAfterBatch      = 0x0080;
        private const ushort                  BatchModeChange            = 0x0100;
        private const ushort                  RefreshingProperties       = 0x0200;

        private ushort                  flags;

        private bool GetFlag(ushort flag) {
            return (flags & flag) != (ushort)0;
        }

        private void SetFlag(ushort flag, bool value) {
            if (value) {
                flags |= flag;
            }
            else {
                flags &= (ushort)~flag;
            }
        }


        private readonly ComponentEventHandler                  onComponentAdd;
        private readonly ComponentEventHandler                  onComponentRemove;
        private readonly ComponentChangedEventHandler           onComponentChanged;
        
        // the cookies for our connection points on objects that support IPropertyNotifySink
        //
        private AxHost.ConnectionPointCookie[] connectionPointCookies = null;

        private static object          EventPropertyValueChanged = new object();
        private static object          EventComComponentNameChanged = new object();
        private static object          EventPropertyTabChanged = new object();
        private static object          EventSelectedGridItemChanged = new object();
        private static object          EventPropertySortChanged = new object();
        private static object          EventSelectedObjectsChanged = new object();
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyGrid"]/*' />
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // the "PropertyGridToolBar" caption is for testing.
                                                                                                        // So we don't have to localize it.
        ]
        public PropertyGrid()  {

            onComponentAdd = new ComponentEventHandler(OnComponentAdd);
            onComponentRemove = new ComponentEventHandler(OnComponentRemove);
            onComponentChanged = new ComponentChangedEventHandler(OnComponentChanged);

            SuspendLayout();
            AutoScaleMode = AutoScaleMode.None;

            // static variables are problem in a child level mixed mode scenario. Changing static variables cause compatibility issue. 
            // So, recalculate static variables everytime property grid initialized.
            if (DpiHelper.IsPerMonitorV2Awareness) {
                RescaleConstants();
            }
            else {
                if (!isScalingInitialized) {
                    if (DpiHelper.IsScalingRequired) {
                        normalButtonSize = LogicalToDeviceUnits(DEFAULT_NORMAL_BUTTON_SIZE);
                        largeButtonSize = LogicalToDeviceUnits(DEFAULT_LARGE_BUTTON_SIZE);
                    }
                    isScalingInitialized = true;
                }
            }

            try
            {
                gridView = CreateGridView(null);
                gridView.TabStop = true;
                gridView.MouseMove += new MouseEventHandler(this.OnChildMouseMove);
                gridView.MouseDown += new MouseEventHandler(this.OnChildMouseDown);
                gridView.TabIndex = 2;

                separator1 = CreateSeparatorButton();
                separator2 = CreateSeparatorButton();

                toolStrip = new ToolStrip();
                toolStrip.SuspendLayout();
                toolStrip.ShowItemToolTips = true;
                toolStrip.AccessibleName = SR.PropertyGridToolbarAccessibleName;
                toolStrip.AccessibleRole = AccessibleRole.ToolBar;
                toolStrip.TabStop = true;
                toolStrip.AllowMerge = false;

                // This caption is for testing.
                toolStrip.Text = "PropertyGridToolBar";

                // LayoutInternal handles positioning, and for perf reasons, we manually size.
                toolStrip.Dock = DockStyle.None;
                toolStrip.AutoSize = false;
                toolStrip.TabIndex = 1;
                toolStrip.ImageScalingSize = normalButtonSize;

                // parity with the old... 
                toolStrip.CanOverflow = false;


                // hide the grip but add in a few more pixels of padding.
                toolStrip.GripStyle = ToolStripGripStyle.Hidden;
                Padding toolStripPadding = toolStrip.Padding;
                toolStripPadding.Left = 2;
                toolStrip.Padding = toolStripPadding;
                SetToolStripRenderer();


                // always add the property tab here
                AddRefTab(DefaultTabType, null, PropertyTabScope.Static, true);

                doccomment = new DocComment(this);
                doccomment.SuspendLayout();
                doccomment.TabStop = false;
                doccomment.Dock = DockStyle.None;
                doccomment.BackColor = SystemColors.Control;
                doccomment.ForeColor = SystemColors.ControlText;
                doccomment.MouseMove += new MouseEventHandler(this.OnChildMouseMove);
                doccomment.MouseDown += new MouseEventHandler(this.OnChildMouseDown);



                hotcommands = new HotCommands(this);
                hotcommands.SuspendLayout();
                hotcommands.TabIndex = 3;
                hotcommands.Dock = DockStyle.None;
                SetHotCommandColors(false);
                hotcommands.Visible = false;
                hotcommands.MouseMove += new MouseEventHandler(this.OnChildMouseMove);
                hotcommands.MouseDown += new MouseEventHandler(this.OnChildMouseDown);
              
                Controls.AddRange(new Control[] { doccomment, hotcommands, gridView, toolStrip });

                SetActiveControlInternal(gridView);
                toolStrip.ResumeLayout(false);  // SetupToolbar should perform the layout
                SetupToolbar();
                this.PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
                this.Text = "PropertyGrid";
                SetSelectState(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally {
                if (doccomment != null) {
                    doccomment.ResumeLayout(false);
                }
                if (hotcommands != null) {
                    hotcommands.ResumeLayout(false);
                }
                ResumeLayout(true);
            }
        }

        internal IDesignerHost ActiveDesigner {
            get{
                if (this.designerHost == null) {
                    designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
                }
                return this.designerHost;
            }
            set{
                if (value != designerHost) {
                    SetFlag(ReInitTab, true);
                    if (this.designerHost != null) {
                        IComponentChangeService cs = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
                        if (cs != null) {
                            cs.ComponentAdded -= onComponentAdd;
                            cs.ComponentRemoved -= onComponentRemove;
                            cs.ComponentChanged -= onComponentChanged;
                        }
                        
                        IPropertyValueUIService pvSvc = (IPropertyValueUIService)designerHost.GetService(typeof(IPropertyValueUIService));
                        if (pvSvc != null) {
                            pvSvc.PropertyUIValueItemsChanged -= new EventHandler(this.OnNotifyPropertyValueUIItemsChanged);
                        }
                        
                        designerHost.TransactionOpened -= new EventHandler(this.OnTransactionOpened);
                        designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
                        SetFlag(BatchMode, false);
                        RemoveTabs(PropertyTabScope.Document, true);
                        this.designerHost = null;
                    }

                    

                    if (value != null) {
                        IComponentChangeService cs = (IComponentChangeService)value.GetService(typeof(IComponentChangeService));
                        if (cs != null) {
                            cs.ComponentAdded += onComponentAdd;
                            cs.ComponentRemoved += onComponentRemove;
                            cs.ComponentChanged += onComponentChanged;
                        }

                        value.TransactionOpened += new EventHandler(this.OnTransactionOpened);
                        value.TransactionClosed += new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
                        SetFlag(BatchMode, false);
                        
                        IPropertyValueUIService pvSvc = (IPropertyValueUIService)value.GetService(typeof(IPropertyValueUIService));
                        if (pvSvc != null) {
                            pvSvc.PropertyUIValueItemsChanged += new EventHandler(this.OnNotifyPropertyValueUIItemsChanged);
                        }
                    }
                    
                    designerHost = value;
                    if (peMain != null) {
                        peMain.DesignerHost = value;
                    }
                    RefreshTabs(PropertyTabScope.Document);
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.AutoScroll"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll {
            get {
                return base.AutoScroll;
            }
            set {
                base.AutoScroll = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.BackColor"]/*' />
        public override Color BackColor {
            get {
                return base.BackColor;
            }
            set {
                base.BackColor = value;
                toolStrip.BackColor = value;
                toolStrip.Invalidate(true);
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.BrowsableAttributes"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ] 
        public AttributeCollection BrowsableAttributes {
            set {
                if (value == null || value == AttributeCollection.Empty) {
                    browsableAttributes = new AttributeCollection(new Attribute[]{BrowsableAttribute.Yes});
                }
                else {
                    Attribute[] attributes = new Attribute[value.Count];
                    value.CopyTo(attributes, 0);
                    browsableAttributes = new AttributeCollection(attributes);
                }
                if (currentObjects != null && currentObjects.Length > 0) {
                    if (peMain != null) {
                        peMain.BrowsableAttributes = BrowsableAttributes;
                        Refresh(true);
                    }
                }
            }
            get {
                if (browsableAttributes == null) {
                    browsableAttributes = new AttributeCollection(new Attribute[]{new BrowsableAttribute(true)});
                }
                return browsableAttributes;
            }
        }
        
        private bool CanCopy {
            get {
                return gridView.CanCopy;
            }
        }
        
        private bool CanCut {
            get {
                return gridView.CanCut;
            }
        }
        
        private bool CanPaste {
            get {
                return gridView.CanPaste;
            }
        }
        
        private bool CanUndo {
            get {
                return gridView.CanUndo;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CanShowCommands"]/*' />
        /// <devdoc>
        /// true if the commands pane will be can be made visible
        /// for the currently selected objects.  Objects that
        /// expose verbs can show commands.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.PropertyGridCanShowCommandsDesc))]
        public virtual bool CanShowCommands {
            get {
                return hotcommands.WouldBeVisible;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CategoryForeColor"]/*' />
        /// <devdoc>
        /// The text used color for category headings. The background color is determined by the LineColor property.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCategoryForeColorDesc)),
        DefaultValue(typeof(Color), "ControlText")
        ]
        public Color CategoryForeColor {
            get {
                return categoryForeColor;
            }
            set {
                if (categoryForeColor != value) {
                    categoryForeColor = value;
                    gridView.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsBackColor"]/*' />
        /// <devdoc>
        /// The background color for the hot commands region.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCommandsBackColorDesc))
        ]
        public Color CommandsBackColor {
            get {
                return hotcommands.BackColor;
            }
            set {
                hotcommands.BackColor = value;
                hotcommands.Label.BackColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsForeColor"]/*' />
        /// <devdoc>
        /// The forground color for the hot commands region.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCommandsForeColorDesc))
        ]
        public Color CommandsForeColor {
            get {
                return hotcommands.ForeColor;
            }
            set {
                hotcommands.ForeColor = value;
                hotcommands.Label.ForeColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsEnabledLinkColor"]/*' />
        /// <devdoc>
        /// The link color for the hot commands region.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCommandsLinkColorDesc))
        ]
        public Color CommandsLinkColor {
            get {
                return hotcommands.Label.LinkColor;
            }
            set {
                hotcommands.Label.LinkColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsActiveLinkColor"]/*' />
        /// <devdoc>
        /// The active link color for the hot commands region.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCommandsActiveLinkColorDesc))
        ]
        public Color CommandsActiveLinkColor {
            get {
                return hotcommands.Label.ActiveLinkColor;
            }
            set {
                hotcommands.Label.ActiveLinkColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsDisabledLinkColor"]/*' />
        /// <devdoc>
        /// The color for the hot commands region when the link is disabled.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCommandsDisabledLinkColorDesc))
        ]
        public Color CommandsDisabledLinkColor {
            get {
                return hotcommands.Label.DisabledLinkColor;
            }
            set {
                hotcommands.Label.DisabledLinkColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsBorderColor"]/*' />
        /// <devdoc>
        ///    <para>The border color for the hot commands region</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCommandsBorderColorDesc)),
        DefaultValue(typeof(Color), "ControlDark")
        ]
        public Color CommandsBorderColor {
            get {
                return hotcommands.BorderColor;
            }
            set {
                hotcommands.BorderColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsVisible"]/*' />
        /// <devdoc>
        /// Returns true if the commands pane is currently shown.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool CommandsVisible {
            get {
                return hotcommands.Visible;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CommandsVisibleIfAvailable"]/*' />
        /// <devdoc>
        /// Returns true if the commands pane will be shown for objects
        /// that expose verbs.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(true),
        SRDescription(nameof(SR.PropertyGridCommandsVisibleIfAvailable))
        ]
        public virtual bool CommandsVisibleIfAvailable {
            get {
                return hotcommands.AllowVisible;
            }
            set {
                bool hotcommandsVisible = hotcommands.Visible;
                hotcommands.AllowVisible = value;
                //PerformLayout();
                if (hotcommandsVisible != hotcommands.Visible) {
                    OnLayoutInternal(false);
                    hotcommands.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ContextMenuDefaultLocation"]/*' />
        /// <devdoc>
        /// Returns a default location for showing the context menu.  This
        /// location is the center of the active property label in the grid, and
        /// is used useful to position the context menu when the menu is invoked
        /// via the keyboard.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Point ContextMenuDefaultLocation {
            get {
                return GetPropertyGridView().ContextMenuDefaultLocation;
            }
        }
        
         /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.Controls"]/*' />
         /// <devdoc>
        ///     Collection of child controls.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ControlCollection Controls {
            get {
                return base.Controls;
            }
        }

        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return new Size(130, 130);
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.DefaultTabType"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        protected virtual Type DefaultTabType {
            get {
               return typeof(PropertiesTab);
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.DrawFlatToolbar"]/*' />
        ///<internalonly/>
        protected bool DrawFlatToolbar {
            get {
                return drawFlatToolBar;
            }
            set {
                if (drawFlatToolBar != value) {
                    drawFlatToolBar = value;
                    SetToolStripRenderer();
                }

                SetHotCommandColors(value && !AccessibilityImprovements.Level2);
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ForeColor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ForeColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                base.ForeColorChanged += value;
            }
            remove {
                base.ForeColorChanged -= value;
            }
        }

        private bool FreezePainting {
            get {
               return paintFrozen > 0;
            }
            set {
               
               if (value && IsHandleCreated && this.Visible) {
                  if (0 == paintFrozen++) {
                     SendMessage(NativeMethods.WM_SETREDRAW, 0, 0);
                  }
               }
               if (!value) {
                  if (paintFrozen == 0) {
                     return;
                  }
               
                  if (0 == --paintFrozen) {
                     SendMessage(NativeMethods.WM_SETREDRAW, 1, 0);
                     Invalidate(true);
                  }
                  
               }
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.HelpBackColor"]/*' />
        /// <devdoc>
        /// The background color for the help region.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridHelpBackColorDesc)),
        DefaultValue(typeof(Color), "Control")
        ]
        public Color HelpBackColor {
            get {
                return doccomment.BackColor;
            }
            set {
                doccomment.BackColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.HelpForeColor"]/*' />
        /// <devdoc>
        /// The forground color for the help region.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridHelpForeColorDesc)),
        DefaultValue(typeof(Color), "ControlText")
        ]
        public Color HelpForeColor {
            get {
                return doccomment.ForeColor;
            }
            set {
                doccomment.ForeColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.HelpBorderColor"]/*' />
        /// <devdoc>
        ///    <para>The border color for the help region</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridHelpBorderColorDesc)),
        DefaultValue(typeof(Color), "ControlDark")
        ]
        public Color HelpBorderColor {
            get {
                return doccomment.BorderColor;
            }
            set {
                doccomment.BorderColor = value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.HelpVisible"]/*' />
        /// <devdoc>
        /// Sets or gets the visiblity state of the help pane.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.PropertyGridHelpVisibleDesc))
        ]
        public virtual bool HelpVisible {
            get {
                return this.helpVisible;
            }
            set {
                this.helpVisible = value;

                doccomment.Visible = value;
                OnLayoutInternal(false);
                Invalidate();
                doccomment.Invalidate();
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedItemWithFocusBackColor"]/*' />
        /// <devdoc>
        ///    <para>Background color for Highlighted text.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridSelectedItemWithFocusBackColorDesc)),
        DefaultValue(typeof(Color), "Highlight")
        ]
        public Color SelectedItemWithFocusBackColor {
            get {
                return selectedItemWithFocusBackColor;
            }
            set {
                if (selectedItemWithFocusBackColor != value)
                {
                    selectedItemWithFocusBackColor = value;
                    gridView.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedItemWithFocusForeColor"]/*' />
        /// <devdoc>
        ///    <para>Foreground color for Highlighted (selected) text.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridSelectedItemWithFocusForeColorDesc)),
        DefaultValue(typeof(Color), "HighlightText")
        ]
        public Color SelectedItemWithFocusForeColor {
            get {
                return selectedItemWithFocusForeColor;
            }
            set {
                if (selectedItemWithFocusForeColor != value)
                {
                    selectedItemWithFocusForeColor = value;
                    gridView.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.DisabledItemForeColor"]/*' />
        /// <devdoc>
        ///    <para>Foreground color for disabled text in the Grid View</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridDisabledItemForeColorDesc)),
        DefaultValue(typeof(Color), "GrayText")
        ]
        public Color DisabledItemForeColor {
            get {
                return gridView.GrayTextColor;
            }
            set {
                gridView.GrayTextColor = value;
                gridView.Invalidate();
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CategorySplitterColor"]/*' />
        /// <devdoc>
        ///    <para>Color for the horizontal splitter line separating property categories.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCategorySplitterColorDesc)),
        DefaultValue(typeof(Color), "Control")
        ]
        public Color CategorySplitterColor {
            get {
                return categorySplitterColor;
            }
            set {
                if (categorySplitterColor != value) {
                    categorySplitterColor = value;
                    gridView.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CanShowVisualStyleGlyphs"]/*' />
        /// <devdoc>
        ///    <para>Enable/Disable use of VisualStyle glyph for PropertyGrid node expansion.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridCanShowVisualStyleGlyphsDesc)),
        DefaultValue(true)
        ]
        public bool CanShowVisualStyleGlyphs {
            get {
                return canShowVisualStyleGlyphs;
            }
            set {
                if (canShowVisualStyleGlyphs != value) {
                    canShowVisualStyleGlyphs = value;
                    gridView.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.InPropertySet"]/*' />
        /// <internalonly/>
        bool IComPropertyBrowser.InPropertySet {
            get {
                return GetPropertyGridView().GetInPropertySet();
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.LineColor"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridLineColorDesc)),
        DefaultValue(typeof(Color), "InactiveBorder")
        ]
        public Color LineColor {
            get {
                return lineColor;
            }
            set {
                if (lineColor != value) {
                    lineColor = value;
                    developerOverride = true;
                    if (lineBrush != null) {
                        lineBrush.Dispose();
                        lineBrush = null;
                    }
                    gridView.Invalidate();
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged {
            add { base.PaddingChanged += value; }
            remove { base.PaddingChanged -= value; }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertySort"]/*' />
        /// <devdoc>
        /// Sets or gets the current property sort type, which can be
        /// PropertySort.Categorized or PropertySort.Alphabetical.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(PropertySort.CategorizedAlphabetical),
        SRDescription(nameof(SR.PropertyGridPropertySortDesc))
        ]
        public PropertySort PropertySort {
            get {
                return propertySortValue;
            }
            set {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)PropertySort.NoSort, (int)PropertySort.CategorizedAlphabetical)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PropertySort));
                }
                ToolStripButton newButton;
                
                if ((value & PropertySort.Categorized) != 0) {
                    newButton = viewSortButtons[CATEGORIES];
                }
                else if ((value & PropertySort.Alphabetical) != 0) {
                    newButton = viewSortButtons[ALPHA];
                }
                else {
                    newButton = viewSortButtons[NO_SORT];
                }
                
                GridItem selectedGridItem = SelectedGridItem;
               
               
                OnViewSortButtonClick(newButton, EventArgs.Empty);
            
                this.propertySortValue = value;
                
                if (selectedGridItem != null) {
                    try {
                        SelectedGridItem = selectedGridItem;
                    }
                    catch (System.ArgumentException) {
                        // When no row is selected, SelectedGridItem returns grid entry for root
                        // object. But this is not a selectable item. So don't worry if setting SelectedGridItem
                        // cause an argument exception whe ntrying to re-select the root object. Just leave the
                        // the grid with no selected row.
                    }
                }
                
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabs"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public PropertyTabCollection PropertyTabs {
            get {
                return new PropertyTabCollection(this);
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedObject"]/*' />
        /// <devdoc>
        /// Sets a single Object into the grid to be browsed.  If multiple
        /// objects are being browsed, this property will return the first
        /// one in the list.  If no objects are selected, null is returned.
        /// </devdoc>
        [
        DefaultValue(null),
        SRDescription(nameof(SR.PropertyGridSelectedObjectDesc)),
        SRCategory(nameof(SR.CatBehavior)),
        TypeConverter(typeof(SelectedObjectConverter))
        ]
        public Object SelectedObject {
            get {
                if (currentObjects == null || currentObjects.Length == 0) {
                    return null;
                }
                return currentObjects[0];
            }
            set {
                if (value == null) {
                    SelectedObjects = new object[0];
                }
                else {
                    SelectedObjects = new Object[]{value};
                }
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedObjects"]/*' />
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object[] SelectedObjects {
            set {   
                try {
                    this.FreezePainting = true;

                    SetFlag(FullRefreshAfterBatch, false);
                    if (GetFlag(BatchMode)) {
                       SetFlag(BatchModeChange, false);
                    }
                    
                    gridView.EnsurePendingChangesCommitted();
                                        
                    bool isSame = false;
                    bool classesSame = false;
                    bool showEvents = true;
                    
                    // validate the array coming in
                    if (value != null && value.Length > 0) {
                       for (int count = 0; count < value.Length; count++) {
                           if (value[count] == null) {
                               throw new ArgumentException(string.Format(SR.PropertyGridSetNull, count.ToString(CultureInfo.CurrentCulture), value.Length.ToString(CultureInfo.CurrentCulture)));
                           }
                           else if (value[count] is IUnimplemented) {
                               throw new NotSupportedException(string.Format(SR.PropertyGridRemotedObject, value[count].GetType().FullName));
                           }
                       }
                    }
                    else {
                        showEvents = false;
                    }
   
                    // make sure we actually changed something before we inspect tabs
                    if (currentObjects != null && value != null &&
                        currentObjects.Length == value.Length) {
                        isSame = true;
                        classesSame = true;
                        for (int i = 0; i < value.Length && (isSame || classesSame); i++) {
                            if (isSame && currentObjects[i] != value[i]) {
                                isSame = false;
                            }
       
                            Type oldType = GetUnwrappedObject(i).GetType();
       
                            Object objTemp = value[i];
       
                            if (objTemp is ICustomTypeDescriptor) {
                                objTemp = ((ICustomTypeDescriptor)objTemp).GetPropertyOwner(null);
                            }
                            Type newType = objTemp.GetType();        
       
                            // check if the types are the same.  If they are, and they 
                            // are COM objects, check their GUID's.  If they are different
                            // or Guid.Emtpy, assume the classes are different.
                            //
                            if (classesSame && 
                                (oldType != newType || oldType.IsCOMObject && newType.IsCOMObject)) {
                                classesSame = false;
                            }
                        }
                    }
       
                    if (!isSame) {

                        EnsureDesignerEventService();

                        showEvents = showEvents && GetFlag(GotDesignerEventService);
                   
                        SetStatusBox("", "");
                       
                        ClearCachedProps();

                        // The default selected entry might still reference the previous selected 
                        // objects. Set it to null to avoid leaks.
                        peDefault = null;

                        if (value == null) {
                            currentObjects = new Object[0];
                        }
                        else {
                            currentObjects = (object[])value.Clone();
                        }
                       
                        SinkPropertyNotifyEvents();
                        SetFlag(PropertiesChanged, true);


                        // Since we are changing the selection, we need to make sure that the
                        // keywords for the currently selected grid entry gets removed
                        if (gridView != null) {
                            // TypeResolutionService is needed to access the HelpKeyword. However,
                            // TypeResolutionService might be disposed when project is closing. We
                            // need swallow the exception in this case.
                            try {
                                gridView.RemoveSelectedEntryHelpAttributes();
                            }
                            catch (COMException) {}
                        }
                                               
                        if (peMain != null) {
                            peMain.Dispose();
                        }
   
                        // throw away any extra component only tabs
                        if (!classesSame && !GetFlag(TabsChanging) && selectedViewTab < viewTabButtons.Length) {
    
                            Type tabType = selectedViewTab == -1 ? null : viewTabs[selectedViewTab].GetType();
                            ToolStripButton viewTabButton = null;
                            RefreshTabs(PropertyTabScope.Component);
                            EnableTabs();
                            if (tabType != null) {
                                for (int i = 0; i < viewTabs.Length;i++) {
                                    if (viewTabs[i].GetType() == tabType && viewTabButtons[i].Visible) {
                                        viewTabButton = viewTabButtons[i];
                                        break;
                                    }
                                }
                            }
                            SelectViewTabButtonDefault(viewTabButton);
                        }
                   
                        // make sure we've also got events on all the objects
                        if (showEvents && viewTabs != null && viewTabs.Length > EVENTS && (viewTabs[EVENTS] is EventsTab)) {
                            showEvents = viewTabButtons[EVENTS].Visible;
                            Object tempObj;
                            PropertyDescriptorCollection events;
                            Attribute[] attrs = new Attribute[BrowsableAttributes.Count];
                            BrowsableAttributes.CopyTo(attrs, 0);
                            
                            Hashtable eventTypes = null;
                            
                            if (currentObjects.Length > 10) {
                               eventTypes = new Hashtable();
                            }
                            
                            for (int i = 0; i < currentObjects.Length && showEvents; i++) {
                               tempObj = currentObjects[i];
                               
                               if (tempObj is ICustomTypeDescriptor) {
                                   tempObj = ((ICustomTypeDescriptor)tempObj).GetPropertyOwner(null);
                               }
                            
                               Type objType = tempObj.GetType();
                            
                               if (eventTypes != null && eventTypes.Contains(objType)) {
                                   continue;
                               }
                               
                               // make sure these things are sited components as well
                               showEvents = showEvents && (tempObj is IComponent && ((IComponent)tempObj).Site != null);
                            
                               // make sure we've also got events on all the objects
                               events =  ((EventsTab)viewTabs[EVENTS]).GetProperties(tempObj, attrs);
                               showEvents = showEvents && events != null && events.Count > 0;
                            
                               if (showEvents && eventTypes != null) {
                                   eventTypes[objType] = objType;
                               }
                            }
                        }
                        ShowEventsButton(showEvents && currentObjects.Length > 0);
                        DisplayHotCommands();
   
                        if (currentObjects.Length == 1) {
                            EnablePropPageButton(currentObjects[0]);
                        }
                        else {
                            EnablePropPageButton(null);
                        }
                        OnSelectedObjectsChanged(EventArgs.Empty);
                    }
   
   
                    /*
       
                    Microsoft, hopefully this won't be a big perf problem, but it looks like we
                           need to refresh even if we didn't change the selected objects.
       
                    if (propertiesChanged) {*/
                    if (!GetFlag(TabsChanging)) {

                        // ReInitTab means that we should set the tab back to what is used to be for a given designer.
                        // Basically, if you select an events tab for your designer and double click to go to code, it should
                        // be the events tab when you get back to the designer.
                        //
                        // so we set that bit when designers get switched, and makes sure we select and refresh that tab
                        // when we load.
                        //
                        if (currentObjects.Length > 0 && GetFlag(ReInitTab)) {
                            object designerKey = ActiveDesigner;

                            // get the active designer, see if we've stashed away state for it.
                            //
                            if (designerKey != null && designerSelections != null && designerSelections.ContainsKey(designerKey.GetHashCode())) {
                                int nButton = (int)designerSelections[designerKey.GetHashCode()];

                                // yep, we know this one.  Make sure it's selected.
                                //
                                if (nButton < viewTabs.Length && (nButton == PROPERTIES || viewTabButtons[nButton].Visible)) {
                                    SelectViewTabButton(viewTabButtons[nButton], true);
                                }
                            }
                            else {
                                Refresh(false);
                            }
                            SetFlag(ReInitTab, false);
                        }
                        else {
                            Refresh(true);
                        }
    
                        if (currentObjects.Length > 0) {
                            SaveTabSelection();
                        }
                    }
                   /*}else {
                       Invalidate();
                       gridView.Invalidate();
                   //}*/
                }
                finally {
                   this.FreezePainting = false;
                }
            }

            get 
            {
                if (currentObjects == null) {
                    return new object[0];
                }
                return (object[])currentObjects.Clone();
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedTab"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public PropertyTab SelectedTab {
            get {
                Debug.Assert(selectedViewTab < viewTabs.Length && selectedViewTab >= 0, "Invalid tab selection!");
                return viewTabs[selectedViewTab];
            }
        }



        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedGridItem"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public GridItem SelectedGridItem {
            get {
                GridItem g = gridView.SelectedGridEntry;
                if (g == null) {
                    return this.peMain;
                }
                return g;
            }
            set {
                gridView.SelectedGridEntry = (GridEntry)value;
            }
        }
       
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ShowFocusCues"]/*' />
        ///<internalonly/>        
        protected internal override bool ShowFocusCues {
            get {
                return true;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.Site"]/*' />
        public override ISite Site {
            get {
                return base.Site;
            }
            set {
               
                // Perf - the base class is possibly going to change the font via ambient properties service
                SuspendAllLayout(this);
               
                base.Site = value;
                gridView.ServiceProvider = value;

                if (value == null) {
                    this.ActiveDesigner = null;
                }
                else {
                    this.ActiveDesigner = (IDesignerHost)value.GetService(typeof(IDesignerHost));
                }
                
                ResumeAllLayout(this,true);

            }
        }

        [Browsable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        [Browsable(false)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }


        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.LargeButtons"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridLargeButtonsDesc)),
        DefaultValue(false)
        ]
        public bool LargeButtons {
            get {
                return buttonType == LARGE_BUTTONS;
            }
            set {
                if (value == (buttonType == LARGE_BUTTONS)) {
                    return;
                }

                this.buttonType = (value ?  LARGE_BUTTONS : NORMAL_BUTTONS);
                if (value) {
                    EnsureLargeButtons();
                    if (this.imageList != null && this.imageList[LARGE_BUTTONS] != null) {
                        toolStrip.ImageScalingSize = this.imageList[LARGE_BUTTONS].ImageSize;
                    }
                }
                else {
                    if (this.imageList != null && this.imageList[NORMAL_BUTTONS] != null) {
                        toolStrip.ImageScalingSize = this.imageList[NORMAL_BUTTONS].ImageSize;
                    }
                }
                
                toolStrip.ImageList = imageList[this.buttonType];
                OnLayoutInternal(false);
                Invalidate();
                toolStrip.Invalidate();
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ToolbarVisible"]/*' />
        /// <devdoc>
        /// Sets or gets the visiblity state of the toolStrip.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(true),
        SRDescription(nameof(SR.PropertyGridToolbarVisibleDesc))
        ]
        public virtual bool ToolbarVisible {
            get {
                return this.toolbarVisible;
            }
            set {
                this.toolbarVisible = value;

                toolStrip.Visible = value;
                OnLayoutInternal(false);
                if (value) {
                    SetupToolbar(this.viewTabsDirty);
                }
                Invalidate();
                toolStrip.Invalidate();
            }
        }

        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected ToolStripRenderer ToolStripRenderer {
            get {
                if (toolStrip != null) {
                    return toolStrip.Renderer;
                }
                return null;     
            }
            set {
                if (toolStrip != null) {
                   toolStrip.Renderer = value;               
                }
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ViewBackColor"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridViewBackColorDesc)),
        DefaultValue(typeof(Color), "Window")
        ]
        public Color ViewBackColor {
            get {
                return gridView.BackColor;
            }
            set {
                gridView.BackColor = value;
                gridView.Invalidate();
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ViewForeColor"]/*' />
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridViewForeColorDesc)),
        DefaultValue(typeof(Color), "WindowText")
        ]
        public Color ViewForeColor {
            get {
                return gridView.ForeColor;
            }
            set {
                gridView.ForeColor = value;
                gridView.Invalidate();
            
            }
        }
  
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ViewBorderColor"]/*' />
        /// <devdoc>
        ///    <para>Border color for the property grid view.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.PropertyGridViewBorderColorDesc)),
        DefaultValue(typeof(Color), "ControlDark")
        ]
        public Color ViewBorderColor {
            get {
                return viewBorderColor;
            }
            set {
                if (viewBorderColor != value) {
                    viewBorderColor = value;
                    gridView.Invalidate();
                }
            }
        }

        private int AddImage(Bitmap image) {
            
            image.MakeTransparent();
            // Resize bitmap only if resizing is needed in order to avoid image distortion.
            if (DpiHelper.IsScalingRequired && (image.Size.Width != normalButtonSize.Width || image.Size.Height != normalButtonSize.Height)) {
                image = DpiHelper.CreateResizedBitmap(image, normalButtonSize);
            }
            int result = imageList[NORMAL_BUTTONS].Images.Count;
            imageList[NORMAL_BUTTONS].Images.Add(image);
            return result;
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.KeyDown"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyEventHandler KeyDown {
            add {
                base.KeyDown += value;
            }
            remove {
                base.KeyDown -= value;
            }
        }


        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.KeyPress"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyPressEventHandler KeyPress {
            add {
                base.KeyPress += value;
            }
            remove {
                base.KeyPress -= value;
            }
        }


        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.KeyUp"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyEventHandler KeyUp {
            add {
                base.KeyUp += value;
            }
            remove {
                base.KeyUp -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.MouseDown"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDown {
            add {
                base.MouseDown += value;
            }
            remove {
                base.MouseDown -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.MouseUp"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseUp {
            add {
                base.MouseUp += value;
            }
            remove {
                base.MouseUp -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.MouseMove"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseMove {
            add {
                base.MouseMove += value;
            }
            remove {
                base.MouseMove -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.MouseEnter"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseEnter {
            add {
                base.MouseEnter += value;
            }
            remove {
                base.MouseEnter -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.MouseLeave"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseLeave {
            add {
                base.MouseLeave += value;
            }
            remove {
                base.MouseLeave -= value;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyValueChanged"]/*' />
        /// <devdoc> Event that is fired when a property value is modified.</devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.PropertyGridPropertyValueChangedDescr))]
        public event PropertyValueChangedEventHandler PropertyValueChanged {
            add {
                Events.AddHandler(EventPropertyValueChanged, value);
            }
            remove {
                Events.RemoveHandler(EventPropertyValueChanged, value);
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.ComComponentNameChanged"]/*' />
        ///<internalonly/>        
        event ComponentRenameEventHandler IComPropertyBrowser.ComComponentNameChanged {
            add {
                Events.AddHandler(EventComComponentNameChanged, value);
            }
            remove {
                Events.RemoveHandler(EventComComponentNameChanged, value);
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabChanged"]/*' />
        /// <devdoc> Event that is fired when the current view tab is changed, such as changing from Properties to Events</devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.PropertyGridPropertyTabchangedDescr))]
        public event PropertyTabChangedEventHandler PropertyTabChanged {
            add {
                Events.AddHandler(EventPropertyTabChanged, value);
            }
            remove {
                Events.RemoveHandler(EventPropertyTabChanged, value);
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertySortChanged"]/*' />
        /// <devdoc> Event that is fired when the sort mode is changed.</devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.PropertyGridPropertySortChangedDescr))]
        public event EventHandler PropertySortChanged {
            add {
                Events.AddHandler(EventPropertySortChanged, value);
            }
            remove {
                Events.RemoveHandler(EventPropertySortChanged, value);
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelectedGridItemChanged"]/*' />
        /// <devdoc> Event that is fired when the selected GridItem is changed</devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.PropertyGridSelectedGridItemChangedDescr))]
        public event SelectedGridItemChangedEventHandler SelectedGridItemChanged {
            add {
                Events.AddHandler(EventSelectedGridItemChanged, value);
            }
            remove {
                Events.RemoveHandler(EventSelectedGridItemChanged, value);
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SelecteObjectsChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.PropertyGridSelectedObjectsChangedDescr))]
        public event EventHandler SelectedObjectsChanged {
            add {
                Events.AddHandler(EventSelectedObjectsChanged, value);
            }
            remove {
                Events.RemoveHandler(EventSelectedObjectsChanged, value);
            }
        }
        
        
        internal void AddTab(Type tabType, PropertyTabScope scope) {
            AddRefTab(tabType, null, scope, true);
        }

 
        internal void AddRefTab(Type tabType, Object component, PropertyTabScope type, bool setupToolbar) {
            PropertyTab tab = null;
            int tabIndex = -1;

            if (viewTabs != null) {
                // check to see if we've already got a tab of this type
                for (int i = 0; i < viewTabs.Length; i++) {
                    Debug.Assert(viewTabs[i] != null, "Null item in tab array!");
                    if (tabType == viewTabs[i].GetType()) {
                        tab = viewTabs[i];
                        tabIndex = i;
                        break;
                    }
                }
            }
            else {
                tabIndex = 0;
            }

            if (tab == null) {
                // the tabs need service providers. The one we hold onto is not good enough,
                // so try to get the one off of the component's site.
                IDesignerHost host = null;
                if (component != null && component is IComponent && ((IComponent) component).Site != null)
                    host = (IDesignerHost) ((IComponent) component).Site.GetService(typeof(IDesignerHost));

                try
                {
                    tab = CreateTab(tabType, host);
                }
                catch (Exception e)
                {
                    Debug.Fail("Bad Tab.  We're not going to show it. ", e.ToString());
                    return;
                }

                // add it at the end of the array
                if (viewTabs != null) {
                    tabIndex = viewTabs.Length;

                    // find the insertion position...special case for event's and properties
                    if (tabType == DefaultTabType) {
                        tabIndex = PROPERTIES;
                    }
                    else if (typeof(EventsTab).IsAssignableFrom(tabType)) {
                        tabIndex = EVENTS;
                    }
                    else {
                        // order tabs alphabetically, we've always got a property tab, so
                        // start after that
                        for (int i = 1; i < viewTabs.Length; i++) {

                            // skip the event tab
                            if (viewTabs[i] is EventsTab) {
                                continue;
                            }

                            if (String.Compare(tab.TabName, viewTabs[i].TabName, false, CultureInfo.InvariantCulture) < 0) {
                                tabIndex = i;
                                break;
                            }
                        }
                    }
                }

                // now add the tab to the tabs array
                PropertyTab[] newTabs = new PropertyTab[viewTabs.Length + 1];
                Array.Copy(viewTabs, 0, newTabs, 0, tabIndex);
                Array.Copy(viewTabs, tabIndex, newTabs, tabIndex + 1, viewTabs.Length - tabIndex);
                newTabs[tabIndex] = tab;
                viewTabs = newTabs;

                viewTabsDirty = true;

                PropertyTabScope[] newTabScopes = new PropertyTabScope[viewTabScopes.Length + 1];
                Array.Copy(viewTabScopes, 0, newTabScopes, 0, tabIndex);
                Array.Copy(viewTabScopes, tabIndex, newTabScopes, tabIndex + 1, viewTabScopes.Length - tabIndex);
                newTabScopes[tabIndex] = type;
                viewTabScopes = newTabScopes;

                Debug.Assert(viewTabs != null, "Tab array destroyed!");
            }

            if (tab != null && component != null) {
                try {
                    Object[] tabComps = tab.Components;
                    int oldArraySize = tabComps == null ? 0 : tabComps.Length;

                    Object[] newComps = new Object[oldArraySize + 1];
                    if (oldArraySize > 0) {
                        Array.Copy(tabComps, newComps, oldArraySize);
                    }
                    newComps[oldArraySize] = component;
                    tab.Components = newComps;
                }
                catch (Exception e) {
                    Debug.Fail("Bad tab. We're going to remove it.", e.ToString());
                    RemoveTab(tabIndex, false);
                }
            }

            if (setupToolbar) {
                SetupToolbar();
                ShowEventsButton(false);
            }
        }        
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CollapseAllGridItems"]/*' />
        /// <devdoc> Collapses all the nodes in the PropertyGrid</devdoc>
        public void CollapseAllGridItems() {
            gridView.RecursivelyExpand(peMain, false, false, -1);
        }
          
        private void ClearCachedProps() {
            if (viewTabProps != null) {
               viewTabProps.Clear();                       
            }
        }  
        
        internal void ClearValueCaches() {
            if (peMain != null) {
               peMain.ClearCachedValues();
            }
        }


        /// <devdoc>
        /// Clears the tabs of the given scope or smaller.
        /// tabScope must be PropertyTabScope.Component or PropertyTabScope.Document.
        /// </devdoc>
        internal void ClearTabs(PropertyTabScope tabScope) {
            if (tabScope < PropertyTabScope.Document) {
                throw new ArgumentException(SR.PropertyGridTabScope);
            }
            RemoveTabs(tabScope, true);
        }

        #if DEBUG
            internal bool inGridViewCreate = false;
        #endif

        private /*protected virtual*/ PropertyGridView CreateGridView(IServiceProvider sp) {
#if DEBUG
            try {
                    inGridViewCreate = true;
#endif
            return new PropertyGridView(sp, this);
#if DEBUG
            }
            finally {
                    inGridViewCreate = false;
            }   
#endif
        }

        private ToolStripSeparator CreateSeparatorButton() {
            ToolStripSeparator button = new ToolStripSeparator();
            return button;
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.CreatePropertyTab"]/*' />
        protected virtual PropertyTab CreatePropertyTab(Type tabType) {
            return null; 
        }

        private PropertyTab CreateTab(Type tabType, IDesignerHost host) {
            PropertyTab tab = CreatePropertyTab(tabType);

            if (tab == null) {
                ConstructorInfo constructor = tabType.GetConstructor(new Type[] {typeof(IServiceProvider)});
                Object param = null;
                if (constructor == null) {
    
                    // try a IDesignerHost ctor
                    constructor = tabType.GetConstructor(new Type[] {typeof(IDesignerHost)});
    
                    if (constructor != null) {
                        param = host;
                    }
                }
                else {
                    param = this.Site;
                }
    
    
                if (param != null && constructor != null) {
                    tab = (PropertyTab) constructor.Invoke(new Object[] {param});
                }
                else {
                    // just call the default ctor
                    // 
                    tab = (PropertyTab)Activator.CreateInstance(tabType);
                }
            }

            Debug.Assert(tab != null, "Failed to create tab!");

            if (tab != null) {
                // ensure it's a valid tab
                Bitmap bitmap = tab.Bitmap;
                
                if (bitmap == null)
                    throw new ArgumentException(string.Format(SR.PropertyGridNoBitmap, tab.GetType().FullName));

                Size size = bitmap.Size;
                if (size.Width != 16 || size.Height != 16) {
                    // resize it to 16x16 if it isn't already.
                    //
                    bitmap = new Bitmap(bitmap, new Size(16,16));
                }

                string name = tab.TabName;
                if (name == null || name.Length == 0)
                    throw new ArgumentException(string.Format(SR.PropertyGridTabName, tab.GetType().FullName));

                // we're good to go!
            }
            return tab;
        }

        /*
        private ToolStripButton CreateToggleButton(string toolTipText, int imageIndex, EventHandler eventHandler) {
            ToolStripButton button = new ToolStripButton();
            button.Text = toolTipText;
            button.AutoToolTip = true;
            button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            button.ImageIndex = imageIndex;
            button.Click += eventHandler;
            button.CheckOnClick = true;
            button.ImageScaling = ToolStripItemImageScaling.None;
            return button;
        }
        */

        private ToolStripButton CreatePushButton(string toolTipText, int imageIndex, EventHandler eventHandler, bool useCheckButtonRole = false) {
            ToolStripButton button = new ToolStripButton();
            button.Text = toolTipText;
            button.AutoToolTip = true;
            button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            button.ImageIndex = imageIndex;
            button.Click += eventHandler;
            button.ImageScaling = ToolStripItemImageScaling.SizeToFit;

            if (AccessibilityImprovements.Level1) {
                if (useCheckButtonRole) {
                    button.AccessibleRole = AccessibleRole.CheckButton;
                }
            }

            return button;
        }
        
        ///<internalonly/>        
        internal void DumpPropsToConsole() {
            gridView.DumpPropsToConsole(peMain, "");
        }

        private void DisplayHotCommands() {
            bool hotCommandsDisplayed = hotcommands.Visible;

            IComponent component = null;
            DesignerVerb[] verbs = null;

            // We favor the menu command service, since it can give us
            // verbs.  If we fail that, we will go straight to the
            // designer.
            //
            if (currentObjects != null && currentObjects.Length > 0) {
                for (int i = 0; i < currentObjects.Length; i++) {
                    object obj = GetUnwrappedObject(i);
                    if (obj is IComponent) {
                        component = (IComponent)obj;
                        break;
                    }
                }

                if (component != null) {
                    ISite site = component.Site;

                    if (site != null) {

                        IMenuCommandService mcs = (IMenuCommandService)site.GetService(typeof(IMenuCommandService));
                        if (mcs != null) {

                            // Got the menu command service.  Let it deal with the set of verbs for
                            // this component.
                            //
                            verbs = new DesignerVerb[mcs.Verbs.Count];
                            mcs.Verbs.CopyTo(verbs, 0);
                        }
                        else {

                            // No menu command service.  Go straight to the component's designer.  We
                            // can only do this if the Object count is 1, because desginers do not
                            // support verbs across a multi-selection.
                            //
                            if (currentObjects.Length == 1 && GetUnwrappedObject(0) is IComponent) {

                                IDesignerHost designerHost = (IDesignerHost) site.GetService(typeof(IDesignerHost));
                                if (designerHost != null) {
                                    IDesigner designer = designerHost.GetDesigner(component);
                                    if (designer != null) {
                                        verbs = new DesignerVerb[designer.Verbs.Count];
                                        designer.Verbs.CopyTo(verbs, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // don't show verbs if a prop grid is on the form at design time.            
            if (!DesignMode) {
            

                if (verbs != null && verbs.Length > 0) {
                    hotcommands.SetVerbs(component, verbs);
                }
                else {
                    hotcommands.SetVerbs(null, null);
                }
    
                if (hotCommandsDisplayed != hotcommands.Visible) {
                    OnLayoutInternal(false);
                }
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.Dispose"]/*' />
        protected override void Dispose(bool disposing) {

            if (disposing) {
                // Unhook IDesignerEventService.ActiveDesignerChanged event
                //
                if (GetFlag(GotDesignerEventService)) {                
                    Debug.Assert(designerEventService != null, "GetFlag(GotDesignerEventService) inconsistent with designerEventService == null");
                    if (designerEventService != null) {
                        designerEventService.ActiveDesignerChanged -= new ActiveDesignerEventHandler(this.OnActiveDesignerChanged);
                    }                
                    designerEventService = null;
                    SetFlag(GotDesignerEventService, false);
                }
                this.ActiveDesigner = null;

                if (viewTabs != null) {
                    for (int i = 0; i < viewTabs.Length; i++) {
                        viewTabs[i].Dispose();
                    }
                    viewTabs = null;
                }

                if (imageList != null) {
                    for (int i = 0; i < imageList.Length; i++) {
                        if(imageList[i] != null) {
                            imageList[i].Dispose();
                        }
                    }
                    imageList = null;
                }

                if (bmpAlpha != null) {
                    bmpAlpha.Dispose();
                    bmpAlpha = null;
                }
                
                if (bmpCategory != null) {
                    bmpCategory.Dispose();
                    bmpCategory = null;
                }
                
                if (bmpPropPage != null) {
                    bmpPropPage.Dispose();
                    bmpPropPage = null;
                }
           
                if (lineBrush != null) {
                    lineBrush.Dispose();
                    lineBrush = null;
                }

                if (peMain != null) {
                    peMain.Dispose();
                    peMain = null;
                }

                if (currentObjects != null) {
                    currentObjects = null;
                    SinkPropertyNotifyEvents();
                }

                ClearCachedProps();
                currentPropEntries = null;            
            }

            base.Dispose(disposing);
        }

        private void DividerDraw(int y) {
            if (y == -1)
                return;

            Rectangle rectangle = gridView.Bounds;
            rectangle.Y = y - cyDivider;
            rectangle.Height = cyDivider;

            DrawXorBar(this,rectangle);
        }

        private SnappableControl DividerInside(int x, int y) {

            int useGrid = -1;

            if (hotcommands.Visible) {
                Point locDoc = hotcommands.Location;
                if (y >= (locDoc.Y - cyDivider) &&
                    y <= (locDoc.Y + 1)) {
                    return hotcommands;
                }
                useGrid = 0;
            }

            if (doccomment.Visible) {
                Point locDoc = doccomment.Location;
                if (y >= (locDoc.Y - cyDivider) &&
                    y <= (locDoc.Y+1)) {
                    return doccomment;
                }

                if (useGrid == -1) {
                    useGrid = 1;
                }
            }

            // also the bottom line of the grid
            if (useGrid != -1) {
                int gridTop = gridView.Location.Y;
                int gridBottom = gridTop + gridView.Size.Height;

                if (Math.Abs(gridBottom - y) <= 1 && y > gridTop) {
                    switch (useGrid) {
                        case 0:
                            return hotcommands;
                        case 1:
                            return doccomment;
                    }
                }
            }
            return null;
        }

        private int DividerLimitHigh(SnappableControl target) {
            int high = gridView.Location.Y + MIN_GRID_HEIGHT;
            if (target == doccomment && hotcommands.Visible)
                high += hotcommands.Size.Height + 2;
            return high;
        }

        private int DividerLimitMove(SnappableControl target, int y) {
            Rectangle rectTarget = target.Bounds;

            int cyNew = y;

            // make sure we're not going to make ourselves zero height -- make 15 the min size
            cyNew = Math.Min((rectTarget.Y + rectTarget.Height - 15),cyNew);

            // make sure we're not going to make ourselves cover up the grid
            cyNew = Math.Max(DividerLimitHigh(target), cyNew);

            // just return what we got here
            return(cyNew);
        }
       
        private static void DrawXorBar(Control ctlDrawTo, Rectangle rcFrame) {
            Rectangle rc = ctlDrawTo.RectangleToScreen(rcFrame);

            if (rc.Width < rc.Height) {
                for (int i = 0; i < rc.Width; i++) {
                    ControlPaint.DrawReversibleLine(new Point(rc.X+i, rc.Y), new Point(rc.X+i, rc.Y+rc.Height), ctlDrawTo.BackColor);
                }
            }
            else {
                for (int i = 0; i < rc.Height; i++) {
                    ControlPaint.DrawReversibleLine(new Point(rc.X, rc.Y+i), new Point(rc.X+rc.Width, rc.Y+i), ctlDrawTo.BackColor);
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.DropDownDone"]/*' />
        /// <internalonly/>
        void IComPropertyBrowser.DropDownDone() {
            GetPropertyGridView().DropDownDone();
        }
        
        private bool EnablePropPageButton(Object obj) {
            if (obj == null) {
                btnViewPropertyPages.Enabled = false;
                return false;
            }

            IUIService uiSvc = (IUIService)GetService(typeof(IUIService));
            bool enable = false;

            if (uiSvc != null) {
                enable = uiSvc.CanShowComponentEditor(obj);
            }
            else {
                enable = (TypeDescriptor.GetEditor(obj, typeof(ComponentEditor)) != null);
            }

            btnViewPropertyPages.Enabled = enable;
            return enable;
        }

        // walk through the current tabs to see if they're all valid for this Object
        private void EnableTabs() {
            if (currentObjects != null) {
                // make sure our toolbars is okay
                SetupToolbar();

                Debug.Assert(viewTabs != null, "Invalid tab array");
                Debug.Assert(viewTabs.Length == viewTabScopes.Length && viewTabScopes.Length == viewTabButtons.Length,"Uh oh, tab arrays aren't all the same length! tabs=" + viewTabs.Length.ToString(CultureInfo.InvariantCulture) + ", scopes=" + viewTabScopes.Length.ToString(CultureInfo.InvariantCulture) + ", buttons=" + viewTabButtons.Length.ToString(CultureInfo.InvariantCulture));



                // skip the property tab since it's always valid
                for (int i = 1; i < viewTabs.Length; i++) {
                    Debug.Assert(viewTabs[i] != null, "Invalid tab array entry");

                    bool canExtend = true;
                    // make sure the tab is valid for all objects
                    for (int j = 0; j < currentObjects.Length; j++) {
                        try
                        {
                            if (!viewTabs[i].CanExtend(GetUnwrappedObject(j)))
                            {
                                canExtend = false;
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Fail("Bad Tab.  Disable for now.", e.ToString());
                            canExtend = false;
                            break;
                        }
                    }

                    if (canExtend != viewTabButtons[i].Visible) {
                        viewTabButtons[i].Visible = canExtend;
                        if (!canExtend && i == selectedViewTab) {
                            SelectViewTabButton(viewTabButtons[PROPERTIES], true);
                        }
                    }
                }
            }
        }

        private void EnsureDesignerEventService() {
            if (GetFlag(GotDesignerEventService)) {
                return;
            }
            designerEventService = (IDesignerEventService)GetService(typeof(IDesignerEventService));
            if (designerEventService != null) {
                SetFlag(GotDesignerEventService, true);
                designerEventService.ActiveDesignerChanged += new ActiveDesignerEventHandler(this.OnActiveDesignerChanged);
                OnActiveDesignerChanged(null, new ActiveDesignerEventArgs(null, designerEventService.ActiveDesigner));
            }
        }

        private void EnsureLargeButtons() {
            if (this.imageList[LARGE_BUTTONS] == null) {

                this.imageList[LARGE_BUTTONS] = new ImageList();
                this.imageList[LARGE_BUTTONS].ImageSize = largeButtonSize;

                if (DpiHelper.IsScalingRequired) {
                    AddLargeImage(bmpAlpha);
                    AddLargeImage(bmpCategory);

                    foreach (PropertyTab tab in viewTabs) {
                        AddLargeImage(tab.Bitmap);
                    }

                    AddLargeImage(bmpPropPage);
                }
                else {
                    ImageList.ImageCollection images = imageList[NORMAL_BUTTONS].Images;

                    for (int i = 0; i < images.Count; i++) {
                        if (images[i] is Bitmap) {
                            this.imageList[LARGE_BUTTONS].Images.Add(new Bitmap((Bitmap)images[i], largeButtonSize.Width, largeButtonSize.Height));
                        }
                    }
                }
            }
        }

        // this method should be called only inside a if (DpiHelper.IsScalingRequired) clause
        private void AddLargeImage(Bitmap originalBitmap) {
            if (originalBitmap == null) {
                return;
            }

            Bitmap largeBitmap = null;
            try {
                Bitmap transparentBitmap = new Bitmap(originalBitmap);
                transparentBitmap.MakeTransparent();
                largeBitmap = DpiHelper.CreateResizedBitmap(transparentBitmap, largeButtonSize);
                transparentBitmap.Dispose();

                this.imageList[LARGE_BUTTONS].Images.Add(largeBitmap);
            }
            catch (Exception ex) {
                Debug.Fail("Failed to add a large property grid toolstrip button, " + ex.ToString());
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.EnsurePendingChangesCommitted"]/*' />
        /// <internalonly/>
        bool IComPropertyBrowser.EnsurePendingChangesCommitted() {

            // The commits sometimes cause transactions to open
            // and close, which will cause refreshes, which we want to ignore.

            try {

                if (this.designerHost != null) {
                    designerHost.TransactionOpened -= new EventHandler(this.OnTransactionOpened);
                    designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
                }
            
                return GetPropertyGridView().EnsurePendingChangesCommitted();
            }
            finally {
                if (this.designerHost != null) {
                    designerHost.TransactionOpened += new EventHandler(this.OnTransactionOpened);
                    designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
                }
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ExpandAllGridItems"]/*' />
        public void ExpandAllGridItems() {
            gridView.RecursivelyExpand(peMain, false, true, PropertyGridView.MaxRecurseExpand);
        }

        private static Type[] GetCommonTabs(Object[] objs, PropertyTabScope tabScope) {

            if (objs == null || objs.Length == 0) {
                return new Type[0];
            }

            Type[] tabTypes = new Type[5];
            int    types = 0;
            int    i,j,k;
            PropertyTabAttribute tabAttr = (PropertyTabAttribute) TypeDescriptor.GetAttributes(objs[0])[typeof(PropertyTabAttribute)];

            if (tabAttr == null) {
                return new Type[0];
            }

            // filter out all the types of the current scope
            for (i = 0; i < tabAttr.TabScopes.Length; i++) {
                PropertyTabScope item =  tabAttr.TabScopes[i];

                if (item == tabScope) {
                    if (types == tabTypes.Length) {
                        Type[] newTabs = new Type[types * 2];
                        Array.Copy(tabTypes, 0, newTabs, 0, types);
                        tabTypes = newTabs;
                    }
                    tabTypes[types++] = tabAttr.TabClasses[i];
                }
            }

            if (types == 0) {
                return new Type[0];
            }

            bool found;

            for (i = 1; i < objs.Length && types > 0; i++) {

                // get the tab attribute
                tabAttr = (PropertyTabAttribute) TypeDescriptor.GetAttributes(objs[i])[typeof(PropertyTabAttribute)];

                if (tabAttr == null) {
                    // if this guy has no tabs at all, we can fail right now
                    return new Type[0];
                }

                // make sure this guy has all the items in the array,
                // if not, remove the items he doesn't have
                for (j = 0; j < types; j++) {
                    found = false;
                    for (k = 0; k < tabAttr.TabClasses.Length; k++) {
                        if (tabAttr.TabClasses[k] == tabTypes[j]) {
                            found = true;
                            break;
                        }
                    }

                    // if we didn't find an item, remove it from the list
                    if (!found) {
                        // swap in with the last item and decrement
                        tabTypes[j] = tabTypes[types-1];
                        tabTypes[types-1] = null;
                        types--;

                        // recheck this item since we'll be ending sooner
                        j--;
                    }
                }
            }

            Type[] returnTypes = new Type[types];
            if (types > 0) {
                Array.Copy(tabTypes, 0, returnTypes, 0, types);
            }
            return returnTypes;
        }

        internal GridEntry GetDefaultGridEntry() {
            if (peDefault == null && currentPropEntries != null) {
                peDefault = (GridEntry)currentPropEntries[0];
            }
            return peDefault;
        }

        private object GetUnwrappedObject(int index) {
            if (currentObjects == null || index < 0 || index > currentObjects.Length) {
                return null;
            }

            Object obj = currentObjects[index];
            if (obj is ICustomTypeDescriptor) {
                obj = ((ICustomTypeDescriptor)obj).GetPropertyOwner(null);
            }
            return obj;
        }

        internal GridEntryCollection GetPropEntries() {

            if (currentPropEntries == null) {
                UpdateSelection();
            }
            SetFlag(PropertiesChanged, false);
            return currentPropEntries;
        }


        private PropertyGridView GetPropertyGridView() {
            return gridView;
        }
        
 
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.HandleF4"]/*' />
        /// <internalonly/>
        void IComPropertyBrowser.HandleF4() {
            
            if (gridView.ContainsFocus) {
                return;
            }
        
            if (this.ActiveControl != gridView) {
                this.SetActiveControlInternal(gridView);
            }
            gridView.FocusInternal();
        }

        internal bool HavePropEntriesChanged() {
            return GetFlag(PropertiesChanged);
        }


        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.LoadState"]/*' />
        /// <internalonly/>
        void IComPropertyBrowser.LoadState(RegistryKey optRoot) {
            if (optRoot != null) {
                Object val = optRoot.GetValue("PbrsAlpha", "0");

                if (val != null && val.ToString().Equals("1")) {
                    this.PropertySort = PropertySort.Alphabetical;
                }
                else {
                    this.PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
                }

                val = optRoot.GetValue("PbrsShowDesc", "1");
                this.HelpVisible = (val != null && val.ToString().Equals("1"));

                val = optRoot.GetValue("PbrsShowCommands", "0");
                this.CommandsVisibleIfAvailable = (val != null && val.ToString().Equals("1"));


                val = optRoot.GetValue("PbrsDescHeightRatio", "-1");

                bool update = false;
                if (val is string) {
                    int ratio = Int32.Parse((string)val, CultureInfo.InvariantCulture);
                    if (ratio > 0) {
                        dcSizeRatio = ratio;
                        update = true;
                    }
                }

                val = optRoot.GetValue("PbrsHotCommandHeightRatio", "-1");
                if (val is string) {
                    int ratio = Int32.Parse((string)val, CultureInfo.InvariantCulture);
                    if (ratio > 0) {
                        dcSizeRatio = ratio;
                        update = true;
                    }
                }

                if (update) {
                    OnLayoutInternal(false);
                }
            }
            else {
                // apply the same defaults from above.
                //
                this.PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
                this.HelpVisible = true;
                this.CommandsVisibleIfAvailable = false;                
            }
        }

        // when the active document is changed, check all the components so see if they
        // are offering up any new tabs
        private void OnActiveDesignerChanged(Object sender, ActiveDesignerEventArgs e) {

            if (e.OldDesigner != null && e.OldDesigner == designerHost) {
                this.ActiveDesigner = null;
            }

            if (e.NewDesigner != null && e.NewDesigner != designerHost) {
                this.ActiveDesigner = e.NewDesigner;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.UnsafeNativeMethods.IPropertyNotifySink.OnChanged"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Called when a property on an Ole32 Object changes.
        /// See IPropertyNotifySink::OnChanged
        /// </devdoc>
        void UnsafeNativeMethods.IPropertyNotifySink.OnChanged(int dispID) {
            // we don't want the grid's own property sets doing this, but if we're getting
            // an OnChanged that isn't the DispID of the property we're currently changing,
            // we need to cause a refresh.
            //
            //
            bool fullRefresh = false;
            PropertyDescriptorGridEntry selectedEntry = gridView.SelectedGridEntry as PropertyDescriptorGridEntry;
            if (selectedEntry != null && selectedEntry.PropertyDescriptor != null && selectedEntry.PropertyDescriptor.Attributes != null) {

                // fish out the DispIdAttribute which will tell us the DispId of the
                // property that we're changing.
                //
                DispIdAttribute dispIdAttr = (DispIdAttribute)selectedEntry.PropertyDescriptor.Attributes[(typeof(DispIdAttribute))];
                if (dispIdAttr != null && !dispIdAttr.IsDefaultAttribute()) {
                    fullRefresh = (dispID != dispIdAttr.Value);
                }
            }

            if (!GetFlag(RefreshingProperties)) {
                if (!gridView.GetInPropertySet() || fullRefresh) {
                    Refresh(fullRefresh);
                }
    
                // this is so changes to names of native
                // objects will be reflected in the combo box
                Object obj = GetUnwrappedObject(0);
                if (ComNativeDescriptor.Instance.IsNameDispId(obj, dispID) || dispID == NativeMethods.ActiveX.DISPID_Name) {
                    OnComComponentNameChanged(new ComponentRenameEventArgs(obj, null, TypeDescriptor.GetClassName(obj)));
                }
            }
        }

        /// <devdoc>
        /// We forward messages from several of our children
        /// to our mouse move so we can put up the spliter over their borders
        /// </devdoc>
        private void OnChildMouseMove(Object sender, MouseEventArgs me) {
            Point newPt = Point.Empty;
            if (ShouldForwardChildMouseMessage((Control)sender, me, ref newPt)) {
                // forward the message
                this.OnMouseMove(new MouseEventArgs(me.Button, me.Clicks, newPt.X, newPt.Y, me.Delta));
                return;
            }
        }

        /// <devdoc>
        /// We forward messages from several of our children
        /// to our mouse move so we can put up the spliter over their borders
        /// </devdoc>
        private void OnChildMouseDown(Object sender, MouseEventArgs me) {
            Point newPt = Point.Empty;

            if (ShouldForwardChildMouseMessage((Control)sender, me, ref newPt)) {
                // forward the message
                this.OnMouseDown(new MouseEventArgs(me.Button, me.Clicks, newPt.X, newPt.Y, me.Delta));
                return;
            }
        }
        
        private void OnComponentAdd(Object sender, ComponentEventArgs e) {

            PropertyTabAttribute attribute = (PropertyTabAttribute) TypeDescriptor.GetAttributes(e.Component.GetType())[typeof(PropertyTabAttribute)];

            if (attribute == null) {
                return;
            }

            // add all the document items
            for (int i=0; i < attribute.TabClasses.Length; i++) {
                if (attribute.TabScopes[i] == PropertyTabScope.Document) {
                    AddRefTab(attribute.TabClasses[i], e.Component, PropertyTabScope.Document, true);
                }
            }
        }

        private void OnComponentChanged(Object sender, ComponentChangedEventArgs e) {
            bool batchMode = GetFlag(BatchMode);
            if (batchMode || GetFlag(InternalChange) || gridView.GetInPropertySet() ||
               (currentObjects == null) || (currentObjects.Length == 0)) {
    
                if (batchMode && !gridView.GetInPropertySet()) {
                    SetFlag(BatchModeChange, true);
                }
                return;
            }

            int objectCount = currentObjects.Length;
            for (int i = 0; i < objectCount; i++) {
                if (currentObjects[i] == e.Component) {
                    Refresh(false);
                    break;
                }
            }
        }

        private void OnComponentRemove(Object sender, ComponentEventArgs e) {

            PropertyTabAttribute attribute = (PropertyTabAttribute) TypeDescriptor.GetAttributes(e.Component.GetType())[typeof(PropertyTabAttribute)];

            if (attribute == null) {
                return;
            }

            // remove all the document items
            for (int i=0; i < attribute.TabClasses.Length; i++) {
                if (attribute.TabScopes[i] == PropertyTabScope.Document) {
                    ReleaseTab(attribute.TabClasses[i], e.Component);
                }
            }
            
            for (int i = 0; i < currentObjects.Length; i++) {
                if (e.Component == currentObjects[i]) {
                    
                        object[] newObjects = new object[currentObjects.Length - 1];
                        Array.Copy(currentObjects, 0, newObjects, 0, i);
                        if (i < newObjects.Length) {
                            // Dev10 

                            Array.Copy(currentObjects, i + 1, newObjects, i, newObjects.Length - i);
                        }

                    if (!GetFlag(BatchMode)) {
                        this.SelectedObjects = newObjects;
                    }
                    else {
                        // otherwise, just dump the selection
                        //
                        gridView.ClearProps();
                        this.currentObjects = newObjects;
                        SetFlag(FullRefreshAfterBatch, true);
                    }
                }
            }

            SetupToolbar();
            
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnEnabledChanged"]/*' />        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnEnabledChanged(EventArgs e) {
            base.OnEnabledChanged(e);
            Refresh();
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnFontChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            Refresh();
       }

        /// <devdoc>
        /// </devdoc>
        internal void OnGridViewMouseWheel(MouseEventArgs e) {
            this.OnMouseWheel(e);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnHandleCreated"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            OnLayoutInternal(false);
            TypeDescriptor.Refreshed += new RefreshEventHandler(this.OnTypeDescriptorRefreshed);
            if (currentObjects != null && currentObjects.Length > 0) {
                Refresh(true);
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnHandleDestroyed"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnHandleDestroyed(EventArgs e) {
            TypeDescriptor.Refreshed -= new RefreshEventHandler(this.OnTypeDescriptorRefreshed);
            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnGotFocus"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnGotFocus(EventArgs e) {
        
            base.OnGotFocus(e);
            
            if (this.ActiveControl == null) {
                this.SetActiveControlInternal(gridView);
            }
            else {
                // sometimes the edit is still the active control
                // when it's hidden or disabled...
                if (!this.ActiveControl.FocusInternal()) {
                    this.SetActiveControlInternal(gridView);
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ScaleCore"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy) {
            int sx = (int)Math.Round(Left * dx);
            int sy = (int)Math.Round(Top * dy);
            int sw = Width;
            sw = (int)Math.Round((Left + Width) * dx - sx);
            int sh = Height;
            sh = (int)Math.Round((Top + Height) * dy - sy);
            SetBounds(sx, sy, sw, sh, BoundsSpecified.All);
        }

        private void OnLayoutInternal(bool dividerOnly) {
        
            if (!IsHandleCreated || !this.Visible) {
                return;
            }

            try {

                this.FreezePainting = true;

                if (!dividerOnly) {
                    // no toolbar or doc comment or commands, just
                    // fill the whole thing with the grid
                    if (!toolStrip.Visible && !doccomment.Visible && !hotcommands.Visible) {
                        gridView.Location = new Point(0,0);
                        gridView.Size = Size;
                        return;
                    }

                    if (toolStrip.Visible) {

                        int toolStripWidth = this.Width;
                        int toolStripHeight = ((LargeButtons) ? largeButtonSize : normalButtonSize).Height + toolStripButtonPaddingY; 
                        Rectangle toolStripBounds = new Rectangle(0,1,toolStripWidth, toolStripHeight); 
                        toolStrip.Bounds = toolStripBounds;
                                               
                        int oldY = gridView.Location.Y;
                        gridView.Location = new Point(0, toolStrip.Height + toolStrip.Top);
                        /*if (oldY < gridView.Location.Y) {
                            // since the toolbar doesn't erase it's
                            // background, we'll have to force it to happen here.
                            Brush b = new SolidBrush(BackColor);
                            Graphics g = toolbar.CreateGraphicsInternal();
                            g.FillRectangle(b, toolbar.ClientRectangle);
                            b.Dispose();
                            g.Dispose();
                            toolbar.Invalidate();
                        }*/
                    }
                    else {
                        gridView.Location = new Point(0, 0);
                    }
                }

                // now work up from the bottom
                int endSize = Size.Height;

                if (endSize < MIN_GRID_HEIGHT) {
                    return;
                }

                int maxSpace = endSize - (gridView.Location.Y + MIN_GRID_HEIGHT);
                int height;

                // if we're just moving the divider, set the requested heights
                int dcRequestedHeight = 0;
                int hcRequestedHeight = 0;
                int dcOptHeight = 0;
                int hcOptHeight = 0;

                if (dividerOnly) {
                    dcRequestedHeight = doccomment.Visible ? doccomment.Size.Height : 0;
                    hcRequestedHeight = hotcommands.Visible ? hotcommands.Size.Height : 0;
                }
                else {
                    if (doccomment.Visible) {
                        dcOptHeight = doccomment.GetOptimalHeight(Size.Width - cyDivider);
                        if (doccomment.userSized) {
                            dcRequestedHeight = doccomment.Size.Height;
                        }
                        else if (dcSizeRatio != -1) {
                            dcRequestedHeight = (this.Height * dcSizeRatio) / 100;
                        }
                        else {
                            dcRequestedHeight = dcOptHeight;
                        }
                    }

                    if (hotcommands.Visible) {
                        hcOptHeight = hotcommands.GetOptimalHeight(Size.Width - cyDivider);
                        if (hotcommands.userSized) {
                            hcRequestedHeight = hotcommands.Size.Height;
                        }
                        else if (hcSizeRatio != -1) {
                            hcRequestedHeight = (this.Height * hcSizeRatio) / 100;
                        }
                        else {
                            hcRequestedHeight = hcOptHeight;
                        }
                    }
                }

                // place the help comment window
                if (dcRequestedHeight > 0) {

                    maxSpace -= cyDivider;

                    if (hcRequestedHeight == 0 || (dcRequestedHeight + hcRequestedHeight) < maxSpace) {
                        // full size
                        height = Math.Min(dcRequestedHeight, maxSpace);
                    }
                    else if (hcRequestedHeight > 0 && hcRequestedHeight < maxSpace) {
                        // give most of the space to the hot commands
                        height = maxSpace - hcRequestedHeight;
                    }
                    else {
                        // split the difference
                        height = Math.Min(dcRequestedHeight, maxSpace / 2 - 1);
                    }

                    height = Math.Max(height, cyDivider * 2);

                    doccomment.SetBounds(0, endSize - height, Size.Width, height);

                    // if we've modified the height to less than the optimal, clear the userSized item
                    if (height <= dcOptHeight && height < dcRequestedHeight) {
                        doccomment.userSized = false;
                    }
                    else if (dcSizeRatio != -1 || doccomment.userSized) {
                        dcSizeRatio = (doccomment.Height * 100) / this.Height;
                    }

                    doccomment.Invalidate();
                    endSize = doccomment.Location.Y - cyDivider;
                    maxSpace -= height;
                }

                // place the hot commands
                if (hcRequestedHeight > 0) {
                    maxSpace -= cyDivider;


                    if (maxSpace > hcRequestedHeight) {
                        // full size
                        height = Math.Min(hcRequestedHeight, maxSpace);
                    }
                    else {
                        // what's left
                        height = maxSpace;
                    }

                    height = Math.Max(height, cyDivider * 2);

                    // if we've modified the height, clear the userSized item
                    if (height <= hcOptHeight && height < hcRequestedHeight) {
                        hotcommands.userSized = false;
                    }
                    else if (hcSizeRatio != -1 || hotcommands.userSized) {
                        hcSizeRatio = (hotcommands.Height * 100) / this.Height;
                    }

                    hotcommands.SetBounds(0, endSize - height, Size.Width, height);
                    hotcommands.Invalidate();
                    endSize = hotcommands.Location.Y - cyDivider;
                }

                gridView.Size = new Size(Size.Width, endSize - gridView.Location.Y);
            }
            finally {
                this.FreezePainting = false;
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnMouseDown"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnMouseDown(MouseEventArgs me) {
            SnappableControl target = DividerInside(me.X,me.Y);
            if (target != null && me.Button == MouseButtons.Left) {
                // capture mouse.
                CaptureInternal = true;
                targetMove = target;
                dividerMoveY = me.Y;
                DividerDraw(dividerMoveY);
            }
            base.OnMouseDown(me);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnMouseMove"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnMouseMove(MouseEventArgs me) {

            if (dividerMoveY == -1) {
                if (DividerInside(me.X,me.Y) != null) {
                    Cursor = Cursors.HSplit;
                }
                else {
                    Cursor = null;
                }
                return;
            }

            int yNew = DividerLimitMove(targetMove, me.Y);

            if (yNew != dividerMoveY) {
                DividerDraw(dividerMoveY);
                dividerMoveY = yNew;
                DividerDraw(dividerMoveY);
            }
            base.OnMouseMove(me);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnMouseUp"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnMouseUp(MouseEventArgs me) {
            if (dividerMoveY == -1)
                return;

            Cursor = null;

            DividerDraw(dividerMoveY);
            dividerMoveY = DividerLimitMove(targetMove, me.Y);
            Rectangle rectDoc = targetMove.Bounds;
            if (dividerMoveY != rectDoc.Y) {
                int yNew = rectDoc.Height + rectDoc.Y - dividerMoveY - (cyDivider / 2); // we subtract two so the mouse is still over the divider
                Size size = targetMove.Size;
                size.Height = Math.Max(0,yNew);
                targetMove.Size = size;
                targetMove.userSized = true;
                OnLayoutInternal(true);
                // invalidate the divider area so we cleanup anything
                // left by the xor
                Invalidate(new Rectangle(0, me.Y - cyDivider, Size.Width, me.Y + cyDivider));

                // in case we're doing the top one, we might have wrecked stuff
                // on the grid
                gridView.Invalidate(new Rectangle(0, gridView.Size.Height - cyDivider, Size.Width, cyDivider));
            }

            // end the move
            CaptureInternal = false;
            dividerMoveY = -1;
            targetMove = null;
            base.OnMouseUp(me);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.UnsafeNativeMethods.IPropertyNotifySink.OnRequestEdit"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Called when a property on an Ole32 Object that is tagged
        /// with "requestedit" is about to be edited.
        /// See IPropertyNotifySink::OnRequestEdit
        /// </devdoc>
        int UnsafeNativeMethods.IPropertyNotifySink.OnRequestEdit(int dispID) {
            // we don't do anything here...
            return NativeMethods.S_OK;
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnResize"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnResize(EventArgs e) {
            if (IsHandleCreated && this.Visible) {
                OnLayoutInternal(false);
            }
            base.OnResize(e);
        }



        private void OnButtonClick(Object sender, EventArgs e) {
            // we don't want to steal focus from the property pages...
            if (sender != btnViewPropertyPages) {
                gridView.FocusInternal();
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnComComponentNameChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected void OnComComponentNameChanged(ComponentRenameEventArgs e) {
            ComponentRenameEventHandler handler = (ComponentRenameEventHandler)Events[EventComComponentNameChanged];
            if (handler != null) handler(this,e);
        }
        
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnNotifyPropertyValueUIItemsChanged"]/*' />
        // Seems safe - doesn't do anything interesting
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected void OnNotifyPropertyValueUIItemsChanged(object sender, EventArgs e) {
            gridView.LabelPaintMargin = 0;
            gridView.Invalidate(true);
        }
  
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnPaint"]/*' />
        // Seems safe - doesn't do anything interesting
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnPaint(PaintEventArgs pevent) {
            
            // just erase the stuff above and below the properties window
            // so we don't flicker.
            Point psheetLoc = gridView.Location;
            int width = Size.Width;
            
            Brush background;
            if (BackColor.IsSystemColor) {
                background = SystemBrushes.FromSystemColor(BackColor);
            }
            else {
                background = new SolidBrush(BackColor);
            }
            pevent.Graphics.FillRectangle(background, new Rectangle(0,0,width, psheetLoc.Y));

            int yLast = psheetLoc.Y + gridView.Size.Height;

            // fill above hotcommands
            if (hotcommands.Visible) {
                pevent.Graphics.FillRectangle(background, new Rectangle(0, yLast, width, hotcommands.Location.Y - yLast));
                yLast += hotcommands.Size.Height;
            }

            // fill above doccomment
            if (doccomment.Visible) {
                pevent.Graphics.FillRectangle(background, new Rectangle(0, yLast, width, doccomment.Location.Y - yLast));
                yLast += doccomment.Size.Height;
            }

            // anything that might be left
            pevent.Graphics.FillRectangle(background, new Rectangle(0, yLast, width, Size.Height - yLast));
            
            if (!BackColor.IsSystemColor) {
                background.Dispose();
            }
            base.OnPaint(pevent);

            if (lineBrush != null) {
                lineBrush.Dispose();
                lineBrush = null;
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnPropertySortChanged"]/*' />
        // Seems safe - just fires an event
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected virtual void OnPropertySortChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EventPropertySortChanged];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnPropertyTabChanged"]/*' />
        // Seems safe - just fires an event
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected virtual void OnPropertyTabChanged (PropertyTabChangedEventArgs e) {
            PropertyTabChangedEventHandler handler = (PropertyTabChangedEventHandler)Events[EventPropertyTabChanged];
            if (handler != null) handler(this,e);
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnPropertyValueChanged"]/*' />
        // Seems safe - just fires an event
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e) {
            PropertyValueChangedEventHandler handler = (PropertyValueChangedEventHandler)Events[EventPropertyValueChanged];
            if (handler != null) handler(this,e);
        }

        internal void OnPropertyValueSet(GridItem changedItem, object oldValue) {
            OnPropertyValueChanged(new PropertyValueChangedEventArgs(changedItem, oldValue));

            // In Level 3 announce the property value change like standalone combobox control do: "[something] selected".
            if (AccessibilityImprovements.Level3) {
                bool dropDown = false;
                Type propertyType = changedItem.PropertyDescriptor.PropertyType;
                UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(propertyType, typeof(UITypeEditor));
                if (editor != null) {
                    dropDown = editor.GetEditStyle() == UITypeEditorEditStyle.DropDown;
                }
                else {
                    var gridEntry = changedItem as GridEntry;
                    if (gridEntry != null && gridEntry.Enumerable) {
                        dropDown = true;
                    }
                }

                if (dropDown && !gridView.DropDownVisible) {
                    this.AccessibilityObject.RaiseAutomationNotification(
                        Automation.AutomationNotificationKind.ActionCompleted,
                        Automation.AutomationNotificationProcessing.All,
                        string.Format(CultureInfo.CurrentCulture, string.Format(SR.PropertyGridPropertyValueSelectedFormat, changedItem.Value)));
                }
            }
        }
        
        internal void OnSelectedGridItemChanged(GridEntry oldEntry, GridEntry newEntry) {
            OnSelectedGridItemChanged(new SelectedGridItemChangedEventArgs(oldEntry, newEntry));
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnSelectedGridItemChanged"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e) {
            SelectedGridItemChangedEventHandler handler = (SelectedGridItemChangedEventHandler)Events[EventSelectedGridItemChanged];
            
            if (handler != null) {
                handler(this, e);
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnSelectedObjectsChanged"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnSelectedObjectsChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EventSelectedObjectsChanged];
            if (handler != null) {
                handler(this, e);
            }
        }

        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
            if (e.LastTransaction) {
                // We should not refresh the grid if the selectedObject is no longer sited.
                IComponent currentSelection = SelectedObject as IComponent;
                if (currentSelection != null)
                {
                    if (currentSelection.Site == null) //The component is not logically sited...so clear the PropertyGrid Selection..
                    {
                        //Setting to null... actually will clear off the state information so that ProperyGrid is in sane State.
                        this.SelectedObject = null;
                        return;
                    }
                }
                SetFlag(BatchMode, false);
                if (GetFlag(FullRefreshAfterBatch)) {
                    this.SelectedObjects = currentObjects;
                    SetFlag(FullRefreshAfterBatch, false);
                }
                else if (GetFlag(BatchModeChange)){
                    Refresh(false);
                }
                SetFlag(BatchModeChange, false);
            }
        }
        
        private void OnTransactionOpened(object sender, EventArgs e) {
            SetFlag(BatchMode, true);
        }

        private void OnTypeDescriptorRefreshed(RefreshEventArgs e) {
            if (InvokeRequired) {
                BeginInvoke(new RefreshEventHandler(this.OnTypeDescriptorRefreshedInvoke), new object[] { e });
            }
            else {
                OnTypeDescriptorRefreshedInvoke(e);
            }
        }

        private void OnTypeDescriptorRefreshedInvoke(RefreshEventArgs e) {
            if (currentObjects != null) {
                for (int i = 0; i < currentObjects.Length; i++) {  
                    Type typeChanged = e.TypeChanged;
                    if (currentObjects[i] == e.ComponentChanged || typeChanged != null && typeChanged.IsAssignableFrom(currentObjects[i].GetType())) {
                        // clear our property hashes
                        ClearCachedProps();
                        Refresh(true);
                        return;
                    }
                }
            }
        }
        
        private void OnViewSortButtonClick(Object sender, EventArgs e) {
            try {
            
               this.FreezePainting = true;
        
               // is this tab selected? If so, do nothing.
               if (sender == viewSortButtons[selectedViewSort]) {
                   viewSortButtons[selectedViewSort].Checked = true;
                   return;
               }
   
               // check new button and uncheck old button.
               viewSortButtons[selectedViewSort].Checked = false;
   
               // find the new button in the list
               int index = 0;
               for (index = 0; index < viewSortButtons.Length; index++) {
                   if (viewSortButtons[index] == sender) {
                       break;
                   }
               }
               
               selectedViewSort = index;
               viewSortButtons[selectedViewSort].Checked = true;
               
               switch (selectedViewSort) {
                  case ALPHA:
                     propertySortValue = PropertySort.Alphabetical;
                     break;
                  case CATEGORIES:
                     propertySortValue = PropertySort.Alphabetical | PropertySort.Categorized;
                     break;
                  case NO_SORT:
                     propertySortValue = PropertySort.NoSort;
                     break;
               }

               OnPropertySortChanged(EventArgs.Empty);
               
               Refresh(false);
               OnLayoutInternal(false);
            }
            finally {
               this.FreezePainting = false;
            }
            OnButtonClick(sender, e);
            
        }

        private void OnViewTabButtonClick(Object sender, EventArgs e) {
            try {
            
               this.FreezePainting = true;
               SelectViewTabButton((ToolStripButton)sender, true);
               OnLayoutInternal(false);
               SaveTabSelection();
            }
            finally {
               this.FreezePainting = false;
            }
            OnButtonClick(sender, e);
         
        }

        private void OnViewButtonClickPP(Object sender, EventArgs e) {

            if (btnViewPropertyPages.Enabled &&
                currentObjects != null &&
                currentObjects.Length > 0) {
                Object baseObject = currentObjects[0];
                Object obj = baseObject;

                bool success = false;

                IUIService uiSvc = (IUIService)GetService(typeof(IUIService));

                try {
                    if (uiSvc != null) {
                        success = uiSvc.ShowComponentEditor(obj, this);
                    }
                    else {
                        try {
                            ComponentEditor editor = (ComponentEditor)TypeDescriptor.GetEditor(obj, typeof(ComponentEditor));
                            if (editor != null) {
                                if (editor is WindowsFormsComponentEditor) {
                                    success = ((WindowsFormsComponentEditor)editor).EditComponent(null, obj, (IWin32Window)this);
                                }
                                else {
                                    success = editor.EditComponent(obj);
                                }
                            }
                        }
                        catch {
                        }
                    }

                    if (success) {

                        if (baseObject is IComponent &&
                            connectionPointCookies[0] == null) {

                            ISite site = ((IComponent)baseObject).Site;
                            if (site != null) {
                                IComponentChangeService changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                                if (changeService != null) {
                                    try {
                                        changeService.OnComponentChanging(baseObject, null);
                                    }
                                    catch (CheckoutException coEx) {
                                        if (coEx == CheckoutException.Canceled) {
                                            return;
                                        }
                                        throw coEx;
                                    }

                                    try {
                                        // Now notify the change service that the change was successful.
                                        //
                                        SetFlag(InternalChange, true);
                                        changeService.OnComponentChanged(baseObject, null, null, null);
                                    }
                                    finally {
                                        SetFlag(InternalChange, false);
                                    }

                                }
                            }
                        }
                        gridView.Refresh();

                    }
                }
                catch (Exception ex)
                {
                    String errString = SR.ErrorPropertyPageFailed;
                    if (uiSvc != null)
                    {
                        uiSvc.ShowError(ex, errString);
                    }
                    else
                    {
                        RTLAwareMessageBox.Show(null, errString, SR.PropertyGridTitle, MessageBoxButtons.OK, MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1, 0);
                    }
                }
            }
            OnButtonClick(sender, e);
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnVisibleChanged"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);
            if (Visible && IsHandleCreated) {
                OnLayoutInternal(false);
                SetupToolbar();
            }
        }

        /*
            
        /// <summary>
        /// Returns the first child control that can take focus
        /// </summary>
        /// <retval>
        /// Returns null if no control is able to take focus
        /// </retval>
        private Control FirstFocusableChild {
            get {
                if (toolbar.Visible) {
                    return toolbar;
                }
                else if (peMain != null) {
                    return gridView;
                }
                else if (hotcommands.Visible) {
                    return hotcommands;
                }
                else if (doccomment.Visible) {
                    return doccomment;
                }
                return null;
            }
        }

        
        private Control LastFocusableChild {
            get {
                if (doccomment.Visible) {
                    return doccomment;
                }
                else if (hotcommands.Visible) {
                    return hotcommands;
                }
                else if (peMain != null) {
                    return gridView;
                }
                else if (toolbar.Visible) {
                    return toolbar;
                }
                return null;
            }
        }

        // 















































*/
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ProcessDialogKey"]/*' />
        /// <devdoc>
        /// Returns the last child control that can take focus
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode) {
                case Keys.Tab:
                     if (((keyData & Keys.Control) != 0) || 
                         ((keyData & Keys.Alt) != 0)) {
                        break;
                     }
                  
                    // are we going forward?
                    if ((keyData & Keys.Shift) != 0) {
                        // this is backward
                        if (hotcommands.Visible && hotcommands.ContainsFocus) {
                            gridView.ReverseFocus();
                        }
                        else if (gridView.FocusInside) {
                            if (toolStrip.Visible) {
                                toolStrip.FocusInternal();
                                if (AccessibilityImprovements.Level1) {
                                    // we need to select first ToolStrip item, otherwise, ToolStrip container has the focus
                                    if (toolStrip.Items.Count > 0) {
                                        toolStrip.SelectNextToolStripItem(null, /*forward =*/ true);
                                    }
                                }
                            }
                            else {
                                return base.ProcessDialogKey(keyData);
                            }
                        }
                        else {
                            // if we get here and the toolbar has focus,
                            // it means we're processing normally, so
                            // pass the focus to the parent
                            if (toolStrip.Focused || !toolStrip.Visible) {
                                return base.ProcessDialogKey(keyData);
                            }
                            else {
                                // otherwise, we're processing a message from elsewhere,
                                // wo we select our bottom guy.
                                if (hotcommands.Visible) {
                                    hotcommands.Select(false);
                                }
                                else if (peMain != null) {
                                    gridView.ReverseFocus();
                                }
                                else if (toolStrip.Visible) {
                                    toolStrip.FocusInternal();
                                }
                                else {
                                    return base.ProcessDialogKey(keyData);
                                }
                            }
                        }
                        return true;
                    }
                    else {

                        bool passToParent = false;

                        // this is forward
                        if (toolStrip.Focused) {
                            // normal stuff, just do the propsheet
                            if (peMain != null) {
                                gridView.FocusInternal();
                            }
                            else {
                                base.ProcessDialogKey(keyData);
                            }
                            return true;
                        }
                        else if (gridView.FocusInside) {
                            if (hotcommands.Visible) {
                                hotcommands.Select(true);
                                return true;
                            }
                            else {
                                passToParent = true;
                            }

                        }
                        else if (hotcommands.ContainsFocus) {
                            passToParent = true;
                        }
                        else {
                            // coming from out side, start with the toolStrip
                            if (toolStrip.Visible) {
                                toolStrip.FocusInternal();
                            }
                            else {
                                gridView.FocusInternal();
                            }
                        }

                        // nobody's claimed the focus, pass it on...
                        if (passToParent) {
                            // properties window is already selected
                            // pass on to parent
                            bool result = base.ProcessDialogKey(keyData);

                            // if we're not hosted in a windows forms thing, just give the parent the focus
                            if (!result && this.Parent == null) {
                                IntPtr hWndParent = UnsafeNativeMethods.GetParent(new HandleRef(this, Handle));
                                if (hWndParent != IntPtr.Zero) {
                                    UnsafeNativeMethods.SetFocus(new HandleRef(null, hWndParent));
                                }
                            }
                            return result;
                        }
                    }
                    return true;
                /* This conflicts with VS tab linking
                case Keys.Prior: // PAGE_UP
                    if ((keyData & Keys.Control) != 0) {
                        SelectPriorView();
                        return true;
                    }
                    break;
                case Keys.Next: //PAGE_DOWN
                    if ((keyData & Keys.Control) != 0) {
                        SelectNextView();
                        return true;
                    }
                    break;
                */

            }
            return base.ProcessDialogKey(keyData);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.Refresh"]/*' />
        public override void Refresh() {
            if (GetFlag(RefreshingProperties)) {
                return;
            }

            Refresh(true);
            base.Refresh();
        }
        
        
        private void Refresh(bool clearCached) {

            if (Disposing) {
                return;
            }
        
            if (GetFlag(RefreshingProperties)) {
                return;
            }

            try {
               this.FreezePainting = true;
               SetFlag(RefreshingProperties, true);
               
               if (clearCached) {
                  ClearCachedProps();
               }
               RefreshProperties(clearCached);
               gridView.Refresh();
               DisplayHotCommands();
           }
           finally {
               this.FreezePainting = false;
               SetFlag(RefreshingProperties, false);
           }
        }

        internal void RefreshProperties(bool clearCached) {
            
            // Clear our current cache so we can do a full refresh.
            if (clearCached && selectedViewTab != -1 && viewTabs != null) {
               PropertyTab tab = viewTabs[selectedViewTab]; 
               if (tab != null && viewTabProps != null) {
                   string tabName = tab.TabName + propertySortValue.ToString();
                   viewTabProps.Remove(tabName);
               }
            }
         
            SetFlag(PropertiesChanged, true);
            UpdateSelection();
        }


        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.RefreshTabs"]/*' />
        /// <devdoc>
        /// Refreshes the tabs of the given scope by deleting them and requerying objects and documents
        /// for them.
        /// </devdoc>
        public void RefreshTabs(PropertyTabScope tabScope) {
            
            if (tabScope < PropertyTabScope.Document) {
                throw new ArgumentException(SR.PropertyGridTabScope);
            }

            RemoveTabs(tabScope, false);

            // check the component level tabs
            if (tabScope <= PropertyTabScope.Component) {
                if (currentObjects != null && currentObjects.Length > 0) {
                    // get the subset of PropertyTabs that's common to all objects
                    Type[] tabTypes = GetCommonTabs(currentObjects, PropertyTabScope.Component);

                    for (int i = 0; i < tabTypes.Length; i++) {
                        for (int j = 0; j < currentObjects.Length; j++) {
                            AddRefTab(tabTypes[i], currentObjects[j], PropertyTabScope.Component, false);
                        }
                    }
                }
            }

            // check the document level tabs
            if (tabScope <= PropertyTabScope.Document && designerHost != null) {
                IContainer container = designerHost.Container;
                if (container != null) {
                    ComponentCollection components = container.Components;
                    if (components != null) {
                        foreach (IComponent comp in components) {
                            PropertyTabAttribute attribute = (PropertyTabAttribute) TypeDescriptor.GetAttributes(comp.GetType())[typeof(PropertyTabAttribute)];

                            if (attribute != null) {
                                for (int j = 0; j < attribute.TabClasses.Length; j++) {
                                    if (attribute.TabScopes[j] == PropertyTabScope.Document) {
                                        AddRefTab(attribute.TabClasses[j], comp, PropertyTabScope.Document, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SetupToolbar();
        }

        internal void ReleaseTab(Type tabType, Object component) {
            PropertyTab tab = null;
            int tabIndex = -1;
            for (int i = 0; i < viewTabs.Length; i++) {
                if (tabType == viewTabs[i].GetType()) {
                    tab = viewTabs[i];
                    tabIndex = i;
                    break;
                }
            }

            if (tab == null) {
                //Debug.Fail("How can we release a tab when it isn't here.");
                return;
            }

            Object[] components = tab.Components;
            bool killTab = false;

            try {
                int index = -1;
                if (components != null)
                    index = Array.IndexOf(components, component);

                if (index >= 0) {
                    object[] newComponents = new object[components.Length - 1];
                    Array.Copy(components, 0, newComponents, 0, index);
                    Array.Copy(components, index + 1, newComponents, index, components.Length - index - 1);
                    components = newComponents;
                    tab.Components = components;
                }
                killTab = (components.Length == 0);
            }
            catch (Exception e)
            {
                Debug.Fail("Bad Tab.  It's going away.", e.ToString());
                killTab = true;
            }

            // we don't remove PropertyTabScope.Global tabs here.  Our owner has to do that.
            if (killTab && viewTabScopes[tabIndex] > PropertyTabScope.Global) {
                RemoveTab(tabIndex, false);
            }
        }

        private void RemoveImage(int index) {
            imageList[NORMAL_BUTTONS].Images.RemoveAt(index);
            if (imageList[LARGE_BUTTONS] != null) {
                imageList[LARGE_BUTTONS].Images.RemoveAt(index);
            }
        }

        // removes all the tabs with a classification greater than or equal to the specified classification.
        // for example, removing PropertyTabScope.Document will remove PropertyTabScope.Document and PropertyTabScope.Component tabs
        internal void RemoveTabs(PropertyTabScope classification, bool setupToolbar) {
            if (classification == PropertyTabScope.Static) {
                throw new ArgumentException(SR.PropertyGridRemoveStaticTabs);
            }
            
            // in case we've been disposed
            if (viewTabButtons == null || viewTabs == null || viewTabScopes == null) {
                return;
            }

            ToolStripButton selectedButton = (selectedViewTab >=0 && selectedViewTab < viewTabButtons.Length ? viewTabButtons[selectedViewTab] : null);

            for (int i = viewTabs.Length-1; i >= 0; i--) {
                if (viewTabScopes[i] >= classification) {

                    // adjust the selected view tab because we're deleting.
                    if (selectedViewTab == i) {
                        selectedViewTab = -1;
                    }
                    else if (selectedViewTab > i) {
                        selectedViewTab--;
                    }
                    
                    PropertyTab[] newTabs = new PropertyTab[viewTabs.Length - 1];
                    Array.Copy(viewTabs, 0, newTabs, 0, i);
                    Array.Copy(viewTabs, i + 1, newTabs, i, viewTabs.Length - i - 1);
                    viewTabs = newTabs;

                    PropertyTabScope[] newTabScopes = new PropertyTabScope[viewTabScopes.Length - 1];
                    Array.Copy(viewTabScopes, 0, newTabScopes, 0, i);
                    Array.Copy(viewTabScopes, i + 1, newTabScopes, i, viewTabScopes.Length - i - 1);
                    viewTabScopes = newTabScopes;

                    viewTabsDirty = true;
                }
            }

            if (setupToolbar && viewTabsDirty) {
                SetupToolbar();

                Debug.Assert(viewTabs != null && viewTabs.Length > 0, "Holy Moly!  We don't have any tabs left!");

                selectedViewTab = -1;
                SelectViewTabButtonDefault(selectedButton);

                // clear the component refs of the tabs
                for (int i = 0; i < viewTabs.Length; i++) {
                    viewTabs[i].Components = new Object[0];
                }
            }
        }

        internal void RemoveTab(int tabIndex, bool setupToolbar) {
            Debug.Assert(viewTabs != null, "Tab array destroyed!");

            if (tabIndex >= viewTabs.Length || tabIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(tabIndex), SR.PropertyGridBadTabIndex);
            }

            if (viewTabScopes[tabIndex] == PropertyTabScope.Static) {
                throw new ArgumentException(SR.PropertyGridRemoveStaticTabs);
            }


            if (selectedViewTab == tabIndex) {
                selectedViewTab = PROPERTIES;
            }
            
            // Remove this tab from our "last selected" group
            //
            if (!GetFlag(ReInitTab) && ActiveDesigner != null) {
               int hashCode = ActiveDesigner.GetHashCode();
               if (designerSelections != null && designerSelections.ContainsKey(hashCode) && (int)designerSelections[hashCode] == tabIndex) {
                  designerSelections.Remove(hashCode);
               }
            }

            ToolStripButton selectedButton = viewTabButtons[selectedViewTab];

            PropertyTab[] newTabs = new PropertyTab[viewTabs.Length - 1];
            Array.Copy(viewTabs, 0, newTabs, 0, tabIndex);
            Array.Copy(viewTabs, tabIndex + 1, newTabs, tabIndex, viewTabs.Length - tabIndex - 1);
            viewTabs = newTabs;

            PropertyTabScope[] newTabScopes = new PropertyTabScope[viewTabScopes.Length - 1];
            Array.Copy(viewTabScopes, 0, newTabScopes, 0, tabIndex);
            Array.Copy(viewTabScopes, tabIndex + 1, newTabScopes, tabIndex, viewTabScopes.Length - tabIndex - 1);
            viewTabScopes = newTabScopes;

            viewTabsDirty = true;

            if (setupToolbar) {
                SetupToolbar();
                selectedViewTab = -1;
                SelectViewTabButtonDefault(selectedButton);
            }
        }

        internal void RemoveTab(Type tabType) {
            PropertyTab tab = null;
            int tabIndex = -1;
            for (int i = 0; i < viewTabs.Length; i++) {
                if (tabType == viewTabs[i].GetType()) {
                    tab = viewTabs[i];
                    tabIndex = i;
                    break;
                }
            }

            // just quit if the tab isn't present.
            if (tabIndex == -1) {
                return;
            }

            PropertyTab[] newTabs = new PropertyTab[viewTabs.Length - 1];
            Array.Copy(viewTabs, 0, newTabs, 0, tabIndex);
            Array.Copy(viewTabs, tabIndex + 1, newTabs, tabIndex, viewTabs.Length - tabIndex - 1);
            viewTabs = newTabs;

            PropertyTabScope[] newTabScopes = new PropertyTabScope[viewTabScopes.Length - 1];
            Array.Copy(viewTabScopes, 0, newTabScopes, 0, tabIndex);
            Array.Copy(viewTabScopes, tabIndex + 1, newTabScopes, tabIndex, viewTabScopes.Length - tabIndex - 1);
            viewTabScopes = newTabScopes;
            
            viewTabsDirty = true;
            SetupToolbar();
        }

        private void ResetCommandsBackColor() {
            hotcommands.ResetBackColor();
        }

        private void ResetCommandsForeColor() {
            hotcommands.ResetForeColor();
        }

        private void ResetCommandsLinkColor() {
            hotcommands.Label.ResetLinkColor();
        }

        private void ResetCommandsActiveLinkColor() {
            hotcommands.Label.ResetActiveLinkColor();
        }

        private void ResetCommandsDisabledLinkColor() {
            hotcommands.Label.ResetDisabledLinkColor();
        }

        private void ResetHelpBackColor() {
            doccomment.ResetBackColor();
        }

        private void ResetHelpForeColor() {
            doccomment.ResetBackColor();
        }

        // This method is intended for use in replacing a specific selected root object with
        // another object of the same exact type. Scenario: An immutable root object being
        // replaced with a new instance because one of its properties was changed by the user.
        //
        internal void ReplaceSelectedObject(object oldObject, object newObject) {
            Debug.Assert(oldObject != null && newObject != null && oldObject.GetType() == newObject.GetType());

            for (int i = 0; i < currentObjects.Length; ++i) {
                if (currentObjects[i] == oldObject) {
                    currentObjects[i] = newObject;
                    Refresh(true);
                    break;
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ResetSelectedProperty"]/*' />
        public void ResetSelectedProperty() {
            GetPropertyGridView().Reset();
        }

        private void SaveTabSelection() {
            if (designerHost != null) {
               if (designerSelections == null) {
                   designerSelections = new Hashtable();
               }
               designerSelections[designerHost.GetHashCode()] = selectedViewTab;
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.IComPropertyBrowser.SaveState"]/*' />
        /// <internalonly/>
        void IComPropertyBrowser.SaveState(RegistryKey optRoot) {

            if (optRoot == null) {
                return;
            }
            
            optRoot.SetValue("PbrsAlpha", (this.PropertySort == PropertySort.Alphabetical ? "1" : "0"));
            optRoot.SetValue("PbrsShowDesc", (this.HelpVisible ? "1" : "0"));
            optRoot.SetValue("PbrsShowCommands", (this.CommandsVisibleIfAvailable ? "1" : "0"));
            optRoot.SetValue("PbrsDescHeightRatio", dcSizeRatio.ToString(CultureInfo.InvariantCulture));
            optRoot.SetValue("PbrsHotCommandHeightRatio", hcSizeRatio.ToString(CultureInfo.InvariantCulture));
       }
       
        void SetHotCommandColors(bool vscompat) {
            if (vscompat) {
                hotcommands.SetColors(SystemColors.Control, SystemColors.ControlText, SystemColors.ActiveCaption, SystemColors.ActiveCaption, SystemColors.ActiveCaption, SystemColors.ControlDark);
            }
            else {
                hotcommands.SetColors(SystemColors.Control, SystemColors.ControlText, Color.Empty, Color.Empty, Color.Empty, Color.Empty);
            }
        }

        internal void SetStatusBox(string title,string desc) {
            doccomment.SetComment(title,desc);
        }

        private void SelectViewTabButton(ToolStripButton button, bool updateSelection) {
            
                Debug.Assert(viewTabButtons != null, "No view tab buttons to select!");
    
                int oldTab = selectedViewTab;
    
                if (!SelectViewTabButtonDefault(button)) {
                    Debug.Fail("Failed to find the tab!");
                }
                
                if (updateSelection) {
                    Refresh(false);
                }
        }

        private bool SelectViewTabButtonDefault(ToolStripButton button) {
                // make sure our selection number is valid
                if (selectedViewTab >= 0 && selectedViewTab >= viewTabButtons.Length) {
                    selectedViewTab = -1;
                }
    
                // is this tab button checked? If so, do nothing.
                if (selectedViewTab >=0 && selectedViewTab < viewTabButtons.Length &&
                    button == viewTabButtons[selectedViewTab]) {
                    viewTabButtons[selectedViewTab].Checked = true;
                    return true;
                }
                
                PropertyTab oldTab = null;
    
                // unselect what's selected
                if (selectedViewTab != -1) {
                    viewTabButtons[selectedViewTab].Checked = false;
                    oldTab = viewTabs[selectedViewTab];
                }
    
                // get the new index of the button
                for (int i = 0; i < viewTabButtons.Length; i++) {
                    if (viewTabButtons[i] == button) {
                        selectedViewTab = i;
                        viewTabButtons[i].Checked = true;
                        try {
                            SetFlag(TabsChanging, true);
                            OnPropertyTabChanged(new PropertyTabChangedEventArgs(oldTab, viewTabs[i]));
                        }
                        finally {
                            SetFlag(TabsChanging, false);
                        }
                        return true;
                    }
                }
    
                // select the first tab if we didn't find that one.
                selectedViewTab = PROPERTIES;
                Debug.Assert(viewTabs[PROPERTIES].GetType() == DefaultTabType, "First item is not property tab!");
                SelectViewTabButton(viewTabButtons[PROPERTIES], false);
                return false;
        }


 
        private void SetSelectState(int state) {
            
        
            if (state >= (viewTabs.Length * viewSortButtons.Length)) {
                state = 0;
            }
            else if (state < 0) {
                state = (viewTabs.Length * viewSortButtons.Length) - 1;
            }


            // NOTE: See GetSelectState for the full description
            // of the state transitions

            // views == 2 (Alpha || Categories)
            // viewTabs = viewTabs.length

            // state -> tab = state / views
            // state -> view = state % views

            int viewTypes = viewSortButtons.Length;
            
            if (viewTypes > 0) {
            
                int tab = state / viewTypes;
                int view = state % viewTypes;
    
                Debug.Assert(tab < viewTabs.Length, "Trying to select invalid tab!");
                Debug.Assert(view < viewSortButtons.Length, "Can't select view type > 1");
    
                OnViewTabButtonClick(viewTabButtons[tab], EventArgs.Empty);
                OnViewSortButtonClick(viewSortButtons[view], EventArgs.Empty);
            }
        }

        private void SetToolStripRenderer() {
            if (DrawFlatToolbar || (SystemInformation.HighContrast && AccessibilityImprovements.Level1)) {
                // use an office look and feel with system colors 
                ProfessionalColorTable colorTable = new ProfessionalColorTable();
                colorTable.UseSystemColors = true;
                ToolStripRenderer = new ToolStripProfessionalRenderer(colorTable);
            }
            else {
                ToolStripRenderer = new ToolStripSystemRenderer();
            }
        }
        

        
        private void SetupToolbar() {
            SetupToolbar(false);
        }

        private void SetupToolbar(bool fullRebuild) {

            // if the tab array hasn't changed, don't bother to do all
            // this work.
            //
            if (!viewTabsDirty && !fullRebuild) {
                return;
            }
            
            try {
               this.FreezePainting = true;
   
   
               if (imageList[NORMAL_BUTTONS] == null || fullRebuild) {
                   imageList[NORMAL_BUTTONS] = new ImageList();
                   if (DpiHelper.IsScalingRequired) {
                       imageList[NORMAL_BUTTONS].ImageSize = normalButtonSize;
                   }
               }
               
               // setup our event handlers
               EventHandler ehViewTab = new EventHandler(this.OnViewTabButtonClick);
               EventHandler ehViewType = new EventHandler(this.OnViewSortButtonClick);
               EventHandler ehPP = new EventHandler(this.OnViewButtonClickPP);
   
               Bitmap b;
               int i;
   
   
               // we manange the buttons as a seperate list so the toobar doesn't flash
               ArrayList buttonList; 
               
               if (fullRebuild) {
                  buttonList = new ArrayList();
               }
               else {
                  buttonList = new ArrayList(toolStrip.Items);
               }
   
               // setup the view type buttons.  We only need to do this once
               if (viewSortButtons == null || fullRebuild) {
                   viewSortButtons = new ToolStripButton[3];
   
                   int alphaIndex = -1;
                   int categoryIndex = -1;

                   try {
                       if (bmpAlpha == null) {
                           bmpAlpha = SortByPropertyImage;
                       }
                       alphaIndex = AddImage(bmpAlpha);
                   }
                   catch (Exception e) {
                       Debug.Fail("Failed to load Alpha bitmap", e.ToString());
                   }
   
                   try {
                       if (bmpCategory == null) {
                           bmpCategory = SortByCategoryImage;
                       }
                       categoryIndex = AddImage(bmpCategory);
                   }
                   catch (Exception e) {
                       Debug.Fail("Failed to load category bitmap", e.ToString());
                   }
   
                   viewSortButtons[ALPHA] = CreatePushButton(SR.PBRSToolTipAlphabetic, alphaIndex, ehViewType, true);
                   viewSortButtons[CATEGORIES] = CreatePushButton(SR.PBRSToolTipCategorized, categoryIndex, ehViewType, true);
                   
                   // we create a dummy hidden button for view sort
                   viewSortButtons[NO_SORT] = CreatePushButton("", 0, ehViewType, true);
                   viewSortButtons[NO_SORT].Visible = false;
   
                   // add the viewType buttons and a separator
                   for (i = 0; i < viewSortButtons.Length; i++) {
                       buttonList.Add(viewSortButtons[i]);
                   }
               }
               else {
                   // clear all the items from the toolStrip and image list after the first two
                   int items = buttonList.Count; 
   
                   for (i = items-1; i >= 2; i--) {
                       buttonList.RemoveAt(i);
                   }
   
                   items = imageList[NORMAL_BUTTONS].Images.Count;
   
                   for (i = items-1; i >= 2; i--) {
                       RemoveImage(i);
                   }
               }
   
               buttonList.Add(separator1);
   
               // here's our buttons array
               viewTabButtons = new ToolStripButton[viewTabs.Length];
               bool doAdd = viewTabs.Length > 1;
   
               // if we've only got the properties tab, don't add
               // the button (or we'll just have a properties button that you can't do anything with)
               // setup the view tab buttons
               for (i = 0; i < viewTabs.Length; i++) {
                   try {
                       b = viewTabs[i].Bitmap;
                       viewTabButtons[i] = CreatePushButton(viewTabs[i].TabName, AddImage(b), ehViewTab, true);
                       if (doAdd) {
                           buttonList.Add(viewTabButtons[i]);
                       }
                   }
                   catch (Exception ex) {
                       Debug.Fail(ex.ToString());
                   }
               }
   
               // if we didn't add anything, we don't need another separator either.
               if (doAdd) {
                   buttonList.Add(separator2);
               }
   
               // add the design page button
               int designpg = 0;

               try {
                   if (bmpPropPage == null) {
                       bmpPropPage = ShowPropertyPageImage;
                   }
                   designpg = AddImage(bmpPropPage);
               }
               catch (Exception e) {
                   Debug.Fail(e.ToString());
               }
   
               // we recreate this every time to ensure it's at the end
               //
               btnViewPropertyPages = CreatePushButton(SR.PBRSToolTipPropertyPages, designpg, ehPP, false);
               btnViewPropertyPages.Enabled = false;
               buttonList.Add(btnViewPropertyPages);
   
               // Dispose this so it will get recreated for any new buttons.
               if (imageList[LARGE_BUTTONS] != null) {
                   imageList[LARGE_BUTTONS].Dispose();
                   imageList[LARGE_BUTTONS] = null;
               }
   
               if (buttonType != NORMAL_BUTTONS) {
                   EnsureLargeButtons();
               }
   
               toolStrip.ImageList = imageList[this.buttonType];

               toolStrip.SuspendLayout();
               toolStrip.Items.Clear();
               for (int j = 0; j < buttonList.Count; j++) {
                    toolStrip.Items.Add(buttonList[j] as ToolStripItem);
               }
               toolStrip.ResumeLayout();
               
               if (viewTabsDirty) {
                  // if we're redoing our tabs make sure
                  // we setup the toolbar area correctly.
                  //
                  OnLayoutInternal(false);
               }
               
               viewTabsDirty = false;
           }
           finally {
               this.FreezePainting = false;
           }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ShowEventsButton"]/*' />
        protected void ShowEventsButton(bool value) {
            if (viewTabs != null && viewTabs.Length > EVENTS && (viewTabs[EVENTS] is EventsTab)) {
               
                Debug.Assert(viewTabButtons != null && viewTabButtons.Length > EVENTS && viewTabButtons[EVENTS] != null, "Events button is not at EVENTS position");
                viewTabButtons[EVENTS].Visible = value;
                if (!value && selectedViewTab == EVENTS) {
                    SelectViewTabButton(viewTabButtons[PROPERTIES], true);
                }
            }

            UpdatePropertiesViewTabVisibility();            
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SortByPropertyImage"]/*' />
        /// <devdoc>
        /// This 16x16 Bitmap is applied to the button which orders properties alphabetically.
        /// </devdoc>      
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        protected virtual Bitmap SortByPropertyImage {
            get {
                return new Bitmap(typeof(PropertyGrid), "PBAlpha.bmp");
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.SortByCategoryImage"]/*' />
        /// <devdoc>
        /// This 16x16 Bitmap is applied to the button which displays properties under the assigned categories.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        protected virtual Bitmap SortByCategoryImage {
            get {
                return new Bitmap(typeof(PropertyGrid), "PBCatego.bmp");
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.ShowPropertyPageImage"]/*' />
        /// <devdoc>
        /// This 16x16 Bitmap is applied to the button which displays property page in the designer pane.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        protected virtual Bitmap ShowPropertyPageImage {
            get {
                return new Bitmap(typeof(PropertyGrid), "PBPPage.bmp");
            }
        }

        private bool ShouldSerializeCommandsBackColor() {
            return hotcommands.ShouldSerializeBackColor();
        }

        private bool ShouldSerializeCommandsForeColor() {
            return hotcommands.ShouldSerializeForeColor();
        }

        private bool ShouldSerializeCommandsLinkColor() {
            return hotcommands.Label.ShouldSerializeLinkColor();
        }

        private bool ShouldSerializeCommandsActiveLinkColor() {
            return hotcommands.Label.ShouldSerializeActiveLinkColor();
        }

        private bool ShouldSerializeCommandsDisabledLinkColor() {
            return hotcommands.Label.ShouldSerializeDisabledLinkColor();
        }

        /// <devdoc>
        ///  Sinks the property notify events on all the objects we are currently
        ///  browsing.
        ///
        ///  See IPropertyNotifySink
        /// </devdoc>
        private void SinkPropertyNotifyEvents() {
            // first clear any existing sinks.
            for (int i = 0;connectionPointCookies != null && i < connectionPointCookies.Length; i++) {
                if (connectionPointCookies[i] != null) {
                    connectionPointCookies[i].Disconnect();
                    connectionPointCookies[i] = null;
                }
            }

            if (currentObjects == null || currentObjects.Length == 0) {
                connectionPointCookies = null;
                return;
            }

            // it's okay if our array is too big...we'll just reuse it and ignore the empty slots.
            if (connectionPointCookies == null || (currentObjects.Length > connectionPointCookies.Length)) {
                connectionPointCookies = new AxHost.ConnectionPointCookie[currentObjects.Length];
            }
            
            for (int i = 0; i < currentObjects.Length; i++) {
                try {
                    Object obj = GetUnwrappedObject(i);

                    if (!Marshal.IsComObject(obj)) {
                        continue;
                    }
                    connectionPointCookies[i] = new AxHost.ConnectionPointCookie(obj, this, typeof(UnsafeNativeMethods.IPropertyNotifySink), /*throwException*/ false);
                }
                catch {
                    // guess we failed eh?
                }
            }
        }

        private bool ShouldForwardChildMouseMessage(Control child, MouseEventArgs me, ref Point pt) {

            Size size = child.Size;

            // are we within two pixels of the edge?
            if (me.Y <= 1 || (size.Height - me.Y) <= 1) {
                // convert the coordinates to
                NativeMethods.POINT temp = new NativeMethods.POINT();
                temp.x = me.X;
                temp.y = me.Y;
                UnsafeNativeMethods.MapWindowPoints(new HandleRef(child, child.Handle), new HandleRef(this, Handle), temp, 1);

                // forward the message
                pt.X = temp.x;
                pt.Y = temp.y;
                return true;
            }
            return false;
        }

        private void UpdatePropertiesViewTabVisibility() {
            // If the only view available is properties-view, there's no need to show the button.
            //
            if (viewTabButtons != null) {
                int nOtherViewsVisible = 0;
                for(int i=1; i < viewTabButtons.Length; i++) { // Starts at index 1, since index 0 is properties-view
                    if (viewTabButtons[i].Visible) {
                        nOtherViewsVisible++;
                    }
                }
                if (nOtherViewsVisible > 0) {
                    viewTabButtons[PROPERTIES].Visible = true;
                    separator2.Visible = true;
                }
                else {
                    viewTabButtons[PROPERTIES].Visible = false;
                    separator2.Visible = false;
                }
            }
        }

        internal void UpdateSelection() {

            if (!GetFlag(PropertiesChanged)) {
                return;
            }
            
            if (viewTabs == null) {
                return;
            }
            
            string tabName = viewTabs[selectedViewTab].TabName + propertySortValue.ToString();

            if (viewTabProps != null && viewTabProps.ContainsKey(tabName)) {
               peMain = (GridEntry)viewTabProps[tabName];
               if (peMain != null) {
                   peMain.Refresh();
               }
            }
            else {
               if (currentObjects != null && currentObjects.Length > 0) {
                   peMain = (GridEntry)GridEntry.Create(gridView, currentObjects, new PropertyGridServiceProvider(this), designerHost, this.SelectedTab, propertySortValue);
               }
               else {
                   peMain = null;
               }
   
               if (peMain == null) {
                   currentPropEntries = new GridEntryCollection(null, new GridEntry[0]);
                   gridView.ClearProps();
                   return;
               }
   
               if (BrowsableAttributes != null) {
                   peMain.BrowsableAttributes = BrowsableAttributes;
               }

               if (viewTabProps == null) {
                    viewTabProps = new Hashtable();
               }
               
               viewTabProps[tabName] = peMain;
            }

            // get entries.
            currentPropEntries = peMain.Children;
            peDefault = peMain.DefaultChild;
            gridView.Invalidate();
        }

        /// <devdoc>
        ///     Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </devdoc>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))
        ]
        public bool UseCompatibleTextRendering {
            get{
                return base.UseCompatibleTextRenderingInt;
            }
            set{
                base.UseCompatibleTextRenderingInt = value;
                doccomment.UpdateTextRenderingEngine();
                gridView.Invalidate();
            }
        }

        internal override bool SupportsUiaProviders {
            get {
                return AccessibilityImprovements.Level3;
            }
        }

        /// <devdoc>
        ///     Determines whether the control supports rendering text using GDI+ and GDI.
        ///     This is provided for container controls to iterate through its children to set UseCompatibleTextRendering to the same
        ///     value if the child control supports it.
        /// </devdoc>
        internal override bool SupportsUseCompatibleTextRendering {
            get {
                return true;
            }
        }

        internal override bool AllowsKeyboardToolTip() {
            return false;
        }

        // a mini version of process dialog key
        // for responding to WM_GETDLGCODE
        internal bool WantsTab(bool forward) {
            if (forward) {
                return toolStrip.Visible && toolStrip.Focused;
            }
            else {
                return gridView.ContainsFocus && toolStrip.Visible;
            }
        }

        private string propName;
        private int    dwMsg;

        private void GetDataFromCopyData(IntPtr lparam) {
            NativeMethods.COPYDATASTRUCT cds = (NativeMethods.COPYDATASTRUCT)UnsafeNativeMethods.PtrToStructure(lparam, typeof(NativeMethods.COPYDATASTRUCT));

            if (cds != null && cds.lpData != IntPtr.Zero) {
                propName = Marshal.PtrToStringAuto(cds.lpData);
                dwMsg = cds.dwData;
            }
        }
        
        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.OnSystemColorsChanged"]/*' />
        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected override void OnSystemColorsChanged(EventArgs e) {
            // refresh the toolbar buttons
            SetupToolbar(true);
            
            // this doesn't stick the first time we do it...
            // either probably a toolbar issue, maybe GDI+, so we call it again
            // fortunately this doesn't happen very often.
            //
            if (!GetFlag(SysColorChangeRefresh)) {
               SetupToolbar(true);
               SetFlag(SysColorChangeRefresh, true);
            }
            base.OnSystemColorsChanged(e);
        }

        /// <summary>
        /// Rescaling constants.
        /// </summary>
        private void RescaleConstants() {
            normalButtonSize = LogicalToDeviceUnits(DEFAULT_NORMAL_BUTTON_SIZE);
            largeButtonSize = LogicalToDeviceUnits(DEFAULT_LARGE_BUTTON_SIZE);
            cyDivider = LogicalToDeviceUnits(CYDIVIDER);
            toolStripButtonPaddingY = LogicalToDeviceUnits(TOOLSTRIP_BUTTON_PADDING_Y); 
        }

        /// <summary>
        /// Rescale constants when DPI changed
        /// </summary>
        /// <param name="deviceDpiOld">old dpi</param>
        /// <param name="deviceDpiNew">new dpi</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            RescaleConstants();
            SetupToolbar(true);
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.WndProc"]/*' />
        // 


        [SuppressMessage("Microsoft.Security", "CA2114:MethodSecurityShouldBeASupersetOfType")]
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {

            switch (m.Msg) {
                case NativeMethods.WM_UNDO:
                    if ((long)m.LParam == 0) {
                        gridView.DoUndoCommand();
                    }
                    else {
                        m.Result = CanUndo ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;
                case NativeMethods.WM_CUT:
                    if ((long)m.LParam == 0) {
                        gridView.DoCutCommand();
                    }
                    else {
                        m.Result = CanCut ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;

                case NativeMethods.WM_COPY:
                    if ((long)m.LParam == 0) {
                        gridView.DoCopyCommand();
                    }
                    else {
                        m.Result = CanCopy ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;

                case NativeMethods.WM_PASTE:
                    if ((long)m.LParam == 0) {
                        gridView.DoPasteCommand();
                    }
                    else {
                        m.Result = CanPaste ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;
                
                case NativeMethods.WM_COPYDATA:
                    GetDataFromCopyData(m.LParam);
                    m.Result = (IntPtr)1;
                    return;
                case AutomationMessages.PGM_GETBUTTONCOUNT:
                    if (toolStrip != null) {
                        m.Result = (IntPtr)toolStrip.Items.Count;
                        return;
                    }
                    break;
                case AutomationMessages.PGM_GETBUTTONSTATE:
                    if (toolStrip != null) {
                        int index = unchecked((int)(long)m.WParam);
                        if( index >= 0 && index < toolStrip.Items.Count ) {
                            ToolStripButton button = toolStrip.Items[index] as ToolStripButton;
                            if (button != null) {
                                m.Result = (IntPtr)(button.Checked ? 1 : 0);
                            }
                            else {
                                m.Result = IntPtr.Zero;
                            }
                        }
                        return;
                    }
                    break;
                case AutomationMessages.PGM_SETBUTTONSTATE:
                    if (toolStrip != null) {
                        int index = unchecked((int)(long)m.WParam);
                        if( index >= 0 && index < toolStrip.Items.Count ) {
                            ToolStripButton button = toolStrip.Items[index] as ToolStripButton;

                            if (button != null) {
                                button.Checked = !button.Checked;
                                // special treatment for the properies page button
                                if (button == btnViewPropertyPages) {
                                    OnViewButtonClickPP(button, EventArgs.Empty);
                                }
                                else {
                                    switch (unchecked((int)(long)m.WParam)) {
                                        case ALPHA:
                                        case CATEGORIES:
                                            OnViewSortButtonClick(button, EventArgs.Empty);
                                            break;
                                        default:
                                            SelectViewTabButton(button, true);
                                            break;
                                    }
                                }
                            }
                        }
                        return;
                    }
                    break;

                case AutomationMessages.PGM_GETBUTTONTEXT:
                case AutomationMessages.PGM_GETBUTTONTOOLTIPTEXT:
                    if (toolStrip != null) {
                        int index = unchecked((int)(long)m.WParam);
                        if( index >= 0 && index < toolStrip.Items.Count ) {
                            string text = "";
                            if (m.Msg == AutomationMessages.PGM_GETBUTTONTEXT) {
                                text = toolStrip.Items[index].Text;
                            }
                            else {
                                text = toolStrip.Items[index].ToolTipText;
                            }

                            // write text into test file.
                            m.Result = AutomationMessages.WriteAutomationText(text);
                        }
                        return;
                    }
                    break;
                    
                case AutomationMessages.PGM_GETTESTINGINFO: {
                    // Get "testing info" string for Nth grid entry (or active entry if N < 0)
                    string testingInfo = gridView.GetTestingInfo(unchecked((int) (long) m.WParam));
                    m.Result = AutomationMessages.WriteAutomationText(testingInfo);
                    return;
                    }
                    
                case AutomationMessages.PGM_GETROWCOORDS:
                    if (m.Msg == this.dwMsg) {
                        m.Result = (IntPtr) gridView.GetPropertyLocation(propName, m.LParam == IntPtr.Zero, m.WParam == IntPtr.Zero);
                        return;
                    }
                    break;
                case AutomationMessages.PGM_GETSELECTEDROW:
                case AutomationMessages.PGM_GETVISIBLEROWCOUNT:
                    m.Result = gridView.SendMessage(m.Msg, m.WParam, m.LParam);
                    return;
                case AutomationMessages.PGM_SETSELECTEDTAB:
                    if( m.LParam != IntPtr.Zero ) {
                        string tabTypeName = AutomationMessages.ReadAutomationText(m.LParam);

                        for (int i = 0; i < viewTabs.Length;i++) {
                           if (viewTabs[i].GetType().FullName == tabTypeName && viewTabButtons[i].Visible) {
                               SelectViewTabButtonDefault(viewTabButtons[i]);
                               m.Result = (IntPtr)1;
                               break;
                           }
                        }
                    }
                    m.Result = (IntPtr)0;
                    return;
            }

            base.WndProc(ref m);
        }

        internal abstract class SnappableControl : Control {
            private Color borderColor = SystemColors.ControlDark;
            protected PropertyGrid ownerGrid;
            internal bool userSized = false;

            public abstract int GetOptimalHeight(int width);
            public abstract int SnapHeightRequest(int request);
            
            public SnappableControl(PropertyGrid ownerGrid) {
                this.ownerGrid = ownerGrid;
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }

            public override Cursor Cursor {
                 get {
                     return Cursors.Default;
                 }
                 set {
                     base.Cursor = value;
                 }
            }


            protected override void OnControlAdded(ControlEventArgs ce) {
                //ce.Control.MouseEnter += new EventHandler(this.OnChildMouseEnter);
            }
            
            /*
            private void OnChildMouseEnter(object sender, EventArgs e) {
                if (sender is Control) {
                    ((Control)sender).Cursor = Cursors.Default;
                }
            }
            */

            public Color BorderColor {
                get {
                    return borderColor;
                }
                set {
                    borderColor = value;
                }
            }

            protected override void OnPaint(PaintEventArgs e) {
                base.OnPaint(e);
                Rectangle r = this.ClientRectangle;
                r.Width --;
                r.Height--;
                using (Pen borderPen = new Pen(BorderColor, 1)) {
                    e.Graphics.DrawRectangle(borderPen, r);
                }
            }
        }

        /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection"]/*' />
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
        public class PropertyTabCollection : ICollection {
        
            internal static PropertyTabCollection Empty = new PropertyTabCollection(null);
            
            private  PropertyGrid   owner;
    
            internal PropertyTabCollection(PropertyGrid owner) {
                this.owner = owner;
            }
            
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.Count"]/*' />
            /// <devdoc>
            ///     Retrieves the number of member attributes.
            /// </devdoc>
            public int Count {
                get {
                    if (owner == null) {
                        return 0;
                    }
                    return owner.viewTabs.Length;
                }
            }
    
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyTabCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }
    
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyTabCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }
    
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the member attribute with the specified index.
            /// </devdoc>
            public PropertyTab this[int index] {
                get {
                    if (owner == null) {
                        throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                    }
                    return owner.viewTabs[index];
                }
            }
            
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.AddTabType"]/*' />
            public void AddTabType(Type propertyTabType) {
                if (owner == null) {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                owner.AddTab(propertyTabType, PropertyTabScope.Global);
            }
            
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.AddTabType1"]/*' />
            public void AddTabType(Type propertyTabType, PropertyTabScope tabScope) {
                if (owner == null) {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                owner.AddTab(propertyTabType, tabScope);
            }
            
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.Clear"]/*' />
            /// <devdoc>
            /// Clears the tabs of the given scope or smaller.
            /// tabScope must be PropertyTabScope.Component or PropertyTabScope.Document.
            /// </devdoc>
            public void Clear(PropertyTabScope tabScope) {
                if (owner == null) {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                owner.ClearTabs(tabScope);
            }
            
            
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyTabCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                if (owner == null) {
                    return;
                }
                if (owner.viewTabs.Length > 0) {
                    System.Array.Copy(owner.viewTabs, 0, dest, index, owner.viewTabs.Length);
                }
            }
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///      Creates and retrieves a new enumerator for this collection.
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                if (owner == null) {
                    return new PropertyTab[0].GetEnumerator();
                }
                
                return owner.viewTabs.GetEnumerator();
            }
            
            /// <include file='doc\PropertyGrid.uex' path='docs/doc[@for="PropertyGrid.PropertyTabCollection.RemoveTabType"]/*' />
            public void RemoveTabType(Type propertyTabType) {
                if (owner == null) {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                owner.RemoveTab(propertyTabType);
            }
    
        }

        /// <devdoc>
        ///     An unimplemented interface.  What is this?  It is an interface that nobody ever
        ///     implements, of course? Where and why would it be used?  Why, to find cross-process
        ///     remoted objects, of course!  If a well-known object comes in from a cross process
        ///     connection, the remoting layer does contain enough type information to determine
        ///     if an object implements an interface.  It assumes that if you are going to cast
        ///     an object to an interface that you know what you're doing, and allows the cast,
        ///     even for objects that DON'T actually implement the interface.  The error here
        ///     is raised later when you make your first call on that interface pointer:  you
        ///     get a remoting exception.
        ///
        ///     This is a big problem for code that does "is" and "as" checks to detect the
        ///     presence of an interface.  We do that all over the place here, so we do a check
        ///     during parameter validation to see if an object implements IUnimplemented.  If it
        ///     does, we know that what we really have is a lying remoting proxy, and we bail.
        /// </devdoc>
        private interface IUnimplemented {}


        internal class SelectedObjectConverter : ReferenceConverter {
            public SelectedObjectConverter() : base(typeof(IComponent)) {
            }
        }

        private class PropertyGridServiceProvider : IServiceProvider {
            PropertyGrid owner;
            
            public PropertyGridServiceProvider(PropertyGrid owner) {
                this.owner = owner;
            }

            public object GetService(Type serviceType) {
               object s = null;
               
               if (owner.ActiveDesigner != null) {
                   s = owner.ActiveDesigner.GetService(serviceType);
               }

               if (s == null) {
                   s = owner.gridView.GetService(serviceType);
               }

               if (s == null && owner.Site != null) {
                   s = owner.Site.GetService(serviceType);
               }
               return s;
            }
        }
    
        /// <devdoc>
        ///     Helper class to support rendering text using either GDI or GDI+.
        /// </devdoc>
        internal static class MeasureTextHelper{
            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font ){
                return MeasureTextSimple(owner, g, text, font, new SizeF(0,0));
            }

            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font, int width ){
                return MeasureText(owner, g, text, font, new SizeF(width,999999));
            }

            public static SizeF MeasureTextSimple(PropertyGrid owner, Graphics g, string text, Font font, SizeF size ){
                SizeF bindingSize;
                if( owner.UseCompatibleTextRendering ){
                    bindingSize = g.MeasureString(text, font, size );
                }
                else{
                    bindingSize = (SizeF) TextRenderer.MeasureText(g, text, font, Size.Ceiling(size), GetTextRendererFlags() );
                }

                return bindingSize;
            }

            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font, SizeF size ){
                SizeF bindingSize;
                if( owner.UseCompatibleTextRendering ){
                    bindingSize = g.MeasureString(text, font, size );
                }
                else{
                    TextFormatFlags flags = 
                        GetTextRendererFlags()                |
                        TextFormatFlags.LeftAndRightPadding   |
                        TextFormatFlags.WordBreak             |
                        TextFormatFlags.NoFullWidthCharacterBreak;
                    
                    bindingSize = (SizeF) TextRenderer.MeasureText(g, text, font, Size.Ceiling(size), flags );
                }

                return bindingSize;
            }

            public static TextFormatFlags GetTextRendererFlags(){
                return  TextFormatFlags.PreserveGraphicsClipping | 
                        TextFormatFlags.PreserveGraphicsTranslateTransform;
            }
        }    
    }

    internal static class AutomationMessages {
        private const int WM_USER = NativeMethods.WM_USER;
        internal const int PGM_GETBUTTONCOUNT = WM_USER + 0x50;
        internal const int PGM_GETBUTTONSTATE = WM_USER + 0x52;
        internal const int PGM_SETBUTTONSTATE = WM_USER + 0x51;
        internal const int PGM_GETBUTTONTEXT = WM_USER + 0x53;
        internal const int PGM_GETBUTTONTOOLTIPTEXT = WM_USER + 0x54;
        internal const int PGM_GETROWCOORDS = WM_USER + 0x55;
        internal const int PGM_GETVISIBLEROWCOUNT = WM_USER + 0x56;
        internal const int PGM_GETSELECTEDROW = WM_USER + 0x57;
        internal const int PGM_SETSELECTEDTAB = WM_USER + 0x58; // DO NOT CHANGE THIS : VC uses it!
        internal const int PGM_GETTESTINGINFO = WM_USER + 0x59;

        /// <summary>
        ///     Writes the specified text into a temporary file of the form %TEMP%\"Maui.[file id].log", where 
        ///     'file id' is a unique id that is return by this method.
        ///     This is to support MAUI interaction with the PropertyGrid control and MAUI should remove the 
        ///     file after used.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static IntPtr WriteAutomationText(string text)
        {
            IntPtr fileId = IntPtr.Zero;
            string fullFileName = GenerateLogFileName(ref fileId);

            if (fullFileName != null)
            {
                try
                {
                    FileStream fs = new FileStream(fullFileName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(text);
                    sw.Dispose();
                    fs.Dispose();
                }
                catch
                {
                    fileId = IntPtr.Zero;
                }
            }

            return fileId;
        }

        /// <summary>
        ///     Writes the contents of a test file as text.  This file needs to have the following naming convention:
        ///     %TEMP%\"Maui.[file id].log", where 'file id' is a unique id sent to this window.
        ///     This is to support MAUI interaction with the PropertyGrid control and MAUI should create/delete this file.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string ReadAutomationText(IntPtr fileId)
        {
            Debug.Assert(fileId != IntPtr.Zero, "Invalid file Id");

            string text = null;

            if (fileId != IntPtr.Zero)
            {
                string fullFileName = GenerateLogFileName(ref fileId);
                Debug.Assert(File.Exists(fullFileName), "Automation log file does not exist");

                if (File.Exists(fullFileName))
                {
                    try
                    {
                        FileStream fs = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs);
                        text = sr.ReadToEnd();
                        sr.Dispose();
                        fs.Dispose();
                    }
                    catch
                    {
                        text = null;
                    }
                }
            }

            return text;
        }

        /// <summary>
        ///     Generate log file from id.
        /// </summary>
        private static string GenerateLogFileName(ref IntPtr fileId)
        {
            string fullFileName = null;

            string filePath = System.Environment.GetEnvironmentVariable("TEMP");
            Debug.Assert(filePath != null, "Could not get value of the TEMP environment variable");

            if (filePath != null)
            {
                if (fileId == IntPtr.Zero) // Create id
                {
                    Random rnd = new Random(DateTime.Now.Millisecond);
                    fileId = new IntPtr(rnd.Next());
                }

                fullFileName = filePath + "\\Maui" + fileId + ".log";
            }

            return fullFileName;
        }
    }

}

