// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
///  TabPage implements a single page of a tab control. It is essentially a Panel with TabItem
///  properties.
/// </summary>
[Designer($"System.Windows.Forms.Design.TabPageDesigner, {AssemblyRef.SystemDesign}")]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultEvent("Click")]
[DefaultProperty("Text")]
public partial class TabPage : Panel
{
    private ImageList.Indexer? _imageIndexer;
    private string _toolTipText = string.Empty;
    private bool _enterFired;
    private bool _leaveFired;
    private bool _useVisualStyleBackColor;
    private List<ToolTip>? _associatedToolTips;
    private ToolTip? _externalToolTip;
    private readonly ToolTip _internalToolTip = new();
    private TabAccessibleObject? _tabAccessibilityObject;

    /// <summary>
    ///  Constructs an empty TabPage.
    /// </summary>
    public TabPage() : base()
    {
        SetStyle(ControlStyles.CacheText, true);
        Text = null;
    }

    /// <summary>
    ///  Constructs a TabPage with text for the tab.
    /// </summary>
    public TabPage(string? text) : this()
    {
        Text = text;
    }

    internal override bool AllowsKeyboardToolTip()
        => ParentInternal is TabControl tabControl && tabControl.ShowToolTips;

    /// <summary>
    ///  Allows the control to optionally shrink when AutoSize is true.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false),]
    [Localizable(false)]
    public override AutoSizeMode AutoSizeMode
    {
        get => AutoSizeMode.GrowOnly;
        set
        {
        }
    }

