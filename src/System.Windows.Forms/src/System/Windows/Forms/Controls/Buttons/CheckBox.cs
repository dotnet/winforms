// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Rendering.CheckBox;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Represents a Windows check box.
/// </summary>
[DefaultProperty(nameof(Checked))]
[DefaultEvent(nameof(CheckedChanged))]
[DefaultBindingProperty(nameof(CheckState))]
[ToolboxItem($"System.Windows.Forms.Design.AutoSizeToolboxItem,{AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionCheckBox))]
public partial class CheckBox : ButtonBase
{
    private static readonly object s_checkedChangedEvent = new();
    private static readonly object s_checkStateChangedEvent = new();
    private static readonly object s_appearanceChangedEvent = new();
    private const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

    private ContentAlignment _checkAlign = ContentAlignment.MiddleLeft;
    private CheckState _checkState;
    private Appearance _appearance;

    private int _flatSystemStylePaddingWidth;
    private int _flatSystemStyleMinimumHeight;

    // A flag indicating if UIA StateChanged event needs to be triggered,
    // to avoid double-triggering when Checked value changes.
    private bool _notifyAccessibilityStateChangedNeeded;
    private AnimatedToggleSwitchRenderer? _toggleSwitchRenderer;

    /// <summary>
    ///  Initializes a new instance of the <see cref="CheckBox"/> class.
    /// </summary>
    public CheckBox() : base()
    {
        // Checkboxes shouldn't respond to right clicks, so we need to do all our own click logic
        SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
        SetAutoSizeMode(AutoSizeMode.GrowAndShrink);

        AutoCheck = true;
        TextAlign = ContentAlignment.MiddleLeft;
    }

    private bool AccObjDoDefaultAction { get; set; }

    /// <summary>
    ///  Gets or sets the value that determines the appearance of a check box control.
    /// </summary>
    [DefaultValue(Appearance.Normal)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.CheckBoxAppearanceDescr))]
    public Appearance Appearance
    {
        get => _appearance;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_appearance == value)
            {
                return;
            }

            using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Appearance))
            {
                _appearance = value;
                if (OwnerDraw)
                {
                    Refresh();
                }
                else
                {
                    UpdateStyles();

#pragma warning disable WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                    if (value == Appearance.ToggleSwitch
                        && VisualStylesMode == VisualStylesMode.Latest)
                    {
                        Refresh();
                    }
#pragma warning restore WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                }

                OnAppearanceChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.CheckBoxOnAppearanceChangedDescr))]
    public event EventHandler? AppearanceChanged
    {
        add => Events.AddHandler(s_appearanceChangedEvent, value);
        remove => Events.RemoveHandler(s_appearanceChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="Checked"/> or <see cref="CheckState"/>
    ///  value and the check box's appearance are automatically changed when it is clicked.
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.CheckBoxAutoCheckDescr))]
    public bool AutoCheck { get; set; }

    /// <summary>
    ///  Gets or sets the horizontal and vertical alignment of a check box on a check box control.
    /// </summary>
    [Bindable(true)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ContentAlignment.MiddleLeft)]
    [SRDescription(nameof(SR.CheckBoxCheckAlignDescr))]
    public ContentAlignment CheckAlign
    {
        get => _checkAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_checkAlign == value)
            {
                return;
            }

            _checkAlign = value;
            LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.CheckAlign);
            if (OwnerDraw)
            {
                Invalidate();
            }
            else
            {
                UpdateStyles();
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the check box is checked.
    /// </summary>
    [Bindable(true),
    SettingsBindable(true)]
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.CheckBoxCheckedDescr))]
    public bool Checked
    {
        get => _checkState != CheckState.Unchecked;
        set
        {
            if (value != Checked)
            {
                CheckState = value ? CheckState.Checked : CheckState.Unchecked;
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the check box is checked.
    /// </summary>
    [Bindable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(CheckState.Unchecked)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.CheckBoxCheckStateDescr))]
    public CheckState CheckState
    {
        get => _checkState;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_checkState == value)
            {
                return;
            }

            bool oldChecked = Checked;

            _checkState = value;

            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.BM_SETCHECK, (WPARAM)(int)_checkState);
            }

            bool checkedChanged = oldChecked != Checked;

            if (checkedChanged)
            {
                OnCheckedChanged(EventArgs.Empty);
            }

            _notifyAccessibilityStateChangedNeeded = !checkedChanged;
            OnCheckStateChanged(EventArgs.Empty);
            _notifyAccessibilityStateChangedNeeded = false;
        }
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseDoubleClick
    {
        add => base.MouseDoubleClick += value;
        remove => base.MouseDoubleClick -= value;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_BUTTON;

            if (OwnerDraw)
            {
                cp.Style |= PInvoke.BS_OWNERDRAW;
            }
            else
            {
#pragma warning disable WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.
                if (VisualStylesMode == VisualStylesMode.Latest)
                {
                    SetStyle(ControlStyles.UserPaint, true);
                    SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    SetStyle(ControlStyles.ResizeRedraw, true);
                    cp.Style |= PInvoke.BS_OWNERDRAW;
                }
                else
                {
                    cp.Style |= PInvoke.BS_3STATE;
                    if (Appearance == Appearance.Button)
                    {
                        cp.Style |= PInvoke.BS_PUSHLIKE;
                    }
                }
#pragma warning restore WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.

                // Determine the alignment of the check box
                ContentAlignment align = RtlTranslateContent(CheckAlign);
                if ((align & AnyRight) != 0)
                {
                    cp.Style |= PInvoke.BS_RIGHTBUTTON;
                }
            }

            return cp;
        }
    }

    protected override Size DefaultSize => new(104, 24);

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ScaleConstants();
    }

    private protected override void InitializeConstantsForInitialDpi(int initialDpi) => ScaleConstants();

    private void ScaleConstants()
    {
        const int LogicalFlatSystemStylePaddingWidth = 25;
        const int LogicalFlatSystemStyleMinimumHeight = 13;

        _flatSystemStylePaddingWidth = LogicalToDeviceUnits(LogicalFlatSystemStylePaddingWidth);
        _flatSystemStyleMinimumHeight = LogicalToDeviceUnits(LogicalFlatSystemStyleMinimumHeight);
    }

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        Size textSize;

