// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;
using Microsoft.Win32;
using Windows.Win32.System.DataExchange;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.PropertyGridDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionPropertyGrid))]
public partial class PropertyGrid : ContainerControl, IComPropertyBrowser, IPropertyNotifySink.Interface
{
    private readonly HelpPane _helpPane;
    private int _helpPaneSizeRatio = -1;
    private int _commandsPaneSizeRatio = -1;
    private readonly CommandsPane _commandsPane;
    private readonly PropertyGridToolStrip _toolStrip;

    private bool _helpVisible = true;
    private bool _toolbarVisible = true;

    private ImageList? _normalButtonImages;
    private ImageList? _largeButtonImages;
    private bool _largeButtons;

    private Bitmap? _alphaBitmap;
    private Bitmap? _categoryBitmap;
    private Bitmap? _propertyPageBitmap;

    private readonly List<TabInfo> _tabs = [];
    private TabInfo? _selectedTab;
    private bool _tabsDirty = true;

    private bool _drawFlatToolBar;

    private Dictionary<string, GridEntry>? _viewTabProperties;

    // Our view type buttons (Alpha vs. categorized)
    private ToolStripButton[]? _viewSortButtons;
    private int _selectedViewSort;
    private PropertySort _propertySortValue;

    private ToolStripButton? _viewPropertyPagesButton;
    private readonly ToolStripSeparator _separator1;
    private readonly ToolStripSeparator _separator2;

    // Our main view
    private readonly PropertyGridView _gridView;

    private IDesignerHost? _designerHost;
    private IDesignerEventService? _designerEventService;

    private Dictionary<int, int>? _designerSelections;

    private GridEntry? _defaultEntry;
    private GridEntry? _rootEntry;
    private GridEntryCollection? _currentEntries;
    private object[]? _selectedObjects;

    private int _paintFrozen;
    private Color _lineColor = SystemInformation.HighContrast ? Application.SystemColors.ControlDarkDark : Application.SystemColors.InactiveBorder;
    private Color _categoryForegroundColor = Application.SystemColors.ControlText;
    private Color _categorySplitterColor = Application.SystemColors.Control;
    private Color _viewBorderColor = Application.SystemColors.ControlDark;
    private Color _selectedItemWithFocusForeColor = Application.SystemColors.HighlightText;
    private Color _selectedItemWithFocusBackColor = Application.SystemColors.Highlight;
    private bool _canShowVisualStyleGlyphs = true;

    private AttributeCollection? _browsableAttributes;

    private SnappableControl? _targetMove;
    private int _dividerMoveY = -1;
    private const int CyDivider = 3;
    private static int s_cyDivider = CyDivider;
    private const int MinGridHeight = 20;

    private const int PropertiesTabIndex = 0;
    private const int EventsTabIndex = 1;

    private const int CategorySortButtonIndex = 0;
    private const int AlphaSortButtonIndex = 1;
    private const int NoSortButtonIndex = 2;

    private const int ToolStripButtonPaddingY = 9;
    private int _toolStripButtonPaddingY = ToolStripButtonPaddingY;
    private static readonly Size s_defaultLargeButtonSize = new(32, 32);
    private static readonly Size s_defaultNormalButtonSize = new(16, 16);
    private static Size s_largeButtonSize = s_defaultLargeButtonSize;
    private static Size s_normalButtonSize = s_defaultNormalButtonSize;
    private static bool s_isScalingInitialized;

    private string? _propertyName;
    private int _copyDataMessage;

    private Flags _flags;

    private bool GetFlag(Flags flag) => (_flags & flag) != 0;

    private void SetFlag(Flags flag, bool value)
    {
        if (value)
        {
            _flags |= flag;
        }
        else
        {
            _flags &= ~flag;
        }
    }

    private readonly ComponentEventHandler _onComponentAdded;
    private readonly ComponentEventHandler _onComponentRemoved;
    private readonly ComponentChangedEventHandler _onComponentChanged;

    // The cookies for our connection points on objects that support IPropertyNotifySink
    private AxHost.ConnectionPointCookie[]? _connectionPointCookies;

    private static readonly object s_propertyValueChangedEvent = new();
    private static readonly object s_comComponentNameChangedEvent = new();
    private static readonly object s_propertyTabChangedEvent = new();
    private static readonly object s_selectedGridItemChangedEvent = new();
    private static readonly object s_propertySortChangedEvent = new();
    private static readonly object s_selectedObjectsChangedEvent = new();

    public PropertyGrid()
    {
        _onComponentAdded = OnComponentAdded;
        _onComponentRemoved = OnComponentRemoved;
        _onComponentChanged = OnComponentChanged;

        using SuspendLayoutScope layoutScope = new(this);

        // Scaling PropertyGrid but its children will be excluded from AutoScale. Please see OnLayoutInternal().
        AutoScaleMode = AutoScaleMode.Inherit;

        // Children of PropertyGrid are special and explicitly resized when propertygrid is resized (by calling OnLayoutInternal())
        // and adjust its children bounds with respect to propertygrid bounds. Autoscale mode should not scale them again.
        _doNotScaleChildren = true;

        // Static variables are problem in a child level mixed mode scenario. Changing static variables causes compatibility issues.
        // So, recalculate static variables every time property grid initialized.
        if (ScaleHelper.IsThreadPerMonitorV2Aware)
        {
            RescaleConstants();
        }
        else if (!s_isScalingInitialized)
        {
            s_normalButtonSize = LogicalToDeviceUnits(s_defaultNormalButtonSize);
            s_largeButtonSize = LogicalToDeviceUnits(s_defaultLargeButtonSize);
            s_isScalingInitialized = true;
        }

        try
        {
            _gridView = CreateGridView(serviceProvider: null);
            _gridView.TabStop = true;
            _gridView.MouseMove += OnChildMouseMove;
            _gridView.MouseDown += OnChildMouseDown;
            _gridView.TabIndex = 2;

            _separator1 = CreateSeparatorButton();
            _separator2 = CreateSeparatorButton();

            _toolStrip = new PropertyGridToolStrip(this);

            // SetupToolbar should perform the layout
            using SuspendLayoutScope suspendToolStripLayout = new(_toolStrip, performLayout: false);
            {
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

                // Parity with the old.
                _toolStrip.CanOverflow = false;

                // Hide the grip but add in a few more pixels of padding.
                _toolStrip.GripStyle = ToolStripGripStyle.Hidden;
                Padding toolStripPadding = _toolStrip.Padding;
                toolStripPadding.Left = 2;
                _toolStrip.Padding = toolStripPadding;
                SetToolStripRenderer();

                // Always add the property tab here.
                AddTab(DefaultTabType, PropertyTabScope.Static);

                _helpPane = new(this);
                using SuspendLayoutScope suspendHelpPaneLayout = new(_helpPane, performLayout: false);

                _helpPane.TabStop = false;
                _helpPane.Dock = DockStyle.None;
                _helpPane.BackColor = Application.SystemColors.Control;
                _helpPane.ForeColor = Application.SystemColors.ControlText;
                _helpPane.MouseMove += OnChildMouseMove;
                _helpPane.MouseDown += OnChildMouseDown;

                _commandsPane = new CommandsPane(this);
                using SuspendLayoutScope suspendCommandsPaneLayout = new(_commandsPane, performLayout: false);
                _commandsPane.TabIndex = 3;
                _commandsPane.Dock = DockStyle.None;
                SetHotCommandColors();
                _commandsPane.Visible = false;
                _commandsPane.MouseMove += OnChildMouseMove;
                _commandsPane.MouseDown += OnChildMouseDown;

                Controls.AddRange([_helpPane, _commandsPane, _gridView, _toolStrip]);

                SetActiveControl(_gridView);
            }

            SetupToolbar();
            PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
            SetSelectState(0);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            Debug.Fail(ex.ToString());
        }
    }

