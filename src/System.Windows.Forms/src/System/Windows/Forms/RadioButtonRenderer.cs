// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

using System;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms.Internal;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

    /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This is a rendering class for the RadioButton control. It works downlevel too (obviously
    ///       without visual styles applied.)
    ///    </para>
    /// </devdoc>
    public sealed class RadioButtonRenderer {

       //Make this per-thread, so that different threads can safely use these methods.
       [ThreadStatic]
       private static VisualStyleRenderer visualStyleRenderer = null;
       private static readonly VisualStyleElement RadioElement = VisualStyleElement.Button.RadioButton.UncheckedNormal;
        private static bool renderMatchingApplicationState = true;

       //cannot instantiate
       private RadioButtonRenderer() {
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

       /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.IsBackgroundPartiallyTransparent"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Returns true if the background corresponding to the given state is partially transparent, else false.
       ///    </para>
       /// </devdoc>
       public static bool IsBackgroundPartiallyTransparent(RadioButtonState state) {
           if (RenderWithVisualStyles) {
               InitializeRenderer((int)state);
    
               return visualStyleRenderer.IsBackgroundPartiallyTransparent();
           }
           else {
               return false; //for downlevel, this is false
           }
       }

       /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.DrawParentBackground"]/*' />
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

        /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.DrawRadioButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a RadioButton control.
        ///    </para>
        /// </devdoc>
        public static void DrawRadioButton(Graphics g, Point glyphLocation, RadioButtonState state) {
            DrawRadioButton(g, glyphLocation, state, IntPtr.Zero);
        }

        internal static void DrawRadioButton(Graphics g, Point glyphLocation, RadioButtonState state, IntPtr hWnd) {
           Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(g, state, hWnd));

           if (RenderWithVisualStyles) {
               InitializeRenderer((int)state);

               visualStyleRenderer.DrawBackground(g, glyphBounds, hWnd);
           }
           else {
               ControlPaint.DrawRadioButton(g, glyphBounds, ConvertToButtonState(state));
           }
       }

       /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.DrawRadioButton1"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Renders a RadioButton control.
       ///    </para>
       /// </devdoc>
       public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, bool focused, RadioButtonState state) {
           DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine, 
                      focused, state);
       }

        /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.DrawRadioButton2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a RadioButton control.
        ///    </para>
        /// </devdoc>
        public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, TextFormatFlags flags, bool focused, RadioButtonState state) {
            DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font, flags, focused, state, IntPtr.Zero);
        }

        internal static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, TextFormatFlags flags, bool focused, RadioButtonState state, IntPtr hWnd) {
           Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(g, state, hWnd));
           Color textColor;

           if (RenderWithVisualStyles) {
               InitializeRenderer((int)state);
    
               visualStyleRenderer.DrawBackground(g, glyphBounds);
               textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
           }
           else {
               ControlPaint.DrawRadioButton(g, glyphBounds, ConvertToButtonState(state));
               textColor = SystemColors.ControlText;
           }
           
           TextRenderer.DrawText(g, radioButtonText, font, textBounds, textColor, flags);

           if (focused) {
               ControlPaint.DrawFocusRectangle(g, textBounds);
           }
       }

       /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.DrawRadioButton3"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Renders a RadioButton control.
       ///    </para>
       /// </devdoc>
       public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, Image image, Rectangle imageBounds, bool focused, RadioButtonState state) {
           DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine, 
                      image, imageBounds, focused, state);
       }



        /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.DrawRadioButton4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a RadioButton control.
        ///    </para>
        /// </devdoc>
        public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, RadioButtonState state) {
            DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font, flags, image, imageBounds, focused, state, IntPtr.Zero);
        }

        internal static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string radioButtonText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, RadioButtonState state, IntPtr hWnd) {
           Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(g, state, hWnd));
           Color textColor;

           if (RenderWithVisualStyles) {
               InitializeRenderer((int)state);
    
               //Keep this drawing order! It matches default drawing order.
               visualStyleRenderer.DrawImage(g, imageBounds, image);
               visualStyleRenderer.DrawBackground(g, glyphBounds);
               textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
           }
           else {
               g.DrawImage(image, imageBounds);
               ControlPaint.DrawRadioButton(g, glyphBounds, ConvertToButtonState(state));
               textColor = SystemColors.ControlText;
           }
           
           TextRenderer.DrawText(g, radioButtonText, font, textBounds, textColor, flags);

           if (focused) {
               ControlPaint.DrawFocusRectangle(g, textBounds);
           }
       }

       /// <include file='doc\RadioButtonRenderer.uex' path='docs/doc[@for="RadioButtonRenderer.GetGlyphSize"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Returns the size of the RadioButton glyph.
       ///    </para>
       /// </devdoc>
       [
           SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // Using Graphics instead of IDeviceContext intentionally
       ]
       public static Size GetGlyphSize(Graphics g, RadioButtonState state) {
           return GetGlyphSize(g, state, IntPtr.Zero); 
       }

       internal static Size GetGlyphSize(Graphics g, RadioButtonState state, IntPtr hWnd) {
           if (RenderWithVisualStyles) {
               InitializeRenderer((int)state);
    
               return visualStyleRenderer.GetPartSize(g, ThemeSizeType.Draw, hWnd);
           }

           return new Size(13, 13); 
       }

       internal static ButtonState ConvertToButtonState(RadioButtonState state) {
           switch (state) {
           case RadioButtonState.CheckedNormal:
           case RadioButtonState.CheckedHot:
               return ButtonState.Checked;
           case RadioButtonState.CheckedPressed:
               return (ButtonState.Checked | ButtonState.Pushed);
           case RadioButtonState.CheckedDisabled:
               return (ButtonState.Checked | ButtonState.Inactive);

           case RadioButtonState.UncheckedPressed:
               return ButtonState.Pushed;
           case RadioButtonState.UncheckedDisabled:
               return ButtonState.Inactive;

           default:
               return ButtonState.Normal;
           }
       }

       internal static RadioButtonState ConvertFromButtonState(ButtonState state, bool isHot) {
           if ((state & ButtonState.Checked) == ButtonState.Checked) {
               if ((state & ButtonState.Pushed) == ButtonState.Pushed) {
                   return RadioButtonState.CheckedPressed;
               }
               else if ((state & ButtonState.Inactive) == ButtonState.Inactive) {
                   return RadioButtonState.CheckedDisabled;
               }
               else if (isHot) {
                   return RadioButtonState.CheckedHot;
               }

               return RadioButtonState.CheckedNormal;
           }
           else { //unchecked
               if ((state & ButtonState.Pushed) == ButtonState.Pushed) {
                   return RadioButtonState.UncheckedPressed;
               }
               else if ((state & ButtonState.Inactive) == ButtonState.Inactive) {
                   return RadioButtonState.UncheckedDisabled;
               }
               else if (isHot) {
                   return RadioButtonState.UncheckedHot;
               }

               return RadioButtonState.UncheckedNormal;
           }
       }

        private static void InitializeRenderer(int state) {
            RadioButtonState radioButtonState = (RadioButtonState)state;
            int part = RadioElement.Part;
            if (AccessibilityImprovements.Level2
                && SystemInformation.HighContrast 
                && (radioButtonState == RadioButtonState.CheckedDisabled || radioButtonState == RadioButtonState.UncheckedDisabled)
                && VisualStyleRenderer.IsCombinationDefined(RadioElement.ClassName, VisualStyleElement.Button.RadioButton.HighContrastDisabledPart)) {
                    part = VisualStyleElement.Button.RadioButton.HighContrastDisabledPart;
            }

            if (visualStyleRenderer == null) {
                visualStyleRenderer = new VisualStyleRenderer(RadioElement.ClassName, part, state);
            }
            else {
                visualStyleRenderer.SetParameters(RadioElement.ClassName, part, state);
            }
        }
    }
}
