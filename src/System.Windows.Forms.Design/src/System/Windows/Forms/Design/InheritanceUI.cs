// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class handles the user interface for inherited components.
    /// </summary>
    internal class InheritanceUI
    {
        private static Bitmap s_inheritanceGlyph;
        private static Rectangle s_inheritanceGlyphRect;
        private ToolTip _tooltip;

        /// <summary>
        ///  The bitmap we use to show inheritance.
        /// </summary>
        public Bitmap InheritanceGlyph
        {
            get
            {
                if (s_inheritanceGlyph == null)
                {
                    s_inheritanceGlyph = new Icon(typeof(InheritanceUI), "InheritedGlyph").ToBitmap();

                    if (DpiHelper.IsScalingRequired)
                    {
                        DpiHelper.ScaleBitmapLogicalToDevice(ref s_inheritanceGlyph);
                    }
                }
                return s_inheritanceGlyph;
            }
        }

        /// <summary>
        ///  The rectangle surrounding the glyph.
        /// </summary>
        public Rectangle InheritanceGlyphRectangle
        {
            get
            {
                if (s_inheritanceGlyphRect == Rectangle.Empty)
                {
                    Size size = InheritanceGlyph.Size;
                    s_inheritanceGlyphRect = new Rectangle(0, 0, size.Width, size.Height);
                }
                return s_inheritanceGlyphRect;
            }
        }

        /// <summary>
        ///  Adds an inherited control to our list.  This creates a tool tip for that control.
        /// </summary>
        public void AddInheritedControl(Control c, InheritanceLevel level)
        {
            if (_tooltip == null)
            {
                _tooltip = new ToolTip
                {
                    ShowAlways = true
                };
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

            _tooltip.SetToolTip(c, text);

            // Also, set all of its non-sited children
            foreach (Control child in c.Controls)
            {
                if (child.Site == null)
                {
                    _tooltip.SetToolTip(child, text);
                }
            }
        }

        public void Dispose()
        {
            if (_tooltip != null)
            {
                _tooltip.Dispose();
            }
        }

        /// <summary>
        ///  Removes a previously added inherited control.
        /// </summary>
        public void RemoveInheritedControl(Control c)
        {
            if (_tooltip != null && _tooltip.GetToolTip(c).Length > 0)
            {
                _tooltip.SetToolTip(c, null);
                // Also, set all of its non-sited children
                foreach (Control child in c.Controls)
                {
                    if (child.Site == null)
                    {
                        _tooltip.SetToolTip(child, null);
                    }
                }
            }
        }
    }
}
