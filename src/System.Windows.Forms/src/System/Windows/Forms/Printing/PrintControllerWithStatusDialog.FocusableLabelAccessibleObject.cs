// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.LinkLabel;

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog
{
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
