// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed partial class EditorPropertyLine
    {
        // Class that renders either the ellipsis or dropdown button
        internal sealed class EditorButton : Button
        {
            private bool _mouseOver;
            private bool _mouseDown;

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button == MouseButtons.Left)
                {
                    _mouseDown = true;
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                _mouseOver = true;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _mouseOver = false;
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (e.Button == MouseButtons.Left)
                {
                    _mouseDown = false;
                }
            }

            public bool Ellipsis { get; set; }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                if (Ellipsis)
                {
                    PushButtonState buttonState = PushButtonState.Normal;
                    if (_mouseDown)
                    {
                        buttonState = PushButtonState.Pressed;
                    }
                    else if (_mouseOver)
                    {
                        buttonState = PushButtonState.Hot;
                    }

                    ButtonRenderer.DrawButton(g, new Rectangle(-1, -1, Width + 2, Height + 2), "…", Font, Focused, buttonState);
                    return;
                }

                if (ComboBoxRenderer.IsSupported)
                {
                    ComboBoxState state = ComboBoxState.Normal;
                    if (Enabled)
                    {
                        if (_mouseDown)
                        {
                            state = ComboBoxState.Pressed;
                        }
                        else if (_mouseOver)
                        {
                            state = ComboBoxState.Hot;
                        }
                    }
                    else
                    {
                        state = ComboBoxState.Disabled;
                    }

                    ComboBoxRenderer.DrawDropDownButton(g, new Rectangle(0, 0, Width, Height), state);
                }
                else
                {
                    PushButtonState buttonState = PushButtonState.Normal;
                    if (Enabled)
                    {
                        if (_mouseDown)
                        {
                            buttonState = PushButtonState.Pressed;
                        }
                        else if (_mouseOver)
                        {
                            buttonState = PushButtonState.Hot;
                        }
                    }
                    else
                    {
                        buttonState = PushButtonState.Disabled;
                    }

                    ButtonRenderer.DrawButton(
                        g,
                        new Rectangle(-1, -1, Width + 2, Height + 2),
                        string.Empty,
                        Font,
                        Focused,
                        buttonState);

                    // Draw the arrow icon
                    try
                    {
                        using Icon icon = new(typeof(DesignerActionPanel), "Arrow.ico");
                        using Bitmap arrowBitmap = icon.ToBitmap();

                        // Make sure we draw properly under high contrast by re-mapping
                        // the arrow color to the WindowText color
                        using ImageAttributes attributes = new();
                        (Color OldColor, Color NewColor) map = new(Color.Black, SystemColors.WindowText);
                        attributes.SetRemapTable(ColorAdjustType.Bitmap, new ReadOnlySpan<(Color OldColor, Color NewColor)>(ref map));
                        int imageWidth = arrowBitmap.Width;
                        int imageHeight = arrowBitmap.Height;
                        g.DrawImage(
                            arrowBitmap,
                            new Rectangle(
                                (Width - imageWidth + 1) / 2,
                                (Height - imageHeight + 1) / 2,
                                imageWidth,
                                imageHeight),
                            0, 0, imageWidth, imageWidth,
                            GraphicsUnit.Pixel,
                            attributes,
                            callback: null,
                            0);
                    }
                    catch
                    {
                    }
                }

                if (Focused)
                {
                    ControlPaint.DrawFocusRectangle(g, new Rectangle(2, 2, Width - 5, Height - 5));
                }
            }

            public void ResetMouseStates()
            {
                _mouseDown = false;
                _mouseOver = false;
                Invalidate();
            }
        }
    }
}