    /// <summary>
    ///  Hide AutoSize: it doesn't make sense for this control
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    /// <summary>
    ///  The background color of this control. This is an ambient property and will always return
    ///  a non-null value.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ControlBackColorDescr))]
    public override Color BackColor
    {
        get
        {
            Color color = base.BackColor;
#pragma warning disable WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (color != DefaultBackColor)
            {
                return color;
            }
            else if (!IsDarkModeEnabled
                && Application.RenderWithVisualStyles
                && UseVisualStyleBackColor
                && (ParentInternal is TabControl parent && parent.Appearance == TabAppearance.Normal))
            {
                return Color.Transparent;
            }
#pragma warning restore WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            return color;
        }
        set
        {
            if (DesignMode)
            {
                if (value != Color.Empty)
                {
                    PropertyDescriptor? pd = TypeDescriptor.GetProperties(this)[nameof(UseVisualStyleBackColor)];
                    pd?.SetValue(this, false);
                }
            }
            else
            {
                UseVisualStyleBackColor = false;
            }

            base.BackColor = value;
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new TabPageAccessibleObject(this);

    /// <summary>
    ///  Constructs the new instance of the Controls collection objects.
    /// </summary>
    protected override ControlCollection CreateControlsInstance() => new TabPageControlCollection(this);

    private protected override string? GetCaptionForTool(ToolTip toolTip)
    {
        // Return the internal toolTip text if it is set
        if (!string.IsNullOrEmpty(_toolTipText))
        {
            return _toolTipText;
        }

        // Return the external toolTip text for this page
        return toolTip.GetCaptionForTool(this);
    }

    private protected override IList<Rectangle> GetNeighboringToolsRectangles()
    {
        List<Rectangle> neighbors = [];

        if (ParentInternal is not TabControl tabControl)
        {
            return neighbors;
        }

        int currentIndex = tabControl.TabPages.IndexOf(this);
        if (currentIndex == -1)
        {
            return neighbors;
        }

        // Get the previous tab rectangle
        if (currentIndex > 0)
        {
            neighbors.Add(tabControl.RectangleToScreen(tabControl.GetTabRect(currentIndex - 1)));
        }

        // Get the next tab rectangle
        if (currentIndex < tabControl.TabCount - 1)
        {
            neighbors.Add(tabControl.RectangleToScreen(tabControl.GetTabRect(currentIndex + 1)));
        }

        return neighbors;
    }

    private protected override bool IsHoveredWithMouse()
    {
        if (ParentInternal is not TabControl tabControl)
        {
            return false;
        }

        // Check if any tab contains the mouse
        for (int i = 0; i < tabControl.TabCount; i++)
        {
            if (tabControl.RectangleToScreen(tabControl.GetTabRect(i)).Contains(MousePosition))
            {
                return true;
            }
        }

        // Check if the selected page contains the mouse
        TabPage? selectedTab = tabControl.SelectedTab;
        if (selectedTab is not null)
        {
            return selectedTab.AccessibilityObject.Bounds.Contains(MousePosition);
        }

        return false;
    }

    internal ImageList.Indexer ImageIndexer => _imageIndexer ??= new ImageList.Indexer();

    /// <summary>
    ///  Returns the imageIndex for the TabPage. This should point to an image
    ///  in the TabControl's associated imageList that will appear on the tab, or be -1.
    /// </summary>
    [TypeConverter(typeof(ImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(-1)]
    [SRDescription(nameof(SR.TabItemImageIndexDescr))]
    public int ImageIndex
    {
        get => ImageIndexer.Index;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);

            if (ParentInternal is TabControl parent)
            {
                ImageIndexer.ImageList = parent.ImageList;
            }

            ImageIndexer.Index = value;
            UpdateParent();
        }
    }

    /// <summary>
    ///  Returns the imageIndex for the TabPage. This should point to an image in the TabControl's
    ///  associated imageList that will appear on the tab, or be -1.
    /// </summary>
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
    [DefaultValue("")]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.TabItemImageIndexDescr))]
    [AllowNull]
    public string ImageKey
    {
        get => ImageIndexer.Key;
        set
        {
            ImageIndexer.Key = value;

            if (ParentInternal is TabControl parent)
            {
                ImageIndexer.ImageList = parent.ImageList;
            }

            UpdateParent();
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override AnchorStyles Anchor
    {
        get => base.Anchor;
        set => base.Anchor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override DockStyle Dock
    {
        get => base.Dock;
        set => base.Dock = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DockChanged
    {
        add => base.DockChanged += value;
        remove => base.DockChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool Enabled
    {
        get => base.Enabled;
        set => base.Enabled = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? EnabledChanged
    {
        add => base.EnabledChanged += value;
        remove => base.EnabledChanged -= value;
    }

    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TabItemUseVisualStyleBackColorDescr))]
    public bool UseVisualStyleBackColor
    {
        get => _useVisualStyleBackColor;
        set
        {
            if (_useVisualStyleBackColor == value)
            {
                return;
            }

            _useVisualStyleBackColor = value;
            Invalidate(true);
        }
    }

    /// <summary>
    ///  Make the Location property non-browsable for the tab pages.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Point Location
    {
        get => base.Location;
        set => base.Location = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? LocationChanged
    {
        add => base.LocationChanged += value;
        remove => base.LocationChanged -= value;
    }

    [DefaultValue(typeof(Size), "0, 0")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Size MaximumSize
    {
        get => base.MaximumSize;
        set => base.MaximumSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Size MinimumSize
    {
        get => base.MinimumSize;
        set => base.MinimumSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Size PreferredSize => base.PreferredSize;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int TabIndex
    {
        get => base.TabIndex;
        set => base.TabIndex = value;
    }

    /// <summary>
    ///  This property is required by certain controls (TabPage) to render its transparency using
    ///  theming API. We don't want all controls (that are have transparent BackColor) to use
    ///  theming API to render its background because it has large performance cost.
    /// </summary>
    internal override bool RenderTransparencyWithVisualStyles => true;

    internal override bool SupportsUiaProviders => true;

    internal TabAccessibleObject TabAccessibilityObject => _tabAccessibilityObject ??= new TabAccessibleObject(this);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabIndexChanged
    {
        add => base.TabIndexChanged += value;
        remove => base.TabIndexChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    [Localizable(true)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            UpdateParent();
        }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    ///  The toolTipText for the tab, that will appear when the mouse hovers over the tab and the
    ///  TabControl's showToolTips property is true.
    /// </summary>
    [DefaultValue("")]
    [Localizable(true)]
    [SRDescription(nameof(SR.TabItemToolTipTextDescr))]
    [AllowNull]
    public string ToolTipText
    {
        get => _toolTipText;
        set
        {
            value ??= string.Empty;

            if (value == _toolTipText)
            {
                return;
            }

            _toolTipText = value;
            UpdateParent();

            if (_externalToolTip is null && _associatedToolTips is null)
            {
                _internalToolTip.SetToolTip(this, value);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool Visible
    {
        get => base.Visible;
        set => base.Visible = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? VisibleChanged
    {
        add => base.VisibleChanged += value;
        remove => base.VisibleChanged -= value;
    }

    /// <summary>
    ///  Assigns a new parent control. Sends out the appropriate property change notifications for
    ///  properties that are affected by the change of parent.
    /// </summary>
    internal override void AssignParent(Control? value)
    {
        if (value is not null and not TabControl)
        {
            throw new ArgumentException(string.Format(SR.TabControlTabPageNotOnTabControl, value.GetType().FullName));
        }

        base.AssignParent(value);
    }

    /// <summary>
    ///  Given a component, this retrieves the tab page that it's parented to, or null if it's not
    ///  parented to any tab page.
    /// </summary>
    public static TabPage? GetTabPageOfComponent(object? comp)
    {
        Control? c = comp as Control;
        if (c is null)
        {
            return null;
        }

        while (c is not null and not TabPage)
        {
            c = c.ParentInternal;
        }

        return (TabPage?)c;
    }

    internal Rectangle GetPageRectangle() => base.GetToolNativeScreenRectangle();

    internal override Rectangle GetToolNativeScreenRectangle()
    {
        // Check SelectedIndex of the parental TabControl instead of SelectedTab
        // because it is used in GetTabRect next.
        // So check this to make sure that the value is correct
        // to avoid ArgumentOutOfRangeException in GetTabRect.
        if (ParentInternal is TabControl tabControl && tabControl.SelectedIndex >= 0)
        {
            Rectangle rect = tabControl.GetTabRect(tabControl.SelectedIndex);
            return tabControl.RectangleToScreen(rect);
        }

        return Rectangle.Empty;
    }

    /// <summary>
    ///  This is an internal method called by the TabControl to fire the Leave event when TabControl leave occurs.
    /// </summary>
    internal void FireLeave(EventArgs e)
    {
        _leaveFired = true;
        OnLeave(e);
    }

    /// <summary>
    ///  This is an internal method called by the TabControl to fire the Enter event when TabControl leave occurs.
    /// </summary>
    internal void FireEnter(EventArgs e)
    {
        _enterFired = true;
        OnEnter(e);
    }

    /// <summary>
    ///  Actually goes and fires the OnEnter event. Inheriting controls should use this to know
    ///  when the event is fired [this is preferable to adding an event handler on yourself for
    ///  this event]. They should, however, remember to call base.OnEnter(e); to ensure the event
    ///  i still fired to external listeners
    ///  This listener is overidden so that we can fire SAME ENTER and LEAVE events on the TabPage.
    ///  TabPage should fire enter when the focus is on the TabPage and not when the control
    ///  within the TabPage gets Focused.
    /// </summary>
    protected internal override void OnEnter(EventArgs e)
    {
        if (ParentInternal is TabControl)
        {
            if (_enterFired)
            {
                base.OnEnter(e);
            }

            _enterFired = false;
        }
    }

    /// <summary>
    ///  Actually goes and fires the OnLeave event. Inheriting controls should use this to know
    ///  when the event is fired [this is preferable to adding an event handler on yourself for
    ///  this event]. They should, however, remember to call base.OnLeave(e); to ensure the event
    ///  is still fired to external listeners
    ///  This listener is overidden so that we can fire same enter and leave events on the TabPage.
    ///  TabPage should fire enter when the focus is on the TabPage and not when the control within
    ///  the TabPage gets Focused.
    ///  Similary the Leave should fire when the TabControl (and hence the TabPage) loses focus.
    /// </summary>
    protected internal override void OnLeave(EventArgs e)
    {
        if (ParentInternal is TabControl)
        {
            if (_leaveFired)
            {
                base.OnLeave(e);
            }

            _leaveFired = false;
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Utilize the TabRenderer new to Whidbey to draw the tab pages so that the panels are
        // drawn using the correct visual styles when the application supports using visual
        // styles.

        // Utilize the UseVisualStyleBackColor property to determine whether or not the themed
        // background should be utilized.
        if (Application.RenderWithVisualStyles
            && UseVisualStyleBackColor
            && (ParentInternal is TabControl parent && parent.Appearance == TabAppearance.Normal))
        {
#pragma warning disable WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            Color bkColor = (UseVisualStyleBackColor && !IsDarkModeEnabled)
                ? Color.Transparent
                : BackColor;
#pragma warning restore WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            Rectangle inflateRect = LayoutUtils.InflateRect(DisplayRectangle, Padding);

            // To ensure that the TabPage draws correctly (the border will get clipped and
            // and gradient fill will match correctly with the TabControl). Unfortunately,
            // there is no good way to determine the padding used on the TabPage.
            Rectangle rectWithBorder = new(
                inflateRect.X - 4,
                inflateRect.Y - 2,
                inflateRect.Width + 8,
                inflateRect.Height + 6);

            TabRenderer.DrawTabPage(e, rectWithBorder);

            // TabRenderer does not support painting the background image on the panel, so
            // draw it ourselves.
            if (BackgroundImage is not null)
            {
                ControlPaint.DrawBackgroundImage(
                    e.Graphics,
                    BackgroundImage,
                    bkColor,
                    BackgroundImageLayout,
                    inflateRect,
                    inflateRect,
                    DisplayRectangle.Location);
            }
        }
        else
        {
            base.OnPaintBackground(e);
        }
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        PInvoke.UiaDisconnectProvider(_tabAccessibilityObject);
        _tabAccessibilityObject = null;

        base.ReleaseUiaProvider(handle);
    }

    internal override void RemoveToolTip(ToolTip toolTip)
    {
        // If a user used one ToolTIp instance to set a toolTip text before.
        if (_associatedToolTips is null)
        {
            Debug.Assert(_externalToolTip == toolTip, "RemoveToolTip should remove a toolTip that was set.");
            _externalToolTip = null;
            _internalToolTip.SetToolTip(this, ToolTipText);
            return;
        }

        if (_associatedToolTips.Contains(toolTip))
        {
            _associatedToolTips.Remove(toolTip);
        }
        else
        {
            Debug.Fail("RemoveToolTip should remove a toolTip that was set.");
        }

        // If there is only one associated toolTip set it as _externalToolTip
        // and remove the List collection to improve performance.
        if (_associatedToolTips.Count == 1)
        {
            _externalToolTip = _associatedToolTips[0];
            _associatedToolTips = null;
        }
    }

    /// <summary>
    ///  Usually users create one ToolTip instance and set toolTip texts for several controls using this instance.
    ///  This method will store the link to this ToolTip instance in _externalToolTip
    ///  to use it as a base for a keyboard toolTip instead _internalToolTip instance.
    ///  That is strange and unexpected but a user can set several toolTip instances for this TabPage,
    ///  in this case, we have to check all associated toolTips.
    ///  Because of that, create a new List collection to do that.
    /// </summary>
    internal override void SetToolTip(ToolTip toolTip)
    {
        // "_externalToolTip == toolTip" condition means a user just set a new text using a ToolTip instance
        // that was already set for this TabPage.
        if (toolTip is null || _externalToolTip == toolTip)
        {
            return;
        }

        // If a user sets toolTip text using a ToolTip instance first time.
        // In this case, use external ToolTip instance to show keyboard toolTip instead internal one.
        if (_externalToolTip is null)
        {
            _externalToolTip = toolTip;
            _internalToolTip.RemoveAll();
            return;
        }

        // If a user sets a toolTip text for this TabPage using one more ToolTip instance.
        // Use the List collection to track all associated toolTips.
        // In this case, the keyboard toolTip will show the text of the latest toolTip instance that was set.
        if (_associatedToolTips is null)
        {
            _associatedToolTips = [_externalToolTip, toolTip];
            return;
        }

        if (!_associatedToolTips.Contains(toolTip))
        {
            _associatedToolTips.Add(toolTip);
        }
    }

    /// <summary>
    ///  Overrides main setting of our bounds so that we can control our size and that of our
    ///  TabPages.
    /// </summary>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        Control? parent = ParentInternal;

        if (parent is TabControl && parent.IsHandleCreated)
        {
            Rectangle r = parent.DisplayRectangle;

            // LayoutEngines send BoundsSpecified.None so they can know they are the ones causing the size change
            // in the subsequent InitLayout. We need to be careful preserve a None.
            base.SetBoundsCore(r.X, r.Y, r.Width, r.Height, specified == BoundsSpecified.None ? BoundsSpecified.None : BoundsSpecified.All);
        }
        else
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }
    }

    /// <summary>
    ///  Determines if the Location property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeLocation() => Left != 0 || Top != 0;

    /// <summary>
    ///  The text property is what is returned for the TabPages default printing.
    /// </summary>
    public override string ToString() => $"TabPage: {{{Text}}}";

    internal void UpdateParent()
    {
        if (ParentInternal is TabControl parent)
        {
            parent.UpdateTab(this);
        }
    }
}
