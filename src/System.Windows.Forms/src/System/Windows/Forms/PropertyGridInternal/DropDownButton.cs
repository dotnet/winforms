// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal sealed class DropDownButton : Button
    {
        private bool _useComboBoxTheme;

        public DropDownButton()
        {
            SetStyle(ControlStyles.Selectable, true);
            SetAccessibleName();
        }

        // when the holder is open, we don't fire clicks
        //
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
                ComboBoxState cbState = ComboBoxState.Normal;

                if (MouseIsDown)
                {
                    cbState = ComboBoxState.Pressed;
                }
                else if (MouseIsOver)
                {
                    cbState = ComboBoxState.Hot;
                }

                Rectangle dropDownButtonRect = new Rectangle(0, 0, Width, Height);
                if (cbState == ComboBoxState.Normal)
                {
                    pevent.Graphics.FillRectangle(SystemBrushes.Window, dropDownButtonRect);
                }

                using (var hdc = new DeviceContextHdcScope(pevent))
                {
                    ComboBoxRenderer.DrawDropDownButtonForHandle(
                        hdc,
                        dropDownButtonRect,
                        cbState,
                        DpiHelper.IsScalingRequirementMet ? HandleInternal : IntPtr.Zero);
                }

                // Redraw focus cues
                // For consistency with other PropertyGrid buttons, i.e. those opening system dialogs ("..."), that always show visual cues when focused,
                // we need to do the same for this custom button, painted as ComboBox control part (drop-down).
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
            if (_useComboBoxTheme)
            {
                AccessibleName = SR.PropertyGridDropDownButtonComboBoxAccessibleName;
            }
            else
            {
                AccessibleName = SR.PropertyGridDropDownButtonAccessibleName;
            }
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object for this control.</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DropDownButtonAccessibleObject(this);
        }

        internal override ButtonBaseAdapter CreateStandardAdapter()
        {
            return new DropDownButtonAdapter(this);
        }
    }

    internal class DropDownButtonAdapter : ButtonStandardAdapter
    {
        internal DropDownButtonAdapter(ButtonBase control) : base(control) { }

        private void DDB_Draw3DBorder(PaintEventArgs e, Rectangle r, bool raised)
        {
            if (Control.BackColor != SystemColors.Control && SystemInformation.HighContrast)
            {
                if (raised)
                {
                    Color c = ControlPaint.LightLight(Control.BackColor);
                    ControlPaint.DrawBorder(
                        e, r,
                        c, 1, ButtonBorderStyle.Outset,
                        c, 1, ButtonBorderStyle.Outset,
                        c, 2, ButtonBorderStyle.Inset,
                        c, 2, ButtonBorderStyle.Inset);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, ControlPaint.Dark(Control.BackColor));
                }
            }
            else
            {
                if (raised)
                {
                    Color c = ControlPaint.Light(Control.BackColor);
                    ControlPaint.DrawBorder(
                        e, r,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.Solid,
                        Control.BackColor, 2, ButtonBorderStyle.Outset,
                        Control.BackColor, 2, ButtonBorderStyle.Outset);

                    Rectangle inside = r;
                    inside.Offset(1, 1);
                    inside.Width -= 3;
                    inside.Height -= 3;
                    c = ControlPaint.LightLight(Control.BackColor);
                    ControlPaint.DrawBorder(
                        e, inside,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.None,
                        c, 1, ButtonBorderStyle.None);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, ControlPaint.Dark(Control.BackColor));
                }
            }
        }

        internal override void PaintUp(PaintEventArgs pevent, CheckState state)
        {
            base.PaintUp(pevent, state);
            if (!Application.RenderWithVisualStyles)
            {
                DDB_Draw3DBorder(pevent, Control.ClientRectangle, raised: true);
            }
            else
            {
                Color c = (ARGB)SystemColors.Window;
                Rectangle rect = Control.ClientRectangle;
                rect.Inflate(0, -1);
                ControlPaint.DrawBorder(
                    pevent, rect,
                    c, 1, ButtonBorderStyle.None,
                    c, 1, ButtonBorderStyle.None,
                    c, 1, ButtonBorderStyle.Solid,
                    c, 1, ButtonBorderStyle.None);
            }
        }

        internal override void DrawImageCore(Graphics graphics, Image image, Rectangle imageBounds, Point imageStart, LayoutData layout)
        {
            ControlPaint.DrawImageReplaceColor(
                graphics,
                image,
                imageBounds,
                Color.Black,
                IsHighContrastHighlighted() && !Control.MouseIsDown ? SystemColors.HighlightText : Control.ForeColor);
        }
    }

    /// <summary>
    ///  Represents the accessibility object for the PropertyGrid DropDown button.
    ///  This DropDownButtonAccessibleObject is available in Level3 only.
    /// </summary>
    internal class DropDownButtonAccessibleObject : Control.ControlAccessibleObject
    {
        private readonly DropDownButton _owningDropDownButton;
        private readonly PropertyGridView _owningPropertyGrid;

        /// <summary>
        ///  Constructs the new instance of DropDownButtonAccessibleObject.
        /// </summary>
        /// <param name="owningDropDownButton"></param>
        public DropDownButtonAccessibleObject(DropDownButton owningDropDownButton) : base(owningDropDownButton)
        {
            _owningDropDownButton = owningDropDownButton;
            _owningPropertyGrid = owningDropDownButton.Parent as PropertyGridView;

            if (owningDropDownButton.IsHandleCreated)
            {
                UseStdAccessibleObjects(owningDropDownButton.Handle);
            }
        }

        public override void DoDefaultAction()
        {
            if (_owningDropDownButton.IsHandleCreated)
            {
                _owningDropDownButton.PerformButtonClick();
            }
        }

        /// <summary>
        ///  Request to return the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (direction == UiaCore.NavigateDirection.Parent &&
                _owningPropertyGrid.SelectedGridEntry != null &&
                _owningDropDownButton.Visible)
            {
                return _owningPropertyGrid.SelectedGridEntry?.AccessibilityObject;
            }
            else if (direction == UiaCore.NavigateDirection.PreviousSibling)
            {
                return _owningPropertyGrid.EditAccessibleObject;
            }

            return base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  Returns the element that is the root node of this fragment of UI.
        /// </summary>
        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
        {
            get
            {
                return _owningPropertyGrid.AccessibilityObject;
            }
        }

        /// <summary>
        ///  Request value of specified property from an element.
        /// </summary>
        /// <param name="propertyID">Identifier indicating the property to return</param>
        /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
        internal override object GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
        {
            UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
            UiaCore.UIA.NamePropertyId => Name,
            UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => true,
            UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
            _ => base.GetPropertyValue(propertyID),
        };

        /// <summary>
        ///  Gets the accessible role.
        /// </summary>
        public override AccessibleRole Role => AccessibleRole.PushButton;

        /// <summary>
        ///  Request that focus is set to this item.
        ///  The UIAutomation framework will ensure that the UI hosting this fragment is already
        ///  focused before calling this method, so this method should only update its internal
        ///  focus state; it should not attempt to give its own HWND the focus, for example.
        /// </summary>
        internal override void SetFocus()
        {
            if (!_owningDropDownButton.IsHandleCreated)
            {
                return;
            }

            RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);

            base.SetFocus();
        }
    }
}
