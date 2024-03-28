// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
///  Summary description for ProfessionalToolStripRenderer.
/// </summary>
public class ToolStripProfessionalRenderer : ToolStripRenderer
{
    private const int GripPadding = 4;
    private int _scaledGripPadding = GripPadding;

    private const int ICON_WELL_GRADIENT_WIDTH = 12;
    private int _iconWellGradientWidth = ICON_WELL_GRADIENT_WIDTH;

    private static readonly Size s_onePix = new(1, 1);

    private bool _isScalingInitialized;
    private const int OVERFLOW_BUTTON_WIDTH = 12;
    private const int OVERFLOW_ARROW_WIDTH = 9;
    private const int OVERFLOW_ARROW_HEIGHT = 5;
    private const int OVERFLOW_ARROW_OFFSETY = 8;
    private int _overflowButtonWidth = OVERFLOW_BUTTON_WIDTH;
    private int _overflowArrowWidth = OVERFLOW_ARROW_WIDTH;
    private int _overflowArrowHeight = OVERFLOW_ARROW_HEIGHT;
    private int _overflowArrowOffsetY = OVERFLOW_ARROW_OFFSETY;

    private const int DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE = 1;
    private Padding _scaledDropDownMenuItemPaintPadding = new(DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE + 1, 0, DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE, 0);
    private readonly ProfessionalColorTable? _professionalColorTable;
    private bool _roundedEdges = true;
    private ToolStripRenderer? _toolStripHighContrastRenderer;
    private ToolStripRenderer? _toolStripLowResolutionRenderer;

    public ToolStripProfessionalRenderer()
    {
    }

    internal ToolStripProfessionalRenderer(bool isDefault) : base(isDefault)
    {
    }

    public ToolStripProfessionalRenderer(ProfessionalColorTable professionalColorTable)
    {
        _professionalColorTable = professionalColorTable;
    }

    public ProfessionalColorTable ColorTable => _professionalColorTable ?? ProfessionalColors.ColorTable;

    internal override ToolStripRenderer? RendererOverride
    {
        get
        {
            if (DisplayInformation.HighContrast)
            {
                return HighContrastRenderer;
            }

            if (DisplayInformation.LowResolution)
            {
                return LowResolutionRenderer;
            }

            return null;
        }
    }

    internal ToolStripRenderer HighContrastRenderer
    {
        get
        {
            _toolStripHighContrastRenderer ??= new ToolStripHighContrastRenderer(/*renderLikeSystem*/false);

            return _toolStripHighContrastRenderer;
        }
    }

    internal ToolStripRenderer LowResolutionRenderer
    {
        get
        {
            _toolStripLowResolutionRenderer ??= new ToolStripProfessionalLowResolutionRenderer();

            return _toolStripLowResolutionRenderer;
        }
    }

    public bool RoundedEdges
    {
        get
        {
            return _roundedEdges;
        }
        set
        {
            _roundedEdges = value;
        }
    }

    private bool UseSystemColors
    {
        get { return (ColorTable.UseSystemColors || !ToolStripManager.VisualStylesEnabled); }
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderToolStripBackground(e);
            return;
        }

        ToolStrip toolStrip = e.ToolStrip;

        if (!ShouldPaintBackground(toolStrip))
        {
            return;
        }