#pragma warning disable WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Appearance == Appearance.ToggleSwitch
            && VisualStylesMode == VisualStylesMode.Latest)
        {
            _toggleSwitchRenderer ??= new AnimatedToggleSwitchRenderer(this, ModernCheckBoxStyle.Rounded);
            int dpiScale = (int)(DeviceDpi / 96f);

            textSize = TextRenderer.MeasureText(Text, Font);
            int switchWidth = 50 * dpiScale;
            int switchHeight = 25 * dpiScale;
            int totalWidth = textSize.Width + switchWidth + 20 * dpiScale; // 10 dpi padding on each side
            int totalHeight = Math.Max(textSize.Height, switchHeight);

            return new Size(totalWidth, totalHeight);
        }
#pragma warning restore WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (Appearance == Appearance.Button)
        {
            ButtonStandardAdapter adapter = new(this);
            return adapter.GetPreferredSizeCore(proposedConstraints);
        }

        if (FlatStyle != FlatStyle.System)
        {
            return base.GetPreferredSizeCore(proposedConstraints);
        }

        textSize = TextRenderer.MeasureText(Text, Font);
        Size size = SizeFromClientSize(textSize);
        size.Width += _flatSystemStylePaddingWidth;

        // Ensure minimum height to avoid truncation of check-box or text
        size.Height = Math.Max(size.Height + 5, _flatSystemStyleMinimumHeight);
        return size + Padding.Size;
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
#pragma warning disable WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (VisualStylesMode == VisualStylesMode.Latest
            && Appearance == Appearance.ToggleSwitch)
        {
            _toggleSwitchRenderer?.RenderControl(pevent.Graphics);
            return;
        }
