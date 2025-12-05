// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Provides dark mode rendering capabilities for ToolStrip controls in Windows Forms.
/// </summary>
/// <remarks>
///  <para>
///   This renderer is designed to be used with the ToolStripSystemRenderer to provide dark mode
///   styling while maintaining accessibility features. It inherits from ToolStripRenderer
///   and overrides necessary methods to provide dark-themed rendering.
///  </para>
/// </remarks>
internal class ToolStripSystemDarkModeRenderer : ToolStripRenderer
{
    /// <summary>
    ///  Initializes a new instance of the ToolStripSystemDarkModeRenderer class.
    /// </summary>
    public ToolStripSystemDarkModeRenderer()
    {
    }

    /// <summary>
    ///  Initializes a new instance of the ToolStripSystemDarkModeRenderer class with the specified default state.
    /// </summary>
    /// <param name="isSystemDefaultAlternative">
    ///  True if this should be seen as a variation of the default renderer
    ///  (so, no _custom_ renderer provided by the user); otherwise, false.
    /// </param>
    internal ToolStripSystemDarkModeRenderer(bool isSystemDefaultAlternative)
        : base(isSystemDefaultAlternative)
    {
    }

    /// <summary>
    ///  Gets dark-appropriate system colors based on the control type.
    /// </summary>
    /// <param name="color">The color to convert to a dark mode equivalent.</param>
    /// <returns>A color suitable for dark mode.</returns>
    private static Color GetDarkModeColor(Color color) =>
        color switch
        {
            Color c when c == SystemColors.Control => Color.FromArgb(45, 45, 45),
            Color c when c == SystemColors.ControlLight => Color.FromArgb(60, 60, 60),
            Color c when c == SystemColors.ControlDark => Color.FromArgb(30, 30, 30),
            Color c when c == SystemColors.ControlText => Color.FromArgb(240, 240, 240),
            Color c when c == SystemColors.ButtonFace => Color.FromArgb(45, 45, 45),
            Color c when c == SystemColors.Highlight => Color.FromArgb(0, 120, 215),
            Color c when c == SystemColors.HighlightText => Color.White,
            Color c when c == SystemColors.Window => Color.FromArgb(32, 32, 32),
            Color c when c == SystemColors.WindowText => Color.FromArgb(240, 240, 240),
            Color c when c == SystemColors.GrayText => Color.FromArgb(153, 153, 153),
            Color c when c == SystemColors.InactiveBorder => Color.FromArgb(70, 70, 70),
            Color c when c == SystemColors.ButtonHighlight => Color.FromArgb(80, 80, 80),
            Color c when c == SystemColors.ButtonShadow => Color.FromArgb(20, 20, 20),
            Color c when c == SystemColors.Menu => Color.FromArgb(45, 45, 45),
            Color c when c == SystemColors.MenuText => Color.FromArgb(240, 240, 240),
            _ when color.GetBrightness() > 0.5 => ControlPaint.Dark(color, 0.2f),
            _ => color
        };

    /// <summary>
    ///  Creates a dark mode compatible brush. Important:
    ///  Always do: `using var brush = GetDarkModeBrush(color)`,
    ///  since you're dealing with a cached brush => scope, really!
    ///  Creates a dark mode compatible brush. Always use 'using var brush = GetDarkModeBrush(color)'.
    /// </summary>
    /// <param name="color">The system color to convert.</param>
    /// <returns>A brush with the dark mode color.</returns>
    private static SolidBrushCache.Scope GetDarkModeBrush(Color color) =>
        GetDarkModeColor(color).GetCachedSolidBrushScope();

    /// <summary>
    ///  Creates a dark mode compatible pen. Always use 'using var pen = GetDarkModePen(color)'.
    /// </summary>
    /// <param name="color">The system color to convert.</param>
    /// <returns>A pen with the dark mode color.</returns>
    private static PenCache.Scope GetDarkModePen(Color color) =>
        GetDarkModeColor(color).GetCachedPenScope();

