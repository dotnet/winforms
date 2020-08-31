// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    [Designer("System.Windows.Forms.Design.PropertyGridDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionPropertyGrid))]
    public class PropertyGrid : ContainerControl, IComPropertyBrowser, Ole32.IPropertyNotifySink
    {
        private readonly DocComment _doccomment;
        private int _dcSizeRatio = -1;
        private int _hcSizeRatio = -1;
        private readonly HotCommands _hotcommands;
        private readonly ToolStrip _toolStrip;

        private bool _helpVisible = true;
        private bool _toolbarVisible = true;

        private ImageList[] _imageList = new ImageList[2];
        private Bitmap _bmpAlpha;
        private Bitmap _bmpCategory;
        private Bitmap _bmpPropPage;

        // Our array of viewTabs
        private bool _viewTabsDirty = true;
        private bool _drawFlatToolBar;
        private PropertyTab[] _viewTabs = Array.Empty<PropertyTab>();
        private PropertyTabScope[] _viewTabScopes = Array.Empty<PropertyTabScope>();
        private Hashtable _viewTabProps;

        private ToolStripButton[] _viewTabButtons;
        private int _selectedViewTab;

        // Our view type buttons (Alpha vs. categorized)
        private ToolStripButton[] _viewSortButtons;
        private int _selectedViewSort;
        private PropertySort _propertySortValue;

        private ToolStripButton _btnViewPropertyPages;
        private readonly ToolStripSeparator _separator1;
        private readonly ToolStripSeparator _separator2;
        private int _buttonType = NormalButtonSize;

        // Our main view
        private readonly PropertyGridView _gridView;

        private IDesignerHost _designerHost;
        private IDesignerEventService _designerEventService;

        private Hashtable _designerSelections;

        private GridEntry _peDefault;
        private GridEntry _peMain;
        private GridEntryCollection _currentPropEntries;
        private object[] _currentObjects;

        private int _paintFrozen;
        private Color _lineColor = SystemInformation.HighContrast ? SystemColors.ControlDarkDark : SystemColors.InactiveBorder;
        internal bool _developerOverride;
        private Color _categoryForeColor = SystemColors.ControlText;
        private Color _categorySplitterColor = SystemColors.Control;
        private Color _viewBorderColor = SystemColors.ControlDark;
        private Color _selectedItemWithFocusForeColor = SystemColors.HighlightText;
        private Color _selectedItemWithFocusBackColor = SystemColors.Highlight;
        private bool _canShowVisualStyleGlyphs = true;

        private AttributeCollection _browsableAttributes;

        private SnappableControl _targetMove;
        private int _dividerMoveY = -1;
        private const int CYDIVIDER = 3;
        private static int s_cyDivider = CYDIVIDER;
        private const int MinGridHeight = 20;

        private const int PROPERTIES = 0;
        private const int EVENTS = 1;
        private const int ALPHA = 1;
        private const int CATEGORIES = 0;
        private const int NO_SORT = 2;

        private const int NormalButtonSize = 0;
        private const int LargeButtonSize = 1;

        private const int ToolStripButtonPaddingY = 9;
        private int _toolStripButtonPaddingY = ToolStripButtonPaddingY;
        private static readonly Size s_defaultLargeButtonSize = new Size(32, 32);
        private static readonly Size s_defaultNormalButtonSize = new Size(16, 16);
        private static Size s_largeButtonSize = s_defaultLargeButtonSize;
        private static Size s_normalButtonSize = s_defaultNormalButtonSize;
        private static bool s_isScalingInitialized;

        private const ushort PropertiesChanged = 0x0001;
        private const ushort GotDesignerEventService = 0x0002;
        private const ushort InternalChange = 0x0004;
        private const ushort TabsChanging = 0x0008;
        private const ushort BatchMode = 0x0010;
        private const ushort ReInitTab = 0x0020;
        private const ushort SysColorChangeRefresh = 0x0040;
        private const ushort FullRefreshAfterBatch = 0x0080;
        private const ushort BatchModeChange = 0x0100;
        private const ushort RefreshingProperties = 0x0200;

        private ushort Flags;

        private bool GetFlag(ushort flag)
        {
            return (Flags & flag) != (ushort)0;
        }

        private void SetFlag(ushort flag, bool value)
        {
            if (value)
            {
                Flags |= flag;
            }
            else
            {
                Flags &= (ushort)~flag;
            }
        }

        private readonly ComponentEventHandler onComponentAdd;
        private readonly ComponentEventHandler onComponentRemove;
        private readonly ComponentChangedEventHandler onComponentChanged;

        // the cookies for our connection points on objects that support IPropertyNotifySink
        //
        private AxHost.ConnectionPointCookie[] connectionPointCookies;

        private static readonly object EventPropertyValueChanged = new object();
        private static readonly object EventComComponentNameChanged = new object();
        private static readonly object EventPropertyTabChanged = new object();
        private static readonly object EventSelectedGridItemChanged = new object();
        private static readonly object EventPropertySortChanged = new object();
        private static readonly object EventSelectedObjectsChanged = new object();

        public PropertyGrid()
        {
            onComponentAdd = new ComponentEventHandler(OnComponentAdd);
            onComponentRemove = new ComponentEventHandler(OnComponentRemove);
            onComponentChanged = new ComponentChangedEventHandler(OnComponentChanged);

            SuspendLayout();
            AutoScaleMode = AutoScaleMode.None;

            SetStyle(ControlStyles.UseTextForAccessibility, false);

            // static variables are problem in a child level mixed mode scenario. Changing static variables cause compatibility issue.
            // So, recalculate static variables everytime property grid initialized.
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                RescaleConstants();
            }
            else
            {
                if (!s_isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        s_normalButtonSize = LogicalToDeviceUnits(s_defaultNormalButtonSize);
                        s_largeButtonSize = LogicalToDeviceUnits(s_defaultLargeButtonSize);
                    }
                    s_isScalingInitialized = true;
                }
            }

            try
            {
                _gridView = CreateGridView(null);
                _gridView.TabStop = true;
                _gridView.MouseMove += new MouseEventHandler(OnChildMouseMove);
                _gridView.MouseDown += new MouseEventHandler(OnChildMouseDown);
                _gridView.TabIndex = 2;

                _separator1 = CreateSeparatorButton();
                _separator2 = CreateSeparatorButton();

                _toolStrip = new PropertyGridToolStrip(this);
                _toolStrip.SuspendLayout();
                _toolStrip.ShowItemToolTips = true;

                _toolStrip.AccessibleRole = AccessibleRole.ToolBar;
                _toolStrip.TabStop = true;
                _toolStrip.AllowMerge = false;

                // This caption is for testing.
                _toolStrip.Text = "PropertyGridToolBar";

                // LayoutInternal handles positioning, and for perf reasons, we manually size.
                _toolStrip.Dock = DockStyle.None;
                _toolStrip.AutoSize = false;
                _toolStrip.TabIndex = 1;
                _toolStrip.ImageScalingSize = s_normalButtonSize;

                // parity with the old...
                _toolStrip.CanOverflow = false;

                // hide the grip but add in a few more pixels of padding.
                _toolStrip.GripStyle = ToolStripGripStyle.Hidden;
                Padding toolStripPadding = _toolStrip.Padding;
                toolStripPadding.Left = 2;
                _toolStrip.Padding = toolStripPadding;
                SetToolStripRenderer();

                // always add the property tab here
                AddRefTab(DefaultTabType, null, PropertyTabScope.Static, true);

                _doccomment = new DocComment(this);
                _doccomment.SuspendLayout();
                _doccomment.TabStop = false;
                _doccomment.Dock = DockStyle.None;
                _doccomment.BackColor = SystemColors.Control;
                _doccomment.ForeColor = SystemColors.ControlText;
                _doccomment.MouseMove += new MouseEventHandler(OnChildMouseMove);
                _doccomment.MouseDown += new MouseEventHandler(OnChildMouseDown);

                _hotcommands = new HotCommands(this);
                _hotcommands.SuspendLayout();
                _hotcommands.TabIndex = 3;
                _hotcommands.Dock = DockStyle.None;
                SetHotCommandColors(false);
                _hotcommands.Visible = false;
                _hotcommands.MouseMove += new MouseEventHandler(OnChildMouseMove);
                _hotcommands.MouseDown += new MouseEventHandler(OnChildMouseDown);

                Controls.AddRange(new Control[] { _doccomment, _hotcommands, _gridView, _toolStrip });

                SetActiveControl(_gridView);
                _toolStrip.ResumeLayout(false);  // SetupToolbar should perform the layout
                SetupToolbar();
                PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
                SetSelectState(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if (_doccomment != null)
                {
                    _doccomment.ResumeLayout(false);
                }
                if (_hotcommands != null)
                {
                    _hotcommands.ResumeLayout(false);
                }
                ResumeLayout(true);
            }
        }

        internal IDesignerHost ActiveDesigner
        {
            get
            {
                if (_designerHost is null)
                {
                    _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
                }
                return _designerHost;
            }
            set
            {
                if (value != _designerHost)
                {
                    SetFlag(ReInitTab, true);
                    if (_designerHost != null)
                    {
                        IComponentChangeService cs = (IComponentChangeService)_designerHost.GetService(typeof(IComponentChangeService));
                        if (cs != null)
                        {
                            cs.ComponentAdded -= onComponentAdd;
                            cs.ComponentRemoved -= onComponentRemove;
                            cs.ComponentChanged -= onComponentChanged;
                        }

                        IPropertyValueUIService pvSvc = (IPropertyValueUIService)_designerHost.GetService(typeof(IPropertyValueUIService));
                        if (pvSvc != null)
                        {
                            pvSvc.PropertyUIValueItemsChanged -= new EventHandler(OnNotifyPropertyValueUIItemsChanged);
                        }

                        _designerHost.TransactionOpened -= new EventHandler(OnTransactionOpened);
                        _designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                        SetFlag(BatchMode, false);
                        RemoveTabs(PropertyTabScope.Document, true);
                        _designerHost = null;
                    }

                    if (value != null)
                    {
                        IComponentChangeService cs = (IComponentChangeService)value.GetService(typeof(IComponentChangeService));
                        if (cs != null)
                        {
                            cs.ComponentAdded += onComponentAdd;
                            cs.ComponentRemoved += onComponentRemove;
                            cs.ComponentChanged += onComponentChanged;
                        }

                        value.TransactionOpened += new EventHandler(OnTransactionOpened);
                        value.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                        SetFlag(BatchMode, false);

                        IPropertyValueUIService pvSvc = (IPropertyValueUIService)value.GetService(typeof(IPropertyValueUIService));
                        if (pvSvc != null)
                        {
                            pvSvc.PropertyUIValueItemsChanged += new EventHandler(OnNotifyPropertyValueUIItemsChanged);
                        }
                    }

                    _designerHost = value;
                    if (_peMain != null)
                    {
                        _peMain.DesignerHost = value;
                    }
                    RefreshTabs(PropertyTabScope.Document);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll
        {
            get => base.AutoScroll;
            set => base.AutoScroll = value;
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                _toolStrip.BackColor = value;
                _toolStrip.Invalidate(true);
            }
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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AttributeCollection BrowsableAttributes
        {
            set
            {
                if (value is null || value == AttributeCollection.Empty)
                {
                    _browsableAttributes = new AttributeCollection(new Attribute[] { BrowsableAttribute.Yes });
                }
                else
                {
                    Attribute[] attributes = new Attribute[value.Count];
                    value.CopyTo(attributes, 0);
                    _browsableAttributes = new AttributeCollection(attributes);
                }
                if (_currentObjects != null && _currentObjects.Length > 0)
                {
                    if (_peMain != null)
                    {
                        _peMain.BrowsableAttributes = BrowsableAttributes;
                        Refresh(true);
                    }
                }
            }
            get
            {
                if (_browsableAttributes is null)
                {
                    _browsableAttributes = new AttributeCollection(new Attribute[] { new BrowsableAttribute(true) });
                }
                return _browsableAttributes;
            }
        }

        private bool CanCopy
        {
            get
            {
                return _gridView.CanCopy;
            }
        }

        private bool CanCut
        {
            get
            {
                return _gridView.CanCut;
            }
        }

        private bool CanPaste
        {
            get
            {
                return _gridView.CanPaste;
            }
        }

        private bool CanUndo
        {
            get
            {
                return _gridView.CanUndo;
            }
        }

        /// <summary>
        ///  true if the commands pane will be can be made visible
        ///  for the currently selected objects.  Objects that
        ///  expose verbs can show commands.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SRDescription(nameof(SR.PropertyGridCanShowCommandsDesc))]
        public virtual bool CanShowCommands
        {
            get
            {
                return _hotcommands.WouldBeVisible;
            }
        }

        /// <summary>
        ///  The text used color for category headings. The background color is determined by the LineColor property.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCategoryForeColorDesc))]
        [DefaultValue(typeof(Color), "ControlText")]
        public Color CategoryForeColor
        {
            get
            {
                return _categoryForeColor;
            }
            set
            {
                if (_categoryForeColor != value)
                {
                    _categoryForeColor = value;
                    _gridView.Invalidate();
                }
            }
        }

        /// <summary>
        ///  The background color for the hot commands region.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCommandsBackColorDesc))]
        public Color CommandsBackColor
        {
            get
            {
                return _hotcommands.BackColor;
            }
            set
            {
                _hotcommands.BackColor = value;
                _hotcommands.Label.BackColor = value;
            }
        }

        /// <summary>
        ///  The forground color for the hot commands region.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCommandsForeColorDesc))]
        public Color CommandsForeColor
        {
            get
            {
                return _hotcommands.ForeColor;
            }
            set
            {
                _hotcommands.ForeColor = value;
                _hotcommands.Label.ForeColor = value;
            }
        }

        /// <summary>
        ///  The link color for the hot commands region.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCommandsLinkColorDesc))]
        public Color CommandsLinkColor
        {
            get
            {
                return _hotcommands.Label.LinkColor;
            }
            set
            {
                _hotcommands.Label.LinkColor = value;
            }
        }

        /// <summary>
        ///  The active link color for the hot commands region.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCommandsActiveLinkColorDesc))]
        public Color CommandsActiveLinkColor
        {
            get
            {
                return _hotcommands.Label.ActiveLinkColor;
            }
            set
            {
                _hotcommands.Label.ActiveLinkColor = value;
            }
        }

        /// <summary>
        ///  The color for the hot commands region when the link is disabled.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCommandsDisabledLinkColorDesc))]
        public Color CommandsDisabledLinkColor
        {
            get
            {
                return _hotcommands.Label.DisabledLinkColor;
            }
            set
            {
                _hotcommands.Label.DisabledLinkColor = value;
            }
        }

        /// <summary>
        ///  The border color for the hot commands region
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCommandsBorderColorDesc))]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color CommandsBorderColor
        {
            get
            {
                return _hotcommands.BorderColor;
            }
            set
            {
                _hotcommands.BorderColor = value;
            }
        }

        /// <summary>
        ///  Returns true if the commands pane is currently shown.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool CommandsVisible
        {
            get
            {
                return _hotcommands.Visible;
            }
        }

        /// <summary>
        ///  Returns true if the commands pane will be shown for objects
        ///  that expose verbs.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.PropertyGridCommandsVisibleIfAvailable))]
        public virtual bool CommandsVisibleIfAvailable
        {
            get
            {
                return _hotcommands.AllowVisible;
            }
            set
            {
                bool hotcommandsVisible = _hotcommands.Visible;
                _hotcommands.AllowVisible = value;
                //PerformLayout();
                if (hotcommandsVisible != _hotcommands.Visible)
                {
                    OnLayoutInternal(false);
                    _hotcommands.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Returns a default location for showing the context menu.  This
        ///  location is the center of the active property label in the grid, and
        ///  is used useful to position the context menu when the menu is invoked
        ///  via the keyboard.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point ContextMenuDefaultLocation
        {
            get
            {
                return GetPropertyGridView().ContextMenuDefaultLocation;
            }
        }

        /// <summary>
        ///  Collection of child controls.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls
        {
            get => base.Controls;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(130, 130);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Type DefaultTabType
        {
            get
            {
                return typeof(PropertiesTab);
            }
        }

        protected bool DrawFlatToolbar
        {
            get
            {
                return _drawFlatToolBar;
            }
            set
            {
                if (_drawFlatToolBar != value)
                {
                    _drawFlatToolBar = value;
                    SetToolStripRenderer();
                }

                SetHotCommandColors(false);
            }
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

        private bool FreezePainting
        {
            get
            {
                return _paintFrozen > 0;
            }
            set
            {
                if (value && IsHandleCreated && Visible)
                {
                    if (0 == _paintFrozen++)
                    {
                        User32.SendMessageW(this, User32.WM.SETREDRAW, PARAM.FromBool(false));
                    }
                }
                if (!value)
                {
                    if (_paintFrozen == 0)
                    {
                        return;
                    }

                    if (0 == --_paintFrozen)
                    {
                        User32.SendMessageW(this, User32.WM.SETREDRAW, PARAM.FromBool(true));
                        Invalidate(true);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the help control accessibility object.
        /// </summary>
        internal AccessibleObject HelpAccessibleObject
        {
            get
            {
                return _doccomment.AccessibilityObject;
            }
        }

        /// <summary>
        ///  The background color for the help region.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridHelpBackColorDesc))]
        [DefaultValue(typeof(Color), "Control")]
        public Color HelpBackColor
        {
            get
            {
                return _doccomment.BackColor;
            }
            set
            {
                _doccomment.BackColor = value;
            }
        }

        /// <summary>
        ///  The forground color for the help region.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridHelpForeColorDesc))]
        [DefaultValue(typeof(Color), "ControlText")]
        public Color HelpForeColor
        {
            get
            {
                return _doccomment.ForeColor;
            }
            set
            {
                _doccomment.ForeColor = value;
            }
        }

        /// <summary>
        ///  The border color for the help region
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridHelpBorderColorDesc))]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color HelpBorderColor
        {
            get
            {
                return _doccomment.BorderColor;
            }
            set
            {
                _doccomment.BorderColor = value;
            }
        }

        /// <summary>
        ///  Sets or gets the visiblity state of the help pane.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(true)]
        [Localizable(true)]
        [SRDescription(nameof(SR.PropertyGridHelpVisibleDesc))]
        public virtual bool HelpVisible
        {
            get
            {
                return _helpVisible;
            }
            set
            {
                _helpVisible = value;

                _doccomment.Visible = value;
                OnLayoutInternal(false);
                Invalidate();
                _doccomment.Invalidate();
            }
        }

        /// <summary>
        ///  Gets the hot commands control accessible object.
        /// </summary>
        internal AccessibleObject HotCommandsAccessibleObject
        {
            get
            {
                return _hotcommands.AccessibilityObject;
            }
        }

        /// <summary>
        ///  Gets the main entry accessible object.
        /// </summary>
        internal AccessibleObject GridViewAccessibleObject
        {
            get
            {
                return _gridView.AccessibilityObject;
            }
        }

        /// <summary>
        ///  Gets the value indicating whether the main entry is visible.
        /// </summary>
        internal bool GridViewVisible
        {
            get
            {
                return _gridView != null && _gridView.Visible;
            }
        }

        /// <summary>
        ///  Background color for Highlighted text.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridSelectedItemWithFocusBackColorDesc))]
        [DefaultValue(typeof(Color), "Highlight")]
        public Color SelectedItemWithFocusBackColor
        {
            get
            {
                return _selectedItemWithFocusBackColor;
            }
            set
            {
                if (_selectedItemWithFocusBackColor != value)
                {
                    _selectedItemWithFocusBackColor = value;
                    _gridView.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Foreground color for Highlighted (selected) text.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridSelectedItemWithFocusForeColorDesc))]
        [DefaultValue(typeof(Color), "HighlightText")]
        public Color SelectedItemWithFocusForeColor
        {
            get
            {
                return _selectedItemWithFocusForeColor;
            }
            set
            {
                if (_selectedItemWithFocusForeColor != value)
                {
                    _selectedItemWithFocusForeColor = value;
                    _gridView.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Foreground color for disabled text in the Grid View
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridDisabledItemForeColorDesc))]
        [DefaultValue(typeof(Color), "GrayText")]
        public Color DisabledItemForeColor
        {
            get
            {
                return _gridView.GrayTextColor;
            }
            set
            {
                _gridView.GrayTextColor = value;
                _gridView.Invalidate();
            }
        }

        /// <summary>
        ///  Color for the horizontal splitter line separating property categories.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCategorySplitterColorDesc))]
        [DefaultValue(typeof(Color), "Control")]
        public Color CategorySplitterColor
        {
            get
            {
                return _categorySplitterColor;
            }
            set
            {
                if (_categorySplitterColor != value)
                {
                    _categorySplitterColor = value;
                    _gridView.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Enable/Disable use of VisualStyle glyph for PropertyGrid node expansion.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridCanShowVisualStyleGlyphsDesc))]
        [DefaultValue(true)]
        public bool CanShowVisualStyleGlyphs
        {
            get
            {
                return _canShowVisualStyleGlyphs;
            }
            set
            {
                if (_canShowVisualStyleGlyphs != value)
                {
                    _canShowVisualStyleGlyphs = value;
                    _gridView.Invalidate();
                }
            }
        }

        bool IComPropertyBrowser.InPropertySet
        {
            get
            {
                return GetPropertyGridView().GetInPropertySet();
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridLineColorDesc))]
        [DefaultValue(typeof(Color), "InactiveBorder")]
        public Color LineColor
        {
            get
            {
                return _lineColor;
            }
            set
            {
                if (_lineColor != value)
                {
                    _lineColor = value;
                    _developerOverride = true;
                    _gridView.Invalidate();
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get => base.Padding;
            set => base.Padding = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  Sets or gets the current property sort type, which can be
        ///  PropertySort.Categorized or PropertySort.Alphabetical.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(PropertySort.CategorizedAlphabetical)]
        [SRDescription(nameof(SR.PropertyGridPropertySortDesc))]
        public PropertySort PropertySort
        {
            get
            {
                return _propertySortValue;
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)PropertySort.NoSort, (int)PropertySort.CategorizedAlphabetical))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PropertySort));
                }
                ToolStripButton newButton;

                if ((value & PropertySort.Categorized) != 0)
                {
                    newButton = _viewSortButtons[CATEGORIES];
                }
                else if ((value & PropertySort.Alphabetical) != 0)
                {
                    newButton = _viewSortButtons[ALPHA];
                }
                else
                {
                    newButton = _viewSortButtons[NO_SORT];
                }

                GridItem selectedGridItem = SelectedGridItem;

                OnViewSortButtonClick(newButton, EventArgs.Empty);

                _propertySortValue = value;

                if (selectedGridItem != null)
                {
                    try
                    {
                        SelectedGridItem = selectedGridItem;
                    }
                    catch (ArgumentException)
                    {
                        // When no row is selected, SelectedGridItem returns grid entry for root
                        // object. But this is not a selectable item. So don't worry if setting SelectedGridItem
                        // cause an argument exception whe ntrying to re-select the root object. Just leave the
                        // the grid with no selected row.
                    }
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PropertyTabCollection PropertyTabs
        {
            get
            {
                return new PropertyTabCollection(this);
            }
        }

        /// <summary>
        ///  Sets a single Object into the grid to be browsed.  If multiple
        ///  objects are being browsed, this property will return the first
        ///  one in the list.  If no objects are selected, null is returned.
        /// </summary>
        [DefaultValue(null)]
        [SRDescription(nameof(SR.PropertyGridSelectedObjectDesc))]
        [SRCategory(nameof(SR.CatBehavior))]
        [TypeConverter(typeof(SelectedObjectConverter))]
        public object SelectedObject
        {
            get
            {
                if (_currentObjects is null || _currentObjects.Length == 0)
                {
                    return null;
                }
                return _currentObjects[0];
            }
            set
            {
                if (value is null)
                {
                    SelectedObjects = Array.Empty<object>();
                }
                else
                {
                    SelectedObjects = new object[] { value };
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object[] SelectedObjects
        {
            set
            {
                try
                {
                    FreezePainting = true;

                    SetFlag(FullRefreshAfterBatch, false);
                    if (GetFlag(BatchMode))
                    {
                        SetFlag(BatchModeChange, false);
                    }

                    _gridView.EnsurePendingChangesCommitted();

                    bool isSame = false;
                    bool classesSame = false;
                    bool showEvents = true;

                    // validate the array coming in
                    if (value != null && value.Length > 0)
                    {
                        for (int count = 0; count < value.Length; count++)
                        {
                            if (value[count] is null)
                            {
                                throw new ArgumentException(string.Format(SR.PropertyGridSetNull, count.ToString(CultureInfo.CurrentCulture), value.Length.ToString(CultureInfo.CurrentCulture)));
                            }
                        }
                    }
                    else
                    {
                        showEvents = false;
                    }

                    // make sure we actually changed something before we inspect tabs
                    if (_currentObjects != null && value != null &&
                        _currentObjects.Length == value.Length)
                    {
                        isSame = true;
                        classesSame = true;
                        for (int i = 0; i < value.Length && (isSame || classesSame); i++)
                        {
                            if (isSame && _currentObjects[i] != value[i])
                            {
                                isSame = false;
                            }

                            Type oldType = GetUnwrappedObject(i).GetType();

                            object objTemp = value[i];

                            if (objTemp is ICustomTypeDescriptor)
                            {
                                objTemp = ((ICustomTypeDescriptor)objTemp).GetPropertyOwner(null);
                            }
                            Type newType = objTemp.GetType();

                            // check if the types are the same.  If they are, and they
                            // are COM objects, check their GUID's.  If they are different
                            // or Guid.Emtpy, assume the classes are different.
                            //
                            if (classesSame &&
#pragma warning disable SA1408 // Conditional expressions should declare precedence
                                (oldType != newType || oldType.IsCOMObject && newType.IsCOMObject))
#pragma warning restore SA1408 // Conditional expressions should declare precedence
                            {
                                classesSame = false;
                            }
                        }
                    }

                    if (!isSame)
                    {
                        EnsureDesignerEventService();

                        showEvents = showEvents && GetFlag(GotDesignerEventService);

                        SetStatusBox("", "");

                        ClearCachedProps();

                        // The default selected entry might still reference the previous selected
                        // objects. Set it to null to avoid leaks.
                        _peDefault = null;

                        if (value is null)
                        {
                            _currentObjects = Array.Empty<object>();
                        }
                        else
                        {
                            _currentObjects = (object[])value.Clone();
                        }

                        SinkPropertyNotifyEvents();
                        SetFlag(PropertiesChanged, true);

                        // Since we are changing the selection, we need to make sure that the
                        // keywords for the currently selected grid entry gets removed
                        if (_gridView != null)
                        {
                            // TypeResolutionService is needed to access the HelpKeyword. However,
                            // TypeResolutionService might be disposed when project is closing. We
                            // need swallow the exception in this case.
                            try
                            {
                                _gridView.RemoveSelectedEntryHelpAttributes();
                            }
                            catch (COMException) { }
                        }

                        if (_peMain != null)
                        {
                            _peMain.Dispose();
                        }

                        // throw away any extra component only tabs
                        if (!classesSame && !GetFlag(TabsChanging) && _selectedViewTab < _viewTabButtons.Length)
                        {
                            Type tabType = _selectedViewTab == -1 ? null : _viewTabs[_selectedViewTab].GetType();
                            ToolStripButton viewTabButton = null;
                            RefreshTabs(PropertyTabScope.Component);
                            EnableTabs();
                            if (tabType != null)
                            {
                                for (int i = 0; i < _viewTabs.Length; i++)
                                {
                                    if (_viewTabs[i].GetType() == tabType && _viewTabButtons[i].Visible)
                                    {
                                        viewTabButton = _viewTabButtons[i];
                                        break;
                                    }
                                }
                            }
                            SelectViewTabButtonDefault(viewTabButton);
                        }

                        // make sure we've also got events on all the objects
                        if (showEvents && _viewTabs != null && _viewTabs.Length > EVENTS && (_viewTabs[EVENTS] is EventsTab))
                        {
                            showEvents = _viewTabButtons[EVENTS].Visible;
                            object tempObj;
                            PropertyDescriptorCollection events;
                            Attribute[] attrs = new Attribute[BrowsableAttributes.Count];
                            BrowsableAttributes.CopyTo(attrs, 0);

                            Hashtable eventTypes = null;

                            if (_currentObjects.Length > 10)
                            {
                                eventTypes = new Hashtable();
                            }

                            for (int i = 0; i < _currentObjects.Length && showEvents; i++)
                            {
                                tempObj = _currentObjects[i];

                                if (tempObj is ICustomTypeDescriptor)
                                {
                                    tempObj = ((ICustomTypeDescriptor)tempObj).GetPropertyOwner(null);
                                }

                                Type objType = tempObj.GetType();

                                if (eventTypes != null && eventTypes.Contains(objType))
                                {
                                    continue;
                                }

                                // make sure these things are sited components as well
                                showEvents = showEvents && (tempObj is IComponent && ((IComponent)tempObj).Site != null);

                                // make sure we've also got events on all the objects
                                events = ((EventsTab)_viewTabs[EVENTS]).GetProperties(tempObj, attrs);
                                showEvents = showEvents && events != null && events.Count > 0;

                                if (showEvents && eventTypes != null)
                                {
                                    eventTypes[objType] = objType;
                                }
                            }
                        }
                        ShowEventsButton(showEvents && _currentObjects.Length > 0);
                        DisplayHotCommands();

                        if (_currentObjects.Length == 1)
                        {
                            EnablePropPageButton(_currentObjects[0]);
                        }
                        else
                        {
                            EnablePropPageButton(null);
                        }
                        OnSelectedObjectsChanged(EventArgs.Empty);
                    }

                    // This won't be a big perf problem, but it looks like we need to refresh
                    // even if we didn't change the selected objects.
                    if (!GetFlag(TabsChanging))
                    {
                        // ReInitTab means that we should set the tab back to what is used to be for a given designer.
                        // Basically, if you select an events tab for your designer and double click to go to code, it should
                        // be the events tab when you get back to the designer.
                        //
                        // so we set that bit when designers get switched, and makes sure we select and refresh that tab
                        // when we load.
                        //
                        if (_currentObjects.Length > 0 && GetFlag(ReInitTab))
                        {
                            object designerKey = ActiveDesigner;

                            // get the active designer, see if we've stashed away state for it.
                            //
                            if (designerKey != null && _designerSelections != null && _designerSelections.ContainsKey(designerKey.GetHashCode()))
                            {
                                int nButton = (int)_designerSelections[designerKey.GetHashCode()];

                                // yep, we know this one.  Make sure it's selected.
                                //
                                if (nButton < _viewTabs.Length && (nButton == PROPERTIES || _viewTabButtons[nButton].Visible))
                                {
                                    SelectViewTabButton(_viewTabButtons[nButton], true);
                                }
                            }
                            else
                            {
                                Refresh(false);
                            }
                            SetFlag(ReInitTab, false);
                        }
                        else
                        {
                            Refresh(true);
                        }

                        if (_currentObjects.Length > 0)
                        {
                            SaveTabSelection();
                        }
                    }
                }
                finally
                {
                    FreezePainting = false;
                }
            }

            get
            {
                if (_currentObjects is null)
                {
                    return Array.Empty<object>();
                }
                return (object[])_currentObjects.Clone();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PropertyTab SelectedTab
        {
            get
            {
                Debug.Assert(_selectedViewTab < _viewTabs.Length && _selectedViewTab >= 0, "Invalid tab selection!");
                return _viewTabs[_selectedViewTab];
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridItem SelectedGridItem
        {
            get
            {
                GridItem g = _gridView.SelectedGridEntry;
                if (g is null)
                {
                    return _peMain;
                }
                return g;
            }
            set
            {
                _gridView.SelectedGridEntry = (GridEntry)value;
            }
        }

        protected internal override bool ShowFocusCues
        {
            get
            {
                return true;
            }
        }

        public override ISite Site
        {
            get => base.Site;
            set
            {
                // Perf - the base class is possibly going to change the font via ambient properties service
                SuspendAllLayout(this);

                base.Site = value;
                _gridView.ServiceProvider = value;

                if (value is null)
                {
                    ActiveDesigner = null;
                }
                else
                {
                    ActiveDesigner = (IDesignerHost)value.GetService(typeof(IDesignerHost));
                }

                ResumeAllLayout(this, true);
            }
        }

        /// <summary>
        ///  Gets the value indicating whether the Property grid is sorted by categories.
        /// </summary>
        internal bool SortedByCategories
        {
            get
            {
                return (PropertySort & PropertySort.Categorized) != 0;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridLargeButtonsDesc))]
        [DefaultValue(false)]
        public bool LargeButtons
        {
            get
            {
                return _buttonType == LargeButtonSize;
            }
            set
            {
                if (value == (_buttonType == LargeButtonSize))
                {
                    return;
                }

                _buttonType = (value ? LargeButtonSize : NormalButtonSize);
                if (value)
                {
                    EnsureLargeButtons();
                    if (_imageList != null && _imageList[LargeButtonSize] != null)
                    {
                        _toolStrip.ImageScalingSize = _imageList[LargeButtonSize].ImageSize;
                    }
                }
                else
                {
                    if (_imageList != null && _imageList[NormalButtonSize] != null)
                    {
                        _toolStrip.ImageScalingSize = _imageList[NormalButtonSize].ImageSize;
                    }
                }

                _toolStrip.ImageList = _imageList[_buttonType];
                OnLayoutInternal(false);
                Invalidate();
                _toolStrip.Invalidate();
            }
        }

        /// <summary>
        ///  Gets the toolbar control accessibility object.
        /// </summary>
        internal AccessibleObject ToolbarAccessibleObject
        {
            get
            {
                return _toolStrip.AccessibilityObject;
            }
        }

        /// <summary>
        ///  Sets or gets the visiblity state of the toolStrip.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.PropertyGridToolbarVisibleDesc))]
        public virtual bool ToolbarVisible
        {
            get
            {
                return _toolbarVisible;
            }
            set
            {
                _toolbarVisible = value;

                _toolStrip.Visible = value;
                OnLayoutInternal(false);
                if (value)
                {
                    SetupToolbar(_viewTabsDirty);
                }
                Invalidate();
                _toolStrip.Invalidate();
            }
        }

        protected ToolStripRenderer ToolStripRenderer
        {
            get
            {
                if (_toolStrip != null)
                {
                    return _toolStrip.Renderer;
                }
                return null;
            }
            set
            {
                if (_toolStrip != null)
                {
                    _toolStrip.Renderer = value;
                }
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridViewBackColorDesc))]
        [DefaultValue(typeof(Color), "Window")]
        public Color ViewBackColor
        {
            get
            {
                return _gridView.BackColor;
            }
            set
            {
                _gridView.BackColor = value;
                _gridView.Invalidate();
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridViewForeColorDesc))]
        [DefaultValue(typeof(Color), "WindowText")]
        public Color ViewForeColor
        {
            get
            {
                return _gridView.ForeColor;
            }
            set
            {
                _gridView.ForeColor = value;
                _gridView.Invalidate();
            }
        }

        /// <summary>
        ///  Border color for the property grid view.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.PropertyGridViewBorderColorDesc))]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color ViewBorderColor
        {
            get
            {
                return _viewBorderColor;
            }
            set
            {
                if (_viewBorderColor != value)
                {
                    _viewBorderColor = value;
                    _gridView.Invalidate();
                }
            }
        }

        private int AddImage(Bitmap image)
        {
            if (image.RawFormat.Guid != ImageFormat.Icon.Guid)
            {
                image.MakeTransparent();
            }
            // Resize bitmap only if resizing is needed in order to avoid image distortion.
            if (DpiHelper.IsScalingRequired && (image.Size.Width != s_normalButtonSize.Width || image.Size.Height != s_normalButtonSize.Height))
            {
                image = DpiHelper.CreateResizedBitmap(image, s_normalButtonSize);
            }
            int result = _imageList[NormalButtonSize].Images.Count;
            _imageList[NormalButtonSize].Images.Add(image);
            return result;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyEventHandler KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyPressEventHandler KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event KeyEventHandler KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDown
        {
            add => base.MouseDown += value;
            remove => base.MouseDown -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseUp
        {
            add => base.MouseUp += value;
            remove => base.MouseUp -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseMove
        {
            add => base.MouseMove += value;
            remove => base.MouseMove -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseEnter
        {
            add => base.MouseEnter += value;
            remove => base.MouseEnter -= value;
        }

        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseLeave
        {
            add => base.MouseLeave += value;
            remove => base.MouseLeave -= value;
        }

        /// <summary> Event that is fired when a property value is modified.</summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.PropertyGridPropertyValueChangedDescr))]
        public event PropertyValueChangedEventHandler PropertyValueChanged
        {
            add => Events.AddHandler(EventPropertyValueChanged, value);
            remove => Events.RemoveHandler(EventPropertyValueChanged, value);
        }

        event ComponentRenameEventHandler IComPropertyBrowser.ComComponentNameChanged
        {
            add => Events.AddHandler(EventComComponentNameChanged, value);
            remove => Events.RemoveHandler(EventComComponentNameChanged, value);
        }

        /// <summary> Event that is fired when the current view tab is changed, such as changing from Properties to Events</summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.PropertyGridPropertyTabchangedDescr))]
        public event PropertyTabChangedEventHandler PropertyTabChanged
        {
            add => Events.AddHandler(EventPropertyTabChanged, value);
            remove => Events.RemoveHandler(EventPropertyTabChanged, value);
        }

        /// <summary> Event that is fired when the sort mode is changed.</summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.PropertyGridPropertySortChangedDescr))]
        public event EventHandler PropertySortChanged
        {
            add => Events.AddHandler(EventPropertySortChanged, value);
            remove => Events.RemoveHandler(EventPropertySortChanged, value);
        }

        /// <summary> Event that is fired when the selected GridItem is changed</summary>
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.PropertyGridSelectedGridItemChangedDescr))]
        public event SelectedGridItemChangedEventHandler SelectedGridItemChanged
        {
            add => Events.AddHandler(EventSelectedGridItemChanged, value);
            remove => Events.RemoveHandler(EventSelectedGridItemChanged, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.PropertyGridSelectedObjectsChangedDescr))]
        public event EventHandler SelectedObjectsChanged
        {
            add => Events.AddHandler(EventSelectedObjectsChanged, value);
            remove => Events.RemoveHandler(EventSelectedObjectsChanged, value);
        }

        internal void AddTab(Type tabType, PropertyTabScope scope)
        {
            AddRefTab(tabType, null, scope, true);
        }

        internal void AddRefTab(Type tabType, object component, PropertyTabScope type, bool setupToolbar)
        {
            PropertyTab tab = null;
            int tabIndex = -1;

            if (_viewTabs != null)
            {
                // check to see if we've already got a tab of this type
                for (int i = 0; i < _viewTabs.Length; i++)
                {
                    Debug.Assert(_viewTabs[i] != null, "Null item in tab array!");
                    if (tabType == _viewTabs[i].GetType())
                    {
                        tab = _viewTabs[i];
                        tabIndex = i;
                        break;
                    }
                }
            }
            else
            {
                tabIndex = 0;
            }

            if (tab is null)
            {
                // the tabs need service providers. The one we hold onto is not good enough,
                // so try to get the one off of the component's site.
                IDesignerHost host = null;
                if (component != null && component is IComponent && ((IComponent)component).Site != null)
                {
                    host = (IDesignerHost)((IComponent)component).Site.GetService(typeof(IDesignerHost));
                }

                try
                {
                    tab = CreateTab(tabType, host);
                }
                catch (Exception)
                {
                    return;
                }

                // add it at the end of the array
                if (_viewTabs != null)
                {
                    tabIndex = _viewTabs.Length;

                    // find the insertion position...special case for event's and properties
                    if (tabType == DefaultTabType)
                    {
                        tabIndex = PROPERTIES;
                    }
                    else if (typeof(EventsTab).IsAssignableFrom(tabType))
                    {
                        tabIndex = EVENTS;
                    }
                    else
                    {
                        // order tabs alphabetically, we've always got a property tab, so
                        // start after that
                        for (int i = 1; i < _viewTabs.Length; i++)
                        {
                            // skip the event tab
                            if (_viewTabs[i] is EventsTab)
                            {
                                continue;
                            }

                            if (string.Compare(tab.TabName, _viewTabs[i].TabName, false, CultureInfo.InvariantCulture) < 0)
                            {
                                tabIndex = i;
                                break;
                            }
                        }
                    }
                }

                // now add the tab to the tabs array
                PropertyTab[] newTabs = new PropertyTab[_viewTabs.Length + 1];
                Array.Copy(_viewTabs, 0, newTabs, 0, tabIndex);
                Array.Copy(_viewTabs, tabIndex, newTabs, tabIndex + 1, _viewTabs.Length - tabIndex);
                newTabs[tabIndex] = tab;
                _viewTabs = newTabs;

                _viewTabsDirty = true;

                PropertyTabScope[] newTabScopes = new PropertyTabScope[_viewTabScopes.Length + 1];
                Array.Copy(_viewTabScopes, 0, newTabScopes, 0, tabIndex);
                Array.Copy(_viewTabScopes, tabIndex, newTabScopes, tabIndex + 1, _viewTabScopes.Length - tabIndex);
                newTabScopes[tabIndex] = type;
                _viewTabScopes = newTabScopes;

                Debug.Assert(_viewTabs != null, "Tab array destroyed!");
            }

            if (tab != null && component != null)
            {
                try
                {
                    object[] tabComps = tab.Components;
                    int oldArraySize = tabComps is null ? 0 : tabComps.Length;

                    object[] newComps = new object[oldArraySize + 1];
                    if (oldArraySize > 0)
                    {
                        Array.Copy(tabComps, newComps, oldArraySize);
                    }
                    newComps[oldArraySize] = component;
                    tab.Components = newComps;
                }
                catch (Exception e)
                {
                    Debug.Fail("Bad tab. We're going to remove it.", e.ToString());
                    RemoveTab(tabIndex, false);
                }
            }

            if (setupToolbar)
            {
                SetupToolbar();
                ShowEventsButton(false);
            }
        }

        /// <summary> Collapses all the nodes in the PropertyGrid</summary>
        public void CollapseAllGridItems()
        {
            _gridView.RecursivelyExpand(_peMain, false, false, -1);
        }

        private void ClearCachedProps()
        {
            if (_viewTabProps != null)
            {
                _viewTabProps.Clear();
            }
        }

        internal void ClearValueCaches()
        {
            if (_peMain != null)
            {
                _peMain.ClearCachedValues();
            }
        }

        /// <summary>
        ///  Clears the tabs of the given scope or smaller.
        ///  tabScope must be PropertyTabScope.Component or PropertyTabScope.Document.
        /// </summary>
        internal void ClearTabs(PropertyTabScope tabScope)
        {
            if (tabScope < PropertyTabScope.Document)
            {
                throw new ArgumentException(SR.PropertyGridTabScope);
            }
            RemoveTabs(tabScope, true);
        }

#if DEBUG
        internal bool inGridViewCreate;
#endif

        /// <summary>
        ///  Constructs the new instance of the accessibility object for current PropertyGrid control.
        /// </summary>
        /// <returns></returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new PropertyGridAccessibleObject(this);
        }

        private /*protected virtual*/ PropertyGridView CreateGridView(IServiceProvider sp)
        {
#if DEBUG
            try
            {
                inGridViewCreate = true;
#endif
                return new PropertyGridView(sp, this);
#if DEBUG
            }
            finally
            {
                inGridViewCreate = false;
            }
#endif
        }

        private ToolStripSeparator CreateSeparatorButton()
        {
            ToolStripSeparator button = new ToolStripSeparator();
            return button;
        }

        protected virtual PropertyTab CreatePropertyTab(Type tabType)
        {
            return null;
        }

        private PropertyTab CreateTab(Type tabType, IDesignerHost host)
        {
            PropertyTab tab = CreatePropertyTab(tabType);

            if (tab is null)
            {
                ConstructorInfo constructor = tabType.GetConstructor(new Type[] { typeof(IServiceProvider) });
                object param = null;
                if (constructor is null)
                {
                    // try a IDesignerHost ctor
                    constructor = tabType.GetConstructor(new Type[] { typeof(IDesignerHost) });

                    if (constructor != null)
                    {
                        param = host;
                    }
                }
                else
                {
                    param = Site;
                }

                if (param != null && constructor != null)
                {
                    tab = (PropertyTab)constructor.Invoke(new object[] { param });
                }
                else
                {
                    // just call the default ctor
                    //
                    tab = (PropertyTab)Activator.CreateInstance(tabType);
                }
            }

            Debug.Assert(tab != null, "Failed to create tab!");

            if (tab != null)
            {
                // ensure it's a valid tab
                Bitmap bitmap = tab.Bitmap;

                if (bitmap is null)
                {
                    throw new ArgumentException(string.Format(SR.PropertyGridNoBitmap, tab.GetType().FullName));
                }

                Size size = bitmap.Size;
                if (size.Width != 16 || size.Height != 16)
                {
                    // resize it to 16x16 if it isn't already.
                    //
                    bitmap = new Bitmap(bitmap, new Size(16, 16));
                }

                string name = tab.TabName;
                if (name is null || name.Length == 0)
                {
                    throw new ArgumentException(string.Format(SR.PropertyGridTabName, tab.GetType().FullName));
                }

                // we're good to go!
            }
            return tab;
        }

        private ToolStripButton CreatePushButton(string toolTipText, int imageIndex, EventHandler eventHandler, bool useCheckButtonRole = false)
        {
            ToolStripButton button = new ToolStripButton
            {
                Text = toolTipText,
                AutoToolTip = true,
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                ImageIndex = imageIndex
            };
            button.Click += eventHandler;
            button.ImageScaling = ToolStripItemImageScaling.SizeToFit;

            if (useCheckButtonRole)
            {
                button.AccessibleRole = AccessibleRole.CheckButton;
            }

            return button;
        }

        internal void DumpPropsToConsole()
        {
            _gridView.DumpPropsToConsole(_peMain, "");
        }

        private void DisplayHotCommands()
        {
            bool hotCommandsDisplayed = _hotcommands.Visible;

            IComponent component = null;
            DesignerVerb[] verbs = null;

            // We favor the menu command service, since it can give us
            // verbs.  If we fail that, we will go straight to the
            // designer.
            //
            if (_currentObjects != null && _currentObjects.Length > 0)
            {
                for (int i = 0; i < _currentObjects.Length; i++)
                {
                    object obj = GetUnwrappedObject(i);
                    if (obj is IComponent)
                    {
                        component = (IComponent)obj;
                        break;
                    }
                }

                if (component != null)
                {
                    ISite site = component.Site;

                    if (site != null)
                    {
                        IMenuCommandService mcs = (IMenuCommandService)site.GetService(typeof(IMenuCommandService));
                        if (mcs != null)
                        {
                            // Got the menu command service.  Let it deal with the set of verbs for
                            // this component.
                            //
                            verbs = new DesignerVerb[mcs.Verbs.Count];
                            mcs.Verbs.CopyTo(verbs, 0);
                        }
                        else
                        {
                            // No menu command service.  Go straight to the component's designer.  We
                            // can only do this if the Object count is 1, because desginers do not
                            // support verbs across a multi-selection.
                            //
                            if (_currentObjects.Length == 1 && GetUnwrappedObject(0) is IComponent)
                            {
                                IDesignerHost designerHost = (IDesignerHost)site.GetService(typeof(IDesignerHost));
                                if (designerHost != null)
                                {
                                    IDesigner designer = designerHost.GetDesigner(component);
                                    if (designer != null)
                                    {
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
            if (!DesignMode)
            {
                if (verbs != null && verbs.Length > 0)
                {
                    _hotcommands.SetVerbs(component, verbs);
                }
                else
                {
                    _hotcommands.SetVerbs(null, null);
                }

                if (hotCommandsDisplayed != _hotcommands.Visible)
                {
                    OnLayoutInternal(false);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unhook IDesignerEventService.ActiveDesignerChanged event
                //
                if (GetFlag(GotDesignerEventService))
                {
                    Debug.Assert(_designerEventService != null, "GetFlag(GotDesignerEventService) inconsistent with designerEventService == null");
                    if (_designerEventService != null)
                    {
                        _designerEventService.ActiveDesignerChanged -= new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                    }
                    _designerEventService = null;
                    SetFlag(GotDesignerEventService, false);
                }
                ActiveDesigner = null;

                if (_viewTabs != null)
                {
                    for (int i = 0; i < _viewTabs.Length; i++)
                    {
                        _viewTabs[i].Dispose();
                    }
                    _viewTabs = null;
                }

                if (_imageList != null)
                {
                    for (int i = 0; i < _imageList.Length; i++)
                    {
                        if (_imageList[i] != null)
                        {
                            _imageList[i].Dispose();
                        }
                    }
                    _imageList = null;
                }

                if (_bmpAlpha != null)
                {
                    _bmpAlpha.Dispose();
                    _bmpAlpha = null;
                }

                if (_bmpCategory != null)
                {
                    _bmpCategory.Dispose();
                    _bmpCategory = null;
                }

                if (_bmpPropPage != null)
                {
                    _bmpPropPage.Dispose();
                    _bmpPropPage = null;
                }

                if (_peMain != null)
                {
                    _peMain.Dispose();
                    _peMain = null;
                }

                if (_currentObjects != null)
                {
                    _currentObjects = null;
                    SinkPropertyNotifyEvents();
                }

                ClearCachedProps();
                _currentPropEntries = null;
            }

            base.Dispose(disposing);
        }

        private void DividerDraw(int y)
        {
            if (y == -1)
            {
                return;
            }

            Rectangle rectangle = _gridView.Bounds;
            rectangle.Y = y - s_cyDivider;
            rectangle.Height = s_cyDivider;

            DrawXorBar(this, rectangle);
        }

        private SnappableControl DividerInside(int x, int y)
        {
            int useGrid = -1;

            if (_hotcommands.Visible)
            {
                Point locDoc = _hotcommands.Location;
                if (y >= (locDoc.Y - s_cyDivider) &&
                    y <= (locDoc.Y + 1))
                {
                    return _hotcommands;
                }
                useGrid = 0;
            }

            if (_doccomment.Visible)
            {
                Point locDoc = _doccomment.Location;
                if (y >= (locDoc.Y - s_cyDivider) &&
                    y <= (locDoc.Y + 1))
                {
                    return _doccomment;
                }

                if (useGrid == -1)
                {
                    useGrid = 1;
                }
            }

            // also the bottom line of the grid
            if (useGrid != -1)
            {
                int gridTop = _gridView.Location.Y;
                int gridBottom = gridTop + _gridView.Size.Height;

                if (Math.Abs(gridBottom - y) <= 1 && y > gridTop)
                {
                    switch (useGrid)
                    {
                        case 0:
                            return _hotcommands;
                        case 1:
                            return _doccomment;
                    }
                }
            }
            return null;
        }

        private int DividerLimitHigh(SnappableControl target)
        {
            int high = _gridView.Location.Y + MinGridHeight;
            if (target == _doccomment && _hotcommands.Visible)
            {
                high += _hotcommands.Size.Height + 2;
            }

            return high;
        }

        private int DividerLimitMove(SnappableControl target, int y)
        {
            Rectangle rectTarget = target.Bounds;

            int cyNew = y;

            // make sure we're not going to make ourselves zero height -- make 15 the min size
            cyNew = Math.Min((rectTarget.Y + rectTarget.Height - 15), cyNew);

            // make sure we're not going to make ourselves cover up the grid
            cyNew = Math.Max(DividerLimitHigh(target), cyNew);

            // just return what we got here
            return (cyNew);
        }

        private static void DrawXorBar(Control ctlDrawTo, Rectangle rcFrame)
        {
            Rectangle rc = ctlDrawTo.RectangleToScreen(rcFrame);

            if (rc.Width < rc.Height)
            {
                for (int i = 0; i < rc.Width; i++)
                {
                    ControlPaint.DrawReversibleLine(new Point(rc.X + i, rc.Y), new Point(rc.X + i, rc.Y + rc.Height), ctlDrawTo.BackColor);
                }
            }
            else
            {
                for (int i = 0; i < rc.Height; i++)
                {
                    ControlPaint.DrawReversibleLine(new Point(rc.X, rc.Y + i), new Point(rc.X + rc.Width, rc.Y + i), ctlDrawTo.BackColor);
                }
            }
        }

        void IComPropertyBrowser.DropDownDone()
        {
            GetPropertyGridView().DropDownDone();
        }

        private bool EnablePropPageButton(object obj)
        {
            if (obj is null)
            {
                _btnViewPropertyPages.Enabled = false;
                return false;
            }

            IUIService uiSvc = (IUIService)GetService(typeof(IUIService));
            bool enable = false;

            if (uiSvc != null)
            {
                enable = uiSvc.CanShowComponentEditor(obj);
            }
            else
            {
                enable = (TypeDescriptor.GetEditor(obj, typeof(ComponentEditor)) != null);
            }

            _btnViewPropertyPages.Enabled = enable;
            return enable;
        }

        // walk through the current tabs to see if they're all valid for this Object
        private void EnableTabs()
        {
            if (_currentObjects != null)
            {
                // make sure our toolbars is okay
                SetupToolbar();

                Debug.Assert(_viewTabs != null, "Invalid tab array");
                Debug.Assert(_viewTabs.Length == _viewTabScopes.Length && _viewTabScopes.Length == _viewTabButtons.Length, "Uh oh, tab arrays aren't all the same length! tabs=" + _viewTabs.Length.ToString(CultureInfo.InvariantCulture) + ", scopes=" + _viewTabScopes.Length.ToString(CultureInfo.InvariantCulture) + ", buttons=" + _viewTabButtons.Length.ToString(CultureInfo.InvariantCulture));

                // skip the property tab since it's always valid
                for (int i = 1; i < _viewTabs.Length; i++)
                {
                    Debug.Assert(_viewTabs[i] != null, "Invalid tab array entry");

                    bool canExtend = true;
                    // make sure the tab is valid for all objects
                    for (int j = 0; j < _currentObjects.Length; j++)
                    {
                        try
                        {
                            if (!_viewTabs[i].CanExtend(GetUnwrappedObject(j)))
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

                    if (canExtend != _viewTabButtons[i].Visible)
                    {
                        _viewTabButtons[i].Visible = canExtend;
                        if (!canExtend && i == _selectedViewTab)
                        {
                            SelectViewTabButton(_viewTabButtons[PROPERTIES], true);
                        }
                    }
                }
            }
        }

        private void EnsureDesignerEventService()
        {
            if (GetFlag(GotDesignerEventService))
            {
                return;
            }
            _designerEventService = (IDesignerEventService)GetService(typeof(IDesignerEventService));
            if (_designerEventService != null)
            {
                SetFlag(GotDesignerEventService, true);
                _designerEventService.ActiveDesignerChanged += new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                OnActiveDesignerChanged(null, new ActiveDesignerEventArgs(null, _designerEventService.ActiveDesigner));
            }
        }

        private void EnsureLargeButtons()
        {
            if (_imageList[LargeButtonSize] is null)
            {
                _imageList[LargeButtonSize] = new ImageList
                {
                    ImageSize = s_largeButtonSize
                };

                if (DpiHelper.IsScalingRequired)
                {
                    AddLargeImage(_bmpAlpha);
                    AddLargeImage(_bmpCategory);

                    foreach (PropertyTab tab in _viewTabs)
                    {
                        AddLargeImage(tab.Bitmap);
                    }

                    AddLargeImage(_bmpPropPage);
                }
                else
                {
                    ImageList.ImageCollection images = _imageList[NormalButtonSize].Images;

                    for (int i = 0; i < images.Count; i++)
                    {
                        if (images[i] is Bitmap)
                        {
                            _imageList[LargeButtonSize].Images.Add(new Bitmap((Bitmap)images[i], s_largeButtonSize.Width, s_largeButtonSize.Height));
                        }
                    }
                }
            }
        }

        // this method should be called only inside a if (DpiHelper.IsScalingRequired) clause
        private void AddLargeImage(Bitmap originalBitmap)
        {
            if (originalBitmap is null)
            {
                return;
            }

            Bitmap largeBitmap = null;
            try
            {
                Bitmap transparentBitmap = new Bitmap(originalBitmap);
                largeBitmap = DpiHelper.CreateResizedBitmap(transparentBitmap, s_largeButtonSize);
                transparentBitmap.Dispose();

                _imageList[LargeButtonSize].Images.Add(largeBitmap);
            }
            catch (Exception ex)
            {
                Debug.Fail("Failed to add a large property grid toolstrip button, " + ex.ToString());
            }
        }

        bool IComPropertyBrowser.EnsurePendingChangesCommitted()
        {
            // The commits sometimes cause transactions to open
            // and close, which will cause refreshes, which we want to ignore.

            try
            {
                if (_designerHost != null)
                {
                    _designerHost.TransactionOpened -= new EventHandler(OnTransactionOpened);
                    _designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                }

                return GetPropertyGridView().EnsurePendingChangesCommitted();
            }
            finally
            {
                if (_designerHost != null)
                {
                    _designerHost.TransactionOpened += new EventHandler(OnTransactionOpened);
                    _designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                }
            }
        }

        public void ExpandAllGridItems()
        {
            _gridView.RecursivelyExpand(_peMain, false, true, PropertyGridView.MaxRecurseExpand);
        }

        private static Type[] GetCommonTabs(object[] objs, PropertyTabScope tabScope)
        {
            if (objs is null || objs.Length == 0)
            {
                return Array.Empty<Type>();
            }

            Type[] tabTypes = new Type[5];
            int types = 0;
            int i, j, k;
            PropertyTabAttribute tabAttr = (PropertyTabAttribute)TypeDescriptor.GetAttributes(objs[0])[typeof(PropertyTabAttribute)];

            if (tabAttr is null)
            {
                return Array.Empty<Type>();
            }

            // filter out all the types of the current scope
            for (i = 0; i < tabAttr.TabScopes.Length; i++)
            {
                PropertyTabScope item = tabAttr.TabScopes[i];

                if (item == tabScope)
                {
                    if (types == tabTypes.Length)
                    {
                        Type[] newTabs = new Type[types * 2];
                        Array.Copy(tabTypes, 0, newTabs, 0, types);
                        tabTypes = newTabs;
                    }
                    tabTypes[types++] = tabAttr.TabClasses[i];
                }
            }

            if (types == 0)
            {
                return Array.Empty<Type>();
            }

            bool found;

            for (i = 1; i < objs.Length && types > 0; i++)
            {
                // get the tab attribute
                tabAttr = (PropertyTabAttribute)TypeDescriptor.GetAttributes(objs[i])[typeof(PropertyTabAttribute)];

                if (tabAttr is null)
                {
                    // if this guy has no tabs at all, we can fail right now
                    return Array.Empty<Type>();
                }

                // make sure this guy has all the items in the array,
                // if not, remove the items he doesn't have
                for (j = 0; j < types; j++)
                {
                    found = false;
                    for (k = 0; k < tabAttr.TabClasses.Length; k++)
                    {
                        if (tabAttr.TabClasses[k] == tabTypes[j])
                        {
                            found = true;
                            break;
                        }
                    }

                    // if we didn't find an item, remove it from the list
                    if (!found)
                    {
                        // swap in with the last item and decrement
                        tabTypes[j] = tabTypes[types - 1];
                        tabTypes[types - 1] = null;
                        types--;

                        // recheck this item since we'll be ending sooner
                        j--;
                    }
                }
            }

            Type[] returnTypes = new Type[types];
            if (types > 0)
            {
                Array.Copy(tabTypes, 0, returnTypes, 0, types);
            }
            return returnTypes;
        }

        internal GridEntry GetDefaultGridEntry()
        {
            if (_peDefault is null && _currentPropEntries != null)
            {
                _peDefault = (GridEntry)_currentPropEntries[0];
            }
            return _peDefault;
        }

        /// <summary>
        ///  Gets the element from point.
        /// </summary>
        /// <param name="point">The point where to search the element.</param>
        /// <returns>The element found in the current point.</returns>
        internal Control GetElementFromPoint(Point point)
        {
            if (ToolbarAccessibleObject.Bounds.Contains(point))
            {
                return _toolStrip;
            }

            if (GridViewAccessibleObject.Bounds.Contains(point))
            {
                return _gridView;
            }

            if (HotCommandsAccessibleObject.Bounds.Contains(point))
            {
                return _hotcommands;
            }

            if (HelpAccessibleObject.Bounds.Contains(point))
            {
                return _doccomment;
            }

            return null;
        }

        private object GetUnwrappedObject(int index)
        {
            if (_currentObjects is null || index < 0 || index > _currentObjects.Length)
            {
                return null;
            }

            object obj = _currentObjects[index];
            if (obj is ICustomTypeDescriptor)
            {
                obj = ((ICustomTypeDescriptor)obj).GetPropertyOwner(null);
            }
            return obj;
        }

        internal GridEntryCollection GetPropEntries()
        {
            if (_currentPropEntries is null)
            {
                UpdateSelection();
            }
            SetFlag(PropertiesChanged, false);
            return _currentPropEntries;
        }

        private PropertyGridView GetPropertyGridView()
        {
            return _gridView;
        }

        void IComPropertyBrowser.HandleF4()
        {
            if (_gridView.ContainsFocus)
            {
                return;
            }

            if (ActiveControl != _gridView)
            {
                SetActiveControl(_gridView);
            }
            _gridView.Focus();
        }

        internal bool HavePropEntriesChanged()
        {
            return GetFlag(PropertiesChanged);
        }

        void IComPropertyBrowser.LoadState(RegistryKey optRoot)
        {
            if (optRoot != null)
            {
                object val = optRoot.GetValue("PbrsAlpha", "0");

                if (val != null && val.ToString().Equals("1"))
                {
                    PropertySort = PropertySort.Alphabetical;
                }
                else
                {
                    PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
                }

                val = optRoot.GetValue("PbrsShowDesc", "1");
                HelpVisible = (val != null && val.ToString().Equals("1"));

                val = optRoot.GetValue("PbrsShowCommands", "0");
                CommandsVisibleIfAvailable = (val != null && val.ToString().Equals("1"));

                val = optRoot.GetValue("PbrsDescHeightRatio", "-1");

                bool update = false;
                if (val is string)
                {
                    int ratio = int.Parse((string)val, CultureInfo.InvariantCulture);
                    if (ratio > 0)
                    {
                        _dcSizeRatio = ratio;
                        update = true;
                    }
                }

                val = optRoot.GetValue("PbrsHotCommandHeightRatio", "-1");
                if (val is string)
                {
                    int ratio = int.Parse((string)val, CultureInfo.InvariantCulture);
                    if (ratio > 0)
                    {
                        _dcSizeRatio = ratio;
                        update = true;
                    }
                }

                if (update)
                {
                    OnLayoutInternal(false);
                }
            }
            else
            {
                // apply the same defaults from above.
                //
                PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
                HelpVisible = true;
                CommandsVisibleIfAvailable = false;
            }
        }

        // when the active document is changed, check all the components so see if they
        // are offering up any new tabs
        private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs e)
        {
            if (e.OldDesigner != null && e.OldDesigner == _designerHost)
            {
                ActiveDesigner = null;
            }

            if (e.NewDesigner != null && e.NewDesigner != _designerHost)
            {
                ActiveDesigner = e.NewDesigner;
            }
        }

        /// <summary>
        ///  Called when a property on an Ole32 Object changes.
        ///  See IPropertyNotifySink::OnChanged
        /// </summary>
        HRESULT Ole32.IPropertyNotifySink.OnChanged(Ole32.DispatchID dispID)
        {
            // we don't want the grid's own property sets doing this, but if we're getting
            // an OnChanged that isn't the DispID of the property we're currently changing,
            // we need to cause a refresh.
            bool fullRefresh = false;
            if (_gridView.SelectedGridEntry is PropertyDescriptorGridEntry selectedEntry && selectedEntry.PropertyDescriptor != null && selectedEntry.PropertyDescriptor.Attributes != null)
            {
                // fish out the DispIdAttribute which will tell us the DispId of the
                // property that we're changing.
                DispIdAttribute dispIdAttr = (DispIdAttribute)selectedEntry.PropertyDescriptor.Attributes[(typeof(DispIdAttribute))];
                if (dispIdAttr != null && !dispIdAttr.IsDefaultAttribute())
                {
                    fullRefresh = (dispID != (Ole32.DispatchID)dispIdAttr.Value);
                }
            }

            if (!GetFlag(RefreshingProperties))
            {
                if (!_gridView.GetInPropertySet() || fullRefresh)
                {
                    Refresh(fullRefresh);
                }

                // this is so changes to names of native
                // objects will be reflected in the combo box
                object obj = GetUnwrappedObject(0);
                if (ComNativeDescriptor.Instance.IsNameDispId(obj, dispID) || dispID == Ole32.DispatchID.Name)
                {
                    OnComComponentNameChanged(new ComponentRenameEventArgs(obj, null, TypeDescriptor.GetClassName(obj)));
                }
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  We forward messages from several of our children
        ///  to our mouse move so we can put up the spliter over their borders
        /// </summary>
        private void OnChildMouseMove(object sender, MouseEventArgs me)
        {
            Point newPt = Point.Empty;
            if (ShouldForwardChildMouseMessage((Control)sender, me, ref newPt))
            {
                // forward the message
                OnMouseMove(new MouseEventArgs(me.Button, me.Clicks, newPt.X, newPt.Y, me.Delta));
                return;
            }
        }

        /// <summary>
        ///  We forward messages from several of our children
        ///  to our mouse move so we can put up the spliter over their borders
        /// </summary>
        private void OnChildMouseDown(object sender, MouseEventArgs me)
        {
            Point newPt = Point.Empty;

            if (ShouldForwardChildMouseMessage((Control)sender, me, ref newPt))
            {
                // forward the message
                OnMouseDown(new MouseEventArgs(me.Button, me.Clicks, newPt.X, newPt.Y, me.Delta));
                return;
            }
        }

        private void OnComponentAdd(object sender, ComponentEventArgs e)
        {
            PropertyTabAttribute attribute = (PropertyTabAttribute)TypeDescriptor.GetAttributes(e.Component.GetType())[typeof(PropertyTabAttribute)];

            if (attribute is null)
            {
                return;
            }

            // add all the document items
            for (int i = 0; i < attribute.TabClasses.Length; i++)
            {
                if (attribute.TabScopes[i] == PropertyTabScope.Document)
                {
                    AddRefTab(attribute.TabClasses[i], e.Component, PropertyTabScope.Document, true);
                }
            }
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            bool batchMode = GetFlag(BatchMode);
            if (batchMode || GetFlag(InternalChange) || _gridView.GetInPropertySet() ||
               (_currentObjects is null) || (_currentObjects.Length == 0))
            {
                if (batchMode && !_gridView.GetInPropertySet())
                {
                    SetFlag(BatchModeChange, true);
                }
                return;
            }

            int objectCount = _currentObjects.Length;
            for (int i = 0; i < objectCount; i++)
            {
                if (_currentObjects[i] == e.Component)
                {
                    Refresh(false);
                    break;
                }
            }
        }

        private void OnComponentRemove(object sender, ComponentEventArgs e)
        {
            PropertyTabAttribute attribute = (PropertyTabAttribute)TypeDescriptor.GetAttributes(e.Component.GetType())[typeof(PropertyTabAttribute)];

            if (attribute is null)
            {
                return;
            }

            // remove all the document items
            for (int i = 0; i < attribute.TabClasses.Length; i++)
            {
                if (attribute.TabScopes[i] == PropertyTabScope.Document)
                {
                    ReleaseTab(attribute.TabClasses[i], e.Component);
                }
            }

            for (int i = 0; i < _currentObjects.Length; i++)
            {
                if (e.Component == _currentObjects[i])
                {
                    object[] newObjects = new object[_currentObjects.Length - 1];
                    Array.Copy(_currentObjects, 0, newObjects, 0, i);
                    if (i < newObjects.Length)
                    {
                        // Fixed for .NET Framework 4.0
                        Array.Copy(_currentObjects, i + 1, newObjects, i, newObjects.Length - i);
                    }

                    if (!GetFlag(BatchMode))
                    {
                        SelectedObjects = newObjects;
                    }
                    else
                    {
                        // otherwise, just dump the selection
                        //
                        _gridView.ClearProps();
                        _currentObjects = newObjects;
                        SetFlag(FullRefreshAfterBatch, true);
                    }
                }
            }

            SetupToolbar();
        }

        //
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Refresh();
        }

        //
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Refresh();
        }

        internal void OnGridViewMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }

        //
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            OnLayoutInternal(false);
            TypeDescriptor.Refreshed += new RefreshEventHandler(OnTypeDescriptorRefreshed);
            if (_currentObjects != null && _currentObjects.Length > 0)
            {
                Refresh(true);
            }
        }

        //
        protected override void OnHandleDestroyed(EventArgs e)
        {
            TypeDescriptor.Refreshed -= new RefreshEventHandler(OnTypeDescriptorRefreshed);
            base.OnHandleDestroyed(e);
        }

        //
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (ActiveControl is null)
            {
                SetActiveControl(_gridView);
            }
            else
            {
                // sometimes the edit is still the active control
                // when it's hidden or disabled...
                if (!ActiveControl.Focus())
                {
                    SetActiveControl(_gridView);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy)
        {
            int sx = (int)Math.Round(Left * dx);
            int sy = (int)Math.Round(Top * dy);
            int sw = Width;
            sw = (int)Math.Round((Left + Width) * dx - sx);
            int sh = Height;
            sh = (int)Math.Round((Top + Height) * dy - sy);
            SetBounds(sx, sy, sw, sh, BoundsSpecified.All);
        }

        private void OnLayoutInternal(bool dividerOnly)
        {
            if (!IsHandleCreated || !Visible)
            {
                return;
            }

            try
            {
                FreezePainting = true;

                if (!dividerOnly)
                {
                    // no toolbar or doc comment or commands, just
                    // fill the whole thing with the grid
                    if (!_toolStrip.Visible && !_doccomment.Visible && !_hotcommands.Visible)
                    {
                        _gridView.Location = new Point(0, 0);
                        _gridView.Size = Size;
                        return;
                    }

                    if (_toolStrip.Visible)
                    {
                        int toolStripWidth = Width;
                        int toolStripHeight = ((LargeButtons) ? s_largeButtonSize : s_normalButtonSize).Height + _toolStripButtonPaddingY;
                        Rectangle toolStripBounds = new Rectangle(0, 1, toolStripWidth, toolStripHeight);
                        _toolStrip.Bounds = toolStripBounds;

                        int oldY = _gridView.Location.Y;
                        _gridView.Location = new Point(0, _toolStrip.Height + _toolStrip.Top);
                    }
                    else
                    {
                        _gridView.Location = new Point(0, 0);
                    }
                }

                // now work up from the bottom
                int endSize = Size.Height;

                if (endSize < MinGridHeight)
                {
                    return;
                }

                int maxSpace = endSize - (_gridView.Location.Y + MinGridHeight);
                int height;

                // if we're just moving the divider, set the requested heights
                int dcRequestedHeight = 0;
                int hcRequestedHeight = 0;
                int dcOptHeight = 0;
                int hcOptHeight = 0;

                if (dividerOnly)
                {
                    dcRequestedHeight = _doccomment.Visible ? _doccomment.Size.Height : 0;
                    hcRequestedHeight = _hotcommands.Visible ? _hotcommands.Size.Height : 0;
                }
                else
                {
                    if (_doccomment.Visible)
                    {
                        dcOptHeight = _doccomment.GetOptimalHeight(Size.Width - s_cyDivider);
                        if (_doccomment.userSized)
                        {
                            dcRequestedHeight = _doccomment.Size.Height;
                        }
                        else if (_dcSizeRatio != -1)
                        {
                            dcRequestedHeight = (Height * _dcSizeRatio) / 100;
                        }
                        else
                        {
                            dcRequestedHeight = dcOptHeight;
                        }
                    }

                    if (_hotcommands.Visible)
                    {
                        hcOptHeight = _hotcommands.GetOptimalHeight(Size.Width - s_cyDivider);
                        if (_hotcommands.userSized)
                        {
                            hcRequestedHeight = _hotcommands.Size.Height;
                        }
                        else if (_hcSizeRatio != -1)
                        {
                            hcRequestedHeight = (Height * _hcSizeRatio) / 100;
                        }
                        else
                        {
                            hcRequestedHeight = hcOptHeight;
                        }
                    }
                }

                // place the help comment window
                if (dcRequestedHeight > 0)
                {
                    maxSpace -= s_cyDivider;

                    if (hcRequestedHeight == 0 || (dcRequestedHeight + hcRequestedHeight) < maxSpace)
                    {
                        // full size
                        height = Math.Min(dcRequestedHeight, maxSpace);
                    }
                    else if (hcRequestedHeight > 0 && hcRequestedHeight < maxSpace)
                    {
                        // give most of the space to the hot commands
                        height = maxSpace - hcRequestedHeight;
                    }
                    else
                    {
                        // split the difference
                        height = Math.Min(dcRequestedHeight, maxSpace / 2 - 1);
                    }

                    height = Math.Max(height, s_cyDivider * 2);

                    _doccomment.SetBounds(0, endSize - height, Size.Width, height);

                    // if we've modified the height to less than the optimal, clear the userSized item
                    if (height <= dcOptHeight && height < dcRequestedHeight)
                    {
                        _doccomment.userSized = false;
                    }
                    else if (_dcSizeRatio != -1 || _doccomment.userSized)
                    {
                        _dcSizeRatio = (_doccomment.Height * 100) / Height;
                    }

                    _doccomment.Invalidate();
                    endSize = _doccomment.Location.Y - s_cyDivider;
                    maxSpace -= height;
                }

                // place the hot commands
                if (hcRequestedHeight > 0)
                {
                    maxSpace -= s_cyDivider;

                    if (maxSpace > hcRequestedHeight)
                    {
                        // full size
                        height = Math.Min(hcRequestedHeight, maxSpace);
                    }
                    else
                    {
                        // what's left
                        height = maxSpace;
                    }

                    height = Math.Max(height, s_cyDivider * 2);

                    // if we've modified the height, clear the userSized item
                    if (height <= hcOptHeight && height < hcRequestedHeight)
                    {
                        _hotcommands.userSized = false;
                    }
                    else if (_hcSizeRatio != -1 || _hotcommands.userSized)
                    {
                        _hcSizeRatio = (_hotcommands.Height * 100) / Height;
                    }

                    _hotcommands.SetBounds(0, endSize - height, Size.Width, height);
                    _hotcommands.Invalidate();
                    endSize = _hotcommands.Location.Y - s_cyDivider;
                }

                _gridView.Size = new Size(Size.Width, endSize - _gridView.Location.Y);
            }
            finally
            {
                FreezePainting = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs me)
        {
            SnappableControl target = DividerInside(me.X, me.Y);
            if (target != null && me.Button == MouseButtons.Left)
            {
                // Capture the mouse.
                Capture = true;
                _targetMove = target;
                _dividerMoveY = me.Y;
                DividerDraw(_dividerMoveY);
            }

            base.OnMouseDown(me);
        }

        protected override void OnMouseMove(MouseEventArgs me)
        {
            if (_dividerMoveY == -1)
            {
                if (DividerInside(me.X, me.Y) != null)
                {
                    Cursor = Cursors.HSplit;
                }
                else
                {
                    Cursor = null;
                }
                return;
            }

            int yNew = DividerLimitMove(_targetMove, me.Y);

            if (yNew != _dividerMoveY)
            {
                DividerDraw(_dividerMoveY);
                _dividerMoveY = yNew;
                DividerDraw(_dividerMoveY);
            }

            base.OnMouseMove(me);
        }

        protected override void OnMouseUp(MouseEventArgs me)
        {
            if (_dividerMoveY == -1)
            {
                return;
            }

            Cursor = null;

            DividerDraw(_dividerMoveY);
            _dividerMoveY = DividerLimitMove(_targetMove, me.Y);
            Rectangle rectDoc = _targetMove.Bounds;
            if (_dividerMoveY != rectDoc.Y)
            {
                int yNew = rectDoc.Height + rectDoc.Y - _dividerMoveY - (s_cyDivider / 2); // we subtract two so the mouse is still over the divider
                Size size = _targetMove.Size;
                size.Height = Math.Max(0, yNew);
                _targetMove.Size = size;
                _targetMove.userSized = true;
                OnLayoutInternal(true);
                // invalidate the divider area so we cleanup anything
                // left by the xor
                Invalidate(new Rectangle(0, me.Y - s_cyDivider, Size.Width, me.Y + s_cyDivider));

                // in case we're doing the top one, we might have wrecked stuff
                // on the grid
                _gridView.Invalidate(new Rectangle(0, _gridView.Size.Height - s_cyDivider, Size.Width, s_cyDivider));
            }

            // End the move
            Capture = false;
            _dividerMoveY = -1;
            _targetMove = null;
            base.OnMouseUp(me);
        }

        /// <summary>
        ///  Called when a property on an Ole32 Object that is tagged with "requestedit" is
        ///  about to be edited. See IPropertyNotifySink::OnRequestEdit
        /// </summary>
        HRESULT Ole32.IPropertyNotifySink.OnRequestEdit(Ole32.DispatchID dispID)
        {
            // Don't do anything here.
            return HRESULT.S_OK;
        }

        protected override void OnResize(EventArgs e)
        {
            if (IsHandleCreated && Visible)
            {
                OnLayoutInternal(false);
            }
            base.OnResize(e);
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            // we don't want to steal focus from the property pages...
            if (sender != _btnViewPropertyPages)
            {
                _gridView.Focus();
            }
        }

        //
        protected void OnComComponentNameChanged(ComponentRenameEventArgs e)
        {
            ((ComponentRenameEventHandler)Events[EventComComponentNameChanged])?.Invoke(this, e);
        }

        // Seems safe - doesn't do anything interesting
        protected void OnNotifyPropertyValueUIItemsChanged(object sender, EventArgs e)
        {
            _gridView.LabelPaintMargin = 0;
            _gridView.Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Just erase the stuff above and below the properties window so we don't flicker.
            Point psheetLoc = _gridView.Location;
            int width = Size.Width;

            using var backgroundBrush = BackColor.GetCachedSolidBrushScope();
            pevent.Graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, width, psheetLoc.Y));

            int yLast = psheetLoc.Y + _gridView.Size.Height;

            // fill above hotcommands
            if (_hotcommands.Visible)
            {
                pevent.Graphics.FillRectangle(
                    backgroundBrush,
                    new Rectangle(0, yLast, width, _hotcommands.Location.Y - yLast));
                yLast += _hotcommands.Size.Height;
            }

            // Fill above doccomment
            if (_doccomment.Visible)
            {
                pevent.Graphics.FillRectangle(
                    backgroundBrush,
                    new Rectangle(0, yLast, width, _doccomment.Location.Y - yLast));
                yLast += _doccomment.Size.Height;
            }

            // anything that might be left
            pevent.Graphics.FillRectangle(backgroundBrush, new Rectangle(0, yLast, width, Size.Height - yLast));

            base.OnPaint(pevent);
        }

        // Seems safe - just fires an event
        protected virtual void OnPropertySortChanged(EventArgs e)
        {
            ((EventHandler)Events[EventPropertySortChanged])?.Invoke(this, e);
        }

        // Seems safe - just fires an event
        protected virtual void OnPropertyTabChanged(PropertyTabChangedEventArgs e)
        {
            ((PropertyTabChangedEventHandler)Events[EventPropertyTabChanged])?.Invoke(this, e);
        }

        // Seems safe - just fires an event
        protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
        {
            ((PropertyValueChangedEventHandler)Events[EventPropertyValueChanged])?.Invoke(this, e);
        }

        internal void OnPropertyValueSet(GridItem changedItem, object oldValue)
        {
            OnPropertyValueChanged(new PropertyValueChangedEventArgs(changedItem, oldValue));

            if (changedItem is null)
            {
                return;
            }

            // Announce the property value change like standalone combobox control do: "[something] selected".
            bool dropDown = false;
            Type propertyType = changedItem.PropertyDescriptor.PropertyType;
            UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(propertyType, typeof(UITypeEditor));
            if (editor != null)
            {
                dropDown = editor.GetEditStyle() == UITypeEditorEditStyle.DropDown;
            }
            else
            {
                if (changedItem is GridEntry gridEntry && gridEntry.Enumerable)
                {
                    dropDown = true;
                }
            }

            if (dropDown && !_gridView.DropDownVisible)
            {
                AccessibilityObject.RaiseAutomationNotification(
                    Automation.AutomationNotificationKind.ActionCompleted,
                    Automation.AutomationNotificationProcessing.All,
                    string.Format(SR.PropertyGridPropertyValueSelectedFormat, changedItem.Value));
            }
        }

        internal void OnSelectedGridItemChanged(GridEntry oldEntry, GridEntry newEntry)
        {
            OnSelectedGridItemChanged(new SelectedGridItemChangedEventArgs(oldEntry, newEntry));
        }

        //
        protected virtual void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            ((SelectedGridItemChangedEventHandler)Events[EventSelectedGridItemChanged])?.Invoke(this, e);
        }

        //
        protected virtual void OnSelectedObjectsChanged(EventArgs e)
        {
            ((EventHandler)Events[EventSelectedObjectsChanged])?.Invoke(this, e);
        }

        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction)
            {
                // We should not refresh the grid if the selectedObject is no longer sited.
                if (SelectedObject is IComponent currentSelection)
                {
                    if (currentSelection.Site is null) //The component is not logically sited...so clear the PropertyGrid Selection..
                    {
                        //Setting to null... actually will clear off the state information so that ProperyGrid is in sane State.
                        SelectedObject = null;
                        return;
                    }
                }
                SetFlag(BatchMode, false);
                if (GetFlag(FullRefreshAfterBatch))
                {
                    SelectedObjects = _currentObjects;
                    SetFlag(FullRefreshAfterBatch, false);
                }
                else if (GetFlag(BatchModeChange))
                {
                    Refresh(false);
                }
                SetFlag(BatchModeChange, false);
            }
        }

        private void OnTransactionOpened(object sender, EventArgs e)
        {
            SetFlag(BatchMode, true);
        }

        private void OnTypeDescriptorRefreshed(RefreshEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RefreshEventHandler(OnTypeDescriptorRefreshedInvoke), new object[] { e });
            }
            else
            {
                OnTypeDescriptorRefreshedInvoke(e);
            }
        }

        private void OnTypeDescriptorRefreshedInvoke(RefreshEventArgs e)
        {
            if (_currentObjects != null)
            {
                for (int i = 0; i < _currentObjects.Length; i++)
                {
                    Type typeChanged = e.TypeChanged;
                    if (_currentObjects[i] == e.ComponentChanged || typeChanged?.IsAssignableFrom(_currentObjects[i].GetType()) == true)
                    {
                        // clear our property hashes
                        ClearCachedProps();
                        Refresh(true);
                        return;
                    }
                }
            }
        }

        private void OnViewSortButtonClick(object sender, EventArgs e)
        {
            try
            {
                FreezePainting = true;

                // is this tab selected? If so, do nothing.
                if (sender == _viewSortButtons[_selectedViewSort])
                {
                    _viewSortButtons[_selectedViewSort].Checked = true;
                    return;
                }

                // check new button and uncheck old button.
                _viewSortButtons[_selectedViewSort].Checked = false;

                // find the new button in the list
                int index = 0;
                for (index = 0; index < _viewSortButtons.Length; index++)
                {
                    if (_viewSortButtons[index] == sender)
                    {
                        break;
                    }
                }

                _selectedViewSort = index;
                _viewSortButtons[_selectedViewSort].Checked = true;

                switch (_selectedViewSort)
                {
                    case ALPHA:
                        _propertySortValue = PropertySort.Alphabetical;
                        break;
                    case CATEGORIES:
                        _propertySortValue = PropertySort.Alphabetical | PropertySort.Categorized;
                        break;
                    case NO_SORT:
                        _propertySortValue = PropertySort.NoSort;
                        break;
                }

                OnPropertySortChanged(EventArgs.Empty);

                Refresh(false);
                OnLayoutInternal(false);
            }
            finally
            {
                FreezePainting = false;
            }
            OnButtonClick(sender, e);
        }

        private void OnViewTabButtonClick(object sender, EventArgs e)
        {
            try
            {
                FreezePainting = true;
                SelectViewTabButton((ToolStripButton)sender, true);
                OnLayoutInternal(false);
                SaveTabSelection();
            }
            finally
            {
                FreezePainting = false;
            }
            OnButtonClick(sender, e);
        }

        private void OnViewButtonClickPP(object sender, EventArgs e)
        {
            if (_btnViewPropertyPages.Enabled &&
                _currentObjects != null &&
                _currentObjects.Length > 0)
            {
                object baseObject = _currentObjects[0];
                object obj = baseObject;

                bool success = false;

                IUIService uiSvc = (IUIService)GetService(typeof(IUIService));

                try
                {
                    if (uiSvc != null)
                    {
                        success = uiSvc.ShowComponentEditor(obj, this);
                    }
                    else
                    {
                        try
                        {
                            ComponentEditor editor = (ComponentEditor)TypeDescriptor.GetEditor(obj, typeof(ComponentEditor));
                            if (editor != null)
                            {
                                if (editor is WindowsFormsComponentEditor)
                                {
                                    success = ((WindowsFormsComponentEditor)editor).EditComponent(null, obj, (IWin32Window)this);
                                }
                                else
                                {
                                    success = editor.EditComponent(obj);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (success)
                    {
                        if (baseObject is IComponent &&
                            connectionPointCookies[0] is null)
                        {
                            ISite site = ((IComponent)baseObject).Site;
                            if (site != null)
                            {
                                IComponentChangeService changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                                if (changeService != null)
                                {
                                    try
                                    {
                                        changeService.OnComponentChanging(baseObject, null);
                                    }
                                    catch (CheckoutException coEx)
                                    {
                                        if (coEx == CheckoutException.Canceled)
                                        {
                                            return;
                                        }
                                        throw;
                                    }

                                    try
                                    {
                                        // Now notify the change service that the change was successful.
                                        //
                                        SetFlag(InternalChange, true);
                                        changeService.OnComponentChanged(baseObject, null, null, null);
                                    }
                                    finally
                                    {
                                        SetFlag(InternalChange, false);
                                    }
                                }
                            }
                        }
                        _gridView.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    string errString = SR.ErrorPropertyPageFailed;
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

        //
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && IsHandleCreated)
            {
                OnLayoutInternal(false);
                SetupToolbar();
            }
        }

        /// <summary>
        ///  Returns the last child control that can take focus
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Tab:
                    if (((keyData & Keys.Control) != 0) ||
                        ((keyData & Keys.Alt) != 0))
                    {
                        break;
                    }

                    // are we going forward?
                    if ((keyData & Keys.Shift) != 0)
                    {
                        // this is backward
                        if (_hotcommands.Visible && _hotcommands.ContainsFocus)
                        {
                            _gridView.ReverseFocus();
                        }
                        else if (_gridView.FocusInside)
                        {
                            if (_toolStrip.Visible)
                            {
                                _toolStrip.Focus();

                                // we need to select first ToolStrip item, otherwise, ToolStrip container has the focus
                                if (_toolStrip.Items.Count > 0)
                                {
                                    _toolStrip.SelectNextToolStripItem(null, /*forward =*/ true);
                                }
                            }
                            else
                            {
                                return base.ProcessDialogKey(keyData);
                            }
                        }
                        else
                        {
                            // if we get here and the toolbar has focus,
                            // it means we're processing normally, so
                            // pass the focus to the parent
                            if (_toolStrip.Focused || !_toolStrip.Visible)
                            {
                                return base.ProcessDialogKey(keyData);
                            }
                            else
                            {
                                // otherwise, we're processing a message from elsewhere,
                                // wo we select our bottom guy.
                                if (_hotcommands.Visible)
                                {
                                    _hotcommands.Select(false);
                                }
                                else if (_peMain != null)
                                {
                                    _gridView.ReverseFocus();
                                }
                                else if (_toolStrip.Visible)
                                {
                                    _toolStrip.Focus();
                                }
                                else
                                {
                                    return base.ProcessDialogKey(keyData);
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        bool passToParent = false;

                        // this is forward
                        if (_toolStrip.Focused)
                        {
                            // normal stuff, just do the propsheet
                            if (_peMain != null)
                            {
                                _gridView.Focus();
                            }
                            else
                            {
                                base.ProcessDialogKey(keyData);
                            }
                            return true;
                        }
                        else if (_gridView.FocusInside)
                        {
                            if (_hotcommands.Visible)
                            {
                                _hotcommands.Select(true);
                                return true;
                            }
                            else
                            {
                                passToParent = true;
                            }
                        }
                        else if (_hotcommands.ContainsFocus)
                        {
                            passToParent = true;
                        }
                        else
                        {
                            // coming from out side, start with the toolStrip
                            if (_toolStrip.Visible)
                            {
                                _toolStrip.Focus();
                            }
                            else
                            {
                                _gridView.Focus();
                            }
                        }

                        // nobody's claimed the focus, pass it on...
                        if (passToParent)
                        {
                            // properties window is already selected
                            // pass on to parent
                            bool result = base.ProcessDialogKey(keyData);

                            // if we're not hosted in a windows forms thing, just give the parent the focus
                            if (!result && Parent is null)
                            {
                                IntPtr hWndParent = User32.GetParent(this);
                                if (hWndParent != IntPtr.Zero)
                                {
                                    User32.SetFocus(hWndParent);
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

        public override void Refresh()
        {
            if (GetFlag(RefreshingProperties))
            {
                return;
            }

            Refresh(true);
            base.Refresh();
        }

        private void Refresh(bool clearCached)
        {
            if (Disposing)
            {
                return;
            }

            if (GetFlag(RefreshingProperties))
            {
                return;
            }

            try
            {
                FreezePainting = true;
                SetFlag(RefreshingProperties, true);

                if (clearCached)
                {
                    ClearCachedProps();
                }
                RefreshProperties(clearCached);
                _gridView.Refresh();
                DisplayHotCommands();
            }
            finally
            {
                FreezePainting = false;
                SetFlag(RefreshingProperties, false);
            }
        }

        internal void RefreshProperties(bool clearCached)
        {
            // Clear our current cache so we can do a full refresh.
            if (clearCached && _selectedViewTab != -1 && _viewTabs != null)
            {
                PropertyTab tab = _viewTabs[_selectedViewTab];
                if (tab != null && _viewTabProps != null)
                {
                    string tabName = tab.TabName + _propertySortValue.ToString();
                    _viewTabProps.Remove(tabName);
                }
            }

            SetFlag(PropertiesChanged, true);
            UpdateSelection();
        }

        /// <summary>
        ///  Refreshes the tabs of the given scope by deleting them and requerying objects and documents
        ///  for them.
        /// </summary>
        public void RefreshTabs(PropertyTabScope tabScope)
        {
            if (tabScope < PropertyTabScope.Document)
            {
                throw new ArgumentException(SR.PropertyGridTabScope);
            }

            RemoveTabs(tabScope, false);

            // check the component level tabs
            if (tabScope <= PropertyTabScope.Component)
            {
                if (_currentObjects != null && _currentObjects.Length > 0)
                {
                    // get the subset of PropertyTabs that's common to all objects
                    Type[] tabTypes = GetCommonTabs(_currentObjects, PropertyTabScope.Component);

                    for (int i = 0; i < tabTypes.Length; i++)
                    {
                        for (int j = 0; j < _currentObjects.Length; j++)
                        {
                            AddRefTab(tabTypes[i], _currentObjects[j], PropertyTabScope.Component, false);
                        }
                    }
                }
            }

            // check the document level tabs
            if (tabScope <= PropertyTabScope.Document && _designerHost != null)
            {
                IContainer container = _designerHost.Container;
                if (container != null)
                {
                    ComponentCollection components = container.Components;
                    if (components != null)
                    {
                        foreach (IComponent comp in components)
                        {
                            PropertyTabAttribute attribute = (PropertyTabAttribute)TypeDescriptor.GetAttributes(comp.GetType())[typeof(PropertyTabAttribute)];

                            if (attribute != null)
                            {
                                for (int j = 0; j < attribute.TabClasses.Length; j++)
                                {
                                    if (attribute.TabScopes[j] == PropertyTabScope.Document)
                                    {
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

        internal void ReleaseTab(Type tabType, object component)
        {
            PropertyTab tab = null;
            int tabIndex = -1;
            for (int i = 0; i < _viewTabs.Length; i++)
            {
                if (tabType == _viewTabs[i].GetType())
                {
                    tab = _viewTabs[i];
                    tabIndex = i;
                    break;
                }
            }

            if (tab is null)
            {
                return;
            }

            object[] components = tab.Components;
            bool killTab = false;

            try
            {
                int index = -1;
                if (components != null)
                {
                    index = Array.IndexOf(components, component);
                }

                if (index >= 0)
                {
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
            if (killTab && _viewTabScopes[tabIndex] > PropertyTabScope.Global)
            {
                RemoveTab(tabIndex, false);
            }
        }

        private void RemoveImage(int index)
        {
            _imageList[NormalButtonSize].Images.RemoveAt(index);
            if (_imageList[LargeButtonSize] != null)
            {
                _imageList[LargeButtonSize].Images.RemoveAt(index);
            }
        }

        // removes all the tabs with a classification greater than or equal to the specified classification.
        // for example, removing PropertyTabScope.Document will remove PropertyTabScope.Document and PropertyTabScope.Component tabs
        internal void RemoveTabs(PropertyTabScope classification, bool setupToolbar)
        {
            if (classification == PropertyTabScope.Static)
            {
                throw new ArgumentException(SR.PropertyGridRemoveStaticTabs);
            }

            // in case we've been disposed
            if (_viewTabButtons is null || _viewTabs is null || _viewTabScopes is null)
            {
                return;
            }

            ToolStripButton selectedButton = (_selectedViewTab >= 0 && _selectedViewTab < _viewTabButtons.Length ? _viewTabButtons[_selectedViewTab] : null);

            for (int i = _viewTabs.Length - 1; i >= 0; i--)
            {
                if (_viewTabScopes[i] >= classification)
                {
                    // adjust the selected view tab because we're deleting.
                    if (_selectedViewTab == i)
                    {
                        _selectedViewTab = -1;
                    }
                    else if (_selectedViewTab > i)
                    {
                        _selectedViewTab--;
                    }

                    PropertyTab[] newTabs = new PropertyTab[_viewTabs.Length - 1];
                    Array.Copy(_viewTabs, 0, newTabs, 0, i);
                    Array.Copy(_viewTabs, i + 1, newTabs, i, _viewTabs.Length - i - 1);
                    _viewTabs = newTabs;

                    PropertyTabScope[] newTabScopes = new PropertyTabScope[_viewTabScopes.Length - 1];
                    Array.Copy(_viewTabScopes, 0, newTabScopes, 0, i);
                    Array.Copy(_viewTabScopes, i + 1, newTabScopes, i, _viewTabScopes.Length - i - 1);
                    _viewTabScopes = newTabScopes;

                    _viewTabsDirty = true;
                }
            }

            if (setupToolbar && _viewTabsDirty)
            {
                SetupToolbar();

                Debug.Assert(_viewTabs != null && _viewTabs.Length > 0, "Holy Moly!  We don't have any tabs left!");

                _selectedViewTab = -1;
                SelectViewTabButtonDefault(selectedButton);

                // clear the component refs of the tabs
                for (int i = 0; i < _viewTabs.Length; i++)
                {
                    _viewTabs[i].Components = Array.Empty<object>();
                }
            }
        }

        internal void RemoveTab(int tabIndex, bool setupToolbar)
        {
            Debug.Assert(_viewTabs != null, "Tab array destroyed!");

            if (tabIndex >= _viewTabs.Length || tabIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tabIndex), SR.PropertyGridBadTabIndex);
            }

            if (_viewTabScopes[tabIndex] == PropertyTabScope.Static)
            {
                throw new ArgumentException(SR.PropertyGridRemoveStaticTabs);
            }

            if (_selectedViewTab == tabIndex)
            {
                _selectedViewTab = PROPERTIES;
            }

            // Remove this tab from our "last selected" group
            //
            if (!GetFlag(ReInitTab) && ActiveDesigner != null)
            {
                int hashCode = ActiveDesigner.GetHashCode();
                if (_designerSelections != null && _designerSelections.ContainsKey(hashCode) && (int)_designerSelections[hashCode] == tabIndex)
                {
                    _designerSelections.Remove(hashCode);
                }
            }

            ToolStripButton selectedButton = _viewTabButtons[_selectedViewTab];

            PropertyTab[] newTabs = new PropertyTab[_viewTabs.Length - 1];
            Array.Copy(_viewTabs, 0, newTabs, 0, tabIndex);
            Array.Copy(_viewTabs, tabIndex + 1, newTabs, tabIndex, _viewTabs.Length - tabIndex - 1);
            _viewTabs = newTabs;

            PropertyTabScope[] newTabScopes = new PropertyTabScope[_viewTabScopes.Length - 1];
            Array.Copy(_viewTabScopes, 0, newTabScopes, 0, tabIndex);
            Array.Copy(_viewTabScopes, tabIndex + 1, newTabScopes, tabIndex, _viewTabScopes.Length - tabIndex - 1);
            _viewTabScopes = newTabScopes;

            _viewTabsDirty = true;

            if (setupToolbar)
            {
                SetupToolbar();
                _selectedViewTab = -1;
                SelectViewTabButtonDefault(selectedButton);
            }
        }

        internal void RemoveTab(Type tabType)
        {
            PropertyTab tab = null;
            int tabIndex = -1;
            for (int i = 0; i < _viewTabs.Length; i++)
            {
                if (tabType == _viewTabs[i].GetType())
                {
                    tab = _viewTabs[i];
                    tabIndex = i;
                    break;
                }
            }

            // just quit if the tab isn't present.
            if (tabIndex == -1)
            {
                return;
            }

            PropertyTab[] newTabs = new PropertyTab[_viewTabs.Length - 1];
            Array.Copy(_viewTabs, 0, newTabs, 0, tabIndex);
            Array.Copy(_viewTabs, tabIndex + 1, newTabs, tabIndex, _viewTabs.Length - tabIndex - 1);
            _viewTabs = newTabs;

            PropertyTabScope[] newTabScopes = new PropertyTabScope[_viewTabScopes.Length - 1];
            Array.Copy(_viewTabScopes, 0, newTabScopes, 0, tabIndex);
            Array.Copy(_viewTabScopes, tabIndex + 1, newTabScopes, tabIndex, _viewTabScopes.Length - tabIndex - 1);
            _viewTabScopes = newTabScopes;

            _viewTabsDirty = true;
            SetupToolbar();
        }

        private void ResetCommandsBackColor()
        {
            _hotcommands.ResetBackColor();
        }

        private void ResetCommandsForeColor()
        {
            _hotcommands.ResetForeColor();
        }

        private void ResetCommandsLinkColor()
        {
            _hotcommands.Label.ResetLinkColor();
        }

        private void ResetCommandsActiveLinkColor()
        {
            _hotcommands.Label.ResetActiveLinkColor();
        }

        private void ResetCommandsDisabledLinkColor()
        {
            _hotcommands.Label.ResetDisabledLinkColor();
        }

        private void ResetHelpBackColor()
        {
            _doccomment.ResetBackColor();
        }

        private void ResetHelpForeColor()
        {
            _doccomment.ResetBackColor();
        }

        // This method is intended for use in replacing a specific selected root object with
        // another object of the same exact type. Scenario: An immutable root object being
        // replaced with a new instance because one of its properties was changed by the user.
        //
        internal void ReplaceSelectedObject(object oldObject, object newObject)
        {
            Debug.Assert(oldObject != null && newObject != null && oldObject.GetType() == newObject.GetType());

            for (int i = 0; i < _currentObjects.Length; ++i)
            {
                if (_currentObjects[i] == oldObject)
                {
                    _currentObjects[i] = newObject;
                    Refresh(true);
                    break;
                }
            }
        }

        public void ResetSelectedProperty()
        {
            GetPropertyGridView().Reset();
        }

        private void SaveTabSelection()
        {
            if (_designerHost != null)
            {
                if (_designerSelections is null)
                {
                    _designerSelections = new Hashtable();
                }
                _designerSelections[_designerHost.GetHashCode()] = _selectedViewTab;
            }
        }

        void IComPropertyBrowser.SaveState(RegistryKey optRoot)
        {
            if (optRoot is null)
            {
                return;
            }

            optRoot.SetValue("PbrsAlpha", (PropertySort == PropertySort.Alphabetical ? "1" : "0"));
            optRoot.SetValue("PbrsShowDesc", (HelpVisible ? "1" : "0"));
            optRoot.SetValue("PbrsShowCommands", (CommandsVisibleIfAvailable ? "1" : "0"));
            optRoot.SetValue("PbrsDescHeightRatio", _dcSizeRatio.ToString(CultureInfo.InvariantCulture));
            optRoot.SetValue("PbrsHotCommandHeightRatio", _hcSizeRatio.ToString(CultureInfo.InvariantCulture));
        }

        void SetHotCommandColors(bool vscompat)
        {
            if (vscompat)
            {
                _hotcommands.SetColors(SystemColors.Control, SystemColors.ControlText, SystemColors.ActiveCaption, SystemColors.ActiveCaption, SystemColors.ActiveCaption, SystemColors.ControlDark);
            }
            else
            {
                _hotcommands.SetColors(SystemColors.Control, SystemColors.ControlText, Color.Empty, Color.Empty, Color.Empty, Color.Empty);
            }
        }

        internal void SetStatusBox(string title, string desc)
        {
            _doccomment.SetComment(title, desc);
        }

        private void SelectViewTabButton(ToolStripButton button, bool updateSelection)
        {
            Debug.Assert(_viewTabButtons != null, "No view tab buttons to select!");

            int oldTab = _selectedViewTab;

            if (!SelectViewTabButtonDefault(button))
            {
                Debug.Fail("Failed to find the tab!");
            }

            if (updateSelection)
            {
                Refresh(false);
            }
        }

        private bool SelectViewTabButtonDefault(ToolStripButton button)
        {
            // make sure our selection number is valid
            if (_selectedViewTab >= 0 && _selectedViewTab >= _viewTabButtons.Length)
            {
                _selectedViewTab = -1;
            }

            // is this tab button checked? If so, do nothing.
            if (_selectedViewTab >= 0 && _selectedViewTab < _viewTabButtons.Length &&
                button == _viewTabButtons[_selectedViewTab])
            {
                _viewTabButtons[_selectedViewTab].Checked = true;
                return true;
            }

            PropertyTab oldTab = null;

            // unselect what's selected
            if (_selectedViewTab != -1)
            {
                _viewTabButtons[_selectedViewTab].Checked = false;
                oldTab = _viewTabs[_selectedViewTab];
            }

            // get the new index of the button
            for (int i = 0; i < _viewTabButtons.Length; i++)
            {
                if (_viewTabButtons[i] == button)
                {
                    _selectedViewTab = i;
                    _viewTabButtons[i].Checked = true;
                    try
                    {
                        SetFlag(TabsChanging, true);
                        OnPropertyTabChanged(new PropertyTabChangedEventArgs(oldTab, _viewTabs[i]));
                    }
                    finally
                    {
                        SetFlag(TabsChanging, false);
                    }
                    return true;
                }
            }

            // select the first tab if we didn't find that one.
            _selectedViewTab = PROPERTIES;
            Debug.Assert(_viewTabs[PROPERTIES].GetType() == DefaultTabType, "First item is not property tab!");
            SelectViewTabButton(_viewTabButtons[PROPERTIES], false);
            return false;
        }

        private void SetSelectState(int state)
        {
            if (state >= (_viewTabs.Length * _viewSortButtons.Length))
            {
                state = 0;
            }
            else if (state < 0)
            {
                state = (_viewTabs.Length * _viewSortButtons.Length) - 1;
            }

            // NOTE: See GetSelectState for the full description
            // of the state transitions

            // views == 2 (Alpha || Categories)
            // viewTabs = viewTabs.length

            // state -> tab = state / views
            // state -> view = state % views

            int viewTypes = _viewSortButtons.Length;

            if (viewTypes > 0)
            {
                int tab = state / viewTypes;
                int view = state % viewTypes;

                Debug.Assert(view < _viewSortButtons.Length, "Can't select view type > 1");

                OnViewTabButtonClick(_viewTabButtons[tab], EventArgs.Empty);
                OnViewSortButtonClick(_viewSortButtons[view], EventArgs.Empty);
            }
        }

        private void SetToolStripRenderer()
        {
            if (DrawFlatToolbar || SystemInformation.HighContrast)
            {
                // use an office look and feel with system colors
                ProfessionalColorTable colorTable = new ProfessionalColorTable
                {
                    UseSystemColors = true
                };
                ToolStripRenderer = new ToolStripProfessionalRenderer(colorTable);
            }
            else
            {
                ToolStripRenderer = new ToolStripSystemRenderer();
            }
        }

        private void SetupToolbar()
        {
            SetupToolbar(false);
        }

        private void SetupToolbar(bool fullRebuild)
        {
            // if the tab array hasn't changed, don't bother to do all
            // this work.
            //
            if (!_viewTabsDirty && !fullRebuild)
            {
                return;
            }

            try
            {
                FreezePainting = true;

                if (_imageList[NormalButtonSize] is null || fullRebuild)
                {
                    _imageList[NormalButtonSize] = new ImageList();
                    if (DpiHelper.IsScalingRequired)
                    {
                        _imageList[NormalButtonSize].ImageSize = s_normalButtonSize;
                    }
                }

                // setup our event handlers
                EventHandler ehViewTab = new EventHandler(OnViewTabButtonClick);
                EventHandler ehViewType = new EventHandler(OnViewSortButtonClick);
                EventHandler ehPP = new EventHandler(OnViewButtonClickPP);

                Bitmap b;
                int i;

                // we manange the buttons as a seperate list so the toobar doesn't flash
                ArrayList buttonList;

                if (fullRebuild)
                {
                    buttonList = new ArrayList();
                }
                else
                {
                    buttonList = new ArrayList(_toolStrip.Items);
                }

                // setup the view type buttons.  We only need to do this once
                if (_viewSortButtons is null || fullRebuild)
                {
                    _viewSortButtons = new ToolStripButton[3];

                    int alphaIndex = -1;
                    int categoryIndex = -1;

                    try
                    {
                        if (_bmpAlpha is null)
                        {
                            _bmpAlpha = SortByPropertyImage;
                        }
                        alphaIndex = AddImage(_bmpAlpha);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (_bmpCategory is null)
                        {
                            _bmpCategory = SortByCategoryImage;
                        }
                        categoryIndex = AddImage(_bmpCategory);
                    }
                    catch (Exception)
                    {
                    }

                    _viewSortButtons[ALPHA] = CreatePushButton(SR.PBRSToolTipAlphabetic, alphaIndex, ehViewType, true);
                    _viewSortButtons[CATEGORIES] = CreatePushButton(SR.PBRSToolTipCategorized, categoryIndex, ehViewType, true);

                    // we create a dummy hidden button for view sort
                    _viewSortButtons[NO_SORT] = CreatePushButton("", 0, ehViewType, true);
                    _viewSortButtons[NO_SORT].Visible = false;

                    // add the viewType buttons and a separator
                    for (i = 0; i < _viewSortButtons.Length; i++)
                    {
                        buttonList.Add(_viewSortButtons[i]);
                    }
                }
                else
                {
                    // clear all the items from the toolStrip and image list after the first two
                    int items = buttonList.Count;

                    for (i = items - 1; i >= 2; i--)
                    {
                        buttonList.RemoveAt(i);
                    }

                    items = _imageList[NormalButtonSize].Images.Count;

                    for (i = items - 1; i >= 2; i--)
                    {
                        RemoveImage(i);
                    }
                }

                buttonList.Add(_separator1);

                // here's our buttons array
                _viewTabButtons = new ToolStripButton[_viewTabs.Length];
                bool doAdd = _viewTabs.Length > 1;

                // if we've only got the properties tab, don't add
                // the button (or we'll just have a properties button that you can't do anything with)
                // setup the view tab buttons
                for (i = 0; i < _viewTabs.Length; i++)
                {
                    try
                    {
                        b = _viewTabs[i].Bitmap;
                        _viewTabButtons[i] = CreatePushButton(_viewTabs[i].TabName, AddImage(b), ehViewTab, true);
                        if (doAdd)
                        {
                            buttonList.Add(_viewTabButtons[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.ToString());
                    }
                }

                // if we didn't add anything, we don't need another separator either.
                if (doAdd)
                {
                    buttonList.Add(_separator2);
                }

                // add the design page button
                int designpg = 0;

                try
                {
                    if (_bmpPropPage is null)
                    {
                        _bmpPropPage = ShowPropertyPageImage;
                    }
                    designpg = AddImage(_bmpPropPage);
                }
                catch (Exception)
                {
                }

                // we recreate this every time to ensure it's at the end
                //
                _btnViewPropertyPages = CreatePushButton(SR.PBRSToolTipPropertyPages, designpg, ehPP, false);
                _btnViewPropertyPages.Enabled = false;
                buttonList.Add(_btnViewPropertyPages);

                // Dispose this so it will get recreated for any new buttons.
                if (_imageList[LargeButtonSize] != null)
                {
                    _imageList[LargeButtonSize].Dispose();
                    _imageList[LargeButtonSize] = null;
                }

                if (_buttonType != NormalButtonSize)
                {
                    EnsureLargeButtons();
                }

                _toolStrip.ImageList = _imageList[_buttonType];

                _toolStrip.SuspendLayout();
                _toolStrip.Items.Clear();
                for (int j = 0; j < buttonList.Count; j++)
                {
                    _toolStrip.Items.Add(buttonList[j] as ToolStripItem);
                }
                _toolStrip.ResumeLayout();

                if (_viewTabsDirty)
                {
                    // if we're redoing our tabs make sure
                    // we setup the toolbar area correctly.
                    //
                    OnLayoutInternal(false);
                }

                _viewTabsDirty = false;
            }
            finally
            {
                FreezePainting = false;
            }
        }

        protected void ShowEventsButton(bool value)
        {
            if (_viewTabs != null && _viewTabs.Length > EVENTS && (_viewTabs[EVENTS] is EventsTab))
            {
                Debug.Assert(_viewTabButtons != null && _viewTabButtons.Length > EVENTS && _viewTabButtons[EVENTS] != null, "Events button is not at EVENTS position");
                _viewTabButtons[EVENTS].Visible = value;
                if (!value && _selectedViewTab == EVENTS)
                {
                    SelectViewTabButton(_viewTabButtons[PROPERTIES], true);
                }
            }

            UpdatePropertiesViewTabVisibility();
        }

        /// <summary>
        ///  This 16x16 Bitmap is applied to the button which orders properties alphabetically.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Bitmap SortByPropertyImage
        {
            get
            {
                return DpiHelper.GetBitmapFromIcon(typeof(PropertyGrid), "PBAlpha");
            }
        }

        /// <summary>
        ///  This 16x16 Bitmap is applied to the button which displays properties under the assigned categories.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Bitmap SortByCategoryImage
        {
            get
            {
                return DpiHelper.GetBitmapFromIcon(typeof(PropertyGrid), "PBCatego");
            }
        }

        /// <summary>
        ///  This 16x16 Bitmap is applied to the button which displays property page in the designer pane.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Bitmap ShowPropertyPageImage
        {
            get
            {
                return DpiHelper.GetBitmapFromIcon(typeof(PropertyGrid), "PBPPage");
            }
        }

        private bool ShouldSerializeCommandsBackColor()
        {
            return _hotcommands.ShouldSerializeBackColor();
        }

        private bool ShouldSerializeCommandsForeColor()
        {
            return _hotcommands.ShouldSerializeForeColor();
        }

        private bool ShouldSerializeCommandsLinkColor()
        {
            return _hotcommands.Label.ShouldSerializeLinkColor();
        }

        private bool ShouldSerializeCommandsActiveLinkColor()
        {
            return _hotcommands.Label.ShouldSerializeActiveLinkColor();
        }

        private bool ShouldSerializeCommandsDisabledLinkColor()
        {
            return _hotcommands.Label.ShouldSerializeDisabledLinkColor();
        }

        /// <summary>
        ///  Sinks the property notify events on all the objects we are currently
        ///  browsing.
        ///
        ///  See IPropertyNotifySink
        /// </summary>
        private void SinkPropertyNotifyEvents()
        {
            // first clear any existing sinks.
            for (int i = 0; connectionPointCookies != null && i < connectionPointCookies.Length; i++)
            {
                if (connectionPointCookies[i] != null)
                {
                    connectionPointCookies[i].Disconnect();
                    connectionPointCookies[i] = null;
                }
            }

            if (_currentObjects is null || _currentObjects.Length == 0)
            {
                connectionPointCookies = null;
                return;
            }

            // it's okay if our array is too big...we'll just reuse it and ignore the empty slots.
            if (connectionPointCookies is null || (_currentObjects.Length > connectionPointCookies.Length))
            {
                connectionPointCookies = new AxHost.ConnectionPointCookie[_currentObjects.Length];
            }

            for (int i = 0; i < _currentObjects.Length; i++)
            {
                try
                {
                    object obj = GetUnwrappedObject(i);

                    if (!Marshal.IsComObject(obj))
                    {
                        continue;
                    }
                    connectionPointCookies[i] = new AxHost.ConnectionPointCookie(obj, this, typeof(Ole32.IPropertyNotifySink), /*throwException*/ false);
                }
                catch
                {
                    // guess we failed eh?
                }
            }
        }

        private bool ShouldForwardChildMouseMessage(Control child, MouseEventArgs me, ref Point pt)
        {
            Size size = child.Size;

            // are we within two pixels of the edge?
            if (me.Y <= 1 || (size.Height - me.Y) <= 1)
            {
                // convert the coordinates to
                var temp = new Point(me.X, me.Y);
                User32.MapWindowPoints(new HandleRef(child, child.Handle), new HandleRef(this, Handle), ref temp, 1);

                // forward the message
                pt = temp;
                return true;
            }

            return false;
        }

        private void UpdatePropertiesViewTabVisibility()
        {
            // If the only view available is properties-view, there's no need to show the button.
            //
            if (_viewTabButtons != null)
            {
                int nOtherViewsVisible = 0;
                for (int i = 1; i < _viewTabButtons.Length; i++)
                { // Starts at index 1, since index 0 is properties-view
                    if (_viewTabButtons[i].Visible)
                    {
                        nOtherViewsVisible++;
                    }
                }
                if (nOtherViewsVisible > 0)
                {
                    _viewTabButtons[PROPERTIES].Visible = true;
                    _separator2.Visible = true;
                }
                else
                {
                    _viewTabButtons[PROPERTIES].Visible = false;
                    _separator2.Visible = false;
                }
            }
        }

        internal void UpdateSelection()
        {
            if (!GetFlag(PropertiesChanged))
            {
                return;
            }

            if (_viewTabs is null)
            {
                return;
            }

            string tabName = _viewTabs[_selectedViewTab].TabName + _propertySortValue.ToString();

            if (_viewTabProps != null && _viewTabProps.ContainsKey(tabName))
            {
                _peMain = (GridEntry)_viewTabProps[tabName];
                if (_peMain != null)
                {
                    _peMain.Refresh();
                }
            }
            else
            {
                if (_currentObjects != null && _currentObjects.Length > 0)
                {
                    _peMain = (GridEntry)GridEntry.Create(_gridView, _currentObjects, new PropertyGridServiceProvider(this), _designerHost, SelectedTab, _propertySortValue);
                }
                else
                {
                    _peMain = null;
                }

                if (_peMain is null)
                {
                    _currentPropEntries = new GridEntryCollection(null, Array.Empty<GridEntry>());
                    _gridView.ClearProps();
                    return;
                }

                if (BrowsableAttributes != null)
                {
                    _peMain.BrowsableAttributes = BrowsableAttributes;
                }

                if (_viewTabProps is null)
                {
                    _viewTabProps = new Hashtable();
                }

                _viewTabProps[tabName] = _peMain;
            }

            // get entries.
            _currentPropEntries = _peMain.Children;
            _peDefault = _peMain.DefaultChild;
            _gridView.Invalidate();
        }

        /// <summary>
        ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </summary>
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))]
        public bool UseCompatibleTextRendering
        {
            get => base.UseCompatibleTextRenderingInt;
            set
            {
                base.UseCompatibleTextRenderingInt = value;
                _doccomment.UpdateTextRenderingEngine();
                _gridView.Invalidate();
            }
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  Determines whether the control supports rendering text using GDI+ and GDI.
        ///  This is provided for container controls to iterate through its children to set UseCompatibleTextRendering to the same
        ///  value if the child control supports it.
        /// </summary>
        internal override bool SupportsUseCompatibleTextRendering
        {
            get
            {
                return true;
            }
        }

        internal override bool AllowsKeyboardToolTip()
        {
            return false;
        }

        // a mini version of process dialog key
        // for responding to WM_GETDLGCODE
        internal bool WantsTab(bool forward)
        {
            if (forward)
            {
                return _toolStrip.Visible && _toolStrip.Focused;
            }
            else
            {
                return _gridView.ContainsFocus && _toolStrip.Visible;
            }
        }

        private string propName;
        private int dwMsg;

        private unsafe void GetDataFromCopyData(IntPtr lparam)
        {
            User32.COPYDATASTRUCT* cds = (User32.COPYDATASTRUCT*)lparam;

            if (cds != null && cds->lpData != IntPtr.Zero)
            {
                propName = Marshal.PtrToStringAuto(cds->lpData);
                dwMsg = (int)cds->dwData;
            }
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            // refresh the toolbar buttons
            SetupToolbar(true);

            // this doesn't stick the first time we do it...
            // either probably a toolbar issue, maybe GDI+, so we call it again
            // fortunately this doesn't happen very often.
            //
            if (!GetFlag(SysColorChangeRefresh))
            {
                SetupToolbar(true);
                SetFlag(SysColorChangeRefresh, true);
            }
            base.OnSystemColorsChanged(e);
        }

        /// <summary>
        ///  Rescaling constants.
        /// </summary>
        private void RescaleConstants()
        {
            s_normalButtonSize = LogicalToDeviceUnits(s_defaultNormalButtonSize);
            s_largeButtonSize = LogicalToDeviceUnits(s_defaultLargeButtonSize);
            s_cyDivider = LogicalToDeviceUnits(CYDIVIDER);
            _toolStripButtonPaddingY = LogicalToDeviceUnits(ToolStripButtonPaddingY);
        }

        /// <summary>
        ///  Rescale constants when DPI changed
        /// </summary>
        /// <param name="deviceDpiOld">old dpi</param>
        /// <param name="deviceDpiNew">new dpi</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            RescaleConstants();
            SetupToolbar(true);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)User32.WM.UNDO:
                    if ((long)m.LParam == 0)
                    {
                        _gridView.DoUndoCommand();
                    }
                    else
                    {
                        m.Result = CanUndo ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;
                case (int)User32.WM.CUT:
                    if ((long)m.LParam == 0)
                    {
                        _gridView.DoCutCommand();
                    }
                    else
                    {
                        m.Result = CanCut ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;

                case (int)User32.WM.COPY:
                    if ((long)m.LParam == 0)
                    {
                        _gridView.DoCopyCommand();
                    }
                    else
                    {
                        m.Result = CanCopy ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;

                case (int)User32.WM.PASTE:
                    if ((long)m.LParam == 0)
                    {
                        _gridView.DoPasteCommand();
                    }
                    else
                    {
                        m.Result = CanPaste ? (IntPtr)1 : (IntPtr)0;
                    }
                    return;

                case (int)User32.WM.COPYDATA:
                    GetDataFromCopyData(m.LParam);
                    m.Result = (IntPtr)1;
                    return;
                case AutomationMessages.PGM_GETBUTTONCOUNT:
                    if (_toolStrip != null)
                    {
                        m.Result = (IntPtr)_toolStrip.Items.Count;
                        return;
                    }
                    break;
                case AutomationMessages.PGM_GETBUTTONSTATE:
                    if (_toolStrip != null)
                    {
                        int index = unchecked((int)(long)m.WParam);
                        if (index >= 0 && index < _toolStrip.Items.Count)
                        {
                            if (_toolStrip.Items[index] is ToolStripButton button)
                            {
                                m.Result = (IntPtr)(button.Checked ? 1 : 0);
                            }
                            else
                            {
                                m.Result = IntPtr.Zero;
                            }
                        }
                        return;
                    }
                    break;
                case AutomationMessages.PGM_SETBUTTONSTATE:
                    if (_toolStrip != null)
                    {
                        int index = unchecked((int)(long)m.WParam);
                        if (index >= 0 && index < _toolStrip.Items.Count)
                        {
                            if (_toolStrip.Items[index] is ToolStripButton button)
                            {
                                button.Checked = !button.Checked;
                                // special treatment for the properies page button
                                if (button == _btnViewPropertyPages)
                                {
                                    OnViewButtonClickPP(button, EventArgs.Empty);
                                }
                                else
                                {
                                    switch (unchecked((int)(long)m.WParam))
                                    {
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
                    if (_toolStrip != null)
                    {
                        int index = unchecked((int)(long)m.WParam);
                        if (index >= 0 && index < _toolStrip.Items.Count)
                        {
                            string text = string.Empty;
                            if (m.Msg == AutomationMessages.PGM_GETBUTTONTEXT)
                            {
                                text = _toolStrip.Items[index].Text;
                            }
                            else
                            {
                                text = _toolStrip.Items[index].ToolTipText;
                            }

                            // write text into test file.
                            m.Result = AutomationMessages.WriteAutomationText(text);
                        }
                        return;
                    }
                    break;

                case AutomationMessages.PGM_GETTESTINGINFO:
                    {
                        // Get "testing info" string for Nth grid entry (or active entry if N < 0)
                        string testingInfo = _gridView.GetTestingInfo(unchecked((int)(long)m.WParam));
                        m.Result = AutomationMessages.WriteAutomationText(testingInfo);
                        return;
                    }

                case AutomationMessages.PGM_GETROWCOORDS:
                    if (m.Msg == dwMsg)
                    {
                        m.Result = (IntPtr)_gridView.GetPropertyLocation(propName, m.LParam == IntPtr.Zero, m.WParam == IntPtr.Zero);
                        return;
                    }
                    break;
                case AutomationMessages.PGM_GETSELECTEDROW:
                case AutomationMessages.PGM_GETVISIBLEROWCOUNT:
                    m.Result = User32.SendMessageW(_gridView, (User32.WM)m.Msg, m.WParam, m.LParam);
                    return;
                case AutomationMessages.PGM_SETSELECTEDTAB:
                    if (m.LParam != IntPtr.Zero)
                    {
                        string tabTypeName = AutomationMessages.ReadAutomationText(m.LParam);

                        for (int i = 0; i < _viewTabs.Length; i++)
                        {
                            if (_viewTabs[i].GetType().FullName == tabTypeName && _viewTabButtons[i].Visible)
                            {
                                SelectViewTabButtonDefault(_viewTabButtons[i]);
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

        internal abstract class SnappableControl : Control
        {
            protected PropertyGrid ownerGrid;
            internal bool userSized;

            public abstract int GetOptimalHeight(int width);
            public abstract int SnapHeightRequest(int request);

            public SnappableControl(PropertyGrid ownerGrid)
            {
                this.ownerGrid = ownerGrid;
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }

            public override Cursor Cursor
            {
                get
                {
                    return Cursors.Default;
                }
                set => base.Cursor = value;
            }

            protected override void OnControlAdded(ControlEventArgs ce)
            {
            }

            public Color BorderColor { get; set; } = SystemColors.ControlDark;

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Rectangle r = ClientRectangle;
                r.Width--;
                r.Height--;

                using var borderPen = BorderColor.GetCachedPenScope();
                e.Graphics.DrawRectangle(borderPen, r);
            }
        }

        public class PropertyTabCollection : ICollection
        {
            private readonly PropertyGrid _owner;

            internal PropertyTabCollection(PropertyGrid owner)
            {
                _owner = owner;
            }

            /// <summary>
            ///  Retrieves the number of member attributes.
            /// </summary>
            public int Count
            {
                get
                {
                    if (_owner is null)
                    {
                        return 0;
                    }
                    return _owner._viewTabs.Length;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Retrieves the member attribute with the specified index.
            /// </summary>
            public PropertyTab this[int index]
            {
                get
                {
                    if (_owner is null)
                    {
                        throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                    }
                    return _owner._viewTabs[index];
                }
            }

            public void AddTabType(Type propertyTabType)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.AddTab(propertyTabType, PropertyTabScope.Global);
            }

            public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.AddTab(propertyTabType, tabScope);
            }

            /// <summary>
            ///  Clears the tabs of the given scope or smaller.
            ///  tabScope must be PropertyTabScope.Component or PropertyTabScope.Document.
            /// </summary>
            public void Clear(PropertyTabScope tabScope)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.ClearTabs(tabScope);
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (_owner is null)
                {
                    return;
                }
                if (_owner._viewTabs.Length > 0)
                {
                    System.Array.Copy(_owner._viewTabs, 0, dest, index, _owner._viewTabs.Length);
                }
            }
            /// <summary>
            ///  Creates and retrieves a new enumerator for this collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                if (_owner is null)
                {
                    return Array.Empty<PropertyTab>().GetEnumerator();
                }

                return _owner._viewTabs.GetEnumerator();
            }

            public void RemoveTabType(Type propertyTabType)
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.PropertyGridPropertyTabCollectionReadOnly);
                }
                _owner.RemoveTab(propertyTabType);
            }
        }

        internal class SelectedObjectConverter : ReferenceConverter
        {
            public SelectedObjectConverter() : base(typeof(IComponent))
            {
            }
        }

        private class PropertyGridServiceProvider : IServiceProvider
        {
            private readonly PropertyGrid _owner;

            public PropertyGridServiceProvider(PropertyGrid owner)
            {
                _owner = owner;
            }

            public object GetService(Type serviceType)
            {
                object s = null;

                if (_owner.ActiveDesigner != null)
                {
                    s = _owner.ActiveDesigner.GetService(serviceType);
                }

                if (s is null)
                {
                    s = _owner._gridView.GetService(serviceType);
                }

                if (s is null && _owner.Site != null)
                {
                    s = _owner.Site.GetService(serviceType);
                }
                return s;
            }
        }

        /// <summary>
        ///  Helper class to support rendering text using either GDI or GDI+.
        /// </summary>
        internal static class MeasureTextHelper
        {
            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font)
            {
                return MeasureTextSimple(owner, g, text, font, new SizeF(0, 0));
            }

            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font, int width)
            {
                return MeasureText(owner, g, text, font, new SizeF(width, 999999));
            }

            public static SizeF MeasureTextSimple(PropertyGrid owner, Graphics g, string text, Font font, SizeF size)
            {
                SizeF bindingSize;
                if (owner.UseCompatibleTextRendering)
                {
                    bindingSize = g.MeasureString(text, font, size);
                }
                else
                {
                    bindingSize = (SizeF)TextRenderer.MeasureText(g, text, font, Size.Ceiling(size), GetTextRendererFlags());
                }

                return bindingSize;
            }

            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font, SizeF size)
            {
                SizeF bindingSize;
                if (owner.UseCompatibleTextRendering)
                {
                    bindingSize = g.MeasureString(text, font, size);
                }
                else
                {
                    TextFormatFlags flags =
                        GetTextRendererFlags() |
                        TextFormatFlags.LeftAndRightPadding |
                        TextFormatFlags.WordBreak |
                        TextFormatFlags.NoFullWidthCharacterBreak;

                    bindingSize = (SizeF)TextRenderer.MeasureText(g, text, font, Size.Ceiling(size), flags);
                }

                return bindingSize;
            }

            public static TextFormatFlags GetTextRendererFlags()
            {
                return TextFormatFlags.PreserveGraphicsClipping |
                        TextFormatFlags.PreserveGraphicsTranslateTransform;
            }
        }
    }

    internal static class AutomationMessages
    {
        internal const int PGM_GETBUTTONCOUNT = (int)User32.WM.USER + 0x50;
        internal const int PGM_GETBUTTONSTATE = (int)User32.WM.USER + 0x52;
        internal const int PGM_SETBUTTONSTATE = (int)User32.WM.USER + 0x51;
        internal const int PGM_GETBUTTONTEXT = (int)User32.WM.USER + 0x53;
        internal const int PGM_GETBUTTONTOOLTIPTEXT = (int)User32.WM.USER + 0x54;
        internal const int PGM_GETROWCOORDS = (int)User32.WM.USER + 0x55;
        internal const int PGM_GETVISIBLEROWCOUNT = (int)User32.WM.USER + 0x56;
        internal const int PGM_GETSELECTEDROW = (int)User32.WM.USER + 0x57;
        internal const int PGM_SETSELECTEDTAB = (int)User32.WM.USER + 0x58; // DO NOT CHANGE THIS : VC uses it!
        internal const int PGM_GETTESTINGINFO = (int)User32.WM.USER + 0x59;

        /// <summary>
        ///  Writes the specified text into a temporary file of the form %TEMP%\"Maui.[file id].log", where
        ///  'file id' is a unique id that is return by this method.
        ///  This is to support MAUI interaction with the PropertyGrid control and MAUI should remove the
        ///  file after used.
        /// </summary>
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
        ///  Writes the contents of a test file as text.  This file needs to have the following naming convention:
        ///  %TEMP%\"Maui.[file id].log", where 'file id' is a unique id sent to this window.
        ///  This is to support MAUI interaction with the PropertyGrid control and MAUI should create/delete this file.
        /// </summary>
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
        ///  Generate log file from id.
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

    /// <summary>
    ///  Represents the PropertyGrid accessibility object.
    ///  Is used only in Accessibility Improvements of level3 to show correct accessible hierarchy.
    /// </summary>
    internal class PropertyGridAccessibleObject : Control.ControlAccessibleObject
    {
        private readonly PropertyGrid _owningPropertyGrid;

        /// <summary>
        ///  Initializes new instance of PropertyGridAccessibleObject
        /// </summary>
        /// <param name="owningPropertyGrid">The PropertyGrid owning control.</param>
        public PropertyGridAccessibleObject(PropertyGrid owningPropertyGrid) : base(owningPropertyGrid)
        {
            _owningPropertyGrid = owningPropertyGrid;
        }

        /// <summary>
        ///  Return the child element at the specified point, if one exists,
        ///  otherwise return this element if the point is on this element,
        ///  otherwise return null.
        /// </summary>
        /// <param name="x">x coordinate of point to check</param>
        /// <param name="y">y coordinate of point to check</param>
        /// <returns>Return the child element at the specified point, if one exists,
        ///  otherwise return this element if the point is on this element,
        ///  otherwise return null.
        /// </returns>
        internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
        {
            if (!_owningPropertyGrid.IsHandleCreated)
            {
                return null;
            }

            Point clientPoint = _owningPropertyGrid.PointToClient(new Point((int)x, (int)y));

            Control element = _owningPropertyGrid.GetElementFromPoint(clientPoint);
            if (element != null)
            {
                return element.AccessibilityObject;
            }

            return base.ElementProviderFromPoint(x, y);
        }

        /// <summary>
        ///  Request to return the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    return null;
                case UiaCore.NavigateDirection.FirstChild:
                    return GetChildFragment(0);
                case UiaCore.NavigateDirection.LastChild:
                    var childFragmentCount = GetChildFragmentCount();
                    if (childFragmentCount > 0)
                    {
                        return GetChildFragment(childFragmentCount - 1);
                    }
                    break;
            }

            return base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  Request to return the element in the specified direction regarding the provided child element.
        /// </summary>
        /// <param name="childFragment">The child element regarding which the target element is searched.</param>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal UiaCore.IRawElementProviderFragment ChildFragmentNavigate(AccessibleObject childFragment, UiaCore.NavigateDirection direction)
        {
            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    return this;
                case UiaCore.NavigateDirection.NextSibling:
                    int fragmentCount = GetChildFragmentCount();
                    int childFragmentIndex = GetChildFragmentIndex(childFragment);
                    int nextChildFragmentIndex = childFragmentIndex + 1;
                    if (fragmentCount > nextChildFragmentIndex)
                    {
                        return GetChildFragment(nextChildFragmentIndex);
                    }

                    return null;
                case UiaCore.NavigateDirection.PreviousSibling:
                    fragmentCount = GetChildFragmentCount();
                    childFragmentIndex = GetChildFragmentIndex(childFragment);
                    if (childFragmentIndex > 0)
                    {
                        return GetChildFragment(childFragmentIndex - 1);
                    }

                    return null;
            }

            return null;
        }

        /// <summary>
        ///  Return the element that is the root node of this fragment of UI.
        /// </summary>
        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        ///  Gets the accessible child corresponding to the specified index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>The accessible child.</returns>
        internal AccessibleObject GetChildFragment(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (_owningPropertyGrid.ToolbarVisible)
            {
                if (index == 0)
                {
                    return _owningPropertyGrid.ToolbarAccessibleObject;
                }

                index--;
            }

            if (_owningPropertyGrid.GridViewVisible)
            {
                if (index == 0)
                {
                    return _owningPropertyGrid.GridViewAccessibleObject;
                }

                index--;
            }

            if (_owningPropertyGrid.CommandsVisible)
            {
                if (index == 0)
                {
                    return _owningPropertyGrid.HotCommandsAccessibleObject;
                }

                index--;
            }

            if (_owningPropertyGrid.HelpVisible)
            {
                if (index == 0)
                {
                    return _owningPropertyGrid.HelpAccessibleObject;
                }
            }

            return null;
        }

        /// <summary>
        ///  Gets the number of children belonging to an accessible object.
        /// </summary>
        /// <returns>The number of children.</returns>
        internal int GetChildFragmentCount()
        {
            int childCount = 0;

            if (_owningPropertyGrid.ToolbarVisible)
            {
                childCount++;
            }

            if (_owningPropertyGrid.GridViewVisible)
            {
                childCount++;
            }

            if (_owningPropertyGrid.CommandsVisible)
            {
                childCount++;
            }

            if (_owningPropertyGrid.HelpVisible)
            {
                childCount++;
            }

            return childCount;
        }

        /// <summary>
        ///  Return the element in this fragment which has the keyboard focus,
        /// </summary>
        /// <returns>Return the element in this fragment which has the keyboard focus,
        ///  if any; otherwise return null.</returns>
        internal override UiaCore.IRawElementProviderFragment GetFocus()
        {
            return GetFocused();
        }

        /// <summary>
        ///  Gets the child control index.
        /// </summary>
        /// <param name="controlAccessibleObject">The control accessible object which index should be found.</param>
        /// <returns>The child accessible index or -1 if not found.</returns>
        internal int GetChildFragmentIndex(AccessibleObject controlAccessibleObject)
        {
            int childFragmentCount = GetChildFragmentCount();
            for (int i = 0; i < childFragmentCount; i++)
            {
                AccessibleObject childFragment = GetChildFragment(i);
                if (childFragment == controlAccessibleObject)
                {
                    return i;
                }
            }

            return -1;
        }

        internal override object GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.NamePropertyId => Name,
                _ => base.GetPropertyValue(propertyID),
            };
    }

    /// <summary>
    ///  Represents the PropertyGrid inner ToolStrip control.
    ///  Is used starting with Accessibility Improvements of level 3.
    /// </summary>
    internal class PropertyGridToolStrip : ToolStrip
    {
        private readonly PropertyGrid _parentPropertyGrid;

        /// <summary>
        ///  Initializes new instance of PropertyGridToolStrip control.
        /// </summary>
        /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
        public PropertyGridToolStrip(PropertyGrid parentPropertyGrid)
        {
            _parentPropertyGrid = parentPropertyGrid;
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object for this control.</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new PropertyGridToolStripAccessibleObject(this, _parentPropertyGrid);
        }
    }

    /// <summary>
    ///  Represents the PropertyGridToolStrip control accessibility object.
    /// </summary>
    internal class PropertyGridToolStripAccessibleObject : ToolStrip.ToolStripAccessibleObject
    {
        private readonly PropertyGrid _parentPropertyGrid;

        /// <summary>
        ///  Constructs new instance of PropertyGridToolStripAccessibleObject
        /// </summary>
        /// <param name="owningPropertyGridToolStrip">The PropertyGridToolStrip owning control.</param>
        /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
        public PropertyGridToolStripAccessibleObject(PropertyGridToolStrip owningPropertyGridToolStrip, PropertyGrid parentPropertyGrid) : base(owningPropertyGridToolStrip)
        {
            _parentPropertyGrid = parentPropertyGrid;
        }

        /// <summary>
        ///  Request to return the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (_parentPropertyGrid.IsHandleCreated &&
                _parentPropertyGrid.AccessibilityObject is PropertyGridAccessibleObject propertyGridAccessibleObject)
            {
                UiaCore.IRawElementProviderFragment navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                if (navigationTarget != null)
                {
                    return navigationTarget;
                }
            }

            return base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  Request value of specified property from an element.
        /// </summary>
        /// <param name="propertyID">Identifier indicating the property to return</param>
        /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
        internal override object GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ToolBarControlTypeId,
                UiaCore.UIA.NamePropertyId => Name,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string Name
        {
            get
            {
                string name = Owner?.AccessibleName;
                if (name != null)
                {
                    return name;
                }

                return _parentPropertyGrid?.AccessibilityObject.Name;
            }
        }
    }
}
