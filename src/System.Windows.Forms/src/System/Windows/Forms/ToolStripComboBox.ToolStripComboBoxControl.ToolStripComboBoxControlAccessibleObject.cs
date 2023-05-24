// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripComboBox
{
    internal partial class ToolStripComboBoxControl : ComboBox
    {
        internal class ToolStripComboBoxControlAccessibleObject : ComboBoxAccessibleObject
        {
            public ToolStripComboBoxControlAccessibleObject(ToolStripComboBoxControl toolStripComboBoxControl)
                : base(toolStripComboBoxControl)
            {
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                    case UiaCore.NavigateDirection.PreviousSibling:
                    case UiaCore.NavigateDirection.NextSibling:
                        if (this.TryGetOwnerAs(out ToolStripComboBoxControl? owner))
                        {
                            return owner.Owner?.AccessibilityObject.FragmentNavigate(direction);
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
                => this.TryGetOwnerAs(out ToolStripComboBoxControl? owner)
                    ? owner.Owner?.Owner?.AccessibilityObject
                    : base.FragmentRoot;

            internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
            {
                UiaCore.UIA.ExpandCollapsePatternId or UiaCore.UIA.ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
        }
    }
}
