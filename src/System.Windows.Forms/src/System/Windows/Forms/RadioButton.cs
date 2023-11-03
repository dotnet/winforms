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
///  Encapsulates a
///  standard
///  Windows radio button (option button).
/// </summary>
[DefaultProperty(nameof(Checked))]
[DefaultEvent(nameof(CheckedChanged))]
[DefaultBindingProperty(nameof(Checked))]
[ToolboxItem($"System.Windows.Forms.Design.AutoSizeToolboxItem,{AssemblyRef.SystemDesign}")]
[Designer($"System.Windows.Forms.Design.RadioButtonDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionRadioButton))]
public partial class RadioButton : ButtonBase
{
    private static readonly object EVENT_CHECKEDCHANGED = new();
    private const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

    // Used to see if we need to iterate through the autochecked items and modify their tabstops.
    private bool _firstfocus = true;
    private bool _isChecked;
    private bool _autoCheck = true;
    private ContentAlignment _checkAlign = ContentAlignment.MiddleLeft;
    private Appearance _appearance = Appearance.Normal;

    private const int FlatSystemStylePaddingWidth = 24;
    private const int FlatSystemStyleMinimumHeight = 13;

    internal int _flatSystemStylePaddingWidth = FlatSystemStylePaddingWidth;
    internal int _flatSystemStyleMinimumHeight = FlatSystemStyleMinimumHeight;

    /// <summary>
    ///  Initializes a new instance of the <see cref="RadioButton"/>
    ///  class.
    /// </summary>
    public RadioButton() : base()
    {
        if (DpiHelper.IsScalingRequirementMet)
        {
            _flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
            _flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
        }

        // Radio buttons shouldn't respond to right clicks, so we need to do all our own click logic
        SetStyle(ControlStyles.StandardClick, false);

        TextAlign = ContentAlignment.MiddleLeft;
        TabStop = false;
        SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="Checked"/>
    ///  value and the appearance of
    ///  the control automatically change when the control is clicked.
    /// </summary>
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RadioButtonAutoCheckDescr))]
    public bool AutoCheck
    {
        get
        {
            return _autoCheck;
        }

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
    ///  Gets or sets the appearance of the radio
    ///  button
    ///  control is drawn.
    /// </summary>
    [DefaultValue(Appearance.Normal)]
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [SRDescription(nameof(SR.RadioButtonAppearanceDescr))]
    public Appearance Appearance
    {
        get
        {
            return _appearance;
        }

        set
        {
            if (_appearance != value)
            {
                //valid values are 0x0 to 0x1
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

    private static readonly object EVENT_APPEARANCECHANGED = new();

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.RadioButtonOnAppearanceChangedDescr))]
    public event EventHandler? AppearanceChanged
    {
        add => Events.AddHandler(EVENT_APPEARANCECHANGED, value);

        remove => Events.RemoveHandler(EVENT_APPEARANCECHANGED, value);
    }

    /// <summary>
    ///  Gets or
    ///  sets the location of the check box portion of the
    ///  radio button control.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ContentAlignment.MiddleLeft)]
    [SRDescription(nameof(SR.RadioButtonCheckAlignDescr))]
    public ContentAlignment CheckAlign
    {
        get
        {
            return _checkAlign;
        }
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
    ///  Gets or sets a value indicating whether the
    ///  control is checked or not.
    /// </summary>
    [Bindable(true),
    SettingsBindable(true)]
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.RadioButtonCheckedDescr))]
    public bool Checked
    {
        get
        {
            return _isChecked;
        }

        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;

                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.BM_SETCHECK, (WPARAM)(BOOL)value);
                }

                Invalidate();
                Update();
                PerformAutoUpdates(false);
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

        if (DpiHelper.IsScalingRequirementMet)
        {
            _flatSystemStylePaddingWidth = LogicalToDeviceUnits(FlatSystemStylePaddingWidth);
            _flatSystemStyleMinimumHeight = LogicalToDeviceUnits(FlatSystemStyleMinimumHeight);
        }
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
        size.Height = DpiHelper.IsScalingRequirementMet ? Math.Max(size.Height + 5, _flatSystemStyleMinimumHeight) : size.Height + 5; // ensure minimum height to avoid truncation of RadioButton circle or text
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
            else
            {
                if (FlatStyle == FlatStyle.Standard)
                {
                    // this Rectangle will cause no Invalidation
                    // can't use Rectangle.Empty because it will cause Invalidate(ClientRectangle)
                    return new Rectangle(-1, -1, 1, 1);
                }
                else
                {
                    return Adapter.CommonLayout().Layout().CheckBounds;
                }
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
                return Adapter.CommonLayout().Layout().CheckBounds;
            }
        }
    }

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
        add => Events.AddHandler(EVENT_CHECKEDCHANGED, value);
        remove => Events.RemoveHandler(EVENT_CHECKEDCHANGED, value);
    }

    /// <summary>
    ///  Constructs the new instance of the accessibility object for this control. Subclasses
    ///  should not call base.CreateAccessibilityObject.
    /// </summary>
    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new RadioButtonAccessibleObject(this);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.BM_SETCHECK, (WPARAM)(BOOL)_isChecked);
        }
    }

    /// <summary>
    ///  Raises the <see cref="CheckBox.CheckedChanged"/>
    ///  event.
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

        ((EventHandler?)Events[EVENT_CHECKEDCHANGED])?.Invoke(this, e);
    }

    /// <summary>
    ///  We override this to implement the autoCheck functionality.
    /// </summary>
    protected override void OnClick(EventArgs e)
    {
        if (_autoCheck)
        {
            Checked = true;
        }

        base.OnClick(e);
    }

    protected override void OnEnter(EventArgs e)
    {
        // Just like the Win32 RadioButton, fire a click if the
        // user arrows onto the control..
        if (MouseButtons == MouseButtons.None)
        {
            if (PInvoke.GetKeyState((int)Keys.Tab) >= 0)
            {
                //We enter the radioButton by using arrow keys
                //Paint in raised state...
                ResetFlagsandPaint();
                if (!ValidationCancelled)
                {
                    OnClick(e);
                }
            }
            else
            {
                //we enter the radioButton by pressing Tab
                PerformAutoUpdates(true);
                //reset the TabStop so we can come back later
                //notice that PerformAutoUpdates will set the
                //TabStop of this button to false
                TabStop = true;
            }
        }

        base.OnEnter(e);
    }

    private void PerformAutoUpdates(bool tabbedInto)
    {
        if (_autoCheck)
        {
            if (_firstfocus)
            {
                WipeTabStops(tabbedInto);
            }

            TabStop = _isChecked;
            if (_isChecked)
            {
                Control? parent = ParentInternal;
                if (parent is not null)
                {
                    ControlCollection children = parent.Controls;
                    for (int i = 0; i < children.Count; i++)
                    {
                        Control ctl = children[i];
                        if (ctl != this && ctl is RadioButton radioButton)
                        {
                            RadioButton button = radioButton;
                            if (button._autoCheck && button.Checked)
                            {
                                PropertyDescriptor? propDesc = TypeDescriptor.GetProperties(this)["Checked"];
                                propDesc?.SetValue(button, false);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Removes tabstops from all radio buttons, other than the one that currently has the focus.
    /// </summary>
    private void WipeTabStops(bool tabbedInto)
    {
        Control? parent = ParentInternal;
        if (parent is not null)
        {
            ControlCollection children = parent.Controls;
            for (int i = 0; i < children.Count; i++)
            {
                Control ctl = children[i];
                if (ctl is RadioButton button)
                {
                    if (!tabbedInto)
                    {
                        button._firstfocus = false;
                    }

                    if (button._autoCheck)
                    {
                        button.TabStop = false;
                    }
                }
            }
        }
    }

    internal override ButtonBaseAdapter CreateFlatAdapter()
    {
        return new RadioButtonFlatAdapter(this);
    }

    internal override ButtonBaseAdapter CreatePopupAdapter()
    {
        return new RadioButtonPopupAdapter(this);
    }

    internal override ButtonBaseAdapter CreateStandardAdapter()
    {
        return new RadioButtonStandardAdapter(this);
    }

    private void OnAppearanceChanged(EventArgs e)
    {
        if (Events[EVENT_APPEARANCECHANGED] is EventHandler eh)
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

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s}, Checked: {Checked}";
    }
}
