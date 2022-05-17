// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripDropDownMenu : ToolStripDropDown
    {
        internal class ToolStripDropDownMenuAccessibleObject : ToolStripDropDownAccessibleObject
        {
            public ToolStripDropDownMenuAccessibleObject(ToolStripDropDownMenu owner) : base(owner)
            { }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent when Owner is ToolStripDropDownMenu menu
                        => menu.OwnerItem?.AccessibilityObject,
                    _ => base.FragmentNavigate(direction)
                };
        }
    }
}
