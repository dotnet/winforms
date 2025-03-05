// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class DropDownButton : Button
{
    private bool _useComboBoxTheme;

    public DropDownButton()
    {
        SetStyle(ControlStyles.Selectable, true);
        SetAccessibleName();
    }

    // When the holder is open, we don't fire clicks.
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
        base.OnPaint(pevent);

        if (Application.RenderWithVisualStyles & _useComboBoxTheme)
        {
            ComboBoxState state = ComboBoxState.Normal;

            if (MouseIsDown)
            {
                state = ComboBoxState.Pressed;
            }
            else if (MouseIsOver)
            {
                state = ComboBoxState.Hot;
            }

            Rectangle dropDownButtonRect = new(0, 0, Width, Height);
            if (state == ComboBoxState.Normal)
            {
                pevent.Graphics.FillRectangle(SystemBrushes.Window, dropDownButtonRect);
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
                ControlPaint.DrawFocusRectangle(pevent.Graphics, dropDownButtonRect, ForeColor, BackColor);
            }
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
