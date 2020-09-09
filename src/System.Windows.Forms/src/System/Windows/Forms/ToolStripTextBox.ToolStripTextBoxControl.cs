// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
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
                    RECT rect = new RECT();
                    CreateParams cp = CreateParams;

                    AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);

                    // the coordinates we get back are negative, we need to translate this back to positive.
                    int offsetX = -rect.left; // one to get back to 0,0, another to translate
                    int offsetY = -rect.top;

                    // fetch the client rect, then apply the offset.
                    User32.GetClientRect(new HandleRef(this, Handle), ref rect);

                    rect.left += offsetX;
                    rect.right += offsetX;
                    rect.top += offsetY;
                    rect.bottom += offsetY;

                    return rect;
                }
            }
            private Rectangle AbsoluteClientRectangle
            {
                get
                {
                    RECT rect = AbsoluteClientRECT;
                    return Rectangle.FromLTRB(rect.top, rect.top, rect.right, rect.bottom);
                }
            }

            private ProfessionalColorTable ColorTable
            {
                get
                {
                    if (Owner != null)
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
                             (Owner != null && (Owner.Renderer is ToolStripProfessionalRenderer)));
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

            internal override bool SupportsUiaProviders => true;

            private unsafe void InvalidateNonClient()
            {
                if (!IsPopupTextBox)
                {
                    return;
                }

                RECT absoluteClientRectangle = AbsoluteClientRECT;

                // Get the total client area, then exclude the client by using XOR
                using var hTotalRegion = new Gdi32.RegionScope(0, 0, Width, Height);
                using var hClientRegion = new Gdi32.RegionScope(
                    absoluteClientRectangle.left,
                    absoluteClientRectangle.top,
                    absoluteClientRectangle.right,
                    absoluteClientRectangle.bottom);
                using var hNonClientRegion = new Gdi32.RegionScope(0, 0, 0, 0);

                Gdi32.CombineRgn(hNonClientRegion, hTotalRegion, hClientRegion, Gdi32.RGN.XOR);

                // Call RedrawWindow with the region.
                User32.RedrawWindow(
                    new HandleRef(this, Handle),
                    null,
                    hNonClientRegion,
                    User32.RDW.INVALIDATE | User32.RDW.ERASE | User32.RDW.UPDATENOW
                        | User32.RDW.ERASENOW | User32.RDW.FRAME);
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
            {
                return new ToolStripTextBoxControlAccessibleObject(this, Owner);
            }

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

                using var hdc = new User32.GetDcScope(m.HWnd, IntPtr.Zero, User32.DCX.WINDOW | User32.DCX.USESTYLE);
                if (hdc.IsNull)
                {
                    throw new Win32Exception();
                }

                // Don't set the clipping region based on the WParam - windows seems to take out the two pixels intended for the non-client border.

                Color outerBorderColor = (MouseIsOver || Focused) ? ColorTable.TextBoxBorder : BackColor;
                Color innerBorderColor = BackColor;

                if (!Enabled)
                {
                    outerBorderColor = SystemColors.ControlDark;
                    innerBorderColor = SystemColors.Control;
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
                m.Result = IntPtr.Zero;
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.WM.NCPAINT)
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
}
