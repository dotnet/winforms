// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms 
{

    using System.Diagnostics;
    using System;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Windows.Forms.VisualStyles;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using Microsoft.Win32;

    /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs"]/*' />
    /// <devdoc>
    ///     This class contains the information a user needs to paint the ToolTip.
    /// </devdoc>
    public class DrawToolTipEventArgs : EventArgs 
    {

        private readonly Graphics graphics;
        private readonly IWin32Window associatedWindow;
        private readonly Control associatedControl;
        private readonly Rectangle bounds;
        private readonly string toolTipText;
        private readonly Color backColor;
        private readonly Color foreColor;
        private readonly Font font;
                
        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.DrawToolTipEventArgs"]/*' />
        /// <devdoc>
        ///     Creates a new DrawToolTipEventArgs with the given parameters.
        /// </devdoc>
        public DrawToolTipEventArgs(Graphics graphics, IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, 
				    Color backColor, Color foreColor, Font font) 
        {
            this.graphics = graphics; 
            this.associatedWindow = associatedWindow;
            this.associatedControl = associatedControl;
            this.bounds = bounds;
            this.toolTipText = toolTipText;
            this.backColor = backColor;
            this.foreColor = foreColor;
            this.font = font;
        }
        
        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///     Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics 
        {
            get 
            {
                return graphics;
            }
        }

        /// <include file='doc\PopupEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.AssociatedWindow"]/*' />
        /// <devdoc>
        ///     The window for which the tooltip is being painted. 
        /// </devdoc>
        public IWin32Window AssociatedWindow {
            get {
                return associatedWindow;
            }
        }
	
        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.AssociatedControl"]/*' />
        /// <devdoc>
        ///     The control for which the tooltip is being painted. 
        /// </devdoc>
        public Control AssociatedControl 
        {
            get 
            {
                return associatedControl;
            }
        }

        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.Bounds"]/*' />
        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds 
        {
            get 
            {
                return bounds;
            }
        }


        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.ToolTipText"]/*' />
        /// <devdoc>
        ///     The text that should be drawn.
        /// </devdoc>
        public string ToolTipText 
        {
            get 
            {
                return toolTipText;
            }
        }

        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.Font"]/*' />
        /// <devdoc>
        ///     The font used to draw tooltip text.
        /// </devdoc>
        public Font Font 
        {
            get 
            {
                return font;
            }
        }

        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.DrawBackground"]/*' />
        /// <devdoc>
        ///     Draws the background of the ToolTip.
        /// </devdoc>
        public void DrawBackground() 
        {
            Brush backBrush = new SolidBrush(backColor);
            Graphics.FillRectangle(backBrush, bounds);
            backBrush.Dispose();
        }

        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.DrawText"]/*' />
        /// <devdoc>
        ///     Draws the text (overloaded)
        /// </devdoc>
        public void DrawText() 
        {
            //Pass in a set of flags to mimic default behavior
            DrawText(TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.HidePrefix);
        }
	
        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.DrawText1"]/*' />
        /// <devdoc>
        ///     Draws the text (overloaded) - takes a TextFormatFlags argument.
        /// </devdoc>
        public void DrawText(TextFormatFlags flags) 
        {
            TextRenderer.DrawText(graphics, toolTipText, font, bounds, foreColor, flags);
        }

        /// <include file='doc\DrawToolTipEventArgs.uex' path='docs/doc[@for="DrawToolTipEventArgs.DrawBorder"]/*' />
        /// <devdoc>
        ///     Draws a border for the ToolTip similar to the default border.
        /// </devdoc>
        public void DrawBorder() 
        {
            ControlPaint.DrawBorder(graphics, bounds, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
        }
    }
}