    /// <summary>
    ///  Returns whether the background should be painted.
    /// </summary>
    /// <param name="toolStrip">The ToolStrip to check.</param>
    /// <returns>true if the background should be painted; otherwise, false.</returns>
    private static bool ShouldPaintBackground(ToolStrip toolStrip)
        => toolStrip is null || toolStrip.BackgroundImage is null;

    /// <summary>
    ///  Fills the background with the specified color.
    /// </summary>
    /// <param name="g">The Graphics to draw on.</param>
    /// <param name="bounds">The bounds to fill.</param>
    /// <param name="backColor">The background color.</param>
    private static void FillBackground(Graphics g, Rectangle bounds, Color backColor)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using var brush = GetDarkModeBrush(backColor);

        g.FillRectangle(brush, bounds);
    }

    /// <summary>
    ///  Raises the RenderToolStripBackground event.
    /// </summary>
    /// <param name="e">A ToolStripRenderEventArgs that contains the event data.</param>
    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        ToolStrip toolStrip = e.ToolStrip;
        Rectangle bounds = e.AffectedBounds;

        if (!ShouldPaintBackground(toolStrip))
        {
            return;
        }

        Graphics g = e.Graphics;

        if (toolStrip is StatusStrip)
        {
            RenderStatusStripBackground(e);

            return;
        }

        if (toolStrip.IsDropDown)
        {
            // Dark mode dropdown background
            FillBackground(g, bounds, GetDarkModeColor(SystemColors.Menu));
        }
        else if (toolStrip is MenuStrip)
        {
            // Dark mode menu background
            FillBackground(g, bounds, GetDarkModeColor(SystemColors.Menu));
        }
        else
        {
            // Standard ToolStrip background
            FillBackground(g, bounds, GetDarkModeColor(e.BackColor));
        }
    }

    /// <summary>
    ///  Renders the StatusStrip background in dark mode.
    /// </summary>
    /// <param name="e">A ToolStripRenderEventArgs that contains the event data.</param>
    internal static void RenderStatusStripBackground(ToolStripRenderEventArgs e)
    {
        Rectangle bounds = e.AffectedBounds;

        // Dark mode StatusStrip background
        FillBackground(e.Graphics, bounds, GetDarkModeColor(SystemColors.Control));
    }

    /// <summary>
    ///  Raises the RenderToolStripBorder event.
    /// </summary>
    /// <param name="e">A ToolStripRenderEventArgs that contains the event data.</param>
    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        ToolStrip toolStrip = e.ToolStrip;

        if (toolStrip is StatusStrip)
        {
            RenderStatusStripBorder(e);
            return;
        }

        Graphics g = e.Graphics;
        Rectangle bounds = e.ToolStrip.ClientRectangle;

        using var borderPen = GetDarkModePen(SystemColors.ControlDark);

        if (toolStrip is ToolStripDropDown toolStripDropDown)
        {
            if (toolStripDropDown.DropShadowEnabled)
            {
                bounds.Width -= 1;
                bounds.Height -= 1;

                g.DrawRectangle(borderPen, bounds);
            }
            else
            {
                g.DrawRectangle(borderPen, bounds);
            }

            return;
        }

        g.DrawLine(borderPen, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
    }

    /// <summary>
    ///  Renders the StatusStrip border in dark mode.
    /// </summary>
    /// <param name="e">A ToolStripRenderEventArgs that contains the event data.</param>
    private static void RenderStatusStripBorder(ToolStripRenderEventArgs e)
    {
        Graphics g = e.Graphics;
        Rectangle bounds = e.ToolStrip.ClientRectangle;

        using var borderPen = GetDarkModePen(SystemColors.ControlDark);

        g.DrawLine(borderPen, 0, 0, bounds.Width, 0);
    }

    /// <summary>
    ///  Raises the RenderItemBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Rectangle bounds = new(Point.Empty, e.Item.Size);

        if (e.Item.IsOnDropDown)
        {
            bounds.X += 2;
            bounds.Width -= 3;
        }

        if (e.Item.Selected || e.Item.Pressed)
        {
            // Dark mode selection highlight
            using var highlightBrush = GetDarkModeBrush(SystemColors.Highlight);
            e.Graphics.FillRectangle(highlightBrush, bounds);

            return;
        }

        // Render background image if available
        if (e.Item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(
                g: e.Graphics,
                backgroundImage: e.Item.BackgroundImage,
                backColor: GetDarkModeColor(e.Item.BackColor),
                backgroundImageLayout: e.Item.BackgroundImageLayout,
                bounds: e.Item.ContentRectangle,
                clipRect: bounds);

            return;
        }

        if (e.Item.BackColor != Color.Transparent && e.Item.BackColor != Color.Empty)
        {
            // Custom background color (apply dark mode transformation)
            FillBackground(e.Graphics, bounds, e.Item.BackColor);
        }
    }

    /// <summary>
    ///  Raises the RenderButtonBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Rectangle bounds = new(Point.Empty, e.Item.Size);
        bool isPressed;
        bool isSelected;

        if (e.Item is ToolStripButton button)
        {
            isPressed = button.Pressed;
            isSelected = button.Selected || button.Checked;
        }
        else
        {
            isPressed = e.Item.Pressed;
            isSelected = e.Item.Selected;
        }

        if (isPressed || isSelected)
        {
            using var fillColor = isPressed
                ? GetDarkModeBrush(SystemColors.ControlDark)
                : GetDarkModeBrush(SystemColors.Highlight);

            e.Graphics.FillRectangle(fillColor, bounds);
        }
    }

    /// <summary>
    ///  Raises the RenderDropDownButtonBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        OnRenderButtonBackground(e);
    }

    /// <summary>
    ///  Raises the RenderSplitButtonBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Item is not ToolStripSplitButton splitButton)
        {
            return;
        }

        Rectangle bounds = new(Point.Empty, e.Item.Size);

        // Render the background based on state
        if (splitButton.Selected || splitButton.Pressed)
        {
            using var fillColor = splitButton.Pressed
                ? GetDarkModeBrush(SystemColors.ControlDark)
                : GetDarkModeBrush(SystemColors.Highlight);

            e.Graphics.FillRectangle(fillColor, bounds);
        }

        // Draw the split line
        Rectangle dropDownRect = splitButton.DropDownButtonBounds;
        using var linePen = GetDarkModePen(SystemColors.ControlDark);

        e.Graphics.DrawLine(
            pen: linePen,
            x1: dropDownRect.Left - 1,
            y1: dropDownRect.Top + 2,
            x2: dropDownRect.Left - 1,
            y2: dropDownRect.Bottom - 2);

        DrawArrow(new ToolStripArrowRenderEventArgs(
            g: e.Graphics,
            toolStripItem: e.Item,
            arrowRectangle: dropDownRect,
            arrowColor: SystemColors.ControlText,
            arrowDirection: ArrowDirection.Down));
    }

    /// <summary>
    ///  Raises the RenderSeparator event.
    /// </summary>
    /// <param name="e">A ToolStripSeparatorRenderEventArgs that contains the event data.</param>
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Rectangle bounds = e.Item.ContentRectangle;
        bool isVertical = e.Vertical;
        Graphics g = e.Graphics;

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        bool rightToLeft = e.Item.RightToLeft == RightToLeft.Yes;

        if (isVertical)
        {
            int startX = bounds.Width / 2;

            if (rightToLeft)
            {
                using var leftPen = GetDarkModeColor(SystemColors.ControlDark).GetCachedPenScope();
                g.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom);

                startX++;

                using var rightPen = GetDarkModeColor(SystemColors.ButtonShadow).GetCachedPenScope();
                g.DrawLine(rightPen, startX, bounds.Top, startX, bounds.Bottom);
            }
            else
            {
                using var leftPen = GetDarkModeColor(SystemColors.ButtonShadow).GetCachedPenScope();
                g.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom);

                startX++;

                using var rightPen = GetDarkModeColor(SystemColors.ControlDark).GetCachedPenScope();
                g.DrawLine(rightPen, startX, bounds.Top, startX, bounds.Bottom);
            }
        }
        else
        {
            // Horizontal separator
            if (bounds.Width >= 4)
            {
                bounds.Inflate(-2, 0);
            }

            int startY = bounds.Height / 2;

            using var foreColorPen = GetDarkModeColor(SystemColors.ControlDark).GetCachedPenScope();
            g.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right, startY);

            startY++;

            using var darkModePen = GetDarkModeColor(SystemColors.ButtonShadow).GetCachedPenScope();
            g.DrawLine(darkModePen, bounds.Left, startY, bounds.Right, startY);
        }
    }

    /// <summary>
    ///  Raises the RenderOverflowButtonBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Item is not ToolStripOverflowButton item)
        {
            return;
        }

        Rectangle bounds = new(Point.Empty, e.Item.Size);

        // Render the background based on state
        if (item.Selected || item.Pressed)
        {
            using var fillBrush = item.Pressed
                ? GetDarkModeColor(SystemColors.ControlDark).GetCachedSolidBrushScope()
                : GetDarkModeColor(SystemColors.Highlight).GetCachedSolidBrushScope();

            e.Graphics.FillRectangle(fillBrush, bounds);
        }

        Rectangle arrowRect = item.ContentRectangle;

        Point middle = new(
            x: arrowRect.Left + arrowRect.Width / 2,
            y: arrowRect.Top + arrowRect.Height / 2);

        // Default to down arrow for overflow buttons
        ArrowDirection arrowDirection = ArrowDirection.Down;

        // Determine actual direction based on dropdown direction
        ToolStripDropDownDirection direction = item.DropDownDirection;

        if (direction is ToolStripDropDownDirection.AboveLeft or ToolStripDropDownDirection.AboveRight)
        {
            arrowDirection = ArrowDirection.Up;
        }
        else if (direction == ToolStripDropDownDirection.Left)
        {
            arrowDirection = ArrowDirection.Left;
        }
        else if (direction == ToolStripDropDownDirection.Right)
        {
            arrowDirection = ArrowDirection.Right;
        }

        // else default to ArrowDirection.Down

        // Set arrow color based on state
        using var arrowBrush = GetDarkModeColor(
            item.Pressed || item.Selected
                ? SystemColors.HighlightText
                : SystemColors.ControlText)
            .GetCachedSolidBrushScope();

        // Define arrow polygon based on direction
        Point[] arrow = arrowDirection switch
        {
            ArrowDirection.Up =>
                [
                    new Point(middle.X - 2, middle.Y + 1),
                    new Point(middle.X + 3, middle.Y + 1),
                    new Point(middle.X, middle.Y - 2)
                ],
            ArrowDirection.Left =>
                [
                    new Point(middle.X + 2, middle.Y - 4),
                    new Point(middle.X + 2, middle.Y + 4),
                    new Point(middle.X - 2, middle.Y)
                ],
            ArrowDirection.Right =>
                [
                    new Point(middle.X - 2, middle.Y - 4),
                    new Point(middle.X - 2, middle.Y + 4),
                    new Point(middle.X + 2, middle.Y)
                ],
            _ =>
                [
                    new Point(middle.X - 2, middle.Y - 1),
                    new Point(middle.X + 3, middle.Y - 1),
                    new Point(middle.X, middle.Y + 2)
                ],
        };

        // Draw the arrow
        e.Graphics.FillPolygon(arrowBrush, arrow);
    }

    /// <summary>
    ///  Raises the RenderGrip event.
    /// </summary>
    /// <param name="e">A ToolStripGripRenderEventArgs that contains the event data.</param>
    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.GripBounds.Width <= 0 || e.GripBounds.Height <= 0)
        {
            return;
        }

        using var darkColorBrush = GetDarkModeColor(SystemColors.ControlDark).GetCachedSolidBrushScope();
        using var lightColorBrush = GetDarkModeColor(SystemColors.ControlLight).GetCachedSolidBrushScope();

        ToolStrip toolStrip = e.ToolStrip;
        Graphics g = e.Graphics;
        Rectangle bounds = e.GripBounds;

        // Draw grip dots
        if (toolStrip.Orientation == Orientation.Horizontal)
        {
            // Draw horizontal grip
            int y = bounds.Top + 2;

            while (y < bounds.Bottom - 3)
            {
                g.FillRectangle(darkColorBrush, bounds.Left + 2, y, 1, 1);
                g.FillRectangle(lightColorBrush, bounds.Left + 3, y + 1, 1, 1);

                y += 3;
            }
        }
        else
        {
            // Draw vertical grip
            int x = bounds.Left + 2;

            while (x < bounds.Right - 3)
            {
                g.FillRectangle(darkColorBrush, x, bounds.Top + 2, 1, 1);
                g.FillRectangle(lightColorBrush, x + 1, bounds.Top + 3, 1, 1);

                x += 3;
            }
        }
    }

    /// <summary>
    ///  Raises the RenderArrow event in the derived class with dark mode support.
    /// </summary>
    /// <param name="e">A ToolStripArrowRenderEventArgs that contains the event data.</param>
    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        Debug.Assert(e.Item is not null, "The ToolStripItem should not be null on rendering the Arrow.");

        Color arrowColor = (e.Item.Selected || e.Item.Pressed)
            ? GetDarkModeColor(SystemColors.HighlightText)
            : GetDarkModeColor(e.ArrowColor);

        RenderArrowCore(e, arrowColor);
    }

    /// <summary>
    ///  Raises the RenderImageMargin event.
    /// </summary>
    /// <param name="e">A ToolStripRenderEventArgs that contains the event data.</param>
    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // Fill the image margin with a slightly different color than the background
        using var marginColorBrush = GetDarkModeBrush(SystemColors.ControlLight);

        e.Graphics.FillRectangle(marginColorBrush, e.AffectedBounds);
    }

    /// <summary>
    ///  Raises the RenderItemText event.
    /// </summary>
    /// <param name="e">A ToolStripItemTextRenderEventArgs that contains the event data.</param>
    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Color textColor = (e.Item.Selected || e.Item.Pressed)
            ? GetDarkModeColor(SystemColors.HighlightText)
            : !e.Item.Enabled
                ? GetDarkModeColor(SystemColors.GrayText)
                : GetDarkModeColor(e.TextColor);

        TextRenderer.DrawText(
            dc: e.Graphics,
            text: e.Text,
            font: e.TextFont,
            bounds: e.TextRectangle,
            foreColor: textColor,
            flags: e.TextFormat);
    }

    /// <summary>
    ///  Raises the RenderItemImage event.
    /// </summary>
    /// <param name="e">A ToolStripItemImageRenderEventArgs that contains the event data.</param>
    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Image is null)
        {
            return;
        }

        // DarkMode adjustments for the image are done by
        // the base class implementation already.
        Image image = e.Item.Enabled
            ? e.Image
            : CreateDisabledImage(e.Image);

        e.Graphics.DrawImage(image, e.ImageRectangle);
    }

    /// <summary>
    ///  Raises the RenderLabelBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
        // Use default item background rendering
        OnRenderItemBackground(e);
    }

    /// <summary>
    ///  Raises the RenderToolStripStatusLabelBackground event.
    /// </summary>
    /// <param name="e">A ToolStripItemRenderEventArgs that contains the event data.</param>
    protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        using var highLightBrush = GetDarkModeBrush(SystemColors.GrayText);
        using var shadowBrush = GetDarkModeBrush(SystemColors.ButtonShadow);

        OnRenderStatusStripSizingGrip(
            e: e,
            highLightBrush: highLightBrush,
            shadowBrush: shadowBrush);
    }
}
