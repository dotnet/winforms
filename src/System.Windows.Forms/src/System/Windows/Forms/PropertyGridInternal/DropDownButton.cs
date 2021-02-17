// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal sealed partial class DropDownButton : Button
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
}
