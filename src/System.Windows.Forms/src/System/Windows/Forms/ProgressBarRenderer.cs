// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms { 

using System;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

    /// <include file='doc\ProgressBarRenderer.uex' path='docs/doc[@for="ProgressBarRenderer"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This is a rendering class for the ProgressBar control.
    ///    </para>
    /// </devdoc>
    public sealed class ProgressBarRenderer {

       //Make this per-thread, so that different threads can safely use these methods.
       [ThreadStatic]
       private static VisualStyleRenderer visualStyleRenderer = null;
       
        //cannot instantiate
       private ProgressBarRenderer() {
       }

       /// <include file='doc\ProgressBarRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.IsSupported"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Returns true if this class is supported for the current OS and user/application settings, 
       ///       otherwise returns false.
       ///    </para>
       /// </devdoc>
       public static bool IsSupported {
           get {
               return VisualStyleRenderer.IsSupported; // no downlevel support
           }
       }

       /// <include file='doc\ProgressBarRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.DrawHorizontalBar"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Renders a horizontal bar.
       ///    </para>
       /// </devdoc>
       [
           SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // Using Graphics instead of IDeviceContext intentionally
       ]
       public static void DrawHorizontalBar(Graphics g, Rectangle bounds) {
           InitializeRenderer(VisualStyleElement.ProgressBar.Bar.Normal);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <include file='doc\ProgressBarRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.DrawVerticalBar"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Renders a vertical bar.
       ///    </para>
       /// </devdoc>
       [
           SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // Using Graphics instead of IDeviceContext intentionally
       ]
       public static void DrawVerticalBar(Graphics g, Rectangle bounds) {
           InitializeRenderer(VisualStyleElement.ProgressBar.BarVertical.Normal);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <include file='doc\ProgressBarRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.DrawHorizontalChunks"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Renders a number of constant size horizontal chunks in the given bounds.
       ///    </para>
       /// </devdoc>
       [
           SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // Using Graphics instead of IDeviceContext intentionally
       ]
       public static void DrawHorizontalChunks(Graphics g, Rectangle bounds) {
           InitializeRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <include file='doc\ProgressBarRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.DrawVerticalChunks"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Renders a number of constant size vertical chunks in the given bounds.
       ///    </para>
       /// </devdoc>
       [
           SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters") // Using Graphics instead of IDeviceContext intentionally
       ]
       public static void DrawVerticalChunks(Graphics g, Rectangle bounds) {
           InitializeRenderer(VisualStyleElement.ProgressBar.ChunkVertical.Normal);

           visualStyleRenderer.DrawBackground(g, bounds);
       }

       /// <include file='doc\TextBoxRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.GetChunkThickness"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Returns the  width/height of a single horizontal/vertical progress bar chunk.
       ///    </para>
       /// </devdoc>
       public static int ChunkThickness {
           get {
               InitializeRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);
    
               return (visualStyleRenderer.GetInteger(IntegerProperty.ProgressChunkSize));
           }
       }

       /// <include file='doc\TextBoxRenderer.uex' path='docs/doc[@for="ProgressBarRenderer.GetChunkSpaceThickness"]/*' />
       /// <devdoc>
       ///    <para>
       ///       Returns the  width/height of the space between horizontal/vertical progress bar chunks.
       ///    </para>
       /// </devdoc>
       public static int ChunkSpaceThickness {
           get {
               InitializeRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);
    
               return (visualStyleRenderer.GetInteger(IntegerProperty.ProgressSpaceSize));
           }
       }

       private static void InitializeRenderer(VisualStyleElement element) {
           if (visualStyleRenderer == null) {
               visualStyleRenderer = new VisualStyleRenderer(element);
           }
           else {
               visualStyleRenderer.SetParameters(element);
           }
       }
    }
}
