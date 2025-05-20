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
            // First, check for high contrast mode (accessibility takes precedence)
            if (DisplayInformation.HighContrast)
            {
                return HighContrastRenderer;
            }

            // Then check for dark mode
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (Application.IsDarkModeEnabled)
            {
                return DarkModeRenderer;
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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
    ///  Fill the item's background as bounded by the rectangle
    /// </summary>
    private static void FillBackground(Graphics g, Rectangle bounds, Color backColor)
    {
        // Fill the background with the item's back color
        using var backBrush = backColor.GetCachedSolidBrushScope();
        g.FillRectangle(backBrush, bounds);
    }

    /// <summary>
    ///  translates the ToolStrip item state into a toolbar state, which is something the renderer understands
    /// </summary>
    private static int GetItemState(ToolStripItem item)
    {
        return (int)GetToolBarState(item);
    }

    /// <summary>
    ///  translates the ToolStrip item state into a toolbar state, which is something the renderer understands
    /// </summary>
    private static int GetSplitButtonDropDownItemState(ToolStripSplitButton? item)
    {
        return (int)GetSplitButtonToolBarState(item, true);
    }

    /// <summary>
    ///  translates the ToolStrip item state into a toolbar state, which is something the renderer understands
    /// </summary>
    private static int GetSplitButtonItemState(ToolStripSplitButton? item)
    {
        return (int)GetSplitButtonToolBarState(item, false);
    }

    /// <summary>
    ///  translates the ToolStrip item state into a toolbar state, which is something the renderer understands
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
    ///  translates the ToolStrip item state into a toolbar state, which is something the renderer understands
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
            if (toolStripButton.Selected)
            {
                state = ToolBarState.Hot; // we'd prefer HotChecked here, but Color Theme uses the same color as Checked
            }
            else
            {
                state = ToolBarState.Checked;
            }
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
        // It is not entirely clear, why we're not leaving it to the respective renderers
        // what to do here, but for keeping as much as possible of the original behavior,
        // we need to check if the renderer override is set, test for particular renderer types
        // and then decide what to do.
        if (RendererOverride is ToolStripSystemDarkModeRenderer darkModeRenderer
            && darkModeRenderer.IsSystemDefaultAlternative)
        {
            base.OnRenderToolStripBackground(e);

            return;
        }

        ToolStrip toolStrip = e.ToolStrip;
        Graphics g = e.Graphics;
        Rectangle bounds = e.AffectedBounds;

        if (!ShouldPaintBackground(toolStrip))
        {
            return;
        }

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
                FillBackground(g, bounds, (toolStrip is ToolStripDropDown)
                     ? SystemColors.ControlLight
                     : e.BackColor);
            }
            else if (toolStrip.IsDropDown)
            {
                FillBackground(g, bounds, ToolStripManager.VisualStylesEnabled
                    ? SystemColors.Menu
                    : e.BackColor);
            }
            else if (toolStrip is MenuStrip)
            {
                FillBackground(g, bounds, ToolStripManager.VisualStylesEnabled
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
                FillBackground(g, bounds, ToolStripManager.VisualStylesEnabled
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

        if (toolStrip is StatusStrip)
        {
            RenderStatusStripBorder(e);
        }
        else if (toolStrip is ToolStripDropDown toolStripDropDown)
        {
            // Paint the border for the window depending on whether or not we have a drop shadow effect.
            if (toolStripDropDown.DropShadowEnabled && ToolStripManager.VisualStylesEnabled)
            {
                bounds.Width -= 1;
                bounds.Height -= 1;
                e.Graphics.DrawRectangle(SystemPens.ControlDark, bounds);
            }
            else
            {
                ControlPaint.DrawBorder3D(e.Graphics, bounds, Border3DStyle.Raised);
            }
        }
        else
        {
            if (ToolStripManager.VisualStylesEnabled)
            {
                e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
                e.Graphics.DrawLine(SystemPens.InactiveBorder, 0, bounds.Bottom - 2, bounds.Width, bounds.Bottom - 2);
            }
            else
            {
                e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
                e.Graphics.DrawLine(SystemPens.ButtonShadow, 0, bounds.Bottom - 2, bounds.Width, bounds.Bottom - 2);
            }
        }
    }

    /// <summary>
    ///  Draw the grip. ToolStrip users should override this if they want to draw differently.
    /// </summary>
    protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
    {
        Graphics g = e.Graphics;
        Rectangle bounds = new(Point.Empty, e.GripBounds.Size);
        bool verticalGrip = e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical;

        if (ToolStripManager.VisualStylesEnabled
            && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Gripper.Normal))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;

            if (verticalGrip)
            {
                vsRenderer.SetParameters(VisualStyleElement.Rebar.Gripper.Normal);

                bounds.Height = ((bounds.Height - 2/*number of pixels for border*/) / 4) * 4; // make sure height is an even interval of 4.
                bounds.Y = Math.Max(0, (e.GripBounds.Height - bounds.Height - 2/*number of pixels for border*/) / 2);
            }
            else
            {
                vsRenderer.SetParameters(VisualStyleElement.Rebar.GripperVertical.Normal);
            }

            vsRenderer.DrawBackground(g, bounds);
        }
        else
        {
            // do some fixup so that we don't paint from end to end.
            Color backColor = e.ToolStrip.BackColor;
            FillBackground(g, bounds, backColor);

            if (verticalGrip)
            {
                if (bounds.Height >= 4)
                {
                    bounds.Inflate(0, -2);     // scoot down 2PX and start drawing
                }

                bounds.Width = 3;
            }
            else
            {
                if (bounds.Width >= 4)
                {
                    bounds.Inflate(-2, 0);        // scoot over 2PX and start drawing
                }

                bounds.Height = 3;
            }

            RenderSmall3DBorderInternal(g, bounds, ToolBarState.Hot, e.ToolStrip.RightToLeft == RightToLeft.Yes);
        }
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
        Graphics g = e.Graphics;

        if (ToolStripManager.VisualStylesEnabled && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Chevron.Normal))
        {
            VisualStyleElement chevronElement = VisualStyleElement.Rebar.Chevron.Normal;
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(chevronElement.ClassName, chevronElement.Part, GetItemState(item));
            vsRenderer.DrawBackground(g, new Rectangle(Point.Empty, item.Size));
        }
        else
        {
            RenderItemInternal(e);
            Color arrowColor = item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
            DrawArrow(new ToolStripArrowRenderEventArgs(g, item, new Rectangle(Point.Empty, item.Size), arrowColor, ArrowDirection.Down));
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
        Graphics g = e.Graphics;

        if (item is MdiControlStrip.SystemMenuItem)
        {
            return; // no highlights are painted behind a system menu item
        }

        if (item is not null)
        {
            Rectangle bounds = new(Point.Empty, item.Size);
            if (item.IsTopLevel && !ToolStripManager.VisualStylesEnabled)
            {
                // Classic Mode (3D edges)
                // Draw box highlight for toplevel items in downlevel platforms
                if (item.BackgroundImage is not null)
                {
                    ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, item.ContentRectangle, item.ContentRectangle);
                }
                else if (item.RawBackColor != Color.Empty)
                {
                    FillBackground(g, item.ContentRectangle, item.BackColor);
                }

                // Toplevel menu items do 3D borders.
                ToolBarState state = GetToolBarState(item);
                RenderSmall3DBorderInternal(g, bounds, state, (item.RightToLeft == RightToLeft.Yes));
            }
            else
            {
                // Modern MODE (no 3D edges)
                // Draw blue filled highlight for toplevel items in themed platforms
                // or items parented to a drop down
                Rectangle fillRect = new(Point.Empty, item.Size);
                if (item.IsOnDropDown)
                {
                    // Scoot in by 2 pixels when selected
                    fillRect.X += 2;
                    fillRect.Width -= 3; // its already 1 away from the right edge
                }

                if (item.Selected || item.Pressed)
                {
                    // Legacy behavior is to always paint the menu item background. The correct behavior is to only
                    // paint the background if the menu item is enabled.
                    if (item.Enabled)
                    {
                        g.FillRectangle(SystemBrushes.Highlight, fillRect);
                    }

                    Color borderColor = ToolStripManager.VisualStylesEnabled
                        ? SystemColors.Highlight
                        : ProfessionalColors.MenuItemBorder;

                    // Draw selection border - always drawn regardless of Enabled.
                    using var pen = borderColor.GetCachedPenScope();
                    g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
                else
                {
                    if (item.BackgroundImage is not null)
                    {
                        ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, item.ContentRectangle, fillRect);
                    }
                    else if (!ToolStripManager.VisualStylesEnabled && (item.RawBackColor != Color.Empty))
                    {
                        FillBackground(g, fillRect, item.BackColor);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Draws a toolbar separator. ToolStrip users should override this function to change the
    ///  drawing of all separators.
    /// </summary>
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        RenderSeparatorInternal(e.Graphics, e.Item, new Rectangle(Point.Empty, e.Item.Size), e.Vertical);
    }

    protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
    {
        RenderLabelInternal(e);
        ToolStripStatusLabel? item = e.Item as ToolStripStatusLabel;
        if (item is not null)
        {
            ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0, 0, item.Width - 1, item.Height - 1), item.BorderStyle, (Border3DSide)item.BorderSides);
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

        Graphics g = e.Graphics;

        bool rightToLeft = splitButton.RightToLeft == RightToLeft.Yes;
        Color arrowColor = splitButton.Enabled
            ? SystemColors.ControlText
            : SystemColors.ControlDark;

        // in right to left - we need to swap the parts so we don't draw  v][ toolStripSplitButton
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
            vsRenderer.SetParameters(splitButtonPart.ClassName, splitButtonPart.Part, GetSplitButtonItemState(splitButton));

            // the Windows theming for split button comes in three pieces:
            //  SplitButtonDropDown: [ v |
            //  Separator:               |
            //  SplitButton:             |  ]
            // this is great except if you want to swap the button in RTL. In this case we need
            // to use the DropDownButton instead of the SplitButtonDropDown and paint the arrow ourselves.
            Rectangle splitButtonBounds = splitButton.ButtonBounds;
            if (rightToLeft)
            {
                // scoot to the left so we don't draw double shadow like so: ][
                splitButtonBounds.Inflate(2, 0);
            }

            // Draw the button portion of it.
            vsRenderer.DrawBackground(g, splitButtonBounds);

            // Draw the SplitButton DropDownButton portion of it.
            vsRenderer.SetParameters(splitButtonDropDownPart.ClassName, splitButtonDropDownPart.Part, GetSplitButtonDropDownItemState(splitButton));

            // Draw the drop down button portion
            vsRenderer.DrawBackground(g, splitButton.DropDownButtonBounds);

            // fill in the background image
            Rectangle fillRect = splitButton.ContentRectangle;
            if (splitButton.BackgroundImage is not null)
            {
                ControlPaint.DrawBackgroundImage(g, splitButton.BackgroundImage, splitButton.BackColor, splitButton.BackgroundImageLayout, fillRect, fillRect);
            }

            // draw the separator over it.
            RenderSeparatorInternal(g, splitButton, splitButton.SplitterBounds, true);

            // and of course, now if we're in RTL we now need to paint the arrow
            // because we're no longer using a part that has it built in.
            if (rightToLeft || splitButton.BackgroundImage is not null)
            {
                DrawArrow(new ToolStripArrowRenderEventArgs(g, splitButton, splitButton.DropDownButtonBounds, arrowColor, ArrowDirection.Down));
            }

            ToolBarState state = GetToolBarState(e.Item);
            if (e.Item is ToolStripSplitButton item && !SystemInformation.HighContrast &&
                (state == ToolBarState.Hot || state == ToolBarState.Pressed || state == ToolBarState.Checked))
            {
                var clientBounds = item.ClientBounds;
                ControlPaint.DrawBorderSimple(g, clientBounds, SystemColors.Highlight);
                using var brush = SystemColors.Highlight.GetCachedSolidBrushScope();
                g.FillRectangle(brush, item.SplitterBounds);
            }
        }
        else
        {
            // Draw the split button button
            Rectangle splitButtonButtonRect = splitButton.ButtonBounds;

            if (splitButton.BackgroundImage is not null)
            {
                Rectangle bounds = new(Point.Empty, splitButton.Size);

                // fill in the background image
                Rectangle fillRect = splitButton.Selected ? splitButton.ContentRectangle : bounds;
                if (splitButton.BackgroundImage is not null)
                {
                    ControlPaint.DrawBackgroundImage(g, splitButton.BackgroundImage, splitButton.BackColor, splitButton.BackgroundImageLayout, bounds, fillRect);
                }
            }
            else
            {
                FillBackground(g, splitButtonButtonRect, splitButton.BackColor);
            }

            ToolBarState state = GetSplitButtonToolBarState(splitButton, false);

            RenderSmall3DBorderInternal(g, splitButtonButtonRect, state, rightToLeft);

            // draw the split button drop down
            Rectangle dropDownRect = splitButton.DropDownButtonBounds;

            // fill the color in the dropdown button
            if (splitButton.BackgroundImage is null)
            {
                FillBackground(g, dropDownRect, splitButton.BackColor);
            }

            state = GetSplitButtonToolBarState(splitButton, true);

            if (state is ToolBarState.Pressed or ToolBarState.Hot)
            {
                RenderSmall3DBorderInternal(g, dropDownRect, state, rightToLeft);
            }

            DrawArrow(new ToolStripArrowRenderEventArgs(g, splitButton, dropDownRect, arrowColor, ArrowDirection.Down));
        }
    }

    /// <summary>
    ///  This exists mainly so that buttons, labels and items, etc can share the same implementation.
    ///  If OnRenderButton called OnRenderItem we would never be able to change the implementation
    ///  as it would be a breaking change. If in v1, the user overrode OnRenderItem to draw green triangles
    ///  and in v2 we decided to add a feature to button that would require us to no longer call OnRenderItem -
    ///  the user's version of OnRenderItem would not get called when he upgraded his framework. Hence
    ///  everyone should just call this private shared method. Users need to override each item they want
    ///  to change the look and feel of.
    /// </summary>
    private static void RenderItemInternal(ToolStripItemRenderEventArgs e)
    {
        ToolStripItem item = e.Item;
        Graphics g = e.Graphics;

        ToolBarState state = GetToolBarState(item);
        VisualStyleElement toolBarElement = VisualStyleElement.ToolBar.Button.Normal;

        if (ToolStripManager.VisualStylesEnabled
            && VisualStyleRenderer.IsElementDefined(toolBarElement))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(toolBarElement.ClassName, toolBarElement.Part, (int)state);
            vsRenderer.DrawBackground(g, new Rectangle(Point.Empty, item.Size));

            if (!SystemInformation.HighContrast &&
                (state == ToolBarState.Hot || state == ToolBarState.Pressed || state == ToolBarState.Checked))
            {
                var bounds = item.ClientBounds;
                bounds.Height -= 1;
                ControlPaint.DrawBorderSimple(g, bounds, SystemColors.Highlight);
            }
        }
        else
        {
            RenderSmall3DBorderInternal(g, new Rectangle(Point.Empty, item.Size), state, (item.RightToLeft == RightToLeft.Yes));
        }

        Rectangle fillRect = item.ContentRectangle;

        if (item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, fillRect, fillRect);
        }
        else
        {
            ToolStrip? parent = item.GetCurrentParent();
            if ((parent is not null) && (state != ToolBarState.Checked) && (item.BackColor != parent.BackColor))
            {
                FillBackground(g, fillRect, item.BackColor);
            }
        }
    }

    /// <summary>
    /// </summary>
    private static void RenderSeparatorInternal(Graphics g, ToolStripItem item, Rectangle bounds, bool vertical)
    {
        VisualStyleElement separator = (vertical)
            ? VisualStyleElement.ToolBar.SeparatorHorizontal.Normal
            : VisualStyleElement.ToolBar.SeparatorVertical.Normal;

        if (ToolStripManager.VisualStylesEnabled
            && (VisualStyleRenderer.IsElementDefined(separator)))
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(separator.ClassName, separator.Part, GetItemState(item));
            vsRenderer.DrawBackground(g, bounds);
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
                g.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom);

                // Draw highlight one pixel to the right
                startX++;
                g.DrawLine(rightPen, startX, bounds.Top, startX, bounds.Bottom);
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
                g.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right, startY);

                // Draw highlight one pixel to the right
                startY++;
                g.DrawLine(SystemPens.ButtonHighlight, bounds.Left, startY, bounds.Right, startY);
            }
        }
    }

    private static void RenderSmall3DBorderInternal(Graphics g, Rectangle bounds, ToolBarState state, bool rightToLeft)
    {
        if (state is ToolBarState.Hot or ToolBarState.Pressed or ToolBarState.Checked)
        {
            Pen leftPen, topPen, rightPen, bottomPen;
            topPen = (state == ToolBarState.Hot) ? SystemPens.ButtonHighlight : SystemPens.ButtonShadow;
            bottomPen = (state == ToolBarState.Hot) ? SystemPens.ButtonShadow : SystemPens.ButtonHighlight;

            leftPen = (rightToLeft) ? bottomPen : topPen;
            rightPen = (rightToLeft) ? topPen : bottomPen;

            g.DrawLine(topPen, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top);
            g.DrawLine(leftPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1);
            g.DrawLine(rightPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
            g.DrawLine(bottomPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
        }
    }

    private static void RenderStatusStripBorder(ToolStripRenderEventArgs e)
    {
        if (!Application.RenderWithVisualStyles)
        {
            e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, 0, e.ToolStrip.Width, 0);
        }
    }

    private static void RenderStatusStripBackground(ToolStripRenderEventArgs e)
    {
        if (Application.RenderWithVisualStyles)
        {
            VisualStyleRenderer vsRenderer = VisualStyleRenderer!;
            vsRenderer.SetParameters(VisualStyleElement.Status.Bar.Normal);
            vsRenderer.DrawBackground(e.Graphics, new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1));
        }
        else
        {
            if (!SystemInformation.InLockedTerminalSession())
            {
                e.Graphics.Clear(e.BackColor);
            }
        }
    }

    private static void RenderLabelInternal(ToolStripItemRenderEventArgs e)
    {
        // don't call RenderItemInternal, as we NEVER want to paint hot.
        ToolStripItem item = e.Item;
        Graphics g = e.Graphics;

        Rectangle fillRect = item.ContentRectangle;

        if (item.BackgroundImage is not null)
        {
            ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, fillRect, fillRect);
        }
        else
        {
            VisualStyleRenderer? vsRenderer = VisualStyleRenderer;

            if (vsRenderer is null || (item.BackColor != SystemColors.Control))
            {
                FillBackground(g, fillRect, item.BackColor);
            }
        }
    }
}
