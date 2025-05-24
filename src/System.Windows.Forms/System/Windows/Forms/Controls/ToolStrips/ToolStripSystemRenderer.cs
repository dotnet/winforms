// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides system rendering for ToolStrip controls with support for dark mode.
/// </summary>
public class ToolStripSystemRenderer : ToolStripRenderer
{
    [ThreadStatic]
    private static VisualStyleRenderer? t_renderer;

    private ToolStripRenderer? _toolStripHighContrastRenderer;
    private ToolStripRenderer? _toolStripDarkModeRenderer;

    /// <summary>
    ///  Initializes a new instance of the ToolStripSystemRenderer class.
    /// </summary>
    public ToolStripSystemRenderer()
    {
    }

    /// <summary>
    ///  Initializes a new instance of the ToolStripSystemRenderer class with the specified default state.
    /// </summary>
    /// <param name="isDefault">true if this is the default renderer; otherwise, false.</param>
    internal ToolStripSystemRenderer(bool isDefault) : base(isDefault)
    {
    }

    /// <summary>
    ///  Gets the HighContrastRenderer for accessibility support.
    /// </summary>
    internal ToolStripRenderer HighContrastRenderer
    {
        get
        {
            _toolStripHighContrastRenderer ??= new ToolStripHighContrastRenderer(systemRenderMode: false);

            return _toolStripHighContrastRenderer;
        }
    }

    /// <summary>
    ///  Gets the DarkModeRenderer for dark mode support.
    /// </summary>
    internal ToolStripRenderer DarkModeRenderer
    {
        get
        {
            _toolStripDarkModeRenderer ??= new ToolStripSystemDarkModeRenderer(isSystemDefaultAlternative: false);

            return _toolStripDarkModeRenderer;
        }
    }

    /// <summary>
    ///  Gets the renderer that should be used based on current display settings.
    /// </summary>
    internal override ToolStripRenderer? RendererOverride
    {
        get
        {
            if (DisplayInformation.HighContrast)
            {
                return HighContrastRenderer;
            }

#pragma warning disable WFO5001
            if (Application.IsDarkModeEnabled)
            {
                return DarkModeRenderer;
            }
#pragma warning restore WFO5001

            return null;
        }
    }