        if (toolStrip is ToolStripDropDown)
        {
            RenderToolStripDropDownBackground(e);
        }
        else if (toolStrip is MenuStrip)
        {
            RenderMenuStripBackground(e);
        }
        else if (toolStrip is StatusStrip)
        {
            RenderStatusStripBackground(e);
        }
        else
        {
            RenderToolStripBackgroundInternal(e);
        }
    }

    protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.ToolStrip is not null)
        {
            ScaleObjectSizesIfNeeded(e.ToolStrip.DeviceDpi);
        }

        if (RendererOverride is not null)
        {
            base.OnRenderOverflowButtonBackground(e);
            return;
        }

        ToolStripItem item = e.Item;
        Graphics g = e.Graphics;

        // fill in the background colors
        bool rightToLeft = item.RightToLeft == RightToLeft.Yes;
        RenderOverflowBackground(e, rightToLeft);

        bool horizontal = e.ToolStrip is not null && e.ToolStrip.Orientation == Orientation.Horizontal;

        Rectangle overflowArrowRect;
        if (rightToLeft)
        {
            overflowArrowRect = new Rectangle(0, item.Height - _overflowArrowOffsetY, _overflowArrowWidth, _overflowArrowHeight);
        }
        else
        {
            overflowArrowRect = new Rectangle(item.Width - _overflowButtonWidth, item.Height - _overflowArrowOffsetY, _overflowArrowWidth, _overflowArrowHeight);
        }

        ArrowDirection direction = horizontal ? ArrowDirection.Down : ArrowDirection.Right;

        // in RTL the white highlight goes BEFORE the black triangle.
        int rightToLeftShift = (rightToLeft && horizontal) ? -1 : 1;

        // draw highlight
        overflowArrowRect.Offset(1 * rightToLeftShift, 1);
        RenderArrowInternal(g, overflowArrowRect, direction, SystemBrushes.ButtonHighlight);

        // draw black triangle
        overflowArrowRect.Offset(-1 * rightToLeftShift, -1);
        Point middle = RenderArrowInternal(g, overflowArrowRect, direction, SystemBrushes.ControlText);

        // draw lines
        if (horizontal)
        {
            rightToLeftShift = rightToLeft ? -2 : 0;
            // width of the both lines is 1 pixel and lines are drawn next to each other, this the highlight line is 1 pixel below the black line
            g.DrawLine(SystemPens.ControlText,
                middle.X - Offset2X,
                overflowArrowRect.Y - Offset2Y,
                middle.X + Offset2X,
                overflowArrowRect.Y - Offset2Y);
            g.DrawLine(SystemPens.ButtonHighlight,
                middle.X - Offset2X + 1 + rightToLeftShift,
                overflowArrowRect.Y - Offset2Y + 1,
                middle.X + Offset2X + 1 + rightToLeftShift,
                overflowArrowRect.Y - Offset2Y + 1);
        }
        else
        {
            g.DrawLine(SystemPens.ControlText,
                overflowArrowRect.X,
                middle.Y - Offset2Y,
                overflowArrowRect.X,
                middle.Y + Offset2Y);
            g.DrawLine(SystemPens.ButtonHighlight,
                overflowArrowRect.X + 1,
                middle.Y - Offset2Y + 1,
                overflowArrowRect.X + 1,
                middle.Y + Offset2Y + 1);
        }
    }

    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderDropDownButtonBackground(e);
            return;
        }

        if (e.Item is ToolStripDropDownItem item && item.Pressed && item.HasDropDownItems)
        {
            Rectangle bounds = new(Point.Empty, item.Size);

            RenderPressedGradient(e.Graphics, bounds);
        }
        else
        {
            RenderItemInternal(e, /*useHotBorder =*/true);
        }
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderSeparator(e);
            return;
        }

        RenderSeparatorInternal(e.Graphics, e.Item, new Rectangle(Point.Empty, e.Item.Size), e.Vertical);
    }

    protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderSplitButtonBackground(e);
            return;
        }

        Graphics g = e.Graphics;

        if (e.Item is not ToolStripSplitButton item)
        {
            return;
        }

        Rectangle bounds = new(Point.Empty, item.Size);
        if (item.BackgroundImage is not null)
        {
            Rectangle fillRect = item.Selected ? item.ContentRectangle : bounds;
            ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
        }

        bool buttonPressedOrSelected = (item.Pressed || item.ButtonPressed || item.Selected || item.ButtonSelected);

        if (buttonPressedOrSelected)
        {
            RenderItemInternal(e, useHotBorder: true);
        }

        if (item.ButtonPressed)
        {
            Rectangle buttonBounds = item.ButtonBounds;
            // We subtract 1 from each side except the right.
            // This is because we've already drawn the border, and we don't
            // want to draw over it.  We don't do the right edge, because we
            // drew the border around the whole control, not the button.
            Padding deflatePadding = item.RightToLeft == RightToLeft.Yes ? new Padding(0, 1, 1, 1) : new Padding(1, 1, 0, 1);
            buttonBounds = LayoutUtils.DeflateRect(buttonBounds, deflatePadding);
            RenderPressedButtonFill(g, buttonBounds);
        }
        else if (item.Pressed)
        {
            RenderPressedGradient(e.Graphics, bounds);
        }

        Rectangle dropDownRect = item.DropDownButtonBounds;

        if (buttonPressedOrSelected && !item.Pressed)
        {
            using var brush = ColorTable.ButtonSelectedBorder.GetCachedSolidBrushScope();
            g.FillRectangle(brush, item.SplitterBounds);
        }

        DrawArrow(new ToolStripArrowRenderEventArgs(g, item, dropDownRect, SystemColors.ControlText, ArrowDirection.Down));
    }

    protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderToolStripStatusLabelBackground(e);
            return;
        }

        RenderLabelInternal(e);
        ToolStripStatusLabel? item = e.Item as ToolStripStatusLabel;
        if (item is not null)
        {
            ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0, 0, item.Width, item.Height), item.BorderStyle, (Border3DSide)item.BorderSides);
        }
    }

    protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderLabelBackground(e);
            return;
        }

        RenderLabelInternal(e);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderButtonBackground(e);
            return;
        }

        ToolStripButton? item = e.Item as ToolStripButton;
        Graphics g = e.Graphics;
        Rectangle bounds = new(Point.Empty, item?.Size ?? Size.Empty);

        if (item is not null && item.CheckState == CheckState.Unchecked)
        {
            RenderItemInternal(e, useHotBorder: true);
        }
        else
        {
            Rectangle fillRect = item is not null && item.Selected ? item.ContentRectangle : bounds;

            if (item?.BackgroundImage is not null)
            {
                ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
            }

            if (UseSystemColors)
            {
                if (item is not null && item.Selected)
                {
                    RenderPressedButtonFill(g, bounds);
                }
                else
                {
                    RenderCheckedButtonFill(g, bounds);
                }

                using var pen = ColorTable.ButtonSelectedBorder.GetCachedPenScope();
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                if (item is not null && item.Selected)
                {
                    RenderPressedButtonFill(g, bounds);
                }
                else
                {
                    RenderCheckedButtonFill(g, bounds);
                }

                using var pen = ColorTable.ButtonSelectedBorder.GetCachedPenScope();
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderToolStripBorder(e);
            return;
        }

        ToolStrip toolStrip = e.ToolStrip;
        Graphics g = e.Graphics;

        if (toolStrip is ToolStripDropDown)
        {
            RenderToolStripDropDownBorder(e);
        }
        else if (toolStrip is MenuStrip)
        {
        }
        else if (toolStrip is StatusStrip)
        {
            RenderStatusStripBorder(e);
        }
        else
        {
            Rectangle bounds = new(Point.Empty, toolStrip.Size);

            // draw the shadow lines on the bottom and right
            using (var pen = ColorTable.ToolStripBorder.GetCachedPenScope())
            {
                if (toolStrip.Orientation == Orientation.Horizontal)
                {
                    // horizontal line at bottom
                    g.DrawLine(pen, bounds.Left, bounds.Height - 1, bounds.Right, bounds.Height - 1);
                    if (RoundedEdges)
                    {
                        // one pix corner rounding (right bottom)
                        g.DrawLine(pen, bounds.Width - 2, bounds.Height - 2, bounds.Width - 1, bounds.Height - 3);
                    }
                }
                else
                {
                    // draw vertical line on the right
                    g.DrawLine(pen, bounds.Width - 1, 0, bounds.Width - 1, bounds.Height - 1);
                    if (RoundedEdges)
                    {
                        // one pix corner rounding (right bottom)
                        g.DrawLine(pen, bounds.Width - 2, bounds.Height - 2, bounds.Width - 1, bounds.Height - 3);
                    }
                }
            }

            if (RoundedEdges)
            {
                // OverflowButton rendering
                if (toolStrip.OverflowButton.Visible)
                {
                    RenderOverflowButtonEffectsOverBorder(e);
                }
                else
                {
                    // Draw 1PX edging to round off the toolStrip
                    Rectangle edging;
                    if (toolStrip.Orientation == Orientation.Horizontal)
                    {
                        edging = new Rectangle(bounds.Width - 1, 3, 1, bounds.Height - 3);
                    }
                    else
                    {
                        edging = new Rectangle(3, bounds.Height - 1, bounds.Width - 3, bounds.Height - 1);
                    }

                    ScaleObjectSizesIfNeeded(toolStrip.DeviceDpi);
                    FillWithDoubleGradient(ColorTable.OverflowButtonGradientBegin, ColorTable.OverflowButtonGradientMiddle, ColorTable.OverflowButtonGradientEnd, e.Graphics, edging, _iconWellGradientWidth, _iconWellGradientWidth, LinearGradientMode.Vertical, /*flipHorizontal=*/false);
                    RenderToolStripCurve(e);
                }
            }
        }
    }

    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderGrip(e);
            return;
        }

        ScaleObjectSizesIfNeeded(e.ToolStrip.DeviceDpi);

        Graphics g = e.Graphics;
        Rectangle bounds = e.GripBounds;
        ToolStrip toolStrip = e.ToolStrip;

        bool rightToLeft = (e.ToolStrip.RightToLeft == RightToLeft.Yes);

        int height = (toolStrip.Orientation == Orientation.Horizontal) ? bounds.Height : bounds.Width;
        int width = (toolStrip.Orientation == Orientation.Horizontal) ? bounds.Width : bounds.Height;

        int numRectangles = (height - (_scaledGripPadding * 2)) / 4;

        if (numRectangles > 0)
        {
            // a MenuStrip starts its grip lower and has fewer grip rectangles.
            int yOffset = (toolStrip is MenuStrip) ? 2 : 0;

            Rectangle[] shadowRects = new Rectangle[numRectangles];
            int startY = _scaledGripPadding + 1 + yOffset;
            int startX = width / 2;

            for (int i = 0; i < numRectangles; i++)
            {
                shadowRects[i] = (toolStrip.Orientation == Orientation.Horizontal) ?
                                    new Rectangle(startX, startY, 2, 2) :
                                    new Rectangle(startY, startX, 2, 2);

                startY += 4;
            }

            // in RTL the GripLight rects should paint to the left of the GripDark rects.
            int xOffset = rightToLeft ? 1 : -1;

            if (rightToLeft)
            {
                // scoot over the rects in RTL so they fit within the bounds.
                for (int i = 0; i < numRectangles; i++)
                {
                    shadowRects[i].Offset(-xOffset, 0);
                }
            }

            using var gripLightBrush = ColorTable.GripLight.GetCachedSolidBrushScope();
            g.FillRectangles(gripLightBrush, shadowRects);

            for (int i = 0; i < numRectangles; i++)
            {
                shadowRects[i].Offset(xOffset, -1);
            }

            using var gripDarkBrush = ColorTable.GripDark.GetCachedSolidBrushScope();
            g.FillRectangles(gripDarkBrush, shadowRects);
        }
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderMenuItemBackground(e);
            return;
        }

        ToolStripItem item = e.Item;
        Graphics g = e.Graphics;
        Rectangle bounds = new(Point.Empty, item.Size);

        if ((bounds.Width == 0) || (bounds.Height == 0))
        {
            return;  // can't new up a linear gradient brush with no dimension.
        }

        if (item is MdiControlStrip.SystemMenuItem)
        {
            return; // no highlights are painted behind a system menu item
        }

        if (item.IsOnDropDown)
        {
            ScaleObjectSizesIfNeeded(item.DeviceDpi);

            bounds = LayoutUtils.DeflateRect(bounds, _scaledDropDownMenuItemPaintPadding);

            if (item.Selected)
            {
                Color borderColor = ColorTable.MenuItemBorder;
                if (item.Enabled)
                {
                    if (UseSystemColors)
                    {
                        borderColor = SystemColors.Highlight;
                        RenderSelectedButtonFill(g, bounds);
                    }
                    else
                    {
                        using Brush b = new LinearGradientBrush(
                            bounds,
                            ColorTable.MenuItemSelectedGradientBegin,
                            ColorTable.MenuItemSelectedGradientEnd,
                            LinearGradientMode.Vertical);

                        g.FillRectangle(b, bounds);
                    }
                }

                // Draw selection border - always drawn regardless of Enabled.
                using var pen = borderColor.GetCachedPenScope();
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                Rectangle fillRect = bounds;

                if (item.BackgroundImage is not null)
                {
                    ControlPaint.DrawBackgroundImage(
                        g,
                        item.BackgroundImage,
                        item.BackColor,
                        item.BackgroundImageLayout,
                        bounds,
                        fillRect);
                }
                else if (item.Owner is not null && item.BackColor != item.Owner.BackColor)
                {
                    using var brush = item.BackColor.GetCachedSolidBrushScope();
                    g.FillRectangle(brush, fillRect);
                }
            }
        }
        else
        {
            if (item.Pressed)
            {
                // Toplevel toolstrip rendering
                RenderPressedGradient(g, bounds);
            }
            else if (item.Selected)
            {
                // Hot, Pressed behavior
                // Fill with orange
                Color borderColor = ColorTable.MenuItemBorder;

                if (item.Enabled)
                {
                    if (UseSystemColors)
                    {
                        borderColor = SystemColors.Highlight;
                        RenderSelectedButtonFill(g, bounds);
                    }
                    else
                    {
                        using Brush b = new LinearGradientBrush(
                            bounds,
                            ColorTable.MenuItemSelectedGradientBegin,
                            ColorTable.MenuItemSelectedGradientEnd,
                            LinearGradientMode.Vertical);

                        g.FillRectangle(b, bounds);
                    }
                }

                // Draw selection border - always drawn regardless of Enabled.
                using var pen = borderColor.GetCachedPenScope();
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                Rectangle fillRect = bounds;

                if (item.BackgroundImage is not null)
                {
                    ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                }
                else if (item.Owner is not null && item.BackColor != item.Owner.BackColor)
                {
                    using var brush = item.BackColor.GetCachedSolidBrushScope();
                    g.FillRectangle(brush, fillRect);
                }
                else if (item is ToolStripMenuItem menuItem && menuItem.CheckState == CheckState.Checked)
                {
                    using var pen = ColorTable.MenuItemBorder.GetCachedPenScope();
                    g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
        }
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderArrow(e);
            return;
        }

        ToolStripItem? item = e.Item;

        if (item is ToolStripDropDownItem)
        {
            e.DefaultArrowColor = item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
        }

        base.OnRenderArrow(e);
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderImageMargin(e);
            return;
        }

        ScaleObjectSizesIfNeeded(e.ToolStrip.DeviceDpi);

        Rectangle bounds = e.AffectedBounds;
        bounds.Y += 2;
        bounds.Height -= 4; /*shrink to accomodate 1PX line*/
        RightToLeft rightToLeft = e.ToolStrip.RightToLeft;
        Color begin = (rightToLeft == RightToLeft.No) ? ColorTable.ImageMarginGradientBegin : ColorTable.ImageMarginGradientEnd;
        Color end = (rightToLeft == RightToLeft.No) ? ColorTable.ImageMarginGradientEnd : ColorTable.ImageMarginGradientBegin;

        FillWithDoubleGradient(begin, ColorTable.ImageMarginGradientMiddle, end, e.Graphics, bounds, _iconWellGradientWidth, _iconWellGradientWidth, LinearGradientMode.Horizontal, /*flipHorizontal=*/(e.ToolStrip.RightToLeft == RightToLeft.Yes));
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderItemText(e);
            return;
        }

        if (e.Item is ToolStripMenuItem && (e.Item.Selected || e.Item.Pressed))
        {
            e.DefaultTextColor = e.Item.ForeColor;
        }

        base.OnRenderItemText(e);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderItemCheck(e);
            return;
        }

        RenderCheckBackground(e);
        base.OnRenderItemCheck(e);
    }

    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderItemImage(e);
            return;
        }

        Rectangle imageRect = e.ImageRectangle;
        Image? image = e.Image;

        if (e.Item is ToolStripMenuItem)
        {
            ToolStripMenuItem? item = e.Item as ToolStripMenuItem;
            if (item is not null && item.CheckState != CheckState.Unchecked)
            {
                if (item.ParentInternal is ToolStripDropDownMenu dropDownMenu && !dropDownMenu.ShowCheckMargin && dropDownMenu.ShowImageMargin)
                {
                    RenderCheckBackground(e);
                }
            }
        }

        if (imageRect != Rectangle.Empty && image is not null)
        {
            if (!e.Item.Enabled)
            {
                base.OnRenderItemImage(e);
                return;
            }

            // Since office images don't scoot one px we have to override all painting but enabled = false;
            if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
            {
                e.Graphics.DrawImage(image, imageRect, new Rectangle(Point.Empty, imageRect.Size), GraphicsUnit.Pixel);
            }
            else
            {
                e.Graphics.DrawImage(image, imageRect);
            }
        }
    }

    protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderToolStripPanelBackground(e);
            return;
        }

        ToolStripPanel toolStripPanel = e.ToolStripPanel;

        if (!ShouldPaintBackground(toolStripPanel))
        {
            return;
        }

        // don't paint background effects
        e.Handled = true;

        RenderBackgroundGradient(e.Graphics, toolStripPanel, ColorTable.ToolStripPanelGradientBegin, ColorTable.ToolStripPanelGradientEnd);
    }

    protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
    {
        if (RendererOverride is not null)
        {
            base.OnRenderToolStripContentPanelBackground(e);
            return;
        }

        ToolStripContentPanel toolStripContentPanel = e.ToolStripContentPanel;

        if (!ShouldPaintBackground(toolStripContentPanel))
        {
            return;
        }

        if (SystemInformation.InLockedTerminalSession())
        {
            return;
        }

        // don't paint background effects
        e.Handled = true;

        e.Graphics.Clear(ColorTable.ToolStripContentPanelGradientEnd);

        // RenderBackgroundGradient(e.Graphics, toolStripContentPanel, ColorTable.ToolStripContentPanelGradientBegin, ColorTable.ToolStripContentPanelGradientEnd);
    }

    #region PrivatePaintHelpers

    // consider make public
    internal override Region? GetTransparentRegion(ToolStrip toolStrip)
    {
        if (toolStrip is ToolStripDropDown or MenuStrip or StatusStrip)
        {
            return null;
        }

        if (!RoundedEdges)
        {
            return null;
        }

        Rectangle bounds = new(Point.Empty, toolStrip.Size);

        // Render curve
        // eat away at the corners by drawing the parent background
        if (toolStrip.ParentInternal is not null)
        {
            // Paint pieces of the parent here to give toolStrip rounded effect
            Point topLeft = Point.Empty;
            Point topRight = new(bounds.Width - 1, 0);
            Point bottomLeft = new(0, bounds.Height - 1);
            Point bottomRight = new(bounds.Width - 1, bounds.Height - 1);

            // Pixels to eat away with the parent background
            // Grip side
            Rectangle topLeftParentHorizontalPixels = new(topLeft, s_onePix);
            Rectangle bottomLeftParentHorizontalPixels = new(bottomLeft, new Size(2, 1));
            Rectangle bottomLeftParentVerticalPixels = new(bottomLeft.X, bottomLeft.Y - 1, 1, 2);

            // OverflowSide
            Rectangle bottomRightHorizontalPixels = new(bottomRight.X - 1, bottomRight.Y, 2, 1);
            Rectangle bottomRightVerticalPixels = new(bottomRight.X, bottomRight.Y - 1, 1, 2);

            // TopSide
            Rectangle topRightHorizontalPixels, topRightVerticalPixels;

            if (toolStrip.OverflowButton.Visible)
            {
                topRightHorizontalPixels = new Rectangle(topRight.X - 1, topRight.Y, 1, 1);
                topRightVerticalPixels = new Rectangle(topRight.X, topRight.Y, 1, 2);
            }
            else
            {
                topRightHorizontalPixels = new Rectangle(topRight.X - 2, topRight.Y, 2, 1);
                topRightVerticalPixels = new Rectangle(topRight.X, topRight.Y, 1, 3);
            }

            Region parentRegionToPaint = new(topLeftParentHorizontalPixels);
            parentRegionToPaint.Union(topLeftParentHorizontalPixels);
            parentRegionToPaint.Union(bottomLeftParentHorizontalPixels);
            parentRegionToPaint.Union(bottomLeftParentVerticalPixels);
            parentRegionToPaint.Union(bottomRightHorizontalPixels);
            parentRegionToPaint.Union(bottomRightVerticalPixels);
            parentRegionToPaint.Union(topRightHorizontalPixels);
            parentRegionToPaint.Union(topRightVerticalPixels);

            return parentRegionToPaint;
        }

        return null;
    }

    /// <summary>
    /// We want to make sure the overflow button looks like it's the last thing on the toolbar.
    /// This touches up the few pixels that get clobbered by painting the border.
    /// </summary>
    private void RenderOverflowButtonEffectsOverBorder(ToolStripRenderEventArgs e)
    {
        ToolStrip toolStrip = e.ToolStrip;
        ToolStripItem item = toolStrip.OverflowButton;
        if (!item.Visible)
        {
            return;
        }

        Graphics g = e.Graphics;

        Color overflowBottomLeftShadow, overflowTopShadow;

        if (item.Pressed)
        {
            overflowBottomLeftShadow = ColorTable.ButtonPressedGradientBegin;
            overflowTopShadow = overflowBottomLeftShadow;
        }
        else if (item.Selected)
        {
            overflowBottomLeftShadow = ColorTable.ButtonSelectedGradientMiddle;
            overflowTopShadow = overflowBottomLeftShadow;
        }
        else
        {
            overflowBottomLeftShadow = ColorTable.ToolStripBorder;
            overflowTopShadow = ColorTable.ToolStripGradientMiddle;
        }

        // Extend the gradient color over the border.
        using (var brush = overflowBottomLeftShadow.GetCachedSolidBrushScope())
        {
            g.FillRectangle(brush, toolStrip.Width - 1, toolStrip.Height - 2, 1, 1);
            g.FillRectangle(brush, toolStrip.Width - 2, toolStrip.Height - 1, 1, 1);
        }

        using (var brush = overflowTopShadow.GetCachedSolidBrushScope())
        {
            g.FillRectangle(brush, toolStrip.Width - 2, 0, 1, 1);
            g.FillRectangle(brush, toolStrip.Width - 1, 1, 1, 1);
        }
    }

    /// <summary>
    ///  This function paints with three colors, beginning, middle, and end.
    ///  it paints:
    ///  (1)the entire bounds in the middle color
    ///  (2)gradient from beginning to middle of width firstGradientWidth
    ///  (3)gradient from middle to end of width secondGradientWidth
    ///
    ///  if there isn't enough room to do (2) and (3) it merges into a single gradient from beginning to end.
    /// </summary>
    private static void FillWithDoubleGradient(Color beginColor, Color middleColor, Color endColor, Graphics g, Rectangle bounds, int firstGradientWidth, int secondGradientWidth, LinearGradientMode mode, bool flipHorizontal)
    {
        if ((bounds.Width == 0) || (bounds.Height == 0))
        {
            return;  // can't new up a linear gradient brush with no dimension.
        }

        Rectangle endGradient = bounds;
        Rectangle beginGradient = bounds;
        bool useDoubleGradient;

        if (mode == LinearGradientMode.Horizontal)
        {
            if (flipHorizontal)
            {
                (beginColor, endColor) = (endColor, beginColor);
            }

            beginGradient.Width = firstGradientWidth;
            endGradient.Width = secondGradientWidth + 1;
            endGradient.X = bounds.Right - endGradient.Width;
            useDoubleGradient = (bounds.Width > (firstGradientWidth + secondGradientWidth));
        }
        else
        {
            beginGradient.Height = firstGradientWidth;
            endGradient.Height = secondGradientWidth + 1;
            endGradient.Y = bounds.Bottom - endGradient.Height;
            useDoubleGradient = (bounds.Height > (firstGradientWidth + secondGradientWidth));
        }

        if (useDoubleGradient)
        {
            // Fill with middleColor
            using (var brush = middleColor.GetCachedSolidBrushScope())
            {
                g.FillRectangle(brush, bounds);
            }

            // draw first gradient
            using (Brush b = new LinearGradientBrush(beginGradient, beginColor, middleColor, mode))
            {
                g.FillRectangle(b, beginGradient);
            }

            // draw second gradient
            using (LinearGradientBrush b = new(endGradient, middleColor, endColor, mode))
            {
                if (mode == LinearGradientMode.Horizontal)
                {
                    endGradient.X += 1;
                    endGradient.Width -= 1;
                }
                else
                {
                    endGradient.Y += 1;
                    endGradient.Height -= 1;
                }

                g.FillRectangle(b, endGradient);
            }
        }
        else
        {
            // not big enough for a swath in the middle.  lets just do a single gradient.
            using Brush b = new LinearGradientBrush(bounds, beginColor, endColor, mode);
            g.FillRectangle(b, bounds);
        }
    }

    private void RenderStatusStripBorder(ToolStripRenderEventArgs e)
    {
        using Pen p = new(ColorTable.StatusStripBorder);
        e.Graphics.DrawLine(p, 0, 0, e.ToolStrip.Width, 0);
    }

    private void RenderStatusStripBackground(ToolStripRenderEventArgs e)
    {
        StatusStrip? statusStrip = e.ToolStrip as StatusStrip;
        if (statusStrip is not null)
        {
            RenderBackgroundGradient(
                e.Graphics,
                statusStrip,
                ColorTable.StatusStripGradientBegin,
                ColorTable.StatusStripGradientEnd,
                statusStrip.Orientation);
        }
    }

    private void RenderCheckBackground(ToolStripItemImageRenderEventArgs e)
    {
        Rectangle bounds = ScaleHelper.IsScalingRequired
            ? new Rectangle(
                e.ImageRectangle.Left - 2,
                (e.Item.Height - e.ImageRectangle.Height) / 2 - 1,
                e.ImageRectangle.Width + 4, e.ImageRectangle.Height + 2)
            : new Rectangle(e.ImageRectangle.Left - 2, 1, e.ImageRectangle.Width + 4, e.Item.Height - 2);

        Graphics g = e.Graphics;

        if (!UseSystemColors)
        {
            Color fill = (e.Item.Selected) ? ColorTable.CheckSelectedBackground : ColorTable.CheckBackground;
            fill = (e.Item.Pressed) ? ColorTable.CheckPressedBackground : fill;
            using var brush = fill.GetCachedSolidBrushScope();
            g.FillRectangle(brush, bounds);

            using var pen = ColorTable.ButtonSelectedBorder.GetCachedPenScope();
            g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }
        else
        {
            if (e.Item.Pressed)
            {
                RenderPressedButtonFill(g, bounds);
            }
            else
            {
                RenderSelectedButtonFill(g, bounds);
            }

            g.DrawRectangle(SystemPens.Highlight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }
    }

    private void RenderPressedGradient(Graphics g, Rectangle bounds)
    {
        if ((bounds.Width == 0) || (bounds.Height == 0))
        {
            return;  // can't new up a linear gradient brush with no dimension.
        }

        // Paints a horizontal gradient similar to the image margin.
        using Brush b = new LinearGradientBrush(
            bounds,
            ColorTable.MenuItemPressedGradientBegin,
            ColorTable.MenuItemPressedGradientEnd,
            LinearGradientMode.Vertical);
        g.FillRectangle(b, bounds);

        // draw a box around the gradient
        using var pen = ColorTable.MenuBorder.GetCachedPenScope();
        g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
    }

    private void RenderMenuStripBackground(ToolStripRenderEventArgs e)
    {
        RenderBackgroundGradient(e.Graphics, e.ToolStrip, ColorTable.MenuStripGradientBegin, ColorTable.MenuStripGradientEnd, e.ToolStrip.Orientation);
    }

    private static void RenderLabelInternal(ToolStripItemRenderEventArgs e)
    {
        Graphics g = e.Graphics;
        ToolStripItem item = e.Item;
        Rectangle bounds = new(Point.Empty, item.Size);

        Rectangle fillRect = item.Selected ? item.ContentRectangle : bounds;

        if (item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
        }
    }

    private static void RenderBackgroundGradient(Graphics g, Control control, Color beginColor, Color endColor)
    {
        RenderBackgroundGradient(g, control, beginColor, endColor, Orientation.Horizontal);
    }

    // renders the overall gradient
    private static void RenderBackgroundGradient(Graphics g, Control control, Color beginColor, Color endColor, Orientation orientation)
    {
        if (control.RightToLeft == RightToLeft.Yes)
        {
            (endColor, beginColor) = (beginColor, endColor);
        }

        if (orientation != Orientation.Horizontal)
        {
            using var brush = beginColor.GetCachedSolidBrushScope();
            g.FillRectangle(brush, new Rectangle(Point.Empty, control.Size));
            return;
        }

        Control? parent = control.ParentInternal;
        if (parent is not null)
        {
            Rectangle gradientBounds = new(Point.Empty, parent.Size);
            if (!LayoutUtils.IsZeroWidthOrHeight(gradientBounds))
            {
                using LinearGradientBrush b = new(
                    gradientBounds,
                    beginColor,
                    endColor,
                    LinearGradientMode.Horizontal);
                b.TranslateTransform(parent.Width - control.Location.X, parent.Height - control.Location.Y);
                g.FillRectangle(b, new Rectangle(Point.Empty, control.Size));
            }
        }
        else
        {
            Rectangle gradientBounds = new(Point.Empty, control.Size);
            if (!LayoutUtils.IsZeroWidthOrHeight(gradientBounds))
            {
                // Don't have a parent that we know about - paint the gradient as if there isn't another container.
                using LinearGradientBrush b = new(
                    gradientBounds,
                    beginColor,
                    endColor,
                    LinearGradientMode.Horizontal);
                g.FillRectangle(b, gradientBounds);
            }
        }
    }

    private void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
    {
        ScaleObjectSizesIfNeeded(e.ToolStrip.DeviceDpi);

        ToolStrip toolStrip = e.ToolStrip;
        Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);

        // fill up the background
        LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
        FillWithDoubleGradient(ColorTable.ToolStripGradientBegin, ColorTable.ToolStripGradientMiddle, ColorTable.ToolStripGradientEnd, e.Graphics, bounds, _iconWellGradientWidth, _iconWellGradientWidth, mode, /*flipHorizontal=*/false);
    }

    private void RenderToolStripDropDownBackground(ToolStripRenderEventArgs e)
    {
        Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);

        using var brush = ColorTable.ToolStripDropDownBackground.GetCachedSolidBrushScope();
        e.Graphics.FillRectangle(brush, bounds);
    }

    private void RenderToolStripDropDownBorder(ToolStripRenderEventArgs e)
    {
        Graphics g = e.Graphics;

        if (e.ToolStrip is ToolStripDropDown toolStripDropDown)
        {
            Rectangle bounds = new(Point.Empty, toolStripDropDown.Size);

            using (var pen = ColorTable.MenuBorder.GetCachedPenScope())
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            if (toolStripDropDown is not ToolStripOverflow)
            {
                // make the neck connected.
                using var brush = ColorTable.ToolStripDropDownBackground.GetCachedSolidBrushScope();
                g.FillRectangle(brush, e.ConnectedArea);
            }
        }
    }

    private void RenderOverflowBackground(ToolStripItemRenderEventArgs e, bool rightToLeft)
    {
        ScaleObjectSizesIfNeeded(e.Item.DeviceDpi);

        Graphics g = e.Graphics;
        ToolStripOverflowButton? item = e.Item as ToolStripOverflowButton;
        Rectangle overflowBoundsFill = new(Point.Empty, e.Item.Size);
        Rectangle bounds = overflowBoundsFill;

        bool drawCurve = RoundedEdges && (item?.GetCurrentParent() is not MenuStrip);
        bool horizontal = e.ToolStrip is not null && e.ToolStrip.Orientation == Orientation.Horizontal;
        // undone RTL

        if (horizontal)
        {
            overflowBoundsFill.X += overflowBoundsFill.Width - _overflowButtonWidth + 1;
            overflowBoundsFill.Width = _overflowButtonWidth;
            if (rightToLeft)
            {
                overflowBoundsFill = LayoutUtils.RTLTranslate(overflowBoundsFill, bounds);
            }
        }
        else
        {
            overflowBoundsFill.Y = overflowBoundsFill.Height - _overflowButtonWidth + 1;
            overflowBoundsFill.Height = _overflowButtonWidth;
        }

        Color overflowButtonGradientBegin, overflowButtonGradientMiddle, overflowButtonGradientEnd, overflowBottomLeftShadow, overflowTopShadow;

        if (item is not null && item.Pressed)
        {
            overflowButtonGradientBegin = ColorTable.ButtonPressedGradientBegin;
            overflowButtonGradientMiddle = ColorTable.ButtonPressedGradientMiddle;
            overflowButtonGradientEnd = ColorTable.ButtonPressedGradientEnd;
            overflowBottomLeftShadow = ColorTable.ButtonPressedGradientBegin;
            overflowTopShadow = overflowBottomLeftShadow;
        }
        else if (item is not null && item.Selected)
        {
            overflowButtonGradientBegin = ColorTable.ButtonSelectedGradientBegin;
            overflowButtonGradientMiddle = ColorTable.ButtonSelectedGradientMiddle;
            overflowButtonGradientEnd = ColorTable.ButtonSelectedGradientEnd;
            overflowBottomLeftShadow = ColorTable.ButtonSelectedGradientMiddle;
            overflowTopShadow = overflowBottomLeftShadow;
        }
        else
        {
            overflowButtonGradientBegin = ColorTable.OverflowButtonGradientBegin;
            overflowButtonGradientMiddle = ColorTable.OverflowButtonGradientMiddle;
            overflowButtonGradientEnd = ColorTable.OverflowButtonGradientEnd;
            overflowBottomLeftShadow = ColorTable.ToolStripBorder;
            overflowTopShadow = horizontal ? ColorTable.ToolStripGradientMiddle : ColorTable.ToolStripGradientEnd;
        }

        if (drawCurve)
        {
            // draw shadow pixel on bottom left +1, +1
            using var pen = overflowBottomLeftShadow.GetCachedPenScope();
            Point start = new(overflowBoundsFill.Left - 1, overflowBoundsFill.Height - 2);
            Point end = new(overflowBoundsFill.Left, overflowBoundsFill.Height - 2);
            if (rightToLeft)
            {
                start.X = overflowBoundsFill.Right + 1;
                end.X = overflowBoundsFill.Right;
            }

            g.DrawLine(pen, start, end);
        }

        LinearGradientMode mode = horizontal ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

        // fill main body
        FillWithDoubleGradient(overflowButtonGradientBegin, overflowButtonGradientMiddle, overflowButtonGradientEnd, g, overflowBoundsFill, _iconWellGradientWidth, _iconWellGradientWidth, mode, false);

        if (!drawCurve)
        {
            return;
        }

        // Render shadow pixels (ToolStrip only)

        // top left and top right shadow pixels
        using (var brush = overflowTopShadow.GetCachedSolidBrushScope())
        {
            if (horizontal)
            {
                Point top1 = new(overflowBoundsFill.X - 2, 0);
                Point top2 = new(overflowBoundsFill.X - 1, 1);

                if (rightToLeft)
                {
                    top1.X = overflowBoundsFill.Right + 1;
                    top2.X = overflowBoundsFill.Right;
                }

                g.FillRectangle(brush, top1.X, top1.Y, 1, 1);
                g.FillRectangle(brush, top2.X, top2.Y, 1, 1);
            }
            else
            {
                g.FillRectangle(brush, overflowBoundsFill.Width - 3, overflowBoundsFill.Top - 1, 1, 1);
                g.FillRectangle(brush, overflowBoundsFill.Width - 2, overflowBoundsFill.Top - 2, 1, 1);
            }
        }

        using (var brush = overflowButtonGradientBegin.GetCachedSolidBrushScope())
        {
            if (horizontal)
            {
                Rectangle fillRect = new(overflowBoundsFill.X - 1, 0, 1, 1);
                if (rightToLeft)
                {
                    fillRect.X = overflowBoundsFill.Right;
                }

                g.FillRectangle(brush, fillRect);
            }
            else
            {
                g.FillRectangle(brush, overflowBoundsFill.X, overflowBoundsFill.Top - 1, 1, 1);
            }
        }
    }

    private void RenderToolStripCurve(ToolStripRenderEventArgs e)
    {
        Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);
        ToolStrip toolStrip = e.ToolStrip;
        Rectangle displayRect = toolStrip.DisplayRectangle;

        Graphics g = e.Graphics;

        Point topLeft = Point.Empty;
        Point topRight = new(bounds.Width - 1, 0);
        Point bottomLeft = new(0, bounds.Height - 1);

        // Add in shadow pixels - the detail that makes them look round

        // Draw in rounded shadow pixels on the top left & right (consider: if this is slow use precanned corners)
        using (var brush = ColorTable.ToolStripGradientMiddle.GetCachedSolidBrushScope())
        {
            // there are two shadow rects (one pixel wide) on the top
            Rectangle topLeftShadowRect = new(topLeft, s_onePix);
            topLeftShadowRect.X += 1;

            // second shadow rect
            Rectangle topLeftShadowRect2 = new(topLeft, s_onePix);
            topLeftShadowRect2.Y += 1;

            // on the right there are two more shadow rects
            Rectangle topRightShadowRect = new(topRight, s_onePix);
            topRightShadowRect.X -= 2; // was 2?

            // second top right shadow pix
            Rectangle topRightShadowRect2 = topRightShadowRect;
            topRightShadowRect2.Y += 1;
            topRightShadowRect2.X += 1;

            Rectangle[] paintRects = [topLeftShadowRect, topLeftShadowRect2, topRightShadowRect, topRightShadowRect2];

            // prevent the painting of anything that would obscure an item.
            for (int i = 0; i < paintRects.Length; i++)
            {
                if (displayRect.IntersectsWith(paintRects[i]))
                {
                    paintRects[i] = Rectangle.Empty;
                }
            }

            g.FillRectangles(brush, paintRects);
        }

        // Draw in rounded shadow pixels on the bottom left
        using (var brush = ColorTable.ToolStripGradientEnd.GetCachedSolidBrushScope())
        {
            // this gradient is the one just before the dark shadow line starts on pixel #3.
            Point gradientCopyPixel = bottomLeft;
            gradientCopyPixel.Offset(1, -1);
            if (!displayRect.Contains(gradientCopyPixel))
            {
                g.FillRectangle(brush, new Rectangle(gradientCopyPixel, s_onePix));
            }

            // set the one dark pixel in the bottom left hand corner
            Rectangle otherBottom = new(bottomLeft.X, bottomLeft.Y - 2, 1, 1);
            if (!displayRect.IntersectsWith(otherBottom))
            {
                g.FillRectangle(brush, otherBottom);
            }
        }
    }

    private void RenderSelectedButtonFill(Graphics g, Rectangle bounds)
    {
        if ((bounds.Width == 0) || (bounds.Height == 0))
        {
            return;  // can't new up a linear gradient brush with no dimension.
        }

        if (!UseSystemColors)
        {
            using Brush b = new LinearGradientBrush(
                bounds,
                ColorTable.ButtonSelectedGradientBegin,
                ColorTable.ButtonSelectedGradientEnd,
                LinearGradientMode.Vertical);

            g.FillRectangle(b, bounds);
        }
        else
        {
            using var brush = ColorTable.ButtonSelectedHighlight.GetCachedSolidBrushScope();
            g.FillRectangle(brush, bounds);
        }
    }

    private void RenderCheckedButtonFill(Graphics g, Rectangle bounds)
    {
        if ((bounds.Width == 0) || (bounds.Height == 0))
        {
            return;  // can't new up a linear gradient brush with no dimension.
        }

        if (!UseSystemColors)
        {
            using Brush b = new LinearGradientBrush(
                bounds,
                ColorTable.ButtonCheckedGradientBegin,
                ColorTable.ButtonCheckedGradientEnd,
                LinearGradientMode.Vertical);

            g.FillRectangle(b, bounds);
        }
        else
        {
            using var brush = ColorTable.ButtonCheckedHighlight.GetCachedSolidBrushScope();
            g.FillRectangle(brush, bounds);
        }
    }

    private void RenderSeparatorInternal(Graphics g, ToolStripItem item, Rectangle bounds, bool vertical)
    {
        bool isSeparator = item is ToolStripSeparator;
        bool isHorizontalSeparatorNotOnDropDownMenu = false;

        if (isSeparator)
        {
            if (vertical)
            {
                if (!item.IsOnDropDown)
                {
                    // center so that it matches office
                    bounds.Y += 3;
                    bounds.Height = Math.Max(0, bounds.Height - 6);
                }
            }
            else
            {
                // offset after the image margin
                if (item.GetCurrentParent() is ToolStripDropDownMenu dropDownMenu)
                {
                    if (dropDownMenu.RightToLeft == RightToLeft.No)
                    {
                        // scoot over by the padding (that will line you up with the text - but go two PX before so that it visually looks
                        // like the line meets up with the text).
                        bounds.X += dropDownMenu.Padding.Left - 2;
                        bounds.Width = dropDownMenu.Width - bounds.X;
                    }
                    else
                    {
                        // scoot over by the padding (that will line you up with the text - but go two PX before so that it visually looks
                        // like the line meets up with the text).
                        bounds.X += 2;
                        bounds.Width = dropDownMenu.Width - bounds.X - dropDownMenu.Padding.Right;
                    }
                }
                else
                {
                    isHorizontalSeparatorNotOnDropDownMenu = true;
                }
            }
        }

        using var foreColorPen = ColorTable.SeparatorDark.GetCachedPenScope();
        using var highlightColorPen = ColorTable.SeparatorLight.GetCachedPenScope();

        if (vertical)
        {
            if (bounds.Height >= 4)
            {
                bounds.Inflate(0, -2);     // scoot down 2PX and start drawing
            }

            bool rightToLeft = (item.RightToLeft == RightToLeft.Yes);
            Pen leftPen = (rightToLeft) ? highlightColorPen : foreColorPen;
            Pen rightPen = (rightToLeft) ? foreColorPen : highlightColorPen;

            // Draw dark line
            int startX = bounds.Width / 2;

            g.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom - 1);

            // Draw highlight one pixel to the right
            startX++;
            g.DrawLine(rightPen, startX, bounds.Top + 1, startX, bounds.Bottom);
        }
        else
        {
            // Horizontal separator- draw dark line

            if (isHorizontalSeparatorNotOnDropDownMenu && bounds.Width >= 4)
            {
                bounds.Inflate(-2, 0);     // scoot down 2PX and start drawing
            }

            int startY = bounds.Height / 2;

            g.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right - 1, startY);

            if (!isSeparator || isHorizontalSeparatorNotOnDropDownMenu)
            {
                // Draw highlight one pixel to the right
                startY++;
                g.DrawLine(highlightColorPen, bounds.Left + 1, startY, bounds.Right - 1, startY);
            }
        }
    }

    private void RenderPressedButtonFill(Graphics g, Rectangle bounds)
    {
        if ((bounds.Width == 0) || (bounds.Height == 0))
        {
            return;  // can't new up a linear gradient brush with no dimension.
        }

        if (!UseSystemColors)
        {
            using Brush b = new LinearGradientBrush(
                bounds,
                ColorTable.ButtonPressedGradientBegin,
                ColorTable.ButtonPressedGradientEnd,
                LinearGradientMode.Vertical);
            g.FillRectangle(b, bounds);
        }
        else
        {
            using var brush = ColorTable.ButtonPressedHighlight.GetCachedSolidBrushScope();
            g.FillRectangle(brush, bounds);
        }
    }

    private void RenderItemInternal(ToolStripItemRenderEventArgs e, bool useHotBorder)
    {
        Graphics g = e.Graphics;
        ToolStripItem item = e.Item;
        Rectangle bounds = new(Point.Empty, item.Size);
        bool drawHotBorder = false;

        Rectangle fillRect = (item.Selected) ? item.ContentRectangle : bounds;

        if (item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
        }

        if (item.Pressed)
        {
            RenderPressedButtonFill(g, bounds);
            drawHotBorder = useHotBorder;
        }
        else if (item.Selected)
        {
            RenderSelectedButtonFill(g, bounds);
            drawHotBorder = useHotBorder;
        }
        else if (item.Owner is not null && item.BackColor != item.Owner.BackColor)
        {
            using var brush = item.BackColor.GetCachedSolidBrushScope();
            g.FillRectangle(brush, bounds);
        }

        if (drawHotBorder)
        {
            using var pen = ColorTable.ButtonSelectedBorder.GetCachedPenScope();
            g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }
    }

    private void ScaleObjectSizesIfNeeded(int currentDeviceDpi)
    {
        if (ScaleHelper.IsThreadPerMonitorV2Aware)
        {
            if (_previousDeviceDpi != currentDeviceDpi)
            {
                ScaleArrowOffsetsIfNeeded(currentDeviceDpi);
                _overflowButtonWidth = ScaleHelper.ScaleToDpi(OVERFLOW_BUTTON_WIDTH, currentDeviceDpi);
                _overflowArrowWidth = ScaleHelper.ScaleToDpi(OVERFLOW_ARROW_WIDTH, currentDeviceDpi);
                _overflowArrowHeight = ScaleHelper.ScaleToDpi(OVERFLOW_ARROW_HEIGHT, currentDeviceDpi);
                _overflowArrowOffsetY = ScaleHelper.ScaleToDpi(OVERFLOW_ARROW_OFFSETY, currentDeviceDpi);

                _scaledGripPadding = ScaleHelper.ScaleToDpi(GripPadding, currentDeviceDpi);
                _iconWellGradientWidth = ScaleHelper.ScaleToDpi(ICON_WELL_GRADIENT_WIDTH, currentDeviceDpi);
                int scaledSize = ScaleHelper.ScaleToDpi(DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE, currentDeviceDpi);
                _scaledDropDownMenuItemPaintPadding = new Padding(scaledSize + 1, 0, scaledSize, 0);
                _previousDeviceDpi = currentDeviceDpi;
                _isScalingInitialized = true;
                return;
            }
        }

        if (_isScalingInitialized)
        {
            return;
        }

        if (ScaleHelper.IsScalingRequired)
        {
            ScaleArrowOffsetsIfNeeded();
            _overflowButtonWidth = ScaleHelper.ScaleToInitialSystemDpi(OVERFLOW_BUTTON_WIDTH);
            _overflowArrowWidth = ScaleHelper.ScaleToInitialSystemDpi(OVERFLOW_ARROW_WIDTH);
            _overflowArrowHeight = ScaleHelper.ScaleToInitialSystemDpi(OVERFLOW_ARROW_HEIGHT);
            _overflowArrowOffsetY = ScaleHelper.ScaleToInitialSystemDpi(OVERFLOW_ARROW_OFFSETY);

            _scaledGripPadding = ScaleHelper.ScaleToInitialSystemDpi(GripPadding);
            _iconWellGradientWidth = ScaleHelper.ScaleToInitialSystemDpi(ICON_WELL_GRADIENT_WIDTH);
            int scaledSize = ScaleHelper.ScaleToInitialSystemDpi(DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE);
            _scaledDropDownMenuItemPaintPadding = new Padding(scaledSize + 1, 0, scaledSize, 0);
        }

        _isScalingInitialized = true;
    }

    // This draws differently sized arrows than the base one...
    // used only for drawing the overflow button madness.
    private static Point RenderArrowInternal(Graphics g, Rectangle dropDownRect, ArrowDirection direction, Brush brush)
    {
        Point middle = new(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);

        // if the width is odd - favor pushing it over one pixel right.
        middle.X += dropDownRect.Width % 2;

        Point[] arrow = direction switch
        {
            ArrowDirection.Up =>
            [
                new(middle.X - Offset2X, middle.Y + 1),
                new(middle.X + Offset2X + 1, middle.Y + 1),
                new(middle.X, middle.Y - Offset2Y)
            ],
            ArrowDirection.Left =>
            [
                new(middle.X + Offset2X, middle.Y - Offset2Y - 1),
                new(middle.X + Offset2X, middle.Y + Offset2Y + 1),
                new(middle.X - 1, middle.Y)
            ],
            ArrowDirection.Right =>
            [
                new(middle.X - Offset2X, middle.Y - Offset2Y - 1),
                new(middle.X - Offset2X, middle.Y + Offset2Y + 1),
                new(middle.X + 1, middle.Y)
            ],
            _ =>
            [
                new(middle.X - Offset2X, middle.Y - 1),
                new(middle.X + Offset2X + 1, middle.Y - 1),
                new(middle.X, middle.Y + Offset2Y)
            ],
        };

        g.FillPolygon(brush, arrow);

        return middle;
    }

    #endregion PrivatePaintHelpers
}
