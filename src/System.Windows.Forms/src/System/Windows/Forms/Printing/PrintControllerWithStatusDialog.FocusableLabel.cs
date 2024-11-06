// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.LinkLabel;

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog
{
    private class FocusableLabel : LinkLabel
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

        protected override AccessibleObject CreateAccessibilityInstance() => new FocusableLabelAccessibleObject(this);
    }

    private class FocusableLabelAccessibleObject : LinkLabelAccessibleObject
    {
        public FocusableLabelAccessibleObject(LinkLabel owner) : base(owner) { }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        => direction switch
        {
            NavigateDirection.NavigateDirection_FirstChild
                => null,
            NavigateDirection.NavigateDirection_LastChild
                => null,
            _ => base.FragmentNavigate(direction),
        };

        public override int GetChildCount() => 0;
    }
}