    /// <summary>
    ///  Get the Visual Style Renderer. This is used to draw the background of the ToolStrip.
    /// </summary>
    private static VisualStyleRenderer? VisualStyleRenderer
    {
        get
        {
            if (Application.RenderWithVisualStyles)
            {
                if (t_renderer is null && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ToolBar.Button.Normal))
                {
                    t_renderer = new VisualStyleRenderer(VisualStyleElement.ToolBar.Button.Normal);
                }
            }
            else
            {
                t_renderer = null;
            }

            return t_renderer;
        }
    }

    /// <summary>
    ///  Fill the item's background as bounded by the rectangle.
    /// </summary>
    private static void FillBackground(Graphics g, Rectangle bounds, Color backColor)
    {
        using var backBrush = backColor.GetCachedSolidBrushScope();

        g.FillRectangle(backBrush, bounds);
    }

    /// <summary>
    ///  Translates the ToolStrip item state into a toolbar state, which is something the renderer understands.
    /// </summary>
    private static int GetItemState(ToolStripItem item) =>
        (int)GetToolBarState(item);

    /// <summary>
    ///  Translates the ToolStrip item state into a toolbar state, which is something the renderer understands.
    /// </summary>
    private static int GetSplitButtonDropDownItemState(ToolStripSplitButton? item) =>
        (int)GetSplitButtonToolBarState(item, true);

    /// <summary>
    ///  Translates the ToolStrip item state into a toolbar state, which is something the renderer understands.
    /// </summary>
    private static int GetSplitButtonItemState(ToolStripSplitButton? item) =>
        (int)GetSplitButtonToolBarState(item, false);

    /// <summary>
    ///  Translates the ToolStrip item state into a toolbar state, which is something the renderer understands.
    /// </summary>
    private static ToolBarState GetSplitButtonToolBarState(ToolStripSplitButton? button, bool dropDownButton)
    {
        ToolBarState state = ToolBarState.Normal;

        if (button is not null)
        {
            if (!button.Enabled)
            {
                state = ToolBarState.Disabled;
            }
            else if (dropDownButton)
            {
                if (button.DropDownButtonPressed || button.ButtonPressed)
                {
                    state = ToolBarState.Pressed;
                }
                else if (button.DropDownButtonSelected || button.ButtonSelected)
                {
                    state = ToolBarState.Hot;
                }
            }
            else
            {
                if (button.ButtonPressed)
                {
                    state = ToolBarState.Pressed;
                }
                else if (button.ButtonSelected)
                {
                    state = ToolBarState.Hot;
                }
            }
        }

        return state;
    }

    /// <summary>
    ///  Translates the ToolStrip item state into a toolbar state, which is something the renderer understands.
    /// </summary>
    private static ToolBarState GetToolBarState(ToolStripItem item)
    {
        ToolBarState state = ToolBarState.Normal;

        if (!item.Enabled)
        {
            state = ToolBarState.Disabled;
        }

        if (item is ToolStripButton toolStripButton && toolStripButton.Checked)
        {
            state = toolStripButton.Selected
                ? ToolBarState.Hot
                : ToolBarState.Checked;
        }
        else if (item.Pressed)
        {
            state = ToolBarState.Pressed;
        }
        else if (item.Selected)
        {
            state = ToolBarState.Hot;
        }

        return state;
    }

    /// <summary>
    ///  Draw the ToolStrip background. ToolStrip users should override this if they want to draw differently.
    /// </summary>
    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        if (RendererOverride is ToolStripSystemDarkModeRenderer darkModeRenderer
            && darkModeRenderer.IsSystemDefaultAlternative)
        {
            base.OnRenderToolStripBackground(e);

            return;
        }

        ToolStrip toolStrip = e.ToolStrip;
        Rectangle bounds = e.AffectedBounds;

        if (!ShouldPaintBackground(toolStrip))
        {
            return;
        }

        var g = e.Graphics;

        if (toolStrip is StatusStrip)
        {
            RenderStatusStripBackground(e);
        }
        else
        {
            if (DisplayInformation.HighContrast)
            {
                FillBackground(g, bounds, SystemColors.ButtonFace);
            }
            else if (DisplayInformation.LowResolution)
            {
                FillBackground(
                    g,
                    bounds,
                    toolStrip is ToolStripDropDown
                        ? SystemColors.ControlLight
                        : e.BackColor);
            }
            else if (toolStrip.IsDropDown)
            {
                FillBackground(
                    g,
                    bounds,
                    ToolStripManager.VisualStylesEnabled
                        ? SystemColors.Menu
                        : e.BackColor);
            }
            else if (toolStrip is MenuStrip)
            {
                FillBackground(
                    g,
                    bounds,
                    ToolStripManager.VisualStylesEnabled
                        ? SystemColors.MenuBar
                        : e.BackColor);
            }
            else if (ToolStripManager.VisualStylesEnabled
                && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Band.Normal))
            {
                VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
                vsRenderer.SetParameters(VisualStyleElement.ToolBar.Bar.Normal);
                vsRenderer.DrawBackground(g, bounds);
            }
            else
            {
                FillBackground(
                    g,
                    bounds,
                    ToolStripManager.VisualStylesEnabled
                        ? SystemColors.MenuBar
                        : e.BackColor);
            }
        }
    }

    /// <summary>
    ///  Draw the border around the ToolStrip. This should be done as the last step.
    /// </summary>
    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        ToolStrip toolStrip = e.ToolStrip;
        Rectangle bounds = toolStrip.ClientRectangle;
        Graphics graphics = e.Graphics;

        if (toolStrip is StatusStrip)
        {
            RenderStatusStripBorder(e);

            return;
        }

        if (toolStrip is ToolStripDropDown toolStripDropDown)
        {
            // Paint the border for the window depending on whether or not we have a drop shadow effect.
            if (toolStripDropDown.DropShadowEnabled && ToolStripManager.VisualStylesEnabled)
            {
                bounds.Width -= 1;
                bounds.Height -= 1;
                graphics.DrawRectangle(SystemPens.ControlDark, bounds);
            }
            else
            {
                ControlPaint.DrawBorder3D(graphics, bounds, Border3DStyle.Raised);
            }

            return;
        }

        if (ToolStripManager.VisualStylesEnabled)
        {
            graphics.DrawLine(SystemPens.ButtonHighlight, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
            graphics.DrawLine(SystemPens.InactiveBorder, 0, bounds.Bottom - 2, bounds.Width, bounds.Bottom - 2);
        }
        else
        {
            graphics.DrawLine(SystemPens.ButtonHighlight, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
            graphics.DrawLine(SystemPens.ButtonShadow, 0, bounds.Bottom - 2, bounds.Width, bounds.Bottom - 2);
        }
    }

    /// <summary>
    ///  Draw the grip. ToolStrip users should override this if they want to draw differently.
    /// </summary>
    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
        Graphics graphics = e.Graphics;
        Rectangle bounds = new(Point.Empty, e.GripBounds.Size);
        bool verticalGrip = e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical;

        if (ToolStripManager.VisualStylesEnabled
            && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Gripper.Normal))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;

            if (verticalGrip)
            {
                vsRenderer.SetParameters(VisualStyleElement.Rebar.Gripper.Normal);

                // Make sure height is an even interval of 4.
                bounds.Height = ((bounds.Height - 2) / 4) * 4;
                bounds.Y = Math.Max(0, (e.GripBounds.Height - bounds.Height - 2) / 2);
            }
            else
            {
                vsRenderer.SetParameters(VisualStyleElement.Rebar.GripperVertical.Normal);
            }

            vsRenderer.DrawBackground(graphics, bounds);

            return;
        }

        // Do some fixup so that we don't paint from end to end.
        Color backColor = e.ToolStrip.BackColor;
        FillBackground(graphics, bounds, backColor);

        if (verticalGrip)
        {
            if (bounds.Height >= 4)
            {
                bounds.Inflate(0, -2);
            }

            bounds.Width = 3;
        }
        else
        {
            if (bounds.Width >= 4)
            {
                bounds.Inflate(-2, 0);
            }

            bounds.Height = 3;
        }

        RenderSmall3DBorderInternal(graphics, bounds, ToolBarState.Hot, e.ToolStrip.RightToLeft == RightToLeft.Yes);
    }

    /// <summary>
    ///  Draw the items background
    /// </summary>
    protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
    {
    }

    /// <summary>
    ///  Draw the items background
    /// </summary>
    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
    }

    /// <summary>
    ///  Draw the button background
    /// </summary>
    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        // If system in high contrast mode and specific renderer override is defined, use that.
        // For ToolStripSystemRenderer in High Contrast mode the RendererOverride property will be ToolStripHighContrastRenderer.
        if (RendererOverride is not null)
        {
            base.OnRenderButtonBackground(e);
            return;
        }

        RenderItemInternal(e);
    }

    /// <summary>
    ///  Draw the button background
    /// </summary>
    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        // If system in high contrast mode and specific renderer override is defined, use that.
        // For ToolStripSystemRenderer in High Contrast mode the RendererOverride property will be ToolStripHighContrastRenderer.
        if (RendererOverride is not null)
        {
            base.OnRenderDropDownButtonBackground(e);
            return;
        }

        RenderItemInternal(e);
    }

    /// <summary>
    ///  Draw the button background
    /// </summary>
    protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
    {
        ToolStripItem item = e.Item;
        Graphics graphics = e.Graphics;

        if (ToolStripManager.VisualStylesEnabled && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Chevron.Normal))
        {
            VisualStyleElement chevronElement = VisualStyleElement.Rebar.Chevron.Normal;
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(chevronElement.ClassName, chevronElement.Part, GetItemState(item));
            vsRenderer.DrawBackground(graphics, new Rectangle(Point.Empty, item.Size));
        }
        else
        {
            RenderItemInternal(e);
            Color arrowColor = item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
            DrawArrow(new ToolStripArrowRenderEventArgs(graphics, item, new Rectangle(Point.Empty, item.Size), arrowColor, ArrowDirection.Down));
        }
    }

    /// <summary>
    ///  Draw the button background
    /// </summary>
    protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
    {
        RenderLabelInternal(e);
    }

    /// <summary>
    ///  Draw the items background
    /// </summary>
    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        ToolStripMenuItem? item = e.Item as ToolStripMenuItem;
        Graphics graphics = e.Graphics;

        if (item is MdiControlStrip.SystemMenuItem)
        {
            // No highlights are painted behind a system menu item.
            return;
        }

        if (item is not null)
        {
            Rectangle bounds = new(Point.Empty, item.Size);

            if (item.IsTopLevel && !ToolStripManager.VisualStylesEnabled)
            {
                // Classic Mode (3D edges)
                // Draw box highlight for toplevel items in downlevel platforms.
                if (item.BackgroundImage is not null)
                {
                    ControlPaint.DrawBackgroundImage(
                        g: graphics,
                        backgroundImage: item.BackgroundImage,
                        backColor: item.BackColor,
                        backgroundImageLayout: item.BackgroundImageLayout,
                        bounds: item.ContentRectangle,
                        clipRect: item.ContentRectangle);
                }
                else if (item.RawBackColor != Color.Empty)
                {
                    FillBackground(graphics, item.ContentRectangle, item.BackColor);
                }

                // Toplevel menu items do 3D borders.
                ToolBarState state = GetToolBarState(item);

                RenderSmall3DBorderInternal(
                    graphics: graphics,
                    bounds: bounds,
                    state: state,
                    rightToLeft: item.RightToLeft == RightToLeft.Yes);

                return;
            }

            // Modern MODE (no 3D edges)
            // Draw blue filled highlight for toplevel items in themed platforms or items parented to a drop down.
            Rectangle fillRect = new(Point.Empty, item.Size);

            if (item.IsOnDropDown)
            {
                // Scoot in by 2 pixels when selected.
                fillRect.X += 2;
                fillRect.Width -= 3; // It's already 1 away from the right edge.
            }

            if (item.Selected || item.Pressed)
            {
                // Only paint the background if the menu item is enabled.
                if (item.Enabled)
                {
                    // SystemBrushes.Highlight is already cached and must not be disposed.
                    graphics.FillRectangle(SystemBrushes.Highlight, fillRect);
                }

                Color borderColor = ToolStripManager.VisualStylesEnabled
                    ? SystemColors.Highlight
                    : ProfessionalColors.MenuItemBorder;

                // Draw selection border - always drawn regardless of Enabled.
                using var pen = borderColor.GetCachedPenScope();

                graphics.DrawRectangle(
                    pen: pen,
                    x: bounds.X,
                    y: bounds.Y,
                    width: bounds.Width - 1,
                    height: bounds.Height - 1);

                return;
            }

            if (item.BackgroundImage is not null)
            {
                ControlPaint.DrawBackgroundImage(
                    g: graphics,
                    backgroundImage: item.BackgroundImage,
                    backColor: item.BackColor,
                    backgroundImageLayout: item.BackgroundImageLayout,
                    bounds: item.ContentRectangle,
                    clipRect: fillRect);
            }
            else if (!(ToolStripManager.VisualStylesEnabled || item.RawBackColor == Color.Empty))
            {
                FillBackground(graphics, fillRect, item.BackColor);
            }
        }
    }

    /// <summary>
    ///  Draws a toolbar separator. ToolStrip users should override this function to change the
    ///  drawing of all separators.
    /// </summary>
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        RenderSeparatorInternal(
            graphics: e.Graphics,
            item: e.Item,
            bounds: new Rectangle(Point.Empty, e.Item.Size),
            vertical: e.Vertical);
    }

    protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
    {
        RenderLabelInternal(e);

        ToolStripStatusLabel? item = e.Item as ToolStripStatusLabel;

        if (item is not null)
        {
            ControlPaint.DrawBorder3D(
                graphics: e.Graphics,
                rectangle: new Rectangle(0, 0, item.Width - 1, item.Height - 1),
                style: item.BorderStyle,
                sides: (Border3DSide)item.BorderSides);
        }
    }

    /// <summary>
    ///  Draw the item's background.
    /// </summary>
    protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
    {
        // If system in high contrast mode and specific renderer override is defined, use that.
        // For ToolStripSystemRenderer in High Contrast mode the RendererOverride property will be ToolStripHighContrastRenderer.
        if (RendererOverride is not null)
        {
            base.OnRenderSplitButtonBackground(e);
            return;
        }

        if (e.Item is not ToolStripSplitButton splitButton)
        {
            return;
        }

        Graphics graphics = e.Graphics;

        bool rightToLeft = splitButton.RightToLeft == RightToLeft.Yes;
        Color arrowColor = splitButton.Enabled
            ? SystemColors.ControlText
            : SystemColors.ControlDark;

        // In right to left - we need to swap the parts so we don't draw  v][ toolStripSplitButton.
        VisualStyleElement splitButtonDropDownPart = rightToLeft
            ? VisualStyleElement.ToolBar.SplitButton.Normal
            : VisualStyleElement.ToolBar.SplitButtonDropDown.Normal;

        VisualStyleElement splitButtonPart = rightToLeft
            ? VisualStyleElement.ToolBar.DropDownButton.Normal
            : VisualStyleElement.ToolBar.SplitButton.Normal;

        if (ToolStripManager.VisualStylesEnabled
            && VisualStyleRenderer.IsElementDefined(splitButtonDropDownPart)
            && VisualStyleRenderer.IsElementDefined(splitButtonPart))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;

            // Draw the SplitButton Button portion of it.
            vsRenderer.SetParameters(
                splitButtonPart.ClassName,
                splitButtonPart.Part,
                GetSplitButtonItemState(splitButton));

            Rectangle splitButtonBounds = splitButton.ButtonBounds;

            if (rightToLeft)
            {
                // Scoot to the left so we don't draw double shadow like so: ][
                splitButtonBounds.Inflate(2, 0);
            }

            // Draw the button portion of it.
            vsRenderer.DrawBackground(graphics, splitButtonBounds);

            // Draw the SplitButton DropDownButton portion of it.
            vsRenderer.SetParameters(
                splitButtonDropDownPart.ClassName,
                splitButtonDropDownPart.Part,
                GetSplitButtonDropDownItemState(splitButton));

            vsRenderer.DrawBackground(graphics, splitButton.DropDownButtonBounds);

            // Fill in the background image.
            Rectangle fillRect = splitButton.ContentRectangle;

            if (splitButton.BackgroundImage is not null)
            {
                ControlPaint.DrawBackgroundImage(
                    graphics,
                    splitButton.BackgroundImage,
                    splitButton.BackColor,
                    splitButton.BackgroundImageLayout,
                    fillRect,
                    fillRect);
            }

            // Draw the separator over it.
            RenderSeparatorInternal(
                graphics,
                splitButton,
                splitButton.SplitterBounds,
                vertical: true);

            // If we're in RTL or have a background image, paint the arrow.
            if (rightToLeft || splitButton.BackgroundImage is not null)
            {
                DrawArrow(new ToolStripArrowRenderEventArgs(
                    graphics,
                    splitButton,
                    splitButton.DropDownButtonBounds,
                    arrowColor,
                    ArrowDirection.Down));
            }

            ToolBarState state = GetToolBarState(e.Item);

            if (e.Item is ToolStripSplitButton item
                && !SystemInformation.HighContrast
                && (state == ToolBarState.Hot
                    || state == ToolBarState.Pressed
                    || state == ToolBarState.Checked))
            {
                Rectangle clientBounds = item.ClientBounds;

                using var highlightPen = SystemColors.Highlight.GetCachedPenScope();
                ControlPaint.DrawBorderSimple(graphics, clientBounds, SystemColors.Highlight);

                using var highlightBrush = SystemColors.Highlight.GetCachedSolidBrushScope();
                graphics.FillRectangle(highlightBrush, item.SplitterBounds);
            }

            return;
        }

        // Draw the split button button.
        Rectangle splitButtonButtonRect = splitButton.ButtonBounds;

        if (splitButton.BackgroundImage is not null)
        {
            Rectangle bounds = new(Point.Empty, splitButton.Size);

            // Fill in the background image.
            Rectangle fillRect = splitButton.Selected
                ? splitButton.ContentRectangle
                : bounds;

            ControlPaint.DrawBackgroundImage(
                g: graphics,
                backgroundImage: splitButton.BackgroundImage,
                backColor: splitButton.BackColor,
                backgroundImageLayout: splitButton.BackgroundImageLayout,
                bounds: bounds,
                clipRect: fillRect);
        }
        else
        {
            FillBackground(graphics, splitButtonButtonRect, splitButton.BackColor);
        }

        ToolBarState buttonState = GetSplitButtonToolBarState(splitButton, dropDownButton: false);

        RenderSmall3DBorderInternal(
            graphics: graphics,
            bounds: splitButtonButtonRect,
            state: buttonState,
            rightToLeft: rightToLeft);

        // Draw the split button drop down.
        Rectangle dropDownRect = splitButton.DropDownButtonBounds;

        // Fill the color in the dropdown button.
        if (splitButton.BackgroundImage is null)
        {
            FillBackground(graphics, dropDownRect, splitButton.BackColor);
        }

        ToolBarState dropDownState = GetSplitButtonToolBarState(splitButton, dropDownButton: true);

        if (dropDownState is ToolBarState.Pressed or ToolBarState.Hot)
        {
            RenderSmall3DBorderInternal(
                graphics: graphics,
                bounds: dropDownRect,
                state: dropDownState,
                rightToLeft: rightToLeft);
        }

        DrawArrow(new ToolStripArrowRenderEventArgs(
            g: graphics,
            toolStripItem: splitButton,
            arrowRectangle: dropDownRect,
            arrowColor: arrowColor,
            arrowDirection: ArrowDirection.Down));
    }

    /// <summary>
    ///  This method exists so that buttons, labels, and other items can share the same implementation.
    ///  If OnRenderButton called OnRenderItem, we would never be able to change the implementation
    ///  without causing a breaking change. For example, if in v1 a user overrode OnRenderItem to draw green triangles,
    ///  and in v2 we added a feature to buttons that required us to stop calling OnRenderItem,
    ///  the user's version of OnRenderItem would not be called when they upgraded their framework.
    ///  Therefore, everyone should call this private shared method. Users need to override each item they want
    ///  to change the look and feel of.
    /// </summary>
    private static void RenderItemInternal(ToolStripItemRenderEventArgs e)
    {
        ToolStripItem item = e.Item;
        Graphics graphics = e.Graphics;

        ToolBarState state = GetToolBarState(item);
        VisualStyleElement toolBarElement = VisualStyleElement.ToolBar.Button.Normal;

        if (ToolStripManager.VisualStylesEnabled
            && VisualStyleRenderer.IsElementDefined(toolBarElement))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(toolBarElement.ClassName, toolBarElement.Part, (int)state);
            vsRenderer.DrawBackground(graphics, new Rectangle(Point.Empty, item.Size));

            if (!SystemInformation.HighContrast &&
                (state == ToolBarState.Hot || state == ToolBarState.Pressed || state == ToolBarState.Checked))
            {
                var bounds = item.ClientBounds;
                bounds.Height -= 1;

                ControlPaint.DrawBorderSimple(graphics, bounds, SystemColors.Highlight);
            }
        }
        else
        {
            RenderSmall3DBorderInternal(graphics, new Rectangle(Point.Empty, item.Size), state, (item.RightToLeft == RightToLeft.Yes));
        }

        Rectangle fillRect = item.ContentRectangle;

        if (item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(graphics, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, fillRect, fillRect);
        }
        else
        {
            ToolStrip? parent = item.GetCurrentParent();
            if ((parent is not null) && (state != ToolBarState.Checked) && (item.BackColor != parent.BackColor))
            {
                FillBackground(graphics, fillRect, item.BackColor);
            }
        }
    }

    /// <summary>
    ///  Renders a separator for ToolStrip items
    /// </summary>
    private static void RenderSeparatorInternal(Graphics graphics, ToolStripItem item, Rectangle bounds, bool vertical)
    {
        VisualStyleElement separator = (vertical)
            ? VisualStyleElement.ToolBar.SeparatorHorizontal.Normal
            : VisualStyleElement.ToolBar.SeparatorVertical.Normal;

        if (ToolStripManager.VisualStylesEnabled
            && (VisualStyleRenderer.IsElementDefined(separator)))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(separator.ClassName, separator.Part, GetItemState(item));
            vsRenderer.DrawBackground(graphics, bounds);
        }
        else
        {
            using var foreColorPen = item.ForeColor.GetCachedPenScope();

            if (vertical)
            {
                if (bounds.Height >= 4)
                {
                    bounds.Inflate(0, -2);     // scoot down 2PX and start drawing
                }

                bool rightToLeft = (item.RightToLeft == RightToLeft.Yes);
                Pen leftPen = (rightToLeft) ? SystemPens.ButtonHighlight : foreColorPen;
                Pen rightPen = (rightToLeft) ? foreColorPen : SystemPens.ButtonHighlight;

                // Draw dark line
                int startX = bounds.Width / 2;
                graphics.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom);

                // Draw highlight one pixel to the right
                startX++;
                graphics.DrawLine(rightPen, startX, bounds.Top, startX, bounds.Bottom);
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
                graphics.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right, startY);

                // Draw highlight one pixel to the right
                startY++;
                using var highlightPen = SystemColors.ButtonHighlight.GetCachedPenScope();
                graphics.DrawLine(highlightPen, bounds.Left, startY, bounds.Right, startY);
            }
        }
    }

    private static void RenderSmall3DBorderInternal(Graphics graphics, Rectangle bounds, ToolBarState state, bool rightToLeft)
    {
        if (state is ToolBarState.Hot or ToolBarState.Pressed or ToolBarState.Checked)
        {
            Pen topPen = state == ToolBarState.Hot
                ? SystemPens.ButtonHighlight
                : SystemPens.ButtonShadow;

            Pen bottomPen = state == ToolBarState.Hot
                ? SystemPens.ButtonShadow
                : SystemPens.ButtonHighlight;

            Pen leftPen = rightToLeft
                ? bottomPen
                : topPen;

            Pen rightPen = rightToLeft
                ? topPen
                : bottomPen;

            graphics.DrawLine(topPen, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top);
            graphics.DrawLine(leftPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1);
            graphics.DrawLine(rightPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
            graphics.DrawLine(bottomPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
        }
    }

    private static void RenderStatusStripBorder(ToolStripRenderEventArgs e)
    {
        if (!Application.RenderWithVisualStyles)
        {
            using PenCache.Scope highlightPen = SystemColors.ButtonHighlight.GetCachedPenScope();
            e.Graphics.DrawLine(highlightPen, 0, 0, e.ToolStrip.Width, 0);
        }
    }

    private static void RenderStatusStripBackground(ToolStripRenderEventArgs e)
    {
        Graphics graphics = e.Graphics;

        if (Application.RenderWithVisualStyles)
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(VisualStyleElement.Status.Bar.Normal);
            vsRenderer.DrawBackground(graphics, new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1));
        }
        else
        {
            if (!SystemInformation.InLockedTerminalSession())
            {
                graphics.Clear(e.BackColor);
            }
        }
    }

    private static void RenderLabelInternal(ToolStripItemRenderEventArgs e)
    {
        // Do not call RenderItemInternal, as we NEVER want to paint hot.
        ToolStripItem item = e.Item;
        Graphics graphics = e.Graphics;

        Rectangle fillRect = item.ContentRectangle;

        if (item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(
                g: graphics,
                backgroundImage: item.BackgroundImage,
                backColor: item.BackColor,
                backgroundImageLayout: item.BackgroundImageLayout,
                bounds: fillRect,
                clipRect: fillRect);

            return;
        }

        VisualStyleRenderer? vsRenderer = VisualStyleRenderer;

        if (vsRenderer is null || item.BackColor != SystemColors.Control)
        {
            FillBackground(graphics, fillRect, item.BackColor);
        }
    }
}
