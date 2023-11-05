// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripOverflowButton
{
    internal class ToolStripOverflowButtonAccessibleObject : ToolStripDropDownItemAccessibleObject
    {
        private readonly ToolStripOverflowButton _owningToolStripOverflowButton;

        public ToolStripOverflowButtonAccessibleObject(ToolStripOverflowButton owner) : base(owner)
        {
            _owningToolStripOverflowButton = owner;
        }

        [AllowNull]
        public override string Name
        {
            get => Owner.AccessibleName ?? SR.ToolStripOptions;
            set => base.Name = value;
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.NavigateDirection_FirstChild:
                case NavigateDirection.NavigateDirection_LastChild:
                    // Don't show the inner menu while it is invisible.
                    // Otherwise it will affect accessibility tree,
                    // especially for items-controls that have not been created yet.
                    return _owningToolStripOverflowButton.DropDown.Visible
                        ? _owningToolStripOverflowButton.DropDown.AccessibilityObject
                        : null;
            }

            return base.FragmentNavigate(direction);
        }
    }
}
