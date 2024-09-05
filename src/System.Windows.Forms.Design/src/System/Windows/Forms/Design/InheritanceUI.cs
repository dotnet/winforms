// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles the user interface for inherited components.
/// </summary>
internal class InheritanceUI
{
    private static Bitmap? s_inheritanceGlyph;
    private static Rectangle s_inheritanceGlyphRect;
    private ToolTip? _toolTip;

    /// <summary>
    ///  The bitmap we use to show inheritance.
    /// </summary>
    public static Bitmap InheritanceGlyph =>
        s_inheritanceGlyph ??= ScaleHelper.GetSmallIconResourceAsBitmap(
            typeof(InheritanceUI),
            "InheritedGlyph",
            ScaleHelper.InitialSystemDpi);

    /// <summary>
    ///  The rectangle surrounding the glyph.
    /// </summary>
    public static Rectangle InheritanceGlyphRectangle
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
    ///  Adds an inherited control to our list. This creates a tool tip for that control.
    /// </summary>
    public void AddInheritedControl(Control c, InheritanceLevel level)
    {
        _toolTip ??= new ToolTip
        {
            ShowAlways = true
        };

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

        _toolTip.SetToolTip(c, text);

        // Also, set all of its non-sited children
        foreach (Control child in c.Controls)
        {
            if (child.Site is null)
            {
                _toolTip.SetToolTip(child, text);
            }
        }
    }

    public void Dispose()
    {
        _toolTip?.Dispose();
    }

    /// <summary>
    ///  Removes a previously added inherited control.
    /// </summary>
    public void RemoveInheritedControl(Control c)
    {
        if (_toolTip is not null
            && !string.IsNullOrEmpty(_toolTip.GetToolTip(c)))
        {
            _toolTip.SetToolTip(c, caption: null);

            // Also, set all of its non-sited children
            foreach (Control child in c.Controls)
            {
                if (child.Site is null)
                {
                    _toolTip.SetToolTip(child, caption: null);
                }
            }
        }
    }
}
