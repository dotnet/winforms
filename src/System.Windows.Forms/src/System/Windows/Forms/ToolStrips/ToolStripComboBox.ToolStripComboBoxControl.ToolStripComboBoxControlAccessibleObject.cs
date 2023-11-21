// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

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

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            {
                switch (direction)
                {
                    case NavigateDirection.NavigateDirection_Parent:
                    case NavigateDirection.NavigateDirection_PreviousSibling:
                    case NavigateDirection.NavigateDirection_NextSibling:
                        if (this.TryGetOwnerAs(out ToolStripComboBoxControl? owner))
                        {
                            return owner.Owner?.AccessibilityObject.FragmentNavigate(direction);
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
                this.TryGetOwnerAs(out ToolStripComboBoxControl? owner)
                    ? owner.Owner?.Owner?.AccessibilityObject
                    : base.FragmentRoot;

            internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
            {
                UIA_PATTERN_ID.UIA_ExpandCollapsePatternId or UIA_PATTERN_ID.UIA_ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
        }
    }
}
