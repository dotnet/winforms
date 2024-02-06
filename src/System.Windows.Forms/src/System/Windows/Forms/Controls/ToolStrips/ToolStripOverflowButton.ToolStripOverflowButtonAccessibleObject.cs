// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripOverflowButton
{
    internal sealed class ToolStripOverflowButtonAccessibleObject : ToolStripDropDownItemAccessibleObject
    {
        private readonly ToolStripOverflowButton _owningToolStripOverflowButton;

        public ToolStripOverflowButtonAccessibleObject(ToolStripOverflowButton owner) : base(owner)
        {
            _owningToolStripOverflowButton = owner;
        }

        [AllowNull]
        public override string Name => Owner.AccessibleName ?? SR.ToolStripOptions;

        private protected override bool IsInternal => true;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild or NavigateDirection.NavigateDirection_LastChild
                    => _owningToolStripOverflowButton.DropDown.Visible
                        ? _owningToolStripOverflowButton.DropDown.AccessibilityObject
                        : null,
                // Don't show the inner menu while it is invisible.
                // Otherwise it will affect accessibility tree,
                // especially for items-controls that have not been created yet.
                _ => base.FragmentNavigate(direction),
            };
    }
}
