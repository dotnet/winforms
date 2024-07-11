// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Windows.Forms;

// this renderer supports high contrast for ToolStripProfessional and ToolStripSystemRenderer.
internal class ToolStripHighContrastRenderer : ToolStripSystemRenderer
{
    private const int GRIP_PADDING = 4;

    private BitVector32 _options;
    private static readonly int s_optionsDottedBorder = BitVector32.CreateMask();
    private static readonly int s_optionsDottedGrip = BitVector32.CreateMask(s_optionsDottedBorder);
    private static readonly int s_optionsFillWhenSelected = BitVector32.CreateMask(s_optionsDottedGrip);

    public ToolStripHighContrastRenderer(bool systemRenderMode)
    {
        _options[s_optionsDottedBorder | s_optionsDottedGrip | s_optionsFillWhenSelected] = !systemRenderMode;
    }

    public bool DottedBorder
    {
        get { return _options[s_optionsDottedBorder]; }
    }

    public bool DottedGrip
    {
        get { return _options[s_optionsDottedGrip]; }
    }

    public bool FillWhenSelected
    {
        get { return _options[s_optionsFillWhenSelected]; }
    }

    // this is a renderer override, so return null so we don't get into an infinite loop.
    internal override ToolStripRenderer? RendererOverride
    {
        get { return null; }
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        base.OnRenderArrow(e);
    }

    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
        if (DottedGrip)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.GripBounds;
            ToolStrip toolStrip = e.ToolStrip;

            int height = (toolStrip.Orientation == Orientation.Horizontal) ? bounds.Height : bounds.Width;
            int width = (toolStrip.Orientation == Orientation.Horizontal) ? bounds.Width : bounds.Height;

            int numRectangles = (height - (GRIP_PADDING * 2)) / 4;