#pragma warning restore WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        base.OnPaint(pevent);
    }

    internal override Rectangle OverChangeRectangle
    {
        get
        {
            if (Appearance == Appearance.Button)
            {
                return base.OverChangeRectangle;
            }
            else if (FlatStyle == FlatStyle.Standard)
            {
                // Return an out of bounds rectangle to avoid invalidation.
                return new Rectangle(-1, -1, 1, 1);
            }
            else
            {
                // Popup mouseover rectangle is actually bigger than GetCheckmarkRectangle
                return Adapter.CommonLayout().Layout().CheckBounds;
            }
        }
    }

    internal override Rectangle DownChangeRectangle
    {
        get
        {
            if (Appearance == Appearance.Button || FlatStyle == FlatStyle.System)
            {
                return base.DownChangeRectangle;
            }
            else
            {
                // Popup mouseover rectangle is actually bigger than GetCheckmarkRectangle()
                return Adapter.CommonLayout().Layout().CheckBounds;
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Gets or sets a value indicating the alignment of the text on the checkbox control.
    /// </summary>
    [Localizable(true)]
    [DefaultValue(ContentAlignment.MiddleLeft)]
    public override ContentAlignment TextAlign
    {
        get => base.TextAlign;
        set => base.TextAlign = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the check box will allow three check states rather than two.
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.CheckBoxThreeStateDescr))]
    public bool ThreeState { get; set; }

    /// <summary>
    ///  Occurs when the value of the <see cref="Checked"/> property changes.
    /// </summary>
    [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
    public event EventHandler? CheckedChanged
    {
        add => Events.AddHandler(s_checkedChangedEvent, value);
        remove => Events.RemoveHandler(s_checkedChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the value of the <see cref="CheckState"/> property changes.
    /// </summary>
    [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
    public event EventHandler? CheckStateChanged
    {
        add => Events.AddHandler(s_checkStateChangedEvent, value);
        remove => Events.RemoveHandler(s_checkStateChangedEvent, value);
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new CheckBoxAccessibleObject(this);

    protected virtual void OnAppearanceChanged(EventArgs e)
    {
        if (Events[s_appearanceChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="CheckedChanged"/>  event.
    /// </summary>
    protected virtual void OnCheckedChanged(EventArgs e)
    {
        NotifyAccessibilityStateChanged();

        ((EventHandler?)Events[s_checkedChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="CheckStateChanged"/> event.
    /// </summary>
    protected virtual void OnCheckStateChanged(EventArgs e)
    {
#pragma warning disable WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (VisualStylesMode == VisualStylesMode.Latest
            && Appearance == Appearance.ToggleSwitch)
        {
            _toggleSwitchRenderer?.RestartAnimation();

            Refresh();
        }
#pragma warning restore WFO9000 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (OwnerDraw)
        {
            Refresh();
        }

        if (_notifyAccessibilityStateChangedNeeded)
        {
            NotifyAccessibilityStateChanged();
        }

        ((EventHandler?)Events[s_checkStateChangedEvent])?.Invoke(this, e);
    }

    private void NotifyAccessibilityStateChanged()
    {
        if (FlatStyle == FlatStyle.System)
        {
            AccessibilityNotifyClients(AccessibleEvents.SystemCaptureStart, -1);
        }

        // MSAA events:
        AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

        // UIA events:
        if (IsAccessibilityObjectCreated)
        {
            using var nameVariant = (VARIANT)Name;
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_NamePropertyId, nameVariant, nameVariant);
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationPropertyChangedEventId);
        }

        if (FlatStyle == FlatStyle.System)
        {
            AccessibilityNotifyClients(AccessibleEvents.SystemCaptureEnd, -1);
        }
    }

    protected override void OnClick(EventArgs e)
    {
        if (AutoCheck)
        {
            switch (CheckState)
            {
                case CheckState.Unchecked:
                    CheckState = CheckState.Checked;
                    break;
                case CheckState.Checked:
                    if (ThreeState)
                    {
                        CheckState = CheckState.Indeterminate;

                        // If the check box is clicked as a result of AccObj::DoDefaultAction then the native check box
                        // does not fire OBJ_STATE_CHANGE event when going to Indeterminate state and we need to.
                        if (AccObjDoDefaultAction)
                        {
                            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
                        }
                    }
                    else
                    {
                        CheckState = CheckState.Unchecked;
                    }

                    break;
                default:
                    CheckState = CheckState.Unchecked;
                    break;
            }
        }

        base.OnClick(e);
    }

    /// <summary>
    ///  We override this to ensure that the control's click values are set up correctly.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.BM_SETCHECK, (WPARAM)(int)_checkState);
        }
    }

    /// <summary>
    ///  Raises the <see cref="ButtonBase.OnMouseUp"/> event.
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        // It's best not to have the mouse captured while running Click events
        if (mevent.Button == MouseButtons.Left
            && MouseIsPressed
            && MouseIsDown
            && PInvoke.WindowFromPoint(PointToScreen(mevent.Location)) == HWND)
        {
            // Paint in raised state.
            ResetFlagsandPaint();
            if (!ValidationCancelled)
            {
                if (Capture)
                {
                    OnClick(mevent);
                }

                OnMouseClick(mevent);
            }
        }

        base.OnMouseUp(mevent);
    }

    internal override ButtonBaseAdapter CreateFlatAdapter() => new CheckBoxFlatAdapter(this);

    internal override ButtonBaseAdapter CreatePopupAdapter() => new CheckBoxPopupAdapter(this);

    internal override ButtonBaseAdapter CreateStandardAdapter() => new CheckBoxStandardAdapter(this);

    /// <summary>
    ///  Overridden to handle mnemonics properly.
    /// </summary>
    protected internal override bool ProcessMnemonic(char charCode)
    {
        if (UseMnemonic && IsMnemonic(charCode, Text) && CanSelect)
        {
            if (Focus())
            {
                // Paint in raised state...
                ResetFlagsandPaint();
                if (!ValidationCancelled)
                {
                    OnClick(EventArgs.Empty);
                }
            }

            return true;
        }

        return false;
    }

    public override string ToString() => $"{base.ToString()}, CheckState: {(int)CheckState}";
}
