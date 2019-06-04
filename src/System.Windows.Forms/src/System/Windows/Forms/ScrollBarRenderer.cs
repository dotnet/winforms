// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{

    using System;
    using System.Drawing;
    using System.Windows.Forms.VisualStyles;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Win32;

    /// <summary>
    ///    <para>
    ///       This is a rendering class for the ScrollBar control.
    ///    </para>
    /// </summary>
    public sealed class ScrollBarRenderer
    {

        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;

        //cannot instantiate
        private ScrollBarRenderer()
        {
        }

        /// <summary>
        ///    <para>
        ///       Returns true if this class is supported for the current OS and user/application settings, 
        ///       otherwise returns false.
        ///    </para>
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                return VisualStyleRenderer.IsSupported; // no downlevel support
            }
        }

        /// <summary>
        ///    <para>
        ///       Renders a ScrollBar arrow button.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawArrowButton(Graphics g, Rectangle bounds, ScrollBarArrowButtonState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.ArrowButton.LeftNormal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a horizontal ScrollBar thumb.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawHorizontalThumb(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a vertical ScrollBar thumb.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawVerticalThumb(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.ThumbButtonVertical.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a horizontal ScrollBar thumb grip.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawHorizontalThumbGrip(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.GripperHorizontal.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a vertical ScrollBar thumb grip.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawVerticalThumbGrip(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.GripperVertical.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a horizontal ScrollBar thumb.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawRightHorizontalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a horizontal ScrollBar thumb.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawLeftHorizontalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a vertical ScrollBar thumb in the center of the given bounds.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawUpperVerticalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.UpperTrackVertical.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a vertical ScrollBar thumb in the center of the given bounds.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawLowerVerticalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.LowerTrackVertical.Normal, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Renders a ScrollBar size box in the center of the given bounds.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawSizeBox(Graphics g, Rectangle bounds, ScrollBarSizeBoxState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.SizeBox.LeftAlign, (int)state);

            visualStyleRenderer.DrawBackground(g, bounds);
        }

        /// <summary>
        ///    <para>
        ///       Returns the size of the ScrollBar thumb grip.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static Size GetThumbGripSize(Graphics g, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.GripperHorizontal.Normal, (int)state);

            return visualStyleRenderer.GetPartSize(g, ThemeSizeType.True);
        }

        /// <summary>
        ///    <para>
        ///       Returns the size of the ScrollBar size box.
        ///    </para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static Size GetSizeBoxSize(Graphics g, ScrollBarState state)
        {
            InitializeRenderer(VisualStyleElement.ScrollBar.SizeBox.LeftAlign, (int)state);

            return visualStyleRenderer.GetPartSize(g, ThemeSizeType.True);
        }

        private static void InitializeRenderer(VisualStyleElement element, int state)
        {
            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(element.ClassName, element.Part, state);
            }
            else
            {
                visualStyleRenderer.SetParameters(element.ClassName, element.Part, state);
            }
        }
    }
}
