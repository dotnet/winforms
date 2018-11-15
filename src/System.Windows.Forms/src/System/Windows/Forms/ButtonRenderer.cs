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

    /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This is a rendering class for the Button control. It works downlevel too (obviously
    ///       without visual styles applied.)
    ///    </para>
    /// </devdoc>
    public sealed class ButtonRenderer {

        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;
        private static readonly VisualStyleElement ButtonElement = VisualStyleElement.Button.PushButton.Normal;
        private static bool renderMatchingApplicationState = true;

        //cannot instantiate
        private ButtonRenderer() {
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

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.IsBackgroundPartiallyTransparent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns true if the background corresponding to the given state is partially transparent, else false.
        ///    </para>
        /// </devdoc>
        public static bool IsBackgroundPartiallyTransparent(PushButtonState state) {
            if (RenderWithVisualStyles) {
                InitializeRenderer((int)state);

                return visualStyleRenderer.IsBackgroundPartiallyTransparent();
            }
            else {
                return false; //for downlevel, this is false
            }
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawParentBackground"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This is just a convenience wrapper for VisualStyleRenderer.DrawThemeParentBackground. For downlevel,
        ///       this isn't required and does nothing.
        ///    </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl) {
            if (RenderWithVisualStyles) {
                InitializeRenderer(0);
                visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
            }
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, PushButtonState state) {
                if (RenderWithVisualStyles) {
                    InitializeRenderer((int)state);

                    visualStyleRenderer.DrawBackground(g, bounds);
                }
                else {
                    ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                }
        }

        /// <summary>
        /// Method to draw visualstyle themes in case of per-monitor scenarios where Hwnd is necessary
        /// </summary>
        /// <param name="g"> graphics object</param>
        /// <param name="bounds"> button bounds</param>
        /// <param name="focused"> is focused?</param>
        /// <param name="state"> state</param>
        /// <param name="handle"> handle to the control</param>
        internal static void DrawButtonForHandle(Graphics g, Rectangle bounds, bool focused, PushButtonState state, IntPtr handle) {
            Rectangle contentBounds;

            if (RenderWithVisualStyles) {
                InitializeRenderer((int)state);

                visualStyleRenderer.DrawBackground(g, bounds, handle);
                contentBounds = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            }
            else {
                ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                contentBounds = Rectangle.Inflate(bounds, -3, -3);
            }

            if (focused) {
                ControlPaint.DrawFocusRectangle(g, contentBounds);
            }
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state) {
            DrawButtonForHandle(g, bounds, focused, state, IntPtr.Zero);
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, bool focused, PushButtonState state) {
            DrawButton(g, bounds, buttonText, font,
                       TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                       focused, state);
        }


        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, bool focused, PushButtonState state) {
                Rectangle contentBounds;
                Color textColor;

                if (RenderWithVisualStyles) {
                    InitializeRenderer((int)state);

                    visualStyleRenderer.DrawBackground(g, bounds);
                    contentBounds = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                    textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
                }
                else {
                    ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                    contentBounds = Rectangle.Inflate(bounds, -3, -3);
                    textColor = SystemColors.ControlText;
                }

                TextRenderer.DrawText(g, buttonText, font, contentBounds, textColor, flags);

                if (focused) {
                    ControlPaint.DrawFocusRectangle(g, contentBounds);
                }
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state) {
                Rectangle contentBounds;

                if (RenderWithVisualStyles) {
                    InitializeRenderer((int)state);

                    visualStyleRenderer.DrawBackground(g, bounds);
                    visualStyleRenderer.DrawImage(g, imageBounds, image);
                    contentBounds = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                }
                else {
                    ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                    g.DrawImage(image, imageBounds);
                    contentBounds = Rectangle.Inflate(bounds, -3, -3);
                }

                if (focused) {
                    ControlPaint.DrawFocusRectangle(g, contentBounds);
                }
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, Image image, Rectangle imageBounds, bool focused, PushButtonState state) {
            DrawButton(g, bounds, buttonText, font,
                       TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                       image, imageBounds, focused, state);
        }

        /// <include file='doc\ButtonRenderer.uex' path='docs/doc[@for="ButtonRenderer.DrawButton6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Renders a Button control.
        ///    </para>
        /// </devdoc>
        public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, PushButtonState state) {
                Rectangle contentBounds;
                Color textColor;

                if (RenderWithVisualStyles) {
                    InitializeRenderer((int)state);

                    visualStyleRenderer.DrawBackground(g, bounds);
                    visualStyleRenderer.DrawImage(g, imageBounds, image);
                    contentBounds = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                    textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
                }
                else {
                    ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                    g.DrawImage(image, imageBounds);
                    contentBounds = Rectangle.Inflate(bounds, -3, -3);
                    textColor = SystemColors.ControlText;
                }

                TextRenderer.DrawText(g, buttonText, font, contentBounds, textColor, flags);

                if (focused) {
                    ControlPaint.DrawFocusRectangle(g, contentBounds);
                }
        }

        internal static ButtonState ConvertToButtonState(PushButtonState state) {
            switch (state) {

                case PushButtonState.Pressed:
                    return ButtonState.Pushed;
                case PushButtonState.Disabled:
                    return ButtonState.Inactive;

                default:
                    return ButtonState.Normal;
            }
        }

        private static void InitializeRenderer(int state) {
            if (visualStyleRenderer == null) {
                visualStyleRenderer = new VisualStyleRenderer(ButtonElement.ClassName, ButtonElement.Part, state);
            }
            else {
                visualStyleRenderer.SetParameters(ButtonElement.ClassName, ButtonElement.Part, state);
            }
        }
    }
}
