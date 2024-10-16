// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Encapsulates a standard Windows radio button (option button).
/// </summary>
[DefaultProperty(nameof(Checked))]
[DefaultEvent(nameof(CheckedChanged))]
[DefaultBindingProperty(nameof(Checked))]
[ToolboxItem($"System.Windows.Forms.Design.AutoSizeToolboxItem,{AssemblyRef.SystemDesign}")]
[Designer($"System.Windows.Forms.Design.RadioButtonDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionRadioButton))]
public partial class RadioButton : ButtonBase
{
    private static readonly object s_checkedChangedEvent = new();
    private static readonly object s_appearanceChangeEvent = new();

    private const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

    // Used to see if we need to iterate through the auto checked items and modify their tab stops.
    private bool _firstFocus = true;
    private bool _isChecked;
    private bool _autoCheck = true;
    private ContentAlignment _checkAlign = ContentAlignment.MiddleLeft;
    private Appearance _appearance = Appearance.Normal;
    private int _flatSystemStylePaddingWidth;
    private int _flatSystemStyleMinimumHeight;

    /// <summary>
    ///  Initializes a new instance of the <see cref="RadioButton"/> class.
    /// </summary>
    public RadioButton() : base()
    {
        ScaleConstants();

        // Radio buttons shouldn't respond to right clicks, so we need to do all our own click logic
        SetStyle(ControlStyles.StandardClick, false);

        TextAlign = ContentAlignment.MiddleLeft;
        TabStop = false;
        SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="Checked"/> value and the appearance of
    ///  the control automatically change when the control is clicked.
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RadioButtonAutoCheckDescr))]
    public bool AutoCheck
    {
        get => _autoCheck;
        set
        {
            if (_autoCheck != value)
            {
                _autoCheck = value;
                PerformAutoUpdates(false);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the appearance of the radio button control is drawn.
    /// </summary>
    [DefaultValue(Appearance.Normal)]
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.RadioButtonAppearanceDescr))]
    public Appearance Appearance
    {
        get => _appearance;
        set
        {
            if (_appearance != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

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
                    }

                    OnAppearanceChanged(EventArgs.Empty);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.RadioButtonOnAppearanceChangedDescr))]
    public event EventHandler? AppearanceChanged
    {
        add => Events.AddHandler(s_appearanceChangeEvent, value);
        remove => Events.RemoveHandler(s_appearanceChangeEvent, value);
    }

    /// <summary>
    ///  Gets or sets the location of the check box portion of the radio button control.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ContentAlignment.MiddleLeft)]
    [SRDescription(nameof(SR.RadioButtonCheckAlignDescr))]
    public ContentAlignment CheckAlign
    {
        get => _checkAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            _checkAlign = value;
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
    ///  Gets or sets a value indicating whether the control is checked or not.
    /// </summary>
    [Bindable(true),
    SettingsBindable(true)]
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.RadioButtonCheckedDescr))]
    public bool Checked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;

                if (IsHandleCreated)
                {
                    PInvokeCore.SendMessage(this, PInvoke.BM_SETCHECK, (WPARAM)(BOOL)value);
                }

                Invalidate();
                Update();
                PerformAutoUpdates(tabbedInto: false);
                OnCheckedChanged(EventArgs.Empty);
            }
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
                cp.Style |= PInvoke.BS_RADIOBUTTON;
                if (Appearance == Appearance.Button)
                {
                    cp.Style |= PInvoke.BS_PUSHLIKE;
                }

                // Determine the alignment of the radio button
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

    /// <summary>
    ///  When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
    ///  For RadioButton controls, scale the width of the system-style padding and height of the radio button image.
    ///  Must call the base class method to get the current DPI values. This method is invoked only when
    ///  Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has
    ///  EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
    /// </summary>
    /// <param name="deviceDpiOld">Old DPI value</param>
    /// <param name="deviceDpiNew">New DPI value</param>
    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ScaleConstants();
    }

    private protected override void InitializeConstantsForInitialDpi(int initialDpi)
        => ScaleConstants();

    private void ScaleConstants()
    {
        const int LogicalFlatSystemStylePaddingWidth = 24;
        const int LogicalFlatSystemStyleMinimumHeight = 13;
        _flatSystemStylePaddingWidth = LogicalToDeviceUnits(LogicalFlatSystemStylePaddingWidth);
        _flatSystemStyleMinimumHeight = LogicalToDeviceUnits(LogicalFlatSystemStyleMinimumHeight);
    }

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        if (FlatStyle != FlatStyle.System)
        {
            return base.GetPreferredSizeCore(proposedConstraints);
        }

        Size textSize = TextRenderer.MeasureText(Text, Font);
        Size size = SizeFromClientSize(textSize);
        size.Width += _flatSystemStylePaddingWidth;

        // Ensure minimum height to avoid truncation of RadioButton circle or text.
        size.Height = ScaleHelper.IsScalingRequirementMet
            ? Math.Max(size.Height + 5, _flatSystemStyleMinimumHeight)
            : size.Height + 5;

        return size;
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
                return Adapter.CommonLayout().Layout().CheckBounds;
            }
        }
    }

    internal override Rectangle DownChangeRectangle =>
        Appearance == Appearance.Button || FlatStyle == FlatStyle.System
                ? base.DownChangeRectangle
                : Adapter.CommonLayout().Layout().CheckBounds;

    internal override bool SupportsUiaProviders => true;

    [DefaultValue(false)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    /// <summary>
    ///  Gets or sets the value indicating whether the user can give the focus to this
    ///  control using the TAB key.
    /// </summary>
    [Localizable(true)]
    [DefaultValue(ContentAlignment.MiddleLeft)]
    public override ContentAlignment TextAlign
    {
        get => base.TextAlign;
        set => base.TextAlign = value;
    }

    /// <summary>
    ///  Occurs when the
    ///  value of the <see cref="Checked"/>
    ///  property changes.
    /// </summary>
    [SRDescription(nameof(SR.RadioButtonOnCheckedChangedDescr))]
    public event EventHandler? CheckedChanged
    {
        add => Events.AddHandler(s_checkedChangedEvent, value);
        remove => Events.RemoveHandler(s_checkedChangedEvent, value);
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new RadioButtonAccessibleObject(this);

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (IsHandleCreated)
        {
            PInvokeCore.SendMessage(this, PInvoke.BM_SETCHECK, (WPARAM)(BOOL)_isChecked);
        }
    }

    /// <summary>
    ///  Raises the <see cref="CheckBox.CheckedChanged"/> event.
    /// </summary>
    protected virtual void OnCheckedChanged(EventArgs e)
    {
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

        ((EventHandler?)Events[s_checkedChangedEvent])?.Invoke(this, e);
    }

    protected override void OnClick(EventArgs e)
    {
        if (_autoCheck)
        {
            Checked = true;
        }

        base.OnClick(e);
    }

    protected internal override void OnEnter(EventArgs e)
    {
        // Just like the Win32 RadioButton, fire a click if the user arrows onto the control.
        if (MouseButtons == MouseButtons.None)
        {
            if (PInvoke.GetKeyState((int)Keys.Tab) >= 0)
            {
                // Entered the radio button by using arrow keys.
                // Paint in raised state.
                ResetFlagsandPaint();
                if (!ValidationCancelled)
                {
                    OnClick(e);
                }
            }
            else
            {
                // Entered the radio button by pressing Tab
                PerformAutoUpdates(true);

                // Reset the TabStop so we can come back later. PerformAutoUpdates will set TabStop to false.
                TabStop = true;
            }
        }

        base.OnEnter(e);
    }

    private void PerformAutoUpdates(bool tabbedInto)
    {
        if (!_autoCheck)
        {
            return;
        }

        if (_firstFocus)
        {
            WipeTabStops(tabbedInto);
        }

        TabStop = _isChecked;
        if (!_isChecked || ParentInternal is not { } parent)
        {
            return;
        }

        ControlCollection children = parent.Controls;
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] is RadioButton radioButton
                && radioButton != this
                && radioButton.AutoCheck
                && radioButton.Checked)
            {
                radioButton.Checked = false;
            }
        }
    }

    /// <summary>
    ///  Removes tab stops from all radio buttons, other than the one that currently has the focus.
    /// </summary>
    private void WipeTabStops(bool tabbedInto)
    {
        if (ParentInternal is not { } parent)
        {
            return;
        }

        ControlCollection children = parent.Controls;
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] is RadioButton button)
            {
                if (!tabbedInto)
                {
                    button._firstFocus = false;
                }

                if (button._autoCheck)
                {
                    button.TabStop = false;
                }
            }
        }
    }

    internal override ButtonBaseAdapter CreateFlatAdapter() => new RadioButtonFlatAdapter(this);

    internal override ButtonBaseAdapter CreatePopupAdapter() => new RadioButtonPopupAdapter(this);

    internal override ButtonBaseAdapter CreateStandardAdapter() => new RadioButtonStandardAdapter(this);

    private void OnAppearanceChanged(EventArgs e)
    {
        if (Events[s_appearanceChangeEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        if (mevent.Button == MouseButtons.Left
            && GetStyle(ControlStyles.UserPaint)
            && MouseIsDown
            && PInvoke.WindowFromPoint(PointToScreen(mevent.Location)) == HWND)
        {
            // Paint in raised state.
            ResetFlagsandPaint();
            if (!ValidationCancelled)
            {
                OnClick(mevent);
                OnMouseClick(mevent);
            }
        }

        base.OnMouseUp(mevent);
    }

    /// <summary>
    ///  Generates a <see cref="Control.Click"/> event for the button, simulating a click by a user.
    /// </summary>
    public void PerformClick()
    {
        if (CanSelect)
        {
            // Paint in raised state.
            ResetFlagsandPaint();
            if (!ValidationCancelled)
            {
                OnClick(EventArgs.Empty);
            }
        }
    }

    protected internal override bool ProcessMnemonic(char charCode)
    {
        if (UseMnemonic && IsMnemonic(charCode, Text) && CanSelect)
        {
            if (!Focused)
            {
                // This will cause an OnEnter event, which in turn will fire the click event.
                Focus();
            }
            else
            {
                // Generate a click if already focused.
                PerformClick();
            }

            return true;
        }

        return false;
    }

    public override string ToString() => $"{base.ToString()}, Checked: {Checked}";
}
