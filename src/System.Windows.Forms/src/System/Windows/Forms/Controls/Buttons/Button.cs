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
///  Represents a Windows button.
/// </summary>
[SRDescription(nameof(SR.DescriptionButton))]
[Designer($"System.Windows.Forms.Design.ButtonBaseDesigner, {AssemblyRef.SystemDesign}")]
public partial class Button : ButtonBase, IButtonControl
{
    /// <summary>
    ///  The dialog result that will be sent to the parent dialog form when we are clicked.
    /// </summary>
    private DialogResult _dialogResult;

    private const int InvalidDimensionValue = int.MinValue;

    /// <summary>
    ///  For buttons whose style is <see cref="FlatStyle.Flat"/>, this property specifies the size, in pixels
    ///  of the border around the button.
    /// </summary>
    private Size _systemSize = new(InvalidDimensionValue, InvalidDimensionValue);

    /// <summary>
    ///  Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    public Button() : base()
    {
        // Buttons shouldn't respond to right clicks, so we need to do all our own click logic
        SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
    }

    /// <summary>
    ///  Allows the control to optionally shrink when <see cref="Control.AutoSize"/> is <see langword="true"/>.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(true)]
    [DefaultValue(AutoSizeMode.GrowOnly)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ControlAutoSizeModeDescr))]
    public AutoSizeMode AutoSizeMode
    {
        get => GetAutoSizeMode();
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (GetAutoSizeMode() != value)
            {
                SetAutoSizeMode(value);
                if (ParentInternal is not null)
                {
                    // DefaultLayout does not keep anchor information until it needs to. When
                    // AutoSize became a common property, we could no longer blindly call into
                    // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                    if (ParentInternal.LayoutEngine == DefaultLayout.Instance)
                    {
                        ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                    }

                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                }
            }
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new ButtonAccessibleObject(this);

    internal override ButtonBaseAdapter CreateFlatAdapter() => new ButtonFlatAdapter(this);

    internal override ButtonBaseAdapter CreatePopupAdapter() => new ButtonPopupAdapter(this);

    internal override ButtonBaseAdapter CreateStandardAdapter() => new ButtonStandardAdapter(this);

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        if (FlatStyle != FlatStyle.System)
        {
            Size preferredSize = base.GetPreferredSizeCore(proposedConstraints);
            return AutoSizeMode == AutoSizeMode.GrowAndShrink ? preferredSize : LayoutUtils.UnionSizes(preferredSize, Size);
        }

        if (_systemSize.Width == InvalidDimensionValue)
        {
            // Note: The result from the BCM_GETIDEALSIZE message isn't accurate if the font has been
            // changed, because this method is called before the font is set into the device context.
            Size requiredSize = TextRenderer.MeasureText(Text, Font);
            requiredSize = SizeFromClientSize(requiredSize);

            // This padding makes FlatStyle.System about the same size as FlatStyle.Standard
            // with an 8px font.
            requiredSize.Width += 14;
            requiredSize.Height += 9;
            _systemSize = requiredSize;
        }

        Size paddedSize = _systemSize + Padding.Size;
        return AutoSizeMode == AutoSizeMode.GrowAndShrink ? paddedSize : LayoutUtils.UnionSizes(paddedSize, Size);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_BUTTON;
            if (GetStyle(ControlStyles.UserPaint))
            {
                cp.Style |= PInvoke.BS_OWNERDRAW;
            }
            else
            {
                cp.Style |= PInvoke.BS_PUSHBUTTON;
                if (IsDefault)
                {
                    cp.Style |= PInvoke.BS_DEFPUSHBUTTON;
                }
            }

            return cp;
        }
    }

    /// <summary>
    ///  Gets or sets a value that is returned to the parent form when the button is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DialogResult.None)]
    [SRDescription(nameof(SR.ButtonDialogResultDescr))]
    public virtual DialogResult DialogResult
    {
        get => _dialogResult;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            _dialogResult = value;
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseEnter"/> event.
    /// </summary>
    protected override void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseLeave"/> event.
    /// </summary>
    protected override void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event MouseEventHandler? MouseDoubleClick
    {
        add => base.MouseDoubleClick += value;
        remove => base.MouseDoubleClick -= value;
    }

    /// <summary>
    ///  Notifies the <see cref="Button"/> whether it is the default button so that it can adjust its appearance
    ///  accordingly.
    /// </summary>
    public virtual void NotifyDefault(bool value)
    {
        if (IsDefault != value)
        {
            IsDefault = value;
        }
    }

    protected override void OnClick(EventArgs e)
    {
        if (FindForm() is { } form)
        {
            form.DialogResult = _dialogResult;
        }

        AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

        // UIA events:
        if (IsAccessibilityObjectCreated)
        {
            using var nameVariant = (VARIANT)Name;
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_NamePropertyId, nameVariant, nameVariant);
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationPropertyChangedEventId);
        }

        base.OnClick(e);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        _systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
        base.OnFontChanged(e);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        if (mevent.Button == MouseButtons.Left && MouseIsPressed)
        {
            bool isMouseDown = MouseIsDown;

            if (GetStyle(ControlStyles.UserPaint))
            {
                // Paint in raised state.
                ResetFlagsandPaint();
            }

            if (isMouseDown)
            {
                if (PInvoke.WindowFromPoint(PointToScreen(mevent.Location)) == HWND && !ValidationCancelled)
                {
                    if (GetStyle(ControlStyles.UserPaint))
                    {
                        OnClick(mevent);
                    }

                    OnMouseClick(mevent);
                }
            }
        }

        base.OnMouseUp(mevent);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        _systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
        base.OnTextChanged(e);
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

        if (ScaleHelper.IsScalingRequirementMet)
        {
            // Reset cached boundary size - it needs to be recalculated for new DPI
            _systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
        }
    }

    /// <summary>
    ///  Generates a <see cref="Control.Click"/> event for a button.
    /// </summary>
    public void PerformClick()
    {
        if (CanSelect)
        {
            bool validate = ValidateActiveControl(out bool validatedControlAllowsFocusChange);
            if (!ValidationCancelled && (validate || validatedControlAllowsFocusChange))
            {
                // Paint in raised state.
                ResetFlagsandPaint();
                OnClick(EventArgs.Empty);
            }
        }
    }

    protected internal override bool ProcessMnemonic(char charCode)
    {
        if (UseMnemonic && CanProcessMnemonic() && IsMnemonic(charCode, Text))
        {
            PerformClick();
            return true;
        }

        return base.ProcessMnemonic(charCode);
    }

    public override string ToString() => $"{base.ToString()}, Text: {Text}";

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_COMMAND:
                if (m.WParamInternal.HIWORD == PInvoke.BN_CLICKED)
                {
                    if (!ValidationCancelled)
                    {
                        OnClick(EventArgs.Empty);
                    }
                }

                break;
            case PInvokeCore.WM_ERASEBKGND:
                DefWndProc(ref m);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
