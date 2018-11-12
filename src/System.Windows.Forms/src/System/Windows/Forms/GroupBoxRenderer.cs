// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms { 

using System;
using System.Drawing;
using System.Windows.Forms.Internal;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;


    /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This is a rendering class for the GroupBox control.
    ///    </para>
    /// </devdoc>
    public sealed class GroupBoxRenderer {

        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;
        private static readonly VisualStyleElement GroupBoxElement = VisualStyleElement.Button.GroupBox.Normal;
        private const int textOffset = 8;        //MAGIC NUMBER - WHERE DID IT COME FROM?
        private const int boxHeaderWidth = 7;    // The groupbox frame shows 7 pixels before the caption.
        private static bool renderMatchingApplicationState = true;
        //cannot instantiate
        private GroupBoxRenderer() {
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.RenderMatchingApplicationState"]/*' />
        /// <devdoc>
        ///    <para>
        ///      If this property is true, then the renderer will use the setting from Application.RenderWithVisualStyles to 
        /// determine how to render.
        ///      If this property is false, the renderer will always render with visualstyles.
        ///    </para>
        /// </devdoc>
        public static bool RenderMatchingApplicationState {
            get {
                return renderMatchingApplicationState;
            }
            set {
                renderMatchingApplicationState = value;
            }
        }

        private static bool RenderWithVisualStyles {
            get {
                return (!renderMatchingApplicationState || Application.RenderWithVisualStyles);
            }
        }

        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.IsBackgroundPartiallyTransparent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns true if the background corresponding to the given state is partially transparent, else false.
        ///    </para>
        /// </devdoc>
        public static bool IsBackgroundPartiallyTransparent(GroupBoxState state) {
           if (RenderWithVisualStyles) {
               InitializeRenderer((int)state);
               return visualStyleRenderer.IsBackgroundPartiallyTransparent();
           }
           else {
               return false; //for downlevel, this is false
           }
        }
        
        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.DrawParentBackground"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This is just a convenience wrapper for VisualStyleRenderer.DrawThemeParentBackground. For downlevel,
        ///       this isn't required and does nothing.
        ///    </para>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // Using Graphics instead of IDeviceContext intentionally
        ]
        public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl) {
           if (RenderWithVisualStyles) {
               InitializeRenderer(0);
               visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
           }
        }

        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.DrawGroupBox"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a GroupBox control.
        ///    </para>
        /// </devdoc>
        public static void DrawGroupBox(Graphics g, Rectangle bounds, GroupBoxState state) {
            if (RenderWithVisualStyles)
                DrawThemedGroupBoxNoText(g, bounds, state);
            else
                DrawUnthemedGroupBoxNoText(g, bounds, state);
        }

        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.DrawGroupBox1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a GroupBox control. Uses the text color specified by the theme.
        ///    </para>
        /// </devdoc>
        public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, GroupBoxState state) {
            DrawGroupBox(g, bounds, groupBoxText, font, TextFormatFlags.Top | TextFormatFlags.Left, state);
        }

        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.DrawGroupBox2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a GroupBox control.
        ///    </para>
        /// </devdoc>
        public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, GroupBoxState state) {
            DrawGroupBox(g, bounds, groupBoxText, font, textColor, TextFormatFlags.Top | TextFormatFlags.Left, state);
        }

        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.DrawGroupBox3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a GroupBox control. Uses the text color specified by the theme.
        ///    </para>
        /// </devdoc>
        public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, TextFormatFlags flags, GroupBoxState state) {
            if (RenderWithVisualStyles)
                DrawThemedGroupBoxWithText(g, bounds, groupBoxText, font, DefaultTextColor(state), flags, state);
            else
                DrawUnthemedGroupBoxWithText(g, bounds, groupBoxText, font, DefaultTextColor(state), flags, state);
        }

        /// <include file='doc\GroupBoxRenderer.uex' path='docs/doc[@for="GroupBoxRenderer.DrawGroupBox4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a GroupBox control.
        ///    </para>
        /// </devdoc>
        public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, TextFormatFlags flags, GroupBoxState state) {
            if (RenderWithVisualStyles)
                DrawThemedGroupBoxWithText(g, bounds, groupBoxText, font, textColor, flags, state);
            else
                DrawUnthemedGroupBoxWithText(g, bounds, groupBoxText, font, textColor, flags, state);
        }

        /// <summary>
        ///     Draws a themed GroupBox with no text label.
        /// </summary>
        /// <internalonly/>
        private static void DrawThemedGroupBoxNoText(Graphics g, Rectangle bounds, GroupBoxState state) {
            InitializeRenderer((int)state);
            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///     Draws a themed GroupBox with a text label.
        /// </summary>
        /// <internalonly/>
        private static void DrawThemedGroupBoxWithText(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, TextFormatFlags flags, GroupBoxState state) {
            InitializeRenderer((int)state);

            // Calculate text area, and render text inside it
            Rectangle textBounds = bounds;

            textBounds.Width -= 2 * boxHeaderWidth;
            Size measuredBounds = TextRenderer.MeasureText(g, groupBoxText, font, new Size(textBounds.Width, textBounds.Height), flags);
            textBounds.Width = measuredBounds.Width;
            textBounds.Height = measuredBounds.Height;

            if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right) {
                textBounds.X = bounds.Right - textBounds.Width - boxHeaderWidth + 1;  // +1 to account for the margin built in the MeasureText result
            }
            else {
                textBounds.X += boxHeaderWidth - 1;                                   // -1 to account for the margin built in the MeasureText result
            }

            TextRenderer.DrawText(g, groupBoxText, font, textBounds, textColor, flags);

            // Calculate area for background box
            Rectangle boxBounds = bounds;
            boxBounds.Y += font.Height / 2;
            boxBounds.Height -= font.Height / 2;

            // Break box into three segments, that don't overlap the text area
            Rectangle clipLeft   = boxBounds;
            Rectangle clipMiddle = boxBounds;
            Rectangle clipRight  = boxBounds;

            clipLeft.Width = boxHeaderWidth;
            clipMiddle.Width = Math.Max(0, textBounds.Width - 3);  // -3 to account for the margin built in the MeasureText result
            if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
            {
                clipLeft.X = boxBounds.Right - boxHeaderWidth;
                clipMiddle.X = clipLeft.Left - clipMiddle.Width;
                clipRight.Width = clipMiddle.X - boxBounds.X;
            }
            else
            {
                clipMiddle.X = clipLeft.Right;
                clipRight.X = clipMiddle.Right;
                clipRight.Width = boxBounds.Right - clipRight.X;
            }
            clipMiddle.Y = textBounds.Bottom;
            clipMiddle.Height -= (textBounds.Bottom - boxBounds.Top);
            
            Debug.Assert(textBounds.Y <= boxBounds.Y, "if text below box, need to render area of box above text");

            // Render clipped portion of background in each segment
            visualStyleRenderer.DrawBackground(g, boxBounds, clipLeft);
            visualStyleRenderer.DrawBackground(g, boxBounds, clipMiddle);
            visualStyleRenderer.DrawBackground(g, boxBounds, clipRight);
        }

        /// <summary>
        ///     Draws an un-themed GroupBox with no text label.
        /// </summary>
        /// <internalonly/>
        private static void DrawUnthemedGroupBoxNoText(Graphics g, Rectangle bounds, GroupBoxState state) {
            Color backColor = SystemColors.Control;         
            Pen light = new Pen(ControlPaint.Light(backColor, 1.0f));
            Pen dark = new Pen(ControlPaint.Dark(backColor, 0f));
            try {
                // left
                g.DrawLine(light, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Height - 1);
                g.DrawLine(dark, bounds.Left, bounds.Top + 1, bounds.Left, bounds.Height - 2);

                // bottom
                g.DrawLine(light, bounds.Left, bounds.Height - 1, bounds.Width - 1, bounds.Height - 1);
                g.DrawLine(dark, bounds.Left, bounds.Height - 2, bounds.Width - 1, bounds.Height - 2);

                // top
                g.DrawLine(light, bounds.Left + 1, bounds.Top + 1, bounds.Width - 1, bounds.Top + 1);
                g.DrawLine(dark, bounds.Left, bounds.Top, bounds.Width - 2, bounds.Top);
                
                // right
                g.DrawLine(light, bounds.Width - 1, bounds.Top, bounds.Width - 1, bounds.Height - 1);
                g.DrawLine(dark, bounds.Width - 2, bounds.Top, bounds.Width - 2, bounds.Height - 2);
            }
            finally {
                if (light != null) {
                    light.Dispose();
                }
                if (dark != null) {
                    dark.Dispose();
                }
            }
        }

        /// <summary>
        ///     Draws an un-themed GroupBox with a text label.
        ///     Variation of the logic in GroupBox.DrawGroupBox().
        /// </summary>
        /// <internalonly/>
        private static void DrawUnthemedGroupBoxWithText(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, TextFormatFlags flags, GroupBoxState state) {
            // Calculate text area, and render text inside it
            Rectangle textBounds = bounds;

            textBounds.Width -= textOffset;
            Size measuredBounds = TextRenderer.MeasureText(g, groupBoxText, font, new Size(textBounds.Width, textBounds.Height), flags);
            textBounds.Width = measuredBounds.Width;
            textBounds.Height = measuredBounds.Height;

            if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right) {
                textBounds.X = bounds.Right - textBounds.Width - textOffset;
            }
            else {
                textBounds.X += textOffset;
            }

            TextRenderer.DrawText(g, groupBoxText, font, textBounds, textColor, flags);

            // Pad text area to stop background from touching text
            if (textBounds.Width > 0)
                textBounds.Inflate(2, 0);

            Pen light = new Pen(SystemColors.ControlLight);
            Pen dark = new Pen(SystemColors.ControlDark);

            int boxTop = bounds.Top + font.Height / 2;
            
            // left
            g.DrawLine(light, bounds.Left + 1, boxTop, bounds.Left + 1, bounds.Height - 1);
            g.DrawLine(dark, bounds.Left, boxTop - 1, bounds.Left, bounds.Height - 2);

            // bottom
            g.DrawLine(light, bounds.Left, bounds.Height - 1, bounds.Width, bounds.Height - 1);
            g.DrawLine(dark, bounds.Left, bounds.Height - 2, bounds.Width - 1, bounds.Height - 2);

            // top-left
            g.DrawLine(light, bounds.Left + 1, boxTop, textBounds.X - 2, boxTop);
            g.DrawLine(dark, bounds.Left, boxTop - 1, textBounds.X - 3, boxTop - 1);
            
            // top-right
            g.DrawLine(light, textBounds.X + textBounds.Width + 1, boxTop, bounds.Width - 1, boxTop);
            g.DrawLine(dark, textBounds.X + textBounds.Width + 2, boxTop - 1, bounds.Width - 2, boxTop - 1);
            
            // right
            g.DrawLine(light, bounds.Width - 1, boxTop, bounds.Width - 1, bounds.Height - 1);
            g.DrawLine(dark, bounds.Width - 2, boxTop - 1, bounds.Width - 2, bounds.Height - 2);

            light.Dispose();
            dark.Dispose();
        }

        private static Color DefaultTextColor(GroupBoxState state) {
            if (RenderWithVisualStyles) {
                InitializeRenderer((int)state);
                return visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else {
                return SystemColors.ControlText;
            }
        }

        private static void InitializeRenderer(int state) {
            int part = GroupBoxElement.Part;
            if (AccessibilityImprovements.Level2
                && SystemInformation.HighContrast
                && ((GroupBoxState)state == GroupBoxState.Disabled)
                && VisualStyleRenderer.IsCombinationDefined(GroupBoxElement.ClassName, VisualStyleElement.Button.GroupBox.HighContrastDisabledPart)) {
                    part = VisualStyleElement.Button.GroupBox.HighContrastDisabledPart;
            }

            if (visualStyleRenderer == null) {
                visualStyleRenderer = new VisualStyleRenderer(GroupBoxElement.ClassName, part, state);
            }
            else {
                visualStyleRenderer.SetParameters(GroupBoxElement.ClassName, part, state);
            }
        }
    }
}
