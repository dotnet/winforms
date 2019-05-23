// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms { 

using System;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

    /// <summary>
    ///    <para>
    ///       This is a rendering class for the TrackBar control.
    ///    </para>
    /// </summary>
    public sealed class TrackBarRenderer {

       //Make this per-thread, so that different threads can safely use these methods.
       [ThreadStatic]
       private static VisualStyleRenderer visualStyleRenderer = null;
       const int lineWidth = 2;

        //cannot instantiate
       private TrackBarRenderer() {
       }

       /// <summary>
       ///    <para>
       ///       Returns true if this class is supported for the current OS and user/application settings, 
       ///       otherwise returns false.
       ///    </para>
       /// </summary>
       public static bool IsSupported {
           get {
               return VisualStyleRenderer.IsSupported; // no downlevel support
           }
       }

       /// <summary>
       ///    <para>
       ///       Renders a horizontal track.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawHorizontalTrack(Graphics g, Rectangle bounds) {
           InitializeRenderer(VisualStyleElement.TrackBar.Track.Normal, 1);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a vertical track.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawVerticalTrack(Graphics g, Rectangle bounds) {
           InitializeRenderer(VisualStyleElement.TrackBar.TrackVertical.Normal, 1);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a horizontal thumb.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawHorizontalThumb(Graphics g, Rectangle bounds, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.Thumb.Normal, (int)state);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a vertical thumb.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawVerticalThumb(Graphics g, Rectangle bounds, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbVertical.Normal, (int)state);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a constant size left pointing thumb centered in the given bounds.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawLeftPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbLeft.Normal, (int)state);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a constant size right pointing thumb centered in the given bounds.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawRightPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbRight.Normal, (int)state);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a constant size top pointing thumb centered in the given bounds.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawTopPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbTop.Normal, (int)state);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a constant size bottom pointing thumb centered in the given bounds.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawBottomPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbBottom.Normal, (int)state);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <summary>
       ///    <para>
       ///       Renders a horizontal tick.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")] // PM team has reviewed and decided on naming changes already
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static void DrawHorizontalTicks(Graphics g, Rectangle bounds, int numTicks, EdgeStyle edgeStyle) {
           if (numTicks <= 0 || bounds.Height <= 0 || bounds.Width <= 0 || g == null) {
               return;
           }

           InitializeRenderer(VisualStyleElement.TrackBar.Ticks.Normal, 1);

           //trivial case -- avoid calcs
           if (numTicks == 1) {
               visualStyleRenderer.DrawEdge(g, new Rectangle(bounds.X, bounds.Y, lineWidth, bounds.Height), Edges.Left, edgeStyle, EdgeEffects.None);
               return;
           }

           float inc = ((float)bounds.Width - lineWidth) / ((float)numTicks - 1);

           while (numTicks > 0) {
               //draw the nth tick
               float x = bounds.X + ((float)(numTicks - 1)) * inc;
               visualStyleRenderer.DrawEdge(g, new Rectangle((int)Math.Round(x), bounds.Y, lineWidth, bounds.Height), Edges.Left, edgeStyle, EdgeEffects.None);
               numTicks--;
           }
       }

       /// <summary>
       ///    <para>
       ///       Renders a vertical tick.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]  // PM team has reviewed and decided on naming changes already
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawVerticalTicks(Graphics g, Rectangle bounds, int numTicks, EdgeStyle edgeStyle) {
            if (numTicks<=0 || bounds.Height <= 0 || bounds.Width<=0 || g == null ) {
               return;
           }

           InitializeRenderer(VisualStyleElement.TrackBar.TicksVertical.Normal, 1);

           //trivial case
           if (numTicks == 1) {
               visualStyleRenderer.DrawEdge(g, new Rectangle(bounds.X, bounds.Y, bounds.Width, lineWidth), Edges.Top, edgeStyle, EdgeEffects.None);
               return;
           }

           float inc = ((float)bounds.Height - lineWidth) / ((float)numTicks - 1);

           while (numTicks > 0) {
               //draw the nth tick
               float y = bounds.Y + ((float)(numTicks - 1)) * inc;
               visualStyleRenderer.DrawEdge(g, new Rectangle(bounds.X, (int)Math.Round(y), bounds.Width, lineWidth), Edges.Top, edgeStyle, EdgeEffects.None);
               numTicks--;
           }
       }

       /// <summary>
       ///    <para>
       ///       Returns the size of a left pointing thumb.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static Size GetLeftPointingThumbSize(Graphics g, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbLeft.Normal, (int)state);

           return (visualStyleRenderer.GetPartSize(g, ThemeSizeType.True));
       }

       /// <summary>
       ///    <para>
       ///       Returns the size of a right pointing thumb.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static Size GetRightPointingThumbSize(Graphics g, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbRight.Normal, (int)state);

           return (visualStyleRenderer.GetPartSize(g, ThemeSizeType.True));
       }

       /// <summary>
       ///    <para>
       ///       Returns the size of a top pointing thumb.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static Size GetTopPointingThumbSize(Graphics g, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbTop.Normal, (int)state);

           return (visualStyleRenderer.GetPartSize(g, ThemeSizeType.True));
       }

       /// <summary>
       ///    <para>
       ///       Returns the size of a bottom pointing thumb.
       ///    </para>
       /// </summary>
       [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
       public static Size GetBottomPointingThumbSize(Graphics g, TrackBarThumbState state) {
           InitializeRenderer(VisualStyleElement.TrackBar.ThumbBottom.Normal, (int)state);

           return (visualStyleRenderer.GetPartSize(g, ThemeSizeType.True));
       }

       private static void InitializeRenderer(VisualStyleElement element, int state) {
           if (visualStyleRenderer == null) {
               visualStyleRenderer = new VisualStyleRenderer(element.ClassName, element.Part, state);
           }
           else {
               visualStyleRenderer.SetParameters(element.ClassName, element.Part, state);
           }
       }
    }
}
