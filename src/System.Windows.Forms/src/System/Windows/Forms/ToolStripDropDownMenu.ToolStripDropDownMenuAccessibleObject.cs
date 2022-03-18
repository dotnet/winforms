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
            private readonly ToolStripDropDownMenu _owningToolStrip;

            public ToolStripDropDownMenuAccessibleObject(ToolStripDropDownMenu owner) : base(owner)
            {
                _owningToolStrip = owner;
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => _owningToolStrip.OwnerItem?.AccessibilityObject,
                    UiaCore.NavigateDirection.FirstChild => _owningToolStrip.Items.Count > 0 ? _owningToolStrip.Items[0].AccessibilityObject : null,
                    UiaCore.NavigateDirection.LastChild => _owningToolStrip.Items.Count > 0 ? _owningToolStrip.Items[^1].AccessibilityObject : null,
                    _ => base.FragmentNavigate(direction)
                };
            }
        }
    }
}
