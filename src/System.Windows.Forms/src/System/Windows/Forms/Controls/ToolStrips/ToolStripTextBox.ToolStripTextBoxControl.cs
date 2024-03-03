// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Microsoft.Win32;

namespace System.Windows.Forms;

public partial class ToolStripTextBox
{
    private class ToolStripTextBoxControl : TextBox
    {
        private bool _mouseIsOver;
        private bool _isFontSet = true;
        private bool _alreadyHooked;

        public ToolStripTextBoxControl()
        {
            // required to make the text box height match the combo.
            Font = ToolStripManager.DefaultFont;
            _isFontSet = false;
        }

        // returns the distance from the client rect to the upper left hand corner of the control
        private RECT AbsoluteClientRECT
        {
            get
            {
                RECT rect = default;
                CreateParams cp = CreateParams;

                AdjustWindowRectExForControlDpi(ref rect, (WINDOW_STYLE)cp.Style, false, (WINDOW_EX_STYLE)cp.ExStyle);

                // the coordinates we get back are negative, we need to translate this back to positive.
                int offsetX = -rect.left; // one to get back to 0,0, another to translate
                int offsetY = -rect.top;

                // fetch the client rect, then apply the offset.
                PInvokeCore.GetClientRect(this, out var clientRect);

                clientRect.left += offsetX;
                clientRect.right += offsetX;
                clientRect.top += offsetY;
                clientRect.bottom += offsetY;

                return clientRect;
            }
        }

        private Rectangle AbsoluteClientRectangle => AbsoluteClientRECT;

        private ProfessionalColorTable ColorTable
        {
            get
            {
                if (Owner is not null)
                {
                    if (Owner.Renderer is ToolStripProfessionalRenderer renderer)
                    {
                        return renderer.ColorTable;
                    }
                }

                return ProfessionalColors.ColorTable;
            }
        }

        private bool IsPopupTextBox
        {
            get
            {
                return ((BorderStyle == BorderStyle.Fixed3D) &&
                         (Owner is not null && (Owner.Renderer is ToolStripProfessionalRenderer)));
            }
        }

        internal bool MouseIsOver
        {
            get { return _mouseIsOver; }
            set
            {
                if (_mouseIsOver != value)
                {
                    _mouseIsOver = value;
                    if (!Focused)
                    {
                        InvalidateNonClient();
                    }
                }
            }
        }

        [AllowNull]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                _isFontSet = ShouldSerializeFont();
            }
        }

        public ToolStripTextBox? Owner { get; set; }

        private unsafe void InvalidateNonClient()
        {
            if (!IsPopupTextBox)
            {
                return;
            }

            var absoluteClientRectangle = AbsoluteClientRECT;

            // Get the total client area, then exclude the client by using XOR
            using RegionScope hTotalRegion = new(0, 0, Width, Height);
            using RegionScope hClientRegion = new(
                absoluteClientRectangle.left,
                absoluteClientRectangle.top,
                absoluteClientRectangle.right,
                absoluteClientRectangle.bottom);
            using RegionScope hNonClientRegion = new(0, 0, 0, 0);

            PInvokeCore.CombineRgn(hNonClientRegion, hTotalRegion, hClientRegion, RGN_COMBINE_MODE.RGN_XOR);

            // Call RedrawWindow with the region.
            PInvoke.RedrawWindow(
                this,
                lprcUpdate: null,
                hNonClientRegion,
                REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_ERASE | REDRAW_WINDOW_FLAGS.RDW_UPDATENOW
                    | REDRAW_WINDOW_FLAGS.RDW_ERASENOW | REDRAW_WINDOW_FLAGS.RDW_FRAME);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            InvalidateNonClient();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            InvalidateNonClient();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            MouseIsOver = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseIsOver = false;
        }

        private void HookStaticEvents(bool hook)
        {
            if (hook)
            {
                if (!_alreadyHooked)
                {
                    try
                    {
                        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                    }
                    finally
                    {
                        _alreadyHooked = true;
                    }
                }
            }
            else if (_alreadyHooked)
            {
                try
                {
                    SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                }
                finally
                {
                    _alreadyHooked = false;
                }
            }
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Window)
            {
                if (!_isFontSet)
                {
                    Font = ToolStripManager.DefaultFont;
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Disposing && !IsDisposed)
            {
                HookStaticEvents(Visible);
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new ToolStripTextBoxControlAccessibleObject(this);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                HookStaticEvents(false);
            }

            base.Dispose(disposing);
        }

        private void WmNCPaint(ref Message m)
        {
            if (!IsPopupTextBox)
            {
                base.WndProc(ref m);
                return;
            }

            // Paint over the edges of the text box.

            // Note that GetWindowDC just calls GetDCEx with DCX_WINDOW | DCX_USESTYLE.

            using GetDcScope hdc = new(m.HWND, HRGN.Null, GET_DCX_FLAGS.DCX_WINDOW | (GET_DCX_FLAGS)0x00010000 /* DCX_USESTYLE */);
            if (hdc.IsNull)
            {
                throw new Win32Exception();
            }

            // Don't set the clipping region based on the WParam - windows seems to take out the two pixels intended for the non-client border.

            bool focused = MouseIsOver || Focused;
            Color outerBorderColor = focused ? ColorTable.TextBoxBorder : BackColor;
            Color innerBorderColor = SystemInformation.HighContrast && !focused ? ColorTable.MenuBorder : BackColor;

            if (!Enabled)
            {
                outerBorderColor = Application.SystemColors.ControlDark;
                innerBorderColor = Application.SystemColors.Control;
            }

            using Graphics g = hdc.CreateGraphics();
            Rectangle clientRect = AbsoluteClientRectangle;

            // Could have set up a clip and fill-rectangled, thought this would be faster.
            using var brush = innerBorderColor.GetCachedSolidBrushScope();
            g.FillRectangle(brush, 0, 0, Width, clientRect.Top);                                // top border
            g.FillRectangle(brush, 0, 0, clientRect.Left, Height);                              // left border
            g.FillRectangle(brush, 0, clientRect.Bottom, Width, Height - clientRect.Height);    // bottom border
            g.FillRectangle(brush, clientRect.Right, 0, Width - clientRect.Right, Height);      // right border

            // Paint the outside rect.
            using var pen = outerBorderColor.GetCachedPenScope();
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            // We've handled WM_NCPAINT.
            m.ResultInternal = (LRESULT)0;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.MsgInternal == PInvoke.WM_NCPAINT)
            {
                WmNCPaint(ref m);
                return;
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