            if (numRectangles > 0)
            {
                Rectangle[] shadowRects = new Rectangle[numRectangles];
                int startY = GRIP_PADDING;
                int startX = (width / 2);

                for (int i = 0; i < numRectangles; i++)
                {
                    shadowRects[i] = (toolStrip.Orientation == Orientation.Horizontal) ?
                                        new Rectangle(startX, startY, 2, 2) :
                                        new Rectangle(startY, startX, 2, 2);

                    startY += 4;
                }

                g.FillRectangles(SystemBrushes.ControlLight, shadowRects);
            }
        }
        else
        {
            base.OnRenderGrip(e);
        }
    }

    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (FillWhenSelected)
        {
            RenderItemInternalFilled(e, false);
        }
        else
        {
            base.OnRenderDropDownButtonBackground(e);
            if (e.Item.Pressed)
            {
                e.Graphics.DrawRectangle(SystemPens.ButtonHighlight, new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1));
            }
        }
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Image is not { } image)
        {
            return;
        }

        if (Image.GetPixelFormatSize(image.PixelFormat) > 16)
        {
            // For 24, 32 bit images, just paint normally - mapping the color table is not
            // going to work when you can have full color.
            base.OnRenderItemCheck(e);
            return;
        }

        RenderItemImageOfLowColorDepth(e);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        // do nothing
    }

    protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
    {
        base.OnRenderItemBackground(e);
    }

    protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
    {
        Rectangle bounds = new(Point.Empty, e.Item.Size);
        Graphics g = e.Graphics;

        if (e.Item is ToolStripSplitButton item)
        {
            Rectangle dropDownRect = item.DropDownButtonBounds;

            if (item.Pressed)
            {
                g.DrawRectangle(SystemPens.ButtonHighlight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
            else if (item.Selected)
            {
                g.FillRectangle(SystemBrushes.Highlight, bounds);
                DrawHightContrastDashedBorder(g, e.Item);
                g.DrawRectangle(SystemPens.ButtonHighlight, dropDownRect);
            }

            Color arrowColor = item.Selected && !item.Pressed ? SystemColors.HighlightText : SystemColors.ControlText;
            DrawArrow(new ToolStripArrowRenderEventArgs(g, item, dropDownRect, arrowColor, ArrowDirection.Down));
        }
    }

    protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
    {
        base.OnRenderStatusStripSizingGrip(e);
    }

    protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
        if (FillWhenSelected)
        {
            RenderItemInternalFilled(e);
        }
        else
        {
            base.OnRenderLabelBackground(e);
        }
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        base.OnRenderMenuItemBackground(e);
        if (!e.Item.IsOnDropDown && e.Item.Pressed)
        {
            e.Graphics.DrawRectangle(SystemPens.ButtonHighlight, 0, 0, e.Item.Width - 1, e.Item.Height - 1);
        }

        if (e.Item is ToolStripMenuItem menuItem && !e.Item.IsOnDropDown && (menuItem.Checked || menuItem.Selected))
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new(Point.Empty, menuItem.Size);

            g.FillRectangle(SystemBrushes.Highlight, bounds);
            g.DrawRectangle(SystemPens.ControlLight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            if (menuItem.Selected)
            {
                DrawHightContrastDashedBorder(g, menuItem);
            }
        }
    }

    protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (FillWhenSelected)
        {
            RenderItemInternalFilled(e, /*pressFill = */false);
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
            Color arrowColor = !item.Enabled ? SystemColors.ControlDark
                : item.Selected && !item.Pressed ? SystemColors.HighlightText
                : SystemColors.ControlText;
            DrawArrow(new ToolStripArrowRenderEventArgs(g, item, new Rectangle(Point.Empty, item.Size), arrowColor, ArrowDirection.Down));
        }
        else
        {
            base.OnRenderOverflowButtonBackground(e);
        }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (e.Item.Selected && (!e.Item.Pressed || e.Item is ToolStripButton))
        {
            e.DefaultTextColor = SystemColors.HighlightText;
        }
        else if (e.TextColor != SystemColors.HighlightText && e.TextColor != SystemColors.ControlText)
        {
            // we'll change the DefaultTextColor, if someone wants to change this,manually set the TextColor property.
            if (e.Item.Selected || e.Item.Pressed)
            {
                e.DefaultTextColor = SystemColors.HighlightText;
            }
            else
            {
                e.DefaultTextColor = SystemColors.ControlText;
            }
        }

        // ToolstripButtons and ToolstripMenuItems that are checked are rendered with a highlight
        // background. In that case, set the text color to highlight as well.
        if ((typeof(ToolStripButton).IsAssignableFrom(e.Item.GetType())
            && ((ToolStripButton)e.Item).DisplayStyle != ToolStripItemDisplayStyle.Image
            && ((ToolStripButton)e.Item).Checked)
            || (typeof(ToolStripMenuItem).IsAssignableFrom(e.Item.GetType())
            && ((ToolStripMenuItem)e.Item).DisplayStyle != ToolStripItemDisplayStyle.Image
            && !e.Item.IsOnDropDown
            && ((ToolStripMenuItem)e.Item).Checked))
        {
            e.TextColor = SystemColors.HighlightText;
        }

        base.OnRenderItemText(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        // do nothing. LOGO requirements ask us not to paint background effects behind
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);
        Graphics g = e.Graphics;

        if (e.ToolStrip is ToolStripDropDown)
        {
            g.DrawRectangle(SystemPens.ButtonHighlight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

            if (e.ToolStrip is not ToolStripOverflow)
            {
                // make the neck connected.
                g.FillRectangle(SystemBrushes.Control, e.ConnectedArea);
            }
        }
        else if (e.ToolStrip is MenuStrip)
        {
            // do nothing
        }
        else if (e.ToolStrip is StatusStrip)
        {
            g.DrawRectangle(SystemPens.ButtonShadow, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }
        else
        {
            RenderToolStripBackgroundInternal(e);
        }
    }

    private void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
    {
        Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);
        Graphics g = e.Graphics;

        if (DottedBorder)
        {
            using Pen p = new(SystemColors.ButtonShadow)
            {
                DashStyle = DashStyle.Dot
            };

            bool oddWidth = ((bounds.Width & 0x1) == 0x1);
            bool oddHeight = ((bounds.Height & 0x1) == 0x1);
            int indent = 2;

            // top
            g.DrawLine(p, bounds.X + indent, bounds.Y, bounds.Width - 1, bounds.Y);
            // bottom
            g.DrawLine(p, bounds.X + indent, bounds.Height - 1, bounds.Width - 1, bounds.Height - 1);

            // left
            g.DrawLine(p, bounds.X, bounds.Y + indent, bounds.X, bounds.Height - 1);
            // right
            g.DrawLine(p, bounds.Width - 1, bounds.Y + indent, bounds.Width - 1, bounds.Height - 1);

            // connecting pixels

            // top left connecting pixel - always drawn
            g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(1, 1, 1, 1));

            if (oddWidth)
            {
                // top right pixel
                g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(bounds.Width - 2, 1, 1, 1));
            }

            // bottom connecting pixels - drawn only if height is odd
            if (oddHeight)
            {
                // bottom left
                g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(1, bounds.Height - 2, 1, 1));
            }

            // top and bottom right connecting pixel - drawn only if height and width are odd
            if (oddHeight && oddWidth)
            {
                // bottom right
                g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(bounds.Width - 2, bounds.Height - 2, 1, 1));
            }
        }
        else
        {
            // draw solid border
            bounds.Width -= 1;
            bounds.Height -= 1;
            g.DrawRectangle(SystemPens.ButtonShadow, bounds);
        }
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        Pen foreColorPen = SystemPens.ButtonShadow;

        Graphics g = e.Graphics;
        Rectangle bounds = new(Point.Empty, e.Item.Size);

        if (e.Vertical)
        {
            if (bounds.Height >= 8)
            {
                bounds.Inflate(0, -4);     // scoot down 4PX and start drawing
            }

            // Draw dark line
            int startX = bounds.Width / 2;

            g.DrawLine(foreColorPen, startX, bounds.Top, startX, bounds.Bottom - 1);
        }
        else
        {
            // horizontal separator
            if (bounds.Width >= 4)
            {
                bounds.Inflate(-2, 0);        // scoot over 2PX and start drawing
            }

            // Draw dark line
            int startY = bounds.Height / 2;

            g.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right - 1, startY);
        }
    }

    // Indicates whether system is currently set to high contrast 'white on black' mode
    internal static bool IsHighContrastWhiteOnBlack()
    {
        return SystemColors.Control.ToArgb() == Color.Black.ToArgb();
    }

    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Image is not { } image)
        {
            return;
        }

        if (Image.GetPixelFormatSize(image.PixelFormat) > 16)
        {
            // For 24, 32 bit images, just paint normally - mapping the color table is not
            // going to work when you can have full color.
            base.OnRenderItemImage(e);
            return;
        }

        RenderItemImageOfLowColorDepth(e);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (FillWhenSelected)
        {
            if (e.Item is ToolStripButton button && button.Checked)
            {
                Graphics g = e.Graphics;
                Rectangle bounds = new(Point.Empty, e.Item.Size);

                g.FillRectangle(SystemBrushes.Highlight, bounds);

                if (button.Selected)
                {
                    DrawHightContrastDashedBorder(g, button);
                }
                else
                {
                    g.DrawRectangle(SystemPens.ControlLight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
            else
            {
                RenderItemInternalFilled(e);
            }
        }
        else
        {
            base.OnRenderButtonBackground(e);
        }
    }

    private static void RenderItemInternalFilled(ToolStripItemRenderEventArgs e)
    {
        RenderItemInternalFilled(e, /*pressFill=*/true);
    }

    private static void RenderItemInternalFilled(ToolStripItemRenderEventArgs e, bool pressFill)
    {
        Graphics g = e.Graphics;
        Rectangle bounds = new(Point.Empty, e.Item.Size);

        if (e.Item.Pressed)
        {
            if (pressFill)
            {
                g.FillRectangle(SystemBrushes.Highlight, bounds);
            }
            else
            {
                g.DrawRectangle(SystemPens.ControlLight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }
        else if (e.Item.Selected)
        {
            g.FillRectangle(SystemBrushes.Highlight, bounds);
            DrawHightContrastDashedBorder(g, e.Item);
        }
    }

    private static void DrawHightContrastDashedBorder(Graphics graphics, ToolStripItem item)
    {
        Rectangle bounds = item.ClientBounds;
        float[] dashValues = [2, 2];
        int penWidth = 2;

        Pen focusPen1 = new(SystemColors.ControlText, penWidth)
        {
            DashPattern = dashValues
        };

        Pen focusPen2 = new(SystemColors.Control, penWidth)
        {
            DashPattern = dashValues,
            DashOffset = 2
        };

        graphics.DrawRectangle(focusPen1, bounds);
        graphics.DrawRectangle(focusPen2, bounds);
    }

    private void RenderItemImageOfLowColorDepth(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Image is not { } image)
        {
            return;
        }

        ToolStripItem item = e.Item;

        using ImageAttributes attrs = new();

        if (IsHighContrastWhiteOnBlack() && !(FillWhenSelected && (item.Pressed || item.Selected)))
        {
            // Translate white, black and blue to colors visible in high contrast mode.
            Span<(Color OldColor, Color NewColor)> map =
            [
                new(Color.Black, Color.White),
                new(Color.White, Color.Black),
                new(Color.FromArgb(0, 0, 128), Color.White)
            ];

            attrs.SetRemapTable(ColorAdjustType.Bitmap, map);
        }

        Graphics g = e.Graphics;
        Rectangle imageRect = e.ImageRectangle;

        if (item.ImageScaling == ToolStripItemImageScaling.None)
        {
            g.DrawImage(image, imageRect, 0, 0, imageRect.Width, imageRect.Height, GraphicsUnit.Pixel, attrs);
        }
        else
        {
            g.DrawImage(image, imageRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attrs);
        }
    }
}