    internal IDesignerHost? ActiveDesigner
    {
        get => _designerHost ??= GetService<IDesignerHost>();
        set
        {
            if (value == _designerHost)
            {
                return;
            }

            SetFlag(Flags.ReInitTab, true);
            if (_designerHost is not null)
            {
                if (_designerHost.TryGetService(out IComponentChangeService? changeService))
                {
                    changeService.ComponentAdded -= _onComponentAdded;
                    changeService.ComponentRemoved -= _onComponentRemoved;
                    changeService.ComponentChanged -= _onComponentChanged;
                }

                if (_designerHost.TryGetService(out IPropertyValueUIService? propertyValueService))
                {
                    propertyValueService.PropertyUIValueItemsChanged -= OnNotifyPropertyValueUIItemsChanged;
                }

                _designerHost.TransactionOpened -= OnTransactionOpened;
                _designerHost.TransactionClosed -= OnTransactionClosed;
                SetFlag(Flags.BatchMode, false);

                RemoveTabs(PropertyTabScope.Document, setupToolbar: true);
                _designerHost = null;
            }

            if (value is not null)
            {
                if (value.TryGetService(out IComponentChangeService? changeService))
                {
                    changeService.ComponentAdded += _onComponentAdded;
                    changeService.ComponentRemoved += _onComponentRemoved;
                    changeService.ComponentChanged += _onComponentChanged;
                }

                value.TransactionOpened += OnTransactionOpened;
                value.TransactionClosed += OnTransactionClosed;
                SetFlag(Flags.BatchMode, false);

                if (value.TryGetService(out IPropertyValueUIService? propertyValueService))
                {
                    propertyValueService.PropertyUIValueItemsChanged += OnNotifyPropertyValueUIItemsChanged;
                }
            }

            _designerHost = value;
            if (_rootEntry is not null)
            {
                _rootEntry.DesignerHost = value;
            }

            RefreshTabs(PropertyTabScope.Document);
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
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
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
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public AttributeCollection BrowsableAttributes
    {
        get => _browsableAttributes ??= new(BrowsableAttribute.Yes);
        set
        {
            if (value is null || value == AttributeCollection.Empty)
            {
                _browsableAttributes = new(BrowsableAttribute.Yes);
            }
            else
            {
                var attributes = new Attribute[value.Count];
                value.CopyTo(attributes, 0);
                _browsableAttributes = new(attributes);
            }

            if (_selectedObjects is not null && _selectedObjects.Length > 0 && _rootEntry is not null)
            {
                _rootEntry.BrowsableAttributes = BrowsableAttributes;
                Refresh(clearCached: true);
            }
        }
    }

    private bool CanCopy => _gridView.CanCopy;

    private bool CanCut => _gridView.CanCut;

    private bool CanPaste => _gridView.CanPaste;

    private bool CanUndo => _gridView.CanUndo;

    /// <summary>
    ///  Returns true if the commands pane can be made visible for the currently selected objects.
    ///  Objects that expose verbs can show commands.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.PropertyGridCanShowCommandsDesc))]
    public virtual bool CanShowCommands => _commandsPane.WouldBeVisible;

    /// <summary>
    ///  The text used color for category headings. The background color is determined by the LineColor property.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCategoryForeColorDesc))]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color CategoryForeColor
    {
        get => _categoryForegroundColor;
        set
        {
            if (_categoryForegroundColor != value)
            {
                _categoryForegroundColor = value;
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
        get => _commandsPane.BackColor;
        set
        {
            _commandsPane.BackColor = value;
            _commandsPane.Label.BackColor = value;
        }
    }

    /// <summary>
    ///  The foreground color for the hot commands region.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCommandsForeColorDesc))]
    public Color CommandsForeColor
    {
        get => _commandsPane.ForeColor;
        set
        {
            _commandsPane.ForeColor = value;
            _commandsPane.Label.ForeColor = value;
        }
    }

    /// <summary>
    ///  The link color for the hot commands region.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCommandsLinkColorDesc))]
    public Color CommandsLinkColor
    {
        get => _commandsPane.Label.LinkColor;
        set => _commandsPane.Label.LinkColor = value;
    }

    /// <summary>
    ///  The active link color for the hot commands region.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCommandsActiveLinkColorDesc))]
    public Color CommandsActiveLinkColor
    {
        get => _commandsPane.Label.ActiveLinkColor;
        set => _commandsPane.Label.ActiveLinkColor = value;
    }

    /// <summary>
    ///  The color for the hot commands region when the link is disabled.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCommandsDisabledLinkColorDesc))]
    public Color CommandsDisabledLinkColor
    {
        get => _commandsPane.Label.DisabledLinkColor;
        set => _commandsPane.Label.DisabledLinkColor = value;
    }

    /// <summary>
    ///  The border color for the hot commands region
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCommandsBorderColorDesc))]
    [DefaultValue(typeof(Color), "ControlDark")]
    public Color CommandsBorderColor
    {
        get => _commandsPane.BorderColor;
        set => _commandsPane.BorderColor = value;
    }

    /// <summary>
    ///  Returns true if the commands pane is currently shown.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CommandsVisible => _commandsPane.Visible;

    /// <summary>
    ///  Returns true if the commands pane will be shown for objects
    ///  that expose verbs.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PropertyGridCommandsVisibleIfAvailable))]
    public virtual bool CommandsVisibleIfAvailable
    {
        get => _commandsPane.AllowVisible;
        set
        {
            bool hotcommandsVisible = _commandsPane.Visible;
            _commandsPane.AllowVisible = value;

            if (hotcommandsVisible != _commandsPane.Visible)
            {
                OnLayoutInternal(dividerOnly: false);
                _commandsPane.Invalidate();
            }
        }
    }

    /// <summary>
    ///  Returns a default location for showing the context menu. This location is the center of the active
    ///  property label in the grid, and is used useful to position the context menu when the menu is invoked
    ///  via the keyboard.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Point ContextMenuDefaultLocation => _gridView.ContextMenuDefaultLocation;

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

    protected override Size DefaultSize => new(130, 130);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected virtual Type DefaultTabType => typeof(PropertiesTab);

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="PropertyGrid"/> control paints its toolbar
    ///  with flat buttons.
    /// </summary>
    protected bool DrawFlatToolbar
    {
        get => _drawFlatToolBar;
        set
        {
            if (_drawFlatToolBar != value)
            {
                _drawFlatToolBar = value;
                SetToolStripRenderer();
            }

            SetHotCommandColors();
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
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    private bool FreezePainting
    {
        get => _paintFrozen > 0;
        set
        {
            if (value && IsHandleCreated && Visible)
            {
                if (_paintFrozen++ == 0)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)false);
                }
            }

            if (!value)
            {
                if (_paintFrozen == 0)
                {
                    return;
                }

                if (--_paintFrozen == 0)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)true);
                    Invalidate(true);
                }
            }
        }
    }

    /// <summary>
    ///  Gets the help control accessibility object.
    /// </summary>
    internal AccessibleObject HelpPaneAccessibleObject => _helpPane.AccessibilityObject;

    /// <summary>
    ///  The background color for the help region.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridHelpBackColorDesc))]
    [DefaultValue(typeof(Color), "Control")]
    public Color HelpBackColor
    {
        get => _helpPane.BackColor;
        set => _helpPane.BackColor = value;
    }

    /// <summary>
    ///  The foreground color for the help region.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridHelpForeColorDesc))]
    [DefaultValue(typeof(Color), "ControlText")]
    public Color HelpForeColor
    {
        get => _helpPane.ForeColor;
        set => _helpPane.ForeColor = value;
    }

    /// <summary>
    ///  The border color for the help region.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridHelpBorderColorDesc))]
    [DefaultValue(typeof(Color), "ControlDark")]
    public Color HelpBorderColor
    {
        get => _helpPane.BorderColor;
        set => _helpPane.BorderColor = value;
    }

    /// <summary>
    ///  Sets or gets the visibility state of the help pane.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [Localizable(true)]
    [SRDescription(nameof(SR.PropertyGridHelpVisibleDesc))]
    public virtual bool HelpVisible
    {
        get => _helpVisible;
        set
        {
            _helpVisible = value;

            _helpPane.Visible = value;
            OnLayoutInternal(dividerOnly: false);
            Invalidate();
            _helpPane.Invalidate();
        }
    }

    /// <summary>
    ///  Gets the hot commands control accessible object.
    /// </summary>
    internal AccessibleObject CommandsPaneAccessibleObject => _commandsPane.AccessibilityObject;

    /// <summary>
    ///  Gets the main entry accessible object.
    /// </summary>
    internal AccessibleObject GridViewAccessibleObject => _gridView.AccessibilityObject;

    /// <summary>
    ///  Gets the value indicating whether the main entry is visible.
    /// </summary>
    internal bool GridViewVisible => _gridView is not null && _gridView.Visible;

    /// <summary>
    ///  Background color for Highlighted text.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridSelectedItemWithFocusBackColorDesc))]
    [DefaultValue(typeof(Color), "Highlight")]
    public Color SelectedItemWithFocusBackColor
    {
        get => _selectedItemWithFocusBackColor;
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
        get => _selectedItemWithFocusForeColor;
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
        get => _gridView.GrayTextColor;
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
        get => _categorySplitterColor;
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
    ///  Gets or sets a value that indicates whether OS-specific visual style glyphs are used for the expansion
    ///  nodes in the grid area.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridCanShowVisualStyleGlyphsDesc))]
    [DefaultValue(true)]
    public bool CanShowVisualStyleGlyphs
    {
        get => _canShowVisualStyleGlyphs;
        set
        {
            if (_canShowVisualStyleGlyphs != value)
            {
                _canShowVisualStyleGlyphs = value;
                _gridView.Invalidate();
            }
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridLineColorDesc))]
    [DefaultValue(typeof(Color), "InactiveBorder")]
    public Color LineColor
    {
        get => _lineColor;
        set
        {
            if (_lineColor != value)
            {
                _lineColor = value;
                HasCustomLineColor = true;
                _gridView.Invalidate();
            }
        }
    }

    internal bool HasCustomLineColor { get; private set; }

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
    public new event EventHandler? PaddingChanged
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
        get => _propertySortValue;
        set
        {
            // Valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            ToolStripButton newButton;

            if ((value & PropertySort.Categorized) != 0)
            {
                newButton = _viewSortButtons![CategorySortButtonIndex];
            }
            else if ((value & PropertySort.Alphabetical) != 0)
            {
                newButton = _viewSortButtons![AlphaSortButtonIndex];
            }
            else
            {
                newButton = _viewSortButtons![NoSortButtonIndex];
            }

            GridItem? selectedGridItem = SelectedGridItem;

            OnViewSortButtonClick(newButton, EventArgs.Empty);

            _propertySortValue = value;

            if (selectedGridItem is not null)
            {
                try
                {
                    SelectedGridItem = selectedGridItem;
                }
                catch (ArgumentException)
                {
                    // When no row is selected, SelectedGridItem returns the grid entry for the root
                    // object. But this is not a selectable item. So don't worry if setting SelectedGridItem
                    // causes an argument exception when trying to re-select the root object. Just leave the
                    // the grid with no selected row.
                }
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PropertyTabCollection PropertyTabs => new(this);

    /// <summary>
    ///  Sets a single object into the grid to be browsed. If multiple objects are being browsed, this property
    ///  will return the first one in the list. If no objects are selected, null is returned.
    /// </summary>
    [DefaultValue(null)]
    [SRDescription(nameof(SR.PropertyGridSelectedObjectDesc))]
    [SRCategory(nameof(SR.CatBehavior))]
    [TypeConverter(typeof(SelectedObjectConverter))]
    public object? SelectedObject
    {
        get => _selectedObjects is null || _selectedObjects.Length == 0 ? null : _selectedObjects[0];
        set => SelectedObjects = value is null ? [] : ([value]);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public object[] SelectedObjects
    {
        get => _selectedObjects is null ? [] : (object[])_selectedObjects.Clone();
        set
        {
            using FreezePaintScope _ = new(this);

            SetFlag(Flags.FullRefreshAfterBatch, false);
            if (GetFlag(Flags.BatchMode))
            {
                SetFlag(Flags.BatchModeChange, false);
            }

            _gridView.EnsurePendingChangesCommitted();

            bool isSame = false;
            bool classesSame = false;
            bool showEvents = true;

            // Validate the array coming in.
            if (value is not null && value.Length > 0)
            {
                for (int count = 0; count < value.Length; count++)
                {
                    if (value[count] is null)
                    {
                        throw new ArgumentException(string.Format(SR.PropertyGridSetNull, count, value.Length));
                    }
                }
            }
            else
            {
                // Removing binding, no need to show events.
                showEvents = false;
            }

            // Make sure we actually changed something before we inspect tabs.
            if (_selectedObjects is not null
                && value is not null
                && _selectedObjects.Length == value.Length)
            {
                isSame = true;
                classesSame = true;

                for (int i = 0; i < value.Length && (isSame || classesSame); i++)
                {
                    if (isSame && _selectedObjects[i] != value[i])
                    {
                        isSame = false;
                    }

                    if (!classesSame)
                    {
                        continue;
                    }

                    // We have already done the same checks as inside GetUnwrappedObject that return null.
                    Type oldType = GetUnwrappedObject(i)!.GetType();

                    object newObject = value[i];

                    if (newObject is ICustomTypeDescriptor descriptor)
                    {
                        newObject = descriptor.GetPropertyOwner(pd: null)!;
                    }

                    Type newType = newObject.GetType();

                    // Check if the types are the same. If they are COM objects assume the classes are different.
                    if (classesSame && (oldType != newType || (oldType.IsCOMObject && newType.IsCOMObject)))
                    {
                        classesSame = false;
                    }
                }
            }

            if (!isSame)
            {
                // Ensure we've hooked ActiveDesignerChanged if possible.
                if (!GetFlag(Flags.GotDesignerEventService) && TryGetService(out _designerEventService))
                {
                    SetFlag(Flags.GotDesignerEventService, true);
                    _designerEventService.ActiveDesignerChanged += OnActiveDesignerChanged;
                    OnActiveDesignerChanged(sender: null, new(oldDesigner: null, _designerEventService.ActiveDesigner));
                }

                showEvents = showEvents && GetFlag(Flags.GotDesignerEventService);

                SetStatusBox(string.Empty, string.Empty);

                ClearCachedProperties();

                // The default selected entry might still reference the previous selected
                // objects. Set it to null to avoid leaks.
                _defaultEntry = null;

                _selectedObjects = value is null ? [] : (object[])value.Clone();

                SinkPropertyNotifyEvents();
                SetFlag(Flags.PropertiesChanged, true);

                // Since we are changing the selection, we need to make sure that the
                // keywords for the currently selected grid entry get removed.
                try
                {
                    _gridView?.RemoveSelectedEntryHelpAttributes();
                }
                catch (COMException)
                {
                    // TypeResolutionService is needed to access the HelpKeyword. However,
                    // TypeResolutionService might be disposed when project is closing. We
                    // need swallow the exception in this case.
                }

                _rootEntry?.Dispose();
                _rootEntry = null;

                if (!classesSame && !GetFlag(Flags.TabsChanging))
                {
                    // Throw away any extra component only tabs.

                    Type? tabType = _selectedTab?.TabType;
                    ToolStripButton? viewTabButton = null;
                    RefreshTabs(PropertyTabScope.Component);
                    EnableTabs();

                    if (tabType is not null)
                    {
                        foreach (var tab in _tabs)
                        {
                            if (tab.TabType == tabType && tab.Button.Visible)
                            {
                                viewTabButton = tab.Button;
                                break;
                            }
                        }
                    }

                    SelectViewTabButtonDefault(viewTabButton);
                }

                // Make sure we've also got events on all the objects.
                if (showEvents && _tabs.Count > EventsTabIndex
                    && _tabs[EventsTabIndex] is { } eventTab && eventTab.Tab is EventsTab)
                {
                    showEvents = eventTab.Button.Visible;
                    var attributes = new Attribute[BrowsableAttributes.Count];
                    BrowsableAttributes.CopyTo(attributes, 0);

                    // Used to avoid looking up events on the same type multiple times.
                    HashSet<Type>? typesWithEvents = _selectedObjects.Length > 10 ? new() : null;

                    for (int i = 0; i < _selectedObjects.Length && showEvents; i++)
                    {
                        object currentObject = _selectedObjects[i];

                        if (currentObject is ICustomTypeDescriptor descriptor)
                        {
                            currentObject = descriptor.GetPropertyOwner(pd: null)!;
                        }

                        Type objectType = currentObject.GetType();

                        if (typesWithEvents is not null && typesWithEvents.Contains(objectType))
                        {
                            continue;
                        }

                        // Only show events if all objects have events and are sited.
                        showEvents = showEvents
                            && currentObject is IComponent component
                            && component.Site is not null
                            // Use the original (possibly ICustomTypeDescriptor) to check whether there are
                            // properties as this is what what we'll actually use to fill the events tab.
                            && (eventTab.Tab.GetProperties(_selectedObjects[i], attributes)?.Count ?? 0) > 0;

                        if (showEvents)
                        {
                            typesWithEvents?.Add(objectType);
                        }
                    }
                }

                ShowEventsButton(showEvents && _selectedObjects.Length > 0);
                DisplayCommandsPane();
                EnablePropPageButton(_selectedObjects.Length == 1 ? _selectedObjects[0] : null);
                OnSelectedObjectsChanged(EventArgs.Empty);
            }

            if (GetFlag(Flags.TabsChanging))
            {
                return;
            }

            // If you select an events tab for your designer and double click to go to code, it should
            // be the events tab when you get back to the designer. Check for that state here and
            // make sure we select and refresh that tab when we load.
            if (_selectedObjects!.Length > 0 && GetFlag(Flags.ReInitTab))
            {
                object? designerKey = ActiveDesigner;

                // Get the active designer and see if we've stashed away state for it.
                if (TryGetSavedTabIndex(out int selectedTab))
                {
                    if (selectedTab < _tabs.Count && (selectedTab == PropertiesTabIndex || _tabs[selectedTab].Button.Visible))
                    {
                        SelectViewTabButton(_tabs[selectedTab].Button, updateSelection: true);
                    }
                }
                else
                {
                    Refresh(clearCached: false);
                }

                SetFlag(Flags.ReInitTab, false);
            }
            else
            {
                Refresh(clearCached: true);
            }

            if (_selectedObjects.Length > 0)
            {
                SaveSelectedTabIndex();
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PropertyTab SelectedTab
    {
        get
        {
            Debug.Assert(_selectedTab is not null, "Invalid tab selection!");
            return _selectedTab?.Tab!;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DisallowNull]
    public GridItem? SelectedGridItem
    {
        get => _gridView.SelectedGridEntry ?? _rootEntry;
        set => _gridView.SelectedGridEntry = (GridEntry)value;
    }

    protected internal override bool ShowFocusCues => true;

    public override ISite? Site
    {
        get => base.Site;
        set
        {
            // Perf - the base class is possibly going to change the font via ambient properties service
            SuspendAllLayout(this);

            base.Site = value;
            _gridView.ServiceProvider = value;

            ActiveDesigner = value?.GetService<IDesignerHost>();

            ResumeAllLayout(this, performLayout: true);
        }
    }

    /// <summary>
    ///  Gets the value indicating whether the Property grid is sorted by categories.
    /// </summary>
    internal bool SortedByCategories => (PropertySort & PropertySort.Categorized) != 0;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.PropertyGridLargeButtonsDesc))]
    [DefaultValue(false)]
    public bool LargeButtons
    {
        get => _largeButtons;
        set
        {
            if (value == _largeButtons)
            {
                return;
            }

            _largeButtons = value;
            if (value)
            {
                EnsureLargeButtons();
                if (_largeButtonImages is not null)
                {
                    _toolStrip.ImageScalingSize = _largeButtonImages.ImageSize;
                }

                _toolStrip.ImageList = _largeButtonImages;
            }
            else
            {
                if (_normalButtonImages is not null)
                {
                    _toolStrip.ImageScalingSize = _normalButtonImages.ImageSize;
                }

                _toolStrip.ImageList = _normalButtonImages;
            }

            OnLayoutInternal(dividerOnly: false);
            Invalidate();
            _toolStrip.Invalidate();
        }
    }

    /// <summary>
    ///  Gets the toolbar control accessibility object.
    /// </summary>
    internal AccessibleObject ToolbarAccessibleObject => _toolStrip.AccessibilityObject;

    /// <summary>
    ///  Sets or gets the visibility state of the toolStrip.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PropertyGridToolbarVisibleDesc))]
    public virtual bool ToolbarVisible
    {
        get => _toolbarVisible;
        set
        {
            _toolbarVisible = value;

            _toolStrip.Visible = value;
            OnLayoutInternal(dividerOnly: false);
            if (value)
            {
                SetupToolbar(fullRebuild: _tabsDirty);
            }

            Invalidate();
            _toolStrip.Invalidate();
        }
    }

    protected ToolStripRenderer? ToolStripRenderer
    {
        get => _toolStrip?.Renderer;
        set
        {
            if (_toolStrip is not null)
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
        get => _gridView.BackColor;
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
        get => _gridView.ForeColor;
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
        get => _viewBorderColor;
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

        image = ScaleHelper.CopyAndScaleToSize(image, s_normalButtonSize);
        int result = _normalButtonImages!.Images.Count;
        _normalButtonImages.Images.Add(image);

        return result;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event KeyEventHandler? KeyDown
    {
        add => base.KeyDown += value;
        remove => base.KeyDown -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => base.KeyPress += value;
        remove => base.KeyPress -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event KeyEventHandler? KeyUp
    {
        add => base.KeyUp += value;
        remove => base.KeyUp -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseDown
    {
        add => base.MouseDown += value;
        remove => base.MouseDown -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseUp
    {
        add => base.MouseUp += value;
        remove => base.MouseUp -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseMove
    {
        add => base.MouseMove += value;
        remove => base.MouseMove -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? MouseEnter
    {
        add => base.MouseEnter += value;
        remove => base.MouseEnter -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? MouseLeave
    {
        add => base.MouseLeave += value;
        remove => base.MouseLeave -= value;
    }

    /// <summary>
    ///  Event that is fired when a property value is modified.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.PropertyGridPropertyValueChangedDescr))]
    public event PropertyValueChangedEventHandler? PropertyValueChanged
    {
        add => Events.AddHandler(s_propertyValueChangedEvent, value);
        remove => Events.RemoveHandler(s_propertyValueChangedEvent, value);
    }

    event ComponentRenameEventHandler? IComPropertyBrowser.ComComponentNameChanged
    {
        add => Events.AddHandler(s_comComponentNameChangedEvent, value);
        remove => Events.RemoveHandler(s_comComponentNameChangedEvent, value);
    }

    /// <summary>
    ///  Event that is fired when the current view tab is changed, such as changing from Properties to Events.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.PropertyGridPropertyTabchangedDescr))]
    public event PropertyTabChangedEventHandler? PropertyTabChanged
    {
        add => Events.AddHandler(s_propertyTabChangedEvent, value);
        remove => Events.RemoveHandler(s_propertyTabChangedEvent, value);
    }

    /// <summary>
    ///  Event that is fired when the sort mode is changed.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.PropertyGridPropertySortChangedDescr))]
    public event EventHandler? PropertySortChanged
    {
        add => Events.AddHandler(s_propertySortChangedEvent, value);
        remove => Events.RemoveHandler(s_propertySortChangedEvent, value);
    }

    /// <summary>
    ///  Event that is fired when the selected <see cref="GridItem"/> is changed.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.PropertyGridSelectedGridItemChangedDescr))]
    public event SelectedGridItemChangedEventHandler? SelectedGridItemChanged
    {
        add => Events.AddHandler(s_selectedGridItemChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedGridItemChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.PropertyGridSelectedObjectsChangedDescr))]
    public event EventHandler? SelectedObjectsChanged
    {
        add => Events.AddHandler(s_selectedObjectsChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedObjectsChangedEvent, value);
    }

    internal void AddTab(
        Type tabType,
        PropertyTabScope scope,
        object? @object = null,
        bool setupToolbar = true)
    {
        PropertyTab? tab = null;
        int tabIndex = -1;

        // Check to see if we've already got a tab of this type.
        for (int i = 0; i < _tabs.Count; i++)
        {
            Debug.Assert(_tabs[i] is not null, "Null item in tab array!");
            if (tabType == _tabs[i].Tab.GetType())
            {
                tab = _tabs[i].Tab;
                tabIndex = i;
                break;
            }
        }

        if (tab is null)
        {
            // The tabs need service providers. The one we hold onto is not good enough,
            // so try to get the one off of the component's site.
            IDesignerHost? host = null;
            if (@object is IComponent component && component.Site is ISite site)
            {
                host = site.GetService<IDesignerHost>();
            }

            tab = CreateTab(tabType, host);

            if (tab is null)
            {
                return;
            }

            // Default to inserting at the end.
            tabIndex = _tabs.Count;

            // Find the insertion position. Special case for event's and properties.
            if (tabType == DefaultTabType)
            {
                tabIndex = PropertiesTabIndex;
            }
            else if (typeof(EventsTab).IsAssignableFrom(tabType))
            {
                tabIndex = EventsTabIndex;
            }
            else
            {
                // Order tabs alphabetically, we've always got a property tab, so start after that.
                for (int i = 1; i < _tabs.Count; i++)
                {
                    var current = _tabs[i].Tab;

                    // Skip the event tab.
                    if (current is EventsTab)
                    {
                        continue;
                    }

                    if (string.Compare(tab.TabName, current.TabName, ignoreCase: false, CultureInfo.InvariantCulture) < 0)
                    {
                        tabIndex = i;
                        break;
                    }
                }
            }

            ToolStripButton button = CreatePushButton(
                tab.TabName,
                imageIndex: -1,
                OnViewTabButtonClick,
                useRadioButtonRole: true);

            _tabs.Insert(tabIndex, new(tab, scope, button));
            _tabsDirty = true;
        }

        if (tab is not null && @object is not null)
        {
            try
            {
                object[]? tabComponents = tab.Components;
                int oldArraySize = tabComponents is null ? 0 : tabComponents.Length;

                object[] newComponents = new object[oldArraySize + 1];
                if (tabComponents is not null)
                {
                    Array.Copy(tabComponents, newComponents, oldArraySize);
                }

                newComponents[oldArraySize] = @object;
                tab.Components = newComponents;
            }
            catch (Exception e)
            {
                Debug.Fail("Bad tab. We're going to remove it.", e.ToString());
                RemoveTab(tabIndex, setupToolbar: false);
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
        => _gridView.RecursivelyExpand(_rootEntry, initialize: false, expand: false, maxExpands: -1);

    private void ClearCachedProperties() => _viewTabProperties?.Clear();

    internal void ClearCachedValues() => _rootEntry?.ClearCachedValues();

    /// <summary>
    ///  Clears the tabs of the given <paramref name="tabScope"/> or smaller.
    /// </summary>
    /// <param name="tabScope">
    ///  Must be <see cref="PropertyTabScope.Component" /> or <see cref="PropertyTabScope.Document"/>
    /// </param>
    internal void ClearTabs(PropertyTabScope tabScope)
    {
        if (tabScope < PropertyTabScope.Document)
        {
            throw new ArgumentException(SR.PropertyGridTabScope, nameof(tabScope));
        }

        RemoveTabs(tabScope, true);
    }

#if DEBUG
    internal bool _inGridViewCreate;
#endif

    /// <summary>
    ///  Constructs the new instance of the accessibility object for current PropertyGrid control.
    /// </summary>
    protected override AccessibleObject CreateAccessibilityInstance() => new PropertyGridAccessibleObject(this);

    private PropertyGridView CreateGridView(IServiceProvider? serviceProvider)
    {
#if DEBUG
        try
        {
            _inGridViewCreate = true;
#endif
            return new PropertyGridView(serviceProvider, this);
#if DEBUG
        }
        finally
        {
            _inGridViewCreate = false;
        }
#endif
    }

    [Conditional("DEBUG")]
#pragma warning disable CA1822 // Mark members as static
    internal void CheckInCreate()
#pragma warning restore CA1822 // Mark members as static
    {
#if DEBUG
        if (_inGridViewCreate)
        {
            _inGridViewCreate = false;
            throw new InvalidOperationException("PERF REGRESSION - Creating item in grid view create");
        }
#endif
    }

    private static ToolStripSeparator CreateSeparatorButton()
    {
        ToolStripSeparator button = new();
        return button;
    }

    protected virtual PropertyTab? CreatePropertyTab(Type tabType) => null;

    private PropertyTab? CreateTab(Type tabType, IDesignerHost? host)
    {
        PropertyTab? tab;

        try
        {
            tab = CreatePropertyTab(tabType);
        }
        catch (Exception exception)
        {
            Debug.Fail($"{nameof(CreatePropertyTab)} failed. {exception.Message}");
            return null;
        }

        if (tab is null)
        {
            ConstructorInfo? constructor = tabType.GetConstructor([typeof(IServiceProvider)]);
            object? parameter = null;
            if (constructor is null)
            {
                // Try a IDesignerHost constructor.
                constructor = tabType.GetConstructor([typeof(IDesignerHost)]);

                if (constructor is not null)
                {
                    parameter = host;
                }
            }
            else
            {
                parameter = Site;
            }

            try
            {
                if (parameter is not null && constructor is not null)
                {
                    tab = (PropertyTab)constructor.Invoke([parameter]);
                }
                else
                {
                    // Just call the default constructor.
                    tab = (PropertyTab)Activator.CreateInstance(tabType)!;
                }
            }
            catch (Exception exception)
            {
                Debug.Fail($"Failed to create {nameof(PropertyTab)}. {exception.Message}");
                tab = null;
            }
        }

        if (tab is not null)
        {
            if (tab.Bitmap is null)
            {
                throw new ArgumentException(string.Format(SR.PropertyGridNoBitmap, tab.GetType().FullName));
            }

            if (string.IsNullOrEmpty(tab.TabName))
            {
                throw new ArgumentException(string.Format(SR.PropertyGridTabName, tab.GetType().FullName));
            }
        }

        return tab;
    }

    private PropertyGridToolStripButton CreatePushButton(
        string? toolTipText,
        int imageIndex,
        EventHandler eventHandler,
        bool useRadioButtonRole = false)
    {
        PropertyGridToolStripButton button = new(this, useRadioButtonRole)
        {
            Text = toolTipText,
            AutoToolTip = true,
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            ImageIndex = imageIndex
        };

        button.Click += eventHandler;
        button.ImageScaling = ToolStripItemImageScaling.SizeToFit;

        if (useRadioButtonRole)
        {
            // As discussed in https://github.com/dotnet/winforms/issues/4428 issue, set the accessible role
            // to "RadioButton" instead of "CheckBox" as it better matches the behavior of the button.
            button.AccessibleRole = AccessibleRole.RadioButton;
        }

        return button;
    }

    private void DisplayCommandsPane()
    {
        bool commandsPaneDisplayed = _commandsPane.Visible;

        IComponent? component = null;
        DesignerVerb[]? verbs = null;

        // We favor the menu command service, since it can give us verbs.
        // If we fail that, we will go straight to the designer.
        if (_selectedObjects is not null && _selectedObjects.Length > 0)
        {
            for (int i = 0; i < _selectedObjects.Length; i++)
            {
                if (GetUnwrappedObject(i) is IComponent iComponent)
                {
                    component = iComponent;
                    break;
                }
            }

            if (component?.Site is ISite site)
            {
                if (site.TryGetService(out IMenuCommandService? menuCommandService))
                {
                    // Got the menu command service.  Let it deal with the set of verbs for this component.
                    verbs = new DesignerVerb[menuCommandService.Verbs.Count];
                    menuCommandService.Verbs.CopyTo(verbs, 0);
                }
                else
                {
                    // No menu command service.  Go straight to the component's designer.  We can only do this
                    // if the object count is 1, because designers do not support verbs across a multi-selection.
                    if (_selectedObjects.Length == 1 && site.TryGetService(out IDesignerHost? designerHost))
                    {
                        IDesigner? designer = designerHost.GetDesigner(component);
                        if (designer?.Verbs is not null)
                        {
                            verbs = new DesignerVerb[designer.Verbs.Count];
                            designer.Verbs.CopyTo(verbs, 0);
                        }
                    }
                }
            }
        }

        // Don't show verbs if a property grid is on the form at design time.
        if (!DesignMode)
        {
            if (verbs is not null && verbs.Length > 0)
            {
                _commandsPane.SetVerbs(component, verbs);
            }
            else
            {
                _commandsPane.SetVerbs(component: null, verbs: null);
            }

            if (commandsPaneDisplayed != _commandsPane.Visible)
            {
                OnLayoutInternal(dividerOnly: false);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unhook IDesignerEventService.ActiveDesignerChanged event
            if (GetFlag(Flags.GotDesignerEventService))
            {
                Debug.Assert(
                    _designerEventService is not null,
                    "GetFlag(GotDesignerEventService) inconsistent with designerEventService == null");

                if (_designerEventService is not null)
                {
                    _designerEventService.ActiveDesignerChanged -= OnActiveDesignerChanged;
                }

                _designerEventService = null;
                SetFlag(Flags.GotDesignerEventService, false);
            }

            ActiveDesigner = null;

            foreach (var tabInfo in _tabs)
            {
                tabInfo.Tab.Dispose();
            }

            _tabs.Clear();

            _normalButtonImages?.Dispose();
            _normalButtonImages = null;

            _largeButtonImages?.Dispose();
            _largeButtonImages = null;

            _alphaBitmap?.Dispose();
            _alphaBitmap = null;

            _categoryBitmap?.Dispose();
            _categoryBitmap = null;

            _propertyPageBitmap?.Dispose();
            _propertyPageBitmap = null;

            _rootEntry?.Dispose();
            _rootEntry = null;

            if (_selectedObjects is not null)
            {
                _selectedObjects = null;
                SinkPropertyNotifyEvents();
            }

            ClearCachedProperties();
            _currentEntries = null;
        }

        base.Dispose(disposing);
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        if (_viewTabProperties?.Count > 0)
        {
            foreach (GridEntry gridEntry in _viewTabProperties.Values)
            {
                gridEntry.ReleaseUiaProvider();
            }
        }

        _helpPane?.ReleaseUiaProvider(HWND.Null);
        _commandsPane?.ReleaseUiaProvider(HWND.Null);
        _toolStrip?.ReleaseUiaProvider(HWND.Null);
        _rootEntry?.ReleaseUiaProvider();

        base.ReleaseUiaProvider(handle);
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

    private SnappableControl? DividerInside(int y)
    {
        int useGrid = -1;

        if (_commandsPane.Visible)
        {
            Point location = _commandsPane.Location;
            if (y >= (location.Y - s_cyDivider) &&
                y <= (location.Y + 1))
            {
                return _commandsPane;
            }

            useGrid = 0;
        }

        if (_helpPane.Visible)
        {
            Point location = _helpPane.Location;
            if (y >= (location.Y - s_cyDivider) &&
                y <= (location.Y + 1))
            {
                return _helpPane;
            }

            if (useGrid == -1)
            {
                useGrid = 1;
            }
        }

        // Also the bottom line of the grid.
        if (useGrid != -1)
        {
            int gridTop = _gridView.Location.Y;
            int gridBottom = gridTop + _gridView.Size.Height;

            if (Math.Abs(gridBottom - y) <= 1 && y > gridTop)
            {
                switch (useGrid)
                {
                    case 0:
                        return _commandsPane;
                    case 1:
                        return _helpPane;
                }
            }
        }

        return null;
    }

    private int DividerLimitHigh(SnappableControl target)
    {
        int high = _gridView.Location.Y + MinGridHeight;
        if (target == _helpPane && _commandsPane.Visible)
        {
            high += _commandsPane.Size.Height + 2;
        }

        return high;
    }

    private int DividerLimitMove(SnappableControl target, int y)
    {
        Rectangle rectTarget = target.Bounds;

        // Make sure we're not going to make ourselves zero height -- make 15 the min size
        y = Math.Min((rectTarget.Y + rectTarget.Height - 15), y);

        // Make sure we're not going to make ourselves cover up the grid
        return Math.Max(DividerLimitHigh(target), y);
    }

    private static void DrawXorBar(Control targetControl, Rectangle rcFrame)
    {
        Rectangle rc = targetControl.RectangleToScreen(rcFrame);

        if (rc.Width < rc.Height)
        {
            for (int i = 0; i < rc.Width; i++)
            {
                ControlPaint.DrawReversibleLine(new Point(rc.X + i, rc.Y), new Point(rc.X + i, rc.Y + rc.Height), targetControl.BackColor);
            }
        }
        else
        {
            for (int i = 0; i < rc.Height; i++)
            {
                ControlPaint.DrawReversibleLine(new Point(rc.X, rc.Y + i), new Point(rc.X + rc.Width, rc.Y + i), targetControl.BackColor);
            }
        }
    }

    private bool EnablePropPageButton(object? obj)
    {
        if (_viewPropertyPagesButton is null)
        {
            throw new InvalidOperationException();
        }

        if (obj is null)
        {
            _viewPropertyPagesButton.Enabled = false;
            return false;
        }

        bool enable;

        if (TryGetService(out IUIService? uiService))
        {
            enable = uiService.CanShowComponentEditor(obj);
        }
        else
        {
            enable = TypeDescriptor.GetEditor(obj, typeof(ComponentEditor)) is not null;
        }

        _viewPropertyPagesButton.Enabled = enable;
        return enable;
    }

    private void EnableTabs()
    {
        if (_selectedObjects is null)
        {
            return;
        }

        // Make sure our toolbars are okay.
        SetupToolbar();

        // Walk through the current tabs to validate that they apply to all currently selected objects.
        // Skip the property tab since it's always valid.
        foreach (var tab in _tabs)
        {
            bool canExtend = true;

            // Make sure the tab is valid for all objects.
            for (int j = 0; j < _selectedObjects.Length; j++)
            {
                try
                {
                    if (!tab.Tab.CanExtend(GetUnwrappedObject(j)))
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

            if (canExtend != tab.Button.Visible)
            {
                tab.Button.Visible = canExtend;
                if (!canExtend && tab == _selectedTab)
                {
                    SelectViewTabButton(_tabs[PropertiesTabIndex].Button, updateSelection: true);
                }
            }
        }
    }

    private void EnsureLargeButtons()
    {
        if (_largeButtonImages is not null)
        {
            return;
        }

        _largeButtonImages = new ImageList
        {
            ImageSize = s_largeButtonSize
        };

        if (ScaleHelper.IsScalingRequired)
        {
            AddLargeImage(_alphaBitmap);
            AddLargeImage(_categoryBitmap);

            foreach (var tab in _tabs)
            {
                AddLargeImage(tab.Tab.Bitmap);
            }

            AddLargeImage(_propertyPageBitmap);
        }
        else
        {
            if (_normalButtonImages is null)
            {
                throw new InvalidOperationException();
            }

            ImageList.ImageCollection images = _normalButtonImages.Images;

            for (int i = 0; i < images.Count; i++)
            {
                if (images[i] is Bitmap bitmap)
                {
                    _largeButtonImages.Images.Add(new Bitmap(bitmap, s_largeButtonSize.Width, s_largeButtonSize.Height));
                }
            }
        }
    }

    // This method should be called only inside a if (DpiHelper.IsScalingRequired) clause.
    private void AddLargeImage(Bitmap? originalBitmap)
    {
        if (originalBitmap is null)
        {
            return;
        }

        try
        {
            Bitmap largeBitmap = ScaleHelper.CopyAndScaleToSize(originalBitmap, s_largeButtonSize);
            _largeButtonImages!.Images.Add(largeBitmap);
        }
        catch (Exception ex)
        {
            Debug.Fail($"Failed to add a large property grid toolstrip button, {ex}");
        }
    }

    bool IComPropertyBrowser.EnsurePendingChangesCommitted()
    {
        // The commits sometimes cause transactions to open and close, which will cause refreshes,
        // which we want to ignore.

        try
        {
            if (_designerHost is not null)
            {
                _designerHost.TransactionOpened -= OnTransactionOpened;
                _designerHost.TransactionClosed -= OnTransactionClosed;
            }

            return _gridView.EnsurePendingChangesCommitted();
        }
        finally
        {
            if (_designerHost is not null)
            {
                _designerHost.TransactionOpened += OnTransactionOpened;
                _designerHost.TransactionClosed += OnTransactionClosed;
            }
        }
    }

    public void ExpandAllGridItems()
        => _gridView.RecursivelyExpand(_rootEntry, initialize: false, expand: true, PropertyGridView.MaxRecurseExpand);

    private static IEnumerable<Type> GetCommonTabs(object[] components, PropertyTabScope tabScope)
    {
        if (components is null || components.Length == 0)
        {
            return Array.Empty<Type>();
        }

        if (!TypeDescriptorHelper.TryGetAttribute(components[0], out PropertyTabAttribute? tabAttribute))
        {
            return Array.Empty<Type>();
        }

        List<Type> tabClasses = [];

        // Find all tab types that match the requested scope.
        for (int i = 0; i < tabAttribute.TabScopes.Length; i++)
        {
            if (tabAttribute.TabScopes[i] == tabScope)
            {
                tabClasses.Add(tabAttribute.TabClasses[i]);
            }
        }

        for (int i = 1; i < components.Length && tabClasses.Count > 0; i++)
        {
            if (!TypeDescriptorHelper.TryGetAttribute(components[i], out tabAttribute))
            {
                // If the current item has no tabs at all, we can fail right now.
                return Array.Empty<Type>();
            }

            // Remove any tab classes we can't find a match for.
            for (int j = tabClasses.Count - 1; j >= 0; j--)
            {
                if (!tabAttribute.TabClasses.Contains(tabClasses[j]))
                {
                    tabClasses.RemoveAt(j);
                }
            }
        }

        return tabClasses;
    }

    internal GridEntry? GetDefaultGridEntry() => _defaultEntry ??= _currentEntries?[0];

    /// <summary>
    ///  Gets the element from point.
    /// </summary>
    /// <param name="point">The point where to search the element.</param>
    /// <returns>The element found in the current point.</returns>
    internal Control? GetElementFromPoint(Point point)
    {
        if (ToolbarAccessibleObject.Bounds.Contains(point))
        {
            return _toolStrip;
        }

        if (GridViewAccessibleObject.Bounds.Contains(point))
        {
            return _gridView;
        }

        if (CommandsPaneAccessibleObject.Bounds.Contains(point))
        {
            return _commandsPane;
        }

        if (HelpPaneAccessibleObject.Bounds.Contains(point))
        {
            return _helpPane;
        }

        return null;
    }

    private object? GetUnwrappedObject(int index)
    {
        if (_selectedObjects is null || index < 0 || index > _selectedObjects.Length)
        {
            return null;
        }

        object @object = _selectedObjects[index];
        return @object is ICustomTypeDescriptor descriptor
            ? descriptor.GetPropertyOwner(pd: null)
            : @object;
    }

    internal GridEntryCollection? GetCurrentEntries()
    {
        if (_currentEntries is null)
        {
            UpdateSelection();
        }

        SetFlag(Flags.PropertiesChanged, false);
        return _currentEntries;
    }

    internal bool HavePropertyEntriesChanged() => GetFlag(Flags.PropertiesChanged);

    bool IComPropertyBrowser.InPropertySet => _gridView.InPropertySet;

    void IComPropertyBrowser.DropDownDone() => _gridView.CloseDropDown();

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

    void IComPropertyBrowser.SaveState(RegistryKey? key)
    {
        if (key is null)
        {
            return;
        }

        key.SetValue(RegistryStateNames.AlphabeticalSort, PropertySort == PropertySort.Alphabetical ? "1" : "0");
        key.SetValue(RegistryStateNames.HelpVisible, HelpVisible ? "1" : "0");
        key.SetValue(RegistryStateNames.CommandsVisible, CommandsVisibleIfAvailable ? "1" : "0");
        key.SetValue(RegistryStateNames.CommentSizeRatio, _helpPaneSizeRatio.ToString(CultureInfo.InvariantCulture));
        key.SetValue(RegistryStateNames.CommandSizeRatio, _commandsPaneSizeRatio.ToString(CultureInfo.InvariantCulture));
    }

    void IComPropertyBrowser.LoadState(RegistryKey? key)
    {
        if (key is null)
        {
            // Apply the same defaults from above.
            PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
            HelpVisible = true;
            CommandsVisibleIfAvailable = false;
        }
        else
        {
            object value = key.GetValue(RegistryStateNames.AlphabeticalSort, "0");

            PropertySort = value is not null && value.ToString()!.Equals("1")
                ? PropertySort.Alphabetical
                : PropertySort.Categorized | PropertySort.Alphabetical;

            value = key.GetValue(RegistryStateNames.HelpVisible, "1");
            HelpVisible = value is not null && value.ToString()!.Equals("1");

            value = key.GetValue(RegistryStateNames.CommandsVisible, "0");
            CommandsVisibleIfAvailable = value is not null && value.ToString()!.Equals("1");

            bool update = false;

            value = key.GetValue(RegistryStateNames.CommentSizeRatio, "-1");
            if (value is string descriptionString)
            {
                int ratio = int.Parse(descriptionString, CultureInfo.InvariantCulture);
                if (ratio > 0)
                {
                    _helpPaneSizeRatio = ratio;
                    update = true;
                }
            }

            value = key.GetValue(RegistryStateNames.CommandSizeRatio, "-1");
            if (value is string commandString)
            {
                int ratio = int.Parse(commandString, CultureInfo.InvariantCulture);
                if (ratio > 0)
                {
                    _commandsPaneSizeRatio = ratio;
                    update = true;
                }
            }

            if (update)
            {
                OnLayoutInternal(dividerOnly: false);
            }
        }
    }

    private void OnActiveDesignerChanged(object? sender, ActiveDesignerEventArgs e)
    {
        // When the active document is changed, check all the components so see if they
        // are offering up any new tabs.

        if (e.OldDesigner is not null && e.OldDesigner == _designerHost)
        {
            ActiveDesigner = null;
        }

        if (e.NewDesigner is not null && e.NewDesigner != _designerHost)
        {
            ActiveDesigner = e.NewDesigner;
        }
    }

    /// <summary>
    ///  Called when a property on an Ole32 Object changes.
    /// </summary>
    HRESULT IPropertyNotifySink.Interface.OnChanged(int dispID)
    {
        // We don't want the grid's own property sets doing this, but if we're getting
        // an OnChanged that isn't the DispID of the property we're currently changing,
        // we need to cause a refresh.
        bool fullRefresh = false;
        if (_gridView.SelectedGridEntry is PropertyDescriptorGridEntry selectedEntry
            && selectedEntry.PropertyDescriptor.Attributes is not null)
        {
            // Fish out the DispIdAttribute which will tell us the DispId of the property that we're changing.
            if (selectedEntry.PropertyDescriptor.TryGetAttribute(out DispIdAttribute? dispIdAttribute)
                && !dispIdAttribute.IsDefaultAttribute())
            {
                fullRefresh = dispID != dispIdAttribute.Value;
            }
        }

        if (!GetFlag(Flags.RefreshingProperties))
        {
            if (!_gridView.InPropertySet || fullRefresh)
            {
                Refresh(clearCached: fullRefresh);
            }

            // This is so changes to names of native objects will be reflected in the combo box.
            object? obj = GetUnwrappedObject(0);
            if ((ComNativeDescriptor.IsNameDispId(obj, dispID) || dispID == PInvokeCore.DISPID_Name) && obj is not null)
            {
                OnComComponentNameChanged(new ComponentRenameEventArgs(obj, oldName: null, TypeDescriptor.GetClassName(obj)));
            }
        }

        return HRESULT.S_OK;
    }

    /// <summary>
    ///  We forward messages from several of our children to our mouse move so we can put up the splitter over their borders
    /// </summary>
    private void OnChildMouseMove(object? sender, MouseEventArgs e)
    {
        Point newPoint = Point.Empty;
        if (ShouldForwardChildMouseMessage((Control?)sender, e, ref newPoint))
        {
            // Forward the message
            OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, newPoint.X, newPoint.Y, e.Delta));
            return;
        }
    }

    /// <summary>
    ///  We forward messages from several of our children to our mouse move so
    ///  we can put up the splitter over their borders.
    /// </summary>
    private void OnChildMouseDown(object? sender, MouseEventArgs e)
    {
        Point newPoint = Point.Empty;

        if (ShouldForwardChildMouseMessage((Control?)sender, e, ref newPoint))
        {
            // Forward the message
            OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, newPoint.X, newPoint.Y, e.Delta));
            return;
        }
    }

    private void OnComponentAdded(object? sender, ComponentEventArgs e)
    {
        if (e.Component is null)
        {
            throw new InvalidOperationException();
        }

        if (!TypeDescriptorHelper.TryGetAttribute(e.Component.GetType(), out PropertyTabAttribute? tabAttribute))
        {
            return;
        }

        // Add all the document tabs.
        for (int i = 0; i < tabAttribute.TabClasses.Length; i++)
        {
            if (tabAttribute.TabScopes[i] == PropertyTabScope.Document)
            {
                AddTab(tabAttribute.TabClasses[i], PropertyTabScope.Document, e.Component, setupToolbar: true);
            }
        }
    }

    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        bool batchMode = GetFlag(Flags.BatchMode);
        if (batchMode || GetFlag(Flags.InternalChange) || _gridView.InPropertySet ||
           (_selectedObjects is null) || (_selectedObjects.Length == 0))
        {
            if (batchMode && !_gridView.InPropertySet)
            {
                SetFlag(Flags.BatchModeChange, true);
            }

            return;
        }

        int objectCount = _selectedObjects.Length;
        for (int i = 0; i < objectCount; i++)
        {
            if (_selectedObjects[i] == e.Component)
            {
                Refresh(clearCached: false);
                break;
            }
        }
    }

    private void OnComponentRemoved(object? sender, ComponentEventArgs e)
    {
        if (e.Component is null)
        {
            throw new InvalidOperationException();
        }

        if (!TypeDescriptorHelper.TryGetAttribute(e.Component.GetType(), out PropertyTabAttribute? tabAttribute))
        {
            return;
        }

        // Remove all the document items.
        for (int i = 0; i < tabAttribute.TabClasses.Length; i++)
        {
            if (tabAttribute.TabScopes[i] == PropertyTabScope.Document)
            {
                ReleaseTab(tabAttribute.TabClasses[i], e.Component);
            }
        }

        if (_selectedObjects is not null)
        {
            for (int i = 0; i < _selectedObjects.Length; i++)
            {
                if (e.Component == _selectedObjects[i])
                {
                    object[] newObjects = new object[_selectedObjects.Length - 1];
                    Array.Copy(_selectedObjects, 0, newObjects, 0, i);
                    if (i < newObjects.Length)
                    {
                        // Fixed for .NET Framework 4.0
                        Array.Copy(_selectedObjects, i + 1, newObjects, i, newObjects.Length - i);
                    }

                    if (!GetFlag(Flags.BatchMode))
                    {
                        SelectedObjects = newObjects;
                    }
                    else
                    {
                        // Otherwise, just dump the selection.
                        _gridView.ClearGridEntries();
                        _selectedObjects = newObjects;
                        SetFlag(Flags.FullRefreshAfterBatch, true);
                    }
                }
            }
        }

        SetupToolbar();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Refresh();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        Refresh();
    }

    internal void OnGridViewMouseWheel(MouseEventArgs e) => OnMouseWheel(e);

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        OnLayoutInternal(dividerOnly: false);
        TypeDescriptor.Refreshed += OnTypeDescriptorRefreshed;
        if (_selectedObjects is not null && _selectedObjects.Length > 0)
        {
            Refresh(clearCached: true);
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        TypeDescriptor.Refreshed -= OnTypeDescriptorRefreshed;
        base.OnHandleDestroyed(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        if (ActiveControl is null)
        {
            SetActiveControl(_gridView);
        }
        else
        {
            // Sometimes the edit is still the active control when it's hidden or disabled.
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
        int sw = (int)Math.Round((Left + Width) * dx - sx);
        int sh = (int)Math.Round((Top + Height) * dy - sy);
        SetBounds(sx, sy, sw, sh, BoundsSpecified.All);
    }

    private void OnLayoutInternal(bool dividerOnly)
    {
        if (!IsHandleCreated || !Visible)
        {
            return;
        }

        using FreezePaintScope _ = new(this);

        if (!dividerOnly)
        {
            // PropertyGrid does a special handling on scaling and positioning its
            // child controls. These are not scaled by their parent when Dpi/Font change.
            if (_oldDeviceDpi != _deviceDpi)
            {
                RescaleConstants();
                SetupToolbar(fullRebuild: true);
            }

            // No toolbar or help or commands visible, just fill the whole thing with the grid.
            if (!_toolStrip.Visible && !_helpPane.Visible && !_commandsPane.Visible)
            {
                _gridView.Location = new Point(0, 0);
                _gridView.Size = Size;
                return;
            }

            if (_toolStrip.Visible)
            {
                int toolStripWidth = Width;
                int toolStripHeight = ((LargeButtons) ? s_largeButtonSize : s_normalButtonSize).Height + _toolStripButtonPaddingY;
                Rectangle toolStripBounds = new(0, 1, toolStripWidth, toolStripHeight);
                _toolStrip.Bounds = toolStripBounds;

                _gridView.Location = new Point(0, _toolStrip.Height + _toolStrip.Top);
            }
            else
            {
                _gridView.Location = new Point(0, 0);
            }
        }

        // Now work up from the bottom.
        int endSize = Size.Height;

        if (endSize < MinGridHeight)
        {
            return;
        }

        int maxSpace = endSize - (_gridView.Location.Y + MinGridHeight);
        int height;

        // If we're just moving the divider, set the requested heights.
        int helpRequestedHeight = 0;
        int commandsRequestedHeight = 0;
        int helpOptimalHeight = 0;
        int commandsOptimalHeight = 0;

        if (dividerOnly)
        {
            helpRequestedHeight = _helpPane.Visible ? _helpPane.Size.Height : 0;
            commandsRequestedHeight = _commandsPane.Visible ? _commandsPane.Size.Height : 0;
        }
        else
        {
            if (_helpPane.Visible)
            {
                helpOptimalHeight = _helpPane.GetOptimalHeight(Size.Width - s_cyDivider);
                if (_helpPane.UserSized)
                {
                    helpRequestedHeight = _helpPane.Size.Height;
                }
                else if (_helpPaneSizeRatio != -1)
                {
                    helpRequestedHeight = (Height * _helpPaneSizeRatio) / 100;
                }
                else
                {
                    helpRequestedHeight = helpOptimalHeight;
                }
            }

            if (_commandsPane.Visible)
            {
                commandsOptimalHeight = _commandsPane.GetOptimalHeight(Size.Width - s_cyDivider);
                if (_commandsPane.UserSized)
                {
                    commandsRequestedHeight = _commandsPane.Size.Height;
                }
                else if (_commandsPaneSizeRatio != -1)
                {
                    commandsRequestedHeight = (Height * _commandsPaneSizeRatio) / 100;
                }
                else
                {
                    commandsRequestedHeight = commandsOptimalHeight;
                }
            }
        }

        // Place the help comment window.
        if (helpRequestedHeight > 0)
        {
            maxSpace -= s_cyDivider;

            if (commandsRequestedHeight == 0 || (helpRequestedHeight + commandsRequestedHeight) < maxSpace)
            {
                // Full size.
                height = Math.Min(helpRequestedHeight, maxSpace);
            }
            else if (commandsRequestedHeight > 0 && commandsRequestedHeight < maxSpace)
            {
                // Give most of the space to the hot commands.
                height = maxSpace - commandsRequestedHeight;
            }
            else
            {
                // Split the difference.
                height = Math.Min(helpRequestedHeight, maxSpace / 2 - 1);
            }

            height = Math.Max(height, s_cyDivider * 2);

            _helpPane.SetBounds(0, endSize - height, Size.Width, height);

            // If we've modified the height to less than the optimal, clear the userSized item.
            if (height <= helpOptimalHeight && height < helpRequestedHeight)
            {
                _helpPane.UserSized = false;
            }
            else if (_helpPaneSizeRatio != -1 || _helpPane.UserSized)
            {
                _helpPaneSizeRatio = (_helpPane.Height * 100) / Height;
            }

            _helpPane.Invalidate();
            endSize = _helpPane.Location.Y - s_cyDivider;
            maxSpace -= height;
        }

        // Place the hot commands.
        if (commandsRequestedHeight > 0)
        {
            maxSpace -= s_cyDivider;

            if (maxSpace > commandsRequestedHeight)
            {
                // Full size.
                height = Math.Min(commandsRequestedHeight, maxSpace);
            }
            else
            {
                // What's left.
                height = maxSpace;
            }

            height = Math.Max(height, s_cyDivider * 2);

            // If we've modified the height, clear the userSized item.
            if (height <= commandsOptimalHeight && height < commandsRequestedHeight)
            {
                _commandsPane.UserSized = false;
            }
            else if (_commandsPaneSizeRatio != -1 || _commandsPane.UserSized)
            {
                _commandsPaneSizeRatio = (_commandsPane.Height * 100) / Height;
            }

            _commandsPane.SetBounds(0, endSize - height, Size.Width, height);
            _commandsPane.Invalidate();
            endSize = _commandsPane.Location.Y - s_cyDivider;
        }

        _gridView.Size = new Size(Size.Width, endSize - _gridView.Location.Y);
    }

#pragma warning disable CA1725 // Parameter name shipped as 'me'
    protected override void OnMouseDown(MouseEventArgs me)
    {
        SnappableControl? target = DividerInside(me.Y);
        if (target is not null && me.Button == MouseButtons.Left)
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
            if (DividerInside(me.Y) is not null)
            {
                Cursor = Cursors.HSplit;
            }
            else
            {
                Cursor = null;
            }

            return;
        }

        // _targetMove is initialized in OnMouseDown.
        int yNew = DividerLimitMove(_targetMove!, me.Y);

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

        // _targetMove is initialized in OnMouseDown.
        _dividerMoveY = DividerLimitMove(_targetMove!, me.Y);
        Rectangle rectDoc = _targetMove!.Bounds;
        if (_dividerMoveY != rectDoc.Y)
        {
            // We subtract half the height so the mouse is still over the divider.
            int yNew = rectDoc.Height + rectDoc.Y - _dividerMoveY - (s_cyDivider / 2);
            Size size = _targetMove.Size;
            size.Height = Math.Max(0, yNew);
            _targetMove.Size = size;
            _targetMove.UserSized = true;
            OnLayoutInternal(dividerOnly: true);

            // Invalidate the divider area so we cleanup anything left by the xor.
            Invalidate(new Rectangle(0, me.Y - s_cyDivider, Size.Width, me.Y + s_cyDivider));

            // In case we're doing the top one, we might have wrecked stuff on the grid.
            _gridView.Invalidate(new Rectangle(0, _gridView.Size.Height - s_cyDivider, Size.Width, s_cyDivider));
        }

        // End the move
        Capture = false;
        _dividerMoveY = -1;
        _targetMove = null;
        base.OnMouseUp(me);
    }
#pragma warning restore CA1725

    /// <summary>
    ///  Called when a property on an Ole32 Object that is tagged with "requestedit" is
    ///  about to be edited. See IPropertyNotifySink::OnRequestEdit
    /// </summary>
    HRESULT IPropertyNotifySink.Interface.OnRequestEdit(int dispID)
    {
        // Don't do anything here.
        return HRESULT.S_OK;
    }

    protected override void OnResize(EventArgs e)
    {
        if (IsHandleCreated && Visible)
        {
            OnLayoutInternal(dividerOnly: false);
        }

        base.OnResize(e);
    }

    private void OnButtonClick(object? sender, EventArgs e)
    {
        // We don't want to steal focus from the property pages.
        if (sender != _viewPropertyPagesButton)
        {
            _gridView.Focus();
        }
    }

    protected void OnComComponentNameChanged(ComponentRenameEventArgs e)
    {
        ((ComponentRenameEventHandler?)Events[s_comComponentNameChangedEvent])?.Invoke(this, e);
    }

    protected void OnNotifyPropertyValueUIItemsChanged(object? sender, EventArgs e)
    {
        _gridView.LabelPaintMargin = 0;
        _gridView.Invalidate(invalidateChildren: true);
    }

#pragma warning disable CA1725 // Parameter name shipped as 'pevent'
    protected override void OnPaint(PaintEventArgs pevent)
    {
        // Just erase the stuff above and below the properties window so we don't flicker.
        Point psheetLoc = _gridView.Location;
        int width = Size.Width;

        using var backgroundBrush = BackColor.GetCachedSolidBrushScope();
        pevent.Graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, width, psheetLoc.Y));

        int yLast = psheetLoc.Y + _gridView.Size.Height;

        // Fill above the commands pane.
        if (_commandsPane.Visible)
        {
            pevent.Graphics.FillRectangle(
                backgroundBrush,
                new Rectangle(0, yLast, width, _commandsPane.Location.Y - yLast));
            yLast += _commandsPane.Size.Height;
        }

        // Fill above the help pane.
        if (_helpPane.Visible)
        {
            pevent.Graphics.FillRectangle(
                backgroundBrush,
                new Rectangle(0, yLast, width, _helpPane.Location.Y - yLast));
            yLast += _helpPane.Size.Height;
        }

        // Anything that might be left.
        pevent.Graphics.FillRectangle(backgroundBrush, new Rectangle(0, yLast, width, Size.Height - yLast));

        base.OnPaint(pevent);
    }
#pragma warning restore CA1725

    protected virtual void OnPropertySortChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_propertySortChangedEvent])?.Invoke(this, e);
    }

    protected virtual void OnPropertyTabChanged(PropertyTabChangedEventArgs e)
    {
        ((PropertyTabChangedEventHandler?)Events[s_propertyTabChangedEvent])?.Invoke(this, e);
    }

    protected virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
    {
        ((PropertyValueChangedEventHandler?)Events[s_propertyValueChangedEvent])?.Invoke(this, e);
    }

    internal void OnPropertyValueSet(GridItem changedItem, object? oldValue)
    {
        OnPropertyValueChanged(new PropertyValueChangedEventArgs(changedItem, oldValue));

        if (changedItem is null)
        {
            return;
        }

        // Announce the property value change like standalone combobox control do: "[something] selected".
        bool dropDown = false;
        Type propertyType = changedItem.PropertyDescriptor!.PropertyType;
        var editor = (UITypeEditor?)TypeDescriptor.GetEditor(propertyType, typeof(UITypeEditor));
        if (editor is not null)
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

        if (IsAccessibilityObjectCreated && dropDown && !_gridView.DropDownVisible)
        {
            AccessibilityObject.RaiseAutomationNotification(
                Automation.AutomationNotificationKind.ActionCompleted,
                Automation.AutomationNotificationProcessing.All,
                string.Format(SR.PropertyGridPropertyValueSelectedFormat, changedItem.Value));
        }
    }

    internal void OnSelectedGridItemChanged(GridEntry? oldEntry, GridEntry? newEntry)
    {
        OnSelectedGridItemChanged(new SelectedGridItemChangedEventArgs(oldEntry, newEntry));
    }

    protected virtual void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
    {
        ((SelectedGridItemChangedEventHandler?)Events[s_selectedGridItemChangedEvent])?.Invoke(this, e);
    }

    protected virtual void OnSelectedObjectsChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_selectedObjectsChangedEvent])?.Invoke(this, e);
    }

    private void OnTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (!e.LastTransaction)
        {
            return;
        }

        // We should not refresh the grid if the selectedObject is no longer sited.
        if (SelectedObject is IComponent currentSelection && currentSelection.Site is null)
        {
            // The component is not logically sited- clear the PropertyGrid Selection.
            // Setting to null will clear off the state information so that PropertyGrid is in sane State.
            SelectedObject = null;
            return;
        }

        SetFlag(Flags.BatchMode, false);
        if (GetFlag(Flags.FullRefreshAfterBatch))
        {
            SelectedObjects = _selectedObjects;
            SetFlag(Flags.FullRefreshAfterBatch, false);
        }
        else if (GetFlag(Flags.BatchModeChange))
        {
            Refresh(clearCached: false);
        }

        SetFlag(Flags.BatchModeChange, false);
    }

    private void OnTransactionOpened(object? sender, EventArgs e) => SetFlag(Flags.BatchMode, true);

    private void OnTypeDescriptorRefreshed(RefreshEventArgs e)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new RefreshEventHandler(OnTypeDescriptorRefreshedInvoke), [e]);
        }
        else
        {
            OnTypeDescriptorRefreshedInvoke(e);
        }
    }

    private void OnTypeDescriptorRefreshedInvoke(RefreshEventArgs e)
    {
        if (_selectedObjects is null)
        {
            return;
        }

        for (int i = 0; i < _selectedObjects.Length; i++)
        {
            Type? typeChanged = e.TypeChanged;
            if (_selectedObjects[i] == e.ComponentChanged || typeChanged?.IsAssignableFrom(_selectedObjects[i].GetType()) == true)
            {
                // Clear our property hashes.
                ClearCachedProperties();
                Refresh(clearCached: true);
                return;
            }
        }
    }

    private void OnViewSortButtonClick(object? sender, EventArgs e)
    {
        using (FreezePaintScope _ = new(this))
        {
            // Is this tab selected? If so, do nothing.
            if (sender == _viewSortButtons![_selectedViewSort])
            {
                _viewSortButtons[_selectedViewSort].Checked = true;
                return;
            }

            // Check new button and uncheck old button.
            _viewSortButtons[_selectedViewSort].Checked = false;

            // Find the new button in the list.
            int index;
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
                case AlphaSortButtonIndex:
                    _propertySortValue = PropertySort.Alphabetical;
                    break;
                case CategorySortButtonIndex:
                    _propertySortValue = PropertySort.Alphabetical | PropertySort.Categorized;
                    break;
                case NoSortButtonIndex:
                    _propertySortValue = PropertySort.NoSort;
                    break;
            }

            OnPropertySortChanged(EventArgs.Empty);

            Refresh(clearCached: false);
            OnLayoutInternal(dividerOnly: false);
        }

        OnButtonClick(sender, e);
    }

    private void OnViewTabButtonClick(object? sender, EventArgs e)
    {
        using (FreezePaintScope _ = new(this))
        {
            if (sender is not null)
            {
                SelectViewTabButton((ToolStripButton)sender, true);
            }

            OnLayoutInternal(dividerOnly: false);
            SaveSelectedTabIndex();
        }

        OnButtonClick(sender, e);
    }

    private void OnViewPropertyPagesButtonClick(object? sender, EventArgs e)
    {
        if (_viewPropertyPagesButton!.Enabled &&
            _selectedObjects is not null &&
            _selectedObjects.Length > 0)
        {
            object baseObject = _selectedObjects[0];
            object obj = baseObject;

            bool success = false;

            var uiService = GetService<IUIService>();

            try
            {
                if (uiService is not null)
                {
                    success = uiService.ShowComponentEditor(obj, this);
                }
                else
                {
                    try
                    {
                        var editor = (ComponentEditor?)TypeDescriptor.GetEditor(obj, typeof(ComponentEditor));
                        if (editor is not null)
                        {
                            success = editor is WindowsFormsComponentEditor formsEditor
                                ? formsEditor.EditComponent(null, obj, this)
                                : editor.EditComponent(obj);
                        }
                    }
                    catch
                    {
                    }
                }

                if (success)
                {
                    if (baseObject is IComponent component
                        && _connectionPointCookies is not null
                        && _connectionPointCookies[0] is null
                        && component.Site is ISite site
                        && site.TryGetService(out IComponentChangeService? changeService))
                    {
                        try
                        {
                            changeService.OnComponentChanging(baseObject);
                        }
                        catch (CheckoutException checkoutException) when (checkoutException == CheckoutException.Canceled)
                        {
                            return;
                        }

                        try
                        {
                            // Now notify the change service that the change was successful.
                            SetFlag(Flags.InternalChange, true);
                            changeService.OnComponentChanged(baseObject);
                        }
                        finally
                        {
                            SetFlag(Flags.InternalChange, false);
                        }
                    }

                    _gridView.Refresh();
                }
            }
            catch (Exception ex)
            {
                string error = SR.ErrorPropertyPageFailed;
                if (uiService is not null)
                {
                    uiService.ShowError(ex, error);
                }
                else
                {
                    RTLAwareMessageBox.Show(
                        null,
                        error,
                        SR.PropertyGridTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
            }
        }

        OnButtonClick(sender, e);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible && IsHandleCreated)
        {
            OnLayoutInternal(dividerOnly: false);
            SetupToolbar();
        }
    }

    /// <summary>
    ///  Returns the last child control that can take focus
    /// </summary>
    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & Keys.KeyCode) != Keys.Tab
            || keyData.HasFlag(Keys.Control)
            || keyData.HasFlag(Keys.Alt))
        {
            return base.ProcessDialogKey(keyData);
        }

        // Are we going forward?
        if (keyData.HasFlag(Keys.Shift))
        {
            // This is backward
            if (_commandsPane.Visible && _commandsPane.ContainsFocus)
            {
                _gridView.ReverseFocus();
            }
            else if (_gridView.FocusInside)
            {
                if (_toolStrip.Visible)
                {
                    _toolStrip.Focus();

                    // We need to select first ToolStrip item, otherwise ToolStrip container has the focus.
                    if (_toolStrip.Items.Count > 0)
                    {
                        _toolStrip.SelectNextToolStripItem(start: null, forward: true);
                    }
                }
                else
                {
                    return base.ProcessDialogKey(keyData);
                }
            }
            else
            {
                if (_toolStrip.Focused || !_toolStrip.Visible)
                {
                    // If we get here and the toolbar has focus it means we're processing normally, so
                    // pass the focus to the parent.
                    return base.ProcessDialogKey(keyData);
                }
                else
                {
                    // Otherwise, we're processing a message from elsewhere so we select our bottom item.
                    if (_commandsPane.Visible)
                    {
                        _commandsPane.FocusLabel();
                    }
                    else if (_rootEntry is not null)
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

            // This is forward.
            if (_toolStrip.Focused)
            {
                // Normal stuff, just do the propsheet.
                if (_rootEntry is not null)
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
                if (_commandsPane.Visible)
                {
                    _commandsPane.FocusLabel();
                    return true;
                }
                else
                {
                    passToParent = true;
                }
            }
            else if (_commandsPane.ContainsFocus)
            {
                passToParent = true;
            }
            else
            {
                // Coming from out side, start with the toolStrip.
                if (_toolStrip.Visible)
                {
                    _toolStrip.Focus();
                }
                else
                {
                    _gridView.Focus();
                }
            }

            // Nobody's claimed the focus, pass it on.
            if (passToParent)
            {
                // Properties window is already selected, pass on to parent.
                bool result = base.ProcessDialogKey(keyData);

                // If we're not hosted in a windows forms thing, just give the parent the focus.
                if (!result && Parent is null)
                {
                    HWND hWndParent = PInvoke.GetParent(this);
                    if (!hWndParent.IsNull)
                    {
                        PInvoke.SetFocus(hWndParent);
                    }
                }

                return result;
            }
        }

        return true;
    }

    public override void Refresh()
    {
        if (GetFlag(Flags.RefreshingProperties))
        {
            return;
        }

        Refresh(clearCached: true);
        base.Refresh();
    }

    private void Refresh(bool clearCached)
    {
        if (Disposing || GetFlag(Flags.RefreshingProperties))
        {
            return;
        }

        try
        {
            FreezePainting = true;
            SetFlag(Flags.RefreshingProperties, true);

            if (clearCached)
            {
                ClearCachedProperties();
            }

            RefreshProperties(clearCached);
            _gridView.Refresh();
            DisplayCommandsPane();
        }
        finally
        {
            FreezePainting = false;
            SetFlag(Flags.RefreshingProperties, false);
        }
    }

    internal void RefreshProperties(bool clearCached)
    {
        // Clear our current cache so we can do a full refresh.
        if (clearCached && _selectedTab is not null)
        {
            PropertyTab selectedTab = _selectedTab.Tab;
            if (selectedTab is not null && _viewTabProperties is not null)
            {
                _viewTabProperties.Remove($"{selectedTab.TabName}{_propertySortValue}");
            }
        }

        SetFlag(Flags.PropertiesChanged, true);
        UpdateSelection();
    }

    /// <summary>
    ///  Refreshes the tabs of the specified <paramref name="tabScope"/>.
    /// </summary>
    /// <param name="tabScope">
    ///  Either <see cref="PropertyTabScope.Component"/> or <see cref="PropertyTabScope.Document"/>.
    /// </param>
    /// <remarks>
    ///  <para>
    ///   The <see cref="RefreshTabs(PropertyTabScope)"/> method first deletes the property tabs of the specified
    ///   scope, it then requires the objects and documents to rebuild the tabs.
    ///  </para>
    /// </remarks>
    public void RefreshTabs(PropertyTabScope tabScope)
    {
        if (tabScope < PropertyTabScope.Document)
        {
            throw new ArgumentException(SR.PropertyGridTabScope);
        }

        RemoveTabs(tabScope, setupToolbar: false);

        // Check the component level tabs.
        if (tabScope <= PropertyTabScope.Component && _selectedObjects is not null && _selectedObjects.Length > 0)
        {
            // Get the subset of PropertyTabs that are common to all selected objects.
            foreach (Type tabType in GetCommonTabs(_selectedObjects, PropertyTabScope.Component))
            {
                foreach (object @object in _selectedObjects)
                {
                    AddTab(tabType, PropertyTabScope.Component, @object, setupToolbar: false);
                }
            }
        }

        // Check the document level tabs.
        if (tabScope <= PropertyTabScope.Document
            && _designerHost?.Container?.Components is ComponentCollection components)
        {
            foreach (IComponent component in components)
            {
                if (TypeDescriptorHelper.TryGetAttribute(components.GetType(), out PropertyTabAttribute? tabAttribute))
                {
                    for (int i = 0; i < tabAttribute.TabClasses.Length; i++)
                    {
                        if (tabAttribute.TabScopes[i] == PropertyTabScope.Document)
                        {
                            AddTab(tabAttribute.TabClasses[i], PropertyTabScope.Document, component, setupToolbar: false);
                        }
                    }
                }
            }
        }

        SetupToolbar();
    }

    internal void ReleaseTab(Type tabType, object component)
    {
        PropertyTab? tab = null;
        int tabIndex = -1;
        for (int i = 0; i < _tabs.Count; i++)
        {
            if (tabType == _tabs[i].Tab.GetType())
            {
                tab = _tabs[i].Tab;
                tabIndex = i;
                break;
            }
        }

        if (tab is null)
        {
            return;
        }

        object[]? components = tab.Components;
        bool killTab;

        try
        {
            int index = -1;
            if (components is not null)
            {
                index = Array.IndexOf(components, component);
            }

            if (index >= 0)
            {
                object[] newComponents = new object[components!.Length - 1];
                Array.Copy(components, 0, newComponents, 0, index);
                Array.Copy(components, index + 1, newComponents, index, components.Length - index - 1);
                components = newComponents;
                tab.Components = components;
            }

            killTab = components is not null && components.Length == 0;
        }
        catch (Exception e)
        {
            Debug.Fail("Bad Tab.  It's going away.", e.ToString());
            killTab = true;
        }

        // We don't remove PropertyTabScope.Global tabs here.  Our owner has to do that.
        if (killTab && _tabs[tabIndex].Scope > PropertyTabScope.Global)
        {
            RemoveTab(tabIndex, false);
        }
    }

    /// <summary>
    ///  Removes all the tabs with a classification greater than or equal to the specified classification.
    ///  For example, removing <see cref="PropertyTabScope.Document"/> will remove <see cref="PropertyTabScope.Document"/>
    ///  and <see cref="PropertyTabScope.Component"/> tabs.
    /// </summary>
    internal void RemoveTabs(PropertyTabScope classification, bool setupToolbar)
    {
        if (classification == PropertyTabScope.Static)
        {
            throw new ArgumentException(SR.PropertyGridRemoveStaticTabs);
        }

        // In case we've been disposed.
        if (_tabs.Count == 0)
        {
            return;
        }

        ToolStripButton? selectedButton = _selectedTab?.Button;

        if (_tabs.RemoveAll(i => i.Scope >= classification) > 0)
        {
            _tabsDirty = true;
            _selectedTab = _tabs.FirstOrDefault();
        }

        if (setupToolbar && _tabsDirty)
        {
            SetupToolbar();

            Debug.Assert(_tabs.Count > 0, "We don't have any tabs left!");

            _selectedTab = null;
            SelectViewTabButtonDefault(selectedButton);

            // Clear the component refs of the tabs.
            foreach (TabInfo info in _tabs)
            {
                info.Tab.Components = [];
            }
        }
    }

    internal void RemoveTab(int tabIndex, bool setupToolbar)
    {
        Debug.Assert(_tabs.Count > 0, "Tab array destroyed!");

        ArgumentOutOfRangeException.ThrowIfNegative(tabIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(tabIndex, _tabs.Count);

        if (_tabs[tabIndex].Scope == PropertyTabScope.Static)
        {
            throw new ArgumentException(SR.PropertyGridRemoveStaticTabs);
        }

        if (_selectedTab == _tabs[tabIndex])
        {
            _selectedTab = _tabs[PropertiesTabIndex];
        }

        // Remove this tab from our "last selected" group
        if (!GetFlag(Flags.ReInitTab) && TryGetSavedTabIndex(out int selectedTab) && selectedTab == tabIndex)
        {
            _designerSelections.Remove(ActiveDesigner.GetHashCode());
        }

        ToolStripButton? selectedButton = _selectedTab?.Button;

        _tabs.RemoveAt(tabIndex);
        _tabsDirty = true;

        if (setupToolbar)
        {
            SetupToolbar();
            _selectedTab = null;
            SelectViewTabButtonDefault(selectedButton);
        }
    }

    internal void RemoveTab(Type tabType)
    {
        int tabIndex = -1;
        for (int i = 0; i < _tabs.Count; i++)
        {
            if (tabType == _tabs[i].Tab.GetType())
            {
                tabIndex = i;
                break;
            }
        }

        // Just quit if the tab isn't present.
        if (tabIndex == -1)
        {
            return;
        }

        _tabs.RemoveAt(tabIndex);
        _tabsDirty = true;
        SetupToolbar();
    }

    // The following Reset methods are used via reflection.

    private void ResetCommandsBackColor() => _commandsPane.ResetBackColor();

    private void ResetCommandsForeColor() => _commandsPane.ResetForeColor();

    private void ResetCommandsLinkColor() => _commandsPane.Label.ResetLinkColor();

    private void ResetCommandsActiveLinkColor() => _commandsPane.Label.ResetActiveLinkColor();

    private void ResetCommandsDisabledLinkColor() => _commandsPane.Label.ResetDisabledLinkColor();

    private void ResetHelpBackColor() => _helpPane.ResetBackColor();

    private void ResetHelpForeColor() => _helpPane.ResetBackColor();

    /// <summary>
    ///  This method is intended for use in replacing a specific selected root object with another object of the
    ///  same exact type. Scenario: An immutable root object being replaced with a new instance because one of its
    ///  properties was changed by the user.
    /// </summary>
    internal void ReplaceSelectedObject(object oldObject, object newObject)
    {
        Debug.Assert(oldObject is not null && newObject is not null && oldObject.GetType() == newObject.GetType());

        if (_selectedObjects is null)
        {
            return;
        }

        for (int i = 0; i < _selectedObjects.Length; ++i)
        {
            if (_selectedObjects[i] == oldObject)
            {
                _selectedObjects[i] = newObject;
                Refresh(clearCached: true);
                break;
            }
        }
    }

    public void ResetSelectedProperty() => _gridView.Reset();

    private void SaveSelectedTabIndex()
    {
        if (_designerHost is not null)
        {
            _designerSelections ??= [];

            // If _selectedTab is null, we will set the _designerSelections item to -1.
            _designerSelections[_designerHost.GetHashCode()] = _tabs.IndexOf(_selectedTab!);
        }
    }

    [MemberNotNullWhen(true, nameof(_designerSelections))]
    [MemberNotNullWhen(true, nameof(ActiveDesigner))]
    private bool TryGetSavedTabIndex(out int selectedTabIndex)
    {
        selectedTabIndex = -1;
        if (_designerSelections is null || ActiveDesigner is null)
        {
            return false;
        }

        int hashCode = ActiveDesigner.GetHashCode();
        if (!_designerSelections.TryGetValue(hashCode, out int value))
        {
            return false;
        }

        selectedTabIndex = value;
        return true;
    }

    private void SetHotCommandColors()
        => _commandsPane.SetColors(SystemColors.Control, Application.SystemColors.ControlText, Color.Empty, Color.Empty, Color.Empty, Color.Empty);

    internal void SetStatusBox(string? title, string? description) => _helpPane.SetDescription(title, description);

    private void SelectViewTabButton(ToolStripButton button, bool updateSelection)
    {
        Debug.Assert(_tabs.Count > 0, "No view tab buttons to select!");

        if (!SelectViewTabButtonDefault(button))
        {
            Debug.Fail("Failed to find the tab!");
        }

        if (updateSelection)
        {
            Refresh(clearCached: false);
        }
    }

    private bool SelectViewTabButtonDefault(ToolStripButton? button)
    {
        if (button is null)
        {
            return false;
        }

        // Is this tab button checked? If so, do nothing.
        if (button == _selectedTab?.Button)
        {
            button.Checked = true;
            return true;
        }

        PropertyTab? oldTab = null;

        // Unselect what's selected.
        if (_selectedTab is not null)
        {
            _selectedTab.Button.Checked = false;
            oldTab = _selectedTab.Tab;
        }

        // Get the new index of the button.
        foreach (TabInfo info in _tabs)
        {
            if (info.Button == button)
            {
                _selectedTab = info;
                info.Button.Checked = true;

                try
                {
                    SetFlag(Flags.TabsChanging, true);
                    OnPropertyTabChanged(new(oldTab, info.Tab));
                }
                finally
                {
                    SetFlag(Flags.TabsChanging, false);
                }

                return true;
            }
        }

        // Select the first tab if we didn't find that one.
        _selectedTab = _tabs[PropertiesTabIndex];
        Debug.Assert(_tabs[PropertiesTabIndex].Tab.GetType() == DefaultTabType, "First item is not property tab!");
        SelectViewTabButton(_tabs[PropertiesTabIndex].Button, updateSelection: false);
        return false;
    }

    private void SetSelectState(int state)
    {
        if (_viewSortButtons is null)
        {
            return;
        }

        if (state >= (_tabs.Count * _viewSortButtons.Length))
        {
            state = 0;
        }
        else if (state < 0)
        {
            state = (_tabs.Count * _viewSortButtons.Length) - 1;
        }

        // NOTE: See GetSelectState for the full description of the state transitions

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

            OnViewTabButtonClick(_tabs[tab].Button, EventArgs.Empty);
            OnViewSortButtonClick(_viewSortButtons[view], EventArgs.Empty);
        }
    }

    private void SetToolStripRenderer()
    {
        if (DrawFlatToolbar || SystemInformation.HighContrast)
        {
            // Use an office look and feel with system colors.
            ProfessionalColorTable colorTable = new()
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

    private void SetupToolbar() => SetupToolbar(fullRebuild: false);

    private void SetupToolbar(bool fullRebuild)
    {
        // If the tab array hasn't changed, don't bother to do all this work.
        if (!_tabsDirty && !fullRebuild)
        {
            return;
        }

        using FreezePaintScope _ = new(this);

        if (_normalButtonImages is null || fullRebuild)
        {
            _normalButtonImages?.Dispose();
            _normalButtonImages = new ImageList();
            if (ScaleHelper.IsScalingRequired)
            {
                _normalButtonImages.ImageSize = s_normalButtonSize;
            }
        }

        // Setup our event handlers.
        EventHandler tabButtonHandler = OnViewTabButtonClick;
        EventHandler sortButtonHandler = OnViewSortButtonClick;
        EventHandler propertyPagesButtonHandler = OnViewPropertyPagesButtonClick;

        int i;

        // We manage the buttons as a separate list so the toolbar doesn't flash.
        List<ToolStripItem> buttonList = fullRebuild ? new() : new(_toolStrip.Items.OfType<ToolStripItem>());

        // Setup the view type buttons. We only need to do this once.
        if (_viewSortButtons is null || fullRebuild)
        {
            _viewSortButtons = new ToolStripButton[3];

            int alphaIndex = -1;
            int categoryIndex = -1;

            try
            {
                _alphaBitmap ??= SortByPropertyImage;
                alphaIndex = AddImage(_alphaBitmap);
            }
            catch (Exception)
            {
            }

            try
            {
                _categoryBitmap ??= SortByCategoryImage;
                categoryIndex = AddImage(_categoryBitmap);
            }
            catch (Exception)
            {
            }

            _viewSortButtons[AlphaSortButtonIndex] = CreatePushButton(
                SR.PBRSToolTipAlphabetic,
                alphaIndex,
                sortButtonHandler,
                useRadioButtonRole: true);
            _viewSortButtons[CategorySortButtonIndex] = CreatePushButton(
                SR.PBRSToolTipCategorized,
                categoryIndex,
                sortButtonHandler,
                useRadioButtonRole: true);

            // We create a dummy hidden button for view sort.
            _viewSortButtons[NoSortButtonIndex] = CreatePushButton(
                string.Empty,
                0,
                sortButtonHandler,
                useRadioButtonRole: true);

            _viewSortButtons[NoSortButtonIndex].Visible = false;

            // Add the viewType buttons and a separator.
            buttonList.AddRange(_viewSortButtons);
        }
        else
        {
            // Clear all the items from the toolStrip and image list after the first two.
            int count = buttonList.Count;

            if (count > 2)
            {
                buttonList.RemoveRange(2, count - 2);
            }

            count = _normalButtonImages.Images.Count;

            for (i = count - 1; i >= 2; i--)
            {
                _normalButtonImages.Images.RemoveAt(i);
                _largeButtonImages?.Images.RemoveAt(i);
            }
        }

        buttonList.Add(_separator1);

        // If we've only got the properties tab, don't add the button
        // (or we'll just have a properties button that you can't do anything with).
        if (_tabs.Count > 1)
        {
            foreach (TabInfo info in _tabs)
            {
                try
                {
                    info.Button.ImageIndex = AddImage(info.Tab.Bitmap!);
                    buttonList.Add(info.Button);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }

            buttonList.Add(_separator2);
        }

        // Add the design page button.
        int designPage = 0;

        try
        {
            _propertyPageBitmap ??= ShowPropertyPageImage;
            designPage = AddImage(_propertyPageBitmap);
        }
        catch (Exception)
        {
        }

        // We recreate this every time to ensure it's at the end.
        _viewPropertyPagesButton = CreatePushButton(
            SR.PBRSToolTipPropertyPages,
            designPage,
            propertyPagesButtonHandler,
            useRadioButtonRole: false);

        _viewPropertyPagesButton.Enabled = false;
        buttonList.Add(_viewPropertyPagesButton);

        // Dispose this so it will get recreated for any new buttons.
        _largeButtonImages?.Dispose();
        _largeButtonImages = null;

        if (LargeButtons)
        {
            EnsureLargeButtons();
        }

        _toolStrip.ImageList = LargeButtons ? _largeButtonImages : _normalButtonImages;

        using (SuspendLayoutScope scope = new(_toolStrip))
        {
            _toolStrip.Items.Clear();
            for (int j = 0; j < buttonList.Count; j++)
            {
                _toolStrip.Items.Add(buttonList[j]);
            }
        }

        if (_tabsDirty)
        {
            // If we're redoing our tabs make sure we setup the toolbar area correctly.
            OnLayoutInternal(dividerOnly: false);
        }

        _tabsDirty = false;
    }

    protected void ShowEventsButton(bool value)
    {
        if (_tabs.Count > EventsTabIndex && _tabs[EventsTabIndex] is { } info && info.Tab is EventsTab)
        {
            info.Button.Visible = value;
            if (!value && _selectedTab == info)
            {
                SelectViewTabButton(_tabs[PropertiesTabIndex].Button, updateSelection: true);
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
    protected virtual Bitmap SortByPropertyImage => ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(PropertyGrid), "PBAlpha");

    /// <summary>
    ///  This 16x16 Bitmap is applied to the button which displays properties under the assigned categories.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected virtual Bitmap SortByCategoryImage => ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(PropertyGrid), "PBCategory");

    /// <summary>
    ///  This 16x16 Bitmap is applied to the button which displays property page in the designer pane.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected virtual Bitmap ShowPropertyPageImage => ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(PropertyGrid), "PBPPage");

    // "Should" methods are used by the designer via reflection.

    private bool ShouldSerializeCommandsBackColor() => _commandsPane.ShouldSerializeBackColor();

    private bool ShouldSerializeCommandsForeColor() => _commandsPane.ShouldSerializeForeColor();

    private bool ShouldSerializeCommandsLinkColor() => _commandsPane.Label.ShouldSerializeLinkColor();

    private bool ShouldSerializeCommandsActiveLinkColor() => _commandsPane.Label.ShouldSerializeActiveLinkColor();

    private bool ShouldSerializeCommandsDisabledLinkColor() => _commandsPane.Label.ShouldSerializeDisabledLinkColor();

    /// <summary>
    ///  Sinks the property notify events on all the COM objects we are currently browsing.
    ///
    ///  <see cref="IPropertyNotifySink"/>
    /// </summary>
    private void SinkPropertyNotifyEvents()
    {
        // First clear any existing sinks.
        for (int i = 0; _connectionPointCookies is not null && i < _connectionPointCookies.Length; i++)
        {
            if (_connectionPointCookies[i] is not null)
            {
                _connectionPointCookies[i].Disconnect();
                _connectionPointCookies[i] = null!;
            }
        }

        if (_selectedObjects is null || _selectedObjects.Length == 0)
        {
            _connectionPointCookies = null;
            return;
        }

        // It's okay if our array is too big, we'll just reuse it and ignore the empty slots.
        if (_connectionPointCookies is null || (_selectedObjects.Length > _connectionPointCookies.Length))
        {
            _connectionPointCookies = new AxHost.ConnectionPointCookie[_selectedObjects.Length];
        }

        for (int i = 0; i < _selectedObjects.Length; i++)
        {
            try
            {
                object? obj = GetUnwrappedObject(i);

                if (obj is not null && !Marshal.IsComObject(obj))
                {
                    continue;
                }

                _connectionPointCookies[i] = new(obj, this, typeof(IPropertyNotifySink.Interface), throwException: false);
            }
            catch
            {
            }
        }
    }

    private bool ShouldForwardChildMouseMessage(Control? child, MouseEventArgs e, ref Point point)
    {
        if (child is null)
        {
            return false;
        }

        Size size = child.Size;

        // Are we within two pixels of the edge?
        if (e.Y <= 1 || (size.Height - e.Y) <= 1)
        {
            // Convert the coordinates.
            Point temp = new(e.X, e.Y);
            temp = WindowsFormsUtils.TranslatePoint(temp, child, this);

            // Forward the message.
            point = temp;
            return true;
        }

        return false;
    }

    private void UpdatePropertiesViewTabVisibility()
    {
        // If the only view available is properties-view, there's no need to show the button.

        if (_tabs.Count <= 1)
        {
            return;
        }

        bool shouldBeVisible = false;

        // Starts at index 1, since index 0 is properties-view
        for (int i = 1; i < _tabs.Count; i++)
        {
            if (_tabs[i].Button.Visible)
            {
                shouldBeVisible = true;
                break;
            }
        }

        _tabs[PropertiesTabIndex].Button.Visible = shouldBeVisible;
        _separator2.Visible = shouldBeVisible;
    }

    internal void UpdateSelection()
    {
        if (!GetFlag(Flags.PropertiesChanged) || _tabs.Count == 0)
        {
            return;
        }

        string tabName = $"{_selectedTab?.Tab.TabName}{_propertySortValue}";

        if (_viewTabProperties is not null && _viewTabProperties.TryGetValue(tabName, out GridEntry? value))
        {
            _rootEntry = value;
            _rootEntry.Refresh();
        }
        else
        {
            if (_selectedObjects is not null && _selectedObjects.Length > 0)
            {
                _rootEntry = GridEntry.CreateRootGridEntry(
                    _gridView,
                    _selectedObjects,
                    new PropertyGridServiceProvider(this),
                    _designerHost,
                    SelectedTab,
                    _propertySortValue);
            }
            else
            {
                _rootEntry = null;
            }

            if (_rootEntry is null)
            {
                _currentEntries = new(disposeItems: false);
                _gridView.ClearGridEntries();
                return;
            }

            if (BrowsableAttributes is not null)
            {
                _rootEntry.BrowsableAttributes = BrowsableAttributes;
            }

            _viewTabProperties ??= [];
            _viewTabProperties[tabName] = _rootEntry;
        }

        // Get entries.
        _currentEntries = _rootEntry.Children;
        _defaultEntry = _rootEntry.DefaultChild;
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
        get => UseCompatibleTextRenderingInternal;
        set
        {
            UseCompatibleTextRenderingInternal = value;
            _helpPane.UpdateTextRenderingEngine();
            _gridView.Invalidate();
        }
    }

    internal override bool SupportsUiaProviders => true;

    internal override bool SupportsUseCompatibleTextRendering => true;

    internal override bool AllowsKeyboardToolTip() => false;

    internal bool WantsTab(bool forward)
    {
        // A mini version of process dialog key for responding to WM_GETDLGCODE.
        if (forward)
        {
            return _toolStrip.Visible && _toolStrip.Focused;
        }
        else
        {
            return _gridView.ContainsFocus && _toolStrip.Visible;
        }
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        // Refresh the toolbar buttons.
        SetupToolbar(fullRebuild: true);

        // This doesn't stick the first time we do it, so we call it again.
        // Fortunately this doesn't happen very often.
        if (!GetFlag(Flags.SysColorChangeRefresh))
        {
            SetupToolbar(fullRebuild: true);
            SetFlag(Flags.SysColorChangeRefresh, true);
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
        s_cyDivider = LogicalToDeviceUnits(CyDivider);
        _toolStripButtonPaddingY = LogicalToDeviceUnits(ToolStripButtonPaddingY);
    }

    protected override unsafe void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_UNDO:
                if (m.LParamInternal == 0)
                {
                    _gridView.DoUndoCommand();
                }
                else
                {
                    m.ResultInternal = (LRESULT)(nint)(BOOL)CanUndo;
                }

                return;
            case PInvoke.WM_CUT:
                if (m.LParamInternal == 0)
                {
                    _gridView.DoCutCommand();
                }
                else
                {
                    m.ResultInternal = (LRESULT)(nint)(BOOL)CanCut;
                }

                return;

            case PInvoke.WM_COPY:
                if (m.LParamInternal == 0)
                {
                    _gridView.DoCopyCommand();
                }
                else
                {
                    m.ResultInternal = (LRESULT)(nint)(BOOL)CanCopy;
                }

                return;

            case PInvoke.WM_PASTE:
                if (m.LParamInternal == 0)
                {
                    _gridView.DoPasteCommand();
                }
                else
                {
                    m.ResultInternal = (LRESULT)(nint)(BOOL)CanPaste;
                }

                return;

            case PInvoke.WM_COPYDATA:
                var cds = (COPYDATASTRUCT*)(nint)m.LParamInternal;

                if (cds is not null && cds->lpData is not null)
                {
                    _propertyName = Marshal.PtrToStringAuto((nint)cds->lpData);
                    _copyDataMessage = (int)cds->dwData;
                }

                m.ResultInternal = (LRESULT)1;
                return;
            case AutomationMessages.PGM_GETBUTTONCOUNT:
                if (_toolStrip is not null)
                {
                    m.ResultInternal = (LRESULT)_toolStrip.Items.Count;
                    return;
                }

                break;
            case AutomationMessages.PGM_GETBUTTONSTATE:
                if (_toolStrip is not null)
                {
                    int index = (int)m.WParamInternal;
                    if (index >= 0 && index < _toolStrip.Items.Count)
                    {
                        if (_toolStrip.Items[index] is ToolStripButton button)
                        {
                            m.ResultInternal = (LRESULT)(nint)(BOOL)button.Checked;
                        }
                        else
                        {
                            m.ResultInternal = (LRESULT)0;
                        }
                    }

                    return;
                }

                break;
            case AutomationMessages.PGM_SETBUTTONSTATE:
                if (_toolStrip is not null)
                {
                    int index = (int)m.WParamInternal;
                    if (index >= 0 && index < _toolStrip.Items.Count && _toolStrip.Items[index] is ToolStripButton button)
                    {
                        button.Checked = !button.Checked;

                        // Special treatment for the properties page button.
                        if (button == _viewPropertyPagesButton)
                        {
                            OnViewPropertyPagesButtonClick(button, EventArgs.Empty);
                        }
                        else
                        {
                            switch ((int)m.WParamInternal)
                            {
                                case AlphaSortButtonIndex:
                                case CategorySortButtonIndex:
                                    OnViewSortButtonClick(button, EventArgs.Empty);
                                    break;
                                default:
                                    SelectViewTabButton(button, true);
                                    break;
                            }
                        }
                    }

                    return;
                }

                break;

            case AutomationMessages.PGM_GETBUTTONTEXT:
            case AutomationMessages.PGM_GETBUTTONTOOLTIPTEXT:
                if (_toolStrip is not null)
                {
                    int index = (int)m.WParamInternal;
                    if (index >= 0 && index < _toolStrip.Items.Count)
                    {
                        string? text;
                        if (m.Msg == AutomationMessages.PGM_GETBUTTONTEXT)
                        {
                            text = _toolStrip.Items[index].Text;
                        }
                        else
                        {
                            text = _toolStrip.Items[index].ToolTipText;
                        }

                        // Write text into test file.
                        m.ResultInternal = (LRESULT)AutomationMessages.WriteAutomationText(text);
                    }

                    return;
                }

                break;

            case AutomationMessages.PGM_GETTESTINGINFO:
                {
                    // Get "testing info" string for Nth grid entry (or active entry if N < 0)
                    string testingInfo = _gridView.GetTestingInfo((int)m.WParamInternal);
                    m.ResultInternal = (LRESULT)AutomationMessages.WriteAutomationText(testingInfo);
                    return;
                }

            case AutomationMessages.PGM_GETROWCOORDS:
                if (m.Msg == _copyDataMessage)
                {
                    m.ResultInternal = (LRESULT)_gridView.GetPropertyLocation(
                        _propertyName,
                        getXY: m.LParamInternal == 0,
                        rowValue: m.WParamInternal == 0u);
                    return;
                }

                break;
            case AutomationMessages.PGM_GETSELECTEDROW:
            case AutomationMessages.PGM_GETVISIBLEROWCOUNT:
                m.ResultInternal = PInvoke.SendMessage(_gridView, m.MsgInternal, m.WParamInternal, m.LParamInternal);
                return;
            case AutomationMessages.PGM_SETSELECTEDTAB:
                if (m.LParamInternal != 0)
                {
                    string? tabTypeName = AutomationMessages.ReadAutomationText(m.LParamInternal);

                    foreach (TabInfo info in _tabs)
                    {
                        if (info.Tab.GetType().FullName == tabTypeName && info.Button.Visible)
                        {
                            SelectViewTabButtonDefault(info.Button);

                            // This gets set again to 0 below. This seems wrong, but has always been this way.
                            // Leaving this should we find we need to return instead of break.
                            m.ResultInternal = (LRESULT)1;
                            break;
                        }
                    }
                }

                m.ResultInternal = (LRESULT)0;
                return;
        }

        base.WndProc(ref m);
    }
}
