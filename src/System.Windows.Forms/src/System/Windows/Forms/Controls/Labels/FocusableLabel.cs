﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal partial class FocusableLabel : LinkLabel
{
    public FocusableLabel()
    {
        LinkBehavior = LinkBehavior.NeverUnderline;
        ActiveLinkColor = SystemColors.ControlText;
        LinkColor = SystemColors.ControlText;
        VisitedLinkColor = SystemColors.ControlText;
    }

    protected override void WndProc(ref Message msg)
    {
        switch (msg.MsgInternal)
        {
            case PInvokeCore.WM_SETCURSOR:
                break;
            default:
                base.WndProc(ref msg);
                break;
        }
    }

    internal override AccessibleRole LinkAccessibleRole => AccessibleRole.StaticText;

    protected override AccessibleObject CreateAccessibilityInstance() => new FocusableLabelAccessibleObject(this);
}
