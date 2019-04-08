// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// This class handles the user interface for inherited components.
    /// </summary>
    internal class InheritanceUI
    {
        private static Bitmap inheritanceGlyph;
        private static Rectangle inheritanceGlyphRect;
        private ToolTip tooltip;

        /// <summary>
        /// The bitmap we use to show inheritance.
        /// </summary>
        public Bitmap InheritanceGlyph
        {
            get
            {
                if (inheritanceGlyph == null)
                {
                    inheritanceGlyph = new Bitmap(typeof(InheritanceUI), "InheritedGlyph.bmp");
                    inheritanceGlyph.MakeTransparent();
                    if (DpiHelper.IsScalingRequired)
                    {
                        DpiHelper.ScaleBitmapLogicalToDevice(ref inheritanceGlyph);
                    }
                }
                return inheritanceGlyph;
            }
        }

        /// <summary>
        /// The rectangle surrounding the glyph.
        /// </summary>
        public Rectangle InheritanceGlyphRectangle
        {
            get
            {
                if (inheritanceGlyphRect == Rectangle.Empty)
                {
                    Size size = InheritanceGlyph.Size;
                    inheritanceGlyphRect = new Rectangle(0, 0, size.Width, size.Height);
                }
                return inheritanceGlyphRect;
            }
        }

        /// <summary>
        /// Adds an inherited control to our list.  This creates a tool tip for that control.
        /// </summary>
        public void AddInheritedControl(Control c, InheritanceLevel level)
        {
            if (tooltip == null)
            {
                tooltip = new ToolTip();
                tooltip.ShowAlways = true;
            }

            Debug.Assert(level != InheritanceLevel.NotInherited, "This should only be called for inherited components.");
            string text;
            if (level == InheritanceLevel.InheritedReadOnly)
            {
                text = SR.DesignerInheritedReadOnly;
            }
            else
            {
                text = SR.DesignerInherited;
            }

            tooltip.SetToolTip(c, text);
            foreach (Control child in c.Controls)
            {
                if (child.Site == null)
                {
                    tooltip.SetToolTip(child, text);
                }
            }
        }

        public void Dispose()
        {
            if (tooltip != null)
            {
                tooltip.Dispose();
            }
        }

        /// <summary>
        /// Removes a previously added inherited control.
        /// </summary>
        public void RemoveInheritedControl(Control c)
        {
            if (tooltip != null && tooltip.GetToolTip(c).Length > 0)
            {
                tooltip.SetToolTip(c, null);
                foreach (Control child in c.Controls)
                {
                    if (child.Site == null)
                    {
                        tooltip.SetToolTip(child, null);
                    }
                }
            }
        }
    }
}
