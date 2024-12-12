// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal class FocusableLabel : Label
{
    public bool UnderlineWhenFocused { get; set; } = true;

    public FocusableLabel()
    {
        SetStyle(ControlStyles.Selectable, true);
        TabStop = true;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassName = null;
            return createParams;
        }
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);

        if (UnderlineWhenFocused)
        {
            Focus();
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (UnderlineWhenFocused)
        {
            Font = new Font(Font, FontStyle.Underline);
        }

        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        if (UnderlineWhenFocused)
        {
            Font = new Font(Font, FontStyle.Regular);
        }

        base.OnLostFocus(e);
    }

    internal override bool SupportsUiaProviders => false;
}
