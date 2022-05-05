// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using static Interop;

namespace System.Windows.Forms
{
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

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return _owningToolStripOverflowButton.ParentToolStrip is not null && _owningToolStripOverflowButton.ParentToolStrip.OverflowItems.Count > 0
                            ? _owningToolStripOverflowButton.DropDown.AccessibilityObject
                            : null;
                }

                return base.FragmentNavigate(direction);
            }
        }
    }
}
