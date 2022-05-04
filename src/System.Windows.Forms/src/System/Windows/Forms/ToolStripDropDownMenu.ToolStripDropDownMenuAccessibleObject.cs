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
            private readonly ToolStripDropDownMenu _owningToolStripDropDownMenu;

            public ToolStripDropDownMenuAccessibleObject(ToolStripDropDownMenu owner) : base(owner)
            {
                _owningToolStripDropDownMenu = owner;
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => _owningToolStripDropDownMenu.OwnerItem?.AccessibilityObject,
                    UiaCore.NavigateDirection.FirstChild => GetFirstChild(),
                    UiaCore.NavigateDirection.LastChild => GetLastChild(),
                    _ => base.FragmentNavigate(direction)
                };

            private AccessibleObject? GetFirstChild()
                => _owningToolStripDropDownMenu.Items.Count > 0
                    ? _owningToolStripDropDownMenu.DisplayedItems.Contains(_owningToolStripDropDownMenu.UpScrollButton)
                        ? _owningToolStripDropDownMenu.UpScrollButton.AccessibilityObject
                        : _owningToolStripDropDownMenu.Items[0].AccessibilityObject
                    : null;

            private AccessibleObject? GetLastChild()
                => _owningToolStripDropDownMenu.Items.Count > 0
                    ? _owningToolStripDropDownMenu.DisplayedItems.Contains(_owningToolStripDropDownMenu.DownScrollButton)
                        ? _owningToolStripDropDownMenu.DownScrollButton.AccessibilityObject
                        : _owningToolStripDropDownMenu.Items[^1].AccessibilityObject
                    : null;
        }
    }
}
