// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("StatusBarPanel has been deprecated.  Use the StatusStrip control instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
public class StatusBarPanel : Component, ISupportInitialize
{
    private const int DEFAULTMINWIDTH = 10;
    private const int PANELTEXTINSET = 3;
    private const int PANELGAP = 2;

    private string text = "";

    private HorizontalAlignment alignment = HorizontalAlignment.Left;
    private StatusBarPanelStyle style = StatusBarPanelStyle.Text;

    // these are package scope so the parent can get at them.
    private StatusBar parent;
    private int right;
    private int minWidth = DEFAULTMINWIDTH;
    private int index;
    private StatusBarPanelAutoSize autoSize = StatusBarPanelAutoSize.None;

    public StatusBarPanel()
    {
        throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public HorizontalAlignment Alignment
    {
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelAutoSize AutoSize
    {
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelBorderStyle BorderStyle
    {
        get
        {
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal bool Created
    {
        get
        {
            return parent is not null && parent.ArePanelsRealized();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Icon Icon
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int MinWidth
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string Name
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBar Parent
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal StatusBar ParentInternal
    {
        set
        {
            parent = value;
        }
    }

    internal int Right
    {
        get
        {
            return right;
        }
        set
        {
            right = value;
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelStyle Style
    {
        get { throw new PlatformNotSupportedException(); }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object Tag
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string Text
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string ToolTipText
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int Width
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void BeginInit()
    {
        throw new PlatformNotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (parent is not null)
            {
                int index = GetIndex();
                if (index != -1)
                {
                    parent.Panels.RemoveAt(index);
                }
            }
        }

        base.Dispose(disposing);
    }

    public void EndInit()
    {
        throw new PlatformNotSupportedException();
    }

    internal int GetContentsWidth(bool newPanel)
    {
        string text;
        if (newPanel)
        {
            if (this.text is null)
                text = "";
            else
                text = this.text;
        }
        else
            text = Text;

        Graphics g = parent.CreateGraphicsInternal();
        Size sz = Size.Ceiling(g.MeasureString(text, parent.Font));

        g.Dispose();

        int width = sz.Width + SystemInformation.BorderSize.Width * 2 + PANELTEXTINSET * 2 + PANELGAP;
        return Math.Max(width, minWidth);
    }

    private int GetIndex()
    {
        return index;
    }

    internal void Realize()
    {
        if (Created)
        {
            string text;
            string sendText;

            if (this.text is null)
            {
                text = "";
            }
            else
            {
                text = this.text;
            }

            HorizontalAlignment align = alignment;
            // Translate the alignment for Rtl apps
            if (parent.RightToLeft == RightToLeft.Yes)
            {
                switch (align)
                {
                    case HorizontalAlignment.Left:
                        align = HorizontalAlignment.Right;
                        break;
                    case HorizontalAlignment.Right:
                        align = HorizontalAlignment.Left;
                        break;
                }
            }

            switch (align)
            {
                case HorizontalAlignment.Center:
                    sendText = "\t" + text;
                    break;
                case HorizontalAlignment.Right:
                    sendText = "\t\t" + text;
                    break;
                default:
                    sendText = text;
                    break;
            }

            switch (style)
            {
                case StatusBarPanelStyle.Text:
                    break;
            }
        }
    }

    private void UpdateSize()
    {
        if (autoSize == StatusBarPanelAutoSize.Contents)
        {
            ApplyContentSizing();
        }
        else
        {
            if (Created)
            {
                parent.DirtyLayout();
                parent.PerformLayout();
            }
        }
    }

    private void ApplyContentSizing()
    {
        if (autoSize == StatusBarPanelAutoSize.Contents &&
            parent is not null)
        {
            int newWidth = GetContentsWidth(false);
            if (newWidth != Width)
            {
                Width = newWidth;
                if (Created)
                {
                    parent.DirtyLayout();
                    parent.PerformLayout();
                }
            }
        }
    }

    public override string ToString()
    {
        return "StatusBarPanel: {" + Text + "}";
    }
}
