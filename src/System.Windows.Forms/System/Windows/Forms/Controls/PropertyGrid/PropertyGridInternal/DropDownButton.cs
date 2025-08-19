// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.ControlPaint;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class DropDownButton : Button
{
    private bool _useComboBoxTheme;

    public DropDownButton()
    {
        SetStyle(ControlStyles.Selectable, true);
        SetAccessibleName();
    }

    /// <summary>
    ///  Indicates whether the control should be rendered in dark mode.
    ///  Set this property if you use this class for a control in dark mode.
    /// </summary>
    public bool RequestDarkModeRendering { get; set; }

    /// <summary>
    ///  Gets or sets the style used for rendering the control button.
    /// </summary>
    public ModernControlButtonStyle ControlButtonStyle { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether mouse events should be ignored when the holder is open.
    /// </summary>
    public bool IgnoreMouse { get; set; }

    /// <summary>
    ///  Indicates whether or not the control supports UIA Providers via
    ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
    /// </summary>
    internal override bool SupportsUiaProviders => true;

    public bool UseComboBoxTheme
    {
        set
        {
            if (_useComboBoxTheme != value)
            {
                _useComboBoxTheme = value;
                SetAccessibleName();

                Invalidate();
            }
        }
    }

    protected override void OnClick(EventArgs e)
    {
        if (!IgnoreMouse)
        {
            base.OnClick(e);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (!IgnoreMouse)
        {
            base.OnMouseUp(e);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!IgnoreMouse)
        {
            base.OnMouseDown(e);
        }
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        ComboBoxState state = ComboBoxState.Normal;

        if (!Enabled)
        {
            state = ComboBoxState.Disabled;
        }
        else if (MouseIsDown)
        {
            state = ComboBoxState.Pressed;
        }
        else if (MouseIsOver)
        {
            state = ComboBoxState.Hot;
        }
        else if (Focused)
        {
            state = ComboBoxState.Focused;
        }

        base.OnPaint(pevent);

        if (Application.IsDarkModeEnabled && RequestDarkModeRendering)
        {
            ModernControlButtonState buttonState = state switch
            {
                ComboBoxState.Disabled => ModernControlButtonState.Disabled,
                ComboBoxState.Hot => ModernControlButtonState.Hover,
                ComboBoxState.Pressed => ModernControlButtonState.Pressed,
                ComboBoxState.Focused => ModernControlButtonState.Focused,
                _ => ModernControlButtonState.Normal
            };

            DrawModernControlButton(
                pevent.Graphics,
                new Rectangle(0, 0, Width, Height),
                ControlButtonStyle,
                buttonState,
                isDarkMode: true);

            return;
        }

        if (Application.RenderWithVisualStyles & _useComboBoxTheme)
        {
            RenderComboBoxButtonWithVisualStyles(pevent, state);
        }
    }

    private void RenderComboBoxButtonWithVisualStyles(PaintEventArgs pevent, ComboBoxState state)
    {
        Rectangle dropDownButtonRect = new(0, 0, Width, Height);

        if (state == ComboBoxState.Normal)
        {
            pevent.Graphics.FillRectangle(
                SystemBrushes.Window,
                dropDownButtonRect);
        }

        using (DeviceContextHdcScope hdc = new(pevent))
        {
            ComboBoxRenderer.DrawDropDownButtonForHandle(
                hdc,
                dropDownButtonRect,
                state,
                ScaleHelper.IsScalingRequirementMet ? HWNDInternal : HWND.Null);
        }

        // Redraw focus cues.
        //
        // For consistency with other PropertyGrid buttons, i.e. those opening system dialogs ("..."), that
        // always show visual cues when focused, we need to do the same for this custom button, painted as
        // a ComboBox control part (drop-down).
        if (Focused)
        {
            dropDownButtonRect.Inflate(-1, -1);
            DrawFocusRectangle(
                pevent.Graphics,
                dropDownButtonRect,
                ForeColor,
                BackColor);
        }
    }

    internal void PerformButtonClick()
    {
        if (Visible && Enabled)
        {
            OnClick(EventArgs.Empty);
        }
    }

    private void SetAccessibleName()
    {
        AccessibleName = _useComboBoxTheme
            ? SR.PropertyGridDropDownButtonComboBoxAccessibleName
            : SR.PropertyGridDropDownButtonAccessibleName;
    }

    /// <summary>
    ///  Constructs the new instance of the accessibility object for this control.
    /// </summary>
    /// <returns>The accessibility object for this control.</returns>
    protected override AccessibleObject CreateAccessibilityInstance() => new DropDownButtonAccessibleObject(this);

    internal override ButtonBaseAdapter CreateStandardAdapter() => new DropDownButtonAdapter(this);
}
